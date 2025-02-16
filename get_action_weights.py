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
from typing import Any
import numpy as np
import pickle
import glob

from http.server import BaseHTTPRequestHandler, HTTPServer
from scipy.linalg import lstsq
from sklearn.model_selection import train_test_split
from sklearn.linear_model import LinearRegression
from sklearn.ensemble import GradientBoostingRegressor
from sklearn.ensemble import RandomForestRegressor
from sklearn.neural_network import MLPClassifier

# import tensorflow as tf
# from keras.models import Sequential
# from keras.layers import Dense, Flatten
# from keras.models import load_model
# from keras import optimizers

import torch
import torch.nn as nn

#from read_game_data  import fetchDatabaseData, getTorchData, getBetterPrediction, getTorchPrediction, read_data
from read_game_data_json import read_json, getTorchData, getTorchPrediction
import read_game_data_json

action_data = None

#Torch settings
dtype = torch.float
device = torch.device('cuda' if torch.cuda.is_available() else 'cpu')
# print('Using device:', device)
# print()


def load_data():
  read_json()
  global action_data#, compare_count, action_count

  action_data = getTorchData()

  # conn = sqlite3.connect(os.getcwd() +'/cardData.cdb')
  # c = conn.cursor()
  # c.execute('SELECT max(rowid) FROM L_CompareTo')
  # compare_count = c.fetchone()[0]
  # c.execute('SELECT max(rowid) FROM L_ActionList')
  # action_count = c.fetchone()[0]
  # conn.close()

  # print("Compare Count:" + str(compare_count))

def get_predictions(data: typing.List[int], actions: typing.List[int], key_name: string):
  global action_data
  state_count, action_count = read_game_data_json.state_count, read_game_data_json.action_count
  if (action_data == None):
    return []

  if key_name not in action_data.keys():
    if "master" in action_data.keys():
      key_name = "master"
    else:
      return []
  
  final_result = {}
  

  input_length = 1 + state_count + 1 + action_count
  input_list = [0] * (input_length)

  if data[0] != '': # Some fail safe since a 0 data entry is ['']
    for id in data:
      index = int(id)
      if (index < len(input_list) and index >= 0):
        input_list[index] = 1

  for id in actions:
    index = state_count + 1 + int(id) 
    if (index < len(input_list) and index >= 0):
      input_list[index] = 1

  # for id in actions:
  #     if int(id) > action_count:
  #       continue

  #     input_list[state_count + 1 + int(id)] = -1

  # for id in actions:
  #     if int(id) > action_count:
  #       continue

  #     input_list[int(id) - 1 + state_count] = 1
  #     predict = getTorchPrediction(action_data, [input_list])
  #     percentage = 0
  #     for key in predict:
  #       percentage = predict[key][0]
  #       if key not in final_result:
  #         final_result[key] = [0] * (1 + action_count)
  #       final_result[key][int(id)] = percentage#str(percentage)
          
  #     input_list[int(id) - 1 + state_count] = 0#-1
  # print(action_data)
  # print(len(input_list))
  final_result = getTorchPrediction(action_data, [input_list])
      

  # ind = np.argpartition(final_result[key_name], -4)[-4:]
  # index = ind[np.argsort(final_result[key_name][ind])]
  # index = index[::-1]

  result = []
  for i in range(1 + action_count):
    avg = 0.0
    avg2 = 0.0
    cnt = 0
    for key in final_result:
      avg += final_result[key][i]
      if round(final_result[key][i]) > 0 and round(final_result[key][i]) < 100:
        avg2 += final_result[key][i]
        cnt += 1
    if len(final_result) > 0:
      avg /= len(final_result)
    
    cnt = max(1,cnt)
    avg2 /= cnt

    result.append(str(avg))

  result = []
  text = "master:"
  for i in range(1 + action_count):
    result.append(str(final_result["master"][i]))

  for i in actions:
    if int(i) > action_count:
      continue
    text += "[" + str(i) + "]" + ":" + str(round(float(result[int(i)])*100)) + ","
  print(text)


  result = []
  text = key_name + ":"
  for i in range(1 + action_count):
    result.append(str(final_result[key_name][i]))

  for i in actions:
    if int(i) > action_count:
      continue
    text += "[" + str(i) + "]" + ":" + str(round(float(result[int(i)])*100)) + ","
  print(text)
  
  # better = getBetterPrediction(final_result, 0)[0]
  # final_result.clear()
  # final_result = [0] * len(better[0])
  # for b in better:
  #   final_result[b[0]] = b[1]
  
  return result

def run_command_line():
  while True:
    data = input("Enter input data\n").split(' ')
    actions = input("Enter Actions\n").split(' ')
    predictions = get_predictions(data, actions)

    for action in predictions:
      print("Action" + str(action) + ":" +str(predictions[action]))

class handler(BaseHTTPRequestHandler):
    def do_POST(self):
        content_len = int(self.headers.get('Content-Length'))
        get_body = self.rfile.read(content_len).decode()
        raw_data = json.loads(get_body)

        # print(raw_data["data"])
        # print(raw_data["actions"])

        data = raw_data["data"].split(' ')
        actions = raw_data["actions"].split(' ')
        name = raw_data["name"].split(' ')[0]
        predictions = get_predictions(data, actions, name)

        self.send_response(200)
        self.send_header('Content-type','text/html')
        self.end_headers()

        # print(predictions)
        message = json.dumps(predictions)
        self.wfile.write(bytes(message, "utf8"))

    def log_message(self, format: str, *args: Any) -> None:
      return
      return super().log_message(format, *args)

def run_server():
  with HTTPServer(('', 8000), handler) as server:
      server.serve_forever()

if __name__ == "__main__":
  global input_length
  input_length = 1
  torch.multiprocessing.set_start_method('spawn')
  #fetchDatabaseData(True)
  load_data()
  print("ready")
  #run_command_line()
  run_server()