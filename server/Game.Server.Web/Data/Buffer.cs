using System.Globalization;
using System.Security.Cryptography;

namespace Game.Server.Web.Data;

/// <summary>
/// https://stackoverflow.com/questions/12294296/list-with-limited-item
/// </summary>
/// <typeparam name="T"></typeparam>
public class Buffer<T> : Queue<T>
{
    public int? MaxCapacity { get; }
    public Buffer(int capacity) { MaxCapacity = capacity; }
    public int TotalItemsAddedCount { get; private set; }

    public void Add(T newElement)
    {
        // not thread safe 🤷‍
        if (Count == (MaxCapacity ?? -1)) Dequeue();
        Enqueue(newElement);
        TotalItemsAddedCount++;
    }
}

public static class BufferExtensions
{
    private static int score;
    private static int score2;
    private static int gameNumber;

    public static Point AddNewRandomPoint(this Buffer<Point> buffer)
    {
        gameNumber ++;

        score += RandomNumberGenerator.GetInt32(1, 5);
        string teamName = "StarterBot";
        var point = new Point(gameNumber, score, teamName);
        buffer.Add(point);

        score2 += RandomNumberGenerator.GetInt32(1, 10);
        string teamName2 = "NightmareBot";
        var point2 = new Point(gameNumber, score2, teamName2);
        buffer.Add(point2);

        return point;
    }
}