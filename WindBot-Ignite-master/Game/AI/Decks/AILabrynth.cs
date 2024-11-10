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
    [Deck("Labrynth", "AI_Labrynth2")]
    public class AILabrynth : AIHardCodedBase
    {

        public AILabrynth(GameAI ai, Duel duel)
            : base(ai, duel)
        {
            // Basically First Actions
            AddExecutor(ExecutorType.Activate, CardId.EradicatorVirus);
            AddExecutor(ExecutorType.Activate, CardId.AntiSpellFragrance);
            AddExecutor(ExecutorType.Activate, CardId.DimensionalBarrier);
            AddExecutor(ExecutorType.Activate, CardId.DifferentDimensionGround);
            AddExecutor(ExecutorType.Activate, CardId.SimultaneousCannon, SimultaneousCannonActivate);

            // Normal Priority
            AddExecutor(ExecutorType.Summon, CardId.ArianePinkLabrynth);
            AddExecutor(ExecutorType.Summon, CardId.AriannaGreenLabrynth);
            AddExecutor(ExecutorType.Activate, CardId.AriannaGreenLabrynth, GreenActivate);
            AddExecutor(ExecutorType.Activate, CardId.ArianePinkLabrynth);
            AddExecutor(ExecutorType.Activate, CardId.LabrynthChandraglier, FurnatureActivate);
            AddExecutor(ExecutorType.Activate, CardId.LabrynthStovie, FurnatureActivate);
            AddExecutor(ExecutorType.Activate, CardId.LabrynthCooClock, ClockActivate);
            AddExecutor(ExecutorType.Activate, CardId.LovelyLabrynth, LovelyActivate);
            AddExecutor(ExecutorType.Activate, CardId.LadyLabrnyth, LadyActivate);
            AddExecutor(ExecutorType.Activate, CardId.BigWelcomeLabrnyth, BigWelcomeActivate);
            AddExecutor(ExecutorType.Activate, CardId.WelcomeLabrynth, WelcomeActivate);
            AddExecutor(ExecutorType.SpellSet, CardId.BigWelcomeLabrnyth);
            AddExecutor(ExecutorType.SpellSet, CardId.WelcomeLabrynth);
            AddExecutor(ExecutorType.Activate, CardId.LabrynthLabyrinth);
            AddExecutor(ExecutorType.Activate, CardId.LabrynthSetup, SetupActivate);
            AddExecutor(ExecutorType.Summon, CardId.BackJack);
            AddExecutor(ExecutorType.Activate, CardId.BackJack, BackJackActivate);
            AddExecutor(ExecutorType.Activate, CardId.DarumaCannon, DarumaActivate);
            AddExecutor(ExecutorType.Activate, CardId.DogmatikaPunishment, PunishmentActivate);
            AddExecutor(ExecutorType.Activate, CardId.TrapTrick, TraptrickActivate);

            AddExecutor(ExecutorType.SpSummon, CardId.ChaosAngel, ChaosAngelSpecial);
            AddExecutor(ExecutorType.Activate, CardId.ChaosAngel);
            
            AddExecutor(ExecutorType.SpSummon, CardId.Muckracker, MuckrackerSpecial);
            AddExecutor(ExecutorType.Activate, CardId.Muckracker);

            AddExecutor(ExecutorType.SpSummon, CardId.TyphonSkyCrisis, TyphonSummon);
            AddExecutor(ExecutorType.Activate, CardId.TyphonSkyCrisis, TyphonActivate);

            AddExecutor(ExecutorType.SpSummon, CardId.RelinquishdAnima, AnimaSummon);
            AddExecutor(ExecutorType.SpSummon, CardId.UnchainedSoulYama, YamaSummon);
            AddExecutor(ExecutorType.SpSummon, CardId.UnchainedSoulAnguish, AnguishSummon);
            AddExecutor(ExecutorType.SpSummon, CardId.UnchainedSoulAbomination, AbominationSummon);
            AddExecutor(ExecutorType.SpSummon, CardId.SPLittleKnight, SPSummon);

            AddExecutor(ExecutorType.Activate, CardId.EnigmasterPackbit, PackbitActivate);
            AddExecutor(ExecutorType.Activate, CardId.BerfometKingPhantomBeast, BerfometActivate);
            AddExecutor(ExecutorType.Activate, CardId.Garura);
            AddExecutor(ExecutorType.Activate, CardId.ElderEntityNtss);
            AddExecutor(ExecutorType.Activate, CardId.RelinquishdAnima);
            AddExecutor(ExecutorType.Activate, CardId.UnchainedSoulYama);
            AddExecutor(ExecutorType.Activate, CardId.UnchainedSoulAnguish);
            AddExecutor(ExecutorType.Activate, CardId.UnchainedSoulAbomination, AbominationActivate);
            AddExecutor(ExecutorType.Activate, CardId.TriBrigadeBucephalus);
            AddExecutor(ExecutorType.Activate, CardId.SPLittleKnight, SpActivate);
            AddExecutor(ExecutorType.Activate, CardId.LordOfHeavelyPrison, LordPrisonActivate);
            // Low Priority

            // Reactive
            AddExecutor(ExecutorType.Activate, CardId.SolemnStrike, DefaultSolemnStrike);
            AddExecutor(ExecutorType.Activate, CardId.TorrentialTribute, DarumaActivate);
            AddExecutor(ExecutorType.Activate, CardId.CrossoutDesignator, CrossoutActivate);
            AddExecutor(ExecutorType.Activate, CardId.ForbiddenDroplet, DropletActivate);
            AddExecutor(ExecutorType.Activate, CardId.DimensionalBarrier, DefaultDimensionalBarrier);
            AddExecutor(ExecutorType.Activate, CardId.CrossoutDesignator, CosmicActivate);
            AddExecutor(ExecutorType.Activate, CardId.FantasticalPhantazmay);

            AddExecutor(ExecutorType.SpellSet, DefaultSpellSet);

            AddExecutor(ExecutorType.Repos, DefaultMonsterRepos);
            AddExecutor(ExecutorType.GoToBattlePhase);

            // Others
            AddExecutor(ExecutorType.Activate, CardId.RunickFlashingFire, FlashingFireActivate);

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
                    CardId.LadyLabrnyth,
                    CardId.LadyLabrnyth,
                    CardId.LovelyLabrynth,
                    CardId.AriasLabrnyth,
                    CardId.AriasLabrnyth,
                    CardId.AriasLabrnyth,
                    CardId.AriannaGreenLabrynth,
                    CardId.WelcomeLabrynth,
                    CardId.WelcomeLabrynth,
                    CardId.WelcomeLabrynth,
                    CardId.BigWelcomeLabrnyth,
                    CardId.BigWelcomeLabrnyth,
                    CardId.BigWelcomeLabrnyth,
                    CardId.InfiniteImpermanence,
                    CardId.InfiniteImpermanence,
                    CardId.InfiniteImpermanence,
                    CardId.SimultaneousCannon,
                    CardId.SimultaneousCannon,
                    CardId.SimultaneousCannon,
                    CardId.DarumaCannon,
                    CardId.DarumaCannon,
                    CardId.DarumaCannon,
                    CardId.TrapTrick,
                    CardId.TrapTrick,
                    CardId.TrapTrick,
                    CardId.SkillDrain,
                    CardId.SkillDrain,
                    CardId.SkillDrain,
                    CardId.SolemnStrike,
                    CardId.SolemnStrike,
                    CardId.SolemnStrike,
                };

                AddCardsToList(_main, pool, mainCount, minMain);
            }

            if (winResult == 1)
            {
                int[] toAdd =
                {
                    CardId.AriannaGreenLabrynth,
                    CardId.AriannaGreenLabrynth,
                };
                AddCardsToList(_main, pool, mainCount, toAdd);
            }

            switch(enemyDeck)
            {
                case Archetypes.SnakeEyes:
                    if (winResult == 0) // win
                    {
                        int[] toAdd =
                        {
                            CardId.RivalyOfWarlords,
                            CardId.FantasticalPhantazmay,
                            CardId.FantasticalPhantazmay,
                            CardId.FantasticalPhantazmay,
                            CardId.MultchummyPurulia,
                            CardId.MultchummyPurulia,
                            CardId.MultchummyPurulia,
                            CardId.EvenlyMatched,
                            CardId.EvenlyMatched,
                            CardId.EvenlyMatched,
                        };
                        AddCardsToList(_main, pool, mainCount, toAdd);
                    }
                    else // lose
                    {
                        int[] toAdd =
                        {
                            CardId.RivalyOfWarlords,
                            CardId.DifferentDimensionGround,
                            CardId.DifferentDimensionGround,
                            CardId.TorrentialTribute,
                            CardId.TorrentialTribute,
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

        public override bool OnSelectYesNo(long desc)
        {
            var option = Util.GetOptionFromDesc(desc);
            var cardId = Util.GetCardIdFromDesc(desc);

            if ((cardId == CardId.ArianePinkLabrynth || cardId == CardId.AriannaGreenLabrynth)) //Should special/set
            {
                return true;
            }

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
                if (CardId.BigWelcomeLabrnyth == currentCard.Id)
                {
                    if (hint == HintMsg.SpSummon)
                    {
                        if (Bot.GetMonsterCount() == 0)
                        {
                            if (Duel.Player == 0)
                            {
                                if (!Bot.HasInHand(CardId.AriannaGreenLabrynth))
                                    selected.Add(_cards.FirstOrDefault(x => x.Id == CardId.AriannaGreenLabrynth));
                                selected.Add(_cards.FirstOrDefault(x => x.Id == CardId.LadyLabrnyth));
                            }
                            else
                            {
                                selected.Add(_cards.FirstOrDefault(x => x.Id == CardId.LadyLabrnyth));
                                selected.Add(_cards.FirstOrDefault(x => x.Id == CardId.LabrynthStovie));
                                selected.Add(_cards.FirstOrDefault(x => x.Id == CardId.LabrynthChandraglier));
                                selected.Add(_cards.FirstOrDefault(x => x.Id == CardId.LabrynthCooClock));
                            }
                        }
                        else
                        {
                            selected.Add(_cards.FirstOrDefault(x => x.Id == CardId.LovelyLabrynth));
                            selected.Add(_cards.FirstOrDefault(x => x.Id == CardId.AriannaGreenLabrynth));
                            selected.Add(_cards.FirstOrDefault(x => x.Id == CardId.ArianePinkLabrynth));
                            selected.Add(_cards.FirstOrDefault(x => x.Id == CardId.LadyLabrnyth));
                        }
                    }
                    else if (hint == HintMsg.ReturnToHand)
                    {
                        selected.Add(_cards.FirstOrDefault(x => x.Id == CardId.LabrynthCooClock));
                        selected.Add(_cards.FirstOrDefault(x => x.Id == CardId.LabrynthStovie));
                        selected.Add(_cards.FirstOrDefault(x => x.Id == CardId.LabrynthChandraglier));
                        selected.Add(_cards.FirstOrDefault(x => x.Id == CardId.AriasLabrnyth));
                        selected.Add(_cards.FirstOrDefault(x => x.Id == CardId.LadyLabrnyth));
                        selected.Add(_cards.FirstOrDefault(x => x.Id == CardId.AriannaGreenLabrynth));
                        selected.Add(_cards.FirstOrDefault(x => x.Id == CardId.ArianePinkLabrynth));
                    }
                }
                else if (CardId.LabrynthStovie == currentCard.Id || CardId.LabrynthChandraglier == currentCard.Id)
                {
                    if (hint == HintMsg.Discard)
                    {
                        if (Duel.CurrentChain.Any(x => x.Id == CardId.Nibiru && x.Owner == 0))
                            selected.Add(_cards.FirstOrDefault(x => x.Id == CardId.Nibiru));
                        selected.Add(_cards.FirstOrDefault(x => x.Id == CardId.BackJack));
                        selected.Add(_cards.FirstOrDefault(x => x.Id == CardId.LovelyLabrynth));
                        selected.Add(_cards.FirstOrDefault(x => x.Id == CardId.LabrynthChandraglier));
                        selected.Add(_cards.FirstOrDefault(x => x.Id == CardId.LabrynthCooClock));
                        selected.Add(_cards.FirstOrDefault(x => x.Id == CardId.WelcomeLabrynth));
                        selected.Add(_cards.FirstOrDefault(x => x.Id == CardId.BigWelcomeLabrnyth));
                    }
                    else if (hint == HintMsg.Set)
                    {
                        if (!Bot.HasInSpellZoneOrInGraveyard(CardId.BigWelcomeLabrnyth))
                            selected.Add(_cards.FirstOrDefault(x => x.Id == CardId.BigWelcomeLabrnyth));
                        if (!Bot.HasInSpellZone(CardId.WelcomeLabrynth))
                            selected.Add(_cards.FirstOrDefault(x => x.Id == CardId.WelcomeLabrynth));
                        selected.Add(_cards.FirstOrDefault(x => x.Id == CardId.LabrynthLabyrinth));
                        selected.Add(_cards.FirstOrDefault(x => x.Id == CardId.BigWelcomeLabrnyth));
                        selected.Add(_cards.FirstOrDefault(x => x.Id == CardId.WelcomeLabrynth));

                    }
                }
                else if (CardId.AriannaGreenLabrynth == currentCard.Id)
                {
                    if (hint == HintMsg.AddToHand)
                    {
                        if (Duel.Player == 0)
                        {
                            int[] trapsToActivate =
                            {
                                CardId.WelcomeLabrynth,
                                CardId.BigWelcomeLabrnyth,
                                CardId.TrapTrick
                            };
                            if (Bot.HasInHand(trapsToActivate) && !Bot.HasInHandOrHasInMonstersZone(CardId.AriasLabrnyth) && !HasPerformedPreviously(CardId.AriasLabrnyth))
                            {
                                selected.Add(_cards.FirstOrDefault(x => x.Id == CardId.AriasLabrnyth));
                            }
                        }
                        else
                        {
                            selected.Add(_cards.FirstOrDefault(x => x.Id == CardId.LabrynthCooClock));
                            selected.Add(_cards.FirstOrDefault(x => x.Id == CardId.AriasLabrnyth));
                            selected.Add(_cards.FirstOrDefault(x => x.Id == CardId.LadyLabrnyth));
                            selected.Add(_cards.FirstOrDefault(x => x.Id == CardId.LabrynthStovie));
                            selected.Add(_cards.FirstOrDefault(x => x.Id == CardId.LabrynthChandraglier));
                        }
                    }
                }
                else if (CardId.SimultaneousCannon == currentCard.Id)
                {
                    int[] levels = SimultaneousCannonLevels();
                    if (levels != null)
                    {
                        selected.Add(_cards.FirstOrDefault(x => x.HasType(CardType.Fusion) && x.Level == levels[0]));
                        selected.Add(_cards.FirstOrDefault(x => x.HasType(CardType.Xyz) && x.Level == levels[1]));
                        //selected.Add(_cards.FirstOrDefault(x => x.HasType(CardType.Xyz) && x.Level == levels[1])); maybe don't need due to auto selector?
                    }
                }
                else if (CardId.RunickFlashingFire == currentCard.Id)
                {
                    if (hint == HintMsg.SpSummon)
                        selected.Add(_cards.FirstOrDefault(x => x.Id == CardId.HuginRunick));
                    else
                    {
                        foreach (var target in MONSTER_FIELD_TARGETS)
                            selected.Add(_cards.FirstOrDefault(x => x.Id == target));
                    }
                }
            }
            else
            {

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
                var cardId = Util.GetCardIdFromDesc(options[0]);
                if (cardId == CardId.RunickFlashingFire)
                {
                    if (SpecialRunick())
                        return 1;
                    else
                        return 0;
                }

                // Other scenario 
                var hint = options[0];
                cardId = Util.GetCardIdFromDesc(options[1]);


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
        public bool BackJackActivate()
        {
            return true;
        }

        public bool ChaosAngelSpecial()
        {
            return true;
        }

        public bool MuckrackerSpecial()
        {
            return true;
        }

        public bool AnimaSummon()
        {
            return true;
        }

        public bool YamaSummon()
        {
            return true;
        }

        public bool AnguishSummon()
        {
            return true;
        }

        public bool AbominationSummon()
        {
            return true;
        }

        public bool SPSummon()
        {
            return true;
        }

        public bool PackbitActivate()
        {
            return true;
        }

        public bool BerfometActivate()
        {
            return true;
        }

        public bool AbominationActivate()
        {
            return true;
        }

        public bool SpActivate()
        {
            return true;
        }

        public bool LordPrisonActivate()
        {
            return true;
        }


        #endregion


        #region Generic Spells

        #endregion


        #region Generic Traps
        public bool DarumaActivate()
        {
            if (Duel.Player == 0)
                return false;
            if (Enemy.GetMonsterCount() == 0)
                return false;
            if (Duel.Phase == DuelPhase.Battle || Duel.Phase == DuelPhase.BattleStart)
                return true;
            if (Duel.ChainTargets.Any(x => x.Equals(Card)))
                return true;
            if (Enemy.GetMonsterCount() >= Bot.GetMonsterCount() + 2)
                return true;
            return false;
        }

        public bool PunishmentActivate()
        {
            if (Enemy.GetMonsters().Any(x => MONSTER_FIELD_TARGETS.Any(y => y == x.Id)))
            {
                return true;
            }
            if (Enemy.GetSpells().Any(x => SPELL_FIELD_TARGETS.Any(y => y == x.Id)) &&
                Enemy.GetMonsters().Any(x => x.Attack <= 2500) && Bot.ExtraDeck.ContainsCardWithId(CardId.ElderEntityNtss)

                )
            {
                return true;
            }
            return false;
        }

        public bool TraptrickActivate()
        {
            if (Duel.Player == 0)
                return true;
            else if (Bot.GetSpells().Count(x => x.IsFacedown()) <= 2) // todo count non activatable spell traps
                return true;
            return false;
        }
        #endregion

        #region Labrynth
        public bool GreenActivate()
        {
            return true;
        }

        public bool FurnatureActivate()
        {
            if (Duel.Phase == DuelPhase.End)
                return true;
            if (HasPerformedPreviously(CardId.LabrynthCooClock))
                return true;
            if (Bot.HasInHand(CardId.BackJack))
                return true;
            return false;
        }

        public bool ClockActivate()
        {
            return true;
        }

        public bool LovelyActivate()
        {
            return true;
        }

        public bool LadyActivate()
        {
            return true;
        }


        public bool BigWelcomeActivate()
        {
            if (Enemy.GetMonsters().Any(x => MONSTER_FIELD_TARGETS.Any(y => y == x.Id)))
                return true;
            if (Duel.Player == 0)
                return true;
            if (Duel.Phase == DuelPhase.End)
                return true;
            if (Duel.Phase == DuelPhase.BattleStart)
                return true;
            return false;
        }

        public bool WelcomeActivate()
        {
            return true;
        }

        public bool SetupActivate()
        {
            if (Bot.GetMonsters().Any(x => x.HasRace(CardRace.Fiend) && x.IsFaceup()))
                return true;
            return false;
        }

        public bool SimultaneousCannonActivate()
        {
            if (SimultaneousCannonLevels() != null)
                return true;

            return false;
        }

        // First is fusion, second value is xyz
        public int[] SimultaneousCannonLevels()
        {
            List<int> fusionLevels = new List<int>();
            List<int> xyzPairs = new List<int>();

            List<int> targetLevels = new List<int>();
            int totalCards = Bot.GetFieldHandCount() + Enemy.GetFieldHandCount();

            foreach(var card in Enemy.GetMonsters())
            {
                int level = card.Level;
                if (!targetLevels.Contains(level))
                    targetLevels.Add(level);
            }

            // Find a fusion first
            foreach(var fusionCard in Bot.ExtraDeck)
            {
                if (fusionCard == null)
                    continue;
                if (!fusionCard.HasType(CardType.Fusion))
                    continue;

                int fusionLevel = fusionCard.Level;
                List<int> xyzLevels = new List<int>(); // This holds single xyz leves
                foreach (var xyzCard in Bot.ExtraDeck) // Find 2 xyz now
                {
                    if (xyzCard == null)
                        continue;
                    if (!xyzCard.HasType(CardType.Xyz))
                        continue;
                    if (xyzLevels.Contains(xyzCard.Level))
                    {
                        if (fusionLevel + 2 * xyzCard.Level == totalCards && targetLevels.Contains(fusionLevel + xyzCard.Level))
                            return new int[] { fusionLevel, xyzCard.Level };
                    }
                    else
                        xyzLevels.Add(xyzCard.Level);
                }
            }


            return null;
        }
        #endregion

        public bool FlashingFireActivate()
        {
            if (SpecialRunick())
                return true;
            if (Duel.LastChainPlayer == 0)
                return false;
            if (Enemy.GetMonsters().Any(x => MONSTER_FIELD_TARGETS.Any(y => y == x.Id)))
                return true;
            if (Bot.GetMonstersInExtraZone() != null && Duel.Phase == DuelPhase.End)
                return true;
            return false;
        }

        public bool SpecialRunick()
        {
            if (Duel.LastChainPlayer == 0)
                return false;

            int[] protectAgainst =
            {
                CardId.KnightmarePhoenix,
                CardId.FeatherDuster,
                CardId.LightningStorm,
            };

            if (Duel.CurrentChain.Count >= 1)
                return Duel.CurrentChain.Any(x => protectAgainst.Any(y => y == x.Id));
            return false;
        }
    }
}
