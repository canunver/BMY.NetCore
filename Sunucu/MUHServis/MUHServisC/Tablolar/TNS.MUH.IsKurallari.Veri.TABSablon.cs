using System;
using System.Data;
using TNS.Veri;

namespace TNS.MUH.IsKurallari.Veri
{
    /// <summary>
    /// MUHSABLON* veri tabanı tablosu ile ilgili, listeleme ve yazma işlemleri tutan nesne
    /// </summary>
    public class TABSablon : TABOrtak
    {
        /// <summary>
        /// Sınıfın oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABSablon(VeriErisim veriErisim) : base(veriErisim) { }

        static string TABLOADFORM = "MUHSABLONFORM";
        static string TABLOADDETAY = "MUHSABLON";
        static string TABLOADICERIKTUR = "MUHSABLONICERIK";
        static string TABLOADHESAP = "MUHHESAPPLANI";

        enum ENUMAlanlarForm
        {
            KOD = 0,
            YIL = 1,
            BELGETUR = 2,
            BUTCETUR = 3,
            ICERIKTUR = 4,
            AD = 5,
            DURUM = 6,
            DURUMTARIH = 7,
            ISLEMYAPAN = 8,
            GORMEYETKISI = 9,
            KURUMKOD = 10,
            MUHASEBEKOD = 11,
            BIRIMKOD = 12,
            IADSOYAD = 13,
            IKIMLIKNO = 14,
            ISUBEAD = 15,
            IHESAPNO = 16,
            IVERGIDAIRE = 17,
            ACIKLAMA = 18,
            KURKOD = 19,
            ORGANIZASYONKOD = 20,
        }
        static string[] ALANLARFORM = { "KOD", "YIL", "BELGETUR", "BUTCETUR", "ICERIKTUR", "AD", "DURUM", "DURUMTARIH", "ISLEMYAPAN", "GORMEYETKISI", "KURUMKOD", "MUHASEBEKOD", "BIRIMKOD", "IADSOYAD", "IKIMLIKNO", "ISUBEAD", "IHESAPNO", "IVERGIDAIRE", "ACIKLAMA", "KURKOD", "ORGANIZASYONKOD" };

        enum ENUMAlanlarDetay
        {
            KOD = 0,
            SIRA = 1,
            YIL = 2,
            HESAPKOD = 3,
            KURKOD = 4,
            FONKOD = 5,
            FINKOD = 6,
            EKOKOD = 7,
            BORC = 8,
            ALACAK = 9,
            KESINTI = 10,
            PEBKOD = 11,
            DOVIZ = 12,
            DOVIZKURTUR = 13,
            DOVIZBORC = 14,
            DOVIZALACAK = 15,
            DOVIZKUR = 16,
            DETAYBIRIM = 17,
            ALTHESAPKOD = 18,
            SATIRACIKLAMA = 19
        }
        static string[] ALANLARDETAY = { "KOD", "SIRA", "YIL", "HESAPKOD", "KURKOD", "FONKOD", "FINKOD", "EKOKOD", "BORC", "ALACAK", "KESINTI", "PEBKOD", "DOVIZ", "DOVIZKURTUR", "DOVIZBORC", "DOVIZALACAK", "DOVIZKUR", "DETAYBIRIM", "ALTHESAPKOD", "SATIRACIKLAMA" };

        enum ENUMAlanlarIcerikTur
        {
            BELGETUR = 0,
            KOD = 1,
            AD = 2,
        }
        static string[] ALANLARICERIKTUR = { "BELGETUR", "KOD", "AD" };

        enum ENUMAlanlarHESAP
        {
            YIL = 0,
            HESAPKOD = 1,
            ACIKLAMA = 2,
            KURUMKOD = 3,
        }
        static string[] ALANLARHESAP = { "YIL", "HESAPKOD", "ACIKLAMA", "KURUMKOD" };

        /// <summary>
        /// Liste sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon.
        /// </summary>
        /// <param name="sablon">Kriter bilgisi.</param>
        /// <returns></returns>
        public IDataReader Liste(Sablon sablon)
        {
            string sql = "";

            sql = "SELECT " + VeriErisim.TumAlanlar("", ALANLARFORM) + " FROM " + TABLOADFORM + " WHERE ";

            if (sablon.kod > 0)
                sql += VeriErisim.KosulOl("", ALANLARFORM[(int)ENUMAlanlarForm.KOD], "=", sablon.kod);
            else
                sql += VeriErisim.KosulOl("", ALANLARFORM[(int)ENUMAlanlarForm.KOD], ">", 0);

            if (sablon.yil > 0)
                sql += " AND " + VeriErisim.KosulOl("", ALANLARFORM[(int)ENUMAlanlarForm.YIL], "=", sablon.yil);

            if (sablon.belgeTur != ENUMBelgeTuru.TANIMSIZ)
                sql += " AND " + VeriErisim.KosulOl("", ALANLARFORM[(int)ENUMAlanlarForm.BELGETUR], "=", (int)sablon.belgeTur);

            if (sablon.butceTur != "")
                sql += " AND " + VeriErisim.KosulOl("", ALANLARFORM[(int)ENUMAlanlarForm.BUTCETUR], "=", sablon.butceTur);

            if (sablon.icerikTur > 0)
                sql += " AND " + VeriErisim.KosulOl("", ALANLARFORM[(int)ENUMAlanlarForm.ICERIKTUR], "=", sablon.icerikTur);

            if (sablon.islemYapan != "")
                sql += " AND " + VeriErisim.KosulOl("", ALANLARFORM[(int)ENUMAlanlarForm.ISLEMYAPAN], "=", sablon.islemYapan);

            if (sablon.kurum != "")
                sql += " AND " + VeriErisim.KosulOl("", ALANLARFORM[(int)ENUMAlanlarForm.KURUMKOD], "=", sablon.kurum.Replace(".", ""));

            if (sablon.gormeYetkisi > 0)
                sql += " AND " + VeriErisim.KosulOl("", ALANLARFORM[(int)ENUMAlanlarForm.GORMEYETKISI], "=", sablon.gormeYetkisi);

            if (sablon.muhasebe > 0)
                sql += " AND " + VeriErisim.KosulOl("", ALANLARFORM[(int)ENUMAlanlarForm.MUHASEBEKOD], "=", sablon.muhasebe);

            sql += " ORDER BY " + ALANLARFORM[(int)ENUMAlanlarForm.AD];

            IDataReader r = veriErisim.DataReaderDondur(sql);

            return r;
        }

        /// <summary>
        /// Liste sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon.
        /// </summary>
        /// <param name="sablon">Kriter bilgisi.</param>
        /// <returns></returns>
        public IDataReader ListeDetay(Sablon sablon)
        {
            string sql = "";

            sql = "SELECT " + VeriErisim.TumAlanlar(TABLOADDETAY, ALANLARDETAY) + "," + ALANLARHESAP[(int)ENUMAlanlarHESAP.ACIKLAMA] + " FROM " + TABLOADDETAY + "," + TABLOADHESAP + " WHERE ";

            sql += VeriErisim.KosulOl(TABLOADDETAY, ALANLARDETAY[(int)ENUMAlanlarDetay.KOD], "=", sablon.kod);
            sql += " AND " + VeriErisim.KosulOl(TABLOADDETAY, ALANLARDETAY[(int)ENUMAlanlarDetay.YIL], "=", sablon.yil);

            sql = VeriErisim.KosulEkle(sql, "AND", TABLOADDETAY + "." + ALANLARDETAY[(int)ENUMAlanlarDetay.YIL] + "=" + TABLOADHESAP + "." + ALANLARHESAP[(int)ENUMAlanlarHESAP.YIL]);
            sql = VeriErisim.KosulEkle(sql, "AND", TABLOADDETAY + "." + ALANLARDETAY[(int)ENUMAlanlarDetay.HESAPKOD] + "=" + TABLOADHESAP + "." + ALANLARHESAP[(int)ENUMAlanlarHESAP.HESAPKOD]);
            //sql = VeriErisim.KosulEkle(sql, "AND", TABLOADHESAP + "." + ALANLARHESAP[(int)ENUMAlanlarHESAP.KURUMKOD] + "='" + sablon.butceTur + "'");

            sql += " ORDER BY " + TABLOADDETAY + "." + ALANLARDETAY[(int)ENUMAlanlarDetay.SIRA];

            IDataReader r = veriErisim.DataReaderDondur(sql);

            return r;
        }

        /// <summary>
        /// Parametre olarak gelen bilgileri veri tabanına ekleyen sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="sablon">Eklenecek kayıt bilgisini tutan nesne</param>
        /// <returns>Kaydet komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc Kaydet(Sablon sablon)
        {
            Sonuc sonuc = new Sonuc(false, "", "");
            string sql = "";

            sql = "INSERT INTO " + TABLOADFORM + " (" + VeriErisim.TumAlanlar("", ALANLARFORM) + ") VALUES (";
            sql = sql + VeriErisim.DegerYap(sablon.kod) + ", ";
            sql = sql + VeriErisim.DegerYap(sablon.yil) + ", ";
            sql = sql + VeriErisim.DegerYap((int)sablon.belgeTur) + ", ";
            sql = sql + VeriErisim.DegerYap(sablon.butceTur) + ", ";
            sql = sql + VeriErisim.DegerYap(sablon.icerikTur) + ", ";
            sql = sql + VeriErisim.DegerYap(sablon.ad) + ", ";
            sql = sql + VeriErisim.DegerYap(sablon.durum) + ", ";
            sql = sql + VeriErisim.DegerYap(new TNSDateTime(DateTime.Now.ToLongDateString())) + ", ";
            sql = sql + VeriErisim.DegerYap(sablon.islemYapan) + ", ";
            sql = sql + VeriErisim.DegerYap(sablon.gormeYetkisi) + ", ";
            sql = sql + VeriErisim.DegerYap(sablon.kurum.Replace(".", "")) + ", ";
            sql = sql + VeriErisim.DegerYap(sablon.muhasebe) + ", ";
            sql = sql + VeriErisim.DegerYap(sablon.birim) + ", ";
            sql = sql + VeriErisim.DegerYap(sablon.ilgiliAd) + ", ";
            sql = sql + VeriErisim.DegerYap(sablon.ilgiliNo) + ", ";
            sql = sql + VeriErisim.DegerYap(sablon.ilgiliBankaAd) + ", ";
            sql = sql + VeriErisim.DegerYap(sablon.ilgiliBankaNo) + ", ";
            sql = sql + VeriErisim.DegerYap(sablon.ilgiliVD) + ", ";
            sql = sql + VeriErisim.DegerYap(sablon.aciklama) + ", ";
            sql = sql + VeriErisim.DegerYap(sablon.kurKod) + ",";
            sql = sql + VeriErisim.DegerYap(sablon.organizasyonKodu) + ")";

            sonuc = KomutCalistir(sql, 1);

            if (sonuc.islemSonuc)
            {
                int siraNo = 0;
                foreach (SablonDetay detay in sablon.detay.objeler)
                {
                    siraNo++;
                    string kur = "";
                    string fon = "";
                    string fin = "";
                    string eko = "";
                    string peb = "";

                    if (detay.tertip.kodlar.Length > 0)
                        kur = OrtakFonksiyonlar.ConvertToStr(detay.tertip.kodlar[0] + "");
                    if (detay.tertip.kodlar.Length > 1)
                        fon = OrtakFonksiyonlar.ConvertToStr(detay.tertip.kodlar[1] + "");
                    if (detay.tertip.kodlar.Length > 2)
                        fin = OrtakFonksiyonlar.ConvertToStr(detay.tertip.kodlar[2] + "");
                    if (detay.tertip.kodlar.Length > 3)
                        eko = OrtakFonksiyonlar.ConvertToStr(detay.tertip.kodlar[3] + "");
                    if (detay.tertip.kodlar.Length > 4)
                        peb = OrtakFonksiyonlar.ConvertToStr(detay.tertip.kodlar[4] + "");

                    sql = "INSERT INTO " + TABLOADDETAY + " (" + VeriErisim.TumAlanlar("", ALANLARDETAY) + ") VALUES (";
                    sql = sql + VeriErisim.DegerYap(sablon.kod) + ", ";
                    sql = sql + VeriErisim.DegerYap(siraNo) + ", ";
                    sql = sql + VeriErisim.DegerYap(sablon.yil) + ", ";
                    sql = sql + VeriErisim.DegerYap(detay.hesapKod.Replace(".", "")) + ", ";
                    sql = sql + VeriErisim.DegerYap(kur.Replace(".", "")) + ", ";
                    sql = sql + VeriErisim.DegerYap(fon.Replace(".", "")) + ", ";
                    sql = sql + VeriErisim.DegerYap(fin.Replace(".", "")) + ", ";
                    sql = sql + VeriErisim.DegerYap(eko.Replace(".", "")) + ", ";
                    sql = sql + VeriErisim.DegerYap(detay.borc) + ", ";
                    sql = sql + VeriErisim.DegerYap(detay.alacak) + ", ";
                    sql = sql + VeriErisim.DegerYap(detay.kesinti) + ",";
                    sql = sql + VeriErisim.DegerYap(peb.Replace(".", "")) + ", ";
                    sql = sql + VeriErisim.DegerYap(detay.doviz) + ",";
                    sql = sql + VeriErisim.DegerYap(detay.dovizKurTur) + ",";
                    sql = sql + VeriErisim.DegerYap(detay.dovizBorc) + ",";
                    sql = sql + VeriErisim.DegerYap(detay.dovizAlacak) + ",";
                    sql = sql + VeriErisim.DegerYap(detay.dovizKur) + ",";
                    sql = sql + VeriErisim.DegerYap(detay.detayBirim) + ",";
                    sql = sql + VeriErisim.DegerYap(detay.altHesapKod) + ",";
                    sql = sql + VeriErisim.DegerYap(detay.satirAciklama) + ")";

                    sonuc = KomutCalistir(sql, 1);
                }
            }

            return sonuc;
        }

        /// <summary>
        /// Parametre olarak gelen anahtar alanları kullanarak, ilgili kaydı veri tabanından silen sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="yil">Yıl</param>
        /// <param name="kod">Şablon Kod</param>
        /// <returns>Sil komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc Sil(int yil, int kod)
        {
            Sonuc sonuc = new Sonuc(false, "", "");
            string sql = "";

            sql = "DELETE FROM " + TABLOADFORM;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARFORM[(int)ENUMAlanlarForm.KOD], "=", kod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARFORM[(int)ENUMAlanlarForm.YIL], "=", yil));

            sonuc = KomutCalistir(sql, 0);

            if (sonuc.islemSonuc)
            {
                sql = "DELETE FROM " + TABLOADDETAY;
                sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARDETAY[(int)ENUMAlanlarDetay.KOD], "=", kod));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARDETAY[(int)ENUMAlanlarDetay.YIL], "=", yil));

                sonuc = KomutCalistir(sql, 0);
            }

            return sonuc;
        }

        /// <summary>
        /// Icerik tur liste sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <returns></returns>
        public IDataReader IcerikTurListe()
        {
            string sql = "";

            sql = "SELECT " + VeriErisim.TumAlanlar("", ALANLARICERIKTUR) + " FROM " + TABLOADICERIKTUR;
            sql += " ORDER BY " + ALANLARICERIKTUR[(int)ENUMAlanlarIcerikTur.AD];

            IDataReader r = veriErisim.DataReaderDondur(sql);

            return r;
        }
    }
}
