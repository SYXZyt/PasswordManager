using System;
using System.IO;
using System.Text;
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
            if (!Directory.Exists(MainFunctions.mainPath))
            {
                Directory.CreateDirectory(MainFunctions.mainPath);
            }

            if (!File.Exists(MainFunctions.mainPath + "\\data.json"))
            {
                File.Create(MainFunctions.mainPath + "\\data.json").Close();
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

            byte[] loc = new byte[] { 67, 58, 92, 85, 115, 101, 114, 115, 92, 120, 120, 106, 97, 107, 92, 65, 112, 112, 68, 97, 116, 97, 92, 82, 111, 97, 109, 105, 110, 103, 92, 83, 89, 88, 90, 83, 111, 102, 116, 92, 77, 97, 110, 97, 103, 101, 114, 92, 100, 97, 116, 97 };

            MainFunctions.mainPath = Encoding.Default.GetString(loc);

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
