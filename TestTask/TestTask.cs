using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FX.Services.Components;
using System.IO;
using System.Web;

namespace FX.Tasks
{
	public class TestTask : TaskBase
	{
		public override void Execute(System.Xml.XmlNode configuration)
		{
			using (var outfile = new StreamWriter(Path.Combine(HttpContext.Current.Server.MapPath("/"), "_TestTask_Log.txt")))
			{
				outfile.WriteLine(DateTime.Now.ToString() + " TestTask executed");
				outfile.Close();
			}
		}
	}
}
