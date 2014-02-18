using UnityEngine;
using UnityEditor;
using System;
using System.Collections;

namespace GoogleFu
{
	[CustomEditor(typeof(Weapons))]
	public class WeaponsEditor : Editor
	{
		public int Index = 0;
		public override void OnInspectorGUI ()
		{
			Weapons s = target as Weapons;
			WeaponsRow r = s.Rows[ Index ];

			EditorGUILayout.BeginHorizontal();
			if ( GUILayout.Button("<<") )
			{
				Index = 0;
			}
			if ( GUILayout.Button("<") )
			{
				Index -= 1;
				if ( Index < 0 )
					Index = s.Rows.Count - 1;
			}
			if ( GUILayout.Button(">") )
			{
				Index += 1;
				if ( Index >= s.Rows.Count )
					Index = 0;
			}
			if ( GUILayout.Button(">>") )
			{
				Index = s.Rows.Count - 1;
			}

			EditorGUILayout.EndHorizontal();

			EditorGUILayout.BeginHorizontal();
			GUILayout.Label( "ID", GUILayout.Width( 150.0f ) );
			{
				EditorGUILayout.LabelField( s.rowNames[ Index ] );
			}
			EditorGUILayout.EndHorizontal();

			EditorGUILayout.BeginHorizontal();
			GUILayout.Label( "NAME", GUILayout.Width( 150.0f ) );
			{
				EditorGUILayout.LabelField( r._NAME );
			}
			EditorGUILayout.EndHorizontal();

			EditorGUILayout.BeginHorizontal();
			GUILayout.Label( "LIGHTATTACKID", GUILayout.Width( 150.0f ) );
			{
				EditorGUILayout.LabelField( r._LIGHTATTACKID );
			}
			EditorGUILayout.EndHorizontal();

			EditorGUILayout.BeginHorizontal();
			GUILayout.Label( "HEAVYATTACKID", GUILayout.Width( 150.0f ) );
			{
				EditorGUILayout.LabelField( r._HEAVYATTACKID );
			}
			EditorGUILayout.EndHorizontal();

		}
	}
}
