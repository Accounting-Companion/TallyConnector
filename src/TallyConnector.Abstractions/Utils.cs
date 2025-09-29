using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace TallyConnector.Abstractions;
public  class Utils
{
    public static string GenerateUniqueNameSuffix(string combinedInput, int length = 4)
    {

        using SHA256 sha256 = SHA256.Create();
        byte[] hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(combinedInput));
        var hash = Convert.ToBase64String(hashBytes);
        StringBuilder sb = new(length);
        hash = hash.Substring(0, length);
        foreach (char c in hash)
        {
            switch (c)
            {
                case '+':
                    sb.Append('P'); // Replace '+' with 'P' (or another valid char like '_')
                    break;
                case '/':
                    sb.Append('S'); // Replace '/' with 'S' (or another valid char like '_')
                    break;
                case '=':
                    // Skip Base64 padding character
                    break;
                default:
                    // Append only if it's a letter or digit to keep the suffix clean
                    if (char.IsLetterOrDigit(c))
                    {
                        sb.Append(c);
                    }
                    // Other characters are ignored
                    break;
            }
        }
        var finalHash = sb.ToString();
        if (finalHash.Length < 4)
        {
            finalHash = finalHash.PadRight(4, 'X');
        }
        return finalHash.ToUpper();
    }
}
