using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.IO;
using System.Web.UI.WebControls;

namespace assignment
{
    public partial class login_page : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btn_login_Click(object sender, EventArgs e)
        {
            string username = Request.Form["username"];
            string password = Request.Form["password"];
            try
            {
                string filename = "C:\\Users\\FinIQ\\Downloads\\repos\\assignment\\logs.txt";
                FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read);
                StreamReader sr = new StreamReader(fs);
                sr.BaseStream.Seek(0, SeekOrigin.Begin);
                string str = sr.ReadLine();
                while (str != null)
                {
                    int index = str.IndexOf(",");
                    string actual_username = str.Substring(0, index);
                    string actual_password = str.Substring(index + 1);
                    if (actual_username == username)
                    {
                        if (actual_password == password)
                        {
                            Response.Write("Login Successful!");
                            Response.Redirect("color-change.aspx");
                        }
                    }
                    str = sr.ReadLine();
                }

                sr.Close();
                fs.Close();
            }
            catch (FileNotFoundException)
            {
                Response.Write("log file not found. Please check the file path in the code.");
            }
            catch (DirectoryNotFoundException)
            {
                Response.Write("log file not found. Please check the file path in the code.");
            }

        }

        protected void btn_register_Click(object sender, EventArgs e)
        {
            Response.Redirect("reg_page.aspx");
        }
    }
}