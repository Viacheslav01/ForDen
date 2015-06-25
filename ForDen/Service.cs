using System.Linq;
using System.ServiceProcess;

namespace ForDen
{
	public partial class Service
		: ServiceBase
	{
		public Service()
		{
			InitializeComponent();

			ServiceName = "FDFileChecker";
		}

		FileCheker _fileCheker;

		protected override void OnStart(string[] args)
		{
			if (_fileCheker != null)
			{
				return;
			}

			var settings = new Settings();
			settings.Reload();

			_fileCheker = new FileCheker(settings.Period, settings.SmtpHost, settings.SmtpUser, settings.SmtpPassword, settings.FromEmail, settings.ReportEmail, settings.Files.Cast<string>().ToArray());
			_fileCheker.Start();
		}

		protected override void OnStop()
		{
			if(_fileCheker == null)
			{
				return;
			}

			_fileCheker.Stop();
			_fileCheker = null;
		}
	}
}
