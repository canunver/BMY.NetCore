using System;
using System.Data;
using TNS.Veri;

namespace TNS.MUH.IsKurallari.Veri
{
    /// <summary>
    /// MUHGONDERMETENKISFORM, MUHGONDERMETENKIS veri tabanı tablosu ile ilgili, listeleme ve yazma işlemleri tutan nesne
    /// </summary>
    public class TABGondermeTenkis : TABOrtak
    {
        /// <summary>
        /// Sınıfın oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABGondermeTenkis(VeriErisim veriErisim) : base(veriErisim) { }

        public static string TABLOADGONF = "MUHGONDERMETENKISFORM";
        public static string TABLOADGOND = "MUHGONDERMETENKIS";

        public enum ENUMAlanlarGONF
        {
            YIL = 0,
            BELGETUR = 1,
            BELGENO = 2,
            KURUMKOD = 3,
            MUHASEBEKOD = 4,
            BIRIMKOD = 5,
            ACIKLAMA = 6,
            ISLEMTARIH = 7,
            DURUMTARIH = 8,
            FORMDURUM = 9,
            IPTALNEDEN = 10,
            ICMALNO = 11,
            ICMALTARIH = 12,
            ONAYNO = 13,
            ONAYTARIH = 14,
            YAZITARIH = 15,
            YAZINO = 16,
            ISTEKYAZITARIH = 17,
            ISTEKYAZINO = 18,
            ISLEMYAPAN = 19,
            DORTDUZEYDEGISIM = 20,
            SURECYIL = 21,
            SURECNO = 22,
            IOIISKOD = 23,
        }
        public static string[] ALANLARGONF = { "YIL", "BELGETUR", "BELGENO", "KURUMKOD", "MUHASEBEKOD", "BIRIMKOD", "ACIKLAMA", "ISLEMTARIH", "DURUMTARIH", "FORMDURUM", "IPTALNEDEN", "ICMALNO", "ICMALTARIH", "ONAYNO", "ONAYTARIH", "YAZITARIH", "YAZINO", "ISTEKYAZITARIH", "ISTEKYAZINO", "ISLEMYAPAN", "DORTDUZEYDEGISIM", "SURECYIL", "SURECNO", "IOIISKOD" };

        public enum ENUMAlanlarGOND
        {
            YIL = 0,
            BELGETUR = 1,
            BELGENO = 2,
            KURKOD = 3,
            FONKOD = 4,
            FINKOD = 5,
            EKOKOD = 6,
            PEBKOD = 7,
            MIKTAR = 8,
            SIRA = 9,
        }
        public static string[] ALANLARGOND = { "YIL", "BELGETUR", "BELGENO", "KURKOD", "FONKOD", "FINKOD", "EKOKOD", "PEBKOD", "MIKTAR", "SIRA" };

        /// <summary>
        /// Parametre olarak gelen bilgileri veri tabanına ekleyen sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="form">Eklenecek kayıt bilgisini tutan nesne</param>
        /// <returns>Kaydet komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc Kaydet(GondermeTenkisForm form, string talepYapan, string talepNo, bool ekle)
        {
            if (form.belgeTur == ENUMBelgeTuru.ODENEKGONDERME || form.belgeTur == ENUMBelgeTuru.TENKIS)
                return KaydetGONTEN(form, talepYapan, talepNo, ekle);
            else
                return new Sonuc(false, "Kayıt edilmek istenilen belgenin türü belli değil.", "");
        }

        /// <summary>
        /// Verilen bilgilere ödenek gönderme / tenkis bilgilerini kayıt eder.
        /// </summary>
        /// <param name="form">Kayıt edilecek bilgileri taşıyan bilgi</param>
        /// <returns></returns>
        public Sonuc KaydetGONTEN(GondermeTenkisForm form, string talepYapan, string talepNo, bool ekle)
        {
            Sonuc sonuc = new Sonuc(false, "", "");
            string sql;
            if (ekle)
            {
                sql = "INSERT INTO " + TABLOADGONF + " (" + VeriErisim.TumAlanlar("", ALANLARGONF) + ") VALUES (";
                sql = sql + VeriErisim.DegerYap(form.yil) + ", ";
                sql = sql + VeriErisim.DegerYap((int)form.belgeTur) + ", ";
                sql = sql + VeriErisim.DegerYap(form.belgeNo) + ", ";
                sql = sql + VeriErisim.DegerYap(form.kurum.Replace(".", "")) + ", ";
                sql = sql + VeriErisim.DegerYap(form.muhasebe) + ", ";
                sql = sql + VeriErisim.DegerYap(form.birim) + ", ";
                sql = sql + VeriErisim.DegerYap(form.aciklama) + ", ";
                sql = sql + VeriErisim.DegerYap(form.islemTarih) + ", ";
                sql = sql + VeriErisim.DegerYap(new TNSDateTime(DateTime.Now)) + ", ";
                sql = sql + VeriErisim.DegerYap(form.formDurum) + ", ";
                sql = sql + VeriErisim.DegerYap(form.iptalNeden) + ", ";
                sql = sql + VeriErisim.DegerYap(form.icmalNo) + ", ";
                sql = sql + VeriErisim.DegerYap(form.icmalTarih) + ", ";
                sql = sql + VeriErisim.DegerYap(form.onayNo) + ", ";
                sql = sql + VeriErisim.DegerYap(form.onayTarih) + ", ";
                sql = sql + VeriErisim.DegerYap(form.yaziTarih) + ", ";
                sql = sql + VeriErisim.DegerYap(form.yaziNo) + ", ";
                sql = sql + VeriErisim.DegerYap(form.istekYaziTarih) + ", ";
                sql = sql + VeriErisim.DegerYap(form.istekYaziNo) + ", ";
                sql = sql + VeriErisim.DegerYap(form.islemYapan) + ",";
                sql = sql + VeriErisim.DegerYap(form.DortDuzeyDegisim) + ",";
                sql = sql + VeriErisim.DegerYap(form.surecYil) + ",";
                sql = sql + VeriErisim.DegerYap(form.surecNo) + ",";
                sql = sql + (form.isKodu == 0 ? "NULL" : VeriErisim.DegerYap(form.isKodu)) + ")";
                sonuc = KomutCalistir(sql, 1);
            }
            else
            {
                sql = "UPDATE " + TABLOADGONF + " SET ";
                sql = sql + ALANLARGONF[(int)ENUMAlanlarGONF.MUHASEBEKOD] + "=" + VeriErisim.DegerYap(form.muhasebe) + ", ";
                sql = sql + ALANLARGONF[(int)ENUMAlanlarGONF.BIRIMKOD] + "=" + VeriErisim.DegerYap(form.birim) + ", ";
                sql = sql + ALANLARGONF[(int)ENUMAlanlarGONF.ACIKLAMA] + "=" + VeriErisim.DegerYap(form.aciklama) + ", ";
                sql = sql + ALANLARGONF[(int)ENUMAlanlarGONF.ISLEMTARIH] + "=" + VeriErisim.DegerYap(form.islemTarih) + ", ";
                sql = sql + ALANLARGONF[(int)ENUMAlanlarGONF.DURUMTARIH] + "=" + VeriErisim.DegerYap(new TNSDateTime(DateTime.Now)) + ", ";
                sql = sql + ALANLARGONF[(int)ENUMAlanlarGONF.FORMDURUM] + "=" + VeriErisim.DegerYap(form.formDurum) + ", ";
                sql = sql + ALANLARGONF[(int)ENUMAlanlarGONF.YAZITARIH] + "=" + VeriErisim.DegerYap(form.yaziTarih) + ", ";
                sql = sql + ALANLARGONF[(int)ENUMAlanlarGONF.YAZINO] + "=" + VeriErisim.DegerYap(form.yaziNo) + ", ";
                sql = sql + ALANLARGONF[(int)ENUMAlanlarGONF.ISTEKYAZITARIH] + "=" + VeriErisim.DegerYap(form.istekYaziTarih) + ", ";
                sql = sql + ALANLARGONF[(int)ENUMAlanlarGONF.ISTEKYAZINO] + "=" + VeriErisim.DegerYap(form.istekYaziNo) + ", ";
                sql = sql + ALANLARGONF[(int)ENUMAlanlarGONF.ONAYNO] + "=" + VeriErisim.DegerYap(form.onayNo) + ", ";
                sql = sql + ALANLARGONF[(int)ENUMAlanlarGONF.ONAYTARIH] + "=" + VeriErisim.DegerYap(form.onayTarih) + ", ";
                sql = sql + ALANLARGONF[(int)ENUMAlanlarGONF.ISLEMYAPAN] + "=" + VeriErisim.DegerYap(form.islemYapan) + ", ";
                sql = sql + ALANLARGONF[(int)ENUMAlanlarGONF.DORTDUZEYDEGISIM] + "=" + VeriErisim.DegerYap(form.DortDuzeyDegisim) + ", ";
                sql = sql + ALANLARGONF[(int)ENUMAlanlarGONF.SURECYIL] + "=" + VeriErisim.DegerYap(form.surecYil) + ", ";
                sql = sql + ALANLARGONF[(int)ENUMAlanlarGONF.SURECNO] + "=" + VeriErisim.DegerYap(form.surecNo) + ", ";
                sql = sql + ALANLARGONF[(int)ENUMAlanlarGONF.IOIISKOD] + "=" + (form.isKodu == 0 ? "NULL" : VeriErisim.DegerYap(form.isKodu));
                sql = sql + " WHERE ";
                sql = sql + ALANLARGONF[(int)ENUMAlanlarGONF.YIL] + "=" + VeriErisim.DegerYap(form.yil) + " AND ";
                sql = sql + ALANLARGONF[(int)ENUMAlanlarGONF.BELGETUR] + "=" + VeriErisim.DegerYap((int)form.belgeTur) + " AND ";
                sql = sql + ALANLARGONF[(int)ENUMAlanlarGONF.BELGENO] + "=" + VeriErisim.DegerYap(form.belgeNo);

                sonuc = KomutCalistir(sql, 1);
            }

            if (sonuc.islemSonuc)
            {
                int siraNo = 0;
                foreach (GondermeTenkisDetay detay in form.detay.objeler)
                {
                    double miktar = detay.miktar;
                    if (form.belgeTur == ENUMBelgeTuru.TENKIS)
                        if (miktar > 0)
                            miktar = miktar * -1;

                    siraNo++;
                    string pebKod;
                    if (detay.tertip.kodlar.Length > 4)
                    {
                        pebKod = detay.tertip.kodlar[4];
                        if (string.IsNullOrEmpty(pebKod)) pebKod = "000000";
                    }
                    else
                        pebKod = "000000";

                    sql = "INSERT INTO " + TABLOADGOND + " (" + VeriErisim.TumAlanlar("", ALANLARGOND) + ") VALUES (";
                    sql = sql + VeriErisim.DegerYap(form.yil) + ", ";
                    sql = sql + VeriErisim.DegerYap((int)form.belgeTur) + ", ";
                    sql = sql + VeriErisim.DegerYap(form.belgeNo) + ", ";
                    sql = sql + VeriErisim.DegerYap(detay.tertip.kodlar[0].Replace(".", "")) + ", ";
                    sql = sql + VeriErisim.DegerYap(detay.tertip.kodlar[1].Replace(".", "")) + ", ";
                    sql = sql + VeriErisim.DegerYap(detay.tertip.kodlar[2].Replace(".", "")) + ", ";
                    sql = sql + VeriErisim.DegerYap(detay.tertip.kodlar[3].Replace(".", "")) + ", ";
                    sql = sql + VeriErisim.DegerYap(pebKod.Replace(".", "")) + ", ";
                    sql = sql + VeriErisim.DegerYap(miktar) + ", ";
                    sql = sql + VeriErisim.DegerYap(siraNo) + ") ";
                    sonuc = KomutCalistir(sql, 1);
                }
            }
            if (!string.IsNullOrEmpty(talepYapan))
                KaydetTarihce(form.yil, (int)form.belgeTur, form.belgeNo, talepYapan, "Talep çevrildi (" + talepNo + ")");
            string islem = TNS.MUH.Arac.FormDurumAciklama(form.formDurum, form.belgeTur);
            KaydetTarihce(form.yil, (int)form.belgeTur, form.belgeNo, form.islemYapan, islem);

            return sonuc;
        }

        /// <summary>
        /// Parametre olarak gelen anahtar alanları kullanarak, ilgili kaydı veri tabanından silen sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="yil">Yıl</param>
        /// <param name="belgeNo">Belge Numarası</param>
        /// <param name="belgeTur">Belge Türü</param>
        /// <returns>Sil komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc Sil(int yil, string belgeNo, int belgeTur)
        {
            Sonuc sonuc = new Sonuc(false, "", "");
            string sql = "";

            sql = "DELETE FROM " + TABLOADGOND;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARGOND[(int)ENUMAlanlarGOND.YIL], "=", yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARGOND[(int)ENUMAlanlarGOND.BELGETUR], "=", belgeTur));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARGOND[(int)ENUMAlanlarGOND.BELGENO], "=", belgeNo));
            sonuc = KomutCalistir(sql, 0);

            if (sonuc.islemSonuc)
            {
                sql = "DELETE FROM " + TABLOADGONF;
                sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARGONF[(int)ENUMAlanlarGONF.YIL], "=", yil));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARGONF[(int)ENUMAlanlarGONF.BELGETUR], "=", belgeTur));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARGONF[(int)ENUMAlanlarGONF.BELGENO], "=", belgeNo));
                sonuc = KomutCalistir(sql, 0);
            }

            return sonuc;
        }

        /// <summary>
        /// Parametre olarak gelen anahtar alanları kullanarak, ilgili kaydı veri tabanından silen sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="yil">Yıl</param>
        /// <param name="belgeNo">Belge Numarası</param>
        /// <param name="belgeTur">Belge Türü</param>
        /// <returns>Sil komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc SilDetay(int yil, string belgeNo, int belgeTur)
        {
            Sonuc sonuc = new Sonuc(false, "", "");
            string sql = "";

            sql = "DELETE FROM " + TABLOADGOND;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARGOND[(int)ENUMAlanlarGOND.YIL], "=", yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARGOND[(int)ENUMAlanlarGOND.BELGETUR], "=", belgeTur));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARGOND[(int)ENUMAlanlarGOND.BELGENO], "=", belgeNo));
            sonuc = KomutCalistir(sql, 0);

            return sonuc;
        }

        /// <summary>
        /// Ödenek önderme belgelerini listeyen sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcı</param>
        /// <param name="f">Kriter bilgileri</param>
        /// <param name="detay">Eğer <c>true</c> ise satır bilgilerinide listele.</param>
        /// <param name="siralama">Listelencek bilgilerin sıralama biçimini tarif eder.</param>
        /// <param name="ekoUzunluk">Ekonomik kodun uzunluğu.</param>
        /// <returns></returns>
        public IDataReader FormListe(KYM.Kullanici kullanan, GondermeTenkisForm f, int detay, int siralama, int ekoUzunluk, bool buyukKucukOlmasin, bool ekliDosyalar)
        {
            string ekliDosyaAlanlar, ekliDosyaYokAlanlar;
            if (ekliDosyalar)
            {
                ekliDosyaAlanlar = ", ek1.DOSYAADI, ek2.DOSYAADI ";
                ekliDosyaYokAlanlar = "";
            }
            else
            {
                ekliDosyaAlanlar = "";
                ekliDosyaYokAlanlar = ", null, null";
            }

            string groupByTumAlanlar;
            string groupByArkaAlanlar = "";
            string groupByOnAlanlar = VeriErisim.TumAlanlar(TABLOADGONF, ALANLARGONF) + ", KURAD, MUHASEBEAD, BIRIMAD, HRCIDAREBILGI.VNO, IOIP.IOIPROJEADI " + ekliDosyaAlanlar;
            groupByTumAlanlar = groupByOnAlanlar;
            string ekoAlan;
            if (detay > 0)
            {
                groupByArkaAlanlar = ", " + TABLOADGOND + "." + ALANLARGOND[(int)ENUMAlanlarGOND.KURKOD] + ", ";
                groupByArkaAlanlar += TABLOADGOND + "." + ALANLARGOND[(int)ENUMAlanlarGOND.FONKOD] + ", ";
                groupByArkaAlanlar += TABLOADGOND + "." + ALANLARGOND[(int)ENUMAlanlarGOND.FINKOD] + ", ";
                if (detay == 1)
                {
                    BKD.ButceKodYapi bky = BKD.Arac.Tanimla().ButceKodBilgiBul(f.yil, 4, string.Empty);
                    ekoAlan = TNS.BKD.Arac.Eko23AlanOl(f.yil, bky, TABLOADGOND + ".", ALANLARGOND[(int)ENUMAlanlarGOND.EKOKOD]);
                    groupByArkaAlanlar += ekoAlan;
                }
                else
                {
                    ekoAlan = TABLOADGOND + "." + ALANLARGOND[(int)ENUMAlanlarGOND.EKOKOD];
                    groupByArkaAlanlar += ekoAlan + ", ";
                    groupByArkaAlanlar += TABLOADGOND + "." + ALANLARGOND[(int)ENUMAlanlarGOND.PEBKOD] + "";
                }
                groupByTumAlanlar = groupByOnAlanlar + groupByArkaAlanlar;
            }
            else
                ekoAlan = "";
            string sql = "SELECT " + groupByOnAlanlar+ ekliDosyaYokAlanlar + groupByArkaAlanlar + ", SUM(" + ALANLARGOND[(int)ENUMAlanlarGOND.MIKTAR] + ")";
            sql += " FROM " + TABLOADGONF + " LEFT OUTER JOIN " + TABLOADGOND;
            sql = VeriErisim.KosulEkle(sql, "ON", TABLOADGONF + "." + ALANLARGONF[(int)ENUMAlanlarGONF.YIL] + "=" + TABLOADGOND + "." + ALANLARGOND[(int)ENUMAlanlarGOND.YIL]);
            sql = VeriErisim.KosulEkle(sql, "AND", TABLOADGONF + "." + ALANLARGONF[(int)ENUMAlanlarGONF.BELGENO] + "=" + TABLOADGOND + "." + ALANLARGOND[(int)ENUMAlanlarGOND.BELGENO]);
            sql = VeriErisim.KosulEkle(sql, "AND", TABLOADGONF + "." + ALANLARGONF[(int)ENUMAlanlarGONF.BELGETUR] + "=" + TABLOADGOND + "." + ALANLARGOND[(int)ENUMAlanlarGOND.BELGETUR]);
            sql += " LEFT OUTER JOIN ABSKUR";
            sql = VeriErisim.KosulEkle(sql, "ON", TABLOADGONF + "." + ALANLARGONF[(int)ENUMAlanlarGONF.YIL] + "=" + "ABSKUR.YIL");
            sql = VeriErisim.KosulEkle(sql, "AND", TABLOADGONF + "." + ALANLARGONF[(int)ENUMAlanlarGONF.KURUMKOD] + "=" + "ABSKUR.KURKOD");
            sql += " LEFT OUTER JOIN MUHMUHASEBE";
            sql = VeriErisim.KosulEkle(sql, "ON", TABLOADGONF + "." + ALANLARGONF[(int)ENUMAlanlarGONF.MUHASEBEKOD] + "=" + "MUHMUHASEBE.MUHASEBEKOD");
            sql += " LEFT OUTER JOIN MUHMUHASEBEBIRIM";
            sql = VeriErisim.KosulEkle(sql, "ON", TABLOADGONF + "." + ALANLARGONF[(int)ENUMAlanlarGONF.MUHASEBEKOD] + "=" + "MUHMUHASEBEBIRIM.MUHASEBEKOD");
            sql = VeriErisim.KosulEkle(sql, "AND", TABLOADGONF + "." + ALANLARGONF[(int)ENUMAlanlarGONF.BIRIMKOD] + "=" + "MUHMUHASEBEBIRIM.BIRIMKOD");
            sql = VeriErisim.KosulEkle(sql, "AND", TABLOADGOND + "." + ALANLARGOND[(int)ENUMAlanlarGOND.KURKOD] + "=" + "MUHMUHASEBEBIRIM.KURKOD");
            sql += " LEFT OUTER JOIN " + TABOrtak.DatabaseAdBul("HarcamaYonetimi.dbo.") + "HRCIDAREBILGI HRCIDAREBILGI";
            sql = VeriErisim.KosulEkle(sql, "ON", TABLOADGONF + "." + ALANLARGONF[(int)ENUMAlanlarGONF.MUHASEBEKOD] + "=" + "HRCIDAREBILGI.MUHASEBEKOD");
            sql = VeriErisim.KosulEkle(sql, "AND", TABLOADGONF + "." + ALANLARGONF[(int)ENUMAlanlarGONF.BIRIMKOD] + "=" + "HRCIDAREBILGI.BIRIMKOD");
            sql += " LEFT OUTER JOIN ButceHazirlikv2.dbo.BHMV2IOIPROJE IOIP ON IOIP.SIRANO = " + TABLOADGONF + ".IOIISKOD";

            if (ekliDosyalar)
            {
                sql += " LEFT OUTER JOIN Kullanici.dbo.KYMEKLIDOSYA ek1 ON ek1.MODUL = 'OGT' AND ek1.DOSYANO = 1 AND ek1.FORM = CONVERT(varchar, MUHGONDERMETENKISFORM.YIL) + '-' + CONVERT(varchar, MUHGONDERMETENKISFORM.BELGETUR) + '-' + MUHGONDERMETENKISFORM.BELGENO";
                sql += " LEFT OUTER JOIN Kullanici.dbo.KYMEKLIDOSYA ek2 ON ek2.MODUL = 'OGT' AND ek2.DOSYANO = 2 AND ek2.FORM = CONVERT(varchar, MUHGONDERMETENKISFORM.YIL) + '-' + CONVERT(varchar, MUHGONDERMETENKISFORM.BELGETUR) + '-' + MUHGONDERMETENKISFORM.BELGENO";
            }

            int anahtarSay = 0;
            if (f.yil > 0)
            {
                sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOADGONF, ALANLARGONF[(int)ENUMAlanlarGONF.YIL], "=", f.yil));
                anahtarSay++;
            }
            else
                sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOADGONF, ALANLARGONF[(int)ENUMAlanlarGONF.YIL], ">", 0));

            if ((int)f.belgeTur > -1)
            {
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADGONF, ALANLARGONF[(int)ENUMAlanlarGONF.BELGETUR], "=", (int)f.belgeTur));
                anahtarSay++;
            }

            if (f.belgeNo != "")
            {
                string[] t = f.belgeNo.Split(';');

                if (t.Length == 1 && t[0] != "")
                {
                    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADGONF, ALANLARGONF[(int)ENUMAlanlarGONF.BELGENO], "=", t[0]));
                    anahtarSay++;
                }
                else if (t.Length == 2 && !buyukKucukOlmasin)
                {
                    if (t[1] != "")
                    {
                        sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADGONF, ALANLARGONF[(int)ENUMAlanlarGONF.BELGENO], ">=", t[0]));
                        sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADGONF, ALANLARGONF[(int)ENUMAlanlarGONF.BELGENO], "<=", t[1]));
                    }
                    else
                    {
                        sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADGONF, ALANLARGONF[(int)ENUMAlanlarGONF.BELGENO], "=", t[0]));
                        anahtarSay++;
                    }
                }
                else
                {
                    string icSql = "";
                    foreach (string item in t)
                    {
                        icSql = VeriErisim.KosulEkle(icSql, "OR", VeriErisim.KosulOl(TABLOADGONF, ALANLARGONF[(int)ENUMAlanlarGONF.BELGENO], "=", item.Trim()));
                    }

                    sql = VeriErisim.KosulEkle(sql, "AND", "(" + icSql + ")");
                }
            }

            if (anahtarSay < 3)
            {
                if (f.formDurum > 0)
                {
                    if (f.formDurum == (int)TNS.MUH.ENUMBelgeDurumu.ISLEMDE)
                    {
                        sql += " AND " + VeriErisim.KosulOl(TABLOADGONF, ALANLARGONF[(int)ENUMAlanlarGONF.FORMDURUM], "<", (int)ENUMBelgeDurumu.ONAYLI);
                    }
                    else if (f.formDurum == (int)TNS.MUH.ENUMBelgeDurumu.DIGERSURECISLEMBELGELER)
                    {
                        sql += " AND (" + VeriErisim.KosulOl(TABLOADGONF, ALANLARGONF[(int)ENUMAlanlarGONF.FORMDURUM], "<", (int)ENUMBelgeDurumu.ONAYLI);
                        sql += " OR " + VeriErisim.KosulOl(TABLOADGONF, ALANLARGONF[(int)ENUMAlanlarGONF.FORMDURUM], "=", (int)ENUMBelgeDurumu.TALEP) + ")";
                    }
                    else if (f.formDurum == 1 || f.formDurum == 20 || f.formDurum == 21)
                    {
                        sql += " AND (" + VeriErisim.KosulOl(TABLOADGONF, ALANLARGONF[(int)ENUMAlanlarGONF.FORMDURUM], "=", (int)ENUMBelgeDurumu.YENI);
                        sql += " OR " + VeriErisim.KosulOl(TABLOADGONF, ALANLARGONF[(int)ENUMAlanlarGONF.FORMDURUM], "=", (int)ENUMBelgeDurumu.DEGISTIRILDI) + ")";
                    }
                    else if (f.formDurum == (int)ENUMBelgeDurumu.YENI_ONAYLITALEP)
                    {
                        sql += " AND (" + VeriErisim.KosulOl(TABLOADGONF, ALANLARGONF[(int)ENUMAlanlarGONF.FORMDURUM], "=", (int)ENUMBelgeDurumu.YENITALEP);
                        sql += " OR " + VeriErisim.KosulOl(TABLOADGONF, ALANLARGONF[(int)ENUMAlanlarGONF.FORMDURUM], "=", (int)ENUMBelgeDurumu.TALEP) + ")";
                    }
                    else if (f.formDurum == 22)
                        sql += " AND " + VeriErisim.KosulOl(TABLOADGONF, ALANLARGONF[(int)ENUMAlanlarGONF.FORMDURUM], "<", (int)ENUMBelgeDurumu.MUHASEBATIADE);
                    else
                        sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARGONF[(int)ENUMAlanlarGONF.FORMDURUM], "=", f.formDurum));

                    if (f.formDurum == 20)
                    {
                        sql += " AND (" + VeriErisim.KosulOl(TABLOADGONF, ALANLARGONF[(int)ENUMAlanlarGONF.ICMALNO], "=", 0);
                        sql += " OR " + TABLOADGONF + "." + ALANLARGONF[(int)ENUMAlanlarGONF.ICMALNO] + "=" + "null" + ")";
                    }

                    if (f.formDurum == 21)
                        sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARGONF[(int)ENUMAlanlarGONF.ICMALNO], ">", 0));
                }

                if (f.formDurum != 9)
                {
                    sql += " AND " + VeriErisim.KosulOl(TABLOADGONF, ALANLARGONF[(int)ENUMAlanlarGONF.FORMDURUM], "<>", (int)ENUMBelgeDurumu.IPTAL);
                    //if (f.formDurum != 99)
                    //    sql += " AND " + VeriErisim.KosulOl(TABLOADGONF, ALANLARGONF[(int)ENUMAlanlarGONF.FORMDURUM], "<>", (int)ENUMBelgeDurumu.TALEP);
                }

                if (f.icmalNo > 0)
                    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARGONF[(int)ENUMAlanlarGONF.ICMALNO], "=", f.icmalNo));

                if (f.kurum != "")
                    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADGONF, ALANLARGONF[(int)ENUMAlanlarGONF.KURUMKOD], "=", f.kurum.Replace(".", "")));

                if (f.sorguMuhasebeKod != "")
                {
                    string[] t = f.sorguMuhasebeKod.Split(';');

                    if (t.Length == 1 && t[0] != "")
                        sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADGONF, ALANLARGONF[(int)ENUMAlanlarGONF.MUHASEBEKOD], "=", OrtakFonksiyonlar.ConvertToInt(t[0], 0)));

                    if (t.Length == 2)
                    {
                        if (t[1] != "")
                        {
                            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADGONF, ALANLARGONF[(int)ENUMAlanlarGONF.MUHASEBEKOD], ">=", OrtakFonksiyonlar.ConvertToInt(t[0], 0)));
                            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADGONF, ALANLARGONF[(int)ENUMAlanlarGONF.MUHASEBEKOD], "<=", OrtakFonksiyonlar.ConvertToInt(t[1], 0)));
                        }
                        else
                            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADGONF, ALANLARGONF[(int)ENUMAlanlarGONF.MUHASEBEKOD], "=", OrtakFonksiyonlar.ConvertToInt(t[0], 0)));
                    }
                }

                if (f.muhasebe > 0)
                {
                    if (f.muhasebe % 1000 == 0)
                    {
                        int muh = f.muhasebe / 1000;
                        sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADGONF, ALANLARGONF[(int)ENUMAlanlarGONF.MUHASEBEKOD], ">=", muh * 1000));
                        sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADGONF, ALANLARGONF[(int)ENUMAlanlarGONF.MUHASEBEKOD], "<", (muh + 1) * 1000));
                    }
                    else
                        sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADGONF, ALANLARGONF[(int)ENUMAlanlarGONF.MUHASEBEKOD], "=", f.muhasebe));
                }

                if (f.birim != "")
                {
                    string[] t = f.birim.Split(';');

                    if (t.Length == 1 && t[0] != "")
                        sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADGONF, ALANLARGONF[(int)ENUMAlanlarGONF.BIRIMKOD], "=", t[0]));

                    if (t.Length == 2)
                    {
                        if (t[1] != "")
                        {
                            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADGONF, ALANLARGONF[(int)ENUMAlanlarGONF.BIRIMKOD], ">=", t[0]));
                            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADGONF, ALANLARGONF[(int)ENUMAlanlarGONF.BIRIMKOD], "<=", t[1]));
                        }
                        else
                            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADGONF, ALANLARGONF[(int)ENUMAlanlarGONF.BIRIMKOD], "=", t[0]));
                    }
                }
                if (f.DortDuzeyDegisim == 0)
                {
                    string icKosul = "(" + VeriErisim.KosulOl("", ALANLARGONF[(int)ENUMAlanlarGONF.DORTDUZEYDEGISIM], "=", 0) + " OR " + ALANLARGONF[(int)ENUMAlanlarGONF.DORTDUZEYDEGISIM] + " IS NULL)";
                    sql = VeriErisim.KosulEkle(sql, "AND", icKosul);
                }
                else
                    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARGONF[(int)ENUMAlanlarGONF.DORTDUZEYDEGISIM], "=", 1));

                if (f.surecYil > 0)
                    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARGONF[(int)ENUMAlanlarGONF.SURECYIL], "=", f.surecYil));

                if (!string.IsNullOrWhiteSpace(f.surecNo))
                {
                    string kos;
                    string esit = "=";
                    bool sifirla = false;
                    if (f.formDurum == (int)TNS.MUH.ENUMBelgeDurumu.DIGERSURECISLEMBELGELER)
                    {
                        kos = f.surecNo;
                        esit = "<>";
                        sifirla = true;
                    }
                    else
                    {
                        if (f.surecNo == "0" || f.surecNo.StartsWith("-"))
                        {
                            kos = "";
                            if (f.surecNo != "0")
                                esit = "<>";
                        }
                        else
                        {
                            kos = f.surecNo;
                            sifirla = true;
                        }
                    }
                    if (sifirla)
                    {
                        kos = OrtakFonksiyonlar.ConvertToInt(kos, 0).ToString("000000");
                    }
                    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADGONF, ALANLARGONF[(int)ENUMAlanlarGONF.SURECNO], esit, kos));
                }

                if (!string.IsNullOrEmpty(f.islemYapan))
                {
                    string kosul = "1 IN (SELECT 1 FROM MUHBELGETARIHCE M WHERE M.YIL=MUHGONDERMETENKISFORM.YIL AND M.BELGETUR=MUHGONDERMETENKISFORM.BELGETUR AND M.BELGENO=MUHGONDERMETENKISFORM.BELGENO AND M.ISLEMYAPAN=" + VeriErisim.DegerYap(f.islemYapan) + ")";
                    sql = VeriErisim.KosulEkle(sql, "AND", kosul);
                }

                if (!string.IsNullOrEmpty(f.aciklama))
                    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARGONF[(int)ENUMAlanlarGONF.ACIKLAMA], "LIKE", f.aciklama));

                if (!f.formDurumTarih.isNull)
                    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADGONF, ALANLARGONF[(int)ENUMAlanlarGONF.DURUMTARIH], ">=", f.formDurumTarih));

                if (!f.icmalTarih.isNull)
                    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADGONF, ALANLARGONF[(int)ENUMAlanlarGONF.ICMALTARIH], ">=", f.icmalTarih));

                if (!f.islemTarih.isNull)
                    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADGONF, ALANLARGONF[(int)ENUMAlanlarGONF.ISLEMTARIH], ">=", f.islemTarih));

                if (f.sorguIslemTarih != "")
                {
                    string[] t = f.sorguIslemTarih.Split(';');
                    System.DateTime d1;
                    System.DateTime d2;
                    if (t.Length == 1 && t[0] != "" || (t.Length == 2 && t[0] != "" && t[1] == ""))
                    {
                        d1 = Convert.ToDateTime(new TNSDateTime(t[0]).ToString());
                        d2 = Convert.ToDateTime(new TNSDateTime(t[0]).ToString()).AddDays(1).AddSeconds(-1);
                        sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADGONF, ALANLARGONF[(int)ENUMAlanlarGONF.ISLEMTARIH], ">=", new TNSDateTime(d1)));
                        sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADGONF, ALANLARGONF[(int)ENUMAlanlarGONF.ISLEMTARIH], "<=", new TNSDateTime(d2)));
                    }
                    else if (t.Length == 2 && t[0] != "" && t[1] != "")
                    {
                        d1 = Convert.ToDateTime(new TNSDateTime(t[0]).ToString());
                        d2 = Convert.ToDateTime(new TNSDateTime(t[1]).ToString()).AddDays(1).AddSeconds(-1);
                        sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADGONF, ALANLARGONF[(int)ENUMAlanlarGONF.ISLEMTARIH], ">=", new TNSDateTime(d1)));
                        sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADGONF, ALANLARGONF[(int)ENUMAlanlarGONF.ISLEMTARIH], "<=", new TNSDateTime(d2)));
                    }
                    else if (t.Length == 2 && t[0] == "" && t[1] != "")
                    {
                        d2 = Convert.ToDateTime(new TNSDateTime(t[1]).ToString()).AddDays(1).AddSeconds(-1);
                        sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADGONF, ALANLARGONF[(int)ENUMAlanlarGONF.ISLEMTARIH], "<=", new TNSDateTime(d2)));
                    }
                }

                if (f.sorguIcmalTarih != "")
                {
                    string[] t = f.sorguIcmalTarih.Split(';');
                    System.DateTime d1;
                    System.DateTime d2;
                    if (t.Length == 1 && t[0] != "" || (t.Length == 2 && t[0] != "" && t[1] == ""))
                    {
                        d1 = Convert.ToDateTime(new TNSDateTime(t[0]).ToString());
                        d2 = Convert.ToDateTime(new TNSDateTime(t[0]).ToString()).AddDays(1).AddSeconds(-1);
                        sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADGONF, ALANLARGONF[(int)ENUMAlanlarGONF.ICMALTARIH], ">=", new TNSDateTime(d1)));
                        sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADGONF, ALANLARGONF[(int)ENUMAlanlarGONF.ICMALTARIH], "<=", new TNSDateTime(d2)));
                    }
                    else if (t.Length == 2 && t[0] != "" && t[1] != "")
                    {
                        d1 = Convert.ToDateTime(new TNSDateTime(t[0]).ToString());
                        d2 = Convert.ToDateTime(new TNSDateTime(t[1]).ToString()).AddDays(1).AddSeconds(-1);
                        sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADGONF, ALANLARGONF[(int)ENUMAlanlarGONF.ICMALTARIH], ">=", new TNSDateTime(d1)));
                        sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADGONF, ALANLARGONF[(int)ENUMAlanlarGONF.ICMALTARIH], "<=", new TNSDateTime(d2)));
                    }
                    else if (t.Length == 2 && t[0] == "" && t[1] != "")
                    {
                        d2 = Convert.ToDateTime(new TNSDateTime(t[1]).ToString()).AddDays(1).AddSeconds(-1);
                        sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADGONF, ALANLARGONF[(int)ENUMAlanlarGONF.ICMALTARIH], "<=", new TNSDateTime(d2)));
                    }
                }

                if (f.sorguFormDurumTarih != "")
                {
                    string[] t = f.sorguFormDurumTarih.Split(';');
                    System.DateTime d1;
                    System.DateTime d2;
                    if (t.Length == 1 && t[0] != "" || (t.Length == 2 && t[0] != "" && t[1] == ""))
                    {
                        d1 = Convert.ToDateTime(new TNSDateTime(t[0]).ToString());
                        d2 = Convert.ToDateTime(new TNSDateTime(t[0]).ToString()).AddDays(1).AddSeconds(-1);
                        sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADGONF, ALANLARGONF[(int)ENUMAlanlarGONF.DURUMTARIH], ">=", new TNSDateTime(d1)));
                        sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADGONF, ALANLARGONF[(int)ENUMAlanlarGONF.DURUMTARIH], "<=", new TNSDateTime(d2)));
                    }
                    else if (t.Length == 2 && t[0] != "" && t[1] != "")
                    {
                        d1 = Convert.ToDateTime(new TNSDateTime(t[0]).ToString());
                        d2 = Convert.ToDateTime(new TNSDateTime(t[1]).ToString()).AddDays(1).AddSeconds(-1);
                        sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADGONF, ALANLARGONF[(int)ENUMAlanlarGONF.DURUMTARIH], ">=", new TNSDateTime(d1)));
                        sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADGONF, ALANLARGONF[(int)ENUMAlanlarGONF.DURUMTARIH], "<=", new TNSDateTime(d2)));
                    }
                    else if (t.Length == 2 && t[0] == "" && t[1] != "")
                    {
                        d2 = Convert.ToDateTime(new TNSDateTime(t[1]).ToString()).AddDays(1).AddSeconds(-1);
                        sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADGONF, ALANLARGONF[(int)ENUMAlanlarGONF.DURUMTARIH], "<=", new TNSDateTime(d2)));
                    }
                }

                foreach (GondermeTenkisDetay d in f.detay.objeler)
                {
                    string kur = Arac.DiziElemanAl(d.tertip.kodlar, 0, "").Replace(".", "");
                    string fon = Arac.DiziElemanAl(d.tertip.kodlar, 1, "").Replace(".", "");
                    string fin = Arac.DiziElemanAl(d.tertip.kodlar, 2, "").Replace(".", "");
                    string eko = Arac.DiziElemanAl(d.tertip.kodlar, 3, "").Replace(".", "");
                    string peb = Arac.DiziElemanAl(d.tertip.kodlar, 4, "").Replace(".", "");

                    if (kur != "")
                    {
                        string[] kurlar = kur.Split(';');
                        string kurKosul = "";
                        for (int i = 0; i < kurlar.Length; i++)
                            if (kurlar[i].Trim() != "")
                                kurKosul = VeriErisim.KosulEkle(kurKosul, "OR", VeriErisim.KosulOl(TABLOADGOND, ALANLARGOND[(int)ENUMAlanlarGOND.KURKOD], " LIKE ", kurlar[i].Replace(".", "") + "%"));

                        if (kurKosul != "")
                            sql += " AND (" + kurKosul + ")";
                    }
                    if (fon != "")
                        sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADGOND, ALANLARGOND[(int)ENUMAlanlarGOND.FONKOD], "=", fon));
                    if (fin != "")
                        sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADGOND, ALANLARGOND[(int)ENUMAlanlarGOND.FINKOD], "=", fin));
                    if (eko != "")
                        sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADGOND, ALANLARGOND[(int)ENUMAlanlarGOND.EKOKOD], "LIKE", eko.Replace(".", "") + "%"));
                    if (peb != "")
                        sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADGOND, ALANLARGOND[(int)ENUMAlanlarGOND.PEBKOD], "LIKE", peb.Replace(".", "") + "%"));
                }
            }
            sql += " GROUP BY " + groupByTumAlanlar;
            if (f.tutarBE > 0 || f.tutarKE > 0)
            {
                string havingKosul = "";
                if (f.tutarBE > 0)
                    havingKosul = VeriErisim.KosulEkle(havingKosul, "AND", VeriErisim.KosulOl("", "SUM(" + ALANLARGOND[(int)ENUMAlanlarGOND.MIKTAR] + ")", ">=", f.tutarBE));
                if (f.tutarKE > 0)
                    havingKosul = VeriErisim.KosulEkle(havingKosul, "AND", VeriErisim.KosulOl("", "SUM(" + ALANLARGOND[(int)ENUMAlanlarGOND.MIKTAR] + ")", "<=", f.tutarKE));
                sql += " HAVING " + havingKosul;
            }


            if (siralama == 0)
                sql += " ORDER BY " + TABLOADGONF + "." + ALANLARGONF[(int)ENUMAlanlarGONF.YIL] + "," +
                                    TABLOADGONF + "." + ALANLARGONF[(int)ENUMAlanlarGONF.BELGENO];
            else if (siralama == 1)
                sql += " ORDER BY " + TABLOADGONF + "." + ALANLARGONF[(int)ENUMAlanlarGONF.YIL] + "," +
                                    TABLOADGONF + "." + ALANLARGONF[(int)ENUMAlanlarGONF.BELGENO] + "," +
                                    TABLOADGONF + "." + ALANLARGONF[(int)ENUMAlanlarGONF.MUHASEBEKOD] + "," +
                                    TABLOADGONF + "." + ALANLARGONF[(int)ENUMAlanlarGONF.BIRIMKOD] + "," +
                                    TABLOADGOND + "." + ALANLARGOND[(int)ENUMAlanlarGOND.KURKOD] + "," +
                                    TABLOADGOND + "." + ALANLARGOND[(int)ENUMAlanlarGOND.FONKOD] + "," +
                                    TABLOADGOND + "." + ALANLARGOND[(int)ENUMAlanlarGOND.FINKOD] + "," +
                                    ekoAlan;
            else if (siralama == 2)
                sql += " ORDER BY " + TABLOADGONF + "." + ALANLARGONF[(int)ENUMAlanlarGONF.YIL] + "," +
                                    TABLOADGONF + "." + ALANLARGONF[(int)ENUMAlanlarGONF.MUHASEBEKOD] + "," +
                                    TABLOADGONF + "." + ALANLARGONF[(int)ENUMAlanlarGONF.BIRIMKOD] + "," +
                                    TABLOADGONF + "." + ALANLARGONF[(int)ENUMAlanlarGONF.BELGENO] + "," +
                                    TABLOADGOND + "." + ALANLARGOND[(int)ENUMAlanlarGOND.KURKOD] + "," +
                                    TABLOADGOND + "." + ALANLARGOND[(int)ENUMAlanlarGOND.FONKOD] + "," +
                                    TABLOADGOND + "." + ALANLARGOND[(int)ENUMAlanlarGOND.FINKOD] + "," +
                                    ekoAlan;
            else if (siralama == 3)
                sql += " ORDER BY " + TABLOADGONF + "." + ALANLARGONF[(int)ENUMAlanlarGONF.YIL] + "," +
                                    TABLOADGOND + "." + ALANLARGOND[(int)ENUMAlanlarGOND.KURKOD] + "," +
                                    TABLOADGOND + "." + ALANLARGOND[(int)ENUMAlanlarGOND.FONKOD] + "," +
                                    TABLOADGOND + "." + ALANLARGOND[(int)ENUMAlanlarGOND.FINKOD] + "," +
                                    ekoAlan + "," +
                                    TABLOADGONF + "." + ALANLARGONF[(int)ENUMAlanlarGONF.MUHASEBEKOD] + "," +
                                    TABLOADGONF + "." + ALANLARGONF[(int)ENUMAlanlarGONF.BIRIMKOD] + "," +
                                    TABLOADGONF + "." + ALANLARGONF[(int)ENUMAlanlarGONF.BELGENO];
            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Saymanlik/Birim bazında ödenek gönderme rakamlarını listeyen sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon.
        /// </summary>
        /// <param name="f">The f.</param>
        /// <returns></returns>
        public IDataReader SaymanlikBirimBelgeListe(GondermeTenkisSayBirim f)
        {
            string sql = "";

            sql = "SELECT YIL,KURKOD,FONKOD,FINKOD,EKOKOD,SUM(MIKTAR) from " + TABLOADGOND + "," + TABLOADGONF;

            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOADGONF, ALANLARGONF[(int)ENUMAlanlarGONF.YIL], "=", f.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADGONF, ALANLARGONF[(int)ENUMAlanlarGONF.BELGETUR], "=", (int)f.belgeTur));

            if (f.formDurum > 0)
                sql += " AND " + VeriErisim.KosulOl(TABLOADGONF, ALANLARGONF[(int)ENUMAlanlarGONF.FORMDURUM], "=", (int)ENUMBelgeDurumu.ONAYLI);

            if (f.kurum != "")
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADGONF, ALANLARGONF[(int)ENUMAlanlarGONF.KURUMKOD], "=", f.kurum.Replace(".", "")));

            foreach (GondermeTenkisDetay d in f.detay.objeler)
            {
                string kur = ((string)d.tertip.kodlar[0]).Replace(".", "");
                string fon = ((string)d.tertip.kodlar[1]).Replace(".", "");
                string fin = ((string)d.tertip.kodlar[2]).Replace(".", "");
                string eko = ((string)d.tertip.kodlar[3]).Replace(".", "");

                if (kur != "")
                {
                    string[] kurlar = kur.Split(';');
                    string kurKosul = "";
                    for (int i = 0; i < kurlar.Length; i++)
                        if (kurlar[i].Trim() != "")
                            kurKosul = VeriErisim.KosulEkle(kurKosul, "OR", VeriErisim.KosulOl(TABLOADGOND, ALANLARGOND[(int)ENUMAlanlarGOND.KURKOD], " LIKE ", kurlar[i].Replace(".", "") + "%"));

                    if (kurKosul != "")
                        sql += " AND (" + kurKosul + ")";
                }
                if (fon != "")
                    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADGOND, ALANLARGOND[(int)ENUMAlanlarGOND.FONKOD], "=", fon));
                if (fin != "")
                    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADGOND, ALANLARGOND[(int)ENUMAlanlarGOND.FINKOD], "=", fin));
                if (eko != "")
                    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADGOND, ALANLARGOND[(int)ENUMAlanlarGOND.EKOKOD], "=", eko));
            }

            foreach (string saybirim in f.saymanlikBirim.objeler)
            {
                if (saybirim != "")
                {
                    string[] saybirimler = saybirim.Split('-');
                    string saykosul = "";
                    string saymanlik = "";
                    string birim = "";
                    saymanlik = saybirimler[0].Trim();
                    birim = saybirimler[1].Trim();

                    saykosul = VeriErisim.KosulEkle(saykosul, "AND", VeriErisim.KosulOl(TABLOADGOND, ALANLARGOND[(int)ENUMAlanlarGONF.MUHASEBEKOD], "=", saymanlik));
                    saykosul = VeriErisim.KosulEkle(saykosul, "AND", VeriErisim.KosulOl(TABLOADGOND, ALANLARGOND[(int)ENUMAlanlarGONF.BIRIMKOD], "=", birim));
                    if (saykosul != "")
                        sql += " AND (" + saykosul + ")";
                }

            }

            sql = VeriErisim.KosulEkle(sql, "AND", TABLOADGONF + "." + ALANLARGONF[(int)ENUMAlanlarGONF.YIL] + "=" + TABLOADGOND + "." + ALANLARGOND[(int)ENUMAlanlarGOND.YIL]);
            sql = VeriErisim.KosulEkle(sql, "AND", TABLOADGONF + "." + ALANLARGONF[(int)ENUMAlanlarGONF.BELGETUR] + "=" + TABLOADGOND + "." + ALANLARGOND[(int)ENUMAlanlarGOND.BELGETUR]);
            sql = VeriErisim.KosulEkle(sql, "AND", TABLOADGONF + "." + ALANLARGONF[(int)ENUMAlanlarGONF.YIL] + "=" + "ABSKUR.YIL");
            sql = VeriErisim.KosulEkle(sql, "AND", TABLOADGONF + "." + ALANLARGONF[(int)ENUMAlanlarGONF.KURUMKOD] + "=" + "ABSKUR.KURKOD");
            sql = VeriErisim.KosulEkle(sql, "AND", TABLOADGONF + "." + ALANLARGONF[(int)ENUMAlanlarGONF.MUHASEBEKOD] + "=" + "MUHMUHASEBE.MUHASEBEKOD");

            sql += " GROUP BY YIL,KURKOD,FONKOD,FINKOD,EKOKOD";

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Parametre olarak verilen değerlere uygun belgenin satırlarını listeyen sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="yil">Yıl.</param>
        /// <param name="belgeNo">Belge no.</param>
        /// <param name="belgeTur">Belge tur.</param>
        /// <returns></returns>
        public IDataReader TekBelgeOkuDetay(int yil, string belgeNo, int belgeTur)
        {
            return TekBelgeOkuDetay(yil, belgeNo, belgeTur, null);
        }

        /// <summary>
        /// Parametre olarak verilen değerlere uygun belgenin satırlarını listeyen sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="yil">Yıl.</param>
        /// <param name="belgeNo">Belge no.</param>
        /// <param name="belgeTur">Belge tur.</param>
        /// <param name="tertip">Tertip.</param>
        /// <returns></returns>
        public IDataReader TekBelgeOkuDetay(int yil, string belgeNo, int belgeTur, string tertip)
        {
            string sql;

            sql = "SELECT " + VeriErisim.TumAlanlar(TABLOADGOND, ALANLARGOND) + " FROM " + TABLOADGOND;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARGOND[(int)ENUMAlanlarGOND.YIL], "=", yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARGOND[(int)ENUMAlanlarGOND.BELGENO], "=", belgeNo));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARGOND[(int)ENUMAlanlarGOND.BELGETUR], "=", belgeTur));

            if (!string.IsNullOrEmpty(tertip))
            {
                string[] tertipler = tertip.Replace(".", "").Split('-');

                if (!string.IsNullOrEmpty(tertipler[0]))
                    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARGOND[(int)ENUMAlanlarGOND.KURKOD], "=", tertipler[0]));
                if (!string.IsNullOrEmpty(tertipler[1]))
                    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARGOND[(int)ENUMAlanlarGOND.FONKOD], "=", tertipler[1]));
                if (!string.IsNullOrEmpty(tertipler[2]))
                    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARGOND[(int)ENUMAlanlarGOND.FINKOD], "=", tertipler[2]));
                if (!string.IsNullOrEmpty(tertipler[3]))
                    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARGOND[(int)ENUMAlanlarGOND.EKOKOD], "=", tertipler[3]));
                if (tertipler.Length > 4)
                {
                    if (!string.IsNullOrEmpty(tertipler[4]))
                        sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARGOND[(int)ENUMAlanlarGOND.PEBKOD], "=", tertipler[4]));
                }
            }

            sql += " ORDER BY " + ALANLARGOND[(int)ENUMAlanlarGOND.KURKOD] + "," + ALANLARGOND[(int)ENUMAlanlarGOND.FONKOD] + "," + ALANLARGOND[(int)ENUMAlanlarGOND.FINKOD] + "," + ALANLARGOND[(int)ENUMAlanlarGOND.EKOKOD];

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Parametre olarak verilen değerlere uygun belgenin durum değişikliğini yapan sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="form">Form bilgileri.</param>
        /// <returns></returns>
        public Sonuc BelgeDurumDegistir(GondermeTenkisForm form, string yeniBelgeNo = null, bool islemTarihDegistir = false)
        {
            Sonuc sonuc = new Sonuc(false, "", "");
            string sql = "";

            sql = "UPDATE " + TABLOADGONF;
            sql += " SET " + VeriErisim.KosulOl("", ALANLARGONF[(int)ENUMAlanlarGONF.FORMDURUM], "=", form.formDurum) + ",";
            sql += VeriErisim.KosulOl("", ALANLARGONF[(int)ENUMAlanlarGONF.DURUMTARIH], "=", new TNSDateTime(DateTime.Now)) + ",";
            if (islemTarihDegistir)
                sql += VeriErisim.KosulOl("", ALANLARGONF[(int)ENUMAlanlarGONF.ISLEMTARIH], "=", form.islemTarih) + ",";

            sql += VeriErisim.KosulOl("", ALANLARGONF[(int)ENUMAlanlarGONF.ONAYNO], "=", form.onayNo) + ",";
            sql += VeriErisim.KosulOl("", ALANLARGONF[(int)ENUMAlanlarGONF.ONAYTARIH], "=", form.onayTarih) + ",";
            sql += VeriErisim.KosulOl("", ALANLARGONF[(int)ENUMAlanlarGONF.ISLEMYAPAN], "=", form.islemYapan) + ",";
            sql += VeriErisim.KosulOl("", ALANLARGONF[(int)ENUMAlanlarGONF.IPTALNEDEN], "=", form.iptalNeden);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARGONF[(int)ENUMAlanlarGONF.YIL], "=", form.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARGONF[(int)ENUMAlanlarGONF.BELGENO], "=", form.belgeNo));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARGONF[(int)ENUMAlanlarGONF.BELGETUR], "=", (int)form.belgeTur));

            sonuc = KomutCalistir(sql, 1);

            if (sonuc.islemSonuc)
            {
                string islem = TNS.MUH.Arac.FormDurumAciklama(form.formDurum, form.belgeTur, yeniBelgeNo);
                KaydetTarihce(form.yil, (int)form.belgeTur, form.belgeNo, form.islemYapan, islem);
            }
            else
            {
                OrtakFonksiyonlar.HataDosyaYaz(DosyaAdBul(), form.yil + "/" + form.belgeNo + "/" + form.belgeTur.ToString() + " " + sonuc.hataStr, false);
                veriErisim.hata = "";
            }
            return sonuc;
        }

        public static string DosyaAdBul()
        {
            return "OdenekGondermeKontrol" + DateTime.Now.ToString("yyMMddHH") + ".txt";
        }

        /// <summary>
        /// Parametre olarak verilen bilgilere uygun en büyük icmal numarasını veren sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="yil">Yıl.</param>
        /// <param name="belgeTur">Belge tür.</param>
        /// <param name="kurum">Kurum.</param>
        /// <returns></returns>
        public int EnBuyukIcmalNo(int yil, int belgeTur, string kurum)
        {
            string sql = "";
            int icmalNo = 0;

            sql = "SELECT MAX(" + ALANLARGONF[(int)ENUMAlanlarGONF.ICMALNO] + ") FROM " + TABLOADGONF;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARGONF[(int)ENUMAlanlarGONF.YIL], "=", yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARGONF[(int)ENUMAlanlarGONF.BELGETUR], "=", belgeTur));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARGONF[(int)ENUMAlanlarGONF.KURUMKOD], "=", kurum.Replace(".", "")));

            IDataReader r = veriErisim.DataReaderDondur(sql);
            if (r.Read())
                icmalNo = r.GetInt32(0);

            r.Close();

            return (icmalNo + 1);
        }

        /// <summary>
        /// Sıradaki icmal numarasını veren sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="form">Form bilgileri.</param>
        /// <returns></returns>
        public Sonuc IcmalNoVer(GondermeTenkisForm form)
        {
            Sonuc sonuc = null;
            string sql = "UPDATE " + TABLOADGONF;
            sql += " SET " + VeriErisim.KosulOl("", ALANLARGONF[(int)ENUMAlanlarGONF.ICMALNO], "=", form.icmalNo) + ",";
            sql += VeriErisim.KosulOl("", ALANLARGONF[(int)ENUMAlanlarGONF.ICMALTARIH], "=", new TNSDateTime(DateTime.Now)) + ",";
            sql += VeriErisim.KosulOl("", ALANLARGONF[(int)ENUMAlanlarGONF.DURUMTARIH], "=", new TNSDateTime(DateTime.Now)) + ",";
            sql += VeriErisim.KosulOl("", ALANLARGONF[(int)ENUMAlanlarGONF.ISLEMYAPAN], "=", form.islemYapan);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARGONF[(int)ENUMAlanlarGONF.YIL], "=", form.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARGONF[(int)ENUMAlanlarGONF.BELGENO], "=", form.belgeNo));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARGONF[(int)ENUMAlanlarGONF.BELGETUR], "=", (int)form.belgeTur));

            sonuc = KomutCalistir(sql, 0);

            string islem = TNS.MUH.Arac.FormDurumAciklama(form.formDurum, form.belgeTur);
            KaydetTarihce(form.yil, (int)form.belgeTur, form.belgeNo, form.islemYapan, islem + "(" + form.icmalNo + ")");

            return sonuc;
        }

        /// <summary>
        /// Ödenek Gönderme/Tenkis rakamlarını veren sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon.
        /// </summary>
        /// <param name="kr">Kriter bilgileri</param>
        /// <param name="belgeTur">Belge tur.</param>
        /// <returns></returns>
        public IDataReader GondermeTenkisToplam(OdenekDurumKriter kr, int belgeTur)
        {
            string sql = "SELECT ";
            string alanlar = "";

            if (kr.detay == ENUMOdenekDurumDetay.MUHASEBE)
            {
                if (alanlar != "") alanlar += ",";
                alanlar += ALANLARGONF[(int)ENUMAlanlarGONF.MUHASEBEKOD];
            }
            else if (kr.detay == ENUMOdenekDurumDetay.BIRIM)
            {
                if (alanlar != "") alanlar += ",";
                alanlar += ALANLARGONF[(int)ENUMAlanlarGONF.MUHASEBEKOD];
                alanlar += "," + ALANLARGONF[(int)ENUMAlanlarGONF.BIRIMKOD];
            }
            else if (kr.detay == ENUMOdenekDurumDetay.EKO1BASAMAK)
            {
                if (alanlar != "") alanlar += ",";
                alanlar += ALANLARGONF[(int)ENUMAlanlarGONF.MUHASEBEKOD];
                alanlar += "," + ALANLARGONF[(int)ENUMAlanlarGONF.BIRIMKOD];
                alanlar += "," + VeriErisim.IslecBul("SUBSTRING") + "(" + ALANLARGOND[(int)ENUMAlanlarGOND.EKOKOD] + ", 1, 2)";
            }
            if (!kr.konsolide)
            {
                if (alanlar != "") alanlar += ",";
                alanlar += ALANLARGOND[(int)ENUMAlanlarGOND.KURKOD];
                alanlar += "," + ALANLARGOND[(int)ENUMAlanlarGOND.FONKOD];
                alanlar += "," + ALANLARGOND[(int)ENUMAlanlarGOND.FINKOD];

                alanlar += "," + BKD.Arac.EkoAlanOl(kr.yil, "", ALANLARGOND[(int)ENUMAlanlarGOND.EKOKOD], kr.ekoKodKirilim, kr.eko23, VeriErisim.IslecBul("SUBSTRING"));

                if (kr.faaliyetGoster)
                    alanlar += "," + ALANLARGOND[(int)ENUMAlanlarGOND.PEBKOD];
            }

            if (alanlar != "") alanlar += ",";
            alanlar += TABLOADGONF + "." + ALANLARGONF[(int)ENUMAlanlarGONF.BELGETUR] + ",";
            alanlar += ALANLARGONF[(int)ENUMAlanlarGONF.FORMDURUM];

            sql += alanlar;
            if (alanlar != "") sql += ",";

            sql += "SUM(" + ALANLARGOND[(int)ENUMAlanlarGOND.MIKTAR] + ")";

            sql += " FROM " + TABLOADGONF + "," + TABLOADGOND + " WHERE ";
            sql += VeriErisim.KosulOl(TABLOADGONF, ALANLARGONF[(int)ENUMAlanlarGONF.YIL], "=", kr.yil);

            if (kr.muhasebe > 0)
                sql += " AND " + VeriErisim.KosulOl(TABLOADGONF, ALANLARGONF[(int)ENUMAlanlarGONF.MUHASEBEKOD], "=", kr.muhasebe);

            if (kr.birim != "")
                sql += " AND " + VeriErisim.KosulOl(TABLOADGONF, ALANLARGONF[(int)ENUMAlanlarGONF.BIRIMKOD], "=", kr.birim);

            sql = TABOdenekHarcamaSorgu.TertipKosulOl(sql, kr.tertip, TABLOADGOND, ALANLARGOND[(int)ENUMAlanlarGOND.KURKOD], ALANLARGOND[(int)ENUMAlanlarGOND.FONKOD], ALANLARGOND[(int)ENUMAlanlarGOND.FINKOD], ALANLARGOND[(int)ENUMAlanlarGOND.EKOKOD], ALANLARGOND[(int)ENUMAlanlarGOND.PEBKOD]);

            if (belgeTur != (int)ENUMBelgeTuru.TANIMSIZ)
            {
                sql += " AND " + VeriErisim.KosulOl(TABLOADGONF, ALANLARGONF[(int)ENUMAlanlarGONF.BELGETUR], "=", belgeTur);
                sql += " AND " + VeriErisim.KosulOl(TABLOADGONF, ALANLARGONF[(int)ENUMAlanlarGONF.FORMDURUM], "=", (int)ENUMBelgeDurumu.ONAYLI);
            }
            if (kr.durum > 0)
            {
                sql += " AND " + VeriErisim.KosulOl(TABLOADGONF, ALANLARGONF[(int)ENUMAlanlarGONF.FORMDURUM], "=", (int)kr.durum);
            }
            sql += " AND " + VeriErisim.KosulOl(TABLOADGONF, ALANLARGONF[(int)ENUMAlanlarGONF.FORMDURUM], "<>", (int)ENUMBelgeDurumu.TALEPKARSILA);
            sql += " AND " + VeriErisim.KosulOl(TABLOADGONF, ALANLARGONF[(int)ENUMAlanlarGONF.FORMDURUM], "<>", (int)ENUMBelgeDurumu.IPTAL);
            sql += " AND " + VeriErisim.KosulOl(TABLOADGONF, ALANLARGONF[(int)ENUMAlanlarGONF.FORMDURUM], "<>", (int)ENUMBelgeDurumu.MUHASEBATIADE);
            sql += " AND " + VeriErisim.KosulOl(TABLOADGONF, ALANLARGONF[(int)ENUMAlanlarGONF.FORMDURUM], "<>", (int)ENUMBelgeDurumu.TALEP);
            sql += " AND " + VeriErisim.KosulOl(TABLOADGONF, ALANLARGONF[(int)ENUMAlanlarGONF.FORMDURUM], "<>", (int)ENUMBelgeDurumu.YENITALEP);
            sql = VeriErisim.KosulEkle(sql, "AND", TABLOADGONF + "." + ALANLARGONF[(int)ENUMAlanlarGONF.YIL] + "=" + TABLOADGOND + "." + ALANLARGOND[(int)ENUMAlanlarGOND.YIL]);
            sql = VeriErisim.KosulEkle(sql, "AND", TABLOADGONF + "." + ALANLARGONF[(int)ENUMAlanlarGONF.BELGENO] + "=" + TABLOADGOND + "." + ALANLARGOND[(int)ENUMAlanlarGOND.BELGENO]);
            sql = VeriErisim.KosulEkle(sql, "AND", TABLOADGONF + "." + ALANLARGONF[(int)ENUMAlanlarGONF.BELGETUR] + "=" + TABLOADGOND + "." + ALANLARGOND[(int)ENUMAlanlarGOND.BELGETUR]);

            if (alanlar != "")
            {
                sql += " GROUP BY " + alanlar;
                sql += " ORDER BY " + alanlar;
            }

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Ödenek Gönderme/Tenkis rakamlarını veren sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon.
        /// </summary>
        /// <param name="kr">Kriter bilgisi.</param>
        /// <returns></returns>
        public IDataReader GondermeTenkisToplam(OdenekDurumKriter kr)
        {
            return GondermeTenkisToplam(kr, (int)ENUMBelgeTuru.TANIMSIZ);
        }

        void KaydetTarihce(int yil, int belgeTur, string belgeNo, string islemYapan, string islem)
        {
            string sql = "";

            sql = "INSERT INTO MUHBELGETARIHCE (YIL,BELGETUR,BELGENO,ISLEMYAPAN,ISLEM,ISLEMTARIH) VALUES (";
            sql = sql + VeriErisim.DegerYap(yil) + ", ";
            sql = sql + VeriErisim.DegerYap(belgeTur) + ", ";
            sql = sql + VeriErisim.DegerYap(belgeNo) + ", ";
            sql = sql + VeriErisim.DegerYap(islemYapan) + ", ";
            sql = sql + VeriErisim.DegerYap(islem) + ", ";
            sql = sql + VeriErisim.DegerYap(new TNSDateTime(DateTime.Now)) + ")";

            KomutCalistir(sql, 1);
        }

        ///// <summary>
        ///// Hatalı kayıtların tespit edilebilmesi için tüm belgelerin listelenmesi sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon.
        ///// </summary>
        ///// <param name="yil">Yıl.</param>
        ///// <param name="kurum">Kurum.</param>
        ///// <returns></returns>
        //public IDataReader GondermeTenkisTumFormlar(int yil, string kurum)
        //{
        //    string sql = "";
        //    string kosul = "";
        //    sql = "SELECT " + VeriErisim.TumAlanlar(TABLOADGONF, ALANLARGONF) + " FROM " + TABLOADGONF + " where ";
        //    kosul = VeriErisim.KosulOl(TABLOADGONF, ALANLARGONF[(int)ENUMAlanlarGONF.YIL], "=", yil);
        //    if (kurum != "")
        //        kosul = VeriErisim.KosulEkle(kosul, "and", VeriErisim.KosulOl("", ALANLARGONF[(int)ENUMAlanlarGONF.KURUMKOD], "=", kurum));
        //    sql = sql + kosul + " order by 1,2,3";
        //    IDataReader r = veriErisim.DataReaderDondur(sql);
        //    return r;
        //}

        public static string OdenekGondermeSorgu(int yil, int muhasebe, string birim, int durumAralik, ENUMBelgeTuru belgeTur, bool faaliyetli, string kur, string fon, string fin, string eko)
        {
            kur = kur.Replace(".", "");
            fon = fon.Replace(".", "");
            fin = fin.Replace(".", "");
            eko = eko.Replace(".", "");
            string alanlar = TABLOADGONF + "." + ALANLARGONF[(int)ENUMAlanlarGONF.MUHASEBEKOD];
            alanlar += ", " + TABLOADGONF + "." + ALANLARGONF[(int)ENUMAlanlarGONF.BIRIMKOD];
            alanlar += ", " + TABLOADGOND + "." + ALANLARGOND[(int)ENUMAlanlarGOND.KURKOD];
            alanlar += ", " + TABLOADGOND + "." + ALANLARGOND[(int)ENUMAlanlarGOND.FONKOD];
            alanlar += ", " + TABLOADGOND + "." + ALANLARGOND[(int)ENUMAlanlarGOND.FINKOD];
            BKD.Tertip tUzBul = new BKD.Tertip(yil, BKD.ENUMTabloKod.TABPEBOdenek);
            int eko2Uz = tUzBul.KodUzunlukBul(3, 2, true);

            alanlar += ", " + VeriErisim.IslecBul("SUBSTRING") + "(" + TABLOADGOND + "." + ALANLARGOND[(int)ENUMAlanlarGOND.EKOKOD] + $", 1, {eko2Uz})";
            if (faaliyetli)
                alanlar += ", " + TABLOADGOND + "." + ALANLARGOND[(int)ENUMAlanlarGOND.PEBKOD];
            alanlar += ", MUHMUHASEBE.MUHASEBEAD, MUHMUHASEBEBIRIM.BIRIMAD";

            string kur2 = "";
            if (kur.Length == 2)
                kur2 = kur + "70";

            string sql = "SELECT " + alanlar + ", SUM(" + ALANLARGOND[(int)ENUMAlanlarGOND.MIKTAR] + ")";

            sql += " FROM " + TABLOADGONF + " INNER JOIN " + TABLOADGOND;
            sql = VeriErisim.KosulEkle(sql, "ON", TABLOADGONF + "." + ALANLARGONF[(int)ENUMAlanlarGONF.YIL] + "=" + TABLOADGOND + "." + ALANLARGOND[(int)ENUMAlanlarGOND.YIL]);
            sql = VeriErisim.KosulEkle(sql, "AND", TABLOADGONF + "." + ALANLARGONF[(int)ENUMAlanlarGONF.BELGENO] + "=" + TABLOADGOND + "." + ALANLARGOND[(int)ENUMAlanlarGOND.BELGENO]);
            sql = VeriErisim.KosulEkle(sql, "AND", TABLOADGONF + "." + ALANLARGONF[(int)ENUMAlanlarGONF.BELGETUR] + "=" + TABLOADGOND + "." + ALANLARGOND[(int)ENUMAlanlarGOND.BELGETUR]);
            sql += " INNER JOIN MUHMUHASEBE ON MUHMUHASEBE.MUHASEBEKOD = " + TABLOADGONF + "." + ALANLARGONF[(int)ENUMAlanlarGONF.MUHASEBEKOD];
            sql += " INNER JOIN MUHMUHASEBEBIRIM ON MUHMUHASEBEBIRIM.MUHASEBEKOD = " + TABLOADGONF + "." + ALANLARGONF[(int)ENUMAlanlarGONF.MUHASEBEKOD];
            sql = VeriErisim.KosulEkle(sql, "AND", "MUHMUHASEBEBIRIM.BIRIMKOD =" + TABLOADGONF + "." + ALANLARGONF[(int)ENUMAlanlarGONF.BIRIMKOD]);
            sql = VeriErisim.KosulEkle(sql, "AND", "MUHMUHASEBEBIRIM.KURKOD =" + TABLOADGOND + "." + ALANLARGOND[(int)ENUMAlanlarGOND.KURKOD]);

            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOADGONF, ALANLARGONF[(int)ENUMAlanlarGONF.YIL], "=", yil));

            if (muhasebe > 0)
                sql += " AND " + VeriErisim.KosulOl(TABLOADGONF, ALANLARGONF[(int)ENUMAlanlarGONF.MUHASEBEKOD], "=", muhasebe);

            if (birim != "")
                sql += " AND " + VeriErisim.KosulOl(TABLOADGONF, ALANLARGONF[(int)ENUMAlanlarGONF.BIRIMKOD], "=", birim);

            if (kur != "")
            {
                if (kur2 != "")
                    sql += " AND " + VeriErisim.KosulOl(TABLOADGOND, ALANLARGOND[(int)ENUMAlanlarGOND.KURKOD], " >= ", kur + "");
                else
                    sql += " AND " + VeriErisim.KosulOl(TABLOADGOND, ALANLARGOND[(int)ENUMAlanlarGOND.KURKOD], " LIKE ", kur + "%");

                if (kur2 != "")
                    sql += " AND " + VeriErisim.KosulOl(TABLOADGOND, ALANLARGOND[(int)ENUMAlanlarGOND.KURKOD], " < ", kur2);
            }

            if (fon != "")
                sql += " AND " + VeriErisim.KosulOl(TABLOADGOND, ALANLARGOND[(int)ENUMAlanlarGOND.FONKOD], " LIKE ", fon + "%");
            if (fin != "")
                sql += " AND " + VeriErisim.KosulOl(TABLOADGOND, ALANLARGOND[(int)ENUMAlanlarGOND.FINKOD], " LIKE ", fin + "%");
            if (eko != "")
                sql += " AND " + VeriErisim.KosulOl(TABLOADGOND, ALANLARGOND[(int)ENUMAlanlarGOND.EKOKOD], " LIKE ", eko + "%");
            if (belgeTur != ENUMBelgeTuru.TANIMSIZ)
            {
                sql += " AND " + VeriErisim.KosulOl(TABLOADGONF, ALANLARGONF[(int)ENUMAlanlarGONF.BELGETUR], "=", (int)belgeTur);
            }
            if (durumAralik == 1)
                sql += " AND " + VeriErisim.KosulOl(TABLOADGONF, ALANLARGONF[(int)ENUMAlanlarGONF.FORMDURUM], "<", (int)ENUMBelgeDurumu.ONAYLI);
            else if (durumAralik == 5)
                sql += " AND " + VeriErisim.KosulOl(TABLOADGONF, ALANLARGONF[(int)ENUMAlanlarGONF.FORMDURUM], "=", (int)ENUMBelgeDurumu.ONAYLI);
            else
                sql += " AND " + VeriErisim.KosulOl(TABLOADGONF, ALANLARGONF[(int)ENUMAlanlarGONF.FORMDURUM], "<=", (int)ENUMBelgeDurumu.ONAYLI);
            sql += " GROUP BY " + alanlar + " ORDER BY " + alanlar;
            return sql;
        }

        public Sonuc BIBagKopart(int revizeYil, int revizeNo)
        {
            string sql = $"UPDATE MPGMASRAFPLANIREVIZEFORM SET ODEGONYIL=0, ODEGONTUR=0, ODEGONNO='' WHERE YIL={revizeYil} AND REVIZENO={revizeNo}";
            return KomutCalistir(sql, 1);
        }

        public Sonuc BIIptalEt(int revizeYil, int revizeNo)
        {
            string sql = $"UPDATE MPGMASRAFPLANIREVIZEFORM SET FORMDURUM=9 WHERE YIL={revizeYil} AND REVIZENO={revizeNo}";
            return KomutCalistir(sql, 1);
        }
    }
}
