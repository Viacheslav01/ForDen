using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Threading;
using System.Threading.Tasks;
using log4net;

namespace ForDen
{
	internal class FileCheker
	{
		private static ILog _log = LogManager.GetLogger(typeof(FileCheker));

		private readonly int _period;
		private readonly string _server;
		private readonly string _user;
		private readonly string _password;
		private readonly string _fromEmail;
		private readonly string _reportEmail;
		private readonly string[] _files;

		public FileCheker(int period, string server, string user, string password, string fromEmail, string reportEmail, string[] files)
		{
			if (period < 1)
			{
				throw new ArgumentOutOfRangeException(nameof(period), "Перод не может быть меньше одной минуты");
			}

			Guard.NotNullOrWhiteSpace(server);
			Guard.NotNullOrWhiteSpace(user);
			Guard.NotNullOrWhiteSpace(password);
			Guard.NotNullOrWhiteSpace(fromEmail);
			Guard.NotNullOrWhiteSpace(reportEmail);
			Guard.NotNull(files);

			_period = period;
			_server = server;
			_user = user;
			_password = password;
			_fromEmail = fromEmail;
			_reportEmail = reportEmail;
			_files = files;
		}

		private CancellationTokenSource _cts;

		public void Start()
		{
			var cts = new CancellationTokenSource();

			var current = Interlocked.CompareExchange(ref _cts, cts, null);
			if(current != null)
			{
				return;
			}

			_log.Info("Started");

			Task.Run(async () => await CheckFiles(cts.Token), cts.Token);
		}

		public void Stop()
		{
			_cts.Cancel();
			_cts = null;
		}

		private async Task CheckFiles(CancellationToken token)
		{
			var snapshorts = new Dictionary<string, FileSnapshort>();

			while (!token.IsCancellationRequested)
			{
				var wrongFiles = new List<string>();

				_log.Info("Begin cheking");

				foreach (var file in _files)
				{
					try
					{
						var fileInfo = new FileInfo(file);

						FileSnapshort snapshort;
						if (!snapshorts.TryGetValue(file, out snapshort))
						{
							snapshorts.Add(file, new FileSnapshort(fileInfo));
							continue;
						}

						if (!snapshort.CompareAndUpdate(fileInfo))
						{
							continue;
						}

						wrongFiles.Add(file);
					}
					catch(Exception ex)
					{
						_log.Error("Error on file checking", ex);
					}
				}

				if(wrongFiles.Count > 0)
				{
					_log.Info("Wrong files detected");
					SendEmail(wrongFiles);
				}

				_log.Info("Cheking complete");

				await Task.Delay(_period * 1000 * 60, token);
			}

			_log.Info("Stoped");
		}

		private void SendEmail(List<string> wrongFiles)
		{
			try
			{
				_log.InfoFormat("Email sending to: {0}", _reportEmail);

				var smtp = new SmtpClient(_server)
				{
					Credentials = new NetworkCredential(_user, _password)
				};

				smtp.Send(_fromEmail, _reportEmail, "Отчет проверки файлов", string.Join(Environment.NewLine, wrongFiles));
			}
			catch (Exception ex)
			{
				_log.Error("Error was received on email sent", ex);
			}
		}
	}
}
