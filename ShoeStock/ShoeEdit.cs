using ShoeStock.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ShoeStock
{
    public partial class ShoeEdit : Form
    {
        string oldPath = "", filePath = "", fileName = "";
        Shoe shoe;
        public ShoeEdit()
        {
            InitializeComponent();
        }
        public int EditId { get; set; }
        public MasterForm MasterForm { get; set; }

        private void ShoeEdit_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.MasterForm.ShoeUpdated(shoe);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (this.openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                if (this.openFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    this.filePath = this.openFileDialog1.FileName;
                    this.label4.Text = Path.GetFileName(this.filePath);
                    this.pictureBox1.Image = Image.FromFile(this.filePath);
                }
            }
        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {

        }

        private void ShoeEdit_Load(object sender, EventArgs e)
        {
            this.textBox1.Text = this.EditId.ToString();
            using(SqlConnection con = new SqlConnection(DbConnectionUtil.ConString))
            {
                using (SqlCommand cmd = new SqlCommand("SELECT * FROM Shoes WHERE ShoeId=@i", con))
                {
                    cmd.Parameters.AddWithValue("@i", this.EditId);
                    con.Open();
                    SqlDataReader dr = cmd.ExecuteReader();
                    if (dr.Read())
                    {
                        textBox2.Text = dr.GetString(dr.GetOrdinal("Model"));
                        checkBox1.Checked = dr.GetBoolean(dr.GetOrdinal("Active"));
                        dateTimePicker1.Value = dr.GetDateTime(dr.GetOrdinal("FirstIntroducedOn"));
                        oldPath = dr.GetString(dr.GetOrdinal("Picture"));
                        pictureBox1.Image = Image.FromFile(Path.Combine(@"..\..\Pictures", dr.GetString(dr.GetOrdinal("Picture"))));
                        
                    }
                    con.Close();
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            using (SqlConnection con = new SqlConnection(DbConnectionUtil.ConString))
            {
                con.Open();
                using(SqlTransaction tran = con.BeginTransaction())
                {
                    using (SqlCommand cmd = new SqlCommand(@"UPDATE Shoes SET 
                                    Model=@m, FirstIntroducedOn=@f, Active=@a, Picture=@p
                                    WHERE ShoeId=@i", con, tran))
                    {
                        cmd.Parameters.AddWithValue("@i", int.Parse(textBox1.Text));
                        cmd.Parameters.AddWithValue("@m", textBox2.Text);
                        cmd.Parameters.AddWithValue("@f", dateTimePicker1.Value);
                        cmd.Parameters.AddWithValue("@a", checkBox1.Checked);
                        if (!string.IsNullOrEmpty(this.filePath))
                        {
                            string ext = Path.GetExtension(this.filePath);
                            fileName = $"{Guid.NewGuid()}{ext}";
                            string savePath = Path.Combine(Path.GetFullPath(@"..\..\Pictures"), fileName);
                            File.Copy(filePath, savePath, true);
                            cmd.Parameters.AddWithValue("@p", fileName);
                        }
                        else
                        {
                            cmd.Parameters.AddWithValue("@p", oldPath);
                        }
                        try
                        {
                            if (cmd.ExecuteNonQuery() > 0)
                            {
                                MessageBox.Show("Data Saved", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                                tran.Commit();
                                shoe = new Shoe { 
                                ShoeId = EditId,
                                Model = textBox2.Text,
                                FirstIntroducedOn= dateTimePicker1.Value,
                                Active = checkBox1.Checked,
                                Picture = oldPath == "" ? fileName: oldPath
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
    }
}
