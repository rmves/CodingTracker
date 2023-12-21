using System;

namespace CodingTracker
{
    public static class MainMenu
    {
        public static void DisplayMenu()
        {
            int userInput;

            do
            {
                Console.WriteLine("Welcome to the Main Menu");
                Console.WriteLine("0 - Close Application");
                Console.WriteLine("1 - View All Data");
                Console.WriteLine("2 - Insert Data");
                Console.WriteLine("3 - Update Data");
                Console.WriteLine("4 - Delete Data");

                userInput = UserInput.GetIntegerInput("Select an option from the list above: ");

                switch (userInput)
                {
                    case 0:
                        Environment.Exit(0);
                        break;
                    case 1:
                        DatabaseManager.ViewAllRecords();
                        break;
                    case 2:
                        Console.WriteLine("Insert");
                        break;
                    case 3:
                        Console.WriteLine("Update");
                        break;
                    case 4:
                        Console.WriteLine("Delete");
                        break;
                    default:
                        Console.WriteLine("Invalid option. Please choose a valid option.");
                        break;
                }
            } while (userInput != 0);
        }
    }
}
