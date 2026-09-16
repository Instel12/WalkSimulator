using System;
using UnityEngine;
using WalkSimulator.Rigging;

namespace WalkSimulator.Animators
{
	// Token: 0x02000012 RID: 18
	public class EmoteAnimator : AnimatorBase
	{
		// Token: 0x06000072 RID: 114 RVA: 0x00004C10 File Offset: 0x00002E10
		public override void Animate()
		{
			switch (this.emote)
			{
			case EmoteAnimator.Emote.Wave:
				this.handPositioner = new Func<HandDriver, float, EmoteAnimator.HandPositionInfo>(this.WavePositioner);
				break;
			case EmoteAnimator.Emote.Point:
				this.handPositioner = new Func<HandDriver, float, EmoteAnimator.HandPositionInfo>(this.PointPositioner);
				break;
			case EmoteAnimator.Emote.ThumbsUp:
				this.handPositioner = new Func<HandDriver, float, EmoteAnimator.HandPositionInfo>(this.ThumbsUpPositioner);
				break;
			case EmoteAnimator.Emote.ThumbsDown:
				this.handPositioner = new Func<HandDriver, float, EmoteAnimator.HandPositionInfo>(this.ThumbsDownPositioner);
				break;
			case EmoteAnimator.Emote.Shrug:
				this.handPositioner = new Func<HandDriver, float, EmoteAnimator.HandPositionInfo>(this.ShrugPositioner);
				break;
			case EmoteAnimator.Emote.Dance:
				this.handPositioner = new Func<HandDriver, float, EmoteAnimator.HandPositionInfo>(this.DancePositioner);
				break;
			}
			this.AnimateBody();
			this.AnimateHand(this.leftHand);
			this.AnimateHand(this.rightHand);
		}

		// Token: 0x06000073 RID: 115 RVA: 0x00004CE0 File Offset: 0x00002EE0
		private void AnimateBody()
		{
			this.rig.active = true;
			this.rig.useGravity = false;
			bool flag = this.emote == EmoteAnimator.Emote.Dance;
			if (flag)
			{
				float num = Time.time - this.startTime;
				Vector3 vector;
				vector = new Vector3(0f, Mathf.Sin(num * 10f) * 0.1f, 0f);
				this.rig.targetPosition = this.startingPosition + vector;
			}
			else
			{
				this.rig.targetPosition = this.startingPosition;
			}
		}

		// Token: 0x06000074 RID: 116 RVA: 0x00004D74 File Offset: 0x00002F74
		private void AnimateHand(HandDriver hand)
		{
			EmoteAnimator.HandPositionInfo handPositionInfo = this.handPositioner.Invoke(hand, Time.time - this.startTime);
			bool flag = !handPositionInfo.used;
			if (!flag)
			{
				hand.grip = handPositionInfo.grip;
				hand.trigger = handPositionInfo.trigger;
				hand.primary = handPositionInfo.thumb;
				hand.targetPosition = handPositionInfo.position;
				hand.lookAt = handPositionInfo.lookAt;
				hand.up = handPositionInfo.up;
			}
		}

		// Token: 0x06000075 RID: 117 RVA: 0x00004DF4 File Offset: 0x00002FF4
		private EmoteAnimator.HandPositionInfo DancePositioner(HandDriver hand, float t)
		{
			bool flag = Time.time - this.lastDanceSwitch > this.danceSwitchRate;
			if (flag)
			{
				this.dance = UnityEngine.Random.Range(0, 4);
				this.lastDanceSwitch = Time.time;
			}
			EmoteAnimator.HandPositionInfo result = default(EmoteAnimator.HandPositionInfo);
			float num = hand.isLeft ? 0f : 3.1415927f;
			switch (this.dance)
			{
			case 0:
			{
				Vector3 lookAt = hand.isLeft ? (hand.targetPosition + this.head.right + this.head.forward) : (hand.targetPosition - this.head.right + this.head.forward);
				Vector3 vector;
				vector = new Vector3(hand.isLeft ? -0.2f : 0.2f, -0.3f, 0.2f);
				vector.y += Mathf.Sin(t * 10f + num) * 0.1f;
				EmoteAnimator.HandPositionInfo handPositionInfo = new EmoteAnimator.HandPositionInfo
				{
					position = this.head.TransformPoint(vector),
					lookAt = lookAt,
					up = this.head.up,
					grip = true,
					trigger = true,
					thumb = true,
					used = true
				};
				result = handPositionInfo;
				break;
			}
			case 1:
			{
				Vector3 lookAt = hand.targetPosition + this.head.up;
				Vector3 vector;
				vector = new Vector3(hand.isLeft ? -0.2f : 0.2f, -0.2f, 0.3f);
				vector.z += Mathf.Sin(t * 10f + num) * 0.1f;
				EmoteAnimator.HandPositionInfo handPositionInfo = new EmoteAnimator.HandPositionInfo
				{
					position = this.head.TransformPoint(vector),
					lookAt = lookAt,
					up = (hand.isLeft ? this.head.right : (-this.head.right)),
					grip = false,
					trigger = false,
					thumb = false,
					used = true
				};
				result = handPositionInfo;
				break;
			}
			case 2:
			{
				Vector3 lookAt = hand.targetPosition + this.head.up;
				Vector3 vector;
				vector = new Vector3(hand.isLeft ? -0.2f : 0.2f, -0.3f, 0.2f);
				vector.y += Mathf.Sin(t * 10f + num) * 0.1f;
				EmoteAnimator.HandPositionInfo handPositionInfo = new EmoteAnimator.HandPositionInfo
				{
					position = this.head.TransformPoint(vector),
					lookAt = lookAt,
					up = (hand.isLeft ? this.head.right : (-this.head.right)),
					grip = true,
					trigger = false,
					thumb = true,
					used = true
				};
				result = handPositionInfo;
				break;
			}
			case 3:
			{
				Vector3 lookAt = hand.targetPosition + this.head.up;
				Vector3 vector;
				vector = new Vector3(hand.isLeft ? -0.2f : 0.2f, -0.2f, 0.3f);
				vector.x += Mathf.Cos(t * 10f) * 0.1f;
				vector.z += Mathf.Sin(t * 10f) * 0.1f;
				EmoteAnimator.HandPositionInfo handPositionInfo = new EmoteAnimator.HandPositionInfo
				{
					position = this.head.TransformPoint(vector),
					lookAt = lookAt,
					up = (hand.isLeft ? this.head.right : (-this.head.right)),
					grip = true,
					trigger = true,
					thumb = true,
					used = true
				};
				result = handPositionInfo;
				break;
			}
			}
			return result;
		}

		// Token: 0x06000076 RID: 118 RVA: 0x00005208 File Offset: 0x00003408
		private EmoteAnimator.HandPositionInfo ThumbsDownPositioner(HandDriver hand, float _)
		{
			Vector3 lookAt = hand.isLeft ? (hand.targetPosition + this.head.right) : (hand.targetPosition - this.head.right);
			return new EmoteAnimator.HandPositionInfo
			{
				position = this.head.TransformPoint(new Vector3(hand.isLeft ? -0.2f : 0.2f, 0f, 0.4f)),
				lookAt = lookAt,
				up = -this.head.up,
				grip = true,
				trigger = true,
				used = true
			};
		}

		// Token: 0x06000077 RID: 119 RVA: 0x000052C8 File Offset: 0x000034C8
		private EmoteAnimator.HandPositionInfo ThumbsUpPositioner(HandDriver hand, float _)
		{
			Vector3 lookAt = hand.isLeft ? (hand.targetPosition + this.head.right) : (hand.targetPosition - this.head.right);
			return new EmoteAnimator.HandPositionInfo
			{
				position = this.head.TransformPoint(new Vector3(hand.isLeft ? -0.2f : 0.2f, 0f, 0.4f)),
				lookAt = lookAt,
				up = this.head.up,
				grip = true,
				trigger = true,
				used = true
			};
		}

		// Token: 0x06000078 RID: 120 RVA: 0x00005384 File Offset: 0x00003584
		private EmoteAnimator.HandPositionInfo ShrugPositioner(HandDriver hand, float _)
		{
			Vector3 lookAt = hand.isLeft ? (hand.targetPosition - this.head.right + this.head.forward) : (hand.targetPosition + this.head.right + this.head.forward);
			return new EmoteAnimator.HandPositionInfo
			{
				position = this.body.TransformPoint(new Vector3(hand.isLeft ? -0.4f : 0.4f, 0f, 0.2f)),
				lookAt = lookAt,
				up = -this.head.forward,
				used = true
			};
		}

		// Token: 0x06000079 RID: 121 RVA: 0x00005454 File Offset: 0x00003654
		private EmoteAnimator.HandPositionInfo PointPositioner(HandDriver hand, float __)
		{
			bool isLeft = hand.isLeft;
			EmoteAnimator.HandPositionInfo result;
			if (isLeft)
			{
				result = new EmoteAnimator.HandPositionInfo
				{
					used = true,
					position = hand.DefaultPosition,
					lookAt = hand.targetPosition + this.head.forward,
					up = this.head.right
				};
			}
			else
			{
				result = new EmoteAnimator.HandPositionInfo
				{
					position = this.head.TransformPoint(new Vector3(0.25f, 0f, 0.7f)),
					lookAt = hand.targetPosition + this.head.forward,
					up = -this.head.right,
					grip = true,
					trigger = false,
					used = true
				};
			}
			return result;
		}

		// Token: 0x0600007A RID: 122 RVA: 0x0000553C File Offset: 0x0000373C
		private EmoteAnimator.HandPositionInfo WavePositioner(HandDriver hand, float time)
		{
			bool isLeft = hand.isLeft;
			EmoteAnimator.HandPositionInfo result;
			if (isLeft)
			{
				result = new EmoteAnimator.HandPositionInfo
				{
					used = true,
					position = hand.DefaultPosition,
					lookAt = hand.targetPosition + this.head.forward,
					up = this.head.right
				};
			}
			else
			{
				Vector3 vector;
				vector = new Vector3(0.25f, 0f, 0.2f);
				float num = 0.25f;
				float num2 = 0.25f;
				float num3 = 5f;
				float num4 = Mathf.Sin(Time.time * num3);
				float num5 = Mathf.Cos(Time.time * num3);
				Vector3 vector2 = Vector3.zero;
				vector2.x = num4 * num;
				vector2.y = Mathf.Abs(num5) * num2;
				vector2 += vector;
				Vector3 vector3 = hand.targetPosition - this.head.TransformPoint(vector - Vector3.up * 0.25f);
				result = new EmoteAnimator.HandPositionInfo
				{
					position = this.head.TransformPoint(vector2),
					lookAt = hand.targetPosition + vector3,
					up = -this.head.right,
					used = true
				};
			}
			return result;
		}

		// Token: 0x0600007B RID: 123 RVA: 0x0000569F File Offset: 0x0000389F
		public override void Setup()
		{
			Logging.Debug(new object[]
			{
				"===SETUP==="
			});
			base.Start();
			this.startingPosition = this.body.position;
		}

		// Token: 0x0400006B RID: 107
		public EmoteAnimator.Emote emote;

		// Token: 0x0400006C RID: 108
		private float startTime;

		// Token: 0x0400006D RID: 109
		private Func<HandDriver, float, EmoteAnimator.HandPositionInfo> handPositioner;

		// Token: 0x0400006E RID: 110
		private Vector3 startingPosition;

		// Token: 0x0400006F RID: 111
		private float lastDanceSwitch;

		// Token: 0x04000070 RID: 112
		private float danceSwitchRate = 1.5f;

		// Token: 0x04000071 RID: 113
		private int dance;

		// Token: 0x0200001B RID: 27
		public enum Emote
		{
			// Token: 0x04000094 RID: 148
			Wave,
			// Token: 0x04000095 RID: 149
			Point,
			// Token: 0x04000096 RID: 150
			ThumbsUp,
			// Token: 0x04000097 RID: 151
			ThumbsDown,
			// Token: 0x04000098 RID: 152
			Shrug,
			// Token: 0x04000099 RID: 153
			Dance
		}

		// Token: 0x0200001C RID: 28
		private struct HandPositionInfo
		{
			// Token: 0x0400009A RID: 154
			public Vector3 position;

			// Token: 0x0400009B RID: 155
			public Vector3 lookAt;

			// Token: 0x0400009C RID: 156
			public Vector3 up;

			// Token: 0x0400009D RID: 157
			public bool grip;

			// Token: 0x0400009E RID: 158
			public bool trigger;

			// Token: 0x0400009F RID: 159
			public bool thumb;

			// Token: 0x040000A0 RID: 160
			public bool used;
		}
	}
}
