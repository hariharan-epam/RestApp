
namespace RestApp
{
    public class Logger : ILogger
    {
        public void Error(Exception exception)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"[ERROR] {exception.Message}");
            Console.ResetColor();
        }
    }
}
