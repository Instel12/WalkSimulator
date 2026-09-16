using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using WalkSimulator.Animators;
using WalkSimulator.Rigging;

namespace WalkSimulator.Menus
{
	// Token: 0x02000010 RID: 16
	public class RadialMenu : MonoBehaviour
	{
		// Token: 0x06000068 RID: 104 RVA: 0x00004798 File Offset: 0x00002998
		private void Awake()
		{
			Image component = base.transform.Find("Icons/Walk").GetComponent<Image>();
			Image component2 = base.transform.Find("Icons/Pose").GetComponent<Image>();
			Image component3 = base.transform.Find("Icons/Interact").GetComponent<Image>();
			Image component4 = base.transform.Find("Icons/Fly").GetComponent<Image>();
			List<RadialMenu.Icon> list = new List<RadialMenu.Icon>();
			list.Add(new RadialMenu.Icon
			{
				image = component,
				direction = Vector2.up,
				animator = Plugin.Instance.walkAnimator
			});
			list.Add(new RadialMenu.Icon
			{
				image = component3,
				direction = Vector2.left,
				animator = Plugin.Instance.grabAnimator
			});
			list.Add(new RadialMenu.Icon
			{
				image = component2,
				direction = Vector2.down,
				animator = Plugin.Instance.handAnimator
			});
			list.Add(new RadialMenu.Icon
			{
				image = component4,
				direction = Vector2.right,
				animator = Plugin.Instance.flyAnimator
			});
			this.icons = list;
		}

        // Token: 0x06000069 RID: 105 RVA: 0x000048E4 File Offset: 0x00002AE4
        private void Update()
        {
            Vector2 vector = Mouse.current.position.ReadValue();
            Vector2 vector2 = new Vector2(Screen.width / 2f, Screen.height / 2f);
            bool flag = (vector - vector2).magnitude < (float)Screen.width / 20f;
			if (!flag)
			{
				RadialMenu.Icon icon = default(RadialMenu.Icon);
				float num = float.PositiveInfinity;
				foreach (RadialMenu.Icon icon2 in this.icons)
				{
					float num2 = Vector2.Distance(vector - vector2, icon2.direction);
					bool flag2 = num2 < num;
					if (flag2)
					{
						icon = icon2;
						num = num2;
					}
				}
				this.selectedAnimator = icon.animator;
				foreach (RadialMenu.Icon icon3 in this.icons)
				{
					icon3.image.color = (icon3.Equals(icon) ? Color.white : Color.gray);
					icon3.image.transform.localScale = Vector3.one * (icon3.Equals(icon) ? 1.5f : 1f);
				}
			}
		}

		// Token: 0x0600006A RID: 106 RVA: 0x00004A7C File Offset: 0x00002C7C
		private void OnEnable()
		{
			this.cursorWasLocked = HeadDriver.Instance.LockCursor;
			this.wasTurning = HeadDriver.Instance.turn;
			HeadDriver.Instance.LockCursor = false;
			HeadDriver.Instance.turn = false;
		}

		// Token: 0x0600006B RID: 107 RVA: 0x00004AB8 File Offset: 0x00002CB8
		private void OnDisable()
		{
			Logging.Debug(new object[]
			{
				"RadialMenu disabled"
			});
			HeadDriver.Instance.LockCursor = this.cursorWasLocked;
			HeadDriver.Instance.turn = this.wasTurning;
			Rig.Instance.Animator = this.selectedAnimator;
			Logging.Debug(new object[]
			{
				"--Finished"
			});
		}

		// Token: 0x04000061 RID: 97
		public List<RadialMenu.Icon> icons;

		// Token: 0x04000062 RID: 98
		private AnimatorBase selectedAnimator;

		// Token: 0x04000063 RID: 99
		private bool cursorWasLocked;

		// Token: 0x04000064 RID: 100
		private bool wasTurning;

		// Token: 0x0200001A RID: 26
		public struct Icon
		{
			// Token: 0x04000090 RID: 144
			public Image image;

			// Token: 0x04000091 RID: 145
			public Vector2 direction;

			// Token: 0x04000092 RID: 146
			public AnimatorBase animator;
		}
	}
}
