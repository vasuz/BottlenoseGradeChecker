using System;

namespace BottlenoseGradeChecker
{
    public static class Program
    {
        /// <summary>
        /// Entry point for the program.
        /// 
        /// Arguments in the form of:
        /// [0] - Username
        /// [1] - Password (ciphered)
        /// Can be specified in order to bypass credential prompts.
        /// </summary>
        /// <param name="args">Optional username/password input.</param>
        static void Main(string[] args)
        {
            string user;
            string pass;

            if (args.Length == 2)
            {
                user = args[0];
                pass = args[1].Rot13();
            }
            else
            {
                Console.WriteLine("Enter username:");
                user = Console.ReadLine();
                pass = new System.Net.NetworkCredential(string.Empty, new Utils().GetPasswordFromConsole("Enter password:")).Password;
            }

            Web.Login(user, pass);
            Console.ReadLine();
        }
    }
}