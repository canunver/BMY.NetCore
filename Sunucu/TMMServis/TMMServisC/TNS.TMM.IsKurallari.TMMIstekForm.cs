using System;
using System.Data;
using TNS.KYM;
using TNS.TMM.IsKurallari.Veri;
using TNS.Veri;

namespace TNS.TMM.IsKurallari
{
    /// <summary>
    /// Taşınır istek formunun üst bilgileriyle ilgili, iş kuralları kontrollerini yapıp,
    /// veri tabanı bağlantısını alıp, veri tabanı fonksiyonlarını çağırarak
    /// okuma, yazma işlemlerini gerçekleştiren fonksiyonları tutan sınıf
    /// </summary>
    public class TMMIstekForm
    {
        /// <summary>
        /// Taşınır istek formunu varsa güncelleyen, yoksa kaydeden yordam
        /// Taşınır istek formu ve detay bilgileri ile ilgili bir takım kontroller yapılmakta,
        /// sorunsuz bir bilgi kümesi olduğuna karar verilirse kayıt işlemi
        /// yapılmaktadır, aksi takdirde işlem yapılmaz ve hata döndürülür.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="iForm">Taşınır istek formu bilgilerini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public static Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, IstekForm iForm)
        {
            Sonuc sonuc = new Sonuc();
            //sonuc.islemSonuc = TMMYetkiKontrol.YazmaYetkisiVarMi(kullanan, iForm.muhasebeKod, iForm.harcamaKod, iForm.ambarKod);
            //if (!sonuc.islemSonuc)
            //{
            //    sonuc.hataStr = TasinirMalResI.TMM138;
            //    return sonuc;
            //}

            string hata = string.Empty;
            if (iForm.yil <= 0)
                hata += TasinirMalResI.TMM036 + "<br>";
            if (string.IsNullOrEmpty(iForm.muhasebeKod))
                hata += TasinirMalResI.TMM142 + "<br>";
            if (string.IsNullOrEmpty(iForm.harcamaKod))
                hata += TasinirMalResI.TMM014 + "<br>";
            if (string.IsNullOrEmpty(iForm.ambarKod))
                hata += TasinirMalResI.TMM031 + "<br>";
            if (string.IsNullOrEmpty(iForm.istekYapanKod))
                hata += TasinirMalResI.TMM402 + "<br>";
            if (iForm.belgeTarihi.isNull)
                hata += "Belge tarihi boş bırakılamaz." + "<br>";

            if (!string.IsNullOrEmpty(hata))
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = hata;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABIstekForm tabIF = new TABIstekForm(ve);
            ve.BeginTransaction();

            hata += Genel.KontrolMuhasebeHarcamaAmbar(iForm.muhasebeKod, iForm.harcamaKod, iForm.ambarKod);

            if (!string.IsNullOrEmpty(hata))
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = hata;
                ve.VeriErisimKapat();
                return sonuc;
            }

            try
            {
                if (string.IsNullOrEmpty(iForm.belgeNo))
                    sonuc = tabIF.Kaydet(iForm);
                else
                    sonuc = tabIF.Guncelle(iForm);

                string belgeNo = sonuc.anahtar;
                iForm.belgeNo = belgeNo;

                //Detayları yaz
                if (sonuc.islemSonuc)
                {
                    foreach (IstekDetay id in iForm.detay)
                        id.belgeNo = sonuc.anahtar.Trim();

                    sonuc = IstekDetayClass.Kaydet(ve, kullanan, iForm.detay);
                    sonuc.anahtar = belgeNo;
                }
            }
            catch (Exception e)
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = e.Message;
            }

            if (sonuc.islemSonuc)
            {
                sonuc.anahtar = iForm.belgeNo;
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
        /// Verilen kriterlere uygun olan taşınır istek formlarını döndüren yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen IstekForm nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="iForm">Taşınır istek formu bilgilerini tutan nesne</param>
        /// <param name="detayVer">Taşınır istek formlarının detay bilgilerinin istenip istenmediği bilgisi</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında taşınır istek formları listesi döndürülür.</returns>
        public static ObjectArray Listele(VeriErisim veriErisim, Kullanici kullanan, IstekForm iForm, bool detayVer)
        {
            ObjectArray donusDeger = new ObjectArray();

            //Harcama Yönetiminden çağrıldığı için yetki kontrolü yapılmamalı. Ama önceden kapatılmış

            //donusDeger.sonuc.islemSonuc = TMMYetkiKontrol.GormeYetkisiVarMi(kullanan, iForm.muhasebeKod, iForm.harcamaKod, iForm.ambarKod);
            //if (!donusDeger.sonuc.islemSonuc)
            //{
            //    donusDeger.sonuc.hataStr = TasinirMalResI.TMM062;
            //    return donusDeger;
            //}

            string hata = string.Empty;
            if (iForm.yil <= 0)
                hata += TasinirMalResI.TMM036 + "<br>";
            //if (string.IsNullOrEmpty(sf.muhasebeKod))
            //    hata += TasinirMalResI.TMM142 + "<br>";
            //if (string.IsNullOrEmpty(sf.harcamaKod))
            //    hata += TasinirMalResI.TMM014 + "<br>";
            //if (string.IsNullOrEmpty(sf.ambarKod))
            //    hata += TasinirMalResI.TMM031 + "<br>";
            if (kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.MISAFIR) && string.IsNullOrEmpty(iForm.istekYapanKod))
                hata += TasinirMalResI.TMM402 + "<br>";

            if (!string.IsNullOrEmpty(hata))
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = hata;
                return donusDeger;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));
            VeriErisim ve2 = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABIstekForm tabIF = new TABIstekForm(ve);
            IDataReader dr = null;

            try
            {
                dr = tabIF.Listele(iForm);
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
                IstekForm gelen = new IstekForm();
                gelen.yil = VeriErisim.DegerAl(dr, 0, 0);

                gelen.muhasebeKod = VeriErisim.DegerAl(dr, 1, string.Empty);
                gelen.muhasebeAd = Genel.AdGetir(isimler, gelen.muhasebeKod, "TASMUHASEBE");

                gelen.harcamaKod = VeriErisim.DegerAl(dr, 2, string.Empty);
                gelen.harcamaKod = OrtakFonksiyonlar.KodNoktaKoy(Genel.KodYapiDondur(false), gelen.harcamaKod);
                gelen.harcamaAd = Genel.AdGetir(isimler, gelen.muhasebeKod + "-" + gelen.harcamaKod, "TASHARCAMABIRIM");

                gelen.ambarKod = VeriErisim.DegerAl(dr, 3, string.Empty);
                gelen.ambarAd = Genel.AdGetir(isimler, gelen.muhasebeKod + "-" + gelen.harcamaKod + "-" + gelen.ambarKod, "TASAMBAR");

                gelen.belgeNo = VeriErisim.DegerAl(dr, 4, string.Empty);
                gelen.belgeTarihi = VeriErisim.DegerAl(dr, 5);

                gelen.istekYapanKod = VeriErisim.DegerAl(dr, 6, string.Empty);
                gelen.istekYapanAd = Genel.AdGetir(isimler, gelen.istekYapanKod, "TASPERSONEL");

                gelen.hediyelik = VeriErisim.DegerAl(dr, 7, 0);

                gelen.ilKod = VeriErisim.DegerAl(dr, 8, string.Empty);
                gelen.ilAd = VeriErisim.DegerAl(dr, 9, string.Empty);
                gelen.ilceKod = VeriErisim.DegerAl(dr, 10, string.Empty);
                gelen.ilceAd = VeriErisim.DegerAl(dr, 11, string.Empty);

                //Detayları listele
                if (detayVer)
                {
                    IstekDetay id = new IstekDetay();
                    id.yil = gelen.yil;
                    id.muhasebeKod = gelen.muhasebeKod;
                    id.harcamaKod = gelen.harcamaKod;
                    id.ambarKod = gelen.ambarKod;
                    id.belgeNo = gelen.belgeNo;

                    ObjectArray detayBilgi = IstekDetayClass.Listele(ve2, kullanan, id);
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