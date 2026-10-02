using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace StokTakip_Modern.Formlar
{
    public partial class frmMarkaIslemleri : Form
    {
        private int seciliMarkaId = 0;
        public frmMarkaIslemleri()
        {
            InitializeComponent();
        }

        private void Temizle()
        {
            textBox1.Clear();
            richTextBox1.Clear();

            comboBox1.SelectedIndex = 0;

            seciliMarkaId = 0;

            dataGridView1.ClearSelection();

            textBox1.Focus();
        }
        private void labelEdit3_Click(object sender, EventArgs e)
        {

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
                return;

            try
            {
                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];

                seciliMarkaId = Convert.ToInt32(row.Cells["MarkaId"].Value);

                textBox1.Text = row.Cells["MarkaAdi"].Value?.ToString();
                richTextBox1.Text = row.Cells["Aciklama"].Value?.ToString();
                comboBox1.Text = row.Cells["Durum"].Value?.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Marka seçilirken hata oluştu:\n" + ex.Message,
                    "Hata",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void foxButton1_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBox1.Text))
            {
                MessageBox.Show("Marka adı boş bırakılamaz.");
                textBox1.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(comboBox1.Text))
            {
                MessageBox.Show("Durum seçiniz.");
                comboBox1.Focus();
                return;
            }

            try
            {
                Sınıflar.SqlBaglanti db = new Sınıflar.SqlBaglanti();

                using (SqlConnection conn = db.Connection())
                {
                    string sorgu = @"
                INSERT INTO Marka
                (
                    MarkaAdi,
                    Aciklama,
                    AktifMi,
                    OlusturmaTarihi
                )
                VALUES
                (
                    @MarkaAdi,
                    @Aciklama,
                    @AktifMi,
                    GETDATE()
                )";

                    using (SqlCommand cmd =
                        new SqlCommand(sorgu, conn))
                    {
                        cmd.Parameters.AddWithValue(
                            "@MarkaAdi",
                            textBox1.Text.Trim());

                        cmd.Parameters.AddWithValue(
                            "@Aciklama",
                            richTextBox1.Text.Trim());

                        cmd.Parameters.AddWithValue(
                            "@AktifMi",
                            comboBox1.Text == "Aktif" ? 1 : 0);

                        cmd.ExecuteNonQuery();
                    }
                }

                MessageBox.Show(
                    "Marka başarıyla kaydedildi.",
                    "Başarılı",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                Temizle();
                MarkaListele();
            }
            catch (SqlException ex)
            {
                MessageBox.Show(
                    "Veritabanı hatası:\n\n" + ex.Message,
                    "Hata",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void frmMarkaIslemleri_Load(object sender, EventArgs e)
        {
           
            comboBox1.Items.Clear();
            comboBox1.Items.Add("Aktif");
            comboBox1.Items.Add("Pasif");

            comboBox1.SelectedIndex = 0;

            MarkaListele();
        
    }
        private void MarkaListele()
        {
            try
            {
                Sınıflar.SqlBaglanti db = new Sınıflar.SqlBaglanti();

                using (SqlConnection conn = db.Connection())
                {
                    string sorgu = @"
                SELECT
                    MarkaId,
                    MarkaAdi,
                    Aciklama,
                    CASE
                        WHEN AktifMi = 1 THEN 'Aktif'
                        ELSE 'Pasif'
                    END AS Durum,
                    OlusturmaTarihi
                FROM Marka
                ORDER BY MarkaId DESC";

                    using (SqlDataAdapter da =
                        new SqlDataAdapter(sorgu, conn))
                    {
                        DataTable dt = new DataTable();

                        da.Fill(dt);

                        dataGridView1.DataSource = dt;
                    }
                }

                dataGridView1.AutoSizeColumnsMode =
                    DataGridViewAutoSizeColumnsMode.Fill;

                dataGridView1.SelectionMode =
                    DataGridViewSelectionMode.FullRowSelect;

                dataGridView1.MultiSelect = false;

                dataGridView1.ReadOnly = true;

                dataGridView1.AllowUserToAddRows = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Markalar yüklenirken hata oluştu:\n\n" + ex.Message,
                    "Hata",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void foxButton2_Click(object sender, EventArgs e)
        {
            Temizle();
        }

        private void foxButton4_Click(object sender, EventArgs e)
        {
            if (seciliMarkaId == 0)
            {
                MessageBox.Show(
                    "Lütfen düzenlemek istediğiniz markayı seçin.");
                return;
            }

            if (string.IsNullOrWhiteSpace(textBox1.Text))
            {
                MessageBox.Show("Marka adı boş bırakılamaz.");
                textBox1.Focus();
                return;
            }

            try
            {
                Sınıflar.SqlBaglanti db = new Sınıflar.SqlBaglanti();

                using (SqlConnection conn = db.Connection())
                {
                    string sorgu = @"
                UPDATE Marka
                SET
                    MarkaAdi = @MarkaAdi,
                    Aciklama = @Aciklama,
                    AktifMi = @AktifMi
                WHERE MarkaId = @MarkaId";

                    using (SqlCommand cmd =
                        new SqlCommand(sorgu, conn))
                    {
                        cmd.Parameters.AddWithValue(
                            "@MarkaId",
                            seciliMarkaId);

                        cmd.Parameters.AddWithValue(
                            "@MarkaAdi",
                            textBox1.Text.Trim());

                        cmd.Parameters.AddWithValue(
                            "@Aciklama",
                            richTextBox1.Text.Trim());

                        cmd.Parameters.AddWithValue(
                            "@AktifMi",
                            comboBox1.Text == "Aktif" ? 1 : 0);

                        cmd.ExecuteNonQuery();
                    }
                }

                MessageBox.Show(
                    "Marka başarıyla güncellendi.",
                    "Başarılı",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                Temizle();
                MarkaListele();
            }
            catch (SqlException ex)
            {
                MessageBox.Show(
                    "Veritabanı hatası:\n\n" + ex.Message,
                    "Hata",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void foxButton5_Click(object sender, EventArgs e)
        {
            if (seciliMarkaId == 0)
            {
                MessageBox.Show(
                    "Lütfen silmek istediğiniz markayı seçin.",
                    "Uyarı",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                return;
            }

            DialogResult cevap = MessageBox.Show(
                "Seçili markayı silmek istediğinize emin misiniz?",
                "Marka Sil",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (cevap != DialogResult.Yes)
                return;

            try
            {
                Sınıflar.SqlBaglanti db = new Sınıflar.SqlBaglanti();

                using (SqlConnection conn = db.Connection())
                {
                    string sorgu = @"
                DELETE FROM Marka
                WHERE MarkaId = @MarkaId";

                    using (SqlCommand cmd = new SqlCommand(sorgu, conn))
                    {
                        cmd.Parameters.AddWithValue(
                            "@MarkaId",
                            seciliMarkaId);

                        cmd.ExecuteNonQuery();
                    }
                }

                MessageBox.Show(
                    "Marka başarıyla silindi.",
                    "Başarılı",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                seciliMarkaId = 0;

                Temizle();
                MarkaListele();
            }
            catch (SqlException ex)
            {
                MessageBox.Show(
                    "Marka silinemedi.\n\n" +
                    "Bu marka bir ürüne bağlı olabilir.\n\n" +
                    ex.Message,
                    "Hata",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Silme sırasında hata oluştu:\n\n" + ex.Message,
                    "Hata",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void foxButton3_Click(object sender, EventArgs e)
        {
            textBox1.Clear();
            richTextBox1.Clear();

            comboBox1.SelectedIndex = 0;

            seciliMarkaId = 0;

            dataGridView1.ClearSelection();

            textBox1.Focus();
        }
    }
}
