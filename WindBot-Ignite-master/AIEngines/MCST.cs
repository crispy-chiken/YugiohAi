using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WindBot.Game;
using WindBot.Game.AI;
using WindBot.Game.AI.Decks.Util;
using static WindBot.AbstractAIEngine;
using static WindBot.NeuralNet;

namespace WindBot
{
    public class MCST
    {
        public class Node
        {
            public List<Node> Children;
            public Node Parent;
            public double AvgTurn = 0;
            public double Rewards = 0;
            public int Visited = 0;
            public string CardId;
            public string Action;
            public long NodeId = -4;
            public long Desc = -1; // for activated card description
            public MLUtil.GameState StateCurrent = null;
            public MLUtil.GameState StateAfterPlay = null;
            public MLUtil.GameState StateAfterTurn = null;
            public MLUtil.GameState StateAfterOtherTurn = null;
            public bool SaveParent = true;
            public bool SaveChild = true;
            public ClientCard Card = null;

            public Node(Node parent, string cardId, ExecutorType action, ClientField[] field)
                : this(parent, cardId, action)
            {
                StateCurrent = new MLUtil.GameState(field);
            }

            public Node(Node parent, Node child, string cardId, ExecutorType action, ClientField[] field)
                :this(parent, child, cardId, action)
            {
                StateCurrent = new MLUtil.GameState(field);
            }

            public Node(Node parent, string cardId, ExecutorType action)
                :this(parent, null, cardId, action)
            {
            }

            public Node(Node parent, string cardId, ExecutorType action, ClientCard card)
                : this(parent, null, cardId, action)
            {
                Card = card;
            }

            public Node(Node parent, string cardId, ExecutorType action, long desc)
                : this(parent, null, cardId, action)
            {
                Desc = desc;
            }


            public Node(Node parent, Node child, string cardId, ExecutorType action)
            {
                Children = new List<Node>();
                Parent = parent;
                CardId = cardId;
                Action = action.ToString();
                StateCurrent = new MLUtil.GameState();

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

            public double Heuristic()
            {
                double value = 0;
                if (StateCurrent != null)
                {
                    if (StateAfterTurn != null)
                    {
                        double cardAdvantageHand = StateAfterTurn.BotField.HandCount - StateCurrent.BotField.HandCount + StateCurrent.EnemyField.HandCount - StateAfterTurn.EnemyField.HandCount;
                        double cardAdvantageField = StateAfterTurn.BotField.FieldCount - StateCurrent.BotField.FieldCount + StateCurrent.EnemyField.FieldCount - StateAfterTurn.EnemyField.FieldCount;

                        double fieldAdvantage = StateAfterTurn.BotField.FieldCount - StateAfterTurn.EnemyField.FieldCount;
                        double enemyFieldLoss = StateAfterTurn.EnemyField.FieldCount - StateCurrent.EnemyField.FieldCount;
                        double playerFieldGain = StateAfterTurn.BotField.FieldCount - StateCurrent.BotField.FieldCount;
                        double advantageGain = cardAdvantageHand + cardAdvantageField;
                        value += Math.Sign(fieldAdvantage);
                        value += Math.Sign(enemyFieldLoss);
                        //value += Math.Sign(playerFieldGain);
                        value += Math.Sign(advantageGain);

                        /*value += StateAfterTurn.BotField.HandCount - StateCurrent.BotField.HandCount;
                        value += StateAfterTurn.BotField.FieldCount - StateCurrent.BotField.FieldCount;
                        value -= StateAfterTurn.EnemyField.FieldCount - StateCurrent.EnemyField.FieldCount;*/

                    }

                    if (StateAfterOtherTurn != null)
                    {
                        double cardAdvantageHand = StateAfterOtherTurn.BotField.HandCount - StateCurrent.BotField.HandCount + 1 + StateCurrent.EnemyField.HandCount - StateAfterOtherTurn.EnemyField.HandCount;
                        double cardAdvantageField = StateAfterOtherTurn.BotField.FieldCount - StateCurrent.BotField.FieldCount + StateCurrent.EnemyField.FieldCount - StateAfterOtherTurn.EnemyField.FieldCount;

                        double advantageGain = cardAdvantageHand + cardAdvantageField;
                        double cardAdvantage = StateAfterTurn.BotField.FieldCount - StateAfterTurn.EnemyField.FieldCount + StateAfterTurn.BotField.HandCount - StateAfterTurn.EnemyField.HandCount;

                        value += StateAfterOtherTurn.BotField.HandCount - StateAfterOtherTurn.EnemyField.HandCount;
                        value += StateAfterOtherTurn.BotField.FieldCount - StateAfterOtherTurn.EnemyField.FieldCount;
                    }
                }

                //value /= Math.Max(Visited,1);

                return Math.Sign(value);
            }

            public override string ToString()
            {
                string s = "";
                s += NodeId.ToString() + " | ";
                s += CardId + ": " + Action + " | ";
                if (Desc >= 0)
                    s += Desc.ToString() + " | ";
                s += Heuristic();

                return s;
            }
        }

        public List<Node> Path;
        public int PathIndex { get; private set; } = 1;

        Node _current;
        Node _lastNode = null;
        public List<Node> possibleActions;
        int ActionCount = 0;
        int BestActionCount = int.MaxValue;
        public List<History> BestRecord = new List<History>();
        float AvgCount = 0;

        public MCST()
        {
            Path = new List<Node>();
            OnNewGame();
        }

        public void OnNewGame()
        {
            possibleActions = new List<Node>();
            _current = new Node(null, "", default(ExecutorType));
            _lastNode = _current;
            if (Path.Count == 0)
                Path.Add(_current);
            PathIndex = 1;
            ActionCount = 0;
        }

        public void OnGameEnd(int result, Duel duel, List<History> records)
        {
            bool reset = SQLComm.ShouldBackPropagate;
            double reward = result == 0 ? 1 : 0;
            //double reward = result == 0 ? 1.0 - Math.Min(duel.Turn * 0.01, 0.1) - Math.Min(ActionCount * 0.001, 0.2): 0;
           // SQLComm.Backpropagate(Path, _lastNode, reward, ActionCount);// duel.Turn);

            if (reset)
            {
                Path.Clear();
            }

            if (ActionCount < BestActionCount && result == 0)
            {
                BestActionCount = ActionCount;
                BestRecord = records;
            }
        }


        /*
         * For Multiple Actions
         */

        public void AddPossibleAction(string cardId, ExecutorType action, ClientCard card = null)
        {
            Node node = new Node(_current, cardId, action, card);
            possibleActions.Add(node);
        }

        public void AddPossibleAction(string cardId, ExecutorType action, long desc)
        {
            Node node = new Node(_current, cardId, action, desc);
            possibleActions.Add(node);
        }

        public bool ShouldActivate(string cardId, ExecutorType action, List<FieldStateValues> comparisons)
        {
            Node toActivate = new Node(_current, cardId, action);
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
                foreach (Node n in possibleActions)
                {
                    double visited = Math.Max(0.01, n.Visited);
                    //double estimate = SQLComm.GetNodeEstimate(n);
                    //double w = n.Rewards/visited + c * Math.Sqrt((Math.Log(n.Parent.Visited + 1) + 1) / visited);
                    double w = 0.1 * n.Rewards / visited + (n.Parent.Visited) / visited;//c * Math.Sqrt(Math.Log(n.Parent.Visited + 1) / visited);
                    if (n.AvgTurn > 0 && n.AvgTurn != 9999)
                        w += AvgCount / n.AvgTurn;
                    else
                        w += 1; // Default just increase the weight by 1
                    //w += estimate;
                    if (CSVReader.InBaseActions(n.CardId, n.Action, comparisons) && visited < 10)
                        w += 1;

                    if (w >= weight)
                    {
                        weight = w;
                        best = n;
                    }
                }

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

                foreach(var action in possibleActions)
                {
                    if (CSVReader.InBaseActions(action.CardId, action.Action, comparisons))
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
    }
}

