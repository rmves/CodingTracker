using CodingTracker;

class Program
{
    static void Main(string[] args)
    {
        DatabaseManager.CreateDatabase();

        MainMenu. DisplayMenu();
    }
}