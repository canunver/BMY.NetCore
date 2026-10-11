using System;
using System.Data;
using TNS;
using TNS.Veri;
using TNS.EVR;
namespace TNS.EVR.IsKurallari.Veri
{
    /// <summary>
    /// Evrak hareket işlem bilgilerini içeren EVREVRAKHAREKET veri tabanı tablosu ile ilgili, kaydetme ve listeleme yordamlarını tutan sınıf.
    /// </summary>
    public class TABEvrakHareket : TABOrtak
    {
        /// <summary>        
        /// Parametre olarak verilen veriErisim nesnesine göre, TABEvrakHareket sınıfının yeni bir örneğini oluşturur.
        /// </summary>
        /// <param name="veriErisim">Veri erişim bilgisini içeren nesne</param>
        public TABEvrakHareket(VeriErisim veriErisim) : base(veriErisim) { }

        /// <summary>
        /// Evrak hareket işlem bilgilerini içeren veri tabanı tablosunun adını tutan değişken.
        /// </summary>
        public static string TABLOAD = "EVREVRAKHAREKET";

        /// <summary>
        /// Evrak hareket işlem bilgilerini içeren veri tabanı tablosunun alanlarının sırasını tutan numaralandırma.
        /// </summary>
        public enum ENUMAlanlar : int
        {
            PREVRAKNO = 0,
            SIRANO = 1,
            BIRIMKOD1 = 2,
            BIRIMKOD2 = 3,
            KISI1 = 4,
            KISI2 = 5,
            ISLEMTARIH = 6,
            ISLEMYAPAN = 7,
            ISLEM = 8,
            HAVALENOTU = 9,
            OKUNDU = 10,
            KISI1AD = 11,
            KISI2AD = 12,
            ISLEMYAPANAD = 13,
            VEKIL = 14,
            VEKILAD = 15,
            ERISEMEZ = 16,
        }

        /// <summary>
        /// Evrak hareket işlem bilgilerini içeren veri tabanı tablosunun alanlarının isimlerini tutan dizi.
        /// </summary>
        public static string[] ALANLAR = { "PREVRAKNO", "SIRANO", "BIRIMKOD1", "BIRIMKOD2", "KISI1", "KISI2", "ISLEMTARIH", "ISLEMYAPAN", "ISLEM", "HAVALENOTU", "OKUNDU", "KISI1AD", "KISI2AD", "ISLEMYAPANAD", "VEKIL", "VEKILAD", "ERISEMEZ" };

        /// <summary>
        /// Parametre olarak verilen evrak hareket bilgisine göre; evrak hareket bilgilerini, sql cümlesi hazırlayıp çalıştırarak veritabanına kaydeden yordam.
        /// </summary>
        /// <param name="form">Evrak hareket bilgisini içeren nesne</param>
        /// <returns>İşlem sonucu Sonuc nesnesi olarak döndürülür.</returns>
        public Sonuc Kaydet(EvrakHareket form)
        {
            Sonuc sonuc = new Sonuc(false, "", "");

            string sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
            sql += VeriErisim.DegerYap(form.prEvrakNo) + ",";
            sql += VeriErisim.DegerYap(form.siraNo) + ",";
            sql += VeriErisim.DegerYap(form.birimKod1) + ",";
            sql += VeriErisim.DegerYap(form.birimKod2) + ",";
            sql += VeriErisim.DegerYap(form.kisi1) + ",";
            sql += VeriErisim.DegerYap(form.kisi2) + ",";
            sql += VeriErisim.DegerYap(form.islemTarih) + ",";
            sql += VeriErisim.DegerYap(form.islemYapan) + ",";
            sql += VeriErisim.DegerYap(form.islem) + ",";
            sql += VeriErisim.DegerYap(form.havaleNotu) + ",";
            sql += VeriErisim.DegerYap(form.okundu) + ",";
            sql += VeriErisim.DegerYap(form.kisi1Ad) + ",";
            sql += VeriErisim.DegerYap(form.kisi2Ad) + ",";
            sql += VeriErisim.DegerYap(form.islemYapanAd) + ",";
            sql += VeriErisim.DegerYap(form.vekil) + ",";
            sql += VeriErisim.DegerYap(form.vekilAd) + ",";
            sql += VeriErisim.DegerYap(form.erisemez) + ")";

            sonuc = KomutCalistir(sql, 1);

            if (!sonuc.islemSonuc)
            {
                if (form.islem == (int)ENUMEvrakHareketDurum.NUMARAALINDI)
                    OrtakFonksiyonlar.HataStrYaz("no almada hareket hatası:" + sonuc.hataStr);
            }

            if (sonuc.islemSonuc && form.islem != (int)ENUMEvrakHareketDurum.NUMARAALINDI)
                KaydetSonDurum(form);

            return sonuc;
        }

        public Sonuc KaydetSonDurum(EvrakHareket form)
        {
            Sonuc sonuc = new Sonuc(false, "", "");

            string sql = "UPDATE " + "EVREVRAKHAREKETSONDURUM" + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.SIRANO], form.siraNo) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.BIRIMKOD1], form.birimKod1) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.BIRIMKOD2], form.birimKod2) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.KISI1], form.kisi1) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.KISI2], form.kisi2) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ISLEMTARIH], form.islemTarih) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ISLEMYAPAN], form.islemYapan) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ISLEM], form.islem) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.HAVALENOTU], form.havaleNotu) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.OKUNDU], form.okundu) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.KISI1AD], form.kisi1Ad) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.KISI2AD], form.kisi2Ad) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ISLEMYAPANAD], form.islemYapanAd) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.VEKIL], form.vekil) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.VEKILAD], form.vekilAd);
            sql += " WHERE " + VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.PREVRAKNO], "=", form.prEvrakNo);
            sonuc = KomutCalistir(sql, 1);

            if (!sonuc.islemSonuc)
            {
                sql = "INSERT INTO " + "EVREVRAKHAREKETSONDURUM" + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
                sql += VeriErisim.DegerYap(form.prEvrakNo) + ",";
                sql += VeriErisim.DegerYap(form.siraNo) + ",";
                sql += VeriErisim.DegerYap(form.birimKod1) + ",";
                sql += VeriErisim.DegerYap(form.birimKod2) + ",";
                sql += VeriErisim.DegerYap(form.kisi1) + ",";
                sql += VeriErisim.DegerYap(form.kisi2) + ",";
                sql += VeriErisim.DegerYap(form.islemTarih) + ",";
                sql += VeriErisim.DegerYap(form.islemYapan) + ",";
                sql += VeriErisim.DegerYap(form.islem) + ",";
                sql += VeriErisim.DegerYap(form.havaleNotu) + ",";
                sql += VeriErisim.DegerYap(form.okundu) + ",";
                sql += VeriErisim.DegerYap(form.kisi1Ad) + ",";
                sql += VeriErisim.DegerYap(form.kisi2Ad) + ",";
                sql += VeriErisim.DegerYap(form.islemYapanAd) + ",";
                sql += VeriErisim.DegerYap(form.vekil) + ",";
                sql += VeriErisim.DegerYap(form.vekilAd) + ",";
                sql += VeriErisim.DegerYap(form.erisemez) + ")";
                sonuc = KomutCalistir(sql, 1);
            }

            return sonuc;
        }

        public Sonuc OkunduOlarakIsaretleSonDurum(EvrakHareket form)
        {
            string sql = "UPDATE " + "EVREVRAKHAREKETSONDURUM" + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.OKUNDU], 1);
            sql += " WHERE " + VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.PREVRAKNO], "=", form.prEvrakNo);
            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Parametre olarak verilen bilgiye göre; evrak hareket bilgisine okundu bilgisini, sql cümlesi hazırlayıp çalıştırarak veritabanına kaydeden yordam.
        /// </summary>
        /// <param name="form">Evrak hareket bilgisini içeren nesne</param>
        /// <returns>İşlem sonucu Sonuc nesnesi olarak döndürülür.</returns>
        public Sonuc OkunduOlarakIsaretle(EvrakHareket form)
        {
            Sonuc sonuc = new Sonuc(false, "", "");
            string sql = "UPDATE " + TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.OKUNDU], 1);
            sql += " WHERE " + VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.PREVRAKNO], "=", form.prEvrakNo);
            sql += " AND " + VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.ISLEM], "=", form.islem);
            sql += " AND " + VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.BIRIMKOD1], "=", form.birimKod1);
            sql += " AND " + VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.BIRIMKOD2], "=", form.birimKod2);
            sql += " AND " + VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KISI1], "=", form.kisi1);
            sql += " AND " + VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KISI2], "=", form.kisi2);

            sonuc = KomutCalistir(sql, 1);
            if (sonuc.islemSonuc)
                OkunduOlarakIsaretleSonDurum(form);

            return sonuc;
        }

        /// <summary>
        /// Parametre olarak verilen evrak bilgisine göre; evrak sayısını, sql cümlesi hazırlayıp çalıştırarak veritabanından okuyan yordam.
        /// </summary>
        /// <param name="form">Evrak bilgisini içeren nesne</param>
        /// <returns>Evrak sayısı double olarak döndürülür.</returns>
        public double EvrakSayisiVer(EvrakSinif form)
        {
            string sql = "SELECT COUNT(*) FROM  EVREVRAKSICIL INNER JOIN ";
            sql += " EVREVRAK ON EVREVRAKSICIL.EVREVRAKNO = EVREVRAK.EVREVRAKNO INNER JOIN ";
            sql += " EVREVRAKHAREKETSONDURUM ON EVREVRAKSICIL.PREVRAKNO = EVREVRAKHAREKETSONDURUM.PREVRAKNO ";

            string ekKosul = "";

            if (form.evrakTipi == (int)ENUMEvrakTipi.GELEN || form.evrakTipi == (int)ENUMEvrakTipi.GIDEN)
                sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", "EVRAKTIPI", "=", form.evrakTipi));
            if (form.hareket.kisi1 != "")
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("EVREVRAKHAREKETSONDURUM", ALANLAR[(int)ENUMAlanlar.KISI1], "=", form.hareket.kisi1));
            if (form.hareket.kisi1Ad != "")
            {
                ekKosul = "";
                ekKosul = VeriErisim.KosulEkle(ekKosul, "AND", VeriErisim.KosulOl("EVREVRAKHAREKETSONDURUM", ALANLAR[(int)ENUMAlanlar.KISI1], "=", ""));
                ekKosul = VeriErisim.KosulEkle(ekKosul, "OR", VeriErisim.KosulOl("EVREVRAKHAREKETSONDURUM", ALANLAR[(int)ENUMAlanlar.KISI1], "=", form.hareket.kisi1Ad));
                sql += " AND (" + ekKosul + ")";
            }
            if (form.hareket.kisi2 != "")
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("EVREVRAKHAREKETSONDURUM", ALANLAR[(int)ENUMAlanlar.KISI2], "=", form.hareket.kisi2));
            if (form.hareket.kisi2Ad != "")
            {
                ekKosul = "";
                ekKosul = VeriErisim.KosulEkle(ekKosul, "AND", VeriErisim.KosulOl("EVREVRAKHAREKETSONDURUM", ALANLAR[(int)ENUMAlanlar.KISI2], "=", ""));
                ekKosul = VeriErisim.KosulEkle(ekKosul, "OR", VeriErisim.KosulOl("EVREVRAKHAREKETSONDURUM", ALANLAR[(int)ENUMAlanlar.KISI2], "=", form.hareket.kisi2Ad));
                sql += " AND (" + ekKosul + ")";
            }
            if (form.hareket.okundu > -1)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("EVREVRAKHAREKETSONDURUM", ALANLAR[(int)ENUMAlanlar.OKUNDU], "=", 0));
            if (form.hareket.birimKod1 != "")
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("EVREVRAKHAREKETSONDURUM", ALANLAR[(int)ENUMAlanlar.BIRIMKOD1], "=", form.hareket.birimKod1));
            if (form.hareket.birimKod2 != "")
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("EVREVRAKHAREKETSONDURUM", ALANLAR[(int)ENUMAlanlar.BIRIMKOD2], "=", form.hareket.birimKod2));
            if (form.cevapSuresi > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", "CEVAPSURESI", ">", 0));

            //sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("EVREVRAKHAREKETSONDURUM", ALANLAR[(int)ENUMAlanlar.PREVRAKNO], "LIKE", form.hareket.prEvrakNo));//prEvrakNo içinde yıl bilgisi geliyor

            if (form.hareket.islem == (int)ENUMEvrakHareketDurum.KISIUZERINDE)
            {
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("EVREVRAKHAREKETSONDURUM", ALANLAR[(int)ENUMAlanlar.ISLEM], "<", 50));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("EVREVRAKHAREKETSONDURUM", ALANLAR[(int)ENUMAlanlar.ISLEM], "<>", (int)ENUMEvrakHareketDurum.IMZAYAGONDER));
            }
            else if (form.hareket.islem == (int)ENUMEvrakTarihceDurum.IMZAYAGELENLER)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("EVREVRAKHAREKETSONDURUM", ALANLAR[(int)ENUMAlanlar.ISLEM], "=", (int)ENUMEvrakHareketDurum.IMZAYAGONDER));
            else if (form.hareket.islem == (int)ENUMEvrakTarihceDurum.HAVALEGELEN)
            {
                string havaleGelenKosul = "";
                havaleGelenKosul = VeriErisim.KosulEkle(havaleGelenKosul, "AND", VeriErisim.KosulOl("EVREVRAKHAREKETSONDURUM", ALANLAR[(int)ENUMAlanlar.ISLEM], "=", (int)ENUMEvrakHareketDurum.HAVALEEDILDI));
                havaleGelenKosul = VeriErisim.KosulEkle(havaleGelenKosul, "OR", VeriErisim.KosulOl("EVREVRAKHAREKETSONDURUM", ALANLAR[(int)ENUMAlanlar.ISLEM], "=", (int)ENUMEvrakHareketDurum.DAGITIMKAYIT));
                sql += " AND (" + havaleGelenKosul + ")";
            }
            else
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("EVREVRAKHAREKETSONDURUM", ALANLAR[(int)ENUMAlanlar.ISLEM], "=", form.hareket.islem));

            string kisi = form.hareket.islem != 51 ? form.hareket.kisi1 : form.hareket.kisi2;

            return veriErisim.TekKolonSQLDbl(sql);
        }

        public double EvrakSayisiVerDevamEdenIsler(EvrakSinif form)
        {
            string sql = "SELECT COUNT(*) FROM EVREVRAK INNER JOIN EVREVRAKSICIL TS1 ON EVREVRAK.EVREVRAKNO = TS1.EVREVRAKNO ";
            sql += "INNER JOIN EVREVRAKHAREKET ON TS1.PREVRAKNO = EVREVRAKHAREKET.PREVRAKNO ";
            sql += "INNER JOIN EVREVRAKHAREKETSONDURUM ON TS1.PREVRAKNO = EVREVRAKHAREKETSONDURUM.PREVRAKNO ";


            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", "EVRAKTIPI", "=", form.evrakTipi));

            if (form.hareket.birimKod1 != "")
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("EVREVRAKHAREKET", ALANLAR[(int)ENUMAlanlar.BIRIMKOD1], "=", form.hareket.birimKod1));
            if (form.hareket.kisi1 != "")
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("EVREVRAKHAREKET", ALANLAR[(int)ENUMAlanlar.KISI1], "=", form.hareket.kisi1));

            string ekKosul = "";
            if (form.hareket.kisi1Ad != "")
            {
                ekKosul = "";
                ekKosul = VeriErisim.KosulEkle(ekKosul, "AND", VeriErisim.KosulOl("EVREVRAKHAREKET", ALANLAR[(int)ENUMAlanlar.KISI1], "=", ""));
                ekKosul = VeriErisim.KosulEkle(ekKosul, "OR", VeriErisim.KosulOl("EVREVRAKHAREKET", ALANLAR[(int)ENUMAlanlar.KISI1], "=", form.hareket.kisi1Ad));
                sql += " AND (" + ekKosul + ")";
            }

            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("EVREVRAK", "YIL", "=", form.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("TS1", "ASIL", "=", 1));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("EVREVRAKHAREKET", ALANLAR[(int)ENUMAlanlar.ISLEM], "=", (int)ENUMEvrakHareketDurum.HAVALEEDILDI));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("EVREVRAKHAREKETSONDURUM", ALANLAR[(int)ENUMAlanlar.ISLEM], "<", (int)ENUMEvrakHareketDurum.DOSYAYAKALDIR));


            if (form.hareket.islem == (int)ENUMEvrakTarihceDurum.DEVAMEDENISLERVEMIADLIISLER)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("EVREVRAK", "CEVAPSURESI", ">", 0));

            //string kisi = form.hareket.islem != 51 ? form.hareket.kisi1 : form.hareket.kisi2;

            return veriErisim.TekKolonSQLDbl(sql);
        }

        /// <summary>
        /// Parametre olarak verilen evrak hareket bilgisine göre; evrak hareket bilgilerini, sql cümlesi hazırlayıp çalıştırarak veritabanından okuyan yordam.
        /// </summary>
        /// <param name="form">Evrak hareket bilgisini içeren nesne</param>
        /// <returns>İşlem sonucu DataReader olarak döndürülür.</returns>
        public IDataReader Listele(EvrakHareket form)
        {
            string kosul = "";
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR) + " FROM " + TABLOAD;

            if (form.prEvrakNo != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.PREVRAKNO], "=", form.prEvrakNo));
            if (form.siraNo > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SIRANO], "=", form.siraNo));
            if (!string.IsNullOrEmpty(form.birimKod1))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.BIRIMKOD1], "=", form.birimKod1));

            if (form.havaleNotu == "X")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.ISLEM], "<>", (int)ENUMEvrakHareketDurum.TESLIMALDI));
            else
                if (form.islem > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.ISLEM], "=", form.islem));

            if (!form.islemTarih.isNull)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.ISLEMTARIH], "=", form.islemTarih));
            if (form.havaleNotu != "")
            {
                string kosulHavale = "(" + ALANLAR[(int)ENUMAlanlar.HAVALENOTU] + " <> '' AND ";
                kosulHavale += ALANLAR[(int)ENUMAlanlar.HAVALENOTU] + " IS NOT NULL)";
                if (kosul != "")
                    kosul += " AND " + kosulHavale;
            }
            if (kosul != "")
                sql += " WHERE " + kosul;

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Parametre olarak verilen evrak hareket bilgisine göre; maksimum sıra numarasını, sql cümlesi hazırlayıp çalıştırarak veritabanından okuyan yordam.
        /// </summary>
        /// <param name="form">Evrak hareket bilgisini içeren nesne</param>
        /// <returns>Evrak sıra numarası int olarak döndürülür.</returns>
        public int MaksSiraNo(EvrakHareket form)
        {
            string sql = "SELECT MAX(" + ALANLAR[(int)ENUMAlanlar.SIRANO] + ") ";
            sql += " FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.PREVRAKNO], "=", form.prEvrakNo));

            return this.veriErisim.TekKolonSQLInt(sql) + 1;
        }

        /// <summary>
        /// Parametre olarak verilen evrak bilgisine göre; evrak'ın son hareket bilgisini, sql cümlesi hazırlayıp çalıştırarak veritabanından okuyan yordam.
        /// </summary>
        /// <param name="prEvrakNo">Evrak prEvrakNo bilgisi içeren nesne</param>
        /// <returns>İşlem sonucu DataReader olarak döndürülür.</returns>
        public IDataReader SonHaraketGetir(string prEvrakNo)
        {
            string sql = "SELECT PREVRAKNO, SIRANO, BIRIMKOD1, BIRIMKOD2, KISI1, KISI2, ISLEMTARIH, ISLEMYAPAN, ISLEM FROM EVREVRAKHAREKETSONDURUM";
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.PREVRAKNO], "=", prEvrakNo));

            return veriErisim.DataReaderDondur(sql);
        }

        public Sonuc ErisimYetkisiDegistir(EvrakHareket form)
        {
            Sonuc sonuc = new Sonuc(false, "", "");
            string sql = "UPDATE " + TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ERISEMEZ], form.erisemez);
            sql += " WHERE " + VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.PREVRAKNO], "=", form.prEvrakNo);
            sql += " AND ((" + VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KISI2], "=", form.kisi2) + ") AND (ISLEM = 51) OR (" + VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.ISLEMYAPAN], "=", form.islemYapan) + ") AND (ISLEM <> 51))";

            return KomutCalistir(sql, 0);
        }

    }
}
