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
    public partial class frmRafTanimlari : Form
    {
        public frmRafTanimlari()
        {
            InitializeComponent();
            this.Load += frmRafTanimlari_Load;
            dataGridView1.CellClick += dataGridView1_CellContentClick; 
        }

        private void frmRafTanimlari_Load(object sender, EventArgs e)
        {
            DepolariGetir();
            RaflariGetir();
        }
        private void DepolariGetir()
        {
            try
            {
                SqlBaglanti baglanti = new SqlBaglanti();

                using (SqlConnection conn = baglanti.Connection())
                {
                    string sql = @"
                SELECT DepoId, DepoAdi
                FROM Depo
                WHERE AktifMi = 1
                ORDER BY DepoAdi";

                    using (SqlDataAdapter da = new SqlDataAdapter(sql, conn))
                    {
                        DataTable dt = new DataTable();
                        da.Fill(dt);

                        comboBox2.DataSource = dt;
                        comboBox2.DisplayMember = "DepoAdi";
                        comboBox2.ValueMember = "DepoId";
                        comboBox2.SelectedIndex = -1;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Depolar yüklenirken hata oluştu.\n\n" + ex.Message,
                    "Hata",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }
        private void RaflariGetir()
        {
            try
            {
                SqlBaglanti baglanti = new SqlBaglanti();

                using (SqlConnection conn = baglanti.Connection())
                {
                    string sql = @"
                SELECT
                    R.RafId,
                    R.DepoId,
                    D.DepoAdi,
                    R.RafKodu,
                    R.Aciklama,
                    CASE
                        WHEN R.AktifMi = 1 THEN 'Aktif'
                        ELSE 'Pasif'
                    END AS Durum
                FROM Raf R
                INNER JOIN Depo D
                    ON R.DepoId = D.DepoId
                ORDER BY
                    D.DepoAdi,
                    R.RafKodu";

                    using (SqlDataAdapter da = new SqlDataAdapter(sql, conn))
                    {
                        DataTable dt = new DataTable();
                        da.Fill(dt);

                        dataGridView1.DataSource = dt;
                    }
                }

                // ID'leri kullanıcıya göstermiyoruz,
                // ama düzenleme/pasifleştirme işlemlerinde kullanacağız.
                if (dataGridView1.Columns.Contains("RafId"))
                    dataGridView1.Columns["RafId"].Visible = false;

                if (dataGridView1.Columns.Contains("DepoId"))
                    dataGridView1.Columns["DepoId"].Visible = false;

                // Kolon başlıkları
                if (dataGridView1.Columns.Contains("DepoAdi"))
                    dataGridView1.Columns["DepoAdi"].HeaderText = "Depo";

                if (dataGridView1.Columns.Contains("RafKodu"))
                    dataGridView1.Columns["RafKodu"].HeaderText = "Raf Kodu";

                if (dataGridView1.Columns.Contains("Aciklama"))
                    dataGridView1.Columns["Aciklama"].HeaderText = "Açıklama";

                if (dataGridView1.Columns.Contains("Durum"))
                    dataGridView1.Columns["Durum"].HeaderText = "Durum";

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
                    "Raflar yüklenirken bir hata oluştu.\n\n" + ex.Message,
                    "Hata",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void foxButton1_Click(object sender, EventArgs e) //KAYDET BUTONU
        {
            // Depo seçilmiş mi?
            if (comboBox2.SelectedIndex == -1)
            {
                MessageBox.Show(
                    "Lütfen bir depo seçin.",
                    "Uyarı",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                return;
            }

            // Raf kodu boş mu?
            if (string.IsNullOrWhiteSpace(textBox1.Text))
            {
                MessageBox.Show(
                    "Lütfen raf kodunu girin.",
                    "Uyarı",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                textBox1.Focus();
                return;
            }

            // Durum seçilmiş mi?
            if (comboBox1.SelectedIndex == -1)
            {
                MessageBox.Show(
                    "Lütfen raf durumunu seçin.",
                    "Uyarı",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                return;
            }

            try
            {
                int depoId = Convert.ToInt32(comboBox2.SelectedValue);
                string rafKodu = textBox1.Text.Trim();
                string aciklama = richTextBox1.Text.Trim();

                bool aktifMi = comboBox1.SelectedItem.ToString() == "Aktif";

                SqlBaglanti baglanti = new SqlBaglanti();

                using (SqlConnection conn = baglanti.Connection())
                {
                    string sql = @"
                INSERT INTO Raf
                (
                    DepoId,
                    RafKodu,
                    Aciklama,
                    AktifMi
                )
                VALUES
                (
                    @DepoId,
                    @RafKodu,
                    @Aciklama,
                    @AktifMi
                )";

                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@DepoId", depoId);
                        cmd.Parameters.AddWithValue("@RafKodu", rafKodu);

                        if (string.IsNullOrWhiteSpace(aciklama))
                            cmd.Parameters.AddWithValue("@Aciklama", DBNull.Value);
                        else
                            cmd.Parameters.AddWithValue("@Aciklama", aciklama);

                        cmd.Parameters.AddWithValue("@AktifMi", aktifMi);

                        cmd.ExecuteNonQuery();
                    }
                }

                MessageBox.Show(
                    "Raf başarıyla kaydedildi.",
                    "Başarılı",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                // Listeyi yenile
                RaflariGetir();

                // Alanları temizle
                RafFormuTemizle();
            }
            catch (SqlException ex)
            {
                // Aynı depoda aynı raf kodu varsa UNIQUE hatası
                if (ex.Number == 2627 || ex.Number == 2601)
                {
                    MessageBox.Show(
                        "Bu raf kodu seçilen depoda zaten kayıtlı.",
                        "Raf Kodu Kullanılıyor",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                }
                else
                {
                    MessageBox.Show(
                        "Raf kaydedilirken bir hata oluştu.\n\n" + ex.Message,
                        "SQL Hatası",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Raf kaydedilirken bir hata oluştu.\n\n" + ex.Message,
                    "Hata",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }
        private void RafFormuTemizle()
        {
            comboBox2.SelectedIndex = -1;
            textBox1.Clear();
            richTextBox1.Clear();
            comboBox1.SelectedIndex = -1;
        }

        private void button2_Click(object sender, EventArgs e) //düzenle butonu
        {
            if (dataGridView1.CurrentRow == null)
            {
                MessageBox.Show(
                    "Lütfen düzenlemek istediğiniz rafı listeden seçin.",
                    "Uyarı",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                return;
            }

            if (comboBox2.SelectedIndex == -1)
            {
                MessageBox.Show(
                    "Lütfen bir depo seçin.",
                    "Uyarı",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                return;
            }

            if (string.IsNullOrWhiteSpace(textBox1.Text))
            {
                MessageBox.Show(
                    "Lütfen raf kodunu girin.",
                    "Uyarı",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                textBox1.Focus();
                return;
            }

            if (comboBox1.SelectedIndex == -1)
            {
                MessageBox.Show(
                    "Lütfen raf durumunu seçin.",
                    "Uyarı",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                return;
            }

            int rafId = Convert.ToInt32(
                dataGridView1.CurrentRow.Cells["RafId"].Value);

            int depoId = Convert.ToInt32(comboBox2.SelectedValue);

            string rafKodu = textBox1.Text.Trim();
            string aciklama = richTextBox1.Text.Trim();

            bool aktifMi = comboBox1.SelectedItem.ToString() == "Aktif";

            DialogResult sonuc = MessageBox.Show(
                "Seçili rafın bilgilerini güncellemek istediğinizden emin misiniz?",
                "Raf Düzenleme",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (sonuc != DialogResult.Yes)
                return;

            try
            {
                SqlBaglanti baglanti = new SqlBaglanti();

                using (SqlConnection conn = baglanti.Connection())
                {
                    string sql = @"
                UPDATE Raf
                SET
                    DepoId = @DepoId,
                    RafKodu = @RafKodu,
                    Aciklama = @Aciklama,
                    AktifMi = @AktifMi
                WHERE RafId = @RafId";

                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@RafId", rafId);
                        cmd.Parameters.AddWithValue("@DepoId", depoId);
                        cmd.Parameters.AddWithValue("@RafKodu", rafKodu);

                        if (string.IsNullOrWhiteSpace(aciklama))
                            cmd.Parameters.AddWithValue("@Aciklama", DBNull.Value);
                        else
                            cmd.Parameters.AddWithValue("@Aciklama", aciklama);

                        cmd.Parameters.AddWithValue("@AktifMi", aktifMi);

                        int etkilenenSatir = cmd.ExecuteNonQuery();

                        if (etkilenenSatir > 0)
                        {
                            MessageBox.Show(
                                "Raf başarıyla güncellendi.",
                                "Başarılı",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Information);

                            RaflariGetir();
                            RafFormuTemizle();
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                if (ex.Number == 2627 || ex.Number == 2601)
                {
                    MessageBox.Show(
                        "Bu raf kodu seçilen depoda zaten kullanılıyor.",
                        "Uyarı",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                }
                else
                {
                    MessageBox.Show(
                        "Raf güncellenirken SQL hatası oluştu.\n\n" + ex.Message,
                        "SQL Hatası",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Raf güncellenirken bir hata oluştu.\n\n" + ex.Message,
                    "Hata",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void button3_Click(object sender, EventArgs e) //pasifleştir butonu
        {
            if (dataGridView1.CurrentRow == null)
            {
                MessageBox.Show(
                    "Lütfen pasifleştirmek istediğiniz rafı listeden seçin.",
                    "Uyarı",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                return;
            }

            int rafId = Convert.ToInt32(
                dataGridView1.CurrentRow.Cells["RafId"].Value);

            string rafKodu =
                dataGridView1.CurrentRow.Cells["RafKodu"].Value?.ToString();

            string depoAdi =
                dataGridView1.CurrentRow.Cells["DepoAdi"].Value?.ToString();

            DialogResult sonuc = MessageBox.Show(
                $"{depoAdi} içerisindeki {rafKodu} rafını pasifleştirmek istediğinizden emin misiniz?",
                "Raf Pasifleştirme",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (sonuc != DialogResult.Yes)
                return;

            try
            {
                SqlBaglanti baglanti = new SqlBaglanti();

                using (SqlConnection conn = baglanti.Connection())
                {
                    string sql = @"
                UPDATE Raf
                SET AktifMi = 0
                WHERE RafId = @RafId";

                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@RafId", rafId);

                        int etkilenenSatir = cmd.ExecuteNonQuery();

                        if (etkilenenSatir > 0)
                        {
                            MessageBox.Show(
                                "Raf başarıyla pasifleştirildi.",
                                "Başarılı",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Information);

                            RaflariGetir();
                            RafFormuTemizle();
                        }
                        else
                        {
                            MessageBox.Show(
                                "Raf bulunamadı.",
                                "Uyarı",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Raf pasifleştirilirken bir hata oluştu.\n\n" + ex.Message,
                    "Hata",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }

        }

        private void foxButton2_Click(object sender, EventArgs e)
        {
            RafFormuTemizle();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            // Başlık satırına tıklanırsa işlem yapma
            if (e.RowIndex < 0)
                return;

            DataGridViewRow satir = dataGridView1.Rows[e.RowIndex];

            // Raf kodu
            textBox1.Text =
                satir.Cells["RafKodu"].Value?.ToString() ?? "";

            // Açıklama
            richTextBox1.Text =
                satir.Cells["Aciklama"].Value?.ToString() ?? "";

            // Depo seçimi
            comboBox2.SelectedValue =
                satir.Cells["DepoId"].Value;

            // Aktif / Pasif durumu
            string durum =
                satir.Cells["Durum"].Value?.ToString() ?? "";

            comboBox1.SelectedItem = durum;
        }
    }
}
