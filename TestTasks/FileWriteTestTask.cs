using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FX.Services.Components;
using System.IO;
using System.Web;

namespace FX.Tasks
{
	public class FileWriteTestTask : TaskBase
	{
		public override void Execute(System.Xml.XmlNode configuration)
		{
			using (var outfile = new StreamWriter(Path.Combine(this.PortalRootPath, "_FileWriteTestTask_Log.txt"), true))
			{
				outfile.WriteLine(DateTime.Now.ToString() + " TestTask executed");
				outfile.Close();
			}
		}
	}
}