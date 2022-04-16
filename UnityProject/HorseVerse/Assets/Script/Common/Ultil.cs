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
}
