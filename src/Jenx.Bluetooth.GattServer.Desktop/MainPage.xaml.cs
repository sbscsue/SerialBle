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



        private string folderPath = @"C:\06_nsv_lead2_160_baselineRemove\annotation\N";
        private string folderPath2 = @"C:\06_nsv_lead2_160_baselineRemove\annotation\S";
        private string folderPath3 = @"C:\06_nsv_lead2_160_baselineRemove\annotation\V";
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
            
            n = new segment(check);
            s = new segment(folderPath2);
            v = new segment(folderPath3);
            await n.getFileQueueAsync();
            await s.getFileQueueAsync();
            await v.getFileQueueAsync();

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
                _gattServer.runNotifyCharaterstic(n.popDataAsync().Result.AsBuffer());
                await Task.Delay(125);
            }
            
        }


       


     

    }


}