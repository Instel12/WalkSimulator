using System;
using UnityEngine;
using WalkSimulator.Rigging;

namespace WalkSimulator.Animators
{
	// Token: 0x02000011 RID: 17
	public abstract class AnimatorBase : MonoBehaviour
	{
		// Token: 0x0600006D RID: 109 RVA: 0x00004B2C File Offset: 0x00002D2C
		protected virtual void Start()
		{
			Logging.Debug(new object[]
			{
				"==START=="
			});
			this.rig = Rig.Instance;
			this.body = this.rig.body;
			this.head = this.rig.head;
			this.rigidbody = this.rig.rigidbody;
			this.leftHand = this.rig.leftHand;
			this.rightHand = this.rig.rightHand;
		}

		// Token: 0x0600006E RID: 110
		public abstract void Setup();

		// Token: 0x0600006F RID: 111 RVA: 0x00004BB0 File Offset: 0x00002DB0
		public virtual void Cleanup()
		{
			base.enabled = false;
			this.rig.active = false;
			this.rig.useGravity = true;
			this.rig.headDriver.turn = true;
			this.leftHand.Reset();
			this.rightHand.Reset();
		}

		// Token: 0x06000070 RID: 112
		public abstract void Animate();

		// Token: 0x04000065 RID: 101
		protected Transform body;

		// Token: 0x04000066 RID: 102
		protected Transform head;

		// Token: 0x04000067 RID: 103
		protected Rigidbody rigidbody;

		// Token: 0x04000068 RID: 104
		protected HandDriver leftHand;

		// Token: 0x04000069 RID: 105
		protected HandDriver rightHand;

		// Token: 0x0400006A RID: 106
		protected Rig rig;
	}
}
