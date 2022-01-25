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

namespace Jenx.Bluetooth.GattServer.Desktop
{
    public sealed partial class MainPage : Page
    {
       

        private ILogger _logger;
        private IGattServer _gattServer;


        private const int sendLength = 50;
        private const int bleSendCnt = 30 * 60 * 400 * 8;


        private string path = @"C:\sebin\lab\ecg2\data\all_mitbih\01)400fs";
        private String ecgString;
        private float[] ecgfloat;
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
            var file = await folder.GetFileAsync("203.csv");
            ecgString = await FileIO.ReadTextAsync(file);

            ecgfloat = parsingStringCsvToFloat(ecgString);


            flag = 0;


        }

        private float[] parsingStringCsvToFloat(String data)
        {
            String[] dirtySplitSample = data.Split("\r\n");
            String[] splitSample = dirtySplitSample.Take(dirtySplitSample.Length - 1).ToArray();

            float[] ecgFloat = new float[splitSample.Length];
            for (int i = 0; i < splitSample.Length; i++)
            {
                ecgFloat[i] = Convert.ToSingle(splitSample[i]);      
            }

            return ecgFloat;

            
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


            while (true)
            {
                var start = flag * sendLength;

                
                _gattServer.runNotifyCharaterstic(floatToString(ecgfloat,start,sendLength).AsBuffer());
                await Task.Delay(125);
                flag += 1;
            }

        }

        private byte[] floatToString(float[] data,int start,int length) {
            ArraySegment<float> ecgSeg = new ArraySegment<float>(data, start, length);
            string all = string.Join(",", ecgSeg.ToArray());

            byte[] StrByte = Encoding.UTF8.GetBytes(all);

            return StrByte;
        }

        private void StopGattServer_Click(object sender, RoutedEventArgs e)
        {
            _gattServer.Stop();
        }



        private async Task advertiseGattServerAsync()
        {   


            while (true)
            {
                var start = flag * sendLength;

                if (flag != bleSendCnt)
                {
                    ArraySegment<byte> ecg = new ArraySegment<byte>(ecgBytes, start, sendLength);

                    _gattServer.runNotifyCharaterstic(ecg.ToArray().AsBuffer());
                    await Task.Delay(10);
                    flag += 1;

                }
                else
                {
                    break;
                }





            }

          
           
        }


    }


}