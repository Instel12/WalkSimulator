using System;
using Cinemachine;
using GorillaLocomotion;
using UnityEngine;
using UnityEngine.InputSystem;

namespace WalkSimulator.Rigging
{
	// Token: 0x0200000C RID: 12
	public class HeadDriver : MonoBehaviour
	{
		// Token: 0x17000006 RID: 6
		// (get) Token: 0x06000045 RID: 69 RVA: 0x000034F0 File Offset: 0x000016F0
		// (set) Token: 0x06000046 RID: 70 RVA: 0x00003508 File Offset: 0x00001708
		public bool LockCursor
		{
			get
			{
				return this._lockCursor;
			}
			set
			{
				this._lockCursor = value;
				bool lockCursor = this._lockCursor;
				if (lockCursor)
				{
					Cursor.lockState = CursorLockMode.Locked;
					Cursor.visible = false;
				}
				else
				{
					Cursor.lockState = 0;
					Cursor.visible = true;
				}
			}
		}

		// Token: 0x06000047 RID: 71 RVA: 0x00003549 File Offset: 0x00001749
		private void Awake()
		{
			HeadDriver.Instance = this;
		}

		// Token: 0x06000048 RID: 72 RVA: 0x00003554 File Offset: 0x00001754
		private void Start()
		{
			Cinemachine3rdPersonFollow cinemachine3rdPersonFollow = GameObject.FindObjectOfType<Cinemachine3rdPersonFollow>();
			this.thirpyTarget = cinemachine3rdPersonFollow.VirtualCamera.Follow;
			Camera componentInParent = cinemachine3rdPersonFollow.gameObject.GetComponentInParent<Camera>();
			this.cameraObject = new GameObject("WalkSim First Person Camera");
			this.overrideCam = this.cameraObject.AddComponent<Camera>();
			this.overrideCam.fieldOfView = 90f;
			this.overrideCam.nearClipPlane = componentInParent.nearClipPlane;
			this.overrideCam.farClipPlane = componentInParent.farClipPlane;
			this.overrideCam.targetDisplay = componentInParent.targetDisplay;
			this.overrideCam.cullingMask = componentInParent.cullingMask;
			this.overrideCam.depth = componentInParent.depth + 1f;
			this.overrideCam.targetDisplay = componentInParent.targetDisplay;
			this.overrideCam.enabled = false;
		}

		// Token: 0x06000049 RID: 73 RVA: 0x00003638 File Offset: 0x00001838
		private void LateUpdate() // i dont wanna hear it, these null checks are essential to being able to look around
		{
			if (cameraObject != null)
			{
				this.cameraObject.transform.position = GTPlayer.Instance.headCollider.transform.TransformPoint(this.offset);
				this.cameraObject.transform.forward = this.head.forward;
			}

			bool flag = this.turn;
			if (flag)
			{
				GTPlayer.Instance.Turn(Mouse.current.delta.value.x / 10f);
				Vector3 eulerAngles = GorillaTagger.Instance.offlineVRRig.headConstraint.eulerAngles;
				eulerAngles.x -= Mouse.current.delta.value.y / 10f;
				bool flag2 = eulerAngles.x > 180f;
				if (flag2)
				{
					eulerAngles.x -= 360f;
				}
				eulerAngles.x = Mathf.Clamp(eulerAngles.x, -60f, 60f);
				GorillaTagger.Instance.offlineVRRig.headConstraint.eulerAngles = eulerAngles;
				eulerAngles.y += 90f;
                if (thirpyTarget != null) this.thirpyTarget.localEulerAngles = new Vector3(eulerAngles.x, 0f, 0f);
				GTPlayer.Instance.headCollider.transform.localEulerAngles = new Vector3(eulerAngles.x, 0f, 0f);
			}
		}

		// Token: 0x0600004A RID: 74 RVA: 0x000037AC File Offset: 0x000019AC
		private void OverrideHeadMovement()
		{
			Logging.Debug(new object[]
			{
				"Overriding head movement"
			});
			this.head = GorillaTagger.Instance.offlineVRRig.head.rigTarget;
		}

		// Token: 0x0600004B RID: 75 RVA: 0x000037DD File Offset: 0x000019DD
		internal void ToggleCam()
		{
			this.overrideCam.enabled = !this.overrideCam.enabled;
		}

		// Token: 0x17000007 RID: 7
		// (get) Token: 0x0600004C RID: 76 RVA: 0x000037FC File Offset: 0x000019FC
		// (set) Token: 0x0600004D RID: 77 RVA: 0x00003819 File Offset: 0x00001A19
		public bool FirstPerson
		{
			get
			{
				return this.overrideCam.enabled;
			}
			set
			{
				this.overrideCam.enabled = value;
			}
		}

		// Token: 0x0600004E RID: 78 RVA: 0x0000382C File Offset: 0x00001A2C
		private void OnEnable()
		{
			Logging.Debug(new object[]
			{
				"Enabled"
			});
			bool flag = Rig.Instance.Animator == null;
			if (!flag)
			{
				this.LockCursor = true;
				this.OverrideHeadMovement();
			}
		}

		// Token: 0x0600004F RID: 79 RVA: 0x00003870 File Offset: 0x00001A70
		private void OnDisable()
		{
			bool flag = !this.head;
			if (!flag)
			{
				this.LockCursor = false;
				GorillaTagger.Instance.offlineVRRig.head.rigTarget = this.head;
			}
		}

		// Token: 0x04000034 RID: 52
		public static HeadDriver Instance;

		// Token: 0x04000035 RID: 53
		public Transform thirpyTarget;

		// Token: 0x04000036 RID: 54
		public Transform head;

		// Token: 0x04000037 RID: 55
		public GameObject cameraObject;

		// Token: 0x04000038 RID: 56
		public GameObject cameraTransform;

		// Token: 0x04000039 RID: 57
		private Vector3 offset = new Vector3(0f, 0f, 0f);

		// Token: 0x0400003A RID: 58
		private Camera overrideCam;

		// Token: 0x0400003B RID: 59
		public bool turn = true;

		// Token: 0x0400003C RID: 60
		private bool _lockCursor;
	}
}
