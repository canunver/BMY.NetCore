using System;
using System.Data;
using TNS.Veri;

namespace TNS.MUH.IsKurallari.Veri
{
    /// <summary>
    /// 
    /// </summary>
    public class TABFaturaTakip : TABOrtak
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TABFaturaTakip"/> class.
        /// </summary>
        /// <param name="veriErisim">The veri erisim.</param>
        public TABFaturaTakip(VeriErisim veriErisim) : base(veriErisim) { }

        public static string TABLOAD = "MUHFATURATAKIP";
        public static string[] ALANLAR = { "YIL", "BELGENO", "DURUM" };
        public enum ENUMAlanlar
        {
            YIL = 0,
            BELGENO = 1,
            DURUM = 2,
        }

        /// <summary>
        /// Kaydets the specified bilgi.
        /// </summary>
        /// <param name="form">The bilgi.</param>
        /// <returns></returns>
        public Sonuc Kaydet(FaturaTakip form) 
        {
            string sql = "INSERT INTO " + TABLOAD + "(" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";

            sql += VeriErisim.DegerYap(form.yil) + ", ";
            sql += VeriErisim.DegerYap(form.belgeNo) + ", ";
            sql += VeriErisim.DegerYap(form.takipDurum) + ") ";

            Sonuc sonuc = KomutCalistir(sql, 1);

            if (!sonuc.islemSonuc)
            {
                veriErisim.hata = "";

                sql = "UPDATE " + TABLOAD + " SET ";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.DURUM], form.takipDurum);
                sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", form.yil));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.BELGENO], "=", form.belgeNo));

                sonuc = KomutCalistir(sql, 1);
            }

            return sonuc;
        }

        /// <summary>
        /// Listeles the specified bilgi.
        /// </summary>
        /// <param name="form">The bilgi.</param>
        /// <returns></returns>
        public IDataReader Listele(FaturaTakip form)
        {
            string sql = "";
            string kosul = "";


            sql = @"SELECT MUHMIFFORM.YIL, MUHMIFFORM.BELGETUR, MUHMIFFORM.BELGENO, MUHMIFFORM.KURUMKOD, MUHMIFFORM.MUHASEBEKOD, MUHMIFFORM.BIRIMKOD, MUHMIFFORM.IADSOYAD, MUHMIFFORM.IKIMLIKNO
                    , MUHMIFFORM.ISUBEAD, MUHMIFFORM.IHESAPNO, MUHMIFFORM.IVERGIDAIRE, MUHMIFFORM.ACIKLAMA, MUHMIFFORM.ISLEMTARIH, MUHMIFFORM.DURUMTARIH, MUHMIFFORM.FORMDURUM
                    , MUHMIFFORM.IPTALNEDEN, MUHMIFFORM.YEVMIYETARIH, MUHMIFFORM.YEVMIYENO, MUHMIFFORM.ETARIH, MUHMIFFORM.ENO, MUHMIFFORM.EKIM, MUHMIFFORM.ATARIH, MUHMIFFORM.ANO
                    , MUHMIFFORM.ATUTAR, MUHMIFFORM.OTUR, MUHMIFFORM.OTARIH, MUHMIFFORM.ONO, MUHMIFFORM.OTUTAR, MUHMIFFORM.VERILEEMRINO, MUHMIFFORM.CEKEMRINO, MUHMIFFORM.ISLEMYAPAN
                    , MUHMIFFORM.PROJENO, MUHMIFFORM.IHALE, MUHMIFFORM.FISTUR, MUHMIFFORM.ILGILIBELGENO, MUHMIFFORM.SURECYIL, MUHMIFFORM.SURECNO, MUHMIFFORM.KURKOD, MUHMIFFORM.ORGANIZASYONKOD
                    , MUHMIFFORM.YEVMIYEACIKLAMA, MUHMIFFORM.DUZELTILENODEMEEMRINO, MUHMIFFORM.ISTKOD1, MUHMIFFORM.ISTKOD2
                    , KURAD, MUHASEBEAD, SUM(BORC), SUM(ALACAK)
                    , (SELECT count(*) FROM MUHBELGELER where YIL = MUHMIFFORM.YIL AND BELGENO = MUHMIFFORM.BELGENO) 
                    , MUHFATURATAKIP.DURUM BELGEDURUM
                    FROM MUHMIF INNER JOIN MUHMIFFORM ON MUHMIF.YIL = MUHMIFFORM.YIL AND MUHMIF.BELGENO = MUHMIFFORM.BELGENO 
                    INNER JOIN ABSKUR ON MUHMIFFORM.YIL = ABSKUR.YIL AND MUHMIFFORM.KURUMKOD = ABSKUR.KURKOD 
                    LEFT OUTER JOIN MUHMUHASEBE ON MUHMIFFORM.MUHASEBEKOD = MUHMUHASEBE.MUHASEBEKOD 
                    LEFT OUTER JOIN MUHFATURATAKIP ON MUHMIF.YIL = MUHFATURATAKIP.YIL AND MUHMIF.BELGENO = MUHFATURATAKIP.BELGENO
                  ";

            sql += " WHERE MUHMIFFORM.YIL = " + form.yil;

            if (form.formDurum > 0)
            {
                if (form.formDurum == 1 || form.formDurum == 20 || form.formDurum == 21)
                {
                    sql += " AND (" + VeriErisim.KosulOl("MUHMIFFORM", "FORMDURUM", " = ", (int)ENUMBelgeDurumu.YENI);
                    sql += " OR " + VeriErisim.KosulOl("MUHMIFFORM", "FORMDURUM", "=", (int)ENUMBelgeDurumu.DEGISTIRILDI) + ")";
                }
                else if (form.formDurum == 22)
                    sql += " AND " + VeriErisim.KosulOl("MUHMIFFORM", "FORMDURUM", "<", (int)ENUMBelgeDurumu.IPTAL);
                else if (form.formDurum == 23)
                {
                    sql += " AND " + VeriErisim.KosulOl("MUHMIFFORM", "FORMDURUM", "<>", (int)ENUMBelgeDurumu.IPTAL);
                    sql += " AND (" + VeriErisim.KosulOl("MUHMIFFORM", "YEVMIYENO", " = ", 0);
                    sql += " OR " + "MUHMIFFORM" + "." + "YEVMIYENO" + " = " + "null" + ")";
                }
                else if (form.formDurum == 24)
                {
                    sql += " AND (" + VeriErisim.KosulOl("MUHMIFFORM", "FORMDURUM", " = ", (int)ENUMBelgeDurumu.ONAYLI);
                    sql += " OR " + VeriErisim.KosulOl("MUHMIFFORM", "FORMDURUM", "=", (int)ENUMBelgeDurumu.YEVMIYE_OESGBYEGONDER) + ")";
                }
                else
                    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", "FORMDURUM", "=", form.formDurum));

                if (form.formDurum == 21)
                    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", "YEVMIYENO", ">", 0));
            }

            if (form.fisTur != null && form.fisTur.Length > 0)
            {
                sql = VeriErisim.KosulEkle(sql, "AND", TABMizanBilanco.SorguOl("FISTUR", form.fisTur));
            }

            if (form.muhasebe > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("MUHMIFFORM", "MUHASEBEKOD", "=", form.muhasebe));

            if (form.birim != "")
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("MUHMIFFORM", "BIRIMKOD", "=", form.birim));


            if (form.belgeNo != "")
            {
                string[] t = form.belgeNo.Split(';');

                if (t.Length == 1 && t[0] != "")
                    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("MUHMIFFORM", "BELGENO", " = ", t[0]));

                if (t.Length == 2)
                {
                    if (t[1] != "")
                    {
                        sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("MUHMIFFORM", "BELGENO", ">=", t[0]));
                        sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("MUHMIFFORM", "BELGENO", "<=", t[1]));
                    }
                    else
                        sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("MUHMIFFORM", "BELGENO", "=", t[0]));
                }
            }

            if (form.sorguYEVYevNo != "")
            {
                string[] t = form.sorguYEVYevNo.Split(';');

                if (t.Length == 1 && t[0] != "")
                    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("MUHMIFFORM", "YEVMIYENO", " = ", t[0]));

                if (t.Length == 2)
                {
                    if (t[1] != "")
                    {
                        sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("MUHMIFFORM", "YEVMIYENO", ">=", t[0]));
                        sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("MUHMIFFORM", "YEVMIYENO", "<=", t[1]));
                    }
                    else
                        sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("MUHMIFFORM", "YEVMIYENO", "=", t[0]));
                }
            }

            if (form.sorguIslemTarih != "")
            {
                string[] t = form.sorguIslemTarih.Split(';');
                YilKontrol(ref t, form.yil);

                System.DateTime d1;
                System.DateTime d2;
                if (t.Length == 1 && t[0] != "" || (t.Length == 2 && t[0] != "" && t[1] == ""))
                {
                    d1 = Convert.ToDateTime(new TNSDateTime(t[0]).ToString());
                    d2 = Convert.ToDateTime(new TNSDateTime(t[0]).ToString()).AddDays(1).AddSeconds(-1);
                    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("MUHMIFFORM", "ISLEMTARIH", " >= ", new TNSDateTime(d1)));
                    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("MUHMIFFORM", "ISLEMTARIH", "<=", new TNSDateTime(d2)));
                }
                else if (t.Length == 2 && t[0] != "" && t[1] != "")
                {
                    d1 = Convert.ToDateTime(new TNSDateTime(t[0]).ToString());
                    d2 = Convert.ToDateTime(new TNSDateTime(t[1]).ToString()).AddDays(1).AddSeconds(-1);
                    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("MUHMIFFORM", "ISLEMTARIH", ">=", new TNSDateTime(d1)));
                    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("MUHMIFFORM", "ISLEMTARIH", "<=", new TNSDateTime(d2)));
                }
                else if (t.Length == 2 && t[0] == "" && t[1] != "")
                {
                    d2 = Convert.ToDateTime(new TNSDateTime(t[1]).ToString()).AddDays(1).AddSeconds(-1);
                    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("MUHMIFFORM", "ISLEMTARIH", "<=", new TNSDateTime(d2)));
                }
            }

            if (form.detay.objeler.Count > 0)
            {
                OdemeEmriMIFDetay detay = (OdemeEmriMIFDetay)form.detay[0];

                string hesapKod = detay.hesapKod.Replace(".", "");
                string altHesapKod = detay.altHesapKod.Replace(".", "");

                if (hesapKod != "")
                    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("MUHMIF", "HESAPKOD", "LIKE", hesapKod + "%"));
                if (altHesapKod != "")
                    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("MUHMIF", "ALTHESAPKOD", "LIKE", altHesapKod + "%"));
            }

            if (form.takipDurum < 100)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("MUHFATURATAKIP", "DURUM", "=", form.takipDurum));


            sql += " GROUP BY MUHMIFFORM.YIL, MUHMIFFORM.BELGETUR, MUHMIFFORM.BELGENO, MUHMIFFORM.KURUMKOD, MUHMIFFORM.MUHASEBEKOD, MUHMIFFORM.BIRIMKOD, MUHMIFFORM.IADSOYAD, MUHMIFFORM.IKIMLIKNO, MUHMIFFORM.ISUBEAD, MUHMIFFORM.IHESAPNO, MUHMIFFORM.IVERGIDAIRE, MUHMIFFORM.ACIKLAMA, MUHMIFFORM.ISLEMTARIH, MUHMIFFORM.DURUMTARIH, MUHMIFFORM.FORMDURUM, MUHMIFFORM.IPTALNEDEN, MUHMIFFORM.YEVMIYETARIH, MUHMIFFORM.YEVMIYENO, MUHMIFFORM.ETARIH, MUHMIFFORM.ENO, MUHMIFFORM.EKIM, MUHMIFFORM.ATARIH, MUHMIFFORM.ANO, MUHMIFFORM.ATUTAR, MUHMIFFORM.OTUR, MUHMIFFORM.OTARIH, MUHMIFFORM.ONO, MUHMIFFORM.OTUTAR, MUHMIFFORM.VERILEEMRINO, MUHMIFFORM.CEKEMRINO, MUHMIFFORM.ISLEMYAPAN, MUHMIFFORM.PROJENO, MUHMIFFORM.IHALE, MUHMIFFORM.FISTUR, MUHMIFFORM.ILGILIBELGENO, MUHMIFFORM.SURECYIL, MUHMIFFORM.SURECNO, MUHMIFFORM.KURKOD, MUHMIFFORM.ORGANIZASYONKOD, MUHMIFFORM.YEVMIYEACIKLAMA, MUHMIFFORM.DUZELTILENODEMEEMRINO, MUHMIFFORM.ISTKOD1, MUHMIFFORM.ISTKOD2,KURAD,MUHASEBEAD , MUHFATURATAKIP.DURUM ";
            sql += " ORDER BY MUHMIFFORM.YIL, MUHMIFFORM.BELGENO ";


            return veriErisim.DataReaderDondur(sql);
        }

        private void YilKontrol(ref string[] t, int fyil)
        {
            for (int i = 0; i < t.Length; i++)
            {
                string[] parca = t[i].Split('-', '.', '/', ' ');
                if (parca.Length == 3 && parca[2] != fyil.ToString() && fyil > 0)
                {
                    t[i] = parca[0] + "." + parca[1] + "." + fyil;
                }

                //Sorgulama yapılan yıl artık yıl ve tarih 29.02 geldi ise, kesin hesap raporları bu tarihleri baz alarak 
                //önceki yıllardaki rakamlarını getirmeye çalışıyor. Bu durumda tarih 29.02.2015 gibi artık olmayan 
                //yılda şubat 29 tarihi çevrilmek isteniyor. Bundan dolayı tarihi 28.02 yapmak lazım
                TNSDateTime kontrolTarih = new TNSDateTime(t[i]);
                if (kontrolTarih.isNull)
                {
                    if (!(t[i] == null || t[i] == ""))
                    {
                        string[] parcaT = t[i].Split('-', '.', '/', ' ');
                        int yilT = OrtakFonksiyonlar.ConvertToInt(parca[2], 0);
                        int ayT = OrtakFonksiyonlar.ConvertToInt(parca[1], 0);
                        int gunT = OrtakFonksiyonlar.ConvertToInt(parca[0], 0);
                        if (gunT == 29 && ayT == 2)
                        {
                            if (!(yilT % 4 == 0 && yilT % 400 == 0))
                            {
                                t[i] = "28" + "." + parcaT[1] + "." + parcaT[2];
                            }
                        }
                    }
                }
            }
        }

    }
}
