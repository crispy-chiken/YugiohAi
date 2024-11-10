using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using WindBot.Game.AI;

namespace WindBot
{
    public class NeuralNet : AbstractAIEngine
    {
        public NeuralNet(Executor source) :
            base(source)
        {

        }

        protected override ActionInfo GetBestAction(History history)
        {
            List<ActionInfo> actions = history.ActionInfo;
            List<FieldStateValues> comparisons = history.FieldState;

            var stopwatch = Stopwatch.StartNew();

            Console.WriteLine("Actions for Turn " + source.Duel.Turn + " Action # " + ActionNumber);
            if (actions.Count == 0)
                return null;

            actions = GetActionWeights(actions, comparisons);
            Console.WriteLine("Record length : {0}---------", Records.Count);
            Console.WriteLine("Current State:---------");
            comparisons.Reverse();
            foreach (var i in comparisons)
            {
                Console.WriteLine("     " + i.ToString());
            }

            Console.WriteLine("Weights:---------");
            foreach (var action in actions)
            {
                Console.WriteLine(Math.Round(action.Weight, 3).ToString() + ":" + action.ToString());
            }

            var results = actions.OrderByDescending(x => x.Weight).ToList();
            // Take the top percentile
            var max_guess = results.Max(x => x.Weight);
            results = results.Where(x => x.Weight >= max_guess - 0.02).ToList();
            var best = results[source.Rand.Next(results.Count)];
            //FIXED RNG
            //if (!SQLComm.IsTraining)
            best = results[0];

            // Randomness
            best = actions[source.Rand.Next(actions.Count)];


            if (!SQLComm.IsManual)
            {
                // If only one option, only choose if greater than .5
                /*if (actions.Count == 1)
                {
                    actions[0].Performed = actions[0].Weight >= 0.5;
                    Console.WriteLine("Chose " + ":" + (actions[0].Performed ? "Yes" : "No"));
                    return actions[0];
                }*/
                Console.WriteLine("------- BEST ------");
                Console.WriteLine(best.ToString());

                Console.WriteLine("Chose " + ":" + best.ToString());

                Logger.DebugWriteLine("NeuralNet - GetBestAction Time:" + stopwatch.Elapsed, ConsoleColor.Green);

                return best;
            }


            if (SQLComm.IsManual)
            {
                Console.WriteLine("");
                Console.WriteLine("Actions:---------");

                for (int i = 0; i < actions.Count; i++)
                {
                    Console.WriteLine(" " + i + ":" + actions[i].ToString());
                }

                Console.WriteLine("");

                Console.WriteLine("------- BEST ------");
                Console.WriteLine(best.ToString());


                int choice = -1;


                // If there is only one option, choose it
                if (actions.Count == 1)
                    choice = 0;

                while (choice == -1)
                {
                    int result;
                    if (int.TryParse(Console.ReadLine(), out result))
                    {
                        if (result >= 0 && result < actions.Count)
                        {
                            choice = result;
                        }
                    }
                    else
                    {
                        Console.WriteLine("Chosing AI Best");
                        return best;
                    }
                }
                Console.WriteLine("Chose " + choice + ":" + actions[choice].ToString());
                return actions[choice];
            }


            // Else automatic training
            return null;
        }

        private List<ActionInfo> GetActionWeights(List<ActionInfo> actions, List<FieldStateValues> comparisons)
        {
            if (actions.Count == 0)
                return actions;

            List<long> input = new List<long>();
            List<long> data = new List<long>();

            foreach (var i in actions)
            {
                input.Add(i.ActionId);
            }
            foreach (var i in comparisons)
            {
                data.Add(i.Id);
            }
            List<double> best_predict = new List<double>(HttpComm.GetBestActionAsync(input, data, SQLComm.Name).Result);

            foreach (ActionInfo action in actions)
            {
                // Skip actions with assigned weight already
                if (action.Weight >= 0)
                {
                    if ((int)action.ActionId < best_predict.Count)
                        Console.WriteLine(best_predict[(int)action.ActionId] + "*:" + action.ToString());
                    continue;
                }

                if ((int)action.ActionId < best_predict.Count)
                {
                    action.Weight = best_predict[(int)action.ActionId];
                }
                Console.WriteLine(action.Weight + ":" + action.ToString());

                if ((action.Action == ExecutorType.GoToBattlePhase.ToString() || action.Action == ExecutorType.GoToEndPhase.ToString()))// && SQLComm.IsTraining)
                {
                    action.Weight = Math.Min(0.45, action.Weight);
                }
            }

            /*
             * Special select, find closest if a weight is -1 Very specific
             */
            if (actions[0].Action == ExecutorType.Select.ToString())
            {
                foreach (ActionInfo action in actions)
                {
                    //if (action.Weight >= 0.45)
                    //    continue;

                    // From select string builder
                    var words = action.Name.Split(';');

                    if (words.Length < 4)
                        continue;

                    string name = words[0];
                    string location = words[1];
                    string position = words[2];
                    string controller = words[3];
                    string desc = action.Desc.ToString();

                    List<double> similar1 = new List<double>();
                    List<double> similar2 = new List<double>();

                    foreach (ActionInfo allSelect in allSelectActions)
                    {
                        var Swords = allSelect.Name.Split(';');


                        if (Swords.Length < 4)
                            continue;

                        string Sname = Swords[0];
                        string Slocation = Swords[1];
                        string Sposition = Swords[2];
                        string Scontroller = Swords[3];
                        string Sdesc = "";
                        if (Swords.Length > 4)
                            Sdesc = Swords[4];

                        if ((int)allSelect.ActionId >= best_predict.Count)
                            continue; // Not sure if it is a bug, but the last action id of the list always cuts off

                        double weight = best_predict[(int)allSelect.ActionId];

                        if (weight < action.Weight)
                            continue;

                        if (weight <= 0)
                            continue;

                        int similarity = 0;

                        // Check similrity
                        if (desc != Sdesc)
                            continue;

                        if (location == Slocation)
                            similarity++;

                        if (controller == Scontroller)
                        {
                            similarity++;
                            if (name == Sname)
                                similarity++;
                        }

                        if (similarity == 1)
                        {
                            similar1.Add(weight);
                        }
                        else if (similarity >= 2)
                        {
                            similar2.Add(weight);
                        }
                    }

                    double total1 = similar1.Aggregate(0.0, (a, b) => a + b);
                    double total2 = similar2.Aggregate(0.0, (a, b) => a + b);

                    if (similar1.Count > 0) total1 /= similar1.Count;
                    if (similar2.Count > 0) total2 /= similar2.Count;

                    double multi1 = 0.2;
                    double multi2 = 0.3;
                    double multi3 = 0.5;

                    if (total1 == 0)
                    {
                        multi2 = 0.5;
                    }
                    else if (total2 == 0)
                    {
                        multi1 = 0.5;
                    }
                    else
                    {
                        multi3 = 1;
                    }

                    action.Weight = Math.Max(0, action.Weight);
                    action.Weight = Math.Min(1, total1 * multi1 + total2 * multi2 + action.Weight * multi3);
                }
            }
            // Find similar actions for activate
            else
            {
                foreach (ActionInfo action in actions)
                {
                    //if (action.Action != ExecutorType.Activate.ToString())
                    //    continue;

                    if (action.Weight >= 0)
                        continue;

                    // From string builder
                    var words = action.Name.Split(';');

                    if (words.Length < 5)
                        continue;

                    string name = words[0];
                    string id = words[1];
                    string location = words[2];
                    string phase = words[3];
                    string player = words[4]; // Current player turn
                    //string desc = action.Desc.ToString();

                    List<double> similar1 = new List<double>();
                    List<double> similar2 = new List<double>();

                    foreach (ActionInfo allSelect in allSelectActions)
                    {
                        var Swords = allSelect.Name.Split(';');

                        if (Swords.Length < 5)
                            continue;

                        string Sname = Swords[0];
                        string Sid = Swords[1];
                        string Slocation = Swords[2];
                        string Sphase = Swords[3];
                        string Splayer = Swords[4];  // Current player turn
                        //string Sdesc = "";
                        //if (Swords.Length > 4)
                        //    Sdesc = Swords[4];

                        if ((int)allSelect.ActionId >= best_predict.Count)
                            continue; // Not sure if it is a bug, but the last action id of the list always cuts off

                        // Make sure its the same card
                        if (name != Sname)
                            continue;

                        // make sure its the same action
                        if (action.Action != allSelect.Action)
                            continue;

                        double weight = best_predict[(int)allSelect.ActionId];

                        if (weight < action.Weight)
                            continue;

                        if (weight <= 0)
                            continue;

                        int similarity = 0;


                        if (location == Slocation)
                            similarity++;

                        if (phase == Sphase)
                            similarity++;

                        if (player == Splayer)
                            similarity++;


                        if (similarity == 1)
                        {
                            similar1.Add(weight);
                        }
                        else if (similarity >= 2)
                        {
                            similar2.Add(weight);
                        }
                    }

                    double total1 = similar1.Aggregate(0.0, (a, b) => a + b);
                    double total2 = similar2.Aggregate(0.0, (a, b) => a + b);

                    if (similar1.Count > 0) total1 /= similar1.Count;
                    if (similar2.Count > 0) total2 /= similar2.Count;

                    double multi1 = 0.3;
                    double multi2 = 0.4;
                    double multi3 = 0.3;

                    if (total1 == 0)
                    {
                        multi2 = 0.5;
                        multi3 = 0.5;
                    }
                    else if (total2 == 0)
                    {
                        multi1 = 0.5;
                        multi3 = 0.5;
                    }
                    else
                    {
                        multi3 = 1;
                    }

                    action.Weight = Math.Max(0, action.Weight);
                    action.Weight = Math.Min(1, total1 * multi1 + total2 * multi2 + action.Weight * multi3);
                }
            }


            return actions;
        }
    }
}

