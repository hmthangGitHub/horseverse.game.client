using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace FixRound
{
	public static class FixRoundDefine{
		public static int Number = 3;
		public static float Buffer = 0.00001f;
		public static float Round = 0.015f;
	}

	public static class FloatValue {
		public static float Round(float x){
			return (float)Math.Round (x + FixRoundDefine.Buffer, FixRoundDefine.Number);
		}
	}
	public static class Vector3Value {
		public static Vector3 Round(Vector3 vector3){
			return new Vector3(FloatValue.Round(vector3.x), FloatValue.Round(vector3.y), FloatValue.Round(vector3.z));
		}
		public static Vector3 Round(float x, float y, float z){
			return new Vector3(FloatValue.Round(x), FloatValue.Round(y), FloatValue.Round(z));
		}
		public static Vector3 Normalize(Vector3 vector1, Vector3 vector2){
			return Round(Vector3.Normalize(vector1 - vector2));
		}
        public static Vector3 NormalizeIgnoreY(Vector3 vector1, Vector3 vector2)
        {
            vector1.y = 0;
            vector2.y = 0;
            return Round(Vector3.Normalize(vector1 - vector2));
        }
		public static float Angle(Vector3 vector1, Vector3 vector2){
			return FloatValue.Round(Vector3.Angle(vector1, vector2));
		}
		public static float Distance(Vector3 vector1, Vector3 vector2){
			return FloatValue.Round(Vector3.Distance(vector1, vector2));
		}
        public static float DistanceIgnoreY(Vector3 vector1, Vector3 vector2)
        {
            vector1.y = 0;
            vector2.y = 0;
            return FloatValue.Round(Vector3.Distance(vector1, vector2));
        }
	}
	public static class Vector2Value {
		public static Vector2 Round(Vector2 vector2){
			return new Vector2(FloatValue.Round(vector2.x), FloatValue.Round(vector2.y));
		}
		public static float Angle(Vector2 vector1, Vector2 vector2){
			return FloatValue.Round(Vector2.Angle(vector1, vector2));
		}
		public static float Distance(Vector2 vector1, Vector2 vector2){
			return FloatValue.Round(Vector2.Distance(vector1, vector2));
		}
	}
}
