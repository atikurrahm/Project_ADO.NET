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
    public partial class StockEdit : Form
    {
        Stock stock;
        public StockEdit()
        {
            InitializeComponent();
        }
        public MasterForm MasterForm { get; set; }
        public int EditId { get; set; }
        private void StockEdit_Load(object sender, EventArgs e)
        {
            this.textBox1.Text = this.EditId.ToString();
            LoadCombo();
            using (SqlConnection con = new SqlConnection(DbConnectionUtil.ConString))
            {
                using (SqlCommand cmd = new SqlCommand("SELECT * FROM Stocks WHERE StockId=@i", con))
                {
                    cmd.Parameters.AddWithValue("@i", this.EditId);
                    con.Open();
                    SqlDataReader dr = cmd.ExecuteReader();
                    if (dr.Read())
                    {
                        textBox2.Text = dr.GetString(dr.GetOrdinal("Size"));
                        textBox3.Text = dr.GetDecimal(dr.GetOrdinal("Price")).ToString("0.00");
                        textBox4.Text = dr.GetInt32(dr.GetOrdinal("StockQuantity")).ToString();
                        comboBox1.SelectedValue = dr.GetInt32(dr.GetOrdinal("ShoeId"));

                    }
                    con.Close();
                }
            }
        }

        private void LoadCombo()
        {
            using (SqlConnection con = new SqlConnection(DbConnectionUtil.ConString))
            {
                using (SqlDataAdapter da = new SqlDataAdapter("SELECT ShoeId, Model FROM Shoes", con))
                {
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    this.comboBox1.DataSource = dt.DefaultView;
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
                    using (SqlCommand cmd = new SqlCommand(@"UPDATE Stocks SET 
                                    Size=@s, Price=@p, StockQuantity=@q, ShoeId=@st
                                    WHERE StockId=@i", con, tran))
                    {
                        cmd.Parameters.AddWithValue("@i", int.Parse(textBox1.Text));
                        cmd.Parameters.AddWithValue("@s", textBox2.Text);
                        cmd.Parameters.AddWithValue("@p", decimal.Parse(textBox3.Text));
                        cmd.Parameters.AddWithValue("@q", int.Parse(textBox4.Text));
                        cmd.Parameters.AddWithValue("@st", (int)comboBox1.SelectedValue);

                        try
                        {
                            if (cmd.ExecuteNonQuery() > 0)
                            {
                                MessageBox.Show("Data Saved", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                                tran.Commit();
                                stock = new Stock
                                {
                                    StockId = EditId,
                                    Size = textBox2.Text,
                                    Price = decimal.Parse(textBox3.Text),
                                    StockQuantity = int.Parse(textBox4.Text),
                                    ShoeId = (int)comboBox1.SelectedValue
                                };



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

        private void StockEdit_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (stock != null)
                this.MasterForm.StockUpdated(stock);
        }
    }
}