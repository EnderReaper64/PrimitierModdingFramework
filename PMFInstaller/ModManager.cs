﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace PMFInstaller
{
	public static class ModManager
	{

		public static List<Mod> LoadedMods = new List<Mod>();
		public static List<Mod> ActiveMods = new List<Mod>();

		public static event Action OnModListsUpdate;


		public static void EnableMod(Mod mod)
		{
			if (ActiveMods.Contains(mod))
			{
				return;
			}
			if (!LoadedMods.Contains(mod))
			{
				return;
			}

			LoadedMods.Remove(mod);
			
			ActiveMods.Add(mod);
			AddToActiveModsConfig(mod.Name);



		   OnModListsUpdate?.Invoke();

		}



		public static void DisableMod(Mod mod)
		{
			if (!ActiveMods.Contains(mod))
			{
				return;
			}

			ActiveMods.Remove(mod);
			RemoveFromActiveModsConfig(mod.Name);

			if (!LoadedMods.Contains(mod))
			{
				LoadedMods.Add(mod);
			}


			OnModListsUpdate?.Invoke();
		}

		private static void AddToActiveModsConfig(string name)
		{
			var sourceArray = ConfigFile.Config.ActiveMods;
			string[] newActiveModHashArray = new string[sourceArray.Length + 1];
			Array.Copy(sourceArray, newActiveModHashArray, sourceArray.Length);
			newActiveModHashArray[sourceArray.Length] = name;
			ConfigFile.Config.ActiveMods = newActiveModHashArray;

		}
		private static void RemoveFromActiveModsConfig(string name)
		{

			string[] newActiveModHashArray = new string[ConfigFile.Config.ActiveMods.Length - 1];
			int ii = 0;
			for (int i = 0; i < ConfigFile.Config.ActiveMods.Length; i++)
			{
				var file = ConfigFile.Config.ActiveMods[i];
				if (file == name)
				{
					continue;
				}
				newActiveModHashArray[ii] = file;

				ii++;
			}
			ConfigFile.Config.ActiveMods = newActiveModHashArray;
		}


		public static void ReloadMods()
		{

			LoadedMods.Clear();
			ActiveMods.Clear();

			if (ConfigFile.Config == null)
			{
				ConfigFile.Load();
			}
			if (ConfigFile.Config == null)
			{
				return;
			}
			
			var modDirFiles = Directory.GetFiles(ConfigFile.PMFModsDirPath);
			List<string> ActiveModsNames = new List<string>(modDirFiles.Length);
			foreach (var modPath in modDirFiles)
			{
				var mod = LoadModFromFile(modPath);
				if (mod != null)
				{
					if (LoadedMods.Contains(mod) || ActiveMods.Contains(mod))
					{
						ErrorHandeler.ShowError("Found 2 or more mods with the same Hash");
						continue;
					}
					


					if (ConfigFile.Config.ActiveMods.Contains(mod.Name))
					{
						ActiveMods.Add(mod);
						ActiveModsNames.Add(mod.Name);
						
					}
					else
					{
						LoadedMods.Add(mod);
					}

				}

			}
			ConfigFile.Config.ActiveMods = ActiveModsNames.ToArray();
			ConfigFile.Save();

			OnModListsUpdate?.Invoke();
		}

		public static ZipArchiveEntry GenerateModJsonFile(ZipArchive zip, string zipFilePath)
		{
			
			var modJsonEntry = zip.CreateEntry("Mod.json");
			var modJsonStream = modJsonEntry.Open();

			var mod = new Mod();
			mod.Name = Path.GetFileNameWithoutExtension(zipFilePath);
			mod.DisplayName = mod.Name;
			mod.Description = "Generated from a melon mod";
			mod.FileName = zipFilePath;
			mod.IsGenerated = true;
			mod.InitUI();


			var stringMod = JsonConvert.SerializeObject(mod);

			modJsonStream.Write(Encoding.ASCII.GetBytes(stringMod));

			modJsonStream.Close();

			return modJsonEntry;
		}

		public static void GenerateModJsonFile(string filePath)
		{
			var zipStream = File.Open(filePath, FileMode.Open, FileAccess.ReadWrite);

			var zip = new ZipArchive(zipStream, ZipArchiveMode.Update);
			GenerateModJsonFile(zip, filePath);

			zip.Dispose();
			zipStream.Close();
		}



		public static void DeleteMod(Mod mod)
		{
			if (!File.Exists(mod.FileName))
			{
				return;
			}

			File.Delete(mod.FileName);


			ReloadMods();
		}
		public static void AddMod(string file)
		{
			if (!ValidateModFile(file, true, tryFix:true))
			{
				return;
			}

			try
			{
				File.Copy(file, Path.Combine(ConfigFile.PMFModsDirPath, Path.GetFileName(file)), true);
			}catch (Exception e)
			{
				ErrorHandeler.ShowError("Can not copy mod files");

			}

			ReloadMods();
		}


		public static Mod? LoadModFromFile(string file)
		{
			var zipStream = File.Open(file, FileMode.Open, FileAccess.ReadWrite);
			var zip = new ZipArchive(zipStream, ZipArchiveMode.Update);

			var modjsonEntry = FindEntryZip("Mod.json", zip);
			if (modjsonEntry == null)
			{
				modjsonEntry = GenerateModJsonFile(zip, file);
			}

			Mod mod = null;
			var bytes = ReadEntryZipBytes(modjsonEntry);
			try
			{
				mod = JsonConvert.DeserializeObject<Mod>(Encoding.ASCII.GetString(bytes));
			}
			catch (Exception e)
			{
				ErrorHandeler.ShowError($"Can not load mod '{file}' invalid Mod.json");
				return null;
			}

			var iconEntry = FindEntryZip("Icon.png", zip);
			if (iconEntry != null)
			{
				throw new NotImplementedException();
			}

			zip.Dispose();
			zipStream.Close();


			mod.Name = Path.GetFileNameWithoutExtension(file);
			mod.FileName = file;
			mod.InitUI();
			return mod;
		}

		private static bool ValidateModFile(string file, bool displayErrors=false, bool tryFix=false)
		{
			if (!File.Exists(file))
			{
				if (displayErrors)
					ErrorHandeler.ShowError($"Can not find mod '{file}'");
				return false;
			}

			if (Path.GetExtension(file) != ".pmfm" && Path.GetExtension(file) != ".zip")
			{
				if (displayErrors)
					ErrorHandeler.ShowError($"Mod '{file}' is not the right file type");
				return false;
			}

			ZipArchive zip=null;
			FileStream zipStream=null;
			try
			{
				zipStream = File.Open(file, FileMode.Open, FileAccess.ReadWrite);
				zip = new ZipArchive(zipStream, ZipArchiveMode.Update, true);
			}catch(Exception e)
			{
				if (displayErrors)
					ErrorHandeler.ShowError($"Can't read '{file}'");
				return false;
			}
			
			if (FindEntryZip("Mod.json", zip) == null)
			{
				if (tryFix)
				{
					GenerateModJsonFile(zip, file);
				}
				else
				{
					ErrorHandeler.ShowError($"Mod '{file}' doesn't contain a Mod.json file");
					return false;
				}

				
			}



			zipStream?.Close();
			return true;
		}


		private static ZipArchiveEntry? FindEntryZip(string filePath, ZipArchive zip)
		{
			foreach (var entry in zip.Entries)
			{
				if (entry.FullName == filePath)
				{
					return entry;
				}

			}

			return null;
		}

		private static byte[] ReadEntryZipBytes(ZipArchiveEntry entry)
		{
			var stream = entry.Open();
			
			byte[] bytes = new byte[stream.Length];
			stream.Read(bytes, 0, bytes.Length);

			stream.Close();
			return bytes;
		}

	}
}
