using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.IO;
using System.Web.UI.WebControls;

namespace voting
{
    public partial class login_page : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Session["loginauth"] = "0";
        }

        protected void btn_login_Click(object sender, EventArgs e)
        {
            string username = Request.Form["textBox_username"];
            string password = Request.Form["textBox_password"];
            string filename = "C:\\Users\\FinIQ\\Downloads\\repos\\voting\\logs.txt";
            //FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read);
            Boolean isLogin = false;
            Boolean username_state = false;
            try
            {
                StreamReader sr = new StreamReader(filename);
                sr.BaseStream.Seek(0, SeekOrigin.Begin);
                string str = sr.ReadLine();
                while (str != null)
                {
                    int index = str.IndexOf(",");
                    char comma = ',';
                    string[] record_lists = str.Split(comma);
                    string actual_username = record_lists[0];
                    string actual_password = record_lists[1];
                    if (actual_username == username)
                    {
                        if (actual_password == password)
                        {
                            Response.Write("Login Successful!");
                            //System.Threading.Thread.Sleep(2000);
                            sr.Close();
                            Session["loginauth"] = "1";
                            //Session["ID"] = Session.SessionID;
                            Response.Redirect("voting.aspx?Username=" + username);
                            isLogin = true;
                        }
                        else
                        {
                            Response.Write("Wrong Password!");
                            username_state = true;
                            //System.Threading.Thread.Sleep(1000);
                            //Server.Transfer("login_page.aspx");
                        }
                    }
                    str = sr.ReadLine();
                }

                sr.Close();
            }
            catch (FileNotFoundException)
            {
                Response.Write("log file not found. Please check the file path in the code.");
            }
            catch (DirectoryNotFoundException)
            {
                Response.Write("log file not found. Please check the file path in the code.");
            }
            catch (Exception exp)
            {
                Response.Write("Exception: " + exp.Message);
                System.Threading.Thread.Sleep(2000);
            }
            if (!isLogin && !username_state)
            {
                Response.Write("Wrong Username! Try again.");
            }
        }

        protected void btn_register_Click(object sender, EventArgs e)
        {
            Response.Redirect("reg_page.aspx");
        }
    }
}