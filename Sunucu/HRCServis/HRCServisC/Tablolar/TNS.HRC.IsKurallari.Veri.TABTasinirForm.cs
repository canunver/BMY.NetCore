using System;
using System.Data;
using TNS.Veri;

namespace TNS.HRC.IsKurallari.Veri
{
    /// <summary>
    /// Taşınır belge bilgisini tutan HRCTASINIRFORM ve HRCTASINIRDETAY veri tabanı tabloları ile ilgili, 
    /// listeleme ve yazma işlemleri tutan nesne
    /// </summary>
    public class TABTasinirForm : TABOrtak
    {
        /// <summary>
        /// Sınıfın oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABTasinirForm(VeriErisim veriErisim) : base(veriErisim) { }

        /// <summary>
        /// Taşınır belge bilgisi tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLOAD = "HRCTASINIRFORM";

        /// <summary>
        /// Taşınır belge bilgisi tablo alanlarının sırasını tutan enumeration
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
            /// PRTASINIRNO alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            PRTASINIRNO = 4,
            /// <summary>
            /// PRFATURANO alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            PRFATURANO = 5,
            /// <summary>
            /// TASINIRNO alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            TASINIRNO = 6,
            /// <summary>
            /// TASINIRTARIH alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            TASINIRTARIH = 7,
            /// <summary>
            /// HARCAMABIRIMKOD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            HARCAMABIRIMKOD = 8,
            /// <summary>
            /// AMBARKOD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            AMBARKOD = 9,
            /// <summary>
            /// ACIKLAMA alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            ACIKLAMA = 10,
            DOVIZKUR = 11
        }

        /// <summary>
        /// Taşınır belge tablosu alanlarının isimlerini tutan sabit dizi
        /// </summary>
        public static string[] ALANLAR = { "YIL", "MUHASEBEKOD", "BIRIMKOD", "SURECNO", "PRTASINIRNO", "PRFATURANO", "TASINIRNO", "TASINIRTARIH", "HARCAMABIRIMKOD", "AMBARKOD", "ACIKLAMA", "DOVIZKUR" };

        /// <summary>
        /// Taşınır belge detay bilgisi tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLOADDETAY = "HRCTASINIRDETAY";

        /// <summary>
        /// Taşınır belge detay bilgisi tablo alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlarDetay
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
            /// PRTASINIRNO alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            PRTASINIRNO = 4,
            /// <summary>
            /// SIRANO alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            SIRANO = 5,
            /// <summary>
            /// ADI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            ADI = 6,
            /// <summary>
            /// HESAPPLANKOD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            HESAPPLANKOD = 7,
            /// <summary>
            /// MIKTAR alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            MIKTAR = 8,
            /// <summary>
            /// OLCUBIRIMKOD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            OLCUBIRIMKOD = 9,
            /// <summary>
            /// KDVORAN alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            KDVORAN = 10,
            /// <summary>
            /// BIRIMFIYAT alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            BIRIMFIYAT = 11,
            /// <summary>
            /// OLUSTURULDU alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            OLUSTURULDU = 12,
            /// <summary>
            /// FATURADETAYSIRANO alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            FATURADETAYSIRANO = 13
        }

        /// <summary>
        /// Taşınır belge detay tablosu alanlarının isimlerini tutan sabit dizi
        /// </summary>
        public static string[] ALANLARDETAY = { "YIL", "MUHASEBEKOD", "BIRIMKOD", "SURECNO", "PRTASINIRNO", "SIRANO", "ADI", "HESAPPLANKOD", "MIKTAR", "OLCUBIRIMKOD", "KDVORAN", "BIRIMFIYAT", "OLUSTURULDU", "FATURADETAYSIRANO" };

        /// <summary>
        /// Taşınır belge bilgisini veri tabanına eklemek için
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="bilgi">Eklenecek taşınır belge bilgisini tutan nesne</param>
        /// <returns>Oluşturulan insert komutunun çalıştırılması sonucunu, Sonuc yapısında döndürür</returns>
        public Sonuc Kaydet(TasinirForm bilgi)
        {
            Sonuc sonuc = new Sonuc(true, "", "");
            string sql = "";

            sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
            sql += VeriErisim.DegerYap(bilgi.yil) + ",";
            sql += VeriErisim.DegerYap(bilgi.muhasebe) + ",";
            sql += VeriErisim.DegerYap(bilgi.birim) + ",";
            sql += VeriErisim.DegerYap(bilgi.surecNo) + ",";
            sql += VeriErisim.DegerYap(bilgi.prTasinirNo) + ",";
            sql += VeriErisim.DegerYap(bilgi.prFaturaNo) + ",";
            sql += VeriErisim.DegerYap(bilgi.tasinirNo) + ",";
            sql += VeriErisim.DegerYap(bilgi.tasinirTarih) + ",";
            sql += VeriErisim.DegerYap(bilgi.harcamaBirimKod.Replace(".", "")) + ",";
            sql += VeriErisim.DegerYap(bilgi.ambarKod) + ",";
            sql += VeriErisim.DegerYap(bilgi.aciklama) + ",";
            sql += VeriErisim.DegerYap(bilgi.dovizKur) + ")";

            sonuc = KomutCalistir(sql, 1);

            if (!sonuc.islemSonuc)
            {
                veriErisim.hata = "";
                sql = "UPDATE " + TABLOAD + " SET ";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.PRFATURANO], bilgi.prFaturaNo) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.TASINIRNO], bilgi.tasinirNo) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.TASINIRTARIH], bilgi.tasinirTarih) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.HARCAMABIRIMKOD], bilgi.harcamaBirimKod.Replace(".", "")) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.AMBARKOD], bilgi.ambarKod) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.DOVIZKUR], bilgi.dovizKur) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ACIKLAMA], bilgi.aciklama) + " ";
                sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", bilgi.yil));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", bilgi.muhasebe));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.BIRIMKOD], "=", bilgi.birim));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SURECNO], "=", bilgi.surecNo));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.PRTASINIRNO], "=", bilgi.prTasinirNo));

                sonuc = KomutCalistir(sql, 1);
            }
            return sonuc;
        }

        /// <summary>
        /// Taşınır belge detay bilgisini veri tabanına eklemek için
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="bilgi">Eklenecek taşınır belge bilgisini tutan nesne</param>
        /// <returns>Oluşturulan insert komutunun çalıştırılması sonucunu, Sonuc yapısında döndürür</returns>
        public Sonuc KaydetDetay(TasinirForm bilgi)
        {
            Sonuc sonuc = new Sonuc(false, "", "");
            string sql = "";

            for (int i = 0; i < bilgi.detay.objeler.Count; i++)
            {
                sql = "INSERT INTO " + TABLOADDETAY + " (" + VeriErisim.TumAlanlar("", ALANLARDETAY) + ") VALUES (";
                sql += VeriErisim.DegerYap(bilgi.yil) + ",";
                sql += VeriErisim.DegerYap(bilgi.muhasebe) + ",";
                sql += VeriErisim.DegerYap(bilgi.birim) + ",";
                sql += VeriErisim.DegerYap(bilgi.surecNo) + ",";
                sql += VeriErisim.DegerYap(bilgi.prTasinirNo) + ",";
                sql += VeriErisim.DegerYap(i + 1) + ",";
                sql += VeriErisim.DegerYap(((TasinirDetay)bilgi.detay.objeler[i]).adi) + ",";
                sql += VeriErisim.DegerYap(((TasinirDetay)bilgi.detay.objeler[i]).hesapPlanKod.Replace(".", "")) + ",";
                sql += VeriErisim.DegerYap(((TasinirDetay)bilgi.detay.objeler[i]).miktar) + ",";
                sql += VeriErisim.DegerYap(((TasinirDetay)bilgi.detay.objeler[i]).olcuBirimKod) + ",";
                sql += VeriErisim.DegerYap(((TasinirDetay)bilgi.detay.objeler[i]).kdvOran) + ",";
                sql += VeriErisim.DegerYap(((TasinirDetay)bilgi.detay.objeler[i]).birimFiyat) + ",";
                sql += VeriErisim.DegerYap(((TasinirDetay)bilgi.detay.objeler[i]).olusturuldu) + ",";
                sql += VeriErisim.DegerYap(((TasinirDetay)bilgi.detay.objeler[i]).faturaDetaySiraNo) + ")";

                sonuc = KomutCalistir(sql, 1);
                if (!sonuc.islemSonuc)
                    break;
            }

            return sonuc;
        }

        /// <summary>
        /// Taşınır belge bilgisini veri tabanından çeken sql cümleciğini verilen kıstasa göre
        /// üretip, çalıştırıp, datareader döndüren yordam.
        /// </summary>
        /// <param name="bilgi">Listeleme için kullanılacak taşınır belge bilgisi kıstas parametresi</param>
        /// <returns>
        /// Taşınır belge bilgisini bulmak için oluşturulan select komutunun çalıştırılması sonucu elde edilen
        /// DataReader nesnesini döndürür
        /// </returns>
        public IDataReader Listele(TasinirForm bilgi)
        {
            string sql = "";
            string kosul = "";

            sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR) + " FROM " + TABLOAD;

            if (bilgi.yil > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.YIL], "=", bilgi.yil));
            if (bilgi.muhasebe > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", bilgi.muhasebe));
            if (bilgi.birim != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.BIRIMKOD], "=", bilgi.birim));
            if (bilgi.surecNo != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.SURECNO], "=", bilgi.surecNo));
            if (bilgi.prTasinirNo > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.PRTASINIRNO], "=", bilgi.prTasinirNo));
            if (bilgi.prFaturaNo > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.PRFATURANO], "=", bilgi.prFaturaNo));
            if (bilgi.tasinirNo != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.TASINIRNO], "=", bilgi.tasinirNo));
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

            sql += " ORDER BY " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD] + ", " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.BIRIMKOD] + ", " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.PRTASINIRNO];

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Taşınır belge detay bilgisini veri tabanından çeken sql cümleciğini verilen kıstasa göre
        /// üretip, çalıştırıp, datareader döndüren yordam.
        /// </summary>
        /// <param name="bilgi">Listeleme için kullanılacak taşınır belge bilgisi kıstas parametresi</param>
        /// <returns>
        /// Taşınır belge detay bilgisini bulmak için oluşturulan select komutunun çalıştırılması sonucu elde edilen
        /// DataReader nesnesini döndürür
        /// </returns>
        public IDataReader ListeleDetay(TasinirForm bilgi)
        {
            string sql = "";
            string kosul = "";

            sql = "SELECT " + VeriErisim.TumAlanlar("", ALANLARDETAY) + " FROM " + TABLOADDETAY;
            if (bilgi.yil > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlarDetay.YIL], "=", bilgi.yil));
            if (bilgi.muhasebe > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlarDetay.MUHASEBEKOD], "=", bilgi.muhasebe));
            if (bilgi.birim != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlarDetay.BIRIMKOD], "=", bilgi.birim));
            if (bilgi.surecNo != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlarDetay.SURECNO], "=", bilgi.surecNo));
            if (bilgi.prTasinirNo > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlarDetay.PRTASINIRNO], "=", bilgi.prTasinirNo));

            if (kosul != "")
                sql += " WHERE " + kosul;

            sql += " ORDER BY " + ALANLARDETAY[(int)ENUMAlanlarDetay.MUHASEBEKOD] + ", " + ALANLARDETAY[(int)ENUMAlanlarDetay.BIRIMKOD] + "," + ALANLARDETAY[(int)ENUMAlanlarDetay.SURECNO] + "," + ALANLARDETAY[(int)ENUMAlanlarDetay.PRTASINIRNO] + "," + ALANLARDETAY[(int)ENUMAlanlarDetay.SIRANO];

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Taşınır belge bilgisini veri tabanından silmek için
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="bilgi">Silinecek taşınır belge bilgisini tutan nesne</param>
        /// <returns>Oluşturulan delete komutunun çalıştırılması sonucunu, Sonuc yapısında döndürür</returns>
        public Sonuc Sil(TasinirForm bilgi)
        {
            string sql = "";

            sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlarDetay.YIL], "=", (bilgi.yil)));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlarDetay.MUHASEBEKOD], "=", (bilgi.muhasebe)));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlarDetay.BIRIMKOD], "=", (bilgi.birim)));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlarDetay.SURECNO], "=", (bilgi.surecNo)));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlarDetay.PRTASINIRNO], "=", (bilgi.prTasinirNo)));

            return KomutCalistir(sql, 0);
        }

        /// <summary>
        /// Taşınır belge detay bilgisini veri tabanından silmek için
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="bilgi">Silinecek taşınır belge bilgisini tutan nesne</param>
        /// <returns>Oluşturulan delete komutunun çalıştırılması sonucunu, Sonuc yapısında döndürür</returns>
        public Sonuc SilDetay(TasinirForm bilgi)
        {
            string sql = "";

            sql = "DELETE FROM " + TABLOADDETAY;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARDETAY[(int)ENUMAlanlarDetay.YIL], "=", (bilgi.yil)));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARDETAY[(int)ENUMAlanlarDetay.MUHASEBEKOD], "=", (bilgi.muhasebe)));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARDETAY[(int)ENUMAlanlarDetay.BIRIMKOD], "=", (bilgi.birim)));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARDETAY[(int)ENUMAlanlarDetay.SURECNO], "=", (bilgi.surecNo)));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARDETAY[(int)ENUMAlanlarDetay.PRTASINIRNO], "=", (bilgi.prTasinirNo)));

            return KomutCalistir(sql, 0);
        }

        /// <summary>
        /// Taşınır harcama bilgisini veri tabanından çeken sql cümleciğini verilen kıstasa göre
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
        /// Taşınır harcama bilgisini bulmak için oluşturulan select komutunun çalıştırılması sonucu elde edilen
        /// DataReader nesnesini döndürür
        /// </returns>
        public IDataReader ListeleButceHazirlikTeslimHarcama(int yil, int ay, int muhasebe, string birim, string kur, string fon, string fin, string eko, string peb)
        {
            string sql = string.Empty;

            sql = "SELECT     YEAR(HRCTASINIRFORM.TASINIRTARIH) AS ISLEMYIL, MONTH(HRCTASINIRFORM.TASINIRTARIH) AS ISLEMAY, HRCSUREC.YIL AS SURECYIL,  " +
                      " HRCTASINIRFORM.SURECNO, HRCTASINIRFORM.MUHASEBEKOD, HRCTASINIRFORM.BIRIMKOD, HRCIHALEONAYTERTIP.KURKOD,  " +
                      " HRCIHALEONAYTERTIP.FONKOD, HRCIHALEONAYTERTIP.FINKOD, HRCIHALEONAYTERTIP.EKOKOD, HRCIHALEONAYTERTIP.PEBKOD,  " +
                      " SUM(HRCTASINIRDETAY.MIKTAR * HRCTASINIRDETAY.BIRIMFIYAT) AS TOPLAM, HRCSUREC.YILLARASARIISLEMMI " +
                    " FROM         HRCIHALEONAYTERTIP RIGHT OUTER JOIN " +
                      " HRCFATURAFORM ON HRCIHALEONAYTERTIP.YIL = HRCFATURAFORM.YIL AND  " +
                      " HRCIHALEONAYTERTIP.SURECNO = HRCFATURAFORM.SURECNO RIGHT OUTER JOIN " +
                      " HRCTASINIRFORM ON HRCFATURAFORM.YIL = HRCTASINIRFORM.YIL AND HRCFATURAFORM.SURECNO = HRCTASINIRFORM.SURECNO AND  " +
                      " HRCFATURAFORM.PRFATURANO = HRCTASINIRFORM.PRFATURANO LEFT OUTER JOIN " +
                      " HRCSUREC ON HRCTASINIRFORM.YIL = HRCSUREC.YIL AND HRCTASINIRFORM.SURECNO = HRCSUREC.SURECNO LEFT OUTER JOIN " +
                      " HRCTASINIRDETAY ON HRCTASINIRFORM.YIL = HRCTASINIRDETAY.YIL AND HRCTASINIRFORM.SURECNO = HRCTASINIRDETAY.SURECNO AND  " +
                      " HRCTASINIRFORM.PRTASINIRNO = HRCTASINIRDETAY.PRTASINIRNO " +
                    " WHERE     (HRCSUREC.SURECDURUM <> 9) ";

            if (yil > 0)
                sql += " AND (YEAR(HRCTASINIRFORM.TASINIRTARIH) = " + yil + ") ";
            if (ay > 0)
                sql += " AND (MONTH(HRCTASINIRFORM.TASINIRTARIH) = " + ay + ") ";
            if (muhasebe > 0)
                sql += " AND (HRCTASINIRFORM.MUHASEBEKOD = " + muhasebe + ") ";
            if (!string.IsNullOrEmpty(birim))
                sql += " AND (HRCTASINIRFORM.BIRIMKOD = '" + birim + "') ";
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

            sql += " GROUP BY YEAR(HRCTASINIRFORM.TASINIRTARIH), MONTH(HRCTASINIRFORM.TASINIRTARIH), HRCTASINIRFORM.SURECNO, HRCTASINIRFORM.MUHASEBEKOD,  " +
                      " HRCTASINIRFORM.BIRIMKOD, HRCIHALEONAYTERTIP.KURKOD, HRCIHALEONAYTERTIP.FONKOD, HRCIHALEONAYTERTIP.FINKOD,  " +
                      " HRCIHALEONAYTERTIP.EKOKOD, HRCIHALEONAYTERTIP.PEBKOD, HRCSUREC.YILLARASARIISLEMMI, HRCSUREC.YIL " +
                    " ORDER BY ISLEMYIL DESC, ISLEMAY DESC, HRCTASINIRFORM.SURECNO, HRCTASINIRFORM.MUHASEBEKOD, HRCTASINIRFORM.BIRIMKOD,  " +
                      " HRCIHALEONAYTERTIP.KURKOD, HRCIHALEONAYTERTIP.FONKOD, HRCIHALEONAYTERTIP.FINKOD, HRCIHALEONAYTERTIP.EKOKOD, " +
                      " HRCIHALEONAYTERTIP.PEBKOD ";

            return veriErisim.DataReaderDondur(sql);
        }
    }
}
