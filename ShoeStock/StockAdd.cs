using ShoeStock.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ShoeStock
{
    public partial class StockAdd : Form
    {
        List<Stock> stocks = new List<Stock>();
        public StockAdd()
        {
            InitializeComponent();
        }

        public MasterForm MasterForm { get; set; }

        private void StockAdd_Load(object sender, EventArgs e)
        {
            SetNewId(textBox1);
            using(SqlConnection con = new SqlConnection(DbConnectionUtil.ConString))
            {
                using(SqlDataAdapter da = new SqlDataAdapter("SELECT ShoeId, Model FROM Shoes", con))
                {
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    this.comboBox1.DataSource = dt.DefaultView;
                }
            }
        }
        private void SetNewId(TextBox t)
        {
            using (SqlConnection con = new SqlConnection(DbConnectionUtil.ConString))
            {
                using (SqlCommand cmd = new SqlCommand("SELECT ISNULL(MAX(StockId), 0) FROM Stocks", con))
                {
                    con.Open();
                    int id = (int)cmd.ExecuteScalar();
                    con.Close();
                    t.Text = $"{id + 1}";
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            using (SqlConnection con = new SqlConnection(DbConnectionUtil.ConString))
            {
                con.Open();
                using (SqlTransaction tran = con.BeginTransaction())
                {

                    using (SqlCommand cmd = new SqlCommand(@"INSERT INTO Stocks 
                                            (StockId, Size, Price, StockQuantity, ShoeId) VALUES
                                            (@i, @s, @p, @q,@r)", con, tran))
                    {
                        cmd.Parameters.AddWithValue("@i", int.Parse(textBox1.Text));
                        cmd.Parameters.AddWithValue("@s", textBox2.Text);
                        cmd.Parameters.AddWithValue("@p", decimal.Parse(textBox3.Text));
                        cmd.Parameters.AddWithValue("@q", int.Parse(textBox4.Text));
                        cmd.Parameters.AddWithValue("@r", (int)comboBox1.SelectedValue);


                        try
                        {
                            if (cmd.ExecuteNonQuery() > 0)
                            {
                                MessageBox.Show("Data Saved", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                                tran.Commit();
                                stocks.Add(new Stock
                                {
                                    StockId = int.Parse(textBox1.Text),
                                    Size = textBox2.Text,
                                    Price = decimal.Parse(textBox3.Text),
                                    StockQuantity = int.Parse(textBox4.Text),
                                    ShoeId = (int)comboBox1.SelectedValue

                                });
                                SetNewId(textBox1);
                                textBox2.Clear();
                                textBox3.Clear();
                                textBox4.Clear();

                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"Error: {ex.Message}", "Success", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            tran.Rollback();
                        }
                        finally
                        {
                            if (con.State == ConnectionState.Open)
                            {
                                con.Close();
                            }
                        }

                    }
                }
            }
        }

        private void StockAdd_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.MasterForm.StocksAdded(stocks);
        }
    }
}
