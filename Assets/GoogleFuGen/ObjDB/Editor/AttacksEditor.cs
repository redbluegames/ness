using UnityEngine;
using UnityEditor;
using System;
using System.Collections;

namespace GoogleFu
{
	[CustomEditor(typeof(Attacks))]
	public class AttacksEditor : Editor
	{
		public int Index = 0;
		public override void OnInspectorGUI ()
		{
			Attacks s = target as Attacks;
			AttacksRow r = s.Rows[ Index ];

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
			GUILayout.Label( "MINDAMAGE", GUILayout.Width( 150.0f ) );
			{
				EditorGUILayout.IntField( r._MINDAMAGE );
			}
			EditorGUILayout.EndHorizontal();

			EditorGUILayout.BeginHorizontal();
			GUILayout.Label( "MAXDAMAGE", GUILayout.Width( 150.0f ) );
			{
				EditorGUILayout.IntField( r._MAXDAMAGE );
			}
			EditorGUILayout.EndHorizontal();

			EditorGUILayout.BeginHorizontal();
			GUILayout.Label( "RANGE", GUILayout.Width( 150.0f ) );
			{
				EditorGUILayout.FloatField( r._RANGE );
			}
			EditorGUILayout.EndHorizontal();

			EditorGUILayout.BeginHorizontal();
			GUILayout.Label( "STRENGTH", GUILayout.Width( 150.0f ) );
			{
				EditorGUILayout.LabelField( r._STRENGTH );
			}
			EditorGUILayout.EndHorizontal();

			EditorGUILayout.BeginHorizontal();
			GUILayout.Label( "CONTROLTYPE", GUILayout.Width( 150.0f ) );
			{
				EditorGUILayout.LabelField( r._CONTROLTYPE );
			}
			EditorGUILayout.EndHorizontal();

			EditorGUILayout.BeginHorizontal();
			GUILayout.Label( "REACTIONTYPE", GUILayout.Width( 150.0f ) );
			{
				EditorGUILayout.LabelField( r._REACTIONTYPE );
			}
			EditorGUILayout.EndHorizontal();

			EditorGUILayout.BeginHorizontal();
			GUILayout.Label( "MINKNOCKBACK", GUILayout.Width( 150.0f ) );
			{
				EditorGUILayout.FloatField( r._MINKNOCKBACK );
			}
			EditorGUILayout.EndHorizontal();

			EditorGUILayout.BeginHorizontal();
			GUILayout.Label( "MAXKNOCKBACK", GUILayout.Width( 150.0f ) );
			{
				EditorGUILayout.FloatField( r._MAXKNOCKBACK );
			}
			EditorGUILayout.EndHorizontal();

			EditorGUILayout.BeginHorizontal();
			GUILayout.Label( "CHARGEANIMATION", GUILayout.Width( 150.0f ) );
			{
				EditorGUILayout.LabelField( r._CHARGEANIMATION );
			}
			EditorGUILayout.EndHorizontal();

			EditorGUILayout.BeginHorizontal();
			GUILayout.Label( "SWINGANIMATION", GUILayout.Width( 150.0f ) );
			{
				EditorGUILayout.LabelField( r._SWINGANIMATION );
			}
			EditorGUILayout.EndHorizontal();

			EditorGUILayout.BeginHorizontal();
			GUILayout.Label( "PROJECTILEPREFAB", GUILayout.Width( 150.0f ) );
			{
				EditorGUILayout.LabelField( r._PROJECTILEPREFAB );
			}
			EditorGUILayout.EndHorizontal();

			EditorGUILayout.BeginHorizontal();
			GUILayout.Label( "CAMERASHAKEINTENSITY", GUILayout.Width( 150.0f ) );
			{
				EditorGUILayout.FloatField( r._CAMERASHAKEINTENSITY );
			}
			EditorGUILayout.EndHorizontal();

		}
	}
}
