using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;
using static WindBot.AbstractAIEngine;
using System.IO;

namespace WindBot
{
    class JsonExporter
    {
        public static string ExportPath = "";
        public static void SavePlayHistory(List<History> records, int result)
        {
            string fileName = SQLComm.Name + DateTime.Now.ToString("_yyyy-MM,dd-HH-mm-ss") + ".json";
            string directory = Directory.GetCurrentDirectory();
            if (ExportPath != "")
                directory = ExportPath;

            var path = Path.Combine(directory, fileName);

            using (StreamWriter sr = new StreamWriter(File.OpenWrite(path)))
            {
                foreach (var record in records)
                {
                    List<string> actions = new List<string>();
                    List<string> data = new List<string>();

                    foreach (var action in record.ActionInfo)
                    {
                        actions.Add(action.ActionId.ToString());
                    }

                    foreach (var state in record.FieldState)
                    {
                        data.Add(state.Id.ToString());
                    }

                    var values = new Dictionary<string, string>
                    {
                        { "actions", string.Join(",", actions) },
                        { "performed" , record.ActionInfo.Where(x => x.Performed).FirstOrDefault()?.ActionId.ToString()},
                        { "state", string.Join(",", data) },
                        { "result", result.ToString() }
                    };

                    var json = JsonConvert.SerializeObject(values);
                    sr.WriteLine(json);
                }
            }            
        }
    }
}
