using UnityEngine;

namespace UnityTest
{
	public static class Icons
	{
		public static readonly Texture2D failImg;
		public static readonly Texture2D ignoreImg;
		public static readonly Texture2D runImg;
		public static readonly Texture2D runFailedImg;
		public static readonly Texture2D runAllImg;
		public static readonly Texture2D successImg;
		public static readonly Texture2D unknownImg;
		public static readonly Texture2D inconclusiveImg;
		public static readonly Texture2D stopwatchImg;
		public static readonly Texture2D plusImg;
		public static readonly Texture2D gearImg;
		public static readonly Texture2D greenredyellowImg1;
		public static readonly Texture2D greenredyellowImg2;
		public static readonly Texture2D greenredyellowImg3;
		public static readonly Texture2D greenredyellowImg4;

		public static readonly GUIContent guiUnknownImg;
		public static readonly GUIContent guiInconclusiveImg;
		public static readonly GUIContent guiIgnoreImg;
		public static readonly GUIContent guiSuccessImg;
		public static readonly GUIContent guiFailImg;
		public static readonly GUIContent guiGreenredyellowImg1;
		public static readonly GUIContent guiGreenredyellowImg2;
		public static readonly GUIContent guiGreenredyellowImg3;
		public static readonly GUIContent guiGreenredyellowImg4;
		
		static Icons ()
		{
			failImg = (Texture2D)Resources.LoadAssetAtPath ("Assets/UnityTestTools/Common/icons/red.png", typeof (Texture2D));
			ignoreImg = (Texture2D)Resources.LoadAssetAtPath ("Assets/UnityTestTools/Common/icons/grey.png", typeof (Texture2D));
			runImg = (Texture2D)Resources.LoadAssetAtPath ("Assets/UnityTestTools/Common/icons/play.png", typeof (Texture2D));
			runFailedImg = (Texture2D)Resources.LoadAssetAtPath ("Assets/UnityTestTools/Common/icons/playred.png", typeof (Texture2D));
			runAllImg = (Texture2D)Resources.LoadAssetAtPath ("Assets/UnityTestTools/Common/icons/playall.png", typeof (Texture2D));
			successImg = (Texture2D)Resources.LoadAssetAtPath ("Assets/UnityTestTools/Common/icons/green.png", typeof (Texture2D));
			unknownImg = (Texture2D)Resources.LoadAssetAtPath ("Assets/UnityTestTools/Common/icons/white.png", typeof (Texture2D));
			inconclusiveImg = (Texture2D)Resources.LoadAssetAtPath ("Assets/UnityTestTools/Common/icons/yellow.png", typeof (Texture2D));
			stopwatchImg = (Texture2D)Resources.LoadAssetAtPath ("Assets/UnityTestTools/Common/icons/stopwatch.png", typeof (Texture2D));
			plusImg = (Texture2D)Resources.LoadAssetAtPath ("Assets/UnityTestTools/Common/icons/plus.png", typeof (Texture2D));
			gearImg = (Texture2D)Resources.LoadAssetAtPath ("Assets/UnityTestTools/Common/icons/gear.png", typeof (Texture2D));
			greenredyellowImg1 = (Texture2D)Resources.LoadAssetAtPath ("Assets/UnityTestTools/Common/icons/green-red-yellow1.png", typeof (Texture2D));
			greenredyellowImg2 = (Texture2D)Resources.LoadAssetAtPath ("Assets/UnityTestTools/Common/icons/green-red-yellow2.png", typeof (Texture2D));
			greenredyellowImg3 = (Texture2D)Resources.LoadAssetAtPath ("Assets/UnityTestTools/Common/icons/green-red-yellow3.png", typeof (Texture2D));
			greenredyellowImg4 = (Texture2D)Resources.LoadAssetAtPath ("Assets/UnityTestTools/Common/icons/green-red-yellow4.png", typeof (Texture2D));

			guiUnknownImg = new GUIContent (unknownImg);
			guiInconclusiveImg = new GUIContent (inconclusiveImg);
			guiIgnoreImg = new GUIContent (ignoreImg);
			guiSuccessImg = new GUIContent (successImg);
			guiFailImg = new GUIContent (failImg);
			guiGreenredyellowImg1 = new GUIContent (greenredyellowImg1);
			guiGreenredyellowImg2 = new GUIContent (greenredyellowImg2);
			guiGreenredyellowImg3 = new GUIContent (greenredyellowImg3);
			guiGreenredyellowImg4 = new GUIContent (greenredyellowImg4);
		}

		public static GUIContent GetSpinningIcon ()
		{
			var frame = ((int) (Time.realtimeSinceStartup * 7)) % 4;
			switch (frame)
			{
				case 0:
					return guiGreenredyellowImg1;
				case 1:
					return guiGreenredyellowImg2;
				case 2:
					return guiGreenredyellowImg3;
				case 3:
					return guiGreenredyellowImg4;
				default:
					return guiGreenredyellowImg1;
			}
		}
	}
}
