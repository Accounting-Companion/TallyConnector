using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace TallyConnector.Core.Models.TallyComplexObjects;

public static class TallyUnitHelper
{
    private const string Separator = " of ";

    /// <summary>
    /// Converts a simple quantity into a compound unit string based on the provided unit definition string.
    /// Example: 300, "Box of 10 Nos" -> "30 Box 0 Nos"
    /// Example: 2545, "Box of 100 strips of 10 tablets" -> "2 Box 54 strips 5 tablets"
    /// </summary>
    public static string ConvertToCompoundUnit(decimal quantity, string unitStr)
    {
        if (string.IsNullOrWhiteSpace(unitStr))
        {
            return FormatDecimal(quantity);
        }

        // Fast path: if no separator, return simple formatted string
        if (unitStr.IndexOf(Separator, StringComparison.OrdinalIgnoreCase) < 0)
        {
            return $"{FormatDecimal(quantity)} {unitStr}";
        }

        try
        {
            var units = ParseUnits(unitStr.AsSpan());
            
            // Per-unit factors relative to the base unit
            int count = units.Count;
            if (count == 0) return $"{FormatDecimal(quantity)} {unitStr}";

            // Using array for simplicity and speed on small collections
            decimal[] totalFactors = new decimal[count];
            
            // Base unit (last one) is always equivalent to 1 base unit
            totalFactors[count - 1] = 1m;

            for (int i = count - 2; i >= 0; i--)
            {
                totalFactors[i] = units[i].FactorToNext * totalFactors[i + 1];
            }

            var parts = new (decimal Value, string Name)[count];
            decimal remainingQty = quantity;
            int sign = Math.Sign(remainingQty);
            remainingQty = Math.Abs(remainingQty);

            for (int i = 0; i < count; i++)
            {
                var (name, _) = units[i];
                decimal factor = totalFactors[i];

                decimal val;
                if (i == count - 1)
                {
                    // Last unit (Base unit) - take remainder
                    val = remainingQty;
                }
                else
                {
                    if (factor == 0) factor = 1;

                    // Calculate whole number of this unit
                    val = (long)(remainingQty / factor);
                    remainingQty = remainingQty % factor;
                }
                parts[i] = (val, name);
            }
            // Apply sign to the first unit


            var sb = new StringBuilder();
            if (sign < 0)
            {
                sb.Append('-');
            }

            for (int i = 0; i < count; i++)
            {
                if (i > 0) sb.Append(' ');
                sb.Append(FormatDecimal(Math.Abs(parts[i].Value)));
                sb.Append(' ');
                sb.Append(parts[i].Name);
            }

            return sb.ToString();
        }
        catch
        {
            // Robust fallback to simple display if parsing fails
            return $"{FormatDecimal(quantity)} {unitStr}";
        }
    }

    private static string FormatDecimal(decimal d)
    {
        // Remove trailing zeros using InvariantCulture for consistency
        return (d % 1 == 0) ? ((long)d).ToString(CultureInfo.InvariantCulture) : d.ToString("0.##", CultureInfo.InvariantCulture);
    }

    private static List<(string Name, decimal FactorToNext)> ParseUnits(ReadOnlySpan<char> source)
    {
        var result = new List<(string Name, decimal FactorToNext)>(4);
        
        int separatorLen = Separator.Length;
        int nextIndex = source.IndexOf(Separator, StringComparison.OrdinalIgnoreCase);
        
        if (nextIndex < 0)
        {
            result.Add((source.Trim().ToString(), 1m));
            return result;
        }

        // First part: Name only
        var firstNameSpan = source.Slice(0, nextIndex).Trim();
        string currentName = firstNameSpan.ToString();
        
        // Advance source past the first name and separator
        source = source.Slice(nextIndex + separatorLen);

        while (true)
        {
            nextIndex = source.IndexOf(Separator, StringComparison.OrdinalIgnoreCase);
            
            ReadOnlySpan<char> segment;
            if (nextIndex >= 0)
            {
                segment = source.Slice(0, nextIndex).Trim();
                // Move source past this segment and separator for next loop
                source = source.Slice(nextIndex + separatorLen);
            }
            else
            {
                // Last segment
                segment = source.Trim();
            }

            // Segment format is expected to be "Multiplier Name"
            int spaceIndex = segment.IndexOf(' ');
            if (spaceIndex < 0)
            {
                throw new FormatException("Missing multiplier in segment");
            }

            var factorSpan = segment.Slice(0, spaceIndex);
            var nameSpan = segment.Slice(spaceIndex + 1).Trim();

            if (!decimal.TryParse(factorSpan, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal multiplier))
            {
                 throw new FormatException($"Invalid multiplier format: {factorSpan.ToString()}");
            }

            // Add previous unit with conversion factor to this unit
            result.Add((currentName, multiplier));
            
            currentName = nameSpan.ToString();

            if (nextIndex < 0) break;
        }

        // Add the last unit (base unit) with factor 1
        result.Add((currentName, 1m));

        return result;
    }
}
