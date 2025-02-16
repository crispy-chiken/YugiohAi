using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static WindBot.AbstractAIEngine;
using System.Net.Http;
using Newtonsoft.Json;
using System.IO;
using WindBot.Game.AI;
using static WindBot.NeuralNet;

namespace WindBot
{
    public class CSVReader
    {
        public static string csvPath = $@"../../../baseaction.csv";
        public static string csvCombos = $@"../../../combos.csv";

        public static Dictionary<ActionInfo, List<FieldStateValues>> BaseActions = new Dictionary<ActionInfo, List<FieldStateValues>>();

        public static void Init()
        {
            using (var reader = new StreamReader(csvPath))
            {
                string headerLine = reader.ReadLine();

                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var values = line.Split('|');

                    string name = values[0];
                    string actiontype = values[1];
                    string location = values[2];
                    string compare = values[3];
                    string value = values[4];

                    ExecutorType action;
                    if (!Enum.TryParse(actiontype, out action))
                    {
                        Logger.WriteErrorLine($"Could not parse action {actiontype}");
                        continue;
                    }

                    ActionInfo actionInfo = new ActionInfo( name, action.ToString(), null );
                    var c = new FieldStateValues(){ Location = location, Compare = compare, Value = value };

                    var sequence = BaseActions.Keys.Where(x => x.Name == actionInfo.Name && x.Action == actionInfo.Action);
                    ActionInfo key = null;

                    if (sequence.Any())
                    {
                        key = sequence.First();
                    }

                    if (key == null)
                    {
                        key = actionInfo;
                        BaseActions.Add(key, new List<FieldStateValues>());
                    }

                    BaseActions[key].Add(c);
                }
            }
        }

        public static ActionInfo GetNextAction(List<ActionInfo> previousActions)
        {
            return null;
        }

        public static List<double> GetBaseActionValues(int length, List<ActionInfo> actions, List<FieldStateValues> comparisons)
        {
            double bonus = 0.5;
            List<double> bonusWeight = new List<double>(new double[length]);

            foreach(var action in actions)
            {
                var sequence = BaseActions.Keys.Where(x => action.Name.Contains(x.Name) && x.Action == action.Action);
                if (sequence.Any())
                {
                    foreach(var c in BaseActions[sequence.First()])
                    {
                        var compare_sequence = comparisons.Where(x =>
                                (c.Compare == "" || x.Compare == c.Compare) &&
                                (c.Value == "" || x.Value == c.Value) &&
                                (c.Location == "" || x.Location == c.Location)
                            );

                        // Is in base action
                        if (compare_sequence.Any())
                            bonusWeight[(int)action.ActionId] = bonus;

                    }
                }
            }

            return bonusWeight;
        }

        public static bool InBaseActions(string name, string action, List<FieldStateValues> comparisons)
        {
            var sequence = BaseActions.Keys.Where(x => x.Name == name && x.Action.ToString() == action);
            if (sequence.Any())
            {
                foreach (var c in BaseActions[sequence.First()])
                {
                    var compare_sequence = comparisons.Where(x =>
                            (c.Compare == "" || x.Compare == c.Compare) &&
                            (c.Value == "" || x.Value == c.Value) &&
                            (c.Location == "" || x.Location == c.Location)
                        );

                    // Is in base action
                    if (compare_sequence.Any())
                        return true;

                }
            }

            return false;
        }
    }
}
