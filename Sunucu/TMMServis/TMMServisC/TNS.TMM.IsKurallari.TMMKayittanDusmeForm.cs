using System;
using System.Data;
using TNS.KYM;
using TNS.TMM.IsKurallari.Veri;
using TNS.Veri;

namespace TNS.TMM.IsKurallari
{
    /// <summary>
    /// Kayıttan düşme teklif ve onay tutanağının üst bilgileriyle ilgili, iş kuralları
    /// kontrollerini yapıp, veri tabanı bağlantısını alıp, veri tabanı fonksiyonlarını
    /// çağırarak okuma, yazma işlemlerini gerçekleştiren fonksiyonları tutan sınıf
    /// </summary>
    public class TMMKayittanDusmeFormClass
    {
        /// <summary>
        /// Kayıttan düşme teklif ve onay tutanağını varsa güncelleyen, yoksa kaydeden yordam
        /// Kayıttan düşme teklif ve onay tutanağı ve detay bilgileri ile ilgili bir takım kontroller
        /// yapılmakta, sorunsuz bir bilgi kümesi olduğuna karar verilirse kayıt işlemi yapılmaktadır.
        /// Kullanıcı işleme yetkili değilse veya gönderilen KayittanDusmeForm nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="kf">Kayıttan düşme teklif ve onay tutanağı bilgilerini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public static Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, KayittanDusmeForm kf)
        {
            Sonuc sonuc = new Sonuc();
            sonuc.islemSonuc = TMMYetkiKontrol.YazmaYetkisiVarMi(kullanan, kf.muhasebeKod, kf.harcamaKod, kf.ambarKod);
            if (!sonuc.islemSonuc)
            {
                sonuc.hataStr = TasinirMalResI.TMM138;
                return sonuc;
            }

            string hata = string.Empty;
            if (kf.yil <= 0)
                hata += TasinirMalResI.TMM036 + "<br>";
            if (string.IsNullOrEmpty(kf.muhasebeKod))
                hata += TasinirMalResI.TMM142 + "<br>";
            if (string.IsNullOrEmpty(kf.harcamaKod))
                hata += TasinirMalResI.TMM014 + "<br>";
            if (string.IsNullOrEmpty(kf.ambarKod))
                hata += TasinirMalResI.TMM031 + "<br>";
            if (kf.tutanakTarihi.isNull)
                hata += "Tutanak Tarihi boş bırakılamaz" + "<br>";

            if (!string.IsNullOrEmpty(hata))
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = hata;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABKayittanDusmeForm tabKF = new TABKayittanDusmeForm(ve);
            ve.BeginTransaction();

            hata += Genel.KontrolMuhasebeHarcamaAmbar(kf.muhasebeKod, kf.harcamaKod, kf.ambarKod);
            if (!string.IsNullOrEmpty(hata))
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = hata;
                ve.VeriErisimKapat();
                return sonuc;
            }

            try
            {
                if (string.IsNullOrEmpty(kf.tutanakNo))
                    sonuc = tabKF.Kaydet(kf);
                else
                    sonuc = tabKF.Guncelle(kf);

                string tutanakNo = sonuc.anahtar;
                kf.tutanakNo = tutanakNo;

                //Detayları yaz
                if (sonuc.islemSonuc)
                {
                    foreach (KayittanDusmeDetay kd in kf.detay)
                        kd.tutanakNo = tutanakNo;

                    sonuc = TMMKayittanDusmeDetay.Kaydet(ve, kullanan, kf.detay);
                }
            }
            catch (Exception e)
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = e.Message;
            }

            if (sonuc.islemSonuc)
            {
                sonuc.anahtar = kf.tutanakNo;
                sonuc.bilgiStr = TasinirMalResI.TMM017;
                ve.Commit();
            }
            else
            {
                sonuc.hataStr = TasinirMalResI.TMM018 + "<br>" + sonuc.hataStr;
                ve.Rollback();
            }

            ve.VeriErisimKapat();
            return sonuc;
        }

        /// <summary>
        /// Parametre olarak verilen kayıttan düşme teklif ve onay tutanağını ve detaylarını silen yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen KayittanDusmeForm nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="kf">Kayıttan düşme teklif ve onay tutanağının bilgilerini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public static Sonuc Sil(VeriErisim veriErisim, Kullanici kullanan, KayittanDusmeForm kf)
        {
            Sonuc sonuc = new Sonuc();
            sonuc.islemSonuc = TMMYetkiKontrol.YazmaYetkisiVarMi(kullanan, kf.muhasebeKod, kf.harcamaKod, kf.ambarKod);
            if (!sonuc.islemSonuc)
            {
                sonuc.hataStr = TasinirMalResI.TMM050;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABKayittanDusmeForm tabKF = new TABKayittanDusmeForm(ve);
            TABKayittanDusmeDetay tabKFDetay = new TABKayittanDusmeDetay(ve);

            ve.BeginTransaction();

            sonuc = tabKFDetay.Sil(kf);
            if (sonuc.islemSonuc)
                sonuc = tabKF.Sil(kf);

            if (sonuc.islemSonuc)
            {
                sonuc.bilgiStr = TasinirMalResI.TMM162 + "</b>";
                ve.Commit();
            }
            else
            {
                sonuc.hataStr = TasinirMalResI.TMM163 + "<br>" + sonuc.hataStr;
                ve.Rollback();
            }

            ve.VeriErisimKapat();

            return sonuc;
        }

        /// <summary>
        /// Verilen kriterlere uygun olan kayıttan düşme teklif ve onay tutanaklarını döndüren yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen KayittanDusmeForm nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="kf">Kayıttan düşme teklif ve onay tutanağı bilgilerini tutan nesne</param>
        /// <param name="detayVer">Kayıttan düşme teklif ve onay tutanaklarının detay bilgilerinin istenip istenmediği bilgisi</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında kayıttan düşme teklif ve onay tutanakları listesi döndürülür.</returns>
        public static ObjectArray Listele(VeriErisim veriErisim, Kullanici kullanan, KayittanDusmeForm kf, bool detayVer)
        {
            ObjectArray donusDeger = new ObjectArray();

            donusDeger.sonuc.islemSonuc = TMMYetkiKontrol.GormeYetkisiVarMi(kullanan, kf.muhasebeKod, kf.harcamaKod, kf.ambarKod);
            if (!donusDeger.sonuc.islemSonuc)
            {
                donusDeger.sonuc.hataStr = TasinirMalResI.TMM062;
                return donusDeger;
            }

            string hata = string.Empty;
            if (kf.yil <= 0)
                hata += TasinirMalResI.TMM036 + "<br>";

            if (!string.IsNullOrEmpty(hata))
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = hata;
                return donusDeger;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));
            VeriErisim ve2 = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABKayittanDusmeForm tabKF = new TABKayittanDusmeForm(ve);
            IDataReader dr = null;

            try
            {
                dr = tabKF.Listele(kf);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.Message;
                dr.Close();
                ve.VeriErisimKapat();
                ve2.VeriErisimKapat();
                return donusDeger;
            }

            System.Collections.Hashtable isimler = new System.Collections.Hashtable();
            while (dr.Read())
            {
                KayittanDusmeForm gelen = new KayittanDusmeForm();
                gelen.yil = VeriErisim.DegerAl(dr, 0, 0);

                gelen.muhasebeKod = VeriErisim.DegerAl(dr, 1, string.Empty);
                gelen.muhasebeAd = VeriErisim.DegerAl(dr, 6, string.Empty);

                gelen.harcamaKod = VeriErisim.DegerAl(dr, 2, string.Empty);
                gelen.harcamaKod = OrtakFonksiyonlar.KodNoktaKoy(Genel.KodYapiDondur(false), gelen.harcamaKod);
                gelen.harcamaAd = Genel.AdGetir(isimler, gelen.muhasebeKod + "-" + gelen.harcamaKod, "TASHARCAMABIRIM");

                gelen.tutanakNo = VeriErisim.DegerAl(dr, 3, string.Empty);

                gelen.ambarKod = VeriErisim.DegerAl(dr, 4, string.Empty);
                gelen.ambarAd = Genel.AdGetir(isimler, gelen.muhasebeKod + "-" + gelen.harcamaKod + "-" + gelen.ambarKod, "TASAMBAR");

                gelen.tutanakTarihi = VeriErisim.DegerAl(dr, 5);
                //6. kolon muhasebeAdı
                gelen.ilKod = VeriErisim.DegerAl(dr, 7, string.Empty);
                gelen.ilAd = VeriErisim.DegerAl(dr, 8, string.Empty);
                gelen.ilceKod = VeriErisim.DegerAl(dr, 9, string.Empty);
                gelen.ilceAd = VeriErisim.DegerAl(dr, 10, string.Empty);

                //Detayları listele
                if (detayVer)
                {
                    KayittanDusmeDetay kd = new KayittanDusmeDetay();
                    kd.yil = gelen.yil;
                    kd.muhasebeKod = gelen.muhasebeKod;
                    kd.harcamaKod = gelen.harcamaKod;
                    kd.tutanakNo = gelen.tutanakNo;

                    ObjectArray detayBilgi = TMMKayittanDusmeDetay.Listele(ve2, kullanan, kd);
                    if (!detayBilgi.sonuc.islemSonuc)
                    {
                        donusDeger.sonuc = detayBilgi.sonuc;
                        break;
                    }
                    gelen.detay = detayBilgi.objeler;
                }

                donusDeger.objeler.Add(gelen);
            }

            if (donusDeger.sonuc.islemSonuc && donusDeger.objeler.Count == 0)
                donusDeger.sonuc.bilgiStr = TasinirMalResI.TMM029;

            dr.Close();
            ve.VeriErisimKapat();
            ve2.VeriErisimKapat();
            return donusDeger;
        }
    }
}