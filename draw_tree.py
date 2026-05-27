import math
import os
import sqlite3
from pyvis.network import Network
import networkx as nx
import matplotlib.pyplot as plt

from read_game_data_json import Action

G = nx.DiGraph()

action_list = {}
states = {}

conn = sqlite3.connect(os.getcwd() +'/cardData.cdb')
c = conn.cursor()

print("fetch action list")
c.execute('SELECT rowid, Name, Action FROM L_ActionList')
records = c.fetchall()
for record in records:
  action_list[record[0]] = Action(record[0], record[1], record[2])

c.execute('SELECT rowid, * FROM ComboLines')
records = c.fetchall()

for record in records:
    rowid = record[0]
    heuristic = record[1]
    heuristicBest = record[2]
    parentid = record[3]
    actionid = record[4]
    isend = record[5]
    name = record[6]
    isFirst = record[7]
    visited = record[8]

    action = action_list[int(actionid)]
    node_name = str(rowid) + action.name + action.action
    G.add_node(node_name)
    states[rowid] = node_name


c.close()

pos = nx.planar_layout(G)
nx.draw(G, pos, node_size=0, alpha=1, edge_color="r", font_size=16, with_labels=True)
ax = plt.gca()
ax.margins(0.08)
plt.show()

# nt = Network('1000px', '1000px', directed=True)
# # populates the nodes and edges data structures
# # uses PyVis
# nt.from_nx(nx_graph)
# nt.show_buttons(filter_=['physics'])
# nt.show('nx.html')