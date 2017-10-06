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

namespace NetworkDiscoveryGUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //public ObservableCollection<InfoToSend> AllInfo;
        public ObservableCollection<InfoToSend> AllInfo { get; set; }
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


            BusyIndicator.IsBusy = true;
            BusyIndicator.BusyContent = "Scanning Network...";
            BackgroundWorker worker = new BackgroundWorker();
            worker.DoWork += (o, a) =>
            {
                Program.Discovery();
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
                BusyIndicator.IsBusy = false;
            };
            worker.RunWorkerAsync();
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
