/* Author: Durgesh Pachghare
 * Last Edited: 18-06-2019       Pushed to master: NO
 * Description: This collection of pages is a simple website for giving vote. It includes voting for a question and showing results at the end. Session and user login 
 *              details are maintained. After logout, user cannot go back to user authanticated pages which requires log in. 
 *              
 * Language Used: ASP.net, .Net           IDE: Visual Studio Community 2019
 * Queries Contact: durgeshpachghare01@gmail.com
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;

namespace voting
{
    public partial class reg_page : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            
        }

        protected void register_Click(object sender, EventArgs e)
        {
            string name = Request.Form["name"];
            string college = Request.Form["college"];
            string address = Request.Form["address"];
            string username = Request.Form["username"];
            string password = Request.Form["password"];
            string filename = "C:\\Users\\FinIQ\\Downloads\\repos\\voting\\logs.txt";
            try
            {
                StreamReader sr = new StreamReader(filename);
                sr.BaseStream.Seek(0, SeekOrigin.Begin);
                string str = sr.ReadLine();
                bool isAlreadyRegistered = false;
                while (str != null)
                {
                    int index = str.IndexOf(",");
                    string actual_username = str.Substring(0, index);
                    if (username == actual_username)
                    {
                        Response.Write("This username is already registered. If it's yours, then go to login, othrwise try another username.");
                        isAlreadyRegistered = true;
                        break;
                    }
                    str = sr.ReadLine();
                }
                sr.Close();


                if (isAlreadyRegistered == false)
                {
                    FileStream fs = new FileStream(filename, FileMode.Append, FileAccess.Write);
                    StreamWriter sw = new StreamWriter(fs);
                    string record = username + "," + password;
                    sw.WriteLine(record);
                    sw.Flush();
                    sw.Close();
                    fs.Close();

                    Response.Redirect("login_page.aspx");
                }
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

        protected void login_page_Click(object sender, EventArgs e)
        {
            Response.Redirect("login_page.aspx");
        }
    }
}