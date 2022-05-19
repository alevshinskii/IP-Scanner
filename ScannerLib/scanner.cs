using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ScannerLib
{

    public interface IScanner
    {
        void StartScanAsync();
    }
    public class IPv4Scanner:IScanner
    {
        private int oneTimePingLimit = 10;
        public ScannerSettings Settings { get; set; }

        public void StartScanAsync()
        {
            if (Settings.Interface is Ipv4Interface ipv4Interface)
            {
                if (Settings.ScanType == "Subnet")
                {
                    var ip = ipv4Interface.Ip;
                    var subnet = ipv4Interface.SubnetMask;
                }
                else if (Settings.ScanType == "Range")
                {
                    if (IPAddress.TryParse(Settings.IPRangeBottom,out var bottom) && IPAddress.TryParse(Settings.IPRangeTop, out var top))
                    {
                        int i = 0;
                        var bottomSplit = new int[4];
                        var topSplit = new int[4];
                        foreach (var okt in bottom.ToString().Split('.', StringSplitOptions.RemoveEmptyEntries))
                        {
                            bottomSplit[i] = int.Parse(okt);
                            i++;
                        }
                        i = 0;
                        foreach (var okt in top.ToString().Split('.', StringSplitOptions.RemoveEmptyEntries))
                        {
                            topSplit[i] = int.Parse(okt);
                            i++;
                        }

                        if (CheckCorrectRange(bottomSplit, topSplit))
                        {
                            if (bottomSplit[0] != topSplit[0])
                            {
                                topSplit[0]=bottomSplit[0];
                            }
                            else if (bottomSplit[1] != topSplit[1])
                            {
                                topSplit[1]=bottomSplit[1];
                            }
                            else if (bottomSplit[2] != topSplit[2])
                            {

                            }
                            else if (bottomSplit[3] != topSplit[3])
                            {

                            }
                            else
                            {
                                throw new ArgumentException();
                            }
                        }
                    }
                }
                else
                {
                    throw new NotImplementedException();
                }
            }
            else
            {
                throw new ArgumentException();
            }
        }

        private bool CheckCorrectRange(int[] bottomSplit, int[] topSplit)
        {

            for (int i = 0; i < bottomSplit.Length; i++)
            {
                if (bottomSplit[i] > topSplit[i])
                {
                    return false;
                }
            }
            return true;
        }
    }
    public class IPv6Scanner:IScanner
    {
        public ScannerSettings Settings { get; set; }

        public void StartScanAsync()
        {
            throw new NotImplementedException();
        }
    }

    public class ScannerSettings
    {
        public INetInterface Interface { get; set; }
        public string ScanType { get; set; }
        public string IPRangeBottom { get; set; }
        public string IPRangeTop { get; set; }
        public int Interval { get; set; }

    }

}
