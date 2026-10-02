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
using LiveCharts;
using LiveCharts.Wpf;




namespace StokTakip_Modern.Formlar
{
    public partial class frmDashboard : Form
    {
        SqlBaglanti bgl = new SqlBaglanti();
        public frmDashboard()
        {
            InitializeComponent();
            
        }
       

        private void ToplamUrunGetir()
        { 
           using (SqlConnection conn = bgl.Connection())
        {
        SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM Urun", conn);

        labelEdit4.Text = cmd.ExecuteScalar().ToString();
    }
}
        private void ToplamStokGetir()
        {
            using (SqlConnection conn = bgl.Connection())
            {
                SqlCommand cmd = new SqlCommand("SELECT ISNULL(SUM(MevcutMiktar),0) FROM DepoStok", conn);

                labelEdit5.Text = cmd.ExecuteScalar().ToString();
            }
        }
        //private void ToplamDepoGetir()
        //{
        //    using(SqlConnection conn = bgl.Connection())
        //    {
        //        SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM Depo ",conn);
        //        labelEdit7.Text = cmd.ExecuteScalar().ToString();
        //    }
        //}

        private void ToplamTedarikçiGetir()
        {
            using (SqlConnection conn = bgl.Connection())
            {
                SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM Tedarikci WHERE AktifMi = 1", conn);
                labelEdit13.Text = cmd.ExecuteScalar().ToString();
            }
        }
        private void ToplamMarkaGetir()
        {
            using (SqlConnection con = bgl.Connection())
            {
                SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM Marka WHERE AktifMi = 1", con);
                labelEdit11.Text = cmd.ExecuteScalar().ToString();

            }
        }
        
        private void panel4_Paint(object sender, PaintEventArgs e)
        {

        }
        private void StokGenelGorunumu()
        {
            ChartValues<int> giris = new ChartValues<int>();
            ChartValues<int> cikis = new ChartValues<int>();

            List<string> tarihler = new List<string>();

            using (SqlConnection conn = bgl.Connection())
            {
                for (int gun = 1; gun <= 13; gun++)
                {
                    string tarih = $"2026-08-{gun:00}";

                    // Giriş
                    SqlCommand cmdGiris = new SqlCommand(@"
            SELECT ISNULL(SUM(Miktar),0)
            FROM StokHareket
            WHERE HareketTipiId = 1
            AND CONVERT(date,HareketTarihi)=@tarih", conn);

                    cmdGiris.Parameters.AddWithValue("@tarih", tarih);

                    giris.Add(Convert.ToInt32(cmdGiris.ExecuteScalar()));

                    // Çıkış
                    SqlCommand cmdCikis = new SqlCommand(@"
            SELECT ISNULL(SUM(Miktar),0)
            FROM StokHareket
            WHERE HareketTipiId = 2
            AND CONVERT(date,HareketTarihi)=@tarih", conn);

                    cmdCikis.Parameters.AddWithValue("@tarih", tarih);

                    cikis.Add(Convert.ToInt32(cmdCikis.ExecuteScalar()));

                    tarihler.Add(gun.ToString());
                }
            }

            cartesianChart1.Series = new SeriesCollection
    {
        new LineSeries
        {
            Title = "Stok Girişi",
            Values = giris
        },

        new LineSeries
        {
            Title = "Stok Çıkışı",
            Values = cikis
        }
    };

            cartesianChart1.AxisX.Clear();

            cartesianChart1.AxisX.Add(new Axis
            {
                Title = "Ağustos",
                Labels = tarihler
            });

            cartesianChart1.AxisY.Clear();

            cartesianChart1.AxisY.Add(new Axis
            {
                Title = "Ürün Adedi"
            });
        }


        private void KategoriGrafik()
        {
            SeriesCollection seri = new SeriesCollection();

            using (SqlConnection conn = bgl.Connection())
            {
                SqlCommand cmd = new SqlCommand(@"
        SELECT
            K.KategoriAdi,
            COUNT(UK.UrunId) AS UrunSayisi
        FROM Kategori K
        LEFT JOIN UrunKategori UK
            ON K.KategoriId = UK.KategoriId
        GROUP BY K.KategoriAdi", conn);

                SqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    seri.Add(new PieSeries
                    {
                        Title = dr["KategoriAdi"].ToString(),
                        Values = new ChartValues<int>
                {
                    Convert.ToInt32(dr["UrunSayisi"])
                },
                        DataLabels = true
                    });
                }
            }

            pieChartKategori.Series = seri;
        }


        private void DepoGrafik()
        {
            SeriesCollection seri = new SeriesCollection();

            using (SqlConnection conn = bgl.Connection())
            {
                SqlCommand cmd = new SqlCommand(@"
        SELECT
            D.DepoAdi,
            SUM(DS.MevcutMiktar) AS ToplamStok
        FROM Depo D
        INNER JOIN DepoStok DS
            ON D.DepoId = DS.DepoId
        GROUP BY D.DepoAdi
        ORDER BY ToplamStok DESC", conn);

                SqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    seri.Add(new PieSeries
                    {
                        Title = dr["DepoAdi"].ToString(),
                        Values = new ChartValues<int>
                {
                    Convert.ToInt32(dr["ToplamStok"])
                },
                        DataLabels = true,
                       
                    });
                }
            }

            pieChartDepo.Series = seri;
        }






        private void frmDashboard_Load(object sender, EventArgs e)
        {
            ToplamUrunGetir();
            ToplamStokGetir();
            ToplamTedarikçiGetir();
            ToplamMarkaGetir();
            StokGenelGorunumu();
            KategoriGrafik();
            DepoGrafik();
        }

        private void panel10_Paint(object sender, PaintEventArgs e)
        {

        }

        private void labelEdit1_Click(object sender, EventArgs e)
        {

        }

        private void labelEdit2_Click(object sender, EventArgs e)
        {

        }
    }
}
