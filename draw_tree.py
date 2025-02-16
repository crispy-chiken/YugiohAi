from argparse import Action
import math
import os
import sqlite3
from pyvis.network import Network
import networkx as nx


conn = sqlite3.connect(os.getcwd() +'/cardData.cdb')
c = conn.cursor()

c.execute('SELECT Count(Visited) FROM MCST WHERE Visited > 0')
total = c.fetchone()[0]
print("Total search made=" + str(total))

nx_graph = nx.DiGraph()
const = 0.5

c.execute('SELECT rowid, * FROM MCST')
records = c.fetchall()
group_count = 0
for record in records:
    rowid = record[0]
    name = record[1]
    parentid = record[2]
    actionid = record[3]
    reward = record[4]
    visited = max(0.0001, record[5])
    isFirst = record[6]
    isTraining = record[7]

    if visited < 1:
      continue

    activation = reward + const * math.sqrt((math.log(total + 1) + 1) / visited)
    activation = min(activation, 25)
    nx_graph.add_node(rowid, label=f'{rowid} {isFirst} {isTraining} {actionid}', group=1)
    if parentid != -4:
      nx_graph.add_edge(parentid, rowid, weight=activation, group=2)
    # if childid != -4:
    #   nx_graph.add_edge(rowid, childid, weight=activation, group=3)

    # if (action == "GoToEndPhase"):
    #   group_count += 1

# Get Best average moves?
# c.execute("Select cardId,Action, SUM(Reward) as r,SUM(Visited) as v from MCST WHERE IsTraining='False' group by cardid, action order by cardid, action")
# records = c.fetchall()
# for record in records:
#     cardid = record[0]
#     action = record[1]
#     reward = record[2]
    
#     visited =record[3]
#     #const = 0

#     activation = 0
#     activation2 = 0
#     if (visited > 0):
#       actication = round((reward + const * math.sqrt((math.log(total + 1) + 1) / (visited))) * 100)/100
#       activation2 = round(reward / (visited) * 1000)/10

#     print(f"{activation}| {activation2} | {cardid} | {action}")

c.close()

nt = Network('1000px', '1000px', directed=True)
# populates the nodes and edges data structures
nt.from_nx(nx_graph)
nt.show_buttons(filter_=['physics'])
nt.show('nx.html')