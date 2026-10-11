using System;
using System.Data;
using TNS.Veri;

namespace TNS.EVR.IsKurallari.Veri
{
    /// <summary>
    /// Birim bilgilerini içeren EVRBIRIM veri tabanı tablosu ile ilgili, kaydetme, silme ve listeleme yordamlarını tutan sınıf.    
    /// </summary>
    public class TABBirim : TABOrtak
    {
        /// <summary>        
        /// Parametre olarak verilen veriErisim nesnesine göre, TABBirim sınıfının yeni bir örneğini oluşturur.
        /// </summary>
        /// <param name="veriErisim">Veri erişim bilgisini içeren nesne</param>
        public TABBirim(VeriErisim veriErisim) : base(veriErisim) { }

        /// <summary>
        /// Birim bilgilerini içeren veri tabanı tablosunun adını tutan değişken.
        /// </summary>
        public static string TABLOAD = "EVRBIRIM";

        /// <summary>
        /// Birim bilgilerini içeren veri tabanı tablosunun alanlarının sırasını tutan numaralandırma.
        /// </summary>
        public enum ENUMAlanlar
        {
            KOD = 0,
            IDAREKIMLIKKODU = 1,
            AD = 2,
            GENELEVRAK = 3,
            USTBIRIMKODU = 4,
            SEVIYE = 5,
            DETAY = 6,
            KULLANILMIYOR = 7,
            EPOSTA = 8,
            KISIYEHAVALE = 9,
            KISAAD = 10,
            DETAYDAGITIM = 11,
            DAGITIMBIRIMKOD = 12,
        }

        /// <summary>
        /// Birim bilgilerini içeren veri tabanı tablosunun alanlarının isimlerini tutan dizi.
        /// </summary>
        static string[] ALANLAR = { "KOD", "IDAREKIMLIKKODU", "AD", "GENELEVRAK", "USTBIRIMKODU", "SEVIYE", "DETAY", "KULLANILMIYOR", "EPOSTA", "KISIYEHAVALE", "KISAAD", "DETAYDAGITIM", "DAGITIMBIRIMKOD" };

        /// <summary>
        /// Personel bilgilerini içeren veri tabanı tablosunun adını tutan değişken.
        /// </summary>
        public static string TABLOADPERSONEL = "EVRPERSONEL";
        public static string TABLOADPERSONELORG = "EVRPERSONELORG";

        /// <summary>
        /// Personel bilgilerini içeren veri tabanı tablosunun alanlarının sırasını tutan numaralandırma.
        /// </summary>
        enum ENUMAlanlarPERSONEL
        {
            TCKIMLIKNO = 0,
            SICILNO = 1,
            AD = 2,
            SOYAD = 3,
            EPOSTA = 4,
            DURUM = 5,
            MUHASEBEKODU = 6,
            UNVAN = 7,
            EVRAKBIRIMKOD = 8,
            AKADEMIKUNVAN = 9
        }

        /// <summary>
        /// Personel bilgilerini içeren veri tabanı tablosunun alanlarının isimlerini tutan dizi.
        /// </summary>
        static string[] ALANLARPERSONEL = { "TCKIMLIKNO", "SICILNO", "AD", "SOYAD", "EPOSTA", "DURUM", "MUHASEBEKODU", "UNVAN", "EVRAKBIRIMKOD", "AKADEMIKUNVAN" };

        /// <summary>
        /// Parametre olarak verilen birim nesne koleksiyonuna göre; birim bilgilerini, sql cümlesi hazırlayıp çalıştırarak veritabanına kaydeden yordam.
        /// </summary>
        /// <param name="liste">Birim nesne koleksiyonunu içeren nesne</param>
        /// <returns>İşlem sonucu Sonuc nesnesi olarak döndürülür.</returns>
        public Sonuc Kaydet(ObjectArray liste)
        {
            Sonuc sonuc = new Sonuc(false, "", "");

            foreach (Birim bilgi in liste.objeler)
            {
                string[] st = bilgi.kod.Split('.');
                int seviye = st.Length;

                string sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
                sql = sql + VeriErisim.DegerYap(bilgi.kod) + ", ";
                sql = sql + VeriErisim.DegerYap(bilgi.idareKimlikKodu) + ", ";
                sql = sql + VeriErisim.DegerYap(bilgi.ad) + ", ";
                sql = sql + VeriErisim.DegerYap(bilgi.genelEvrakMi) + ", ";
                sql = sql + VeriErisim.DegerYap(bilgi.ustBirim) + ",";
                sql = sql + VeriErisim.DegerYap(seviye) + ", ";
                sql = sql + VeriErisim.DegerYap(0) + ", ";
                sql = sql + VeriErisim.DegerYap(bilgi.kullanilmiyor) + ",";
                sql = sql + VeriErisim.DegerYap(bilgi.ePosta) + ",";
                sql = sql + VeriErisim.DegerYap(bilgi.kisiyeHavale) + ",";
                sql = sql + VeriErisim.DegerYap(bilgi.kisaAd) + ",";
                sql = sql + VeriErisim.DegerYap(bilgi.detayDagitim) + ",";
                sql = sql + VeriErisim.DegerYap(bilgi.dagitimBirimKod) + ")";

                sonuc = KomutCalistir(sql, 1);

                if (!sonuc.islemSonuc)
                {
                    veriErisim.hata = "";

                    sql = "UPDATE " + TABLOAD + " SET ";
                    sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.IDAREKIMLIKKODU], bilgi.idareKimlikKodu) + ",";
                    sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.AD], bilgi.ad) + ",";
                    sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.GENELEVRAK], bilgi.genelEvrakMi) + ",";
                    sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.KULLANILMIYOR], bilgi.kullanilmiyor) + ",";
                    sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.USTBIRIMKODU], bilgi.ustBirim) + ",";
                    sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.EPOSTA], bilgi.ePosta) + ",";
                    sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.KISAAD], bilgi.kisaAd) + ",";
                    sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.KISIYEHAVALE], bilgi.kisiyeHavale) + ",";
                    sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.DETAYDAGITIM], bilgi.detayDagitim) + ",";
                    sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.DAGITIMBIRIMKOD], bilgi.dagitimBirimKod);
                    sql += " WHERE " + VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", bilgi.kod);

                    sonuc = KomutCalistir(sql, 1);
                }

                if (!sonuc.islemSonuc)
                    return sonuc;
            }

            if (sonuc.islemSonuc)
                DetaySayilariDuzenle("");

            if (liste.objeler.Count == 0)//Liste boş ise hata dönmemesi için
                sonuc.islemSonuc = true;

            return sonuc;
        }

        /// <summary>
        /// Parametre olarak verilen birim nesne koleksiyonuna göre; birim bilgilerini, sql cümlesi hazırlayıp çalıştırarak veritabanından silen yordam.
        /// </summary>
        /// <param name="liste">Birim nesne koleksiyonunu içeren nesne</param>
        /// <returns>İşlem sonucu Sonuc nesnesi olarak döndürülür.</returns>
        public Sonuc Sil(ObjectArray liste)
        {
            Sonuc sonuc = new Sonuc(false, "", "");

            foreach (Birim bilgi in liste.objeler)
            {
                string sql = "";

                sql = "DELETE FROM " + TABLOAD + " WHERE ";
                sql += VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", bilgi.kod);

                sonuc = KomutCalistir(sql, 1);

                if (!sonuc.islemSonuc)
                    return sonuc;
            }

            if (liste.objeler.Count == 0)//Liste boş ise hata dönmemesi için
                sonuc.islemSonuc = true;

            if (sonuc.islemSonuc)
                DetaySayilariDuzenle("");

            return sonuc;
        }

        /// <summary>
        /// Paramete olarak verilen birim nesnesine göre; birim bilgilerini, sql cümlesi hazırlayıp çalıştırarak veritabanından okuyan yordam.
        /// <param name="bilgi">Birim bilgilerini içeren nesne</param>
        /// <returns>İşlem sonucu DataReader olarak döndürülür.</returns>
        public IDataReader Listele(Birim bilgi, bool anaBirim)
        {
            string kosul = "";
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR) + "," + TABLOAD + "_1.AD FROM " + TABLOAD + " LEFT OUTER JOIN " + TABLOAD + " AS " + TABLOAD + "_1 ON " + TABLOAD + ".USTBIRIMKODU = " + TABLOAD + "_1.KOD ";

            if (bilgi.kod != "")
            {
                string[] bler = bilgi.kod.Split(';');
                string icKosul = "";
                foreach (string bb in bler)
                {
                    icKosul = VeriErisim.KosulEkle(icKosul, "OR", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KOD], " LIKE ", bb.Trim()));
                }
                if (icKosul != "")
                    kosul = VeriErisim.KosulEkle(kosul, "AND", icKosul);
            }
            if (bilgi.idareKimlikKodu != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.IDAREKIMLIKKODU], "=", bilgi.idareKimlikKodu));
            if (bilgi.ad != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.AD], " LIKE ", "%" + bilgi.ad + "%"));
            if (bilgi.genelEvrakMi > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.GENELEVRAK], "=", bilgi.genelEvrakMi));
            if (bilgi.seviye != 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.SEVIYE], "=", bilgi.seviye));
            if (bilgi.detay == true)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.DETAY], "=", 0));
            if (bilgi.kullanilmiyor > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KULLANILMIYOR], "=", bilgi.kullanilmiyor));
            if (bilgi.ustBirim != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.USTBIRIMKODU], "=", bilgi.ustBirim));
            if (bilgi.kisiyeHavale > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KISIYEHAVALE], "=", bilgi.kisiyeHavale));
            if (anaBirim)
            {
                string ekKosul = VeriErisim.KosulEkle("", "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.USTBIRIMKODU], "LIKE", bilgi.ustBirim + "%"));
                ekKosul = VeriErisim.KosulEkle(ekKosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.GENELEVRAK], "=", (int)ENUMGenelEvrakBirimi.EVET));

                if (kosul != "")
                    kosul += " OR ";
                kosul += "(" + ekKosul + ")";
            }

            if (kosul != "")
                sql += " WHERE " + kosul;

            sql += " ORDER BY 1";

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Paramete olarak verilen evrak birim kodunun daha önce kullanılıp kullanılmadığını, sql cümlesi hazırlayıp çalıştırarak veritabanından sorgulayan yordam.
        /// </summary>
        /// <param name="kod">Birim kod bilgisini içeren nesne</param>
        /// <returns>Evrak birim kodu kullanılmış ise işlem sonucu Doğru, kullanılmamış ise Yanlış döndürülür.</returns>
        public bool KullanilmisMi(string kod)
        {
            string sql = "SELECT COUNT(*) FROM EVREVRAK ";
            sql = VeriErisim.KosulEkle(sql, " WHERE", VeriErisim.KosulOl("", "EVRAKBIRIMKODU", "=", kod.Trim()));

            return veriErisim.TekKolonSQLInt(sql) > 0;
        }

        /// <summary>
        /// Parametre olarak verilen evrak birim kodunu detayAra kriterine göre, sql cümlesi harızlayıp çalıştırarak veritabanından detay kayıtlarını sorgulayan yordam.
        /// </summary>
        /// <param name="kod">Birim kod bilgisini içeren nesne</param>
        /// <param name="detayAra">Detay arama durumu bilgisini içeren nesne</param>
        /// <returns>Detay kayıtları varsa işlem sonucu Doğru, yoksa Yanlış döndürülür.</returns>
        public bool AnasiCocuguVarMi(string kod, bool detayAra)
        {
            string sql = "";
            bool varMi = false;

            sql = "SELECT COUNT(*) FROM " + TABLOAD;
            if (detayAra)
                sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "LIKE", kod + "%"));
            else
                sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", kod));

            IDataReader r = veriErisim.DataReaderDondur(sql);

            if (veriErisim.hata == "")
                if (r.Read())
                    if (detayAra)
                        varMi = VeriErisim.DegerAl(r, 0, 0) >= 2;
                    else
                        varMi = VeriErisim.DegerAl(r, 0, 0) >= 1;

            r.Close();

            return varMi;
        }

        /// <summary>
        /// Paramete olarak verilen birim kodu bilgisine göre; birim bilgisinin detay seviyelerini, sql cümlesi hazırlayıp çalıştırarak veritabanına kaydeden yordam.
        /// </summary>
        /// <param name="kod">Birim kod bilgisini içeren nesne</param>
        /// <returns>İşlem sonucu Sonuc nesnesi olarak döndürülür.</returns>
        public Sonuc DetaySayilariDuzenle(string kod)
        {
            string sql;
            string bulKod = "";
            Sonuc sonuc = new Sonuc(false, "", "");
            bulKod = kod;

            if (kod.LastIndexOf('.') > 0)
                bulKod = kod.Substring(0, kod.LastIndexOf('.'));

            if (bulKod.Trim() != "")
            {
                sql = "UPDATE " + TABLOAD;
                sql += " SET DETAY=(SELECT COUNT(*) FROM " + TABLOAD + " B";
                sql += " WHERE B.KOD LIKE " + TABLOAD + ".KOD" + VeriErisim.IslecBul("+") + "'%' AND ";
                sql += " B.SEVIYE>" + TABLOAD + ".SEVIYE AND KOD LIKE '" + VeriErisim.DegerYap(bulKod) + "%') WHERE KOD LIKE '" + VeriErisim.DegerYap(bulKod) + "%'";
            }
            else
            {
                sql = "UPDATE " + TABLOAD;
                sql += " SET DETAY=(SELECT COUNT(*) FROM " + TABLOAD + " B";
                sql += " WHERE B.KOD LIKE " + TABLOAD + ".KOD" + VeriErisim.IslecBul("+") + "'%' AND ";
                sql += " B.SEVIYE>" + TABLOAD + ".SEVIYE)";
            }
            sonuc = KomutCalistir(sql, 0);

            //degiskenVarsayilanlar.Add("/KODYAPI/KOD18/KODYAPI", "9.99.9.999.9.99.9.99.9.99.99"); 
            sql = "UPDATE " + TABLOAD + " SET SEVIYE=1 WHERE LEN(KOD)=1";
            sonuc = KomutCalistir(sql, 0);
            sql = "UPDATE " + TABLOAD + " SET SEVIYE=2 WHERE LEN(KOD)=4";
            sonuc = KomutCalistir(sql, 0);
            sql = "UPDATE " + TABLOAD + " SET SEVIYE=3 WHERE LEN(KOD)=6";
            sonuc = KomutCalistir(sql, 0);
            sql = "UPDATE " + TABLOAD + " SET SEVIYE=4 WHERE LEN(KOD)=10";
            sonuc = KomutCalistir(sql, 0);
            sql = "UPDATE " + TABLOAD + " SET SEVIYE=5 WHERE LEN(KOD)=12";
            sonuc = KomutCalistir(sql, 0);
            sql = "UPDATE " + TABLOAD + " SET SEVIYE=6 WHERE LEN(KOD)=15";
            sonuc = KomutCalistir(sql, 0);
            sql = "UPDATE " + TABLOAD + " SET SEVIYE=7 WHERE LEN(KOD)=17";
            sonuc = KomutCalistir(sql, 0);
            sql = "UPDATE " + TABLOAD + " SET SEVIYE=8 WHERE LEN(KOD)=20";
            sonuc = KomutCalistir(sql, 0);
            sql = "UPDATE " + TABLOAD + " SET SEVIYE=9 WHERE LEN(KOD)=22";
            sonuc = KomutCalistir(sql, 0);
            sql = "UPDATE " + TABLOAD + " SET SEVIYE=10 WHERE LEN(KOD)=25";
            sonuc = KomutCalistir(sql, 0);
            sql = "UPDATE " + TABLOAD + " SET SEVIYE=11 WHERE LEN(KOD)=28";
            sonuc = KomutCalistir(sql, 0);

            return sonuc;
        }

        /// <summary>
        /// Paramete olarak verilen personel nesnesine göre; personel bilgilerini, sql cümlesi hazırlayıp çalıştırarak veritabanından okuyan yordam.
        /// </summary>
        /// <param name="bilgi">Birim bilgilerini içeren nesne</param>
        /// <returns>İşlem sonucu DataReader olarak döndürülür.</returns>
        public IDataReader PersonelListele(Personel bilgi, bool orgSemaPersonel = false)
        {
            string tabloAd = orgSemaPersonel ? TABLOADPERSONELORG : TABLOADPERSONEL;
            string evrakBirimKodu = orgSemaPersonel ? bilgi.evrakBirimKodu.Replace(";", "").TrimEnd('%') : bilgi.evrakBirimKodu;
            evrakBirimKodu = (bilgi.tcKimlikNo == "100" ? evrakBirimKodu + "%" : evrakBirimKodu);

            string sql = "SELECT " + VeriErisim.TumAlanlar("P", ALANLARPERSONEL) + " FROM " + tabloAd + " P";

            sql += " LEFT OUTER JOIN ";
            sql += "EVRBIRIM ON P.EVRAKBIRIMKOD = EVRBIRIM.KOD " + (orgSemaPersonel ? " " : "+ ';' ");
            sql += "WHERE (P.DURUM = 1) AND (EVRBIRIM.KULLANILMIYOR = 1 OR EVRBIRIM.KULLANILMIYOR IS NULL) ";

            if (!string.IsNullOrEmpty(bilgi.ad + bilgi.soyad))
                sql += "AND (P.AD +' '+ P.SOYAD LIKE " + VeriErisim.DegerYap("%" + bilgi.ad + "%") + ") ";
            if (bilgi.evrakBirimKodu != "")
                sql += "AND (P.EVRAKBIRIMKOD LIKE " + VeriErisim.DegerYap(evrakBirimKodu) + ") ";
            if (bilgi.tcKimlikNo != "" && bilgi.tcKimlikNo != "100")
                sql += "AND (P.TCKIMLIKNO = " + VeriErisim.DegerYap(bilgi.tcKimlikNo) + ") ";
            if (!string.IsNullOrEmpty(bilgi.unvan))
                sql += "AND (P.UNVAN = " + VeriErisim.DegerYap(bilgi.unvan) + ") ";

            sql += "ORDER BY P.AD, P.SOYAD";

            //kosul = VeriErisim.KosulEkle(kosul, "WHERE", VeriErisim.KosulOl(TABLOADPERSONEL, ALANLARPERSONEL[(int)ENUMAlanlarPERSONEL.DURUM], "=", 1));
            //if (bilgi.tcKimlikNo != "" && bilgi.tcKimlikNo != "100")
            //    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADPERSONEL, ALANLARPERSONEL[(int)ENUMAlanlarPERSONEL.TCKIMLIKNO], "=", bilgi.tcKimlikNo));
            //if (bilgi.evrakBirimKodu != "")
            //    //kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADPERSONEL, ALANLARPERSONEL[(int)ENUMAlanlarPERSONEL.EVRAKBIRIMKOD], " LIKE ", "%" + bilgi.evrakBirimKodu + "%"));
            //    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADPERSONEL, ALANLARPERSONEL[(int)ENUMAlanlarPERSONEL.EVRAKBIRIMKOD], "LIKE", bilgi.evrakBirimKodu));//Like ile arandığında Evrak birim listesi alındığında alt biriminde personelleri listeleniyor
            //if (!string.IsNullOrEmpty(bilgi.unvan))
            //    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADPERSONEL, ALANLARPERSONEL[(int)ENUMAlanlarPERSONEL.UNVAN], " LIKE ", "%" + bilgi.unvan + "%"));
            //if (bilgi.tcKimlikNo != "100")
            //{
            //    if (!string.IsNullOrEmpty(bilgi.ad))
            //        kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADPERSONEL, ALANLARPERSONEL[(int)ENUMAlanlarPERSONEL.AD], " LIKE ", "%" + bilgi.ad + "%"));
            //    if (!string.IsNullOrEmpty(bilgi.soyad))
            //        kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADPERSONEL, ALANLARPERSONEL[(int)ENUMAlanlarPERSONEL.SOYAD], " LIKE ", "%" + bilgi.soyad + "%"));
            //}
            //else if (!string.IsNullOrEmpty(bilgi.ad + bilgi.soyad))
            //    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADPERSONEL, ALANLARPERSONEL[(int)ENUMAlanlarPERSONEL.AD] + "+ ' ' +" + TABLOADPERSONEL + "." + ALANLARPERSONEL[(int)ENUMAlanlarPERSONEL.SOYAD], " LIKE ", "%" + bilgi.ad + "%"));

            //if (kosul != "")
            //    sql += " WHERE " + kosul;

            //sql += " ORDER BY AD,SOYAD";

            return veriErisim.DataReaderDondur(sql);
        }

        public Sonuc DTVTBirimBilgisiKaydet(Birim form)
        {
            Sonuc sonuc = new Sonuc(false, "", "");

            string[] st = form.kod.Split('.'); //seviye

            string sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
            sql = sql + VeriErisim.DegerYap(form.kod) + ", ";
            sql = sql + VeriErisim.DegerYap(form.idareKimlikKodu) + ", ";
            sql = sql + VeriErisim.DegerYap(form.ad) + ", ";
            sql = sql + VeriErisim.DegerYap(form.genelEvrakMi) + ", ";
            sql = sql + VeriErisim.DegerYap(form.ustBirim) + ",";
            sql = sql + VeriErisim.DegerYap(st.Length) + ", ";
            sql = sql + VeriErisim.DegerYap(0) + ", ";
            sql = sql + VeriErisim.DegerYap(form.kullanilmiyor) + ",";
            sql = sql + VeriErisim.DegerYap(form.ePosta) + ",";
            sql = sql + VeriErisim.DegerYap(form.kisiyeHavale) + ",";
            sql = sql + VeriErisim.DegerYap(form.kisaAd) + ",";
            sql = sql + VeriErisim.DegerYap(form.detayDagitim) + ",";
            sql = sql + VeriErisim.DegerYap(form.dagitimBirimKod) + ")";

            sonuc = KomutCalistir(sql, 1);

            if (!sonuc.islemSonuc)
            {
                veriErisim.hata = "";

                sql = "UPDATE " + TABLOAD + " SET ";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.IDAREKIMLIKKODU], form.idareKimlikKodu) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.AD], form.ad);
                sql += " WHERE " + VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", form.kod);

                sonuc = KomutCalistir(sql, 1);
            }

            return sonuc;
        }

        public string IdareKimlikKodGetir(string kod)
        {
            string sql = "SELECT " + ALANLAR[(int)ENUMAlanlar.IDAREKIMLIKKODU] + " FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, " WHERE", VeriErisim.KosulOl("", "KOD", "=", kod));

            return veriErisim.TekKolonSQLStr(sql);
        }
        public string BirimKodGetir(string idareKimlikKod)
        {
            string sql = "SELECT " + ALANLAR[(int)ENUMAlanlar.KOD] + " FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, " WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.IDAREKIMLIKKODU], "=", idareKimlikKod));

            return veriErisim.TekKolonSQLStr(sql);
        }
        public string UstBirimKodGetir(string kod)
        {
            string sql = "SELECT " + ALANLAR[(int)ENUMAlanlar.USTBIRIMKODU] + " FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, " WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", kod));

            return veriErisim.TekKolonSQLStr(sql);
        }
        public string DagitimBirimKodGetir(string kod)
        {
            string sql = "SELECT " + ALANLAR[(int)ENUMAlanlar.DAGITIMBIRIMKOD] + " FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, " WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", kod));

            return veriErisim.TekKolonSQLStr(sql);
        }
        public int BirimKullanimDurumu(string kod)
        {
            string sql = "SELECT " + ALANLAR[(int)ENUMAlanlar.KULLANILMIYOR] + " FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, " WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", kod));

            return veriErisim.TekKolonSQLInt(sql);
        }

        public IDataReader Listele(string birimKodListesi)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar("", ALANLAR) + " FROM " + TABLOAD;
            string kosul = "";

            string[] kodlar = birimKodListesi.Split(';');
            foreach (var kod in kodlar)
            {
                if (string.IsNullOrWhiteSpace(kod))
                    continue;
                kosul = VeriErisim.KosulEkle(kosul, "OR", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KOD], "=", kod));
            }

            if (kosul != "")
                sql += " WHERE " + kosul;

            sql += " ORDER BY " + ALANLAR[(int)ENUMAlanlar.AD] + " ASC";

            return veriErisim.DataReaderDondur(sql);
        }
    }
}
