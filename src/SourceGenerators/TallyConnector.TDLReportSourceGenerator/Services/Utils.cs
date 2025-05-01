using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace TallyConnector.TDLReportSourceGenerator.Services;
public  class Utils
{
    public static string GenerateUniqueNameSuffix(string combinedInput)
    {
        using SHA256 sha256 = SHA256.Create();
        byte[] hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(combinedInput));
        var hash = Convert.ToBase64String(hashBytes);
        hash = hash.Substring(0, 4);
        return hash;
    }
}
