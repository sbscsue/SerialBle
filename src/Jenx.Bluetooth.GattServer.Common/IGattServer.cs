using System;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using static Jenx.Bluetooth.GattServer.Common.GattServer;

namespace Jenx.Bluetooth.GattServer.Common
{
    public interface IGattServer
    {
        event GattChararteristicHandler OnChararteristicWrite;
        Task Initialize();

        Task<bool> AddNotifyCharacteristicAsync(Guid characteristicId,string userDescription);
        void setNotifyCharaterisitic(GattLocalCharacteristicResult gattResult);
        void runNotifyCharaterstic(Windows.Storage.Streams.Buffer value);
        bool getNotifyCharateristicStateAsync();

        Task<bool> AddReadCharacteristicAsync(Guid characteristicId, string characteristicValue, string userDescription = "N/A");
        Task<bool> AddWriteCharacteristicAsync(Guid characteristicId, string userDescription = "N/A");
        Task<bool> AddReadWriteCharacteristicAsync(Guid characteristicId, string userDescription = "N/A");

        void Start();
        void Stop();
    }
}