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
    # need to convert float64 to float32 else 
    # will get the following error
    # RuntimeError: expected scalar type Double but found Float
    self.X = torch.from_numpy(X_train.astype(np.float32))
    # need to convert float64 to Long else 
    # will get the following error
    # RuntimeError: expected scalar type Long but found Float
    self.y = torch.from_numpy(y_train).type(torch.LongTensor)
    self.len = self.X.shape[0]
  
  def __getitem__(self, index):
    return self.X[index], self.y[index]
  
  def __len__(self):
    return self.len
    
class Network(nn.Module):
  def __init__(self, input_dim, output_dim):
    super(Network, self).__init__()
    hidden_layers = 10#(input_dim + output_dim)# * 2

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
    #x = self.dropout1(x)
    x = self.act2(x)

    x = self.layer2(x)
    x = self.act2(x)

    x = self.output(x)
    #x = self.act0(x)
    return x

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
  
  return action_data

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
  batch_size = min(40, len(y_train))#len(y_train)#
  trainloader = DataLoader(traindata, batch_size=batch_size, shuffle=True, collate_fn=lambda x: tuple(x_.to(device) for x_ in default_collate(x)))
  clf = Network(action_count + state_count + 1 + 1, action_count + 1)
  clf.to(device)
  print("Batch size " + str(batch_size))
  criterion2 = nn.BCEWithLogitsLoss().cuda()
  criterion = nn.CrossEntropyLoss().cuda()
  #criterion = nn.MultiLabelMarginLoss().cuda()
  optimizer = torch.optim.Adam(clf.parameters(), lr=0.001)#, weight_decay=1e-5)
  optimizer2 = torch.optim.Adam(clf.parameters(), lr=0.001)#, weight_decay=1e-5)
  #optimizer = torch.optim.SGD(clf.parameters(), lr=0.01)
  epochs = 20
  for epoch in range(epochs):
    y_true = []
    y_pred = []
    running_loss = 0.0
    for i, data in enumerate(trainloader):
      inputs, labels = data
      inputs, labels = inputs.to(device), labels.to(device).float()
      
      clf.train()

      # forward propagation
      outputs = clf(inputs)
      #outputs = torch.sigmoid(outputs)
      #outputs = torch.softmax(outputs, 1)

      # Filter out indexes to be only values we want to train
      # Probably donesnt work the way I think it does
      has_neg = True# -1 in labels.cpu()
      mask = (labels.cpu() != -1).to(device)
      # indexes = np.argwhere(labels.cpu() != -1)
      outputs2 = outputs.masked_select(mask)
      labels2 = labels.masked_select(mask)
      #mask[mask == False] = 0.00
      #outputs3 = outputs * (mask)

      #loss = criterion(outputs2, labels2.long())
      loss = None
      if has_neg:
        loss = criterion2(outputs2, labels2.float())
      else:
        loss = criterion(outputs, labels.argmax(1)) # For CrossEntropyLoss
      #loss = criterion(outputs, torch.sigmoid(labels))
      #loss = criterion(outputs, labels.unsqueeze(1).float())


      # set optimizer to zero grad to remove previous epoch gradients
      if has_neg:
        optimizer2.zero_grad()
      else:
        optimizer.zero_grad()
      # for param in clf.parameters():
      #   param.grad = None
      # backward propagation
      loss.backward()
      
      # optimize
      if has_neg:
        optimizer2.step()
      else:
        optimizer.step()

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
      print(f"[{epoch + 1}, {i + 1:5d}]Accuracy on training set is " + str(accuracy_score(np.array(y_true),np.array(y_pred))))
      print(f'[{epoch + 1}, {i + 1:5d}] loss: {running_loss / (i + 1):.5f}')
    
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

def read_json():
  global action_count, state_count
  compiled = {}
  action_count = 0
  state_count = 0
  
  for p in Path('./GameData').glob('*.json'):
      with p.open() as f:
          name = os.path.basename(p).split("_")[0]
          for data in f:
            j = json.loads(data)

            # TODO fix this later
            if j["performed"] == None:
              continue
            # if j["result"] == '1':
            #   continue
            
            j["actions"] = list(map(int, j["actions"].split(',')))
            j["state"] = list(map(int, j["state"].split(',')))
            j["performed"] = int(j["performed"])
            j["result"] = int(j["result"])

            if len(j["actions"]) <= 1:
              continue

            action_count = max(action_count, max(j["actions"]))
            state_count = max(state_count, max(j["state"]))
            
            if name not in compiled:
              compiled[name] = []
            compiled[name].append(j)

  print("Action count: " + str(action_count))
  print("state count: " + str(state_count))

  return compiled

"""
  compiled_data is a dictionary of lists
"""
def create_dataset(compiled_data:typing.Dict[str, typing.List]):
  global action_count, state_count
  dataset = []

  for name in compiled_data:

    inputs = []
    outputs = []

    for json_data in compiled_data[name]:
      data = np.zeros(state_count + action_count + 1 + 1)
      answer = np.zeros(action_count + 1)

      # Mask all answer result
      answer[answer == 0] = -1

      # One hot encoding
      for d in json_data["state"]:
        data[d] = 1
      for d in json_data["actions"]:
        data[d + state_count + 1] = 1
        answer[d] = 0 # Marked as not performed
      # Only select answer
      answer[json_data["performed"]] = 1
      #answer = json_data["performed"]

      # Penalize losses, -1 is masked out later 
      if json_data["result"] == 1:
        answer[answer == 0] = -1
        answer[answer == 1] = 0
        continue # Comment this out to not skip losses

      inputs.append(data)
      outputs.append(answer)
    
    dataset.append( (inputs, outputs, name) )

  return dataset

def combine_datasets(dataset:typing.List[typing.Tuple]):
  inputs = []
  outputs = []
  for data in dataset:
    inputs.extend(data[0])
    outputs.extend(data[1])
  return inputs, outputs

def trainData(data, answer, name):
  if len(data) > 0:
    # Solve data
    print("Training " + name)
    print("data length:"+str(len(data)))

    if(len(data) < 10) or TrainAll:
      x_train = x_test = data
      y_train = y_test = answer
    else:
      x_train, x_test, y_train, y_test = train_test_split(data, answer, test_size=0.3)
    
    trainTorch(x_train, y_train, x_test, y_test, name)

def SeeDataResult():
  action_data = getTorchData()

def read_data():
  global ShowData
  
  clearLocalData()

  # Generate training data
  dataset = create_dataset(read_json())
  # Train data
  for data in dataset:
    trainData(*data)
  trainData(*combine_datasets(dataset), "master")

  if __name__ != "__main__":
    clearLocalData()
  
if __name__ == "__main__":
  torch.multiprocessing.set_start_method('spawn')
  deleteData()
  read_data()