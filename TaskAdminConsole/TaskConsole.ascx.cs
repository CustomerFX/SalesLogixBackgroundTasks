using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Sage.Platform.WebPortal.SmartParts;
using Sage.Platform.Application.UI;
using Sage.Platform.Application;
using FX.Services;
using FX.Services.Components;

public partial class TaskConsole : UserControl, ISmartPartInfoProvider
{
    protected void Page_Load(object sender, EventArgs e)
    {
		TaskManagerService taskSvc = ApplicationContext.Current.Services.Get<TaskManagerService>() as TaskManagerService;

		labelTasks.Text = string.Format("{0} task{1} configured", taskSvc.Tasks.Count, (taskSvc.Tasks.Count == 1 ? "" : "s"));

		listTasks.DataSource = taskSvc.Tasks;
		sectionNoItems.Visible = (taskSvc.Tasks.Count == 0);

		listTasks.DataBind();
    }

	protected string GetTaskStatus(Task task)
	{
		string status = "Waiting";

		if (task.IsRunning)
			status = "Running";

		if (!task.Enabled)
			status = "Stopped";

		return status;
	}

	int eo = 1;
	protected string GetItemStyle()
	{
		eo++;
		return (eo % 2 == 0 ? "" : "background-color:whitesmoke;");
	}

	public ISmartPartInfo GetSmartPartInfo(Type smartPartInfoType)
	{
		ToolsSmartPartInfo tinfo = new ToolsSmartPartInfo();
		tinfo.ImagePath = "/SlxClient/ImageResource.axd?scope=global&type=Global_Images&key=Task_Main_16x16";
		tinfo.Title = "Task Console";

		return tinfo;
	}
}