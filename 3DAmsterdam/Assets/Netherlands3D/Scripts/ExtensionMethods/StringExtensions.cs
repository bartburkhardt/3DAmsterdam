﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ConvertCoordinates;
using System.Linq;
using System.Text;
using System;
using System.Text.RegularExpressions;
using System.Globalization;

public static class StringExtensions
{
       
    
    /// <summary>
    /// Replace the template string and fill in the x and y values
    /// </summary>
    /// <param name="template">The template string</param>
    /// <param name="x">double value x</param>
    /// <param name="y">double value y</param>
    /// <returns>The replaced template string</returns>
    public static string ReplaceXY(this string template, double x, double y)
    {
        StringBuilder sb = new StringBuilder(template);
        sb.Replace("{x}", $"{x}");
        sb.Replace("{y}", $"{y}");
        return sb.ToString();        
    }

    /// <summary>
    /// Replace the template properties defined in a dynamic object
    /// </summary>
    /// <param name="template">The template string</param>
    /// <param name="d">dynamic object</param>    
    /// <returns>The replaced template string</returns>
    public static string ReplacePlaceholders(this string template, object d)
    {
        StringBuilder sb = new StringBuilder(template);
        object o = d;
        string[] propertyNames = o.GetType().GetProperties().Select(p => p.Name).ToArray();
        foreach (var prop in propertyNames)
        {
            object val = o.GetType().GetProperty(prop).GetValue(o, null);
            sb.Replace($"{{{prop}}}", $"{val}");
        }
        return sb.ToString();
    }

    /// <summary>
    /// Get the RD coordinate of a files string
    /// </summary>
    /// <param name="filepath">string filepath</param>
    /// <returns>The Vector3RD coordinate</returns>
	public static Vector3RD GetRDCoordinate(this string filepath)
	{
		var numbers = new Regex(@"(\d{4,6})");

		var matches = numbers.Matches(filepath);
		if(matches.Count == 2)
        {
			return new Vector3RD()
			{
				x = double.Parse(matches[0].Value),
				y = double.Parse(matches[1].Value)
			};
		}
        else
        {
			throw new Exception($"Could not get RD coordinate of string: {filepath}");
		}
	}

    public static Vector3RD GetRDCoordinateByUrl(this string url)
    {
        Vector3RD nodata = new Vector3RD(0, 0, 0);
        var xypart = url.Split(new string[] { "position=" }, StringSplitOptions.None);
        if (xypart.Length == 1) return nodata;

        var position = xypart[1].Split('_');
        if (position.Length == 1) return nodata;

        var ypart = position[1].Split('&');

        return new Vector3RD()
        {
            x = double.Parse(position[0]),
            y = double.Parse(ypart[0])
        };

    }

    public static string ToInvariant(this double d)
    {
        return d.ToString(CultureInfo.InvariantCulture);
    }

}
