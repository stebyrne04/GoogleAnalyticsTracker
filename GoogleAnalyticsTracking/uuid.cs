using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Management;
using System.IO;
using System.Net.NetworkInformation;

namespace GoogleAnalyticsTracking
{
    class uuid
    {

        static string _uuid = null;

        public static string Uuid
        {
            get
            {
                if (_uuid == null)
                {
                    uuid start = new uuid();
                }
                return uuid._uuid;
            }

        }

        public uuid() {

            uuid._uuid = GetUniqueID("");
        }

        private string GetUniqueID(string drive)
        {
            if (drive == string.Empty)
            {
                //Find first drive
                foreach (DriveInfo compDrive in DriveInfo.GetDrives())
                {
                    if (compDrive.IsReady)
                    {
                        drive = compDrive.RootDirectory.ToString();
                        break;
                    }
                }
            }

            if (drive.EndsWith(":\\"))
            {
                //C:\ -> C
                drive = drive.Substring(0, drive.Length - 2);
            }

            string volumeSerial = GetVolumeSerial(drive);
            string cpuID = GetCPUID();
            string macAddress = GetMACAddress();

            return macAddress.Substring(0, 4) + "" + cpuID.Substring(4, 4) + "-" + macAddress.Substring(8) + "-" + cpuID.Substring(1, 4)+ "-5043-" + volumeSerial + "" + macAddress.Substring(4, 4);

        }

        private string GetVolumeSerial(string drive)
        {
            ManagementObject disk = new ManagementObject(@"win32_logicaldisk.deviceid=""" + drive + @":""");
            disk.Get();

            string volumeSerial = disk["VolumeSerialNumber"].ToString();
            disk.Dispose();

            return volumeSerial;
        }

        private string GetCPUID()
        {
            string cpuInfo = "";
            ManagementClass managClass = new ManagementClass("win32_processor");
            ManagementObjectCollection managCollec = managClass.GetInstances();

            foreach (ManagementObject managObj in managCollec)
            {
                if (cpuInfo == "")
                {
                    //Get only the first CPU's ID
                    cpuInfo = managObj.Properties["processorID"].Value.ToString();
                    break;
                }
            }

            return cpuInfo;
        }
        public string GetMACAddress()
        {
            string mac = "";
            foreach (NetworkInterface nic in NetworkInterface.GetAllNetworkInterfaces())
            {

                if (nic.OperationalStatus == OperationalStatus.Up && (!nic.Description.Contains("Virtual") && !nic.Description.Contains("Pseudo")))
                {
                    if (nic.GetPhysicalAddress().ToString() != "")
                    {
                        mac = nic.GetPhysicalAddress().ToString();
                    }
                }
            }
            return mac;
        }

    }
}
