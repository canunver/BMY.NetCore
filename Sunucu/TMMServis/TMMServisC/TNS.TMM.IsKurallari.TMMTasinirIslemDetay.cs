using System;
using System.Data;
using System.Diagnostics;
using TNS.KYM;
using TNS.TMM.IsKurallari.Veri;
using TNS.Veri;

namespace TNS.TMM.IsKurallari
{
    /// <summary>
    /// Taşınır işlem fişinin detay bilgileriyle ilgili, iş kuralları kontrollerini yapıp,
    /// veri tabanı bağlantısını alıp, veri tabanı fonksiyonlarını çağırarak
    /// okuma, yazma işlemlerini gerçekleştiren fonksiyonları tutan sınıf
    /// </summary>
    public class TMMTasinirIslemDetay
    {
        /// <summary>
        /// Taşınır malzemelerinin alınabileceği en yüksek tutarı tutan değişken
        /// </summary>
        private static decimal enYuksekTutar = OrtakFonksiyonlar.ConvertToDecimal(System.Configuration.ConfigurationManager.AppSettings.Get("TasinirEnYuksekAlimTutari") + "", 0);

        /// <summary>
        /// Taşınır işlem fişinin detay bilgilerini kaydeden yordam
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="fisNo">Taşınır işlem fişinin belge numarası</param>
        /// <param name="td">Taşınır işlem fişinin detay bilgileri listesini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public static Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, string fisNo, TNSCollection td)
        {
            Sonuc sonuc = new Sonuc();
            ///////////////////////////////////////////////////////////
            bool muhasebeKdvMukellefiMi = false;
            TMMMuhasebe tmmMuhasebe = new TMMMuhasebe();
            muhasebeKdvMukellefiMi = tmmMuhasebe.KdvMukellefiMi(((TasinirIslemDetay)td[0]).muhasebeKod);
            ///////////////////////////////////////////////////////////

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABTasinirIslemDetay tabTasinirIslemDetay = new TABTasinirIslemDetay(ve);
            ve.BeginTransaction();

            foreach (TasinirIslemDetay tDetay in td)
            {
                tDetay.fisNo = fisNo;
                //İstemciden hatalı gelebilir. Burada hesaplanıyor. KDV mükellefi ise kdvlibirimfiyat=birimfiyat
                if (muhasebeKdvMukellefiMi)
                    tDetay.birimFiyatKDVLi = tDetay.birimFiyat;
                else
                    tDetay.birimFiyatKDVLi = (1 + (OrtakFonksiyonlar.ConvertToDecimal(tDetay.kdvOran) / 100)) * tDetay.birimFiyat;

                sonuc = tabTasinirIslemDetay.Kaydet(ve, tDetay);

                if (sonuc.islemSonuc)
                {
                    if (tDetay.ozellik.amortismanYil == 0)
                    {
                        ObjectArray hesapPlanListe = new TMMHesapPlani().Listele(kullanan, new HesapPlaniSatir() { hesapKod = tDetay.hesapPlanKod }, new Sayfalama());
                        foreach (HesapPlaniSatir h in hesapPlanListe.objeler)
                            tDetay.ozellik.amortismanYil = h.amortiYil;
                    }

                    sonuc = tabTasinirIslemDetay.OzellikKaydet(ve, tDetay);
                }
                else
                {
                    sonuc.hataStr = TasinirMalResI.TMM200 + "" + sonuc.hataStr;
                    break;
                }
            }

            if (sonuc.islemSonuc)
                ve.Commit();
            else
                ve.Rollback();

            ve.VeriErisimKapat();
            return sonuc;
        }

        /// <summary>
        /// Parametre olarak verilen taşınır işlem fişinin detay bilgilerini silen yordam
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="tf">Taşınır işlem fişi bilgilerini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public static Sonuc Sil(VeriErisim veriErisim, Kullanici kullanan, TasinirIslemForm tf)
        {
            Sonuc sonuc = new Sonuc();

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABTasinirIslemDetay tabTasinirIslemDetay = new TABTasinirIslemDetay(ve);
            ve.BeginTransaction();

            sonuc = tabTasinirIslemDetay.Sil(tf);

            if (sonuc.islemSonuc)
            {
                ve.Commit();
                sonuc.bilgiStr = TasinirMalResI.TMM162 + "";
            }
            else
            {
                ve.Rollback();
                sonuc.hataStr = "Silme başarısız!";
            }

            ve.VeriErisimKapat();
            return sonuc;
        }

        /// <summary>
        /// Parametre olarak verilen taşınır işlem fişinin detay bilgilerini döndüren yordam
        /// </summary>
        /// <param name="tf">Taşınır işlem fişi bilgilerini tutan nesne</param>
        /// <param name="siraNoSirali">Taşınır işlem fişinin detay bilgilerinin satır sırasına göre sıralı olup olmaması bilgisi</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında taşınır işlem fişine ait detay bilgileri listesi döndürülür.</returns>
        public static ObjectArray Listele(TasinirIslemForm tf, bool siraNoSirali, int sayfaNo, int kayitSayisi)
        {
            string kodYapisi = TNS.TMM.Arac.DegiskenDegerBul(0, "/KODYAPI/KOD13/KODYAPI");

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(null));
            TABTasinirIslemDetay tabTasinirIslemDetay = new TABTasinirIslemDetay(ve);

            int islemTur = (int)TMMServis.UzaySayiDegerGetir("TASISLEMTIPTUR", tf.islemTipKod.ToString(), true, "");

            ObjectArray bilgi = new ObjectArray();
            tf.fisNo = tf.fisNo.PadLeft(6, '0');


            DateTime dt = DateTime.Now;
            IDataReader dr = tabTasinirIslemDetay.Listele(tf, siraNoSirali, sayfaNo, kayitSayisi);

            TimeSpan ts = DateTime.Now - dt;
            OrtakFonksiyonlar.HataDosyaYaz("TasinirDetaySorgu.txt", DateTime.Now.ToString() + " - Veri tabanı sorgulama " + ts.TotalMilliseconds + " ms sürdü", false);


            if (dr != null)
            {
                int sonSira = (int)TABTasinirIslemDetay.ENUMAlanlar.SATISBEDELI + 1;
                int sonSiraOzellik = (int)TABTasinirIslemDetay.ENUMAlanlarOzellik.BULUNDUGUYER;

                while (dr.Read())
                {
                    TasinirIslemDetay tDetay = new TasinirIslemDetay();

                    tDetay.ambarKod = tf.ambarKod;

                    tDetay.yil = VeriErisim.DegerAl(dr, (int)TABTasinirIslemDetay.ENUMAlanlar.YIL, 0);
                    tDetay.muhasebeKod = VeriErisim.DegerAl(dr, (int)TABTasinirIslemDetay.ENUMAlanlar.MUHASEBEKOD, string.Empty);
                    tDetay.harcamaKod = VeriErisim.DegerAl(dr, (int)TABTasinirIslemDetay.ENUMAlanlar.HARCAMABIRIMKOD, string.Empty);
                    tDetay.fisNo = VeriErisim.DegerAl(dr, (int)TABTasinirIslemDetay.ENUMAlanlar.FISNO, string.Empty);
                    tDetay.siraNo = VeriErisim.DegerAl(dr, (int)TABTasinirIslemDetay.ENUMAlanlar.SIRANO, 0);
                    tDetay.hesapPlanKod = OrtakFonksiyonlar.KodNoktaKoy(kodYapisi, VeriErisim.DegerAl(dr, (int)TABTasinirIslemDetay.ENUMAlanlar.HESAPPLANKOD, string.Empty));
                    tDetay.sicilNo = VeriErisim.DegerAl(dr, (int)TABTasinirIslemDetay.ENUMAlanlar.SICILNO, 0);
                    tDetay.miktar = VeriErisim.DegerAl(dr, (int)TABTasinirIslemDetay.ENUMAlanlar.MIKTAR, (decimal)0);
                    tDetay.kdvOran = VeriErisim.DegerAl(dr, (int)TABTasinirIslemDetay.ENUMAlanlar.KDVORAN, 0);
                    tDetay.birimFiyat = VeriErisim.DegerAl(dr, (int)TABTasinirIslemDetay.ENUMAlanlar.BIRIMFIYAT, (decimal)0);
                    tDetay.birimFiyatKDVLi = VeriErisim.DegerAl(dr, (int)TABTasinirIslemDetay.ENUMAlanlar.BIRIMFIYATKDVLI, (decimal)0);
                    tDetay.eSicilNo = VeriErisim.DegerAl(dr, (int)TABTasinirIslemDetay.ENUMAlanlar.ESICILNO, string.Empty);
                    tDetay.eAlimTarihi = VeriErisim.DegerAl(dr, (int)TABTasinirIslemDetay.ENUMAlanlar.EALIMTARIHI);
                    tDetay.eTedarikSekli = VeriErisim.DegerAl(dr, (int)TABTasinirIslemDetay.ENUMAlanlar.ETEDARIKSEKLI, string.Empty);
                    tDetay.eAciklama = VeriErisim.DegerAl(dr, (int)TABTasinirIslemDetay.ENUMAlanlar.EACIKLAMA, string.Empty);
                    tDetay.sonTarih = VeriErisim.DegerAl(dr, (int)TABTasinirIslemDetay.ENUMAlanlar.SONTARIH);

                    tDetay.yerleskeYeri = VeriErisim.DegerAl(dr, (int)TABTasinirIslemDetay.ENUMAlanlar.YERLESKEYERI, "");
                    tDetay.satisBedeli = VeriErisim.DegerAl(dr, (int)TABTasinirIslemDetay.ENUMAlanlar.SATISBEDELI, (decimal)0);

                    if ((tDetay.hesapPlanKod.StartsWith("2") || TNS.TMM.Arac.MerkezBankasiKullaniyor()) &&
                      !(islemTur == (int)ENUMIslemTipi.SATINALMAGIRIS ||
                        islemTur == (int)ENUMIslemTipi.BAGISGIRIS ||
                        islemTur == (int)ENUMIslemTipi.SAYIMFAZLASIGIRIS ||
                        islemTur == (int)ENUMIslemTipi.IADEGIRIS ||
                        islemTur == (int)ENUMIslemTipi.URETILENGIRIS ||
                        islemTur == (int)ENUMIslemTipi.ACILIS ||
                        islemTur == (int)ENUMIslemTipi.TUKETIMCIKIS ||
                        islemTur == (int)ENUMIslemTipi.DEVIRGIRISKURUM))
                    {
                        if (tDetay.eSicilNo != "" && tDetay.sicilNo <= 0)
                            tDetay.gorSicilNo = TMM.IsKurallari.Genel.GorSicilNoGetir(tDetay.muhasebeKod, tDetay.harcamaKod.Replace(".", ""), OrtakFonksiyonlar.ConvertToInt(tDetay.eSicilNo, 0));
                        else
                            tDetay.gorSicilNo = TMM.IsKurallari.Genel.GorSicilNoGetir(tDetay.muhasebeKod, tDetay.harcamaKod.Replace(".", ""), tDetay.sicilNo);

                        //Merkez bankası devir giriş otomatik yeni kayıt olarak yapılıyor. Onay işleminden önce gönderilen birimden sorgulanması gerekiyor. HÖ
                        if (tDetay.gorSicilNo == "" && TNS.TMM.Arac.MerkezBankasiKullaniyor() && islemTur == (int)ENUMIslemTipi.DEVIRGIRIS)
                            tDetay.gorSicilNo = TMM.IsKurallari.Genel.GorSicilNoGetir(tDetay.muhasebeKod, tf.gHarcamaKod.Replace(".", ""), tDetay.sicilNo);
                    }

                    tDetay.ozellik.markaKod = VeriErisim.DegerAl(dr, sonSira + (int)TABTasinirIslemDetay.ENUMAlanlarOzellik.MARKAKOD, 0);
                    tDetay.ozellik.modelKod = VeriErisim.DegerAl(dr, sonSira + (int)TABTasinirIslemDetay.ENUMAlanlarOzellik.MODELKOD, 0);
                    tDetay.ozellik.saseNo = VeriErisim.DegerAl(dr, sonSira + (int)TABTasinirIslemDetay.ENUMAlanlarOzellik.SASENO, string.Empty);
                    tDetay.ozellik.motorNo = VeriErisim.DegerAl(dr, sonSira + (int)TABTasinirIslemDetay.ENUMAlanlarOzellik.MOTORNO, string.Empty);
                    tDetay.ozellik.plaka = VeriErisim.DegerAl(dr, sonSira + (int)TABTasinirIslemDetay.ENUMAlanlarOzellik.PLAKA, string.Empty);
                    tDetay.ozellik.ciltNo = VeriErisim.DegerAl(dr, sonSira + (int)TABTasinirIslemDetay.ENUMAlanlarOzellik.CILTNO, string.Empty);
                    tDetay.ozellik.dil = VeriErisim.DegerAl(dr, sonSira + (int)TABTasinirIslemDetay.ENUMAlanlarOzellik.DIL, string.Empty);
                    tDetay.ozellik.yazarAdi = VeriErisim.DegerAl(dr, sonSira + (int)TABTasinirIslemDetay.ENUMAlanlarOzellik.YAZARADI, string.Empty);
                    tDetay.ozellik.adi = VeriErisim.DegerAl(dr, sonSira + (int)TABTasinirIslemDetay.ENUMAlanlarOzellik.ADI, string.Empty);
                    tDetay.ozellik.yayinYeri = VeriErisim.DegerAl(dr, sonSira + (int)TABTasinirIslemDetay.ENUMAlanlarOzellik.YAYINYERI, string.Empty);
                    tDetay.ozellik.yayinTarihi = VeriErisim.DegerAl(dr, sonSira + (int)TABTasinirIslemDetay.ENUMAlanlarOzellik.YAYINTARIHI, string.Empty);
                    tDetay.ozellik.neredenGeldi = VeriErisim.DegerAl(dr, sonSira + (int)TABTasinirIslemDetay.ENUMAlanlarOzellik.NEREDENGELDI, string.Empty);
                    tDetay.ozellik.boyutlari = VeriErisim.DegerAl(dr, sonSira + (int)TABTasinirIslemDetay.ENUMAlanlarOzellik.BOYUTLARI, string.Empty);
                    tDetay.ozellik.satirSayisi = VeriErisim.DegerAl(dr, sonSira + (int)TABTasinirIslemDetay.ENUMAlanlarOzellik.SATIRSAYISI, string.Empty);
                    tDetay.ozellik.yaprakSayisi = VeriErisim.DegerAl(dr, sonSira + (int)TABTasinirIslemDetay.ENUMAlanlarOzellik.YAPRAKSAYISI, string.Empty);
                    tDetay.ozellik.sayfaSayisi = VeriErisim.DegerAl(dr, sonSira + (int)TABTasinirIslemDetay.ENUMAlanlarOzellik.SAYFASAYISI, string.Empty);
                    tDetay.ozellik.ciltTuru = VeriErisim.DegerAl(dr, sonSira + (int)TABTasinirIslemDetay.ENUMAlanlarOzellik.CILTNO, string.Empty);
                    tDetay.ozellik.cesidi = VeriErisim.DegerAl(dr, sonSira + (int)TABTasinirIslemDetay.ENUMAlanlarOzellik.CESIDI, string.Empty);
                    tDetay.ozellik.yeriKonusu = VeriErisim.DegerAl(dr, sonSira + (int)TABTasinirIslemDetay.ENUMAlanlarOzellik.YERIKONUSU, string.Empty);
                    tDetay.ozellik.gelisTarihi = VeriErisim.DegerAl(dr, sonSira + (int)TABTasinirIslemDetay.ENUMAlanlarOzellik.GELISTARIHI, string.Empty);
                    tDetay.ozellik.neredeBulundu = VeriErisim.DegerAl(dr, sonSira + (int)TABTasinirIslemDetay.ENUMAlanlarOzellik.NEREDEBULUNDU, string.Empty);
                    tDetay.ozellik.cagi = VeriErisim.DegerAl(dr, sonSira + (int)TABTasinirIslemDetay.ENUMAlanlarOzellik.CAGI, string.Empty);
                    tDetay.ozellik.durumuMaddesi = VeriErisim.DegerAl(dr, sonSira + (int)TABTasinirIslemDetay.ENUMAlanlarOzellik.DURUMUMADDESI, string.Empty);
                    tDetay.ozellik.onYuz = VeriErisim.DegerAl(dr, sonSira + (int)TABTasinirIslemDetay.ENUMAlanlarOzellik.ONYUZ, string.Empty);
                    tDetay.ozellik.arkaYuz = VeriErisim.DegerAl(dr, sonSira + (int)TABTasinirIslemDetay.ENUMAlanlarOzellik.ARKAYUZ, string.Empty);
                    tDetay.ozellik.fotograf = VeriErisim.DegerAl(dr, sonSira + (int)TABTasinirIslemDetay.ENUMAlanlarOzellik.FOTOGRAF, string.Empty);
                    tDetay.ozellik.puan = VeriErisim.DegerAl(dr, sonSira + (int)TABTasinirIslemDetay.ENUMAlanlarOzellik.PUAN, string.Empty);
                    tDetay.ozellik.disSicilNo = VeriErisim.DegerAl(dr, sonSira + (int)TABTasinirIslemDetay.ENUMAlanlarOzellik.DISSICILNO, string.Empty);
                    tDetay.ozellik.disSicilNo2 = VeriErisim.DegerAl(dr, sonSira + (int)TABTasinirIslemDetay.ENUMAlanlarOzellik.DISSICILNO2, string.Empty);
                    tDetay.ozellik.bisNo = VeriErisim.DegerAl(dr, sonSira + (int)TABTasinirIslemDetay.ENUMAlanlarOzellik.BISNO, string.Empty);
                    tDetay.ozellik.eskiBisNo1 = VeriErisim.DegerAl(dr, sonSira + (int)TABTasinirIslemDetay.ENUMAlanlarOzellik.ESKIBISNO1, string.Empty);
                    tDetay.ozellik.eskiBisNo2 = VeriErisim.DegerAl(dr, sonSira + (int)TABTasinirIslemDetay.ENUMAlanlarOzellik.ESKIBISNO2, string.Empty);
                    tDetay.ozellik.rfidEtiketTurKod = VeriErisim.DegerAl(dr, sonSira + (int)TABTasinirIslemDetay.ENUMAlanlarOzellik.RFIDETIKETTURKOD, 0);
                    tDetay.ozellik.garantiBitisTarihi = VeriErisim.DegerAl(dr, sonSira + (int)TABTasinirIslemDetay.ENUMAlanlarOzellik.GARANTIBITISTARIHI);
                    tDetay.ozellik.giai = VeriErisim.DegerAl(dr, sonSira + (int)TABTasinirIslemDetay.ENUMAlanlarOzellik.GIAI, string.Empty);
                    tDetay.ozellik.ekNo = VeriErisim.DegerAl(dr, sonSira + (int)TABTasinirIslemDetay.ENUMAlanlarOzellik.EKNO, string.Empty);
                    tDetay.ozellik.hesapTarih = VeriErisim.DegerAl(dr, sonSira + (int)TABTasinirIslemDetay.ENUMAlanlarOzellik.HESAPTARIHI, string.Empty);
                    tDetay.ozellik.butceKodu = VeriErisim.DegerAl(dr, sonSira + (int)TABTasinirIslemDetay.ENUMAlanlarOzellik.BUTCEKODU, string.Empty);
                    tDetay.ozellik.blokajKodu = VeriErisim.DegerAl(dr, sonSira + (int)TABTasinirIslemDetay.ENUMAlanlarOzellik.BLOKAJKODU, string.Empty);
                    tDetay.ozellik.geldigiSube = VeriErisim.DegerAl(dr, sonSira + (int)TABTasinirIslemDetay.ENUMAlanlarOzellik.GELDIGISUBE, string.Empty);
                    tDetay.ozellik.amortismanYil = VeriErisim.DegerAl(dr, sonSira + (int)TABTasinirIslemDetay.ENUMAlanlarOzellik.AMORTISMANYIL, 0);
                    tDetay.ozellik.amortismanBitti = VeriErisim.DegerAl(dr, sonSira + (int)TABTasinirIslemDetay.ENUMAlanlarOzellik.AMORTISMANBITTI, 0);
                    tDetay.ozellik.bulunduguYer = VeriErisim.DegerAl(dr, sonSira + (int)TABTasinirIslemDetay.ENUMAlanlarOzellik.BULUNDUGUYER, "");


                    tDetay.hesapPlanAd = VeriErisim.DegerAl(dr, sonSira + sonSiraOzellik + 1, string.Empty);
                    tDetay.olcuBirimAd = VeriErisim.DegerAl(dr, sonSira + sonSiraOzellik + 2, string.Empty);
                    tDetay.ozellik.markaAd = VeriErisim.DegerAl(dr, sonSira + sonSiraOzellik + 3, string.Empty);
                    tDetay.ozellik.modelAd = VeriErisim.DegerAl(dr, sonSira + sonSiraOzellik + 4, string.Empty);
                    tDetay.ozellik.rfidEtiketTurAd = VeriErisim.DegerAl(dr, sonSira + sonSiraOzellik + 5, string.Empty);
                    tDetay.yerleskeYeriAd = VeriErisim.DegerAl(dr, sonSira + sonSiraOzellik + 6, string.Empty);

                    bilgi.Ekle(tDetay);

                    //MB aktarımı için kapatıldı. Daha sonra açılacak.
                    //if (bilgi.objeler.Count > 2000)
                    //{
                    //    bilgi.sonuc.islemSonuc = false;
                    //    bilgi.sonuc.hataStr = "Bu fişe kayıtlı malzeme sayısı 2000 den fazla oladuğu için gösterilemektedir. Belge Yazdırma, Onay gibi işlemleri Listeleme yapmadan da yapabilirsiniz.";
                    //    break;
                    //}
                }
                dr.Close();
            }
            else
            {
                bilgi.sonuc.DegerAta(ve.hata);
            }
            ve.VeriErisimKapat();

            ts = DateTime.Now - dt;
            OrtakFonksiyonlar.HataDosyaYaz("TasinirDetaySorgu.txt", DateTime.Now.ToString() + " - Gelen verinin nesneye doldurulması " + ts.TotalMilliseconds + " ms sürdü", false);


            if (bilgi.ObjeSayisi < 1 && bilgi.sonuc.islemSonuc)
            {
                bilgi.sonuc.islemSonuc = false;
                bilgi.sonuc.hataStr = TasinirMalResI.TMM088;
            }

            return bilgi;
        }

        /// <summary>
        /// Parametre olarak verilen taşınır işlem fişinin detay bilgilerinin
        /// diğer parametrelerle birlikte doğruluğunu kontrol eden yordam
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="tdetay">Taşınır işlem fişinin detay bilgileri listesini tutan nesne</param>
        /// <param name="islemTur">Taşınır işlem fişinin işlem türü</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public static Sonuc Kontrol(Kullanici kullanan, ObjectArray tdetay, int islemTur)
        {
            Sonuc sonuc = new Sonuc();
            string hataStr = string.Empty;

            for (int i = 0; i < tdetay.objeler.Count; i++)
            {
                string sicilNo = ((TasinirIslemDetay)tdetay.objeler[i]).gorSicilNo;
                if (sicilNo != "" && (i + 1) <= tdetay.objeler.Count)
                {
                    for (int j = i + 1; j < tdetay.objeler.Count; j++)
                    {
                        if (sicilNo == ((TasinirIslemDetay)tdetay.objeler[j]).gorSicilNo)
                            hataStr += string.Format(TasinirMalResI.TMM225, (i + 1), (j + 1)) + "<br>";
                    }
                }
            }

            //Hesap kodu detay olmak zorunda olmama zorunluluğu kaldırılanbilsin
            int detayHesapZorunlulugu = OrtakFonksiyonlar.ConvertToInt(System.Configuration.ConfigurationManager.AppSettings.Get("TasinirDetayHesapKullanmasin") + "", 0);

            decimal tifToplamTutar = 0;
            bool yilDevriIslemi = false;
            foreach (TasinirIslemDetay td in tdetay.objeler)
            {
                if (string.IsNullOrWhiteSpace(td.hesapPlanKod))// || td.hesapPlanKod.Replace(".", "").Length < 7) Merkez Bankasında hesap kodları 7 karakterden az
                {
                    hataStr += string.Format(TasinirMalResI.TMM226, td.siraNo.ToString()) + "<br>";
                    //if (td.hesapPlanKod.Replace(".", "").Length < 7)
                    //    break;
                }

                if (td.miktar <= (decimal)0)
                    hataStr += string.Format(TasinirMalResI.TMM227, td.siraNo.ToString()) + "<br>";
                else if (TNS.TMM.Arac.DemirbasCesidiMi(td.hesapPlanKod))
                    td.miktar = Math.Round(td.miktar);

                if (td.kdvOran < 0)
                    hataStr += string.Format(TasinirMalResI.TMM228, td.siraNo.ToString()) + "<br>";

                if (td.birimFiyat < (decimal)0)
                    hataStr += string.Format(TasinirMalResI.TMM229, td.siraNo.ToString()) + "<br>";

                if (td.gorSicilNo.Trim() != "" && !td.gorSicilNo.StartsWith(td.hesapPlanKod.Replace(".", "")) && !TNS.TMM.Arac.MerkezBankasiKullaniyor())
                    hataStr += string.Format(TasinirMalResI.TMM230, td.siraNo.ToString()) + "<br>";

                if (td.hesapPlanKod.StartsWith(((int)ENUMTasinirHesapKodu.TUKETIM).ToString()) && td.gorSicilNo.Trim() != "")
                    hataStr += string.Format(TasinirMalResI.TMM231, td.siraNo.ToString()) + "<br>";

                if (islemTur == (int)ENUMIslemTipi.YILDEVIRCIKIS ||
                    islemTur == (int)ENUMIslemTipi.YILDEVIRGIRIS)
                {
                    yilDevriIslemi = true;

                    if (!td.hesapPlanKod.StartsWith(((int)ENUMTasinirHesapKodu.TUKETIM).ToString()))
                        hataStr += TasinirMalResI.TMM232 + "<br>";
                }

                HesapPlaniSatir hs = new HesapPlaniSatir();
                hs.hesapKod = td.hesapPlanKod;

                TMMHesapPlani hp = new TMMHesapPlani();
                ObjectArray bilgiHp = hp.Listele(kullanan, hs, new Sayfalama());
                if (bilgiHp.ObjeSayisi < 1)
                    hataStr += string.Format(TasinirMalResI.TMM233, td.siraNo.ToString()) + "<br>";
                else if (((HesapPlaniSatir)bilgiHp[0]).seviye < 2)
                    hataStr += string.Format(TasinirMalResI.TMM234, td.siraNo.ToString(), hs.hesapKod) + "<br>";

                if (islemTur == (int)ENUMIslemTipi.SATINALMAGIRIS ||
                    islemTur == (int)ENUMIslemTipi.BAGISGIRIS ||
                    islemTur == (int)ENUMIslemTipi.SAYIMFAZLASIGIRIS ||
                    islemTur == (int)ENUMIslemTipi.IADEGIRIS ||
                    islemTur == (int)ENUMIslemTipi.URETILENGIRIS ||
                    islemTur == (int)ENUMIslemTipi.DEVIRGIRISKURUM ||
                    islemTur == (int)ENUMIslemTipi.ACILIS)
                {
                    //Giren malzemeler tutar olarak belirli bir miktarı geçemez
                    tifToplamTutar += td.miktar * td.birimFiyat;
                    if (enYuksekTutar > 0 && (td.miktar * td.birimFiyat) > (enYuksekTutar / 4))
                        hataStr += string.Format(TasinirMalResI.TMM235, td.siraNo.ToString(), hs.hesapKod, (enYuksekTutar / 4).ToString("#,###")) + "<br>";

                    if (bilgiHp.ObjeSayisi > 0)
                        if (!((HesapPlaniSatir)bilgiHp[0]).detay && detayHesapZorunlulugu == 0)
                            hataStr += string.Format(TasinirMalResI.TMM236, td.siraNo.ToString(), hs.hesapKod) + "<br>";
                }

                if (TNS.TMM.Arac.DemirbasCesidiMi(td.hesapPlanKod))
                {
                    if (islemTur == (int)ENUMIslemTipi.BAGISCIKIS ||
                        islemTur == (int)ENUMIslemTipi.SATISCIKIS ||
                        islemTur == (int)ENUMIslemTipi.DEVIRCIKIS ||
                        islemTur == (int)ENUMIslemTipi.DEGERARTTIR ||
                        islemTur == (int)ENUMIslemTipi.ENFLASYONARTISI ||
                        islemTur == (int)ENUMIslemTipi.HURDACIKIS ||
                        islemTur == (int)ENUMIslemTipi.DEVIRCIKISKURUM ||
                        islemTur == (int)ENUMIslemTipi.SAYIMNOKSANICIKIS ||
                        islemTur == (int)ENUMIslemTipi.DAGITIMIADECIKIS)
                    {
                        if (td.gorSicilNo.Trim() != "")
                        {
                            SicilNoHareket sicilNo = new SicilNoHareket();
                            sicilNo.yil = td.yil;
                            sicilNo.muhasebeKod = td.muhasebeKod;
                            sicilNo.harcamaBirimKod = td.harcamaKod;
                            sicilNo.ambarKod = td.ambarKod;
                            sicilNo.sicilNo = td.gorSicilNo;
                            sicilNo.fiyat = td.birimFiyat;

                            ObjectArray o = TMMSicilNoHareket.AmbardakiSicilNolar(null, kullanan, sicilNo);

                            if (o.objeler.Count <= 0)
                                hataStr += string.Format(TasinirMalResI.TMM237, td.siraNo.ToString(), td.gorSicilNo) + "<br>";
                            else
                            {
                                td.sicilNo = ((SicilNoHareket)o.objeler[0]).prSicilNo;

                                if (td.birimFiyat == 0)
                                {
                                    td.birimFiyat = ((SicilNoHareket)o.objeler[0]).fiyat;
                                    td.kdvOran = ((SicilNoHareket)o.objeler[0]).kdvOran;
                                }

                                if (islemTur != (int)ENUMIslemTipi.ENFLASYONARTISI)
                                {
                                    if (td.birimFiyat.ToString("#,###.##") != ((SicilNoHareket)o.objeler[0]).fiyat.ToString("#,###.##")) //Giriş fiyatı 14,324567 gibi ise 14,32 den işlem görüyor
                                        hataStr += string.Format(TasinirMalResI.TMM238, td.siraNo.ToString(), td.gorSicilNo, td.birimFiyat.ToString("#,##0.00"), ((SicilNoHareket)o.objeler[0]).fiyat.ToString("#,##0.00")) + "<br>";
                                    if (td.kdvOran != ((SicilNoHareket)o.objeler[0]).kdvOran)
                                        hataStr += string.Format(TasinirMalResI.TMM239, td.siraNo.ToString(), td.gorSicilNo, ((SicilNoHareket)o.objeler[0]).kdvOran) + "<br>";
                                }
                            }
                        }
                        else
                            hataStr += string.Format(TasinirMalResI.TMM240, td.siraNo.ToString(), td.hesapPlanKod) + "<br>";
                    }

                    if (islemTur == (int)ENUMIslemTipi.DEVIRGIRIS || islemTur == (int)ENUMIslemTipi.DAGITIMIADEGIRIS)
                    {
                        int sicilGecici = (int)TMMServis.UzaySayiDegerGetir("TASPRSICILGETIR", td.gonMuhasebeKod + "-" + td.gonHarcamaKod.Replace(".", "") + "-" + td.gorSicilNo, true, "");

                        if (sicilGecici < 1)
                            hataStr += string.Format(TasinirMalResI.TMM237, td.siraNo.ToString(), td.gorSicilNo) + "<br>";

                        if (islemTur == (int)ENUMIslemTipi.DEVIRGIRIS || islemTur == (int)ENUMIslemTipi.DAGITIMIADEGIRIS)
                            td.eSicilNo = sicilGecici.ToString();
                    }
                    else if (islemTur == (int)ENUMIslemTipi.TUKETIMCIKIS)
                        hataStr += string.Format(TasinirMalResI.TMM241, td.siraNo.ToString()) + "<br>";
                    else if (islemTur == (int)ENUMIslemTipi.KULLANILMAZCIKIS)
                        hataStr += string.Format(TasinirMalResI.TMM242, td.siraNo.ToString()) + "<br>";

                }
                else if (OrtakFonksiyonlar.ConvertToInt(td.hesapPlanKod.Substring(0, 3), 0) == (int)ENUMTasinirHesapKodu.TUKETIM)
                {
                    //KHK Çıkış ilk giren kontrolü olmasın
                    string kontrolIslemTipKod = System.Configuration.ConfigurationManager.AppSettings.Get("TasinirKHKIslemKoduCikis") + "";

                    if (islemTur >= (int)ENUMIslemTipi.TUKETIMCIKIS && islemTur != OrtakFonksiyonlar.ConvertToInt(kontrolIslemTipKod, 0))
                    {
                        StokHareketBilgi shBilgi = new StokHareketBilgi();
                        shBilgi.yil = td.yil;
                        shBilgi.hesapPlanKod = td.hesapPlanKod.Replace(".", "");
                        shBilgi.muhasebeKod = td.muhasebeKod;
                        shBilgi.harcamaKod = td.harcamaKod;
                        shBilgi.ambarKod = td.ambarKod;
                        //shBilgi.birimFiyat = td.birimFiyat;
                        //shBilgi.kdvOran = td.kdvOran;

                        ObjectArray bilgi = TMMStokHareket.AmbardakiStoklariGetir(null, kullanan, shBilgi);

                        if (bilgi.objeler.Count <= 0)
                            hataStr += string.Format(TasinirMalResI.TMM243, td.siraNo.ToString(), td.hesapPlanKod) + "<br>";
                        else
                        {
                            string tuketimHata = "";
                            int sonKontrolEdilenSira = -1;
                            foreach (StokHareketBilgi stoktaki in bilgi.objeler)
                            {
                                foreach (TasinirIslemDetay tifdeki in tdetay.objeler)
                                {
                                    if (tuketimHata != "")
                                        break;

                                    if (stoktaki.hesapPlanKod.Replace(".", "").Trim() == tifdeki.hesapPlanKod.Replace(".", "").Trim() && sonKontrolEdilenSira < tifdeki.siraNo)
                                    {
                                        sonKontrolEdilenSira = tifdeki.siraNo;

                                        if ((Math.Round(stoktaki.birimFiyat, 10) != Math.Round(tifdeki.birimFiyat, 10) || stoktaki.kdvOran != tifdeki.kdvOran) && !yilDevriIslemi)
                                        {
                                            tuketimHata += string.Format(TasinirMalResI.TMM244, tifdeki.siraNo, stoktaki.miktar, stoktaki.birimFiyat, stoktaki.kdvOran) + "<br>";
                                            OrtakFonksiyonlar.HataDosyaYaz("YilDevriHata.txt", "tuketimHata4", false);
                                        }

                                        if (tuketimHata == "")
                                        {
                                            if (stoktaki.miktar < (decimal)tifdeki.miktar && !yilDevriIslemi)
                                                tuketimHata += string.Format(TasinirMalResI.TMM245, tifdeki.siraNo, tifdeki.hesapPlanKod, stoktaki.miktar) + "<br>";

                                            //İlk satırdaki malzemenin hepsi kullanılmamış ve tifde aynı hesap kodlu başka bir satır var ise
                                            else if (stoktaki.miktar > (decimal)tifdeki.miktar)
                                            {
                                                foreach (TasinirIslemDetay tifkontrol in tdetay.objeler)
                                                {
                                                    if (stoktaki.hesapPlanKod.Replace(".", "").Trim() == tifkontrol.hesapPlanKod.Replace(".", "").Trim() &&
                                                        tifkontrol.siraNo > tifdeki.siraNo && !yilDevriIslemi)
                                                    {
                                                        tuketimHata += string.Format(TasinirMalResI.TMM246, tifkontrol.siraNo, tifdeki.siraNo, stoktaki.miktar) + " <br>";
                                                        OrtakFonksiyonlar.HataDosyaYaz("YilDevriHata.txt", "tuketimHata5", false);
                                                        break;
                                                    }
                                                }
                                            }
                                            else
                                                break;
                                        }
                                    }
                                }

                            }

                            hataStr += tuketimHata;
                            decimal miktarKontrol = 0;
                            decimal miktarKontrol2 = 0;
                            //TİFde 2 farklı satır giriş yapıldı fakat stokta tek malzeme var ise aşağıdaki kontrolde gerekli
                            foreach (TasinirIslemDetay td2 in tdetay.objeler)
                            {
                                if (td2.hesapPlanKod.Replace(".", "").Trim() == td.hesapPlanKod.Replace(".", "").Trim() &&
                                    Math.Round(td2.birimFiyat, 10) == Math.Round(td.birimFiyat, 10) &&
                                    td2.kdvOran == td.kdvOran)
                                    miktarKontrol += (decimal)td2.miktar;
                            }
                            foreach (StokHareketBilgi td2 in bilgi.objeler)
                            {
                                if (td2.hesapPlanKod.Replace(".", "").Trim() == td.hesapPlanKod.Replace(".", "").Trim() &&
                                    Math.Round(td2.birimFiyat, 10) == Math.Round(td.birimFiyat, 10) &&
                                    td2.kdvOran == td.kdvOran)
                                    miktarKontrol2 += td2.miktar;
                            }
                            if (Math.Round(miktarKontrol, 4) > Math.Round(miktarKontrol2, 4))
                                hataStr += string.Format(TasinirMalResI.TMM247, td.siraNo, td.hesapPlanKod, miktarKontrol2.ToString(), miktarKontrol.ToString()) + "<br>";
                        }
                    }
                }
            }

            if (enYuksekTutar > 0 && tifToplamTutar > 0 && tifToplamTutar > enYuksekTutar)
                hataStr += string.Format(TasinirMalResI.TMM248, enYuksekTutar.ToString("#,###")) + "<br>";

            if (hataStr != String.Empty)
                sonuc.islemSonuc = false;

            if (yilDevriIslemi)
                OrtakFonksiyonlar.HataDosyaYaz("YilDevriHata.txt", "tuketimHata6:" + hataStr, false);
            sonuc.hataStr = hataStr;
            return sonuc;
        }

        public static Sonuc VTAlanUzunlukKontrolEt(TNSCollection tifDetay)
        {
            int muhasebeUzunluk = TNS.TMM.Arac.DegiskenDegerBul(0, "/KODYAPI/KOD12/KODYAPI").Length;
            int harcamaUzunluk = TNS.TMM.Arac.DegiskenDegerBul(0, "/KODYAPI/KOD10/KODYAPI").Replace(".", "").Length;
            int hesapPlanUzunluk = TNS.TMM.Arac.DegiskenDegerBul(0, "/KODYAPI/KOD13/KODYAPI").Replace(".", "").Length;

            Sonuc sonuc = new Sonuc();

            foreach (TasinirIslemDetay detay in tifDetay)
            {
                //TASISLEMDETAY tablosu alanları kontrolü
                if (detay.muhasebeKod.Length > muhasebeUzunluk)
                    sonuc.hataStr = TasinirMalResI.TMM337 + "<br>";
                if (detay.harcamaKod.Replace(".", "").Length > harcamaUzunluk)
                    sonuc.hataStr += TasinirMalResI.TMM338 + "<br>";
                if (detay.fisNo.Length > 10)
                    sonuc.hataStr += TasinirMalResI.TMM339 + "<br>";
                if (detay.hesapPlanKod.Replace(".", "").Length > hesapPlanUzunluk)
                    sonuc.hataStr += TasinirMalResI.TMM340 + "<br>";
                if (detay.eSicilNo.Length > 50)
                    sonuc.hataStr += TasinirMalResI.TMM341 + "<br>";
                if (detay.eTedarikSekli.Length > 255)
                    sonuc.hataStr += TasinirMalResI.TMM342 + "<br>";
                if (detay.eAciklama.Length > 1000)////
                    sonuc.hataStr += TasinirMalResI.TMM343 + "<br>";

                //TASISLEMOZELLIK tablosu alanları kontrolü
                if (detay.ozellik.saseNo.Length > 1000)////
                    sonuc.hataStr += TasinirMalResI.TMM344 + "<br>";
                if (detay.ozellik.motorNo.Length > 255)
                    sonuc.hataStr += TasinirMalResI.TMM345 + "<br>";
                if (detay.ozellik.plaka.Length > 255)
                    sonuc.hataStr += TasinirMalResI.TMM346 + "<br>";
                if (detay.ozellik.ciltNo.Length > 255)
                    sonuc.hataStr += TasinirMalResI.TMM347 + "<br>";
                if (detay.ozellik.dil.Length > 255)
                    sonuc.hataStr += TasinirMalResI.TMM348 + "<br>";
                if (detay.ozellik.yazarAdi.Length > 255)
                    sonuc.hataStr += TasinirMalResI.TMM349 + "<br>";
                if (detay.ozellik.adi.Length > 1000)////
                    sonuc.hataStr += TasinirMalResI.TMM350 + "<br>";
                if (detay.ozellik.yayinYeri.Length > 500)
                    sonuc.hataStr += TasinirMalResI.TMM351 + "<br>";
                if (detay.ozellik.yayinTarihi.Length > 255)
                    sonuc.hataStr += TasinirMalResI.TMM352 + "<br>";
                if (detay.ozellik.neredenGeldi.Length > 255)
                    sonuc.hataStr += TasinirMalResI.TMM353 + "<br>";
                if (detay.ozellik.boyutlari.Length > 255)
                    sonuc.hataStr += TasinirMalResI.TMM354 + "<br>";
                if (detay.ozellik.satirSayisi.Length > 255)
                    sonuc.hataStr += TasinirMalResI.TMM355 + "<br>";
                if (detay.ozellik.yaprakSayisi.Length > 255)
                    sonuc.hataStr += TasinirMalResI.TMM356 + "<br>";
                if (detay.ozellik.sayfaSayisi.Length > 255)
                    sonuc.hataStr += TasinirMalResI.TMM357 + "<br>";
                if (detay.ozellik.ciltTuru.Length > 255)
                    sonuc.hataStr += TasinirMalResI.TMM358 + "<br>";
                if (detay.ozellik.cesidi.Length > 255)
                    sonuc.hataStr += TasinirMalResI.TMM359 + "<br>";
                if (detay.ozellik.yeriKonusu.Length > 255)
                    sonuc.hataStr += TasinirMalResI.TMM360 + "<br>";
                if (detay.ozellik.gelisTarihi.Length > 255)
                    sonuc.hataStr += TasinirMalResI.TMM361 + "<br>";
                if (detay.ozellik.neredeBulundu.Length > 255)
                    sonuc.hataStr += TasinirMalResI.TMM362 + "<br>";
                if (detay.ozellik.cagi.Length > 255)
                    sonuc.hataStr += TasinirMalResI.TMM363 + "<br>";
                if (detay.ozellik.durumuMaddesi.Length > 255)
                    sonuc.hataStr += TasinirMalResI.TMM364 + "<br>";
                if (detay.ozellik.onYuz.Length > 255)
                    sonuc.hataStr += TasinirMalResI.TMM365 + "<br>";
                if (detay.ozellik.arkaYuz.Length > 255)
                    sonuc.hataStr += TasinirMalResI.TMM366 + "<br>";
                if (detay.ozellik.fotograf.Length > 255)
                    sonuc.hataStr += TasinirMalResI.TMM367 + "<br>";
                if (detay.ozellik.puan.Length > 255)
                    sonuc.hataStr += TasinirMalResI.TMM368 + "<br>";
                if (detay.ozellik.disSicilNo.Length > 255)
                    sonuc.hataStr += TasinirMalResI.TMM369 + "<br>";

                if (!string.IsNullOrEmpty(sonuc.hataStr))
                {
                    sonuc.hataStr = string.Format(TasinirMalResI.TMM370, detay.siraNo) + "<br>" + sonuc.hataStr;
                    sonuc.islemSonuc = false;
                    break;
                }
            }

            return sonuc;
        }
    }
}