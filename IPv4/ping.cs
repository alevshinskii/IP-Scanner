using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lib
{
    public class PingItem
    {
        public string Ip { get; set; }

        public string MacAddress { get; set; }

        public string Type { get; set; }
    }


    public class PingUtil
    {
        public string GetPingResult(string ip)
        {
            using (Process process = Process.Start(new ProcessStartInfo("ping", ip)
                   {
                       CreateNoWindow = true,
                       UseShellExecute = false,
                       RedirectStandardOutput = true
                   }))
            {
                var output = process.StandardOutput.ReadToEnd();
                return output;
            }
        }

        private List<PingItem> ParsePingResult(string output)
        {
            var lines = output.Split('\n');

            var result = from line in lines
                let item = line.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
                where item.Count() == 4
                select new PingItem()
                {
                    Ip = item[0],
                    MacAddress = item[1],
                    Type = item[2]
                };

            return result.ToList();
        }
    }
}
