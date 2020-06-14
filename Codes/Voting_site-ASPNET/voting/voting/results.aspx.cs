using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace voting
{
    public partial class results : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["loginauth"].ToString() == "1"/* && Session["ID"] == Session.SessionID*/)
            {
                double yes_count = Convert.ToDouble(Application["yes_count"]);
                double no_count = Convert.ToDouble(Application["no_count"]);
                double cant_say_count = Convert.ToDouble(Application["cant_say_count"]);
                double total = yes_count + no_count + cant_say_count;
                double yes_percentage = (yes_count / total) * 100;
                double no_percentage = (no_count / total) * 100;
                double cant_say_percentage = (cant_say_count / total) * 100;
                if (double.IsNaN(yes_percentage))
                    yes_percentage = 0.0;
                if (double.IsNaN(no_percentage))
                    no_percentage = 0.0;
                if (double.IsNaN(cant_say_percentage))
                    cant_say_percentage = 0.0;
                yes_percentage = Math.Round(yes_percentage, 2);
                no_percentage = Math.Round(no_percentage, 2);
                cant_say_percentage = Math.Round(cant_say_percentage, 2);
                
                label_yes_count.Text = Convert.ToString(yes_percentage) + "%";

                label_no_count.Text = Convert.ToString(no_percentage) + "%";

                label_cantsay_count.Text = Convert.ToString(cant_say_percentage) + "%";
            }
            else
            {
                Response.Redirect("login_page.aspx");
            }
        }

        protected void btn_logout_click(object sender, EventArgs e)
        {
            Session["loginauth"] = "0";
            HttpResponse.RemoveOutputCacheItem("/caching/results.aspx");
            HttpResponse.RemoveOutputCacheItem("/caching/voting.aspx");
            HttpResponse.RemoveOutputCacheItem("/caching/login_page.aspx");
            Response.Redirect("login_page.aspx");
            
        }
    }
}