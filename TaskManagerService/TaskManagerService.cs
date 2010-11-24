using System;
using Sage.Platform;
using Sage.Platform.Application;
using Sage.Platform.Data;
using Sage.Platform.Security;
using System.Collections.Generic;
using System.Xml;
using FX.Services.Components;
using System.Web;

namespace FX.Services
{
	public class TaskManagerService
	{
		#region Public Constructors

		public TaskManagerService()
		{
			XmlDocument xml = new XmlDocument();
			xml.Load(HttpContext.Current.Server.MapPath("tasks.config"));
			this._nodes = xml.SelectNodes("Tasks/Task");

			Initialize();

			this.StartTasks();
		}

		#endregion

		#region Service Dependencies

		[ServiceDependency]
		public IDataService DataService { get; set; }

		[ServiceDependency]
		public IUserService UserService { get; set; }

		#endregion

		#region Fields

		public static List<Task> _tasks = null;
		private XmlNodeList _nodes = null;

		#endregion

		#region Public Methods

		public void StartTasks()
		{
			foreach (Task task in _tasks)
			{
				if (!task.IsRunning)
					task.Start();
			}
		}

		public void StopTasks()
		{
			foreach (Task task in _tasks)
			{
				task.Stop();
			}
		}

		#endregion

		#region Private Methods

		private void Initialize()
		{
			_tasks = new List<Task>();

			foreach (XmlNode node in this._nodes)
			{
				if (node.Name == "Task")
				{
					try
					{
						XmlAttributeCollection attributes = node.Attributes;

						if (bool.Parse(attributes["enabled"].Value))
						{
							Task task = new Task(double.Parse(attributes["interval"].Value));

							task.Name = attributes["name"].Value;
							task.TaskType = Type.GetType(attributes["type"].Value, true);
							task.Enabled = bool.Parse(attributes["enabled"].Value);
							task.Priority = (Priority)Convert.ToInt16(attributes["priority"].Value);
							task.ConfigurationNode = node;
							task.DataService = this.DataService;
							task.UserService = this.UserService;

							_tasks.Add(task);
						}
					}
					catch
					{
						// Handle the exception or log a warning
					}
				}
			}
		}

		#endregion
	}
}
