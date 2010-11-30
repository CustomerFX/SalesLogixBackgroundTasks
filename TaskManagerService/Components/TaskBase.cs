using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FX.Services.Components
{
	public abstract class TaskBase : ITask
	{
		public abstract void Execute(System.Xml.XmlNode configuration);

		public Sage.Platform.Data.IDataService DataService { get; set; }
		public Sage.Platform.Security.IUserService UserService { get; set; }
		public string PortalRootPath { get; set; }
	}
}
