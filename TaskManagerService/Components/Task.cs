using System;
using System.Reflection;
using System.Threading;
using System.Timers;
using System.Xml;

namespace FX.Services.Components
{
	public class Task
	{
		#region Fields

		System.Timers.Timer timer = null;

		#endregion

		#region Properties

		public string Name { get; set; }

		public Type TaskType { get; set; }

		public bool IsRunning { get; set; }

		public bool Enabled { get; set; }

		public DateTime LastRunTime { get; set; }

		public bool IsLastRunSuccessful { get; set; }

		public double Interval { get; set; }

		public bool Stopped { get; set; }

		public XmlNode ConfigurationNode { get; set; }

		public Sage.Platform.Data.IDataService DataService { get; set; }

		public Sage.Platform.Security.IUserService UserService { get; set; }

		public Priority Priority { get; set; }

		#endregion

		#region Public Constructors

		public Task(double interval)
		{
			this.Interval = interval;
			Initialize();
		}

		#endregion

		#region Public Methods

		public void Start()
		{
			this.Stopped = false;
			this.StartTask();
		}

		public void Stop()
		{
			this.Stopped = true;
		}

		#endregion

		#region Private Methods

		private void Initialize()
		{
			this.Stopped = false;
			this.Enabled = true;

			timer = new System.Timers.Timer(this.Interval);
			timer.Elapsed += new ElapsedEventHandler(timer_Elapsed);
			timer.Enabled = true;
		}

		private void StartTask()
		{
			if (!this.Stopped)
			{
				var thread = new Thread(new ThreadStart(Execute));
				thread.Start();
			}
		}

		private void Execute()
		{
			try
			{
				this.IsRunning = true;

				this.LastRunTime = DateTime.Now;

				var method = this.TaskType.GetMethod("Execute");
				object[] arguments = { this.ConfigurationNode };

				object obj = Activator.CreateInstance(this.TaskType);

				var propDataSvc = this.TaskType.GetProperty("DataService");
				propDataSvc.SetValue(obj, this.DataService, null);

				var propUserSvc = this.TaskType.GetProperty("UserService");
				propUserSvc.SetValue(obj, this.UserService, null);

				method.Invoke(obj, new object[] { this.ConfigurationNode });

				this.IsLastRunSuccessful = true;
			}
			catch
			{
				this.IsLastRunSuccessful = false;
			}
			finally
			{
				this.IsRunning = false;
			}
		}

		void timer_Elapsed(object sender, ElapsedEventArgs e)
		{
			if (!this.IsRunning)
				StartTask();
		}

		#endregion
	}
}
