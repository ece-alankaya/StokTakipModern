using ReaLTaiizor.Controls;
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
    public partial class frmUrunIslemleri : Form
    {
        public frmUrunIslemleri()
        {
            InitializeComponent();
        }
        private void frmUrunIslemleri_Load(object sender, EventArgs e)
        {
            UrunleriGetir();
        }


        private void UrunleriGetir()
        {
            try
            {
                Sınıflar.SqlBaglanti db = new Sınıflar.SqlBaglanti();

                using (SqlConnection conn = db.Connection())
                {
                    string sorgu = @"
                        SELECT
                            U.UrunId,
                            ISNULL(B.BarkodNo, '') AS Barkod,
                            U.UrunAdi,
                            ISNULL(K.KategoriAdi, '') AS Kategori,
                            ISNULL(M.MarkaAdi, '') AS Marka,
                            U.MinimumStok,
                            ISNULL(S.Stok, 0) AS Stok,
                            CASE
                                WHEN U.AktifMi = 1 THEN 'Aktif'
                                ELSE 'Pasif'
                            END AS Durum,
                            U.OlusturmaTarihi
                        FROM Urun U

                        OUTER APPLY
                        (
                            SELECT TOP 1 BarkodNo
                            FROM Barkod
                            WHERE Barkod.UrunId = U.UrunId
                            ORDER BY AnaBarkod DESC, BarkodId
                        ) B

                        OUTER APPLY
                        (
                            SELECT TOP 1 KategoriAdi
                            FROM UrunKategori UK
                            INNER JOIN Kategori K
                                ON K.KategoriId = UK.KategoriId
                            WHERE UK.UrunId = U.UrunId
                        ) K

                        OUTER APPLY
                        (
                            SELECT TOP 1 MarkaAdi
                            FROM UrunMarka UM
                            INNER JOIN Marka M
                                ON M.MarkaId = UM.MarkaId
                            WHERE UM.UrunId = U.UrunId
                        ) M

                        OUTER APPLY
                        (
                            SELECT SUM(MevcutMiktar) AS Stok
                            FROM DepoStok
                            WHERE DepoStok.UrunId = U.UrunId
                        ) S

                        ORDER BY U.UrunId DESC";

                    using (SqlDataAdapter adapter =
                           new SqlDataAdapter(sorgu, conn))
                    {
                        DataTable tablo = new DataTable();

                        adapter.Fill(tablo);
                         dataGridView1.DataSource = tablo;
                        
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Ürünler yüklenirken hata oluştu.\n\n" + ex.Message,
                    "Hata",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }
        private void labelEdit2_Click(object sender, EventArgs e)
        {

        }

        

        private void labelEdit5_Click(object sender, EventArgs e)
        {

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
                return;

            DataGridViewRow satir = dataGridView1.Rows[e.RowIndex];

            textBox1.Text = satir.Cells["Barkod"].Value?.ToString() ?? "";
            textBox2.Text = satir.Cells["UrunAdi"].Value?.ToString() ?? "";
            textBox3.Text = satir.Cells["Kategori"].Value?.ToString() ?? "";
            textBox4.Text = satir.Cells["Marka"].Value?.ToString() ?? "";
            textBox5.Text = satir.Cells["MinimumStok"].Value?.ToString() ?? "";

            this.Tag = satir.Cells["UrunId"].Value;
           
        }

        private void foxButton1_Click(object sender, EventArgs e)
        {
            textBox1.Clear();
            textBox2.Clear();
            textBox3.Clear();
            textBox4.Clear();
            textBox5.Clear();   
            

        }

        private void foxButton2_Click(object sender, EventArgs e)
        {
            // =========================================
            // BOŞ ALAN KONTROLLERİ
            // =========================================

            if (string.IsNullOrWhiteSpace(textBox1.Text))
            {
                MessageBox.Show(
                    "Barkod boş bırakılamaz.",
                    "Uyarı",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                textBox1.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(textBox2.Text))
            {
                MessageBox.Show(
                    "Ürün adı boş bırakılamaz.",
                    "Uyarı",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                textBox2.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(textBox3.Text))
            {
                MessageBox.Show(
                    "Kategori boş bırakılamaz.",
                    "Uyarı",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                textBox3.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(textBox4.Text))
            {
                MessageBox.Show(
                    "Marka boş bırakılamaz.",
                    "Uyarı",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                textBox4.Focus();
                return;
            }

            // =========================================
            // MİNİMUM STOK KONTROLÜ
            // =========================================

            if (!int.TryParse(textBox5.Text.Trim(), out int minimumStok))
            {
                MessageBox.Show(
                    "Minimum stok değeri sayı olmalıdır.",
                    "Uyarı",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                textBox5.Focus();
                textBox5.SelectAll();
                return;
            }

            try
            {
                Sınıflar.SqlBaglanti db = new Sınıflar.SqlBaglanti();

                using (SqlConnection conn = db.Connection())
                {
                    // =========================================
                    // 1. BARKOD UNIQUE KONTROLÜ
                    // =========================================

                    string barkodKontrolSorgu = @"
                SELECT COUNT(*)
                FROM Barkod
                WHERE BarkodNo = @BarkodNo";

                    using (SqlCommand cmdBarkodKontrol =
                           new SqlCommand(barkodKontrolSorgu, conn))
                    {
                        cmdBarkodKontrol.Parameters.AddWithValue(
                            "@BarkodNo",
                            textBox1.Text.Trim());

                        int barkodSayisi =
                            Convert.ToInt32(
                                cmdBarkodKontrol.ExecuteScalar());

                        if (barkodSayisi > 0)
                        {
                            MessageBox.Show(
                                "Bu barkod başka bir üründe zaten kullanılıyor.\n\n" +
                                "Barkod: " + textBox1.Text.Trim(),
                                "Barkod Zaten Kullanılıyor",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning);

                            textBox1.Focus();
                            textBox1.SelectAll();

                            return;
                        }
                    }


                    // =========================================
                    // 2. KATEGORİ VAR MI?
                    // YOKSA OTOMATİK OLUŞTUR
                    // =========================================

                    int kategoriId;

                    string kategoriSorgu = @"
                IF EXISTS
                (
                    SELECT 1
                    FROM Kategori
                    WHERE KategoriAdi = @KategoriAdi
                )
                BEGIN
                    SELECT KategoriId
                    FROM Kategori
                    WHERE KategoriAdi = @KategoriAdi
                END
                ELSE
                BEGIN
                    INSERT INTO Kategori
                    (
                        KategoriAdi
                    )
                    OUTPUT INSERTED.KategoriId
                    VALUES
                    (
                        @KategoriAdi
                    )
                END";

                    using (SqlCommand cmdKategori =
                           new SqlCommand(kategoriSorgu, conn))
                    {
                        cmdKategori.Parameters.AddWithValue(
                            "@KategoriAdi",
                            textBox3.Text.Trim());

                        kategoriId = Convert.ToInt32(
                            cmdKategori.ExecuteScalar());
                    }


                    // =========================================
                    // 3. MARKA VAR MI?
                    // YOKSA OTOMATİK OLUŞTUR
                    // =========================================

                    int markaId;

                    string markaSorgu = @"
                IF EXISTS
                (
                    SELECT 1
                    FROM Marka
                    WHERE MarkaAdi = @MarkaAdi
                )
                BEGIN
                    SELECT MarkaId
                    FROM Marka
                    WHERE MarkaAdi = @MarkaAdi
                END
                ELSE
                BEGIN
                    INSERT INTO Marka
                    (
                        MarkaAdi
                    )
                    OUTPUT INSERTED.MarkaId
                    VALUES
                    (
                        @MarkaAdi
                    )
                END";

                    using (SqlCommand cmdMarka =
                           new SqlCommand(markaSorgu, conn))
                    {
                        cmdMarka.Parameters.AddWithValue(
                            "@MarkaAdi",
                            textBox4.Text.Trim());

                        markaId = Convert.ToInt32(
                            cmdMarka.ExecuteScalar());
                    }


                    // =========================================
                    // 4. ÜRÜNÜ EKLE
                    // =========================================

                    int urunId;

                    string urunSorgu = @"
                INSERT INTO Urun
                (
                    UrunAdi,
                    MinimumStok,
                    AktifMi,
                    OlusturmaTarihi
                )
                OUTPUT INSERTED.UrunId
                VALUES
                (
                    @UrunAdi,
                    @MinimumStok,
                    1,
                    GETDATE()
                )";

                    using (SqlCommand cmdUrun =
                           new SqlCommand(urunSorgu, conn))
                    {
                        cmdUrun.Parameters.AddWithValue(
                            "@UrunAdi",
                            textBox2.Text.Trim());

                        cmdUrun.Parameters.AddWithValue(
                            "@MinimumStok",
                            minimumStok);

                        urunId = Convert.ToInt32(
                            cmdUrun.ExecuteScalar());
                    }


                    // =========================================
                    // 5. BARKODU EKLE
                    // =========================================

                    string barkodSorgu = @"
                INSERT INTO Barkod
                (
                    UrunId,
                    BarkodNo,
                    AnaBarkod
                )
                VALUES
                (
                    @UrunId,
                    @BarkodNo,
                    1
                )";

                    using (SqlCommand cmdBarkod =
                           new SqlCommand(barkodSorgu, conn))
                    {
                        cmdBarkod.Parameters.AddWithValue(
                            "@UrunId",
                            urunId);

                        cmdBarkod.Parameters.AddWithValue(
                            "@BarkodNo",
                            textBox1.Text.Trim());

                        cmdBarkod.ExecuteNonQuery();
                    }


                    // =========================================
                    // 6. ÜRÜN - KATEGORİ İLİŞKİSİ
                    // =========================================

                    string urunKategoriSorgu = @"
                INSERT INTO UrunKategori
                (
                    UrunId,
                    KategoriId
                )
                VALUES
                (
                    @UrunId,
                    @KategoriId
                )";

                    using (SqlCommand cmdUrunKategori =
                           new SqlCommand(urunKategoriSorgu, conn))
                    {
                        cmdUrunKategori.Parameters.AddWithValue(
                            "@UrunId",
                            urunId);

                        cmdUrunKategori.Parameters.AddWithValue(
                            "@KategoriId",
                            kategoriId);

                        cmdUrunKategori.ExecuteNonQuery();
                    }


                    // =========================================
                    // 7. ÜRÜN - MARKA İLİŞKİSİ
                    // =========================================

                    string urunMarkaSorgu = @"
                INSERT INTO UrunMarka
                (
                    UrunId,
                    MarkaId
                )
                VALUES
                (
                    @UrunId,
                    @MarkaId
                )";

                    using (SqlCommand cmdUrunMarka =
                           new SqlCommand(urunMarkaSorgu, conn))
                    {
                        cmdUrunMarka.Parameters.AddWithValue(
                            "@UrunId",
                            urunId);

                        cmdUrunMarka.Parameters.AddWithValue(
                            "@MarkaId",
                            markaId);

                        cmdUrunMarka.ExecuteNonQuery();
                    }
                }


                // =========================================
                // BAŞARILI
                // =========================================

                MessageBox.Show(
                    "Ürün başarıyla kaydedildi.",
                    "Başarılı",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);


                // =========================================
                // ALANLARI TEMİZLE
                // =========================================

                textBox1.Clear();
                textBox2.Clear();
                textBox3.Clear();
                textBox4.Clear();
                textBox5.Clear();
                richTextBox1.Clear();


                // =========================================
                // DATAGRIDVIEW YENİLE
                // =========================================

                UrunleriGetir();
            }
            catch (SqlException ex)
            {
                MessageBox.Show(
                    "Veritabanı işlemi sırasında hata oluştu.\n\n" +
                    ex.Message,
                    "Veritabanı Hatası",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Ürün kaydedilirken hata oluştu.\n\n" +
                    ex.Message,
                    "Hata",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void foxButton3_Click(object sender, EventArgs e)
        {
            // =========================================
            // ÜRÜN SEÇİLMİŞ Mİ?
            // =========================================

            if (this.Tag == null)
            {
                MessageBox.Show(
                    "Lütfen güncellemek istediğiniz ürünü listeden seçin.",
                    "Uyarı",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                return;
            }

            int urunId;

            try
            {
                urunId = Convert.ToInt32(this.Tag);
            }
            catch
            {
                MessageBox.Show(
                    "Seçilen ürünün ID bilgisi alınamadı.",
                    "Hata",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);

                return;
            }


            // =========================================
            // BOŞ ALAN KONTROLLERİ
            // =========================================

            if (string.IsNullOrWhiteSpace(textBox1.Text))
            {
                MessageBox.Show(
                    "Barkod boş bırakılamaz.",
                    "Uyarı",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                textBox1.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(textBox2.Text))
            {
                MessageBox.Show(
                    "Ürün adı boş bırakılamaz.",
                    "Uyarı",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                textBox2.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(textBox3.Text))
            {
                MessageBox.Show(
                    "Kategori boş bırakılamaz.",
                    "Uyarı",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                textBox3.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(textBox4.Text))
            {
                MessageBox.Show(
                    "Marka boş bırakılamaz.",
                    "Uyarı",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                textBox4.Focus();
                return;
            }


            // =========================================
            // MİNİMUM STOK KONTROLÜ
            // =========================================

            if (!int.TryParse(textBox5.Text.Trim(), out int minimumStok))
            {
                MessageBox.Show(
                    "Minimum stok değeri sayı olmalıdır.",
                    "Uyarı",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                textBox5.Focus();
                textBox5.SelectAll();

                return;
            }


            try
            {
                Sınıflar.SqlBaglanti db = new Sınıflar.SqlBaglanti();

                using (SqlConnection conn = db.Connection())
                {
                    // =========================================
                    // TRANSACTION BAŞLAT
                    // =========================================

                    using (SqlTransaction transaction = conn.BeginTransaction())
                    {
                        try
                        {
                            // =========================================
                            // 1. BARKOD UNIQUE KONTROLÜ
                            // =========================================
                            // Aynı barkod başka üründe var mı?
                            // Kendi ürününün barkodunu dikkate alma.

                            string barkodKontrolSorgu = @"
                        SELECT COUNT(*)
                        FROM Barkod
                        WHERE BarkodNo = @BarkodNo
                        AND UrunId <> @UrunId";

                            using (SqlCommand cmdBarkodKontrol =
                                   new SqlCommand(
                                       barkodKontrolSorgu,
                                       conn,
                                       transaction))
                            {
                                cmdBarkodKontrol.Parameters.AddWithValue(
                                    "@BarkodNo",
                                    textBox1.Text.Trim());

                                cmdBarkodKontrol.Parameters.AddWithValue(
                                    "@UrunId",
                                    urunId);

                                int barkodSayisi =
                                    Convert.ToInt32(
                                        cmdBarkodKontrol.ExecuteScalar());

                                if (barkodSayisi > 0)
                                {
                                    transaction.Rollback();

                                    MessageBox.Show(
                                        "Bu barkod başka bir üründe zaten kullanılıyor.\n\n" +
                                        "Barkod: " + textBox1.Text.Trim(),
                                        "Barkod Zaten Kullanılıyor",
                                        MessageBoxButtons.OK,
                                        MessageBoxIcon.Warning);

                                    textBox1.Focus();
                                    textBox1.SelectAll();

                                    return;
                                }
                            }


                            // =========================================
                            // 2. ÜRÜNÜ GÜNCELLE
                            // =========================================

                            string urunGuncelleSorgu = @"
                        UPDATE Urun
                        SET
                            UrunAdi = @UrunAdi,
                            MinimumStok = @MinimumStok
                        WHERE UrunId = @UrunId";

                            using (SqlCommand cmdUrun =
                                   new SqlCommand(
                                       urunGuncelleSorgu,
                                       conn,
                                       transaction))
                            {
                                cmdUrun.Parameters.AddWithValue(
                                    "@UrunAdi",
                                    textBox2.Text.Trim());

                                cmdUrun.Parameters.AddWithValue(
                                    "@MinimumStok",
                                    minimumStok);

                                cmdUrun.Parameters.AddWithValue(
                                    "@UrunId",
                                    urunId);

                                cmdUrun.ExecuteNonQuery();
                            }


                            // =========================================
                            // 3. BARKODU GÜNCELLE
                            // =========================================

                            string barkodGuncelleSorgu = @"
                        UPDATE Barkod
                        SET BarkodNo = @BarkodNo
                        WHERE UrunId = @UrunId
                        AND AnaBarkod = 1";

                            using (SqlCommand cmdBarkod =
                                   new SqlCommand(
                                       barkodGuncelleSorgu,
                                       conn,
                                       transaction))
                            {
                                cmdBarkod.Parameters.AddWithValue(
                                    "@BarkodNo",
                                    textBox1.Text.Trim());

                                cmdBarkod.Parameters.AddWithValue(
                                    "@UrunId",
                                    urunId);

                                cmdBarkod.ExecuteNonQuery();
                            }


                            // =========================================
                            // 4. KATEGORİYİ BUL
                            // YOKSA OLUŞTUR
                            // =========================================

                            int kategoriId;

                            string kategoriSorgu = @"
                        IF EXISTS
                        (
                            SELECT 1
                            FROM Kategori
                            WHERE KategoriAdi = @KategoriAdi
                        )
                        BEGIN
                            SELECT KategoriId
                            FROM Kategori
                            WHERE KategoriAdi = @KategoriAdi
                        END
                        ELSE
                        BEGIN
                            INSERT INTO Kategori
                            (
                                KategoriAdi
                            )
                            OUTPUT INSERTED.KategoriId
                            VALUES
                            (
                                @KategoriAdi
                            )
                        END";

                            using (SqlCommand cmdKategori =
                                   new SqlCommand(
                                       kategoriSorgu,
                                       conn,
                                       transaction))
                            {
                                cmdKategori.Parameters.AddWithValue(
                                    "@KategoriAdi",
                                    textBox3.Text.Trim());

                                kategoriId = Convert.ToInt32(
                                    cmdKategori.ExecuteScalar());
                            }


                            // =========================================
                            // 5. ÜRÜN - KATEGORİ İLİŞKİSİNİ GÜNCELLE
                            // =========================================

                            string kategoriGuncelleSorgu = @"
                        UPDATE UrunKategori
                        SET KategoriId = @KategoriId
                        WHERE UrunId = @UrunId";

                            using (SqlCommand cmdKategori =
                                   new SqlCommand(
                                       kategoriGuncelleSorgu,
                                       conn,
                                       transaction))
                            {
                                cmdKategori.Parameters.AddWithValue(
                                    "@KategoriId",
                                    kategoriId);

                                cmdKategori.Parameters.AddWithValue(
                                    "@UrunId",
                                    urunId);

                                int etkilenen =
                                    cmdKategori.ExecuteNonQuery();

                                // İlişki yoksa oluştur
                                if (etkilenen == 0)
                                {
                                    string kategoriEkleSorgu = @"
                                INSERT INTO UrunKategori
                                (
                                    UrunId,
                                    KategoriId
                                )
                                VALUES
                                (
                                    @UrunId,
                                    @KategoriId
                                )";

                                    using (SqlCommand cmdEkle =
                                           new SqlCommand(
                                               kategoriEkleSorgu,
                                               conn,
                                               transaction))
                                    {
                                        cmdEkle.Parameters.AddWithValue(
                                            "@UrunId",
                                            urunId);

                                        cmdEkle.Parameters.AddWithValue(
                                            "@KategoriId",
                                            kategoriId);

                                        cmdEkle.ExecuteNonQuery();
                                    }
                                }
                            }


                            // =========================================
                            // 6. MARKAYI BUL
                            // YOKSA OLUŞTUR
                            // =========================================

                            int markaId;

                            string markaSorgu = @"
                        IF EXISTS
                        (
                            SELECT 1
                            FROM Marka
                            WHERE MarkaAdi = @MarkaAdi
                        )
                        BEGIN
                            SELECT MarkaId
                            FROM Marka
                            WHERE MarkaAdi = @MarkaAdi
                        END
                        ELSE
                        BEGIN
                            INSERT INTO Marka
                            (
                                MarkaAdi
                            )
                            OUTPUT INSERTED.MarkaId
                            VALUES
                            (
                                @MarkaAdi
                            )
                        END";

                            using (SqlCommand cmdMarka =
                                   new SqlCommand(
                                       markaSorgu,
                                       conn,
                                       transaction))
                            {
                                cmdMarka.Parameters.AddWithValue(
                                    "@MarkaAdi",
                                    textBox4.Text.Trim());

                                markaId = Convert.ToInt32(
                                    cmdMarka.ExecuteScalar());
                            }


                            // =========================================
                            // 7. ÜRÜN - MARKA İLİŞKİSİNİ GÜNCELLE
                            // =========================================

                            string markaGuncelleSorgu = @"
                        UPDATE UrunMarka
                        SET MarkaId = @MarkaId
                        WHERE UrunId = @UrunId";

                            using (SqlCommand cmdMarka =
                                   new SqlCommand(
                                       markaGuncelleSorgu,
                                       conn,
                                       transaction))
                            {
                                cmdMarka.Parameters.AddWithValue(
                                    "@MarkaId",
                                    markaId);

                                cmdMarka.Parameters.AddWithValue(
                                    "@UrunId",
                                    urunId);

                                int etkilenen =
                                    cmdMarka.ExecuteNonQuery();

                                // İlişki yoksa oluştur
                                if (etkilenen == 0)
                                {
                                    string markaEkleSorgu = @"
                                INSERT INTO UrunMarka
                                (
                                    UrunId,
                                    MarkaId
                                )
                                VALUES
                                (
                                    @UrunId,
                                    @MarkaId
                                )";

                                    using (SqlCommand cmdEkle =
                                           new SqlCommand(
                                               markaEkleSorgu,
                                               conn,
                                               transaction))
                                    {
                                        cmdEkle.Parameters.AddWithValue(
                                            "@UrunId",
                                            urunId);

                                        cmdEkle.Parameters.AddWithValue(
                                            "@MarkaId",
                                            markaId);

                                        cmdEkle.ExecuteNonQuery();
                                    }
                                }
                            }


                            // =========================================
                            // HER ŞEY BAŞARILI
                            // =========================================

                            transaction.Commit();
                        }
                        catch
                        {
                            transaction.Rollback();
                            throw;
                        }
                    }
                }


                // =========================================
                // BAŞARILI MESAJ
                // =========================================

                MessageBox.Show(
                    "Ürün başarıyla güncellendi.",
                    "Başarılı",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);


                // =========================================
                // SEÇİMİ TEMİZLE
                // =========================================

                this.Tag = null;


                // =========================================
                // TEXTBOXLARI TEMİZLE
                // =========================================

                textBox1.Clear();
                textBox2.Clear();
                textBox3.Clear();
                textBox4.Clear();
                textBox5.Clear();
                richTextBox1.Clear();


                // =========================================
                // LİSTEYİ YENİLE
                // =========================================

                UrunleriGetir();
            }
            catch (SqlException ex)
            {
                MessageBox.Show(
                    "Ürün güncellenirken veritabanı hatası oluştu.\n\n" +
                    ex.Message,
                    "Veritabanı Hatası",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Ürün güncellenirken hata oluştu.\n\n" +
                    ex.Message,
                    "Hata",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }
        
        private void foxButton4_Click(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentRow == null)
            {
                MessageBox.Show(
                    "Lütfen silmek istediğiniz ürünü seçin.",
                    "Uyarı",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                return;
            }

            // =========================================
            // ÜRÜN ID AL
            // =========================================

            int urunId;

            try
            {
                urunId = Convert.ToInt32(
                    dataGridView1.CurrentRow.Cells["UrunId"].Value);
            }
            catch
            {
                MessageBox.Show(
                    "Seçilen ürünün ID bilgisi alınamadı.",
                    "Hata",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);

                return;
            }

            // =========================================
            // ÜRÜN ADINI AL
            // =========================================

            string urunAdi =
                dataGridView1.CurrentRow.Cells["UrunAdi"].Value?.ToString();

            // =========================================
            // SİLME ONAYI
            // =========================================

            DialogResult cevap = MessageBox.Show(
                "Seçili ürünü silmek istediğinize emin misiniz?\n\n" +
                "Ürün: " + urunAdi,
                "Ürün Sil",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (cevap != DialogResult.Yes)
            {
                return;
            }

            try
            {
                Sınıflar.SqlBaglanti db = new Sınıflar.SqlBaglanti();

                using (SqlConnection conn = db.Connection())
                {
                    // =========================================
                    // TRANSACTION BAŞLAT
                    // =========================================

                    using (SqlTransaction transaction = conn.BeginTransaction())
                    {
                        try
                        {
                            // =========================================
                            // 1. BARKODLARI SİL
                            // =========================================

                            string barkodSil = @"
                        DELETE FROM Barkod
                        WHERE UrunId = @UrunId";

                            using (SqlCommand cmd =
                                   new SqlCommand(
                                       barkodSil,
                                       conn,
                                       transaction))
                            {
                                cmd.Parameters.AddWithValue(
                                    "@UrunId",
                                    urunId);

                                cmd.ExecuteNonQuery();
                            }


                            // =========================================
                            // 2. ÜRÜN - KATEGORİ İLİŞKİSİNİ SİL
                            // =========================================

                            string kategoriIliskiSil = @"
                        DELETE FROM UrunKategori
                        WHERE UrunId = @UrunId";

                            using (SqlCommand cmd =
                                   new SqlCommand(
                                       kategoriIliskiSil,
                                       conn,
                                       transaction))
                            {
                                cmd.Parameters.AddWithValue(
                                    "@UrunId",
                                    urunId);

                                cmd.ExecuteNonQuery();
                            }


                            // =========================================
                            // 3. ÜRÜN - MARKA İLİŞKİSİNİ SİL
                            // =========================================

                            string markaIliskiSil = @"
                        DELETE FROM UrunMarka
                        WHERE UrunId = @UrunId";

                            using (SqlCommand cmd =
                                   new SqlCommand(
                                       markaIliskiSil,
                                       conn,
                                       transaction))
                            {
                                cmd.Parameters.AddWithValue(
                                    "@UrunId",
                                    urunId);

                                cmd.ExecuteNonQuery();
                            }


                            // =========================================
                            // 4. ÜRÜNÜ SİL
                            // =========================================

                            string urunSil = @"
                        DELETE FROM Urun
                        WHERE UrunId = @UrunId";

                            using (SqlCommand cmd =
                                   new SqlCommand(
                                       urunSil,
                                       conn,
                                       transaction))
                            {
                                cmd.Parameters.AddWithValue(
                                    "@UrunId",
                                    urunId);

                                int silinen =
                                    cmd.ExecuteNonQuery();

                                if (silinen == 0)
                                {
                                    throw new Exception(
                                        "Ürün bulunamadı veya silinemedi.");
                                }
                            }


                            // =========================================
                            // HER ŞEY BAŞARILI
                            // =========================================

                            transaction.Commit();
                        }
                        catch
                        {
                            // Herhangi bir yerde hata varsa
                            // yapılan işlemleri geri al
                            transaction.Rollback();

                            throw;
                        }
                    }
                }


                // =========================================
                // BAŞARILI MESAJI
                // =========================================

                MessageBox.Show(
                    "Ürün başarıyla silindi.",
                    "Başarılı",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);


                // =========================================
                // LİSTEYİ YENİLE
                // =========================================

                UrunleriGetir();
            }
            catch (SqlException ex)
            {
                MessageBox.Show(
                    "Ürün silinirken veritabanı hatası oluştu.\n\n" +
                    ex.Message,
                    "Veritabanı Hatası",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Ürün silinirken hata oluştu.\n\n" +
                    ex.Message,
                    "Hata",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }

        }
    }
}
