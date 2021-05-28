using System;
using System.IO;
using System.Linq;
using PasswordManager.AES;

namespace PasswordManager
{
    //C# 9 Required
    //Todo
    //Encrpy global key
    //Stop view all passwords from crashing, when no passwords exist
    //Comment some code

    class Entry
    {
        public static byte[] hashCheck;

        static bool CheckArrayEqual<T>(T[] first, T[]second)
        {
            return first.SequenceEqual(second);
        }

        static void InitaliseFiles()
        {
            if (!File.Exists("data.json"))
            {
                File.Create("data.json").Close();
            }
            if (!File.Exists("config.dat"))
            {
                File.Create("config.dat").Close();
            }
        }

        static void InitaliseMaster()
        {
            //First check if the file actually exists
            if (File.Exists("config.dat"))
            {
                //Read the hashed password and store it to hashCheck
                StreamReader reader = new(File.OpenRead("config.dat"));
                hashCheck = Convert.FromBase64String(reader.ReadLine());
                reader.Close();
                return;
            }
            //Encrypt the string password and store it to the config file
            Console.WriteLine("No Master Password Found. Please Enter A Password");
            hashCheck = Encrypt.EncryptString(MainMenu.globalKey, new byte[16], Text.MaskInput());
            StreamWriter writer = new(File.Open("config.dat", FileMode.Create));
            writer.WriteLine(Convert.ToBase64String(hashCheck));
            writer.Close();
            MainMenu.Menu();
        }

        static void Main()
        {
            Console.Title = "Password Manager!";

            InitaliseFiles();
            InitaliseMaster();

            while (true)
            {
                Console.WriteLine("Please Enter The Master Password:");
                string input = Text.MaskInput();
                byte[] inputHashed = Encrypt.EncryptString(MainMenu.globalKey, new byte[16], input);

                if (CheckArrayEqual(inputHashed, hashCheck))
                {
                    MainMenu.Menu();
                }
                else
                {
                    Console.Clear();
                    Console.WriteLine("Incorrect Password");
                }
            }
        }
    }
}
