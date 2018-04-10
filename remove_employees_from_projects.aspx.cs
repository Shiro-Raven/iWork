﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Text;

public partial class remove_employees_from_projects : System.Web.UI.Page
{
    protected DropDownList regular_employees_ddl = new DropDownList();

    protected void Page_Load(object sender, EventArgs e)
    {
        string connStr = ConfigurationManager.ConnectionStrings["iWorkDBConn"].ToString();
        SqlConnection conn = new SqlConnection(connStr);

        SqlCommand cmd = new SqlCommand("get_all_regulars_in_manager_department", conn);
        cmd.CommandType = CommandType.StoredProcedure;

        conn.Open();

        //retrieve the username of the manager

        string username = Session["username"].ToString();
        cmd.Parameters.Add(new SqlParameter("@manager_username", username));

        SqlDataReader rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection);

        int counter = 0;


        StringBuilder reg_employees_table = new StringBuilder();

        //username, salary, day_off, number_of_leaves,
        //company_email, job_id
        reg_employees_table.Append("<H3>Extra Information about the regular employees.</H3>" +
            "<table><tr><th>Username</th><th>Full Name</th><th>Job ID</th><th>Job Title</th><th>Company Email</th></tr>");
        while (rdr.Read())
        {
            //add item to dropdown list
            ListItem listItem = new ListItem();

            listItem.Text = rdr.GetValue(rdr.GetOrdinal("username")).ToString();

            regular_employees_ddl.Items.Add(listItem);
            reg_employees_table.Append(
                "<tr><th>" + rdr.GetValue(rdr.GetOrdinal("username")) + "</th>"
                + "<th>" + rdr.GetValue(rdr.GetOrdinal("first_name")) + " " + rdr.GetValue(rdr.GetOrdinal("middle_name")) + " " + rdr.GetValue(rdr.GetOrdinal("last_name")) + "</th>"
                + "<th>" + rdr.GetValue(rdr.GetOrdinal("job_id")) + "</th>"
                + "<th>" + rdr.GetValue(rdr.GetOrdinal("title")) + "</th>"
                + "<th>" + rdr.GetValue(rdr.GetOrdinal("company_email")) + "</th></tr>"
                );

            counter++;
        }
        conn.Close();
        reg_employees_table.Append("</table>");


        remove_form.Controls.Add(new Literal { Text = "<H2>Please choose a regular employee to remove from to project " + Session["project_to_remove_from"].ToString() + ".</H2>" });

        remove_form.Controls.Add(new Literal { Text = "<H3>There are " + counter + " regular employees in your department to choose from.</H3>" });

        if (counter > 0)
        {
            remove_form.Controls.Add(new Literal { Text = "<p> Select Employee Username: </p> " });
            remove_form.Controls.Add(regular_employees_ddl);
            remove_form.Controls.Add(new Literal { Text = "<br/>" });

            Button removeButton = new Button();
            removeButton.Text = "Remove";
            removeButton.Click += new EventHandler(perform_removal);

            remove_form.Controls.Add(removeButton);
            remove_form.Controls.Add(new Literal { Text = reg_employees_table.ToString() });
        }
    }


    protected void perform_removal(object sender, EventArgs e)
    {
        string regular_employee_username = regular_employees_ddl.SelectedItem.Text;

        string[] project_pk = Session["project_to_remove_from"].ToString().Split('/');

        string company_domain = project_pk[0];
        string project_name = project_pk[1];

        //establish connection
        string connStr = ConfigurationManager.ConnectionStrings["iWorkDBConn"].ToString();
        SqlConnection conn = new SqlConnection(connStr);

        //reference procedure
        SqlCommand cmd = new SqlCommand("remove_from_project", conn);
        cmd.CommandType = CommandType.StoredProcedure;

        //add parameters
        //@compdomain varchar(120),@projname varchar(200),@employee varchar(200), @message int output
        cmd.Parameters.Add(new SqlParameter("@compdomain", company_domain));
        cmd.Parameters.Add(new SqlParameter("@projname", project_name));
        cmd.Parameters.Add(new SqlParameter("@employee", regular_employee_username));

        SqlParameter message = cmd.Parameters.Add("@message", SqlDbType.Int);
        message.Direction = ParameterDirection.Output;

        //execute procedure
        conn.Open();
        cmd.ExecuteNonQuery();
        conn.Close();

        switch (message.Value.ToString())
        {
            case "0":
                Response.Write(regular_employee_username+" is currently assigned to unfinished tasks in this project. " +
                    "Please wait until he finishes all his tasks and they become \"Closed\", or" +
                    " assign the tasks to another employee before removing him from the project.");
                break;
            case "1":
                Response.Write(regular_employee_username + " is already not working on this project.");
                break;
            case "2":
                Response.Write(regular_employee_username+" has been successfully removed from the project.");
                break;
        }
    }

    protected void to_project_assignment(object sender, EventArgs e)
    {
        Response.Redirect("project_assignment", true);
    }

}