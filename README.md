# Yugioh Ai
This is a Yugioh Ai Simulator.
The final build will have Ai find the best deck as well as play optimally.

[Discord](https://discord.gg/ZtdQ7cmuUR)

## Video
[![AI Learns Snake eyes combos](https://img.youtube.com/vi/boo75sCnxpM/0.jpg)](https://www.youtube.com/watch?v=boo75sCnxpM)

[![AI Plays Yugioh](https://img.youtube.com/vi/dxBGokCkbrE/0.jpg)](https://www.youtube.com/watch?v=dxBGokCkbrE)


## Breakdown
There are 2 Ais that need to made. A deck building Ai which is written in python. And a game Ai written in c#.

### Setup
* Use Project Ignis Edopro OR if building EdoPro, see https://github.com/edo9300/edopro/wiki/
* You will also need to edit the EDOPro's code in order to launch LAN mode and start the game automaticly when running from the python scripts. See ChangestoEdopro.txt
* Download [sqllite expert personal](http://www.sqliteexpert.com/download.html) to read the .cdb database files
* Copy the edopro files into the folder called edopro_bin
* Copy the cards.cdb and any other .cdb files into the WindBot-Ignite-master/bin/Debug folder
* In Windbot-Ignite in Programs.cs line 88, add all the .cdb file in use like
```
  dbPaths = new string[]{
            Path.GetFullPath("cards.cdb"),
            Path.GetFullPath("cards.delta.cdb"),
            Path.GetFullPath("release-rota.cdb"),
        };
 ```
### How to run
* (In progress) Run python train_ai.py -t -r to train the ai. -t is to set the flag to train, -r is to set the flag to reset the database
* ```read_game_data.py``` will train all the data in the database. Add the flag -f to see what it thinks which action to take, use -a to train with all the data
* ```get_action_weights.py``` will run an http server which windbot uses. You must run this if you are going to manualy run the AI.
* After building the Windbot, in the bin/Debug file, you can run a pre program AI using this command ``` .\WindBot.exe Deck=Master Hand=2 Name=Random2 TotalGames=25 IsTraining=false IsFirst=false ``` and run a trained AI using this command ``` .\WindBot.exe Deck=Random1 Hand=1 Name=Random1 TotalGames=100 IsFirst=true IsTraining=false IsManual=false ``` be sure to run the python server by using the command ```get_action_weights.py```.
* To run an AI on manualy train mode, add the commands IsManual=True ShouldUpdate=true Deck=AIBase, and add the deck you want to play with DeckFile=DECKFILE_PATH
* Input Parameters for windbot
  * Name - Name of the bot, this is also kept track of in training if deckfile is not specified
  * Deck - The ai bot to use, AI_Base is the Selftraining bot program (WIP), AI_HardCodedBase Contains a bunch of AI code (Untested) combined into one file.
  * DeckFile - The Path to a file for the ai to read and use as a deck, .ydk files only
  * Port - the port to join on
  * Hand - Rock paper scissors - 0,1,2 ?
  * RolloutCount - For MCTS, How many rollouts to perform
  * TotalGames - How many games to play before stopping
  * IsFirst - For some AIs to choose first or second
  * IsMCTS - (Old) to set the ai engine to use mcts
  * IsHardCoded - ???
  * IsTraining - Makes AI_Base moves more random? or for MCTS makes it search the tree more
  * ShouldUpdate - If the Ai should record the data into the database (It will still record some information)
  * WinThreshold - The win percentage before it should stop
  * PastWinLimit - How many previous games it should check for the win threshold count.
  * Id - The AI Unique Id (Idk if I use this)
  * IsManual - For AI_Base to let the player manually enter moves
### Editing the ai
* Notable files in Windbot are Logger.cs, GameAI.cs and RandomExecutorBase.cs
* Logger.cs controls the Weights when they are updated and read
* GameAI controls how the ai works
* RandomExecutorBase controls all the math and decision the AI makes.
* In order to run the AI on EDOPro, you need to update the version number of the windbot in WindbotInfo.cs. You can get the version number from EDOPro by running a LAN game, adding a bot, and copying it's launch arguments
### Using 
* Project Ignis EDOPRO-core https://github.com/ProjectIgnis/EDOPro-Core
* Project Ignis Windbot https://github.com/ProjectIgnis/windbot
