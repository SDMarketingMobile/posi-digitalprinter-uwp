using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Devices.SerialCommunication;
using Windows.Devices.Enumeration;

namespace POSIDigitalPrinter.Printer
{

    public sealed class PrinterImpl
    {
        public async Task<List<SerialDevice>> ListSerialPort()
        {
            string aqs = SerialDevice.GetDeviceSelector();
            DeviceInformationCollection deviceInfoCollection = await DeviceInformation.FindAllAsync(aqs);
            List<SerialDevice> devs = new List<SerialDevice>();
            foreach (DeviceInformation devInfo in deviceInfoCollection)
            {
                SerialDevice sd = await SerialDevice.FromIdAsync(devInfo.Id);
                if (sd != null)
                {
                    devs.Add(sd);
                }
            }
            return devs;
        }
    }
}
