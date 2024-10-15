using System;

public class Base
{
    public Base()
    {
    }

    public string GetTimestamp()
    {
        return DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss");
    }

}
