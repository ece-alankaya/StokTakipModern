using StokTakip_Modern.Sınıflar;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace StokTakip_Modern.Formlar
{
    public partial class frmSevkiyatYonetimi : Form
    {
        SqlBaglanti baglanti = new SqlBaglanti();

        private DataTable sevkiyatUrunleri = new DataTable();
        private BindingSource sevkiyatBinding = new BindingSource();

        private int aktifSevkiyatId = 0;

        public frmSevkiyatYonetimi()
        {
            InitializeComponent();

            Load += frmSevkiyatYonetimi_Load;

            comboBox1.SelectedIndexChanged += comboBox1_SelectedIndexChanged;
            comboBox3.SelectedIndexChanged += comboBox3_SelectedIndexChanged;
            

            // Butonları burada tekrar bağlamıyoruz.
            // Designer üzerinden bağlı olan _1 metotları çalışacak.
        }

        // =========================================================
        // FORM LOAD
        // =========================================================

        private void frmSevkiyatYonetimi_Load(object sender, EventArgs e)
        {
            try
            {
                DepolariGetir();
                TedarikcileriGetir();
                UrunleriGetir();
                DurumlariGetir();

                comboBox2.DataSource = null;
                comboBox4.DataSource = null;

                textBox1.Clear();
                textBox2.Clear();

                // Ürün ekleme tablosunu hazırla
                SevkiyatUrunTablosunuHazirla();

                // Kayıtlı sevkiyatları göster
                SevkiyatlariGetir();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Form yüklenirken hata oluştu.\n\n" +
                    ex.Message,
                    "Hata",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        // =========================================================
        // SEVKİYAT ÜRÜN TABLOSU
        // =========================================================

        private void SevkiyatUrunTablosunuHazirla()
        {
            sevkiyatUrunleri = new DataTable();

            sevkiyatUrunleri.Columns.Add("CikisDepo", typeof(string));
            sevkiyatUrunleri.Columns.Add("CikisRaf", typeof(string));
            sevkiyatUrunleri.Columns.Add("VarisDepo", typeof(string));
            sevkiyatUrunleri.Columns.Add("VarisRaf", typeof(string));
            sevkiyatUrunleri.Columns.Add("Tedarikci", typeof(string));
            sevkiyatUrunleri.Columns.Add("Durum", typeof(string));

            // Veritabanında kullanılacak, ekranda gizli
            sevkiyatUrunleri.Columns.Add("UrunId", typeof(int));

            sevkiyatUrunleri.Columns.Add("Ürün", typeof(string));
            sevkiyatUrunleri.Columns.Add("Miktar", typeof(int));
            sevkiyatUrunleri.Columns.Add("Açıklama", typeof(string));

            sevkiyatBinding.DataSource = sevkiyatUrunleri;

            dataGridView1.DataSource = sevkiyatBinding;

            dataGridView1.ReadOnly = true;
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.AllowUserToDeleteRows = false;
            dataGridView1.MultiSelect = false;

            dataGridView1.SelectionMode =
                DataGridViewSelectionMode.FullRowSelect;

            if (dataGridView1.Columns.Contains("UrunId"))
                dataGridView1.Columns["UrunId"].Visible = false;

            dataGridView1.Columns["CikisDepo"].HeaderText = "Çıkış Depo";
            dataGridView1.Columns["CikisRaf"].HeaderText = "Çıkış Rafı";
            dataGridView1.Columns["VarisDepo"].HeaderText = "Varış Deposu";
            dataGridView1.Columns["VarisRaf"].HeaderText = "Varış Rafı";
            dataGridView1.Columns["Tedarikci"].HeaderText = "Tedarikçi";
            dataGridView1.Columns["Durum"].HeaderText = "Durum";
            dataGridView1.Columns["Ürün"].HeaderText = "Ürün";
            dataGridView1.Columns["Miktar"].HeaderText = "Miktar";
            dataGridView1.Columns["Açıklama"].HeaderText = "Açıklama";

            dataGridView1.AutoSizeColumnsMode =
                DataGridViewAutoSizeColumnsMode.Fill;

            // Sil butonu
            if (!dataGridView1.Columns.Contains("Sil"))
            {
                DataGridViewButtonColumn silKolonu =
                    new DataGridViewButtonColumn();

                silKolonu.Name = "Sil";
                silKolonu.HeaderText = "İşlem";
                silKolonu.Text = "Sil";
                silKolonu.UseColumnTextForButtonValue = true;

                dataGridView1.Columns.Add(silKolonu);
            }
        }

        // =========================================================
        // DATAGRIDVIEW SİL
        // =========================================================

        private void dataGridView1_CellClick(
            object sender,
            DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
                return;

            if (e.ColumnIndex < 0)
                return;

            if (!dataGridView1.Columns.Contains("Sil"))
                return;

            if (dataGridView1.Columns[e.ColumnIndex].Name != "Sil")
                return;

            // Sadece ürün ekleme tablosundaysa sil
            if (dataGridView1.DataSource != sevkiyatBinding)
                return;

            if (e.RowIndex >= sevkiyatUrunleri.Rows.Count)
                return;

            sevkiyatUrunleri.Rows.RemoveAt(e.RowIndex);

            sevkiyatBinding.ResetBindings(false);
        }

        // =========================================================
        // DEPOLARI GETİR
        // =========================================================

        private void DepolariGetir()
        {
            try
            {
                using (SqlConnection conn = baglanti.Connection())
                {
                    string sql = @"
                        SELECT DepoId, DepoAdi
                        FROM Depo
                        WHERE AktifMi = 1
                        ORDER BY DepoAdi";

                    using (SqlDataAdapter da =
                        new SqlDataAdapter(sql, conn))
                    {
                        DataTable dt = new DataTable();

                        da.Fill(dt);

                        // Çıkış deposu
                        comboBox1.DataSource = dt.Copy();
                        comboBox1.DisplayMember = "DepoAdi";
                        comboBox1.ValueMember = "DepoId";
                        comboBox1.SelectedIndex = -1;

                        // Varış deposu
                        comboBox3.DataSource = dt;
                        comboBox3.DisplayMember = "DepoAdi";
                        comboBox3.ValueMember = "DepoId";
                        comboBox3.SelectedIndex = -1;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Depolar yüklenirken hata oluştu.\n\n" +
                    ex.Message,
                    "Hata",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        // =========================================================
        // ÇIKIŞ DEPOSU DEĞİŞİNCE
        // =========================================================

        private void comboBox1_SelectedIndexChanged(
            object sender,
            EventArgs e)
        {
            if (comboBox1.SelectedValue == null ||
                comboBox1.SelectedValue is DataRowView)
            {
                return;
            }

            try
            {
                int depoId =
                    Convert.ToInt32(comboBox1.SelectedValue);

                RaflariGetir(comboBox2, depoId);
            }
            catch
            {
                comboBox2.DataSource = null;
            }
        }

        // =========================================================
        // VARIŞ DEPOSU DEĞİŞİNCE
        // =========================================================

        private void comboBox3_SelectedIndexChanged(
            object sender,
            EventArgs e)
        {
            if (comboBox3.SelectedValue == null ||
                comboBox3.SelectedValue is DataRowView)
            {
                return;
            }

            try
            {
                int depoId =
                    Convert.ToInt32(comboBox3.SelectedValue);

                RaflariGetir(comboBox4, depoId);
            }
            catch
            {
                comboBox4.DataSource = null;
            }
        }

        // =========================================================
        // RAFLARI GETİR
        // =========================================================

        private void RaflariGetir(
            ComboBox comboBox,
            int depoId)
        {
            try
            {
                using (SqlConnection conn = baglanti.Connection())
                {
                    string sql = @"
                        SELECT RafId, RafKodu
                        FROM Raf
                        WHERE DepoId = @DepoId
                        ORDER BY RafKodu";

                    using (SqlCommand cmd =
                        new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.Add(
                            "@DepoId",
                            SqlDbType.Int).Value = depoId;

                        using (SqlDataAdapter da =
                            new SqlDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();

                            da.Fill(dt);

                            comboBox.DataSource = null;
                            comboBox.DisplayMember = "RafKodu";
                            comboBox.ValueMember = "RafId";
                            comboBox.DataSource = dt;
                            comboBox.SelectedIndex = -1;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Raflar yüklenirken hata oluştu.\n\n" +
                    ex.Message,
                    "Hata",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        // =========================================================
        // TEDARİKÇİLERİ GETİR
        // =========================================================

        private void TedarikcileriGetir()
        {
            try
            {
                using (SqlConnection conn = baglanti.Connection())
                {
                    string sql = @"
                        SELECT TedarikciId, FirmaAdi
                        FROM Tedarikci
                        WHERE AktifMi = 1
                        ORDER BY FirmaAdi";

                    using (SqlDataAdapter da =
                        new SqlDataAdapter(sql, conn))
                    {
                        DataTable dt = new DataTable();

                        da.Fill(dt);

                        comboBox7.DataSource = dt;
                        comboBox7.DisplayMember = "FirmaAdi";
                        comboBox7.ValueMember = "TedarikciId";
                        comboBox7.SelectedIndex = -1;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Tedarikçiler yüklenirken hata oluştu.\n\n" +
                    ex.Message,
                    "Hata",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        // =========================================================
        // ÜRÜNLERİ GETİR
        // =========================================================

        private void UrunleriGetir()
        {
            try
            {
                using (SqlConnection conn = baglanti.Connection())
                {
                    string sql = @"
                        SELECT UrunId, UrunAdi
                        FROM Urun
                        WHERE AktifMi = 1
                        ORDER BY UrunAdi";

                    using (SqlDataAdapter da =
                        new SqlDataAdapter(sql, conn))
                    {
                        DataTable dt = new DataTable();

                        da.Fill(dt);

                        comboBox6.DataSource = dt;
                        comboBox6.DisplayMember = "UrunAdi";
                        comboBox6.ValueMember = "UrunId";
                        comboBox6.SelectedIndex = -1;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Ürünler yüklenirken hata oluştu.\n\n" +
                    ex.Message,
                    "Hata",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        // =========================================================
        // DURUMLAR
        // =========================================================

        private void DurumlariGetir()
        {
            comboBox5.Items.Clear();

            comboBox5.Items.Add("Hazırlanıyor");
            comboBox5.Items.Add("Sevk Edildi");
            comboBox5.Items.Add("Teslim Edildi");
            comboBox5.Items.Add("İptal");

            comboBox5.SelectedIndex = -1;
        }

        // =========================================================
        // ÜRÜN GİRİŞ BUTONU
        // =========================================================

        private void btnGiris_Click(object sender, EventArgs e)
        {
            // Eski event.
            // Asıl işlem btnGiris_Click_1 içerisinde.
        }

        // =========================================================
        // KAYDET BUTONU
        // =========================================================

        private void hopeButton1_Click(object sender, EventArgs e)
        {
            // Eski event.
            // Asıl işlem hopeButton1_Click_1 içerisinde.
        }

        // =========================================================
        // TEMİZLE BUTONU
        // =========================================================

        private void hopeButton2_Click(object sender, EventArgs e)
        {
            FormuTemizle();
        }

        // =========================================================
        // FORM TEMİZLE
        // =========================================================

        private void FormuTemizle()
        {
            aktifSevkiyatId = 0;

            comboBox1.SelectedIndex = -1;
            comboBox2.DataSource = null;

            comboBox3.SelectedIndex = -1;
            comboBox4.DataSource = null;

            comboBox5.SelectedIndex = -1;
            comboBox6.SelectedIndex = -1;
            comboBox7.SelectedIndex = -1;

            textBox1.Clear();
            textBox2.Clear();

            if (sevkiyatUrunleri != null)
                sevkiyatUrunleri.Clear();

            dataGridView1.DataSource = sevkiyatBinding;

            sevkiyatBinding.ResetBindings(false);

            comboBox6.Focus();
        }

        // =========================================================
        // SEVKİYATLARI LİSTELE
        // =========================================================

        private void SevkiyatlariGetir()
        {
            try
            {
                using (SqlConnection conn = baglanti.Connection())
                {
                    string sql = @"
                        SELECT
                            s.SevkiyatId AS [ID],
                            cd.DepoAdi AS [Çıkış Deposu],
                            vd.DepoAdi AS [Varış Deposu],
                            ISNULL(t.FirmaAdi, '-') AS [Tedarikçi],
                            s.SevkiyatTarihi AS [Tarih],
                            s.Durum AS [Durum],
                            s.Aciklama AS [Açıklama]
                        FROM Sevkiyat s
                        INNER JOIN Depo cd
                            ON s.CikisDepoId = cd.DepoId
                        LEFT JOIN Depo vd
                            ON s.VarisDepoId = vd.DepoId
                        LEFT JOIN Tedarikci t
                            ON s.TedarikciId = t.TedarikciId
                        ORDER BY s.SevkiyatId DESC";

                    using (SqlDataAdapter da =
                        new SqlDataAdapter(sql, conn))
                    {
                        DataTable dt = new DataTable();

                        da.Fill(dt);

                        dataGridView1.DataSource = dt;
                    }

                    dataGridView1.AutoSizeColumnsMode =
                        DataGridViewAutoSizeColumnsMode.Fill;

                    dataGridView1.ReadOnly = true;
                    dataGridView1.AllowUserToAddRows = false;
                    dataGridView1.AllowUserToDeleteRows = false;

                    dataGridView1.SelectionMode =
                        DataGridViewSelectionMode.FullRowSelect;

                    dataGridView1.MultiSelect = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Sevkiyatlar yüklenirken hata oluştu.\n\n" +
                    ex.Message,
                    "Hata",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        // =========================================================
        // ÜRÜN EKLE
        // =========================================================

        private void hopeButton3_Click(object sender, EventArgs e)
        {
            if (comboBox6.SelectedValue == null ||
                comboBox6.SelectedValue is DataRowView)
            {
                MessageBox.Show(
                    "Lütfen ürün seçiniz.",
                    "Uyarı",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                return;
            }

            int miktar;

            if (!int.TryParse(
                textBox1.Text.Trim(),
                out miktar) ||
                miktar <= 0)
            {
                MessageBox.Show(
                    "Lütfen 0'dan büyük geçerli bir miktar giriniz.",
                    "Uyarı",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                textBox1.Focus();
                return;
            }

            string cikisDepo = comboBox1.Text.Trim();
            string cikisRaf = comboBox2.Text.Trim();

            string varisDepo = comboBox3.Text.Trim();
            string varisRaf = comboBox4.Text.Trim();

            string durum = comboBox5.Text.Trim();
            string tedarikci = comboBox7.Text.Trim();

            string urunAdi = comboBox6.Text.Trim();

            int urunId =
                Convert.ToInt32(comboBox6.SelectedValue);

            string aciklama =
                textBox2.Text.Trim();

            // -------------------------------------------------
            // AYNI ÜRÜN VARSA MİKTARI ARTIR
            // -------------------------------------------------

            foreach (DataRow row in sevkiyatUrunleri.Rows)
            {
                int mevcutUrunId =
                    Convert.ToInt32(row["UrunId"]);

                if (mevcutUrunId == urunId)
                {
                    int mevcutMiktar =
                        Convert.ToInt32(row["Miktar"]);

                    row["Miktar"] =
                        mevcutMiktar + miktar;

                    sevkiyatBinding.ResetBindings(false);

                    comboBox6.SelectedIndex = -1;
                    textBox1.Clear();
                    comboBox6.Focus();

                    return;
                }
            }

            // -------------------------------------------------
            // YENİ SATIR
            // -------------------------------------------------

            DataRow yeniSatir =
                sevkiyatUrunleri.NewRow();

            yeniSatir["CikisDepo"] = cikisDepo;
            yeniSatir["CikisRaf"] = cikisRaf;
            yeniSatir["VarisDepo"] = varisDepo;
            yeniSatir["VarisRaf"] = varisRaf;
            yeniSatir["Tedarikci"] = tedarikci;
            yeniSatir["Durum"] = durum;
            yeniSatir["UrunId"] = urunId;
            yeniSatir["Ürün"] = urunAdi;
            yeniSatir["Miktar"] = miktar;
            yeniSatir["Açıklama"] = aciklama;

            sevkiyatUrunleri.Rows.Add(yeniSatir);

            // Ürün tablosuna tekrar dön
            dataGridView1.DataSource = sevkiyatBinding;

            sevkiyatBinding.ResetBindings(false);

            comboBox6.SelectedIndex = -1;
            textBox1.Clear();

            comboBox6.Focus();
        }

        // =========================================================
        // TEMİZLE - 2
        // =========================================================

        private void hopeButton2_Click_1(object sender, EventArgs e)
        {
            FormuTemizle();
        }

        // =========================================================
        // KAYDET - 2
        // =========================================================

        private void hopeButton1_Click_1(object sender, EventArgs e)
        {
            if (aktifSevkiyatId > 0)
            {
                MessageBox.Show(
                    "Bu sevkiyat zaten kaydedildi.\n\n" +
                    "Aynı sevkiyatı tekrar kaydetmek yerine " +
                    "Sevkiyatı Oluştur butonunu kullanınız.",
                    "Bilgi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                return;
            }

            if (comboBox1.SelectedValue == null ||
                comboBox1.SelectedValue is DataRowView)
            {
                MessageBox.Show(
                    "Lütfen çıkış deposunu seçiniz.",
                    "Uyarı",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                comboBox1.Focus();
                return;
            }

            if (comboBox2.SelectedValue == null ||
                comboBox2.SelectedValue is DataRowView)
            {
                MessageBox.Show(
                    "Lütfen çıkış rafını seçiniz.",
                    "Uyarı",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                comboBox2.Focus();
                return;
            }

            if (comboBox7.SelectedValue == null ||
                comboBox7.SelectedValue is DataRowView)
            {
                MessageBox.Show(
                    "Lütfen tedarikçi seçiniz.",
                    "Uyarı",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                comboBox7.Focus();
                return;
            }

            if (comboBox5.SelectedIndex == -1)
            {
                MessageBox.Show(
                    "Lütfen durum seçiniz.",
                    "Uyarı",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                comboBox5.Focus();
                return;
            }

            if (sevkiyatUrunleri == null ||
                sevkiyatUrunleri.Rows.Count == 0)
            {
                MessageBox.Show(
                    "Sevkiyata en az bir ürün eklemelisiniz.",
                    "Uyarı",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                comboBox6.Focus();
                return;
            }

            int cikisDepoId =
                Convert.ToInt32(comboBox1.SelectedValue);

            int cikisRafId =
                Convert.ToInt32(comboBox2.SelectedValue);

            int tedarikciId =
                Convert.ToInt32(comboBox7.SelectedValue);

            string durum =
                comboBox5.Text.Trim();

            string aciklama =
                textBox2.Text.Trim();

            int? varisDepoId = null;
            int? varisRafId = null;

            if (comboBox3.SelectedValue != null &&
                !(comboBox3.SelectedValue is DataRowView) &&
                comboBox3.SelectedIndex != -1)
            {
                varisDepoId =
                    Convert.ToInt32(comboBox3.SelectedValue);
            }

            if (comboBox4.SelectedValue != null &&
                !(comboBox4.SelectedValue is DataRowView) &&
                comboBox4.SelectedIndex != -1)
            {
                varisRafId =
                    Convert.ToInt32(comboBox4.SelectedValue);
            }

            if (varisDepoId.HasValue &&
                !varisRafId.HasValue)
            {
                MessageBox.Show(
                    "Varış deposu seçildiğinde varış rafını da seçiniz.",
                    "Uyarı",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                comboBox4.Focus();
                return;
            }

            if (!varisDepoId.HasValue &&
                varisRafId.HasValue)
            {
                MessageBox.Show(
                    "Varış rafı seçildiğinde varış deposunu da seçiniz.",
                    "Uyarı",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                comboBox3.Focus();
                return;
            }

            if (varisDepoId.HasValue &&
                varisRafId.HasValue)
            {
                if (cikisDepoId == varisDepoId.Value &&
                    cikisRafId == varisRafId.Value)
                {
                    MessageBox.Show(
                        "Çıkış ve varış depo/rafı aynı olamaz.",
                        "Uyarı",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);

                    return;
                }
            }

            using (SqlConnection conn = baglanti.Connection())
            using (SqlTransaction transaction =
                conn.BeginTransaction())
            {
                try
                {
                    // -------------------------------------------------
                    // SEVKİYAT
                    // -------------------------------------------------

                    string sqlSevkiyat = @"
                        INSERT INTO Sevkiyat
                        (
                            CikisDepoId,
                            VarisDepoId,
                            TedarikciId,
                            SevkiyatTarihi,
                            Durum,
                            Aciklama
                        )
                        VALUES
                        (
                            @CikisDepoId,
                            @VarisDepoId,
                            @TedarikciId,
                            GETDATE(),
                            @Durum,
                            @Aciklama
                        );

                        SELECT CAST(SCOPE_IDENTITY() AS INT);";

                    using (SqlCommand cmdSevkiyat =
                        new SqlCommand(
                            sqlSevkiyat,
                            conn,
                            transaction))
                    {
                        cmdSevkiyat.Parameters.Add(
                            "@CikisDepoId",
                            SqlDbType.Int).Value =
                            cikisDepoId;

                        cmdSevkiyat.Parameters.Add(
                            "@VarisDepoId",
                            SqlDbType.Int).Value =
                            (object)varisDepoId ??
                            DBNull.Value;

                        cmdSevkiyat.Parameters.Add(
                            "@TedarikciId",
                            SqlDbType.Int).Value =
                            tedarikciId;

                        cmdSevkiyat.Parameters.Add(
                            "@Durum",
                            SqlDbType.NVarChar,
                            50).Value =
                            durum;

                        cmdSevkiyat.Parameters.Add(
                            "@Aciklama",
                            SqlDbType.NVarChar,
                            300).Value =
                            string.IsNullOrWhiteSpace(aciklama)
                                ? (object)DBNull.Value
                                : aciklama;

                        aktifSevkiyatId =
                            Convert.ToInt32(
                                cmdSevkiyat.ExecuteScalar());
                    }

                    // -------------------------------------------------
                    // SEVKİYAT DETAY
                    // -------------------------------------------------

                    foreach (DataRow row in
                        sevkiyatUrunleri.Rows)
                    {
                        int urunId =
                            Convert.ToInt32(
                                row["UrunId"]);

                        int miktar =
                            Convert.ToInt32(
                                row["Miktar"]);

                        if (miktar <= 0)
                        {
                            throw new Exception(
                                "Ürün miktarı 0'dan büyük olmalıdır.");
                        }

                        string sqlDetay = @"
                            INSERT INTO SevkiyatDetay
                            (
                                SevkiyatId,
                                UrunId,
                                Miktar,
                                CikisRafId,
                                VarisRafId
                            )
                            VALUES
                            (
                                @SevkiyatId,
                                @UrunId,
                                @Miktar,
                                @CikisRafId,
                                @VarisRafId
                            );";

                        using (SqlCommand cmdDetay =
                            new SqlCommand(
                                sqlDetay,
                                conn,
                                transaction))
                        {
                            cmdDetay.Parameters.Add(
                                "@SevkiyatId",
                                SqlDbType.Int).Value =
                                aktifSevkiyatId;

                            cmdDetay.Parameters.Add(
                                "@UrunId",
                                SqlDbType.Int).Value =
                                urunId;

                            cmdDetay.Parameters.Add(
                                "@Miktar",
                                SqlDbType.Int).Value =
                                miktar;

                            cmdDetay.Parameters.Add(
                                "@CikisRafId",
                                SqlDbType.Int).Value =
                                cikisRafId;

                            cmdDetay.Parameters.Add(
                                "@VarisRafId",
                                SqlDbType.Int).Value =
                                (object)varisRafId ??
                                DBNull.Value;

                            cmdDetay.ExecuteNonQuery();
                        }
                    }

                    transaction.Commit();

                    MessageBox.Show(
                        "Sevkiyat başarıyla kaydedildi.\n\n" +
                        "Sevkiyat No: " +
                        aktifSevkiyatId +
                        "\n\n" +
                        "Stoklar henüz değiştirilmedi.",
                        "Başarılı",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);

                    // Ürün tablosunu temizle
                    sevkiyatUrunleri.Clear();
                    sevkiyatBinding.ResetBindings(false);
                }
                catch (Exception ex)
                {
                    try
                    {
                        transaction.Rollback();
                    }
                    catch
                    {
                    }

                    aktifSevkiyatId = 0;

                    MessageBox.Show(
                        "Sevkiyat kaydedilirken hata oluştu.\n\n" +
                        ex.Message,
                        "Hata",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
            }

            // Kayıtlı sevkiyatları tekrar göster
            SevkiyatlariGetir();
        }

        // =========================================================
        // FORM KONTROL
        // =========================================================

        private bool FormuKontrolEt()
        {
            if (comboBox1.SelectedValue == null ||
                comboBox1.SelectedValue is DataRowView)
            {
                MessageBox.Show(
                    "Çıkış deposunu seçiniz.",
                    "Uyarı",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                return false;
            }

            if (comboBox2.SelectedValue == null ||
                comboBox2.SelectedValue is DataRowView)
            {
                MessageBox.Show(
                    "Çıkış rafını seçiniz.",
                    "Uyarı",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                return false;
            }

            if (sevkiyatUrunleri == null ||
                sevkiyatUrunleri.Rows.Count == 0)
            {
                MessageBox.Show(
                    "Sevkiyata ürün ekleyiniz.",
                    "Uyarı",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                return false;
            }

            if (comboBox7.SelectedValue == null ||
                comboBox7.SelectedValue is DataRowView)
            {
                MessageBox.Show(
                    "Lütfen tedarikçi seçiniz.",
                    "Uyarı",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                comboBox7.Focus();
                return false;
            }

            return true;
        }

        // =========================================================
        // DATAGRIDVIEW CELL CONTENT CLICK
        // =========================================================

        private void dataGridView1_CellContentClick(
            object sender,
            DataGridViewCellEventArgs e)
        {
            // Silme işlemini CellClick yapıyor.
            // Burada tekrar işlem yapmıyoruz.
        }

        // =========================================================
        // SEVKİYATI OLUŞTUR / STOKLARI GÜNCELLE
        // =========================================================

        private void btnGiris_Click_1(
            object sender,
            EventArgs e)
        {
            if (sevkiyatUrunleri.Rows.Count == 0)
            {
                MessageBox.Show(
                    "Sevkiyata ürün ekleyiniz.",
                    "Uyarı",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                return;
            }

            if (aktifSevkiyatId == 0)
            {
                MessageBox.Show(
                    "Önce sevkiyatı Kaydet butonu ile kaydediniz.",
                    "Bilgi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                return;
            }

            using (SqlConnection conn = baglanti.Connection())
            using (SqlTransaction transaction =
                conn.BeginTransaction())
            {
                try
                {
                    int cikisDepoId =
                        Convert.ToInt32(
                            comboBox1.SelectedValue);

                    int cikisRafId =
                        Convert.ToInt32(
                            comboBox2.SelectedValue);

                    int? varisDepoId = null;
                    int? varisRafId = null;

                    if (comboBox3.SelectedValue != null &&
                        !(comboBox3.SelectedValue is DataRowView))
                    {
                        varisDepoId =
                            Convert.ToInt32(
                                comboBox3.SelectedValue);
                    }

                    if (comboBox4.SelectedValue != null &&
                        !(comboBox4.SelectedValue is DataRowView))
                    {
                        varisRafId =
                            Convert.ToInt32(
                                comboBox4.SelectedValue);
                    }

                    // -------------------------------------------------
                    // HER ÜRÜN
                    // -------------------------------------------------

                    foreach (DataRow row in
                        sevkiyatUrunleri.Rows)
                    {
                        int urunId =
                            Convert.ToInt32(
                                row["UrunId"]);

                        int miktar =
                            Convert.ToInt32(
                                row["Miktar"]);

                        // -------------------------------------------------
                        // KAYNAK STOK
                        // -------------------------------------------------

                        int mevcutStok = 0;

                        using (SqlCommand cmd =
                            new SqlCommand(
                                @"
                                SELECT MevcutMiktar
                                FROM DepoStok
                                WHERE DepoId = @DepoId
                                  AND RafId = @RafId
                                  AND UrunId = @UrunId",
                                conn,
                                transaction))
                        {
                            cmd.Parameters.Add(
                                "@DepoId",
                                SqlDbType.Int).Value =
                                cikisDepoId;

                            cmd.Parameters.Add(
                                "@RafId",
                                SqlDbType.Int).Value =
                                cikisRafId;

                            cmd.Parameters.Add(
                                "@UrunId",
                                SqlDbType.Int).Value =
                                urunId;

                            object sonuc =
                                cmd.ExecuteScalar();

                            if (sonuc == null)
                            {
                                throw new Exception(
                                    "Çıkış rafında ürün stok kaydı bulunamadı.");
                            }

                            mevcutStok =
                                Convert.ToInt32(sonuc);
                        }

                        if (mevcutStok < miktar)
                        {
                            throw new Exception(
                                "Yeterli stok bulunmuyor.\n\n" +
                                "Ürün ID: " + urunId +
                                "\nMevcut stok: " +
                                mevcutStok +
                                "\nSevk miktarı: " +
                                miktar);
                        }

                        // -------------------------------------------------
                        // KAYNAK STOKTAN DÜŞ
                        // -------------------------------------------------

                        using (SqlCommand cmd =
                            new SqlCommand(
                                @"
                                UPDATE DepoStok
                                SET MevcutMiktar =
                                    MevcutMiktar - @Miktar
                                WHERE DepoId = @DepoId
                                  AND RafId = @RafId
                                  AND UrunId = @UrunId",
                                conn,
                                transaction))
                        {
                            cmd.Parameters.Add(
                                "@Miktar",
                                SqlDbType.Int).Value =
                                miktar;

                            cmd.Parameters.Add(
                                "@DepoId",
                                SqlDbType.Int).Value =
                                cikisDepoId;

                            cmd.Parameters.Add(
                                "@RafId",
                                SqlDbType.Int).Value =
                                cikisRafId;

                            cmd.Parameters.Add(
                                "@UrunId",
                                SqlDbType.Int).Value =
                                urunId;

                            cmd.ExecuteNonQuery();
                        }

                        // -------------------------------------------------
                        // VARIŞ STOKU
                        // -------------------------------------------------

                        if (varisDepoId.HasValue &&
                            varisRafId.HasValue)
                        {
                            bool hedefVar = false;

                            using (SqlCommand cmd =
                                new SqlCommand(
                                    @"
                                    SELECT MevcutMiktar
                                    FROM DepoStok
                                    WHERE DepoId = @DepoId
                                      AND RafId = @RafId
                                      AND UrunId = @UrunId",
                                    conn,
                                    transaction))
                            {
                                cmd.Parameters.Add(
                                    "@DepoId",
                                    SqlDbType.Int).Value =
                                    varisDepoId.Value;

                                cmd.Parameters.Add(
                                    "@RafId",
                                    SqlDbType.Int).Value =
                                    varisRafId.Value;

                                cmd.Parameters.Add(
                                    "@UrunId",
                                    SqlDbType.Int).Value =
                                    urunId;

                                object sonuc =
                                    cmd.ExecuteScalar();

                                if (sonuc != null)
                                    hedefVar = true;
                            }

                            if (hedefVar)
                            {
                                using (SqlCommand cmd =
                                    new SqlCommand(
                                        @"
                                        UPDATE DepoStok
                                        SET MevcutMiktar =
                                            MevcutMiktar + @Miktar
                                        WHERE DepoId = @DepoId
                                          AND RafId = @RafId
                                          AND UrunId = @UrunId",
                                        conn,
                                        transaction))
                                {
                                    cmd.Parameters.Add(
                                        "@Miktar",
                                        SqlDbType.Int).Value =
                                        miktar;

                                    cmd.Parameters.Add(
                                        "@DepoId",
                                        SqlDbType.Int).Value =
                                        varisDepoId.Value;

                                    cmd.Parameters.Add(
                                        "@RafId",
                                        SqlDbType.Int).Value =
                                        varisRafId.Value;

                                    cmd.Parameters.Add(
                                        "@UrunId",
                                        SqlDbType.Int).Value =
                                        urunId;

                                    cmd.ExecuteNonQuery();
                                }
                            }
                            else
                            {
                                using (SqlCommand cmd =
                                    new SqlCommand(
                                        @"
                                        INSERT INTO DepoStok
                                        (
                                            DepoId,
                                            RafId,
                                            UrunId,
                                            MevcutMiktar
                                        )
                                        VALUES
                                        (
                                            @DepoId,
                                            @RafId,
                                            @UrunId,
                                            @Miktar
                                        )",
                                        conn,
                                        transaction))
                                {
                                    cmd.Parameters.Add(
                                        "@DepoId",
                                        SqlDbType.Int).Value =
                                        varisDepoId.Value;

                                    cmd.Parameters.Add(
                                        "@RafId",
                                        SqlDbType.Int).Value =
                                        varisRafId.Value;

                                    cmd.Parameters.Add(
                                        "@UrunId",
                                        SqlDbType.Int).Value =
                                        urunId;

                                    cmd.Parameters.Add(
                                        "@Miktar",
                                        SqlDbType.Int).Value =
                                        miktar;

                                    cmd.ExecuteNonQuery();
                                }
                            }
                        }
                    }

                    // -------------------------------------------------
                    // SEVKİYAT DURUMU
                    // -------------------------------------------------

                    using (SqlCommand cmd =
                        new SqlCommand(
                            @"
                            UPDATE Sevkiyat
                            SET Durum = 'Sevk Edildi'
                            WHERE SevkiyatId = @SevkiyatId",
                            conn,
                            transaction))
                    {
                        cmd.Parameters.Add(
                            "@SevkiyatId",
                            SqlDbType.Int).Value =
                            aktifSevkiyatId;

                        cmd.ExecuteNonQuery();
                    }

                    transaction.Commit();

                    MessageBox.Show(
                        "Sevkiyat başarıyla oluşturuldu ve stoklar güncellendi.\n\n" +
                        "Sevkiyat No: " +
                        aktifSevkiyatId,
                        "Başarılı",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);

                    aktifSevkiyatId = 0;

                    sevkiyatUrunleri.Clear();
                    sevkiyatBinding.ResetBindings(false);

                    comboBox1.SelectedIndex = -1;
                    comboBox2.DataSource = null;

                    comboBox3.SelectedIndex = -1;
                    comboBox4.DataSource = null;

                    comboBox5.SelectedIndex = -1;
                    comboBox6.SelectedIndex = -1;
                    comboBox7.SelectedIndex = -1;

                    textBox1.Clear();
                    textBox2.Clear();

                    // Güncel sevkiyat listesini göster
                    SevkiyatlariGetir();
                }
                catch (Exception ex)
                {
                    try
                    {
                        transaction.Rollback();
                    }
                    catch
                    {
                    }

                    MessageBox.Show(
                        "Sevkiyat oluşturulurken hata oluştu.\n\n" +
                        ex.Message,
                        "Hata",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
            }
        }

        // =========================================================
        // BOŞ EVENT
        // =========================================================

        private void comboBox5_SelectedIndexChanged(
            object sender,
            EventArgs e)
        {
        }
    }
}