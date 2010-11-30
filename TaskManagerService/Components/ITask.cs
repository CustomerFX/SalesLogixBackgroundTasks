using System;
using System.Xml;

namespace FX.Services.Components
{
	public interface ITask
	{
		void Execute(XmlNode configuration);

		Sage.Platform.Data.IDataService DataService { get; set; }
		Sage.Platform.Security.IUserService UserService { get; set; }
		string PortalRootPath { get; set; }
	}
}
