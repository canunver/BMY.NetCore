using System;
using System.Data;
using TNS.Veri;

namespace TNS.MUH.IsKurallari.Veri
{
    /// <summary>
    /// ABSEntegrasyonMIF veri tabanı tablosu ile ilgili, listeleme ve yazma işlemleri tutan nesne
    /// </summary>
    public class TABEntegrasyonMIF : TABOrtak
    {
        /// <summary>
        /// Sınıfın oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABEntegrasyonMIF(VeriErisim veriErisim) : base(veriErisim) { }


        static string TABLOAD = "MUHENTEGRASYONMIFFORM";
        public enum ENUMAlanlar
        {
            YIL = 0,
            BELGENO = 1,
            KURUMKOD = 2,
            MUHASEBEKOD = 3,
            BIRIMKOD = 4,
            ACIKLAMA = 5,
            ISLEMTARIH = 6,
            FORMDURUM = 7,
            ISLEMYAPAN = 8,
            FISTUR = 9,
            KURKOD = 10,
            BASLAMATARIHI = 11,
            BITISTARIHI = 12,
            YEVMIYENO1 = 13,
            YEVMIYENO2 = 14

        }
        static string[] ALANLAR = { "YIL", "BELGENO", "KURUMKOD", "MUHASEBEKOD", "BIRIMKOD", "ACIKLAMA", "ISLEMTARIH", "FORMDURUM", "ISLEMYAPAN", "FISTUR", "KURKOD", "BASLAMATARIHI", "BITISTARIHI", "YEVMIYENO1", "YEVMIYENO2" };


        static string TABLOADDETAY = "MUHENTEGRASYONMIF";
        public enum ENUMAlanlarDetay
        {
            YIL = 0,
            BELGENO = 1,
            SIRA = 2,
            HESAPKOD = 3,
            KURKOD = 4,
            FONKOD = 5,
            FINKOD = 6,
            EKOKOD = 7,
            BORC = 8,
            ALACAK = 9,
            ALTHESAPKOD = 10,
            ACIKLAMA = 11,
            DETAYBIRIM = 12,
            TERTIP = 13,
        }
        static string[] ALANLARDETAY = { "YIL", "BELGENO", "SIRA", "HESAPKOD", "KURKOD", "FONKOD", "FINKOD",
            "EKOKOD", "BORC", "ALACAK", "ALTHESAPKOD", "ACIKLAMA", "DETAYBIRIM", "TERTIP"};


        static string TABLOADSONUC = "MUHENTEGRASYONSONUC";
        public enum ENUMAlanlarSonuc : int
        {
            YIL = 0,
            BELGENO = 1,
            ETTN = 2,
            TARIH = 3,
            ISLEMYAPAN = 4,
            DURUM = 5,
        }
        static string[] ALANLARGIDEREBUTCE = { "YIL", "BELGENO", "ETTN", "TARIH", "ISLEMYAPAN", "DURUM" };

        /// <summary>
        /// Parametre olarak gelen bilgileri veri tabanına ekleyen sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="bilgi">Eklenecek kayıt bilgisini tutan nesne</param>
        /// <returns>Kaydet komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc Kaydet(OdemeEmriMIFForm bilgi)
        {
            Sonuc sonuc = new Sonuc(false, "", "");
            string sql = "";

            sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
            sql = sql + VeriErisim.DegerYap(bilgi.yil) + ", ";
            sql = sql + VeriErisim.DegerYap(bilgi.belgeNo) + ", ";
            sql = sql + VeriErisim.DegerYap(bilgi.kurum.Replace(".", "")) + ", ";
            sql = sql + VeriErisim.DegerYap(bilgi.muhasebe) + ", ";
            sql = sql + VeriErisim.DegerYap(bilgi.birim) + ", ";
            sql = sql + VeriErisim.DegerYap(bilgi.aciklama) + ", ";
            sql = sql + VeriErisim.DegerYap(bilgi.islemTarih) + ", ";
            sql = sql + VeriErisim.DegerYap(bilgi.formDurum) + ", ";
            sql = sql + VeriErisim.DegerYap(bilgi.islemYapan) + ",";
            sql = sql + VeriErisim.DegerYap((int)bilgi.fisTur[0]) + ",";
            sql = sql + VeriErisim.DegerYap(bilgi.kurKod.Replace(".", "")) + ",";
            sql = sql += VeriErisim.DegerYap(bilgi.baslamaTarihi) + ",";
            sql = sql += VeriErisim.DegerYap(bilgi.bitisTarihi) + ",";
            sql = sql += VeriErisim.DegerYap(bilgi.yevmiyeNo1) + ",";
            sql = sql += VeriErisim.DegerYap(bilgi.yevmiyeNo2) + ")";
            sonuc = KomutCalistir(sql, 1);

            if (sonuc.islemSonuc && !bilgi.sadeceUstVeri)
            {
                int siraNo = 0;
                foreach (OdemeEmriMIFDetay detay in bilgi.detay.objeler)
                {
                    siraNo++;

                    detay.satirNo = siraNo;

                    sql = "INSERT INTO " + TABLOADDETAY + " (" + VeriErisim.TumAlanlar("", ALANLARDETAY) + ") VALUES (";
                    sql = sql + VeriErisim.DegerYap(bilgi.yil) + ", ";
                    sql = sql + VeriErisim.DegerYap(bilgi.belgeNo) + ", ";
                    sql = sql + VeriErisim.DegerYap(siraNo) + ", ";
                    sql = sql + VeriErisim.DegerYap(detay.hesapKod.Replace(".", "").Trim()) + ", ";
                    sql = sql + VeriErisim.DegerYap(detay.kur.Replace(".", "").Trim()) + ", ";
                    sql = sql + VeriErisim.DegerYap(detay.fon.Replace(".", "").Trim()) + ", ";
                    sql = sql + VeriErisim.DegerYap(detay.fin.Replace(".", "").Trim()) + ", ";
                    sql = sql + VeriErisim.DegerYap(detay.eko.Replace(".", "").Trim()) + ", ";
                    sql = sql + VeriErisim.DegerYap(Math.Round(detay.borc, 2)) + ", ";
                    sql = sql + VeriErisim.DegerYap(Math.Round(detay.alacak, 2)) + ", ";
                    sql = sql + VeriErisim.DegerYap(detay.altHesapKod.Trim()) + ",";
                    sql = sql + VeriErisim.DegerYap(detay.satirAciklama) + ",";
                    sql = sql + VeriErisim.DegerYap(detay.detayBirim) + ",";
                    sql = sql + VeriErisim.DegerYap(detay.sorguTertip) + ")";

                    sonuc = KomutCalistir(sql, 1);
                }
            }

            string islem = FormDurumAciklama(bilgi.formDurum, bilgi.belgeTur);
            KaydetTarihce(bilgi.yil, bilgi.belgeNo, bilgi.islemYapan, islem, bilgi.islemYapanAd);

            return sonuc;
        }

        /// <summary>
        /// Parametre olarak gelen bilgileri veri tabanına ekleyen sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="bilgi">Eklenecek kayıt bilgisini tutan nesne</param>
        /// <returns>Kaydet komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc Guncelle(OdemeEmriMIFForm bilgi)
        {
            string sql = "UPDATE " + TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], bilgi.muhasebe) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.BIRIMKOD], bilgi.birim) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ACIKLAMA], bilgi.aciklama) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.FISTUR], (int)bilgi.fisTur[0]) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.KURKOD], bilgi.kurKod) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.BASLAMATARIHI], bilgi.baslamaTarihi) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.BITISTARIHI], bilgi.bitisTarihi);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", bilgi.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.BELGENO], "=", bilgi.belgeNo));

            string islem = FormDurumAciklama(bilgi.formDurum, bilgi.belgeTur);
            KaydetTarihce(bilgi.yil, bilgi.belgeNo, bilgi.islemYapan, islem, bilgi.islemYapanAd);

            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Parametre olarak gelen bilgileri veri tabanına ekleyen sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="bilgi">Eklenecek kayıt bilgisini tutan nesne</param>
        /// <returns>Kaydet komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc GuncelleDurum(OdemeEmriMIFForm bilgi)
        {
            string sql = "UPDATE " + TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.FORMDURUM], bilgi.formDurum);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", bilgi.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.BELGENO], "=", bilgi.belgeNo));

            string islem = FormDurumAciklama(bilgi.formDurum, bilgi.belgeTur);
            KaydetTarihce(bilgi.yil, bilgi.belgeNo, bilgi.islemYapan, islem, bilgi.islemYapanAd);

            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Parametre olarak gelen anahtar alanları kullanarak, ilgili kaydı veri tabanından silen sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="yil">Yıl</param>
        /// <param name="belgeNo">Belge numarası.</param>
        /// <returns>Sil komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc Sil(int yil, string belgeNo)
        {
            string sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.BELGENO], "=", belgeNo));
            Sonuc sonuc = new Sonuc();

            sonuc = KomutCalistir(sql, 0);

            if (sonuc.islemSonuc)
            {
                sql = "DELETE FROM " + TABLOADDETAY;
                sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARDETAY[(int)ENUMAlanlarDetay.YIL], "=", yil));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARDETAY[(int)ENUMAlanlarDetay.BELGENO], "=", belgeNo));
                sonuc = KomutCalistir(sql, 0);
            }

            return sonuc;
        }

        /// <summary>
        /// Parametre olarak gelen alanları kıstas olarak kullanarak, select sql cümlesini oluşturup, çalıştıran ve sonucunu DataReader olarak döndüren fonksiyon
        /// </summary>
        /// <param name="bilgi">bilgi.</param>
        /// <returns></returns>
        public IDataReader Listele(OdemeEmriMIFForm bilgi)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR);
            sql += " ,(SELECT SUM(TA.BORC) FROM MUHENTEGRASYONMIF TA WHERE TA.BELGENO = MUHENTEGRASYONMIFFORM.BELGENO) AS BORC";
            sql += " ,(SELECT SUM(TA.ALACAK) FROM MUHENTEGRASYONMIF TA WHERE TA.BELGENO = MUHENTEGRASYONMIFFORM.BELGENO) AS ALACAK, MUHENTEGRASYONSONUC.ETTN, MUHMUHASEBE.MUHASEBEAD";
            sql += " FROM " + TABLOAD;
            sql += " LEFT OUTER JOIN MUHENTEGRASYONSONUC ON MUHENTEGRASYONSONUC.YIL = MUHENTEGRASYONMIFFORM.YIL AND MUHENTEGRASYONSONUC.BELGENO = MUHENTEGRASYONMIFFORM.BELGENO";
            sql += " LEFT OUTER JOIN MUHMUHASEBE ON MUHMUHASEBE.MUHASEBEKOD = MUHENTEGRASYONMIFFORM.MUHASEBEKOD";

            string kosul = "";
            if (bilgi.yil > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.YIL], "=", bilgi.yil));

            if (bilgi.belgeNo != "")
            {
                string[] t = bilgi.belgeNo.Split(';');

                if (t.Length == 1 && t[0] != "")
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.BELGENO], "=", t[0]));

                if (t.Length == 2)
                {
                    if (t[1] != "")
                    {
                        kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.BELGENO], ">=", t[0]));
                        kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.BELGENO], "<=", t[1]));
                    }
                    else
                        kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.BELGENO], "=", t[0]));
                }
            }

            if (bilgi.sorguIslemTarih != "")
            {
                string[] t = bilgi.sorguIslemTarih.Split(';');

                if (t.Length == 1 && t[0] != "")
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ISLEMTARIH], "=", t[0]));

                if (t.Length == 2)
                {
                    if (t[1] != "")
                    {
                        kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ISLEMTARIH], ">=", new TNSDateTime(t[0])));
                        kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ISLEMTARIH], "<=", new TNSDateTime(t[1])));
                    }
                    //else
                    //    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ISLEMTARIH], "=", t[0]));
                }
            }

            if (bilgi.muhasebe > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", bilgi.muhasebe));

            if (bilgi.formDurum > 0)
            {
                if (bilgi.formDurum == 22)
                    sql += " AND " + VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.FORMDURUM], "<", (int)ENUMBelgeDurumu.IPTAL);
                else
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.FORMDURUM], "=", bilgi.formDurum));
            }

            if (!string.IsNullOrWhiteSpace(bilgi.birim))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.BIRIMKOD], "=", bilgi.birim));

            if (kosul != "")
                sql += " WHERE " + kosul;

            sql += " ORDER BY " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.YIL];

            IDataReader r = veriErisim.DataReaderDondur(sql);
            return r;
        }



        /// <summary>
        /// Parametre olarak gelen alanları kıstas olarak kullanarak, select sql cümlesini oluşturup, çalıştıran ve sonucunu DataReader olarak döndüren fonksiyon
        /// </summary>
        /// <param name="bilgi">bilgi.</param>
        /// <returns></returns>
        public IDataReader ListeleDetay(OdemeEmriMIFForm bilgi)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOADDETAY, ALANLARDETAY) + ", MUHHESAPPLANI.ACIKLAMA";
            sql += " FROM " + TABLOADDETAY;
            sql += " INNER JOIN MUHHESAPPLANI ON MUHHESAPPLANI.HESAPKOD = MUHENTEGRASYONMIF.HESAPKOD AND MUHHESAPPLANI.YIL = MUHENTEGRASYONMIF.YIL";

            string kosul = "";
            if (bilgi.yil > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADDETAY, ALANLARDETAY[(int)ENUMAlanlarDetay.YIL], "=", bilgi.yil));

            if (!string.IsNullOrWhiteSpace(bilgi.belgeNo))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADDETAY, ALANLARDETAY[(int)ENUMAlanlarDetay.BELGENO], "=", bilgi.belgeNo));

            if (kosul != "")
                sql += " WHERE " + kosul;

            sql += " ORDER BY " + TABLOADDETAY + "." + ALANLARDETAY[(int)ENUMAlanlarDetay.YIL];

            IDataReader r = veriErisim.DataReaderDondur(sql);
            return r;
        }

        void KaydetTarihce(int yil, string belgeNo, string islemYapan, string islem, string islemYapanAd)
        {
            string sql = "";

            sql = "INSERT INTO MUHBELGETARIHCE (YIL,BELGETUR,BELGENO,ISLEMYAPAN,ISLEM,ISLEMTARIH,ISLEMYAPANAD) VALUES (";
            sql = sql + VeriErisim.DegerYap(yil) + ", ";
            sql = sql + VeriErisim.DegerYap(999) + ", ";
            sql = sql + VeriErisim.DegerYap(belgeNo) + ", ";
            sql = sql + VeriErisim.DegerYap(islemYapan) + ", ";
            sql = sql + VeriErisim.DegerYap(islem) + ", ";
            sql = sql + VeriErisim.DegerYap(new TNSDateTime(DateTime.Now)) + ",";
            sql = sql + VeriErisim.DegerYap(islemYapanAd) + ")";

            KomutCalistir(sql, 1);
        }

        string FormDurumAciklama(int durum, ENUMBelgeTuru belgeTur)
        {
            return TNS.MUH.Arac.FormDurumAciklama(durum, belgeTur);
        }

    }
}
