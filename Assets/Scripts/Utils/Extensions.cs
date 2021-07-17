using System;
using UnityEngine;

namespace Utils
{
    public static class Extensions
    {
        public static T Next<T>(this T src) where T : struct
        {
            if (!typeof(T).IsEnum) throw new ArgumentException($"Argument {typeof(T).FullName} is not an Enum");

            var arr = (T[]) Enum.GetValues(src.GetType());
            var j = Array.IndexOf(arr, src) + 1;
            return (arr.Length == j) ? arr[0] : arr[j];
        }
        public static T Previous<T>(this T src) where T : struct
        {
            if (!typeof(T).IsEnum) throw new ArgumentException($"Argument {typeof(T).FullName} is not an Enum");

            var arr = (T[]) Enum.GetValues(src.GetType());
            var j = Array.IndexOf(arr, src) - 1;
            return (-1 == j) ? arr[arr.Length - 1] : arr[j];
        }
     
        public static Vector2 WithNewX(this Vector2 vector, float newX) => new Vector2(newX, vector.y);
    }
}