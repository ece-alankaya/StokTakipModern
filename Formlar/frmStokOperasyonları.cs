using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace StokTakip_Modern.Formlar
{
    public partial class frmStokOperasyonları : Form
    {
        public frmStokOperasyonları()
        {
            InitializeComponent();
        }

        private void FormYukle(Form form)
        {
            pnlStok.Controls.Clear();

            form.TopLevel = false;
            form.FormBorderStyle = FormBorderStyle.None;
            form.Dock = DockStyle.Fill;

            pnlStok.Controls.Add(form);
            form.Show();
        }
        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void hopeButton1_Click(object sender, EventArgs e)
        {

        }

       

        private void button2_Click(object sender, EventArgs e)
        {
            FormYukle(new frmSevkiyatYonetimi());
        }

        private void button3_Click(object sender, EventArgs e)
        {
            FormYukle(new frmTedarikci());
        }

        private void hopeButton1_Click_1(object sender, EventArgs e)
        {
            FormYukle(new frmStokHareketleri());
        }

        private void hopeButton2_Click(object sender, EventArgs e)
        {
            FormYukle(new frmSevkiyatYonetimi());

        }

        private void hopeButton3_Click(object sender, EventArgs e)
        {
           
            FormYukle(new frmTedarikci());

        }

       
    }

    
}
