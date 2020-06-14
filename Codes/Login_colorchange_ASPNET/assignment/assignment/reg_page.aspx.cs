using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Web.UI;
using System.Web.UI.WebControls;
namespace assignment
{
    /*
    public class jsonData
    {
        public string Name { get; set; }
        public string College  { get; set; }
        public string Address  { get; set; }
        public string Username  { get; set; }
        public string Password  { get; set; }
    }*/
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
            /*
            List<jsonData> _data = new List<jsonData>();
            _data.Add(new jsonData()
            {
                Name = name,
                College = college,
                Address = address,
                Username = username,
                Password = password
            });
             * */
            try
            {
                string filename = "C:\\Users\\FinIQ\\Downloads\\repos\\assignment\\logs.txt";
                FileStream fs = new FileStream(filename, FileMode.Append, FileAccess.Write);
                StreamWriter sw = new StreamWriter(fs);
                string record = username + "," + password;
                sw.WriteLine(record);


                sw.Flush();
                sw.Close();
                fs.Close();

                Response.Redirect("login_page.aspx");
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
    }
}