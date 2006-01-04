using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using ProjectTracker.Library;

public partial class ProjectList : System.Web.UI.Page
{
  protected void Page_Load(object sender, EventArgs e)
  {
    if (!IsPostBack)
      ApplyAuthorizationRules();
    else
      ErrorLabel.Text = string.Empty;
  }

  private void ApplyAuthorizationRules()
  {
    this.GridView1.Columns[
      this.GridView1.Columns.Count - 1].Visible =
      Project.CanDeleteObject();
    NewProjectButton.Visible =
      ProjectTracker.Library.Project.CanAddObject();
  }

  protected void GridView1_SelectedIndexChanged(
    object sender, EventArgs e)
  {
    // allow user to view or edit selected project
    string idString = GridView1.SelectedDataKey.Value.ToString();
    Response.Redirect("ProjectEdit.aspx?id=" + idString);
  }

  protected void NewProjectButton_Click(object sender, EventArgs e)
  {
    // allow user to add a new project
    Response.Redirect("ProjectEdit.aspx");
  }

  private ProjectTracker.Library.ProjectList GetProjectList()
  {
    object businessObject = Session["currentObject"];
    if (businessObject == null || 
      !(businessObject is ProjectTracker.Library.ProjectList))
    {
      businessObject =
        ProjectTracker.Library.ProjectList.GetProjectList();
      Session["currentObject"] = businessObject;
    }
    return (ProjectTracker.Library.ProjectList)businessObject;
  }

  #region ProjectListDataSource

  protected void ProjectListDataSource_DeleteObject(
    object sender, Csla.Web.DeleteObjectArgs e)
  {
    try
    {
      ProjectTracker.Library.Project.DeleteProject(
        new Guid(e.Keys["Id"].ToString()));
      e.RowsAffected = 1;
    }
    catch (Csla.DataPortalException ex)
    {
      this.ErrorLabel.Text = ex.BusinessException.Message;
      e.RowsAffected = 0;
    }
    catch (Exception ex)
    {
      this.ErrorLabel.Text = ex.Message;
      e.RowsAffected = 0;
    }
  }

  protected void ProjectListDataSource_SelectObject(
    object sender, Csla.Web.SelectObjectArgs e)
  {
    e.BusinessObject = GetProjectList();
  }

  #endregion
}
