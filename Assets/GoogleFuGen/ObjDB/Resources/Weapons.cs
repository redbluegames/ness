//----------------------------------------------
//    GoogleFu: Google Doc Unity integration
//         Copyright ?? 2013 Litteratus
//
//        This file has been auto-generated
//              Do not manually edit
//----------------------------------------------

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace GoogleFu
{
	[System.Serializable]
	public class WeaponsRow 
	{
		public string _NAME;
		public string _LIGHTATTACKID;
		public string _HEAVYATTACKID;
		public WeaponsRow(string __NAME, string __LIGHTATTACKID, string __HEAVYATTACKID) 
		{
			_NAME = __NAME;
			_LIGHTATTACKID = __LIGHTATTACKID;
			_HEAVYATTACKID = __HEAVYATTACKID;
		}

		public string GetStringData( string colID )
		{
			string ret = String.Empty;
			switch( colID.ToUpper() )
			{
				case "NAME":
					ret = _NAME.ToString();
					break;
				case "LIGHTATTACKID":
					ret = _LIGHTATTACKID.ToString();
					break;
				case "HEAVYATTACKID":
					ret = _HEAVYATTACKID.ToString();
					break;
			}

			return ret;
		}
	}
	public class Weapons :  GoogleFuComponentBase
	{
		public enum rowIds {
			SPEAR, SWORD, ENEMY_SWORD, BOW
		};
		public string [] rowNames = {
			"SPEAR", "SWORD", "ENEMY_SWORD", "BOW"
		};
		public List<WeaponsRow> Rows = new List<WeaponsRow>();
		public override void AddRowGeneric (List<string> input)
		{
			Rows.Add(new WeaponsRow(input[0],input[1],input[2]));
		}
		public override void Clear ()
		{
			Rows.Clear();
		}
		public WeaponsRow GetRow(rowIds rowID)
		{
			WeaponsRow ret = null;
			try
			{
				ret = Rows[(int)rowID];
			}
			catch( KeyNotFoundException ex )
			{
				Debug.LogError( rowID + " not found: " + ex.Message );
			}
			return ret;
		}
		public WeaponsRow GetRow(string rowString)
		{
			WeaponsRow ret = null;
			try
			{
				ret = Rows[(int)Enum.Parse(typeof(rowIds), rowString)];
			}
			catch(ArgumentException) {
				Debug.LogError( rowString + " is not a member of the rowIds enumeration.");
			}
			return ret;
		}

	}

}
