using System;
using System.Windows.Forms;

namespace StokTakip_Modern.Formlar
{
    public partial class frmAnaForm : Form
    {
        public string kullanici_adi;

        public frmAnaForm(string kullanici_adi)
        {
            InitializeComponent();

            this.kullanici_adi = kullanici_adi;
           

            FormYukle(new frmDashboard());
        }

        private void btnDashboard_Click(object sender, EventArgs e)
        {
            FormYukle(new frmDashboard());
        }

        private void FormYukle(Form form)
        {
            pnlIcerik.Controls.Clear();

            form.TopLevel = false;
            form.FormBorderStyle = FormBorderStyle.None;
            form.Dock = DockStyle.Fill;

            pnlIcerik.Controls.Add(form);
            pnlIcerik.Tag = form;

            form.Show();
        }

        private void foxButton8_Click(object sender, EventArgs e)
        {

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void foxButton1_Click(object sender, EventArgs e)
        {
            FormYukle(new frmDashboard());


        }

        private void foxButton2_Click(object sender, EventArgs e)
        {
            FormYukle(new frmUrunIslemleri());


        }

        private void labelEdit3_Click(object sender, EventArgs e)
        {

        }

        private void labelEdit4_Click(object sender, EventArgs e)
        {

        }

        private void foxButton3_Click(object sender, EventArgs e)
        {
            FormYukle(new frmMarkaIslemleri());
        }

        private void foxButton4_Click(object sender, EventArgs e)
        {
            FormYukle(new frmKategoriIslemleri());

        }

        private void foxButton5_Click(object sender, EventArgs e)
        {
            FormYukle(new frmDepoTanimlari());
        }

        private void foxButton6_Click(object sender, EventArgs e)
        {
            FormYukle(new frmRafTanimlari());
        }

        private void foxButton10_Click(object sender, EventArgs e)
        {
            FormYukle(new frmKullaniciVeYetki());
        }

        private void foxButton8_Click_1(object sender, EventArgs e)
        {
            FormYukle(new frmStokOperasyonları());
        }

        private void frmAnaForm_Load(object sender, EventArgs e)
        {

        }

        private void pictureBox6_Click(object sender, EventArgs e)
        {

        }
    }
}