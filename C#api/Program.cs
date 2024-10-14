using System.Net;
public class Program
{
    private const int PORT = 3000;

    public static void Main()
    {
        var httpListener = new HttpListener();
        httpListener.Prefixes.Add($"http://localhost:{PORT}/");

        AuthProvider.Init(); // Initialize authentication provider
        DataProvider.Init(); // Initialize data provider
        NotificationSystem n = new NotificationSystem();
        NotificationSystem.Start(); // Start notification processor

        httpListener.Start();
        Console.WriteLine($"Serving on port {PORT}...");

        Task.Run(async () =>
        {
            while (true)
            {
                var context = await httpListener.GetContextAsync();
                var handler = new ApiRequestHandler();
                handler.HandleGet(context.Request, context.Response);
            }
        });

        // Prevent the main thread from exiting
        Console.ReadLine();
        httpListener.Stop();
    }
}
