using System;
using System.Data;
using TNS.Veri;

namespace TNS.HRC.IsKurallari.Veri
{
    /// <summary>
    /// Maaş temel tanım bilgisini tutan PSLMAASTEMELBILGITANIM veri tabanı tablosu ile ilgili, 
    /// listeleme ve yazma işlemleri tutan nesne
    /// </summary>
    public class TABMaasTemelBilgi : TABOrtak
    {
        /// <summary>
        /// Sınıfın oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABMaasTemelBilgi(VeriErisim veriErisim) : base(veriErisim) { }

        /// <summary>
        /// Maaş temel tanım bilgisi tablo adını tutan sabit değişken
        /// </summary>
        static string TABLOAD = "PSLMAASTEMELBILGITANIM";

        /// <summary>
        /// Maaş temel tanım bilgisi tablo alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlar
        {
            /// <summary>
            /// YIL alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            YIL = 0,
            /// <summary>
            /// AY alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            AY = 1,
            /// <summary>
            /// GELIRVERGISIORAN alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            GELIRVERGISIORAN = 2,
            /// <summary>
            /// ENYUKSMAASGOSTERGE alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            ENYUKSMAASGOSTERGE = 3,
            /// <summary>
            /// AYLIKKATSAYI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            AYLIKKATSAYI = 4,
            /// <summary>
            /// TABANAYLIKKATSAYI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            TABANAYLIKKATSAYI = 5,
            /// <summary>
            /// YANODEMEKATSAYI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            YANODEMEKATSAYI = 6,
            /// <summary>
            /// SSKISVERENPAYI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            SSKISVERENPAYI = 7,
            /// <summary>
            /// SSKISCIPAYI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            SSKISCIPAYI = 8,
            /// <summary>
            /// SSKISSIZLIKSIGISVERENPAYI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            SSKISSIZLIKSIGISVERENPAYI = 9,
            /// <summary>
            /// SSKISSIZLIKSIGISCIPAYI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            SSKISSIZLIKSIGISCIPAYI = 10,
            /// <summary>
            /// DAMGAVERGISIORAN alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            DAMGAVERGISIORAN = 11,
            /// <summary>
            /// BRUTASGARIUCRET alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            BRUTASGARIUCRET = 12,
            /// <summary>
            /// ISKAZASIMESLEKIHAST alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            ISKAZASIMESLEKIHAST = 13,
            /// <summary>
            /// ANALIKSIGORTASI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi) 
            /// </summary>
            ANALIKSIGORTASI = 14,
            /// <summary>
            /// HASTALIKSIGORTASI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            HASTALIKSIGORTASI = 15,
            /// <summary>
            /// MALULYASLIOLUMSIG alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            MALULYASLIOLUMSIG = 16,
            /// <summary>
            /// NETASGARIUCRET alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            NETASGARIUCRET = 17,

            SAKATLIKINDIRIM1 = 18,
            SAKATLIKINDIRIM2 = 19,
            SAKATLIKINDIRIM3 = 20,
            KIRA1 = 21,
            YAKIT1 = 22,
            SSKTAVAN = 23,
            TOPLUGORUSMESABITI = 24,
            AILEYARDIMIKATSAYISI = 25,
        }

        /// <summary>
        /// Maaş temel tanım bilgisi tablosu alanlarının isimlerini tutan sabit dizi
        /// </summary>
        static string[] ALANLAR = { "YIL", "AY", "GELIRVERGISIORAN", "ENYUKSMAASGOSTERGE", "AYLIKKATSAYI", "TABANAYLIKKATSAYI", "YANODEMEKATSAYI",
            "SSKISVERENPAYI", "SSKISCIPAYI", "SSKISSIZLIKSIGISVERENPAYI", "SSKISSIZLIKSIGISCIPAYI", "DAMGAVERGISIORAN",
            "BRUTASGARIUCRET", "ISKAZASIMESLEKIHAST", "ANALIKSIGORTASI", "HASTALIKSIGORTASI", "MALULYASLIOLUMSIG", "NETASGARIUCRET", "SAKATLIKINDIRIM1", "SAKATLIKINDIRIM2", "SAKATLIKINDIRIM3", "KIRA1", "YAKIT1", "SSKTAVAN", "TOPLUGORUSMESABITI", "AILEYARDIMIKATSAYISI" };

        /// <summary>
        /// Maaş temel tanım bilgisini veri tabanına eklemek için
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="bilgi">Eklenecek maaş temel tanım bilgisini tutan nesne</param>
        /// <returns>
        /// Oluşturulan insert komutunun çalıştırılması sonucunu, Sonuc yapısında döndürür
        /// </returns>
        public Sonuc Kaydet(ObjectArray bilgi)
        {
            Sonuc sonuc = new Sonuc(true, "", "");
            string sql = "";

            foreach (TNS.HRC.PSLMaasTemelBilgisi f in bilgi.objeler)
            {
                sql = "DELETE FROM " + TABLOAD;
                sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", f.yil));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.AY], "=", f.ay));
                sonuc = KomutCalistir(sql, 0);
                if (!sonuc.islemSonuc)
                    break;

                sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
                sql += VeriErisim.DegerYap(f.yil);
                sql += ", " + VeriErisim.DegerYap(f.ay);
                sql += ", " + VeriErisim.DegerYap(f.gelirVergisiOran);
                sql += ", " + VeriErisim.DegerYap(f.enYuksMemurMaasGost);
                sql += ", " + VeriErisim.DegerYap(f.aylikKatsayi);
                sql += ", " + VeriErisim.DegerYap(f.tabanAylikKatsayi);
                sql += ", " + VeriErisim.DegerYap(f.yanOdemeKatsayi);
                sql += ", " + VeriErisim.DegerYap(f.sskIsverenPayiOran);
                sql += ", " + VeriErisim.DegerYap(f.sskIsciPayiOran);
                sql += ", " + VeriErisim.DegerYap(f.sskIssizSigIsVerenPayiOran);
                sql += ", " + VeriErisim.DegerYap(f.sskIssizSigIsciPayiOran);
                sql += ", " + VeriErisim.DegerYap(f.damgaVergisiOran);
                sql += ", " + VeriErisim.DegerYap(f.brutAsgariUcret);
                sql += ", " + VeriErisim.DegerYap(f.isKazasiMeslekHast);
                sql += ", " + VeriErisim.DegerYap(f.analikSigortasi);
                sql += ", " + VeriErisim.DegerYap(f.hastalikSigortasi);
                sql += ", " + VeriErisim.DegerYap(f.malulYasliOlumSigortasi);
                sql += ", " + VeriErisim.DegerYap(f.netAsgariUcret);
                sql += ", " + VeriErisim.DegerYap(f.sakatlikIndirimi1);
                sql += ", " + VeriErisim.DegerYap(f.sakatlikIndirimi2);
                sql += ", " + VeriErisim.DegerYap(f.sakatlikIndirimi3);
                sql += ", " + VeriErisim.DegerYap(f.kira1);
                sql += ", " + VeriErisim.DegerYap(f.yakit1);
                sql += ", " + VeriErisim.DegerYap(f.sskTavan);
                sql += ", " + VeriErisim.DegerYap(f.topluGorusmeSabiti);
                sql += ", " + VeriErisim.DegerYap(f.aileYardimiKatsayisi);
                sql += ") ";

                sonuc = KomutCalistir(sql, 1);
                if (!sonuc.islemSonuc)
                    break;
            }
            return sonuc;
        }

        /// <summary>
        /// Maaş temel tanım bilgisini veri tabanından çeken sql cümleciğini verilen kıstasa göre
        /// üretip, çalıştırıp, datareader döndüren yordam.
        /// </summary>
        /// <param name="bilgi">Listeleme için kullanılacak maaş temel tanım bilgisi kıstas parametresi</param>
        /// <returns>
        /// Maaş temel tanım bilgisini bulmak için oluşturulan select komutunun çalıştırılması sonucu elde edilen
        /// DataReader nesnesini döndürür
        /// </returns>
        public IDataReader Listele(TNS.HRC.PSLMaasTemelBilgisi bilgi)
        {
            string sql = "";
            string kosul = "";

            sql = "SELECT " + VeriErisim.TumAlanlar("", ALANLAR) + " FROM " + TABLOAD;
            if (bilgi.yil > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", bilgi.yil));
            if (bilgi.ay > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.AY], "=", bilgi.ay));

            if (kosul != "")
                sql += " WHERE " + kosul;

            sql += " ORDER BY " + ALANLAR[(int)ENUMAlanlar.YIL] + "," + ALANLAR[(int)ENUMAlanlar.AY];

            return veriErisim.DataReaderDondur(sql);
        }
    }
}
