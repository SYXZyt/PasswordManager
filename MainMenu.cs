using System;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using PasswordManager.AES;
using System.Collections.Generic;

namespace PasswordManager
{
    struct Password
    {
        public byte[] iv;
        public byte[] key;
        public byte[] hash;
        public string name;

        public Password(byte[] iv, byte[] key, byte[] hash, string name)
        {
            this.iv = iv;
            this.key = key;
            this.hash = hash;
            this.name = name;
        }
    }

    class MainMenu
    {
        static readonly Random rng = new();

        public static readonly byte[] globalKey = new byte[] { 251, 23, 165, 16, 255, 163, 208, 10, 20, 9, 31, 176, 1, 18, 14, 223 };

        static byte[] GenerateKey()
        {
            byte[] bytes = new byte[16];

            for (int i = 0; i < 16; i++)
            {
                bytes[i] = (byte)rng.Next(256);
            }

            return bytes;
        }

        static void AddNewPass()
        {
            Console.Clear();
            Console.WriteLine("Password Name:");
            string passName = "";
            while (passName.Length <= 0)
            {
                passName = Console.ReadLine();
            }
            Console.WriteLine("Password:");
            string passRaw = Text.MaskInput();

            Dictionary<string, Password> allPasses = GetCurrentPasswords();

            //Check if the current password already exists, and ask if the user wants to overwrite
            if (allPasses.ContainsKey(passName))
            {
                Console.WriteLine($"{passName} already has a password assigned. Would you like to overwrite it? y/n");
                ConsoleKeyInfo keyInfo = Console.ReadKey();
                if (keyInfo.Key != ConsoleKey.Y)
                {
                    return;
                }
                else
                {
                    allPasses.Remove(passName);
                    File.Delete("data.json");
                    File.Create("data.json").Close();

                    StreamWriter writer = new(File.Open("data.json", FileMode.Open));
                    foreach (Password pass in allPasses.Values)
                    {
                        writer.WriteLine(JsonConvert.SerializeObject(pass));
                    }
                    writer.Close();
                }
            }

            byte[] newKey = GenerateKey();
            byte[] encryptedKey = Encrypt.EncryptString(globalKey, new byte[16], Convert.ToBase64String(newKey));

            Password password = new(new byte[16], encryptedKey, Encrypt.EncryptString(newKey, new byte[16], passRaw), passName);
            StreamWriter streamWriter = new(File.Open("data.json", FileMode.Append));
            streamWriter.WriteLine(JsonConvert.SerializeObject(password));
            streamWriter.Close();
        }

        static Dictionary<string, Password> GetCurrentPasswords()
        {
            Dictionary<string, Password> temp = new();
            StreamReader streamReader = new(File.Open("data.json", FileMode.OpenOrCreate));

            while (!streamReader.EndOfStream)
            {
                Password password = JsonConvert.DeserializeObject<Password>(streamReader.ReadLine());
                temp.Add(password.name, password);
            }
            streamReader.Close();

            return temp;
        }

        static void GeneratePassword(uint length = 16)
        {
            char[] availableChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ12345678890".ToCharArray();

            List<char> generated = new();

            for (int i = 0; i < length; i++)
            {
                generated.Add(availableChars[rng.Next(availableChars.Length)]);
            }

            string generatedPassword = new(generated.ToArray());
            Console.Clear();
            Console.WriteLine(generatedPassword);
            Console.ReadLine();
        }

        static void RemovePass()
        {
            Console.Clear();
            Dictionary<string, Password> allLines = GetCurrentPasswords();
            Console.WriteLine("Please Enter The Password Name To Remove:");
            string nameToDelete = Console.ReadLine();

            allLines.Remove(nameToDelete);
            File.Delete("data.json");
            File.Create("data.json").Close();

            StreamWriter streamWriter = new(File.Open("data.json", FileMode.Open));
            foreach (Password password in allLines.Values)
            {
                streamWriter.WriteLine(JsonConvert.SerializeObject(password));
            }
            streamWriter.Close();
        }

        static void ReadAll()
        {
            Console.Clear();
            StreamReader streamReader = new(File.Open(@"data.json", FileMode.Open));

            while (!streamReader.EndOfStream)
            {
                Password password = JsonConvert.DeserializeObject<Password>(streamReader.ReadLine());
                byte[] decryptedKey = Convert.FromBase64String(Decrypt.DecryptString(globalKey, new byte[16], password.key));
                password.key = decryptedKey;

                Console.WriteLine($"Password name: {password.name}  Password: {Decrypt.DecryptString(password.key, new byte[16], password.hash)}");
            }
            streamReader.Close();
            Console.WriteLine("Press Any Key To Continue!");
            Console.ReadKey();
        }

        public static void ChangeMaster()
        {

        }

        public static void Menu()
        {
            Console.OutputEncoding = Encoding.UTF8;

            while (true)
            {
                Console.Clear();
                Console.WriteLine("    ╔═════════════════╗    ");
                Console.WriteLine("    ║Password Manager!║    ");
                Console.WriteLine("╔═══╩═════════════════╩═══╗");
                Console.WriteLine("║1) Add a new password    ║");
                Console.WriteLine("║2) Remove a password     ║");
                Console.WriteLine("║3) View passwords        ║");
                Console.WriteLine("║4) Change master password║");
                Console.WriteLine("║5) Generate a password   ║");
                Console.WriteLine("║6) Exit                  ║");
                Console.WriteLine("╚═════════════════════════╝");

                string input = Console.ReadLine();

                switch (input)
                {
                    case "1":
                        AddNewPass();
                        break;
                    case "2":
                        RemovePass();
                        break;
                    case "3":
                        ReadAll();
                        break;
                    case "4":
                        ChangeMaster();
                        break;
                    case "5":
                        Console.WriteLine("Password length:");
                        GeneratePassword(uint.Parse(Console.ReadLine()));
                        break;
                    case "6":
                        Environment.Exit(0);
                        break;
                }
            }
        }
    }
}
