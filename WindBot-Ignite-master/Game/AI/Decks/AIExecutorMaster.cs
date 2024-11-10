using YGOSharp.OCGWrapper.Enums;
using System.Collections.Generic;
using WindBot;
using WindBot.Game;
using WindBot.Game.AI;
using System.Linq;

namespace WindBot.Game.AI.Decks
{
    [Deck("Master", "AI_Random2")]
    public class AIExecutorMaster : DefaultExecutor
    {
        public class CardId
        {
            public const int Alexandrite = 43096270;
            public const int Avram = 84754430;
            public const int Luster = 11091375;
            public const int Sabersaurus = 37265642;
            public const int Artorigus = 92125819;
            public const int CyberDragon = 70095154;
            public const int Tricky = 14778250;
            public const int SnipeHunter = 84290642;
            public const int WindUpSoldier = 12299841;
            public const int ExiledForce = 74131780;
            public const int Snowman = 91133740;
            public const int Krawler = 88316955;
            public const int BlockAttack = 25880422;
            public const int PotOfGreed = 55144522;
            public const int StopDefence = 63102017;
            public const int ManEaterBug = 54652250;

        }

        public AIExecutorMaster(GameAI ai, Duel duel)
            : base(ai, duel)
        {
            AddExecutor(ExecutorType.Activate, CardId.PotOfGreed);

            AddExecutor(ExecutorType.SpSummon, CardId.CyberDragon);

            //AddExecutor(ExecutorType.Summon, CardId.SnipeHunter);
            AddExecutor(ExecutorType.Summon, CardId.ExiledForce);
            AddExecutor(ExecutorType.Summon, CardId.WindUpSoldier, SnipeHunterSummon);

            AddExecutor(ExecutorType.MonsterSet, CardId.Krawler);

            AddExecutor(ExecutorType.Summon, CardId.Avram);
            AddExecutor(ExecutorType.Summon, CardId.Alexandrite);
            AddExecutor(ExecutorType.Summon, CardId.Luster);
            AddExecutor(ExecutorType.Summon, CardId.Sabersaurus);
            AddExecutor(ExecutorType.Summon, CardId.Artorigus);
            AddExecutor(ExecutorType.Summon, CardId.WindUpSoldier);
            AddExecutor(ExecutorType.MonsterSet, CardId.Snowman);
            AddExecutor(ExecutorType.MonsterSet, CardId.ManEaterBug);

            AddExecutor(ExecutorType.SpSummon, CardId.Tricky);

            AddExecutor(ExecutorType.Activate, CardId.StopDefence, ActivateStopDefence);
            AddExecutor(ExecutorType.Activate, CardId.BlockAttack, ActivateBlockAttack);
            AddExecutor(ExecutorType.Activate, CardId.SnipeHunter, ActivateExiledForce);
            AddExecutor(ExecutorType.Activate, CardId.ExiledForce, ActivateExiledForce);
            AddExecutor(ExecutorType.Activate, CardId.ManEaterBug, ActivateExiledForce);
            AddExecutor(ExecutorType.Activate, CardId.Krawler, SnipeHunterSummon);
            AddExecutor(ExecutorType.Activate, CardId.Krawler, ActivateExiledForce);
            AddExecutor(ExecutorType.Activate, CardId.Snowman, ActivateExiledForce);

            AddExecutor(ExecutorType.Repos, DefaultMonsterRepos);
        }

        private List<long> HintMsgForEnemy = new List<long>
        {
            HintMsg.Release, HintMsg.Destroy, HintMsg.Remove, HintMsg.ToGrave, HintMsg.ReturnToHand, HintMsg.ToDeck,
            HintMsg.FusionMaterial, HintMsg.SynchroMaterial, HintMsg.XyzMaterial, HintMsg.LinkMaterial
        };

        private List<long> HintMsgForDeck = new List<long>
        {
            HintMsg.SpSummon, HintMsg.ToGrave, HintMsg.Remove, HintMsg.AddToHand, HintMsg.FusionMaterial
        };

        private List<long> HintMsgForSelf = new List<long>
        {
            HintMsg.Equip
        };

        private List<long> HintMsgForMaterial = new List<long>
        {
            HintMsg.FusionMaterial, HintMsg.SynchroMaterial, HintMsg.XyzMaterial, HintMsg.LinkMaterial, HintMsg.Release
        };

        private List<long> HintMsgForMaxSelect = new List<long>
        {
            HintMsg.SpSummon, HintMsg.ToGrave, HintMsg.AddToHand, HintMsg.FusionMaterial, HintMsg.Destroy
        };

        private bool ActivateBlockAttack()
        {
            ClientCard target = null;
            int highest_attack = 0;
            if (Util.GetBestBotMonster(true) != null) highest_attack = Util.GetBestBotMonster(true).Attack;
            foreach (ClientCard card in Util.Enemy.GetMonsters())
                if (card.Position == (int)CardPosition.Attack)
                    if (card.Attack > card.Defense && (target == null || card.Attack > target.Attack)
                        && highest_attack > card.Defense)
                        target = card;
            if (target != null)
            {
                AI.SelectCard(target);
                return true;
            }
            return false;
        }

        private bool ActivateStopDefence()
        {
            ClientCard target = null;
            int highest_attack = 0;
            if (Util.GetBestBotMonster(true) != null) highest_attack = Util.GetBestBotMonster(true).Attack;
            foreach (ClientCard card in Util.Enemy.GetMonsters())
                if (card.Position == (int)CardPosition.Defence)
                    if (card.Defense > card.Attack && (target == null || card.Defense > target.Defense)
                         && highest_attack > card.Attack)
                        target = card;
            if (target != null)
            {
                AI.SelectCard(target);
                return true;
            }
            return false;
        }

        private bool ActivateExiledForce()
        {
            ClientCard target = Util.GetBestEnemyMonster(true, true);
            if (target != null)
            {
                AI.SelectCard(target);
                return true;
            }
            return false;
        }

        private bool SnipeHunterSummon()
        {
            IList<int> atkTarg = new[] {
                CardId.CyberDragon,
                CardId.Tricky,
                CardId.Alexandrite,
                CardId.Avram
            };

            IList<int> defTarg = new[] {
                CardId.Krawler
            };

            return (Util.Enemy.HasInMonstersZone(atkTarg) && !Bot.HasInHand(CardId.BlockAttack))
                || (Util.Enemy.HasInMonstersZone(defTarg) && !Bot.HasInHand(CardId.StopDefence));
        }

        public override bool OnSelectHand()
        {
            //if (SQLComm.IsTraining)
            return SQLComm.IsFirst;
            return base.OnSelectHand();
        }

        public override IList<ClientCard> OnSelectCard(IList<ClientCard> _cards, int min, int max, long hint, bool cancelable)
        {
            if (Duel.Phase == DuelPhase.BattleStart)
                return null;
            if (AI.HaveSelectedCards())
                return null;

            IList<ClientCard> selected = new List<ClientCard>();
            IList<ClientCard> cards = new List<ClientCard>(_cards);
            if (max > cards.Count)
                max = cards.Count;

            if (HintMsgForEnemy.Contains(hint))
            {
                IList<ClientCard> enemyCards = cards.Where(card => card.Controller == 1).ToList();

                // select enemy's card first
                while (enemyCards.Count > 0 && selected.Count < max)
                {
                    ClientCard card = enemyCards[Rand.Next(enemyCards.Count)];
                    selected.Add(card);
                    enemyCards.Remove(card);
                    cards.Remove(card);
                }
            }

            if (HintMsgForDeck.Contains(hint))
            {
                IList<ClientCard> deckCards = cards.Where(card => card.Location == CardLocation.Deck).ToList();

                // select deck's card first
                while (deckCards.Count > 0 && selected.Count < max)
                {
                    ClientCard card = deckCards[Rand.Next(deckCards.Count)];
                    selected.Add(card);
                    deckCards.Remove(card);
                    cards.Remove(card);
                }
            }

            if (HintMsgForSelf.Contains(hint))
            {
                IList<ClientCard> botCards = cards.Where(card => card.Controller == 0).ToList();

                // select bot's card first
                while (botCards.Count > 0 && selected.Count < max)
                {
                    ClientCard card = botCards[Rand.Next(botCards.Count)];
                    selected.Add(card);
                    botCards.Remove(card);
                    cards.Remove(card);
                }
            }

            if (HintMsgForMaterial.Contains(hint))
            {
                IList<ClientCard> materials = cards.OrderBy(card => card.Attack).ToList();

                // select low attack first
                while (materials.Count > 0 && selected.Count < min)
                {
                    ClientCard card = materials[0];
                    selected.Add(card);
                    materials.Remove(card);
                    cards.Remove(card);
                }
            }

            // select random cards
            while (selected.Count < min)
            {
                ClientCard card = cards[Rand.Next(cards.Count)];
                selected.Add(card);
                cards.Remove(card);
            }

            if (HintMsgForMaxSelect.Contains(hint))
            {
                // select max cards
                while (selected.Count < max)
                {
                    ClientCard card = cards[Rand.Next(cards.Count)];
                    selected.Add(card);
                    cards.Remove(card);
                }
            }

            return selected;
        }

        public override int OnSelectOption(IList<long> options)
        {
            return Rand.Next(options.Count);
        }

    }
}
