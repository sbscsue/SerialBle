using Jenx.Bluetooth.GattServer.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using segmentRead;


namespace Jenx.Bluetooth.GattServer.Desktop
{
    public sealed partial class MainPage : Page
    {
       

        private ILogger _logger;
        private IGattServer _gattServer;

        private segment n;
        private segment s;
        private segment v;



        private string path = @"C:\sebin\lab\ecg2\data\segmentAll\01)NSV_320";
      
        private string check = @"C:\sebin";

        public MainPage()
        {
            InitializeComponent();
            InitializeLogger();
            InitializeGattServer();
            initializePath();


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

        private async void initializePath()
        {
            //관련 객체 생성 n/s/v
            
            //n = new segment(path,"N.csv");
            //await n.setFile();
            s = new segment(path,"S.csv");
            await s.setFile();
            v = new segment(path,"V.csv");
            await v.setFile();


        }

        private segment segmentRead(object p)
        {
            throw new NotImplementedException();
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
         

         
        }

        private void StopGattServer_Click(object sender, RoutedEventArgs e)
        {
            _gattServer.Stop();
        }


        private async void AdvertiseGattServer_Click(object sender, RoutedEventArgs e)
        {
            while (true)
            {
                _gattServer.runNotifyCharaterstic(s.popData().AsBuffer());
                await Task.Delay(100);
            }
            
        }


       


     

    }


}