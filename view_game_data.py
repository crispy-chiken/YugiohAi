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

from read_game_data_json import getTorchData, getTorchPrediction, read_json
import read_game_data_json

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

input_length = 0
output_length = 0

def clearLocalData():
  global action_list,  compare_to
  
  action_list = {}
  compare_to = {}

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
  state_count, action_count = read_game_data_json.state_count, read_game_data_json.action_count
  global action_list, compare_to

  action_data = getTorchData()

  raw_records = read_json()

  records = []
  print("compiling data ")
  # Reformat records
  for name in raw_records:
    for data in raw_records[name]:
      data["name"] = name
      records.append(data)

  random.shuffle(records)
  print("done")
  
  for r in records:
    actions = r["actions"]
    state = r["state"]
    performed = r["performed"]
    result = r["result"]
    name = r["name"]

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

    # Only show wins
    # if result != 1:
    #   continue


    # Find ones with more than 2 choices
    if len(actions) <= 2:
      continue

    print("--------Field State--------")
    for j in state:
      print("  " + str(compare_to[j]))

    print("--------Possible Actions--------")

    for j in actions:
      print("  (" + str(j) + ")" + str(j == performed) + "| " + str(action_list[j]))
    
    final_result = []
    final_result = getTorchPrediction(action_data, [input_list])

    avg = 0
    avg2 = 0
    cnt = 0
    for key in final_result:
      res = final_result[key]

      text = key + ":"
      nth = len(res)#4
      ind = np.argpartition(res, -nth)[-nth:]
      index = ind[np.argsort(res[ind])]
      index = index[::-1]

      # index = sorted(range(len(output)), key=lambda k: output[k])
		  # index = index[::-1]
      for i in index:
        if i not in actions:
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

    better = getBetterPrediction(final_result, actions, 0)[0][:4]
    print("Better Prediction MAX :" + str(better))
    better = getBetterPrediction(final_result, actions, 1)[0][:4]
    print("Better Prediction AVG :" + str(better))
  
    #print("Expected answer:" + str(result))
    print("Result:" + str(result) + " Source:" + str(name)) 

    if len(actions) <= 1:
      continue
    if len(actions) == 2 and performed == None:
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
  pass
def getSimilarActionPerformed(recordId):
  pass


if __name__ == "__main__":
  torch.multiprocessing.set_start_method('spawn')
  read_json()
  clearLocalData()
  fetchDatabaseData()
  showGameHistory()