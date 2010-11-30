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
		LoadTasks();
    }

	private void LoadTasks()
	{
		var taskSvc = ApplicationContext.Current.Services.Get<TaskManagerService>() as TaskManagerService;

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

		if (!task.Enabled || task.Stopped)
			status = "Stopped";

		if (!task.IsLastRunSuccessful)
			status = "Error";

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
		var tinfo = new ToolsSmartPartInfo();
		tinfo.ImagePath = "/SlxClient/ImageResource.axd?scope=global&type=Global_Images&key=Task_Main_24x24";
		tinfo.Title = "Task Console";

		return tinfo;
	}

	protected void buttonStartAll_Click(object sender, ImageClickEventArgs e)
	{
		var taskSvc = ApplicationContext.Current.Services.Get<TaskManagerService>() as TaskManagerService;
		taskSvc.StartTasks();

		buttonPauseAll.Enabled = true;
		buttonStartAll.Enabled = false;
		LoadTasks();
	}

	protected void buttonPauseAll_Click(object sender, ImageClickEventArgs e)
	{
		var taskSvc = ApplicationContext.Current.Services.Get<TaskManagerService>() as TaskManagerService;
		taskSvc.StopTasks();

		buttonPauseAll.Enabled = false;
		buttonStartAll.Enabled = true;
		LoadTasks();
	}

	protected void listTasks_ItemDataBound(object sender, RepeaterItemEventArgs e)
	{
		var task = (Task)e.Item.DataItem;
		if (task == null) return;

		var buttonStart = (ImageButton)e.Item.FindControl("buttonStart");
		var buttonPause = (ImageButton)e.Item.FindControl("buttonPause");
		var buttonDisabled = (ImageButton)e.Item.FindControl("buttonDisabled");

		buttonStart.CommandArgument = task.Name;
		buttonPause.CommandArgument = task.Name;

		if (task.Stopped)
		{
			buttonStart.Visible = true;
			buttonPause.Visible = false;
			buttonDisabled.Visible = false;
		}
		else
		{
			buttonStart.Visible = false;
			buttonPause.Visible = true;
			buttonDisabled.Visible = false;
		}

		if (!task.Enabled)
		{
			buttonStart.Visible = false;
			buttonPause.Visible = false;
			buttonDisabled.Visible = true;
		}
	}

	protected void listTasks_ItemCommand(object sender, RepeaterCommandEventArgs e)
	{
		var task = GetTask(e.CommandArgument.ToString());
		if (task == null) return;

		if (e.CommandName == "Pause")
			task.Stop();

		if (e.CommandName == "Start")
			task.Start();

		LoadTasks();
	}

	private Task GetTask(string Name)
	{
		var taskSvc = ApplicationContext.Current.Services.Get<TaskManagerService>() as TaskManagerService;
		foreach (var task in taskSvc.Tasks)
			if (task.Name == Name) return task;

		return null;
	}

	protected void Timer1_Tick(object sender, EventArgs e)
	{
		LoadTasks();
	}
}