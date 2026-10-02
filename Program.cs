using System;
using System.Windows.Forms;

namespace StokTakip_Modern
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            try
            {
                using (Formlar.frmLogin login = new Formlar.frmLogin())
                {
                    if (login.ShowDialog() == DialogResult.OK)
                    {
                        Application.Run(new Formlar.frmAnaForm(login.GirisYapanKullanici));
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    ex.ToString(),
                    "Hata",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }
    }
}