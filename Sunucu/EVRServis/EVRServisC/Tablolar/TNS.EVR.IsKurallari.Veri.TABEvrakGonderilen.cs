using System;
using System.Data;
using TNS;
using TNS.Veri;
using TNS.EVR;
namespace TNS.EVR.IsKurallari.Veri
{
    /// <summary>
    /// Gönderilen evrak bilgilerini içeren EVREVRAKGONDERILEN veri tabanı tablosu ile ilgili, kaydetme, silme ve listeleme yordamlarını tutan sınıf.
    /// </summary>
    public class TABEvrakGonderilen : TABOrtak
    {
        /// <summary>        
        /// Parametre olarak verilen veriErisim nesnesine göre, TABEvrakGonderilensınıfının yeni bir örneğini oluşturur.
        /// </summary>
        /// <param name="veriErisim">Veri erişim bilgisini içeren nesne</param>
        public TABEvrakGonderilen(VeriErisim veriErisim) : base(veriErisim) { }

        /// <summary>
        /// Gönderilen evrak bilgilerini içeren veri tabanı tablosunun adını tutan değişken.
        /// </summary>
        public static string TABLOAD = "EVREVRAKGONDERILEN";

        /// <summary>
        /// Gönderilen evrak bilgilerini içeren veri tabanı tablosunun alanlarının sırasını tutan numaralandırma.
        /// </summary>
        public enum ENUMAlanlar : int
        {
            PREVRAKNO = 0, GONDERILENBIRIMKODU = 1, GONDERENBIRIMKODU = 2, KONU = 3, EVRAKTARIH = 4, EVRAKNO = 5, GIZLILIK = 6, YAZIDURUM = 7, EVRAKDOSYAKODU = 8, DOSYA = 9, ALINDIMI = 10
        }

        /// <summary>
        /// Gönderilen evrak bilgilerini içeren veri tabanı tablosunun alanlarının isimlerini tutan dizi.
        /// </summary>
        public static string[] ALANLAR = { "PREVRAKNO", "GONDERILENBIRIMKODU", "GONDERENBIRIMKODU", "KONU", "EVRAKTARIH", "EVRAKNO", "GIZLILIK", "YAZIDURUM", "EVRAKDOSYAKODU", "DOSYA", "ALINDIMI" };

        /// <summary>
        /// Parametre olarak verilen EvrakGonderilen nesne koleksiyonuna göre; yeni gönderilen evrak bilgileri olarak, sql cümlesi hazırlayıp çalıştırarak veritabanına kaydeden yordam.
        /// </summary>
        /// <param name="liste">Gönderilen evrak bilisini içeren nesne</param>
        /// <returns>İşlem sonucu Sonuc nesnesi olarak döndürülür.</returns>
        public Sonuc Kaydet(ObjectArray liste)
        {
            Sonuc sonuc = new Sonuc(false, "", "");
            if (liste.sonuc.anahtar == "1") { sonuc = Guncelle(liste); } //Güncelle
            else
            {
                string sql = "";
                foreach (EvrakGonderilen form in liste.objeler)
                {
                    sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
                    sql += VeriErisim.DegerYap(form.prEvrakNo) + ",";
                    sql += VeriErisim.DegerYap(form.gonderilenBirimKodu) + ",";
                    sql += VeriErisim.DegerYap(form.gonderenBirimKodu) + ",";
                    sql += VeriErisim.DegerYap(form.konu) + ",";
                    sql += VeriErisim.DegerYap(form.evrakTarih) + ",";
                    sql += VeriErisim.DegerYap(form.evrakNo) + ",";
                    sql += VeriErisim.DegerYap(form.gizlilik) + ",";
                    sql += VeriErisim.DegerYap(form.yaziDurum) + ",";
                    sql += VeriErisim.DegerYap(form.evrakDosyaKodu) + ",";
                    sql += VeriErisim.DegerYap(form.dosya) + ",";
                    sql += VeriErisim.DegerYap(form.alindimi) + ")";

                    sonuc = KomutCalistir(sql, 1);
                    if (!sonuc.islemSonuc)
                        return sonuc;
                }
            }
            return sonuc;
        }

        /// <summary>
        /// Parametre olarak verilen EvrakGonderilen nesne koleksiyonuna göre; veritabanında kayıtlı gönderilen evrak bilgilerinin son halini, sql cümlesi hazırlayıp çalıştırarak veritabanına kaydeden yordam.
        /// </summary>
        /// <param name="liste">Gönderilen evrak bilisini içeren nesne</param>
        /// <returns>İşlem sonucu Sonuc nesnesi olarak döndürülür.</returns>
        public Sonuc Guncelle(ObjectArray liste)
        {
            Sonuc sonuc = new Sonuc(false, "", "");
            string sql = "";
            foreach (EvrakGonderilen form in liste.objeler)
            {
                sql = "UPDATE " + TABLOAD + " SET ";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ALINDIMI], form.alindimi);
                sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.PREVRAKNO], "=", (form.prEvrakNo)));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.GONDERILENBIRIMKODU], "=", (form.gonderilenBirimKodu)));

                sonuc = KomutCalistir(sql, 1);
                if (!sonuc.islemSonuc) return sonuc;
            }
            return sonuc;
        }

        /// <summary>
        /// Paramete olarak verilen bilgilere göre; gönderilen evrak bilgilerini, sql cümlesi hazırlayıp çalıştırarak veritabanından okuyan yordam.
        /// </summary>
        /// <param name="gonderilenBirimKodu">Gönderilen evrak birim kodu bilgisini içeren nesne</param>
        /// <param name="alindimi">Alınıp alınmadığı bilgisini içeren nesne</param>
        /// <returns>İşlem sonucu DataReader olarak döndürülür.</returns>
        public IDataReader Listele(string gonderilenBirimKodu, int alindimi)
        {
            string sql = "";
            string kosul = "";

            sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR) + " FROM " + TABLOAD;

            if (gonderilenBirimKodu != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.GONDERILENBIRIMKODU], "LIKE", gonderilenBirimKodu + "%"));
            if (alindimi == 0) //Alınmayanları Listele
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ALINDIMI], "=", ""));
            if (alindimi == 1) //Alınanları Listele
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ALINDIMI], "<>", ""));

            if (kosul != "")
                sql += " WHERE " + kosul;

            return veriErisim.DataReaderDondur(sql);
        }
    }
}
