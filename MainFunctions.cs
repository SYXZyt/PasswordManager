using System;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using PasswordManager.AES;
using System.Collections.Generic;

namespace PasswordManager
{
    class MainFunctions
    {
        public static string mainPath;

        public static byte[] GenerateKey()
        {
            byte[] bytes = new byte[16];

            for (int i = 0; i < 16; i++)
            {
                bytes[i] = (byte)MainMenu.rng.Next(256);
            }

            return bytes;
        }

        public static void Backup()
        {
            if (File.Exists(mainPath + "\\bkg.backup"))
            {
                File.Delete(mainPath + "\\bkg.backup");
            }
            File.Copy(mainPath + "\\data.json", mainPath + "\\bkg.backup");
        }

        public static void AddNewPass()
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
                    File.Delete($"{mainPath}\\data.json");
                    File.Create($"{mainPath}\\data.json").Close();

                    StreamWriter writer = new(File.Open($"{mainPath}\\data.json", FileMode.Open));
                    foreach (Password pass in allPasses.Values)
                    {
                        writer.WriteLine(JsonConvert.SerializeObject(pass));
                    }
                    writer.Close();
                }
            }

            byte[] newKey = GenerateKey();
            byte[] encryptedKey = Encrypt.EncryptString(MainMenu.globalKey, new byte[16], Convert.ToBase64String(newKey));

            Password password = new(new byte[16], encryptedKey, Encrypt.EncryptString(newKey, new byte[16], passRaw), passName);
            StreamWriter streamWriter = new(File.Open($"{mainPath}\\data.json", FileMode.Append));
            streamWriter.WriteLine(JsonConvert.SerializeObject(password));
            streamWriter.Close();
        }

        public static Dictionary<string, Password> GetCurrentPasswords()
        {
            Dictionary<string, Password> temp = new();
            StreamReader streamReader = new(File.Open($"{mainPath}\\data.json", FileMode.OpenOrCreate));

            while (!streamReader.EndOfStream)
            {
                Password password = JsonConvert.DeserializeObject<Password>(streamReader.ReadLine());
                temp.Add(password.name, password);
            }
            streamReader.Close();

            return temp;
        }

        public static void GeneratePassword(uint length = 16)
        {
            char[] availableChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ12345678890!()?[]~_{};:*~#$%^+=".ToCharArray();

            List<char> generated = new();

            for (int i = 0; i < length; i++)
            {
                generated.Add(availableChars[MainMenu.rng.Next(availableChars.Length)]);
            }

            string generatedPassword = new(generated.ToArray());
            Console.Clear();
            Console.WriteLine(generatedPassword);
            Console.ReadLine();
        }

        public static void RemovePass()
        {
            Console.Clear();
            Dictionary<string, Password> allLines = GetCurrentPasswords();
            Console.WriteLine("Please Enter The Password Name To Remove:");
            string nameToDelete = Console.ReadLine();

            allLines.Remove(nameToDelete);
            File.Delete($"{mainPath}\\data.json");
            File.Create($"{mainPath}\\data.json").Close();

            StreamWriter streamWriter = new(File.Open($"{mainPath}\\data.json", FileMode.Open));
            foreach (Password password in allLines.Values)
            {
                streamWriter.WriteLine(JsonConvert.SerializeObject(password));
            }
            streamWriter.Close();
        }

        public static void ReadAll()
        {
            Console.Clear();
            StreamReader streamReader = new(File.Open(@$"{mainPath}\\data.json", FileMode.Open));

            while (!streamReader.EndOfStream)
            {
                Password password = JsonConvert.DeserializeObject<Password>(streamReader.ReadLine());
                byte[] decryptedKey = Convert.FromBase64String(Decrypt.DecryptString(MainMenu.globalKey, new byte[16], password.key));
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
    }
}
