using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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

    public static void MinMaxNormalization(this float[,] source)
    {
        var data = source.Cast<float>();
        float max = data.Max();
        float min = data.Min();

        for (var i = 0; i < source.GetLength(0); i++)
        {
            for (var j = 0; j < source.GetLength(1); j++)
            {
                source[i, j] = ((source[i, j] - min) / (max - min));
            }
        }
    }

    public static void Dump<T>(this T[,] source)
    {
        for (int i = 0; i < source.GetLength(0); i++)
        {
            string str = "";
            for (int j = 0; j < source.GetLength(1); j++)
            {
                str = str + source[i, j] + " ";
            }
            Debug.Log(str);
        }
    }

    public static IEnumerable<SearchResult> GetCoordListByValue<T>(this T[,] source, T value)
    {
        var length = source.GetLength(1);
        var result = source.Cast<T>()//1ŽŸŒ³”z—ñ‚É–ß‚·
            .Select((d, i) => new SearchResult(i / length, i % length, d))
            .Where(i => i.Value.Equals(value));
        return result;
    }
}