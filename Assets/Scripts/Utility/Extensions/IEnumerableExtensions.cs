using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class IEnumerableExtensions
{
    public static T RandomElementByWeight<T>(this IEnumerable<T> sequence, Func<T, float> weightSelector)
    {
        float totalWeight = sequence.Sum(weightSelector);
        // The weight we are after...
        float itemWeightIndex = (float) UnityEngine.Random.Range(0, totalWeight);
        float currentWeightIndex = 0;

        foreach (var item in sequence)
        {
            currentWeightIndex += weightSelector(item);

            // If we've hit or passed the weight we are after for this item then it's the one we want....
            if (currentWeightIndex >= itemWeightIndex)
                return item;
        }

        return default(T);
    }

    public static T RandomElement<T>(this IEnumerable<T> sequence)
    {
        return sequence.OrderBy(_ => UnityEngine.Random.value).First();
    }

    public static Color Average(this IEnumerable<Color> source)
    {
        float reds = 0;
        float greens = 0;
        float blues = 0;
        float alphas = 0;
        float cols = 0;

        foreach (Color color in source)
        {
            reds += color.r;
            greens += color.g;
            blues += color.b;
            alphas += color.a;
            cols++;
        }

        return new Color(reds / cols, greens / cols, blues / cols, alphas / cols);
    }
}