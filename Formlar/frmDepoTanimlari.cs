using StokTakip_Modern.Sınıflar;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace StokTakip_Modern.Formlar
{
    public partial class frmDepoTanimlari : Form
    {
        public frmDepoTanimlari()
        {
            InitializeComponent();

            this.Load += frmDepoTanimlari_Load;
        }

        // =========================================================
        // KAYDET
        // =========================================================
        private void foxButton1_Click(object sender, EventArgs e)
        {
            // Depo adı kontrolü
            if (string.IsNullOrWhiteSpace(textBox1.Text))
            {
                MessageBox.Show(
                    "Lütfen depo adını giriniz.",
                    "Uyarı",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                textBox1.Focus();
                return;
            }

            // Konum kontrolü
            // richTextBox2 = KONUM
            if (string.IsNullOrWhiteSpace(richTextBox2.Text))
            {
                MessageBox.Show(
                    "Lütfen depo konumunu giriniz.",
                    "Uyarı",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                richTextBox2.Focus();
                return;
            }

            try
            {
                SqlBaglanti baglanti = new SqlBaglanti();

                using (SqlConnection conn = baglanti.Connection())
                {
                    string sql = @"
                        INSERT INTO Depo
                        (
                            DepoAdi,
                            Konum,
                            Aciklama,
                            AktifMi
                        )
                        VALUES
                        (
                            @DepoAdi,
                            @Konum,
                            @Aciklama,
                            @AktifMi
                        )";

                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        // Depo Adı
                        cmd.Parameters.AddWithValue(
                            "@DepoAdi",
                            textBox1.Text.Trim());

                        // richTextBox2 = Konum
                        cmd.Parameters.AddWithValue(
                            "@Konum",
                            richTextBox2.Text.Trim());

                        // richTextBox1 = Açıklama
                        cmd.Parameters.AddWithValue(
                            "@Aciklama",
                            string.IsNullOrWhiteSpace(richTextBox1.Text)
                                ? (object)DBNull.Value
                                : richTextBox1.Text.Trim());

                        // Aktif / Pasif
                        bool aktifMi = true;

                        if (comboBox1.SelectedIndex >= 0)
                        {
                            aktifMi = comboBox1.Text == "Aktif";
                        }

                        cmd.Parameters.AddWithValue(
                            "@AktifMi",
                            aktifMi);

                        int sonuc = cmd.ExecuteNonQuery();

                        if (sonuc > 0)
                        {
                            MessageBox.Show(
                                "Depo başarıyla kaydedildi.",
                                "Başarılı",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Information);

                            DepolariGetir();
                            DepoFormuTemizle();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Depo kaydedilirken bir hata oluştu.\n\n" + ex.Message,
                    "Hata",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        // =========================================================
        // TEMİZLE
        // =========================================================
        private void foxButton2_Click(object sender, EventArgs e)
        {
            DepoFormuTemizle();
        }

        private void DepoFormuTemizle()
        {
            textBox1.Clear();

            // richTextBox2 = Konum
            richTextBox2.Clear();

            // richTextBox1 = Açıklama
            richTextBox1.Clear();

            comboBox1.SelectedIndex = -1;
        }

        // =========================================================
        // DÜZENLE / GÜNCELLE
        // =========================================================
        private void button2_Click(object sender, EventArgs e)
        {
            // Listeden depo seçilmiş mi?
            if (dataGridView1.CurrentRow == null)
            {
                MessageBox.Show(
                    "Lütfen düzenlemek istediğiniz depoyu listeden seçin.",
                    "Uyarı",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                return;
            }

            // DepoId değerini al
            int depoId = Convert.ToInt32(
                dataGridView1.CurrentRow.Cells["DepoId"].Value);

            // Depo adı kontrolü
            if (string.IsNullOrWhiteSpace(textBox1.Text))
            {
                MessageBox.Show(
                    "Lütfen depo adını giriniz.",
                    "Uyarı",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                textBox1.Focus();
                return;
            }

            // richTextBox2 = Konum
            if (string.IsNullOrWhiteSpace(richTextBox2.Text))
            {
                MessageBox.Show(
                    "Lütfen depo konumunu giriniz.",
                    "Uyarı",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                richTextBox2.Focus();
                return;
            }

            DialogResult sonuc = MessageBox.Show(
                "Seçili deponun bilgilerini güncellemek istediğinizden emin misiniz?",
                "Depo Düzenleme",
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
                        UPDATE Depo
                        SET
                            DepoAdi = @DepoAdi,
                            Konum = @Konum,
                            Aciklama = @Aciklama,
                            AktifMi = @AktifMi
                        WHERE DepoId = @DepoId";

                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue(
                            "@DepoId",
                            depoId);

                        // Depo Adı
                        cmd.Parameters.AddWithValue(
                            "@DepoAdi",
                            textBox1.Text.Trim());

                        // richTextBox2 = Konum
                        cmd.Parameters.AddWithValue(
                            "@Konum",
                            richTextBox2.Text.Trim());

                        // richTextBox1 = Açıklama
                        cmd.Parameters.AddWithValue(
                            "@Aciklama",
                            string.IsNullOrWhiteSpace(richTextBox1.Text)
                                ? (object)DBNull.Value
                                : richTextBox1.Text.Trim());

                        // Aktif / Pasif
                        bool aktifMi = true;

                        if (comboBox1.SelectedIndex >= 0)
                        {
                            aktifMi = comboBox1.Text == "Aktif";
                        }

                        cmd.Parameters.AddWithValue(
                            "@AktifMi",
                            aktifMi);

                        int sonucSatir = cmd.ExecuteNonQuery();

                        if (sonucSatir > 0)
                        {
                            MessageBox.Show(
                                "Depo bilgileri başarıyla güncellendi.",
                                "Başarılı",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Information);

                            DepolariGetir();
                            DepoFormuTemizle();
                        }
                        else
                        {
                            MessageBox.Show(
                                "Güncellenecek depo bulunamadı.",
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
                    "Depo güncellenirken bir hata oluştu.\n\n" + ex.Message,
                    "Hata",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        // =========================================================
        // PASİFLEŞTİR
        // =========================================================
        private void button3_Click(object sender, EventArgs e)
        {
            // Önce listeden bir depo seçilmiş mi?
            if (dataGridView1.CurrentRow == null)
            {
                MessageBox.Show(
                    "Lütfen pasifleştirmek istediğiniz depoyu listeden seçin.",
                    "Uyarı",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                return;
            }

            // DepoId değerini al
            int depoId = Convert.ToInt32(
                dataGridView1.CurrentRow.Cells["DepoId"].Value);

            // Kullanıcıdan onay al
            DialogResult sonuc = MessageBox.Show(
                "Seçili depoyu pasifleştirmek istediğinizden emin misiniz?",
                "Depo Pasifleştirme",
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
                        UPDATE Depo
                        SET AktifMi = 0
                        WHERE DepoId = @DepoId";

                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue(
                            "@DepoId",
                            depoId);

                        int sonucSatir = cmd.ExecuteNonQuery();

                        if (sonucSatir > 0)
                        {
                            MessageBox.Show(
                                "Depo başarıyla pasifleştirildi.",
                                "Başarılı",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Information);

                            DepolariGetir();
                        }
                        else
                        {
                            MessageBox.Show(
                                "Depo bulunamadı.",
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
                    "Depo pasifleştirilirken bir hata oluştu.\n\n" + ex.Message,
                    "Hata",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        // =========================================================
        // FORM LOAD
        // =========================================================
        private void frmDepoTanimlari_Load(object sender, EventArgs e)
        {
            DepolariGetir();
        }

        // =========================================================
        // DEPOLARI GETİR
        // =========================================================
        private void DepolariGetir()
        {
            try
            {
                SqlBaglanti baglanti = new SqlBaglanti();

                using (SqlConnection conn = baglanti.Connection())
                {
                    string sql = @"
                        SELECT
                            DepoId,
                            DepoAdi,
                            Konum,
                            Aciklama,
                            CASE
                                WHEN AktifMi = 1 THEN 'Aktif'
                                ELSE 'Pasif'
                            END AS Durum
                        FROM Depo
                        ORDER BY DepoId DESC";

                    using (SqlDataAdapter da =
                        new SqlDataAdapter(sql, conn))
                    {
                        DataTable dt = new DataTable();

                        da.Fill(dt);

                        dataGridView1.DataSource = dt;
                    }
                }

                // ID kullanıcıya görünmesin
                // Ama arkada tutulmaya devam etsin
                if (dataGridView1.Columns.Contains("DepoId"))
                {
                    dataGridView1.Columns["DepoId"].Visible = false;
                }

                // Başlıkları Türkçeleştir
                if (dataGridView1.Columns.Contains("DepoAdi"))
                {
                    dataGridView1.Columns["DepoAdi"].HeaderText =
                        "Depo Adı";
                }

                if (dataGridView1.Columns.Contains("Konum"))
                {
                    dataGridView1.Columns["Konum"].HeaderText =
                        "Konum";
                }

                if (dataGridView1.Columns.Contains("Aciklama"))
                {
                    dataGridView1.Columns["Aciklama"].HeaderText =
                        "Açıklama";
                }

                if (dataGridView1.Columns.Contains("Durum"))
                {
                    dataGridView1.Columns["Durum"].HeaderText =
                        "Durum";
                }

                dataGridView1.AutoSizeColumnsMode =
                    DataGridViewAutoSizeColumnsMode.Fill;

                dataGridView1.SelectionMode =
                    DataGridViewSelectionMode.FullRowSelect;

                dataGridView1.MultiSelect = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Depolar yüklenirken bir hata oluştu.\n\n" + ex.Message,
                    "Hata",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        // =========================================================
        // DATAGRIDVIEW SATIR SEÇİMİ
        // =========================================================
        private void dataGridView1_CellContentClick(
            object sender,
            DataGridViewCellEventArgs e)
        {
            // Başlık satırına tıklanırsa işlem yapma
            if (e.RowIndex < 0)
                return;

            DataGridViewRow row =
                dataGridView1.Rows[e.RowIndex];

            // Depo Adı
            textBox1.Text =
                row.Cells["DepoAdi"].Value?.ToString() ?? "";

            // richTextBox2 = KONUM
            richTextBox2.Text =
                row.Cells["Konum"].Value?.ToString() ?? "";

            // richTextBox1 = AÇIKLAMA
            richTextBox1.Text =
                row.Cells["Aciklama"].Value?.ToString() ?? "";

            // Durum
            string durum =
                row.Cells["Durum"].Value?.ToString();

            if (durum == "Aktif")
            {
                comboBox1.Text = "Aktif";
            }
            else if (durum == "Pasif")
            {
                comboBox1.Text = "Pasif";
            }
        }

        // =========================================================
        // BOŞ EVENTLER
        // =========================================================

        private void foxButton3_Click(object sender, EventArgs e)
        {
        }

        private void labelEdit4_Click(object sender, EventArgs e)
        {
        }

        private void labelEdit1_Click(object sender, EventArgs e)
        {
        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
        }
    }
}