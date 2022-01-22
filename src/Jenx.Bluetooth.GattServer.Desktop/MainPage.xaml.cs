using Jenx.Bluetooth.GattServer.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Jenx.Bluetooth.GattServer.Desktop
{
    public sealed partial class MainPage : Page
    {
       
        private ILogger _logger;
        private IGattServer _gattServer;
       
       

       

        private string path = "C:\\sebin\\lab\\ecg2\\data\\segment\\07_nsv_lead2_144_baselineRemove_resmaple8000\\files\\105\\ann\\V\\105_48.csv";
        Windows.Storage.StorageFile f;
        
        private IBuffer buffer;

        public MainPage()
        {
            InitializeComponent();
            InitializeLogger();
            InitializeGattServer();
            initializeFileAsync();


        }
        

        private void InitializeLogger()
        {
            _logger = new ControlLogger(LogTextBox);
        }

        private void InitializeGattServer()
        {
            _gattServer = new Common.GattServer(GattCharacteristicIdentifiers.ServiceId, _logger);
            _gattServer.OnChararteristicWrite += _gattServer_OnChararteristicWrite; ;
        }

        private async Task initializeFileAsync()
        {
            Windows.Storage.StorageFolder storageFolder =Windows.Storage.ApplicationData.Current.LocalFolder;
            f =await storageFolder.GetFileAsync(path);
            if(f!= null)
            {
                buffer = await Windows.Storage.FileIO.ReadBufferAsync(f);
            }
            else
            {
                Console.WriteLine("dfdf");
            }
           
        }

        private async void _gattServer_OnChararteristicWrite(object myObject, CharacteristicEventArgs myArgs)
        {
            await _logger.LogMessageAsync($"Characteristic with Guid: {myArgs.Characteristic.ToString()} changed: {myArgs.Value.ToString()}");
        }

        private async void StartGattServer_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                await _gattServer.Initialize();
            }
            catch
            {
                return;
            }
           
      
            await _gattServer.AddNotifyCharacteristicAsync(GattCharacteristicIdentifiers.FirmwareVersion,"MitBihECG");
           
            _gattServer.Start();
            await Task.Delay(2000);
           _gattServer.runNotifyCharaterstic((Windows.Storage.Streams.Buffer)buffer);
        }

        private void StopGattServer_Click(object sender, RoutedEventArgs e)
        {
            _gattServer.Stop();
        }



        private void advertiseGattServer()
        {
            double[] data = new double[50];
            DataWriter writer = new DataWriter();
           
            for(int i=0; i<50; i++)
            {
                //data[i] = System.Double.Parse(f.ReadLine());
            }
            
            //_gattServer.runNotifyCharaterstic(data);
        }
    }
}