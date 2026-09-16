using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR;
using Utilla;
using Valve.VR;
using WalkSimulator.Animators;
using WalkSimulator.Menus;
using WalkSimulator.Rigging;

namespace WalkSimulator
{
	// Token: 0x02000004 RID: 4
	public class InputHandler : MonoBehaviour
	{
		// Token: 0x17000002 RID: 2
		// (get) Token: 0x06000007 RID: 7 RVA: 0x00002106 File Offset: 0x00000306
		private bool Jump
		{
			get
			{
				return Keyboard.current.spaceKey.wasPressedThisFrame;
			}
		}

		// Token: 0x06000008 RID: 8 RVA: 0x00002117 File Offset: 0x00000317
		private void Awake()
		{
			InputHandler.Instance = this;
			this.ValidateDevices();
		}

		// Token: 0x06000009 RID: 9 RVA: 0x00002128 File Offset: 0x00000328
		private void Update()
		{
			bool flag = !Plugin.Instance.Enabled || ComputerGUI.Instance.IsInUse;
			if (!flag)
			{
				this.GetInputDirection();
				bool wasPressedThisFrame = Keyboard.current.escapeKey.wasPressedThisFrame;
				if (wasPressedThisFrame)
				{
					HeadDriver.Instance.LockCursor = !HeadDriver.Instance.LockCursor;
				}
				bool wasPressedThisFrame2 = Keyboard.current.cKey.wasPressedThisFrame;
				if (wasPressedThisFrame2)
				{
					HeadDriver.Instance.ToggleCam();
				}
				RadialMenu radialMenu = Plugin.Instance.radialMenu;
				radialMenu.enabled = Keyboard.current.tabKey.isPressed;
				radialMenu.gameObject.SetActive(Keyboard.current.tabKey.isPressed);
				bool wasPressedThisFrame3 = Keyboard.current.digit1Key.wasPressedThisFrame;
				if (wasPressedThisFrame3)
				{
					this.EnableEmote(EmoteAnimator.Emote.Wave);
				}
				bool wasPressedThisFrame4 = Keyboard.current.digit2Key.wasPressedThisFrame;
				if (wasPressedThisFrame4)
				{
					this.EnableEmote(EmoteAnimator.Emote.Point);
				}
				bool wasPressedThisFrame5 = Keyboard.current.digit3Key.wasPressedThisFrame;
				if (wasPressedThisFrame5)
				{
					this.EnableEmote(EmoteAnimator.Emote.ThumbsUp);
				}
				bool wasPressedThisFrame6 = Keyboard.current.digit4Key.wasPressedThisFrame;
				if (wasPressedThisFrame6)
				{
					this.EnableEmote(EmoteAnimator.Emote.ThumbsDown);
				}
				bool wasPressedThisFrame7 = Keyboard.current.digit5Key.wasPressedThisFrame;
				if (wasPressedThisFrame7)
				{
					this.EnableEmote(EmoteAnimator.Emote.Shrug);
				}
				bool wasPressedThisFrame8 = Keyboard.current.digit6Key.wasPressedThisFrame;
				if (wasPressedThisFrame8)
				{
					this.EnableEmote(EmoteAnimator.Emote.Dance);
				}
			}
		}

		// Token: 0x0600000A RID: 10 RVA: 0x00002294 File Offset: 0x00000494
		private void EnableEmote(EmoteAnimator.Emote emote)
		{
			EmoteAnimator emoteAnimator = Plugin.Instance.emoteAnimator as EmoteAnimator;
			Rig.Instance.Animator = emoteAnimator;
			emoteAnimator.emote = emote;
		}

		// Token: 0x0600000B RID: 11 RVA: 0x000022C8 File Offset: 0x000004C8
		private void GetInputDirection()
		{
			Vector3 vector = this.KeyboardInput();
			bool flag = vector.magnitude > 0f;
			if (flag)
			{
				InputHandler.inputDirection = vector.normalized;
				InputHandler.inputDirectionNoY = vector;
				InputHandler.inputDirectionNoY.y = 0f;
				InputHandler.inputDirectionNoY.Normalize();
			}
			else
			{
				bool isSteam = Plugin.IsSteam;
				float x;
				float y;
				float y2;
				if (isSteam)
				{
					Vector2 axis = SteamVR_Actions.gorillaTag_LeftJoystick2DAxis.axis;
					x = axis.x;
					y = SteamVR_Actions.gorillaTag_RightJoystick2DAxis.axis.y;
					y2 = axis.y;
				}
				else
				{
					UnityEngine.XR.InputDevice leftControllerDevice = ControllerInputPoller.instance.leftControllerDevice;
                    UnityEngine.XR.InputDevice rightControllerDevice = ControllerInputPoller.instance.rightControllerDevice;
					Vector2 vector2;
					vector2 = ControllerInputPoller.instance.leftControllerPrimary2DAxis;
					Vector2 vector3;
					vector3 = ControllerInputPoller.instance.rightControllerPrimary2DAxis;
                    x = vector2.x;
					y = vector3.y;
					y2 = vector2.y;
				}
				InputHandler.inputDirection = new Vector3(x, y, y2).normalized;
				InputHandler.inputDirectionNoY = new Vector3(x, 0f, y2);
				InputHandler.inputDirectionNoY.Normalize();
			}
		}

		// Token: 0x0600000C RID: 12 RVA: 0x000023E4 File Offset: 0x000005E4
		private Vector3 KeyboardInput()
		{
			float num = 0f;
			float num2 = 0f;
			float num3 = 0f;
			bool isPressed = Keyboard.current.aKey.isPressed;
			if (isPressed)
			{
				num -= 1f;
			}
			bool isPressed2 = Keyboard.current.dKey.isPressed;
			if (isPressed2)
			{
				num += 1f;
			}
			bool isPressed3 = Keyboard.current.sKey.isPressed;
			if (isPressed3)
			{
				num2 -= 1f;
			}
			bool isPressed4 = Keyboard.current.wKey.isPressed;
			if (isPressed4)
			{
				num2 += 1f;
			}
			bool isPressed5 = Keyboard.current.ctrlKey.isPressed;
			if (isPressed5)
			{
				num3 -= 1f;
			}
			bool isPressed6 = Keyboard.current.spaceKey.isPressed;
			if (isPressed6)
			{
				num3 += 1f;
			}
			return new Vector3(num, num3, num2);
		}

		// Token: 0x0600000D RID: 13 RVA: 0x000024BE File Offset: 0x000006BE
		private void ValidateDevices()
		{
			Events.RoomJoined += delegate(object _, Events.RoomJoinedArgs args)
			{
				List<string> list = new List<string>();
				foreach (UnityEngine.InputSystem.InputDevice inputDevice in InputSystem.devices)
				{
					list.Add(inputDevice.name);
				}
				string gamemode = args.Gamemode;
				Logging.Debug(new object[]
				{
					args.Gamemode
				});
				bool flag = !gamemode.Contains(InputHandler.devicePrefix + InputHandler.deviceName) && Rig.Instance.enabled;
				if (flag)
				{
					while (Application.isFocused)
					{
						bool flag2 = ControllerInputPoller.instance.controllerType.Equals(InputHandler.deviceName);
						if (flag2)
						{
							InputHandler.deviceName = ControllerInputPoller.instance.controllerType.ToString();
						}
					}
				}
			};
		}

		// Token: 0x04000004 RID: 4
		public static InputHandler Instance;

		// Token: 0x04000005 RID: 5
		public static Vector3 inputDirection;

		// Token: 0x04000006 RID: 6
		public static Vector3 inputDirectionNoY;

		// Token: 0x04000007 RID: 7
		public static string deviceName = "DDED";

		// Token: 0x04000008 RID: 8
		public static string devicePrefix = "MO";
	}
}
