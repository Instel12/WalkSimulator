using System;
using GorillaLocomotion;
using UnityEngine;
using UnityEngine.InputSystem;
using WalkSimulator.Rigging;

namespace WalkSimulator.Animators
{
	// Token: 0x02000015 RID: 21
	public class PoseAnimator : AnimatorBase
	{
		// Token: 0x0600008F RID: 143 RVA: 0x00005C11 File Offset: 0x00003E11
		public override void Animate()
		{
			this.rig.headDriver.turn = false;
			this.AnimateBody();
			this.AnimateHands();
		}

		// Token: 0x06000090 RID: 144 RVA: 0x00005C34 File Offset: 0x00003E34
		private void Update()
		{
			bool wasPressedThisFrame = Keyboard.current.qKey.wasPressedThisFrame;
			if (wasPressedThisFrame)
			{
				HandDriver handDriver = this.main;
				this.main = this.secondary;
				this.secondary = handDriver;
			}
			bool isPressed = Keyboard.current.rKey.isPressed;
			if (isPressed)
			{
				this.RotateHand();
			}
			else
			{
				this.PositionHand();
			}
			Vector3 vector = this.main.isLeft ? this.lookAtLeft : this.lookAtRight;
			float num = this.main.isLeft ? this.zRotationLeft : this.zRotationRight;
			float num2 = (float)(this.main.isLeft ? -1 : 1);
			this.main.up = Quaternion.AngleAxis(num * num2, vector) * this.head.up;
			this.main.trigger = Mouse.current.leftButton.isPressed;
			this.main.grip = Mouse.current.rightButton.isPressed;
			this.main.primary = (Mouse.current.backButton.isPressed || Keyboard.current.leftBracketKey.isPressed);
			this.main.secondary = (Mouse.current.forwardButton.isPressed || Keyboard.current.rightBracketKey.isPressed);
		}

		// Token: 0x06000091 RID: 145 RVA: 0x00005D98 File Offset: 0x00003F98
		private void RotateHand()
		{
			this.eulerAngles.x = this.eulerAngles.x - Mouse.current.delta.value.y / 10f;
			bool flag = this.eulerAngles.x > 180f;
			if (flag)
			{
				this.eulerAngles.x = this.eulerAngles.x - 360f;
			}
			this.eulerAngles.x = Mathf.Clamp(this.eulerAngles.x, -85f, 85f);
			this.eulerAngles.y = this.eulerAngles.y + Mouse.current.delta.value.x / 10f;
			bool flag2 = this.eulerAngles.y > 180f;
			if (flag2)
			{
				this.eulerAngles.y = this.eulerAngles.y - 360f;
			}
			this.eulerAngles.y = Mathf.Clamp(this.eulerAngles.y, -85f, 85f);
			bool isLeft = this.main.isLeft;
			if (isLeft)
			{
				this.lookAtLeft = Quaternion.Euler(this.eulerAngles) * this.head.forward;
				this.zRotationLeft += Mouse.current.scroll.ReadValue().y / 5f;
			}
			else
			{
				this.lookAtRight = Quaternion.Euler(this.eulerAngles) * this.head.forward;
				this.zRotationRight += Mouse.current.scroll.ReadValue().y / 5f;
			}
		}

		// Token: 0x06000092 RID: 146 RVA: 0x00005F4C File Offset: 0x0000414C
		private void PositionHand()
		{
			Vector3 vector = this.main.isLeft ? this.offsetLeft : this.offsetRight;
			vector.z += Mouse.current.scroll.ReadValue().y / 10f;
			bool wasPressedThisFrame = Keyboard.current.upArrowKey.wasPressedThisFrame;
			if (wasPressedThisFrame)
			{
				vector.z += 0.1f;
			}
			bool wasPressedThisFrame2 = Keyboard.current.downArrowKey.wasPressedThisFrame;
			if (wasPressedThisFrame2)
			{
				vector.z -= 0.1f;
			}
			vector.z = Mathf.Clamp(vector.z, -0.25f, 0.75f);
			vector.x += Mouse.current.delta.ReadValue().x / 1000f;
			vector.x = Mathf.Clamp(vector.x, -0.5f, 0.5f);
			vector.y += Mouse.current.delta.ReadValue().y / 1000f;
			vector.y = Mathf.Clamp(vector.y, -0.5f, 0.5f);
			bool isLeft = this.main.isLeft;
			if (isLeft)
			{
				this.offsetLeft = vector;
			}
			else
			{
				this.offsetRight = vector;
			}
		}

		// Token: 0x06000093 RID: 147 RVA: 0x0000609E File Offset: 0x0000429E
		private void AnimateBody()
		{
			this.rig.active = true;
			this.rig.useGravity = false;
			this.rig.targetPosition = this.body.position;
		}

		// Token: 0x06000094 RID: 148 RVA: 0x000060D0 File Offset: 0x000042D0
		private void AnimateHands()
		{
			float num = (float)(this.main.isLeft ? -1 : 1);
			Vector3 vector = this.main.isLeft ? this.offsetLeft : this.offsetRight;
			Vector3 vector2 = this.main.isLeft ? this.lookAtLeft : this.lookAtRight;
			this.main.targetPosition = this.body.TransformPoint(new Vector3(num * 0.2f, 0.1f, 0.3f) + vector);
			this.main.lookAt = this.main.targetPosition + vector2;
			this.main.hideControllerTransform = false;
		}

		// Token: 0x06000095 RID: 149 RVA: 0x00006184 File Offset: 0x00004384
		public override void Setup()
		{
			base.Start();
			HeadDriver.Instance.LockCursor = true;
			this.main = this.rightHand;
			this.secondary = this.leftHand;
			this.offsetLeft = Vector3.zero;
			this.lookAtLeft = this.head.forward;
			this.offsetRight = Vector3.zero;
			this.lookAtRight = this.head.forward;
			this.secondary.targetPosition = this.secondary.DefaultPosition + Vector3.up * 0.2f * GTPlayer.Instance.scale;
			this.secondary.lookAt = this.secondary.targetPosition + this.head.forward;
			this.secondary.up = this.body.right * (float)(this.main.isLeft ? -1 : 1);
		}

		// Token: 0x04000077 RID: 119
		private Vector3 offsetLeft;

		// Token: 0x04000078 RID: 120
		private Vector3 lookAtLeft = Vector3.forward;

		// Token: 0x04000079 RID: 121
		private Vector3 offsetRight;

		// Token: 0x0400007A RID: 122
		private Vector3 lookAtRight = Vector3.forward;

		// Token: 0x0400007B RID: 123
		private float zRotationLeft;

		// Token: 0x0400007C RID: 124
		private float zRotationRight;

		// Token: 0x0400007D RID: 125
		private HandDriver main;

		// Token: 0x0400007E RID: 126
		private HandDriver secondary;

		// Token: 0x0400007F RID: 127
		private Vector3 eulerAngles;
	}
}
