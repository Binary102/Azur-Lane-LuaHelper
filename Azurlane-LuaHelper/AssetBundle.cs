using System;
using System.IO;
using System.Reflection;

namespace Azurlane
{
    internal static class AssetBundle
    {
        private static readonly byte[] Decrypted, Encrypted;
        private static readonly object Instance;

        static AssetBundle()
        {
            if (Decrypted == null)
            {
                Decrypted = new byte[] {
                    0x55, 0x6E, 0x69, 0x74, 0x79, 0x46, 0x53, 0x00,
                    0x00, 0x00, 0x00, 0x06, 0x35, 0x2E, 0x78, 0x2E
                };
            }

            if (Encrypted == null)
            {
                Encrypted = new byte[] {
                    0xC7, 0xD5, 0xFC, 0x1F, 0x4C, 0x92, 0x94, 0x55,
                    0x85, 0x03, 0x16, 0xA3, 0x7F, 0x7B, 0x8B, 0x55
                };
            }

            if (Instance == null)
            {
                var assembly = Assembly.Load(Properties.Resources.Salt);
                Instance = Activator.CreateInstance(assembly.GetType("LL.Salt"));
            }
        }

        internal static bool Compare(byte[] b1, byte[] b2)
        {
            for (var i = 0; i < b2.Length; i++)
            {
                if (b1[i] != b2[i])
                    return false;
            }
            return true;
        }

        internal static void Run(string path, Tasks task)
        {
            var bytes = File.ReadAllBytes(path);

            Console.Write("[+] Checking AssetBundle...");
            if (Compare(bytes, Encrypted))
            {
                Console.Write(" <Encrypted>\n");
                if (task == Tasks.Encrypt)
                {
                    Console.WriteLine("[+] AssetBundle is already encrypted... <Aborted>");
                    return;
                }
                else if (task == Tasks.Unpack || task == Tasks.Repack)
                {
                    Console.WriteLine("[+] You cannot unpack/repack an encrypted AssetBundle... <Aborted>");
                    return;
                }
            }
            else if (Compare(bytes, Decrypted))
            {
                Console.Write(" <Decrypted>\n");
                if (task == Tasks.Decrypt)
                {
                    Console.WriteLine("[+] AssetBundle is already decrypted... <Aborted>");
                    return;
                }
            }
            else
            {
                Console.Write(" <Unknown>\n");
                Console.WriteLine("[+] Not a valid or damaged AssetBundle file... <Aborted>");
                return;
            }

            Console.Write(string.Format("[+] {0} {1}...", task == Tasks.Decrypt ? "Decrypting" : task == Tasks.Encrypt ? "Encrypting" : task == Tasks.Unpack ? "Unpacking" : "Repacking", Path.GetFileName(path)));
            if (task == Tasks.Decrypt || task == Tasks.Encrypt)
            {
                var method = Instance.GetType().GetMethod("Make", BindingFlags.Static | BindingFlags.Public);
                bytes = (byte[])method.Invoke(Instance, new object[] { bytes, task == Tasks.Encrypt });

                File.WriteAllBytes(path + (task == Tasks.Encrypt ? "_enc" : "_dec"), bytes);
            }
            else if (task == Tasks.Unpack || task == Tasks.Repack)
            {
                Utils.Command(string.Format("UnityEX.exe {0} \"{1}\"", task == Tasks.Unpack ? "export" : "import", path));
            }
            Console.Write(" <Done>\n");
            Program.isInvalid = false;
        }
    }
}