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

from read_game_data_json import getTorchData, getTorchPrediction, compile_data, fetchDatabaseData, Action


action_count = 0
state_count = 0
action_list = {}
compare_to = {}


def clearLocalData():
  global action_list, action_state, compare_to, field_state, play_record, game_result
  
  action_list = {}
  action_state = {}
  compare_to = {}
  field_state = {}
  play_record = {}
  game_result = {}

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

        if name not in compiled:
          compiled[name] = []
        compiled[name].append(game_data)

  print("Action count: " + str(action_count))
  print("state count: " + str(state_count))

  return compiled

def show_data():
  data = read_json()
  while True:
    print("Valid names:" + str(data.keys()))
    name = input("Enter name:")
    while name not in data:
      name = input("Invalid name, valid names:" + str(data.keys()))

    print("len of data for " + name + ":" + str(len(data[name]) - 1))
    for i in range(len(data[name])):
      print(str(i) + " result:" + str(data[name][i][0]["result"]))

    res1 = data[name][get_data(1, len(data[name]) - 1)]
    res2 = data[name][get_data(2, len(data[name]) - 1)]

    print("Result1 len: " + str(len(res1)))
    print("Result2 len: " + str(len(res2)))
    
    for i in range(max(len(res1), len(res2))):
      if i < len(res1) and i < len(res2):
        # Do this so when comparing same data with diferent results, they are the same
        temp_result = res2[i]["result"]
        res2[i]["result"] = res1[i]["result"]

        equal = res1[i] == res2[i]

        res2[i]["result"] = temp_result

        if equal:
          continue
          print_state(res1[i])
        else:
          print(str(i) + "-----")
          print_state(res1[i])
          print(str(i) + "+++++")
          print_state(res2[i])
      elif i < len(res1): # Old is longer
        print(str(i) + "-----")
        print_state(res1[i]) 
      else: # New is longer
        print(str(i) + "+++++")
        print_state(res2[i])
      
      user_in = input("press to continue..., type s to skip")
      if user_in == 's':
        break

def get_data(index, length):
  ind = int(input("enter index for game " + str(index) + ":"))
  while not(0 <= ind <= length):
    ind = int(input("enter index for game" + str(index) +".Valid range is 0-" + str(length) + ":"))
  
  return ind

def print_state(data:typing.Dict):
    actions = data["actions"]
    state = data["state"]
    performed = data["performed"]
    result = data["result"]
    if performed == None:
      print("None")
    else:
      print("(" + str(performed) + ")" + str(action_list[performed]))

    print(" Field State")
    for j in state:
      print("  " + str(compare_to[j]))

    print(" Possible Actions")
    for j in actions:
      print("  (" + str(j) + ")" + str(j == performed) + "| " + str(action_list[j]))
  

def find_first_difference_game(game1, game2):
  diff1,diff2 = None, None

  for i in range(max(len(game1), len(game2))):
    if i < len(game1) and i < len(game2):
      # Do this so when comparing same data with diferent results, they are the same
      temp_result = game2[i]["result"]
      game2[i]["result"] = game1[i]["result"]

      equal = game1[i] == game2[i]

      game2[i]["result"] = temp_result
      
      if equal:
        continue
      else:
        diff1 = game1[i]
        diff2 = game2[i]
        break
    elif i < len(game1): # Old is longer
      print("ERROR Should not have a longer game difference before choice")
    else: # New is longer
      print("ERROR Should not have a longer game difference before choice")
    
  return diff1, diff2

if __name__ == "__main__":
  fetchDatabaseData()
  compile_data()
  show_data()