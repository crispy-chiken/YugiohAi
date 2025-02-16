import datetime
import glob
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

TrainData = not (len(sys.argv)>1 and ("--f" in sys.argv or "-f" in sys.argv))
TrainAll = (len(sys.argv)>1 and ("--a" in sys.argv or "-a" in sys.argv))
print("Train all " + str(TrainAll))
ShowData = True
ShowAcc = False

#Torch settings
dtype = torch.float
device = torch.device('cuda' if torch.cuda.is_available() else 'cpu')
# print('Using device:', device)
# print()


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

class CustomLoss(nn.Module):
    def __init__(self):
        super(CustomLoss, self).__init__()

    def forward(self, inputs, targets):
        loss = F.nll_loss(inputs, targets)
        #loss = F.softmax(loss)
        #loss = -1 * (targets * torch.log(inputs) + (1 - targets) * torch.log(1 - inputs))
        return loss
    
class Network(nn.Module):
  def __init__(self, input_dim, output_dim):
    super(Network, self).__init__()
    hidden_layers = 1000#(input_dim + output_dim)# * 2

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

# Data base Classes

class Action:
  def __init__(self, id, name, action) -> None:
    self.id = id
    self.name = name
    self.action = action

  def __str__(self) -> str:
    return str(self.name + " " + self.action)

class ActionState:
  def __init__(self, id, actionId, historyId, performed: bool) -> None:
    self.id = id
    self.actionId = actionId
    self.historyId = historyId
    self.performed = performed

class CompareTo:
  def __init__(self, id, location, compare, value) -> None:
    self.id = id
    self.location = location
    self.compare = compare
    self.value = value

  def __str__(self) -> str:
    return f"({self.id}) " + str(self.location + " " + self.compare + " " + self.value)

class FieldState:
  def __init__(self, id, compareId, historyId) -> None:
    self.id = id
    self.compareId = compareId
    self.historyId = historyId

class PlayRecord:
  def __init__(self, id, gameId, turnId, actionId, c1h, c1f, c2h, c2f, p1h, p1f, p2h, p2f) -> None:
    self.id = id
    self.gameId = gameId
    self.turnId = turnId
    self.actionId = actionId
    self.curP1Hand = c1h
    self.curP1Field = c1f
    self.curP2Hand = c2h
    self.curP2Field = c2f
    self.postP1Hand = p1h
    self.postP1Field = p1f
    self.postP2Hand = p2h
    self.postP2Field = p2f

class GameResult:
  def __init__(self, id, name, result, placement, manual, shouldUpdate) -> None:
    self.id = id
    self.name = name
    self.result = result
    self.placement = placement
    self.manual = manual
    self.shouldUpdate = shouldUpdate != 'False'

action_list: typing.Dict[int, Action] = {}
action_state: typing.Dict[int, typing.List[ActionState]] = {}
compare_to: typing.Dict[int, CompareTo] = {}
field_state: typing.Dict[int, typing.List[FieldState]] = {}
play_record: typing.Dict[int, typing.List[PlayRecord]] = {}
record_to_game: typing.Dict[int, int] = {}
game_result: typing.Dict[str, typing.Dict[int, GameResult]] = {}

input_length = 0
output_length = 0

def deleteData():
  global TrainData, ShowData
  if (TrainData):
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

def fetchDatabaseData(ignore_results = False):
  global action_list, action_state, compare_to, field_state, play_record, game_result, record_to_game
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

  if not ignore_results:
    
    print("fetch action state")
    c.execute('SELECT rowid, ActionId, HistoryId, Performed FROM L_ActionState')
    records = c.fetchall()
    for record in records:
      id = record[2]
      if (id not in action_state):
        action_state[id] = []
      action_state[id].append(ActionState(record[0], record[1], record[2], record[3]))

    print("fetch field state")
    c.execute('SELECT rowid, CompareId, HistoryId FROM L_FieldState')
    records = c.fetchall()
    for record in records:
      id = record[2]
      if (id not in field_state):
        field_state[id] = []
      field_state[id].append(FieldState(record[0], record[1], record[2]))

    print("fetch play record")
    c.execute('SELECT rowid, GameId, TurnId, ActionId, CurP1Hand, CurP1Field, CurP2Hand, CurP2Field, PostP1Hand, PostP1Field, PostP2Hand, PostP2Field FROM L_PlayRecord')
    records = c.fetchall()
    for record in records:
      id = record[1]
      if (id not in play_record):
        play_record[id] = []
      play_record[id].append(PlayRecord(record[0], record[1], record[2], record[3], record[4], record[5], record[6], record[7], record[8], record[9], record[10], record[11]))
      record_to_game[record[0]] = id

    print("fetch game result")
    c.execute('SELECT rowid, Name, Result, Placement, IsManual, ShouldUpdate FROM L_GameResult')
    records = c.fetchall()
    for record in records:
      id = record[0]
      name = record[1]
      if (name not in game_result):
        game_result[name] = {}
      if (id not in game_result[name]):
        game_result[name][id] = GameResult(record[0], record[1], record[2], record[3], record[4], record[5])
  
  #conn.commit()
  conn.close()

  input_length = 1 + len(compare_to)# +  len(action_list)
  output_length = 1 + len(action_list)
  print("length")
  print("input"+str(input_length))
  print("output"+str(output_length))
  print("records" +str(len(play_record)))

def getWinRate(newOnly = True, limit = 50):
  data = {}
  conn = sqlite3.connect(os.getcwd() +'/cardData.cdb')
  c = conn.cursor()
  limiter = "(select * from L_GameResult order by rowid desc limit "+str(limit)+")"
  if newOnly:
    limiter = "L_GameResult"
  sql = "select a.name, cast(b.result as float)/cast(count(a.result) as float) as winrate from " + limiter + " as a, (select name, count(result) as result from " + limiter + " where result = 1 group by name) as b where a.name = b.name "
  if newOnly:
    sql += "and a.Placement is null " 
  sql += "group by b.name order by winrate desc"
  c.execute(sql)
  records = c.fetchall()
  for record in records:
    data[record[0]] = float(record[1])
  conn.close()

  return dict(sorted(data.items(), key=lambda item: item[1], reverse=True))

def markPlacements():
  global play_record

  best = list(getWinRate().keys())
  best = best[:math.ceil(len(best)/2)]

  if len(best) == 0:
    return

  conn = sqlite3.connect(os.getcwd() +'/cardData.cdb')
  c = conn.cursor()

  best_id = "'"
  for b in best:
    best_id += str(b) + ","
  best_id = best_id[:-1] + "'"

  select_sql = "(select c.rowid from L_GameResult c where Placement is null and Name like " + best_id + ")"

  update_sql = "update L_GameResult set Placement = 1 where Placement is null and L_GameResult.rowid in " + select_sql
  #print(update_sql)
  c.execute(update_sql)

  update_sql = "update L_GameResult set Placement = 0 where Placement is null"
  c.execute(update_sql)

  conn.commit()
  conn.close()
 
def getTorchData():
  action_data = {}
  directory = './data'
  for filename in os.listdir(directory):
    f = os.path.join(directory, filename)
    if os.path.isfile(f):
        clf = Network(input_length + output_length + 1, output_length)
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

def createBetterDataset():
  global input_length, output_length
  global action_list, action_state, compare_to, field_state, play_record, game_result
  
  if (len(action_list) + len(compare_to)) == 0:
    return
  if (len(play_record) <= 0):
    return
  
  best = list(getWinRate().keys())
  best = best[:int(len(best)/2)]

  action_data = getTorchData()

  data = []
  answer = []
  possible_action_list = []
  for name in game_result:
    for game_id in game_result[name]:

      # Only select the top values
      # if game_result[name][game_id].placement != 1:
      #   continue

      if game_id in play_record.keys():
        for history in play_record[game_id]:
          reward = 1
          punishment = -0.10

          if (game_result[name][game_id].result != 1 and not game_result[name][game_id].manual):
            continue

          input_list = [0] * input_length
          output_list = [0] * (output_length)
          next_phase = False
          
          # All field states at the end
          for state in field_state[history.id]:
            input_list[state.compareId] = 1
          
          posssible_actions = action_state[history.id]
          # All possible actions as input
          for state in posssible_actions:
            #input_list[state.actionId  + len(compare_to)] = 1
            if (state.performed == 'True'):
              if (action_list[state.actionId].name == "" and len(posssible_actions) > 2):
                next_phase = True
              if (len(posssible_actions) <= 1):
                next_phase = True
            else:
              output_list[state.actionId] = punishment

          if next_phase:
            continue

          data.append(input_list)
          possible_action_list.append(posssible_actions)

  final_result = getTorchPrediction(action_data, data, True)
  better_final = {}
  for b in best:
    better_final[b] = final_result[b]

  better = getBetterPrediction(final_result, 1, True)

  final_data = []
  for i in range(len(better)):
    output_list = 0 #[0] * (output_length)
    found = False
    # Find the first result that is in the possible action list
    for result in better[i]:
      index = result[0]
      percentage = result[1]
      for state in possible_action_list[i]:
        if state.actionId == index:
          output_list = index #[index] = 1
          found = True
          break
      if found:
        final_data.append(data[i])
        answer.append(output_list)
        break
  data = final_data


  trainData(data, answer, "master")

  directory = './data'
  for filename in os.listdir(directory):
    f = os.path.join(directory, filename)
    if os.path.isfile(f) and not f.endswith('master'):
      shutil.copyfile('./data/master', f)
  os.unlink('./data/master')

def getBetterPrediction(final_result, possibleActions, mode = 0, multi = False):
  lst_best_score: typing.List[typing.List[typing.Dict[int, float]]] = []
  scores: typing.List[typing.Dict[int, float]] = []
  best_score: typing.List[typing.Dict[int, typing.List[float]]] = []
  
  for key in final_result: 
    results = final_result[key]
    if not multi:
      results = [results]
    for game_index in range(len(results)):
      result = results[game_index]
      s = {}
      # Only get top 4
      nth = len(result)#4
      ind = np.argpartition(result, -4)[-nth:]
      index = ind[np.argsort(result[ind])]
      index = index[::-1]
      
      for i in index: # Get all percentages from one dataset
        if i not in possibleActions:
          continue
        if i not in s:
          s[i] = []
        s[i].append(result[i])
      if game_index in range(len(scores)):
        for key in s: # Loop through the dictionary
          if key in scores[game_index].keys(): # If the input action key is in the list, append it
            scores[game_index][key].extend(s[key])
          else:
            scores[game_index][key] = s[key]
      else:
        scores.append(s)

  for s in scores:
    best = {}
    if mode == 0: # Get the greatest score
      for i in s:
        best[i] = 0
        for weight in s[i]:
          if best[i] < weight:
            best[i] = weight
    elif mode == 1: # Average out all the scores
      for i in s:
        total = 0.0
        count = 0.0
        for weight in s[i]:
          total += weight
          count += 1
        best[i] = round(total / float(count) * 100) / 100
    elif mode == 2: # Most common score
        for i in s:
          best[i] = 0
          for weight in s[i]:
            best[i] += weight
    
    best_score.append(best)
  
  for best in best_score:
    # Find the best score for each data entry
    lst_best_score.append(list(sorted(best.items(), key=lambda item: item[1]))[::-1])

  return lst_best_score

def showGameHistory():
  global input_length, output_length
  global action_list, action_state, compare_to, field_state, play_record, game_result

  action_data = getTorchData()

  records = []
  for name in game_result:
    for game_id in game_result[name]:
      if game_id not in play_record:
        continue
      for game_record in play_record[game_id]:
        records.append((game_record, game_result[name][game_id].result, name))

  random.shuffle(records)
  
  for r in records:
    record = r[0]
    result = r[1]
    ai_name = r[2]
    input_list = [0] * (input_length + output_length + 1)#(input_length)#
    output_list = -1

    # Only show wins
    # if result != 1:
    #   continue

    # if 'SnakeEyes' not in ai_name:
    #   continue

    #Go first or second 
    # if record.turnId != 0:
    #   continue

    # Might not be in action state as there were no actions performed?
    if record.id not in action_state:
      continue

    # Find ones with more than 3 choices
    if len(action_state[record.id]) <= 3:
      continue

    ## Check start
    playedAction = -1
    stateAction = action_state[record.id]
    for j in stateAction:
      if j.performed == 'True':
        playedAction = j.actionId

    ## Check end

    if record.id in field_state:
      for state in field_state[record.id]:
        input_list[state.compareId] = 1

    for state in action_state[record.id]:
      index = input_length + 1 + int(state.actionId) 
      input_list[index] = 1
      if (state.performed == 'True'):
        output_list = state.actionId

    print("Game:" + str(record.gameId) + " Turn:" + str(record.turnId) + " Action:" + str(record.actionId))
    print("--------Stats--------")
    print("curP1Hand:" + str(record.curP1Hand))
    print("curP1Field:" + str(record.curP1Field))
    print("curP2Hand:" + str(record.curP2Hand))
    print("curP2Field:" + str(record.curP2Field))
    print("postP1Hand:" + str(record.postP1Hand))
    print("postP1Field:" + str(record.postP1Field))
    print("postP2Hand:" + str(record.postP2Hand))
    print("postP2Field:" + str(record.postP2Field))

    # field = [
    #   int(record.curP1Hand),
    #   int(record.curP1Field),
    #   int(record.curP2Hand),
    #   int(record.curP2Field),
    #   int(record.postP1Hand),
    #   int(record.postP1Field),
    #   int(record.postP2Hand),
    #   int(record.postP2Field)
    # ]
  

    if record.id in field_state:
      print("--------Field State--------")
      stateField = field_state[record.id]
      for j in stateField: # To Update
        compare = compare_to[j.compareId]
        print("  " + str(compare))

    print("--------Possible Actions--------")

    stateAction = action_state[record.id]
    possibleActions = []
    for j in stateAction:
      action = action_list[j.actionId]
      possibleActions.append(j.actionId)
      print("  (" + str(j.actionId) + ")" + str(j.performed) + "| " + str(action))
    
    final_result = []
    final_result = getTorchPrediction(action_data, [input_list])

    avg = 0
    avg2 = 0
    cnt = 0
    for key in final_result:
      res = final_result[key]


      # curActions = []
      # for i in possibleActions:
      #   curActions.append(res[i])
      # curActions = scipy.special.softmax(curActions,0)
      # for i in range(len(possibleActions)):
      #   res[possibleActions[i]] = curActions[i]


      text = key + ":"
      nth = len(res)#4
      ind = np.argpartition(res, -nth)[-nth:]
      index = ind[np.argsort(res[ind])]
      index = index[::-1]

      # index = sorted(range(len(output)), key=lambda k: output[k])
		  # index = index[::-1]
      for i in index:
        if i not in possibleActions:
          continue
        text += "[" + str(i) + "]" + ":" + str(round(res[i]*100)) + ","
        avg += res[i]
        cnt += 1

      #text += " max " + str(max(res)*100  )
    
      print(text)
      #print(sum(result))

    # avg/=len(final_result)
    # cnt = max(1,cnt)
    # avg2 /= cnt
    # avg /= max(1,cnt)
    # print("Avg:" + str(avg))
    # print("Avg2:" + str(avg2))

    better = getBetterPrediction(final_result, possibleActions, 0)[0][:4]
    print("Better Prediction MAX :" + str(better))
    better = getBetterPrediction(final_result, possibleActions, 1)[0][:4]
    print("Better Prediction AVG :" + str(better))
  
    #print("Expected answer:" + str(result))
    print("Result:" + str(result) + " Source:" + str(ai_name)) 

    if len(stateAction) <= 1:
      continue
    if len(possibleActions) == 2 and playedAction == -1:
      continue

    value = -1
    leave = False

    # if True:
    #   getSimilarActionPerformed(record.id)

    while value != '0' and value != '1':
      value = input("good (1) or bad (0)")
      try:
        if (len(value) == 0):
          leave = True
          break
        elif (int(value) != 0 and int(value) != 1):
          value = -1
      except:
        value = -1
        print("Input error, try again")
    print("")
    if (leave):
      break

def getSimilarFieldStates(recordId):
  global input_length, output_length
  global action_list, action_state, compare_to, field_state, play_record, game_result, record_to_game

  cur_field = field_state[recordId]
  field_state_ids = []
  for c in cur_field:
    field_state_ids.append(c.compareId)

  related:typing.Dict[int, typing.List[int]] = {}
  
  # Find related
  for i in field_state.keys():
    if i != recordId:
        diff_count = len(field_state_ids)
        for j in field_state[i]:
          if j.compareId not in field_state_ids:
            diff_count += 1
          else:
            diff_count -= 1

        if diff_count not in related:
          related[diff_count] = []

        related[diff_count].append(i)

  _ = 1 

  # Print related
  for i in range(0,6):
    print("Related Dist:" + str(i))
    if i in related:
        for related_id in related[i]:
          if related_id not in action_state:
            continue

          # Find result
          game_id = record_to_game[related_id]
          for name in game_result:
            if game_id in game_result[name]:
              print("Result:" + str(game_result[name][game_id].result))
          

          print("--------Field State--------")
          stateField = field_state[related_id]
          for j in stateField:
            compare = compare_to[j.compareId]
            print("  " + str(compare))

          stateAction = action_state[related_id]
          possibleActions = []
          for j in stateAction:
            action = action_list[j.actionId]
            possibleActions.append(j.actionId)
            print("  (" + str(j.actionId) + ")" + str(j.performed) + "| " + str(action))

def getSimilarActionPerformed(recordId):
  global input_length, output_length
  global action_list, action_state, compare_to, field_state, play_record, game_result, record_to_game

  # Might not be in action state as there were no actions performed?
  if recordId not in action_state:
    return -1
  
  # Get the action played
  cur_actions = action_state[recordId]
  cur_action_played = 0
  for j in cur_actions:
    if j.performed == 'True':
      cur_action_played = j.actionId

  # Set up current field state
  cur_field = field_state[recordId]
  field_state_ids = []
  for c in cur_field:
    field_state_ids.append(c.compareId)

  related:typing.Dict[int, typing.List[int]] = {}
  
  # Find all similar actions 
  for i in action_state.keys():
    diff_count = len(field_state_ids)
    actionPlayed = 0
    actionIn = False
    # ignore self
    if i == recordId:
      continue
    # Check if action is played
    for j in action_state[i]:
      if j.actionId == cur_action_played:
        actionIn = True
      # if j.performed == 'True':
      #   actionPlayed = j.actionId
      #   break

    if actionIn:#actionPlayed == cur_action_played:

      # Find the difference in field state
      for j in field_state[i]:
        if j.compareId not in field_state_ids:
          diff_count += 1
        else:
          diff_count -= 1

      if diff_count not in related:
        related[diff_count] = []

      related[diff_count].append(i)

  _ = 1 

  # Print related
  weight = 0
  for i in range(0,6):
    if not TrainData:
      print("Related Dist:" + str(i))
    if i in related:
      activatedSum = 0
      notActivateSum = 0
      for related_id in related[i]:
        if related_id not in action_state:
          continue
        
        stateAction = action_state[related_id]
        for j in stateAction:
          action = action_list[j.actionId]
          #print("  (" + str(j.actionId) + ")" + str(j.performed) + "| " + str(action))
          if j.actionId == cur_action_played:
            if j.performed == 'True':
              activatedSum += 1
            else:
              notActivateSum += 1

        # Find result
        # game_id = record_to_game[related_id]
        # for name in game_result:
        #   if game_id in game_result[name]:
        #     print("Result:" + str(game_result[name][game_id].result))
        

        # print("--------Field State--------")
        # stateField = field_state[related_id]
        # for j in stateField:
        #   compare = compare_to[j.compareId]
        #   print("  " + str(compare))

        # stateAction = action_state[related_id]
        # for j in stateAction:
        #   action = action_list[j.actionId]
        #   print("  (" + str(j.actionId) + ")" + str(j.performed) + "| " + str(action))
      if not TrainData:
        print("Activated:" + str(activatedSum) + " Not Activated:" + str(notActivateSum))
      weight += 5/(5 + i) * (activatedSum - notActivateSum)
  
  return weight


def showDataPredictionPercentage():
  global input_length, output_length
  global action_list, action_state, compare_to, field_state, play_record, game_result

  CorrectPredSelfWin: typing.Dict[str,typing.Tuple[int, int]] = {}
  CorrectPredOtherWin: typing.Dict[str,typing.Tuple[int, int]] = {}
  CorrectPredSelfLoss: typing.Dict[str,typing.Tuple[int, int]] = {}
  CorrectPredOtherLoss: typing.Dict[str,typing.Tuple[int, int]] = {}

  action_data = getTorchData()
  records = []
  for name in game_result:
    for game_id in game_result[name]:
      for game_record in play_record[game_id]:
        records.append((game_record, game_result[name][game_id].result, name))

  random.shuffle(records)
  
  for r in records:
    record = r[0]
    outcome = r[1]
    ai_name = r[2]
    input_list = [0] * input_length
    output_list = -1

    for state in field_state[record.id]:
      input_list[state.compareId] = 1

    for state in action_state[record.id]:
      #input_list[state.actionId - 1 + len(compare_to)] = 1
      if (state.performed == 'True'):
        output_list = state.actionId

    # field = [
    #   int(record.curP1Hand),
    #   int(record.curP1Field),
    #   int(record.curP2Hand),
    #   int(record.curP2Field),
    #   int(record.postP1Hand),
    #   int(record.postP1Field),
    #   int(record.postP2Hand),
    #   int(record.postP2Field)
    # ]

    stateAction = action_state[record.id]
    if len(stateAction) <= 1:
      continue

    final_result = []
    final_result = getTorchPrediction(action_data, [input_list])
  
    for key in final_result:
      result = final_result[key]
      ind = np.argpartition(result, -4)[-4:]
      index = ind[np.argsort(result[ind])]
      index = index[::-1]
      # index = sorted(range(len(output)), key=lambda k: output[k])
		  # index = index[::-1]

      result_type = 0
        
      if ai_name in key:
        result_type = 0
      else:
        result_type = 2

      if outcome == 1: # Win
        pass
      else:
        result_type += 1
      
      correct = output_list in index[:4]
      
      if result_type == 0: # Self win
        if key not in CorrectPredSelfWin:
          CorrectPredSelfWin[key] = (0,0)
        data = CorrectPredSelfWin[key]
        CorrectPredSelfWin[key] = (data[0] + correct, data[1] + 1)
      elif result_type == 1: # Self loss
        if key not in CorrectPredSelfLoss:
          CorrectPredSelfLoss[key] = (0,0)
        data = CorrectPredSelfLoss[key]
        CorrectPredSelfLoss[key] = (data[0] + correct, data[1] + 1)
      elif result_type == 2: # Other Win
        if key not in CorrectPredOtherWin:
          CorrectPredOtherWin[key] = (0,0)
        data = CorrectPredOtherWin[key]
        CorrectPredOtherWin[key] = (data[0] + correct, data[1] + 1)
      elif result_type == 3: # Other Loss
        if key not in CorrectPredOtherLoss:
          CorrectPredOtherLoss[key] = (0,0)
        data = CorrectPredOtherLoss[key]
        CorrectPredOtherLoss[key] = (data[0] + correct, data[1] + 1)
    
    # Do better prediction result
    better = getBetterPrediction(final_result, 0)[0][0]
    correct = output_list == better[0]
    key = "max"

    if outcome == 1: # Win
      if key not in CorrectPredOtherWin:
        CorrectPredOtherWin[key] = (0,0)
      data = CorrectPredOtherWin[key]
      CorrectPredOtherWin[key] = (data[0] + correct, data[1] + 1)
    else:
      if key not in CorrectPredOtherLoss:
        CorrectPredOtherLoss[key] = (0,0)
      data = CorrectPredOtherLoss[key]
      CorrectPredOtherLoss[key] = (data[0] + correct, data[1] + 1)

  print("------------")
  printresults(CorrectPredSelfWin, "self win")
  printresults(CorrectPredSelfLoss, "self loss")
  printresults(CorrectPredOtherWin, "other win")
  printresults(CorrectPredOtherLoss, "other loss")

def printresults(results, name):
  for key in results:
    c = results[key]
    name += "\n " + key + " " + str(float(c[0])/float(c[1]))
  
  print(name)

def trainTorch(x_train, y_train, x_test, y_test, name):
  #bias = getBias(name)
  traindata = Data(np.array(x_train), np.array(y_train))
  batch_size = min(40, len(y_train))#len(y_train)#
  trainloader = DataLoader(traindata, batch_size=batch_size, shuffle=True, collate_fn=lambda x: tuple(x_.to(device) for x_ in default_collate(x)))
  clf = Network(input_length + output_length + 1, output_length)
  clf.to(device)
  
  print("Batch size " + str(batch_size))
  #criterion = nn.BCEWithLogitsLoss().cuda()
  criterion = nn.CrossEntropyLoss().cuda()
  optimizer = torch.optim.Adam(clf.parameters(), lr=0.001)#, weight_decay=1e-5)
  #optimizer = torch.optim.SGD(clf.parameters(), lr=0.01)

  epochs = 100
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
      mask = (labels.cpu() != -1).to(device)
      # indexes = np.argwhere(labels.cpu() != -1)
      outputs2 = outputs.masked_select(mask)
      labels2 = labels.masked_select(mask)
      #mask[mask == False] = 0.00
      outputs3 = outputs * (mask)

      # loss = criterion(outputs2, labels2.float())
      loss = criterion(outputs, labels.argmax(1)) # For CrossEntropyLoss
      #loss = criterion(outputs, torch.sigmoid(labels))
      #loss = criterion(outputs, labels.unsqueeze(1).float())


      # set optimizer to zero grad to remove previous epoch gradients
      # optimizer.zero_grad()
      for param in clf.parameters():
        param.grad = None
      # backward propagation
      loss.backward()
      # optimize
      optimizer.step()

      running_loss += loss.item()

      #PREDICTIONS 
      clf.eval()
      with torch.no_grad():
        outputs = torch.sigmoid(outputs) * mask
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

def getBias(name):
  global input_length, output_length
  global action_list, action_state, compare_to, field_state, play_record, game_result

  bias = [(0,0)] * (output_length)
  weight_bias = [0]  * (output_length)

  for n in game_result:
    if n == "master" or n == name:
      for game_id in game_result[n]:
        if game_id in play_record.keys():
          for history in play_record[game_id]:

            result = game_result[n][game_id].result
            output_list = 0
            next_phase = False
            
            if (history.id not in action_state):
              continue

            posssible_actions = action_state[history.id]
            # All possible actions as input
            for state in posssible_actions:
              if (state.performed == 'True'):
                output_list = state.actionId #[state.actionId] = reward

                if (action_list[state.actionId].name == "" and len(posssible_actions) > 2):
                  next_phase = True
                if (len(posssible_actions) <= 1):
                  next_phase = True
              
            scores = bias[output_list]
            scores = (scores[0], scores[1] + 1)
            if result == 1:
              scores = (scores[0] + 1, scores[1])
            bias[output_list] = scores

            if next_phase:
              continue
      
    for i in range(output_length):
      if bias[i][1] == 0:
        continue
      losses = bias[i][1] - bias[i][0] - 1
      if losses == 0:
        continue
      weight_bias[i] = float(bias[i][0])/float(losses)
  
  #print("weight bias")
  #print(weight_bias)
  return torch.from_numpy(np.array(weight_bias)).to(device).float()

def compileData():
  global input_length, output_length
  global action_list, action_state, compare_to, field_state, play_record, game_result


  total_data = []
  total_answer = []

  for name in game_result:
    data = []
    answer = []
    critic_answer = []
    for game_id in game_result[name]:

      # Only select the top values
      # if game_result[name][game_id].placement != 1:
      #   continue

      if game_id not in play_record.keys():
        continue

      for history in play_record[game_id]:

        # If deemed a bad record, remove 
        # if getSimilarActionPerformed(history.id) < 0:
        #   continue

        reward = 1
        critic_reward = 1
        punishment = -0.1
        result = game_result[name][game_id].result

        # Skip if shouldn't update
        if not game_result[name][game_id].shouldUpdate:
          continue

        # Skip if not manual
        if not game_result[name][game_id].manual:
          continue

        # Assume manual is correct
        if game_result[name][game_id].manual:
          result = 1
        
        
        if (result == -1):
          #continue # Only select wins
          result = 0
        
        # if (result != 1):
        #   continue
        # if (result == -1):
        #   reward = 0
        #   critic_reward = -1
        #   punishment = 0
        # else:
        #   continue

        input_list = [0] * (input_length + output_length + 1)#input_length#
        #output_list = 0 
        output_list = [-1] * (output_length)
        next_phase = False
        
        if history.id not in field_state:
          continue
        # All field states at the end
        for state in field_state[history.id]:
          input_list[state.compareId] = 1

        
        if history.id not in action_state:
          continue
        

        skipAction = False
        posssible_actions = action_state[history.id]
        # All possible actions as input
        for state in posssible_actions:

          # Have possible actions be input
          index = input_length + 1 + int(state.actionId) 
          input_list[index] = 1
          
          # Mark which action you performed
          if (state.performed == 'True'):
            output_list[state.actionId] = result

            # Ignore skip actions?
            if ("DontPerform" in action_list[state.actionId].name or 
                "GoToEndPhase" in action_list[state.actionId].action or  
                "GoToBattlePhase" in action_list[state.actionId].action or
                "GoToMainPhase" in action_list[state.actionId].action):
              skipAction = True
              break


          elif result == 1 and output_list[state.actionId] == -1: 
            output_list[state.actionId] = 0
        
        if len(posssible_actions) <= 1:
          continue

        if skipAction:
          continue

        data.append(input_list)
        answer.append(output_list)
        critic_answer.append(critic_reward)

    total_data.extend(data)
    total_answer.extend(answer)
    trainData(data, answer, name)

  trainData(total_data,total_answer,"master")

def trainData(data, answer, name):
  if len(data) > 0:
    # Solve data
    print("Training " + name)
    print("data length:"+str(len(data)))

    if(len(data) < 10) or TrainAll:
      x_train = x_test = data
      y_train = y_test = answer
      xc_train = xc_test = data
      #yc_train = yc_test = critic_answer
    else:
      x_train, x_test, y_train, y_test = train_test_split(data, answer, test_size=0.3)
      #xc_train, xc_test, yc_train, yc_test = train_test_split(data, critic_answer, test_size=0.2)
    
    trainTorch(x_train, y_train, x_test, y_test, name)

def drawNeuralNet():
  pass

def read_data(masterData = False):
  global TrainData, ShowData
  global input_length, output_length
  
  clearLocalData()
  fetchDatabaseData()
  markPlacements()

  if (len(action_list) + len(compare_to)) == 0:
    return
  if (len(play_record) <= 0):
    return

  print("to check length:" + str(len(compare_to)))
  print("action length:" + str(len(action_list)))

  # Game History
  if __name__ == "__main__" and ShowData and not TrainData:
    #showDataPredictionPercentage()
    showGameHistory()

  
  # Generate training data
  if TrainData:
    compileData()
 
  if __name__ != "__main__":
    clearLocalData()
  
if __name__ == "__main__":
  torch.multiprocessing.set_start_method('spawn')
  deleteData()
  read_data()
  #createBetterDataset()