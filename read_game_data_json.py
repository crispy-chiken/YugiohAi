import datetime
import glob
import json
import math
import os
import shutil
import sqlite3
import string
import subprocess
import sys
import time
import random
import typing
import csv
import numpy as np
import pickle
import itertools
import scipy
import scipy.special
from sklearn.model_selection import train_test_split
from sklearn.metrics import accuracy_score

import torch
import numpy as np
import torch.nn
from torch.utils.data import Dataset, DataLoader
import torch.nn as nn
import torch.nn.functional as F
from torch.utils.data.dataloader import default_collate

from sys import platform
from pathlib import Path

TrainAll = (len(sys.argv)>1 and ("--a" in sys.argv or "-a" in sys.argv))
print("Train all " + str(TrainAll))
ShowData = True
ShowAcc = False

#Torch settings
dtype = torch.float
device = torch.device('cuda' if torch.cuda.is_available() else 'cpu')
print('Using device:', device)
print()

action_count = 0
state_count = 0


# Torch Classes

class Data(Dataset):
  def __init__(self, X_train, y_train):
    super(Data, self).__init__()
    # need to convert float64 to float32 else 
    # will get the following error
    # RuntimeError: expected scalar type Double but found Float
    self.X = torch.from_numpy(X_train).type(torch.FloatTensor).to(device)
    # need to convert float64 to Long else 
    # will get the following error
    # RuntimeError: expected scalar type Long but found Float
    self.y = torch.from_numpy(y_train).type(torch.FloatTensor).to(device)
    self.len = self.X.shape[0]
  
  def __getitem__(self, index):
    return self.X[index], self.y[index]
  
  def __len__(self):
    return self.len
    
class Network(nn.Module):
  def __init__(self, input_dim, output_dim):
    super(Network, self).__init__()
    hidden_layers = 2 #(input_dim + output_dim)

    self.layer1 = nn.Linear(input_dim, hidden_layers)
    self.layer2 = nn.Linear(hidden_layers, hidden_layers)
    self.output = nn.Linear(hidden_layers, output_dim)
    #self.single = nn.Linear(input_dim, output_dim)

    #self.output.bias = nn.Parameter(bias)
    self.dropout1 = nn.Dropout(0.7)
    self.dropout2 = nn.Dropout(0.2)
    self.act1 = nn.Tanh() # Weights tend to be lower, messes up on new data, but somewhat consistant on familiar states, probably not good
    self.act2 = nn.ReLU() # Seems ok, never reaches negative values,
    self.act0 = nn.Sigmoid() # Might get multiple choices
    self.act3 = nn.LeakyReLU() # Ususaly very high on prediction weights and can be multiples, but can randomy put 1s on actions it has never performed, also too egar

    #print(self.layer1.weight)
    #print(self.layer1.weight)

  def forward(self, x):
    #return self.single(x)
    x = self.layer1(x)
    x = self.act0(x)

    # x = self.layer2(x)
    # x = self.act2(x)

    x = self.output(x)
    #x = self.act0(x)
    return x

class NetworkCritic(nn.Module):
  def __init__(self, input_dim):
    super(NetworkCritic, self).__init__()
    hidden_layers = (input_dim) * 1

    self.layer1 = nn.Linear(input_dim, hidden_layers)
    self.layer2 = nn.Linear(hidden_layers, hidden_layers)
    self.output = nn.Linear(hidden_layers, 1)
    self.single = nn.Linear(input_dim, 1)

    #self.output.bias = nn.Parameter(bias)
    self.dropout1 = nn.Dropout(0.7)
    self.dropout2 = nn.Dropout(0.2)
    self.act1 = nn.Tanh() # Weights tend to be lower, messes up on new data, but somewhat consistant on familiar states, probably not good
    self.act2 = nn.ReLU() # Seems ok, never reaches negative values,
    self.act0 = nn.Sigmoid() # Might get multiple choices
    self.act3 = nn.LeakyReLU() # Ususaly very high on prediction weights and can be multiples, but can randomy put 1s on actions it has never performed, also too egar

    #print(self.layer1.weight)
    #print(self.layer1.weight)

  def forward(self, x):
    #return self.single(x)
    x = self.layer1(x)
    #x = self.dropout1(x)
    x = self.act2(x)

    # x = self.layer2(x)
    # x = self.act3(x)

    x = self.output(x)
    #x = self.act0(x)
    return x


# Data base Classes

class Action:
  def __init__(self, id, name, action) -> None:
    self.id = id
    self.name = name
    self.action = action

  def __str__(self) -> str:
    return str(self.name + " " + self.action)

class CompareTo:
  def __init__(self, id, location, compare, value) -> None:
    self.id = id
    self.location = location
    self.compare = compare
    self.value = value

  def __str__(self) -> str:
    return f"({self.id}) " + str(self.location + " " + self.compare + " " + self.value)
  
def deleteData():
  global ShowData
  folder = './data'
  for filename in os.listdir(folder):
      file_path = os.path.join(folder, filename)
      try:
          if os.path.isfile(file_path) or os.path.islink(file_path):
              os.unlink(file_path)
          elif os.path.isdir(file_path):
              shutil.rmtree(file_path)
      except Exception as e:
          print('Failed to delete %s. Reason: %s' % (file_path, e))
  os.mkdir('./data/critic')

def clearLocalData():
  global action_list, action_state, compare_to, field_state, play_record, game_result
  
  action_list = {}
  action_state = {}
  compare_to = {}
  field_state = {}
  play_record = {}
  game_result = {}

def getTorchData():
  global action_count, state_count
  action_data = {}
  critic_data = {}
  directory = './data'
  for filename in os.listdir(directory):
    f = os.path.join(directory, filename)
    if os.path.isfile(f):
        clf = Network(action_count + state_count + 1 + 1, action_count + 1)
        #clf = Network(input_length + output_length, 1, getBias(filename))
        clf.load_state_dict(torch.load(f))
        clf.to(device)
        clf.eval()
        action_data[filename] = clf
  directory = './data/critic'
  for filename in os.listdir(directory):
    f = os.path.join(directory, filename)
    if os.path.isfile(f):
        clf = NetworkCritic(action_count + state_count + 1 + 1)
        clf.load_state_dict(torch.load(f))
        clf.to(device)
        clf.eval()
        critic_data[filename] = clf
  
  return action_data, critic_data

def getTorchPrediction(action_data, input_list, multi = False):
  final_result = {}
  with torch.no_grad():
    for key in action_data.keys():
        torch_data = torch.from_numpy(np.array(input_list)).to(device).float()
        result = action_data[key](torch_data)
        #result = torch.softmax(result,1)
        result = torch.sigmoid(result)
        result = result.cpu().data.numpy()
        if multi:
          final_result[key] = result
        else: 
          final_result[key] = result[0]

  return final_result

def trainTorch(x_train, y_train, x_test, y_test, name):
  global action_count, state_count
  traindata = Data(np.array(x_train), np.array(y_train))
  batch_size = len(y_train)#min(64, len(y_train))#
  trainloader = DataLoader(traindata, batch_size=batch_size, shuffle=True)
  print("Batch size " + str(batch_size))
  clf = Network(action_count + state_count + 1 + 1, action_count + 1).to(device)
  criterion2 = nn.BCEWithLogitsLoss().cuda()
  criterion = nn.CrossEntropyLoss().cuda()
  #criterion2 = nn.MultiLabelMarginLoss().cuda()
  optimizer = torch.optim.Adam(clf.parameters(), lr=0.001)#, weight_decay=1e-5)
  optimizer2 = torch.optim.AdamW(clf.parameters(), lr=0.001)#, weight_decay=1e-5)
  #optimizer2 = torch.optim.SGD(clf.parameters(), lr=0.01)
  epochs = 40
  for epoch in range(epochs):
    y_true = []
    y_pred = []
    running_loss = 0.0
    train_start_time = time.time()
    for i, data in enumerate(trainloader):
      inputs, labels = data
      #inputs, labels = inputs.to(device), labels.to(device).float()
      
      clf.train()

      # forward propagation
      outputs = clf(inputs)
      #outputs = torch.sigmoid(outputs)
      #outputs = torch.softmax(outputs, 1)

      # Filter out indexes to be only values we want to train
      # Probably donesnt work the way I think it does
      #cur_labels = labels.cpu()

      #mask = (labels.cpu() != -1).to(device)
      mask = (labels != -1)
      # indexes = np.argwhere(labels.cpu() != -1)
      outputs2 = outputs.masked_select(mask)
      labels2 = labels.masked_select(mask)
      
      # mask[mask == False] = 0.00
      # outputs3 = outputs * (mask)

      # loss = criterion(outputs, labels.argmax(1)) # For CrossEntropyLoss

      # set optimizer to zero grad to remove previous epoch gradients
      # optimizer.zero_grad()
      # # for param in clf.parameters():
      # #   param.grad = None
      # # backward propagation
      # loss.backward()
      # # optimize
      # optimizer.step()
      # running_loss += loss.item()

      loss = criterion2(outputs2, labels2.float())
      optimizer2.zero_grad()
      loss.backward()
      optimizer2.step()
      running_loss += loss.item()

      #PREDICTIONS 
      clf.eval()
      with torch.no_grad():
        #outputs = torch.sigmoid(outputs) * mask
        pred = outputs.cpu().detach().numpy()
        labels = labels.cpu().detach().numpy()    
        # y_pred = pred.tolist()
        # y_true = labels.tolist()
        y_true.extend(labels.tolist())
        y_pred.extend(pred.tolist())
    
    if epoch % 10 == 9:
      # display statistics
      
      y_pred = [np.argmax(i) for i in y_pred]
      y_true = [np.argmax(i) for i in y_true]
      
      #print(f"[{epoch + 1}, {i + 1:5d}]Accuracy on training set is"+" {} / {} with accuracy {}".format(num_correct, num_samples, float(num_correct)/float(num_samples)*100))
      print(f"[{epoch + 1}, {i + 1:5d}]Accuracy on training set is " + str(accuracy_score(np.array(y_true),np.array(y_pred))))
      print(f'[{epoch + 1}, {i + 1:5d}] loss: {running_loss / (i + 1):.5f}')
    
    if (running_loss / (i + 1)) < 0.0005:
      print("loss is very small, skipping rest")
      break
  
  #PREDICTIONS 

  with torch.no_grad():
    clf.eval()
    
    y_pred = torch.sigmoid(clf(torch.from_numpy(np.array(x_test)).to(device).float()))
    #y_pred = torch.softmax(y_pred.cpu().detach().numpy(), 1)


    y_test = np.array(y_test)
    mask = (y_test != -1)
    y_test *= mask
    #y_test[y_test==0] = -1
    y_pred = y_pred.cpu()
    y_pred = torch.Tensor([np.argmax(i) for i in y_pred])
    y_test = torch.Tensor([np.argmax(i) for i in y_test])
    num_correct = (y_pred == y_test).sum()
    num_samples = y_pred.size(0)
    # predictions = (y_pred > 0.45).long()
    # num_correct = (predictions == torch.Tensor(y_test)).sum()
    # num_samples = predictions.size(0) * predictions.size(1)

    print("Got {} / {} with accuracy {}".format(num_correct, num_samples, float(num_correct)/float(num_samples)*100))

    #print(f"Accuracy on test set is " + str(accuracy_score(np.array(y_test),np.array(y_pred))))

  PATH = "./data/" + name
  torch.save(clf.state_dict(), PATH)


def trainCritic(x_train, y_train, x_test, y_test, name):
  global action_count, state_count
  traindata = Data(np.array(x_train), np.array(y_train))
  batch_size = len(y_train)#min(40, len(y_train))#
  # trainloader = DataLoader(traindata, batch_size=batch_size, shuffle=True, collate_fn=lambda x: tuple(x_.to(device) for x_ in default_collate(x)))
  trainloader = DataLoader(traindata, batch_size=batch_size, shuffle=True)
  # trainloader = DataLoader(traindata, batch_size=batch_size, shuffle=True, num_workers=4, 
  #                          persistent_workers=True
  #                          )
  print("Batch size " + str(batch_size))
  criterion = nn.BCEWithLogitsLoss().cuda()

  clf = NetworkCritic(action_count + state_count + 1 + 1).to(device)
  optimizer2 = torch.optim.AdamW(clf.parameters(), lr=0.001)#, weight_decay=1e-5)
  #optimizer = torch.optim.SGD(clf.parameters(), lr=0.01)
  start_time = time.time()
  epochs = 20
  for epoch in range(epochs):
    y_true = []
    y_pred = []
    running_loss = 0.0
    train_start_time = time.time()
    for i, data in enumerate(trainloader):
      # batch_start_time = time.time()

      inputs, labels = data
      #inputs, labels = inputs.to(device), labels.to(device).float()
      
      clf.train()
      labels = torch.unsqueeze(labels, 1)

      # forward propagation
      outputs = clf(inputs)

      # print(f"  Batch forward prop: {time.time() - batch_start_time}")
      # batch_start_time = time.time()

      #outputs = torch.sigmoid(outputs)
      #outputs = torch.softmax(outputs, 1)

      loss = criterion(outputs, labels.float())

      # print(f"  Batch criterion: {time.time() - batch_start_time}")
      # batch_start_time = time.time()


      optimizer2.zero_grad()
      loss.backward()
      optimizer2.step()
      running_loss += loss.item()

      # print(f"  Batch running loss: {time.time() - batch_start_time}")
      # batch_start_time = time.time()

      #PREDICTIONS 
      clf.eval()
      with torch.no_grad():
        #outputs = torch.sigmoid(outputs) * mask
        pred = outputs#outputs.cpu().detach().numpy()
        labels = labels#.cpu().detach().numpy()    
        # y_pred = pred.tolist()
        # y_true = labels.tolist()
        y_true.extend(labels.tolist())
        y_pred.extend(pred.tolist())
  

    #print(f"Elapsed time training: {time.time() - train_start_time}")
    
    if epoch % 10 == 9:
      # display statistics
      
      y_pred = [np.round(i) for i in y_pred]
      y_true = [np.round(i) for i in y_true]
      print(f"[{epoch + 1}, {i + 1:5d}]Accuracy on training set is " + str(accuracy_score(np.array(y_true),np.array(y_pred))))
      print(f'[{epoch + 1}, {i + 1:5d}] loss: {running_loss / (i + 1):.5f}')
      print(f"Elapsed time: {time.time() - start_time}")
    
    if (running_loss / (i + 1)) < 0.0005:
      break
   
  #PREDICTIONS 

  with torch.no_grad():
    clf.eval()
    
    y_pred = torch.sigmoid(clf(torch.from_numpy(np.array(x_test)).to(device).float()))
    #y_pred = torch.softmax(y_pred.cpu().detach().numpy(), 1)


    y_test = np.array(y_test)
    mask = (y_test != -1)
    y_test *= mask
    #y_test[y_test==0] = -1
    y_pred = y_pred.cpu()
    y_pred = torch.Tensor([np.round(i) for i in y_pred])
    y_test = torch.Tensor([np.round(i) for i in y_test])
    num_correct = (y_pred == y_test).sum()
    num_samples = y_pred.size(0)
    # predictions = (y_pred > 0.45).long()
    # num_correct = (predictions == torch.Tensor(y_test)).sum()
    # num_samples = predictions.size(0) * predictions.size(1)

    print("Got {} / {} with accuracy {}".format(num_correct, num_samples, float(num_correct)/float(num_samples)*100))

    #print(f"Accuracy on test set is " + str(accuracy_score(np.array(y_test),np.array(y_pred))))

  
  PATH = "./data/critic/" + name
  torch.save(clf.state_dict(), PATH)

def read_json():
  global action_list, compare_to
  global action_count, state_count
  compiled = {}
  action_count = 0
  state_count = 0
  
  for p in Path('./GameData').glob('*.json'):
      with p.open() as f:
        name = os.path.basename(p).split("_")[0]
        game_data = []
        for data in f:
          data = data.strip("\n")

          try:
            j = json.loads(data)
            
            j["actions"] = list(map(int, j["actions"].split(',')))
            j["actions"].sort()
            if len(j["state"]) > 0:
              j["state"] = list(map(int, j["state"].split(',')))
              j["state"].sort()
            else:
              j["state"] = []
            if j["performed"] != None:
              j["performed"] = int(j["performed"])
            else:
              j["performed"] = None
            j["result"] = int(j["result"])

            action_count = max(action_count, max(j["actions"]))
            if len(j["state"]) > 0:
              state_count = max(state_count, max(j["state"]))
            
            game_data.append(j)
          except:
            print("error occured when parsing")

        if name not in compiled:
          compiled[name] = []
        compiled[name].append(game_data)

  print("Action count: " + str(action_count))
  print("state count: " + str(state_count))

  return compiled

# def read_json():
#   global action_count, state_count
#   compiled = {}
#   action_count = 0
#   state_count = 0
  
#   for p in Path('./GameData').glob('*.json'):
#       with p.open() as f:
#           name = os.path.basename(p).split("_")[0]
#           for data in f:
#             j = json.loads(data)

#             # TODO fix this later
#             if j["performed"] == None:
#               continue
#             # if j["result"] == '1':
#             #   continue
            
#             j["actions"] = list(map(int, j["actions"].split(',')))
#             j["state"] = list(map(int, j["state"].split(',')))
#             j["performed"] = int(j["performed"])
#             j["result"] = int(j["result"])

#             if len(j["actions"]) <= 1:
#               continue

#             action_count = max(action_count, max(j["actions"]))
#             state_count = max(state_count, max(j["state"]))
            
#             if name not in compiled:
#               compiled[name] = []
#             compiled[name].append(j)

#   print("Action count: " + str(action_count))
#   print("state count: " + str(state_count))

#   return compiled

"""
  compiled_data is a dictionary of lists
"""
def create_dataset(compiled_data:typing.Dict[str, typing.List]):
  print("creating dataset")
  global action_count, state_count
  dataset = []
  critic_data = []

  for name in compiled_data:

    inputs = []
    outputs = []
    input_critic = []
    results = []

    for json_data in compiled_data[name]:
      #for json_data in game_data:
        data = np.zeros(state_count + action_count + 1 + 1)
        answer = np.ones(action_count + 1)
        answer = -1 * answer

        if len(json_data["actions"]) <= 1:
          continue
        # Mask all answer result
        # answer[answer == 0] = -1

        # One hot encoding
        for d in json_data["state"]:
          data[d] = 1
        for d in json_data["actions"]:
          data[d + state_count] = 1
          answer[d] = 0#json_data["result"]#0 # Marked as not performed
        
        # Penalize losses, -1 is masked out later 
        if json_data["result"] >= 0.7:
          answer[answer == 0] = -1
        #   answer[answer == 1] = 0
          #continue # Comment this to not skip losses
        #else:
        inputs.append(data)
        outputs.append(answer)
        # Only select answer
        answer[json_data["performed"]] = round(1 - json_data["result"])
        #json_data["result"] = 1 - json_data["result"]

  

        input_critic.append(data)
        results.append(1 - json_data["result"])
    
    dataset.append( (inputs, outputs, name) )
    critic_data.append( (input_critic, results, "critic_" + name) )

  return (dataset, critic_data)

"""
Creates a dataset based on the results from a fixed seed game
"""
def create_dataset_search_tree():
  fetchDatabaseData()
  read_json() # Call this just to get the action and state count
  good_data, bad_data, ok_data = compile_data()

  compiled_data = {}

  for name in good_data:
    if name not in compiled_data:
      compiled_data[name] = []
    compiled_data[name].extend(good_data[name])
  
  for name in bad_data:
    if name not in compiled_data:
      compiled_data[name] = []
    compiled_data[name].extend(bad_data[name])

  # for name in ok_data:
  #   if name not in compiled_data:
  #     compiled_data[name] = []
  #   compiled_data[name].extend(ok_data[name])

  return create_dataset(compiled_data)[0]

def combine_datasets(dataset:typing.List[typing.Tuple]):
  inputs = []
  outputs = []
  for data in dataset:
    inputs.extend(data[0])
    outputs.extend(data[1])
  return inputs, outputs

def trainData(data, answer, name, training):
  if len(data) > 0:
    # Solve data
    print("Training " + name)
    print("data length:"+str(len(data)))

    if(len(data) < 10) or TrainAll:
      x_train = x_test = data
      y_train = y_test = answer
    else:
      x_train, x_test, y_train, y_test = train_test_split(data, answer, test_size=0.3)
    
    training(x_train, y_train, x_test, y_test, name)

def read_data():
  global ShowData
  
  clearLocalData()

  # Generate training data
  json_data = {}
  all_data = read_json()
  for name in all_data:
    json_data[name] = []
    for game_data in all_data[name]:
      json_data[name].extend(game_data)
  critic = create_dataset(json_data)[1]
  for data in critic:
    trainData(*data, trainCritic)
  trainData(*combine_datasets(critic), "critic_master", trainCritic)

  dataset = create_dataset_search_tree()
  # Train data
  for data in dataset:
    trainData(*data, trainTorch)
  trainData(*combine_datasets(dataset), "master", trainTorch)


def compile_data():
  data = read_json()
  good_data = {}
  bad_data = {}
  ok_data = {}

  _, critic_data = getTorchData()

  for name in data:
    good_actions = []
    bad_actions = []
    ok_actions = []
    # Find the first win
    
    # for game_id in range(len(data[name]) - 1, 0, -1):
    #   game_data = data[name][game_id]
    #   if (game_data[0]["result"] == 0):
    #     good_actions.extend(game_data)
    #     break
    
    for game_result in data[name]:
      if len(game_result) <= 1:
        print("Error not enough data for game_result")
        continue
      critic_current = getTorchPrediction(critic_data, get_input_data(game_result[0]))["critic_" + name]
      for action_id in range(len(game_result)):
        critic_future = 1
        if game_result[action_id]["result"] != 0:
          critic_future = 0
        if action_id < len(game_result) - 1:
          critic_future = getTorchPrediction(critic_data, get_input_data(game_result[action_id + 1]))["critic_" + name]
        
        diff = critic_future - critic_current
        
        game_result[action_id]["result"] = (0.5 - diff / 2.0)

        if diff > 0.2:
          good_actions.append(game_result[action_id])
        elif diff < -0.2:
          bad_actions.append(game_result[action_id])
        else:
          ok_actions.append(game_result[action_id])

        critic_current = critic_future
    
    print(name + " Good Data-----------")
    for good in good_actions:
      actions = good["actions"]
      state = good["state"]
      performed = good["performed"]
      result = good["result"]

      if round(result) == 1:
        print("ERROR Good data shouldn't have a loss!")

      if performed == 'None':
        performed = None

      # if performed == None:
      #   print("None")
      # else:
      #   print(str(round(result*100)/100) + "(" + str(performed) + ")" + str(action_list[performed]))
      # if __name__ == "__main__":
      #   input("press any key...")
    print(name + " Bad Data-----------")
    for bad in bad_actions:
      actions = bad["actions"]
      state = bad["state"]
      performed = bad["performed"]
      result = bad["result"]

      if round(result) == 0:
        print("ERROR Bad data shouldn't have a Win!")

      # if performed == None:
      #   print("None")
      # else:
      #   print(str(round(result*100)/100) +"(" + str(performed) + ")" + str(action_list[performed]))
      # if __name__ == "__main__":
      #   input("press any key...")
    print(name + " Ok Data-----------")
    for ok in ok_actions:
      actions = ok["actions"]
      state = ok["state"]
      performed = ok["performed"]
      result = ok["result"]

      if performed == 'None':
        performed = None

      # if performed == None:
      #   print("None")
      # else:
      #   print(str(round(result*100)/100) +"(" + str(performed) + ")" + str(action_list[performed]))
      # if __name__ == "__main__":
      #   input("press any key...")
    good_data[name] = good_actions
    bad_data[name] = bad_actions
    ok_data[name] = ok_actions

  return good_data, bad_data, ok_data
  
  if __name__ != "__main__":
    clearLocalData()

## Gets the input info for the action
def get_input_data(action_info):
  actions = action_info["actions"]
  state = action_info["state"]
  performed = action_info["performed"]
  result = action_info["result"]

  input_length = 1 + state_count + 1 + action_count
  input_list = [0] * (input_length)

  for id in state:
    index = int(id)
    if (index < len(input_list) and index >= 0):
      input_list[index] = 1

  for id in actions:
    index = state_count + 1 + int(id) 
    if (index < len(input_list) and index >= 0):
      input_list[index] = 1
  
  return input_list

def fetchDatabaseData():
  global action_list, compare_to
  global input_length, output_length

  print("Reading data")
  conn = sqlite3.connect(os.getcwd() +'/cardData.cdb')
  c = conn.cursor()

  #c.execute('SELECT rowid, Name, Action FROM L_ActionList where Output = ?', (node_id,))
  print("fetch action list")
  c.execute('SELECT rowid, Name, Action FROM L_ActionList')
  records = c.fetchall()
  for record in records:
    action_list[record[0]] = Action(record[0], record[1], record[2])

  print("fetch compare to")
  c.execute('SELECT rowid, Location, Compare, Value FROM L_CompareTo')
  records = c.fetchall()
  for record in records:
    compare_to[record[0]] = CompareTo(record[0], record[1], record[2], record[3])
  
  #conn.commit()
  conn.close()

  input_length = 1 + len(compare_to)# +  len(action_list)
  output_length = 1 + len(action_list)
  print("length")
  print("input"+str(input_length))
  print("output"+str(output_length))
 
if __name__ == "__main__":
  print("start time " + str(datetime.datetime.now()))
  torch.multiprocessing.set_start_method('spawn')
  torch.backends.cudnn.benchmark = True
  deleteData()
  read_data()

  print("end time " + str(datetime.datetime.now()))