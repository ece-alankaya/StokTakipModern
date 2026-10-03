# StokTakipModern
C# Windows Forms ile geliştirilmiş modern bir Stok Takip ve Yönetim Sistemi
## Proje Hakkında
işletmelerin ürün, stok, tedarikçi, depo ve sevkiyat süreçlerini tek bir merkezden kolayca yönetebilmesi amacıyla geliştirilmiştir. 
Kullanıcı dostu arayüzü sayesinde stok hareketleri anlık olarak izlenebilir, grafiksel raporlar üzerinden stok durumu analiz edilebilir ve yetkilendirme sistemi ile güvenli bir kullanım sağlanır.
## Özellikler
* **Ürün & Stok Yönetimi:** Ürün ekleme, güncelleme, silme ve detaylı arama/listeleme.
* **Kategori & Marka Tanımları:** Ürünleri kategorilere ve markalara göre organize etme.
* **Depo & Raf Takibi:** Ürünlerin hangi depo ve rafta bulunduğunu konum bazlı takip etme.
* **Stok Operasyonları & Hareketleri:** Anlık stok giriş-çıkış işlemleri ve geçmiş stok hareketleri dökümü.
* **Sevkiyat Yönetimi:** Ürün sevkiyat süreçlerinin oluşturulması ve takibi.
* **Tedarikçi Yönetimi:** Tedarikçi kartları oluşturma ve tedarikçi bazlı işlem takibi.
* **Grafiksel Dashboard (LiveCharts):** Mevcut stok durumlarının grafik ve istatistiklerle görselleştirilmesi.
* **Kullanıcı & Yetki Yönetimi:** Sisteme erişim sağlayacak kullanıcıların tanımlanması ve yetkilendirilmesi.
## Kullanılan Teknolojiler
* **Dil:** C#
* **Platform:** .NET / Windows Forms
* **Geliştirme Ortamı:** Microsoft Visual Studio
* **Veritabanı:** SQL Server
* **Grafik Kütüphanesi:** LiveCharts
* **Versiyon Kontrolü:** Git & GitHub
## Gereksinimler & Kurulum
### Gereksinimler
* Microsoft Visual Studio 2019 veya üzeri
* .NET Framework (Proje hedef sürümü)
* Microsoft SQL Server
* Gerekli NuGet Paketleri (LiveCharts, WinForms )
## Kurulum
### Kurulum Adımları
1. **Repository'yi bilgisayarınıza klonlayın:**
   ```bash
   git clone [https://github.com/ecealankaya/StokTakip_Modern.git](https://github.com/ecealankaya/StokTakip_Modern.git)
   
2. **Projeyi Visual Studio ile Açın:**
StokTakip_Modern.sln dosyasına çift tıklayarak veya Visual Studio içerisinden projeyi açın.

3.**Veritabanı Bağlantısını Yapılandırın:**
Sınıflar/SqlBaglantisi.cs veya App.config dosyası içerisindeki SQL ConnectionString bilgisini kendi yerel SQL Server ayarlarınıza göre güncelleyin:
```text
// Örnek Connection String
SqlConnection baglanti = new SqlConnection(@"Data Source=YOUR_SERVER_NAME;Initial Catalog=YOUR_DATABASE_NAME;Integrated Security=True");
 ```
4.**NuGet Paketlerini Yükleyin:**
Visual Studio üst menüsünden:
Tools > NuGet Package Manager > Restore NuGet Packages
(Veya Package Manager Console üzerinden Update-Package -reinstall komutunu çalıştırabilirsiniz).

5.**Projeyi Çalıştırın:**
Projeyi derleyin: Ctrl + Shift + B
Başlatın: F5

## Veritabanı Mimarisi
Proje, veritabanı işlemleri için ilintisel SQL yapısı kullanmaktadır. Aşağıda projeye ait veritabanı diyagramı yer almaktadır:

<img width="800" height="400" alt="image" src="https://github.com/user-attachments/assets/d2b2ab88-7f30-4c54-9450-e2e88597e31b" />


## Proje Yapısı

```text
StokTakip_Modern/
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
````
## Ekran görüntüleri
### Giriş Ekranı
Kullanıcıların sisteme kullanıcı adı ve şifre bilgileriyle güvenli giriş yapmasını sağlayan login ekranıdır.

<img width="800" height="400" alt="login_ekranı" src="https://github.com/user-attachments/assets/6c709556-33be-42d4-be46-107edeb6598a" />

### Ana Menü
Tüm modüllere erişim sağlayan ana gezinme ekranıdır. LiveCharts entegrasyonu sayesinde stok verileri grafikler üzerinden anlık izlenebilir.

<img width="800" height="400" alt="dashboard" src="https://github.com/user-attachments/assets/2e73c64a-fb11-40db-ba36-c4a8264e175f" />

### Ürün İşlemleri
Ürünlerin sisteme eklenmesi, güncellenmesi, silinmesi ve listelenmesi işlemlerini kapsar.

<img width="800" height="400" alt="urun_islemleri" src="https://github.com/user-attachments/assets/94814913-fadb-4108-8d4a-b2277b7d09fe" />

### Marka İşlemleri
Sistemdeki markaların tanımlandığı ve yönetildiği modüldür.

<img width="800" height="400" alt="marka_islemleri" src="https://github.com/user-attachments/assets/1893363b-38ac-430b-8502-3042568f0877" />

### Kategori İşlemleri
Ürün kategorilerinin dinamik olarak yönetilmesini sağlar.

<img width="800" height="400" alt="kategori_islemleri" src="https://github.com/user-attachments/assets/de38c0d7-8953-4812-9bb2-e1e21ad45f52" />

### Depo Tanımları
İşletmeye ait depoların sisteme tanımlandığı formdur.

<img width="800" height="400" alt="depo_islemleri" src="https://github.com/user-attachments/assets/79b475c1-ade8-450a-94b5-8992c76db6eb" />

### Raf Tanımları
Depolar içerisinde yer alan rafların konum bazlı yönetilmesini sağlar.

<img width="800" height="400" alt="raf_islemleri" src="https://github.com/user-attachments/assets/0a36e33e-af66-43d7-9a31-cff08fbbec75" />

### Stok Operasyonları
Stok hareketlerini yönlendiren, sevkiyat ve tedarikçi bağlantılarını kuran ana operasyon panelidir.

<img width="800" height="400" alt="stok_operasyonları" src="https://github.com/user-attachments/assets/726a93d9-1f9b-423f-af36-8c96381073cd" />

### Stok Hareketleri
Ürünlerin stok giriş ve çıkış kayıtlarının tutulduğu ekrandır.

<img width="800" height="400" alt="stok_hareketleri" src="https://github.com/user-attachments/assets/bc063289-1fb1-4f4d-b58d-3ba66022f9c3" />

### Sevkiyat Yönetimi
Ürün sevkiyat kayıtlarının oluşturulması, güncellenmesi ve takibini sağlar.

<img width="800" height="400" alt="sevkiyat_hareketleri" src="https://github.com/user-attachments/assets/7aa16d7d-065f-4ee6-9936-00ad5abdae36" />

### Tedarikçi İşlemleri
Tedarikçi firma bilgilerinin ve işlem geçmişinin güncel tutulduğu ekrandır.

<img width="800" height="400" alt="tedarikci_islemleri" src="https://github.com/user-attachments/assets/fbede9f7-5ca1-46fa-982f-3c205bbcf675" />

### Kullanıcı ve Yetki
Sistem kullanıcılarının eklenmesi, yetkilerinin düzenlenmesi ve hesap yönetimi için kullanılır.

<img width="800" height="400" alt="kullanıcı_ve_yetki" src="https://github.com/user-attachments/assets/c95d55f4-e08f-4ddf-aa9f-c7d98f52b74b" />













