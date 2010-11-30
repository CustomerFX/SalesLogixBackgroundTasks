using System;
using System.Reflection;
using System.Threading;
using System.Timers;
using System.Xml;
using log4net;

namespace FX.Services.Components
{
	public class Task
	{
		#region Fields

		private System.Timers.Timer timer = null;
		private static readonly ILog _log = LogManager.GetLogger("TaskManagerService");

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

		public string PortalPathRoot { get; set; }

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
			if (!this.Enabled) return;

			_log.Info(string.Format("Task {0} starting", this.Name));
			this.Stopped = false;
			this.StartTask();
		}

		public void Stop()
		{
			if (!this.Enabled) return;

			_log.Info(string.Format("Task {0} stopping", this.Name));
			this.Stopped = true;
		}

		#endregion

		#region Private Methods

		private void Initialize()
		{
			_log.Info(string.Format("Initializing task {0}", this.Name));
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
				_log.Info(string.Format("Executing task {0}", this.Name));

				this.IsRunning = true;
				this.LastRunTime = DateTime.Now;

				var method = this.TaskType.GetMethod("Execute");
				object[] arguments = { this.ConfigurationNode };

				object obj = Activator.CreateInstance(this.TaskType);

				var propDataSvc = this.TaskType.GetProperty("DataService");
				propDataSvc.SetValue(obj, this.DataService, null);

				var propUserSvc = this.TaskType.GetProperty("UserService");
				propUserSvc.SetValue(obj, this.UserService, null);

				var propPortalPath = this.TaskType.GetProperty("PortalRootPath");
				propPortalPath.SetValue(obj, this.PortalPathRoot, null);

				method.Invoke(obj, new object[] { this.ConfigurationNode });

				this.IsLastRunSuccessful = true;
			}
			catch (Exception ex)
			{
				this.IsLastRunSuccessful = false;
				_log.Error("Error executing task " + this.Name, ex);
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
