using System;

namespace PasswordManager
{
    class Text
    {
        public static string MaskInput()
        {
            string input = "";
            ConsoleKeyInfo key = new();

            while (key.Key != ConsoleKey.Enter)
            {
                key = Console.ReadKey(intercept: true);

                if (key.Key == ConsoleKey.Backspace && input.Length > 0)
                {
                    Console.Write("\b \b");
                    input = input[0..^1];
                }
                else if (!char.IsControl(key.KeyChar))
                {
                    Console.Write("*");
                    input += key.KeyChar;
                }

                if (key.Key == ConsoleKey.Enter)
                {
                    if (input.Length <= 0)
                    {
                        Console.WriteLine("Password must be more than 0 characters long");
                        key = new();
                    }
                }
            }

            return input;
        }
    }
}
