using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace WpfApp1
{
    public partial class MainWindow : Window
    {

        private CountdownEvent _countdown;
        private int _upCount;
        private object _lockCount = new object();
        private bool _resolveNames = true;
        private int _timeout = 100;
        private Stopwatch _sw = new Stopwatch();
        private bool _continueScan = true;
        private bool _showErrors = false;
        private string _welcomeText = " Welcome to IP Scanner!\r\n It's small program, that shows IP devices in your local network.\r\n Just paste mask in text box on the top and press 'scan'.\r\n For example, with mask '192.168.1.' program start to ping IPs in range from '192.168.1.0' to '192.168.1.255'\r\n Skipping more than 2 octets (like '...' or '192...') in mask may decrease performance of your device.";
        public MainWindow()
        {
            InitializeComponent();
            TaskPB.Visibility = Visibility.Hidden;
            stopBTN.Visibility = Visibility.Hidden;
            outTB.Text = _welcomeText;
        }

        private void ScanBTN_OnClick(object sender, RoutedEventArgs e)
        {
            EditProgressBar(0);
            HideStopButton();
            TaskPB.Visibility = Visibility.Visible;
            scanBTN.IsEnabled = false;
            UpdateErrorsCheckBox();
            StartIPv4Scan(maskTB.Text);
            ShowStopButton();
        }

        private async void ShowStopButton()
        {
            await Task.Run((() =>
                {
                    Thread.Sleep(3000);
                    this.Dispatcher.Invoke(() =>
                    {
                        stopBTN.Visibility = Visibility.Visible;
                    });
                }
                ));
        }

        private async void HideStopButton()
        {
            await Task.Run((() =>
                    {
                        this.Dispatcher.Invoke(() =>
                        {
                            if (stopBTN.Visibility == Visibility.Visible)
                                stopBTN.Visibility = Visibility.Hidden;
                        });
                    }
                ));
        }

        private void UpdateErrorsCheckBox()
        {
            if (errorsCB.IsChecked == true)
                _showErrors = true;
            else
                _showErrors = false;
        }

        private async void StartIPv4Scan(string ipBase)
        {
            _continueScan = true;
            outTB.Clear();
            TryReadTimeout();
            WriteToConsole("Starting ...\r\n");
            await Task.Run(() => PingIPv4Async(ipBase));
            TimeSpan span = new TimeSpan(_sw.ElapsedTicks);
            WriteToConsole($"Took {_sw.ElapsedMilliseconds} milliseconds. {_upCount} hosts active.\r\n");
            scanBTN.IsEnabled = true;
            HideStopButton();
        }

        private void TryReadTimeout()
        {
            try
            {
                _timeout = int.Parse(timeoutTB.Text);
            }
            catch (Exception)
            {
                _timeout = 100;
                WriteToConsole("Cannot read timeout. Now used 100 ms.\r\n");
                timeoutTB.Text = "100";
            }
        }

        private void PingIPv4Async(string ipBase)
        {
            _upCount = 0;
            _sw.Start();

            var allAdresses = GetAdresses(ipBase);
            int n = allAdresses.Count;
            int i = 0;
            while (i < allAdresses.Count && _continueScan)
            {
                List<string> adresses;
                if (i + 100 <= allAdresses.Count)
                    adresses = allAdresses.GetRange(i, 100);
                else
                    adresses = allAdresses.GetRange(i, allAdresses.Count % 100);
                _countdown = new CountdownEvent(1);
                foreach (var ip in adresses)
                {
                    _countdown.AddCount(1);
                    Ping p = new Ping();
                    p.PingCompleted += PingCompleted;
                    try
                    {
                        p.SendAsync(ip, _timeout, ip);
                    }
                    catch (PingException)
                    {
                        if (_showErrors)
                            WriteToConsole($"{ip} cannot be reached.\r\n");
                        p.Dispose();
                        _countdown.Signal();
                    }
                }
                _countdown.Signal();
                _countdown.Wait();
                i += 100;
                EditProgressBar((i * 100) / n);
            }
            EditProgressBar(100);
            _sw.Stop();
        }

        private void PingCompleted(object sender, PingCompletedEventArgs e)
        {
            string ip = (string)e.UserState;
            if (e.Reply != null && e.Reply.Status == IPStatus.Success)
            {
                if (_resolveNames)
                {
                    string name;
                    try
                    {
                        IPHostEntry hostEntry = Dns.GetHostEntry(ip);
                        name = hostEntry.HostName;
                    }
                    catch (SocketException)
                    {
                        name = "?";
                    }
                    WriteToConsole($"{ip} ({name}) is up: ({e.Reply.RoundtripTime} ms)\r\n");
                }
                else
                {
                    WriteToConsole($"{ip} is up: ({e.Reply.RoundtripTime} ms)\r\n");
                }
                lock (_lockCount)
                {
                    _upCount++;
                }
            }
            else if (e.Reply == null || (e.Reply != null && e.Reply.Status != IPStatus.Success))
            {
                if (_showErrors)
                    WriteToConsole($"Pinging {ip} failed.\r\n");
            }
            _countdown.Signal();
        }

        private void EditProgressBar(int value)
        {
            try
            {
                this.Dispatcher.Invoke(() =>
                {
                    TaskPB.Value = value;
                });
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        private void WriteToConsole(string line)
        {
            try
            {
                this.Dispatcher.Invoke(() =>
                {
                    outTB.AppendText(line);
                });
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        private List<string> GetAdresses(string input)
        {
            var result = new List<string>();
            var regex = new Regex(@"(\d)*\.(\d)*\.(\d)*\.(\d)*");
            var mask = regex.Match(input).ToString();
            if (!string.IsNullOrEmpty(mask))
            {
                var oktToMake = new List<int>();
                int count = 0;
                int[] okt = new int[4];
                for (int i = 0; i < okt.Length; i++)
                {
                    okt[i] = -1;
                }
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
                        count++;
                    }
                }

                for (int i = 0; i < okt.Length; i++)
                {
                    if (okt[i] < 0)
                    {
                        oktToMake.Add(i);
                    }
                }
                for (int i = 0; i < okt.Length; i++)
                {
                    var tempAdr = new List<string>();
                    try
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
                    catch (OutOfMemoryException)
                    {
                        WriteToConsole("A lot of combinations, try to write more octets.\r\n");
                        tempAdr.Clear();
                        result.Clear();
                        return new List<string>();
                    }
                }
            }
            return result;
        }

        private void StopBTN_OnClick(object sender, RoutedEventArgs e)
        {
            if (_continueScan)
            {
                _continueScan = false;
                WriteToConsole("Stopping...\r\n");
            }
        }

        private void OutTB_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            this.Dispatcher.Invoke(() =>
            {
                ScrollViewer.ScrollToEnd();
            });
        }
    }
}
