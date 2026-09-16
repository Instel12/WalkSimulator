using System;
using UnityEngine;
using UnityEngine.InputSystem;
using WalkSimulator.Rigging;
using WalkSimulator.Tools;

namespace WalkSimulator.Animators
{
	// Token: 0x02000013 RID: 19
	public class FlyAnimator : AnimatorBase
	{
		// Token: 0x0600007D RID: 125 RVA: 0x000056E2 File Offset: 0x000038E2
		public override void Animate()
		{
			this.AnimateBody();
			this.AnimateHands();
		}

		// Token: 0x0600007E RID: 126 RVA: 0x000056F4 File Offset: 0x000038F4
		private void Update()
		{
			this.speed += Mouse.current.scroll.ReadValue().y / 10f;
			this.speed = Mathf.Clamp(this.speed, this.minSpeed, this.maxSpeed);
		}

		// Token: 0x0600007F RID: 127 RVA: 0x00005748 File Offset: 0x00003948
		private void AnimateBody()
		{
			this.rig.active = true;
			this.rig.useGravity = false;
			this.rig.targetPosition = this.body.TransformPoint(InputHandler.inputDirection * this.speed);
		}

		// Token: 0x06000080 RID: 128 RVA: 0x00005794 File Offset: 0x00003994
		private void AnimateHands()
		{
			this.leftHand.followRate = (this.rightHand.followRate = Extensions.Map(this.speed, this.minSpeed, this.maxSpeed, 0f, 1f));
			this.leftHand.targetPosition = this.leftHand.DefaultPosition;
			this.rightHand.targetPosition = this.rightHand.DefaultPosition;
			this.leftHand.lookAt = this.leftHand.targetPosition + this.body.forward;
			this.rightHand.lookAt = this.rightHand.targetPosition + this.body.forward;
			this.leftHand.up = this.body.right;
			this.rightHand.up = -this.body.right;
		}

		// Token: 0x06000081 RID: 129 RVA: 0x00005888 File Offset: 0x00003A88
		public override void Cleanup()
		{
			base.Cleanup();
			this.leftHand.followRate = (this.rightHand.followRate = 0.1f);
		}

		// Token: 0x06000082 RID: 130 RVA: 0x000058BB File Offset: 0x00003ABB
		public override void Setup()
		{
			HeadDriver.Instance.LockCursor = true;
			HeadDriver.Instance.turn = true;
		}

		// Token: 0x04000072 RID: 114
		private float speed = 1f;

		// Token: 0x04000073 RID: 115
		private float minSpeed = 0f;

		// Token: 0x04000074 RID: 116
		private float maxSpeed = 5f;
	}
}
