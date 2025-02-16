using YGOSharp.OCGWrapper.Enums;
using System.Collections.Generic;
using WindBot;
using WindBot.Game;
using WindBot.Game.AI;

using static WindBot.NEAT;
using System.Linq;
using System;

namespace WindBot.Game.AI.Decks
{
    [Deck("Yubel", "AI_Yubel")]
    public class AIYubel : AIHardCodedBase
    {
        int[] dont_use_as_link_mat =
        {
            CardId.UnchainedSoulRage,
            CardId.SPLittleKnight,
            CardId.FiendsmithDiesIrae,
            CardId.PhantomOfYubel,
            CardId.VarudrasBringerofEndTimes,
            CardId.DDDHighKingCaesar
        };

        public AIYubel(GameAI ai, Duel duel)
            : base(ai, duel)
        {
            // Basically First Actions

            // Normal Priority
            AddExecutor(ExecutorType.Activate, CardId.Yubel);
            AddExecutor(ExecutorType.Activate, CardId.Yubel11);
            AddExecutor(ExecutorType.Activate, CardId.Yubel12);
            AddExecutor(ExecutorType.Activate, CardId.SpiritOfYubel);
            AddExecutor(ExecutorType.Activate, CardId.BystialDruiswurm, BystialActivate);
            AddExecutor(ExecutorType.Activate, CardId.BystialMagnamhut, BystialActivate);

            AddExecutor(ExecutorType.Activate, CardId.TheFiendsmith, TheFiendsmithActivate);
            AddExecutor(ExecutorType.SpSummon, CardId.FiendsmithRequiem, FiendSmithRequiemSummon);
            AddExecutor(ExecutorType.SpSummon, CardId.FiendsmithSequentia, FiendsmithSequentiaSummon);
            AddExecutor(ExecutorType.SpSummon, CardId.VarudrasBringerofEndTimes);
            AddExecutor(ExecutorType.SpSummon, CardId.BeatriceLadyOfEnternal, BeatriceSummon);
            AddExecutor(ExecutorType.SpSummon, CardId.DDDHighKingCaesar);
            AddExecutor(ExecutorType.Activate, CardId.Terraforming);
            AddExecutor(ExecutorType.Activate, CardId.FiendsmithTractus);
            AddExecutor(ExecutorType.Activate, CardId.FiendsmithDiesIrae, FiendsmithDiesIraeActivate);
            AddExecutor(ExecutorType.Activate, CardId.FiendsmithLacrimosa);
            AddExecutor(ExecutorType.Activate, CardId.FiendsmithSequentia, FiendsmithSequentiaActivate);
            AddExecutor(ExecutorType.Activate, CardId.FiendsmithRequiem, FiendsmithRequiemActivate);
            AddExecutor(ExecutorType.Activate, CardId.BeatriceLadyOfEnternal, BeatriceActivate);
            //AddExecutor(ExecutorType.SpSummon, CardId.MoonOfTheClosedHeaven, ClosedHeavenSummon);

            AddExecutor(ExecutorType.Activate, CardId.NightmarePain, NightmarePainActivate);
            AddExecutor(ExecutorType.Activate, CardId.OpeningOfTheSpritGates, SpiritGatesActivate);
            AddExecutor(ExecutorType.Activate, CardId.NightmareThrone);

            AddExecutor(ExecutorType.Summon, CardId.DarkBeckoningBeast);
            AddExecutor(ExecutorType.Summon, CardId.SamsaraDLotus);
            AddExecutor(ExecutorType.Summon, CardId.ChaosSummoningBeast);

            AddExecutor(ExecutorType.Activate, CardId.UnchainedSoulSharvara, ShavaraActivate);
            AddExecutor(ExecutorType.Activate, CardId.DarkBeckoningBeast);
            AddExecutor(ExecutorType.Activate, CardId.GruesumGraveSquirmer, GraveSquirmerActivate);
            AddExecutor(ExecutorType.Activate, CardId.ChaosSummoningBeast);
            AddExecutor(ExecutorType.Activate, CardId.ChaosSummoningBeast);
            AddExecutor(ExecutorType.Activate, CardId.EscapeOfUnchained, EscapeActivate);
            AddExecutor(ExecutorType.Activate, CardId.PhantomOfYubel, PhantomActivate);
            AddExecutor(ExecutorType.Activate, CardId.VarudrasBringerofEndTimes, VarudrasActivate);
            AddExecutor(ExecutorType.Activate, CardId.DDDHighKingCaesar, CaesarActivate);
            AddExecutor(ExecutorType.Activate, CardId.UnchainedSoulYama);
            AddExecutor(ExecutorType.Activate, CardId.KnightmarePhoenix);
            AddExecutor(ExecutorType.SpSummon, CardId.UnchainedSoulRage, RageActivate);
            AddExecutor(ExecutorType.Activate, CardId.SPLittleKnight, SPActivate);

            AddExecutor(ExecutorType.SpSummon, CardId.PhantomOfYubel, PhantomSummon);

            AddExecutor(ExecutorType.Activate, CardId.Muckracker, MuckrackerActivate);
            AddExecutor(ExecutorType.SpSummon, CardId.UnchainedSoulYama, YamaSummon);
            AddExecutor(ExecutorType.SpSummon, CardId.KnightmarePhoenix, PhoenixSummon);
            AddExecutor(ExecutorType.SpSummon, CardId.UnchainedSoulRage);
            AddExecutor(ExecutorType.SpSummon, CardId.SPLittleKnight, SPSummon);
            AddExecutor(ExecutorType.SpSummon, CardId.Muckracker, MuckrackerSummon);


            // Low Priority

            // Reactive

            AddExecutor(ExecutorType.SpellSet, DefaultSpellSet);

            AddExecutor(ExecutorType.Repos, DefaultMonsterRepos);
            AddExecutor(ExecutorType.GoToBattlePhase);
        }


        // Choose Go first or second
        public override bool OnSelectHand()
        {
            bool choice = true;
            return choice;
        }


        // Assume no extra deck side
        public override void OnChangeSide(IList<int> _main, IList<int> _extra, IList<int> _side)
        {
            IList<int> pool = new List<int>(_main);
            pool = pool.Concat(_side).ToList();
            int mainCount = _main.Count();
            int sideCount = _side.Count();

            _main.Clear();
            _side.Clear();

            Archetypes enemyDeck = GetEnemyDeckType();

            {
                int[] minMain =
                {
               
                };

                AddCardsToList(_main, pool, mainCount, minMain);
            }

            switch(enemyDeck)
            {
                case Archetypes.SnakeEyes:
                    if (winResult == 0) // win
                    {
                        int[] toAdd =
                        {
                         
                        };
                        AddCardsToList(_main, pool, mainCount, toAdd);
                    }
                    else // lose
                    {
                        int[] toAdd =
                        {

                        };
                        AddCardsToList(_main, pool, mainCount, toAdd);
                    }
                    break;
            }



            // Fill out the remaining main deck and side deck slots.
            AddCardsToList(_main, pool, mainCount);
            AddCardsToList(_side, pool, sideCount);

            postSide = true;
        }

        /// <summary>
        /// Decide which card should the attacker attack.
        /// </summary>
        /// <param name="attacker">Card that attack.</param>
        /// <param name="defenders">Cards that defend.</param>
        /// <returns>BattlePhaseAction including the target, or null (in this situation, GameAI will check the next attacker)</returns>
        public override BattlePhaseAction OnSelectAttackTarget(ClientCard attacker, IList<ClientCard> defenders)
        {
            // Attack with yubels
            int[] yubels =
            {
                CardId.Yubel,
                CardId.Yubel11,
                CardId.Yubel12,
                CardId.SpiritOfYubel,
                CardId.PhantomOfYubel
            };
            if (yubels.Any(x => x == attacker.Id) && !attacker.IsDisabled())
            {
                ClientCard toAttack = null;
                if (defenders.Count > 0)
                    toAttack = defenders.OrderByDescending(x => x.GetDefensePower()).FirstOrDefault();

                return AI.Attack(attacker, toAttack);
            }

            return base.OnSelectAttackTarget(attacker, defenders);
        }

        public override bool OnSelectYesNo(long desc)
        {
            var option = Util.GetOptionFromDesc(desc);
            var cardId = Util.GetCardIdFromDesc(desc);
            return base.OnSelectYesNo(desc);
        }

        public override IList<ClientCard> OnSelectCard(IList<ClientCard> _cards, int min, int max, long hint, bool cancelable)
        {
            if (AI.HaveSelectedCards())
                return null;

            ClientCard currentCard = GetCurrentCardResolveInChain();
            IList<ClientCard> selected = new List<ClientCard>();

            #region AI Selected
            if (currentCard != null)
            {
                if (CardId.BeatriceLadyOfEnternal == currentCard.Id)
                {
                    selected.Add(_cards.Where(x => x.Id == CardId.SamsaraDLotus).FirstOrDefault());
                }
            }
            else
            {
                if (hint == HintMsg.LinkMaterial)
                {
                    IList<ClientCard> highPriority = new List<ClientCard>();
                    IList<ClientCard> lowPriority = new List<ClientCard>();

                    int[] highList =
                    {
                        CardId.DarkBeckoningBeast,
                        CardId.SamsaraDLotus,
                        CardId.FabledLurrie,
                        CardId.ChaosSummoningBeast,
                    };
                    int[] lowList = dont_use_as_link_mat;

                    _cards = _cards
                        .OrderBy(x => highList.Contains(x.Id) ? 0 : 1)
                        .ThenBy(x => x.HasType(CardType.Link) ? -x.LinkCount : 0) // Use the highest link monster first
                        .ThenBy(x => lowList.Contains(x.Id) ? 1 : 0)
                        .ToList();

                    // Stop selecting if using low priorty cards
                    if (materialSelected > 0 && cancelable && _cards.Where(x => lowList.Contains(x.Id)).Count() == _cards.Count())
                        return null;

                    materialSelected += 1;
                }
            }
            #endregion

            return SelectMinimum(selected, _cards, min, max, hint);
        }

        public override IList<ClientCard> OnCardSorting(IList<ClientCard> cards)
        {
            return base.OnCardSorting(cards);
        }


        public override int OnSelectOption(IList<long> options)
        {
            foreach(long desc in options)
            {
                var option = Util.GetOptionFromDesc(desc);
                var cardId = Util.GetCardIdFromDesc(desc);
            }

            if (options.Count == 2)
            {
                var hint = options[0];
                var cardId = Util.GetCardIdFromDesc(options[1]);

            }

            return base.OnSelectOption(options);
        }

        public override IList<ClientCard> OnSelectSum(IList<ClientCard> cards, int sum, int min, int max, long hint, bool mode)
        {
            return base.OnSelectSum(cards, sum, min, max, hint, mode);
        }

        public override int OnAnnounceCard(IList<int> avail)
        {
            return base.OnAnnounceCard(avail);
        }


        #region Generic Actions

        #endregion


        #region Generic Monsters
        public bool ShavaraActivate()
        {
            return true;
        }

        public bool BeatriceActivate()
        {
            return true;
        }

        public bool BeatriceSummon()
        {
            return true;
        }

        public bool VarudrasActivate()
        {
            return true;
        }

        public bool CaesarActivate()
        {
            return true;
        }

        public bool RageActivate()
        {
            return true;
        }

        public bool SPActivate()
        {
            return true;
        }

        public bool SPSummon()
        {
            if (Bot.GetLinkMaterialWorth(dont_use_as_link_mat) < 2)
                return false;
            return true;
        }

        public bool MuckrackerActivate()
        {
            return true;
        }

        public bool MuckrackerSummon()
        {
            if (Bot.GetLinkMaterialWorth(dont_use_as_link_mat) < 2)
                return false;
            if (Enemy.HasInSpellZone(CardId.AntiSpellFragrance))
                return true;
            return false;
        }

        public bool YamaSummon()
        {
            if (Bot.GetLinkMaterialWorth(dont_use_as_link_mat) < 2)
                return false;
            return true;
        }

        public bool PhoenixSummon()
        {
            if (Enemy.GetSpells().Any(x => SPELL_FIELD_TARGETS.Any(y => y.Equals(x))))
                return true;
            return false;
        }

        public bool ClosedHeavenSummon()
        {
            return true;
        }

        #endregion


        #region Generic Spells

        #endregion


        #region Generic Traps
        public bool EscapeActivate()
        {
            if (Enemy.GetMonsters().Any(x => MONSTER_FIELD_TARGETS.Any(y => y == x.Id)))
                return true;
            if (Enemy.GetSpells().Any(x => SPELL_FIELD_TARGETS.Any(y => y == x.Id)))
                return true;
            return false;
        }
        #endregion

        #region Yubel
        public bool GraveSquirmerActivate()
        {
            return true;
        }

        public bool NightmarePainActivate()
        {
            return true;
        }

        public bool SpiritGatesActivate()
        {
            return true;
        }

        public bool PhantomActivate()
        {
            return true;
        }

        public bool PhantomSummon()
        {
            return true;
        }
        #endregion
    }
}
