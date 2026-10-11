using System;
using System.Data;
using TNS.Veri;

namespace TNS.HRC.IsKurallari.Veri
{
    /// <summary>
    /// Telekom fatura bilgisini tutan HRCABONEFATURATELEKOM veri tabanı tablosu ile ilgili, listeleme ve yazma işlemleri tutan nesne
    /// </summary>
    public class TABAboneFaturaTelekom : TABOrtak
    {
        /// <summary>
        /// Sınıfın oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABAboneFaturaTelekom(VeriErisim veriErisim) : base(veriErisim)
        {
        }

        /// <summary>
        /// Abone Fatura Telekom tablo adını tutan sabit değişken
        /// </summary>
        static string TABLOAD = "HRCABONEFATURATELEKOM";

        /// <summary>
        /// Tablo alanlarının sırasını tutan enumeration
        /// </summary>
        enum ENUMAlanlar : int
        {
            /// <summary>
            /// FATURAYIL alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            FATURAYIL = 0,
            /// <summary>
            /// FATURAAY alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            FATURAAY = 1,
            /// <summary>
            /// HIZMETNO alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            HIZMETNO = 2,
            /// <summary>
            /// FATURANO alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            FATURANO = 3,
            /// <summary>
            /// SIRANO alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            SIRANO = 4,
            /// <summary>
            /// TCKIMLIKNO alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            TCKIMLIKNO = 5,
            /// <summary>
            /// VKN alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            VKN = 6,
            /// <summary>
            /// ADSOYADUNVAN alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            ADSOYADUNVAN = 7,
            /// <summary>
            /// ADRES alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            ADRES = 8,
            /// <summary>
            /// DOCNO alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            DOCNO = 9,
            /// <summary>
            /// TARIH alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            TARIH = 10,
            /// <summary>
            /// SONODEMETARIH alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            SONODEMETARIH = 11,
            /// <summary>
            /// ACIKLAMA alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            ACIKLAMA = 12,
            /// <summary>
            /// PARABIRIMKOD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            PARABIRIMKOD = 13,
            /// <summary>
            /// MALHIZTOPTUTAR alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            MALHIZTOPTUTAR = 14,
            /// <summary>
            /// TOPISKONTO alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            TOPISKONTO = 15,
            /// <summary>
            /// KDVORAN alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            KDVORAN = 16,
            /// <summary>
            /// KDVTUTAR alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            KDVTUTAR = 17,
            /// <summary>
            /// KDVTOPTUTAR alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            KDVTOPTUTAR = 18,
            /// <summary>
            /// DIGVERFONKOD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            DIGVERFONKOD = 19,
            /// <summary>
            /// DIGVERFONORAN alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            DIGVERFONORAN = 20,
            /// <summary>
            /// DIGVERFONTUTAR alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            DIGVERFONTUTAR = 21,
            /// <summary>
            /// DVFTOPTUTAR alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            DVFTOPTUTAR = 22,
            /// <summary>
            /// FATTOPTUTAR alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            FATTOPTUTAR = 23,
            /// <summary>
            /// ODNTUTAR alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            ODNTUTAR = 24,
            /// <summary>
            /// AYLIKUCRET alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            AYLIKUCRET = 25,
            /// <summary>
            /// GORUSME alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            GORUSME = 26,
            /// <summary>
            /// KAYITLI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            KAYITLI = 27,
            /// <summary>
            /// AYRINTI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            AYRINTI = 28,
            /// <summary>
            /// FONOTEL alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            FONOTEL = 29,
            /// <summary>
            /// GECIKME alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            GECIKME = 30,
            /// <summary>
            /// KAPATILMA alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            KAPATILMA = 31,
            /// <summary>
            /// BAGLANTI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            BAGLANTI = 32,
            /// <summary>
            /// FATURAEDILMEMIS alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            FATURAEDILMEMIS = 33,
            /// <summary>
            /// CESITLI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            CESITLI = 34,
            /// <summary>
            /// DAMGA alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            DAMGA = 35,
            /// <summary>
            /// SEHIRICI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            SEHIRICI = 36,
            /// <summary>
            /// SEHIRDISI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            SEHIRDISI = 37,
            /// <summary>
            /// MILLETLERARASI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            MILLETLERARASI = 38,
            /// <summary>
            /// UCRETLISERVISLER alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            UCRETLISERVISLER = 39,
            /// <summary>
            /// INTERNET alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            INTERNET = 40,
            /// <summary>
            /// GSM alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            GSM = 41,
            /// <summary>
            /// SESLIMESAJAYLIKUCRET alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            SESLIMESAJAYLIKUCRET = 42,
            /// <summary>
            /// SMSUCRET alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            SMSUCRET = 43,
            /// <summary>
            /// SESLIMESAJUCRETI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            SESLIMESAJUCRETI = 44,
            /// <summary>
            /// AILEKARTUCRETI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            AILEKARTUCRETI = 45,
            /// <summary>
            /// CIHAZUCRETI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            CIHAZUCRETI = 46,
        }

        /// <summary>
        /// Abone Fatura Telekom tablosu alanlarının isimlerini tutan sabit dizi
        /// </summary>
        static string[] ALANLAR = { "FATURAYIL", "FATURAAY", "HIZMETNO", "FATURANO", "SIRANO", "TCKIMLIKNO", "VKN", "ADSOYADUNVAN", "ADRES", "DOCNO", "TARIH", "SONODEMETARIH", "ACIKLAMA", "PARABIRIMKOD", "MALHIZTOPTUTAR", "TOPISKONTO", "KDVORAN", "KDVTUTAR", "KDVTOPTUTAR", "DIGVERFONKOD", "DIGVERFONORAN", "DIGVERFONTUTAR", "DVFTOPTUTAR", "FATTOPTUTAR", "ODNTUTAR", "AYLIKUCRET", "GORUSME", "KAYITLI", "AYRINTI", "FONOTEL", "GECIKME", "KAPATILMA", "BAGLANTI", "FATURAEDILMEMIS", "CESITLI", "DAMGA", "SEHIRICI", "SEHIRDISI", "MILLETLERARASI", "UCRETLISERVISLER", "INTERNET", "GSM", "SESLIMESAJAYLIKUCRET", "SMSUCRET", "SESLIMESAJUCRETI", "AILEKARTUCRETI", "CIHAZUCRETI" };

        /// <summary>
        /// Abone telekom fatura bilgisini veri tabanına eklemek için 
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="fatura">Eklenecek abone telekom fatura bilgisini tutan nesne</param>
        /// <returns>Oluşturulan insert komutunun çalıştırılması sonucunu, Sonuc yapısında döndürür</returns>
        public Sonuc Kaydet(AboneFaturaTelefon fatura)
        {
            string sql = "";
            Sonuc sonuc = new Sonuc(false, "Beklenmeyen bir hata oluştu", "x");

            sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";

            sql += VeriErisim.DegerYap(fatura.faturaYil) + ",";
            sql += VeriErisim.DegerYap(fatura.faturaAy) + ",";
            sql += VeriErisim.DegerYap(fatura.hizmetNo.Trim()) + ",";
            sql += VeriErisim.DegerYap(fatura.faturaNo) + ",";
            sql += VeriErisim.DegerYap(fatura.siraNo) + ",";
            sql += VeriErisim.DegerYap(fatura.tcKimlikNo) + ",";
            sql += VeriErisim.DegerYap(fatura.vkn) + ",";
            sql += VeriErisim.DegerYap(fatura.adSoyadUnvan) + ",";
            sql += VeriErisim.DegerYap(fatura.adres) + ",";
            sql += VeriErisim.DegerYap(fatura.docNo) + ",";
            sql += VeriErisim.DegerYap(fatura.tarih) + ",";
            sql += VeriErisim.DegerYap(fatura.sonOdemeTarih) + ",";
            sql += VeriErisim.DegerYap(fatura.aciklama) + ",";
            sql += VeriErisim.DegerYap(fatura.paraBirimKod) + ",";
            sql += VeriErisim.DegerYap(fatura.malHizTopTutar) + ",";
            sql += VeriErisim.DegerYap(fatura.topIskonto) + ",";
            sql += VeriErisim.DegerYap(fatura.kdvOran) + ",";
            sql += VeriErisim.DegerYap(fatura.kdvTutar) + ",";
            sql += VeriErisim.DegerYap(fatura.kdvTopTutar) + ",";
            sql += VeriErisim.DegerYap(fatura.digVerFonKod) + ",";
            sql += VeriErisim.DegerYap(fatura.digVerFonOran) + ",";
            sql += VeriErisim.DegerYap(fatura.digVerFonTutar) + ",";
            sql += VeriErisim.DegerYap(fatura.dvfTopTutar) + ",";
            sql += VeriErisim.DegerYap(fatura.fatTopTutar) + ",";
            sql += VeriErisim.DegerYap(fatura.odnTutar) + ",";
            sql += VeriErisim.DegerYap(fatura.aylikUcret) + ",";
            sql += VeriErisim.DegerYap(fatura.gorusme) + ",";
            sql += VeriErisim.DegerYap(fatura.kayitli) + ",";
            sql += VeriErisim.DegerYap(fatura.ayrinti) + ",";
            sql += VeriErisim.DegerYap(fatura.fonotel) + ",";
            sql += VeriErisim.DegerYap(fatura.gecikme) + ",";
            sql += VeriErisim.DegerYap(fatura.kapatilma) + ",";
            sql += VeriErisim.DegerYap(fatura.baglanti) + ",";
            sql += VeriErisim.DegerYap(fatura.faturaEdilmemis) + ",";
            sql += VeriErisim.DegerYap(fatura.cesitli) + ",";
            sql += VeriErisim.DegerYap(fatura.damga) + ",";
            sql += VeriErisim.DegerYap(fatura.sehirIci) + ",";
            sql += VeriErisim.DegerYap(fatura.sehirDisi) + ",";
            sql += VeriErisim.DegerYap(fatura.milletlerArasi) + ",";
            sql += VeriErisim.DegerYap(fatura.ucretliServisler) + ",";
            sql += VeriErisim.DegerYap(fatura.internet) + ",";
            sql += VeriErisim.DegerYap(fatura.gsm) + ",";
            sql += VeriErisim.DegerYap(fatura.sesliMesajAylikUcret) + ",";
            sql += VeriErisim.DegerYap(fatura.smsUcret) + ",";
            sql += VeriErisim.DegerYap(fatura.sesliMesajUcreti) + ",";
            sql += VeriErisim.DegerYap(fatura.aileKartUcreti) + ",";
            sql += VeriErisim.DegerYap(fatura.cihazUcreti) + ")";

            sonuc = KomutCalistir(sql, 1);

            if (!sonuc.islemSonuc) //Kayıt var, güncelle
            {
                sql = "UPDATE " + TABLOAD + " SET ";

                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.FATURANO], fatura.faturaNo) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.SIRANO], fatura.siraNo) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.TCKIMLIKNO], fatura.tcKimlikNo) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.VKN], fatura.vkn) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ADSOYADUNVAN], fatura.adSoyadUnvan) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ADRES], fatura.adres) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.DOCNO], fatura.docNo) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.TARIH], fatura.tarih) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.SONODEMETARIH], fatura.sonOdemeTarih) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ACIKLAMA], fatura.aciklama) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.PARABIRIMKOD], fatura.paraBirimKod) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.MALHIZTOPTUTAR], fatura.malHizTopTutar) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.TOPISKONTO], fatura.topIskonto) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.KDVORAN], fatura.kdvOran) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.KDVTUTAR], fatura.kdvTutar) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.KDVTOPTUTAR], fatura.kdvTopTutar) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.DIGVERFONKOD], fatura.digVerFonKod) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.DIGVERFONORAN], fatura.digVerFonOran) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.DIGVERFONTUTAR], fatura.digVerFonTutar) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.DVFTOPTUTAR], fatura.dvfTopTutar) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.FATTOPTUTAR], fatura.fatTopTutar) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ODNTUTAR], fatura.odnTutar) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.AYLIKUCRET], fatura.aylikUcret) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.GORUSME], fatura.gorusme) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.KAYITLI], fatura.kayitli) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.AYRINTI], fatura.ayrinti) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.FONOTEL], fatura.fonotel) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.GECIKME], fatura.gecikme) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.KAPATILMA], fatura.kapatilma) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.BAGLANTI], fatura.baglanti) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.FATURAEDILMEMIS], fatura.faturaEdilmemis) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.CESITLI], fatura.cesitli) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.DAMGA], fatura.damga) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.SEHIRICI], fatura.sehirIci) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.SEHIRDISI], fatura.sehirDisi) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.MILLETLERARASI], fatura.milletlerArasi) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.UCRETLISERVISLER], fatura.ucretliServisler) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.INTERNET], fatura.internet) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.GSM], fatura.gsm) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.SESLIMESAJAYLIKUCRET], fatura.sesliMesajAylikUcret) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.SMSUCRET], fatura.smsUcret) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.SESLIMESAJUCRETI], fatura.sesliMesajUcreti) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.AILEKARTUCRETI], fatura.aileKartUcreti) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.CIHAZUCRETI], fatura.cihazUcreti) + " ";

                sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.FATURAYIL], "=", fatura.faturaYil));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.FATURAAY], "=", fatura.faturaAy));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.HIZMETNO], "=", fatura.hizmetNo));

                sonuc = KomutCalistir(sql, 1);
            }
            return sonuc;
        }

        /// <summary>
        /// Abone telekom fatura bilgisini veri tabanından çeken sql cümleciğini verilen abone telekom fatura kıstasına göre
        /// üretip, çalıştırıp, datareader döndüren yordam.
        /// </summary>
        /// <param name="bilgi">Listelenecek abone telekom fatura için kıstas parametresi</param>
        /// <param name="sadeceYeniler">Eğer <c>true</c> verilirse süreç oluşturulmamış faturaları listeler</param>
        /// <returns>
        /// Abone telekom fatura bilgisini bulmak için oluşturulan select komutunun çalıştırılması sonucu elde edilen
        /// DataReader nesnesini döndürür
        /// </returns>
        public IDataReader Listele(AboneFaturaTelefon bilgi, bool sadeceYeniler)
        {
            string sql = "";
            sql = "SELECT " + VeriErisim.TumAlanlar("A", ALANLAR) +
                "," + TABAboneNo.TABLOAD + "." + TABAboneNo.ALANLAR[(int)TABAboneNo.ENUMAlanlar.MUHASEBEKOD] +
                "," + TABAboneNo.TABLOAD + "." + TABAboneNo.ALANLAR[(int)TABAboneNo.ENUMAlanlar.BIRIMKOD] +
                "," + TABIdareBilgi.TABLOAD + "." + TABIdareBilgi.ALANLAR[(int)TABIdareBilgi.ENUMAlanlar.EPOSTA];
            sql += " FROM HRCIDAREBILGI RIGHT OUTER JOIN";
            sql += " HRCABONENO ON HRCIDAREBILGI.MUHASEBEKOD = HRCABONENO.MUHASEBEKOD AND ";
            sql += " HRCIDAREBILGI.BIRIMKOD = HRCABONENO.BIRIMKOD LEFT OUTER JOIN";
            sql += " HRCABONEFATURATELEKOM A ON HRCABONENO.ABONENO = A.HIZMETNO";

            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.FATURAYIL], "=", bilgi.faturaYil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.FATURAAY], "=", bilgi.faturaAy));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABAboneNo.TABLOAD, TABAboneNo.ALANLAR[(int)TABAboneNo.ENUMAlanlar.ABONETIP], "=", (int)ENUMAboneTip.TELEFON));
            if (bilgi.hizmetNo != "")
            {
                string kosul = "";
                string[] h = bilgi.hizmetNo.Split(';');
                for (int i = 0; i < h.Length; i++)
                    kosul = VeriErisim.KosulEkle(kosul, "OR", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.HIZMETNO], "=", h[i]));

                if (kosul != "")
                    sql += " AND (" + kosul + ")";
            }
            if (sadeceYeniler)//Süreç oluşturulmamışlar listelemek için
                sql += " AND A.HIZMETNO NOT IN (SELECT ABONENO FROM HRCABONEFATURA B,HRCSUREC WHERE HRCSUREC.YIL=B.YIL AND HRCSUREC.SURECNO=B.SURECNO AND A.FATURAYIL = B.FATURAYIL AND A.FATURAAY = B.FATURAAY AND A.HIZMETNO = B.ABONENO)";//AND HRCSUREC.SURECDURUM<9 //daha önce kayıt ettiğimiz fakat kullanıcının ödeme yapmış olmasından dolayı iptal ettiği süreçleride dikkate almalıyız

            sql += " ORDER BY " + TABAboneNo.TABLOAD + "." + TABAboneNo.ALANLAR[(int)TABAboneNo.ENUMAlanlar.MUHASEBEKOD] + "," + TABAboneNo.TABLOAD + "." + TABAboneNo.ALANLAR[(int)TABAboneNo.ENUMAlanlar.BIRIMKOD] + ",A.SIRANO";

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Telekom faturası abonelerine ait mail bilgisini veri tabanından çeken sql cümleciğini verilen abone telekom fatura kıstasına göre 
        /// üretip, çalıştırıp, datareader döndüren yordam.
        /// </summary>
        /// <param name="bilgi">Listelemede kıstas olarak kullanılacak nesne</param>
        /// <returns>
        /// Telekom faturası abonelerine ait mail bilgisini bulmak için oluşturulan select 
        /// komutunun çalıştırılması sonucu elde edilen DataReader nesnesini döndürür
        /// </returns>
        public IDataReader MailIcinListe(AboneFaturaTelefon bilgi)
        {
            string sql = "";
            sql = "SELECT HRCABONENO.MUHASEBEKOD, HRCABONENO.BIRIMKOD, HRCIDAREBILGI.EPOSTA, HRCABONEFATURA.ODEMEEMRINO, HRCABONEFATURA.SURECNO, A.HIZMETNO, A.SONODEMETARIH, A.ODNTUTAR";
            sql += " FROM HRCABONEFATURA INNER JOIN";
            sql += " HRCABONENO ON HRCABONEFATURA.MUHASEBEKOD = HRCABONENO.MUHASEBEKOD AND ";
            sql += " HRCABONEFATURA.BIRIMKOD = HRCABONENO.BIRIMKOD LEFT OUTER JOIN";
            sql += " HRCABONEFATURATELEKOM A ON HRCABONEFATURA.ABONENO = A.HIZMETNO AND HRCABONEFATURA.FATURAAY = A.FATURAAY AND ";
            sql += " HRCABONEFATURA.FATURAYIL = A.FATURAYIL AND HRCABONENO.ABONENO = A.HIZMETNO LEFT OUTER JOIN";
            sql += " HRCIDAREBILGI ON HRCABONENO.MUHASEBEKOD = HRCIDAREBILGI.MUHASEBEKOD AND ";
            sql += " HRCABONENO.BIRIMKOD = HRCIDAREBILGI.BIRIMKOD";
            sql += " WHERE A.FATURAYIL = " + bilgi.faturaYil;
            sql += " AND A.FATURAAY = " + bilgi.faturaAy;
            sql += " AND HRCABONENO.ABONETIP =" + (int)ENUMAboneTip.TELEFON;
            sql += " AND EPOSTA IS NOT NULL AND EPOSTA<>''";
            sql += " ORDER BY HRCABONENO.MUHASEBEKOD, HRCABONENO.BIRIMKOD";

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Abone telekom fatura istatistik bilgisini veri tabanından çeken sql cümleciğini verilen kıstaslara göre 
        /// üretip, çalıştırıp, datareader döndüren yordam.
        /// </summary>
        /// <param name="tip">Listelemede kıstas olarak kullanılacak tip bilgisi</param>
        /// <param name="yil">Listelemede kıstas olarak kullanılacak yıl bilgisi</param>
        /// <param name="muhasebe">Listelemede kıstas olarak kullanılacak muhasebe kod bilgisi.</param>
        /// <param name="birim">Listelemede kıstas olarak kullanılacak birim kod bilgis.</param>
        /// <returns>
        /// Abone telekom fatura istatistik bilgisini bulmak için oluşturulan select komutunun 
        /// çalıştırılması sonucu elde edilen DataReader nesnesini döndürür
        /// </returns>
        public IDataReader ListeleTelekomIstatistik(ENUMAboneTip tip, int yil, int muhasebe, string birim)
        {
            string sql = "";
            sql = "SELECT     HRCABONEFATURA.FATURAYIL, HRCABONEFATURA.FATURAAY, HRCABONEFATURA.MUHASEBEKOD, HRCABONEFATURA.BIRIMKOD, " +
                      " SUM(HRCABONEFATURA.FATURATUTAR) AS TOPLAM, COUNT(HRCABONEFATURA.ABONENO) AS SAYI " +
                    " FROM         HRCABONEFATURA LEFT OUTER JOIN " +
                      " HRCSUREC ON HRCABONEFATURA.YIL = HRCSUREC.YIL AND HRCABONEFATURA.SURECNO = HRCSUREC.SURECNO " +
                    " WHERE     (HRCABONEFATURA.ABONETIP = " + (int)tip + ") AND (HRCABONEFATURA.FATURAYIL = " + yil + ") AND (HRCABONEFATURA.ODEMEEMRINO <> '') AND " +
                      " (HRCABONEFATURA.MUHASEBEKOD = " + muhasebe + ") AND (HRCABONEFATURA.BIRIMKOD = '" + birim + "') " +
                    " GROUP BY HRCABONEFATURA.FATURAYIL, HRCABONEFATURA.FATURAAY, HRCABONEFATURA.MUHASEBEKOD, HRCABONEFATURA.BIRIMKOD " +
                    " ORDER BY HRCABONEFATURA.FATURAYIL, HRCABONEFATURA.MUHASEBEKOD, HRCABONEFATURA.BIRIMKOD, HRCABONEFATURA.FATURAAY";

            return veriErisim.DataReaderDondur(sql);
        }
    }
}
