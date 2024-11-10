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
    [Deck("SnakeEyes", "AI_SnakeEyes")]
    public class AISnakeEyes : AIHardCodedBase
    {
        int[] SNAKE_EYE_SEND_SPELL_COST = {
                CardId.SkillDrain,
                CardId.AntiSpellFragrance,
                CardId.SnakeEyeDiabellstar,
                CardId.DiabellstarBlackWitch,
                CardId.SnakeEyePoplar,
                CardId.SnakeEyeOak,
                CardId.SnakeEyeAsh,
                CardId.DivineTempleSnakeEyes,
                CardId.WANTEDSinfulSpoils,
         };

        int[] SNAKE_EYES_STARTER =
{
                CardId.SnakeEyeAsh,
                CardId.DiabellstarBlackWitch,
                CardId.WANTEDSinfulSpoils,
                CardId.OriginalSinfulSpoilsSnakeEyes,
                CardId.SnakeEyePoplar
        };

        public AISnakeEyes(GameAI ai, Duel duel)
            : base(ai, duel)
        {
            // Basically First Actions
            AddExecutor(ExecutorType.Activate, CardId.WANTEDSinfulSpoils);
            AddExecutor(ExecutorType.Activate, CardId.LightningStorm);
            AddExecutor(ExecutorType.Activate, CardId.SkillDrain);
            AddExecutor(ExecutorType.Activate, CardId.AntiSpellFragrance);
            AddExecutor(ExecutorType.Activate, CardId.DarkRulerNoMore);
            AddExecutor(ExecutorType.Activate, CardId.FeatherDuster);
            AddExecutor(ExecutorType.GoToBattlePhase, GoToBattlePhase);
            AddExecutor(ExecutorType.Activate, CardId.EvenlyMatched);
            AddExecutor(ExecutorType.Activate, CardId.FiendsmithTractus, FiendsmithTractusActivate);
            AddExecutor(ExecutorType.Activate, CardId.Bonfire);

            // Normal Priority
            AddExecutor(ExecutorType.Activate, CardId.TripleTacticsTalent);
            AddExecutor(ExecutorType.SpSummon, CardId.MoonOfTheClosedHeaven, ClosedMoonSpecial);
            AddExecutor(ExecutorType.Activate, CardId.FiendsmithSequentia, FiendsmithSequentiaActivate);
            AddExecutor(ExecutorType.Activate, CardId.FiendsmithRequiem, FiendsmithRequiemActivate);
            AddExecutor(ExecutorType.Activate, CardId.TheFiendsmith, TheFiendsmithActivate);
            AddExecutor(ExecutorType.SpSummon, CardId.NecroqiopPrincess);
            AddExecutor(ExecutorType.Activate, CardId.BeatriceLadyOfEnternal);

            AddExecutor(ExecutorType.SpSummon, CardId.BeatriceLadyOfEnternal);
            AddExecutor(ExecutorType.SpSummon, CardId.FiendsmithRequiem, FiendSmithRequiemSummon);
            AddExecutor(ExecutorType.SpSummon, CardId.FiendsmithSequentia, FiendsmithSequentiaSummon);
            AddExecutor(ExecutorType.Activate, CardId.FiendsmithLacrimosa, FiendsmithLacrimosaActivate);
            AddExecutor(ExecutorType.Activate, CardId.FiendsmithDiesIrae, FiendsmithDiesIraeActivate);

            AddExecutor(ExecutorType.Activate, CardId.DivineTempleSnakeEyes);
            AddExecutor(ExecutorType.Summon, CardId.SnakeEyeAsh, SnakeEyeAshSummon);
            AddExecutor(ExecutorType.Activate, CardId.SnakeEyeAsh, SnakeEyeAshActivate);
            AddExecutor(ExecutorType.Activate, CardId.SnakeEyeFlamberge, SnakeEyeFlambergeActivate);
            AddExecutor(ExecutorType.Activate, CardId.SnakeEyePoplar);
            AddExecutor(ExecutorType.SpSummon, CardId.DiabellstarBlackWitch, DiabellstarBlackWitchSPSummon);
            AddExecutor(ExecutorType.Summon, CardId.SnakeEyeOak, SnakeEyeOakSummon);
            AddExecutor(ExecutorType.Activate, CardId.SnakeEyeOak, SnakeEyeOakActivate);
            AddExecutor(ExecutorType.SpSummon, CardId.IPMasquerena, IPSummon);
            AddExecutor(ExecutorType.SpSummon, CardId.PromethianPrincess, PromethianPrincessSummon);
            AddExecutor(ExecutorType.Activate, CardId.DiabellstarBlackWitch);
            AddExecutor(ExecutorType.Activate, CardId.PromethianPrincess, PromethianPrincessActivate);
            AddExecutor(ExecutorType.SpSummon, CardId.SalamangreatRagingPhoenix, RagingPhoenixSummon);
            AddExecutor(ExecutorType.SpSummon, CardId.HiitaCharmerAblaze, HiitaSummon);
            AddExecutor(ExecutorType.SpSummon, CardId.DharcCharmerGloomy, DharcSummon);
            AddExecutor(ExecutorType.Activate, CardId.HiitaCharmerAblaze);
            AddExecutor(ExecutorType.Activate, CardId.DharcCharmerGloomy);
            AddExecutor(ExecutorType.Activate, CardId.SnakeEyeDiabellstar, SnakeEyeDiabellstarActivate);
            AddExecutor(ExecutorType.SpSummon, CardId.SeleneQueenofMasterMagicians, SeleneMagicianSummon);
            AddExecutor(ExecutorType.SpSummon, CardId.AccesscodeTalker, AccessCodeSummon);
            AddExecutor(ExecutorType.Activate, CardId.AccesscodeTalker, AccessCodeActivate);
            AddExecutor(ExecutorType.SpSummon, CardId.Apollusa, ApollusaSummon);

            AddExecutor(ExecutorType.SpSummon, CardId.RelinquishdAnima, AnimaSummon);
            AddExecutor(ExecutorType.Activate, CardId.OriginalSinfulSpoilsSnakeEyes, OSSSEActivate);
            AddExecutor(ExecutorType.Activate, CardId.OneForOne, OneforOneActivate);
            AddExecutor(ExecutorType.SpSummon, CardId.LinkSpider, LinkSpiderSummon);

            AddExecutor(ExecutorType.Activate, CardId.SalamangreatRagingPhoenix);
            AddExecutor(ExecutorType.SpSummon, CardId.KnightmarePhoenix, KnightmarePhoenixSummon);
            AddExecutor(ExecutorType.Activate, CardId.KnightmarePhoenix);
  


            AddExecutor(ExecutorType.SpSummon, CardId.SPLittleKnight, SPSummon);
            AddExecutor(ExecutorType.Activate, CardId.IPMasquerena, IPActivate);
            AddExecutor(ExecutorType.Activate, CardId.SPLittleKnight, SPActivate);
            AddExecutor(ExecutorType.SpSummon, CardId.WorldseadragonZealantis, ZealantisSummon);
            AddExecutor(ExecutorType.Activate, CardId.WorldseadragonZealantis);
            AddExecutor(ExecutorType.Activate, CardId.SeleneQueenofMasterMagicians);
            AddExecutor(ExecutorType.Activate, CardId.Apollusa, ApollusaActivate);
            AddExecutor(ExecutorType.SpSummon, CardId.UnderworldGoddess, UnderworldGoddessSummon);
            AddExecutor(ExecutorType.Activate, CardId.UnderworldGoddess);

            // Low Priority
            AddExecutor(ExecutorType.Summon, CardId.SnakeEyePoplar);
            AddExecutor(ExecutorType.SpSummon, CardId.TyphonSkyCrisis, TyphonSummon);
            AddExecutor(ExecutorType.Activate, CardId.TyphonSkyCrisis, TyphonActivate);

            // Reactive
            AddExecutor(ExecutorType.Activate, CardId.CrossoutDesignator, CrossoutActivate);
            AddExecutor(ExecutorType.Activate, CardId.CalledByTheGrave, CalledByActivate);

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
                CardId.SnakeEyeFlamberge,
                CardId.SnakeEyeDiabellstar,
                CardId.DiabellstarBlackWitch,
                CardId.SnakeEyeOak,
                CardId.SnakeEyeAsh,
                CardId.SnakeEyeAsh,
                CardId.SnakeEyeAsh,
                CardId.SnakeEyePoplar,
                CardId.SnakeEyePoplar,
                CardId.Bonfire,
                CardId.Bonfire,
                CardId.Bonfire,
                CardId.OriginalSinfulSpoilsSnakeEyes,
                CardId.WANTEDSinfulSpoils,
                CardId.WANTEDSinfulSpoils,
                CardId.WANTEDSinfulSpoils,
                CardId.DivineTempleSnakeEyes,
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
                            CardId.AshBlossom,
                            CardId.AshBlossom,
                            CardId.AshBlossom,
                            CardId.GhostMourner,
                            CardId.GhostMourner,
                            CardId.GhostMourner,
                            CardId.EffectVeiler,
                            CardId.EffectVeiler,
                            CardId.EffectVeiler,
                            CardId.InfiniteImpermanence,
                            CardId.InfiniteImpermanence,
                            CardId.InfiniteImpermanence,
                            CardId.Nibiru,
                            CardId.Nibiru,
                            CardId.Nibiru,
                            CardId.CrossoutDesignator,
                            CardId.CrossoutDesignator,
                            CardId.CrossoutDesignator,
                            CardId.OneForOne,
                            CardId.TripleTacticsTalent,
                            CardId.TripleTacticsTalent,
                            CardId.TripleTacticsTalent,
                        };
                        AddCardsToList(_main, pool, mainCount, toAdd);
                    }
                    else // lose
                    {
                        int[] toAdd =
{
                            CardId.AshBlossom,
                            CardId.AshBlossom,
                            CardId.AshBlossom,
                            CardId.GhostMourner,
                            CardId.EffectVeiler,
                            CardId.EffectVeiler,
                            CardId.EffectVeiler,
                            CardId.InfiniteImpermanence,
                            CardId.InfiniteImpermanence,
                            CardId.InfiniteImpermanence,
                            CardId.Nibiru,
                            CardId.Nibiru,
                            CardId.Nibiru,
                            CardId.CrossoutDesignator,
                            CardId.CrossoutDesignator,
                            CardId.CrossoutDesignator,
                            CardId.AntiSpellFragrance,
                            CardId.SkillDrain,
                            CardId.SkillDrain,
                            CardId.SkillDrain,
                            CardId.TripleTacticsTalent,
                        };
                        AddCardsToList(_main, pool, mainCount, toAdd);
                    }
                    break;
                case Archetypes.Tenpai:
                    if (winResult == 0) // win
                    {
                        int[] toAdd =
                        {
                            CardId.AshBlossom,
                            CardId.AshBlossom,
                            CardId.AshBlossom,
                            CardId.EffectVeiler,
                            CardId.EffectVeiler,
                            CardId.EffectVeiler,
                            CardId.InfiniteImpermanence,
                            CardId.InfiniteImpermanence,
                            CardId.InfiniteImpermanence,
                            CardId.DrollnLockBird,
                            CardId.DrollnLockBird,
                            CardId.DrollnLockBird,
                            CardId.CalledByTheGrave,
                            CardId.AntiSpellFragrance,
                            CardId.SkillDrain,
                            CardId.SkillDrain,
                            CardId.SkillDrain,
                            CardId.TripleTacticsTalent,
                            CardId.TripleTacticsTalent,
                            CardId.TripleTacticsTalent,
                            CardId.GhostMourner,
                            CardId.GhostMourner,
                            CardId.GhostMourner,
                        };
                        AddCardsToList(_main, pool, mainCount, toAdd);
                    }
                    else // lose
                    {
                        int[] toAdd =
                        {
                            CardId.AshBlossom,
                            CardId.AshBlossom,
                            CardId.AshBlossom,
                            CardId.EffectVeiler,
                            CardId.EffectVeiler,
                            CardId.EffectVeiler,
                            CardId.InfiniteImpermanence,
                            CardId.InfiniteImpermanence,
                            CardId.InfiniteImpermanence,
                            CardId.DrollnLockBird,
                            CardId.DrollnLockBird,
                            CardId.DrollnLockBird,
                            CardId.CalledByTheGrave,
                            CardId.AntiSpellFragrance,
                            CardId.SkillDrain,
                            CardId.SkillDrain,
                            CardId.SkillDrain,
                            CardId.TripleTacticsTalent,
                            CardId.TripleTacticsTalent,
                            CardId.TripleTacticsTalent,
                            CardId.GhostMourner,
                            CardId.GhostMourner,
                            CardId.GhostMourner,
                        };
                        AddCardsToList(_main, pool, mainCount, toAdd);
                    }
                    break;
                case Archetypes.Labrynth:
                    {
                        int[] toAdd =
                        {
                            CardId.BystialBaldrake,
                            CardId.BystialDruiswurm,
                            CardId.BystialMagnamhut,
                            CardId.BystialSaronir,
                            CardId.CalledByTheGrave,
                            CardId.FeatherDuster,
                            CardId.CosmicCyclone,
                            CardId.CosmicCyclone,
                            CardId.CosmicCyclone,
                            CardId.EvenlyMatched,
                            CardId.EvenlyMatched,
                            CardId.EvenlyMatched,
                            CardId.LightningStorm,
                            CardId.LightningStorm,
                            CardId.LightningStorm,
                            CardId.AshBlossom,
                            CardId.AshBlossom,
                            CardId.AshBlossom,
                            CardId.EffectVeiler,
                            CardId.EffectVeiler,
                            CardId.EffectVeiler,
                            CardId.InfiniteImpermanence,
                            CardId.InfiniteImpermanence,
                            CardId.InfiniteImpermanence,
                            CardId.TripleTacticsTalent,
                            CardId.TripleTacticsTalent,
                            CardId.TripleTacticsTalent,
                        };
                        AddCardsToList(_main, pool, mainCount, toAdd);
                    }
                    break;
                case Archetypes.Runick:
                    {
                        int[] toAdd =
                        {
                            CardId.FeatherDuster,
                            CardId.CosmicCyclone,
                            CardId.CosmicCyclone,
                            CardId.CosmicCyclone,
                            CardId.EvenlyMatched,
                            CardId.EvenlyMatched,
                            CardId.EvenlyMatched,
                            CardId.LightningStorm,
                            CardId.LightningStorm,
                            CardId.LightningStorm,
                            CardId.AshBlossom,
                            CardId.AshBlossom,
                            CardId.AshBlossom,
                            CardId.InfiniteImpermanence,
                            CardId.InfiniteImpermanence,
                            CardId.InfiniteImpermanence,
                            CardId.CrossoutDesignator,
                            CardId.CrossoutDesignator,
                            CardId.CrossoutDesignator,
                            CardId.SkillDrain,
                            CardId.SkillDrain,
                            CardId.EffectVeiler,
                            CardId.EffectVeiler,
                            CardId.EffectVeiler,
                            CardId.CalledByTheGrave,
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
            return true;
        }

        public override IList<ClientCard> OnSelectCard(IList<ClientCard> _cards, int min, int max, long hint, bool cancelable)
        {
            //if (Duel.Phase == DuelPhase.BattleStart)
            //    return null;
            if (AI.HaveSelectedCards())
                return null;

            ClientCard currentCard = GetCurrentCardResolveInChain();
            IList<ClientCard> selected = new List<ClientCard>();

            #region AI Selected
            if (currentCard != null)
            {
                if (CardId.DivineTempleSnakeEyes == currentCard.Id)
                {
                    if (hint == HintMsg.SpSummon)
                    {
                        selected.Add(_cards.Where(x => x.Id == CardId.IPMasquerena).FirstOrDefault());
                        selected.Add(_cards.Where(x => x.Id == CardId.PromethianPrincess).FirstOrDefault());
                        selected.Add(_cards.Where(x => x.Id == CardId.SnakeEyeFlamberge).FirstOrDefault());
                    }
                }
                else if (CardId.BeatriceLadyOfEnternal == currentCard.Id)
                {
                    if (GetEnemyDeckType() == Archetypes.Tenpai)
                        selected.Add(_cards.Where(x => x.Id == CardId.RiseToFullHeight).FirstOrDefault());
                    if (!HasCombo())
                        selected.Add(_cards.Where(x => x.Id == CardId.SnakeEyeAsh).FirstOrDefault());
                    selected.Add(_cards.Where(x => x.Id == CardId.BlackGoat).FirstOrDefault());
                }
                else if (CardId.PromethianPrincess == currentCard.Id)
                {
                    selected.Add(_cards.Where(x => x.Controller == 0 && x.Id == CardId.SnakeEyeFlamberge).FirstOrDefault());
                    selected.Add(_cards.Where(x => x.Controller == 1 && !DONT_DESTROY.Any(y => y == x.Id)).FirstOrDefault());
                }
                else if (CardId.IPMasquerena == currentCard.Id)
                {
                    selected.Add(_cards.Where(x => x.Id == CardId.SPLittleKnight).FirstOrDefault());
                }
            }
            if (hint == HintMsg.AddToHand)
            {
                if (_cards.ContainsCardWithId(CardId.SnakeEyeAsh) && !HasPerformedPreviously(CardId.SnakeEyeAsh, ExecutorType.Summon))
                    selected.Add(_cards.Where(x => x.Id == CardId.SnakeEyeAsh).FirstOrDefault());

                if (_cards.ContainsCardWithId(CardId.SnakeEyePoplar) && !HasPerformedPreviously(CardId.SnakeEyePoplar, 0))
                    selected.Add(_cards.Where(x => x.Id == CardId.SnakeEyePoplar).FirstOrDefault());

                selected.Add(_cards.Where(x => x.Id == CardId.DivineTempleSnakeEyes).FirstOrDefault());
                selected.Add(_cards.Where(x => x.Id == CardId.OriginalSinfulSpoilsSnakeEyes).FirstOrDefault());
                selected.Add(_cards.Where(x => x.Id == CardId.DiabellstarBlackWitch).FirstOrDefault());

            }
            else if (hint == HintMsg.ToField) // Place in spell and trap zone?
            {
                selected.Add(_cards.Where(x => x.Id == CardId.SnakeEyeDiabellstar).FirstOrDefault());
            }
            else if (hint == HintMsg.Set)
            {
                selected.Add(_cards.Where(x => x.Id == CardId.OriginalSinfulSpoilsSnakeEyes).FirstOrDefault());
            }
            else if (hint == HintMsg.ToGrave) // Send for cost?
            {
                foreach (int id in SNAKE_EYE_SEND_SPELL_COST)
                    if (_cards.ContainsCardWithId(id))
                        selected.Add(_cards.Where(x => x.Id == id && x.Location == CardLocation.SpellZone).FirstOrDefault());

                // Send from spell zone then hand first
                _cards = _cards
                    .OrderBy(x => x.Location == CardLocation.SpellZone ? 0 : 1)
                    .OrderBy(x => x.Location == CardLocation.Hand ? 0 : 1)
                    .ThenBy(x => x.Id == CardId.DiabellstarBlackWitch ? 0 : 1)
                    .ThenBy(x => x.Id == CardId.SnakeEyeAsh ? 0 : 1)
                    .ThenBy(x => x.Id == CardId.SnakeEyePoplar ? 0 : 1)
                    .ToList();
            }
            else if (hint == HintMsg.SpSummon) // From anywhere
            {
                // Summon flamberge from gy first
                 selected.Add(_cards.Where(x => x.Id == CardId.SnakeEyeFlamberge && x.Location == CardLocation.Grave).FirstOrDefault());

                if (_cards.Where(x => x.Id == CardId.SnakeEyeOak).Any() && !Bot.HasInSpellZone(CardId.OriginalSinfulSpoilsSnakeEyes))
                    selected.Add(_cards.Where(x => x.Id == CardId.SnakeEyeOak).FirstOrDefault());

                if (_cards.ContainsCardWithId(CardId.SnakeEyeFlamberge))
                {
                    // Get the ones from deck first
                    foreach(ClientCard card in _cards.Where(x => x.Id == CardId.SnakeEyeFlamberge).OrderBy(x => x.Location == CardLocation.Deck ? 0 : 1).ToList())
                        selected.Add(card);
                }

                selected.Add(_cards.Where(x => x.Id == CardId.SnakeEyeAsh).FirstOrDefault());
                selected.Add(_cards.Where(x => x.Id == CardId.SnakeEyeOak).FirstOrDefault());

            }
            else if (hint == HintMsg.Target)
            {
                if (CardId.SnakeEyeOak == Duel.CurrentChain.LastOrDefault().Id)
                {
                    if (_cards.ContainsCardWithId(CardId.SnakeEyeAsh) && !HasPerformedPreviously(CardId.SnakeEyeAsh, ExecutorType.Activate))
                        selected.Add(_cards.Where(x => x.Id == CardId.SnakeEyeAsh).FirstOrDefault());

                    if (_cards.ContainsCardWithId(CardId.SnakeEyePoplar) && !HasPerformedPreviously(CardId.SnakeEyePoplar, 0))
                        selected.Add(_cards.Where(x => x.Id == CardId.SnakeEyePoplar).FirstOrDefault());

                    selected.Add(_cards.Where(x => x.Id == CardId.SnakeEyeAsh).FirstOrDefault());
                }
                else if (CardId.SnakeEyePoplar == Duel.CurrentChain.LastOrDefault().Id)
                {
                    if (_cards.ContainsCardWithId(CardId.SnakeEyeDiabellstar) && !HasPerformedPreviously(CardId.SnakeEyeDiabellstar, ExecutorType.Activate))
                        selected.Add(_cards.Where(x => x.Id == CardId.SnakeEyeDiabellstar).FirstOrDefault());

                    if (Bot.GetGraveyardMonsters().Where(x => x.Level == 1 && x.HasAttribute(CardAttribute.Fire)).Count() > 2)
                    {
                        if (_cards.ContainsCardWithId(CardId.SnakeEyeFlamberge))
                            selected.Add(_cards.Where(x => x.Id == CardId.SnakeEyeFlamberge).FirstOrDefault());

                        if (_cards.ContainsCardWithId(CardId.SnakeEyePoplar))
                            selected.Add(_cards.Where(x => x.Id == CardId.SnakeEyePoplar).FirstOrDefault());

                        if (_cards.ContainsCardWithId(CardId.SnakeEyeOak))
                            selected.Add(_cards.Where(x => x.Id == CardId.SnakeEyeOak).FirstOrDefault());

                        if (_cards.ContainsCardWithId(CardId.SnakeEyeAsh))
                            selected.Add(_cards.Where(x => x.Id == CardId.SnakeEyeAsh).FirstOrDefault());
                    }
                }
                else if (CardId.SnakeEyeDiabellstar == Duel.CurrentChain.LastOrDefault().Id)
                {
                    if (_cards.ContainsCardWithId(CardId.SnakeEyeOak))
                        selected.Add(_cards.Where(x => x.Id == CardId.SnakeEyeOak).FirstOrDefault());

                    if (_cards.ContainsCardWithId(CardId.SnakeEyeAsh))
                        selected.Add(_cards.Where(x => x.Id == CardId.SnakeEyeAsh).FirstOrDefault());

                    if (_cards.ContainsCardWithId(CardId.SnakeEyePoplar))
                        selected.Add(_cards.Where(x => x.Id == CardId.SnakeEyePoplar).FirstOrDefault());

                    if (_cards.ContainsCardWithId(CardId.SnakeEyeFlamberge))
                        selected.Add(_cards.Where(x => x.Id == CardId.SnakeEyeFlamberge).FirstOrDefault());
                }
                else if (CardId.SnakeEyeFlamberge == Duel.CurrentChain.LastOrDefault().Id)
                {
                    if (Duel.Player == 0)
                    {
                        if (Duel.Turn != 1)// Non first turn actions
                        {
                            foreach (int id in MONSTER_FIELD_TARGETS)
                                selected.Add(_cards.Where(x => x.Id == id && x.Owner == 1).FirstOrDefault());
                        }

                        // Other targets to place
                        selected.Add(_cards.Where(x => x.Id == CardId.IPMasquerena && x.Owner == 0 && x.Location == CardLocation.Grave).FirstOrDefault());
                    }
                    else
                    {
                        selected.Add(_cards.Where(x => x.Id == CardId.IPMasquerena).FirstOrDefault());
                        selected.Add(_cards.Where(x => x.Id == CardId.SnakeEyeOak).FirstOrDefault());
                        selected.Add(_cards.Where(x => x.Id == CardId.SnakeEyeAsh).FirstOrDefault());
                    }
                }
                else if (CardId.KnightmarePhoenix == currentCard.Id)
                {
                    foreach (var target in SPELL_FIELD_TARGETS)
                        selected.Add(_cards.FirstOrDefault(x => x.Id == target));
                }
                
            }
            else if (hint == HintMsg.LinkMaterial)
            {
                IList<ClientCard> highPriority = new List<ClientCard>();
                IList<ClientCard> lowPriority = new List<ClientCard>();

                int[] highList =
                {
                    CardId.HiitaCharmerAblaze,
                    CardId.SnakeEyePoplar
                };
                int[] lowList =
                {
                    CardId.SnakeEyeFlamberge,
                };

                _cards = _cards
                    .OrderBy(x => highList.Contains(x.Id) ? 0 : 1)
                    .ThenBy(x => x.HasType(CardType.Link) ? -x.LinkCount: 0) // Use the highest link monster first
                    .ThenBy(x => lowList.Contains(x.Id) ? 1 : 0)
                    .ToList();

                selected.Add(_cards[0]);

                // Stop selecting if using low priorty cards
                if (materialSelected > 0 && cancelable && _cards.Where(x => lowList.Contains(x.Id)).Count() == _cards.Count())
                    return null;

                materialSelected += 1;
            }
            else if (hint == HintMsg.Remove)
            {
                if (CardId.AccesscodeTalker == Card.Id)
                {
                    // Grave yard first
                    _cards = _cards.OrderBy(x => x.Location == CardLocation.Grave ? 0 : 1).ToArray();
                }
            }
            else if (hint == HintMsg.Destroy)
            {
                _cards = _cards
                            .OrderBy(x => x.Controller == 1 ? 0 : 1) // Enemies first
                            .ThenBy(x => x.Location == CardLocation.MonsterZone ? 0 : 1)
                            .ToArray();
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

                if (hint == HintMsg.GYToHand)
                {
                    if (cardId == CardId.SnakeEyeOak)
                        return 1; // 0 = to hand, 1 = ss to field
                }
            }

            return base.OnSelectOption(options);
        }

        public override int OnAnnounceCard(IList<int> avail)
        {
            return base.OnAnnounceCard(avail);
        }


        #region Generic Actions

        #endregion


        #region Generic Monsters

        public bool IPSummon()
        {
            if (Duel.Turn == 1)
            {
                if (Bot.HasInMonstersZone(CardId.SnakeEyeFlamberge) && !HasPerformedPreviously(CardId.SnakeEyeFlamberge, 2))
                    return true;
            }
            return false;
        }

        public bool IPActivate()
        {
            return SPActivate();
        }

        public bool PromethianPrincessSummon()
        {
            if (!Bot.GetMonsters().Any(x => x.HasAttribute(CardAttribute.Fire)) && !Bot.GetGraveyardMonsters().Any(x => x.HasAttribute(CardAttribute.Fire)))
                return false;
            if (Duel.Turn == 1)
            {
                if (Bot.HasInGraveyard(CardId.DiabellstarBlackWitch) && Bot.GetLinkMaterialWorth() >= 5)
                    return true;
                if (Bot.GetLinkMaterialWorth() >= 4)
                    return true;
                if (Bot.GetLinkMaterialWorth() == 3 && Bot.GetMonsterCount() == 2)
                    return true;
                if (Bot.HasInGraveyard(CardId.SnakeEyeAsh) && !HasPerformedPreviously(CardId.SnakeEyeAsh))
                    return true;

                return false;
            }
            return true;
        }

        public bool PromethianPrincessActivate()
        {
            if (Card.Location == CardLocation.Grave)
            {
                if (Enemy.GetMonsters().Where(x => !DONT_DESTROY.Contains(x.Id)).Any())
                    return true;

                return false;
            }

            return true; // Always activate on field
        }

        public bool HiitaSummon()
        {
            if (Bot.GetLinkMaterialWorth(dontUseAsMaterial) < 2)
                return false;
            if (Duel.Turn == 1)
            {
                if (Bot.HasInMonsterZoneSpellZoneOrGraveyard(CardId.SnakeEyeDiabellstar) && Bot.GetLinkMaterialWorth() >= 5)
                    return true;
                return false;
            }
            if (Enemy.GetGraveyardMonsters().Where(x => x.HasAttribute(CardAttribute.Fire)).Any())
                return true;

            return false;
        }

        public bool DharcSummon()
        {
            if (Bot.GetLinkMaterialWorth(dontUseAsMaterial) < 2)
                return false;
            if (Enemy.GetGraveyardMonsters().Where(x => x.HasAttribute(CardAttribute.Dark)).Any())
                return true;

            return false;
        }

        public bool SeleneMagicianSummon()
        {
            int spellCount = 0;
            spellCount += Bot.GetGraveyardSpells().Count;
            spellCount += Bot.GetSpellCount(); // might be off due to facedown cards
            spellCount += Enemy.GetGraveyardSpells().Count;
            spellCount += Enemy.GetSpellCount(); // might be off due to facedown cards

            if (spellCount < 3)
                return false;

            if (Bot.GetMonsters().Any(x => x?.HasRace(CardRace.SpellCaster) ?? false))
                return true;
            return false;
        }

        public bool AnimaSummon()
        {
            if (Bot.GetMonsters().ContainsCardWithId(CardId.SnakeEyePoplar))
                return true;
            if (Bot.GetMonsterCount() == 2)
                return true;
            return false;
        }

        public bool RagingPhoenixSummon()
        {
            int[] dontuse = (int[])dontUseAsMaterial.Clone();
            int[] alsodontuse = { CardId.SnakeEyeFlamberge, CardId.NecroqiopPrincess };
            dontuse = dontUseAsMaterial.Union(alsodontuse).ToArray();

            if (Bot.GetLinkMaterialWorth(dontuse) >= 4)
                return true;
            if (Duel.Turn > 1 && Bot.GetLinkMaterialWorth() >= 5)
                return true;
            return false;
        }

        public bool SPSummon()
        {
            if (Duel.Turn == 1)
            {
                if (Enemy.HasInMonstersZone(CardId.Nibiru) && Bot.GetMonsters().Where(x => x.IsExtraCard()).Any())
                    return true;
            }
            else if (Bot.GetFieldCount() == 2 && Bot.GetMonsters().Where(x => x.IsExtraCard()).Any())
                return true;
            return false;
        }

        public bool SPActivate()
        {
            return true;
        }

        public bool AccessCodeSummon()
        {
            if (Duel.Turn == 1)
                return false;
            if (Duel.Player == 1)
                return false;
            if (Enemy.GetMonsterCount() == 0)
                return false;


            if (Bot.GetMonsters().Where(x => x.HasType(CardType.Link) && x.LinkCount >= 2).Any())
                return true;

            return false;
        }

        public bool AccessCodeActivate()
        {
            if (Util.GetOptionFromDesc(ActivateDescription) == 0)
                return true;

            if (Bot.GetGraveyardMonsters().Where(x => x.HasType(CardType.Link)).Any())
                return true;

            return false;
        }

        public bool KnightmarePhoenixSummon()
        {
            if (Enemy.GetSpells().Any(x => SPELL_FIELD_TARGETS.Any(y => y == x.Id)))
                return true;
            return false;
        }

        public bool ZealantisSummon()
        {
            if (Bot.GetMonsters().ContainsCardWithId(CardId.SalamangreatRagingPhoenix))
            {
                // Set up zelantis otk
                if (Enemy.GetMonsterCount() > 0)
                {
                    if (Bot.GetMonsters().Count() >= 2)
                        return true;
                }
                else if (Bot.GetMonsters().Count() >= 3)
                {
                    if (Duel.Turn > 1)
                    {
                        // OTK with phoenix
                        return true;
                    }
                }
            }
            else if (Bot.GetMonsters().Where(x => x.Id == CardId.Apollusa && x.Attack < 1600).Any())
                return true;

            return false;
        }

        public bool UnderworldGoddessSummon()
        {
            return false;
        }

        public bool LinkSpiderSummon()
        {
            if (Bot.GetFieldCount() > 1)
                return true;
            return false;
        }

        public bool ClosedMoonSpecial()
        {
            if (!Bot.HasInHandOrInSpellZone(CardId.OriginalSinfulSpoilsSnakeEyes) || !Bot.HasInSpellZone(CardId.SnakeEyeDiabellstar) || Bot.GetLinkMaterialWorth(dontUseAsMaterial) < 2)
                return false;
            if (HasPerformedPreviously(CardId.FiendsmithRequiem))
                return false;
            return true;
        }

        #endregion


        #region Generic Spells
        public bool OneforOneActivate()
        {
            if (!HasPerformedPreviously(CardId.SnakeEyeAsh, ExecutorType.Activate) || !HasPerformedPreviously(CardId.SnakeEyeOak, ExecutorType.Activate))
                return true;
            return false;
        }
        #endregion


        #region Generic Traps
        #endregion

        #region SnakeEyes

        public bool SnakeEyeAshSummon()
        {
            if (Bot.HasInHand(CardId.SnakeEyeOak) && SnakeEyeOakSummon())
                return false;

            return true;
        }

        public bool SnakeEyeAshActivate()
        {
            if (Card.IsDisabled())
                return false;

            if (ActivateDescription == Util.GetStringId(CardId.SnakeEyeAsh, 0)) // Currently broken, value is 62205969853120512
            {
                return true;
            }
            else if (ActivateDescription == Util.GetStringId(CardId.SnakeEyeAsh, 1))
            {
                return SnakeEyeSummonFromDeck();
            }

            return true;
        }

        public bool SnakeEyeOakSummon()
        {
            if (Bot.GetGraveyardMonsters().Any(x => x.Level == 1 && x.HasAttribute(CardAttribute.Fire)) || Bot.Banished.Any(x => x.Level == 1 && x.HasAttribute(CardAttribute.Fire)))
                return true;

            return false;
        }

        public bool SnakeEyeOakActivate()
        {
            if (Card.IsDisabled())
                return false;

            if (ActivateDescription == Util.GetStringId(CardId.SnakeEyeOak, 0))
            {
                return true;
            }
            else if (ActivateDescription == Util.GetStringId(CardId.SnakeEyeOak, 1))
            {
                return SnakeEyeSummonFromDeck();
            }

            return true;
        }

        public bool SnakeEyeSummonFromDeck()
        {
            if (Bot.HasInSpellZone(SNAKE_EYE_SEND_SPELL_COST))
                return true;

            if (Bot.GetMonsterCount() >= 3 && Bot.GetMonsters().Where(x => x.Level == 1 && x.HasAttribute(CardAttribute.Fire)).Count() >= 2)
                return true;

            if (Bot.GetMonsterCount() == 2 && Bot.HasInMonstersZone(CardId.RelinquishdAnima))
                return true;

            return false;
        }

        public bool SnakeEyeFlambergeActivate()
        {
            if (Card.IsDisabled())
                return false;

            var a = Util.GetStringId(CardId.SnakeEyeFlamberge, 0);
            var b = ActivateDescription;

            if (Duel.Player == 0) // Own turn effect to place
            {
                if (Duel.Turn != 1)// Non first turn actions
                {
                    foreach (int id in MONSTER_FIELD_TARGETS)
                        if (Enemy.HasInMonstersZone(id))
                            return true;
                }

                // Other targets to place
                if (Bot.HasInGraveyard(CardId.IPMasquerena))
                    return true;
            }
            else // Enemy Turn
            {
                return true; // Shot gun whatever
            }

            // If in GY activate float effect
            if (Card.Location == CardLocation.Grave)
            {
                return true;
            }

            return false;
        }

        public bool DiabellstarBlackWitchSPSummon()
        {
            if (Bot.HasInSpellZone(SNAKE_EYE_SEND_SPELL_COST))
                return true;
            if (Bot.Hand.Count() > 2)
                return true;

            return true;
        }

        public bool SnakeEyeDiabellstarActivate()
        {
            // Battle phase effect
            if (Card.Location == CardLocation.MonsterZone)
                return true;

            return true;
        }

        public bool OSSSEActivate()
        {
            if (HasPerformedPreviously(CardId.SnakeEyeAsh, ExecutorType.Activate) && HasPerformedPreviously(CardId.SnakeEyeOak, ExecutorType.Activate))
                return false;

            if (Bot.HasInMonsterZoneSpellZone(SNAKE_EYE_SEND_SPELL_COST))
                return true;

            return false;
        }

        #endregion

        public bool HasCombo()
        {
            return (SNAKE_EYES_STARTER.Any(x => Bot.HasInHandOrInSpellZone(x) && !HasPerformedPreviously(x)));
        }
    }
}
