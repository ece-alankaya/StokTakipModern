using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
using StokTakip_Modern.Sınıflar;

namespace StokTakip_Modern.Formlar
{
    public partial class frmTedarikci : Form
    {
        private readonly SqlBaglanti baglanti = new SqlBaglanti();

        public frmTedarikci()
        {
            InitializeComponent();
        }

        // =========================================================
        // FORM AÇILINCA
        // =========================================================

        private void frmTedarikci_Load(object sender, EventArgs e)
        {
            try
            {
                TedarikcileriGetir();
                DepolariGetir();

                comboBox2.DataSource = null;
                comboBox4.DataSource = null;

                textBox3.Text = DateTime.Now.ToString("dd.MM.yyyy HH:mm");
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Form yüklenirken hata oluştu:\n\n" + ex.Message,
                    "Hata",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        // =========================================================
        // TEDARİKÇİLER
        // =========================================================

        private void TedarikcileriGetir()
        {
            using (SqlConnection conn = baglanti.Connection())
            {
                string sql = @"
                    SELECT
                        TedarikciId,
                        FirmaAdi
                    FROM Tedarikci
                    WHERE AktifMi = 1
                    ORDER BY FirmaAdi";

                using (SqlDataAdapter da =
                    new SqlDataAdapter(sql, conn))
                {
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    comboBox1.DataSource = dt;
                    comboBox1.DisplayMember = "FirmaAdi";
                    comboBox1.ValueMember = "TedarikciId";
                    comboBox1.SelectedIndex = -1;
                }
            }
        }

        // =========================================================
        // TEDARİKÇİ DEĞİŞİNCE
        // =========================================================

        private void comboBox1_SelectedIndexChanged(
            object sender,
            EventArgs e)
        {
            if (comboBox1.SelectedValue == null ||
                comboBox1.SelectedValue is DataRowView)
            {
                comboBox2.DataSource = null;
                return;
            }

            try
            {
                int tedarikciId =
                    Convert.ToInt32(comboBox1.SelectedValue);

                TedarikciUrunleriGetir(tedarikciId);
            }
            catch
            {
                comboBox2.DataSource = null;
            }
        }

        private void TedarikciUrunleriGetir(int tedarikciId)
        {
            using (SqlConnection conn = baglanti.Connection())
            {
                string sql = @"
                    SELECT
                        u.UrunId,
                        u.UrunAdi
                    FROM TedarikciUrun tu
                    INNER JOIN Urun u
                        ON u.UrunId = tu.UrunId
                    WHERE tu.TedarikciId = @TedarikciId
                      AND u.AktifMi = 1
                    ORDER BY u.UrunAdi";

                using (SqlCommand cmd =
                    new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue(
                        "@TedarikciId",
                        tedarikciId);

                    using (SqlDataAdapter da =
                        new SqlDataAdapter(cmd))
                    {
                        DataTable dt = new DataTable();
                        da.Fill(dt);

                        comboBox2.DataSource = dt;
                        comboBox2.DisplayMember = "UrunAdi";
                        comboBox2.ValueMember = "UrunId";

                        if (dt.Rows.Count > 0)
                            comboBox2.SelectedIndex = -1;
                    }
                }
            }
        }

        // =========================================================
        // DEPOLAR
        // =========================================================

        private void DepolariGetir()
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

                using (SqlDataAdapter da =
                    new SqlDataAdapter(sql, conn))
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

        // =========================================================
        // DEPO DEĞİŞİNCE RAF
        // =========================================================

        private void comboBox3_SelectedIndexChanged(
            object sender,
            EventArgs e)
        {
            if (comboBox3.SelectedValue == null ||
                comboBox3.SelectedValue is DataRowView)
            {
                comboBox4.DataSource = null;
                return;
            }

            try
            {
                int depoId =
                    Convert.ToInt32(comboBox3.SelectedValue);

                RaflariGetir(depoId);
            }
            catch
            {
                comboBox4.DataSource = null;
            }
        }

        private void RaflariGetir(int depoId)
        {
            using (SqlConnection conn = baglanti.Connection())
            {
                string sql = @"
                    SELECT
                        RafId,
                        RafKodu
                    FROM Raf
                    WHERE DepoId = @DepoId
                      AND AktifMi = 1
                    ORDER BY RafKodu";

                using (SqlCommand cmd =
                    new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue(
                        "@DepoId",
                        depoId);

                    using (SqlDataAdapter da =
                        new SqlDataAdapter(cmd))
                    {
                        DataTable dt = new DataTable();
                        da.Fill(dt);

                        comboBox4.DataSource = dt;
                        comboBox4.DisplayMember = "RafKodu";
                        comboBox4.ValueMember = "RafId";

                        if (dt.Rows.Count > 0)
                            comboBox4.SelectedIndex = -1;
                    }
                }
            }
        }

        // =========================================================
        // KAYDET
        // =========================================================

        private void hopeButton1_Click(
            object sender,
            EventArgs e)
        {
            if (comboBox1.SelectedValue == null ||
                comboBox1.SelectedValue is DataRowView)
            {
                MessageBox.Show(
                    "Lütfen tedarikçi seçiniz.",
                    "Uyarı",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            if (comboBox2.SelectedValue == null ||
                comboBox2.SelectedValue is DataRowView)
            {
                MessageBox.Show(
                    "Lütfen ürün seçiniz.",
                    "Uyarı",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            if (!int.TryParse(
                textBox1.Text.Trim(),
                out int miktar) ||
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

            if (comboBox3.SelectedValue == null ||
                comboBox3.SelectedValue is DataRowView)
            {
                MessageBox.Show(
                    "Lütfen hedef depo seçiniz.",
                    "Uyarı",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            if (comboBox4.SelectedValue == null ||
                comboBox4.SelectedValue is DataRowView)
            {
                MessageBox.Show(
                    "Lütfen hedef raf seçiniz.",
                    "Uyarı",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            int tedarikciId =
                Convert.ToInt32(comboBox1.SelectedValue);

            int urunId =
                Convert.ToInt32(comboBox2.SelectedValue);

            int depoId =
                Convert.ToInt32(comboBox3.SelectedValue);

            int rafId =
                Convert.ToInt32(comboBox4.SelectedValue);

            DateTime islemTarihi;

            if (!DateTime.TryParse(
                textBox3.Text.Trim(),
                out islemTarihi))
            {
                islemTarihi = DateTime.Now;
            }

            string aciklama = textBox2.Text.Trim();

            using (SqlConnection conn = baglanti.Connection())
            {
                
                SqlTransaction transaction =
                    conn.BeginTransaction();

                try
                {
                    // -------------------------------------------------
                    // TEDARİKÇİ - ÜRÜN İLİŞKİSİ
                    // -------------------------------------------------

                    string kontrolSql = @"
                        SELECT COUNT(*)
                        FROM TedarikciUrun
                        WHERE TedarikciId = @TedarikciId
                          AND UrunId = @UrunId";

                    using (SqlCommand cmd =
                        new SqlCommand(
                            kontrolSql,
                            conn,
                            transaction))
                    {
                        cmd.Parameters.AddWithValue(
                            "@TedarikciId",
                            tedarikciId);

                        cmd.Parameters.AddWithValue(
                            "@UrunId",
                            urunId);

                        int sonuc =
                            Convert.ToInt32(cmd.ExecuteScalar());

                        if (sonuc == 0)
                        {
                            throw new Exception(
                                "Seçilen ürün bu tedarikçiye ait değil.");
                        }
                    }

                    // -------------------------------------------------
                    // DEPOSTOK KONTROL
                    // -------------------------------------------------

                    string stokKontrolSql = @"
                        SELECT COUNT(*)
                        FROM DepoStok
                        WHERE DepoId = @DepoId
                          AND RafId = @RafId
                          AND UrunId = @UrunId";

                    int stokVar;

                    using (SqlCommand cmd =
                        new SqlCommand(
                            stokKontrolSql,
                            conn,
                            transaction))
                    {
                        cmd.Parameters.AddWithValue(
                            "@DepoId",
                            depoId);

                        cmd.Parameters.AddWithValue(
                            "@RafId",
                            rafId);

                        cmd.Parameters.AddWithValue(
                            "@UrunId",
                            urunId);

                        stokVar =
                            Convert.ToInt32(cmd.ExecuteScalar());
                    }

                    // -------------------------------------------------
                    // DEPOSTOK GÜNCELLE
                    // -------------------------------------------------

                    if (stokVar > 0)
                    {
                        string sql = @"
                            UPDATE DepoStok
                            SET MevcutMiktar =
                                MevcutMiktar + @Miktar
                            WHERE DepoId = @DepoId
                              AND RafId = @RafId
                              AND UrunId = @UrunId";

                        using (SqlCommand cmd =
                            new SqlCommand(
                                sql,
                                conn,
                                transaction))
                        {
                            cmd.Parameters.AddWithValue(
                                "@Miktar",
                                miktar);

                            cmd.Parameters.AddWithValue(
                                "@DepoId",
                                depoId);

                            cmd.Parameters.AddWithValue(
                                "@RafId",
                                rafId);

                            cmd.Parameters.AddWithValue(
                                "@UrunId",
                                urunId);

                            cmd.ExecuteNonQuery();
                        }
                    }
                    else
                    {
                        string sql = @"
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
                            )";

                        using (SqlCommand cmd =
                            new SqlCommand(
                                sql,
                                conn,
                                transaction))
                        {
                            cmd.Parameters.AddWithValue(
                                "@DepoId",
                                depoId);

                            cmd.Parameters.AddWithValue(
                                "@RafId",
                                rafId);

                            cmd.Parameters.AddWithValue(
                                "@UrunId",
                                urunId);

                            cmd.Parameters.AddWithValue(
                                "@Miktar",
                                miktar);

                            cmd.ExecuteNonQuery();
                        }
                    }

                    // -------------------------------------------------
                    // STOK HAREKETİ
                    // 1 = Stok Girişi
                    // -------------------------------------------------

                    string hareketSql = @"
                        INSERT INTO StokHareket
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
                            1,
                            @Miktar,
                            @HareketTarihi,
                            @Aciklama
                        )";

                    using (SqlCommand cmd =
                        new SqlCommand(
                            hareketSql,
                            conn,
                            transaction))
                    {
                        cmd.Parameters.AddWithValue(
                            "@UrunId",
                            urunId);

                        cmd.Parameters.AddWithValue(
                            "@DepoId",
                            depoId);

                        cmd.Parameters.AddWithValue(
                            "@RafId",
                            rafId);

                        cmd.Parameters.AddWithValue(
                            "@Miktar",
                            miktar);

                        cmd.Parameters.AddWithValue(
                            "@HareketTarihi",
                            islemTarihi);

                        cmd.Parameters.AddWithValue(
                            "@Aciklama",
                            string.IsNullOrWhiteSpace(aciklama)
                                ? (object)DBNull.Value
                                : aciklama);

                        cmd.ExecuteNonQuery();
                    }

                    transaction.Commit();

                    MessageBox.Show(
                        "Tedarikçi ürün girişi başarıyla kaydedildi.",
                        "Başarılı",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);

                    Temizle();
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
                        "Kayıt sırasında hata oluştu:\n\n" +
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

        private void hopeButton2_Click(
            object sender,
            EventArgs e)
        {
            Temizle();
        }

        private void Temizle()
        {
            comboBox1.SelectedIndex = -1;
            comboBox2.DataSource = null;

            comboBox3.SelectedIndex = -1;
            comboBox4.DataSource = null;

            textBox1.Clear();
            textBox2.Clear();

            textBox3.Text =
                DateTime.Now.ToString("dd.MM.yyyy HH:mm");

            comboBox1.Focus();
        }

        private void monthCalendar1_DateChanged(object sender, DateRangeEventArgs e)
        {
          textBox3.Text= e.Start.ToString("dd.MM.yyyy");


        }

       
    }
}