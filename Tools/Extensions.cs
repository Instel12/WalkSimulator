using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace WalkSimulator.Tools
{
	// Token: 0x0200000A RID: 10
	public static class Extensions
	{
		// Token: 0x06000034 RID: 52 RVA: 0x00002FC8 File Offset: 0x000011C8
		public static T GetOrAddComponent<T>(this GameObject obj) where T : Component
		{
			T component = obj.GetComponent<T>();
			return component ? component : obj.AddComponent<T>();
		}

		// Token: 0x06000035 RID: 53 RVA: 0x00002FF7 File Offset: 0x000011F7
		public static void Obliterate(this GameObject self)
		{
            GameObject.Destroy(self);
		}

		// Token: 0x06000036 RID: 54 RVA: 0x00003001 File Offset: 0x00001201
		public static void Obliterate(this Component self)
		{
			GameObject.Destroy(self);
		}

		// Token: 0x06000037 RID: 55 RVA: 0x0000300C File Offset: 0x0000120C
		public static float Distance(this Vector3 self, Vector3 other)
		{
			return Vector3.Distance(self, other);
		}

		// Token: 0x06000038 RID: 56 RVA: 0x00003028 File Offset: 0x00001228
		public static int Wrap(int x, int min, int max)
		{
			int num = max - min;
			int num2 = (x - min) % num;
			bool flag = num2 < 0;
			if (flag)
			{
				num2 += num;
			}
			return num2 + min;
		}

		// Token: 0x06000039 RID: 57 RVA: 0x00003058 File Offset: 0x00001258
		public static float Map(float x, float a1, float a2, float b1, float b2)
		{
			float num = a2 - a1;
			float num2 = b2 - b1;
			float num3 = (x - a1) / num;
			return b1 + num3 * num2;
		}

		// Token: 0x0600003A RID: 58 RVA: 0x00003084 File Offset: 0x00001284
		public static byte[] StringToBytes(this string str)
		{
			return Encoding.UTF8.GetBytes(str);
		}

		// Token: 0x0600003B RID: 59 RVA: 0x000030A4 File Offset: 0x000012A4
		public static string BytesToString(this byte[] bytes)
		{
			return Encoding.UTF8.GetString(bytes);
		}

		// Token: 0x0600003C RID: 60 RVA: 0x000030C4 File Offset: 0x000012C4
		public static void Shuffle<T>(this IList<T> list)
		{
			int i = list.Count;
			while (i > 1)
			{
				i--;
				int num = Extensions.rng.Next(i + 1);
				T t = list[num];
				list[num] = list[i];
				list[i] = t;
			}
		}

		// Token: 0x04000020 RID: 32
		private static System.Random rng = new System.Random();
	}
}
