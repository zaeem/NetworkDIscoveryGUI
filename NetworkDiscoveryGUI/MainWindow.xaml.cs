using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Net.Sockets;
using System.Threading;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Net.NetworkInformation;
using System.Web;
using System.Management;
using System.Runtime.Serialization.Json;
using System.IO;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Media.Animation;

namespace NetworkDiscoveryGUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //public ObservableCollection<InfoToSend> AllInfo;
        public ObservableCollection<InfoToSend> AllInfo { get; set; }
        int ip1;
        int ip2;
        int ip3;
        int ip4;
        int ip5;
        ProgressBar PBar2;
        public MainWindow()
        {
            InitializeComponent();
            AllInfo = new ObservableCollection<InfoToSend>();
            DataContext = this;
            discovered.ItemsSource = this.AllInfo;
        }
        
        static FormUrlEncodedContent reqData(List<InfoToSend> data)
        {
            List<KeyValuePair<string, List<InfoToSend>>> bodyProperties = new List<KeyValuePair<string, List<InfoToSend>>>();
            bodyProperties.Add(new KeyValuePair<string, List<InfoToSend>>("data", data));
            for (int i = 0; i < bodyProperties.Count; i++)
            {
                var temp = bodyProperties[0];
                //   Functions.log(string.Format("Key : {0}", temp.Key), 3);
                for (int j = 0; j < temp.Value.Count; j++)
                {
                    //   Functions.log(string.Format("DisplayName {0}", temp.Value[j].displayName), 3);
                    //    Functions.log(string.Format("MacAddress {0}", temp.Value[j].macAddress), 3);
                    //    Functions.log(string.Format("IPAddress {0}", temp.Value[j].ipAddress), 3);
                }
                /*Functions.log(string.Format("DisplayName {0}", temp.DisplayName), 3);
                Functions.log(string.Format("MacAddress {0}", temp.MacAddress), 3);
                Functions.log(string.Format("IPAddress {0}", temp.IpAddress), 3);*/
            }
            DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(List<InfoToSend>));
            MemoryStream ms = new MemoryStream();
            ser.WriteObject(ms, data);
            string jsonString = Encoding.UTF8.GetString(ms.ToArray());
            ms.Close();
            // Functions.log(string.Format("JSON {0}", jsonString), 3);
            string secretkey = "";
            const Int32 BufferSize = 128;
            using (var fileStream = File.OpenRead("temp.txt"))
            using (var streamReader = new StreamReader(fileStream, Encoding.UTF8, true, BufferSize))
            {
                String line;
                while ((line = streamReader.ReadLine()) != null)
                {
                    secretkey = line;
                }
            }

            return new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("secretKey", secretkey),
                new KeyValuePair<string, string>("data", jsonString)
            });
        }

        private void Start_Discovery(object sender, RoutedEventArgs e)
        {
            CreateDynamicProgressBarControl();
            if (this.range.Text != "")
            {
                if (isValidIP())
                {
                    BackgroundWorker worker = new BackgroundWorker();
                    worker.WorkerReportsProgress = true;
                    worker.DoWork += (o, a) =>
                    {
                        Program.Discovery(Percent,PBar2,ip1.ToString()+"."+ ip2.ToString() + "." + ip3.ToString() + ".", ip4,ip5);
                    };
                    worker.RunWorkerCompleted += (o, a) =>
                    {
                        while (AllInfo.Count != 0)
                        {
                            AllInfo.RemoveAt(0);
                        }
                        foreach (InfoToSend i in Program.AllInfo)
                        {
                            if (checkIP(i.ipAddress))
                            {
                                this.AllInfo.Add(i);
                            }
                        }
                        count.Text = "Devices Found: " + (this.AllInfo.Count).ToString();
                        Application.Current.Dispatcher.Invoke(() => { PBar2.Value = 100; });
                        Application.Current.Dispatcher.Invoke(() => { Percent.Text = "100%"; });
                        Application.Current.Dispatcher.Invoke(() => { PBar2.Value = 0; });
                        SBar.Items.Remove(PBar2);
                    };
                    worker.RunWorkerAsync();
                }
                else
                {
                    MessageBox.Show("Enter Valid IP Range.", "Nulodgic Discovery Tool", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                BackgroundWorker worker = new BackgroundWorker();
                worker.WorkerReportsProgress = true;
                worker.DoWork += (o, a) =>
                {
                    Program.Discovery(Percent, PBar2);
                };
                worker.RunWorkerCompleted += (o, a) =>
                {
                    while (AllInfo.Count != 0)
                    {
                        AllInfo.RemoveAt(0);
                    }
                    foreach (InfoToSend i in Program.AllInfo)
                    {
                        this.AllInfo.Add(i);
                    }
                    count.Text = "Devices Found: " + (AllInfo.Count).ToString();
                    Application.Current.Dispatcher.Invoke(() => { PBar2.Value = 100; });
                    Application.Current.Dispatcher.Invoke(() => { Percent.Text = "100%"; });
                    Application.Current.Dispatcher.Invoke(() => { PBar2.Value = 0; });
                    SBar.Items.Remove(PBar2);
                };
                worker.RunWorkerAsync();
            }
        }
        private void CreateDynamicProgressBarControl()
        {
            PBar2 = new ProgressBar();
            PBar2.IsIndeterminate = false;
            PBar2.Orientation = Orientation.Horizontal;
            PBar2.Width = 900;
            PBar2.Height = 20;
            SBar.Items.Add(PBar2);
        }
        private bool checkIP(string ip)
        {
            string[] valid = ip.Split('.');
            if (int.Parse(valid[0]) != ip1)
            {
                return false;
            }
            if (int.Parse(valid[1]) != ip2)
            {
                return false;
            }
            if (int.Parse(valid[2]) != ip3)
            {
                return false;
            }
            if (int.Parse(valid[3]) < ip4 || int.Parse(valid[3]) > ip5)
            {
                return false;
            }
            return true;

        }
        private bool isValidIP()
        {
            try
            {
                string IPrange = this.range.Text;
                string[] valid = IPrange.Split('.');
                string[] valid2 = valid[3].Split('-');
                ip1 = int.Parse(valid[0]);
                if (!(ip1 >= 1 && ip1 <= 255))
                {
                    return false;
                }
                ip2 = int.Parse(valid[1]);
                if (!(ip2 >= 0 && ip2 <= 255))
                {
                    return false;
                }
                ip3 = int.Parse(valid[2]);
                if (!(ip3 >= 0 && ip3 <= 255))
                {
                    return false;
                }
                ip4 = int.Parse(valid2[0]);
                if (!(ip4 >= 0 && ip4 <= 255))
                {
                    return false;
                }
                ip5 = int.Parse(valid2[1]);
                if (!(ip5 > ip4 && ip5 <= 255))
                {
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        private void Push_Data(object sender, RoutedEventArgs e)
        {
            if (this.AllInfo.Count != 0)
            {
                BusyIndicator.IsBusy = true;
                BusyIndicator.BusyContent = "Sending Data...";
                BackgroundWorker worker = new BackgroundWorker();
                worker.DoWork += (o, a) =>
                {
                    AssetHandler asset = new AssetHandler(new HttpClient());
                    asset.initRequest();
                    string BaseUrl = "http://secure.nulodgic-staging.com".ToString();
                    asset.postAssetData(reqData(Program.AllInfo), BaseUrl + "/asset_collections/discovery_asset");
                };
                worker.RunWorkerCompleted += (o, a) =>
                {
                    BusyIndicator.IsBusy = false;
                    MessageBox.Show("Data Sent.", "Nulodgic Discovery Tool", MessageBoxButton.OK, MessageBoxImage.Information);
                };
                worker.RunWorkerAsync();
            }
            else
            {
                MessageBox.Show("No Data to send.", "Nulodgic Discovery Tool", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
