using System;
using System.Collections;
using GorillaNetworking;
using UnityEngine;
using UnityEngine.InputSystem;
using WalkSimulator.Rigging;
using WalkSimulator.Tools;

namespace WalkSimulator.Animators
{
	// Token: 0x02000014 RID: 20
	public class InteractAnimator : AnimatorBase
	{
		// Token: 0x06000084 RID: 132 RVA: 0x000058FF File Offset: 0x00003AFF
		public override void Animate()
		{
			this.reticle.gameObject.SetActive(true);
			HeadDriver.Instance.FirstPerson = true;
			this.AnimateBody();
			this.AnimateHands();
		}

		// Token: 0x06000085 RID: 133 RVA: 0x00005930 File Offset: 0x00003B30
		private void Update()
		{
			bool flag = this.state > InteractAnimator.State.IDLE;
			if (!flag)
			{
				bool isPressed = Mouse.current.leftButton.isPressed;
				if (isPressed)
				{
					this.state = InteractAnimator.State.WAIT;
					base.StartCoroutine(this.Raycast(this.leftHand));
				}
				else
				{
					bool isPressed2 = Mouse.current.rightButton.isPressed;
					if (isPressed2)
					{
						this.state = InteractAnimator.State.WAIT;
						base.StartCoroutine(this.Raycast(this.rightHand));
					}
				}
			}
		}

		// Token: 0x06000086 RID: 134 RVA: 0x000059AD File Offset: 0x00003BAD
		private void AnimateBody()
		{
			this.rig.active = true;
			this.rig.useGravity = false;
			this.rig.targetPosition = this.body.position;
		}

		// Token: 0x06000087 RID: 135 RVA: 0x000059DE File Offset: 0x00003BDE
		private IEnumerator Raycast(HandDriver main)
		{
			Ray ray = new Ray(Camera.main.transform.position, this.reticle.position - Camera.main.transform.position);
			int buttonLayer = LayerMask.GetMask(new string[]
			{
				"GorillaInteractable",
				"TransparentFX"
			});
			RaycastHit[] hits = Physics.RaycastAll(ray, 0.82f, buttonLayer);
			foreach (RaycastHit hit in hits)
			{
				bool flag = hit.transform.GetComponent<GorillaPressableButton>() || hit.transform.GetComponent<GorillaKeyboardButton>() || hit.transform.GetComponent<GorillaPlayerLineButton>();
				if (flag)
				{
					yield return this.PressButton(main, hit.point - Camera.main.transform.forward * 0.05f);
				}
				//hit = default(RaycastHit);
			}
			RaycastHit[] array = null;
			this.state = InteractAnimator.State.IDLE;
			yield break;
		}

		// Token: 0x06000088 RID: 136 RVA: 0x000059F4 File Offset: 0x00003BF4
		private IEnumerator PressButton(HandDriver hand, Vector3 targetPosition)
		{
			this.state = InteractAnimator.State.BUTTON;
			hand.grip = true;
			hand.targetPosition = this.reticle.position;
			hand.lookAt = targetPosition;
			hand.up = (hand.isLeft ? this.head.right : (-this.head.right));
			yield return new WaitForSeconds(0.1f);
			hand.targetPosition = targetPosition;
			while (hand.transform.position.Distance(targetPosition) > 0.05f)
			{
				yield return new WaitForFixedUpdate();
			}
			hand.targetPosition = this.reticle.position + Camera.main.transform.forward * 0.05f;
			yield return new WaitForSeconds(0.1f);
			hand.targetPosition = hand.DefaultPosition;
			this.state = InteractAnimator.State.IDLE;
			yield break;
		}

		// Token: 0x06000089 RID: 137 RVA: 0x00005A14 File Offset: 0x00003C14
		private void AnimateHands()
		{
			bool flag = this.state == InteractAnimator.State.IDLE;
			if (flag)
			{
				this.leftHand.targetPosition = this.leftHand.DefaultPosition;
				this.rightHand.targetPosition = this.rightHand.DefaultPosition;
				this.leftHand.lookAt = this.leftHand.targetPosition + this.head.forward;
				this.rightHand.lookAt = this.rightHand.targetPosition + this.head.forward;
				this.leftHand.up = this.head.right;
				this.rightHand.up = -this.head.right;
			}
		}

		// Token: 0x0600008A RID: 138 RVA: 0x00005AE0 File Offset: 0x00003CE0
		protected override void Start()
		{
			base.Start();
			this.reticle = GameObject.CreatePrimitive(0).transform;
			this.reticle.localScale = Vector3.one * 0.001f;
			this.reticle.GetComponent<MeshRenderer>().material.color = Color.white;
			this.reticle.GetComponent<MeshRenderer>().material.shader = Shader.Find("GorillaTag/UberShader");
			this.reticle.GetComponent<SphereCollider>().enabled = false;
			this.reticle.transform.SetParent(Camera.main.transform);
			this.reticle.transform.localPosition = Vector3.forward * 0.1f;
			this.reticle.gameObject.SetActive(false);
		}

		// Token: 0x0600008B RID: 139 RVA: 0x00005BBB File Offset: 0x00003DBB
		public override void Setup()
		{
			HeadDriver.Instance.LockCursor = true;
		}

		// Token: 0x0600008C RID: 140 RVA: 0x00005BCA File Offset: 0x00003DCA
		public override void Cleanup()
		{
			base.Cleanup();
			this.reticle.gameObject.SetActive(false);
			this.state = InteractAnimator.State.IDLE;
			base.StopAllCoroutines();
		}

		// Token: 0x0600008D RID: 141 RVA: 0x00005BF4 File Offset: 0x00003DF4
		private void OnDestory()
		{
			GameObject.Destroy(this.reticle.gameObject);
		}

		// Token: 0x04000075 RID: 117
		private Transform reticle;

		// Token: 0x04000076 RID: 118
		private InteractAnimator.State state;

		// Token: 0x0200001D RID: 29
		public enum State
		{
			// Token: 0x040000A2 RID: 162
			IDLE,
			// Token: 0x040000A3 RID: 163
			WAIT,
			// Token: 0x040000A4 RID: 164
			BUTTON,
			// Token: 0x040000A5 RID: 165
			GRAB
		}
	}
}
