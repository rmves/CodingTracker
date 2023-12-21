using System;

namespace CodingTracker
{
    public class UserInput
    {
        public static int GetIntegerInput(string message)
        {
            int userInput;
            bool isValidInput;

            do
            {
                Console.WriteLine(message);
                isValidInput = int.TryParse(Console.ReadLine(), out userInput);

                if (!isValidInput)
                {
                    Console.WriteLine("Please enter a valid integer.");
                }
            } while (!isValidInput);

            return userInput;
        }

        public static string GetStringInput(string message)
        {
            Console.WriteLine(message);
            return Console.ReadLine();
        }
    }

}
