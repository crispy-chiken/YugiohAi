using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using WindBot.AIEngines.Util;
using WindBot.Game;
using WindBot.Game.AI;
using WindBot.Game.AI.Decks;
using WindBot.Game.AI.Decks.Util;
using YGOSharp.OCGWrapper.Enums;

namespace WindBot
{
    public class SearchEngine : AbstractAIEngine
    {
        public class Node
        {
            public List<long> CompareIds { get; set; } = new List<long>();
            public List<long> ActionIds { get; set; } = new List<long>();
            public List<ActionResult> Children { get; set; } = new List<ActionResult>();
            public long NodeId { get; set; } = -4;
            public int Visited { get; set; } = 0;

            // The action to get to this node
            public ActionInfo Action { get; set; }
            public History History { get; set; } = null;

            public float Heuristic { get; set; } = -1;
            public float BestHeuristic { get; set; } = -1;

            // Estimate is not saved
            public float EstimateHeuristic { get; set; } = -1;
            public float EstimateCompleted { get; set; } = 0;

            public bool IsEnd { get; set; } = false;

            public Node Parent { get; set; } = null;

            public Node(Node parent, ActionInfo action, History history = null)
            {
                Action = action;
                History = history;
                Parent = parent;

                if (parent != null)
                {
                    parent.AddChild(this);
                }
                if (history != null)
                {
                    foreach(var a in history.ActionInfo)
                    {
                        ActionIds.Add(a.ActionId);
                    }
                    foreach (var f in history.FieldState)
                    {
                        CompareIds.Add(f.Id);
                    }
                }
            }

            public void AddChild(Node child)
            {
                var existing = Children.Where(x => x.Action.Equals(child.Action)).FirstOrDefault();

                if (existing == null && Children.Count > 0)
                {

                }

                if (existing != null)
                {
                    existing.NextNode = child;
                }
                else
                {
                    Children.Add(new ActionResult(child.Action, child));
                }
            }

            public override string ToString()
            {
                string s = "";
                s +=  "{" + NodeId.ToString() + "} ";
                s += Heuristic.ToString() + " ";
                s += " (" + BestHeuristic.ToString() + ") ";
                if (EstimateHeuristic > 0)
                {
                    s += $" est({EstimateHeuristic:0.00}) {EstimateCompleted:0.00}% ";
                }
                s += " weight: " + GetWeight().ToString("0.00") + " ";
                s += $"({Visited})";
                s += IsEnd + " | ";
                s += Action.ToString();

                return s;
            }

            public int Diff(Node other)
            {
                CompareIds = CompareIds.Distinct().ToList();
                other.CompareIds = other.CompareIds.Distinct().ToList();
                int total = Math.Max(CompareIds.Count, other.CompareIds.Count);// + Math.Max(ActionIds.Count, other.ActionIds.Count);
                int compareDiff = CompareIds.Intersect(other.CompareIds).ToList().Count;
                int actionDiff = 0; //CompareIds.Intersect(other.CompareIds).ToList().Count;
                total = Math.Max(total, 1);

                return total - compareDiff;
            }

            public double GetWeight()
            {
                float c = 1;
                double visited = Math.Max(0.001, Visited);
                double res = Math.Max(1, BestHeuristic) / visited;
                if (Parent != null)
                    res += c * Math.Sqrt((Math.Log(Parent.Visited + 1) + 1) / visited);

                return res;
            }
        }

        public class ActionResult
        {
            public ActionInfo Action;
            public Node NextNode;

            public ActionResult(ActionInfo info, Node node)
            {
                Action = info;
                NextNode = node;
            }
        }

        public List<Node> Path { get; set; }

        List<Node> _nodeMappings { get; set; }
        Node _current { get; set; }

        // Actions to calculate for next heuristic calculation
        List<Node> _groupedActions { get; set; }
        public List<Node> possibleActions { get; set; }

        protected float threshold = 0.0f;

        Stopwatch _stopwatch = null;

        public SearchEngine(AIBase source) : base(source)
        {
            Path = new List<Node>();
            OnNewGame();
        }

        public void OnNewGame()
        {
            var stopwatch = Stopwatch.StartNew();
            Path.Clear();
            possibleActions = new List<Node>();
            _groupedActions = new List<Node>();
            _nodeMappings = SQLComm.GetAllSearchNodes();

            _current = GetNode(null, new ActionInfo("Start", "", 0), new History(null, new List<ActionInfo>(), new List<FieldStateValues>()));
            Path.Add(_current);

            Logger.DebugWriteLine("OnNewGame - Elaspsed Time:" + stopwatch.Elapsed, ConsoleColor.Green);
            _stopwatch = Stopwatch.StartNew();
        }

        public override void OnNewTurn(Duel duel)
        {
 
            if (duel.Turn == 1)
            {
                foreach(var card in duel.Fields[0].Hand)
                {
                    // Set up start turn? Maybe that is for name
                }
            }
            else
            {
                // Always surrender on the next turn
                _current.Heuristic = Heuristics.GetHeuristics(source);
                _current.BestHeuristic = Math.Max(_current.BestHeuristic, _current.Heuristic);
                SQLComm.ShouldSurrender = true;
            }

            base.OnNewTurn(duel);
        }

        public override void OnChainSolving()
        {
            base.OnChainSolving();
        }

        public override void OnChainSolved()
        {
            base.OnChainSolved();
        }

        public override void OnChainEnd()
        {
            base.OnChainEnd();
            UpdateHeurisitcs();
        }

        public override void SetMain(MainPhase main, List<FieldStateValues> fieldState, Duel duel)
        {
            base.SetMain(main, fieldState, duel);

            //UpdateHeurisitcs();
        }


        /**
            * For Multiple Actions
            */

        private void AddPossibleAction(ActionInfo action, History history)
        {
            if (action.Action == ExecutorType.MonsterSet.ToString())
            {
                return;
            }
            if (action.Action == ExecutorType.GoToEndPhase.ToString())
            {
                return;
            }
            if (action.Name == DONT_PERFORM_STR)
            {
                // Can't have this, some effects like oak need this
                //return;
            }
            Node node = GetNode(_current, action, history);
            node.History = history;
            possibleActions.Add(node);
        }


        /**
         * Called after setting all possible actions
         */
        private Node GetNextAction(List<FieldStateValues> comparisons, bool pop = false)
        {
            // No new actions were added
            if (possibleActions.Count == 0)
            {
                _current.Children.Clear();
                return null;
            }

            //List<Node> similar = _nodeMappings.Where(x => x.NodeId != -4 && x.Diff(_current) <= 2).OrderBy(x => x.Diff(_current)).ToList();
            //List<Node> similar = _nodeMappings
            //                        .Where(x => x.Children.Any(y => possibleActions.Any(z => y.Action.Equals(z.Action))) && x.Diff(_current) <= 3)
            //                        .OrderBy(x => x.Diff(_current)).ToList();

            List<Node> close = new List<Node>(); // Nodes whos heurisitics are close to each other
            double bestWeight = 0;
 
            foreach (Node n in possibleActions)
            {
                if (n.IsEnd && SQLComm.IsTraining)
                    continue; // This path has been fully explored

                List<Node> similar = _nodeMappings
                                    .Where(x => x.Action.Equals(n.Action))// && x.Diff(n) <= 10)
                                    .OrderBy(x => x.Diff(n)).ToList();

                List<Node> similarResult = new List<Node>();
                foreach(var res in similar)
                {
                    if (!res.Action.Equals(n.Action))
                        continue;

                    similarResult.Add(res);
                    n.EstimateHeuristic += res.Heuristic;
                    if (res.IsEnd)
                        n.EstimateCompleted += 1;
                }

                if (similarResult.Count > 1)
                {
                    n.EstimateHeuristic /= similarResult.Count;
                    n.EstimateCompleted /= similarResult.Count;
                }

                if (n.Heuristic == -1)
                {

                }


                // Skip action who makes youur current position worse
                //if (_current.Heuristic - threshold > n.Heuristic && n.Heuristic != -1 && _current.Heuristic != -1)
                //    continue;

                var weight = n.GetWeight();

                if (ActivationUtil.ShouldNotPerform(n.Action, source))
                    weight -= 100;
                else if (ActivationUtil.ShouldPerform(n.Action, source))
                    weight += 100;

                if (!SQLComm.IsTraining)
                    weight = n.BestHeuristic;

                if (weight > bestWeight)
                {
                    close.Clear();
                    close.Add(n);
                    bestWeight = Math.Max(bestWeight, weight);
                }
                else if (weight > bestWeight - threshold)
                {
                    close.Add(n);
                }
            }

            // Finished all searches
            if (close.Count == 0)
            {
                _current.Children.Clear();
                SQLComm.ShouldSurrender = true;
                if (Path.Count <= 1)
                    SQLComm.IsDone = true;
                return possibleActions[0];
            }


            var possible = possibleActions;
            Node best = getRandomWeightedNode(close);
            //Node best = close[source.Rand.Next(close.Count)];

            if (best == _current)
            {
                // For some reason, don't perform gets repeated twice?
                //throw new Exception("Best should not be current");
            }

            if (best != null)
            {
                // _current.Children.Add(best);
                _current = best;
            }

            if (pop)
            {
                possibleActions.Remove(best);
            }
            else
            {
                possibleActions.Clear();
            }

            Path.Add(best);
            _groupedActions.Add(best);

            return best;
        }


        public override void OnWin(int result)
        {
            if (_stopwatch != null)
            {
                Logger.DebugWriteLine("Duel - Elaspsed Time:" + _stopwatch.Elapsed, ConsoleColor.Green);
            }

            var stopwatch = Stopwatch.StartNew();
            // Always tie
            base.OnWin(2);

            List<Node> visited = new List<Node>();
            Queue<Node> queue = new Queue<Node>();
            queue.Enqueue(_current);

            // Update the best heuristic for path
            while(queue.Count > 0)
            {
                var cur = queue.Dequeue();

                if (visited.Contains(cur))
                    continue;

                visited.Add(cur);

                // Mark all children who makes your current position worse as finished
                foreach(ActionResult a in cur.Children)
                {
                    if (a.NextNode == null)
                        continue;

                    //if (a.NextNode.Heuristic < cur.Heuristic - threshold && a.NextNode.Heuristic != -1)
                    //    a.NextNode.IsEnd = true;
                }

                if (cur.Children.Count == 0)
                {
                    cur.IsEnd = true;
                }
                else if (cur.Children.All(x => x.NextNode != null && x.NextNode.IsEnd))
                    cur.IsEnd = true;

                cur.BestHeuristic = cur.Heuristic;

                foreach(var child in cur.Children)
                {
                    if (child.NextNode == null)
                        continue;
                    cur.BestHeuristic = Math.Max(cur.BestHeuristic, child.NextNode.BestHeuristic);
                }

                foreach(var actionResult in cur.Children)
                {
                    if (actionResult.NextNode == null)
                        continue;
                    if (!queue.Contains(actionResult.NextNode))
                    {
                        queue.Enqueue(actionResult.NextNode);
                    }
                }

                if (cur.Parent != null)
                    queue.Enqueue(cur.Parent);
            }

            //SQLComm.InsertSearchNodes(Tree);
            SQLComm.UpdateSearchNodes(Path);

            Logger.DebugWriteLine("OnWin - Elaspsed Time:" + stopwatch.Elapsed, ConsoleColor.Green);

            OnNewGame();
        }

        internal override ActionInfo GetBestAction(History history)
        {
           // NewIntermediateNode($"{source.Duel.Turn};{ActionNumber}");
            List<ActionInfo> actions = history.ActionInfo;
            List<FieldStateValues> comparisons = history.FieldState;
            var stopwatch = Stopwatch.StartNew();
           /* Console.WriteLine("Current State:---------");
            comparisons.Reverse();
            foreach (var i in comparisons)
            {
                Console.WriteLine("     " + i.ToString());
            }*/


            foreach (var action in actions)
            {
                AddPossibleAction(action, history);
            }

            ActionInfo next = GetNextAction(comparisons)?.Action;

            //Logger.DebugWriteLine("SearchEngine - GetBestAction Time:" + stopwatch.Elapsed, ConsoleColor.Green);
            return next;
        }

        private void UpdateHeurisitcs()
        {
            foreach (Node n in _groupedActions)
            {
                if (n == null)
                    continue;

                float h = Heuristics.GetHeuristics(source);
                // Update current heuristics
                if (n.Heuristic != -1 && n.Heuristic != h)
                {
                    // Heuristics are wrong?
                }
                n.Heuristic = h;
                n.BestHeuristic = Math.Max(n.BestHeuristic, n.Heuristic);
            }

            _groupedActions.Clear();
        }

        private Node GetNode(Node parent, ActionInfo action, History history)
        {
            Node node = null;
            long parentId = parent?.NodeId ?? -4;


            Node toFind = new Node(null, null, history);
            foreach (var n in _nodeMappings)
            {
                if (n.NodeId == 8422)
                {

                }

                if (n.NodeId == -4)
                    continue;

                if (n.Diff(toFind) > 0 && action.ActionId != 1)
                    continue;

                if (n.Action.ActionId != action.ActionId)
                    continue;

                node = n;
                break;
            }

            if (node == null)
            {
                if (parent != null && parent.Children.Count > 0 && parent.Children.Any(x => x.Action.Equals(action) & x.NextNode != null))
                {

                }

                node = new Node(parent, action);
                _nodeMappings.Add(node);
            }
            else if (parent != null)
            {
                node.Action = action;

                if (node.Parent != parent)
                {
                    // Could be multiple routes going into 1
                }

                if (!parent.Children.Any(x => x.Action.Equals(action)))
                {
                    parent.AddChild(node);
                }

                node.Parent = parent;
                //
            }


            return node;
        }

        private Node getRandomWeightedNode(List<Node> nodes)
        {
            float totalWeight = nodes.Sum(x => x.BestHeuristic);
            float randomNumber = Math.Abs((float)source.Rand.NextDouble() * totalWeight);

            Node selected = null;
            foreach (Node n in nodes)
            {
                if (randomNumber <= n.BestHeuristic || n.BestHeuristic < 0)
                {
                    selected = n;
                    break;
                }

                randomNumber -= n.BestHeuristic;
            }

            return selected;
        }
    }
}

