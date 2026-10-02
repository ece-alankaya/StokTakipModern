using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace StokTakip_Modern.Sınıflar
{
    public class SqlBaglanti
    {
        private readonly string baglanti = @"Server=(local);Database=StokTakipDbV5_Yeni;Trusted_Connection=True;TrustServerCertificate=True;";

        public SqlConnection Connection()
        {
            SqlConnection conn = new SqlConnection(baglanti);
            conn.Open();
            return conn;
        }
    }
}
