using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public static class Extensions
{
    /// <summary>
    /// コレクションのダンプ
    /// 2021/01/11 Fantom
    /// http://fantom1x.blog130.fc2.com/blog-entry-391.html
    /// </summary>
    public static string Dump<T>(this IEnumerable<T> source)
    {
        return (source == null) ? "[]"
            : ("[" + string.Join(", ", source.Select(e => e.ToString()).ToArray()) + "]");
    }

    /// <summary>
    /// コレクションのダンプ（string.Format 付き）
    /// 2021/01/11 Fantom
    /// http://fantom1x.blog130.fc2.com/blog-entry-391.html
    /// </summary>
    public static string Dump<T>(this IEnumerable<T> source, string format)
    {
        return (source == null) ? "[]"
            : ("[" + string.Join(", ", source.Select(e => string.Format(format, e)).ToArray()) + "]");
    }

    /// <summary>
    /// ランダムに取得
    /// </summary>
    public static T GetRandom<T>(this IEnumerable<T> source, Random random)
    {
        if (source == null) throw new ArgumentNullException(nameof(source));

        var list = source as IList<T>;
        if (list != null)
        {
            if (list.Count > 0) return list[random.Next(list.Count)];
        }
        else
        {
            using (var e = source.GetEnumerator())
            {
                if (e.MoveNext())
                {
                    int n = 1;
                    T selected = default(T);
                    do
                    {
                        if (random.Next(n) == 0)
                            selected = e.Current;
                        n++;
                    } while (e.MoveNext());

                    return selected;
                }
            }
        }
        throw new InvalidOperationException("source is empty");
    }

    public static T GetRandomByEnum<T>(this IEnumerable<T> source)
    {
        List<T> statusList = Enum.GetValues(typeof(T))
            .Cast<T>()
            .ToList();
        return statusList.GetRandom(new System.Random());
    }
}