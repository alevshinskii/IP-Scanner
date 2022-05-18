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
using System.Windows.Controls;

namespace App
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            PrepareWindow();
        }

        private void PrepareWindow()
        {
            
        }


        private void ScanBTN_OnClick(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void StopBTN_OnClick(object sender, RoutedEventArgs e)
        {
        }



        private void Interfaces_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
        }

        private void OutTB_TextChanged(object sender, TextChangedEventArgs e)
        {
            throw new NotImplementedException();
        }


    }
}
