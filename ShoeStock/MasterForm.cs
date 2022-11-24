using ShoeStock.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ShoeStock
{
    public partial class MasterForm : Form
    {
        DataSet ds;
        BindingSource 
            bsShoes = new BindingSource(),
            bsStocks = new BindingSource();
        public MasterForm()
        {
            InitializeComponent();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void MasterForm_Load(object sender, EventArgs e)
        {
            FillDataset();
            BindFormControls();
        }

        private void BindFormControls()
        {
            bsShoes.DataSource = ds;
            bsShoes.DataMember = "Shoes";
            bsStocks.DataSource = bsShoes;
            bsStocks.DataMember = "FK_shooe_stock";
            this.dataGridView1.DataSource = bsStocks;
            label4.DataBindings.Add(new Binding("Text", bsShoes, "Model"));
            label5.DataBindings.Add(new Binding("Text", bsShoes, "FirstIntroducedOn"));
            label5.DataBindings["Text"].Format += (s, e) =>
            {
                e.Value = ((DateTime)e.Value).ToString("yyyy-MM-dd");
            };
            checkBox1.DataBindings.Add(new Binding("Checked", bsShoes, "Active"));
            pictureBox1.DataBindings.Add(new Binding("Image", bsShoes, "Image", true));
        }

        private void shoesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new ShoeAdd { MasterForm = this }.ShowDialog();
        }

        private void FillDataset()
        {
            ds = new DataSet();
            using (SqlConnection con = new SqlConnection(DbConnectionUtil.ConString))
            {
                using (SqlDataAdapter da = new SqlDataAdapter("SELECT * FROM Shoes", con))
                {
                    da.Fill(ds, "Shoes");
                    ds.Tables["Shoes"].Columns.Add(new DataColumn("Image", typeof(byte[])));
                    for (var i = 0; i < ds.Tables["Shoes"].Rows.Count; i++)
                    {
                        ds.Tables["Shoes"].Rows[i]["Image"] = File.ReadAllBytes(Path.Combine(@"..\..\Pictures", ds.Tables["Shoes"].Rows[i]["Picture"].ToString()));
                    }
                    da.SelectCommand.CommandText = "SELECT * FROM Stocks";
                    da.Fill(ds, "Stocks");
                    ds.Relations.Add(new DataRelation(
                            "FK_shooe_stock",
                            ds.Tables["Shoes"].Columns["ShoeId"],
                            ds.Tables["Stocks"].Columns["ShoeId"]
                        ));
                    ds.AcceptChanges();
                }
            }
        }

        private void stockToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new StockAdd { MasterForm = this }.ShowDialog();
        }

        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            this.bsShoes.MoveLast();
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            if(bsShoes.Position < bsShoes.Count -1)
                this.bsShoes.MoveNext();
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            if (bsShoes.Position > 0)
                this.bsShoes.MovePrevious();
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            this.bsShoes.MoveFirst();
        }

        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            //Debug.WriteLine(e.RowIndex);
            int id = (int)dataGridView1.Rows[e.RowIndex].Cells[0].Value;
            new StockEdit { MasterForm = this, EditId = id }.ShowDialog();
        }
        public void ShoeUpdated(Shoe s)
        {
            for(var i=0; i< ds.Tables["Shoes"].Rows.Count; i++)
            {
                if((int)ds.Tables["Shoes"].Rows[i]["ShoeId"]== s.ShoeId)
                {
                    ds.Tables["Shoes"].Rows[i]["Model"] = s.Model;
                    ds.Tables["Shoes"].Rows[i]["FirstIntroducedOn"] = s.FirstIntroducedOn;
                    ds.Tables["Shoes"].Rows[i]["Active"] = s.Active;
                    ds.Tables["Shoes"].Rows[i]["Picture"] = s.Picture;
                    ds.Tables["Shoes"].Rows[i]["Image"] = File.ReadAllBytes(Path.Combine(@"..\..\Pictures", s.Picture));
                    break;
                }
            }
            ds.AcceptChanges();
        }
        public void StockUpdated(Stock s)
        {
            for (var i = 0; i < ds.Tables["Stocks"].Rows.Count; i++)
            {
                if ((int)ds.Tables["Stocks"].Rows[i]["StockId"] == s.StockId)
                {
                    ds.Tables["Stocks"].Rows[i]["Size"] = s.Size;
                    ds.Tables["Stocks"].Rows[i]["Price"] = s.Price;
                    ds.Tables["Stocks"].Rows[i]["StockQuantity"] = s.StockQuantity;
                    ds.Tables["Stocks"].Rows[i]["ShoeId"] = s.ShoeId;
                    
                    break;
                }
            }
            ds.AcceptChanges();
        }
        public void ShoesAdded (List<Shoe> shoes)
        {
            foreach (var s in shoes)
            {
                DataRow dr = ds.Tables["Shoes"].NewRow();
                dr[0] = s.ShoeId; 
                dr["Model"] = s.Model;
                dr["FirstIntroducedOn"] = s.FirstIntroducedOn;
                dr["Active"] = s.Active;
                
                dr["Picture"] = s.Picture;
                dr["Image"] = File.ReadAllBytes(Path.Combine(Path.GetFullPath(@"..\..\Pictures"), s.Picture));
                ds.Tables["Shoes"].Rows.Add(dr);

            }
            ds.AcceptChanges();
            bsShoes.MoveLast();

            
        }

        private void toolStripButton5_Click(object sender, EventArgs e)
        {
            int id = (int)(bsShoes.Current as DataRowView).Row["ShoeId"];
            new ShoeEdit { MasterForm = this, EditId = id }.ShowDialog();
        }

        private void report1ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new ReportForm1().ShowDialog();
        }

        private void report2ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new ReportForm2().ShowDialog();
        }

        public void StocksAdded(List<Stock> stocks)
        {
            foreach (var s in stocks)
            {
                DataRow dr = ds.Tables["Stocks"].NewRow();
                dr[0] = s.StockId;
                dr["Size"] = s.Size;
                dr["Price"] = s.Price;
                dr["StockQuantity"] = s.StockQuantity;

                dr["ShoeId"] = s.ShoeId;
               
                ds.Tables["Stocks"].Rows.Add(dr);

            }
            ds.AcceptChanges();
            //bsShoes.MoveLast();


        }
    }
}
