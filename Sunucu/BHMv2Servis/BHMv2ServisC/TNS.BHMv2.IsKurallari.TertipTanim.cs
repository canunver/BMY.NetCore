using System;
using System.Data;
using TNS;
using TNS.Veri;
using TNS.BHMv2;
using TNS.BHMv2.IsKurallari.Veri;
using TNS.KYM;
using System.Collections;
using TNS.BKD;

namespace TNS.BHMv2.IsKurallari
{
    /// <summary>
    /// Gelir ve gider tertip tanımlarının yapılmasıyla ilgili, iş kuralları kontrollerini yapıp,
    /// veri tabanı bağlantısını alıp, veri tabanı fonksiyonlarını çağırarak
    /// güncelleme işlemlerini gerçekleştiren fonksiyonları tutan sınıf.
    /// </summary>
    public class TertipTanim
    {
        /// <summary>
        /// Kullanıcının, tertip tanım bilgileri üzerinde değişiklik yapma hakkını bulup döndüren yordam.
        /// Kullanıcı sistem yöndetici ise değişiklik yapma hakkı vardır.
        /// </summary>
        /// <param name="kullanan">Yetki kontrolü yapılacak kullanıcıya ait bilgiler</param>
        /// <returns>Değişiklik yapma hakkı varsa doğru, yoksa yanlış döndürülür.</returns>
        public static bool YazmaYetkisiVarMi(Kullanici kullanan)
        {
            if (kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.SISTEMYONETICISI))
                return true;
            else
                return false;
        }

        /// <summary>
        /// Verilen alt faaliyet nesnesindeki alanların tutarlılığını kontrol eden fonksiyon
        /// <param name="kullanan">Alan tutarlılığı kontrol edilirken, kullanıcılara göre farklılık olacaksa, kullanıcı bilgisini tutan nesne</param>
        /// <param name="tertip">Alan tutarlılığı kontrol edilecek tertip nesnesi</param>
        /// </summary>
        /// <returns>Alanlar tutarlı ise, Sonuc yapısında true, değilse false ve açıklaması döndürülür.</returns>
        public static Sonuc AlanlarGecerliMi(Kullanici kullanan, Tertip tertip)
        {
            return new Sonuc(true, string.Empty, string.Empty);
        }

        /// <summary>
        /// Bütçe hazırlık işlemleri sırasında kullanılan gelir ve gider tertipleri en alt düzeyde sistemde tanımlanmış olmalıdır.
        /// Verilen tertip dizisindeki, tertip bilgileri sisteme kaydedilir.
        /// Parametre olarak verilen dizide, tertip adı boş olan tertipler, tertip havuzundan silinir.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler</param>
        /// <param name="tertipler">GiderTertipListe yapısında GiderTertipListe: bilgilerini tutan değişken</param>
        /// <returns>İşlem sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public static Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, Tertip[] tertipler)
        {
            Sonuc s = new Sonuc(true, string.Empty, string.Empty);
            if (!YazmaYetkisiVarMi(kullanan))
            {
                s.DegerAta(ButceHazirlikResI.BHMTT001);
                return s;
            }
            Tertip ilk = null;
            if (tertipler.Length == 0)
                s.DegerAta("Kaydedilecek tertip tanımı bulunamadı!");
            else
            {
                ilk = tertipler[0];
                s = AlanlarGecerliMi(kullanan, ilk);
            }
            if (!s.islemSonuc)
            {
                return s;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, BHMv2Genel.VeriErisimAdBul(ilk.yil), kullanan.KimlikBul());
            TABBHMTERTIPLISTESI tabTertipler = TabloOl(ilk.tabloKod, ve);

            TNS.UZY.IUZYServis uzyServis = TNS.UZY.Arac.Tanimla();
            string hataKodAd, hataKod;
            Hashtable[] hashTable = new Hashtable[ilk.KodNoSay()];
            for (int i = 0; i < hashTable.Length; i++)
            {
                hashTable[i] = new Hashtable();
            }
            Hashtable fonKodHash = new Hashtable();

            for (int i = 0; i < tertipler.Length && s.islemSonuc; i++)
            {
                if (!string.IsNullOrEmpty(tertipler[i].tertipAdi))
                {
                    tertipler[i].AdlariBul(uzyServis, hashTable);
                    if (!tertipler[i].IsimlerTamMi(out hataKodAd, out hataKod))
                    {
                        s.DegerAta(string.Format(ButceHazirlikResI.BHMTT002, (i + 1), tertipler[i].TertipStrOl(), hataKodAd, hataKod));
                    }
                    else
                    {
                        FonkodUyumKontrolu(ve, kullanan, tertipler[i], s, fonKodHash);
                    }
                }
                else //Silinecek. Kayıt olmaması lazım.
                {
                    int say = tabTertipler.KayitSayisiBul(tertipler[i]);
                    if (say != 0)
                    {
                        s.DegerAta(string.Format(ButceHazirlikResI.BHMTT003, (i + 1), tertipler[i].TertipStrOl()));
                    }
                }

                if (!tertipler[i].TamMi())
                    s.DegerAta(string.Format(ButceHazirlikResI.BHMTT004, (i + 1), tertipler[i].TertipStrOl()));
            }
            if (s.islemSonuc)
            {
                ve.BeginTransaction();
                try
                {
                    for (int i = 0; i < tertipler.Length && s.islemSonuc; i++)
                    {
                        s = tabTertipler.Sil(tertipler[i]);
                        if (s.islemSonuc && !string.IsNullOrEmpty(tertipler[i].tertipAdi))
                            s = tabTertipler.Ekle(tertipler[i]);
                        if (!s.islemSonuc)
                            break;
                    }
                }
                catch (Exception ex)
                {
                    s.DegerAta(ex.Message);
                }
                if (s.islemSonuc)
                    ve.Commit();
                else
                    ve.Rollback();
            }

            ve.VeriErisimKapat();
            return s;
        }

        private static void FonkodUyumKontrolu(VeriErisim ve, Kullanici kullanan, Tertip tertip, Sonuc s, Hashtable hashTable)
        {
            if (tertip.tabloKod == ENUMTabloKod.TABPEBOdenek)
            {
                TNS.BKD.IBKDServis muhServis = TNS.BKD.Arac.Tanimla();
                string tertipFinKod = tertip.kodlar[2];
                if (!YikobMu(tertipFinKod))
                {
                    string tertipFonKod = tertip.kodlar[1];
                    string kod = tertip.kodlar[tertip.kodlar.Length - 1];
                    string kistasFonKod;
                    if (!hashTable.ContainsKey(kod))
                    {
                        kistasFonKod = "";

                        ObjectArray oa = muhServis.PrgPebIliskiListele(kullanan, new PrgPebIliski() { yil = tertip.yil, pebKod = kod });
                        if (oa.sonuc.islemSonuc && oa.ObjeSayisi == 1)
                        {
                            kistasFonKod = ((PrgPebIliski)oa.objeler[0]).fonKod;
                        }
                        hashTable.Add(kod, kistasFonKod);
                    }
                    else
                        kistasFonKod = (string)hashTable[kod];
                    if (!string.IsNullOrWhiteSpace(kistasFonKod) && !tertipFonKod.StartsWith(kistasFonKod))
                        s.DegerAta($"Fonksiyonel kodu {tertipFonKod} olan {kod} alt faaliyet kodlu tertibin, fonksiyonel kodu {kistasFonKod} ile uyumlu olmalıdır!");
                }
            }
        }

        public static bool YikobMu(string tertipFinKod)
        {
            return (tertipFinKod == "09" || tertipFinKod == "16");
        }

        /// <summary>
        /// Bütçe hazırlık işlemleri sırasında kullanılan gelir ve gider tertipleri en alt düzeyde sistemde tanımlanmış olmalıdır.
        /// Tanımlı tertiplerden verilen tertip kıstasına uygun olanlarının listesini dizi olarak döndüren yordam.
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler</param>
        /// <param name="kistasTertip">Listelecek tertiplerin uyacağı kıstasları tutan tertip nesnesi</param>
        /// <param name="siralamaTur">Parametre değeri, 1 ise ekonomik kod, 2 ise kurkd ve ekonomik koda göre sıralama yapılır.</param>
        /// <param name="tertipDurumlar">Listelecek tertiplerin uyacağı durumların listesi. Tertip bu durumlardan birinde ise döndürülür.</param>
        /// <returns>Verilen tertip kıstasına uyan tertip bilgileri dizi olarak döndürülür.</returns>
        public static Tertip[] Listele(VeriErisim veriErisim, Kullanici kullanan, Tertip kistasTertip, int siralamaTur, ENUMTertipDurum[] tertipDurumlar, bool fonkodUyumsuz = false)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, BHMv2Genel.VeriErisimAdBul(kistasTertip.yil), kullanan == null ? null : kullanan.KimlikBul());
            VeriErisim veFon = VeriErisim.VeriErisimBul(null, BHMv2Genel.VeriErisimAdBul(kistasTertip.yil), kullanan == null ? null : kullanan.KimlikBul());
            TABBHMTERTIPLISTESI tabTertipler = TabloOl(kistasTertip.tabloKod, ve);

            IDataReader dr;
            ArrayList al;
            dr = tabTertipler.Listele(kistasTertip, siralamaTur, tertipDurumlar);
            if (dr == null)
            {
                ve.VeriErisimKapat();
                throw new Exception(ve.hata);
            }
            al = new ArrayList();
            Hashtable fonkodHash = new Hashtable();
            while (dr.Read())
            {
                Tertip detay = new Tertip(kistasTertip.yil, kistasTertip.tabloKod);
                if (tabTertipler is TABBHMTERTIPLISTESIGIDER)
                {
                    detay.TertipAta(VeriErisim.DegerAl(dr, (int)TABBHMTERTIPLISTESIGIDER.ENUMAlanlar.KURKOD, string.Empty)
                                  , VeriErisim.DegerAl(dr, (int)TABBHMTERTIPLISTESIGIDER.ENUMAlanlar.FONKOD, string.Empty)
                                  , VeriErisim.DegerAl(dr, (int)TABBHMTERTIPLISTESIGIDER.ENUMAlanlar.FINKOD, string.Empty)
                                  , VeriErisim.DegerAl(dr, (int)TABBHMTERTIPLISTESIGIDER.ENUMAlanlar.EKOKOD, string.Empty)
                                  , VeriErisim.DegerAl(dr, (int)TABBHMTERTIPLISTESIGIDER.ENUMAlanlar.PEBKOD, string.Empty));

                    detay.tertipAdi = VeriErisim.DegerAl(dr, (int)TABBHMTERTIPLISTESIGIDER.ENUMAlanlar.AD, string.Empty);
                    detay.durum = (ENUMTertipDurum)VeriErisim.DegerAl(dr, (int)TABBHMTERTIPLISTESIGIDER.ENUMAlanlar.DURUM, 0);
                }
                else if (tabTertipler is TABBHMTERTIPLISTESIGELIR)
                {
                    detay.TertipAta(VeriErisim.DegerAl(dr, (int)TABBHMTERTIPLISTESIGELIR.ENUMAlanlar.KURKOD, string.Empty)
                                  , VeriErisim.DegerAl(dr, (int)TABBHMTERTIPLISTESIGELIR.ENUMAlanlar.EKOKOD, string.Empty)
                                  , VeriErisim.DegerAl(dr, (int)TABBHMTERTIPLISTESIGELIR.ENUMAlanlar.PEBKOD, string.Empty));

                    detay.tertipAdi = VeriErisim.DegerAl(dr, (int)TABBHMTERTIPLISTESIGELIR.ENUMAlanlar.AD, string.Empty);
                    detay.durum = (ENUMTertipDurum)VeriErisim.DegerAl(dr, (int)TABBHMTERTIPLISTESIGELIR.ENUMAlanlar.DURUM, 0);
                }
                detay.Noktala();
                bool ekle;
                if (fonkodUyumsuz)
                {
                    Sonuc snc = new Sonuc();
                    FonkodUyumKontrolu(veFon, kullanan, detay, snc, fonkodHash);
                    ekle = !snc.islemSonuc;
                }
                else
                    ekle = true;
                if (ekle)
                    al.Add(detay);
            }
            dr.Close();
            ve.VeriErisimKapat();
            veFon.VeriErisimKapat();
            return (Tertip[])al.ToArray(typeof(Tertip));
        }

        /// <summary>
        /// Bütçe hazırlık işlemleri sırasında kullanılan gelir ve gider tertipleri en alt düzeyde sistemde tanımlanmış olmalıdır.
        /// Tanımlı tertiplerden verilen tertip kıstasına uygun olanlarının listesini dizi olarak döndüren yordam.
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler</param>
        /// <param name="kistasTertip">Listelecek tertiplerin uyacağı kıstasları tutan tertip nesnesi</param>
        /// <param name="kodBilgileri">Tertip sonuç listesi üretilecekken, sonuç listesinde oluşacak boyutların kodları, sırası ve kırılımlarını tutan dizi.</param>
        /// <returns>Verilen tertip kıstasına uyan tertip bilgileri dizi olarak döndürülür.</returns>
        public static string[] Listele(VeriErisim veriErisim, Kullanici kullanan, Tertip kistasTertip, UcgenKodBilgileri kodBilgileri)
        {
            string[] degerMaske = new string[kodBilgileri.BoyutSayisi];
            KistasYapi kY = new KistasYapi();
            kY.ekleTertipler = new Tertip[1];
            kY.ekleTertipler[0] = kistasTertip;
            string alanlar = UcgenServisi.UcgenKodBilgileriSorguAlanlarOl(kistasTertip.yil, kodBilgileri, ref degerMaske, EkDegerParam.TabloAdiEkleBul(kistasTertip.tabloKod), kistasTertip.tabloKod);

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, BHMv2Genel.VeriErisimAdBul(kistasTertip.yil), kullanan == null ? null : kullanan.KimlikBul());
            TABBHMTERTIPLISTESI tabTertipler = TabloOl(kistasTertip.tabloKod, ve);

            IDataReader dr = tabTertipler.FarkliTertipListele(alanlar, kistasTertip, kodBilgileri.BoyutSayisi);
            if (dr == null)
            {
                ve.VeriErisimKapat();
                throw new Exception(ve.hata);
            }
            ArrayList al = new ArrayList();
            while (dr.Read())
            {
                string trtpStr = "";
                for (int i = 0; i < kodBilgileri.BoyutSayisi; i++)
                {
                    string tut = VeriErisim.DegerAl(dr, i, string.Empty);
                    tut = OrtakFonksiyonlar.KodNoktaKoy(degerMaske[i], tut);
                    if (i != 0) trtpStr += "-";
                    trtpStr += tut;
                }
                al.Add(trtpStr);
            }
            dr.Close();
            ve.VeriErisimKapat();

            return (string[])al.ToArray(typeof(string));
        }

        /// <summary>
        /// Tertip içi tablo koduna göre, tertip listesi tablosu nesnesi, veri erişim nesnesi kullanılarak oluşturulur ve döndürülür.
        /// </summary>
        /// <param name="tabloKod">Tertip listesi tablosu nesnesi oluşturulacak, ödenek tertip türü</param>
        /// <param name="ve">Tablo oluşumunda kullanılacak veri erişim nesnesi</param>
        /// <returns>Tertip listesi tabloları için ortak tablo nesnesi, verilen tablo koduna göre oluşturulur ve döndürülür.</returns>
        private static TABBHMTERTIPLISTESI TabloOl(ENUMTabloKod tabloKod, VeriErisim ve)
        {
            if (tabloKod == ENUMTabloKod.TABPEBGelir)
                return new TABBHMTERTIPLISTESIGELIR(ve);
            else
                return new TABBHMTERTIPLISTESIGIDER(ve);
        }

        public static Tertip[] TertipOdenekAlacaklarListele(VeriErisim veriErisim, Kullanici kullanan, Tertip tertip, bool kayitli)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, BHMv2Genel.VeriErisimAdBul(tertip.yil), kullanan.KimlikBul());
            TABBHMTERTIPLISTESIGIDER tab = new TABBHMTERTIPLISTESIGIDER(ve);
            IDataReader dr = tab.OdenekAlacaklariListele(tertip);
            if (dr == null)
            {
                ve.VeriErisimKapat();
                throw new Exception(ve.hata);
            }
            ArrayList al = new ArrayList();
            while (dr.Read())
            {
                Tertip t = new Tertip(tertip.yil, tertip.tabloKod);
                t.kodlar[0] = VeriErisim.DegerAl(dr, (int)TABBHMTERTIPLISTESIGIDER.ENUMAlanlarALACAK.ALKURKOD, "");
                t.kodlar[1] = VeriErisim.DegerAl(dr, (int)TABBHMTERTIPLISTESIGIDER.ENUMAlanlarALACAK.ALFONKOD, "");
                t.kodlar[2] = VeriErisim.DegerAl(dr, (int)TABBHMTERTIPLISTESIGIDER.ENUMAlanlarALACAK.ALFINKOD, "");
                t.kodlar[3] = VeriErisim.DegerAl(dr, (int)TABBHMTERTIPLISTESIGIDER.ENUMAlanlarALACAK.ALEKOKOD, "");
                t.kodlar[4] = VeriErisim.DegerAl(dr, (int)TABBHMTERTIPLISTESIGIDER.ENUMAlanlarALACAK.ALPEBKOD, "");
                t.Noktala();
                al.Add(t);
            }
            dr.Close();
            if (!kayitli)
            {
                ENUMTertipDurum[] tdler = { ENUMTertipDurum.VERIR, ENUMTertipDurum.ALIR_VERIR };
                Tertip kistasTertip = new Tertip(tertip.yil, tertip.tabloKod);
                kistasTertip.kodlar[0] = tertip.kodlar[0];
                kistasTertip.kodlar[1] = tertip.kodlar[1];
                kistasTertip.kodlar[2] = tertip.kodlar[2];
                kistasTertip.kodlar[3] = tertip.kodlar[3].Substring(0, 4);
                Tertip[] verenler = BHMv2.IsKurallari.TertipTanim.Listele(ve, kullanan, kistasTertip, 2, tdler);
                if (verenler != null)
                {
                    for (int i = 0; i < verenler.Length; i++)
                    {
                        al.Add(verenler[i]);
                    }
                }
            }

            return (Tertip[])al.ToArray(typeof(Tertip));
        }
        public static Sonuc TertipOdenekAlacaklarKaydet(VeriErisim veriErisim, Kullanici kullanan, Tertip tertip, Tertip[] odenekAlacagiTertipler)
        {
            Sonuc s = new Sonuc(true, "", "");
            if (!kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.SISTEMYONETICISI))
            {
                s.DegerAta("Yetkiniz Yok!");
                return s;
            }
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, BHMv2Genel.VeriErisimAdBul(tertip.yil), kullanan.KimlikBul());
            TABBHMTERTIPLISTESIGIDER tab = new TABBHMTERTIPLISTESIGIDER(ve);

            ve.BeginTransaction();
            s = tab.OdenekAlacaklariSil(tertip);
            for (int i = 0; i < odenekAlacagiTertipler.Length && s.islemSonuc; i++)
            {
                s = tab.OdenekAlacagiEkle(tertip, odenekAlacagiTertipler[i]);
            }
            if (s.islemSonuc)
                ve.Commit();
            else
                ve.Rollback();
            return s;
        }

        private static ENUMTertipDurum TertipDurumBul(VeriErisim ve, Tertip tertip)
        {
            TABBHMTERTIPLISTESIGIDER tab = new TABBHMTERTIPLISTESIGIDER(ve);
            return tab.TertipDurumBul(tertip);
        }

        private static bool TertipDegisimKaydiVarMi(VeriErisim ve, Tertip alanTertip, Tertip verenTertip)
        {
            TABBHMTERTIPLISTESIGIDER tab = new TABBHMTERTIPLISTESIGIDER(ve);
            return tab.TertipDegisimKaydiSay(alanTertip, verenTertip) == 1;
        }

        public static bool TertipDegisimIzniVarMi(VeriErisim veriErisim, Kullanici kullanan, Tertip alanTertip, Tertip verenTertip)
        {
            bool donDeger;
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, BHMv2Genel.VeriErisimAdBul(alanTertip.yil), kullanan.KimlikBul());
            if (alanTertip.durum == ENUMTertipDurum.TANIMSIZ)
                alanTertip.durum = IsKurallari.TertipTanim.TertipDurumBul(ve, alanTertip);
            if (verenTertip.durum == ENUMTertipDurum.TANIMSIZ)
                verenTertip.durum = IsKurallari.TertipTanim.TertipDurumBul(ve, verenTertip);
            if (verenTertip.VerirMi() && alanTertip.AlirMi())
                donDeger = true;
            else if (IsKurallari.TertipTanim.TertipDegisimKaydiVarMi(ve, alanTertip, verenTertip))
                donDeger = true;
            else
                donDeger = false;
            ve.VeriErisimKapat();
            return donDeger;
        }

        internal static int TertipSayisiBul(VeriErisim veriErisim, Tertip tertip)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, BHMv2Genel.VeriErisimAdBul(tertip.yil), null);
            TABBHMTERTIPLISTESI tab = TabloOl(tertip.tabloKod, ve);
            int say = tab.TertipSayisiBul(tertip);
            ve.VeriErisimKapat();
            return say;
        }

        internal static Sonuc TertipTanimKopyalaOncYil(VeriErisim veriErisim, Kullanici kullanan, Tertip tertip)
        {
            Sonuc s = new Sonuc(true, "", "");
            if (!kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.SISTEMYONETICISI))
            {
                s.DegerAta("Yetkiniz Yok!");
                return s;
            }
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, BHMv2Genel.VeriErisimAdBul(tertip.yil), kullanan.KimlikBul());
            TABBHMTERTIPLISTESI tab = TabloOl(tertip.tabloKod, ve);
            ve.BeginTransaction();
            s = tab.TertipTanimKopyalaOncYil(tertip);
            if (s.islemSonuc)
                s = tab.OdenekAlacaklarKopyala(tertip);
            if (s.islemSonuc)
                ve.Commit();
            else
                ve.Rollback();
            ve.VeriErisimKapat();
            return s;
        }

        internal static ABSKod[] BirimKodListele(VeriErisim veriErisim, int yil, string kodTur, string birimKurKod)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, BHMv2Genel.VeriErisimAdBul(yil), null);
            TABBHMTERTIPLISTESIGIDER tabTertipler = new TABBHMTERTIPLISTESIGIDER(ve);

            IDataReader dr = tabTertipler.KodListele(yil, kodTur, birimKurKod);
            if (dr == null)
            {
                ve.VeriErisimKapat();
                throw new Exception(ve.hata);
            }
            IBKDServis bkdServis = TNS.BKD.Arac.Tanimla();
            ArrayList al = new ArrayList();
            if (dr != null)
            {
                while (dr.Read())
                {
                    string tut = VeriErisim.DegerAl(dr, 0, string.Empty);
                    ABSKodlarVAN akv = bkdServis.KodListele(yil, kodTur, tut);
                    if (akv.sonuc.islemSonuc && akv.absKodlar.Count > 0)
                        al.Add(akv.absKodlar[0]);
                }
                dr.Close();
            }
            ve.VeriErisimKapat();

            return (ABSKod[])al.ToArray(typeof(ABSKod));
        }

        internal static ObjectArray ListeleAciklama(VeriErisim veIc, Kullanici kullanan, KistasYapi kistas, UcgenKodBilgileri kodBilgileri, EkDegerParam ekDegerParam)
        {
            int alanSay = kodBilgileri.BoyutSayisi;
            string[] degerMaske = new string[alanSay];
            int yil = kistas.ekleTertipler[0].yil;
            string sql = UcgenServisi.UcgenSQLOlYeni(kullanan, kistas, kodBilgileri, ekDegerParam, ref degerMaske, true);
            IDataReader dr = veIc.DataReaderDondur(sql);
            if (dr == null)
            {
                return null;
            }
            ObjectArray oa = new ObjectArray();
            StrKodStrAd kodAd = null;
            while (dr.Read())
            {
                string trtpStr = "";
                for (int i = 0; i < alanSay; i++)
                {
                    string tut = VeriErisim.DegerAl(dr, i, string.Empty);
                    tut = OrtakFonksiyonlar.KodNoktaKoy(degerMaske[i], tut);
                    if (i != 0) trtpStr += "-";
                    trtpStr += tut;
                }

                string aciklama = VeriErisim.DegerAl(dr, alanSay, "");
                if (aciklama != null) aciklama = aciklama.Trim();
                if (!string.IsNullOrWhiteSpace(aciklama))
                {
                    if (kodAd == null || kodAd.kod != trtpStr)
                    {
                        kodAd = new StrKodStrAd();
                        kodAd.kod = trtpStr;
                        kodAd.ad = "";
                        oa.objeler.Add(kodAd);
                    }
                    if (!string.IsNullOrWhiteSpace(kodAd.ad)) kodAd.ad += ", ";
                    kodAd.ad += aciklama;
                }
            }
            return oa;
        }
    }
}
