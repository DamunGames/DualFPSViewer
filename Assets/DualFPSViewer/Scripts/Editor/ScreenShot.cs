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
		string fileName = $"{FileName}_{dateTime.Year}_{dateTime.Month}_{dateTime.Day}_{dateTime.Hour}_{dateTime.Minute}_{dateTime.Second}.png";
		string outputPath = Path.Combine(OutputDirectory, fileName);

		ScreenCapture.CaptureScreenshot(outputPath);
		Debug.Log($"Captured Screenshot path: {outputPath}");
	}
}
#endif