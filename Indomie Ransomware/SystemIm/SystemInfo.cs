using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Indomie_Ransomware.System
{
    internal class SystemInfo
    {
        public static string GetSystemMD5()
        {
            return GetMD5(GetHWID());   
        }
        static string GetHWID()
        {
            try
            {
                string processorId = "";
                string volumeSerial = "";

                // Lấy Processor ID
                using (var mc = new ManagementClass("Win32_Processor"))
                {
                    foreach (ManagementObject mo in mc.GetInstances())
                    {
                        processorId = mo.Properties["ProcessorId"].Value.ToString();
                        break;
                    }
                }

                // Lấy Volume Serial Number (C drive)
                using (var mo = new ManagementObject(@"win32_logicaldisk.deviceid=""C:"""))
                {
                    mo.Get();
                    volumeSerial = mo["VolumeSerialNumber"].ToString();
                }

                return processorId + volumeSerial;
            }
            catch
            {
                return "UNKNOWN_HWID";
            }
        }

        static string GetMD5(string input)
        {
            using (var md5 = MD5.Create())
            {
                byte[] inputBytes = Encoding.UTF8.GetBytes(input);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                // Chuyển sang chuỗi hexa
                StringBuilder sb = new StringBuilder();
                foreach (byte b in hashBytes)
                    sb.Append(b.ToString("x2"));

                return sb.ToString();
            }
        }
    }
}
