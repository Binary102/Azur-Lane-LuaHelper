using System;
using System.IO;
using System.Text;

namespace Azurlane
{
    internal static class Lua
    {
        internal static uint ReadUleb128(this BinaryReader reader)
        {
            uint value = reader.ReadByte();
            if (value >= 0x80)
            {
                var bitshift = 0;
                value &= 0x7f;
                while (true)
                {
                    var b = reader.ReadByte();
                    bitshift += 7;
                    value |= (uint)((b & 0x7f) << bitshift);
                    if (b < 0x80)
                        break;
                }
            }
            return value;
        }

        internal static void Run(string path, Tasks task)
        {
            var bytes = File.ReadAllBytes(path);

            Console.Write(string.Format("[+] Checking {0}...", Path.GetFileName(path)));
            if (bytes[3] == 0x80)
            {
                Console.Write(" <Encrypted>\n");
                if (task == Tasks.Encrypt)
                {
                    Console.WriteLine(string.Format("[+] {0} is already encrypted... <Aborted>", Path.GetFileName(path)));
                    return;
                }
                else if (task == Tasks.Decompile || task == Tasks.Recompile)
                {
                    Console.WriteLine("[+] You cannot decompile an encrypted lua... <Aborted>");
                    return;
                }
            }
            else if (bytes[3] == 0x02)
            {
                Console.Write(" <Decrypted>\n");
                if (task == Tasks.Decrypt)
                {
                    Console.WriteLine(string.Format("[+] {0} is already decrypted... <Aborted>", Path.GetFileName(path)));
                    return;
                }
            }
            else if (task != Tasks.Recompile)
            {
                Console.Write(" <Unknown>\n");
                Console.WriteLine("[+] Not a valid or damaged lua file... <Aborted>");
                return;
            }
            else
            {
                Console.Write(" <OK>\n");
            }

            Console.Write($"[+] {(task == Tasks.Decrypt ? "Decrypting" : task == Tasks.Encrypt ? "Encrypting" : task == Tasks.Decompile ? "Decompiling" : "Recompiling")} {Path.GetFileName(path)}...");
            var luaPath = Path.Combine(PathMgr.Environment(task == Tasks.Decrypt ? "Decrypted_lua" : task == Tasks.Encrypt ? "Encrypted_lua" : task == Tasks.Decompile ? "Decompiled_lua" : "Recompiled_lua"), Path.GetFileName(path));

            try
            {
                if (task == Tasks.Decrypt || task == Tasks.Encrypt)
                {
                    using (var reader = new BinaryReader(new MemoryStream(bytes)))
                    {
                        var magic = reader.ReadBytes(3);
                        var version = reader.ReadByte();
                        var bits = reader.ReadUleb128();

                        var is_stripped = ((bits & 2u) != 0u);
                        if (!is_stripped)
                        {
                            var length = reader.ReadUleb128();
                            var name = Encoding.UTF8.GetString(reader.ReadBytes((int)length));
                        }

                        while (reader.BaseStream.Position < reader.BaseStream.Length)
                        {
                            var size = reader.ReadUleb128();

                            if (size == 0)
                                break;

                            var next = reader.BaseStream.Position + size;
                            bits = reader.ReadByte();

                            var arguments_count = reader.ReadByte();
                            var framesize = reader.ReadByte();
                            var upvalues_count = reader.ReadByte();
                            var complex_constants_count = reader.ReadUleb128();
                            var numeric_constants_count = reader.ReadUleb128();
                            var instructions_count = reader.ReadUleb128();
                            var start = (int)reader.BaseStream.Position;

                            if (task == Tasks.Encrypt)
                            {
                                bytes[3] = 0x80;
                                bytes = Lock(start, bytes, (int)instructions_count);
                            }
                            else
                            {
                                bytes[3] = 2;
                                bytes = Unlock(start, bytes, (int)instructions_count);
                            }

                            reader.BaseStream.Position = next;
                        }
                    }
                    if (File.Exists(luaPath)) File.Delete(luaPath);
                    File.WriteAllBytes(luaPath, bytes);
                }
                else if (task == Tasks.Decompile || task == Tasks.Recompile)
                {
                    Utils.Command(task == Tasks.Decompile ? $"python main.py -f \"{path}\" -o \"{luaPath}\"" : $"luajit.exe -b \"{path}\" \"{luaPath}\"");
                }
                Console.Write("<done>\n");
                Program.isInvalid = false;
            }
            catch (Exception e)
            {
                Utils.ExceptionLogger($"Exception detected during {(task == Tasks.Decrypt ? "decrypting" : task == Tasks.Encrypt ? "encrypting" : task == Tasks.Decompile ? "decompiling" : "recompiling")} {Path.GetFileName(path)}", e);
            }
        }

        private static byte[] Lock(int start, byte[] bytes, int count)
        {
            var result = start;
            result += 4;
            var v2 = 0;
            do
            {
                var v3 = bytes[result - 4];
                result += 4;
                var v4 = bytes[result - 7] ^ v2++;
                bytes[result - 8] = (byte)(Properties.Resources.Lock[v3] ^ v4);
            }
            while (v2 != count);
            return bytes;
        }

        private static byte[] Unlock(int start, byte[] bytes, int count)
        {
            var result = start;
            result += 4;
            var v2 = 0;
            do
            {
                var v3 = bytes[result - 4];
                result += 4;
                var v4 = bytes[result - 7] ^ v3 ^ (v2++ & 0xFF);
                bytes[result - 8] = Properties.Resources.Unlock[v4];
            }
            while (v2 != count);
            return bytes;
        }
    }
}