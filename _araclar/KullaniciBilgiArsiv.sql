/*
  KULLANICI_BILGI arşivi (kullanıcı veri tabanında çalıştırılır; BaglantiSatiriKYM'nin veri tabanı).

  Uygulama bu işleri kendisi de yapar (Istemci\Kullanici\KullaniciBilgiArsivIsi.cs, ayda bir kez, ayın ilk günü saat 03:00'ten sonra).
  Tablo çok büyükse ilk index oluşturma ve ilk taşıma uzun sürebilir; bunu mesai dışında elle yapmak için bu betik kullanılır.
  Betik tekrar tekrar çalıştırılabilir: her adım "yoksa" koşulludur.

  1) KULLANICI_BILGI_ARSIV: KULLANICI_BILGI ile aynı alanlar (aynı tipler).
  2) İki tabloya ISLEMTARIHI index'i (liste ve grafik sorguları tarihe göre arar).
  3) 2 yıldan eski kayıtlar 5000'lik partilerle arşive taşınır. Her parti tek komuttur (DELETE ... OUTPUT INTO):
     bir satır ya taşınır ya yerinde kalır. Saklama süresini değiştirmek için @SaklamaYil'i değiştirin.
*/
SET NOCOUNT ON;

IF OBJECT_ID(N'KULLANICI_BILGI_ARSIV', N'U') IS NULL
    SELECT TOP 0 KULLANICIKODU, ISLEMTARIHI, ISLEMYERI, SAYFAADI, BILGI1, BILGI2, BILGI3, BILGI4, BILGI5
    INTO KULLANICI_BILGI_ARSIV
    FROM KULLANICI_BILGI;
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_KULLANICI_BILGI_ISLEMTARIHI' AND object_id = OBJECT_ID(N'KULLANICI_BILGI'))
    CREATE INDEX IX_KULLANICI_BILGI_ISLEMTARIHI ON KULLANICI_BILGI (ISLEMTARIHI);
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_KULLANICI_BILGI_ARSIV_ISLEMTARIHI' AND object_id = OBJECT_ID(N'KULLANICI_BILGI_ARSIV'))
    CREATE INDEX IX_KULLANICI_BILGI_ARSIV_ISLEMTARIHI ON KULLANICI_BILGI_ARSIV (ISLEMTARIHI);
GO

DECLARE @SaklamaYil int = 2, @Parti int = 5000, @Adet int = 1, @Toplam bigint = 0;
DECLARE @Sinir datetime = DATEADD(year, -@SaklamaYil, CAST(GETDATE() AS date));

WHILE @Adet > 0
BEGIN
    DELETE TOP (@Parti) FROM KULLANICI_BILGI
    OUTPUT DELETED.KULLANICIKODU, DELETED.ISLEMTARIHI, DELETED.ISLEMYERI, DELETED.SAYFAADI,
           DELETED.BILGI1, DELETED.BILGI2, DELETED.BILGI3, DELETED.BILGI4, DELETED.BILGI5
    INTO KULLANICI_BILGI_ARSIV (KULLANICIKODU, ISLEMTARIHI, ISLEMYERI, SAYFAADI, BILGI1, BILGI2, BILGI3, BILGI4, BILGI5)
    WHERE ISLEMTARIHI < @Sinir;

    SET @Adet = @@ROWCOUNT;
    SET @Toplam = @Toplam + @Adet;
    IF @Adet > 0 WAITFOR DELAY '00:00:00.200';
END

PRINT 'Arşive taşınan kayıt: ' + CAST(@Toplam AS varchar(20));
GO
