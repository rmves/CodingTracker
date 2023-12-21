using System;

namespace CodingTracker
{
    public static class MainMenu
    {
        public static void DisplayMenu()
        {

            while (true)
            {
                Console.WriteLine("Welcome to the Main Menu");
                Console.WriteLine("0 - Close Application");
                Console.WriteLine("1 - View All Data");
                Console.WriteLine("2 - Insert Data");
                Console.WriteLine("3 - Update Data");
                Console.WriteLine("4 - Delete Data");

                int userInput = UserInput.GetIntegerInput("Select an option from the list above: ");

                switch (userInput)
                {
                    case 0:
                        Environment.Exit(0);
                        break;
                    case 1:
                        Console.Clear();
                        DatabaseManager.ViewAllRecords();
                        ReturnToMainMenu();
                        break;
                    case 2:
                        Console.Clear();
                        DatabaseManager.Insert();
                        break;
                    case 3:
                        Console.Clear();
                        DatabaseManager.Update();
                        break;
                    case 4:
                        Console.Clear();
                        DatabaseManager.Delete();
                        break;
                    default:
                        Console.WriteLine("Invalid option. Please choose a valid option.");
                        break;
                }
            }
        }

        public static void ReturnToMainMenu()
        {
            Console.WriteLine("Press any key to return to the main menu...");
            Console.ReadKey();
            Console.Clear();
        }
    }
}
