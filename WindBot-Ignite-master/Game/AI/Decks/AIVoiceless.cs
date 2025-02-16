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
    [Deck("Voiceless", "AI_Voiceless")]
    public class AIVoicelss : AIHardCodedBase
    {

        public AIVoicelss(GameAI ai, Duel duel)
            : base(ai, duel)
        {
            // Basically First Actions
            
            // Normal Priority
            
            // Low Priority

            // Reactive
            AddExecutor(ExecutorType.Activate, CardId.ForbiddenDroplet, DropletActivate);
            AddExecutor(ExecutorType.Activate, CardId.DimensionalBarrier, DefaultDimensionalBarrier);

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

        #endregion


        #region Generic Traps
        #endregion

        #region Labrynth
      
        #endregion
    }
}
