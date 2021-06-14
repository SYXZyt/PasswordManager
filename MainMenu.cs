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
        public static readonly Random rng = new();

        public static readonly byte[] globalKey = new byte[] { 251, 23, 165, 16, 255, 163, 208, 10, 20, 9, 31, 176, 1, 18, 14, 223 };

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
                Console.WriteLine("║6) Backup data file      ║");
                Console.WriteLine("║7) Exit                  ║");
                Console.WriteLine("╚═════════════════════════╝");

                string input = Console.ReadLine();

                switch (input)
                {
                    case "1":
                        MainFunctions.AddNewPass();
                        break;
                    case "2":
                        MainFunctions.RemovePass();
                        break;
                    case "3":
                        MainFunctions.ReadAll();
                        break;
                    case "4":
                        MainFunctions.ChangeMaster();
                        break;
                    case "5":
                        Console.WriteLine("Password length:");
                        MainFunctions.GeneratePassword(uint.Parse(Console.ReadLine()));
                        break;
                    case "6":
                        MainFunctions.Backup();
                        break;
                    case "7":
                        Environment.Exit(0);
                        break;
                }
            }
        }
    }
}
