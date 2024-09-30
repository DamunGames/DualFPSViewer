#if UNITY_EDITOR
using System;
using System.IO;
using UnityEngine;
using UnityEditor;

public static class ScreenShot
{
	const string OutputDirectory = "Screenshots";
	const string FileName = "Screenshot";

	[MenuItem("Tools/Screenshot")]
	public static void CaptureScreenshot()
	{
		if (!Directory.Exists(OutputDirectory)) {
			Directory.CreateDirectory(OutputDirectory);
		}

		DateTime dateTime = DateTime.Now;
		string fileName = $"{FileName}_{dateTime.Year:0000}_{dateTime.Month:00}_{dateTime.Day:00}_{dateTime.Hour:00}_{dateTime.Minute:00}_{dateTime.Second:00}.png";
		string outputPath = Path.Combine(OutputDirectory, fileName);

		ScreenCapture.CaptureScreenshot(outputPath);
		Debug.Log($"Captured Screenshot path: {outputPath}");
	}
}
#endif