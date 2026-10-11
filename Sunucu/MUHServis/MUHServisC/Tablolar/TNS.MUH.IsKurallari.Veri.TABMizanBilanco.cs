using System.Data;
using TNS.Veri;
using TNS.BKD;
using TNS.DEG;

namespace TNS.MUH.IsKurallari.Veri
{
    /// <summary>
    /// Mizan/Bilanço raporları için çeşitli veri tabanı tabloları ile ilgili, listeleme ve yazma işlemleri tutan nesne
    /// </summary>
    public class TABMizanBilanco : TABOrtak
    {
        /// <summary>
        /// Sınıfın oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABMizanBilanco(VeriErisim veriErisim) : base(veriErisim) { }

        /// <summary>
        /// Bilanço select sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon.
        /// </summary>
        /// <param name="yil">Yıl.</param>
        /// <param name="kurum">Kurum.</param>
        /// <param name="saymanlikKodu">Saymanlık kodu.</param>
        /// <param name="birim">Harcama birim kodu.</param>
        /// <param name="tur">Tür.</param>
        /// <param name="maskeUzunluk">Maske uzunluk.</param>
        /// <param name="fisTur">Fiş tür.</param>
        /// <param name="tarih1">The tarih1.</param>
        /// <param name="tarih2">The tarih2.</param>
        /// <returns></returns>
        public IDataReader BilancoBilgi(int yil, string kurum, int saymanlikKodu, string birim, int tur, int maskeUzunluk, int fisTur, TNSDateTime tarih1, TNSDateTime tarih2)
        {
            string tarihAlanAd;
            if (tur == 2 || tur == 3)
                tarihAlanAd = "ISLEMTARIH";
            else
                tarihAlanAd = "YEVMIYETARIH";
            string hesapNoAlanAd = VeriErisim.IslecBul("SUBSTRING") + "(S.HESAPKOD, 1, " + maskeUzunluk.ToString() + ")";
            string sql = "SELECT  H.GRUP, " + hesapNoAlanAd + ", H.ACIKLAMA, SUM(S.BORC), SUM(S.ALACAK)";
            sql += " FROM MUHMIFFORM F, MUHMIF S, MUHHESAPPLANI H";
            sql += " WHERE F.YIL=S.YIL AND F.BELGENO=S.BELGENO AND H.YIL=S.YIL AND F.FORMDURUM<>9 AND H.HESAPKOD=" + hesapNoAlanAd;

            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("F", "YIL", "=", yil));
            //sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("F", "KURUMKOD", "=", kurum.Replace(".", "")));
            if (saymanlikKodu > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("F", "MUHASEBEKOD", "=", saymanlikKodu.ToString()));
            if (birim != "")
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("F", "BIRIMKOD", "=", birim));
            if (tarih1 != null && !tarih1.isNull)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("F", tarihAlanAd, ">=", tarih1));
            if (tarih2 != null && !tarih2.isNull)
            {
                //Oracleda Date alanı saat tuttuğu için
                tarih2 = new TNSDateTime(tarih2.Oku().AddDays(1));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("F", tarihAlanAd, "<", tarih2));
            }
            if (tur == 1)
                sql = VeriErisim.KosulEkle(sql, "AND", "F.YEVMIYENO > 0");
            else if (tur == 2)
                sql = VeriErisim.KosulEkle(sql, "AND", "F.FORMDURUM=5");
            else
                sql = VeriErisim.KosulEkle(sql, "AND", "F.FORMDURUM<>9");

            if (fisTur > 0)//Açılış,kapanış
            {
                if (fisTur == 98)
                    sql = VeriErisim.KosulEkle(sql, "AND", "F.FISTUR<>1");
                else if (fisTur == 99)
                    sql = VeriErisim.KosulEkle(sql, "AND", "F.FISTUR<>2");
                else if (fisTur == 97)
                {
                    sql = VeriErisim.KosulEkle(sql, "AND", "F.FISTUR<>1");
                    sql = VeriErisim.KosulEkle(sql, "AND", "F.FISTUR<>2");
                }
                else
                    sql = VeriErisim.KosulEkle(sql, "AND", "F.FISTUR=" + fisTur);
            }

            sql += " GROUP BY H.GRUP, " + hesapNoAlanAd + ", H.ACIKLAMA";
            sql += " ORDER BY H.GRUP, " + hesapNoAlanAd;

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Mizan raporu için koşul sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon.
        /// </summary>
        /// <param name="tabloAdDetay">Tablo ad detay.</param>
        /// <param name="tabloAdAna">Tablo ad ana.</param>
        /// <param name="yil">Yıl.</param>
        /// <param name="kurum">Kurum.</param>
        /// <param name="saymanlikKodu">Saymanlık kodu.</param>
        /// <param name="birim">Harcama birim kodu.</param>
        /// <param name="tarih1">Başlangıç Tarih.</param>
        /// <param name="tarih2">Bitiş Tarih.</param>
        /// <param name="basHesap">Başlangıç hesap.</param>
        /// <param name="bitHesap">Bitiş hesap.</param>
        /// <param name="tur">1 Yevmiyeli, 2 Onaylı, 3 Hepsi</param>
        /// <param name="maskeUzunluk">Maske uzunluk.</param>
        /// <param name="fisTur">Fiş tür.</param>
        /// <param name="listeTur">The liste tur.</param>
        /// <returns></returns>
        public static string MizanKosulOl(string tabloAdDetay, string tabloAdAna, int yil, string kurum, int saymanlikKodu, string birim, TNSDateTime tarih1, TNSDateTime tarih2, string basHesap, string bitHesap, int tur, int maskeUzunluk, ENUMFISTUR[] fisTur, int listeTur, string kurKod, string fonKod, string altHesap, int borcAlacak, int yevmiyeNo1, int yevmiyeNo2)
        {
            bool detayBirim = false;
            if (TNS.MUH.Arac.DetayBirimVarMi())
                detayBirim = true;

            string tarihAlanAd, sql;
            if (tur == 2 || tur == 3)
                tarihAlanAd = "ISLEMTARIH";
            else
                tarihAlanAd = "YEVMIYETARIH";
            sql = "";
            //SQL sorgusu YIL=2023 koşulu olunca 28 sn sürüyor. Bu kaldırdığımızda sonuç hemen geliyor
            //sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(tabloAdAna, "YIL", "=", yil));
            //sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(tabloAdAna, "KURUMKOD", "=", kurum.Replace(".", "")));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(tabloAdAna, "FORMDURUM", "<>", 9));

            if (saymanlikKodu > 0 || birim != "")
            {
                if (detayBirim)
                {
                    if (saymanlikKodu > 0)
                        sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(tabloAdAna, "MUHASEBEKOD", "=", saymanlikKodu.ToString()));
                    if (birim != "")
                    {
                        BirimKosulOl(ref sql, birim, tabloAdDetay, "DETAYBIRIM");
                    }
                }
                else
                {
                    sql += " AND ((";
                    if (saymanlikKodu > 0)
                        sql = VeriErisim.KosulEkle(sql, "", VeriErisim.KosulOl(tabloAdAna, "MUHASEBEKOD", "=", saymanlikKodu.ToString()));
                    if (birim != "")
                        BirimKosulOl(ref sql, birim, tabloAdAna, "BIRIMKOD");

                    //sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(tabloAdAna, "", "=", birim));

                    sql += ") OR (";

                    if (saymanlikKodu > 0)
                        sql = VeriErisim.KosulEkle(sql, "", VeriErisim.KosulOl(tabloAdAna, "MUHASEBEKOD", "=", saymanlikKodu.ToString()));

                    if (birim != "")
                    {
                        sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(tabloAdAna, "BIRIMKOD", "=", "000"));
                        sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(tabloAdDetay, "KURKOD", "LIKE", kurKod.Replace(".", "") + "%"));
                    }
                    sql += "))";
                }
            }
            if (kurKod != "")
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(tabloAdDetay, "KURKOD", "LIKE", kurKod.Replace(".", "") + "%"));

            if (fonKod != "")
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(tabloAdDetay, "FONKOD", "LIKE", fonKod.Replace(".", "") + "%"));

            if (altHesap != "")
            {
                if (altHesap == "-999-")
                    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(tabloAdDetay, "ALTHESAPKOD", "<>", ""));
                else
                    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(tabloAdDetay, "ALTHESAPKOD", "=", altHesap));
            }

            if (tarih1 != null && !tarih1.isNull)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(tabloAdAna, tarihAlanAd, ">=", tarih1));
            if (tarih2 != null && !tarih2.isNull)
            {
                //Oracleda Date alanı saat tuttuğu için
                tarih2 = new TNSDateTime(tarih2.Oku().AddDays(1));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(tabloAdAna, tarihAlanAd, "<", tarih2));
            }
            if (basHesap != "")
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(tabloAdDetay, "HESAPKOD", ">=", basHesap.Replace(".", "")));

            if (bitHesap != "")
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(tabloAdDetay, "HESAPKOD", "<=", bitHesap.Replace(".", "") + "z"));

            if (tur == 1)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(tabloAdAna, "YEVMIYENO", ">", "0"));
            else if (tur == 2)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(tabloAdAna, "FORMDURUM", "=", "5"));
            else if (tur == 3)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(tabloAdAna, "FORMDURUM", "<>", "9"));

            if (fisTur != null && fisTur.Length > 0)//Açılış,kapanış
            {
                sql = VeriErisim.KosulEkle(sql, "AND", SorguOl("FISTUR", fisTur));
            }
            if (listeTur == 2)
            {
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("MUHHESAPPLANI", "DOVIZ", "<>", ""));
                sql = VeriErisim.KosulEkle(sql, "AND", "MUHHESAPPLANI.DOVIZ IS NOT NULL");
            }

            if (borcAlacak == 1)
            {
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(tabloAdDetay, "BORC", ">", 0));
            }
            else if (borcAlacak == 2)
            {
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(tabloAdDetay, "ALACAK", ">", 0));
            }

            if (yevmiyeNo1 > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(tabloAdAna, "YEVMIYENO", ">=", yevmiyeNo1));

            if (yevmiyeNo2 > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(tabloAdAna, "YEVMIYENO", "<=", yevmiyeNo2));

            return sql;
        }

        public static string SorguOl(string alanAd, ENUMFISTUR[] fisTur)
        {
            return alanAd + " IN (" + ListeOl(fisTur) + ")";
        }

        public static string ListeOl(ENUMFISTUR[] fisTur)
        {
            string tut = "";
            for (int i = 0; i < fisTur.Length; i++)
            {
                if (tut != "") tut += ",";
                tut += (int)fisTur[i];
            }
            return tut;
        }

        private static void BirimKosulOl(ref string sql, string birim, string tabloAd, string alanAd)
        {
            string[] birimler = birim.Split(';', ',');
            string ickosul = "";
            foreach (string b in birimler)
            {
                ickosul = VeriErisim.KosulEkle(ickosul, "OR", VeriErisim.KosulOl(tabloAd, alanAd, "=", b));
            }
            if (ickosul != "")
                sql = VeriErisim.KosulEkle(sql, "AND", "(" + ickosul + ")");
        }

        /// <summary>
        /// Mizan raporu için toplam rakamları bulan sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon..
        /// </summary>
        /// <param name="yil">Yıl.</param>
        /// <param name="kurum">Kurum.</param>
        /// <param name="saymanlikKodu">Saymanlık kodu.</param>
        /// <param name="birim">Harcama birim kodu.</param>
        /// <param name="tarih1">Başlangıç Tarih.</param>
        /// <param name="tarih2">Bitiş Tarih.</param>
        /// <param name="basHesap">Başlangıç hesap.</param>
        /// <param name="bitHesap">Bitiş hesap.</param>
        /// <param name="tur">Tür.</param>
        /// <param name="maskeUzunluk">Maske uzunluk.</param>
        /// <param name="borc">Borc.</param>
        /// <param name="alacak">Alacak.</param>
        /// <param name="fisTur">Fiş tür.</param>
        /// <param name="listeTur">2 ise doviz borç alacak olarak çalışır</param>
        /// <param name="kurKod">The kur kod.</param>
        /// <param name="fonKod">The fon kod.</param>
        /// <param name="altHesap">The alt hesap.</param>
        public void MizanToplamBul(int yil, string kurum, int saymanlikKodu, string birim, TNSDateTime tarih1, TNSDateTime tarih2, string basHesap, string bitHesap, int tur, int maskeUzunluk, out double borc, out double alacak, ENUMFISTUR[] fisTur, int listeTur, string kurKod, string fonKod, string altHesap, int yevmiyeNo1, int yevmiyeNo2)
        {
            borc = 0;
            alacak = 0;
            string borcAlan = "BORC";
            string alacakAlan = "ALACAK";
            if (listeTur == 2)
            {
                borcAlan = "DOVIZBORC";
                alacakAlan = "DOVIZALACAK";
            }

            if (tarih2.Gun > 1 && tarih2.Ay >= 1 && tarih2.Yil == yil)
            {
                string hesapNoAlanAd = VeriErisim.IslecBul("SUBSTRING") + "(MUHMIF.HESAPKOD, 1, " + maskeUzunluk.ToString() + ")";
                string sqlKosul = MizanKosulOl("MUHMIF", "MUHMIFFORM", yil, kurum, saymanlikKodu, birim, tarih1, tarih2, basHesap, bitHesap, tur, maskeUzunluk, fisTur, listeTur, kurKod, fonKod, altHesap, 0, yevmiyeNo1, yevmiyeNo2);
                string sql = "SELECT SUM(" + borcAlan + "), SUM(" + alacakAlan + ") FROM MUHMIF, MUHMIFFORM, MUHHESAPPLANI WHERE MUHMIF.YIL=MUHMIFFORM.YIL AND MUHMIF.BELGENO=MUHMIFFORM.BELGENO AND MUHMIFFORM.FORMDURUM<>9 AND MUHMIF.YIL=MUHHESAPPLANI.YIL AND " + hesapNoAlanAd + "=MUHHESAPPLANI.HESAPKOD AND " + sqlKosul;

                IDataReader dr = veriErisim.DataReaderDondur(sql);
                if (dr != null)
                {
                    if (dr.Read())
                    {
                        borc = VeriErisim.DegerAl(dr, 0, 0.0);
                        alacak = VeriErisim.DegerAl(dr, 1, 0.0);
                    }
                    dr.Close();
                }
            }
        }

        /// <summary>
        /// Mizan raporu için sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon..
        /// </summary>
        /// <param name="yil">Yıl.</param>
        /// <param name="kurum">Kurum.</param>
        /// <param name="saymanlikKodu">Saymanlık kodu.</param>
        /// <param name="birim">Harcama birim kodu.</param>
        /// <param name="tarih1">Başlangıç Tarih.</param>
        /// <param name="tarih2">Bitiş Tarih.</param>
        /// <param name="basHesap">Başlangıç hesap.</param>
        /// <param name="bitHesap">Bitiş hesap.</param>
        /// <param name="tur">Tür.</param>
        /// <param name="maskeUzunluk">Maske uzunluk.</param>
        /// <param name="fisTur">Fiş tür.</param>
        /// <param name="listeTur">The liste tur.</param>
        /// <param name="kurKod">The kur kod.</param>
        /// <param name="fonKod">The fon kod.</param>
        /// <param name="altHesap">The alt hesap.</param>
        /// <returns></returns>
        public IDataReader MizanHesapBul(int yil, string kurum, int saymanlikKodu, string birim, TNSDateTime tarih1, TNSDateTime tarih2, string basHesap, string bitHesap, int tur, int maskeUzunluk, ENUMFISTUR[] fisTur, int listeTur, string kurKod, string fonKod, string altHesap)
        {
            string borcAlan = "BORC";
            string alacakAlan = "ALACAK";
            if (listeTur == 2)
            {
                borcAlan = "DOVIZBORC";
                alacakAlan = "DOVIZALACAK";
            }

            string hesapNoAlanAd = VeriErisim.IslecBul("SUBSTRING") + "(MUHMIF.HESAPKOD, 1, " + maskeUzunluk.ToString() + ")";
            string altHesapAlanAd = "";
            string altHesapAlanAdGrp = "";
            if (altHesap != "")
            {
                altHesapAlanAd = ", MUHMIF.ALTHESAPKOD";
                altHesapAlanAd += ", (SELECT MIN(AD) FROM " + TABOrtak.DatabaseAdBul("HarcamaYonetimi.dbo.") + "HRCFIRMABILGISI where VERGINO=MUHMIF.ALTHESAPKOD)";
                altHesapAlanAd += ", (SELECT MIN(AD + ' ' + SOYAD) FROM " + TABOrtak.DatabaseAdBul("Personel.dbo.") + "PERPersonel where TCKIMLIKNO=MUHMIF.ALTHESAPKOD)";

                altHesapAlanAdGrp = ", MUHMIF.ALTHESAPKOD";
            }

            string sqlKosul = MizanKosulOl("MUHMIF", "MUHMIFFORM", yil, kurum, saymanlikKodu, birim, tarih1, tarih2, basHesap, bitHesap, tur, maskeUzunluk, fisTur, listeTur, kurKod, fonKod, altHesap, 0, 0, 0);
            string sql = "SELECT " + hesapNoAlanAd + ", MUHHESAPPLANI.ACIKLAMA, SUM(" + borcAlan + "), SUM(" + alacakAlan + ")" + altHesapAlanAd + " FROM MUHMIF, MUHMIFFORM, MUHHESAPPLANI WHERE MUHMIF.YIL=MUHMIFFORM.YIL AND MUHMIF.BELGENO=MUHMIFFORM.BELGENO AND MUHMIF.YIL=MUHHESAPPLANI.YIL AND MUHMIFFORM.FORMDURUM<>9 AND " + hesapNoAlanAd + "=MUHHESAPPLANI.HESAPKOD AND " + sqlKosul + " GROUP BY " + hesapNoAlanAd + altHesapAlanAdGrp + ", MUHHESAPPLANI.ACIKLAMA ORDER BY " + hesapNoAlanAd + altHesapAlanAdGrp;

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Mizan raporu için sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon..
        /// </summary>
        /// <param name="yil">Yıl.</param>
        /// <param name="kurum">Kurum.</param>
        /// <param name="saymanlikKodu">Saymanlık kodu.</param>
        /// <param name="birim">Harcama birim kodu.</param>
        /// <param name="tarih1">Başlangıç Tarih.</param>
        /// <param name="tarih2">Bitiş Tarih.</param>
        /// <param name="basHesap">Başlangıç hesap.</param>
        /// <param name="bitHesap">Bitiş hesap.</param>
        /// <param name="tur">Tür.</param>
        /// <param name="maskeUzunluk">Maske uzunluk.</param>
        /// <param name="fisTur">Fiş tür.</param>
        /// <param name="listeTur">The liste tur.</param>
        /// <returns></returns>
        public IDataReader MizanBirimHesapBul(int yil, string kurum, int saymanlikKodu, string birim, TNSDateTime tarih1, TNSDateTime tarih2, string basHesap, string bitHesap, int tur, int maskeUzunluk, ENUMFISTUR[] fisTur, int listeTur, string kurKod = "", string fonKod = "", string altHesap = "")
        {
            string hesapNoAlanAd = VeriErisim.IslecBul("SUBSTRING") + "(MUHMIF.HESAPKOD, 1, " + maskeUzunluk.ToString() + ")";
            string sqlKosul = MizanKosulOl("MUHMIF", "MUHMIFFORM", yil, kurum, saymanlikKodu, birim, tarih1, tarih2, basHesap, bitHesap, tur, maskeUzunluk, fisTur, listeTur, kurKod, fonKod, altHesap, 0, 0, 0);

            string birimKod = "BIRIMKOD";
            if (TNS.MUH.Arac.DetayBirimVarMi())
                birimKod = "DETAYBIRIM";

            string sql = "SELECT " + hesapNoAlanAd + ", MUHHESAPPLANI.ACIKLAMA, " + birimKod + ", SUM(BORC), SUM(ALACAK) FROM MUHMIF, MUHMIFFORM, MUHHESAPPLANI WHERE MUHMIF.YIL=MUHMIFFORM.YIL AND MUHMIF.BELGENO=MUHMIFFORM.BELGENO AND MUHMIF.YIL=MUHHESAPPLANI.YIL AND MUHMIFFORM.FORMDURUM<>9 AND " + hesapNoAlanAd + "=MUHHESAPPLANI.HESAPKOD AND " + sqlKosul + " GROUP BY " + hesapNoAlanAd + ", MUHHESAPPLANI.ACIKLAMA," + birimKod + " ORDER BY " + hesapNoAlanAd + "," + birimKod + "";
            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Mizan raporu için sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon..
        /// </summary>
        /// <param name="yil">Yıl.</param>
        /// <param name="kurum">Kurum.</param>
        /// <param name="saymanlikKodu">Saymanlık kodu.</param>
        /// <param name="birim">Harcama birim kodu.</param>
        /// <param name="tarih1">Başlangıç Tarih.</param>
        /// <param name="tarih2">Bitiş Tarih.</param>
        /// <param name="basHesap">Başlangıç hesap.</param>
        /// <param name="bitHesap">Bitiş hesap.</param>
        /// <param name="tur">Tür.</param>
        /// <param name="maskeUzunluk">Maske uzunluk.</param>
        /// <param name="fisTur">Fiş tür.</param>
        /// <param name="listeTur">The liste tur.</param>
        /// <returns></returns>
        public IDataReader MizanBirimTertipBul(int yil, string kurum, int saymanlikKodu, string birim, TNSDateTime tarih1, TNSDateTime tarih2, string basHesap, string bitHesap, int tur, int maskeUzunluk, ENUMFISTUR[] fisTur, int listeTur, string altHesap, bool gercekEkoKodVer, int yevmiyeNo1, int yevmiyeNo2)
        {
            string hesapNoAlanAd = VeriErisim.IslecBul("SUBSTRING") + "(MUHMIF.HESAPKOD, 1, " + maskeUzunluk.ToString() + ")";

            string ekoKodAlanAd = VeriErisim.IslecBul("SUBSTRING") + "(MUHMIF.HESAPKOD, 1, " + maskeUzunluk.ToString() + ")";
            if (gercekEkoKodVer)
                ekoKodAlanAd = "EKOKOD";

            string sqlKosul = MizanKosulOl("MUHMIF", "MUHMIFFORM", yil, kurum, saymanlikKodu, birim, tarih1, tarih2, basHesap, bitHesap, tur, maskeUzunluk, fisTur, listeTur, "", "", altHesap, 0, yevmiyeNo1, yevmiyeNo2);
            string sql = "SELECT " + hesapNoAlanAd + ", MUHMIF.KURKOD,FONKOD,FINKOD," + ekoKodAlanAd + ",PEBKOD, SUM(BORC), SUM(ALACAK),MUHHESAPPLANI.ACIKLAMA,DETAYBIRIM FROM MUHMIF, MUHMIFFORM, MUHHESAPPLANI WHERE MUHMIF.YIL=MUHMIFFORM.YIL AND MUHMIF.BELGENO=MUHMIFFORM.BELGENO AND MUHMIF.YIL=MUHHESAPPLANI.YIL AND MUHMIFFORM.FORMDURUM<>9 AND " + hesapNoAlanAd + "=MUHHESAPPLANI.HESAPKOD AND " + sqlKosul + " GROUP BY " + hesapNoAlanAd + ", MUHMIF.KURKOD,FONKOD,FINKOD," + ekoKodAlanAd + ",PEBKOD,MUHHESAPPLANI.ACIKLAMA,DETAYBIRIM" + " ORDER BY " + hesapNoAlanAd + ",MUHMIF.KURKOD,FONKOD,FINKOD,PEBKOD,MUHHESAPPLANI.ACIKLAMA";
            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// 830-905 hesaplarının kontrol edildiği rapor için sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon..
        /// </summary>
        /// <param name="yil">Yıl.</param>
        /// <param name="saymanlikKodu">Saymanlık kodu.</param>
        /// <param name="birim">Harcama birim kodu.</param>
        /// <returns></returns>
        public IDataReader Kontrol830905(int yil, int saymanlikKodu, string birim)
        {
            string sql = "";

            sql = "SELECT MUHMIF.YIL, MUHMIF.BELGETUR, MUHMIF.BELGENO, " + VeriErisim.IslecBul("SUBSTRING") + "(HESAPKOD, 1, 7), MUHMIF.KURKOD, FONKOD, FINKOD, " + VeriErisim.IslecBul("SUBSTRING") + "(EKOKOD, 1, 3), SUM(BORC), SUM(ALACAK)";
            sql += " FROM MUHMIFFORM,MUHMIF";
            sql += " WHERE MUHMIF.YIL=MUHMIFFORM.YIL AND MUHMIF.BELGENO=MUHMIFFORM.BELGENO AND FORMDURUM<>9";
            sql += " AND ((HESAPKOD LIKE '830%') OR (HESAPKOD LIKE '905%'))";
            if (saymanlikKodu > 0)
                sql += " AND MUHASEBEKOD=" + saymanlikKodu;
            if (birim != "")
                sql += " AND BIRIMKOD='" + birim + "'";

            sql += " GROUP BY MUHMIF.YIL, MUHMIF.BELGETUR, MUHMIF.BELGENO, " + VeriErisim.IslecBul("SUBSTRING") + "(HESAPKOD, 1, 7), MUHMIF.KURKOD, FONKOD, FINKOD, " + VeriErisim.IslecBul("SUBSTRING") + "(EKOKOD, 1, 3)";
            sql += " ORDER BY MUHMIF.YIL, MUHMIF.BELGETUR, MUHMIF.BELGENO, " + VeriErisim.IslecBul("SUBSTRING") + "(HESAPKOD, 1, 7), MUHMIF.KURKOD, FONKOD, FINKOD, " + VeriErisim.IslecBul("SUBSTRING") + "(EKOKOD, 1, 3)";

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Ödenek üstü rapor için sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon..
        /// </summary>
        /// <param name="yil">Yıl.</param>
        /// <param name="saymanlikKodu">Saymanlık kodu.</param>
        /// <param name="birim">Harcama birim kodu.</param>
        /// <returns></returns>
        public IDataReader OdenekUstu(int yil, int saymanlikKodu, string birim)
        {
            string sql = "";

            sql = "SELECT MUHMIF.YIL, MUHMIF.BELGETUR, MUHMIF.BELGENO, " + VeriErisim.IslecBul("SUBSTRING") + "(HESAPKOD, 1, 7), KURKOD, FONKOD, FINKOD, " + VeriErisim.IslecBul("SUBSTRING") + "(EKOKOD, 1, 3), SUM(BORC), SUM(ALACAK)";
            sql += " FROM MUHMIFFORM,MUHMIF";
            sql += " WHERE MUHMIF.YIL=MUHMIFFORM.YIL AND MUHMIF.BELGENO=MUHMIFFORM.BELGENO AND FORMDURUM<>9";
            sql += " AND ((HESAPKOD LIKE '830%') OR (HESAPKOD LIKE '900%'))";
            if (saymanlikKodu > 0)
                sql += " AND MUHASEBEKOD=" + saymanlikKodu;
            if (birim != "")
                sql += " AND BIRIMKOD='" + birim + "'";

            sql += " GROUP BY MUHMIF.YIL, MUHMIF.BELGETUR, MUHMIF.BELGENO, " + VeriErisim.IslecBul("SUBSTRING") + "(HESAPKOD, 1, 7), KURKOD, FONKOD, FINKOD, " + VeriErisim.IslecBul("SUBSTRING") + "(EKOKOD, 1, 3)";
            sql += " ORDER BY MUHMIF.YIL, MUHMIF.BELGETUR, MUHMIF.BELGENO, " + VeriErisim.IslecBul("SUBSTRING") + "(HESAPKOD, 1, 7), KURKOD, FONKOD, FINKOD, " + VeriErisim.IslecBul("SUBSTRING") + "(EKOKOD, 1, 3)";

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Hatalı kullanılan hesap kodların tespiti için sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon..
        /// </summary>
        /// <param name="yil">Yıl.</param>
        /// <returns></returns>
        public IDataReader UstHesapKodKullanilanlar(int yil)
        {
            string sql = "";

            sql = "SELECT MUHMIF.BELGENO, MUHMIF.HESAPKOD,MUHMIF.BORC, MUHMIF.ALACAK,MUHHESAPPLANI.ACIKLAMA";
            sql += " FROM MUHMIF LEFT JOIN";
            sql += " MUHHESAPPLANI ON MUHMIF.YIL = MUHHESAPPLANI.YIL AND MUHMIF.HESAPKOD = MUHHESAPPLANI.HESAPKOD";
            sql += " WHERE (MUHHESAPPLANI.DETAY > 0 OR MUHHESAPPLANI.DETAY IS NULL) AND";
            sql += " MUHMIF.YIL=" + yil;
            sql += " ORDER BY MUHMIF.HESAPKOD";

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Gelir Kesin tertip listele için sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon..
        /// </summary>
        /// <param name="yil">Yıl.</param>
        /// <returns></returns>
        public IDataReader KesinGelirButceTertipListele(int yil)
        {
            string sql = "";
            sql = "SELECT EKOKOD FROM ABSBUTCEGELIR WHERE YIL=" + yil;
            sql += "ORDER BY EKOKOD";
            return veriErisim.DataReaderDondur(sql);
        }

        public IDataReader AvansSorgula(AvansSorguKriter form)
        {
            string tarihAlanAd, altHesapAlanAd, tertipler;
            if (form.durum == 1 || form.durum == 2)
                tarihAlanAd = "MAX(ISLEMTARIH)";
            else
                tarihAlanAd = "MAX(YEVMIYETARIH)";
            if (form.altHesaptan)
                altHesapAlanAd = "S.ALTHESAPKOD";
            else
                altHesapAlanAd = "F.IKIMLIKNO";

            if (form.tertipli)
                tertipler = ", S.KURKOD, S.FONKOD, S.FINKOD, S.EKOKOD, S.PEBKOD";
            else
                tertipler = "";

            string hesapNoAlanAd = "S.HESAPKOD"; // VeriErisim.IslecBul("SUBSTRING") + "(S.HESAPKOD, 1, " + maskeUzunluk.ToString() + ")";
            string alanlar = altHesapAlanAd + ", " + hesapNoAlanAd + ", H.ACIKLAMA" + tertipler;
            string sql = "SELECT " + alanlar + "," + tarihAlanAd + ", SUM(S.BORC), SUM(S.ALACAK)";
            sql += " FROM MUHMIFFORM F, MUHMIF S, MUHHESAPPLANI H";
            sql += " WHERE F.YIL=S.YIL AND F.BELGENO=S.BELGENO AND H.YIL=S.YIL AND F.FORMDURUM<>9 AND H.HESAPKOD=" + hesapNoAlanAd;

            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("F", "YIL", "=", form.yil));
            //sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("F", "KURUMKOD", "=", form.kurum.Replace(".", "")));
            if (form.saymanlikKodu > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("F", "MUHASEBEKOD", "=", form.saymanlikKodu));

            if (!string.IsNullOrWhiteSpace(form.birim))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("F", "BIRIMKOD", "=", form.birim));

            if (!string.IsNullOrWhiteSpace(form.hesapKod))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("S", "HESAPKOD", "LIKE", form.hesapKod.Replace(".", "") + "%"));

            if (!string.IsNullOrWhiteSpace(form.tcVno))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", altHesapAlanAd, "=", form.tcVno));

            if (form.durum == 3)
                sql = VeriErisim.KosulEkle(sql, "AND", "F.YEVMIYENO > 0");
            else if (form.durum == 2)
                sql = VeriErisim.KosulEkle(sql, "AND", "F.FORMDURUM = 5");

            //if (fisTur > 0)//Açılış,kapanış
            //{
            //    if (fisTur == 98)
            //        sql = VeriErisim.KosulEkle(sql, "AND", "F.FISTUR<>1");
            //    else if (fisTur == 99)
            //        sql = VeriErisim.KosulEkle(sql, "AND", "F.FISTUR<>2");
            //    else if (fisTur == 97)
            //    {
            //        sql = VeriErisim.KosulEkle(sql, "AND", "F.FISTUR<>1");
            //        sql = VeriErisim.KosulEkle(sql, "AND", "F.FISTUR<>2");
            //    }
            //    else
            //        sql = VeriErisim.KosulEkle(sql, "AND", "F.FISTUR=" + fisTur);
            //}
            sql += " GROUP BY " + alanlar;
            if (form.farklilar)
                sql += " HAVING SUM(S.BORC) <> SUM(S.ALACAK)";
            sql += " ORDER BY 1";

            return veriErisim.DataReaderDondur(sql);
        }

        public IDataReader AvansSorgulaDetay(AvansSorguKriter form)
        {
            string tarihAlanAd, altHesapAlanAd, tertipler;
            if (form.durum == 1 || form.durum == 2)
                tarihAlanAd = "ISLEMTARIH";
            else
                tarihAlanAd = "YEVMIYETARIH";
            if (form.altHesaptan)
                altHesapAlanAd = "S.ALTHESAPKOD";
            else
                altHesapAlanAd = "F.IKIMLIKNO";

            if (form.tertipli)
                tertipler = ", S.KURKOD, S.FONKOD, S.FINKOD, S.EKOKOD, S.PEBKOD";
            else
                tertipler = "";

            string sql = "SELECT F.BELGENO, " + tarihAlanAd + tertipler + ", F.ACIKLAMA, S.BORC, S.ALACAK";
            sql += " FROM MUHMIFFORM F, MUHMIF S";
            sql += " WHERE F.YIL=S.YIL AND F.BELGENO=S.BELGENO AND F.FORMDURUM<>9";

            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("F", "YIL", "=", form.yil));
            //sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("F", "KURUMKOD", "=", form.kurum.Replace(".", "")));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("S", "HESAPKOD", "LIKE", form.hesapKod.Replace(".", "") + "%"));
            if (string.IsNullOrEmpty(form.tcVno))
                sql += " AND (" + altHesapAlanAd + "='' OR " + altHesapAlanAd + " IS NULL)";
            else
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", altHesapAlanAd, "=", form.tcVno));

            if (form.tertipli)
            {
                string[] tertipParcalar = form.tertip.Split('-');
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("S", "KURKOD", "=", tertipParcalar[0].Replace(".", "")));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("S", "FONKOD", "=", tertipParcalar[1].Replace(".", "")));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("S", "FINKOD", "=", tertipParcalar[2].Replace(".", "")));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("S", "EKOKOD", "=", tertipParcalar[3].Replace(".", "")));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("S", "PEBKOD", "=", tertipParcalar[4].Replace(".", "")));
            }
            if (form.saymanlikKodu > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("F", "MUHASEBEKOD", "=", form.saymanlikKodu));
            if (!string.IsNullOrWhiteSpace(form.birim))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("F", "BIRIMKOD", "=", form.birim));
            if (form.durum == 3)
                sql = VeriErisim.KosulEkle(sql, "AND", "F.YEVMIYENO > 0");
            else if (form.durum == 2)
                sql = VeriErisim.KosulEkle(sql, "AND", "F.FORMDURUM = 5");

            //if (fisTur > 0)//Açılış,kapanış
            //{
            //    if (fisTur == 98)
            //        sql = VeriErisim.KosulEkle(sql, "AND", "F.FISTUR<>1");
            //    else if (fisTur == 99)
            //        sql = VeriErisim.KosulEkle(sql, "AND", "F.FISTUR<>2");
            //    else if (fisTur == 97)
            //    {
            //        sql = VeriErisim.KosulEkle(sql, "AND", "F.FISTUR<>1");
            //        sql = VeriErisim.KosulEkle(sql, "AND", "F.FISTUR<>2");
            //    }
            //    else
            //        sql = VeriErisim.KosulEkle(sql, "AND", "F.FISTUR=" + fisTur);
            //}
            sql += " ORDER BY 1";

            return veriErisim.DataReaderDondur(sql);
        }
    }
}
