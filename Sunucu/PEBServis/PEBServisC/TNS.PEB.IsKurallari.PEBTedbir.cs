using System;
using System.Data;
using TNS.KYM;
using TNS.Veri;
using TNS.PEB.IsKurallari.Veri;

namespace TNS.PEB.IsKurallari
{
    /// <summary>
    /// Birimlerin yaptıkları işlerle ilgili aldıkları tedbir bilgileriyle ilgili,
    /// iş kuralları kontrollerini yapıp, veri tabanı bağlantısını alıp,
    /// veri tabanı fonksiyonlarını çağırarak okuma, yazma
    /// işlemlerini gerçekleştiren fonksiyonları tutan sınıf
    /// </summary>
    public class PEBTedbir
    {
        /// <summary>
        /// Tedbir varsa güncelleyen, yoksa kaydeden yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen Tedbir nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// Tedbirin detay bilgileri ve detayın da mevcut durum bilgileri de kayıt edilir.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="tedbir">Tedbir bilgilerini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public static Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, Tedbir tedbir)
        {
            Sonuc sonuc = Yetki.YazmaHakkiVarMi(kullanan, tedbir.kurKod);
            if (!sonuc.islemSonuc)
                return sonuc;

            string hata = string.Empty;
            if (tedbir.kurKod <= 0)
                hata += PerformansButceResI.PEB005 + "<br>";
            if (tedbir.baslangicTarih.isNull)
                hata += PerformansButceResI.PEB108 + "<br>";
            if (!string.IsNullOrEmpty(hata))
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = hata;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriPEB", TNS.KYM.Arac.KimlikBul(kullanan));
            TABTedbir tTedbir = new TABTedbir(ve);
            ve.BeginTransaction();

            if (Yetki.YetkiliBirimMi(kullanan))
            {
                //Tedbir kayıt ediliyor
                if (tTedbir.TedbirVarMi(tedbir))
                    sonuc = tTedbir.GuncelleTedbir(tedbir);
                else
                    sonuc = tTedbir.KaydetTedbir(tedbir);
            }

            //Tedbir detayları kayıt ediliyor
            if (sonuc.islemSonuc)
            {
                if (!tedbir.mevcutTarih.isNull)
                    tTedbir.SilTedbirDetayMevcut((TedbirDetay)tedbir.detaylar[0], false);

                foreach (TedbirDetay td in tedbir.detaylar)
                {
                    if (Yetki.YetkiliBirimMi(kullanan))
                    {
                        if (tTedbir.TedbirDetayVarMi(td))
                            sonuc = tTedbir.GuncelleTedbirDetay(td);
                        else
                            sonuc = tTedbir.KaydetTedbirDetay(td);
                    }

                    if (sonuc.islemSonuc && (!string.IsNullOrEmpty(td.mevcutDurum) || !string.IsNullOrEmpty(td.yetkiliGorusu)))
                    {
                        if (td.mevcutTarih.isNull)
                        {
                            sonuc.islemSonuc = false;
                            sonuc.hataStr = PerformansButceResI.PEB109 + "<br>";
                            break;
                        }
                        sonuc = tTedbir.KaydetTedbirDetayMevcut(td);
                    }

                    if (!sonuc.islemSonuc)
                        break;
                }

                //Silinmesi gereken detaylar ve mevcut durumları siliniyor
                if (sonuc.islemSonuc)
                {
                    TedbirDetay silTD = (TedbirDetay)tedbir.detaylar[tedbir.detaylar.Count - 1];
                    sonuc = tTedbir.SilTedbirDetayMevcut(silTD, true);
                    if (sonuc.islemSonuc && Yetki.YetkiliBirimMi(kullanan))
                        sonuc = tTedbir.SilTedbirDetay(silTD);
                }
            }

            if (sonuc.islemSonuc)
            {
                sonuc.bilgiStr = PerformansButceResI.PEB015;
                ve.Commit();
            }
            else
            {
                sonuc.hataStr = PerformansButceResI.PEB016 + "<br>" + sonuc.hataStr;
                ve.Rollback();
            }

            ve.VeriErisimKapat();
            return sonuc;
        }

        /// <summary>
        /// Verilen kriterlere uygun olan tedbir, detay ve mevcut durum bilgilerini döndüren yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="tedbir">Tedbir bilgilerini tutan nesne</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında tedbir nesnesi döndürülür.</returns>
        public static ObjectArray Listele(VeriErisim veriErisim, Kullanici kullanan, Tedbir tedbir)
        {
            ObjectArray tedbirVAN = new ObjectArray();
            if (tedbir.kurKod > 0)
                tedbirVAN.sonuc = Yetki.OkumaHakkiVarMi(kullanan, tedbir.kurKod);
            else
                tedbirVAN.sonuc = Yetki.OkumaHakkiVarMi(kullanan, (int)ENUMTedbirBirim.YETKILIBIRIM);
            if (!tedbirVAN.sonuc.islemSonuc)
                return tedbirVAN;

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriPEB", TNS.KYM.Arac.KimlikBul(kullanan));
            TABTedbir tTedbir = new TABTedbir(ve);

            IDataReader dr = tTedbir.ListeleTedbir(tedbir);
            Tedbir gelen = new Tedbir();
            gelen.yil = tedbir.yil;
            gelen.kurKod = tedbir.kurKod;
            gelen.kurAd = tedbir.kurAd;
            gelen.mevcutTarih = tedbir.mevcutTarih;
            while (dr.Read())
            {
                gelen.baslangicTarih = VeriErisim.DegerAl(dr, 0);

                TedbirDetay td = new TedbirDetay();
                td.siraNo = VeriErisim.DegerAl(dr, 1, 0);
                td.anaEksen = VeriErisim.DegerAl(dr, 2, string.Empty);
                td.altEksen = VeriErisim.DegerAl(dr, 3, string.Empty);
                td.oncelikTedbir = VeriErisim.DegerAl(dr, 4, string.Empty);
                td.isbirlikciIdare = VeriErisim.DegerAl(dr, 5, string.Empty);
                td.sure = VeriErisim.DegerAl(dr, 6, string.Empty);
                td.islemAciklama = VeriErisim.DegerAl(dr, 7, string.Empty);
                td.baslangicDurum = VeriErisim.DegerAl(dr, 8, string.Empty);

                gelen.detaylar.Add(td);
            }
            dr.Close();

            if (gelen.detaylar.Count > 0)
                tedbirVAN.objeler.Add(gelen);

            if (!tedbir.mevcutTarih.isNull)
            {
                dr = tTedbir.ListeleMevcutDurum(tedbir);
                while (dr.Read())
                {
                    foreach (TedbirDetay td in gelen.detaylar)
                    {
                        if (td.siraNo == VeriErisim.DegerAl(dr, 0, 0))
                        {
                            td.mevcutDurum = VeriErisim.DegerAl(dr, 1, string.Empty);
                            td.yetkiliGorusu = VeriErisim.DegerAl(dr, 2, string.Empty);
                            td.mevcutTarih = gelen.mevcutTarih;
                            break;
                        }
                    }
                }
                dr.Close();
            }

            if (tedbirVAN.objeler.Count == 0)
                tedbirVAN.sonuc.bilgiStr = PerformansButceResI.PEB017;

            ve.VeriErisimKapat();
            return tedbirVAN;
        }

        /// <summary>
        /// Verilen kriterlere uygun olan tedbir mevcut tarihlerini döndüren yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="yil">Yıl kriteri</param>
        /// <param name="kurKod">Birim kodu</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında mevcut tarih listesi döndürülür.</returns>
        public static ObjectArray ListeleMevcutTarih(VeriErisim veriErisim, Kullanici kullanan, int yil, int kurKod)
        {
            ObjectArray tarihVAN = new ObjectArray();
            tarihVAN.sonuc = Yetki.OkumaHakkiVarMi(kullanan);
            if (!tarihVAN.sonuc.islemSonuc)
                return tarihVAN;

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriPEB", TNS.KYM.Arac.KimlikBul(kullanan));
            TABTedbir tTedbir = new TABTedbir(ve);

            IDataReader dr = tTedbir.ListeleMevcutTarih(yil, kurKod);
            while (dr.Read())
            {
                TNSDateTime gelen = new TNSDateTime();
                gelen = VeriErisim.DegerAl(dr, 0);

                tarihVAN.objeler.Add(gelen);
            }
            dr.Close();

            if (tarihVAN.objeler.Count == 0)
                tarihVAN.sonuc.bilgiStr = PerformansButceResI.PEB017;

            ve.VeriErisimKapat();
            return tarihVAN;
        }

        /// <summary>
        /// Tedbir birim listesini döndüren yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında tedbir birimleri listesi döndürülür.</returns>
        public static ObjectArray ListeleTedbirBirim(VeriErisim veriErisim, Kullanici kullanan)
        {
            ObjectArray birimVAN = new ObjectArray();
            birimVAN.sonuc = Yetki.OkumaHakkiVarMi(kullanan);
            if (!birimVAN.sonuc.islemSonuc)
                return birimVAN;

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriPEB", TNS.KYM.Arac.KimlikBul(kullanan));
            TABTedbir tTedbir = new TABTedbir(ve);

            IDataReader dr = tTedbir.ListeleTedbirBirim();
            while (dr.Read())
            {
                TedbirBirim gelen = new TedbirBirim();
                gelen.kurKod = VeriErisim.DegerAl(dr, 0, 0);
                gelen.kurAd = VeriErisim.DegerAl(dr, 1, string.Empty);

                birimVAN.objeler.Add(gelen);
            }
            dr.Close();

            if (birimVAN.objeler.Count == 0)
                birimVAN.sonuc.bilgiStr = PerformansButceResI.PEB017;

            ve.VeriErisimKapat();
            return birimVAN;
        }
    }
}