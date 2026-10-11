using System;
using System.Data;
using TNS.IKY;
using TNS.IKY.IsKurallari.Veri;
using TNS.KYM;
using TNS.Veri;
using TNS.UZY;

namespace TNS.IKY.IsKurallari
{
    public class IKYPersonelResim
    {
        private static readonly string BosKisiResmiBase64 =
        @"/9j/4AAQSkZJRgABAgEASABIAAD/4QlCRXhpZgAATU0AKgAAAAgABwESAAMAAAABAAEAAAEa
AAUAAAABAAAAYgEbAAUAAAABAAAAagEoAAMAAAABAAIAAAExAAIAAAAQAAAAcgEyAAIA
AAAUAAAAggAAAABHb29nbGUAAABIAAAASAAAAAEAAJAHAAQAAAAwMTAwAJAHAAQAAAAw
MTAwAP/bAEMAAwICAwICAwMDAwMEAwMDBAYEBAQEBAgGBgUGCQgKCgkICQkKDA8MCgsO
DQwKCgwSDRASFxUYGRgYGBga/8AAEQgB9gH2AwEiAAIRAQMRAf/EAB8AAAEFAQEBAQEBA
AAAAAAAAAABAgMEBQYHCAkKC//EALUQAAIBAwMCBAMFBQQEAAABfQECAwAEEQUSITFBBh
NRYQcicRQygZGhCCNCscEVUtHwJDNicoIJChYXGBkaJSYnKCkqNDU2Nzg5OkNERUZHSEl
KU1RVVldYWVpjZGVmZ2hpanN0dXZ3eHl6g4SFhoeIiYqSk5SVlpeYmZqio6Slpqeoqaqs
tba3uLm6wsPExcbHyMnK0tPU1dbX2Nna4eLj5OXm5+jp6vHy8/T19vf4+fr/2gAMAwEAA
hEDEQA/AO2iiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAK
KKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKAC
iiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigA
ooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigD/9k=";



        private static readonly byte[] BosKisiResmiBytes = Convert.FromBase64String(BosKisiResmiBase64);

        /// <summary>
        /// Kullanıcı yetkisini göz önüne alarak girilen bilgilere bağlı olarak yeni bir kayıt ekler veya varolan kaydı günceller.
        /// </summary>
        /// <param name="ve">Bağlantıya ait bilgileri içerir</param>
        /// <param name="resim">The resim.</param>
        /// <returns>
        /// Sonuc yapısı içerisindeki; islemSonuc:komut çalışmışsa true, çalışmamışsa false; hataStr:Hata olmuşsa, hata bilgisini, döndürür.
        /// </returns>
        public static Sonuc Kaydet(VeriErisim ve, PersonelResim resim)
        {
            Sonuc sonuc = new Sonuc(false, "", "");

            //sonuc = Kontrol(ENUMKontrolTip.KAYDET, resim);
            //if (!sonuc.islemSonuc)
            //    return sonuc;

            ve = VeriErisim.VeriErisimBul(ve, "BaglantiSatiriIKY", TNS.KYM.Arac.KimlikBul(null));
            ve.BeginTransaction();

            TABPersonelResim to = new TABPersonelResim(ve);

            if (to.VarMi(resim))
                sonuc = to.Guncelle(resim);
            else
                sonuc = to.Kaydet(resim);

            if (sonuc.islemSonuc)
                ve.Commit();
            else
                ve.Rollback();

            ve.VeriErisimKapat();
            return sonuc;
        }

        /// <summary>
        /// Girilen veriye uygun bilgileri listeleme işlemini yapar.
        /// </summary>
        /// <param name="ve">Bağlantıya ait bilgileri içerir</param>
        /// <param name="tcKimklikNo">The tc kimklik no.</param>
        /// <returns>
        /// ObjectArray yapısı içinde, uygun olan verileri döndürür.
        /// </returns>
        public static byte[] Getir(VeriErisim ve, int kod, int buyukluk, int en, int boy)
        {
            byte[] liste = null;

            ve = VeriErisim.VeriErisimBul(ve, "BaglantiSatiriIKY", TNS.KYM.Arac.KimlikBul(null));
            TABPersonelResim to = new TABPersonelResim(ve);
            IDataReader dr = to.Getir(kod);

            if (dr != null)
            {
                if (dr.Read())
                {
                    if (!dr.IsDBNull(0))
                        liste = (byte[])dr.GetValue(0);
                }
                dr.Close();
            }
            if (liste == null)
            {
                return BosKisiResmiBytes;
                //string bosResimAdres = TNS.DEG.Arac.DegiskenDegerBul(0, "BosKisiResmi");
                //try
                //{
                //    System.IO.MemoryStream msResim = new System.IO.MemoryStream();
                //    System.Drawing.Image img = System.Drawing.Image.FromFile(bosResimAdres);
                //    img.Save(msResim, System.Drawing.Imaging.ImageFormat.Jpeg);
                //    liste = msResim.ToArray();
                //}
                //catch (Exception ex)
                //{
                //    OrtakFonksiyonlar.HataStrYaz(bosResimAdres + " okunamadı!" + ex.Message);
                //}
            }
            ve.VeriErisimKapat();
            return TNS.IKY.Arac.ResimAyarla(liste, buyukluk, en, boy);
        }

        /// <summary>
        /// Girilen veri ile uyuşan verinin silinmesini gerçekleştirir.
        /// </summary>
        /// <param name="ve">Bağlantıya ait bilgileri içerir</param>
        /// <param name="resim">Girilen bilgilere bağlı olarak oluşturulmuş, işleme sokulacak verileri içerir</param>
        /// <returns>
        /// Sonuc yapısı içerisindeki; islemSonuc:komut çalışmışsa true, çalışmamışsa false; hataStr:Hata olmuşsa, hata bilgisini, döndürür.
        /// </returns>
        public static Sonuc Sil(VeriErisim ve, PersonelResim resim)
        {
            Sonuc sonuc = new Sonuc(false, "", "");

            ve = VeriErisim.VeriErisimBul(ve, "BaglantiSatiriIKY", TNS.KYM.Arac.KimlikBul(null));
            ve.BeginTransaction();

            TABPersonelResim to = new TABPersonelResim(ve);

            sonuc = to.Sil(resim);

            if (sonuc.islemSonuc)
                ve.Commit();
            else
                ve.Rollback();

            ve.VeriErisimKapat();
            return sonuc;
        }
    }
}
