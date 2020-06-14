/* Author: Durgesh Pachghare
 * Last Edited: 21-06-2019       Pushed to master: YES
 * Description: This GUI program is simmple simulation of Some programs in FX trading. It contains FX Transaction Trading screen where you can order
 *              and Cross pair calculation, where anyone can calculate cross pair's rates. 3rd screen contains Premium Calculation by Black-Scholes Equation
 *              
 * Language Used: C#, Winform on .Net           IDE: Visual Studio Community 2019
 * Queries Contact: durgeshpachghare01@gmail.com
 */


using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net.Http.Headers;

namespace fx_app
{
    
    public partial class fx_app_form : Form
    {
        string api_key = "aeNuEkLSTi4sjwm1RvgGdNOv";
        //Some global Variables needed later
        private bool is_target1_reversed = false;
        private bool is_target2_reversed = false;
        private bool isPair1Solved = false;
        private bool is_Initial_amount_entered = false;

        //Dictionary of pairs and their rates, used previously before introducing API
        Dictionary<string, string> rate_list = new Dictionary<string, string>()
        {
            {"EUR-GBP","0.8857-0.8859"},
            {"EUR-AUD","1.6107-1.6108"},
            {"EUR-NZD","1.7040-1.7043"},
            {"EUR-USD","1.1254-1.1255"},
            {"EUR-INR","77.926-78.087"},
            {"GBP-AUD","1.8231-1.8235"},
            {"GBP-USD","1.2714-1.2715"},
            {"GBP-JPY","138.31-141.67"},
            {"GBP-INR","88.264-88.283"},
            {"USD-JPY","108.18-108.20"},
            {"USD-INR","69.340-69.370"},
            {"JPY-INR","0.6399-0.6412"},
        };

        //Dictionary for storing priority of Currencies, which is needed in some calculations further
        Dictionary<string, int> currency_priority = new Dictionary<string, int>()
        {
            {"EUR",0},
            {"GBP",1},
            {"AUD",2},
            {"NZD",3},
            {"USD",4},
            {"CAD",5},
            {"CHF",6},
            {"JPY",7},
            {"INR",8},
        };

        //Dictionary of balance amount which will get added later
        Dictionary<string, Double> balance = new Dictionary<string, Double>();
        public fx_app_form()
        {
            //Start the form
            InitializeComponent();
            button2_target_pair.Enabled = false;
            button2_target2_pair.Enabled = false;
            select4_time_to_maturity_unit.SelectedIndex = 2;
            textBox4_premium.Enabled = false;
            button4_calculate_volatility.Enabled = false;
        }

        //Show topmost panel where fx transactions takes place, sticking at the root of hierarchy (see the Document Tree)
        private void Btn_fx_transaction_Click(object sender, EventArgs e)
        {
            if (is_Initial_amount_entered)
            {
                panel_cross_pair_calc.Visible = false;
                panel_initial_amount.Visible = false;
                panel_calculate_premium.Visible = false;
            }
            else
            {
                panel_initial_amount.Visible = true;
                panel_cross_pair_calc.Visible = true;
                panel_calculate_premium.Visible = false;
            }
            btn_fx_transaction.BackColor = Color.DarkSlateGray;
            btn_calculate_premium.BackColor = System.Drawing.ColorTranslator.FromHtml("#2D2D30");
            btn_cross_pair_calc.BackColor = System.Drawing.ColorTranslator.FromHtml("#2D2D30");
        }
        private void select_buy_changed(object sender, EventArgs e)
        {
            label1_details.Text = "";
            if (select_cust_sell.SelectedIndex != -1)
            {
                calculate_rate();
            }
        }
        private void select_sell_changed(object sender, EventArgs e)
        {
            label1_details.Text = "";
            if (select_cust_buy.SelectedIndex != -1)
            {
                calculate_rate();
            }
        }
        //==================================API PART / FETCHING RATES AND STORING IN DICTIONARY=================================
        //-----------------------JSON Classes-------------------------------
        
        public class EffectiveParams
        {
            public string data_set { get; set; }
            public List<string> base_currencies { get; set; }
            public List<string> quote_currencies { get; set; }
        }

        public class Meta
        {
            public EffectiveParams effective_params { get; set; }
            public string endpoint { get; set; }
            public DateTime request_time { get; set; }
            public List<object> skipped_currency_pairs { get; set; }
        }

        public class Quote
        {
            public string base_currency { get; set; }
            public string quote_currency { get; set; }
            public string bid { get; set; }
            public string ask { get; set; }
            public string midpoint { get; set; }
        }

        public class RootObject
        {
            public Meta meta { get; set; }
            public List<Quote> quotes { get; set; }
        }
        //-----------------------------------------------------------------------------------------------------------------
        //-----------------------------API FETCH FUNCTIONS------------------------------------------------------------------
        public TResponse Process<TResponse>(string base_currency, string quote_currency)
        {
            // Execute Api call
            //https://www1.oanda.com/rates/api/v2/rates/spot.json?api_key=ggeAzjVhxieAUEI5pDdANQUK&base=USD&quote=INR
            string host = "https://www1.oanda.com/rates/";
            api_key = "<YOUR API KEY>";
            string api_link = "api/v2/rates/spot.json?api_key=";
            string api = api_link + api_key + "&base=" + base_currency + "&quote=" + quote_currency;
            var httpResponseMessage = MakeApiCall(host, api);

            // Process Json string result to fetch final deserialized model
            return FetchResult<TResponse>(httpResponseMessage);
        }

        public HttpResponseMessage MakeApiCall(string host, string api)
        {
            // Create HttpClient
            var client = new HttpClient(new HttpClientHandler { UseDefaultCredentials = true }) { BaseAddress = new Uri(host) };
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            // Make an API call and receive HttpResponseMessage
            HttpResponseMessage responseMessage = client.GetAsync(api, HttpCompletionOption.ResponseContentRead).GetAwaiter().GetResult();

            return responseMessage;
        }

        public T FetchResult<T>(HttpResponseMessage result)
        {
            if (result.IsSuccessStatusCode)
            {
                // Convert the HttpResponseMessage to string
                var resultArray = result.Content.ReadAsStringAsync().GetAwaiter().GetResult();

                // Json.Net Deserialization
                var final = JsonConvert.DeserializeObject<T>(resultArray);

                return final;
            }
            return default(T);
        }
        //------------------------------------------------------------------------------------------------------------------
        //-----------------------------------------OTHER CALCULATIONS--------------------------------------------------------

        private void pips_keyPress(object sender, KeyPressEventArgs e)
        {
            int n = (int)e.KeyChar;
            e.Handled = !(n == 8 || (n >= 48 && n <= 57));
        }
        //Calculate The Actual Rate after adding or subtracting pips
        private void pips_keyUp(object sender, KeyEventArgs e)
        {
            int selected_buy_index = select_cust_buy.SelectedIndex;
            int selected_sell_index = select_cust_sell.SelectedIndex;
            string selected_buy_currency = select_cust_buy.Text;
            string selected_sell_currency = select_cust_sell.Text;
            string pair;
            double pips_value = 0;
            if (selected_buy_index != -1 || selected_sell_index != -1)
            {
                try
                {
                    if (selected_buy_currency == "JPY" || selected_sell_currency == "JPY")
                    {
                        pips_value = Convert.ToDouble(textBox_pips.Text) / 100;
                    }
                    else
                    {
                        pips_value = Convert.ToDouble(textBox_pips.Text) / 10000;
                    }
                }
                catch (FormatException)
                {
                    //MessageBox.Show("Please enter numbers only. Other Characters are not allowed!", "Only Numbers Allowed!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                try
                {
                    if (selected_buy_index < selected_sell_index)
                    {
                        pair = selected_buy_currency + "-" + selected_sell_currency;
                        string current_rate = rate_list[pair];
                        label_cust_rate.Text = Convert.ToString(Convert.ToDouble(current_rate.Substring(7, 6)) + pips_value);
                    }
                    else
                    {
                        pair = selected_sell_currency + "-" + selected_buy_currency;
                        string current_rate = rate_list[pair];
                        label_cust_rate.Text = Convert.ToString(Convert.ToDouble(current_rate.Substring(0, 6)) - pips_value);
                    }
                }
                catch (KeyNotFoundException)
                {
                    MessageBox.Show("Rates not found anywhere. Check API settings. \n [Hint: API key may have been expired. Update the API key from OANDA.]", "Rates not found!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Please Select Both Currencies. We need both currency to fetch the rate.", "Select Both Currencies!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void amount_buy_keyPress(object sender, KeyPressEventArgs e)
        {
            int n = (int)e.KeyChar;
            e.Handled = !(n == 8 || (n >= 48 && n <= 57) || n == 46);
        }

        private void amount_buy_keyUp(object sender, KeyEventArgs e)
        {
            if (textBox_pips.Text != null)
            {
                double needed_amount = 0;
                double rate = 0;
                try
                {
                    rate = Convert.ToDouble(label_cust_rate.Text);
                    needed_amount = Convert.ToDouble(textBox_amoun_to_buy.Text);
                }
                catch (FormatException)
                {

                }
                int buy_index = select_cust_buy.SelectedIndex;
                int sell_index = select_cust_sell.SelectedIndex;
                double final_amount_needed;
                if (buy_index < sell_index)
                {
                    final_amount_needed = rate * needed_amount;
                }
                else
                {
                    final_amount_needed = needed_amount / rate;
                }
                label_amount_required_value.Text = Convert.ToString(final_amount_needed);
            }
            else
            {
                MessageBox.Show("Please Enter pips first to calculate final amount!", "Enter pips first!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void calculate_rate()
        {
            label1_details.Text = "Fetching Forex Rates from API [OANDA]";
            bool is_API_working = false;
            int selected_buy_index = select_cust_buy.SelectedIndex;
            int selected_sell_index = select_cust_sell.SelectedIndex;
            string selected_buy_currency = select_cust_buy.Text;
            string selected_sell_currency = select_cust_sell.Text;
            string pair;
            string current_rate = "";
            double bid = 0.0;
            double ask = 0.0;
            if (selected_buy_currency != selected_sell_currency)
            {
                if (selected_buy_index < selected_sell_index)
                {
                    pair = selected_buy_currency + "-" + selected_sell_currency;
                }
                else
                {
                    pair = selected_sell_currency + "-" + selected_buy_currency;
                }
                try
                {
                    //https://www1.oanda.com/rates/api/v2/rates/spot.json?api_key=ggeAzjVhxieAUEI5pDdANQUK&base=USD&quote=INR
                    var rootObj = Process<RootObject>(pair.Substring(0, 3), pair.Substring(4, 3));
                    foreach (var quote in rootObj.quotes)
                    {
                        bid = Convert.ToDouble(quote.bid);
                        ask = Convert.ToDouble(quote.ask);
                        string sBid = bid.ToString();
                        string sAsk = ask.ToString();
                        if (sBid.IndexOf('.') == 1 && sAsk.IndexOf('.') == 1)
                            current_rate = bid.ToString("F4") + "-" + ask.ToString("F4");
                        else if (sBid.IndexOf('.') == 2 && sAsk.IndexOf('.') == 2)
                            current_rate = bid.ToString("F3") + "-" + ask.ToString("F3");
                        else if (sBid.IndexOf('.') == 3 && sAsk.IndexOf('.') == 3)
                            current_rate = bid.ToString("F") + "-" + ask.ToString("F");
                        label_currency_pair.Text = pair;
                        string rate__1 = current_rate.Substring(0, 4);
                        string rate__2 = current_rate.Substring(4, 2);
                        string rate__3 = current_rate.Substring(6, 1);
                        string rate__4 = current_rate.Substring(7, 4);
                        string rate__5 = current_rate.Substring(11, 2);
                        label_rate_1.Text = rate__1;
                        label_rate_2.Text = rate__2;
                        label_rate_3.Text = rate__3;
                        label_rate_4.Text = rate__4;
                        label_rate_5.Text = rate__5;

                    }
                    label1_details.Text = "Rates Fetched from API...";
                    is_API_working = true;
                }
                catch (Exception)
                {
                    is_API_working = false;
                    MessageBox.Show("API didn't respond. Checking in Local Dictionary", "API not working!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                try
                {
                    current_rate = rate_list[pair];
                }
                catch (KeyNotFoundException)
                {
                    if(is_API_working == true)
                        rate_list.Add(pair, current_rate);
                }
                if (is_API_working == false)
                {
                    try
                    {
                        current_rate = rate_list[pair];
                        label1_details.Text = "API Didn't Respond...";
                        label_currency_pair.Text = pair;
                        string rate__1 = current_rate.Substring(0, 4);
                        string rate__2 = current_rate.Substring(4, 2);
                        string rate__3 = current_rate.Substring(6, 1);
                        string rate__4 = current_rate.Substring(7, 4);
                        string rate__5 = current_rate.Substring(11, 2);
                        label_rate_1.Text = rate__1;
                        label_rate_2.Text = rate__2;
                        label_rate_3.Text = rate__3;
                        label_rate_4.Text = rate__4;
                        label_rate_5.Text = rate__5;
                    }
                    catch (KeyNotFoundException)
                    {
                        MessageBox.Show("Rates not found anywhere. Check API settings.", "Rates not found!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                
            }
            else
            {
                MessageBox.Show("You cannot select same currency to buy and sell.", "Same currency Selected!", MessageBoxButtons.OK, MessageBoxIcon.Error);

            }

        }
        
        private void Button_order_Click(object sender, EventArgs e)
        {
            if ((select_cust_buy.SelectedIndex != -1) && (select_cust_sell.SelectedIndex != -1) && (textBox_pips.Text != null) && (textBox_amoun_to_buy.Text.Length > 0) && (textBox_amoun_to_buy.Text.Length > 0))
            {

                int buy_index = select_cust_buy.SelectedIndex;
                int sell_index = select_cust_sell.SelectedIndex;
                string currency_buy = select_cust_buy.Text;
                string currency_sell = select_cust_sell.Text;

                Boolean isOrderExecuted = false;

                double final_sell_amount = balance[currency_sell] - Convert.ToDouble(label_amount_required_value.Text);
                if (final_sell_amount < 0)
                {
                    isOrderExecuted = false;
                    MessageBox.Show("Cannot Place order. You don't have sufficient amount in your wallet! Please add amount to place order.", "Insufficient Amount!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    isOrderExecuted = true;
                    balance[currency_sell] = final_sell_amount;
                    double final_buy_amount = balance[currency_buy] + Convert.ToDouble(textBox_amoun_to_buy.Text);
                    balance[currency_buy] = final_buy_amount;
                }

                if (isOrderExecuted)
                {
                    //Dynamically updating the table according to order details
                    tableLayoutPanel_display_orders.Visible = false;

                    string count = Convert.ToString(tableLayoutPanel_display_orders.RowCount);
                    tableLayoutPanel_display_orders.RowCount = tableLayoutPanel_display_orders.RowCount + 1;
                    tableLayoutPanel_display_orders.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F));
                    int orderCount = tableLayoutPanel_display_orders.RowCount;

                    tableLayoutPanel_display_orders.Controls.Add(new Label() { Text = "After Order No. " + (orderCount / 3), Anchor = AnchorStyles.Right | AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Bottom, TextAlign = ContentAlignment.MiddleCenter, AutoSize = true, Font = new Font("Century Gothic", 14), ForeColor = System.Drawing.ColorTranslator.FromHtml("#50BFBF") }, 1, tableLayoutPanel_display_orders.RowCount - 1);

                    tableLayoutPanel_display_orders.RowCount = tableLayoutPanel_display_orders.RowCount + 1;
                    tableLayoutPanel_display_orders.RowStyles.Add(new RowStyle(SizeType.Absolute, 25F));
                    tableLayoutPanel_display_orders.Controls.Add(new Label() { Text = "Buying: " + currency_buy, BorderStyle = BorderStyle.FixedSingle, Anchor = AnchorStyles.Right | AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Bottom, TextAlign = ContentAlignment.MiddleCenter, AutoSize = true, Font = new Font("Century Gothic", 11), ForeColor = System.Drawing.ColorTranslator.FromHtml("#50BFBF") }, 0, tableLayoutPanel_display_orders.RowCount - 1);
                    tableLayoutPanel_display_orders.Controls.Add(new Label() { Text = "1234567899 ", BorderStyle = BorderStyle.FixedSingle, Anchor = AnchorStyles.Right | AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Bottom, TextAlign = ContentAlignment.MiddleCenter, AutoSize = true, Font = new Font("Century Gothic", 11), ForeColor = System.Drawing.ColorTranslator.FromHtml("#50BFBF") }, 1, tableLayoutPanel_display_orders.RowCount - 1);
                    tableLayoutPanel_display_orders.Controls.Add(new Label() { Text = balance[currency_buy].ToString("#,000.00"), BorderStyle = BorderStyle.FixedSingle, Anchor = AnchorStyles.Right | AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Bottom, TextAlign = ContentAlignment.MiddleCenter, AutoSize = true, Font = new Font("Century Gothic", 11), ForeColor = System.Drawing.ColorTranslator.FromHtml("#50BFBF") }, 2, tableLayoutPanel_display_orders.RowCount - 1);

                    tableLayoutPanel_display_orders.RowCount = tableLayoutPanel_display_orders.RowCount + 1;
                    tableLayoutPanel_display_orders.RowStyles.Add(new RowStyle(SizeType.Absolute, 25F));
                    tableLayoutPanel_display_orders.Controls.Add(new Label() { Text = "Selling: " + currency_sell, BorderStyle = BorderStyle.FixedSingle, Anchor = AnchorStyles.Right | AnchorStyles.Left | AnchorStyles.Top, TextAlign = ContentAlignment.MiddleCenter, AutoSize = true, Font = new Font("Century Gothic", 11), ForeColor = System.Drawing.ColorTranslator.FromHtml("#50BFBF") }, 0, tableLayoutPanel_display_orders.RowCount - 1);
                    tableLayoutPanel_display_orders.Controls.Add(new Label() { Text = "1234567899 ", BorderStyle = BorderStyle.FixedSingle, Anchor = AnchorStyles.Right | AnchorStyles.Left | AnchorStyles.Top, TextAlign = ContentAlignment.MiddleCenter, AutoSize = true, Font = new Font("Century Gothic", 11), ForeColor = System.Drawing.ColorTranslator.FromHtml("#50BFBF") }, 1, tableLayoutPanel_display_orders.RowCount - 1);
                    tableLayoutPanel_display_orders.Controls.Add(new Label() { Text = balance[currency_sell].ToString("#,000.00"), BorderStyle = BorderStyle.FixedSingle, Anchor = AnchorStyles.Right | AnchorStyles.Left | AnchorStyles.Top, TextAlign = ContentAlignment.MiddleCenter, AutoSize = true, Font = new Font("Century Gothic", 11), ForeColor = System.Drawing.ColorTranslator.FromHtml("#50BFBF") }, 2, tableLayoutPanel_display_orders.RowCount - 1);

                    tableLayoutPanel_display_orders.Visible = true;
                }
            }
            else
            {
                MessageBox.Show("Please fill all the above fields to place an order.", "Insufficient Data!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void button1_check_balance_Click(object sender, EventArgs e)
        {
            try
            {
                textBox3_curr1.Text = balance[label3_curr1.Text].ToString("#,000.00");
                textBox3_curr2.Text = balance[label3_curr2.Text].ToString("#,000.00");
                textBox3_curr3.Text = balance[label3_curr3.Text].ToString("#,000.00");
                textBox3_curr4.Text = balance[label3_curr4.Text].ToString("#,000.00");
                textBox3_curr5.Text = balance[label3_curr5.Text].ToString("#,000.00");
                textBox3_curr6.Text = balance[label3_curr6.Text].ToString("#,000.00");
                textBox3_curr7.Text = balance[label3_curr7.Text].ToString("#,000.00");
                textBox3_curr8.Text = balance[label3_curr8.Text].ToString("#,000.00");
                textBox3_curr9.Text = balance[label3_curr9.Text].ToString("#,000.00");
                panel_calculate_premium.Visible = false;
                panel_cross_pair_calc.Visible = true;
                panel_initial_amount.Visible = true;

            }
            catch (KeyNotFoundException)
            {
                MessageBox.Show("Error Occured while fetching from Dictionary.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        //==================================================================================================================================================================================
        //------------------------------------CROSS PAIR CALCULATIONS ONWARDS---------------------------------------------------------------------------------------------------------------
        //==================================================================================================================================================================================

        private void Select2_pair1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (select2_pair2.SelectedIndex != -1)
            {

                string pair1 = select2_pair1.Text;
                string pair2 = select2_pair2.Text;
                double pair1_bid = Convert.ToDouble(textBox2_pair1_bid.Text);
                double pair1_ask = Convert.ToDouble(textBox2_pair1_ask.Text);
                double pair2_bid = Convert.ToDouble(textBox2_pair2_bid.Text);
                double pair2_ask = Convert.ToDouble(textBox2_pair2_ask.Text);
                string answer = calculate_cross_pair(pair1, pair2, pair1_bid, pair1_ask, pair2_bid, pair2_ask);
                if (answer == "-1")
                {
                    MessageBox.Show("Both pairs must have one common currency.", "Wrong Pair selected.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    string[] currency_rate = answer.Split('$');
                    button2_target_pair.Text = currency_rate[0];
                    label2_target_pair_bid.Text = currency_rate[1];
                    label2_target_pair_ask.Text = currency_rate[2];
                }
            }
        }

        private void Btn_cross_pair_calc_Click(object sender, EventArgs e)
        {
            panel_cross_pair_calc.Visible = true;
            panel_calculate_premium.Visible = false;
            panel_initial_amount.Visible = false;
            btn_fx_transaction.BackColor = System.Drawing.ColorTranslator.FromHtml("#2D2D30");
            btn_calculate_premium.BackColor = System.Drawing.ColorTranslator.FromHtml("#2D2D30");
            btn_cross_pair_calc.BackColor = Color.DarkSlateGray;
        }

        private void Btn_add_pair_Click(object sender, EventArgs e)
        {
            if (isPair1Solved == true)
            {
                tableLayoutPanel_cross_pair_calc.Visible = false;
                select2_pair3.Visible = true;
                textBox2_pair3_bid.Visible = true;
                textBox2_pair3_ask.Visible = true;
                label2_target_pair_label.Visible = true;
                tableLayoutPanel_cross_pair_calc.Visible = true;
            }
            else
            {
                MessageBox.Show("1st pair must be solved first!", "Solve first pair", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void select2_pair2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if ((select2_pair1.SelectedIndex == -1) || (textBox2_pair1_bid.Text == "") || (textBox2_pair2_ask.Text == "") || (textBox2_pair1_bid.Text == "Enter Pair 1 Bid") || (textBox2_pair2_ask.Text == "Enter Pair 1 Ask"))
            {
                MessageBox.Show("Please Select Pair 1 and enter rates of pair 1.", "Select Pair 1 First", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                select2_pair2.SelectedIndex = -1;
            }
        }
        private string calculate_cross_pair(string pair1, string pair2, double pair1_bid, double pair1_ask, double pair2_bid, double pair2_ask)
        {
            string pair1_currency1 = pair1.Substring(0, 3);
            string pair1_currency2 = pair1.Substring(4, 3);
            string pair2_currency1 = pair2.Substring(0, 3);
            string pair2_currency2 = pair2.Substring(4, 3);

            int currency11_priority = currency_priority[pair1_currency1];
            int currency12_priority = currency_priority[pair1_currency2];
            int currency21_priority = currency_priority[pair2_currency1];
            int currency22_priority = currency_priority[pair2_currency2];

            int currency1_priority = 0;
            int currency2_priority = 0;

            string target_pair = "";
            double target_bid = 0;
            double target_ask = 0;

            int condition = -1;
            if (pair1_currency1 == pair2_currency1)
            {
                condition = 0;
                currency1_priority = currency12_priority;
                currency2_priority = currency22_priority;
            }
            else if (pair1_currency1 == pair2_currency2)
            {
                condition = 1;
                currency1_priority = currency12_priority;
                currency2_priority = currency21_priority;
            }
            else if (pair1_currency2 == pair2_currency1)
            {
                condition = 2;
                currency1_priority = currency11_priority;
                currency2_priority = currency22_priority;
            }
            else if (pair1_currency2 == pair2_currency2)
            {
                condition = 3;
                currency1_priority = currency11_priority;
                currency2_priority = currency21_priority;
            }
            else
                condition = -1;
            if (condition == 0 || condition == 3)
            {
                if (currency1_priority < currency2_priority)
                {
                    if (condition == 0)
                        target_pair = pair1_currency2 + "-" + pair2_currency2;
                    else
                        target_pair = pair1_currency1 + "-" + pair2_currency1;

                    target_bid = pair2_bid / pair1_ask;
                    target_ask = pair2_ask / pair1_bid;
                }
                else
                {
                    if (condition == 0)
                        target_pair = pair2_currency2 + "-" + pair1_currency2;
                    else
                        target_pair = pair2_currency1 + "-" + pair1_currency1;

                    target_bid = pair1_bid / pair2_ask;
                    target_ask = pair1_ask / pair2_bid;
                }
            }
            else if (condition == 1 || condition == 2)
            {
                if (currency1_priority < currency2_priority)
                {
                    if (condition == 1)
                        target_pair = pair1_currency2 + "-" + pair2_currency1;
                    else
                        target_pair = pair1_currency1 + "-" + pair2_currency2;
                }
                else
                {
                    if (condition == 1)
                        target_pair = pair2_currency1 + "-" + pair1_currency2;
                    else
                        target_pair = pair2_currency2 + "-" + pair1_currency1;
                }
                target_bid = pair1_bid * pair2_bid;
                target_ask = pair1_ask * pair2_ask;
            }
            else if (condition == -1)
            {
                return "-1";
            }
            target_bid = Math.Round(target_bid, 4);
            target_ask = Math.Round(target_ask, 4);
            return target_pair + "$" + target_bid.ToString() + "$" + target_ask.ToString();
        }

        private void fx_app_form_Load(object sender, EventArgs e)
        {

        }
        

        private void button1_Click(object sender, EventArgs e)
        {
            if (is_Initial_amount_entered == false)
            {
                try
                {
                    balance.Add(label3_curr1.Text, Convert.ToDouble(textBox3_curr1.Text));
                    balance.Add(label3_curr2.Text, Convert.ToDouble(textBox3_curr2.Text));
                    balance.Add(label3_curr3.Text, Convert.ToDouble(textBox3_curr3.Text));
                    balance.Add(label3_curr4.Text, Convert.ToDouble(textBox3_curr4.Text));
                    balance.Add(label3_curr5.Text, Convert.ToDouble(textBox3_curr5.Text));
                    balance.Add(label3_curr6.Text, Convert.ToDouble(textBox3_curr6.Text));
                    balance.Add(label3_curr7.Text, Convert.ToDouble(textBox3_curr7.Text));
                    balance.Add(label3_curr8.Text, Convert.ToDouble(textBox3_curr8.Text));
                    balance.Add(label3_curr9.Text, Convert.ToDouble(textBox3_curr9.Text));
                    label133.Text = "Balance in all currencies";
                }
                catch (Exception)
                {
                    MessageBox.Show("Enter Proper Amount!", "Invalid Input!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                is_Initial_amount_entered = true;
                left_panel.Visible = true;
            }
            panel_cross_pair_calc.Visible = false;
            panel_initial_amount.Visible = false;
            panel_calculate_premium.Visible = false;
        }

        private void textBox2_pair1_bid_Enter(object sender, EventArgs e)
        {
            if (select2_pair1.SelectedIndex == -1)
            {
                MessageBox.Show("Please Select Pair 1 First before proceeding.", "Select Pair 1 First", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                this.ActiveControl = select2_pair1;
                return;
            }
            textBox2_pair1_bid.Text = "";
        }

        private void textBox2_pair1_ask_Enter(object sender, EventArgs e)
        {
            if (select2_pair1.SelectedIndex == -1)
            {
                MessageBox.Show("Please Select Pair 1 First before proceeding.", "Select Pair 1 First", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                this.ActiveControl = select2_pair1;
                return;
            }
            textBox2_pair1_ask.Text = "";
        }

        private void textBox2_pair2_bid_Enter(object sender, EventArgs e)
        {
            if (select2_pair1.SelectedIndex == -1)
            {
                MessageBox.Show("Please Select Pair 1 First before proceeding.", "Select Pair 1 First", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                this.ActiveControl = select2_pair1;
                return;
            }
            else if (textBox2_pair1_bid.Text == "" || textBox2_pair1_bid.Text == "Enter Pair 1 Bid")
            {
                MessageBox.Show("Please Enter Rates of 1st pair first!", "Enter Rates of pair 1", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                this.ActiveControl = textBox2_pair1_bid;
                return;
            }
            else if (textBox2_pair1_ask.Text == "" || textBox2_pair1_ask.Text == "Enter Pair 1 Ask")
            {
                MessageBox.Show("Please Enter Rates of 1st pair first!", "Enter Rates of pair 1", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                this.ActiveControl = textBox2_pair1_bid;
                return;
            }
            else if (select2_pair2.SelectedIndex == -1)
            {
                MessageBox.Show("Select Pair 2 First", "Select Pair 2", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            textBox2_pair2_bid.Text = "";
        }
        private void textBox2_pair2_ask_Enter(object sender, EventArgs e)
        {
            if (select2_pair1.SelectedIndex == -1)
            {
                MessageBox.Show("Please Select Pair 1 First before proceeding.", "Select Pair 1 First", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                this.ActiveControl = select2_pair1;
                return;
            }
            else if (textBox2_pair1_bid.Text == "" || textBox2_pair1_bid.Text == "Enter Pair 1 Bid")
            {
                MessageBox.Show("Please Enter Rates of 1st pair first!", "Enter Rates of pair 1", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                this.ActiveControl = textBox2_pair1_bid;
                return;
            }
            else if (textBox2_pair1_ask.Text == "" || textBox2_pair1_ask.Text == "Enter Pair 1 Ask")
            {
                MessageBox.Show("Please Enter Rates of 1st pair first!", "Enter Rates of pair 1", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                this.ActiveControl = textBox2_pair1_bid;
                return;
            }
            else if (select2_pair2.SelectedIndex == -1)
            {
                MessageBox.Show("Select Pair 2 First", "Select Pair 2", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            textBox2_pair2_ask.Text = "";
        }

        private void textBox2_pair3_bid_Enter(object sender, EventArgs e)
        {
            if (select2_pair3.SelectedIndex == -1)
            {
                MessageBox.Show("Select Pair 3 First", "Select Pair 2", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            textBox2_pair3_bid.Text = "";
        }

        private void keyPress_OnlyDigit_validation(object sender, KeyPressEventArgs e)
        {
            int n = (int)e.KeyChar;
            e.Handled = !(n == 8 || (n >= 48 && n <= 57) || n == 46);
        }
        private void textBox2_pair3_ask_Enter(object sender, EventArgs e)
        {
            if (select2_pair3.SelectedIndex == -1)
            {
                MessageBox.Show("Select Pair 3 First", "Select Pair 2", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            textBox2_pair3_ask.Text = "";
        }
        
        private void TextBox2_pair2_bid_KeyUp(object sender, KeyEventArgs e)
        {
            if (textBox2_pair2_ask.Text != "" && textBox2_pair2_ask.Text != "Enter Pair 2 Ask")
            {
                string pair1 = select2_pair1.Text;
                string pair2 = select2_pair2.Text;
                double pair1_bid, pair1_ask, pair2_bid, pair2_ask;
                pair1_bid = pair1_ask = pair2_bid = pair2_ask = 0;
                try
                {
                    pair1_bid = Convert.ToDouble(textBox2_pair1_bid.Text);
                    pair1_ask = Convert.ToDouble(textBox2_pair1_ask.Text);
                    pair2_bid = Convert.ToDouble(textBox2_pair2_bid.Text);
                    pair2_ask = Convert.ToDouble(textBox2_pair2_ask.Text);
                }
                catch (FormatException)
                {
                    MessageBox.Show("Please Enters only Numbers in Bid/Ask field", "Wrong input!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                string answer = calculate_cross_pair(pair1, pair2, pair1_bid, pair1_ask, pair2_bid, pair2_ask);
                if (answer == "-1")
                {
                    MessageBox.Show("Both pairs must have one common currency.", "Wrong Pair selected.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    select2_pair2.SelectedIndex = -1;
                }
                else
                {
                    button2_target_pair.Enabled = true;
                    string[] currency_rate = answer.Split('$');
                    isPair1Solved = true;
                    button2_target_pair.Text = currency_rate[0];
                    label2_target_pair_bid.Text = currency_rate[1];
                    label2_target_pair_ask.Text = currency_rate[2];
                }
            }
        }

        private void TextBox2_pair2_ask_KeyUp(object sender, KeyEventArgs e)
        {
            if (textBox2_pair2_bid.Text != "" && textBox2_pair2_bid.Text != "Enter Pair 2 Bid")
            {
                string pair1 = select2_pair1.Text;
                string pair2 = select2_pair2.Text;
                double pair1_bid, pair1_ask, pair2_bid, pair2_ask;
                pair1_bid = pair1_ask = pair2_bid = pair2_ask = 0;
                try
                {
                    pair1_bid = Convert.ToDouble(textBox2_pair1_bid.Text);
                    pair1_ask = Convert.ToDouble(textBox2_pair1_ask.Text);
                    pair2_bid = Convert.ToDouble(textBox2_pair2_bid.Text);
                    pair2_ask = Convert.ToDouble(textBox2_pair2_ask.Text);
                }
                catch (FormatException)
                {
                    MessageBox.Show("Please Enters only Numbers in Bid/Ask field", "Wrong input!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                string answer = calculate_cross_pair(pair1, pair2, pair1_bid, pair1_ask, pair2_bid, pair2_ask);
                if (answer == "-1")
                {
                    MessageBox.Show("Both pairs must have one common currency.", "Wrong Pair selected.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    select2_pair2.SelectedIndex = -1;
                }
                else
                {
                    button2_target_pair.Enabled = true;
                    string[] currency_rate = answer.Split('$');
                    isPair1Solved = true;
                    button2_target_pair.Text = currency_rate[0];
                    label2_target_pair_bid.Text = currency_rate[1];
                    label2_target_pair_ask.Text = currency_rate[2];
                }
            }
        }

        private void select2_pair3_SelectedIndexChanged(object sender, EventArgs e)
        {
            if ((isPair1Solved == true) && (button2_target_pair.Text != "") && (label2_target_pair_bid.Text != "") && (label2_target_pair_ask.Text != ""))
            {

            }
            else
            {
                MessageBox.Show("1st pair must be solved first!", "Solve first pair", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void TextBox2_pair3_bid_KeyUp(object sender, KeyEventArgs e)
        {
            if (textBox2_pair3_ask.Text != "" && textBox2_pair3_ask.Text != "Enter Pair 3 Ask")
            {
                string pair1 = button2_target_pair.Text;
                string pair2 = select2_pair3.Text;
                double pair1_bid, pair1_ask, pair2_bid, pair2_ask;
                pair1_bid = pair1_ask = pair2_bid = pair2_ask = 0;
                try
                {
                    pair1_bid = Convert.ToDouble(label2_target_pair_bid.Text);
                    pair1_ask = Convert.ToDouble(label2_target_pair_ask.Text);
                    pair2_bid = Convert.ToDouble(textBox2_pair3_bid.Text);
                    pair2_ask = Convert.ToDouble(textBox2_pair3_ask.Text);
                }
                catch (FormatException)
                {
                    MessageBox.Show("Please Enters only Numbers in Bid/Ask field", "Wrong input!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                string answer = calculate_cross_pair(pair1, pair2, pair1_bid, pair1_ask, pair2_bid, pair2_ask);
                if (answer == "-1")
                {
                    MessageBox.Show("Both pairs must have one common currency.", "Wrong Pair selected.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    select2_pair2.SelectedIndex = -1;
                }
                else
                {
                    button2_target2_pair.Enabled = true;
                    string[] currency_rate = answer.Split('$');
                    button2_target2_pair.Text = currency_rate[0];
                    label2_target2_bid.Text = currency_rate[1];
                    label2_target2_ask.Text = currency_rate[2];
                }

            }
        }

        private void textBox2_pair3_ask_KeyUp(object sender, KeyEventArgs e)
        {
            if (textBox2_pair3_bid.Text != "" && textBox2_pair3_bid.Text != "Enter Pair 3 Bid")
            {
                string pair1 = button2_target_pair.Text;
                string pair2 = select2_pair3.Text;
                double pair1_bid, pair1_ask, pair2_bid, pair2_ask;
                pair1_bid = pair1_ask = pair2_bid = pair2_ask = 0;
                try
                {
                    pair1_bid = Convert.ToDouble(label2_target_pair_bid.Text);
                    pair1_ask = Convert.ToDouble(label2_target_pair_ask.Text);
                    pair2_bid = Convert.ToDouble(textBox2_pair3_bid.Text);
                    pair2_ask = Convert.ToDouble(textBox2_pair3_ask.Text);
                }
                catch (FormatException)
                {
                    MessageBox.Show("Please Enters only Numbers in Bid/Ask field", "Wrong input!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                string answer = calculate_cross_pair(pair1, pair2, pair1_bid, pair1_ask, pair2_bid, pair2_ask);
                if (answer == "-1")
                {
                    MessageBox.Show("Both pairs must have one common currency.", "Wrong Pair selected.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    select2_pair2.SelectedIndex = -1;
                }
                else
                {
                    button2_target2_pair.Enabled = true;
                    string[] currency_rate = answer.Split('$');
                    button2_target2_pair.Text = currency_rate[0];
                    label2_target2_bid.Text = currency_rate[1];
                    label2_target2_ask.Text = currency_rate[2];
                }
            }
        }

        private void button2_target_pair_Click(object sender, EventArgs e)
        {
            if (button2_target_pair.Text != "")
            {
                if (is_target1_reversed == true)
                    is_target1_reversed = false;
                else
                    is_target1_reversed = true;

                string pair = button2_target_pair.Text;
                pair = pair.Substring(4, 3) + "-" + pair.Substring(0, 3);
                button2_target_pair.Text = pair;
                double bid = Convert.ToDouble(label2_target_pair_bid.Text);
                double ask = Convert.ToDouble(label2_target_pair_ask.Text);
                double temp = bid;
                bid = 1 / ask;
                ask = 1 / temp;
                bid = Math.Round(bid, 4);
                ask = Math.Round(ask, 4);
                label2_target_pair_bid.Text = bid.ToString();
                label2_target_pair_ask.Text = ask.ToString();
            }
        }

        private void button2_target2_pair_Click(object sender, EventArgs e)
        {
            if (button2_target2_pair.Text != "")
            {
                if (is_target2_reversed == true)
                    is_target2_reversed = false;
                else
                    is_target2_reversed = true;

                string pair = button2_target2_pair.Text;
                pair = pair.Substring(4, 3) + "-" + pair.Substring(0, 3);
                button2_target2_pair.Text = pair;
                double bid = Convert.ToDouble(label2_target2_bid.Text);
                double ask = Convert.ToDouble(label2_target2_ask.Text);
                double temp = bid;
                bid = 1 / ask;
                ask = 1 / temp;
                bid = Math.Round(bid, 4);
                ask = Math.Round(ask, 4);
                label2_target2_bid.Text = bid.ToString();
                label2_target2_ask.Text = ask.ToString();
            }
        }

        private void button2_reset_Clicked(object sender, EventArgs e)
        {

            textBox2_pair1_ask.Text = "";
            textBox2_pair1_bid.Text = "";
            textBox2_pair2_bid.Text = "";
            textBox2_pair2_ask.Text = "";
            textBox2_pair3_bid.Text = "";
            textBox2_pair3_ask.Text = "";
            button2_target_pair.Text = "";
            button2_target2_pair.Text = "";
            button2_target_pair.Enabled = false;
            button2_target2_pair.Enabled = false;
            select2_pair3.Visible = false;
            label2_target_pair_label.Visible = false;
            textBox2_pair3_ask.Visible = false;
            textBox2_pair3_bid.Visible = false;
            label2_target_pair_ask.Text = "";
            label2_target_pair_bid.Text = "";
            label2_target2_bid.Text = "";
            label2_target2_ask.Text = "";
            select2_pair2.SelectedIndex = -1;
            select2_pair3.SelectedIndex = -1;
            select2_pair1.SelectedIndex = -1;
        }
        //------------------------for premium calculation----------------------------------
        public double CND(double X)
        {
            double L = 0.0;
            double K = 0.0;
            double dCND = 0.0;
            const double a1 = 0.31938153;
            const double a2 = -0.356563782;
            const double a3 = 1.781477937;
            const double a4 = -1.821255978;
            const double a5 = 1.330274429;
            L = Math.Abs(X);
            K = 1.0 / (1.0 + 0.2316419 * L);
            dCND = 1.0 - 1.0 / Math.Sqrt(2 * Convert.ToDouble(Math.PI.ToString())) * Math.Exp(-L * L / 2.0) * (a1 * K + a2 * K * K + a3 * Math.Pow(K, 3.0) + a4 * Math.Pow(K, 4.0) + a5 * Math.Pow(K, 5.0));
            if (X < 0)
            {
                return 1.0 - dCND;
            }
            else
            {
                return dCND;
            }
        }
        public double[] calculate_premium(Boolean flag_callput, double strike, double spot, double riskfree_rate, double volatality, double time_to_maturity)
        {
            double d1 = (Math.Log(spot / strike) + ((riskfree_rate + ((volatality * volatality) / 2)) * time_to_maturity)) / (volatality * Math.Sqrt(time_to_maturity));
            double d2 = d1 - (volatality * Math.Sqrt(time_to_maturity));
            double[] premium = new double[3];
            if (flag_callput)
            {
                premium[0] = (spot * CND(d1)) - (strike * Math.Exp(-riskfree_rate * time_to_maturity) * CND(d2));
            }
            else
            {
                premium[0] = (strike * Math.Exp(-riskfree_rate * time_to_maturity) * CND(-d2)) - (spot * CND(-d1));
            }
            premium[1] = d1;
            premium[2] = d2;
            return premium;
        }

        private void textBox4_strike_KeyUp(object sender, KeyEventArgs e)
        {
            call_to_calculate_premium();
        }

        private void textBox4_spot_KeyUp(object sender, KeyEventArgs e)
        {
            call_to_calculate_premium();
        }

        private void textBox4_riskfree_rate_KeyUp(object sender, KeyEventArgs e)
        {
            call_to_calculate_premium();
        }

        private void textBox4_volatility_KeyUp(object sender, KeyEventArgs e)
        {
            call_to_calculate_premium();
        }

        private void textBox4_time_to_maturity_KeyUp(object sender, KeyEventArgs e)
        {
            call_to_calculate_premium();
        }

        private void select4_callput_SelectedIndexChanged(object sender, EventArgs e)
        {
            call_to_calculate_premium();
        }
        private void select4_time_to_maturity_unit_SelectedIndexChanged(object sender, EventArgs e)
        {
            call_to_calculate_premium();
        }
        private void call_to_calculate_premium()
        {
            if (textBox4_strike.Text != "" && textBox4_spot.Text != "" && textBox4_riskfree_rate.Text != "" && textBox4_time_to_maturity.Text != "" && textBox4_volatility.Text != "" && select4_callput.SelectedIndex != -1 && checkBox4_calculate_volatility.Checked == false)
            {
                double strike = Convert.ToDouble(textBox4_strike.Text);
                double spot = Convert.ToDouble(textBox4_spot.Text);
                double riskfree_rate = Convert.ToDouble(textBox4_riskfree_rate.Text) / 100;
                double volatality = Convert.ToDouble(textBox4_volatility.Text) / 100;
                double time_to_maturity = 0.0;
                if (select4_time_to_maturity_unit.SelectedIndex == 0)
                    time_to_maturity = Convert.ToDouble(textBox4_time_to_maturity.Text) / 365;
                else if (select4_time_to_maturity_unit.SelectedIndex == 1)
                    time_to_maturity = Convert.ToDouble(textBox4_time_to_maturity.Text) / 12;
                else if (select4_time_to_maturity_unit.SelectedIndex == 2)
                    time_to_maturity = Convert.ToDouble(textBox4_time_to_maturity.Text);

                if (select4_callput.SelectedIndex == 0)
                {
                    double[] premium = calculate_premium(true, strike, spot, riskfree_rate, volatality, time_to_maturity);
                    premium[0] = Math.Round(premium[0], 5);
                    textBox4_premium.Text = Convert.ToString(premium[0]);
                }
                else if (select4_callput.SelectedIndex == 1)
                {
                    double[] premium = calculate_premium(false, strike, spot, riskfree_rate, volatality, time_to_maturity);
                    premium[0] = Math.Round(premium[0], 5);
                    textBox4_premium.Text = Convert.ToString(premium[0]);
                }
            }
        }

        private void btn_calculate_premium_Click(object sender, EventArgs e)
        {
            panel_cross_pair_calc.Visible = true;
            panel_initial_amount.Visible = true;
            panel_calculate_premium.Visible = true;
            btn_fx_transaction.BackColor = System.Drawing.ColorTranslator.FromHtml("#2D2D30");
            btn_calculate_premium.BackColor = Color.DarkSlateGray;
            btn_cross_pair_calc.BackColor = System.Drawing.ColorTranslator.FromHtml("#2D2D30");
        }

        private void checkBox4_calculate_volatility_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox4_calculate_volatility.Checked)
            {
                textBox4_premium.Enabled = true;
                textBox4_premium.BorderStyle = BorderStyle.Fixed3D;
                button4_calculate_volatility.Enabled = true;
                select4_callput.SelectedIndex = 0;
                select4_callput.Enabled = false;
                label16.Text = "Implied Volatility Calculations";
                textBox4_premium.Text = "";
            }
            else
            {
                textBox4_premium.Enabled = false;
                textBox4_premium.BorderStyle = BorderStyle.None;
                button4_calculate_volatility.Enabled = false;
                select4_callput.SelectedIndex = 0;
                select4_callput.Enabled = true;
                textBox4_premium.Text = "";
                label16.Text = "Premium Calculation with Black-Scholes Equation";
                call_to_calculate_premium();
            }
        }
        private double calculate_volatility(double call_premium, double strike, double spot, double riskfree_rate, double volatility, double time_to_maturity)
        {
            double difference = 0;
            do
            {
                double[] premium = calculate_premium(true, strike, spot, riskfree_rate, volatility, time_to_maturity);
                double call_premium1 = premium[0];
                double d1 = premium[1];
                double d2 = premium[2];
                difference = Math.Abs(call_premium1 - call_premium);
                if (difference < 0.001)
                    return volatility;
                double N1_d1 = Math.Exp((-d1 * d1) / 2) / Math.Sqrt(2 * Math.PI);
                double vega = spot * Math.Sqrt(time_to_maturity) * N1_d1;
                volatility = volatility - (difference / vega);
            } while (difference >= 0.001);
            return volatility;
        }

        private void button4_calculate_volatility_Click(object sender, EventArgs e)
        {
            if (textBox4_strike.Text != "" && textBox4_spot.Text != "" && textBox4_riskfree_rate.Text != "" && textBox4_time_to_maturity.Text != "" && textBox4_volatility.Text != "" && select4_callput.SelectedIndex == 0 && textBox4_premium.Text != "")
            {
                double strike = Convert.ToDouble(textBox4_strike.Text);
                double spot = Convert.ToDouble(textBox4_spot.Text);
                double riskfree_rate = Convert.ToDouble(textBox4_riskfree_rate.Text) / 100;
                double volatility = Convert.ToDouble(textBox4_volatility.Text) / 100;
                double time_to_maturity = 0.0;
                if (select4_time_to_maturity_unit.SelectedIndex == 0)
                    time_to_maturity = Convert.ToDouble(textBox4_time_to_maturity.Text) / 365;
                else if (select4_time_to_maturity_unit.SelectedIndex == 1)
                    time_to_maturity = Convert.ToDouble(textBox4_time_to_maturity.Text) / 12;
                else if (select4_time_to_maturity_unit.SelectedIndex == 2)
                    time_to_maturity = Convert.ToDouble(textBox4_time_to_maturity.Text);
                double call_premium = Convert.ToDouble(textBox4_premium.Text);
                double final_volatility = calculate_volatility(call_premium, strike, spot, riskfree_rate, volatility, time_to_maturity);
                if(Double.IsNaN(final_volatility))
                    MessageBox.Show("Check your inputs", "Wrong input!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                else
                    label4_volatility.Text = Math.Round(final_volatility, 6).ToString();
            }
        }
    }
}
