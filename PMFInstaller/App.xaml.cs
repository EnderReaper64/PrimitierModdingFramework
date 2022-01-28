﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace PMFInstaller
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application
	{
		public static MainWindow MainWindow = null;
		

		private void Application_Startup(object sender, StartupEventArgs e)
		{
			
			new MainWindow();
			MainWindow.Show();
			ConfigFile.Load();
		}

		private void Application_Exit(object sender, ExitEventArgs e)
		{
			ConfigFile.Save();

		}
	}
}
