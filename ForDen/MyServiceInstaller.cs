using System.Collections;
using System.ComponentModel;
using System.Configuration.Install;
using System.ServiceProcess;

namespace ForDen
{
	[RunInstaller(true)]
	public partial class MyServiceInstaller
		: Installer
	{
		public MyServiceInstaller()
		{
			InitializeComponent();
		}

		public override void Install(IDictionary savedState)
		{
			ConfigureInstallers();
			base.Install(savedState);
		}

		public override void Uninstall(IDictionary savedState)
		{
			ConfigureInstallers();
			base.Uninstall(savedState);
		}

		private void ConfigureInstallers()
		{
			Installers.Add(ConfigureProcessInstaller());
			Installers.Add(ConfigureServiceInstaller());
		}

		private ServiceProcessInstaller ConfigureProcessInstaller()
		{
			var result = new ServiceProcessInstaller
			{
				Account = ServiceAccount.LocalService,
				Username = null,
				Password = null
			};

			return result;
		}

		private ServiceInstaller ConfigureServiceInstaller()
		{
			var result = new ServiceInstaller
			{
				ServiceName = "FDFileChecker",
				DisplayName = "FD Files Checker",
				Description = "Сервис периодической проверки фалов на обязательное наличие изменений",
				StartType = ServiceStartMode.Automatic
			};

			return result;
		}
	}
}
