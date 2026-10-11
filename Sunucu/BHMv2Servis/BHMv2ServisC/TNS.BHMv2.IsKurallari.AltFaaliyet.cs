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
    /// Bütçe tertiplerinde, faaliyet ve yatırım projeleriyle ilişkiyi kuran
    /// Alt Faaliyet boyutu ile ilgili iş kuralları yordamlarını tutan sınıf.
    /// </summary>
    public class BHMv2AltFaaliyet
    {
        /// <summary>
        /// Kullanıcının, alt faaliyet bilgileri üzerinde değişiklik yapma hakkını bulup döndüren yordam.
        /// Kullanıcı sistem yöndetici ise değişiklik yapma hakkı vardır.
        /// </summary>
        /// <param name="kullanan">Yetki kontrolü yapılacak kullanıcıya ait bilgiler</param>
        /// <returns>Değişiklik yapma hakkı varsa doğru, yoksa yanlış döndürülür.</returns>
        public static bool YazmaYetkisiVarMi(Kullanici kullanan, int tur)
        {
            if (kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.SISTEMYONETICISI))
                return true;
            if (tur == 1 && kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.PERFORMANSBUTCEYONETICISI))
                return true;
            return false;
        }

        /// <summary>
        /// Kullanıcının, alt faaliyet bilgileri üzerinde değişiklik yapma hakkını bulup döndüren yordam.
        /// Kullanıcı sistem yöndetici ise değişiklik yapma hakkı vardır.
        /// </summary>
        /// <param name="kullanan">Yetki kontrolü yapılacak kullanıcıya ait bilgiler</param>
        /// <param name="altFaaliyet">Kullanıcının değiştirmeye çalıştığı alt faaliyet.</param>
        /// <returns>Değişiklik yapma hakkı varsa doğru, yoksa yanlış döndürülür.</returns>
        public static bool YazmaYetkisiVarMi(Kullanici kullanan, AltFaaliyet altFaaliyet)
        {
            if (kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.SISTEMYONETICISI))
                return true;
            if (altFaaliyet == null || string.IsNullOrEmpty(altFaaliyet.pebKod))
                return false;

            if (kullanan == null) return false;

            return kullanan.YetkiTipiDegeriIceriyorMu("Bütçe Hazırlığı Kaydedebileceği Altfaaliyetler", altFaaliyet.pebKod, false);
        }

        /// <summary>
        /// Verilen alt faaliyet nesnesindeki alanların tutarlılığını kontrol eden fonksiyon
        /// <param name="kullanan">Alan tutarlılığı kontrol edilirken, kullanıcılara göre farklılık olacaksa, kullanıcı bilgisini tutan nesne</param>
        /// <param name="altFaaliyet">Alan tutarlılığı kontrol edilecek alt faaliyet nesnesi</param>
        /// </summary>
        /// <returns>Alanlar tutarlı ise, Sonuc yapısında true, değilse false ve açıklaması döndürülür.</returns>
        public static Sonuc AlanlarGecerliMi(Kullanici kullanan, AltFaaliyet altFaaliyet)
        {
            string hataStr = string.Empty;
            //if (string.IsNullOrEmpty(form.kurKod)) 
            //    hataStr = "Kurumsal Kod alanı boş olamaz";
            //if (string.IsNullOrEmpty(form.faaliyetSabitKod))
            //    hataStr = "Faaliyet/Sabit Maliyet alanı boş olamaz";
            return new Sonuc(string.IsNullOrEmpty(hataStr), hataStr, string.Empty);
        }


        /// <summary>
        /// Alt faaliyet nesnesini veri tabanına yazan fonksiyon
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler</param>
        /// <param name="altFaaliyet">Alt Faaliyet tanım bilgilerini tutan parametre</param>
        /// <returns>Kaydetme işleminin sonucu, Sonuc yapısında döndürülür.</returns>
        public static Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, AltFaaliyet altFaaliyet)
        {
            Sonuc s = new Sonuc(true, string.Empty, string.Empty);
            if (!YazmaYetkisiVarMi(kullanan, altFaaliyet))
            {
                s.DegerAta(ButceHazirlikResI.BHM016);
                return s;
            }

            s = AlanlarGecerliMi(kullanan, altFaaliyet);
            if (!s.islemSonuc)
            {
                return s;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, BHMv2Genel.VeriErisimAdBulSozluk(), kullanan.KimlikBul());
            VeriErisim veYat = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriYAT", kullanan.KimlikBul());
            VeriErisim vePerf = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriPEB", kullanan.KimlikBul());
            TABABSPEBKOD tabKod = new TABABSPEBKOD(ve);
            TABABSPEBKODKURYAPI tabKodKurYapi = new TABABSPEBKODKURYAPI(ve);
            TABABSPEBKOD tabYatBag = new TABABSPEBKOD(veYat);
            TABABSPEBKOD tabPerfBag = new TABABSPEBKOD(vePerf);

            ve.BeginTransaction();
            veYat.BeginTransaction();
            vePerf.BeginTransaction();

            if (s.islemSonuc)
                s = tabKodKurYapi.Sil(altFaaliyet);
            if (s.islemSonuc)
                s = tabKod.Sil(altFaaliyet);
            //if (s.islemSonuc)
            //    s = tabYatBag.YatBagSil(altFaaliyet);
            if (s.islemSonuc)
                s = tabPerfBag.PerfBagSil(altFaaliyet);
            if (s.islemSonuc)
                s = tabKod.Ekle(altFaaliyet);
            //if (s.islemSonuc && altFaaliyet.altProje > 0 && altFaaliyet.butceKaynakNo > 0)
            //    s = tabYatBag.YatBagEkle(altFaaliyet);

            if (s.islemSonuc && altFaaliyet.faaliyetBag != null)
                s = tabPerfBag.PerfBagEkle(altFaaliyet);

            for (int i = 0; i < altFaaliyet.kurYapiSablonListe.Length && s.islemSonuc; i++)
            {
                s = tabKodKurYapi.Ekle(altFaaliyet, i);
            }

            if (s.islemSonuc)
            {
                ve.Commit();
                vePerf.Commit();
                veYat.Commit();
            }
            else
            {
                ve.Rollback();
                vePerf.Rollback();
                veYat.Rollback();
            }
            ve.VeriErisimKapat();
            vePerf.VeriErisimKapat();
            veYat.VeriErisimKapat();
            return s;
        }

        /// <summary>
        /// Dizi içinde verilen her bir Alt faaliyet kimlik bilgisini, kendisi ile ilişkili tertip nesnesi yoksa, veri tabanından silen fonksiyon
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler</param>
        /// <param name="altFaaliyetler">Silinecek alt faaliyet kimlik bilgilerini tutan dizi</param>
        /// <returns>Silme işleminin sonucu, Sonuc yapısında döndürülür.</returns>
        public static Sonuc Sil(VeriErisim veriErisim, Kullanici kullanan, AltFaaliyet[] altFaaliyetler)
        {
            Sonuc s = new Sonuc(true, string.Empty, string.Empty);
            if (altFaaliyetler == null || altFaaliyetler.Length == 0)
            {
                s.islemSonuc = false;
                s.hataStr = ButceHazirlikResI.BHM017;
                return s;
            }

            if (!YazmaYetkisiVarMi(kullanan, 0))
            {
                s.DegerAta(ButceHazirlikResI.BHM016);
                return s;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, BHMv2Genel.VeriErisimAdBulSozluk(), kullanan.KimlikBul());
            VeriErisim veYat = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriYAT", kullanan.KimlikBul());
            VeriErisim vePerf = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriPEB", kullanan.KimlikBul());
            TABABSPEBKOD tabKod = new TABABSPEBKOD(ve);
            TABABSPEBKODKURYAPI tabKodKurYapi = new TABABSPEBKODKURYAPI(ve);
            TABABSPEBKOD tabYatBag = new TABABSPEBKOD(veYat);
            TABABSPEBKOD tabPerfBag = new TABABSPEBKOD(vePerf);

            ve.BeginTransaction();
            veYat.BeginTransaction();
            vePerf.BeginTransaction();

            for (int i = 0; i < altFaaliyetler.Length; i++)
            {
                AltFaaliyet form = altFaaliyetler[i];
                if (form.yil <= 0)
                    s.hataStr = ButceHazirlikResI.BHM018 + "<br>";
                if (string.IsNullOrEmpty(form.pebKod))
                    s.hataStr += ButceHazirlikResI.BHM019 + "<br>";
                if (!string.IsNullOrEmpty(s.hataStr))
                {
                    s.islemSonuc = false;
                    break;
                }
                //Burada alt faaliyetle ilgili tertip var mı kontrolü yapılmalı
                if (s.islemSonuc)
                    s = tabKodKurYapi.Sil(form);
                if (s.islemSonuc)
                    s = tabKod.Sil(form);
                if (s.islemSonuc)
                    s = tabYatBag.YatBagSil(form, "");
                if (s.islemSonuc)
                    s = tabPerfBag.PerfBagSil(form);

                if (!s.islemSonuc)
                    break;
            }

            if (s.islemSonuc)
            {
                ve.Commit();
                vePerf.Commit();
                veYat.Commit();
                s.bilgiStr = ButceHazirlikResI.BHM020;
            }
            else
            {
                ve.Rollback();
                vePerf.Rollback();
                veYat.Rollback();
            }

            ve.VeriErisimKapat();
            vePerf.VeriErisimKapat();
            veYat.VeriErisimKapat();
            return s;
        }

        /// <summary>
        /// Alt faaliyet bilgisini, verilen alt faaliyet nesnesindeki anahtar bilgilerle okuyup döndüren yordam.
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler</param>
        /// <param name="altFaaliyetKistas">Bilgileri okunacak alt faaliyetin anahtar bilgilerini tutan nesne</param>
        public static AltFaaliyet Oku(VeriErisim veriErisim, Kullanici kullanan, AltFaaliyet altFaaliyetKistas)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, BHMv2Genel.VeriErisimAdBulSozluk(), kullanan.KimlikBul());
            TABABSPEBKOD tabKod = new TABABSPEBKOD(ve);
            TABABSPEBKODKURYAPI tabKodKurYapi = new TABABSPEBKODKURYAPI(ve);

            AltFaaliyet okunanAltFaaliyet = new AltFaaliyet();
            IDataReader dr;
            dr = tabKod.Oku(altFaaliyetKistas);
            if (dr == null)
            {
                ve.VeriErisimKapat();
                throw new Exception(ve.hata);
            }
            if (dr.Read())
            {
                okunanAltFaaliyet.yil = VeriErisim.DegerAl(dr, (int)TABABSPEBKOD.ENUMAlanlar.YIL, 0);
                okunanAltFaaliyet.pebKod = VeriErisim.DegerAl(dr, (int)TABABSPEBKOD.ENUMAlanlar.PEBKOD, string.Empty);
                okunanAltFaaliyet.pebAd = VeriErisim.DegerAl(dr, (int)TABABSPEBKOD.ENUMAlanlar.PEBAD, string.Empty);
                okunanAltFaaliyet.pebAd2 = VeriErisim.DegerAl(dr, (int)TABABSPEBKOD.ENUMAlanlar.PEBAD2, string.Empty);
                okunanAltFaaliyet.kurKod = VeriErisim.DegerAl(dr, (int)TABABSPEBKOD.ENUMAlanlar.KURKOD, string.Empty);
                okunanAltFaaliyet.fonKod = VeriErisim.DegerAl(dr, (int)TABABSPEBKOD.ENUMAlanlar.FONKOD, string.Empty);
                okunanAltFaaliyet.finKod = VeriErisim.DegerAl(dr, (int)TABABSPEBKOD.ENUMAlanlar.FINKOD, string.Empty);
            }
            else
            {
                dr.Close();
                ve.VeriErisimKapat();
                throw new Exception(ButceHazirlikResI.BHM021);
            }
            dr.Close();
            VeriErisim veYat = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriYAT", kullanan.KimlikBul());
            VeriErisim vePerf = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriPEB", kullanan.KimlikBul());
            TABABSPEBKOD tabYatBag = new TABABSPEBKOD(veYat);
            TABABSPEBKOD tabPerfBag = new TABABSPEBKOD(vePerf);

            string[] sonuclar = tabPerfBag.PerfBagOku(okunanAltFaaliyet).Split('-');
            if (sonuclar.Length == 3)
            {
                if (sonuclar[1].Equals("0"))
                    sonuclar[1] = "F";
                else
                    sonuclar[1] = "S";
                okunanAltFaaliyet.faaliyetBag = new FaaliyetBag(sonuclar[0], sonuclar[1] + "-" + sonuclar[2]);
            }
            tabYatBag.YatBagOku(okunanAltFaaliyet, "", okunanAltFaaliyet);
            //okunanAltFaaliyet.altProje = OrtakFonksiyonlar.ConvertToInt(sonuclar[0], 0);
            //okunanAltFaaliyet.butceKaynakNo = OrtakFonksiyonlar.ConvertToInt(sonuclar[1], 0);
            Tertip t = new Tertip(altFaaliyetKistas.yil, ENUMTabloKod.TABPEBOdenek);
            Noktala(t, okunanAltFaaliyet);

            ArrayList al;
            dr = tabKodKurYapi.Oku(altFaaliyetKistas);
            if (dr == null)
            {
                ve.VeriErisimKapat();
                throw new Exception(ve.hata);
            }
            al = new ArrayList();
            while (dr.Read())
            {
                al.Add(VeriErisim.DegerAl(dr, (int)TABABSPEBKODKURYAPI.ENUMAlanlar.SABLON, string.Empty));
            }
            okunanAltFaaliyet.kurYapiSablonListe = (string[])al.ToArray(typeof(string));
            dr.Close();
            ve.VeriErisimKapat();
            return okunanAltFaaliyet;
        }

        /// <summary>
        /// Son seviyedeki alt faaliyetlerin listesini verilen kıstas alt faaliyet bilgilerine göre döndüren fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler</param>
        /// <param name="altFaaliyetKistas">Listelenecek alt faaliyetlerle ilgili kıstas bilgilerini tutan Alt faaliyet nesnesi</param>
        public static AltFaaliyet[] Listele(VeriErisim veriErisim, Kullanici kullanan, AltFaaliyet altFaaliyetKistas, string senaryoKod)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, BHMv2Genel.VeriErisimAdBulSozluk(), kullanan.KimlikBul());
            TABABSPEBKOD tabAltFaaliyet = new TABABSPEBKOD(ve);
            TABABSPEBKODKURYAPI tabAltFaaliyetKurumsalYapi = new TABABSPEBKODKURYAPI(ve);
            Tertip t = new Tertip(altFaaliyetKistas.yil, ENUMTabloKod.TABPEBOdenek);

            IDataReader dr = tabAltFaaliyet.ListeleUzunluk(altFaaliyetKistas);
            if (dr == null)
            {
                ve.VeriErisimKapat();
                throw new Exception(ve.hata);
            }
            ArrayList al = new ArrayList();
            while (dr.Read())
            {
                AltFaaliyet altFaaliyetIc = new AltFaaliyet();
                altFaaliyetIc.yil = VeriErisim.DegerAl(dr, (int)TABABSPEBKOD.ENUMAlanlar.YIL, 0);
                altFaaliyetIc.pebKod = VeriErisim.DegerAl(dr, (int)TABABSPEBKOD.ENUMAlanlar.PEBKOD, string.Empty);
                altFaaliyetIc.pebAd = VeriErisim.DegerAl(dr, (int)TABABSPEBKOD.ENUMAlanlar.PEBAD, string.Empty);
                altFaaliyetIc.pebAd2 = VeriErisim.DegerAl(dr, (int)TABABSPEBKOD.ENUMAlanlar.PEBAD2, string.Empty);
                altFaaliyetIc.kurKod = VeriErisim.DegerAl(dr, (int)TABABSPEBKOD.ENUMAlanlar.KURKOD, string.Empty);
                altFaaliyetIc.fonKod = VeriErisim.DegerAl(dr, (int)TABABSPEBKOD.ENUMAlanlar.FONKOD, string.Empty);
                altFaaliyetIc.finKod = VeriErisim.DegerAl(dr, (int)TABABSPEBKOD.ENUMAlanlar.FINKOD, string.Empty);
                Noktala(t, altFaaliyetIc);
                al.Add(altFaaliyetIc);
            }
            dr.Close();

            foreach (AltFaaliyet altFaaliyet in al)
            {
                dr = tabAltFaaliyetKurumsalYapi.Oku(altFaaliyet);
                if (dr == null)
                {
                    ve.VeriErisimKapat();
                    throw new Exception(ve.hata);
                }
                ArrayList icAl = new ArrayList();
                while (dr.Read())
                {
                    icAl.Add(VeriErisim.DegerAl(dr, (int)TABABSPEBKODKURYAPI.ENUMAlanlar.SABLON, string.Empty));
                }
                altFaaliyet.kurYapiSablonListe = (string[])icAl.ToArray(typeof(string));
                dr.Close();
            }

            if (altFaaliyetKistas.iliskileriOku > 0)
            {
                IUZYServis servisUZY = TNS.UZY.Arac.Tanimla();
                VeriErisim veYat = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriYAT", kullanan.KimlikBul());
                VeriErisim vePerf = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriPEB", kullanan.KimlikBul());
                TABABSPEBKOD tabYatBag = new TABABSPEBKOD(veYat);
                TABABSPEBKOD tabPerfBag = new TABABSPEBKOD(vePerf);

                string[] sonuclar;
                foreach (AltFaaliyet okunanAltFaaliyet in al)
                {
                    if (altFaaliyetKistas.iliskileriOku == 1 || altFaaliyetKistas.iliskileriOku == 3)
                    {
                        sonuclar = tabPerfBag.PerfBagOku(okunanAltFaaliyet).Split('-');
                        if (sonuclar.Length == 3)
                        {
                            string uzayAd;
                            if (sonuclar[1].Equals("0"))
                            {
                                sonuclar[1] = "F";
                                uzayAd = "PEBFAALIYETPROJE";
                            }
                            else
                            {
                                sonuclar[1] = "S";
                                uzayAd = "PEBSABITMALIYET";
                            }

                            okunanAltFaaliyet.faaliyetBag = new FaaliyetBag(sonuclar[0], sonuclar[1] + "-" + sonuclar[2]);
                            string kosul = TNS.UZY.Arac.UzayKosulOl(okunanAltFaaliyet.yil.ToString(), sonuclar[0], sonuclar[2]);
                            okunanAltFaaliyet.faaliyetAd = servisUZY.UzayDegeriStr(kullanan, uzayAd, kosul, true, null);
                        }
                    }
                    if (altFaaliyetKistas.iliskileriOku == 2 || altFaaliyetKistas.iliskileriOku == 3)
                    {
                        tabYatBag.YatBagOku(okunanAltFaaliyet, senaryoKod, okunanAltFaaliyet);
                    }
                }
                veYat.VeriErisimKapat();
                vePerf.VeriErisimKapat();
            }

            ve.VeriErisimKapat();
            return (AltFaaliyet[])al.ToArray(typeof(AltFaaliyet));
        }

        /// <summary>
        /// Alt faaliyet nesnesindeki, alt faaliyetin ilişkili olabileceği tertiplerle ilgili kodları tutan alanları,
        /// alan kırılım yapısına göre formatlayan yordam.
        /// </summary>
        /// <param name="tertip">Noktalama için, alanların kırılım yapılarını tutan tertip nesnesi</param>
        /// <param name="altFaaliyet">Alanları noktalanacak olan faaliyet nesnesi</param>
        private static void Noktala(Tertip tertip, AltFaaliyet altFaaliyet)
        {
            altFaaliyet.pebKod = OrtakFonksiyonlar.KodNoktaKoy(tertip.Kirilimlar(4), altFaaliyet.pebKod);
            altFaaliyet.kurKod = OrtakFonksiyonlar.KodNoktaKoy(tertip.Kirilimlar(0), altFaaliyet.kurKod);
            altFaaliyet.fonKod = OrtakFonksiyonlar.KodNoktaKoy(tertip.Kirilimlar(1), altFaaliyet.fonKod);
            altFaaliyet.finKod = OrtakFonksiyonlar.KodNoktaKoy(tertip.Kirilimlar(2), altFaaliyet.finKod);
        }

        /// <summary>
        /// Alt faaliyetlerden, verilen kurumsal kodla ilişkilendirilmiş olanlarını listeleyen yordam.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, yordam çağrılırken açılmış olmalıdır.</param>
        /// <param name="altFaaliyetKistas">Listelenecek alt faaliyetlerle ilgili kıstas bilgilerini tutan Alt faaliyet nesnesi</param>
        /// <param name="kurumsalYapiKod">Kendisi ile ilişkili faaliyetlerin listeleneceği kurumsal kod</param>
        /// <returns>Verilen kurumsal kadla ilişkili ve kıstasa uygun alt faaliyetlerin dizisi döndürülür.</returns>
        public static AltFaaliyet[] Listele(VeriErisim veriErisim, AltFaaliyet altFaaliyetKistas, string kurumsalYapiKod)
        {
            KurumsalYapi kurumsalYapi = BHMv2KurumsalYapi.KurumsalYapiDondur(veriErisim, kurumsalYapiKod);
            AltFaaliyet[] kodlar = Listele(veriErisim, null, altFaaliyetKistas, "");
            for (int i = 0; i < kodlar.Length; i++)
            {
                bool ekle = false;
                if (TNS.BKD.Arac.KodKiyasla(kodlar[i].kurKod, kurumsalYapi.kurKod))
                {
                    if (kodlar[i].kurYapiSablonListe.Length > 0)
                    {
                        for (int j = 0; j < kodlar[i].kurYapiSablonListe.Length && !ekle; j++)
                        {
                            if (TNS.BKD.Arac.KodKiyasla(kurumsalYapiKod, kodlar[i].kurYapiSablonListe[j]))
                            {
                                ekle = true;
                            }
                        }
                    }
                    else
                        ekle = true;
                }
                if (!ekle)
                    kodlar[i] = null;
            }
            return kodlar;
        }

        /// <summary>
        /// Alt faaliyetlerden, verilen kurumsal kodla ilişkilendirilmiş olanlarını, verilen ayraç ile birleştirip döndüren yordam.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, yordam çağrılırken açılmış olmalıdır.</param>
        /// <param name="altFaaliyetKistas">Listelenecek alt faaliyetlerle ilgili kıstas bilgilerini tutan Alt faaliyet nesnesi</param>
        /// <param name="kurumsalYapiKod">Kendisi ile ilişkili faaliyetlerin listeleneceği kurumsal kod</param>
        /// <param name="ayrac">Bulunan alt faaliyet kodları, bu parametrede verilen karakter ile birleştirilir.</param>
        /// <returns>Verilen kurumsal kadla ilişkili ve kıstasa uygun alt faaliyetler, verilen ayraçla birleştirilerek dizgi olarak döndürülür.</returns>
        public static string Listele(VeriErisim veriErisim, AltFaaliyet altFaaliyetKistas, string kurumsalYapiKod, char ayrac)
        {
            AltFaaliyet[] kodlar = Listele(veriErisim, altFaaliyetKistas, kurumsalYapiKod);
            string tut = string.Empty;
            for (int i = 0; i < kodlar.Length; i++)
            {
                if (kodlar[i] != null)
                {
                    if (!string.IsNullOrEmpty(tut)) tut += ayrac;
                    tut += kodlar[i].pebKod;
                }
            }
            return tut;
        }

        /// <summary>
        /// Kıstas olarak verilen alt faaliyet kıstas bilgilerinin ve/veya yatırım projesinin
        /// ilişkili olduğu tüm alt proje kaynaklarını ve ilişkili olduğu alt faaliyetleri döndüren yordam.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, yordam çağrılırken açılmış olmalıdır.</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler</param>
        /// <param name="altFaaliyetKistas">Listelenecek alt faaliyetlerle ilgili kıstas bilgilerini tutan Alt faaliyet nesnesi</param>
        /// <param name="projeKod">Alt faaliyetleri bulunacak DPT yatırım projesi kodu</param>
        /// <returns>Verilen kıstaslarla ilişkili alt faaliyetlerin listesi döndürülür.</returns>
        public static AltFaaliyet[] YatBagPebKodlariListele(VeriErisim veriErisim, Kullanici kullanan, AltFaaliyet altFaaliyetKistas, int projeKod, string senaryoKod)
        {
            VeriErisim veYat = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriYAT", kullanan.KimlikBul());
            TABABSPEBKOD tabABSPEBKOD = new TABABSPEBKOD(veYat);
            Tertip t = new Tertip(altFaaliyetKistas.yil, ENUMTabloKod.TABPEBOdenek);

            IDataReader dr = tabABSPEBKOD.YatBagPebKodlariBul(altFaaliyetKistas, projeKod, senaryoKod);
            if (dr == null)
            {
                veYat.VeriErisimKapat();
                throw new Exception(veYat.hata);
            }
            ArrayList al = new ArrayList();
            while (dr.Read())
            {
                AltFaaliyet altFaaliyetIc = new AltFaaliyet();
                altFaaliyetIc.yil = VeriErisim.DegerAl(dr, 0, 0);
                altFaaliyetIc.pebKod = VeriErisim.DegerAl(dr, 1, string.Empty);
                altFaaliyetIc.altProje = VeriErisim.DegerAl(dr, 2, 0);
                altFaaliyetIc.butceKaynakNo = VeriErisim.DegerAl(dr, 3, 0);
                Noktala(t, altFaaliyetIc);
                al.Add(altFaaliyetIc);
            }
            dr.Close();

            veYat.VeriErisimKapat();
            return (AltFaaliyet[])al.ToArray(typeof(AltFaaliyet));
        }

        internal static Sonuc IliskiKaydet(VeriErisim veriErisim, Kullanici kullanan, AltFaaliyet[] altFaaliyetler, int tur, string senaryoKod)
        {
            Sonuc s = new Sonuc(true, string.Empty, string.Empty);
            if (!YazmaYetkisiVarMi(kullanan, tur))
            {
                s.DegerAta(ButceHazirlikResI.BHM016);
                return s;
            }

            if (tur == 2)
            {
                VeriErisim veYat = VeriErisim.VeriErisimBul(null, "BaglantiSatiriYAT", kullanan.KimlikBul());
                TABABSPEBKOD tabYatBag = new TABABSPEBKOD(veYat);
                veYat.BeginTransaction();
                for (int i = 0; i < altFaaliyetler.Length; i++)
                {
                    if (s.islemSonuc)
                        s = tabYatBag.YatBagSil(altFaaliyetler[i], senaryoKod);
                    if (s.islemSonuc && altFaaliyetler[i].altProje > 0 && altFaaliyetler[i].butceKaynakNo > 0)
                        s = tabYatBag.YatBagEkle(altFaaliyetler[i], senaryoKod);
                    if (!s.islemSonuc)
                        break;
                }
                if (s.islemSonuc)
                    veYat.Commit();
                else
                    veYat.Rollback();
                veYat.VeriErisimKapat();
            }
            else
            {
                VeriErisim vePerf = VeriErisim.VeriErisimBul(null, "BaglantiSatiriPEB", kullanan.KimlikBul());
                TABABSPEBKOD tabPerfBag = new TABABSPEBKOD(vePerf);
                vePerf.BeginTransaction();

                for (int i = 0; i < altFaaliyetler.Length; i++)
                {
                    if (altFaaliyetler[i].faaliyetBag == null)
                        s.DegerAta(string.Format("{0} kodlu alt faaliyet için, faaliyet seçmelisiniz!", altFaaliyetler[i].pebKod));
                    if (s.islemSonuc)
                        s = tabPerfBag.PerfBagSil(altFaaliyetler[i]);
                    if (s.islemSonuc && altFaaliyetler[i].faaliyetBag != null)
                        s = tabPerfBag.PerfBagEkle(altFaaliyetler[i]);
                    if (!s.islemSonuc)
                        break;
                }

                if (s.islemSonuc)
                    vePerf.Commit();
                else
                    vePerf.Rollback();
                vePerf.VeriErisimKapat();
            }
            return s;
        }
    }
}