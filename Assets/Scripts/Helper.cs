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
}