using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public static class Extensions
{
    public static void AddRange<T>(this ObservableCollection<T> collection, IEnumerable<T> items)
    {
        foreach (var item in items)
        {
            collection.Add(item);
        }
    }

    public static void SetRange<T>(this ObservableCollection<T> collection, IEnumerable<T> items)
    {
        collection.Clear();
        collection.AddRange(items);
    }

    public static void SortDesc<T, TRes>(this ObservableCollection<T> collection, Func<T, TRes> keySelector)
    {
        var clones = collection.OrderByDescending(keySelector).ToArray();
        for (int i = 0; i < clones.Length; i++)
        {
            var oldIndex = collection.IndexOf(clones[i]);
            if (i != oldIndex)
            {
                // WP ItemsControl doesn't like .Move - so removing and adding in a lazy way
                // collection.Move(oldIndex, i);
                var itemA = collection[i];
                var itemB = collection[oldIndex];

                collection[i] = itemB;
                collection[oldIndex] = itemA;
            }
        }
    }
}

