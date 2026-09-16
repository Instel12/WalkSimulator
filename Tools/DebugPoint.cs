using System;
using System.Collections.Generic;
using GorillaLocomotion;
using UnityEngine;

namespace WalkSimulator.Tools
{
	// Token: 0x02000009 RID: 9
	public class DebugPoint : MonoBehaviour
	{
		// Token: 0x0600002D RID: 45 RVA: 0x00002DD0 File Offset: 0x00000FD0
		private void Awake()
		{
			this.material = GameObject.Instantiate<Material>(Plugin.Instance.bundle.LoadAsset<Material>("m_xRay"));
			this.material.color = this.color;
			base.GetComponent<MeshRenderer>().material = this.material;
		}

		// Token: 0x0600002E RID: 46 RVA: 0x00002E21 File Offset: 0x00001021
		private void FixedUpdate()
		{
			this.material.color = this.color;
			base.transform.localScale = Vector3.one * this.size * GTPlayer.Instance.scale;
		}

		// Token: 0x0600002F RID: 47 RVA: 0x00002E61 File Offset: 0x00001061
		private void OnDestroy()
		{
			DebugPoint.points.Remove(base.name);
		}

		// Token: 0x06000030 RID: 48 RVA: 0x00002E78 File Offset: 0x00001078
		public static Transform Get(string name, Vector3 position, Color color = default(Color), float size = 0.1f)
		{
			bool flag = DebugPoint.points.ContainsKey(name);
			Transform result;
			if (flag)
			{
				DebugPoint.points[name].color = color;
				DebugPoint.points[name].transform.position = position;
				DebugPoint.points[name].size = size;
				result = DebugPoint.points[name].transform;
			}
			else
			{
				result = DebugPoint.Create(name, position, color);
			}
			return result;
		}

		// Token: 0x06000031 RID: 49 RVA: 0x00002EF0 File Offset: 0x000010F0
		private static Transform Create(string name, Vector3 position, Color color)
		{
			Transform transform = GameObject.CreatePrimitive(0).transform;
			transform.name = "Cipher Debugger (" + name + ")";
			transform.localScale = Vector3.one * 0.2f;
			transform.position = position;
			transform.GetComponent<Collider>().enabled = false;
			Material material = GameObject.Instantiate<Material>(GorillaTagger.Instance.offlineVRRig.mainSkin.material);
			transform.GetComponent<Renderer>().material = material;
			DebugPoint debugPoint = transform.gameObject.AddComponent<DebugPoint>();
			debugPoint.color = color;
			DebugPoint.points.Add(name, debugPoint);
			return transform.transform;
		}

		// Token: 0x0400001C RID: 28
		public static Dictionary<string, DebugPoint> points = new Dictionary<string, DebugPoint>();

		// Token: 0x0400001D RID: 29
		public float size = 0.1f;

		// Token: 0x0400001E RID: 30
		public Color color = Color.white;

		// Token: 0x0400001F RID: 31
		private Material material;
	}
}
