using System;
using System.Data;
using TNS.KYM;
using TNS.TMM.IsKurallari.Veri;
using TNS.Veri;

namespace TNS.TMM.IsKurallari
{
    /// <summary>
    /// Taşınır istek formunun detay bilgileriyle ilgili, iş kuralları kontrollerini yapıp,
    /// veri tabanı bağlantısını alıp, veri tabanı fonksiyonlarını çağırarak
    /// okuma, yazma işlemlerini gerçekleştiren fonksiyonları tutan sınıf
    /// </summary>
    public class IstekDetayClass
    {
        /// <summary>
        /// Taşınır istek formunun detay bilgilerini kaydeden yordam
        /// Taşınır istek formunun detay bilgileri ile ilgili bir takım kontroller yapılmakta,
        /// sorunsuz bir bilgi kümesi olduğuna karar verilirse kayıt işlemi
        /// yapılmaktadır, aksi takdirde işlem yapılmaz ve hata döndürülür.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="id">Taşınır istek formunun detay bilgileri listesini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public static Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, TNSCollection id)
        {
            Sonuc sonuc = new Sonuc();

            if (id.Count <= 0)
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = TasinirMalResI.TMM141;
                return sonuc;
            }

            IstekDetay d = (IstekDetay)id[0];

            string hata = string.Empty;

            if (d.yil <= 0)
                hata += TasinirMalResI.TMM036 + "<br>";
            if (string.IsNullOrEmpty(d.muhasebeKod))
                hata += TasinirMalResI.TMM142 + "<br>";
            if (string.IsNullOrEmpty(d.harcamaKod))
                hata += TasinirMalResI.TMM014 + "<br>";
            if (string.IsNullOrEmpty(d.ambarKod))
                hata += TasinirMalResI.TMM031 + "<br>";
            if (string.IsNullOrEmpty(d.belgeNo))
                hata += TasinirMalResI.TMM143 + "<br>";

            hata += Genel.KontrolMuhasebeHarcamaAmbar(d.muhasebeKod, d.harcamaKod, d.ambarKod);

            if (!string.IsNullOrEmpty(hata))
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = hata;
                return sonuc;
            }

            //sonuc.islemSonuc = TMMYetkiKontrol.YazmaYetkisiVarMi(kullanan, d.muhasebeKod, d.harcamaKod, d.ambarKod);
            //if (!sonuc.islemSonuc)
            //{
            //    sonuc.hataStr = TasinirMalResI.TMM138;
            //    return sonuc;
            //}

            int i = 0;
            foreach (IstekDetay detay in id)
            {
                hata = string.Empty;

                i++;

                if (string.IsNullOrEmpty(detay.hesapPlanKod))
                    hata += string.Format(TasinirMalResI.TMM144, i.ToString());

                hata += Genel.KontrolHesapPlan(kullanan, detay.hesapPlanKod, i, true, false);

                if (!string.IsNullOrEmpty(hata))
                {
                    sonuc.islemSonuc = false;
                    sonuc.hataStr += hata;
                }
            }

            if (!sonuc.islemSonuc)
                return sonuc;

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABIstekDetay tabID = new TABIstekDetay(ve);
            ve.BeginTransaction();

            try
            {
                tabID.Sil((IstekDetay)id[0]);
                foreach (IstekDetay detay in id)
                {
                    if (detay.istenilenMiktar > 0 || detay.karsilananMiktar > 0)
                    {
                        sonuc = tabID.Kaydet(detay);
                        if (!sonuc.islemSonuc)
                            break;
                    }
                }
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
        /// Verilen taşınır istek formunun detay bilgilerini döndüren yordam
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="id">Taşınır istek formunun kriter bilgilerini tutan nesne</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında taşınır istek formuna ait detay bilgileri listesi döndürülür.</returns>
        public static ObjectArray Listele(VeriErisim veriErisim, Kullanici kullanan, IstekDetay id)
        {
            ObjectArray donusDeger = new ObjectArray();

            //donusDeger.sonuc.islemSonuc = TMMYetkiKontrol.GormeYetkisiVarMi(kullanan, id.muhasebeKod, id.harcamaKod, id.ambarKod);
            //if (!donusDeger.sonuc.islemSonuc)
            //{
            //    donusDeger.sonuc.hataStr = TasinirMalResI.TMM062;
            //    return donusDeger;
            //}

            //string hata = string.Empty;
            //if (sd.yil <= 0)
            //    hata += TasinirMalResI.TMM036 + "<br>";
            //if (string.IsNullOrEmpty(sd.muhasebeKod))
            //    hata += TasinirMalResI.TMM142 + "<br>";
            //if (string.IsNullOrEmpty(sd.harcamaKod))
            //    hata += TasinirMalResI.TMM014 + "<br>";
            //if (string.IsNullOrEmpty(sd.ambarKod))
            //    hata += TasinirMalResI.TMM031 + "<br>";
            //if (string.IsNullOrEmpty(sd.sayimNo))
            //    hata += "Sayım No boş geliyor.<br>";
            //if (string.IsNullOrEmpty(sd.hesapPlanKod))
            //    hata += "Taşınır Kodu boş bırakılamaz.<br>";
            //else
            //    hata += Genel.KontrolHesapPlan(kullanan, sd.hesapPlanKod);

            //if (!string.IsNullOrEmpty(hata))
            //{
            //    donusDeger.sonuc.islemSonuc = false;
            //    donusDeger.sonuc.hataStr = hata;
            //    return donusDeger;
            //}

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABIstekDetay tabID = new TABIstekDetay(ve);
            IDataReader dr = null;

            try
            {
                dr = tabID.Listele(id);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.Message;
                dr.Close();
                ve.VeriErisimKapat();
                return donusDeger;
            }

            System.Collections.Hashtable isimler = new System.Collections.Hashtable();
            while (dr.Read())
            {
                IstekDetay gelen = new IstekDetay();
                gelen.yil = VeriErisim.DegerAl(dr, 0, 0);

                gelen.muhasebeKod = VeriErisim.DegerAl(dr, 1, string.Empty);
                gelen.muhasebeAd = Genel.AdGetir(isimler, gelen.muhasebeKod, "TASMUHASEBE");

                gelen.harcamaKod = VeriErisim.DegerAl(dr, 2, string.Empty);
                gelen.harcamaKod = OrtakFonksiyonlar.KodNoktaKoy(Genel.KodYapiDondur(false), gelen.harcamaKod);
                gelen.harcamaAd = Genel.AdGetir(isimler, gelen.muhasebeKod + "-" + gelen.harcamaKod, "TASHARCAMABIRIM");

                gelen.ambarKod = VeriErisim.DegerAl(dr, 3, string.Empty);
                gelen.ambarAd = Genel.AdGetir(isimler, gelen.muhasebeKod + "-" + gelen.harcamaKod + "-" + gelen.ambarKod, "TASAMBAR");

                gelen.belgeNo = VeriErisim.DegerAl(dr, 4, string.Empty);

                gelen.hesapPlanKod = VeriErisim.DegerAl(dr, 5, string.Empty);
                gelen.hesapPlanKod = OrtakFonksiyonlar.KodNoktaKoy(Genel.KodYapiDondur(true), gelen.hesapPlanKod);
                gelen.hesapPlanAd = Genel.AdGetir(isimler, gelen.hesapPlanKod, "TASHESAPPLAN");

                gelen.istenilenMiktar = VeriErisim.DegerAl(dr, 6, (decimal)0);
                gelen.karsilananMiktar = VeriErisim.DegerAl(dr, 7, (decimal)0);

                gelen.kdvOran = VeriErisim.DegerAl(dr, 8, 0);
                gelen.birimFiyat = VeriErisim.DegerAl(dr, 9, (decimal)0);
                gelen.aciklama = VeriErisim.DegerAl(dr, 10, string.Empty);

                gelen.olcuBirimKod = VeriErisim.DegerAl(dr, 11, 0);
                gelen.olcuBirimAd = VeriErisim.DegerAl(dr, 12, string.Empty);

                donusDeger.objeler.Add(gelen);
            }

            if (donusDeger.objeler.Count == 0)
                donusDeger.sonuc.bilgiStr = TasinirMalResI.TMM029;

            donusDeger.sonuc.islemSonuc = true;

            dr.Close();
            ve.VeriErisimKapat();
            return donusDeger;
        }
    }
}