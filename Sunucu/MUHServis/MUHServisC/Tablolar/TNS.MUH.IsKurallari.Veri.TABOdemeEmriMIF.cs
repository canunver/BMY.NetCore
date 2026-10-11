using System;
using System.Data;
using TNS.Veri;

namespace TNS.MUH.IsKurallari.Veri
{
    /// <summary>
    /// MUHMIFFORM, MUHMIF, ABSBUTCEGIDER veri tabanı tablosu ile ilgili, listeleme ve yazma işlemleri tutan nesne
    /// </summary>
    public class TABOdemeEmriMIF : TABOrtak
    {
        static bool yenialan = true;

        /// <summary>
        /// Sınıfın oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABOdemeEmriMIF(VeriErisim veriErisim) : base(veriErisim) { }

        public static string TABLOADMIFF = "MUHMIFFORM";
        public static string TABLOADMIFD = "MUHMIF";
        static string TABLOADTERTIP = "ABSBUTCEGIDER";
        static string TABLOADKAPANIS = "MUHKAPATMA";

        public enum ENUMAlanlarMIFF
        {
            YIL = 0,
            BELGETUR = 1,
            BELGENO = 2,
            KURUMKOD = 3,
            MUHASEBEKOD = 4,
            BIRIMKOD = 5,
            IADSOYAD = 6,
            IKIMLIKNO = 7,
            ISUBEAD = 8,
            IHESAPNO = 9,
            IVERGIDAIRE = 10,
            ACIKLAMA = 11,
            ISLEMTARIH = 12,
            DURUMTARIH = 13,
            FORMDURUM = 14,
            IPTALNEDEN = 15,
            YEVMIYETARIH = 16,
            YEVMIYENO = 17,
            ETARIH = 18,
            ENO = 19,
            EKIM = 20,
            ATARIH = 21,
            ANO = 22,
            ATUTAR = 23,
            OTUR = 24,
            OTARIH = 25,
            ONO = 26,
            OTUTAR = 27,
            VERILEEMRINO = 28,
            CEKEMRINO = 29,
            ISLEMYAPAN = 30,
            PROJENO = 31,
            IHALE = 32,
            FISTUR = 33,
            ILGILIBELGENO = 34,
            SURECYIL = 35,
            SURECNO = 36,
            KURKOD = 37,
            ORGANIZASYONKOD = 38,
            YEVMIYEACIKLAMA = 39,
            DUZELTILENODEMEEMRINO = 40,
            ISTKOD1 = 41,
            ISTKOD2 = 42
        }

        public enum ENUMSorguEkler1
        {
            KURAD = 43,
            MUHASEBEAD = 44,
            BORC = 45,
            ALACAK = 46
        }

        public enum ENUMSorguEkler2
        {
            KURAD = 43,
            MUHASEBEAD = 44,
            YIL = 45,
            BELGETUR = 46,
            BELGENO = 47,
            SIRA = 48,
            HESAPKOD = 49,
            KURKOD = 50,
            FONKOD = 51,
            FINKOD = 52,
            EKOKOD = 53,
            BORC = 54,
            ALACAK = 55,
            KESINTI = 56,
            PEBKOD = 57,
            ALTHESAPKOD = 58,
            DOVIZ = 59,
            DOVIZKURTUR = 60,
            DOVIZBORC = 61,
            DOVIZALACAK = 62,
            DOVIZKUR = 63,
            ACIKLAMA = 64,
            DETAYBIRIM = 65,
            MUHISKODU = 66,
            MUHISADI = 67,
            HESAPPLANI_ACIKLAMA = 68,
            KAPATAN = 69,
            KAPATILAN = 70,
            ALTHESAPAD1 = 71,
            ALTHESAPAD2 = 72,
            PRGKOD = 73,
            APRGKOD = 74,
            FAALKOD = 75,
            AFAALKOD = 76,
        }

        public static string[] ALANLARMIFF = { "YIL", "BELGETUR", "BELGENO", "KURUMKOD", "MUHASEBEKOD", "BIRIMKOD", "IADSOYAD", "IKIMLIKNO", "ISUBEAD", "IHESAPNO", "IVERGIDAIRE", "ACIKLAMA", "ISLEMTARIH", "DURUMTARIH", "FORMDURUM", "IPTALNEDEN", "YEVMIYETARIH", "YEVMIYENO", "ETARIH", "ENO", "EKIM", "ATARIH", "ANO", "ATUTAR", "OTUR", "OTARIH", "ONO", "OTUTAR", "VERILEEMRINO", "CEKEMRINO", "ISLEMYAPAN", "PROJENO", "IHALE", "FISTUR", "ILGILIBELGENO", "SURECYIL", "SURECNO", "KURKOD", "ORGANIZASYONKOD", "YEVMIYEACIKLAMA", "DUZELTILENODEMEEMRINO", "ISTKOD1", "ISTKOD2" };

        public enum ENUMAlanlarMIFD
        {
            YIL = 0,
            BELGETUR = 1,
            BELGENO = 2,
            SIRA = 3,
            HESAPKOD = 4,
            KURKOD = 5,
            FONKOD = 6,
            FINKOD = 7,
            EKOKOD = 8,
            BORC = 9,
            ALACAK = 10,
            KESINTI = 11,
            PEBKOD = 12,
            ALTHESAPKOD = 13,
            DOVIZ = 14,
            DOVIZKURTUR = 15,
            DOVIZBORC = 16,
            DOVIZALACAK = 17,
            DOVIZKUR = 18,
            ACIKLAMA = 19,
            DETAYBIRIM = 20,
            ISKOD = 21,
        }
        static string[] ALANLARMIFD = { "YIL", "BELGETUR", "BELGENO", "SIRA", "HESAPKOD", "KURKOD", "FONKOD", "FINKOD", "EKOKOD", "BORC", "ALACAK", "KESINTI", "PEBKOD", "ALTHESAPKOD", "DOVIZ", "DOVIZKURTUR", "DOVIZBORC", "DOVIZALACAK", "DOVIZKUR", "ACIKLAMA", "DETAYBIRIM", "ISKOD" };

        enum ENUMAlanlarHESAP
        {
            YIL = 0,
            HESAPKOD = 1,
            ACIKLAMA = 2,
            KURUMKOD = 3,
        }
        static string[] ALANLARHESAP = { "YIL", "HESAPKOD", "ACIKLAMA", "KURUMKOD" };

        enum ENUMAlanlarTERTIP
        {
            YIL = 0,
            KURKOD = 1,
            FONKOD = 2,
            FINKOD = 3,
            EKOKOD = 4
        }
        static string[] ALANLARTERTIP = { "YIL", "KURKOD", "FONKOD", "FINKOD", "EKOKOD" };

        public enum ENUMAlanlarKAPANIS
        {
            YIL = 0,
            BELGENO = 1,
            KYIL = 2,
            KBELGENO = 3,
            KHESAPKOD = 4,
            KILGILIVNO = 5,
            KTERTIP = 6,
            KTUTAR = 7,
            SIRANO = 8,
            BORCALACAK = 9,
            KBIRIMKOD = 10,

        }
        static string[] ALANLARKAPANIS = { "YIL", "BELGENO", "KYIL", "KBELGENO", "KHESAPKOD", "KILGILIVNO", "KTERTIP", "KTUTAR", "SIRANO", "BORCALACAK", "KBIRIMKOD" };


        static string TABLOADBELGELER = "MUHBELGELER";
        public enum ENUMAlanlarBelgeler : int
        {
            YIL = 0,
            BELGENO = 1,
            SIRANO = 2,
            BELGEADI = 3,
            BELGE = 4
        }
        static string[] ALANLARBELGELER = { "YIL", "BELGENO", "SIRANO", "BELGEADI", "BELGE" };


        static string TABLOADOYBS = "MUHMIFFORMOYBS";
        public enum ENUMAlanlarOybs : int
        {
            YIL = 0,
            BELGENO = 1,
            ISLEMTIP = 2,
            OYBSID = 3,
        }
        static string[] ALANLAROYBS = { "YIL", "BELGENO", "ISLEMTIP", "OYBSID" };



        static string TABLOADENTEGRASYON = "MUHENTEGRASYONSONUC";
        static string[] ALANLARENTEGRASYON = { "YIL", "BELGENO", "ETTN", "TARIH", "ISLEMYAPAN", "DURUM" };
        public enum ENUMAlanlarEntegrasyon : int
        {
            YIL = 0,
            BELGENO = 1,
            ETTN = 2,
            TARIH = 3,
            ISLEMYAPAN = 4,
            DURUM = 5
        }

        /// <summary>
        /// Parametre olarak gelen bilgileri veri tabanına ekleyen sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="form">Eklenecek kayıt bilgisini tutan nesne</param>
        /// <returns>Kaydet komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc Kaydet(OdemeEmriMIFForm form)
        {
            if (!form.islemTarih.isNull && (form.islemTarih.Saat > 0 || form.islemTarih.Dakika > 0))
                form.islemTarih = new TNSDateTime(form.islemTarih.ToString());

            if (form.belgeTur == ENUMBelgeTuru.ODEMEEMRI || form.belgeTur == ENUMBelgeTuru.MIF)
                return KaydetODEMIF(form);
            else
                return new Sonuc(false, "Kayıt edilmek istenilen belgenin türü belli değil.", "");
        }

        /// <summary>
        /// Listeles the belgeler.
        /// </summary>
        /// <param name="dokuman">The dokuman.</param>
        /// <returns></returns>
        public IDataReader ListeleBelgeler(Dokuman dokuman)
        {
            string sql = "";
            string kosul = "";

            if (dokuman.siraNo > 0)
                sql = "SELECT " + VeriErisim.TumAlanlar(TABLOADBELGELER, ALANLARBELGELER) + " FROM " + TABLOADBELGELER;
            else
                sql = "SELECT YIL, BELGENO, SIRANO, BELGEADI FROM " + TABLOADBELGELER;

            if (dokuman.yil > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADBELGELER, ALANLARBELGELER[(int)ENUMAlanlarBelgeler.YIL], "=", dokuman.yil));

            if (!string.IsNullOrEmpty(dokuman.belgeNo))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADBELGELER, ALANLARBELGELER[(int)ENUMAlanlarBelgeler.BELGENO], "=", dokuman.belgeNo));

            if (dokuman.siraNo > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADBELGELER, ALANLARBELGELER[(int)ENUMAlanlarBelgeler.SIRANO], "=", dokuman.siraNo));

            if (!string.IsNullOrEmpty(dokuman.dokumanAdi))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADBELGELER, ALANLARBELGELER[(int)ENUMAlanlarBelgeler.BELGEADI], "=", dokuman.dokumanAdi));

            if (kosul != "")
                sql += " WHERE " + kosul;

            sql += " ORDER BY " + ALANLARBELGELER[(int)ENUMAlanlarBelgeler.SIRANO];

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Kaydets the belgeler.
        /// </summary>
        /// <param name="dokuman">The dokuman.</param>
        /// <returns></returns>
        public Sonuc KaydetBelgeler(Dokuman dokuman)
        {
            string imgStr = "";
            if (dokuman.dokumanIcerik != null && dokuman.dokumanIcerik.Length > 0)
                imgStr = "0x" + System.BitConverter.ToString(dokuman.dokumanIcerik).Replace("-", string.Empty);
            else
                imgStr = "null";

            string sql = "";
            sql = "INSERT INTO " + TABLOADBELGELER + " (" + VeriErisim.TumAlanlar("", ALANLARBELGELER) + ") VALUES (";
            sql += VeriErisim.DegerYap(dokuman.yil) + ",";
            sql += VeriErisim.DegerYap(dokuman.belgeNo) + ",";
            sql += VeriErisim.DegerYap(dokuman.siraNo) + ",";
            sql += VeriErisim.DegerYap(dokuman.dokumanAdi) + ",";
            sql += imgStr + ")";

            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Sils the belgeler.
        /// </summary>
        /// <param name="dokuman">The dokuman.</param>
        /// <returns></returns>
        public Sonuc SilBelgeler(Dokuman dokuman)
        {
            string sql = "";

            sql = "DELETE FROM " + TABLOADBELGELER;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARBELGELER[(int)ENUMAlanlarBelgeler.YIL], "=", dokuman.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARBELGELER[(int)ENUMAlanlarBelgeler.BELGENO], "=", dokuman.belgeNo));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARBELGELER[(int)ENUMAlanlarBelgeler.SIRANO], "=", dokuman.siraNo));

            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Belges the sira no al.
        /// </summary>
        /// <param name="yil">The yil.</param>
        /// <param name="belgeNo">The belge no.</param>
        /// <returns></returns>
        public int BelgeSiraNoAl(int yil, string belgeNo)
        {
            string sql = "";
            int siraNo = 0;

            sql = "SELECT MAX(" + ALANLARBELGELER[(int)ENUMAlanlarBelgeler.SIRANO] + ") FROM " + TABLOADBELGELER;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARBELGELER[(int)ENUMAlanlarBelgeler.YIL], "=", yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARBELGELER[(int)ENUMAlanlarBelgeler.BELGENO], "=", belgeNo));

            IDataReader r = veriErisim.DataReaderDondur(sql);
            try
            {
                if (r.Read())
                    siraNo = r.GetInt32(0);
            }
            catch { }
            r.Close();

            return (siraNo + 1);
        }

        /// <summary>
        /// Parametre olarak gelen bilgileri veri tabanına ekleyen sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="form">Eklenecek kayıt bilgisini tutan nesne</param>
        /// <returns>Kaydet komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc KaydetODEMIF(OdemeEmriMIFForm form)
        {
            Sonuc sonuc = new Sonuc(false, "", "");
            string sql = "";

            sql = "INSERT INTO " + TABLOADMIFF + " (" + VeriErisim.TumAlanlar("", ALANLARMIFF) + ") VALUES (";
            sql = sql + VeriErisim.DegerYap(form.yil) + ", ";
            sql = sql + VeriErisim.DegerYap((int)form.belgeTur) + ", ";
            sql = sql + VeriErisim.DegerYap(form.belgeNo) + ", ";
            sql = sql + VeriErisim.DegerYap(form.kurum.Replace(".", "")) + ", ";
            sql = sql + VeriErisim.DegerYap(form.muhasebe) + ", ";
            sql = sql + VeriErisim.DegerYap(form.birim) + ", ";
            sql = sql + VeriErisim.DegerYap(form.ilgiliAd) + ", ";
            sql = sql + VeriErisim.DegerYap(form.ilgiliNo) + ", ";
            sql = sql + VeriErisim.DegerYap(form.ilgiliBankaAd) + ", ";
            sql = sql + VeriErisim.DegerYap(form.ilgiliBankaNo) + ", ";
            sql = sql + VeriErisim.DegerYap(form.ilgiliVD) + ", ";
            sql = sql + VeriErisim.DegerYap(form.aciklama) + ", ";
            sql = sql + VeriErisim.DegerYap(form.islemTarih) + ", ";
            sql = sql + VeriErisim.DegerYap(new TNSDateTime(DateTime.Now.ToLongDateString())) + ", ";
            sql = sql + VeriErisim.DegerYap(form.formDurum) + ", ";
            sql = sql + VeriErisim.DegerYap(form.iptalNeden) + ", ";
            sql = sql + VeriErisim.DegerYap(form.YEVYevTarih) + ", ";
            sql = sql + VeriErisim.DegerYap(form.YEVYevNo) + ", ";
            sql = sql + VeriErisim.DegerYap(form.IOETarih) + ", ";
            sql = sql + VeriErisim.DegerYap(form.IOEYevNo) + ", ";
            sql = sql + VeriErisim.DegerYap(form.IOEKim) + ", ";
            sql = sql + VeriErisim.DegerYap(form.AVKTarih) + ", ";
            sql = sql + VeriErisim.DegerYap(form.AVKYevNo) + ", ";
            sql = sql + VeriErisim.DegerYap(form.AVKTutar) + ", ";
            sql = sql + VeriErisim.DegerYap(form.OEBTur) + ", ";
            sql = sql + VeriErisim.DegerYap(form.OEBTarih) + ", ";
            sql = sql + VeriErisim.DegerYap(form.OEBNo) + ", ";
            sql = sql + VeriErisim.DegerYap(form.OEBTutar) + ", ";
            sql = sql + VeriErisim.DegerYap(form.YEVVerNo) + ", ";
            sql = sql + VeriErisim.DegerYap(form.YEVCekNo) + ", ";
            sql = sql + VeriErisim.DegerYap(form.islemYapan) + ",";
            sql = sql + VeriErisim.DegerYap(form.projeNo) + ",";
            sql = sql + VeriErisim.DegerYap(form.ihale) + ",";
            sql = sql + VeriErisim.DegerYap((int)form.fisTur[0]) + ",";
            sql = sql + VeriErisim.DegerYap(form.ilgiliBelgeNo) + ",";
            sql = sql + VeriErisim.DegerYap(form.surecYil) + ",";
            sql = sql + VeriErisim.DegerYap(form.surecNo) + ",";
            sql = sql + VeriErisim.DegerYap(form.kurKod.Replace(".", "")) + ",";
            sql = sql + VeriErisim.DegerYap(form.organizasyonKodu) + ",";
            sql = sql + VeriErisim.DegerYap((string)null) + ",";
            sql = sql + VeriErisim.DegerYap((string)null) + ",";
            sql = sql + VeriErisim.DegerYap(form.istKod1.degerKod) + ",";
            sql = sql + VeriErisim.DegerYap(form.istKod2.degerKod) + ")";
            sonuc = KomutCalistir(sql, 1);

            if (sonuc.islemSonuc && !form.sadeceUstVeri)
            {
                int siraNo = 0;
                foreach (OdemeEmriMIFDetay detay in form.detay.objeler)
                {
                    siraNo++;

                    detay.satirNo = siraNo;

                    sql = "INSERT INTO " + TABLOADMIFD + " (" + VeriErisim.TumAlanlar("", ALANLARMIFD) + ") VALUES (";
                    sql = sql + VeriErisim.DegerYap(form.yil) + ", ";
                    sql = sql + VeriErisim.DegerYap((int)form.belgeTur) + ", ";
                    sql = sql + VeriErisim.DegerYap(form.belgeNo) + ", ";
                    sql = sql + VeriErisim.DegerYap(siraNo) + ", ";
                    sql = sql + VeriErisim.DegerYap(detay.hesapKod.Replace(".", "").Trim()) + ", ";
                    sql = sql + VeriErisim.DegerYap(detay.kur) + ", ";
                    sql = sql + VeriErisim.DegerYap(detay.fon) + ", ";
                    sql = sql + VeriErisim.DegerYap(detay.fin) + ", ";
                    sql = sql + VeriErisim.DegerYap(detay.eko) + ", ";
                    sql = sql + VeriErisim.DegerYap(Math.Round(detay.borc, 2)) + ", ";
                    sql = sql + VeriErisim.DegerYap(Math.Round(detay.alacak, 2)) + ", ";
                    sql = sql + VeriErisim.DegerYap(detay.kesinti) + ",";
                    sql = sql + VeriErisim.DegerYap(detay.peb) + ", ";
                    sql = sql + VeriErisim.DegerYap(detay.altHesapKod.Trim()) + ",";
                    sql = sql + VeriErisim.DegerYap(detay.doviz) + ",";
                    sql = sql + VeriErisim.DegerYap(detay.dovizKurTur) + ",";
                    sql = sql + VeriErisim.DegerYap(detay.dovizBorc) + ",";
                    sql = sql + VeriErisim.DegerYap(detay.dovizAlacak) + ",";
                    sql = sql + VeriErisim.DegerYap(detay.dovizKur) + ",";
                    sql = sql + VeriErisim.DegerYap(detay.satirAciklama) + ",";
                    sql = sql + VeriErisim.DegerYap(detay.detayBirim) + ",";
                    sql = sql + (detay.isKod == 0 ? "NULL" : VeriErisim.DegerYap(detay.isKod)) + ")";

                    sonuc = KomutCalistir(sql, 1);
                }
            }

            string islem = FormDurumAciklama(form.formDurum, form.belgeTur);
            KaydetTarihce(form.yil, (int)form.belgeTur, form.belgeNo, form.islemYapan, islem, form.islemYapanAd);

            return sonuc;
        }

        /// <summary>
        /// Parametre olarak gelen bilgileri veri tabanına ekleyen sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="form">Eklenecek kayıt bilgisini tutan nesne</param>
        /// <returns>Kaydet komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc KaydetODEMIFTekSatir(OdemeEmriMIFForm form)
        {
            Sonuc sonuc = new Sonuc(false, "", "");
            string sql = "";
            int satirNo = 0;
            int belgeTur = 0;

            if (((OdemeEmriMIFDetay)form.detay[0]).satirNo == 0)
            {
                satirNo = EnBuyukSatirNo(form.yil, form.belgeNo);
                belgeTur = BelgeTurVer(form.yil, form.belgeNo);
            }

            foreach (OdemeEmriMIFDetay detay in form.detay.objeler)
            {
                if (detay.satirNo > 0)
                {
                    sql = "UPDATE " + TABLOADMIFD;
                    sql += " SET " + VeriErisim.KosulOl("", ALANLARMIFD[(int)ENUMAlanlarMIFD.HESAPKOD], "=", detay.hesapKod.Replace(".", "").Trim()) + ",";
                    sql += VeriErisim.KosulOl("", ALANLARMIFD[(int)ENUMAlanlarMIFD.BORC], "=", detay.borc) + ",";
                    sql += VeriErisim.KosulOl("", ALANLARMIFD[(int)ENUMAlanlarMIFD.ALACAK], "=", detay.alacak) + ",";
                    sql += VeriErisim.KosulOl("", ALANLARMIFD[(int)ENUMAlanlarMIFD.KESINTI], "=", detay.kesinti) + ",";
                    sql += VeriErisim.KosulOl("", ALANLARMIFD[(int)ENUMAlanlarMIFD.ALTHESAPKOD], "=", detay.altHesapKod.Trim()) + ",";
                    sql += VeriErisim.KosulOl("", ALANLARMIFD[(int)ENUMAlanlarMIFD.KURKOD], "=", detay.kur.Replace(".", "")) + ",";
                    sql += VeriErisim.KosulOl("", ALANLARMIFD[(int)ENUMAlanlarMIFD.FONKOD], "=", detay.fon.Replace(".", "")) + ",";
                    sql += VeriErisim.KosulOl("", ALANLARMIFD[(int)ENUMAlanlarMIFD.FINKOD], "=", detay.fin.Replace(".", "")) + ",";
                    sql += VeriErisim.KosulOl("", ALANLARMIFD[(int)ENUMAlanlarMIFD.EKOKOD], "=", detay.eko.Replace(".", "")) + ",";
                    sql += VeriErisim.KosulOl("", ALANLARMIFD[(int)ENUMAlanlarMIFD.DOVIZ], "=", detay.doviz) + ",";
                    sql += VeriErisim.KosulOl("", ALANLARMIFD[(int)ENUMAlanlarMIFD.DOVIZKURTUR], "=", detay.dovizKurTur) + ",";
                    sql += VeriErisim.KosulOl("", ALANLARMIFD[(int)ENUMAlanlarMIFD.DOVIZBORC], "=", detay.dovizBorc) + ",";
                    sql += VeriErisim.KosulOl("", ALANLARMIFD[(int)ENUMAlanlarMIFD.DOVIZALACAK], "=", detay.dovizAlacak) + ",";
                    sql += VeriErisim.KosulOl("", ALANLARMIFD[(int)ENUMAlanlarMIFD.DOVIZKUR], "=", detay.dovizKur) + ",";
                    sql += VeriErisim.KosulOl("", ALANLARMIFD[(int)ENUMAlanlarMIFD.ACIKLAMA], "=", detay.satirAciklama) + ",";
                    sql += VeriErisim.KosulOl("", ALANLARMIFD[(int)ENUMAlanlarMIFD.PEBKOD], "=", detay.peb.Replace(".", "")) + ",";
                    sql += VeriErisim.KosulOl("", ALANLARMIFD[(int)ENUMAlanlarMIFD.DETAYBIRIM], "=", detay.detayBirim) + ",";

                    if (detay.isKod == 0)
                        sql += VeriErisim.KosulOl("", ALANLARMIFD[(int)ENUMAlanlarMIFD.ISKOD], "=", "NULL");
                    else
                        sql += VeriErisim.KosulOl("", ALANLARMIFD[(int)ENUMAlanlarMIFD.ISKOD], "=", detay.isKod);

                    sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARMIFD[(int)ENUMAlanlarMIFD.YIL], "=", form.yil));
                    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARMIFD[(int)ENUMAlanlarMIFD.BELGENO], "=", form.belgeNo));
                    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARMIFD[(int)ENUMAlanlarMIFD.SIRA], "=", detay.satirNo));
                }
                else
                {
                    sql = "INSERT INTO " + TABLOADMIFD + " (" + VeriErisim.TumAlanlar("", ALANLARMIFD) + ") VALUES (";
                    sql = sql + VeriErisim.DegerYap(form.yil) + ", ";
                    sql = sql + VeriErisim.DegerYap(belgeTur) + ", ";
                    sql = sql + VeriErisim.DegerYap(form.belgeNo) + ", ";
                    sql = sql + VeriErisim.DegerYap(satirNo) + ", ";
                    sql = sql + VeriErisim.DegerYap(detay.hesapKod.Replace(".", "").Trim()) + ", ";
                    sql = sql + VeriErisim.DegerYap(detay.kur.Replace(".", "")) + ", ";
                    sql = sql + VeriErisim.DegerYap(detay.fon.Replace(".", "")) + ", ";
                    sql = sql + VeriErisim.DegerYap(detay.fin.Replace(".", "")) + ", ";
                    sql = sql + VeriErisim.DegerYap(detay.eko.Replace(".", "")) + ", ";
                    sql = sql + VeriErisim.DegerYap(detay.borc) + ", ";
                    sql = sql + VeriErisim.DegerYap(detay.alacak) + ", ";
                    sql = sql + VeriErisim.DegerYap(detay.kesinti) + ",";
                    sql = sql + VeriErisim.DegerYap(detay.peb.Replace(".", "")) + ",";
                    sql = sql + VeriErisim.DegerYap(detay.altHesapKod) + ",";
                    sql = sql + VeriErisim.DegerYap(detay.doviz) + ",";
                    sql = sql + VeriErisim.DegerYap(detay.dovizKurTur) + ",";
                    sql = sql + VeriErisim.DegerYap(detay.dovizBorc) + ",";
                    sql = sql + VeriErisim.DegerYap(detay.dovizAlacak) + ",";
                    sql = sql + VeriErisim.DegerYap(detay.dovizKur) + ",";
                    sql = sql + VeriErisim.DegerYap(detay.satirAciklama) + ",";
                    sql = sql + VeriErisim.DegerYap(detay.detayBirim) + ",";
                    sql = sql + (detay.isKod == 0 ? "NULL" : VeriErisim.DegerYap(detay.isKod)) + ")";


                    satirNo++;
                }
                sonuc = KomutCalistir(sql, 1);
            }
            return sonuc;
        }

        /// <summary>
        /// Parametre olarak gelen bilgileri veri tabanına ekleyen sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="yil">Yıl.</param>
        /// <param name="belgeNo">Belge numarası.</param>
        /// <param name="ilgiliBelgeNo">İlgili belge no.</param>
        /// <returns></returns>
        public Sonuc KaydetODEMIFIlgiliBelge(int yil, string belgeNo, string ilgiliBelgeNo)
        {
            string sql = "UPDATE " + TABLOADMIFF + " SET ";
            sql += ALANLARMIFF[(int)ENUMAlanlarMIFF.ILGILIBELGENO] + "=" + VeriErisim.DegerYap(ilgiliBelgeNo);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARMIFF[(int)ENUMAlanlarMIFF.YIL], "=", yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARMIFF[(int)ENUMAlanlarMIFF.BELGENO], "=", belgeNo));

            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Parametre olarak gelen anahtar alanları kullanarak, ilgili kaydı veri tabanından silen sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="yil">Yıl</param>
        /// <param name="belgeNo">Belge numarası.</param>
        /// <returns>Sil komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc Sil(int yil, string belgeNo, bool sadeceUstVeri)
        {
            string sql = "DELETE FROM " + TABLOADMIFD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARMIFD[(int)ENUMAlanlarMIFD.YIL], "=", yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARMIFD[(int)ENUMAlanlarMIFD.BELGENO], "=", belgeNo));
            Sonuc sonuc = new Sonuc();

            if (!sadeceUstVeri)
                sonuc = KomutCalistir(sql, 0);

            if (sonuc.islemSonuc)
            {
                sql = "DELETE FROM " + TABLOADMIFF;
                sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARMIFF[(int)ENUMAlanlarMIFF.YIL], "=", yil));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARMIFF[(int)ENUMAlanlarMIFF.BELGENO], "=", belgeNo));
                sonuc = KomutCalistir(sql, 0);
            }

            return sonuc;
        }

        /// <summary>
        /// Form listesi sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon..
        /// </summary>
        /// <param name="f">Ödeme emri/MİF form bilgileri içeren nesne.</param>
        /// <param name="detay">Eğer <c>true</c> ise detay bilgileri listele.</param>
        /// <param name="mahsup">Eğer <c>true</c> ise mahsup bilgilerini listele.</param>
        /// <param name="onceAnaSonraTarih">if set to <c>true</c> [once ana sonra tarih].</param>
        /// <returns></returns>
        public IDataReader FormListe(OdemeEmriMIFForm f, bool detay, bool mahsup, bool onceAnaSonraTarih) // = false
        {
            string sql;
            string borcAlan = "BORC";
            string alacakAlan = "ALACAK";
            if (f.sorguDovizliIslemler)
            {
                borcAlan = "DOVIZBORC";
                alacakAlan = "DOVIZALACAK";
            }

            if (detay)
            {
                sql = "SELECT " + VeriErisim.TumAlanlar(TABLOADMIFF, ALANLARMIFF) + ",KURAD,MUHASEBEAD," + VeriErisim.TumAlanlar(TABLOADMIFD, ALANLARMIFD) + ",MUHIS.ADI,MUHHESAPPLANI.ACIKLAMA, ";
                sql += "KAPATAN, KAPATILAN";
                sql += ", (SELECT MIN(AD) FROM " + TABOrtak.DatabaseAdBul("HarcamaYonetimi.dbo.") + "HRCFIRMABILGISI where VERGINO=MUHMIF.ALTHESAPKOD)";
                sql += ", (SELECT MIN(AD + ' ' + SOYAD) FROM " + TABOrtak.DatabaseAdBul("Personel.dbo.") + "PERPersonel where TCKIMLIKNO=MUHMIF.ALTHESAPKOD)";
                if (Arac.prgBag)
                    sql += ", PRGBAG.PRGKOD, PRGBAG.ALTPRGKOD, PRGBAG.FAALKOD, PRGBAG.ALTFAALKOD";
                else
                    sql += ", 0, 0, 0, 0";
            }
            else
            {
                sql = "SELECT " + VeriErisim.TumAlanlar(TABLOADMIFF, ALANLARMIFF) + ", KURAD, MUHASEBEAD, SUM(" + borcAlan + "), SUM(" + alacakAlan + ")";
                //Bu satır 17.07.2018 tarihinde eklendi. Tuğçe hanım listeleme ekranında ödeme emrine eklenmiş dosya var mı görmek istiyor. Melih
                sql += ", (SELECT count(*) FROM MUHBELGELER where YIL=MUHMIFFORM.YIL AND BELGENO=MUHMIFFORM.BELGENO)";
            }

            if (detay)
            {
                sql += " FROM MUHMIF";
                sql += " INNER JOIN MUHMIFFORM ON MUHMIF.YIL = MUHMIFFORM.YIL AND MUHMIF.BELGENO = MUHMIFFORM.BELGENO";
                sql += " INNER JOIN ABSKUR ON MUHMIFFORM.YIL = ABSKUR.YIL AND MUHMIFFORM.KURUMKOD = ABSKUR.KURKOD";

                sql += " LEFT OUTER JOIN MUHHESAPPLANI ON MUHMIF.HESAPKOD = MUHHESAPPLANI.HESAPKOD AND MUHMIF.YIL = MUHHESAPPLANI.YIL";
                sql += " LEFT OUTER JOIN MUHMUHASEBE ON MUHMIFFORM.MUHASEBEKOD = MUHMUHASEBE.MUHASEBEKOD";
                sql += " CROSS APPLY (Select convert(varchar,TA.YIL)+'^'+TA.BELGENO+'^'+TA.KBELGENO+'^'+convert(varchar,TA.KTUTAR)+'^'+convert(varchar,TA.BORCALACAK)+'^'+TA.KILGILIVNO+'^'+TA.KBIRIMKOD+'|' From MUHKAPATMA TA WHERE MUHMIF.YIL = TA.YIL    AND MUHMIF.BELGENO = TA.BELGENO    AND MUHMIF.HESAPKOD = TA.KHESAPKOD AND MUHMIF.DETAYBIRIM = TA.KBIRIMKOD AND MUHMIF.SIRA = TA.SIRANO For XML PATH('')) TKP(KAPATAN)";
                sql += " CROSS APPLY (Select convert(varchar,TA.YIL)+'^'+TA.BELGENO+'^'+TA.KBELGENO+'^'+convert(varchar,TA.KTUTAR)+'^'+convert(varchar,TA.BORCALACAK)+'^'+TA.KILGILIVNO+'^'+TA.KBIRIMKOD+'|' From MUHKAPATMA TA WHERE MUHMIF.YIL = TA.KYIL   AND MUHMIF.BELGENO = TA.KBELGENO   AND MUHMIF.HESAPKOD = TA.KHESAPKOD AND MUHMIF.DETAYBIRIM = TA.KBIRIMKOD AND MUHMIF.ALTHESAPKOD = TA.KILGILIVNO For XML PATH('')) TKN(KAPATILAN)";
                sql += " LEFT OUTER JOIN MUHIS ON MUHMIF.ISKOD = MUHIS.KOD";
                if (Arac.prgBag)
                    sql += " LEFT OUTER JOIN " + TABOrtak.DatabaseAdBul("ButceHazirlik.dbo.") + "PRGPEBILISKI PRGBAG ON MUHMIF.PEBKOD != '' AND PRGBAG.YIL = MUHMIF.YIL AND PRGBAG.PEBKOD = MUHMIF.PEBKOD"; //Prg bağ tablosu bağlantı kurulacak
            }
            else
            {
                sql += " FROM MUHMIF";
                sql += " INNER JOIN MUHMIFFORM ON MUHMIF.YIL = MUHMIFFORM.YIL AND MUHMIF.BELGENO = MUHMIFFORM.BELGENO";
                sql += " INNER JOIN ABSKUR ON MUHMIFFORM.YIL = ABSKUR.YIL AND MUHMIFFORM.KURUMKOD = ABSKUR.KURKOD";
                sql += " LEFT OUTER JOIN MUHMUHASEBE ON MUHMIFFORM.MUHASEBEKOD = MUHMUHASEBE.MUHASEBEKOD";
            }

            if (f.yil == 0)
                sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOADMIFF, ALANLARMIFF[(int)ENUMAlanlarMIFF.YIL], ">", f.yil));
            else
                sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOADMIFF, ALANLARMIFF[(int)ENUMAlanlarMIFF.YIL], "=", f.yil));

            if (f.sorguBelgeTur > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADMIFF, ALANLARMIFF[(int)ENUMAlanlarMIFF.BELGETUR], "=", (int)f.sorguBelgeTur));

            if (f.formDurum > 0)
            {
                if (f.formDurum == 1 || f.formDurum == 20 || f.formDurum == 21)
                {
                    sql += " AND (" + VeriErisim.KosulOl(TABLOADMIFF, ALANLARMIFF[(int)ENUMAlanlarMIFF.FORMDURUM], "=", (int)ENUMBelgeDurumu.YENI);
                    sql += " OR " + VeriErisim.KosulOl(TABLOADMIFF, ALANLARMIFF[(int)ENUMAlanlarMIFF.FORMDURUM], "=", (int)ENUMBelgeDurumu.DEGISTIRILDI) + ")";
                }
                else if (f.formDurum == 22)
                    sql += " AND " + VeriErisim.KosulOl(TABLOADMIFF, ALANLARMIFF[(int)ENUMAlanlarMIFF.FORMDURUM], "<", (int)ENUMBelgeDurumu.IPTAL);
                else if (f.formDurum == 23)
                {
                    sql += " AND " + VeriErisim.KosulOl(TABLOADMIFF, ALANLARMIFF[(int)ENUMAlanlarMIFF.FORMDURUM], "<>", (int)ENUMBelgeDurumu.IPTAL);
                    sql += " AND (" + VeriErisim.KosulOl(TABLOADMIFF, ALANLARMIFF[(int)ENUMAlanlarMIFF.YEVMIYENO], "=", 0);
                    sql += " OR " + TABLOADMIFF + "." + ALANLARMIFF[(int)ENUMAlanlarMIFF.YEVMIYENO] + "=" + "null" + ")";
                }
                else if (f.formDurum == 24)
                {
                    sql += " AND (" + VeriErisim.KosulOl(TABLOADMIFF, ALANLARMIFF[(int)ENUMAlanlarMIFF.FORMDURUM], "=", (int)ENUMBelgeDurumu.ONAYLI);
                    sql += " OR " + VeriErisim.KosulOl(TABLOADMIFF, ALANLARMIFF[(int)ENUMAlanlarMIFF.FORMDURUM], "=", (int)ENUMBelgeDurumu.YEVMIYE_OESGBYEGONDER) + ")";
                }
                else
                    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARMIFF[(int)ENUMAlanlarMIFF.FORMDURUM], "=", f.formDurum));

                if (f.formDurum == 21)
                    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARMIFF[(int)ENUMAlanlarMIFF.YEVMIYENO], ">", 0));
            }

            if (f.YEVYevNo > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARMIFF[(int)ENUMAlanlarMIFF.YEVMIYENO], "=", f.YEVYevNo));

            if (f.muhasebe > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADMIFF, ALANLARMIFF[(int)ENUMAlanlarMIFF.MUHASEBEKOD], "=", f.muhasebe));

            if (f.muhasebeListe != "")
            {
                string[] birimler = f.muhasebeListe.Split(';');
                string birimKosul = "";
                for (int i = 0; i < birimler.Length; i++)
                    birimKosul = VeriErisim.KosulEkle(birimKosul, "OR", VeriErisim.KosulOl(TABLOADMIFF, ALANLARMIFF[(int)ENUMAlanlarMIFF.MUHASEBEKOD], "=", OrtakFonksiyonlar.ConvertToInt(birimler[i], 0)));

                if (birimKosul != "")
                    sql += " AND (" + birimKosul + ")";
            }

            if (f.birim != "")
            {
                string[] birimler = f.birim.Split(';');
                string birimKosul = "";
                for (int i = 0; i < birimler.Length; i++)
                {
                    if (ANABIRIM)
                        birimKosul = VeriErisim.KosulEkle(birimKosul, "OR", VeriErisim.KosulOl("", ALANLARMIFF[(int)ENUMAlanlarMIFF.BIRIMKOD], "=", birimler[i]));
                    else
                        birimKosul = VeriErisim.KosulEkle(birimKosul, "OR", VeriErisim.KosulOl("", ALANLARMIFD[(int)ENUMAlanlarMIFD.DETAYBIRIM], "=", birimler[i]));
                }
                //Birim veya Kurumsal Kod
                string birimKurKosul = "";
                foreach (OdemeEmriMIFDetay d in f.detay.objeler)
                {
                    if (d.kur != "")
                    {
                        string[] dizi = d.kur.Split(';');
                        for (int i = 0; i < dizi.Length; i++)
                        {
                            if (dizi[i].Trim() != "")
                                birimKurKosul = VeriErisim.KosulEkle(birimKurKosul, "OR", VeriErisim.KosulOl(TABLOADMIFD, ALANLARMIFD[(int)ENUMAlanlarMIFD.KURKOD], "LIKE", dizi[i].Replace(".", "") + "%"));
                        }
                        d.kur = "";
                    }
                }
                if (birimKurKosul != "")
                {
                    birimKurKosul = VeriErisim.KosulOl("", ALANLARMIFF[(int)ENUMAlanlarMIFF.BIRIMKOD], "=", "000") + " AND (" + birimKurKosul + ")";
                    if (birimKosul != "")
                        birimKosul += " OR (" + birimKurKosul + ")";
                }

                if (birimKosul != "")
                    sql += " AND (" + birimKosul + ")";
            }

            if (f.sorguBirim != "")
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARMIFF[(int)ENUMAlanlarMIFF.BIRIMKOD], "=", f.sorguBirim));

            if (f.islemYapan != "")
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARMIFF[(int)ENUMAlanlarMIFF.ISLEMYAPAN], "=", f.islemYapan));

            if (!f.formDurumTarih.isNull)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADMIFF, ALANLARMIFF[(int)ENUMAlanlarMIFF.DURUMTARIH], "=", f.formDurumTarih));

            if (!f.YEVYevTarih.isNull)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADMIFF, ALANLARMIFF[(int)ENUMAlanlarMIFF.YEVMIYETARIH], ">=", f.YEVYevTarih));

            if (!f.islemTarih.isNull)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADMIFF, ALANLARMIFF[(int)ENUMAlanlarMIFF.ISLEMTARIH], ">=", f.islemTarih));

            if (f.ilgiliAd != "")
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARMIFF[(int)ENUMAlanlarMIFF.IADSOYAD], " LIKE ", "%" + f.ilgiliAd + "%"));

            if (f.ilgiliNo != "")
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARMIFF[(int)ENUMAlanlarMIFF.IKIMLIKNO], "=", f.ilgiliNo));

            if (f.projeNo != "")
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARMIFF[(int)ENUMAlanlarMIFF.PROJENO], "=", f.projeNo));

            if (f.aciklama != "")
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADMIFF, ALANLARMIFF[(int)ENUMAlanlarMIFF.ACIKLAMA], "LIKE", "%" + f.aciklama + "%"));

            if (f.surecYil > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARMIFF[(int)ENUMAlanlarMIFF.SURECYIL], "=", f.surecYil));

            if (f.surecNo != "" && f.surecNo != "000000")
            {
                if (f.surecNo == "0000-1")
                    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARMIFF[(int)ENUMAlanlarMIFF.SURECNO], "=", ""));
                else if (f.surecNo == "0000-2")
                    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARMIFF[(int)ENUMAlanlarMIFF.SURECNO], "<>", ""));
                else
                    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARMIFF[(int)ENUMAlanlarMIFF.SURECNO], "=", f.surecNo));
            }

            if (!string.IsNullOrWhiteSpace(f.ihale))
            {
                if (f.ihale == "-1")
                    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARMIFF[(int)ENUMAlanlarMIFF.IHALE], "=", ""));
                else if (f.ihale == "-2")
                    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARMIFF[(int)ENUMAlanlarMIFF.IHALE], "<>", ""));
                else
                    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARMIFF[(int)ENUMAlanlarMIFF.IHALE], "LIKE", f.ihale.Replace(".", "") + "%"));
            }

            if (f.fisTur != null && f.fisTur.Length > 0)
            {
                sql = VeriErisim.KosulEkle(sql, "AND", TABMizanBilanco.SorguOl(ALANLARMIFF[(int)ENUMAlanlarMIFF.FISTUR], f.fisTur));
            }

            if (f.OEBTur != "")
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARMIFF[(int)ENUMAlanlarMIFF.OTUR], "LIKE", f.OEBTur + "%"));

            if (f.OEBNo != "")
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARMIFF[(int)ENUMAlanlarMIFF.ONO], "=", f.OEBNo));

            if (!f.OEBTarih.isNull)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARMIFF[(int)ENUMAlanlarMIFF.OTARIH], "=", f.OEBTarih));

            if (f.YEVCekNo != "")
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARMIFF[(int)ENUMAlanlarMIFF.CEKEMRINO], "LIKE", f.YEVCekNo + "%"));

            if (f.belgeNo != "")
            {
                string[] t = f.belgeNo.Split(';');

                if (t.Length == 1 && t[0] != "")
                    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADMIFF, ALANLARMIFF[(int)ENUMAlanlarMIFF.BELGENO], "=", t[0]));

                if (t.Length == 2)
                {
                    if (t[1] != "")
                    {
                        sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADMIFF, ALANLARMIFF[(int)ENUMAlanlarMIFF.BELGENO], ">=", t[0]));
                        sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADMIFF, ALANLARMIFF[(int)ENUMAlanlarMIFF.BELGENO], "<=", t[1]));
                    }
                    else
                        sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADMIFF, ALANLARMIFF[(int)ENUMAlanlarMIFF.BELGENO], "=", t[0]));
                }
            }

            if (f.sorguYEVYevNo != "")
            {
                string[] t = f.sorguYEVYevNo.Split(';');

                if (t.Length == 1 && t[0] != "")
                    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADMIFF, ALANLARMIFF[(int)ENUMAlanlarMIFF.YEVMIYENO], "=", t[0]));

                if (t.Length == 2)
                {
                    if (t[1] != "")
                    {
                        sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADMIFF, ALANLARMIFF[(int)ENUMAlanlarMIFF.YEVMIYENO], ">=", t[0]));
                        sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADMIFF, ALANLARMIFF[(int)ENUMAlanlarMIFF.YEVMIYENO], "<=", t[1]));
                    }
                    else
                        sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADMIFF, ALANLARMIFF[(int)ENUMAlanlarMIFF.YEVMIYENO], "=", t[0]));
                }
            }

            if (f.sorguIslemTarih != "")
            {
                string[] t = f.sorguIslemTarih.Split(';');
                YilKontrol(ref t, f.yil);

                System.DateTime d1;
                System.DateTime d2;
                if (t.Length == 1 && t[0] != "" || (t.Length == 2 && t[0] != "" && t[1] == ""))
                {
                    d1 = Convert.ToDateTime(new TNSDateTime(t[0]).ToString());
                    d2 = Convert.ToDateTime(new TNSDateTime(t[0]).ToString()).AddDays(1).AddSeconds(-1);
                    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADMIFF, ALANLARMIFF[(int)ENUMAlanlarMIFF.ISLEMTARIH], ">=", new TNSDateTime(d1)));
                    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADMIFF, ALANLARMIFF[(int)ENUMAlanlarMIFF.ISLEMTARIH], "<=", new TNSDateTime(d2)));
                }
                else if (t.Length == 2 && t[0] != "" && t[1] != "")
                {
                    d1 = Convert.ToDateTime(new TNSDateTime(t[0]).ToString());
                    d2 = Convert.ToDateTime(new TNSDateTime(t[1]).ToString()).AddDays(1).AddSeconds(-1);
                    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADMIFF, ALANLARMIFF[(int)ENUMAlanlarMIFF.ISLEMTARIH], ">=", new TNSDateTime(d1)));
                    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADMIFF, ALANLARMIFF[(int)ENUMAlanlarMIFF.ISLEMTARIH], "<=", new TNSDateTime(d2)));
                }
                else if (t.Length == 2 && t[0] == "" && t[1] != "")
                {
                    d2 = Convert.ToDateTime(new TNSDateTime(t[1]).ToString()).AddDays(1).AddSeconds(-1);
                    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADMIFF, ALANLARMIFF[(int)ENUMAlanlarMIFF.ISLEMTARIH], "<=", new TNSDateTime(d2)));
                }
            }

            if (f.sorguYEVYevTarih != "")
            {
                string[] t = f.sorguYEVYevTarih.Split(';');
                YilKontrol(ref t, f.yil);

                System.DateTime d1;
                System.DateTime d2;
                if (t.Length == 1 && t[0] != "" || (t.Length == 2 && t[0] != "" && t[1] == ""))
                {
                    d1 = Convert.ToDateTime(new TNSDateTime(t[0]).ToString());
                    d2 = Convert.ToDateTime(new TNSDateTime(t[0]).ToString()).AddDays(1).AddSeconds(-1);
                    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADMIFF, ALANLARMIFF[(int)ENUMAlanlarMIFF.YEVMIYETARIH], ">=", new TNSDateTime(d1)));
                    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADMIFF, ALANLARMIFF[(int)ENUMAlanlarMIFF.YEVMIYETARIH], "<=", new TNSDateTime(d2)));
                }
                else if (t.Length == 2 && t[0] != "" && t[1] != "")
                {
                    d1 = Convert.ToDateTime(new TNSDateTime(t[0]).ToString());
                    d2 = Convert.ToDateTime(new TNSDateTime(t[1]).ToString()).AddDays(1).AddSeconds(-1);
                    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADMIFF, ALANLARMIFF[(int)ENUMAlanlarMIFF.YEVMIYETARIH], ">=", new TNSDateTime(d1)));
                    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADMIFF, ALANLARMIFF[(int)ENUMAlanlarMIFF.YEVMIYETARIH], "<=", new TNSDateTime(d2)));
                }
                else if (t.Length == 2 && t[0] == "" && t[1] != "")
                {
                    d2 = Convert.ToDateTime(new TNSDateTime(t[1]).ToString()).AddDays(1).AddSeconds(-1);
                    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADMIFF, ALANLARMIFF[(int)ENUMAlanlarMIFF.YEVMIYETARIH], "<=", new TNSDateTime(d2)));
                }
            }

            if (f.sorguFormDurumTarih != "")
            {
                string[] t = f.sorguFormDurumTarih.Split(';');
                YilKontrol(ref t, f.yil);

                System.DateTime d1;
                System.DateTime d2;
                if (t.Length == 1 && t[0] != "" || (t.Length == 2 && t[0] != "" && t[1] == ""))
                {
                    d1 = Convert.ToDateTime(new TNSDateTime(t[0]).ToString());
                    d2 = Convert.ToDateTime(new TNSDateTime(t[0]).ToString()).AddDays(1).AddSeconds(-1);
                    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADMIFF, ALANLARMIFF[(int)ENUMAlanlarMIFF.DURUMTARIH], ">=", new TNSDateTime(d1)));
                    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADMIFF, ALANLARMIFF[(int)ENUMAlanlarMIFF.DURUMTARIH], "<=", new TNSDateTime(d2)));
                }
                else if (t.Length == 2 && t[0] != "" && t[1] != "")
                {
                    d1 = Convert.ToDateTime(new TNSDateTime(t[0]).ToString());
                    d2 = Convert.ToDateTime(new TNSDateTime(t[1]).ToString()).AddDays(1).AddSeconds(-1);
                    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADMIFF, ALANLARMIFF[(int)ENUMAlanlarMIFF.DURUMTARIH], ">=", new TNSDateTime(d1)));
                    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADMIFF, ALANLARMIFF[(int)ENUMAlanlarMIFF.DURUMTARIH], "<=", new TNSDateTime(d2)));
                }
                else if (t.Length == 2 && t[0] == "" && t[1] != "")
                {
                    d2 = Convert.ToDateTime(new TNSDateTime(t[1]).ToString()).AddDays(1).AddSeconds(-1);
                    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADMIFF, ALANLARMIFF[(int)ENUMAlanlarMIFF.DURUMTARIH], "<=", new TNSDateTime(d2)));
                }
            }

            if (f.sorguDovizliIslemler)
            {
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("MUHHESAPPLANI", "DOVIZ", "<>", ""));
                sql = VeriErisim.KosulEkle(sql, "AND", "MUHHESAPPLANI.DOVIZ IS NOT NULL");
            }

            sql = VeriErisim.KosulEkle(sql, "AND", IstatistikKosulOl(TABLOADMIFF + "." + ALANLARMIFF[(int)ENUMAlanlarMIFF.ISTKOD1], f.istKod1.degerKod));
            sql = VeriErisim.KosulEkle(sql, "AND", IstatistikKosulOl(TABLOADMIFF + "." + ALANLARMIFF[(int)ENUMAlanlarMIFF.ISTKOD2], f.istKod2.degerKod));
            bool altHesapKosulYok = true;
            foreach (OdemeEmriMIFDetay d in f.detay.objeler)
            {
                if (d.hesapKod.Trim() != "")
                {
                    //Eğer noktalı virgül ile gelen hesap kodu sayısı 2 den fazla ise hesap kodu aralığı değildir Melih 20.08.2015 
                    //**************************************************************************************************************
                    string[] kontrol = d.hesapKod.Split(';');
                    if (kontrol.Length > 2)
                        d.hesapKod = d.hesapKod.Replace(";", "|");
                    //**************************************************************************************************************
                    if (d.hesapKod.IndexOf(";") > 0)
                    {
                        string[] hler = d.hesapKod.Split(';');
                        sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADMIFD, ALANLARMIFD[(int)ENUMAlanlarMIFD.HESAPKOD], ">=", hler[0].Replace(".", "").Trim() + ""));
                        sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADMIFD, ALANLARMIFD[(int)ENUMAlanlarMIFD.HESAPKOD], "<=", hler[1].Replace(".", "").Trim() + "z"));
                    }
                    else if (d.hesapKod.IndexOf("|") > 0 || d.hesapKod.IndexOf(",") > 0)
                    {
                        string islec;
                        string yuzde;
                        if (d.hesapKod.IndexOf(",") > 0)
                        {
                            islec = "like";
                            yuzde = "%";
                        }
                        else
                        {
                            islec = "=";
                            yuzde = "";
                        }
                        string[] hler = d.hesapKod.Split('|', ',');
                        string ekKosul = "";
                        for (int iii = 0; iii < hler.Length; iii++)
                        {
                            if (hler[iii].Replace(".", "").Trim() == "") continue;
                            ekKosul = VeriErisim.KosulEkle(ekKosul, "OR", VeriErisim.KosulOl(TABLOADMIFD, ALANLARMIFD[(int)ENUMAlanlarMIFD.HESAPKOD], islec, hler[iii].Replace(".", "").Trim() + yuzde));
                        }

                        if (ekKosul != "")
                            sql = sql + " AND (" + ekKosul + ")";
                    }
                    else
                        sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADMIFD, ALANLARMIFD[(int)ENUMAlanlarMIFD.HESAPKOD], "LIKE", d.hesapKod.Replace(".", "").Trim() + "%"));
                }

                if (!string.IsNullOrWhiteSpace(d.altHesapKod))
                {
                    altHesapKosulYok = false;
                    if (d.altHesapKod == "-999-")
                        sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADMIFD, ALANLARMIFD[(int)ENUMAlanlarMIFD.ALTHESAPKOD], "<>", ""));
                    else
                    {
                        string ekKosul = "";
                        ekKosul = VeriErisim.KosulEkle(ekKosul, "AND", VeriErisim.KosulOl(TABLOADMIFD, ALANLARMIFD[(int)ENUMAlanlarMIFD.ALTHESAPKOD], "=", d.altHesapKod));

                        if (!d.sadeceAltHesaptanKontrol)
                            ekKosul = VeriErisim.KosulEkle(ekKosul, "OR", VeriErisim.KosulOl(TABLOADMIFF, ALANLARMIFF[(int)ENUMAlanlarMIFF.IKIMLIKNO], "=", d.altHesapKod));

                        if (ekKosul != "")
                            sql = sql + " AND (" + ekKosul + ")";

                    }
                }

                if (d.borc > 0)
                    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADMIFD, ALANLARMIFD[(int)ENUMAlanlarMIFD.BORC], ">=", d.borc));

                if (d.alacak > 0)
                    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADMIFD, ALANLARMIFD[(int)ENUMAlanlarMIFD.ALACAK], ">=", d.alacak));

                if (d.kur != "")
                {
                    string[] dizi = d.kur.Split(';');
                    string kosul = "";
                    for (int i = 0; i < dizi.Length; i++)
                        if (dizi[i].Trim() != "")
                            kosul = VeriErisim.KosulEkle(kosul, "OR", VeriErisim.KosulOl(TABLOADMIFD, ALANLARMIFD[(int)ENUMAlanlarMIFD.KURKOD], "LIKE", dizi[i].Replace(".", "") + "%"));

                    sql = VeriErisim.KosulEkle(sql, "AND", "(" + kosul + ")");
                }
                if (d.fon != "")
                    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADMIFD, ALANLARMIFD[(int)ENUMAlanlarMIFD.FONKOD], "LIKE", d.fon.Replace(".", "") + "%"));
                if (d.fin != "")
                    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADMIFD, ALANLARMIFD[(int)ENUMAlanlarMIFD.FINKOD], "=", d.fin));
                if (d.eko != "")
                {
                    string[] dizi = d.eko.Split(';');
                    string kosul = "";
                    for (int i = 0; i < dizi.Length; i++)
                        if (dizi[i].Trim() != "")
                            kosul = VeriErisim.KosulEkle(kosul, "OR", VeriErisim.KosulOl(TABLOADMIFD, ALANLARMIFD[(int)ENUMAlanlarMIFD.EKOKOD], "LIKE", dizi[i].Replace(".", "") + "%"));

                    sql = VeriErisim.KosulEkle(sql, "AND", "(" + kosul + ")");
                }
                if (d.peb != "")
                    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADMIFD, ALANLARMIFD[(int)ENUMAlanlarMIFD.PEBKOD], "LIKE", d.peb.Replace(".", "") + "%"));

                if (mahsup)
                {
                    if (d.kapatma != null)
                    {
                        foreach (KapanisBilgileri kapatma in d.kapatma.objeler)
                        {
                            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADKAPANIS, ALANLARKAPANIS[(int)ENUMAlanlarKAPANIS.YIL], "=", kapatma.yil));
                            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADKAPANIS, ALANLARKAPANIS[(int)ENUMAlanlarKAPANIS.BELGENO], "=", kapatma.belgeNo));
                            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADKAPANIS, ALANLARKAPANIS[(int)ENUMAlanlarKAPANIS.KBIRIMKOD], "=", kapatma.kBirimKod));
                        }
                    }
                }

                if (d.isKod > 0)
                    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADMIFD, ALANLARMIFD[(int)ENUMAlanlarMIFD.ISKOD], "=", d.isKod));

            }

            if (mahsup)
            {
                //sql += " AND ";
                //sql += "(" + TABLOADMIFF + "." + ALANLARMIFF[(int)ENUMAlanlarMIFF.ILGILIBELGENO] + " = ''";
                //sql += " OR " + TABLOADMIFF + "." + ALANLARMIFF[(int)ENUMAlanlarMIFF.ILGILIBELGENO] + " IS NULL)";

                //sql += " AND " + TABLOADMIFF + "." + ALANLARMIFF[(int)ENUMAlanlarMIFF.BELGENO] + " NOT IN ";
                //sql += " (SELECT " + ALANLARMIFF[(int)ENUMAlanlarMIFF.ILGILIBELGENO] + " FROM " + TABLOADMIFF;
                //sql += " WHERE ";
                //sql += TABLOADMIFF + "." + ALANLARMIFF[(int)ENUMAlanlarMIFF.YIL] + "=" + f.yil + " AND ";
                //sql += TABLOADMIFF + "." + ALANLARMIFF[(int)ENUMAlanlarMIFF.FORMDURUM] + "=" + f.formDurum + " AND ";

                //if (f.muhasebe > 0)
                //    sql += TABLOADMIFF + "." + ALANLARMIFF[(int)ENUMAlanlarMIFF.MUHASEBEKOD] + "=" + f.muhasebe + " AND ";
                //if (f.birim != "")
                //    sql += TABLOADMIFF + "." + ALANLARMIFF[(int)ENUMAlanlarMIFF.MUHASEBEKOD] + "=" + f.birim + " AND ";

                //sql += TABLOADMIFD + "." + ALANLARMIFD[(int)ENUMAlanlarMIFD.HESAPKOD] + " LIKE '" + ((OdemeEmriMIFDetay)f.detay[0]).hesapKod.Replace(".", "") + "%' AND ";

                //sql += "(" + TABLOADMIFF + "." + ALANLARMIFF[(int)ENUMAlanlarMIFF.ILGILIBELGENO] + " <> ''";
                //sql += " OR " + TABLOADMIFF + "." + ALANLARMIFF[(int)ENUMAlanlarMIFF.ILGILIBELGENO] + " IS NOT NULL))";
            }

            if (!detay)
                sql += " GROUP BY " + VeriErisim.TumAlanlar(TABLOADMIFF, ALANLARMIFF) + ",KURAD,MUHASEBEAD";

            string tarihAlanAd = "";
            string noALanAd = "";
            if (f.formDurum == (int)ENUMBelgeDurumu.YEVMIYE_OESGBYEGONDER)
            {
                tarihAlanAd = TABLOADMIFF + "." + ALANLARMIFF[(int)ENUMAlanlarMIFF.YEVMIYETARIH];
                noALanAd = TABLOADMIFF + "." + ALANLARMIFF[(int)ENUMAlanlarMIFF.YEVMIYENO];
            }
            else
            {
                tarihAlanAd = TABLOADMIFF + "." + ALANLARMIFF[(int)ENUMAlanlarMIFF.ISLEMTARIH];
                noALanAd = TABLOADMIFF + "." + ALANLARMIFF[(int)ENUMAlanlarMIFF.BELGENO];
            }

            if (f.sorguTarihSiraliMi)
                sql += " ORDER BY " + TABLOADMIFF + "." + ALANLARMIFF[(int)ENUMAlanlarMIFF.YIL] + "," + tarihAlanAd + "," + noALanAd;
            else if (f.sorguHesapKodSiraliMi)
                sql += " ORDER BY " + TABLOADMIFD + "." + ALANLARMIFD[(int)ENUMAlanlarMIFD.HESAPKOD] + "," + tarihAlanAd + "," + noALanAd;
            else if (f.sorguEkoSiraliMi)
            {
                if (onceAnaSonraTarih)
                {
                    sql += " ORDER BY " + TABLOADMIFF + "." + ALANLARMIFF[(int)ENUMAlanlarMIFF.YIL] + "," + VeriErisim.IslecBul("SUBSTRING") + "(" + TABLOADMIFD + "." + ALANLARMIFD[(int)ENUMAlanlarMIFD.HESAPKOD] + ",1,3), " + (altHesapKosulYok ? "" : (ALANLARMIFD[(int)ENUMAlanlarMIFD.ALTHESAPKOD] + ", ")) + tarihAlanAd + "," + noALanAd;
                }
                else
                    sql += " ORDER BY " + TABLOADMIFF + "." + ALANLARMIFF[(int)ENUMAlanlarMIFF.YIL] + "," + TABLOADMIFD + "." + ALANLARMIFD[(int)ENUMAlanlarMIFD.HESAPKOD] + "," + (altHesapKosulYok ? "" : (ALANLARMIFD[(int)ENUMAlanlarMIFD.ALTHESAPKOD] + ",")) + tarihAlanAd + "," + noALanAd;
            }
            else
                sql += " ORDER BY " + TABLOADMIFF + "." + ALANLARMIFF[(int)ENUMAlanlarMIFF.YIL] + "," + noALanAd;

            if (detay)
                sql += "," + ALANLARMIFD[(int)ENUMAlanlarMIFD.SIRA];

            veriErisim.sureAsimi = 500;
            return veriErisim.DataReaderDondur(sql);
        }

        private string IstatistikKosulOl(string AlanAd, int deger)
        {
            if (deger != 0)
            {
                if (deger == -1)
                    return "(" + AlanAd + " = 0 OR " + AlanAd + " IS NULL)";
                else
                    return "(" + AlanAd + " = " + VeriErisim.DegerYap(deger) + ")";
            }
            return "";
        }

        /// <summary>
        /// Belge durum bilgisini veren sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon.
        /// </summary>
        /// <param name="yil">Yıl.</param>
        /// <param name="belgeNo">Belge numarası.</param>
        /// <returns></returns>
        public IDataReader MIFKimlikOku(int yil, string belgeNo)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar("", ALANLARMIFF) + " FROM " + TABLOADMIFF + " WHERE YIL=" + yil + " AND BELGENO='" + belgeNo + "'";
            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Detay Tek belge okuma sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon..
        /// </summary>
        /// <param name="yil">Yıl.</param>
        /// <param name="belgeNo">Belge numarası.</param>
        /// <param name="detayHesapKod">Detay hesap kod.</param>
        /// <returns></returns>
        public IDataReader TekBelgeOkuDetay(int yil, string belgeNo, string detayHesapKod, int sira)
        {
            string sql = "";

            sql = "SELECT " + VeriErisim.TumAlanlar(TABLOADMIFD, ALANLARMIFD) + ",MUHIS.ADI," + "MUHHESAPPLANI." + ALANLARHESAP[(int)ENUMAlanlarHESAP.ACIKLAMA] + "," + "MUHKAPATMA." + ALANLARKAPANIS[(int)ENUMAlanlarKAPANIS.KBELGENO];// +" FROM " + TABLOADMIFD + "," + TABLOADHESAP;
            sql += ", (SELECT MIN(AD) FROM " + TABOrtak.DatabaseAdBul("HarcamaYonetimi.dbo.") + "HRCFIRMABILGISI where VERGINO=MUHMIF.ALTHESAPKOD)";
            sql += ", (SELECT MIN(AD + ' ' + SOYAD) FROM " + TABOrtak.DatabaseAdBul("Personel.dbo.") + "PERPersonel where TCKIMLIKNO=MUHMIF.ALTHESAPKOD)";
            if (Arac.prgBag)
                sql += ", PRGBAG.PRGKOD, PRGBAG.ALTPRGKOD, PRGBAG.FAALKOD, PRGBAG.ALTFAALKOD";
            else
                sql += ", 0, 0, 0, 0";

            sql += " FROM MUHMIF LEFT OUTER JOIN ";
            sql += " MUHKAPATMA ON MUHMIF.SIRA = MUHKAPATMA.SIRANO AND MUHMIF.BELGENO = MUHKAPATMA.BELGENO AND MUHMIF.YIL = MUHKAPATMA.YIL LEFT OUTER JOIN";
            sql += " MUHHESAPPLANI ON MUHMIF.HESAPKOD = MUHHESAPPLANI.HESAPKOD AND MUHMIF.YIL = MUHHESAPPLANI.YIL";
            sql += " LEFT OUTER JOIN MUHIS ON MUHMIF.ISKOD = MUHIS.KOD";
            if (Arac.prgBag)
                sql += " LEFT OUTER JOIN " + TABOrtak.DatabaseAdBul("ButceHazirlik.dbo.") + "PRGPEBILISKI PRGBAG ON MUHMIF.PEBKOD != '' AND PRGBAG.YIL = MUHMIF.YIL AND PRGBAG.PEBKOD = MUHMIF.PEBKOD"; //Prg bağ tablosu bağlantı kurulacak

            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOADMIFD, ALANLARMIFD[(int)ENUMAlanlarMIFD.YIL], "=", yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADMIFD, ALANLARMIFD[(int)ENUMAlanlarMIFD.BELGENO], "=", belgeNo));
            //sql = VeriErisim.KosulEkle(sql, "AND", TABLOADMIFD + "." + ALANLARMIFD[(int)ENUMAlanlarMIFD.YIL] + "=" + TABLOADHESAP + "." + ALANLARHESAP[(int)ENUMAlanlarHESAP.YIL]);
            //sql = VeriErisim.KosulEkle(sql, "AND", TABLOADMIFD + "." + ALANLARMIFD[(int)ENUMAlanlarMIFD.HESAPKOD] + "=" + TABLOADHESAP + "." + ALANLARHESAP[(int)ENUMAlanlarHESAP.HESAPKOD]);
            if (detayHesapKod != "")
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADMIFD, ALANLARMIFD[(int)ENUMAlanlarMIFD.HESAPKOD], "LIKE", detayHesapKod.Replace(".", "").Trim() + "%"));
            if (sira > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADMIFD, ALANLARMIFD[(int)ENUMAlanlarMIFD.SIRA], "=", sira));

            sql += " ORDER BY " + ALANLARMIFD[(int)ENUMAlanlarMIFD.SIRA];

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Belge durum degistirme sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon..
        /// </summary>
        /// <param name="form">Ödeme emri/MİF form bilgileri içeren nesne.</param>
        /// <param name="onayKaldir">Eğer <c>true</c> ise Onay kaldirma işlemini yapar.</param>
        /// <returns></returns>
        public Sonuc BelgeDurumDegistir(OdemeEmriMIFForm form, bool onayKaldir)
        {
            Sonuc sonuc = new Sonuc(false, "", "");
            string sql = "";

            sql = "UPDATE " + TABLOADMIFF;
            sql += " SET " + VeriErisim.KosulOl("", ALANLARMIFF[(int)ENUMAlanlarMIFF.FORMDURUM], "=", form.formDurum) + ",";
            sql += VeriErisim.KosulOl("", ALANLARMIFF[(int)ENUMAlanlarMIFF.DURUMTARIH], "=", new TNSDateTime(DateTime.Now)) + ",";
            sql += VeriErisim.KosulOl("", ALANLARMIFF[(int)ENUMAlanlarMIFF.ISLEMYAPAN], "=", form.islemYapan);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARMIFF[(int)ENUMAlanlarMIFF.YIL], "=", form.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARMIFF[(int)ENUMAlanlarMIFF.BELGENO], "=", form.belgeNo));

            sonuc = KomutCalistir(sql, 0);

            if (onayKaldir)
                form.formDurum = (int)ENUMBelgeDurumu.ONAYKALDIR;

            string islem = FormDurumAciklama(form.formDurum, form.belgeTur);
            KaydetTarihce(form.yil, (int)form.belgeTur, form.belgeNo, form.islemYapan, islem, form.islemYapanAd);

            return sonuc;
        }

        /// <summary>
        /// Belge türü degistirme sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon..
        /// </summary>
        /// <param name="form">Ödeme emri/MİF form bilgileri içeren nesne</param>
        /// <param name="yeniBelgeTur">Yeni belge tur.</param>
        /// <returns></returns>
        public Sonuc BelgeTurDegistir(OdemeEmriMIFForm form, int yeniBelgeTur)
        {
            Sonuc sonuc = new Sonuc(false, "", "");
            string sql = "";

            sql = "UPDATE " + TABLOADMIFF;
            sql += " SET " + VeriErisim.KosulOl("", ALANLARMIFF[(int)ENUMAlanlarMIFF.BELGETUR], "=", yeniBelgeTur);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARMIFF[(int)ENUMAlanlarMIFF.YIL], "=", form.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARMIFF[(int)ENUMAlanlarMIFF.BELGENO], "=", form.belgeNo));

            sonuc = KomutCalistir(sql, 1);

            if (sonuc.islemSonuc)
            {
                sql = "UPDATE " + TABLOADMIFD;
                sql += " SET " + VeriErisim.KosulOl("", ALANLARMIFD[(int)ENUMAlanlarMIFD.BELGETUR], "=", yeniBelgeTur);
                sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARMIFF[(int)ENUMAlanlarMIFD.YIL], "=", form.yil));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARMIFF[(int)ENUMAlanlarMIFD.BELGENO], "=", form.belgeNo));

                sonuc = KomutCalistir(sql, 0);
            }

            if (sonuc.islemSonuc)
            {
                sql = "UPDATE MUHBELGETARIHCE ";
                sql += " SET " + VeriErisim.KosulOl("", "BELGETUR", "=", yeniBelgeTur);
                sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", "YIL", "=", form.yil));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", "BELGENO", "=", form.belgeNo));

                sonuc = KomutCalistir(sql, 0);
            }

            string islem = FormDurumAciklama((int)ENUMBelgeDurumu.BELGETURDEGISTIR, (ENUMBelgeTuru)yeniBelgeTur);
            KaydetTarihce(form.yil, yeniBelgeTur, form.belgeNo, form.islemYapan, islem, form.islemYapanAd);

            return sonuc;
        }

        /// <summary>
        /// En buyuk yevmiye no veren sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon..
        /// </summary>
        /// <param name="yil">Yıl.</param>
        /// <returns></returns>
        public int EnBuyukYevmiyeNo(int yil, int muhasebe)
        {
            string sql = "SELECT MAX(" + ALANLARMIFF[(int)ENUMAlanlarMIFF.YEVMIYENO] + ") FROM " + TABLOADMIFF;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARMIFF[(int)ENUMAlanlarMIFF.YIL], "=", yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARMIFF[(int)ENUMAlanlarMIFF.MUHASEBEKOD], "=", muhasebe));

            return veriErisim.TekKolonSQLInt(sql) + 1;
        }

        /// <summary>
        /// Verilmek istenilen yevmiye no kayıtlı mı sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon..
        /// </summary>
        /// <param name="yil">Yıl.</param>
        /// <param name="yevmiyeNo">Yevmiye no</param>
        /// <returns></returns>
        public bool YevmiyeNoVarmi(int yil, int muhasebe, int yevmiyeNo)
        {
            string sql = "SELECT COUNT(*) FROM " + TABLOADMIFF;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARMIFF[(int)ENUMAlanlarMIFF.YIL], "=", yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARMIFF[(int)ENUMAlanlarMIFF.MUHASEBEKOD], "=", muhasebe));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARMIFF[(int)ENUMAlanlarMIFF.YEVMIYENO], "=", yevmiyeNo));

            return veriErisim.TekKolonSQLInt(sql) > 0;
        }

        /// <summary>
        /// Yevmiye no verme sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon..
        /// </summary>
        /// <param name="form">Ödeme emri/MİF form bilgileri içeren nesne</param>
        /// <param name="numaraVer">if set to <c>true</c> [numara ver].</param>
        /// <returns></returns>
        public Sonuc YevmiyeNoVer(OdemeEmriMIFForm form, bool numaraVer)
        {
            Sonuc sonuc = new Sonuc(false, "", "");
            string sql = "";

            if (numaraVer)
            {
                sql = "UPDATE " + TABLOADMIFF;
                sql += " SET " + VeriErisim.KosulOl("", ALANLARMIFF[(int)ENUMAlanlarMIFF.YEVMIYENO], "=", form.YEVYevNo) + ",";
                sql += VeriErisim.KosulOl("", ALANLARMIFF[(int)ENUMAlanlarMIFF.YEVMIYETARIH], "=", form.YEVYevTarih);
                sql += ", " + VeriErisim.KosulOl("", ALANLARMIFF[(int)ENUMAlanlarMIFF.FORMDURUM], "=", (int)ENUMBelgeDurumu.YEVMIYE_OESGBYEGONDER);
                sql += ", " + VeriErisim.KosulOl("", ALANLARMIFF[(int)ENUMAlanlarMIFF.ISLEMYAPAN], "=", form.islemYapan);
                sql += ", " + VeriErisim.KosulOl("", ALANLARMIFF[(int)ENUMAlanlarMIFF.DURUMTARIH], "=", new TNSDateTime(DateTime.Now));
                sql += ", " + VeriErisim.KosulOl("", ALANLARMIFF[(int)ENUMAlanlarMIFF.YEVMIYEACIKLAMA], "=", form.yevmiyeAciklama);
                sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARMIFF[(int)ENUMAlanlarMIFF.YIL], "=", form.yil));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARMIFF[(int)ENUMAlanlarMIFF.BELGENO], "=", form.belgeNo));
            }
            else
            {
                sql = "UPDATE " + TABLOADMIFF;
                sql += " SET " + VeriErisim.KosulOl("", ALANLARMIFF[(int)ENUMAlanlarMIFF.YEVMIYENO], "=", 0) + ",";
                sql += ALANLARMIFF[(int)ENUMAlanlarMIFF.YEVMIYETARIH] + "=null";
                sql += ", " + VeriErisim.KosulOl("", ALANLARMIFF[(int)ENUMAlanlarMIFF.FORMDURUM], "=", (int)ENUMBelgeDurumu.DEGISTIRILDI);
                sql += ", " + VeriErisim.KosulOl("", ALANLARMIFF[(int)ENUMAlanlarMIFF.ISLEMYAPAN], "=", form.islemYapan);
                sql += ", " + VeriErisim.KosulOl("", ALANLARMIFF[(int)ENUMAlanlarMIFF.DURUMTARIH], "=", new TNSDateTime(DateTime.Now));
                sql += ", " + VeriErisim.KosulOl("", ALANLARMIFF[(int)ENUMAlanlarMIFF.YEVMIYEACIKLAMA], "=", form.yevmiyeAciklama);
                sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARMIFF[(int)ENUMAlanlarMIFF.YIL], "=", form.yil));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARMIFF[(int)ENUMAlanlarMIFF.BELGENO], "=", form.belgeNo));
            }
            sonuc = KomutCalistir(sql, 0);

            string islem = FormDurumAciklama(form.formDurum, form.belgeTur);
            KaydetTarihce(form.yil, (int)form.belgeTur, form.belgeNo, form.islemYapan, islem + "(" + form.YEVYevNo + ")", form.islemYapanAd);

            return sonuc;
        }

        /// <summary>
        /// Çek no verme sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon..
        /// </summary>
        /// <param name="form">Ödeme emri/MİF form bilgileri içeren nesne</param>
        /// <param name="numaraVer">if set to <c>true</c> [numara ver].</param>
        /// <returns></returns>
        public Sonuc CekNoVer(OdemeEmriMIFForm form, bool numaraVer)
        {
            Sonuc sonuc = new Sonuc(false, "", "");
            string sql = "";

            if (numaraVer)
            {
                sql = "UPDATE " + TABLOADMIFF;
                sql += " SET " + VeriErisim.KosulOl("", ALANLARMIFF[(int)ENUMAlanlarMIFF.CEKEMRINO], "=", form.YEVCekNo) + ",";
                sql += VeriErisim.KosulOl("", ALANLARMIFF[(int)ENUMAlanlarMIFF.ETARIH], "=", new TNSDateTime(DateTime.Now));
                sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARMIFF[(int)ENUMAlanlarMIFF.YIL], "=", form.yil));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARMIFF[(int)ENUMAlanlarMIFF.BELGENO], "=", form.belgeNo));
            }
            else
            {
                sql = "UPDATE " + TABLOADMIFF;
                sql += " SET " + VeriErisim.KosulOl("", ALANLARMIFF[(int)ENUMAlanlarMIFF.CEKEMRINO], "=", "") + ",";
                sql += ALANLARMIFF[(int)ENUMAlanlarMIFF.ETARIH] + "=null";
                sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARMIFF[(int)ENUMAlanlarMIFF.YIL], "=", form.yil));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARMIFF[(int)ENUMAlanlarMIFF.BELGENO], "=", form.belgeNo));
            }
            sonuc = KomutCalistir(sql, 0);

            string islem = FormDurumAciklama(form.formDurum, form.belgeTur);
            KaydetTarihce(form.yil, (int)form.belgeTur, form.belgeNo, form.islemYapan, islem + "(" + form.YEVCekNo + ")", form.islemYapanAd);

            return sonuc;
        }

        /// <summary>
        /// En büyük satir no verme sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon..
        /// </summary>
        /// <param name="yil">Yıl.</param>
        /// <param name="belgeNo">Belge numarası.</param>
        /// <returns></returns>
        public int EnBuyukSatirNo(int yil, string belgeNo)
        {
            string sql = "";
            int siraNo = 0;

            sql = "SELECT MAX(" + ALANLARMIFD[(int)ENUMAlanlarMIFD.SIRA] + ") FROM " + TABLOADMIFD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARMIFD[(int)ENUMAlanlarMIFD.YIL], "=", yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARMIFD[(int)ENUMAlanlarMIFD.BELGENO], "=", belgeNo));

            IDataReader r = veriErisim.DataReaderDondur(sql);
            if (r.Read())
                siraNo = r.GetInt32(0);

            r.Close();

            return (siraNo + 1);
        }

        /// <summary>
        /// Belge turü verme sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon..
        /// </summary>
        /// <param name="yil">Yıl.</param>
        /// <param name="belgeNo">Belge numarası.</param>
        /// <returns></returns>
        public int BelgeTurVer(int yil, string belgeNo)
        {
            string sql = "";
            int belgeTur = 0;

            sql = "SELECT " + ALANLARMIFF[(int)ENUMAlanlarMIFF.BELGETUR] + " FROM " + TABLOADMIFF;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARMIFF[(int)ENUMAlanlarMIFF.YIL], "=", yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARMIFF[(int)ENUMAlanlarMIFF.BELGENO], "=", belgeNo));

            IDataReader r = veriErisim.DataReaderDondur(sql);
            if (r.Read())
                belgeTur = r.GetInt32(0);

            r.Close();

            return belgeTur;
        }

        /// <summary>
        /// Odeme emri / MIF toplamı veren sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon..
        /// </summary>
        /// <param name="kr">Kriter bilgileri.</param>
        /// <param name="avans">true ise sadece avanslar döndürülür</param>
        /// <returns></returns>
        public IDataReader OdemeEmriMIFToplam(OdenekDurumKriter kr, bool avans)
        {
            string sql = "SELECT ";
            string alanlar = ALANLARMIFF[(int)ENUMAlanlarMIFF.FORMDURUM];

            if (kr.detay == ENUMOdenekDurumDetay.MUHASEBE)
            {
                if (alanlar != "") alanlar += ",";
                alanlar += ALANLARMIFF[(int)ENUMAlanlarMIFF.MUHASEBEKOD];
            }
            else if (kr.detay == ENUMOdenekDurumDetay.BIRIM)
            {
                if (alanlar != "") alanlar += ",";
                alanlar += ALANLARMIFF[(int)ENUMAlanlarMIFF.MUHASEBEKOD];
                if (ANABIRIM)
                    alanlar += "," + ALANLARMIFF[(int)ENUMAlanlarMIFF.BIRIMKOD];
                else
                    alanlar += "," + ALANLARMIFD[(int)ENUMAlanlarMIFD.DETAYBIRIM];
            }
            else if (kr.detay == ENUMOdenekDurumDetay.EKO1BASAMAK)
            {
                if (alanlar != "") alanlar += ",";
                alanlar += ALANLARMIFF[(int)ENUMAlanlarMIFF.MUHASEBEKOD];
                alanlar += "," + ALANLARMIFD[(int)ENUMAlanlarMIFD.DETAYBIRIM];
                alanlar += "," + VeriErisim.IslecBul("SUBSTRING") + "(" + ALANLARMIFD[(int)ENUMAlanlarMIFD.EKOKOD] + ",1,2)";
            }

            if (!kr.konsolide)
            {
                alanlar += "," + TABLOADMIFD + "." + ALANLARMIFD[(int)ENUMAlanlarMIFD.KURKOD];
                alanlar += "," + TABLOADMIFD + "." + ALANLARMIFD[(int)ENUMAlanlarMIFD.FONKOD];
                alanlar += "," + TABLOADMIFD + "." + ALANLARMIFD[(int)ENUMAlanlarMIFD.FINKOD];

                alanlar += "," + BKD.Arac.EkoAlanOl(kr.yil, "", ALANLARMIFD[(int)ENUMAlanlarMIFD.EKOKOD], kr.ekoKodKirilim, kr.eko23, VeriErisim.IslecBul("SUBSTRING"));

                if (kr.faaliyetGoster)
                    alanlar += "," + ALANLARMIFD[(int)ENUMAlanlarMIFD.PEBKOD];
            }
            sql += alanlar;
            if (alanlar != "") sql += ",";

            sql += "SUM(" + ALANLARMIFD[(int)ENUMAlanlarMIFD.BORC] + "-" + ALANLARMIFD[(int)ENUMAlanlarMIFD.ALACAK] + ")";

            sql += " FROM " + TABLOADMIFF + "," + TABLOADMIFD + " WHERE ";
            sql += VeriErisim.KosulOl(TABLOADMIFF, ALANLARMIFF[(int)ENUMAlanlarMIFF.YIL], "=", kr.yil);
            sql += " AND " + VeriErisim.KosulOl(TABLOADMIFD, ALANLARMIFD[(int)ENUMAlanlarMIFD.FONKOD], " <> ", "");

            //Sadece Gider Hesap Kodunu sorgula Ödenek Durum için sadece harcama kalemleri gerekli
            if (avans)
            {
                sql += " AND " + VeriErisim.KosulOl(TABLOADMIFD, ALANLARMIFD[(int)ENUMAlanlarMIFD.HESAPKOD], "LIKE", "16%");
                sql += " AND " + VeriErisim.KosulOl(TABLOADMIFD, ALANLARMIFD[(int)ENUMAlanlarMIFD.HESAPKOD], "NOT LIKE", "162%");
            }
            else
                sql += " AND " + VeriErisim.KosulOl(TABLOADMIFD, ALANLARMIFD[(int)ENUMAlanlarMIFD.HESAPKOD], "LIKE", "830%");

            if (kr.muhasebe > 0)
                sql += " AND " + VeriErisim.KosulOl(TABLOADMIFF, ALANLARMIFF[(int)ENUMAlanlarMIFF.MUHASEBEKOD], "=", kr.muhasebe);

            if (kr.ay > 0)
                sql += " AND " + VeriErisim.KosulOl("", "DATEPART(month, MUHMIFFORM.ISLEMTARIH)", "<=", kr.ay);

            if (kr.birim != "")
            {
                if (ANABIRIM)
                    sql += " AND " + VeriErisim.KosulOl(TABLOADMIFF, ALANLARMIFF[(int)ENUMAlanlarMIFF.BIRIMKOD], "=", kr.birim);
                else
                    sql += " AND " + VeriErisim.KosulOl(TABLOADMIFD, ALANLARMIFD[(int)ENUMAlanlarMIFD.DETAYBIRIM], "=", kr.birim);
            }

            sql = TABOdenekHarcamaSorgu.TertipKosulOl(sql, kr.tertip, TABLOADMIFD, ALANLARMIFD[(int)ENUMAlanlarMIFD.KURKOD], ALANLARMIFD[(int)ENUMAlanlarMIFD.FONKOD], ALANLARMIFD[(int)ENUMAlanlarMIFD.FINKOD], ALANLARMIFD[(int)ENUMAlanlarMIFD.EKOKOD], ALANLARMIFD[(int)ENUMAlanlarMIFD.PEBKOD]);

            sql += " AND " + VeriErisim.KosulOl(TABLOADMIFF, ALANLARMIFF[(int)ENUMAlanlarMIFF.FORMDURUM], "<>", (int)ENUMBelgeDurumu.IPTAL);
            sql = VeriErisim.KosulEkle(sql, "AND", TABLOADMIFF + "." + ALANLARMIFF[(int)ENUMAlanlarMIFF.YIL] + "=" + TABLOADMIFD + "." + ALANLARMIFD[(int)ENUMAlanlarMIFD.YIL]);
            sql = VeriErisim.KosulEkle(sql, "AND", TABLOADMIFF + "." + ALANLARMIFF[(int)ENUMAlanlarMIFF.BELGENO] + "=" + TABLOADMIFD + "." + ALANLARMIFD[(int)ENUMAlanlarMIFD.BELGENO]);
            sql = VeriErisim.KosulEkle(sql, "AND", TABLOADMIFF + "." + ALANLARMIFF[(int)ENUMAlanlarMIFF.FISTUR] + "<" + 2);

            if (alanlar != "")
            {
                sql += " GROUP BY " + alanlar;
                sql += " ORDER BY " + alanlar;
            }

            IDataReader r = veriErisim.DataReaderDondur(sql);

            return r;
        }

        void KaydetTarihce(int yil, int belgeTur, string belgeNo, string islemYapan, string islem, string islemYapanAd)
        {
            string sql = "";

            sql = "INSERT INTO MUHBELGETARIHCE (YIL,BELGETUR,BELGENO,ISLEMYAPAN,ISLEM,ISLEMTARIH,ISLEMYAPANAD) VALUES (";
            sql = sql + VeriErisim.DegerYap(yil) + ", ";
            sql = sql + VeriErisim.DegerYap(belgeTur) + ", ";
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

        /// <summary>
        /// Tertip listeleme sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon..
        /// </summary>
        /// <param name="yil">Yıl.</param>
        /// <param name="kur">Kurumsal kod.</param>
        /// <param name="fon">Fonksiyonel kod.</param>
        /// <param name="fin">Finans kodu.</param>
        /// <param name="eko">Ekonomik kod.</param>
        /// <returns></returns>
        public IDataReader TertipListele(int yil, string kur, string fon, string fin, string eko)
        {
            string sql = "";
            sql = "SELECT " + VeriErisim.TumAlanlar(TABLOADTERTIP, ALANLARTERTIP) + " FROM " + TABLOADTERTIP;

            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOADTERTIP, ALANLARTERTIP[(int)ENUMAlanlarTERTIP.YIL], "=", yil));

            if (kur != "")
            {
                string[] birimler = kur.Split(',');
                string birimKosul = "";
                for (int i = 0; i < birimler.Length; i++)
                    birimKosul = VeriErisim.KosulEkle(birimKosul, "OR", VeriErisim.KosulOl(TABLOADTERTIP, ALANLARTERTIP[(int)ENUMAlanlarTERTIP.KURKOD], "LIKE", birimler[i] + "%"));

                if (birimKosul != "")
                    sql += " AND (" + birimKosul + ")";
            }
            if (fon != "")
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADTERTIP, ALANLARTERTIP[(int)ENUMAlanlarTERTIP.FONKOD], "LIKE", fon + "%"));
            if (fin != "")
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADTERTIP, ALANLARTERTIP[(int)ENUMAlanlarTERTIP.FINKOD], "LIKE", fin + "%"));
            if (eko != "")
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADTERTIP, ALANLARTERTIP[(int)ENUMAlanlarTERTIP.EKOKOD], "LIKE", eko + "%"));

            sql += "ORDER BY 1,2,3,4,5";

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Ekonomik 4.düzey Tertip listeleme sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon..
        /// </summary>
        /// <param name="yil">Yıl.</param>
        /// <param name="kur">Kurumsal kod.</param>
        /// <param name="fon">Fonksiyonel kod.</param>
        /// <param name="fin">Finans kodu.</param>
        /// <param name="eko">Ekonomik kod.</param>
        /// <returns></returns>
        public IDataReader TertipListeleEko4(int yil, string kur, string fon, string fin, string eko)
        {
            BKD.Tertip tUzBul = new BKD.Tertip(yil, BKD.ENUMTabloKod.TABPEBOdenek);
            int eko4Uz = tUzBul.KodUzunlukBul(3, 2, true);

            string sql = "";
            sql = "SELECT ABSBUTCEGIDER.YIL, ABSBUTCEGIDER.KURKOD, ABSBUTCEGIDER.FONKOD, ABSBUTCEGIDER.FINKOD, ABSEKO.EKOKOD, ABSEKO.EKOAD";
            sql += " FROM ABSEKO ,ABSBUTCEGIDER";
            sql += " WHERE ABSEKO.YIL = ABSBUTCEGIDER.YIL";
            sql += " AND " + VeriErisim.IslecBul("SUBSTRING") + "(ABSEKO.EKOKOD, 1, " + eko4Uz + ") = " + VeriErisim.IslecBul("SUBSTRING") + "(ABSBUTCEGIDER.EKOKOD, 1, " + eko4Uz + ")";
            //sql += " AND " + VeriErisim.IslecBul("LEN") + "(ABSEKO.EKOKOD) = 6";//Tacik ödenek listelemiyor eko kod yapısı farklı

            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADTERTIP, ALANLARTERTIP[(int)ENUMAlanlarTERTIP.YIL], "=", yil));

            if (kur != "")
            {
                string[] birimler = kur.Split(',');
                string birimKosul = "";
                for (int i = 0; i < birimler.Length; i++)
                    birimKosul = VeriErisim.KosulEkle(birimKosul, "OR", VeriErisim.KosulOl(TABLOADTERTIP, ALANLARTERTIP[(int)ENUMAlanlarTERTIP.KURKOD], "LIKE", birimler[i] + "%"));

                if (birimKosul != "")
                    sql += " AND (" + birimKosul + ")";
            }
            if (fon != "")
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADTERTIP, ALANLARTERTIP[(int)ENUMAlanlarTERTIP.FONKOD], "LIKE", fon + "%"));
            if (fin != "")
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADTERTIP, ALANLARTERTIP[(int)ENUMAlanlarTERTIP.FINKOD], "LIKE", fin + "%"));
            if (eko != "")
            {
                string[] birimler = eko.Split(';');
                string birimKosul = "";
                for (int i = 0; i < birimler.Length; i++)
                    if (birimler[i].Trim() != "")
                        birimKosul = VeriErisim.KosulEkle(birimKosul, "OR", VeriErisim.KosulOl("ABSEKO", "EKOKOD", "LIKE", birimler[i] + "%"));

                if (birimKosul != "")
                    sql += " AND (" + birimKosul + ")";
            }

            sql += "ORDER BY 1,2,3,4,5";

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Ekonomik 4.düzey Tertip listeleme sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon.
        /// </summary>
        /// <param name="yil">Yıl.</param>
        /// <param name="kur">Kurumsal kod.</param>
        /// <param name="fon">Fonksiyonel kod.</param>
        /// <param name="fin">Finans kodu.</param>
        /// <param name="eko">Ekonomik kod.</param>
        /// <param name="peb">Performans bütçe kodu.</param>
        /// <returns></returns>
        public IDataReader TertipListeleEko4Peb(int yil, string kur, string fon, string fin, string eko, string peb)
        {
            BKD.Tertip tUzBul = new BKD.Tertip(yil, BKD.ENUMTabloKod.TABPEBOdenek);
            int eko4Uz = tUzBul.KodUzunlukBul(3, 2, true);

            string sql = "";

            sql = "SELECT     ABSBUTCEGIDER.YIL, ABSBUTCEGIDER.KURKOD, ABSBUTCEGIDER.FONKOD, ABSBUTCEGIDER.FINKOD, ABSEKO.EKOKOD, ABSEKO.EKOAD, " +
                      " MPGMASRAFPLANITERTIP.PEBFONKOD " +
                  " FROM         ABSEKO INNER JOIN " +
                      " ABSBUTCEGIDER ON ABSEKO.YIL = ABSBUTCEGIDER.YIL AND SUBSTRING(ABSEKO.EKOKOD, 1, " + eko4Uz + ") = SUBSTRING(ABSBUTCEGIDER.EKOKOD, 1, " + eko4Uz + ") INNER JOIN " +
                      " MPGMASRAFPLANITERTIP ON ABSBUTCEGIDER.YIL = MPGMASRAFPLANITERTIP.YIL AND ABSBUTCEGIDER.KURKOD = MPGMASRAFPLANITERTIP.KURKOD AND  " +
                      " ABSBUTCEGIDER.FONKOD = MPGMASRAFPLANITERTIP.FONKOD AND ABSBUTCEGIDER.FINKOD = MPGMASRAFPLANITERTIP.FINKOD " +
                      " WHERE  (LEN(ABSEKO.EKOKOD) = 6) AND   (MPGMASRAFPLANITERTIP.DURUM = 5) ";


            //sql = "SELECT     ABSBUTCEGIDER.YIL, ABSBUTCEGIDER.KURKOD, ABSBUTCEGIDER.FONKOD, ABSBUTCEGIDER.FINKOD, ABSEKO.EKOKOD, ABSEKO.EKOAD, PEBODENEK.PEBFONKOD ";
            //sql += " FROM ABSEKO INNER JOIN ABSBUTCEGIDER ON ABSEKO.YIL = ABSBUTCEGIDER.YIL AND SUBSTRING(ABSEKO.EKOKOD, 1, 3) = SUBSTRING(ABSBUTCEGIDER.EKOKOD, 1, 3) ";
            //sql += " INNER JOIN PEBODENEK ON ABSBUTCEGIDER.YIL = PEBODENEK.YIL AND ABSBUTCEGIDER.KURKOD = PEBODENEK.KURKOD AND ABSBUTCEGIDER.FONKOD = PEBODENEK.FONKOD AND ABSBUTCEGIDER.FINKOD = PEBODENEK.FINKOD ";

            //sql += " WHERE     (LEN(ABSEKO.EKOKOD) = 6) AND (PEBODENEK.ASAMA = 3) ";


            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADTERTIP, ALANLARTERTIP[(int)ENUMAlanlarTERTIP.YIL], "=", yil));

            if (kur != "")
            {
                string[] birimler = kur.Split(',');
                string birimKosul = "";
                for (int i = 0; i < birimler.Length; i++)
                    birimKosul = VeriErisim.KosulEkle(birimKosul, "OR", VeriErisim.KosulOl(TABLOADTERTIP, ALANLARTERTIP[(int)ENUMAlanlarTERTIP.KURKOD], "LIKE", birimler[i] + "%"));

                if (birimKosul != "")
                    sql += " AND (" + birimKosul + ")";
            }
            if (fon != "")
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADTERTIP, ALANLARTERTIP[(int)ENUMAlanlarTERTIP.FONKOD], "LIKE", fon + "%"));
            if (fin != "")
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADTERTIP, ALANLARTERTIP[(int)ENUMAlanlarTERTIP.FINKOD], "LIKE", fin + "%"));
            if (eko != "")
            {
                string[] birimler = eko.Split(';');
                string birimKosul = "";
                string birimKosul2 = "";
                for (int i = 0; i < birimler.Length; i++)
                    if (birimler[i].Trim() != "")
                    {
                        birimKosul = VeriErisim.KosulEkle(birimKosul, "OR", VeriErisim.KosulOl("ABSEKO", "EKOKOD", "LIKE", birimler[i] + "%"));
                        birimKosul2 = VeriErisim.KosulEkle(birimKosul2, "OR", VeriErisim.KosulOl("MPGMASRAFPLANITERTIP", "EKOKOD", "LIKE", birimler[i] + "%"));
                    }
                if (birimKosul != "")
                    sql += " AND (" + birimKosul + ")" + " AND (" + birimKosul2 + ")";
            }
            if (peb != "")
                sql += " AND MPGMASRAFPLANITERTIP.PEBFONKOD=" + peb;


            sql += " GROUP BY ABSBUTCEGIDER.YIL, ABSBUTCEGIDER.KURKOD, ABSBUTCEGIDER.FONKOD, ABSBUTCEGIDER.FINKOD, ABSEKO.EKOKOD, ABSEKO.EKOAD, MPGMASRAFPLANITERTIP.PEBFONKOD " +
            " ORDER BY ABSBUTCEGIDER.YIL, ABSBUTCEGIDER.KURKOD, ABSBUTCEGIDER.FONKOD, ABSBUTCEGIDER.FINKOD, ABSEKO.EKOKOD";
            //sql += " GROUP BY ABSBUTCEGIDER.YIL, ABSBUTCEGIDER.KURKOD, ABSBUTCEGIDER.FONKOD, ABSBUTCEGIDER.FINKOD, ABSEKO.EKOKOD, ABSEKO.EKOAD, PEBODENEK.PEBFONKOD";
            //sql += " ORDER BY 1,2,3,4,5";

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Kayıt edilmek istenilen fatura bilgisi daha önce kayıt edilmiş mi sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon.
        /// </summary>
        /// <param name="fYil">Yıl.</param>
        /// <param name="fNo">Fiş no.</param>
        /// <param name="fTarih">Fiş tarihi.</param>
        /// <param name="muhasebe">Muhasebe kodu.</param>
        /// <param name="fBelgeNo">Belge no.</param>
        /// <param name="belgeTur">Belge tur.</param>
        /// <returns></returns>
        public IDataReader FaturaKayitlimi(int fYil, string fNo, TNSDateTime fTarih, int muhasebe, string fBelgeNo, int belgeTur, string kimlikNo)
        {
            string sql = "";
            sql = "SELECT BELGENO FROM MUHMIFFORM";
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOADMIFF, ALANLARMIFF[(int)ENUMAlanlarMIFF.YIL], "=", fYil));
            //sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADMIFF, ALANLARMIFF[(int)ENUMAlanlarMIFF.BELGETUR], "=", belgeTur));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADMIFF, ALANLARMIFF[(int)ENUMAlanlarMIFF.MUHASEBEKOD], "=", muhasebe));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADMIFF, ALANLARMIFF[(int)ENUMAlanlarMIFF.IKIMLIKNO], "=", kimlikNo.Trim()));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADMIFF, ALANLARMIFF[(int)ENUMAlanlarMIFF.ONO], "=", fNo.Trim()));

            //Cumhurbaşkanlığı fatura kontrolünde tarih olmasın istiyor 08.12.2021 Melih 
            //sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADMIFF, ALANLARMIFF[(int)ENUMAlanlarMIFF.OTARIH], "=", fTarih));

            //ilgili farklı yazıldığında kayıtlı fatura bulunamıyordu
            //sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADMIFF, ALANLARMIFF[(int)ENUMAlanlarMIFF.IADSOYAD], "=", fIlgili.Trim()));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADMIFF, ALANLARMIFF[(int)ENUMAlanlarMIFF.FORMDURUM], "<>", (int)ENUMBelgeDurumu.IPTAL));
            if (fBelgeNo.Trim() != "")
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADMIFF, ALANLARMIFF[(int)ENUMAlanlarMIFF.BELGENO], "<>", fBelgeNo.Trim()));

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Avans listele sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon..
        /// </summary>
        /// <param name="yil">Yıl.</param>
        /// <param name="muhasebe">Muhasebe kodu.</param>
        /// <param name="birim">Harcama birim kodu.</param>
        /// <param name="kimlikNo">Kimlik no.</param>
        /// <param name="adSoyad">Ad soyad.</param>
        /// <returns></returns>
        public IDataReader AvansListele(int yil, int muhasebe, string birim, string kimlikNo, string adSoyad)
        {
            string sql = "";
            string kosul = "";
            sql = "SELECT MUHMIFFORM.YIL, MUHMIFFORM.MUHASEBEKOD, MUHMIFFORM.BIRIMKOD, MUHMIFFORM.IKIMLIKNO, MUHMIFFORM.IADSOYAD, ";
            sql += " SUM(MUHMIF.BORC), SUM(MUHMIF.ALACAK), MUHMUHASEBE.MUHASEBEAD ";
            sql += " FROM MUHMIFFORM INNER JOIN";
            sql += " MUHMIF ON MUHMIFFORM.YIL = MUHMIF.YIL AND MUHMIFFORM.BELGENO = MUHMIF.BELGENO LEFT OUTER JOIN";
            sql += " MUHMUHASEBE ON MUHMIFFORM.MUHASEBEKOD = MUHMUHASEBE.MUHASEBEKOD ";
            sql += " WHERE (MUHMIF.HESAPKOD LIKE '16%') AND (MUHMIF.HESAPKOD NOT LIKE '162%') AND (MUHMIFFORM.FORMDURUM <> 9) ";
            sql += " GROUP BY MUHMIFFORM.YIL, MUHMIFFORM.MUHASEBEKOD, MUHMIFFORM.BIRIMKOD, MUHMIFFORM.IKIMLIKNO, MUHMIFFORM.IADSOYAD,MUHMUHASEBE.MUHASEBEAD";

            if (yil > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADMIFF, ALANLARMIFF[(int)ENUMAlanlarMIFF.YIL], "=", yil));
            if (muhasebe > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADMIFF, ALANLARMIFF[(int)ENUMAlanlarMIFF.MUHASEBEKOD], "=", muhasebe));
            if (birim != "")
            {
                if (ANABIRIM)
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADMIFF, ALANLARMIFF[(int)ENUMAlanlarMIFF.BIRIMKOD], "=", birim));
                else
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADMIFD, ALANLARMIFD[(int)ENUMAlanlarMIFD.DETAYBIRIM], "=", birim));
            }
            if (kimlikNo != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADMIFF, ALANLARMIFF[(int)ENUMAlanlarMIFF.IKIMLIKNO], "LIKE", "%" + kimlikNo + "%"));
            if (adSoyad != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADMIFF, ALANLARMIFF[(int)ENUMAlanlarMIFF.IADSOYAD], "LIKE", "%" + adSoyad + "%"));

            if (kosul != "")
                sql = sql + " HAVING " + kosul;

            sql += "ORDER BY 1,2,3,4";
            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Kesin hesap bilgi listele sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon..
        /// </summary>
        /// <param name="f">Ödeme emri/MİF form bilgileri içeren nesne</param>
        /// <param name="rakamVer">Eğer <c>true</c> ise rakam ver.</param>
        /// <param name="sadeceHesapKod">Eğer <c>true</c> ise sadece gider hesap kodu döndür.</param>
        /// <param name="gelirHesapKod">Eğer <c>true</c> ise sadece gelir hesap kodu döndür.</param>
        /// <param name="tur">Tür.</param>
        /// <returns></returns>
        public IDataReader KesinHesapBilgiListele(OdemeEmriMIFForm f, bool rakamVer, bool sadeceHesapKod, bool gelirHesapKod, int tur)
        {
            BKD.Tertip tUzBul = new BKD.Tertip(f.yil, BKD.ENUMTabloKod.TABPEBOdenek);
            int eko4Uz = tUzBul.KodUzunlukBul(3, 2, true);

            string sql = "";

            if (tur == (int)ENUMKesinHesapButce.KURUMSAL)
                sql = "SELECT " + VeriErisim.IslecBul("SUBSTRING") + "(HESAPKOD,1,3),MUHMIF.KURKOD,SUM(BORC),SUM(ALACAK)";
            else if (tur == (int)ENUMKesinHesapButce.FONKSIYONEL)
                sql = "SELECT " + VeriErisim.IslecBul("SUBSTRING") + "(HESAPKOD,1,3),FONKOD,SUM(BORC),SUM(ALACAK)";
            else if (tur == (int)ENUMKesinHesapButce.FINANS)
                sql = "SELECT " + VeriErisim.IslecBul("SUBSTRING") + "(HESAPKOD,1,3),FINKOD,SUM(BORC),SUM(ALACAK)";
            else if (tur == (int)ENUMKesinHesapButce.EKONOMIK)
                sql = "SELECT " + VeriErisim.IslecBul("SUBSTRING") + "(HESAPKOD,1,3),EKOKOD,SUM(BORC),SUM(ALACAK)";
            else if (tur == (int)ENUMKesinHesapButce.EKONOMIK2)
                sql = "SELECT '', " + VeriErisim.IslecBul("SUBSTRING") + "(EKOKOD,1," + eko4Uz + "), SUM(BORC),SUM(ALACAK)";
            else if (tur == (int)ENUMKesinHesapButce.FINEKO2PEB)
                sql = "SELECT  '', '--' + FINKOD + '-'+ " + VeriErisim.IslecBul("SUBSTRING") + "(EKOKOD,1," + eko4Uz + ")+'-'+PEBKOD, SUM(BORC), SUM(ALACAK)";
            else if (tur == (int)ENUMKesinHesapButce.GELIREKONOMIK || tur == (int)ENUMKesinHesapButce.GIDEREKONOMIK)
                sql = "SELECT " + VeriErisim.IslecBul("SUBSTRING") + "(HESAPKOD,1,11)," + VeriErisim.IslecBul("SUBSTRING") + "(HESAPKOD,1,11) BOSLUK,SUM(BORC),SUM(ALACAK)";//HESAPKOD 2 kere olmasının nedeni çağıran fonksiyon ortak olduğu için 2 bir alan istiyor
            else if (tur == (int)ENUMKesinHesapButce.BILONCO)
                sql = "SELECT " + VeriErisim.IslecBul("SUBSTRING") + "(HESAPKOD,1,3)," + VeriErisim.IslecBul("SUBSTRING") + "(HESAPKOD,1,3) BOSLUK,SUM(BORC),SUM(ALACAK)";//HESAPKOD 2 kere olmasının nedeni çağıran fonksiyon ortak olduğu için 2 bir alan istiyor
            else if (rakamVer)
                sql = "SELECT SUM(BORC),SUM(ALACAK)";
            else if (sadeceHesapKod)
            {
                if (gelirHesapKod)
                    sql = "SELECT " + VeriErisim.IslecBul("SUBSTRING") + "(HESAPKOD,1,11)";
                else
                    sql = "SELECT HESAPKOD";
            }
            else
                sql = "SELECT MUHMIF.KURKOD,FONKOD,FINKOD," + VeriErisim.IslecBul("SUBSTRING") + "(EKOKOD,1," + eko4Uz + ")";

            sql += " FROM MUHMIF INNER JOIN";
            sql += " MUHMIFFORM ON MUHMIF.YIL = MUHMIFFORM.YIL AND MUHMIF.BELGENO = MUHMIFFORM.BELGENO";

            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOADMIFF, ALANLARMIFF[(int)ENUMAlanlarMIFF.YIL], "=", f.yil));

            if (f.formDurum > 0)
            {
                if (f.formDurum == 1 || f.formDurum == 20 || f.formDurum == 21)
                {
                    sql += " AND (" + VeriErisim.KosulOl(TABLOADMIFF, ALANLARMIFF[(int)ENUMAlanlarMIFF.FORMDURUM], "=", (int)ENUMBelgeDurumu.YENI);
                    sql += " OR " + VeriErisim.KosulOl(TABLOADMIFF, ALANLARMIFF[(int)ENUMAlanlarMIFF.FORMDURUM], "=", (int)ENUMBelgeDurumu.DEGISTIRILDI) + ")";
                }
                else if (f.formDurum == 22)
                    sql += " AND " + VeriErisim.KosulOl(TABLOADMIFF, ALANLARMIFF[(int)ENUMAlanlarMIFF.FORMDURUM], "<", (int)ENUMBelgeDurumu.MUHASEBATIADE);
                else if (f.formDurum == 23)
                {
                    sql += " AND " + VeriErisim.KosulOl(TABLOADMIFF, ALANLARMIFF[(int)ENUMAlanlarMIFF.FORMDURUM], "=", (int)ENUMBelgeDurumu.ONAYLI);
                    sql += " AND (" + VeriErisim.KosulOl(TABLOADMIFF, ALANLARMIFF[(int)ENUMAlanlarMIFF.YEVMIYENO], "=", 0);
                    sql += " OR " + TABLOADMIFF + "." + ALANLARMIFF[(int)ENUMAlanlarMIFF.YEVMIYENO] + "=" + "null" + ")";
                }
                else
                    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARMIFF[(int)ENUMAlanlarMIFF.FORMDURUM], "=", f.formDurum));

                if (f.formDurum == 21)
                    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARMIFF[(int)ENUMAlanlarMIFF.YEVMIYENO], ">", 0));
            }

            if (f.muhasebe > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADMIFF, ALANLARMIFF[(int)ENUMAlanlarMIFF.MUHASEBEKOD], "=", f.muhasebe));

            if (f.birim != "")
            {
                string t1;
                string a1;
                if (ANABIRIM)
                {
                    t1 = TABLOADMIFF;
                    a1 = ALANLARMIFF[(int)ENUMAlanlarMIFF.BIRIMKOD];
                }
                else
                {
                    t1 = TABLOADMIFD;
                    a1 = ALANLARMIFD[(int)ENUMAlanlarMIFD.DETAYBIRIM];
                }
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.IN_KosulOl("", t1 + "." + a1, f.birim, true, false));
                //sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(t1, a1, "=", f.birim));
            }

            if (f.fisTur != null && f.fisTur.Length > 0)
            {
                sql = VeriErisim.KosulEkle(sql, "AND", TABMizanBilanco.SorguOl(ALANLARMIFF[(int)ENUMAlanlarMIFF.FISTUR], f.fisTur));
            }

            if (f.sorguIslemTarih != "")
            {
                string[] t = f.sorguIslemTarih.Split(';');
                YilKontrol(ref t, f.yil);

                System.DateTime d1;
                System.DateTime d2;
                if (t.Length == 1 && t[0] != "" || (t.Length == 2 && t[0] != "" && t[1] == ""))
                {
                    d1 = Convert.ToDateTime(new TNSDateTime(t[0]).ToString());
                    d2 = Convert.ToDateTime(new TNSDateTime(t[0]).ToString()).AddDays(1).AddSeconds(-1);
                    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADMIFF, ALANLARMIFF[(int)ENUMAlanlarMIFF.ISLEMTARIH], ">=", new TNSDateTime(d1)));
                    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADMIFF, ALANLARMIFF[(int)ENUMAlanlarMIFF.ISLEMTARIH], "<=", new TNSDateTime(d2)));
                }
                else if (t.Length == 2 && t[0] != "" && t[1] != "")
                {
                    d1 = Convert.ToDateTime(new TNSDateTime(t[0]).ToString());
                    d2 = Convert.ToDateTime(new TNSDateTime(t[1]).ToString()).AddDays(1).AddSeconds(-1);
                    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADMIFF, ALANLARMIFF[(int)ENUMAlanlarMIFF.ISLEMTARIH], ">=", new TNSDateTime(d1)));
                    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADMIFF, ALANLARMIFF[(int)ENUMAlanlarMIFF.ISLEMTARIH], "<=", new TNSDateTime(d2)));
                }
                else if (t.Length == 2 && t[0] == "" && t[1] != "")
                {
                    d2 = Convert.ToDateTime(new TNSDateTime(t[1]).ToString()).AddDays(1).AddSeconds(-1);
                    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADMIFF, ALANLARMIFF[(int)ENUMAlanlarMIFF.ISLEMTARIH], "<=", new TNSDateTime(d2)));
                }
            }

            if (f.sorguYEVYevTarih != "")
            {
                string[] t = f.sorguYEVYevTarih.Split(';');
                YilKontrol(ref t, f.yil);

                System.DateTime d1;
                System.DateTime d2;
                if (t.Length == 1 && t[0] != "" || (t.Length == 2 && t[0] != "" && t[1] == ""))
                {
                    d1 = Convert.ToDateTime(new TNSDateTime(t[0]).ToString());
                    d2 = Convert.ToDateTime(new TNSDateTime(t[0]).ToString()).AddDays(1).AddSeconds(-1);
                    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADMIFF, ALANLARMIFF[(int)ENUMAlanlarMIFF.YEVMIYETARIH], ">=", new TNSDateTime(d1)));
                    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADMIFF, ALANLARMIFF[(int)ENUMAlanlarMIFF.YEVMIYETARIH], "<=", new TNSDateTime(d2)));
                }
                else if (t.Length == 2 && t[0] != "" && t[1] != "")
                {
                    d1 = Convert.ToDateTime(new TNSDateTime(t[0]).ToString());
                    d2 = Convert.ToDateTime(new TNSDateTime(t[1]).ToString()).AddDays(1).AddSeconds(-1);
                    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADMIFF, ALANLARMIFF[(int)ENUMAlanlarMIFF.YEVMIYETARIH], ">=", new TNSDateTime(d1)));
                    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADMIFF, ALANLARMIFF[(int)ENUMAlanlarMIFF.YEVMIYETARIH], "<=", new TNSDateTime(d2)));
                }
                else if (t.Length == 2 && t[0] == "" && t[1] != "")
                {
                    d2 = Convert.ToDateTime(new TNSDateTime(t[1]).ToString()).AddDays(1).AddSeconds(-1);
                    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADMIFF, ALANLARMIFF[(int)ENUMAlanlarMIFF.YEVMIYETARIH], "<=", new TNSDateTime(d2)));
                }
            }

            if (f.sorguFormDurumTarih != "")
            {
                string[] t = f.sorguFormDurumTarih.Split(';');
                YilKontrol(ref t, f.yil);

                System.DateTime d1;
                System.DateTime d2;
                if (t.Length == 1 && t[0] != "" || (t.Length == 2 && t[0] != "" && t[1] == ""))
                {
                    d1 = Convert.ToDateTime(new TNSDateTime(t[0]).ToString());
                    d2 = Convert.ToDateTime(new TNSDateTime(t[0]).ToString()).AddDays(1).AddSeconds(-1);
                    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADMIFF, ALANLARMIFF[(int)ENUMAlanlarMIFF.DURUMTARIH], ">=", new TNSDateTime(d1)));
                    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADMIFF, ALANLARMIFF[(int)ENUMAlanlarMIFF.DURUMTARIH], "<=", new TNSDateTime(d2)));
                }
                else if (t.Length == 2 && t[0] != "" && t[1] != "")
                {
                    d1 = Convert.ToDateTime(new TNSDateTime(t[0]).ToString());
                    d2 = Convert.ToDateTime(new TNSDateTime(t[1]).ToString()).AddDays(1).AddSeconds(-1);
                    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADMIFF, ALANLARMIFF[(int)ENUMAlanlarMIFF.DURUMTARIH], ">=", new TNSDateTime(d1)));
                    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADMIFF, ALANLARMIFF[(int)ENUMAlanlarMIFF.DURUMTARIH], "<=", new TNSDateTime(d2)));
                }
                else if (t.Length == 2 && t[0] == "" && t[1] != "")
                {
                    d2 = Convert.ToDateTime(new TNSDateTime(t[1]).ToString()).AddDays(1).AddSeconds(-1);
                    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADMIFF, ALANLARMIFF[(int)ENUMAlanlarMIFF.DURUMTARIH], "<=", new TNSDateTime(d2)));
                }
            }

            foreach (OdemeEmriMIFDetay d in f.detay.objeler)
            {
                string[] hesapKod = d.hesapKod.Split(';');
                string hesapSQL = "";
                for (int i = 0; i < hesapKod.Length; i++)
                {
                    if (hesapKod[i].Trim() != "" && !hesapKod[i].StartsWith("-"))
                        hesapSQL = VeriErisim.KosulEkle(hesapSQL, "OR", VeriErisim.KosulOl(TABLOADMIFD, ALANLARMIFD[(int)ENUMAlanlarMIFD.HESAPKOD], "LIKE", hesapKod[i].Trim() + "%"));
                }
                if (hesapSQL != "")
                    sql += " AND (" + hesapSQL + ")";

                for (int i = 0; i < hesapKod.Length; i++)
                {
                    if (hesapKod[i].Trim() != "" && hesapKod[i].StartsWith("-"))
                    {
                        hesapKod[i] = hesapKod[i].Replace("-", "");
                        sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADMIFD, ALANLARMIFD[(int)ENUMAlanlarMIFD.HESAPKOD], "NOT LIKE", hesapKod[i].Trim() + "%"));
                    }
                }

                string kur = d.kur.Replace(".", "");
                string fon = d.fon.Replace(".", "");
                string fin = d.fin.Replace(".", "");
                string eko = d.eko.Replace(".", "");
                string peb = d.peb.Replace(".", "");

                if (kur != "")
                    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADMIFD, ALANLARMIFD[(int)ENUMAlanlarMIFD.KURKOD], "LIKE", kur + "%"));
                if (fon != "")
                    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADMIFD, ALANLARMIFD[(int)ENUMAlanlarMIFD.FONKOD], "LIKE", fon + "%"));
                if (fin != "")
                    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADMIFD, ALANLARMIFD[(int)ENUMAlanlarMIFD.FINKOD], "=", fin));
                if (eko != "")
                    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADMIFD, ALANLARMIFD[(int)ENUMAlanlarMIFD.EKOKOD], "LIKE", eko + "%"));
                if (peb != "")
                    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADMIFD, ALANLARMIFD[(int)ENUMAlanlarMIFD.PEBKOD], "LIKE", peb + "%"));
            }

            if (tur == (int)ENUMKesinHesapButce.KURUMSAL)
            {
                sql += " GROUP BY " + VeriErisim.IslecBul("SUBSTRING") + "(HESAPKOD,1,3),MUHMIF.KURKOD";
                sql += " ORDER BY " + VeriErisim.IslecBul("SUBSTRING") + "(HESAPKOD,1,3),MUHMIF.KURKOD";
            }
            else if (tur == (int)ENUMKesinHesapButce.FONKSIYONEL)
            {
                sql += " GROUP BY " + VeriErisim.IslecBul("SUBSTRING") + "(HESAPKOD,1,3),FONKOD";
                sql += " ORDER BY " + VeriErisim.IslecBul("SUBSTRING") + "(HESAPKOD,1,3),FONKOD";
            }
            else if (tur == (int)ENUMKesinHesapButce.FINANS)
            {
                sql += " GROUP BY " + VeriErisim.IslecBul("SUBSTRING") + "(HESAPKOD,1,3),FINKOD";
                sql += " ORDER BY " + VeriErisim.IslecBul("SUBSTRING") + "(HESAPKOD,1,3),FINKOD";
            }
            else if (tur == (int)ENUMKesinHesapButce.EKONOMIK)
            {
                sql += " GROUP BY " + VeriErisim.IslecBul("SUBSTRING") + "(HESAPKOD,1,3),EKOKOD";
                sql += " ORDER BY " + VeriErisim.IslecBul("SUBSTRING") + "(HESAPKOD,1,3),EKOKOD";
            }
            else if (tur == (int)ENUMKesinHesapButce.EKONOMIK2)
            {
                sql += " GROUP BY " + VeriErisim.IslecBul("SUBSTRING") + "(EKOKOD,1," + eko4Uz + ")";
                sql += " ORDER BY " + VeriErisim.IslecBul("SUBSTRING") + "(EKOKOD,1," + eko4Uz + ")";
            }
            else if (tur == (int)ENUMKesinHesapButce.FINEKO2PEB)
            {
                sql += " GROUP BY '--' + FINKOD + '-'+ " + VeriErisim.IslecBul("SUBSTRING") + "(EKOKOD,1," + eko4Uz + ")+'-'+PEBKOD";
                sql += " ORDER BY '--' + FINKOD + '-'+ " + VeriErisim.IslecBul("SUBSTRING") + "(EKOKOD,1," + eko4Uz + ")+'-'+PEBKOD";
            }
            else if (tur == (int)ENUMKesinHesapButce.GELIREKONOMIK || tur == (int)ENUMKesinHesapButce.GIDEREKONOMIK)
            {
                sql += " GROUP BY " + VeriErisim.IslecBul("SUBSTRING") + "(HESAPKOD,1,11)," + VeriErisim.IslecBul("SUBSTRING") + "(HESAPKOD,1,11)";
                sql += " ORDER BY " + VeriErisim.IslecBul("SUBSTRING") + "(HESAPKOD,1,11), BOSLUK";
            }
            else if (tur == (int)ENUMKesinHesapButce.BILONCO)
            {
                sql += " GROUP BY " + VeriErisim.IslecBul("SUBSTRING") + "(HESAPKOD,1,3)," + VeriErisim.IslecBul("SUBSTRING") + "(HESAPKOD,1,3)";
                sql += " ORDER BY " + VeriErisim.IslecBul("SUBSTRING") + "(HESAPKOD,1,3), BOSLUK";
            }
            //else if (rakamVer)
            //{
            //    sql += " GROUP BY " + VeriErisim.IslecBul("SUBSTRING") + "(HESAPKOD,1,3),MUHMIF.KURKOD";
            //    sql += " ORDER BY " + VeriErisim.IslecBul("SUBSTRING") + "(HESAPKOD,1,3),MUHMIF.KURKOD";
            //}
            else if (sadeceHesapKod)
            {
                if (gelirHesapKod)
                {
                    sql += "GROUP BY " + VeriErisim.IslecBul("SUBSTRING") + "(HESAPKOD,1,11)";
                    sql += "ORDER BY  " + VeriErisim.IslecBul("SUBSTRING") + "(HESAPKOD,1,11)";
                }
                else
                {
                    sql += " GROUP BY HESAPKOD";
                    sql += " ORDER BY HESAPKOD";
                }
            }
            else if (!rakamVer)
            {
                sql += " GROUP BY MUHMIF.KURKOD,FONKOD,FINKOD," + VeriErisim.IslecBul("SUBSTRING") + "(EKOKOD,1," + eko4Uz + ")";
                sql += " ORDER BY MUHMIF.KURKOD,FONKOD,FINKOD," + VeriErisim.IslecBul("SUBSTRING") + "(EKOKOD,1," + eko4Uz + ")";
            }
            veriErisim.sureAsimi = 60;
            return veriErisim.DataReaderDondur(sql);
        }

        private void YilKontrol(ref string[] t, int fyil)
        {
            for (int i = 0; i < t.Length; i++)
            {
                string[] parca = t[i].Split('-', '.', '/', ' ');
                if (parca.Length == 3 && parca[2] != fyil.ToString() && fyil > 0)
                {
                    t[i] = parca[0] + "." + parca[1] + "." + fyil;
                }

                //Sorgulama yapılan yıl artık yıl ve tarih 29.02 geldi ise, kesin hesap raporları bu tarihleri baz alarak 
                //önceki yıllardaki rakamlarını getirmeye çalışıyor. Bu durumda tarih 29.02.2015 gibi artık olmayan 
                //yılda şubat 29 tarihi çevrilmek isteniyor. Bundan dolayı tarihi 28.02 yapmak lazım
                TNSDateTime kontrolTarih = new TNSDateTime(t[i]);
                if (kontrolTarih.isNull)
                {
                    if (!(t[i] == null || t[i] == ""))
                    {
                        string[] parcaT = t[i].Split('-', '.', '/', ' ');
                        int yilT = OrtakFonksiyonlar.ConvertToInt(parca[2], 0);
                        int ayT = OrtakFonksiyonlar.ConvertToInt(parca[1], 0);
                        int gunT = OrtakFonksiyonlar.ConvertToInt(parca[0], 0);
                        if (gunT == 29 && ayT == 2)
                        {
                            if (!(yilT % 4 == 0 && yilT % 400 == 0))
                            {
                                t[i] = "28" + "." + parcaT[1] + "." + parcaT[2];
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Maaş entegrasyou liste sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon..
        /// </summary>
        /// <param name="f">Ödeme emri/MİF form bilgileri içeren nesne</param>
        /// <returns></returns>
        public IDataReader MaasEntegrasyouListe(OdemeEmriMIFForm f)
        {
            string sql = "";

            sql = "SELECT * FROM MUHBORDRO M LEFT OUTER JOIN MUHBORDRODETAY D ON M.FISNO = D.FISNO";
            sql += " WHERE M.YIL=" + f.yil;
            sql += " AND M.AY=" + f.ay;
            if (f.belgeNo != "")
                sql += " AND M.FISNO=" + f.belgeNo;
            if (f.muhasebe > 0)
                sql += " AND M.MUHASEBEKOD=" + f.muhasebe;
            if (f.birim != "")
                sql += " AND M.BIRIMKOD=" + f.birim;
            if (f.bordroTur != "")
                sql += " AND M.FISTUR=" + f.bordroTur;
            if (f.perTur != "")
                sql += " AND M.PERTUR=" + f.perTur;
            sql += " ORDER BY M.FISNO,SIRANO";

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Maaş entegrasyonu işlendi sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon..
        /// </summary>
        /// <param name="fisNo">Fiş no.</param>
        public void MaasEntegrasyonuIslendi(string fisNo)
        {
            string sql = "";

            sql = "UPDATE MUHBORDRO SET ALINDI='1'";
            sql += " WHERE FISNO=" + fisNo;

            KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Kapanis bilgilerini kaydet
        /// </summary>
        /// <param name="kapatma">The k.</param>
        /// <returns></returns>
        public Sonuc KapanisBilgisiKaydet(KapanisBilgileri kapatma)
        {
            string sql = "INSERT INTO " + TABLOADKAPANIS + " (" + VeriErisim.TumAlanlar("", ALANLARKAPANIS) + ") VALUES (";
            sql = sql + VeriErisim.DegerYap(kapatma.yil) + ", ";
            sql = sql + VeriErisim.DegerYap(kapatma.belgeNo) + ", ";
            sql = sql + VeriErisim.DegerYap(kapatma.kYil) + ", ";
            sql = sql + VeriErisim.DegerYap(kapatma.kBelgeNo) + ", ";
            sql = sql + VeriErisim.DegerYap(kapatma.kHesapKod.Replace(".", "").Trim()) + ", ";
            sql = sql + VeriErisim.DegerYap(kapatma.kIlgiliVno) + ", ";
            sql = sql + VeriErisim.DegerYap(kapatma.kTertip.Replace(".", "")) + ", ";
            sql = sql + VeriErisim.DegerYap(kapatma.kTutar) + ",";
            sql = sql + VeriErisim.DegerYap(kapatma.siraNo) + ",";
            sql = sql + VeriErisim.DegerYap(kapatma.borcAlacak) + ",";
            sql = sql + VeriErisim.DegerYap(kapatma.kBirimKod) + ")";
            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Kapanis bilgilerini iptal eder
        /// </summary>
        /// <param name="kapatma">The k.</param>
        /// <returns></returns>
        public Sonuc KapanisBilgisiIptalEt(KapanisBilgileri kapatma)
        {
            string sql = "DELETE FROM " + TABLOADKAPANIS;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARKAPANIS[(int)ENUMAlanlarKAPANIS.YIL], "=", kapatma.yil));
            if (kapatma.belgeNo != "")
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARKAPANIS[(int)ENUMAlanlarKAPANIS.BELGENO], "=", kapatma.belgeNo));
            else
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARKAPANIS[(int)ENUMAlanlarKAPANIS.KBELGENO], "=", kapatma.kBelgeNo));

            return KomutCalistir(sql, 0);
        }

        public IDataReader KapanisBilgisiListele(KapanisBilgileri kriter)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOADKAPANIS, ALANLARKAPANIS) + " FROM " + TABLOADKAPANIS;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOADKAPANIS, ALANLARKAPANIS[(int)ENUMAlanlarKAPANIS.YIL], "=", kriter.yil));
            if (kriter.belgeNo != "")
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADKAPANIS, ALANLARKAPANIS[(int)ENUMAlanlarKAPANIS.BELGENO], "=", kriter.belgeNo));
            if (kriter.kHesapKod != "")
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADKAPANIS, ALANLARKAPANIS[(int)ENUMAlanlarKAPANIS.KHESAPKOD], "=", kriter.kHesapKod.Replace(".", "")));
            if (kriter.kBelgeNo != "")
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADKAPANIS, ALANLARKAPANIS[(int)ENUMAlanlarKAPANIS.KBELGENO], "=", kriter.kBelgeNo));
            if (kriter.kIlgiliVno != "")
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADKAPANIS, ALANLARKAPANIS[(int)ENUMAlanlarKAPANIS.KILGILIVNO], "=", kriter.kIlgiliVno));
            if (kriter.kBirimKod != "")
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADKAPANIS, ALANLARKAPANIS[(int)ENUMAlanlarKAPANIS.KBIRIMKOD], "=", kriter.kBirimKod));

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Yenis the alan olustur.
        /// </summary>
        public void YeniAlanOlustur()
        {
            if (yenialan)
            {
                yenialan = false;
                //string sql = "";
                //Sonuc sonuc = new Sonuc();
                //try
                //{
                //    sql = "ALTER TABLE MUHMIF ADD ALTHESAPKOD varchar(30) NULL";
                //    sonuc = KomutCalistir(sql, 0, 0);
                //    if (sonuc.islemSonuc)
                //    {
                //        sql = "UPDATE MUHMIF SET ALTHESAPKOD ='' WHERE ALTHESAPKOD IS NULL";
                //        KomutCalistir(sql, 0, 0);
                //    }
                //}
                //catch { }
                //veriErisim.hata = "";
                //try
                //{
                //    sql = "ALTER TABLE MUHMIFFORM ADD KURKOD varchar(20) NULL";
                //    sonuc = KomutCalistir(sql, 0, 0);
                //    if (sonuc.islemSonuc)
                //    {
                //        sql = "UPDATE MUHMIFFORM SET KURKOD='' WHERE KURKOD IS NULL";
                //        KomutCalistir(sql, 0, 0);
                //        sql = "UPDATE MF SET MF.KURKOD = M.KURKOD FROM MUHMIFFORM AS MF INNER JOIN MUHMIF AS M ON MF.YIL = M.YIL AND MF.BELGENO = M.BELGENO AND MF.YIL = 2011 AND M.KURKOD<>''";
                //        KomutCalistir(sql, 0, 0);
                //    }
                //}
                //catch { }
                //veriErisim.hata = "";

                //try
                //{
                //    sql = "ALTER TABLE MUHHESAPPLANI ADD ALTHESAPKOD int NULL";
                //    sonuc = KomutCalistir(sql, 0, 0);
                //    if (sonuc.islemSonuc)
                //    {
                //        sql = "UPDATE MUHHESAPPLANI SET ALTHESAPKOD=0 WHERE ALTHESAPKOD IS NULL";
                //        KomutCalistir(sql, 0, 0);
                //    }
                //}
                //catch { }
                //veriErisim.hata = "";
                //try
                //{
                //    sql = "CREATE TABLE [MUHKAPATMA]( [YIL] [int] NOT NULL, [BELGENO] [varchar](20) NOT NULL, [KYIL] [int] NOT NULL, [KBELGENO] [varchar](20) NOT NULL, [KHESAPKOD] [varchar](30) NOT NULL, [KILGILIVNO] [varchar](20) NOT NULL, [KTERTIP] [varchar](50) NULL, [KTUTAR] [money] NULL, CONSTRAINT [PK_MUHKAPATMA] PRIMARY KEY CLUSTERED (	[YIL] ASC,	[BELGENO] ASC,	[KYIL] ASC,	[KBELGENO] ASC,	[KHESAPKOD] ASC,	[KILGILIVNO] ASC)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]) ON [PRIMARY]";
                //    sonuc = KomutCalistir(sql, 0, 0);
                //}
                //catch { }

                //veriErisim.hata = "";
                //try
                //{
                //    sql = "ALTER TABLE MUHMIFFORM ADD ORGANIZASYONKOD int NULL";
                //    sonuc = KomutCalistir(sql, 0, 0);
                //    if (sonuc.islemSonuc)
                //    {
                //        sql = "UPDATE MUHMIFFORM SET ORGANIZASYONKOD=0 WHERE ORGANIZASYONKOD IS NULL";
                //        KomutCalistir(sql, 0, 0);
                //    }
                //}
                //catch { }

                //veriErisim.hata = "";
                //try
                //{
                //    sql = "ALTER TABLE dbo.MUHMIF ADD DOVIZ varchar(20) NULL, DOVIZKURTUR varchar(20) NULL, DOVIZBORC money NULL, DOVIZALACAK money NULL, DOVIZKUR decimal(18, 6) NULL";
                //    sonuc = KomutCalistir(sql, 0, 0);
                //}
                //catch { }

                //veriErisim.hata = "";
                //try
                //{
                //    sql = "ALTER TABLE dbo.MUHHESAPPLANI ADD ACIKLAMA2 varchar(50) NULL, DOVIZ varchar(20) NULL, DOVIZKURTURSATIS varchar(20) NULL, DOVIZKURTURALIS varchar(20) NULL, KURFARKIBORCHESABI varchar(50) NULL, KURFARKIALACAKHESABI varchar(50) NULL";
                //    sonuc = KomutCalistir(sql, 0, 0);
                //}
                //catch { }


                //veriErisim.hata = "";
                //try
                //{
                //    sql = "ALTER TABLE MUHSABLONFORM ADD ORGANIZASYONKOD int NULL, KURKOD varchar(20) NULL";
                //    sonuc = KomutCalistir(sql, 0, 0);
                //}
                //catch { }
                //veriErisim.hata = "";
                //try
                //{
                //    sql = "ALTER TABLE dbo.MUHSABLON ADD DOVIZ varchar(20) NULL, DOVIZKURTUR varchar(20) NULL, DOVIZBORC money NULL, DOVIZALACAK money NULL, DOVIZKUR decimal(18, 6) NULL";
                //    sonuc = KomutCalistir(sql, 0, 0);
                //}
                //catch { }

                //veriErisim.hata = "";
                //try
                //{
                //    sql = "ALTER TABLE dbo.MUHMIF ADD ACIKLAMA varchar(150) NULL";
                //    sonuc = KomutCalistir(sql, 0, 0);
                //}
                //catch { }

                //veriErisim.hata = "";
                //try
                //{
                //    sql = "UPDATE dbo.MUHMIF SET KURKOD=REPLACE(KURKOD,'.','') WHERE KURKOD LIKE '%.%'";
                //    sonuc = KomutCalistir(sql, 0, 0);
                //    sql = "UPDATE dbo.MUHMIF SET FONKOD=REPLACE(FONKOD,'.','') WHERE FONKOD LIKE '%.%'";
                //    sonuc = KomutCalistir(sql, 0, 0);
                //    sql = "UPDATE dbo.MUHMIF SET EKOKOD=REPLACE(EKOKOD,'.','') WHERE EKOKOD LIKE '%.%'";
                //    sonuc = KomutCalistir(sql, 0, 0);
                //}
                //catch { }
            }
        }

        /// <summary>
        /// Kaydets the oybs.
        /// </summary>
        /// <param name="yil">The yil.</param>
        /// <param name="belgeNo">The belge no.</param>
        /// <param name="islemTip">The islem tip.</param>
        /// <param name="oybsId">The oybs id.</param>
        /// <returns></returns>
        public Sonuc KaydetOybs(int yil, string belgeNo, int islemTip, long oybsId)
        {
            Sonuc sonuc = new Sonuc(false, "", "");
            string sql = "";

            sql = "INSERT INTO " + TABLOADOYBS + " (" + VeriErisim.TumAlanlar("", ALANLAROYBS) + ") VALUES (";
            sql = sql + VeriErisim.DegerYap(yil) + ", ";
            sql = sql + VeriErisim.DegerYap(belgeNo) + ", ";
            sql = sql + VeriErisim.DegerYap(islemTip) + ", ";
            sql = sql + VeriErisim.DegerYap(oybsId) + ") ";

            sonuc = KomutCalistir(sql, 1);

            return sonuc;
        }

        /// <summary>
        /// Sils the oybs.
        /// </summary>
        /// <param name="yil">The yil.</param>
        /// <param name="belgeNo">The belge no.</param>
        /// <returns></returns>
        public Sonuc SilOybs(int yil, string belgeNo)
        {
            string sql = "";

            sql = "DELETE FROM " + TABLOADOYBS;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAROYBS[(int)ENUMAlanlarOybs.YIL], "=", yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAROYBS[(int)ENUMAlanlarOybs.BELGENO], "=", belgeNo));

            return KomutCalistir(sql, 0);
        }

        public IDataReader ListeleOybs(int yil, string belgeNo)
        {
            string sql = "";
            string kosul = "";

            sql = "SELECT " + VeriErisim.TumAlanlar(TABLOADOYBS, ALANLAROYBS) + " FROM " + TABLOADOYBS;

            if (yil > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADOYBS, ALANLAROYBS[(int)ENUMAlanlarOybs.YIL], "=", yil));

            if (!string.IsNullOrEmpty(belgeNo))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADOYBS, ALANLAROYBS[(int)ENUMAlanlarOybs.BELGENO], "=", belgeNo));

            if (kosul != "")
                sql += " WHERE " + kosul;

            return veriErisim.DataReaderDondur(sql);
        }

        public IDataReader ButceOdenekKapat(int yil, int muhasebe, string birim, string belgeNo, TNSDateTime belgeTarih)
        {
            string sql = "SELECT  MUHMIFFORM.MUHASEBEKOD, MUHMIFFORM.BIRIMKOD, MUHMIF.HESAPKOD, MUHMIF.KURKOD, MUHMIF.FONKOD, MUHMIF.FINKOD, MUHMIF.EKOKOD, MUHMIF.PEBKOD, SUM(MUHMIF.ALACAK) - SUM(MUHMIF.BORC)";
            sql += " FROM MUHMIF INNER JOIN MUHMIFFORM ON MUHMIF.YIL = MUHMIFFORM.YIL AND MUHMIF.BELGENO = MUHMIFFORM.BELGENO";
            sql += " WHERE (MUHMIFFORM.YIL = " + yil + ") AND (MUHMIF.HESAPKOD LIKE '903%') AND (MUHMIFFORM.FORMDURUM = 6)";
            if (muhasebe > 0)
                sql += " AND MUHMIFFORM.MUHASEBEKOD=" + muhasebe;
            if (birim != "")
                sql += " AND MUHMIFFORM.BIRIMKOD='" + birim + "'";
            if (belgeNo != "")
                sql += " AND MUHMIFFORM.BELGENO<>'" + belgeNo + "'";
            if (!belgeTarih.isNull)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("MUHMIFFORM", "YEVMIYETARIH", "<=", belgeTarih));

            sql += " GROUP BY  MUHMIFFORM.MUHASEBEKOD, MUHMIFFORM.BIRIMKOD, MUHMIF.HESAPKOD, MUHMIF.KURKOD, MUHMIF.FONKOD, MUHMIF.FINKOD, MUHMIF.EKOKOD, MUHMIF.PEBKOD";
            sql += " HAVING (SUM(MUHMIF.ALACAK) - SUM(MUHMIF.BORC) > 0)";

            return veriErisim.DataReaderDondur(sql);
        }

        public IDataReader SurecBlokeToplam(OdenekDurumKriter kr)
        {
            string sql = "SELECT ";
            string alanlar = "";

            if (kr.detay == ENUMOdenekDurumDetay.MUHASEBE)
            {
                if (alanlar != "") alanlar += ",";
                alanlar += ALANLARMIFF[(int)ENUMAlanlarMIFF.MUHASEBEKOD];
            }
            else if (kr.detay == ENUMOdenekDurumDetay.BIRIM)
            {
                if (alanlar != "") alanlar += ",";
                alanlar += ALANLARMIFF[(int)ENUMAlanlarMIFF.MUHASEBEKOD];
                if (ANABIRIM)
                    alanlar += "," + ALANLARMIFF[(int)ENUMAlanlarMIFF.BIRIMKOD];
                else
                    alanlar += "," + ALANLARMIFD[(int)ENUMAlanlarMIFD.DETAYBIRIM];
            }

            if (!kr.konsolide)
            {
                if (alanlar != "") alanlar += ",";
                alanlar += ALANLARMIFD[(int)ENUMAlanlarMIFD.KURKOD];
                alanlar += "," + ALANLARMIFD[(int)ENUMAlanlarMIFD.FONKOD];
                alanlar += "," + ALANLARMIFD[(int)ENUMAlanlarMIFD.FINKOD];

                alanlar += "," + BKD.Arac.EkoAlanOl(kr.yil, "", ALANLARMIFD[(int)ENUMAlanlarMIFD.EKOKOD], kr.ekoKodKirilim, kr.eko23, VeriErisim.IslecBul("SUBSTRING"));

                if (kr.faaliyetGoster)
                    alanlar += "," + ALANLARMIFD[(int)ENUMAlanlarMIFD.PEBKOD];
            }
            sql += alanlar;
            if (alanlar != "") sql += ",";

            sql += "SUM(" + "BLOKETUTAR" + ")";
            sql += " FROM HRCBLOKEODENEK WHERE ";
            sql += VeriErisim.KosulOl("", ALANLARMIFF[(int)ENUMAlanlarMIFF.YIL], "=", kr.yil);

            if (kr.muhasebe > 0)
                sql += " AND " + VeriErisim.KosulOl("", ALANLARMIFF[(int)ENUMAlanlarMIFF.MUHASEBEKOD], "=", kr.muhasebe);

            if (kr.birim != "")
            {
                if (ANABIRIM)
                    sql += " AND " + VeriErisim.KosulOl("", ALANLARMIFF[(int)ENUMAlanlarMIFF.BIRIMKOD], "=", kr.birim);
                else
                    sql += " AND " + VeriErisim.KosulOl("", ALANLARMIFD[(int)ENUMAlanlarMIFD.DETAYBIRIM], "=", kr.birim);
            }

            sql = TABOdenekHarcamaSorgu.TertipKosulOl(sql, kr.tertip, "", ALANLARMIFD[(int)ENUMAlanlarMIFD.KURKOD], ALANLARMIFD[(int)ENUMAlanlarMIFD.FONKOD], ALANLARMIFD[(int)ENUMAlanlarMIFD.FINKOD], ALANLARMIFD[(int)ENUMAlanlarMIFD.EKOKOD], ALANLARMIFD[(int)ENUMAlanlarMIFD.PEBKOD]);

            if (alanlar != "")
            {
                sql += " GROUP BY " + alanlar;
                sql += " ORDER BY " + alanlar;
            }

            return veriErisim.DataReaderDondur(sql);
        }

        public double YuzdeOnOdenenBul(int yil, string yuzdeOnAlimTur, int muhasebe, string birim, string haricOdemeEmriNo)
        {
            string sql = "SELECT SUM(" + ALANLARMIFD[(int)ENUMAlanlarMIFD.BORC] + "-" + ALANLARMIFD[(int)ENUMAlanlarMIFD.ALACAK] + ")";
            sql += " FROM " + TABLOADMIFF + "," + TABLOADMIFD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", TABLOADMIFF + "." + ALANLARMIFF[(int)ENUMAlanlarMIFF.YIL] + "=" + TABLOADMIFD + "." + ALANLARMIFD[(int)ENUMAlanlarMIFD.YIL]);
            sql = VeriErisim.KosulEkle(sql, "AND", TABLOADMIFF + "." + ALANLARMIFF[(int)ENUMAlanlarMIFF.BELGENO] + "=" + TABLOADMIFD + "." + ALANLARMIFD[(int)ENUMAlanlarMIFD.BELGENO]);

            sql += " AND " + VeriErisim.KosulOl(TABLOADMIFF, ALANLARMIFF[(int)ENUMAlanlarMIFF.YIL], "=", yil);
            sql += " AND " + VeriErisim.KosulOl(TABLOADMIFF, ALANLARMIFF[(int)ENUMAlanlarMIFF.FORMDURUM], "<>", (int)ENUMBelgeDurumu.IPTAL);
            sql += " AND " + TABLOADMIFF + "." + ALANLARMIFF[(int)ENUMAlanlarMIFF.IHALE] + " IN (" + yuzdeOnAlimTur + ")";
            sql += " AND " + VeriErisim.KosulOl(TABLOADMIFD, ALANLARMIFD[(int)ENUMAlanlarMIFD.HESAPKOD], "LIKE", "830%");

            if (muhasebe > 0)
                sql += " AND " + VeriErisim.KosulOl(TABLOADMIFF, ALANLARMIFF[(int)ENUMAlanlarMIFF.MUHASEBEKOD], "=", muhasebe);

            if (!string.IsNullOrWhiteSpace(birim))
            {
                if (ANABIRIM)
                    sql += " AND " + VeriErisim.KosulOl(TABLOADMIFF, ALANLARMIFF[(int)ENUMAlanlarMIFF.BIRIMKOD], "IN", "(" + birim + ")");
                else
                    sql += " AND " + VeriErisim.KosulOl(TABLOADMIFD, ALANLARMIFD[(int)ENUMAlanlarMIFD.DETAYBIRIM], "IN", "(" + birim + ")");
            }
            if (!string.IsNullOrWhiteSpace(haricOdemeEmriNo))
                sql += " AND " + VeriErisim.KosulOl(TABLOADMIFF, ALANLARMIFF[(int)ENUMAlanlarMIFF.BELGENO], "<>", haricOdemeEmriNo);

            return veriErisim.TekKolonSQLDbl(sql);
        }

        public IDataReader SurecBelgeNoListele(int surecYil, string surecNo)
        {
            string sql = "SELECT MUHMIFFORM.YIL, MUHMIFFORM.BELGENO, SUM(MUHMIF.ALACAK)";
            sql += " FROM " + TABLOADMIFF + " LEFT OUTER JOIN " + TABLOADMIFD + " ON MUHMIFFORM.YIL = MUHMIF.YIL AND MUHMIFFORM.BELGENO = MUHMIF.BELGENO AND MUHMIF.HESAPKOD = '835'";

            sql += " WHERE " + VeriErisim.KosulOl(TABLOADMIFF, ALANLARMIFF[(int)ENUMAlanlarMIFF.SURECYIL], "=", surecYil);
            sql += " AND " + VeriErisim.KosulOl(TABLOADMIFF, ALANLARMIFF[(int)ENUMAlanlarMIFF.SURECNO], "=", surecNo);
            sql += " AND " + VeriErisim.KosulOl(TABLOADMIFF, ALANLARMIFF[(int)ENUMAlanlarMIFF.FORMDURUM], "<>", (int)ENUMBelgeDurumu.IPTAL);
            sql += " GROUP BY MUHMIFFORM.YIL, MUHMIFFORM.BELGENO";

            return veriErisim.DataReaderDondur(sql);
        }

        public Sonuc SurecBilgiGuncelle(int yil, string belgeNo, int surecYil, string surecNo, string ihaleKod)
        {
            string sql = "UPDATE " + TABLOADMIFF;
            sql += " SET " + VeriErisim.KosulOl("", ALANLARMIFF[(int)ENUMAlanlarMIFF.SURECNO], "=", surecNo) + ",";
            sql += VeriErisim.KosulOl("", ALANLARMIFF[(int)ENUMAlanlarMIFF.SURECYIL], "=", surecYil) + ",";
            sql += VeriErisim.KosulOl("", ALANLARMIFF[(int)ENUMAlanlarMIFF.IHALE], "=", ihaleKod.Replace(".", ""));
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARMIFF[(int)ENUMAlanlarMIFF.YIL], "=", yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARMIFF[(int)ENUMAlanlarMIFF.BELGENO], "=", belgeNo));
            return KomutCalistir(sql, 1);
        }

        private static string MuhasebeSorgu(string alan, string saymanlikKod)
        {
            string[] kodlar = saymanlikKod.Split(',');
            string icSorgu = "";
            foreach (string item in kodlar)
            {
                if (string.IsNullOrWhiteSpace(item)) continue;
                string icKosul = MuhasebeSorguAralik(alan, item);
                if (icSorgu != "") icSorgu += " OR ";
                icSorgu += icKosul;
            }
            return "(" + icSorgu + ")";
        }

        private static string MuhasebeSorguAralik(string alan, string saymanlikKod)
        {
            string[] kodlar = saymanlikKod.Split('/', '-');
            if (kodlar.Length == 2)
            {
                return alan + ">=" + kodlar[0] + " AND " + alan + "<" + kodlar[1];
            }
            else
                return alan + "=" + kodlar[0];
        }

        public static string FarkMIFSorgu(SGBKBSFark sgbKbsFark, bool eko2)
        {
            string ekoAlanAd;
            if (eko2)
                ekoAlanAd = BKD.Arac.EkoAlanOl(sgbKbsFark.yil, "", "MUHMIF.EKOKOD", 2, true, VeriErisim.IslecBul("SUBSTRING"));
            else
                ekoAlanAd = "MUHMIF.EKOKOD";
            string pebAlan = "";
            string pebGrupAlan = "";
            if (sgbKbsFark.pebli)
            {
                pebAlan = ", MUHMIF.PEBKOD PEBKOD";
                pebGrupAlan = ", MUHMIF.PEBKOD";
            }
            string sorguTabloOE = "SELECT MUHMIFFORM.MUHASEBEKOD MUHASEBEKOD, MUHMIFFORM.BIRIMKOD BIRIMKOD, MUHMIF.KURKOD KURKOD, MUHMIF.FONKOD FONKOD, MUHMIF.FINKOD FINKOD, " + ekoAlanAd + " EKOKOD " + pebAlan + ", SUM(MUHMIF.BORC-MUHMIF.ALACAK) GIDER";
            sorguTabloOE += " FROM MUHMIF LEFT OUTER JOIN";
            sorguTabloOE += " MUHMIFFORM ON MUHMIF.YIL = MUHMIFFORM.YIL AND MUHMIF.BELGENO = MUHMIFFORM.BELGENO";
            sorguTabloOE = VeriErisim.KosulEkle(sorguTabloOE, "WHERE", VeriErisim.KosulOl("MUHMIFFORM", "YIL", "=", sgbKbsFark.yil));
            if (!string.IsNullOrWhiteSpace(sgbKbsFark.SaySorgu))
                sorguTabloOE = VeriErisim.KosulEkle(sorguTabloOE, "AND", MuhasebeSorgu("MUHMIFFORM.MUHASEBEKOD", sgbKbsFark.SaySorgu));
            if (!string.IsNullOrWhiteSpace(sgbKbsFark.BirimKod))
                sorguTabloOE = VeriErisim.KosulEkle(sorguTabloOE, "AND", VeriErisim.KosulOl("MUHMIFFORM", "BIRIMKOD", "=", sgbKbsFark.BirimKod));
            sorguTabloOE = VeriErisim.KosulEkle(sorguTabloOE, "AND", VeriErisim.KosulOl("MUHMIFFORM", "FORMDURUM", "<", 9));
            sorguTabloOE = VeriErisim.KosulEkle(sorguTabloOE, "AND", "(MUHMIF.HESAPKOD LIKE '830%')");

            if (!string.IsNullOrWhiteSpace(sgbKbsFark.kurKod))
                sorguTabloOE = VeriErisim.KosulEkle(sorguTabloOE, "AND", VeriErisim.KosulOl("MUHMIF", "KURKOD", "LIKE", sgbKbsFark.kurKod.Replace(".", "") + "%"));
            if (!string.IsNullOrWhiteSpace(sgbKbsFark.fonKod))
                sorguTabloOE = VeriErisim.KosulEkle(sorguTabloOE, "AND", VeriErisim.KosulOl("MUHMIF", "FONKOD", "LIKE", sgbKbsFark.fonKod.Replace(".", "") + "%"));
            if (!string.IsNullOrWhiteSpace(sgbKbsFark.finKod))
                sorguTabloOE = VeriErisim.KosulEkle(sorguTabloOE, "AND", VeriErisim.KosulOl("MUHMIF", "FINKOD", "LIKE", sgbKbsFark.finKod.Replace(".", "") + "%"));
            if (!string.IsNullOrWhiteSpace(sgbKbsFark.ekoKod))
                sorguTabloOE = VeriErisim.KosulEkle(sorguTabloOE, "AND", VeriErisim.KosulOl("MUHMIF", "EKOKOD", "LIKE", sgbKbsFark.ekoKod.Replace(".", "") + "%"));
            if (!string.IsNullOrWhiteSpace(sgbKbsFark.pebKod))
                sorguTabloOE = VeriErisim.KosulEkle(sorguTabloOE, "AND", VeriErisim.KosulOl("MUHMIF", "PEBKOD", "LIKE", sgbKbsFark.pebKod.Replace(".", "") + "%"));

            sorguTabloOE += " GROUP BY MUHMIFFORM.MUHASEBEKOD, MUHMIFFORM.BIRIMKOD, MUHMIF.KURKOD, MUHMIF.FONKOD, MUHMIF.FINKOD, " + ekoAlanAd + pebGrupAlan;
            return sorguTabloOE;
        }

        public static string FarkGONDSorgu(SGBKBSFark sgbKbsFark)
        {
            BKD.Tertip tUzBul = new BKD.Tertip(sgbKbsFark.yil, BKD.ENUMTabloKod.TABPEBOdenek);
            int eko2Uz = tUzBul.KodUzunlukBul(3, 2, true);

            string ekoAlan = "D.EKOKOD";
            string sorguTabloOE = "SELECT F.MUHASEBEKOD MUHASEBEKOD, F.BIRIMKOD BIRIMKOD, D.KURKOD KURKOD, D.FONKOD FONKOD, D.FINKOD FINKOD, " + ekoAlan + " EKOKOD, D.PEBKOD PEBKOD, SUM(D.MIKTAR) GONDERILEN";
            sorguTabloOE += " FROM MUHGONDERMETENKIS D LEFT OUTER JOIN";
            sorguTabloOE += " MUHGONDERMETENKISFORM F ON D.YIL = F.YIL AND D.BELGENO = F.BELGENO AND D.BELGETUR=F.BELGETUR";
            sorguTabloOE = VeriErisim.KosulEkle(sorguTabloOE, "WHERE", VeriErisim.KosulOl("F", "YIL", "=", sgbKbsFark.yil));
            if (!string.IsNullOrWhiteSpace(sgbKbsFark.SaySorgu))
                sorguTabloOE = VeriErisim.KosulEkle(sorguTabloOE, "AND", MuhasebeSorgu("F.MUHASEBEKOD", sgbKbsFark.SaySorgu));
            if (!string.IsNullOrWhiteSpace(sgbKbsFark.BirimKod))
                sorguTabloOE = VeriErisim.KosulEkle(sorguTabloOE, "AND", VeriErisim.KosulOl("F", "BIRIMKOD", "=", sgbKbsFark.BirimKod));
            sorguTabloOE = VeriErisim.KosulEkle(sorguTabloOE, "AND", VeriErisim.KosulOl("F", "FORMDURUM", "<=", 6));

            if (!string.IsNullOrWhiteSpace(sgbKbsFark.kurKod))
                sorguTabloOE = VeriErisim.KosulEkle(sorguTabloOE, "AND", VeriErisim.KosulOl("D", "KURKOD", "LIKE", sgbKbsFark.kurKod.Replace(".", "") + "%"));
            if (!string.IsNullOrWhiteSpace(sgbKbsFark.fonKod))
                sorguTabloOE = VeriErisim.KosulEkle(sorguTabloOE, "AND", VeriErisim.KosulOl("D", "FONKOD", "LIKE", sgbKbsFark.fonKod.Replace(".", "") + "%"));
            if (!string.IsNullOrWhiteSpace(sgbKbsFark.finKod))
                sorguTabloOE = VeriErisim.KosulEkle(sorguTabloOE, "AND", VeriErisim.KosulOl("D", "FINKOD", "LIKE", sgbKbsFark.finKod.Replace(".", "") + "%"));
            if (!string.IsNullOrWhiteSpace(sgbKbsFark.ekoKod))
                sorguTabloOE = VeriErisim.KosulEkle(sorguTabloOE, "AND", VeriErisim.KosulOl("D", "EKOKOD", "LIKE", sgbKbsFark.ekoKod.Replace(".", "") + "%"));
            if (!string.IsNullOrWhiteSpace(sgbKbsFark.pebKod))
                sorguTabloOE = VeriErisim.KosulEkle(sorguTabloOE, "AND", VeriErisim.KosulOl("D", "PEBKOD", "LIKE", sgbKbsFark.pebKod.Replace(".", "") + "%"));
            sorguTabloOE += " GROUP BY F.MUHASEBEKOD, F.BIRIMKOD, D.KURKOD, D.FONKOD, D.FINKOD, " + ekoAlan + ", D.PEBKOD";
            return sorguTabloOE;
        }

        public static string FarkMuhSorgu(SGBKBSFark sgbKbsFark, bool eko2)
        {

            string ekoAlanAd;

            if (eko2)
                ekoAlanAd = BKD.Arac.EkoAlanOl(sgbKbsFark.yil, "", "EKOKOD", 2, true, VeriErisim.IslecBul("SUBSTRING"));
            else
                ekoAlanAd = "EKOKOD";

            string pebAlanAd;
            if (sgbKbsFark.pebli)
                pebAlanAd = ", PEBKOD";
            else
                pebAlanAd = "";

            string sorguTabloMuh = $"SELECT MUHASEBEKOD, BIRIMKOD, KURKOD, FONKOD, FINKOD,{ekoAlanAd} EKOKOD{pebAlanAd}, SUM(BORC-ALACAK) GIDER FROM MUHASEBAT";
            sorguTabloMuh = VeriErisim.KosulEkle(sorguTabloMuh, "WHERE", VeriErisim.KosulOl("", "YIL", "=", sgbKbsFark.yil));

            string ayKosul;
            string hesapSorgu;
            if (TABOdenekHarcamaSorgu.muhasebatTur == "")
            {
                ayKosul = "=";
                hesapSorgu = "HESAPKOD LIKE '830%' or HESAPKOD LIKE '834%'";
            }
            else
            {
                ayKosul = "<=";
                hesapSorgu = "HESAPKOD LIKE '830%'";
            }

            sorguTabloMuh = VeriErisim.KosulEkle(sorguTabloMuh, "AND", VeriErisim.KosulOl("", "AY", ayKosul, sgbKbsFark.ay));
            if (!string.IsNullOrWhiteSpace(sgbKbsFark.SaySorgu))
                sorguTabloMuh = VeriErisim.KosulEkle(sorguTabloMuh, "AND", MuhasebeSorgu("MUHASEBEKOD", sgbKbsFark.SaySorgu));
            if (!string.IsNullOrWhiteSpace(sgbKbsFark.BirimKod))
                sorguTabloMuh = VeriErisim.KosulEkle(sorguTabloMuh, "AND", VeriErisim.KosulOl("", "BIRIMKOD", "=", sgbKbsFark.BirimKod));
            sorguTabloMuh = VeriErisim.KosulEkle(sorguTabloMuh, "AND", "(" + hesapSorgu + ")");

            if (!string.IsNullOrWhiteSpace(sgbKbsFark.kurKod))
                sorguTabloMuh = VeriErisim.KosulEkle(sorguTabloMuh, "AND", VeriErisim.KosulOl("", "KURKOD", "LIKE", sgbKbsFark.kurKod.Replace(".", "") + "%"));
            if (!string.IsNullOrWhiteSpace(sgbKbsFark.fonKod))
                sorguTabloMuh = VeriErisim.KosulEkle(sorguTabloMuh, "AND", VeriErisim.KosulOl("", "FONKOD", "LIKE", sgbKbsFark.fonKod.Replace(".", "") + "%"));
            if (!string.IsNullOrWhiteSpace(sgbKbsFark.finKod))
                sorguTabloMuh = VeriErisim.KosulEkle(sorguTabloMuh, "AND", VeriErisim.KosulOl("", "FINKOD", "LIKE", sgbKbsFark.finKod.Replace(".", "") + "%"));
            if (!string.IsNullOrWhiteSpace(sgbKbsFark.ekoKod))
                sorguTabloMuh = VeriErisim.KosulEkle(sorguTabloMuh, "AND", VeriErisim.KosulOl("", "EKOKOD", "LIKE", sgbKbsFark.ekoKod.Replace(".", "") + "%"));
            if (!string.IsNullOrWhiteSpace(sgbKbsFark.pebKod))
                sorguTabloMuh = VeriErisim.KosulEkle(sorguTabloMuh, "AND", VeriErisim.KosulOl("", "PEBKOD", "LIKE", sgbKbsFark.pebKod.Replace(".", "") + "%"));

            sorguTabloMuh += $" GROUP BY MUHASEBEKOD, BIRIMKOD, KURKOD, FONKOD, FINKOD, {ekoAlanAd}{pebAlanAd}";

            return sorguTabloMuh;
        }

        public static string FarkMuhMIFSorgu(string alanlar, int yil, int ay, string saymanlikKod, string birimKod, bool MuhBuyuk, string ekKosul, string siralama, string kurKod, string fonKod, string finKod, string ekoKod, string pebKod, bool pebli, int tur)
        {
            SGBKBSFark sgbKbsFark = new SGBKBSFark();
            sgbKbsFark.yil = yil;
            sgbKbsFark.ay = ay;
            sgbKbsFark.SaySorgu = saymanlikKod;
            sgbKbsFark.BirimKod = birimKod;

            sgbKbsFark.kurKod = kurKod;
            sgbKbsFark.fonKod = fonKod;
            sgbKbsFark.finKod = finKod;
            sgbKbsFark.ekoKod = ekoKod;
            sgbKbsFark.pebKod = pebKod;
            sgbKbsFark.pebli = pebli;

            string sorguTabloMuh = FarkMuhSorgu(sgbKbsFark, false);
            string sorguTabloOE = FarkMIFSorgu(sgbKbsFark, false);

            string sql = "SELECT " + alanlar + " FROM (" + sorguTabloMuh + ") M FULL OUTER JOIN  ( " + sorguTabloOE + " ) O ON M.MUHASEBEKOD=O.MUHASEBEKOD AND M.BIRIMKOD=O.BIRIMKOD AND M.KURKOD=O.KURKOD AND M.FONKOD=O.FONKOD AND M.FINKOD=O.FINKOD AND M.EKOKOD=O.EKOKOD ";
            if (pebli)
                sql += " AND M.PEBKOD=O.PEBKOD ";
            //string tabloAd;
            if (MuhBuyuk)
            {
                sql += "WHERE (M.GIDER>O.GIDER OR (M.GIDER>0 AND O.GIDER IS NULL)) ";
                //tabloAd = "M";
            }
            else
            {
                sql += "WHERE (M.GIDER<>O.GIDER OR (M.GIDER<>0 AND O.GIDER IS NULL) OR (M.GIDER IS NULL AND O.GIDER<>0)) ";
                //tabloAd = "O";
            }
            //if (!string.IsNullOrWhiteSpace(kurKod))
            //    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(tabloAd, "KURKOD", "LIKE", kurKod.Replace(".", "") + "%"));
            //if (!string.IsNullOrWhiteSpace(fonKod))
            //    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(tabloAd, "FONKOD", "LIKE", fonKod.Replace(".", "") + "%"));
            //if (!string.IsNullOrWhiteSpace(finKod))
            //    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(tabloAd, "FINKOD", "LIKE", finKod.Replace(".", "") + "%"));
            //if (!string.IsNullOrWhiteSpace(ekoKod))
            //    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(tabloAd, "EKOKOD", "LIKE", ekoKod.Replace(".", "") + "%"));

            sql += " " + ekKosul;
            sql += " " + siralama;
            return sql;
        }

        public IDataReader OdenekUstuAltiBul(SGBKBSFark sgbKbsFark)
        {
            string sorguTabloMUH = FarkMuhSorgu(sgbKbsFark, true);
            string sorguTabloG = FarkGONDSorgu(sgbKbsFark);
            string alanlar = "case when M.MUHASEBEKOD is NULL then O.MUHASEBEKOD else M.MUHASEBEKOD end, case when M.BIRIMKOD is NULL then O.BIRIMKOD else M.BIRIMKOD end, case when M.KURKOD is NULL then O.KURKOD else M.KURKOD end, case when M.FONKOD is NULL then O.FONKOD else M.FONKOD end, case when M.FINKOD is NULL then O.FINKOD else M.FINKOD end, case when M.EKOKOD is NULL then O.EKOKOD else M.EKOKOD end, case when M.PEBKOD is NULL then O.PEBKOD else M.PEBKOD end, M.GIDER, O.GONDERILEN";
            string sql = "SELECT " + alanlar + " FROM (" + sorguTabloG + ") O FULL OUTER JOIN  ( " + sorguTabloMUH + " ) M ON M.MUHASEBEKOD=O.MUHASEBEKOD AND M.BIRIMKOD=O.BIRIMKOD AND M.KURKOD=O.KURKOD AND M.FONKOD=O.FONKOD AND M.FINKOD=O.FINKOD AND M.EKOKOD=O.EKOKOD AND M.PEBKOD=O.PEBKOD ";
            sql += "WHERE (ISNULL(O.GONDERILEN, 0)<>ISNULL(M.GIDER, 0))";
            sql += " order by 1,2,3,4,5,6,7";
            return veriErisim.DataReaderDondur(sql);
        }

        public double BirimHarcamaSayOEBul(int yil, int ay, int saymanlikKod, string birimKod)
        {
            string ekoKodKisit = "";// AND M.EKOKOD NOT IN ('034204', '033201') ";
            string sql = FarkMuhMIFSorgu("COUNT(*)", yil, ay, saymanlikKod.ToString(), birimKod, true, ekoKodKisit, "", "", "", "", "", "", false, 1);
            return veriErisim.TekKolonSQLDbl(sql);
        }

        public IDataReader SGBKbsFarkBul(SGBKBSFark sgbKbsFark)
        {
            string pebAlan;
            if (sgbKbsFark.pebli)
                pebAlan = "case when M.PEBKOD is NULL then O.PEBKOD else M.PEBKOD end, ";
            else
                pebAlan = "";
            string sql = FarkMuhMIFSorgu($"case when M.MUHASEBEKOD is NULL then O.MUHASEBEKOD else M.MUHASEBEKOD end, case when M.BIRIMKOD is NULL then O.BIRIMKOD else M.BIRIMKOD end, case when M.KURKOD is NULL then O.KURKOD else M.KURKOD end, case when M.FONKOD is NULL then O.FONKOD else M.FONKOD end, case when M.FINKOD is NULL then O.FINKOD else M.FINKOD end, case when M.EKOKOD is NULL then O.EKOKOD else M.EKOKOD end, {pebAlan} M.GIDER, O.GIDER", sgbKbsFark.yil, sgbKbsFark.ay, sgbKbsFark.SaySorgu, sgbKbsFark.BirimKod, false, "", " order by 1,2,3,4,5,6", sgbKbsFark.kurKod, sgbKbsFark.fonKod, sgbKbsFark.finKod, sgbKbsFark.ekoKod, sgbKbsFark.pebKod, sgbKbsFark.pebli, sgbKbsFark.mode);
            //OrtakFonksiyonlar.HataDosyaYaz("SGBKbsFarkBul.txt", sql, false);
            return veriErisim.DataReaderDondur(sql);
        }

        public Sonuc OdemeEmriEvrakNoKaydet(int yil, string evrakNo, string odemeEmriNo, int durum)
        {
            Sonuc sonuc = new Sonuc(false, "", "");
            string sql = "";

            sql = "INSERT INTO MUHMIFEVRAK (YIL,EVRAKNO,BELGENO,DURUM) VALUES (";
            sql = sql + VeriErisim.DegerYap(yil) + ", ";
            sql = sql + VeriErisim.DegerYap(evrakNo) + ", ";
            sql = sql + VeriErisim.DegerYap(odemeEmriNo) + ", ";
            sql = sql + VeriErisim.DegerYap(durum) + ") ";

            sonuc = KomutCalistir(sql, 1);

            return sonuc;
        }

        public IDataReader MalHizmetYapimListele(int yil, string MalHizmetYapim, string saymanlikKodlar, string birimKodlar)
        {
            string alanlar = "MUHMIF.EKOKOD, ABSEKO.EKOAD, MUHMIFFORM.IHALE";
            string sql = "SELECT " + alanlar + ", SUM(MUHMIF.BORC - MUHMIF.ALACAK)";
            sql += " FROM MUHMIFFORM INNER JOIN ";
            sql += " MUHMIF ON MUHMIFFORM.YIL = MUHMIF.YIL AND MUHMIFFORM.BELGENO = MUHMIF.BELGENO INNER JOIN";
            sql += " ABSEKO ON MUHMIF.YIL = ABSEKO.YIL AND MUHMIF.EKOKOD = ABSEKO.EKOKOD";
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("MUHMIFFORM", "YIL", "=", yil));
            if (!string.IsNullOrWhiteSpace(MalHizmetYapim))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("MUHMIFFORM", "IHALE", "LIKE", "%" + MalHizmetYapim));
            else
            {
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.IslecBul("LEN") + "(MUHMIFFORM.IHALE) <= 3");
                sql = VeriErisim.KosulEkle(sql, "AND", "(MUHMIF.EKOKOD LIKE '03%' OR MUHMIF.EKOKOD LIKE '06%') ");
            }
            sql = VeriErisim.KosulEkle(sql, "AND", "(MUHMIFFORM.FORMDURUM <= 6) AND HESAPKOD LIKE '830%'");
            //if (!string.IsNullOrEmpty(kistasEkoKod))
            //    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("MUHMIF", "EKOKOD", "LIKE", kistasEkoKod + '%'));
            sql += " group by " + alanlar;
            sql += " order by 1,2,3";
            return veriErisim.DataReaderDondur(sql);
        }

        public string OdemeEmriEvrakNoOku(int yil, string belgeNo)
        {
            string sql = "SELECT EVRAKNO FROM MUHMIFEVRAK ";

            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", "YIL", "=", yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", "BELGENO", "=", belgeNo));

            return veriErisim.TekKolonSQLStr(sql);
        }

        public IDataReader MahsupEdilecekBelgeListele(OdemeEmriMIFForm oeForm, bool mahsupAlaniBosOlanlar)
        {
            string sql = "SELECT MUHMIFFORM.YIL,MUHASEBEKOD,BIRIMKOD,IADSOYAD,MUHMIFFORM.BELGENO,ISLEMTARIH,YEVMIYENO,YEVMIYETARIH,MUHMIF.HESAPKOD,MUHMIF.BORC, MUHMIF.ALACAK, MUHMIF.ALTHESAPKOD, (SELECT MIN(AD) FROM " + TABOrtak.DatabaseAdBul("HarcamaYonetimi.dbo.") + "HRCFIRMABILGISI where VERGINO=MUHMIF.ALTHESAPKOD), (SELECT MIN(AD + ' ' + SOYAD) FROM " + TABOrtak.DatabaseAdBul("Personel.dbo.") + "PERPersonel where TCKIMLIKNO=MUHMIF.ALTHESAPKOD) ";
            sql += " ,(SELECT sum(KTUTAR) FROM MUHKAPATMA AS KAPAT WHERE MUHMIFFORM.YIL=KAPAT.KYIL AND MUHMIFFORM.BELGENO=KAPAT.KBELGENO AND  MUHMIF.HESAPKOD=KAPAT.KHESAPKOD) AS KAPATMA,MUHMIF.DOVIZ,MUHMIF.DOVIZKURTUR,MUHMIF.DOVIZBORC,MUHMIF.DOVIZALACAK,MUHMIF.DOVIZKUR,MUHMIF.ACIKLAMA";
            sql += " FROM MUHMIFFORM INNER JOIN ";
            sql += " MUHMIF ON MUHMIFFORM.YIL = MUHMIF.YIL AND MUHMIFFORM.BELGENO = MUHMIF.BELGENO";
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("MUHMIFFORM", "YIL", "=", oeForm.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("MUHMIFFORM", "FORMDURUM", "<", 9));

            if (oeForm.ilgiliVD.Trim() != "")//Hesap Kodu vd de taşınıyor
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("MUHMIF", "HESAPKOD", "LIKE", oeForm.ilgiliVD.Replace(".", "").Trim() + "%"));
            if (oeForm.ilgiliNo.Trim() != "")
            {
                //string icKosul = "";
                //icKosul = VeriErisim.KosulEkle(icKosul, "AND", VeriErisim.KosulOl("MUHMIFFORM", "IKIMLIKNO", "=", oeForm.ilgiliNo));
                //icKosul = VeriErisim.KosulEkle(icKosul, "OR", VeriErisim.KosulOl("MUHMIF", "ALTHESAPKOD", "=", oeForm.ilgiliNo));
                //icKosul = "(" + icKosul + ")";
                //sql = VeriErisim.KosulEkle(sql, "AND", icKosul);

                //İlgilisi farklı althesabı farklı olduğu durumda ilgilisinden listelendiğinde sanki onun mahsup fişiymiş gibi gözüküyor.
                //Bu nedenle İlgiliden alınmayı (üst kısmı) kaldırdım. Bir kurum karşı çıkarsa eski haline gelebilir. Melih 03.03.2017 Manas için
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("MUHMIF", "ALTHESAPKOD", "=", oeForm.ilgiliNo));
            }
            if (oeForm.ilgiliAd.Trim() != "")
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("MUHMIFFORM", "IADSOYAD", "LIKE", "%" + oeForm.ilgiliAd + "%"));
            if (oeForm.muhasebe > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("MUHMIFFORM", "MUHASEBEKOD", "=", oeForm.muhasebe));
            if (oeForm.birim != "")
            {
                string icKosul = "";
                icKosul = VeriErisim.KosulEkle(icKosul, "AND", VeriErisim.KosulOl("MUHMIFFORM", "BIRIMKOD", "=", oeForm.birim));
                icKosul = VeriErisim.KosulEkle(icKosul, "OR", VeriErisim.KosulOl("MUHMIF", "DETAYBIRIM", "=", oeForm.birim));
                icKosul = "(" + icKosul + ")";
                sql = VeriErisim.KosulEkle(sql, "AND", icKosul);
            }
            if (mahsupAlaniBosOlanlar)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("MUHMIFFORM", "ILGILIBELGENO", "=", ""));

            if (oeForm.formDurum > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("MUHMIFFORM", "FORMDURUM", "=", oeForm.formDurum));

            return veriErisim.DataReaderDondur(sql);
        }

        public static string DETAYBIRIMKULLAN = null;
        public static bool ANABIRIM
        {
            get
            {
                if (DETAYBIRIMKULLAN == null)
                {
                    DETAYBIRIMKULLAN = TNS.DEG.Arac.Tanimla().DegiskenDegerBul(0, "/DETAYBIRIM");
                }
                return DETAYBIRIMKULLAN != "1";
            }
        }

        public string MahsupYapilmamisBelgeBul(int yil, string hesapKod, string altHesapKod, bool borclu, string belgeNo, double miktar)
        {
            string alanAd = "";
            if (borclu)
                alanAd = "BORC";
            else
                alanAd = "ALACAK";
            string altHesapKosul1 = "";
            string altHesapKosul2 = "";
            if (!string.IsNullOrWhiteSpace(altHesapKod))
            {
                altHesapKosul1 = " AND MUHMIF.ALTHESAPKOD = " + VeriErisim.DegerYap(altHesapKod);
                altHesapKosul2 = " AND MUHKAPATMA.KILGILIVNO = " + VeriErisim.DegerYap(altHesapKod);
            }

            hesapKod = VeriErisim.DegerYap(hesapKod.Replace(".", ""));
            string sql = "SELECT MIN(ACILIS.BELGENO) FROM";
            sql += " (SELECT MUHMIFFORM.BELGENO, SUM(MUHMIF." + alanAd + ") ACILAN";
            sql += " FROM MUHMIF INNER JOIN MUHMIFFORM ON MUHMIF.YIL = MUHMIFFORM.YIL AND MUHMIF.BELGENO=MUHMIFFORM.BELGENO";
            sql += " WHERE MUHMIFFORM.YIL=" + yil + " AND HESAPKOD=" + hesapKod + " AND MUHMIFFORM.FORMDURUM<9 AND MUHMIF." + alanAd + ">0" + altHesapKosul1;
            sql += " GROUP BY MUHMIFFORM.BELGENO) ACILIS ";
            sql += " LEFT OUTER JOIN ";
            sql += " (SELECT MUHKAPATMA.KBELGENO, SUM(KTUTAR) KAPATILAN FROM MUHKAPATMA INNER JOIN MUHMIFFORM ON MUHKAPATMA.YIL = MUHMIFFORM.YIL AND  MUHKAPATMA.BELGENO=MUHMIFFORM.BELGENO";
            sql += " WHERE  MUHKAPATMA.BELGENO <>" + VeriErisim.DegerYap(belgeNo) + " and MUHKAPATMA.KYIL=" + yil + " AND KHESAPKOD=" + hesapKod + " AND MUHMIFFORM.FORMDURUM<9" + altHesapKosul2;
            sql += " GROUP BY MUHKAPATMA.KBELGENO) KAPANIS ";
            sql += " ON ACILIS.BELGENO=KAPANIS.KBELGENO";
            sql += " WHERE ACILAN-CASE WHEN  KAPATILAN IS NULL THEN 0 ELSE KAPATILAN END >=" + VeriErisim.DegerYap(miktar);

            //string sql = "SELECT MAX(F.BELGENO) FROM MUHMIF D INNER JOIN MUHMIFFORM F ON D.YIL=F.YIL AND D.BELGENO=F.BELGENO";
            //sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("F", "YIL", "=", yil));
            //sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("D", (borclu ? "BORC" : "ALACAK"), ">", 0));
            //sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("D", "HESAPKOD", "=", hesapKod.Replace(".", "")));
            //if (!string.IsNullOrWhiteSpace(altHesapKod))
            //    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("D", "ALTHESAPKOD", "=", altHesapKod));
            //sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("F", "FORMDURUM", "<=", 9));
            //sql = VeriErisim.KosulEkle(sql, "AND", "(1 NOT IN (SELECT 1 FROM MUHKAPATMA WHERE MUHKAPATMA.YIL = D.YIL AND MUHKAPATMA.BELGENO = D.BELGENO AND KBELGENO <>" + VeriErisim.DegerYap(belgeNo) + "))");
            //OrtakFonksiyonlar.HataDosyaYaz("MahsupYapilmamisBelgeBul.txt", sql, false);
            return veriErisim.TekKolonSQLStr(sql);
        }

        public IDataReader MahsupSatiriBul(int yil, string belgeNo, string hesapKod, string altHesapKod, bool borclu, double miktar)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar("D", ALANLARMIFD) + " FROM MUHMIF D ";
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("D", "YIL", "=", yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("D", "BELGENO", "=", belgeNo));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("D", (borclu ? "BORC" : "ALACAK"), ">", 0));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("D", "HESAPKOD", "=", hesapKod.Replace(".", "")));
            if (!string.IsNullOrWhiteSpace(altHesapKod))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("D", "ALTHESAPKOD", "=", altHesapKod));
            return veriErisim.DataReaderDondur(sql);
        }

        public double SatirToplamBul(int yil, string belgeNo, string hesapKod, string altHesapKod)
        {
            string sql = "SELECT SUM(BORC-ALACAK) FROM MUHMIF";
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", "YIL", "=", yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", "BELGENO", "=", belgeNo));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", "HESAPKOD", "=", hesapKod.Replace(".", "")));

            if (altHesapKod != "")
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", "ALTHESAPKOD", "=", altHesapKod.Replace(".", "")));

            return veriErisim.TekKolonSQLDbl(sql);
        }

        public double SatirToplamBulDoviz(int yil, string belgeNo, string hesapKod, string altHesapKod)
        {
            string sql = "SELECT SUM(DOVIZBORC-DOVIZALACAK) FROM MUHMIF";
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", "YIL", "=", yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", "BELGENO", "=", belgeNo));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", "HESAPKOD", "=", hesapKod.Replace(".", "")));

            if (altHesapKod != "")
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", "ALTHESAPKOD", "=", altHesapKod.Replace(".", "")));

            return veriErisim.TekKolonSQLDbl(sql);
        }

        public Sonuc AltHesapDegistir(int yil, string belgeNo, int satirNo, string altHesapKod)
        {
            string sql = "UPDATE " + TABLOADMIFD;
            sql += " SET " + VeriErisim.KosulOl("", ALANLARMIFD[(int)ENUMAlanlarMIFD.ALTHESAPKOD], "=", altHesapKod);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARMIFD[(int)ENUMAlanlarMIFD.YIL], "=", yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARMIFD[(int)ENUMAlanlarMIFD.BELGENO], "=", belgeNo));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARMIFD[(int)ENUMAlanlarMIFD.SIRA], "=", satirNo));
            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// KBSs the aktarim listele.
        /// </summary>
        /// <param name="yil">The yil.</param>
        /// <returns></returns>
        public IDataReader KBSAktarimListele(int yil)
        {
            string sql = "SELECT MUHHESAPPLANI.YIL, MUHHESAPPLANI.HESAPKOD, MUHHESAPPLANI.ACIKLAMA, MUHKBSAKTARIM.KOD, MUHKBSAKTARIM.GOSTER, MUHKBSAKTARIM.HESAPUZUNLUK, MUHKBSAKTARIM.FONUZUNLUK, ";
            sql += " MUHKBSAKTARIM.FINUZUNLUK";
            sql += " FROM MUHHESAPPLANI LEFT OUTER JOIN";
            sql += " MUHKBSAKTARIM ON MUHHESAPPLANI.YIL = MUHKBSAKTARIM.YIL AND MUHHESAPPLANI.HESAPKOD = MUHKBSAKTARIM.HESAPKOD";

            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("MUHHESAPPLANI", "YIL", "=", yil));
            sql = VeriErisim.KosulEkle(sql, "AND", "LEN(MUHHESAPPLANI.HESAPKOD) = 3");
            sql += " ORDER BY MUHHESAPPLANI.HESAPKOD";

            return veriErisim.DataReaderDondur(sql);
        }

        public IDataReader YillikAylikDagilimHesapKod(OdemeEmriMIFForm kriter, string hesapKod, int seviye)
        {
            string alan = "MUHMIFFORM.ISLEMTARIH";
            if (kriter.formDurum > 0)
                alan = "MUHMIFFORM.YEVMIYETARIH";

            string hesapAlan = "SUBSTRING(MUHMIF.HESAPKOD,1," + seviye + ")";

            string sql = "SELECT YEAR(" + alan + "), MONTH(" + alan + "), " + hesapAlan + ", MUHHESAPPLANI.ACIKLAMA, SUM(MUHMIF.BORC)-SUM(MUHMIF.ALACAK)";
            sql += " FROM  MUHMIFFORM INNER JOIN";
            sql += " MUHMIF ON MUHMIFFORM.YIL = MUHMIF.YIL AND MUHMIFFORM.BELGENO = MUHMIF.BELGENO LEFT OUTER JOIN";
            sql += " MUHHESAPPLANI ON MUHMIF.YIL = MUHHESAPPLANI.YIL AND " + hesapAlan + " = MUHHESAPPLANI.HESAPKOD";
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", "MUHMIF.YIL", ">=", kriter.yil - 2));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", "MUHMIF.HESAPKOD", "LIKE", hesapKod + "%"));

            if (kriter.fisTur != null && kriter.fisTur.Length > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", TABMizanBilanco.SorguOl("MUHMIFFORM.FISTUR", kriter.fisTur));
            else
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", "MUHMIFFORM.FISTUR", "<", 2));

            if (kriter.muhasebe > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", "MUHMIFFORM.MUHASEBEKOD", "=", kriter.muhasebe));

            if (kriter.birim != "")
            {
                string[] birimler = kriter.birim.Split(';');
                string birimKosul = "";
                for (int i = 0; i < birimler.Length; i++)
                {
                    if (ANABIRIM)
                        birimKosul = VeriErisim.KosulEkle(birimKosul, "OR", VeriErisim.KosulOl("", ALANLARMIFF[(int)ENUMAlanlarMIFF.BIRIMKOD], "=", birimler[i]));
                    else
                        birimKosul = VeriErisim.KosulEkle(birimKosul, "OR", VeriErisim.KosulOl("", ALANLARMIFD[(int)ENUMAlanlarMIFD.DETAYBIRIM], "=", birimler[i]));
                }

                if (birimKosul != "")
                    sql += " AND (" + birimKosul + ")";
            }

            if (kriter.formDurum > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", "MUHMIFFORM.FORMDURUM", "=", 6));
            else
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", "MUHMIFFORM.FORMDURUM", "<", 9));

            sql += " GROUP BY YEAR(" + alan + "), MONTH(" + alan + "), " + hesapAlan + ", MUHHESAPPLANI.ACIKLAMA";
            sql += " ORDER BY " + hesapAlan + ",MONTH(" + alan + "),YEAR(" + alan + ")";

            return veriErisim.DataReaderDondur(sql);
        }

        public IDataReader YillikAylikTarihselDagilimHesapKod(OdemeEmriMIFForm kriter, string hesapKod, int seviye)
        {
            string alan = "MUHMIFFORM.ISLEMTARIH";
            if (kriter.formDurum > 0)
                alan = "MUHMIFFORM.YEVMIYETARIH";

            string hesapAlan = "SUBSTRING(MUHMIF.HESAPKOD,1," + seviye + ")";

            string sql = "SELECT YEAR(" + alan + "), " + hesapAlan + ", MUHHESAPPLANI.ACIKLAMA, SUM(MUHMIF.BORC)-SUM(MUHMIF.ALACAK)";
            sql += " FROM  MUHMIFFORM INNER JOIN";
            sql += " MUHMIF ON MUHMIFFORM.YIL = MUHMIF.YIL AND MUHMIFFORM.BELGENO = MUHMIF.BELGENO LEFT OUTER JOIN";
            sql += " MUHHESAPPLANI ON MUHMIF.YIL = MUHHESAPPLANI.YIL AND " + hesapAlan + " = MUHHESAPPLANI.HESAPKOD";
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", "MUHMIF.YIL", "=", kriter.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", "MUHMIF.HESAPKOD", "LIKE", hesapKod + "%"));

            if (kriter.fisTur != null && kriter.fisTur.Length > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", TABMizanBilanco.SorguOl("MUHMIFFORM.FISTUR", kriter.fisTur));
            else
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", "MUHMIFFORM.FISTUR", "<", 2));

            if (kriter.muhasebe > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", "MUHMIFFORM.MUHASEBEKOD", "=", kriter.muhasebe));

            if (kriter.birim != "")
            {
                string[] birimler = kriter.birim.Split(';');
                string birimKosul = "";
                for (int i = 0; i < birimler.Length; i++)
                {
                    if (ANABIRIM)
                        birimKosul = VeriErisim.KosulEkle(birimKosul, "OR", VeriErisim.KosulOl("", ALANLARMIFF[(int)ENUMAlanlarMIFF.BIRIMKOD], "=", birimler[i]));
                    else
                        birimKosul = VeriErisim.KosulEkle(birimKosul, "OR", VeriErisim.KosulOl("", ALANLARMIFD[(int)ENUMAlanlarMIFD.DETAYBIRIM], "=", birimler[i]));
                }

                if (birimKosul != "")
                    sql += " AND (" + birimKosul + ")";
            }

            if (kriter.sorguIslemTarih != "")
            {
                string[] t = kriter.sorguIslemTarih.Split(';');
                YilKontrol(ref t, kriter.yil);

                System.DateTime d1;
                System.DateTime d2;
                if (t.Length == 1 && t[0] != "" || (t.Length == 2 && t[0] != "" && t[1] == ""))
                {
                    d1 = Convert.ToDateTime(new TNSDateTime(t[0]).ToString());
                    d2 = Convert.ToDateTime(new TNSDateTime(t[0]).ToString()).AddDays(1).AddSeconds(-1);
                    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADMIFF, ALANLARMIFF[(int)ENUMAlanlarMIFF.ISLEMTARIH], ">=", new TNSDateTime(d1)));
                    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADMIFF, ALANLARMIFF[(int)ENUMAlanlarMIFF.ISLEMTARIH], "<=", new TNSDateTime(d2)));
                }
                else if (t.Length == 2 && t[0] != "" && t[1] != "")
                {
                    d1 = Convert.ToDateTime(new TNSDateTime(t[0]).ToString());
                    d2 = Convert.ToDateTime(new TNSDateTime(t[1]).ToString()).AddDays(1).AddSeconds(-1);
                    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADMIFF, ALANLARMIFF[(int)ENUMAlanlarMIFF.ISLEMTARIH], ">=", new TNSDateTime(d1)));
                    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADMIFF, ALANLARMIFF[(int)ENUMAlanlarMIFF.ISLEMTARIH], "<=", new TNSDateTime(d2)));
                }
                else if (t.Length == 2 && t[0] == "" && t[1] != "")
                {
                    d2 = Convert.ToDateTime(new TNSDateTime(t[1]).ToString()).AddDays(1).AddSeconds(-1);
                    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADMIFF, ALANLARMIFF[(int)ENUMAlanlarMIFF.ISLEMTARIH], "<=", new TNSDateTime(d2)));
                }
            }

            if (kriter.sorguYEVYevTarih != "")
            {
                string[] t = kriter.sorguYEVYevTarih.Split(';');
                YilKontrol(ref t, kriter.yil);

                System.DateTime d1;
                System.DateTime d2;
                if (t.Length == 1 && t[0] != "" || (t.Length == 2 && t[0] != "" && t[1] == ""))
                {
                    d1 = Convert.ToDateTime(new TNSDateTime(t[0]).ToString());
                    d2 = Convert.ToDateTime(new TNSDateTime(t[0]).ToString()).AddDays(1).AddSeconds(-1);
                    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADMIFF, ALANLARMIFF[(int)ENUMAlanlarMIFF.YEVMIYETARIH], ">=", new TNSDateTime(d1)));
                    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADMIFF, ALANLARMIFF[(int)ENUMAlanlarMIFF.YEVMIYETARIH], "<=", new TNSDateTime(d2)));
                }
                else if (t.Length == 2 && t[0] != "" && t[1] != "")
                {
                    d1 = Convert.ToDateTime(new TNSDateTime(t[0]).ToString());
                    d2 = Convert.ToDateTime(new TNSDateTime(t[1]).ToString()).AddDays(1).AddSeconds(-1);
                    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADMIFF, ALANLARMIFF[(int)ENUMAlanlarMIFF.YEVMIYETARIH], ">=", new TNSDateTime(d1)));
                    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADMIFF, ALANLARMIFF[(int)ENUMAlanlarMIFF.YEVMIYETARIH], "<=", new TNSDateTime(d2)));
                }
                else if (t.Length == 2 && t[0] == "" && t[1] != "")
                {
                    d2 = Convert.ToDateTime(new TNSDateTime(t[1]).ToString()).AddDays(1).AddSeconds(-1);
                    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADMIFF, ALANLARMIFF[(int)ENUMAlanlarMIFF.YEVMIYETARIH], "<=", new TNSDateTime(d2)));
                }
            }


            if (kriter.formDurum > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", "MUHMIFFORM.FORMDURUM", "=", 6));
            else
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", "MUHMIFFORM.FORMDURUM", "<", 9));

            sql += " GROUP BY YEAR(" + alan + "), " + hesapAlan + ", MUHHESAPPLANI.ACIKLAMA";
            sql += " ORDER BY " + hesapAlan + ",YEAR(" + alan + ")";

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Ayliks the dagilim hesap kod.
        /// </summary>
        /// <param name="yil">The yil.</param>
        /// <param name="muhasebe">The muhasebe.</param>
        /// <param name="hesapKod">The hesap kod.</param>
        /// <param name="durum">The durum.</param>
        /// <returns></returns>
        public IDataReader AylikDagilimHesapKod(OdemeEmriMIFForm kriter, string hesapKod, int seviye)
        {
            string alan = "MUHMIFFORM.ISLEMTARIH";
            if (kriter.formDurum > 0)
                alan = "MUHMIFFORM.YEVMIYETARIH";

            string hesapAlan = "SUBSTRING(MUHMIF.HESAPKOD,1," + seviye + ")";

            string sql = "SELECT MONTH(" + alan + "), " + hesapAlan + ", MUHHESAPPLANI.ACIKLAMA, SUM(MUHMIF.BORC)-SUM(MUHMIF.ALACAK)";
            sql += " FROM  MUHMIFFORM INNER JOIN";
            sql += " MUHMIF ON MUHMIFFORM.YIL = MUHMIF.YIL AND MUHMIFFORM.BELGENO = MUHMIF.BELGENO LEFT OUTER JOIN";
            sql += " MUHHESAPPLANI ON MUHMIF.YIL = MUHHESAPPLANI.YIL AND " + hesapAlan + " = MUHHESAPPLANI.HESAPKOD";
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", "MUHMIF.YIL", "=", kriter.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", "MUHMIF.HESAPKOD", "LIKE", hesapKod + "%"));

            if (kriter.fisTur != null && kriter.fisTur.Length > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", TABMizanBilanco.SorguOl("MUHMIFFORM.FISTUR", kriter.fisTur));
            else
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", "MUHMIFFORM.FISTUR", "<", 2));

            if (kriter.muhasebe > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", "MUHMIFFORM.MUHASEBEKOD", "=", kriter.muhasebe));

            if (kriter.birim != "")
            {
                string[] birimler = kriter.birim.Split(';');
                string birimKosul = "";
                for (int i = 0; i < birimler.Length; i++)
                {
                    if (ANABIRIM)
                        birimKosul = VeriErisim.KosulEkle(birimKosul, "OR", VeriErisim.KosulOl("", ALANLARMIFF[(int)ENUMAlanlarMIFF.BIRIMKOD], "=", birimler[i]));
                    else
                        birimKosul = VeriErisim.KosulEkle(birimKosul, "OR", VeriErisim.KosulOl("", ALANLARMIFD[(int)ENUMAlanlarMIFD.DETAYBIRIM], "=", birimler[i]));
                }

                if (birimKosul != "")
                    sql += " AND (" + birimKosul + ")";
            }

            if (kriter.formDurum > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", "MUHMIFFORM.FORMDURUM", "=", 6));
            else
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", "MUHMIFFORM.FORMDURUM", "<", 9));

            sql += " GROUP BY MONTH(" + alan + "), " + hesapAlan + ", MUHHESAPPLANI.ACIKLAMA";
            sql += " ORDER BY " + hesapAlan + ",MONTH(" + alan + ")";

            return veriErisim.DataReaderDondur(sql);
        }

        public IDataReader YillikDagilimHesapKod(OdemeEmriMIFForm kriter, string hesapKod, int seviye)
        {
            string alan = "MUHMIFFORM.ISLEMTARIH";
            if (kriter.formDurum > 0)
                alan = "MUHMIFFORM.YEVMIYETARIH";

            string hesapAlan = "SUBSTRING(MUHMIF.HESAPKOD,1," + seviye + ")";

            string sql = "SELECT YEAR(" + alan + "), " + hesapAlan + ", MUHHESAPPLANI.ACIKLAMA, SUM(MUHMIF.BORC)-SUM(MUHMIF.ALACAK)";
            sql += " FROM  MUHMIFFORM INNER JOIN";
            sql += " MUHMIF ON MUHMIFFORM.YIL = MUHMIF.YIL AND MUHMIFFORM.BELGENO = MUHMIF.BELGENO LEFT OUTER JOIN";
            sql += " MUHHESAPPLANI ON MUHMIF.YIL = MUHHESAPPLANI.YIL AND " + hesapAlan + " = MUHHESAPPLANI.HESAPKOD";
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", "MUHMIF.YIL", ">", kriter.yil - 5));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", "MUHMIF.HESAPKOD", "LIKE", hesapKod + "%"));

            if (kriter.fisTur != null && kriter.fisTur.Length > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", TABMizanBilanco.SorguOl("MUHMIFFORM.FISTUR", kriter.fisTur));
            else
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", "MUHMIFFORM.FISTUR", "<", 2));

            if (kriter.muhasebe > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", "MUHMIFFORM.MUHASEBEKOD", "=", kriter.muhasebe));

            if (kriter.birim != "")
            {
                string[] birimler = kriter.birim.Split(';');
                string birimKosul = "";
                for (int i = 0; i < birimler.Length; i++)
                {
                    if (ANABIRIM)
                        birimKosul = VeriErisim.KosulEkle(birimKosul, "OR", VeriErisim.KosulOl("", ALANLARMIFF[(int)ENUMAlanlarMIFF.BIRIMKOD], "=", birimler[i]));
                    else
                        birimKosul = VeriErisim.KosulEkle(birimKosul, "OR", VeriErisim.KosulOl("", ALANLARMIFD[(int)ENUMAlanlarMIFD.DETAYBIRIM], "=", birimler[i]));
                }

                if (birimKosul != "")
                    sql += " AND (" + birimKosul + ")";
            }

            if (kriter.formDurum > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", "MUHMIFFORM.FORMDURUM", "=", 6));
            else
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", "MUHMIFFORM.FORMDURUM", "<", 9));

            sql += " GROUP BY YEAR(" + alan + "), " + hesapAlan + ", MUHHESAPPLANI.ACIKLAMA";
            sql += " ORDER BY " + hesapAlan + ",YEAR(" + alan + ")";

            return veriErisim.DataReaderDondur(sql);
        }

        public IDataReader BirimDagilimHesapKod(OdemeEmriMIFForm kriter, string hesapKod, int seviye)
        {
            //string alan = "MUHMIFFORM.ISLEMTARIH";
            //if (kriter.formDurum > 0)
            //    alan = "MUHMIFFORM.YEVMIYETARIH";

            string hesapAlan = "SUBSTRING(MUHMIF.HESAPKOD,1," + seviye + ")";

            string sql = "SELECT MUHMIF.DETAYBIRIM, " + hesapAlan + ", MUHHESAPPLANI.ACIKLAMA, SUM(MUHMIF.BORC)-SUM(MUHMIF.ALACAK)";
            sql += " FROM  MUHMIFFORM INNER JOIN";
            sql += " MUHMIF ON MUHMIFFORM.YIL = MUHMIF.YIL AND MUHMIFFORM.BELGENO = MUHMIF.BELGENO LEFT OUTER JOIN";
            sql += " MUHHESAPPLANI ON MUHMIF.YIL = MUHHESAPPLANI.YIL AND " + hesapAlan + " = MUHHESAPPLANI.HESAPKOD";
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", "MUHMIF.YIL", "=", kriter.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", "MUHMIF.HESAPKOD", "LIKE", hesapKod + "%"));

            if (kriter.fisTur != null && kriter.fisTur.Length > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", TABMizanBilanco.SorguOl("MUHMIFFORM.FISTUR", kriter.fisTur));
            else
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", "MUHMIFFORM.FISTUR", "<", 2));

            if (kriter.muhasebe > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", "MUHMIFFORM.MUHASEBEKOD", "=", kriter.muhasebe));

            if (kriter.birim != "")
            {
                string[] birimler = kriter.birim.Split(';');
                string birimKosul = "";
                for (int i = 0; i < birimler.Length; i++)
                {
                    if (ANABIRIM)
                        birimKosul = VeriErisim.KosulEkle(birimKosul, "OR", VeriErisim.KosulOl("", ALANLARMIFF[(int)ENUMAlanlarMIFF.BIRIMKOD], "=", birimler[i]));
                    else
                        birimKosul = VeriErisim.KosulEkle(birimKosul, "OR", VeriErisim.KosulOl("", ALANLARMIFD[(int)ENUMAlanlarMIFD.DETAYBIRIM], "=", birimler[i]));
                }

                if (birimKosul != "")
                    sql += " AND (" + birimKosul + ")";
            }

            if (kriter.formDurum > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", "MUHMIFFORM.FORMDURUM", "=", 6));
            else
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", "MUHMIFFORM.FORMDURUM", "<", 9));


            if (kriter.sorguIslemTarih != "")
            {
                string[] t = kriter.sorguIslemTarih.Split(';');
                YilKontrol(ref t, kriter.yil);

                System.DateTime d1;
                System.DateTime d2;
                if (t.Length == 1 && t[0] != "" || (t.Length == 2 && t[0] != "" && t[1] == ""))
                {
                    d1 = Convert.ToDateTime(new TNSDateTime(t[0]).ToString());
                    d2 = Convert.ToDateTime(new TNSDateTime(t[0]).ToString()).AddDays(1).AddSeconds(-1);
                    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADMIFF, ALANLARMIFF[(int)ENUMAlanlarMIFF.ISLEMTARIH], ">=", new TNSDateTime(d1)));
                    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADMIFF, ALANLARMIFF[(int)ENUMAlanlarMIFF.ISLEMTARIH], "<=", new TNSDateTime(d2)));
                }
                else if (t.Length == 2 && t[0] != "" && t[1] != "")
                {
                    d1 = Convert.ToDateTime(new TNSDateTime(t[0]).ToString());
                    d2 = Convert.ToDateTime(new TNSDateTime(t[1]).ToString()).AddDays(1).AddSeconds(-1);
                    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADMIFF, ALANLARMIFF[(int)ENUMAlanlarMIFF.ISLEMTARIH], ">=", new TNSDateTime(d1)));
                    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADMIFF, ALANLARMIFF[(int)ENUMAlanlarMIFF.ISLEMTARIH], "<=", new TNSDateTime(d2)));
                }
                else if (t.Length == 2 && t[0] == "" && t[1] != "")
                {
                    d2 = Convert.ToDateTime(new TNSDateTime(t[1]).ToString()).AddDays(1).AddSeconds(-1);
                    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADMIFF, ALANLARMIFF[(int)ENUMAlanlarMIFF.ISLEMTARIH], "<=", new TNSDateTime(d2)));
                }
            }

            if (kriter.sorguYEVYevTarih != "")
            {
                string[] t = kriter.sorguYEVYevTarih.Split(';');
                YilKontrol(ref t, kriter.yil);

                System.DateTime d1;
                System.DateTime d2;
                if (t.Length == 1 && t[0] != "" || (t.Length == 2 && t[0] != "" && t[1] == ""))
                {
                    d1 = Convert.ToDateTime(new TNSDateTime(t[0]).ToString());
                    d2 = Convert.ToDateTime(new TNSDateTime(t[0]).ToString()).AddDays(1).AddSeconds(-1);
                    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADMIFF, ALANLARMIFF[(int)ENUMAlanlarMIFF.YEVMIYETARIH], ">=", new TNSDateTime(d1)));
                    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADMIFF, ALANLARMIFF[(int)ENUMAlanlarMIFF.YEVMIYETARIH], "<=", new TNSDateTime(d2)));
                }
                else if (t.Length == 2 && t[0] != "" && t[1] != "")
                {
                    d1 = Convert.ToDateTime(new TNSDateTime(t[0]).ToString());
                    d2 = Convert.ToDateTime(new TNSDateTime(t[1]).ToString()).AddDays(1).AddSeconds(-1);
                    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADMIFF, ALANLARMIFF[(int)ENUMAlanlarMIFF.YEVMIYETARIH], ">=", new TNSDateTime(d1)));
                    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADMIFF, ALANLARMIFF[(int)ENUMAlanlarMIFF.YEVMIYETARIH], "<=", new TNSDateTime(d2)));
                }
                else if (t.Length == 2 && t[0] == "" && t[1] != "")
                {
                    d2 = Convert.ToDateTime(new TNSDateTime(t[1]).ToString()).AddDays(1).AddSeconds(-1);
                    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADMIFF, ALANLARMIFF[(int)ENUMAlanlarMIFF.YEVMIYETARIH], "<=", new TNSDateTime(d2)));
                }
            }



            sql += " GROUP BY MUHMIF.DETAYBIRIM, " + hesapAlan + ", MUHHESAPPLANI.ACIKLAMA";
            sql += " ORDER BY " + hesapAlan + ",MUHMIF.DETAYBIRIM";

            return veriErisim.DataReaderDondur(sql);
        }

        public IDataReader TasinirRaporu(int yil, int muhasebe, string birim, string hesapKod, int durum)
        {
            string sql = "SELECT YIL,MUHASEBEKOD,HARCAMABIRIMKOD,FISNO,FISTARIH,AMBARKOD,ISLEMTIPIKOD,NEREDENGELDI,ISLEMTARIH,HESAPPLANKOD,MIKTAR,BIRIMFIYAT,MIKTAR*BIRIMFIYAT,BIRIMFIYATKDVLI,ACIKLAMA,BELGENO FROM TASISLEMLER";
            sql += " WHERE YIL=" + yil + " AND ";
            sql += " BELGENO IN (SELECT F.BELGENO from MUHMIFFORM F,MUHMIF D WHERE F.YIL=D.YIL AND F.BELGENO=D.BELGENO AND ";
            sql += " (D.HESAPKOD LIKE '15%' OR D.HESAPKOD LIKE '25%') AND";
            sql += " F.YIL=" + yil + " AND F.FISTUR=0 AND F.BELGENO IN (";
            sql += " SELECT F.BELGENO             from MUHMIFFORM F,MUHMIF D WHERE F.YIL=D.YIL AND F.BELGENO=D.BELGENO AND ";
            sql += " (D.HESAPKOD LIKE '830%')";
            sql += " AND F.YIL=" + yil;
            sql += " AND D.HESAPKOD LIKE '" + hesapKod + "%' ";

            if (birim != "")
            {
                string[] birimler = birim.Split(';');
                string birimKosul = "";
                for (int i = 0; i < birimler.Length; i++)
                {
                    birimKosul = VeriErisim.KosulEkle(birimKosul, "OR", VeriErisim.KosulOl("D", "DETAYBIRIM", "=", birimler[i]));
                }

                if (birimKosul != "")
                    sql += " AND (" + birimKosul + ")";
            }

            sql += " ))";
            sql += " ORDER BY YIL,MUHASEBEKOD,HARCAMABIRIMKOD,FISNO";

            return veriErisim.DataReaderDondur(sql);
        }

        public IDataReader KurFarkiListe(int yil, TNSDateTime tarih)
        {
            string sql = "SELECT DETAYBIRIM,MUHMIF.KURKOD,FONKOD,FINKOD,EKOKOD,PEBKOD, HESAPKOD,ALTHESAPKOD,MUHMIF.DOVIZ,SUM(BORC) BORC,SUM(ALACAK) ALACAK,SUM(DOVIZBORC) DBORC,SUM(DOVIZALACAK) DALACAK,SUM(BORC)-SUM(ALACAK)BAKIYE,(SUM(DOVIZBORC)-SUM(DOVIZALACAK)) BAKIYEDOVIZ";
            sql += " , CONVERT(MONEY,round(((SUM(BORC)-SUM(ALACAK))-ROUND((SUM(DOVIZBORC)-SUM(DOVIZALACAK))*(SELECT EFEKTIFALIS FROM " + TABOrtak.DatabaseAdBul("HarcamaYonetimi.dbo.") + "HRCMERKEZBANKKUR WHERE PARABIRIMI=MUHMIF.DOVIZ AND KURTARIH='" + tarih.Oku().ToString("yyyy-MM-dd") + "'),2)),2)) AS KURFARKI";
            sql += " FROM MUHMIF,MUHMIFFORM";
            sql += " WHERE MUHMIF.YIL=MUHMIFFORM.YIL AND MUHMIFFORM.BELGENO=MUHMIF.BELGENO AND (FORMDURUM=5 OR FORMDURUM=6) AND MUHMIFFORM.YIL=" + yil;
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("MUHMIFFORM", "ISLEMTARIH", "<=", tarih));
            sql += " AND HESAPKOD IN (SELECT HESAPKOD FROM MUHHESAPPLANI WHERE DOVIZ<>'' AND YIL=MUHMIFFORM.YIL) ";// AND HESAPKOD=REPLACE('3300702','.','')";
            sql += " GROUP BY DETAYBIRIM,MUHMIF.KURKOD,FONKOD,FINKOD,EKOKOD,PEBKOD,HESAPKOD,ALTHESAPKOD,MUHMIF.DOVIZ";
            sql += " ORDER BY HESAPKOD,DETAYBIRIM,MUHMIF.KURKOD,FONKOD,FINKOD,EKOKOD,PEBKOD,ALTHESAPKOD,MUHMIF.DOVIZ";

            return veriErisim.DataReaderDondur(sql);
        }

        public Sonuc KBSAktarimKaydet(KBSAktarim kbs)
        {
            string sql = "";
            sql = "INSERT INTO MUHKBSAKTARIM (KOD,YIL,GOSTER,HESAPKOD,HESAPADI,HESAPUZUNLUK,FONUZUNLUK,FINUZUNLUK) VALUES (";
            sql += VeriErisim.DegerYap(kbs.kod) + ",";
            sql += VeriErisim.DegerYap(kbs.yil) + ",";
            sql += VeriErisim.DegerYap(kbs.goster) + ",";
            sql += VeriErisim.DegerYap(kbs.hesapKod) + ",";
            sql += VeriErisim.DegerYap(kbs.hesapAd) + ",";
            sql += VeriErisim.DegerYap(kbs.hesapUzunluk) + ",";
            sql += VeriErisim.DegerYap(kbs.fonUzunluk) + ",";
            sql += VeriErisim.DegerYap(kbs.finUzunluk) + ")";

            return KomutCalistir(sql, 1);
        }

        public Sonuc KBSAktarimSil(int yil)
        {
            string sql = "DELETE FROM MUHKBSAKTARIM";
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", "YIL", "=", yil));

            return KomutCalistir(sql, 0);
        }

        public Sonuc KBSAktarimDetayKaydet(KBSAktarim kbs)
        {
            string sql = "";
            sql = "INSERT INTO MUHKBSAKTARIMDETAY (KOD,YIL,HESAPKOD,HESAPADI) VALUES (";
            sql += VeriErisim.DegerYap(kbs.kod) + ",";
            sql += VeriErisim.DegerYap(kbs.yil) + ",";
            sql += VeriErisim.DegerYap(kbs.hesapKod) + ",";
            sql += VeriErisim.DegerYap(kbs.hesapAd) + ")";

            return KomutCalistir(sql, 1);
        }

        public Sonuc KBSAktarimDetaySil(int yil)
        {
            string sql = "DELETE FROM MUHKBSAKTARIMDETAY";
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", "YIL", "=", yil));

            return KomutCalistir(sql, 0);
        }

        public IDataReader KBSAktarimDetayListele(int yil)
        {
            string sql = "SELECT MUHHESAPPLANI.YIL, MUHHESAPPLANI.HESAPKOD, MUHHESAPPLANI.ACIKLAMA, MUHKBSAKTARIMDETAY.KOD ";
            sql += " FROM MUHHESAPPLANI INNER JOIN";
            sql += " MUHKBSAKTARIMDETAY ON MUHHESAPPLANI.YIL = MUHKBSAKTARIMDETAY.YIL AND MUHHESAPPLANI.HESAPKOD = MUHKBSAKTARIMDETAY.HESAPKOD";
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("MUHHESAPPLANI", "YIL", "=", yil));
            sql += " ORDER BY MUHHESAPPLANI.HESAPKOD";

            return veriErisim.DataReaderDondur(sql);
        }

        public Sonuc IstatistikKodAta(OdemeEmriMIFForm form, int ist1, int ist2)
        {
            string degerAta = "";
            if (ist1 > 0)
                degerAta = ALANLARMIFF[(int)ENUMAlanlarMIFF.ISTKOD1] + "=" + VeriErisim.DegerYap(ist1);
            if (ist2 > 0)
            {
                if (degerAta != "") degerAta += ",";
                degerAta += ALANLARMIFF[(int)ENUMAlanlarMIFF.ISTKOD2] + "=" + VeriErisim.DegerYap(ist2);
            }

            if (degerAta != "")
            {
                string sql = "UPDATE " + TABLOADMIFF + " SET " + degerAta;
                sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARMIFF[(int)ENUMAlanlarMIFF.YIL], "=", form.yil));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARMIFF[(int)ENUMAlanlarMIFF.BELGENO], "=", form.belgeNo));
                return KomutCalistir(sql, 1);
            }
            else
                return new Sonuc();
        }

        /// <summary>
        /// Parametre olarak gelen bilgileri veri tabanına ekleyen sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="form">Eklenecek kayıt bilgisini tutan nesne</param>
        /// <returns>guncelle komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc GuncelleAltHesapKod(string yeniVergiNo, string eskiVergiNo)
        {
            string sql = "UPDATE " + TABLOADMIFD;
            sql += " SET " + VeriErisim.KosulOl("", ALANLARMIFD[(int)ENUMAlanlarMIFD.ALTHESAPKOD], "=", yeniVergiNo);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARMIFD[(int)ENUMAlanlarMIFD.ALTHESAPKOD], "=", eskiVergiNo));
            return KomutCalistir(sql, 0);
        }

        /// <summary>
        /// Parametre olarak gelen bilgileri veri tabanına ekleyen sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="form">Silinecek kayıt bilgisini tutan nesne</param>
        /// <returns>Sil komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc SilHRCFirma(string vergiNo)
        {
            string sql = " DELETE FROM " + TABOrtak.DatabaseAdBul("HarcamaYonetimi.dbo.") + "HRCFIRMABILGISI ";
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", "VERGINO", "=", vergiNo));
            return KomutCalistir(sql, 0);
        }


        public IDataReader MukerrerBelgeKontrol(int yil, int muhasebeKod, string birimKod, string ilgiliKod, string hesapKodlar, double borc, double alacak)
        {
            string sql = " SELECT A.YIL,A.BELGENO FROM MUHMIFFORM A,MUHMIF B ";
            sql += " WHERE ";
            sql += " A.YIL=B.YIL AND A.BELGENO=B.BELGENO AND A.FORMDURUM<9 ";
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("A", "YIL", "=", yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("A", "MUHASEBEKOD", "=", muhasebeKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("A", "BIRIMKOD", "=", birimKod));

            if (ilgiliKod.Trim() != "")
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("A", "IKIMLIKNO", "=", ilgiliKod));

            sql += " AND B.HESAPKOD IN (" + hesapKodlar + ")  ";

            sql += " GROUP BY A.YIL,A.BELGENO  ";
            sql += " HAVING " + VeriErisim.KosulOl("", "SUM(BORC)", "=", borc) + " AND " + VeriErisim.KosulOl("", "SUM(ALACAK)", "=", alacak);

            return veriErisim.DataReaderDondur(sql);
        }

        public static string HarcamaSorgu(int yil, int muhasebe, string birim, int durumAralik, ENUMBelgeTuru belgeTur, bool faaliyetli, string kur, string fon, string fin, string eko)
        {
            BKD.Tertip tUzBul = new BKD.Tertip(yil, BKD.ENUMTabloKod.TABPEBOdenek);
            int eko4Uz = tUzBul.KodUzunlukBul(3, 2, true);

            kur = kur.Replace(".", "");
            fon = fon.Replace(".", "");
            fin = fin.Replace(".", "");
            eko = eko.Replace(".", "");
            string alanlar = TABLOADMIFF + "." + ALANLARMIFF[(int)ENUMAlanlarMIFF.MUHASEBEKOD];
            alanlar += ", " + TABLOADMIFF + "." + ALANLARMIFF[(int)ENUMAlanlarMIFF.BIRIMKOD];
            alanlar += ", " + TABLOADMIFD + "." + ALANLARMIFD[(int)ENUMAlanlarMIFD.KURKOD];
            alanlar += ", " + TABLOADMIFD + "." + ALANLARMIFD[(int)ENUMAlanlarMIFD.FONKOD];
            alanlar += ", " + TABLOADMIFD + "." + ALANLARMIFD[(int)ENUMAlanlarMIFD.FINKOD];
            alanlar += ", " + VeriErisim.IslecBul("SUBSTRING") + "(" + TABLOADMIFD + "." + ALANLARMIFD[(int)ENUMAlanlarMIFD.EKOKOD] + ",1," + eko4Uz + ")";
            if (faaliyetli)
                alanlar += ", " + TABLOADMIFD + "." + ALANLARMIFD[(int)ENUMAlanlarMIFD.PEBKOD];
            alanlar += ", MUHMUHASEBE.MUHASEBEAD, MUHMUHASEBEBIRIM.BIRIMAD";

            string kur2 = "";
            if (kur.Length == 2)
                kur2 = kur + "70";

            string sql = "SELECT " + alanlar + ", SUM(" + ALANLARMIFD[(int)ENUMAlanlarMIFD.BORC] + "-" + ALANLARMIFD[(int)ENUMAlanlarMIFD.ALACAK] + ")";

            sql += " FROM " + TABLOADMIFF + " INNER JOIN " + TABLOADMIFD;
            sql = VeriErisim.KosulEkle(sql, "ON", TABLOADMIFF + "." + ALANLARMIFF[(int)ENUMAlanlarMIFF.YIL] + "=" + TABLOADMIFD + "." + ALANLARMIFD[(int)ENUMAlanlarMIFD.YIL]);
            sql = VeriErisim.KosulEkle(sql, "AND", TABLOADMIFF + "." + ALANLARMIFF[(int)ENUMAlanlarMIFF.BELGENO] + "=" + TABLOADMIFD + "." + ALANLARMIFD[(int)ENUMAlanlarMIFD.BELGENO]);
            sql += " INNER JOIN MUHMUHASEBE ON MUHMUHASEBE.MUHASEBEKOD = " + TABLOADMIFF + "." + ALANLARMIFF[(int)ENUMAlanlarMIFF.MUHASEBEKOD];
            sql += " INNER JOIN MUHMUHASEBEBIRIM ON MUHMUHASEBEBIRIM.MUHASEBEKOD = " + TABLOADMIFF + "." + ALANLARMIFF[(int)ENUMAlanlarMIFF.MUHASEBEKOD];
            sql = VeriErisim.KosulEkle(sql, "AND", "MUHMUHASEBEBIRIM.BIRIMKOD =" + TABLOADMIFF + "." + ALANLARMIFF[(int)ENUMAlanlarMIFF.BIRIMKOD]);
            sql = VeriErisim.KosulEkle(sql, "AND", "MUHMUHASEBEBIRIM.KURKOD =" + TABLOADMIFD + "." + ALANLARMIFD[(int)ENUMAlanlarMIFD.KURKOD]);

            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOADMIFF, ALANLARMIFF[(int)ENUMAlanlarMIFF.YIL], "=", yil));
            sql += " AND " + VeriErisim.KosulOl(TABLOADMIFD, ALANLARMIFD[(int)ENUMAlanlarMIFD.HESAPKOD], " LIKE ", "830%");

            if (muhasebe > 0)
                sql += " AND " + VeriErisim.KosulOl(TABLOADMIFF, ALANLARMIFF[(int)ENUMAlanlarMIFF.MUHASEBEKOD], "=", muhasebe);

            if (birim != "")
                sql += " AND " + VeriErisim.KosulOl(TABLOADMIFF, ALANLARMIFF[(int)ENUMAlanlarMIFF.BIRIMKOD], "=", birim);

            if (kur != "")
            {
                if (kur2 != "")
                    sql += " AND " + VeriErisim.KosulOl(TABLOADMIFD, ALANLARMIFD[(int)ENUMAlanlarMIFD.KURKOD], " >= ", kur + "");
                else
                    sql += " AND " + VeriErisim.KosulOl(TABLOADMIFD, ALANLARMIFD[(int)ENUMAlanlarMIFD.KURKOD], " LIKE ", kur + "%");

                if (kur2 != "")
                    sql += " AND " + VeriErisim.KosulOl(TABLOADMIFD, ALANLARMIFD[(int)ENUMAlanlarMIFD.KURKOD], " < ", kur2);
            }

            if (fon != "")
                sql += " AND " + VeriErisim.KosulOl(TABLOADMIFD, ALANLARMIFD[(int)ENUMAlanlarMIFD.FONKOD], " LIKE ", fon + "%");
            if (fin != "")
                sql += " AND " + VeriErisim.KosulOl(TABLOADMIFD, ALANLARMIFD[(int)ENUMAlanlarMIFD.FINKOD], " LIKE ", fin + "%");
            if (eko != "")
                sql += " AND " + VeriErisim.KosulOl(TABLOADMIFD, ALANLARMIFD[(int)ENUMAlanlarMIFD.EKOKOD], " LIKE ", eko + "%");
            sql += " AND " + VeriErisim.KosulOl(TABLOADMIFF, ALANLARMIFF[(int)ENUMAlanlarMIFF.FORMDURUM], "<", (int)ENUMBelgeDurumu.IPTAL);
            sql += " GROUP BY " + alanlar + " ORDER BY " + alanlar;
            return sql;
        }

        public IDataReader FaaliyetHarcamaListele(int yil, int ay, int muhasebe, string birim, string kur, string fon, string fin, string eko)
        {
            string alanlar = ALANLARMIFD[(int)ENUMAlanlarMIFD.PEBKOD];
            string sql = "SELECT " + alanlar + ", SUM(" + ALANLARMIFD[(int)ENUMAlanlarMIFD.BORC] + "-" + ALANLARMIFD[(int)ENUMAlanlarMIFD.ALACAK] + ")";

            sql += " FROM " + TABLOADMIFF + " INNER JOIN " + TABLOADMIFD;
            sql = VeriErisim.KosulEkle(sql, "ON", TABLOADMIFF + "." + ALANLARMIFF[(int)ENUMAlanlarMIFF.YIL] + "=" + TABLOADMIFD + "." + ALANLARMIFD[(int)ENUMAlanlarMIFD.YIL]);
            sql = VeriErisim.KosulEkle(sql, "AND", TABLOADMIFF + "." + ALANLARMIFF[(int)ENUMAlanlarMIFF.BELGENO] + "=" + TABLOADMIFD + "." + ALANLARMIFD[(int)ENUMAlanlarMIFD.BELGENO]);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOADMIFF, ALANLARMIFF[(int)ENUMAlanlarMIFF.YIL], "=", yil));
            sql += " AND " + VeriErisim.KosulOl(TABLOADMIFD, ALANLARMIFD[(int)ENUMAlanlarMIFD.HESAPKOD], " LIKE ", "830%");
            sql += " AND " + VeriErisim.KosulOl(TABLOADMIFF, ALANLARMIFF[(int)ENUMAlanlarMIFF.MUHASEBEKOD], "=", muhasebe);
            sql += " AND " + VeriErisim.KosulOl(TABLOADMIFF, ALANLARMIFF[(int)ENUMAlanlarMIFF.BIRIMKOD], "=", birim);
            sql += " AND " + VeriErisim.KosulOl(TABLOADMIFD, ALANLARMIFD[(int)ENUMAlanlarMIFD.KURKOD], "=", kur.Replace(".", ""));
            sql += " AND " + VeriErisim.KosulOl(TABLOADMIFD, ALANLARMIFD[(int)ENUMAlanlarMIFD.FONKOD], "=", fon.Replace(".", ""));
            sql += " AND " + VeriErisim.KosulOl(TABLOADMIFD, ALANLARMIFD[(int)ENUMAlanlarMIFD.FINKOD], "=", fin.Replace(".", ""));
            sql += " AND " + VeriErisim.KosulOl(TABLOADMIFD, ALANLARMIFD[(int)ENUMAlanlarMIFD.EKOKOD], "LIKE", eko.Replace(".", "") + "%");
            sql += " AND " + VeriErisim.KosulOl(TABLOADMIFF, ALANLARMIFF[(int)ENUMAlanlarMIFF.FORMDURUM], "<", (int)ENUMBelgeDurumu.IPTAL);
            sql += " GROUP BY " + alanlar + " ORDER BY " + alanlar;
            return veriErisim.DataReaderDondur(sql);
        }

        public IDataReader FaaliyetGonderilenListele(int yil, int ay, int muhasebe, string birim, string kur, string fon, string fin, string eko)
        {
            BKD.Tertip tUzBul = new BKD.Tertip(yil, BKD.ENUMTabloKod.TABPEBOdenek);
            int eko4Uz = tUzBul.KodUzunlukBul(3, 2, true);

            string sql = "";
            string groupByAlanlar = TABGondermeTenkis.TABLOADGOND + "." + TABGondermeTenkis.ALANLARGOND[(int)TABGondermeTenkis.ENUMAlanlarGOND.PEBKOD];
            sql = "SELECT " + groupByAlanlar + ", SUM(" + TABGondermeTenkis.ALANLARGOND[(int)TABGondermeTenkis.ENUMAlanlarGOND.MIKTAR] + ")";
            sql += " FROM " + TABGondermeTenkis.TABLOADGONF + " LEFT OUTER JOIN " + TABGondermeTenkis.TABLOADGOND;
            sql = VeriErisim.KosulEkle(sql, "ON", TABGondermeTenkis.TABLOADGONF + "." + TABGondermeTenkis.ALANLARGONF[(int)TABGondermeTenkis.ENUMAlanlarGONF.YIL] + "=" + TABGondermeTenkis.TABLOADGOND + "." + TABGondermeTenkis.ALANLARGOND[(int)TABGondermeTenkis.ENUMAlanlarGOND.YIL]);
            sql = VeriErisim.KosulEkle(sql, "AND", TABGondermeTenkis.TABLOADGONF + "." + TABGondermeTenkis.ALANLARGONF[(int)TABGondermeTenkis.ENUMAlanlarGONF.BELGENO] + "=" + TABGondermeTenkis.TABLOADGOND + "." + TABGondermeTenkis.ALANLARGOND[(int)TABGondermeTenkis.ENUMAlanlarGOND.BELGENO]);
            sql = VeriErisim.KosulEkle(sql, "AND", TABGondermeTenkis.TABLOADGONF + "." + TABGondermeTenkis.ALANLARGONF[(int)TABGondermeTenkis.ENUMAlanlarGONF.BELGETUR] + "=" + TABGondermeTenkis.TABLOADGOND + "." + TABGondermeTenkis.ALANLARGOND[(int)TABGondermeTenkis.ENUMAlanlarGOND.BELGETUR]);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABGondermeTenkis.TABLOADGONF, TABGondermeTenkis.ALANLARGONF[(int)TABGondermeTenkis.ENUMAlanlarGONF.YIL], "=", yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABGondermeTenkis.TABLOADGONF, TABGondermeTenkis.ALANLARGONF[(int)TABGondermeTenkis.ENUMAlanlarGONF.MUHASEBEKOD], "=", muhasebe));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABGondermeTenkis.TABLOADGONF, TABGondermeTenkis.ALANLARGONF[(int)TABGondermeTenkis.ENUMAlanlarGONF.BIRIMKOD], "=", birim));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABGondermeTenkis.TABLOADGONF, TABGondermeTenkis.ALANLARGONF[(int)TABGondermeTenkis.ENUMAlanlarGONF.FORMDURUM], "<=", (int)ENUMBelgeDurumu.ONAYLI));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABGondermeTenkis.TABLOADGOND, TABGondermeTenkis.ALANLARGOND[(int)TABGondermeTenkis.ENUMAlanlarGOND.KURKOD], "=", kur.Replace(".", "")));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABGondermeTenkis.TABLOADGOND, TABGondermeTenkis.ALANLARGOND[(int)TABGondermeTenkis.ENUMAlanlarGOND.FONKOD], "=", fon.Replace(".", "")));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABGondermeTenkis.TABLOADGOND, TABGondermeTenkis.ALANLARGOND[(int)TABGondermeTenkis.ENUMAlanlarGOND.FINKOD], "=", fin.Replace(".", "")));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABGondermeTenkis.TABLOADGOND, TABGondermeTenkis.ALANLARGOND[(int)TABGondermeTenkis.ENUMAlanlarGOND.EKOKOD], "=", eko.Replace(".", "").Substring(0, eko4Uz)));
            sql += " GROUP BY " + groupByAlanlar + " ORDER BY " + groupByAlanlar;

            return veriErisim.DataReaderDondur(sql);
        }

        public IDataReader KapatmaListele(int yil, string belgeNo)
        {
            string kosul = "";
            string sql = "SELECT " + VeriErisim.TumAlanlar("", ALANLARKAPANIS) + " FROM " + TABLOADKAPANIS;

            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARKAPANIS[(int)ENUMAlanlarKAPANIS.YIL], "=", yil));
            kosul = VeriErisim.KosulEkle(kosul, "OR", VeriErisim.KosulOl("", ALANLARKAPANIS[(int)ENUMAlanlarKAPANIS.BELGENO], "=", belgeNo));
            kosul = VeriErisim.KosulEkle(kosul, "OR", VeriErisim.KosulOl("", ALANLARKAPANIS[(int)ENUMAlanlarKAPANIS.KBELGENO], "=", belgeNo));
            sql += " AND (" + kosul + ")";
            sql += " ORDER BY SIRANO";

            return veriErisim.DataReaderDondur(sql);
        }

        public IDataReader BirimGelirGider(OdemeEmriMIFForm kriter)
        {
            BKD.Tertip tUzBul = new BKD.Tertip(kriter.yil, BKD.ENUMTabloKod.TABPEBOdenek);
            int eko4Uz = tUzBul.KodUzunlukBul(3, 2, true);

            string sql = "SELECT F.YIL,F.MUHASEBEKOD, F.BIRIMKOD,MB.BIRIMAD,SUBSTRING(HESAPKOD,1,3),SUM(BORC-ALACAK) FROM MUHMIFFORM F,MUHMIF D,MUHMUHASEBEBIRIM MB ";
            sql += "WHERE F.YIL=D.YIL AND F.BELGENO=D.BELGENO AND F.BIRIMKOD=MB.BIRIMKOD";
            sql += " AND (D.HESAPKOD LIKE '800%' OR D.HESAPKOD LIKE '830%')";
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", "F.YIL", "=", kriter.yil));

            if (kriter.muhasebe > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", "F.MUHASEBEKOD", "=", kriter.muhasebe));

            if (kriter.birim != "")
            {
                string[] birimler = kriter.birim.Split(';');
                string birimKosul = "";
                for (int i = 0; i < birimler.Length; i++)
                {
                    if (ANABIRIM)
                        birimKosul = VeriErisim.KosulEkle(birimKosul, "OR", VeriErisim.KosulOl("", ALANLARMIFF[(int)ENUMAlanlarMIFF.BIRIMKOD], "=", birimler[i]));
                    else
                        birimKosul = VeriErisim.KosulEkle(birimKosul, "OR", VeriErisim.KosulOl("", ALANLARMIFD[(int)ENUMAlanlarMIFD.DETAYBIRIM], "=", birimler[i]));
                }

                if (birimKosul != "")
                    sql += " AND (" + birimKosul + ")";
            }

            if (kriter.sorguIslemTarih != "")
            {
                string[] t = kriter.sorguIslemTarih.Split(';');
                YilKontrol(ref t, kriter.yil);

                System.DateTime d1;
                System.DateTime d2;
                if (t.Length == 1 && t[0] != "" || (t.Length == 2 && t[0] != "" && t[1] == ""))
                {
                    d1 = Convert.ToDateTime(new TNSDateTime(t[0]).ToString());
                    d2 = Convert.ToDateTime(new TNSDateTime(t[0]).ToString()).AddDays(1).AddSeconds(-1);
                    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("F", ALANLARMIFF[(int)ENUMAlanlarMIFF.ISLEMTARIH], ">=", new TNSDateTime(d1)));
                    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("F", ALANLARMIFF[(int)ENUMAlanlarMIFF.ISLEMTARIH], "<=", new TNSDateTime(d2)));
                }
                else if (t.Length == 2 && t[0] != "" && t[1] != "")
                {
                    d1 = Convert.ToDateTime(new TNSDateTime(t[0]).ToString());
                    d2 = Convert.ToDateTime(new TNSDateTime(t[1]).ToString()).AddDays(1).AddSeconds(-1);
                    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("F", ALANLARMIFF[(int)ENUMAlanlarMIFF.ISLEMTARIH], ">=", new TNSDateTime(d1)));
                    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("F", ALANLARMIFF[(int)ENUMAlanlarMIFF.ISLEMTARIH], "<=", new TNSDateTime(d2)));
                }
                else if (t.Length == 2 && t[0] == "" && t[1] != "")
                {
                    d2 = Convert.ToDateTime(new TNSDateTime(t[1]).ToString()).AddDays(1).AddSeconds(-1);
                    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("F", ALANLARMIFF[(int)ENUMAlanlarMIFF.ISLEMTARIH], "<=", new TNSDateTime(d2)));
                }
            }

            if (kriter.sorguYEVYevTarih != "")
            {
                string[] t = kriter.sorguYEVYevTarih.Split(';');
                YilKontrol(ref t, kriter.yil);

                System.DateTime d1;
                System.DateTime d2;
                if (t.Length == 1 && t[0] != "" || (t.Length == 2 && t[0] != "" && t[1] == ""))
                {
                    d1 = Convert.ToDateTime(new TNSDateTime(t[0]).ToString());
                    d2 = Convert.ToDateTime(new TNSDateTime(t[0]).ToString()).AddDays(1).AddSeconds(-1);
                    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("F", ALANLARMIFF[(int)ENUMAlanlarMIFF.YEVMIYETARIH], ">=", new TNSDateTime(d1)));
                    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("F", ALANLARMIFF[(int)ENUMAlanlarMIFF.YEVMIYETARIH], "<=", new TNSDateTime(d2)));
                }
                else if (t.Length == 2 && t[0] != "" && t[1] != "")
                {
                    d1 = Convert.ToDateTime(new TNSDateTime(t[0]).ToString());
                    d2 = Convert.ToDateTime(new TNSDateTime(t[1]).ToString()).AddDays(1).AddSeconds(-1);
                    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("F", ALANLARMIFF[(int)ENUMAlanlarMIFF.YEVMIYETARIH], ">=", new TNSDateTime(d1)));
                    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("F", ALANLARMIFF[(int)ENUMAlanlarMIFF.YEVMIYETARIH], "<=", new TNSDateTime(d2)));
                }
                else if (t.Length == 2 && t[0] == "" && t[1] != "")
                {
                    d2 = Convert.ToDateTime(new TNSDateTime(t[1]).ToString()).AddDays(1).AddSeconds(-1);
                    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("F", ALANLARMIFF[(int)ENUMAlanlarMIFF.YEVMIYETARIH], "<=", new TNSDateTime(d2)));
                }
            }

            if (kriter.formDurum > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", "F.FORMDURUM", "=", 6));
            else
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", "F.FORMDURUM", "<", 9));

            if (kriter.fisTur != null && kriter.fisTur.Length > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", TABMizanBilanco.SorguOl("F.FISTUR", kriter.fisTur));
            else
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", "F.FISTUR", "<", 2));

            sql += " GROUP BY F.YIL,F.MUHASEBEKOD,F.BIRIMKOD,MB.BIRIMAD,SUBSTRING(HESAPKOD,1,3)";
            sql += " ORDER BY F.YIL,F.MUHASEBEKOD,F.BIRIMKOD,MB.BIRIMAD,SUBSTRING(HESAPKOD,1,3)";

            return veriErisim.DataReaderDondur(sql);
        }

        public IDataReader GelirGiderListe(OdemeEmriMIFForm kriter)
        {
            string sql = "SELECT D.HESAPKOD,HP.ACIKLAMA, SUM(BORC-ALACAK) FROM MUHMIFFORM F,MUHMIF D, MUHHESAPPLANI HP";
            sql += " WHERE F.YIL=D.YIL AND F.BELGENO=D.BELGENO AND D.HESAPKOD=HP.HESAPKOD AND F.YIL=HP.YIL";
            sql += " AND (D.HESAPKOD LIKE '800%' OR D.HESAPKOD LIKE '830%')";
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", "F.YIL", "=", kriter.yil));

            if (kriter.muhasebe > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", "F.MUHASEBEKOD", "=", kriter.muhasebe));

            if (kriter.birim != "")
            {
                string[] birimler = kriter.birim.Split(';');
                string birimKosul = "";
                for (int i = 0; i < birimler.Length; i++)
                {
                    if (ANABIRIM)
                        birimKosul = VeriErisim.KosulEkle(birimKosul, "OR", VeriErisim.KosulOl("F", ALANLARMIFF[(int)ENUMAlanlarMIFF.BIRIMKOD], "=", birimler[i]));
                    else
                        birimKosul = VeriErisim.KosulEkle(birimKosul, "OR", VeriErisim.KosulOl("D", ALANLARMIFD[(int)ENUMAlanlarMIFD.DETAYBIRIM], "=", birimler[i]));
                }

                if (birimKosul != "")
                    sql += " AND (" + birimKosul + ")";
            }

            if (kriter.sorguIslemTarih != "")
            {
                string[] t = kriter.sorguIslemTarih.Split(';');
                YilKontrol(ref t, kriter.yil);

                System.DateTime d1;
                System.DateTime d2;
                if (t.Length == 1 && t[0] != "" || (t.Length == 2 && t[0] != "" && t[1] == ""))
                {
                    d1 = Convert.ToDateTime(new TNSDateTime(t[0]).ToString());
                    d2 = Convert.ToDateTime(new TNSDateTime(t[0]).ToString()).AddDays(1).AddSeconds(-1);
                    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("F", ALANLARMIFF[(int)ENUMAlanlarMIFF.ISLEMTARIH], ">=", new TNSDateTime(d1)));
                    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("F", ALANLARMIFF[(int)ENUMAlanlarMIFF.ISLEMTARIH], "<=", new TNSDateTime(d2)));
                }
                else if (t.Length == 2 && t[0] != "" && t[1] != "")
                {
                    d1 = Convert.ToDateTime(new TNSDateTime(t[0]).ToString());
                    d2 = Convert.ToDateTime(new TNSDateTime(t[1]).ToString()).AddDays(1).AddSeconds(-1);
                    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("F", ALANLARMIFF[(int)ENUMAlanlarMIFF.ISLEMTARIH], ">=", new TNSDateTime(d1)));
                    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("F", ALANLARMIFF[(int)ENUMAlanlarMIFF.ISLEMTARIH], "<=", new TNSDateTime(d2)));
                }
                else if (t.Length == 2 && t[0] == "" && t[1] != "")
                {
                    d2 = Convert.ToDateTime(new TNSDateTime(t[1]).ToString()).AddDays(1).AddSeconds(-1);
                    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("F", ALANLARMIFF[(int)ENUMAlanlarMIFF.ISLEMTARIH], "<=", new TNSDateTime(d2)));
                }
            }

            if (kriter.sorguYEVYevTarih != "")
            {
                string[] t = kriter.sorguYEVYevTarih.Split(';');
                YilKontrol(ref t, kriter.yil);

                System.DateTime d1;
                System.DateTime d2;
                if (t.Length == 1 && t[0] != "" || (t.Length == 2 && t[0] != "" && t[1] == ""))
                {
                    d1 = Convert.ToDateTime(new TNSDateTime(t[0]).ToString());
                    d2 = Convert.ToDateTime(new TNSDateTime(t[0]).ToString()).AddDays(1).AddSeconds(-1);
                    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("F", ALANLARMIFF[(int)ENUMAlanlarMIFF.YEVMIYETARIH], ">=", new TNSDateTime(d1)));
                    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("F", ALANLARMIFF[(int)ENUMAlanlarMIFF.YEVMIYETARIH], "<=", new TNSDateTime(d2)));
                }
                else if (t.Length == 2 && t[0] != "" && t[1] != "")
                {
                    d1 = Convert.ToDateTime(new TNSDateTime(t[0]).ToString());
                    d2 = Convert.ToDateTime(new TNSDateTime(t[1]).ToString()).AddDays(1).AddSeconds(-1);
                    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("F", ALANLARMIFF[(int)ENUMAlanlarMIFF.YEVMIYETARIH], ">=", new TNSDateTime(d1)));
                    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("F", ALANLARMIFF[(int)ENUMAlanlarMIFF.YEVMIYETARIH], "<=", new TNSDateTime(d2)));
                }
                else if (t.Length == 2 && t[0] == "" && t[1] != "")
                {
                    d2 = Convert.ToDateTime(new TNSDateTime(t[1]).ToString()).AddDays(1).AddSeconds(-1);
                    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("F", ALANLARMIFF[(int)ENUMAlanlarMIFF.YEVMIYETARIH], "<=", new TNSDateTime(d2)));
                }
            }

            if (kriter.formDurum > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", "F.FORMDURUM", "=", 6));
            else
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", "F.FORMDURUM", "<", 9));

            if (kriter.fisTur != null && kriter.fisTur.Length > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", TABMizanBilanco.SorguOl("F.FISTUR", kriter.fisTur));
            else
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", "F.FISTUR", "<", 2));

            sql += " GROUP BY D.HESAPKOD,HP.ACIKLAMA";
            sql += " ORDER BY D.HESAPKOD,HP.ACIKLAMA";

            return veriErisim.DataReaderDondur(sql);
        }

        public IDataReader EkoKod1Basamak(OdemeEmriMIFForm kriter)
        {
            string sql = "SELECT MUHMIFFORM.YIL, MUHMIFFORM.BIRIMKOD, MUHMIFFORM.IADSOYAD, MUHMIFFORM.IKIMLIKNO, SUBSTRING(MUHMIF.EKOKOD, 1, 2) AS Expr1, MUHMIFFORM.ACIKLAMA, MUHMIFFORM.ISLEMTARIH, SUM(MUHMIF.BORC - MUHMIF.ALACAK)";
            sql += " FROM MUHMIFFORM";
            sql += " INNER JOIN MUHMIF ON MUHMIFFORM.YIL = MUHMIF.YIL AND MUHMIFFORM.BELGENO = MUHMIF.BELGENO";
            sql += " WHERE MUHMIF.HESAPKOD LIKE '830%' ";
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("MUHMIFFORM", "YIL", "=", kriter.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("MUHMIF", "DETAYBIRIM", "=", kriter.birim));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("MUHMIFFORM", "FORMDURUM", "<", (int)ENUMBelgeDurumu.IPTAL));

            sql += " GROUP BY MUHMIFFORM.YIL, MUHMIFFORM.BIRIMKOD, MUHMIFFORM.IADSOYAD, MUHMIFFORM.IKIMLIKNO, SUBSTRING(MUHMIF.EKOKOD, 1, 2), MUHMIFFORM.ACIKLAMA, MUHMIFFORM.ISLEMTARIH";

            return veriErisim.DataReaderDondur(sql);
        }

        public Sonuc KaydetEntegrasyon(MIFEntegrasyon bilgi)
        {
            string sql = "INSERT INTO " + TABLOADENTEGRASYON + "(" + VeriErisim.TumAlanlar("", ALANLARENTEGRASYON) + ") VALUES (";

            sql = sql + VeriErisim.DegerYap(bilgi.yil) + ", ";
            sql = sql + VeriErisim.DegerYap(bilgi.belgeNo) + ", ";
            sql = sql + VeriErisim.DegerYap(bilgi.ettn) + ", ";
            sql = sql + VeriErisim.DegerYap(bilgi.tarih) + ", ";
            sql = sql + VeriErisim.DegerYap(bilgi.islemYapan) + ", ";
            sql = sql + VeriErisim.DegerYap(bilgi.durum) + ") ";

            return KomutCalistir(sql, 1);
        }

        public Sonuc GuncelleEntegrasyon(MIFEntegrasyon bilgi)
        {
            string sql = "UPDATE " + TABLOADENTEGRASYON + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLARENTEGRASYON[(int)ENUMAlanlarEntegrasyon.ETTN], bilgi.ettn) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLARENTEGRASYON[(int)ENUMAlanlarEntegrasyon.TARIH], bilgi.tarih) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLARENTEGRASYON[(int)ENUMAlanlarEntegrasyon.ISLEMYAPAN], bilgi.islemYapan) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLARENTEGRASYON[(int)ENUMAlanlarEntegrasyon.DURUM], bilgi.durum) + ", ";
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARENTEGRASYON[(int)ENUMAlanlarEntegrasyon.YIL], "=", bilgi.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARENTEGRASYON[(int)ENUMAlanlarEntegrasyon.BELGENO], "=", bilgi.belgeNo));

            return KomutCalistir(sql, 1);
        }


        public Sonuc SilEntegrasyon(MIFEntegrasyon bilgi)
        {
            string sql = "DELETE FROM " + TABLOADENTEGRASYON;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOADENTEGRASYON, ALANLARENTEGRASYON[(int)ENUMAlanlarEntegrasyon.YIL], "=", bilgi.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADENTEGRASYON, ALANLARENTEGRASYON[(int)ENUMAlanlarEntegrasyon.BELGENO], "=", bilgi.belgeNo));

            return KomutCalistir(sql, 0);
        }

        public IDataReader Listele(MIFEntegrasyon bilgi)
        {
            string sql = "";
            string kosul = "";

            sql = "SELECT " + VeriErisim.TumAlanlar(TABLOADENTEGRASYON, ALANLARENTEGRASYON);
            sql += " FROM " + TABLOADENTEGRASYON;
            sql += " INNER JOIN MUHMIFFORM ON " + TABLOADENTEGRASYON + ".YIL = MUHMIFFORM.YIL AND " + TABLOADENTEGRASYON + ".BELGENO = MUHMIFFORM.BELGENO";

            if (bilgi.yil > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADENTEGRASYON, ALANLARENTEGRASYON[(int)ENUMAlanlarEntegrasyon.YIL], "=", bilgi.yil));

            if (bilgi.belgeNo != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADENTEGRASYON, ALANLARENTEGRASYON[(int)ENUMAlanlarEntegrasyon.BELGENO], "=", bilgi.belgeNo));

            if (bilgi.ettn != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADENTEGRASYON, ALANLARENTEGRASYON[(int)ENUMAlanlarEntegrasyon.ETTN], "=", bilgi.ettn));

            if (!string.IsNullOrEmpty(kosul))
            {
                sql += " WHERE " + kosul;
            }

            sql += " ORDER BY " + ALANLARENTEGRASYON[(int)ENUMAlanlarEntegrasyon.YIL] + "," + ALANLARENTEGRASYON[(int)ENUMAlanlarEntegrasyon.BELGENO];

            return veriErisim.DataReaderDondur(sql);
        }

        public IDataReader ManasGunlukTakip(ManasGunlukTakip kriter, string tur)
        {
            string sql = "";
            string sorguTarihIslec = "";
            if (tur == "BAKIYE")
            {
                sql = "SELECT HESAPKOD,SUM(BORC)-SUM(ALACAK) AS BAKIYE,SUM(DOVIZBORC)-SUM(DOVIZALACAK) AS DOVIZBAKIYE,SUM(BORC),SUM(ALACAK),SUM(DOVIZBORC),SUM(DOVIZALACAK)";
                sorguTarihIslec = "<";
            }
            else
            {
                sql = "SELECT HESAPKOD,SUM(BORC) AS BORC,SUM(ALACAK) AS ALACAK,SUM(DOVIZBORC) AS DOVIZBORC ,SUM(DOVIZALACAK) AS DOVIZALACAK";
                sorguTarihIslec = "=";
            }

            sql += " FROM MUHODEMEEMRI";
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("MUHODEMEEMRI", "YIL", "=", kriter.sorguTarih.Yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("MUHODEMEEMRI", "YEVMIYENO", ">", 0));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("MUHODEMEEMRI", "YEVMIYETARIH", sorguTarihIslec, kriter.sorguTarih));

            string[] hkodlar = kriter.hesapKod.Split(',');
            string icSorgu = "";
            foreach (string hk in hkodlar)
            {
                if (hk == "") continue;
                icSorgu = VeriErisim.KosulEkle(icSorgu, "OR", VeriErisim.KosulOl("MUHODEMEEMRI", "HESAPKOD", "=", hk));
            }
            if (icSorgu != "")
                sql = VeriErisim.KosulEkle(sql, "AND", "(" + icSorgu + ")");

            sql += " GROUP BY HESAPKOD";

            return veriErisim.DataReaderDondur(sql);
        }

        public bool IsKullanildiMi(int isKod)
        {
            string sql = "SELECT COUNT(*) FROM MUHMIF";
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", "ISKOD", "=", isKod));

            return veriErisim.TekKolonSQLInt(sql) > 0;
        }

    }
}
