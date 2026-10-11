using System.Data;
using TNS.TMM.IsKurallari.Veri;
using TNS.Veri;

namespace TNS.TMM.IsKurallari
{
    /// <summary>
    /// Firma bilgileriyle ilgili, iş kuralları kontrollerini yapıp,
    /// veri tabanı bağlantısını alıp, veri tabanı fonksiyonlarını çağırarak
    /// okuma, yazma işlemlerini gerçekleştiren fonksiyonları tutan sınıf
    /// </summary>
    public class TMMFirma
    {
        /// <summary>
        /// Parametre olarak verilen firmaları varsa güncelleyen, yoksa kaydeden yordam
        /// </summary>
        /// <param name="firmaBilgileri">Firma bilgileri listesini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc Kaydet(ObjectArray firmaBilgileri)
        {
            Sonuc sonuc = new Sonuc(false, TasinirMalResI.TMM069, "");

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(null));
            VeriErisim ve2 = VeriErisim.VeriErisimBul(null, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(null));

            ve.BeginTransaction();

            TABFirma to = new TABFirma(ve);
            TABGenel tg = new TABGenel(ve2);

            foreach (FirmaBilgisi firma in firmaBilgileri.objeler)
            {
                if (firma.kod == 0)
                    firma.kod = OrtakFonksiyonlar.ConvertToInt(tg.BelgeNoAl("FIRMA", "0"), 0);

                sonuc = to.FirmaKaydet(firma);

                if (!sonuc.islemSonuc)
                    break;
            }

            if (sonuc.islemSonuc)
            {
                ve.Commit();
                sonuc.bilgiStr = TasinirMalResI.TMM017;
            }
            else
                ve.Rollback();

            ve.VeriErisimKapat();
            ve2.VeriErisimKapat();
            return sonuc;
        }

        /// <summary>
        /// Parametre olarak verilen firmaları silen yordam
        /// </summary>
        /// <param name="firmaBilgileri">Firma bilgileri listesini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc Sil(ObjectArray firmaBilgileri)
        {
            Sonuc sonuc = new Sonuc(false, TasinirMalResI.TMM071, "");

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(null));

            ve.BeginTransaction();
            TABFirma to = new TABFirma(ve);

            foreach (FirmaBilgisi firma in firmaBilgileri.objeler)
            {
                if (firma.kod > 0)
                    sonuc = to.FirmaSil(firma);

                if (!sonuc.islemSonuc)
                    break;
            }

            if (sonuc.islemSonuc)
            {
                ve.Commit();
                sonuc.bilgiStr = TasinirMalResI.TMM025;
            }
            else
                ve.Rollback();

            ve.VeriErisimKapat();

            return sonuc;
        }

        /// <summary>
        /// Verilen kriterlere uygun olan firma bilgilerini döndüren yordam
        /// </summary>
        /// <param name="pFirma">Firma bilgisini tutan nesne</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında firma listesi döndürülür.</returns>
        public ObjectArray Listele(FirmaBilgisi pFirma)
        {
            ObjectArray iVAN = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(null));

            TABFirma to = new TABFirma(ve);
            IDataReader r = to.FirmaListele(pFirma);

            if (to.Hata() == "")
            {
                while (r.Read())
                {
                    FirmaBilgisi firma = new FirmaBilgisi();

                    firma.kullaniciKodu = VeriErisim.DegerAl(r, 0, "");
                    firma.kod = VeriErisim.DegerAl(r, 1, 0);
                    firma.ad = VeriErisim.DegerAl(r, 2, "");
                    firma.vno = VeriErisim.DegerAl(r, 3, "");
                    firma.vd = VeriErisim.DegerAl(r, 4, "");
                    firma.banka = VeriErisim.DegerAl(r, 5, "");
                    firma.hesapNo = VeriErisim.DegerAl(r, 6, "");
                    firma.tel = VeriErisim.DegerAl(r, 7, "");
                    firma.fax = VeriErisim.DegerAl(r, 8, "");

                    iVAN.objeler.Add(firma);
                }
            }

            r.Close();

            ve.VeriErisimKapat();

            return iVAN;
        }
    }
}