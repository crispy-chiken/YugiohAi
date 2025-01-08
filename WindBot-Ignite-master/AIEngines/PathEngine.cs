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
        public class PathNode
        {
            public ActionInfo Action { get; set; }
            public History History { get; set; }
            public int BestDepth { get; set; }
            public bool Visited { get; set; }
            public bool IsManditory { get; set; }
            public bool Skip { get; set; }

            public PathNode(Node node)
            {
                Action = node.Action;
                History = node.History;
                BestDepth = node.BestDepth;
                Visited = false;
                IsManditory = false;
                Skip = false;
            }

            public PathNode(ActionInfo action, PathNode node)
            {
                Action = action;
                History = node.History;
                BestDepth = node.BestDepth;
                Skip = false;
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
                s += "|" + Visited;
                s += "|" + IsManditory;

                return s;
            }
        }

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

            public Node(Node node)
            {
                Id = node.Id;
                Depth = node.Depth;
                BestDepth = node.BestDepth;
                Action = node.Action;
                History = node.History;
                Children = new List<Node>();
            }

            public List<Node> GetSibilings()
            {
                if (Parent == null)
                    return new List<Node>();
                return Parent.Children.Where(x => x != this).ToList();
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
        List<PathNode> _bestPath { get; set; }
        List<PathNode> _nextPath { get; set; }
        List<History> BestRecord { get; set; }
        Node _current { get; set; }
        Node _start { get; set; }
        int _depthCount { get; set; }
        int _pathIndex { get; set; }

        NeuralNet net { get; set; }

        public PathEngine(Executor source) : base(source)
        {
            _start = new Node(null, new ActionInfo("Start", "", 0));
            _bestPath = new List<PathNode>();
            _nextPath = new List<PathNode>();
            net = new NeuralNet(source);
            BestRecord = new List<History>();
            OnNewGame();
        }

        public void OnNewGame()
        {
            _possibleActions = new List<Node>();
            _current = _start;
            _depthCount = 0;
            _pathIndex = 0;
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
        private Node GetNextAction(List<FieldStateValues> comparisons, History history)
        {
            Node best = _current;
            // Ignore actions that only have 1 option
            if (_possibleActions.Count > 0)
            {
                // Sanity check
                if (_current.Children.Count > 0)
                {
                    foreach (var child in _current.Children)
                    {
                        bool inActions = false;
                        foreach (var node in _possibleActions)
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

                if (_pathIndex < _nextPath.Count)
                {
                    if (_current.Children.Count == 0)
                        _current.Children = _possibleActions;

                    // Follow _nextPath
                    PathNode selectedPath = _nextPath[_pathIndex];
                    if (selectedPath.Skip)
                    {
                        _pathIndex++;
                        if (_pathIndex < _nextPath.Count)
                            selectedPath = _nextPath[_pathIndex];
                    }
                    var currentIndex = _pathIndex;

                    while (selectedPath.History?.Info.Turn <= source.Duel.Turn && currentIndex < _nextPath.Count)
                    {
                        selectedPath = _nextPath[currentIndex];

                        best = _current.Children.FirstOrDefault(x => x.Action.Equals(selectedPath.Action));
                        if (best == null)
                        {
                            Logger.WriteLine("!Something went wrong, Cant find the same actions in _current.Children actions");
                        }
                        else if (!(currentIndex > _pathIndex && selectedPath.Action.Name == DONT_PERFORM_STR)) // ignore don't perform actions in path if looking aheadw
                        {
                            if (_possibleActions.Any(x => x.Action.Equals(best.Action)))
                            {
                                _pathIndex = currentIndex;
                                break;
                            }
                            else
                            {
                                Logger.WriteLine("!Something went wrong, Cant find the same actions in possibleActions");
                            }
                        }
                        else
                        {

                        }

                        currentIndex += 1;

                    }

                    if (selectedPath.History?.Info.Turn > source.Duel.Turn && currentIndex - _pathIndex > 1)
                    {
                        
                        best = _current.Children.FirstOrDefault(x => x.Action.Equals(_nextPath[_pathIndex].Action));
                        if (best == null)
                        {
                            // New action has appeared on the list, probably don't perform
                            best = FindSkipAction(_possibleActions);
                            if (best == null) // Something has gone wrong
                                best = GetBestNeuralNet(_possibleActions, history); //if you are here, probably is a selection that isn't there due to removing a node in the path?
                            _pathIndex--; // Stay on the same node
                        }
                        else
                        {
                            // No actions, this action must be manditory, perfrom action and check next action for skip, or a new action has appeared
                            if (_pathIndex < _nextPath.Count - 1)
                                _nextPath[_pathIndex + 1].Skip = true;
                        }
                        selectedPath.Skip = false;
                    }
                    else if (selectedPath.History?.Info.Turn > source.Duel.Turn)
                    {
                        best = FindSkipAction(_possibleActions);
                        if (best == null) // Something has gone wrong
                            best = GetBestNeuralNet(_possibleActions, history);// probably some select
                        _pathIndex--; // Stay on the same node
                    }


                    _pathIndex++;
                }
                else // No path yet
                {
                    if (_pathIndex >= _nextPath.Count && _nextPath.Count > 0)
                        Logger.WriteLine("Going off path");
                    // Choose the winning action with the least depth
                    if (_current.Children.Count > 0)
                    {
                        _current.Children.Sort(CompareNodeBestDepth);
                        best = _current.Children[0];

                        if (best.BestDepth == int.MaxValue)
                        {
                            // Try a random one
                            best = _current.Children[source.Rand.Next(_current.Children.Count)];
                            //best = GetBestNeuralNet(_current.Children, history);
                        }
                    }
                    else
                    {
                        // Choose one randomly? or use another method
                        if (source.Rand.NextDouble() >= 0.5 || !SQLComm.IsTraining)
                            best = GetBestNeuralNet(_possibleActions, history);
                        else
                            best = _possibleActions[source.Rand.Next(_possibleActions.Count)];
                        _current.Children = _possibleActions;
                    }
                }

                if (best != null)
                {
                    foreach (Node node in _possibleActions)
                    {
                        if (node.Action.Equals(best.Action))
                        {
                            // Copy the information into selected as that is the node connected to parent
                            best.Action = node.Action;

                            // Todo check if history is identical
                            best.History = node.History;
                            break;
                        }
                    }
                }

                if (best == null || best == _current)
                {
                    Logger.WriteErrorLine("Something went wrong, enemy may have changed moves");
                    best = FindSkipAction(_possibleActions);
                    if (best == null) // Something has gone wrong?
                        best = GetBestNeuralNet(_possibleActions, history);// could be a selection 
                    _current.Children = _possibleActions;
                }

                _depthCount++;
                foreach(var node in _current.Children)
                {
                    node.History = _possibleActions[0].History;
                }
                _current = best;
                Logger.WriteLine($"Playing action: {best}");
            }
            else
            {
                best = _possibleActions[0];
            }

            // reset possible actions
            _possibleActions = new List<Node>();

            return best;
        }

        private Node GetBestNeuralNet(List<Node> inputs, History history)
        {
            ActionInfo action = net.GetBestAction(history);
            Node node = inputs.Where(x => x.Action.Equals(action)).FirstOrDefault();
            if (node == null)
            {
                node = inputs[source.Rand.Next(inputs.Count)];
                Logger.WriteErrorLine("No matchin inputs");
            }
            return node;
        }

        public override void OnWin(int result)
        {
            var cur = _current;
            Logger.WriteLine($"Result:{result}");
            int bestActionCount = CountActions(_bestPath);

            //base.OnWin(result);

            if (result == 0) // Win
            {
                // Count the number of actions performed
                int actionCount = CountActions(cur) - 1; // Don't count start          

                List<PathNode> path = new List<PathNode>();
                while (cur != null)
                {
                    Logger.WriteLine(cur.ToString());

                    path.Add(new PathNode(cur));
                    cur.BestDepth = actionCount;
                    cur = cur.Parent;
                }

                path.Reverse();
                path.RemoveAt(0);

                // Set best path
                if (_bestPath.Count == 0)
                {
                    _bestPath = path;
                    BestRecord = Records;
                }
                else if (_bestPath.Count >= path.Count && bestActionCount >= actionCount)
                {
                    // update path's manditory and path's visited values
                    for(int i = 0; i < _bestPath.Count; i++)
                    {
                        if (_bestPath[i].Action.Equals(path[i].Action))
                        {
                            path[i].Visited = _bestPath[i].Visited;
                            path[i].IsManditory = _bestPath[i].IsManditory;
                        }
                        else // TODO Optimize this
                        {
                            Logger.DebugWriteLine("End of similarities");
                            break;
                        }
                    }

                    _bestPath = path;
                    BestRecord = Records;
                    bestActionCount = actionCount;
                }
            }
            else if (_bestPath.Count > 0)
            {

                // Mark as faliure
                while (cur != null)
                {
                    Logger.WriteLine(cur.ToString());
                    if (cur.BestDepth == int.MaxValue)
                    {
                        cur.BestDepth = -1;
                    }
                    cur = cur.Parent;
                }
            }

            // Set next path to follow
            if (_bestPath.Count > 0)
            {
                Logger.WriteLine("Best Path");
                // Print Out Best Path
                foreach (var node in _bestPath)
                    Logger.WriteLine(node.ToString());


                _nextPath = new List<PathNode>(_bestPath);

                // Try and remove nodes now
                bool removed = false;
                for (int i = 0; i < _nextPath.Count; i++)
                {
                    PathNode node = _nextPath[i];

                    if (IsSkipAction(node.Action))
                        continue;

                    node.Skip = false;
                    if (!node.IsManditory)
                    {
                        node.IsManditory = true;
                        node.Skip = true;
                        //_nextPath.Remove(node);
                        removed = true;
                        Logger.WriteLine($"Selected to remove: {node}");
                        break;
                    }
                }

                if (!removed)
                {
                    Console.WriteLine("No more deletable nodes");

                    List<PathNode> removableNodes = new List<PathNode>();
                    for (int i = 0; i < _nextPath.Count; i++)
                    {
                        PathNode node = _nextPath[i];
                        if (node.History == null)
                            continue; // Probably just start
                        PathNode skip = new PathNode(ContainsSkipAction(node.History.ActionInfo), node);
                        if (skip.Action != null && !node.Visited)//&& skip.BestDepth >= 0 && (skip.BestDepth < bestActionCount || skip.BestDepth == int.MaxValue))
                        {
                            removableNodes.Add(node);
                        }
                    }

                    if (removableNodes.Count > 0)
                    {
                        int index = source.Rand.Next(removableNodes.Count);
                        PathNode selected = removableNodes[index];
                        selected.Visited = true;
                        Logger.WriteLine($"Selected to not activate: {selected}");

                        index = _nextPath.IndexOf(selected);

                        _nextPath[index] = new PathNode(ContainsSkipAction(selected.History.ActionInfo), selected);
                        _nextPath[index].Visited = true;

                        var a = CountActions(_nextPath);
                    }
                    else
                    {
                        Console.WriteLine("No more removable nodes");
                        Records = BestRecord;
                        base.OnWin(0);
                        SQLComm.GamesPlayed = SQLComm.TotalGames;
                    }
                }
            }

            // base.onwin stuff
            allSelectActions = SQLComm.GetAllActions().Values.ToList();
            allFieldStateValues = SQLComm.GetAllComparisons();
            Records.Clear();
            OnNewGame();
            if (result == 0)
                source.Rand = new Random(1);
        }

        private Node ActionToNode(ActionInfo action, List<Node> nodes)
        {
            foreach (var node in nodes)
            {
                if (node.Action.Equals(action))
                    return node;
            }

            return null;
        }

        private int CountActions(List<PathNode> path)
        {
            int actionCount = 0;
            foreach (var node in path)
            {
                if (!IsSkipAction(node.Action))
                {
                    actionCount++;
                }
            }
            return actionCount;
        }

        private int CountActions(Node endNode)
        {
            Node node = endNode;
            int actionCount = 0;
            while(node != null)
            {
                if (!IsSkipAction(node.Action))
                {
                    actionCount++;
                }
                node = node.Parent;
            }
            return actionCount;
        }

        private ActionInfo ContainsSkipAction(List<ActionInfo> actions)
        {
            if (actions.Count == 0)
                return null;
            ActionInfo r = null;
            foreach(ActionInfo action in actions)
            {
                if (IsSkipAction(action) && (r == null || action.Action != ExecutorType.GoToEndPhase.ToString()))
                    r = action;
            }
            return r;
        }

        private Node FindSkipAction(List<Node> nodes)
        {
            if (nodes.Count == 0)
                return null;
            Node n = null;
            foreach (Node node in nodes)
            {
                if (IsSkipAction(node) && (n == null || node.Action.Action != ExecutorType.GoToEndPhase.ToString()))
                    n = node;
            }
            return n;
        }

        private bool IsSkipAction(ActionInfo action)
        {
            return (action.Name == DONT_PERFORM_STR ||
                    action.Action == ExecutorType.GoToBattlePhase.ToString() ||
                    action.Action == ExecutorType.GoToMainPhase2.ToString() ||
                    action.Action == ExecutorType.GoToEndPhase.ToString());
        }

        private bool IsSkipAction(Node node)
        {
            return (node.Action.Name == DONT_PERFORM_STR ||
                    node.Action.Action == ExecutorType.GoToBattlePhase.ToString() ||
                    node.Action.Action == ExecutorType.GoToMainPhase2.ToString() ||
                    node.Action.Action == ExecutorType.GoToEndPhase.ToString());
        }

        internal override ActionInfo GetBestAction(History history)
        {
            List<ActionInfo> actions = history.ActionInfo;
            List<FieldStateValues> comparisons = history.FieldState;
            var stopwatch = Stopwatch.StartNew();

            foreach (var action in actions)
            {
                AddPossibleAction(action, history);
            }

            ActionInfo next = GetNextAction(comparisons, history).Action;

            //Logger.DebugWriteLine("PathEngine - GetBestAction Time:" + stopwatch.Elapsed, ConsoleColor.Green);
            return next;
        }

        private static int CompareNodeBestDepth(Node a, Node b)
        {
            int a_depth = a.BestDepth;
            int b_depth = b.BestDepth;

            if (a_depth < 0)
                a_depth = int.MaxValue;
            if (b_depth < 0)
                b_depth = int.MaxValue;

            if (a_depth == b_depth)
                return 0;
            if (a_depth < b_depth)
                return -1;
            return 1;
        }
    }
}

