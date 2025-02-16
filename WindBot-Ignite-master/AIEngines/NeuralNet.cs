using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WindBot.Game;
using WindBot.Game.AI;
using WindBot.Game.AI.Decks.Util;
using YGOSharp.OCGWrapper;
using YGOSharp.OCGWrapper.Enums;

namespace WindBot
{
    public class NeuralNet : AbstractAIEngine
    {
        private List<History> Records = new List<History>();
        private List<History> CurrentTurn = new List<History>();

        private int ActionNumber = 0;
        private ActionInfo BestAction = null;

        public NeuralNet(Executor source) :
            base(source)
        {

        }

        private void AddHistory(History history, Duel duel)
        {
            if (duel.Turn > 0)
            {
                history.CurP1Field = duel.Fields[0].GetFieldCount();
                history.CurP1Hand = duel.Fields[0].GetHandCount();
                history.CurP2Field = duel.Fields[1].GetFieldCount();
                history.CurP2Hand = duel.Fields[1].GetHandCount();
            }

            Records.Add(history);
            CurrentTurn.Add(history);
        }

        public override ClientCard OnSelectAttacker(IList<ClientCard> attackers, List<FieldStateValues> fieldState, Duel duel)
        {
            // AI Selection

            ActionNumber++;
            List<ActionInfo> actions = new List<ActionInfo>();
            GameInfo gameInfo = new GameInfo(SQLComm.Id, duel.Turn, ActionNumber);

            //var dontPerform = new ActionInfo("DontPerform", "", 0.45);
            //actions.Add(dontPerform);

            foreach (ClientCard attacker in attackers)
            {
                actions.Add(new ActionInfo(BuildActionString(attacker, duel), "SelectAttacker", attacker));
            }

            var choice = GetBestAction(actions, fieldState);
            if (choice != null)
            {
                choice.Performed = true;
            }


            AddHistory(new History(gameInfo, actions, fieldState), duel);

            return choice.Card;
        }

        public override ClientCard OnSelectAttackTarget(ClientCard attacker, IList<ClientCard> defenders, List<FieldStateValues> fieldState, Duel duel)
        {
            // AI Selection

            ActionNumber++;
            List<ActionInfo> actions = new List<ActionInfo>();
            GameInfo gameInfo = new GameInfo(SQLComm.Id, duel.Turn, ActionNumber);

            var dontPerform = new ActionInfo(BuildActionString(attacker, duel) + ";DirectAttack", "SelectAttackTarget", null);
            if (attacker.CanDirectAttack)
                actions.Add(dontPerform);

            foreach (ClientCard defender in defenders)
            {
                actions.Add(new ActionInfo(BuildActionString(attacker, duel) + ";" + BuildActionString(defender, duel), "SelectAttackTarget", defender));
            }

            var choice = GetBestAction(actions, fieldState);

            if (choice != null)
            {
                choice.Performed = true;
            }
            

            AddHistory(new History(gameInfo, actions, fieldState), duel);

            return choice?.Card;
        }

        public override CardPosition OnSelectPosition(int cardId, IList<CardPosition> positions, List<FieldStateValues> fieldState, Duel duel)
        {
            CardPosition cardPosition = positions[0];

            // AI Selection

            ActionNumber++;
            List<ActionInfo> actions = new List<ActionInfo>();
            GameInfo gameInfo = new GameInfo(SQLComm.Id, duel.Turn, ActionNumber);

            foreach (CardPosition position in positions)
            {
                actions.Add(new ActionInfo(cardId.ToString() + ";" + position.ToString(), "SetPosition", null));

            }


            var choice = GetBestAction(actions, fieldState);
            if (choice != null)
            {
                choice.Performed = true;
            }

            cardPosition = positions[actions.FindIndex(x => x.Performed)];

            AddHistory(new History(gameInfo, actions, fieldState), duel);


            return cardPosition;
        }

        // TODO
        public override int OnAnnounceCard(ClientCard card, IList<int> avail, List<FieldStateValues> fieldState, Duel duel)
        {
            int chosen = 0;

            // AI Selection

            ActionNumber++;
            List<ActionInfo> actions = new List<ActionInfo>();
            GameInfo gameInfo = new GameInfo(SQLComm.Id, duel.Turn, ActionNumber);

            foreach (int cardId in avail)
            {
                actions.Add(new ActionInfo(cardId.ToString(), "AnnounceCard", null));
            }


            var choice = GetBestAction(actions, fieldState);
            if (choice != null)
            {
                choice.Performed = true;
                chosen = int.Parse(choice.Name);
            }


            AddHistory(new History(gameInfo, actions, fieldState), duel);


            return chosen;

        }

        public override void OnNewPhase()
        {
            BestAction = null;
        }

        public override void OnNewTurn(Duel duel)
        {
            ActionNumber = 0;


            foreach (var info in CurrentTurn)
            {
                info.PostP1Field = duel.Fields[0].GetFieldCount();
                info.PostP1Hand = duel.Fields[0].GetHandCount();
                info.PostP2Field = duel.Fields[1].GetFieldCount();
                info.PostP2Hand = duel.Fields[1].GetHandCount();
            }

            CurrentTurn.Clear();
        }

        public override void OnChainSolving()
        {
            BestAction = null;
        }

        public override void OnChainSolved()
        {
            BestAction = null;
        }

        public override void OnWin(int result)
        {
            JsonExporter.SavePlayHistory(Records, result);
            //SQLComm.SavePlayHistory(Records, result);
        }

        public override IList<ClientCard> SelectCards(ClientCard currentCard, int min, int max, long hint, bool cancelable, IList<ClientCard> selectable, List<FieldStateValues> fieldState, Duel duel)
        {
            IList<ClientCard> selected = new List<ClientCard>();
            IList<ClientCard> cards = new List<ClientCard>(selectable);

            int toSelect = min;

            // AI Selection
            // Select number of cards to select
            if (min != max)
            {
                ActionNumber++;
                List<ActionInfo> actions = new List<ActionInfo>();
                GameInfo gameInfo = new GameInfo(SQLComm.Id, duel.Turn, ActionNumber);


                for (int i = min; i <= max; i ++)
                {
                    string card = i.ToString() + ";" + BuildActionString(currentCard, duel);
                    actions.Add(new ActionInfo(card, "SelectAmount", currentCard, hint));
                }


                var choice = GetBestAction(actions, fieldState);
                if (choice != null)
                {
                    choice.Performed = true;
                }

                AddHistory(new History(gameInfo, actions, fieldState), duel);

                toSelect = actions.FindIndex(x => x == choice) + min;
            }

            {
                ActionNumber++;
                List<ActionInfo> actions = new List<ActionInfo>();
                GameInfo gameInfo = new GameInfo(SQLComm.Id, duel.Turn, ActionNumber);

                foreach (ClientCard clientCard in cards)
                {
                    string action = $"Select" + hint.ToString();
                    string card = SelectStringBuilder(clientCard);
                    actions.Add(new ActionInfo(card, ExecutorType.Select.ToString(), clientCard, hint));

                }


                var actionCopy = new List<ActionInfo>(actions);

                while (actionCopy.Count > 0 && selected.Count < toSelect)
                {
                    var choice = GetBestAction(actionCopy, fieldState);
                    if (choice != null)
                    {
                        choice.Performed = true;
                        selected.Add(choice.Card);
                        actionCopy.Remove(choice);
                    }
                }

                AddHistory(new History(gameInfo, actions, fieldState), duel);
            }

            return selected;
        }

        public override int SelectOption(IList<long> options, List<FieldStateValues> fieldState, Duel duel, AIUtil Util)
        {
            List<ActionInfo> actions = new List<ActionInfo>();
            ActionNumber++;
            GameInfo gameInfo = new GameInfo(SQLComm.Id, duel.Turn, ActionNumber);

            if (options.Count == 2)
            {
                var cardId = Util.GetCardIdFromDesc(options[0]);
                string name = NamedCardsManager.GetCard((int)cardId)?.Name ?? null;

                if (name != null)
                {
                    actions.Add(new ActionInfo(name + ":" + 0 + ":" + duel.Phase.ToString(), "SelectOption", null, options[1]));
                    actions.Add(new ActionInfo(name + ":" + 1 + ":" + duel.Phase.ToString(), "SelectOption", null, options[1]));
                }
                else
                {
                    var hint = options[0];
                    cardId = Util.GetCardIdFromDesc(options[1]);
                    name = NamedCardsManager.GetCard((int)cardId)?.Name ?? null;

                    if (name != null)
                    {
                        actions.Add(new ActionInfo(name + ":" + 0 + ":" + duel.Phase.ToString(), "SelectOption", null, hint));
                        actions.Add(new ActionInfo(name + ":" + 1 + ":" + duel.Phase.ToString(), "SelectOption", null, hint));
                    }
                }

                Console.WriteLine("Actions for Turn " + duel.Turn + " Action # " + ActionNumber);
                BestAction = GetBestAction(actions, fieldState);
                if (BestAction != null)
                    BestAction.Performed = true;

                AddHistory(new History(gameInfo, actions, fieldState), duel);

                return actions.IndexOf(BestAction);
            }
            else
            {
                foreach (long desc in options)
                {
                    var option = Util.GetOptionFromDesc(desc);
                    var cardId = Util.GetCardIdFromDesc(desc);
                    string name = NamedCardsManager.GetCard((int)cardId)?.Name ?? null;
                    if (name == null)
                        name = cardId.ToString();
                    actions.Add(new ActionInfo(name + ":" + option + ":" + duel.Phase.ToString(), "SelectOption", null, desc));
                }

                Console.WriteLine("Actions for Turn " + duel.Turn + " Action # " + ActionNumber);
                BestAction = GetBestAction(actions, fieldState);
                if (BestAction != null)
                    BestAction.Performed = true;

                AddHistory(new History(gameInfo, actions, fieldState), duel);

                return options.IndexOf(BestAction.Desc);
            }
        }

        public override void SetBattle(BattlePhase battle, List<FieldStateValues> fieldState, Duel duel)
        {
            if (battle.ActivableCards.Count == 0)
                return;

            ActionNumber++;

            GameInfo gameInfo = new GameInfo(SQLComm.Id, duel.Turn, ActionNumber);
            List<ActionInfo> actions = new List<ActionInfo>();
            var dontPerform = new ActionInfo("ToAttack", "", 0.45);

            actions.Add(dontPerform);

            //loop through activatable cards
            for (int i = 0; i < battle.ActivableCards.Count; ++i)
            {
                ClientCard card = battle.ActivableCards[i];
                actions.Add(new ActionInfo(BuildActionString(card, duel) + ";" + card.Attacked.ToString(), ExecutorType.Activate.ToString(), card, battle.ActivableDescs[i]));
            }

            if (battle.CanMainPhaseTwo)
            {
                actions.Add(new ActionInfo("", ExecutorType.GoToMainPhase2.ToString(), null));
            }

            Console.WriteLine("Actions for Turn " + duel.Turn + " Action # " + ActionNumber);
            BestAction = GetBestAction(actions, fieldState);
            if (BestAction != null)
                BestAction.Performed = true;
            //if (BestAction == dontPerform)
            //    BestAction = null;

            AddHistory(new History(gameInfo, actions, fieldState), duel);
        }

        public override void SetChain(IList<ClientCard> cards, IList<long> descs, bool forced, List<FieldStateValues> fieldState, Duel duel, AIUtil Util)
        {
            if (cards.Count() == 0)
                return;

            ActionNumber++;
            GameInfo gameInfo = new GameInfo(SQLComm.Id, duel.Turn, ActionNumber);
            List<ActionInfo> actions = new List<ActionInfo>();
            var dontPerform = new ActionInfo("DontPerform", "", 0.45);

            if (!forced)
                actions.Add(dontPerform);
            for (int i = 0; i < cards.Count; ++i)
            {
                ClientCard card = cards[i];
                var option = Util.GetOptionFromDesc(descs[i]);
                var cardId = Util.GetCardIdFromDesc(descs[i]);
                actions.Add(new ActionInfo(BuildActionString(card, duel), ExecutorType.Activate.ToString(), card, descs[i]));
            }



            Console.WriteLine("Actions for Turn " + duel.Turn + " Action # " + ActionNumber);
            BestAction = GetBestAction(actions, fieldState);
            if (BestAction != null)
                BestAction.Performed = true;
            //if (BestAction == dontPerform)
            //    BestAction = null;

            AddHistory(new History(gameInfo, actions, fieldState), duel);

        }

        public override void SetMain(MainPhase main, List<FieldStateValues> fieldState, Duel duel)
        {
            List<ActionInfo> actions = new List<ActionInfo>();

            if (duel.Phase == DuelPhase.Main2)
            {
                //return;
            }

            ActionNumber++;
            GameInfo gameInfo = new GameInfo(SQLComm.Id, duel.Turn, ActionNumber);

            foreach (ClientCard card in main.MonsterSetableCards)
            {
                actions.Add(new ActionInfo(BuildActionString(card, duel), ExecutorType.MonsterSet.ToString(), card));
            }
            //loop through cards that can change position
            foreach (ClientCard card in main.ReposableCards)
            {
                actions.Add(new ActionInfo(BuildActionString(card, duel) + ";" + card.Position.ToString(), ExecutorType.Repos.ToString(), card));
            }
            //Loop through normal summonable monsters
            foreach (ClientCard card in main.SummonableCards)
            {
                actions.Add(new ActionInfo(BuildActionString(card, duel), ExecutorType.Summon.ToString(), card));
            }
            //loop through special summonable monsters
            foreach (ClientCard card in main.SpecialSummonableCards)
            {
                actions.Add(new ActionInfo(BuildActionString(card, duel), ExecutorType.SpSummon.ToString(), card));
            }
            //loop through activatable cards
            for (int i = 0; i < main.ActivableCards.Count; ++i)
            {
                ClientCard card = main.ActivableCards[i];
                actions.Add(new ActionInfo(BuildActionString(card, duel), ExecutorType.Activate.ToString(), card, main.ActivableDescs[i]));
            }
            //loop through setable cards
            for (int i = 0; i < main.SpellSetableCards.Count; ++i)
            {
                ClientCard card = main.SpellSetableCards[i];
                actions.Add(new ActionInfo(BuildActionString(card, duel), ExecutorType.SpellSet.ToString(), card));
            }


            if (main.CanBattlePhase)
            {
                actions.Add(new ActionInfo("", ExecutorType.GoToBattlePhase.ToString(), null));
            }
            if (main.CanEndPhase)
            {
                actions.Add(new ActionInfo("", ExecutorType.GoToEndPhase.ToString(), null));
            }

            Console.WriteLine("Actions for Turn " + duel.Turn + " Action # " + ActionNumber);
            BestAction = GetBestAction(actions, fieldState);
            if (BestAction != null)
                BestAction.Performed = true;

            AddHistory(new History(gameInfo, actions, fieldState), duel);
        }

        public override bool ShouldPerform(ClientCard card, string action, long desc, List<FieldStateValues> fieldState, Duel duel)
        {
            if (BestAction != null)
            {
                string id = BestAction.Name?.Split(';')[0];
                if (desc < 0)
                    BestAction.Desc = desc;
                if (id == null)
                    id = "";
                if (card == null)
                    id = null;
                //ActivateDescription
                if (card?.Name == id && BestAction.Action == action && BestAction.Desc == desc)
                {
                    BestAction = null;
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                ActionNumber++;
                GameInfo info = new GameInfo(SQLComm.Id, duel.Turn, ActionNumber);
                string cardName = BuildActionString(card, duel);
                ActionInfo actionInfo = new ActionInfo(BuildActionString(card, duel), action, card, desc);
                List<ActionInfo> actions = new List<ActionInfo>
                {
                    new ActionInfo("DontPerform","", 0.45),
                    actionInfo

                };

                Console.WriteLine("Single Actions for Turn " + duel.Turn + " Action # " + ActionNumber);
                ActionInfo best = GetBestAction(actions, fieldState);
                if (best == null || best != actionInfo)
                {
                    // return false;
                }
                else
                {
                    best.Performed = true;
                }

                AddHistory(new History(info, actions, fieldState), duel);

                return actionInfo.Performed;
            }
        }


        private ActionInfo GetBestAction(List<ActionInfo> actions, List<FieldStateValues> comparisons)
        {
            if (actions.Count == 0)
                return null;

            actions = GetActionWeights(actions, comparisons);
            Console.WriteLine("Record length : {0}---------", Records.Count);
            Console.WriteLine("Current State:---------");
            comparisons.Reverse();
            foreach (var i in comparisons)
            {
                Console.WriteLine("     " + i.ToString());
            }

            Console.WriteLine("Weights:---------");
            foreach (var action in actions)
            {
                Console.WriteLine(Math.Round(action.Weight, 3).ToString() + ":" + action.ToString());
            }

            var results = actions.OrderByDescending(x => x.Weight).ToList();
            // Take the top percentile
            var max_guess = results.Max(x => x.Weight);
            results = results.Where(x => x.Weight >= max_guess - 0.02).ToList();
            var best = results[Program.Rand.Next(results.Count)];
            //FIXED RNG
            //if (!SQLComm.IsTraining)
            //best = results[0];



            if (!SQLComm.IsManual)
            {
                // If only one option, only choose if greater than .5
                /*if (actions.Count == 1)
                {
                    actions[0].Performed = actions[0].Weight >= 0.5;
                    Console.WriteLine("Chose " + ":" + (actions[0].Performed ? "Yes" : "No"));
                    return actions[0];
                }*/
                Console.WriteLine("------- BEST ------");
                Console.WriteLine(best.ToString());

                Console.WriteLine("Chose " + ":" + best.ToString());
                return best;
            }


            if (SQLComm.IsManual)
            {
                Console.WriteLine("");
                Console.WriteLine("Actions:---------");

                for (int i = 0; i < actions.Count; i++)
                {
                    Console.WriteLine(" " + i + ":" + actions[i].ToString());
                }

                Console.WriteLine("");

                Console.WriteLine("------- BEST ------");
                Console.WriteLine(best.ToString());


                int choice = -1;


                // If there is only one option, choose it
                if (actions.Count == 1)
                    choice = 0;

                while (choice == -1)
                {
                    int result;
                    if (int.TryParse(Console.ReadLine(), out result))
                    {
                        if (result >= 0 && result < actions.Count)
                        {
                            choice = result;
                        }
                    }
                    else
                    {
                        Console.WriteLine("Chosing AI Best");
                        return best;
                    }
                }
                Console.WriteLine("Chose " + choice + ":" + actions[choice].ToString());

                return actions[choice];
            }

            // Else automatic training
            return null;
        }

        private List<ActionInfo> GetActionWeights(List<ActionInfo> actions, List<FieldStateValues> comparisons)
        {
            if (actions.Count == 0)
                return actions;

            List<long> input = new List<long>();
            List<long> data = new List<long>();

            foreach (var i in actions)
            {
                input.Add(i.ActionId);
            }
            foreach (var i in comparisons)
            {
                data.Add(i.Id);
            }
            List<double> best_predict = new List<double>(HttpComm.GetBestActionAsync(input, data, SQLComm.Name).Result);

            foreach (ActionInfo action in actions)
            {
                // Skip actions with assigned weight already
                if (action.Weight >= 0)
                {
                    if ((int)action.ActionId < best_predict.Count)
                        Console.WriteLine(best_predict[(int)action.ActionId] + "*:" + action.ToString());
                    continue;
                }

                if ((int)action.ActionId < best_predict.Count)
                {
                    action.Weight = best_predict[(int)action.ActionId];
                }
                Console.WriteLine(action.Weight + ":" + action.ToString());

                if ((action.Action == ExecutorType.GoToBattlePhase.ToString() || action.Action == ExecutorType.GoToEndPhase.ToString()))// && SQLComm.IsTraining)
                {
                    action.Weight = Math.Min(0.45, action.Weight);
                }
            }

            /*
             * Special select, find closest if a weight is -1 Very specific
             */
            if (actions[0].Action == ExecutorType.Select.ToString())
            {
                foreach (ActionInfo action in actions)
                {
                    //if (action.Weight >= 0.45)
                    //    continue;

                    // From select string builder
                    var words = action.Name.Split(';');

                    if (words.Length < 4)
                        continue;

                    string name = words[0];
                    string location = words[1];
                    string position = words[2];
                    string controller = words[3];
                    string desc = action.Desc.ToString();

                    List<double> similar1 = new List<double>();
                    List<double> similar2 = new List<double>();

                    foreach (ActionInfo allSelect in allSelectActions)
                    {
                        var Swords = allSelect.Name.Split(';');


                        if (Swords.Length < 4)
                            continue;

                        string Sname = Swords[0];
                        string Slocation = Swords[1];
                        string Sposition = Swords[2];
                        string Scontroller = Swords[3];
                        string Sdesc = "";
                        if (Swords.Length > 4)
                            Sdesc = Swords[4];

                        if ((int)allSelect.ActionId >= best_predict.Count)
                            continue; // Not sure if it is a bug, but the last action id of the list always cuts off

                        double weight = best_predict[(int)allSelect.ActionId];

                        if (weight < action.Weight)
                            continue;

                        if (weight <= 0)
                            continue;

                        int similarity = 0;

                        // Check similrity
                        if (desc != Sdesc)
                            continue;

                        if (location == Slocation)
                            similarity++;

                        if (controller == Scontroller)
                        {
                            similarity++;
                            if (name == Sname)
                                similarity++;
                        }

                        if (similarity == 1)
                        {
                            similar1.Add(weight);
                        }
                        else if (similarity >= 2)
                        {
                            similar2.Add(weight);
                        }
                    }

                    double total1 = similar1.Aggregate(0.0, (a, b) => a + b);
                    double total2 = similar2.Aggregate(0.0, (a, b) => a + b);

                    if (similar1.Count > 0) total1 /= similar1.Count;
                    if (similar2.Count > 0) total2 /= similar2.Count;

                    double multi1 = 0.2;
                    double multi2 = 0.3;
                    double multi3 = 0.5;

                    if (total1 == 0)
                    {
                        multi2 = 0.5;
                    }
                    else if (total2 == 0)
                    {
                        multi1 = 0.5;
                    }
                    else
                    {
                        multi3 = 1;
                    }

                    action.Weight = Math.Max(0, action.Weight);
                    action.Weight = Math.Min(1, total1 * multi1 + total2 * multi2 + action.Weight * multi3);
                }
            }
            // Find similar actions for activate
            else
            {
                foreach (ActionInfo action in actions)
                {
                    //if (action.Action != ExecutorType.Activate.ToString())
                    //    continue;

                    if (action.Weight >= 0)
                        continue;

                    // From string builder
                    var words = action.Name.Split(';');

                    if (words.Length < 5)
                        continue;

                    string name = words[0];
                    string id = words[1];
                    string location = words[2];
                    string phase = words[3];
                    string player = words[4]; // Current player turn
                    //string desc = action.Desc.ToString();

                    List<double> similar1 = new List<double>();
                    List<double> similar2 = new List<double>();

                    foreach (ActionInfo allSelect in allSelectActions)
                    {
                        var Swords = allSelect.Name.Split(';');

                        if (Swords.Length < 5)
                            continue;

                        string Sname = Swords[0];
                        string Sid = Swords[1];
                        string Slocation = Swords[2];
                        string Sphase = Swords[3];
                        string Splayer = Swords[4];  // Current player turn
                        //string Sdesc = "";
                        //if (Swords.Length > 4)
                        //    Sdesc = Swords[4];

                        if ((int)allSelect.ActionId >= best_predict.Count)
                            continue; // Not sure if it is a bug, but the last action id of the list always cuts off

                        // Make sure its the same card
                        if (name != Sname)
                            continue;

                        // make sure its the same action
                        if (action.Action != allSelect.Action)
                            continue;

                        double weight = best_predict[(int)allSelect.ActionId];

                        if (weight < action.Weight)
                            continue;

                        if (weight <= 0)
                            continue;

                        int similarity = 0;


                        if (location == Slocation)
                            similarity++;

                        if (phase == Sphase)
                            similarity++;

                        if (player == Splayer)
                            similarity++;


                        if (similarity == 1)
                        {
                            similar1.Add(weight);
                        }
                        else if (similarity >= 2)
                        {
                            similar2.Add(weight);
                        }
                    }

                    double total1 = similar1.Aggregate(0.0, (a, b) => a + b);
                    double total2 = similar2.Aggregate(0.0, (a, b) => a + b);

                    if (similar1.Count > 0) total1 /= similar1.Count;
                    if (similar2.Count > 0) total2 /= similar2.Count;

                    double multi1 = 0.3;
                    double multi2 = 0.4;
                    double multi3 = 0.3;

                    if (total1 == 0)
                    {
                        multi2 = 0.5;
                        multi3 = 0.5;
                    }
                    else if (total2 == 0)
                    {
                        multi1 = 0.5;
                        multi3 = 0.5;
                    }
                    else
                    {
                        multi3 = 1;
                    }

                    action.Weight = Math.Max(0, action.Weight);
                    action.Weight = Math.Min(1, total1 * multi1 + total2 * multi2 + action.Weight * multi3);
                }
            }


            return actions;
        }
    }
}

