using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace IPv6_Scanner
{
    public class IPv6Scanner
    {
        private CountdownEvent countdown;
        private int upCount = 0;
        private object lockObj = new object();
        private const bool resolveNames = true;

        public void Start(string ipBase)
        {
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
                    p.SendAsync(ip, 100, ip);
                }
                catch (PingException)
                {
                    Console.WriteLine($"{ip} cannot be reached.");
                    p.Dispose();
                    countdown.Signal();
                }
                finally
                {
                    countdown.Signal();
                    countdown.Wait();
                }
                sw.Stop();
            }
            TimeSpan span = new TimeSpan(sw.ElapsedTicks);
            Console.WriteLine("Took {0} milliseconds. {1} hosts active.\r\n", sw.ElapsedMilliseconds, upCount);
            Console.ReadLine();
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
                var tempAdr = new List<string>();

                for (int i = 0; i < okt.Length; i++)
                {
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
                    Console.WriteLine("{0} ({1}) is up: ({2} ms)", ip, name, e.Reply.RoundtripTime);
                }
                else
                {
                    Console.WriteLine("{0} is up: ({1} ms)", ip, e.Reply.RoundtripTime);
                }
                lock (lockObj)
                {
                    upCount++;
                }
            }
            else if (e.Reply == null)
            {
                Console.WriteLine("Pinging {0} failed.", ip);
            }
            countdown.Signal();
        }
    }
}
//        private static void PingCompletedCallback(object sender, PingCompletedEventArgs e)
//{
//    // If the operation was canceled, display a message to the user.
//    if (e.Cancelled)
//    {
//        Console.WriteLine("Ping canceled.");

//        // Let the main thread resume.
//        // UserToken is the AutoResetEvent object that the main thread
//        // is waiting for.
//        ((AutoResetEvent)e.UserState).Set();
//    }

//    // If an error occurred, display the exception to the user.
//    if (e.Error != null)
//    {
//        Console.WriteLine("Ping failed:");
//        Console.WriteLine(e.Error.ToString());

//        // Let the main thread resume.
//        ((AutoResetEvent)e.UserState).Set();
//    }

//    PingReply reply = e.Reply;

//    DisplayReply(reply);

//    // Let the main thread resume.
//    ((AutoResetEvent)e.UserState).Set();
//}

//public static void DisplayReply(PingReply reply)
//{
//    if (reply == null)
//        return;

//    Console.WriteLine("ping status: {0}", reply.Status);
//    if (reply.Status == IPStatus.Success)
//    {
//        Console.WriteLine("Address: {0}", reply.Address.ToString());
//        Console.WriteLine("RoundTrip time: {0}", reply.RoundtripTime);
//        Console.WriteLine("Time to live: {0}", reply.Options.Ttl);
//        Console.WriteLine("Don't fragment: {0}", reply.Options.DontFragment);
//        Console.WriteLine("Buffer size: {0}", reply.Buffer.Length);
//    }
//}