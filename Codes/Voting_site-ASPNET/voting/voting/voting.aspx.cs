using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace voting
{
    public partial class voting : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["loginauth"].ToString() == "1"/* && Session["ID"] == Session.SessionID*/)
            {
                string filename = "C:\\Users\\FinIQ\\Downloads\\repos\\voting\\logs.txt";
                //FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read);
                string username = Request.QueryString["Username"].ToString();
                Boolean isVoted = false;
                try
                {
                    StreamReader sr = new StreamReader(filename);
                    sr.BaseStream.Seek(0, SeekOrigin.Begin);
                    string str = sr.ReadLine();
                    int counter = 0;
                    while (str != null)
                    {
                        int index = str.IndexOf(",");
                        string actual_username = str.Substring(0, index);
                        if (username == actual_username)
                        {
                            if (str.IndexOf(",", str.Length - 2, 1) == str.Length - 2)
                            {
                                string answer = str.Substring(str.Length - 1, 1);
                                int ans = Convert.ToInt32(answer);
                                if (ans == 0)
                                    answer = "Yes";
                                else if (ans == 1)
                                    answer = "No";
                                else if (ans == 2)
                                    answer = "Can't Say!";
                                Response.Write("You have already voted for this question. Your answer is: " + answer);
                                isVoted = true;
                                Session["line"] = Convert.ToString(counter);
                                break;
                            }
                            else
                            {
                            }
                            Session["line"] = Convert.ToString(counter);
                            break;
                        }
                        counter++;
                        str = sr.ReadLine();
                    }
                    if (isVoted == false)
                    {
                        Session["voted"] = "0";
                    }
                    else
                    {
                        Session["voted"] = "1";
                        btn_submit.Text = "Go to Results";
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
                }
            }
            else
            {
                Response.Redirect("login_page.aspx");
            }
        }

        protected void btn_submit_Click(object sender, EventArgs e)
        {
            int isVoted = Convert.ToInt32(Session["voted"].ToString());
            string filename = "C:\\Users\\FinIQ\\Downloads\\repos\\voting\\logs.txt";
            if(isVoted == 0)
            {
                if (answer_list.SelectedIndex == -1)
                {
                    Response.Write("Please Select an answer!");
                }
                else
                {
                    int selected_index = answer_list.SelectedIndex;
                    int temp = -1;
                    switch (selected_index)
                    {
                        case 0: temp = Convert.ToInt16(Application["yes_count"]);
                            temp++;
                            Application["yes_count"] = temp;
                            break;
                        case 1: temp = Convert.ToInt16(Application["no_count"]);
                            temp++;
                            Application["no_count"] = temp;
                            break;
                        case 2: temp = Convert.ToInt16(Application["cant_say_count"]);
                            temp++;
                            Application["cant_say_count"] = temp;
                            break;
                    }
                    try
                    {
                        string username = Request.QueryString["username"].ToString();
                        int line = Convert.ToInt32(Session["line"].ToString());

                        string record = File.ReadLines(filename).Skip(line).Take(1).First();
                        record = record + "," + selected_index.ToString();

                        string[] arrLine = File.ReadAllLines(filename);
                        arrLine[line] = record;
                        File.WriteAllLines(filename, arrLine);
                        Response.Redirect("results.aspx");
                        //System.IO.File.WriteAllLines(filename,
                        //arrLine.Select(tb => (double.Parse(tb.Text)).ToString()));
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
            else
            {
                Response.Redirect("results.aspx");
            }
        }
    }
}