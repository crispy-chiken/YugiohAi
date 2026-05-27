import os
import sqlite3
import matplotlib.pyplot as plt
import networkx as nx


conn = sqlite3.connect(os.getcwd() +'/cardData.cdb')
c = conn.cursor()

c.execute('SELECT Count(Visited) FROM MCST WHERE Visited > 0')


def mbox_graph():
    G = nx.MultiDiGraph()  # create empty graph
    G.add_edge("alice@edu", "bob@go", message="A")
    G.add_edge("alice@edu", "carl@go", message="A")

    return G


G = mbox_graph()

# print edges with message subject
for u, v, d in G.edges(data=True):
    print(f"From: {u} To: {v} Subject: {d}")

pos = nx.planar_layout(G)
nx.draw(G, pos, node_size=0, alpha=1, edge_color="r", font_size=16, with_labels=True)
ax = plt.gca()
ax.margins(0.08)
plt.show()