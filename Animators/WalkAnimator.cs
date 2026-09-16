using System;
using GorillaLocomotion;
using UnityEngine;
using UnityEngine.InputSystem;
using WalkSimulator.Rigging;
using WalkSimulator.Tools;

namespace WalkSimulator.Animators
{
	// Token: 0x02000016 RID: 22
	public class WalkAnimator : AnimatorBase
	{
		// Token: 0x06000097 RID: 151 RVA: 0x000062A0 File Offset: 0x000044A0
		public override void Animate()
		{
			this.MoveBody();
			this.AnimateHands();
		}

		// Token: 0x06000098 RID: 152 RVA: 0x000062B4 File Offset: 0x000044B4
		private void Update()
		{
			bool flag = !Plugin.Instance.Enabled;
			if (!flag)
			{
				bool flag2 = !this.hasJumped && this.rig.onGround && Keyboard.current.spaceKey.wasPressedThisFrame;
				if (flag2)
				{
					this.hasJumped = true;
					this.onJumpCooldown = true;
					this.jumpTime = Time.time;
					this.rig.active = false;
					this.rigidbody.AddForce(Vector3.up * 250f * GTPlayer.Instance.scale, ForceMode.Impulse);
				}
				bool flag3 = (this.hasJumped && !this.rig.onGround) || Time.time - this.jumpTime > 1f;
				if (flag3)
				{
					this.onJumpCooldown = false;
				}
				bool flag4 = this.rig.onGround && !this.onJumpCooldown;
				if (flag4)
				{
					this.hasJumped = false;
				}
			}
		}

		// Token: 0x06000099 RID: 153 RVA: 0x000063B0 File Offset: 0x000045B0
		public void MoveBody()
		{
			this.rig.active = (this.rig.onGround && !this.hasJumped);
			this.rig.useGravity = !this.rig.onGround;
			bool flag = !this.rig.onGround;
			if (!flag)
			{
				bool notMoving = this.NotMoving;
				float num;
				float num2;
				float num3;
				if (notMoving)
				{
					num = 0.5f;
					num2 = 0.55f;
					num3 = Time.time * 3.1415927f * 2f;
				}
				else
				{
					num = 0.3f;
					num2 = 0.8f;
					num3 = this.walkCycleTime * 3.1415927f * 2f;
				}
				bool isPressed = Keyboard.current.ctrlKey.isPressed;
				if (isPressed)
				{
					num -= 0.3f;
					num2 -= 0.3f;
				}
				this.targetHeight = Extensions.Map(Mathf.Sin(num3), -1f, 1f, num, num2);
				this.height = this.targetHeight;
				Vector3 vector = this.rig.lastGroundPosition + Vector3.up * this.height * GTPlayer.Instance.scale;
				Vector3 vector2 = this.body.TransformDirection(InputHandler.inputDirectionNoY);
				vector2.y = 0f;
				bool flag2 = Vector3.Dot(this.rig.lastNormal, Vector3.up) > 0.3f;
				if (flag2)
				{
					vector2 = Vector3.ProjectOnPlane(vector2, this.rig.lastNormal);
				}
				vector2 *= GTPlayer.Instance.scale;
				float num4 = this.IsSprinting ? (this.speed * 3f) : this.speed;
				vector += vector2 * num4 / 10f;
				this.rig.targetPosition = vector;
			}
		}

		// Token: 0x0600009A RID: 154 RVA: 0x00006590 File Offset: 0x00004790
		private void AnimateHands()
		{
			this.leftHand.lookAt = this.leftHand.targetPosition + this.body.forward;
			this.rightHand.lookAt = this.rightHand.targetPosition + this.body.forward;
			this.leftHand.up = this.body.right;
			this.rightHand.up = -this.body.right;
			bool flag = !this.rig.onGround;
			if (flag)
			{
				this.leftHand.grounded = false;
				this.rightHand.grounded = false;
				Vector3 vector = Vector3.up * 0.2f * GTPlayer.Instance.scale;
				this.leftHand.targetPosition = this.leftHand.DefaultPosition;
				this.rightHand.targetPosition = this.rightHand.DefaultPosition + vector;
			}
			else
			{
				this.UpdateHitInfo(this.leftHand);
				this.UpdateHitInfo(this.rightHand);
				bool notMoving = this.NotMoving;
				if (notMoving)
				{
					this.leftHand.targetPosition = this.leftHand.hit;
					this.rightHand.targetPosition = this.rightHand.hit;
				}
				else
				{
					bool flag2 = !this.leftHand.grounded && !this.rightHand.grounded;
					if (flag2)
					{
						this.leftHand.grounded = true;
						this.leftHand.lastSnap = this.leftHand.hit;
						this.leftHand.targetPosition = this.leftHand.hit;
						this.rightHand.lastSnap = this.rightHand.hit;
						this.rightHand.targetPosition = this.rightHand.hit;
					}
					this.AnimateHand(this.leftHand, this.rightHand);
					this.AnimateHand(this.rightHand, this.leftHand);
				}
			}
		}

		// Token: 0x0600009B RID: 155 RVA: 0x000067A0 File Offset: 0x000049A0
		private void UpdateHitInfo(HandDriver hand)
		{
			float scale = GTPlayer.Instance.scale;
			float num = 0.5f * scale;
			Vector3 smoothedGroundPosition = this.rig.smoothedGroundPosition;
			Vector3 lastNormal = this.rig.lastNormal;
			float x = Mathf.Abs(Vector3.Dot(InputHandler.inputDirectionNoY, Vector3.forward));
			float num2 = Extensions.Map(x, 0f, 1f, 0.4f, 0.5f);
			Vector3 vector = this.body.TransformDirection(InputHandler.inputDirectionNoY * num2);
			vector.y = 0f;
			vector *= scale;
			Vector3 vector2 = Vector3.ProjectOnPlane(hand.DefaultPosition - smoothedGroundPosition + vector, lastNormal);
			vector2 += smoothedGroundPosition + lastNormal * 0.3f * scale;
			RaycastHit raycastHit;
			bool flag = !Physics.Raycast(vector2, -lastNormal, out raycastHit, num, GTPlayer.Instance.locomotionEnabledLayers);
			if (flag)
			{
				bool notMoving = this.NotMoving;
				if (notMoving)
				{
					hand.targetPosition = hand.DefaultPosition;
				}
			}
			else
			{
				hand.hit = raycastHit.point;
				hand.normal = raycastHit.normal;
				hand.lookAt = hand.transform.position + this.body.forward;
			}
		}

		// Token: 0x0600009C RID: 156 RVA: 0x000068F4 File Offset: 0x00004AF4
		private void AnimateHand(HandDriver hand, HandDriver otherHand)
		{
			float x = Mathf.Abs(Vector3.Dot(InputHandler.inputDirectionNoY, Vector3.forward));
			float x2 = Vector3.Dot(this.rig.lastNormal, Vector3.up);
			float num = Extensions.Map(x2, 0f, 1f, 0.1f, 0.6f);
			float num2 = Extensions.Map(x, 0f, 1f, 0.5f, 1.25f);
			num2 *= num * GTPlayer.Instance.scale;
			float num3 = 0.2f * GTPlayer.Instance.scale;
			float num4 = otherHand.hit.Distance(otherHand.lastSnap) / num2;
			bool flag = otherHand.grounded && num4 >= 1f;
			if (flag)
			{
				hand.targetPosition = hand.hit;
				hand.lastSnap = hand.hit;
				hand.grounded = true;
				otherHand.grounded = false;
			}
			else
			{
				bool grounded = otherHand.grounded;
				if (grounded)
				{
					this.walkCycleTime = num4;
					hand.targetPosition = Vector3.Slerp(hand.lastSnap, hand.hit, this.walkCycleTime);
					hand.targetPosition += hand.normal * num3 * Mathf.Sin(this.walkCycleTime);
					hand.grounded = false;
				}
			}
			bool flag2 = hand.targetPosition.Distance(hand.DefaultPosition) > 1f;
			if (flag2)
			{
				hand.targetPosition = hand.DefaultPosition;
			}
		}

		// Token: 0x0600009D RID: 157 RVA: 0x00006A74 File Offset: 0x00004C74
		public override void Setup()
		{
			HeadDriver.Instance.LockCursor = true;
			HeadDriver.Instance.turn = true;
		}

		// Token: 0x1700000C RID: 12
		// (get) Token: 0x0600009E RID: 158 RVA: 0x00006A90 File Offset: 0x00004C90
		private bool IsSprinting
		{
			get
			{
				return Keyboard.current.leftShiftKey.isPressed;
			}
		}

		// Token: 0x1700000D RID: 13
		// (get) Token: 0x0600009F RID: 159 RVA: 0x00006AB4 File Offset: 0x00004CB4
		private bool NotMoving
		{
			get
			{
				return InputHandler.inputDirectionNoY == Vector3.zero;
			}
		}

		// Token: 0x04000080 RID: 128
		private float speed = 4f;

		// Token: 0x04000081 RID: 129
		private float height = 0.4f;

		// Token: 0x04000082 RID: 130
		private float targetHeight;

		// Token: 0x04000083 RID: 131
		private bool hasJumped;

		// Token: 0x04000084 RID: 132
		private bool onJumpCooldown;

		// Token: 0x04000085 RID: 133
		private float jumpTime;

		// Token: 0x04000086 RID: 134
		private float walkCycleTime = 0f;
	}
}
