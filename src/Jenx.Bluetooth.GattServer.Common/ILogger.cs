﻿using System;
using System.Threading.Tasks;

namespace Jenx.Bluetooth.GattServer.Common
{
    public interface ILogger
    {
        Task LogMessageAsync(string message);
        //Task LogMessageAsync(Func<string> toString);
    }
}