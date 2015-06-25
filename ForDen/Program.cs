using System;
using System.Configuration.Install;
using System.Linq;
using System.Reflection;
using System.ServiceProcess;

namespace ForDen
{
	internal static class Program
	{
		public static void Main(string[] args)
		{
			if (Environment.UserInteractive && HandleCmdArgs(args))
			{
				Console.Write("Press enter to close application");
				Console.ReadLine();

				return;
			}
			
			ServiceBase.Run(new Service());
		}

		private static bool HandleCmdArgs(string[] args)
		{
			if(args.Length == 0)
			{
				return false;
			}

			if(args.Length > 1)
			{
				ShowHelp();
				return true;
			}

			switch(args[0])
			{
				case "-i":
					InstallService();
					return true;

				case "-u":
					UninstallService();
					return true;

				case "-c":
					RunInConsole();
					return true;
			}

			ShowHelp();
			return true;
		}

		private static void InstallService()
		{
			ManagedInstallerClass.InstallHelper(new[] { Assembly.GetExecutingAssembly().Location });
		}

		private static void UninstallService()
		{
			ManagedInstallerClass.InstallHelper(new[] { "/u", Assembly.GetExecutingAssembly().Location });
		}

		private static void RunInConsole()
		{
			var settings = new Settings();

			var checker = new FileCheker(settings.Period, settings.SmtpHost, settings.SmtpUser, settings.SmtpPassword, settings.FromEmail, settings.ReportEmail, settings.Files.Cast<string>().ToArray());
			checker.Start();
		}

		private static void ShowHelp()
		{
			var appName = AppDomain.CurrentDomain.FriendlyName;

			Console.WriteLine("{0} -i | -u |-c", appName);
			Console.WriteLine("  -i installs windows service");
			Console.WriteLine("  -u uninstalls windows service");
			Console.WriteLine("  -c starts service in console mode");
			Console.WriteLine();
		}
	}
}
