using Android.Bluetooth;
using Application.Models;
using System;
using System.Collections.ObjectModel;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

// Добавляем перечисление для типов соединений
public enum ConnectionType
{
    None,
    Bluetooth,
    Wifi
}

namespace Application.Patterns.Singleton
{
    internal class ExhibitionManager
    {
        private static ExhibitionManager instance;

        public ObservableCollection<Exhibition> Items { get; set; }
        public ObservableCollection<Exhibition> CurrentItem { get; set; }

        public BluetoothSocket socket;
        public TcpClient wifiClient;

        // Параметры тестового сообщения. Можно настраивать в зависимости от требований.
        private const string bluetoothTestMessage = "PING_BT\n";
        private const string wifiTestMessage = "PING_WIFI\n";

        private ExhibitionManager()
        {
            Items = new ObservableCollection<Exhibition>();
            CurrentItem = new ObservableCollection<Exhibition>();
        }

        public static ExhibitionManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new ExhibitionManager();
                }
                return instance;
            }
        }

        // Метод, выбирающий наилучшее доступное соединение
        public async Task<ConnectionType> GetBestConnectionAsync()
        {
            if (await TestBluetoothConnectionAsync())
            {
                return ConnectionType.Bluetooth;
            }
            else if (await TestWifiConnectionAsync())
            {
                return ConnectionType.Wifi;
            }
            return ConnectionType.None;
        }

        // Унифицированный метод отправки данных с выбором канала
        private async Task WriteAsync(byte[] buffer, int offset, int count)
        {
            ConnectionType bestConnection = await GetBestConnectionAsync();

            switch (bestConnection)
            {
                case ConnectionType.Bluetooth:
                    await socket.OutputStream.WriteAsync(buffer, 0, buffer.Length);
                    break;
                case ConnectionType.Wifi:
                    await wifiClient.GetStream().WriteAsync(buffer, 0, buffer.Length);
                    break;
                case ConnectionType.None:
                    throw new Exception("Нет доступных соединений для отправки данных");
            }
        }

        // Метод отправки данных в виде строки, который использует WriteAsync
        public async Task SendDataAsync(string message)
        {
            message += "\n";
            byte[] buffer = Encoding.UTF8.GetBytes(message);
            await WriteAsync(buffer, 0, buffer.Length);
        }


        /// <summary>
        /// Проверяем Bluetooth-соединение, отправляя тестовое сообщение.
        /// Если отправка проходит без исключений, соединение считается работоспособным.
        /// Если выбрасывается исключение с сообщением "Broken pipe", возвращаем false.
        /// </summary>
        public async Task<bool> TestBluetoothConnectionAsync()
        {
            // Дополнительная проверка состояния
            if (socket == null || !socket.IsConnected)
                return false;
            byte[] testBuffer = Encoding.UTF8.GetBytes(bluetoothTestMessage);
            try
            {
                await socket.OutputStream.WriteAsync(testBuffer, 0, testBuffer.Length);
                return true;
            }
            catch (Exception ex)
            {
                // Если ошибка содержит "Broken pipe" – считаем соединение нерабочим
                if (ex.Message.Contains("Broken pipe"))
                    return false;

                // Для остальных исключений можно логировать или пробрасывать дальше
                return false;
            }
        }

        /// <summary>
        /// Проверяем Wi‑Fi соединение, отправляя тестовое сообщение.
        /// Если отправка проходит без исключений, соединение считается работоспособным.
        /// Если выбрасывается исключение "Broken pipe", возвращаем false.
        /// </summary>
        public async Task<bool> TestWifiConnectionAsync()
        {
            if (wifiClient == null || !wifiClient.Connected)
                return false;
            byte[] testBuffer = Encoding.UTF8.GetBytes(wifiTestMessage);
            try
            {
                await wifiClient.GetStream().WriteAsync(testBuffer, 0, testBuffer.Length);
                return true;
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("Broken pipe"))
                    return false;
                return false;
            }
        }
    }
}
