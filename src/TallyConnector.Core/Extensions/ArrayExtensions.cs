using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TallyConnector.Core.Extensions;
public static class ArrayExtensions
{
    public static void AddToArray<T>(this T[] src, T[] newArray, int startIndex)
    {
        var maxLength = startIndex + newArray.Length;
        if (src.Length < maxLength)
        {
            throw new Exception("src array is shorter");
        }
        for (int i = 0; i < newArray.Length; i++)
        {
            src[i + startIndex] = newArray[i];
        }
    }
}
