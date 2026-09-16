using System;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace WalkSimulator.Tools
{
	// Token: 0x02000008 RID: 8
	public static class AssetUtils
	{
		// Token: 0x0600002A RID: 42 RVA: 0x00002D2C File Offset: 0x00000F2C
		private static string FormatPath(string path)
		{
			return path.Replace("/", ".").Replace("\\", ".");
		}

		// Token: 0x0600002B RID: 43 RVA: 0x00002D60 File Offset: 0x00000F60
		public static AssetBundle LoadAssetBundle(string path)
		{
			path = AssetUtils.FormatPath(path);
			Stream manifestResourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(path);
			AssetBundle result = AssetBundle.LoadFromStream(manifestResourceStream);
			manifestResourceStream.Close();
			return result;
		}

		// Token: 0x0600002C RID: 44 RVA: 0x00002D98 File Offset: 0x00000F98
		public static string[] GetResourceNames()
		{
			Assembly callingAssembly = Assembly.GetCallingAssembly();
			string[] manifestResourceNames = callingAssembly.GetManifestResourceNames();
			bool flag = manifestResourceNames == null;
			string[] result;
			if (flag)
			{
				Console.WriteLine("No manifest resources found.");
				result = null;
			}
			else
			{
				result = manifestResourceNames;
			}
			return result;
		}
	}
}
