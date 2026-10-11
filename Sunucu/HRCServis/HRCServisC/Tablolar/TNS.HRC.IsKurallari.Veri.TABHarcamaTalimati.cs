using System;
using System.Data;
using TNS.Veri;

namespace TNS.HRC.IsKurallari.Veri
{
    /// <summary>
    /// Harcama Talimatı bilgisini tutan HRCHARCAMATALIMATI veri tabanı tablosu ile ilgili, 
    /// listeleme ve yazma işlemleri tutan nesne
    /// </summary>
    public class TABHarcamaTalimati : TABOrtak
    {
        /// <summary>
        /// Sınıfın oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABHarcamaTalimati(VeriErisim veriErisim) : base(veriErisim) { }

        /// <summary>
        /// Harcama Talimatı bilgisi tablo adını tutan sabit değişken
        /// </summary>
        static string TABLOAD = "HRCHARCAMATALIMATI";

        /// <summary>
        /// Harcama Talimatı bilgisi tablo alanlarının sırasını tutan enumeration
        /// </summary>
        enum ENUMAlanlar
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
            /// HARCAMAMAKAMI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            HARACAMAMAKAMI = 4,
            /// <summary>
            /// HARCAMABIRIMI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            HARCAMABIRIMI = 5,
            /// <summary>
            /// GEREKCESI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            GEREKCESI = 6,
            /// <summary>
            /// KONUSU alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            KONUSU = 7,
            /// <summary>
            /// MIKTARACIKLAMA alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            MIKTARACIKLAMA = 8,
            /// <summary>
            /// GSURESI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            GSURESI = 9,
            /// <summary>
            /// GUSULU alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            GUSULU = 10,
            /// <summary>
            /// YAKLASIKBEDEL alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            YAKLASIKBEDEL = 11,
            /// <summary>
            /// BUTCETERTIP alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            BUTCETERTIP = 12,
            /// <summary>
            /// ODENEKTUTAR alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            ODENEKTUTAR = 13,
            /// <summary>
            /// EVRAKTARIHI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            EVRAKTARIHI = 14,
            /// <summary>
            /// EVRAKSERI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            EVRAKSERI = 15,
            /// <summary>
            /// EVRAKSAYI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            EVRAKSAYI = 16,
            /// <summary>
            /// GGOREVLI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            GGOREVLI = 17,
            /// <summary>
            /// ACIKLAMA alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            ACIKLAMA = 18,
            /// <summary>
            /// TEKLIFEDEN alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            TEKLIFEDEN = 19,
            /// <summary>
            /// TEKLIFEDENUNVAN alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            TEKLIFEDENUNVAN = 20,
            /// <summary>
            /// HRCYETKILI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            HRCYETKILI = 21,
            /// <summary>
            /// HRCYETKILIUNVAN alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            HRCYETKILIUNVAN = 22,
            /// <summary>
            /// ONAYTARIHI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            ONAYTARIHI = 23,
            /// <summary>
            /// AVANSVARMI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            AVANSVARMI = 24,
            /// <summary>
            /// MIHMANDARVARMI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            MIHMANDARVARMI = 25,
            /// <summary>
            /// PERTCNO alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            PERTCNO = 26,
            /// <summary>
            /// PERADSOYAD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            PERADSOYAD = 27,
            /// <summary>
            /// PERBANKAAD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            PERBANKAAD = 28,
            /// <summary>
            /// PERHESAPNO alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            PERHESAPNO = 29,
            /// <summary>
            /// PERVERGIDAIRESI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            PERVERGIDAIRESI = 30,
            /// <summary>
            /// PERUNVAN alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            PERUNVAN = 31,
        }

        /// <summary>
        /// Harcama Talimatı bilgisi tablosu alanlarının isimlerini tutan sabit dizi
        /// </summary>
        static string[] ALANLAR = { "YIL", "MUHASEBEKOD", "BIRIMKOD", "SURECNO", "HARACAMAMAKAMI", "HARCAMABIRIMI", "GEREKCESI", "KONUSU", "MIKTARACIKLAMA", "GSURESI", "GUSULU", "YAKLASIKBEDEL", "BUTCETERTIP", "ODENEKTUTAR", "EVRAKTARIHI", "EVRAKSERI", "EVRAKSAYI", "GGOREVLI", "ACIKLAMA", "TEKLIFEDEN", "TEKLIFEDENUNVAN", "HRCYETKILI", "HRCYETKILIUNVAN", "ONAYTARIHI", "AVANSVARMI", "MIHMANDARVARMI", "PERTCNO", "PERADSOYAD", "PERBANKAAD", "PERHESAPNO", "PERVERGIDAIRESI", "PERUNVAN" };

        /// <summary>
        /// Hakediş bilgisini veri tabanına eklemek için
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="bilgi">Eklenecek harcama talimatı bilgisini tutan nesne</param>
        /// <returns>Oluşturulan insert komutunun çalıştırılması sonucunu, Sonuc yapısında döndürür</returns>
        public Sonuc Kaydet(HarcamaTalimati bilgi)
        {
            Sonuc sonuc = new Sonuc(true, "", "");
            string sql = "";
            string kosul = "";
            {

                sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
                sql += VeriErisim.DegerYap(bilgi.yil) + ",";
                sql += VeriErisim.DegerYap(bilgi.muhasebe) + ",";
                sql += VeriErisim.DegerYap(bilgi.birim) + ",";
                sql += VeriErisim.DegerYap(bilgi.surecNo) + ",";
                sql += VeriErisim.DegerYap(bilgi.harcamaMakami) + ",";
                sql += VeriErisim.DegerYap(bilgi.harcamaBirimi) + ",";
                sql += VeriErisim.DegerYap(bilgi.gerekcesi) + ",";
                sql += VeriErisim.DegerYap(bilgi.konusu) + ",";
                sql += VeriErisim.DegerYap(bilgi.miktarAciklama) + ",";
                sql += VeriErisim.DegerYap(bilgi.gerceklestirmeSuresi) + ",";
                sql += VeriErisim.DegerYap(bilgi.gerceklestirmeUsulu) + ",";
                sql += VeriErisim.DegerYap(bilgi.yaklasikBedel) + ",";
                sql += VeriErisim.DegerYap(bilgi.butceTertip) + ",";
                sql += VeriErisim.DegerYap(bilgi.odenekTutar) + ",";
                sql += VeriErisim.DegerYap(bilgi.evrakTarihi) + ",";
                sql += VeriErisim.DegerYap(bilgi.evrakSeri) + ",";
                sql += VeriErisim.DegerYap(bilgi.evrakSayi) + ",";
                sql += VeriErisim.DegerYap(bilgi.gerceklestirmeGorevli) + ",";
                sql += VeriErisim.DegerYap(bilgi.aciklama) + ",";
                sql += VeriErisim.DegerYap(bilgi.teklifEdenYetkili) + ",";
                sql += VeriErisim.DegerYap(bilgi.teklifEdenYetkiliUnvan) + ",";
                sql += VeriErisim.DegerYap(bilgi.harcamaYetkilisi) + ",";
                sql += VeriErisim.DegerYap(bilgi.harcamaYetkilisiUnvan) + ",";
                sql += VeriErisim.DegerYap(bilgi.onayTarihi) + ",";
                sql += VeriErisim.DegerYap(bilgi.avansVarmi) + ",";
                sql += VeriErisim.DegerYap(bilgi.mihmandarlikVarmi) + ",";
                sql += VeriErisim.DegerYap(bilgi.perTcNo) + ",";
                sql += VeriErisim.DegerYap(bilgi.perAdSoyad) + ",";
                sql += VeriErisim.DegerYap(bilgi.perBankaAd) + ",";
                sql += VeriErisim.DegerYap(bilgi.perHesapNo) + ",";
                sql += VeriErisim.DegerYap(bilgi.perVergiDairesi) + ",";
                sql += VeriErisim.DegerYap(bilgi.perUnvan) + ")";


                sonuc = KomutCalistir(sql, 1);

                if (!sonuc.islemSonuc)
                {
                    veriErisim.hata = "";
                    sql = "UPDATE " + TABLOAD + " SET ";
                    sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ACIKLAMA], bilgi.aciklama) + ",";
                    sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.PERADSOYAD], bilgi.perAdSoyad) + ",";
                    sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.AVANSVARMI], bilgi.avansVarmi) + ",";
                    sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.PERBANKAAD], bilgi.perBankaAd) + ",";
                    sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.BUTCETERTIP], bilgi.butceTertip) + ",";
                    sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.EVRAKSAYI], bilgi.evrakSayi) + ",";
                    sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.EVRAKSERI], bilgi.evrakSeri) + ",";
                    sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.EVRAKTARIHI], bilgi.evrakTarihi) + ",";
                    sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.GEREKCESI], bilgi.gerekcesi) + ",";
                    sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.GGOREVLI], bilgi.gerceklestirmeGorevli) + ",";
                    sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.GSURESI], bilgi.gerceklestirmeSuresi) + ",";
                    sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.GUSULU], bilgi.gerceklestirmeUsulu) + ",";
                    sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.HARACAMAMAKAMI], bilgi.harcamaMakami) + ",";
                    sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.HARCAMABIRIMI], bilgi.harcamaBirimi) + ",";
                    sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.PERHESAPNO], bilgi.perHesapNo) + ",";
                    sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.HRCYETKILI], bilgi.harcamaYetkilisi) + ",";
                    sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.HRCYETKILIUNVAN], bilgi.harcamaYetkilisiUnvan) + ",";
                    sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ONAYTARIHI], bilgi.onayTarihi) + ",";
                    sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.KONUSU], bilgi.konusu) + ",";
                    sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.MIHMANDARVARMI], bilgi.mihmandarlikVarmi) + ",";
                    sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.MIKTARACIKLAMA], bilgi.miktarAciklama) + ",";
                    sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ODENEKTUTAR], bilgi.odenekTutar) + ",";
                    sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.PERTCNO], bilgi.perTcNo) + ",";
                    sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.TEKLIFEDEN], bilgi.teklifEdenYetkili) + ",";
                    sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.TEKLIFEDENUNVAN], bilgi.teklifEdenYetkiliUnvan) + ",";
                    sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.PERVERGIDAIRESI], bilgi.perVergiDairesi) + ",";
                    sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.YAKLASIKBEDEL], bilgi.yaklasikBedel) + ",";
                    sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.PERUNVAN], bilgi.perUnvan);

                    kosul = VeriErisim.KosulEkle(" WHERE", "", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", (bilgi.yil)));
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", (bilgi.muhasebe)));
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.BIRIMKOD], "=", (bilgi.birim)));
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SURECNO], "=", (bilgi.surecNo)));

                    sql += kosul;

                    sonuc = KomutCalistir(sql, 1);
                }

            }
            return sonuc;
        }

        /// <summary>
        /// Harcama talimatı bilgisini veri tabanından çeken sql cümleciğini verilen kıstasa göre
        /// üretip, çalıştırıp, datareader döndüren yordam.
        /// </summary>
        /// <param name="bilgi">Listeleme için kullanılacak hakediş bilgisi kıstas parametresi</param>
        /// <returns>
        /// Harcama talimatı bilgisini bulmak için oluşturulan select komutunun çalıştırılması sonucu elde edilen
        /// DataReader nesnesini döndürür
        /// </returns>
        public IDataReader Listele(HarcamaTalimati bilgi)
        {
            string sql = "";
            string kosul = "";

            sql = "SELECT " + VeriErisim.TumAlanlar("", ALANLAR) + " FROM " + TABLOAD;
            if (bilgi.yil != 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", bilgi.yil));
            if (bilgi.muhasebe != 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", bilgi.muhasebe));
            if (bilgi.birim != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.BIRIMKOD], "=", bilgi.birim));
            if (bilgi.surecNo != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SURECNO], "=", bilgi.surecNo));

            if (kosul != "")
                sql += " WHERE " + kosul;

            sql += " ORDER BY " + ALANLAR[(int)ENUMAlanlar.SURECNO];

            return veriErisim.DataReaderDondur(sql);
        }
    }
}