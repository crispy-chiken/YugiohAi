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
    public class PathEngine : AbstractAIEngine
    {
        public class Node
        {
            static int _idcounter = 0;

            public int Id { get; private set; }
            public List<Node> Children { get; set; }
            public Node Parent { get; set; }
            public int Depth { get; set; }
            public int BestDepth { get; set; }
            public ActionInfo Action { get; set; }
            public History History { get; set; }


            public Node(Node parent, ActionInfo action)
            {
                Children = new List<Node>();
                Parent = parent;
                Action = action;
                Id = _idcounter++;
                BestDepth =  int.MaxValue;
                Depth = 0;
                History = null;

            }

            public override string ToString()
            {
                string s = $"[{BestDepth}]";
                if (History != null)
                {
                    s += History.Info.Turn;
                    s += "|";
                    s += History.Info.ActionNumber;
                }
                s += Action.ToString();

                return s;
            }

            /*public bool Equals(Node o)
            {
                if (o == null)
                    return false;
                return o.Action == Action;
            }*/
        }

        List<Node> _possibleActions { get; set; }
        List<Node> _bestPath { get; set; }
        Node _current { get; set; }
        Node _start { get; set; }
        int _depthCount { get; set; }

        public PathEngine(Executor source) : base(source)
        {
            _start = new Node(null, new ActionInfo("Start", "", 0));
            OnNewGame();
        }

        public void OnNewGame()
        {
            _possibleActions = new List<Node>();
            _current = _start;
            _depthCount = 0;
        }



        private void AddPossibleAction(ActionInfo action, History history)
        {
            Node node = new Node(_current, action)
            {
                History = history,
                Depth = _depthCount
            };
            _possibleActions.Add(node);
        }

        /**
         * Called after setting all possible actions
         */
        private Node GetNextAction(List<FieldStateValues> comparisons)
        {
            Node best = _current;
            // Ignore actions that only have 1 option
            if (_possibleActions.Count > 1)
            {
                if (!SQLComm.IsRollout)
                {
                    // Choose the winning action with the least depth
                    if (_current.Children.Count > 0)
                    {
                        _current.Children.Sort(CompareNodeBestDepth);
                        Node selected = _current.Children[0];

                        if (selected.BestDepth == int.MaxValue)
                        {
                            selected = _current.Children[source.Rand.Next(_current.Children.Count)];
                        }

                        if (SQLComm.GamesPlayed > 0)
                        {
                            foreach(var child in _current.Children)
                            {
                                bool inActions = false;
                                foreach(var node in _possibleActions)
                                {
                                    if (node.Action.Equals(child.Action))
                                    {
                                        inActions = true;
                                        break;
                                    }
                                }

                                if (!inActions)
                                {
                                    Logger.WriteErrorLine("Something went wrong, Cant find the same actions in possible actions");
                                }
                            }
                        }

                        foreach (Node node in _possibleActions) 
                        {
                            if (node.Action.Equals(selected.Action))
                            {
                                // Copy the information into selected as that is the node connected to parent
                                selected.Action = node.Action;

                                // Todo check if history is identical
                                selected.History = node.History;
                                best = selected;
                                break;
                            }
                        }
                        if (best == _current)
                        {
                            Logger.WriteErrorLine("Something went wrong, enemy may have changed moves");
                        }
                        
                    }
                    else
                    {
                        // Choose one randomly? or use another method
                        best = _possibleActions[source.Rand.Next(_possibleActions.Count)];
                        _current.Children = _possibleActions;
                    }
                }

                _depthCount++;

                _current = best;
            }
            else
            {
                best = _possibleActions[0];
            }

            _possibleActions = new List<Node>();

            return best;
        }

        public override void OnWin(int result)
        {
            //base.OnWin(result);
            if (result == 0) // Win
            {
                int i = 0;
                while (_current != null)
                {
                    _current.BestDepth = _depthCount;
                    _current = _current.Parent;
                    i++;
                }
            }

            OnNewGame();
        }

        protected override ActionInfo GetBestAction(History history)
        {
            List<ActionInfo> actions = history.ActionInfo;
            List<FieldStateValues> comparisons = history.FieldState;
            var stopwatch = Stopwatch.StartNew();

            foreach (var action in actions)
            {
                AddPossibleAction(action, history);
            }

            ActionInfo next = GetNextAction(comparisons).Action;

            //Logger.DebugWriteLine("PathEngine - GetBestAction Time:" + stopwatch.Elapsed, ConsoleColor.Green);
            return next;
        }

        private static int CompareNodeBestDepth(Node a, Node b)
        {
            if (a.BestDepth < b.BestDepth)
                return -1;
            if (a.BestDepth == b.BestDepth)
                return 0;
            return 1;
        }
    }
}

