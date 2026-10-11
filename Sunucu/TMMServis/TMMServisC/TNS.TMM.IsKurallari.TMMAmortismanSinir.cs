using System;
using System.Data;
using TNS.KYM;
using TNS.TMM.IsKurallari.Veri;
using TNS.Veri;

namespace TNS.TMM.IsKurallari
{
    /// <summary>
    /// Amortisman alt sınır tutar bilgileriyle ilgili, iş kuralları kontrollerini yapıp,
    /// veri tabanı bağlantısını alıp, veri tabanı fonksiyonlarını çağırarak
    /// okuma, yazma işlemlerini gerçekleştiren fonksiyonları tutan sınıf
    /// </summary>
    public class TMMAmortismanSinir
    {
        /// <summary>
        /// Amortisman sınırı varsa güncelleyen, yoksa kaydeden yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen AmortismanSinir nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="sinir">Amortisman sınır bilgisini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public static Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, AmortismanSinir sinir)
        {
            Sonuc sonuc = new Sonuc(false, TasinirMalResI.TMM069, "");

            if (!TMMYetkiKontrol.Yoneticimi(kullanan))
            {
                sonuc.hataStr = TasinirMalResI.TMM030;
                sonuc.islemSonuc = false;
                return sonuc;
            }

            string hata = string.Empty;
            if (sinir.yil <= 0)
                hata = TasinirMalResI.TMM036 + "<br>";
            if (sinir.sinir <= 0)
                hata += TasinirMalResI.TMM070;
            if (!string.IsNullOrEmpty(hata))
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = hata;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));

            //if (TMMAmortisman.AmortismanYapilmisMi(ve, sinir.yil))
            //{
            //    sonuc.islemSonuc = false;
            //    sonuc.hataStr = TMMAmortisman.amortismanHata;
            //    ve.VeriErisimKapat();
            //    return sonuc;
            //}

            TABAmortismanSinir tSinir = new TABAmortismanSinir(ve);
            ve.BeginTransaction();

            try
            {
                sonuc = tSinir.Sil(sinir.yil);
                if (sonuc.islemSonuc)
                    sonuc = tSinir.Kaydet(sinir);
            }
            catch (Exception e)
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = e.Message;
            }

            if (sonuc.islemSonuc)
                ve.Commit();
            else
                ve.Rollback();

            ve.VeriErisimKapat();
            return sonuc;
        }

        /// <summary>
        /// Parametre olarak verilen yılın amortisman sınırını döndüren yordam
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="yil">Yıl kriteri</param>
        /// <returns>Amortisman alt sınırı döndürülür.</returns>
        public static decimal AmortismanSinirVer(VeriErisim veriErisim, Kullanici kullanan, int yil)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABAmortismanSinir tSinir = new TABAmortismanSinir(ve);
            decimal sinir = tSinir.AmortismanSinirVer(yil);
            ve.VeriErisimKapat();
            return sinir;
        }

        public static decimal AmortismanSinirTasitVer(VeriErisim veriErisim, Kullanici kullanan, int yil)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABAmortismanSinir tSinir = new TABAmortismanSinir(ve);
            decimal sinirTasit = tSinir.AmortismanSinirTasitVer(yil);
            ve.VeriErisimKapat();
            return sinirTasit;
        }

        /// <summary>
        /// Parametre olarak verilen yılın amortisman sınırını döndüren yordam
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="yil">Yıl kriteri</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında amortisman sınır bilgileri döndürülür.</returns>
        public static ObjectArray Listele(VeriErisim veriErisim, Kullanici kullanan, int yil)
        {
            ObjectArray listeVAN = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABAmortismanSinir tSinir = new TABAmortismanSinir(ve);
            TABAmortisman tAmortisman = new TABAmortisman(ve);
            int basYil = 0;
            int bitYil = DateTime.Now.Year;

            //Merkez bankasının Test ortamında taşıt amortisman testi yapabilmesi için eklendi.
            if (TNS.TMM.Arac.MerkezBankasiKullaniyor() && OrtakFonksiyonlar.WebConfigOku("AmortismanDonemKontroluOlmasin", "0") == "1")
                bitYil = DateTime.Now.Year + 4;

            if (yil == 0)
            {
                basYil = tAmortisman.MinYilBul();
                for (int i = basYil; i <= bitYil; i++)
                {
                    AmortismanSinir gelen = new AmortismanSinir();
                    gelen.yil = i;
                    gelen.sinir = (double)0;
                    listeVAN.objeler.Add(gelen);
                }
            }

            IDataReader dr = tSinir.Listele(yil);
            if (dr != null)
            {
                while (dr.Read())
                {
                    AmortismanSinir gelen = new AmortismanSinir();
                    gelen.yil = VeriErisim.DegerAl(dr, 0, 0);
                    gelen.sinir = VeriErisim.DegerAl(dr, 1, (double)0);
                    gelen.mifBelgeNo = VeriErisim.DegerAl(dr, 2, "");
                    gelen.sinirTasit = VeriErisim.DegerAl(dr, 3, (double)0);

                    if (yil != 0)
                    {
                        listeVAN.objeler.Add(gelen);
                    }
                    else
                    {
                        if (gelen.yil >= basYil && gelen.yil <= bitYil)
                        {
                            int i = gelen.yil - basYil;
                            AmortismanSinir eski = (AmortismanSinir)listeVAN.objeler[i];
                            eski.sinir = gelen.sinir;
                            eski.mifBelgeNo = gelen.mifBelgeNo;
                            eski.sinirTasit = gelen.sinirTasit;
                        }
                    }
                }
                dr.Close();
            }
            else
                listeVAN.sonuc.DegerAta(ve.hata);

            ve.VeriErisimKapat();
            return listeVAN;
        }

        internal static Sonuc MIFKaydet(VeriErisim veriErisim, Kullanici kullanan, int yil, string mifNo)
        {
            Sonuc sonuc = new Sonuc(false, TasinirMalResI.TMM069, "");

            if (!TMMYetkiKontrol.Yoneticimi(kullanan))
            {
                sonuc.hataStr = TasinirMalResI.TMM030;
                sonuc.islemSonuc = false;
                return sonuc;
            }

            string hata = string.Empty;
            if (yil <= 0)
                hata = TasinirMalResI.TMM036 + "<br>";
            if (!string.IsNullOrEmpty(hata))
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = hata;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));

            TABAmortismanSinir tSinir = new TABAmortismanSinir(ve);
            ve.BeginTransaction();

            try
            {
                sonuc = tSinir.MIFKaydet(yil, mifNo);
            }
            catch (Exception e)
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = e.Message;
            }

            if (sonuc.islemSonuc)
                ve.Commit();
            else
                ve.Rollback();

            ve.VeriErisimKapat();
            return sonuc;
        }
    }
}