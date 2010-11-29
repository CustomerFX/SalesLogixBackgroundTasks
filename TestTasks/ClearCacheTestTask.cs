using System;
using System.Collections;
using System.Web;
using FX.Services.Components;

namespace FX.Tasks
{
	public class ClearCacheTestTask : TaskBase
	{
		public override void Execute(System.Xml.XmlNode configuration)
		{
			foreach (DictionaryEntry entry in HttpRuntime.Cache)
			{
				HttpRuntime.Cache.Remove(entry.Key.ToString());
			}
		}
	}
}
