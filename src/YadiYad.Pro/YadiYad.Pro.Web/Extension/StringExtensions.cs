using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

public static class StringExtensions
{
    public static string ToMask(this string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return null;
        }
        var formattedKeyword = value
            .Trim()
            .Split(' ')
            .Select(str =>
            {
                if (char.IsLetterOrDigit(str.ElementAt(0)))
                {
                    return $"{str.ElementAt(0).ToString()}{string.Join(string.Empty, str.ToCharArray().Skip(1).Select(c => 'x').ToList())}";
                }else
                {
                    return $"{string.Join(string.Empty, str.ToCharArray().Select(c => 'x').ToList())}";
                }
            })
            .ToList();
        return string.Join(' ', formattedKeyword);
    }
}
