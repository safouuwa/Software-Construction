using System;

public class Base
{
    public Base()
    {
    }

    public string GetTimestamp()
    {
        return DateTime.UtcNow.ToString("o") + "Z";
    }
}
