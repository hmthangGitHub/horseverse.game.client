using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System;

namespace Core
{
    public static class CoreExtension
    {
        #region list
        public static T FindMaxOrMin<T>(this List<T> source, Func<T, T, bool> func)
        {
            if (source.Count == 0) return default(T);
            T maxValue = source[0];
            for (int i = 1; i < source.Count; i++){
                var value = source[i];
                if(func(maxValue, value)){
                    maxValue = value;
                }
            }
            return maxValue;
        }
        #endregion

        #region quick sort
        public static void QuickSort<T>(this IList<T> source, Func<T, T, bool> func)
        {
            if (source.Count > 1)
            {
                QuickSort(source, func, 0, source.Count - 1);
            }
        }

        static void QuickSort<T>(IList<T> source, Func<T, T, bool> func, int left, int right)
        {
            int i = left, j = right;
            T tmp;
            T pivot = source[(left + right) / 2];

            while (i <= j)
            {
                while (func(source[i], pivot))
                    i++;
                while (func(pivot, source[j]))
                    j--;
                if (i <= j)
                {
                    tmp = source[i];
                    source[i] = source[j];
                    source[j] = tmp;
                    i++;
                    j--;
                }
            }

            if (left < j)
                QuickSort(source, func, left, j);
            if (i < right)
                QuickSort(source, func, i, right);
        }
        #endregion

        #region sort list
        public static void AddHasSort<T>(this List<T> source, T t)
        {
            if (source.Count > 0)
            {
                var index = GetIndexWillAdd(source, 0, source.Count - 1, t.GetHashCode());
                source.Insert(index, t);
            }
            else
            {
                source.Add(t);
            }
        }

        public static void AddRangeHasSort<T>(this List<T> source, List<T> list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                source.AddHasSort(list[i]);
            }
        }

        public static void RemoveHasSort<T>(this List<T> source, T t)
        {
            if (source.Count > 0)
            {
                var index = GetIndexByHashCode(source, 0, source.Count - 1, t.GetHashCode());
                if (index >= 0)
                {
                    source.RemoveAt(index);
                }
            }
        }

        static int GetIndexWillAdd<T>(List<T> source, int left, int right, int hashCode)
        {
            if (left > right)
            {
                return left;
            }
            if (left == right)
            {
                var value = source[left];
                if (value.GetHashCode() > hashCode)
                {
                    return left;
                }
                else
                {
                    return left + 1;
                }
            }
            else if (left == right - 1)
            {
                var value = source[left];
                if (value.GetHashCode() > hashCode)
                {
                    return left;
                }
                else
                {
                    var valueR = source[right];
                    if (valueR.GetHashCode() > hashCode)
                    {
                        return right;
                    }
                    else
                    {
                        return right + 1;
                    }
                }
            }

            int index = (left + right) / 2;
            var mid = source[index];

            if (hashCode < mid.GetHashCode())
            {
                return GetIndexWillAdd(source, left, index, hashCode);
            }
            else
            {
                return GetIndexWillAdd(source, index, right, hashCode);
            }
        }

        public static void SortByHashCode<T>(this List<T> source)
        {
            source.Sort((x, y) =>
            {
                return x.GetHashCode().CompareTo(y.GetHashCode());
            });
        }

        public static T GetByHashCode<T>(this List<T> source, int hashCode)
        {
            var n = source.Count;
            var t = GetByHashCode(source, 0, n - 1, hashCode);
            return t;
        }

        public static int GetIndexByHashCode<T>(this List<T> source, int hashCode)
        {
            var n = source.Count;
            return GetIndexByHashCode(source, 0, n - 1, hashCode);
        }

        static T GetByHashCode<T>(List<T> source, int left, int right, int hashCode)
        {
            if (left > right)
            {
                return default(T);
            }
            if (left == right)
            {
                var value = source[left];
                if (value.GetHashCode() == hashCode)
                {
                    return value;
                }
                else
                {
                    return default(T);
                }
            }
            else if (left == right - 1)
            {
                var value = source[left];
                if (value.GetHashCode() == hashCode)
                {
                    return value;
                }
                else
                {
                    var valueR = source[right];
                    if (valueR.GetHashCode() == hashCode)
                    {
                        return valueR;
                    }
                    else
                    {
                        return default(T);
                    }
                }
            }

            int index = (left + right) / 2;
            var mid = source[index];

            if (hashCode == mid.GetHashCode())
            {
                return mid;
            }
            else if (hashCode < mid.GetHashCode())
            {
                return GetByHashCode(source, left, index, hashCode);
            }
            else
            {
                return GetByHashCode(source, index, right, hashCode);
            }
        }

        static int GetIndexByHashCode<T>(List<T> source, int left, int right, int hashCode)
        {
            if (left > right)
            {
                return -1;
            }
            if (left == right)
            {
                var value = source[left];
                if (value.GetHashCode() == hashCode)
                {
                    return left;
                }
                else
                {
                    return -1;
                }
            }
            else if (left == right - 1)
            {
                var value = source[left];
                if (value.GetHashCode() == hashCode)
                {
                    return left;
                }
                else
                {
                    var valueR = source[right];
                    if (valueR.GetHashCode() == hashCode)
                    {
                        return right;
                    }
                    else
                    {
                        return -1;
                    }
                }
            }

            int index = (left + right) / 2;
            var mid = source[index];

            if (hashCode == mid.GetHashCode())
            {
                return index;
            }
            else if (hashCode < mid.GetHashCode())
            {
                return GetIndexByHashCode(source, left, index, hashCode);
            }
            else
            {
                return GetIndexByHashCode(source, index, right, hashCode);
            }
        }
        #endregion

        public static void CompareToFind2ExceptList<T>(this List<T> list, List<T> compareList, out List<T> exceptList1, out List<T> exceptList2)
        {
            exceptList1 = new List<T>();
            exceptList2 = new List<T>();
            exceptList2.AddRange(compareList);
            List<T> toList = new List<T>();
            for (int i = 0; i < list.Count; i++)
            {
                var toCompare = list[i];
                var index = exceptList2.GetIndexByHashCode(toCompare.GetHashCode());
                if (index == -1)
                {
                    exceptList1.Add(toCompare);
                }
                else
                {
                    exceptList2.RemoveAt(index);
                }
            }
        }
    }
}
