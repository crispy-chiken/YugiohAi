#https://plotly.com/python/tree-plots/

import igraph
from igraph import Graph, EdgeSeq
import plotly.graph_objects as go

def make_annotations(pos, text, font_size=10, font_color='rgb(250,250,250)'):
    L=len(pos)
    if len(text)!=L:
        raise ValueError('The lists pos and text must have the same len')
    annotations = []
    for k in range(L):
        annotations.append(
            dict(
                text=labels[k], # or replace labels with a different list for the text within the circle
                x=pos[k][0], y=2*M-position[k][1],
                xref='x1', yref='y1',
                font=dict(color=font_color, size=font_size),
                showarrow=False)
        )
    return annotations



import math
import os
import sqlite3

nr_vertices = 25
v_label = list(map(str, range(nr_vertices)))
G = Graph()
lay = G.layout('rt')


conn = sqlite3.connect(os.getcwd() +'/cardData.cdb')
c = conn.cursor()

c.execute('SELECT Count(Visited) FROM MCST WHERE Visited > 0')
total = c.fetchone()[0]
print("Total search made=" + str(total))

const = 0.5

c.execute('SELECT rowid, * FROM MCST ORDER BY rowid')
records = c.fetchall()
group_count = 0

for record in records:
    rowid = int(record[0])
    name = record[1]
    parentid = int(record[2])
    actionid = record[3]
    reward = record[4]
    visited = max(0.0001, record[5])
    isFirst = record[6]
    isTraining = record[7]

    if visited < 1:
      continue

    activation = reward + const * math.sqrt((math.log(total + 1) + 1) / visited)
    activation = min(activation, 25)

    G.add_vertex(rowid)

    if parentid != -4:
      G.add_edge(parentid, rowid)

c.close()

# nt = Network('1000px', '1000px', directed=True)
# # populates the nodes and edges data structures
# nt.from_nx(nx_graph)
# nt.show_buttons(filter_=['physics'])
# nt.show('nx.html')


position = {k: lay[k] for k in range(nr_vertices)}
Y = [lay[k][1] for k in range(nr_vertices)]
M = max(Y)

es = EdgeSeq(G) # sequence of edges
E = [e.tuple for e in G.es] # list of edges

L = len(position)
Xn = [position[k][0] for k in range(L)]
Yn = [2*M-position[k][1] for k in range(L)]
Xe = []
Ye = []
for edge in E:
    Xe+=[position[edge[0]][0],position[edge[1]][0], None]
    Ye+=[2*M-position[edge[0]][1],2*M-position[edge[1]][1], None]

labels = v_label


fig = go.Figure()
fig.add_trace(go.Scatter(x=Xe,
                   y=Ye,
                   mode='lines',
                   line=dict(color='rgb(210,210,210)', width=1),
                   hoverinfo='none'
                   ))

fig.add_trace(go.Scatter(x=Xn,
                  y=Yn,
                  mode='markers',
                  name='bla',
                  marker=dict(symbol='circle-dot',
                                size=18,
                                color='#6175c1',    #'#DB4551',
                                line=dict(color='rgb(50,50,50)', width=1)
                                ),
                  text=labels,
                  hoverinfo='text',
                  opacity=0.8
                  ))


axis = dict(showline=False, # hide axis line, grid, ticklabels and  title
            zeroline=False,
            showgrid=False,
            showticklabels=False,
            )

fig.update_layout(title= 'MCTS',
              annotations=make_annotations(position, v_label),
              font_size=12,
              showlegend=False,
              xaxis=axis,
              yaxis=axis,
              margin=dict(l=40, r=40, b=85, t=100),
              hovermode='closest',
              plot_bgcolor='rgb(248,248,248)'
              )
fig.show()

fig.write_html('network.html', auto_open=False)
