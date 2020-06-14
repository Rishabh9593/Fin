using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Drawing;
using System.Web.UI.WebControls;

namespace assignment
{
    public partial class color_change : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void select_color_SelectedIndexChanged(object sender, EventArgs e)
        {
            Color c = Color.FromName(select_color_list.SelectedValue);
            label_withcolor.ForeColor = c;
            label_withcolor.Text = "Text in " + c.ToString();
        }
    }
}