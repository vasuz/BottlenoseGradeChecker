using System;
using System.Security;

namespace BottlenoseGradeChecker
{
    /// <summary>
    /// Utility methods.
    /// </summary>
    public class Utils
    {
        /// <summary>
        /// Prompts the user to securely enter a password via console.
        /// </summary>
        /// <param name="displayMessage">The message to prompt the user with.</param>
        /// <returns>The password the user enters.</returns>
        public SecureString GetPasswordFromConsole(string displayMessage)
        {
            Console.WriteLine(displayMessage);

            SecureString pass = new SecureString();
            ConsoleKeyInfo key;

            Console.Write("*");
            do
            {
                key = Console.ReadKey(true);

                // Backspace should not work
                if (!char.IsControl(key.KeyChar))
                {
                    pass.AppendChar(key.KeyChar);
                }
                else
                {
                    if (key.Key == ConsoleKey.Backspace && pass.Length > 0)
                    {
                        pass.RemoveAt(pass.Length - 1);
                        Console.Write("\b \b");
                    }
                }
            }
            // Stops receiving keys once enter is pressed
            while (key.Key != ConsoleKey.Enter);

            Console.WriteLine();

            return pass;
        }
    }
}
