using System;

namespace WalkSimulatorClassic
{
	// Token: 0x02000002 RID: 2
	public static class Sounds
	{
		// Token: 0x06000001 RID: 1 RVA: 0x00002050 File Offset: 0x00000250
		public static void Play(int sound, float volume = 0.1f)
		{
			GorillaTagger.Instance.offlineVRRig.PlayHandTapLocal(sound, false, volume);
		}
	}
}
