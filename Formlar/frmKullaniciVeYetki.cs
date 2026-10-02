using StokTakip_Modern.Sınıflar;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace StokTakip_Modern.Formlar
{
    public partial class frmKullaniciVeYetki : Form
    {
        SqlBaglanti baglanti = new SqlBaglanti();

        public frmKullaniciVeYetki()
        {
            InitializeComponent();

            Load += frmKullaniciVeYetki_Load;

            button1.Click += button1_Click;
            button2.Click += button2_Click;
            button3.Click += button3_Click;
            button4.Click += button4_Click;

            comboBox2.SelectedIndexChanged += comboBox2_SelectedIndexChanged;

            // Designer'da zaten bağlı olduğu için
            // comboBox1 event'ini burada tekrar bağlamıyoruz.
        }

        // =========================================================
        // FORM LOAD
        // =========================================================

        private void frmKullaniciVeYetki_Load(object sender, EventArgs e)
        {
            // Şifre alanları
            textBox4.UseSystemPasswordChar = true;
            textBox5.UseSystemPasswordChar = true;

            // Durum
            comboBox3.SelectedIndex = 0;

            // Verileri getir
            RolleriGetir();
            KullanicilariGetir();

            // Sağ paneli temizle
            labelEdit13.Text = ".";
            labelEdit14.Text = ".";

            YetkileriTemizle();

            // İlk alan
            textBox1.Focus();
        }

        // =========================================================
        // ROLLER
        // comboBox1
        // =========================================================

        private void RolleriGetir()
        {
            try
            {
                using (SqlConnection conn = baglanti.Connection())
                {
                    string sql = @"
                        SELECT
                            YetkiId,
                            YetkiAdi
                        FROM Yetki
                        ORDER BY YetkiAdi";

                    using (SqlDataAdapter da =
                        new SqlDataAdapter(sql, conn))
                    {
                        DataTable dt = new DataTable();
                        da.Fill(dt);

                        comboBox1.DataSource = dt;
                        comboBox1.DisplayMember = "YetkiAdi";
                        comboBox1.ValueMember = "YetkiId";
                        comboBox1.SelectedIndex = -1;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Roller yüklenirken hata oluştu.\n\n" +
                    ex.Message,
                    "Hata",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        // =========================================================
        // KULLANICILAR
        // comboBox2
        // =========================================================

        private void KullanicilariGetir()
        {
            try
            {
                using (SqlConnection conn = baglanti.Connection())
                {
                    string sql = @"
                        SELECT
                            KullaniciId,
                            KullaniciAdi
                        FROM Kullanici
                        ORDER BY KullaniciAdi";

                    using (SqlDataAdapter da =
                        new SqlDataAdapter(sql, conn))
                    {
                        DataTable dt = new DataTable();
                        da.Fill(dt);

                        comboBox2.DataSource = dt;
                        comboBox2.DisplayMember = "KullaniciAdi";
                        comboBox2.ValueMember = "KullaniciId";
                        comboBox2.SelectedIndex = -1;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Kullanıcılar yüklenirken hata oluştu.\n\n" +
                    ex.Message,
                    "Hata",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        // =========================================================
        // KULLANICI SEÇİLİNCE
        // =========================================================

        private void comboBox2_SelectedIndexChanged(
            object sender,
            EventArgs e)
        {
            if (comboBox2.SelectedValue == null ||
                comboBox2.SelectedValue is DataRowView)
            {
                return;
            }

            try
            {
                int kullaniciId =
                    Convert.ToInt32(comboBox2.SelectedValue);

                KullaniciBilgileriniGetir(kullaniciId);
                YetkileriGetir(kullaniciId);
            }
            catch
            {
                // ComboBox ilk yüklenirken oluşabilecek
                // geçici dönüşümler için.
            }
        }

        // =========================================================
        // SEÇİLİ KULLANICI BİLGİLERİ
        // =========================================================

        private void KullaniciBilgileriniGetir(int kullaniciId)
        {
            using (SqlConnection conn = baglanti.Connection())
            {
                string sql = @"
                    SELECT
                        k.Ad,
                        k.Soyad,
                        k.AktifMi,
                        y.YetkiAdi
                    FROM Kullanici k
                    LEFT JOIN KullaniciYetki ky
                        ON k.KullaniciId = ky.KullaniciId
                    LEFT JOIN Yetki y
                        ON ky.YetkiId = y.YetkiId
                    WHERE k.KullaniciId = @KullaniciId";

                using (SqlCommand cmd =
                    new SqlCommand(sql, conn))
                {
                    cmd.Parameters.Add(
                        "@KullaniciId",
                        SqlDbType.Int).Value = kullaniciId;

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            labelEdit13.Text =
                                dr["Ad"].ToString() +
                                " " +
                                dr["Soyad"].ToString();

                            labelEdit14.Text =
                                dr["YetkiAdi"] == DBNull.Value
                                    ? "-"
                                    : dr["YetkiAdi"].ToString();

                            comboBox3.SelectedItem =
                                Convert.ToBoolean(dr["AktifMi"])
                                    ? "Aktif"
                                    : "Pasif";
                        }
                    }
                }
            }
        }

        // =========================================================
        // SEÇİLİ KULLANICININ YETKİLERİ
        // =========================================================

        private void YetkileriGetir(int kullaniciId)
        {
            YetkileriTemizle();

            using (SqlConnection conn = baglanti.Connection())
            {
                string sql = @"
                    SELECT
                        yd.ModulAdi,
                        yd.ErisimIzni
                    FROM KullaniciYetki ky
                    INNER JOIN YetkiDetay yd
                        ON ky.YetkiId = yd.YetkiId
                    WHERE ky.KullaniciId = @KullaniciId";

                using (SqlCommand cmd =
                    new SqlCommand(sql, conn))
                {
                    cmd.Parameters.Add(
                        "@KullaniciId",
                        SqlDbType.Int).Value = kullaniciId;

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            string modul =
                                dr["ModulAdi"].ToString();

                            bool izin =
                                Convert.ToBoolean(
                                    dr["ErisimIzni"]);

                            CheckBox cb =
                                CheckBoxBul(modul);

                            if (cb != null)
                                cb.Checked = izin;
                        }
                    }
                }
            }
        }

        // =========================================================
        // MODÜL → CHECKBOX
        // =========================================================

        private CheckBox CheckBoxBul(string modulAdi)
        {
            switch (modulAdi)
            {
                case "Ürün Görüntüle":
                    return checkBox1;

                case "Ürün Ekle":
                    return checkBox2;

                case "Ürün Sil":
                    return checkBox3;

                case "Depo Görüntüle":
                    return checkBox4;

                case "Depo Ekle":
                    return checkBox5;

                case "Depo Düzenle":
                    return checkBox6;

                case "Depo Pasifleştir":
                    return checkBox7;

                case "Stok Giriş":
                    return checkBox8;

                case "Stok Çıkış":
                    return checkBox9;

                default:
                    return null;
            }
        }

        // =========================================================
        // KULLANICI KAYDET
        // button1
        // =========================================================

        private void button1_Click(object sender, EventArgs e)
        {
            string kullaniciAdi = textBox1.Text.Trim();
            string ad = textBox2.Text.Trim();
            string soyad = textBox3.Text.Trim();
            string sifre = textBox4.Text;
            string sifreTekrar = textBox5.Text;

            // -----------------------------------------------------
            // KONTROLLER
            // -----------------------------------------------------

            if (string.IsNullOrWhiteSpace(kullaniciAdi))
            {
                MessageBox.Show(
                    "Kullanıcı adı giriniz.",
                    "Uyarı",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                textBox1.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(ad))
            {
                MessageBox.Show(
                    "Ad giriniz.",
                    "Uyarı",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                textBox2.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(soyad))
            {
                MessageBox.Show(
                    "Soyad giriniz.",
                    "Uyarı",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                textBox3.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(sifre))
            {
                MessageBox.Show(
                    "Şifre giriniz.",
                    "Uyarı",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                textBox4.Focus();
                return;
            }

            if (sifre != sifreTekrar)
            {
                MessageBox.Show(
                    "Şifreler eşleşmiyor.",
                    "Uyarı",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                textBox5.Focus();
                return;
            }

            if (comboBox1.SelectedValue == null ||
                comboBox1.SelectedValue is DataRowView)
            {
                MessageBox.Show(
                    "Rol seçiniz.",
                    "Uyarı",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                comboBox1.Focus();
                return;
            }

            int yetkiId =
                Convert.ToInt32(comboBox1.SelectedValue);

            bool aktif =
                comboBox3.SelectedIndex == 0;

            // -----------------------------------------------------
            // TRANSACTION
            // -----------------------------------------------------

            using (SqlConnection conn = baglanti.Connection())
            using (SqlTransaction transaction =
                conn.BeginTransaction())
            {
                try
                {
                    // Kullanıcı adı kontrolü
                    string kontrolSql = @"
                        SELECT COUNT(*)
                        FROM Kullanici
                        WHERE KullaniciAdi = @KullaniciAdi";

                    using (SqlCommand kontrol =
                        new SqlCommand(
                            kontrolSql,
                            conn,
                            transaction))
                    {
                        kontrol.Parameters.Add(
                            "@KullaniciAdi",
                            SqlDbType.NVarChar,
                            50).Value =
                            kullaniciAdi;

                        int adet =
                            Convert.ToInt32(
                                kontrol.ExecuteScalar());

                        if (adet > 0)
                        {
                            transaction.Rollback();

                            MessageBox.Show(
                                "Bu kullanıcı adı zaten kullanılıyor.",
                                "Uyarı",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning);

                            return;
                        }
                    }

                    // Kullanıcı ekle
                    string kullaniciSql = @"
                        INSERT INTO Kullanici
                        (
                            Ad,
                            Soyad,
                            KullaniciAdi,
                            Sifre,
                            AktifMi
                        )
                        VALUES
                        (
                            @Ad,
                            @Soyad,
                            @KullaniciAdi,
                            @Sifre,
                            @AktifMi
                        );

                        SELECT CAST(SCOPE_IDENTITY() AS INT);";

                    int kullaniciId;

                    using (SqlCommand cmd =
                        new SqlCommand(
                            kullaniciSql,
                            conn,
                            transaction))
                    {
                        cmd.Parameters.Add(
                            "@Ad",
                            SqlDbType.NVarChar,
                            50).Value = ad;

                        cmd.Parameters.Add(
                            "@Soyad",
                            SqlDbType.NVarChar,
                            50).Value = soyad;

                        cmd.Parameters.Add(
                            "@KullaniciAdi",
                            SqlDbType.NVarChar,
                            50).Value =
                            kullaniciAdi;

                        cmd.Parameters.Add(
                            "@Sifre",
                            SqlDbType.NVarChar,
                            200).Value =
                            sifre;

                        cmd.Parameters.Add(
                            "@AktifMi",
                            SqlDbType.Bit).Value =
                            aktif;

                        kullaniciId =
                            Convert.ToInt32(
                                cmd.ExecuteScalar());
                    }

                    // Kullanıcı → Rol
                    string kullaniciYetkiSql = @"
                        INSERT INTO KullaniciYetki
                        (
                            KullaniciId,
                            YetkiId
                        )
                        VALUES
                        (
                            @KullaniciId,
                            @YetkiId
                        );";

                    using (SqlCommand cmd =
                        new SqlCommand(
                            kullaniciYetkiSql,
                            conn,
                            transaction))
                    {
                        cmd.Parameters.Add(
                            "@KullaniciId",
                            SqlDbType.Int).Value =
                            kullaniciId;

                        cmd.Parameters.Add(
                            "@YetkiId",
                            SqlDbType.Int).Value =
                            yetkiId;

                        cmd.ExecuteNonQuery();
                    }

                    transaction.Commit();

                    MessageBox.Show(
                        "Kullanıcı başarıyla oluşturuldu.",
                        "Başarılı",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);

                    KullanicilariGetir();
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
                        "Kullanıcı kaydedilirken hata oluştu.\n\n" +
                        ex.Message,
                        "Hata",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
            }
        }

        // =========================================================
        // YETKİLENDİR
        // button3
        // =========================================================

        private void button3_Click(object sender, EventArgs e)
        {
            if (comboBox2.SelectedValue == null ||
                comboBox2.SelectedValue is DataRowView)
            {
                MessageBox.Show(
                    "Önce kullanıcı seçiniz.",
                    "Uyarı",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                return;
            }

            int kullaniciId =
                Convert.ToInt32(comboBox2.SelectedValue);

            int? yetkiId =
                KullaniciRolIdGetir(kullaniciId);

            if (!yetkiId.HasValue)
            {
                MessageBox.Show(
                    "Bu kullanıcının rolü bulunamadı.",
                    "Uyarı",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                return;
            }

            using (SqlConnection conn = baglanti.Connection())
            using (SqlTransaction transaction =
                conn.BeginTransaction())
            {
                try
                {
                    // Önce mevcut izinleri sil
                    string deleteSql = @"
                        DELETE FROM YetkiDetay
                        WHERE YetkiId = @YetkiId";

                    using (SqlCommand cmd =
                        new SqlCommand(
                            deleteSql,
                            conn,
                            transaction))
                    {
                        cmd.Parameters.Add(
                            "@YetkiId",
                            SqlDbType.Int).Value =
                            yetkiId.Value;

                        cmd.ExecuteNonQuery();
                    }

                    // Seçilenleri ekle
                    foreach (CheckBox cb in TumYetkiCheckboxlari())
                    {
                        if (!cb.Checked)
                            continue;

                        string insertSql = @"
                            INSERT INTO YetkiDetay
                            (
                                YetkiId,
                                ModulAdi,
                                ErisimIzni
                            )
                            VALUES
                            (
                                @YetkiId,
                                @ModulAdi,
                                1
                            )";

                        using (SqlCommand cmd =
                            new SqlCommand(
                                insertSql,
                                conn,
                                transaction))
                        {
                            cmd.Parameters.Add(
                                "@YetkiId",
                                SqlDbType.Int).Value =
                                yetkiId.Value;

                            cmd.Parameters.Add(
                                "@ModulAdi",
                                SqlDbType.NVarChar,
                                50).Value =
                                cb.Text;

                            cmd.ExecuteNonQuery();
                        }
                    }

                    transaction.Commit();

                    MessageBox.Show(
                        "Yetkiler başarıyla kaydedildi.",
                        "Başarılı",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);

                    YetkileriGetir(kullaniciId);
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
                        "Yetkiler kaydedilirken hata oluştu.\n\n" +
                        ex.Message,
                        "Hata",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
            }
        }

        // =========================================================
        // KULLANICININ ROLÜNÜ BUL
        // =========================================================

        private int? KullaniciRolIdGetir(int kullaniciId)
        {
            using (SqlConnection conn = baglanti.Connection())
            {
                string sql = @"
                    SELECT TOP 1 YetkiId
                    FROM KullaniciYetki
                    WHERE KullaniciId = @KullaniciId";

                using (SqlCommand cmd =
                    new SqlCommand(sql, conn))
                {
                    cmd.Parameters.Add(
                        "@KullaniciId",
                        SqlDbType.Int).Value =
                        kullaniciId;

                    object sonuc =
                        cmd.ExecuteScalar();

                    if (sonuc == null)
                        return null;

                    return Convert.ToInt32(sonuc);
                }
            }
        }

        // =========================================================
        // SIFIRLA
        // button4
        // =========================================================

        private void button4_Click(object sender, EventArgs e)
        {
            if (comboBox2.SelectedValue == null ||
                comboBox2.SelectedValue is DataRowView)
            {
                MessageBox.Show(
                    "Önce kullanıcı seçiniz.",
                    "Uyarı",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                return;
            }

            int kullaniciId =
                Convert.ToInt32(comboBox2.SelectedValue);

            int? yetkiId =
                KullaniciRolIdGetir(kullaniciId);

            if (!yetkiId.HasValue)
                return;

            DialogResult cevap =
                MessageBox.Show(
                    "Seçili rolün tüm yetkileri kaldırılacak. Devam edilsin mi?",
                    "Onay",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning);

            if (cevap != DialogResult.Yes)
                return;

            using (SqlConnection conn = baglanti.Connection())
            {
                string sql = @"
                    DELETE FROM YetkiDetay
                    WHERE YetkiId = @YetkiId";

                using (SqlCommand cmd =
                    new SqlCommand(sql, conn))
                {
                    cmd.Parameters.Add(
                        "@YetkiId",
                        SqlDbType.Int).Value =
                        yetkiId.Value;

                    cmd.ExecuteNonQuery();
                }
            }

            YetkileriTemizle();

            MessageBox.Show(
                "Yetkiler sıfırlandı.",
                "Bilgi",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }

        // =========================================================
        // TEMİZLE
        // button2
        // =========================================================

        private void button2_Click(object sender, EventArgs e)
        {
            Temizle();
        }

        private void Temizle()
        {
            textBox1.Clear();
            textBox2.Clear();
            textBox3.Clear();
            textBox4.Clear();
            textBox5.Clear();

            comboBox1.SelectedIndex = -1;
            comboBox3.SelectedIndex = 0;

            labelEdit13.Text = ".";
            labelEdit14.Text = ".";

            YetkileriTemizle();

            textBox1.Focus();
        }

        // =========================================================
        // TÜM CHECKBOXLAR
        // =========================================================

        private CheckBox[] TumYetkiCheckboxlari()
        {
            return new CheckBox[]
            {
                checkBox1,
                checkBox2,
                checkBox3,
                checkBox4,
                checkBox5,
                checkBox6,
                checkBox7,
                checkBox8,
                checkBox9
            };
        }

        private void YetkileriTemizle()
        {
            foreach (CheckBox cb in TumYetkiCheckboxlari())
            {
                cb.Checked = false;
            }
        }

        // =========================================================
        // DESIGNER'DAKİ EVENTLER
        // =========================================================

        private void comboBox1_SelectedIndexChanged(
            object sender,
            EventArgs e)
        {
            // Rol seçimi burada gerekirse kullanılabilir.
        }

        private void labelEdit1_Click(
            object sender,
            EventArgs e)
        {
        }

        private void labelEdit11_Click(
            object sender,
            EventArgs e)
        {
        }

        private void pictureBox4_Click(
            object sender,
            EventArgs e)
        {
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {

        }
    }
}