using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataImporter.Common.Extensions
{
    public static class ListExtensions
    {
        public static List<List<T>> SplitList<T>(this List<T> bigList, int size = 0)
        {
            if (size == default)
                size = bigList.Count;
            if (bigList.Count < size)
                throw new InvalidOperationException("Chunk Size is Bigger than Original List");
            if(bigList.Count % size != 0)
                throw new InvalidOperationException("Can't be broken into Equal pieces");

            var smallLists = new List<List<T>>();

            for (int i = 0; i < bigList.Count; i += size)
                smallLists.Add(bigList.GetRange(i, Math.Min(size, bigList.Count - i)));
            return smallLists;
        }
    }
}
