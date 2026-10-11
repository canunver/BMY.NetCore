using System;
using System.Data;
using System.Collections.Generic;
using System.Text;
using TNS;
using TNS.KYM;
using TNS.Veri;
using TNS.YAT.IsKurallari.Veri;
using TNS.BHMv2;
using TNS.BKD;
using System.Collections;

namespace TNS.YAT.IsKurallari
{
    /// <summary>
    /// Bütçe kaynağın proje ve yıl ödenek teklif bilgileriyle ilgili, iş kuralları kontrollerini yapıp,
    /// veri tabanı bağlantısını alıp, veri tabanı fonksiyonlarını çağırarak
    /// okuma, yazma işlemlerini gerçekleştiren fonksiyonları tutan sınıf.
    /// </summary>
    public class ButceKaynakTeklifClass
    {
        /// <summary>
        /// Bütçe kaynağın proje ve yıl ödenek teklif bilgilerini kaydeden yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen liste içindeki
        /// ButceKaynakTeklif nesnelerinden herhangi biri eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="kaynakNo">Kaydı yapılacak kaynak No</param>
        /// <param name="butceYil">Kaydı yapılacak bütçe yılı</param>
        /// <param name="senaryoKod">Kaydı yapılacak senaryo kodu</param>
        /// <param name="teklif">ButceKaynakTeklif nesnelerinden oluşan listeyi tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public static Sonuc Kaydet(VeriErisim ve, Kullanici kullanan, int kaynakNo, int butceYil, string senaryoKod, TNSCollection teklifler)
        {
            TABYATButceKaynakTeklif tabKT = new TABYATButceKaynakTeklif(ve);
            Sonuc donusDeger = new Sonuc();

            for (int i = 0; i < teklifler.Count; i++)
            {
                ButceKaynakTeklif teklif = (ButceKaynakTeklif)teklifler[i];
                if (teklif != null)
                {
                    if (donusDeger.islemSonuc)
                        donusDeger = tabKT.Sil(teklif.butceYil, kaynakNo, senaryoKod, butceYil - teklif.butceYil);

                    if (butceYil - teklif.butceYil == 0)
                    {
                        for (int j = 0; j < teklif.OVTeklif.Length; j++)
                        {
                            if (!donusDeger.islemSonuc)
                                break;
                            donusDeger = tabKT.Kaydet(butceYil, senaryoKod, kaynakNo, (ENUMProjeMaliYapiKod)j, teklif.OVTeklif[j]);
                        }
                        if (donusDeger.islemSonuc)
                            donusDeger = tabKT.Kaydet(butceYil, senaryoKod, kaynakNo, ENUMProjeMaliYapiKod.kaynakToplam, teklif.kaynakToplam);
                        if (donusDeger.islemSonuc)
                            donusDeger = tabKT.Kaydet(butceYil, senaryoKod, kaynakNo, ENUMProjeMaliYapiKod.sonrakiYillar, teklif.sonrakiYillar);

                    }
                    else if (butceYil - teklif.butceYil == 1)
                    {
                        if (donusDeger.islemSonuc)
                            donusDeger = tabKT.Kaydet(teklif.butceYil, senaryoKod, kaynakNo, ENUMProjeMaliYapiKod.harcamaTahmin, teklif.harcamaTahmin);

                        if (donusDeger.islemSonuc)
                            donusDeger = tabKT.Kaydet(teklif.butceYil, senaryoKod, kaynakNo, ENUMProjeMaliYapiKod.harcama6ay, teklif.harcama6ay);

                        if (donusDeger.islemSonuc)
                            donusDeger = tabKT.Kaydet(teklif.butceYil, senaryoKod, kaynakNo, ENUMProjeMaliYapiKod.harcamaYilSonu, teklif.harcamaYilSonu);
                    }
                    else
                    {
                        if (donusDeger.islemSonuc)
                            donusDeger = tabKT.Kaydet(teklif.butceYil, senaryoKod, kaynakNo, ENUMProjeMaliYapiKod.kumulatifHarcama, teklif.kumulatifHarcama);
                    }
                }
            }

            return donusDeger;
        }

        /// <summary>
        /// Verilen kriterlere uygun olan proje ve yıl ödenek teklif bilgilerini döndüren yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="kaynakNo">Proje ve yıl ödenek teklif bilgileri istenen bütçe kaynağın programatik kodu</param>
        /// <param name="yil">Yıl kriteri</param>
        /// <param name="senaryoKod">Senaryo kriteri</param>
        /// <param name="projeKodu">Yatırım projesinin programatik kodu</param>
        /// <param name="tumYillar">Projenin geçmiş yıl rakamlarını da getirir.</param>
        /// <param name="ilkYil">Eğer verilmişse projenşn verilen yıllardaki rakamları getirilir.</param>
        /// <returns>Hata varsa BilgiVAN içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında ButceKaynakTeklif nesneleri döndürülür.</returns>
        public static BilgiVAN Listele(VeriErisim veriErisim, Kullanici kullanan, int kaynakNo, int yil, string senaryoKod, int projeKodu, bool tekYil, string birimKodu, bool eskiYillarKBO)
        {
            BilgiVAN donusDeger = new BilgiVAN();

            donusDeger.sonuc = Yetki.OkumaHakkiVarMi(kullanan, birimKodu);
            if (!donusDeger.sonuc.islemSonuc)
                return donusDeger;

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriYAT", TNS.KYM.Arac.KimlikBul(kullanan));
            TABYATButceKaynakTeklif tabKT = new TABYATButceKaynakTeklif(ve);
            IDataReader dr = tabKT.Listele(kaynakNo, yil, projeKodu, senaryoKod, Yetki.BirimKodlariDondur(kullanan, null), tekYil, eskiYillarKBO); //, tumYillar, ilkYil

            if (dr != null)
            {
                TeklifListeDoldur(dr, donusDeger);
            }
            else
            {
                donusDeger.sonuc.DegerAta(ve.hata);
                ve.VeriErisimKapat();
                return donusDeger;
            }
            ve.VeriErisimKapat();
            return donusDeger;
        }

        public static BilgiVAN Listele(string okunanXML)
        {
            BilgiVAN donusDeger = new BilgiVAN();

            IDataReader dr = new XMLStringDataReader(okunanXML);

            if (dr != null)
            {
                TeklifListeDoldur(dr, donusDeger);
            }
            else
            {
                donusDeger.sonuc.DegerAta("Okunan veri teklif listesine çevrilemedi.");
                return donusDeger;
            }

            return donusDeger;
        }

        private static void TeklifListeDoldur(IDataReader dr, BilgiVAN donusDeger)
        {
            int oncKaynakNo = -1;
            int oncButceYil = -1;
            ButceKaynakTeklif gelen = null;
            while (dr.Read())
            {
                int simdKaynakNo = VeriErisim.DegerAl(dr, (int)TABYATButceKaynakTeklif.ENUMAlanlar.KAYNAKNO, -1);
                int simdButceYil = VeriErisim.DegerAl(dr, (int)TABYATButceKaynakTeklif.ENUMAlanlar.YIL, -1);

                if (gelen == null || oncKaynakNo != simdKaynakNo || oncButceYil != simdButceYil)
                {
                    oncKaynakNo = simdKaynakNo;
                    oncButceYil = simdButceYil;
                    gelen = new ButceKaynakTeklif();
                    gelen.kaynakNo = simdKaynakNo;
                    donusDeger.liste.Add(gelen);
                }
                //gelen.kaynakNo = VeriErisim.DegerAl(dr, 0, -1);
                gelen.butceYil = simdButceYil;

                ENUMProjeMaliYapiKod indeks = (ENUMProjeMaliYapiKod)VeriErisim.DegerAl(dr, (int)TABYATButceKaynakTeklif.ENUMAlanlar.TEKLIFYIL, -10);

                double kaynakToplam = VeriErisim.DegerAl(dr, (int)TABYATButceKaynakTeklif.ENUMAlanlar.PROJEBUTCE, (double)0);
                double fin7 = VeriErisim.DegerAl(dr, (int)TABYATButceKaynakTeklif.ENUMAlanlar.YILBUTCEDIS, (double)0);
                double disOzkaynak = VeriErisim.DegerAl(dr, (int)TABYATButceKaynakTeklif.ENUMAlanlar.PROJEBUTCEDISAGIDEN, (double)0);

                gelen.DegerAta(indeks, fin7, kaynakToplam, disOzkaynak);
            }
            dr.Close();
        }

        /// <summary>
        /// Bütçe kaynağın proje ve yıl ödenek bilgilerini silen yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen liste içindeki
        /// ButceKaynakTeklif nesnelerinden herhangi biri eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="kaynakNo">Silinecek kaynağın kodu</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public static Sonuc Sil(VeriErisim veriErisim, Kullanici kullanan, int yil, string senaryoKod, int altProjeNo, int kaynakNo)
        {
            Sonuc donusDeger = Yetki.YazmaHakkiVarMi(kullanan);
            if (!donusDeger.islemSonuc)
                return donusDeger;

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriYAT", TNS.KYM.Arac.KimlikBul(kullanan));
            TABYATButceKaynakTeklif tabTeklif = new TABYATButceKaynakTeklif(ve);
            ve.BeginTransaction();

            if (kaynakNo <= 0)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = YatirimResI.YAT023;
            }
            if (!Genel.ButceKaynakVarMi(ve, yil, senaryoKod, kaynakNo))
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = YatirimResI.YAT024;
            }

            if (donusDeger.islemSonuc)
                donusDeger = tabTeklif.KaynakSil(yil, senaryoKod, kaynakNo);

            if (donusDeger.islemSonuc)
                ve.Commit();
            else
                ve.Rollback();

            ve.VeriErisimKapat();
            return donusDeger;
        }

        /// <summary>
        /// Alt faaliyet-yatırım bağı üzerinden bütçe hazırlık ödenek tekliflerini bulup,
        /// bütçe kaynak bazında ödenek toplamlarını hesaplayan ve kaydeden yordam
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public static Sonuc OdenekAktar(Kullanici kullanan)
        {
            return OdenekAktar(kullanan, Genel.HazirlikYilBul());
        }

        /// <summary>
        /// Alt faaliyet-yatırım bağı üzerinden bütçe hazırlık ödenek tekliflerini bulup,
        /// bütçe kaynak bazında ödenek toplamlarını hesaplayan ve kaydeden yordam
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="yil">Yıl kriteri</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public static Sonuc OdenekAktar(Kullanici kullanan, int yil)
        {
            Sonuc s = new Sonuc();
            ButceSenaryo bs = new ButceSenaryo();
            bs.kod = "";
            IBHMv2Servis servisBHMv2 = TNS.BHMv2.Arac.Tanimla();
            ButceSenaryo[] bsler = servisBHMv2.ButceSenaryoListele(kullanan, bs);

            if (bsler != null)
            {
                foreach (ButceSenaryo b in bsler)
                {
                    s = OdenekAktar(kullanan, yil, b.kod);
                    if (!s.islemSonuc)
                        break;
                }
            }
            else
                s.DegerAta("Aşamalar bulunamadı!");
            return s;
        }

        /// <summary>
        /// Alt faaliyet-yatırım bağı üzerinden bütçe hazırlık ödenek tekliflerini bulup,
        /// bütçe kaynak bazında ödenek toplamlarını hesaplayan ve kaydeden yordam
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="yil">Yıl kriteri</param>
        /// <param name="senaryoKod">Senaryo kriteri</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public static Sonuc OdenekAktar(Kullanici kullanan, int yil, string senaryoKod, int kaynakNo = 0)
        {
            Sonuc donusDeger = new Sonuc();
            int butceHazYil = OrtakFonksiyonlar.ConvertToInt(TNS.DEG.Arac.DegiskenDegerBul(0, "/BUTCEHAZIRLIK"), 0);
            if (butceHazYil != yil)
            {
                if (kaynakNo == 0)
                {
                    if (kullanan.kullaniciKodu != "stratek")
                    {
                        donusDeger.DegerAta("Toplu hesaplama işlemi hazırlık yılından önceki yıllar için yalnızca yetkili kullanıcı tarafından yapılabilir!");
                        return donusDeger;
                    }
                }
            }
            TNS.BHMv2.IBHMv2Servis servis = TNS.BHMv2.Arac.Tanimla();
            if (ProjeClass.KapaliMi(servis, kullanan, yil, senaryoKod))
            {
                donusDeger.DegerAta("Veri giriş dönemi kapatıldığından kayıt işlemi yapamazsınız!");
                return donusDeger;
            }
            string kurumKod = TNS.DEG.Arac.DegiskenDegerBul(yil, "/KURUMKOD");
            int entegreYil = Genel.YatirimEntegreYilBul();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriYAT", TNS.KYM.Arac.KimlikBul(kullanan));
            TABYATButceKaynakTeklif tabTeklif = new TABYATButceKaynakTeklif(ve);
            ve.BeginTransaction();
            if (donusDeger.islemSonuc)
                donusDeger = tabTeklif.EksikSatirlariDoldur(yil, yil - 1, senaryoKod, ENUMProjeMaliYapiKod.odenekDusulen, kaynakNo);
            if (donusDeger.islemSonuc)
                donusDeger = tabTeklif.EksikSatirlariDoldur(yil, yil - 1, senaryoKod, ENUMProjeMaliYapiKod.odenekEklenen, kaynakNo);
            if (donusDeger.islemSonuc)
                donusDeger = tabTeklif.EksikSatirlariDoldur(yil, yil - 1, senaryoKod, ENUMProjeMaliYapiKod.harcamaYilSonu, kaynakNo);
            if (donusDeger.islemSonuc)
                donusDeger = tabTeklif.EksikSatirlariDoldur(yil, yil - 1, senaryoKod, ENUMProjeMaliYapiKod.harcamaTahmin, kaynakNo);
            if (donusDeger.islemSonuc)
                donusDeger = tabTeklif.EksikSatirlariDoldur(yil, yil - 1, senaryoKod, ENUMProjeMaliYapiKod.harcama6ay, kaynakNo);
            if (donusDeger.islemSonuc)
                donusDeger = tabTeklif.EksikSatirlariDoldur(yil, yil - 1, senaryoKod, ENUMProjeMaliYapiKod.kumulatifHarcama, kaynakNo);
            if (donusDeger.islemSonuc)
                donusDeger = tabTeklif.EksikSatirlariDoldur(yil, yil, senaryoKod, ENUMProjeMaliYapiKod.teklif0, kaynakNo);
            if (donusDeger.islemSonuc)
                donusDeger = tabTeklif.EksikSatirlariDoldur(yil, yil, senaryoKod, ENUMProjeMaliYapiKod.teklif1, kaynakNo);
            if (donusDeger.islemSonuc)
                donusDeger = tabTeklif.EksikSatirlariDoldur(yil, yil, senaryoKod, ENUMProjeMaliYapiKod.teklif2, kaynakNo);

            if (donusDeger.islemSonuc)
                donusDeger = tabTeklif.EksikSatirlariDoldur(yil, yil, senaryoKod, ENUMProjeMaliYapiKod.ekteklif0, kaynakNo);
            if (donusDeger.islemSonuc)
                donusDeger = tabTeklif.EksikSatirlariDoldur(yil, yil, senaryoKod, ENUMProjeMaliYapiKod.ekteklif1, kaynakNo);
            if (donusDeger.islemSonuc)
                donusDeger = tabTeklif.EksikSatirlariDoldur(yil, yil, senaryoKod, ENUMProjeMaliYapiKod.ekteklif2, kaynakNo);
            if (donusDeger.islemSonuc)
                donusDeger = tabTeklif.EksikSatirlariDoldur(yil, yil, senaryoKod, ENUMProjeMaliYapiKod.kaynakToplam, kaynakNo);


            if (donusDeger.islemSonuc)
                ve.Commit();
            else
            {
                ve.Rollback();
                return donusDeger;
            }

            int yer = 1;
            if (yil >= entegreYil)
            {
                donusDeger = tabTeklif.OdenekKopyala(yil, senaryoKod, ENUMProjeMaliYapiKod.teklif0, kaynakNo);
                if (donusDeger.islemSonuc)
                {
                    yer++;
                    donusDeger = tabTeklif.OdenekKopyala(yil, senaryoKod, ENUMProjeMaliYapiKod.teklif1, kaynakNo);
                }
                if (donusDeger.islemSonuc)
                {
                    yer++;
                    donusDeger = tabTeklif.OdenekKopyala(yil, senaryoKod, ENUMProjeMaliYapiKod.teklif2, kaynakNo);
                }

                if (donusDeger.islemSonuc)
                {
                    yer++;
                    donusDeger = tabTeklif.EklenenKopyala(yil - 1, senaryoKod, ENUMProjeMaliYapiKod.odenekEklenen, kaynakNo);
                }

                if (donusDeger.islemSonuc)
                {
                    yer++;
                    donusDeger = tabTeklif.DusulenKopyala(yil - 1, senaryoKod, ENUMProjeMaliYapiKod.odenekDusulen, kaynakNo);
                }

                if (donusDeger.islemSonuc)
                {
                    yer++;
                    donusDeger = tabTeklif.HarcamaKopyala(yil, yil - 1, senaryoKod, ENUMProjeMaliYapiKod.harcamaYilSonu, 12, kaynakNo);
                }

                if (donusDeger.islemSonuc)
                {
                    yer++;
                    donusDeger = tabTeklif.HarcamaTahminKopyala(yil, yil - 1, senaryoKod, ENUMProjeMaliYapiKod.harcamaTahmin, kaynakNo);
                }

                if (donusDeger.islemSonuc)
                {
                    yer++;
                    donusDeger = tabTeklif.HarcamaKopyala(yil, yil - 1, senaryoKod, ENUMProjeMaliYapiKod.harcama6ay, 6, kaynakNo);
                }

                if (donusDeger.islemSonuc)
                {
                    yer++;
                    donusDeger = tabTeklif.OdenekKopyalaEk(yil, senaryoKod, ENUMProjeMaliYapiKod.ekteklif0, kaynakNo);
                }

                if (donusDeger.islemSonuc)
                {
                    yer++;
                    donusDeger = tabTeklif.OdenekKopyalaEk(yil, senaryoKod, ENUMProjeMaliYapiKod.ekteklif1, kaynakNo);
                }

                if (donusDeger.islemSonuc)
                {
                    yer++;
                    donusDeger = tabTeklif.OdenekKopyalaEk(yil, senaryoKod, ENUMProjeMaliYapiKod.ekteklif2, kaynakNo);
                }
            }
            if (donusDeger.islemSonuc)
            {
                yer++;
                donusDeger = tabTeklif.KumulatifHarcamaTahminiOl(yil, yil - 1, senaryoKod, ENUMProjeMaliYapiKod.kumulatifHarcama, kaynakNo);
            }

            if (donusDeger.islemSonuc)
            {
                yer++;
                donusDeger = tabTeklif.ProjeBedeliOlDevamEden(yil, senaryoKod, ENUMProjeMaliYapiKod.kaynakToplam, kaynakNo);
            }

            if (donusDeger.islemSonuc)
            {
                yer++;
                donusDeger = tabTeklif.ProjeBedeliOlEskiYeni(yil, senaryoKod, ENUMProjeMaliYapiKod.kaynakToplam, kaynakNo);
            }

            if (!donusDeger.islemSonuc) donusDeger.hataStr = string.Format("Aktarım işleminde {0}. adımda hata oldu. Oluşan Hata:{1}", yer, donusDeger.hataStr);
            ve.VeriErisimKapat();
            return donusDeger;
        }

        private static string KodListeYap(TNSCollection projeKodlar)
        {
            string retval = "";
            foreach (Sozluk item in projeKodlar)
            {
                if (retval != "") retval += ",";
                retval += item.kod.ToString();
            }
            return retval;
        }

        internal static Sonuc AsamaKopyala(Kullanici kullanan, int kaynakYil, string kaynakAsama, int hedefYil, string hedefAsama, bool[] kopyalanacaklar, TNSCollection projeKodlar)
        {
            string projeKodListe = null;
            if (projeKodlar != null) projeKodListe = KodListeYap(projeKodlar);

            Sonuc bilgi = new Sonuc();
            string kurumKod = TNS.DEG.Arac.DegiskenDegerBul(kaynakYil, "/KURUMKOD");

            if (projeKodlar != null && projeKodlar.Count == 0)
                bilgi.DegerAta("Kopyalanacak projeleri seçiniz!");
            if (string.IsNullOrWhiteSpace(kaynakAsama))
                bilgi.DegerAta("Kaynak aşama boş olamaz");

            if (string.IsNullOrWhiteSpace(hedefAsama))
                bilgi.DegerAta("Hedef aşama boş olamaz");
            if (bilgi.islemSonuc)
                if (kaynakYil == hedefYil && kaynakAsama.Replace(".", "") == hedefAsama.Replace(".", ""))
                {
                    bilgi.DegerAta("Kaynak ve hedef aynı olamaz!");
                }
            if (!bilgi.islemSonuc)
                return bilgi;

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriYAT", TNS.KYM.Arac.KimlikBul(kullanan));

            if (DiziElemanAl(kopyalanacaklar, 0, false)) //Projeler
            {
                TABYATProje tab = new TABYATProje(ve);
                TABYATIlIlce tabIlilce = new TABYATIlIlce(ve);
                TABYATALTPROJEBILGI tabBilgi = new TABYATALTPROJEBILGI(ve);
                //ve.BeginTransaction();
                Sonuc donusDeger = tab.ProjeKopyala(kaynakYil, kaynakAsama, hedefYil, hedefAsama, projeKodListe);
                if (donusDeger.islemSonuc)
                    BilgiEkle(bilgi, "Projeler kopyalandı.");
                else
                    BilgiEkle(bilgi, "Projeler kopyalanırken hata oluştu." + PrimaryKeyDegistir(donusDeger.hataStr));

                ve.hata = "";
                donusDeger = tabIlilce.IlIlceKopyala(kaynakYil, kaynakAsama, hedefYil, hedefAsama, true, projeKodListe);
                if (donusDeger.islemSonuc)
                    BilgiEkle(bilgi, "Projelerin il ilçe bilgileri kopyalandı.");
                else
                    BilgiEkle(bilgi, "Projelerin il ilçe bilgileri kopyalanırken hata oluştu." + PrimaryKeyDegistir(donusDeger.hataStr));

                ve.hata = "";
                donusDeger = tabBilgi.BilgiKopyala(kaynakYil, kaynakAsama, hedefYil, hedefAsama, true, projeKodListe);
                if (donusDeger.islemSonuc)
                    BilgiEkle(bilgi, "Projelerin bilgileri kopyalandı.");
                else
                    BilgiEkle(bilgi, "Projelerin bilgileri kopyalanırken hata oluştu." + PrimaryKeyDegistir(donusDeger.hataStr));
            }

            if (DiziElemanAl(kopyalanacaklar, 1, false)) //AltProjeler
            {
                TABYATAltProje tab = new TABYATAltProje(ve);
                TABYATIlIlce tabIlilce = new TABYATIlIlce(ve);
                TABYATPROJEKALKINMABOLGE tabBolge = new TABYATPROJEKALKINMABOLGE(ve);
                TABYATALTPROJEBILGI tabBilgi = new TABYATALTPROJEBILGI(ve);
                TABYATALTPROJEDETAYPROGRAMI tabDetayProgram = new TABYATALTPROJEDETAYPROGRAMI(ve);
                ve.hata = "";
                //ve.BeginTransaction();
                Sonuc donusDeger = tab.AltProjeKopyala(kaynakYil, kaynakAsama, hedefYil, hedefAsama, projeKodListe);
                if (donusDeger.islemSonuc)
                    BilgiEkle(bilgi, "Alt projeler kopyalandı.");
                else
                    BilgiEkle(bilgi, "Alt projeler kopyalanırken hata oluştu:" + PrimaryKeyDegistir(donusDeger.hataStr));

                ve.hata = "";
                donusDeger = tabIlilce.IlIlceKopyala(kaynakYil, kaynakAsama, hedefYil, hedefAsama, false, projeKodListe);
                if (donusDeger.islemSonuc)
                    BilgiEkle(bilgi, "Alt proje ilçeleri kopyalandı.");
                else
                    BilgiEkle(bilgi, "Alt proje ilçeleri kopyalanırken hata oluştu:" + PrimaryKeyDegistir(donusDeger.hataStr));

                ve.hata = "";
                donusDeger = tabBolge.Kopyala(kaynakYil, kaynakAsama, hedefYil, hedefAsama, false, projeKodListe);
                if (donusDeger.islemSonuc)
                    BilgiEkle(bilgi, "Alt proje yatırım bölgeleri kopyalandı.");
                else
                    BilgiEkle(bilgi, "Alt proje yatırım bölgeleri kopyalanırken hata oluştu:" + PrimaryKeyDegistir(donusDeger.hataStr));

                ve.hata = "";
                donusDeger = tabBilgi.BilgiKopyala(kaynakYil, kaynakAsama, hedefYil, hedefAsama, false, projeKodListe);
                if (donusDeger.islemSonuc)
                    BilgiEkle(bilgi, "Alt proje bilgileri kopyalandı.");
                else
                    BilgiEkle(bilgi, "Alt proje bilgileri kopyalanırken hata oluştu:" + PrimaryKeyDegistir(donusDeger.hataStr));

                ve.hata = "";
                donusDeger = tabDetayProgram.Kopyala(kaynakYil, kaynakAsama, hedefYil, hedefAsama, projeKodListe);
                if (donusDeger.islemSonuc)
                    BilgiEkle(bilgi, "Detay program kopyalandı.");
                else
                    BilgiEkle(bilgi, "Detay program kopyalanırken hata oluştu:" + PrimaryKeyDegistir(donusDeger.hataStr));
            }

            if (DiziElemanAl(kopyalanacaklar, 2, false)) //Kaynaklar
            {
                TABYATButceKaynak tab = new TABYATButceKaynak(ve);

                ve.hata = "";
                Sonuc donusDeger = tab.KaynakKopyala(kaynakYil, kaynakAsama, hedefYil, hedefAsama, projeKodListe);
                if (donusDeger.islemSonuc)
                    BilgiEkle(bilgi, "Kaynaklar kopyalandı.");
                else
                    BilgiEkle(bilgi, "Kaynaklar kopyalanırken hata oluştu:" + PrimaryKeyDegistir(donusDeger.hataStr));

                ve.hata = "";
                donusDeger = tab.PebKodKopyala(kaynakYil, kaynakAsama, hedefYil, hedefAsama, projeKodListe);
                if (donusDeger.islemSonuc)
                    BilgiEkle(bilgi, "Kaynak-faaliyet kodları kopyalandı.");
                else
                    BilgiEkle(bilgi, "Kaynak-faaliyet kodları kopyalanırken hata oluştu:" + PrimaryKeyDegistir(donusDeger.hataStr));

                ve.hata = "";
                donusDeger = tab.GeriOdemeKopyala(kaynakYil, kaynakAsama, hedefYil, hedefAsama, projeKodListe);
                if (donusDeger.islemSonuc)
                    BilgiEkle(bilgi, "Geri ödemeler kopyalandı.");
                else
                    BilgiEkle(bilgi, "Geri ödemeler kopyalanırken hata oluştu:" + PrimaryKeyDegistir(donusDeger.hataStr));

                ve.hata = "";
                donusDeger = tab.KrediDetayKopyala(kaynakYil, kaynakAsama, hedefYil, hedefAsama, projeKodListe);
                if (donusDeger.islemSonuc)
                    BilgiEkle(bilgi, "Kredi bilgileri kopyalandı.");
                else
                    BilgiEkle(bilgi, "Kredi bilgileri kopyalanırken hata oluştu:" + PrimaryKeyDegistir(donusDeger.hataStr));
            }

            if (kopyalanacaklar == null || DiziElemanAl(kopyalanacaklar, 3, false)) //Rakamlar
            {
                if (kaynakYil == hedefYil)
                {
                    TABYATButceKaynakTeklif tabTeklif = new TABYATButceKaynakTeklif(ve);

                    ve.hata = "";
                    ve.BeginTransaction();
                    Sonuc donusDeger = tabTeklif.AsamaSil(kaynakYil, kaynakAsama, hedefAsama, projeKodListe);
                    if (donusDeger.islemSonuc)
                        donusDeger = tabTeklif.AsamaKopyala(kaynakYil, kaynakAsama, hedefAsama, projeKodListe);

                    if (donusDeger.islemSonuc)
                        donusDeger = tabTeklif.YeniProjeTeklifSil(kaynakYil, kaynakAsama, hedefAsama, projeKodListe);

                    if (donusDeger.islemSonuc)
                        donusDeger = tabTeklif.AsamaKopyalaYeniProje(kaynakYil, kaynakAsama, hedefAsama, projeKodListe);

                    if (donusDeger.islemSonuc)
                    {
                        BilgiEkle(bilgi, "Ödenek ve harcama rakamları kopyalandı.");
                        ve.Commit();
                    }
                    else
                    {
                        BilgiEkle(bilgi, "Ödenek ve harcama rakamları kopyalanırken hata oluştu:" + PrimaryKeyDegistir(donusDeger.hataStr));
                        ve.Rollback();
                    }
                }
                else
                    BilgiEkle(bilgi, "Rakamlar kopyalanırken aynı yıla ait rakamları kopyalayabilirsiniz!");
            }

            if (kopyalanacaklar == null || DiziElemanAl(kopyalanacaklar, 4, false)) //Detay Program
            {
                TABYATALTPROJEDETAYPROGRAMI tabDetay = new TABYATALTPROJEDETAYPROGRAMI(ve);

                ve.hata = "";
                ve.BeginTransaction();
                Sonuc donusDeger = tabDetay.DetayProgramKopyala(kaynakYil, kaynakAsama, hedefYil, hedefAsama, projeKodListe);

                if (donusDeger.islemSonuc)
                {
                    BilgiEkle(bilgi, "Detay programlar kopyalandı.");
                    ve.Commit();
                }
                else
                {
                    BilgiEkle(bilgi, "Detay programlar kopyalanırken hata oluştu:" + PrimaryKeyDegistir(donusDeger.hataStr));
                    ve.Rollback();
                }
            }

            if (kopyalanacaklar == null || DiziElemanAl(kopyalanacaklar, 5, false)) //Yolluk Programı
            {
                TABYATYOLLUKPLAN tabYolluk = new TABYATYOLLUKPLAN(ve);

                ve.hata = "";
                ve.BeginTransaction();
                Sonuc donusDeger = tabYolluk.YollukPlanKopyala(kaynakYil, kaynakAsama, hedefYil, hedefAsama, projeKodListe);

                if (donusDeger.islemSonuc)
                {
                    BilgiEkle(bilgi, "Detay programlar kopyalandı.");
                    ve.Commit();
                }
                else
                {
                    BilgiEkle(bilgi, "Detay programlar kopyalanırken hata oluştu:" + PrimaryKeyDegistir(donusDeger.hataStr));
                    ve.Rollback();
                }
            }

            return bilgi;
        }

        private static string PrimaryKeyDegistir(string p)
        {
            if (p.Contains("PRIMARY KEY"))
                return "Tekrarlanan kayıt olduğundan işlem yapılamadı!";
            else
                return p;
        }

        private static void BilgiEkle(Sonuc sonuc, string bilgi)
        {
            if (!string.IsNullOrWhiteSpace(sonuc.bilgiStr))
                sonuc.bilgiStr += "<p><p>";
            sonuc.bilgiStr += bilgi;
        }

        private static bool DiziElemanAl(bool[] dizi, int indis, bool def)
        {
            if (dizi == null) return def;
            if (dizi.Length <= indis) return def;
            return dizi[indis];
        }

        internal static ObjectArray ProjeHarcamalari(Kullanici kullanan, int yil, Tertip kistasTertip, string senaryoKod, string altProjeKurKod, int avansDurum, bool adlariBul)
        {
            ObjectArray oa = new ObjectArray();
            oa.sonuc = Yetki.OkumaHakkiVarMi(kullanan, altProjeKurKod);
            if (!oa.sonuc.islemSonuc)
                return oa;

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriYAT", TNS.KYM.Arac.KimlikBul(kullanan));
            TABYATButceKaynak tab = new TABYATButceKaynak(ve);

            UcgenKodBilgileri kodBilgileri = new UcgenKodBilgileri();

            kodBilgileri.Add(new UcgenKodBilgi(kistasTertip.KodNolar(4), kistasTertip.KodNoKirilimSay(4)));
            kodBilgileri.Add(new UcgenKodBilgi(kistasTertip.KodNolar(0), kistasTertip.KodNoKirilimSay(0)));
            kodBilgileri.Add(new UcgenKodBilgi(kistasTertip.KodNolar(1), kistasTertip.KodNoKirilimSay(1)));
            kodBilgileri.Add(new UcgenKodBilgi(kistasTertip.KodNolar(2), kistasTertip.KodNoKirilimSay(2)));
            kodBilgileri.Add(new UcgenKodBilgi(kistasTertip.KodNolar(3), 2));

            EkDegerParam[] ekBilgiler = new EkDegerParam[4];
            ekBilgiler[0] = new EkDegerParam(ENUMTabloKod.TABPEBOdenek, 0, 0, senaryoKod);
            ekBilgiler[1] = new EkDegerParam(ENUMTabloKod.TABMasrafPlaniBirim, 0, 0, "8;0/13;3,4,7,8");
            ekBilgiler[2] = new EkDegerParam(ENUMTabloKod.TABOdenekGonderme, 0, 0, "");
            ENUMTabloKod tk;
            if (avansDurum == 1)
                tk = ENUMTabloKod.TABPEBHarcama;
            else
                tk = ENUMTabloKod.TABOdemeEmri;
            ekBilgiler[3] = new EkDegerParam(tk, 0, 0, "");
            IBHMv2Servis servisBHMv2 = TNS.BHMv2.Arac.Tanimla();
            OdenekBilgi[] obler = servisBHMv2.ListeTertipOl(kullanan, kistasTertip, kodBilgileri, ekBilgiler, true, false, adlariBul);

            string sektorKod = "";
            string sektorAd = "";
            string projeAd = "";
            string DPTprojeKod = "";
            string altProjeAd = "";
            int altProjeNo = 0;
            if (obler != null)
            {
                string eskiPebKod = "";
                for (int i = 0; i < obler.Length; i++)
                {
                    string pebKod = obler[i].tertip.kodlar[4];
                    if (pebKod != eskiPebKod)
                    {
                        eskiPebKod = pebKod;
                        FaaliyetProjeBilgiBul(tab, yil, senaryoKod, pebKod, "", out sektorKod, out sektorAd, out DPTprojeKod, out projeAd, out altProjeAd, out altProjeNo, false, ve, kullanan);
                    }
                    ArrayList al = new ArrayList();
                    al.Add(sektorKod);
                    al.Add(sektorAd);
                    al.Add(DPTprojeKod);
                    al.Add(projeAd);
                    al.Add(altProjeNo);
                    al.Add(altProjeAd);
                    al.Add(obler[i]);
                    oa.Ekle(al);
                }
            }
            else
                oa.sonuc.DegerAta("Veri bulunamadı");
            ve.VeriErisimKapat();
            return oa;
        }

        internal static ObjectArray ProjeSunumBilgileri(Kullanici kullanan, int yil, Tertip kistasTertip, string senaryoKod, string senaryoKodOnc, int Hepsi1Eski2Yeni3)
        {
            ObjectArray oa = new ObjectArray();
            oa.sonuc = Yetki.OkumaHakkiVarMi(kullanan, kistasTertip.kodlar[0]);
            if (!oa.sonuc.islemSonuc)
                return oa;
            UZY.IUZYServis uzyServis = UZY.Arac.Tanimla();
            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriYAT", TNS.KYM.Arac.KimlikBul(kullanan));
            TABYATButceKaynak tab = new TABYATButceKaynak(ve);

            UcgenKodBilgileri kodBilgileri = new UcgenKodBilgileri();

            kodBilgileri.Add(new UcgenKodBilgi(kistasTertip.KodNolar(4), kistasTertip.KodNoKirilimSay(4)));

            EkDegerParam[] ekBilgiler = new EkDegerParam[3];
            ekBilgiler[0] = new EkDegerParam(ENUMTabloKod.TABPEBOdenek, -1, 0, senaryoKodOnc);
            ekBilgiler[1] = new EkDegerParam(ENUMTabloKod.TABPEBOdenek, 0, 0, senaryoKod);
            ekBilgiler[2] = new EkDegerParam(ENUMTabloKod.TABPEBTavanUstu, 0, 0, senaryoKod);
            //OrtakFonksiyonlar.HataDosyaYaz("YATButceSunum.txt", kistasTertip.TertipStrOl(), true);

            IBHMv2Servis servisBHMv2 = TNS.BHMv2.Arac.Tanimla();
            OdenekBilgi[] obler = servisBHMv2.ListeTertipOl(kullanan, kistasTertip, kodBilgileri, ekBilgiler, true, false);

            string sektorKod = "";
            string sektorAd = "";
            string projeAd = "";
            string DPTprojeKod = "";
            string altProjeAd = "";
            string peb2Ad = "";
            int altProjeNo = 0;
            if (obler != null)
            {
                string eskiPebKod = "";
                string eskiPeb2 = "";

                for (int i = 0; i < obler.Length; i++)
                {
                    string pebKod = obler[i].tertip.kodlar[4];
                    string peb2 = pebKod.Substring(0, 2);

                    string pebOrta = pebKod.Substring(3, 2);
                    //OrtakFonksiyonlar.HataDosyaYaz("YATButceSunum.txt", $"Önce: {pebKod},{peb2},{pebOrta}", false);
                    if (Hepsi1Eski2Yeni3 == 2 && pebOrta == "20" || Hepsi1Eski2Yeni3 == 3 && pebOrta != "20") continue;
                    //OrtakFonksiyonlar.HataDosyaYaz("YATButceSunum.txt", $"Sonra: {pebKod},{peb2},{pebOrta}", false);
                    if (peb2 != eskiPeb2)
                    {
                        eskiPeb2 = peb2;
                        peb2Ad = PebAdBul(uzyServis, kullanan, yil, peb2);
                    }

                    if (pebKod != eskiPebKod)
                    {
                        eskiPebKod = pebKod;
                        FaaliyetProjeBilgiBul(tab, yil, senaryoKod, pebKod, "", out sektorKod, out sektorAd, out DPTprojeKod, out projeAd, out altProjeAd, out altProjeNo, false, ve, kullanan);
                    }

                    ArrayList al = new ArrayList();
                    al.Add(peb2Ad);
                    al.Add(projeAd);
                    al.Add(DPTprojeKod);
                    al.Add(sektorKod);
                    al.Add(sektorAd);
                    al.Add(altProjeNo);
                    al.Add(altProjeAd);
                    al.Add(obler[i]);
                    oa.Ekle(al);
                    //                    OrtakFonksiyonlar.HataDosyaYaz("YATButceSunum.txt", $"al: {peb2Ad}{projeAd} {DPTprojeKod} {sektorKod} {sektorAd} {altProjeNo} {altProjeAd} {obler[i].tertip.TertipStrOl()}{obler[i].EkRakam(2)}", false);
                }
            }
            else
                oa.sonuc.DegerAta("Veri bulunamadı");
            ve.VeriErisimKapat();
            return oa;
        }

        private static string PebAdBul(UZY.IUZYServis servis, Kullanici kullanan, int yil, string peb2)
        {
            return servis.UzayDegeriStr(kullanan, "ABSPEBKOD", TNS.UZY.Arac.UzayKosulOl(yil.ToString(), peb2), true, null);
        }

        internal static ObjectArray ProjeIslemleri(Kullanici kullanan, MPRevizeFormSorgu form)
        {
            int yil = form.yil;
            string senaryoKod = "0600";
            string altProjeKurKod = form.tertip.kodlar[0];
            ObjectArray oa = new ObjectArray();
            oa.sonuc = Yetki.OkumaHakkiVarMi(kullanan, altProjeKurKod);
            if (!oa.sonuc.islemSonuc)
                return oa;
            TNS.BHMv2.IBHMv2Servis bhmv2Servis = TNS.BHMv2.Arac.Tanimla();
            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(kullanan));
            VeriErisim ve2 = VeriErisim.VeriErisimBul(null, "BaglantiSatiriYAT", TNS.KYM.Arac.KimlikBul(kullanan));
            TABYATButceKaynak tab = new TABYATButceKaynak(ve);
            TABYATButceKaynak tab2 = new TABYATButceKaynak(ve2);
            int eko2Uz = TNS.BHMv2.Arac.Eko2UzBul(yil);

            IDataReader dr = tab.MasrafPlaniTertipBilgi(form, bhmv2Servis.FormKosulOl(kullanan, form, form.detayGoster ? "IC" : "MPGMASRAFPLANIREVIZEFORM", form.detayGoster, form.detayGoster ? "MPGMASRAFPLANIREVIZEFORM" : null), eko2Uz);

            string sektorKod = "";
            string sektorAd = "";
            string projeAd = "";
            string DPTprojeKod = "";
            string altProjeAd = "";
            int altProjeNo = 0;
            if (dr != null)
            {
                string eskiPebKod = "";
                while (dr.Read())
                {
                    string kurKod = OrtakFonksiyonlar.KodNoktaKoy(form.tertip.Kirilimlar(0), VeriErisim.DegerAl(dr, 0, ""));
                    string fonKod = OrtakFonksiyonlar.KodNoktaKoy(form.tertip.Kirilimlar(1), VeriErisim.DegerAl(dr, 1, ""));
                    string finKod = OrtakFonksiyonlar.KodNoktaKoy(form.tertip.Kirilimlar(2), VeriErisim.DegerAl(dr, 2, ""));
                    string ekoKod = OrtakFonksiyonlar.KodNoktaKoy(form.tertip.Kirilimlar(3), VeriErisim.DegerAl(dr, 3, ""));
                    string pebKod = OrtakFonksiyonlar.KodNoktaKoy(form.tertip.Kirilimlar(4), VeriErisim.DegerAl(dr, 4, ""));
                    string kyKod = VeriErisim.DegerAl(dr, 5, "");
                    double miktar = VeriErisim.DegerAl(dr, 6, 0.0);
                    int revizeNo = VeriErisim.DegerAl(dr, 7, 0);
                    TNSDateTime kayitTarih = VeriErisim.DegerAl(dr, 8);
                    int belgeTur = VeriErisim.DegerAl(dr, 9, 0);
                    string aciklama = VeriErisim.DegerAl(dr, 10, "");
                    string bumkoIslem = VeriErisim.DegerAl(dr, 11, "");
                    string altIslemKod = OrtakFonksiyonlar.KodNoktaKoy("99.99.99", VeriErisim.DegerAl(dr, 12, ""));
                    int formDurum = VeriErisim.DegerAl(dr, 13, 0);

                    ArrayList al = new ArrayList();
                    if (pebKod != eskiPebKod)
                    {
                        eskiPebKod = pebKod;
                        FaaliyetProjeBilgiBul(tab2, yil, senaryoKod, pebKod, "", out sektorKod, out sektorAd, out DPTprojeKod, out projeAd, out altProjeAd, out altProjeNo, false, ve, kullanan);
                    }
                    al.Add(sektorKod);
                    al.Add(sektorAd);
                    al.Add(DPTprojeKod);
                    al.Add(projeAd);
                    al.Add(altProjeNo);
                    al.Add(altProjeAd);
                    al.Add(kurKod);
                    al.Add(fonKod);
                    al.Add(finKod);
                    al.Add(ekoKod);
                    al.Add(pebKod);
                    al.Add(kyKod);
                    al.Add(miktar);
                    al.Add(revizeNo);
                    al.Add(kayitTarih);
                    al.Add(belgeTur);
                    al.Add(aciklama);
                    al.Add(bumkoIslem);
                    al.Add(altIslemKod);
                    al.Add(formDurum);
                    oa.Ekle(al);
                }
                dr.Close();
            }
            else
                oa.sonuc.DegerAta(ve.hata);
            ve.VeriErisimKapat();
            ve2.VeriErisimKapat();
            return oa;
        }

        public static AltFaalBilgi AltFaaliyetBilgiBul(VeriErisim veriErisim, Kullanici kullanan, int yil, string pebKod, string kurKod, string senaryoKod, bool illleriBul)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriYAT", TNS.KYM.Arac.KimlikBul(kullanan));
            TABYATButceKaynak tab = new TABYATButceKaynak(ve);
            string sektorKod;
            string sektorAd;
            string DPTprojeKod;
            string projeAd;
            string altProjeAd;
            int altProjeNo;

            AltFaalBilgi afb = FaaliyetProjeBilgiBul(tab, yil, senaryoKod, pebKod, kurKod, out sektorKod, out sektorAd, out DPTprojeKod, out projeAd, out altProjeAd, out altProjeNo, illleriBul, ve, kullanan);
            ve.VeriErisimKapat();
            return afb;
        }

        private static AltFaalBilgi FaaliyetProjeBilgiBul(TABYATButceKaynak tab, int yil, string senaryoKod, string pebKod, string kurKod, out string sektorKod, out string sektorAd, out string DPTprojeKod, out string projeAd, out string altProjeAd, out int altProjeNo, bool illleriBul, VeriErisim ve, Kullanici kullanan)
        {
            AltFaalBilgi afb = null;
            sektorKod = "";
            sektorAd = "";
            DPTprojeKod = "";
            projeAd = "";
            altProjeNo = 0;
            altProjeAd = "";
            int projeNo = 0;
            IDataReader dr = tab.PebKodProjeBilgiBul(yil, senaryoKod, pebKod, kurKod);
            if (dr != null)
            {
                if (dr.Read())
                {
                    sektorKod = OrtakFonksiyonlar.KodNoktaKoy("99.99", VeriErisim.DegerAl(dr, 0, ""));
                    sektorAd = VeriErisim.DegerAl(dr, 1, "");

                    DPTprojeKod = VeriErisim.DegerAl(dr, 2, "");
                    projeAd = VeriErisim.DegerAl(dr, 3, "");

                    altProjeNo = VeriErisim.DegerAl(dr, 4, 0);
                    altProjeAd = VeriErisim.DegerAl(dr, 5, "");
                    projeNo = VeriErisim.DegerAl(dr, 6, 0);

                    afb = new AltFaalBilgi();
                    afb.pebKod = pebKod;
                    afb.sektor = new SektorSozluk(sektorKod, sektorAd, "");
                    afb.altProje = new AltProje();
                    afb.altProje.ad = altProjeAd;
                    afb.altProje.kod = altProjeNo;

                    afb.proje = new Proje();
                    afb.proje.kod = projeNo;
                    afb.proje.DPTProjeNo = DPTprojeKod;
                    afb.proje.ad = projeAd;
                }
                dr.Close();
            }
            if (illleriBul && altProjeNo > 0)
            {
                afb.iller = IsKurallari.IlIlce.Listele(ve, kullanan, yil, senaryoKod, projeNo, altProjeNo);
            }
            return afb;
        }

        public static ObjectArray MaliYapiBul(VeriErisim veriErisim, Kullanici kullanan, int raporYil, string raporSenaryoKod, int butceYil, string senaryoKod, ENUMProjeMaliYapiKod projeMaliYapiTur, int projeNo, int altProjeNo, bool oncekiYillarDahil, bool detayAltProje)
        {
            ObjectArray donusDeger = new ObjectArray();

            donusDeger.sonuc = Yetki.OkumaHakkiVarMi(kullanan, null);
            if (!donusDeger.sonuc.islemSonuc)
                return donusDeger;

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriYAT", TNS.KYM.Arac.KimlikBul(kullanan));
            TABYATButceKaynakTeklif tabBK = new TABYATButceKaynakTeklif(ve);
            IDataReader dr = tabBK.ListeMaliYapi(raporYil, raporSenaryoKod, butceYil, senaryoKod, projeMaliYapiTur, projeNo, altProjeNo, oncekiYillarDahil, detayAltProje);
            if (dr == null)
            {
                donusDeger.sonuc.DegerAta(ve.hata);
                ve.VeriErisimKapat();
                return donusDeger;
            }
            ProjeMaliYapisi pmy = null;
            while (dr.Read())
            {
                int icProjeNo = VeriErisim.DegerAl(dr, 0, 0);
                int icAltprojeNo = VeriErisim.DegerAl(dr, 1, 0);

                if (pmy == null || pmy.projeNo != icProjeNo || pmy.altProjeNo != icAltprojeNo)
                {
                    pmy = new ProjeMaliYapisi();
                    pmy.projeNo = icProjeNo;
                    pmy.altProjeNo = icAltprojeNo;
                    donusDeger.Ekle(pmy);
                }
                pmy.DegerAta((ENUMButceKaynakTur)VeriErisim.DegerAl(dr, 2, 0), VeriErisim.DegerAl(dr, 3, (double)0), VeriErisim.DegerAl(dr, 4, (double)0), VeriErisim.DegerAl(dr, 5, (double)0));
            }
            dr.Close();
            ve.VeriErisimKapat();
            return donusDeger;
        }

        internal static ObjectArray OdenekListesiBul(VeriErisim veriErisim, Kullanici kullanan, int butceYil, string senaryoKod, int teklifYil, int projeNo, int altProjeNo)
        {
            ObjectArray donusDeger = new ObjectArray();
            Tertip tUz = new Tertip(butceYil, ENUMTabloKod.TABPEBOdenek);
            int eko2Uz = tUz.KodUzunlukBul(3, 2, true);
            string ekoYapi = tUz.Kirilimlar(3);
            donusDeger.sonuc = Yetki.OkumaHakkiVarMi(kullanan, null);
            if (!donusDeger.sonuc.islemSonuc)
                return donusDeger;

            string[] sorgular = new string[5];
            string[] baglantilar = new string[5];
            sorgular[0] = TABYATProje.OdenekListesiSql(projeNo, altProjeNo, butceYil, senaryoKod, teklifYil, "", eko2Uz); //0,1
            sorgular[1] = TABYATProje.OdenekListesiSql(projeNo, altProjeNo, butceYil, senaryoKod, teklifYil, "07", eko2Uz);//2,3
            sorgular[2] = TABYATProje.OdenekListesiYeniSql(projeNo, altProjeNo, butceYil, senaryoKod, teklifYil, eko2Uz); //4,5,6
            sorgular[3] = TABYATProje.OdenekListesiSql(projeNo, altProjeNo, butceYil, senaryoKod, teklifYil, "", eko2Uz, true); //7,8
            sorgular[4] = TABYATProje.OdenekListesiSql(projeNo, altProjeNo, butceYil, senaryoKod, teklifYil, "07", eko2Uz, true); //9,10
            int[] ekoAdYer = { 0, 2, 4, 7, 9 };
            baglantilar[0] = "BaglantiSatiriBHMv2";
            baglantilar[1] = "BaglantiSatiriBHMv2";
            baglantilar[2] = "BaglantiSatiriBHMv2";
            baglantilar[3] = "BaglantiSatiriBHMv2";
            baglantilar[4] = "BaglantiSatiriBHMv2";

            TNS.UZY.IUZYServis servis = TNS.UZY.Arac.Tanimla();
            SortedList ht = servis.SorguBirlestir(sorgular, baglantilar, new int[] { 0 });
            foreach (DictionaryEntry de in ht)
            {
                TNS.UZY.HTArray alAnahtar = (TNS.UZY.HTArray)de.Key;
                TNS.UZY.HTArray alVeri = (TNS.UZY.HTArray)de.Value;

                string ekoKod = alAnahtar.StrAl(0);
                ekoKod = OrtakFonksiyonlar.KodNoktaKoy(ekoYapi, ekoKod);

                OdenekBilgi ob = new OdenekBilgi(butceYil, ENUMTabloKod.TABPEBOdenek);
                ob.tertip.kodlar[3] = ekoKod;
                for (int i = 0; i < 5; i++)
                {
                    ob.tertip.adlar[3] = alVeri.StrAl(ekoAdYer[i]);
                    if (!string.IsNullOrWhiteSpace(ob.tertip.adlar[3])) break;
                }

                double toplam = alVeri.DblAl(1);
                double dis = alVeri.DblAl(3);

                toplam += (alVeri.DblAl(5) + alVeri.DblAl(6));
                dis += (alVeri.DblAl(5));

                ob.ekRakamlar.Add(toplam);
                ob.ekRakamlar.Add(dis);
                ob.ekRakamlar.Add(alVeri.DblAl(8));
                ob.ekRakamlar.Add(alVeri.DblAl(10));

                donusDeger.Ekle(ob);
            }

            return donusDeger;
        }

        internal static UcgenSatir[] OdenekListesiBulTam(VeriErisim veriErisim, Kullanici kullanan, int butceYil, string senaryoKod, int projeNo, int altProjeNo)
        {
            ObjectArray donusDeger = new ObjectArray();

            donusDeger.sonuc = Yetki.OkumaHakkiVarMi(kullanan, null);
            if (!donusDeger.sonuc.islemSonuc)
                throw new Exception(donusDeger.sonuc.hataStr);

            string[] sorgular = new string[5];
            string[] baglantilar = new string[5];
            sorgular[0] = TABYATProje.OdenekListesiSqlTam(projeNo, altProjeNo, butceYil, senaryoKod, 0);
            sorgular[1] = TABYATProje.OdenekListesiSqlTam(projeNo, altProjeNo, butceYil, senaryoKod, 1);
            sorgular[2] = TABYATProje.OdenekListesiSqlTam(projeNo, altProjeNo, butceYil, senaryoKod, 2);
            sorgular[3] = TABYATProje.OdenekListesiYeniSqlTam(projeNo, altProjeNo, butceYil, senaryoKod, "01");
            sorgular[4] = TABYATProje.OdenekListesiYeniSqlTam(projeNo, altProjeNo, butceYil, senaryoKod, "07");

            baglantilar[0] = "BaglantiSatiriBHMv2";
            baglantilar[1] = "BaglantiSatiriBHMv2";
            baglantilar[2] = "BaglantiSatiriBHMv2";
            baglantilar[3] = "BaglantiSatiriBHMv2";
            baglantilar[4] = "BaglantiSatiriBHMv2";

            Tertip tUz = new Tertip(butceYil, ENUMTabloKod.TABPEBOdenek);

            TNS.UZY.IUZYServis servis = TNS.UZY.Arac.Tanimla();
            SortedList ht = servis.SorguBirlestir(sorgular, baglantilar, new int[] { 0, 1, 2, 3 });
            foreach (DictionaryEntry de in ht)
            {
                TNS.UZY.HTArray alAnahtar = (TNS.UZY.HTArray)de.Key;
                TNS.UZY.HTArray alVeri = (TNS.UZY.HTArray)de.Value;
                OdenekBilgi ob = new OdenekBilgi(butceYil, ENUMTabloKod.TABPEBOdenek);

                ob.tertip.kodlar[0] = OrtakFonksiyonlar.KodNoktaKoy(ob.tertip.Kirilimlar(0), alAnahtar.StrAl(0));
                ob.tertip.kodlar[1] = OrtakFonksiyonlar.KodNoktaKoy(ob.tertip.Kirilimlar(1), alAnahtar.StrAl(1));
                ob.tertip.kodlar[2] = OrtakFonksiyonlar.KodNoktaKoy(ob.tertip.Kirilimlar(2), alAnahtar.StrAl(2));
                ob.tertip.kodlar[3] = OrtakFonksiyonlar.KodNoktaKoy(ob.tertip.Kirilimlar(3), alAnahtar.StrAl(3));

                ob.tertip.adlar[3] = alVeri.StrAl(0);
                double yil0 = alVeri.DblAl(1);
                if (string.IsNullOrWhiteSpace(ob.tertip.adlar[3])) ob.tertip.adlar[3] = alVeri.StrAl(2);
                double yil1 = alVeri.DblAl(3);
                if (string.IsNullOrWhiteSpace(ob.tertip.adlar[3])) ob.tertip.adlar[3] = alVeri.StrAl(4);
                double yil2 = alVeri.DblAl(5);

                if (string.IsNullOrWhiteSpace(ob.tertip.adlar[3])) ob.tertip.adlar[3] = alVeri.StrAl(6);
                yil0 += alVeri.DblAl(7);
                yil1 += alVeri.DblAl(8);
                yil2 += alVeri.DblAl(9);

                if (string.IsNullOrWhiteSpace(ob.tertip.adlar[3])) ob.tertip.adlar[3] = alVeri.StrAl(10);
                yil0 += alVeri.DblAl(11);
                yil1 += alVeri.DblAl(12);
                yil2 += alVeri.DblAl(13);

                ob.ekRakamlar.Add(yil0);
                ob.ekRakamlar.Add(yil1);
                ob.ekRakamlar.Add(yil2);
                donusDeger.Ekle(ob);
            }
            UcgenKodBilgileri kodBilgileri = new UcgenKodBilgileri();
            kodBilgileri.Add(new UcgenKodBilgi(1, tUz.KodNoKirilimSay(0)));
            kodBilgileri.Add(new UcgenKodBilgi(2, 4));
            kodBilgileri.Add(new UcgenKodBilgi(3, 1));
            kodBilgileri.Add(new UcgenKodBilgi(4, 4));
            return TNS.BHMv2.Arac.UcgenTertipOl(kullanan, kodBilgileri, true, (OdenekBilgi[])donusDeger.objeler.ToArray(typeof(OdenekBilgi)), butceYil);
        }


        internal static ObjectArray OdenekListesiBulEko2(VeriErisim veriErisim, Kullanici kullanan, int butceYil, string senaryoKod, int projeNo, int altProjeNo)
        {
            ObjectArray donusDeger = new ObjectArray();

            donusDeger.sonuc = Yetki.OkumaHakkiVarMi(kullanan, null);
            if (!donusDeger.sonuc.islemSonuc)
                throw new Exception(donusDeger.sonuc.hataStr);
            //Tertip tUz = new Tertip(butceYil, ENUMTabloKod.TABPEBOdenek);

            string[] sorgular = new string[6];
            string[] baglantilar = new string[6];
            sorgular[0] = TABYATProje.OdenekListesiSql(projeNo, altProjeNo, butceYil, senaryoKod, 0, "", 4, false, true);
            sorgular[1] = TABYATProje.OdenekListesiSql(projeNo, altProjeNo, butceYil, senaryoKod, 1, "", 4, false, true);
            sorgular[2] = TABYATProje.OdenekListesiSql(projeNo, altProjeNo, butceYil, senaryoKod, 2, "", 4, false, true);
            sorgular[3] = TABYATProje.OdenekListesiSql(projeNo, altProjeNo, butceYil, senaryoKod, 0, "", 4, true, true);
            sorgular[4] = TABYATProje.OdenekListesiSql(projeNo, altProjeNo, butceYil, senaryoKod, 1, "", 4, true, true);
            sorgular[5] = TABYATProje.OdenekListesiSql(projeNo, altProjeNo, butceYil, senaryoKod, 2, "", 4, true, true);

            for (int i = 0; i < 6; i++)
            {
                baglantilar[i] = "BaglantiSatiriBHMv2";
            }

            TNS.UZY.IUZYServis servis = TNS.UZY.Arac.Tanimla();
            SortedList ht = servis.SorguBirlestir(sorgular, baglantilar, new int[] { 0, 1 });
            foreach (DictionaryEntry de in ht)
            {
                TNS.UZY.HTArray alAnahtar = (TNS.UZY.HTArray)de.Key;
                TNS.UZY.HTArray alVeri = (TNS.UZY.HTArray)de.Value;
                OdenekBilgi ob = new OdenekBilgi(butceYil, ENUMTabloKod.TABPEBOdenek);
                donusDeger.Ekle(ob);

                ob.tertip.kodlar[2] = OrtakFonksiyonlar.KodNoktaKoy(ob.tertip.Kirilimlar(2), alAnahtar.StrAl(1));
                ob.tertip.kodlar[3] = OrtakFonksiyonlar.KodNoktaKoy(ob.tertip.Kirilimlar(3), alAnahtar.StrAl(0));
                ob.tertip.adlar[3] = alVeri.StrAl(0);
                //if (string.IsNullOrWhiteSpace(ob.tertip.adlar[3])) ob.tertip.adlar[3] = alVeri.StrAl(10);

                for (int i = 0; i < 6; i++)
                {
                    ob.ekRakamlar.Add(alVeri.DblAl(1 + i * 2));
                }
            }
            return donusDeger;
        }


        internal static YeniProjeButce[] YeniProjeListele(int yil, string okunanXML)
        {
            ArrayList al = new ArrayList();
            IDataReader dr = new XMLStringDataReader(okunanXML);
            if (dr != null)
            {
                ProjeListeOl(yil, dr, al);
            }
            return (YeniProjeButce[])al.ToArray(typeof(YeniProjeButce));
        }

        internal static YeniProjeButce[] YeniProjeListele(VeriErisim ve, Kullanici kullanan, int kaynakNo, int yil, string senaryoKod)
        {
            ArrayList al = new ArrayList();
            TABYATYENIPROJEBUTCE tab = new TABYATYENIPROJEBUTCE(ve);
            IDataReader dr = tab.Listele(yil, senaryoKod, kaynakNo);
            if (dr != null)
            {
                ProjeListeOl(yil, dr, al);
            }
            return (YeniProjeButce[])al.ToArray(typeof(YeniProjeButce));
        }

        private static void ProjeListeOl(int yil, IDataReader dr, ArrayList al)
        {
            string ekoKodYapi = TNS.DEG.Arac.DegiskenDegerBul(yil, "/KODYAPI/KOD4/KODYAPI");
            while (dr.Read())
            {
                YeniProjeButce yp = new YeniProjeButce();
                yp.ekoKod = OrtakFonksiyonlar.KodNoktaKoy(ekoKodYapi, VeriErisim.DegerAl(dr, (int)TABYATYENIPROJEBUTCE.ENUMAlanlar.EKOKOD, ""));

                yp.disKredi0 = VeriErisim.DegerAl(dr, (int)TABYATYENIPROJEBUTCE.ENUMAlanlar.DISKREDI0, 0.0);
                yp.disKredi1 = VeriErisim.DegerAl(dr, (int)TABYATYENIPROJEBUTCE.ENUMAlanlar.DISKREDI1, 0.0);
                yp.disKredi2 = VeriErisim.DegerAl(dr, (int)TABYATYENIPROJEBUTCE.ENUMAlanlar.DISKREDI2, 0.0);

                yp.icKaynak0 = VeriErisim.DegerAl(dr, (int)TABYATYENIPROJEBUTCE.ENUMAlanlar.ICKAYNAK0, 0.0);
                yp.icKaynak1 = VeriErisim.DegerAl(dr, (int)TABYATYENIPROJEBUTCE.ENUMAlanlar.ICKAYNAK1, 0.0);
                yp.icKaynak2 = VeriErisim.DegerAl(dr, (int)TABYATYENIPROJEBUTCE.ENUMAlanlar.ICKAYNAK2, 0.0);

                yp.projeTutariIc = VeriErisim.DegerAl(dr, (int)TABYATYENIPROJEBUTCE.ENUMAlanlar.PROJETUTARIIC, 0.0);
                yp.projeTutariDis = VeriErisim.DegerAl(dr, (int)TABYATYENIPROJEBUTCE.ENUMAlanlar.PROJETUTARIDIS, 0.0);

                yp.ekoAd = VeriErisim.DegerAl(dr, ((int)TABYATYENIPROJEBUTCE.ENUMAlanlar.ICKAYNAK2) + 1, "");
                al.Add(yp);
            }
            dr.Close();
        }
    }
}