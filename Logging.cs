using System;
using System.Diagnostics;
using System.Reflection;
using BepInEx.Logging;

namespace WalkSimulator
{
	// Token: 0x02000007 RID: 7
	public static class Logging
	{
		// Token: 0x06000021 RID: 33 RVA: 0x00002A5B File Offset: 0x00000C5B
		public static void Init()
		{
			Logging.logger = Logger.CreateLogSource("WalkSimulator");
		}

		// Token: 0x06000022 RID: 34 RVA: 0x00002A70 File Offset: 0x00000C70
		public static void Exception(Exception e)
		{
			MethodBase method = new StackTrace().GetFrame(1).GetMethod();
			Logging.logger.LogWarning(string.Concat(new string[]
			{
				"(",
				method.ReflectedType.Name,
				".",
				method.Name,
				"()) ",
				string.Join(" ", new string[]
				{
					e.Message,
					e.StackTrace
				})
			}));
		}

		// Token: 0x06000023 RID: 35 RVA: 0x00002AFC File Offset: 0x00000CFC
		public static void Fatal(params object[] content)
		{
			MethodBase method = new StackTrace().GetFrame(1).GetMethod();
			Logging.logger.LogFatal(string.Concat(new string[]
			{
				"(",
				method.ReflectedType.Name,
				".",
				method.Name,
				"()) ",
				string.Join(" ", content)
			}));
		}

		// Token: 0x06000024 RID: 36 RVA: 0x00002B70 File Offset: 0x00000D70
		public static void Warning(params object[] content)
		{
			MethodBase method = new StackTrace().GetFrame(1).GetMethod();
			Logging.logger.LogWarning(string.Concat(new string[]
			{
				"(",
				method.ReflectedType.Name,
				".",
				method.Name,
				"()) ",
				string.Join(" ", content)
			}));
		}

		// Token: 0x06000025 RID: 37 RVA: 0x00002BE4 File Offset: 0x00000DE4
		public static void Info(params object[] content)
		{
			MethodBase method = new StackTrace().GetFrame(1).GetMethod();
			Logging.logger.LogInfo(string.Concat(new string[]
			{
				"(",
				method.ReflectedType.Name,
				".",
				method.Name,
				"()) ",
				string.Join(" ", content)
			}));
		}

		// Token: 0x06000026 RID: 38 RVA: 0x00002C58 File Offset: 0x00000E58
		public static void Debug(params object[] content)
		{
			MethodBase method = new StackTrace().GetFrame(1).GetMethod();
			Logging.logger.LogDebug(string.Concat(new string[]
			{
				"(",
				method.ReflectedType.Name,
				".",
				method.Name,
				"()) ",
				string.Join("  ", content)
			}));
		}

		// Token: 0x06000027 RID: 39 RVA: 0x00002CCA File Offset: 0x00000ECA
		public static void Debugger(params object[] content)
		{
			Logging.Debug(content);
		}

		// Token: 0x06000028 RID: 40 RVA: 0x00002CD4 File Offset: 0x00000ED4
		public static string PrependTextToLog(string log, string text)
		{
			log = text + "\n" + log;
			string[] array = log.Split('\n');
			bool flag = array.Length > Logging.DebuggerLines;
			if (flag)
			{
				log = string.Join("\n", array, 0, Logging.DebuggerLines);
			}
			return log;
		}

		// Token: 0x0400001A RID: 26
		private static ManualLogSource logger;

		// Token: 0x0400001B RID: 27
		public static int DebuggerLines = 20;
	}
}
