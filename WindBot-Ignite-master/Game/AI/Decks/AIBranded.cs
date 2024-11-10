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
    [Deck("Branded", "AI_Branded")]
    public class AIBranded : AIHardCodedBase
    {

        public AIBranded(GameAI ai, Duel duel)
            : base(ai, duel)
        {
            // Basically First Actions
            AddExecutor(ExecutorType.GoToBattlePhase, GoToBattlePhase);
            AddExecutor(ExecutorType.Activate, CardId.EvenlyMatched);
            // Normal Priority
            AddExecutor(ExecutorType.Summon, CardId.AluberDespia);
            AddExecutor(ExecutorType.Summon, CardId.GuidingQuem);
            AddExecutor(ExecutorType.Summon, CardId.BlazingCartesia);
            AddExecutor(ExecutorType.Summon, CardId.FallenOfAlbaz);
            AddExecutor(ExecutorType.Summon, CardId.SpringansKitt);

            AddExecutor(ExecutorType.SpSummon, CardId.BystialLubellion);

            AddExecutor(ExecutorType.Activate, CardId.BystialLubellion);
            AddExecutor(ExecutorType.Activate, CardId.AlbionTheShroudedDragon);
            AddExecutor(ExecutorType.Activate, CardId.BystialSaronir, BystialActivate);
            AddExecutor(ExecutorType.Activate, CardId.BystialMagnamhut, BystialActivate);
            AddExecutor(ExecutorType.Activate, CardId.AluberDespia);
            AddExecutor(ExecutorType.Activate, CardId.FallenOfAlbaz);
            AddExecutor(ExecutorType.Activate, CardId.SpringansKitt);
            AddExecutor(ExecutorType.Activate, CardId.BlazingCartesia, CartesiaActivate);
            AddExecutor(ExecutorType.Activate, CardId.GuidingQuem);
            AddExecutor(ExecutorType.Activate, CardId.TriBrigadeMercourier, MercourierActivate);
            AddExecutor(ExecutorType.Activate, CardId.DespianTragedy);

            AddExecutor(ExecutorType.Activate, CardId.AllureOfDarkness, DefaultAllureofDarkness);
            AddExecutor(ExecutorType.Activate, CardId.BrandedFusion);
            AddExecutor(ExecutorType.Activate, CardId.FoolishBurial);
            AddExecutor(ExecutorType.Activate, CardId.FusionDeployment);
            AddExecutor(ExecutorType.Activate, CardId.GoldSarc);
            AddExecutor(ExecutorType.Activate, CardId.TripleTacticsTalent);
            AddExecutor(ExecutorType.Activate, CardId.TripleTacticsThrust);
            AddExecutor(ExecutorType.Activate, CardId.BrandedInHighSpirits, HighSpiritsActivate);
            AddExecutor(ExecutorType.Activate, CardId.BrandedInRed, BrandedInRedActivate);
            AddExecutor(ExecutorType.Activate, CardId.BrandedOpening);
            AddExecutor(ExecutorType.Activate, CardId.CalledByTheGrave, CalledByActivate);
            AddExecutor(ExecutorType.Activate, CardId.ForbiddenDroplet, DropletActivate);
            AddExecutor(ExecutorType.Activate, CardId.BrandedRetribution, RetributionActivate);
            AddExecutor(ExecutorType.Activate, CardId.FusionDuplication, DuplicationActivate);

            AddExecutor(ExecutorType.Activate, CardId.GuardianChimera);
            AddExecutor(ExecutorType.Activate, CardId.AlbionTheSanctifireDragon);
            AddExecutor(ExecutorType.Activate, CardId.BorreloadFuriousDragon, FuriousActivate);
            AddExecutor(ExecutorType.Activate, CardId.MirrorJadeTheIcebladeDragon, MirrorjadeActivate);
            AddExecutor(ExecutorType.Activate, CardId.PredaplantDRagostapelia, FaceUpEffectNegate);
            AddExecutor(ExecutorType.Activate, CardId.LubellionSearingDragon);
            AddExecutor(ExecutorType.Activate, CardId.AlbaLenatusAbyssDragon);
            AddExecutor(ExecutorType.Activate, CardId.DespianQuaeritis, QuartusActivate);
            AddExecutor(ExecutorType.Activate, CardId.GranguignolDuskDragon);
            AddExecutor(ExecutorType.Activate, CardId.TitanikladAshDragon);
            AddExecutor(ExecutorType.Activate, CardId.AlbionTheBrandedDragon);
            AddExecutor(ExecutorType.Activate, CardId.RindbrummStrikingDragon, RindbrummActivate);

            AddExecutor(ExecutorType.Activate, CardId.ShadollDragon);
            AddExecutor(ExecutorType.Activate, CardId.SnatchSteal);
            AddExecutor(ExecutorType.Activate, CardId.ChangeOfHeart);
            AddExecutor(ExecutorType.Activate, CardId.BookOfEclipse, EclipseActivate);

            AddExecutor(ExecutorType.Activate, CardId.BrightestBlazingBranded,BlazingBrandedActivate);


            // Low Priority

            // Reactive
            AddExecutor(ExecutorType.Activate, CardId.CrossoutDesignator, CrossoutActivate);
            AddExecutor(ExecutorType.Activate, CardId.ForbiddenDroplet, DropletActivate);
            AddExecutor(ExecutorType.Activate, CardId.DimensionalBarrier, DefaultDimensionalBarrier);
            AddExecutor(ExecutorType.Activate, CardId.CrossoutDesignator, CosmicActivate);
            AddExecutor(ExecutorType.Activate, CardId.FantasticalPhantazmay);

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
       


        #endregion


        #region Generic Spells
        public bool EclipseActivate()
        {
            return true;
        }
        #endregion


        #region Generic Traps
        #endregion

        #region Branded
        public bool CartesiaActivate()
        {
            return true;
        }

        public bool MercourierActivate()
        {
            return true;
        }

        public bool HighSpiritsActivate()
        {
            return true;
        }

        public bool BrandedInRedActivate()
        {
            return true;
        }

        public bool RetributionActivate()
        {
            return true;
        }

        public bool DuplicationActivate()
        {
            return true;
        }


        public bool FuriousActivate()
        {
            return true;
        }

        public bool MirrorjadeActivate()
        {
            return true;
        }

        public bool QuartusActivate()
        {
            return true;
        }

        public bool RindbrummActivate()
        {
            return true;
        }

        public bool BlazingBrandedActivate()
        {
            return true;
        }

        #endregion
    }
}
