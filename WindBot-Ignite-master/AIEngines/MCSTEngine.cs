using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using WindBot.Game;
using WindBot.Game.AI;
using WindBot.Game.AI.Decks.Util;
using YGOSharp.OCGWrapper.Enums;

namespace WindBot
{
    public class MCTSEngine : AbstractAIEngine
    {
        public class Node
        {
            public List<Node> Children { get; set; }
            public Node Parent { get; set; }
            public double Rewards { get; set; } = 0;
            public int Visited { get; set; } = 0;
            public long NodeId { get; set; } = -4;
            public ActionInfo Action { get; set; }

            public bool SaveParent { get; set; } = true;
            public bool SaveChild { get; set; } = true;
            public ClientCard Card { get; set; } = null;

            public Node(Node parent, ActionInfo action) : this(parent, null, action)
            {
            }

            public Node(Node parent, Node child, ActionInfo action)
            {
                Children = new List<Node>();
                Parent = parent;
                Action = action;

                if (child != null)
                {
                    Children.Add(child);
                }
                else
                    SaveChild = false;

                if (parent == null)
                {
                    SaveParent = false;
                }

                /*if (!SQLComm.GetNodeInfo(this))
                {
                    SQLComm.InsertNode(this);
                    SQLComm.GetNodeInfo(this);
                }*/
            }

            public override string ToString()
            {
                string s = "";
                s += NodeId.ToString() + " | ";
                s += Action.ToString();

                return s;
            }
        }

        private static List<Node> _nodeMappings = SQLComm.GetAllNodes();

        public List<Node> Path { get; set; }
        public int PathIndex { get; private set; } = 1;

        Node _current { get; set; }
        Node _lastNode { get; set; } = null;
        public List<Node> possibleActions { get; set; }
        int ActionCount { get; set; } = 0;
        int BestActionCount { get; set; } = int.MaxValue;
        public List<History> BestRecord { get; set; } = new List<History>();

        public MCTSEngine(Executor source) : base(source)
        {
            Path = new List<Node>();
            OnNewGame();
        }

        public void OnNewGame()
        {
            possibleActions = new List<Node>();
            _current = GetNode(null, new ActionInfo("Start", "", 0));
            _lastNode = _current;
            if (Path.Count == 0)
                Path.Add(_current);
            PathIndex = 1;
            ActionCount = 0;
        }


        /*
         * For Multiple Actions
         */

        public void AddPossibleAction(ActionInfo action)
        {
            Node node = GetNode(_current, action);
            possibleActions.Add(node);
        }

        /*
         * For single action
         */
        public bool ShouldActivate(ActionInfo action, List<FieldStateValues> comparisons)
        {
            Node toActivate = GetNode(_current, action);
            possibleActions.Add(toActivate);
            //possibleActions.Add(new Node(_current, cardId + "[No]", action));
            Node best = GetNextAction(comparisons);
            return best == toActivate;
        }

        /**
         * Called after setting all possible actions
         */
        public Node GetNextAction(List<FieldStateValues> comparisons, bool pop = false)
        {
            Node best = _current;
            double weight = -1;
            double c = 1;

            if (!SQLComm.IsRollout)
            {
                List<Node> close = new List<Node>();

                foreach (Node n in possibleActions)
                {
                    double visited = Math.Max(0.01, n.Visited);
                    //double estimate = SQLComm.GetNodeEstimate(n);
                    //double w = n.Rewards/visited + c * Math.Sqrt((Math.Log(n.Parent.Visited + 1) + 1) / visited);
                    double w = 0.1 * n.Rewards / visited + c * Math.Sqrt(Math.Log(n.Parent.Visited + 1) / visited); //(n.Parent.Visited) / visited;//
                    //w += estimate;
                    if (CSVReader.InBaseActions(n.Action.Name, n.Action.Action, comparisons) && visited < 10)
                        w += 1;



                    if (Math.Abs(w - weight) < 0.1)
                    {
                        close.Add(n);
                    }
                    else
                    {
                        close.Clear();
                        close.Add(n);
                    }

                    if (w >= weight)
                    {
                        weight = w;
                    }
                }

                //FIXED RNG
                //if (!SQLComm.IsTraining)
                best = close[Program.Rand.Next(close.Count)];

                // Randomness

                if (best != null && best != _current)
                {
                    _current.Children.Add(best);
                    _current = best;
                    Path.Add(best);
                    /*if (best.Visited <= 0 && best.NodeId != 0)
                    {
                        _lastNode = best;
                        SQLComm.IsRollout = true;
                        PathIndex = Path.Count;
                    }*/
                }

            }
            else if (PathIndex < Path.Count && possibleActions.Count > 0)
            {
                foreach (var action in possibleActions)
                {
                    if (action.NodeId == Path[PathIndex].NodeId)
                    {
                        PathIndex++;
                        best = action;
                        break;
                    }
                }

                if (best == _current)
                {
                    Logger.WriteErrorLine("Could not follow saved path!");
                    PathIndex = Path.Count;
                    best = possibleActions[0];
                }

                _current.Children.Add(best);
                _current = best;
            }
            else if (possibleActions.Count > 0)
            {

                List<Node> bestPossible = new List<Node>();

                foreach (var action in possibleActions)
                {
                    if (CSVReader.InBaseActions(action.Action.Name, action.Action.Action, comparisons))
                        bestPossible.Add(action);
                }


                if (bestPossible.Count > 0)
                {
                    if (SQLComm.IsTraining)
                        best = bestPossible[Program.Rand.Next(0, bestPossible.Count)];
                    else
                        best = bestPossible[0];
                }
                else
                {
                    if (SQLComm.IsTraining)
                        best = possibleActions[Program.Rand.Next(0, possibleActions.Count)];
                    else
                        best = possibleActions[0];
                }

                _current.Children.Add(best);
                _current = best;
            }

            if (possibleActions.Count > 1)
                ActionCount++;

            if (pop)
            {
                possibleActions.Remove(best);
            }
            else
            {
                possibleActions.Clear();
            }

            return best;
        }

        public void Clear()
        {
            possibleActions.Clear();
        }


        public override void OnWin(int result)
        {
            // Add any missing nodes
            foreach(var node in _nodeMappings)
            {
                if (node.NodeId == -4)
                {
                    if (node.Visited != 0)
                        Console.WriteLine("Hunh?");
                    SQLComm.InsertNode(node);
                    SQLComm.GetNodeInfo(node);
                }
            }


            bool reset = SQLComm.ShouldBackPropagate;
            double reward = result == 0 ? 1 : 0;
            //double reward = result == 0 ? 1.0 - Math.Min(duel.Turn * 0.01, 0.1) - Math.Min(ActionCount * 0.001, 0.2): 0;
            SQLComm.Backpropagate(Path, _lastNode, reward, ActionCount);// duel.Turn);

            if (reset)
            {
                Path.Clear();
            }

            if (ActionCount < BestActionCount && result == 0)
            {
                BestActionCount = ActionCount;
                BestRecord = Records;
                base.OnWin(result);
            }
            OnNewGame();
        }

        protected override ActionInfo GetBestAction(List<ActionInfo> actions, List<FieldStateValues> comparisons)
        {
            var stopwatch = Stopwatch.StartNew();
           /* Console.WriteLine("Current State:---------");
            comparisons.Reverse();
            foreach (var i in comparisons)
            {
                Console.WriteLine("     " + i.ToString());
            }*/


            foreach (var action in actions)
            {
                AddPossibleAction(action);
            }

            ActionInfo next = GetNextAction(comparisons).Action;

            Logger.DebugWriteLine("MCST - GetBestAction Time:" + stopwatch.Elapsed, ConsoleColor.Green);
            return next;
        }

        private Node GetNode(Node parent,  ActionInfo action, Node child = null)
        {
            Node node = null;
            long parentId = parent?.NodeId ?? -4;

            foreach (var n in _nodeMappings)
            {
                if (action.ActionId != n.Action.ActionId)
                    continue;

                if (parentId != (n.Parent?.NodeId ?? -4))
                    continue;

                if (n.NodeId == -4)
                    continue;

                node = n;
                break;
            }

            if (node == null)
            {
                node = new Node(parent, child, action);
                _nodeMappings.Add(node);
            }

            // Make sure the action is the lastest one
            node.Action = action;

            return node;
        }
    }
}

