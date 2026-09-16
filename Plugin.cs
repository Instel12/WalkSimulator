using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using BepInEx;
using GorillaNetworking;
using HarmonyLib;
using UnityEngine;
using Utilla;
using WalkSimulator.Animators;
using WalkSimulator.Menus;
using WalkSimulator.Rigging;
using WalkSimulator.Tools;
using Utilla.Attributes;
using Utilla.Behaviours;

namespace WalkSimulator
{
	// Token: 0x02000005 RID: 5
	[ModdedGamemode]
	[BepInDependency("org.legoandmars.gorillatag.utilla", "1.5.0")]
	[BepInPlugin(PluginInfo.GUID, PluginInfo.Name, PluginInfo.Version)]
	public class Plugin : BaseUnityPlugin
	{
		// Token: 0x17000003 RID: 3
		// (get) Token: 0x06000010 RID: 16 RVA: 0x00002505 File Offset: 0x00000705
		// (set) Token: 0x06000011 RID: 17 RVA: 0x0000250C File Offset: 0x0000070C
		public static bool IsSteam { get; protected set; }

		// Token: 0x17000004 RID: 4
		// (get) Token: 0x06000012 RID: 18 RVA: 0x00002514 File Offset: 0x00000714
		// (set) Token: 0x06000013 RID: 19 RVA: 0x0000252C File Offset: 0x0000072C
		public bool Enabled
		{
			get
			{
				return Plugin._enabled;
			}
			protected set
			{
				Plugin._enabled = value;
				if (value)
				{
					base.gameObject.GetOrAddComponent<InputHandler>();
					base.gameObject.GetOrAddComponent<Rig>();
					this.walkAnimator = base.gameObject.GetOrAddComponent<WalkAnimator>();
					this.flyAnimator = base.gameObject.GetOrAddComponent<FlyAnimator>();
					this.emoteAnimator = base.gameObject.GetOrAddComponent<EmoteAnimator>();
					this.handAnimator = base.gameObject.GetOrAddComponent<PoseAnimator>();
					this.grabAnimator = base.gameObject.GetOrAddComponent<InteractAnimator>();
					bool flag = !this.radialMenu;
					if (flag)
					{
						this.radialMenu = GameObject.Instantiate<GameObject>(this.bundle.LoadAsset<GameObject>("Radial Menu")).AddComponent<RadialMenu>();
					}
					this.computerGUI = base.gameObject.GetOrAddComponent<ComputerGUI>();
					this.walkAnimator.enabled = false;
					this.flyAnimator.enabled = false;
					this.emoteAnimator.enabled = false;
					this.handAnimator.enabled = false;
					this.grabAnimator.enabled = false;
				}
				else
				{
					InputHandler instance = InputHandler.Instance;
					if (instance != null)
					{
						instance.Obliterate();
					}
					Rig instance2 = Rig.Instance;
					if (instance2 != null)
					{
						instance2.Obliterate();
					}
					AnimatorBase animatorBase = this.walkAnimator;
					if (animatorBase != null)
					{
						animatorBase.Obliterate();
					}
					AnimatorBase animatorBase2 = this.flyAnimator;
					if (animatorBase2 != null)
					{
						animatorBase2.Obliterate();
					}
					AnimatorBase animatorBase3 = this.emoteAnimator;
					if (animatorBase3 != null)
					{
						animatorBase3.Obliterate();
					}
					AnimatorBase animatorBase4 = this.handAnimator;
					if (animatorBase4 != null)
					{
						animatorBase4.Obliterate();
					}
					AnimatorBase animatorBase5 = this.grabAnimator;
					if (animatorBase5 != null)
					{
						animatorBase5.Obliterate();
					}
					ComputerGUI computerGUI = this.computerGUI;
					if (computerGUI != null)
					{
						computerGUI.Obliterate();
					}
				}
			}
		}

		// Token: 0x06000014 RID: 20 RVA: 0x000026D4 File Offset: 0x000008D4
		private void Awake()
		{
			Plugin.Instance = this;
			Logging.Init();
			try
			{
				string text = Paths.ConfigPath + "/BepInEx.cfg";
				string text2 = File.ReadAllText(text);
				text2 = Regex.Replace(text2, "HideManagerGameObject = .+", "HideManagerGameObject = true");
				File.WriteAllText(text, text2);
			}
			catch (Exception e)
			{
				Logging.Exception(e);
			}
			this.bundle = Tools.AssetUtils.LoadAssetBundle("WalkSimulator/assetbundle");
		}

		// Token: 0x06000015 RID: 21 RVA: 0x00002750 File Offset: 0x00000950
		private void Start()
		{
            Events.GameInitialized += new EventHandler(this.OnGameInitialized);
			Events.RoomJoined += new EventHandler<Events.RoomJoinedArgs>(this.OnRoomJoined);
			Events.RoomLeft += new EventHandler<Events.RoomJoinedArgs>(this.OnRoomLeft);
		}

		// Token: 0x06000016 RID: 22 RVA: 0x00002789 File Offset: 0x00000989
		private void OnRoomLeft(object sender, Events.RoomJoinedArgs e)
		{
			this.Enabled = true;
		}

		// Token: 0x06000017 RID: 23 RVA: 0x00002794 File Offset: 0x00000994
		private void OnRoomJoined(object sender, Events.RoomJoinedArgs args)
		{
			string gamemode = args.Gamemode;
			this.Enabled = gamemode.ToUpper().Contains("MODDED");
		}

		// Token: 0x06000018 RID: 24 RVA: 0x000027C0 File Offset: 0x000009C0
		private void OnEnable()
		{
			HarmonyPatches.ApplyHarmonyPatches();
		}

		// Token: 0x06000019 RID: 25 RVA: 0x000027C9 File Offset: 0x000009C9
		private void OnDisable()
		{
			HarmonyPatches.RemoveHarmonyPatches();
		}

		// Token: 0x0600001A RID: 26 RVA: 0x000027D4 File Offset: 0x000009D4
		private void OnGameInitialized(object sender, EventArgs e)
		{
			//string text = (string)Traverse.Create(PlayFabAuthenticator.instance).Field("platform").GetValue();
			//Plugin.IsSteam = text.ToLower().Contains("steam");
			IsSteam = PlayFabAuthenticator.instance.platform.ToString().ToLower().Contains("steam");
			this.Enabled = true;
		}

		// Token: 0x0600001B RID: 27 RVA: 0x00002821 File Offset: 0x00000A21
		private void FixedUpdate()
		{
			this.TryInitializeGamemode();
		}

		// Token: 0x0600001C RID: 28 RVA: 0x0000282C File Offset: 0x00000A2C
		public void TryInitializeGamemode()
		{
			bool flag = this.setGamemode;
			if (!flag)
			{
				try
				{
					//GameObject.FindObjectOfType<UtillaGamemodeSelector>().NextPage();
					foreach (ModeSelectButton modeSelectButton in GameObject.FindObjectsOfType<ModeSelectButton>())
					{
						bool flag2 = modeSelectButton.gameMode.Contains("MODDED_CASUAL");
						if (flag2)
						{
							modeSelectButton.ButtonActivationWithHand(false);
						}
					}
					this.setGamemode = true;
				}
				catch
				{
				}
			}
		}

		// Token: 0x0600001D RID: 29 RVA: 0x000028B0 File Offset: 0x00000AB0
		private void OnGUI()
		{
			float num = 400f;
			float num2 = 40f;
			float num3 = (float)Screen.width - num - 10f;
			float num4 = 10f;
			GUI.skin.label.fontSize = 20;
			foreach (string text in new List<string>(this.labels.Keys))
			{
				num4 += num2;
				GUI.Label(new Rect(num3, num4, num, num2), text + ": " + this.labels[text]);
			}
			foreach (string text2 in new List<string>(this.sliders.Keys))
			{
				num4 += num2;
				this.sliders[text2] = GUI.HorizontalSlider(new Rect(num3, num4, num, num2), this.sliders[text2], 0f, 10f);
				GUI.Label(new Rect(num3 - num, num4, num, num2), text2 + ": " + this.sliders[text2].ToString());
			}
		}

		// Token: 0x04000009 RID: 9
		public static Plugin Instance;

		// Token: 0x0400000B RID: 11
		private static bool _enabled = true;

		// Token: 0x0400000C RID: 12
		public AnimatorBase walkAnimator;

		// Token: 0x0400000D RID: 13
		public AnimatorBase flyAnimator;

		// Token: 0x0400000E RID: 14
		public AnimatorBase emoteAnimator;

		// Token: 0x0400000F RID: 15
		public AnimatorBase handAnimator;

		// Token: 0x04000010 RID: 16
		public AnimatorBase grabAnimator;

		// Token: 0x04000011 RID: 17
		public ComputerGUI computerGUI;

		// Token: 0x04000012 RID: 18
		public AssetBundle bundle;

		// Token: 0x04000013 RID: 19
		public RadialMenu radialMenu;

		// Token: 0x04000014 RID: 20
		private bool setGamemode = false;

		// Token: 0x04000015 RID: 21
		public Dictionary<string, float> sliders = new Dictionary<string, float>();

		// Token: 0x04000016 RID: 22
		public Dictionary<string, string> labels = new Dictionary<string, string>();
	}
}
