using System;
using System.Data;
using TNS.Veri;

namespace TNS.HRC.IsKurallari.Veri
{
    /// <summary>
    /// Fatura bilgisini tutan HRCFATURAFORM ve HRCFATURADETAY veri tabanı tabloları ile ilgili, 
    /// listeleme ve yazma işlemleri tutan nesne
    /// </summary>
    public class TABFaturaForm : TABOrtak
    {
        /// <summary>
        /// Sınıfın oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABFaturaForm(VeriErisim veriErisim) : base(veriErisim) { }

        /// <summary>
        /// Fatura tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLOAD = "HRCFATURAFORM";

        /// <summary>
        /// Fatura tablo alanlarının sırasını tutan enumeration
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
            /// PRFATURANO alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            PRFATURANO = 4,
            /// <summary>
            /// FIRMANO alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            FIRMANO = 5,
            /// <summary>
            /// FATURANO alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            FATURANO = 6,
            /// <summary>
            /// FATURATARIH alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            FATURATARIH = 7,
            /// <summary>
            /// ACIKLAMA alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            ACIKLAMA = 8,
            /// <summary>
            /// TASINIRNO alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            TASINIRNO = 9,
            /// <summary>
            /// ODEMEEMRINO alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            ODEMEEMRINO = 10,
            /// <summary>
            /// PERAMIRI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            PERAMIRI = 11,
            /// <summary>
            /// PERAMIRIUNVAN alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            PERAMIRIUNVAN = 12,
            /// <summary>
            /// GENELTOPLAM alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            GENELTOPLAM = 13,
            /// <summary>
            /// OIV alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            OIV = 14,
            /// <summary>
            /// KDVTEVKIFATI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            KDVTEVKIFATI = 15,
            /// <summary>
            /// DURUM alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            DURUM = 16,
            /// <summary>
            /// ISKONTO alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            ISKONTO = 17,
            /// <summary>
            /// ISKONTOORAN alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            ISKONTOORAN = 18,
            /// <summary>
            /// ODEMEEMRIYIL alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            ODEMEEMRIYIL = 19,
            PARABIRIMI = 20,
        }

        /// <summary>
        /// Fatura tablosu alanlarının isimlerini tutan sabit dizi
        /// </summary>
        public static string[] ALANLAR = { "YIL", "MUHASEBEKOD", "BIRIMKOD", "SURECNO", "PRFATURANO", "FIRMANO", "FATURANO", "FATURATARIH", "ACIKLAMA", "TASINIRNO", "ODEMEEMRINO", "PERAMIRI", "PERAMIRIUNVAN", "GENELTOPLAM", "OIV", "KDVTEVKIFATI", "DURUM", "ISKONTO", "ISKONTOORAN", "ODEMEEMRIYIL", "PARABIRIMI" };

        /// <summary>
        /// Fatura Detay tablo adını tutan sabit değişken
        /// </summary>
        static string TABLOADDETAY = "HRCFATURADETAY";

        /// <summary>
        /// Fatura Detay tablo alanlarının sırasını tutan enumeration
        /// </summary>
        enum ENUMAlanlarDetay
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
            /// PRFATURANO alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            PRFATURANO = 4,
            /// <summary>
            /// SIRANO alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            SIRANO = 5,
            /// <summary>
            /// ADI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            ADI = 6,
            /// <summary>
            /// MIKTAR alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            MIKTAR = 7,
            /// <summary>
            /// KDVORAN alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            KDVORAN = 8,
            /// <summary>
            /// BIRIMFIYAT alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            BIRIMFIYAT = 9,
            /// <summary>
            /// BELGESAHIBI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            BELGESAHIBI = 10,
            /// <summary>
            /// BELGETARIHI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            BELGETARIHI = 11,
            /// <summary>
            /// BELGENO alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            BELGENO = 12,
            /// <summary>
            /// ACIKLAMA alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            ACIKLAMA = 13,
            /// <summary>
            /// DETAYTIP alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            DETAYTIP = 14,
            /// <summary>
            /// SATIRTOPLAM alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            SATIRTOPLAM = 15,
            /// <summary>
            /// BELGESAHIBITCNO alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            BELGESAHIBITCNO = 16,
            /// <summary>
            /// KDVTUTARI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            KDVTUTARI = 17,
            /// <summary>
            /// MALZEMENO alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            MALZEMENO = 18,
            /// <summary>
            /// BELGEYIALAN alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            BELGEYIALAN = 19,
            /// <summary>
            /// BELGEYIALANTCNO alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            BELGEYIALANTCNO = 20,
            /// <summary>
            /// BIRIMFIYATIND alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            BIRIMFIYATIND = 21,
        }

        /// <summary>
        /// Fatura Detay tablosu alanlarının isimlerini tutan sabit dizi
        /// </summary>
        static string[] ALANLARDETAY = { "YIL", "MUHASEBEKOD", "BIRIMKOD", "SURECNO", "PRFATURANO", "SIRANO", "ADI", "MIKTAR", "KDVORAN", "BIRIMFIYAT", "BELGESAHIBI", "BELGETARIHI", "BELGENO", "ACIKLAMA", "DETAYTIP", "SATIRTOPLAM", "BELGESAHIBITCNO", "KDVTUTARI", "MALZEMENO", "BELGEYIALAN", "BELGEYIALANTCNO", "BIRIMFIYATIND" };

        /// <summary>
        /// Malzeme tablo adını tutan sabit değişken
        /// </summary>
        //static string TABLOADMALZEME = "HRCTEKLIFMALZEME";

        /// <summary>
        /// Malzeme tablo alanlarının sırasını tutan enumeration
        /// </summary>
        enum ENUMAlanlarMalzeme
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
            /// MALZEMENO alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            MALZEMENO = 4,
            /// <summary>
            /// MALZEMEKOD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            MALZEMEKOD = 5,
            /// <summary>
            /// OLCUBIRIMKOD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            OLCUBIRIMKOD = 6,
        }

        /// <summary>
        /// Malzeme tablosu alanlarının isimlerini tutan sabit dizi
        /// </summary>
        static string[] ALANLARMALZEME = { "YIL", "MUHASEBEKOD", "BIRIMKOD", "SURECNO", "MALZEMENO", "MALZEMEKOD", "OLCUBIRIMKOD" };

        /// <summary>
        /// Ölçü birim tablo adını tutan sabit değişken
        /// </summary>
        //static string TABLOADOLCUBIRIM = "HRCOLCUBIRIM";

        /// <summary>
        /// Ölçü birim tablo alanlarının sırasını tutan enumeration
        /// </summary>
        enum ENUMAlanlarOlcuBirim
        {
            /// <summary>
            /// KOD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            KOD = 0,
            /// <summary>
            /// AD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            AD = 1,
        }

        /// <summary>
        /// Ölçü birim tablosu alanlarının isimlerini tutan sabit dizi
        /// </summary>
        static string[] ALANLAROLCUBIRIM = { "KOD", "AD" };

        /// <summary>
        /// Süreç tablo adını tutan sabit değişken
        /// </summary>
        static string TABLOADSUREC = "HRCSUREC";

        /// <summary>
        /// Süreç tablo alanlarının sırasını tutan enumeration
        /// </summary>
        enum ENUMAlanlarSurec
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
            /// SURECDURUM alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            SURECDURUM = 4,
        }

        /// <summary>
        /// Fatura tablosu alanlarının isimlerini tutan sabit dizi
        /// </summary>
        static string[] ALANLARSUREC = { "YIL", "MUHASEBEKOD", "BIRIMKOD", "SURECNO", "SURECDURUM" };

        /// <summary>
        /// Malzeme tablo adını tutan sabit değişken
        /// </summary>
        //static string TABLOADRAPORFIRMA = "HRCFIRMAIHALE";

        /// <summary>
        /// Malzeme tablo alanlarının sırasını tutan enumeration
        /// </summary>
        enum ENUMAlanlarRaporFirma
        {
            YIL = 0,
            SURECNO = 1,
            KURKOD = 2,
            MUHASEBEKOD = 3,
            BIRIMKOD = 4,
            IHALEUSULU = 5,
            ADI = 6,
            GENELTOPLAM = 7,
            VERGINO = 8,
            AD = 9,
            ISLEMKOD = 10,
            FATURATARIH = 11,
        }

        /// <summary>
        /// Malzeme tablosu alanlarının isimlerini tutan sabit dizi
        /// </summary>
        static string[] ALANLARRAPORFIRMA = { "YIL", "SURECNO", "KURKOD", "MUHASEBEKOD", "BIRIMKOD", "IHALEUSULU", "ADI", "GENELTOPLAM", "VERGINO", "AD", "ISLEMKOD", "FATURATARIH" };



        /// <summary>
        /// Fatura bilgisini veri tabanına eklemek için 
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="bilgi">Eklenecek fatura bilgisini tutan nesne</param>
        /// <returns>Oluşturulan insert komutunun çalıştırılması sonucunu, Sonuc yapısında döndürür</returns>
        public Sonuc Kaydet(FaturaForm bilgi)
        {
            Sonuc sonuc = new Sonuc(true, "", "");
            string sql = "";

            sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
            sql += VeriErisim.DegerYap(bilgi.yil) + ",";
            sql += VeriErisim.DegerYap(bilgi.muhasebe) + ",";
            sql += VeriErisim.DegerYap(bilgi.birim) + ",";
            sql += VeriErisim.DegerYap(bilgi.surecNo) + ",";
            sql += VeriErisim.DegerYap(bilgi.prFaturaNo) + ",";
            sql += VeriErisim.DegerYap(bilgi.firmaNo) + ",";
            sql += VeriErisim.DegerYap(bilgi.faturaNo) + ",";
            sql += VeriErisim.DegerYap(bilgi.faturaTarih) + ",";
            sql += VeriErisim.DegerYap(bilgi.aciklama) + ",";
            sql += VeriErisim.DegerYap(bilgi.tasinirNo) + ",";
            sql += VeriErisim.DegerYap(bilgi.odemeEmriNo) + ",";
            sql += VeriErisim.DegerYap(bilgi.personelAmiri) + ",";
            sql += VeriErisim.DegerYap(bilgi.personelAmiriUnvan) + ",";
            sql += VeriErisim.DegerYap(bilgi.genelToplam) + ",";
            sql += VeriErisim.DegerYap(bilgi.oiv) + ",";
            sql += VeriErisim.DegerYap(bilgi.kdvTevkifati) + ",";
            sql += VeriErisim.DegerYap(bilgi.durum) + ",";
            sql += VeriErisim.DegerYap(bilgi.iskonto) + ",";
            sql += VeriErisim.DegerYap(bilgi.iskontoOran) + ",";
            sql += VeriErisim.DegerYap(bilgi.odemeEmriYil) + ",";
            sql += VeriErisim.DegerYap(bilgi.paraBirimi) + ")";

            sonuc = KomutCalistir(sql, 1);

            if (!sonuc.islemSonuc)
            {
                veriErisim.hata = "";
                sql = "UPDATE " + TABLOAD + " SET ";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.FIRMANO], bilgi.firmaNo) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.FATURANO], bilgi.faturaNo) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.FATURATARIH], bilgi.faturaTarih) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ACIKLAMA], bilgi.aciklama) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.PERAMIRI], bilgi.personelAmiri) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.PERAMIRIUNVAN], bilgi.personelAmiriUnvan) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.GENELTOPLAM], bilgi.genelToplam) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.OIV], bilgi.oiv) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.KDVTEVKIFATI], bilgi.kdvTevkifati) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.PARABIRIMI], bilgi.paraBirimi) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ISKONTO], bilgi.iskonto) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ISKONTOORAN], bilgi.iskontoOran) + " ";

                sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", bilgi.yil));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", bilgi.muhasebe));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.BIRIMKOD], "=", bilgi.birim));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SURECNO], "=", bilgi.surecNo));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.PRFATURANO], "=", bilgi.prFaturaNo));

                sonuc = KomutCalistir(sql, 1);
            }
            return sonuc;
        }

        /// <summary>
        /// Fatura detay bilgisini veri tabanına eklemek için 
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="bilgi">Eklenecek fatura bilgisini tutan nesne</param>
        /// <returns>Oluşturulan insert komutunun çalıştırılması sonucunu, Sonuc yapısında döndürür</returns>
        public Sonuc KaydetDetay(FaturaForm bilgi)
        {
            Sonuc sonuc = new Sonuc(false, "", "");
            string sql = "";
            int siraNo = bilgi.detaySiraNo;
            for (int i = 0; i < bilgi.detay.objeler.Count; i++)
            {
                if (siraNo == 0)
                    siraNo++;
                sql = "INSERT INTO " + TABLOADDETAY + " (" + VeriErisim.TumAlanlar("", ALANLARDETAY) + ") VALUES (";
                sql += VeriErisim.DegerYap(bilgi.yil) + ",";
                sql += VeriErisim.DegerYap(bilgi.muhasebe) + ",";
                sql += VeriErisim.DegerYap(bilgi.birim) + ",";
                sql += VeriErisim.DegerYap(bilgi.surecNo) + ",";
                sql += VeriErisim.DegerYap(bilgi.prFaturaNo) + ",";
                sql += VeriErisim.DegerYap(siraNo) + ",";
                sql += VeriErisim.DegerYap(((FaturaDetay)bilgi.detay.objeler[i]).adi) + ",";
                sql += VeriErisim.DegerYap(((FaturaDetay)bilgi.detay.objeler[i]).miktar) + ",";
                sql += VeriErisim.DegerYap(((FaturaDetay)bilgi.detay.objeler[i]).kdvOran) + ",";
                sql += VeriErisim.DegerYap(((FaturaDetay)bilgi.detay.objeler[i]).birimFiyat) + ",";
                sql += VeriErisim.DegerYap(((FaturaDetay)bilgi.detay.objeler[i]).belgeSahibi) + ",";
                sql += VeriErisim.DegerYap(((FaturaDetay)bilgi.detay.objeler[i]).belgeTarihi) + ",";
                sql += VeriErisim.DegerYap(((FaturaDetay)bilgi.detay.objeler[i]).belgeNo) + ",";
                sql += VeriErisim.DegerYap(((FaturaDetay)bilgi.detay.objeler[i]).aciklama) + ",";
                sql += VeriErisim.DegerYap(((FaturaDetay)bilgi.detay.objeler[i]).detayTip) + ",";
                sql += VeriErisim.DegerYap(((FaturaDetay)bilgi.detay.objeler[i]).satirToplam) + ",";
                sql += VeriErisim.DegerYap(((FaturaDetay)bilgi.detay.objeler[i]).belgeSahibiTCNo) + ",";
                sql += VeriErisim.DegerYap(((FaturaDetay)bilgi.detay.objeler[i]).kdvTutari) + ",";
                sql += VeriErisim.DegerYap(((FaturaDetay)bilgi.detay.objeler[i]).malzemeNo) + ",";
                sql += VeriErisim.DegerYap(((FaturaDetay)bilgi.detay.objeler[i]).belgeyiAlan) + ",";
                sql += VeriErisim.DegerYap(((FaturaDetay)bilgi.detay.objeler[i]).belgeyiAlanTCNo) + ",";
                sql += VeriErisim.DegerYap(((FaturaDetay)bilgi.detay.objeler[i]).birimFiyatIndirimli) + ")";
                if (bilgi.detayTip != (int)ENUMFaturaDetayTip.HARCAMAPUSULASI)
                    siraNo++;

                sonuc = KomutCalistir(sql, 1);
                if (!sonuc.islemSonuc)
                    break;
            }
            return sonuc;
        }

        /// <summary>
        /// Fatura bilgisini veri tabanından çeken sql cümleciğini verilen kıstasa göre
        /// üretip, çalıştırıp, datareader döndüren yordam.
        /// </summary>
        /// <param name="bilgi">Listelenecek fatura bilgisi için kıstas parametresi</param>
        /// <returns>
        /// Fatura bilgisini bulmak için oluşturulan select komutunun çalıştırılması sonucu elde edilen
        /// DataReader nesnesini döndürür
        /// </returns>
        public IDataReader Listele(FaturaForm bilgi)
        {
            string sql = "";
            string kosul = "";

            sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR);
            if (bilgi.detayTip == 0)
            {
                sql += ",HRCFIRMABILGISI.KOD, HRCFIRMABILGISI.MUHASEBEKOD, HRCFIRMABILGISI.BIRIMKOD, HRCFIRMABILGISI.AD";
                sql += ",HRCFIRMABILGISI.VERGINO, HRCFIRMABILGISI.VERGIDAIRESI, HRCFIRMABILGISI.BANKA, HRCFIRMABILGISI.HESAPNO";
                sql += ",HRCFIRMABILGISI.EMAIL, HRCFIRMABILGISI.FIRMAYETKILISI";
                sql += ",HRCFIRMABILGISI.TELEFON, HRCFIRMABILGISI.FAKS, HRCFIRMABILGISI.ADRES, HRCFIRMABILGISI.UYRUK, HRCFIRMABILGISI.GORULMESIN";
                sql += ",HRCFIRMABILGISI.YASAKLI, HRCFIRMABILGISI.CEPTEL, HRCFIRMABILGISI.WEB";
            }

            sql += " FROM " + TABLOAD;
            if (bilgi.detayTip == 0)
            {
                sql += " LEFT OUTER JOIN";
                sql += " HRCFIRMABILGISI ON HRCFATURAFORM.MUHASEBEKOD = HRCFIRMABILGISI.MUHASEBEKOD AND HRCFATURAFORM.FIRMANO = HRCFIRMABILGISI.KOD ";
            }

            if (bilgi.yil > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.YIL], "=", bilgi.yil));
            if (bilgi.muhasebe > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", bilgi.muhasebe));
            if (bilgi.birim != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.BIRIMKOD], "=", bilgi.birim));
            if (bilgi.surecNo != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.SURECNO], "=", bilgi.surecNo));
            if (bilgi.prFaturaNo > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.PRFATURANO], "=", bilgi.prFaturaNo));
            if (bilgi.firmaNo > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.FIRMANO], "=", bilgi.firmaNo));
            if (bilgi.prFaturaNo == 0 && bilgi.durum > -1)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.DURUM], "=", bilgi.durum));
            if (bilgi.prFaturaNoSorgu != "")
            {
                string[] fler = bilgi.prFaturaNoSorgu.Split(';');
                string icKosul = "";
                foreach (string fn in fler)
                {
                    if (fn.Trim() == "") continue;

                    icKosul = VeriErisim.KosulEkle(icKosul, "OR", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.PRFATURANO], "=", OrtakFonksiyonlar.ConvertToInt(fn, 0)));
                }

                if (icKosul != "")
                {
                    icKosul = "(" + icKosul + ")";
                    kosul = VeriErisim.KosulEkle(kosul, "AND", icKosul);
                }
            }

            if (kosul != "")
                sql += " WHERE " + kosul;

            sql += " ORDER BY " + ALANLAR[(int)ENUMAlanlar.FATURATARIH] + " DESC, " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD] + ", " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.BIRIMKOD];

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Fatura detay bilgisini veri tabanından çeken sql cümleciğini verilen kıstasa göre
        /// üretip, çalıştırıp, datareader döndüren yordam.
        /// </summary>
        /// <param name="bilgi">Listelenecek fatura bilgisi için kıstas parametresi</param>
        /// <returns>
        /// Fatura detay bilgisini bulmak için oluşturulan select komutunun çalıştırılması sonucu elde edilen
        /// DataReader nesnesini döndürür
        /// </returns>
        public IDataReader ListeleDetay(FaturaForm bilgi)
        {
            string kosulFatDetay = "";

            string sqlFatDetay = "SELECT " + VeriErisim.TumAlanlar(TABLOADDETAY, ALANLARDETAY) + " FROM HRCFATURADETAY";
            if (bilgi.yil > 0)
                kosulFatDetay = VeriErisim.KosulEkle(kosulFatDetay, "AND", VeriErisim.KosulOl(TABLOADDETAY, ALANLARDETAY[(int)ENUMAlanlarDetay.YIL], "=", bilgi.yil));
            if (bilgi.muhasebe > 0)
                kosulFatDetay = VeriErisim.KosulEkle(kosulFatDetay, "AND", VeriErisim.KosulOl(TABLOADDETAY, ALANLARDETAY[(int)ENUMAlanlarDetay.MUHASEBEKOD], "=", bilgi.muhasebe));
            if (bilgi.birim != "")
                kosulFatDetay = VeriErisim.KosulEkle(kosulFatDetay, "AND", VeriErisim.KosulOl(TABLOADDETAY, ALANLARDETAY[(int)ENUMAlanlarDetay.BIRIMKOD], "=", bilgi.birim));
            if (bilgi.surecNo != "")
                kosulFatDetay = VeriErisim.KosulEkle(kosulFatDetay, "AND", VeriErisim.KosulOl(TABLOADDETAY, ALANLARDETAY[(int)ENUMAlanlarDetay.SURECNO], "=", bilgi.surecNo));
            if (bilgi.prFaturaNo > 0)
                kosulFatDetay = VeriErisim.KosulEkle(kosulFatDetay, "AND", VeriErisim.KosulOl(TABLOADDETAY, ALANLARDETAY[(int)ENUMAlanlarDetay.PRFATURANO], "=", bilgi.prFaturaNo));
            if (bilgi.detayTip > 0)
                kosulFatDetay = VeriErisim.KosulEkle(kosulFatDetay, "AND", VeriErisim.KosulOl(TABLOADDETAY, ALANLARDETAY[(int)ENUMAlanlarDetay.DETAYTIP], "=", bilgi.detayTip));
            if (bilgi.detaySiraNo > 0)
                kosulFatDetay = VeriErisim.KosulEkle(kosulFatDetay, "AND", VeriErisim.KosulOl(TABLOADDETAY, ALANLARDETAY[(int)ENUMAlanlarDetay.SIRANO], "=", bilgi.detaySiraNo));
            if (kosulFatDetay != "")
                sqlFatDetay += " WHERE " + kosulFatDetay; // +" AND HRCFATURAFORM.DURUM = 1 ";

            string sqlTekMalzeme = "SELECT HRCTEKLIFMALZEME.MALZEMENO, HRCTEKLIFMALZEME.ADI, HRCTEKLIFMALZEME.MIKTAR, HRCTEKLIFMALZEME.OLCUBIRIMKOD, HRCOLCUBIRIM.AD, HRCTEKLIFMALZEME.MALZEMEKOD";
            sqlTekMalzeme += " FROM HRCOLCUBIRIM INNER JOIN HRCTEKLIFMALZEME ON HRCOLCUBIRIM.KOD = HRCTEKLIFMALZEME.OLCUBIRIMKOD";
            sqlTekMalzeme += " WHERE (HRCTEKLIFMALZEME.YIL = " + VeriErisim.DegerYap(bilgi.yil) + ") AND (HRCTEKLIFMALZEME.SURECNO = " + VeriErisim.DegerYap(bilgi.surecNo) + ") AND (HRCTEKLIFMALZEME.TIP = 3)";

            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOADDETAY, ALANLARDETAY) + "," +
                "SURECMALZEME." + ALANLARMALZEME[(int)ENUMAlanlarMalzeme.MALZEMEKOD] + "," +
                "SURECMALZEME." + ALANLARMALZEME[(int)ENUMAlanlarMalzeme.OLCUBIRIMKOD] + "," +
                "SURECMALZEME." + ALANLAROLCUBIRIM[(int)ENUMAlanlarOlcuBirim.AD] + ", SURECMALZEME.ADI, SURECMALZEME.MIKTAR, SURECMALZEME.MALZEMENO, SURECMALZEME.MALZEMEKOD";

            sql += " FROM (" + sqlFatDetay + ") AS HRCFATURADETAY FULL OUTER JOIN (" + sqlTekMalzeme + ") AS SURECMALZEME ON HRCFATURADETAY.MALZEMENO = SURECMALZEME.MALZEMENO";
            sql += " ORDER BY " + TABLOADDETAY + "." + ALANLARDETAY[(int)ENUMAlanlarDetay.SIRANO];
            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Fatura kontrol bilgisini veri tabanından çeken sql cümleciğini verilen kıstasa göre
        /// üretip, çalıştırıp, datareader döndüren yordam.
        /// </summary>
        /// <param name="bilgi">Listelenecek fatura bilgisi için kıstas parametresi</param>
        /// <returns>
        /// Fatura kontrol bilgisini bulmak için oluşturulan select komutunun çalıştırılması sonucu elde edilen
        /// DataReader nesnesini döndürür
        /// </returns>
        public IDataReader ListeleKontrol(FaturaForm bilgi)
        {
            string sql = "";
            string kosul = "";

            sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR);
            sql += " FROM " + TABLOAD + " LEFT OUTER JOIN " + TABLOADSUREC + " ON ";
            sql += TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.YIL] + "=" + TABLOADSUREC + "." + ALANLARSUREC[(int)ENUMAlanlarSurec.YIL] + " AND ";
            sql += TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD] + "=" + TABLOADSUREC + "." + ALANLARSUREC[(int)ENUMAlanlarSurec.MUHASEBEKOD] + " AND ";
            sql += TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.BIRIMKOD] + "=" + TABLOADSUREC + "." + ALANLARSUREC[(int)ENUMAlanlarSurec.BIRIMKOD] + " AND ";
            sql += TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.SURECNO] + "=" + TABLOADSUREC + "." + ALANLARSUREC[(int)ENUMAlanlarSurec.SURECNO];

            if (bilgi.yil > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.YIL], "=", bilgi.yil));
            if (bilgi.faturaNo != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.FATURANO], "=", bilgi.faturaNo.Trim()));
            if (!bilgi.faturaTarih.isNull)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.FATURATARIH], "=", bilgi.faturaTarih));
            if (bilgi.firmaNo > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.FIRMANO], "=", bilgi.firmaNo));
            if (bilgi.prFaturaNo > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.PRFATURANO], "<>", bilgi.prFaturaNo));
            if (bilgi.durum.ToString() != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.DURUM], "=", bilgi.durum));

            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADSUREC, ALANLARSUREC[(int)ENUMAlanlarSurec.SURECDURUM], "<>", 9));

            if (kosul != "")
                sql += " WHERE " + kosul;

            sql += " ORDER BY " + ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD] + ", " + ALANLAR[(int)ENUMAlanlar.BIRIMKOD] + "," + ALANLAR[(int)ENUMAlanlar.SURECNO];

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Fatura detay bilgisini veri tabanından silmek için 
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="bilgi">Silinecek fatura detay bilgisini tutan nesne</param>
        /// <returns>Oluşturulan delete komutunun çalıştırılması sonucunu, Sonuc yapısında döndürür</returns>
        public Sonuc SilDetay(FaturaForm bilgi)
        {
            string sql = "";

            sql = "DELETE FROM " + TABLOADDETAY;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARDETAY[(int)ENUMAlanlarDetay.YIL], "=", (bilgi.yil)));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARDETAY[(int)ENUMAlanlarDetay.MUHASEBEKOD], "=", (bilgi.muhasebe)));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARDETAY[(int)ENUMAlanlarDetay.BIRIMKOD], "=", (bilgi.birim)));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARDETAY[(int)ENUMAlanlarDetay.SURECNO], "=", (bilgi.surecNo)));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARDETAY[(int)ENUMAlanlarDetay.PRFATURANO], "=", (bilgi.prFaturaNo)));
            //faturadan harcama pusulası kontrolu yapılıyor
            if (bilgi.detaySiraNo > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARDETAY[(int)ENUMAlanlarDetay.SIRANO], "=", bilgi.detaySiraNo));

            return KomutCalistir(sql, 0);
        }

        /// <summary>
        /// Faturaya ait ödeme emri belge numarası bilgisini veri tabanında güncellemek için 
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="bilgi">Güncellenecek fatura bilgisini tutan nesne</param>
        /// <returns>Oluşturulan update komutunun çalıştırılması sonucunu, Sonuc yapısında döndürür</returns>
        public Sonuc OdemeEmriNoKaydet(FaturaForm bilgi)
        {
            string sql = "";

            sql = "UPDATE " + TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ODEMEEMRINO], bilgi.odemeEmriNo) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ODEMEEMRIYIL], bilgi.odemeEmriYil);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", bilgi.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SURECNO], "=", bilgi.surecNo));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.PRFATURANO], "=", bilgi.prFaturaNo));

            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Faturaya ait durum bilgisini veri tabanında güncellemek için 
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="bilgi">Güncellenecek fatura bilgisini tutan nesne</param>
        /// <returns>Oluşturulan update komutunun çalıştırılması sonucunu, Sonuc yapısında döndürür</returns>
        public Sonuc DurumDegistir(FaturaForm bilgi)
        {
            Sonuc sonuc = new Sonuc(false, "", "");
            string sql = "";

            sql = "UPDATE " + TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.DURUM], 0);

            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", bilgi.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", bilgi.muhasebe));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.BIRIMKOD], "=", bilgi.birim));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SURECNO], "=", bilgi.surecNo));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.PRFATURANO], "=", bilgi.prFaturaNo));

            sonuc = KomutCalistir(sql, 1);

            return sonuc;
        }

        /// <summary>
        /// Fatura harcama bilgisini veri tabanından çeken sql cümleciğini verilen kıstasa göre
        /// üretip, çalıştırıp, datareader döndüren yordam.
        /// </summary>
        /// <param name="yil">Listeleme için kullanılacak yıl kıstas parametresi</param>
        /// <param name="ay">Listeleme için kullanılacak ay kıstas parametresi</param>
        /// <param name="muhasebe">Listeleme için kullanılacak muhasebe kod kıstas parametresi</param>
        /// <param name="birim">Listeleme için kullanılacak birim kod kıstas parametresi</param>
        /// <param name="kur">Listeleme için kullanılacak kurumsal kod kıstas parametresi</param>
        /// <param name="fon">Listeleme için kullanılacak fonksiyonel kod kıstas parametresi</param>
        /// <param name="fin">Listeleme için kullanılacak finansman kod kıstas parametresi</param>
        /// <param name="eko">Listeleme için kullanılacak ekonomik kod kıstas parametresi</param>
        /// <param name="peb">Listeleme için kullanılacak alt faaliyet kod kıstas parametresi</param>
        /// <returns>
        /// Fatura harcama bilgisini bulmak için oluşturulan select komutunun çalıştırılması sonucu elde edilen
        /// DataReader nesnesini döndürür
        /// </returns>
        public IDataReader ListeleButceHazirlikFaturaHarcama(int yil, int ay, int muhasebe, string birim, string kur, string fon, string fin, string eko, string peb)
        {
            string sql = string.Empty;

            sql = "SELECT     YEAR(HRCFATURAFORM.FATURATARIH) AS ISLEMYIL, MONTH(HRCFATURAFORM.FATURATARIH) AS ISLEMAY, HRCSUREC.YIL AS SURECYIL,  " +
                      " HRCFATURAFORM.SURECNO, HRCFATURAFORM.MUHASEBEKOD, HRCFATURAFORM.BIRIMKOD, HRCIHALEONAYTERTIP.KURKOD,  " +
                      " HRCIHALEONAYTERTIP.FONKOD, HRCIHALEONAYTERTIP.FINKOD, HRCIHALEONAYTERTIP.EKOKOD, HRCIHALEONAYTERTIP.PEBKOD,  " +
                      " SUM(HRCFATURAFORM.GENELTOPLAM) AS TOPLAM, HRCSUREC.YILLARASARIISLEMMI " +
                    " FROM         HRCFATURAFORM LEFT OUTER JOIN " +
                      " HRCSUREC ON HRCFATURAFORM.YIL = HRCSUREC.YIL AND HRCFATURAFORM.SURECNO = HRCSUREC.SURECNO LEFT OUTER JOIN " +
                      " HRCIHALEONAYTERTIP ON HRCFATURAFORM.YIL = HRCIHALEONAYTERTIP.YIL AND HRCFATURAFORM.SURECNO = HRCIHALEONAYTERTIP.SURECNO " +
                    " WHERE     (HRCSUREC.SURECDURUM <> 9) AND (HRCFATURAFORM.DURUM = 1)";

            if (yil > 0)
                sql += " AND (YEAR(HRCFATURAFORM.FATURATARIH) = " + yil + ") ";
            if (ay > 0)
                sql += " AND (MONTH(HRCFATURAFORM.FATURATARIH) = " + ay + ") ";
            if (muhasebe > 0)
                sql += " AND (HRCFATURAFORM.MUHASEBEKOD = " + muhasebe + ") ";
            if (!string.IsNullOrEmpty(birim))
                sql += " AND (HRCFATURAFORM.BIRIMKOD = '" + birim + "') ";
            if (!string.IsNullOrEmpty(kur))
                sql += " AND (HRCIHALEONAYTERTIP.KURKOD = '" + kur + "') ";
            if (!string.IsNullOrEmpty(fon))
                sql += " AND (HRCIHALEONAYTERTIP.FONKOD = '" + fon + "') ";
            if (!string.IsNullOrEmpty(fin))
                sql += " AND (HRCIHALEONAYTERTIP.FINKOD = '" + fin + "') ";
            if (!string.IsNullOrEmpty(eko))
                sql += " AND (HRCIHALEONAYTERTIP.EKOKOD = '" + eko + "') ";
            if (!string.IsNullOrEmpty(peb))
                sql += " AND (HRCIHALEONAYTERTIP.PEBKOD = '" + peb + "') ";

            sql += " GROUP BY YEAR(HRCFATURAFORM.FATURATARIH), MONTH(HRCFATURAFORM.FATURATARIH), HRCFATURAFORM.SURECNO, HRCFATURAFORM.MUHASEBEKOD,  " +
                      " HRCFATURAFORM.BIRIMKOD, HRCIHALEONAYTERTIP.KURKOD, HRCIHALEONAYTERTIP.FONKOD, HRCIHALEONAYTERTIP.FINKOD,  " +
                      " HRCIHALEONAYTERTIP.EKOKOD, HRCIHALEONAYTERTIP.PEBKOD, HRCSUREC.YILLARASARIISLEMMI, HRCSUREC.YIL " +
                    " ORDER BY ISLEMYIL DESC, ISLEMAY DESC, HRCFATURAFORM.SURECNO, HRCFATURAFORM.MUHASEBEKOD, HRCFATURAFORM.BIRIMKOD,  " +
                      " HRCIHALEONAYTERTIP.KURKOD, HRCIHALEONAYTERTIP.FONKOD, HRCIHALEONAYTERTIP.FINKOD, HRCIHALEONAYTERTIP.EKOKOD, " +
                      " HRCIHALEONAYTERTIP.PEBKOD";

            return veriErisim.DataReaderDondur(sql);
        }

        public IDataReader ListeleRaporIhaleFirma(RaporIhaleFirma bilgi)
        {
            string sql = "";
            string kosul = "";

            sql = "SELECT     KURKOD, IHALEUSULU, SUM(GENELTOPLAM) AS GENELTOPLAM, VERGINO, AD ";
            sql += " FROM         HRCFIRMAIHALE ";



            if (bilgi.yil > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", "YEAR(" + ALANLARRAPORFIRMA[(int)ENUMAlanlarRaporFirma.FATURATARIH] + ")", "=", bilgi.yil));
            if (bilgi.ay > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", "MONTH(" + ALANLARRAPORFIRMA[(int)ENUMAlanlarRaporFirma.FATURATARIH] + ")", "<=", bilgi.ay));
            if (bilgi.muhasebe > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLARRAPORFIRMA[(int)ENUMAlanlarRaporFirma.MUHASEBEKOD], "=", bilgi.muhasebe));
            if (bilgi.birim != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLARRAPORFIRMA[(int)ENUMAlanlarRaporFirma.BIRIMKOD], "=", bilgi.birim));
            if (bilgi.kurKod != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLARRAPORFIRMA[(int)ENUMAlanlarRaporFirma.KURKOD], "=", bilgi.kurKod));
            if (bilgi.firmaVergiNo != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLARRAPORFIRMA[(int)ENUMAlanlarRaporFirma.VERGINO], "=", bilgi.firmaVergiNo));

            if (kosul != "")
                sql += " WHERE KURKOD IS NOT NULL AND " + kosul; // +" AND HRCFATURAFORM.DURUM = 1 ";

            sql += " GROUP BY KURKOD, IHALEUSULU, VERGINO, AD ";
            sql += "ORDER BY KURKOD, GENELTOPLAM DESC, IHALEUSULU ";

            return veriErisim.DataReaderDondur(sql);
        }

        public double ToplamAl(int surecYil, string surecNo)
        {
            string sql = "SELECT SUM(GENELTOPLAM) AS TOPLAM FROM HRCFATURAFORM";
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", "YIL", "=", surecYil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", "SURECNO", "=", surecNo));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", "DURUM", "=", 1));

            return this.veriErisim.TekKolonSQLDbl(sql);
        }

        public IDataReader FaturaListele(FaturaTum kriter)
        {
            string sql = "SELECT F.YIL, F.MUHASEBEKOD, F.BIRIMKOD, F.SURECNO, F.FATURANO, F.FATURATARIH, F.ACIKLAMA, FRM.AD, FRM.VERGINO, MIF.YIL, MIF.BELGENO, MIF.YEVMIYETARIH, MIF.YEVMIYENO, MIF.FORMDURUM, SUM(D.SATIRTOPLAM), MAX(F.GENELTOPLAM)";
            sql += " FROM HRCFATURAFORM AS F";
            sql += " INNER JOIN HRCFATURADETAY AS D ON F.YIL = D.YIL AND F.MUHASEBEKOD = D.MUHASEBEKOD AND F.BIRIMKOD = D.BIRIMKOD AND F.SURECNO = D.SURECNO AND F.PRFATURANO = D.PRFATURANO";
            sql += " INNER JOIN HRCFIRMABILGISI AS FRM ON F.MUHASEBEKOD = FRM.MUHASEBEKOD AND F.FIRMANO = FRM.KOD";
            sql += " INNER JOIN HRCSUREC ON HRCSUREC.YIL = F.YIL AND HRCSUREC.SURECNO = F.SURECNO";
            sql += " LEFT OUTER JOIN ButceIslemleri.dbo.MUHMIFFORM AS MIF ON MIF.SURECYIL= F.YIL AND MIF.SURECNO = F.SURECNO AND MIF.FORMDURUM<>9 AND (MIF.IKIMLIKNO=FRM.VERGINO) and F.FATURANO=MIF.ONO";// bir süreçte birden fazla fimnın faturası olabilir. (MIF.IKIMLIKNO=FRM.VERGINO) onun için eklendi
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("F", "YIL", "=", kriter.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("F", "DURUM", "=", 1));
            if (!string.IsNullOrWhiteSpace(kriter.firmaKodu))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("FRM", "VERGINO", "=", kriter.firmaKodu));
            if (kriter.muhasebe > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("F", "MUHASEBEKOD", "=", kriter.muhasebe));
            if (!string.IsNullOrWhiteSpace(kriter.birim))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("F", "BIRIMKOD", "=", kriter.birim));
            if (!string.IsNullOrWhiteSpace(kriter.faturaNo))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("F", "FATURANO", "LIKE", "%" + kriter.faturaNo + "%"));
            if (!string.IsNullOrWhiteSpace(kriter.aciklama))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("F", "ACIKLAMA", "LIKE", "%" + kriter.faturaNo + "%"));
            if (!string.IsNullOrWhiteSpace(kriter.firmaAdi))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("FRM", "AD", "LIKE", kriter.firmaAdi + "%"));
            if (kriter.mifDurum == -100)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("HRCSUREC", "SURECDURUM", "<", 9));

            sql += " group by F.YIL, F.MUHASEBEKOD, F.BIRIMKOD, F.SURECNO, F.FATURANO, F.FATURATARIH, F.ACIKLAMA, FRM.AD, FRM.VERGINO, MIF.YIL, MIF.BELGENO, MIF.YEVMIYETARIH, MIF.YEVMIYENO, MIF.FORMDURUM";

            return this.veriErisim.DataReaderDondur(sql);
        }

        public IDataReader FaturaListeleServis(FaturaForm kriter)
        {
            string kosul = "";

            string sql = "SELECT HRCFATURADETAY.YIL, HRCTEKLIFMALZEME.SURECNO, HRCFATURAFORM.FIRMANO, HRCFIRMABILGISI.AD, HRCFATURAFORM.FATURATARIH, HRCFATURADETAY.BIRIMFIYAT, HRCTEKLIFMALZEME.MALZEMEKOD, HRCTEKLIFMALZEME.ADI";
            sql += " FROM HRCFATURADETAY";
            sql += " INNER JOIN HRCTEKLIFMALZEME ON HRCFATURADETAY.YIL = HRCTEKLIFMALZEME.YIL";
            sql += " AND HRCFATURADETAY.MUHASEBEKOD = HRCTEKLIFMALZEME.MUHASEBEKOD AND HRCFATURADETAY.BIRIMKOD = HRCTEKLIFMALZEME.BIRIMKOD";
            sql += " AND HRCFATURADETAY.SURECNO = HRCTEKLIFMALZEME.SURECNO AND HRCFATURADETAY.MALZEMENO = HRCTEKLIFMALZEME.MALZEMENO";
            sql += " AND HRCTEKLIFMALZEME.TIP = 3";
            sql += " INNER JOIN HRCFATURAFORM ON HRCFATURADETAY.YIL = HRCFATURAFORM.YIL";
            sql += " AND HRCFATURADETAY.MUHASEBEKOD = HRCFATURAFORM.MUHASEBEKOD AND HRCFATURADETAY.BIRIMKOD = HRCFATURAFORM.BIRIMKOD";
            sql += " AND HRCFATURADETAY.SURECNO = HRCFATURAFORM.SURECNO";
            sql += " INNER JOIN HRCSUREC ON HRCFATURAFORM.YIL = HRCSUREC.YIL";
            sql += " AND HRCFATURAFORM.SURECNO = HRCSUREC.SURECNO AND HRCFATURAFORM.MUHASEBEKOD = HRCSUREC.MUHASEBEKOD";
            sql += " AND HRCFATURAFORM.BIRIMKOD = HRCSUREC.BIRIMKOD";
            sql += " INNER JOIN HRCFIRMABILGISI ON HRCFATURAFORM.FIRMANO = HRCFIRMABILGISI.KOD";

            if (!string.IsNullOrEmpty(kriter.tMalzeme.malzemeKod))
            {
                kosul = VeriErisim.KosulEkle(kosul, "AND", "(REPLACE(HRCTEKLIFMALZEME.MALZEMEKOD, '.', '') IN (" + kriter.tMalzeme.malzemeKod + "))");
                kosul = VeriErisim.KosulEkle(kosul, "AND", "HRCFATURADETAY.BIRIMFIYAT>0");
            }

            kosul = VeriErisim.KosulEkle(kosul, "AND", "HRCFATURAFORM.FATURATARIH = (SELECT MAX( D.FATURATARIH) FROM HRCFATURADETAY FD" +
                                                  " INNER JOIN HRCTEKLIFMALZEME TM ON FD.YIL = TM.YIL" +
                                                  " AND FD.MUHASEBEKOD = TM.MUHASEBEKOD AND FD.BIRIMKOD = TM.BIRIMKOD" +
                                                  " AND FD.SURECNO = TM.SURECNO AND FD.MALZEMENO = TM.MALZEMENO" +
                                                  " AND TM.TIP = 3" +
                                                  " INNER JOIN HRCFATURAFORM D ON D.PRFATURANO = FD.PRFATURANO" +
                                                  " WHERE TM.MALZEMEKOD = HRCTEKLIFMALZEME.MALZEMEKOD)");

            if (!string.IsNullOrEmpty(kosul))
                sql += " WHERE " + kosul;

            return veriErisim.DataReaderDondur(sql);
        }
    }
}
