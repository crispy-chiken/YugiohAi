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
    [Deck("Runick", "AI_Runick")]
    public class AIRunick : AIHardCodedBase
    {
        int[] RunickSpells =
        {
            CardId.RunickTip,
            CardId.RunickFreezingCurse,
            CardId.RunickGoldenDroplet,
            CardId.RunickSlumber,
            CardId.RunickSmitingStorm,
            CardId.RunickFlashingFire,
            CardId.RunickDestruction,
            CardId.RunickDispelling
        };
        public AIRunick(GameAI ai, Duel duel)
            : base(ai, duel)
        {
            // Basically First Actions
            AddExecutor(ExecutorType.Activate, CardId.HeatWave);
            // Normal Priority
            AddExecutor(ExecutorType.Activate, CardId.PotOfDuality);
            AddExecutor(ExecutorType.Activate, CardId.DimensonalFissure);
            AddExecutor(ExecutorType.Activate, CardId.UpstartGoblin);
            AddExecutor(ExecutorType.Activate, CardId.TimeTearingMorganite);
            AddExecutor(ExecutorType.Activate, CardId.OneDayOfPeace);
            AddExecutor(ExecutorType.GoToBattlePhase, GoToBattlePhase);
            AddExecutor(ExecutorType.Activate, CardId.EvenlyMatched);
            AddExecutor(ExecutorType.SpSummon, CardId.LavaGolemn);
            AddExecutor(ExecutorType.Summon, CardId.MajestyFiend);
            AddExecutor(ExecutorType.Summon, CardId.AmanoIwato);
            AddExecutor(ExecutorType.Activate, CardId.Terraforming);
            AddExecutor(ExecutorType.Activate, CardId.MessengerOfPeace);
            AddExecutor(ExecutorType.Activate, CardId.InstantFusion, InstantFusionActivate);

            AddExecutor(ExecutorType.Activate, CardId.RunickGoldenDroplet, GoldenDropletActivate);
            AddExecutor(ExecutorType.Activate, CardId.RunickFreezingCurse, FreezingCurseActivate);
            AddExecutor(ExecutorType.Activate, CardId.RunickTip, TipActivate);
            AddExecutor(ExecutorType.Activate, CardId.RunickDispelling, DispellingActivate);
            AddExecutor(ExecutorType.Activate, CardId.RunickSlumber, SlumberActivate);
            AddExecutor(ExecutorType.Activate, CardId.RunickFlashingFire, FlashingFireActivate);
            AddExecutor(ExecutorType.Activate, CardId.RunickSmitingStorm, StormActivate);
            AddExecutor(ExecutorType.Activate, CardId.RunickDestruction, DestructionActivate);
            AddExecutor(ExecutorType.Activate, CardId.DrawMuscle);
            AddExecutor(ExecutorType.Activate, CardId.ChickenGame);
            AddExecutor(ExecutorType.Activate, CardId.CardScanner);
            AddExecutor(ExecutorType.Activate, CardId.RunickFountain, FountainActivate);
            AddExecutor(ExecutorType.Activate, CardId.ThereCanBeOnlyOne);
            AddExecutor(ExecutorType.Activate, CardId.GozenMatch);
            AddExecutor(ExecutorType.Activate, CardId.SynchroZone);
            AddExecutor(ExecutorType.Activate, CardId.SkillDrain);
            AddExecutor(ExecutorType.Activate, CardId.RivalyOfWarlords);
            AddExecutor(ExecutorType.Activate, CardId.TripleTacticsTalent);
            AddExecutor(ExecutorType.Activate, CardId.GraveOfTheSuperAncient);
            AddExecutor(ExecutorType.Activate, CardId.SolemnJudgment, SolemnJudgmentActivate);
            AddExecutor(ExecutorType.Summon, CardId.Bagooska);
            AddExecutor(ExecutorType.Activate, CardId.SleipnirRunick, SleipnirActivate);
            AddExecutor(ExecutorType.Activate, CardId.FrekiRunick);
            AddExecutor(ExecutorType.Activate, CardId.GeriRunick);
            AddExecutor(ExecutorType.Activate, CardId.MuninRunick, MuninActivate);
            AddExecutor(ExecutorType.Activate, CardId.HuginRunick, HuginActivate);
            AddExecutor(ExecutorType.Activate, CardId.InterdimensionalMatterTransolcator);
            AddExecutor(ExecutorType.Activate, CardId.PotOfDesires);
            AddExecutor(ExecutorType.Activate, CardId.CardOfDemise);

            // Low Priority

            // Reactive
            AddExecutor(ExecutorType.Activate, CardId.ForbiddenDroplet, DropletActivate);
            AddExecutor(ExecutorType.Activate, CardId.BattleFader);

            AddExecutor(ExecutorType.SpellSet, SpellSet);

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

            {
                int[] minMain =
                {
                    CardId.Terraforming,
                    CardId.RunickDestruction,
                    CardId.RunickDestruction,
                    CardId.RunickDestruction,
                    CardId.RunickFlashingFire,
                    CardId.RunickFlashingFire,
                    CardId.RunickFlashingFire,
                    CardId.RunickFountain,
                    CardId.RunickFountain,
                    CardId.RunickFreezingCurse,
                    CardId.RunickFreezingCurse,
                    CardId.RunickFreezingCurse,
                    CardId.RunickSlumber,
                    CardId.RunickSlumber,
                    CardId.RunickSlumber,
                    CardId.RunickSmitingStorm,
                    CardId.RunickSmitingStorm,
                    CardId.RunickSmitingStorm,
                    CardId.RunickTip,
                    CardId.RunickTip,
                    CardId.RunickTip,
                    CardId.SkillDrain,
                    CardId.SkillDrain,
                    CardId.SkillDrain,
                    CardId.MessengerOfPeace,
                    CardId.MessengerOfPeace,
                    CardId.PotOfDuality,
                    CardId.PotOfDuality,
                    CardId.PotOfDuality,
                    CardId.PotOfDesires,
                    CardId.TimeTearingMorganite,
                    CardId.CardOfDemise,
                    CardId.CardScanner,
                    CardId.InterdimensionalMatterTransolcator,
                };

                AddCardsToList(_main, pool, mainCount, minMain);
            }

            Archetypes enemyDeck = GetEnemyDeckType();

            switch(enemyDeck)
            {
                case Archetypes.SnakeEyes:
                    if (winResult == 0) // win
                    {
                        int[] toAdd =
                        {
                            CardId.LavaGolemn,
                            CardId.LavaGolemn,
                            CardId.LavaGolemn,
                            CardId.SphereMode,
                            CardId.SphereMode,
                            CardId.SphereMode,
                            CardId.EvenlyMatched,
                            CardId.EvenlyMatched,
                            CardId.EvenlyMatched,
                            CardId.TripleTacticsTalent,
                            CardId.RivalyOfWarlords,
                            CardId.GozenMatch,
                            CardId.DimensonalFissure,
                            CardId.DimensonalFissure,
                            CardId.DimensonalFissure,
                        };
                        AddCardsToList(_main, pool, mainCount, toAdd);
                    }
                    else // lose
                    {
                        int[] toAdd =
                        {
                            CardId.AmanoIwato,
                            CardId.AmanoIwato,
                            CardId.MajestyFiend,
                            CardId.MajestyFiend,
                            CardId.MajestyFiend,
                            CardId.SolemnJudgment,
                            CardId.SolemnJudgment,
                            CardId.SolemnJudgment,
                            CardId.RivalyOfWarlords,
                            CardId.ThereCanBeOnlyOne,
                            CardId.GozenMatch,
                            CardId.DimensonalFissure,
                            CardId.DimensonalFissure,
                            CardId.DimensonalFissure,
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
                if (hint == HintMsg.SpSummon)
                {
                    if (!Bot.HasInHandOrInSpellZone(CardId.RunickFountain))
                        selected.Add(_cards.FirstOrDefault(x => x.Id == CardId.HuginRunick));
                    if (Duel.Phase.HasFlag(DuelPhase.BattleStep))
                    {
                        selected.Add(_cards.FirstOrDefault(x => x.Id == CardId.GeriRunick));
                        selected.Add(_cards.FirstOrDefault(x => x.Id == CardId.FrekiRunick));
                    }
                    selected.Add(_cards.FirstOrDefault(x => x.Id == CardId.SleipnirRunick));
                    selected.Add(_cards.FirstOrDefault(x => x.Id == CardId.MuninRunick));
                }
                else if (CardId.RunickFreezingCurse == currentCard.Id)
                {
                    if (hint == HintMsg.Target)
                    {
                        if (Duel.CurrentChain.Count() >= 2)
                        {
                            if (CardId.InfiniteImpermanence == Card.Id ||
                                CardId.EffectVeiler == Card.Id ||
                                CardId.GhostMourner == Card.Id)
                                selected.Add(_cards.FirstOrDefault(x => Duel.CurrentChain.Any(y => y.Equals(x))));
                        }
                    }
                }
                else if (CardId.RunickTip == currentCard.Id)
                {
                    if (!Bot.HasInHandOrInSpellZone(CardId.RunickFountain))
                        selected.Add(_cards.FirstOrDefault(x => x.Id == CardId.RunickFountain));
                    if (!Bot.HasInHandOrInSpellZone(CardId.RunickFreezingCurse))
                        selected.Add(_cards.FirstOrDefault(x => x.Id == CardId.RunickFreezingCurse));
                    if (!Bot.HasInHandOrInSpellZone(CardId.RunickFlashingFire))
                        selected.Add(_cards.FirstOrDefault(x => x.Id == CardId.RunickFlashingFire));
                    if (!Bot.HasInHandOrInSpellZone(CardId.RunickFountain))
                        selected.Add(_cards.FirstOrDefault(x => x.Id == CardId.RunickFountain));
                    if (!Bot.HasInHandOrInSpellZone(CardId.RunickSlumber))
                        selected.Add(_cards.FirstOrDefault(x => x.Id == CardId.RunickSlumber));
                }
                else if (CardId.RunickDestruction == currentCard.Id)
                {
                    foreach (var target in SPELL_FIELD_TARGETS)
                        selected.Add(_cards.FirstOrDefault(x => x.Id == target));
                }
                else if (CardId.RunickFlashingFire == currentCard.Id)
                {
                    foreach (var target in MONSTER_FIELD_TARGETS)
                        selected.Add(_cards.FirstOrDefault(x => x.Id == target));
                }
                else if (CardId.InterdimensionalMatterTransolcator == currentCard.Id)
                {
                    selected.Add(_cards.FirstOrDefault(x => x.Id == CardId.MajestyFiend));
                    selected.Add(_cards.FirstOrDefault(x => x.Id == CardId.AmanoIwato));
                    foreach (var card in _cards)
                        if (card.Controller == 0)
                            selected.Add(card);
                }
                else if (CardId.PotOfDuality == currentCard.Id)
                {
                    if (!Bot.HasInHandOrInSpellZone(CardId.SolemnJudgment))
                        selected.Add(_cards.FirstOrDefault(x => x.Id == CardId.SolemnJudgment));
                    if (!Bot.HasInHandOrInSpellZone(CardId.SkillDrain))
                        selected.Add(_cards.FirstOrDefault(x => x.Id == CardId.SkillDrain));
                    if (!Bot.HasInHandOrInSpellZone(CardId.GozenMatch))
                        selected.Add(_cards.FirstOrDefault(x => x.Id == CardId.GozenMatch));
                    if (!Bot.HasInHandOrInSpellZone(CardId.RivalyOfWarlords))
                        selected.Add(_cards.FirstOrDefault(x => x.Id == CardId.RivalyOfWarlords));
                    if (!Bot.HasInHandOrInSpellZone(CardId.RunickTip))
                        selected.Add(_cards.FirstOrDefault(x => x.Id == CardId.RunickTip));
                    if (!Bot.HasInHandOrInSpellZone(CardId.RunickFountain))
                        selected.Add(_cards.FirstOrDefault(x => x.Id == CardId.RunickFountain));
                    if (!Bot.HasInHandOrInSpellZone(CardId.RunickFreezingCurse))
                        selected.Add(_cards.FirstOrDefault(x => x.Id == CardId.RunickFreezingCurse));
                    if (!Bot.HasInHandOrInSpellZone(CardId.RunickFlashingFire))
                        selected.Add(_cards.FirstOrDefault(x => x.Id == CardId.RunickFlashingFire));
                    if (!Bot.HasInHandOrInSpellZone(CardId.RunickFountain))
                        selected.Add(_cards.FirstOrDefault(x => x.Id == CardId.RunickFountain));
                    if (!Bot.HasInHandOrInSpellZone(CardId.RunickSlumber))
                        selected.Add(_cards.FirstOrDefault(x => x.Id == CardId.RunickSlumber));
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
            if (Card.Id == CardId.CardScanner)
                return 1;
            foreach(long desc in options)
            {
                var option = Util.GetOptionFromDesc(desc);
                var cardId = Util.GetCardIdFromDesc(desc);
            }

            if (options.Count == 2)
            {
                var cardId = Util.GetCardIdFromDesc(options[0]);
                if (RunickSpells.Any(x => x == cardId))
                {
                    if (SpecialRunick() && cardId != CardId.RunickTip)
                        return 1;
                    else
                        return 0;
                }


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
        public bool SpellSet()
        {
            if (RunickSpells.Any(x => x == Card.Id) && Bot.GetFieldSpellCard()?.Id == CardId.RunickFountain)
                return false;

            return (Card.IsTrap() || Card.HasType(CardType.QuickPlay) || DefaultSpellMustSetFirst()) && Bot.GetSpellCountWithoutField() < 4;
        }
        #endregion


        #region Generic Monsters



        #endregion


        #region Generic Spells
        public bool InstantFusionActivate()
        {
            return true;
        }
        #endregion


        #region Generic Traps
        public bool SolemnJudgmentActivate()
        {
            return DefaultSolemnJudgment();
        }
        #endregion

        #region Runick
        public bool GoldenDropletActivate()
        {
            if (SpecialRunick())
                return true;
            if (Enemy.Deck.Count < 15)
                return true;
            return false;
        }

        public bool FreezingCurseActivate()
        {
            if (SpecialRunick())
                return true;
            if (FaceUpEffectNegate())
                return true;
            if (Bot.GetMonstersInExtraZone() != null && Duel.Phase == DuelPhase.End)
                return true;
            return false;
        }

        public bool TipActivate()
        {
            if (SpecialRunick())
                return true;
            if (Duel.LastChainPlayer == 0)
                return false;
            return true;
        }

        public bool DispellingActivate()
        {
            if (SpecialRunick())
                return true;
            if (Bot.GetMonstersInExtraZone() != null)
                return true;
            return false;
        }

        public bool SlumberActivate()
        {
            if (SpecialRunick())
                return true;
            if (Duel.Player == 1 && Duel.Phase.HasFlag(DuelPhase.BattleStep))
                return true;
            if (Bot.GetMonstersInExtraZone() != null && Duel.Phase == DuelPhase.End)
                return true;
            return false;
        }

        public bool FlashingFireActivate()
        {
            if (SpecialRunick())
                return true;
            if (Enemy.GetMonsters().Any(x => MONSTER_FIELD_TARGETS.Any(y => y == x.Id)))
                return true;
            if (Bot.GetMonstersInExtraZone() != null && Duel.Phase == DuelPhase.End)
                return true;
            return false;
        }

        public bool StormActivate()
        {
            if (SpecialRunick())
                return true;
            if (Enemy.GetFieldCount() >= 4)
                return true;
            return false;
        }

        public bool DestructionActivate()
        {
            if (SpecialRunick())
                return true;
            if (Enemy.GetSpells().Any(x => SPELL_FIELD_TARGETS.Any(y => y == x.Id)))
                return true;
            if (CosmicActivate())
                return true;
            if (Bot.GetMonstersInExtraZone() != null && Duel.Phase == DuelPhase.End)
                return true;
            return false;
        }

        public bool FountainActivate()
        {
            if (!Bot.HasInHand(RunickSpells))
                return true;
            if (Bot.GetGraveyardSpells().Count(x => RunickSpells.Any(y => y == x.Id)) >= 2)
                return true;
            return false;
        }

        public bool SleipnirActivate()
        {
            return true;
        }

        public bool MuninActivate()
        {
            return true;
        }

        public bool HuginActivate()
        {
            return true;
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

            if (Duel.CurrentChain.Count >= 1 && Duel.CurrentChain.Any(x => x.Controller == 1 && protectAgainst.Any(y => y == x.Id)))
                return true;
            if (!Bot.HasInHandOrInSpellZone(CardId.RunickFountain))
                return true;
            if (Duel.Phase.HasFlag(DuelPhase.BattleStep) && Bot.GetMonstersInExtraZone() == null)
                return true;

            return false;
        }
        #endregion
    }
}
