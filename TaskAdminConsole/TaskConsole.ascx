<%@ Control Language="C#" AutoEventWireup="true" CodeFile="TaskConsole.ascx.cs" Inherits="TaskConsole" %>
<table border="0" cellpadding="1" cellspacing="0" class="formtable">
	<col width="100%" />
	<tr>
		<td>

			<table border="0" cellpadding="0" cellspacing="0" style="width:98%;" class="datagrid">
			<tbody>
			<tr class="gridPager" align="right">
				<td style="white-space: nowrap; width: 20px;" colspan="4">
					<asp:Label runat="server" ID="labelTasks"></asp:Label>
				</td>
			</tr>

			<asp:Repeater runat="server" ID="listTasks">
				<HeaderTemplate>
					<tr class="rowhead">
						<th style="width:auto;">Task</td>
						<th>Status</td>
						<th>Last Run</td>
						<th>Next Run</td>
					</tr>
				</HeaderTemplate>
				<ItemTemplate>
					<tr style="<%# GetItemStyle() %>">
						<td>
							<img src="/SlxClient/images/BackgroundTasks/Task_<%# GetTaskStatus((FX.Services.Components.Task)DataBinder.GetDataItem(Container)) %>_16x16.png" />
							&nbsp;
							<%# DataBinder.Eval(Container.DataItem, "Name") %>
						</td>
						<td>
							<%# GetTaskStatus((FX.Services.Components.Task)DataBinder.GetDataItem(Container)) %>
						</td>
						<td>
							<%# Convert.ToDateTime(DataBinder.Eval(Container.DataItem, "LastRunTime")) == DateTime.MinValue ? "&nbsp;" : DataBinder.Eval(Container.DataItem, "LastRunTime") %>
						</td>
						<td>
							<%# Convert.ToDateTime(DataBinder.Eval(Container.DataItem, "LastRunTime")) == DateTime.MinValue ? "&nbsp;" : Convert.ToDateTime(DataBinder.Eval(Container.DataItem, "LastRunTime")).AddMilliseconds(Convert.ToInt32(DataBinder.Eval(Container.DataItem, "Interval"))).ToString()%>
						</td>
					</tr>
				</ItemTemplate>
				<FooterTemplate></FooterTemplate>
			</asp:Repeater>
			
			<asp:PlaceHolder runat="server" ID="sectionNoItems">
				<tr>
					<td colspan="4">
						No configured tasks. Visit <a href="http://customerfx.com/" target="_blank">customerfx.com</a> for information on creating custom background tasks.
					</td>
				</tr>
			</asp:PlaceHolder>
			
			</tbody></table>

			<br /><br />
			<asp:Image runat="server" ID="imgLogo" ImageUrl="~/images/BackgroundTasks/cfxlogo.jpg" BorderWidth="0" ImageAlign="AbsMiddle" />
			<span style="font-size:smaller; color:Gray">&copy; <%=DateTime.Now.Year %> Customer FX Corporation - <a href="http://customerfx.com/" target="_blank">customerfx.com</a></span>

		</td>
	</tr>
</table>