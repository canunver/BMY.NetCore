using System.Collections;
using System.Collections.Generic;
using System.Data;
using TNS.KYM;
using TNS.TMM.IsKurallari.Veri;
using TNS.Veri;

namespace TNS.TMM.IsKurallari
{
    /// <summary>
    /// Taşınır demirbaş bilgileriyle ilgili, iş kuralları kontrollerini yapıp,
    /// veri tabanı bağlantısını alıp, veri tabanı fonksiyonlarını çağırarak
    /// okuma, yazma işlemlerini gerçekleştiren fonksiyonları tutan sınıf
    /// </summary>
    public class TMMSicilNoHareket
    {
        /// <summary>
        /// Taşınır demirbaşlarına sicil numarası verme yöntemini tutan değişken
        /// Kurum bazında veya harcama birimi bazında olabilir.
        /// </summary>
        static string verisSekli = System.Configuration.ConfigurationManager.AppSettings.Get("TasinirSicilNoVermeTuru") + "";

        /// <summary>
        /// sicilNo parametresinde verilen demirbaşın devir girişi işlemi için uygun olup olmadığını kontrol eden yordam
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="sicilNo">Demirbaşa ilişkin bilgileri tutan nesne</param>
        /// <param name="miktar">Devir girişi yapılacak miktar (1 olması bekleniyor.)</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public static Sonuc DevirGirisiYapilabilirmi(VeriErisim veriErisim, Kullanici kullanan, SicilNoHareket sicilNo, int miktar)
        {
            Sonuc sonuc = new Sonuc();

            sonuc.islemSonuc = true;

            if (sicilNo.islemTurKod != (int)ENUMIslemTipi.DEVIRGIRIS && sicilNo.islemTurKod != (int)ENUMIslemTipi.DAGITIMIADEGIRIS)
                return sonuc;

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABSicilHareket tabSicilNo = new TABSicilHareket(ve);

            for (int i = 0; i < miktar; i++)
            {
                sicilNo.prSicilNo = OrtakFonksiyonlar.ConvertToInt(sicilNo.eSicilNo, 0);

                IDataReader r = tabSicilNo.DevirCikisiSiraNoVer(sicilNo, sicilNo.islemTurKod);

                int siraNo = 0;
                if (r.Read())
                    siraNo = VeriErisim.DegerAl(r, 0, 0);

                r.Close();

                if (siraNo > 0)
                {
                    if (tabSicilNo.SonrasindaHareketVarmi(sicilNo.prSicilNo, siraNo, sicilNo.muhasebeKod, sicilNo.harcamaBirimKod))
                    {
                        sonuc.islemSonuc = false;
                        break;
                    }
                }
                else
                {
                    sonuc.islemSonuc = false;
                    break;
                }
            }

            ve.VeriErisimKapat();
            return sonuc;
        }

        /// <summary>
        /// Parametre olarak verilen demirbaşlara sicil numarası üreten ve hareketi kaydeden yordam
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="sicilNolar">Sicil numarası verilecek demirbaşları tutan nesne</param>
        /// <param name="numaraVermeIslemi">Numara verme işlemi olup olmadığını tutan nesne. Merkez bankası için onay öncesinde sicil numarası verilmesi isteniyor.</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public static Sonuc SicilNoVer(VeriErisim veriErisim, Kullanici kullanan, ObjectArray sicilNolar, bool numaraVermeIslemi = false)
        {
            Sonuc sonuc = new Sonuc();
            ///////////////////////////////////////////////////////////
            bool muhasebeKdvMukellefiMi = false;
            TMMMuhasebe tmmMuhasebe = new TMMMuhasebe();
            muhasebeKdvMukellefiMi = tmmMuhasebe.KdvMukellefiMi(((SicilNoHareket)sicilNolar[0]).muhasebeKod);
            ///////////////////////////////////////////////////////////

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));
            VeriErisim ve2 = VeriErisim.VeriErisimBul(null, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABSicilHareket tabSicilNo = new TABSicilHareket(ve);
            TABGenel tabGenel = new TABGenel(ve2);

            bool merkezBankasiMi = TNS.TMM.Arac.MerkezBankasiKullaniyor();

            ObjectArray sicilNoNumaralar = new ObjectArray();
            SicilNoHareket sicil = new SicilNoHareket();

            if (merkezBankasiMi)
            {
                sicil.yil = ((SicilNoHareket)sicilNolar.objeler[0]).yil;
                sicil.muhasebeKod = ((SicilNoHareket)sicilNolar.objeler[0]).muhasebeKod;
                sicil.harcamaBirimKod = ((SicilNoHareket)sicilNolar.objeler[0]).harcamaBirimKod;
                sicil.ambarKod = ((SicilNoHareket)sicilNolar.objeler[0]).ambarKod;
                sicil.fisNo = ((SicilNoHareket)sicilNolar.objeler[0]).fisNo;

                sicilNoNumaralar = ListeleSicilNoNumara(ve, kullanan, sicil);
            }

            ve.BeginTransaction();

            if (sicilNoNumaralar.objeler.Count > 0)
            {
                sonuc = tabSicilNo.SilSicilNoHareket(sicil);

                if (sonuc.islemSonuc)
                {
                    int[] prSicilNoListe = new int[sicilNoNumaralar.objeler.Count];
                    int sira = 0;
                    foreach (SicilNoHareket sh in sicilNoNumaralar.objeler)
                    {
                        prSicilNoListe[sira] = sh.prSicilNo;
                        sira++;
                    }

                    //TASSICILNOOZELLIK kayıtlarınında silinmesi gerekiyor
                    sonuc = new TMMSicilNoOzellik().Sil(kullanan, 0, "", "", "", prSicilNoListe);

                }

                if (!sonuc.islemSonuc)
                {
                    ve.Rollback();
                    ve.VeriErisimKapat();
                    ve2.VeriErisimKapat();
                    return sonuc;
                }
            }

            bool sicilNolarBitti = false;
            int sicilNoIndex = 0;
            Hashtable htHesapKod = new Hashtable();
            foreach (SicilNoHareket sicilNo in sicilNolar.objeler)
            {
                int miktar = sicilNo.miktar;
                if (miktar == 0)
                    continue;

                sicilNo.hesapPlanKod = sicilNo.hesapPlanKod.Replace(".", "");

                if (sicilNo.yil == 0)
                    sonuc.hataStr += TasinirMalResI.TMM036 + "<br>";
                if (sicilNo.muhasebeKod == string.Empty)
                    sonuc.hataStr += TasinirMalResI.TMM013 + "<br>";
                else if (sicilNo.harcamaBirimKod == string.Empty)
                    sonuc.hataStr += TasinirMalResI.TMM014 + "<br>";
                else if (sicilNo.fisNo == string.Empty)
                    sonuc.hataStr += TasinirMalResI.TMM211 + "<br>";
                else if (sicilNo.hesapPlanKod == string.Empty)
                    sonuc.hataStr += TasinirMalResI.TMM027 + "<br>";

                //Hesap Kod kontrolü
                //********************
                string kontrol = (string)htHesapKod[sicilNo.hesapPlanKod];

                if (kontrol == null || kontrol == "")
                {
                    if (sicilNo.islemTurKod == (int)ENUMIslemTipi.DEVIRGIRIS || sicilNo.islemTurKod == (int)ENUMIslemTipi.DAGITIMIADEGIRIS)
                        sonuc.hataStr += Genel.KontrolHesapPlan(kullanan, sicilNo.hesapPlanKod, sicilNo.fisSiraNo, false, true);
                    else
                        sonuc.hataStr += Genel.KontrolHesapPlan(kullanan, sicilNo.hesapPlanKod, sicilNo.fisSiraNo, true, false);
                }
                htHesapKod[sicilNo.hesapPlanKod] = "1";

                if (sonuc.hataStr != string.Empty)
                {
                    sonuc.islemSonuc = false;
                    ve.Rollback();
                    ve.VeriErisimKapat();
                    ve2.VeriErisimKapat();
                    return sonuc;
                }

                string muhasebe = sicilNo.muhasebeKod;
                string harcamaBirimi = sicilNo.harcamaBirimKod;

                if (verisSekli == "Kurum")
                {
                    muhasebe = "";
                    harcamaBirimi = "";
                }
                //sayaç kod için gerekli olan muhasebe ve harcamabirim kodu yukarıda işlem görüyor
                string sayacKod = "SICIL" + sicilNo.yil + muhasebe + harcamaBirimi.Replace(".", "") + sicilNo.hesapPlanKod;
                string pSayacKod = "PRSICIL";

                ObjectArray oprSicilNo = new ObjectArray();
                lock (muhasebe.ToString() + harcamaBirimi + sicilNo.hesapPlanKod)
                {
                    bool eskiyiKullan = true;
                    //TCCB de 10000lik kitap kaydı yapılıyor. Burada eski sicil no kullanılmasın. 
                    //Bu nedenle eski sicil no bulma kısmı çalışmasın
                    if (sicilNo.hesapPlanKod.Replace(".", "").StartsWith("25507"))
                        eskiyiKullan = false;

                    if (!merkezBankasiMi && eskiyiKullan)
                    {
                        //iptal edilmiş sicilnoları kullan
                        IDataReader r = null;

                        string fisNo = "";
                        //if (merkezBankasiMi)//Numarası kaldırılan fiş yeniden aynı numaraları alsın. Başkası kullanamasın.
                        //    fisNo = sicilNo.fisNo;

                        if (sicilNo.islemTurKod == (int)ENUMIslemTipi.DEVIRGIRIS || sicilNo.islemTurKod == (int)ENUMIslemTipi.DAGITIMIADEGIRIS)
                        {
                            sicilNo.siraNo = 0;
                            sicilNo.prSicilNo = OrtakFonksiyonlar.ConvertToInt(sicilNo.eSicilNo, 0);

                            //*************Eğer iptal edilen bu prsicilnonun uzantısı bu birimde var ise onu kullan
                            r = tabSicilNo.IptalEdilenNolariVer(sicilNo.yil, muhasebe, harcamaBirimi, sicilNo.hesapPlanKod, sicilNo.prSicilNo, 0, fisNo);
                        }
                        else
                            r = tabSicilNo.IptalEdilenNolariVer(sicilNo.yil, muhasebe, harcamaBirimi, sicilNo.hesapPlanKod, 0, 1, fisNo);//Sira no 1 olma sebebi giriş işlemlerinde iptal edilen ilk satırsa prsicil kullanılabilir. Diğer durumda devir giriş yapılmış sonra iptal edilmiştir.

                        while (r.Read())
                        {
                            SicilNoHareket prSicilNo = new SicilNoHareket();

                            prSicilNo.prSicilNo = VeriErisim.DegerAl(r, 0, 0);
                            prSicilNo.sicilNo = VeriErisim.DegerAl(r, 1, "");
                            prSicilNo.siraNo = VeriErisim.DegerAl(r, 2, 0);

                            //Erdal --- Harcama birimi bazında sicil vermede devir giriş işleminde
                            //sicil sıra numarası boş kalıyor iptalden kullanıldığında. O nedenle if commentlendi.
                            //if (sicilNo.islemTurKod != (int)ENUMIslemTipi.DEVIRGIRIS)
                            prSicilNo.sicilSiraNo = VeriErisim.DegerAl(r, 3, "");

                            oprSicilNo.Ekle(prSicilNo);
                        }
                        r.Close();
                        //****************************************
                    }
                    else if (sicilNo.islemTurKod == (int)ENUMIslemTipi.DEVIRGIRIS || sicilNo.islemTurKod == (int)ENUMIslemTipi.DAGITIMIADEGIRIS)
                    {
                        sicilNo.siraNo = 0;
                        sicilNo.prSicilNo = OrtakFonksiyonlar.ConvertToInt(sicilNo.eSicilNo, 0);
                    }



                    for (int i = 0; i < miktar; i++)
                    {
                        if (sicilNo.islemTurKod != (int)ENUMIslemTipi.DEVIRGIRIS && sicilNo.islemTurKod != (int)ENUMIslemTipi.DAGITIMIADEGIRIS)
                            sicilNo.prSicilNo = 0;

                        sicilNo.sicilNo = "";

                        if (oprSicilNo.objeler.Count > i)
                        {
                            sicilNo.prSicilNo = ((SicilNoHareket)oprSicilNo.objeler[i]).prSicilNo;
                            sicilNo.sicilNo = ((SicilNoHareket)oprSicilNo.objeler[i]).sicilNo;
                            sicilNo.siraNo = ((SicilNoHareket)oprSicilNo.objeler[i]).siraNo;
                            sicilNo.sicilSiraNo = ((SicilNoHareket)oprSicilNo.objeler[i]).sicilSiraNo;

                            //Erdal --- Yeni malzemeler eski yıllara ait sicil numarası almasın diye
                            if (!merkezBankasiMi)
                            {
                                if (sicilNo.yil.ToString().Substring(2) != sicilNo.sicilNo.Substring(sicilNo.hesapPlanKod.Length, 2))
                                {
                                    sicilNo.sicilNo = string.Empty;
                                    sicilNo.sicilSiraNo = string.Empty;
                                }
                            }
                        }

                        if (sicilNo.islemTurKod == (int)ENUMIslemTipi.DEVIRGIRIS || sicilNo.islemTurKod == (int)ENUMIslemTipi.DAGITIMIADEGIRIS)
                        {
                            ////*************Eğer iptal edilen bu prsicilnonun uzantısı bu birimde var ise onu kullan
                            if (verisSekli == "Kurum")
                            {
                                if (sicilNo.sicilNo == "")
                                    //eski muhasebe ve hb verilerek eski sicil no alınmalı
                                    sicilNo.sicilNo = TMM.IsKurallari.Genel.GorSicilNoGetir(sicilNo.eMuhasebeKod, sicilNo.eHarcamaBirimKod.Replace(".", ""), OrtakFonksiyonlar.ConvertToInt(sicilNo.eSicilNo, 0));

                                //Erdal --- Kurum bazında sicil verme kullanılıyorsa
                                //devir giriş işlemlerinde sicil sıra numarası boş olmalı.
                                sicilNo.sicilSiraNo = string.Empty;

                                if (string.IsNullOrEmpty(sicilNo.sicilNo))
                                {
                                    sonuc.hataStr = TasinirMalResI.TMM212 + "<br>";
                                    sonuc.islemSonuc = false;
                                    break;
                                }
                            }
                        }

                        if (sicilNo.sicilNo == "")
                        {
                            //Erdal --- Bir şekilde sayacı arttıramadığı durumlar oluyor gibi geliyor bana
                            //Ondan dolayı commentledim ve aşağıda YeniSicilNoVer fonksiyonu ile sicil ürettim.
                            //Sicil sirano al
                            sicilNo.sicilSiraNo = tabGenel.BelgeNoAl(sayacKod, "000000");

                            //Sicil no oluştur
                            sicilNo.sicilNo = sicilNo.hesapPlanKod + "" + sicilNo.yil.ToString().Substring(2) + "" + sicilNo.sicilSiraNo;

                            if (merkezBankasiMi)
                                sicilNo.sicilNo = "";//Merkez Bankası için Aşağıda prsicilnoya eşitlenecek

                            ////Erdal --- YeniSicilNoVer fonksiyonu yeni sicil no alana kadar devam ediyor
                            ////yeni sicil no bulunca dönüyor. O noktaya kadar sayacı arttırıyor.
                            //sicilNo.sicilNo = TMMSicilTekrarEden.YeniSicilNoVer(ve, muhasebe, harcamaBirimi, sicilNo.yil, sicilNo.hesapPlanKod);
                            //if (!string.IsNullOrEmpty(sicilNo.sicilNo))
                            //    sicilNo.sicilSiraNo = sicilNo.sicilNo.Substring(sicilNo.sicilNo.Length - 6);

                            if (sicilNo.sicilSiraNo == string.Empty)
                            {
                                sonuc.islemSonuc = false;
                                sonuc.hataStr = TasinirMalResI.TMM213 + "<br>";
                                break;
                            }
                        }

                        bool yeniSicilNo = false;
                        if (sicilNo.islemTurKod != (int)ENUMIslemTipi.DEVIRGIRIS && sicilNo.islemTurKod != (int)ENUMIslemTipi.DAGITIMIADEGIRIS)
                        {
                            if (sicilNoNumaralar.objeler.Count > i && merkezBankasiMi && !sicilNolarBitti)
                            {
                                sicilNo.prSicilNo = ((SicilNoHareket)sicilNoNumaralar.objeler[sicilNoIndex]).prSicilNo;
                                sicilNoIndex++;
                                if (sicilNoNumaralar.objeler.Count == sicilNoIndex) sicilNolarBitti = true;

                                //Sicil No silindiğinde sicil özellik tablosunda siliniyor.
                                yeniSicilNo = true;
                            }


                            if (sicilNo.prSicilNo == 0)
                            {
                                //Programatik sicilno al
                                sicilNo.prSicilNo = OrtakFonksiyonlar.ConvertToInt(tabGenel.BelgeNoAl(pSayacKod, "00000000"), 0);

                                if (merkezBankasiMi)
                                    sonuc = KaydetSicilNoNumara(null, kullanan, sicilNo);

                                if (sicilNo.prSicilNo == 0)
                                {
                                    sonuc.islemSonuc = false;
                                    sonuc.hataStr = TasinirMalResI.TMM214 + "<br>";
                                    break;
                                }
                                else
                                    yeniSicilNo = true;
                            }
                        }
                        else if (sicilNo.prSicilNo == 0)
                        {
                            sonuc.islemSonuc = false;
                            sonuc.hataStr = TasinirMalResI.TMM215 + "<br>";
                            break;
                        }

                        if (merkezBankasiMi && sicilNo.sicilNo == "") sicilNo.sicilNo = sicilNo.prSicilNo.ToString();


                        if (merkezBankasiMi && numaraVermeIslemi)
                            sicilNo.durum = (int)ENUMSicilDurum.IPTAL;
                        else
                            sicilNo.durum = (int)ENUMSicilDurum.AKTIF;


                        if (!yeniSicilNo && TNS.TMM.Arac.MerkezBankasiKullaniyor())
                        {
                            var fiyatListe = TMMSicilNoHareket.FiyatGetir(ve, kullanan, sicilNo.prSicilNo);

                            sicilNo.fiyat = fiyatListe["KDVLIBIRIMFIYAT"];
                            sicilNo.degerArtisi = fiyatListe["DEGERARTISI"];
                        }

                        sonuc = tabSicilNo.Kaydet(sicilNo, muhasebeKdvMukellefiMi);
                        if ((yeniSicilNo ||
                            (sicilNo.ozellik.adi != "" || sicilNo.ozellik.arkaYuz != "" || sicilNo.ozellik.boyutlari != "" || sicilNo.ozellik.cagi != ""
                            || sicilNo.ozellik.cesidi != "" || sicilNo.ozellik.ciltNo != "" || sicilNo.ozellik.ciltTuru != "" || sicilNo.ozellik.dil != ""
                            || sicilNo.ozellik.disSicilNo != "" || sicilNo.ozellik.durumuMaddesi != "" || sicilNo.ozellik.fotograf != "" || sicilNo.ozellik.gelisTarihi != ""
                            || sicilNo.ozellik.markaAd != "" || sicilNo.ozellik.markaKod > 0 || sicilNo.ozellik.modelAd != "" || sicilNo.ozellik.modelKod > 0
                            || sicilNo.ozellik.motorNo != "" || sicilNo.ozellik.neredeBulundu != "" || sicilNo.ozellik.neredenGeldi != ""
                            || sicilNo.ozellik.onYuz != "" || sicilNo.ozellik.plaka != "" || sicilNo.ozellik.puan != ""
                            || sicilNo.ozellik.saseNo != "" || sicilNo.ozellik.satirSayisi != "" || sicilNo.ozellik.sayfaSayisi != ""
                            || sicilNo.ozellik.yaprakSayisi != "" || sicilNo.ozellik.yayinTarihi != "" || sicilNo.ozellik.yayinYeri != ""
                            || sicilNo.ozellik.yazarAdi != "" || sicilNo.ozellik.yeriKonusu != ""))
                            && sonuc.islemSonuc)
                        {
                            TABSicilNoOzellik tabSicilNoOzellik = new TABSicilNoOzellik(ve);
                            SicilNoOzellik sicilNoOzellik = new SicilNoOzellik();
                            sicilNoOzellik.adi = sicilNo.ozellik.adi;
                            sicilNoOzellik.arkaYuz = sicilNo.ozellik.arkaYuz;
                            sicilNoOzellik.boyutlari = sicilNo.ozellik.boyutlari;
                            sicilNoOzellik.cagi = sicilNo.ozellik.cagi;
                            sicilNoOzellik.cesidi = sicilNo.ozellik.cesidi;
                            sicilNoOzellik.ciltNo = sicilNo.ozellik.ciltNo;
                            sicilNoOzellik.ciltTuru = sicilNo.ozellik.ciltTuru;
                            sicilNoOzellik.dil = sicilNo.ozellik.dil;
                            sicilNoOzellik.disSicilNo = sicilNo.ozellik.disSicilNo;
                            sicilNoOzellik.durumuMaddesi = sicilNo.ozellik.durumuMaddesi;
                            sicilNoOzellik.fotograf = sicilNo.ozellik.fotograf;
                            sicilNoOzellik.gelisTarihi = sicilNo.ozellik.gelisTarihi;
                            sicilNoOzellik.markaAd = sicilNo.ozellik.markaAd;
                            sicilNoOzellik.markaKod = sicilNo.ozellik.markaKod;
                            sicilNoOzellik.modelAd = sicilNo.ozellik.modelAd;
                            sicilNoOzellik.modelKod = sicilNo.ozellik.modelKod;
                            sicilNoOzellik.motorNo = sicilNo.ozellik.motorNo;
                            sicilNoOzellik.neredeBulundu = sicilNo.ozellik.neredeBulundu;
                            sicilNoOzellik.neredenGeldi = sicilNo.ozellik.neredenGeldi;
                            sicilNoOzellik.onYuz = sicilNo.ozellik.onYuz;
                            sicilNoOzellik.plaka = sicilNo.ozellik.plaka;
                            sicilNoOzellik.prSicilno = sicilNo.prSicilNo;
                            sicilNoOzellik.puan = sicilNo.ozellik.puan;
                            sicilNoOzellik.saseNo = sicilNo.ozellik.saseNo;
                            sicilNoOzellik.satirSayisi = sicilNo.ozellik.satirSayisi;
                            sicilNoOzellik.sayfaSayisi = sicilNo.ozellik.sayfaSayisi;
                            sicilNoOzellik.yaprakSayisi = sicilNo.ozellik.yaprakSayisi;
                            sicilNoOzellik.yayinTarihi = sicilNo.ozellik.yayinTarihi;
                            sicilNoOzellik.yayinYeri = sicilNo.ozellik.yayinYeri;
                            sicilNoOzellik.yazarAdi = sicilNo.ozellik.yazarAdi;
                            sicilNoOzellik.yeriKonusu = sicilNo.ozellik.yeriKonusu;

                            sicilNoOzellik.disSicilNo2 = sicilNo.ozellik.disSicilNo2;
                            sicilNoOzellik.bisNo = sicilNo.ozellik.bisNo;
                            sicilNoOzellik.eskiBisNo1 = sicilNo.ozellik.eskiBisNo1;
                            sicilNoOzellik.eskiBisNo2 = sicilNo.ozellik.eskiBisNo2;
                            sicilNoOzellik.rfidEtiketTurKod = sicilNo.ozellik.rfidEtiketTurKod;
                            sicilNoOzellik.garantiBitisTarihi = sicilNo.ozellik.garantiBitisTarihi;
                            sicilNoOzellik.giai = sicilNo.ozellik.giai;
                            sicilNoOzellik.ekNo = sicilNo.ozellik.ekNo;
                            sicilNoOzellik.hesapTarih = sicilNo.ozellik.hesapTarih;
                            sicilNoOzellik.butceKodu = sicilNo.ozellik.butceKodu;
                            sicilNoOzellik.blokajKodu = sicilNo.ozellik.blokajKodu;
                            sicilNoOzellik.geldigiSube = sicilNo.ozellik.geldigiSube;
                            sicilNoOzellik.amortismanYil = sicilNo.ozellik.amortismanYil;
                            sicilNoOzellik.amortismanBitti = sicilNo.ozellik.amortismanBitti;
                            sicilNoOzellik.bulunduguYer = sicilNo.ozellik.bulunduguYer;

                            sonuc = tabSicilNoOzellik.Kaydet(sicilNoOzellik);

                            if (sonuc.islemSonuc && Arac.MerkezBankasiKullaniyor())
                            {
                                int bisAnaPrSicilNo = OrtakFonksiyonlar.ConvertToInt(sicilNo.ozellik.cesidi, 0);
                                if (bisAnaPrSicilNo > 0)
                                {
                                    List<TNS.TMM.SicilNoOzellik> bisListe = new List<TNS.TMM.SicilNoOzellik>();
                                    TNS.TMM.SicilNoOzellik so = new TNS.TMM.SicilNoOzellik();
                                    so.prSicilno = sicilNo.prSicilNo;
                                    bisListe.Add(so);
                                    sonuc = TMMSicilNoOzellik.BisKaydet(ve, kullanan, sicilNo.yil, sicilNo.muhasebeKod, sicilNo.harcamaBirimKod, sicilNo.ambarKod, "", bisAnaPrSicilNo, bisListe.ToArray(), false);
                                }
                            }

                        }


                        //Devir giriş işleminde - sicilözellik tablosunda bulunan yerleşke bilgisi temizlensin
                        if (sonuc.islemSonuc && merkezBankasiMi && sicilNo.islemTurKod == (int)ENUMIslemTipi.DEVIRGIRIS)
                        {
                            ObjectArray ozellikler = TMMSicilNoOzellik.Listele(ve, kullanan, sicilNo.prSicilNo);
                            if (ozellikler.sonuc.islemSonuc && ozellikler.objeler.Count > 0)
                            {
                                SicilNoOzellik ozellik = (SicilNoOzellik)ozellikler.objeler[0];
                                ozellik.bulunduguYer = (sicilNo.nereyeGitti == "" ? null : sicilNo.nereyeGitti);

                                TABSicilNoOzellik tabSicilNoOzellik = new TABSicilNoOzellik(ve);
                                sonuc = tabSicilNoOzellik.Kaydet(ozellik);
                            }
                        }

                        if (!sonuc.islemSonuc)
                            break;
                    }
                    if (!sonuc.islemSonuc)
                        break;
                }
            }

            if (sonuc.islemSonuc)
            {
                ve.Commit();
            }
            else if (!sonuc.islemSonuc)
            {
                ve.Rollback();
                if (sonuc.hataStr != "")
                    sonuc.hataStr = TasinirMalResI.TMM216 + "<br>" + sonuc.hataStr;
            }

            ve.VeriErisimKapat();
            ve2.VeriErisimKapat();
            return sonuc;
        }

        /// <summary>
        /// Parametre olarak verilen demirbaşların sicil numaralarını iptal eden yordam
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="sicilNolar">Sicil numarası iptal edilecek demirbaşları tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public static Sonuc SicilNoIptalEt(VeriErisim veriErisim, Kullanici kullanan, ObjectArray sicilNolar)
        {
            Sonuc sonuc = new Sonuc();

            if (sicilNolar.objeler.Count == 0)
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = TasinirMalResI.TMM217;
                return sonuc;
            }

            string sonrakiHareketler = "";

            SicilNoHareket fisBilgi = new SicilNoHareket();
            fisBilgi.yil = ((SicilNoHareket)sicilNolar.objeler[0]).yil;
            fisBilgi.muhasebeKod = ((SicilNoHareket)sicilNolar.objeler[0]).muhasebeKod;
            fisBilgi.harcamaBirimKod = ((SicilNoHareket)sicilNolar.objeler[0]).harcamaBirimKod;
            fisBilgi.fisNo = ((SicilNoHareket)sicilNolar.objeler[0]).fisNo;
            fisBilgi.islemTurKod = ((SicilNoHareket)sicilNolar.objeler[0]).islemTurKod;

            //if (!TNS.TMM.Arac.MerkezBankasiKullaniyor()) //Merkez bankasında verildiği yer bilgisi seçilerek ön onay verilirse daha sonra ön onay kaldırma işleminde zimmet kaldırma yapılmadığı için burada sorun çıkıyor. Acil Çözüm için bu kontrol konuldu. H.Ö
            sonrakiHareketler = SonrasindakiHareketler(fisBilgi, false);

            if (sonrakiHareketler != "")
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = "Belgenin onayının kaldırılabilmesi için ilk önce sonrasında yapılan işlemlerin de onayının kaldırılması gereklidir.<br>";
                sonuc.hataStr += "Onayının kaldırılması gereken belge bilgileri;<br>";
                sonuc.hataStr += sonrakiHareketler;
            }

            if (sonuc.islemSonuc)
            {
                VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));
                VeriErisim ve2 = VeriErisim.VeriErisimBul(null, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));
                TABSicilHareket tabSicilNo = new TABSicilHareket(ve);
                TABGenel tabGenel = new TABGenel(ve2);

                ve.BeginTransaction();

                if ((((SicilNoHareket)sicilNolar.objeler[0]).islemTurKod == (int)ENUMIslemTipi.DEVIRGIRIS || ((SicilNoHareket)sicilNolar.objeler[0]).islemTurKod == (int)ENUMIslemTipi.DAGITIMIADEGIRIS) && verisSekli != "Kurum"
                    && (((SicilNoHareket)sicilNolar.objeler[0]).muhasebeKod != ((SicilNoHareket)sicilNolar.objeler[0]).gonMuhasebeKod
                    || ((SicilNoHareket)sicilNolar.objeler[0]).harcamaBirimKod.Replace(".", "") != ((SicilNoHareket)sicilNolar.objeler[0]).gonHarcamaBirimKod.Replace(".", "")))
                {
                    foreach (SicilNoHareket sh in sicilNolar.objeler)
                    {
                        sh.prSicilNo = Genel.PrSicilNoGetir(sh.yil, sh.muhasebeKod, sh.harcamaBirimKod, sh.sicilNo, sh.fiyat);
                        sh.durum = (int)ENUMSicilDurum.IPTAL;
                    }

                    foreach (SicilNoHareket sh in sicilNolar.objeler)
                    {
                        int yeniPrSicilNo = OrtakFonksiyonlar.ConvertToInt(tabGenel.BelgeNoAl("PRSICIL", "00000000"), 0);


                        sonuc = tabSicilNo.DevirBelgesiIptalEt(sh, yeniPrSicilNo);
                        if (!sonuc.islemSonuc)
                            break;
                    }
                }
                else
                {
                    ((SicilNoHareket)sicilNolar.objeler[0]).durum = (int)ENUMSicilDurum.IPTAL;
                    sonuc = tabSicilNo.GirisBelgesiIptalEt((SicilNoHareket)sicilNolar.objeler[0]);
                }

                if (sonuc.islemSonuc)
                    ve.Commit();
                else
                {
                    ve.Rollback();
                    sonuc.hataStr = "Belgenin onayı kaldırılamadı.<br>" + sonuc.hataStr;
                }

                ve.VeriErisimKapat();
                ve2.VeriErisimKapat();
            }

            return sonuc;
        }

        /// <summary>
        /// Parametre olarak verilen taşınır işlem fişindeki veya zimmet fişindeki
        /// malzemelerin ilgili belgeden sonra işlem görüp görmediğini kontrol eden yordam
        /// </summary>
        /// <param name="sicilNo">Taşınır işlem fişi veya zimmet fişi bilgilerini tutan nesne</param>
        /// <returns>İlgili belgeden sonra malzemelerden herhangi birinde işlem varsa işlemlere ilişkin bilgi mesajı döndürülür.</returns>
        public static string SonrasindakiHareketler(SicilNoHareket sicilNo, bool zimmetFisNoDondur)
        {
            string donecekDeger = "";

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(null));
            TABSicilHareket tabSicilNo = new TABSicilHareket(ve);

            IDataReader r = tabSicilNo.PrSicilNoveSiraNoVerFisteki(sicilNo);
            int[] prSicilNo = new int[100000];
            int[] siraNo = new int[100000];
            int sSayac = 0;

            while (r.Read())
            {
                prSicilNo[sSayac] = VeriErisim.DegerAl(r, 0, 0);
                siraNo[sSayac] = VeriErisim.DegerAl(r, 1, 0);
                sSayac++;
            }
            r.Close();

            int basSayac = 0;
            int bitSayac = 400;
            int arttirSayac = 400;
            int sonSayac = prSicilNo.Length / arttirSayac;

            for (int i = basSayac; i < sonSayac; i++)
            {
                if (prSicilNo[basSayac] == 0)
                    break;

                r = tabSicilNo.SonrasindakiHareketler(prSicilNo, siraNo, sicilNo.muhasebeKod, sicilNo.harcamaBirimKod, basSayac, bitSayac);
                basSayac = bitSayac;
                bitSayac = bitSayac + arttirSayac;

                while (r.Read())
                {
                    int yil = VeriErisim.DegerAl(r, 2, 0);
                    string muhasebe = VeriErisim.DegerAl(r, 3, "");
                    string hb = VeriErisim.DegerAl(r, 4, "");
                    string ambar = VeriErisim.DegerAl(r, 10, "");
                    string belgeNo = VeriErisim.DegerAl(r, 5, "");
                    int islemTuru = VeriErisim.DegerAl(r, 14, 0);
                    string islem = "";

                    if (islemTuru == (int)ENUMIslemTipi.ZFDUSME ||
                        islemTuru == (int)ENUMIslemTipi.ZFVERME)
                        islem = TasinirMalResI.TMM064;
                    else if (islemTuru == (int)ENUMIslemTipi.DTLDUSME ||
                        islemTuru == (int)ENUMIslemTipi.DTLVERME)
                        islem = TasinirMalResI.TMM067;
                    else
                        islem = TasinirMalResI.TMM063;

                    string geciciBilgi = "";

                    if (!zimmetFisNoDondur)
                        geciciBilgi = string.Format(TasinirMalResI.TMM283, islem, yil, muhasebe, hb, ambar, belgeNo) + "<br>";
                    else
                        geciciBilgi = belgeNo;

                    if (donecekDeger.IndexOf(geciciBilgi) < 0)
                    {
                        if (donecekDeger != "" && zimmetFisNoDondur)
                            donecekDeger += "," + geciciBilgi;
                        else
                            donecekDeger += geciciBilgi;
                    }
                }
                r.Close();
            }

            ve.VeriErisimKapat();

            return donecekDeger;
        }

        /// <summary>
        /// Parametre olarak verilen demirbaşlara ait yeni hareket kaydeden yordam
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="sicilNolar">Hareket eklenecek demirbaşları tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public static Sonuc HareketKaydet(VeriErisim veriErisim, Kullanici kullanan, ObjectArray sicilNolar)
        {
            Sonuc sonuc = new Sonuc();
            ///////////////////////////////////////////////////////////
            bool muhasebeKdvMukellefiMi = false;
            TMMMuhasebe tmmMuhasebe = new TMMMuhasebe();
            muhasebeKdvMukellefiMi = tmmMuhasebe.KdvMukellefiMi(((SicilNoHareket)sicilNolar[0]).muhasebeKod);
            ///////////////////////////////////////////////////////////

            if (sicilNolar.objeler.Count == 0)
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = TasinirMalResI.TMM217;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABSicilHareket tabSicilNo = new TABSicilHareket(ve);

            ve.BeginTransaction();

            foreach (SicilNoHareket sicilNo in sicilNolar.objeler)
            {
                sicilNo.hesapPlanKod = sicilNo.hesapPlanKod.Replace(".", "");

                if (sicilNo.yil == 0)
                    sonuc.hataStr += TasinirMalResI.TMM036 + "<br>";
                if (sicilNo.muhasebeKod == string.Empty)
                    sonuc.hataStr += TasinirMalResI.TMM013 + "<br>";
                else if (sicilNo.harcamaBirimKod == string.Empty)
                    sonuc.hataStr += TasinirMalResI.TMM014 + "<br>";
                else if (sicilNo.fisNo == string.Empty)
                    sonuc.hataStr += TasinirMalResI.TMM211 + "<br>";
                else if (sicilNo.hesapPlanKod == string.Empty)
                    sonuc.hataStr += TasinirMalResI.TMM027 + "<br>";

                //Hesap Kod kontrolü
                //********************
                HesapPlaniSatir hs = new HesapPlaniSatir();
                hs.hesapKod = sicilNo.hesapPlanKod;

                TMMHesapPlani hp = new TMMHesapPlani();
                ObjectArray bilgiHp = hp.Listele(kullanan, hs, new Sayfalama());
                if (bilgiHp.ObjeSayisi < 1)
                    sonuc.hataStr += TasinirMalResI.TMM218 + "<br>";
                else if (((HesapPlaniSatir)bilgiHp[0]).seviye < 2)
                    sonuc.hataStr += string.Format(TasinirMalResI.TMM219, hs.hesapKod) + "<br>";
                //********************

                if (sonuc.hataStr != string.Empty)
                {
                    sonuc.islemSonuc = false;
                    break;
                }

                if (TNS.TMM.Arac.MerkezBankasiKullaniyor())
                {
                    var fiyatListe = TMMSicilNoHareket.FiyatGetir(ve, kullanan, sicilNo.prSicilNo);

                    sicilNo.fiyat = fiyatListe["KDVLIBIRIMFIYAT"];
                    sicilNo.degerArtisi = fiyatListe["DEGERARTISI"];
                }

                sicilNo.durum = (int)ENUMSicilDurum.AKTIF;
                sonuc = tabSicilNo.Kaydet(sicilNo, muhasebeKdvMukellefiMi);
                if (!sonuc.islemSonuc)
                    break;
            }

            if (sonuc.islemSonuc)
                ve.Commit();
            else if (!sonuc.islemSonuc)
                ve.Rollback();

            ve.VeriErisimKapat();

            return sonuc;
        }

        /// <summary>
        /// Aktif olarak ambarda olan ve verilen kriterlere uygun olan demirbaşları listeleyen yordam
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="sicilNo">Demirbaş kriter bilgilerini tutan nesne</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında demirbaş listesi döndürülür.</returns>
        public static ObjectArray AmbardakiSicilNolar(VeriErisim veriErisim, Kullanici kullanan, SicilNoHareket sicilNo)
        {
            return AmbardakiSicilNolar(veriErisim, kullanan, sicilNo, 0, 0);
        }

        /// <summary>
        /// Ambardakis the sicil nolar.
        /// </summary>
        /// <param name="veriErisim">The veri erisim.</param>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="sicilNo">The sicil no.</param>
        /// <param name="ilkKayit">The ilk kayit.</param>
        /// <param name="kayitSay">The kayit say.</param>
        /// <returns></returns>
        public static ObjectArray AmbardakiSicilNolar(VeriErisim veriErisim, Kullanici kullanan, SicilNoHareket sicilNo, int ilkKayit, int kayitSay)
        {
            string kodYapisiHP = TNS.TMM.Arac.DegiskenDegerBul(0, "/KODYAPI/KOD13/KODYAPI");

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABSicilHareket tabSicilNo = new TABSicilHareket(ve);

            IDataReader dr = tabSicilNo.AmbardakiSicilNolar(sicilNo);

            ObjectArray bilgi = new ObjectArray();

            if (ilkKayit < 0)
                ilkKayit = 0;
            if (kayitSay < 0)
                kayitSay = 0;

            int sayac = -1;
            while (dr.Read())
            {
                sayac++;
                if (sayac < ilkKayit)
                    continue;
                if (sayac != 0 && kayitSay != 0 && sayac == ilkKayit + kayitSay)
                    break;

                SicilNoHareket sicil = new SicilNoHareket();
                sicil.prSicilNo = VeriErisim.DegerAl(dr, 0, 0);
                sicil.siraNo = VeriErisim.DegerAl(dr, 1, 0);
                sicil.yil = VeriErisim.DegerAl(dr, 2, 0);
                sicil.muhasebeKod = VeriErisim.DegerAl(dr, 3, "");
                sicil.harcamaBirimKod = VeriErisim.DegerAl(dr, 4, "");
                sicil.fisNo = VeriErisim.DegerAl(dr, 5, "");
                sicil.fisSiraNo = VeriErisim.DegerAl(dr, 6, 0);
                sicil.sicilNo = VeriErisim.DegerAl(dr, 7, "");
                sicil.sicilSiraNo = VeriErisim.DegerAl(dr, 8, "");
                sicil.hesapPlanKod = VeriErisim.DegerAl(dr, 9, "");
                sicil.ambarKod = VeriErisim.DegerAl(dr, 10, "");
                sicil.fiyat = (decimal)VeriErisim.DegerAl(dr, 11, 0.0);
                sicil.kdvOran = VeriErisim.DegerAl(dr, 12, 0);
                sicil.islemTipKod = VeriErisim.DegerAl(dr, 13, 0);
                sicil.islemTurKod = VeriErisim.DegerAl(dr, 14, 0);
                sicil.islemTarih = VeriErisim.DegerAl(dr, 15);
                sicil.islemYapan = VeriErisim.DegerAl(dr, 16, "");
                sicil.durum = VeriErisim.DegerAl(dr, 17, 0);
                sicil.fisTarih = VeriErisim.DegerAl(dr, 18);
                sicil.neredenGeldi = VeriErisim.DegerAl(dr, 19, "");
                sicil.nereyeGitti = VeriErisim.DegerAl(dr, 20, "");
                sicil.kimeGitti = VeriErisim.DegerAl(dr, 21, "");
                sicil.zimmetOzellik = VeriErisim.DegerAl(dr, 22, "");
                sicil.kdvliBirimFiyat = (decimal)VeriErisim.DegerAl(dr, 23, 0.0);
                sicil.degerArtisi = (decimal)VeriErisim.DegerAl(dr, 24, 0.0);
                sicil.ozellik.markaKod = VeriErisim.DegerAl(dr, 25, 0);
                sicil.ozellik.modelKod = VeriErisim.DegerAl(dr, 26, 0);
                sicil.ozellik.saseNo = VeriErisim.DegerAl(dr, 27, "");
                sicil.ozellik.motorNo = VeriErisim.DegerAl(dr, 28, "");
                sicil.ozellik.plaka = VeriErisim.DegerAl(dr, 29, "");
                sicil.ozellik.ciltNo = VeriErisim.DegerAl(dr, 30, "");
                sicil.ozellik.dil = VeriErisim.DegerAl(dr, 31, "");
                sicil.ozellik.yazarAdi = VeriErisim.DegerAl(dr, 32, "");
                sicil.ozellik.adi = VeriErisim.DegerAl(dr, 33, "");
                sicil.ozellik.yayinYeri = VeriErisim.DegerAl(dr, 34, "");
                sicil.ozellik.yayinTarihi = VeriErisim.DegerAl(dr, 35, "");
                sicil.ozellik.neredenGeldi = VeriErisim.DegerAl(dr, 36, "");
                sicil.ozellik.boyutlari = VeriErisim.DegerAl(dr, 37, "");
                sicil.ozellik.satirSayisi = VeriErisim.DegerAl(dr, 38, "");
                sicil.ozellik.yaprakSayisi = VeriErisim.DegerAl(dr, 39, "");
                sicil.ozellik.sayfaSayisi = VeriErisim.DegerAl(dr, 40, "");
                sicil.ozellik.ciltTuru = VeriErisim.DegerAl(dr, 41, "");
                sicil.ozellik.cesidi = VeriErisim.DegerAl(dr, 42, "");
                sicil.ozellik.yeriKonusu = VeriErisim.DegerAl(dr, 43, "");
                sicil.ozellik.gelisTarihi = VeriErisim.DegerAl(dr, 44, "");
                sicil.ozellik.neredeBulundu = VeriErisim.DegerAl(dr, 45, "");
                sicil.ozellik.cagi = VeriErisim.DegerAl(dr, 46, "");
                sicil.ozellik.durumuMaddesi = VeriErisim.DegerAl(dr, 47, "");
                sicil.ozellik.onYuz = VeriErisim.DegerAl(dr, 48, "");
                sicil.ozellik.arkaYuz = VeriErisim.DegerAl(dr, 49, "");
                sicil.ozellik.puan = VeriErisim.DegerAl(dr, 50, "");
                sicil.ozellik.fotograf = VeriErisim.DegerAl(dr, 51, "");
                sicil.ozellik.disSicilNo = VeriErisim.DegerAl(dr, 52, "");
                sicil.muhasebeAd = VeriErisim.DegerAl(dr, 53, "");
                sicil.harcamaBirimAd = VeriErisim.DegerAl(dr, 54, "");
                sicil.ambarAd = VeriErisim.DegerAl(dr, 55, "");
                sicil.hesapPlanAd = VeriErisim.DegerAl(dr, 56, "");
                sicil.ozellik.markaAd = VeriErisim.DegerAl(dr, 57, "");
                sicil.ozellik.modelAd = VeriErisim.DegerAl(dr, 58, "");
                sicil.hesapPlanKod = OrtakFonksiyonlar.KodNoktaKoy(kodYapisiHP, sicil.hesapPlanKod);

                try
                {
                    sicil.eSicilNo = VeriErisim.DegerAl(dr, 59, "");
                }
                catch (System.Exception)
                {
                }

                if (Genel.rfIdVarMi)
                    sicil.rfIdNo = Genel.PrSicilNoRFIDDondur(sicil.prSicilNo);

                bilgi.Ekle(sicil);
            }

            dr.Close();

            ve.VeriErisimKapat();

            return bilgi;
        }

        /// <summary>
        /// Ambarda veya zimmette ayırdetmeksizin kriterlere uygun olan bütün demirbaşları listeleyen yordam
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="sicilNo">Demirbaş kriter bilgilerini tutan nesne</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında demirbaş listesi döndürülür.</returns>
        public static ObjectArray AmbardakiButunSicilNolar(VeriErisim veriErisim, Kullanici kullanan, SicilNoHareket sicilNo)
        {
            return AmbardakiButunSicilNolar(veriErisim, kullanan, sicilNo, new Sayfalama());
        }

        /// <summary>
        /// Ambarda veya zimmette ayırdetmeksizin kriterlere uygun olan bütün demirbaşları listeleyen yordam
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="sicilNo">Demirbaş kriter bilgilerini tutan nesne</param>
        /// <param name="sayfa">The ilk kayit.</param>
        /// <returns>
        /// Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında demirbaş listesi döndürülür.
        /// </returns>
        public static ObjectArray AmbardakiButunSicilNolar(VeriErisim veriErisim, Kullanici kullanan, SicilNoHareket sicilNo, Sayfalama sayfa)
        {
            ObjectArray bilgi = new ObjectArray();

            string kodYapisiHP = TNS.TMM.Arac.DegiskenDegerBul(0, "/KODYAPI/KOD13/KODYAPI");

            if (sicilNo.muhasebeKod != "" && sicilNo.harcamaBirimKod != "")
            {
                bilgi.sonuc.islemSonuc = TMMYetkiKontrol.GormeYetkisiVarMi(kullanan, sicilNo.muhasebeKod, sicilNo.harcamaBirimKod, "");
                if (!bilgi.sonuc.islemSonuc)
                {
                    bilgi.sonuc.hataStr = TasinirMalResI.TMM060;
                    return bilgi;
                }
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABSicilHareket tabSicilNo = new TABSicilHareket(ve);

            IDataReader dr = tabSicilNo.AmbardakiButunSicilNolar(sicilNo, sayfa);

            int eskiSicilNo = 0;

            if (dr != null)
            {
                try
                {

                    while (dr.Read())
                    {
                        SicilNoHareket sicil = new SicilNoHareket();
                        sicil.prSicilNo = VeriErisim.DegerAl(dr, 14, 0);
                        sicil.siraNo = VeriErisim.DegerAl(dr, 15, 0);
                        sicil.yil = VeriErisim.DegerAl(dr, 0, 0);
                        sicil.muhasebeKod = VeriErisim.DegerAl(dr, 1, "");
                        sicil.harcamaBirimKod = VeriErisim.DegerAl(dr, 3, "");
                        sicil.harcamaBirimKod = OrtakFonksiyonlar.KodNoktaKoy(Genel.KodYapiDondur(false), sicil.harcamaBirimKod);
                        sicil.fisNo = VeriErisim.DegerAl(dr, 8, "");
                        //sicil.fisSiraNo = VeriErisim.DegerAl(dr, 6, 0);
                        sicil.sicilNo = VeriErisim.DegerAl(dr, 16, "");
                        //sicil.sicilSiraNo = VeriErisim.DegerAl(dr, 8, "");
                        sicil.hesapPlanKod = VeriErisim.DegerAl(dr, 11, "");
                        sicil.ambarKod = VeriErisim.DegerAl(dr, 5, "");
                        //sicil.fiyat = (decimal)VeriErisim.DegerAl(dr, 13, 0.0);
                        //sicil.kdvOran = VeriErisim.DegerAl(dr, 12, 0);
                        sicil.islemTipKod = VeriErisim.DegerAl(dr, 23, 0);
                        sicil.islemTurKod = VeriErisim.DegerAl(dr, 25, 0);
                        //sicil.islemTarih = VeriErisim.DegerAl(dr, 15);
                        //sicil.islemYapan = VeriErisim.DegerAl(dr, 16, "");
                        //sicil.durum = VeriErisim.DegerAl(dr, 17, 0);
                        sicil.fisTarih = VeriErisim.DegerAl(dr, 7);
                        sicil.neredenGeldi = VeriErisim.DegerAl(dr, 9, "");
                        sicil.nereyeGitti = VeriErisim.DegerAl(dr, 10, "");
                        sicil.kimeGitti = VeriErisim.DegerAl(dr, 26, "");
                        sicil.zimmetOzellik = VeriErisim.DegerAl(dr, 27, "");
                        sicil.kdvliBirimFiyat = (decimal)VeriErisim.DegerAl(dr, 13, 0.0);
                        sicil.degerArtisi = (decimal)VeriErisim.DegerAl(dr, 59, 0.0);
                        sicil.ozellik.markaKod = VeriErisim.DegerAl(dr, 31, 0);
                        sicil.ozellik.modelKod = VeriErisim.DegerAl(dr, 32, 0);
                        sicil.ozellik.saseNo = VeriErisim.DegerAl(dr, 33, "");
                        sicil.ozellik.motorNo = VeriErisim.DegerAl(dr, 34, "");
                        sicil.ozellik.plaka = VeriErisim.DegerAl(dr, 35, "");
                        sicil.ozellik.ciltNo = VeriErisim.DegerAl(dr, 36, "");
                        sicil.ozellik.dil = VeriErisim.DegerAl(dr, 37, "");
                        sicil.ozellik.yazarAdi = VeriErisim.DegerAl(dr, 38, "");
                        sicil.ozellik.adi = VeriErisim.DegerAl(dr, 39, "");
                        sicil.ozellik.yayinYeri = VeriErisim.DegerAl(dr, 40, "");
                        sicil.ozellik.yayinTarihi = VeriErisim.DegerAl(dr, 41, "");
                        sicil.ozellik.neredenGeldi = VeriErisim.DegerAl(dr, 42, "");
                        sicil.ozellik.boyutlari = VeriErisim.DegerAl(dr, 43, "");
                        sicil.ozellik.satirSayisi = VeriErisim.DegerAl(dr, 44, "");
                        sicil.ozellik.yaprakSayisi = VeriErisim.DegerAl(dr, 45, "");
                        sicil.ozellik.sayfaSayisi = VeriErisim.DegerAl(dr, 46, "");
                        sicil.ozellik.ciltTuru = VeriErisim.DegerAl(dr, 47, "");
                        sicil.ozellik.cesidi = VeriErisim.DegerAl(dr, 48, "");
                        sicil.ozellik.yeriKonusu = VeriErisim.DegerAl(dr, 49, "");
                        sicil.ozellik.gelisTarihi = VeriErisim.DegerAl(dr, 50, "");
                        sicil.ozellik.neredeBulundu = VeriErisim.DegerAl(dr, 51, "");
                        sicil.ozellik.cagi = VeriErisim.DegerAl(dr, 52, "");
                        sicil.ozellik.durumuMaddesi = VeriErisim.DegerAl(dr, 53, "");
                        sicil.ozellik.onYuz = VeriErisim.DegerAl(dr, 54, "");
                        sicil.ozellik.arkaYuz = VeriErisim.DegerAl(dr, 55, "");
                        sicil.ozellik.fotograf = VeriErisim.DegerAl(dr, 56, "");
                        sicil.ozellik.puan = VeriErisim.DegerAl(dr, 57, "");
                        sicil.ozellik.disSicilNo = VeriErisim.DegerAl(dr, 58, "");
                        sicil.muhasebeAd = VeriErisim.DegerAl(dr, 2, "");
                        sicil.harcamaBirimAd = VeriErisim.DegerAl(dr, 4, "");
                        sicil.ambarAd = VeriErisim.DegerAl(dr, 6, "");
                        sicil.hesapPlanAd = VeriErisim.DegerAl(dr, 12, "");
                        sicil.hesapPlanKod = OrtakFonksiyonlar.KodNoktaKoy(kodYapisiHP, sicil.hesapPlanKod);

                        sicil.ozellik.disSicilNo2 = VeriErisim.DegerAl(dr, 61, "");
                        sicil.ozellik.bisNo = VeriErisim.DegerAl(dr, 62, "");
                        sicil.ozellik.eskiBisNo1 = VeriErisim.DegerAl(dr, 63, "");
                        sicil.ozellik.eskiBisNo2 = VeriErisim.DegerAl(dr, 64, "");
                        sicil.ozellik.rfidEtiketTurKod = VeriErisim.DegerAl(dr, 65, 0);

                        sicil.ozellik.garantiBitisTarihi = VeriErisim.DegerAl(dr, 66);
                        sicil.ozellik.giai = VeriErisim.DegerAl(dr, 67, "");
                        sicil.ozellik.ekNo = VeriErisim.DegerAl(dr, 68, "");
                        sicil.ozellik.hesapTarih = VeriErisim.DegerAl(dr, 69, "");
                        sicil.ozellik.butceKodu = VeriErisim.DegerAl(dr, 70, "");
                        sicil.ozellik.blokajKodu = VeriErisim.DegerAl(dr, 71, "");
                        sicil.ozellik.geldigiSube = VeriErisim.DegerAl(dr, 72, "");
                        sicil.ozellik.amortismanYil = VeriErisim.DegerAl(dr, 73, 0);
                        sicil.ozellik.amortismanBitti = VeriErisim.DegerAl(dr, 74, 0);
                        sicil.ozellik.bulunduguYer = VeriErisim.DegerAl(dr, 75, "");
                        sicil.ozellik.bulunduguYerAd = VeriErisim.DegerAl(dr, 76, "");

                        sicil.ozellik.markaAd = VeriErisim.DegerAl(dr, 77, "");
                        sicil.ozellik.modelAd = VeriErisim.DegerAl(dr, 78, "");

                        sicil.fisSiraNo = VeriErisim.DegerAl(dr, 79, 0);

                        if (Genel.rfIdVarMi)
                            sicil.rfIdNo = Genel.PrSicilNoRFIDDondur(sicil.prSicilNo);


                        if (eskiSicilNo == sicil.prSicilNo)
                        {
                            if (TNS.TMM.Arac.MerkezBankasiKullaniyor())
                            {
                                //Son hareket ile güncelle
                                for (int i = 0; i < bilgi.objeler.Count; i++)
                                {
                                    if (((SicilNoHareket)bilgi.objeler[i]).prSicilNo == sicil.prSicilNo)
                                    {
                                        //Alım tarihini son hareket için değiştir
                                        sicil.fisTarih = ((SicilNoHareket)bilgi.objeler[0]).fisTarih;

                                        bilgi.objeler[i] = sicil;
                                        break;
                                    }
                                }
                            }

                            continue;
                        }

                        eskiSicilNo = sicil.prSicilNo;

                        if (sayfa.kayitSayisi > 0)
                            bilgi.sonuc.kayitSay = VeriErisim.DegerAl(dr, 79, 0);

                        bilgi.Ekle(sicil);
                    }
                }
                catch (System.Exception e)
                {

                    OrtakFonksiyonlar.HataStrYaz(e.Message + " " + e.StackTrace.ToString());
                }

                dr.Close();
            }

            ve.VeriErisimKapat();

            return bilgi;
        }

        /// <summary>
        /// Zimmette olan ve verilen kriterlere uygun olan demirbaşları listeleyen yordam
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="sicilNo">Demirbaş kriter bilgilerini tutan nesne</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında demirbaş listesi döndürülür.</returns>
        public static ObjectArray ZimmetliSicilNolar(VeriErisim veriErisim, Kullanici kullanan, SicilNoHareket sicilNo)
        {
            string kodYapisiHP = TNS.TMM.Arac.DegiskenDegerBul(0, "/KODYAPI/KOD13/KODYAPI");

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABSicilHareket tabSicilNo = new TABSicilHareket(ve);

            IDataReader dr = tabSicilNo.ZimmettekiSicilNolar(sicilNo);

            ObjectArray bilgi = new ObjectArray();

            while (dr.Read())
            {
                SicilNoHareket sicil = new SicilNoHareket();

                sicil.prSicilNo = VeriErisim.DegerAl(dr, 0, 0);
                sicil.siraNo = VeriErisim.DegerAl(dr, 1, 0);
                sicil.yil = VeriErisim.DegerAl(dr, 2, 0);
                sicil.muhasebeKod = VeriErisim.DegerAl(dr, 3, "");
                sicil.harcamaBirimKod = VeriErisim.DegerAl(dr, 4, "");
                sicil.fisNo = VeriErisim.DegerAl(dr, 5, "");
                sicil.fisSiraNo = VeriErisim.DegerAl(dr, 6, 0);
                sicil.sicilNo = VeriErisim.DegerAl(dr, 7, "");
                sicil.sicilSiraNo = VeriErisim.DegerAl(dr, 8, "");
                sicil.hesapPlanKod = VeriErisim.DegerAl(dr, 9, "");
                sicil.ambarKod = VeriErisim.DegerAl(dr, 10, "");
                sicil.fiyat = (decimal)VeriErisim.DegerAl(dr, 11, 0.0);
                sicil.kdvOran = VeriErisim.DegerAl(dr, 12, 0);
                sicil.islemTipKod = VeriErisim.DegerAl(dr, 13, 0);
                sicil.islemTurKod = VeriErisim.DegerAl(dr, 14, 0);
                sicil.islemTarih = VeriErisim.DegerAl(dr, 15);
                sicil.islemYapan = VeriErisim.DegerAl(dr, 16, "");
                sicil.durum = VeriErisim.DegerAl(dr, 17, 0);
                sicil.fisTarih = VeriErisim.DegerAl(dr, 18);
                sicil.neredenGeldi = VeriErisim.DegerAl(dr, 19, "");
                sicil.nereyeGitti = VeriErisim.DegerAl(dr, 20, "");
                sicil.kimeGitti = VeriErisim.DegerAl(dr, 21, "");
                sicil.zimmetOzellik = VeriErisim.DegerAl(dr, 22, "");
                sicil.kdvliBirimFiyat = (decimal)VeriErisim.DegerAl(dr, 23, 0.0);
                sicil.degerArtisi = (decimal)VeriErisim.DegerAl(dr, 24, 0.0);
                sicil.ozellik.markaKod = VeriErisim.DegerAl(dr, 25, 0);
                sicil.ozellik.modelKod = VeriErisim.DegerAl(dr, 26, 0);
                sicil.ozellik.saseNo = VeriErisim.DegerAl(dr, 27, "");
                sicil.ozellik.motorNo = VeriErisim.DegerAl(dr, 28, "");
                sicil.ozellik.plaka = VeriErisim.DegerAl(dr, 29, "");
                sicil.ozellik.ciltNo = VeriErisim.DegerAl(dr, 30, "");
                sicil.ozellik.dil = VeriErisim.DegerAl(dr, 31, "");
                sicil.ozellik.yazarAdi = VeriErisim.DegerAl(dr, 32, "");
                sicil.ozellik.adi = VeriErisim.DegerAl(dr, 33, "");
                sicil.ozellik.yayinYeri = VeriErisim.DegerAl(dr, 34, "");
                sicil.ozellik.yayinTarihi = VeriErisim.DegerAl(dr, 35, "");
                sicil.ozellik.neredenGeldi = VeriErisim.DegerAl(dr, 36, "");
                sicil.ozellik.boyutlari = VeriErisim.DegerAl(dr, 37, "");
                sicil.ozellik.satirSayisi = VeriErisim.DegerAl(dr, 38, "");
                sicil.ozellik.yaprakSayisi = VeriErisim.DegerAl(dr, 39, "");
                sicil.ozellik.sayfaSayisi = VeriErisim.DegerAl(dr, 40, "");
                sicil.ozellik.ciltTuru = VeriErisim.DegerAl(dr, 41, "");
                sicil.ozellik.cesidi = VeriErisim.DegerAl(dr, 42, "");
                sicil.ozellik.yeriKonusu = VeriErisim.DegerAl(dr, 43, "");
                sicil.ozellik.gelisTarihi = VeriErisim.DegerAl(dr, 44, "");
                sicil.ozellik.neredeBulundu = VeriErisim.DegerAl(dr, 45, "");
                sicil.ozellik.cagi = VeriErisim.DegerAl(dr, 46, "");
                sicil.ozellik.durumuMaddesi = VeriErisim.DegerAl(dr, 47, "");
                sicil.ozellik.onYuz = VeriErisim.DegerAl(dr, 48, "");
                sicil.ozellik.arkaYuz = VeriErisim.DegerAl(dr, 49, "");
                sicil.ozellik.puan = VeriErisim.DegerAl(dr, 50, "");
                sicil.ozellik.fotograf = VeriErisim.DegerAl(dr, 51, "");
                sicil.ozellik.disSicilNo = VeriErisim.DegerAl(dr, 52, "");
                sicil.muhasebeAd = VeriErisim.DegerAl(dr, 53, "");
                sicil.harcamaBirimAd = VeriErisim.DegerAl(dr, 54, "");
                sicil.ambarAd = VeriErisim.DegerAl(dr, 55, "");
                sicil.hesapPlanAd = VeriErisim.DegerAl(dr, 56, "");
                sicil.ozellik.markaAd = VeriErisim.DegerAl(dr, 57, "");
                sicil.ozellik.modelAd = VeriErisim.DegerAl(dr, 58, "");
                sicil.kisiAd = VeriErisim.DegerAl(dr, 59, "");
                sicil.odaAd = VeriErisim.DegerAl(dr, 60, "");
                sicil.hesapPlanKod = OrtakFonksiyonlar.KodNoktaKoy(kodYapisiHP, sicil.hesapPlanKod);

                if (Genel.rfIdVarMi)
                    sicil.rfIdNo = Genel.PrSicilNoRFIDDondur(sicil.prSicilNo);

                bilgi.Ekle(sicil);
            }

            dr.Close();

            ve.VeriErisimKapat();

            return bilgi;
        }

        /// <summary>
        /// Ambarda veya zimmette ayırdetmeksizin kriterlere uygun olan bütün demirbaşları listeleyen yordam
        /// Kriterlerde personel kriteri de varsa sadece zimmetleri listeler.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="sicilNo">Demirbaş kriter bilgilerini tutan nesne</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında demirbaş listesi döndürülür.</returns>
        public static ObjectArray BarkodSicilNolar(VeriErisim veriErisim, Kullanici kullanan, SicilNoHareket sicilNo, Sayfalama sayfa)
        {
            ObjectArray bilgi = new ObjectArray();

            string kodYapisiHP = TNS.TMM.Arac.DegiskenDegerBul(0, "/KODYAPI/KOD13/KODYAPI");

            string hata = "";

            var kontrolYapma = TMM.Arac.MerkezBankasiKullaniyor() && !string.IsNullOrWhiteSpace(sicilNo.nereyeGitti);

            if (!kontrolYapma)
            {
                if (sicilNo.prSicilNo == 0 && sicilNo.sicilNo == "" && sicilNo.hesapPlanKod == "" && sicilNo.fisNo == "" && sicilNo.eSicilNo == "" && sicilNo.sicilNoAciklama == "")
                {
                    if (sicilNo.muhasebeKod == "")
                        hata += "Muhasebe Kodu giriniz." + "<br>";
                    if (sicilNo.harcamaBirimKod == "")
                        hata += "Harcama Birim Kodu giriniz." + "<br>";
                    //if (sicilNo.ambarKod == "")
                    //    hata += "Ambar Kodu giriniz." + "<br>";

                    if (hata != "") hata += " veya ";
                    if (hata != "")
                        hata += "Sicil No veya Belge No alanlarından birini giriniz." + "<br>";
                }

                if (sicilNo.muhasebeKod != "" && sicilNo.harcamaBirimKod != "")
                {
                    if (hata == "")
                    {
                        bool ogySonuc = TMMYetkiKontrol.GormeYetkisiVarMiHarcamaBirimi(kullanan, sicilNo.muhasebeKod, sicilNo.harcamaBirimKod);
                        if (!ogySonuc)
                            hata += "Belirtilen muhasebe ve harcama birim kodunda yetkiniz yok." + "<br>";
                    }
                }
            }

            if (hata != "")
            {
                bilgi.sonuc.islemSonuc = false;
                bilgi.sonuc.hataStr = hata;
                return bilgi;
            }



            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));
            ve.sureAsimi = 60 * 2; //2 dakika

            TABSicilHareket tabSicilNo = new TABSicilHareket(ve);

            if (TMM.Arac.MerkezBankasiKullaniyor())
                sicilNo.sorguEskiSicilNoAramaLike = false;

            IDataReader dr = tabSicilNo.BarkodSicilNolar(sicilNo, sayfa);

            if (dr != null)
            {
                while (dr.Read())
                {
                    SicilNoHareket sicil = new SicilNoHareket();
                    sicil.sicilNo = VeriErisim.DegerAl(dr, 0, "");
                    sicil.hesapPlanKod = OrtakFonksiyonlar.KodNoktaKoy(kodYapisiHP, VeriErisim.DegerAl(dr, 1, ""));
                    sicil.hesapPlanAd = VeriErisim.DegerAl(dr, 2, "");
                    sicil.kimeGitti = VeriErisim.DegerAl(dr, 3, "");
                    sicil.odaAd = VeriErisim.DegerAl(dr, 4, "");
                    sicil.ozellik.adi = VeriErisim.DegerAl(dr, 5, "");
                    sicil.ozellik.yazarAdi = VeriErisim.DegerAl(dr, 6, "");
                    sicil.ozellik.disSicilNo = VeriErisim.DegerAl(dr, 7, "");
                    sicil.ozellik.yayinTarihi = VeriErisim.DegerAl(dr, 8, "");
                    sicil.ozellik.ciltNo = VeriErisim.DegerAl(dr, 9, "");
                    sicil.ozellik.yeriKonusu = VeriErisim.DegerAl(dr, 10, "");
                    sicil.prSicilNo = VeriErisim.DegerAl(dr, 11, 0);
                    sicil.ozellik.saseNo = VeriErisim.DegerAl(dr, 12, "");

                    sicil.muhasebeKod = VeriErisim.DegerAl(dr, 13, "");
                    sicil.harcamaBirimKod = VeriErisim.DegerAl(dr, 14, "");
                    sicil.ambarKod = VeriErisim.DegerAl(dr, 15, "");
                    sicil.kisiAd = VeriErisim.DegerAl(dr, 16, "");

                    if (sicil.odaAd != "")
                        sicil.odaKod = VeriErisim.DegerAl(dr, 17, "");

                    sicil.epc = VeriErisim.DegerAl(dr, 18, "");

                    sicil.eSicilNo = VeriErisim.DegerAl(dr, 7, "");
                    sicil.kdvOran = VeriErisim.DegerAl(dr, 19, 0);
                    sicil.kdvliBirimFiyat = VeriErisim.DegerAl(dr, 20, (decimal)0);
                    sicil.fiyat = VeriErisim.DegerAl(dr, 21, (decimal)0);

                    sicil.muhasebeAd = VeriErisim.DegerAl(dr, 22, "");
                    sicil.harcamaBirimAd = VeriErisim.DegerAl(dr, 23, "");
                    sicil.ambarAd = VeriErisim.DegerAl(dr, 24, "");

                    sicil.ozellik.markaAd = VeriErisim.DegerAl(dr, 25, "");
                    sicil.ozellik.modelAd = VeriErisim.DegerAl(dr, 26, "");

                    sicil.islemTipiAd = VeriErisim.DegerAl(dr, 27, "");

                    sicil.islemDurumAciklama = VeriErisim.DegerAl(dr, 28, "");
                    int islemDurum2 = VeriErisim.DegerAl(dr, 29, 0);
                    if (sicil.islemDurumAciklama != "")
                    {
                        if (islemDurum2 == 0) sicil.islemDurumAciklama += " yapılıyor";
                        if (islemDurum2 == 1) sicil.islemDurumAciklama += " yapıldı";
                    }
                    else
                        sicil.islemDurumAciklama = "Kullanımda";

                    sicil.ozellik.giai = VeriErisim.DegerAl(dr, 30, "");

                    sicil.fisTarih = VeriErisim.DegerAl(dr, 31);
                    sicil.ozellik.bisNo = VeriErisim.DegerAl(dr, 32, "");
                    sicil.ozellik.eskiBisNo1 = VeriErisim.DegerAl(dr, 33, "");
                    sicil.ozellik.ekNo = VeriErisim.DegerAl(dr, 34, "");

                    sicil.ozellik.amortismanYil = VeriErisim.DegerAl(dr, 35, 0);
                    sicil.amortismanYil = VeriErisim.DegerAl(dr, 35, 0);

                    sicil.ozellik.bulunduguYer = VeriErisim.DegerAl(dr, 36, "");
                    sicil.ozellik.bulunduguYerAd = VeriErisim.DegerAl(dr, 37, "");

                    sicil.ozellik.amortismanBitti = VeriErisim.DegerAl(dr, 38, 0);

                    sicil.ozellik.hesapTarih = VeriErisim.DegerAl(dr, 39).ToString();

                    sicil.islemTarih = VeriErisim.DegerAl(dr, 40);

                    sicil.accessNo = VeriErisim.DegerAl(dr, 41, 0);

                    if (sayfa.kayitSayisi > 0)
                        bilgi.sonuc.kayitSay = VeriErisim.DegerAl(dr, 42, 0);

                    bilgi.Ekle(sicil);
                }

                dr.Close();
            }
            else if (ve.hata != "")
                bilgi.sonuc.DegerAta(ve.hata);

            ve.VeriErisimKapat();

            return bilgi;
        }

        /// <summary>
        /// Parametre olarak verilen kriterleri kullanarak ilgili demirbaşa ait hareketleri bulan yordam
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="sicilNo">Demirbaş kriter bilgilerini tutan nesne</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında demirbaşa ait hareket bilgileri listesi döndürülür.</returns>
        public static ObjectArray SicilNoTarihce(VeriErisim veriErisim, Kullanici kullanan, SicilNoHareket sicilNo)
        {
            ObjectArray bilgi = new ObjectArray();

            string kodYapisiHP = TNS.TMM.Arac.DegiskenDegerBul(0, "/KODYAPI/KOD13/KODYAPI");
            string kodYapisi = TNS.TMM.Arac.DegiskenDegerBul(0, "/KODYAPI/KOD10/KODYAPI");

            if (verisSekli != "Kurum" && sicilNo.rfIdNo <= 0 && sicilNo.prSicilNo <= 0)
            {
                if (sicilNo.muhasebeKod.Trim() == "")
                    bilgi.sonuc.hataStr = TasinirMalResI.TMM013 + "<br>";

                if (sicilNo.harcamaBirimKod.Trim() == "")
                    bilgi.sonuc.hataStr += TasinirMalResI.TMM014 + "<br>";
            }

            if (sicilNo.sicilNo.Trim() == "" && sicilNo.eSicilNo.Trim() == "")
                bilgi.sonuc.hataStr += TasinirMalResI.TMM020 + "<br>";

            if (bilgi.sonuc.hataStr != "")
            {
                bilgi.sonuc.islemSonuc = false;
                return bilgi;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABSicilHareket tabSicilNo = new TABSicilHareket(ve);

            SicilNoHareket sicilH = new SicilNoHareket();
            if (sicilNo.prSicilNo <= 0)
            {
                if (sicilNo.rfIdNo <= 0)
                    sicilH.prSicilNo = tabSicilNo.ProgramSicilNoBul(sicilNo);
                else
                    sicilH.prSicilNo = Genel.RFIDPrSicilNoDondur(sicilNo.rfIdNo);
            }
            else
                sicilH.prSicilNo = sicilNo.prSicilNo;

            if (sicilNo.siraNo > 0)
                sicilH.siraNo = sicilNo.siraNo;

            IDataReader dr = tabSicilNo.Listele(sicilH);

            while (dr.Read())
            {
                SicilNoHareket sicil = new SicilNoHareket();

                sicil.prSicilNo = VeriErisim.DegerAl(dr, 0, 0);
                sicil.siraNo = VeriErisim.DegerAl(dr, 1, 0);
                sicil.yil = VeriErisim.DegerAl(dr, 2, 0);
                sicil.muhasebeKod = VeriErisim.DegerAl(dr, 3, "");
                sicil.muhasebeAd = TMMServis.UzayYaziDegerGetir("TASMUHASEBE", sicil.muhasebeKod, true, string.Empty);
                sicil.harcamaBirimKod = OrtakFonksiyonlar.KodNoktaKoy(kodYapisi, VeriErisim.DegerAl(dr, 4, ""));
                sicil.harcamaBirimAd = TMMServis.UzayYaziDegerGetir("TASHARCAMABIRIM", sicil.muhasebeKod + "-" + sicil.harcamaBirimKod, true, string.Empty);
                sicil.fisNo = VeriErisim.DegerAl(dr, 5, "");
                sicil.fisSiraNo = VeriErisim.DegerAl(dr, 6, 0);
                sicil.sicilNo = VeriErisim.DegerAl(dr, 7, "");
                sicil.sicilSiraNo = VeriErisim.DegerAl(dr, 8, "");
                sicil.hesapPlanKod = VeriErisim.DegerAl(dr, 9, "");
                sicil.hesapPlanKod = OrtakFonksiyonlar.KodNoktaKoy(kodYapisiHP, sicil.hesapPlanKod);
                sicil.hesapPlanAd = TMMServis.UzayYaziDegerGetir("TASHESAPPLAN", sicil.hesapPlanKod.Replace(".", ""), true, "");
                sicil.ambarKod = VeriErisim.DegerAl(dr, 10, "");
                sicil.ambarAd = TMMServis.UzayYaziDegerGetir("TASAMBAR", sicil.muhasebeKod + "-" + sicil.harcamaBirimKod + "-" + sicil.ambarKod, true, string.Empty);
                sicil.fiyat = (decimal)VeriErisim.DegerAl(dr, 11, 0.0);
                sicil.kdvOran = VeriErisim.DegerAl(dr, 12, 0);
                sicil.islemTipKod = VeriErisim.DegerAl(dr, 13, 0);
                sicil.islemTurKod = VeriErisim.DegerAl(dr, 14, 0);
                sicil.islemTarih = VeriErisim.DegerAl(dr, 15);
                sicil.islemYapan = VeriErisim.DegerAl(dr, 16, "");
                sicil.durum = VeriErisim.DegerAl(dr, 17, 0);
                sicil.fisTarih = VeriErisim.DegerAl(dr, 18);
                sicil.neredenGeldi = VeriErisim.DegerAl(dr, 19, "");
                sicil.nereyeGitti = VeriErisim.DegerAl(dr, 20, "");
                sicil.kimeGitti = VeriErisim.DegerAl(dr, 21, "");
                sicil.zimmetOzellik = VeriErisim.DegerAl(dr, 22, "");
                sicil.kdvliBirimFiyat = (decimal)VeriErisim.DegerAl(dr, 23, 0.0);
                sicil.degerArtisi = (decimal)VeriErisim.DegerAl(dr, 24, 0.0);
                sicil.gonMuhasebeKod = VeriErisim.DegerAl(dr, 25, "");
                sicil.gonHarcamaBirimKod = VeriErisim.DegerAl(dr, 26, "");
                sicil.gonAmbarKod = VeriErisim.DegerAl(dr, 27, "");
                sicil.islemTipiAd = VeriErisim.DegerAl(dr, 28, "");
                sicil.eSicilNo = VeriErisim.DegerAl(dr, 29, "");
                sicil.odaAd = VeriErisim.DegerAl(dr, 30, "");
                sicil.fiyat += sicil.degerArtisi;

                if (Genel.rfIdVarMi)
                    sicil.rfIdNo = Genel.PrSicilNoRFIDDondur(sicil.prSicilNo);

                bilgi.Ekle(sicil);
            }

            dr.Close();

            ve.VeriErisimKapat();

            return bilgi;
        }

        public static ObjectArray BisSicilNolar(VeriErisim veriErisim, Kullanici kullanan, SicilNoHareket sicilNo)
        {
            string kodYapisiHP = TNS.TMM.Arac.DegiskenDegerBul(0, "/KODYAPI/KOD13/KODYAPI");

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABSicilHareket tabSicilNo = new TABSicilHareket(ve);

            IDataReader dr = tabSicilNo.BisSicilNoListele(sicilNo);

            ObjectArray bilgi = new ObjectArray();

            int sonSira = (int)TABSicilHareket.ENUMAlanlar.GONAMBARKOD;
            while (dr.Read())
            {
                SicilNoHareket sicil = new SicilNoHareket();
                sicil.prSicilNo = VeriErisim.DegerAl(dr, (int)TABSicilHareket.ENUMAlanlar.PRSICILNO, 0);
                sicil.sicilNo = VeriErisim.DegerAl(dr, (int)TABSicilHareket.ENUMAlanlar.SICILNO, "");
                sicil.hesapPlanKod = OrtakFonksiyonlar.KodNoktaKoy(kodYapisiHP, VeriErisim.DegerAl(dr, (int)TABSicilHareket.ENUMAlanlar.HESAPPLANKOD, ""));
                sicil.muhasebeKod = VeriErisim.DegerAl(dr, (int)TABSicilHareket.ENUMAlanlar.MUHASEBEKOD, "");
                sicil.harcamaBirimKod = VeriErisim.DegerAl(dr, (int)TABSicilHareket.ENUMAlanlar.HARCAMABIRIMKOD, "");
                sicil.ambarKod = VeriErisim.DegerAl(dr, (int)TABSicilHareket.ENUMAlanlar.AMBARKOD, "");
                sicil.hesapPlanAd = VeriErisim.DegerAl(dr, sonSira + 1, "");

                bilgi.Ekle(sicil);
            }

            dr.Close();

            ve.VeriErisimKapat();

            return bilgi;
        }

        public static ObjectArray DosyaSicilNoListele(VeriErisim veriErisim, Kullanici kullanan, SicilNoHareket sicilNo)
        {
            ObjectArray bilgi = new ObjectArray();
            string kodYapisiHP = TNS.TMM.Arac.DegiskenDegerBul(0, "/KODYAPI/KOD13/KODYAPI");
            string kodYapisi = TNS.TMM.Arac.DegiskenDegerBul(0, "/KODYAPI/KOD10/KODYAPI");

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABSicilHareket tabSicilNo = new TABSicilHareket(ve);

            IDataReader dr = tabSicilNo.Listele(sicilNo);

            while (dr.Read())
            {
                SicilNoHareket sicil = new SicilNoHareket();

                sicil.prSicilNo = VeriErisim.DegerAl(dr, 0, 0);
                sicil.siraNo = VeriErisim.DegerAl(dr, 1, 0);
                sicil.yil = VeriErisim.DegerAl(dr, 2, 0);
                sicil.muhasebeKod = VeriErisim.DegerAl(dr, 3, "");
                sicil.muhasebeAd = TMMServis.UzayYaziDegerGetir("TASMUHASEBE", sicil.muhasebeKod, true, string.Empty);
                sicil.harcamaBirimKod = OrtakFonksiyonlar.KodNoktaKoy(kodYapisi, VeriErisim.DegerAl(dr, 4, ""));
                sicil.harcamaBirimAd = TMMServis.UzayYaziDegerGetir("TASHARCAMABIRIM", sicil.muhasebeKod + "-" + sicil.harcamaBirimKod, true, string.Empty);
                sicil.fisNo = VeriErisim.DegerAl(dr, 5, "");
                sicil.fisSiraNo = VeriErisim.DegerAl(dr, 6, 0);
                sicil.sicilNo = VeriErisim.DegerAl(dr, 7, "");
                sicil.sicilSiraNo = VeriErisim.DegerAl(dr, 8, "");
                sicil.hesapPlanKod = VeriErisim.DegerAl(dr, 9, "");
                sicil.hesapPlanKod = OrtakFonksiyonlar.KodNoktaKoy(kodYapisiHP, sicil.hesapPlanKod);
                sicil.hesapPlanAd = TMMServis.UzayYaziDegerGetir("TASHESAPPLAN", sicil.hesapPlanKod.Replace(".", ""), true, "");
                sicil.ambarKod = VeriErisim.DegerAl(dr, 10, "");
                sicil.ambarAd = TMMServis.UzayYaziDegerGetir("TASAMBAR", sicil.muhasebeKod + "-" + sicil.harcamaBirimKod + "-" + sicil.ambarKod, true, string.Empty);
                sicil.fiyat = (decimal)VeriErisim.DegerAl(dr, 11, 0.0);
                sicil.kdvOran = VeriErisim.DegerAl(dr, 12, 0);
                sicil.islemTipKod = VeriErisim.DegerAl(dr, 13, 0);
                sicil.islemTurKod = VeriErisim.DegerAl(dr, 14, 0);
                sicil.islemTarih = VeriErisim.DegerAl(dr, 15);
                sicil.islemYapan = VeriErisim.DegerAl(dr, 16, "");
                sicil.durum = VeriErisim.DegerAl(dr, 17, 0);
                sicil.fisTarih = VeriErisim.DegerAl(dr, 18);
                sicil.neredenGeldi = VeriErisim.DegerAl(dr, 19, "");
                sicil.nereyeGitti = VeriErisim.DegerAl(dr, 20, "");
                sicil.kimeGitti = VeriErisim.DegerAl(dr, 21, "");
                sicil.zimmetOzellik = VeriErisim.DegerAl(dr, 22, "");
                sicil.kdvliBirimFiyat = (decimal)VeriErisim.DegerAl(dr, 23, 0.0);
                sicil.degerArtisi = (decimal)VeriErisim.DegerAl(dr, 24, 0.0);
                sicil.gonMuhasebeKod = VeriErisim.DegerAl(dr, 25, "");
                sicil.gonHarcamaBirimKod = VeriErisim.DegerAl(dr, 26, "");
                sicil.gonAmbarKod = VeriErisim.DegerAl(dr, 27, "");
                sicil.islemTipiAd = VeriErisim.DegerAl(dr, 28, "");
                sicil.eSicilNo = VeriErisim.DegerAl(dr, 29, "");
                sicil.odaAd = VeriErisim.DegerAl(dr, 30, "");
                sicil.fiyat += sicil.degerArtisi;

                bilgi.Ekle(sicil);
            }

            dr.Close();

            ve.VeriErisimKapat();

            return bilgi;
        }

        public static ObjectArray IliskiliMalzemeListele(VeriErisim veriErisim, Kullanici kullanan, int[] prSicilNoListe, bool sadeceSatirSayisi = false)
        {
            string kodYapisiHP = TNS.TMM.Arac.DegiskenDegerBul(0, "/KODYAPI/KOD13/KODYAPI");

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABSicilHareket tabSicilNo = new TABSicilHareket(ve);

            IDataReader dr = tabSicilNo.IliskiliMalzemeListele(prSicilNoListe, sadeceSatirSayisi);

            ObjectArray bilgi = new ObjectArray();

            int sonSira = (int)TABSicilHareket.ENUMAlanlar.GONAMBARKOD;
            while (dr.Read())
            {
                if (!sadeceSatirSayisi)
                {
                    SicilNoHareket sicil = new SicilNoHareket();
                    sicil.prSicilNo = VeriErisim.DegerAl(dr, (int)TABSicilHareket.ENUMAlanlar.PRSICILNO, 0);
                    sicil.sicilNo = VeriErisim.DegerAl(dr, (int)TABSicilHareket.ENUMAlanlar.SICILNO, "");
                    sicil.hesapPlanKod = OrtakFonksiyonlar.KodNoktaKoy(kodYapisiHP, VeriErisim.DegerAl(dr, (int)TABSicilHareket.ENUMAlanlar.HESAPPLANKOD, ""));
                    sicil.muhasebeKod = VeriErisim.DegerAl(dr, (int)TABSicilHareket.ENUMAlanlar.MUHASEBEKOD, "");
                    sicil.harcamaBirimKod = VeriErisim.DegerAl(dr, (int)TABSicilHareket.ENUMAlanlar.HARCAMABIRIMKOD, "");
                    sicil.ambarKod = VeriErisim.DegerAl(dr, (int)TABSicilHareket.ENUMAlanlar.AMBARKOD, "");
                    sicil.kdvOran = VeriErisim.DegerAl(dr, (int)TABSicilHareket.ENUMAlanlar.KDVORAN, 0);
                    sicil.fiyat = VeriErisim.DegerAl(dr, (int)TABSicilHareket.ENUMAlanlar.FIYAT, (decimal)0.0);
                    sicil.hesapPlanAd = VeriErisim.DegerAl(dr, sonSira + 1, "");

                    bilgi.Ekle(sicil);
                }
                else
                    bilgi.sonuc.anahtar = VeriErisim.DegerAl(dr, 0, 0) + "";
            }

            dr.Close();

            ve.VeriErisimKapat();

            return bilgi;
        }

        public static int IliskiliMalzemeSayisi(VeriErisim veriErisim, Kullanici kullanan, int[] prSicilNoListe)
        {
            ObjectArray bilgi = IliskiliMalzemeListele(veriErisim, kullanan, prSicilNoListe, true);
            if (bilgi.sonuc.islemSonuc)
                return OrtakFonksiyonlar.ConvertToInt(bilgi.sonuc.anahtar, 0);
            return 0;
        }

        public static ObjectArray KontrolSicilNo(VeriErisim veriErisim, Kullanici kullanan, SicilNoHareket sicilNo)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABSicilHareket tabSicilNo = new TABSicilHareket(ve);

            IDataReader dr = tabSicilNo.KontrolSicilNo(sicilNo);

            ObjectArray bilgi = new ObjectArray();

            while (dr.Read())
            {
                SicilNoHareket sicil = new SicilNoHareket();
                sicil.prSicilNo = VeriErisim.DegerAl(dr, 0, 0);
                sicil.muhasebeKod = VeriErisim.DegerAl(dr, 1, "");
                sicil.harcamaBirimKod = VeriErisim.DegerAl(dr, 2, "");
                sicil.sicilNo = VeriErisim.DegerAl(dr, 3, "");

                bilgi.Ekle(sicil);
            }

            dr.Close();

            ve.VeriErisimKapat();

            return bilgi;
        }

        public static Sonuc GuncelleDegerArtis(Kullanici kullanan, SicilNoHareket bilgi)
        {
            Sonuc sonuc = new Sonuc(false, TasinirMalResI.TMM069, "");

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABSicilHareket to = new TABSicilHareket(ve);
            ve.BeginTransaction();

            if (bilgi.prSicilNo > 0)
                sonuc = to.GuncelleDegerArtis(bilgi);

            if (sonuc.islemSonuc)
            {
                sonuc.bilgiStr = TasinirMalResI.TMM118;
                ve.Commit();
            }
            else
            {
                sonuc.hataStr = TasinirMalResI.TMM119 + "<br>" + sonuc.hataStr;
                ve.Rollback();
            }

            ve.VeriErisimKapat();

            return sonuc;
        }

        public static decimal SonBedelGetir(VeriErisim veriErisim, Kullanici kullanan, int prSicilNo)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABSicilHareket tab = new TABSicilHareket(ve);
            decimal sonBedel = tab.SonBedelGetir(prSicilNo);
            ve.VeriErisimKapat();
            return sonBedel;
        }

        public static Dictionary<string, decimal> FiyatGetir(VeriErisim veriErisim, Kullanici kullanan, int prSicilNo)
        {
            Dictionary<string, decimal> fiyatListe = new Dictionary<string, decimal>();
            fiyatListe.Add("KDVLIBIRIMFIYAT", 0);
            fiyatListe.Add("DEGERARTISI", 0);

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABSicilHareket tab = new TABSicilHareket(ve);

            IDataReader dr = tab.FiyatGetir(prSicilNo);

            if (dr != null)
            {
                while (dr.Read())
                {
                    fiyatListe["KDVLIBIRIMFIYAT"] = VeriErisim.DegerAl(dr, 0, (decimal)0);
                    fiyatListe["DEGERARTISI"] = VeriErisim.DegerAl(dr, 1, (decimal)0);
                }

                dr.Close();
            }

            ve.VeriErisimKapat();
            return fiyatListe;
        }

        public static Sonuc KaydetSicilNoNumara(VeriErisim veriErisim, Kullanici kullanan, SicilNoHareket sicilNo)
        {
            Sonuc sonuc = new Sonuc();

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABSicilHareket tabSicilNo = new TABSicilHareket(ve);

            ve.BeginTransaction();

            if (sicilNo.yil == 0)
                sonuc.hataStr += TasinirMalResI.TMM036 + "<br>";
            if (sicilNo.muhasebeKod == string.Empty)
                sonuc.hataStr += TasinirMalResI.TMM013 + "<br>";
            else if (sicilNo.harcamaBirimKod == string.Empty)
                sonuc.hataStr += TasinirMalResI.TMM014 + "<br>";
            else if (sicilNo.fisNo == string.Empty)
                sonuc.hataStr += TasinirMalResI.TMM211 + "<br>";
            else if (sicilNo.ambarKod == string.Empty)
                sonuc.hataStr += TasinirMalResI.TMM031 + "<br>";
            else if (sicilNo.prSicilNo == 0)
                sonuc.hataStr += TasinirMalResI.TMM020 + "<br>";

            if (sonuc.hataStr != string.Empty)
            {
                sonuc.islemSonuc = false;
                return sonuc;
            }

            sonuc = tabSicilNo.KaydetSicilNoNumara(sicilNo);
            if (!sonuc.islemSonuc)
                return sonuc;

            if (sonuc.islemSonuc)
                ve.Commit();
            else if (!sonuc.islemSonuc)
                ve.Rollback();

            ve.VeriErisimKapat();

            return sonuc;
        }

        public static ObjectArray ListeleSicilNoNumara(VeriErisim veriErisim, Kullanici kullanan, SicilNoHareket sicilNo)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABSicilHareket tabSicilNo = new TABSicilHareket(ve);

            IDataReader dr = tabSicilNo.ListeleSicilNoNumara(sicilNo);

            ObjectArray bilgi = new ObjectArray();

            while (dr.Read())
            {
                SicilNoHareket sicil = new SicilNoHareket();
                sicil.prSicilNo = VeriErisim.DegerAl(dr, (int)TABSicilHareket.ENUMAlanlarNumara.PRSICILNO, 0);
                sicil.yil = VeriErisim.DegerAl(dr, (int)TABSicilHareket.ENUMAlanlarNumara.YIL, 0);
                sicil.muhasebeKod = VeriErisim.DegerAl(dr, (int)TABSicilHareket.ENUMAlanlarNumara.MUHASEBEKOD, "");
                sicil.harcamaBirimKod = VeriErisim.DegerAl(dr, (int)TABSicilHareket.ENUMAlanlarNumara.HARCAMABIRIMKOD, "");
                sicil.ambarKod = VeriErisim.DegerAl(dr, (int)TABSicilHareket.ENUMAlanlarNumara.AMBARKOD, "");
                sicil.fisNo = VeriErisim.DegerAl(dr, (int)TABSicilHareket.ENUMAlanlarNumara.FISNO, "");

                bilgi.Ekle(sicil);
            }

            dr.Close();

            ve.VeriErisimKapat();

            return bilgi;
        }

        internal static void TasinirFiyatDuzelt()
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriTMM");
            TABSicilHareket tab = new TABSicilHareket(ve);

            List<string> sqlListe = new List<string>();

            for (int i = 2; i < 20; i++)
            {
                IDataReader dr = tab.TasinirFiyatDuzelt(i);

                while (dr.Read())
                {
                    sqlListe.Add(VeriErisim.DegerAl(dr, 0, ""));
                }
                dr.Close();
            }

            ve.BeginTransaction();
            tab.TasinirFiyatDuzelt(sqlListe);
            ve.Commit();

            ve.VeriErisimKapat();

        }

    }
}
