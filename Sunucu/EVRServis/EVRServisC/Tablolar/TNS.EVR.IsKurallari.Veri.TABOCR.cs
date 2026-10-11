using System;
using System.Data;
using TNS.Veri;

namespace TNS.EVR.IsKurallari.Veri
{
    /// <summary>
    /// Veri tabanı tablosu ile ilgili, kaydetme, silme ve listeleme yordamlarını tutan sınıf.    
    /// </summary>
    public class TABOCR : TABOrtak
    {
        /// <summary>        
        /// Parametre olarak verilen veriErisim nesnesine göre, TABOCR sınıfının yeni bir örneğini oluşturur.
        /// </summary>
        /// <param name="veriErisim">Veri erişim bilgisini içeren nesne</param>
        public TABOCR(VeriErisim veriErisim) : base(veriErisim) { }

        /// <summary>
        /// Tablo bilgilerini içeren veri tabanı tablosunun adını tutan değişken.
        /// </summary>
        public static string TABLOAD = "EVREVRAKDOSYA";

        /// <summary>
        /// Tablo bilgilerini içeren veri tabanı tablosunun alanlarının sırasını tutan numaralandırma.
        /// </summary>
        public enum ENUMAlanlar
        {
            EVREVRAKNO = 0,
            SIRANO = 1,
            DOSYAYOLU = 2,
            DOSYAADI = 3,
            DOSYATARIHI = 4,
            DOSYABOYUTU = 5,
            ARSIVKOD = 6,
            USTYAZIDOSYASI = 7,
            EIMZADOSYASI = 8,
            EIMZALANANSIRANO = 9,
            OCRYAPILDI = 10,
            OCRTARIHI = 11,
            OCRGUID = 12,
            ISLEMYAPAN = 13,
            ISLEMYAPANAD = 14,
            ISLEMTARIH = 15,
            IPTAL = 16,
            ACIKLAMA = 17,
            KRIPTOYAPILDI = 18,
            EIMZAGUID = 19,
            KRIPTOTARIHI = 20,
            ICERIKYAPILDI = 21,
            ICERIKGUID = 22,
            ICERIKTARIHI = 23
        }

        /// <summary>
        /// Tablo bilgilerini içeren veri tabanı tablosunun alanlarının isimlerini tutan dizi.
        /// </summary>
        static string[] ALANLAR = { "EVREVRAKNO", "SIRANO", "DOSYAYOLU", "DOSYAADI", "DOSYATARIHI", "DOSYABOYUTU", "ARSIVKOD", "USTYAZIDOSYASI", "EIMZADOSYASI", "EIMZALANANSIRANO", "OCRYAPILDI", "OCRTARIHI", "OCRGUID", "ISLEMYAPAN", "ISLEMYAPANAD", "ISLEMTARIH", "IPTAL", "ACIKLAMA", "KRIPTOYAPILDI", "EIMZAGUID", "KRIPTOTARIHI", "ICERIKYAPILDI", "ICERIKGUID", "ICERIKTARIHI" };


        public static string TABLOADKONTROL = "EVREVRAKDOSYAKONTOL";
        public enum ENUMAlanlarKontrol
        {
            TARIH = 0,
        }
        static string[] ALANLARKONTROL = { "TARIH" };


        public static string TABLOADICERIK = "EVREVRAKDOSYAICERIK";
        public enum ENUMAlanlarIcerik
        {
            KOD = 0,
            EVREVRAKNO = 1,
            DOSYASIRANO = 2,
            ICERIK = 3,
            ISLEMTARIHI = 4,
            DOSYAYOLU = 5,
            DOSYAADI = 6
        }
        static string[] ALANLARICERIK = { "KOD", "EVREVRAKNO", "DOSYASIRANO", "ICERIK", "ISLEMTARIHI", "DOSYAYOLU", "DOSYAADI" };

        /// <summary>
        /// Parametre olarak verilen EvrakDosya nesne koleksiyonuna göre, kullanıcının evrak EvrakDosya, sql cümlesi hazırlayıp çalıştırarak veritabanına kaydeden yordam.
        /// </summary>
        /// <param name="bilgi">EvrakDosya nesne koleksiyonunu içeren nesne</param>
        /// <returns>
        /// İşlem sonucu Sonuc nesnesi olarak döndürülür.
        /// </returns>
        public Sonuc Kaydet(EvrakDosya bilgi)
        {
            string sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
            sql = sql + VeriErisim.DegerYap(bilgi.evrEvrakNo) + ", ";
            sql = sql + VeriErisim.DegerYap(bilgi.siraNo) + ", ";
            sql = sql + VeriErisim.DegerYap(bilgi.dosyaYolu) + ", ";
            sql = sql + VeriErisim.DegerYap(bilgi.dosyaAdi) + ", ";
            sql = sql + VeriErisim.DegerYap(bilgi.dosyaTarihi) + ", ";
            sql = sql + VeriErisim.DegerYap(bilgi.dosyaBoyutu) + ", ";
            sql = sql + VeriErisim.DegerYap(bilgi.arsivKod) + ", ";
            sql = sql + VeriErisim.DegerYap(bilgi.ustYaziDosyasi) + ", ";
            sql = sql + VeriErisim.DegerYap(bilgi.eImzaDosyasi) + ", ";
            sql = sql + VeriErisim.DegerYap(bilgi.eImzalananSiraNo) + ",";
            sql = sql + VeriErisim.DegerYap(bilgi.ocrYapildi) + ", ";
            sql = sql + VeriErisim.DegerYap(bilgi.ocrTarihi) + ", ";
            sql = sql + VeriErisim.DegerYap(bilgi.ocrGuid) + ", ";
            sql = sql + VeriErisim.DegerYap(bilgi.islemYapan) + ",";
            sql = sql + VeriErisim.DegerYap(bilgi.islemYapanAd) + ",";
            sql = sql + VeriErisim.DegerYap(bilgi.islemTarih) + ",";
            sql = sql + VeriErisim.DegerYap(bilgi.iptal) + ",";
            sql = sql + VeriErisim.DegerYap(bilgi.aciklama) + ",";
            sql = sql + VeriErisim.DegerYap(bilgi.kriptoYapildi) + ",";
            sql = sql + VeriErisim.DegerYap(bilgi.eImzaGuid) + ",";
            sql = sql + VeriErisim.DegerYap(bilgi.kriptoTarihi) + ",";
            sql = sql + VeriErisim.DegerYap(bilgi.icerikYapildi) + ",";
            sql = sql + VeriErisim.DegerYap(bilgi.icerikGuid) + ",";
            sql = sql + VeriErisim.DegerYap(bilgi.icerikTarihi) + ")";

            return KomutCalistir(sql, 1);
        }

        public Sonuc Guncelle(EvrakDosya bilgi)
        {
            string sql = "UPDATE " + TABLOAD + " SET ";
            if (bilgi.ustYaziDosyasi > 0)
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.USTYAZIDOSYASI], bilgi.ustYaziDosyasi) + ",";
            if (bilgi.eImzaDosyasi > 0)
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.EIMZADOSYASI], bilgi.eImzaDosyasi) + ",";
            if (bilgi.eImzalananSiraNo > 0)
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.EIMZALANANSIRANO], bilgi.eImzalananSiraNo) + ",";
            if (bilgi.ocrYapildi > 0)
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.OCRYAPILDI], bilgi.ocrYapildi) + ",";
            if (!bilgi.ocrTarihi.isNull)
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.OCRTARIHI], bilgi.ocrTarihi) + ",";
            if (!string.IsNullOrEmpty(bilgi.ocrGuid))
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.OCRGUID], bilgi.ocrGuid) + ",";
            if (bilgi.kriptoYapildi > 0)
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.KRIPTOYAPILDI], bilgi.kriptoYapildi) + ",";
            if (!bilgi.kriptoTarihi.isNull)
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.KRIPTOTARIHI], bilgi.kriptoTarihi) + ",";
            if (!bilgi.dosyaTarihi.isNull)
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.DOSYATARIHI], bilgi.dosyaTarihi) + ",";
            if (bilgi.dosyaBoyutu > 0)
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.DOSYABOYUTU], bilgi.dosyaBoyutu) + ",";
            if (!string.IsNullOrEmpty(bilgi.islemYapan))
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ISLEMYAPAN], bilgi.islemYapan) + ",";
            if (!string.IsNullOrEmpty(bilgi.islemYapanAd))
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ISLEMYAPANAD], bilgi.islemYapanAd) + ",";
            if (!bilgi.islemTarih.isNull)
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ISLEMTARIH], bilgi.islemTarih) + ",";
            if (bilgi.iptal > 0)
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.IPTAL], bilgi.iptal) + ",";
            if (bilgi.aciklama != "")
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ACIKLAMA], bilgi.aciklama) + ",";
            if (bilgi.eImzaGuid != "")
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.EIMZAGUID], bilgi.eImzaGuid) + ",";
            if (bilgi.icerikYapildi > 0)
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ICERIKYAPILDI], bilgi.icerikYapildi) + ",";
            if (bilgi.icerikGuid != "")
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ICERIKGUID], bilgi.icerikGuid) + ",";
            if (!bilgi.icerikTarihi.isNull)
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ICERIKTARIHI], bilgi.icerikTarihi) + ",";

            sql = sql.TrimEnd(',');
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.EVREVRAKNO], "=", bilgi.evrEvrakNo));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SIRANO], "=", bilgi.siraNo));

            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Parametre olarak verilen EvrakDosya nesne koleksiyonuna göre, kullanıcının evrak dosyalarını, sql cümlesi hazırlayıp çalıştırarak veritabanından silen yordam.
        /// </summary>
        /// <param name="liste">EvrakDosya nesne koleksiyonunu içeren nesne</param>
        /// <returns>İşlem sonucu Sonuc nesnesi olarak döndürülür.</returns>
        public Sonuc Sil(EvrakDosya bilgi)
        {
            string sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.EVREVRAKNO], "=", bilgi.evrEvrakNo));
            if (bilgi.siraNo > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.SIRANO], "=", bilgi.siraNo));
            else if (!string.IsNullOrEmpty(bilgi.dosyaYolu))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.DOSYAYOLU], "LIKE", bilgi.dosyaYolu));
            else//Eğer sirano veya dosyayolu verilmemiş ise silme işlemi yapmasın
                return new Sonuc(false, "Koşul hatalı", "");

            return KomutCalistir(sql, 0);

        }

        /// <summary>
        /// Paramete olarak verilen EvrakDosya nesnesine göre, kullanıcının evrak dosyalarını, sql cümlesi hazırlayıp çalıştırarak veritabanından okuyan yordam.
        /// </summary>
        /// <param name="bilgi">EvrakDosya bilgilerini içeren nesne</param>
        /// <returns>
        /// İşlem sonucu DataReader olarak döndürülür.
        /// </returns>
        public IDataReader Listele(EvrakDosya bilgi, int top)
        {
            string sql = "";
            string kriter = "";

            string topStr = "";

            if (top > 0)
                topStr = "TOP " + top + " ";

            sql = "SELECT " + topStr + VeriErisim.TumAlanlar(TABLOAD, ALANLAR) + " FROM " + TABLOAD;

            //İptal edilen dosyalar listelenmesin Melih 24.02.2014
            kriter = VeriErisim.KosulEkle(kriter, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.IPTAL], "<>", 1));

            if (bilgi.evrEvrakNo != "")
            {
                if (bilgi.evrEvrakNo.IndexOf(";") <= -1)//noktalı virgül yok
                    kriter = VeriErisim.KosulEkle(kriter, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.EVREVRAKNO], "=", bilgi.evrEvrakNo));
                else
                {
                    string[] eler = bilgi.evrEvrakNo.Split(';');
                    string icKriter = "";
                    foreach (string ev in eler)
                    {
                        icKriter = VeriErisim.KosulEkle(icKriter, "OR", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.EVREVRAKNO], "=", ev));
                    }
                    if (icKriter != "")
                    {
                        icKriter = "(" + icKriter + ")";
                        kriter = VeriErisim.KosulEkle(kriter, "AND", icKriter);
                    }
                }
            }
            if (bilgi.dosyaYolu != "")
                kriter = VeriErisim.KosulEkle(kriter, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.DOSYAYOLU], "=", bilgi.dosyaYolu));
            if (bilgi.dosyaAdi != "")
                kriter = VeriErisim.KosulEkle(kriter, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.DOSYAADI], "=", bilgi.dosyaAdi));
            if (bilgi.arsivKod > -1)
                kriter = VeriErisim.KosulEkle(kriter, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ARSIVKOD], "=", bilgi.arsivKod));
            if (bilgi.ocrYapildi > -1)
            {
                if (bilgi.ocrYapildi == (int)ENUMEvrakDosyaOCRIslem.ISLEMIBITENLER)
                {
                    string icKriter = VeriErisim.KosulEkle("", "", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.OCRYAPILDI], "=", 1));
                    icKriter = VeriErisim.KosulEkle(icKriter, "OR", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.OCRYAPILDI], "=", 3));
                    icKriter = "(" + icKriter + ")";

                    kriter = VeriErisim.KosulEkle(kriter, "AND ", icKriter);
                }
                else if (bilgi.ocrYapildi <= (int)ENUMEvrakDosyaOCRIslem.YAPILACAK)
                {
                    string icKriter = VeriErisim.KosulEkle("", "", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.OCRYAPILDI], "=", 0));
                    icKriter = VeriErisim.KosulEkle(icKriter, "OR", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.OCRYAPILDI], "=", -1));
                    icKriter = "(" + icKriter + ")";

                    kriter = VeriErisim.KosulEkle(kriter, "AND ", icKriter);
                }
                else
                    kriter = VeriErisim.KosulEkle(kriter, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.OCRYAPILDI], "=", bilgi.ocrYapildi));
            }
            if (!string.IsNullOrEmpty(bilgi.ocrGuid))
                kriter = VeriErisim.KosulEkle(kriter, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.OCRGUID], "=", bilgi.ocrGuid));

            if (!bilgi.sorguDosyaTarihi1.isNull)
                kriter = VeriErisim.KosulEkle(kriter, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.DOSYATARIHI], ">=", bilgi.sorguDosyaTarihi1));
            if (!bilgi.sorguDosyaTarihi2.isNull)
                kriter = VeriErisim.KosulEkle(kriter, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.DOSYATARIHI], "<", bilgi.sorguDosyaTarihi2));
            if (bilgi.ustYaziDosyasi > -1)
                kriter = VeriErisim.KosulEkle(kriter, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.USTYAZIDOSYASI], "=", bilgi.ustYaziDosyasi));
            if (bilgi.eImzaDosyasi > -1)
                kriter = VeriErisim.KosulEkle(kriter, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.EIMZADOSYASI], "=", bilgi.eImzaDosyasi));
            if (bilgi.siraNo > 0)
                kriter = VeriErisim.KosulEkle(kriter, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.SIRANO], "=", bilgi.siraNo));
            if (bilgi.kriptoYapildi > -1)
            {
                if (bilgi.kriptoYapildi <= 0)
                {
                    string icKriter = VeriErisim.KosulEkle("", "", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KRIPTOYAPILDI], "=", 0));
                    icKriter = VeriErisim.KosulEkle(icKriter, "OR", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KRIPTOYAPILDI], "=", -1));
                    icKriter = VeriErisim.KosulEkle(icKriter, "OR", "EVREVRAKDOSYA.ISLEMTARIH > EVREVRAKDOSYA.KRIPTOTARIH");
                    icKriter = "(" + icKriter + ")";

                    kriter = VeriErisim.KosulEkle(kriter, "AND ", icKriter);
                }
                else
                    kriter = VeriErisim.KosulEkle(kriter, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KRIPTOYAPILDI], "=", bilgi.kriptoYapildi));
            }
            if (bilgi.icerikYapildi > -1)
            {
                if (bilgi.icerikYapildi <= 0)
                {
                    string icKriter = VeriErisim.KosulEkle("", "", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ICERIKYAPILDI], "=", 0));
                    icKriter = VeriErisim.KosulEkle(icKriter, "OR", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ICERIKYAPILDI], "=", -1));
                    icKriter = VeriErisim.KosulEkle(icKriter, "OR", "EVREVRAKDOSYA.ISLEMTARIH > EVREVRAKDOSYA.ICERIKTARIHI");
                    icKriter = VeriErisim.KosulEkle(icKriter, "OR", "EVREVRAKDOSYA.OCRTARIHI > EVREVRAKDOSYA.ICERIKTARIHI");
                    icKriter = "(" + icKriter + ")";

                    kriter = VeriErisim.KosulEkle(kriter, "AND ", icKriter);
                }
                else
                    kriter = VeriErisim.KosulEkle(kriter, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ICERIKYAPILDI], "=", bilgi.icerikYapildi));
            }
            if (!string.IsNullOrEmpty(bilgi.eImzaGuid))
                kriter = VeriErisim.KosulEkle(kriter, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.EIMZAGUID], "=", bilgi.eImzaGuid));

            sql = VeriErisim.KosulEkle(sql, "WHERE", kriter);
            sql += " ORDER BY 1";

            //if (sql.IndexOf("TOP 5000") > -1)
            //    OrtakFonksiyonlar.HataStrYaz(sql);

            return veriErisim.DataReaderDondur(sql);
        }


        public Sonuc KaydetKontrol(TNSDateTime tarih)
        {
            string sql = "DELETE FROM " + TABLOADKONTROL;
            KomutCalistir(sql, 1);

            sql = "INSERT INTO " + TABLOADKONTROL + " (" + VeriErisim.TumAlanlar("", ALANLARKONTROL) + ") VALUES (";
            sql = sql + VeriErisim.DegerYap(tarih) + ")";
            return KomutCalistir(sql, 1);
        }

        public TNSDateTime ListeleKontrol()
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOADKONTROL, ALANLARKONTROL) + " FROM " + TABLOADKONTROL;
            return veriErisim.TekKolonSQLDate(sql);
        }

        public int KodAl(string evrEvrakNo)
        {
            string sql = "SELECT MAX(" + ALANLAR[(int)ENUMAlanlar.SIRANO] + ")  FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.EVREVRAKNO], "=", evrEvrakNo));
            return veriErisim.TekKolonSQLInt(sql) + 1;
        }

        public IDataReader ListeleIcerik(string kelime, int yil)
        {
            string[] kelimeler = kelime.Split(' ', ';', ',');
            string kelimelerSQL = "";

            foreach (string k in kelimeler)
            {
                if (string.IsNullOrEmpty(k.Trim()))
                    continue;

                if (kelimelerSQL != "") kelimelerSQL += " AND ";
                kelimelerSQL += "\"" + k + "*\"";
            }

            string sql = "SELECT KOD,EVREVRAKNO,DOSYASIRANO,DOSYAYOLU,DOSYAADI FROM EVREVRAKDOSYAICERIK";
            sql += " WHERE ";
            sql += " CONTAINS(EVREVRAKDOSYAICERIK.ICERIK, '" + kelimelerSQL + "')";

            if (yil > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADICERIK, ALANLARICERIK[(int)ENUMAlanlarIcerik.EVREVRAKNO], "LIKE", yil.ToString() + "%"));

            return veriErisim.DataReaderDondur(sql);
        }

        public Sonuc KaydetIcerik(EvrakDosya bilgi)
        {
            string sql = "INSERT INTO " + TABLOADICERIK + " (" + VeriErisim.TumAlanlar("", ALANLARICERIK) + ") VALUES (";
            sql = sql + VeriErisim.DegerYap(bilgi.icerikGuid) + ", ";
            sql = sql + VeriErisim.DegerYap(bilgi.evrEvrakNo) + ", ";
            sql = sql + VeriErisim.DegerYap(bilgi.siraNo) + ", ";
            sql = sql + VeriErisim.DegerYap(bilgi.icerik) + ", ";
            sql = sql + VeriErisim.DegerYap(new TNSDateTime(DateTime.Now)) + ",";
            sql = sql + VeriErisim.DegerYap(bilgi.dosyaYolu) + ", ";
            sql = sql + VeriErisim.DegerYap(bilgi.dosyaAdi) + ") ";

            return KomutCalistir(sql, 1);
        }

        public Sonuc SilIcerik(EvrakDosya bilgi)
        {
            string sql = "DELETE FROM " + TABLOADICERIK;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARICERIK[(int)ENUMAlanlarIcerik.KOD], "=", bilgi.icerikGuid));

            return KomutCalistir(sql, 1);
        }

    }
}
