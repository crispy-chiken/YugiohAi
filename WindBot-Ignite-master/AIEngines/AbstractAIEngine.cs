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
    public abstract class AbstractAIEngine
    {
        public class FieldStateValues
        {
            public long Id = -1;
            public string Location = "";
            public string Compare = "";
            public string Value = "";

            public FieldStateValues(string location, string compare, string value)
            {
                Location = location;
                Compare = compare;
                Value = value;
                
                foreach(var state in allFieldStateValues)
                {
                    if (state.Location != Location)
                        continue;
                    if (state.Compare != Compare)
                        continue;
                    if (state.Value != Value)
                        continue;

                    Id = state.Id;
                    break;
                }

                if (Id == -1)
                {
                    Id = SQLComm.GetComparisonId(this);
                    allFieldStateValues.Add(this);
                }
            }

            public FieldStateValues()
            {

            }
            
            public override string ToString()
            {
                return "[" + Id + "]" + Location + " " + Compare + " " + Value;
            }

            public bool Equals(FieldStateValues o)
            {
                if (o == null)
                    return false;
                return o.Location == Location &&
                       o.Value == Value &&
                       o.Compare == Compare;
            }
        }

        public class History
        {
            public GameInfo Info;

            public List<ActionInfo> ActionInfo = new List<ActionInfo>();
            public List<FieldStateValues> FieldState = new List<FieldStateValues>();

            public int CurP1Hand = 0;
            public int CurP1Field = 0;
            public int CurP2Hand = 0;
            public int CurP2Field = 0;

            public int PostP1Hand = -1;
            public int PostP1Field = -1;
            public int PostP2Hand = -1;
            public int PostP2Field = -1;

            public long Id = -1;

            public History(GameInfo info, List<ActionInfo> actions, List<FieldStateValues> fieldState)
            {
                Info = info;
                ActionInfo = actions;
                FieldState = fieldState;
            }

            public bool Equals(History o)
            {
                if (o == null)
                    return false;
                return o.ActionInfo.Equals(ActionInfo) && o.FieldState.Equals(FieldState);
            }
        }

        public class GameInfo
        {
            public int Game = 0;
            public int Turn = 0;
            public int ActionNumber = 0;

            public GameInfo(int game, int turn, int actionNumber)
            {
                Game = game;
                Turn = turn;
                ActionNumber = actionNumber;
            }
        }

        public class ActionInfo
        {
            public string Name = "";
            public string Action = "";
            public long ActionId = -1;
            public bool Performed = false;
            public ClientCard Card = null;
            public long Desc = -1;

            public double Weight = -1;

            public ActionInfo(long actionId, string name, string action)
            {
                ActionId = actionId;

                if (name.Length > 0)
                {
                    var last_dividor = name.LastIndexOf(";");

                    Name = name.Substring(0, last_dividor);
                    Desc = long.Parse(name.Substring(last_dividor + 1));
                }
                Action = action;
            }

            public ActionInfo(string name, string action, double weight)
                : this(name, action, null, -1)
            {
                Weight = weight;
            }

            public ActionInfo(string name, string action, ClientCard card)
                : this(name, action, card, -1) { }

            public ActionInfo(ActionInfo toCopy)
            {
                ActionId = toCopy.ActionId;
                Name = toCopy.Name;
                Action = toCopy.Action;
                Card = toCopy.Card;
                Desc = toCopy.Desc;
            }

            public ActionInfo(string name, string action, ClientCard card, long desc)
            {
                Name = name;
                Action = action;
                Card = card;
                Desc = desc;

                foreach (var select in allSelectActions)
                {
                    if (select.Name != Name)
                        continue;
                    if (select.Action != Action)
                        continue;
                    if (select.Desc != Desc)
                        continue;

                    ActionId = select.ActionId;
                    break;
                }

                if (ActionId == -1)
                {
                    ActionId = SQLComm.GetActionId(this);
                    allSelectActions.Add(this);
                }
            }

            public override string ToString()
            {
                string str = "[" + ActionId + "]" + Action?.ToString() + " " + Name;
                if (Desc >= 0)
                    str += " " + Desc.ToString();
                return str;
            }

            public bool Equals(ActionInfo obj)
            {
                if (obj == null)
                {
                    return false;
                }
                return obj.Name == Name &&
                       obj.Action == Action &&
                       obj.Desc == Desc;
            }
        }


        protected static List<ActionInfo> allSelectActions = SQLComm.GetAllActions().Values.ToList();
        protected static List<FieldStateValues> allFieldStateValues = SQLComm.GetAllComparisons();

        protected Executor source;


        protected List<History> Records { get; set;  } = new List<History>();
        protected int ActionNumber { get; set; } = 0;
        protected ActionInfo BestAction = null;

        private List<History> CurrentTurn = new List<History>();

        public AbstractAIEngine(Executor source)
        {
            this.source = source;
        }

        protected abstract ActionInfo GetBestAction(History history);

        public virtual void OnWin(int result)
        {
            SQLComm.SavePlayHistory(Records, result);
            allSelectActions = SQLComm.GetAllActions().Values.ToList();
            allFieldStateValues = SQLComm.GetAllComparisons();
            Records.Clear();
        }

        public bool ShouldPerform(ClientCard card, string action, long desc, List<FieldStateValues> fieldState, Duel duel)
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

                History history = new History(info, actions, fieldState);
                ActionInfo best = GetBestAction(history);
                if (best == null || best != actionInfo)
                {
                    // return false;
                }
                else
                {
                    best.Performed = true;
                }

                AddHistory(history, duel);

                return actionInfo.Performed;
            }
        }

        public void OnNewTurn(Duel duel)
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


        public void OnNewPhase()
        {
            BestAction = null;
        }

        public void OnChainSolving()
        {
            BestAction = null;
        }

        public void OnChainSolved()
        {
            BestAction = null;
        }

        public void SetMain(MainPhase main, List<FieldStateValues> fieldState, Duel duel)
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
            else if (main.CanEndPhase)
            {
                actions.Add(new ActionInfo("", ExecutorType.GoToEndPhase.ToString(), null));
            }

            History history = new History(gameInfo, actions, fieldState);
            BestAction = GetBestAction(history);

            if (BestAction != null)
                BestAction.Performed = true;

            AddHistory(history, duel);
        }
        public void SetChain(IList<ClientCard> cards, IList<long> descs, bool forced, List<FieldStateValues> fieldState, Duel duel, AIUtil Util)
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


            History history = new History(gameInfo, actions, fieldState);
            BestAction = GetBestAction(history);

            if (BestAction != null)
                BestAction.Performed = true;
            //if (BestAction == dontPerform)
            //    BestAction = null;

            AddHistory(history, duel);

        }
        public void SetBattle(BattlePhase battle, List<FieldStateValues> fieldState, Duel duel)
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
                //actions.Add(new ActionInfo("", ExecutorType.GoToMainPhase2.ToString(), null));
            }

            History history = new History(gameInfo, actions, fieldState);
            BestAction = GetBestAction(history);

            if (BestAction != null)
                BestAction.Performed = true;
            //if (BestAction == dontPerform)
            //    BestAction = null;

            AddHistory(history, duel);
        }

        public IList<ClientCard> SelectCards(ClientCard currentCard, int min, int max, long hint, bool cancelable, IList<ClientCard> selectable, List<FieldStateValues> fieldState, Duel duel)
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


                for (int i = min; i <= max; i++)
                {
                    string card = i.ToString() + ";" + BuildActionString(currentCard, duel);
                    actions.Add(new ActionInfo(card, "SelectAmount", currentCard, hint));
                }

                History history = new History(gameInfo, actions, fieldState);
                ActionInfo choice = GetBestAction(history);

                if (choice != null)
                {
                    choice.Performed = true;
                }

                AddHistory(history, duel);

                toSelect = actions.FindIndex(x => x == choice) + min;
            }

            {
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
                    ActionNumber++;

                    History history = new History(gameInfo, actions, fieldState);
                    ActionInfo choice = GetBestAction(history);

                    if (choice != null)
                    {
                        choice.Performed = true;
                        selected.Add(choice.Card);
                        actionCopy.Remove(choice);
                    }

                    AddHistory(history, duel);
                }
            }

            return selected;
        }
        public int SelectOption(IList<long> options, List<FieldStateValues> fieldState, Duel duel, AIUtil Util)
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

                History history = new History(gameInfo, actions, fieldState);
                BestAction = GetBestAction(history);

                if (BestAction != null)
                    BestAction.Performed = true;

                AddHistory(history, duel);

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

                History history = new History(gameInfo, actions, fieldState);
                BestAction = GetBestAction(history);

                if (BestAction != null)
                    BestAction.Performed = true;

                AddHistory(history, duel);

                return options.IndexOf(BestAction.Desc);
            }
        }

        public ClientCard OnSelectAttacker(IList<ClientCard> attackers, List<FieldStateValues> fieldState, Duel duel)
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

            History history = new History(gameInfo, actions, fieldState);
            ActionInfo choice = GetBestAction(history);

            if (choice != null)
            {
                choice.Performed = true;
            }


            AddHistory(history, duel);

            return choice.Card;
        }

        public ClientCard OnSelectAttackTarget(ClientCard attacker, IList<ClientCard> defenders, List<FieldStateValues> fieldState, Duel duel)
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

            History history = new History(gameInfo, actions, fieldState);
            ActionInfo choice = GetBestAction(history);

            if (choice != null)
            {
                choice.Performed = true;
            }


            AddHistory(history, duel);

            return choice?.Card;
        }

        public CardPosition OnSelectPosition(int cardId, IList<CardPosition> positions, List<FieldStateValues> fieldState, Duel duel)
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


            History history = new History(gameInfo, actions, fieldState);
            ActionInfo choice = GetBestAction(history);

            if (choice != null)
            {
                choice.Performed = true;
            }

            cardPosition = positions[actions.FindIndex(x => x.Performed)];

            AddHistory(history, duel);


            return cardPosition;
        }


        public int OnAnnounceCard(ClientCard card, IList<int> avail, List<FieldStateValues> fieldState, Duel duel)
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


            History history = new History(gameInfo, actions, fieldState);
            ActionInfo choice = GetBestAction(history);

            if (choice != null)
            {
                choice.Performed = true;
                chosen = int.Parse(choice.Name);
            }


            AddHistory(history, duel);


            return chosen;

        }     

        protected void AddHistory(History history, Duel duel)
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

        protected string SelectStringBuilder(ClientCard Card, int Quant = 1)
        {
            return $"{Card.Name ?? "Set Card" };{Card?.Location.ToString()};{Card?.Position.ToString()};{Card.Controller}";// x{Quant}";
        }

        protected string BuildActionString(ClientCard card, Duel duel)
        {
            //if (phase == "Main2")
            //    phase = "Main1";
            if (card == null)
                return "";
            string actionString = card.Name ?? "Uknown";
            actionString += ";" + card.Id;
            actionString += ";" + card.Location;
            actionString += ";" + duel.Phase.ToString();
            actionString += ";" + duel.Player.ToString();
            return actionString;
        }

        // TODO Sort out the card to be a comparison list instead of all the details in one list
        protected List<ActionInfo> GetCardComparisonDetails(ClientCard card)
        {
            return null;
        }

        protected float GetCardComparisonWeight(ClientCard card, List<ActionInfo> details)
        {
            return 0;
        }
    }
}

