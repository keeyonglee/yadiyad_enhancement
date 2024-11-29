using System;
using System.Collections.Generic;
using System.Text;

public static class StringExtensions
{
    public static string Right(this string value, int totalWidth, char character)
    {
        var defaultString = "";

        for(int i = 0; i<totalWidth; i++)
        {
            defaultString += character;
        }

        string newValue = defaultString + value;

        newValue = newValue.Substring(newValue.Length - totalWidth, totalWidth);

        return newValue;
    }
}