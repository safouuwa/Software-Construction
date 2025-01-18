using System;
using System.Collections.Generic;
using System.Timers; // Ensure you're using System.Timers
namespace Processors;

public class NotificationSystem
{
    private static readonly int NOTIFICATION_UPDATE_INTERVAL_MS = 30000; // 30 seconds
    private static Queue<string> _queue = new Queue<string>();
    private static System.Timers.Timer _timer; // Fully qualify the Timer

    public NotificationSystem()
    {
        // Initialize the timer
        _timer = new System.Timers.Timer(NOTIFICATION_UPDATE_INTERVAL_MS); // Fully qualify here
        _timer.Elapsed += OnTimedEvent;
        _timer.AutoReset = true; // Restart the timer after each event
    }

    public void Push(string notification)
    {
        lock (_queue) // Lock to ensure thread safety
        {
            _queue.Enqueue(notification);
        }
    }

    private void OnTimedEvent(Object source, ElapsedEventArgs e)
    {
        string notification = null;

        lock (_queue)
        {
            if (_queue.Count > 0)
            {
                notification = _queue.Dequeue();
            }
        }

        if (notification != null)
        {
            Console.WriteLine(notification);
        }
    }

    public static void Start()
    {
        _timer.Start();
    }

    public void Stop()
    {
        _timer.Stop();
    }
}
