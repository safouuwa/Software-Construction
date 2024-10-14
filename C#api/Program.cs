using System;
using System.Net;
using System.Threading.Tasks;

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

                // Determine the HTTP method and call the appropriate handler
                switch (context.Request.HttpMethod)
                {
                    case "GET":
                        handler.HandleGet(context.Request, context.Response);
                        break;
                    case "POST":
                        handler.HandlePost(context.Request, context.Response);
                        break;
                    case "PUT":
                        //\ handler.HandlePut(context.Request, context.Response);
                        break;
                    case "DELETE":
                        handler.HandleDelete(context.Request, context.Response);
                        break;
                    default:
                        context.Response.StatusCode = (int)HttpStatusCode.MethodNotAllowed;
                        context.Response.Close();
                        break;
                }
            }
        });

        // Prevent the main thread from exiting
        Console.ReadLine();
        httpListener.Stop();
    }
}
