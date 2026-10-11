using System.Data;
using TNS.Veri;

namespace TNS.MUH.IsKurallari.Veri
{
    /// <summary>
    /// MUHTAAHHUTODEMEEMRI veri tabanı tablosu ile ilgili, listeleme ve yazma işlemleri tutan nesne
    /// </summary>
    public class TABTaahhutOdemeEmri : TABOrtak
    {
        /// <summary>
        /// Sınıfın oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABTaahhutOdemeEmri(VeriErisim veriErisim) : base(veriErisim) { }

        static string TABLOAD = "MUHTAAHHUTODEMEEMRI";
        enum ENUMAlanlar
        {
            KOD = 0,
            TAAHHUTKOD = 1,
            ODEMEEMRIKOD = 2,
            ODEMEEMRITARIHI = 3,
            ACIKLAMA = 4,
            HAKEDISTUTARI = 5,
            DAMGAVERGISI = 6,
            GELIRVERGISISTOPAJI = 7,
            FONPAYI = 8,
            PARACEZALARI = 9,
            MAHSUPEDILENAVANS = 10,
            ICRAKESINTISI = 11,
            TEMINATKESINTISI = 12,
            SSKPRIMIVEGECIKMEZAMMI = 13,
            VERGIBORCU = 14,
            KDVTEVKIFATI = 15,
            TEMLIK = 16,
            BUTCEEMANETI = 17,
            KDV = 18,
            KESINTILERTOPLAMI = 19,
            YAPILANODEME = 20,
            HAKEDISHESAP = 21,
            ODEMEHESAP = 22,
            YANSITMAHESAP = 23,
            ISLEMTARIHI = 24,
            ISLEMYAPANKOD = 25,
            ISLEMYAPANAD = 26,
            KDVHESAPKODU = 27,
            DAMGAVERGISIHESAPKODU = 28,
            GELIRVERGISIHESAPKODU = 29,
            FONPAYIHESAPKODU = 30,
            PARACEZALARIHESAPKODU = 31,
            MAHSUPEDILENAVANSHESAPKODU = 32,
            ICRAKESINTISIHESAPKODU = 33,
            TEMINATKESINTISIHESAPKODU = 34,
            SSKPRIMIGECIKMEZAMMIHESAPKODU = 35,
            VERGIBORCUHESAPKODU = 36,
            KDVTEVKIFATIHESAPKODU = 37,
            TEMLIKHESAPKODU = 38,
            BUTCEEMANETIHESAPKODU = 39,
            KDVHESAPKODU2 = 40,
            KDV2 = 41,
            KDVHESAPKODU3 = 42,
            KDV3 = 43,
            DAMGAVERGISIHESAPKODU2 = 44,
            DAMGAVERGISI2 = 45,
            KIRAGELIRIHESAKODU = 46,
            KIRAGELIRI = 47,
            KIRAGELIRIKDVHESAKODU = 48,
            KIRAGELIRIKDV = 49,
            CESITLIGELIRHESAKODU = 50,
            CESITLIGELIR = 51,
            CESITLIGELIRKDVHESAKODU = 52,
            CESITLIGELIRKDV = 53,
            DIGERHIZMETGELIRHESAKODU = 54,
            DIGERHIZMETGELIR = 55,
            DIGERHIZMETGELIRKDVHESAKODU = 56,
            DIGERHIZMETGELIRKDV = 57,
        }

        static string[] ALANLAR ={  "KOD", "TAAHHUTKOD", "ODEMEEMRIKOD" ,"ODEMEEMRITARIHI", "ACIKLAMA", "HAKEDISTUTARI", "DAMGAVERGISI", "GELIRVERGISISTOPAJI","FONPAYI",
                                     "PARACEZALARI", "MAHSUPEDILENAVANS", "ICRAKESINTISI", "TEMINATKESINTISI", "SSKPRIMIVEGECIKMEZAMMI",
                                     "VERGIBORCU", "KDVTEVKIFATI", "TEMLIK", "BUTCEEMANETI", "KDV", "KESINTILERTOPLAMI", "YAPILANODEME",
                                     "HAKEDISHESAP","ODEMEHESAP","YANSITMAHESAP", "ISLEMTARIHI", "ISLEMYAPANKOD", "ISLEMYAPANAD",
                                 "KDVHESAPKODU","DAMGAVERGISIHESAPKODU","GELIRVERGISIHESAPKODU","FONPAYIHESAPKODU","PARACEZALARIHESAPKODU","MAHSUPEDILENAVANSHESAPKODU",
                                 "ICRAKESINTISIHESAPKODU","TEMINATKESINTISIHESAPKODU","SSKPRIMIGECIKMEZAMMIHESAPKODU","VERGIBORCUHESAPKODU","KDVTEVKIFATIHESAPKODU",
                                 "TEMLIKHESAPKODU","BUTCEEMANETIHESAPKODU","KDVHESAPKODU2","KDV2","KDVHESAPKODU3","KDV3","DAMGAVERGISIHESAPKODU2","DAMGAVERGISI2",
                                 "KIRAGELIRIHESAKODU","KIRAGELIRI","KIRAGELIRIKDVHESAKODU","KIRAGELIRIKDV",
                                 "CESITLIGELIRHESAKODU", "CESITLIGELIR", "CESITLIGELIRKDVHESAKODU", "CESITLIGELIRKDV",
                                 "DIGERHIZMETGELIRHESAKODU", "DIGERHIZMETGELIR", "DIGERHIZMETGELIRKDVHESAKODU", "DIGERHIZMETGELIRKDV"

                                 };

        static string TABLOADDIGER = "MUHTAAHHUTODEMEEMRIDIGER";
        enum ENUMAlanlarDiger
        {
            KOD = 0,
            TAAHHUTKOD = 1,
            TAAHHUTODEMEEMRIKOD = 2,
            HESAPKOD = 3,
            BORC = 4,
            ALACAK = 5,
            HESAPKODKDV = 6,
            BORCKDV = 7,
            ALACAKKDV = 8,
        }
        static string[] ALANLARDIGER = { "KOD", "TAAHHUTKOD", "TAAHHUTODEMEEMRIKOD", "HESAPKOD", "BORC", "ALACAK", "HESAPKODKDV", "BORCKDV", "ALACAKKDV" };

        /// <summary>
        /// Parametre olarak gelen bilgileri veri tabanına ekleyen sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="odemeEmri">The odeme emri.</param>
        /// <returns></returns>
        public Sonuc Kaydet(TaahhutOdeme odemeEmri)
        {
            Sonuc sonuc = new Sonuc(false, "", "");

            string sql = "";

            sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
            sql = sql + VeriErisim.DegerYap(odemeEmri.kod) + ", ";
            sql = sql + VeriErisim.DegerYap(odemeEmri.taahhutKod) + ", ";
            sql = sql + VeriErisim.DegerYap(odemeEmri.odemeEmriKod) + ", ";
            sql = sql + VeriErisim.DegerYap(odemeEmri.odemeEmriTarihi) + ", ";
            sql = sql + VeriErisim.DegerYap(odemeEmri.aciklama) + ", ";
            sql = sql + VeriErisim.DegerYap(odemeEmri.hakedisTutari) + ", ";
            sql = sql + VeriErisim.DegerYap(odemeEmri.damgaVergisi) + ", ";
            sql = sql + VeriErisim.DegerYap(odemeEmri.gelirVergisiStopaji) + ", ";
            sql = sql + VeriErisim.DegerYap(odemeEmri.fonPayi) + ", ";
            sql = sql + VeriErisim.DegerYap(odemeEmri.paraCezalari) + ", ";
            sql = sql + VeriErisim.DegerYap(odemeEmri.mahsupEdilenAvans) + ", ";
            sql = sql + VeriErisim.DegerYap(odemeEmri.icraKesintisi) + ", ";
            sql = sql + VeriErisim.DegerYap(odemeEmri.teminatKesintisi) + ", ";
            sql = sql + VeriErisim.DegerYap(odemeEmri.sskPrimiVeGecikmeZammi) + ", ";
            sql = sql + VeriErisim.DegerYap(odemeEmri.vergiBorcu) + ", ";
            sql = sql + VeriErisim.DegerYap(odemeEmri.kdvTevkifati) + ", ";
            sql = sql + VeriErisim.DegerYap(odemeEmri.temlik) + ", ";
            sql = sql + VeriErisim.DegerYap(odemeEmri.butceEmaneti) + ", ";
            sql = sql + VeriErisim.DegerYap(odemeEmri.kdv) + ", ";
            sql = sql + VeriErisim.DegerYap(odemeEmri.kesintilerToplami) + ", ";
            sql = sql + VeriErisim.DegerYap(odemeEmri.yapilanOdeme) + ", ";
            sql = sql + VeriErisim.DegerYap(odemeEmri.hakedisHesap) + ", ";
            sql = sql + VeriErisim.DegerYap(odemeEmri.odemeHesap) + ", ";
            sql = sql + VeriErisim.DegerYap(odemeEmri.yansitmaHesap) + ", ";
            sql = sql + VeriErisim.DegerYap(odemeEmri.islemTarihi) + ", ";
            sql = sql + VeriErisim.DegerYap(odemeEmri.islemYapanKod) + ", ";
            sql = sql + VeriErisim.DegerYap(odemeEmri.islemYapanAd) + ", ";
            sql = sql + VeriErisim.DegerYap(odemeEmri.kdvHesapKodu) + ", ";
            sql = sql + VeriErisim.DegerYap(odemeEmri.damgaVergisiHesapKodu) + ", ";
            sql = sql + VeriErisim.DegerYap(odemeEmri.gelirVergisiHesapKodu) + ", ";
            sql = sql + VeriErisim.DegerYap(odemeEmri.fonPayiHesapKodu) + ", ";
            sql = sql + VeriErisim.DegerYap(odemeEmri.paraCezalariHesapKodu) + ", ";
            sql = sql + VeriErisim.DegerYap(odemeEmri.mahsupEdileAvansHesapKodu) + ", ";
            sql = sql + VeriErisim.DegerYap(odemeEmri.icraKesintisiHesapKodu) + ", ";
            sql = sql + VeriErisim.DegerYap(odemeEmri.teminatKesintisiHesapKodu) + ", ";
            sql = sql + VeriErisim.DegerYap(odemeEmri.sskPrimiGecikmeZammiHesapKodu) + ", ";
            sql = sql + VeriErisim.DegerYap(odemeEmri.vergiBorcuHesapKodu) + ", ";
            sql = sql + VeriErisim.DegerYap(odemeEmri.kdvTevkifatiHesapKodu) + ", ";
            sql = sql + VeriErisim.DegerYap(odemeEmri.temlikHesapKodu) + ", ";
            sql = sql + VeriErisim.DegerYap(odemeEmri.butceEmanetiHesapKodu) + ", ";
            sql = sql + VeriErisim.DegerYap(odemeEmri.kdvHesapKodu2) + ", ";
            sql = sql + VeriErisim.DegerYap(odemeEmri.kdv2) + ", ";
            sql = sql + VeriErisim.DegerYap(odemeEmri.kdvHesapKodu3) + ", ";
            sql = sql + VeriErisim.DegerYap(odemeEmri.kdv3) + ", ";
            sql = sql + VeriErisim.DegerYap(odemeEmri.damgaVergisiHesapKodu2) + ", ";
            sql = sql + VeriErisim.DegerYap(odemeEmri.damgaVergisi2) + ", ";
            sql = sql + VeriErisim.DegerYap(odemeEmri.kiraGeliriHesapKodu) + ", ";
            sql = sql + VeriErisim.DegerYap(odemeEmri.kiraGeliri) + ", ";
            sql = sql + VeriErisim.DegerYap(odemeEmri.kiraGeliriKDVHesapKodu) + ", ";
            sql = sql + VeriErisim.DegerYap(odemeEmri.kiraGeliriKDV) + ", ";
            sql = sql + VeriErisim.DegerYap(odemeEmri.cesitliGelirHesapKodu) + ", ";
            sql = sql + VeriErisim.DegerYap(odemeEmri.cesitliGelir) + ", ";
            sql = sql + VeriErisim.DegerYap(odemeEmri.cesitliGelirKDVHesapKodu) + ", ";
            sql = sql + VeriErisim.DegerYap(odemeEmri.cesitliGelirKDV) + ", ";
            sql = sql + VeriErisim.DegerYap(odemeEmri.digerHizmetGelirHesapKodu) + ", ";
            sql = sql + VeriErisim.DegerYap(odemeEmri.digerHizmetGelir) + ", ";
            sql = sql + VeriErisim.DegerYap(odemeEmri.digerHizmetGelirKDVHesapKodu) + ", ";
            sql = sql + VeriErisim.DegerYap(odemeEmri.digerHizmetGelirKDV) + ") ";
            sonuc = KomutCalistir(sql, 1);

            if (!sonuc.islemSonuc)
            {
                veriErisim.hata = "";

                sql = "UPDATE " + TABLOAD + " SET ";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.TAAHHUTKOD], odemeEmri.taahhutKod) + ", ";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ODEMEEMRIKOD], odemeEmri.odemeEmriKod) + ", ";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ODEMEEMRITARIHI], odemeEmri.odemeEmriTarihi) + ", ";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ACIKLAMA], odemeEmri.aciklama) + ", ";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.HAKEDISTUTARI], odemeEmri.hakedisTutari) + ", ";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.DAMGAVERGISI], odemeEmri.damgaVergisi) + ", ";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.GELIRVERGISISTOPAJI], odemeEmri.gelirVergisiStopaji) + ", ";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.FONPAYI], odemeEmri.fonPayi) + ", ";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.PARACEZALARI], odemeEmri.paraCezalari) + ", ";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.MAHSUPEDILENAVANS], odemeEmri.mahsupEdilenAvans) + ", ";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ICRAKESINTISI], odemeEmri.icraKesintisi) + ", ";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.TEMINATKESINTISI], odemeEmri.teminatKesintisi) + ", ";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.SSKPRIMIVEGECIKMEZAMMI], odemeEmri.sskPrimiVeGecikmeZammi) + ", ";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.VERGIBORCU], odemeEmri.vergiBorcu) + ", ";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.KDVTEVKIFATI], odemeEmri.kdvTevkifati) + ", ";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.TEMLIK], odemeEmri.temlik) + ", ";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.BUTCEEMANETI], odemeEmri.butceEmaneti) + ", ";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.KDV], odemeEmri.kdv) + ", ";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.KESINTILERTOPLAMI], odemeEmri.kesintilerToplami) + ", ";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.YAPILANODEME], odemeEmri.yapilanOdeme) + ", ";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.HAKEDISHESAP], odemeEmri.hakedisHesap) + ", ";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ODEMEHESAP], odemeEmri.odemeHesap) + ", ";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.YANSITMAHESAP], odemeEmri.yansitmaHesap) + ", ";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ISLEMTARIHI], odemeEmri.islemTarihi) + ", ";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ISLEMYAPANKOD], odemeEmri.islemYapanKod) + ", ";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ISLEMYAPANAD], odemeEmri.islemYapanAd) + ", ";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.KDVHESAPKODU], odemeEmri.kdvHesapKodu) + ", ";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.DAMGAVERGISIHESAPKODU], odemeEmri.damgaVergisiHesapKodu) + ", ";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.GELIRVERGISIHESAPKODU], odemeEmri.gelirVergisiHesapKodu) + ", ";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.FONPAYIHESAPKODU], odemeEmri.fonPayiHesapKodu) + ", ";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.PARACEZALARIHESAPKODU], odemeEmri.paraCezalariHesapKodu) + ", ";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.MAHSUPEDILENAVANSHESAPKODU], odemeEmri.mahsupEdileAvansHesapKodu) + ", ";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ICRAKESINTISIHESAPKODU], odemeEmri.icraKesintisiHesapKodu) + ", ";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.TEMINATKESINTISIHESAPKODU], odemeEmri.teminatKesintisiHesapKodu) + ", ";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.SSKPRIMIGECIKMEZAMMIHESAPKODU], odemeEmri.sskPrimiGecikmeZammiHesapKodu) + ", ";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.VERGIBORCUHESAPKODU], odemeEmri.vergiBorcuHesapKodu) + ", ";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.KDVTEVKIFATIHESAPKODU], odemeEmri.kdvTevkifatiHesapKodu) + ", ";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.TEMLIKHESAPKODU], odemeEmri.temlikHesapKodu) + ", ";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.BUTCEEMANETIHESAPKODU], odemeEmri.butceEmanetiHesapKodu) + ", ";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.KDVHESAPKODU2], odemeEmri.kdvHesapKodu2) + ", ";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.KDV2], odemeEmri.kdv2) + ", ";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.KDVHESAPKODU3], odemeEmri.kdvHesapKodu3) + ", ";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.KDV3], odemeEmri.kdv3) + ", ";

                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.KIRAGELIRIHESAKODU], odemeEmri.kiraGeliriHesapKodu) + ", ";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.KIRAGELIRI], odemeEmri.kiraGeliri) + ", ";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.KIRAGELIRIKDVHESAKODU], odemeEmri.kiraGeliriKDVHesapKodu) + ", ";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.KIRAGELIRIKDV], odemeEmri.kiraGeliriKDV) + ", ";

                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.CESITLIGELIRHESAKODU], odemeEmri.cesitliGelirHesapKodu) + ", ";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.CESITLIGELIR], odemeEmri.cesitliGelir) + ", ";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.CESITLIGELIRKDVHESAKODU], odemeEmri.cesitliGelirKDVHesapKodu) + ", ";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.CESITLIGELIRKDV], odemeEmri.cesitliGelirKDV) + ", ";

                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.DIGERHIZMETGELIRHESAKODU], odemeEmri.digerHizmetGelirHesapKodu) + ", ";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.DIGERHIZMETGELIR], odemeEmri.digerHizmetGelir) + ", ";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.DIGERHIZMETGELIRKDVHESAKODU], odemeEmri.digerHizmetGelirKDVHesapKodu) + ", ";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.DIGERHIZMETGELIRKDV], odemeEmri.digerHizmetGelirKDV) + ", ";

                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.DAMGAVERGISIHESAPKODU2], odemeEmri.damgaVergisiHesapKodu2) + ", ";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.DAMGAVERGISI2], odemeEmri.damgaVergisi2);
                sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", odemeEmri.kod));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.TAAHHUTKOD], "=", odemeEmri.taahhutKod));
                sonuc = KomutCalistir(sql, 1);
            }

            sonuc.anahtar = odemeEmri.kod.ToString();
            return sonuc;
        }

        /// <summary>
        /// Parametre bilgilerine uygun Tablodaki kayıtları silen sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="kod">The kod.</param>
        /// <returns></returns>
        public Sonuc Sil(int kod)
        {
            string sql = "DELETE FROM " + TABLOAD;
            string kosul = "";

            if (kod != 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", kod + " "));
            if (!string.IsNullOrEmpty(kosul))
                sql += " WHERE " + kosul;

            return KomutCalistir(sql, 0);
        }

        /// <summary>
        /// Parametre olarak gelen alanları kıstas olarak kullanarak, select sql cümlesini oluşturup, çalıştıran ve sonucunu DataReader olarak döndüren fonksiyon
        /// </summary>
        /// <param name="bilgi">bilgi.</param>
        /// <returns></returns>
        public IDataReader Listele(TaahhutOdeme bilgi)
        {
            string sql = "";
            string kosul = "";
            sql = "SELECT " + VeriErisim.TumAlanlar("", ALANLAR) + " FROM " + TABLOAD;

            if (bilgi.taahhutKod != 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.TAAHHUTKOD], "=", bilgi.taahhutKod));

            if (bilgi.kod != 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", bilgi.kod));

            if (kosul != "")
                sql += " WHERE " + kosul;

            sql += " ORDER BY " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.KOD];
            IDataReader r = veriErisim.DataReaderDondur(sql);
            return r;
        }

        //Diğer


        /// <summary>
        /// Parametre olarak gelen bilgileri veri tabanına ekleyen sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="odemeEmriDiger">The odeme emri.</param>
        /// <returns></returns>
        public Sonuc KaydetDiger(TaahhutOdemeDiger odemeEmriDiger)
        {
            Sonuc sonuc = new Sonuc(false, "", "");

            string sql = "";

            sql = "INSERT INTO " + TABLOADDIGER + " (" + VeriErisim.TumAlanlar("", ALANLARDIGER) + ") VALUES (";
            sql = sql + VeriErisim.DegerYap(odemeEmriDiger.kod) + ", ";
            sql = sql + VeriErisim.DegerYap(odemeEmriDiger.taahhutKod) + ", ";
            sql = sql + VeriErisim.DegerYap(odemeEmriDiger.taahhutOdemeKod) + ", ";
            sql = sql + VeriErisim.DegerYap(odemeEmriDiger.hesapKod) + ", ";
            sql = sql + VeriErisim.DegerYap(odemeEmriDiger.borc) + ", ";
            sql = sql + VeriErisim.DegerYap(odemeEmriDiger.alacak) + ", ";
            sql = sql + VeriErisim.DegerYap(odemeEmriDiger.hesapKodKDV) + ", ";
            sql = sql + VeriErisim.DegerYap(odemeEmriDiger.borcKDV) + ", ";
            sql = sql + VeriErisim.DegerYap(odemeEmriDiger.alacakKDV) + ") ";
            sonuc = KomutCalistir(sql, 1);

            sonuc.anahtar = odemeEmriDiger.kod.ToString();
            return sonuc;
        }

        /// <summary>
        /// Parametre bilgilerine uygun Tablodaki kayıtları silen sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="taahhutOdemeKod">The taahhut odeme kod.</param>
        /// <returns></returns>
        public Sonuc SilDiger(int taahhutOdemeKod, int taahhutKod)
        {
            string sql = "DELETE FROM " + TABLOADDIGER;
            if (taahhutOdemeKod > 0)
                sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARDIGER[(int)ENUMAlanlarDiger.TAAHHUTODEMEEMRIKOD], "=", taahhutOdemeKod));
            else
                sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARDIGER[(int)ENUMAlanlarDiger.TAAHHUTKOD], "=", taahhutKod));

            return KomutCalistir(sql, 0);
        }

        /// <summary>
        /// Parametre olarak gelen alanları kıstas olarak kullanarak, select sql cümlesini oluşturup, çalıştıran ve sonucunu DataReader olarak döndüren fonksiyon
        /// </summary>
        /// <param name="taahhutOdemeKod">The taahhut odeme kod.</param>
        /// <returns></returns>
        public IDataReader ListeleDiger(int taahhutOdemeKod)
        {
            string sql;
            sql = "SELECT " + VeriErisim.TumAlanlar("", ALANLARDIGER) + " FROM " + TABLOADDIGER;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARDIGER[(int)ENUMAlanlarDiger.TAAHHUTODEMEEMRIKOD], "=", taahhutOdemeKod));
            sql += " ORDER BY " + TABLOADDIGER + "." + ALANLARDIGER[(int)ENUMAlanlarDiger.KOD];
            return veriErisim.DataReaderDondur(sql);
        }
    }
}
