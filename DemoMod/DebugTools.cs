﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Il2CppSystem.IO;
using MelonLoader;
using PrimitierModdingFramework;
using PrimitierModdingFramework.Debugging;
using PrimitierModdingFramework.Debugging.ComponentDumpers;
using UnityEngine;

namespace DemoMod
{
	public static class DebugTools
	{
		private static InGameDebugTool _InGameDebugTool;

		public static void Update(MelonLogger.Instance logger)
		{
			if (Input.GetKeyDown(KeyCode.Space))
			{
				logger.Msg("Starting Dump");
				HierarchyXmlDumper.DumpCurrentScene(new ComponentDumperList(new TransformDumper(), new RectTransformDumper()));
				logger.Msg($"Dump complete saved at '{Path.Combine(Environment.CurrentDirectory, HierarchyXmlDumper.FilePath)}'");

			}

			if (Input.GetKeyUp(KeyCode.A))
			{
				if (_InGameDebugTool != null)
				{
					GameObject.Destroy(_InGameDebugTool.gameObject);
				}

				_InGameDebugTool = InGameDebugTool.Spawn(PlayerHelper.RHand.position);

			}

			if (PlayerHelper.RHand != null && _InGameDebugTool == null)
			{
				_InGameDebugTool = InGameDebugTool.Spawn(PlayerHelper.RHand.position);
			}

		}

	}
}
