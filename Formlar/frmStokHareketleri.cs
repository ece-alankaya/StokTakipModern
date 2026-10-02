using StokTakip_Modern.Sınıflar;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace StokTakip_Modern.Formlar
{
    public partial class frmStokHareketleri : Form
    {
        SqlBaglanti baglanti = new SqlBaglanti();

        public frmStokHareketleri()
        {
            InitializeComponent();

            Load += frmStokHareketleri_Load;
            hopeButton1.Click += hopeButton1_Click;
            hopeButton2.Click += hopeButton2_Click;
            comboBox3.SelectionChangeCommitted += comboBox3_SelectionChangeCommitted;
   
            comboBox1.SelectedIndexChanged += comboBox1_SelectedIndexChanged;
            comboBox5.SelectedIndexChanged += comboBox5_SelectedIndexChanged_1;
        }

        private void frmStokHareketleri_Load(object sender, EventArgs e)
        {
            HareketTurleriniGetir();
            UrunleriGetir();
            DepolariGetir();
            HareketleriGetir();

            comboBox5.Visible = false;
            comboBox6.Visible = false;

            labelHedefDepo.Visible = false;
            labelHedefRaf.Visible = false;

            textBox2.Text = DateTime.Now.ToString("dd.MM.yyyy HH:mm");
        }

        // =========================================================
        // HAREKET TÜRLERİ
        // =========================================================
        private void HareketTurleriniGetir()
        {
            try
            {
                using (SqlConnection conn = baglanti.Connection())
                {
                    string sql = @"
                        SELECT 
                            HareketTipiId,
                            HareketAdi
                        FROM HareketTipi
                        ORDER BY HareketAdi";

                    using (SqlDataAdapter da = new SqlDataAdapter(sql, conn))
                    {
                        DataTable dt = new DataTable();
                        da.Fill(dt);

                        comboBox1.DataSource = dt;
                        comboBox1.DisplayMember = "HareketAdi";
                        comboBox1.ValueMember = "HareketTipiId";
                        comboBox1.SelectedIndex = -1;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Hareket türleri yüklenirken hata oluştu.\n\n" +
                    ex.Message,
                    "Hata",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        // =========================================================
        // ÜRÜNLER
        // =========================================================

        private void UrunleriGetir()
        {
            try
            {
                using (SqlConnection conn = baglanti.Connection())
                {
                    string sql = @"
                        SELECT
                            UrunId,
                            UrunAdi
                        FROM Urun
                        WHERE AktifMi = 1
                        ORDER BY UrunAdi";

                    using (SqlDataAdapter da = new SqlDataAdapter(sql, conn))
                    {
                        DataTable dt = new DataTable();
                        da.Fill(dt);

                        comboBox2.DataSource = dt;
                        comboBox2.DisplayMember = "UrunAdi";
                        comboBox2.ValueMember = "UrunId";
                        comboBox2.SelectedIndex = -1;
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
        // DEPOLAR
        // =========================================================

        private void DepolariGetir()
        {
            try
            {
                using (SqlConnection conn = baglanti.Connection())
                {
                    string sql = @"
                        SELECT
                            DepoId,
                            DepoAdi
                        FROM Depo
                        WHERE AktifMi = 1
                        ORDER BY DepoAdi";

                    using (SqlDataAdapter da = new SqlDataAdapter(sql, conn))
                    {
                        DataTable dt = new DataTable();
                        da.Fill(dt);

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
        private void HedefAlanlariniHazirla()

        {
            HedefDepolariGetir();
            comboBox5.Visible = false;
            comboBox6.Visible = false;

            // Label isimlerini sende farklı verdiysen
            // aşağıdaki iki ismi kendi label isimlerinle değiştir.
            labelHedefDepo.Visible = false;
            labelHedefRaf.Visible = false;

            comboBox5.DataSource = null;
            comboBox6.DataSource = null;
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex == -1)
                return;

            if (comboBox1.SelectedValue == null ||
                comboBox1.SelectedValue is DataRowView)
                return;

            int hareketTipiId;

            if (!int.TryParse(
                comboBox1.SelectedValue.ToString(),
                out hareketTipiId))
                return;

            bool transferMi = hareketTipiId == 3;

            // Hedef depo ve rafı göster/gizle
            comboBox5.Visible = transferMi;
            comboBox6.Visible = transferMi;

            labelHedefDepo.Visible = transferMi;
            labelHedefRaf.Visible = transferMi;

            // Transfer seçildiyse hedef depoları doldur
            if (transferMi)
            {
                HedefDepolariGetir();
            }
            else
            {
                comboBox5.DataSource = null;
                comboBox6.DataSource = null;
            }
        }


        private void HedefDepolariGetir()
        {
            try
            {
                using (SqlConnection conn = baglanti.Connection())
                {
                    string sql = @"
                SELECT DepoId, DepoAdi
                FROM Depo
                ORDER BY DepoAdi";

                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        SqlDataAdapter da = new SqlDataAdapter(cmd);

                        DataTable dt = new DataTable();
                        da.Fill(dt);

                        comboBox5.DataSource = null;

                        comboBox5.DisplayMember = "DepoAdi";
                        comboBox5.ValueMember = "DepoId";
                        comboBox5.DataSource = dt;

                        comboBox5.SelectedIndex = -1;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Hedef depolar yüklenirken hata oluştu.\n\n" +
                    ex.Message,
                    "Hata",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }

        }



        // =========================================================
        // DEPO DEĞİŞİNCE RAFLARI GETİR
        // =========================================================
        private void comboBox3_SelectionChangeCommitted(object sender, EventArgs e)
        {
            if (comboBox3.SelectedIndex == -1)
            {
                comboBox4.DataSource = null;
                return;
            }

            try
            {
                int depoId = Convert.ToInt32(comboBox3.SelectedValue);

                using (SqlConnection conn = baglanti.Connection())
                {
                    string sql = @"
                SELECT 
                    RafId,
                    RafKodu
                FROM Raf
                WHERE DepoId = @DepoId
                ORDER BY RafKodu";

                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.Add("@DepoId", SqlDbType.Int).Value = depoId;

                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            da.Fill(dt);

                            comboBox4.DataSource = null;

                            comboBox4.DisplayMember = "RafKodu";
                            comboBox4.ValueMember = "RafId";
                            comboBox4.DataSource = dt;

                            comboBox4.SelectedIndex = -1;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Raflar yüklenirken hata oluştu.\n\n" + ex.Message,
                    "Hata",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        // =========================================================
        // STOK HAREKETLERİ
        // =========================================================

        private void HareketleriGetir()
        {
            try
            {
                using (SqlConnection conn = baglanti.Connection())
                {
                    string sql = @"
                        SELECT
                            sh.HareketId AS [ID],
                            u.UrunAdi AS [Ürün],
                            ht.HareketAdi AS [Hareket Türü],
                            sh.Miktar AS [Miktar],
                            d.DepoAdi AS [Depo],
                            r.RafKodu AS [Raf],
                            sh.HareketTarihi AS [Tarih],
                            sh.Aciklama AS [Açıklama]
                        FROM StokHareket sh

                        INNER JOIN Urun u
                            ON sh.UrunId = u.UrunId

                        INNER JOIN HareketTipi ht
                            ON sh.HareketTipiId = ht.HareketTipiId

                        INNER JOIN Depo d
                            ON sh.DepoId = d.DepoId

                        LEFT JOIN Raf r
                            ON sh.RafId = r.RafId

                        ORDER BY sh.HareketTarihi DESC";

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
                    "Stok hareketleri yüklenirken hata oluştu.\n\n" +
                    ex.Message,
                    "Hata",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        // =========================================================
        // KAYDET
        // =========================================================

        private void hopeButton1_Click(object sender, EventArgs e)
        {
            
                if (comboBox1.SelectedValue == null ||
    comboBox1.SelectedValue is DataRowView)
                {
                    return;
                }
            

            if (comboBox2.SelectedIndex == -1)
            {
                MessageBox.Show(
                    "Lütfen ürün seçiniz.",
                    "Uyarı",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                comboBox2.Focus();
                return;
            }

            if (comboBox3.SelectedIndex == -1)
            {
                MessageBox.Show(
                    "Lütfen depo seçiniz.",
                    "Uyarı",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                comboBox3.Focus();
                return;
            }

            if (comboBox4.SelectedIndex == -1)
            {
                MessageBox.Show(
                    "Lütfen raf seçiniz.",
                    "Uyarı",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                comboBox4.Focus();
                return;
            }

            decimal miktar;

            if (!decimal.TryParse(textBox1.Text.Trim(), out miktar) || miktar <= 0)
            {
                MessageBox.Show(
                    "Lütfen 0'dan büyük geçerli bir miktar giriniz.",
                    "Uyarı",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                textBox1.Focus();
                return;
            }

            DateTime hareketTarihi;

            if (!DateTime.TryParse(textBox2.Text.Trim(), out hareketTarihi))
            {
                MessageBox.Show(
                    "Geçerli bir tarih giriniz.",
                    "Uyarı",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                textBox2.Focus();
                return;
            }

            int hareketTipiId = Convert.ToInt32(comboBox1.SelectedValue);
            int urunId = Convert.ToInt32(comboBox2.SelectedValue);
            int depoId = Convert.ToInt32(comboBox3.SelectedValue);
            int rafId = Convert.ToInt32(comboBox4.SelectedValue);
            if (hareketTipiId == 3)
            {
                TransferKaydet(
                    hareketTipiId,
                    urunId,
                    depoId,
                    rafId);

                return;
            }

            string aciklama = richTextBox1.Text.Trim();


            // ---------------------------------------------------------
            // 2. VERİTABANI İŞLEMİ
            // ---------------------------------------------------------

            using (SqlConnection conn = baglanti.Connection())
            using (SqlTransaction transaction = conn.BeginTransaction())
            {
                try
                {
                    // Hareket tipinin adını buluyoruz.
                    string hareketAdi = "";

                    using (SqlCommand cmdTip = new SqlCommand(
                        @"SELECT HareketAdi
                  FROM HareketTipi
                  WHERE HareketTipiId = @HareketTipiId",
                        conn,
                        transaction))
                    {
                        cmdTip.Parameters.Add("@HareketTipiId", SqlDbType.Int)
                            .Value = hareketTipiId;

                        object sonuc = cmdTip.ExecuteScalar();

                        if (sonuc == null)
                        {
                            throw new Exception("Seçilen hareket türü bulunamadı.");
                        }

                        hareketAdi = sonuc.ToString();
                    }

                    // -------------------------------------------------
                    // 3. DEPOSTAKİ MEVCUT STOK
                    // -------------------------------------------------

                    decimal mevcutMiktar = 0;

                    using (SqlCommand cmdStok = new SqlCommand(
                        @"SELECT MevcutMiktar
                  FROM DepoStok
                  WHERE DepoId = @DepoId
                    AND RafId = @RafId
                    AND UrunId = @UrunId",
                        conn,
                        transaction))
                    {
                        cmdStok.Parameters.Add("@DepoId", SqlDbType.Int).Value = depoId;
                        cmdStok.Parameters.Add("@RafId", SqlDbType.Int).Value = rafId;
                        cmdStok.Parameters.Add("@UrunId", SqlDbType.Int).Value = urunId;

                        object sonuc = cmdStok.ExecuteScalar();

                        if (sonuc != null && sonuc != DBNull.Value)
                        {
                            mevcutMiktar = Convert.ToDecimal(sonuc);
                        }
                    }

                    // -------------------------------------------------
                    // 4. HAREKET TÜRÜNE GÖRE STOK HESAPLA
                    // -------------------------------------------------

                    decimal yeniMiktar = mevcutMiktar;

                    if (hareketAdi == "Stok Girişi" ||
                        hareketAdi == "İade Girişi")
                    {
                        yeniMiktar = mevcutMiktar + miktar;
                    }
                    else if (hareketAdi == "Stok Çıkışı")
                    {
                        if (mevcutMiktar < miktar)
                        {
                            MessageBox.Show(
                                "Yeterli stok bulunmuyor.\n\n" +
                                "Mevcut stok: " + mevcutMiktar + "\n" +
                                "Çıkış miktarı: " + miktar,
                                "Yetersiz Stok",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning);

                            transaction.Rollback();
                            return;
                        }

                        yeniMiktar = mevcutMiktar - miktar;
                    }
                    else
                    {
                        MessageBox.Show(
                            "Bu hareket türü şu an bu form üzerinden işlenmiyor:\n\n" +
                            hareketAdi,
                            "Bilgi",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information);

                        transaction.Rollback();
                        return;
                    }

                    // -------------------------------------------------
                    // 5. DEPOSTOK GÜNCELLE / OLUŞTUR
                    // -------------------------------------------------

                    if (mevcutMiktar == 0)
                    {
                        // Kayıt var mı gerçekten kontrol ediyoruz.
                        int stokKayitSayisi = 0;

                        using (SqlCommand cmdKontrol = new SqlCommand(
                            @"SELECT COUNT(*)
                      FROM DepoStok
                      WHERE DepoId = @DepoId
                        AND RafId = @RafId
                        AND UrunId = @UrunId",
                            conn,
                            transaction))
                        {
                            cmdKontrol.Parameters.Add("@DepoId", SqlDbType.Int).Value = depoId;
                            cmdKontrol.Parameters.Add("@RafId", SqlDbType.Int).Value = rafId;
                            cmdKontrol.Parameters.Add("@UrunId", SqlDbType.Int).Value = urunId;

                            stokKayitSayisi = Convert.ToInt32(
                                cmdKontrol.ExecuteScalar());
                        }

                        if (stokKayitSayisi == 0)
                        {
                            using (SqlCommand cmdInsertStok = new SqlCommand(
                                @"INSERT INTO DepoStok
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
                              @MevcutMiktar
                          )",
                                conn,
                                transaction))
                            {
                                cmdInsertStok.Parameters.Add("@DepoId", SqlDbType.Int)
                                    .Value = depoId;

                                cmdInsertStok.Parameters.Add("@RafId", SqlDbType.Int)
                                    .Value = rafId;

                                cmdInsertStok.Parameters.Add("@UrunId", SqlDbType.Int)
                                    .Value = urunId;

                                cmdInsertStok.Parameters.Add("@MevcutMiktar", SqlDbType.Decimal)
                                    .Value = yeniMiktar;

                                cmdInsertStok.ExecuteNonQuery();
                            }
                        }
                        else
                        {
                            using (SqlCommand cmdUpdateStok = new SqlCommand(
                                @"UPDATE DepoStok
                          SET MevcutMiktar = @MevcutMiktar
                          WHERE DepoId = @DepoId
                            AND RafId = @RafId
                            AND UrunId = @UrunId",
                                conn,
                                transaction))
                            {
                                cmdUpdateStok.Parameters.Add("@MevcutMiktar", SqlDbType.Decimal)
                                    .Value = yeniMiktar;

                                cmdUpdateStok.Parameters.Add("@DepoId", SqlDbType.Int)
                                    .Value = depoId;

                                cmdUpdateStok.Parameters.Add("@RafId", SqlDbType.Int)
                                    .Value = rafId;

                                cmdUpdateStok.Parameters.Add("@UrunId", SqlDbType.Int)
                                    .Value = urunId;

                                cmdUpdateStok.ExecuteNonQuery();
                            }
                        }
                    }
                    else
                    {
                        using (SqlCommand cmdUpdateStok = new SqlCommand(
                            @"UPDATE DepoStok
                      SET MevcutMiktar = @MevcutMiktar
                      WHERE DepoId = @DepoId
                        AND RafId = @RafId
                        AND UrunId = @UrunId",
                            conn,
                            transaction))
                        {
                            cmdUpdateStok.Parameters.Add("@MevcutMiktar", SqlDbType.Decimal)
                                .Value = yeniMiktar;

                            cmdUpdateStok.Parameters.Add("@DepoId", SqlDbType.Int)
                                .Value = depoId;

                            cmdUpdateStok.Parameters.Add("@RafId", SqlDbType.Int)
                                .Value = rafId;

                            cmdUpdateStok.Parameters.Add("@UrunId", SqlDbType.Int)
                                .Value = urunId;

                            cmdUpdateStok.ExecuteNonQuery();
                        }
                    }

                    // -------------------------------------------------
                    // 6. STOK HAREKETİ EKLE
                    // -------------------------------------------------

                    using (SqlCommand cmdHareket = new SqlCommand(
                        @"INSERT INTO StokHareket
                  (
                      UrunId,
                      DepoId,
                      RafId,
                      HareketTipiId,
                      Miktar,
                      HareketTarihi,
                      Aciklama
                  )
                  VALUES
                  (
                      @UrunId,
                      @DepoId,
                      @RafId,
                      @HareketTipiId,
                      @Miktar,
                      @HareketTarihi,
                      @Aciklama
                  )",
                        conn,
                        transaction))
                    {
                        cmdHareket.Parameters.Add("@UrunId", SqlDbType.Int)
                            .Value = urunId;

                        cmdHareket.Parameters.Add("@DepoId", SqlDbType.Int)
                            .Value = depoId;

                        cmdHareket.Parameters.Add("@RafId", SqlDbType.Int)
                            .Value = rafId;

                        cmdHareket.Parameters.Add("@HareketTipiId", SqlDbType.Int)
                            .Value = hareketTipiId;

                        cmdHareket.Parameters.Add("@Miktar", SqlDbType.Decimal)
                            .Value = miktar;

                        cmdHareket.Parameters.Add("@HareketTarihi", SqlDbType.DateTime)
                            .Value = hareketTarihi;

                        cmdHareket.Parameters.Add("@Aciklama", SqlDbType.NVarChar)
                            .Value = string.IsNullOrWhiteSpace(aciklama)
                                ? (object)DBNull.Value
                                : aciklama;

                        cmdHareket.ExecuteNonQuery();
                    }

                    // -------------------------------------------------
                    // 7. HER ŞEY BAŞARILI
                    // -------------------------------------------------

                    transaction.Commit();

                    MessageBox.Show(
                        "Stok hareketi başarıyla kaydedildi.",
                        "Başarılı",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);

                    HareketleriGetir();

                    // Formu temizle
                    comboBox1.SelectedIndex = -1;
                    comboBox2.SelectedIndex = -1;
                    comboBox3.SelectedIndex = -1;
                    comboBox4.DataSource = null;

                    textBox1.Clear();
                    textBox2.Text = DateTime.Now.ToString("dd.MM.yyyy HH:mm");
                    richTextBox1.Clear();

                    comboBox1.Focus();
                }
                catch (Exception ex)
                {
                    try
                    {
                        transaction.Rollback();
                    }
                    catch
                    {
                        // Rollback sırasında oluşan hata burada ayrıca gösterilmiyor.
                    }

                    MessageBox.Show(
                        "Stok hareketi kaydedilirken hata oluştu.\n\n" +
                        ex.Message,
                        "Hata",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
            }
        }


        // =========================================================
        // TEMİZLE
        // =========================================================

        private void hopeButton2_Click(object sender, EventArgs e)
        {
            comboBox1.SelectedIndex = -1;
            comboBox2.SelectedIndex = -1;

            comboBox4.DataSource = null;

            comboBox3.SelectedIndex = -1;

            textBox1.Clear();

            textBox2.Clear();

            richTextBox1.Clear();

            comboBox1.Focus();
        }
        private void TransferKaydet(
    int hareketTipiId,
    int urunId,
    int kaynakDepoId,
    int kaynakRafId)
        {
            // ---------------------------------------------------------
            // HEDEF ALAN KONTROLLERİ
            // ---------------------------------------------------------

            if (comboBox5.SelectedIndex == -1)
            {
                MessageBox.Show(
                    "Lütfen hedef depo seçiniz.",
                    "Uyarı",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                comboBox5.Focus();
                return;
            }

            if (comboBox6.SelectedIndex == -1)
            {
                MessageBox.Show(
                    "Lütfen hedef raf seçiniz.",
                    "Uyarı",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                comboBox6.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(textBox1.Text))
            {
                MessageBox.Show(
                    "Lütfen transfer miktarı giriniz.",
                    "Uyarı",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                textBox1.Focus();
                return;
            }

            int miktar;

            if (!int.TryParse(textBox1.Text.Trim(), out miktar) ||
                miktar <= 0)
            {
                MessageBox.Show(
                    "Transfer miktarı 0'dan büyük tam sayı olmalıdır.",
                    "Uyarı",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                textBox1.Focus();
                return;
            }

            int hedefDepoId =
                Convert.ToInt32(comboBox5.SelectedValue);

            int hedefRafId =
                Convert.ToInt32(comboBox6.SelectedValue);

            DateTime hareketTarihi;

            if (!DateTime.TryParse(
                textBox2.Text.Trim(),
                out hareketTarihi))
            {
                hareketTarihi = DateTime.Now;
            }

            string aciklama = richTextBox1.Text.Trim();

            // Aynı depo ve aynı raf olamaz
            if (kaynakDepoId == hedefDepoId &&
                kaynakRafId == hedefRafId)
            {
                MessageBox.Show(
                    "Kaynak ve hedef depo/raf aynı olamaz.",
                    "Uyarı",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                return;
            }

            // ---------------------------------------------------------
            // TRANSACTION
            // ---------------------------------------------------------

            using (SqlConnection conn = baglanti.Connection())
            using (SqlTransaction transaction = conn.BeginTransaction())
            {
                try
                {
                    // -------------------------------------------------
                    // 1. KAYNAK STOK MİKTARINI BUL
                    // -------------------------------------------------

                    int kaynakMiktar = 0;

                    using (SqlCommand cmd = new SqlCommand(
                        @"SELECT MevcutMiktar
                  FROM DepoStok
                  WHERE DepoId = @DepoId
                    AND RafId = @RafId
                    AND UrunId = @UrunId",
                        conn,
                        transaction))
                    {
                        cmd.Parameters.Add("@DepoId", SqlDbType.Int)
                            .Value = kaynakDepoId;

                        cmd.Parameters.Add("@RafId", SqlDbType.Int)
                            .Value = kaynakRafId;

                        cmd.Parameters.Add("@UrunId", SqlDbType.Int)
                            .Value = urunId;

                        object sonuc = cmd.ExecuteScalar();

                        if (sonuc == null)
                        {
                            MessageBox.Show(
                                "Kaynak depoda bu ürün için stok kaydı bulunamadı.",
                                "Uyarı",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning);

                            transaction.Rollback();
                            return;
                        }

                        kaynakMiktar = Convert.ToInt32(sonuc);
                    }

                    // -------------------------------------------------
                    // 2. YETERLİ STOK VAR MI?
                    // -------------------------------------------------

                    if (kaynakMiktar < miktar)
                    {
                        MessageBox.Show(
                            "Transfer için yeterli stok bulunmuyor.\n\n" +
                            "Mevcut stok: " + kaynakMiktar + "\n" +
                            "Transfer miktarı: " + miktar,
                            "Yetersiz Stok",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning);

                        transaction.Rollback();
                        return;
                    }

                    int yeniKaynakMiktar =
                        kaynakMiktar - miktar;

                    // -------------------------------------------------
                    // 3. KAYNAK STOKTAN DÜŞ
                    // -------------------------------------------------

                    using (SqlCommand cmd = new SqlCommand(
                        @"UPDATE DepoStok
                  SET MevcutMiktar = @Miktar
                  WHERE DepoId = @DepoId
                    AND RafId = @RafId
                    AND UrunId = @UrunId",
                        conn,
                        transaction))
                    {
                        cmd.Parameters.Add("@Miktar", SqlDbType.Int)
                            .Value = yeniKaynakMiktar;

                        cmd.Parameters.Add("@DepoId", SqlDbType.Int)
                            .Value = kaynakDepoId;

                        cmd.Parameters.Add("@RafId", SqlDbType.Int)
                            .Value = kaynakRafId;

                        cmd.Parameters.Add("@UrunId", SqlDbType.Int)
                            .Value = urunId;

                        cmd.ExecuteNonQuery();
                    }

                    // -------------------------------------------------
                    // 4. HEDEF STOK VAR MI?
                    // -------------------------------------------------

                    int hedefMiktar = 0;
                    bool hedefKayitVar = false;

                    using (SqlCommand cmd = new SqlCommand(
                        @"SELECT MevcutMiktar
                  FROM DepoStok
                  WHERE DepoId = @DepoId
                    AND RafId = @RafId
                    AND UrunId = @UrunId",
                        conn,
                        transaction))
                    {
                        cmd.Parameters.Add("@DepoId", SqlDbType.Int)
                            .Value = hedefDepoId;

                        cmd.Parameters.Add("@RafId", SqlDbType.Int)
                            .Value = hedefRafId;

                        cmd.Parameters.Add("@UrunId", SqlDbType.Int)
                            .Value = urunId;

                        object sonuc = cmd.ExecuteScalar();

                        if (sonuc != null)
                        {
                            hedefKayitVar = true;
                            hedefMiktar = Convert.ToInt32(sonuc);
                        }
                    }

                    int yeniHedefMiktar =
                        hedefMiktar + miktar;

                    // -------------------------------------------------
                    // 5. HEDEF STOK GÜNCELLE / OLUŞTUR
                    // -------------------------------------------------

                    if (hedefKayitVar)
                    {
                        using (SqlCommand cmd = new SqlCommand(
                            @"UPDATE DepoStok
                      SET MevcutMiktar = @Miktar
                      WHERE DepoId = @DepoId
                        AND RafId = @RafId
                        AND UrunId = @UrunId",
                            conn,
                            transaction))
                        {
                            cmd.Parameters.Add("@Miktar", SqlDbType.Int)
                                .Value = yeniHedefMiktar;

                            cmd.Parameters.Add("@DepoId", SqlDbType.Int)
                                .Value = hedefDepoId;

                            cmd.Parameters.Add("@RafId", SqlDbType.Int)
                                .Value = hedefRafId;

                            cmd.Parameters.Add("@UrunId", SqlDbType.Int)
                                .Value = urunId;

                            cmd.ExecuteNonQuery();
                        }
                    }
                    else
                    {
                        using (SqlCommand cmd = new SqlCommand(
                            @"INSERT INTO DepoStok
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
                            cmd.Parameters.Add("@DepoId", SqlDbType.Int)
                                .Value = hedefDepoId;

                            cmd.Parameters.Add("@RafId", SqlDbType.Int)
                                .Value = hedefRafId;

                            cmd.Parameters.Add("@UrunId", SqlDbType.Int)
                                .Value = urunId;

                            cmd.Parameters.Add("@Miktar", SqlDbType.Int)
                                .Value = miktar;

                            cmd.ExecuteNonQuery();
                        }
                    }

                    // -------------------------------------------------
                    // 6. TRANSFER HAREKETİ KAYDI
                    // -------------------------------------------------

                    string transferAciklama =
                        string.IsNullOrWhiteSpace(aciklama)
                        ? "Depolar arası transfer"
                        : aciklama;

                    transferAciklama +=
                        " | Hedef Depo ID: " + hedefDepoId +
                        ", Hedef Raf ID: " + hedefRafId;

                    using (SqlCommand cmd = new SqlCommand(
                        @"INSERT INTO StokHareket
                  (
                      UrunId,
                      DepoId,
                      RafId,
                      HareketTipiId,
                      Miktar,
                      HareketTarihi,
                      Aciklama
                  )
                  VALUES
                  (
                      @UrunId,
                      @DepoId,
                      @RafId,
                      @HareketTipiId,
                      @Miktar,
                      @HareketTarihi,
                      @Aciklama
                  )",
                        conn,
                        transaction))
                    {
                        cmd.Parameters.Add("@UrunId", SqlDbType.Int)
                            .Value = urunId;

                        cmd.Parameters.Add("@DepoId", SqlDbType.Int)
                            .Value = kaynakDepoId;

                        cmd.Parameters.Add("@RafId", SqlDbType.Int)
                            .Value = kaynakRafId;

                        cmd.Parameters.Add("@HareketTipiId", SqlDbType.Int)
                            .Value = hareketTipiId;

                        cmd.Parameters.Add("@Miktar", SqlDbType.Int)
                            .Value = miktar;

                        cmd.Parameters.Add("@HareketTarihi", SqlDbType.DateTime)
                            .Value = hareketTarihi;

                        cmd.Parameters.Add("@Aciklama", SqlDbType.NVarChar)
                            .Value = transferAciklama;

                        cmd.ExecuteNonQuery();
                    }

                    // -------------------------------------------------
                    // 7. HER ŞEY BAŞARILI
                    // -------------------------------------------------

                    transaction.Commit();

                    MessageBox.Show(
                        "Depolar arası transfer başarıyla gerçekleştirildi.",
                        "Başarılı",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);

                    HareketleriGetir();

                    // Formu temizle
                    comboBox1.SelectedIndex = -1;
                    comboBox2.SelectedIndex = -1;
                    comboBox3.SelectedIndex = -1;
                    comboBox4.DataSource = null;

                    comboBox5.SelectedIndex = -1;
                    comboBox6.DataSource = null;

                    textBox1.Clear();
                    textBox2.Text =
                        DateTime.Now.ToString("dd.MM.yyyy HH:mm");

                    richTextBox1.Clear();

                    comboBox1.Focus();
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
                        "Transfer sırasında hata oluştu.\n\n" +
                        ex.Message,
                        "Hata",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
            }
        }

        private void labelEdit8_Click(object sender, EventArgs e)
        {
        }

        private void comboBox4_SelectedIndexChanged(object sender, EventArgs e)
        {
        }

        private void labelEdit11_Click(object sender, EventArgs e)
        {

        }
        
           
        private void comboBox5_SelectedIndexChanged_1(object sender, EventArgs e)
        {

            if (comboBox5.SelectedIndex == -1)
                return;

            if (comboBox5.SelectedValue == null ||
                comboBox5.SelectedValue is DataRowView)
                return;

            try
            {
                int hedefDepoId =
                    Convert.ToInt32(comboBox5.SelectedValue);

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
                        cmd.Parameters.Add("@DepoId", SqlDbType.Int)
                            .Value = hedefDepoId;

                        SqlDataAdapter da =
                            new SqlDataAdapter(cmd);

                        DataTable dt = new DataTable();
                        da.Fill(dt);

                        comboBox6.DataSource = null;

                        comboBox6.DisplayMember = "RafKodu";
                        comboBox6.ValueMember = "RafId";
                        comboBox6.DataSource = dt;

                        comboBox6.SelectedIndex = -1;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Hedef raflar yüklenirken hata oluştu.\n\n" +
                    ex.Message,
                    "Hata",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

    }
    } 


    