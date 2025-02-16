import datetime
import glob
import math
import os
import shutil
import sqlite3
import string
import subprocess
import sys
from threading import Thread
import time
import random
import typing
from sys import platform
from pathlib import Path
import psutil

# import tensorflow as tf
# from keras.models import Sequential
# from keras.layers import Dense, Flatten
# from keras.models import load_model

import multiprocessing
from export_database import export_database
import read_game_data_json as read_game_data
import get_action_weights as get_action_weights

import torch


#The Deck name and location	
AI1Deck = 'AIBase'
AI2Deck =  'AIBase'#'Swordsoul'#AI_Combined
AIMaster = 'Swordsoul'#'Master'#

deck1 = 'AI_SnakeEyes.ydk'#'AI_Random1.ydk'#'AI_SnakeEyes.ydk'
deck2 = 'AI_SnakeEyes.ydk'#'AI_Random1.ydk'#'AI_Random2.ydk'

totalGames = 10 # Games before any of the below
cycles = 10 # Trains decks
generations = 1 # Shuffles Decks
parallelGames = 1

rolloutCount = 1
isFirst = True
isTraining = True
winThresh = 100
pastWinLim = 5
idNumber = 1

reset = False
swap = False

def isrespondingPID(PID):
  #if platform == "linux" or platform == "linux2":
  return True

  #https://stackoverflow.com/questions/16580285/how-to-tell-if-process-is-responding-in-python-on-windows
  os.system('tasklist /FI "PID eq %d" /FI "STATUS eq running" > tmp.txt' % PID)
  tmp = open('tmp.txt', 'r')
  a = tmp.readlines()
  tmp.close()
  try:
    if int(a[-1].split()[1]) == PID:
      return True
    else:
      return False
  except:
    return False

def deleteData():

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
  
  folder = './GameData'
  for filename in os.listdir(folder):
      file_path = os.path.join(folder, filename)
      try:
          if os.path.isfile(file_path) or os.path.islink(file_path):
              os.unlink(file_path)
          elif os.path.isdir(file_path):
              shutil.rmtree(file_path)
      except Exception as e:
          print('Failed to delete %s. Reason: %s' % (file_path, e))

def resetDB():

  deleteData()
  dbfile = './cardData.cdb'
  con = sqlite3.connect(dbfile)
  cur = con.cursor()
  sql_delete_query = """DELETE from L_ActionList"""
  cur.execute(sql_delete_query)
  sql_delete_query = """DELETE from L_CompareTo"""
  cur.execute(sql_delete_query)
  sql_delete_query = """DELETE from L_PlayRecord"""
  cur.execute(sql_delete_query)
  sql_delete_query = """DELETE from L_FieldState"""
  cur.execute(sql_delete_query)
  sql_delete_query = """DELETE from L_ActionState"""
  cur.execute(sql_delete_query)
  sql_delete_query = """DELETE from L_Weights"""
  cur.execute(sql_delete_query)
  sql_delete_query = """DELETE from L_GameResult"""
  cur.execute(sql_delete_query)
  con.commit()
  con.close()

def resetMCTS():
  dbfile = './cardData.cdb'
  con = sqlite3.connect(dbfile)
  cur = con.cursor()
  sql_delete_query = """DELETE from MCST"""
  cur.execute(sql_delete_query)
  con.commit()
  con.close()

def softResetDB():
  dbfile = './cardData.cdb'
  con = sqlite3.connect(dbfile)
  cur = con.cursor()
  sql_delete_query = """DELETE from L_ActionList"""
  cur.execute(sql_delete_query)
  sql_delete_query = """DELETE from L_CompareTo"""
  cur.execute(sql_delete_query)
  sql_delete_query = """DELETE from L_PlayRecord"""
  cur.execute(sql_delete_query)
  sql_delete_query = """DELETE from L_FieldState"""
  cur.execute(sql_delete_query)
  sql_delete_query = """DELETE from L_ActionState"""
  cur.execute(sql_delete_query)
  sql_delete_query = """DELETE from L_Weights"""
  cur.execute(sql_delete_query)
  sql_delete_query = """DELETE from L_GameResult"""
  cur.execute(sql_delete_query)
  con.commit()
  con.close()

def resetYgoPro():
  print("deleting old deck files from ygopro")
  files = glob.glob(os.getcwd() +"/ProjectIgnis/deck/*")
  for f in files:
      os.remove(f)
    
  print("deleting old replays from ygopro")
  files = glob.glob(os.getcwd() +"/ProjectIgnis/replay/*")
  for f in files:
      os.remove(f)

def parseArg():
  global reset, isTraining, repeatFor, matches, totalGames, isFirst, swap

  reset = len(sys.argv)>1 and ("--reset" in sys.argv or "-r" in sys.argv)
  swap = len(sys.argv)>1 and ("--swap" in sys.argv or "-s" in sys.argv)
  isTraining = len(sys.argv)>1 and ("--training" in sys.argv or "-t" in sys.argv)
  isFirst = len(sys.argv)>1 and ("--first" in sys.argv or "-f" in sys.argv)

  if "--repeat" in sys.argv:
    repeatFor = int(sys.argv[sys.argv.index("--repeat")+1])
  if "--matches" in sys.argv:
    matches = int(sys.argv[sys.argv.index("--matches")+1])
  if "--games" in sys.argv:
    totalGames = int(sys.argv[sys.argv.index("--games")+1])

def runAi(Deck = "Random1", 
          DeckFile = "",
          ExportPath="",
          Name = "Random1",
          Hand = 0,
          TotalGames = 1,
          IsFirst = True,
          IsTraining = True,
          ShouldUpdate = True,
          ShouldRecord = True,
          Id = 0,
          Port = 7911,
          IsMCTS= False
          ):
  currentdir = os.getcwd()
  os.chdir(os.getcwd()+'/WindBot-Ignite-master/bin/Debug')

  file_name = "WindBot.exe"

  if platform == "linux" or platform == "linux2":
    file_name = os.getcwd() + "/WindBot.exe"

  arguments = [file_name,"Deck="+Deck,
                        "DeckFile="+DeckFile,
                        "ExportPath="+ExportPath,
                        "Name="+str(Name),
                        "Hand="+str(Hand),
                        "IsTraining="+str(IsTraining), 
                        "ShouldUpdate="+str(ShouldUpdate), 
                        "ShouldRecord="+str(ShouldRecord),
                        "TotalGames="+str(TotalGames), 
                        "IsFirst="+str(IsFirst), 
                        "Id="+str(Id),
                        "Port="+str(Port),
                        "IsMCTS="+str(IsMCTS)
                        ]
  print(" ".join(arguments))

  p = subprocess.Popen( arguments,
                        stdout=subprocess.DEVNULL
                        )
    
  os.chdir(currentdir)
  
  return p

def shuffle_deck(deck_name):
  filePath = os.getcwd() + '/edopro_bin/deck/' + deck_name 

  f = open(filePath,"r")
  main = []
  extra = []
  side = []
  part = 0
  for line in f.readlines():

    if "#extra" in line:
      part = 1
      continue
    elif "!side" in line:
      part = 2
      continue
    elif "#main" in line:
      part = 0
      continue
    elif "#" in line:
      continue

    if part == 0:
      main.append(line.strip())
    elif part == 1:
      extra.append(line.strip())
    else:
      side.append(line.strip())
    random.shuffle(main)
    random.shuffle(extra)
    random.shuffle(side)

  f.close()

  f = open(filePath, "w")    
  f.write("#created by deck_maker_ai\n")

  f.write("#main\n")
  for i in main:
    f.write(i +'\n')
  f.write("#extra\n")
  for i in extra:
    f.write(i +'\n')
  f.write("!side\n")
  for i in side:
    f.write(i +'\n')    

  f.close()

def swap_decks(deck1, deck2, name1, name2):
  if deck1 == deck2:
    return
  path = os.getcwd() + '/edopro_bin/deck/'
  File1 = path + deck1
  File2 = path + deck2

  os.rename(File2, path + 'temp')
  os.rename(File1, File2)
  os.rename(path + 'temp', File1)

def setup():
  global AI2Deck, AI2Deck, isTraining, totalGames

  if reset:
    if isTraining:
      resetDB()
    shuffle_deck(deck1)
    shuffle_deck(deck2)
    # src_dir=os.getcwd() + '/edopro_bin/deck/' + deck1
    # dst_dir=os.getcwd() + '/edopro_bin/deck/' + deck2
    # shutil.copy(src_dir,dst_dir)

  resetYgoPro()

  # if not isTraining:
  #   AI2Deck = AIMaster

def main_game_runner(isTraining, totalGames, Id1, Id2, Deck1, Deck2, port, isFirst):
  start = time.time()

  #subprocess.Popen - does not wait to finish
  #subprocess.run - waits to finish

  file_path = os.getcwd() + "/edopro_bin/ygoprodll.exe"
  if platform == "linux" or platform == "linux2":
    file_path = str(Path(__file__).resolve().parent.parent) + "/ProjectIgnisLinux/ygopro"
 
  g = subprocess.Popen([file_path, "-p", str(port)], stdout=subprocess.DEVNULL)


  while(g.poll() == None and not isrespondingPID(g.pid)):
    time.sleep(1)

  time.sleep(8)
  
  print("	runningAi1 " + str(Id1) + ":" + Deck1)

  p1 = runAi( Deck = 'AIBase', 
             DeckFile= os.getcwd() + '/edopro_bin/deck/'+Deck1,
             ExportPath= os.getcwd() + '/GameData',
              Name = Id1,
              Hand = 2,
              TotalGames = totalGames,
              IsFirst = (isFirst),
              IsTraining = isTraining,
              ShouldUpdate= True,#isTraining,
              Id = Id1,
              Port = port,
              IsMCTS=True#False#
            )
  time.sleep(1)
  print("	runningAi2 "+ str(Id2) + ":" + Deck2)
  p2 = runAi(Deck = 'AIBase', 
             DeckFile=os.getcwd() + '/edopro_bin/deck/'+Deck2,
             ExportPath= os.getcwd() + '/GameData',
              Name = Id2,
              Hand = 3,
              TotalGames = totalGames,
              IsFirst = (not isFirst),
              IsTraining = isTraining,
              ShouldUpdate= True,#True,#isTraining,#
              ShouldRecord=False,
              WinsThreshold = winThresh,
              PastWinsLimit = pastWinLim,
              Id = Id2,
              Port = port,
              IsMCTS=False
            )
  
  #psutil.Process(g.pid).nice(psutil.BELOW_NORMAL_PRIORITY_CLASS)
  #psutil.Process(p1.pid).nice(psutil.BELOW_NORMAL_PRIORITY_CLASS)
  #psutil.Process(p2.pid).nice(psutil.BELOW_NORMAL_PRIORITY_CLASS)

  if (p1.poll() == None or p2.poll() == None):
    time.sleep(1)
  
  if (not (p1.poll() == None or p2.poll() == None)):
    print("	WARNING! ai is not running")

  timer = 0
  timeout = 30 * 60 # Length of run
  
  # print(p1.pid)
  # print(p2.pid)
  #make sure the game does not run longer than needed
  #ends the ygopro program as soon as the ais are done. Ais play faster than what you see.
  
  while (p1.poll() == None or p2.poll() == None):
    continue
     
  if platform == "linux" or platform == "linux2":
    os.system("kill -9 " + str(g.pid))
  else:
    os.system("	TASKKILL /F /IM ygoprodll.exe")
  
  end = time.time()

  print("Time Past:" + str(datetime.timedelta(seconds=int(end - start))))
  print("Average Game Time:"+str(datetime.timedelta(seconds=int((end - start)/(totalGames)))))

def main():
  global reset, totalGames, generations, cycles, swap, isFirst, deck1, deck2
  parseArg()
  setup()

  # if not reset:
  #   read_game_data.createBetterDataset()
  #   softResetDB()

  for g in range(generations):
    
    if reset:
      shuffle_deck(deck1)
      shuffle_deck(deck2)
    
    for c in range(cycles):
      #read_game_data.read_data()
      #get_action_weights.fetchDatabaseData()
      get_action_weights.load_data()
      if reset or swap:
        resetMCTS()
      #if swap:
        #swap_decks(deck1, deck2)
        #isFirst = not isFirst

      #resetDB()
      proc = multiprocessing.Process(target=get_action_weights.run_server, args=())
      proc.start()

      jobs = []
      pairs = []
      bots = list(range(parallelGames * 2))
      while bots:
        r1 = bots.pop(0)
        r2 = bots.pop(0)
        if swap and (c + g) % 2 == 1:
          pairs.append((r2, r1))
        else:
          pairs.append((r1, r2))    

      for i in range(len(pairs)):
        print("generation " + str(g) + " cycle: " + str(c) +" running game " + str(i) + ":" + str(pairs[i][0]) + "vs" + str(pairs[i][1]) + ": Total games " + str(totalGames))
        port = 7911 + i
        p = multiprocessing.Process(target=main_game_runner, args=(isTraining, totalGames, str(pairs[i][0]), str(pairs[i][1]), deck1, deck2, port, isFirst))
        #psutil.Process(p.pid).nice(psutil.BELOW_NORMAL_PRIORITY_CLASS)
        jobs.append(p)
        p.start()
      
      for job in jobs:
        job.join()
      
      proc.terminate()  # sends a SIGTERM
      print("done cycle")

      # Swap the decks
      if swap:
        isFirst = not isFirst
      
      read_game_data.read_data()

    if g <= generations - 1:
      limit = parallelGames * cycles
      
      #read_game_data.read_data()
      #read_game_data.createBetterDataset()
      #softResetDB()
  read_game_data.read_data()
  export_database()

if __name__ == "__main__":
  torch.multiprocessing.set_start_method('spawn')
  main()
