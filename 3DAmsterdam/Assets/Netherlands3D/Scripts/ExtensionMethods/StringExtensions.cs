using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ConvertCoordinates;
using System.IO;
using System.Text;

public static class StringExtensions
{
    public static Vector3RD GetRDCoordinate(this string file)
    {
        var name = Path.GetFileNameWithoutExtension(file);
        var splitted = name.Split('-');

        return new Vector3RD()
        {
            x = double.Parse(splitted[0]),
            y = double.Parse(splitted[1]),
        };
    }

    
    
    /// <summary>
    /// Replace the template string and fill in the x and y values
    /// </summary>
    /// <param name="template">The template string</param>
    /// <param name="x">double value x</param>
    /// <param name="y">double value y</param>
    /// <returns></returns>
    public static string ReplaceXY(this string template, double x, double y)
    {
        StringBuilder sb = new StringBuilder(template);
        sb.Replace("{x}", $"{x}");
        sb.Replace("{y}", $"{y}");
        return sb.ToString();        
    }




}
