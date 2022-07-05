using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public static class Ultil
{
    public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> list)
    {
        Random rnd = new Random();
        return list.OrderBy<T, int>((item) => rnd.Next());
    }

    public static void ForEach<T>(this IEnumerable<T> enumeration, Action<T> action)
    {
        foreach (T item in enumeration)
        {
            action(item);
        }
    }

    public static async UniTask AsyncForEach<T>(this IEnumerable<T> enumeration, Func<T, UniTask> action)
    {
        foreach (T item in enumeration)
        {
            await action(item);
        }
    }
}
