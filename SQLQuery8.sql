

USE atiktakip_;
GO

CREATE TABLE Kullanicilar(
	KullaniciID INT PRIMARY KEY IDENTITY(1,1),
	Ad NVARCHAR(50) NOT NULL,
	Soyad NVARCHAR(50) NOT NULL,
	Eposta NVARCHAR(100) UNIQUE NOT NULL,
	Sifre NVARCHAR(100) NOT NULL,
	KayitTarihi DATETIME DEFAULT GETDATE()
);

CREATE TABLE AtikTurleri(
	AtikTuruID INT PRIMARY KEY IDENTITY(1,1),
	TurAdi NVARCHAR(50) NOT NULL,
	GeriDonusumTipi NVARCHAR(50),
	Aciklama NVARCHAR(255)
);

CREATE TABLE AtikKayitlari(
	KayitID INT PRIMARY KEY IDENTITY(1,1),
	KullaniciID INT FOREIGN KEY REFERENCES Kullanicilar(KullaniciID),
	AtikTuruID INT FOREIGN KEY REFERENCES AtikTurleri(AtikTuruID),
	Miktar DECIMAL(10,2) NOT NULL,
	Birim NVARCHAR(20) NOT NULL,
	AtilmaTarihi DATETIME DEFAULT GETDATE(),
	GeriDonusumeGitti BIT DEFAULT 0
);

CREATE TABLE GeriDonusumMerkezleri(
	MerkezID INT PRIMARY KEY IDENTITY(1,1),
	MerkezAdi NVARCHAR(100) NOT NULL,
	Adres NVARCHAR(255),
	Telefon NVARCHAR(20),
	KabulEdilenAtikTurleri NVARCHAR(255)
);