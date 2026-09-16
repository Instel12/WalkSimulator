using System;
using System.Collections;
using GorillaLocomotion;
using UnityEngine;
using WalkSimulator.Animators;
using WalkSimulator.Patches;

namespace WalkSimulator.Rigging
{
	// Token: 0x0200000B RID: 11
	public class HandDriver : MonoBehaviour
	{
		// Token: 0x0600003E RID: 62 RVA: 0x00003124 File Offset: 0x00001324
		public void Init(bool isLeft)
		{
			this.isLeft = isLeft;
			this.defaultOffset = new Vector3(isLeft ? -0.25f : 0.25f, -0.45f, 0.2f);
			this.handMap = (isLeft ? GorillaTagger.Instance.offlineVRRig.leftHand : GorillaTagger.Instance.offlineVRRig.rightHand);
			this.head = Rig.Instance.head;
			this.body = Rig.Instance.body;
			this.controller = (isLeft ? GTPlayer.Instance.LeftHand.controllerTransform : GTPlayer.Instance.RightHand.controllerTransform);
			this.handMap = (isLeft ? GorillaTagger.Instance.offlineVRRig.leftHand : GorillaTagger.Instance.offlineVRRig.rightHand);
			base.transform.position = this.DefaultPosition;
			this.targetPosition = this.DefaultPosition;
			this.lastSnap = this.DefaultPosition;
			this.hit = this.DefaultPosition;
			this.up = Vector3.up;
		}

		// Token: 0x0600003F RID: 63 RVA: 0x00003230 File Offset: 0x00001430
		private void FixedUpdate()
		{
			base.transform.position = Vector3.Lerp(base.transform.position, this.targetPosition, this.followRate);
			base.transform.LookAt(this.lookAt, this.up);
			bool flag = this.hideControllerTransform;
			if (flag)
			{
				this.controller.position = this.body.position;
			}
			else
			{
				this.controller.position = base.transform.position;
			}
			bool flag2 = this.isLeft;
			if (flag2)
			{
				FingerPatch.forceLeftGrip = this.grip;
				FingerPatch.forceLeftPrimary = this.primary;
				FingerPatch.forceLeftSecondary = this.secondary;
				FingerPatch.forceLeftTrigger = this.trigger;
			}
			else
			{
				FingerPatch.forceRightGrip = this.grip;
				FingerPatch.forceRightPrimary = this.primary;
				FingerPatch.forceRightSecondary = this.secondary;
				FingerPatch.forceRightTrigger = this.trigger;
			}
		}

		// Token: 0x06000040 RID: 64 RVA: 0x00003320 File Offset: 0x00001520
		private void OnEnable()
		{
			object[] array = new object[2];
			array[0] = "  Current animator:";
			int num = 1;
			AnimatorBase animator = Rig.Instance.Animator;
			array[num] = ((animator != null) ? animator.name : null);
			Logging.Debug(array);
			Logging.Debug(new object[]
			{
				"  body:",
				this.body == null
			});
			bool flag = !this.body || Rig.Instance.Animator == null;
			if (!flag)
			{
				Logging.Debug(new object[]
				{
					"  Enabling HandDriver",
					base.name
				});
				base.transform.position = this.DefaultPosition;
				this.targetPosition = this.DefaultPosition;
				try
				{
					this.handMap.overrideTarget = base.transform;
				}
				catch (Exception e)
				{
					Logging.Exception(e);
				}
			}
		}

		// Token: 0x06000041 RID: 65 RVA: 0x00003410 File Offset: 0x00001610
		private IEnumerator Disable(Action<HandDriver> onDisable)
		{
			base.transform.position = this.DefaultPosition;
			yield return new WaitForSeconds(0.1f);
			this.handMap.overrideTarget = null;
			base.enabled = false;
			if (onDisable != null)
			{
				onDisable.Invoke(this);
			}
			yield break;
		}

		// Token: 0x06000042 RID: 66 RVA: 0x00003428 File Offset: 0x00001628
		public void Reset()
		{
			this.grip = (this.trigger = (this.primary = false));
			this.targetPosition = this.DefaultPosition;
			this.lookAt = this.targetPosition + this.body.forward;
			this.up = (this.isLeft ? this.body.right : (-this.body.right));
			this.hideControllerTransform = true;
			this.grounded = false;
		}

		// Token: 0x17000005 RID: 5
		// (get) Token: 0x06000043 RID: 67 RVA: 0x000034B0 File Offset: 0x000016B0
		public Vector3 DefaultPosition
		{
			get
			{
				return this.body.TransformPoint(this.defaultOffset);
			}
		}

		// Token: 0x04000021 RID: 33
		public bool grip;

		// Token: 0x04000022 RID: 34
		public bool trigger;

		// Token: 0x04000023 RID: 35
		public bool primary;

		// Token: 0x04000024 RID: 36
		public bool secondary;

		// Token: 0x04000025 RID: 37
		public bool isLeft;

		// Token: 0x04000026 RID: 38
		public bool grounded;

		// Token: 0x04000027 RID: 39
		public bool hideControllerTransform = true;

		// Token: 0x04000028 RID: 40
		public Vector3 targetPosition;

		// Token: 0x04000029 RID: 41
		public Vector3 lookAt;

		// Token: 0x0400002A RID: 42
		public Vector3 up;

		// Token: 0x0400002B RID: 43
		public float followRate = 0.1f;

		// Token: 0x0400002C RID: 44
		public Vector3 hit;

		// Token: 0x0400002D RID: 45
		public Vector3 lastSnap;

		// Token: 0x0400002E RID: 46
		public Vector3 normal;

		// Token: 0x0400002F RID: 47
		private VRMap handMap;

		// Token: 0x04000030 RID: 48
		private Vector3 defaultOffset;

		// Token: 0x04000031 RID: 49
		private Transform head;

		// Token: 0x04000032 RID: 50
		private Transform body;

		// Token: 0x04000033 RID: 51
		private Transform controller;
	}
}
