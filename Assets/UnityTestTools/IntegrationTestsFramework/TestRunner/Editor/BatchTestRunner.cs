using System;
using UnityEditor;
using UnityEngine;

public class BatchTestRunner : MonoBehaviour
{
	public static void RunAllTests ()
	{
		foreach (var arg in Environment.GetCommandLineArgs ())
		{
			if (arg.ToLower ().StartsWith ("-testscene="))
			{
				EditorApplication.OpenScene (arg.Substring (10));
				EditorApplication.isPlaying = true;
				break;
			}
		}
	}
}
