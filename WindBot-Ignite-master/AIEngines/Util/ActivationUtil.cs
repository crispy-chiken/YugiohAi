using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WindBot.Game.AI;
using WindBot.Game.AI.Decks;
using static WindBot.AbstractAIEngine;
using static WindBot.PathEngine;

namespace WindBot.AIEngines.Util
{
    class ActivationUtil : Heuristics
    {
        public static bool ShouldNotPerform(ActionInfo action, AIBase e)
        {
            if (action.Name.Contains("Snake-Eyes Flamberge Dragon;48452496;MonsterZone;Main1;0;50806124445696") && !e.Util.Bot.HasInGraveyard(CardId.IPMasquerena))
                return true;
            // TODO check for sp banish on summon desc index
            if (action.Name.Contains("S:P Little Knight;29301450;MonsterZone;Main1;0;") && action.Action == "Activate" && e.Util.Enemy.GetMonsterCount() == 0 && e.Util.Enemy.Graveyard.Count == 0)
                return true;


            return false;
        }

        public static bool ShouldPerform(ActionInfo action, AIBase e)
        {
            if (action.Name.Contains("Snake-Eyes Poplar;90241276;MonsterZone;Main1;0;62205969853120512"))
                return true;

            return false;
        }
    }
}
