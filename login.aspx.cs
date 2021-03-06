﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Collections;

public partial class login : System.Web.UI.Page
{
    static ArrayList arraylist_company_domain_name = new ArrayList();
    static ArrayList arraylist_department_code = new ArrayList();
    protected void Page_Load(object sender, EventArgs e)
    {
        if (IsPostBack)
            return;

        Refresh_Company(sender, e);
    }
    protected void Refresh_Company(object sender, EventArgs e)
    {
        string connStr = ConfigurationManager.ConnectionStrings["iWorkDBConn"].ToString();
        SqlConnection conn = new SqlConnection(connStr);

        SqlCommand cmd = new SqlCommand("view_all_companies", conn);
        cmd.CommandType = CommandType.StoredProcedure;

        SqlParameter output_message = cmd.Parameters.Add("@output_message", SqlDbType.Int);
        output_message.Direction = ParameterDirection.Output;

        conn.Open();
        SqlDataReader rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection);

        while (rdr.Read())
        {
            arraylist_company_domain_name.Add(rdr.GetString(rdr.GetOrdinal("domain_name")));
            drpdwnlst_view_certain_Company.Items.Add(rdr.GetString(rdr.GetOrdinal("name")));
            drpdwnlst_view_certain_department_company_company.Items.Add(rdr.GetString(rdr.GetOrdinal("name")));
        }

        rdr.Close();

        if (!output_message.Value.ToString().Equals("2"))
            tbl_output_table.Rows.Clear();
        if (output_message.Value.ToString().Equals("0"))
            Response.Write("Something Went Wrong. Please Contact The Website Admins Immediately!");
        else if (output_message.Value.ToString().Equals("1"))
            Response.Write("No Companies Saved On The Website Yet!");

        Refresh_Department(sender, e);
    }
    protected void Refresh_Department(object sender, EventArgs e)
    {
        if (arraylist_company_domain_name.Count < 0)
            return;

        string connStr = ConfigurationManager.ConnectionStrings["iWorkDBConn"].ToString();
        SqlConnection conn = new SqlConnection(connStr);

        SqlCommand cmd = new SqlCommand("view_certain_Company_Department", conn);
        cmd.CommandType = CommandType.StoredProcedure;

        string domain_name = arraylist_company_domain_name[drpdwnlst_view_certain_department_company_company.SelectedIndex].ToString();
        cmd.Parameters.AddWithValue("@domain_name", domain_name);

        SqlParameter output_message = cmd.Parameters.Add("@output_message", SqlDbType.Int);
        output_message.Direction = ParameterDirection.Output;

        conn.Open();
        SqlDataReader rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection);

        arraylist_department_code.Clear();
        drpdwnlst_view_certain_department_company_department.Items.Clear();

        while (rdr.Read())
        {
            arraylist_department_code.Add(rdr.GetString(rdr.GetOrdinal("code")));
            drpdwnlst_view_certain_department_company_department.Items.Add(rdr.GetString(rdr.GetOrdinal("dep_name")));
        }

        rdr.Close();

        if (!output_message.Value.ToString().Equals("4"))
            tbl_output_table.Rows.Clear();
        if (output_message.Value.ToString().Equals("0") || output_message.Value.ToString().Equals("2"))
            Response.Write("Something Went Wrong. Please Contact The Website Admins Immediately!");
        else if (output_message.Value.ToString().Equals("1"))
            Response.Write("No Companies Saved On The Website Yet!");
        else if (output_message.Value.ToString().Equals("3"))
            Response.Write("This Company Has No Departments!");
    }
    protected void Login(object sender, EventArgs e)
    {
        string connStr = ConfigurationManager.ConnectionStrings["iWorkDBConn"].ToString();
        SqlConnection conn = new SqlConnection(connStr);

        SqlCommand cmd = new SqlCommand("iwork_login", conn);
        cmd.CommandType = CommandType.StoredProcedure;

        string username = txtbox_username.Text;
        string password = txtbox_password.Text;

        cmd.Parameters.AddWithValue("@username", username);
        cmd.Parameters.AddWithValue("@password", password);

        SqlParameter output_message = cmd.Parameters.Add("@output_message", SqlDbType.Int);
        output_message.Direction = ParameterDirection.Output;

        conn.Open();
        cmd.ExecuteNonQuery();
        conn.Close();

        if (output_message.Value.ToString().Equals("0"))
            Response.Write("Invalid Input(s)");
        else if (output_message.Value.ToString().Equals("1"))
            Response.Write(("Empty Input(s)"));
        else if (output_message.Value.ToString().Equals("2"))
            Response.Write("NULL Input(s)");
        else if (output_message.Value.ToString().Equals("3"))
            Response.Write("WRONG Username OR Password!");
        else if (output_message.Value.ToString().Equals("9"))
            Response.Write("USER Is Neither Job Seeker Or Staff Member. Please Register A New Account!");
        else
        {
            Session["username"] = username;
            Response.Write("LOGGED IN!");
            Response.Redirect("logged_in", true);
        }
    }
    protected void Register(object sender, EventArgs e)
    {
        Response.Redirect("register", true);
    }
    protected void Search_Company_By(object sender, EventArgs e)
    {
        string connStr = ConfigurationManager.ConnectionStrings["iWorkDBConn"].ToString();
        SqlConnection conn = new SqlConnection(connStr);

        SqlCommand cmd = new SqlCommand("view_all_companies", conn);
        cmd.CommandType = CommandType.StoredProcedure;

        string search_keyword = txtbox_search_company_by.Text;

        if (drpdwnlst_search_company_by.SelectedIndex.ToString().Equals("0"))
        {
            cmd.CommandText = "search_for_Company_by_Name";
            cmd.Parameters.AddWithValue("@name", search_keyword);
        }
        else if (drpdwnlst_search_company_by.SelectedIndex.ToString().Equals("1"))
        {
            cmd.CommandText = "search_for_Company_by_Address";
            cmd.Parameters.AddWithValue("@address", search_keyword);
        }
        else if (drpdwnlst_search_company_by.SelectedIndex.ToString().Equals("2"))
        {
            cmd.CommandText = "search_for_Company_by_Type";
            cmd.Parameters.AddWithValue("@type", search_keyword);
        }

        SqlParameter output_message = cmd.Parameters.Add("@output_message", SqlDbType.Int);
        output_message.Direction = ParameterDirection.Output;

        conn.Open();
        SqlDataReader rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection);

        tbl_output_table.Rows.Clear();

        TableHeaderRow tblhdrrw = new TableHeaderRow();

        TableHeaderCell tblhdrcl = new TableHeaderCell();
        tblhdrcl.Controls.Add(new LiteralControl("Company's Domain Name"));
        tblhdrrw.Cells.Add(tblhdrcl);

        tblhdrcl = new TableHeaderCell();
        tblhdrcl.Controls.Add(new LiteralControl("Company's Name"));
        tblhdrrw.Cells.Add(tblhdrcl);

        tblhdrcl = new TableHeaderCell();
        tblhdrcl.Controls.Add(new LiteralControl("Company's Address"));
        tblhdrrw.Cells.Add(tblhdrcl);

        tblhdrcl = new TableHeaderCell();
        tblhdrcl.Controls.Add(new LiteralControl("Company's Email"));
        tblhdrrw.Cells.Add(tblhdrcl);

        tblhdrcl = new TableHeaderCell();
        tblhdrcl.Controls.Add(new LiteralControl("Company's Vision"));
        tblhdrrw.Cells.Add(tblhdrcl);

        tblhdrcl = new TableHeaderCell();
        tblhdrcl.Controls.Add(new LiteralControl("Company's Type"));
        tblhdrrw.Cells.Add(tblhdrcl);

        tblhdrcl = new TableHeaderCell();
        tblhdrcl.Controls.Add(new LiteralControl("Company's Specialization"));
        tblhdrrw.Cells.Add(tblhdrcl);

        tblhdrcl = new TableHeaderCell();
        tblhdrcl.Controls.Add(new LiteralControl("Company's Phone Number(s)"));
        tblhdrrw.Cells.Add(tblhdrcl);

        tbl_output_table.Controls.Add(tblhdrrw);

        bool selected = false;

        while (rdr.Read())
        {
            if (!selected)
            {
                drpdwnlst_view_certain_Company.SelectedIndex = arraylist_company_domain_name.IndexOf(rdr.GetString(rdr.GetOrdinal("domain_name")));
                selected = true;
            }

            TableRow tblrw = new TableRow();

            TableCell tblcl = new TableCell();
            tblcl.Controls.Add(new LiteralControl(rdr.GetString(rdr.GetOrdinal("domain_name"))));
            tblrw.Cells.Add(tblcl);

            tblcl = new TableCell();
            tblcl.Controls.Add(new LiteralControl(rdr.GetString(rdr.GetOrdinal("name"))));
            tblrw.Cells.Add(tblcl);

            tblcl = new TableCell();
            tblcl.Controls.Add(new LiteralControl(rdr.GetString(rdr.GetOrdinal("address"))));
            tblrw.Cells.Add(tblcl);

            tblcl = new TableCell();
            tblcl.Controls.Add(new LiteralControl(rdr.GetString(rdr.GetOrdinal("email"))));
            tblrw.Cells.Add(tblcl);

            tblcl = new TableCell();
            tblcl.Controls.Add(new LiteralControl(rdr.GetString(rdr.GetOrdinal("vision"))));
            tblrw.Cells.Add(tblcl);

            tblcl = new TableCell();
            tblcl.Controls.Add(new LiteralControl(rdr.GetString(rdr.GetOrdinal("type"))));
            tblrw.Cells.Add(tblcl);

            tblcl = new TableCell();
            tblcl.Controls.Add(new LiteralControl(rdr.GetString(rdr.GetOrdinal("field_of_specialization"))));
            tblrw.Cells.Add(tblcl);

            tblcl = new TableCell();

            SqlConnection conn2 = new SqlConnection(connStr);

            SqlCommand cmd2 = new SqlCommand("view_Phone_Numbers", conn2);
            cmd2.CommandType = CommandType.StoredProcedure;

            cmd2.Parameters.Add("@company_domain", rdr.GetString(rdr.GetOrdinal("domain_name")));

            SqlParameter output_message2 = cmd2.Parameters.Add("@output_message", SqlDbType.Int);
            output_message2.Direction = ParameterDirection.Output;

            conn2.Open();
            SqlDataReader rdr2 = cmd2.ExecuteReader(CommandBehavior.CloseConnection);

            while (rdr2.Read())
            {
                tblcl.Controls.Add(new LiteralControl(rdr2.GetString(rdr2.GetOrdinal("phone_number")) + "<br />"));
            }

            tblrw.Cells.Add(tblcl);

            rdr2.Close();

            tbl_output_table.Controls.Add(tblrw);
        }

        rdr.Close();

        if (!output_message.Value.ToString().Equals("4"))
            tbl_output_table.Rows.Clear();
        if (output_message.Value.ToString().Equals("0"))
            Response.Write("Invalid Input(s)");
        else if (output_message.Value.ToString().Equals("1"))
            Response.Write("Search Box Is Empty!");
        else if (output_message.Value.ToString().Equals("2"))
            Response.Write("NULL Input(s)");
        else if (output_message.Value.ToString().Equals("3"))
            Response.Write("No Companies With That Search Found!");
    }
    protected void View_Company_Type(object sender, EventArgs e)
    {
        string connStr = ConfigurationManager.ConnectionStrings["iWorkDBConn"].ToString();
        SqlConnection conn = new SqlConnection(connStr);

        SqlCommand cmd = new SqlCommand("view_all_companies", conn);
        cmd.CommandType = CommandType.StoredProcedure;

        SqlParameter output_message = cmd.Parameters.Add("@output_message", SqlDbType.Int);
        output_message.Direction = ParameterDirection.Output;

        conn.Open();
        SqlDataReader rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection);

        tbl_output_table.Rows.Clear();

        TableHeaderRow tblhdrrw = new TableHeaderRow();

        TableHeaderCell tblhdrcl = new TableHeaderCell();
        tblhdrcl.Controls.Add(new LiteralControl("Company's Name"));
        tblhdrrw.Cells.Add(tblhdrcl);

        tblhdrcl = new TableHeaderCell();
        tblhdrcl.Controls.Add(new LiteralControl("Company's Type"));
        tblhdrrw.Cells.Add(tblhdrcl);

        tbl_output_table.Rows.Add(tblhdrrw);

        while (rdr.Read())
        {
            TableRow tblrw = new TableRow();

            TableCell tblcl = new TableCell();
            tblcl.Controls.Add(new LiteralControl(rdr.GetString(rdr.GetOrdinal("name"))));
            tblrw.Cells.Add(tblcl);

            tblcl = new TableCell();
            tblcl.Controls.Add(new LiteralControl(rdr.GetString(rdr.GetOrdinal("type"))));
            tblrw.Cells.Add(tblcl);

            tbl_output_table.Rows.Add(tblrw);
        }

        rdr.Close();

        if (!output_message.Value.ToString().Equals("2"))
            tbl_output_table.Rows.Clear();
        if (output_message.Value.ToString().Equals("0"))
            Response.Write("Something Went Wrong. Please Contact The Website Admins Immediately!");
        else if (output_message.Value.ToString().Equals("1"))
            Response.Write("No Companies Saved On The Website Yet!");
    }
    protected void View_Certain_Company(object sender, EventArgs e)
    {
        string connStr = ConfigurationManager.ConnectionStrings["iWorkDBConn"].ToString();
        SqlConnection conn = new SqlConnection(connStr);

        SqlCommand cmd = new SqlCommand("view_certain_Company_Department", conn);
        cmd.CommandType = CommandType.StoredProcedure;

        string domain_name = arraylist_company_domain_name[drpdwnlst_view_certain_Company.SelectedIndex].ToString();

        cmd.Parameters.AddWithValue("@domain_name", domain_name);

        SqlParameter output_message = cmd.Parameters.Add("@output_message", SqlDbType.Int);
        output_message.Direction = ParameterDirection.Output;

        conn.Open();
        SqlDataReader rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection);

        tbl_output_table.Rows.Clear();

        TableHeaderRow tblhdrrw = new TableHeaderRow();

        TableHeaderCell tblhdrcl = new TableHeaderCell();
        tblhdrcl.Controls.Add(new LiteralControl("Company's Domain Name"));
        tblhdrrw.Cells.Add(tblhdrcl);

        tblhdrcl = new TableHeaderCell();
        tblhdrcl.Controls.Add(new LiteralControl("Company's Name"));
        tblhdrrw.Cells.Add(tblhdrcl);

        tblhdrcl = new TableHeaderCell();
        tblhdrcl.Controls.Add(new LiteralControl("Company's Address"));
        tblhdrrw.Cells.Add(tblhdrcl);

        tblhdrcl = new TableHeaderCell();
        tblhdrcl.Controls.Add(new LiteralControl("Company's Email"));
        tblhdrrw.Cells.Add(tblhdrcl);

        tblhdrcl = new TableHeaderCell();
        tblhdrcl.Controls.Add(new LiteralControl("Company's Vision"));
        tblhdrrw.Cells.Add(tblhdrcl);

        tblhdrcl = new TableHeaderCell();
        tblhdrcl.Controls.Add(new LiteralControl("Company's Type"));
        tblhdrrw.Cells.Add(tblhdrcl);

        tblhdrcl = new TableHeaderCell();
        tblhdrcl.Controls.Add(new LiteralControl("Company's Specialization"));
        tblhdrrw.Cells.Add(tblhdrcl);

        tblhdrcl = new TableHeaderCell();
        tblhdrcl.Controls.Add(new LiteralControl("Company's Phone Number(s)"));
        tblhdrrw.Cells.Add(tblhdrcl);

        tblhdrcl = new TableHeaderCell();
        tblhdrcl.Controls.Add(new LiteralControl("Department(s)' Name"));
        tblhdrrw.Cells.Add(tblhdrcl);

        tbl_output_table.Controls.Add(tblhdrrw);

        while (rdr.Read())
        {
            TableRow tblrw = new TableRow();

            TableCell tblcl = new TableCell();
            tblcl.Controls.Add(new LiteralControl(rdr.GetString(rdr.GetOrdinal("domain_name"))));
            tblrw.Cells.Add(tblcl);

            tblcl = new TableCell();
            tblcl.Controls.Add(new LiteralControl(rdr.GetString(rdr.GetOrdinal("name"))));
            tblrw.Cells.Add(tblcl);

            tblcl = new TableCell();
            tblcl.Controls.Add(new LiteralControl(rdr.GetString(rdr.GetOrdinal("address"))));
            tblrw.Cells.Add(tblcl);

            tblcl = new TableCell();
            tblcl.Controls.Add(new LiteralControl(rdr.GetString(rdr.GetOrdinal("email"))));
            tblrw.Cells.Add(tblcl);

            tblcl = new TableCell();
            tblcl.Controls.Add(new LiteralControl(rdr.GetString(rdr.GetOrdinal("vision"))));
            tblrw.Cells.Add(tblcl);

            tblcl = new TableCell();
            tblcl.Controls.Add(new LiteralControl(rdr.GetString(rdr.GetOrdinal("type"))));
            tblrw.Cells.Add(tblcl);

            tblcl = new TableCell();
            tblcl.Controls.Add(new LiteralControl(rdr.GetString(rdr.GetOrdinal("field_of_specialization"))));
            tblrw.Cells.Add(tblcl);

            tblcl = new TableCell();

            SqlConnection conn2 = new SqlConnection(connStr);

            SqlCommand cmd2 = new SqlCommand("view_Phone_Numbers", conn2);
            cmd2.CommandType = CommandType.StoredProcedure;

            cmd2.Parameters.Add("@company_domain", rdr.GetString(rdr.GetOrdinal("domain_name")));

            SqlParameter output_message2 = cmd2.Parameters.Add("@output_message", SqlDbType.Int);
            output_message2.Direction = ParameterDirection.Output;

            conn2.Open();
            SqlDataReader rdr2 = cmd2.ExecuteReader(CommandBehavior.CloseConnection);

            while (rdr2.Read())
            {
                tblcl.Controls.Add(new LiteralControl(rdr2.GetString(rdr2.GetOrdinal("phone_number")) + "<br />"));
            }

            tblrw.Cells.Add(tblcl);

            rdr2.Close();

            tblcl = new TableCell();
            tblcl.Controls.Add(new LiteralControl(rdr.GetString(rdr.GetOrdinal("dep_name")) + "<br />" + "<br />"));
            while (rdr.Read())
                tblcl.Controls.Add(new LiteralControl(rdr.GetString(rdr.GetOrdinal("dep_name")) + "<br />" + "<br />"));
            tblrw.Cells.Add(tblcl);

            tbl_output_table.Controls.Add(tblrw);
        }

        rdr.Close();

        if (!output_message.Value.ToString().Equals("4"))
            tbl_output_table.Rows.Clear();
        if (output_message.Value.ToString().Equals("0") || output_message.Value.ToString().Equals("2"))
            Response.Write("Something Went Wrong. Please Contact The Website Admins Immediately!");
        else if (output_message.Value.ToString().Equals("1"))
            Response.Write("No Companies Saved On The Website Yet!");
        else if (output_message.Value.ToString().Equals("3"))
            Response.Write("This Company Has No Departments!");
    }
    protected void View_Certain_Company_Department(object sender, EventArgs e)
    {
        string connStr = ConfigurationManager.ConnectionStrings["iWorkDBConn"].ToString();
        SqlConnection conn = new SqlConnection(connStr);

        SqlCommand cmd = new SqlCommand("view_certain_Company_Department_Job", conn);
        cmd.CommandType = CommandType.StoredProcedure;

        string domain_name = arraylist_company_domain_name[drpdwnlst_view_certain_department_company_company.SelectedIndex].ToString();
        string dep_code = arraylist_department_code[drpdwnlst_view_certain_department_company_department.SelectedIndex].ToString();

        cmd.Parameters.AddWithValue("@company_domain", domain_name);
        cmd.Parameters.AddWithValue("@dep_code", dep_code);

        SqlParameter output_message = cmd.Parameters.Add("@output_message", SqlDbType.Int);
        output_message.Direction = ParameterDirection.Output;

        conn.Open();
        SqlDataReader rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection);

        tbl_output_table.Rows.Clear();

        TableHeaderRow tblhdrrw = new TableHeaderRow();

        TableHeaderCell tblhdrcl = new TableHeaderCell();
        tblhdrcl.Controls.Add(new LiteralControl("Company's Name"));
        tblhdrrw.Cells.Add(tblhdrcl);

        tblhdrcl = new TableHeaderCell();
        tblhdrcl.Controls.Add(new LiteralControl("Department's Name"));
        tblhdrrw.Cells.Add(tblhdrcl);

        tblhdrcl = new TableHeaderCell();
        tblhdrcl.Controls.Add(new LiteralControl("Department's Code"));
        tblhdrrw.Cells.Add(tblhdrcl);

        tblhdrcl = new TableHeaderCell();
        tblhdrcl.Controls.Add(new LiteralControl("Company's Domain"));
        tblhdrrw.Cells.Add(tblhdrcl);

        tblhdrcl = new TableHeaderCell();
        tblhdrcl.Controls.Add(new LiteralControl("Available Job(s)' Title"));
        tblhdrrw.Cells.Add(tblhdrcl);

        tbl_output_table.Controls.Add(tblhdrrw);

        while (rdr.Read())
        {
            TableRow tblrw = new TableRow();

            TableCell tblcl = new TableCell();
            tblcl.Controls.Add(new LiteralControl(rdr.GetString(rdr.GetOrdinal("name"))));
            tblrw.Cells.Add(tblcl);

            tblcl = new TableCell();
            tblcl.Controls.Add(new LiteralControl(rdr.GetString(rdr.GetOrdinal("dep_name"))));
            tblrw.Cells.Add(tblcl);

            tblcl = new TableCell();
            tblcl.Controls.Add(new LiteralControl(rdr.GetString(rdr.GetOrdinal("code"))));
            tblrw.Cells.Add(tblcl);

            tblcl = new TableCell();
            tblcl.Controls.Add(new LiteralControl(rdr.GetString(rdr.GetOrdinal("company_domain_name"))));
            tblrw.Cells.Add(tblcl);

            tblcl = new TableCell();
            tblcl.Controls.Add(new LiteralControl(rdr.GetString(rdr.GetOrdinal("title")) + "<br />"));
            while (rdr.Read())
                tblcl.Controls.Add(new LiteralControl(rdr.GetString(rdr.GetOrdinal("title")) + "<br />"));
            tblrw.Cells.Add(tblcl);

            tbl_output_table.Controls.Add(tblrw);
        }

        rdr.Close();

        if (!output_message.Value.ToString().Equals("7"))
            tbl_output_table.Rows.Clear();
        if (output_message.Value.ToString().Equals("0") || output_message.Value.ToString().Equals("1") || output_message.Value.ToString().Equals("2"))
            Response.Write("Something Went Wrong. Please Contact The Website Admins Immediately!");
        else if (output_message.Value.ToString().Equals("3"))
            Response.Write("Company Was Not Found!");
        else if (output_message.Value.ToString().Equals("4"))
            Response.Write("Department Was not Found!");
        else if (output_message.Value.ToString().Equals("5"))
            Response.Write("This Department Is Not Part Of This Company!");
        else if (output_message.Value.ToString().Equals("6"))
            Response.Write("This Department Has No Available Jobs!");
    }
    protected void Search_For_Job_With_Vacancies_Keyword(object sender, EventArgs e)
    {
        string connStr = ConfigurationManager.ConnectionStrings["iWorkDBConn"].ToString();
        SqlConnection conn = new SqlConnection(connStr);

        SqlCommand cmd = new SqlCommand("search_for_Job_with_Vacancies_by_Keyword", conn);
        cmd.CommandType = CommandType.StoredProcedure;

        string keyword = txtbox_search_for_job_vacancies_keyword.Text;

        cmd.Parameters.AddWithValue("@keyword", keyword);

        SqlParameter output_message = cmd.Parameters.Add("@output_message", SqlDbType.Int);
        output_message.Direction = ParameterDirection.Output;

        conn.Open();
        SqlDataReader rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection);

        TableHeaderRow tblhdrrw = new TableHeaderRow();

        TableHeaderCell tblhdrcl = new TableHeaderCell();
        tblhdrcl.Controls.Add(new LiteralControl("Company's Name"));
        tblhdrrw.Cells.Add(tblhdrcl);

        tblhdrcl = new TableHeaderCell();
        tblhdrcl.Controls.Add(new LiteralControl("Department's Name"));
        tblhdrrw.Cells.Add(tblhdrcl);

        tblhdrcl = new TableHeaderCell();
        tblhdrcl.Controls.Add(new LiteralControl("Job's Title"));
        tblhdrrw.Cells.Add(tblhdrcl);

        tblhdrcl = new TableHeaderCell();
        tblhdrcl.Controls.Add(new LiteralControl("Job's Detailed Description"));
        tblhdrrw.Cells.Add(tblhdrcl);

        tblhdrcl = new TableHeaderCell();
        tblhdrcl.Controls.Add(new LiteralControl("Job's Short Description"));
        tblhdrrw.Cells.Add(tblhdrcl);

        tblhdrcl = new TableHeaderCell();
        tblhdrcl.Controls.Add(new LiteralControl("Job's Minimum Years Of Experience"));
        tblhdrrw.Cells.Add(tblhdrcl);

        tblhdrcl = new TableHeaderCell();
        tblhdrcl.Controls.Add(new LiteralControl("Job's Salary"));
        tblhdrrw.Cells.Add(tblhdrcl);

        tblhdrcl = new TableHeaderCell();
        tblhdrcl.Controls.Add(new LiteralControl("Job's Application Deadline"));
        tblhdrrw.Cells.Add(tblhdrcl);

        tblhdrcl = new TableHeaderCell();
        tblhdrcl.Controls.Add(new LiteralControl("Job's Work Hours"));
        tblhdrrw.Cells.Add(tblhdrcl);

        tblhdrcl = new TableHeaderCell();
        tblhdrcl.Controls.Add(new LiteralControl("Job's Number Of Vacancies"));
        tblhdrrw.Cells.Add(tblhdrcl);

        tbl_output_table.Controls.Add(tblhdrrw);

        while (rdr.Read())
        {
            TableRow tblrw = new TableRow();

            TableCell tblcl = new TableCell();
            tblcl.Controls.Add(new LiteralControl(rdr.GetString(rdr.GetOrdinal("name"))));
            tblrw.Cells.Add(tblcl);

            tblcl = new TableCell();
            tblcl.Controls.Add(new LiteralControl(rdr.GetString(rdr.GetOrdinal("dep_name"))));
            tblrw.Cells.Add(tblcl);

            tblcl = new TableCell();
            tblcl.Controls.Add(new LiteralControl(rdr.GetString(rdr.GetOrdinal("title"))));
            tblrw.Cells.Add(tblcl);

            tblcl = new TableCell();
            tblcl.Controls.Add(new LiteralControl(rdr.GetString(rdr.GetOrdinal("detailed_description"))));
            tblrw.Cells.Add(tblcl);

            tblcl = new TableCell();
            tblcl.Controls.Add(new LiteralControl(rdr.GetString(rdr.GetOrdinal("short_description"))));
            tblrw.Cells.Add(tblcl);

            tblcl = new TableCell();
            tblcl.Controls.Add(new LiteralControl(rdr.GetValue(rdr.GetOrdinal("minimum_years_of_experience")).ToString()));
            tblrw.Cells.Add(tblcl);

            tblcl = new TableCell();
            tblcl.Controls.Add(new LiteralControl(rdr.GetValue(rdr.GetOrdinal("salary")).ToString()));
            tblrw.Cells.Add(tblcl);

            tblcl = new TableCell();
            tblcl.Controls.Add(new LiteralControl(rdr.GetValue(rdr.GetOrdinal("application_deadline")).ToString()));
            tblrw.Cells.Add(tblcl);

            tblcl = new TableCell();
            tblcl.Controls.Add(new LiteralControl(rdr.GetValue(rdr.GetOrdinal("work_hours")).ToString()));
            tblrw.Cells.Add(tblcl);

            tblcl = new TableCell();
            tblcl.Controls.Add(new LiteralControl(rdr.GetValue(rdr.GetOrdinal("number_of_vacancies")).ToString()));
            tblrw.Cells.Add(tblcl);

            tbl_output_table.Controls.Add(tblrw);
        }

        rdr.Close();

        if (!output_message.Value.ToString().Equals("4"))
            tbl_output_table.Rows.Clear();
        if (output_message.Value.ToString().Equals("0"))
            Response.Write("Something Went Wrong. Please Contact The Website Admins Immediately!");
        else if (output_message.Value.ToString().Equals("1"))
            Response.Write("Search Box Is Empty!");
        else if (output_message.Value.ToString().Equals("2"))
            Response.Write("NULL Input(s)!");
        else if (output_message.Value.ToString().Equals("3"))
            Response.Write("No Job Found With Such Keyword(s)!");
    }
    protected void View_Companies_With_Average_Salary_DESC(object sender, EventArgs e)
    {
        string connStr = ConfigurationManager.ConnectionStrings["iWorkDBConn"].ToString();
        SqlConnection conn = new SqlConnection(connStr);

        SqlCommand cmd = new SqlCommand("view_Company_by_highest_Average_Salary", conn);
        cmd.CommandType = CommandType.StoredProcedure;

        SqlParameter output_message = cmd.Parameters.Add("@output_message", SqlDbType.Int);
        output_message.Direction = ParameterDirection.Output;

        conn.Open();
        SqlDataReader rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection);

        TableHeaderRow tblhdrrw = new TableHeaderRow();

        TableHeaderCell tblhdrcl = new TableHeaderCell();
        tblhdrcl.Controls.Add(new LiteralControl("Company's Name"));
        tblhdrrw.Cells.Add(tblhdrcl);

        tblhdrcl = new TableHeaderCell();
        tblhdrcl.Controls.Add(new LiteralControl("Company's Average Salary"));
        tblhdrrw.Cells.Add(tblhdrcl);

        tblhdrrw.Cells.Add(tblhdrcl);

        tbl_output_table.Controls.Add(tblhdrrw);

        while (rdr.Read())
        {
            TableRow tblrw = new TableRow();

            TableCell tblcl = new TableCell();
            tblcl.Controls.Add(new LiteralControl(rdr.GetString(rdr.GetOrdinal("name"))));
            tblrw.Cells.Add(tblcl);

            tblcl = new TableCell();
            tblcl.Controls.Add(new LiteralControl(rdr.GetValue(rdr.GetOrdinal("AVG")).ToString()));
            tblrw.Cells.Add(tblcl);

            tbl_output_table.Controls.Add(tblrw);
        }

        rdr.Close();

        if (!output_message.Value.ToString().Equals("2"))
            tbl_output_table.Rows.Clear();
        if (output_message.Value.ToString().Equals("0"))
            Response.Write("Something Went Wrong. Please Contact The Website Admins Immediately!");
        else if (output_message.Value.ToString().Equals("1"))
            Response.Write("No Companies Saved On The Website Yet!");
    }
}