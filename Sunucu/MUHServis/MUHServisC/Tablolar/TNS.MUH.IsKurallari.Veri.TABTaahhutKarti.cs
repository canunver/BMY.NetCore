using System;
using System.Collections.Generic;
using System.Data;
using TNS.Veri;

namespace TNS.MUH.IsKurallari.Veri
{
    /// <summary>
    /// MUHTAAHHUTKARTI veri tabanı tablosu ile ilgili, listeleme ve yazma işlemleri tutan nesne
    /// </summary>
    public class TABTaahhutKarti : TABOrtak
    {
        /// <summary>
        /// Sınıfın oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABTaahhutKarti(VeriErisim veriErisim) : base(veriErisim) { }

        public static string TABLOAD = "MUHTAAHHUTKARTI";

        public enum ENUMAlanlar
        {
            KOD = 0,
            MUHASEBEKOD = 1,
            PROJESIRANO = 2,
            DPTPROJESIRANO = 3,
            MUTEAHHITKIMLIKNO = 4,
            MUTEAHHITUNVAN = 5,
            MUTEAHHITADRES = 6,
            KONU = 7,
            KESIFBEDELI = 8,
            INDIRIMORANI = 9,
            IHALEBEDELI = 10,
            GERCEKLESENTAAHHUTTUTAR = 11,
            DAMGAVERGISITARIHI = 12,
            DAMGAVERGISINO = 13,
            DAMGAVERGISITUTAR = 14,
            SOZLESMEDVTARIH = 15,
            SOZLESMEDVNO = 16,
            SOZLESMEDVTUTAR = 17,
            SOZLESMETARIHI = 18,
            ISEBASLAMATARIHI = 19,
            GECICIKABULTARIHI = 20,
            ISBITIRMETARIHI = 21,
            SUREUZATIMI1 = 22,
            SUREUZATIM2 = 23,
            SUREUZATIMI3 = 24,
            ODENEKTERTIP = 25,
            TERTIPHESAPKOD = 26,
            DURANTERTIP = 27,
            ISLEMYAPANKOD = 28,
            ISLEMYAPANAD = 29,
            ISLEMTARIHI = 30,
            BIRIMKOD = 31,
            YIL = 32,
            ACIKLAMA = 33,
            ODEMEEMRIYIL = 34,
            ODEMEEMRINO = 35,
            KAPANDI = 36,
            PARABIRIMIKOD = 37,
            GARANTIBASTARIH = 38,
            GARANTIBITTARIH = 39,
            ISKOD = 40,
            SOZTUR = 41,
            ALIMUSULUKOD = 42,
            SGKBILDIRIMI = 43
        }

        static string[] ALANLAR ={ "KOD", "MUHASEBEKOD", "PROJESIRANO", "DPTPROJESIRANO", "MUTEAHHITKIMLIKNO", "MUTEAHHITUNVAN","MUTEAHHITADRES",
                                     "KONU", "KESIFBEDELI", "INDIRIMORANI", "IHALEBEDELI", "GERCEKLESENTAAHHUTTUTAR", "DAMGAVERGISITARIHI", "DAMGAVERGISINO",
                                 "DAMGAVERGISITUTAR","SOZLESMEDVTARIH", "SOZLESMEDVNO", "SOZLESMEDVTUTAR", "SOZLESMETARIHI", "ISEBASLAMATARIHI", "GECICIKABULTARIHI", "ISBITIRMETARIHI" ,"SUREUZATIMI1", "SUREUZATIMI2",
                                 "SUREUZATIMI3", "ODENEKTERTIP", "TERTIPHESAPKOD", "DURANTERTIP",
                                 "ISLEMYAPANKOD", "ISLEMYAPANAD", "ISLEMTARIHI","BIRIMKOD","YIL", "ACIKLAMA","ODEMEEMRIYIL","ODEMEEMRINO", "KAPANDI","PARABIRIMIKOD",
                                 "GARANTIBASTARIH","GARANTIBITTARIH","ISKOD", "SOZTUR", "ALIMUSULUKOD","SGKBILDIRIMI"};

        static string TABLOADDEGERARTISI = "MUHTAAHHUTKARTIDEGERARTISI";

        enum ENUMAlanlarDegerArtisi
        {
            KOD = 0,
            TAAHHUTKOD = 1,
            ONAYTARIHI = 2,
            SOZLESMETARIHI = 3,
            SOZLESMETUTAR = 4,
            DAMGAVERGISITARIHI = 5,
            DAMGAVERGISINO = 6,
            DAMGAVERGISITUTAR = 7,
            ODEMEEMRIYIL = 8,
            ODEMEEMRINO = 9,
            ISLEMYAPANKOD = 10,
            ISLEMYAPANAD = 11,
            ISLEMTARIHI = 12
        }

        static string[] ALANLARDEGERARTISI = { "KOD", "TAAHHUTKOD", "ONAYTARIHI", "SOZLESMETARIHI", "SOZLESMETUTAR", "DAMGAVERGISITARIHI", "DAMGAVERGISINO", "DAMGAVERGISITUTAR", "ODEMEEMRIYIL", "ODEMEEMRINO", "ISLEMYAPANKOD", "ISLEMYAPANAD", "ISLEMTARIHI" };


        /// <summary>
        /// Parametre olarak gelen bilgileri veri tabanına ekleyen sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="taahhut">The taahhut.</param>
        /// <param name="ekle">true:insert, false:update.</param>
        /// <returns></returns>
        public Sonuc Kaydet(TaahhutKartiBilgi taahhut, bool ekle)
        {
            Sonuc sonuc;
            string sql;

            if (ekle)
            {
                sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
                sql = sql + VeriErisim.DegerYap(taahhut.kod) + ", ";
                sql = sql + VeriErisim.DegerYap(taahhut.muhasebeKod) + ", ";
                sql = sql + VeriErisim.DegerYap(taahhut.projeSiraNo) + ", ";
                sql = sql + VeriErisim.DegerYap(taahhut.DptProjeSiraNo) + ", ";
                sql = sql + VeriErisim.DegerYap(taahhut.muteahhitKimlikNo) + ", ";
                sql = sql + VeriErisim.DegerYap(taahhut.muteahhitUnvan) + ", ";
                sql = sql + VeriErisim.DegerYap(taahhut.muteahhitAdres) + ", ";
                sql = sql + VeriErisim.DegerYap(taahhut.konu) + ", ";
                sql = sql + VeriErisim.DegerYap(taahhut.kesifBeldeli) + ", ";
                sql = sql + VeriErisim.DegerYap(taahhut.indirimOrani) + ", ";
                sql = sql + VeriErisim.DegerYap(taahhut.ihaleBedeli) + ", ";
                sql = sql + VeriErisim.DegerYap(taahhut.gerceklesenTaahhutTutar) + ", ";
                sql = sql + VeriErisim.DegerYap(taahhut.damgaVergisiTarihi) + ", ";
                sql = sql + VeriErisim.DegerYap(taahhut.damgaVergisiNo) + ", ";
                sql = sql + VeriErisim.DegerYap(taahhut.damgaVergisiTutar) + ", ";
                sql = sql + VeriErisim.DegerYap(taahhut.sozlesmeDVTarihi) + ", ";
                sql = sql + VeriErisim.DegerYap(taahhut.sozlesmeDVNo) + ", ";
                sql = sql + VeriErisim.DegerYap(taahhut.sozlesmeDVTutar) + ", ";
                sql = sql + VeriErisim.DegerYap(taahhut.sozlesmeTarihi) + ", ";
                sql = sql + VeriErisim.DegerYap(taahhut.iseBaslamaTarihi) + ", ";
                sql = sql + VeriErisim.DegerYap(taahhut.geciciKabulTarihi) + ", ";
                sql = sql + VeriErisim.DegerYap(taahhut.isBitirmeTarihi) + ", ";
                sql = sql + VeriErisim.DegerYap(taahhut.sureUzatimi1) + ", ";
                sql = sql + VeriErisim.DegerYap(taahhut.sureUzatimi2) + ", ";
                sql = sql + VeriErisim.DegerYap(taahhut.sureUzatimi3) + ", ";
                sql = sql + VeriErisim.DegerYap(taahhut.odenekTertip) + ", ";
                sql = sql + VeriErisim.DegerYap(taahhut.tertipHesapKod) + ", ";
                sql = sql + VeriErisim.DegerYap(taahhut.duranTertip) + ", ";
                sql = sql + VeriErisim.DegerYap(taahhut.islemYapanKod) + ", ";
                sql = sql + VeriErisim.DegerYap(taahhut.islemYapanAd) + ", ";
                sql = sql + VeriErisim.DegerYap(taahhut.islemTarihi) + ",";
                sql = sql + VeriErisim.DegerYap(taahhut.birimKod) + ",";
                sql = sql + VeriErisim.DegerYap(taahhut.yil) + ",";
                sql = sql + VeriErisim.DegerYap(taahhut.aciklama) + ",";
                sql = sql + VeriErisim.DegerYap(taahhut.odemeEmriYil) + ",";
                sql = sql + VeriErisim.DegerYap(taahhut.odemeEmriNo) + ",";
                sql = sql + VeriErisim.DegerYap(taahhut.kapandi) + ",";
                sql = sql + VeriErisim.DegerYap(taahhut.parabirimiKod) + ",";
                sql = sql + VeriErisim.DegerYap(taahhut.garantiBaslamaTarihi) + ",";
                sql = sql + VeriErisim.DegerYap(taahhut.garantiBitisTarihi) + ",";
                sql = sql + VeriErisim.DegerYap(taahhut.isKod) + ",";
                sql = sql + VeriErisim.DegerYap((int)taahhut.sozTur) + ",";
                sql = sql + VeriErisim.DegerYap(taahhut.alimUsuluKod) + ",";
                sql = sql + VeriErisim.DegerYap(taahhut.sgkBildirimi) + ")";

                sonuc = KomutCalistir(sql, 1);
                sonuc.bilgiStr = "Yeni";
            }
            else
            {
                sql = "UPDATE " + TABLOAD + " SET ";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.YIL], taahhut.yil) + ", ";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], taahhut.muhasebeKod) + ", ";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.BIRIMKOD], taahhut.birimKod) + ", ";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.PROJESIRANO], taahhut.projeSiraNo) + ", ";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.DPTPROJESIRANO], taahhut.DptProjeSiraNo) + ", ";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.MUTEAHHITKIMLIKNO], taahhut.muteahhitKimlikNo) + ", ";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.MUTEAHHITUNVAN], taahhut.muteahhitUnvan) + ", ";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.MUTEAHHITADRES], taahhut.muteahhitAdres) + ", ";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.KONU], taahhut.konu) + ", ";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.KESIFBEDELI], taahhut.kesifBeldeli) + ", ";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.INDIRIMORANI], taahhut.indirimOrani) + ", ";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.IHALEBEDELI], taahhut.ihaleBedeli) + ", ";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.GERCEKLESENTAAHHUTTUTAR], taahhut.gerceklesenTaahhutTutar) + ", ";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.DAMGAVERGISITARIHI], taahhut.damgaVergisiTarihi) + ", ";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.DAMGAVERGISINO], taahhut.damgaVergisiNo) + ", ";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.DAMGAVERGISITUTAR], taahhut.damgaVergisiTutar) + ", ";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.SOZLESMEDVTARIH], taahhut.sozlesmeDVTarihi) + ", ";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.SOZLESMEDVNO], taahhut.sozlesmeDVNo) + ", ";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.SOZLESMEDVTUTAR], taahhut.sozlesmeDVTutar) + ", ";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.SOZLESMETARIHI], taahhut.sozlesmeTarihi) + ", ";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ISEBASLAMATARIHI], taahhut.iseBaslamaTarihi) + ", ";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.GECICIKABULTARIHI], taahhut.geciciKabulTarihi) + ", ";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ISBITIRMETARIHI], taahhut.isBitirmeTarihi) + ", ";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.SUREUZATIMI1], taahhut.sureUzatimi1) + ", ";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.SUREUZATIM2], taahhut.sureUzatimi2) + ", ";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.SUREUZATIMI3], taahhut.sureUzatimi3) + ", ";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ODENEKTERTIP], taahhut.odenekTertip) + ", ";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.TERTIPHESAPKOD], taahhut.tertipHesapKod) + ", ";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.DURANTERTIP], taahhut.duranTertip) + ", ";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ACIKLAMA], taahhut.aciklama) + ", ";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.PARABIRIMIKOD], taahhut.parabirimiKod) + ", ";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.GARANTIBASTARIH], taahhut.garantiBaslamaTarihi) + ", ";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.GARANTIBITTARIH], taahhut.garantiBitisTarihi) + ", ";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ALIMUSULUKOD], taahhut.alimUsuluKod) + ", ";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.SGKBILDIRIMI], taahhut.sgkBildirimi) + ", ";

                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ISLEMYAPANKOD], taahhut.islemYapanKod) + ", ";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ISLEMYAPANAD], taahhut.islemYapanAd) + ", ";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ISLEMTARIHI], taahhut.islemTarihi) + ",";

                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ISKOD], taahhut.isKod);

                sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", taahhut.kod));

                sonuc = KomutCalistir(sql, 1);
                sonuc.bilgiStr = "Güncelleme";
            }
            sonuc.anahtar = taahhut.kod.ToString();
            return sonuc;
        }

        public Sonuc KaydetDegerArtisiOdemeEmri(List<string> kodlar, int odemeEmriYil, string odemeEmriNo)
        {
            string sql = "UPDATE " + TABLOADDEGERARTISI + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLARDEGERARTISI[(int)ENUMAlanlarDegerArtisi.ODEMEEMRIYIL], odemeEmriYil) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLARDEGERARTISI[(int)ENUMAlanlarDegerArtisi.ODEMEEMRINO], odemeEmriNo);
            sql = VeriErisim.KosulEkle(sql, "WHERE", ALANLARDEGERARTISI[(int)ENUMAlanlarDegerArtisi.KOD] + " IN (" + MetneCevir(kodlar) + ")");

            return KomutCalistir(sql, 0);
        }

        private string MetneCevir(List<string> kartKodlar)
        {
            string m = "";
            foreach (string item in kartKodlar)
            {
                if (m != "") m += ",";
                m += VeriErisim.DegerYap(item);
            }
            return m;
        }

        public Sonuc KaydetDegerArtisiOdemeEmri(string kod, int odemeEmriYil, string odemeEmriNo)
        {
            string sql = "UPDATE " + TABLOADDEGERARTISI + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLARDEGERARTISI[(int)ENUMAlanlarDegerArtisi.ODEMEEMRIYIL], odemeEmriYil) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLARDEGERARTISI[(int)ENUMAlanlarDegerArtisi.ODEMEEMRINO], odemeEmriNo);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARDEGERARTISI[(int)ENUMAlanlarDegerArtisi.KOD], "=", kod));

            return KomutCalistir(sql, 1);
        }

        public Sonuc KaydetOdemeEmri(List<int> kartKodlar, int odemeEmriYil, string odemeEmriNo)
        {
            string sql = "UPDATE " + TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ODEMEEMRIYIL], odemeEmriYil) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ODEMEEMRINO], odemeEmriNo);
            sql = VeriErisim.KosulEkle(sql, "WHERE", ALANLAR[(int)ENUMAlanlar.KOD] + " IN (" + MetneCevir(kartKodlar) + ")");

            return KomutCalistir(sql, 0);
        }

        private string MetneCevir(List<int> kartKodlar)
        {
            string m = "";
            foreach (int item in kartKodlar)
            {
                if (m != "") m += ",";
                m += item.ToString();
            }
            return m;
        }

        public Sonuc KaydetOdemeEmri(int kartYil, int kartKod, int odemeEmriYil, string odemeEmriNo)
        {
            string sql = "UPDATE " + TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ODEMEEMRIYIL], odemeEmriYil) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ODEMEEMRINO], odemeEmriNo);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", kartYil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", kartKod));

            return KomutCalistir(sql, 1);
        }
        /// <summary>
        /// Parametre bilgilerine uygun Tablodaki kayıtları silen sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="kod">kod.</param>
        /// <returns></returns>
        public Sonuc Sil(int kod)
        {
            string sql = "DELETE FROM " + TABLOAD + " WHERE ";
            sql += VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", kod + " ");

            return KomutCalistir(sql, 0);
        }

        /// <summary>
        /// Parametre bilgilerine uygun Tablodaki kayıtları silen sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="kod">kod.</param>
        /// <returns></returns>
        public Sonuc Kapat(int kod, int durum, TNSDateTime iseBaslamaTarihi)
        {
            string sql = "UPDATE " + TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.KAPANDI], durum);
            if (durum == 9)
            {
                DateTime isBitis = iseBaslamaTarihi.Oku().AddDays(-1);
                sql += ", " + VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ISBITIRMETARIHI], new TNSDateTime(isBitis));
            }
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", kod));

            return KomutCalistir(sql, 1);
        }

        public string KesisenSozlesmeBul(TaahhutKartiBilgi bilgi)
        {
            string sql = "SELECT MAX(" + ALANLAR[(int)ENUMAlanlar.PROJESIRANO] + ") FROM " + TABLOAD;

            sql += " WHERE " + VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.MUTEAHHITKIMLIKNO], "=", bilgi.muteahhitKimlikNo);
            sql += " AND " + VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.SOZTUR], "=", (int)bilgi.sozTur);
            sql += " AND " + VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KOD], "<>", bilgi.kod);
            sql += " AND " + string.Format("InsanKaynaklari.dbo.DonemlerKesisiyorMu({0},{1},{2},{3}) = 1", VeriErisim.DegerYap(bilgi.iseBaslamaTarihi), VeriErisim.DegerYap(bilgi.isBitirmeTarihi), TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.ISEBASLAMATARIHI], TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.ISBITIRMETARIHI]);

            return veriErisim.TekKolonSQLStr(sql);
        }

        /// <summary>
        /// Parametre olarak gelen alanları kıstas olarak kullanarak, select sql cümlesini oluşturup, çalıştıran ve sonucunu DataReader olarak döndüren fonksiyon
        /// </summary>
        /// <param name="bilgi">bilgi.</param>
        /// <returns></returns>
        public IDataReader Listele(TaahhutKartiBilgi bilgi)
        {
            if (bilgi.kod > 0)
                bilgi.kapandi = -1;

            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR) + $", {TABOdemeEmriMIF.TABLOADMIFF}.FORMDURUM";
            sql += " ,(select SUM(BORC)   FROM MUHMIF AS MIF, MUHMIFFORM AS MIFFORM WHERE MIFFORM.YIL = MIF.YIL AND MIFFORM.BELGENO = MIF.BELGENO AND MIFFORM.FORMDURUM<>9 AND (MIFFORM.PROJENO = MUHTAAHHUTKARTI.PROJESIRANO) AND MIF.HESAPKOD LIKE '920%' AND (MIF.ALTHESAPKOD = MUHTAAHHUTKARTI.MUTEAHHITKIMLIKNO or MIFFORM.IKIMLIKNO = MUHTAAHHUTKARTI.MUTEAHHITKIMLIKNO))";
            sql += " ,(select SUM(ALACAK) FROM MUHMIF AS MIF, MUHMIFFORM AS MIFFORM WHERE MIFFORM.YIL = MIF.YIL AND MIFFORM.BELGENO = MIF.BELGENO AND MIFFORM.FORMDURUM<>9 AND (MIFFORM.PROJENO = MUHTAAHHUTKARTI.PROJESIRANO) AND MIF.HESAPKOD LIKE '920%' AND (MIF.ALTHESAPKOD = MUHTAAHHUTKARTI.MUTEAHHITKIMLIKNO or MIFFORM.IKIMLIKNO = MUHTAAHHUTKARTI.MUTEAHHITKIMLIKNO))";
            sql += " FROM " + TABLOAD;
            sql += " LEFT OUTER JOIN " + TABOdemeEmriMIF.TABLOADMIFF + $" ON {TABOdemeEmriMIF.TABLOADMIFF}.YIL = {TABLOAD}.ODEMEEMRIYIL AND {TABOdemeEmriMIF.TABLOADMIFF}.BELGENO = {TABLOAD}.ODEMEEMRINO";

            string kosul = "";
            if (TNSDateTime.IsNull(bilgi.arananTarih))
            {
                if (bilgi.yil > 0)
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.YIL], "=", bilgi.yil));
                if (bilgi.kapandi > -1)
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KAPANDI], "=", bilgi.kapandi));
            }
            else
            {
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ISEBASLAMATARIHI], "<=", bilgi.arananTarih));
                kosul = VeriErisim.KosulEkle(kosul, "AND", "(" + VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ISBITIRMETARIHI], ">=", bilgi.arananTarih) + " OR " + ALANLAR[(int)ENUMAlanlar.ISBITIRMETARIHI] + " IS NULL)");
            }
            if (bilgi.muhasebeKod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", bilgi.muhasebeKod));

            if (bilgi.birimKod != "")
            {
                string icKosul = "";
                string[] birimler = bilgi.birimKod.Split(';');

                foreach (string birim in birimler)
                {
                    if (birim.Trim() != "")
                        icKosul = VeriErisim.KosulEkle(icKosul, "OR", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.BIRIMKOD], "=", birim));
                }
                if (icKosul != "")
                    kosul = VeriErisim.KosulEkle(kosul, "AND", "(" + icKosul + ")");
            }

            if (!bilgi.sozlesmeTarihi.isNull)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", "YEAR(" + ALANLAR[(int)ENUMAlanlar.SOZLESMETARIHI] + ")", "=", bilgi.sozlesmeTarihi.Yil));
            if (!string.IsNullOrEmpty(bilgi.muteahhitKimlikNo))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.MUTEAHHITKIMLIKNO], "=", bilgi.muteahhitKimlikNo));
            if (!string.IsNullOrEmpty(bilgi.muteahhitUnvan))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.MUTEAHHITUNVAN], "LIKE", "%" + bilgi.muteahhitUnvan + "%"));
            if (!string.IsNullOrEmpty(bilgi.projeSiraNo))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.PROJESIRANO], "=", bilgi.projeSiraNo));
            if (!string.IsNullOrEmpty(bilgi.konu))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KONU], "LIKE", "%" + bilgi.konu + "%"));
            if (bilgi.kod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KOD], "=", bilgi.kod));
            if (bilgi.sozTur != TaahhutKartiBilgi.ENUMSozTur.Hepsi)
            {
                string kucukEsit;
                if (bilgi.sozTur == TaahhutKartiBilgi.ENUMSozTur.Normal)
                    kucukEsit = "<=";
                else
                    kucukEsit = "=";
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.SOZTUR], kucukEsit, (int)bilgi.sozTur));
            }

            if (!bilgi.sorguSozlesmeTarihBas.isNull)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SOZLESMETARIHI], ">=", bilgi.sorguSozlesmeTarihBas));

            if (!bilgi.sorguSozlesmeTarihBit.isNull)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SOZLESMETARIHI], "<=", bilgi.sorguSozlesmeTarihBit));

            if (bilgi.isKod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ISKOD], "=", bilgi.isKod));

            if (!string.IsNullOrWhiteSpace(bilgi.odenekTertip))
            {
                string odenekTertip = bilgi.odenekTertip;
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ODENEKTERTIP], "LIKE", "%" + odenekTertip + "%"));
            }

            if (kosul != "")
                sql += " WHERE " + kosul;

            sql += " ORDER BY KAPANDI,YIL,PROJESIRANO";

            IDataReader r = veriErisim.DataReaderDondur(sql);
            return r;
        }

        /// <summary>
        /// Parametre olarak gelen bilgileri veri tabanına ekleyen sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="da">The taahhut.</param>
        /// <param name="ekle">true:insert, false:update.</param>
        /// <returns></returns>
        public Sonuc KaydetDegerArtisi(TaahhutKartiDegerArtisi da, bool ekle)
        {
            string sql;

            if (ekle)
            {
                sql = "INSERT INTO " + TABLOADDEGERARTISI + " (" + VeriErisim.TumAlanlar("", ALANLARDEGERARTISI) + ") VALUES (";
                sql = sql + VeriErisim.DegerYap(da.kod) + ", ";
                sql = sql + VeriErisim.DegerYap(da.taahhutKod) + ", ";
                sql = sql + VeriErisim.DegerYap(da.onayTarihi) + ", ";
                sql = sql + VeriErisim.DegerYap(da.sozlesmeTarihi) + ", ";
                sql = sql + VeriErisim.DegerYap(da.sozlesmeTutari) + ", ";
                sql = sql + VeriErisim.DegerYap(da.damgaVergisiTarihi) + ", ";
                sql = sql + VeriErisim.DegerYap(da.damgaVergisiNo) + ", ";
                sql = sql + VeriErisim.DegerYap(da.damgaVergisiTutari) + ", ";
                sql = sql + VeriErisim.DegerYap(da.odemeEmriYil) + ", ";
                sql = sql + VeriErisim.DegerYap(da.odemeEmriNo) + ", ";
                sql = sql + VeriErisim.DegerYap(da.islemYapanKod) + ", ";
                sql = sql + VeriErisim.DegerYap(da.islemYapanAd) + ", ";
                sql = sql + VeriErisim.DegerYap(da.islemTarihi) + ")";
            }
            else
            {
                sql = "UPDATE " + TABLOADDEGERARTISI + " SET ";
                sql += VeriErisim.AtamaOl("", ALANLARDEGERARTISI[(int)ENUMAlanlarDegerArtisi.ONAYTARIHI], da.onayTarihi) + ", ";
                sql += VeriErisim.AtamaOl("", ALANLARDEGERARTISI[(int)ENUMAlanlarDegerArtisi.SOZLESMETARIHI], da.sozlesmeTarihi) + ", ";
                sql += VeriErisim.AtamaOl("", ALANLARDEGERARTISI[(int)ENUMAlanlarDegerArtisi.SOZLESMETUTAR], da.sozlesmeTutari) + ", ";

                sql += VeriErisim.AtamaOl("", ALANLARDEGERARTISI[(int)ENUMAlanlarDegerArtisi.DAMGAVERGISITARIHI], da.damgaVergisiTarihi) + ", ";
                sql += VeriErisim.AtamaOl("", ALANLARDEGERARTISI[(int)ENUMAlanlarDegerArtisi.DAMGAVERGISINO], da.damgaVergisiNo) + ", ";
                sql += VeriErisim.AtamaOl("", ALANLARDEGERARTISI[(int)ENUMAlanlarDegerArtisi.DAMGAVERGISITUTAR], da.damgaVergisiTutari) + ", ";

                sql += VeriErisim.AtamaOl("", ALANLARDEGERARTISI[(int)ENUMAlanlarDegerArtisi.ISLEMYAPANKOD], da.islemYapanKod) + ", ";
                sql += VeriErisim.AtamaOl("", ALANLARDEGERARTISI[(int)ENUMAlanlarDegerArtisi.ISLEMYAPANAD], da.islemYapanAd) + ", ";
                sql += VeriErisim.AtamaOl("", ALANLARDEGERARTISI[(int)ENUMAlanlarDegerArtisi.ISLEMTARIHI], da.islemTarihi);

                sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", "KOD", "=", da.kod));
            }
            return KomutCalistir(sql, 1);
        }

        public Sonuc SilDegerArtisi(int taahhutKod)
        {
            string sql = "DELETE FROM " + TABLOADDEGERARTISI + " WHERE ";
            sql += VeriErisim.KosulOl("", ALANLARDEGERARTISI[(int)ENUMAlanlarDegerArtisi.TAAHHUTKOD], "=", taahhutKod);

            return KomutCalistir(sql, 0);
        }

        public IDataReader ListeleDegerArtisi(int taahhutKod)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOADDEGERARTISI, ALANLARDEGERARTISI) + $", {TABOdemeEmriMIF.TABLOADMIFF}.FORMDURUM FROM " + TABLOADDEGERARTISI;
            sql += " LEFT OUTER JOIN " + TABOdemeEmriMIF.TABLOADMIFF + $" ON {TABOdemeEmriMIF.TABLOADMIFF}.YIL = {TABLOADDEGERARTISI}.ODEMEEMRIYIL AND {TABOdemeEmriMIF.TABLOADMIFF}.BELGENO = {TABLOADDEGERARTISI}.ODEMEEMRINO";

            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOADDEGERARTISI, ALANLARDEGERARTISI[(int)ENUMAlanlarDegerArtisi.TAAHHUTKOD], "=", taahhutKod));

            IDataReader r = veriErisim.DataReaderDondur(sql);
            return r;
        }


        /// <summary>
        /// Siras the no bul.
        /// </summary>
        /// <param name="kod">The kod.</param>
        /// <returns></returns>
        public int SiraNoBul(int kod)
        {
            string sql = "SELECT MAX(" + ALANLAR[(int)ENUMAlanlar.KOD] + ") FROM " + TABLOAD;
            if (kod != -1)
            {
                sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", kod));
                return (int)veriErisim.TekKolonSQLDbl(sql);
            }
            else
                return (int)veriErisim.TekKolonSQLDbl(sql) + 1;
        }

        public bool TaahhutKapaliMi(int kod)
        {
            string sql = "SELECT COUNT(*)  FROM " + TABLOAD;

            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", kod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KAPANDI], "=", 1));
            if (veriErisim.TekKolonSQLInt(sql) > 0)
                return true;
            else
                return false;
        }

        public bool IsKullanildiMi(int isKod)
        {
            string sql = "SELECT COUNT(*) FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", "ISKOD", "=", isKod));

            return veriErisim.TekKolonSQLInt(sql) > 0;
        }
    }
}