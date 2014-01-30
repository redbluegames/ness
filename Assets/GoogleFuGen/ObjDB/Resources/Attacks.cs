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
	public class AttacksRow 
	{
		public string _NAME;
		public int _MINDAMAGE;
		public int _MAXDAMAGE;
		public float _RANGE;
		public string _STRENGTH;
		public string _CONTROLTYPE;
		public string _REACTIONTYPE;
		public float _MINKNOCKBACK;
		public float _MAXKNOCKBACK;
		public string _CHARGEANIMATION;
		public string _SWINGANIMATION;
		public string _PROJECTILEPREFAB;
		public float _CAMERASHAKEINTENSITY;
		public AttacksRow(string __NAME, string __MINDAMAGE, string __MAXDAMAGE, string __RANGE, string __STRENGTH, string __CONTROLTYPE, string __REACTIONTYPE, string __MINKNOCKBACK, string __MAXKNOCKBACK, string __CHARGEANIMATION, string __SWINGANIMATION, string __PROJECTILEPREFAB, string __CAMERASHAKEINTENSITY) 
		{
			_NAME = __NAME;
			{
			int res;
				if(int.TryParse(__MINDAMAGE, out res))
					_MINDAMAGE = res;
				else
					Debug.LogError("Failed To Convert MINDAMAGE string: "+ __MINDAMAGE +" to int");
			}
			{
			int res;
				if(int.TryParse(__MAXDAMAGE, out res))
					_MAXDAMAGE = res;
				else
					Debug.LogError("Failed To Convert MAXDAMAGE string: "+ __MAXDAMAGE +" to int");
			}
			{
			float res;
				if(float.TryParse(__RANGE, out res))
					_RANGE = res;
				else
					Debug.LogError("Failed To Convert RANGE string: "+ __RANGE +" to float");
			}
			_STRENGTH = __STRENGTH;
			_CONTROLTYPE = __CONTROLTYPE;
			_REACTIONTYPE = __REACTIONTYPE;
			{
			float res;
				if(float.TryParse(__MINKNOCKBACK, out res))
					_MINKNOCKBACK = res;
				else
					Debug.LogError("Failed To Convert MINKNOCKBACK string: "+ __MINKNOCKBACK +" to float");
			}
			{
			float res;
				if(float.TryParse(__MAXKNOCKBACK, out res))
					_MAXKNOCKBACK = res;
				else
					Debug.LogError("Failed To Convert MAXKNOCKBACK string: "+ __MAXKNOCKBACK +" to float");
			}
			_CHARGEANIMATION = __CHARGEANIMATION;
			_SWINGANIMATION = __SWINGANIMATION;
			_PROJECTILEPREFAB = __PROJECTILEPREFAB;
			{
			float res;
				if(float.TryParse(__CAMERASHAKEINTENSITY, out res))
					_CAMERASHAKEINTENSITY = res;
				else
					Debug.LogError("Failed To Convert CAMERASHAKEINTENSITY string: "+ __CAMERASHAKEINTENSITY +" to float");
			}
		}

		public string GetStringData( string colID )
		{
			string ret = String.Empty;
			switch( colID.ToUpper() )
			{
				case "NAME":
					ret = _NAME.ToString();
					break;
				case "MINDAMAGE":
					ret = _MINDAMAGE.ToString();
					break;
				case "MAXDAMAGE":
					ret = _MAXDAMAGE.ToString();
					break;
				case "RANGE":
					ret = _RANGE.ToString();
					break;
				case "STRENGTH":
					ret = _STRENGTH.ToString();
					break;
				case "CONTROLTYPE":
					ret = _CONTROLTYPE.ToString();
					break;
				case "REACTIONTYPE":
					ret = _REACTIONTYPE.ToString();
					break;
				case "MINKNOCKBACK":
					ret = _MINKNOCKBACK.ToString();
					break;
				case "MAXKNOCKBACK":
					ret = _MAXKNOCKBACK.ToString();
					break;
				case "CHARGEANIMATION":
					ret = _CHARGEANIMATION.ToString();
					break;
				case "SWINGANIMATION":
					ret = _SWINGANIMATION.ToString();
					break;
				case "PROJECTILEPREFAB":
					ret = _PROJECTILEPREFAB.ToString();
					break;
				case "CAMERASHAKEINTENSITY":
					ret = _CAMERASHAKEINTENSITY.ToString();
					break;
			}

			return ret;
		}
	}
	public class Attacks :  GoogleFuComponentBase
	{
		public enum rowIds {
			SWORD_WEAK, SWORD_STRONG, ENEMY_WEAK, ENEMY_STRONG, PLAYER_BLOCKING_WEAK, PLAYER_BLOCKING_STRONG, SPEAR_JAB, SPEAR_HOLD, BOW_QUICKFIRE, BOW_AIMEDSHOT
		};
		public string [] rowNames = {
			"SWORD_WEAK", "SWORD_STRONG", "ENEMY_WEAK", "ENEMY_STRONG", "PLAYER_BLOCKING_WEAK", "PLAYER_BLOCKING_STRONG", "SPEAR_JAB", "SPEAR_HOLD", "BOW_QUICKFIRE", "BOW_AIMEDSHOT"
		};
		public List<AttacksRow> Rows = new List<AttacksRow>();
		public override void AddRowGeneric (List<string> input)
		{
			Rows.Add(new AttacksRow(input[0],input[1],input[2],input[3],input[4],input[5],input[6],input[7],input[8],input[9],input[10],input[11],input[12]));
		}
		public override void Clear ()
		{
			Rows.Clear();
		}
		public AttacksRow GetRow(rowIds rowID)
		{
			AttacksRow ret = null;
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
		public AttacksRow GetRow(string rowString)
		{
			AttacksRow ret = null;
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
