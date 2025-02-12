﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cocona;

namespace PMFTool.Commands
{
	public class UpdateDllsCommand
	{

		[PrimaryCommand()]
		public void UpdateDlls(
			[Argument(Description ="The path to project directory of the mod you want to update the dlls for")]
			string path="")
		{



			var projectPath = Validator.ValidateProjectPath(path);
			if (projectPath == null)
			{
				return;
			}

			var config  = ConfigFileLoader.LoadMergedConfig(projectPath);
			if (config == null)
			{
				return;
			}

			DirectoryInfo dllDir = null;

			dllDir = new DirectoryInfo(config.DllPath);

			if (!dllDir.Exists)
			{
				if (!ConsoleWriter.AskForYesNo($"Directory '{dllDir.FullName}' doesn't exist.\nDo you want to create it?"))
				{
					return;
				}
				Directory.CreateDirectory(dllDir.FullName);
			}


			int dllFileCount = dllDir.GetFiles().Count((FileInfo file) => { return file.Extension == ".dll"; });
			if (dllFileCount < 150)
			{
				if(!ConsoleWriter.AskForYesNo($"There are only {dllFileCount} dll files in directory '{dllDir.FullName}'. Are you sure this is the right one?"))
				{
					return;
				}
			}

			var proxyDllDir = new DirectoryInfo(Path.Combine(Path.GetDirectoryName(config.PrimitierPath), "MelonLoader", "Managed"));

			if (!proxyDllDir.Exists)
			{
				ConsoleWriter.WriteLineError($"'{proxyDllDir.FullName}' doesn't exist. This could be because MelonLoader is not installed properly");
				return;
			}


			foreach (var file in proxyDllDir.GetFiles())
			{
				if (file.Extension == ".dll" || file.Extension == ".xml")
				{
					ConsoleWriter.WriteLineStatus($"Copying '{file.FullName}' to '{dllDir.FullName}'");
					file.CopyTo(Path.Combine(dllDir.FullName, file.Name), true);
					
				}

			}

			ConsoleWriter.WriteLineStatus("Copying MelonLoader.dll");
			var melonloaderDll = new FileInfo(Path.Combine(Path.GetDirectoryName(config.PrimitierPath), "MelonLoader", "MelonLoader.dll"));
			melonloaderDll.CopyTo(Path.Combine(dllDir.FullName, "MelonLoader.dll"), true);

		}





	}
}
