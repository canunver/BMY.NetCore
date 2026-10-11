using System;
using System.Data;
using TNS.Veri;

namespace TNS.HRC.IsKurallari.Veri
{
    /// <summary>
    /// İşçi maaş bordro bilgisini tutan PSLISCIMAASBORDRO veri tabanı tablosu ile ilgili, 
    /// listeleme ve yazma işlemleri tutan nesne
    /// </summary>
    public class TABIsciMaasBordro : TABOrtak
    {
        /// <summary>
        /// Sınıfın oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABIsciMaasBordro(VeriErisim veriErisim) : base(veriErisim) { }

        /// <summary>
        /// İşçi maaş bordro bilgisi tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLOAD = "PSLISCIMAASBORDRO";

        /// <summary>
        /// İşçi maaş bordro bilgisi tablo alanlarının sırasını tutan enumeration
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
            /// BORDROTIP alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            BORDROTIP = 5,
            /// <summary>
            /// FARKMI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            FARKMI = 6,
            /// <summary>
            /// MAASYIL alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            MAASYIL = 7,
            /// <summary>
            /// MAASAY alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            MAASAY = 8,
            /// <summary>
            /// SIRANO alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            SIRANO = 9,
            /// <summary>
            /// GUN alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            GUN = 10,
            /// <summary>
            /// YEVMIYE alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            YEVMIYE = 11,
            /// <summary>
            /// UCRET alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            UCRET = 12,
            /// <summary>
            /// SAKATLIKINDIRIMI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            SAKATLIKINDIRIMI = 13,
            /// <summary>
            /// OZELSIGORTA alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            OZELSIGORTA = 14,
            /// <summary>
            /// PRIM alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            PRIM = 15,
            /// <summary>
            /// GUNTASIT alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            GUNTASIT = 16,
            /// <summary>
            /// TASITYARDIMI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            TASITYARDIMI = 17,
            /// <summary>
            /// TASITYARDIMIKATSAYI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            TASITYARDIMIKATSAYI = 18,
            /// <summary>
            /// GUNYEMEK alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            GUNYEMEK = 19,
            /// <summary>
            /// YEMEKYARDIMI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            YEMEKYARDIMI = 20,
            /// <summary>
            /// YEMEKYARDIMIKATSAYI
            /// </summary>
            YEMEKYARDIMIKATSAYI = 21,
            /// <summary>
            /// HAKSSKISVERENPAYI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            HAKSSKISVERENPAYI = 22,
            /// <summary>
            /// HAKSSKPRIMINEESASKAZANC alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            HAKSSKPRIMINEESASKAZANC = 23,
            /// <summary>
            /// HAKISSIZLIKSIGISVERENPAYI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            HAKISSIZLIKSIGISVERENPAYI = 24,
            /// <summary>
            /// HAKTOPLAMKAZANC alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            HAKTOPLAMKAZANC = 25,
            /// <summary>
            /// KESDAMGAVERGISIMATRAHI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            KESDAMGAVERGISIMATRAHI = 26,
            /// <summary>
            /// KESGELIRVERGISIMATRAHI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            KESGELIRVERGISIMATRAHI = 27,
            /// <summary>
            /// KESGELIRVERGISI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            KESGELIRVERGISI = 28,
            /// <summary>
            /// GELIRVERGISIORAN alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            GELIRVERGISIORAN = 29,
            /// <summary>
            /// KESDAMGAVERGISI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            KESDAMGAVERGISI = 30,
            /// <summary>
            /// DAMGAVERGISIORAN alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            DAMGAVERGISIORAN = 31,
            /// <summary>
            /// KESSSKPRIMISVERENPAYI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            KESSSKPRIMISVERENPAYI = 32,
            /// <summary>
            /// SSKISVERENORAN alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            SSKISVERENORAN = 33,
            /// <summary>
            /// KESSSKPRIMIISCIPAYI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            KESSSKPRIMISCIPAYI = 34,
            /// <summary>
            /// SSKISCIORANI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            SSKISCIORANI = 35,
            /// <summary>
            /// KESISSIZLIKSIGISVERENPAYI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            KESISSIZLIKSIGISVERENPAYI = 36,
            /// <summary>
            /// ISSIZLIKSIGISVERENORAN alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            ISSIZLIKSIGISVERENORAN = 37,
            /// <summary>
            /// KESISSIZLIKSIGISCIPAYI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            KESISSIZLIKSIGISCIPAYI = 38,
            /// <summary>
            /// ISSIZLIKSIGISCIORAN alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            ISSIZLIKSIGISCIORAN = 39,
            /// <summary>
            /// SENDIKAAIDAT alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            SENDIKAAIDAT = 40,
            /// <summary>
            /// BRUTASGARIUCRET alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            BRUTASGARIUCET = 41,
            /// <summary>
            /// ASGARIGECIMINDIRIMI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            ASGARIGECIMINDIRIMI = 42,
            /// <summary>
            /// ASGGECIMINDKATSAYI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            ASGGECIMINDKATSAYI = 43,
            /// <summary>
            /// TOPLAMKESINTI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            TOPLAMKESINTI = 44,
            /// <summary>
            /// NETODENEN alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            NETODENEN = 45,
            /// <summary>
            /// KUMULATIFVERGIMATRAHI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            KUMULATIFVERGIMATRAHI = 46,
            /// <summary>
            /// BANKAKKODU alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            BANKAKODU = 47,
            /// <summary>
            /// BANKASUBEKODU alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            BANKASUBEKODU = 48,
            /// <summary>
            /// BANKAHESAPNO alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            BANKAHESAPNO = 49,
            /// <summary>
            /// UNVANKODU alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            UNVANKODU = 50,
            /// <summary>
            /// MEDENIDURUM alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            MEDENIDURUM = 51,
            /// <summary>
            /// DIGERKESINTI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            DIGERKESINTI = 52,
            /// <summary>
            /// SOSYALYARDIM alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            SOSYALYARDIM = 53,

        }

        /// <summary>
        /// İşçi maaş bordro tablosu alanlarının isimlerini tutan sabit dizi
        /// </summary>
        public static string[] ALANLAR ={ "YIL","MUHASEBEKOD","BIRIMKOD","SURECNO","TCKIMLIKNO","BORDROTIP","FARKMI","MAASYIL","MAASAY","SIRANO"
                                            ,"GUN","YEVMIYE","UCRET","SAKATLIKINDIRIMI","OZELSIGORTA","PRIM","GUNTASIT" 
                                            ,"TASITYARDIMI","TASITYARDIMIKATSAYI","GUNYEMEK","YEMEKYARDIMI","YEMEKYARDIMIKATSAYI","HAKSSKISVERENPAYI","HAKSSKPRIMINEESASKAZANC"
                                            ,"HAKISSIZSIGISVERENPAYI","HAKTOPLAMKAZANC","KESDAMGAVERGISIMATRAHI","KESGELIRVERGISIMATRAHI"
                                            ,"KESGELIRVERGISI","GELIRVERGISIORAN","KESDAMGAVERGISI","DAMGAVERGISIORAN","KESSSKPRIMISVERENPAYI"
                                            ,"SSKISVERENORAN","KESSSKPRIMISCIPAYI","SSKISCIORANI","KESISSIZSIGISVERENPAYI","ISSIZSIGISVERENORAN"
                                            ,"KESISSIZSIGISCIPAYI","ISSIZSIGISCIORAN","SENDIKAAIDAT","BRUTASGARIUCET","ASGARIGECIMINDIRIMI"
                                            ,"ASGGECIMINDKATSAYI","TOPLAMKESINTI","NETODENEN","KUMULATIFVERGIMATRAHI","BANKAKODU","BANKASUBEKODU","BANKAHESAPNO","UNVANKODU","MEDENIDURUM","DIGERKESINTI","SOSYALYARDIM" };

        /// <summary>
        /// İşçi maaş bordro bilgisini veri tabanına eklemek için
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="list">Eklenecek işçi maaş bordro bilgisini tutan nesne</param>
        /// <returns>Oluşturulan insert komutunun çalıştırılması sonucunu, Sonuc yapısında döndürür</returns>
        public Sonuc Kaydet(ObjectArray list)
        {
            Sonuc sonuc = new Sonuc(true, "", "");
            string sql = "";

            int siraNo = 0;
            foreach (IsciMaasBordro bilgi in list.objeler)
            {
                siraNo++;
                sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
                sql += VeriErisim.DegerYap(bilgi.yil) + ",";
                sql += VeriErisim.DegerYap(bilgi.muhasebe) + ",";
                sql += VeriErisim.DegerYap(bilgi.birim) + ",";
                sql += VeriErisim.DegerYap(bilgi.surecNo) + ",";
                sql += VeriErisim.DegerYap(bilgi.tCKimlikNo) + ",";
                sql += VeriErisim.DegerYap(bilgi.bordroTip) + ",";
                sql += VeriErisim.DegerYap(bilgi.farkmi) + ",";
                sql += VeriErisim.DegerYap(bilgi.maasYil) + ",";
                sql += VeriErisim.DegerYap(bilgi.maasAy) + ",";
                sql += VeriErisim.DegerYap(siraNo) + ",";
                sql += VeriErisim.DegerYap(bilgi.gun) + ",";
                sql += VeriErisim.DegerYap(bilgi.yevmiye) + ",";
                sql += VeriErisim.DegerYap(bilgi.ucret) + ",";
                sql += VeriErisim.DegerYap(bilgi.sakatlikIndirimi) + ",";
                sql += VeriErisim.DegerYap(bilgi.ozelSigorta) + ",";
                sql += VeriErisim.DegerYap(bilgi.prim) + ",";
                sql += VeriErisim.DegerYap(bilgi.tasitGunu) + ",";
                sql += VeriErisim.DegerYap(bilgi.tasitYardimi) + ",";
                sql += VeriErisim.DegerYap(bilgi.tasitYardimKatsayi) + ",";
                sql += VeriErisim.DegerYap(bilgi.yemekGunu) + ",";
                sql += VeriErisim.DegerYap(bilgi.yemekYardimi) + ",";
                sql += VeriErisim.DegerYap(bilgi.yemekYardimKatsayi) + ",";
                sql += VeriErisim.DegerYap(bilgi.hakSskIsverenPayi) + ",";
                sql += VeriErisim.DegerYap(bilgi.hakSskPrimineEsasKazanc) + ",";
                sql += VeriErisim.DegerYap(bilgi.hakIssizlikSigIsverenPayi) + ",";
                sql += VeriErisim.DegerYap(bilgi.hakToplamKazanc) + ",";
                sql += VeriErisim.DegerYap(bilgi.kesDamgaVergisiMatrah) + ",";
                sql += VeriErisim.DegerYap(bilgi.kesGelirVergisiMatrah) + ",";
                sql += VeriErisim.DegerYap(bilgi.kesGelirVergisi) + ",";
                sql += VeriErisim.DegerYap(bilgi.gelirVergisiOrani) + ",";
                sql += VeriErisim.DegerYap(bilgi.kesDamgaVergisi) + ",";
                sql += VeriErisim.DegerYap(bilgi.damgaVergisiOrani) + ",";
                sql += VeriErisim.DegerYap(bilgi.kesSskPrimIsverenPayi) + ",";
                sql += VeriErisim.DegerYap(bilgi.sskIsverenOran) + ",";
                sql += VeriErisim.DegerYap(bilgi.kesSskPrimIsciPayi) + ",";
                sql += VeriErisim.DegerYap(bilgi.sskIsciOran) + ",";
                sql += VeriErisim.DegerYap(bilgi.kesIssizSigIsverenPayi) + ",";
                sql += VeriErisim.DegerYap(bilgi.issizlikSigIsverenOran) + ",";
                sql += VeriErisim.DegerYap(bilgi.kesIssizSigIsciPayi) + ",";
                sql += VeriErisim.DegerYap(bilgi.issizSigIsciOran) + ",";
                sql += VeriErisim.DegerYap(bilgi.sendikaAidat) + ",";
                sql += VeriErisim.DegerYap(bilgi.brutAsgariUcret) + ",";
                sql += VeriErisim.DegerYap(bilgi.asgariGecimIndirim) + ",";
                sql += VeriErisim.DegerYap(bilgi.asgGecimIndKatsayi) + ",";
                sql += VeriErisim.DegerYap(bilgi.toplamKesinti) + ",";
                sql += VeriErisim.DegerYap(bilgi.netOdenen) + ",";
                sql += VeriErisim.DegerYap(bilgi.kumulatifVergiMatrahi) + ",";
                sql += VeriErisim.DegerYap(bilgi.bankaKodu) + ",";
                sql += VeriErisim.DegerYap(bilgi.bankaSubeKodu) + ",";
                sql += VeriErisim.DegerYap(bilgi.bankaHesapNo) + ",";
                sql += VeriErisim.DegerYap(bilgi.unvanKodu) + ",";
                sql += VeriErisim.DegerYap(bilgi.medeniDurumu) + ",";
                sql += VeriErisim.DegerYap(bilgi.digerKesinti) + ",";
                sql += VeriErisim.DegerYap(bilgi.sosyalYardim) + ")";

                sonuc = KomutCalistir(sql, 1);

                if (!sonuc.islemSonuc)
                    break;

            }
            return sonuc;
        }

        /// <summary>
        /// İşçi maaş bordro bilgisini veri tabanından çeken sql cümleciğini verilen kıstasa göre
        /// üretip, çalıştırıp, datareader döndüren yordam.
        /// </summary>
        /// <param name="bilgi">Listeleme için kullanılacak işçi maaş bordro bilgisi kıstas parametresi</param>
        /// <returns>
        /// İşçi maaş bordro bilgisini bulmak için oluşturulan select komutunun çalıştırılması sonucu elde edilen
        /// DataReader nesnesini döndürür
        /// </returns>
        public IDataReader Listele(IsciMaasBordro bilgi)
        {
            string sql = "";
            string kosul = "";

            sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR) + "," + DBDegiskeni.TABLOADBANKA + ".AD " + "," + DBDegiskeni.TABLOADBANKASUBE + ".AD " + "," + DBDegiskeni.TABLOADUNVAN + ".AD " + "," + DBDegiskeni.TABLOADPERSONEL + ".AD " + "," + DBDegiskeni.TABLOADPERSONEL + ".SOYAD " + "," + DBDegiskeni.TABLOADPERSONEL + ".SICILNO " + "FROM " + TABLOAD
                + " LEFT OUTER JOIN " + DBDegiskeni.TABLOADBANKA + " ON " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.BANKAKODU] + " = " + DBDegiskeni.TABLOADBANKA + ".KOD " +
              " LEFT OUTER JOIN " + DBDegiskeni.TABLOADBANKASUBE + " ON " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.BANKASUBEKODU] + " = " + DBDegiskeni.TABLOADBANKASUBE + ".KOD " +
              " AND " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.BANKAKODU] + " = " + DBDegiskeni.TABLOADBANKASUBE + ".BANKAKODU" +
            " LEFT OUTER JOIN " + DBDegiskeni.TABLOADUNVAN + " ON " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.UNVANKODU] + " = " + DBDegiskeni.TABLOADUNVAN + ".KOD " +
            " LEFT OUTER JOIN " + DBDegiskeni.TABLOADPERSONEL + " ON " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.TCKIMLIKNO] + " = " + DBDegiskeni.TABLOADPERSONEL + ".TCKIMLIKNO ";

            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", bilgi.muhasebe));
            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.BIRIMKOD], "=", bilgi.birim));
            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.FARKMI], "=", bilgi.farkmi));

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
            if (bilgi.bordroTip > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.BORDROTIP], "=", bilgi.bordroTip));


            if (bilgi.tCKimlikNo != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.TCKIMLIKNO], "=", bilgi.tCKimlikNo));
            //Fark bordrosu olup olmadığı her zaman sorgulansın


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
        /// İşçi maaş bordro bilgisini veri tabanından silmek için
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="bilgi">Silinecek işçi maaş bordro bilgisini tutan nesne</param>
        /// <returns>Oluşturulan delete komutunun çalıştırılması sonucunu, Sonuc yapısında döndürür</returns>
        public Sonuc Sil(IsciMaasBordro bilgi)
        {
            Sonuc sonuc = new Sonuc(true, "", "");

            string sql = "";

            sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", bilgi.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", bilgi.muhasebe));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.BIRIMKOD], "=", bilgi.birim));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SURECNO], "=", bilgi.surecNo));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.FARKMI], "=", bilgi.farkmi));
            if (bilgi.bordroTip > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.BORDROTIP], "=", bilgi.bordroTip));

            sonuc = KomutCalistir(sql, 0);

            if (!sonuc.islemSonuc)
                return sonuc;

            return sonuc;
        }
    }
}