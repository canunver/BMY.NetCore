using System;
using System.Collections;
using System.Data;
using TNS.KYM;
using TNS.TMM.IsKurallari.Veri;
using TNS.Veri;

namespace TNS.TMM.IsKurallari
{
    /// <summary>
    /// Zimmet fişi bilgileriyle ilgili, iş kuralları kontrollerini yapıp,
    /// veri tabanı bağlantısını alıp, veri tabanı fonksiyonlarını çağırarak
    /// okuma, yazma işlemlerini gerçekleştiren fonksiyonları tutan sınıf
    /// </summary>
    public class TMMZimmetForm
    {
        /// <summary>
        /// Zimmet fişini varsa güncelleyen, yoksa kaydeden yordam
        /// Zimmet fişi ve detay bilgileri ile ilgili bir takım kontroller yapılmakta,
        /// sorunsuz bir bilgi kümesi olduğuna karar verilirse kayıt işlemi yapılmaktadır.
        /// Kullanıcı işleme yetkili değilse veya gönderilen ZimmetForm ve ObjectArray nesneleri eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="zimmet">Zimmet fişi bilgilerini tutan nesne</param>
        /// <param name="zimmetDetay">Zimmet fişinin detay bilgileri listesini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public static Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, ZimmetForm zimmet, ObjectArray zimmetDetay)
        {
            Sonuc sonuc = new Sonuc();

            sonuc.islemSonuc = TMMYetkiKontrol.YazmaYetkisiVarMi(kullanan, zimmet.muhasebeKod, zimmet.harcamaBirimKod, zimmet.ambarKod);
            if (!sonuc.islemSonuc)
                sonuc.islemSonuc = TMMYetkiKontrol.YazmaYetkisiVarMiKullaniciBirimi(kullanan, zimmet.muhasebeKod, zimmet.harcamaBirimKod, zimmet.ambarKod);

            if (!sonuc.islemSonuc)
            {
                sonuc.hataStr = TasinirMalResI.TMM050;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));
            VeriErisim ve2 = VeriErisim.VeriErisimBul(null, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));

            for (int i = 0; i < zimmetDetay.objeler.Count; i++)
            {
                if (((ZimmetDetay)zimmetDetay.objeler[i]).prSicilNo <= 0)
                    ((ZimmetDetay)zimmetDetay.objeler[i]).prSicilNo = (int)TMMServis.UzaySayiDegerGetir("TASPRSICILGETIR", zimmet.muhasebeKod + "-" + zimmet.harcamaBirimKod + "-" + ((ZimmetDetay)zimmetDetay.objeler[i]).gorSicilNo, true, "");
            }

            sonuc = Kontrol(ve, kullanan, zimmet, zimmetDetay);

            if (!sonuc.islemSonuc)
            {
                ve.VeriErisimKapat();
                ve2.VeriErisimKapat();
                return sonuc;
            }

            TABZimmetForm tabZimmetForm = new TABZimmetForm(ve);
            TABZimmetDetay tabZimmetDetay = new TABZimmetDetay(ve);
            TABGenel tabGenel = new TABGenel(ve2);

            ve.BeginTransaction();

            string sayacKod = "ZIM" + zimmet.yil.ToString() + zimmet.muhasebeKod + zimmet.harcamaBirimKod.Replace(".", "") + zimmet.belgeTur.ToString();

            if (string.IsNullOrEmpty(zimmet.fisNo))
            {
                zimmet.fisNo = tabGenel.BelgeNoAl(sayacKod, "000000");

                if (string.IsNullOrEmpty(zimmet.fisNo))
                {
                    sonuc.islemSonuc = false;
                    sonuc.hataStr = TasinirMalResI.TMM083;
                    ve.VeriErisimKapat();
                    ve2.VeriErisimKapat();
                    return sonuc;
                }
            }

            zimmet.ipAdresi = kullanan.telefon;
            sonuc = tabZimmetForm.Kaydet(zimmet);

            if (!sonuc.islemSonuc)
            {
                ve.Rollback();
                ve.VeriErisimKapat();
                ve2.VeriErisimKapat();
                return sonuc;
            }

            //Bu fişe ait satırları sil
            ZimmetDetay zSil = new ZimmetDetay();
            zSil.fisNo = zimmet.fisNo;
            zSil.yil = zimmet.yil;
            zSil.harcamaBirimKod = zimmet.harcamaBirimKod;
            zSil.muhasebeKod = zimmet.muhasebeKod;
            zSil.belgeTur = zimmet.belgeTur;

            sonuc = tabZimmetDetay.Sil(zSil);

            if (!sonuc.islemSonuc)
            {
                ve.Rollback();
                ve.VeriErisimKapat();
                ve2.VeriErisimKapat();
                return sonuc;
            }

            foreach (ZimmetDetay zd in zimmetDetay.objeler)
            {
                zd.fisNo = zimmet.fisNo;

                sonuc = tabZimmetDetay.Kaydet(zd);

                if (!sonuc.islemSonuc)
                {
                    ve.Rollback();
                    ve.VeriErisimKapat();
                    ve2.VeriErisimKapat();
                    return sonuc;
                }
            }

            if (sonuc.islemSonuc)
            {
                sonuc.anahtar = zimmet.fisNo;
                sonuc.bilgiStr = string.Format(TasinirMalResI.TMM199, zimmet.fisNo);
                ve.Commit();
            }
            else
            {
                sonuc.hataStr = TasinirMalResI.TMM200 + "";
                ve.Rollback();
            }

            ve.VeriErisimKapat();
            ve2.VeriErisimKapat();
            return sonuc;
        }

        /// <summary>
        /// Verilen kriterlere uygun olan zimmet fişlerini döndüren yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen ZimmetForm nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="zimmet">Zimmet fişi bilgilerini tutan nesne</param>
        /// <param name="kriter">Sorgulama için ek kriter bilgilerini tutan nesne</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında zimmet fişleri listesi döndürülür.</returns>
        public static ObjectArray Listele(VeriErisim veriErisim, Kullanici kullanan, ZimmetForm zimmet, ZimmetFormKriter kriter, string eOnayDurum)
        {
            ObjectArray bilgi = new ObjectArray();

            bilgi.sonuc.islemSonuc = TMMYetkiKontrol.GormeYetkisiVarMi(kullanan, zimmet.muhasebeKod, zimmet.harcamaBirimKod, zimmet.ambarKod);
            if (!bilgi.sonuc.islemSonuc)
                bilgi.sonuc.islemSonuc = TMMYetkiKontrol.GormeYetkisiVarMiKullaniciBirimi(kullanan, zimmet.muhasebeKod, zimmet.harcamaBirimKod, zimmet.ambarKod);

            if (!bilgi.sonuc.islemSonuc)
            {
                bilgi.sonuc.hataStr = TasinirMalResI.TMM060;
                return bilgi;
            }

            if (!(kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.SISTEMYONETICISI)))
            {
                if (zimmet.yil == 0)
                    bilgi.sonuc.hataStr += TasinirMalResI.TMM036 + "<br>";
                //Yetkisi var ise boş bırakabilir
                //if (zimmet.muhasebeKod == string.Empty)
                //    bilgi.sonuc.hataStr += TasinirMalResI.TMM142 + "<br>";
                //if (zimmet.harcamaBirimKod == string.Empty)
                //    bilgi.sonuc.hataStr += TasinirMalResI.TMM014 + "<br>";
                //if (zimmet.ambarKod == string.Empty)
                //    bilgi.sonuc.hataStr += TasinirMalResI.TMM031 + "<br>";
                if (zimmet.belgeTur == 0)
                    bilgi.sonuc.hataStr += TasinirMalResI.TMM292 + "<br>";

                if (bilgi.sonuc.hataStr != string.Empty)
                {
                    bilgi.sonuc.islemSonuc = false;
                    return bilgi;
                }
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABZimmetForm tabZimmetForm = new TABZimmetForm(ve);

            IDataReader dr = tabZimmetForm.Listele(zimmet, kriter, eOnayDurum);

            while (dr.Read())
            {
                ZimmetForm zimmetForm = new ZimmetForm();

                zimmetForm.yil = zimmet.yil;
                zimmetForm.muhasebeKod = VeriErisim.DegerAl(dr, 1, string.Empty);
                zimmetForm.harcamaBirimKod = VeriErisim.DegerAl(dr, 2, string.Empty);
                zimmetForm.fisNo = VeriErisim.DegerAl(dr, 3, string.Empty);
                zimmetForm.belgeTur = VeriErisim.DegerAl(dr, 13, 0);

                ObjectArray bilgiAsil = FisAc(null, kullanan, zimmetForm);
                ((ZimmetForm)bilgiAsil[0]).muhasebeAd = TMMServis.UzayYaziDegerGetir("TASMUHASEBE", ((ZimmetForm)bilgiAsil[0]).muhasebeKod, true, "");
                ((ZimmetForm)bilgiAsil[0]).ambarAd = TMMServis.UzayYaziDegerGetir("TASAMBAR", ((ZimmetForm)bilgiAsil[0]).muhasebeKod + "-" + ((ZimmetForm)bilgiAsil[0]).harcamaBirimKod + "-" + ((ZimmetForm)bilgiAsil[0]).ambarKod, true, "");
                if (bilgiAsil.sonuc.islemSonuc)
                    bilgi.Ekle(bilgiAsil[0]);
                else
                {
                    bilgi.sonuc.hataStr = bilgiAsil.sonuc.hataStr;
                    return bilgi;
                }
            }

            dr.Close();
            ve.VeriErisimKapat();

            return bilgi;
        }


        /// <summary>
        /// Verilen kriterlere uygun olan zimmet fişlerini  alan eonay bilgisi ile döndüren yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen ZimmetForm nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="zimmet">Zimmet fişi bilgilerini tutan nesne</param>
        /// <param name="kriter">Sorgulama için ek kriter bilgilerini tutan nesne</param>
        /// <param name="eonaykriter"> Sorgulama için eonay kriter bilgilerini tutan nesne</param> 
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında zimmet fişleri listesi döndürülür.</returns>
        public static ObjectArray ListeleEOnayBilgisiyle(VeriErisim veriErisim, Kullanici kullanan, ZimmetForm zimmet, ZimmetFormKriter kriter, ZimmetFormEOnayKriter eonaykriter)
        {
            ObjectArray bilgi = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABZimmetForm tabZimmetForm = new TABZimmetForm(ve);

            IDataReader dr = tabZimmetForm.ListeleEOnayli(zimmet, kriter, eonaykriter);

            while (dr.Read())
            {
                ZimmetFormWithEonay zimmetFormEonay = new ZimmetFormWithEonay();

                zimmetFormEonay.yil = zimmet.yil;
                zimmetFormEonay.muhasebeKod = VeriErisim.DegerAl(dr, 1, string.Empty);
                zimmetFormEonay.harcamaBirimKod = VeriErisim.DegerAl(dr, 2, string.Empty);
                zimmetFormEonay.fisNo = VeriErisim.DegerAl(dr, 3, string.Empty);
                zimmetFormEonay.belgeTur = VeriErisim.DegerAl(dr, 13, 0);

                zimmetFormEonay.emailKimden = VeriErisim.DegerAl(dr, 15, string.Empty);
                zimmetFormEonay.emailKimdenAd = VeriErisim.DegerAl(dr, 16, string.Empty);
                zimmetFormEonay.emailKime = VeriErisim.DegerAl(dr, 17, string.Empty);
                zimmetFormEonay.emailKimeAd = VeriErisim.DegerAl(dr, 18, string.Empty);
                zimmetFormEonay.emailGondermeTarih = VeriErisim.DegerAl(dr, 19);
                zimmetFormEonay.emailCevapTarih = VeriErisim.DegerAl(dr, 20);
                zimmetFormEonay.emaildurum = VeriErisim.DegerAl(dr, 21, 0);

                zimmetFormEonay.muhasebeAd = TMMServis.UzayYaziDegerGetir("TASMUHASEBE", zimmetFormEonay.muhasebeKod, true, "");
                zimmetFormEonay.harcamaBirimAd = TMMServis.UzayYaziDegerGetir("TASHARCAMABIRIM", zimmetFormEonay.muhasebeKod + "-" + zimmetFormEonay.harcamaBirimKod, true, "");



                //zimmetFormEonay.emailKimdenAd = TMMServis.UzayYaziDegerGetir("KULLANICIAD", zimmetFormEonay.emailKimden + "- 0", true, string.Empty);
                //zimmetFormEonay.emailKimeAd = TMMServis.UzayYaziDegerGetir("TASPERSONEL", zimmetFormEonay.emailKime, true, string.Empty);

                bilgi.Ekle(zimmetFormEonay);
            }

            dr.Close();
            ve.VeriErisimKapat();

            return bilgi;
        }


        /// <summary>
        /// Verilen zimmet fişinin detay bilgilerini döndüren yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen ZimmetForm nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="zimmet">Zimmet fişi bilgilerini tutan nesne</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında zimmet fişinin detay bilgileri döndürülür.</returns>
        public static ObjectArray ListeleDetay(VeriErisim veriErisim, Kullanici kullanan, ZimmetForm zimmet)
        {
            string kodYapisi = TNS.TMM.Arac.DegiskenDegerBul(0, "/KODYAPI/KOD13/KODYAPI");

            ObjectArray bilgi = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABZimmetDetay tabZimmetDetay = new TABZimmetDetay(ve);

            ZimmetDetay zd = new ZimmetDetay();
            zd.yil = zimmet.yil;
            zd.muhasebeKod = zimmet.muhasebeKod;
            zd.harcamaBirimKod = zimmet.harcamaBirimKod;
            zd.fisNo = zimmet.fisNo;
            zd.belgeTur = zimmet.belgeTur;

            IDataReader dr = tabZimmetDetay.Listele(zd);

            while (dr.Read())
            {
                ZimmetDetay zDetay = new ZimmetDetay();

                zDetay.prSicilNo = VeriErisim.DegerAl(dr, 5, 0);
                zDetay.siraNo = VeriErisim.DegerAl(dr, 6, 0);
                zDetay.hesapPlanKod = OrtakFonksiyonlar.KodNoktaKoy(kodYapisi, VeriErisim.DegerAl(dr, 7, ""));
                //if (!string.IsNullOrEmpty(kullanan.mernis))
                //    zDetay.gorSicilNo = TMM.IsKurallari.Genel.GorSicilNoGetir(zimmet.muhasebeKod, zimmet.harcamaBirimKod, zDetay.prSicilNo);
                zDetay.ozellik = VeriErisim.DegerAl(dr, 8, "");
                zDetay.kdvOran = VeriErisim.DegerAl(dr, 9, 0);
                zDetay.birimFiyat = VeriErisim.DegerAl(dr, 10, (decimal)0);
                zDetay.teslimDurum = VeriErisim.DegerAl(dr, 11, "");
                zDetay.hesapPlanAd = VeriErisim.DegerAl(dr, 12, "");

                //Barkod yazdırmada gerekli oldu.
                zDetay.ozellikSicil.adi = VeriErisim.DegerAl(dr, 13, "");
                zDetay.ozellikSicil.yazarAdi = VeriErisim.DegerAl(dr, 14, "");
                zDetay.ozellikSicil.disSicilNo = VeriErisim.DegerAl(dr, 15, "");
                zDetay.ozellikSicil.yayinTarihi = VeriErisim.DegerAl(dr, 16, "");
                zDetay.ozellikSicil.ciltNo = VeriErisim.DegerAl(dr, 17, "");
                zDetay.ozellikSicil.yeriKonusu = VeriErisim.DegerAl(dr, 18, "");

                zDetay.ozellikSicil.markaAd = VeriErisim.DegerAl(dr, 19, "");
                zDetay.ozellikSicil.modelAd = VeriErisim.DegerAl(dr, 20, "");

                zDetay.ozellikSicil.saseNo = VeriErisim.DegerAl(dr, 21, "");
                zDetay.ozellikSicil.ekNo = VeriErisim.DegerAl(dr, 22, "");
                zDetay.ozellikSicil.giai = VeriErisim.DegerAl(dr, 23, "");

                zDetay.gorSicilNo = VeriErisim.DegerAl(dr, 24, "");



                //Eski zimmet bilgisi.
                zDetay.ozellikSicilEski.bulunduguYer = VeriErisim.DegerAl(dr, 25, "");
                zDetay.ozellikSicilEski.bulunduguYerAd = VeriErisim.DegerAl(dr, 26, "");


                if (Genel.rfIdVarMi)
                    zDetay.rfIdNo = Genel.PrSicilNoRFIDDondur(zDetay.prSicilNo);

                bilgi.Ekle(zDetay);
            }

            if (bilgi.ObjeSayisi < 1)
            {
                bilgi.sonuc.islemSonuc = false;
                bilgi.sonuc.hataStr = TasinirMalResI.TMM088;
            }

            dr.Close();
            ve.VeriErisimKapat();

            return bilgi;
        }

        /// <summary>
        /// Verilen kriterlere uygun olan zimmet fişini döndüren yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen ZimmetForm nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="zimmet">Zimmet fişi bilgilerini tutan nesne</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında zimmet fişi döndürülür.</returns>
        public static ObjectArray FisAc(VeriErisim veriErisim, Kullanici kullanan, ZimmetForm zimmet)
        {
            ObjectArray bilgi = new ObjectArray();
            string kodYapisiHB = TNS.TMM.Arac.DegiskenDegerBul(0, "/KODYAPI/KOD10/KODYAPI");

            bilgi.sonuc.islemSonuc = TMMYetkiKontrol.GormeYetkisiVarMi(kullanan, zimmet.muhasebeKod, zimmet.harcamaBirimKod, zimmet.ambarKod);

            if (!bilgi.sonuc.islemSonuc)
                bilgi.sonuc.islemSonuc = TMMYetkiKontrol.GormeYetkisiVarMiKullaniciBirimi(kullanan, zimmet.muhasebeKod, zimmet.harcamaBirimKod, zimmet.ambarKod);


            if (!bilgi.sonuc.islemSonuc)
            {
                bilgi.sonuc.hataStr = TasinirMalResI.TMM060;
                return bilgi;
            }

            if (string.IsNullOrEmpty(zimmet.fisNo))
            {
                bilgi.sonuc.islemSonuc = false;
                bilgi.sonuc.hataStr = TasinirMalResI.TMM293;
                return bilgi;
            }
            else if (zimmet.yil == 0)
            {
                bilgi.sonuc.islemSonuc = false;
                bilgi.sonuc.hataStr = TasinirMalResI.TMM294;
                return bilgi;
            }
            else if (string.IsNullOrEmpty(zimmet.muhasebeKod))
            {
                bilgi.sonuc.islemSonuc = false;
                bilgi.sonuc.hataStr = TasinirMalResI.TMM013;
                return bilgi;
            }
            else if (string.IsNullOrEmpty(zimmet.harcamaBirimKod))
            {
                bilgi.sonuc.islemSonuc = false;
                bilgi.sonuc.hataStr = TasinirMalResI.TMM014;
                return bilgi;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABZimmetForm tabZimmetForm = new TABZimmetForm(ve);

            IDataReader dr = tabZimmetForm.FisAc(zimmet);

            while (dr.Read())
            {
                zimmet.yil = VeriErisim.DegerAl(dr, 0, 0);
                zimmet.muhasebeKod = VeriErisim.DegerAl(dr, 1, "");
                zimmet.harcamaBirimKod = VeriErisim.DegerAl(dr, 2, "");
                zimmet.fisNo = VeriErisim.DegerAl(dr, 3, "");
                zimmet.ambarKod = VeriErisim.DegerAl(dr, 4, "");
                zimmet.fisTarih = VeriErisim.DegerAl(dr, 5);
                zimmet.ambarKod = VeriErisim.DegerAl(dr, 4, string.Empty);
                zimmet.tip = VeriErisim.DegerAl(dr, 6, 0);
                zimmet.kimeGitti = VeriErisim.DegerAl(dr, 7, string.Empty);
                zimmet.nereyeGitti = VeriErisim.DegerAl(dr, 8, string.Empty);
                zimmet.vermeDusme = VeriErisim.DegerAl(dr, 9, 0);

                if (zimmet.vermeDusme == (int)ENUMZimmetVermeDusme.ZIMMETTENDUSME)
                    zimmet.dusmeTarih = VeriErisim.DegerAl(dr, 10);

                zimmet.islemYapan = VeriErisim.DegerAl(dr, 11, string.Empty);
                zimmet.islemTarih = VeriErisim.DegerAl(dr, 12);
                zimmet.belgeTur = VeriErisim.DegerAl(dr, 13, 0);
                zimmet.durum = VeriErisim.DegerAl(dr, 14, 0);
                zimmet.ilKod = VeriErisim.DegerAl(dr, 15, string.Empty);
                zimmet.ilAd = VeriErisim.DegerAl(dr, 16, string.Empty);
                zimmet.ilceKod = VeriErisim.DegerAl(dr, 17, string.Empty);
                zimmet.ilceAd = VeriErisim.DegerAl(dr, 18, string.Empty);
                zimmet.harcamaBirimAd = VeriErisim.DegerAl(dr, 19, string.Empty);
                zimmet.kisiAdi = VeriErisim.DegerAl(dr, 20, string.Empty);//ad

                zimmet.harcamaBirimKod = OrtakFonksiyonlar.KodNoktaKoy(kodYapisiHB, zimmet.harcamaBirimKod);

                bilgi.Ekle(zimmet);
            }

            if (bilgi.ObjeSayisi == 0)
            {
                bilgi.sonuc.islemSonuc = false;
                bilgi.sonuc.hataStr = TasinirMalResI.TMM086;
            }
            //else if (bilgi.ObjeSayisi > 1) //Buraya hiç bir zaman girmezsen sevinirim ;)
            //{
            //    bilgi.sonuc.islemSonuc = false;
            //    bilgi.sonuc.hataStr = "Aynı yıl, harcama kodu ve belge numarasına sahip birden fazla belge bulundu!";
            //}
            else
            {
                bilgi.sonuc.islemSonuc = true;
            }

            dr.Close();
            ve.VeriErisimKapat();

            if (!(kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.SISTEMYONETICISI)) && bilgi.sonuc.islemSonuc)
            {
                bilgi.sonuc.islemSonuc = TMMYetkiKontrol.GormeYetkisiVarMi(kullanan, zimmet.muhasebeKod, zimmet.harcamaBirimKod, zimmet.ambarKod);

                if (!bilgi.sonuc.islemSonuc)
                    bilgi.sonuc.islemSonuc = TMMYetkiKontrol.GormeYetkisiVarMiKullaniciBirimi(kullanan, zimmet.muhasebeKod, zimmet.harcamaBirimKod, zimmet.ambarKod);

                if (!bilgi.sonuc.islemSonuc)
                {
                    bilgi.objeler.Clear();
                    bilgi.sonuc.hataStr = TasinirMalResI.TMM060;
                    return bilgi;
                }
            }

            return bilgi;
        }

        /// <summary>
        /// Parametre olarak verilen zimmet fişi ve detay bilgilerinin doğruluğunu kontrol eden yordam
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="zimmet">Zimmet fişi bilgilerini tutan nesne</param>
        /// <param name="zimmetDetay">Zimmet fişinin detay bilgileri listesini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        private static Sonuc Kontrol(VeriErisim veriErisim, Kullanici kullanan, ZimmetForm zimmet, ObjectArray zimmetDetay)
        {
            Sonuc sonuc = new Sonuc();

            if (zimmet.yil == 0)
                sonuc.hataStr += TasinirMalResI.TMM036 + "<br>";
            if (string.IsNullOrEmpty(zimmet.muhasebeKod))
                sonuc.hataStr += TasinirMalResI.TMM013 + "<br>";
            if (string.IsNullOrEmpty(zimmet.harcamaBirimKod))
                sonuc.hataStr += TasinirMalResI.TMM014 + "<br>";
            if (string.IsNullOrEmpty(zimmet.ambarKod))
                sonuc.hataStr += TasinirMalResI.TMM031 + "<br>";
            if (zimmet.fisTarih.isNull)
                sonuc.hataStr += TasinirMalResI.TMM077 + "<br>";
            if (zimmet.fisTarih.Oku().Year != zimmet.yil)
                sonuc.hataStr += string.Format(TasinirMalResI.TMM250, zimmet.yil, zimmet.fisTarih.ToString()) + "<br>";

            if (!string.IsNullOrEmpty(zimmet.muhasebeKod))
            {
                if (string.IsNullOrEmpty(TMMServis.UzayYaziDegerGetir("TASMUHASEBE", zimmet.muhasebeKod, true, "")))
                    sonuc.hataStr += string.Format(TasinirMalResI.TMM096, zimmet.muhasebeKod) + "<br>";
            }
            if (!string.IsNullOrEmpty(zimmet.muhasebeKod) && !string.IsNullOrEmpty(zimmet.harcamaBirimKod))
            {
                if (string.IsNullOrEmpty(TMMServis.UzayYaziDegerGetir("TASHARCAMABIRIM", zimmet.muhasebeKod + "-" + zimmet.harcamaBirimKod, true, "")))
                    sonuc.hataStr += string.Format(TasinirMalResI.TMM097, zimmet.muhasebeKod, zimmet.harcamaBirimKod) + "<br>";
            }
            if (!string.IsNullOrEmpty(zimmet.muhasebeKod) && !string.IsNullOrEmpty(zimmet.harcamaBirimKod) && !string.IsNullOrEmpty(zimmet.ambarKod))
            {
                if (string.IsNullOrEmpty(TMMServis.UzayYaziDegerGetir("TASAMBAR", zimmet.muhasebeKod + "-" + zimmet.harcamaBirimKod + "-" + zimmet.ambarKod, true, "")))
                    sonuc.hataStr += string.Format(TasinirMalResI.TMM098, zimmet.muhasebeKod, zimmet.harcamaBirimKod, zimmet.ambarKod) + "<br>";
            }

            if (sonuc.hataStr == "")
                sonuc.hataStr += AmbarKapalimi(zimmet);

            if (string.IsNullOrEmpty(zimmet.kimeGitti))
                sonuc.hataStr += TasinirMalResI.TMM295 + "<br>";
            else
            {
                string personelKontrol = System.Configuration.ConfigurationManager.AppSettings.Get("ZimmetDusmePersonelKontrolYapma");

                string personel = TMMServis.UzayYaziDegerGetir("TASPERSONEL", zimmet.kimeGitti, true, "");
                if (personel == "" && OrtakFonksiyonlar.ConvertToInt(personelKontrol, 0) == 0)
                    sonuc.hataStr += TasinirMalResI.TMM296 + "(" + zimmet.kimeGitti + ")<br>";
            }

            if (zimmet.belgeTur == (int)ENUMZimmetBelgeTur.DAYANIKLITL)
            {
                if (string.IsNullOrEmpty(zimmet.nereyeGitti))
                    sonuc.hataStr += TasinirMalResI.TMM297 + "<br>";
                else
                {
                    string oda = TMMServis.UzayYaziDegerGetir("TASODA", zimmet.muhasebeKod + "-" + zimmet.harcamaBirimKod + "-" + zimmet.nereyeGitti, true, "");
                    if (oda == "")
                        sonuc.hataStr += TasinirMalResI.TMM298 + "<br>";
                }
            }

            veriErisim = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));

            TABZimmetForm tabZimmetForm = new TABZimmetForm(veriErisim);

            //Onay kontrol
            if (!string.IsNullOrEmpty(zimmet.fisNo)) //Update
            {
                IDataReader dr = tabZimmetForm.BelgeDurumGetir(zimmet);
                if (!dr.Read())
                    sonuc.hataStr += string.Format(TasinirMalResI.TMM270, zimmet.fisNo) + "<br>";
                else
                {
                    int durum = VeriErisim.DegerAl(dr, 0, 0);
                    if (durum == (int)ENUMBelgeDurumu.ONAYLI)
                        sonuc.hataStr += string.Format(TasinirMalResI.TMM271, zimmet.fisNo) + "<br>";
                }
                dr.Close();
                zimmet.durum = (int)ENUMBelgeDurumu.DEGISTIRILDI;
            }
            else
                zimmet.durum = (int)ENUMBelgeDurumu.YENI;

            //Detay kontrol
            if (zimmetDetay.ObjeSayisi < 1)
                sonuc.hataStr += TasinirMalResI.TMM299 + "<br>";
            //else if (zimmetDetay.ObjeSayisi > 5000)
            //    sonuc.hataStr += TasinirMalResI.TMM300 + "<br>";
            else
            {
                //if (zimmet.tip == (int)ENUMZimmetTipi.TASITMAKINE && zimmet.vermeDusme == (int)ENUMZimmetVermeDusme.ZIMMETVERME)//EGO istediği için yapıldı
                //{
                //    if (zimmetDetay.ObjeSayisi > 1)
                //        sonuc.hataStr += TasinirMalResI.TMM301;
                //}

                int flag = 0;
                //Aynı demirbaşı 2 kere mi zimmet edeceksin a saygısız
                for (int i = 0; i < zimmetDetay.ObjeSayisi - 1; i++)
                {
                    for (int j = i + 1; j < zimmetDetay.ObjeSayisi; j++)
                    {
                        if (((ZimmetDetay)zimmetDetay[i]).prSicilNo == ((ZimmetDetay)zimmetDetay[j]).prSicilNo)
                        {
                            sonuc.hataStr += string.Format(TasinirMalResI.TMM302, ((ZimmetDetay)zimmetDetay[i]).siraNo) + "<br>";
                            flag = 1;
                            break;
                        }
                    }
                    if (flag == 1)
                        break;
                }

                //*************************************************************************
                string zimmetlenenHesapKodlari = "@";
                string prSicilNoListe = "";

                foreach (ZimmetDetay zd in zimmetDetay.objeler)
                {
                    if (zimmetlenenHesapKodlari != "") zimmetlenenHesapKodlari += " ";
                    zimmetlenenHesapKodlari += zd.hesapPlanKod;

                    if (zd.prSicilNo > 0)
                    {
                        if (prSicilNoListe != "") prSicilNoListe += "|";
                        prSicilNoListe += zd.prSicilNo;
                    }
                }

                Hashtable htHesapPlanKod = new Hashtable();
                HesapPlaniSatir hs = new HesapPlaniSatir();
                hs.hesapKodAciklama = zimmetlenenHesapKodlari;
                ObjectArray bilgiHp = new TMMHesapPlani().Listele(kullanan, hs, new Sayfalama());
                foreach (HesapPlaniSatir hp in bilgiHp.objeler)
                {
                    htHesapPlanKod[hp.hesapKod.Replace(".", "")] = hp;
                }

                Hashtable htPrSicilNo = new Hashtable();
                if (prSicilNoListe != "")
                {
                    ObjectArray prSicilNolar = TMM.IsKurallari.Genel.SicilNoluMalzemeAmbardami(veriErisim, zimmet.muhasebeKod, zimmet.harcamaBirimKod, zimmet.ambarKod, prSicilNoListe);
                    foreach (SicilNoHareket hp in prSicilNolar.objeler)
                    {
                        htPrSicilNo[hp.prSicilNo] = hp.prSicilNo;
                    }
                }
                //*************************************************************************

                foreach (ZimmetDetay zd in zimmetDetay.objeler)
                {
                    if (zd.prSicilNo == 0)
                        sonuc.hataStr += string.Format(TasinirMalResI.TMM303, zd.siraNo) + "<br>";

                    if (string.IsNullOrEmpty(zd.hesapPlanKod))
                        sonuc.hataStr += string.Format(TasinirMalResI.TMM304, zd.siraNo) + "<br>";

                    //HesapKodu kontrolu
                    //************************************************************
                    string kontrolHesapKod = zd.hesapPlanKod.Replace(".", "");
                    if (htHesapPlanKod[kontrolHesapKod] == null)
                        sonuc.hataStr += TasinirMalResI.TMM218 + "<br>";
                    else
                    {
                        HesapPlaniSatir hp = (HesapPlaniSatir)htHesapPlanKod[kontrolHesapKod];
                        if (hp.seviye < 2)
                            sonuc.hataStr += string.Format(TasinirMalResI.TMM305, zd.siraNo, hs.hesapKod) + "<br>";
                    }
                    //************************************************************

                    //PrSicilNo Kontrolu
                    //Verilen sicil numarası "ambarda" kayıtlımı
                    if (zimmet.vermeDusme == (int)ENUMZimmetVermeDusme.ZIMMETVERME)
                    {
                        if (htPrSicilNo[zd.prSicilNo] == null)
                            sonuc.hataStr += string.Format(TasinirMalResI.TMM306, zd.siraNo, TasinirMalResI.TMM090 + "<br>");
                    }

                    if (sonuc.hataStr == "")
                    {
                        //Verilen sicil numarası haracama biriminde kayıtlımı (ilk bu kontrol vardı. fakat zimmet yaparken malzeme seçiminden sonra ambar değiştirildiği görüldü. yukarıdaki kontrol eklendi.)
                        string sicilGetir = TMM.IsKurallari.Genel.GorSicilNoGetir(zimmet.muhasebeKod, zimmet.harcamaBirimKod.Replace(".", ""), zd.prSicilNo);

                        if (sicilGetir == "")
                            sonuc.hataStr += string.Format(TasinirMalResI.TMM307, zd.siraNo) + "<br>";
                        else if (!TNS.TMM.Arac.MerkezBankasiKullaniyor() && zd.hesapPlanKod.Replace(".", "") != sicilGetir.Substring(0, zd.hesapPlanKod.Replace(".", "").Length))
                            sonuc.hataStr += string.Format(TasinirMalResI.TMM308, zd.siraNo) + "<br>";
                    }

                    if (zimmet.vermeDusme == (int)ENUMZimmetVermeDusme.ZIMMETVERME && sonuc.hataStr == "")
                    {
                        IDataReader drVarMi = tabZimmetForm.ZimmetVarMi(zd.prSicilNo);
                        if (drVarMi.Read())
                        {
                            int islemTuru = VeriErisim.DegerAl(drVarMi, 5, 0);
                            string zimmetTuru = islemTuru == (int)(ENUMIslemTipi.ZFVERME) ? " (Kişiye Zimmet)" : " (Ortak Alana Zimmet)";
                            sonuc.hataStr += string.Format(TasinirMalResI.TMM309, zd.siraNo, VeriErisim.DegerAl(drVarMi, 1, 0).ToString());
                            sonuc.hataStr += " Muhasebe: " + VeriErisim.DegerAl(drVarMi, 2, "").ToString() + " Harcama: " + VeriErisim.DegerAl(drVarMi, 3, "") + " Fiş No: " + VeriErisim.DegerAl(drVarMi, 4, "") + zimmetTuru + "<br>";
                        }
                        drVarMi.Close();
                    }
                    else if (zimmet.vermeDusme == (int)ENUMZimmetVermeDusme.ZIMMETTENDUSME && sonuc.hataStr == "")
                    {
                        IDataReader drVarMi = tabZimmetForm.ZimmetVarMi(zd.prSicilNo);
                        if (drVarMi.Read())
                        {
                            int yil = VeriErisim.DegerAl(drVarMi, 1, 0);
                            string muhasebe = VeriErisim.DegerAl(drVarMi, 2, "");
                            string harcama = VeriErisim.DegerAl(drVarMi, 3, "");
                            string fisNo = VeriErisim.DegerAl(drVarMi, 4, "");
                            int islemTuru = VeriErisim.DegerAl(drVarMi, 5, 0);
                            string kimeGitti = VeriErisim.DegerAl(drVarMi, 6, "");
                            string nereyeGitti = VeriErisim.DegerAl(drVarMi, 7, "");
                            string ambar = VeriErisim.DegerAl(drVarMi, 8, "");

                            if (zimmet.belgeTur == (int)ENUMZimmetBelgeTur.ZIMMETFISI)
                            {
                                if (!(zimmet.yil >= yil &&
                                    zimmet.muhasebeKod == muhasebe &&
                                    zimmet.harcamaBirimKod.Replace(".", "") == harcama &&
                                    zimmet.ambarKod == ambar &&
                                    (string.IsNullOrEmpty(zimmet.fisNo) ||
                                    zimmet.yil > yil ||
                                    (zimmet.yil == yil && OrtakFonksiyonlar.ConvertToInt(zimmet.fisNo, 0) > OrtakFonksiyonlar.ConvertToInt(fisNo, 0))) &&
                                    islemTuru == (int)ENUMIslemTipi.ZFVERME &&
                                    zimmet.kimeGitti.Trim() == kimeGitti.Trim()))
                                    sonuc.hataStr += string.Format(TasinirMalResI.TMM310, zd.siraNo, zimmet.kimeGitti) + "<br>";
                            }
                            else
                            {
                                if (!(zimmet.yil >= yil &&
                                    zimmet.muhasebeKod == muhasebe &&
                                    zimmet.harcamaBirimKod.Replace(".", "") == harcama &&
                                    zimmet.ambarKod == ambar &&
                                    (string.IsNullOrEmpty(zimmet.fisNo) ||
                                    zimmet.yil > yil ||
                                    (zimmet.yil == yil && OrtakFonksiyonlar.ConvertToInt(zimmet.fisNo, 0) > OrtakFonksiyonlar.ConvertToInt(fisNo, 0))) &&
                                    islemTuru == (int)ENUMIslemTipi.DTLVERME &&
                                    zimmet.kimeGitti.Trim() == kimeGitti.Trim() &&
                                    zimmet.nereyeGitti.Trim() == nereyeGitti.Trim()))
                                    sonuc.hataStr += string.Format(TasinirMalResI.TMM311, zd.siraNo, zimmet.kimeGitti, zimmet.nereyeGitti) + "<br>";
                            }
                        }
                        else
                        {
                            if (zimmet.belgeTur == (int)ENUMZimmetBelgeTur.ZIMMETFISI)
                                sonuc.hataStr += string.Format(TasinirMalResI.TMM310, zd.siraNo, zimmet.kimeGitti) + "<br>";
                            else
                                sonuc.hataStr += string.Format(TasinirMalResI.TMM311, zd.siraNo, zimmet.kimeGitti, zimmet.nereyeGitti) + "<br>";
                        }

                        drVarMi.Close();
                    }
                }
            }

            if (!string.IsNullOrEmpty(sonuc.hataStr))
                sonuc.islemSonuc = false;

            veriErisim.VeriErisimKapat();
            return sonuc;
        }

        /// <summary>
        /// Parametre olarak verilen ambarın verilen yılda işlemlere kapalı olup olmadığını kontrol eden yordam
        /// </summary>
        /// <param name="zimmet">Yıl ve ambar bilgilerini tutan nesne</param>
        /// <returns>İşleme kapatılmış bir ambar ise hata mesajı, değilse boşluk döndürülür.</returns>
        private static string AmbarKapalimi(ZimmetForm zimmet)
        {
            IKYMServis servisKYM = TNS.KYM.Arac.Tanimla();

            string degiskenAd = "KAPAT" + "_" + zimmet.yil + "_" + zimmet.muhasebeKod + "_" + zimmet.harcamaBirimKod.Replace(".", "") + "_" + zimmet.ambarKod;
            string degisken = "";

            TNSCollection kDeg = servisKYM.DegiskenleriListele("stratek", degiskenAd);
            if (kDeg != null && kDeg.Count > 0)
                degisken = kDeg[0].ToString();

            if (degisken != "")
                return string.Format(TasinirMalResI.TMM251, zimmet.yil) + "<br>";
            else
                return "";
        }

        /// <summary>
        /// Zimmet fişinin durumunu güncelleyen yordam
        /// Parametre olarak verilen yeniDurum bilgisine bakarak zimmet fişine ve detay bilgilerine bir takım
        /// kontroller uygular, verilen fiş bilgileri bütün kontrolleri sorunsuz geçerse belgeye yeni durum atanır.
        /// Kullanıcı işleme yetkili değilse veya gönderilen ZimmetForm nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="zf">Zimmet fişi bilgilerini tutan nesne</param>
        /// <param name="yeniDurum">Belgeye atanması istenen yeni durum bilgisi - İptal, Onay veya OnayKaldir değerlerini alabilir.</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public static Sonuc BelgeDurumDegistir(VeriErisim veriErisim, Kullanici kullanan, ZimmetForm zf, string yeniDurum)
        {
            Sonuc sonuc = new Sonuc();
            ///////////////////////////////////////////////////////////
            bool muhasebeKdvMukellefiMi = false;
            TMMMuhasebe tmmMuhasebe = new TMMMuhasebe();
            muhasebeKdvMukellefiMi = tmmMuhasebe.KdvMukellefiMi(zf.muhasebeKod);
            ///////////////////////////////////////////////////////////

            string TCCBEntegrasyon = OrtakFonksiyonlar.WebConfigOku("TasinirTCCBEntegrasyon", "");

            zf.islemYapan = kullanan.kullaniciKodu;

            if (yeniDurum == "Onay" || yeniDurum == "OnayKaldir")
            {
                sonuc.islemSonuc = TMMYetkiKontrol.OnaylamaYetkisiVarMi(kullanan, zf.muhasebeKod, zf.harcamaBirimKod, zf.ambarKod);
                if (!sonuc.islemSonuc)
                    sonuc.islemSonuc = TMMYetkiKontrol.OnaylamaYetkisiVarMiKullaniciBirimi(kullanan, zf.muhasebeKod, zf.harcamaBirimKod, zf.ambarKod);

                if (!sonuc.islemSonuc)
                {
                    sonuc.hataStr = TasinirMalResI.TMM277;
                    return sonuc;
                }
            }
            else
            {
                sonuc.islemSonuc = TMMYetkiKontrol.YazmaYetkisiVarMi(kullanan, zf.muhasebeKod, zf.harcamaBirimKod, zf.ambarKod);
                if (!sonuc.islemSonuc)
                    sonuc.islemSonuc = TMMYetkiKontrol.YazmaYetkisiVarMiKullaniciBirimi(kullanan, zf.muhasebeKod, zf.harcamaBirimKod, zf.ambarKod);

                if (!sonuc.islemSonuc)
                {
                    sonuc.hataStr = TasinirMalResI.TMM278;
                    return sonuc;
                }
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));

            TABZimmetForm tabZimmetForm = new TABZimmetForm(ve);

            IDataReader dr = tabZimmetForm.BelgeDurumGetir(zf);

            if (!dr.Read())
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = TasinirMalResI.TMM086;
                dr.Close();
                ve.VeriErisimKapat();
                return sonuc;
            }

            int durum = VeriErisim.DegerAl(dr, 0, 0);
            dr.Close();

            if (durum == (int)ENUMBelgeDurumu.ONAYLI && yeniDurum == "İptal")
                sonuc.hataStr = string.Format(TasinirMalResI.TMM273, zf.fisNo) + "<br>";
            else if (durum == (int)ENUMBelgeDurumu.ONAYLI && yeniDurum == "Onay")
                sonuc.hataStr = string.Format(TasinirMalResI.TMM274, zf.fisNo) + "<br>";
            else if (durum != (int)ENUMBelgeDurumu.ONAYLI && yeniDurum == "OnayKaldir")
                sonuc.hataStr = string.Format(TasinirMalResI.TMM275, zf.fisNo) + "<br>";

            if (sonuc.hataStr != "")
            {
                sonuc.islemSonuc = false;
                ve.VeriErisimKapat();
                return sonuc;
            }

            ObjectArray listeFis = FisAc(ve, kullanan, zf);

            if (listeFis.ObjeSayisi < 1)
            {
                sonuc.hataStr = TasinirMalResI.TMM276;
                sonuc.islemSonuc = false;
                ve.VeriErisimKapat();
                return sonuc;
            }

            zf = (ZimmetForm)listeFis[0];
            if (yeniDurum == "Onay")
            {
                ObjectArray zimmetDetay = ListeleDetay(ve, kullanan, zf);

                sonuc = Kontrol(ve, kullanan, zf, zimmetDetay);

                if (!sonuc.islemSonuc)
                {
                    ve.VeriErisimKapat();
                    return sonuc;
                }
            }
            else if (yeniDurum == "OnayKaldir")
            {
                sonuc.hataStr = AmbarKapalimi(zf);
                if (sonuc.hataStr != "")
                {
                    sonuc.islemSonuc = false;
                    ve.VeriErisimKapat();
                    return sonuc;
                }
            }

            ve.BeginTransaction();

            zf.islemYapan = kullanan.kullaniciKodu;
            sonuc = tabZimmetForm.BelgeDurumDegistir(zf, yeniDurum);

            if (sonuc.islemSonuc)
            {
                TABSicilHareket tabSicilHareket = new TABSicilHareket(ve);

                zf.ipAdresi = kullanan.telefon;
                tabZimmetForm.KaydetTarihce(zf.yil, zf.muhasebeKod, zf.harcamaBirimKod, zf.fisNo, zf.belgeTur, kullanan.kullaniciKodu, ZimmetForm.FormDurumAciklama(TABGenel.FormDurumNoVer(yeniDurum)), zf.ipAdresi);

                if (yeniDurum == "Onay")
                {
                    ObjectArray detay = ListeleDetay(ve, kullanan, zf);

                    foreach (ZimmetDetay zd in detay.objeler)
                    {
                        var fiyatListe = TMMSicilNoHareket.FiyatGetir(ve, kullanan, zd.prSicilNo);
                        if (!TNS.TMM.Arac.MerkezBankasiKullaniyor())
                        {
                            fiyatListe["KDVLIBIRIMFIYAT"] = zd.birimFiyat;
                            fiyatListe["DEGERARTISI"] = 0;
                        }

                        SicilNoHareket sicilNoHareket = new SicilNoHareket();

                        sicilNoHareket.yil = zf.yil;
                        sicilNoHareket.muhasebeKod = zf.muhasebeKod;
                        sicilNoHareket.harcamaBirimKod = zf.harcamaBirimKod;
                        sicilNoHareket.fisNo = zf.fisNo;
                        sicilNoHareket.ambarKod = zf.ambarKod;
                        sicilNoHareket.prSicilNo = zd.prSicilNo;
                        sicilNoHareket.fisSiraNo = zd.siraNo;
                        sicilNoHareket.islemTarih = new TNSDateTime(DateTime.Now);
                        sicilNoHareket.islemYapan = kullanan.kullaniciKodu;
                        sicilNoHareket.zimmetOzellik = zd.ozellik;
                        sicilNoHareket.kimeGitti = zf.kimeGitti;
                        sicilNoHareket.nereyeGitti = zf.nereyeGitti;
                        sicilNoHareket.fisTarih = new TNSDateTime(zf.fisTarih.ToString());
                        sicilNoHareket.hesapPlanKod = zd.hesapPlanKod;
                        sicilNoHareket.hesapPlanAd = zd.hesapPlanAd;
                        sicilNoHareket.sicilNo = zd.gorSicilNo;
                        sicilNoHareket.fiyat = fiyatListe["KDVLIBIRIMFIYAT"];
                        sicilNoHareket.degerArtisi = fiyatListe["DEGERARTISI"];
                        sicilNoHareket.kdvOran = zd.kdvOran;


                        if (zf.belgeTur == (int)ENUMZimmetBelgeTur.ZIMMETFISI)
                            sicilNoHareket.islemTurKod = zf.vermeDusme == (int)ENUMZimmetVermeDusme.ZIMMETVERME ? (int)ENUMIslemTipi.ZFVERME : (int)ENUMIslemTipi.ZFDUSME;
                        else if (zf.belgeTur == (int)ENUMZimmetBelgeTur.DAYANIKLITL)
                            sicilNoHareket.islemTurKod = zf.vermeDusme == (int)ENUMZimmetVermeDusme.ZIMMETVERME ? (int)ENUMIslemTipi.DTLVERME : (int)ENUMIslemTipi.DTLDUSME;
                        else
                        {
                            sonuc.islemSonuc = false;
                            sonuc.hataStr = TasinirMalResI.TMM312;
                            ve.Rollback();
                            ve.VeriErisimKapat();
                            return sonuc;
                        }

                        sicilNoHareket.islemTipKod = sicilNoHareket.islemTurKod == (int)ENUMIslemTipi.ZFVERME ? (int)ENUMZimmetIslemTipi.ZFVERME : sicilNoHareket.islemTurKod == (int)ENUMIslemTipi.ZFDUSME ? (int)ENUMZimmetIslemTipi.ZFDUSME : sicilNoHareket.islemTurKod == (int)ENUMIslemTipi.DTLVERME ? (int)ENUMZimmetIslemTipi.DTLVERME : sicilNoHareket.islemTurKod == (int)ENUMIslemTipi.DTLDUSME ? (int)ENUMZimmetIslemTipi.DTLDUSME : 0;

                        sicilNoHareket.durum = (int)ENUMSicilDurum.AKTIF;
                        sonuc = tabSicilHareket.Kaydet(sicilNoHareket, muhasebeKdvMukellefiMi);

                        if (sonuc.islemSonuc && Arac.MerkezBankasiKullaniyor())
                        {
                            ObjectArray ozellikler = TMMSicilNoOzellik.Listele(ve, kullanan, sicilNoHareket.prSicilNo);
                            if (ozellikler.sonuc.islemSonuc && ozellikler.objeler.Count > 0)
                            {
                                SicilNoOzellik ozellik = (SicilNoOzellik)ozellikler.objeler[0];

                                if (sicilNoHareket.islemTurKod == (int)ENUMIslemTipi.ZFDUSME || sicilNoHareket.islemTurKod == (int)ENUMIslemTipi.DTLDUSME)
                                    ozellik.bulunduguYer = null;
                                else
                                    ozellik.bulunduguYer = (zf.nereyeGitti == "" ? null : zf.nereyeGitti);

                                ozellikler = new ObjectArray();
                                ozellikler.Ekle(ozellik);

                                sonuc = new TMMSicilNoOzellik().Kaydet(kullanan, sicilNoHareket.yil, sicilNoHareket.muhasebeKod, sicilNoHareket.harcamaBirimKod, sicilNoHareket.ambarKod, ozellikler);
                            }
                        }

                        if (sonuc.islemSonuc && !string.IsNullOrWhiteSpace(TCCBEntegrasyon))
                        {
                            try
                            {
                                ObjectArray zListe = new ObjectArray();
                                zListe.objeler.Add(sicilNoHareket);
                                TMMTCCBEntegrasyon.Kaydet255(kullanan, zListe, "zimmet");
                            }
                            catch (Exception ex)
                            {
                                OrtakFonksiyonlar.HataDosyaYaz("TCCBEntegrasyonHataCatch.txt", "zimmet>" + DateTime.Now.ToString("yyyyMMdd HH:mm:ss.fff") + ">" + zd.yil + ">" + zd.muhasebeKod + ">" + zd.fisNo + ">" + zd.gorSicilNo + ">>> ex.Message:" + ex.Message, false);
                            }
                        }


                        if (!sonuc.islemSonuc)
                        {
                            sonuc.hataStr = TasinirMalResI.TMM313 + sonuc.hataStr;
                            ve.Rollback();
                            ve.VeriErisimKapat();
                            return sonuc;
                        }
                    }
                }
                else if (yeniDurum == "OnayKaldir")
                {
                    SicilNoHareket sicilNoHareket = new SicilNoHareket();

                    sicilNoHareket.yil = zf.yil;
                    sicilNoHareket.muhasebeKod = zf.muhasebeKod;
                    sicilNoHareket.harcamaBirimKod = zf.harcamaBirimKod;
                    sicilNoHareket.fisNo = zf.fisNo;

                    if (zf.belgeTur == (int)ENUMZimmetBelgeTur.ZIMMETFISI)
                        sicilNoHareket.islemTurKod = zf.vermeDusme == (int)ENUMZimmetVermeDusme.ZIMMETVERME ? (int)ENUMIslemTipi.ZFVERME : (int)ENUMIslemTipi.ZFDUSME;
                    else if (zf.belgeTur == (int)ENUMZimmetBelgeTur.DAYANIKLITL)
                        sicilNoHareket.islemTurKod = zf.vermeDusme == (int)ENUMZimmetVermeDusme.ZIMMETVERME ? (int)ENUMIslemTipi.DTLVERME : (int)ENUMIslemTipi.DTLDUSME;
                    else
                    {
                        sonuc.islemSonuc = false;
                        sonuc.hataStr = TasinirMalResI.TMM312;
                        ve.Rollback();
                        ve.VeriErisimKapat();
                        return sonuc;
                    }

                    sicilNoHareket.islemTipKod = sicilNoHareket.islemTurKod == (int)ENUMIslemTipi.ZFVERME ? (int)ENUMZimmetIslemTipi.ZFVERME : sicilNoHareket.islemTurKod == (int)ENUMIslemTipi.ZFDUSME ? (int)ENUMZimmetIslemTipi.ZFDUSME : sicilNoHareket.islemTurKod == (int)ENUMIslemTipi.DTLVERME ? (int)ENUMZimmetIslemTipi.DTLVERME : sicilNoHareket.islemTurKod == (int)ENUMIslemTipi.DTLDUSME ? (int)ENUMZimmetIslemTipi.DTLDUSME : 0;
                    string sonrasindakiHareket = string.Empty;
                    ObjectArray sicilIptalZimmetArray = new ObjectArray();

                    if (sicilNoHareket.islemTipKod == 0)
                        sonuc.islemSonuc = false;
                    else
                    {
                        IDataReader drZimmet = tabSicilHareket.HareketPrSicilSiraDondur(sicilNoHareket);
                        while (drZimmet.Read())
                        {
                            SicilNoHareket sHareket = new SicilNoHareket();

                            sHareket.yil = sicilNoHareket.yil;
                            sHareket.muhasebeKod = sicilNoHareket.muhasebeKod;
                            sHareket.harcamaBirimKod = sicilNoHareket.harcamaBirimKod;
                            sHareket.fisNo = sicilNoHareket.fisNo;
                            sHareket.islemTipKod = sicilNoHareket.islemTipKod;
                            sHareket.islemTurKod = sicilNoHareket.islemTurKod;

                            sHareket.prSicilNo = VeriErisim.DegerAl(drZimmet, 0, 0);
                            sHareket.siraNo = VeriErisim.DegerAl(drZimmet, 1, 0);

                            sicilIptalZimmetArray.Ekle(sHareket);
                        }

                        drZimmet.Close();

                        foreach (SicilNoHareket sicilNoHareketZimmet in sicilIptalZimmetArray.objeler)
                        {
                            drZimmet = tabSicilHareket.SonrasindaHareketVarMiZimmet(sicilNoHareketZimmet);

                            while (drZimmet.Read())
                            {
                                int islemTipKod = VeriErisim.DegerAl(drZimmet, 4, 0);
                                string fisTuru = string.Empty;
                                if (islemTipKod == (int)ENUMZimmetIslemTipi.ZFVERME || islemTipKod == (int)ENUMZimmetIslemTipi.ZFDUSME || islemTipKod == (int)ENUMZimmetIslemTipi.DTLVERME || islemTipKod == (int)ENUMZimmetIslemTipi.DTLDUSME)
                                    fisTuru = TasinirMalResI.TMM315;
                                else
                                    fisTuru = TasinirMalResI.TMM316;

                                sonrasindakiHareket += string.Format(TasinirMalResI.TMM314, VeriErisim.DegerAl(drZimmet, 0, 0), VeriErisim.DegerAl(drZimmet, 1, ""), VeriErisim.DegerAl(drZimmet, 2, ""), VeriErisim.DegerAl(drZimmet, 3, ""), fisTuru) + "<br>";
                            }

                            drZimmet.Close();
                        }

                        if (!string.IsNullOrEmpty(sonrasindakiHareket))
                        {
                            sonuc.islemSonuc = false;
                        }
                        else
                            sonuc = tabSicilHareket.IptalEt(sicilNoHareket);
                    }
                    if (!sonuc.islemSonuc)
                    {
                        sonuc.hataStr = string.Format(TasinirMalResI.TMM317, sonrasindakiHareket);
                        ve.Rollback();
                        ve.VeriErisimKapat();
                        return sonuc;
                    }
                }

                sonuc.bilgiStr = string.Format(TasinirMalResI.TMM318, zf.fisNo) + "<br>";
                ve.Commit();
            }
            else
            {
                sonuc.hataStr = string.Format(TasinirMalResI.TMM319, zf.fisNo) + "<br>";
                ve.Rollback();
            }

            ve.VeriErisimKapat();
            return sonuc;
        }

        public static Sonuc ZimmetDusVer(VeriErisim veriErisim, Kullanici kullanan, ZimmetForm zimmet, TNSCollection zimmetDetay, string verilecekPersonelKod, string verilecekOdaKod, ENUMZimmetBelgeTur verilecekBelgeTur)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));

            SicilNoHareket snhKriter = new SicilNoHareket();
            snhKriter.muhasebeKod = zimmet.muhasebeKod;
            snhKriter.harcamaBirimKod = zimmet.harcamaBirimKod;
            snhKriter.ambarKod = zimmet.ambarKod;
            snhKriter.kimeGitti = zimmet.kimeGitti;
            snhKriter.nereyeGitti = zimmet.nereyeGitti;
            if (zimmet.belgeTur == (int)ENUMZimmetBelgeTur.ZIMMETFISI)
                snhKriter.islemTurKod = (int)ENUMIslemTipi.ZFVERME;
            else if (zimmet.belgeTur == (int)ENUMZimmetBelgeTur.DAYANIKLITL)
                snhKriter.islemTurKod = (int)ENUMIslemTipi.DTLVERME;

            ObjectArray zimmetliDemirbaslar = TMMSicilNoHareket.ZimmetliSicilNolar(ve, kullanan, snhKriter);
            if (!zimmetliDemirbaslar.sonuc.islemSonuc)
            {
                ve.VeriErisimKapat();
                return zimmetliDemirbaslar.sonuc;
            }
            if (zimmetliDemirbaslar.objeler.Count <= 0)
            {
                ve.VeriErisimKapat();
                zimmetliDemirbaslar.sonuc.islemSonuc = false;
                zimmetliDemirbaslar.sonuc.hataStr = string.Format(TasinirMalResI.TMM396, zimmet.kimeGitti);
                return zimmetliDemirbaslar.sonuc;
            }

            int siraNo = 0;
            foreach (ZimmetDetay zd in zimmetDetay)
            {
                bool bulundu = false;
                foreach (SicilNoHareket sh in zimmetliDemirbaslar.objeler)
                {
                    if (zd.prSicilNo == sh.prSicilNo)
                    {
                        zd.yil = zimmet.yil;
                        zd.muhasebeKod = zimmet.muhasebeKod;
                        zd.harcamaBirimKod = zimmet.harcamaBirimKod;
                        zd.belgeTur = zimmet.belgeTur;

                        zd.hesapPlanKod = sh.hesapPlanKod;
                        zd.gorSicilNo = sh.sicilNo;
                        zd.siraNo = ++siraNo;
                        zd.kdvOran = sh.kdvOran;
                        zd.birimFiyat = sh.fiyat;

                        bulundu = true;
                        break;
                    }
                }
                if (!bulundu)
                {
                    ve.VeriErisimKapat();
                    zimmetliDemirbaslar.sonuc.islemSonuc = false;
                    zimmetliDemirbaslar.sonuc.hataStr = string.Format(TasinirMalResI.TMM397, zd.prSicilNo, zimmet.kimeGitti);
                    return zimmetliDemirbaslar.sonuc;
                }
            }

            zimmet.tip = (int)ENUMZimmetTipi.DEMIRBASCIHAZ;
            zimmet.vermeDusme = (int)ENUMZimmetVermeDusme.ZIMMETTENDUSME;
            zimmet.dusmeTarih.Yaz(zimmet.fisTarih.Oku());
            zimmet.islemTarih.Yaz(DateTime.Now);
            zimmet.islemYapan = kullanan.kullaniciKodu;

            ve.BeginTransaction();

            string belgeNolar = string.Empty;
            ObjectArray zimmetDetayArr = new ObjectArray();
            zimmetDetayArr.objeler.AddRange(zimmetDetay);
            Sonuc sonuc = Kaydet(ve, kullanan, zimmet, zimmetDetayArr);
            if (sonuc.islemSonuc)
            {
                zimmet.fisNo = sonuc.anahtar;
                belgeNolar = TasinirMalResI.TMM398 + zimmet.fisNo;
                sonuc = BelgeDurumDegistir(ve, kullanan, zimmet, "Onay");
            }

            if (sonuc.islemSonuc)
            {
                //Malzeme, zimmet düşmeden ambara gelsin diye commit etmek zorundayım!
                //Commit etmezsem, başka connection açıp uzaydan bakıp bulamıyor malzemeyi.
                ve.Commit();

                zimmet.fisNo = string.Empty;
                zimmet.vermeDusme = (int)ENUMZimmetVermeDusme.ZIMMETVERME;
                zimmet.belgeTur = (int)verilecekBelgeTur;
                zimmet.kimeGitti = verilecekPersonelKod;
                zimmet.nereyeGitti = verilecekOdaKod;
                zimmet.dusmeTarih = null;

                foreach (ZimmetDetay zd in zimmetDetay)
                {
                    zd.fisNo = string.Empty;
                    zd.belgeTur = (int)verilecekBelgeTur;
                }

                sonuc = Kaydet(ve, kullanan, zimmet, zimmetDetayArr);
            }

            if (sonuc.islemSonuc)
            {
                zimmet.fisNo = sonuc.anahtar;
                belgeNolar += "; " + TasinirMalResI.TMM399 + zimmet.fisNo;
                sonuc = BelgeDurumDegistir(ve, kullanan, zimmet, "Onay");
            }

            if (sonuc.islemSonuc)
            {
                ve.Commit();
                sonuc.bilgiStr = TasinirMalResI.TMM322 + "<br />" + belgeNolar;
            }
            else
            {
                ve.Rollback();
                sonuc.hataStr = TasinirMalResI.TMM323 + "<br />" + sonuc.hataStr;
            }

            ve.VeriErisimKapat();
            return sonuc;
        }


        public static ZimmetForm ZimmetVarMi(VeriErisim veriErisim, Kullanici kullanan, string gorSicilNo)
        {
            ZimmetForm zimmetForm = new ZimmetForm();

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABZimmetForm tabZimmetForm = new TABZimmetForm(ve);

            IDataReader dr = tabZimmetForm.ZimmetVarMi(0, gorSicilNo);

            while (dr.Read())
            {
                int prSicilNo = VeriErisim.DegerAl(dr, 0, 0);
                int yil = VeriErisim.DegerAl(dr, 1, 0);
                string muhasebe = VeriErisim.DegerAl(dr, 2, "");
                string harcama = VeriErisim.DegerAl(dr, 3, "");
                string fisNo = VeriErisim.DegerAl(dr, 4, "");
                int islemTuru = VeriErisim.DegerAl(dr, 5, 0);
                string kimeGitti = VeriErisim.DegerAl(dr, 6, "");
                string nereyeGitti = VeriErisim.DegerAl(dr, 7, "");
                string ambar = VeriErisim.DegerAl(dr, 8, "");
                string hesapKod = VeriErisim.DegerAl(dr, 9, "");
                int kdvoran = VeriErisim.DegerAl(dr, 10, 0);
                string ozellik = VeriErisim.DegerAl(dr, 11, "");
                decimal fiyat = VeriErisim.DegerAl(dr, 12, (decimal)0.0);

                zimmetForm.yil = yil;
                zimmetForm.muhasebeKod = muhasebe;
                zimmetForm.harcamaBirimKod = harcama;
                zimmetForm.ambarKod = ambar;
                zimmetForm.fisNo = fisNo;
                zimmetForm.kimeGitti = kimeGitti;
                zimmetForm.nereyeGitti = nereyeGitti;

                if (gorSicilNo.StartsWith("254"))
                    zimmetForm.tip = 2;
                else
                    zimmetForm.tip = 1;

                if (islemTuru == (int)ENUMIslemTipi.ZFVERME)
                    zimmetForm.belgeTur = (int)ENUMZimmetBelgeTur.ZIMMETFISI;
                else if (islemTuru == (int)ENUMIslemTipi.DTLVERME)
                    zimmetForm.belgeTur = (int)ENUMZimmetBelgeTur.DAYANIKLITL;

                ZimmetDetay zimmetDetay = new ZimmetDetay();
                zimmetDetay.prSicilNo = prSicilNo;
                zimmetDetay.gorSicilNo = gorSicilNo;

                zimmetDetay.yil = zimmetForm.yil;
                zimmetDetay.muhasebeKod = zimmetForm.muhasebeKod;
                zimmetDetay.harcamaBirimKod = zimmetForm.harcamaBirimKod;
                zimmetDetay.belgeTur = zimmetForm.belgeTur;

                zimmetDetay.hesapPlanKod = hesapKod;
                zimmetDetay.kdvOran = kdvoran;
                zimmetDetay.ozellik = ozellik;
                zimmetDetay.siraNo = 1;
                zimmetDetay.birimFiyat = fiyat;

                zimmetForm.sorguDetay.Ekle(zimmetDetay);
            }

            dr.Close();
            ve.VeriErisimKapat();

            return zimmetForm;
        }
    }
}