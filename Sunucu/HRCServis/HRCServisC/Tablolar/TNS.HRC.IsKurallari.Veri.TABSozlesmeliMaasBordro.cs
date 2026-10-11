using System;
using System.Data;
using TNS.Veri;

namespace TNS.HRC.IsKurallari.Veri
{
    /// <summary>
    /// Sözleşmeli maaş bordro bilgisini tutan PSLSOZLESMELIMAASBORDRO ve PSLSOZLESMELIMAASMIF veri tabanı tabloları ile ilgili, 
    /// listeleme ve yazma işlemleri tutan nesne
    /// </summary>
    public class TABSozlesmeliMaasBordro : TABOrtak
    {
        /// <summary>
        /// Sınıfın oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABSozlesmeliMaasBordro(VeriErisim veriErisim) : base(veriErisim) { }

        /// <summary>
        /// Sözleşmeli maaş bordro bilgisi tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLOAD = "PSLSOZLESMELIMAASBORDRO";

        /// <summary>
        /// Sözleşmeli maaş bordro bilgisi tablo alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlar
        {
            /// <summary>
            /// YIL alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            YIL = 0,
            /// <summary>
            /// MUHASEBEKOD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            MUHASEBEKOD = 1,
            /// <summary>
            /// BIRIMKOD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            BIRIMKOD = 2,
            /// <summary>
            /// SURECNO alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            SURECNO = 3,
            /// <summary>
            /// TCKIMLIKNO alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            TCKIMLIKNO = 4,
            /// <summary>
            /// MAASYIL alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            MAASYIL = 5,
            /// <summary>
            /// MAASAY alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            MAASAY = 6,
            /// <summary>
            /// SIRANO alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            SIRANO = 7,
            /// <summary>
            /// GUN alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            GUN = 8,
            /// <summary>
            /// UCRET alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            UCRET = 9,
            /// <summary>
            /// EKTAZMINAT alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            EKTAZMINAT = 10,
            /// <summary>
            /// ASGGECIMINDIRIMI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            ASGGECIMINDIRIMI = 11,
            /// <summary>
            /// OZELSIGORTA alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            OZELSIGORTA = 12,
            /// <summary>
            /// SAKATLIKINDIRIM alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            SAKATLIKINDIRIM = 13,
            /// <summary>
            /// GELIRVERGISIORAN alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            GELIRVERGISIORAN = 14,
            /// <summary>
            /// GELIRVERGISI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            GELIRVERGISI = 15,
            /// <summary>
            /// DAMGAVERGISIORAN alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            DAMGAVERGISIORAN = 16,
            /// <summary>
            /// DAMGAVERGISI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            DAMGAVERGISI = 17,
            /// <summary>
            /// ASILGELIRVERGISI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            ASILGELIRVERGISI = 18,
            /// <summary>
            /// SSKISVERENPAYIORAN alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            SSKISVERENPAYIORAN = 19,
            /// <summary>
            /// SSKISVERENPAYI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            SSKISVERENPAYI = 20,
            /// <summary>
            /// SSKISCIPAYIORAN alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            SSKISCIPAYIORAN = 21,
            /// <summary>
            /// SSKISCIPAYI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            SSKISCIPAYI = 22,
            /// <summary>
            /// LOJMAN alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            LOJMAN = 23,
            /// <summary>
            /// ICRA alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            ICRA = 24,
            /// <summary>
            /// DIGERKESINTI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            DIGERKESINTI = 25,
            /// <summary>
            /// KESINTITOPLAM alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            KESINTITOPLAM = 26,
            /// <summary>
            /// ODENMESIGEREKEN alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            ODENMESIGEREKEN = 27,
            /// <summary>
            /// KUMVERGIMATRAHI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            KUMVERGIMATRAHI = 28,
            /// <summary>
            /// VERGIMATRAHI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            VERGIMATRAHI = 29,
            /// <summary>
            /// TAHAKKUKTOPLAM alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            TAHAKKUKTOPLAM = 30,
            /// <summary>
            /// MAXMAASGOSTERGE alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            MAXMAASGOSTERGE = 31,
            /// <summary>
            /// AYLIKKATSAYI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            AYLIKKATSAYI = 32,
            /// <summary>
            /// BRUTASGARIUCRET alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            BRUTASGARIUCRET = 33,
            /// <summary>
            /// ISSIZSIGPRIMORAN alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            ISSIZSIGPRIMORAN = 34,
            /// <summary>
            /// EKTAZMINATORAN alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            EKTAZMINATORAN = 35,
            /// <summary>
            /// ASGGECIMINDKATSAYI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            ASGGECIMINDKATSAYI = 36,
            /// <summary>
            /// BANKAKODU alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            BANKAKODU = 37,
            /// <summary>
            /// BANKASUBEKODU alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            BANKASUBEKODU = 38,
            /// <summary>
            /// BANKAHESAPNO alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            BANKAHESAPNO = 39,
            /// <summary>
            /// UNVANKODU alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            UNVANKODU = 40,
            /// <summary>
            /// MEDENIDURUM alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            MEDENIDURUM = 41,
            /// <summary>
            /// SENDIKADEVLETYARDIMI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            SENDIKADEVLETYARDIMI = 42,
            /// <summary>
            /// SENDIKAAIDATI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            SENDIKAAIDATI = 43,
            /// <summary>
            /// NAFAKA alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            NAFAKA = 44,
            /// <summary>
            /// BORDROTUR alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            BORDROTUR = 45,
            /// <summary>
            /// AILEYARDIMI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            AILEYARDIMI = 46,
        }

        /// <summary>
        /// Sözleşmeli maaş bordro tablosu alanlarının isimlerini tutan sabit dizi
        /// </summary>
        public static string[] ALANLAR ={ "YIL","MUHASEBEKOD","BIRIMKOD","SURECNO","TCKIMLIKNO","MAASYIL","MAASAY","SIRANO","GUN","UCRET"
                                            ,"EKTAZMINAT","ASGGECIMINDIRIMI","OZELSIGORTA","SAKATLIKINDIRIM","GELIRVERGISIORAN","GELIRVERGISI","DAMGAVERGISIORAN","DAMGAVERGISI"
                                            ,"ASILGELIRVERGISI","SSKISVERENPAYIORAN","SSKISVERENPAYI","SSKISCIPAYIORAN","SSKISCIPAYI","LOJMAN","ICRA"
                                            ,"DIGERKESINTI","KESINTITOPLAM","ODENMESIGEREKEN","KUMVERGIMATRAHI","VERGIMATRAHI","TAHAKKUKTOPLAM"
                                            ,"MAXMAASGOSTERGE","AYLIKKATSAYI","BRUTASGARIUCRET","ISSIZSIGPRIMORAN","EKTAZMINATORAN" ,"ASGGECIMINDKATSAYI","BANKAKODU","BANKASUBEKODU","BANKAHESAPNO","UNVANKODU","MEDENIDURUM"
                                            ,"SENDIKADEVLETYARDIMI","SENDIKAAIDATI", "NAFAKA", "BORDROTUR", "AILEYARDIMI"};

        /// <summary>
        /// Sözleşmeli maaş bordro muhasebe işlem fişi belge numarası bilgisi tablo adını tutan sabit değişken
        /// </summary>
        static string TABLOADMIF = "PSLSOZLESMELIMAASMIF";

        /// <summary>
        /// Sözleşmeli maaş bordro muhasebe işlem fişi belge numarası bilgisi tablo alanlarının sırasını tutan enumeration
        /// </summary>
        enum ENUMAlanlarMIF
        {
            /// <summary>
            /// YIL alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            YIL = 0,
            /// <summary>
            /// MUHASEBEKOD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            MUHASEBEKOD = 1,
            /// <summary>
            /// BIRIMKOD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            BIRIMKOD = 2,
            /// <summary>
            /// SURECNO alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            SURECNO = 3,
            /// <summary>
            /// BELGENO alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            BELGENO = 4
        }

        /// <summary>
        /// Sözleşmeli maaş bordro muhasebe işlem fişi belge numarası tablosu alanlarının isimlerini tutan sabit dizi
        /// </summary>
        static string[] ALANLARMIF = { "YIL", "MUHASEBEKOD", "BIRIMKOD", "SURECNO", "BELGENO" };

        /// <summary>
        /// Sözleşmeli maaş bordro bilgisini veri tabanına eklemek için
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="list">Eklenecek sözleşmeli maaş bordro bilgisini tutan nesne</param>
        /// <returns>Oluşturulan insert komutunun çalıştırılması sonucunu, Sonuc yapısında döndürür</returns>
        public Sonuc Kaydet(ObjectArray list)
        {
            Sonuc sonuc = new Sonuc(true, "", "");
            string sql = "";

            int siraNo = 0;
            foreach (TNS.HRC.PSLSozlesmeliMaasBordro bilgi in list.objeler)
            {
                siraNo++;
                sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
                sql += VeriErisim.DegerYap(bilgi.yil) + ",";
                sql += VeriErisim.DegerYap(bilgi.muhasebe) + ",";
                sql += VeriErisim.DegerYap(bilgi.birim) + ",";
                sql += VeriErisim.DegerYap(bilgi.surecNo) + ",";
                sql += VeriErisim.DegerYap(bilgi.tCKimlikNo) + ",";
                sql += VeriErisim.DegerYap(bilgi.maasYil) + ",";
                sql += VeriErisim.DegerYap(bilgi.maasAy) + ",";
                sql += VeriErisim.DegerYap(siraNo) + ",";
                sql += VeriErisim.DegerYap(bilgi.gun) + ",";
                sql += VeriErisim.DegerYap(bilgi.ucret) + ",";
                sql += VeriErisim.DegerYap(bilgi.ekTazminat) + ",";
                sql += VeriErisim.DegerYap(bilgi.asgGecimIndirimi) + ",";
                sql += VeriErisim.DegerYap(bilgi.ozelSigorta) + ",";
                sql += VeriErisim.DegerYap(bilgi.sakatlikIndirim) + ",";
                sql += VeriErisim.DegerYap(bilgi.gelirVergisiOran) + ",";
                sql += VeriErisim.DegerYap(bilgi.gelirVergisi) + ",";
                sql += VeriErisim.DegerYap(bilgi.damgaVergisiOran) + ",";
                sql += VeriErisim.DegerYap(bilgi.damgaVergisi) + ",";
                sql += VeriErisim.DegerYap(bilgi.asilGelirVergisi) + ",";
                sql += VeriErisim.DegerYap(bilgi.sskIsverenPayiOran) + ",";
                sql += VeriErisim.DegerYap(bilgi.sskIsverenPayi) + ",";
                sql += VeriErisim.DegerYap(bilgi.sskIsciPayiOran) + ",";
                sql += VeriErisim.DegerYap(bilgi.sskIsciPayi) + ",";
                sql += VeriErisim.DegerYap(bilgi.lojman) + ",";
                sql += VeriErisim.DegerYap(bilgi.icra) + ",";
                sql += VeriErisim.DegerYap(bilgi.digerKesinti) + ",";
                sql += VeriErisim.DegerYap(bilgi.kesintiToplam) + ",";
                sql += VeriErisim.DegerYap(bilgi.odenmesiGereken) + ",";
                sql += VeriErisim.DegerYap(bilgi.kumVergiMatrahi) + ",";
                sql += VeriErisim.DegerYap(bilgi.vergiMatrahi) + ",";
                sql += VeriErisim.DegerYap(bilgi.tahakkukToplam) + ",";
                sql += VeriErisim.DegerYap(bilgi.maxMaasGostergesi) + ",";
                sql += VeriErisim.DegerYap(bilgi.aylikKatsayi) + ",";
                sql += VeriErisim.DegerYap(bilgi.brutAsgariUcret) + ",";
                sql += VeriErisim.DegerYap(bilgi.issizSigPrimOran) + ",";
                sql += VeriErisim.DegerYap(bilgi.ekTazminatOran) + ",";
                sql += VeriErisim.DegerYap(bilgi.asgGecimIndKatsayi) + ",";
                sql += VeriErisim.DegerYap(bilgi.bankaKodu) + ",";
                sql += VeriErisim.DegerYap(bilgi.bankaSubeKodu) + ",";
                sql += VeriErisim.DegerYap(bilgi.bankaHesapNo) + ",";
                sql += VeriErisim.DegerYap(bilgi.unvanKodu) + ",";
                sql += VeriErisim.DegerYap(bilgi.medeniDurumu) + ",";
                sql += VeriErisim.DegerYap(bilgi.sendikaDevletYardimi) + ",";
                sql += VeriErisim.DegerYap(bilgi.sendikaAidati) + ",";
                sql += VeriErisim.DegerYap(bilgi.nafaka) + ",";
                sql += VeriErisim.DegerYap(bilgi.bordroTur) + ",";
                sql += VeriErisim.DegerYap(bilgi.aileYardimi) + ")";


                sonuc = KomutCalistir(sql, 1);

                if (!sonuc.islemSonuc)
                    break;

            }
            return sonuc;
        }

        /// <summary>
        /// Sözleşmeli maaş bordro muhasebe işlem fişi belge numarası bilgisini veri tabanına eklemek için
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="f">Eklenecek sözleşmeli maaş bordro bilgisini tutan nesne</param>
        /// <param name="belgeNo">Eklenecek muhasebe işlem fişi belge numarası bilgisini tutan nesne</param>
        /// <returns>
        /// Oluşturulan insert komutunun çalıştırılması sonucunu, Sonuc yapısında döndürür
        /// </returns>
        public Sonuc MIFNoKaydet(PSLSozlesmeliMaasBordro f, string belgeNo)
        {
            string sql = "";
            Sonuc sonuc = new Sonuc();

            sql = "INSERT INTO " + TABLOADMIF + " (" + VeriErisim.TumAlanlar("", ALANLARMIF) + ") VALUES (";
            sql += VeriErisim.DegerYap(f.yil) + ",";
            sql += VeriErisim.DegerYap(f.muhasebe) + ",";
            sql += VeriErisim.DegerYap(f.birim) + ",";
            sql += VeriErisim.DegerYap(f.surecNo) + ",";
            sql += VeriErisim.DegerYap(belgeNo) + ")";

            sonuc = KomutCalistir(sql, 1);

            if (!sonuc.islemSonuc)
            {
                veriErisim.hata = "";

                sql = "UPDATE " + TABLOADMIF + " SET ";
                sql += VeriErisim.AtamaOl("", ALANLARMIF[(int)ENUMAlanlarMIF.BELGENO], belgeNo);
                sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARMIF[(int)ENUMAlanlarMIF.YIL], "=", f.yil));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARMIF[(int)ENUMAlanlarMIF.SURECNO], "=", f.surecNo));

                sonuc = KomutCalistir(sql, 1);
            }

            return sonuc;
        }

        /// <summary>
        /// Sözleşmeli maaş bordro muhasebe işlem fişi belge numarası bilgisini veri tabanından çeken sql cümleciğini verilen kıstasa göre
        /// üretip, çalıştırıp, datareader döndüren yordam.
        /// </summary>
        /// <param name="f">Listeleme için kullanılacak sözleşmeli maaş bordro bilgisi kıstas parametresi</param>
        /// <returns>
        /// Sözleşmeli maaş bordro muhasebe işlem fişi belge numarası bilgisini bulmak için oluşturulan select komutunun çalıştırılması sonucu elde edilen
        /// DataReader nesnesini döndürür
        /// </returns>
        public IDataReader MIFNoListele(PSLSozlesmeliMaasBordro f)
        {
            string sql = "";

            sql = "SELECT " + VeriErisim.TumAlanlar("", ALANLARMIF) + " FROM " + TABLOADMIF;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARMIF[(int)ENUMAlanlarMIF.YIL], "=", f.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARMIF[(int)ENUMAlanlarMIF.SURECNO], "=", f.surecNo));

            return veriErisim.DataReaderDondur(sql);

        }

        /// <summary>
        /// Sözleşmeli maaş bordro bilgisini veri tabanından çeken sql cümleciğini verilen kıstasa göre
        /// üretip, çalıştırıp, datareader döndüren yordam.
        /// </summary>
        /// <param name="bilgi">Listeleme için kullanılacak sözleşmeli maaş bordro bilgisi kıstas parametresi</param>
        /// <returns>
        /// Sözleşmeli maaş bordro bilgisini bulmak için oluşturulan select komutunun çalıştırılması sonucu elde edilen
        /// DataReader nesnesini döndürür
        /// </returns>
        public IDataReader Listele(TNS.HRC.PSLSozlesmeliMaasBordro bilgi)
        {
            string sql = "";
            string kosul = "";

            sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR) + "," + DBDegiskeni.TABLOADBANKA + ".AD " + "," + DBDegiskeni.TABLOADBANKASUBE + ".AD " + "," + DBDegiskeni.TABLOADUNVAN + ".AD " + "," + DBDegiskeni.TABLOADPERSONEL + ".AD " + "," + DBDegiskeni.TABLOADPERSONEL + ".SOYAD " + "," + DBDegiskeni.TABLOADPERSONEL + ".SICILNO " + "FROM " + TABLOAD
            + " LEFT OUTER JOIN HRCSUREC ON PSLSOZLESMELIMAASBORDRO.YIL = HRCSUREC.YIL AND PSLSOZLESMELIMAASBORDRO.SURECNO = HRCSUREC.SURECNO "
                + " LEFT OUTER JOIN " + DBDegiskeni.TABLOADBANKA + " ON " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.BANKAKODU] + " = " + DBDegiskeni.TABLOADBANKA + ".KOD " +
              " LEFT OUTER JOIN " + DBDegiskeni.TABLOADBANKASUBE + " ON " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.BANKASUBEKODU] + " = " + DBDegiskeni.TABLOADBANKASUBE + ".KOD " +
              " AND " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.BANKAKODU] + " = " + DBDegiskeni.TABLOADBANKASUBE + ".BANKAKODU" +
            " LEFT OUTER JOIN " + DBDegiskeni.TABLOADUNVAN + " ON " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.UNVANKODU] + " = " + DBDegiskeni.TABLOADUNVAN + ".KOD " +
            " LEFT OUTER JOIN " + DBDegiskeni.TABLOADPERSONEL + " ON " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.TCKIMLIKNO] + " = " + DBDegiskeni.TABLOADPERSONEL + ".TCKIMLIKNO ";

            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", bilgi.muhasebe));
            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.BIRIMKOD], "=", bilgi.birim));
            kosul = VeriErisim.KosulEkle(kosul, "AND", " HRCSUREC.SURECDURUM <> 9 ");

            if (bilgi.yil > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.YIL], "=", bilgi.yil));
            if (bilgi.surecNo != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.SURECNO], "=", bilgi.surecNo));
            if (bilgi.bankaKodu != null && bilgi.bankaKodu != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(DBDegiskeni.TABLOADBANKASUBE, ALANLAR[(int)ENUMAlanlar.BANKAKODU], "=", bilgi.bankaKodu));
            if (bilgi.maasYil > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.MAASYIL], "=", bilgi.maasYil));
            if (bilgi.maasAy > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.MAASAY], "=", bilgi.maasAy));
            if (bilgi.tCKimlikNo != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.TCKIMLIKNO], "=", bilgi.tCKimlikNo));
            //if (bilgi.bordroTur > 0)
            //    sql = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.BORDROTUR], "=", bilgi.bordroTur));

            if (kosul != "")
                sql += " WHERE " + kosul;

            sql += " ORDER BY " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.MAASYIL] +
                "," + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.MAASAY] +
                "," + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.BANKAKODU] +
                "," + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.BANKASUBEKODU] +
                "," + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.SIRANO];

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Asgari geçim indirimi bordo bilgisini veri tabanından çeken sql cümleciğini verilen kıstasa göre
        /// üretip, çalıştırıp, datareader döndüren yordam.
        /// </summary>
        /// <param name="bilgi">Listeleme için kullanılacak sözleşmeli maaş bordro bilgisi kıstas parametresi</param>
        /// <returns>
        /// Asgari geçim indirimi bordro bilgisini bulmak için oluşturulan select komutunun çalıştırılması sonucu elde edilen
        /// DataReader nesnesini döndürür
        /// </returns>
        public IDataReader ListeleAsgariGecimBordro(TNS.HRC.PSLSozlesmeliMaasBordro bilgi)
        {
            string sql = "";
            string kosul = "";

            sql = "SELECT     PSLSOZLESMELIMAASBORDRO.TCKIMLIKNO, HRCPERSONEL.ADSOYAD, PSLSOZLESMELIMAASBORDRO.MAASYIL, " +
                      "PSLSOZLESMELIMAASBORDRO.MAASAY, PSLSOZBORDROTANIM.ASGARIGECIMINDIRIM, PSLSOZLESMELIMAASBORDRO.ASGGECIMINDIRIMI,  " +
                      "PSLMAASTEMELBILGITANIM.BRUTASGARIUCRET " +
                    "FROM         PSLSOZLESMELIMAASBORDRO INNER JOIN " +
                      "HRCSUREC ON PSLSOZLESMELIMAASBORDRO.YIL = HRCSUREC.YIL AND  " +
                      "PSLSOZLESMELIMAASBORDRO.MUHASEBEKOD = HRCSUREC.MUHASEBEKOD AND  " +
                      "PSLSOZLESMELIMAASBORDRO.BIRIMKOD = HRCSUREC.BIRIMKOD AND  " +
                      "PSLSOZLESMELIMAASBORDRO.SURECNO = HRCSUREC.SURECNO LEFT OUTER JOIN " +
                      "HRCPERSONEL ON PSLSOZLESMELIMAASBORDRO.TCKIMLIKNO = HRCPERSONEL.TCKIMLIKNO LEFT OUTER JOIN " +
                      "PSLSOZBORDROTANIM ON PSLSOZLESMELIMAASBORDRO.MUHASEBEKOD = PSLSOZBORDROTANIM.MUHASEBEKOD AND  " +
                      "PSLSOZLESMELIMAASBORDRO.BIRIMKOD = PSLSOZBORDROTANIM.BIRIMKOD AND  " +
                      "PSLSOZLESMELIMAASBORDRO.MAASYIL = PSLSOZBORDROTANIM.YIL AND  " +
                      "PSLSOZLESMELIMAASBORDRO.MAASAY = PSLSOZBORDROTANIM.AY AND  " +
                      "PSLSOZLESMELIMAASBORDRO.TCKIMLIKNO = PSLSOZBORDROTANIM.TCKIMLIKNO LEFT OUTER JOIN " +
                      "PSLMAASTEMELBILGITANIM ON PSLSOZLESMELIMAASBORDRO.MAASYIL = PSLMAASTEMELBILGITANIM.YIL AND  " +
                      "PSLSOZLESMELIMAASBORDRO.MAASAY = PSLMAASTEMELBILGITANIM.AY ";


            kosul += "(HRCSUREC.SURECDURUM <> 9)";
            if (bilgi.yil > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.YIL], "=", bilgi.yil));
            if (bilgi.muhasebe > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", bilgi.muhasebe));
            if (bilgi.birim != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.BIRIMKOD], "=", bilgi.birim));
            //if (bilgi.bordroTur > 0)
            //    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.BORDROTUR], "=", bilgi.bordroTur));

            if (kosul != "")
                sql += " WHERE " + kosul;

            sql += "ORDER BY PSLSOZLESMELIMAASBORDRO.TCKIMLIKNO, PSLSOZLESMELIMAASBORDRO.MAASYIL, PSLSOZLESMELIMAASBORDRO.MAASAY";

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Sözleşmeli maaş bordrosu için fark bordrosu olup olmadığı bilgisini veri tabanından çeken sql cümleciğini verilen kıstasa göre
        /// üretip, çalıştırıp, double döndüren yordam.
        /// </summary>
        /// <param name="f">Listeleme için kullanılacak sözleşmeli maaş bordro bilgisi kıstas parametresi</param>
        /// <returns>
        /// Sözleşmeli maaş bordro için fark bordrosu olup olmadığı bilgisini bulmak için oluşturulan select komutunun çalıştırılması sonucu elde edilen
        /// DataReader nesnesini döndürür
        /// </returns>
        public double FarkBordrosuVarmi(PSLSozlesmeliMaasBordro f)
        {
            string sql = "";

            sql = "SELECT COUNT(*) FROM " + TABLOAD;

            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", f.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", f.muhasebe));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.BIRIMKOD], "=", f.birim));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SURECNO], "=", f.surecNo));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.BORDROTUR], "=", (int)ENUMBordroTur.FARK));
            if (f.maasYil > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.MAASYIL], "=", f.maasYil));
            if (f.maasAy > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.MAASAY], "=", f.maasAy));

            return veriErisim.TekKolonSQLDbl(sql);
        }

        /// <summary>
        /// Sözleşmeli maaş bordro bilgisini veri tabanından silmek için
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="bilgi">Silinecek sözleşmeli maaş bordro bilgisini tutan nesne</param>
        /// <returns>Oluşturulan delete komutunun çalıştırılması sonucunu, Sonuc yapısında döndürür</returns>
        public Sonuc Sil(TNS.HRC.PSLSozlesmeliMaasBordro bilgi)
        {
            Sonuc sonuc = new Sonuc(true, "", "");

            string sql = "";

            sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", bilgi.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", bilgi.muhasebe));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.BIRIMKOD], "=", bilgi.birim));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SURECNO], "=", bilgi.surecNo));
            if (bilgi.bordroTur > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.BORDROTUR], "=", bilgi.bordroTur));
            sonuc = KomutCalistir(sql, 0);

            if (!sonuc.islemSonuc)
                return sonuc;

            return sonuc;
        }
    }

}