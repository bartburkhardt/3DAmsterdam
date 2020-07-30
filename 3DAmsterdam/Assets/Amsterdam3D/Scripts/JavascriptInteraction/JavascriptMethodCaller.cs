﻿using UnityEngine;
using System.Runtime.InteropServices;
using Amsterdam3D.UserLayers;

public class JavascriptMethodCaller : MonoBehaviour
{

	[DllImport("__Internal")]
	private static extern void UploadButtonCSSDisplay(string display = "none");

	[DllImport("__Internal")]
	private static extern string FetchOBJData();

	[DllImport("__Internal")]
	private static extern string FetchMTLData();

	/// <summary>
	/// This methods activates the html hitarea for the file upload button.
	/// The user will click the hidden file input dialog in the index.html template file that is drawn on top of our WebGL canvas.
	/// This overcomes the problem with browser security where a click is required to open a file upload dialog.
	/// Faking a click through JavaScript is not allowed.
	/// </summary>
	/// <param name="display">Sets the hitarea CSS of the input HTML node to inline, or none</param>
	public static void DisplayOBJUploadButtonHitArea(bool display)
	{
#if UNITY_WEBGL && !UNITY_EDITOR
         UploadButtonCSSDisplay((display) ? "inline" : "none");
#endif
	}

	public static string FetchOBJDataAsString()
	{
		return FetchOBJData();
	}
	public static string FetchMTLDataAsString()
	{
		return FetchMTLData();
	}
}