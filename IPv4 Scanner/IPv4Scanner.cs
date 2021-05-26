using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Controls;

namespace IPv4_Scanner
{
    public class IPv4Scanner
    {
        private CountdownEvent countdown;
        private int upCount = 0;
        private object lockCount = new object();
        private object lockOut = new object();
        private const bool resolveNames = true;
        private bool _showErrors;
        private TextBox _console;

        public void Start(string ipBase, bool showerrors, ref TextBox console)
        {
            _showErrors = showerrors;
            _console = console;
            _console.AppendText("Starting ...");
            countdown = new CountdownEvent(1);
            Stopwatch sw = new Stopwatch();

            sw.Start();
            var adresses = getAdresses(ipBase);
            foreach (var ip in adresses)
            {
                Ping p = new Ping();
                p.PingCompleted += p_PingCompleted;
                countdown.AddCount();
                try
                {
                    lock(lockOut)
                        _console.AppendText($"Pinging {ip}... ");
                    p.SendAsync(ip, 100, ip);
                }
                catch (PingException)
                {
                    if (showerrors)
                        lock (lockOut)
                            _console.AppendText($"{ip} cannot be reached.");
                    p.Dispose();
                    countdown.Signal();
                }
                finally
                {
                    countdown.Signal();
                    countdown.Wait();
                }
            }

            sw.Stop();
            TimeSpan span = new TimeSpan(sw.ElapsedTicks);
            lock (lockOut)
                _console.AppendText($"Took {sw.ElapsedMilliseconds} milliseconds. {upCount} hosts active.\r\n");
        }
        private void p_PingCompleted(object sender, PingCompletedEventArgs e)
        {
            string ip = (string)e.UserState;
            if (e.Reply != null && e.Reply.Status == IPStatus.Success)
            {
                if (resolveNames)
                {
                    string name;
                    try
                    {
                        IPHostEntry hostEntry = Dns.GetHostEntry(ip);
                        name = hostEntry.HostName;
                    }
                    catch (SocketException ex)
                    {
                        name = "?";
                    }
                    lock (lockOut)
                        _console.AppendText($"{ip} ({name}) is up: ({e.Reply.RoundtripTime} ms)");
                }
                else
                {
                    lock (lockOut)
                        _console.AppendText($"{ip} is up: ({e.Reply.RoundtripTime} ms)");
                }
                lock (lockCount)
                {
                    upCount++;
                }
            }
            else if (e.Reply == null)
            {
                if (_showErrors)
                    lock (lockOut)
                        _console.AppendText($"Pinging {ip} failed.");
            }
            countdown.Signal();
        }
        private List<string> getAdresses(string input)
        {
            var result = new List<string>();
            var regex = new Regex(@"(\d)*\.(\d)*\.(\d)*\.(\d)*");
            var mask = regex.Match(input).ToString();
            if (!string.IsNullOrEmpty(mask))
            {
                var oktToMake = new List<int>();
                int count = 0;
                int[] okt = new int[4];
                var num = new StringBuilder();
                for (int i = 0; i < mask.Length; i++)
                {
                    if (char.IsDigit(mask[i])) num.Append(mask[i]);
                    if (mask[i] == '.' || i == mask.Length - 1)
                    {
                        if (num.Length > 0)
                        {
                            okt[count] = int.Parse(num.ToString());
                            num.Clear();
                        }
                        else
                        {
                            oktToMake.Add(count);
                        }
                        count++;
                    }
                }
                for (int i = 0; i < okt.Length; i++)
                {
                    var tempAdr = new List<string>();
                    if (oktToMake.Contains(i))
                    {
                        if (i == 0)
                        {
                            for (int j = 0; j < 256; j++)
                            {
                                result.Add(j.ToString());
                            }
                        }
                        else
                        {
                            foreach (var ip in result)
                            {
                                for (int j = 0; j < 256; j++)
                                {
                                    tempAdr.Add($"{ip}.{j}");
                                }
                            }
                            result = tempAdr;
                        }
                    }
                    else
                    {
                        if (i == 0)
                        {
                            result.Add(okt[i].ToString());
                        }
                        else
                        {
                            foreach (var ip in result)
                            {
                                tempAdr.Add($"{ip}.{okt[i]}");
                            }
                            result = tempAdr;
                        }
                    }
                }
            }

            return result;
        }
    }
}
