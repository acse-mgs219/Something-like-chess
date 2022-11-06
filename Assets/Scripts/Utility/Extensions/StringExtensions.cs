using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;

public static class StringExtensions
{
    private static readonly Regex sWhitespace = new Regex(@"\s+");

    public static string ReplaceWhitespace(this string input, string replacement)
    {
        return sWhitespace.Replace(input, replacement);
    }
}