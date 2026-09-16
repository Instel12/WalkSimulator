using System;
using System.Reflection;
using HarmonyLib;

namespace WalkSimulator
{
	// Token: 0x02000003 RID: 3
	public class HarmonyPatches
	{
		// Token: 0x17000001 RID: 1
		// (get) Token: 0x06000002 RID: 2 RVA: 0x00002066 File Offset: 0x00000266
		// (set) Token: 0x06000003 RID: 3 RVA: 0x0000206D File Offset: 0x0000026D
		public static bool IsPatched { get; private set; }

		// Token: 0x06000004 RID: 4 RVA: 0x00002078 File Offset: 0x00000278
		internal static void ApplyHarmonyPatches()
		{
			bool flag = !HarmonyPatches.IsPatched;
			if (flag)
			{
				bool flag2 = HarmonyPatches.instance == null;
				if (flag2)
				{
					HarmonyPatches.instance = new Harmony("com.kylethescientist.gorillatag.walksimulator");
				}
				HarmonyPatches.instance.PatchAll(Assembly.GetExecutingAssembly());
				HarmonyPatches.IsPatched = true;
			}
		}

		// Token: 0x06000005 RID: 5 RVA: 0x000020C8 File Offset: 0x000002C8
		internal static void RemoveHarmonyPatches()
		{
			bool flag = HarmonyPatches.instance != null && HarmonyPatches.IsPatched;
			if (flag)
			{
				HarmonyPatches.instance.UnpatchSelf();
				HarmonyPatches.IsPatched = false;
			}
		}

		// Token: 0x04000001 RID: 1
		private static Harmony instance;

		// Token: 0x04000003 RID: 3
		public const string InstanceId = "com.kylethescientist.gorillatag.walksimulator";
	}
}
