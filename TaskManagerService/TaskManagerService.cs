using System;
using System.Xml;
using System.Web;
using System.Collections.Generic;
using Sage.Platform;
using Sage.Platform.Application;
using Sage.Platform.Data;
using Sage.Platform.Security;
using log4net;
using FX.Services.Components;
using System.Web.Hosting;

namespace FX.Services
{
	public class TaskManagerService
	{
		#region Public Constructors

		public TaskManagerService()
		{
			LoadConfiguration();
			Initialize();
			StartTasks();
		}

		#endregion

		#region Service Dependencies

		[ServiceDependency]
		public IDataService DataService { get; set; }

		[ServiceDependency]
		public IUserService UserService { get; set; }

		#endregion

		#region Fields

		private static readonly ILog _log = LogManager.GetLogger("TaskManagerService");
		private XmlNodeList _nodes = null;

		public List<Task> Tasks = null;

		#endregion

		#region Public Methods

		public void StartTasks()
		{
			foreach (Task task in Tasks)
			{
				if (!task.IsRunning && task.Enabled)
				{
					_log.Info("Starting task " + task.Name);
					task.Start();
				}
			}
		}

		public void StopTasks()
		{
			foreach (Task task in Tasks)
			{
				if (task.Enabled)
				{
					_log.Info("Stopping task " + task.Name);
					task.Stop();
				}
			}
		}

		#endregion

		#region Private Methods

		private void LoadConfiguration()
		{
			_log.Info("Service starting. Loading tasks.config");

			try
			{
				var config = new XmlDocument();
				config.Load(HttpContext.Current.Server.MapPath("tasks.config"));
				this._nodes = config.SelectNodes("Tasks/Task");
			}
			catch (Exception ex)
			{
				_log.Error("Error loading task configuration", ex);
			}

			_log.Info(string.Format("Loaded configuration for {0} tasks", _nodes.Count));
		}

		private void Initialize()
		{
			Tasks = new List<Task>();

			foreach (XmlNode node in this._nodes)
			{
				if (node.Name == "Task")
				{
					try
					{
						XmlAttributeCollection attributes = node.Attributes;

						Task task = new Task(double.Parse(attributes["interval"].Value));
						task.Name = attributes["name"].Value;
						task.TaskType = Type.GetType(attributes["type"].Value, true);
						task.Enabled = bool.Parse(attributes["enabled"].Value);
						task.Priority = (Priority)Convert.ToInt16(attributes["priority"].Value);
						task.ConfigurationNode = node;
						task.DataService = this.DataService;
						task.UserService = this.UserService;
						task.PortalPathRoot = HostingEnvironment.ApplicationPhysicalPath;

						Tasks.Add(task);
						_log.Info("Added task " + task.Name);
					}
					catch (Exception ex)
					{
						_log.Error("Error parsing task configuration", ex);
					}
				}
			}
		}

		#endregion
	}
}
