using StokTakip_Modern.Sınıflar;
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

namespace StokTakip_Modern.Formlar
{
    public partial class frmKategoriIslemleri : Form
    {
        private int seciliKategoriId = 0;
        public frmKategoriIslemleri()
        {
            InitializeComponent();
        }

        private void foxButton1_Click(object sender, EventArgs e)
        {

            if (string.IsNullOrWhiteSpace(textBox1.Text))
            {
                MessageBox.Show(
                    "Kategori adı boş bırakılamaz.",
                    "Uyarı",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                textBox1.Focus();
                return;
            }

            try
            {
                SqlBaglanti db = new SqlBaglanti();

                using (SqlConnection conn = db.Connection())
                {
                    string sorgu = @"
                INSERT INTO Kategori
                (
                    KategoriAdi,
                    Aciklama,
                    AktifMi,
                    OlusturmaTarihi
                )
                VALUES
                (
                    @KategoriAdi,
                    @Aciklama,
                    @AktifMi,
                    GETDATE()
                )";

                    using (SqlCommand cmd = new SqlCommand(sorgu, conn))
                    {
                        cmd.Parameters.AddWithValue(
                            "@KategoriAdi",
                            textBox1.Text.Trim());

                        cmd.Parameters.AddWithValue(
                            "@Aciklama",
                            string.IsNullOrWhiteSpace(richTextBox1.Text)
                                ? (object)DBNull.Value
                                : richTextBox1.Text.Trim());

                        cmd.Parameters.AddWithValue(
                            "@AktifMi",
                            comboBox1.Text == "Aktif" ? 1 : 0);

                        cmd.ExecuteNonQuery();
                    }
                }

                MessageBox.Show(
                    "Kategori başarıyla kaydedildi.",
                    "Başarılı",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                KategoriListele();
                TemizleKategori();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Kategori kaydedilirken hata oluştu:\n\n" + ex.Message,
                    "Hata",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void frmKategoriIslemleri_Load(object sender, EventArgs e)
        {
            comboBox1.Items.Clear();

            comboBox1.Items.Add("Aktif");
            comboBox1.Items.Add("Pasif");

            comboBox1.SelectedIndex = 0;

            KategoriListele();
        }

        private void KategoriListele()
        {
            try
            {
                SqlBaglanti db = new SqlBaglanti();

                using (SqlConnection conn = db.Connection())
                {
                    string sorgu = @"
    SELECT
        KategoriId,
        KategoriAdi,
        Aciklama,
        CASE
            WHEN AktifMi = 1 THEN 'Aktif'
            ELSE 'Pasif'
        END AS Durum,
        OlusturmaTarihi
    FROM Kategori
    ORDER BY KategoriId DESC";
                    using (SqlDataAdapter da = new SqlDataAdapter(sorgu, conn))
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
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Kategori listesi yüklenirken hata oluştu:\n\n" + ex.Message,
                    "Hata",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }
        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dataGridView1.CurrentRow == null)
                return;

            if (dataGridView1.CurrentRow.IsNewRow)
                return;

            try
            {
                DataGridViewRow row = dataGridView1.CurrentRow;

                seciliKategoriId = Convert.ToInt32(
                    row.Cells["KategoriId"].Value);

                textBox1.Text =
                    row.Cells["KategoriAdi"].Value?.ToString();

                richTextBox1.Text =
                    row.Cells["Aciklama"].Value?.ToString();

                comboBox1.Text =
                    row.Cells["Durum"].Value?.ToString();
            }
            catch
            {
                seciliKategoriId = 0;
            }
        }

        private void foxButton2_Click(object sender, EventArgs e)
        {
            TemizleKategori();
        }
        private void TemizleKategori()
        {
            textBox1.Clear();
            richTextBox1.Clear();

            comboBox1.SelectedIndex = 0;

            seciliKategoriId = 0;

            dataGridView1.ClearSelection();

            textBox1.Focus();
        }

        private void foxButton4_Click(object sender, EventArgs e)
        {
            if (seciliKategoriId == 0)
            {
                MessageBox.Show(
                    "Lütfen düzenlemek istediğiniz kategoriyi seçin.",
                    "Uyarı",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                return;
            }

            if (string.IsNullOrWhiteSpace(textBox1.Text))
            {
                MessageBox.Show(
                    "Kategori adı boş bırakılamaz.",
                    "Uyarı",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                return;
            }

            try
            {
                SqlBaglanti db = new SqlBaglanti();

                using (SqlConnection conn = db.Connection())
                {
                    string sorgu = @"
                UPDATE Kategori
                SET
                    KategoriAdi = @KategoriAdi,
                    Aciklama = @Aciklama,
                    AktifMi = @AktifMi
                WHERE KategoriId = @KategoriId";

                    using (SqlCommand cmd = new SqlCommand(sorgu, conn))
                    {
                        cmd.Parameters.AddWithValue(
                            "@KategoriAdi",
                            textBox1.Text.Trim());

                        cmd.Parameters.AddWithValue(
                            "@Aciklama",
                            string.IsNullOrWhiteSpace(richTextBox1.Text)
                                ? (object)DBNull.Value
                                : richTextBox1.Text.Trim());

                        cmd.Parameters.AddWithValue(
                            "@AktifMi",
                            comboBox1.Text == "Aktif" ? 1 : 0);

                        cmd.Parameters.AddWithValue(
                            "@KategoriId",
                            seciliKategoriId);

                        cmd.ExecuteNonQuery();
                    }
                }

                MessageBox.Show(
                    "Kategori başarıyla güncellendi.",
                    "Başarılı",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                KategoriListele();
                TemizleKategori();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Kategori güncellenirken hata oluştu:\n\n" + ex.Message,
                    "Hata",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void foxButton5_Click(object sender, EventArgs e)
        {
            if (seciliKategoriId == 0)
            {
                MessageBox.Show(
                    "Lütfen silmek istediğiniz kategoriyi seçin.",
                    "Uyarı",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                return;
            }

            DialogResult cevap = MessageBox.Show(
                "Seçili kategoriyi silmek istediğinize emin misiniz?",
                "Kategori Sil",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (cevap != DialogResult.Yes)
                return;

            try
            {
                SqlBaglanti db = new SqlBaglanti();

                using (SqlConnection conn = db.Connection())
                {
                    string sorgu = @"
                DELETE FROM Kategori
                WHERE KategoriId = @KategoriId";

                    using (SqlCommand cmd = new SqlCommand(sorgu, conn))
                    {
                        cmd.Parameters.AddWithValue(
                            "@KategoriId",
                            seciliKategoriId);

                        cmd.ExecuteNonQuery();
                    }
                }

                MessageBox.Show(
                    "Kategori başarıyla silindi.",
                    "Başarılı",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                seciliKategoriId = 0;

                KategoriListele();
                TemizleKategori();
            }
            catch (SqlException ex)
            {
                MessageBox.Show(
                    "Kategori silinemedi.\n\n" +
                    "Bu kategori bir ürüne bağlı olabilir.\n\n" +
                    ex.Message,
                    "Hata",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Silme sırasında hata oluştu:\n\n" +
                    ex.Message,
                    "Hata",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }

        }
    }

}