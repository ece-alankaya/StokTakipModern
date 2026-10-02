StokTakip_Modern
C# ve Windows Forms kullanılarak geliştirilmiş modern bir Stok Takip Sistemi uygulamasıdır.

Proje Hakkında
StokTakip_Modern, işletmelerin ürün, stok, tedarikçi ve depo süreçlerini daha kolay yönetebilmesi amacıyla geliştirilmiştir.
Uygulama üzerinden stok kayıtları takip edilebilir, ürün ve tedarikçi bilgileri yönetilebilir ve 
ilgili işlemler kullanıcı dostu bir arayüz üzerinden gerçekleştirilebilir.

Özellikler
Ürün ve stok yönetimi
Tedarikçi yönetimi
Depo yönetimi
Stok takibi
Yeni ürün ve kayıt ekleme
Kayıt güncelleme
Kayıt silme
Kayıt ve ürün takibi
Windows Forms tabanlı kullanıcı arayüzü
Veritabanı bağlantısı

Kullanılan Teknolojiler
C#
.NET / Windows Forms
Microsoft Visual Studio
SQL
Git & GitHub

Gereksinimler
Projeyi çalıştırmak için:
Microsoft Visual Studio
Uygun .NET Framework / .NET çalışma ortamı
SQL veritabanı
Projede kullanılan gerekli NuGet paketleri

Kurulum
Repository'yi bilgisayarınıza klonlayın:
git clone https://github.com/ecealankaya/StokTakip_Modern.git
StokTakip_Modern.sln dosyasını Microsoft Visual Studio ile açın.
Veritabanı bağlantı ayarlarınızı projenize göre yapılandırın.
Gerekli paketlerin yüklenmesini bekleyin.
Projeyi Build edip çalıştırın.

Veritabanı
Proje veritabanı işlemleri için SQL bağlantısı kullanmaktadır.
Veritabanı bağlantı ayarlarının proje ortamına göre düzenlenmesi gerekebilir.

Proje Yapısı
StokTakip_Modern/
│
├── Formlar/
│   ├── frmAnaForm.cs
│   ├── frmDashboard.cs
│   ├── frmDepoTanimlari.cs
│   ├── frmKategoriIslemleri.cs
│   ├── frmKullaniciVeYetki.cs
│   ├── frmLogin.cs
│   ├── frmMarkaIslemleri.cs
│   ├── frmRafTanimlari.cs
│   ├── frmSevkiyatYonetimi.cs
│   ├── frmStokHareketleri.cs
│   ├── frmStokOperasyonlari.cs
│   ├── frmTedarikci.cs
│   └── frmUrunIslemleri.cs
│
├── Properties/
├── Resources/
├── Sınıflar/
│   └── SqlBaglantisi.cs
│
├── Program.cs
├── App.config
├── StokTakip_Modern.csproj
├── StokTakip_Modern.sln
└── README.md

