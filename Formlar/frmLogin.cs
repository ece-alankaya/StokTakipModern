using StokTakip_Modern.Sınıflar;
using System;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace StokTakip_Modern.Formlar
{
    public partial class frmLogin : Form
    {
        // Ana forma aktarılacak kullanıcı adını tutan property
        public string GirisYapanKullanici { get; private set; }

        public frmLogin()
        {
            InitializeComponent();
            txtbxSifre.PasswordChar = '*';
        }

      

        private bool SifreKontrol(string kullanici_adi, string sifre)
        {
            SqlBaglanti db = new SqlBaglanti();

            using (SqlConnection conn = db.Connection())
            {
                string sql = @"SELECT COUNT(*)
                               FROM Kullanici
                               WHERE KullaniciAdi=@kadi
                               AND Sifre=@sifre
                               AND AktifMi=1";

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@kadi", kullanici_adi);
                cmd.Parameters.AddWithValue("@sifre", sifre);

                int sonuc = (int)cmd.ExecuteScalar();

                return sonuc > 0;
            }
        }

        private void btnGiris_Click(object sender, EventArgs e)
        {
            string kullanici_adi = txtbxKullaadi.Text.Trim();
            string sifre = txtbxSifre.Text;

            if (string.IsNullOrWhiteSpace(kullanici_adi) ||
                string.IsNullOrWhiteSpace(sifre))
            {
                MessageBox.Show("Lütfen kullanıcı adı ve şifrenizi giriniz.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (SifreKontrol(kullanici_adi, sifre))
            {
                //MessageBox.Show("Giriş başarılı.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Beni Hatırla Ayarları
                if (chxbxBeniHatirla.Checked)
                {
                    Properties.Settings.Default.KullaniciAdi = kullanici_adi;
                    Properties.Settings.Default.BeniHatirla = true;
                }
                else
                {
                    Properties.Settings.Default.KullaniciAdi = "";
                    Properties.Settings.Default.BeniHatirla = false;
                }

                Properties.Settings.Default.Save();

                // 1. Kullanıcı adını dışarı aktar
                GirisYapanKullanici = kullanici_adi;

                // 2. ReaLTaiizor butonlarında sorunu çözen ana kısım:
                // Önce DialogResult atıyoruz, hemen ardından formu Hide veya Close ediyoruz.
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else
            {
                MessageBox.Show("Kullanıcı adı veya şifre hatalı!", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void TestBaglanti()
        {
            try
            {
                using (SqlConnection conn = new SqlBaglanti().Connection())
                {
                    // Bağlantı başarılı ise yapılacaklar
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Veritabanı bağlantı hatası: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void hopeRichTextBox1_Click(object sender, EventArgs e)
        {
        }

        private void foxLinkLabel1_Click(object sender, EventArgs e)
        {
        }

        private void linkLabelEdit1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            MessageBox.Show("Şifre sıfırlama ekranı yakında.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void chxbxBeniHatirla_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void frmLogin_Load_1(object sender, EventArgs e)
        {
            TestBaglanti();

            if (Properties.Settings.Default.BeniHatirla)
            {
                txtbxKullaadi.Text = Properties.Settings.Default.KullaniciAdi;
                chxbxBeniHatirla.Checked = true;

                this.BeginInvoke((MethodInvoker)delegate
                {
                    txtbxSifre.Focus();
                });
            }
        }
    }
}