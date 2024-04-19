using System.Collections.Generic;
using System.Linq;

internal struct SearchResult
{
    public SearchResult(int v1, int v2, object d) : this()
    {
        X = v1;
        Y = v2;
        Value = d;
    }

    public int X { get; set; }
    public int Y { get; set; }
    public object Value { get; set; }
}

internal static class ArrayExtensions
{
    public static void Fill<T>(this T[] source, T value)
    {
        for (int i = 0; i < source.Length; i++)
        {
            source[i] = value;
        }
    }

    public static void Fill<T>(this T[,] source, T value)
    {
        for (int i = 0; i < source.GetLength(0); i++)
        {
            for (int j = 0; j < source.GetLength(1); j++)
            {
                source[i, j] = value;
            }
        }
    }

    public static IEnumerable<SearchResult> GetCoordByValue<T>(this T[,] source, T value)
    {
        var length = source.GetLength(1);
        var result = source.Cast<T>()//1ŽŸŒ³”z—ñ‚É–ß‚·
            .Select((d, i) => new SearchResult(i / length, i % length, d))
            .Where(i => i.Value.Equals(value));
        return result;
    }
}