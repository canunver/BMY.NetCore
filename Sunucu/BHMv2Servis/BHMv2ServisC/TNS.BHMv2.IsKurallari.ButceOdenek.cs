using System;
using System.Data;
using TNS;
using TNS.Veri;
using TNS.BHMv2;
using TNS.BHMv2.IsKurallari.Veri;
using TNS.KYM;
using System.Collections;
using TNS.BKD;
using TNS.UZY;

namespace TNS.BHMv2.IsKurallari
{
    /// <summary>
    /// BHMv2ButceOdenek formu ile ilgili, iş kuralları kontrollerini yapıp,
    /// veri tabanı bağlantısını alıp, veri tabanı fonksiyonlarını çağırarak
    /// güncelleme işlemlerini gerçekleştiren fonksiyonları tutan sınıf.
    /// </summary>
    public class BHMv2ButceOdenek
    {
        /// <summary>
        /// Verilen tertip listesindeki çok yıllı ödenekler veri tabanına kaydedilir. 
        /// Tertipler gelir yada gider tertipleri olabilir. Tertip türüne göre veri tabanı tablosu oluşturulur.
        /// Veri tabanı tablosu aynı tablodan türetildiği için, kaynak tablo üzerinden işlemler yapılır.
        /// Sıfır değeri olan ödenekler kaydedilmez. Ödenekler tertibin yılına ve verilen senaryoya kaydedilir.
        /// Verilen tertiplerdeki tüm birimler bulunur ve her biri için yetki kontrolü yapılır.
        /// Tertipler tam, tanımlı ve tertip içi kodlarda tanımlı ise ve yetki kontrolünden geçilmişse, sıfır olmayan tertipler veri tabanına kaydedilir.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler</param>
        /// <param name="senaryoKod">Ödeneklerin kaydedileceği senaryo kodu</param>
        /// <param name="tertipOdenekListesi">Tertip ve çok yıllı ödenek değerlerinin bulunduğu dizi</param>
        /// <returns>İşlem sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public static Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, string senaryoKod, TertipOdenek[] tertipOdenekListesi, bool aciklamali)
        {
            bool tavanZorunlu = TNS.DEG.Arac.VarsayilanKurumBul() == "0023";

            Sonuc sonuc = new Sonuc(true, string.Empty, string.Empty);
            if (string.IsNullOrEmpty(senaryoKod))
                sonuc.DegerAta(ButceHazirlikResI.BHMBO001);
            if (tertipOdenekListesi == null || tertipOdenekListesi.Length == 0)
                sonuc.DegerAta(ButceHazirlikResI.BHMBO002);

            if (!string.IsNullOrEmpty(sonuc.hataStr))
            {
                sonuc.islemSonuc = false;
                return sonuc;
            }

            string kodAd, kod;

            int yil = tertipOdenekListesi[0].tertip.yil;
            ArrayList kurumsalKodlar = new ArrayList();
            string kurumsalKod = string.Empty;
            bool yetkikontrol = false;
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, BHMv2Genel.VeriErisimAdBul(yil), kullanan.KimlikBul());
            VeriErisim veBKD = VeriErisim.VeriErisimBul(veriErisim, BHMv2Genel.VeriErisimAdBulSozluk(), kullanan.KimlikBul());
            string hata = "";
            if (KilitliMiSenaryo(ve, kullanan, yil, TNS.BKD.Arac.KurumsaldanKuruma(tertipOdenekListesi[0].tertip.kodlar[0], tertipOdenekListesi[0].tertip.Kirilimlar(0)), senaryoKod, out hata, true))
            {
                ve.VeriErisimKapat();
                sonuc.DegerAta(hata);
                return sonuc;
            }

            sonuc = TertipListesiniPEBOdenekYap(ref tertipOdenekListesi, senaryoKod, false);
            if (!sonuc.islemSonuc)
            {
                ve.VeriErisimKapat();
                return sonuc;
            }
            //ArrayList programDizi = new ArrayList();
            IUZYServis uzyServis = UZY.Arac.Tanimla();
            Hashtable tavanlar = new Hashtable();
            for (int oNo = 0; oNo < tertipOdenekListesi.Length; oNo++)
            {
                Tertip tertip = tertipOdenekListesi[oNo].tertip;

                for (int i = 0; i < tertip.kodlar.Length; i++)
                {
                    tertip.adlar[i] = UzayYaziDegerGetir(veBKD, uzyServis, kullanan, tertip.UzayAdBul(i), yil.ToString() + "-" + tertip.kodlar[i], true, "");
                }

                if (yil != tertip.yil)
                {
                    sonuc.DegerAta(string.Format(ButceHazirlikResI.BHMBO003, yil.ToString(), tertip.yil.ToString()));
                    break;
                }

                if (kurumsalKod != tertip.kodlar[0])
                {
                    kurumsalKod = tertip.kodlar[0];
                    kurumsalKodlar.Add(kurumsalKod);
                    yetkikontrol = BHMv2Genel.YazmaYetkisiVarMi(kullanan, kurumsalKod, senaryoKod);
                    if (!yetkikontrol)
                    {
                        sonuc.DegerAta(string.Format(ButceHazirlikResI.BHMBO004, tertip.TertipStrOl(), senaryoKod));
                        break;
                    }

                    if (KilitliMiSenaryo(ve, kullanan, yil, kurumsalKod, senaryoKod, out hata, false))
                    {
                        sonuc.DegerAta(hata);
                        return sonuc;
                    }
                }

                if (!tertip.TamMi())
                {
                    sonuc.DegerAta(string.Format(ButceHazirlikResI.BHMBO005, tertip.TertipStrOl()));
                }
                else if (!tertip.IsimlerTamMi(out kodAd, out kod))
                {
                    sonuc.DegerAta(string.Format(ButceHazirlikResI.BHMBO006, kodAd, kod));
                }
                if (sonuc.islemSonuc)
                {
                    //string pKod = TNS.MUH.Arac.DiziElemanAl(tertip.kodlar, 4, "");
                    //if (pKod.Length > 2)
                    //{
                    //    pKod = pKod.Substring(0, 2);
                    //    ProgramlaraEkle(programDizi, pKod);
                    //}
                    if (tertip.kodlar.Length == 5)
                        TavanaEkle(tavanlar, tertip, tertipOdenekListesi[oNo].odenek[0]);
                }
            }
            veBKD.VeriErisimKapat();

            if (yil <= 0)
                sonuc.DegerAta(ButceHazirlikResI.BHMBO007);
            //ProgramKontrol(kullanan, yil, senaryoKod, programDizi, sonuc);
            if (sonuc.islemSonuc)
            {
                TABBHMODENEK tabBHMODENEK = OdenekTabloOl(tertipOdenekListesi[0].tertip.tabloKod, ve);
                TABBHMTAVANUSTU tabBHMTavanUstu = new TABBHMTAVANUSTU(ve);
                TABDEGISIMZAMAN tabZaman = new TABDEGISIMZAMAN(ve);

                ve.BeginTransaction();
                foreach (string item in kurumsalKodlar)
                {
                    sonuc = tabBHMODENEK.DegisimZamaniAyarla(tabZaman, yil, senaryoKod, item, kullanan.kullaniciKodu);
                    if (!sonuc.islemSonuc)
                        break;
                }
                if (sonuc.islemSonuc)
                {
                    for (int oNo = 0; oNo < tertipOdenekListesi.Length; oNo++)
                    {
                        sonuc = tabBHMODENEK.CokYilliOdenekSil(yil, senaryoKod, tertipOdenekListesi[oNo].tertip);
                        if (aciklamali && sonuc.islemSonuc)
                            sonuc = tabBHMODENEK.CokYilliOdenekSilAciklama(yil, senaryoKod, tertipOdenekListesi[oNo].tertip);

                        TertipOdenek to = tertipOdenekListesi[oNo];
                        if (tertipOdenekListesi[0].tertip.tabloKod == ENUMTabloKod.TABPEBTavanUstu)
                        {
                            if (sonuc.islemSonuc)
                                sonuc = tabBHMTavanUstu.Ekle(to.tertip, senaryoKod, to.odenek[0], to.odenek[1], to.odenek[2], to.aciklama);
                        }
                        else
                        {
                            for (int i = 0; i < to.odenek.Length && sonuc.islemSonuc; i++)
                            {
                                double odenek = to.odenek[i];
                                if (odenek != 0)
                                    sonuc = tabBHMODENEK.Ekle(yil, senaryoKod, i, to.tertip, odenek);
                                if (aciklamali && sonuc.islemSonuc && i == 0)
                                    sonuc = tabBHMODENEK.EkleAciklama(yil, senaryoKod, i, to.tertip, to.aciklama);
                            }
                        }
                        if (!sonuc.islemSonuc)
                            break;
                    }
                }
                if (tavanlar.Count > 0 && sonuc.islemSonuc && tabBHMODENEK is TABBHMODENEKGIDER && (senaryoKod == "0000" || senaryoKod == "00.00"))
                {
                    IDataReader drTavanUstu = ((TABBHMODENEKGIDER)tabBHMODENEK).TavanUstuBul(yil, tavanlar, tavanZorunlu);
                    if (drTavanUstu != null)
                    {
                        string hataIc = "";
                        while (drTavanUstu.Read())
                        {
                            hataIc += string.Format("{0} şablonu ile uyumlu tertibinlerin toplamı {2:#,##0}, tavanı ({1:#,##0}) aştığından kayıt yapılmadı!", VeriErisim.DegerAl(drTavanUstu, 0, ""), VeriErisim.DegerAl(drTavanUstu, 1, 0.0), VeriErisim.DegerAl(drTavanUstu, 2, 0.0));
                        }
                        if (hataIc != "")
                        {
                            sonuc.DegerAta(hataIc);
                        }
                        drTavanUstu.Close();
                    }
                    ve.hata = "";
                }
                if (sonuc.islemSonuc)
                    ve.Commit();
                else
                    ve.Rollback();
            }
            ve.VeriErisimKapat();
            return sonuc;
        }

        private static void TavanaEkle(Hashtable tavanlar, Tertip tertip, double odenek)
        {
            if (odenek <= 0) return;
            string tStr = tertip.kodlar[0].Replace(".", "") + "---" + tertip.kodlar[3].Substring(0, 2);
            double eskiOdenek = 0;
            try
            {
                eskiOdenek = (double)tavanlar[tertip];
            }
            catch { }
            tavanlar[tStr] = eskiOdenek + odenek;
        }

        internal static string UzayYaziDegerGetir(TNS.Veri.VeriErisim ve, IUZYServis servisUZY, Kullanici kullanan, string uzayAd, string kosul, bool esitle, string uzayAlan)
        {
            string sql = servisUZY.UzayKosulStr(kullanan == null ? new TNS.KYM.Kullanici() : kullanan, uzayAd, kosul, esitle, uzayAlan);
            if (string.IsNullOrWhiteSpace(sql)) return "";
            string ad = ve.TekKolonSQLStr(sql);
            if (!string.IsNullOrWhiteSpace(ve.hata))
            {
                ad = "";
                ve.hata = "";
            }
            return ad;
        }

        //public static void ProgramKontrol(Kullanici kullanan, int yil, string senaryoKod, ArrayList programDizi, Sonuc sonuc)
        //{
        //    try
        //    {
        //        for (int i = 0; i < programDizi.Count; i++)
        //        {
        //            ProgramOnay progOnayForm = new ProgramOnay();
        //            progOnayForm.butceYili = yil;
        //            progOnayForm.senaryoKod = senaryoKod;
        //            progOnayForm.programKodu = (string)programDizi[i];
        //            progOnayForm = ProgramOnayForm.Oku(null, kullanan, progOnayForm);
        //            if (progOnayForm.durum == 5)
        //            {
        //                sonuc.DegerAta((string)programDizi[i] + " kodlu program onaylandığından kayıt yapılamaz!");
        //            }
        //        }
        //    }
        //    catch { }
        //}

        //public static void ProgramlaraEkle(ArrayList programDizi, string pKod)
        //{
        //    if (programDizi.IndexOf(pKod) >= 0) return;
        //    programDizi.Add(pKod);
        //}

        public static Sonuc TertipListesiniPEBOdenekYap(ref TertipOdenek[] tertipOdenekListesi, string senaryoKod, bool ysTahmin)
        {
            Sonuc s = new Sonuc(true, "", "");
            ArrayList al = new ArrayList();
            for (int j = tertipOdenekListesi.Length - 1; j >= 0; j--)
            {
                TertipOdenek to = tertipOdenekListesi[j];
                if (to.tertip.tabloKod == ENUMTabloKod.TABHarcama || to.tertip.tabloKod == ENUMTabloKod.TABOdenek || to.tertip.tabloKod == ENUMTabloKod.TABButce || to.tertip.tabloKod == ENUMTabloKod.TABPEBOdenek || to.tertip.tabloKod == ENUMTabloKod.TABHarcamaTahmin)
                {
                    bool tertipTammi = to.tertip.TamMi();

                    if (tertipTammi)
                    {
                        al.Add(to);
                    }
                    else if (to.tertip.tabloKod != ENUMTabloKod.TABPEBOdenek && to.tertip.tabloKod != ENUMTabloKod.TABHarcamaTahmin)
                    {
                        s.DegerAta(string.Format(ButceHazirlikResI.BHMBO005, to.tertip.TertipStrOl()));
                        break;
                    }
                    else
                    {
                        Tertip kistasTertip = new Tertip(to.tertip.yil, ENUMTabloKod.TABPEBOdenek, to.tertip.kodlar);
                        UcgenKodBilgileri ukb = new UcgenKodBilgileri();
                        ukb.Add(new UcgenKodBilgi(kistasTertip.KodNolar(0), kistasTertip.KodNoKirilimSay(0)));
                        ukb.Add(new UcgenKodBilgi(kistasTertip.KodNolar(1), kistasTertip.KodNoKirilimSay(1)));
                        ukb.Add(new UcgenKodBilgi(kistasTertip.KodNolar(2), kistasTertip.KodNoKirilimSay(2)));
                        ukb.Add(new UcgenKodBilgi(kistasTertip.KodNolar(3), kistasTertip.KodNoKirilimSay(3)));
                        ukb.Add(new UcgenKodBilgi(kistasTertip.KodNolar(4), kistasTertip.KodNoKirilimSay(4)));

                        int giris = 3;
                        OdenekBilgi[] obler = null;
                        bool cik = false;
                        do
                        {
                            int kaynakYil = 1;
                            string kaynakSenaryo = "06.00";
                            bool tanimliTertip = false;
                            if (giris == 3)
                            {
                                kaynakYil = 0;
                                kaynakSenaryo = senaryoKod;
                            }
                            else if (giris == 2)
                            {
                                string senOnParca = senaryoKod.Substring(0, 2);
                                int senOnParcaInt = OrtakFonksiyonlar.ConvertToInt(senOnParca, 0);
                                if (senOnParcaInt > 0)
                                {
                                    kaynakYil = 0;
                                    kaynakSenaryo = (senOnParcaInt - 1).ToString().PadLeft(2, '0') + ".00";
                                }
                            }
                            else if (giris == 1)
                            {

                            }
                            else if (giris == 0)
                            {
                                tanimliTertip = true;
                                kaynakYil = 0;
                                kaynakSenaryo = senaryoKod;
                            }
                            EkDegerParam[] ekB = new EkDegerParam[1];
                            ekB[0] = new EkDegerParam(ENUMTabloKod.TABPEBOdenek, kaynakYil, 0, kaynakSenaryo);
                            obler = UcgenServisi.ListeTertipOl(null, null, kistasTertip, ukb, ekB, !tanimliTertip, tanimliTertip, true, -1);
                            if (obler.Length > 0) cik = true;
                            else
                            {
                                giris--;
                                if (giris < 0) cik = true;
                            }

                        } while (!cik);

                        if (obler == null || obler.Length == 0)
                        {
                            s.DegerAta(string.Format(ButceHazirlikResI.BHMBO012, to.tertip.TertipStrOl()));
                            break;
                        }

                        double toplam = 0;
                        for (int i = 0; i < obler.Length; i++)
                        {
                            toplam += (double)obler[i].ekRakamlar[0];
                        }

                        TertipOdenek toIc;
                        int odenekSay = to.odenek.Length;
                        double[] icToplam = new double[odenekSay];
                        for (int oNo = 0; oNo < odenekSay; oNo++)
                        {
                            icToplam[oNo] = 0;
                        }

                        for (int i = 0; i < obler.Length - 1; i++)
                        {
                            toIc = new TertipOdenek(odenekSay);
                            toIc.tertip = obler[i].tertip;
                            for (int oNo = 0; oNo < odenekSay; oNo++)
                            {

                                int yuvarla = 0;
                                if (toplam > 0)
                                {
                                    yuvarla = BHMv2.Arac.YuvarlamaTutariBul((double)obler[i].ekRakamlar[0]);
                                    toIc.odenek[oNo] = to.odenek[oNo] * (double)obler[i].ekRakamlar[0] / toplam;
                                }
                                else
                                {
                                    yuvarla = BHMv2.Arac.YuvarlamaTutariBul(to.odenek[oNo] / obler.Length);
                                    toIc.odenek[oNo] = to.odenek[oNo] / obler.Length;
                                }
                                toIc.odenek[oNo] = BHMv2.Arac.YuvarlaAsagi(toIc.odenek[oNo], yuvarla);
                                icToplam[oNo] += toIc.odenek[oNo];
                            }
                            al.Add(toIc);
                        }

                        toIc = new TertipOdenek(odenekSay);
                        toIc.tertip = obler[obler.Length - 1].tertip;

                        for (int oNo = 0; oNo < odenekSay; oNo++)
                        {
                            toIc.odenek[oNo] = to.odenek[oNo] - icToplam[oNo];
                            if (toIc.odenek[oNo] < 0) toIc.odenek[oNo] = 0;
                        }
                        al.Add(toIc);
                    }
                }
                else
                {
                    al.Add(to);
                }
            }
            if (s.islemSonuc)
            {
                tertipOdenekListesi = (TertipOdenek[])al.ToArray(typeof(TertipOdenek));
            }
            return s;
        }

        private static void MebugYaz(string v1, bool v2)
        {
            OrtakFonksiyonlar.HataDosyaYaz("Tertip24.txt", v1, v2);
        }

        public static bool KilitliMiSenaryo(VeriErisim ve, Kullanici kullanan, int yil, string kurumsalKod, string senaryoKod, out string hata, bool sifiraBak)
        {
            string icHata = "";
            hata = "";
            TABBHMODENEKONAY tabBHMONAY = new TABBHMODENEKONAY(ve);
            int sonuc = tabBHMONAY.OnayKod(yil, kurumsalKod, senaryoKod);
            if (!string.IsNullOrEmpty(ve.hata))
            {
                ve.hata = "";
                return true;
            }
            string ek = "";
            if (sonuc <= 1)
                ek = "veri girişine açılmamış!";
            else
                ek = "onaylanmış!";

            icHata = string.Format(" {0} Kurumsal Kodu için {1} senaryosu {2} (Kurum Kontrol={3}, okunan Değer={4})", kurumsalKod, senaryoKod, ek, sifiraBak ? "Evet" : "Hayır", sonuc);
            if (sifiraBak && sonuc == 0)
            {
                hata = icHata;
                return true;
            }
            if (sonuc <= 1)
            {
                return false;
            }
            if (sonuc == 3)
            {
                hata = icHata;
                return true;
            }
            if (kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.SISTEMYONETICISI))
                return false;
            hata = icHata;
            return true;
        }

        internal static Sonuc ButceOdenekOnayla(VeriErisim veriErisim, Kullanici kullanan, string birim, int yil, string asama, int onayDurum)
        {
            if (!kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.SISTEMYONETICISI))
                return new Sonuc(false, ButceHazirlikResI.BHMBTOG001, "");

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, BHMv2Genel.VeriErisimAdBul(yil), kullanan.KimlikBul());
            TABBHMODENEKONAY tabBHMONAY = new TABBHMODENEKONAY(ve);

            ve.BeginTransaction();
            Sonuc sonuc = tabBHMONAY.OnayKodSil(yil, birim, asama);
            if (sonuc.islemSonuc)
                sonuc = tabBHMONAY.OnayKodYaz(yil, birim, asama, onayDurum);

            if (sonuc.islemSonuc)
                ve.Commit();
            else
                ve.Rollback();
            return sonuc;
        }

        /// <summary>
        /// Tertip içi tablo koduna göre, ödenek tablosu nesnesi, veri erişim nesnesi kullanılarak oluşturulur ve döndürülür.
        /// </summary>
        /// <param name="tabloKod">Ödenek tablosu nesnesi oluşturulacak, ödenek tertip türü</param>
        /// <param name="ve">Tablo oluşumunda kullanılacak veri erişim nesnesi</param>
        /// <returns>Ödenek tabloları için ortak tablo nesnesi, verilen tablo koduna göre oluşturulur ve döndürülür.</returns>
        private static TABBHMODENEK OdenekTabloOl(ENUMTabloKod tabloKod, VeriErisim ve)
        {
            if (tabloKod == ENUMTabloKod.TABPEBGelir)
                return new TABBHMODENEKGELIR(ve);
            else if (tabloKod == ENUMTabloKod.TABFinansman)
                return new TABBHMODENEKFINANSMAN(ve);
            else if (tabloKod == ENUMTabloKod.TABPEBTavanUstu)
                return new TABBHMTAVANUSTU(ve);
            else
                return new TABBHMODENEKGIDER(ve);
        }

        /// <summary>
        /// Kullanıcının verilen seanryo ve tertibe ait bilgileri görme yetkisinin olup olmadığı kontrol edilir.
        /// Varsa true yoksa false döndürülür.
        /// <param name="kullanan">Yetkisi kontrol edilecek kullanıcı bilgilerini tutan nesne</param>
        /// <param name="senaryoKod">Kullanıcının görmeye çalıştığı ödeneklerin senaryosu</param>
        /// <param name="kistasTertip">Kullanıcının görmeye çalıştığı tertip</param>
        /// </summary>
        /// <returns>Görme yetkisi varsa true, yoksa false döndürür.</returns>
        public static bool GormeYetkisiVarMi(Kullanici kullanan, string senaryoKod, Tertip kistasTertip)
        {
            return true;
        }

        /// <summary>
        /// Kıstas olarak verilen tertipe uyan, tertipleri (gelir, gider ve finansman) 
        /// ve bu tertiple birlikte, tertip yılı ve senaryo kodu kullanılarak, yıl ve senaryoya ait çok yıllı ödeneklerle birlikte
        /// istenilen ek bilgileri TertipOdenek dizisi şeklinde döndüren yordam.
        /// TertipOdenek nesnesinde ödenek ise 3 + istenilen, tertip ise 1 + istenilen ek bilgi kadar değer oluşacaktır.
        /// Önce tertip listesi alınır, sonrada her tertip için ödenekler bulunur.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler</param>
        /// <param name="senaryoKod">Çok yıllı ödenek değerleri döndürülecek senaryo kodu</param>
        /// <param name="kistasTertip">Listelecek tertiplerin uyacağı kıstasları tutan tertip nesnesi</param>
        /// <param name="ekDegerParamlar">Kullanıcının verdiği senaryoya ait ödeneklerle birlikte döndürülecek ek değerler dizisi</param>
        /// <param name="tahmin">1 ise tahmin bilgileri, değil ise ödenek bilgileri listelenir.</param>
        /// <returns>Verilen kıstas tertibe uygun tertipler, bu tertiplerin verilen senaryoya ait bilgileri ile ek değerleri TertipOdenek dizisinde döndürülür.</returns>
        public static TertipOdenek[] OdenekListele(VeriErisim veriErisim, Kullanici kullanan, string senaryoKod, Tertip kistasTertip, EkDegerParam[] ekDegerParamlar, int tahmin)
        {
            KistasYapi kY = new KistasYapi();
            kY.ekleTertipler = new Tertip[1];
            kY.ekleTertipler[0] = kistasTertip;
            return OdenekListele(veriErisim, kullanan, senaryoKod, kY, ekDegerParamlar, tahmin, null, -1, false);
        }

        /// <summary>
        /// Kıstas olarak verilen tertipe uyan, tertipleri (gelir, gider ve finansman) 
        /// ve bu tertiple birlikte, tertip yılı ve senaryo kodu kullanılarak, yıl ve senaryoya ait çok yıllı ödeneklerle birlikte
        /// istenilen ek bilgileri TertipOdenek dizisi şeklinde döndüren yordam.
        /// TertipOdenek nesnesinde ödenek ise 3 + istenilen, tertip ise 1 + istenilen ek bilgi kadar değer oluşacaktır.
        /// Önce tertip listesi alınır, sonrada her tertip için ödenekler bulunur.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler</param>
        /// <param name="senaryoKod">Çok yıllı ödenek değerleri döndürülecek senaryo kodu</param>
        /// <param name="kistasTertip">Listelecek tertiplerin uyacağı kıstasları tutan tertip nesnesi</param>
        /// <param name="ekDegerParamlar">Kullanıcının verdiği senaryoya ait ödeneklerle birlikte döndürülecek ek değerler dizisi</param>
        /// <param name="tahmin">1 ise tahmin bilgileri, değil ise ödenek bilgileri listelenir.</param>
        /// <returns>Verilen kıstas tertibe uygun tertipler, bu tertiplerin verilen senaryoya ait bilgileri ile ek değerleri TertipOdenek dizisinde döndürülür.</returns>
        public static TertipOdenek[] OdenekListele(VeriErisim veriErisim, Kullanici kullanan, string senaryoKod, Tertip kistasTertip, EkDegerParam[] ekDegerParamlar, int tahmin, UcgenKodBilgileri kodBilgileriGelen)
        {
            KistasYapi kY = new KistasYapi();
            kY.ekleTertipler = new Tertip[1];
            kY.ekleTertipler[0] = kistasTertip;
            return OdenekListele(veriErisim, kullanan, senaryoKod, kY, ekDegerParamlar, tahmin, kodBilgileriGelen, -1, false);
        }

        /// <summary>
        /// Kıstas olarak verilen tertipe uyan, tertipleri (gelir, gider ve finansman) 
        /// ve bu tertiple birlikte, tertip yılı ve senaryo kodu kullanılarak, yıl ve senaryoya ait çok yıllı ödeneklerle birlikte
        /// istenilen ek bilgileri TertipOdenek dizisi şeklinde döndüren yordam.
        /// TertipOdenek nesnesinde ödenek ise 3 + istenilen, tertip ise 1 + istenilen ek bilgi kadar değer oluşacaktır.
        /// Önce tertip listesi alınır, sonrada her tertip için ödenekler bulunur.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler</param>
        /// <param name="senaryoKod">Çok yıllı ödenek değerleri döndürülecek senaryo kodu</param>
        /// <param name="kistasYapi">Listelecek tertiplerin uyacağı kıstasları tutan tertip nesnesi</param>
        /// <param name="ekDegerParamlar">Kullanıcının verdiği senaryoya ait ödeneklerle birlikte döndürülecek ek değerler dizisi</param>
        /// <param name="tahmin">1 ise tahmin bilgileri, değil ise ödenek bilgileri listelenir.</param>
        /// <returns>Verilen kıstas tertibe uygun tertipler, bu tertiplerin verilen senaryoya ait bilgileri ile ek değerleri TertipOdenek dizisinde döndürülür.</returns>
        public static TertipOdenek[] OdenekListele(VeriErisim veriErisim, Kullanici kullanan, string senaryoKod, KistasYapi kistasYapi, EkDegerParam[] ekDegerParamlar, int tahmin, UcgenKodBilgileri kodBilgileriGelen, int aciklamaKolon, bool eko2)
        {
            if (!GormeYetkisiVarMi(kullanan, senaryoKod, kistasYapi.ekleTertipler[0]))
            {
                throw new Exception(ButceHazirlikResI.BHMBO008);
            }

            int ekSay;
            if (tahmin == 1)
                ekSay = 2;
            else
                ekSay = 3;
            UcgenKodBilgileri kodBilgileri;
            if (kodBilgileriGelen == null)
            {
                kodBilgileri = new UcgenKodBilgileri();
                for (int i = 0; i < kistasYapi.ekleTertipler[0].kodlar.Length; i++)
                {
                    int kirSay = kistasYapi.ekleTertipler[0].KodNoKirilimSay(i);
                    if (eko2 && i == 3) kirSay = 2;
                    kodBilgileri.Add(new UcgenKodBilgi(kistasYapi.ekleTertipler[0].KodNolar(i), kirSay));
                }
            }
            else
                kodBilgileri = kodBilgileriGelen;
            EkDegerParam[] ekBilgiler = new EkDegerParam[BirimOdenekGider.EkDegerUzunluk(ekDegerParamlar) + ekSay];
            bool tanimliTertipler;
            int ekli;
            if (tahmin == 0 || tahmin == 2)
            {
                ENUMTabloKod tk = kistasYapi.ekleTertipler[0].tabloKod;
                if (tk == ENUMTabloKod.TABButce) tk = ENUMTabloKod.TABPEBOdenek;
                ekBilgiler[0] = new EkDegerParam(tk, 0, 0, senaryoKod);
                ekBilgiler[1] = new EkDegerParam(tk, 0, 1, senaryoKod);
                ekBilgiler[2] = new EkDegerParam(tk, 0, 2, senaryoKod);
                tanimliTertipler = true;
                ekli = 3;
            }
            else
            {
                ekBilgiler[0] = new EkDegerParam(kistasYapi.ekleTertipler[0].tabloKod, -1, 0, senaryoKod); // Arac.OdenekBul();
                ekli = 1;
                tanimliTertipler = false;
            }
            for (int j = 0; j < BirimOdenekGider.EkDegerUzunluk(ekDegerParamlar); j++)
            {
                ekBilgiler[j + ekli] = ekDegerParamlar[j];
            }
            if (tahmin == 1)
            {
                ENUMTabloKod ic;
                string senveyaAy;
                if (kistasYapi.ekleTertipler[0].tabloKod == ENUMTabloKod.TABGelirTahmini)
                {
                    senveyaAy = "4";
                    ic = ENUMTabloKod.TABGelirGerceklesmesi;
                }
                else
                {
                    senveyaAy = "12;830";
                    ic = ENUMTabloKod.TABHarcama;
                }
                ekBilgiler[ekBilgiler.Length - 1] = new EkDegerParam(ic, -1, 0, senveyaAy);
            }

            OdenekBilgi[] obler = UcgenServisi.ListeTertipOl(null, kullanan, kistasYapi, kodBilgileri, ekBilgiler, false, tanimliTertipler, true, aciklamaKolon);
            if (obler == null)
            {
                throw new Exception(ButceHazirlikResI.BHMBO009);
            }
            TertipOdenek[] tertipOdenekler = new TertipOdenek[obler.Length];

            for (int i = 0; i < obler.Length; i++)
            {
                tertipOdenekler[i] = new TertipOdenek(ekBilgiler.Length - tahmin);
                tertipOdenekler[i].tertip = obler[i].tertip;
                tertipOdenekler[i].aciklama = obler[i].aciklama;

                for (int j = 0; j < ekBilgiler.Length - tahmin; j++)
                {
                    tertipOdenekler[i].odenek[j] = (double)obler[i].ekRakamlar[j];
                }
            }

            return tertipOdenekler;
        }

        public static ObjectVAN OdenekGondermeKontrolMiktarlariniBul(VeriErisim veriErisim, int yil, TertipKodlari kodlar)
        {
            //double miktar = 0.0;
            ObjectVAN miktarlar = new ObjectVAN();
            //VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, BHMv2Genel.VeriErisimAdBulSozluk(), null);
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, BHMv2Genel.VeriErisimAdBul(yil), null);
            //TABBHMODENEK tabOdenek = OdenekTabloOl(ENUMTabloKod.TABPEBOdenek, ve);
            TABBHMODENEKGIDER tabOdenekGider = new TABBHMODENEKGIDER(ve);
            //IDataReader dr = tabOdenek.TertipListesi(yil, senaryoKod, teklifYil, sablon.tertipSablon.Split('-'));
            IDataReader dr = tabOdenekGider.OdenekGondermeKontrolMiktarlariniBul(yil, kodlar);
            if (dr != null)
            {
                if (dr.Read())
                {
                    double[] array = new double[3];
                    array[0] = VeriErisim.DegerAl(dr, 0, 0.0);
                    array[1] = VeriErisim.DegerAl(dr, 1, 0.0);
                    array[2] = VeriErisim.DegerAl(dr, 2, 0.0);

                    miktarlar.o = array;
                }
                dr.Close();
            }
            ve.VeriErisimKapat();

            return miktarlar;
        }

        /// <summary>
        /// ButceTasnifTertipSablon yapısında verilmiş olan sablonlar dizisindeki şablonlara uyan
        /// tertipler ve verilen yıl ve senaryoya ait ödeneğini liste halinde döndürülür.
        /// Ödeneklerin toplamı da toplam parametresinde döndürülür.
        /// Şablondaki eklenecek tertipler bulunur, daha sonra çıkartılması gerekenler sonuç kümesinden çıkartılır.
        /// Bu yordam, ödenek üretiminde kullanılır. Tertip listesi, verilen yönteme göre elde edilir. (Ödenek veya harcamadan)
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="butceYil">Bağlanılacak veri tabanını gösteren, bütçe yılı</param>
        /// <param name="yil">Tertip listesi bulunacak yıl</param>
        /// <param name="senaryoKodveyaAy">Ödeneği bulunacak senaryo kodu</param>
        /// <param name="teklifYil">Çok yıllı bütçeden, hangi yıla ait ödeneği listeleneceği (0, 1, 2)</param>
        /// <param name="sablonlar">Listede bulunacak tertiplerin hangileri olacağını tutan şablon dizisi</param>
        /// <param name="metod">metod 1: Ödenek , metod 2: Harcama</param>
        /// <param name="toplam">Listelenen tertiplerin ödeneklerinin toplamı bu parametrede döndürülür.</param>
        /// <returns>Verilen şablona uyan tertiplerin listesi ve verilen yıl ve senaryoya uyan ödeneği döndürülür.</returns>
        public static TertipOdenek[] TertipListesiBul(VeriErisim veriErisim, int butceYil, int yil, string senaryoKodveyaAy, int teklifYil, ButceTasnifTertipSablon[] sablonlar, ENUMUretimKaynakTur metod, out double toplam)
        {
            ArrayList al = new ArrayList();

            double tmpToplam = 0;
            foreach (ButceTasnifTertipSablon sablon in sablonlar)
            {
                if (sablon.ekleCikar == TertipEkleCikar.Ekle)
                {
                    switch (metod)
                    {
                        case ENUMUretimKaynakTur.Odenek: BHMOdenekDenAl(al, veriErisim, butceYil, yil, senaryoKodveyaAy, teklifYil, sablon, ref tmpToplam); break;
                        case ENUMUretimKaynakTur.Muhasebe: MUHHarcamadanAl(al, veriErisim, butceYil, yil, senaryoKodveyaAy, sablon, ref tmpToplam); break;
                        case ENUMUretimKaynakTur.OdemeEmri: OdemeEmrindenAl(al, veriErisim, butceYil, yil, sablon, ref tmpToplam); break;
                    }
                }
                else if (sablon.ekleCikar == TertipEkleCikar.Cikar)
                {
                    for (int i = 0; i < al.Count; i++)
                    {
                        Tertip kiyasTertip = new Tertip(yil, TNS.BKD.ENUMTabloKod.TABPEBOdenek);
                        kiyasTertip.TertipAta(sablon.tertipSablon);
                        if (((TertipOdenek)al[i]).tertip.Kiyasla(kiyasTertip))
                        {
                            tmpToplam -= ((TertipOdenek)al[i]).odenek[0];
                            al.RemoveAt(i);
                            i--;
                        }
                    }
                }
            }

            toplam = tmpToplam;
            return (TertipOdenek[])al.ToArray(typeof(TertipOdenek));
        }

        /// <summary>
        /// ButceTasnifTertipSablon yapısında verilmiş olan şablona uyan tertipler ödenek tablosundan alınır.
        /// Verilen yıl ve çok yıllı teklif yılı parametresi kullanılarak tertibin ödeneği bulunur.
        /// Ödeneklerin toplamı da toplam parametresinde döndürülür.
        /// </summary>
        /// <param name="dizi">Bulunan tertiplerin ekleneceği dizi</param>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="butceYil">Bağlanılacak veri tabanını gösteren, bütçe yılı</param>
        /// <param name="yil">Tertip listesi bulunacak yıl</param>
        /// <param name="senaryoKod">Ödeneği bulunacak senaryo kodu</param>
        /// <param name="teklifYil">Çok yıllı bütçeden, hangi yıla ait ödeneği listeleneceği (0, 1, 2)</param>
        /// <param name="sablon">Listelenecek tertiplerin uyacağı şablon</param>
        /// <param name="toplam">Listelenen tertiplerin ödeneklerinin toplamı bu parametrede döndürülür.</param>
        private static void BHMOdenekDenAl(ArrayList dizi, VeriErisim veriErisim, int butceYil, int yil, string senaryoKod, int teklifYil, ButceTasnifTertipSablon sablon, ref double toplam)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, BHMv2Genel.VeriErisimAdBul(butceYil), null);
            TABBHMODENEK tabOdenek = OdenekTabloOl(ENUMTabloKod.TABPEBOdenek, ve);

            IDataReader dr = tabOdenek.TertipListesi(yil, senaryoKod, teklifYil, sablon.tertipSablon.Split('-'));

            if (dr != null)
            {
                while (dr.Read())
                {
                    TertipOdenek to = new TertipOdenek();
                    to.tertip = new TNS.BKD.Tertip(yil, TNS.BKD.ENUMTabloKod.TABPEBOdenek);
                    to.tertip.TertipAta(VeriErisim.DegerAl(dr, (int)TABBHMODENEKGIDER.ENUMAlanlar.KURKOD, string.Empty) + "-" +
                        VeriErisim.DegerAl(dr, (int)TABBHMODENEKGIDER.ENUMAlanlar.FONKOD, string.Empty) + "-" +
                        VeriErisim.DegerAl(dr, (int)TABBHMODENEKGIDER.ENUMAlanlar.FINKOD, string.Empty) + "-" +
                        VeriErisim.DegerAl(dr, (int)TABBHMODENEKGIDER.ENUMAlanlar.EKOKOD, string.Empty) + "-" +
                    VeriErisim.DegerAl(dr, (int)TABBHMODENEKGIDER.ENUMAlanlar.PEBKOD, string.Empty));
                    to.tertip.Noktala();
                    to.odenek[0] = VeriErisim.DegerAl(dr, (int)TABBHMODENEKGIDER.ENUMAlanlar.ODENEK, 0.0);
                    toplam += to.odenek[0];
                    dizi.Add(to);
                }
                dr.Close();
            }
            ve.VeriErisimKapat();
        }

        /// <summary>
        /// ButceTasnifTertipSablon yapısında verilmiş olan şablona uyan tertipler ödenek tablosundan alınır.
        /// Verilen yıl ve çok yıllı teklif yılı parametresi kullanılarak tertibin ödeneği bulunur.
        /// Ödeneklerin toplamı da toplam parametresinde döndürülür.
        /// </summary>
        /// <param name="dizi">Bulunan tertiplerin ekleneceği dizi</param>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="butceYil">Bağlanılacak veri tabanını gösteren, bütçe yılı</param>
        /// <param name="yil">Tertip listesi bulunacak yıl</param>
        /// <param name="ay">Ödeneği bulunacak ay</param>
        /// <param name="sablon">Listelenecek tertiplerin uyacağı şablon</param>
        /// <param name="toplam">Listelenen tertiplerin ödeneklerinin toplamı bu parametrede döndürülür.</param>
        private static void MUHHarcamadanAl(ArrayList dizi, VeriErisim veriErisim, int butceYil, int yil, string ay, ButceTasnifTertipSablon sablon, ref double toplam)
        {
            Tertip kistasTertip = new Tertip(yil, ENUMTabloKod.TABHarcama, sablon.tertipSablon.Split('-'));
            UcgenKodBilgileri ukb = new UcgenKodBilgileri();
            ukb.Add(new UcgenKodBilgi(kistasTertip.KodNolar(0), kistasTertip.KodNoKirilimSay(0)));
            ukb.Add(new UcgenKodBilgi(kistasTertip.KodNolar(1), kistasTertip.KodNoKirilimSay(1)));
            ukb.Add(new UcgenKodBilgi(kistasTertip.KodNolar(2), kistasTertip.KodNoKirilimSay(2)));
            ukb.Add(new UcgenKodBilgi(kistasTertip.KodNolar(3), kistasTertip.KodNoKirilimSay(3)));

            EkDegerParam[] ekB = new EkDegerParam[1];
            ekB[0] = new EkDegerParam(ENUMTabloKod.TABHarcama, 0, 0, ay);
            OdenekBilgi[] obler = UcgenServisi.ListeTertipOl(null, null, kistasTertip, ukb, ekB, true, false, true, -1);

            if (obler != null)
            {
                foreach (OdenekBilgi ob in obler)
                {
                    TertipOdenek to = new TertipOdenek();
                    to.tertip = ob.tertip;
                    to.odenek[0] = (double)ob.ekRakamlar[0];
                    toplam += to.odenek[0];
                    dizi.Add(to);
                }
            }
        }

        /// <summary>
        /// ButceTasnifTertipSablon yapısında verilmiş olan şablona uyan tertipler ödenek tablosundan alınır.
        /// Verilen yıl ve çok yıllı teklif yılı parametresi kullanılarak tertibin ödeneği bulunur.
        /// Ödeneklerin toplamı da toplam parametresinde döndürülür.
        /// </summary>
        /// <param name="dizi">Bulunan tertiplerin ekleneceği dizi</param>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="butceYil">Bağlanılacak veri tabanını gösteren, bütçe yılı</param>
        /// <param name="yil">Tertip listesi bulunacak yıl</param>
        /// <param name="ay">Ödeneği bulunacak ay</param>
        /// <param name="sablon">Listelenecek tertiplerin uyacağı şablon</param>
        /// <param name="toplam">Listelenen tertiplerin ödeneklerinin toplamı bu parametrede döndürülür.</param>
        private static void OdemeEmrindenAl(ArrayList dizi, VeriErisim veriErisim, int butceYil, int yil, ButceTasnifTertipSablon sablon, ref double toplam)
        {
            Tertip kistasTertip = new Tertip(yil, ENUMTabloKod.TABPEBOdenek, sablon.tertipSablon.Split('-'));
            UcgenKodBilgileri ukb = new UcgenKodBilgileri();
            ukb.Add(new UcgenKodBilgi(kistasTertip.KodNolar(0), kistasTertip.KodNoKirilimSay(0)));
            ukb.Add(new UcgenKodBilgi(kistasTertip.KodNolar(1), kistasTertip.KodNoKirilimSay(1)));
            ukb.Add(new UcgenKodBilgi(kistasTertip.KodNolar(2), kistasTertip.KodNoKirilimSay(2)));
            ukb.Add(new UcgenKodBilgi(kistasTertip.KodNolar(3), kistasTertip.KodNoKirilimSay(3)));
            ukb.Add(new UcgenKodBilgi(kistasTertip.KodNolar(4), kistasTertip.KodNoKirilimSay(4)));

            EkDegerParam[] ekB = new EkDegerParam[1];
            ekB[0] = new EkDegerParam(ENUMTabloKod.TABOdemeEmri, 0, 0, "");
            OdenekBilgi[] obler = UcgenServisi.ListeTertipOl(null, null, kistasTertip, ukb, ekB, true, false, true, -1);

            if (obler != null)
            {
                foreach (OdenekBilgi ob in obler)
                {
                    TertipOdenek to = new TertipOdenek();
                    to.tertip = ob.tertip;
                    to.odenek[0] = (double)ob.ekRakamlar[0];
                    toplam += to.odenek[0];
                    dizi.Add(to);
                }
            }
        }

        /// <summary>
        /// Verilen tasnife girilen ödenek rakamı kullanılarak, tasnifin kaynağı olarak belirtilen veriler bulunur
        /// ve tasnifte verilen rakama getirilmek üzere her tertip orantılanarak değiştirilir.
        /// Elde edilen tertip ve rakamlar hedef senaryoya yazılır.
        /// Verilen senaryo kodu kullanılarak, kaynak senaryo ve kaynak rakamlar bulunur.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler</param>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="hedefYil">Üretilen verilerin yazılacağı yıl</param>
        /// <param name="hedefSenaryoKod">Üretilen verilerin yazılacağı senaryo</param>
        /// <param name="tasnifSiraNo">Hedef rakamın bulunacağı tasnifinin sıra numarası</param>
        /// <param name="hiyerarsiKod">İşlem yapılacak tasnifin ilgili satırı</param>
        /// <param name="kaynakTur">kaynakTur 1: Ödenek , kaynakTur 2: Harcama</param>
        /// <param name="kaynakYil">Kaynak tertiplerin bulunacağı yıl</param>
        /// <param name="kaynakSenaryoKodveyaAy">Kaynak tertiplerin ödeneklerinin bulunacağı senaryo kodu</param>
        /// <param name="sadeceKontrol">İşlemin yapılıp yapılmayacağı. True ise sadece kontrol yapılır işlem yapılmaz.</param>
        /// <returns>İşlem sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public static Sonuc TasnifdenGiderOdenekOlustur(Kullanici kullanan, VeriErisim veriErisim, int hedefYil, string hedefSenaryoKod, int tasnifSiraNo, string hiyerarsiKod, ENUMUretimKaynakTur kaynakTur, int kaynakYil, string kaynakSenaryoKodveyaAy, bool sadeceKontrol)
        {
            string kontrolHata = "";
            string kontrolBilgi = "";
            ButceTasnifOdenekleriFormu tasnifForm = new ButceTasnifOdenekleriFormu();
            tasnifForm.tasnif.siraNo = tasnifSiraNo;
            tasnifForm.yil = hedefYil;
            IUZYServis uzyServis = TNS.UZY.Arac.Tanimla();
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, BHMv2Genel.VeriErisimAdBul(tasnifForm.yil), kullanan.KimlikBul());
            ButceTasnifOdenekleriFormu btd = ButceTasnifOdenekGiris.Oku(ve, kullanan, tasnifForm, null, hedefSenaryoKod, hiyerarsiKod, "");
            string dondurStr = string.Empty;
            ButceTasnifTertipSablon[] btt = null;
            if (btd.butceTasnifSatirOdenekleri != null && btd.butceTasnifSatirOdenekleri.Length == 1 && btd.butceTasnifSatirOdenekleri[0].hiyerarsi != null)
                btt = btd.butceTasnifSatirOdenekleri[0].hiyerarsi.tertipSablonlar;
            if (btt != null && btt.Length > 0)
            {
                EkDegerParam ekDegerParam = new EkDegerParam(ENUMTabloKod.TABPEBOdenek, 0, 0, hedefSenaryoKod);

                double HedefDeger = ButceTasnifOdenekGiris.EkDegerHesapla(kullanan, uzyServis, hedefYil, btt, ekDegerParam);

                if (sadeceKontrol)
                {
                    if (HedefDeger != 0)
                        kontrolBilgi += "<br>" + btd.butceTasnifSatirOdenekleri[0].hiyerarsi.ad + " (" + HedefDeger.ToString("#,##0") + ")";
                }
                else
                {
                    double toplam = 0;
                    TertipOdenek[] tolist = TertipListesiBul(ve, hedefYil, kaynakYil, kaynakSenaryoKodveyaAy, 0, btt, kaynakTur, out toplam);

                    if (toplam > 0)
                    {
                        double katsayi = btd.butceTasnifSatirOdenekleri[0].ekDegerler[0] / toplam;
                        toplam = 0;
                        foreach (TertipOdenek to in tolist)
                        {
                            int yuvarla = TNS.BHMv2.Arac.YuvarlamaTutariBul(to.odenek[0]);
                            to.tertip.yil = hedefYil;
                            to.odenek[0] = TNS.BHMv2.Arac.Yuvarla(to.odenek[0] * katsayi, yuvarla);
                            toplam += to.odenek[0];
                        }
                        tolist[tolist.Length - 1].odenek[0] += btd.butceTasnifSatirOdenekleri[0].ekDegerler[0] - toplam;
                        if (tolist[tolist.Length - 1].odenek[0] < 0) tolist[tolist.Length - 1].odenek[0] = 0;
                        Sonuc s = BHMv2ButceOdenek.Kaydet(ve, kullanan, hedefSenaryoKod, tolist, false);
                        if (s.islemSonuc)
                        {
                            dondurStr += "<br>" + string.Format(ButceHazirlikResI.BHMBO013, btd.butceTasnifSatirOdenekleri[0].hiyerarsi.ad);
                        }
                        else
                            dondurStr += "<br>" + string.Format(ButceHazirlikResI.BHMBO014, btd.butceTasnifSatirOdenekleri[0].hiyerarsi.ad, s.hataStr);
                    }
                    else
                    {
                        dondurStr += "<br>" + string.Format(ButceHazirlikResI.BHMBO015, btd.butceTasnifSatirOdenekleri[0].hiyerarsi.ad);
                    }
                }
            }
            else
            {
                if (sadeceKontrol)
                    kontrolHata += "<br>" + string.Format(ButceHazirlikResI.BHMBO017, btd.butceTasnifSatirOdenekleri[0].hiyerarsi.ad);
                else
                    dondurStr += "<br>" + string.Format(ButceHazirlikResI.BHMBO017, btd.butceTasnifSatirOdenekleri[0].hiyerarsi.ad);
            }
            ve.VeriErisimKapat();

            if (sadeceKontrol)
                return new Sonuc(kontrolHata == "", kontrolHata, kontrolBilgi);
            else
                return new Sonuc(true, string.Empty, dondurStr);
        }

        /// <summary>
        /// Verilen tasnife girilen ödenek rakamı kullanılarak, tasnifin kaynağı olarak belirtilen veriler bulunur
        /// ve tasnifte verilen rakama getirilmek üzere her tertip orantılanarak değiştirilir.
        /// Elde edilen tertip ve rakamlar hedef senaryoya yazılır.
        /// Verilen senaryo kodu kullanılarak, kaynak senaryo ve kaynak rakamlar bulunur.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler</param>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="hedefYil">Üretilen verilerin yazılacağı yıl</param>
        /// <param name="hedefSenaryoKod">Üretilen verilerin yazılacağı senaryo</param>
        /// <returns>İşlem sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public static Sonuc TasnifdenGiderOdenekOlustur(Kullanici kullanan, VeriErisim veriErisim, int hedefYil, string hedefSenaryoKod)
        {
            ButceSenaryo bs = new ButceSenaryo();
            bs.kod = hedefSenaryoKod;
            ButceSenaryo[] bsler = BHMv2ButceSenaryo.Listele(null, kullanan, bs);
            if (bsler != null && bsler.Length >= 1)
            {
                return TasnifdenGiderOdenekOlustur(kullanan, veriErisim, hedefYil, hedefSenaryoKod, 0, hedefSenaryoKod, ENUMUretimKaynakTur.Odenek, hedefYil - 1, "06.00", false);
            }
            else
                return new Sonuc(false, ButceHazirlikResI.BHMBO010, string.Empty);
        }

        internal static Sonuc ButceOdenekKopyala(VeriErisim veriErisim, Kullanici kullanan, Tertip hedefTertip, string hedefAsama, int kaynakYil, string kaynakAsama)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, BHMv2Genel.VeriErisimAdBul(hedefTertip.yil), kullanan.KimlikBul());
            TABBHMODENEK to = OdenekTabloOl(hedefTertip.tabloKod, ve);
            //ve.BeginTransaction();
            Sonuc s = to.OdenekKopyala(hedefTertip, hedefAsama, kaynakYil, kaynakAsama);
            //if (s.islemSonuc)
            //{
            //    //ve.Commit();
            //    s.bilgiStr = string.Format(ButceHazirlikResI.BHMBO018, s.kayitSay);
            //}
            //else
            //    ve.Rollback();
            return s;
        }

        internal static Hashtable ProgramButceCevrimOku(VeriErisim veriErisim, int yil)
        {
            Hashtable ht = new Hashtable();
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, BHMv2Genel.VeriErisimAdBul(DateTime.Now.Year), "sistem");

            TABBHMODENEKGIDER od = new TABBHMODENEKGIDER(ve);

            IDataReader dr = od.ProgramButceCevrimOku(yil);
            if (dr != null)
            {
                while (dr.Read())
                {
                    int kodNo = VeriErisim.DegerAl(dr, 0, 0);
                    string absKod = VeriErisim.DegerAl(dr, 1, "");
                    string prgKod = VeriErisim.DegerAl(dr, 2, "");
                    ht[kodNo + "_" + absKod] = prgKod;
                }
                dr.Close();
            }
            return ht;
        }
    }
}
