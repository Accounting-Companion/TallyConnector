namespace TallyConnector.Core.Extensions;
public static class ArrayExtensions
{
    public static T[] CombineArrays<T>(this T[] firstArray, T[] secondArray)
    {
        return CombineArrays(firstArray, secondArray);
    }
    public static T[] CombineMultipleArrays<T>(params T[][] arrays)
    {
        int totalCount = 0;
        foreach (var array in arrays)
        {
            totalCount += array.Length;
        }
        var combinedArray = new T[totalCount];
        int lastIndex = 0;
        for (int i = 0; i < arrays.Length; i++)
        {
            T[] currentArray = arrays[i];
            Array.Copy(currentArray, 0, combinedArray, lastIndex, currentArray.Length);
            lastIndex += currentArray.Length;
        }
        return combinedArray;
    }
}
