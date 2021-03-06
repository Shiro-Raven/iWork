﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;

public partial class register : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {

    }
    protected void Submit(object sender, EventArgs e)
    {
        string connStr = ConfigurationManager.ConnectionStrings["iWorkDBConn"].ToString();
        SqlConnection conn = new SqlConnection(connStr);

        SqlCommand cmd = new SqlCommand("register_User", conn);
        cmd.CommandType = CommandType.StoredProcedure;

        if (txtbox_birth_date.Text.Equals("") || txtbox_password.Text.Equals("") || txtbox_first_name.Text.Equals("") || txtbox_middle_name.Text.Equals("") ||
            txtbox_last_name.Text.Equals("") || txtbox_birth_date.Text.Equals("") || txtbox_email.Text.Equals("") || txtbox_years_of_experience.Text.Equals(""))
        {
            Response.Write("Empty Input(s)");
            return;
        }

        string username = txtbox_username.Text;
        string password = txtbox_password.Text;
        string first_name = txtbox_first_name.Text;
        string middle_name = txtbox_middle_name.Text;
        string last_name = txtbox_last_name.Text;
        string birth_date = txtbox_birth_date.Text;
        string email = txtbox_email.Text;
        int yeears_of_experience = int.Parse(txtbox_years_of_experience.Text);

        cmd.Parameters.AddWithValue("@username", username);
        cmd.Parameters.AddWithValue("@password", password);
        cmd.Parameters.AddWithValue("@first_name", first_name);
        cmd.Parameters.AddWithValue("@middle_name", middle_name);
        cmd.Parameters.AddWithValue("@last_name", last_name);
        cmd.Parameters.AddWithValue("@birthdate", birth_date);
        cmd.Parameters.AddWithValue("@email", email);
        cmd.Parameters.AddWithValue("@years_of_experience", yeears_of_experience);

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
            Response.Write("Null Input(s)");
        else if (output_message.Value.ToString().Equals("3"))
            Response.Write("User Name Already Exists!");
        else if (output_message.Value.ToString().Equals("4"))
            Response.Write("Email Already Exists!"); 
        else if (output_message.Value.ToString().Equals("5"))
            Response.Write("Password Must Be At Least 8 Characters");
        else if (output_message.Value.ToString().Equals("6"))
            Response.Write("User Must Be At Least 18 years Old");
        else if (output_message.Value.ToString().Equals("7"))
            Response.Write("Years Of Experience Must Be At Least 0 (ZERO)!");
        else if (output_message.Value.ToString().Equals("8"))
        {
            Session["username"] = username;
            ScriptManager.RegisterStartupScript(this, this.GetType(),
            "alert",
            "alert('REGISTRATION SUCCESSFUL!');window.location ='logged_in.aspx';",
            true);
        }
    }
    protected void Back_To_Login(object sender, EventArgs e)
    {
        Response.Redirect("login", true);
    }
}