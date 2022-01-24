using Jenx.Bluetooth.GattServer.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
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





        private string path = @"C:\sebin\sebin_data";
        private String ecgString;
        private byte[] ecgBytes;
        private int flag;

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
            var folder = await StorageFolder.GetFolderFromPathAsync(path);
            var file = await folder.GetFileAsync("103_2.csv");
            ecgString = await FileIO.ReadTextAsync(file);
            ecgBytes = Encoding.UTF8.GetBytes(ecgString);

            flag = 0;


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


            advertiseGattServerAsync();
           
        }

        private void StopGattServer_Click(object sender, RoutedEventArgs e)
        {
            _gattServer.Stop();
        }



        private async Task advertiseGattServerAsync()
        {   


            while (true)
            {
                var start = flag * 50;
     
               
                
                ArraySegment<byte> ecg = new ArraySegment<byte>(ecgBytes,start,50);
      
                _gattServer.runNotifyCharaterstic(ecg.ToArray().AsBuffer());
                await Task.Delay(10);
                flag += 1;
            }

          
           
        }


    }


}