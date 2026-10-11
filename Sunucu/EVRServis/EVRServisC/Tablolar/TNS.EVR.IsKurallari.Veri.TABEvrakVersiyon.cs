using System;
using System.Data;
using TNS.Veri;

namespace TNS.EVR.IsKurallari.Veri
{

    public class TABEvrakVersiyon : TABOrtak
    {
        /// <summary>        
        /// Parametre olarak verilen veriErisim nesnesine göre, TABEvrakVersiyon sınıfının yeni bir örneğini oluşturur.
        /// </summary>
        /// <param name="veriErisim">Veri erişim bilgisini içeren nesne</param>
        public TABEvrakVersiyon(VeriErisim veriErisim) : base(veriErisim) { }

        public static string TABLOAD = "EVREVRAKVERSIYON";

        public enum ENUMAlanlar : int
        {
            EVREVRAKNO = 0,
            YIL = 1,
            EVRAKTIPI = 2,
            EVRAKBIRIMKODU = 3,
            EVRAKNO = 4,
            EVRAKTARIH = 5,
            GELDIGIYERTUR = 6,
            GELDIGIYER = 7,
            GELENEVRAKNO = 8,
            GELENEVRAKTARIH = 9,
            KONU = 10,
            GIZLILIK = 11,
            GELISSEKLI = 12,
            YAZIDURUM = 13,
            CEVAPSURESI = 14,
            ACIKLAMA = 15,
            EKADEDI = 16,
            HAVALEBIRIMKODU = 17,
            EVRAKTURU = 18,
            PARAF = 19,
            ANTET = 20,
            ICERIK = 21,
            IMZA1 = 22,
            IMZA2 = 23,
            IMZA3 = 24,
            EK = 25,
            ALTBILGI = 26,
            OLUSTURMATARIHI = 27,
            IMZATARIHI = 28,
            EKYAZIALAN = 29,
            SIRANO = 30,
            DOSYANO = 31,
            EVRAKICERIK = 32,
            EVRAKCINSI = 33,
            ESASNO = 34,
            EVRAKBELGETUR = 35,
            IMZAYOLU = 36,

            KOD = 37,
            VERSIYONAD = 38,
            VERSIYONMU = 39,
            ISLEMYAPAN = 40,
            ISLEMYAPANAD = 41,
            ISLEMTARIH = 42,

        }

        /// <summary>
        /// Evrak bilgilerini içeren veri tabanı tablosunun alanlarının isimlerini tutan dizi.
        /// </summary>
        public static string[] ALANLAR = {  "EVREVRAKNO", "YIL", "EVRAKTIPI", "EVRAKBIRIMKODU", "EVRAKNO", "EVRAKTARIH", "GELDIGIYERTUR", "GELDIGIYER", "GELENEVRAKNO", "GELENEVRAKTARIH", "KONU", "GIZLILIK", "GELISSEKLI", "YAZIDURUM", "CEVAPSURESI", "ACIKLAMA", "EKADEDI", "HAVALEBIRIMKODU"
                                    ,"EVRAKTURU","PARAF","ANTET","ICERIK","IMZA1","IMZA2","IMZA3","EK","ALTBILGI"
                                    ,"OLUSTURMATARIHI","IMZATARIHI","EKYAZIALAN","SIRANO", "DOSYANO", "EVRAKICERIK", "EVRAKCINSI","ESASNO","EVRAKBELGETUR","IMZAYOLU"
                                    ,"KOD","VERSIYONAD","VERSIYONMU","ISLEMYAPAN","ISLEMYAPANAD","ISLEMTARIH"};


        public Sonuc Kaydet(EvrakVersiyon form)
        {
            form.kod = this.SonVersiyonKod(form.evrEvrakNo) + 1;
            string sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
            sql += VeriErisim.DegerYap(form.evrEvrakNo) + ",";
            sql += VeriErisim.DegerYap(form.yil) + ",";
            sql += VeriErisim.DegerYap(form.evrakTipi) + ",";
            sql += VeriErisim.DegerYap(form.evrakBirimKodu) + ",";
            sql += VeriErisim.DegerYap(form.evrakNo) + ",";
            sql += VeriErisim.DegerYap(form.evrakTarih) + ",";
            sql += VeriErisim.DegerYap(form.geldigiYerTur) + ",";
            sql += VeriErisim.DegerYap(form.geldigiYer) + ",";
            sql += VeriErisim.DegerYap(form.gelenEvrakNo) + ",";
            sql += VeriErisim.DegerYap(form.gelenEvrakTarih) + ",";
            sql += VeriErisim.DegerYap(form.konu) + ",";
            sql += VeriErisim.DegerYap(form.gizlilik) + ",";
            sql += VeriErisim.DegerYap(form.gelisSekli) + ",";
            sql += VeriErisim.DegerYap(form.yaziDurum) + ",";
            sql += VeriErisim.DegerYap(form.cevapSuresi) + ",";
            sql += VeriErisim.DegerYap(form.aciklama) + ",";
            sql += VeriErisim.DegerYap(form.ekAdedi) + ",";
            sql += VeriErisim.DegerYap(form.havaleBirimKodu) + ",";
            sql += VeriErisim.DegerYap(form.evrakTuru) + ",";
            sql += VeriErisim.DegerYap(form.paraf) + ",";
            sql += VeriErisim.DegerYap(form.antet) + ",";
            sql += VeriErisim.DegerYap(form.icerik) + ",";
            sql += VeriErisim.DegerYap(form.imza1) + ",";
            sql += VeriErisim.DegerYap(form.imza2) + ",";
            sql += VeriErisim.DegerYap(form.imza3) + ",";
            sql += VeriErisim.DegerYap(form.ek) + ",";
            sql += VeriErisim.DegerYap(form.altBilgi) + ",";
            sql += VeriErisim.DegerYap(form.evrakOlusturmaTarihi) + ",";
            sql += VeriErisim.DegerYap(form.imzaTarihi) + ",";
            sql += VeriErisim.DegerYap(form.ekYaziAlan) + ",";
            sql += VeriErisim.DegerYap(form.siraNo) + ",";
            sql += VeriErisim.DegerYap(form.dosyaNo) + ",";
            sql += VeriErisim.DegerYap(form.evrakIcerigi) + ",";
            sql += VeriErisim.DegerYap(form.evrakCinsi) + ",";
            sql += VeriErisim.DegerYap(form.esasNo) + ",";
            sql += VeriErisim.DegerYap(form.evrakBelgeTur) + ",";
            sql += VeriErisim.DegerYap(form.imzaYolu) + ",";

            sql += VeriErisim.DegerYap(form.kod) + ",";
            sql += VeriErisim.DegerYap(form.versiyonAd) + ",";
            sql += VeriErisim.DegerYap(form.versiyonMu) + ",";
            sql += VeriErisim.DegerYap(form.islemYapan) + ",";
            sql += VeriErisim.DegerYap(form.islemYapanAd) + ",";
            sql += VeriErisim.DegerYap(form.islemTarih) + ")";

            return KomutCalistir(sql, 1);
        }

        public IDataReader Listele(EvrakVersiyon form)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar("", ALANLAR) + " FROM " + TABLOAD;

            if (form.kod > 0)
                sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", form.kod));
            else
            {
                sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.EVREVRAKNO], "=", form.evrEvrakNo));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.VERSIYONMU], "=", form.versiyonMu));
            }

            sql += " ORDER BY " + ALANLAR[(int)ENUMAlanlar.KOD] + " DESC";

            return veriErisim.DataReaderDondur(sql);
        }

        public Sonuc Sil(int kod)
        {
            string sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", kod));
            return KomutCalistir(sql, 1);
        }

        public int SonVersiyonKod(string evrEvrakNo)
        {
            string sql = "SELECT MAX(" + ALANLAR[(int)ENUMAlanlar.KOD] + ") FROM " + TABLOAD;
            return veriErisim.TekKolonSQLInt(sql);
        }

    }
}
