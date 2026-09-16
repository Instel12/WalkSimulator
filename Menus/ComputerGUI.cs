using System;
using System.Collections.Generic;
using Cinemachine;
using GorillaLocomotion;
using GorillaNetworking;
using HarmonyLib;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.UI;
using WalkSimulator.Animators;
using WalkSimulator.Rigging;
using WalkSimulator.Tools;
using WalkSimulatorClassic;

namespace WalkSimulator.Menus
{
	// Token: 0x0200000F RID: 15
	public class ComputerGUI : MonoBehaviour
	{
		// Token: 0x1700000B RID: 11
		// (get) Token: 0x0600005F RID: 95 RVA: 0x00003FD7 File Offset: 0x000021D7
		public bool IsInUse
		{
			get
			{
				return false;
				return this.overrideCam.enabled;
			}
		}

		// Token: 0x06000060 RID: 96 RVA: 0x00003FE4 File Offset: 0x000021E4
		private bool IsInRange()
		{
			Vector3 position = GTPlayer.Instance.bodyCollider.transform.position;
			bool flag = position.Distance(GorillaComputer.instance.transform.position) < 2f;
			bool result;
			if (flag)
			{
				this.currentTerminal = GorillaComputer.instance.transform;
				result = true;
			}
			else
			{
				foreach (GorillaComputerTerminal gorillaComputerTerminal in this.terminals)
				{
					bool flag2 = position.Distance(gorillaComputerTerminal.transform.position) < 2f;
					if (flag2)
					{
						this.currentTerminal = gorillaComputerTerminal.transform;
						return true;
					}
				}
				this.currentTerminal = null;
				result = false;
			}
			return result;
		}

		// Token: 0x06000061 RID: 97 RVA: 0x000040A0 File Offset: 0x000022A0
		private void Awake()
		{
			ComputerGUI.Instance = this;
			GameObject gameObject = new GameObject("WalkSim First Person Camera");
			Cinemachine3rdPersonFollow cinemachine3rdPersonFollow = GameObject.FindObjectOfType<Cinemachine3rdPersonFollow>();
			Camera componentInParent = cinemachine3rdPersonFollow.gameObject.GetComponentInParent<Camera>();
			this.overrideCam = gameObject.AddComponent<Camera>();
			this.overrideCam.fieldOfView = 90f;
			this.overrideCam.nearClipPlane = componentInParent.nearClipPlane;
			this.overrideCam.farClipPlane = componentInParent.farClipPlane;
			this.overrideCam.targetDisplay = componentInParent.targetDisplay;
			this.overrideCam.cullingMask = componentInParent.cullingMask;
			this.overrideCam.depth = componentInParent.depth + 1f;
			this.overrideCam.targetDisplay = componentInParent.targetDisplay;
			this.overrideCam.enabled = false;
		}

		// Token: 0x06000062 RID: 98 RVA: 0x0000416D File Offset: 0x0000236D
		private void Start()
		{
			this.BuildButtonMap();
		}

		// Token: 0x06000063 RID: 99 RVA: 0x00004178 File Offset: 0x00002378
		private void Update()
		{
			bool flag = Keyboard.current.eKey.wasPressedThisFrame && this.inRange && !this.overrideCam.enabled;
			if (flag)
			{
				this.overrideCam.enabled = true;
				this.cachedAnimator = Rig.Instance.Animator;
				Rig.Instance.Animator = null;
			}
			else
			{
				bool enabled = this.overrideCam.enabled;
				if (enabled)
				{
					bool wasPressedThisFrame = Keyboard.current.escapeKey.wasPressedThisFrame;
					if (wasPressedThisFrame)
					{
						this.overrideCam.enabled = false;
						Rig.Instance.Animator = this.cachedAnimator;
					}
					else
					{
						foreach (KeyControl keyControl in this.buttonMapping.Keys)
						{
							try
							{
								bool flag2 = keyControl == null;
								if (flag2)
								{
									Logging.Debug(new object[]
									{
										"Key is null"
									});
								}
								bool wasPressedThisFrame2 = keyControl.wasPressedThisFrame;
								if (wasPressedThisFrame2)
								{
									Logging.Debug(new object[]
									{
										"Pressed",
										(keyControl != null) ? keyControl.name : null
									});
									//GorillaComputer.instance.PressButton(this.buttonMapping[keyControl]);
									Sounds.Play(66, 0.5f);
								}
							}
							catch (Exception e)
							{
								Logging.Exception(e);
							}
						}
					}
				}
			}
		}

		// Token: 0x06000064 RID: 100 RVA: 0x00004318 File Offset: 0x00002518
		private void FixedUpdate()
		{
			bool flag = Time.frameCount % 60 == 0;
			if (flag)
			{
				this.inRange = this.IsInRange();
			}
			bool flag2 = !this.inRange && Time.frameCount % 600 == 0;
			if (flag2)
			{
				this.terminals = GameObject.FindObjectsOfType<GorillaComputerTerminal>();
			}
			bool flag3 = this.inRange;
			if (flag3)
			{
				GorillaComputerTerminal component = this.currentTerminal.GetComponent<GorillaComputerTerminal>();
				bool flag4 = component != null;
				Transform transform;
				if (flag4)
				{
					transform = component.myScreenText.transform;
				}
				else
				{
					transform = Traverse.Create(GorillaComputer.instance.screenText).Field("text").GetValue<Text>().transform;
				}
				this.overrideCam.transform.position = transform.TransformPoint(Vector3.forward * -0.3f);
				this.overrideCam.transform.LookAt(transform);
			}
			else
			{
				this.overrideCam.enabled = false;
			}
		}

		// Token: 0x06000065 RID: 101 RVA: 0x00004410 File Offset: 0x00002610
		private void OnGUI()
		{
			string text = "";
			bool flag = this.inRange;
			if (flag)
			{
				text = (this.overrideCam.enabled ? "Press [Escape] to exit" : "Press [E] to use computer");
			}
			GUI.Label(new Rect(20f, 20f, 200f, 200f), text, new GUIStyle
			{
				fontSize = 20,
				normal = new GUIStyleState
				{
					textColor = Color.white
				}
			});
		}

		// Token: 0x06000066 RID: 102 RVA: 0x00004494 File Offset: 0x00002694
		private void BuildButtonMap()
		{
			foreach (GorillaKeyboardButton gorillaKeyboardButton in GameObject.FindObjectsOfType<GorillaKeyboardButton>())
			{
				Key key = this.keyMapping[gorillaKeyboardButton.characterString];
				this.buttonMapping.Add(Keyboard.current[key], gorillaKeyboardButton);
				Logging.Debug(new object[]
				{
					gorillaKeyboardButton.gameObject.layer
				});
			}
		}

		// Token: 0x06000067 RID: 103 RVA: 0x00004508 File Offset: 0x00002708
		public ComputerGUI()
		{
			Dictionary<string, Key> dictionary = new Dictionary<string, Key>();
			//dictionary.Add("A", 15);
			//dictionary.Add("B", 16);
			//dictionary.Add("C", 17);
			//dictionary.Add("D", 18);
			//dictionary.Add("E", 19);
			//dictionary.Add("F", 20);
			//dictionary.Add("G", 21);
			//dictionary.Add("H", 22);
			//dictionary.Add("I", 23);
			//dictionary.Add("J", 24);
			//dictionary.Add("K", 25);
			//dictionary.Add("L", 26);
			//dictionary.Add("M", 27);
			//dictionary.Add("N", 28);
			//dictionary.Add("O", 29);
			//dictionary.Add("P", 30);
			//dictionary.Add("Q", 31);
			//dictionary.Add("R", 32);
			//dictionary.Add("S", 33);
			//dictionary.Add("T", 34);
			//dictionary.Add("U", 35);
			//dictionary.Add("V", 36);
			//dictionary.Add("W", 37);
			//dictionary.Add("X", 38);
			//dictionary.Add("Y", 39);
			//dictionary.Add("Z", 40);
			//dictionary.Add("0", 50);
			//dictionary.Add("1", 41);
			//dictionary.Add("2", 42);
			//dictionary.Add("3", 43);
			//dictionary.Add("4", 44);
			//dictionary.Add("5", 45);
			//dictionary.Add("6", 46);
			//dictionary.Add("7", 47);
			//dictionary.Add("8", 48);
			//dictionary.Add("9", 49);
			//dictionary.Add("option1", 94);
			//dictionary.Add("option2", 95);
			//dictionary.Add("option3", 96);
			//dictionary.Add("enter", 2);
			//dictionary.Add("delete", 65);
			//dictionary.Add("up", 63);
			//dictionary.Add("down", 64);
			//this.keyMapping = dictionary;
			//base..ctor();
		}

		// Token: 0x04000059 RID: 89
		public static ComputerGUI Instance;

		// Token: 0x0400005A RID: 90
		private GorillaComputerTerminal[] terminals = new GorillaComputerTerminal[0];

		// Token: 0x0400005B RID: 91
		private AnimatorBase cachedAnimator;

		// Token: 0x0400005C RID: 92
		private Transform currentTerminal;

		// Token: 0x0400005D RID: 93
		private Camera overrideCam;

		// Token: 0x0400005E RID: 94
		private bool inRange;

		// Token: 0x0400005F RID: 95
		private Dictionary<KeyControl, GorillaKeyboardButton> buttonMapping = new Dictionary<KeyControl, GorillaKeyboardButton>();

		// Token: 0x04000060 RID: 96
		private Dictionary<string, Key> keyMapping;
	}
}
