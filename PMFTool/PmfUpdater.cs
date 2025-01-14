﻿using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace PMFTool
{
	public static class PmfUpdater
	{

		public static async void Update(DirectoryInfo projectDir, FileInfo csprojFile, DirectoryInfo pmfLib, string targetVersion, bool log=true)
		{
			//TODO: rewrite all of this ( Just comment out things that cause errors)

			if (log)
				ConsoleWriter.WriteLineStatus("=== STARTING UPDATE ===");


			var newPmfLibArchive = await PMFDownloader.DownloadPMFLib(targetVersion);

			

			var newPmfLib = new DirectoryInfo(Path.Combine(projectDir.FullName, $"PMFLib{targetVersion}"));

			//ChangeCsprojPMFLibPath(csprojFile, Path.Combine(newPmfLib.FullName));

			pmfLib.Delete();
		}


		private static void ChangeCsprojPMFLibPath(FileInfo csproj, string newPath)
		{
			//TODO: make this work

			using (var csprojFile = csproj.OpenRead())
			{
				XmlDocument doc = new XmlDocument();
				doc.Load(csprojFile);

				foreach (XmlElement referenceElement in  doc.GetElementsByTagName("Reference"))
				{
	

				}
				

			}



		}


	}
}
