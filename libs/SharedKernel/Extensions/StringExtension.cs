using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace SharedKernel.Extensions;

public static partial class StringExtension
{
    public static string Underscored(this string s)
    {
        StringBuilder stringBuilder = new StringBuilder();
        for (int i = 0; i < s.Length; i++)
        {
            if (ShouldUnderscore(i, s))
            {
                stringBuilder.Append('_');
            }

            stringBuilder.Append(char.ToLowerInvariant(s[i]));
        }

        return stringBuilder.ToString();
    }

    public static string SpecialCharacterRemoving(this string s)
    {
        return RemoveSpecialCharacterRegex().Replace(s, string.Empty);
    }

    public static string GenerateRandomString(int codeLength = 16, string? allowedSources = null)
    {
        string? collection = allowedSources ?? "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789._-";
        if (codeLength < 0)
        {
            throw new ArgumentOutOfRangeException("codeLength", "length cannot be less than zero.");
        }

        char[] array = new HashSet<char>(collection).ToArray();
        if (array.Length > 256)
        {
            throw new ArgumentException($"allowedChars may contain no more than {256} characters.");
        }

        using RandomNumberGenerator randomNumberGenerator = RandomNumberGenerator.Create();
        StringBuilder stringBuilder = new StringBuilder();
        byte[] array2 = new byte[128];
        while (stringBuilder.Length < codeLength)
        {
            randomNumberGenerator.GetBytes(array2);
            for (int i = 0; i < array2.Length; i++)
            {
                if (stringBuilder.Length >= codeLength)
                {
                    break;
                }

                if (256 - 256 % array.Length > array2[i])
                {
                    stringBuilder.Append(array[array2[i] % array.Length]);
                }
            }
        }

        return stringBuilder.ToString();
    }

    public static string ToScreamingSnakeCase(this string? input)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            return string.Empty;
        }

        string input2 = PascalAndCamelRegex().Replace(input, "$1_$2");
        input2 = UpperLetter().Replace(input2, "$1_$2");
        input2 = input2.Replace("-", "_");
        return input2.ToUpper();
    }

    public static string NextUniformSequence(this string input)
    {
        string text = input.TrimEnd('0', '1', '2', '3', '4', '5', '6', '7', '8', '9');
        int length = text.Length;
        string text2 = input.Substring(length, input.Length - length);
        int num = ((text2 == string.Empty) ? 1 : int.Parse(text2));
        char[] array = text.ToCharArray();
        bool flag = true;
        for (int i = 0; i < array.Length; i++)
        {
            if (array[i] != 'z')
            {
                flag = false;
                break;
            }
        }

        if (flag)
        {
            return "a" + (num + 1);
        }

        for (int j = 0; j < array.Length; j++)
        {
            array[j] = (char)(array[j] + 1);
        }

        return new string(array) + ((num > 1) ? ((object)num) : string.Empty);
    }

    private static bool ShouldUnderscore(int i, string s)
    {
        if (i == 0 || i >= s.Length || s[i] == '_')
        {
            return false;
        }

        char c = s[i];
        char c2 = s[i - 1];
        char c3 = ((i < s.Length - 2) ? s[i + 1] : '_');
        if (c2 != '_')
        {
            if (!char.IsUpper(c) || (!char.IsLower(c2) && !char.IsLower(c3)))
            {
                if (char.IsNumber(c))
                {
                    return !char.IsNumber(c2);
                }

                return false;
            }

            return true;
        }

        return false;
    }

    public static string CompressString(this string uncompressed)
    {
        byte[] inArray;
        using (MemoryStream memoryStream = new MemoryStream())
        {
            using (DeflateStream deflateStream = new DeflateStream(memoryStream, CompressionMode.Compress))
            {
                byte[] bytes = Encoding.UTF8.GetBytes(uncompressed);
                deflateStream.Write(bytes, 0, bytes.Length);
            }

            inArray = memoryStream.ToArray();
        }

        return Convert.ToBase64String(inArray);
    }

    public static string DecompressString(this string compressed)
    {
        using MemoryStream stream = new MemoryStream(Convert.FromBase64String(compressed));
        using DeflateStream stream2 = new DeflateStream(stream, CompressionMode.Decompress);
        using StreamReader streamReader = new StreamReader(stream2);
        return streamReader.ReadToEnd();
    }

    public static bool IsDigit(this string str)
    {
        if (string.IsNullOrEmpty(str))
        {
            return false;
        }

        foreach (char c in str)
        {
            if ((c < '0' || c > '9') ? true : false)
            {
                return false;
            }
        }

        return true;
    }

    [GeneratedRegex("[^A-Za-z0-9_.]+")]
    private static partial Regex RemoveSpecialCharacterRegex();

    [GeneratedRegex("([a-z])([A-Z])")]
    private static partial Regex PascalAndCamelRegex();

    [GeneratedRegex("([A-Z]+)([A-Z][a-z])")]
    private static partial Regex UpperLetter();
}