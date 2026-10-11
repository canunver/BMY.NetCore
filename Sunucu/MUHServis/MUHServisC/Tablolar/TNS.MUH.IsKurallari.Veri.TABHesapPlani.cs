using System;
using System.Data;
using TNS.BKD;
using TNS.Veri;

namespace TNS.MUH.IsKurallari.Veri
{
    /// <summary>
    /// MUHHESAPPLANI, MUHYANSITMA veri tabanı tablosu ile ilgili, listeleme ve yazma işlemleri tutan nesne
    /// </summary>
    public class TABHesapPlani : TABOrtak
    {
        /// <summary>
        /// Sınıfın oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABHesapPlani(VeriErisim veriErisim) : base(veriErisim) { }

        static string TABLOAD = "MUHHESAPPLANI";
        static string TABLOADYANSITMA = "MUHYANSITMA";

        enum ENUMAlanlar
        {
            YIL = 0,
            HESAPKOD = 1,
            ACIKLAMA = 2,
            SEVIYE = 3,
            KORUMA = 4,
            BAKIYE = 5,
            TERTIP = 6,
            KESINTI = 7,
            NITELIK = 8,
            GRUP = 9,
            DETAY = 10,
            ALTHESAPKOD = 11,
            DOVIZ = 12,
            DOVIZKURTURSATIS = 13,
            DOVIZKURTURALIS = 14,
            KURFARKIALACAKHESABI = 15,
            KURFARKIBORCHESABI = 16,
            EKOBASLA = 17,
        }
        static string[] ALANLAR = { "YIL", "HESAPKOD", "ACIKLAMA", "SEVIYE", "KORUMA", "BAKIYE", "TERTIP", "KESINTI", "NITELIK", "GRUP", "DETAY", "ALTHESAPKOD", "DOVIZ", "DOVIZKURTURSATIS", "DOVIZKURTURALIS", "KURFARKIALACAKHESABI", "KURFARKIBORCHESABI", "EKOBASLA" };
        enum ENUMAlanlarYansitma
        {
            YIL = 0,
            HKOD = 1,
            YKOD = 2,
            SIRANO = 3,
        }
        static string[] ALANLARYANSITMA = { "YIL", "HKOD", "YKOD", "SIRANO" };

        /// <summary>
        /// Parametre olarak gelen bilgileri veri tabanına ekleyen sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="hesapPlani">Kayıt edilecek bilgiler.</param>
        /// <param name="kontrolYok">Eğer <c>true</c> ise kontrol işlemi yapmadan kaydet.</param>
        /// <returns></returns>
        public Sonuc HesapKodKaydet(ObjectArray hesapPlani, bool kontrolYok)
        {
            Sonuc sonuc = new Sonuc(false, "", "");
            Tertip t = new Tertip(DateTime.Now.Year, ENUMTabloKod.TABOdemeEmri);

            foreach (HesapPlaniSatir hs in hesapPlani.objeler)
            {
                string kontrolKod = "";

                if (hs.seviye > 1)
                {
                    hs.hesapKod = hs.hesapKod.Replace(".", "");
                    hs.hesapKod = OrtakFonksiyonlar.KodNoktaKoy((string)t.Kirilimlar(0), hs.hesapKod);
                    string[] st = hs.hesapKod.Split('.');
                    int seviye = st.Length;
                    for (int i = 0; i < seviye - 1; i++)
                    {
                        if (kontrolKod != "") kontrolKod += ".";
                        kontrolKod += st[i];
                    }

                    if (!kontrolYok && !HesapKodAnasiCocuguVarMi(hs.yil, kontrolKod, false))
                    {
                        sonuc.hataStr = hs.yil + " yılına ait " + hs.hesapKod + " nolu hesap kodunun kayıt edilebilmesi için " + kontrolKod + " hesap kodunun olması gerekmektedir. Lütfen ilk önce bu kaydı yapın.";
                        return sonuc;
                    }
                }

                string sql = "";
                if (hs.nitelik.Trim().StartsWith(";"))
                    hs.nitelik = hs.nitelik.Trim().Substring(1, hs.nitelik.Trim().Length - 1);
                if (hs.nitelik.Trim().EndsWith(";"))
                    hs.nitelik = hs.nitelik.Trim().Substring(0, hs.nitelik.Trim().Length - 1);
                hs.nitelik = ";" + hs.nitelik + ";";

                sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
                sql = sql + VeriErisim.DegerYap(hs.yil) + ", ";
                sql = sql + VeriErisim.DegerYap(hs.hesapKod.Replace(".", "")) + ", ";
                sql = sql + VeriErisim.DegerYap(hs.aciklama) + ", ";
                sql = sql + VeriErisim.DegerYap(hs.seviye) + ", ";
                sql = sql + VeriErisim.DegerYap(hs.koruma) + ", ";
                sql = sql + VeriErisim.DegerYap(hs.bakiye) + ", ";
                sql = sql + VeriErisim.DegerYap(hs.tertip) + ", ";
                sql = sql + VeriErisim.DegerYap(hs.kesinti) + ", ";
                sql = sql + VeriErisim.DegerYap(hs.nitelik) + ", ";
                sql = sql + VeriErisim.DegerYap(hs.grup) + ", ";
                sql = sql + VeriErisim.DegerYap(OrtakFonksiyonlar.ConvertToInt(hs.detay.ToString(), 0)) + ",";
                sql = sql + VeriErisim.DegerYap(hs.altHesapKod) + ",";
                sql = sql + VeriErisim.DegerYap(hs.dovizKurKod) + ",";
                sql = sql + VeriErisim.DegerYap(hs.dovizKurTurSatis) + ",";
                sql = sql + VeriErisim.DegerYap(hs.dovizKurTurAlis) + ",";
                sql = sql + VeriErisim.DegerYap(hs.kurFarkiAlacakHesabi) + ",";
                sql = sql + VeriErisim.DegerYap(hs.kurFarkiBorcHesabi) + ",";
                sql = sql + VeriErisim.DegerYap(hs.ekoBasla) + ")";

                sonuc = KomutCalistir(sql, 1);

                if (!sonuc.islemSonuc)
                {
                    veriErisim.hata = "";

                    sql = "UPDATE " + TABLOAD + " SET " + ALANLAR[(int)ENUMAlanlar.YIL] + "=" + hs.yil;
                    sql += ", " + ALANLAR[(int)ENUMAlanlar.HESAPKOD] + "=" + VeriErisim.DegerYap(hs.hesapKod.Replace(".", ""));
                    sql += ", " + ALANLAR[(int)ENUMAlanlar.ACIKLAMA] + "=" + VeriErisim.DegerYap(hs.aciklama);
                    sql += ", " + ALANLAR[(int)ENUMAlanlar.SEVIYE] + "=" + hs.seviye;
                    sql += ", " + ALANLAR[(int)ENUMAlanlar.KORUMA] + "=" + hs.koruma;
                    sql += ", " + ALANLAR[(int)ENUMAlanlar.BAKIYE] + "=" + hs.bakiye;
                    sql += ", " + ALANLAR[(int)ENUMAlanlar.TERTIP] + "=" + hs.tertip;
                    sql += ", " + ALANLAR[(int)ENUMAlanlar.KESINTI] + "=" + hs.kesinti;
                    sql += ", " + ALANLAR[(int)ENUMAlanlar.NITELIK] + "=" + VeriErisim.DegerYap(hs.nitelik);
                    sql += ", " + ALANLAR[(int)ENUMAlanlar.ALTHESAPKOD] + "=" + hs.altHesapKod;
                    sql += ", " + ALANLAR[(int)ENUMAlanlar.GRUP] + "=" + VeriErisim.DegerYap(hs.grup);
                    sql += ", " + ALANLAR[(int)ENUMAlanlar.DOVIZ] + "=" + VeriErisim.DegerYap(hs.dovizKurKod);
                    sql += ", " + ALANLAR[(int)ENUMAlanlar.DOVIZKURTURSATIS] + "=" + VeriErisim.DegerYap(hs.dovizKurTurSatis);
                    sql += ", " + ALANLAR[(int)ENUMAlanlar.DOVIZKURTURALIS] + "=" + VeriErisim.DegerYap(hs.dovizKurTurAlis);
                    sql += ", " + ALANLAR[(int)ENUMAlanlar.KURFARKIALACAKHESABI] + "=" + VeriErisim.DegerYap(hs.kurFarkiAlacakHesabi);
                    sql += ", " + ALANLAR[(int)ENUMAlanlar.KURFARKIBORCHESABI] + "=" + VeriErisim.DegerYap(hs.kurFarkiBorcHesabi);
                    sql += ", " + ALANLAR[(int)ENUMAlanlar.EKOBASLA] + "=" + hs.ekoBasla;
                    sql += " WHERE " + VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", hs.yil);
                    sql += " AND " + VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.HESAPKOD], "=", hs.hesapKod.Replace(".", ""));

                    sonuc = KomutCalistir(sql, 1);
                }

                if (!sonuc.islemSonuc)
                    return sonuc;
            }

            return sonuc;
        }

        /// <summary>
        /// Parametre bilgilerine uygun Tablodaki kayıtları silen sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="hesapPlani">Hesap plani bilgilerini taşıyan nesne.</param>
        /// <returns></returns>
        public Sonuc HesapKodSil(ObjectArray hesapPlani)
        {
            Sonuc sonuc = new Sonuc(false, "", "");

            foreach (HesapPlaniSatir hs in hesapPlani.objeler)
            {
                string sql = "";

                sql = "DELETE FROM " + TABLOAD + " WHERE ";
                sql += VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", hs.yil) + " AND ";
                sql += VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.HESAPKOD], "=", hs.hesapKod.Replace(".", ""));

                sonuc = KomutCalistir(sql, 1);
                if (!sonuc.islemSonuc)
                    return sonuc;
            }

            return sonuc;
        }

        /// <summary>
        /// Kaynak yıldaki bilgileri hedef yıla koplayama sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="kaynakYil">Kaynak yil.</param>
        /// <param name="hedefYil">Hedef yil.</param>
        /// <returns></returns>
        public Sonuc HesapKodAktar(int kaynakYil, int hedefYil)
        {
            string sql = "";

            //Hedef yıldaki veri sil
            sql = "DELETE FROM " + TABLOAD + " WHERE ";
            sql += VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", hedefYil);
            KomutCalistir(sql, 0);

            sql = "INSERT INTO " + TABLOAD + " (YIL, HESAPKOD ,ACIKLAMA ,SEVIYE ,KORUMA ,BAKIYE ,TERTIP ,KESINTI ,NITELIK ,GRUP ,DETAY, ALTHESAPKOD, ACIKLAMA2, DOVIZ, DOVIZKURTURSATIS, DOVIZKURTURALIS, KURFARKIBORCHESABI, KURFARKIALACAKHESABI,EKOBASLA)";
            sql += " SELECT " + hedefYil + ",HESAPKOD ,ACIKLAMA ,SEVIYE ,KORUMA ,BAKIYE ,TERTIP ,KESINTI ,NITELIK ,GRUP ,DETAY, ALTHESAPKOD, ACIKLAMA2, DOVIZ, DOVIZKURTURSATIS, DOVIZKURTURALIS, KURFARKIBORCHESABI, KURFARKIALACAKHESABI, EKOBASLA FROM " + TABLOAD + " WHERE ";
            sql += VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", kaynakYil);

            return KomutCalistir(sql, 0);
        }

        /// <summary>
        /// Parametre olarak gelen alanları kıstas olarak kullanarak, select sql cümlesini oluşturup, çalıştıran ve sonucunu DataReader olarak döndüren fonksiyon
        /// </summary>
        /// <param name="hesKod">Hesap Kodu.</param>
        /// <returns></returns>
        public IDataReader HesapKodListele(HesapPlaniSatir hesKod)
        {
            string sql = "";

            sql = "SELECT " + VeriErisim.TumAlanlar("", ALANLAR) + " FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", hesKod.yil));

            if (hesKod.hesapKod != "")
            {
                string[] hesaplar = hesKod.hesapKod.Split(';');
                string hesapKosul = "";
                for (int i = 0; i < hesaplar.Length; i++)
                    if (hesaplar[i].Replace(".", "").Trim() != "")
                        hesapKosul = VeriErisim.KosulEkle(hesapKosul, "OR", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.HESAPKOD], " LIKE ", hesaplar[i].Replace(".", "").Trim() + "%"));

                if (hesapKosul != "")
                    sql += " AND (" + hesapKosul + ")";
                //sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.HESAPKOD], " LIKE ", hesKod.hesapKod.Replace(".", "").Trim() + "%"));
            }
            if (hesKod.aciklama != "")
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.ACIKLAMA], " LIKE ", hesKod.aciklama + "%"));
            if (hesKod.hesapKodHesapAd != "")
            {
                hesKod.hesapKodHesapAd = hesKod.hesapKodHesapAd.Replace(".", "");
                string[] aranan = hesKod.hesapKodHesapAd.Split(' ', ';');

                foreach (string ara in aranan)
                {
                    if (string.IsNullOrEmpty(ara.Trim()))
                        continue;

                    sql = VeriErisim.KosulEkle(sql, "AND (", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.HESAPKOD], " LIKE ", ara.Replace(".", "").Trim() + "%"));
                    sql = VeriErisim.KosulEkle(sql, "OR", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.ACIKLAMA], " LIKE ", "%" + ara + "%"));
                    sql += ")";
                }
                //string icKosul = "";
                //foreach (string ara in aranan)
                //{
                //    if (string.IsNullOrEmpty(ara.Trim()))
                //        continue;

                //    if (icKosul != "") icKosul += " OR ";
                //    icKosul += "(";
                //    icKosul = VeriErisim.KosulEkle(icKosul, "", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.HESAPKOD], " LIKE ", ara.Replace(".", "").Trim() + "%"));
                //    icKosul = VeriErisim.KosulEkle(icKosul, "OR", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.ACIKLAMA], " LIKE ", "%" + ara + "%"));
                //    icKosul += ")";
                //}
                //if (icKosul != "")
                //    sql = VeriErisim.KosulEkle(sql, "AND", "(" + icKosul + ")");
            }
            if (hesKod.seviye != 0)
            {
                if (hesKod.seviye == -1)
                    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.DETAY], "=", 0));
                else
                    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SEVIYE], "=", hesKod.seviye));
            }
            if (hesKod.koruma != -1)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KORUMA], "=", hesKod.koruma));
            if (hesKod.bakiye != -1)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.BAKIYE], "=", hesKod.bakiye));
            if (hesKod.kesinti != -1)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KESINTI], "=", hesKod.kesinti));
            if (hesKod.altHesapKod != -1)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.ALTHESAPKOD], "=", hesKod.altHesapKod));
            if (hesKod.tertip != -1)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.TERTIP], "=", hesKod.tertip));
            if (hesKod.nitelik != "")
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.NITELIK], "LIKE", "%;" + hesKod.nitelik + ";%"));
            if (hesKod.grup != "")
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.GRUP], " LIKE ", hesKod.grup + "%"));
            if (hesKod.detay)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.DETAY], "=", 0));
            if (hesKod.dovizKurKod == "HEPSİ")
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.DOVIZ], "<>", ""));

            sql += " ORDER BY " + ALANLAR[(int)ENUMAlanlar.HESAPKOD];

            IDataReader r = veriErisim.DataReaderDondur(sql);

            return r;
        }

        /// <summary>
        /// Parametre olarak verilen hesap kodunun özelliklerini listeleyen sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="hKod">Hesap kodu.</param>
        /// <returns></returns>
        public HesapPlaniSatir HesapKodOzellikAl(HesapPlaniSatir hKod)
        {
            string sql = "";
            Tertip t = new Tertip(DateTime.Now.Year, ENUMTabloKod.TABOdemeEmri);

            hKod.hesapKod = hKod.hesapKod.Replace(".", "");

            sql = "SELECT " + VeriErisim.TumAlanlar("", ALANLAR) + " FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", hKod.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.HESAPKOD], "=", hKod.hesapKod));

            IDataReader r = veriErisim.DataReaderDondur(sql);

            hKod.hesapKod = "";
            if (veriErisim.hata == "")
            {
                if (r.Read())
                {
                    hKod.yil = VeriErisim.DegerAl(r, (int)ENUMAlanlar.YIL, 0);
                    hKod.hesapKod = VeriErisim.DegerAl(r, (int)ENUMAlanlar.HESAPKOD, "");
                    hKod.aciklama = VeriErisim.DegerAl(r, (int)ENUMAlanlar.ACIKLAMA, "");
                    hKod.seviye = VeriErisim.DegerAl(r, (int)ENUMAlanlar.SEVIYE, 0);
                    hKod.koruma = VeriErisim.DegerAl(r, (int)ENUMAlanlar.KORUMA, 0);
                    hKod.bakiye = VeriErisim.DegerAl(r, (int)ENUMAlanlar.BAKIYE, 0);
                    hKod.tertip = VeriErisim.DegerAl(r, (int)ENUMAlanlar.TERTIP, 0);
                    hKod.kesinti = VeriErisim.DegerAl(r, (int)ENUMAlanlar.KESINTI, 0);
                    hKod.nitelik = VeriErisim.DegerAl(r, (int)ENUMAlanlar.NITELIK, "");
                    hKod.grup = VeriErisim.DegerAl(r, (int)ENUMAlanlar.GRUP, "");
                    hKod.detay = VeriErisim.DegerAl(r, (int)ENUMAlanlar.DETAY, 0) == 0;
                    hKod.altHesapKod = VeriErisim.DegerAl(r, (int)ENUMAlanlar.ALTHESAPKOD, 0);

                    hKod.dovizKurKod = VeriErisim.DegerAl(r, (int)ENUMAlanlar.DOVIZ, "");
                    hKod.dovizKurTurSatis = VeriErisim.DegerAl(r, (int)ENUMAlanlar.DOVIZKURTURSATIS, "");
                    hKod.dovizKurTurAlis = VeriErisim.DegerAl(r, (int)ENUMAlanlar.DOVIZKURTURALIS, "");
                    hKod.kurFarkiAlacakHesabi = VeriErisim.DegerAl(r, (int)ENUMAlanlar.KURFARKIALACAKHESABI, "");
                    hKod.kurFarkiBorcHesabi = VeriErisim.DegerAl(r, (int)ENUMAlanlar.KURFARKIBORCHESABI, "");

                    hKod.ekoBasla = VeriErisim.DegerAl(r, (int)ENUMAlanlar.EKOBASLA, 0);
                    //HesapKodDetayVarMi(hKod);
                }
            }

            hKod.hesapKod = OrtakFonksiyonlar.KodNoktaKoy((string)t.Kirilimlar(0), hKod.hesapKod.Replace(".", ""));

            r.Close();

            return hKod;
        }

        /// <summary>
        /// Parametre olrak verilen hesap koduna ait alt kayıtların varlığını kontrol eden sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="yil">Yıl.</param>
        /// <param name="hesapKod">Hesap kodu.</param>
        /// <param name="detayAra">Eğer <c>true</c> ise detay ara.</param>
        /// <returns></returns>
        public bool HesapKodAnasiCocuguVarMi(int yil, string hesapKod, bool detayAra)
        {
            string sql = "";
            bool varMi = false;

            sql = "SELECT COUNT(*) FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", yil));
            if (detayAra)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.HESAPKOD], "LIKE", hesapKod.Replace(".", "") + "%"));
            else
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.HESAPKOD], "=", hesapKod.Replace(".", "")));

            IDataReader r = veriErisim.DataReaderDondur(sql);

            if (veriErisim.hata == "")
                if (r.Read())
                    if (detayAra)
                        varMi = VeriErisim.DegerAl(r, 0, 0) >= 2;
                    else
                        varMi = VeriErisim.DegerAl(r, 0, 0) >= 1;

            r.Close();

            return varMi;
        }

        bool HesapKodAciklamaVarmi(int yil, string hesapKod, string aciklama)
        {
            string sql = "";
            bool varMi = false;

            sql = "SELECT COUNT(*) FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.HESAPKOD], "<>", hesapKod.Replace(".", "")));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.ACIKLAMA], "=", aciklama.Trim()));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.HESAPKOD], "NOT LIKE ", "8%"));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.HESAPKOD], "NOT LIKE ", "6%"));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.HESAPKOD], "NOT LIKE ", "9%"));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.HESAPKOD], "NOT LIKE ", "32%"));

            IDataReader r = veriErisim.DataReaderDondur(sql);

            if (veriErisim.hata == "")
                if (r.Read())
                    varMi = VeriErisim.DegerAl(r, 0, 0) >= 1;

            r.Close();

            return varMi;
        }

        /// <summary>
        /// Parametre olarak gelen alanları kıstas olarak kullanarak, select sql cümlesini oluşturup, çalıştıran ve sonucunu DataReader olarak döndüren fonksiyon
        /// </summary>
        /// <param name="hesKod">hesap kodu.</param>
        /// <returns></returns>
        public IDataReader YansitmaListele(ObjectArray hesKod)
        {
            string sql = "";
            string kosul = "";
            int yil = 0;

            sql = "SELECT " + VeriErisim.TumAlanlar(TABLOADYANSITMA, ALANLARYANSITMA) + "," + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.ACIKLAMA] + "," + TABLOAD + "_1" + "." + ALANLAR[(int)ENUMAlanlar.ACIKLAMA] + " FROM " + TABLOADYANSITMA + "," + TABLOAD + "," + TABLOAD + " " + TABLOAD + "_1 WHERE ";

            foreach (HesapPlaniSatir h in hesKod.objeler)
            {
                yil = h.yil;
                kosul = VeriErisim.KosulEkle(kosul, "OR", VeriErisim.KosulOl("", ALANLARYANSITMA[(int)ENUMAlanlarYansitma.HKOD], "=", h.hesapKod.Replace(".", "").Trim()));
            }

            sql += TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.YIL] + "=" + TABLOADYANSITMA + "." + ALANLARYANSITMA[(int)ENUMAlanlarYansitma.YIL] + " AND ";
            sql += TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.HESAPKOD] + "=" + TABLOADYANSITMA + "." + ALANLARYANSITMA[(int)ENUMAlanlarYansitma.HKOD] + " AND ";

            sql += TABLOAD + "_1" + "." + ALANLAR[(int)ENUMAlanlar.YIL] + "=" + TABLOADYANSITMA + "." + ALANLARYANSITMA[(int)ENUMAlanlarYansitma.YIL] + " AND ";
            sql += TABLOAD + "_1" + "." + ALANLAR[(int)ENUMAlanlar.HESAPKOD] + "=" + TABLOADYANSITMA + "." + ALANLARYANSITMA[(int)ENUMAlanlarYansitma.YKOD] + " AND ";

            sql = VeriErisim.KosulEkle(sql, "", VeriErisim.KosulOl(TABLOADYANSITMA, ALANLARYANSITMA[(int)ENUMAlanlarYansitma.YIL], "=", yil));

            if (kosul != "")
                sql += " AND (" + kosul + ")";

            sql += " ORDER BY " + ALANLARYANSITMA[(int)ENUMAlanlarYansitma.SIRANO];
            IDataReader r = veriErisim.DataReaderDondur(sql);

            return r;
        }

        /// <summary>
        /// Verilen yıl kriterine uygun tablodaki bütün kayıtları silen sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="yil">Yıl</param>
        /// <returns>Sil komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc SilHepsini(int yil)
        {
            string sql = "";

            sql = "DELETE FROM " + TABLOAD + " WHERE ";
            sql += VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", yil);

            return KomutCalistir(sql, 0);
        }

        /// <summary>
        /// Hesap kodlarının alt kayıt sayıları ve seviyelerini hesaplayıp tabloyu güncelleyen sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="yil">Yıl.</param>
        /*
        public void HesapPlaniDetaySayilariDuzenle(int yil)
        {
            //Anası olmayan hesap kodları
            //SELECT YIL,HESAPKOD,(SELECT 1 FROM MUHHESAPPLANI B WHERE A.YIL=B.YIL AND SUBSTRING(A.HESAPKOD,1,LEN(A.HESAPKOD)-2)=B.HESAPKOD) AS ANASI FROM MUHHESAPPLANI A 
            //WHERE YIL>2001 AND LEN(HESAPKOD)>3 AND LEN(HESAPKOD)<14 GROUP BY YIL,HESAPKOD ORDER BY ANASI,YIL,HESAPKOD


            string sql;
            Sonuc sonuc = new Sonuc(false, "", "");

            sql = "UPDATE MUHHESAPPLANI SET SEVIYE=1 WHERE YIL=" + yil + " AND " + VeriErisim.IslecBul("LEN") + "(HESAPKOD)=3";
            KomutCalistir(sql, 0, 600000);
            sql = "UPDATE MUHHESAPPLANI SET SEVIYE=2 WHERE YIL=" + yil + " AND " + VeriErisim.IslecBul("LEN") + "(HESAPKOD)=5";
            KomutCalistir(sql, 0, 600000);
            sql = "UPDATE MUHHESAPPLANI SET SEVIYE=3 WHERE YIL=" + yil + " AND " + VeriErisim.IslecBul("LEN") + "(HESAPKOD)=7";
            KomutCalistir(sql, 0, 600000);
            sql = "UPDATE MUHHESAPPLANI SET SEVIYE=4 WHERE YIL=" + yil + " AND " + VeriErisim.IslecBul("LEN") + "(HESAPKOD)=9";
            KomutCalistir(sql, 0, 600000);
            sql = "UPDATE MUHHESAPPLANI SET SEVIYE=5 WHERE YIL=" + yil + " AND " + VeriErisim.IslecBul("LEN") + "(HESAPKOD)=11";
            KomutCalistir(sql, 0, 600000);
            sql = "UPDATE MUHHESAPPLANI SET SEVIYE=6 WHERE YIL=" + yil + " AND " + VeriErisim.IslecBul("LEN") + "(HESAPKOD)=13";
            KomutCalistir(sql, 0, 600000);
            sql = "UPDATE MUHHESAPPLANI SET SEVIYE=7 WHERE YIL=" + yil + " AND " + VeriErisim.IslecBul("LEN") + "(HESAPKOD)=15";
            KomutCalistir(sql, 0, 600000);
            sql = "UPDATE MUHHESAPPLANI SET SEVIYE=8 WHERE YIL=" + yil + " AND " + VeriErisim.IslecBul("LEN") + "(HESAPKOD)=17";
            KomutCalistir(sql, 0, 600000);
            sql = "UPDATE MUHHESAPPLANI SET SEVIYE=0 WHERE YIL=" + yil + " AND HESAPKOD IS NULL";
            KomutCalistir(sql, 0, 600000);

            for (int i = 1; i < 10; i++)
            {
                sql = "UPDATE MUHHESAPPLANI ";
                sql += "SET DETAY=(SELECT COUNT(*) FROM MUHHESAPPLANI ";
                sql += " B WHERE B.YIL=MUHHESAPPLANI.YIL AND ";
                sql += "B.HESAPKOD LIKE MUHHESAPPLANI.HESAPKOD" + VeriErisim.IslecBul("+") + "'%' AND ";
                sql += "B.SEVIYE>MUHHESAPPLANI.SEVIYE AND YIL=" + yil + ") WHERE YIL=" + yil + " AND MUHHESAPPLANI.HESAPKOD LIKE '" + i.ToString() + "%' AND YIL=" + DateTime.Now.Year.ToString();

                sonuc = KomutCalistir(sql, 0, 600000);

            }

            return;
        }
        */

        public void HesapPlaniDetaySayilariDuzenle(int yil, int seviye)
        {
            //Anası olmayan hesap kodları
            //SELECT YIL,HESAPKOD,(SELECT 1 FROM MUHHESAPPLANI B WHERE A.YIL=B.YIL AND SUBSTRING(A.HESAPKOD,1,LEN(A.HESAPKOD)-2)=B.HESAPKOD) AS ANASI FROM MUHHESAPPLANI A 
            //WHERE YIL>2001 AND LEN(HESAPKOD)>3 AND LEN(HESAPKOD)<14 GROUP BY YIL,HESAPKOD ORDER BY ANASI,YIL,HESAPKOD

            string sql;
            Sonuc sonuc = new Sonuc(false, "", "");

            if (seviye == 1)
            {
                sql = "UPDATE MUHHESAPPLANI SET SEVIYE=1 WHERE YIL=" + yil + " AND " + VeriErisim.IslecBul("LEN") + "(HESAPKOD)=3";
                KomutCalistir(sql, 0, 600000);
            }
            else if (seviye == 2)
            {
                sql = "UPDATE MUHHESAPPLANI SET SEVIYE=2 WHERE YIL=" + yil + " AND " + VeriErisim.IslecBul("LEN") + "(HESAPKOD)=5";
                KomutCalistir(sql, 0, 600000);
            }
            else if (seviye == 3)
            {
                sql = "UPDATE MUHHESAPPLANI SET SEVIYE=3 WHERE YIL=" + yil + " AND " + VeriErisim.IslecBul("LEN") + "(HESAPKOD)=7";
                KomutCalistir(sql, 0, 600000);
            }
            else if (seviye == 4)
            {
                sql = "UPDATE MUHHESAPPLANI SET SEVIYE=4 WHERE YIL=" + yil + " AND " + VeriErisim.IslecBul("LEN") + "(HESAPKOD)=9";
                KomutCalistir(sql, 0, 600000);
            }
            else if (seviye == 5)
            {
                sql = "UPDATE MUHHESAPPLANI SET SEVIYE=5 WHERE YIL=" + yil + " AND " + VeriErisim.IslecBul("LEN") + "(HESAPKOD)=11";
                KomutCalistir(sql, 0, 600000);
            }
            else if (seviye == 6)
            {
                sql = "UPDATE MUHHESAPPLANI SET SEVIYE=6 WHERE YIL=" + yil + " AND " + VeriErisim.IslecBul("LEN") + "(HESAPKOD)=13";
                KomutCalistir(sql, 0, 600000);
            }
            else if (seviye == 7)
            {
                sql = "UPDATE MUHHESAPPLANI SET SEVIYE=7 WHERE YIL=" + yil + " AND " + VeriErisim.IslecBul("LEN") + "(HESAPKOD)=15";
                KomutCalistir(sql, 0, 600000);
            }
            else if (seviye == 8)
            {
                sql = "UPDATE MUHHESAPPLANI SET SEVIYE=8 WHERE YIL=" + yil + " AND " + VeriErisim.IslecBul("LEN") + "(HESAPKOD)=17";
                KomutCalistir(sql, 0, 600000);
            }
            else if (seviye == 0)
            {
                sql = "UPDATE MUHHESAPPLANI SET SEVIYE=0 WHERE YIL=" + yil + " AND HESAPKOD IS NULL";
                KomutCalistir(sql, 0, 600000);
            }

            return;
        }

        /// <summary>
        /// Hesaps the plani detay sayilari duzenle.
        /// </summary>
        /// <param name="yil">The yil.</param>
        /// <param name="hesapKod">The hesap kod.</param>
        public void HesapPlaniDetaySayilariDuzenle(int yil, string hesapKod)
        {
            string sql = "UPDATE MUHHESAPPLANI ";
            sql += " SET DETAY=(SELECT COUNT(*) FROM MUHHESAPPLANI B WHERE B.YIL=MUHHESAPPLANI.YIL AND";
            sql += " B.HESAPKOD LIKE MUHHESAPPLANI.HESAPKOD" + VeriErisim.IslecBul("+") + "'%' AND B.SEVIYE>MUHHESAPPLANI.SEVIYE AND YIL=" + yil + ")";
            sql += " WHERE YIL=" + yil + " AND MUHHESAPPLANI.HESAPKOD LIKE '" + hesapKod.Replace(".", "") + "%'";

            KomutCalistir(sql, 0, 600000);

            return;
        }

        /// <summary>
        /// Parametre olarak verilen hesap kodunun niteliklerini parametrede verilen nitelikler ile değiştiren sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon.
        /// </summary>
        /// <param name="hs">Hesap Planı Satır bilgisi taşıyan nesne.</param>
        /// <returns></returns>
        public Sonuc NitelikDegistir(HesapPlaniSatir hs)
        {
            string sql = "";

            if (hs.nitelik != "")
            {
                if (hs.nitelik.Trim().StartsWith(";"))
                    hs.nitelik = hs.nitelik.Trim().Substring(1, hs.nitelik.Trim().Length - 1);
                if (hs.nitelik.Trim().EndsWith(";"))
                    hs.nitelik = hs.nitelik.Trim().Substring(0, hs.nitelik.Trim().Length - 1);
                hs.nitelik = ";" + hs.nitelik.Trim() + ";";
            }

            sql = "UPDATE " + TABLOAD + " SET ";
            sql += ALANLAR[(int)ENUMAlanlar.KORUMA] + "=" + hs.koruma;
            sql += ", " + ALANLAR[(int)ENUMAlanlar.GRUP] + "=" + VeriErisim.DegerYap(hs.grup);
            if (hs.tertip > -1)
                sql += ", " + ALANLAR[(int)ENUMAlanlar.TERTIP] + "=" + hs.tertip;
            if (hs.kesinti > -1)
                sql += ", " + ALANLAR[(int)ENUMAlanlar.KESINTI] + "=" + hs.kesinti;
            if (hs.altHesapKod > -1)
                sql += ", " + ALANLAR[(int)ENUMAlanlar.ALTHESAPKOD] + "=" + hs.altHesapKod;
            if (hs.bakiye > -1)
                sql += ", " + ALANLAR[(int)ENUMAlanlar.BAKIYE] + "=" + hs.bakiye;
            if (hs.nitelik != "")
                sql += ", " + ALANLAR[(int)ENUMAlanlar.NITELIK] + "=" + VeriErisim.DegerYap(hs.nitelik);
            if (hs.ekoBasla > -2)
                sql += ", " + ALANLAR[(int)ENUMAlanlar.EKOBASLA] + "=" + hs.ekoBasla;
            sql += " WHERE " + VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", hs.yil);
            sql += " AND " + VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.HESAPKOD], "LIKE", hs.hesapKod.Replace(".", "") + "%");

            return KomutCalistir(sql, 0);
        }

        public Sonuc NitelikKaldir(HesapPlaniSatir hs, string nitelik)
        {
            string sql = "";

            sql = "UPDATE " + TABLOAD + " SET ";
            sql += ALANLAR[(int)ENUMAlanlar.NITELIK] + "=REPLACE(NITELIK,'" + nitelik + "',';')";
            sql += " WHERE " + VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", hs.yil);
            sql += " AND " + VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.HESAPKOD], "LIKE", hs.hesapKod.Replace(".", "") + "%");

            return KomutCalistir(sql, 0);
        }

        public Sonuc HesapPlaniGuncelle2021()
        {
            string sql = "";
            sql = @"

UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Teşekküllere', HESAPKOD='325030190' WHERE YIL=2021 AND HESAPKOD='325030109'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Teşekküllere', HESAPKOD='32503019090' WHERE YIL=2021 AND HESAPKOD='32503010901'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Teşekküllere', HESAPKOD='32503019090' WHERE YIL=2021 AND HESAPKOD='32503010902'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Hazine Yardımları', HESAPKOD='3250302' WHERE YIL=2021 AND HESAPKOD='3250302'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Genel Bütçeye Hazine Yardımı', HESAPKOD='325030210' WHERE YIL=2021 AND HESAPKOD='325030201'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Genel Bütçeye Hazine Yardımı', HESAPKOD='32503021001' WHERE YIL=2021 AND HESAPKOD='32503020101'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Özel Bütçeli İdarelere Hazine Yardımları', HESAPKOD='325030220' WHERE YIL=2021 AND HESAPKOD='325030202'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Ölçme, Seçme ve Yerleştirme Merkezi Başkanlığı na Hazine Yardımı', HESAPKOD='32503022001' WHERE YIL=2021 AND HESAPKOD='32503020201'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Atatürk Kültür, Dil ve Tarih Yüksek Kurumu na Hazine Yardımı', HESAPKOD='32503022002' WHERE YIL=2021 AND HESAPKOD='32503020202'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Atatürk Araştırma Merkezi ne Hazine Yardımı', HESAPKOD='32503022003' WHERE YIL=2021 AND HESAPKOD='32503020203'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Atatürk Kültür Merkezi ne Hazine Yardımı', HESAPKOD='32503022004' WHERE YIL=2021 AND HESAPKOD='32503020204'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Türk Dil Kurumu na Hazine Yardımı', HESAPKOD='32503022005' WHERE YIL=2021 AND HESAPKOD='32503020205'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Türk Tarih Kurumu na Hazine Yardımı', HESAPKOD='32503022006' WHERE YIL=2021 AND HESAPKOD='32503020206'
DELETE FROM  MUHHESAPPLANI WHERE YIL=2021 AND HESAPKOD='32503020207'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Türkiye Bilimsel ve Teknolojik Araştırma Kurumu Başkanlığı na Hazine Yardımı', HESAPKOD='32503022007' WHERE YIL=2021 AND HESAPKOD='32503020208'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Türkiye Bilimler Akademisi Başkanlığı na Hazine Yardımı', HESAPKOD='32503022008' WHERE YIL=2021 AND HESAPKOD='32503020209'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Türkiye Adalet Akademisi’ne Hazine Yardımı', HESAPKOD='32503022009' WHERE YIL=2021 AND HESAPKOD='32503020210'
DELETE FROM  MUHHESAPPLANI WHERE YIL=2021 AND HESAPKOD='32503020213'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sınıflandırmaya Girmeyen Diğer Hazine Yardımları', HESAPKOD='32503029090' WHERE YIL=2021 AND HESAPKOD='32503020214'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Devlet Tiyatroları Genel Müdürlüğü ne Hazine Yardımı', HESAPKOD='32503022010' WHERE YIL=2021 AND HESAPKOD='32503020215'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Devlet Opera ve Balesi Genel Müdürlüğü ne Hazine Yardımı', HESAPKOD='32503022011' WHERE YIL=2021 AND HESAPKOD='32503020216'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Orman Genel Müdürlüğü ne Hazine Yardımı', HESAPKOD='32503022012' WHERE YIL=2021 AND HESAPKOD='32503020217'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Vakıflar Genel Müdürlüğü ne Hazine Yardımı', HESAPKOD='32503022013' WHERE YIL=2021 AND HESAPKOD='32503020218'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Türkiye Hudut ve Sahiller Sağlık Genel Müdürlüğü ne Hazine Yardımı', HESAPKOD='32503022014' WHERE YIL=2021 AND HESAPKOD='32503020219'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Türk Akreditasyon Kurumu na Hazine Yardımı', HESAPKOD='32503022015' WHERE YIL=2021 AND HESAPKOD='32503020221'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Türk Standartları Enstitüsü ne Hazine Yardımı', HESAPKOD='32503022016' WHERE YIL=2021 AND HESAPKOD='32503020222'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Türk Patent Enstitüsü ne Hazine Yardımı', HESAPKOD='32503022017' WHERE YIL=2021 AND HESAPKOD='32503020224'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Ulusal Bor Araştırma Enstitüsü ne Hazine Yardımı', HESAPKOD='32503022018' WHERE YIL=2021 AND HESAPKOD='32503020226'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Türkiye Atom Enerjisi Kurumu na Hazine Yardımı', HESAPKOD='32503022019' WHERE YIL=2021 AND HESAPKOD='32503020227'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Savunma Sanayi Başkanlığı na Hazine Yardımı', HESAPKOD='32503022020' WHERE YIL=2021 AND HESAPKOD='32503020228'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Küçük ve Orta Ölçekli İşletmeleri Geliştirme ve Destekleme İdaresi Başkanlığı na Hazine Yardımı', HESAPKOD='32503022021' WHERE YIL=2021 AND HESAPKOD='32503020230'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Türk İşbirliği ve Koordinasyon Ajansı Başkanlığı na Hazine Yardımı', HESAPKOD='32503022022' WHERE YIL=2021 AND HESAPKOD='32503020232'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='GAP Bölge Kalkınma İdaresi Başkanlığı na Hazine Yardımı', HESAPKOD='32503022023' WHERE YIL=2021 AND HESAPKOD='32503020234'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Özelleştirme İdaresi Başkanlığı na Hazine Yardımı', HESAPKOD='32503022024' WHERE YIL=2021 AND HESAPKOD='32503020235'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Maden Tetkik ve Arama Genel Müdürlüğü ne Hazine Yardımı', HESAPKOD='32503022025' WHERE YIL=2021 AND HESAPKOD='32503020240'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Ceza İnfaz Kurumları ile Tutukevleri İş Yurtları Kurumu na  Hazine Yardımı', HESAPKOD='32503022026' WHERE YIL=2021 AND HESAPKOD='32503020241'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sivil Havacılık Genel Müdürlüğü ne Hazine Yardımı', HESAPKOD='32503022027' WHERE YIL=2021 AND HESAPKOD='32503020249'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yurtdışı Türkler ve Akraba Topluluklar Başkanlığına Hazine Yardımı', HESAPKOD='32503022029' WHERE YIL=2021 AND HESAPKOD='32503020251'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Karayolları Genel Müdürlüğü ne Hazine Yardımı', HESAPKOD='32503022030' WHERE YIL=2021 AND HESAPKOD='32503020252'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Türkiye Yazma Eserler Kurumu Başkanlığı na Hazine Yardımı', HESAPKOD='32503022031' WHERE YIL=2021 AND HESAPKOD='32503020253'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Doğu Anadolu Projesi Bölge Kalkınma İdaresi Başkanlığı na Hazine Yardımı', HESAPKOD='32503022032' WHERE YIL=2021 AND HESAPKOD='32503020254'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Konya Ovası Projesi Bölge Kalkınma İdaresi Başkanlığı na Hazine Yardımı', HESAPKOD='32503022033' WHERE YIL=2021 AND HESAPKOD='32503020255'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Doğu Karadeniz Projesi Bölge Kalkınma İdaresi Başkanlığı na Hazine Yardımı', HESAPKOD='32503022034' WHERE YIL=2021 AND HESAPKOD='32503020256'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Devlet Su İşleri Genel Müdürlüğü ne Hazine Yardımı', HESAPKOD='32503022035' WHERE YIL=2021 AND HESAPKOD='32503020257'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Türkiye Su Enstitüsü ne Hazine Yardımı', HESAPKOD='32503022036' WHERE YIL=2021 AND HESAPKOD='32503020258'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Türkiye İlaç ve Tıbbi Cihaz Kurumu na Hazine Yardımı', HESAPKOD='32503022037' WHERE YIL=2021 AND HESAPKOD='32503020259'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kamu Denetçiliği Kurumu na Hazine Yardımı', HESAPKOD='32503022038' WHERE YIL=2021 AND HESAPKOD='32503020260'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Türkiye İnsan Hakları ve Eşitlik Kurumu na Hazine Yardımı', HESAPKOD='32503022039' WHERE YIL=2021 AND HESAPKOD='32503020261'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Türkiye Sağlık Enstitüleri Başkanlığı na Hazine Yardımı', HESAPKOD='32503022040' WHERE YIL=2021 AND HESAPKOD='32503020262'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Helal Akreditasyon Kurumu na Hazine Yardımı', HESAPKOD='32503022041' WHERE YIL=2021 AND HESAPKOD='32503020263'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Nadir Toprak Elementleri Araştırma Enstitüsü ne Hazine Yardımı', HESAPKOD='32503022042' WHERE YIL=2021 AND HESAPKOD='32503020264'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Maden ve Petrol İşleri Genel Müdürlüğü ne Hazine Yardımı', HESAPKOD='32503022043' WHERE YIL=2021 AND HESAPKOD='32503020265'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Türkiye Uzay Ajansı na Hazine Yardımı', HESAPKOD='32503022044' WHERE YIL=2021 AND HESAPKOD='32503020266'
DELETE FROM  MUHHESAPPLANI WHERE YIL=2021 AND HESAPKOD='32503020286'
DELETE FROM  MUHHESAPPLANI WHERE YIL=2021 AND HESAPKOD='32503020287'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Düzenleyici ve Denetleyici Kurumlara Hazine Yardımları', HESAPKOD='325030230' WHERE YIL=2021 AND HESAPKOD='325030203'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Radyo ve Televizyon Üst Kurulu na Hazine Yardımı', HESAPKOD='32503023001' WHERE YIL=2021 AND HESAPKOD='32503020301'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Bilgi Teknolojileri ve İletişim Kurumu na Hazine Yardımı', HESAPKOD='32503023002' WHERE YIL=2021 AND HESAPKOD='32503020302'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sermaye Piyasası Kurulu na Hazine Yardımı', HESAPKOD='32503023003' WHERE YIL=2021 AND HESAPKOD='32503020303'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Bankacılık Düzenleme ve Denetleme Kurumu na Hazine Yardımı', HESAPKOD='32503023004' WHERE YIL=2021 AND HESAPKOD='32503020304'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Enerji Piyasası Düzenleme Kurumu na Hazine Yardımı', HESAPKOD='32503023005' WHERE YIL=2021 AND HESAPKOD='32503020305'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kamu İhale Kurumu na Hazine Yardımı', HESAPKOD='32503023006' WHERE YIL=2021 AND HESAPKOD='32503020306'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Rekabet Kurumu na Hazine Yardımı', HESAPKOD='32503023007' WHERE YIL=2021 AND HESAPKOD='32503020307'
DELETE FROM  MUHHESAPPLANI WHERE YIL=2021 AND HESAPKOD='32503020309'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kamu Gözetimi, Muhasebe ve Denetim Standartları Kurumu na Hazine Yardımı', HESAPKOD='32503023008' WHERE YIL=2021 AND HESAPKOD='32503020310'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kişisel Verileri Koruma Kurumu na Hazine Yardımı', HESAPKOD='32503023009' WHERE YIL=2021 AND HESAPKOD='32503020311'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Nükleer Düzenleme Kurumu na Hazine Yardımı', HESAPKOD='32503023010' WHERE YIL=2021 AND HESAPKOD='32503020312'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sigortacılık ve Özel Emeklilik Düzenleme ve Denetleme Kurumu na Hazine Yardımı', HESAPKOD='32503023011' WHERE YIL=2021 AND HESAPKOD='32503020313'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sosyal Güvenlik Kurumlarına Hazine Yardımları', HESAPKOD='325030240' WHERE YIL=2021 AND HESAPKOD='325030204'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Türkiye İş Kurumu Genel Müdürlüğü ne Hazine Yardımı', HESAPKOD='32503024001' WHERE YIL=2021 AND HESAPKOD='32503020404'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sosyal Güvenlik Kurumu na Açık Finansmanı Ödemeleri', HESAPKOD='32503024002' WHERE YIL=2021 AND HESAPKOD='32503020406'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Mahalli İdarelere Hazine Yardımları', HESAPKOD='325030250' WHERE YIL=2021 AND HESAPKOD='325030205'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='İl Özel İdarelerine', HESAPKOD='32503025001' WHERE YIL=2021 AND HESAPKOD='32503020501'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Belediyelere', HESAPKOD='32503025002' WHERE YIL=2021 AND HESAPKOD='32503020502'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Köylere', HESAPKOD='32503025003' WHERE YIL=2021 AND HESAPKOD='32503020503'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Mahalli İdare Birliklerine', HESAPKOD='32503025004' WHERE YIL=2021 AND HESAPKOD='32503020504'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Bağlı İdarelere', HESAPKOD='32503025005' WHERE YIL=2021 AND HESAPKOD='32503020505'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Hasılattan Paylar ve Giderlere Katılma Payları', HESAPKOD='325030260' WHERE YIL=2021 AND HESAPKOD='325030206'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Giderlere Katılma Payları', HESAPKOD='32503026002' WHERE YIL=2021 AND HESAPKOD='32503020619'
DELETE FROM  MUHHESAPPLANI WHERE YIL=2021 AND HESAPKOD='32503020608'
DELETE FROM  MUHHESAPPLANI WHERE YIL=2021 AND HESAPKOD='32503020609'
DELETE FROM  MUHHESAPPLANI WHERE YIL=2021 AND HESAPKOD='32503020610'
DELETE FROM  MUHHESAPPLANI WHERE YIL=2021 AND HESAPKOD='32503020611'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yükseköğretim Kurumlarına Hazine Yardımları', HESAPKOD='325030270' WHERE YIL=2021 AND HESAPKOD='325030207'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yükseköğretim Kurulu na Hazine Yardımı', HESAPKOD='32503027001' WHERE YIL=2021 AND HESAPKOD='32503020701'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Ankara Üniversitesi ne Hazine Yardımı', HESAPKOD='32503027002' WHERE YIL=2021 AND HESAPKOD='32503020702'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Ortadoğu Teknik Üniversitesi ne Hazine Yardımı', HESAPKOD='32503027003' WHERE YIL=2021 AND HESAPKOD='32503020703'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Hacettepe Üniversitesi ne Hazine Yardımı', HESAPKOD='32503027004' WHERE YIL=2021 AND HESAPKOD='32503020704'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Gazi Üniversitesi ne Hazine Yardımı', HESAPKOD='32503027005' WHERE YIL=2021 AND HESAPKOD='32503020705'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='İstanbul Üniversitesi ne Hazine Yardımı', HESAPKOD='32503027006' WHERE YIL=2021 AND HESAPKOD='32503020706'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='İstanbul Teknik Üniversitesi ne Hazine Yardımı', HESAPKOD='32503027007' WHERE YIL=2021 AND HESAPKOD='32503020707'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Boğaziçi Üniversitesi ne Hazine Yardımı', HESAPKOD='32503027008' WHERE YIL=2021 AND HESAPKOD='32503020708'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Marmara Üniversitesi ne Hazine Yardımı', HESAPKOD='32503027009' WHERE YIL=2021 AND HESAPKOD='32503020709'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yıldız Teknik Üniversitesi ne Hazine Yardımı', HESAPKOD='32503027010' WHERE YIL=2021 AND HESAPKOD='32503020710'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Mimar Sinan Güzel Sanatlar Üniversitesi ne Hazine Yardımı', HESAPKOD='32503027011' WHERE YIL=2021 AND HESAPKOD='32503020711'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Ege Üniversitesi ne Hazine Yardımı', HESAPKOD='32503027012' WHERE YIL=2021 AND HESAPKOD='32503020712'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Dokuz Eylül Üniversitesi ne Hazine Yardımı', HESAPKOD='32503027013' WHERE YIL=2021 AND HESAPKOD='32503020713'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Trakya Üniversitesi ne Hazine Yardımı', HESAPKOD='32503027014' WHERE YIL=2021 AND HESAPKOD='32503020714'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Bursa Uludağ Üniversitesi ne Hazine Yardımı', HESAPKOD='32503027015' WHERE YIL=2021 AND HESAPKOD='32503020715'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Anadolu Üniversitesi ne Hazine Yardımı', HESAPKOD='32503027016' WHERE YIL=2021 AND HESAPKOD='32503020716'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Selçuk Üniversitesi ne Hazine Yardımı', HESAPKOD='32503027017' WHERE YIL=2021 AND HESAPKOD='32503020717'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Akdeniz Üniversitesi ne Hazine Yardımı', HESAPKOD='32503027018' WHERE YIL=2021 AND HESAPKOD='32503020718'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Erciyes Üniversitesi ne Hazine Yardımı', HESAPKOD='32503027019' WHERE YIL=2021 AND HESAPKOD='32503020719'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sivas Cumhuriyet Üniversitesi ne Hazine Yardımı', HESAPKOD='32503027020' WHERE YIL=2021 AND HESAPKOD='32503020720'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Çukurova Üniversitesi ne Hazine Yardımı', HESAPKOD='32503027021' WHERE YIL=2021 AND HESAPKOD='32503020721'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Ondokuz Mayıs Üniversitesi ne Hazine Yardımı', HESAPKOD='32503027022' WHERE YIL=2021 AND HESAPKOD='32503020722'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Karadeniz Teknik Üniversitesi ne Hazine Yardımı', HESAPKOD='32503027023' WHERE YIL=2021 AND HESAPKOD='32503020723'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Atatürk Üniversitesi ne Hazine Yardımı', HESAPKOD='32503027024' WHERE YIL=2021 AND HESAPKOD='32503020724'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='İnönü Üniversitesi ne Hazine Yardımı', HESAPKOD='32503027025' WHERE YIL=2021 AND HESAPKOD='32503020725'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Fırat Üniversitesi ne Hazine Yardımı', HESAPKOD='32503027026' WHERE YIL=2021 AND HESAPKOD='32503020726'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Dicle Üniversitesi ne Hazine Yardımı', HESAPKOD='32503027027' WHERE YIL=2021 AND HESAPKOD='32503020727'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yüzüncüyıl Üniversitesi ne Hazine Yardımı', HESAPKOD='32503027028' WHERE YIL=2021 AND HESAPKOD='32503020728'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Gaziantep Üniversitesi ne Hazine Yardımı', HESAPKOD='32503027029' WHERE YIL=2021 AND HESAPKOD='32503020729'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='İzmir Yüksek Teknoloji Enstitüsü ne Hazine Yardımı', HESAPKOD='32503027030' WHERE YIL=2021 AND HESAPKOD='32503020730'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Gebze Teknik Üniversitesi ne Hazine Yardımı', HESAPKOD='32503027031' WHERE YIL=2021 AND HESAPKOD='32503020731'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Harran Üniversitesi ne Hazine Yardımı', HESAPKOD='32503027032' WHERE YIL=2021 AND HESAPKOD='32503020732'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Süleyman Demirel Üniversitesi ne Hazine Yardımı', HESAPKOD='32503027033' WHERE YIL=2021 AND HESAPKOD='32503020733'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Aydın Adnan Menderes Üniversitesi ne Hazine Yardımı', HESAPKOD='32503027034' WHERE YIL=2021 AND HESAPKOD='32503020734'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Zonguldak Bülent Ecevit Üniversitesi ne Hazine Yardımı', HESAPKOD='32503027035' WHERE YIL=2021 AND HESAPKOD='32503020735'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Mersin Üniversitesi ne Hazine Yardımı', HESAPKOD='32503027036' WHERE YIL=2021 AND HESAPKOD='32503020736'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Pamukkale Üniversitesi ne Hazine Yardımı', HESAPKOD='32503027037' WHERE YIL=2021 AND HESAPKOD='32503020737'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Balıkesir Üniversitesi ne Hazine Yardımı', HESAPKOD='32503027038' WHERE YIL=2021 AND HESAPKOD='32503020738'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kocaeli Üniversitesi ne Hazine Yardımı', HESAPKOD='32503027039' WHERE YIL=2021 AND HESAPKOD='32503020739'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sakarya Üniversitesi ne Hazine Yardımı', HESAPKOD='32503027040' WHERE YIL=2021 AND HESAPKOD='32503020740'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Manisa Celal Bayar Üniversitesi’ne Hazine Yardımı', HESAPKOD='32503027041' WHERE YIL=2021 AND HESAPKOD='32503020741'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Bolu Abant İzzet Baysal Üniversitesi ne Hazine Yardımı', HESAPKOD='32503027042' WHERE YIL=2021 AND HESAPKOD='32503020742'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Hatay Mustafa Kemal Üniversitesi ne Hazine Yardımı', HESAPKOD='32503027043' WHERE YIL=2021 AND HESAPKOD='32503020743'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Afyon Kocatepe Üniversitesi ne Hazine Yardımı', HESAPKOD='32503027044' WHERE YIL=2021 AND HESAPKOD='32503020744'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kafkas Üniversitesi ne Hazine Yardımı', HESAPKOD='32503027045' WHERE YIL=2021 AND HESAPKOD='32503020745'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Çanakkale 18 Mart Üniversitesi ne Hazine Yardımı', HESAPKOD='32503027046' WHERE YIL=2021 AND HESAPKOD='32503020746'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Niğde Ömer Halisdemir Üniversitesi’ne Hazine Yardımı', HESAPKOD='32503027047' WHERE YIL=2021 AND HESAPKOD='32503020747'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kütahya Dumlupınar Üniversitesi ne Hazine Yardımı', HESAPKOD='32503027048' WHERE YIL=2021 AND HESAPKOD='32503020748'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Tokat Gaziosmanpaşa Üniversitesi ne Hazine Yardımı', HESAPKOD='32503027049' WHERE YIL=2021 AND HESAPKOD='32503020749'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Muğla Sıtkı Koçman Üniversitesi ne Hazine Yardımı', HESAPKOD='32503027050' WHERE YIL=2021 AND HESAPKOD='32503020750'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kahramanmaraş Sütçü İmam Üniversitesi ne Hazine Yardımı', HESAPKOD='32503027051' WHERE YIL=2021 AND HESAPKOD='32503020751'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kırıkkale Üniversitesi ne Hazine Yardımı', HESAPKOD='32503027052' WHERE YIL=2021 AND HESAPKOD='32503020752'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Eskişehir Osmangazi Üniversitesi ne Hazine Yardımı', HESAPKOD='32503027053' WHERE YIL=2021 AND HESAPKOD='32503020753'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Galatasaray Üniversitesi ne Hazine Yardımı', HESAPKOD='32503027054' WHERE YIL=2021 AND HESAPKOD='32503020754'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kırşehir Ahi Evran Üniversitesi ne Hazine Yardımı', HESAPKOD='32503027055' WHERE YIL=2021 AND HESAPKOD='32503020755'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kastamonu Üniversitesi ne Hazine Yardımı', HESAPKOD='32503027056' WHERE YIL=2021 AND HESAPKOD='32503020756'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Düzce Üniversitesi ne Hazine Yardımı', HESAPKOD='32503027057' WHERE YIL=2021 AND HESAPKOD='32503020757'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Burdur Mehmet Akif Ersoy Üniversitesi ne Hazine Yardımı', HESAPKOD='32503027058' WHERE YIL=2021 AND HESAPKOD='32503020758'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Uşak Üniversitesi ne Hazine Yardımı', HESAPKOD='32503027059' WHERE YIL=2021 AND HESAPKOD='32503020759'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Recep Tayyip Erdoğan Üniversitesi ne Hazine Yardımı', HESAPKOD='32503027060' WHERE YIL=2021 AND HESAPKOD='32503020760'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Tekirdağ Namık Kemal Üniversitesi ne Hazine Yardımı', HESAPKOD='32503027061' WHERE YIL=2021 AND HESAPKOD='32503020761'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Erzincan Binali Yıldırım Üniversitesi ne Hazine Yardımı', HESAPKOD='32503027062' WHERE YIL=2021 AND HESAPKOD='32503020762'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Aksaray Üniversitesi ne Hazine Yardımı', HESAPKOD='32503027063' WHERE YIL=2021 AND HESAPKOD='32503020763'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Giresun Üniversitesi ne Hazine Yardımı', HESAPKOD='32503027064' WHERE YIL=2021 AND HESAPKOD='32503020764'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Hitit Üniversitesi ne Hazine Yardımı', HESAPKOD='32503027065' WHERE YIL=2021 AND HESAPKOD='32503020765'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yozgat Bozok Üniversitesi ne Hazine Yardımı', HESAPKOD='32503027066' WHERE YIL=2021 AND HESAPKOD='32503020766'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Adıyaman Üniversitesi ne Hazine Yardımı', HESAPKOD='32503027067' WHERE YIL=2021 AND HESAPKOD='32503020767'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Ordu Üniversitesi ne Hazine Yardımı', HESAPKOD='32503027068' WHERE YIL=2021 AND HESAPKOD='32503020768'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Amasya Üniversitesi ne Hazine Yardımı', HESAPKOD='32503027069' WHERE YIL=2021 AND HESAPKOD='32503020769'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Karamanoğlu Mehmetbey Üniversitesi ne Hazine Yardımı', HESAPKOD='32503027070' WHERE YIL=2021 AND HESAPKOD='32503020770'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Ağrı İbrahim Çeçen Üniversitesi ne Hazine Yardımı', HESAPKOD='32503027071' WHERE YIL=2021 AND HESAPKOD='32503020771'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sinop Üniversitesi ne Hazine Yardımı', HESAPKOD='32503027072' WHERE YIL=2021 AND HESAPKOD='32503020772'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Siirt Üniversitesi ne Hazine Yardımı', HESAPKOD='32503027073' WHERE YIL=2021 AND HESAPKOD='32503020773'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Nevşehir Hacı Bektaş Veli Üniversitesi ne Hazine Yardımı', HESAPKOD='32503027074' WHERE YIL=2021 AND HESAPKOD='32503020774'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Karabük Üniversitesi ne Hazine Yardımı', HESAPKOD='32503027075' WHERE YIL=2021 AND HESAPKOD='32503020775'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kilis 7 Aralık Üniversitesi ne Hazine Yardımı', HESAPKOD='32503027076' WHERE YIL=2021 AND HESAPKOD='32503020776'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Çankırı Karatekin Üniversitesi ne Hazine Yardımı', HESAPKOD='32503027077' WHERE YIL=2021 AND HESAPKOD='32503020777'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Artvin Çoruh Üniversitesi ne Hazine Yardımı', HESAPKOD='32503027078' WHERE YIL=2021 AND HESAPKOD='32503020778'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Bilecik Şeyh Edebali Üniversitesi ne Hazine Yardımı', HESAPKOD='32503027079' WHERE YIL=2021 AND HESAPKOD='32503020779'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Bitlis Eren Üniversitesi ne Hazine Yardımı', HESAPKOD='32503027080' WHERE YIL=2021 AND HESAPKOD='32503020780'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kırklareli Üniversitesi ne Hazine Yardımı', HESAPKOD='32503027081' WHERE YIL=2021 AND HESAPKOD='32503020781'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Osmaniye Korkut Ata Üniversitesi ne Hazine Yardımı', HESAPKOD='32503027082' WHERE YIL=2021 AND HESAPKOD='32503020782'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Bingöl Üniversitesi ne Hazine Yardımı', HESAPKOD='32503027083' WHERE YIL=2021 AND HESAPKOD='32503020783'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Muş Alparslan Üniversitesi ne Hazine Yardımı', HESAPKOD='32503027084' WHERE YIL=2021 AND HESAPKOD='32503020784'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Mardin Artuklu Üniversitesi ne Hazine Yardımı', HESAPKOD='32503027085' WHERE YIL=2021 AND HESAPKOD='32503020785'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Batman Üniversitesi ne Hazine Yardımı', HESAPKOD='32503027086' WHERE YIL=2021 AND HESAPKOD='32503020786'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Ardahan Üniversitesi ne Hazine Yardımı', HESAPKOD='32503027087' WHERE YIL=2021 AND HESAPKOD='32503020787'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Bartın Üniversitesi ne Hazine Yardımı', HESAPKOD='32503027088' WHERE YIL=2021 AND HESAPKOD='32503020788'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Bayburt Üniversitesi ne Hazine Yardımı', HESAPKOD='32503027089' WHERE YIL=2021 AND HESAPKOD='32503020789'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Gümüşhane Üniversitesi ne Hazine Yardımı', HESAPKOD='32503027090' WHERE YIL=2021 AND HESAPKOD='32503020790'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Hakkari Üniversitesi ne Hazine Yardımı', HESAPKOD='32503027091' WHERE YIL=2021 AND HESAPKOD='32503020791'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Iğdır Üniversitesi ne Hazine Yardımı', HESAPKOD='32503027092' WHERE YIL=2021 AND HESAPKOD='32503020792'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Şırnak Üniversitesi ne Hazine Yardımı', HESAPKOD='32503027093' WHERE YIL=2021 AND HESAPKOD='32503020793'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Munzur Üniversitesi ne Hazine Yardımı', HESAPKOD='32503027094' WHERE YIL=2021 AND HESAPKOD='32503020794'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yalova Üniversitesi ne Hazine Yardımı', HESAPKOD='32503027095' WHERE YIL=2021 AND HESAPKOD='32503020795'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Türk-Alman Üniversitesi ne Hazine Yardımı', HESAPKOD='32503027096' WHERE YIL=2021 AND HESAPKOD='32503020796'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Ankara Yıldırım Beyazıt Üniversitesi ne Hazine Yardımı', HESAPKOD='32503027097' WHERE YIL=2021 AND HESAPKOD='32503020797'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Bursa Teknik Üniversitesi ne Hazine Yardımı', HESAPKOD='32503027098' WHERE YIL=2021 AND HESAPKOD='32503020798'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='İstanbul Medeniyet Üniversitesi ne Hazine Yardımı', HESAPKOD='32503027099' WHERE YIL=2021 AND HESAPKOD='32503020799'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yükseköğretim Kurumlarına Hazine Yardımları', HESAPKOD='325030280' WHERE YIL=2021 AND HESAPKOD='325030208'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='İzmir Katip Çelebi Üniversitesi ne Hazine Yardımı', HESAPKOD='32503028001' WHERE YIL=2021 AND HESAPKOD='32503020801'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Necmettin Erbakan Üniversitesi ne Hazine Yardımı', HESAPKOD='32503028002' WHERE YIL=2021 AND HESAPKOD='32503020802'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Abdullah Gül Üniversitesi ne Hazine Yardımı', HESAPKOD='32503028003' WHERE YIL=2021 AND HESAPKOD='32503020803'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Erzurum Teknik Üniversitesi ne Hazine Yardımı', HESAPKOD='32503028004' WHERE YIL=2021 AND HESAPKOD='32503020804'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Adana Alparslan Türkeş Bilim ve Teknoloji Üniversitesi ne Hazine Yardımı', HESAPKOD='32503028005' WHERE YIL=2021 AND HESAPKOD='32503020805'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Ankara Sosyal Bilimler Üniversitesi ne Hazine Yardımı', HESAPKOD='32503028006' WHERE YIL=2021 AND HESAPKOD='32503020806'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sağlık Bilimleri Üniversitesi ne Hazine Yardımı', HESAPKOD='32503028007' WHERE YIL=2021 AND HESAPKOD='32503020807'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Bandırma Onyedi Eylül Üniversitesi ne Hazine Yardımı', HESAPKOD='32503028008' WHERE YIL=2021 AND HESAPKOD='32503020808'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='İskenderun Teknik Üniversitesi ne Hazine Yardımı', HESAPKOD='32503028009' WHERE YIL=2021 AND HESAPKOD='32503020809'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Alanya Alaaddin Keykubat Üniversitesi ne Hazine Yardımı', HESAPKOD='32503028010' WHERE YIL=2021 AND HESAPKOD='32503020810'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Türkiye Uluslararası İslam, Bilim ve Teknoloji Üniversitesi ne Hazine Yardımı', HESAPKOD='32503028011' WHERE YIL=2021 AND HESAPKOD='32503020811'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='İzmir Bakırçay Üniversitesi ne Hazine Yardımı', HESAPKOD='32503028012' WHERE YIL=2021 AND HESAPKOD='32503020812'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='İzmir Demokrasi Üniversitesi ne Hazine Yardımı', HESAPKOD='32503028013' WHERE YIL=2021 AND HESAPKOD='32503020813'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yükseköğretim Kalite Kurulu na Hazine Yardımı', HESAPKOD='32503028014' WHERE YIL=2021 AND HESAPKOD='32503020814'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Ankara Müzik ve Güzel Sanatlar Üniversitesi ne Hazine Yardımı', HESAPKOD='32503028015' WHERE YIL=2021 AND HESAPKOD='32503020815'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Gaziantep İslam Bilim ve Teknoloji Üniversitesi ne Hazine Yardımı', HESAPKOD='32503028016' WHERE YIL=2021 AND HESAPKOD='32503020816'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Konya Teknik Üniversitesi ne Hazine Yardımı', HESAPKOD='32503028017' WHERE YIL=2021 AND HESAPKOD='32503020817'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kütahya Sağlık Bilimleri Üniversitesi ne Hazine Yardımı', HESAPKOD='32503028018' WHERE YIL=2021 AND HESAPKOD='32503020818'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Malatya Turgut Özal Üniversitesi ne Hazine Yardımı', HESAPKOD='32503028019' WHERE YIL=2021 AND HESAPKOD='32503020819'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='İstanbul Üniversitesi-Cerrahpaşa ya Hazine Yardımı', HESAPKOD='32503028020' WHERE YIL=2021 AND HESAPKOD='32503020820'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Ankara Hacı Bayram Veli Üniversitesi ne Hazine Yardımı', HESAPKOD='32503028021' WHERE YIL=2021 AND HESAPKOD='32503020821'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sakarya Uygulamalı Bilimler Üniversitesi ne Hazine Yardımı', HESAPKOD='32503028022' WHERE YIL=2021 AND HESAPKOD='32503020822'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Samsun Üniversitesi ne Hazine Yardımı', HESAPKOD='32503028023' WHERE YIL=2021 AND HESAPKOD='32503020823'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sivas Bilim ve Teknoloji Üniversitesi ne Hazine Yardımı', HESAPKOD='32503028024' WHERE YIL=2021 AND HESAPKOD='32503020824'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Tarsus Üniversitesi ne Hazine Yardımı', HESAPKOD='32503028025' WHERE YIL=2021 AND HESAPKOD='32503020825'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Trabzon Üniversitesi ne Hazine Yardımı', HESAPKOD='32503028026' WHERE YIL=2021 AND HESAPKOD='32503020826'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kayseri Üniversitesi ne Hazine Yardımı', HESAPKOD='32503028027' WHERE YIL=2021 AND HESAPKOD='32503020827'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kahramanmaraş İstiklal Üniversitesi ne Hazine Yardımı', HESAPKOD='32503028028' WHERE YIL=2021 AND HESAPKOD='32503020828'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Eskişehir Teknik Üniversitesi ne Hazine Yardımı', HESAPKOD='32503028029' WHERE YIL=2021 AND HESAPKOD='32503020829'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Isparta Uygulamalı Bilimler Üniversitesi ne Hazine Yardımı', HESAPKOD='32503028030' WHERE YIL=2021 AND HESAPKOD='32503020830'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Afyonkarahisar Sağlık Bilimleri Üniversitesi ne Hazine Yardımı', HESAPKOD='32503028031' WHERE YIL=2021 AND HESAPKOD='32503020831'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sınıflandırmaya Girmeyen Diğer Hazine Yardımları', HESAPKOD='32503029090' WHERE YIL=2021 AND HESAPKOD='32503020880'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Hazine Yardımları', HESAPKOD='325030290' WHERE YIL=2021 AND HESAPKOD='325030209'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Gençlik ve Spor İl Müdürlükleri ne Hazine Yardımı', HESAPKOD='32503029001' WHERE YIL=2021 AND HESAPKOD='32503020911'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Merkezi Finans ve İhale Birimi ne Hazine Yardımı', HESAPKOD='32503029002' WHERE YIL=2021 AND HESAPKOD='32503020912'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sınıflandırmaya Girmeyen Diğer Hazine Yardımları', HESAPKOD='32503029090' WHERE YIL=2021 AND HESAPKOD='32503020913'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Destekleme ve Fiyat İstikrar Fonu na Hazine Yardımı', HESAPKOD='32503029003' WHERE YIL=2021 AND HESAPKOD='32503020914'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='İşsizlik Sigortası Fonu na Hazine Yardımı', HESAPKOD='32503029004' WHERE YIL=2021 AND HESAPKOD='32503020915'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sınıflandırmaya Girmeyen Diğer Hazine Yardımları', HESAPKOD='32503029090' WHERE YIL=2021 AND HESAPKOD='32503020916'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yatırım Ofisi’ne Hazine Yardımı', HESAPKOD='32503029005' WHERE YIL=2021 AND HESAPKOD='32503020917'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Tarım ve Kırsal Kalkınmayı Destekleme Kurumu na Hazine Yardımı', HESAPKOD='32503029009' WHERE YIL=2021 AND HESAPKOD='32503020918'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='İller Bankası AŞ ye Hazine Yardımı', HESAPKOD='32503029010' WHERE YIL=2021 AND HESAPKOD='32503020919'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Üniversite Döner Sermaye Bütçelerine Hazine Yardımı', HESAPKOD='32503029011' WHERE YIL=2021 AND HESAPKOD='32503020920'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Çanakkale Savaşları Gelibolu Tarihi Alan Başkanlığı’na Hazine Yardımı', HESAPKOD='32503029012' WHERE YIL=2021 AND HESAPKOD='32503020921'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sınıflandırmaya Girmeyen Diğer Hazine Yardımları', HESAPKOD='32503029090' WHERE YIL=2021 AND HESAPKOD='32503020990'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kar Amacı Gütmeyen Kuruluşlara Yapılan Transferler', HESAPKOD='3250303' WHERE YIL=2021 AND HESAPKOD='3250303'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kar Amacı Gütmeyen Kuruluşlara', HESAPKOD='325030310' WHERE YIL=2021 AND HESAPKOD='325030301'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Dernek, Birlik, Kurum, Kuruluş, Sandık vb Kuruluşlara', HESAPKOD='32503031001' WHERE YIL=2021 AND HESAPKOD='32503030101'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Vakıf Üniversitelerine', HESAPKOD='32503031002' WHERE YIL=2021 AND HESAPKOD='32503030102'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kamu İşveren Sendikalarına', HESAPKOD='32503031003' WHERE YIL=2021 AND HESAPKOD='32503030103'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Siyasi Partilere', HESAPKOD='32503031004' WHERE YIL=2021 AND HESAPKOD='32503030104'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Memurların Öğle Yemeğine Yardım', HESAPKOD='32503031005' WHERE YIL=2021 AND HESAPKOD='32503030105'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sosyal Amaçlı Fonlara', HESAPKOD='32503031006' WHERE YIL=2021 AND HESAPKOD='32503030106'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Seçim Giderleri İçin Yapılacak Ödemeler', HESAPKOD='32503031007' WHERE YIL=2021 AND HESAPKOD='32503030107'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Türk Kültür Varlığının Korunması ve Tanıtımı Giderleri', HESAPKOD='32503031008' WHERE YIL=2021 AND HESAPKOD='32503030108'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Siyasi Tanıtma Giderleri', HESAPKOD='32503031009' WHERE YIL=2021 AND HESAPKOD='32503030109'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Mesleki Yeterlilik Kurumuna Yapılacak Ödemeler', HESAPKOD='32503031010' WHERE YIL=2021 AND HESAPKOD='32503030110'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Bilimsel Araştırma Projelerinin Desteklenmesine İlişkin Giderler', HESAPKOD='32503031011' WHERE YIL=2021 AND HESAPKOD='32503030111'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Avrupa Birliği Eğitim ve Gençlik Programları Merkezi Başkanlığına', HESAPKOD='32503031012' WHERE YIL=2021 AND HESAPKOD='32503030112'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Bağımsız Spor Federasyonlarına', HESAPKOD='32503031013' WHERE YIL=2021 AND HESAPKOD='32503030113'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Afet Riski Altındaki Alanların Dönüşüm Giderleri', HESAPKOD='32503031014' WHERE YIL=2021 AND HESAPKOD='32503030114'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Türkiye Maarif Vakfına', HESAPKOD='32503031015' WHERE YIL=2021 AND HESAPKOD='32503030115'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Ulusal Fona', HESAPKOD='32503031080' WHERE YIL=2021 AND HESAPKOD='32503030180'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kar Amacı Gütmeyen Diğer Kuruluşlara', HESAPKOD='32503031090' WHERE YIL=2021 AND HESAPKOD='32503030190'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Hanehalkı ve İşletmelere Yapılan Transferler', HESAPKOD='3250304' WHERE YIL=2021 AND HESAPKOD='3250304'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Burslar ve Harçlıklar', HESAPKOD='325030410' WHERE YIL=2021 AND HESAPKOD='325030401'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yurtiçi Burslar ve Harçlıklar', HESAPKOD='32503041001' WHERE YIL=2021 AND HESAPKOD='32503040101'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yurtdışı Burslar', HESAPKOD='32503041002' WHERE YIL=2021 AND HESAPKOD='32503040102'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yabancı Uyruklu Kişilere Burslar ve Harçlıklar', HESAPKOD='32503041003' WHERE YIL=2021 AND HESAPKOD='32503040103'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yurtiçi Burslar ve Harçlıklar', HESAPKOD='32503041001' WHERE YIL=2021 AND HESAPKOD='32503040190'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Eğitim Amaçlı Diğer Transferler', HESAPKOD='325030420' WHERE YIL=2021 AND HESAPKOD='325030402'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Parasız Yatılı Öğrenci Pansiyon Ücretleri', HESAPKOD='32503042001' WHERE YIL=2021 AND HESAPKOD='32503040201'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yabancı Uyruklu Kişilere Eğitim Amaçlı Transferler', HESAPKOD='32503042002' WHERE YIL=2021 AND HESAPKOD='32503040202'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sağlık Amaçlı Transferler', HESAPKOD='325030430' WHERE YIL=2021 AND HESAPKOD='325030403'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sağlık Amaçlı Transferler', HESAPKOD='32503043001' WHERE YIL=2021 AND HESAPKOD='32503040301'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sosyal Güvenliği Bulunmayanların Prim Ödemelerine İlişkin Transferler  ', HESAPKOD='32503043002' WHERE YIL=2021 AND HESAPKOD='32503040302'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yiyecek Amaçlı Transferler', HESAPKOD='325030440' WHERE YIL=2021 AND HESAPKOD='325030404'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yiyecek Amaçlı Transferler', HESAPKOD='32503044001' WHERE YIL=2021 AND HESAPKOD='32503040401'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Barınma Amaçlı Transferler', HESAPKOD='325030450' WHERE YIL=2021 AND HESAPKOD='325030405'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Koruyucu Ailelere Yardım Ödemeleri', HESAPKOD='32503045001' WHERE YIL=2021 AND HESAPKOD='32503040501'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Barınma Amaçlı Transferler', HESAPKOD='32503045090' WHERE YIL=2021 AND HESAPKOD='32503040590'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Alan Bazlı Tarımsal Destekler', HESAPKOD='325030460' WHERE YIL=2021 AND HESAPKOD='325030406'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Doğrudan Gelir Desteği Ödemeleri', HESAPKOD='32503046912' WHERE YIL=2021 AND HESAPKOD='32503040601'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Ürün Destekleme Ödemeleri', HESAPKOD='32503046913' WHERE YIL=2021 AND HESAPKOD='32503040602'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Hayvancılık Destekleme Ödemeleri', HESAPKOD='32503046914' WHERE YIL=2021 AND HESAPKOD='32503040603'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Tarım Reformu Uygulama Projesi', HESAPKOD='32503046915' WHERE YIL=2021 AND HESAPKOD='32503040604'
DELETE FROM  MUHHESAPPLANI WHERE YIL=2021 AND HESAPKOD='32503040605'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Çevre Amaçlı Tarımsal Alanların Korunması (ÇATAK) Desteği', HESAPKOD='32503046902' WHERE YIL=2021 AND HESAPKOD='32503040606'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='İlave Doğrudan Gelir Desteği Ödemeleri', HESAPKOD='32503046916' WHERE YIL=2021 AND HESAPKOD='32503040607'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Mazot Desteği', HESAPKOD='32503046001' WHERE YIL=2021 AND HESAPKOD='32503040608'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Gübre Desteği', HESAPKOD='32503046002' WHERE YIL=2021 AND HESAPKOD='32503040609'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Alan Bazlı Tarımsal Destekler', HESAPKOD='32503046090' WHERE YIL=2021 AND HESAPKOD='32503040610'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kütlü Pamuk Desteği', HESAPKOD='32503046501' WHERE YIL=2021 AND HESAPKOD='32503040611'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yağlık Ayçiçeği Desteği', HESAPKOD='32503046502' WHERE YIL=2021 AND HESAPKOD='32503040612'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Soya Fasülyesi Desteği', HESAPKOD='32503046503' WHERE YIL=2021 AND HESAPKOD='32503040613'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kanola Desteği', HESAPKOD='32503046504' WHERE YIL=2021 AND HESAPKOD='32503040614'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Dane Mısır Desteği', HESAPKOD='32503046506' WHERE YIL=2021 AND HESAPKOD='32503040615'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Zeytinyağı Desteği', HESAPKOD='32503046507' WHERE YIL=2021 AND HESAPKOD='32503040616'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Aspir Desteği', HESAPKOD='32503046505' WHERE YIL=2021 AND HESAPKOD='32503040617'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Çay Desteği', HESAPKOD='32503046508' WHERE YIL=2021 AND HESAPKOD='32503040618'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Buğday Desteği', HESAPKOD='32503046509' WHERE YIL=2021 AND HESAPKOD='32503040619'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Çeltik Desteği', HESAPKOD='32503046510' WHERE YIL=2021 AND HESAPKOD='32503040620'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Hububat Destekleri', HESAPKOD='32503046590' WHERE YIL=2021 AND HESAPKOD='32503040621'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Fark Ödemesi Niteliğindeki Diğer Tarımsal Destekler', HESAPKOD='32503046591' WHERE YIL=2021 AND HESAPKOD='32503040622'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yem Bitkileri Üretimi Desteği', HESAPKOD='32503046103' WHERE YIL=2021 AND HESAPKOD='32503040623'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Alet ve Makine Desteği', HESAPKOD='32503046119' WHERE YIL=2021 AND HESAPKOD='32503040624'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sertifikalı Yem Bitkileri Tohumluğu Üretim Desteği', HESAPKOD='32503046120' WHERE YIL=2021 AND HESAPKOD='32503040625'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Damızlık Büyükbaş Hayvan (Düve) Desteği', HESAPKOD='32503046121' WHERE YIL=2021 AND HESAPKOD='32503040626'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Suni Tohumlama Desteği', HESAPKOD='32503046122' WHERE YIL=2021 AND HESAPKOD='32503040627'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Çiğ Süt Desteği', HESAPKOD='32503046105' WHERE YIL=2021 AND HESAPKOD='32503040628'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Et Desteği', HESAPKOD='32503046123' WHERE YIL=2021 AND HESAPKOD='32503040629'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Buzağı Desteği', HESAPKOD='32503046106' WHERE YIL=2021 AND HESAPKOD='32503040630'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Arıcılık ve Bal Desteği', HESAPKOD='32503046107' WHERE YIL=2021 AND HESAPKOD='32503040631'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Hastalıklardan Ari İşletme Desteği', HESAPKOD='32503046108' WHERE YIL=2021 AND HESAPKOD='32503040632'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Aşı Desteği', HESAPKOD='32503046109' WHERE YIL=2021 AND HESAPKOD='32503040633'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Su Ürünleri Desteği', HESAPKOD='32503046117' WHERE YIL=2021 AND HESAPKOD='32503040634'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sağım Hijyeni ve Süt Kalitesi Desteği', HESAPKOD='32503046124' WHERE YIL=2021 AND HESAPKOD='32503040635'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Islah Amaçlı Küçükbaş Hayvan Yetiştirici Birlikleri Desteği', HESAPKOD='32503046110' WHERE YIL=2021 AND HESAPKOD='32503040636'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Hayvan Gen Kaynakları Desteği', HESAPKOD='32503046118' WHERE YIL=2021 AND HESAPKOD='32503040637'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Tiftik Üretimi Desteği', HESAPKOD='32503046111' WHERE YIL=2021 AND HESAPKOD='32503040638'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='İpek Böceği Desteği', HESAPKOD='32503046112' WHERE YIL=2021 AND HESAPKOD='32503040639'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Gıda Güvenliği Desteği', HESAPKOD='32503046125' WHERE YIL=2021 AND HESAPKOD='32503040640'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Hayvan Kimlik Sistemi Desteği', HESAPKOD='32503046126' WHERE YIL=2021 AND HESAPKOD='32503040641'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kanatlı Hayvan Hastalıkları Tazminatı Desteği', HESAPKOD='32503046127' WHERE YIL=2021 AND HESAPKOD='32503040642'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Büyükbaş Hayvan Hastalıkları Tazminatı Desteği', HESAPKOD='32503046101' WHERE YIL=2021 AND HESAPKOD='32503040643'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Büyükbaş Hayvan Desteği', HESAPKOD='32503046128' WHERE YIL=2021 AND HESAPKOD='32503040644'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Çevre Amaçlı Tarımsal Alanların Korunması (ÇATAK) Desteği', HESAPKOD='32503046902' WHERE YIL=2021 AND HESAPKOD='32503040645'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Alternatif Ürün Desteği', HESAPKOD='32503046012' WHERE YIL=2021 AND HESAPKOD='32503040646'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sınıflandırmaya Girmeyen Diğer Tarımsal Destekler', HESAPKOD='32503046990' WHERE YIL=2021 AND HESAPKOD='32503040647'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sınıflandırmaya Girmeyen Diğer Tarımsal Destekler', HESAPKOD='32503046990' WHERE YIL=2021 AND HESAPKOD='32503040648'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kırsal Kalkınma Destekleri', HESAPKOD='32503046401' WHERE YIL=2021 AND HESAPKOD='32503040649'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Bitkisel Ürün Sigortası Desteği', HESAPKOD='32503046201' WHERE YIL=2021 AND HESAPKOD='32503040650'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Süt Sığırı Sigortası Desteği', HESAPKOD='32503046202' WHERE YIL=2021 AND HESAPKOD='32503040651'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sera Sigortası Desteği', HESAPKOD='32503046203' WHERE YIL=2021 AND HESAPKOD='32503040652'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kümes Hayvanları Sigortası Desteği', HESAPKOD='32503046204' WHERE YIL=2021 AND HESAPKOD='32503040653'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Hasar Fazlası Desteği', HESAPKOD='32503046205' WHERE YIL=2021 AND HESAPKOD='32503040654'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Tarım Sigortası Destekleri', HESAPKOD='32503046290' WHERE YIL=2021 AND HESAPKOD='32503040655'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Patates Siğili Desteği', HESAPKOD='32503046301' WHERE YIL=2021 AND HESAPKOD='32503040656'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Çay Budama Tazminatı Desteği', HESAPKOD='32503046303' WHERE YIL=2021 AND HESAPKOD='32503040657'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Çay Budama Masraflarını Karşılama Desteği', HESAPKOD='32503046304' WHERE YIL=2021 AND HESAPKOD='32503040658'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sınıflandırmaya Girmeyen Diğer Tarımsal Destekler', HESAPKOD='32503046990' WHERE YIL=2021 AND HESAPKOD='32503040659'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Tarımsal Ar-Ge Destekleri', HESAPKOD='32503046910' WHERE YIL=2021 AND HESAPKOD='32503040660'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sertifikalı Tohum Kullanım Desteği', HESAPKOD='32503046903' WHERE YIL=2021 AND HESAPKOD='32503040661'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sertifikalı Fidan Kullanım Desteği', HESAPKOD='32503046904' WHERE YIL=2021 AND HESAPKOD='32503040662'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sınıflandırmaya Girmeyen Diğer Tarımsal Destekler', HESAPKOD='32503046990' WHERE YIL=2021 AND HESAPKOD='32503040663'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sınıflandırmaya Girmeyen Diğer Tarımsal Destekler', HESAPKOD='32503046990' WHERE YIL=2021 AND HESAPKOD='32503040664'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Çiftlik Muhasebe Veri Ağı (ÇMVA) Katılım Desteği', HESAPKOD='32503046907' WHERE YIL=2021 AND HESAPKOD='32503040665'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kuraklık Desteği', HESAPKOD='32503046013' WHERE YIL=2021 AND HESAPKOD='32503040666'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Toprak Analizi Desteği', HESAPKOD='32503046007' WHERE YIL=2021 AND HESAPKOD='32503040667'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Organik Tarım Desteği', HESAPKOD='32503046005' WHERE YIL=2021 AND HESAPKOD='32503040668'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='İyi Tarım Uygulamaları Desteği', HESAPKOD='32503046006' WHERE YIL=2021 AND HESAPKOD='32503040669'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Bakliyat Desteği', HESAPKOD='32503046511' WHERE YIL=2021 AND HESAPKOD='32503040670'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Büyükbaş Hayvan Desteği', HESAPKOD='32503046128' WHERE YIL=2021 AND HESAPKOD='32503040671'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sınıflandırmaya Girmeyen Diğer Tarımsal Destekler', HESAPKOD='32503046990' WHERE YIL=2021 AND HESAPKOD='32503040672'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Su Ürünleri Sigortası Desteği', HESAPKOD='32503046206' WHERE YIL=2021 AND HESAPKOD='32503040673'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sertifikalı Tohum Üretim Desteği', HESAPKOD='32503046905' WHERE YIL=2021 AND HESAPKOD='32503040674'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Tarım Danışmanlığı Desteği', HESAPKOD='32503046901' WHERE YIL=2021 AND HESAPKOD='32503040675'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Bombus Arısı Desteği', HESAPKOD='32503046104' WHERE YIL=2021 AND HESAPKOD='32503040676'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Fındık Üreticileri Alan Bazlı Gelir Desteği', HESAPKOD='32503046004' WHERE YIL=2021 AND HESAPKOD='32503040677'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Fındık Alternatif Ürün Desteği', HESAPKOD='32503046010' WHERE YIL=2021 AND HESAPKOD='32503040678'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Fındık Söküm Tazminatı', HESAPKOD='32503046011' WHERE YIL=2021 AND HESAPKOD='32503040679'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sınıflandırmaya Girmeyen Diğer Tarımsal Destekler', HESAPKOD='32503046990' WHERE YIL=2021 AND HESAPKOD='32503040680'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Büyükbaş Hayvan Besi Desteği', HESAPKOD='32503046113' WHERE YIL=2021 AND HESAPKOD='32503040681'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Küçükbaş Hayvan Sigortası Desteği', HESAPKOD='32503046207' WHERE YIL=2021 AND HESAPKOD='32503040682'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Süt Regülasyonu Desteği', HESAPKOD='32503046114' WHERE YIL=2021 AND HESAPKOD='32503040683'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Biyolojik Mücadele Desteği', HESAPKOD='32503046102' WHERE YIL=2021 AND HESAPKOD='32503040684'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='IPARD Ulusal Eş Finansman Katkı Payı Ödemeleri', HESAPKOD='32503046402' WHERE YIL=2021 AND HESAPKOD='32503040685'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sürü Yöneticisi İstihdamı Desteği', HESAPKOD='32503046115' WHERE YIL=2021 AND HESAPKOD='32503040686'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Lisanslı Depolarda Ürün Depolama Kira Desteği', HESAPKOD='32503046908' WHERE YIL=2021 AND HESAPKOD='32503040687'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Beş Dekar Altındaki Küçük İşletme Desteği', HESAPKOD='32503046008' WHERE YIL=2021 AND HESAPKOD='32503040688'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Genç Çiftçilere Proje Desteği', HESAPKOD='32503046403' WHERE YIL=2021 AND HESAPKOD='32503040689'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sınıflandırmaya Girmeyen Diğer Tarımsal Destekler', HESAPKOD='32503046990' WHERE YIL=2021 AND HESAPKOD='32503040690'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Büyükbaş Damızlık Hayvan Alım Desteği', HESAPKOD='32503046116' WHERE YIL=2021 AND HESAPKOD='32503040693'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sertifikalı Fidan Üretim Desteği', HESAPKOD='32503046906' WHERE YIL=2021 AND HESAPKOD='32503040694'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Geleneksel Zeytin Bahçelerinin Rehabilitasyonu Desteği', HESAPKOD='32503046009' WHERE YIL=2021 AND HESAPKOD='32503040695'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Tarımsal Sulama Elektrik Desteği', HESAPKOD='32503046909' WHERE YIL=2021 AND HESAPKOD='32503040696'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Bitki Karantinası Tazminatı Desteği', HESAPKOD='32503046302' WHERE YIL=2021 AND HESAPKOD='32503040698'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sosyal Amaçlı Transferler', HESAPKOD='325030470' WHERE YIL=2021 AND HESAPKOD='325030407'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Muhtaç ve Engellilere Yardım', HESAPKOD='32503047001' WHERE YIL=2021 AND HESAPKOD='32503040701'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Vakıf İlgililerine Yapılan Ödemeler', HESAPKOD='32503047002' WHERE YIL=2021 AND HESAPKOD='32503040702'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Vatani Hizmet Aylıkları', HESAPKOD='32503047003' WHERE YIL=2021 AND HESAPKOD='32503040703'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sosyal Amaçlı Diğer Aylık ve Tazminatlar', HESAPKOD='32503047004' WHERE YIL=2021 AND HESAPKOD='32503040704'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Şehit Dul ve Yetimlere İkramiye ve Ek Ödeme', HESAPKOD='32503047005' WHERE YIL=2021 AND HESAPKOD='32503040705'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Sosyal Amaçlı Transferler', HESAPKOD='32503047090' WHERE YIL=2021 AND HESAPKOD='32503040790'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Ekonomik/Mali Amaçlı Transferler', HESAPKOD='325030480' WHERE YIL=2021 AND HESAPKOD='325030408'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Ekonomik/Mali Amaçlı Diğer Transferler', HESAPKOD='32503048090' WHERE YIL=2021 AND HESAPKOD='32503040801'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Hane Halkına ve İşletmelere Yapılan Diğer Transferler', HESAPKOD='325030490' WHERE YIL=2021 AND HESAPKOD='325030409'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sınıflandırmaya Girmeyen Hane Halkı ve İşletmelere Yapılan Diğer Transferler', HESAPKOD='32503049090' WHERE YIL=2021 AND HESAPKOD='32503040901'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Türk Kültür Varlığının Korunması ve Tanıtımı Giderleri', HESAPKOD='32503049001' WHERE YIL=2021 AND HESAPKOD='32503040908'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Siyasi Tanıtma Giderleri', HESAPKOD='32503049002' WHERE YIL=2021 AND HESAPKOD='32503040909'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sağlık Personelinin Mali Sorumluluk Sigortası Giderleri ', HESAPKOD='32503049003' WHERE YIL=2021 AND HESAPKOD='32503040910'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yurtdışına Yapılan Transferler', HESAPKOD='3250305' WHERE YIL=2021 AND HESAPKOD='3250305'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Dış Ülkelere Yapılan Yardımlar', HESAPKOD='325030510' WHERE YIL=2021 AND HESAPKOD='325030501'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kuzey Kıbrıs Türk Cumhuriyeti ne', HESAPKOD='32503051001' WHERE YIL=2021 AND HESAPKOD='32503050101'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Ülkelere', HESAPKOD='32503051090' WHERE YIL=2021 AND HESAPKOD='32503050190'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Uluslararası Kuruluşlara Yapılan Ödemeler', HESAPKOD='325030520' WHERE YIL=2021 AND HESAPKOD='325030502'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Uluslararası Kuruluşlara Üyelik Aidatı Ödemeleri', HESAPKOD='32503052001' WHERE YIL=2021 AND HESAPKOD='32503050201'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Uluslararası Kuruluşlara Katkı Ödemeleri', HESAPKOD='32503052002' WHERE YIL=2021 AND HESAPKOD='32503050202'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Uluslararası Kuruluşlara Yardım Ödemeleri', HESAPKOD='32503052003' WHERE YIL=2021 AND HESAPKOD='32503050203'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Uluslararası Kuruluşlara Diğer Ödemeler', HESAPKOD='32503052090' WHERE YIL=2021 AND HESAPKOD='32503050290'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Uluslarüstü Kuruluşlara', HESAPKOD='325030530' WHERE YIL=2021 AND HESAPKOD='325030503'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Uluslarüstü Kuruluşlara', HESAPKOD='32503053001' WHERE YIL=2021 AND HESAPKOD='32503050301'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Uluslarüstü Kuruluşlardan Merkezlerine ', HESAPKOD='325030540' WHERE YIL=2021 AND HESAPKOD='325030504'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Uluslarüstü Kuruluşlardan Merkezlerine', HESAPKOD='32503054001' WHERE YIL=2021 AND HESAPKOD='32503050401'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Eğitim Kurumlarına', HESAPKOD='325030550' WHERE YIL=2021 AND HESAPKOD='325030505'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Hoca Ahmet Yesevi Uluslararası Türk Kazak Üniversitesine', HESAPKOD='32503055001' WHERE YIL=2021 AND HESAPKOD='32503050501'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kırgızistan-Türkiye Manas Üniversitesine', HESAPKOD='32503055002' WHERE YIL=2021 AND HESAPKOD='32503050502'
DELETE FROM  MUHHESAPPLANI WHERE YIL=2021 AND HESAPKOD='32503050503'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yurtdışına Yapılan Diğer Transferler', HESAPKOD='325030590' WHERE YIL=2021 AND HESAPKOD='325030509'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sınıflandırmaya Girmeyen Diğer Yurtdışı Transferler', HESAPKOD='32503059090' WHERE YIL=2021 AND HESAPKOD='32503050901'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Türk Kültür Varlığının Korunması ve Tanıtımı Giderleri', HESAPKOD='32503059001' WHERE YIL=2021 AND HESAPKOD='32503050908'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Siyasi Tanıtma Giderleri', HESAPKOD='32503059002' WHERE YIL=2021 AND HESAPKOD='32503050909'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kurum, İşletme ve Hanehalkına Yapılan Sermaye Transferleri', HESAPKOD='3250310' WHERE YIL=2021 AND HESAPKOD='3250310'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Özel Bütçeli İdarelere Hazine Yardımları', HESAPKOD='325031220' WHERE YIL=2021 AND HESAPKOD='325031002'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Ölçme, Seçme ve Yerleştirme Merkezi Başkanlığı na Hazine Yardımı', HESAPKOD='32503122001' WHERE YIL=2021 AND HESAPKOD='32503100201'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Atatürk Kültür, Dil ve Tarih Yüksek Kurumu na Hazine Yardımı', HESAPKOD='32503122002' WHERE YIL=2021 AND HESAPKOD='32503100202'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Atatürk Araştırma Merkezi ne Hazine Yardımı', HESAPKOD='32503122003' WHERE YIL=2021 AND HESAPKOD='32503100203'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Atatürk Kültür Merkezi ne Hazine Yardımı', HESAPKOD='32503122004' WHERE YIL=2021 AND HESAPKOD='32503100204'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Türk Dil Kurumu na Hazine Yardımı', HESAPKOD='32503122005' WHERE YIL=2021 AND HESAPKOD='32503100205'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Türk Tarih Kurumu na Hazine Yardımı', HESAPKOD='32503122006' WHERE YIL=2021 AND HESAPKOD='32503100206'
DELETE FROM  MUHHESAPPLANI WHERE YIL=2021 AND HESAPKOD='32503100207'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Türkiye Bilimsel ve Teknolojik Araştırma Kurumu Başkanlığı na Hazine Yardımı', HESAPKOD='32503122007' WHERE YIL=2021 AND HESAPKOD='32503100208'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Türkiye Bilimler Akademisi Başkanlığı na Hazine Yardımı', HESAPKOD='32503122008' WHERE YIL=2021 AND HESAPKOD='32503100209'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Türkiye Adalet Akademisi’ne Hazine Yardımı', HESAPKOD='32503122009' WHERE YIL=2021 AND HESAPKOD='32503100210'
DELETE FROM  MUHHESAPPLANI WHERE YIL=2021 AND HESAPKOD='32503100213'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sınıflandırmaya Girmeyen Diğer Hazine Yardımları', HESAPKOD='32503129090' WHERE YIL=2021 AND HESAPKOD='32503100214'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Devlet Tiyatroları Genel Müdürlüğü ne Hazine Yardımı', HESAPKOD='32503122010' WHERE YIL=2021 AND HESAPKOD='32503100215'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Devlet Opera ve Balesi Genel Müdürlüğü ne Hazine Yardımı', HESAPKOD='32503122011' WHERE YIL=2021 AND HESAPKOD='32503100216'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Orman Genel Müdürlüğü ne Hazine Yardımı', HESAPKOD='32503122012' WHERE YIL=2021 AND HESAPKOD='32503100217'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Vakıflar Genel Müdürlüğü ne Hazine Yardımı', HESAPKOD='32503122013' WHERE YIL=2021 AND HESAPKOD='32503100218'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Türkiye Hudut ve Sahiller Sağlık Genel Müdürlüğü ne Hazine Yardımı', HESAPKOD='32503122014' WHERE YIL=2021 AND HESAPKOD='32503100219'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Türk Akreditasyon Kurumu na Hazine Yardımı', HESAPKOD='32503122015' WHERE YIL=2021 AND HESAPKOD='32503100221'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Türk Standartları Enstitüsü ne Hazine Yardımı', HESAPKOD='32503122016' WHERE YIL=2021 AND HESAPKOD='32503100222'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Türk Patent Enstitüsü ne Hazine Yardımı', HESAPKOD='32503122017' WHERE YIL=2021 AND HESAPKOD='32503100224'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Ulusal Bor Araştırma Enstitüsü ne Hazine Yardımı', HESAPKOD='32503122018' WHERE YIL=2021 AND HESAPKOD='32503100226'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Türkiye Atom Enerjisi Kurumu na Hazine Yardımı', HESAPKOD='32503122019' WHERE YIL=2021 AND HESAPKOD='32503100227'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Kamu Kurum ve Kuruluşlarına', HESAPKOD='32503109003' WHERE YIL=2021 AND HESAPKOD='32503100228'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Küçük ve Orta Ölçekli İşletmeleri Geliştirme ve Destekleme İdaresi Başkanlığı na Hazine Yardımı', HESAPKOD='32503122021' WHERE YIL=2021 AND HESAPKOD='32503100230'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Türk İşbirliği ve Koordinasyon Ajansı Başkanlığı na Hazine Yardımı', HESAPKOD='32503122022' WHERE YIL=2021 AND HESAPKOD='32503100232'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='GAP Bölge Kalkınma İdaresi Başkanlığı na Hazine Yardımı', HESAPKOD='32503122023' WHERE YIL=2021 AND HESAPKOD='32503100234'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Özelleştirme İdaresi Başkanlığı na Hazine Yardımı', HESAPKOD='32503122024' WHERE YIL=2021 AND HESAPKOD='32503100235'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Maden Tetkik ve Arama Genel Müdürlüğü ne Hazine Yardımı', HESAPKOD='32503122025' WHERE YIL=2021 AND HESAPKOD='32503100240'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Ceza İnfaz Kurumları ile Tutukevleri İş Yurtları Kurumu na  Hazine Yardımı', HESAPKOD='32503122026' WHERE YIL=2021 AND HESAPKOD='32503100241'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sivil Havacılık Genel Müdürlüğü ne Hazine Yardımı', HESAPKOD='32503122027' WHERE YIL=2021 AND HESAPKOD='32503100249'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yurtdışı Türkler ve Akraba Topluluklar Başkanlığına Hazine Yardımı', HESAPKOD='32503122029' WHERE YIL=2021 AND HESAPKOD='32503100251'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Karayolları Genel Müdürlüğü ne Hazine Yardımı', HESAPKOD='32503122030' WHERE YIL=2021 AND HESAPKOD='32503100252'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Türkiye Yazma Eserler Kurumu Başkanlığı na Hazine Yardımı', HESAPKOD='32503122031' WHERE YIL=2021 AND HESAPKOD='32503100253'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Doğu Anadolu Projesi Bölge Kalkınma İdaresi Başkanlığı na Hazine Yardımı', HESAPKOD='32503122032' WHERE YIL=2021 AND HESAPKOD='32503100254'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Konya Ovası Projesi Bölge Kalkınma İdaresi Başkanlığı na Hazine Yardımı', HESAPKOD='32503122033' WHERE YIL=2021 AND HESAPKOD='32503100255'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Doğu Karadeniz Projesi Bölge Kalkınma İdaresi Başkanlığı na Hazine Yardımı', HESAPKOD='32503122034' WHERE YIL=2021 AND HESAPKOD='32503100256'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Devlet Su İşleri Genel Müdürlüğü ne Hazine Yardımı', HESAPKOD='32503122035' WHERE YIL=2021 AND HESAPKOD='32503100257'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Türkiye Su Enstitüsü ne Hazine Yardımı', HESAPKOD='32503122036' WHERE YIL=2021 AND HESAPKOD='32503100258'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Türkiye İlaç ve Tıbbi Cihaz Kurumu na Hazine Yardımı', HESAPKOD='32503122037' WHERE YIL=2021 AND HESAPKOD='32503100259'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kamu Denetçiliği Kurumu na Hazine Yardımı', HESAPKOD='32503122038' WHERE YIL=2021 AND HESAPKOD='32503100260'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Türkiye İnsan Hakları ve Eşitlik Kurumu na Hazine Yardımı', HESAPKOD='32503122039' WHERE YIL=2021 AND HESAPKOD='32503100261'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Türkiye Sağlık Enstitüleri Başkanlığı na Hazine Yardımı', HESAPKOD='32503122040' WHERE YIL=2021 AND HESAPKOD='32503100262'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Helal Akreditasyon Kurumu na Hazine Yardımı', HESAPKOD='32503122041' WHERE YIL=2021 AND HESAPKOD='32503100263'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Nadir Toprak Elementleri Araştırma Enstitüsü ne Hazine Yardımı', HESAPKOD='32503122042' WHERE YIL=2021 AND HESAPKOD='32503100264'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Maden ve Petrol İşleri Genel Müdürlüğü ne Hazine Yardımı', HESAPKOD='32503122043' WHERE YIL=2021 AND HESAPKOD='32503100265'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Türkiye Uzay Ajansı na Hazine Yardımı', HESAPKOD='32503122044' WHERE YIL=2021 AND HESAPKOD='32503100266'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Düzenleyici ve Denetleyici Kurumlara Hazine Yardımları', HESAPKOD='325031230' WHERE YIL=2021 AND HESAPKOD='325031003'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Radyo ve Televizyon Üst Kurulu na Hazine Yardımı', HESAPKOD='32503123001' WHERE YIL=2021 AND HESAPKOD='32503100301'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Bilgi Teknolojileri ve İletişim Kurumu na Hazine Yardımı', HESAPKOD='32503123002' WHERE YIL=2021 AND HESAPKOD='32503100302'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sermaye Piyasası Kurulu na Hazine Yardımı', HESAPKOD='32503123003' WHERE YIL=2021 AND HESAPKOD='32503100303'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Bankacılık Düzenleme ve Denetleme Kurumu na Hazine Yardımı', HESAPKOD='32503123004' WHERE YIL=2021 AND HESAPKOD='32503100304'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Enerji Piyasası Düzenleme Kurumu na Hazine Yardımı', HESAPKOD='32503123005' WHERE YIL=2021 AND HESAPKOD='32503100305'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kamu İhale Kurumu na Hazine Yardımı', HESAPKOD='32503123006' WHERE YIL=2021 AND HESAPKOD='32503100306'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Rekabet Kurumu na Hazine Yardımı', HESAPKOD='32503123007' WHERE YIL=2021 AND HESAPKOD='32503100307'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kamu Gözetimi, Muhasebe ve Denetim Standartları Kurumu na Hazine Yardımı', HESAPKOD='32503123008' WHERE YIL=2021 AND HESAPKOD='32503100310'
DELETE FROM  MUHHESAPPLANI WHERE YIL=2021 AND HESAPKOD='32503100309'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kişisel Verileri Koruma Kurumu na Hazine Yardımı', HESAPKOD='32503123009' WHERE YIL=2021 AND HESAPKOD='32503100311'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Nükleer Düzenleme Kurumu na Hazine Yardımı', HESAPKOD='32503123010' WHERE YIL=2021 AND HESAPKOD='32503100312'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sosyal Güvenlik Kurumlarına Hazine Yardımları', HESAPKOD='325031240' WHERE YIL=2021 AND HESAPKOD='325031004'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Türkiye İş Kurumu Genel Müdürlüğü ne', HESAPKOD='32503124001' WHERE YIL=2021 AND HESAPKOD='32503100404'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sosyal Güvenlik Kurumu na', HESAPKOD='32503124002' WHERE YIL=2021 AND HESAPKOD='32503100406'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Mahalli İdarelere Hazine Yardımları', HESAPKOD='325031250' WHERE YIL=2021 AND HESAPKOD='325031005'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='İl Özel İdarelerine', HESAPKOD='32503125001' WHERE YIL=2021 AND HESAPKOD='32503100501'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Belediyelere', HESAPKOD='32503125002' WHERE YIL=2021 AND HESAPKOD='32503100502'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Köylere', HESAPKOD='32503125003' WHERE YIL=2021 AND HESAPKOD='32503100503'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Mahalli İdare Birliklerine', HESAPKOD='32503125004' WHERE YIL=2021 AND HESAPKOD='32503100504'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kamu Teşebbüslerine Sermaye Transferleri', HESAPKOD='325031010' WHERE YIL=2021 AND HESAPKOD='325031006'
DELETE FROM  MUHHESAPPLANI WHERE YIL=2021 AND HESAPKOD='32503100601'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='TC Devlet Demiryolları na', HESAPKOD='32503101001' WHERE YIL=2021 AND HESAPKOD='32503100602'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Tarım İşletmelerine', HESAPKOD='32503101002' WHERE YIL=2021 AND HESAPKOD='32503100603'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Toprak Mahsulleri Ofisi ne', HESAPKOD='32503101003' WHERE YIL=2021 AND HESAPKOD='32503100604'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Teşebbüslere', HESAPKOD='32503101090' WHERE YIL=2021 AND HESAPKOD='32503100605'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Çay İşletmeleri Genel Müdürlüğü ne', HESAPKOD='32503101004' WHERE YIL=2021 AND HESAPKOD='32503100606'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Türkiye Elektrik Dağıtım AŞ ye', HESAPKOD='32503101005' WHERE YIL=2021 AND HESAPKOD='32503100607'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Elektrik Üretim AŞ ye', HESAPKOD='32503101006' WHERE YIL=2021 AND HESAPKOD='32503100608'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Elektrik İletim AŞ ye', HESAPKOD='32503101007' WHERE YIL=2021 AND HESAPKOD='32503100609'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Türkiye Elektrik ve Taahhüt AŞ ye', HESAPKOD='32503101008' WHERE YIL=2021 AND HESAPKOD='32503100610'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Türkiye Taşkömürü Kurumu na', HESAPKOD='32503101009' WHERE YIL=2021 AND HESAPKOD='32503100611'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Üretici Birliklerine', HESAPKOD='32503101010' WHERE YIL=2021 AND HESAPKOD='32503100612'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Tarım Kredi Kooperatiflerine', HESAPKOD='32503101011' WHERE YIL=2021 AND HESAPKOD='32503100613'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Türkiye Şeker Fabrikaları AŞ ye', HESAPKOD='32503101012' WHERE YIL=2021 AND HESAPKOD='32503100614'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sümer Holding AŞ ye', HESAPKOD='32503101013' WHERE YIL=2021 AND HESAPKOD='32503100615'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Makina ve Kimya Endüstrisi Kurumu na', HESAPKOD='32503101014' WHERE YIL=2021 AND HESAPKOD='32503100616'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Teşebbüslere', HESAPKOD='32503101090' WHERE YIL=2021 AND HESAPKOD='32503100617'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Et ve Süt Kurumu Genel Müdürlüğü ne', HESAPKOD='32503101015' WHERE YIL=2021 AND HESAPKOD='32503100619'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Teşebbüslere', HESAPKOD='32503101090' WHERE YIL=2021 AND HESAPKOD='32503100649'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Darphane ve Damga Matbaası Genel Müdürlüğü ne', HESAPKOD='32503102001' WHERE YIL=2021 AND HESAPKOD='32503100650'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Döner Sermaye İşletmelerine', HESAPKOD='32503102090' WHERE YIL=2021 AND HESAPKOD='32503100659'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Ziraat Bankasına', HESAPKOD='32503103001' WHERE YIL=2021 AND HESAPKOD='32503100670'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Türkiye Kalkınma ve Yatırım Bankasına', HESAPKOD='32503103002' WHERE YIL=2021 AND HESAPKOD='32503100671'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Türk Eximbanka', HESAPKOD='32503103003' WHERE YIL=2021 AND HESAPKOD='32503100672'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Mali Kurumlara', HESAPKOD='32503103090' WHERE YIL=2021 AND HESAPKOD='32503100679'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Ulusal Fona', HESAPKOD='32503104080' WHERE YIL=2021 AND HESAPKOD='32503100680'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Fonlara', HESAPKOD='32503104090' WHERE YIL=2021 AND HESAPKOD='32503100689'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yükseköğretim Kurumlarına Hazine Yardımları', HESAPKOD='325031270' WHERE YIL=2021 AND HESAPKOD='325031007'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yükseköğretim Kurulu na Hazine Yardımı', HESAPKOD='32503127001' WHERE YIL=2021 AND HESAPKOD='32503100701'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Ankara Üniversitesi ne Hazine Yardımı', HESAPKOD='32503127002' WHERE YIL=2021 AND HESAPKOD='32503100702'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Ortadoğu Teknik Üniversitesi ne Hazine Yardımı', HESAPKOD='32503127003' WHERE YIL=2021 AND HESAPKOD='32503100703'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Hacettepe Üniversitesi ne Hazine Yardımı', HESAPKOD='32503127004' WHERE YIL=2021 AND HESAPKOD='32503100704'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Gazi Üniversitesi ne Hazine Yardımı', HESAPKOD='32503127005' WHERE YIL=2021 AND HESAPKOD='32503100705'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='İstanbul Üniversitesi ne Hazine Yardımı', HESAPKOD='32503127006' WHERE YIL=2021 AND HESAPKOD='32503100706'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='İstanbul Teknik Üniversitesi ne Hazine Yardımı', HESAPKOD='32503127007' WHERE YIL=2021 AND HESAPKOD='32503100707'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Boğaziçi Üniversitesi ne Hazine Yardımı', HESAPKOD='32503127008' WHERE YIL=2021 AND HESAPKOD='32503100708'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Marmara Üniversitesi ne Hazine Yardımı', HESAPKOD='32503127009' WHERE YIL=2021 AND HESAPKOD='32503100709'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yıldız Teknik Üniversitesi ne Hazine Yardımı', HESAPKOD='32503127010' WHERE YIL=2021 AND HESAPKOD='32503100710'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Mimar Sinan Güzel Sanatlar Üniversitesi ne Hazine Yardımı', HESAPKOD='32503127011' WHERE YIL=2021 AND HESAPKOD='32503100711'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Ege Üniversitesi ne Hazine Yardımı', HESAPKOD='32503127012' WHERE YIL=2021 AND HESAPKOD='32503100712'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Dokuz Eylül Üniversitesi ne Hazine Yardımı', HESAPKOD='32503127013' WHERE YIL=2021 AND HESAPKOD='32503100713'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Trakya Üniversitesi ne Hazine Yardımı', HESAPKOD='32503127014' WHERE YIL=2021 AND HESAPKOD='32503100714'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Bursa Uludağ Üniversitesi ne Hazine Yardımı', HESAPKOD='32503127015' WHERE YIL=2021 AND HESAPKOD='32503100715'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Anadolu Üniversitesi ne Hazine Yardımı', HESAPKOD='32503127016' WHERE YIL=2021 AND HESAPKOD='32503100716'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Selçuk Üniversitesi ne Hazine Yardımı', HESAPKOD='32503127017' WHERE YIL=2021 AND HESAPKOD='32503100717'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Akdeniz Üniversitesi ne Hazine Yardımı', HESAPKOD='32503127018' WHERE YIL=2021 AND HESAPKOD='32503100718'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Erciyes Üniversitesi ne Hazine Yardımı', HESAPKOD='32503127019' WHERE YIL=2021 AND HESAPKOD='32503100719'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sivas Cumhuriyet Üniversitesi ne Hazine Yardımı', HESAPKOD='32503127020' WHERE YIL=2021 AND HESAPKOD='32503100720'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Çukurova Üniversitesi ne Hazine Yardımı', HESAPKOD='32503127021' WHERE YIL=2021 AND HESAPKOD='32503100721'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Ondokuz Mayıs Üniversitesi ne Hazine Yardımı', HESAPKOD='32503127022' WHERE YIL=2021 AND HESAPKOD='32503100722'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Karadeniz Teknik Üniversitesi ne Hazine Yardımı', HESAPKOD='32503127023' WHERE YIL=2021 AND HESAPKOD='32503100723'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Atatürk Üniversitesi ne Hazine Yardımı', HESAPKOD='32503127024' WHERE YIL=2021 AND HESAPKOD='32503100724'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='İnönü Üniversitesi ne Hazine Yardımı', HESAPKOD='32503127025' WHERE YIL=2021 AND HESAPKOD='32503100725'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Fırat Üniversitesi ne Hazine Yardımı', HESAPKOD='32503127026' WHERE YIL=2021 AND HESAPKOD='32503100726'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Dicle Üniversitesi ne Hazine Yardımı', HESAPKOD='32503127027' WHERE YIL=2021 AND HESAPKOD='32503100727'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yüzüncüyıl Üniversitesi ne Hazine Yardımı', HESAPKOD='32503127028' WHERE YIL=2021 AND HESAPKOD='32503100728'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Gaziantep Üniversitesi ne Hazine Yardımı', HESAPKOD='32503127029' WHERE YIL=2021 AND HESAPKOD='32503100729'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='İzmir Yüksek Teknoloji Enstitüsü ne Hazine Yardımı', HESAPKOD='32503127030' WHERE YIL=2021 AND HESAPKOD='32503100730'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Gebze Teknik Üniversitesi ne Hazine Yardımı', HESAPKOD='32503127031' WHERE YIL=2021 AND HESAPKOD='32503100731'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Harran Üniversitesi ne Hazine Yardımı', HESAPKOD='32503127032' WHERE YIL=2021 AND HESAPKOD='32503100732'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Süleyman Demirel Üniversitesi ne Hazine Yardımı', HESAPKOD='32503127033' WHERE YIL=2021 AND HESAPKOD='32503100733'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Aydın Adnan Menderes Üniversitesi ne Hazine Yardımı', HESAPKOD='32503127034' WHERE YIL=2021 AND HESAPKOD='32503100734'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Zonguldak Bülent Ecevit Üniversitesi ne Hazine Yardımı', HESAPKOD='32503127035' WHERE YIL=2021 AND HESAPKOD='32503100735'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Mersin Üniversitesi ne Hazine Yardımı', HESAPKOD='32503127036' WHERE YIL=2021 AND HESAPKOD='32503100736'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Pamukkale Üniversitesi ne Hazine Yardımı', HESAPKOD='32503127037' WHERE YIL=2021 AND HESAPKOD='32503100737'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Balıkesir Üniversitesi ne Hazine Yardımı', HESAPKOD='32503127038' WHERE YIL=2021 AND HESAPKOD='32503100738'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kocaeli Üniversitesi ne Hazine Yardımı', HESAPKOD='32503127039' WHERE YIL=2021 AND HESAPKOD='32503100739'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sakarya Üniversitesi ne Hazine Yardımı', HESAPKOD='32503127040' WHERE YIL=2021 AND HESAPKOD='32503100740'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Manisa Celal Bayar Üniversitesi’ne Hazine Yardımı', HESAPKOD='32503127041' WHERE YIL=2021 AND HESAPKOD='32503100741'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Bolu Abant İzzet Baysal Üniversitesi ne Hazine Yardımı', HESAPKOD='32503127042' WHERE YIL=2021 AND HESAPKOD='32503100742'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Hatay Mustafa Kemal Üniversitesi ne Hazine Yardımı', HESAPKOD='32503127043' WHERE YIL=2021 AND HESAPKOD='32503100743'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Afyon Kocatepe Üniversitesi ne Hazine Yardımı', HESAPKOD='32503127044' WHERE YIL=2021 AND HESAPKOD='32503100744'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kafkas Üniversitesi ne Hazine Yardımı', HESAPKOD='32503127045' WHERE YIL=2021 AND HESAPKOD='32503100745'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Çanakkale 18 Mart Üniversitesi ne Hazine Yardımı', HESAPKOD='32503127046' WHERE YIL=2021 AND HESAPKOD='32503100746'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Niğde Ömer Halisdemir Üniversitesi’ne Hazine Yardımı', HESAPKOD='32503127047' WHERE YIL=2021 AND HESAPKOD='32503100747'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kütahya Dumlupınar Üniversitesi ne Hazine Yardımı', HESAPKOD='32503127048' WHERE YIL=2021 AND HESAPKOD='32503100748'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Tokat Gaziosmanpaşa Üniversitesi ne Hazine Yardımı', HESAPKOD='32503127049' WHERE YIL=2021 AND HESAPKOD='32503100749'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Muğla Sıtkı Koçman Üniversitesi ne Hazine Yardımı', HESAPKOD='32503127050' WHERE YIL=2021 AND HESAPKOD='32503100750'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kahramanmaraş Sütçü İmam Üniversitesi ne Hazine Yardımı', HESAPKOD='32503127051' WHERE YIL=2021 AND HESAPKOD='32503100751'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kırıkkale Üniversitesi ne Hazine Yardımı', HESAPKOD='32503127052' WHERE YIL=2021 AND HESAPKOD='32503100752'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Eskişehir Osmangazi Üniversitesi ne Hazine Yardımı', HESAPKOD='32503127053' WHERE YIL=2021 AND HESAPKOD='32503100753'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Galatasaray Üniversitesi ne Hazine Yardımı', HESAPKOD='32503127054' WHERE YIL=2021 AND HESAPKOD='32503100754'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kırşehir Ahi Evran Üniversitesi ne Hazine Yardımı', HESAPKOD='32503127055' WHERE YIL=2021 AND HESAPKOD='32503100755'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kastamonu Üniversitesi ne Hazine Yardımı', HESAPKOD='32503127056' WHERE YIL=2021 AND HESAPKOD='32503100756'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Düzce Üniversitesi ne Hazine Yardımı', HESAPKOD='32503127057' WHERE YIL=2021 AND HESAPKOD='32503100757'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Burdur Mehmet Akif Ersoy Üniversitesi ne Hazine Yardımı', HESAPKOD='32503127058' WHERE YIL=2021 AND HESAPKOD='32503100758'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Uşak Üniversitesi ne Hazine Yardımı', HESAPKOD='32503127059' WHERE YIL=2021 AND HESAPKOD='32503100759'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Recep Tayyip Erdoğan Üniversitesi ne Hazine Yardımı', HESAPKOD='32503127060' WHERE YIL=2021 AND HESAPKOD='32503100760'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Tekirdağ Namık Kemal Üniversitesi ne Hazine Yardımı', HESAPKOD='32503127061' WHERE YIL=2021 AND HESAPKOD='32503100761'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Erzincan Binali Yıldırım Üniversitesi ne Hazine Yardımı', HESAPKOD='32503127062' WHERE YIL=2021 AND HESAPKOD='32503100762'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Aksaray Üniversitesi ne Hazine Yardımı', HESAPKOD='32503127063' WHERE YIL=2021 AND HESAPKOD='32503100763'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Giresun Üniversitesi ne Hazine Yardımı', HESAPKOD='32503127064' WHERE YIL=2021 AND HESAPKOD='32503100764'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Hitit Üniversitesi ne Hazine Yardımı', HESAPKOD='32503127065' WHERE YIL=2021 AND HESAPKOD='32503100765'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yozgat Bozok Üniversitesi ne Hazine Yardımı', HESAPKOD='32503127066' WHERE YIL=2021 AND HESAPKOD='32503100766'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Adıyaman Üniversitesi ne Hazine Yardımı', HESAPKOD='32503127067' WHERE YIL=2021 AND HESAPKOD='32503100767'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Ordu Üniversitesi ne Hazine Yardımı', HESAPKOD='32503127068' WHERE YIL=2021 AND HESAPKOD='32503100768'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Amasya Üniversitesi ne Hazine Yardımı', HESAPKOD='32503127069' WHERE YIL=2021 AND HESAPKOD='32503100769'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Karamanoğlu Mehmetbey Üniversitesi ne Hazine Yardımı', HESAPKOD='32503127070' WHERE YIL=2021 AND HESAPKOD='32503100770'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Ağrı İbrahim Çeçen Üniversitesi ne Hazine Yardımı', HESAPKOD='32503127071' WHERE YIL=2021 AND HESAPKOD='32503100771'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sinop Üniversitesi ne Hazine Yardımı', HESAPKOD='32503127072' WHERE YIL=2021 AND HESAPKOD='32503100772'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Siirt Üniversitesi ne Hazine Yardımı', HESAPKOD='32503127073' WHERE YIL=2021 AND HESAPKOD='32503100773'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Nevşehir Hacı Bektaş Veli Üniversitesi ne Hazine Yardımı', HESAPKOD='32503127074' WHERE YIL=2021 AND HESAPKOD='32503100774'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Karabük Üniversitesi ne Hazine Yardımı', HESAPKOD='32503127075' WHERE YIL=2021 AND HESAPKOD='32503100775'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kilis 7 Aralık Üniversitesi ne Hazine Yardımı', HESAPKOD='32503127076' WHERE YIL=2021 AND HESAPKOD='32503100776'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Çankırı Karatekin Üniversitesi ne Hazine Yardımı', HESAPKOD='32503127077' WHERE YIL=2021 AND HESAPKOD='32503100777'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Artvin Çoruh Üniversitesi ne Hazine Yardımı', HESAPKOD='32503127078' WHERE YIL=2021 AND HESAPKOD='32503100778'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Bilecik Şeyh Edebali Üniversitesi ne Hazine Yardımı', HESAPKOD='32503127079' WHERE YIL=2021 AND HESAPKOD='32503100779'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Bitlis Eren Üniversitesi ne Hazine Yardımı', HESAPKOD='32503127080' WHERE YIL=2021 AND HESAPKOD='32503100780'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kırklareli Üniversitesi ne Hazine Yardımı', HESAPKOD='32503127081' WHERE YIL=2021 AND HESAPKOD='32503100781'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Osmaniye Korkut Ata Üniversitesi ne Hazine Yardımı', HESAPKOD='32503127082' WHERE YIL=2021 AND HESAPKOD='32503100782'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Bingöl Üniversitesi ne Hazine Yardımı', HESAPKOD='32503127083' WHERE YIL=2021 AND HESAPKOD='32503100783'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Muş Alparslan Üniversitesi ne Hazine Yardımı', HESAPKOD='32503127084' WHERE YIL=2021 AND HESAPKOD='32503100784'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Mardin Artuklu Üniversitesi ne Hazine Yardımı', HESAPKOD='32503127085' WHERE YIL=2021 AND HESAPKOD='32503100785'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Batman Üniversitesi ne Hazine Yardımı', HESAPKOD='32503127086' WHERE YIL=2021 AND HESAPKOD='32503100786'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Ardahan Üniversitesi ne Hazine Yardımı', HESAPKOD='32503127087' WHERE YIL=2021 AND HESAPKOD='32503100787'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Bartın Üniversitesi ne Hazine Yardımı', HESAPKOD='32503127088' WHERE YIL=2021 AND HESAPKOD='32503100788'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Bayburt Üniversitesi ne Hazine Yardımı', HESAPKOD='32503127089' WHERE YIL=2021 AND HESAPKOD='32503100789'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Gümüşhane Üniversitesi ne Hazine Yardımı', HESAPKOD='32503127090' WHERE YIL=2021 AND HESAPKOD='32503100790'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Hakkari Üniversitesi ne Hazine Yardımı', HESAPKOD='32503127091' WHERE YIL=2021 AND HESAPKOD='32503100791'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Iğdır Üniversitesi ne Hazine Yardımı', HESAPKOD='32503127092' WHERE YIL=2021 AND HESAPKOD='32503100792'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Şırnak Üniversitesi ne Hazine Yardımı', HESAPKOD='32503127093' WHERE YIL=2021 AND HESAPKOD='32503100793'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Munzur Üniversitesi ne Hazine Yardımı', HESAPKOD='32503127094' WHERE YIL=2021 AND HESAPKOD='32503100794'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yalova Üniversitesi ne Hazine Yardımı', HESAPKOD='32503127095' WHERE YIL=2021 AND HESAPKOD='32503100795'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Türk-Alman Üniversitesi ne Hazine Yardımı', HESAPKOD='32503127096' WHERE YIL=2021 AND HESAPKOD='32503100796'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Ankara Yıldırım Beyazıt Üniversitesi ne Hazine Yardımı', HESAPKOD='32503127097' WHERE YIL=2021 AND HESAPKOD='32503100797'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Bursa Teknik Üniversitesi ne Hazine Yardımı', HESAPKOD='32503127098' WHERE YIL=2021 AND HESAPKOD='32503100798'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='İstanbul Medeniyet Üniversitesi ne Hazine Yardımı', HESAPKOD='32503127099' WHERE YIL=2021 AND HESAPKOD='32503100799'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yükseköğretim Kurumlarına Hazine Yardımları', HESAPKOD='325031280' WHERE YIL=2021 AND HESAPKOD='325031008'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='İzmir Katip Çelebi Üniversitesi ne Hazine Yardımı', HESAPKOD='32503128001' WHERE YIL=2021 AND HESAPKOD='32503100801'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Necmettin Erbakan Üniversitesi ne Hazine Yardımı', HESAPKOD='32503128002' WHERE YIL=2021 AND HESAPKOD='32503100802'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Abdullah Gül Üniversitesi ne Hazine Yardımı', HESAPKOD='32503128003' WHERE YIL=2021 AND HESAPKOD='32503100803'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Erzurum Teknik Üniversitesi ne Hazine Yardımı', HESAPKOD='32503128004' WHERE YIL=2021 AND HESAPKOD='32503100804'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Adana Alparslan Türkeş Bilim ve Teknoloji Üniversitesi ne Hazine Yardımı', HESAPKOD='32503128005' WHERE YIL=2021 AND HESAPKOD='32503100805'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Ankara Sosyal Bilimler Üniversitesi ne Hazine Yardımı', HESAPKOD='32503128006' WHERE YIL=2021 AND HESAPKOD='32503100806'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sağlık Bilimleri Üniversitesi ne Hazine Yardımı', HESAPKOD='32503128007' WHERE YIL=2021 AND HESAPKOD='32503100807'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Bandırma Onyedi Eylül Üniversitesi ne Hazine Yardımı', HESAPKOD='32503128008' WHERE YIL=2021 AND HESAPKOD='32503100808'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='İskenderun Teknik Üniversitesi ne Hazine Yardımı', HESAPKOD='32503128009' WHERE YIL=2021 AND HESAPKOD='32503100809'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Alanya Alaaddin Keykubat Üniversitesi ne Hazine Yardımı', HESAPKOD='32503128010' WHERE YIL=2021 AND HESAPKOD='32503100810'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Türkiye Uluslararası İslam, Bilim ve Teknoloji Üniversitesi ne Hazine Yardımı', HESAPKOD='32503128011' WHERE YIL=2021 AND HESAPKOD='32503100811'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='İzmir Bakırçay Üniversitesi ne Hazine Yardımı', HESAPKOD='32503128012' WHERE YIL=2021 AND HESAPKOD='32503100812'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='İzmir Demokrasi Üniversitesi ne Hazine Yardımı', HESAPKOD='32503128013' WHERE YIL=2021 AND HESAPKOD='32503100813'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yükseköğretim Kalite Kurulu na Hazine Yardımı', HESAPKOD='32503128014' WHERE YIL=2021 AND HESAPKOD='32503100814'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Ankara Müzik ve Güzel Sanatlar Üniversitesi ne Hazine Yardımı', HESAPKOD='32503128015' WHERE YIL=2021 AND HESAPKOD='32503100815'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Gaziantep İslam Bilim ve Teknoloji Üniversitesi ne Hazine Yardımı', HESAPKOD='32503128016' WHERE YIL=2021 AND HESAPKOD='32503100816'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Konya Teknik Üniversitesi ne Hazine Yardımı', HESAPKOD='32503128017' WHERE YIL=2021 AND HESAPKOD='32503100817'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kütahya Sağlık Bilimleri Üniversitesi ne Hazine Yardımı', HESAPKOD='32503128018' WHERE YIL=2021 AND HESAPKOD='32503100818'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Malatya Turgut Özal Üniversitesi ne Hazine Yardımı', HESAPKOD='32503128019' WHERE YIL=2021 AND HESAPKOD='32503100819'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='İstanbul Üniversitesi-Cerrahpaşa ya Hazine Yardımı', HESAPKOD='32503128020' WHERE YIL=2021 AND HESAPKOD='32503100820'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Ankara Hacı Bayram Veli Üniversitesi ne Hazine Yardımı', HESAPKOD='32503128021' WHERE YIL=2021 AND HESAPKOD='32503100821'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sakarya Uygulamalı Bilimler Üniversitesi ne Hazine Yardımı', HESAPKOD='32503128022' WHERE YIL=2021 AND HESAPKOD='32503100822'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Samsun Üniversitesi ne Hazine Yardımı', HESAPKOD='32503128023' WHERE YIL=2021 AND HESAPKOD='32503100823'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sivas Bilim ve Teknoloji Üniversitesi ne Hazine Yardımı', HESAPKOD='32503128024' WHERE YIL=2021 AND HESAPKOD='32503100824'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Tarsus Üniversitesi ne Hazine Yardımı', HESAPKOD='32503128025' WHERE YIL=2021 AND HESAPKOD='32503100825'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Trabzon Üniversitesi ne Hazine Yardımı', HESAPKOD='32503128026' WHERE YIL=2021 AND HESAPKOD='32503100826'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kayseri Üniversitesi ne Hazine Yardımı', HESAPKOD='32503128027' WHERE YIL=2021 AND HESAPKOD='32503100827'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kahramanmaraş İstiklal Üniversitesi ne Hazine Yardımı', HESAPKOD='32503128028' WHERE YIL=2021 AND HESAPKOD='32503100828'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Eskişehir Teknik Üniversitesi ne Hazine Yardımı', HESAPKOD='32503128029' WHERE YIL=2021 AND HESAPKOD='32503100829'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Isparta Uygulamalı Bilimler Üniversitesi ne Hazine Yardımı', HESAPKOD='32503128030' WHERE YIL=2021 AND HESAPKOD='32503100830'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Afyonkarahisar Sağlık Bilimleri Üniversitesi ne Hazine Yardımı', HESAPKOD='32503128031' WHERE YIL=2021 AND HESAPKOD='32503100831'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Kurum, İşletme ve Hane Halkına Yapılan Sermaye Transferleri', HESAPKOD='325031090' WHERE YIL=2021 AND HESAPKOD='325031009'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Dernek, Birlik, Kurum, Kuruluş, Sandık vb Kuruluşlara', HESAPKOD='32503109001' WHERE YIL=2021 AND HESAPKOD='32503100901'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Vakıf Üniversitelerine', HESAPKOD='32503109002' WHERE YIL=2021 AND HESAPKOD='32503100902'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Kamu Kurum ve Kuruluşlarına', HESAPKOD='32503109003' WHERE YIL=2021 AND HESAPKOD='32503100903'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kamu Ortaklıklarına', HESAPKOD='32503109004' WHERE YIL=2021 AND HESAPKOD='32503100904'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Özel Teşebbüslere', HESAPKOD='32503109005' WHERE YIL=2021 AND HESAPKOD='32503100905'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Hane Halklarına', HESAPKOD='32503109006' WHERE YIL=2021 AND HESAPKOD='32503100906'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Türk Kültür Varlığının Korunması ve Tanıtımı Giderleri', HESAPKOD='32503109007' WHERE YIL=2021 AND HESAPKOD='32503100908'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Siyasi Tanıtma Giderleri', HESAPKOD='32503109008' WHERE YIL=2021 AND HESAPKOD='32503100909'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Gençlik ve Spor İl Müdürlüklerine Hazine Yardımı', HESAPKOD='32503129001' WHERE YIL=2021 AND HESAPKOD='32503100911'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kalkınma Ajanslarına', HESAPKOD='32503109009' WHERE YIL=2021 AND HESAPKOD='32503100912'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Bölgelerarası İşbirliği Proje Giderleri', HESAPKOD='32503109010' WHERE YIL=2021 AND HESAPKOD='32503100913'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sosyal ve İktisadi Araştırma Proje Giderleri', HESAPKOD='32503109011' WHERE YIL=2021 AND HESAPKOD='32503100914'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Tarım ve Kırsal Kalkınmayı Destekleme Kurumuna', HESAPKOD='32503109012' WHERE YIL=2021 AND HESAPKOD='32503100915'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Akademik Ar-Ge Projelerine İlişkin Transferler', HESAPKOD='32503109013' WHERE YIL=2021 AND HESAPKOD='32503100920'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Akademik Ar-Ge Projelerine İlişkin Kurum Hissesi', HESAPKOD='32503109014' WHERE YIL=2021 AND HESAPKOD='32503100921'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kamu Ar-Ge Projelerine İlişkin Transferler', HESAPKOD='32503109015' WHERE YIL=2021 AND HESAPKOD='32503100922'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kamu Ar-Ge Projelerine İlişkin Kurum Hissesi', HESAPKOD='32503109016' WHERE YIL=2021 AND HESAPKOD='32503100923'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Savunma ve Uzay Sanayii Ar-Ge Projelerine İlişkin Transferler', HESAPKOD='32503109017' WHERE YIL=2021 AND HESAPKOD='32503100924'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Savunma ve Uzay Sanayii Ar-Ge Projelerine İlişkin Kurum Hissesi', HESAPKOD='32503109018' WHERE YIL=2021 AND HESAPKOD='32503100925'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Bilim ve Toplum Projelerine İlişkin Transferler', HESAPKOD='32503109019' WHERE YIL=2021 AND HESAPKOD='32503100926'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Bilim ve Toplum Projelerine İlişkin Kurum Hissesi', HESAPKOD='32503109020' WHERE YIL=2021 AND HESAPKOD='32503100927'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sınıflandırmaya Girmeyen Diğer Kurum, İşletme ve Hane Halkına  Yapılan Sermaye Transferleri', HESAPKOD='32503109090' WHERE YIL=2021 AND HESAPKOD='32503100990'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Bilimsel Araştırma Projelerinin Desteklenmesine İlişkin Giderler', HESAPKOD='32503109022' WHERE YIL=2021 AND HESAPKOD='32503100999'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yurtdışı Sermaye Transferleri', HESAPKOD='3250311' WHERE YIL=2021 AND HESAPKOD='3250311'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Dış Ülkelere Yapılan Yardımlar', HESAPKOD='325031110' WHERE YIL=2021 AND HESAPKOD='325031101'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kuzey Kıbrıs Türk Cumhuriyeti ne', HESAPKOD='32503111001' WHERE YIL=2021 AND HESAPKOD='32503110101'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Ülkelere', HESAPKOD='32503111090' WHERE YIL=2021 AND HESAPKOD='32503110190'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Uluslararası Kuruluşlara Yapılan Ödemeler', HESAPKOD='325031120' WHERE YIL=2021 AND HESAPKOD='325031102'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Uluslararası Kuruluşlara Katkı Ödemeleri', HESAPKOD='32503112001' WHERE YIL=2021 AND HESAPKOD='32503110201'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Uluslararası Kuruluşlara Yardım Ödemeleri', HESAPKOD='32503112002' WHERE YIL=2021 AND HESAPKOD='32503110202'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Uluslararası Kuruluşlara Diğer Ödemeler', HESAPKOD='32503112090' WHERE YIL=2021 AND HESAPKOD='32503110290'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Uluslarüstü Kuruluşlara', HESAPKOD='325031130' WHERE YIL=2021 AND HESAPKOD='325031103'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Uluslarüstü Kuruluşlara', HESAPKOD='32503113001' WHERE YIL=2021 AND HESAPKOD='32503110301'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Uluslarüstü Kuruluşlardan Merkezlerine ', HESAPKOD='325031140' WHERE YIL=2021 AND HESAPKOD='325031104'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Uluslarüstü Kuruluşlardan Merkezlerine', HESAPKOD='32503114001' WHERE YIL=2021 AND HESAPKOD='32503110401'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Eğitim Kurumlarına', HESAPKOD='325031150' WHERE YIL=2021 AND HESAPKOD='325031105'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Hoca Ahmet Yesevi Uluslararası Türk Kazak Üniversitesine', HESAPKOD='32503115001' WHERE YIL=2021 AND HESAPKOD='32503110501'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kırgızistan-Türkiye Manas Üniversitesine', HESAPKOD='32503115002' WHERE YIL=2021 AND HESAPKOD='32503110502'
DELETE FROM  MUHHESAPPLANI WHERE YIL=2021 AND HESAPKOD='32503110503'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Yurtdışı Sermaye Transferleri', HESAPKOD='325031190' WHERE YIL=2021 AND HESAPKOD='325031109'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sınıflandırmaya Girmeyen Diğer Yurtdışı Sermaye Transferleri', HESAPKOD='32503119090' WHERE YIL=2021 AND HESAPKOD='32503110901'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Türk Kültür Varlığının Korunması ve Tanıtımı Giderleri', HESAPKOD='32503119001' WHERE YIL=2021 AND HESAPKOD='32503110908'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Siyasi Tanıtma Giderleri', HESAPKOD='32503119002' WHERE YIL=2021 AND HESAPKOD='32503110909'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='GİDER TAHAKKUKLARI HESABI', HESAPKOD='381' WHERE YIL=2021 AND HESAPKOD='381'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Personel Giderleri', HESAPKOD='38101' WHERE YIL=2021 AND HESAPKOD='38101'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Mal ve Hizmet Alım Giderleri', HESAPKOD='38103' WHERE YIL=2021 AND HESAPKOD='38103'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Tüketime Yönelik Mal ve Malzeme Alımları', HESAPKOD='3810302' WHERE YIL=2021 AND HESAPKOD='3810302'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Giyim ve Kuşam Alımları', HESAPKOD='381030250' WHERE YIL=2021 AND HESAPKOD='381030205'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Giyecek Alımları  ', HESAPKOD='38103025001' WHERE YIL=2021 AND HESAPKOD='38103020501'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Giyim ve Kuşam Alımları', HESAPKOD='38103025090' WHERE YIL=2021 AND HESAPKOD='38103020590'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Güvenlik ve Savunmaya Yönelik Mal, Malzeme ve Hizmet Alımları, Yapımları ve Giderleri', HESAPKOD='381030270' WHERE YIL=2021 AND HESAPKOD='381030207'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Güvenlik ve Savunmaya Yönelik Silah, Araç, Gereç ve Savaş Teçhizatı Alımları', HESAPKOD='38103027001' WHERE YIL=2021 AND HESAPKOD='38103020701'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Güvenlik ve Savunmaya Yönelik Silah, Araç, Gereç ve Savaş Teçhizatı İşletme, Bakım ve İdame Giderleri', HESAPKOD='38103027002' WHERE YIL=2021 AND HESAPKOD='38103020702'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Mühimmat Alımları', HESAPKOD='38103027003' WHERE YIL=2021 AND HESAPKOD='38103020703'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Güvenlik ve Savunmaya Yönelik Araştırma-Geliştirme Giderleri', HESAPKOD='38103027004' WHERE YIL=2021 AND HESAPKOD='38103020704'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Güvenlik ve Savunmaya Yönelik Hizmet Alım Giderleri', HESAPKOD='38103027005' WHERE YIL=2021 AND HESAPKOD='38103020705'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Savunma Projeleri ve Acil İhtiyaç Giderleri', HESAPKOD='38103027006' WHERE YIL=2021 AND HESAPKOD='38103020706'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Güvenlik ve Savunmaya Yönelik Diğer Giderler', HESAPKOD='38103027009' WHERE YIL=2021 AND HESAPKOD='38103020709'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Güvenlik ve Savunmaya Yönelik Makine-Teçhizat Alımları', HESAPKOD='38103027011' WHERE YIL=2021 AND HESAPKOD='38103020711'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Güvenlik ve Savunmaya Yönelik Makine-Teçhizat Büyük Onarımları', HESAPKOD='38103027012' WHERE YIL=2021 AND HESAPKOD='38103020712'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Güvenlik ve Savunmaya Yönelik Gayrimenkul Yapım Giderleri', HESAPKOD='38103027021' WHERE YIL=2021 AND HESAPKOD='38103020721'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Güvenlik ve Savunmaya Yönelik Gayrimenkul Büyük Onarım Giderleri', HESAPKOD='38103027022' WHERE YIL=2021 AND HESAPKOD='38103020722'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='NATO Altyapısına İlişkin Gayrimenkul Yapım Giderleri', HESAPKOD='38103027031' WHERE YIL=2021 AND HESAPKOD='38103020731'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='NATO Altyapısına İlişkin Gayrimenkul Büyük Onarım Giderleri', HESAPKOD='38103027032' WHERE YIL=2021 AND HESAPKOD='38103020732'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Savunma Mal ve  Malzeme Alımları ve Yapımları', HESAPKOD='38103027090' WHERE YIL=2021 AND HESAPKOD='38103020790'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='NATO Giderleri İle Gayrimenkul Alım ve Kamulaştırma Giderleri', HESAPKOD='381030280' WHERE YIL=2021 AND HESAPKOD='381030208'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Gayrimenkul Alım ve Kamulaştırma Giderleri', HESAPKOD='38103028001' WHERE YIL=2021 AND HESAPKOD='38103020801'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='NATO Altyapısına İlişkin Gayrimenkul Alım ve Kamulaştırma Giderleri', HESAPKOD='38103028002' WHERE YIL=2021 AND HESAPKOD='38103020802'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='NATO Giderleri', HESAPKOD='38103028003' WHERE YIL=2021 AND HESAPKOD='38103020803'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Tüketim Mal ve Malzemesi Alımları', HESAPKOD='381030290' WHERE YIL=2021 AND HESAPKOD='381030209'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Tüketim Mal ve Malzemesi Alımları', HESAPKOD='38103029090' WHERE YIL=2021 AND HESAPKOD='38103020990'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Görev Giderleri', HESAPKOD='3810304' WHERE YIL=2021 AND HESAPKOD='3810304'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yasal Giderler', HESAPKOD='381030420' WHERE YIL=2021 AND HESAPKOD='381030402'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kusursuz Tazminatlar', HESAPKOD='38103042002' WHERE YIL=2021 AND HESAPKOD='38103040203'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Mahkeme Harç ve Giderleri', HESAPKOD='38103047001' WHERE YIL=2021 AND HESAPKOD='38103040204'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Yasal Giderler', HESAPKOD='38103042090' WHERE YIL=2021 AND HESAPKOD='38103040290'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Ödenecek Vergi, Resim,  Harçlar ve Benzeri Giderler', HESAPKOD='381030430' WHERE YIL=2021 AND HESAPKOD='381030403'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Vergi Ödemeleri ve Benzeri Giderler', HESAPKOD='38103043001' WHERE YIL=2021 AND HESAPKOD='38103040301'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='İşletme Ruhsatı Ödemeleri ve Benzeri Giderler', HESAPKOD='38103043002' WHERE YIL=2021 AND HESAPKOD='38103040302'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Vergi, Resim ve Harçlar ve Benzeri Giderler', HESAPKOD='38103043090' WHERE YIL=2021 AND HESAPKOD='38103040390'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Hizmet Alımları', HESAPKOD='3810305' WHERE YIL=2021 AND HESAPKOD='3810305'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Genel Hizmet Alımları ', HESAPKOD='381030510' WHERE YIL=2021 AND HESAPKOD='381030501'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Etüt-Proje Bilirkişi Ekspertiz Giderleri', HESAPKOD='38103051001' WHERE YIL=2021 AND HESAPKOD='38103050101'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Araştırma ve Geliştirme Hizmet Alım Giderleri', HESAPKOD='38103051002' WHERE YIL=2021 AND HESAPKOD='38103050102'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Bilgisayar Hizmet Alım Giderleri', HESAPKOD='38103051003' WHERE YIL=2021 AND HESAPKOD='38103050103'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Genel Hizmet Alımları', HESAPKOD='38103051090' WHERE YIL=2021 AND HESAPKOD='38103050104'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Harita Yapım ve Alım Giderleri', HESAPKOD='38103051004' WHERE YIL=2021 AND HESAPKOD='38103050105'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Enformasyon ve Raporlama Hizmet Alım Giderleri', HESAPKOD='38103051005' WHERE YIL=2021 AND HESAPKOD='38103050106'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Danışma Yönetim ve İşletim Hizmet Alım Giderleri', HESAPKOD='38103051006' WHERE YIL=2021 AND HESAPKOD='38103050107'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Genel Hizmet Alımları', HESAPKOD='38103051090' WHERE YIL=2021 AND HESAPKOD='38103050108'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Genel Hizmet Alımları', HESAPKOD='38103051090' WHERE YIL=2021 AND HESAPKOD='38103050190'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kiralar', HESAPKOD='381030550' WHERE YIL=2021 AND HESAPKOD='381030505'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Dayanıklı Mal ve Malzeme Kiralama Giderleri', HESAPKOD='38103055001' WHERE YIL=2021 AND HESAPKOD='38103050501'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Taşıt Kiralama Giderleri', HESAPKOD='38103055002' WHERE YIL=2021 AND HESAPKOD='38103050502'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='İş Makinası Kiralama Giderleri', HESAPKOD='38103055003' WHERE YIL=2021 AND HESAPKOD='38103050503'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Canlı Hayvan Kiralama Giderleri', HESAPKOD='38103055004' WHERE YIL=2021 AND HESAPKOD='38103050504'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Hizmet Binası Kiralama Giderleri', HESAPKOD='38103055005' WHERE YIL=2021 AND HESAPKOD='38103050505'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Lojman Kiralama Giderleri', HESAPKOD='38103055006' WHERE YIL=2021 AND HESAPKOD='38103050506'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Arsa ve Arazi Kiralama Giderleri', HESAPKOD='38103055007' WHERE YIL=2021 AND HESAPKOD='38103050507'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yüzer Taşıt Kiralama Giderleri', HESAPKOD='38103055008' WHERE YIL=2021 AND HESAPKOD='38103050508'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Hava Taşıtı Kiralama Giderleri', HESAPKOD='38103055009' WHERE YIL=2021 AND HESAPKOD='38103050509'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Bilgisayar, Bilgisayar Sistemleri ve Yazılımları Kiralama Giderleri', HESAPKOD='38103055010' WHERE YIL=2021 AND HESAPKOD='38103050510'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Tersane Kiralama Giderleri', HESAPKOD='38103055011' WHERE YIL=2021 AND HESAPKOD='38103050511'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Personel Servisi Kiralama Giderleri', HESAPKOD='38103055012' WHERE YIL=2021 AND HESAPKOD='38103050512'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Kiralama Giderleri', HESAPKOD='38103055090' WHERE YIL=2021 AND HESAPKOD='38103050590'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Hizmet Alımları', HESAPKOD='381030590' WHERE YIL=2021 AND HESAPKOD='381030509'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sınıflandırmaya Girmeyen Diğer Hizmet Alımları', HESAPKOD='38103059090' WHERE YIL=2021 AND HESAPKOD='38103050990'
DELETE FROM  MUHHESAPPLANI WHERE YIL=2021 AND HESAPKOD='3810306'
DELETE FROM  MUHHESAPPLANI WHERE YIL=2021 AND HESAPKOD='3810307'
DELETE FROM  MUHHESAPPLANI WHERE YIL=2021 AND HESAPKOD='3810308'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Faiz Giderleri', HESAPKOD='38104' WHERE YIL=2021 AND HESAPKOD='38104'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kamu Kurumlarına Ödenen İç Borç Faiz Giderleri', HESAPKOD='3810401' WHERE YIL=2021 AND HESAPKOD='3810401'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Devlet Tahvili', HESAPKOD='381040110' WHERE YIL=2021 AND HESAPKOD='381040101'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='TL Cinsinden Tahvillerin Faiz Giderleri', HESAPKOD='38104011001' WHERE YIL=2021 AND HESAPKOD='38104010101'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Dövize Endeksli ve Döviz Cinsinden Tahvillerin Faiz Giderleri', HESAPKOD='38104011002' WHERE YIL=2021 AND HESAPKOD='38104010102'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Hazine Bonosu', HESAPKOD='381040120' WHERE YIL=2021 AND HESAPKOD='381040102'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='TL Cinsinden Bonoların Faiz Giderleri', HESAPKOD='38104012001' WHERE YIL=2021 AND HESAPKOD='38104010201'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Dövize Endeksli ve Döviz Cinsinden Bonoların Faiz Giderleri', HESAPKOD='38104012002' WHERE YIL=2021 AND HESAPKOD='38104010202'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer İç Borç Faiz Giderleri', HESAPKOD='3810402' WHERE YIL=2021 AND HESAPKOD='3810402'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Devlet Tahvili', HESAPKOD='381040210' WHERE YIL=2021 AND HESAPKOD='381040201'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='TL Cinsinden Tahvillerin Faiz Giderleri', HESAPKOD='38104021001' WHERE YIL=2021 AND HESAPKOD='38104020101'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Dövize Endeksli ve Döviz Cinsinden Tahvillerin Faiz Giderleri', HESAPKOD='38104021002' WHERE YIL=2021 AND HESAPKOD='38104020102'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Hazine Bonosu', HESAPKOD='381040220' WHERE YIL=2021 AND HESAPKOD='381040202'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='TL Cinsinden Bonoların Faiz Giderleri', HESAPKOD='38104022001' WHERE YIL=2021 AND HESAPKOD='38104020201'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Dövize Endeksli ve Döviz Cinsinden Bonoların Faiz Giderleri', HESAPKOD='38104022002' WHERE YIL=2021 AND HESAPKOD='38104020202'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer İç Borç Faiz Giderleri ', HESAPKOD='381040290' WHERE YIL=2021 AND HESAPKOD='381040209'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='TL Cinsinden Diğer İç Borç Faiz Giderleri', HESAPKOD='38104029001' WHERE YIL=2021 AND HESAPKOD='38104020901'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Dövize Endeksli ve Döviz Cinsinden Diğer İç Borç Faiz Giderleri ', HESAPKOD='38104029002' WHERE YIL=2021 AND HESAPKOD='38104020902'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Dış Borç Faiz Giderleri', HESAPKOD='3810403' WHERE YIL=2021 AND HESAPKOD='3810403'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='ABD Doları Cinsinden Dış Borç Faiz Giderleri', HESAPKOD='381040310' WHERE YIL=2021 AND HESAPKOD='381040301'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='ABD Doları Cinsinden Dış Borç Faiz Giderleri', HESAPKOD='38104031001' WHERE YIL=2021 AND HESAPKOD='38104030101'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Avro Cinsinden Borç Faiz Giderleri', HESAPKOD='381040320' WHERE YIL=2021 AND HESAPKOD='381040302'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Avro Cinsinden Dış Borç Faiz Giderleri', HESAPKOD='38104032001' WHERE YIL=2021 AND HESAPKOD='38104030201'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Japon Yeni Cinsinden Dış Borç Faiz Giderleri', HESAPKOD='381040330' WHERE YIL=2021 AND HESAPKOD='381040303'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Japon Yeni Cinsinden Dış Borç Faiz Giderleri', HESAPKOD='38104033001' WHERE YIL=2021 AND HESAPKOD='38104030301'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Dış Borç Faiz Giderleri', HESAPKOD='381040390' WHERE YIL=2021 AND HESAPKOD='381040309'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Dış Borç Faiz Giderleri', HESAPKOD='38104039001' WHERE YIL=2021 AND HESAPKOD='38104030901'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='İskonto Giderleri', HESAPKOD='3810404' WHERE YIL=2021 AND HESAPKOD='3810404'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='İç Borç İskonto Giderleri', HESAPKOD='381040410' WHERE YIL=2021 AND HESAPKOD='381040401'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Devlet Tahvili İskonto Giderleri', HESAPKOD='38104041001' WHERE YIL=2021 AND HESAPKOD='38104040101'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Hazine Bonosu İskonto Giderleri', HESAPKOD='38104041002' WHERE YIL=2021 AND HESAPKOD='38104040102'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Dış Borç İskonto Giderleri', HESAPKOD='381040420' WHERE YIL=2021 AND HESAPKOD='381040402'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Dış Borç İskonto Giderleri', HESAPKOD='38104042001' WHERE YIL=2021 AND HESAPKOD='38104040201'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Para Piyasası Nakit İşlemleri Faiz Giderleri', HESAPKOD='3810405' WHERE YIL=2021 AND HESAPKOD='3810405'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Para Piyasası Nakit İşlemleri Faiz Giderleri ', HESAPKOD='381040510' WHERE YIL=2021 AND HESAPKOD='381040501'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='TL Cinsinden Para Piyasası Nakit İşlemleri Faiz Giderleri ', HESAPKOD='38104051001' WHERE YIL=2021 AND HESAPKOD='38104050101'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Döviz Cinsinden Kısa Vadeli Nakit İşlemlere Ait Faiz Giderleri', HESAPKOD='38104051002' WHERE YIL=2021 AND HESAPKOD='38104050102'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Cari Transferler', HESAPKOD='38105' WHERE YIL=2021 AND HESAPKOD='38105'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Hane Halkı ve İşletmelere Yapılan Transferler', HESAPKOD='3810504' WHERE YIL=2021 AND HESAPKOD='3810504'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Ekonomik/Mali Amaçlı Transferler', HESAPKOD='381050480' WHERE YIL=2021 AND HESAPKOD='381050408'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='KREDİ FAİZ DESTEKLERİ', HESAPKOD='3810504809001' WHERE YIL=2021 AND HESAPKOD='3810504080101'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='2016 Yılı Makine Teçhizat Kredi Faiz Desteği', HESAPKOD='381050480900101' WHERE YIL=2021 AND HESAPKOD='381050408010101'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='GİDER TAHAKKUKLARI HESABI', HESAPKOD='481' WHERE YIL=2021 AND HESAPKOD='481'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Personel Giderleri', HESAPKOD='48101' WHERE YIL=2021 AND HESAPKOD='48101'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Mal ve Hizmet Alım Giderleri', HESAPKOD='48103' WHERE YIL=2021 AND HESAPKOD='48103'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Tüketime Yönelik Mal ve Malzeme Alımları', HESAPKOD='4810302' WHERE YIL=2021 AND HESAPKOD='4810302'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Giyim ve Kuşam Alımları', HESAPKOD='481030250' WHERE YIL=2021 AND HESAPKOD='481030205'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Giyecek Alımları  ', HESAPKOD='48103025001' WHERE YIL=2021 AND HESAPKOD='48103020501'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Giyim ve Kuşam Alımları', HESAPKOD='48103025090' WHERE YIL=2021 AND HESAPKOD='48103020590'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Güvenlik ve Savunmaya Yönelik Mal, Malzeme ve Hizmet Alımları, Yapımları ve Giderleri', HESAPKOD='481030270' WHERE YIL=2021 AND HESAPKOD='481030207'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Güvenlik ve Savunmaya Yönelik Silah, Araç, Gereç ve Savaş Teçhizatı Alımları', HESAPKOD='48103027001' WHERE YIL=2021 AND HESAPKOD='48103020701'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Güvenlik ve Savunmaya Yönelik Silah, Araç, Gereç ve Savaş Teçhizatı İşletme, Bakım ve İdame Giderleri', HESAPKOD='48103027002' WHERE YIL=2021 AND HESAPKOD='48103020702'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Mühimmat Alımları', HESAPKOD='48103027003' WHERE YIL=2021 AND HESAPKOD='48103020703'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Güvenlik ve Savunmaya Yönelik Araştırma-Geliştirme Giderleri', HESAPKOD='48103027004' WHERE YIL=2021 AND HESAPKOD='48103020704'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Güvenlik ve Savunmaya Yönelik Hizmet Alım Giderleri', HESAPKOD='48103027005' WHERE YIL=2021 AND HESAPKOD='48103020705'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Savunma Projeleri ve Acil İhtiyaç Giderleri', HESAPKOD='48103027006' WHERE YIL=2021 AND HESAPKOD='48103020706'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Güvenlik ve Savunmaya Yönelik Diğer Giderler', HESAPKOD='48103027009' WHERE YIL=2021 AND HESAPKOD='48103020709'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Güvenlik ve Savunmaya Yönelik Makine-Teçhizat Alımları', HESAPKOD='48103027011' WHERE YIL=2021 AND HESAPKOD='48103020711'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Güvenlik ve Savunmaya Yönelik Makine-Teçhizat Büyük Onarımları', HESAPKOD='48103027012' WHERE YIL=2021 AND HESAPKOD='48103020712'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Güvenlik ve Savunmaya Yönelik Gayrimenkul Yapım Giderleri', HESAPKOD='48103027021' WHERE YIL=2021 AND HESAPKOD='48103020721'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Güvenlik ve Savunmaya Yönelik Gayrimenkul Büyük Onarım Giderleri', HESAPKOD='48103027022' WHERE YIL=2021 AND HESAPKOD='48103020722'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='NATO Altyapısına İlişkin Gayrimenkul Yapım Giderleri', HESAPKOD='48103027031' WHERE YIL=2021 AND HESAPKOD='48103020731'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='NATO Altyapısına İlişkin Gayrimenkul Büyük Onarım Giderleri', HESAPKOD='48103027032' WHERE YIL=2021 AND HESAPKOD='48103020732'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Savunma Mal ve  Malzeme Alımları ve Yapımları', HESAPKOD='48103027090' WHERE YIL=2021 AND HESAPKOD='48103020790'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='NATO Giderleri İle Gayrimenkul Alım ve Kamulaştırma Giderleri', HESAPKOD='481030280' WHERE YIL=2021 AND HESAPKOD='481030208'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Gayrimenkul Alım ve Kamulaştırma Giderleri', HESAPKOD='48103028001' WHERE YIL=2021 AND HESAPKOD='48103020801'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='NATO Altyapısına İlişkin Gayrimenkul Alım ve Kamulaştırma Giderleri', HESAPKOD='48103028002' WHERE YIL=2021 AND HESAPKOD='48103020802'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='NATO Giderleri', HESAPKOD='48103028003' WHERE YIL=2021 AND HESAPKOD='48103020803'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Tüketim Mal ve Malzemesi Alımları', HESAPKOD='481030290' WHERE YIL=2021 AND HESAPKOD='481030209'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Tüketim Mal ve Malzemesi Alımları', HESAPKOD='48103029090' WHERE YIL=2021 AND HESAPKOD='48103020990'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Görev Giderleri', HESAPKOD='4810304' WHERE YIL=2021 AND HESAPKOD='4810304'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yasal Giderler', HESAPKOD='481030420' WHERE YIL=2021 AND HESAPKOD='481030402'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kusursuz Tazminatlar', HESAPKOD='48103042002' WHERE YIL=2021 AND HESAPKOD='48103040203'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Mahkeme Harç ve Giderleri', HESAPKOD='48103047001' WHERE YIL=2021 AND HESAPKOD='48103040204'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Yasal Giderler', HESAPKOD='48103042090' WHERE YIL=2021 AND HESAPKOD='48103040290'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Ödenecek Vergi, Resim,  Harçlar ve Benzeri Giderler', HESAPKOD='481030430' WHERE YIL=2021 AND HESAPKOD='481030403'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Vergi Ödemeleri ve Benzeri Giderler', HESAPKOD='48103043001' WHERE YIL=2021 AND HESAPKOD='48103040301'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='İşletme Ruhsatı Ödemeleri ve Benzeri Giderler', HESAPKOD='48103043002' WHERE YIL=2021 AND HESAPKOD='48103040302'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Vergi, Resim ve Harçlar ve Benzeri Giderler', HESAPKOD='48103043090' WHERE YIL=2021 AND HESAPKOD='48103040390'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Hizmet Alımları', HESAPKOD='4810305' WHERE YIL=2021 AND HESAPKOD='4810305'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Genel Hizmet Alımları ', HESAPKOD='481030510' WHERE YIL=2021 AND HESAPKOD='481030501'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Etüt-Proje Bilirkişi Ekspertiz Giderleri', HESAPKOD='48103051001' WHERE YIL=2021 AND HESAPKOD='48103050101'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Araştırma ve Geliştirme Hizmet Alım Giderleri', HESAPKOD='48103051002' WHERE YIL=2021 AND HESAPKOD='48103050102'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Bilgisayar Hizmet Alım Giderleri', HESAPKOD='48103051003' WHERE YIL=2021 AND HESAPKOD='48103050103'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Genel Hizmet Alımları', HESAPKOD='48103051090' WHERE YIL=2021 AND HESAPKOD='48103050104'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Harita Yapım ve Alım Giderleri', HESAPKOD='48103051004' WHERE YIL=2021 AND HESAPKOD='48103050105'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Enformasyon ve Raporlama Hizmet Alım Giderleri', HESAPKOD='48103051005' WHERE YIL=2021 AND HESAPKOD='48103050106'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Danışma Yönetim ve İşletim Hizmet Alım Giderleri', HESAPKOD='48103051006' WHERE YIL=2021 AND HESAPKOD='48103050107'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Genel Hizmet Alımları', HESAPKOD='48103051090' WHERE YIL=2021 AND HESAPKOD='48103050108'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Genel Hizmet Alımları', HESAPKOD='48103051090' WHERE YIL=2021 AND HESAPKOD='48103050190'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kiralar', HESAPKOD='481030550' WHERE YIL=2021 AND HESAPKOD='481030505'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Dayanıklı Mal ve Malzeme Kiralama Giderleri', HESAPKOD='48103055001' WHERE YIL=2021 AND HESAPKOD='48103050501'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Taşıt Kiralama Giderleri', HESAPKOD='48103055002' WHERE YIL=2021 AND HESAPKOD='48103050502'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='İş Makinası Kiralama Giderleri', HESAPKOD='48103055003' WHERE YIL=2021 AND HESAPKOD='48103050503'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Canlı Hayvan Kiralama Giderleri', HESAPKOD='48103055004' WHERE YIL=2021 AND HESAPKOD='48103050504'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Hizmet Binası Kiralama Giderleri', HESAPKOD='48103055005' WHERE YIL=2021 AND HESAPKOD='48103050505'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Lojman Kiralama Giderleri', HESAPKOD='48103055006' WHERE YIL=2021 AND HESAPKOD='48103050506'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Arsa ve Arazi Kiralama Giderleri', HESAPKOD='48103055007' WHERE YIL=2021 AND HESAPKOD='48103050507'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yüzer Taşıt Kiralama Giderleri', HESAPKOD='48103055008' WHERE YIL=2021 AND HESAPKOD='48103050508'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Hava Taşıtı Kiralama Giderleri', HESAPKOD='48103055009' WHERE YIL=2021 AND HESAPKOD='48103050509'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Bilgisayar, Bilgisayar Sistemleri ve Yazılımları Kiralama Giderleri', HESAPKOD='48103055010' WHERE YIL=2021 AND HESAPKOD='48103050510'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Tersane Kiralama Giderleri', HESAPKOD='48103055011' WHERE YIL=2021 AND HESAPKOD='48103050511'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Personel Servisi Kiralama Giderleri', HESAPKOD='48103055012' WHERE YIL=2021 AND HESAPKOD='48103050512'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Kiralama Giderleri', HESAPKOD='48103055090' WHERE YIL=2021 AND HESAPKOD='48103050590'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Hizmet Alımları', HESAPKOD='481030590' WHERE YIL=2021 AND HESAPKOD='481030509'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sınıflandırmaya Girmeyen Diğer Hizmet Alımları', HESAPKOD='48103059090' WHERE YIL=2021 AND HESAPKOD='48103050990'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Temsil ve Tanıtma Giderleri', HESAPKOD='4810306' WHERE YIL=2021 AND HESAPKOD='4810306'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Menkul Mal, Gayrimaddi Hak Alım, Bakım ve Onarım Giderleri', HESAPKOD='4810307' WHERE YIL=2021 AND HESAPKOD='4810307'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Gayrimenkul Mal Bakım ve Onarım Giderleri', HESAPKOD='4810308' WHERE YIL=2021 AND HESAPKOD='4810308'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Faiz Giderleri', HESAPKOD='48104' WHERE YIL=2021 AND HESAPKOD='48104'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kamu Kurumlarına Ödenen İç Borç Faiz Giderleri', HESAPKOD='4810401' WHERE YIL=2021 AND HESAPKOD='4810401'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Devlet Tahvili', HESAPKOD='481040110' WHERE YIL=2021 AND HESAPKOD='481040101'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='TL Cinsinden Tahvillerin Faiz Giderleri', HESAPKOD='48104011001' WHERE YIL=2021 AND HESAPKOD='48104010101'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Dövize Endeksli ve Döviz Cinsinden Tahvillerin Faiz Giderleri', HESAPKOD='48104011002' WHERE YIL=2021 AND HESAPKOD='48104010102'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Hazine Bonosu', HESAPKOD='481040120' WHERE YIL=2021 AND HESAPKOD='481040102'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='TL Cinsinden Bonoların Faiz Giderleri', HESAPKOD='48104012001' WHERE YIL=2021 AND HESAPKOD='48104010201'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Dövize Endeksli ve Döviz Cinsinden Bonoların Faiz Giderleri', HESAPKOD='48104012002' WHERE YIL=2021 AND HESAPKOD='48104010202'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer İç Borç Faiz Giderleri', HESAPKOD='4810402' WHERE YIL=2021 AND HESAPKOD='4810402'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Devlet Tahvili', HESAPKOD='481040210' WHERE YIL=2021 AND HESAPKOD='481040201'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='TL Cinsinden Tahvillerin Faiz Giderleri', HESAPKOD='48104021001' WHERE YIL=2021 AND HESAPKOD='48104020101'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Dövize Endeksli ve Döviz Cinsinden Tahvillerin Faiz Giderleri', HESAPKOD='48104021002' WHERE YIL=2021 AND HESAPKOD='48104020102'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Hazine Bonosu', HESAPKOD='481040220' WHERE YIL=2021 AND HESAPKOD='481040202'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='TL Cinsinden Bonoların Faiz Giderleri', HESAPKOD='48104022001' WHERE YIL=2021 AND HESAPKOD='48104020201'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Dövize Endeksli ve Döviz Cinsinden Bonoların Faiz Giderleri', HESAPKOD='48104022002' WHERE YIL=2021 AND HESAPKOD='48104020202'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer İç Borç Faiz Giderleri ', HESAPKOD='481040290' WHERE YIL=2021 AND HESAPKOD='481040209'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='TL Cinsinden Diğer İç Borç Faiz Giderleri', HESAPKOD='48104029001' WHERE YIL=2021 AND HESAPKOD='48104020901'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Dövize Endeksli ve Döviz Cinsinden Diğer İç Borç Faiz Giderleri ', HESAPKOD='48104029002' WHERE YIL=2021 AND HESAPKOD='48104020902'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Dış Borç Faiz Giderleri', HESAPKOD='4810403' WHERE YIL=2021 AND HESAPKOD='4810403'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='ABD Doları Cinsinden Dış Borç Faiz Giderleri', HESAPKOD='481040310' WHERE YIL=2021 AND HESAPKOD='481040301'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='ABD Doları Cinsinden Dış Borç Faiz Giderleri', HESAPKOD='48104031001' WHERE YIL=2021 AND HESAPKOD='48104030101'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Avro Cinsinden Borç Faiz Giderleri', HESAPKOD='481040320' WHERE YIL=2021 AND HESAPKOD='481040302'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Avro Cinsinden Dış Borç Faiz Giderleri', HESAPKOD='48104032001' WHERE YIL=2021 AND HESAPKOD='48104030201'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Japon Yeni Cinsinden Dış Borç Faiz Giderleri', HESAPKOD='481040330' WHERE YIL=2021 AND HESAPKOD='481040303'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Japon Yeni Cinsinden Dış Borç Faiz Giderleri', HESAPKOD='48104033001' WHERE YIL=2021 AND HESAPKOD='48104030301'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Dış Borç Faiz Giderleri', HESAPKOD='481040390' WHERE YIL=2021 AND HESAPKOD='481040309'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Dış Borç Faiz Giderleri', HESAPKOD='48104039001' WHERE YIL=2021 AND HESAPKOD='48104030901'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='İskonto Giderleri', HESAPKOD='4810404' WHERE YIL=2021 AND HESAPKOD='4810404'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='İç Borç İskonto Giderleri', HESAPKOD='481040410' WHERE YIL=2021 AND HESAPKOD='481040401'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Devlet Tahvili İskonto Giderleri', HESAPKOD='48104041001' WHERE YIL=2021 AND HESAPKOD='48104040101'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Hazine Bonosu İskonto Giderleri', HESAPKOD='48104041002' WHERE YIL=2021 AND HESAPKOD='48104040102'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Dış Borç İskonto Giderleri', HESAPKOD='481040420' WHERE YIL=2021 AND HESAPKOD='481040402'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Dış Borç İskonto Giderleri', HESAPKOD='48104042001' WHERE YIL=2021 AND HESAPKOD='48104040201'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Para Piyasası Nakit İşlemleri Faiz Giderleri', HESAPKOD='4810405' WHERE YIL=2021 AND HESAPKOD='4810405'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Para Piyasası Nakit İşlemleri Faiz Giderleri ', HESAPKOD='481040510' WHERE YIL=2021 AND HESAPKOD='481040501'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='TL Cinsinden Para Piyasası Nakit İşlemleri Faiz Giderleri ', HESAPKOD='48104051001' WHERE YIL=2021 AND HESAPKOD='48104050101'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Döviz Cinsinden Kısa Vadeli Nakit İşlemlere Ait Faiz Giderleri', HESAPKOD='48104051002' WHERE YIL=2021 AND HESAPKOD='48104050102'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Cari Transferler', HESAPKOD='48105' WHERE YIL=2021 AND HESAPKOD='48105'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Hane Halkı ve İşletmelere Yapılan Transferler', HESAPKOD='4810504' WHERE YIL=2021 AND HESAPKOD='4810504'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Ekonomik/Mali Amaçlı Transferler', HESAPKOD='481050480' WHERE YIL=2021 AND HESAPKOD='481050408'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='KREDİ FAİZ DESTEKLERİ', HESAPKOD='4810504809001' WHERE YIL=2021 AND HESAPKOD='4810504080101'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='2016 Yılı Makine Teçhizat Kredi Faiz Desteği', HESAPKOD='481050480900101' WHERE YIL=2021 AND HESAPKOD='481050408010101'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='GİDERLER HESABI', HESAPKOD='630' WHERE YIL=2021 AND HESAPKOD='630'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Personel Giderleri', HESAPKOD='63001' WHERE YIL=2021 AND HESAPKOD='63001'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Memurlar', HESAPKOD='6300101' WHERE YIL=2021 AND HESAPKOD='6300101'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Temel Maaşlar', HESAPKOD='630010110' WHERE YIL=2021 AND HESAPKOD='630010101'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Temel Maaşlar', HESAPKOD='63001011001' WHERE YIL=2021 AND HESAPKOD='63001010101'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Taban Aylığı', HESAPKOD='63001011002' WHERE YIL=2021 AND HESAPKOD='63001010102'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yurtdışı Aylığı', HESAPKOD='63001011003' WHERE YIL=2021 AND HESAPKOD='63001010103'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Zamlar ve Tazminatlar', HESAPKOD='630010120' WHERE YIL=2021 AND HESAPKOD='630010102'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Zamlar ve Tazminatlar', HESAPKOD='63001012001' WHERE YIL=2021 AND HESAPKOD='63001010201'
DELETE FROM  MUHHESAPPLANI WHERE YIL=2021 AND HESAPKOD='63001010226'
DELETE FROM  MUHHESAPPLANI WHERE YIL=2021 AND HESAPKOD='63001010256'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Ödenekler', HESAPKOD='630010130' WHERE YIL=2021 AND HESAPKOD='630010103'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Ödenekler', HESAPKOD='63001013001' WHERE YIL=2021 AND HESAPKOD='63001010301'
DELETE FROM  MUHHESAPPLANI WHERE YIL=2021 AND HESAPKOD='63001010322'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sosyal Haklar', HESAPKOD='630010140' WHERE YIL=2021 AND HESAPKOD='630010104'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sosyal Haklar', HESAPKOD='63001014001' WHERE YIL=2021 AND HESAPKOD='63001010401'
DELETE FROM  MUHHESAPPLANI WHERE YIL=2021 AND HESAPKOD='63001010430'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Ek Çalışma Karşılıkları', HESAPKOD='630010150' WHERE YIL=2021 AND HESAPKOD='630010105'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Ek Çalışma Karşılıkları', HESAPKOD='63001015001' WHERE YIL=2021 AND HESAPKOD='63001010501'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='İş Sağlığı ve Güvenliği Hizmetleri Görevlendirme Ücretleri', HESAPKOD='63001015002' WHERE YIL=2021 AND HESAPKOD='63001010502'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Ek Ders Ücretleri', HESAPKOD='63001015003' WHERE YIL=2021 AND HESAPKOD='63001010503'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Destekleme ve Yetiştirme Kursları Kapsamında Ödenen Ücretler', HESAPKOD='63001015004' WHERE YIL=2021 AND HESAPKOD='63001010504'
DELETE FROM  MUHHESAPPLANI WHERE YIL=2021 AND HESAPKOD='63001010505'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Ödül ve İkramiyeler', HESAPKOD='630010160' WHERE YIL=2021 AND HESAPKOD='630010106'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Ödül ve İkramiyeler', HESAPKOD='63001016001' WHERE YIL=2021 AND HESAPKOD='63001010601'
DELETE FROM  MUHHESAPPLANI WHERE YIL=2021 AND HESAPKOD='630010108'
DELETE FROM  MUHHESAPPLANI WHERE YIL=2021 AND HESAPKOD='63001010801'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Giderler', HESAPKOD='630010190' WHERE YIL=2021 AND HESAPKOD='630010109'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Giderler', HESAPKOD='63001019001' WHERE YIL=2021 AND HESAPKOD='63001010901'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sözleşmeli Personel', HESAPKOD='6300102' WHERE YIL=2021 AND HESAPKOD='6300102'
DELETE FROM  MUHHESAPPLANI WHERE YIL=2021 AND HESAPKOD='63001020002'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Ücretler', HESAPKOD='630010210' WHERE YIL=2021 AND HESAPKOD='630010201'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='657 SK 4/B Sözleşmeli Personel Ücretleri', HESAPKOD='63001021001' WHERE YIL=2021 AND HESAPKOD='63001020101'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kadro Karşılığı Sözleşmeli Personel Ücretleri', HESAPKOD='63001021002' WHERE YIL=2021 AND HESAPKOD='63001020102'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Akademik Sözleşmeli Personelin Ücretleri', HESAPKOD='63001021003' WHERE YIL=2021 AND HESAPKOD='63001020103'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yabancı Uyruklu Sözleşmeli Personelin Ücretleri', HESAPKOD='63001021004' WHERE YIL=2021 AND HESAPKOD='63001020104'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sözleşmeli Sanatçıların  Ücretleri', HESAPKOD='63001021005' WHERE YIL=2021 AND HESAPKOD='63001020105'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yurtdışı Sözleşmeli Personelin Ücretleri', HESAPKOD='63001021006' WHERE YIL=2021 AND HESAPKOD='63001020106'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Sözleşmeli Personel Ücretleri', HESAPKOD='63001021090' WHERE YIL=2021 AND HESAPKOD='63001020190'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Zamlar ve Tazminatlar', HESAPKOD='630010220' WHERE YIL=2021 AND HESAPKOD='630010202'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='657 SK 4/B Sözleşmeli Personel Zam ve Tazminatları', HESAPKOD='63001022001' WHERE YIL=2021 AND HESAPKOD='63001020201'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kadro Karşılığı Sözleşmeli Personel Zam ve Tazminatları', HESAPKOD='63001022002' WHERE YIL=2021 AND HESAPKOD='63001020202'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Akademik Sözleşmeli Personelin Zam ve Tazminatları', HESAPKOD='63001022003' WHERE YIL=2021 AND HESAPKOD='63001020203'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yabancı Uyruklu Sözleşmeli Personelin Zam ve Tazminatları', HESAPKOD='63001022004' WHERE YIL=2021 AND HESAPKOD='63001020204'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sözleşmeli Sanatçıların  Zam ve Tazminatları', HESAPKOD='63001022005' WHERE YIL=2021 AND HESAPKOD='63001020205'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yurtdışı Sözleşmeli Personelin Zam ve Tazminatları', HESAPKOD='63001022006' WHERE YIL=2021 AND HESAPKOD='63001020206'
DELETE FROM  MUHHESAPPLANI WHERE YIL=2021 AND HESAPKOD='63001020212'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Sözleşmeli Personel Zam ve Tazminatları', HESAPKOD='63001022090' WHERE YIL=2021 AND HESAPKOD='63001020290'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Ödenekler', HESAPKOD='630010230' WHERE YIL=2021 AND HESAPKOD='630010203'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='657 SK 4/B Sözleşmeli Personel Ödenekleri', HESAPKOD='63001023001' WHERE YIL=2021 AND HESAPKOD='63001020301'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kadro Karşılığı Sözleşmeli Personel Ödenekleri', HESAPKOD='63001023002' WHERE YIL=2021 AND HESAPKOD='63001020302'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Akademik Sözleşmeli Personelin Ödenekleri', HESAPKOD='63001023003' WHERE YIL=2021 AND HESAPKOD='63001020303'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yabancı Uyruklu Sözleşmeli Personelin Ödenekleri', HESAPKOD='63001023004' WHERE YIL=2021 AND HESAPKOD='63001020304'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sözleşmeli Sanatçıların Ödenekleri', HESAPKOD='63001023005' WHERE YIL=2021 AND HESAPKOD='63001020305'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yurtdışı Sözleşmeli Personelin Ödenekleri', HESAPKOD='63001023006' WHERE YIL=2021 AND HESAPKOD='63001020306'
DELETE FROM  MUHHESAPPLANI WHERE YIL=2021 AND HESAPKOD='63001020310'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Sözleşmeli Personel Ödenekleri', HESAPKOD='63001023090' WHERE YIL=2021 AND HESAPKOD='63001020390'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sosyal Haklar', HESAPKOD='630010240' WHERE YIL=2021 AND HESAPKOD='630010204'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='657 SK 4/B Sözleşmeli Personel Sosyal Hakları', HESAPKOD='63001024001' WHERE YIL=2021 AND HESAPKOD='63001020401'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kadro Karşılığı Sözleşmeli Personel Sosyal Hakları', HESAPKOD='63001024002' WHERE YIL=2021 AND HESAPKOD='63001020402'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Akademik Sözleşmeli Personelin Sosyal Hakları', HESAPKOD='63001024003' WHERE YIL=2021 AND HESAPKOD='63001020403'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yabancı Uyruklu Sözleşmeli Personelin Sosyal Hakları', HESAPKOD='63001024004' WHERE YIL=2021 AND HESAPKOD='63001020404'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sözleşmeli Sanatçıların Sosyal Hakları', HESAPKOD='63001024005' WHERE YIL=2021 AND HESAPKOD='63001020405'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yurtdışı Sözleşmeli Personelin Sosyal Hakları', HESAPKOD='63001024006' WHERE YIL=2021 AND HESAPKOD='63001020406'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Sözleşmeli Personelin Sosyal Hakları', HESAPKOD='63001024090' WHERE YIL=2021 AND HESAPKOD='63001020490'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Ek Çalışma Karşılıkları', HESAPKOD='630010250' WHERE YIL=2021 AND HESAPKOD='630010205'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='657 SK 4/B Sözleşmeli Personelin Ek Çalışma Karşılıkları', HESAPKOD='63001025001' WHERE YIL=2021 AND HESAPKOD='63001020501'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kadro Karşılığı Sözleşmeli Personelin Ek Çalışma Karşılıkları', HESAPKOD='63001025002' WHERE YIL=2021 AND HESAPKOD='63001020502'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Akademik Sözleşmeli Personelin Ek Çalışma Karşılıkları', HESAPKOD='63001025003' WHERE YIL=2021 AND HESAPKOD='63001020503'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yabancı Uyruklu Sözleşmeli Personelin Ek Çalışma Karşılıkları', HESAPKOD='63001025004' WHERE YIL=2021 AND HESAPKOD='63001020504'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sözleşmeli Sanatçıların Ek Çalışma Karşılıkları', HESAPKOD='63001025005' WHERE YIL=2021 AND HESAPKOD='63001020505'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yurtdışı Sözleşmeli Personelin Ek Çalışma Karşılıkları', HESAPKOD='63001025006' WHERE YIL=2021 AND HESAPKOD='63001020506'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Sözleşmeli Personelin Ek Çalışma Karşılıkları', HESAPKOD='63001025090' WHERE YIL=2021 AND HESAPKOD='63001020590'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Ödül ve İkramiyeler', HESAPKOD='630010260' WHERE YIL=2021 AND HESAPKOD='630010206'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='657 SK 4/B Sözleşmeli Personelin Ödül ve İkramiyeleri', HESAPKOD='63001026001' WHERE YIL=2021 AND HESAPKOD='63001020601'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kadro Karşılığı Sözleşmeli Personelin Ödül ve İkramiyeleri', HESAPKOD='63001026002' WHERE YIL=2021 AND HESAPKOD='63001020602'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Akademik Sözleşmeli Personelin Ödül ve İkramiyeleri', HESAPKOD='63001026003' WHERE YIL=2021 AND HESAPKOD='63001020603'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yabancı Uyruklu Sözleşmeli Personelin Ödül ve İkramiyeleri', HESAPKOD='63001026004' WHERE YIL=2021 AND HESAPKOD='63001020604'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sözleşmeli Sanatçıların Ödül ve İkramiyeleri', HESAPKOD='63001026005' WHERE YIL=2021 AND HESAPKOD='63001020605'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yurtdışı Sözleşmeli Personelin Ödül ve İkramiyeleri', HESAPKOD='63001026006' WHERE YIL=2021 AND HESAPKOD='63001020606'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Sözleşmeli Personelin Ödül ve İkramiyeleri', HESAPKOD='63001026090' WHERE YIL=2021 AND HESAPKOD='63001020690'
DELETE FROM  MUHHESAPPLANI WHERE YIL=2021 AND HESAPKOD='630010208'
DELETE FROM  MUHHESAPPLANI WHERE YIL=2021 AND HESAPKOD='63001020801'
DELETE FROM  MUHHESAPPLANI WHERE YIL=2021 AND HESAPKOD='63001020802'
DELETE FROM  MUHHESAPPLANI WHERE YIL=2021 AND HESAPKOD='63001020803'
DELETE FROM  MUHHESAPPLANI WHERE YIL=2021 AND HESAPKOD='63001020804'
DELETE FROM  MUHHESAPPLANI WHERE YIL=2021 AND HESAPKOD='63001020805'
DELETE FROM  MUHHESAPPLANI WHERE YIL=2021 AND HESAPKOD='63001020890'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Giderler', HESAPKOD='630010290' WHERE YIL=2021 AND HESAPKOD='630010209'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='657 SK 4/B Sözleşmeli Personelin Diğer Giderleri', HESAPKOD='63001029001' WHERE YIL=2021 AND HESAPKOD='63001020901'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kadro Karşılığı Sözleşmeli Personelin Diğer Giderleri', HESAPKOD='63001029002' WHERE YIL=2021 AND HESAPKOD='63001020902'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Akademik Sözleşmeli Personelin Diğer Giderleri', HESAPKOD='63001029003' WHERE YIL=2021 AND HESAPKOD='63001020903'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yabancı Uyruklu Sözleşmeli Personelin Diğer Giderleri', HESAPKOD='63001029004' WHERE YIL=2021 AND HESAPKOD='63001020904'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sözleşmeli Sanatçıların Diğer Giderleri', HESAPKOD='63001029005' WHERE YIL=2021 AND HESAPKOD='63001020905'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yurtdışı Sözleşmeli Personelin Diğer Giderleri', HESAPKOD='63001029006' WHERE YIL=2021 AND HESAPKOD='63001020906'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Sözleşmeli Personelin Diğer Giderleri', HESAPKOD='63001029090' WHERE YIL=2021 AND HESAPKOD='63001020990'
DELETE FROM  MUHHESAPPLANI WHERE YIL=2021 AND HESAPKOD='63001020991'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='İşçiler', HESAPKOD='6300103' WHERE YIL=2021 AND HESAPKOD='6300103'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Ücretler', HESAPKOD='630010310' WHERE YIL=2021 AND HESAPKOD='630010301'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sürekli işçilerin Ücretleri', HESAPKOD='63001031001' WHERE YIL=2021 AND HESAPKOD='63001030101'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Geçici işçilerin Ücretleri', HESAPKOD='63001031002' WHERE YIL=2021 AND HESAPKOD='63001030102'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='375 S KHK’nın Geçici 23 üncü Md Kapsamında Sürekli İşçi Kadrolarına Geçirilen İşçilerin Ücretleri', HESAPKOD='63001031003' WHERE YIL=2021 AND HESAPKOD='63001030103'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='375 S KHK’nın Geçici 23 üncü Md Kapsamında Geçici İşçi Pozisyonlarına Geçirilen İşçilerin Ücretleri', HESAPKOD='63001031004' WHERE YIL=2021 AND HESAPKOD='63001030104'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='İhbar ve Kıdem Tazminatları', HESAPKOD='630010320' WHERE YIL=2021 AND HESAPKOD='630010302'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sürekli İşçilerin İhbar ve Kıdem Tazminatları', HESAPKOD='63001032001' WHERE YIL=2021 AND HESAPKOD='63001030201'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Geçici İşçilerin İhbar ve Kıdem Tazminatları', HESAPKOD='63001032002' WHERE YIL=2021 AND HESAPKOD='63001030202'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='375 S KHK’nın Geçici 23 üncü Md Kapsamında Sürekli İşçi Kadrolarına Geçirilen İşçilerin İhbar ve Kıdem Tazminatları', HESAPKOD='63001032003' WHERE YIL=2021 AND HESAPKOD='63001030203'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='375 S KHK’nın Geçici 23 üncü Md Kapsamında Geçici İşçi Pozisyonlarına Geçirilen İşçilerin İhbar ve Kıdem Tazminatları', HESAPKOD='63001032004' WHERE YIL=2021 AND HESAPKOD='63001030204'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sosyal Haklar', HESAPKOD='630010330' WHERE YIL=2021 AND HESAPKOD='630010303'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sürekli İşçilerin Sosyal Hakları', HESAPKOD='63001033001' WHERE YIL=2021 AND HESAPKOD='63001030301'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Geçici İşçilerin Sosyal Hakları', HESAPKOD='63001033002' WHERE YIL=2021 AND HESAPKOD='63001030302'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='375 S KHK’nın Geçici 23 üncü Md Kapsamında Sürekli İşçi Kadrolarına Geçirilen İşçilerin Sosyal Hakları', HESAPKOD='63001033003' WHERE YIL=2021 AND HESAPKOD='63001030303'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='375 S KHK’nın Geçici 23 üncü Md Kapsamında Geçici İşçi Pozisyonlarına Geçirilen İşçilerin Sosyal Hakları', HESAPKOD='63001033004' WHERE YIL=2021 AND HESAPKOD='63001030304'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Fazla Mesailer', HESAPKOD='630010340' WHERE YIL=2021 AND HESAPKOD='630010304'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sürekli İşçilerin Fazla Mesaileri', HESAPKOD='63001034001' WHERE YIL=2021 AND HESAPKOD='63001030401'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Geçici İşçilerin Fazla Mesaileri', HESAPKOD='63001034002' WHERE YIL=2021 AND HESAPKOD='63001030402'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='375 S KHK’nın Geçici 23 üncü Md Kapsamında Sürekli İşçi Kadrolarına Geçirilen İşçilerin Fazla Mesaileri', HESAPKOD='63001034003' WHERE YIL=2021 AND HESAPKOD='63001030403'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='375 S KHK’nın Geçici 23 üncü Md Kapsamında Geçici İşçi Pozisyonlarına Geçirilen İşçilerin Fazla Mesaileri', HESAPKOD='63001034004' WHERE YIL=2021 AND HESAPKOD='63001030404'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Ödül ve İkramiyeler', HESAPKOD='630010350' WHERE YIL=2021 AND HESAPKOD='630010305'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sürekli İşçilerin Ödül ve İkramiyeleri', HESAPKOD='63001035001' WHERE YIL=2021 AND HESAPKOD='63001030501'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Geçici İşçilerin Ödül ve İkramiyeleri', HESAPKOD='63001035002' WHERE YIL=2021 AND HESAPKOD='63001030502'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='375 S KHK’nın Geçici 23 üncü Md Kapsamında Sürekli İşçi Kadrolarına Geçirilen İşçilerin Ödül ve İkramiyeleri', HESAPKOD='63001035003' WHERE YIL=2021 AND HESAPKOD='63001030503'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='375 S KHK’nın Geçici 23 üncü Md Kapsamında Geçici İşçi Pozisyonlarına Geçirilen İşçilerin Ödül ve İkramiyeleri', HESAPKOD='63001035004' WHERE YIL=2021 AND HESAPKOD='63001030504'
DELETE FROM  MUHHESAPPLANI WHERE YIL=2021 AND HESAPKOD='630010308'
DELETE FROM  MUHHESAPPLANI WHERE YIL=2021 AND HESAPKOD='63001030801'
DELETE FROM  MUHHESAPPLANI WHERE YIL=2021 AND HESAPKOD='63001030802'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Ödemeler', HESAPKOD='630010390' WHERE YIL=2021 AND HESAPKOD='630010309'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sürekli işçilerin Diğer Ödemeleri', HESAPKOD='63001039001' WHERE YIL=2021 AND HESAPKOD='63001030901'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Geçici işçilerin Diğer Ödemeleri', HESAPKOD='63001039002' WHERE YIL=2021 AND HESAPKOD='63001030902'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='375 S KHK’nın Geçici 23 üncü Md Kapsamında Sürekli İşçi Kadrolarına Geçirilen İşçilerin Diğer Ödemeleri', HESAPKOD='63001039003' WHERE YIL=2021 AND HESAPKOD='63001030903'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='375 S KHK’nın Geçici 23 üncü Md Kapsamında Geçici İşçi Pozisyonlarına Geçirilen İşçilerin Diğer Ödemeleri', HESAPKOD='63001039004' WHERE YIL=2021 AND HESAPKOD='63001030904'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Geçici Süreli Çalışanlar', HESAPKOD='6300104' WHERE YIL=2021 AND HESAPKOD='6300104'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Ücretler', HESAPKOD='630010410' WHERE YIL=2021 AND HESAPKOD='630010401'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Vizesiz Geçici İşçilerin Ücretleri', HESAPKOD='63001041001' WHERE YIL=2021 AND HESAPKOD='63001040101'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Aday Çırak, Çırak ve Stajyer Öğrencilerin Ücretleri', HESAPKOD='63001041002' WHERE YIL=2021 AND HESAPKOD='63001040102'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Usta Öğreticilere Yapılacak Ödemeler', HESAPKOD='63001041003' WHERE YIL=2021 AND HESAPKOD='63001040103'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Geçici Süreli Çalışanlara Yapılacak Ödemeler', HESAPKOD='63001041090' WHERE YIL=2021 AND HESAPKOD='63001040104'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kısmi Zamanlı Çalışan Öğrencilerin Ücretleri', HESAPKOD='63001041004' WHERE YIL=2021 AND HESAPKOD='63001040105'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Ders Ücreti Karşılığında Görevlendirilenlerin Ücretleri', HESAPKOD='63001041005' WHERE YIL=2021 AND HESAPKOD='63001040106'
DELETE FROM  MUHHESAPPLANI WHERE YIL=2021 AND HESAPKOD='63001040107'
DELETE FROM  MUHHESAPPLANI WHERE YIL=2021 AND HESAPKOD='63001040108'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Geçici Süreli Çalışanlara Yapılacak Ödemeler', HESAPKOD='63001041090' WHERE YIL=2021 AND HESAPKOD='63001040190'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Geçici Süreli Çalışanlara Yapılacak Ödemeler', HESAPKOD='63001041090' WHERE YIL=2021 AND HESAPKOD='630010402'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Geçici Süreli Çalışanlara Yapılacak Ödemeler', HESAPKOD='63001041090' WHERE YIL=2021 AND HESAPKOD='63001040204'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Geçici Süreli Çalışanlara Yapılacak Ödemeler', HESAPKOD='63001041090' WHERE YIL=2021 AND HESAPKOD='630010403'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Geçici Süreli Çalışanlara Yapılacak Ödemeler', HESAPKOD='63001041090' WHERE YIL=2021 AND HESAPKOD='63001040304'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Geçici Süreli Çalışanlara Yapılacak Ödemeler', HESAPKOD='63001041090' WHERE YIL=2021 AND HESAPKOD='630010404'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Geçici Süreli Çalışanlara Yapılacak Ödemeler', HESAPKOD='63001041090' WHERE YIL=2021 AND HESAPKOD='63001040404'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Personel', HESAPKOD='6300105' WHERE YIL=2021 AND HESAPKOD='6300105'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Ücret ve Diğer Ödemeler', HESAPKOD='630010510' WHERE YIL=2021 AND HESAPKOD='630010501'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Muhtarların Ücretleri', HESAPKOD='63001051001' WHERE YIL=2021 AND HESAPKOD='63001050101'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Güvenlik Korucularının Ücretleri', HESAPKOD='63001051002' WHERE YIL=2021 AND HESAPKOD='63001050102'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Er - Erbaş Harçlıkları', HESAPKOD='63001051003' WHERE YIL=2021 AND HESAPKOD='63001050103'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Öğrenci Harçlıkları', HESAPKOD='63001051004' WHERE YIL=2021 AND HESAPKOD='63001050104'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yurtdışı Öğrenimde Ödenen Aylıklar', HESAPKOD='63001051005' WHERE YIL=2021 AND HESAPKOD='63001050105'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Hükümlü Ücretleri', HESAPKOD='63001051006' WHERE YIL=2021 AND HESAPKOD='63001050106'
DELETE FROM  MUHHESAPPLANI WHERE YIL=2021 AND HESAPKOD='63001050188'
DELETE FROM  MUHHESAPPLANI WHERE YIL=2021 AND HESAPKOD='63001050189'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Personele Yapılan Diğer Ödemeler', HESAPKOD='63001051090' WHERE YIL=2021 AND HESAPKOD='63001050190'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Milletvekilleri', HESAPKOD='6300107' WHERE YIL=2021 AND HESAPKOD='6300107'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Zamlar ve Tazminatlar', HESAPKOD='630010720' WHERE YIL=2021 AND HESAPKOD='630010702'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Zamlar ve Tazminatlar', HESAPKOD='63001072001' WHERE YIL=2021 AND HESAPKOD='63001070201'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Ödenekler', HESAPKOD='630010730' WHERE YIL=2021 AND HESAPKOD='630010703'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Ödenekler', HESAPKOD='63001073001' WHERE YIL=2021 AND HESAPKOD='63001070301'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sosyal Haklar', HESAPKOD='630010740' WHERE YIL=2021 AND HESAPKOD='630010704'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sosyal Haklar', HESAPKOD='63001074001' WHERE YIL=2021 AND HESAPKOD='63001070401'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Cumhurbaşkanlığı Ödeneği', HESAPKOD='6300108' WHERE YIL=2021 AND HESAPKOD='6300108'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Cumhurbaşkanı Ödeneği', HESAPKOD='630010810' WHERE YIL=2021 AND HESAPKOD='630010801'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Cumhurbaşkanı Ödeneği', HESAPKOD='63001081001' WHERE YIL=2021 AND HESAPKOD='63001080101'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='İstihbarat Personeli', HESAPKOD='6300109' WHERE YIL=2021 AND HESAPKOD='6300109'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='İstihbarat Personeli Giderleri', HESAPKOD='630010910' WHERE YIL=2021 AND HESAPKOD='630010901'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='İstihbarat Personeli Giderleri', HESAPKOD='63001091001' WHERE YIL=2021 AND HESAPKOD='63001090101'
DELETE FROM  MUHHESAPPLANI WHERE YIL=2021 AND HESAPKOD='6300123'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sosyal Güvenlik Kurumlarına Devlet Primi Giderleri', HESAPKOD='63002' WHERE YIL=2021 AND HESAPKOD='63002'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Memurlar', HESAPKOD='6300201' WHERE YIL=2021 AND HESAPKOD='6300201'
DELETE FROM  MUHHESAPPLANI WHERE YIL=2021 AND HESAPKOD='630020101'
DELETE FROM  MUHHESAPPLANI WHERE YIL=2021 AND HESAPKOD='63002010101'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sosyal Güvenlik Kurumuna', HESAPKOD='630020110' WHERE YIL=2021 AND HESAPKOD='630020106'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sosyal Güvenlik Primi Ödemeleri', HESAPKOD='63002011001' WHERE YIL=2021 AND HESAPKOD='63002010601'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sağlık Primi Ödemeleri', HESAPKOD='63002011002' WHERE YIL=2021 AND HESAPKOD='63002010602'
DELETE FROM  MUHHESAPPLANI WHERE YIL=2021 AND HESAPKOD='63002010608'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sözleşmeli Personel', HESAPKOD='6300202' WHERE YIL=2021 AND HESAPKOD='6300202'
DELETE FROM  MUHHESAPPLANI WHERE YIL=2021 AND HESAPKOD='630020201'
DELETE FROM  MUHHESAPPLANI WHERE YIL=2021 AND HESAPKOD='63002020101'
DELETE FROM  MUHHESAPPLANI WHERE YIL=2021 AND HESAPKOD='630020202'
DELETE FROM  MUHHESAPPLANI WHERE YIL=2021 AND HESAPKOD='63002020201'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='İşsizlik Sigortası Fonuna', HESAPKOD='630020240' WHERE YIL=2021 AND HESAPKOD='630020204'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='İşsizlik Sigortası Fonuna', HESAPKOD='63002024001' WHERE YIL=2021 AND HESAPKOD='63002020401'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sosyal Güvenlik Kurumuna', HESAPKOD='630020210' WHERE YIL=2021 AND HESAPKOD='630020206'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sosyal Güvenlik Primi Ödemeleri', HESAPKOD='63002021001' WHERE YIL=2021 AND HESAPKOD='63002020601'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sağlık Primi Ödemeleri', HESAPKOD='63002021002' WHERE YIL=2021 AND HESAPKOD='63002020602'
DELETE FROM  MUHHESAPPLANI WHERE YIL=2021 AND HESAPKOD='63002020608'
DELETE FROM  MUHHESAPPLANI WHERE YIL=2021 AND HESAPKOD='630020209'
DELETE FROM  MUHHESAPPLANI WHERE YIL=2021 AND HESAPKOD='63002020901'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='İşçiler', HESAPKOD='6300203' WHERE YIL=2021 AND HESAPKOD='6300203'
DELETE FROM  MUHHESAPPLANI WHERE YIL=2021 AND HESAPKOD='630020302'
DELETE FROM  MUHHESAPPLANI WHERE YIL=2021 AND HESAPKOD='63002030201'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='İşsizlik Sigortası Fonuna', HESAPKOD='630020340' WHERE YIL=2021 AND HESAPKOD='630020304'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='İşsizlik Sigortası Fonuna', HESAPKOD='63002034001' WHERE YIL=2021 AND HESAPKOD='63002030401'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sosyal Güvenlik Kurumuna', HESAPKOD='630020310' WHERE YIL=2021 AND HESAPKOD='630020306'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sosyal Güvenlik Primi Ödemeleri', HESAPKOD='63002031001' WHERE YIL=2021 AND HESAPKOD='63002030601'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sağlık Primi Ödemeleri', HESAPKOD='63002031002' WHERE YIL=2021 AND HESAPKOD='63002030602'
DELETE FROM  MUHHESAPPLANI WHERE YIL=2021 AND HESAPKOD='63002030608'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Geçici Süreli Çalışanlar', HESAPKOD='6300204' WHERE YIL=2021 AND HESAPKOD='6300204'
DELETE FROM  MUHHESAPPLANI WHERE YIL=2021 AND HESAPKOD='630020401'
DELETE FROM  MUHHESAPPLANI WHERE YIL=2021 AND HESAPKOD='63002040101'
DELETE FROM  MUHHESAPPLANI WHERE YIL=2021 AND HESAPKOD='630020402'
DELETE FROM  MUHHESAPPLANI WHERE YIL=2021 AND HESAPKOD='63002040201'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='İşsizlik Sigortası Fonuna', HESAPKOD='630020440' WHERE YIL=2021 AND HESAPKOD='630020404'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='İşsizlik Sigortası Fonuna', HESAPKOD='63002044001' WHERE YIL=2021 AND HESAPKOD='63002040401'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sosyal Güvenlik Kurumuna', HESAPKOD='630020410' WHERE YIL=2021 AND HESAPKOD='630020406'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sosyal Güvenlik Primi Ödemeleri', HESAPKOD='63002041001' WHERE YIL=2021 AND HESAPKOD='63002040601'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sağlık Primi Ödemeleri', HESAPKOD='63002041002' WHERE YIL=2021 AND HESAPKOD='63002040602'
DELETE FROM  MUHHESAPPLANI WHERE YIL=2021 AND HESAPKOD='63002040608'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Personel', HESAPKOD='6300205' WHERE YIL=2021 AND HESAPKOD='6300205'
DELETE FROM  MUHHESAPPLANI WHERE YIL=2021 AND HESAPKOD='630020501'
DELETE FROM  MUHHESAPPLANI WHERE YIL=2021 AND HESAPKOD='63002050101'
DELETE FROM  MUHHESAPPLANI WHERE YIL=2021 AND HESAPKOD='630020502'
DELETE FROM  MUHHESAPPLANI WHERE YIL=2021 AND HESAPKOD='63002050201'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Özel Sigorta Prim Giderleri', HESAPKOD='630020530' WHERE YIL=2021 AND HESAPKOD='630020503'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Özel Sigorta Prim Giderleri', HESAPKOD='63002053001' WHERE YIL=2021 AND HESAPKOD='63002050301'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='İşsizlik Sigortası Fonuna', HESAPKOD='630020540' WHERE YIL=2021 AND HESAPKOD='630020504'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='İşsizlik Sigortası Fonuna', HESAPKOD='63002054001' WHERE YIL=2021 AND HESAPKOD='63002050401'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sosyal Güvenlik Kurumuna', HESAPKOD='630020510' WHERE YIL=2021 AND HESAPKOD='630020506'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sosyal Güvenlik Primi Ödemeleri', HESAPKOD='63002051001' WHERE YIL=2021 AND HESAPKOD='63002050601'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sağlık Primi Ödemeleri', HESAPKOD='63002051002' WHERE YIL=2021 AND HESAPKOD='63002050602'
DELETE FROM  MUHHESAPPLANI WHERE YIL=2021 AND HESAPKOD='63002050608'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Milletvekilleri', HESAPKOD='6300207' WHERE YIL=2021 AND HESAPKOD='6300207'
DELETE FROM  MUHHESAPPLANI WHERE YIL=2021 AND HESAPKOD='630020701'
DELETE FROM  MUHHESAPPLANI WHERE YIL=2021 AND HESAPKOD='63002070101'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sosyal Güvenlik Kurumuna', HESAPKOD='630020710' WHERE YIL=2021 AND HESAPKOD='630020706'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sosyal Güvenlik Primi Ödemeleri', HESAPKOD='63002071001' WHERE YIL=2021 AND HESAPKOD='63002070601'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sağlık Primi Ödemeleri', HESAPKOD='63002071002' WHERE YIL=2021 AND HESAPKOD='63002070602'
DELETE FROM  MUHHESAPPLANI WHERE YIL=2021 AND HESAPKOD='63002070608'
DELETE FROM  MUHHESAPPLANI WHERE YIL=2021 AND HESAPKOD='6300208'
DELETE FROM  MUHHESAPPLANI WHERE YIL=2021 AND HESAPKOD='630020806'
DELETE FROM  MUHHESAPPLANI WHERE YIL=2021 AND HESAPKOD='63002080601'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='İstihbarat Personeli', HESAPKOD='6300209' WHERE YIL=2021 AND HESAPKOD='6300209'
DELETE FROM  MUHHESAPPLANI WHERE YIL=2021 AND HESAPKOD='630020901'
DELETE FROM  MUHHESAPPLANI WHERE YIL=2021 AND HESAPKOD='63002090101'
DELETE FROM  MUHHESAPPLANI WHERE YIL=2021 AND HESAPKOD='630020902'
DELETE FROM  MUHHESAPPLANI WHERE YIL=2021 AND HESAPKOD='63002090201'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sosyal Güvenlik Kurumuna', HESAPKOD='630020910' WHERE YIL=2021 AND HESAPKOD='630020906'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sosyal Güvenlik Primi Ödemeleri', HESAPKOD='63002091001' WHERE YIL=2021 AND HESAPKOD='63002090601'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sağlık Primi Ödemeleri', HESAPKOD='63002091002' WHERE YIL=2021 AND HESAPKOD='63002090602'
DELETE FROM  MUHHESAPPLANI WHERE YIL=2021 AND HESAPKOD='63002090608'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Mal ve Hizmet Alım Giderleri', HESAPKOD='63003' WHERE YIL=2021 AND HESAPKOD='63003'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Üretime Yönelik Mal ve Malzeme Alımları', HESAPKOD='6300301' WHERE YIL=2021 AND HESAPKOD='6300301'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Hammadde Alımları', HESAPKOD='630030110' WHERE YIL=2021 AND HESAPKOD='630030101'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Hammadde Alımları', HESAPKOD='63003011001' WHERE YIL=2021 AND HESAPKOD='63003010101'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Gıda Ürünleri İçecekler ve Tütün Alımları', HESAPKOD='630030120' WHERE YIL=2021 AND HESAPKOD='630030102'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Gıda Ürünleri İçecekler ve Tütün Alımları', HESAPKOD='63003012001' WHERE YIL=2021 AND HESAPKOD='63003010201'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Tekstil ve Tekstil Ürünleri Deri ve Deri Ürünleri Alımları', HESAPKOD='630030130' WHERE YIL=2021 AND HESAPKOD='630030103'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Tekstil ve Tekstil Ürünleri Deri ve Deri Ürünleri Alımları', HESAPKOD='63003013001' WHERE YIL=2021 AND HESAPKOD='63003010301'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kereste ve Kereste Ürünleri Alımları', HESAPKOD='630030140' WHERE YIL=2021 AND HESAPKOD='630030104'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kereste ve Kereste Ürünleri Alımları', HESAPKOD='63003014001' WHERE YIL=2021 AND HESAPKOD='63003010401'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kağıt ve Kağıt Ürünleri Alımları', HESAPKOD='630030150' WHERE YIL=2021 AND HESAPKOD='630030105'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kağıt ve Kağıt Ürünleri Alımları', HESAPKOD='63003015001' WHERE YIL=2021 AND HESAPKOD='63003010501'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kimyevi Ürün Alımları', HESAPKOD='630030160' WHERE YIL=2021 AND HESAPKOD='630030106'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kimyevi Ürün Alımları', HESAPKOD='63003016001' WHERE YIL=2021 AND HESAPKOD='63003010601'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kauçuk ve Plastik Ürün Alımları', HESAPKOD='630030170' WHERE YIL=2021 AND HESAPKOD='630030107'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kauçuk ve Plastik Ürün Alımları', HESAPKOD='63003017001' WHERE YIL=2021 AND HESAPKOD='63003010701'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Metal Ürünü Alımları', HESAPKOD='630030180' WHERE YIL=2021 AND HESAPKOD='630030108'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Metal Ürünü Alımları', HESAPKOD='63003018001' WHERE YIL=2021 AND HESAPKOD='63003010801'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Üretim Mal ve Malzemesi Alımları', HESAPKOD='630030190' WHERE YIL=2021 AND HESAPKOD='630030109'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Üretim Mal ve Malzemesi Alımları', HESAPKOD='63003019090' WHERE YIL=2021 AND HESAPKOD='63003010901'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Tüketime Yönelik Mal ve Malzeme Alımları', HESAPKOD='6300302' WHERE YIL=2021 AND HESAPKOD='6300302'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kırtasiye, Yayın, Baskı ve Büro Malzemesi Alımları', HESAPKOD='630030210' WHERE YIL=2021 AND HESAPKOD='630030201'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kırtasiye Alımları', HESAPKOD='63003021001' WHERE YIL=2021 AND HESAPKOD='63003020101'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Büro  Malzemesi Alımları', HESAPKOD='63003021002' WHERE YIL=2021 AND HESAPKOD='63003020102'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Periyodik Yayın Alımları', HESAPKOD='63003021003' WHERE YIL=2021 AND HESAPKOD='63003020103'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Yayın Alımları', HESAPKOD='63003021004' WHERE YIL=2021 AND HESAPKOD='63003020104'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Baskı ve Cilt Giderleri', HESAPKOD='63003021005' WHERE YIL=2021 AND HESAPKOD='63003020105'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Kırtasiye, Yayın, Baskı ve Büro Malzemesi Alımları', HESAPKOD='63003021090' WHERE YIL=2021 AND HESAPKOD='63003020190'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Su ve Temizlik Malzemesi Alımları', HESAPKOD='630030220' WHERE YIL=2021 AND HESAPKOD='630030202'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Su Alımları', HESAPKOD='63003022001' WHERE YIL=2021 AND HESAPKOD='63003020201'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Temizlik Malzemesi Alımları', HESAPKOD='63003022002' WHERE YIL=2021 AND HESAPKOD='63003020202'
DELETE FROM  MUHHESAPPLANI WHERE YIL=2021 AND HESAPKOD='63003020280'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Enerji Alımları', HESAPKOD='630030230' WHERE YIL=2021 AND HESAPKOD='630030203'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yakacak  Alımları', HESAPKOD='63003023001' WHERE YIL=2021 AND HESAPKOD='63003020301'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Akaryakıt ve Yağ  Alımları', HESAPKOD='63003023002' WHERE YIL=2021 AND HESAPKOD='63003020302'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Elektrik Alımları', HESAPKOD='63003023003' WHERE YIL=2021 AND HESAPKOD='63003020303'
DELETE FROM  MUHHESAPPLANI WHERE YIL=2021 AND HESAPKOD='63003020380'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Enerji Alımları', HESAPKOD='63003023090' WHERE YIL=2021 AND HESAPKOD='63003020390'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yiyecek, İçecek ve Yem Alımları', HESAPKOD='630030240' WHERE YIL=2021 AND HESAPKOD='630030204'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yiyecek Alımları ', HESAPKOD='63003024001' WHERE YIL=2021 AND HESAPKOD='63003020401'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='İçecek Alımları', HESAPKOD='63003024002' WHERE YIL=2021 AND HESAPKOD='63003020402'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yem Alımları', HESAPKOD='63003024003' WHERE YIL=2021 AND HESAPKOD='63003020403'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Yiyecek, İçecek ve Yem Alımları', HESAPKOD='63003024090' WHERE YIL=2021 AND HESAPKOD='63003020490'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Giyim ve Kuşam Alımları', HESAPKOD='630030250' WHERE YIL=2021 AND HESAPKOD='630030205'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Giyecek Alımları  ', HESAPKOD='63003025001' WHERE YIL=2021 AND HESAPKOD='63003020501'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Spor Malzemeleri Alımları', HESAPKOD='63003025002' WHERE YIL=2021 AND HESAPKOD='63003020502'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Tören Malzemeleri Alımları', HESAPKOD='63003025003' WHERE YIL=2021 AND HESAPKOD='63003020503'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Bando Malzemeleri Alımları', HESAPKOD='63003025004' WHERE YIL=2021 AND HESAPKOD='63003020504'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kuşam Alımları  ', HESAPKOD='63003025005' WHERE YIL=2021 AND HESAPKOD='63003020505'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Giyim ve Kuşam Alımları', HESAPKOD='63003025090' WHERE YIL=2021 AND HESAPKOD='63003020590'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Özel Malzeme Alımları', HESAPKOD='630030260' WHERE YIL=2021 AND HESAPKOD='630030206'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Laboratuvar Malzemesi ile Kimyevi ve Temrinlik Malzeme Alımları', HESAPKOD='63003026001' WHERE YIL=2021 AND HESAPKOD='63003020601'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Tıbbi Malzeme ve İlaç Alımları', HESAPKOD='63003026002' WHERE YIL=2021 AND HESAPKOD='63003020602'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Zirai Malzeme ve İlaç Alımları', HESAPKOD='63003026003' WHERE YIL=2021 AND HESAPKOD='63003020603'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Canlı Hayvan Alım, Bakım ve Diğer Giderleri', HESAPKOD='63003026004' WHERE YIL=2021 AND HESAPKOD='63003020604'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Özel Malzeme Alımları', HESAPKOD='63003026090' WHERE YIL=2021 AND HESAPKOD='63003020690'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Güvenlik ve Savunmaya Yönelik Mal, Malzeme ve Hizmet Alımları, Yapımları ve Giderleri', HESAPKOD='630030270' WHERE YIL=2021 AND HESAPKOD='630030207'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Güvenlik ve Savunmaya Yönelik Silah, Araç, Gereç ve Savaş Teçhizatı Alımları', HESAPKOD='63003027001' WHERE YIL=2021 AND HESAPKOD='63003020701'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Güvenlik ve Savunmaya Yönelik Silah, Araç, Gereç ve Savaş Teçhizatı İşletme, Bakım ve İdame Giderleri', HESAPKOD='63003027002' WHERE YIL=2021 AND HESAPKOD='63003020702'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Mühimmat Alımları', HESAPKOD='63003027003' WHERE YIL=2021 AND HESAPKOD='63003020703'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Güvenlik ve Savunmaya Yönelik Araştırma-Geliştirme Giderleri', HESAPKOD='63003027004' WHERE YIL=2021 AND HESAPKOD='63003020704'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Güvenlik ve Savunmaya Yönelik Hizmet Alım Giderleri', HESAPKOD='63003027005' WHERE YIL=2021 AND HESAPKOD='63003020705'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Savunma Projeleri ve Acil İhtiyaç Giderleri', HESAPKOD='63003027006' WHERE YIL=2021 AND HESAPKOD='63003020706'
DELETE FROM  MUHHESAPPLANI WHERE YIL=2021 AND HESAPKOD='63003020708'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Güvenlik ve Savunmaya Yönelik Diğer Giderler', HESAPKOD='63003027009' WHERE YIL=2021 AND HESAPKOD='63003020709'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Güvenlik ve Savunmaya Yönelik Makine-Teçhizat Alımları', HESAPKOD='63003027011' WHERE YIL=2021 AND HESAPKOD='63003020711'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Güvenlik ve Savunmaya Yönelik Makine-Teçhizat Büyük Onarımları', HESAPKOD='63003027012' WHERE YIL=2021 AND HESAPKOD='63003020712'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Güvenlik ve Savunmaya Yönelik Gayrimenkul Yapım Giderleri', HESAPKOD='63003027021' WHERE YIL=2021 AND HESAPKOD='63003020721'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Güvenlik ve Savunmaya Yönelik Gayrimenkul Büyük Onarım Giderleri', HESAPKOD='63003027022' WHERE YIL=2021 AND HESAPKOD='63003020722'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='NATO Altyapısına İlişkin Gayrimenkul Yapım Giderleri', HESAPKOD='63003027031' WHERE YIL=2021 AND HESAPKOD='63003020731'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='NATO Altyapısına İlişkin Gayrimenkul Büyük Onarım Giderleri', HESAPKOD='63003027032' WHERE YIL=2021 AND HESAPKOD='63003020732'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Savunma Mal ve  Malzeme Alımları ve Yapımları', HESAPKOD='63003027090' WHERE YIL=2021 AND HESAPKOD='63003020790'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='NATO Giderleri İle Gayrimenkul Alım ve Kamulaştırma Giderleri', HESAPKOD='630030280' WHERE YIL=2021 AND HESAPKOD='630030208'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Gayrimenkul Alım ve Kamulaştırma Giderleri', HESAPKOD='63003028001' WHERE YIL=2021 AND HESAPKOD='63003020801'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='NATO Altyapısına İlişkin Gayrimenkul Alım ve Kamulaştırma Giderleri', HESAPKOD='63003028002' WHERE YIL=2021 AND HESAPKOD='63003020802'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='NATO Giderleri', HESAPKOD='63003028003' WHERE YIL=2021 AND HESAPKOD='63003020803'
DELETE FROM  MUHHESAPPLANI WHERE YIL=2021 AND HESAPKOD='63003020890'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Tüketim Mal ve Malzemesi Alımları', HESAPKOD='630030290' WHERE YIL=2021 AND HESAPKOD='630030209'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Bahçe Malzemesi Alımları ile Yapım ve Bakım Giderleri', HESAPKOD='63003029001' WHERE YIL=2021 AND HESAPKOD='63003020901'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Tüketim Mal ve Malzemesi Alımları', HESAPKOD='63003029090' WHERE YIL=2021 AND HESAPKOD='63003020990'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yolluklar', HESAPKOD='6300303' WHERE YIL=2021 AND HESAPKOD='6300303'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Geçici Görev Yollukları', HESAPKOD='630030310' WHERE YIL=2021 AND HESAPKOD='630030301'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yurtiçi Geçici Görev Yollukları', HESAPKOD='63003031001' WHERE YIL=2021 AND HESAPKOD='63003030101'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yurtiçi Tedavi Yollukları', HESAPKOD='63003031002' WHERE YIL=2021 AND HESAPKOD='63003030102'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sürekli Görev Yollukları', HESAPKOD='630030320' WHERE YIL=2021 AND HESAPKOD='630030302'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yurtiçi Sürekli Görev Yollukları', HESAPKOD='63003032001' WHERE YIL=2021 AND HESAPKOD='63003030201'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Geçici Görev Yollukları', HESAPKOD='630030310' WHERE YIL=2021 AND HESAPKOD='630030303'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yurtdışı Geçici Görev Yollukları', HESAPKOD='63003031003' WHERE YIL=2021 AND HESAPKOD='63003030301'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yurtdışı Tedavi Yollukları', HESAPKOD='63003031004' WHERE YIL=2021 AND HESAPKOD='63003030302'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sürekli Görev Yollukları', HESAPKOD='630030320' WHERE YIL=2021 AND HESAPKOD='630030304'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yurtdışı Sürekli Görev Yollukları', HESAPKOD='63003032002' WHERE YIL=2021 AND HESAPKOD='63003030401'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yolluk Tazminatları', HESAPKOD='630030330' WHERE YIL=2021 AND HESAPKOD='630030305'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Seyyar Görev Tazminatları', HESAPKOD='63003033001' WHERE YIL=2021 AND HESAPKOD='63003030501'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Arazi Tazminatları', HESAPKOD='63003033002' WHERE YIL=2021 AND HESAPKOD='63003030502'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Yolluklar', HESAPKOD='630030390' WHERE YIL=2021 AND HESAPKOD='630030306'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Profesör Uzman ve Memur Mübadele Giderleri', HESAPKOD='63003039001' WHERE YIL=2021 AND HESAPKOD='63003030601'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Öğrenci Mübadele Giderleri', HESAPKOD='63003039002' WHERE YIL=2021 AND HESAPKOD='63003030602'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Görev Giderleri', HESAPKOD='6300304' WHERE YIL=2021 AND HESAPKOD='6300304'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Tahliye Giderleri', HESAPKOD='630030410' WHERE YIL=2021 AND HESAPKOD='630030401'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yurtdışı Tahliye Giderleri', HESAPKOD='63003041001' WHERE YIL=2021 AND HESAPKOD='63003040101'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Afet Bölgesi Tahliye Giderleri', HESAPKOD='63003041002' WHERE YIL=2021 AND HESAPKOD='63003040102'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Mülteci Tahliye Giderleri', HESAPKOD='63003041003' WHERE YIL=2021 AND HESAPKOD='63003040103'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Tahliye Giderleri', HESAPKOD='63003041090' WHERE YIL=2021 AND HESAPKOD='63003040190'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yasal Giderler', HESAPKOD='630030420' WHERE YIL=2021 AND HESAPKOD='630030402'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Beyiye Aidatları', HESAPKOD='63003046001' WHERE YIL=2021 AND HESAPKOD='63003040201'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Oranı Kanunla Belirlenmiş Aidat ve İkramiyeler ', HESAPKOD='63003042001' WHERE YIL=2021 AND HESAPKOD='63003040202'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kusursuz Tazminatlar', HESAPKOD='63003042002' WHERE YIL=2021 AND HESAPKOD='63003040203'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Mahkeme Harç ve Giderleri', HESAPKOD='63003047001' WHERE YIL=2021 AND HESAPKOD='63003040204'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Ödül, İkramiye ve Benzeri Ödemeler ', HESAPKOD='63003042003' WHERE YIL=2021 AND HESAPKOD='63003040205'
DELETE FROM  MUHHESAPPLANI WHERE YIL=2021 AND HESAPKOD='63003040206'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Yasal Giderler', HESAPKOD='63003042090' WHERE YIL=2021 AND HESAPKOD='63003040290'
DELETE FROM  MUHHESAPPLANI WHERE YIL=2021 AND HESAPKOD='63003040291'
DELETE FROM  MUHHESAPPLANI WHERE YIL=2021 AND HESAPKOD='63003040292'
DELETE FROM  MUHHESAPPLANI WHERE YIL=2021 AND HESAPKOD='63003040293'
DELETE FROM  MUHHESAPPLANI WHERE YIL=2021 AND HESAPKOD='63003040294'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Ödenecek Vergi, Resim,  Harçlar ve Benzeri Giderler', HESAPKOD='630030430' WHERE YIL=2021 AND HESAPKOD='630030403'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Vergi Ödemeleri ve Benzeri Giderler', HESAPKOD='63003043001' WHERE YIL=2021 AND HESAPKOD='63003040301'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='İşletme Ruhsatı Ödemeleri ve Benzeri Giderler', HESAPKOD='63003043002' WHERE YIL=2021 AND HESAPKOD='63003040302'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Vergi, Resim ve Harçlar ve Benzeri Giderler', HESAPKOD='63003043090' WHERE YIL=2021 AND HESAPKOD='63003040390'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kültür Varlıkları Alımı ve Korunması Giderleri', HESAPKOD='630030440' WHERE YIL=2021 AND HESAPKOD='630030404'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Arkeolojik Kazı Giderleri', HESAPKOD='63003044001' WHERE YIL=2021 AND HESAPKOD='63003040401'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Restorasyon ve Yenileme Giderleri', HESAPKOD='63003044002' WHERE YIL=2021 AND HESAPKOD='63003040402'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kültür Varlıkları Alımı', HESAPKOD='63003044003' WHERE YIL=2021 AND HESAPKOD='63003040403'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sergi Giderleri', HESAPKOD='63003044004' WHERE YIL=2021 AND HESAPKOD='63003040404'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kültür Varlıklarının Korunmasına İlişkin Diğer Giderler', HESAPKOD='63003044090' WHERE YIL=2021 AND HESAPKOD='63003040490'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Gizli Hizmet Giderleri', HESAPKOD='630030450' WHERE YIL=2021 AND HESAPKOD='630030405'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Gizli Hizmet Giderleri', HESAPKOD='63003045001' WHERE YIL=2021 AND HESAPKOD='63003040501'
DELETE FROM  MUHHESAPPLANI WHERE YIL=2021 AND HESAPKOD='63003040502'
DELETE FROM  MUHHESAPPLANI WHERE YIL=2021 AND HESAPKOD='630030408'
DELETE FROM  MUHHESAPPLANI WHERE YIL=2021 AND HESAPKOD='63003040890'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Görev Giderleri', HESAPKOD='630030490' WHERE YIL=2021 AND HESAPKOD='630030409'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Karantina Giderleri', HESAPKOD='63003049001' WHERE YIL=2021 AND HESAPKOD='63003040901'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Güvenlik Kuvvetleri Nezaretinde Bulundurma Giderleri', HESAPKOD='63003049002' WHERE YIL=2021 AND HESAPKOD='63003040902'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yurtdışına Çıkarma ve Yurtiçinde Yer Değiştirme Giderleri', HESAPKOD='63003049003' WHERE YIL=2021 AND HESAPKOD='63003040903'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Mültecilerin Güvenlik Kuvvetleri Nezaretinde Bulundurulma Giderleri', HESAPKOD='63003049004' WHERE YIL=2021 AND HESAPKOD='63003040904'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Barışı Destekleme ve Koruma Harekat Giderleri', HESAPKOD='63003049005' WHERE YIL=2021 AND HESAPKOD='63003040905'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='375 Sayılı Kanun Hükmünde Kararname Kapsamında Yüklenicilere Yapılacak Tazminat Ödemeleri', HESAPKOD='63003049006' WHERE YIL=2021 AND HESAPKOD='63003040906'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sınıflandırmaya Girmeyen Diğer Görev Giderleri', HESAPKOD='63003049090' WHERE YIL=2021 AND HESAPKOD='63003040990'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Hizmet Alımları', HESAPKOD='6300305' WHERE YIL=2021 AND HESAPKOD='6300305'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Genel Hizmet Alımları ', HESAPKOD='630030510' WHERE YIL=2021 AND HESAPKOD='630030501'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Etüt-Proje Bilirkişi Ekspertiz Giderleri', HESAPKOD='63003051001' WHERE YIL=2021 AND HESAPKOD='63003050101'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Araştırma ve Geliştirme Hizmet Alım Giderleri', HESAPKOD='63003051002' WHERE YIL=2021 AND HESAPKOD='63003050102'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Bilgisayar Hizmet Alım Giderleri', HESAPKOD='63003051003' WHERE YIL=2021 AND HESAPKOD='63003050103'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Genel Hizmet Alımları', HESAPKOD='63003051090' WHERE YIL=2021 AND HESAPKOD='63003050104'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Harita Yapım ve Alım Giderleri', HESAPKOD='63003051004' WHERE YIL=2021 AND HESAPKOD='63003050105'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Enformasyon ve Raporlama Hizmet Alım Giderleri', HESAPKOD='63003051005' WHERE YIL=2021 AND HESAPKOD='63003050106'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Danışma Yönetim ve İşletim Hizmet Alım Giderleri', HESAPKOD='63003051006' WHERE YIL=2021 AND HESAPKOD='63003050107'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Genel Hizmet Alımları', HESAPKOD='63003051090' WHERE YIL=2021 AND HESAPKOD='63003050108'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Genel Hizmet Alımları', HESAPKOD='63003051090' WHERE YIL=2021 AND HESAPKOD='63003050109'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='İş Sağlığı ve Güvenliği Hizmeti Alım Giderleri', HESAPKOD='63003051007' WHERE YIL=2021 AND HESAPKOD='63003050110'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Genel Hizmet Alımları', HESAPKOD='63003051090' WHERE YIL=2021 AND HESAPKOD='63003050111'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Çağrı Merkezi Hizmeti Alım Giderleri', HESAPKOD='63003051008' WHERE YIL=2021 AND HESAPKOD='63003050112'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Genel Hizmet Alımları', HESAPKOD='63003051090' WHERE YIL=2021 AND HESAPKOD='63003050190'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Haberleşme Giderleri', HESAPKOD='630030520' WHERE YIL=2021 AND HESAPKOD='630030502'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Posta ve Telgraf Giderleri', HESAPKOD='63003052001' WHERE YIL=2021 AND HESAPKOD='63003050201'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Telefon Abonelik ve Kullanım Ücretleri', HESAPKOD='63003052002' WHERE YIL=2021 AND HESAPKOD='63003050202'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='İnternet Erişimi Giderleri', HESAPKOD='63003052003' WHERE YIL=2021 AND HESAPKOD='63003050203'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Haberleşme Cihazları Ruhsat ve Kullanım Giderleri', HESAPKOD='63003052004' WHERE YIL=2021 AND HESAPKOD='63003050204'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Uydu Haberleşme Giderleri', HESAPKOD='63003052005' WHERE YIL=2021 AND HESAPKOD='63003050205'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Hat Kira Giderleri', HESAPKOD='63003052006' WHERE YIL=2021 AND HESAPKOD='63003050206'
DELETE FROM  MUHHESAPPLANI WHERE YIL=2021 AND HESAPKOD='63003050280'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Haberleşme Giderleri', HESAPKOD='63003052090' WHERE YIL=2021 AND HESAPKOD='63003050290'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Taşıma Giderleri', HESAPKOD='630030530' WHERE YIL=2021 AND HESAPKOD='630030503'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Taşımaya İlişkin Beslenme, Barındırma Giderleri', HESAPKOD='63003053001' WHERE YIL=2021 AND HESAPKOD='63003050301'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yolcu Taşıma Giderleri', HESAPKOD='63003053002' WHERE YIL=2021 AND HESAPKOD='63003050302'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yük Taşıma Giderleri', HESAPKOD='63003053003' WHERE YIL=2021 AND HESAPKOD='63003050303'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Geçiş Ücretleri', HESAPKOD='63003053004' WHERE YIL=2021 AND HESAPKOD='63003050304'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Taşıma Giderleri', HESAPKOD='63003053090' WHERE YIL=2021 AND HESAPKOD='63003050390'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Tarifeye Bağlı Ödemeler', HESAPKOD='630030540' WHERE YIL=2021 AND HESAPKOD='630030504'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='İlan Giderleri', HESAPKOD='63003054001' WHERE YIL=2021 AND HESAPKOD='63003050401'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sigorta Giderleri', HESAPKOD='63003054002' WHERE YIL=2021 AND HESAPKOD='63003050402'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Komisyon Giderleri', HESAPKOD='63003054003' WHERE YIL=2021 AND HESAPKOD='63003050403'
DELETE FROM  MUHHESAPPLANI WHERE YIL=2021 AND HESAPKOD='63003050404'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Tarifeye Bağlı Ödemeler', HESAPKOD='63003054090' WHERE YIL=2021 AND HESAPKOD='63003050490'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kiralar', HESAPKOD='630030550' WHERE YIL=2021 AND HESAPKOD='630030505'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Dayanıklı Mal ve Malzeme Kiralama Giderleri', HESAPKOD='63003055001' WHERE YIL=2021 AND HESAPKOD='63003050501'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Taşıt Kiralama Giderleri', HESAPKOD='63003055002' WHERE YIL=2021 AND HESAPKOD='63003050502'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='İş Makinası Kiralama Giderleri', HESAPKOD='63003055003' WHERE YIL=2021 AND HESAPKOD='63003050503'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Canlı Hayvan Kiralama Giderleri', HESAPKOD='63003055004' WHERE YIL=2021 AND HESAPKOD='63003050504'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Hizmet Binası Kiralama Giderleri', HESAPKOD='63003055005' WHERE YIL=2021 AND HESAPKOD='63003050505'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Lojman Kiralama Giderleri', HESAPKOD='63003055006' WHERE YIL=2021 AND HESAPKOD='63003050506'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Arsa ve Arazi Kiralama Giderleri', HESAPKOD='63003055007' WHERE YIL=2021 AND HESAPKOD='63003050507'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yüzer Taşıt Kiralama Giderleri', HESAPKOD='63003055008' WHERE YIL=2021 AND HESAPKOD='63003050508'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Hava Taşıtı Kiralama Giderleri', HESAPKOD='63003055009' WHERE YIL=2021 AND HESAPKOD='63003050509'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Bilgisayar, Bilgisayar Sistemleri ve Yazılımları Kiralama Giderleri', HESAPKOD='63003055010' WHERE YIL=2021 AND HESAPKOD='63003050510'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Tersane Kiralama Giderleri', HESAPKOD='63003055011' WHERE YIL=2021 AND HESAPKOD='63003050511'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Personel Servisi Kiralama Giderleri', HESAPKOD='63003055012' WHERE YIL=2021 AND HESAPKOD='63003050512'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Kiralama Giderleri', HESAPKOD='63003055090' WHERE YIL=2021 AND HESAPKOD='63003050590'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Devlet Borçları Genel Giderleri', HESAPKOD='630030560' WHERE YIL=2021 AND HESAPKOD='630030506'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='İç Borçlanma Hazine Bonosu Genel Giderleri', HESAPKOD='63003056001' WHERE YIL=2021 AND HESAPKOD='63003050601'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='İç Borçlanma Devlet Tahvili Genel  Giderleri', HESAPKOD='63003056002' WHERE YIL=2021 AND HESAPKOD='63003050602'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Dış Borçlanma Genel Giderleri', HESAPKOD='63003056003' WHERE YIL=2021 AND HESAPKOD='63003050603'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Hazine İşlemleri Diğer Genel Giderleri', HESAPKOD='63003056004' WHERE YIL=2021 AND HESAPKOD='63003050604'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yargılama Giderleri', HESAPKOD='630030570' WHERE YIL=2021 AND HESAPKOD='630030507'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Adli Yardım Giderleri', HESAPKOD='63003057001' WHERE YIL=2021 AND HESAPKOD='63003050701'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Keşif Giderleri', HESAPKOD='63003057002' WHERE YIL=2021 AND HESAPKOD='63003050702'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Rapor ve Bilirkişi Giderleri', HESAPKOD='63003057003' WHERE YIL=2021 AND HESAPKOD='63003050703'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Uzlaşma Giderleri', HESAPKOD='63003057004' WHERE YIL=2021 AND HESAPKOD='63003050704'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Arabuluculuk Ücreti Giderleri', HESAPKOD='63003057005' WHERE YIL=2021 AND HESAPKOD='63003050705'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Yargılama Giderleri', HESAPKOD='63003057090' WHERE YIL=2021 AND HESAPKOD='63003050790'
DELETE FROM  MUHHESAPPLANI WHERE YIL=2021 AND HESAPKOD='630030508'
DELETE FROM  MUHHESAPPLANI WHERE YIL=2021 AND HESAPKOD='63003050801'
DELETE FROM  MUHHESAPPLANI WHERE YIL=2021 AND HESAPKOD='63003050802'
DELETE FROM  MUHHESAPPLANI WHERE YIL=2021 AND HESAPKOD='63003050803'
DELETE FROM  MUHHESAPPLANI WHERE YIL=2021 AND HESAPKOD='63003050804'
DELETE FROM  MUHHESAPPLANI WHERE YIL=2021 AND HESAPKOD='63003050810'
DELETE FROM  MUHHESAPPLANI WHERE YIL=2021 AND HESAPKOD='63003050811'
DELETE FROM  MUHHESAPPLANI WHERE YIL=2021 AND HESAPKOD='63003050890'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Hizmet Alımları', HESAPKOD='630030590' WHERE YIL=2021 AND HESAPKOD='630030509'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yurtiçi Staj ve Öğrenim Giderleri', HESAPKOD='63003059001' WHERE YIL=2021 AND HESAPKOD='63003050901'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yurtdışı Staj ve Öğrenim Giderleri', HESAPKOD='63003059002' WHERE YIL=2021 AND HESAPKOD='63003050902'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kurslara Katılma ve Eğitim Giderleri', HESAPKOD='63003059003' WHERE YIL=2021 AND HESAPKOD='63003050903'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Öğretim Üyesi Yetiştirme Projesi Giderleri', HESAPKOD='63003059004' WHERE YIL=2021 AND HESAPKOD='63003050904'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Lojman İşletme Maliyetlerine Katılım Giderleri ', HESAPKOD='63003059005' WHERE YIL=2021 AND HESAPKOD='63003050910'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Binaların İşletme Maliyetlerine Katılım Giderleri ', HESAPKOD='63003059006' WHERE YIL=2021 AND HESAPKOD='63003050911'
DELETE FROM  MUHHESAPPLANI WHERE YIL=2021 AND HESAPKOD='63003050912'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sınıflandırmaya Girmeyen Diğer Hizmet Alımları', HESAPKOD='63003059090' WHERE YIL=2021 AND HESAPKOD='63003050990'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Temsil ve Tanıtma Giderleri', HESAPKOD='6300306' WHERE YIL=2021 AND HESAPKOD='6300306'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Temsil, Tanıtma ve Ağırlama Giderleri', HESAPKOD='630030610' WHERE YIL=2021 AND HESAPKOD='630030601'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Temsil, Tanıtma ve Ağırlama Giderleri', HESAPKOD='63003061001' WHERE YIL=2021 AND HESAPKOD='63003060101'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Toplantı ve Organizasyon Giderleri', HESAPKOD='630030620' WHERE YIL=2021 AND HESAPKOD='630030602'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Toplantı ve Organizasyon Giderleri', HESAPKOD='63003062001' WHERE YIL=2021 AND HESAPKOD='63003060201'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Menkul Mal, Gayrimaddi Hak Alım, Bakım ve Onarım Giderleri', HESAPKOD='6300307' WHERE YIL=2021 AND HESAPKOD='6300307'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Menkul Mal  Alım Giderleri', HESAPKOD='630030710' WHERE YIL=2021 AND HESAPKOD='630030701'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Büro ve İşyeri Mal ve Malzeme Alımları', HESAPKOD='63003071001' WHERE YIL=2021 AND HESAPKOD='63003070101'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Büro ve İşyeri Makine ve Techizat Alımları', HESAPKOD='63003071002' WHERE YIL=2021 AND HESAPKOD='63003070102'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Avadanlık ve Yedek Parça Alımları ', HESAPKOD='63003071003' WHERE YIL=2021 AND HESAPKOD='63003070103'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yangından Korunma Malzemeleri Alımları', HESAPKOD='63003071004' WHERE YIL=2021 AND HESAPKOD='63003070104'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Dayanıklı Mal ve Malzeme Alımları', HESAPKOD='63003071090' WHERE YIL=2021 AND HESAPKOD='63003070190'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Gayri Maddi Hak Alımları', HESAPKOD='630030720' WHERE YIL=2021 AND HESAPKOD='630030702'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Bilgisayar Yazılım Alımları ve Yapımları', HESAPKOD='63003072001' WHERE YIL=2021 AND HESAPKOD='63003070201'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Fikri Hak Alımları', HESAPKOD='63003072002' WHERE YIL=2021 AND HESAPKOD='63003070202'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Gayri Maddi Hak Alımları', HESAPKOD='63003072090' WHERE YIL=2021 AND HESAPKOD='63003070290'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Bakım ve Onarım Giderleri', HESAPKOD='630030730' WHERE YIL=2021 AND HESAPKOD='630030703'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Tefrişat Bakım ve Onarım Giderleri', HESAPKOD='63003073001' WHERE YIL=2021 AND HESAPKOD='63003070301'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Makine Teçhizat Bakım ve Onarım Giderleri', HESAPKOD='63003073002' WHERE YIL=2021 AND HESAPKOD='63003070302'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Taşıt Bakım ve Onarım Giderleri', HESAPKOD='63003073003' WHERE YIL=2021 AND HESAPKOD='63003070303'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='İş Makinası Onarım Giderleri', HESAPKOD='63003073004' WHERE YIL=2021 AND HESAPKOD='63003070304'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Bakım ve Onarım Giderleri', HESAPKOD='63003073090' WHERE YIL=2021 AND HESAPKOD='63003070390'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Gayrimenkul Mal Bakım ve Onarım Giderleri', HESAPKOD='6300308' WHERE YIL=2021 AND HESAPKOD='6300308'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Hizmet Binası  Bakım ve Onarım Giderleri', HESAPKOD='630030810' WHERE YIL=2021 AND HESAPKOD='630030801'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Büro Bakım ve Onarımı Giderleri', HESAPKOD='63003081001' WHERE YIL=2021 AND HESAPKOD='63003080101'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Eğitim ve Öğretim Tesisi Bakım ve Onarımı Giderleri', HESAPKOD='63003081002' WHERE YIL=2021 AND HESAPKOD='63003080102'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Hastane Bakım ve Onarımı Giderleri', HESAPKOD='63003081003' WHERE YIL=2021 AND HESAPKOD='63003080103'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Atölye ve Tesis Binaları Bakım ve Onarımı Giderleri', HESAPKOD='63003081004' WHERE YIL=2021 AND HESAPKOD='63003080104'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Hizmet Binası  Bakım ve Onarım Giderleri', HESAPKOD='63003081090' WHERE YIL=2021 AND HESAPKOD='63003080190'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Lojman  Bakım ve Onarımı Giderleri', HESAPKOD='630030820' WHERE YIL=2021 AND HESAPKOD='630030802'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Lojman Bakım ve Onarımı Giderleri', HESAPKOD='63003082001' WHERE YIL=2021 AND HESAPKOD='63003080201'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sosyal Tesis  Bakım ve Onarımı Giderleri', HESAPKOD='630030830' WHERE YIL=2021 AND HESAPKOD='630030803'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sosyal Tesis  Bakım ve Onarımı Giderleri', HESAPKOD='63003083001' WHERE YIL=2021 AND HESAPKOD='63003080301'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Gemi  Bakım ve Onarımı Giderleri', HESAPKOD='630030840' WHERE YIL=2021 AND HESAPKOD='630030804'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Gemi  Bakım ve Onarımı Giderleri', HESAPKOD='63003084001' WHERE YIL=2021 AND HESAPKOD='63003080401'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Tersane Bakım ve Onarımı Giderleri', HESAPKOD='630030850' WHERE YIL=2021 AND HESAPKOD='630030805'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Tersane Bakım ve Onarımı Giderleri', HESAPKOD='63003085001' WHERE YIL=2021 AND HESAPKOD='63003080501'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yüzer Tersane Bakım ve Onarım Giderleri', HESAPKOD='63003085002' WHERE YIL=2021 AND HESAPKOD='63003080502'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yol  Bakım ve Onarımı Giderleri', HESAPKOD='630030860' WHERE YIL=2021 AND HESAPKOD='630030806'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Bitüm Alımları', HESAPKOD='63003086001' WHERE YIL=2021 AND HESAPKOD='63003080601'
DELETE FROM  MUHHESAPPLANI WHERE YIL=2021 AND HESAPKOD='630030808'
DELETE FROM  MUHHESAPPLANI WHERE YIL=2021 AND HESAPKOD='63003080801'
DELETE FROM  MUHHESAPPLANI WHERE YIL=2021 AND HESAPKOD='63003080890'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Taşınmaz Yapım, Bakım ve Onarım Giderleri', HESAPKOD='630030890' WHERE YIL=2021 AND HESAPKOD='630030809'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Taşınmaz Yapım, Bakım ve Onarım Giderleri', HESAPKOD='63003089001' WHERE YIL=2021 AND HESAPKOD='63003080901'
DELETE FROM  MUHHESAPPLANI WHERE YIL=2021 AND HESAPKOD='63003080990'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Tedavi ve Cenaze Giderleri', HESAPKOD='6300309' WHERE YIL=2021 AND HESAPKOD='6300309'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kamu Personeli Tedavi ve Sağlık Malzemesi Giderleri', HESAPKOD='630030910' WHERE YIL=2021 AND HESAPKOD='630030901'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kamu Personeli Tedavi ve Sağlık Malzemesi Giderleri', HESAPKOD='63003091001' WHERE YIL=2021 AND HESAPKOD='63003090101'
DELETE FROM  MUHHESAPPLANI WHERE YIL=2021 AND HESAPKOD='63003090107'
DELETE FROM  MUHHESAPPLANI WHERE YIL=2021 AND HESAPKOD='63003090116'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kamu Personeli İlaç Giderleri', HESAPKOD='630030920' WHERE YIL=2021 AND HESAPKOD='630030902'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kamu Personeli İlaç Giderleri', HESAPKOD='63003092001' WHERE YIL=2021 AND HESAPKOD='63003090201'
DELETE FROM  MUHHESAPPLANI WHERE YIL=2021 AND HESAPKOD='63003090203'
DELETE FROM  MUHHESAPPLANI WHERE YIL=2021 AND HESAPKOD='63003090213'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Cenaze Giderleri', HESAPKOD='630030930' WHERE YIL=2021 AND HESAPKOD='630030903'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Cenaze Giderleri', HESAPKOD='63003093001' WHERE YIL=2021 AND HESAPKOD='63003090301'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Mezar ve Şehitlik Yapım ve Bakım Giderleri', HESAPKOD='63003093002' WHERE YIL=2021 AND HESAPKOD='63003090302'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Milletvekili Tedavi ve Sağlık Malzemesi Giderleri', HESAPKOD='630030940' WHERE YIL=2021 AND HESAPKOD='630030906'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Milletvekili Tedavi ve Sağlık Malzemesi Giderleri', HESAPKOD='63003094001' WHERE YIL=2021 AND HESAPKOD='63003090601'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Milletvekili İlaç Giderleri', HESAPKOD='630030950' WHERE YIL=2021 AND HESAPKOD='630030907'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Milletvekili İlaç Giderleri', HESAPKOD='63003095001' WHERE YIL=2021 AND HESAPKOD='63003090701'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Tedavi ve Sağlık Malzemesi Giderleri', HESAPKOD='630030980' WHERE YIL=2021 AND HESAPKOD='630030908'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Öğrenci  Tedavi ve Sağlık Malzemesi Giderleri', HESAPKOD='63003098001' WHERE YIL=2021 AND HESAPKOD='63003090801'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Er ve Erbaş Tedavi ve Sağlık Malzemesi Giderleri', HESAPKOD='63003098002' WHERE YIL=2021 AND HESAPKOD='63003090802'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Tutuklu ve Hükümlülerin Tedavi ve Sağlık Malzemesi Giderleri', HESAPKOD='63003098003' WHERE YIL=2021 AND HESAPKOD='63003090803'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sosyal Güvenliği Bulunmayanların Tedavi ve Sağlık Malzemesi Giderleri', HESAPKOD='63003098004' WHERE YIL=2021 AND HESAPKOD='63003090804'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sınıflandırılmayan Diğer Tedavi ve Sağlık Malzemesi Giderleri', HESAPKOD='63003098090' WHERE YIL=2021 AND HESAPKOD='63003090890'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer İlaç Giderleri', HESAPKOD='630030990' WHERE YIL=2021 AND HESAPKOD='630030909'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Öğrenci  İlaç Giderleri', HESAPKOD='63003099001' WHERE YIL=2021 AND HESAPKOD='63003090901'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Er ve Erbaş İlaç Giderleri', HESAPKOD='63003099002' WHERE YIL=2021 AND HESAPKOD='63003090902'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Tutuklu ve Hükümlülerin İlaç Giderleri', HESAPKOD='63003099003' WHERE YIL=2021 AND HESAPKOD='63003090903'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sosyal Güvenliği Bulunmayanların İlaç Giderleri', HESAPKOD='63003099004' WHERE YIL=2021 AND HESAPKOD='63003090904'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sınıflandırılmayan Diğer İlaç Giderleri', HESAPKOD='63003099090' WHERE YIL=2021 AND HESAPKOD='63003090990'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Faiz Giderleri', HESAPKOD='63004' WHERE YIL=2021 AND HESAPKOD='63004'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kamu Kurumlarına Ödenen İç Borç Faiz Giderleri', HESAPKOD='6300401' WHERE YIL=2021 AND HESAPKOD='6300401'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Devlet Tahvili', HESAPKOD='630040110' WHERE YIL=2021 AND HESAPKOD='630040101'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='TL Cinsinden Tahvillerin Faiz Giderleri', HESAPKOD='63004011001' WHERE YIL=2021 AND HESAPKOD='63004010101'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Dövize Endeksli ve Döviz Cinsinden Tahvillerin Faiz Giderleri', HESAPKOD='63004011002' WHERE YIL=2021 AND HESAPKOD='63004010102'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Hazine Bonosu', HESAPKOD='630040120' WHERE YIL=2021 AND HESAPKOD='630040102'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='TL Cinsinden Bonoların Faiz Giderleri', HESAPKOD='63004012001' WHERE YIL=2021 AND HESAPKOD='63004010201'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Dövize Endeksli ve Döviz Cinsinden Bonoların Faiz Giderleri', HESAPKOD='63004012002' WHERE YIL=2021 AND HESAPKOD='63004010202'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer İç Borç Faiz Giderleri', HESAPKOD='6300402' WHERE YIL=2021 AND HESAPKOD='6300402'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Devlet Tahvili', HESAPKOD='630040210' WHERE YIL=2021 AND HESAPKOD='630040201'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='TL Cinsinden Tahvillerin Faiz Giderleri', HESAPKOD='63004021001' WHERE YIL=2021 AND HESAPKOD='63004020101'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Dövize Endeksli ve Döviz Cinsinden Tahvillerin Faiz Giderleri', HESAPKOD='63004021002' WHERE YIL=2021 AND HESAPKOD='63004020102'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Hazine Bonosu', HESAPKOD='630040220' WHERE YIL=2021 AND HESAPKOD='630040202'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='TL Cinsinden Bonoların Faiz Giderleri', HESAPKOD='63004022001' WHERE YIL=2021 AND HESAPKOD='63004020201'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Dövize Endeksli ve Döviz Cinsinden Bonoların Faiz Giderleri', HESAPKOD='63004022002' WHERE YIL=2021 AND HESAPKOD='63004020202'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer İç Borç Faiz Giderleri ', HESAPKOD='630040290' WHERE YIL=2021 AND HESAPKOD='630040209'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='TL Cinsinden Diğer İç Borç Faiz Giderleri', HESAPKOD='63004029001' WHERE YIL=2021 AND HESAPKOD='63004020901'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Dövize Endeksli ve Döviz Cinsinden Diğer İç Borç Faiz Giderleri ', HESAPKOD='63004029002' WHERE YIL=2021 AND HESAPKOD='63004020902'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Dış Borç Faiz Giderleri', HESAPKOD='6300403' WHERE YIL=2021 AND HESAPKOD='6300403'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='ABD Doları Cinsinden Dış Borç Faiz Giderleri', HESAPKOD='630040310' WHERE YIL=2021 AND HESAPKOD='630040301'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='ABD Doları Cinsinden Dış Borç Faiz Giderleri', HESAPKOD='63004031001' WHERE YIL=2021 AND HESAPKOD='63004030101'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Avro Cinsinden Borç Faiz Giderleri', HESAPKOD='630040320' WHERE YIL=2021 AND HESAPKOD='630040302'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Avro Cinsinden Dış Borç Faiz Giderleri', HESAPKOD='63004032001' WHERE YIL=2021 AND HESAPKOD='63004030201'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Japon Yeni Cinsinden Dış Borç Faiz Giderleri', HESAPKOD='630040330' WHERE YIL=2021 AND HESAPKOD='630040303'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Japon Yeni Cinsinden Dış Borç Faiz Giderleri', HESAPKOD='63004033001' WHERE YIL=2021 AND HESAPKOD='63004030301'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Dış Borç Faiz Giderleri', HESAPKOD='630040390' WHERE YIL=2021 AND HESAPKOD='630040309'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Dış Borç Faiz Giderleri', HESAPKOD='63004039001' WHERE YIL=2021 AND HESAPKOD='63004030901'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='İskonto Giderleri', HESAPKOD='6300404' WHERE YIL=2021 AND HESAPKOD='6300404'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='İç Borç İskonto Giderleri', HESAPKOD='630040410' WHERE YIL=2021 AND HESAPKOD='630040401'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Devlet Tahvili İskonto Giderleri', HESAPKOD='63004041001' WHERE YIL=2021 AND HESAPKOD='63004040101'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Hazine Bonosu İskonto Giderleri', HESAPKOD='63004041002' WHERE YIL=2021 AND HESAPKOD='63004040102'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Dış Borç İskonto Giderleri', HESAPKOD='630040420' WHERE YIL=2021 AND HESAPKOD='630040402'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Dış Borç İskonto Giderleri', HESAPKOD='63004042001' WHERE YIL=2021 AND HESAPKOD='63004040201'
DELETE FROM  MUHHESAPPLANI WHERE YIL=2021 AND HESAPKOD='63004040301'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Para Piyasası Nakit İşlemleri Faiz Giderleri', HESAPKOD='6300405' WHERE YIL=2021 AND HESAPKOD='6300405'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Para Piyasası Nakit İşlemleri Faiz Giderleri ', HESAPKOD='630040510' WHERE YIL=2021 AND HESAPKOD='630040501'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='TL Cinsinden Para Piyasası Nakit İşlemleri Faiz Giderleri ', HESAPKOD='63004051001' WHERE YIL=2021 AND HESAPKOD='63004050101'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Döviz Cinsinden Kısa Vadeli Nakit İşlemlere Ait Faiz Giderleri', HESAPKOD='63004051002' WHERE YIL=2021 AND HESAPKOD='63004050102'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Türev Ürün Giderleri', HESAPKOD='6300406' WHERE YIL=2021 AND HESAPKOD='6300406'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='İç Borç Türev Ürün Giderleri', HESAPKOD='630040610' WHERE YIL=2021 AND HESAPKOD='630040601'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='İç Borç Türev Ürün Giderleri', HESAPKOD='63004061001' WHERE YIL=2021 AND HESAPKOD='63004060101'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Dış Borç Türev Ürün Giderleri', HESAPKOD='630040620' WHERE YIL=2021 AND HESAPKOD='630040602'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Dış Borç Türev Ürün Giderleri', HESAPKOD='63004062001' WHERE YIL=2021 AND HESAPKOD='63004060201'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kira Sertifikası Giderleri', HESAPKOD='6300407' WHERE YIL=2021 AND HESAPKOD='6300407'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Türk Lirası Cinsinden Kira Sertifikası Giderleri', HESAPKOD='630040710' WHERE YIL=2021 AND HESAPKOD='630040701'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Türk Lirası Cinsinden Kira Sertifikası Giderleri', HESAPKOD='63004071001' WHERE YIL=2021 AND HESAPKOD='63004070101'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Döviz Cinsinden Kira Sertifikası Giderleri', HESAPKOD='630040720' WHERE YIL=2021 AND HESAPKOD='630040702'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Döviz Cinsinden Kira Sertifikası Giderleri', HESAPKOD='63004072001' WHERE YIL=2021 AND HESAPKOD='63004070201'
DELETE FROM  MUHHESAPPLANI WHERE YIL=2021 AND HESAPKOD='6300420'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Cari Transferler', HESAPKOD='63005' WHERE YIL=2021 AND HESAPKOD='63005'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Görevlendirme Giderleri', HESAPKOD='6300501' WHERE YIL=2021 AND HESAPKOD='6300501'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kamu Teşebbüslerine', HESAPKOD='630050110' WHERE YIL=2021 AND HESAPKOD='630050101'
DELETE FROM  MUHHESAPPLANI WHERE YIL=2021 AND HESAPKOD='63005010101'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='TC Devlet Demiryolları na', HESAPKOD='63005011001' WHERE YIL=2021 AND HESAPKOD='63005010102'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Tarım İşletmelerine', HESAPKOD='63005011002' WHERE YIL=2021 AND HESAPKOD='63005010103'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Toprak Mahsulleri Ofisi ne', HESAPKOD='63005011003' WHERE YIL=2021 AND HESAPKOD='63005010104'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Teşebbüslere', HESAPKOD='63005011090' WHERE YIL=2021 AND HESAPKOD='63005010105'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Çay İşletmeleri Genel Müdürlüğü ne', HESAPKOD='63005011004' WHERE YIL=2021 AND HESAPKOD='63005010106'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Türkiye Elektrik Dağıtım AŞ ye', HESAPKOD='63005011005' WHERE YIL=2021 AND HESAPKOD='63005010107'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Elektrik Üretim AŞ ye', HESAPKOD='63005011006' WHERE YIL=2021 AND HESAPKOD='63005010108'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Elektrik İletim AŞ ye', HESAPKOD='63005011007' WHERE YIL=2021 AND HESAPKOD='63005010109'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Türkiye Elektrik ve Taahhüt AŞ ye', HESAPKOD='63005011008' WHERE YIL=2021 AND HESAPKOD='63005010110'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Türkiye Taşkömürü Kurumu na', HESAPKOD='63005011009' WHERE YIL=2021 AND HESAPKOD='63005010111'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Üretici Birliklerine', HESAPKOD='63005011010' WHERE YIL=2021 AND HESAPKOD='63005010112'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Tarım Kredi Kooperatiflerine', HESAPKOD='63005011011' WHERE YIL=2021 AND HESAPKOD='63005010113'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Türkiye Şeker Fabrikaları AŞ ye', HESAPKOD='63005011012' WHERE YIL=2021 AND HESAPKOD='63005010114'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sümer Holding AŞ ye', HESAPKOD='63005011013' WHERE YIL=2021 AND HESAPKOD='63005010115'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Makina ve Kimya Endüstrisi Kurumu na', HESAPKOD='63005011014' WHERE YIL=2021 AND HESAPKOD='63005010116'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Teşebbüslere', HESAPKOD='63005011090' WHERE YIL=2021 AND HESAPKOD='63005010117'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Türkiye Kömür İşletmeleri Kurumu na', HESAPKOD='63005011015' WHERE YIL=2021 AND HESAPKOD='63005010118'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Et ve Süt Kurumu Genel Müdürlüğü ne', HESAPKOD='63005011016' WHERE YIL=2021 AND HESAPKOD='63005010119'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Türkiye Elektromekanik Sanayii Genel Müdürlüğü ne', HESAPKOD='63005011017' WHERE YIL=2021 AND HESAPKOD='63005010120'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Devlet Hava Meydanları İşletmesi Genel Müdürlüğü ne', HESAPKOD='63005011018' WHERE YIL=2021 AND HESAPKOD='63005010121'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Türkiye Petrolleri Anonim Ortaklığı’na', HESAPKOD='63005011019' WHERE YIL=2021 AND HESAPKOD='63005010123'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Teşebbüslere', HESAPKOD='63005011090' WHERE YIL=2021 AND HESAPKOD='63005010190'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sosyal Güvenlik Kurumlarına', HESAPKOD='630050120' WHERE YIL=2021 AND HESAPKOD='630050102'
DELETE FROM  MUHHESAPPLANI WHERE YIL=2021 AND HESAPKOD='63005010201'
DELETE FROM  MUHHESAPPLANI WHERE YIL=2021 AND HESAPKOD='63005010202'
DELETE FROM  MUHHESAPPLANI WHERE YIL=2021 AND HESAPKOD='63005010203'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Türkiye İş Kurumu Genel Müdürlüğü ne', HESAPKOD='63005012001' WHERE YIL=2021 AND HESAPKOD='63005010204'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='5510 Sayılı Kanunun 4/c Maddesi Kapsamındaki Sigortalılara Vazife Malüllük Aylıkları İle Bunların Hak Sahiplerine Bağlanacak Ölüm Aylıkları Karşılığı  Ödenecek Ek Karşılıklar', HESAPKOD='63005012002' WHERE YIL=2021 AND HESAPKOD='63005010205'
DELETE FROM  MUHHESAPPLANI WHERE YIL=2021 AND HESAPKOD='63005010290'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Mali Kurumlara', HESAPKOD='630050130' WHERE YIL=2021 AND HESAPKOD='630050103'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Ziraat Bankasına', HESAPKOD='63005013001' WHERE YIL=2021 AND HESAPKOD='63005010301'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Türkiye Kalkınma ve Yatırım Bankasına', HESAPKOD='63005013002' WHERE YIL=2021 AND HESAPKOD='63005010302'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Türk Eximbanka ', HESAPKOD='63005013003' WHERE YIL=2021 AND HESAPKOD='63005010303'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Halk Bankasına', HESAPKOD='63005013004' WHERE YIL=2021 AND HESAPKOD='63005010304'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Mali Kurumlara', HESAPKOD='63005013090' WHERE YIL=2021 AND HESAPKOD='63005010390'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Döner Sermaye İşletmelerine', HESAPKOD='630050140' WHERE YIL=2021 AND HESAPKOD='630050104'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sağlık Bakanlığı Döner Sermaye İşletmelerine', HESAPKOD='63005014001' WHERE YIL=2021 AND HESAPKOD='63005010401'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Ceza ve Tevkifevleri Döner Sermaye İşletmelerine', HESAPKOD='63005014002' WHERE YIL=2021 AND HESAPKOD='63005010402'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Milli Eğitim Bakanlığı  Döner Sermaye İşletmelerine', HESAPKOD='63005014003' WHERE YIL=2021 AND HESAPKOD='63005010403'
DELETE FROM  MUHHESAPPLANI WHERE YIL=2021 AND HESAPKOD='63005010406'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer  Döner Sermaye İşletmelerine', HESAPKOD='63005014090' WHERE YIL=2021 AND HESAPKOD='63005010490'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Fonlara', HESAPKOD='630050150' WHERE YIL=2021 AND HESAPKOD='630050105'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Fonlara', HESAPKOD='63005015090' WHERE YIL=2021 AND HESAPKOD='63005010501'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sosyal Yardımlaşma ve Dayanışmayı Teşvik Fonuna', HESAPKOD='63005015001' WHERE YIL=2021 AND HESAPKOD='63005010502'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='İşsizlik Sigortası Fonuna', HESAPKOD='63005015002' WHERE YIL=2021 AND HESAPKOD='63005010503'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Fonlara', HESAPKOD='63005015090' WHERE YIL=2021 AND HESAPKOD='63005010590'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Teşekküllere', HESAPKOD='630050190' WHERE YIL=2021 AND HESAPKOD='630050109'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Teşekküllere', HESAPKOD='63005019090' WHERE YIL=2021 AND HESAPKOD='63005010901'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Teşekküllere', HESAPKOD='63005019090' WHERE YIL=2021 AND HESAPKOD='63005010902'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Hazine Yardımları', HESAPKOD='6300502' WHERE YIL=2021 AND HESAPKOD='6300502'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Genel Bütçeye Hazine Yardımı', HESAPKOD='630050210' WHERE YIL=2021 AND HESAPKOD='630050201'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Genel Bütçeye Hazine Yardımı', HESAPKOD='63005021001' WHERE YIL=2021 AND HESAPKOD='63005020101'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Özel Bütçeli İdarelere Hazine Yardımları', HESAPKOD='630050220' WHERE YIL=2021 AND HESAPKOD='630050202'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Ölçme, Seçme ve Yerleştirme Merkezi Başkanlığı na Hazine Yardımı', HESAPKOD='63005022001' WHERE YIL=2021 AND HESAPKOD='63005020201'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Atatürk Kültür, Dil ve Tarih Yüksek Kurumu na Hazine Yardımı', HESAPKOD='63005022002' WHERE YIL=2021 AND HESAPKOD='63005020202'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Atatürk Araştırma Merkezi ne Hazine Yardımı', HESAPKOD='63005022003' WHERE YIL=2021 AND HESAPKOD='63005020203'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Atatürk Kültür Merkezi ne Hazine Yardımı', HESAPKOD='63005022004' WHERE YIL=2021 AND HESAPKOD='63005020204'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Türk Dil Kurumu na Hazine Yardımı', HESAPKOD='63005022005' WHERE YIL=2021 AND HESAPKOD='63005020205'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Türk Tarih Kurumu na Hazine Yardımı', HESAPKOD='63005022006' WHERE YIL=2021 AND HESAPKOD='63005020206'
DELETE FROM  MUHHESAPPLANI WHERE YIL=2021 AND HESAPKOD='63005020207'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Türkiye Bilimsel ve Teknolojik Araştırma Kurumu Başkanlığı na Hazine Yardımı', HESAPKOD='63005022007' WHERE YIL=2021 AND HESAPKOD='63005020208'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Türkiye Bilimler Akademisi Başkanlığı na Hazine Yardımı', HESAPKOD='63005022008' WHERE YIL=2021 AND HESAPKOD='63005020209'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Türkiye Adalet Akademisi’ne Hazine Yardımı', HESAPKOD='63005022009' WHERE YIL=2021 AND HESAPKOD='63005020210'
DELETE FROM  MUHHESAPPLANI WHERE YIL=2021 AND HESAPKOD='63005020213'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sınıflandırmaya Girmeyen Diğer Hazine Yardımları', HESAPKOD='63005029090' WHERE YIL=2021 AND HESAPKOD='63005020214'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Devlet Tiyatroları Genel Müdürlüğü ne Hazine Yardımı', HESAPKOD='63005022010' WHERE YIL=2021 AND HESAPKOD='63005020215'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Devlet Opera ve Balesi Genel Müdürlüğü ne Hazine Yardımı', HESAPKOD='63005022011' WHERE YIL=2021 AND HESAPKOD='63005020216'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Orman Genel Müdürlüğü ne Hazine Yardımı', HESAPKOD='63005022012' WHERE YIL=2021 AND HESAPKOD='63005020217'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Vakıflar Genel Müdürlüğü ne Hazine Yardımı', HESAPKOD='63005022013' WHERE YIL=2021 AND HESAPKOD='63005020218'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Türkiye Hudut ve Sahiller Sağlık Genel Müdürlüğü ne Hazine Yardımı', HESAPKOD='63005022014' WHERE YIL=2021 AND HESAPKOD='63005020219'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Türk Akreditasyon Kurumu na Hazine Yardımı', HESAPKOD='63005022015' WHERE YIL=2021 AND HESAPKOD='63005020221'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Türk Standartları Enstitüsü ne Hazine Yardımı', HESAPKOD='63005022016' WHERE YIL=2021 AND HESAPKOD='63005020222'
DELETE FROM  MUHHESAPPLANI WHERE YIL=2021 AND HESAPKOD='63005020223'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Türk Patent Enstitüsü ne Hazine Yardımı', HESAPKOD='63005022017' WHERE YIL=2021 AND HESAPKOD='63005020224'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Ulusal Bor Araştırma Enstitüsü ne Hazine Yardımı', HESAPKOD='63005022018' WHERE YIL=2021 AND HESAPKOD='63005020226'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Türkiye Atom Enerjisi Kurumu na Hazine Yardımı', HESAPKOD='63005022019' WHERE YIL=2021 AND HESAPKOD='63005020227'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Savunma Sanayi Başkanlığı na Hazine Yardımı', HESAPKOD='63005022020' WHERE YIL=2021 AND HESAPKOD='63005020228'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Küçük ve Orta Ölçekli İşletmeleri Geliştirme ve Destekleme İdaresi Başkanlığı na Hazine Yardımı', HESAPKOD='63005022021' WHERE YIL=2021 AND HESAPKOD='63005020230'
DELETE FROM  MUHHESAPPLANI WHERE YIL=2021 AND HESAPKOD='63005020231'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Türk İşbirliği ve Koordinasyon Ajansı Başkanlığı na Hazine Yardımı', HESAPKOD='63005022022' WHERE YIL=2021 AND HESAPKOD='63005020232'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='GAP Bölge Kalkınma İdaresi Başkanlığı na Hazine Yardımı', HESAPKOD='63005022023' WHERE YIL=2021 AND HESAPKOD='63005020234'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Özelleştirme İdaresi Başkanlığı na Hazine Yardımı', HESAPKOD='63005022024' WHERE YIL=2021 AND HESAPKOD='63005020235'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Maden Tetkik ve Arama Genel Müdürlüğü ne Hazine Yardımı', HESAPKOD='63005022025' WHERE YIL=2021 AND HESAPKOD='63005020240'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Ceza İnfaz Kurumları ile Tutukevleri İş Yurtları Kurumu na  Hazine Yardımı', HESAPKOD='63005022026' WHERE YIL=2021 AND HESAPKOD='63005020241'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sivil Havacılık Genel Müdürlüğü ne Hazine Yardımı', HESAPKOD='63005022027' WHERE YIL=2021 AND HESAPKOD='63005020249'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yurtdışı Türkler ve Akraba Topluluklar Başkanlığına Hazine Yardımı', HESAPKOD='63005022029' WHERE YIL=2021 AND HESAPKOD='63005020251'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Karayolları Genel Müdürlüğü ne Hazine Yardımı', HESAPKOD='63005022030' WHERE YIL=2021 AND HESAPKOD='63005020252'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Türkiye Yazma Eserler Kurumu Başkanlığı na Hazine Yardımı', HESAPKOD='63005022031' WHERE YIL=2021 AND HESAPKOD='63005020253'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Doğu Anadolu Projesi Bölge Kalkınma İdaresi Başkanlığı na Hazine Yardımı', HESAPKOD='63005022032' WHERE YIL=2021 AND HESAPKOD='63005020254'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Konya Ovası Projesi Bölge Kalkınma İdaresi Başkanlığı na Hazine Yardımı', HESAPKOD='63005022033' WHERE YIL=2021 AND HESAPKOD='63005020255'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Doğu Karadeniz Projesi Bölge Kalkınma İdaresi Başkanlığı na Hazine Yardımı', HESAPKOD='63005022034' WHERE YIL=2021 AND HESAPKOD='63005020256'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Devlet Su İşleri Genel Müdürlüğü ne Hazine Yardımı', HESAPKOD='63005022035' WHERE YIL=2021 AND HESAPKOD='63005020257'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Türkiye Su Enstitüsü ne Hazine Yardımı', HESAPKOD='63005022036' WHERE YIL=2021 AND HESAPKOD='63005020258'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Türkiye İlaç ve Tıbbi Cihaz Kurumu na Hazine Yardımı', HESAPKOD='63005022037' WHERE YIL=2021 AND HESAPKOD='63005020259'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kamu Denetçiliği Kurumu na Hazine Yardımı', HESAPKOD='63005022038' WHERE YIL=2021 AND HESAPKOD='63005020260'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Türkiye İnsan Hakları ve Eşitlik Kurumu na Hazine Yardımı', HESAPKOD='63005022039' WHERE YIL=2021 AND HESAPKOD='63005020261'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Türkiye Sağlık Enstitüleri Başkanlığı na Hazine Yardımı', HESAPKOD='63005022040' WHERE YIL=2021 AND HESAPKOD='63005020262'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Helal Akreditasyon Kurumu na Hazine Yardımı', HESAPKOD='63005022041' WHERE YIL=2021 AND HESAPKOD='63005020263'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Nadir Toprak Elementleri Araştırma Enstitüsü ne Hazine Yardımı', HESAPKOD='63005022042' WHERE YIL=2021 AND HESAPKOD='63005020264'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Maden ve Petrol İşleri Genel Müdürlüğü ne Hazine Yardımı', HESAPKOD='63005022043' WHERE YIL=2021 AND HESAPKOD='63005020265'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Türkiye Uzay Ajansı na Hazine Yardımı', HESAPKOD='63005022044' WHERE YIL=2021 AND HESAPKOD='63005020266'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Düzenleyici ve Denetleyici Kurumlara Hazine Yardımları', HESAPKOD='630050230' WHERE YIL=2021 AND HESAPKOD='630050203'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Radyo ve Televizyon Üst Kurulu na Hazine Yardımı', HESAPKOD='63005023001' WHERE YIL=2021 AND HESAPKOD='63005020301'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Bilgi Teknolojileri ve İletişim Kurumu na Hazine Yardımı', HESAPKOD='63005023002' WHERE YIL=2021 AND HESAPKOD='63005020302'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sermaye Piyasası Kurulu na Hazine Yardımı', HESAPKOD='63005023003' WHERE YIL=2021 AND HESAPKOD='63005020303'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Bankacılık Düzenleme ve Denetleme Kurumu na Hazine Yardımı', HESAPKOD='63005023004' WHERE YIL=2021 AND HESAPKOD='63005020304'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Enerji Piyasası Düzenleme Kurumu na Hazine Yardımı', HESAPKOD='63005023005' WHERE YIL=2021 AND HESAPKOD='63005020305'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kamu İhale Kurumu na Hazine Yardımı', HESAPKOD='63005023006' WHERE YIL=2021 AND HESAPKOD='63005020306'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Rekabet Kurumu na Hazine Yardımı', HESAPKOD='63005023007' WHERE YIL=2021 AND HESAPKOD='63005020307'
DELETE FROM  MUHHESAPPLANI WHERE YIL=2021 AND HESAPKOD='63005020309'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kamu Gözetimi, Muhasebe ve Denetim Standartları Kurumu na Hazine Yardımı', HESAPKOD='63005023008' WHERE YIL=2021 AND HESAPKOD='63005020310'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kişisel Verileri Koruma Kurumu na Hazine Yardımı', HESAPKOD='63005023009' WHERE YIL=2021 AND HESAPKOD='63005020311'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Nükleer Düzenleme Kurumu na Hazine Yardımı', HESAPKOD='63005023010' WHERE YIL=2021 AND HESAPKOD='63005020312'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sigortacılık ve Özel Emeklilik Düzenleme ve Denetleme Kurumu na Hazine Yardımı', HESAPKOD='63005023011' WHERE YIL=2021 AND HESAPKOD='63005020313'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sosyal Güvenlik Kurumlarına Hazine Yardımları', HESAPKOD='630050240' WHERE YIL=2021 AND HESAPKOD='630050204'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Türkiye İş Kurumu Genel Müdürlüğü ne Hazine Yardımı', HESAPKOD='63005024001' WHERE YIL=2021 AND HESAPKOD='63005020404'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sosyal Güvenlik Kurumu na Açık Finansmanı Ödemeleri', HESAPKOD='63005024002' WHERE YIL=2021 AND HESAPKOD='63005020406'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Mahalli İdarelere Hazine Yardımları', HESAPKOD='630050250' WHERE YIL=2021 AND HESAPKOD='630050205'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='İl Özel İdarelerine', HESAPKOD='63005025001' WHERE YIL=2021 AND HESAPKOD='63005020501'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Belediyelere', HESAPKOD='63005025002' WHERE YIL=2021 AND HESAPKOD='63005020502'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Köylere', HESAPKOD='63005025003' WHERE YIL=2021 AND HESAPKOD='63005020503'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Mahalli İdare Birliklerine', HESAPKOD='63005025004' WHERE YIL=2021 AND HESAPKOD='63005020504'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Bağlı İdarelere', HESAPKOD='63005025005' WHERE YIL=2021 AND HESAPKOD='63005020505'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Hasılattan Paylar ve Giderlere Katılma Payları', HESAPKOD='630050260' WHERE YIL=2021 AND HESAPKOD='630050206'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Giderlere Katılma Payları', HESAPKOD='63005026002' WHERE YIL=2021 AND HESAPKOD='63005020619'
DELETE FROM  MUHHESAPPLANI WHERE YIL=2021 AND HESAPKOD='63005020608'
DELETE FROM  MUHHESAPPLANI WHERE YIL=2021 AND HESAPKOD='63005020609'
DELETE FROM  MUHHESAPPLANI WHERE YIL=2021 AND HESAPKOD='63005020610'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yükseköğretim Kurumlarına Hazine Yardımları', HESAPKOD='630050270' WHERE YIL=2021 AND HESAPKOD='630050207'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yükseköğretim Kurulu na Hazine Yardımı', HESAPKOD='63005027001' WHERE YIL=2021 AND HESAPKOD='63005020701'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Ankara Üniversitesi ne Hazine Yardımı', HESAPKOD='63005027002' WHERE YIL=2021 AND HESAPKOD='63005020702'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Ortadoğu Teknik Üniversitesi ne Hazine Yardımı', HESAPKOD='63005027003' WHERE YIL=2021 AND HESAPKOD='63005020703'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Hacettepe Üniversitesi ne Hazine Yardımı', HESAPKOD='63005027004' WHERE YIL=2021 AND HESAPKOD='63005020704'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Gazi Üniversitesi ne Hazine Yardımı', HESAPKOD='63005027005' WHERE YIL=2021 AND HESAPKOD='63005020705'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='İstanbul Üniversitesi ne Hazine Yardımı', HESAPKOD='63005027006' WHERE YIL=2021 AND HESAPKOD='63005020706'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='İstanbul Teknik Üniversitesi ne Hazine Yardımı', HESAPKOD='63005027007' WHERE YIL=2021 AND HESAPKOD='63005020707'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Boğaziçi Üniversitesi ne Hazine Yardımı', HESAPKOD='63005027008' WHERE YIL=2021 AND HESAPKOD='63005020708'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Marmara Üniversitesi ne Hazine Yardımı', HESAPKOD='63005027009' WHERE YIL=2021 AND HESAPKOD='63005020709'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yıldız Teknik Üniversitesi ne Hazine Yardımı', HESAPKOD='63005027010' WHERE YIL=2021 AND HESAPKOD='63005020710'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Mimar Sinan Güzel Sanatlar Üniversitesi ne Hazine Yardımı', HESAPKOD='63005027011' WHERE YIL=2021 AND HESAPKOD='63005020711'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Ege Üniversitesi ne Hazine Yardımı', HESAPKOD='63005027012' WHERE YIL=2021 AND HESAPKOD='63005020712'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Dokuz Eylül Üniversitesi ne Hazine Yardımı', HESAPKOD='63005027013' WHERE YIL=2021 AND HESAPKOD='63005020713'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Trakya Üniversitesi ne Hazine Yardımı', HESAPKOD='63005027014' WHERE YIL=2021 AND HESAPKOD='63005020714'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Bursa Uludağ Üniversitesi ne Hazine Yardımı', HESAPKOD='63005027015' WHERE YIL=2021 AND HESAPKOD='63005020715'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Anadolu Üniversitesi ne Hazine Yardımı', HESAPKOD='63005027016' WHERE YIL=2021 AND HESAPKOD='63005020716'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Selçuk Üniversitesi ne Hazine Yardımı', HESAPKOD='63005027017' WHERE YIL=2021 AND HESAPKOD='63005020717'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Akdeniz Üniversitesi ne Hazine Yardımı', HESAPKOD='63005027018' WHERE YIL=2021 AND HESAPKOD='63005020718'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Erciyes Üniversitesi ne Hazine Yardımı', HESAPKOD='63005027019' WHERE YIL=2021 AND HESAPKOD='63005020719'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sivas Cumhuriyet Üniversitesi ne Hazine Yardımı', HESAPKOD='63005027020' WHERE YIL=2021 AND HESAPKOD='63005020720'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Çukurova Üniversitesi ne Hazine Yardımı', HESAPKOD='63005027021' WHERE YIL=2021 AND HESAPKOD='63005020721'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Ondokuz Mayıs Üniversitesi ne Hazine Yardımı', HESAPKOD='63005027022' WHERE YIL=2021 AND HESAPKOD='63005020722'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Karadeniz Teknik Üniversitesi ne Hazine Yardımı', HESAPKOD='63005027023' WHERE YIL=2021 AND HESAPKOD='63005020723'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Atatürk Üniversitesi ne Hazine Yardımı', HESAPKOD='63005027024' WHERE YIL=2021 AND HESAPKOD='63005020724'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='İnönü Üniversitesi ne Hazine Yardımı', HESAPKOD='63005027025' WHERE YIL=2021 AND HESAPKOD='63005020725'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Fırat Üniversitesi ne Hazine Yardımı', HESAPKOD='63005027026' WHERE YIL=2021 AND HESAPKOD='63005020726'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Dicle Üniversitesi ne Hazine Yardımı', HESAPKOD='63005027027' WHERE YIL=2021 AND HESAPKOD='63005020727'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yüzüncüyıl Üniversitesi ne Hazine Yardımı', HESAPKOD='63005027028' WHERE YIL=2021 AND HESAPKOD='63005020728'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Gaziantep Üniversitesi ne Hazine Yardımı', HESAPKOD='63005027029' WHERE YIL=2021 AND HESAPKOD='63005020729'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='İzmir Yüksek Teknoloji Enstitüsü ne Hazine Yardımı', HESAPKOD='63005027030' WHERE YIL=2021 AND HESAPKOD='63005020730'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Gebze Teknik Üniversitesi ne Hazine Yardımı', HESAPKOD='63005027031' WHERE YIL=2021 AND HESAPKOD='63005020731'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Harran Üniversitesi ne Hazine Yardımı', HESAPKOD='63005027032' WHERE YIL=2021 AND HESAPKOD='63005020732'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Süleyman Demirel Üniversitesi ne Hazine Yardımı', HESAPKOD='63005027033' WHERE YIL=2021 AND HESAPKOD='63005020733'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Aydın Adnan Menderes Üniversitesi ne Hazine Yardımı', HESAPKOD='63005027034' WHERE YIL=2021 AND HESAPKOD='63005020734'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Zonguldak Bülent Ecevit Üniversitesi ne Hazine Yardımı', HESAPKOD='63005027035' WHERE YIL=2021 AND HESAPKOD='63005020735'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Mersin Üniversitesi ne Hazine Yardımı', HESAPKOD='63005027036' WHERE YIL=2021 AND HESAPKOD='63005020736'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Pamukkale Üniversitesi ne Hazine Yardımı', HESAPKOD='63005027037' WHERE YIL=2021 AND HESAPKOD='63005020737'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Balıkesir Üniversitesi ne Hazine Yardımı', HESAPKOD='63005027038' WHERE YIL=2021 AND HESAPKOD='63005020738'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kocaeli Üniversitesi ne Hazine Yardımı', HESAPKOD='63005027039' WHERE YIL=2021 AND HESAPKOD='63005020739'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sakarya Üniversitesi ne Hazine Yardımı', HESAPKOD='63005027040' WHERE YIL=2021 AND HESAPKOD='63005020740'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Manisa Celal Bayar Üniversitesi’ne Hazine Yardımı', HESAPKOD='63005027041' WHERE YIL=2021 AND HESAPKOD='63005020741'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Bolu Abant İzzet Baysal Üniversitesi ne Hazine Yardımı', HESAPKOD='63005027042' WHERE YIL=2021 AND HESAPKOD='63005020742'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Hatay Mustafa Kemal Üniversitesi ne Hazine Yardımı', HESAPKOD='63005027043' WHERE YIL=2021 AND HESAPKOD='63005020743'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Afyon Kocatepe Üniversitesi ne Hazine Yardımı', HESAPKOD='63005027044' WHERE YIL=2021 AND HESAPKOD='63005020744'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kafkas Üniversitesi ne Hazine Yardımı', HESAPKOD='63005027045' WHERE YIL=2021 AND HESAPKOD='63005020745'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Çanakkale 18 Mart Üniversitesi ne Hazine Yardımı', HESAPKOD='63005027046' WHERE YIL=2021 AND HESAPKOD='63005020746'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Niğde Ömer Halisdemir Üniversitesi’ne Hazine Yardımı', HESAPKOD='63005027047' WHERE YIL=2021 AND HESAPKOD='63005020747'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kütahya Dumlupınar Üniversitesi ne Hazine Yardımı', HESAPKOD='63005027048' WHERE YIL=2021 AND HESAPKOD='63005020748'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Tokat Gaziosmanpaşa Üniversitesi ne Hazine Yardımı', HESAPKOD='63005027049' WHERE YIL=2021 AND HESAPKOD='63005020749'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Muğla Sıtkı Koçman Üniversitesi ne Hazine Yardımı', HESAPKOD='63005027050' WHERE YIL=2021 AND HESAPKOD='63005020750'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kahramanmaraş Sütçü İmam Üniversitesi ne Hazine Yardımı', HESAPKOD='63005027051' WHERE YIL=2021 AND HESAPKOD='63005020751'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kırıkkale Üniversitesi ne Hazine Yardımı', HESAPKOD='63005027052' WHERE YIL=2021 AND HESAPKOD='63005020752'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Eskişehir Osmangazi Üniversitesi ne Hazine Yardımı', HESAPKOD='63005027053' WHERE YIL=2021 AND HESAPKOD='63005020753'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Galatasaray Üniversitesi ne Hazine Yardımı', HESAPKOD='63005027054' WHERE YIL=2021 AND HESAPKOD='63005020754'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kırşehir Ahi Evran Üniversitesi ne Hazine Yardımı', HESAPKOD='63005027055' WHERE YIL=2021 AND HESAPKOD='63005020755'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kastamonu Üniversitesi ne Hazine Yardımı', HESAPKOD='63005027056' WHERE YIL=2021 AND HESAPKOD='63005020756'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Düzce Üniversitesi ne Hazine Yardımı', HESAPKOD='63005027057' WHERE YIL=2021 AND HESAPKOD='63005020757'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Burdur Mehmet Akif Ersoy Üniversitesi ne Hazine Yardımı', HESAPKOD='63005027058' WHERE YIL=2021 AND HESAPKOD='63005020758'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Uşak Üniversitesi ne Hazine Yardımı', HESAPKOD='63005027059' WHERE YIL=2021 AND HESAPKOD='63005020759'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Recep Tayyip Erdoğan Üniversitesi ne Hazine Yardımı', HESAPKOD='63005027060' WHERE YIL=2021 AND HESAPKOD='63005020760'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Tekirdağ Namık Kemal Üniversitesi ne Hazine Yardımı', HESAPKOD='63005027061' WHERE YIL=2021 AND HESAPKOD='63005020761'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Erzincan Binali Yıldırım Üniversitesi ne Hazine Yardımı', HESAPKOD='63005027062' WHERE YIL=2021 AND HESAPKOD='63005020762'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Aksaray Üniversitesi ne Hazine Yardımı', HESAPKOD='63005027063' WHERE YIL=2021 AND HESAPKOD='63005020763'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Giresun Üniversitesi ne Hazine Yardımı', HESAPKOD='63005027064' WHERE YIL=2021 AND HESAPKOD='63005020764'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Hitit Üniversitesi ne Hazine Yardımı', HESAPKOD='63005027065' WHERE YIL=2021 AND HESAPKOD='63005020765'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yozgat Bozok Üniversitesi ne Hazine Yardımı', HESAPKOD='63005027066' WHERE YIL=2021 AND HESAPKOD='63005020766'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Adıyaman Üniversitesi ne Hazine Yardımı', HESAPKOD='63005027067' WHERE YIL=2021 AND HESAPKOD='63005020767'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Ordu Üniversitesi ne Hazine Yardımı', HESAPKOD='63005027068' WHERE YIL=2021 AND HESAPKOD='63005020768'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Amasya Üniversitesi ne Hazine Yardımı', HESAPKOD='63005027069' WHERE YIL=2021 AND HESAPKOD='63005020769'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Karamanoğlu Mehmetbey Üniversitesi ne Hazine Yardımı', HESAPKOD='63005027070' WHERE YIL=2021 AND HESAPKOD='63005020770'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Ağrı İbrahim Çeçen Üniversitesi ne Hazine Yardımı', HESAPKOD='63005027071' WHERE YIL=2021 AND HESAPKOD='63005020771'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sinop Üniversitesi ne Hazine Yardımı', HESAPKOD='63005027072' WHERE YIL=2021 AND HESAPKOD='63005020772'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Siirt Üniversitesi ne Hazine Yardımı', HESAPKOD='63005027073' WHERE YIL=2021 AND HESAPKOD='63005020773'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Nevşehir Hacı Bektaş Veli Üniversitesi ne Hazine Yardımı', HESAPKOD='63005027074' WHERE YIL=2021 AND HESAPKOD='63005020774'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Karabük Üniversitesi ne Hazine Yardımı', HESAPKOD='63005027075' WHERE YIL=2021 AND HESAPKOD='63005020775'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kilis 7 Aralık Üniversitesi ne Hazine Yardımı', HESAPKOD='63005027076' WHERE YIL=2021 AND HESAPKOD='63005020776'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Çankırı Karatekin Üniversitesi ne Hazine Yardımı', HESAPKOD='63005027077' WHERE YIL=2021 AND HESAPKOD='63005020777'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Artvin Çoruh Üniversitesi ne Hazine Yardımı', HESAPKOD='63005027078' WHERE YIL=2021 AND HESAPKOD='63005020778'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Bilecik Şeyh Edebali Üniversitesi ne Hazine Yardımı', HESAPKOD='63005027079' WHERE YIL=2021 AND HESAPKOD='63005020779'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Bitlis Eren Üniversitesi ne Hazine Yardımı', HESAPKOD='63005027080' WHERE YIL=2021 AND HESAPKOD='63005020780'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kırklareli Üniversitesi ne Hazine Yardımı', HESAPKOD='63005027081' WHERE YIL=2021 AND HESAPKOD='63005020781'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Osmaniye Korkut Ata Üniversitesi ne Hazine Yardımı', HESAPKOD='63005027082' WHERE YIL=2021 AND HESAPKOD='63005020782'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Bingöl Üniversitesi ne Hazine Yardımı', HESAPKOD='63005027083' WHERE YIL=2021 AND HESAPKOD='63005020783'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Muş Alparslan Üniversitesi ne Hazine Yardımı', HESAPKOD='63005027084' WHERE YIL=2021 AND HESAPKOD='63005020784'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Mardin Artuklu Üniversitesi ne Hazine Yardımı', HESAPKOD='63005027085' WHERE YIL=2021 AND HESAPKOD='63005020785'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Batman Üniversitesi ne Hazine Yardımı', HESAPKOD='63005027086' WHERE YIL=2021 AND HESAPKOD='63005020786'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Ardahan Üniversitesi ne Hazine Yardımı', HESAPKOD='63005027087' WHERE YIL=2021 AND HESAPKOD='63005020787'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Bartın Üniversitesi ne Hazine Yardımı', HESAPKOD='63005027088' WHERE YIL=2021 AND HESAPKOD='63005020788'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Bayburt Üniversitesi ne Hazine Yardımı', HESAPKOD='63005027089' WHERE YIL=2021 AND HESAPKOD='63005020789'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Gümüşhane Üniversitesi ne Hazine Yardımı', HESAPKOD='63005027090' WHERE YIL=2021 AND HESAPKOD='63005020790'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Hakkari Üniversitesi ne Hazine Yardımı', HESAPKOD='63005027091' WHERE YIL=2021 AND HESAPKOD='63005020791'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Iğdır Üniversitesi ne Hazine Yardımı', HESAPKOD='63005027092' WHERE YIL=2021 AND HESAPKOD='63005020792'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Şırnak Üniversitesi ne Hazine Yardımı', HESAPKOD='63005027093' WHERE YIL=2021 AND HESAPKOD='63005020793'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Munzur Üniversitesi ne Hazine Yardımı', HESAPKOD='63005027094' WHERE YIL=2021 AND HESAPKOD='63005020794'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yalova Üniversitesi ne Hazine Yardımı', HESAPKOD='63005027095' WHERE YIL=2021 AND HESAPKOD='63005020795'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Türk-Alman Üniversitesi ne Hazine Yardımı', HESAPKOD='63005027096' WHERE YIL=2021 AND HESAPKOD='63005020796'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Ankara Yıldırım Beyazıt Üniversitesi ne Hazine Yardımı', HESAPKOD='63005027097' WHERE YIL=2021 AND HESAPKOD='63005020797'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Bursa Teknik Üniversitesi ne Hazine Yardımı', HESAPKOD='63005027098' WHERE YIL=2021 AND HESAPKOD='63005020798'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='İstanbul Medeniyet Üniversitesi ne Hazine Yardımı', HESAPKOD='63005027099' WHERE YIL=2021 AND HESAPKOD='63005020799'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yükseköğretim Kurumlarına Hazine Yardımları', HESAPKOD='630050280' WHERE YIL=2021 AND HESAPKOD='630050208'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='İzmir Katip Çelebi Üniversitesi ne Hazine Yardımı', HESAPKOD='63005028001' WHERE YIL=2021 AND HESAPKOD='63005020801'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Necmettin Erbakan Üniversitesi ne Hazine Yardımı', HESAPKOD='63005028002' WHERE YIL=2021 AND HESAPKOD='63005020802'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Abdullah Gül Üniversitesi ne Hazine Yardımı', HESAPKOD='63005028003' WHERE YIL=2021 AND HESAPKOD='63005020803'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Erzurum Teknik Üniversitesi ne Hazine Yardımı', HESAPKOD='63005028004' WHERE YIL=2021 AND HESAPKOD='63005020804'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Adana Alparslan Türkeş Bilim ve Teknoloji Üniversitesi ne Hazine Yardımı', HESAPKOD='63005028005' WHERE YIL=2021 AND HESAPKOD='63005020805'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Ankara Sosyal Bilimler Üniversitesi ne Hazine Yardımı', HESAPKOD='63005028006' WHERE YIL=2021 AND HESAPKOD='63005020806'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sağlık Bilimleri Üniversitesi ne Hazine Yardımı', HESAPKOD='63005028007' WHERE YIL=2021 AND HESAPKOD='63005020807'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Bandırma Onyedi Eylül Üniversitesi ne Hazine Yardımı', HESAPKOD='63005028008' WHERE YIL=2021 AND HESAPKOD='63005020808'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='İskenderun Teknik Üniversitesi ne Hazine Yardımı', HESAPKOD='63005028009' WHERE YIL=2021 AND HESAPKOD='63005020809'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Alanya Alaaddin Keykubat Üniversitesi ne Hazine Yardımı', HESAPKOD='63005028010' WHERE YIL=2021 AND HESAPKOD='63005020810'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Türkiye Uluslararası İslam, Bilim ve Teknoloji Üniversitesi ne Hazine Yardımı', HESAPKOD='63005028011' WHERE YIL=2021 AND HESAPKOD='63005020811'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='İzmir Bakırçay Üniversitesi ne Hazine Yardımı', HESAPKOD='63005028012' WHERE YIL=2021 AND HESAPKOD='63005020812'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='İzmir Demokrasi Üniversitesi ne Hazine Yardımı', HESAPKOD='63005028013' WHERE YIL=2021 AND HESAPKOD='63005020813'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yükseköğretim Kalite Kurulu na Hazine Yardımı', HESAPKOD='63005028014' WHERE YIL=2021 AND HESAPKOD='63005020814'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Ankara Müzik ve Güzel Sanatlar Üniversitesi ne Hazine Yardımı', HESAPKOD='63005028015' WHERE YIL=2021 AND HESAPKOD='63005020815'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Gaziantep İslam Bilim ve Teknoloji Üniversitesi ne Hazine Yardımı', HESAPKOD='63005028016' WHERE YIL=2021 AND HESAPKOD='63005020816'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Konya Teknik Üniversitesi ne Hazine Yardımı', HESAPKOD='63005028017' WHERE YIL=2021 AND HESAPKOD='63005020817'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kütahya Sağlık Bilimleri Üniversitesi ne Hazine Yardımı', HESAPKOD='63005028018' WHERE YIL=2021 AND HESAPKOD='63005020818'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Malatya Turgut Özal Üniversitesi ne Hazine Yardımı', HESAPKOD='63005028019' WHERE YIL=2021 AND HESAPKOD='63005020819'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='İstanbul Üniversitesi-Cerrahpaşa ya Hazine Yardımı', HESAPKOD='63005028020' WHERE YIL=2021 AND HESAPKOD='63005020820'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Ankara Hacı Bayram Veli Üniversitesi ne Hazine Yardımı', HESAPKOD='63005028021' WHERE YIL=2021 AND HESAPKOD='63005020821'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sakarya Uygulamalı Bilimler Üniversitesi ne Hazine Yardımı', HESAPKOD='63005028022' WHERE YIL=2021 AND HESAPKOD='63005020822'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Samsun Üniversitesi ne Hazine Yardımı', HESAPKOD='63005028023' WHERE YIL=2021 AND HESAPKOD='63005020823'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sivas Bilim ve Teknoloji Üniversitesi ne Hazine Yardımı', HESAPKOD='63005028024' WHERE YIL=2021 AND HESAPKOD='63005020824'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Tarsus Üniversitesi ne Hazine Yardımı', HESAPKOD='63005028025' WHERE YIL=2021 AND HESAPKOD='63005020825'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Trabzon Üniversitesi ne Hazine Yardımı', HESAPKOD='63005028026' WHERE YIL=2021 AND HESAPKOD='63005020826'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kayseri Üniversitesi ne Hazine Yardımı', HESAPKOD='63005028027' WHERE YIL=2021 AND HESAPKOD='63005020827'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kahramanmaraş İstiklal Üniversitesi ne Hazine Yardımı', HESAPKOD='63005028028' WHERE YIL=2021 AND HESAPKOD='63005020828'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Eskişehir Teknik Üniversitesi ne Hazine Yardımı', HESAPKOD='63005028029' WHERE YIL=2021 AND HESAPKOD='63005020829'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Isparta Uygulamalı Bilimler Üniversitesi ne Hazine Yardımı', HESAPKOD='63005028030' WHERE YIL=2021 AND HESAPKOD='63005020830'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Afyonkarahisar Sağlık Bilimleri Üniversitesi ne Hazine Yardımı', HESAPKOD='63005028031' WHERE YIL=2021 AND HESAPKOD='63005020831'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sınıflandırmaya Girmeyen Diğer Hazine Yardımları', HESAPKOD='63005029090' WHERE YIL=2021 AND HESAPKOD='63005020880'
DELETE FROM  MUHHESAPPLANI WHERE YIL=2021 AND HESAPKOD='63005020890'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Hazine Yardımları', HESAPKOD='630050290' WHERE YIL=2021 AND HESAPKOD='630050209'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Gençlik ve Spor İl Müdürlükleri ne Hazine Yardımı', HESAPKOD='63005029001' WHERE YIL=2021 AND HESAPKOD='63005020911'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Merkezi Finans ve İhale Birimi ne Hazine Yardımı', HESAPKOD='63005029002' WHERE YIL=2021 AND HESAPKOD='63005020912'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sınıflandırmaya Girmeyen Diğer Hazine Yardımları', HESAPKOD='63005029090' WHERE YIL=2021 AND HESAPKOD='63005020913'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Destekleme ve Fiyat İstikrar Fonu na Hazine Yardımı', HESAPKOD='63005029003' WHERE YIL=2021 AND HESAPKOD='63005020914'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='İşsizlik Sigortası Fonu na Hazine Yardımı', HESAPKOD='63005029004' WHERE YIL=2021 AND HESAPKOD='63005020915'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yatırım Ofisi’ne Hazine Yardımı', HESAPKOD='63005029005' WHERE YIL=2021 AND HESAPKOD='63005020917'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Tarım ve Kırsal Kalkınmayı Destekleme Kurumu na Hazine Yardımı', HESAPKOD='63005029009' WHERE YIL=2021 AND HESAPKOD='63005020918'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='İller Bankası AŞ ye Hazine Yardımı', HESAPKOD='63005029010' WHERE YIL=2021 AND HESAPKOD='63005020919'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Üniversite Döner Sermaye Bütçelerine Hazine Yardımı', HESAPKOD='63005029011' WHERE YIL=2021 AND HESAPKOD='63005020920'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Çanakkale Savaşları Gelibolu Tarihi Alan Başkanlığı’na Hazine Yardımı', HESAPKOD='63005029012' WHERE YIL=2021 AND HESAPKOD='63005020921'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sınıflandırmaya Girmeyen Diğer Hazine Yardımları', HESAPKOD='63005029090' WHERE YIL=2021 AND HESAPKOD='63005020990'
DELETE FROM  MUHHESAPPLANI WHERE YIL=2021 AND HESAPKOD='63005020999'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kar Amacı Gütmeyen Kuruluşlara Yapılan Transferler', HESAPKOD='6300503' WHERE YIL=2021 AND HESAPKOD='6300503'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kar Amacı Gütmeyen Kuruluşlara', HESAPKOD='630050310' WHERE YIL=2021 AND HESAPKOD='630050301'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Dernek, Birlik, Kurum, Kuruluş, Sandık vb Kuruluşlara', HESAPKOD='63005031001' WHERE YIL=2021 AND HESAPKOD='63005030101'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Vakıf Üniversitelerine', HESAPKOD='63005031002' WHERE YIL=2021 AND HESAPKOD='63005030102'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kamu İşveren Sendikalarına', HESAPKOD='63005031003' WHERE YIL=2021 AND HESAPKOD='63005030103'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Siyasi Partilere', HESAPKOD='63005031004' WHERE YIL=2021 AND HESAPKOD='63005030104'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Memurların Öğle Yemeğine Yardım', HESAPKOD='63005031005' WHERE YIL=2021 AND HESAPKOD='63005030105'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sosyal Amaçlı Fonlara', HESAPKOD='63005031006' WHERE YIL=2021 AND HESAPKOD='63005030106'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Seçim Giderleri İçin Yapılacak Ödemeler', HESAPKOD='63005031007' WHERE YIL=2021 AND HESAPKOD='63005030107'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Türk Kültür Varlığının Korunması ve Tanıtımı Giderleri', HESAPKOD='63005031008' WHERE YIL=2021 AND HESAPKOD='63005030108'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Siyasi Tanıtma Giderleri', HESAPKOD='63005031009' WHERE YIL=2021 AND HESAPKOD='63005030109'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Mesleki Yeterlilik Kurumuna Yapılacak Ödemeler', HESAPKOD='63005031010' WHERE YIL=2021 AND HESAPKOD='63005030110'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Bilimsel Araştırma Projelerinin Desteklenmesine İlişkin Giderler', HESAPKOD='63005031011' WHERE YIL=2021 AND HESAPKOD='63005030111'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Avrupa Birliği Eğitim ve Gençlik Programları Merkezi Başkanlığına', HESAPKOD='63005031012' WHERE YIL=2021 AND HESAPKOD='63005030112'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Bağımsız Spor Federasyonlarına', HESAPKOD='63005031013' WHERE YIL=2021 AND HESAPKOD='63005030113'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Afet Riski Altındaki Alanların Dönüşüm Giderleri', HESAPKOD='63005031014' WHERE YIL=2021 AND HESAPKOD='63005030114'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Türkiye Maarif Vakfına', HESAPKOD='63005031015' WHERE YIL=2021 AND HESAPKOD='63005030115'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Ulusal Fona', HESAPKOD='63005031080' WHERE YIL=2021 AND HESAPKOD='63005030180'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kar Amacı Gütmeyen Diğer Kuruluşlara', HESAPKOD='63005031090' WHERE YIL=2021 AND HESAPKOD='63005030190'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Hanehalkı ve İşletmelere Yapılan Transferler', HESAPKOD='6300504' WHERE YIL=2021 AND HESAPKOD='6300504'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Burslar ve Harçlıklar', HESAPKOD='630050410' WHERE YIL=2021 AND HESAPKOD='630050401'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yurtiçi Burslar ve Harçlıklar', HESAPKOD='63005041001' WHERE YIL=2021 AND HESAPKOD='63005040101'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yurtdışı Burslar', HESAPKOD='63005041002' WHERE YIL=2021 AND HESAPKOD='63005040102'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yabancı Uyruklu Kişilere Burslar ve Harçlıklar', HESAPKOD='63005041003' WHERE YIL=2021 AND HESAPKOD='63005040103'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yurtiçi Burslar ve Harçlıklar', HESAPKOD='63005041001' WHERE YIL=2021 AND HESAPKOD='63005040190'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Eğitim Amaçlı Diğer Transferler', HESAPKOD='630050420' WHERE YIL=2021 AND HESAPKOD='630050402'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Parasız Yatılı Öğrenci Pansiyon Ücretleri', HESAPKOD='63005042001' WHERE YIL=2021 AND HESAPKOD='63005040201'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yabancı Uyruklu Kişilere Eğitim Amaçlı Transferler', HESAPKOD='63005042002' WHERE YIL=2021 AND HESAPKOD='63005040202'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sağlık Amaçlı Transferler', HESAPKOD='630050430' WHERE YIL=2021 AND HESAPKOD='630050403'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sağlık Amaçlı Transferler', HESAPKOD='63005043001' WHERE YIL=2021 AND HESAPKOD='63005040301'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sosyal Güvenliği Bulunmayanların Prim Ödemelerine İlişkin Transferler  ', HESAPKOD='63005043002' WHERE YIL=2021 AND HESAPKOD='63005040302'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yiyecek Amaçlı Transferler', HESAPKOD='630050440' WHERE YIL=2021 AND HESAPKOD='630050404'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yiyecek Amaçlı Transferler', HESAPKOD='63005044001' WHERE YIL=2021 AND HESAPKOD='63005040401'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Barınma Amaçlı Transferler', HESAPKOD='630050450' WHERE YIL=2021 AND HESAPKOD='630050405'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Koruyucu Ailelere Yardım Ödemeleri', HESAPKOD='63005045001' WHERE YIL=2021 AND HESAPKOD='63005040501'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Barınma Amaçlı Transferler', HESAPKOD='63005045090' WHERE YIL=2021 AND HESAPKOD='63005040590'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Alan Bazlı Tarımsal Destekler', HESAPKOD='630050460' WHERE YIL=2021 AND HESAPKOD='630050406'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Doğrudan Gelir Desteği Ödemeleri', HESAPKOD='63005046912' WHERE YIL=2021 AND HESAPKOD='63005040601'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Ürün Destekleme Ödemeleri', HESAPKOD='63005046913' WHERE YIL=2021 AND HESAPKOD='63005040602'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Hayvancılık Destekleme Ödemeleri', HESAPKOD='63005046914' WHERE YIL=2021 AND HESAPKOD='63005040603'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Tarım Reformu Uygulama Projesi', HESAPKOD='63005046915' WHERE YIL=2021 AND HESAPKOD='63005040604'
DELETE FROM  MUHHESAPPLANI WHERE YIL=2021 AND HESAPKOD='63005040605'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Çevre Amaçlı Tarımsal Alanların Korunması (ÇATAK) Desteği', HESAPKOD='63005046902' WHERE YIL=2021 AND HESAPKOD='63005040606'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='İlave Doğrudan Gelir Desteği Ödemeleri', HESAPKOD='63005046916' WHERE YIL=2021 AND HESAPKOD='63005040607'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Mazot Desteği', HESAPKOD='63005046001' WHERE YIL=2021 AND HESAPKOD='63005040608'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Gübre Desteği', HESAPKOD='63005046002' WHERE YIL=2021 AND HESAPKOD='63005040609'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Alan Bazlı Tarımsal Destekler', HESAPKOD='63005046090' WHERE YIL=2021 AND HESAPKOD='63005040610'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kütlü Pamuk Desteği', HESAPKOD='63005046501' WHERE YIL=2021 AND HESAPKOD='63005040611'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yağlık Ayçiçeği Desteği', HESAPKOD='63005046502' WHERE YIL=2021 AND HESAPKOD='63005040612'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Soya Fasülyesi Desteği', HESAPKOD='63005046503' WHERE YIL=2021 AND HESAPKOD='63005040613'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kanola Desteği', HESAPKOD='63005046504' WHERE YIL=2021 AND HESAPKOD='63005040614'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Dane Mısır Desteği', HESAPKOD='63005046506' WHERE YIL=2021 AND HESAPKOD='63005040615'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Zeytinyağı Desteği', HESAPKOD='63005046507' WHERE YIL=2021 AND HESAPKOD='63005040616'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Aspir Desteği', HESAPKOD='63005046505' WHERE YIL=2021 AND HESAPKOD='63005040617'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Çay Desteği', HESAPKOD='63005046508' WHERE YIL=2021 AND HESAPKOD='63005040618'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Buğday Desteği', HESAPKOD='63005046509' WHERE YIL=2021 AND HESAPKOD='63005040619'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Çeltik Desteği', HESAPKOD='63005046510' WHERE YIL=2021 AND HESAPKOD='63005040620'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Hububat Destekleri', HESAPKOD='63005046590' WHERE YIL=2021 AND HESAPKOD='63005040621'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Fark Ödemesi Niteliğindeki Diğer Tarımsal Destekler', HESAPKOD='63005046591' WHERE YIL=2021 AND HESAPKOD='63005040622'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yem Bitkileri Üretimi Desteği', HESAPKOD='63005046103' WHERE YIL=2021 AND HESAPKOD='63005040623'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Alet ve Makine Desteği', HESAPKOD='63005046119' WHERE YIL=2021 AND HESAPKOD='63005040624'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sertifikalı Yem Bitkileri Tohumluğu Üretim Desteği', HESAPKOD='63005046120' WHERE YIL=2021 AND HESAPKOD='63005040625'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Damızlık Büyükbaş Hayvan (Düve) Desteği', HESAPKOD='63005046121' WHERE YIL=2021 AND HESAPKOD='63005040626'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Suni Tohumlama Desteği', HESAPKOD='63005046122' WHERE YIL=2021 AND HESAPKOD='63005040627'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Çiğ Süt Desteği', HESAPKOD='63005046105' WHERE YIL=2021 AND HESAPKOD='63005040628'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Et Desteği', HESAPKOD='63005046123' WHERE YIL=2021 AND HESAPKOD='63005040629'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Buzağı Desteği', HESAPKOD='63005046106' WHERE YIL=2021 AND HESAPKOD='63005040630'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Arıcılık ve Bal Desteği', HESAPKOD='63005046107' WHERE YIL=2021 AND HESAPKOD='63005040631'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Hastalıklardan Ari İşletme Desteği', HESAPKOD='63005046108' WHERE YIL=2021 AND HESAPKOD='63005040632'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Aşı Desteği', HESAPKOD='63005046109' WHERE YIL=2021 AND HESAPKOD='63005040633'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Su Ürünleri Desteği', HESAPKOD='63005046117' WHERE YIL=2021 AND HESAPKOD='63005040634'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sağım Hijyeni ve Süt Kalitesi Desteği', HESAPKOD='63005046124' WHERE YIL=2021 AND HESAPKOD='63005040635'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Islah Amaçlı Küçükbaş Hayvan Yetiştirici Birlikleri Desteği', HESAPKOD='63005046110' WHERE YIL=2021 AND HESAPKOD='63005040636'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Hayvan Gen Kaynakları Desteği', HESAPKOD='63005046118' WHERE YIL=2021 AND HESAPKOD='63005040637'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Tiftik Üretimi Desteği', HESAPKOD='63005046111' WHERE YIL=2021 AND HESAPKOD='63005040638'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='İpek Böceği Desteği', HESAPKOD='63005046112' WHERE YIL=2021 AND HESAPKOD='63005040639'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Gıda Güvenliği Desteği', HESAPKOD='63005046125' WHERE YIL=2021 AND HESAPKOD='63005040640'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Hayvan Kimlik Sistemi Desteği', HESAPKOD='63005046126' WHERE YIL=2021 AND HESAPKOD='63005040641'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kanatlı Hayvan Hastalıkları Tazminatı Desteği', HESAPKOD='63005046127' WHERE YIL=2021 AND HESAPKOD='63005040642'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Büyükbaş Hayvan Hastalıkları Tazminatı Desteği', HESAPKOD='63005046101' WHERE YIL=2021 AND HESAPKOD='63005040643'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Büyükbaş Hayvan Desteği', HESAPKOD='63005046128' WHERE YIL=2021 AND HESAPKOD='63005040644'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Çevre Amaçlı Tarımsal Alanların Korunması (ÇATAK) Desteği', HESAPKOD='63005046902' WHERE YIL=2021 AND HESAPKOD='63005040645'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Alternatif Ürün Desteği', HESAPKOD='63005046012' WHERE YIL=2021 AND HESAPKOD='63005040646'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sınıflandırmaya Girmeyen Diğer Tarımsal Destekler', HESAPKOD='63005046990' WHERE YIL=2021 AND HESAPKOD='63005040647'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sınıflandırmaya Girmeyen Diğer Tarımsal Destekler', HESAPKOD='63005046990' WHERE YIL=2021 AND HESAPKOD='63005040648'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kırsal Kalkınma Destekleri', HESAPKOD='63005046401' WHERE YIL=2021 AND HESAPKOD='63005040649'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Bitkisel Ürün Sigortası Desteği', HESAPKOD='63005046201' WHERE YIL=2021 AND HESAPKOD='63005040650'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Süt Sığırı Sigortası Desteği', HESAPKOD='63005046202' WHERE YIL=2021 AND HESAPKOD='63005040651'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sera Sigortası Desteği', HESAPKOD='63005046203' WHERE YIL=2021 AND HESAPKOD='63005040652'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kümes Hayvanları Sigortası Desteği', HESAPKOD='63005046204' WHERE YIL=2021 AND HESAPKOD='63005040653'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Hasar Fazlası Desteği', HESAPKOD='63005046205' WHERE YIL=2021 AND HESAPKOD='63005040654'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Tarım Sigortası Destekleri', HESAPKOD='63005046290' WHERE YIL=2021 AND HESAPKOD='63005040655'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Patates Siğili Desteği', HESAPKOD='63005046301' WHERE YIL=2021 AND HESAPKOD='63005040656'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Çay Budama Tazminatı Desteği', HESAPKOD='63005046303' WHERE YIL=2021 AND HESAPKOD='63005040657'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Çay Budama Masraflarını Karşılama Desteği', HESAPKOD='63005046304' WHERE YIL=2021 AND HESAPKOD='63005040658'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sınıflandırmaya Girmeyen Diğer Tarımsal Destekler', HESAPKOD='63005046990' WHERE YIL=2021 AND HESAPKOD='63005040659'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Tarımsal Ar-Ge Destekleri', HESAPKOD='63005046910' WHERE YIL=2021 AND HESAPKOD='63005040660'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sertifikalı Tohum Kullanım Desteği', HESAPKOD='63005046903' WHERE YIL=2021 AND HESAPKOD='63005040661'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sertifikalı Fidan Kullanım Desteği', HESAPKOD='63005046904' WHERE YIL=2021 AND HESAPKOD='63005040662'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sınıflandırmaya Girmeyen Diğer Tarımsal Destekler', HESAPKOD='63005046990' WHERE YIL=2021 AND HESAPKOD='63005040663'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sınıflandırmaya Girmeyen Diğer Tarımsal Destekler', HESAPKOD='63005046990' WHERE YIL=2021 AND HESAPKOD='63005040664'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Çiftlik Muhasebe Veri Ağı (ÇMVA) Katılım Desteği', HESAPKOD='63005046907' WHERE YIL=2021 AND HESAPKOD='63005040665'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kuraklık Desteği', HESAPKOD='63005046013' WHERE YIL=2021 AND HESAPKOD='63005040666'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Toprak Analizi Desteği', HESAPKOD='63005046007' WHERE YIL=2021 AND HESAPKOD='63005040667'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Organik Tarım Desteği', HESAPKOD='63005046005' WHERE YIL=2021 AND HESAPKOD='63005040668'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='İyi Tarım Uygulamaları Desteği', HESAPKOD='63005046006' WHERE YIL=2021 AND HESAPKOD='63005040669'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Bakliyat Desteği', HESAPKOD='63005046511' WHERE YIL=2021 AND HESAPKOD='63005040670'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Büyükbaş Hayvan Desteği', HESAPKOD='63005046128' WHERE YIL=2021 AND HESAPKOD='63005040671'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sınıflandırmaya Girmeyen Diğer Tarımsal Destekler', HESAPKOD='63005046990' WHERE YIL=2021 AND HESAPKOD='63005040672'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Su Ürünleri Sigortası Desteği', HESAPKOD='63005046206' WHERE YIL=2021 AND HESAPKOD='63005040673'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sertifikalı Tohum Üretim Desteği', HESAPKOD='63005046905' WHERE YIL=2021 AND HESAPKOD='63005040674'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Tarım Danışmanlığı Desteği', HESAPKOD='63005046901' WHERE YIL=2021 AND HESAPKOD='63005040675'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Bombus Arısı Desteği', HESAPKOD='63005046104' WHERE YIL=2021 AND HESAPKOD='63005040676'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Fındık Üreticileri Alan Bazlı Gelir Desteği', HESAPKOD='63005046004' WHERE YIL=2021 AND HESAPKOD='63005040677'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Fındık Alternatif Ürün Desteği', HESAPKOD='63005046010' WHERE YIL=2021 AND HESAPKOD='63005040678'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Fındık Söküm Tazminatı', HESAPKOD='63005046011' WHERE YIL=2021 AND HESAPKOD='63005040679'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sınıflandırmaya Girmeyen Diğer Tarımsal Destekler', HESAPKOD='63005046990' WHERE YIL=2021 AND HESAPKOD='63005040680'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Büyükbaş Hayvan Besi Desteği', HESAPKOD='63005046113' WHERE YIL=2021 AND HESAPKOD='63005040681'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Küçükbaş Hayvan Sigortası Desteği', HESAPKOD='63005046207' WHERE YIL=2021 AND HESAPKOD='63005040682'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Süt Regülasyonu Desteği', HESAPKOD='63005046114' WHERE YIL=2021 AND HESAPKOD='63005040683'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Biyolojik Mücadele Desteği', HESAPKOD='63005046102' WHERE YIL=2021 AND HESAPKOD='63005040684'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='IPARD Ulusal Eş Finansman Katkı Payı Ödemeleri', HESAPKOD='63005046402' WHERE YIL=2021 AND HESAPKOD='63005040685'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sürü Yöneticisi İstihdamı Desteği', HESAPKOD='63005046115' WHERE YIL=2021 AND HESAPKOD='63005040686'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Lisanslı Depolarda Ürün Depolama Kira Desteği', HESAPKOD='63005046908' WHERE YIL=2021 AND HESAPKOD='63005040687'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Beş Dekar Altındaki Küçük İşletme Desteği', HESAPKOD='63005046008' WHERE YIL=2021 AND HESAPKOD='63005040688'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Genç Çiftçilere Proje Desteği', HESAPKOD='63005046403' WHERE YIL=2021 AND HESAPKOD='63005040689'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sınıflandırmaya Girmeyen Diğer Tarımsal Destekler', HESAPKOD='63005046990' WHERE YIL=2021 AND HESAPKOD='63005040690'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Büyükbaş Damızlık Hayvan Alım Desteği', HESAPKOD='63005046116' WHERE YIL=2021 AND HESAPKOD='63005040693'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sertifikalı Fidan Üretim Desteği', HESAPKOD='63005046906' WHERE YIL=2021 AND HESAPKOD='63005040694'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Geleneksel Zeytin Bahçelerinin Rehabilitasyonu Desteği', HESAPKOD='63005046009' WHERE YIL=2021 AND HESAPKOD='63005040695'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Tarımsal Sulama Elektrik Desteği', HESAPKOD='63005046909' WHERE YIL=2021 AND HESAPKOD='63005040696'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Bitki Karantinası Tazminatı Desteği', HESAPKOD='63005046302' WHERE YIL=2021 AND HESAPKOD='63005040698'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sosyal Amaçlı Transferler', HESAPKOD='630050470' WHERE YIL=2021 AND HESAPKOD='630050407'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Muhtaç ve Engellilere Yardım', HESAPKOD='63005047001' WHERE YIL=2021 AND HESAPKOD='63005040701'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Vakıf İlgililerine Yapılan Ödemeler', HESAPKOD='63005047002' WHERE YIL=2021 AND HESAPKOD='63005040702'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Vatani Hizmet Aylıkları', HESAPKOD='63005047003' WHERE YIL=2021 AND HESAPKOD='63005040703'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sosyal Amaçlı Diğer Aylık ve Tazminatlar', HESAPKOD='63005047004' WHERE YIL=2021 AND HESAPKOD='63005040704'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Şehit Dul ve Yetimlere İkramiye ve Ek Ödeme', HESAPKOD='63005047005' WHERE YIL=2021 AND HESAPKOD='63005040705'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Sosyal Amaçlı Transferler', HESAPKOD='63005047090' WHERE YIL=2021 AND HESAPKOD='63005040790'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Ekonomik/Mali Amaçlı Transferler', HESAPKOD='630050480' WHERE YIL=2021 AND HESAPKOD='630050408'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Ekonomik/Mali Amaçlı Diğer Transferler', HESAPKOD='63005048090' WHERE YIL=2021 AND HESAPKOD='63005040801'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='KREDİ FAİZ DESTEKLERİ', HESAPKOD='6300504809001' WHERE YIL=2021 AND HESAPKOD='6300504080101'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='2016 Yılı Makine Teçhizat Kredi Faiz Desteği', HESAPKOD='630050480900101' WHERE YIL=2021 AND HESAPKOD='630050408010101'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='KOSGEB Sıfır Faizli İşletme Kredisi Faiz Desteği', HESAPKOD='630050480900102' WHERE YIL=2021 AND HESAPKOD='630050408010102'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kilis İli Acil Destek Kredisi', HESAPKOD='630050480900103' WHERE YIL=2021 AND HESAPKOD='630050408010103'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='2017 Yılı KOSGEB Sıfır Faizli İşletme Kredisi Faiz Desteği', HESAPKOD='630050480900104' WHERE YIL=2021 AND HESAPKOD='630050408010104'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='2017 Yılı Acil Destek Kredisi', HESAPKOD='630050480900105' WHERE YIL=2021 AND HESAPKOD='630050408010105'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Trabzon İli Beşikdüzü ve Vakfıkebir İlçeleri Acil Destek Kredisi', HESAPKOD='630050480900106' WHERE YIL=2021 AND HESAPKOD='630050408010106'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Osmaniye İli Kadirli İlçesi Acil Destek Kredi Faiz Desteği', HESAPKOD='630050480900107' WHERE YIL=2021 AND HESAPKOD='630050408010107'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Iğdır İli Merkez İlçesi Acil Destek Kredi Faiz Desteği', HESAPKOD='630050480900108' WHERE YIL=2021 AND HESAPKOD='630050408010108'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Şanlıurfa Acil Destek Kredi Faiz Desteği', HESAPKOD='630050480900109' WHERE YIL=2021 AND HESAPKOD='630050408010109'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='KOSGEB 2018 Yılı Makine Techizat Kredi Faiz Desteği', HESAPKOD='630050480900110' WHERE YIL=2021 AND HESAPKOD='630050408010110'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Düzce ve İstanbul Acil Destek Kredi Faiz Desteği', HESAPKOD='630050480900111' WHERE YIL=2021 AND HESAPKOD='630050408010111'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='KOBİ Finansman Destek Programı', HESAPKOD='630050480900112' WHERE YIL=2021 AND HESAPKOD='630050408010112'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='İşletme Kredisi', HESAPKOD='63005048090011201' WHERE YIL=2021 AND HESAPKOD='63005040801011201'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Makine Teçhizat Kredisi', HESAPKOD='63005048090011202' WHERE YIL=2021 AND HESAPKOD='63005040801011202'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Acil Destek Kredisi', HESAPKOD='63005048090011203' WHERE YIL=2021 AND HESAPKOD='63005040801011203'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Samsun İli Terme ve Salıpazarı İlçeleri Acil Destek Kredisi', HESAPKOD='6300504809001120301' WHERE YIL=2021 AND HESAPKOD='6300504080101120301'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Elazığ ve Malatya İlleri Acil Destek Kredisi', HESAPKOD='6300504809001120302' WHERE YIL=2021 AND HESAPKOD='6300504080101120302'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Tokat Acil Destek Kredisi', HESAPKOD='6300504809001120303' WHERE YIL=2021 AND HESAPKOD='6300504080101120303'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='İstanbul İli Esenyurt İlçesi Acil Destek Kredisi', HESAPKOD='6300504809001120304' WHERE YIL=2021 AND HESAPKOD='6300504080101120304'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Rize İli Çayeli İlçesi ve Giresun İli Acil Destek Kredisi', HESAPKOD='6300504809001120305' WHERE YIL=2021 AND HESAPKOD='6300504080101120305'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Elazığı ve Malatya İlleri İmalat Sanayi İşletme Kredisi Faiz Desteği', HESAPKOD='630050480900113' WHERE YIL=2021 AND HESAPKOD='630050408010113'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='GENEL DESTEKLER', HESAPKOD='6300504809002' WHERE YIL=2021 AND HESAPKOD='6300504080102'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yurt İçi Fuar Desteği', HESAPKOD='630050480900201' WHERE YIL=2021 AND HESAPKOD='630050408010201'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='İhtisas Fuarı Desteği', HESAPKOD='63005048090020101' WHERE YIL=2021 AND HESAPKOD='63005040801020101'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Uluslararası İhtisas Fuarı Desteği', HESAPKOD='63005048090020102' WHERE YIL=2021 AND HESAPKOD='63005040801020102'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='İzmir Enternasyonal Fuarı Desteği', HESAPKOD='63005048090020103' WHERE YIL=2021 AND HESAPKOD='63005040801020103'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yurt Dışı İş Gezisi Desteği', HESAPKOD='630050480900202' WHERE YIL=2021 AND HESAPKOD='630050408010202'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Tanıtım Desteği', HESAPKOD='630050480900203' WHERE YIL=2021 AND HESAPKOD='630050408010203'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Ürün Kataloğu Desteği', HESAPKOD='63005048090020301' WHERE YIL=2021 AND HESAPKOD='63005040801020301'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yurt Dışında Yayınlanan veya Dağıtılan Basılı Yayınlara Reklam Verme Desteği', HESAPKOD='63005048090020302' WHERE YIL=2021 AND HESAPKOD='63005040801020302'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Web/Mobil Tabanlı Tanıtım Desteği', HESAPKOD='63005048090020303' WHERE YIL=2021 AND HESAPKOD='63005040801020303'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Web Tabanlı Tanıtım Desteği', HESAPKOD='6300504809002030301' WHERE YIL=2021 AND HESAPKOD='6300504080102030301'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Mobil Tabanlı Tanıtım Desteği', HESAPKOD='6300504809002030302' WHERE YIL=2021 AND HESAPKOD='6300504080102030302'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Eşleştirme Desteği', HESAPKOD='630050480900204' WHERE YIL=2021 AND HESAPKOD='630050408010204'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Nitelikli Eleman İstihdam Desteği', HESAPKOD='630050480900205' WHERE YIL=2021 AND HESAPKOD='630050408010205'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Danışmanlık Desteği', HESAPKOD='630050480900206' WHERE YIL=2021 AND HESAPKOD='630050408010206'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Eğitim Desteği', HESAPKOD='630050480900207' WHERE YIL=2021 AND HESAPKOD='630050408010207'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Enerji Verimliliği Desteği', HESAPKOD='630050480900208' WHERE YIL=2021 AND HESAPKOD='630050408010208'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Ön Etüt Desteği', HESAPKOD='63005048090020801' WHERE YIL=2021 AND HESAPKOD='63005040801020801'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Detaylı Etüt Desteği', HESAPKOD='63005048090020802' WHERE YIL=2021 AND HESAPKOD='63005040801020802'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Verimlilik Arttırıcı Proje (VAP) İçin Danışmanlık Desteği', HESAPKOD='63005048090020803' WHERE YIL=2021 AND HESAPKOD='63005040801020803'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Enerji Yöneticisi Eğitimleri Desteği', HESAPKOD='63005048090020804' WHERE YIL=2021 AND HESAPKOD='63005040801020804'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Enerji Verimliliği Uygulama Desteği', HESAPKOD='63005048090020805' WHERE YIL=2021 AND HESAPKOD='63005040801020805'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Tasarım Desteği', HESAPKOD='630050480900209' WHERE YIL=2021 AND HESAPKOD='630050408010209'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sınai Mülkiyet Hakları Desteği', HESAPKOD='630050480900210' WHERE YIL=2021 AND HESAPKOD='630050408010210'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Patent Belgesi Desteği', HESAPKOD='63005048090021001' WHERE YIL=2021 AND HESAPKOD='63005040801021001'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Faydalı Model Belgesi Desteği', HESAPKOD='63005048090021002' WHERE YIL=2021 AND HESAPKOD='63005040801021002'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Endüstriyel Tasarım Tescili Belgesi Desteği', HESAPKOD='63005048090021003' WHERE YIL=2021 AND HESAPKOD='63005040801021003'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Entegre Devre Topografyaları Tescil Belgesi Desteği', HESAPKOD='63005048090021004' WHERE YIL=2021 AND HESAPKOD='63005040801021004'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yurt Dışı Marka Tescil Belgesi Desteği', HESAPKOD='63005048090021005' WHERE YIL=2021 AND HESAPKOD='63005040801021005'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Belgelendirme Desteği', HESAPKOD='630050480900211' WHERE YIL=2021 AND HESAPKOD='630050408010211'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Test, Analiz ve Kalibrasyon Desteği', HESAPKOD='630050480900212' WHERE YIL=2021 AND HESAPKOD='630050408010212'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Bağımsız Denetim Desteği', HESAPKOD='630050480900213' WHERE YIL=2021 AND HESAPKOD='630050408010213'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Gönüllü Uzmanlık Desteği', HESAPKOD='630050480900214' WHERE YIL=2021 AND HESAPKOD='630050408010214'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Lojistik Desteği', HESAPKOD='630050480900215' WHERE YIL=2021 AND HESAPKOD='630050408010215'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='AR-GE, İNOVASYON VE ENDÜSTRİYEL UYGULAMA DESTEKLERİ', HESAPKOD='6300504809003' WHERE YIL=2021 AND HESAPKOD='6300504080103'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Ar-Ge İnovasyon Destekleri', HESAPKOD='630050480900301' WHERE YIL=2021 AND HESAPKOD='630050408010301'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kira Desteği', HESAPKOD='63005048090030101' WHERE YIL=2021 AND HESAPKOD='63005040801030101'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Makine-Teçhizat, Donanım, Hammadde, Yazılım ve Hizmet Alımı Desteği', HESAPKOD='63005048090030102' WHERE YIL=2021 AND HESAPKOD='63005040801030102'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Personel Desteği', HESAPKOD='63005048090030103' WHERE YIL=2021 AND HESAPKOD='63005040801030103'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Başlangıç Sermayesi Desteği', HESAPKOD='63005048090030104' WHERE YIL=2021 AND HESAPKOD='63005040801030104'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Proje Geliştirme Desteği', HESAPKOD='63005048090030105' WHERE YIL=2021 AND HESAPKOD='63005040801030105'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Proje Danışmanlık Desteği', HESAPKOD='6300504809003010501' WHERE YIL=2021 AND HESAPKOD='6300504080103010501'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Eğitim Desteği', HESAPKOD='6300504809003010502' WHERE YIL=2021 AND HESAPKOD='6300504080103010502'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sınai ve Fikri Mülkiyet Hakları Desteği', HESAPKOD='6300504809003010503' WHERE YIL=2021 AND HESAPKOD='6300504080103010503'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Proje Tanıtım Desteği', HESAPKOD='6300504809003010504' WHERE YIL=2021 AND HESAPKOD='6300504080103010504'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yurtiçi-Yurtdışı Kongre/Konferans/Fuar Ziyareti/Teknolojik İşbirliği Ziyareti Desteği', HESAPKOD='6300504809003010505' WHERE YIL=2021 AND HESAPKOD='6300504080103010505'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Test, Analiz, Belgelendirme Desteği', HESAPKOD='6300504809003010506' WHERE YIL=2021 AND HESAPKOD='6300504080103010506'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Endüstriyel Uygulama Destekleri', HESAPKOD='630050480900302' WHERE YIL=2021 AND HESAPKOD='630050408010302'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kira Desteği', HESAPKOD='63005048090030201' WHERE YIL=2021 AND HESAPKOD='63005040801030201'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Makine-Teçhizat, Donanım, Sarf Malzemesi, Yazılım ve Tasarım Desteği', HESAPKOD='63005048090030202' WHERE YIL=2021 AND HESAPKOD='63005040801030202'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Personel Desteği', HESAPKOD='63005048090030203' WHERE YIL=2021 AND HESAPKOD='63005040801030203'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='GİRİŞİMCİLİK DESTEKLERİ', HESAPKOD='6300504809004' WHERE YIL=2021 AND HESAPKOD='6300504080104'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Uygulamalı Girişimcilik Eğitimi Eğitim Hizmeti Bedeli Desteği', HESAPKOD='630050480900401' WHERE YIL=2021 AND HESAPKOD='630050408010401'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yeni Girişimci Desteği', HESAPKOD='630050480900402' WHERE YIL=2021 AND HESAPKOD='630050408010402'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='İşletme Kuruluş Desteği', HESAPKOD='63005048090040201' WHERE YIL=2021 AND HESAPKOD='63005040801040201'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kuruluş Dönemi Makine, Teçhizat, Ofis Donanım ve Yazılım Desteği', HESAPKOD='63005048090040202' WHERE YIL=2021 AND HESAPKOD='63005040801040202'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='İşletme Giderleri Desteği', HESAPKOD='63005048090040203' WHERE YIL=2021 AND HESAPKOD='63005040801040203'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='İş Geliştirme Merkezi (İŞGEM) Desteği', HESAPKOD='630050480900403' WHERE YIL=2021 AND HESAPKOD='630050408010403'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='İŞGEM Kuruluş Desteği', HESAPKOD='63005048090040301' WHERE YIL=2021 AND HESAPKOD='63005040801040301'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Bina Tadilatı Desteği', HESAPKOD='6300504809004030101' WHERE YIL=2021 AND HESAPKOD='6300504080104030101'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Mobilya Donanım Desteği', HESAPKOD='6300504809004030102' WHERE YIL=2021 AND HESAPKOD='6300504080104030102'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Personel Desteği', HESAPKOD='6300504809004030103' WHERE YIL=2021 AND HESAPKOD='6300504080104030103'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='İŞGEM İşletme Desteği', HESAPKOD='63005048090040302' WHERE YIL=2021 AND HESAPKOD='63005040801040302'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Personel Desteği', HESAPKOD='6300504809004030201' WHERE YIL=2021 AND HESAPKOD='6300504080104030201'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Eğitim, Danışmanlık Desteği', HESAPKOD='6300504809004030202' WHERE YIL=2021 AND HESAPKOD='6300504080104030202'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Küçük Tadilat Desteği', HESAPKOD='6300504809004030203' WHERE YIL=2021 AND HESAPKOD='6300504080104030203'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='İŞGEM Tanıtım/Organizasyon, İşbirliği Ağlarına Erişim Desteği', HESAPKOD='6300504809004030204' WHERE YIL=2021 AND HESAPKOD='6300504080104030204'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='İş Planı Ödülü Desteği', HESAPKOD='630050480900404' WHERE YIL=2021 AND HESAPKOD='630050408010404'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='İŞBİRLİĞİ-GÜÇBİRLİĞİ DESTEKLERİ', HESAPKOD='6300504809005' WHERE YIL=2021 AND HESAPKOD='6300504080105'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='İşbirliği-Güçbirliği Destekleri', HESAPKOD='630050480900501' WHERE YIL=2021 AND HESAPKOD='630050408010501'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Personel Desteği', HESAPKOD='63005048090050101' WHERE YIL=2021 AND HESAPKOD='63005040801050101'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Makine-Teçhizat, Hammadde, Malzeme, Yazılım ve Donanım Desteği', HESAPKOD='63005048090050102' WHERE YIL=2021 AND HESAPKOD='63005040801050102'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Hizmet Alımı Desteği', HESAPKOD='63005048090050103' WHERE YIL=2021 AND HESAPKOD='63005040801050103'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Giderler Desteği', HESAPKOD='63005048090050104' WHERE YIL=2021 AND HESAPKOD='63005040801050104'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='GELİŞEN İŞLETMELER PİYASASI KOBİ DESTEKLERİ', HESAPKOD='6300504809006' WHERE YIL=2021 AND HESAPKOD='6300504080106'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Piyasa Danışmanı Danışmanlık Hizmet Bedeli Desteği', HESAPKOD='630050480900601' WHERE YIL=2021 AND HESAPKOD='630050408010601'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Bağımsız Denetim Hizmeti Bedeli Desteği', HESAPKOD='630050480900602' WHERE YIL=2021 AND HESAPKOD='630050408010602'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sermaye Piyasası Kurulu (SPK) Kurul Kaydına Alma Ücreti Desteği', HESAPKOD='630050480900603' WHERE YIL=2021 AND HESAPKOD='630050408010603'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Borsa İstanbul (BIST) Gelişen İşletmeler Piyasası Listesine Kabul Ücreti Desteği', HESAPKOD='630050480900604' WHERE YIL=2021 AND HESAPKOD='630050408010604'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Merkezi Kayıt Kuruluşu (MKK) Masrafı Desteği', HESAPKOD='630050480900605' WHERE YIL=2021 AND HESAPKOD='630050408010605'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Aracı Kuruluşa Ödenecek Aracılık Komisyonu Desteği', HESAPKOD='630050480900606' WHERE YIL=2021 AND HESAPKOD='630050408010606'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='TEKNOLOJİK ÜRÜN TANITIM VE PAZARLAMA (TEKNOPAZAR) DESTEKLERİ', HESAPKOD='6300504809007' WHERE YIL=2021 AND HESAPKOD='6300504080107'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yurt İçi Tanıtım ve Pazarlama Faaliyetleri Desteği', HESAPKOD='630050480900701' WHERE YIL=2021 AND HESAPKOD='630050408010701'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Basılı veya Elektronik Tanıtım Materyalleri Desteği', HESAPKOD='63005048090070101' WHERE YIL=2021 AND HESAPKOD='63005040801070101'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yurt İçi Fuar Katılımı Desteği', HESAPKOD='63005048090070102' WHERE YIL=2021 AND HESAPKOD='63005040801070102'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Elektronik Ticaret (e-ticaret) Sitelerine Üyelik Desteği', HESAPKOD='63005048090070103' WHERE YIL=2021 AND HESAPKOD='63005040801070103'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Konaklama ve Ulaşım Desteği', HESAPKOD='63005048090070104' WHERE YIL=2021 AND HESAPKOD='63005040801070104'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yazılı Medyada Yapılan Tanıtım Giderleri Desteği', HESAPKOD='63005048090070105' WHERE YIL=2021 AND HESAPKOD='63005040801070105'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yurt Dışı Tanıtım ve Pazarlama Faaliyetleri Desteği', HESAPKOD='630050480900702' WHERE YIL=2021 AND HESAPKOD='630050408010702'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yurt Dışı Fuar Katılımı Desteği', HESAPKOD='63005048090070201' WHERE YIL=2021 AND HESAPKOD='63005040801070201'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yurt Dışı Fuarlarda Gümrük İşlem Giderleri Desteği', HESAPKOD='63005048090070202' WHERE YIL=2021 AND HESAPKOD='63005040801070202'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Elektronik Ticaret (e-ticaret) Sitelerine Üyelik Desteği', HESAPKOD='63005048090070203' WHERE YIL=2021 AND HESAPKOD='63005040801070203'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yazılı Medyada Teknolojik Ürün ya da Prototip Tanıtım Desteği', HESAPKOD='63005048090070204' WHERE YIL=2021 AND HESAPKOD='63005040801070204'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yurt Dışı Fuarlara İlişkin Konaklama ve Ulaşım Desteği', HESAPKOD='63005048090070205' WHERE YIL=2021 AND HESAPKOD='63005040801070205'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='ULUSLARARASI KULUÇKA MERKEZİ VE HIZLANDIRICI DESTEKLERİ', HESAPKOD='6300504809008' WHERE YIL=2021 AND HESAPKOD='6300504080108'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Uluslararası Kuluçka Merkezi Kurma Programı', HESAPKOD='630050480900801' WHERE YIL=2021 AND HESAPKOD='630050408010801'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kuruluş ve Donanım Desteği', HESAPKOD='63005048090080101' WHERE YIL=2021 AND HESAPKOD='63005040801080101'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='İşletme Kuruluş Desteği', HESAPKOD='6300504809008010101' WHERE YIL=2021 AND HESAPKOD='6300504080108010101'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Bina Tadilat ve Altyapı Desteği', HESAPKOD='6300504809008010102' WHERE YIL=2021 AND HESAPKOD='6300504080108010102'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Ofis Donanım Desteği', HESAPKOD='6300504809008010103' WHERE YIL=2021 AND HESAPKOD='6300504080108010103'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yazılım Desteği', HESAPKOD='6300504809008010104' WHERE YIL=2021 AND HESAPKOD='6300504080108010104'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Operasyonel Giderler Desteği', HESAPKOD='63005048090080102' WHERE YIL=2021 AND HESAPKOD='63005040801080102'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kira Desteği', HESAPKOD='6300504809008010201' WHERE YIL=2021 AND HESAPKOD='6300504080108010201'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='İşletim Desteği', HESAPKOD='6300504809008010202' WHERE YIL=2021 AND HESAPKOD='6300504080108010202'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Personel Desteği', HESAPKOD='6300504809008010203' WHERE YIL=2021 AND HESAPKOD='6300504080108010203'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Ulaşım Desteği', HESAPKOD='6300504809008010204' WHERE YIL=2021 AND HESAPKOD='6300504080108010204'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Tanıtım Desteği', HESAPKOD='6300504809008010205' WHERE YIL=2021 AND HESAPKOD='6300504080108010205'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Eğitim, Danışmanlık, Mentörlük, İş Yönetimi, Hukuk, Fikri ve Sınai Mülkiyet Hakları Desteği', HESAPKOD='6300504809008010206' WHERE YIL=2021 AND HESAPKOD='6300504080108010206'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yeminli Mali Müşavirlik Desteği', HESAPKOD='6300504809008010207' WHERE YIL=2021 AND HESAPKOD='6300504080108010207'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Uluslararası Hızlandırıcı Programı', HESAPKOD='630050480900802' WHERE YIL=2021 AND HESAPKOD='630050408010802'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Hızlandırıcı Ofis Kirası Desteği', HESAPKOD='63005048090080201' WHERE YIL=2021 AND HESAPKOD='63005040801080201'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Eğitim Desteği', HESAPKOD='63005048090080202' WHERE YIL=2021 AND HESAPKOD='63005040801080202'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Danışmanlık Desteği', HESAPKOD='63005048090080203' WHERE YIL=2021 AND HESAPKOD='63005040801080203'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Mentörlük Desteği', HESAPKOD='63005048090080204' WHERE YIL=2021 AND HESAPKOD='63005040801080204'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='İş Yönetimi, Hukuk, Fikri ve Sınai Mülkiyet Hakları Desteği', HESAPKOD='63005048090080205' WHERE YIL=2021 AND HESAPKOD='63005040801080205'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Ulaşım Desteği', HESAPKOD='63005048090080206' WHERE YIL=2021 AND HESAPKOD='63005040801080206'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Konaklama Desteği', HESAPKOD='63005048090080207' WHERE YIL=2021 AND HESAPKOD='63005040801080207'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yeminli Mali Müşavirlik Hizmetleri Desteği', HESAPKOD='63005048090080208' WHERE YIL=2021 AND HESAPKOD='63005040801080208'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Organizasyonla İlgili Genel Giderler Desteği', HESAPKOD='63005048090080209' WHERE YIL=2021 AND HESAPKOD='63005040801080209'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='TEMİNAT GİDERLERİ DESTEKLERİ', HESAPKOD='6300504809009' WHERE YIL=2021 AND HESAPKOD='6300504080109'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Teminat Giderleri Desteği', HESAPKOD='630050480900901' WHERE YIL=2021 AND HESAPKOD='630050408010901'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='KOBİ GELİŞİM (KOBİGEL) DESTEKLERİ', HESAPKOD='6300504809010' WHERE YIL=2021 AND HESAPKOD='6300504080110'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Personel Desteği', HESAPKOD='630050480901001' WHERE YIL=2021 AND HESAPKOD='630050408011001'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Makine-Teçhizat Desteği', HESAPKOD='630050480901002' WHERE YIL=2021 AND HESAPKOD='630050408011002'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yazılım Desteği', HESAPKOD='630050480901003' WHERE YIL=2021 AND HESAPKOD='630050408011003'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Hizmet Alımları Desteği', HESAPKOD='630050480901004' WHERE YIL=2021 AND HESAPKOD='630050408011004'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Giderler Desteği', HESAPKOD='630050480901005' WHERE YIL=2021 AND HESAPKOD='630050408011005'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='STRATEJİK ÜRÜN DESTEKLERİ', HESAPKOD='6300504809011' WHERE YIL=2021 AND HESAPKOD='6300504080111'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Makine-Teçhizat Desteği', HESAPKOD='630050480901101' WHERE YIL=2021 AND HESAPKOD='630050408011101'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yazılım Desteği', HESAPKOD='630050480901102' WHERE YIL=2021 AND HESAPKOD='630050408011102'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Personel Desteği', HESAPKOD='630050480901103' WHERE YIL=2021 AND HESAPKOD='630050408011103'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Bilgi Transferi Desteği', HESAPKOD='630050480901104' WHERE YIL=2021 AND HESAPKOD='630050408011104'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Test-Analiz, Kalibrasyon ve Referans Numune Desteği', HESAPKOD='630050480901105' WHERE YIL=2021 AND HESAPKOD='630050408011105'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Hizmet Alımı Desteği', HESAPKOD='630050480901106' WHERE YIL=2021 AND HESAPKOD='630050408011106'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Eğitim ve Danışmanlık Desteği', HESAPKOD='63005048090110601' WHERE YIL=2021 AND HESAPKOD='63005040801110601'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Tasarım Desteği', HESAPKOD='63005048090110602' WHERE YIL=2021 AND HESAPKOD='63005040801110602'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Hizmet Alımı Desteği', HESAPKOD='63005048090110603' WHERE YIL=2021 AND HESAPKOD='63005040801110603'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='KOBİ TEKNOLOJİK ÜRÜN YATIRIM (KOBİ TEKNOYATIRIM) DESTEKLERİ', HESAPKOD='6300504809012' WHERE YIL=2021 AND HESAPKOD='6300504080112'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yatırım Projesi Dönemi KOBİ TEKNOYATIRIM Desteği', HESAPKOD='630050480901201' WHERE YIL=2021 AND HESAPKOD='630050408011201'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Makine-Teçhizat ve Yazılım Desteği', HESAPKOD='63005048090120101' WHERE YIL=2021 AND HESAPKOD='63005040801120101'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Personel Desteği', HESAPKOD='63005048090120102' WHERE YIL=2021 AND HESAPKOD='63005040801120102'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Eğitim ve Danışmanlık Desteği', HESAPKOD='63005048090120103' WHERE YIL=2021 AND HESAPKOD='63005040801120103'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kira Desteği', HESAPKOD='63005048090120104' WHERE YIL=2021 AND HESAPKOD='63005040801120104'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yatırım Projesi Sonrası KOBİ TEKNOYATIRIM Desteği', HESAPKOD='630050480901202' WHERE YIL=2021 AND HESAPKOD='630050408011202'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Personel Desteği', HESAPKOD='63005048090120201' WHERE YIL=2021 AND HESAPKOD='63005040801120201'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Enerji Desteği', HESAPKOD='63005048090120202' WHERE YIL=2021 AND HESAPKOD='63005040801120202'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='İŞLETME GELİŞTİRME DESTEKLERİ', HESAPKOD='6300504809013' WHERE YIL=2021 AND HESAPKOD='6300504080113'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yurt İçi Fuar Desteği', HESAPKOD='630050480901301' WHERE YIL=2021 AND HESAPKOD='630050408011301'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='İhtisas Fuarı Desteği', HESAPKOD='63005048090130101' WHERE YIL=2021 AND HESAPKOD='63005040801130101'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Uluslararası İhtisas Fuarı Desteği', HESAPKOD='63005048090130102' WHERE YIL=2021 AND HESAPKOD='63005040801130102'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='İzmir Enternasyonal Fuarı Desteği', HESAPKOD='63005048090130103' WHERE YIL=2021 AND HESAPKOD='63005040801130103'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='IDEF Uluslararası Savunma Sanayi Fuarı', HESAPKOD='63005048090130104' WHERE YIL=2021 AND HESAPKOD='63005040801130104'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yurt Dışı İş Gezisi Desteği', HESAPKOD='630050480901302' WHERE YIL=2021 AND HESAPKOD='630050408011302'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Nitelikli Eleman İstihdam Desteği', HESAPKOD='630050480901303' WHERE YIL=2021 AND HESAPKOD='630050408011303'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Eğitim Desteği', HESAPKOD='630050480901304' WHERE YIL=2021 AND HESAPKOD='630050408011304'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Enerji Verimliliği Desteği', HESAPKOD='630050480901305' WHERE YIL=2021 AND HESAPKOD='630050408011305'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Ön Etüt Desteği', HESAPKOD='63005048090130501' WHERE YIL=2021 AND HESAPKOD='63005040801130501'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Detaylı Etüt Desteği', HESAPKOD='63005048090130502' WHERE YIL=2021 AND HESAPKOD='63005040801130502'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Verimlilik Arttırıcı Proje (VAP) İçin Danışmanlık Desteği', HESAPKOD='63005048090130503' WHERE YIL=2021 AND HESAPKOD='63005040801130503'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Enerji Yöneticisi Eğitimleri Desteği', HESAPKOD='63005048090130504' WHERE YIL=2021 AND HESAPKOD='63005040801130504'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Tasarım Desteği', HESAPKOD='630050480901306' WHERE YIL=2021 AND HESAPKOD='630050408011306'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sınai Mülkiyet Hakları Desteği', HESAPKOD='630050480901307' WHERE YIL=2021 AND HESAPKOD='630050408011307'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Patent Belgesi Desteği', HESAPKOD='63005048090130701' WHERE YIL=2021 AND HESAPKOD='63005040801130701'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Faydalı Model Belgesi Desteği', HESAPKOD='63005048090130702' WHERE YIL=2021 AND HESAPKOD='63005040801130702'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Endüstriyel Tasarım Tescili Belgesi Desteği', HESAPKOD='63005048090130703' WHERE YIL=2021 AND HESAPKOD='63005040801130703'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Entegre Devre Topografyaları Tescil Belgesi Desteği', HESAPKOD='63005048090130704' WHERE YIL=2021 AND HESAPKOD='63005040801130704'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yurt Dışı Marka Tescil Belgesi Desteği', HESAPKOD='63005048090130705' WHERE YIL=2021 AND HESAPKOD='63005040801130705'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Belgelendirme Desteği', HESAPKOD='630050480901308' WHERE YIL=2021 AND HESAPKOD='630050408011308'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Test ve Analiz Desteği', HESAPKOD='630050480901309' WHERE YIL=2021 AND HESAPKOD='630050408011309'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Enerji Verimli Elektrik Motorları Değişimi Desteği', HESAPKOD='630050480901310' WHERE YIL=2021 AND HESAPKOD='630050408011310'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Bağımsız Değerlendirme Desteği', HESAPKOD='630050480901311' WHERE YIL=2021 AND HESAPKOD='630050408011311'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Model Fabrika Desteği', HESAPKOD='630050480901312' WHERE YIL=2021 AND HESAPKOD='630050408011312'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yurt Dışı Pazar Destek Programı', HESAPKOD='6300504809014' WHERE YIL=2021 AND HESAPKOD='6300504080114'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Personel Gideri Desteği', HESAPKOD='630050480901401' WHERE YIL=2021 AND HESAPKOD='630050408011401'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yazılım ve Donanım Giderleri Desteği', HESAPKOD='630050480901402' WHERE YIL=2021 AND HESAPKOD='630050408011402'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Tanıtım Giderleri Desteği', HESAPKOD='630050480901403' WHERE YIL=2021 AND HESAPKOD='630050408011403'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yurt Dışı Fuar ve Seyahat Giderleri Desteği', HESAPKOD='630050480901404' WHERE YIL=2021 AND HESAPKOD='630050408011404'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Test, Analiz ve Belgelendirme Giderleri Desteği', HESAPKOD='630050480901405' WHERE YIL=2021 AND HESAPKOD='630050408011405'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Hizmet Alım Giderleri Desteği', HESAPKOD='630050480901406' WHERE YIL=2021 AND HESAPKOD='630050408011406'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='GİRİŞİMCİLİĞİ GELİŞTİRME DESTEKLERİ', HESAPKOD='6300504809015' WHERE YIL=2021 AND HESAPKOD='6300504080115'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Girişimcilik Eğitimi Desteği', HESAPKOD='630050480901501' WHERE YIL=2021 AND HESAPKOD='630050408011501'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yeni Girişimci Programı Desteği', HESAPKOD='630050480901502' WHERE YIL=2021 AND HESAPKOD='630050408011502'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Geleneksel Girişimci Programı Desteği', HESAPKOD='63005048090150201' WHERE YIL=2021 AND HESAPKOD='63005040801150201'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kuruluş Desteği', HESAPKOD='6300504809015020101' WHERE YIL=2021 AND HESAPKOD='6300504080115020101'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Performans Desteği', HESAPKOD='6300504809015020102' WHERE YIL=2021 AND HESAPKOD='6300504080115020102'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='İleri Girişimci Programı Desteği', HESAPKOD='63005048090150202' WHERE YIL=2021 AND HESAPKOD='63005040801150202'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kuruluş Desteği', HESAPKOD='6300504809015020201' WHERE YIL=2021 AND HESAPKOD='6300504080115020201'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Performans Desteği', HESAPKOD='6300504809015020202' WHERE YIL=2021 AND HESAPKOD='6300504080115020202'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Makine, Teçhizat ve Yazılım Desteği', HESAPKOD='6300504809015020203' WHERE YIL=2021 AND HESAPKOD='6300504080115020203'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Mentörlük, Danışmanlık ve İşletme Koçluğu Desteği', HESAPKOD='6300504809015020204' WHERE YIL=2021 AND HESAPKOD='6300504080115020204'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='İŞGEM Programı', HESAPKOD='630050480901503' WHERE YIL=2021 AND HESAPKOD='630050408011503'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='İŞGEM Programı Mobilya ve Donanım Desteği', HESAPKOD='63005048090150301' WHERE YIL=2021 AND HESAPKOD='63005040801150301'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='İŞGEM Programı Ortak Kullanıma Yönelik Makine-Teçhizat ve Yazılım Desteği', HESAPKOD='63005048090150302' WHERE YIL=2021 AND HESAPKOD='63005040801150302'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='İŞGEM Programı Personel Giderleri Desteği', HESAPKOD='63005048090150303' WHERE YIL=2021 AND HESAPKOD='63005040801150303'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='İŞGEM Programı Eğitim, Danışmanlık, Organizasyon ve Tanıtım Giderleri Desteği', HESAPKOD='63005048090150304' WHERE YIL=2021 AND HESAPKOD='63005040801150304'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='İŞGEM Programı Eğitim ve Danışmanlık Desteği', HESAPKOD='6300504809015030401' WHERE YIL=2021 AND HESAPKOD='6300504080115030401'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='İŞGEM Programı Organizasyon Desteği', HESAPKOD='6300504809015030402' WHERE YIL=2021 AND HESAPKOD='6300504080115030402'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='İŞGEM Programı Tanıtım Desteği', HESAPKOD='6300504809015030403' WHERE YIL=2021 AND HESAPKOD='6300504080115030403'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='TEKMER Programı', HESAPKOD='630050480901504' WHERE YIL=2021 AND HESAPKOD='630050408011504'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='TEKMER Programı Mobilya ve Donanım Desteği', HESAPKOD='63005048090150401' WHERE YIL=2021 AND HESAPKOD='63005040801150401'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='TEKMER Programı Ortak Kullanıma Yönelik Makine-Teçhizat ve Yazılım Desteği', HESAPKOD='63005048090150402' WHERE YIL=2021 AND HESAPKOD='63005040801150402'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='TEKMER Programı Personel Giderleri Desteği', HESAPKOD='63005048090150403' WHERE YIL=2021 AND HESAPKOD='63005040801150403'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='TEKMER Programı Eğitim, Danışmanlık, Organizasyon ve Tanıtım Giderleri Desteği', HESAPKOD='63005048090150404' WHERE YIL=2021 AND HESAPKOD='63005040801150404'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='TEKMER Programı Eğitim ve Danışmanlık Desteği', HESAPKOD='6300504809015040401' WHERE YIL=2021 AND HESAPKOD='6300504080115040401'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='TEKMER Programı Organizasyon Desteği', HESAPKOD='6300504809015040402' WHERE YIL=2021 AND HESAPKOD='6300504080115040402'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='TEKMER Programı Tanıtım Desteği', HESAPKOD='6300504809015040403' WHERE YIL=2021 AND HESAPKOD='6300504080115040403'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='İŞBİRLİĞİ DESTEKLERİ', HESAPKOD='6300504809016' WHERE YIL=2021 AND HESAPKOD='6300504080116'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Personel Gideri Desteği', HESAPKOD='630050480901601' WHERE YIL=2021 AND HESAPKOD='630050408011601'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='İş Yeri Kirası Giderleri Desteği', HESAPKOD='630050480901602' WHERE YIL=2021 AND HESAPKOD='630050408011602'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Makine, Teçhizat ve Kalıp Giderleri Desteği', HESAPKOD='630050480901603' WHERE YIL=2021 AND HESAPKOD='630050408011603'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yazılım Giderleri Desteği', HESAPKOD='630050480901604' WHERE YIL=2021 AND HESAPKOD='630050408011604'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Hammadde ve Malzeme Giderleri Desteği', HESAPKOD='630050480901605' WHERE YIL=2021 AND HESAPKOD='630050408011605'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Hizmet Alımı Giderleri Desteği', HESAPKOD='630050480901606' WHERE YIL=2021 AND HESAPKOD='630050408011606'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Hukuki Danışmanlık Giderleri Desteği', HESAPKOD='63005048090160601' WHERE YIL=2021 AND HESAPKOD='63005040801160601'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Fizibilite Çalışması Giderleri Desteği', HESAPKOD='63005048090160602' WHERE YIL=2021 AND HESAPKOD='63005040801160602'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Hizmet Alımı Giderleri Desteği', HESAPKOD='63005048090160603' WHERE YIL=2021 AND HESAPKOD='63005040801160603'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Proje Giderleri', HESAPKOD='630050480901607' WHERE YIL=2021 AND HESAPKOD='630050408011607'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='GELENEKSEL GİRİŞİMCİ DESTEKLERİ', HESAPKOD='6300504809017' WHERE YIL=2021 AND HESAPKOD='6300504080117'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kuruluş Desteği', HESAPKOD='630050480901701' WHERE YIL=2021 AND HESAPKOD='630050408011701'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Performans Desteği', HESAPKOD='630050480901702' WHERE YIL=2021 AND HESAPKOD='630050408011702'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sertifika Desteği', HESAPKOD='630050480901703' WHERE YIL=2021 AND HESAPKOD='630050408011703'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='İLERİ GİRİŞİMCİ DESTEKLERİ', HESAPKOD='6300504809018' WHERE YIL=2021 AND HESAPKOD='6300504080118'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kuruluş Desteği', HESAPKOD='630050480901801' WHERE YIL=2021 AND HESAPKOD='630050408011801'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Performans Desteği', HESAPKOD='630050480901802' WHERE YIL=2021 AND HESAPKOD='630050408011802'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Makine, Teçhizat ve Yazılım Desteği', HESAPKOD='630050480901803' WHERE YIL=2021 AND HESAPKOD='630050408011803'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Mentörlük, Danışmanlık ve İşletme Koçluğu Desteği', HESAPKOD='630050480901804' WHERE YIL=2021 AND HESAPKOD='630050408011804'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sertifika Desteği', HESAPKOD='630050480901805' WHERE YIL=2021 AND HESAPKOD='630050408011805'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='İŞGEM/TEKMER DESTEKLERİ', HESAPKOD='6300504809019' WHERE YIL=2021 AND HESAPKOD='6300504080119'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='İŞGEM Programı', HESAPKOD='630050480901901' WHERE YIL=2021 AND HESAPKOD='630050408011901'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='İŞGEM Programı Mobilya ve Donanım Desteği', HESAPKOD='63005048090190101' WHERE YIL=2021 AND HESAPKOD='63005040801190101'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='İŞGEM Programı Ortak Kullanıma Yönelik Makine-Teçhizat ve Yazılım Desteği', HESAPKOD='63005048090190102' WHERE YIL=2021 AND HESAPKOD='63005040801190102'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='İŞGEM Programı Personel Giderleri Desteği', HESAPKOD='63005048090190103' WHERE YIL=2021 AND HESAPKOD='63005040801190103'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='İŞGEM Programı Eğitim, Danışmanlık, Organizasyon ve Tanıtım Giderleri Desteği', HESAPKOD='63005048090190104' WHERE YIL=2021 AND HESAPKOD='63005040801190104'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='İŞGEM Programı Eğitim ve Danışmanlık Desteği', HESAPKOD='6300504809019010401' WHERE YIL=2021 AND HESAPKOD='6300504080119010401'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='İŞGEM Programı Organizasyon Desteği', HESAPKOD='6300504809019010402' WHERE YIL=2021 AND HESAPKOD='6300504080119010402'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='İŞGEM Programı Tanıtım Desteği', HESAPKOD='6300504809019010403' WHERE YIL=2021 AND HESAPKOD='6300504080119010403'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='TEKMER Programı', HESAPKOD='630050480901902' WHERE YIL=2021 AND HESAPKOD='630050408011902'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='TEKMER Programı Mobilya ve Donanım Desteği', HESAPKOD='63005048090190201' WHERE YIL=2021 AND HESAPKOD='63005040801190201'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='TEKMER Programı Ortak Kullanıma Yönelik Makine-Teçhizat ve Yazılım Desteği', HESAPKOD='63005048090190202' WHERE YIL=2021 AND HESAPKOD='63005040801190202'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='TEKMER Programı Personel Giderleri Desteği', HESAPKOD='63005048090190203' WHERE YIL=2021 AND HESAPKOD='63005040801190203'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='TEKMER Programı Eğitim, Danışmanlık, Organizasyon ve Tanıtım Giderleri Desteği', HESAPKOD='63005048090190204' WHERE YIL=2021 AND HESAPKOD='63005040801190204'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='TEKMER Programı Eğitim ve Danışmanlık Desteği', HESAPKOD='6300504809019020401' WHERE YIL=2021 AND HESAPKOD='6300504080119020401'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='TEKMER Programı Organizasyon Desteği', HESAPKOD='6300504809019020402' WHERE YIL=2021 AND HESAPKOD='6300504080119020402'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='TEKMER Programı Tanıtım Desteği', HESAPKOD='6300504809019020403' WHERE YIL=2021 AND HESAPKOD='6300504080119020403'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='İŞ PLANI ÖDÜLÜ DESTEKLERİ', HESAPKOD='6300504809020' WHERE YIL=2021 AND HESAPKOD='6300504080120'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='İş Planı Ödülü Destek Programı', HESAPKOD='630050480902001' WHERE YIL=2021 AND HESAPKOD='630050408012001'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='TEMATİK PROJE DESTEKLERİ', HESAPKOD='6300504809021' WHERE YIL=2021 AND HESAPKOD='6300504080121'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Meslek Kuruluşu Proje Destek Programı Destekleri', HESAPKOD='630050480902101' WHERE YIL=2021 AND HESAPKOD='630050408012101'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='KOBİ PROJE DESTEKLERİ', HESAPKOD='6300504809022' WHERE YIL=2021 AND HESAPKOD='6300504080122'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Personel Giderleri', HESAPKOD='630050480902201' WHERE YIL=2021 AND HESAPKOD='630050408012201'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Makine-Teçhizat, Hammadde Malzeme Giderleri', HESAPKOD='630050480902202' WHERE YIL=2021 AND HESAPKOD='630050408012202'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Hizmet Alımı Giderleri', HESAPKOD='630050480902203' WHERE YIL=2021 AND HESAPKOD='630050408012203'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yazılım Alımı Giderleri', HESAPKOD='630050480902204' WHERE YIL=2021 AND HESAPKOD='630050408012204'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Giderler', HESAPKOD='630050480902205' WHERE YIL=2021 AND HESAPKOD='630050408012205'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='FRIT-1-KOBİ’ler ve Yeni Girişimciler için Küçük Hibeler', HESAPKOD='6300504809090' WHERE YIL=2021 AND HESAPKOD='6300504080190'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='FRIT-1- Proje Destek Programı', HESAPKOD='630050480909001' WHERE YIL=2021 AND HESAPKOD='630050408019001'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='FRIT-1- Girişimcilik Programı', HESAPKOD='630050480909002' WHERE YIL=2021 AND HESAPKOD='630050408019002'
DELETE FROM  MUHHESAPPLANI WHERE YIL=2021 AND HESAPKOD='63005040810'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Hane Halkına ve İşletmelere Yapılan Diğer Transferler', HESAPKOD='630050490' WHERE YIL=2021 AND HESAPKOD='630050409'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sınıflandırmaya Girmeyen Hane Halkı ve İşletmelere Yapılan Diğer Transferler', HESAPKOD='63005049090' WHERE YIL=2021 AND HESAPKOD='63005040901'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Türk Kültür Varlığının Korunması ve Tanıtımı Giderleri', HESAPKOD='63005049001' WHERE YIL=2021 AND HESAPKOD='63005040908'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Siyasi Tanıtma Giderleri', HESAPKOD='63005049002' WHERE YIL=2021 AND HESAPKOD='63005040909'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sağlık Personelinin Mali Sorumluluk Sigortası Giderleri ', HESAPKOD='63005049003' WHERE YIL=2021 AND HESAPKOD='63005040910'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yurtdışına Yapılan Transferler', HESAPKOD='6300506' WHERE YIL=2021 AND HESAPKOD='6300506'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Dış Ülkelere Yapılan Yardımlar', HESAPKOD='630050610' WHERE YIL=2021 AND HESAPKOD='630050601'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kuzey Kıbrıs Türk Cumhuriyeti ne', HESAPKOD='63005061001' WHERE YIL=2021 AND HESAPKOD='63005060101'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Ülkelere', HESAPKOD='63005061090' WHERE YIL=2021 AND HESAPKOD='63005060190'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Uluslararası Kuruluşlara Yapılan Ödemeler', HESAPKOD='630050620' WHERE YIL=2021 AND HESAPKOD='630050602'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Uluslararası Kuruluşlara Üyelik Aidatı Ödemeleri', HESAPKOD='63005062001' WHERE YIL=2021 AND HESAPKOD='63005060201'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Uluslararası Kuruluşlara Katkı Ödemeleri', HESAPKOD='63005062002' WHERE YIL=2021 AND HESAPKOD='63005060202'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Uluslararası Kuruluşlara Yardım Ödemeleri', HESAPKOD='63005062003' WHERE YIL=2021 AND HESAPKOD='63005060203'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Uluslararası Kuruluşlara Diğer Ödemeler', HESAPKOD='63005062090' WHERE YIL=2021 AND HESAPKOD='63005060290'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Uluslarüstü Kuruluşlara', HESAPKOD='630050630' WHERE YIL=2021 AND HESAPKOD='630050603'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Uluslarüstü Kuruluşlara', HESAPKOD='63005063001' WHERE YIL=2021 AND HESAPKOD='63005060301'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Uluslarüstü Kuruluşlardan Merkezlerine ', HESAPKOD='630050640' WHERE YIL=2021 AND HESAPKOD='630050604'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Uluslarüstü Kuruluşlardan Merkezlerine', HESAPKOD='63005064001' WHERE YIL=2021 AND HESAPKOD='63005060401'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Eğitim Kurumlarına', HESAPKOD='630050650' WHERE YIL=2021 AND HESAPKOD='630050605'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Hoca Ahmet Yesevi Uluslararası Türk Kazak Üniversitesine', HESAPKOD='63005065001' WHERE YIL=2021 AND HESAPKOD='63005060501'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kırgızistan-Türkiye Manas Üniversitesine', HESAPKOD='63005065002' WHERE YIL=2021 AND HESAPKOD='63005060502'
DELETE FROM  MUHHESAPPLANI WHERE YIL=2021 AND HESAPKOD='63005060503'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yurtdışına Yapılan Diğer Transferler', HESAPKOD='630050690' WHERE YIL=2021 AND HESAPKOD='630050609'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sınıflandırmaya Girmeyen Diğer Yurtdışı Transferler', HESAPKOD='63005069090' WHERE YIL=2021 AND HESAPKOD='63005060901'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Türk Kültür Varlığının Korunması ve Tanıtımı Giderleri', HESAPKOD='63005069001' WHERE YIL=2021 AND HESAPKOD='63005060908'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Siyasi Tanıtma Giderleri', HESAPKOD='63005069002' WHERE YIL=2021 AND HESAPKOD='63005060909'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Gelirlerden Ayrılan Paylar', HESAPKOD='6300508' WHERE YIL=2021 AND HESAPKOD='6300508'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Genel Bütçeye Verilen Paylar', HESAPKOD='630050810' WHERE YIL=2021 AND HESAPKOD='630050801'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Genel Bütçeye Verilen Paylar', HESAPKOD='63005081001' WHERE YIL=2021 AND HESAPKOD='63005080101'
DELETE FROM  MUHHESAPPLANI WHERE YIL=2021 AND HESAPKOD='63005080102'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Evrensel Hizmet Katkı Payı', HESAPKOD='63005081002' WHERE YIL=2021 AND HESAPKOD='63005080103'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Elektronik Haberleşme Sektörüne İlişkin Ar-Ge Payı', HESAPKOD='63005081003' WHERE YIL=2021 AND HESAPKOD='63005080104'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Özel Bütçeli İdarelere Verilen Paylar', HESAPKOD='630050820' WHERE YIL=2021 AND HESAPKOD='630050802'
DELETE FROM  MUHHESAPPLANI WHERE YIL=2021 AND HESAPKOD='63005080201'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yükseköğrenim Kredi Yurtlar Kurumuna Verilen Paylar', HESAPKOD='63005082001' WHERE YIL=2021 AND HESAPKOD='63005080202'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Türkiye Akreditasyon Kurumuna Verilen Paylar', HESAPKOD='63005082002' WHERE YIL=2021 AND HESAPKOD='63005080203'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Türkiye Uzay Ajansına Verilen Paylar', HESAPKOD='63005082003' WHERE YIL=2021 AND HESAPKOD='63005080204'
DELETE FROM  MUHHESAPPLANI WHERE YIL=2021 AND HESAPKOD='63005080290'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Düzenleyici ve Denetleyici Kurumlara Verilen Paylar', HESAPKOD='630050830' WHERE YIL=2021 AND HESAPKOD='630050803'
DELETE FROM  MUHHESAPPLANI WHERE YIL=2021 AND HESAPKOD='63005080301'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Enerji Piyasası Düzenleme Kurumuna Verilen Paylar ', HESAPKOD='63005083001' WHERE YIL=2021 AND HESAPKOD='63005080302'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sosyal Güvenlik Kurumlarına Verilen Paylar', HESAPKOD='630050840' WHERE YIL=2021 AND HESAPKOD='630050804'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sosyal Güvenlik Kurumlarına Verilen Paylar', HESAPKOD='63005084001' WHERE YIL=2021 AND HESAPKOD='63005080401'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Mahalli İdarelere Verilen Paylar', HESAPKOD='630050850' WHERE YIL=2021 AND HESAPKOD='630050805'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Genel Bütçe Gelirlerinden İl Özel İdarelerine Ayrılan Paylar', HESAPKOD='63005085001' WHERE YIL=2021 AND HESAPKOD='63005080501'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Genel Bütçe Gelirlerinden Büyükşehir Belediyelere Ayrılan Paylar', HESAPKOD='63005085002' WHERE YIL=2021 AND HESAPKOD='63005080502'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Genel Bütçe Gelirlerinden Diğer Belediyelere Ayrılan Paylar', HESAPKOD='63005085003' WHERE YIL=2021 AND HESAPKOD='63005080503'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Genel Bütçe Gelirlerinden Büyükşehir İlçe Belediyelerine Ayrılan Paylar', HESAPKOD='63005085004' WHERE YIL=2021 AND HESAPKOD='63005080504'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Fonlara Verilen Paylar', HESAPKOD='630050860' WHERE YIL=2021 AND HESAPKOD='630050806'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Savunma Sanayi Destekleme Fonuna Verilen Paylar', HESAPKOD='63005086001' WHERE YIL=2021 AND HESAPKOD='63005080601'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sosyal Yardımlaşma ve Dayanışmayı Teşvik Fonuna Verilen Paylar', HESAPKOD='63005086002' WHERE YIL=2021 AND HESAPKOD='63005080602'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sivil Savunma Fonuna Verilen Paylar ', HESAPKOD='63005086004' WHERE YIL=2021 AND HESAPKOD='63005080603'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Tanıtma Fonuna Verilen Paylar', HESAPKOD='63005086003' WHERE YIL=2021 AND HESAPKOD='63005080604'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Döner Sermayelere Verilen Paylar', HESAPKOD='630050870' WHERE YIL=2021 AND HESAPKOD='630050807'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Döner Sermayelere Verilen Paylar', HESAPKOD='63005087001' WHERE YIL=2021 AND HESAPKOD='63005080701'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Döner Sermayelere Verilen Paylar', HESAPKOD='63005087001' WHERE YIL=2021 AND HESAPKOD='63005080702'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğerlerine Verilen Paylar', HESAPKOD='630050890' WHERE YIL=2021 AND HESAPKOD='630050809'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Türkiye Barolar Birliğine Verilen Paylar', HESAPKOD='63005089001' WHERE YIL=2021 AND HESAPKOD='63005080901'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Türkiye Odalar ve Borsalar Birliğine Verilen Paylar', HESAPKOD='63005089002' WHERE YIL=2021 AND HESAPKOD='63005080902'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Türkiye Esnaf ve Sanatkarlar Konfederasyonuna Bağlı Odalara Verilen Paylar', HESAPKOD='63005089003' WHERE YIL=2021 AND HESAPKOD='63005080903'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Olimpiyat Oyunları Hazırlık ve Düzenleme Kuruluna Verilen Paylar', HESAPKOD='63005089004' WHERE YIL=2021 AND HESAPKOD='63005080904'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Toplu Konut İdaresi Başkanlığına Verilen Paylar', HESAPKOD='63005089006' WHERE YIL=2021 AND HESAPKOD='63005080905'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Şeker Kurumuna Verilen Paylar', HESAPKOD='63005089005' WHERE YIL=2021 AND HESAPKOD='63005080906'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sermaye Giderleri', HESAPKOD='63006' WHERE YIL=2021 AND HESAPKOD='63006'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Mamul Mal Alımları', HESAPKOD='6300601' WHERE YIL=2021 AND HESAPKOD='6300601'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Büro ve İşyeri Mefruşatı Alımları', HESAPKOD='630060110' WHERE YIL=2021 AND HESAPKOD='630060101'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Büro Mefruşatı Alımları', HESAPKOD='63006011001' WHERE YIL=2021 AND HESAPKOD='63006010101'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Büro ve İşyeri Makine Teçhizat Alımları', HESAPKOD='630060120' WHERE YIL=2021 AND HESAPKOD='630060102'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Büro Makinaları Alımları ', HESAPKOD='63006012001' WHERE YIL=2021 AND HESAPKOD='63006010201'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Bilgisayar Alımları', HESAPKOD='63006012002' WHERE YIL=2021 AND HESAPKOD='63006010202'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Makine Teçhizat Alımları', HESAPKOD='63006012090' WHERE YIL=2021 AND HESAPKOD='63006010290'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Taşıt Alımları', HESAPKOD='630060140' WHERE YIL=2021 AND HESAPKOD='630060104'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kara Taşıtı  Alımları   ', HESAPKOD='63006014001' WHERE YIL=2021 AND HESAPKOD='63006010401'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yayın Alımları ve Yapımları', HESAPKOD='630060160' WHERE YIL=2021 AND HESAPKOD='630060106'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Basılı Yayın Alımları ve Yapımları', HESAPKOD='63006016001' WHERE YIL=2021 AND HESAPKOD='63006010601'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Gayri Maddi Hak Alımları', HESAPKOD='6300603' WHERE YIL=2021 AND HESAPKOD='6300603'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Bilgisayar Yazılımı Alımları', HESAPKOD='630060310' WHERE YIL=2021 AND HESAPKOD='630060301'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Bilgisayar Yazılımı Alımları', HESAPKOD='63006031001' WHERE YIL=2021 AND HESAPKOD='63006030101'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Gayrimenkul Sermaye Üretim Giderleri', HESAPKOD='6300605' WHERE YIL=2021 AND HESAPKOD='6300605'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Müteahhitlik Giderleri', HESAPKOD='630060570' WHERE YIL=2021 AND HESAPKOD='630060507'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Hizmet Binası Yapım Giderleri', HESAPKOD='63006057001' WHERE YIL=2021 AND HESAPKOD='63006050701'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Gayrimenkul Büyük Onarım Giderleri', HESAPKOD='6300607' WHERE YIL=2021 AND HESAPKOD='6300607'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Müteahhitlik Giderleri', HESAPKOD='630060770' WHERE YIL=2021 AND HESAPKOD='630060707'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Hizmet Binası Büyük Onarım Giderleri', HESAPKOD='63006077001' WHERE YIL=2021 AND HESAPKOD='63006070701'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sermaye Transferleri', HESAPKOD='63007' WHERE YIL=2021 AND HESAPKOD='63007'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kurum, İşletme ve Hanehalkına Yapılan Sermaye Transferleri', HESAPKOD='6300701' WHERE YIL=2021 AND HESAPKOD='6300701'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Genel Bütçeye Hazine Yardımları', HESAPKOD='630070310' WHERE YIL=2021 AND HESAPKOD='630070101'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Genel Bütçeye Hazine Yardımları', HESAPKOD='63007031001' WHERE YIL=2021 AND HESAPKOD='63007010101'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Özel Bütçeli İdarelere Hazine Yardımları', HESAPKOD='630070320' WHERE YIL=2021 AND HESAPKOD='630070102'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Ölçme, Seçme ve Yerleştirme Merkezi Başkanlığı na Hazine Yardımı', HESAPKOD='63007032001' WHERE YIL=2021 AND HESAPKOD='63007010201'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Atatürk Kültür, Dil ve Tarih Yüksek Kurumu na Hazine Yardımı', HESAPKOD='63007032002' WHERE YIL=2021 AND HESAPKOD='63007010202'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Atatürk Araştırma Merkezi ne Hazine Yardımı', HESAPKOD='63007032003' WHERE YIL=2021 AND HESAPKOD='63007010203'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Atatürk Kültür Merkezi ne Hazine Yardımı', HESAPKOD='63007032004' WHERE YIL=2021 AND HESAPKOD='63007010204'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Türk Dil Kurumu na Hazine Yardımı', HESAPKOD='63007032005' WHERE YIL=2021 AND HESAPKOD='63007010205'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Türk Tarih Kurumu na Hazine Yardımı', HESAPKOD='63007032006' WHERE YIL=2021 AND HESAPKOD='63007010206'
DELETE FROM  MUHHESAPPLANI WHERE YIL=2021 AND HESAPKOD='63007010207'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Türkiye Bilimsel ve Teknolojik Araştırma Kurumu Başkanlığı na Hazine Yardımı', HESAPKOD='63007032007' WHERE YIL=2021 AND HESAPKOD='63007010208'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Türkiye Bilimler Akademisi Başkanlığı na Hazine Yardımı', HESAPKOD='63007032008' WHERE YIL=2021 AND HESAPKOD='63007010209'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Türkiye Adalet Akademisi’ne Hazine Yardımı', HESAPKOD='63007032009' WHERE YIL=2021 AND HESAPKOD='63007010210'
DELETE FROM  MUHHESAPPLANI WHERE YIL=2021 AND HESAPKOD='63007010213'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sınıflandırmaya Girmeyen Diğer Hazine Yardımları', HESAPKOD='63007039090' WHERE YIL=2021 AND HESAPKOD='63007010214'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Devlet Tiyatroları Genel Müdürlüğü ne Hazine Yardımı', HESAPKOD='63007032010' WHERE YIL=2021 AND HESAPKOD='63007010215'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Devlet Opera ve Balesi Genel Müdürlüğü ne Hazine Yardımı', HESAPKOD='63007032011' WHERE YIL=2021 AND HESAPKOD='63007010216'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Orman Genel Müdürlüğü ne Hazine Yardımı', HESAPKOD='63007032012' WHERE YIL=2021 AND HESAPKOD='63007010217'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Vakıflar Genel Müdürlüğü ne Hazine Yardımı', HESAPKOD='63007032013' WHERE YIL=2021 AND HESAPKOD='63007010218'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Türkiye Hudut ve Sahiller Sağlık Genel Müdürlüğü ne Hazine Yardımı', HESAPKOD='63007032014' WHERE YIL=2021 AND HESAPKOD='63007010219'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Türk Akreditasyon Kurumu na Hazine Yardımı', HESAPKOD='63007032015' WHERE YIL=2021 AND HESAPKOD='63007010221'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Türk Standartları Enstitüsü ne Hazine Yardımı', HESAPKOD='63007032016' WHERE YIL=2021 AND HESAPKOD='63007010222'
DELETE FROM  MUHHESAPPLANI WHERE YIL=2021 AND HESAPKOD='63007010223'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Türk Patent Enstitüsü ne Hazine Yardımı', HESAPKOD='63007032017' WHERE YIL=2021 AND HESAPKOD='63007010224'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Ulusal Bor Araştırma Enstitüsü ne Hazine Yardımı', HESAPKOD='63007032018' WHERE YIL=2021 AND HESAPKOD='63007010226'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Türkiye Atom Enerjisi Kurumu na Hazine Yardımı', HESAPKOD='63007032019' WHERE YIL=2021 AND HESAPKOD='63007010227'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Kamu Kurum ve Kuruluşlarına', HESAPKOD='63007019003' WHERE YIL=2021 AND HESAPKOD='63007010228'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Küçük ve Orta Ölçekli İşletmeleri Geliştirme ve Destekleme İdaresi Başkanlığı na Hazine Yardımı', HESAPKOD='63007032021' WHERE YIL=2021 AND HESAPKOD='63007010230'
DELETE FROM  MUHHESAPPLANI WHERE YIL=2021 AND HESAPKOD='63007010231'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Türk İşbirliği ve Koordinasyon Ajansı Başkanlığı na Hazine Yardımı', HESAPKOD='63007032022' WHERE YIL=2021 AND HESAPKOD='63007010232'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='GAP Bölge Kalkınma İdaresi Başkanlığı na Hazine Yardımı', HESAPKOD='63007032023' WHERE YIL=2021 AND HESAPKOD='63007010234'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Özelleştirme İdaresi Başkanlığı na Hazine Yardımı', HESAPKOD='63007032024' WHERE YIL=2021 AND HESAPKOD='63007010235'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Maden Tetkik ve Arama Genel Müdürlüğü ne Hazine Yardımı', HESAPKOD='63007032025' WHERE YIL=2021 AND HESAPKOD='63007010240'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Ceza İnfaz Kurumları ile Tutukevleri İş Yurtları Kurumu na  Hazine Yardımı', HESAPKOD='63007032026' WHERE YIL=2021 AND HESAPKOD='63007010241'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sivil Havacılık Genel Müdürlüğü ne Hazine Yardımı', HESAPKOD='63007032027' WHERE YIL=2021 AND HESAPKOD='63007010249'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yurtdışı Türkler ve Akraba Topluluklar Başkanlığına Hazine Yardımı', HESAPKOD='63007032029' WHERE YIL=2021 AND HESAPKOD='63007010251'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Karayolları Genel Müdürlüğü ne Hazine Yardımı', HESAPKOD='63007032030' WHERE YIL=2021 AND HESAPKOD='63007010252'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Türkiye Yazma Eserler Kurumu Başkanlığı na Hazine Yardımı', HESAPKOD='63007032031' WHERE YIL=2021 AND HESAPKOD='63007010253'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Doğu Anadolu Projesi Bölge Kalkınma İdaresi Başkanlığı na Hazine Yardımı', HESAPKOD='63007032032' WHERE YIL=2021 AND HESAPKOD='63007010254'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Konya Ovası Projesi Bölge Kalkınma İdaresi Başkanlığı na Hazine Yardımı', HESAPKOD='63007032033' WHERE YIL=2021 AND HESAPKOD='63007010255'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Doğu Karadeniz Projesi Bölge Kalkınma İdaresi Başkanlığı na Hazine Yardımı', HESAPKOD='63007032034' WHERE YIL=2021 AND HESAPKOD='63007010256'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Devlet Su İşleri Genel Müdürlüğü ne Hazine Yardımı', HESAPKOD='63007032035' WHERE YIL=2021 AND HESAPKOD='63007010257'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Türkiye Su Enstitüsü ne Hazine Yardımı', HESAPKOD='63007032036' WHERE YIL=2021 AND HESAPKOD='63007010258'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Türkiye İlaç ve Tıbbi Cihaz Kurumu na Hazine Yardımı', HESAPKOD='63007032037' WHERE YIL=2021 AND HESAPKOD='63007010259'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kamu Denetçiliği Kurumu na Hazine Yardımı', HESAPKOD='63007032038' WHERE YIL=2021 AND HESAPKOD='63007010260'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Türkiye İnsan Hakları ve Eşitlik Kurumu na Hazine Yardımı', HESAPKOD='63007032039' WHERE YIL=2021 AND HESAPKOD='63007010261'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Türkiye Sağlık Enstitüleri Başkanlığı na Hazine Yardımı', HESAPKOD='63007032040' WHERE YIL=2021 AND HESAPKOD='63007010262'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Helal Akreditasyon Kurumu na Hazine Yardımı', HESAPKOD='63007032041' WHERE YIL=2021 AND HESAPKOD='63007010263'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Nadir Toprak Elementleri Araştırma Enstitüsü ne Hazine Yardımı', HESAPKOD='63007032042' WHERE YIL=2021 AND HESAPKOD='63007010264'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Maden ve Petrol İşleri Genel Müdürlüğü ne Hazine Yardımı', HESAPKOD='63007032043' WHERE YIL=2021 AND HESAPKOD='63007010265'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Türkiye Uzay Ajansı na Hazine Yardımı', HESAPKOD='63007032044' WHERE YIL=2021 AND HESAPKOD='63007010266'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Düzenleyici ve Denetleyici Kurumlara Hazine Yardımları', HESAPKOD='630070330' WHERE YIL=2021 AND HESAPKOD='630070103'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Radyo ve Televizyon Üst Kurulu na Hazine Yardımı', HESAPKOD='63007033001' WHERE YIL=2021 AND HESAPKOD='63007010301'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Bilgi Teknolojileri ve İletişim Kurumu na Hazine Yardımı', HESAPKOD='63007033002' WHERE YIL=2021 AND HESAPKOD='63007010302'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sermaye Piyasası Kurulu na Hazine Yardımı', HESAPKOD='63007033003' WHERE YIL=2021 AND HESAPKOD='63007010303'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Bankacılık Düzenleme ve Denetleme Kurumu na Hazine Yardımı', HESAPKOD='63007033004' WHERE YIL=2021 AND HESAPKOD='63007010304'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Enerji Piyasası Düzenleme Kurumu na Hazine Yardımı', HESAPKOD='63007033005' WHERE YIL=2021 AND HESAPKOD='63007010305'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kamu İhale Kurumu na Hazine Yardımı', HESAPKOD='63007033006' WHERE YIL=2021 AND HESAPKOD='63007010306'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Rekabet Kurumu na Hazine Yardımı', HESAPKOD='63007033007' WHERE YIL=2021 AND HESAPKOD='63007010307'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kamu Gözetimi, Muhasebe ve Denetim Standartları Kurumu na Hazine Yardımı', HESAPKOD='63007033008' WHERE YIL=2021 AND HESAPKOD='63007010310'
DELETE FROM  MUHHESAPPLANI WHERE YIL=2021 AND HESAPKOD='63007010309'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kişisel Verileri Koruma Kurumu na Hazine Yardımı', HESAPKOD='63007033009' WHERE YIL=2021 AND HESAPKOD='63007010311'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Nükleer Düzenleme Kurumu na Hazine Yardımı', HESAPKOD='63007033010' WHERE YIL=2021 AND HESAPKOD='63007010312'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sosyal Güvenlik Kurumlarına Hazine Yardımları', HESAPKOD='630070340' WHERE YIL=2021 AND HESAPKOD='630070104'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Türkiye İş Kurumu Genel Müdürlüğü ne', HESAPKOD='63007034001' WHERE YIL=2021 AND HESAPKOD='63007010404'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sosyal Güvenlik Kurumu na', HESAPKOD='63007034002' WHERE YIL=2021 AND HESAPKOD='63007010406'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Mahalli İdarelere Hazine Yardımları', HESAPKOD='630070350' WHERE YIL=2021 AND HESAPKOD='630070105'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='İl Özel İdarelerine', HESAPKOD='63007035001' WHERE YIL=2021 AND HESAPKOD='63007010501'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Belediyelere', HESAPKOD='63007035002' WHERE YIL=2021 AND HESAPKOD='63007010502'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Köylere', HESAPKOD='63007035003' WHERE YIL=2021 AND HESAPKOD='63007010503'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Mahalli İdare Birliklerine', HESAPKOD='63007035004' WHERE YIL=2021 AND HESAPKOD='63007010504'
DELETE FROM  MUHHESAPPLANI WHERE YIL=2021 AND HESAPKOD='63007010512'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kamu Teşebbüslerine Sermaye Transferleri', HESAPKOD='630070110' WHERE YIL=2021 AND HESAPKOD='630070106'
DELETE FROM  MUHHESAPPLANI WHERE YIL=2021 AND HESAPKOD='63007010601'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='TC Devlet Demiryolları na', HESAPKOD='63007011001' WHERE YIL=2021 AND HESAPKOD='63007010602'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Tarım İşletmelerine', HESAPKOD='63007011002' WHERE YIL=2021 AND HESAPKOD='63007010603'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Toprak Mahsulleri Ofisi ne', HESAPKOD='63007011003' WHERE YIL=2021 AND HESAPKOD='63007010604'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Teşebbüslere', HESAPKOD='63007011090' WHERE YIL=2021 AND HESAPKOD='63007010605'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Çay İşletmeleri Genel Müdürlüğü ne', HESAPKOD='63007011004' WHERE YIL=2021 AND HESAPKOD='63007010606'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Türkiye Elektrik Dağıtım AŞ ye', HESAPKOD='63007011005' WHERE YIL=2021 AND HESAPKOD='63007010607'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Elektrik Üretim AŞ ye', HESAPKOD='63007011006' WHERE YIL=2021 AND HESAPKOD='63007010608'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Elektrik İletim AŞ ye', HESAPKOD='63007011007' WHERE YIL=2021 AND HESAPKOD='63007010609'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Türkiye Elektrik ve Taahhüt AŞ ye', HESAPKOD='63007011008' WHERE YIL=2021 AND HESAPKOD='63007010610'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Türkiye Taşkömürü Kurumu na', HESAPKOD='63007011009' WHERE YIL=2021 AND HESAPKOD='63007010611'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Üretici Birliklerine', HESAPKOD='63007011010' WHERE YIL=2021 AND HESAPKOD='63007010612'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Tarım Kredi Kooperatiflerine', HESAPKOD='63007011011' WHERE YIL=2021 AND HESAPKOD='63007010613'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Türkiye Şeker Fabrikaları AŞ ye', HESAPKOD='63007011012' WHERE YIL=2021 AND HESAPKOD='63007010614'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sümer Holding AŞ ye', HESAPKOD='63007011013' WHERE YIL=2021 AND HESAPKOD='63007010615'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Makina ve Kimya Endüstrisi Kurumu na', HESAPKOD='63007011014' WHERE YIL=2021 AND HESAPKOD='63007010616'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Teşebbüslere', HESAPKOD='63007011090' WHERE YIL=2021 AND HESAPKOD='63007010617'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Et ve Süt Kurumu Genel Müdürlüğü ne', HESAPKOD='63007011015' WHERE YIL=2021 AND HESAPKOD='63007010619'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Türkiye Petrolleri Anonim Ortaklığı’na', HESAPKOD='63007011016' WHERE YIL=2021 AND HESAPKOD='63007010623'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Teşebbüslere', HESAPKOD='63007011090' WHERE YIL=2021 AND HESAPKOD='63007010649'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Darphane ve Damga Matbaası Genel Müdürlüğü ne', HESAPKOD='63007012001' WHERE YIL=2021 AND HESAPKOD='63007010650'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Döner Sermaye İşletmelerine', HESAPKOD='63007012090' WHERE YIL=2021 AND HESAPKOD='63007010659'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Ziraat Bankasına', HESAPKOD='63007013001' WHERE YIL=2021 AND HESAPKOD='63007010670'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Türkiye Kalkınma ve Yatırım Bankasına', HESAPKOD='63007013002' WHERE YIL=2021 AND HESAPKOD='63007010671'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Türk Eximbanka', HESAPKOD='63007013003' WHERE YIL=2021 AND HESAPKOD='63007010672'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Mali Kurumlara', HESAPKOD='63007013090' WHERE YIL=2021 AND HESAPKOD='63007010679'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Ulusal Fona', HESAPKOD='63007014080' WHERE YIL=2021 AND HESAPKOD='63007010680'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Fonlara', HESAPKOD='63007014090' WHERE YIL=2021 AND HESAPKOD='63007010689'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yükseköğretim Kurumlarına Hazine Yardımları', HESAPKOD='630070370' WHERE YIL=2021 AND HESAPKOD='630070107'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yükseköğretim Kurulu na Hazine Yardımı', HESAPKOD='63007037001' WHERE YIL=2021 AND HESAPKOD='63007010701'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Ankara Üniversitesi ne Hazine Yardımı', HESAPKOD='63007037002' WHERE YIL=2021 AND HESAPKOD='63007010702'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Ortadoğu Teknik Üniversitesi ne Hazine Yardımı', HESAPKOD='63007037003' WHERE YIL=2021 AND HESAPKOD='63007010703'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Hacettepe Üniversitesi ne Hazine Yardımı', HESAPKOD='63007037004' WHERE YIL=2021 AND HESAPKOD='63007010704'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Gazi Üniversitesi ne Hazine Yardımı', HESAPKOD='63007037005' WHERE YIL=2021 AND HESAPKOD='63007010705'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='İstanbul Üniversitesi ne Hazine Yardımı', HESAPKOD='63007037006' WHERE YIL=2021 AND HESAPKOD='63007010706'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='İstanbul Teknik Üniversitesi ne Hazine Yardımı', HESAPKOD='63007037007' WHERE YIL=2021 AND HESAPKOD='63007010707'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Boğaziçi Üniversitesi ne Hazine Yardımı', HESAPKOD='63007037008' WHERE YIL=2021 AND HESAPKOD='63007010708'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Marmara Üniversitesi ne Hazine Yardımı', HESAPKOD='63007037009' WHERE YIL=2021 AND HESAPKOD='63007010709'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yıldız Teknik Üniversitesi ne Hazine Yardımı', HESAPKOD='63007037010' WHERE YIL=2021 AND HESAPKOD='63007010710'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Mimar Sinan Güzel Sanatlar Üniversitesi ne Hazine Yardımı', HESAPKOD='63007037011' WHERE YIL=2021 AND HESAPKOD='63007010711'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Ege Üniversitesi ne Hazine Yardımı', HESAPKOD='63007037012' WHERE YIL=2021 AND HESAPKOD='63007010712'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Dokuz Eylül Üniversitesi ne Hazine Yardımı', HESAPKOD='63007037013' WHERE YIL=2021 AND HESAPKOD='63007010713'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Trakya Üniversitesi ne Hazine Yardımı', HESAPKOD='63007037014' WHERE YIL=2021 AND HESAPKOD='63007010714'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Bursa Uludağ Üniversitesi ne Hazine Yardımı', HESAPKOD='63007037015' WHERE YIL=2021 AND HESAPKOD='63007010715'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Anadolu Üniversitesi ne Hazine Yardımı', HESAPKOD='63007037016' WHERE YIL=2021 AND HESAPKOD='63007010716'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Selçuk Üniversitesi ne Hazine Yardımı', HESAPKOD='63007037017' WHERE YIL=2021 AND HESAPKOD='63007010717'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Akdeniz Üniversitesi ne Hazine Yardımı', HESAPKOD='63007037018' WHERE YIL=2021 AND HESAPKOD='63007010718'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Erciyes Üniversitesi ne Hazine Yardımı', HESAPKOD='63007037019' WHERE YIL=2021 AND HESAPKOD='63007010719'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sivas Cumhuriyet Üniversitesi ne Hazine Yardımı', HESAPKOD='63007037020' WHERE YIL=2021 AND HESAPKOD='63007010720'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Çukurova Üniversitesi ne Hazine Yardımı', HESAPKOD='63007037021' WHERE YIL=2021 AND HESAPKOD='63007010721'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Ondokuz Mayıs Üniversitesi ne Hazine Yardımı', HESAPKOD='63007037022' WHERE YIL=2021 AND HESAPKOD='63007010722'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Karadeniz Teknik Üniversitesi ne Hazine Yardımı', HESAPKOD='63007037023' WHERE YIL=2021 AND HESAPKOD='63007010723'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Atatürk Üniversitesi ne Hazine Yardımı', HESAPKOD='63007037024' WHERE YIL=2021 AND HESAPKOD='63007010724'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='İnönü Üniversitesi ne Hazine Yardımı', HESAPKOD='63007037025' WHERE YIL=2021 AND HESAPKOD='63007010725'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Fırat Üniversitesi ne Hazine Yardımı', HESAPKOD='63007037026' WHERE YIL=2021 AND HESAPKOD='63007010726'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Dicle Üniversitesi ne Hazine Yardımı', HESAPKOD='63007037027' WHERE YIL=2021 AND HESAPKOD='63007010727'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yüzüncüyıl Üniversitesi ne Hazine Yardımı', HESAPKOD='63007037028' WHERE YIL=2021 AND HESAPKOD='63007010728'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Gaziantep Üniversitesi ne Hazine Yardımı', HESAPKOD='63007037029' WHERE YIL=2021 AND HESAPKOD='63007010729'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='İzmir Yüksek Teknoloji Enstitüsü ne Hazine Yardımı', HESAPKOD='63007037030' WHERE YIL=2021 AND HESAPKOD='63007010730'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Gebze Teknik Üniversitesi ne Hazine Yardımı', HESAPKOD='63007037031' WHERE YIL=2021 AND HESAPKOD='63007010731'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Harran Üniversitesi ne Hazine Yardımı', HESAPKOD='63007037032' WHERE YIL=2021 AND HESAPKOD='63007010732'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Süleyman Demirel Üniversitesi ne Hazine Yardımı', HESAPKOD='63007037033' WHERE YIL=2021 AND HESAPKOD='63007010733'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Aydın Adnan Menderes Üniversitesi ne Hazine Yardımı', HESAPKOD='63007037034' WHERE YIL=2021 AND HESAPKOD='63007010734'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Zonguldak Bülent Ecevit Üniversitesi ne Hazine Yardımı', HESAPKOD='63007037035' WHERE YIL=2021 AND HESAPKOD='63007010735'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Mersin Üniversitesi ne Hazine Yardımı', HESAPKOD='63007037036' WHERE YIL=2021 AND HESAPKOD='63007010736'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Pamukkale Üniversitesi ne Hazine Yardımı', HESAPKOD='63007037037' WHERE YIL=2021 AND HESAPKOD='63007010737'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Balıkesir Üniversitesi ne Hazine Yardımı', HESAPKOD='63007037038' WHERE YIL=2021 AND HESAPKOD='63007010738'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kocaeli Üniversitesi ne Hazine Yardımı', HESAPKOD='63007037039' WHERE YIL=2021 AND HESAPKOD='63007010739'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sakarya Üniversitesi ne Hazine Yardımı', HESAPKOD='63007037040' WHERE YIL=2021 AND HESAPKOD='63007010740'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Manisa Celal Bayar Üniversitesi’ne Hazine Yardımı', HESAPKOD='63007037041' WHERE YIL=2021 AND HESAPKOD='63007010741'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Bolu Abant İzzet Baysal Üniversitesi ne Hazine Yardımı', HESAPKOD='63007037042' WHERE YIL=2021 AND HESAPKOD='63007010742'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Hatay Mustafa Kemal Üniversitesi ne Hazine Yardımı', HESAPKOD='63007037043' WHERE YIL=2021 AND HESAPKOD='63007010743'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Afyon Kocatepe Üniversitesi ne Hazine Yardımı', HESAPKOD='63007037044' WHERE YIL=2021 AND HESAPKOD='63007010744'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kafkas Üniversitesi ne Hazine Yardımı', HESAPKOD='63007037045' WHERE YIL=2021 AND HESAPKOD='63007010745'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Çanakkale 18 Mart Üniversitesi ne Hazine Yardımı', HESAPKOD='63007037046' WHERE YIL=2021 AND HESAPKOD='63007010746'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Niğde Ömer Halisdemir Üniversitesi’ne Hazine Yardımı', HESAPKOD='63007037047' WHERE YIL=2021 AND HESAPKOD='63007010747'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kütahya Dumlupınar Üniversitesi ne Hazine Yardımı', HESAPKOD='63007037048' WHERE YIL=2021 AND HESAPKOD='63007010748'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Tokat Gaziosmanpaşa Üniversitesi ne Hazine Yardımı', HESAPKOD='63007037049' WHERE YIL=2021 AND HESAPKOD='63007010749'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Muğla Sıtkı Koçman Üniversitesi ne Hazine Yardımı', HESAPKOD='63007037050' WHERE YIL=2021 AND HESAPKOD='63007010750'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kahramanmaraş Sütçü İmam Üniversitesi ne Hazine Yardımı', HESAPKOD='63007037051' WHERE YIL=2021 AND HESAPKOD='63007010751'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kırıkkale Üniversitesi ne Hazine Yardımı', HESAPKOD='63007037052' WHERE YIL=2021 AND HESAPKOD='63007010752'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Eskişehir Osmangazi Üniversitesi ne Hazine Yardımı', HESAPKOD='63007037053' WHERE YIL=2021 AND HESAPKOD='63007010753'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Galatasaray Üniversitesi ne Hazine Yardımı', HESAPKOD='63007037054' WHERE YIL=2021 AND HESAPKOD='63007010754'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kırşehir Ahi Evran Üniversitesi ne Hazine Yardımı', HESAPKOD='63007037055' WHERE YIL=2021 AND HESAPKOD='63007010755'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kastamonu Üniversitesi ne Hazine Yardımı', HESAPKOD='63007037056' WHERE YIL=2021 AND HESAPKOD='63007010756'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Düzce Üniversitesi ne Hazine Yardımı', HESAPKOD='63007037057' WHERE YIL=2021 AND HESAPKOD='63007010757'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Burdur Mehmet Akif Ersoy Üniversitesi ne Hazine Yardımı', HESAPKOD='63007037058' WHERE YIL=2021 AND HESAPKOD='63007010758'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Uşak Üniversitesi ne Hazine Yardımı', HESAPKOD='63007037059' WHERE YIL=2021 AND HESAPKOD='63007010759'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Recep Tayyip Erdoğan Üniversitesi ne Hazine Yardımı', HESAPKOD='63007037060' WHERE YIL=2021 AND HESAPKOD='63007010760'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Tekirdağ Namık Kemal Üniversitesi ne Hazine Yardımı', HESAPKOD='63007037061' WHERE YIL=2021 AND HESAPKOD='63007010761'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Erzincan Binali Yıldırım Üniversitesi ne Hazine Yardımı', HESAPKOD='63007037062' WHERE YIL=2021 AND HESAPKOD='63007010762'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Aksaray Üniversitesi ne Hazine Yardımı', HESAPKOD='63007037063' WHERE YIL=2021 AND HESAPKOD='63007010763'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Giresun Üniversitesi ne Hazine Yardımı', HESAPKOD='63007037064' WHERE YIL=2021 AND HESAPKOD='63007010764'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Hitit Üniversitesi ne Hazine Yardımı', HESAPKOD='63007037065' WHERE YIL=2021 AND HESAPKOD='63007010765'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yozgat Bozok Üniversitesi ne Hazine Yardımı', HESAPKOD='63007037066' WHERE YIL=2021 AND HESAPKOD='63007010766'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Adıyaman Üniversitesi ne Hazine Yardımı', HESAPKOD='63007037067' WHERE YIL=2021 AND HESAPKOD='63007010767'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Ordu Üniversitesi ne Hazine Yardımı', HESAPKOD='63007037068' WHERE YIL=2021 AND HESAPKOD='63007010768'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Amasya Üniversitesi ne Hazine Yardımı', HESAPKOD='63007037069' WHERE YIL=2021 AND HESAPKOD='63007010769'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Karamanoğlu Mehmetbey Üniversitesi ne Hazine Yardımı', HESAPKOD='63007037070' WHERE YIL=2021 AND HESAPKOD='63007010770'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Ağrı İbrahim Çeçen Üniversitesi ne Hazine Yardımı', HESAPKOD='63007037071' WHERE YIL=2021 AND HESAPKOD='63007010771'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sinop Üniversitesi ne Hazine Yardımı', HESAPKOD='63007037072' WHERE YIL=2021 AND HESAPKOD='63007010772'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Siirt Üniversitesi ne Hazine Yardımı', HESAPKOD='63007037073' WHERE YIL=2021 AND HESAPKOD='63007010773'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Nevşehir Hacı Bektaş Veli Üniversitesi ne Hazine Yardımı', HESAPKOD='63007037074' WHERE YIL=2021 AND HESAPKOD='63007010774'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Karabük Üniversitesi ne Hazine Yardımı', HESAPKOD='63007037075' WHERE YIL=2021 AND HESAPKOD='63007010775'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kilis 7 Aralık Üniversitesi ne Hazine Yardımı', HESAPKOD='63007037076' WHERE YIL=2021 AND HESAPKOD='63007010776'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Çankırı Karatekin Üniversitesi ne Hazine Yardımı', HESAPKOD='63007037077' WHERE YIL=2021 AND HESAPKOD='63007010777'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Artvin Çoruh Üniversitesi ne Hazine Yardımı', HESAPKOD='63007037078' WHERE YIL=2021 AND HESAPKOD='63007010778'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Bilecik Şeyh Edebali Üniversitesi ne Hazine Yardımı', HESAPKOD='63007037079' WHERE YIL=2021 AND HESAPKOD='63007010779'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Bitlis Eren Üniversitesi ne Hazine Yardımı', HESAPKOD='63007037080' WHERE YIL=2021 AND HESAPKOD='63007010780'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kırklareli Üniversitesi ne Hazine Yardımı', HESAPKOD='63007037081' WHERE YIL=2021 AND HESAPKOD='63007010781'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Osmaniye Korkut Ata Üniversitesi ne Hazine Yardımı', HESAPKOD='63007037082' WHERE YIL=2021 AND HESAPKOD='63007010782'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Bingöl Üniversitesi ne Hazine Yardımı', HESAPKOD='63007037083' WHERE YIL=2021 AND HESAPKOD='63007010783'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Muş Alparslan Üniversitesi ne Hazine Yardımı', HESAPKOD='63007037084' WHERE YIL=2021 AND HESAPKOD='63007010784'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Mardin Artuklu Üniversitesi ne Hazine Yardımı', HESAPKOD='63007037085' WHERE YIL=2021 AND HESAPKOD='63007010785'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Batman Üniversitesi ne Hazine Yardımı', HESAPKOD='63007037086' WHERE YIL=2021 AND HESAPKOD='63007010786'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Ardahan Üniversitesi ne Hazine Yardımı', HESAPKOD='63007037087' WHERE YIL=2021 AND HESAPKOD='63007010787'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Bartın Üniversitesi ne Hazine Yardımı', HESAPKOD='63007037088' WHERE YIL=2021 AND HESAPKOD='63007010788'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Bayburt Üniversitesi ne Hazine Yardımı', HESAPKOD='63007037089' WHERE YIL=2021 AND HESAPKOD='63007010789'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Gümüşhane Üniversitesi ne Hazine Yardımı', HESAPKOD='63007037090' WHERE YIL=2021 AND HESAPKOD='63007010790'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Hakkari Üniversitesi ne Hazine Yardımı', HESAPKOD='63007037091' WHERE YIL=2021 AND HESAPKOD='63007010791'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Iğdır Üniversitesi ne Hazine Yardımı', HESAPKOD='63007037092' WHERE YIL=2021 AND HESAPKOD='63007010792'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Şırnak Üniversitesi ne Hazine Yardımı', HESAPKOD='63007037093' WHERE YIL=2021 AND HESAPKOD='63007010793'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Munzur Üniversitesi ne Hazine Yardımı', HESAPKOD='63007037094' WHERE YIL=2021 AND HESAPKOD='63007010794'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yalova Üniversitesi ne Hazine Yardımı', HESAPKOD='63007037095' WHERE YIL=2021 AND HESAPKOD='63007010795'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Türk-Alman Üniversitesi ne Hazine Yardımı', HESAPKOD='63007037096' WHERE YIL=2021 AND HESAPKOD='63007010796'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Ankara Yıldırım Beyazıt Üniversitesi ne Hazine Yardımı', HESAPKOD='63007037097' WHERE YIL=2021 AND HESAPKOD='63007010797'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Bursa Teknik Üniversitesi ne Hazine Yardımı', HESAPKOD='63007037098' WHERE YIL=2021 AND HESAPKOD='63007010798'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='İstanbul Medeniyet Üniversitesi ne Hazine Yardımı', HESAPKOD='63007037099' WHERE YIL=2021 AND HESAPKOD='63007010799'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yükseköğretim Kurumlarına Hazine Yardımları', HESAPKOD='630070380' WHERE YIL=2021 AND HESAPKOD='630070108'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='İzmir Katip Çelebi Üniversitesi ne Hazine Yardımı', HESAPKOD='63007038001' WHERE YIL=2021 AND HESAPKOD='63007010801'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Necmettin Erbakan Üniversitesi ne Hazine Yardımı', HESAPKOD='63007038002' WHERE YIL=2021 AND HESAPKOD='63007010802'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Abdullah Gül Üniversitesi ne Hazine Yardımı', HESAPKOD='63007038003' WHERE YIL=2021 AND HESAPKOD='63007010803'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Erzurum Teknik Üniversitesi ne Hazine Yardımı', HESAPKOD='63007038004' WHERE YIL=2021 AND HESAPKOD='63007010804'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Adana Alparslan Türkeş Bilim ve Teknoloji Üniversitesi ne Hazine Yardımı', HESAPKOD='63007038005' WHERE YIL=2021 AND HESAPKOD='63007010805'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Ankara Sosyal Bilimler Üniversitesi ne Hazine Yardımı', HESAPKOD='63007038006' WHERE YIL=2021 AND HESAPKOD='63007010806'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sağlık Bilimleri Üniversitesi ne Hazine Yardımı', HESAPKOD='63007038007' WHERE YIL=2021 AND HESAPKOD='63007010807'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Bandırma Onyedi Eylül Üniversitesi ne Hazine Yardımı', HESAPKOD='63007038008' WHERE YIL=2021 AND HESAPKOD='63007010808'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='İskenderun Teknik Üniversitesi ne Hazine Yardımı', HESAPKOD='63007038009' WHERE YIL=2021 AND HESAPKOD='63007010809'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Alanya Alaaddin Keykubat Üniversitesi ne Hazine Yardımı', HESAPKOD='63007038010' WHERE YIL=2021 AND HESAPKOD='63007010810'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Türkiye Uluslararası İslam, Bilim ve Teknoloji Üniversitesi ne Hazine Yardımı', HESAPKOD='63007038011' WHERE YIL=2021 AND HESAPKOD='63007010811'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='İzmir Bakırçay Üniversitesi ne Hazine Yardımı', HESAPKOD='63007038012' WHERE YIL=2021 AND HESAPKOD='63007010812'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='İzmir Demokrasi Üniversitesi ne Hazine Yardımı', HESAPKOD='63007038013' WHERE YIL=2021 AND HESAPKOD='63007010813'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yükseköğretim Kalite Kurulu na Hazine Yardımı', HESAPKOD='63007038014' WHERE YIL=2021 AND HESAPKOD='63007010814'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Ankara Müzik ve Güzel Sanatlar Üniversitesi ne Hazine Yardımı', HESAPKOD='63007038015' WHERE YIL=2021 AND HESAPKOD='63007010815'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Gaziantep İslam Bilim ve Teknoloji Üniversitesi ne Hazine Yardımı', HESAPKOD='63007038016' WHERE YIL=2021 AND HESAPKOD='63007010816'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Konya Teknik Üniversitesi ne Hazine Yardımı', HESAPKOD='63007038017' WHERE YIL=2021 AND HESAPKOD='63007010817'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kütahya Sağlık Bilimleri Üniversitesi ne Hazine Yardımı', HESAPKOD='63007038018' WHERE YIL=2021 AND HESAPKOD='63007010818'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Malatya Turgut Özal Üniversitesi ne Hazine Yardımı', HESAPKOD='63007038019' WHERE YIL=2021 AND HESAPKOD='63007010819'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='İstanbul Üniversitesi-Cerrahpaşa ya Hazine Yardımı', HESAPKOD='63007038020' WHERE YIL=2021 AND HESAPKOD='63007010820'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Ankara Hacı Bayram Veli Üniversitesi ne Hazine Yardımı', HESAPKOD='63007038021' WHERE YIL=2021 AND HESAPKOD='63007010821'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sakarya Uygulamalı Bilimler Üniversitesi ne Hazine Yardımı', HESAPKOD='63007038022' WHERE YIL=2021 AND HESAPKOD='63007010822'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Samsun Üniversitesi ne Hazine Yardımı', HESAPKOD='63007038023' WHERE YIL=2021 AND HESAPKOD='63007010823'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sivas Bilim ve Teknoloji Üniversitesi ne Hazine Yardımı', HESAPKOD='63007038024' WHERE YIL=2021 AND HESAPKOD='63007010824'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Tarsus Üniversitesi ne Hazine Yardımı', HESAPKOD='63007038025' WHERE YIL=2021 AND HESAPKOD='63007010825'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Trabzon Üniversitesi ne Hazine Yardımı', HESAPKOD='63007038026' WHERE YIL=2021 AND HESAPKOD='63007010826'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kayseri Üniversitesi ne Hazine Yardımı', HESAPKOD='63007038027' WHERE YIL=2021 AND HESAPKOD='63007010827'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kahramanmaraş İstiklal Üniversitesi ne Hazine Yardımı', HESAPKOD='63007038028' WHERE YIL=2021 AND HESAPKOD='63007010828'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Eskişehir Teknik Üniversitesi ne Hazine Yardımı', HESAPKOD='63007038029' WHERE YIL=2021 AND HESAPKOD='63007010829'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Isparta Uygulamalı Bilimler Üniversitesi ne Hazine Yardımı', HESAPKOD='63007038030' WHERE YIL=2021 AND HESAPKOD='63007010830'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Afyonkarahisar Sağlık Bilimleri Üniversitesi ne Hazine Yardımı', HESAPKOD='63007038031' WHERE YIL=2021 AND HESAPKOD='63007010831'
DELETE FROM  MUHHESAPPLANI WHERE YIL=2021 AND HESAPKOD='63007010890'
DELETE FROM  MUHHESAPPLANI WHERE YIL=2021 AND HESAPKOD='63007010899'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Kurum, İşletme ve Hane Halkına Yapılan Sermaye Transferleri', HESAPKOD='630070190' WHERE YIL=2021 AND HESAPKOD='630070109'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Dernek, Birlik, Kurum, Kuruluş, Sandık vb Kuruluşlara', HESAPKOD='63007019001' WHERE YIL=2021 AND HESAPKOD='63007010901'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Vakıf Üniversitelerine', HESAPKOD='63007019002' WHERE YIL=2021 AND HESAPKOD='63007010902'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Kamu Kurum ve Kuruluşlarına', HESAPKOD='63007019003' WHERE YIL=2021 AND HESAPKOD='63007010903'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kamu Ortaklıklarına', HESAPKOD='63007019004' WHERE YIL=2021 AND HESAPKOD='63007010904'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Özel Teşebbüslere', HESAPKOD='63007019005' WHERE YIL=2021 AND HESAPKOD='63007010905'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Hane Halklarına', HESAPKOD='63007019006' WHERE YIL=2021 AND HESAPKOD='63007010906'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Türk Kültür Varlığının Korunması ve Tanıtımı Giderleri', HESAPKOD='63007019007' WHERE YIL=2021 AND HESAPKOD='63007010908'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Siyasi Tanıtma Giderleri', HESAPKOD='63007019008' WHERE YIL=2021 AND HESAPKOD='63007010909'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Gençlik ve Spor İl Müdürlüklerine Hazine Yardımı', HESAPKOD='63007039001' WHERE YIL=2021 AND HESAPKOD='63007010911'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kalkınma Ajanslarına', HESAPKOD='63007019009' WHERE YIL=2021 AND HESAPKOD='63007010912'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Bölgelerarası İşbirliği Proje Giderleri', HESAPKOD='63007019010' WHERE YIL=2021 AND HESAPKOD='63007010913'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sosyal ve İktisadi Araştırma Proje Giderleri', HESAPKOD='63007019011' WHERE YIL=2021 AND HESAPKOD='63007010914'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Tarım ve Kırsal Kalkınmayı Destekleme Kurumuna', HESAPKOD='63007019012' WHERE YIL=2021 AND HESAPKOD='63007010915'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Akademik Ar-Ge Projelerine İlişkin Transferler', HESAPKOD='63007019013' WHERE YIL=2021 AND HESAPKOD='63007010920'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Akademik Ar-Ge Projelerine İlişkin Kurum Hissesi', HESAPKOD='63007019014' WHERE YIL=2021 AND HESAPKOD='63007010921'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kamu Ar-Ge Projelerine İlişkin Transferler', HESAPKOD='63007019015' WHERE YIL=2021 AND HESAPKOD='63007010922'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kamu Ar-Ge Projelerine İlişkin Kurum Hissesi', HESAPKOD='63007019016' WHERE YIL=2021 AND HESAPKOD='63007010923'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Savunma ve Uzay Sanayii Ar-Ge Projelerine İlişkin Transferler', HESAPKOD='63007019017' WHERE YIL=2021 AND HESAPKOD='63007010924'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Savunma ve Uzay Sanayii Ar-Ge Projelerine İlişkin Kurum Hissesi', HESAPKOD='63007019018' WHERE YIL=2021 AND HESAPKOD='63007010925'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Bilim ve Toplum Projelerine İlişkin Transferler', HESAPKOD='63007019019' WHERE YIL=2021 AND HESAPKOD='63007010926'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Bilim ve Toplum Projelerine İlişkin Kurum Hissesi', HESAPKOD='63007019020' WHERE YIL=2021 AND HESAPKOD='63007010927'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Devlet Yükseköğretim Kurumlarına Bölgesel Kalkınma Destekleri', HESAPKOD='63007019021' WHERE YIL=2021 AND HESAPKOD='63007010928'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sınıflandırmaya Girmeyen Diğer Kurum, İşletme ve Hane Halkına  Yapılan Sermaye Transferleri', HESAPKOD='63007019090' WHERE YIL=2021 AND HESAPKOD='63007010990'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Bilimsel Araştırma Projelerinin Desteklenmesine İlişkin Giderler', HESAPKOD='63007019022' WHERE YIL=2021 AND HESAPKOD='63007010999'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yurtdışı Sermaye Transferleri', HESAPKOD='6300702' WHERE YIL=2021 AND HESAPKOD='6300702'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Dış Ülkelere Yapılan Yardımlar', HESAPKOD='630070210' WHERE YIL=2021 AND HESAPKOD='630070201'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kuzey Kıbrıs Türk Cumhuriyeti ne', HESAPKOD='63007021001' WHERE YIL=2021 AND HESAPKOD='63007020101'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Ülkelere', HESAPKOD='63007021090' WHERE YIL=2021 AND HESAPKOD='63007020190'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Uluslararası Kuruluşlara Yapılan Ödemeler', HESAPKOD='630070220' WHERE YIL=2021 AND HESAPKOD='630070202'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Uluslararası Kuruluşlara Katkı Ödemeleri', HESAPKOD='63007022001' WHERE YIL=2021 AND HESAPKOD='63007020201'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Uluslararası Kuruluşlara Yardım Ödemeleri', HESAPKOD='63007022002' WHERE YIL=2021 AND HESAPKOD='63007020202'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Uluslararası Kuruluşlara Diğer Ödemeler', HESAPKOD='63007022090' WHERE YIL=2021 AND HESAPKOD='63007020290'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Uluslarüstü Kuruluşlara', HESAPKOD='630070230' WHERE YIL=2021 AND HESAPKOD='630070203'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Uluslarüstü Kuruluşlara', HESAPKOD='63007023001' WHERE YIL=2021 AND HESAPKOD='63007020301'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Uluslarüstü Kuruluşlardan Merkezlerine ', HESAPKOD='630070240' WHERE YIL=2021 AND HESAPKOD='630070204'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Uluslarüstü Kuruluşlardan Merkezlerine', HESAPKOD='63007024001' WHERE YIL=2021 AND HESAPKOD='63007020401'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Eğitim Kurumlarına', HESAPKOD='630070250' WHERE YIL=2021 AND HESAPKOD='630070205'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Hoca Ahmet Yesevi Uluslararası Türk Kazak Üniversitesine', HESAPKOD='63007025001' WHERE YIL=2021 AND HESAPKOD='63007020501'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kırgızistan-Türkiye Manas Üniversitesine', HESAPKOD='63007025002' WHERE YIL=2021 AND HESAPKOD='63007020502'
DELETE FROM  MUHHESAPPLANI WHERE YIL=2021 AND HESAPKOD='63007020503'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Yurtdışı Sermaye Transferleri', HESAPKOD='630070290' WHERE YIL=2021 AND HESAPKOD='630070209'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sınıflandırmaya Girmeyen Diğer Yurtdışı Sermaye Transferleri', HESAPKOD='63007029090' WHERE YIL=2021 AND HESAPKOD='63007020901'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Türk Kültür Varlığının Korunması ve Tanıtımı Giderleri', HESAPKOD='63007029001' WHERE YIL=2021 AND HESAPKOD='63007020908'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Siyasi Tanıtma Giderleri', HESAPKOD='63007029002' WHERE YIL=2021 AND HESAPKOD='63007020909'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Borç Verme', HESAPKOD='63008' WHERE YIL=2021 AND HESAPKOD='63008'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yurtiçi Borç Verme ', HESAPKOD='6300801' WHERE YIL=2021 AND HESAPKOD='6300801'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kamu Teşebbüslerine, Döner Sermayelere, Fonlara ve Mali Kurumlara Borç Verme', HESAPKOD='630080160' WHERE YIL=2021 AND HESAPKOD='630080106'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kredi Garanti Fonuna', HESAPKOD='63008016301' WHERE YIL=2021 AND HESAPKOD='63008010680'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Yurtiçi Borç Verme', HESAPKOD='630080190' WHERE YIL=2021 AND HESAPKOD='630080109'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sınıflandırmaya Girmeyen Diğer Yurtiçi Borç Verme', HESAPKOD='63008019090' WHERE YIL=2021 AND HESAPKOD='63008010990'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Proje Kapsamında Yapılan Cari Giderler', HESAPKOD='63030' WHERE YIL=2021 AND HESAPKOD='63030'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Proje Kapsamında Yapılan Cari Giderler', HESAPKOD='6303006' WHERE YIL=2021 AND HESAPKOD='6303006'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Mamul Mal Alımları', HESAPKOD='630300601' WHERE YIL=2021 AND HESAPKOD='630300601'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Taşıt Alımları', HESAPKOD='63030060140' WHERE YIL=2021 AND HESAPKOD='63030060104'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='İş Makinası ve Taşınır Tesis Alımları', HESAPKOD='63030060150' WHERE YIL=2021 AND HESAPKOD='63030060105'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Menkul Sermaye Üretim Giderleri', HESAPKOD='630300602' WHERE YIL=2021 AND HESAPKOD='630300602'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Müşavir Firma ve Kişilere Ödemeler', HESAPKOD='63030060210' WHERE YIL=2021 AND HESAPKOD='63030060201'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Gayri Maddi Hak Alımları', HESAPKOD='630300603' WHERE YIL=2021 AND HESAPKOD='630300603'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Bilgisayar Yazılımı Alımları', HESAPKOD='63030060310' WHERE YIL=2021 AND HESAPKOD='63030060301'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Lisans Alımları', HESAPKOD='63030060330' WHERE YIL=2021 AND HESAPKOD='63030060303'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Gayrimenkul Sermaye Üretim Giderleri', HESAPKOD='630300605' WHERE YIL=2021 AND HESAPKOD='630300605'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Müşavir Firma ve Kişilere Ödemeler', HESAPKOD='63030060510' WHERE YIL=2021 AND HESAPKOD='63030060501'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Malzeme Giderleri', HESAPKOD='63030060520' WHERE YIL=2021 AND HESAPKOD='63030060502'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Enerji Giderleri', HESAPKOD='63030060540' WHERE YIL=2021 AND HESAPKOD='63030060504'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Haberleşme Giderleri', HESAPKOD='63030060550' WHERE YIL=2021 AND HESAPKOD='63030060505'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kira Giderleri', HESAPKOD='63030060560' WHERE YIL=2021 AND HESAPKOD='63030060506'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Müteahhitlik Giderleri', HESAPKOD='63030060570' WHERE YIL=2021 AND HESAPKOD='63030060507'
DELETE FROM  MUHHESAPPLANI WHERE YIL=2021 AND HESAPKOD='63030060508'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Gayrimenkul Sermaye Üretim Giderleri', HESAPKOD='63030060590' WHERE YIL=2021 AND HESAPKOD='63030060509'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Menkul Malların Büyük Onarım Giderleri', HESAPKOD='630300606' WHERE YIL=2021 AND HESAPKOD='630300606'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Müşavir Firma ve Kişilere Ödemeler', HESAPKOD='63030060610' WHERE YIL=2021 AND HESAPKOD='63030060601'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Malzeme Giderleri', HESAPKOD='63030060620' WHERE YIL=2021 AND HESAPKOD='63030060602'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Taşıma Giderleri', HESAPKOD='63030060630' WHERE YIL=2021 AND HESAPKOD='63030060603'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Enerji Giderleri', HESAPKOD='63030060640' WHERE YIL=2021 AND HESAPKOD='63030060604'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Haberleşme Giderleri', HESAPKOD='63030060650' WHERE YIL=2021 AND HESAPKOD='63030060605'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kira Giderleri', HESAPKOD='63030060660' WHERE YIL=2021 AND HESAPKOD='63030060606'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Müteahhitlik Giderleri', HESAPKOD='63030060670' WHERE YIL=2021 AND HESAPKOD='63030060607'
DELETE FROM  MUHHESAPPLANI WHERE YIL=2021 AND HESAPKOD='63030060608'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Menkul Malların Diğer Büyük Onarım Giderleri', HESAPKOD='63030060690' WHERE YIL=2021 AND HESAPKOD='63030060609'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Gayrimenkul Büyük Onarım Giderleri', HESAPKOD='630300607' WHERE YIL=2021 AND HESAPKOD='630300607'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Müşavir Firma ve Kişilere Ödemeler', HESAPKOD='63030060710' WHERE YIL=2021 AND HESAPKOD='63030060701'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Malzeme Giderleri', HESAPKOD='63030060720' WHERE YIL=2021 AND HESAPKOD='63030060702'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Enerji Giderleri', HESAPKOD='63030060740' WHERE YIL=2021 AND HESAPKOD='63030060704'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Haberleşme Giderleri', HESAPKOD='63030060750' WHERE YIL=2021 AND HESAPKOD='63030060705'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kira Giderleri', HESAPKOD='63030060760' WHERE YIL=2021 AND HESAPKOD='63030060706'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Müteahhitlik Giderleri', HESAPKOD='63030060770' WHERE YIL=2021 AND HESAPKOD='63030060707'
DELETE FROM  MUHHESAPPLANI WHERE YIL=2021 AND HESAPKOD='63030060708'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Gayrimenkul Büyük Onarım Giderleri', HESAPKOD='63030060790' WHERE YIL=2021 AND HESAPKOD='63030060709'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Stok Alımları', HESAPKOD='630300608' WHERE YIL=2021 AND HESAPKOD='630300608'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Tarımsal Ürünler', HESAPKOD='63030060820' WHERE YIL=2021 AND HESAPKOD='63030060802'
DELETE FROM  MUHHESAPPLANI WHERE YIL=2021 AND HESAPKOD='63030060808'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Sermaye Giderleri', HESAPKOD='630300609' WHERE YIL=2021 AND HESAPKOD='630300609'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Gizli Hizmet Giderleri', HESAPKOD='63030060910' WHERE YIL=2021 AND HESAPKOD='63030060901'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yolluk Giderleri', HESAPKOD='63030060920' WHERE YIL=2021 AND HESAPKOD='63030060902'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Sermaye Giderleri', HESAPKOD='63030060990' WHERE YIL=2021 AND HESAPKOD='63030060909'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='BÜTÇE GİDERLERİ HESABI', HESAPKOD='830' WHERE YIL=2021 AND HESAPKOD='830'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Personel Giderleri', HESAPKOD='83001' WHERE YIL=2021 AND HESAPKOD='83001'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Memurlar', HESAPKOD='8300101' WHERE YIL=2021 AND HESAPKOD='8300101'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Temel Maaşlar', HESAPKOD='830010110' WHERE YIL=2021 AND HESAPKOD='830010101'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Temel Maaşlar', HESAPKOD='83001011001' WHERE YIL=2021 AND HESAPKOD='83001010101'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Taban Aylığı', HESAPKOD='83001011002' WHERE YIL=2021 AND HESAPKOD='83001010102'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yurtdışı Aylığı', HESAPKOD='83001011003' WHERE YIL=2021 AND HESAPKOD='83001010103'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Zamlar ve Tazminatlar', HESAPKOD='830010120' WHERE YIL=2021 AND HESAPKOD='830010102'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Zamlar ve Tazminatlar', HESAPKOD='83001012001' WHERE YIL=2021 AND HESAPKOD='83001010201'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Ödenekler', HESAPKOD='830010130' WHERE YIL=2021 AND HESAPKOD='830010103'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Ödenekler', HESAPKOD='83001013001' WHERE YIL=2021 AND HESAPKOD='83001010301'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sosyal Haklar', HESAPKOD='830010140' WHERE YIL=2021 AND HESAPKOD='830010104'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sosyal Haklar', HESAPKOD='83001014001' WHERE YIL=2021 AND HESAPKOD='83001010401'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Ek Çalışma Karşılıkları', HESAPKOD='830010150' WHERE YIL=2021 AND HESAPKOD='830010105'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Ek Çalışma Karşılıkları', HESAPKOD='83001015001' WHERE YIL=2021 AND HESAPKOD='83001010501'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='İş Sağlığı ve Güvenliği Hizmetleri Görevlendirme Ücretleri', HESAPKOD='83001015002' WHERE YIL=2021 AND HESAPKOD='83001010502'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Ek Ders Ücretleri', HESAPKOD='83001015003' WHERE YIL=2021 AND HESAPKOD='83001010503'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Destekleme ve Yetiştirme Kursları Kapsamında Ödenen Ücretler', HESAPKOD='83001015004' WHERE YIL=2021 AND HESAPKOD='83001010504'
DELETE FROM  MUHHESAPPLANI WHERE YIL=2021 AND HESAPKOD='83001010505'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Ödül ve İkramiyeler', HESAPKOD='830010160' WHERE YIL=2021 AND HESAPKOD='830010106'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Ödül ve İkramiyeler', HESAPKOD='83001016001' WHERE YIL=2021 AND HESAPKOD='83001010601'
DELETE FROM  MUHHESAPPLANI WHERE YIL=2021 AND HESAPKOD='830010108'
DELETE FROM  MUHHESAPPLANI WHERE YIL=2021 AND HESAPKOD='83001010801'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Giderler', HESAPKOD='830010190' WHERE YIL=2021 AND HESAPKOD='830010109'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Giderler', HESAPKOD='83001019001' WHERE YIL=2021 AND HESAPKOD='83001010901'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sözleşmeli Personel', HESAPKOD='8300102' WHERE YIL=2021 AND HESAPKOD='8300102'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Ücretler', HESAPKOD='830010210' WHERE YIL=2021 AND HESAPKOD='830010201'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='657 SK 4/B Sözleşmeli Personel Ücretleri', HESAPKOD='83001021001' WHERE YIL=2021 AND HESAPKOD='83001020101'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kadro Karşılığı Sözleşmeli Personel Ücretleri', HESAPKOD='83001021002' WHERE YIL=2021 AND HESAPKOD='83001020102'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Akademik Sözleşmeli Personelin Ücretleri', HESAPKOD='83001021003' WHERE YIL=2021 AND HESAPKOD='83001020103'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yabancı Uyruklu Sözleşmeli Personelin Ücretleri', HESAPKOD='83001021004' WHERE YIL=2021 AND HESAPKOD='83001020104'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sözleşmeli Sanatçıların  Ücretleri', HESAPKOD='83001021005' WHERE YIL=2021 AND HESAPKOD='83001020105'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yurtdışı Sözleşmeli Personelin Ücretleri', HESAPKOD='83001021006' WHERE YIL=2021 AND HESAPKOD='83001020106'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Sözleşmeli Personel Ücretleri', HESAPKOD='83001021090' WHERE YIL=2021 AND HESAPKOD='83001020190'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Zamlar ve Tazminatlar', HESAPKOD='830010220' WHERE YIL=2021 AND HESAPKOD='830010202'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='657 SK 4/B Sözleşmeli Personel Zam ve Tazminatları', HESAPKOD='83001022001' WHERE YIL=2021 AND HESAPKOD='83001020201'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kadro Karşılığı Sözleşmeli Personel Zam ve Tazminatları', HESAPKOD='83001022002' WHERE YIL=2021 AND HESAPKOD='83001020202'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Akademik Sözleşmeli Personelin Zam ve Tazminatları', HESAPKOD='83001022003' WHERE YIL=2021 AND HESAPKOD='83001020203'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yabancı Uyruklu Sözleşmeli Personelin Zam ve Tazminatları', HESAPKOD='83001022004' WHERE YIL=2021 AND HESAPKOD='83001020204'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sözleşmeli Sanatçıların  Zam ve Tazminatları', HESAPKOD='83001022005' WHERE YIL=2021 AND HESAPKOD='83001020205'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yurtdışı Sözleşmeli Personelin Zam ve Tazminatları', HESAPKOD='83001022006' WHERE YIL=2021 AND HESAPKOD='83001020206'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Sözleşmeli Personel Zam ve Tazminatları', HESAPKOD='83001022090' WHERE YIL=2021 AND HESAPKOD='83001020290'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Ödenekler', HESAPKOD='830010230' WHERE YIL=2021 AND HESAPKOD='830010203'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='657 SK 4/B Sözleşmeli Personel Ödenekleri', HESAPKOD='83001023001' WHERE YIL=2021 AND HESAPKOD='83001020301'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kadro Karşılığı Sözleşmeli Personel Ödenekleri', HESAPKOD='83001023002' WHERE YIL=2021 AND HESAPKOD='83001020302'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Akademik Sözleşmeli Personelin Ödenekleri', HESAPKOD='83001023003' WHERE YIL=2021 AND HESAPKOD='83001020303'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yabancı Uyruklu Sözleşmeli Personelin Ödenekleri', HESAPKOD='83001023004' WHERE YIL=2021 AND HESAPKOD='83001020304'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sözleşmeli Sanatçıların Ödenekleri', HESAPKOD='83001023005' WHERE YIL=2021 AND HESAPKOD='83001020305'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yurtdışı Sözleşmeli Personelin Ödenekleri', HESAPKOD='83001023006' WHERE YIL=2021 AND HESAPKOD='83001020306'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Sözleşmeli Personel Ödenekleri', HESAPKOD='83001023090' WHERE YIL=2021 AND HESAPKOD='83001020390'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sosyal Haklar', HESAPKOD='830010240' WHERE YIL=2021 AND HESAPKOD='830010204'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='657 SK 4/B Sözleşmeli Personel Sosyal Hakları', HESAPKOD='83001024001' WHERE YIL=2021 AND HESAPKOD='83001020401'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kadro Karşılığı Sözleşmeli Personel Sosyal Hakları', HESAPKOD='83001024002' WHERE YIL=2021 AND HESAPKOD='83001020402'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Akademik Sözleşmeli Personelin Sosyal Hakları', HESAPKOD='83001024003' WHERE YIL=2021 AND HESAPKOD='83001020403'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yabancı Uyruklu Sözleşmeli Personelin Sosyal Hakları', HESAPKOD='83001024004' WHERE YIL=2021 AND HESAPKOD='83001020404'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sözleşmeli Sanatçıların Sosyal Hakları', HESAPKOD='83001024005' WHERE YIL=2021 AND HESAPKOD='83001020405'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yurtdışı Sözleşmeli Personelin Sosyal Hakları', HESAPKOD='83001024006' WHERE YIL=2021 AND HESAPKOD='83001020406'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Sözleşmeli Personelin Sosyal Hakları', HESAPKOD='83001024090' WHERE YIL=2021 AND HESAPKOD='83001020490'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Ek Çalışma Karşılıkları', HESAPKOD='830010250' WHERE YIL=2021 AND HESAPKOD='830010205'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='657 SK 4/B Sözleşmeli Personelin Ek Çalışma Karşılıkları', HESAPKOD='83001025001' WHERE YIL=2021 AND HESAPKOD='83001020501'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kadro Karşılığı Sözleşmeli Personelin Ek Çalışma Karşılıkları', HESAPKOD='83001025002' WHERE YIL=2021 AND HESAPKOD='83001020502'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Akademik Sözleşmeli Personelin Ek Çalışma Karşılıkları', HESAPKOD='83001025003' WHERE YIL=2021 AND HESAPKOD='83001020503'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yabancı Uyruklu Sözleşmeli Personelin Ek Çalışma Karşılıkları', HESAPKOD='83001025004' WHERE YIL=2021 AND HESAPKOD='83001020504'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sözleşmeli Sanatçıların Ek Çalışma Karşılıkları', HESAPKOD='83001025005' WHERE YIL=2021 AND HESAPKOD='83001020505'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yurtdışı Sözleşmeli Personelin Ek Çalışma Karşılıkları', HESAPKOD='83001025006' WHERE YIL=2021 AND HESAPKOD='83001020506'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Sözleşmeli Personelin Ek Çalışma Karşılıkları', HESAPKOD='83001025090' WHERE YIL=2021 AND HESAPKOD='83001020590'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Ödül ve İkramiyeler', HESAPKOD='830010260' WHERE YIL=2021 AND HESAPKOD='830010206'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='657 SK 4/B Sözleşmeli Personelin Ödül ve İkramiyeleri', HESAPKOD='83001026001' WHERE YIL=2021 AND HESAPKOD='83001020601'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kadro Karşılığı Sözleşmeli Personelin Ödül ve İkramiyeleri', HESAPKOD='83001026002' WHERE YIL=2021 AND HESAPKOD='83001020602'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Akademik Sözleşmeli Personelin Ödül ve İkramiyeleri', HESAPKOD='83001026003' WHERE YIL=2021 AND HESAPKOD='83001020603'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yabancı Uyruklu Sözleşmeli Personelin Ödül ve İkramiyeleri', HESAPKOD='83001026004' WHERE YIL=2021 AND HESAPKOD='83001020604'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sözleşmeli Sanatçıların Ödül ve İkramiyeleri', HESAPKOD='83001026005' WHERE YIL=2021 AND HESAPKOD='83001020605'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yurtdışı Sözleşmeli Personelin Ödül ve İkramiyeleri', HESAPKOD='83001026006' WHERE YIL=2021 AND HESAPKOD='83001020606'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Sözleşmeli Personelin Ödül ve İkramiyeleri', HESAPKOD='83001026090' WHERE YIL=2021 AND HESAPKOD='83001020690'
DELETE FROM  MUHHESAPPLANI WHERE YIL=2021 AND HESAPKOD='830010208'
DELETE FROM  MUHHESAPPLANI WHERE YIL=2021 AND HESAPKOD='83001020801'
DELETE FROM  MUHHESAPPLANI WHERE YIL=2021 AND HESAPKOD='83001020802'
DELETE FROM  MUHHESAPPLANI WHERE YIL=2021 AND HESAPKOD='83001020803'
DELETE FROM  MUHHESAPPLANI WHERE YIL=2021 AND HESAPKOD='83001020804'
DELETE FROM  MUHHESAPPLANI WHERE YIL=2021 AND HESAPKOD='83001020805'
DELETE FROM  MUHHESAPPLANI WHERE YIL=2021 AND HESAPKOD='83001020890'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Giderler', HESAPKOD='830010290' WHERE YIL=2021 AND HESAPKOD='830010209'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='657 SK 4/B Sözleşmeli Personelin Diğer Giderleri', HESAPKOD='83001029001' WHERE YIL=2021 AND HESAPKOD='83001020901'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kadro Karşılığı Sözleşmeli Personelin Diğer Giderleri', HESAPKOD='83001029002' WHERE YIL=2021 AND HESAPKOD='83001020902'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Akademik Sözleşmeli Personelin Diğer Giderleri', HESAPKOD='83001029003' WHERE YIL=2021 AND HESAPKOD='83001020903'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yabancı Uyruklu Sözleşmeli Personelin Diğer Giderleri', HESAPKOD='83001029004' WHERE YIL=2021 AND HESAPKOD='83001020904'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sözleşmeli Sanatçıların Diğer Giderleri', HESAPKOD='83001029005' WHERE YIL=2021 AND HESAPKOD='83001020905'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yurtdışı Sözleşmeli Personelin Diğer Giderleri', HESAPKOD='83001029006' WHERE YIL=2021 AND HESAPKOD='83001020906'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Sözleşmeli Personelin Diğer Giderleri', HESAPKOD='83001029090' WHERE YIL=2021 AND HESAPKOD='83001020990'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='İşçiler', HESAPKOD='8300103' WHERE YIL=2021 AND HESAPKOD='8300103'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Ücretler', HESAPKOD='830010310' WHERE YIL=2021 AND HESAPKOD='830010301'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sürekli işçilerin Ücretleri', HESAPKOD='83001031001' WHERE YIL=2021 AND HESAPKOD='83001030101'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Geçici işçilerin Ücretleri', HESAPKOD='83001031002' WHERE YIL=2021 AND HESAPKOD='83001030102'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='375 S KHK’nın Geçici 23 üncü Md Kapsamında Sürekli İşçi Kadrolarına Geçirilen İşçilerin Ücretleri', HESAPKOD='83001031003' WHERE YIL=2021 AND HESAPKOD='83001030103'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='375 S KHK’nın Geçici 23 üncü Md Kapsamında Geçici İşçi Pozisyonlarına Geçirilen İşçilerin Ücretleri', HESAPKOD='83001031004' WHERE YIL=2021 AND HESAPKOD='83001030104'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='İhbar ve Kıdem Tazminatları', HESAPKOD='830010320' WHERE YIL=2021 AND HESAPKOD='830010302'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sürekli İşçilerin İhbar ve Kıdem Tazminatları', HESAPKOD='83001032001' WHERE YIL=2021 AND HESAPKOD='83001030201'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Geçici İşçilerin İhbar ve Kıdem Tazminatları', HESAPKOD='83001032002' WHERE YIL=2021 AND HESAPKOD='83001030202'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='375 S KHK’nın Geçici 23 üncü Md Kapsamında Sürekli İşçi Kadrolarına Geçirilen İşçilerin İhbar ve Kıdem Tazminatları', HESAPKOD='83001032003' WHERE YIL=2021 AND HESAPKOD='83001030203'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='375 S KHK’nın Geçici 23 üncü Md Kapsamında Geçici İşçi Pozisyonlarına Geçirilen İşçilerin İhbar ve Kıdem Tazminatları', HESAPKOD='83001032004' WHERE YIL=2021 AND HESAPKOD='83001030204'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sosyal Haklar', HESAPKOD='830010330' WHERE YIL=2021 AND HESAPKOD='830010303'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sürekli İşçilerin Sosyal Hakları', HESAPKOD='83001033001' WHERE YIL=2021 AND HESAPKOD='83001030301'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Geçici İşçilerin Sosyal Hakları', HESAPKOD='83001033002' WHERE YIL=2021 AND HESAPKOD='83001030302'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='375 S KHK’nın Geçici 23 üncü Md Kapsamında Sürekli İşçi Kadrolarına Geçirilen İşçilerin Sosyal Hakları', HESAPKOD='83001033003' WHERE YIL=2021 AND HESAPKOD='83001030303'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='375 S KHK’nın Geçici 23 üncü Md Kapsamında Geçici İşçi Pozisyonlarına Geçirilen İşçilerin Sosyal Hakları', HESAPKOD='83001033004' WHERE YIL=2021 AND HESAPKOD='83001030304'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Fazla Mesailer', HESAPKOD='830010340' WHERE YIL=2021 AND HESAPKOD='830010304'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sürekli İşçilerin Fazla Mesaileri', HESAPKOD='83001034001' WHERE YIL=2021 AND HESAPKOD='83001030401'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Geçici İşçilerin Fazla Mesaileri', HESAPKOD='83001034002' WHERE YIL=2021 AND HESAPKOD='83001030402'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='375 S KHK’nın Geçici 23 üncü Md Kapsamında Sürekli İşçi Kadrolarına Geçirilen İşçilerin Fazla Mesaileri', HESAPKOD='83001034003' WHERE YIL=2021 AND HESAPKOD='83001030403'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='375 S KHK’nın Geçici 23 üncü Md Kapsamında Geçici İşçi Pozisyonlarına Geçirilen İşçilerin Fazla Mesaileri', HESAPKOD='83001034004' WHERE YIL=2021 AND HESAPKOD='83001030404'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Ödül ve İkramiyeler', HESAPKOD='830010350' WHERE YIL=2021 AND HESAPKOD='830010305'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sürekli İşçilerin Ödül ve İkramiyeleri', HESAPKOD='83001035001' WHERE YIL=2021 AND HESAPKOD='83001030501'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Geçici İşçilerin Ödül ve İkramiyeleri', HESAPKOD='83001035002' WHERE YIL=2021 AND HESAPKOD='83001030502'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='375 S KHK’nın Geçici 23 üncü Md Kapsamında Sürekli İşçi Kadrolarına Geçirilen İşçilerin Ödül ve İkramiyeleri', HESAPKOD='83001035003' WHERE YIL=2021 AND HESAPKOD='83001030503'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='375 S KHK’nın Geçici 23 üncü Md Kapsamında Geçici İşçi Pozisyonlarına Geçirilen İşçilerin Ödül ve İkramiyeleri', HESAPKOD='83001035004' WHERE YIL=2021 AND HESAPKOD='83001030504'
DELETE FROM  MUHHESAPPLANI WHERE YIL=2021 AND HESAPKOD='830010308'
DELETE FROM  MUHHESAPPLANI WHERE YIL=2021 AND HESAPKOD='83001030801'
DELETE FROM  MUHHESAPPLANI WHERE YIL=2021 AND HESAPKOD='83001030802'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Ödemeler', HESAPKOD='830010390' WHERE YIL=2021 AND HESAPKOD='830010309'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sürekli işçilerin Diğer Ödemeleri', HESAPKOD='83001039001' WHERE YIL=2021 AND HESAPKOD='83001030901'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Geçici işçilerin Diğer Ödemeleri', HESAPKOD='83001039002' WHERE YIL=2021 AND HESAPKOD='83001030902'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='375 S KHK’nın Geçici 23 üncü Md Kapsamında Sürekli İşçi Kadrolarına Geçirilen İşçilerin Diğer Ödemeleri', HESAPKOD='83001039003' WHERE YIL=2021 AND HESAPKOD='83001030903'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='375 S KHK’nın Geçici 23 üncü Md Kapsamında Geçici İşçi Pozisyonlarına Geçirilen İşçilerin Diğer Ödemeleri', HESAPKOD='83001039004' WHERE YIL=2021 AND HESAPKOD='83001030904'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Geçici Süreli Çalışanlar', HESAPKOD='8300104' WHERE YIL=2021 AND HESAPKOD='8300104'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Ücretler', HESAPKOD='830010410' WHERE YIL=2021 AND HESAPKOD='830010401'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Vizesiz Geçici İşçilerin Ücretleri', HESAPKOD='83001041001' WHERE YIL=2021 AND HESAPKOD='83001040101'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Aday Çırak, Çırak ve Stajyer Öğrencilerin Ücretleri', HESAPKOD='83001041002' WHERE YIL=2021 AND HESAPKOD='83001040102'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Usta Öğreticilere Yapılacak Ödemeler', HESAPKOD='83001041003' WHERE YIL=2021 AND HESAPKOD='83001040103'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Geçici Süreli Çalışanlara Yapılacak Ödemeler', HESAPKOD='83001041090' WHERE YIL=2021 AND HESAPKOD='83001040104'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kısmi Zamanlı Çalışan Öğrencilerin Ücretleri', HESAPKOD='83001041004' WHERE YIL=2021 AND HESAPKOD='83001040105'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Ders Ücreti Karşılığında Görevlendirilenlerin Ücretleri', HESAPKOD='83001041005' WHERE YIL=2021 AND HESAPKOD='83001040106'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Geçici Süreli Çalışanlara Yapılacak Ödemeler', HESAPKOD='83001041090' WHERE YIL=2021 AND HESAPKOD='83001040190'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Geçici Süreli Çalışanlara Yapılacak Ödemeler', HESAPKOD='83001041090' WHERE YIL=2021 AND HESAPKOD='830010402'
DELETE FROM  MUHHESAPPLANI WHERE YIL=2021 AND HESAPKOD='83001040201'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Geçici Süreli Çalışanlara Yapılacak Ödemeler', HESAPKOD='83001041090' WHERE YIL=2021 AND HESAPKOD='83001040204'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Geçici Süreli Çalışanlara Yapılacak Ödemeler', HESAPKOD='83001041090' WHERE YIL=2021 AND HESAPKOD='830010403'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Geçici Süreli Çalışanlara Yapılacak Ödemeler', HESAPKOD='83001041090' WHERE YIL=2021 AND HESAPKOD='83001040304'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Geçici Süreli Çalışanlara Yapılacak Ödemeler', HESAPKOD='83001041090' WHERE YIL=2021 AND HESAPKOD='830010404'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Geçici Süreli Çalışanlara Yapılacak Ödemeler', HESAPKOD='83001041090' WHERE YIL=2021 AND HESAPKOD='83001040404'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Personel', HESAPKOD='8300105' WHERE YIL=2021 AND HESAPKOD='8300105'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Ücret ve Diğer Ödemeler', HESAPKOD='830010510' WHERE YIL=2021 AND HESAPKOD='830010501'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Muhtarların Ücretleri', HESAPKOD='83001051001' WHERE YIL=2021 AND HESAPKOD='83001050101'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Güvenlik Korucularının Ücretleri', HESAPKOD='83001051002' WHERE YIL=2021 AND HESAPKOD='83001050102'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Er - Erbaş Harçlıkları', HESAPKOD='83001051003' WHERE YIL=2021 AND HESAPKOD='83001050103'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Öğrenci Harçlıkları', HESAPKOD='83001051004' WHERE YIL=2021 AND HESAPKOD='83001050104'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yurtdışı Öğrenimde Ödenen Aylıklar', HESAPKOD='83001051005' WHERE YIL=2021 AND HESAPKOD='83001050105'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Hükümlü Ücretleri', HESAPKOD='83001051006' WHERE YIL=2021 AND HESAPKOD='83001050106'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Personele Yapılan Diğer Ödemeler', HESAPKOD='83001051090' WHERE YIL=2021 AND HESAPKOD='83001050190'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Milletvekilleri', HESAPKOD='8300107' WHERE YIL=2021 AND HESAPKOD='8300107'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Zamlar ve Tazminatlar', HESAPKOD='830010720' WHERE YIL=2021 AND HESAPKOD='830010702'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Zamlar ve Tazminatlar', HESAPKOD='83001072001' WHERE YIL=2021 AND HESAPKOD='83001070201'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Ödenekler', HESAPKOD='830010730' WHERE YIL=2021 AND HESAPKOD='830010703'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Ödenekler', HESAPKOD='83001073001' WHERE YIL=2021 AND HESAPKOD='83001070301'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sosyal Haklar', HESAPKOD='830010740' WHERE YIL=2021 AND HESAPKOD='830010704'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sosyal Haklar', HESAPKOD='83001074001' WHERE YIL=2021 AND HESAPKOD='83001070401'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Cumhurbaşkanlığı Ödeneği', HESAPKOD='8300108' WHERE YIL=2021 AND HESAPKOD='8300108'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Cumhurbaşkanı Ödeneği', HESAPKOD='830010810' WHERE YIL=2021 AND HESAPKOD='830010801'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Cumhurbaşkanı Ödeneği', HESAPKOD='83001081001' WHERE YIL=2021 AND HESAPKOD='83001080101'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='İstihbarat Personeli', HESAPKOD='8300109' WHERE YIL=2021 AND HESAPKOD='8300109'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='İstihbarat Personeli Giderleri', HESAPKOD='830010910' WHERE YIL=2021 AND HESAPKOD='830010901'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='İstihbarat Personeli Giderleri', HESAPKOD='83001091001' WHERE YIL=2021 AND HESAPKOD='83001090101'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sosyal Güvenlik Kurumlarına Devlet Primi Giderleri', HESAPKOD='83002' WHERE YIL=2021 AND HESAPKOD='83002'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Memurlar', HESAPKOD='8300201' WHERE YIL=2021 AND HESAPKOD='8300201'
DELETE FROM  MUHHESAPPLANI WHERE YIL=2021 AND HESAPKOD='830020101'
DELETE FROM  MUHHESAPPLANI WHERE YIL=2021 AND HESAPKOD='83002010101'
DELETE FROM  MUHHESAPPLANI WHERE YIL=2021 AND HESAPKOD='83002010105'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sosyal Güvenlik Kurumuna', HESAPKOD='830020110' WHERE YIL=2021 AND HESAPKOD='830020106'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sosyal Güvenlik Primi Ödemeleri', HESAPKOD='83002011001' WHERE YIL=2021 AND HESAPKOD='83002010601'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sağlık Primi Ödemeleri', HESAPKOD='83002011002' WHERE YIL=2021 AND HESAPKOD='83002010602'
DELETE FROM  MUHHESAPPLANI WHERE YIL=2021 AND HESAPKOD='83002010608'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sözleşmeli Personel', HESAPKOD='8300202' WHERE YIL=2021 AND HESAPKOD='8300202'
DELETE FROM  MUHHESAPPLANI WHERE YIL=2021 AND HESAPKOD='830020201'
DELETE FROM  MUHHESAPPLANI WHERE YIL=2021 AND HESAPKOD='83002020101'
DELETE FROM  MUHHESAPPLANI WHERE YIL=2021 AND HESAPKOD='830020202'
DELETE FROM  MUHHESAPPLANI WHERE YIL=2021 AND HESAPKOD='83002020201'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='İşsizlik Sigortası Fonuna', HESAPKOD='830020240' WHERE YIL=2021 AND HESAPKOD='830020204'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='İşsizlik Sigortası Fonuna', HESAPKOD='83002024001' WHERE YIL=2021 AND HESAPKOD='83002020401'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sosyal Güvenlik Kurumuna', HESAPKOD='830020210' WHERE YIL=2021 AND HESAPKOD='830020206'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sosyal Güvenlik Primi Ödemeleri', HESAPKOD='83002021001' WHERE YIL=2021 AND HESAPKOD='83002020601'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sağlık Primi Ödemeleri', HESAPKOD='83002021002' WHERE YIL=2021 AND HESAPKOD='83002020602'
DELETE FROM  MUHHESAPPLANI WHERE YIL=2021 AND HESAPKOD='83002020608'
DELETE FROM  MUHHESAPPLANI WHERE YIL=2021 AND HESAPKOD='830020209'
DELETE FROM  MUHHESAPPLANI WHERE YIL=2021 AND HESAPKOD='83002020901'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='İşçiler', HESAPKOD='8300203' WHERE YIL=2021 AND HESAPKOD='8300203'
DELETE FROM  MUHHESAPPLANI WHERE YIL=2021 AND HESAPKOD='830020302'
DELETE FROM  MUHHESAPPLANI WHERE YIL=2021 AND HESAPKOD='83002030201'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='İşsizlik Sigortası Fonuna', HESAPKOD='830020340' WHERE YIL=2021 AND HESAPKOD='830020304'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='İşsizlik Sigortası Fonuna', HESAPKOD='83002034001' WHERE YIL=2021 AND HESAPKOD='83002030401'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sosyal Güvenlik Kurumuna', HESAPKOD='830020310' WHERE YIL=2021 AND HESAPKOD='830020306'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sosyal Güvenlik Primi Ödemeleri', HESAPKOD='83002031001' WHERE YIL=2021 AND HESAPKOD='83002030601'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sağlık Primi Ödemeleri', HESAPKOD='83002031002' WHERE YIL=2021 AND HESAPKOD='83002030602'
DELETE FROM  MUHHESAPPLANI WHERE YIL=2021 AND HESAPKOD='83002030608'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Geçici Süreli Çalışanlar', HESAPKOD='8300204' WHERE YIL=2021 AND HESAPKOD='8300204'
DELETE FROM  MUHHESAPPLANI WHERE YIL=2021 AND HESAPKOD='830020401'
DELETE FROM  MUHHESAPPLANI WHERE YIL=2021 AND HESAPKOD='83002040101'
DELETE FROM  MUHHESAPPLANI WHERE YIL=2021 AND HESAPKOD='830020402'
DELETE FROM  MUHHESAPPLANI WHERE YIL=2021 AND HESAPKOD='83002040201'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='İşsizlik Sigortası Fonuna', HESAPKOD='830020440' WHERE YIL=2021 AND HESAPKOD='830020404'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='İşsizlik Sigortası Fonuna', HESAPKOD='83002044001' WHERE YIL=2021 AND HESAPKOD='83002040401'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sosyal Güvenlik Kurumuna', HESAPKOD='830020410' WHERE YIL=2021 AND HESAPKOD='830020406'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sosyal Güvenlik Primi Ödemeleri', HESAPKOD='83002041001' WHERE YIL=2021 AND HESAPKOD='83002040601'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sağlık Primi Ödemeleri', HESAPKOD='83002041002' WHERE YIL=2021 AND HESAPKOD='83002040602'
DELETE FROM  MUHHESAPPLANI WHERE YIL=2021 AND HESAPKOD='83002040608'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Personel', HESAPKOD='8300205' WHERE YIL=2021 AND HESAPKOD='8300205'
DELETE FROM  MUHHESAPPLANI WHERE YIL=2021 AND HESAPKOD='830020501'
DELETE FROM  MUHHESAPPLANI WHERE YIL=2021 AND HESAPKOD='83002050101'
DELETE FROM  MUHHESAPPLANI WHERE YIL=2021 AND HESAPKOD='830020502'
DELETE FROM  MUHHESAPPLANI WHERE YIL=2021 AND HESAPKOD='83002050201'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Özel Sigorta Prim Giderleri', HESAPKOD='830020530' WHERE YIL=2021 AND HESAPKOD='830020503'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Özel Sigorta Prim Giderleri', HESAPKOD='83002053001' WHERE YIL=2021 AND HESAPKOD='83002050301'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='İşsizlik Sigortası Fonuna', HESAPKOD='830020540' WHERE YIL=2021 AND HESAPKOD='830020504'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='İşsizlik Sigortası Fonuna', HESAPKOD='83002054001' WHERE YIL=2021 AND HESAPKOD='83002050401'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sosyal Güvenlik Kurumuna', HESAPKOD='830020510' WHERE YIL=2021 AND HESAPKOD='830020506'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sosyal Güvenlik Primi Ödemeleri', HESAPKOD='83002051001' WHERE YIL=2021 AND HESAPKOD='83002050601'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sağlık Primi Ödemeleri', HESAPKOD='83002051002' WHERE YIL=2021 AND HESAPKOD='83002050602'
DELETE FROM  MUHHESAPPLANI WHERE YIL=2021 AND HESAPKOD='83002050608'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Milletvekilleri', HESAPKOD='8300207' WHERE YIL=2021 AND HESAPKOD='8300207'
DELETE FROM  MUHHESAPPLANI WHERE YIL=2021 AND HESAPKOD='830020701'
DELETE FROM  MUHHESAPPLANI WHERE YIL=2021 AND HESAPKOD='83002070101'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sosyal Güvenlik Kurumuna', HESAPKOD='830020710' WHERE YIL=2021 AND HESAPKOD='830020706'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sosyal Güvenlik Primi Ödemeleri', HESAPKOD='83002071001' WHERE YIL=2021 AND HESAPKOD='83002070601'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sağlık Primi Ödemeleri', HESAPKOD='83002071002' WHERE YIL=2021 AND HESAPKOD='83002070602'
DELETE FROM  MUHHESAPPLANI WHERE YIL=2021 AND HESAPKOD='83002070608'
DELETE FROM  MUHHESAPPLANI WHERE YIL=2021 AND HESAPKOD='8300208'
DELETE FROM  MUHHESAPPLANI WHERE YIL=2021 AND HESAPKOD='830020806'
DELETE FROM  MUHHESAPPLANI WHERE YIL=2021 AND HESAPKOD='83002080601'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='İstihbarat Personeli', HESAPKOD='8300209' WHERE YIL=2021 AND HESAPKOD='8300209'
DELETE FROM  MUHHESAPPLANI WHERE YIL=2021 AND HESAPKOD='830020901'
DELETE FROM  MUHHESAPPLANI WHERE YIL=2021 AND HESAPKOD='83002090101'
DELETE FROM  MUHHESAPPLANI WHERE YIL=2021 AND HESAPKOD='830020902'
DELETE FROM  MUHHESAPPLANI WHERE YIL=2021 AND HESAPKOD='83002090201'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sosyal Güvenlik Kurumuna', HESAPKOD='830020910' WHERE YIL=2021 AND HESAPKOD='830020906'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sosyal Güvenlik Primi Ödemeleri', HESAPKOD='83002091001' WHERE YIL=2021 AND HESAPKOD='83002090601'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sağlık Primi Ödemeleri', HESAPKOD='83002091002' WHERE YIL=2021 AND HESAPKOD='83002090602'
DELETE FROM  MUHHESAPPLANI WHERE YIL=2021 AND HESAPKOD='83002090608'
DELETE FROM  MUHHESAPPLANI WHERE YIL=2021 AND HESAPKOD='8300299'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Mal ve Hizmet Alım Giderleri', HESAPKOD='83003' WHERE YIL=2021 AND HESAPKOD='83003'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Üretime Yönelik Mal ve Malzeme Alımları', HESAPKOD='8300301' WHERE YIL=2021 AND HESAPKOD='8300301'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Hammadde Alımları', HESAPKOD='830030110' WHERE YIL=2021 AND HESAPKOD='830030101'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Hammadde Alımları', HESAPKOD='83003011001' WHERE YIL=2021 AND HESAPKOD='83003010101'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Gıda Ürünleri İçecekler ve Tütün Alımları', HESAPKOD='830030120' WHERE YIL=2021 AND HESAPKOD='830030102'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Gıda Ürünleri İçecekler ve Tütün Alımları', HESAPKOD='83003012001' WHERE YIL=2021 AND HESAPKOD='83003010201'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Tekstil ve Tekstil Ürünleri Deri ve Deri Ürünleri Alımları', HESAPKOD='830030130' WHERE YIL=2021 AND HESAPKOD='830030103'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Tekstil ve Tekstil Ürünleri Deri ve Deri Ürünleri Alımları', HESAPKOD='83003013001' WHERE YIL=2021 AND HESAPKOD='83003010301'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kereste ve Kereste Ürünleri Alımları', HESAPKOD='830030140' WHERE YIL=2021 AND HESAPKOD='830030104'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kereste ve Kereste Ürünleri Alımları', HESAPKOD='83003014001' WHERE YIL=2021 AND HESAPKOD='83003010401'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kağıt ve Kağıt Ürünleri Alımları', HESAPKOD='830030150' WHERE YIL=2021 AND HESAPKOD='830030105'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kağıt ve Kağıt Ürünleri Alımları', HESAPKOD='83003015001' WHERE YIL=2021 AND HESAPKOD='83003010501'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kimyevi Ürün Alımları', HESAPKOD='830030160' WHERE YIL=2021 AND HESAPKOD='830030106'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kimyevi Ürün Alımları', HESAPKOD='83003016001' WHERE YIL=2021 AND HESAPKOD='83003010601'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kauçuk ve Plastik Ürün Alımları', HESAPKOD='830030170' WHERE YIL=2021 AND HESAPKOD='830030107'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kauçuk ve Plastik Ürün Alımları', HESAPKOD='83003017001' WHERE YIL=2021 AND HESAPKOD='83003010701'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Metal Ürünü Alımları', HESAPKOD='830030180' WHERE YIL=2021 AND HESAPKOD='830030108'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Metal Ürünü Alımları', HESAPKOD='83003018001' WHERE YIL=2021 AND HESAPKOD='83003010801'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Üretim Mal ve Malzemesi Alımları', HESAPKOD='830030190' WHERE YIL=2021 AND HESAPKOD='830030109'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Üretim Mal ve Malzemesi Alımları', HESAPKOD='83003019090' WHERE YIL=2021 AND HESAPKOD='83003010901'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Tüketime Yönelik Mal ve Malzeme Alımları', HESAPKOD='8300302' WHERE YIL=2021 AND HESAPKOD='8300302'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kırtasiye, Yayın, Baskı ve Büro Malzemesi Alımları', HESAPKOD='830030210' WHERE YIL=2021 AND HESAPKOD='830030201'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kırtasiye Alımları', HESAPKOD='83003021001' WHERE YIL=2021 AND HESAPKOD='83003020101'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Büro  Malzemesi Alımları', HESAPKOD='83003021002' WHERE YIL=2021 AND HESAPKOD='83003020102'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Periyodik Yayın Alımları', HESAPKOD='83003021003' WHERE YIL=2021 AND HESAPKOD='83003020103'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Yayın Alımları', HESAPKOD='83003021004' WHERE YIL=2021 AND HESAPKOD='83003020104'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Baskı ve Cilt Giderleri', HESAPKOD='83003021005' WHERE YIL=2021 AND HESAPKOD='83003020105'
DELETE FROM  MUHHESAPPLANI WHERE YIL=2021 AND HESAPKOD='83003020108'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Kırtasiye, Yayın, Baskı ve Büro Malzemesi Alımları', HESAPKOD='83003021090' WHERE YIL=2021 AND HESAPKOD='83003020190'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Su ve Temizlik Malzemesi Alımları', HESAPKOD='830030220' WHERE YIL=2021 AND HESAPKOD='830030202'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Su Alımları', HESAPKOD='83003022001' WHERE YIL=2021 AND HESAPKOD='83003020201'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Temizlik Malzemesi Alımları', HESAPKOD='83003022002' WHERE YIL=2021 AND HESAPKOD='83003020202'
DELETE FROM  MUHHESAPPLANI WHERE YIL=2021 AND HESAPKOD='83003020280'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Enerji Alımları', HESAPKOD='830030230' WHERE YIL=2021 AND HESAPKOD='830030203'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yakacak  Alımları', HESAPKOD='83003023001' WHERE YIL=2021 AND HESAPKOD='83003020301'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Akaryakıt ve Yağ  Alımları', HESAPKOD='83003023002' WHERE YIL=2021 AND HESAPKOD='83003020302'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Elektrik Alımları', HESAPKOD='83003023003' WHERE YIL=2021 AND HESAPKOD='83003020303'
DELETE FROM  MUHHESAPPLANI WHERE YIL=2021 AND HESAPKOD='83003020380'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Enerji Alımları', HESAPKOD='83003023090' WHERE YIL=2021 AND HESAPKOD='83003020390'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yiyecek, İçecek ve Yem Alımları', HESAPKOD='830030240' WHERE YIL=2021 AND HESAPKOD='830030204'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yiyecek Alımları ', HESAPKOD='83003024001' WHERE YIL=2021 AND HESAPKOD='83003020401'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='İçecek Alımları', HESAPKOD='83003024002' WHERE YIL=2021 AND HESAPKOD='83003020402'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yem Alımları', HESAPKOD='83003024003' WHERE YIL=2021 AND HESAPKOD='83003020403'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Yiyecek, İçecek ve Yem Alımları', HESAPKOD='83003024090' WHERE YIL=2021 AND HESAPKOD='83003020490'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Giyim ve Kuşam Alımları', HESAPKOD='830030250' WHERE YIL=2021 AND HESAPKOD='830030205'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Giyecek Alımları  ', HESAPKOD='83003025001' WHERE YIL=2021 AND HESAPKOD='83003020501'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Spor Malzemeleri Alımları', HESAPKOD='83003025002' WHERE YIL=2021 AND HESAPKOD='83003020502'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Tören Malzemeleri Alımları', HESAPKOD='83003025003' WHERE YIL=2021 AND HESAPKOD='83003020503'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Bando Malzemeleri Alımları', HESAPKOD='83003025004' WHERE YIL=2021 AND HESAPKOD='83003020504'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kuşam Alımları  ', HESAPKOD='83003025005' WHERE YIL=2021 AND HESAPKOD='83003020505'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Giyim ve Kuşam Alımları', HESAPKOD='83003025090' WHERE YIL=2021 AND HESAPKOD='83003020590'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Özel Malzeme Alımları', HESAPKOD='830030260' WHERE YIL=2021 AND HESAPKOD='830030206'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Laboratuvar Malzemesi ile Kimyevi ve Temrinlik Malzeme Alımları', HESAPKOD='83003026001' WHERE YIL=2021 AND HESAPKOD='83003020601'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Tıbbi Malzeme ve İlaç Alımları', HESAPKOD='83003026002' WHERE YIL=2021 AND HESAPKOD='83003020602'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Zirai Malzeme ve İlaç Alımları', HESAPKOD='83003026003' WHERE YIL=2021 AND HESAPKOD='83003020603'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Canlı Hayvan Alım, Bakım ve Diğer Giderleri', HESAPKOD='83003026004' WHERE YIL=2021 AND HESAPKOD='83003020604'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Özel Malzeme Alımları', HESAPKOD='83003026090' WHERE YIL=2021 AND HESAPKOD='83003020690'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Güvenlik ve Savunmaya Yönelik Mal, Malzeme ve Hizmet Alımları, Yapımları ve Giderleri', HESAPKOD='830030270' WHERE YIL=2021 AND HESAPKOD='830030207'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Güvenlik ve Savunmaya Yönelik Silah, Araç, Gereç ve Savaş Teçhizatı Alımları', HESAPKOD='83003027001' WHERE YIL=2021 AND HESAPKOD='83003020701'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Güvenlik ve Savunmaya Yönelik Silah, Araç, Gereç ve Savaş Teçhizatı İşletme, Bakım ve İdame Giderleri', HESAPKOD='83003027002' WHERE YIL=2021 AND HESAPKOD='83003020702'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Mühimmat Alımları', HESAPKOD='83003027003' WHERE YIL=2021 AND HESAPKOD='83003020703'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Güvenlik ve Savunmaya Yönelik Araştırma-Geliştirme Giderleri', HESAPKOD='83003027004' WHERE YIL=2021 AND HESAPKOD='83003020704'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Güvenlik ve Savunmaya Yönelik Hizmet Alım Giderleri', HESAPKOD='83003027005' WHERE YIL=2021 AND HESAPKOD='83003020705'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Savunma Projeleri ve Acil İhtiyaç Giderleri', HESAPKOD='83003027006' WHERE YIL=2021 AND HESAPKOD='83003020706'
DELETE FROM  MUHHESAPPLANI WHERE YIL=2021 AND HESAPKOD='83003020708'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Güvenlik ve Savunmaya Yönelik Diğer Giderler', HESAPKOD='83003027009' WHERE YIL=2021 AND HESAPKOD='83003020709'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Güvenlik ve Savunmaya Yönelik Makine-Teçhizat Alımları', HESAPKOD='83003027011' WHERE YIL=2021 AND HESAPKOD='83003020711'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Güvenlik ve Savunmaya Yönelik Makine-Teçhizat Büyük Onarımları', HESAPKOD='83003027012' WHERE YIL=2021 AND HESAPKOD='83003020712'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Güvenlik ve Savunmaya Yönelik Gayrimenkul Yapım Giderleri', HESAPKOD='83003027021' WHERE YIL=2021 AND HESAPKOD='83003020721'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Güvenlik ve Savunmaya Yönelik Gayrimenkul Büyük Onarım Giderleri', HESAPKOD='83003027022' WHERE YIL=2021 AND HESAPKOD='83003020722'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='NATO Altyapısına İlişkin Gayrimenkul Yapım Giderleri', HESAPKOD='83003027031' WHERE YIL=2021 AND HESAPKOD='83003020731'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='NATO Altyapısına İlişkin Gayrimenkul Büyük Onarım Giderleri', HESAPKOD='83003027032' WHERE YIL=2021 AND HESAPKOD='83003020732'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Savunma Mal ve  Malzeme Alımları ve Yapımları', HESAPKOD='83003027090' WHERE YIL=2021 AND HESAPKOD='83003020790'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='NATO Giderleri İle Gayrimenkul Alım ve Kamulaştırma Giderleri', HESAPKOD='830030280' WHERE YIL=2021 AND HESAPKOD='830030208'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Gayrimenkul Alım ve Kamulaştırma Giderleri', HESAPKOD='83003028001' WHERE YIL=2021 AND HESAPKOD='83003020801'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='NATO Altyapısına İlişkin Gayrimenkul Alım ve Kamulaştırma Giderleri', HESAPKOD='83003028002' WHERE YIL=2021 AND HESAPKOD='83003020802'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='NATO Giderleri', HESAPKOD='83003028003' WHERE YIL=2021 AND HESAPKOD='83003020803'
DELETE FROM  MUHHESAPPLANI WHERE YIL=2021 AND HESAPKOD='83003020890'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Tüketim Mal ve Malzemesi Alımları', HESAPKOD='830030290' WHERE YIL=2021 AND HESAPKOD='830030209'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Bahçe Malzemesi Alımları ile Yapım ve Bakım Giderleri', HESAPKOD='83003029001' WHERE YIL=2021 AND HESAPKOD='83003020901'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Tüketim Mal ve Malzemesi Alımları', HESAPKOD='83003029090' WHERE YIL=2021 AND HESAPKOD='83003020990'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yolluklar', HESAPKOD='8300303' WHERE YIL=2021 AND HESAPKOD='8300303'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Geçici Görev Yollukları', HESAPKOD='830030310' WHERE YIL=2021 AND HESAPKOD='830030301'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yurtiçi Geçici Görev Yollukları', HESAPKOD='83003031001' WHERE YIL=2021 AND HESAPKOD='83003030101'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yurtiçi Tedavi Yollukları', HESAPKOD='83003031002' WHERE YIL=2021 AND HESAPKOD='83003030102'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sürekli Görev Yollukları', HESAPKOD='830030320' WHERE YIL=2021 AND HESAPKOD='830030302'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yurtiçi Sürekli Görev Yollukları', HESAPKOD='83003032001' WHERE YIL=2021 AND HESAPKOD='83003030201'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Geçici Görev Yollukları', HESAPKOD='830030310' WHERE YIL=2021 AND HESAPKOD='830030303'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yurtdışı Geçici Görev Yollukları', HESAPKOD='83003031003' WHERE YIL=2021 AND HESAPKOD='83003030301'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yurtdışı Tedavi Yollukları', HESAPKOD='83003031004' WHERE YIL=2021 AND HESAPKOD='83003030302'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sürekli Görev Yollukları', HESAPKOD='830030320' WHERE YIL=2021 AND HESAPKOD='830030304'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yurtdışı Sürekli Görev Yollukları', HESAPKOD='83003032002' WHERE YIL=2021 AND HESAPKOD='83003030401'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yolluk Tazminatları', HESAPKOD='830030330' WHERE YIL=2021 AND HESAPKOD='830030305'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Seyyar Görev Tazminatları', HESAPKOD='83003033001' WHERE YIL=2021 AND HESAPKOD='83003030501'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Arazi Tazminatları', HESAPKOD='83003033002' WHERE YIL=2021 AND HESAPKOD='83003030502'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Yolluklar', HESAPKOD='830030390' WHERE YIL=2021 AND HESAPKOD='830030306'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Profesör Uzman ve Memur Mübadele Giderleri', HESAPKOD='83003039001' WHERE YIL=2021 AND HESAPKOD='83003030601'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Öğrenci Mübadele Giderleri', HESAPKOD='83003039002' WHERE YIL=2021 AND HESAPKOD='83003030602'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Görev Giderleri', HESAPKOD='8300304' WHERE YIL=2021 AND HESAPKOD='8300304'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Tahliye Giderleri', HESAPKOD='830030410' WHERE YIL=2021 AND HESAPKOD='830030401'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yurtdışı Tahliye Giderleri', HESAPKOD='83003041001' WHERE YIL=2021 AND HESAPKOD='83003040101'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Afet Bölgesi Tahliye Giderleri', HESAPKOD='83003041002' WHERE YIL=2021 AND HESAPKOD='83003040102'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Mülteci Tahliye Giderleri', HESAPKOD='83003041003' WHERE YIL=2021 AND HESAPKOD='83003040103'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Tahliye Giderleri', HESAPKOD='83003041090' WHERE YIL=2021 AND HESAPKOD='83003040190'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yasal Giderler', HESAPKOD='830030420' WHERE YIL=2021 AND HESAPKOD='830030402'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Beyiye Aidatları', HESAPKOD='83003046001' WHERE YIL=2021 AND HESAPKOD='83003040201'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Oranı Kanunla Belirlenmiş Aidat ve İkramiyeler ', HESAPKOD='83003042001' WHERE YIL=2021 AND HESAPKOD='83003040202'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kusursuz Tazminatlar', HESAPKOD='83003042002' WHERE YIL=2021 AND HESAPKOD='83003040203'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Mahkeme Harç ve Giderleri', HESAPKOD='83003047001' WHERE YIL=2021 AND HESAPKOD='83003040204'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Ödül, İkramiye ve Benzeri Ödemeler ', HESAPKOD='83003042003' WHERE YIL=2021 AND HESAPKOD='83003040205'
DELETE FROM  MUHHESAPPLANI WHERE YIL=2021 AND HESAPKOD='83003040206'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Yasal Giderler', HESAPKOD='83003042090' WHERE YIL=2021 AND HESAPKOD='83003040290'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Ödenecek Vergi, Resim,  Harçlar ve Benzeri Giderler', HESAPKOD='830030430' WHERE YIL=2021 AND HESAPKOD='830030403'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Vergi Ödemeleri ve Benzeri Giderler', HESAPKOD='83003043001' WHERE YIL=2021 AND HESAPKOD='83003040301'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='İşletme Ruhsatı Ödemeleri ve Benzeri Giderler', HESAPKOD='83003043002' WHERE YIL=2021 AND HESAPKOD='83003040302'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Vergi, Resim ve Harçlar ve Benzeri Giderler', HESAPKOD='83003043090' WHERE YIL=2021 AND HESAPKOD='83003040390'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kültür Varlıkları Alımı ve Korunması Giderleri', HESAPKOD='830030440' WHERE YIL=2021 AND HESAPKOD='830030404'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Arkeolojik Kazı Giderleri', HESAPKOD='83003044001' WHERE YIL=2021 AND HESAPKOD='83003040401'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Restorasyon ve Yenileme Giderleri', HESAPKOD='83003044002' WHERE YIL=2021 AND HESAPKOD='83003040402'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kültür Varlıkları Alımı', HESAPKOD='83003044003' WHERE YIL=2021 AND HESAPKOD='83003040403'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sergi Giderleri', HESAPKOD='83003044004' WHERE YIL=2021 AND HESAPKOD='83003040404'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kültür Varlıklarının Korunmasına İlişkin Diğer Giderler', HESAPKOD='83003044090' WHERE YIL=2021 AND HESAPKOD='83003040490'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Gizli Hizmet Giderleri', HESAPKOD='830030450' WHERE YIL=2021 AND HESAPKOD='830030405'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Gizli Hizmet Giderleri', HESAPKOD='83003045001' WHERE YIL=2021 AND HESAPKOD='83003040501'
DELETE FROM  MUHHESAPPLANI WHERE YIL=2021 AND HESAPKOD='83003040502'
DELETE FROM  MUHHESAPPLANI WHERE YIL=2021 AND HESAPKOD='830030408'
DELETE FROM  MUHHESAPPLANI WHERE YIL=2021 AND HESAPKOD='83003040890'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Görev Giderleri', HESAPKOD='830030490' WHERE YIL=2021 AND HESAPKOD='830030409'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Karantina Giderleri', HESAPKOD='83003049001' WHERE YIL=2021 AND HESAPKOD='83003040901'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Güvenlik Kuvvetleri Nezaretinde Bulundurma Giderleri', HESAPKOD='83003049002' WHERE YIL=2021 AND HESAPKOD='83003040902'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yurtdışına Çıkarma ve Yurtiçinde Yer Değiştirme Giderleri', HESAPKOD='83003049003' WHERE YIL=2021 AND HESAPKOD='83003040903'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Mültecilerin Güvenlik Kuvvetleri Nezaretinde Bulundurulma Giderleri', HESAPKOD='83003049004' WHERE YIL=2021 AND HESAPKOD='83003040904'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Barışı Destekleme ve Koruma Harekat Giderleri', HESAPKOD='83003049005' WHERE YIL=2021 AND HESAPKOD='83003040905'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='375 Sayılı Kanun Hükmünde Kararname Kapsamında Yüklenicilere Yapılacak Tazminat Ödemeleri', HESAPKOD='83003049006' WHERE YIL=2021 AND HESAPKOD='83003040906'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sınıflandırmaya Girmeyen Diğer Görev Giderleri', HESAPKOD='83003049090' WHERE YIL=2021 AND HESAPKOD='83003040990'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Hizmet Alımları', HESAPKOD='8300305' WHERE YIL=2021 AND HESAPKOD='8300305'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Genel Hizmet Alımları ', HESAPKOD='830030510' WHERE YIL=2021 AND HESAPKOD='830030501'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Etüt-Proje Bilirkişi Ekspertiz Giderleri', HESAPKOD='83003051001' WHERE YIL=2021 AND HESAPKOD='83003050101'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Araştırma ve Geliştirme Hizmet Alım Giderleri', HESAPKOD='83003051002' WHERE YIL=2021 AND HESAPKOD='83003050102'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Bilgisayar Hizmet Alım Giderleri', HESAPKOD='83003051003' WHERE YIL=2021 AND HESAPKOD='83003050103'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Genel Hizmet Alımları', HESAPKOD='83003051090' WHERE YIL=2021 AND HESAPKOD='83003050104'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Harita Yapım ve Alım Giderleri', HESAPKOD='83003051004' WHERE YIL=2021 AND HESAPKOD='83003050105'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Enformasyon ve Raporlama Hizmet Alım Giderleri', HESAPKOD='83003051005' WHERE YIL=2021 AND HESAPKOD='83003050106'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Danışma Yönetim ve İşletim Hizmet Alım Giderleri', HESAPKOD='83003051006' WHERE YIL=2021 AND HESAPKOD='83003050107'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Genel Hizmet Alımları', HESAPKOD='83003051090' WHERE YIL=2021 AND HESAPKOD='83003050108'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Genel Hizmet Alımları', HESAPKOD='83003051090' WHERE YIL=2021 AND HESAPKOD='83003050109'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='İş Sağlığı ve Güvenliği Hizmeti Alım Giderleri', HESAPKOD='83003051007' WHERE YIL=2021 AND HESAPKOD='83003050110'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Genel Hizmet Alımları', HESAPKOD='83003051090' WHERE YIL=2021 AND HESAPKOD='83003050111'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Çağrı Merkezi Hizmeti Alım Giderleri', HESAPKOD='83003051008' WHERE YIL=2021 AND HESAPKOD='83003050112'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Genel Hizmet Alımları', HESAPKOD='83003051090' WHERE YIL=2021 AND HESAPKOD='83003050190'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Haberleşme Giderleri', HESAPKOD='830030520' WHERE YIL=2021 AND HESAPKOD='830030502'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Posta ve Telgraf Giderleri', HESAPKOD='83003052001' WHERE YIL=2021 AND HESAPKOD='83003050201'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Telefon Abonelik ve Kullanım Ücretleri', HESAPKOD='83003052002' WHERE YIL=2021 AND HESAPKOD='83003050202'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='İnternet Erişimi Giderleri', HESAPKOD='83003052003' WHERE YIL=2021 AND HESAPKOD='83003050203'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Haberleşme Cihazları Ruhsat ve Kullanım Giderleri', HESAPKOD='83003052004' WHERE YIL=2021 AND HESAPKOD='83003050204'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Uydu Haberleşme Giderleri', HESAPKOD='83003052005' WHERE YIL=2021 AND HESAPKOD='83003050205'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Hat Kira Giderleri', HESAPKOD='83003052006' WHERE YIL=2021 AND HESAPKOD='83003050206'
DELETE FROM  MUHHESAPPLANI WHERE YIL=2021 AND HESAPKOD='83003050280'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Haberleşme Giderleri', HESAPKOD='83003052090' WHERE YIL=2021 AND HESAPKOD='83003050290'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Taşıma Giderleri', HESAPKOD='830030530' WHERE YIL=2021 AND HESAPKOD='830030503'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Taşımaya İlişkin Beslenme, Barındırma Giderleri', HESAPKOD='83003053001' WHERE YIL=2021 AND HESAPKOD='83003050301'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yolcu Taşıma Giderleri', HESAPKOD='83003053002' WHERE YIL=2021 AND HESAPKOD='83003050302'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yük Taşıma Giderleri', HESAPKOD='83003053003' WHERE YIL=2021 AND HESAPKOD='83003050303'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Geçiş Ücretleri', HESAPKOD='83003053004' WHERE YIL=2021 AND HESAPKOD='83003050304'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Taşıma Giderleri', HESAPKOD='83003053090' WHERE YIL=2021 AND HESAPKOD='83003050390'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Tarifeye Bağlı Ödemeler', HESAPKOD='830030540' WHERE YIL=2021 AND HESAPKOD='830030504'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='İlan Giderleri', HESAPKOD='83003054001' WHERE YIL=2021 AND HESAPKOD='83003050401'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sigorta Giderleri', HESAPKOD='83003054002' WHERE YIL=2021 AND HESAPKOD='83003050402'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Komisyon Giderleri', HESAPKOD='83003054003' WHERE YIL=2021 AND HESAPKOD='83003050403'
DELETE FROM  MUHHESAPPLANI WHERE YIL=2021 AND HESAPKOD='83003050404'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Tarifeye Bağlı Ödemeler', HESAPKOD='83003054090' WHERE YIL=2021 AND HESAPKOD='83003050490'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kiralar', HESAPKOD='830030550' WHERE YIL=2021 AND HESAPKOD='830030505'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Dayanıklı Mal ve Malzeme Kiralama Giderleri', HESAPKOD='83003055001' WHERE YIL=2021 AND HESAPKOD='83003050501'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Taşıt Kiralama Giderleri', HESAPKOD='83003055002' WHERE YIL=2021 AND HESAPKOD='83003050502'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='İş Makinası Kiralama Giderleri', HESAPKOD='83003055003' WHERE YIL=2021 AND HESAPKOD='83003050503'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Canlı Hayvan Kiralama Giderleri', HESAPKOD='83003055004' WHERE YIL=2021 AND HESAPKOD='83003050504'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Hizmet Binası Kiralama Giderleri', HESAPKOD='83003055005' WHERE YIL=2021 AND HESAPKOD='83003050505'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Lojman Kiralama Giderleri', HESAPKOD='83003055006' WHERE YIL=2021 AND HESAPKOD='83003050506'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Arsa ve Arazi Kiralama Giderleri', HESAPKOD='83003055007' WHERE YIL=2021 AND HESAPKOD='83003050507'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yüzer Taşıt Kiralama Giderleri', HESAPKOD='83003055008' WHERE YIL=2021 AND HESAPKOD='83003050508'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Hava Taşıtı Kiralama Giderleri', HESAPKOD='83003055009' WHERE YIL=2021 AND HESAPKOD='83003050509'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Bilgisayar, Bilgisayar Sistemleri ve Yazılımları Kiralama Giderleri', HESAPKOD='83003055010' WHERE YIL=2021 AND HESAPKOD='83003050510'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Tersane Kiralama Giderleri', HESAPKOD='83003055011' WHERE YIL=2021 AND HESAPKOD='83003050511'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Personel Servisi Kiralama Giderleri', HESAPKOD='83003055012' WHERE YIL=2021 AND HESAPKOD='83003050512'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Kiralama Giderleri', HESAPKOD='83003055090' WHERE YIL=2021 AND HESAPKOD='83003050590'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Devlet Borçları Genel Giderleri', HESAPKOD='830030560' WHERE YIL=2021 AND HESAPKOD='830030506'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='İç Borçlanma Hazine Bonosu Genel Giderleri', HESAPKOD='83003056001' WHERE YIL=2021 AND HESAPKOD='83003050601'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='İç Borçlanma Devlet Tahvili Genel  Giderleri', HESAPKOD='83003056002' WHERE YIL=2021 AND HESAPKOD='83003050602'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Dış Borçlanma Genel Giderleri', HESAPKOD='83003056003' WHERE YIL=2021 AND HESAPKOD='83003050603'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Hazine İşlemleri Diğer Genel Giderleri', HESAPKOD='83003056004' WHERE YIL=2021 AND HESAPKOD='83003050604'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yargılama Giderleri', HESAPKOD='830030570' WHERE YIL=2021 AND HESAPKOD='830030507'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Adli Yardım Giderleri', HESAPKOD='83003057001' WHERE YIL=2021 AND HESAPKOD='83003050701'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Keşif Giderleri', HESAPKOD='83003057002' WHERE YIL=2021 AND HESAPKOD='83003050702'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Rapor ve Bilirkişi Giderleri', HESAPKOD='83003057003' WHERE YIL=2021 AND HESAPKOD='83003050703'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Uzlaşma Giderleri', HESAPKOD='83003057004' WHERE YIL=2021 AND HESAPKOD='83003050704'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Arabuluculuk Ücreti Giderleri', HESAPKOD='83003057005' WHERE YIL=2021 AND HESAPKOD='83003050705'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Yargılama Giderleri', HESAPKOD='83003057090' WHERE YIL=2021 AND HESAPKOD='83003050790'
DELETE FROM  MUHHESAPPLANI WHERE YIL=2021 AND HESAPKOD='830030508'
DELETE FROM  MUHHESAPPLANI WHERE YIL=2021 AND HESAPKOD='83003050801'
DELETE FROM  MUHHESAPPLANI WHERE YIL=2021 AND HESAPKOD='83003050802'
DELETE FROM  MUHHESAPPLANI WHERE YIL=2021 AND HESAPKOD='83003050803'
DELETE FROM  MUHHESAPPLANI WHERE YIL=2021 AND HESAPKOD='83003050804'
DELETE FROM  MUHHESAPPLANI WHERE YIL=2021 AND HESAPKOD='83003050810'
DELETE FROM  MUHHESAPPLANI WHERE YIL=2021 AND HESAPKOD='83003050811'
DELETE FROM  MUHHESAPPLANI WHERE YIL=2021 AND HESAPKOD='83003050890'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Hizmet Alımları', HESAPKOD='830030590' WHERE YIL=2021 AND HESAPKOD='830030509'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yurtiçi Staj ve Öğrenim Giderleri', HESAPKOD='83003059001' WHERE YIL=2021 AND HESAPKOD='83003050901'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yurtdışı Staj ve Öğrenim Giderleri', HESAPKOD='83003059002' WHERE YIL=2021 AND HESAPKOD='83003050902'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kurslara Katılma ve Eğitim Giderleri', HESAPKOD='83003059003' WHERE YIL=2021 AND HESAPKOD='83003050903'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Öğretim Üyesi Yetiştirme Projesi Giderleri', HESAPKOD='83003059004' WHERE YIL=2021 AND HESAPKOD='83003050904'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Lojman İşletme Maliyetlerine Katılım Giderleri ', HESAPKOD='83003059005' WHERE YIL=2021 AND HESAPKOD='83003050910'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Binaların İşletme Maliyetlerine Katılım Giderleri ', HESAPKOD='83003059006' WHERE YIL=2021 AND HESAPKOD='83003050911'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sınıflandırmaya Girmeyen Diğer Hizmet Alımları', HESAPKOD='83003059090' WHERE YIL=2021 AND HESAPKOD='83003050990'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Temsil ve Tanıtma Giderleri', HESAPKOD='8300306' WHERE YIL=2021 AND HESAPKOD='8300306'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Temsil, Tanıtma ve Ağırlama Giderleri', HESAPKOD='830030610' WHERE YIL=2021 AND HESAPKOD='830030601'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Temsil, Tanıtma ve Ağırlama Giderleri', HESAPKOD='83003061001' WHERE YIL=2021 AND HESAPKOD='83003060101'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Toplantı ve Organizasyon Giderleri', HESAPKOD='830030620' WHERE YIL=2021 AND HESAPKOD='830030602'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Toplantı ve Organizasyon Giderleri', HESAPKOD='83003062001' WHERE YIL=2021 AND HESAPKOD='83003060201'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Menkul Mal, Gayrimaddi Hak Alım, Bakım ve Onarım Giderleri', HESAPKOD='8300307' WHERE YIL=2021 AND HESAPKOD='8300307'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Menkul Mal  Alım Giderleri', HESAPKOD='830030710' WHERE YIL=2021 AND HESAPKOD='830030701'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Büro ve İşyeri Mal ve Malzeme Alımları', HESAPKOD='83003071001' WHERE YIL=2021 AND HESAPKOD='83003070101'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Büro ve İşyeri Makine ve Techizat Alımları', HESAPKOD='83003071002' WHERE YIL=2021 AND HESAPKOD='83003070102'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Avadanlık ve Yedek Parça Alımları ', HESAPKOD='83003071003' WHERE YIL=2021 AND HESAPKOD='83003070103'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yangından Korunma Malzemeleri Alımları', HESAPKOD='83003071004' WHERE YIL=2021 AND HESAPKOD='83003070104'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Dayanıklı Mal ve Malzeme Alımları', HESAPKOD='83003071090' WHERE YIL=2021 AND HESAPKOD='83003070190'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Gayri Maddi Hak Alımları', HESAPKOD='830030720' WHERE YIL=2021 AND HESAPKOD='830030702'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Bilgisayar Yazılım Alımları ve Yapımları', HESAPKOD='83003072001' WHERE YIL=2021 AND HESAPKOD='83003070201'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Fikri Hak Alımları', HESAPKOD='83003072002' WHERE YIL=2021 AND HESAPKOD='83003070202'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Gayri Maddi Hak Alımları', HESAPKOD='83003072090' WHERE YIL=2021 AND HESAPKOD='83003070290'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Bakım ve Onarım Giderleri', HESAPKOD='830030730' WHERE YIL=2021 AND HESAPKOD='830030703'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Tefrişat Bakım ve Onarım Giderleri', HESAPKOD='83003073001' WHERE YIL=2021 AND HESAPKOD='83003070301'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Makine Teçhizat Bakım ve Onarım Giderleri', HESAPKOD='83003073002' WHERE YIL=2021 AND HESAPKOD='83003070302'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Taşıt Bakım ve Onarım Giderleri', HESAPKOD='83003073003' WHERE YIL=2021 AND HESAPKOD='83003070303'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='İş Makinası Onarım Giderleri', HESAPKOD='83003073004' WHERE YIL=2021 AND HESAPKOD='83003070304'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Bakım ve Onarım Giderleri', HESAPKOD='83003073090' WHERE YIL=2021 AND HESAPKOD='83003070390'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Gayrimenkul Mal Bakım ve Onarım Giderleri', HESAPKOD='8300308' WHERE YIL=2021 AND HESAPKOD='8300308'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Hizmet Binası  Bakım ve Onarım Giderleri', HESAPKOD='830030810' WHERE YIL=2021 AND HESAPKOD='830030801'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Büro Bakım ve Onarımı Giderleri', HESAPKOD='83003081001' WHERE YIL=2021 AND HESAPKOD='83003080101'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Eğitim ve Öğretim Tesisi Bakım ve Onarımı Giderleri', HESAPKOD='83003081002' WHERE YIL=2021 AND HESAPKOD='83003080102'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Hastane Bakım ve Onarımı Giderleri', HESAPKOD='83003081003' WHERE YIL=2021 AND HESAPKOD='83003080103'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Atölye ve Tesis Binaları Bakım ve Onarımı Giderleri', HESAPKOD='83003081004' WHERE YIL=2021 AND HESAPKOD='83003080104'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Hizmet Binası  Bakım ve Onarım Giderleri', HESAPKOD='83003081090' WHERE YIL=2021 AND HESAPKOD='83003080190'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Lojman  Bakım ve Onarımı Giderleri', HESAPKOD='830030820' WHERE YIL=2021 AND HESAPKOD='830030802'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Lojman Bakım ve Onarımı Giderleri', HESAPKOD='83003082001' WHERE YIL=2021 AND HESAPKOD='83003080201'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sosyal Tesis  Bakım ve Onarımı Giderleri', HESAPKOD='830030830' WHERE YIL=2021 AND HESAPKOD='830030803'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sosyal Tesis  Bakım ve Onarımı Giderleri', HESAPKOD='83003083001' WHERE YIL=2021 AND HESAPKOD='83003080301'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Gemi  Bakım ve Onarımı Giderleri', HESAPKOD='830030840' WHERE YIL=2021 AND HESAPKOD='830030804'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Gemi  Bakım ve Onarımı Giderleri', HESAPKOD='83003084001' WHERE YIL=2021 AND HESAPKOD='83003080401'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Tersane Bakım ve Onarımı Giderleri', HESAPKOD='830030850' WHERE YIL=2021 AND HESAPKOD='830030805'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Tersane Bakım ve Onarımı Giderleri', HESAPKOD='83003085001' WHERE YIL=2021 AND HESAPKOD='83003080501'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yüzer Tersane Bakım ve Onarım Giderleri', HESAPKOD='83003085002' WHERE YIL=2021 AND HESAPKOD='83003080502'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yol  Bakım ve Onarımı Giderleri', HESAPKOD='830030860' WHERE YIL=2021 AND HESAPKOD='830030806'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Bitüm Alımları', HESAPKOD='83003086001' WHERE YIL=2021 AND HESAPKOD='83003080601'
DELETE FROM  MUHHESAPPLANI WHERE YIL=2021 AND HESAPKOD='830030808'
DELETE FROM  MUHHESAPPLANI WHERE YIL=2021 AND HESAPKOD='83003080801'
DELETE FROM  MUHHESAPPLANI WHERE YIL=2021 AND HESAPKOD='83003080890'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Taşınmaz Yapım, Bakım ve Onarım Giderleri', HESAPKOD='830030890' WHERE YIL=2021 AND HESAPKOD='830030809'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Taşınmaz Yapım, Bakım ve Onarım Giderleri', HESAPKOD='83003089001' WHERE YIL=2021 AND HESAPKOD='83003080901'
DELETE FROM  MUHHESAPPLANI WHERE YIL=2021 AND HESAPKOD='83003080990'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Tedavi ve Cenaze Giderleri', HESAPKOD='8300309' WHERE YIL=2021 AND HESAPKOD='8300309'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kamu Personeli Tedavi ve Sağlık Malzemesi Giderleri', HESAPKOD='830030910' WHERE YIL=2021 AND HESAPKOD='830030901'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kamu Personeli Tedavi ve Sağlık Malzemesi Giderleri', HESAPKOD='83003091001' WHERE YIL=2021 AND HESAPKOD='83003090101'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kamu Personeli İlaç Giderleri', HESAPKOD='830030920' WHERE YIL=2021 AND HESAPKOD='830030902'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kamu Personeli İlaç Giderleri', HESAPKOD='83003092001' WHERE YIL=2021 AND HESAPKOD='83003090201'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Cenaze Giderleri', HESAPKOD='830030930' WHERE YIL=2021 AND HESAPKOD='830030903'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Cenaze Giderleri', HESAPKOD='83003093001' WHERE YIL=2021 AND HESAPKOD='83003090301'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Mezar ve Şehitlik Yapım ve Bakım Giderleri', HESAPKOD='83003093002' WHERE YIL=2021 AND HESAPKOD='83003090302'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Milletvekili Tedavi ve Sağlık Malzemesi Giderleri', HESAPKOD='830030940' WHERE YIL=2021 AND HESAPKOD='830030906'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Milletvekili Tedavi ve Sağlık Malzemesi Giderleri', HESAPKOD='83003094001' WHERE YIL=2021 AND HESAPKOD='83003090601'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Milletvekili İlaç Giderleri', HESAPKOD='830030950' WHERE YIL=2021 AND HESAPKOD='830030907'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Milletvekili İlaç Giderleri', HESAPKOD='83003095001' WHERE YIL=2021 AND HESAPKOD='83003090701'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Tedavi ve Sağlık Malzemesi Giderleri', HESAPKOD='830030980' WHERE YIL=2021 AND HESAPKOD='830030908'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Öğrenci  Tedavi ve Sağlık Malzemesi Giderleri', HESAPKOD='83003098001' WHERE YIL=2021 AND HESAPKOD='83003090801'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Er ve Erbaş Tedavi ve Sağlık Malzemesi Giderleri', HESAPKOD='83003098002' WHERE YIL=2021 AND HESAPKOD='83003090802'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Tutuklu ve Hükümlülerin Tedavi ve Sağlık Malzemesi Giderleri', HESAPKOD='83003098003' WHERE YIL=2021 AND HESAPKOD='83003090803'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sosyal Güvenliği Bulunmayanların Tedavi ve Sağlık Malzemesi Giderleri', HESAPKOD='83003098004' WHERE YIL=2021 AND HESAPKOD='83003090804'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sınıflandırılmayan Diğer Tedavi ve Sağlık Malzemesi Giderleri', HESAPKOD='83003098090' WHERE YIL=2021 AND HESAPKOD='83003090890'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer İlaç Giderleri', HESAPKOD='830030990' WHERE YIL=2021 AND HESAPKOD='830030909'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Öğrenci  İlaç Giderleri', HESAPKOD='83003099001' WHERE YIL=2021 AND HESAPKOD='83003090901'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Er ve Erbaş İlaç Giderleri', HESAPKOD='83003099002' WHERE YIL=2021 AND HESAPKOD='83003090902'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Tutuklu ve Hükümlülerin İlaç Giderleri', HESAPKOD='83003099003' WHERE YIL=2021 AND HESAPKOD='83003090903'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sosyal Güvenliği Bulunmayanların İlaç Giderleri', HESAPKOD='83003099004' WHERE YIL=2021 AND HESAPKOD='83003090904'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sınıflandırılmayan Diğer İlaç Giderleri', HESAPKOD='83003099090' WHERE YIL=2021 AND HESAPKOD='83003090990'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Faiz Giderleri', HESAPKOD='83004' WHERE YIL=2021 AND HESAPKOD='83004'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kamu Kurumlarına Ödenen İç Borç Faiz Giderleri', HESAPKOD='8300401' WHERE YIL=2021 AND HESAPKOD='8300401'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Devlet Tahvili', HESAPKOD='830040110' WHERE YIL=2021 AND HESAPKOD='830040101'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='TL Cinsinden Tahvillerin Faiz Giderleri', HESAPKOD='83004011001' WHERE YIL=2021 AND HESAPKOD='83004010101'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Dövize Endeksli ve Döviz Cinsinden Tahvillerin Faiz Giderleri', HESAPKOD='83004011002' WHERE YIL=2021 AND HESAPKOD='83004010102'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Hazine Bonosu', HESAPKOD='830040120' WHERE YIL=2021 AND HESAPKOD='830040102'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='TL Cinsinden Bonoların Faiz Giderleri', HESAPKOD='83004012001' WHERE YIL=2021 AND HESAPKOD='83004010201'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Dövize Endeksli ve Döviz Cinsinden Bonoların Faiz Giderleri', HESAPKOD='83004012002' WHERE YIL=2021 AND HESAPKOD='83004010202'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer İç Borç Faiz Giderleri', HESAPKOD='8300402' WHERE YIL=2021 AND HESAPKOD='8300402'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Devlet Tahvili', HESAPKOD='830040210' WHERE YIL=2021 AND HESAPKOD='830040201'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='TL Cinsinden Tahvillerin Faiz Giderleri', HESAPKOD='83004021001' WHERE YIL=2021 AND HESAPKOD='83004020101'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Dövize Endeksli ve Döviz Cinsinden Tahvillerin Faiz Giderleri', HESAPKOD='83004021002' WHERE YIL=2021 AND HESAPKOD='83004020102'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Hazine Bonosu', HESAPKOD='830040220' WHERE YIL=2021 AND HESAPKOD='830040202'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='TL Cinsinden Bonoların Faiz Giderleri', HESAPKOD='83004022001' WHERE YIL=2021 AND HESAPKOD='83004020201'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Dövize Endeksli ve Döviz Cinsinden Bonoların Faiz Giderleri', HESAPKOD='83004022002' WHERE YIL=2021 AND HESAPKOD='83004020202'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer İç Borç Faiz Giderleri ', HESAPKOD='830040290' WHERE YIL=2021 AND HESAPKOD='830040209'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='TL Cinsinden Diğer İç Borç Faiz Giderleri', HESAPKOD='83004029001' WHERE YIL=2021 AND HESAPKOD='83004020901'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Dövize Endeksli ve Döviz Cinsinden Diğer İç Borç Faiz Giderleri ', HESAPKOD='83004029002' WHERE YIL=2021 AND HESAPKOD='83004020902'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Dış Borç Faiz Giderleri', HESAPKOD='8300403' WHERE YIL=2021 AND HESAPKOD='8300403'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='ABD Doları Cinsinden Dış Borç Faiz Giderleri', HESAPKOD='830040310' WHERE YIL=2021 AND HESAPKOD='830040301'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='ABD Doları Cinsinden Dış Borç Faiz Giderleri', HESAPKOD='83004031001' WHERE YIL=2021 AND HESAPKOD='83004030101'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Avro Cinsinden Borç Faiz Giderleri', HESAPKOD='830040320' WHERE YIL=2021 AND HESAPKOD='830040302'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Avro Cinsinden Dış Borç Faiz Giderleri', HESAPKOD='83004032001' WHERE YIL=2021 AND HESAPKOD='83004030201'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Japon Yeni Cinsinden Dış Borç Faiz Giderleri', HESAPKOD='830040330' WHERE YIL=2021 AND HESAPKOD='830040303'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Japon Yeni Cinsinden Dış Borç Faiz Giderleri', HESAPKOD='83004033001' WHERE YIL=2021 AND HESAPKOD='83004030301'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Dış Borç Faiz Giderleri', HESAPKOD='830040390' WHERE YIL=2021 AND HESAPKOD='830040309'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Dış Borç Faiz Giderleri', HESAPKOD='83004039001' WHERE YIL=2021 AND HESAPKOD='83004030901'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='İskonto Giderleri', HESAPKOD='8300404' WHERE YIL=2021 AND HESAPKOD='8300404'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='İç Borç İskonto Giderleri', HESAPKOD='830040410' WHERE YIL=2021 AND HESAPKOD='830040401'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Devlet Tahvili İskonto Giderleri', HESAPKOD='83004041001' WHERE YIL=2021 AND HESAPKOD='83004040101'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Hazine Bonosu İskonto Giderleri', HESAPKOD='83004041002' WHERE YIL=2021 AND HESAPKOD='83004040102'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Dış Borç İskonto Giderleri', HESAPKOD='830040420' WHERE YIL=2021 AND HESAPKOD='830040402'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Dış Borç İskonto Giderleri', HESAPKOD='83004042001' WHERE YIL=2021 AND HESAPKOD='83004040201'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Para Piyasası Nakit İşlemleri Faiz Giderleri', HESAPKOD='8300405' WHERE YIL=2021 AND HESAPKOD='8300405'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Para Piyasası Nakit İşlemleri Faiz Giderleri ', HESAPKOD='830040510' WHERE YIL=2021 AND HESAPKOD='830040501'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='TL Cinsinden Para Piyasası Nakit İşlemleri Faiz Giderleri ', HESAPKOD='83004051001' WHERE YIL=2021 AND HESAPKOD='83004050101'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Döviz Cinsinden Kısa Vadeli Nakit İşlemlere Ait Faiz Giderleri', HESAPKOD='83004051002' WHERE YIL=2021 AND HESAPKOD='83004050102'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Türev Ürün Giderleri', HESAPKOD='8300406' WHERE YIL=2021 AND HESAPKOD='8300406'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='İç Borç Türev Ürün Giderleri', HESAPKOD='830040610' WHERE YIL=2021 AND HESAPKOD='830040601'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='İç Borç Türev Ürün Giderleri', HESAPKOD='83004061001' WHERE YIL=2021 AND HESAPKOD='83004060101'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Dış Borç Türev Ürün Giderleri', HESAPKOD='830040620' WHERE YIL=2021 AND HESAPKOD='830040602'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Dış Borç Türev Ürün Giderleri', HESAPKOD='83004062001' WHERE YIL=2021 AND HESAPKOD='83004060201'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kira Sertifikası Giderleri', HESAPKOD='8300407' WHERE YIL=2021 AND HESAPKOD='8300407'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Türk Lirası Cinsinden Kira Sertifikası Giderleri', HESAPKOD='830040710' WHERE YIL=2021 AND HESAPKOD='830040701'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Türk Lirası Cinsinden Kira Sertifikası Giderleri', HESAPKOD='83004071001' WHERE YIL=2021 AND HESAPKOD='83004070101'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Döviz Cinsinden Kira Sertifikası Giderleri', HESAPKOD='830040720' WHERE YIL=2021 AND HESAPKOD='830040702'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Döviz Cinsinden Kira Sertifikası Giderleri', HESAPKOD='83004072001' WHERE YIL=2021 AND HESAPKOD='83004070201'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Cari Transferler', HESAPKOD='83005' WHERE YIL=2021 AND HESAPKOD='83005'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Görevlendirme Giderleri', HESAPKOD='8300501' WHERE YIL=2021 AND HESAPKOD='8300501'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kamu Teşebbüslerine', HESAPKOD='830050110' WHERE YIL=2021 AND HESAPKOD='830050101'
DELETE FROM  MUHHESAPPLANI WHERE YIL=2021 AND HESAPKOD='83005010101'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='TC Devlet Demiryolları na', HESAPKOD='83005011001' WHERE YIL=2021 AND HESAPKOD='83005010102'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Tarım İşletmelerine', HESAPKOD='83005011002' WHERE YIL=2021 AND HESAPKOD='83005010103'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Toprak Mahsulleri Ofisi ne', HESAPKOD='83005011003' WHERE YIL=2021 AND HESAPKOD='83005010104'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Teşebbüslere', HESAPKOD='83005011090' WHERE YIL=2021 AND HESAPKOD='83005010105'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Çay İşletmeleri Genel Müdürlüğü ne', HESAPKOD='83005011004' WHERE YIL=2021 AND HESAPKOD='83005010106'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Türkiye Elektrik Dağıtım AŞ ye', HESAPKOD='83005011005' WHERE YIL=2021 AND HESAPKOD='83005010107'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Elektrik Üretim AŞ ye', HESAPKOD='83005011006' WHERE YIL=2021 AND HESAPKOD='83005010108'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Elektrik İletim AŞ ye', HESAPKOD='83005011007' WHERE YIL=2021 AND HESAPKOD='83005010109'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Türkiye Elektrik ve Taahhüt AŞ ye', HESAPKOD='83005011008' WHERE YIL=2021 AND HESAPKOD='83005010110'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Türkiye Taşkömürü Kurumu na', HESAPKOD='83005011009' WHERE YIL=2021 AND HESAPKOD='83005010111'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Üretici Birliklerine', HESAPKOD='83005011010' WHERE YIL=2021 AND HESAPKOD='83005010112'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Tarım Kredi Kooperatiflerine', HESAPKOD='83005011011' WHERE YIL=2021 AND HESAPKOD='83005010113'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Türkiye Şeker Fabrikaları AŞ ye', HESAPKOD='83005011012' WHERE YIL=2021 AND HESAPKOD='83005010114'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sümer Holding AŞ ye', HESAPKOD='83005011013' WHERE YIL=2021 AND HESAPKOD='83005010115'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Makina ve Kimya Endüstrisi Kurumu na', HESAPKOD='83005011014' WHERE YIL=2021 AND HESAPKOD='83005010116'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Teşebbüslere', HESAPKOD='83005011090' WHERE YIL=2021 AND HESAPKOD='83005010117'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Türkiye Kömür İşletmeleri Kurumu na', HESAPKOD='83005011015' WHERE YIL=2021 AND HESAPKOD='83005010118'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Et ve Süt Kurumu Genel Müdürlüğü ne', HESAPKOD='83005011016' WHERE YIL=2021 AND HESAPKOD='83005010119'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Türkiye Elektromekanik Sanayii Genel Müdürlüğü ne', HESAPKOD='83005011017' WHERE YIL=2021 AND HESAPKOD='83005010120'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Devlet Hava Meydanları İşletmesi Genel Müdürlüğü ne', HESAPKOD='83005011018' WHERE YIL=2021 AND HESAPKOD='83005010121'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Türkiye Petrolleri Anonim Ortaklığı’na', HESAPKOD='83005011019' WHERE YIL=2021 AND HESAPKOD='83005010123'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Teşebbüslere', HESAPKOD='83005011090' WHERE YIL=2021 AND HESAPKOD='83005010190'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sosyal Güvenlik Kurumlarına', HESAPKOD='830050120' WHERE YIL=2021 AND HESAPKOD='830050102'
DELETE FROM  MUHHESAPPLANI WHERE YIL=2021 AND HESAPKOD='83005010201'
DELETE FROM  MUHHESAPPLANI WHERE YIL=2021 AND HESAPKOD='83005010202'
DELETE FROM  MUHHESAPPLANI WHERE YIL=2021 AND HESAPKOD='83005010203'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Türkiye İş Kurumu Genel Müdürlüğü ne', HESAPKOD='83005012001' WHERE YIL=2021 AND HESAPKOD='83005010204'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='5510 Sayılı Kanunun 4/c Maddesi Kapsamındaki Sigortalılara Vazife Malüllük Aylıkları İle Bunların Hak Sahiplerine Bağlanacak Ölüm Aylıkları Karşılığı  Ödenecek Ek Karşılıklar', HESAPKOD='83005012002' WHERE YIL=2021 AND HESAPKOD='83005010205'
DELETE FROM  MUHHESAPPLANI WHERE YIL=2021 AND HESAPKOD='83005010290'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Mali Kurumlara', HESAPKOD='830050130' WHERE YIL=2021 AND HESAPKOD='830050103'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Ziraat Bankasına', HESAPKOD='83005013001' WHERE YIL=2021 AND HESAPKOD='83005010301'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Türkiye Kalkınma ve Yatırım Bankasına', HESAPKOD='83005013002' WHERE YIL=2021 AND HESAPKOD='83005010302'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Türk Eximbanka ', HESAPKOD='83005013003' WHERE YIL=2021 AND HESAPKOD='83005010303'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Halk Bankasına', HESAPKOD='83005013004' WHERE YIL=2021 AND HESAPKOD='83005010304'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Mali Kurumlara', HESAPKOD='83005013090' WHERE YIL=2021 AND HESAPKOD='83005010390'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Döner Sermaye İşletmelerine', HESAPKOD='830050140' WHERE YIL=2021 AND HESAPKOD='830050104'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sağlık Bakanlığı Döner Sermaye İşletmelerine', HESAPKOD='83005014001' WHERE YIL=2021 AND HESAPKOD='83005010401'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Ceza ve Tevkifevleri Döner Sermaye İşletmelerine', HESAPKOD='83005014002' WHERE YIL=2021 AND HESAPKOD='83005010402'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Milli Eğitim Bakanlığı  Döner Sermaye İşletmelerine', HESAPKOD='83005014003' WHERE YIL=2021 AND HESAPKOD='83005010403'
DELETE FROM  MUHHESAPPLANI WHERE YIL=2021 AND HESAPKOD='83005010406'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer  Döner Sermaye İşletmelerine', HESAPKOD='83005014090' WHERE YIL=2021 AND HESAPKOD='83005010490'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Fonlara', HESAPKOD='830050150' WHERE YIL=2021 AND HESAPKOD='830050105'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Fonlara', HESAPKOD='83005015090' WHERE YIL=2021 AND HESAPKOD='83005010501'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sosyal Yardımlaşma ve Dayanışmayı Teşvik Fonuna', HESAPKOD='83005015001' WHERE YIL=2021 AND HESAPKOD='83005010502'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='İşsizlik Sigortası Fonuna', HESAPKOD='83005015002' WHERE YIL=2021 AND HESAPKOD='83005010503'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Fonlara', HESAPKOD='83005015090' WHERE YIL=2021 AND HESAPKOD='83005010590'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Teşekküllere', HESAPKOD='830050190' WHERE YIL=2021 AND HESAPKOD='830050109'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Teşekküllere', HESAPKOD='83005019090' WHERE YIL=2021 AND HESAPKOD='83005010901'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Teşekküllere', HESAPKOD='83005019090' WHERE YIL=2021 AND HESAPKOD='83005010902'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Hazine Yardımları', HESAPKOD='8300502' WHERE YIL=2021 AND HESAPKOD='8300502'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Genel Bütçeye Hazine Yardımı', HESAPKOD='830050210' WHERE YIL=2021 AND HESAPKOD='830050201'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Genel Bütçeye Hazine Yardımı', HESAPKOD='83005021001' WHERE YIL=2021 AND HESAPKOD='83005020101'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Özel Bütçeli İdarelere Hazine Yardımları', HESAPKOD='830050220' WHERE YIL=2021 AND HESAPKOD='830050202'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Ölçme, Seçme ve Yerleştirme Merkezi Başkanlığı na Hazine Yardımı', HESAPKOD='83005022001' WHERE YIL=2021 AND HESAPKOD='83005020201'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Atatürk Kültür, Dil ve Tarih Yüksek Kurumu na Hazine Yardımı', HESAPKOD='83005022002' WHERE YIL=2021 AND HESAPKOD='83005020202'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Atatürk Araştırma Merkezi ne Hazine Yardımı', HESAPKOD='83005022003' WHERE YIL=2021 AND HESAPKOD='83005020203'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Atatürk Kültür Merkezi ne Hazine Yardımı', HESAPKOD='83005022004' WHERE YIL=2021 AND HESAPKOD='83005020204'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Türk Dil Kurumu na Hazine Yardımı', HESAPKOD='83005022005' WHERE YIL=2021 AND HESAPKOD='83005020205'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Türk Tarih Kurumu na Hazine Yardımı', HESAPKOD='83005022006' WHERE YIL=2021 AND HESAPKOD='83005020206'
DELETE FROM  MUHHESAPPLANI WHERE YIL=2021 AND HESAPKOD='83005020207'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Türkiye Bilimsel ve Teknolojik Araştırma Kurumu Başkanlığı na Hazine Yardımı', HESAPKOD='83005022007' WHERE YIL=2021 AND HESAPKOD='83005020208'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Türkiye Bilimler Akademisi Başkanlığı na Hazine Yardımı', HESAPKOD='83005022008' WHERE YIL=2021 AND HESAPKOD='83005020209'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Türkiye Adalet Akademisi’ne Hazine Yardımı', HESAPKOD='83005022009' WHERE YIL=2021 AND HESAPKOD='83005020210'
DELETE FROM  MUHHESAPPLANI WHERE YIL=2021 AND HESAPKOD='83005020213'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sınıflandırmaya Girmeyen Diğer Hazine Yardımları', HESAPKOD='83005029090' WHERE YIL=2021 AND HESAPKOD='83005020214'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Devlet Tiyatroları Genel Müdürlüğü ne Hazine Yardımı', HESAPKOD='83005022010' WHERE YIL=2021 AND HESAPKOD='83005020215'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Devlet Opera ve Balesi Genel Müdürlüğü ne Hazine Yardımı', HESAPKOD='83005022011' WHERE YIL=2021 AND HESAPKOD='83005020216'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Orman Genel Müdürlüğü ne Hazine Yardımı', HESAPKOD='83005022012' WHERE YIL=2021 AND HESAPKOD='83005020217'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Vakıflar Genel Müdürlüğü ne Hazine Yardımı', HESAPKOD='83005022013' WHERE YIL=2021 AND HESAPKOD='83005020218'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Türkiye Hudut ve Sahiller Sağlık Genel Müdürlüğü ne Hazine Yardımı', HESAPKOD='83005022014' WHERE YIL=2021 AND HESAPKOD='83005020219'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Türk Akreditasyon Kurumu na Hazine Yardımı', HESAPKOD='83005022015' WHERE YIL=2021 AND HESAPKOD='83005020221'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Türk Standartları Enstitüsü ne Hazine Yardımı', HESAPKOD='83005022016' WHERE YIL=2021 AND HESAPKOD='83005020222'
DELETE FROM  MUHHESAPPLANI WHERE YIL=2021 AND HESAPKOD='83005020223'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Türk Patent Enstitüsü ne Hazine Yardımı', HESAPKOD='83005022017' WHERE YIL=2021 AND HESAPKOD='83005020224'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Ulusal Bor Araştırma Enstitüsü ne Hazine Yardımı', HESAPKOD='83005022018' WHERE YIL=2021 AND HESAPKOD='83005020226'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Türkiye Atom Enerjisi Kurumu na Hazine Yardımı', HESAPKOD='83005022019' WHERE YIL=2021 AND HESAPKOD='83005020227'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Savunma Sanayi Başkanlığı na Hazine Yardımı', HESAPKOD='83005022020' WHERE YIL=2021 AND HESAPKOD='83005020228'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Küçük ve Orta Ölçekli İşletmeleri Geliştirme ve Destekleme İdaresi Başkanlığı na Hazine Yardımı', HESAPKOD='83005022021' WHERE YIL=2021 AND HESAPKOD='83005020230'
DELETE FROM  MUHHESAPPLANI WHERE YIL=2021 AND HESAPKOD='83005020231'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Türk İşbirliği ve Koordinasyon Ajansı Başkanlığı na Hazine Yardımı', HESAPKOD='83005022022' WHERE YIL=2021 AND HESAPKOD='83005020232'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='GAP Bölge Kalkınma İdaresi Başkanlığı na Hazine Yardımı', HESAPKOD='83005022023' WHERE YIL=2021 AND HESAPKOD='83005020234'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Özelleştirme İdaresi Başkanlığı na Hazine Yardımı', HESAPKOD='83005022024' WHERE YIL=2021 AND HESAPKOD='83005020235'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Maden Tetkik ve Arama Genel Müdürlüğü ne Hazine Yardımı', HESAPKOD='83005022025' WHERE YIL=2021 AND HESAPKOD='83005020240'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Ceza İnfaz Kurumları ile Tutukevleri İş Yurtları Kurumu na  Hazine Yardımı', HESAPKOD='83005022026' WHERE YIL=2021 AND HESAPKOD='83005020241'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sivil Havacılık Genel Müdürlüğü ne Hazine Yardımı', HESAPKOD='83005022027' WHERE YIL=2021 AND HESAPKOD='83005020249'
DELETE FROM  MUHHESAPPLANI WHERE YIL=2021 AND HESAPKOD='83005020250'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yurtdışı Türkler ve Akraba Topluluklar Başkanlığına Hazine Yardımı', HESAPKOD='83005022029' WHERE YIL=2021 AND HESAPKOD='83005020251'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Karayolları Genel Müdürlüğü ne Hazine Yardımı', HESAPKOD='83005022030' WHERE YIL=2021 AND HESAPKOD='83005020252'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Türkiye Yazma Eserler Kurumu Başkanlığı na Hazine Yardımı', HESAPKOD='83005022031' WHERE YIL=2021 AND HESAPKOD='83005020253'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Doğu Anadolu Projesi Bölge Kalkınma İdaresi Başkanlığı na Hazine Yardımı', HESAPKOD='83005022032' WHERE YIL=2021 AND HESAPKOD='83005020254'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Konya Ovası Projesi Bölge Kalkınma İdaresi Başkanlığı na Hazine Yardımı', HESAPKOD='83005022033' WHERE YIL=2021 AND HESAPKOD='83005020255'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Doğu Karadeniz Projesi Bölge Kalkınma İdaresi Başkanlığı na Hazine Yardımı', HESAPKOD='83005022034' WHERE YIL=2021 AND HESAPKOD='83005020256'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Devlet Su İşleri Genel Müdürlüğü ne Hazine Yardımı', HESAPKOD='83005022035' WHERE YIL=2021 AND HESAPKOD='83005020257'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Türkiye Su Enstitüsü ne Hazine Yardımı', HESAPKOD='83005022036' WHERE YIL=2021 AND HESAPKOD='83005020258'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Türkiye İlaç ve Tıbbi Cihaz Kurumu na Hazine Yardımı', HESAPKOD='83005022037' WHERE YIL=2021 AND HESAPKOD='83005020259'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kamu Denetçiliği Kurumu na Hazine Yardımı', HESAPKOD='83005022038' WHERE YIL=2021 AND HESAPKOD='83005020260'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Türkiye İnsan Hakları ve Eşitlik Kurumu na Hazine Yardımı', HESAPKOD='83005022039' WHERE YIL=2021 AND HESAPKOD='83005020261'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Türkiye Sağlık Enstitüleri Başkanlığı na Hazine Yardımı', HESAPKOD='83005022040' WHERE YIL=2021 AND HESAPKOD='83005020262'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Helal Akreditasyon Kurumu na Hazine Yardımı', HESAPKOD='83005022041' WHERE YIL=2021 AND HESAPKOD='83005020263'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Nadir Toprak Elementleri Araştırma Enstitüsü ne Hazine Yardımı', HESAPKOD='83005022042' WHERE YIL=2021 AND HESAPKOD='83005020264'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Maden ve Petrol İşleri Genel Müdürlüğü ne Hazine Yardımı', HESAPKOD='83005022043' WHERE YIL=2021 AND HESAPKOD='83005020265'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Türkiye Uzay Ajansı na Hazine Yardımı', HESAPKOD='83005022044' WHERE YIL=2021 AND HESAPKOD='83005020266'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Düzenleyici ve Denetleyici Kurumlara Hazine Yardımları', HESAPKOD='830050230' WHERE YIL=2021 AND HESAPKOD='830050203'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Radyo ve Televizyon Üst Kurulu na Hazine Yardımı', HESAPKOD='83005023001' WHERE YIL=2021 AND HESAPKOD='83005020301'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Bilgi Teknolojileri ve İletişim Kurumu na Hazine Yardımı', HESAPKOD='83005023002' WHERE YIL=2021 AND HESAPKOD='83005020302'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sermaye Piyasası Kurulu na Hazine Yardımı', HESAPKOD='83005023003' WHERE YIL=2021 AND HESAPKOD='83005020303'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Bankacılık Düzenleme ve Denetleme Kurumu na Hazine Yardımı', HESAPKOD='83005023004' WHERE YIL=2021 AND HESAPKOD='83005020304'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Enerji Piyasası Düzenleme Kurumu na Hazine Yardımı', HESAPKOD='83005023005' WHERE YIL=2021 AND HESAPKOD='83005020305'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kamu İhale Kurumu na Hazine Yardımı', HESAPKOD='83005023006' WHERE YIL=2021 AND HESAPKOD='83005020306'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Rekabet Kurumu na Hazine Yardımı', HESAPKOD='83005023007' WHERE YIL=2021 AND HESAPKOD='83005020307'
DELETE FROM  MUHHESAPPLANI WHERE YIL=2021 AND HESAPKOD='83005020309'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kamu Gözetimi, Muhasebe ve Denetim Standartları Kurumu na Hazine Yardımı', HESAPKOD='83005023008' WHERE YIL=2021 AND HESAPKOD='83005020310'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kişisel Verileri Koruma Kurumu na Hazine Yardımı', HESAPKOD='83005023009' WHERE YIL=2021 AND HESAPKOD='83005020311'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Nükleer Düzenleme Kurumu na Hazine Yardımı', HESAPKOD='83005023010' WHERE YIL=2021 AND HESAPKOD='83005020312'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sigortacılık ve Özel Emeklilik Düzenleme ve Denetleme Kurumu na Hazine Yardımı', HESAPKOD='83005023011' WHERE YIL=2021 AND HESAPKOD='83005020313'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sosyal Güvenlik Kurumlarına Hazine Yardımları', HESAPKOD='830050240' WHERE YIL=2021 AND HESAPKOD='830050204'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Türkiye İş Kurumu Genel Müdürlüğü ne Hazine Yardımı', HESAPKOD='83005024001' WHERE YIL=2021 AND HESAPKOD='83005020404'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sosyal Güvenlik Kurumu na Açık Finansmanı Ödemeleri', HESAPKOD='83005024002' WHERE YIL=2021 AND HESAPKOD='83005020406'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Mahalli İdarelere Hazine Yardımları', HESAPKOD='830050250' WHERE YIL=2021 AND HESAPKOD='830050205'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='İl Özel İdarelerine', HESAPKOD='83005025001' WHERE YIL=2021 AND HESAPKOD='83005020501'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Belediyelere', HESAPKOD='83005025002' WHERE YIL=2021 AND HESAPKOD='83005020502'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Köylere', HESAPKOD='83005025003' WHERE YIL=2021 AND HESAPKOD='83005020503'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Mahalli İdare Birliklerine', HESAPKOD='83005025004' WHERE YIL=2021 AND HESAPKOD='83005020504'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Bağlı İdarelere', HESAPKOD='83005025005' WHERE YIL=2021 AND HESAPKOD='83005020505'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Hasılattan Paylar ve Giderlere Katılma Payları', HESAPKOD='830050260' WHERE YIL=2021 AND HESAPKOD='830050206'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Giderlere Katılma Payları', HESAPKOD='83005026002' WHERE YIL=2021 AND HESAPKOD='83005020619'
DELETE FROM  MUHHESAPPLANI WHERE YIL=2021 AND HESAPKOD='83005020608'
DELETE FROM  MUHHESAPPLANI WHERE YIL=2021 AND HESAPKOD='83005020609'
DELETE FROM  MUHHESAPPLANI WHERE YIL=2021 AND HESAPKOD='83005020610'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yükseköğretim Kurumlarına Hazine Yardımları', HESAPKOD='830050270' WHERE YIL=2021 AND HESAPKOD='830050207'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yükseköğretim Kurulu na Hazine Yardımı', HESAPKOD='83005027001' WHERE YIL=2021 AND HESAPKOD='83005020701'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Ankara Üniversitesi ne Hazine Yardımı', HESAPKOD='83005027002' WHERE YIL=2021 AND HESAPKOD='83005020702'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Ortadoğu Teknik Üniversitesi ne Hazine Yardımı', HESAPKOD='83005027003' WHERE YIL=2021 AND HESAPKOD='83005020703'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Hacettepe Üniversitesi ne Hazine Yardımı', HESAPKOD='83005027004' WHERE YIL=2021 AND HESAPKOD='83005020704'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Gazi Üniversitesi ne Hazine Yardımı', HESAPKOD='83005027005' WHERE YIL=2021 AND HESAPKOD='83005020705'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='İstanbul Üniversitesi ne Hazine Yardımı', HESAPKOD='83005027006' WHERE YIL=2021 AND HESAPKOD='83005020706'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='İstanbul Teknik Üniversitesi ne Hazine Yardımı', HESAPKOD='83005027007' WHERE YIL=2021 AND HESAPKOD='83005020707'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Boğaziçi Üniversitesi ne Hazine Yardımı', HESAPKOD='83005027008' WHERE YIL=2021 AND HESAPKOD='83005020708'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Marmara Üniversitesi ne Hazine Yardımı', HESAPKOD='83005027009' WHERE YIL=2021 AND HESAPKOD='83005020709'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yıldız Teknik Üniversitesi ne Hazine Yardımı', HESAPKOD='83005027010' WHERE YIL=2021 AND HESAPKOD='83005020710'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Mimar Sinan Güzel Sanatlar Üniversitesi ne Hazine Yardımı', HESAPKOD='83005027011' WHERE YIL=2021 AND HESAPKOD='83005020711'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Ege Üniversitesi ne Hazine Yardımı', HESAPKOD='83005027012' WHERE YIL=2021 AND HESAPKOD='83005020712'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Dokuz Eylül Üniversitesi ne Hazine Yardımı', HESAPKOD='83005027013' WHERE YIL=2021 AND HESAPKOD='83005020713'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Trakya Üniversitesi ne Hazine Yardımı', HESAPKOD='83005027014' WHERE YIL=2021 AND HESAPKOD='83005020714'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Bursa Uludağ Üniversitesi ne Hazine Yardımı', HESAPKOD='83005027015' WHERE YIL=2021 AND HESAPKOD='83005020715'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Anadolu Üniversitesi ne Hazine Yardımı', HESAPKOD='83005027016' WHERE YIL=2021 AND HESAPKOD='83005020716'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Selçuk Üniversitesi ne Hazine Yardımı', HESAPKOD='83005027017' WHERE YIL=2021 AND HESAPKOD='83005020717'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Akdeniz Üniversitesi ne Hazine Yardımı', HESAPKOD='83005027018' WHERE YIL=2021 AND HESAPKOD='83005020718'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Erciyes Üniversitesi ne Hazine Yardımı', HESAPKOD='83005027019' WHERE YIL=2021 AND HESAPKOD='83005020719'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sivas Cumhuriyet Üniversitesi ne Hazine Yardımı', HESAPKOD='83005027020' WHERE YIL=2021 AND HESAPKOD='83005020720'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Çukurova Üniversitesi ne Hazine Yardımı', HESAPKOD='83005027021' WHERE YIL=2021 AND HESAPKOD='83005020721'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Ondokuz Mayıs Üniversitesi ne Hazine Yardımı', HESAPKOD='83005027022' WHERE YIL=2021 AND HESAPKOD='83005020722'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Karadeniz Teknik Üniversitesi ne Hazine Yardımı', HESAPKOD='83005027023' WHERE YIL=2021 AND HESAPKOD='83005020723'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Atatürk Üniversitesi ne Hazine Yardımı', HESAPKOD='83005027024' WHERE YIL=2021 AND HESAPKOD='83005020724'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='İnönü Üniversitesi ne Hazine Yardımı', HESAPKOD='83005027025' WHERE YIL=2021 AND HESAPKOD='83005020725'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Fırat Üniversitesi ne Hazine Yardımı', HESAPKOD='83005027026' WHERE YIL=2021 AND HESAPKOD='83005020726'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Dicle Üniversitesi ne Hazine Yardımı', HESAPKOD='83005027027' WHERE YIL=2021 AND HESAPKOD='83005020727'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yüzüncüyıl Üniversitesi ne Hazine Yardımı', HESAPKOD='83005027028' WHERE YIL=2021 AND HESAPKOD='83005020728'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Gaziantep Üniversitesi ne Hazine Yardımı', HESAPKOD='83005027029' WHERE YIL=2021 AND HESAPKOD='83005020729'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='İzmir Yüksek Teknoloji Enstitüsü ne Hazine Yardımı', HESAPKOD='83005027030' WHERE YIL=2021 AND HESAPKOD='83005020730'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Gebze Teknik Üniversitesi ne Hazine Yardımı', HESAPKOD='83005027031' WHERE YIL=2021 AND HESAPKOD='83005020731'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Harran Üniversitesi ne Hazine Yardımı', HESAPKOD='83005027032' WHERE YIL=2021 AND HESAPKOD='83005020732'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Süleyman Demirel Üniversitesi ne Hazine Yardımı', HESAPKOD='83005027033' WHERE YIL=2021 AND HESAPKOD='83005020733'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Aydın Adnan Menderes Üniversitesi ne Hazine Yardımı', HESAPKOD='83005027034' WHERE YIL=2021 AND HESAPKOD='83005020734'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Zonguldak Bülent Ecevit Üniversitesi ne Hazine Yardımı', HESAPKOD='83005027035' WHERE YIL=2021 AND HESAPKOD='83005020735'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Mersin Üniversitesi ne Hazine Yardımı', HESAPKOD='83005027036' WHERE YIL=2021 AND HESAPKOD='83005020736'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Pamukkale Üniversitesi ne Hazine Yardımı', HESAPKOD='83005027037' WHERE YIL=2021 AND HESAPKOD='83005020737'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Balıkesir Üniversitesi ne Hazine Yardımı', HESAPKOD='83005027038' WHERE YIL=2021 AND HESAPKOD='83005020738'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kocaeli Üniversitesi ne Hazine Yardımı', HESAPKOD='83005027039' WHERE YIL=2021 AND HESAPKOD='83005020739'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sakarya Üniversitesi ne Hazine Yardımı', HESAPKOD='83005027040' WHERE YIL=2021 AND HESAPKOD='83005020740'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Manisa Celal Bayar Üniversitesi’ne Hazine Yardımı', HESAPKOD='83005027041' WHERE YIL=2021 AND HESAPKOD='83005020741'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Bolu Abant İzzet Baysal Üniversitesi ne Hazine Yardımı', HESAPKOD='83005027042' WHERE YIL=2021 AND HESAPKOD='83005020742'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Hatay Mustafa Kemal Üniversitesi ne Hazine Yardımı', HESAPKOD='83005027043' WHERE YIL=2021 AND HESAPKOD='83005020743'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Afyon Kocatepe Üniversitesi ne Hazine Yardımı', HESAPKOD='83005027044' WHERE YIL=2021 AND HESAPKOD='83005020744'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kafkas Üniversitesi ne Hazine Yardımı', HESAPKOD='83005027045' WHERE YIL=2021 AND HESAPKOD='83005020745'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Çanakkale 18 Mart Üniversitesi ne Hazine Yardımı', HESAPKOD='83005027046' WHERE YIL=2021 AND HESAPKOD='83005020746'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Niğde Ömer Halisdemir Üniversitesi’ne Hazine Yardımı', HESAPKOD='83005027047' WHERE YIL=2021 AND HESAPKOD='83005020747'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kütahya Dumlupınar Üniversitesi ne Hazine Yardımı', HESAPKOD='83005027048' WHERE YIL=2021 AND HESAPKOD='83005020748'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Tokat Gaziosmanpaşa Üniversitesi ne Hazine Yardımı', HESAPKOD='83005027049' WHERE YIL=2021 AND HESAPKOD='83005020749'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Muğla Sıtkı Koçman Üniversitesi ne Hazine Yardımı', HESAPKOD='83005027050' WHERE YIL=2021 AND HESAPKOD='83005020750'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kahramanmaraş Sütçü İmam Üniversitesi ne Hazine Yardımı', HESAPKOD='83005027051' WHERE YIL=2021 AND HESAPKOD='83005020751'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kırıkkale Üniversitesi ne Hazine Yardımı', HESAPKOD='83005027052' WHERE YIL=2021 AND HESAPKOD='83005020752'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Eskişehir Osmangazi Üniversitesi ne Hazine Yardımı', HESAPKOD='83005027053' WHERE YIL=2021 AND HESAPKOD='83005020753'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Galatasaray Üniversitesi ne Hazine Yardımı', HESAPKOD='83005027054' WHERE YIL=2021 AND HESAPKOD='83005020754'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kırşehir Ahi Evran Üniversitesi ne Hazine Yardımı', HESAPKOD='83005027055' WHERE YIL=2021 AND HESAPKOD='83005020755'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kastamonu Üniversitesi ne Hazine Yardımı', HESAPKOD='83005027056' WHERE YIL=2021 AND HESAPKOD='83005020756'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Düzce Üniversitesi ne Hazine Yardımı', HESAPKOD='83005027057' WHERE YIL=2021 AND HESAPKOD='83005020757'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Burdur Mehmet Akif Ersoy Üniversitesi ne Hazine Yardımı', HESAPKOD='83005027058' WHERE YIL=2021 AND HESAPKOD='83005020758'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Uşak Üniversitesi ne Hazine Yardımı', HESAPKOD='83005027059' WHERE YIL=2021 AND HESAPKOD='83005020759'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Recep Tayyip Erdoğan Üniversitesi ne Hazine Yardımı', HESAPKOD='83005027060' WHERE YIL=2021 AND HESAPKOD='83005020760'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Tekirdağ Namık Kemal Üniversitesi ne Hazine Yardımı', HESAPKOD='83005027061' WHERE YIL=2021 AND HESAPKOD='83005020761'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Erzincan Binali Yıldırım Üniversitesi ne Hazine Yardımı', HESAPKOD='83005027062' WHERE YIL=2021 AND HESAPKOD='83005020762'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Aksaray Üniversitesi ne Hazine Yardımı', HESAPKOD='83005027063' WHERE YIL=2021 AND HESAPKOD='83005020763'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Giresun Üniversitesi ne Hazine Yardımı', HESAPKOD='83005027064' WHERE YIL=2021 AND HESAPKOD='83005020764'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Hitit Üniversitesi ne Hazine Yardımı', HESAPKOD='83005027065' WHERE YIL=2021 AND HESAPKOD='83005020765'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yozgat Bozok Üniversitesi ne Hazine Yardımı', HESAPKOD='83005027066' WHERE YIL=2021 AND HESAPKOD='83005020766'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Adıyaman Üniversitesi ne Hazine Yardımı', HESAPKOD='83005027067' WHERE YIL=2021 AND HESAPKOD='83005020767'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Ordu Üniversitesi ne Hazine Yardımı', HESAPKOD='83005027068' WHERE YIL=2021 AND HESAPKOD='83005020768'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Amasya Üniversitesi ne Hazine Yardımı', HESAPKOD='83005027069' WHERE YIL=2021 AND HESAPKOD='83005020769'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Karamanoğlu Mehmetbey Üniversitesi ne Hazine Yardımı', HESAPKOD='83005027070' WHERE YIL=2021 AND HESAPKOD='83005020770'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Ağrı İbrahim Çeçen Üniversitesi ne Hazine Yardımı', HESAPKOD='83005027071' WHERE YIL=2021 AND HESAPKOD='83005020771'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sinop Üniversitesi ne Hazine Yardımı', HESAPKOD='83005027072' WHERE YIL=2021 AND HESAPKOD='83005020772'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Siirt Üniversitesi ne Hazine Yardımı', HESAPKOD='83005027073' WHERE YIL=2021 AND HESAPKOD='83005020773'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Nevşehir Hacı Bektaş Veli Üniversitesi ne Hazine Yardımı', HESAPKOD='83005027074' WHERE YIL=2021 AND HESAPKOD='83005020774'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Karabük Üniversitesi ne Hazine Yardımı', HESAPKOD='83005027075' WHERE YIL=2021 AND HESAPKOD='83005020775'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kilis 7 Aralık Üniversitesi ne Hazine Yardımı', HESAPKOD='83005027076' WHERE YIL=2021 AND HESAPKOD='83005020776'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Çankırı Karatekin Üniversitesi ne Hazine Yardımı', HESAPKOD='83005027077' WHERE YIL=2021 AND HESAPKOD='83005020777'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Artvin Çoruh Üniversitesi ne Hazine Yardımı', HESAPKOD='83005027078' WHERE YIL=2021 AND HESAPKOD='83005020778'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Bilecik Şeyh Edebali Üniversitesi ne Hazine Yardımı', HESAPKOD='83005027079' WHERE YIL=2021 AND HESAPKOD='83005020779'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Bitlis Eren Üniversitesi ne Hazine Yardımı', HESAPKOD='83005027080' WHERE YIL=2021 AND HESAPKOD='83005020780'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kırklareli Üniversitesi ne Hazine Yardımı', HESAPKOD='83005027081' WHERE YIL=2021 AND HESAPKOD='83005020781'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Osmaniye Korkut Ata Üniversitesi ne Hazine Yardımı', HESAPKOD='83005027082' WHERE YIL=2021 AND HESAPKOD='83005020782'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Bingöl Üniversitesi ne Hazine Yardımı', HESAPKOD='83005027083' WHERE YIL=2021 AND HESAPKOD='83005020783'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Muş Alparslan Üniversitesi ne Hazine Yardımı', HESAPKOD='83005027084' WHERE YIL=2021 AND HESAPKOD='83005020784'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Mardin Artuklu Üniversitesi ne Hazine Yardımı', HESAPKOD='83005027085' WHERE YIL=2021 AND HESAPKOD='83005020785'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Batman Üniversitesi ne Hazine Yardımı', HESAPKOD='83005027086' WHERE YIL=2021 AND HESAPKOD='83005020786'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Ardahan Üniversitesi ne Hazine Yardımı', HESAPKOD='83005027087' WHERE YIL=2021 AND HESAPKOD='83005020787'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Bartın Üniversitesi ne Hazine Yardımı', HESAPKOD='83005027088' WHERE YIL=2021 AND HESAPKOD='83005020788'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Bayburt Üniversitesi ne Hazine Yardımı', HESAPKOD='83005027089' WHERE YIL=2021 AND HESAPKOD='83005020789'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Gümüşhane Üniversitesi ne Hazine Yardımı', HESAPKOD='83005027090' WHERE YIL=2021 AND HESAPKOD='83005020790'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Hakkari Üniversitesi ne Hazine Yardımı', HESAPKOD='83005027091' WHERE YIL=2021 AND HESAPKOD='83005020791'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Iğdır Üniversitesi ne Hazine Yardımı', HESAPKOD='83005027092' WHERE YIL=2021 AND HESAPKOD='83005020792'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Şırnak Üniversitesi ne Hazine Yardımı', HESAPKOD='83005027093' WHERE YIL=2021 AND HESAPKOD='83005020793'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Munzur Üniversitesi ne Hazine Yardımı', HESAPKOD='83005027094' WHERE YIL=2021 AND HESAPKOD='83005020794'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yalova Üniversitesi ne Hazine Yardımı', HESAPKOD='83005027095' WHERE YIL=2021 AND HESAPKOD='83005020795'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Türk-Alman Üniversitesi ne Hazine Yardımı', HESAPKOD='83005027096' WHERE YIL=2021 AND HESAPKOD='83005020796'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Ankara Yıldırım Beyazıt Üniversitesi ne Hazine Yardımı', HESAPKOD='83005027097' WHERE YIL=2021 AND HESAPKOD='83005020797'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Bursa Teknik Üniversitesi ne Hazine Yardımı', HESAPKOD='83005027098' WHERE YIL=2021 AND HESAPKOD='83005020798'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='İstanbul Medeniyet Üniversitesi ne Hazine Yardımı', HESAPKOD='83005027099' WHERE YIL=2021 AND HESAPKOD='83005020799'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yükseköğretim Kurumlarına Hazine Yardımları', HESAPKOD='830050280' WHERE YIL=2021 AND HESAPKOD='830050208'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='İzmir Katip Çelebi Üniversitesi ne Hazine Yardımı', HESAPKOD='83005028001' WHERE YIL=2021 AND HESAPKOD='83005020801'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Necmettin Erbakan Üniversitesi ne Hazine Yardımı', HESAPKOD='83005028002' WHERE YIL=2021 AND HESAPKOD='83005020802'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Abdullah Gül Üniversitesi ne Hazine Yardımı', HESAPKOD='83005028003' WHERE YIL=2021 AND HESAPKOD='83005020803'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Erzurum Teknik Üniversitesi ne Hazine Yardımı', HESAPKOD='83005028004' WHERE YIL=2021 AND HESAPKOD='83005020804'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Adana Alparslan Türkeş Bilim ve Teknoloji Üniversitesi ne Hazine Yardımı', HESAPKOD='83005028005' WHERE YIL=2021 AND HESAPKOD='83005020805'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Ankara Sosyal Bilimler Üniversitesi ne Hazine Yardımı', HESAPKOD='83005028006' WHERE YIL=2021 AND HESAPKOD='83005020806'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sağlık Bilimleri Üniversitesi ne Hazine Yardımı', HESAPKOD='83005028007' WHERE YIL=2021 AND HESAPKOD='83005020807'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Bandırma Onyedi Eylül Üniversitesi ne Hazine Yardımı', HESAPKOD='83005028008' WHERE YIL=2021 AND HESAPKOD='83005020808'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='İskenderun Teknik Üniversitesi ne Hazine Yardımı', HESAPKOD='83005028009' WHERE YIL=2021 AND HESAPKOD='83005020809'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Alanya Alaaddin Keykubat Üniversitesi ne Hazine Yardımı', HESAPKOD='83005028010' WHERE YIL=2021 AND HESAPKOD='83005020810'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Türkiye Uluslararası İslam, Bilim ve Teknoloji Üniversitesi ne Hazine Yardımı', HESAPKOD='83005028011' WHERE YIL=2021 AND HESAPKOD='83005020811'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='İzmir Bakırçay Üniversitesi ne Hazine Yardımı', HESAPKOD='83005028012' WHERE YIL=2021 AND HESAPKOD='83005020812'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='İzmir Demokrasi Üniversitesi ne Hazine Yardımı', HESAPKOD='83005028013' WHERE YIL=2021 AND HESAPKOD='83005020813'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yükseköğretim Kalite Kurulu na Hazine Yardımı', HESAPKOD='83005028014' WHERE YIL=2021 AND HESAPKOD='83005020814'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Ankara Müzik ve Güzel Sanatlar Üniversitesi ne Hazine Yardımı', HESAPKOD='83005028015' WHERE YIL=2021 AND HESAPKOD='83005020815'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Gaziantep İslam Bilim ve Teknoloji Üniversitesi ne Hazine Yardımı', HESAPKOD='83005028016' WHERE YIL=2021 AND HESAPKOD='83005020816'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Konya Teknik Üniversitesi ne Hazine Yardımı', HESAPKOD='83005028017' WHERE YIL=2021 AND HESAPKOD='83005020817'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kütahya Sağlık Bilimleri Üniversitesi ne Hazine Yardımı', HESAPKOD='83005028018' WHERE YIL=2021 AND HESAPKOD='83005020818'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Malatya Turgut Özal Üniversitesi ne Hazine Yardımı', HESAPKOD='83005028019' WHERE YIL=2021 AND HESAPKOD='83005020819'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='İstanbul Üniversitesi-Cerrahpaşa ya Hazine Yardımı', HESAPKOD='83005028020' WHERE YIL=2021 AND HESAPKOD='83005020820'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Ankara Hacı Bayram Veli Üniversitesi ne Hazine Yardımı', HESAPKOD='83005028021' WHERE YIL=2021 AND HESAPKOD='83005020821'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sakarya Uygulamalı Bilimler Üniversitesi ne Hazine Yardımı', HESAPKOD='83005028022' WHERE YIL=2021 AND HESAPKOD='83005020822'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Samsun Üniversitesi ne Hazine Yardımı', HESAPKOD='83005028023' WHERE YIL=2021 AND HESAPKOD='83005020823'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sivas Bilim ve Teknoloji Üniversitesi ne Hazine Yardımı', HESAPKOD='83005028024' WHERE YIL=2021 AND HESAPKOD='83005020824'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Tarsus Üniversitesi ne Hazine Yardımı', HESAPKOD='83005028025' WHERE YIL=2021 AND HESAPKOD='83005020825'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Trabzon Üniversitesi ne Hazine Yardımı', HESAPKOD='83005028026' WHERE YIL=2021 AND HESAPKOD='83005020826'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kayseri Üniversitesi ne Hazine Yardımı', HESAPKOD='83005028027' WHERE YIL=2021 AND HESAPKOD='83005020827'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kahramanmaraş İstiklal Üniversitesi ne Hazine Yardımı', HESAPKOD='83005028028' WHERE YIL=2021 AND HESAPKOD='83005020828'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Eskişehir Teknik Üniversitesi ne Hazine Yardımı', HESAPKOD='83005028029' WHERE YIL=2021 AND HESAPKOD='83005020829'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Isparta Uygulamalı Bilimler Üniversitesi ne Hazine Yardımı', HESAPKOD='83005028030' WHERE YIL=2021 AND HESAPKOD='83005020830'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Afyonkarahisar Sağlık Bilimleri Üniversitesi ne Hazine Yardımı', HESAPKOD='83005028031' WHERE YIL=2021 AND HESAPKOD='83005020831'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sınıflandırmaya Girmeyen Diğer Hazine Yardımları', HESAPKOD='83005029090' WHERE YIL=2021 AND HESAPKOD='83005020880'
DELETE FROM  MUHHESAPPLANI WHERE YIL=2021 AND HESAPKOD='83005020890'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Hazine Yardımları', HESAPKOD='830050290' WHERE YIL=2021 AND HESAPKOD='830050209'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Gençlik ve Spor İl Müdürlükleri ne Hazine Yardımı', HESAPKOD='83005029001' WHERE YIL=2021 AND HESAPKOD='83005020911'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Merkezi Finans ve İhale Birimi ne Hazine Yardımı', HESAPKOD='83005029002' WHERE YIL=2021 AND HESAPKOD='83005020912'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sınıflandırmaya Girmeyen Diğer Hazine Yardımları', HESAPKOD='83005029090' WHERE YIL=2021 AND HESAPKOD='83005020913'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Destekleme ve Fiyat İstikrar Fonu na Hazine Yardımı', HESAPKOD='83005029003' WHERE YIL=2021 AND HESAPKOD='83005020914'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='İşsizlik Sigortası Fonu na Hazine Yardımı', HESAPKOD='83005029004' WHERE YIL=2021 AND HESAPKOD='83005020915'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yatırım Ofisi’ne Hazine Yardımı', HESAPKOD='83005029005' WHERE YIL=2021 AND HESAPKOD='83005020917'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Tarım ve Kırsal Kalkınmayı Destekleme Kurumu na Hazine Yardımı', HESAPKOD='83005029009' WHERE YIL=2021 AND HESAPKOD='83005020918'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='İller Bankası AŞ ye Hazine Yardımı', HESAPKOD='83005029010' WHERE YIL=2021 AND HESAPKOD='83005020919'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Üniversite Döner Sermaye Bütçelerine Hazine Yardımı', HESAPKOD='83005029011' WHERE YIL=2021 AND HESAPKOD='83005020920'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Çanakkale Savaşları Gelibolu Tarihi Alan Başkanlığı’na Hazine Yardımı', HESAPKOD='83005029012' WHERE YIL=2021 AND HESAPKOD='83005020921'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sınıflandırmaya Girmeyen Diğer Hazine Yardımları', HESAPKOD='83005029090' WHERE YIL=2021 AND HESAPKOD='83005020990'
DELETE FROM  MUHHESAPPLANI WHERE YIL=2021 AND HESAPKOD='83005020999'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kar Amacı Gütmeyen Kuruluşlara Yapılan Transferler', HESAPKOD='8300503' WHERE YIL=2021 AND HESAPKOD='8300503'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kar Amacı Gütmeyen Kuruluşlara', HESAPKOD='830050310' WHERE YIL=2021 AND HESAPKOD='830050301'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Dernek, Birlik, Kurum, Kuruluş, Sandık vb Kuruluşlara', HESAPKOD='83005031001' WHERE YIL=2021 AND HESAPKOD='83005030101'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Vakıf Üniversitelerine', HESAPKOD='83005031002' WHERE YIL=2021 AND HESAPKOD='83005030102'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kamu İşveren Sendikalarına', HESAPKOD='83005031003' WHERE YIL=2021 AND HESAPKOD='83005030103'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Siyasi Partilere', HESAPKOD='83005031004' WHERE YIL=2021 AND HESAPKOD='83005030104'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Memurların Öğle Yemeğine Yardım', HESAPKOD='83005031005' WHERE YIL=2021 AND HESAPKOD='83005030105'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sosyal Amaçlı Fonlara', HESAPKOD='83005031006' WHERE YIL=2021 AND HESAPKOD='83005030106'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Seçim Giderleri İçin Yapılacak Ödemeler', HESAPKOD='83005031007' WHERE YIL=2021 AND HESAPKOD='83005030107'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Türk Kültür Varlığının Korunması ve Tanıtımı Giderleri', HESAPKOD='83005031008' WHERE YIL=2021 AND HESAPKOD='83005030108'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Siyasi Tanıtma Giderleri', HESAPKOD='83005031009' WHERE YIL=2021 AND HESAPKOD='83005030109'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Mesleki Yeterlilik Kurumuna Yapılacak Ödemeler', HESAPKOD='83005031010' WHERE YIL=2021 AND HESAPKOD='83005030110'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Bilimsel Araştırma Projelerinin Desteklenmesine İlişkin Giderler', HESAPKOD='83005031011' WHERE YIL=2021 AND HESAPKOD='83005030111'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Avrupa Birliği Eğitim ve Gençlik Programları Merkezi Başkanlığına', HESAPKOD='83005031012' WHERE YIL=2021 AND HESAPKOD='83005030112'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Bağımsız Spor Federasyonlarına', HESAPKOD='83005031013' WHERE YIL=2021 AND HESAPKOD='83005030113'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Afet Riski Altındaki Alanların Dönüşüm Giderleri', HESAPKOD='83005031014' WHERE YIL=2021 AND HESAPKOD='83005030114'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Türkiye Maarif Vakfına', HESAPKOD='83005031015' WHERE YIL=2021 AND HESAPKOD='83005030115'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Ulusal Fona', HESAPKOD='83005031080' WHERE YIL=2021 AND HESAPKOD='83005030180'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kar Amacı Gütmeyen Diğer Kuruluşlara', HESAPKOD='83005031090' WHERE YIL=2021 AND HESAPKOD='83005030190'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Hanehalkı ve İşletmelere Yapılan Transferler', HESAPKOD='8300504' WHERE YIL=2021 AND HESAPKOD='8300504'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Burslar ve Harçlıklar', HESAPKOD='830050410' WHERE YIL=2021 AND HESAPKOD='830050401'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yurtiçi Burslar ve Harçlıklar', HESAPKOD='83005041001' WHERE YIL=2021 AND HESAPKOD='83005040101'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yurtdışı Burslar', HESAPKOD='83005041002' WHERE YIL=2021 AND HESAPKOD='83005040102'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yabancı Uyruklu Kişilere Burslar ve Harçlıklar', HESAPKOD='83005041003' WHERE YIL=2021 AND HESAPKOD='83005040103'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yurtiçi Burslar ve Harçlıklar', HESAPKOD='83005041001' WHERE YIL=2021 AND HESAPKOD='83005040190'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Eğitim Amaçlı Diğer Transferler', HESAPKOD='830050420' WHERE YIL=2021 AND HESAPKOD='830050402'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Parasız Yatılı Öğrenci Pansiyon Ücretleri', HESAPKOD='83005042001' WHERE YIL=2021 AND HESAPKOD='83005040201'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yabancı Uyruklu Kişilere Eğitim Amaçlı Transferler', HESAPKOD='83005042002' WHERE YIL=2021 AND HESAPKOD='83005040202'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sağlık Amaçlı Transferler', HESAPKOD='830050430' WHERE YIL=2021 AND HESAPKOD='830050403'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sağlık Amaçlı Transferler', HESAPKOD='83005043001' WHERE YIL=2021 AND HESAPKOD='83005040301'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sosyal Güvenliği Bulunmayanların Prim Ödemelerine İlişkin Transferler  ', HESAPKOD='83005043002' WHERE YIL=2021 AND HESAPKOD='83005040302'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yiyecek Amaçlı Transferler', HESAPKOD='830050440' WHERE YIL=2021 AND HESAPKOD='830050404'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yiyecek Amaçlı Transferler', HESAPKOD='83005044001' WHERE YIL=2021 AND HESAPKOD='83005040401'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Barınma Amaçlı Transferler', HESAPKOD='830050450' WHERE YIL=2021 AND HESAPKOD='830050405'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Koruyucu Ailelere Yardım Ödemeleri', HESAPKOD='83005045001' WHERE YIL=2021 AND HESAPKOD='83005040501'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Barınma Amaçlı Transferler', HESAPKOD='83005045090' WHERE YIL=2021 AND HESAPKOD='83005040590'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Alan Bazlı Tarımsal Destekler', HESAPKOD='830050460' WHERE YIL=2021 AND HESAPKOD='830050406'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Doğrudan Gelir Desteği Ödemeleri', HESAPKOD='83005046912' WHERE YIL=2021 AND HESAPKOD='83005040601'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Ürün Destekleme Ödemeleri', HESAPKOD='83005046913' WHERE YIL=2021 AND HESAPKOD='83005040602'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Hayvancılık Destekleme Ödemeleri', HESAPKOD='83005046914' WHERE YIL=2021 AND HESAPKOD='83005040603'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Tarım Reformu Uygulama Projesi', HESAPKOD='83005046915' WHERE YIL=2021 AND HESAPKOD='83005040604'
DELETE FROM  MUHHESAPPLANI WHERE YIL=2021 AND HESAPKOD='83005040605'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Çevre Amaçlı Tarımsal Alanların Korunması (ÇATAK) Desteği', HESAPKOD='83005046902' WHERE YIL=2021 AND HESAPKOD='83005040606'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='İlave Doğrudan Gelir Desteği Ödemeleri', HESAPKOD='83005046916' WHERE YIL=2021 AND HESAPKOD='83005040607'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Mazot Desteği', HESAPKOD='83005046001' WHERE YIL=2021 AND HESAPKOD='83005040608'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Gübre Desteği', HESAPKOD='83005046002' WHERE YIL=2021 AND HESAPKOD='83005040609'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Alan Bazlı Tarımsal Destekler', HESAPKOD='83005046090' WHERE YIL=2021 AND HESAPKOD='83005040610'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kütlü Pamuk Desteği', HESAPKOD='83005046501' WHERE YIL=2021 AND HESAPKOD='83005040611'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yağlık Ayçiçeği Desteği', HESAPKOD='83005046502' WHERE YIL=2021 AND HESAPKOD='83005040612'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Soya Fasülyesi Desteği', HESAPKOD='83005046503' WHERE YIL=2021 AND HESAPKOD='83005040613'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kanola Desteği', HESAPKOD='83005046504' WHERE YIL=2021 AND HESAPKOD='83005040614'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Dane Mısır Desteği', HESAPKOD='83005046506' WHERE YIL=2021 AND HESAPKOD='83005040615'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Zeytinyağı Desteği', HESAPKOD='83005046507' WHERE YIL=2021 AND HESAPKOD='83005040616'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Aspir Desteği', HESAPKOD='83005046505' WHERE YIL=2021 AND HESAPKOD='83005040617'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Çay Desteği', HESAPKOD='83005046508' WHERE YIL=2021 AND HESAPKOD='83005040618'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Buğday Desteği', HESAPKOD='83005046509' WHERE YIL=2021 AND HESAPKOD='83005040619'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Çeltik Desteği', HESAPKOD='83005046510' WHERE YIL=2021 AND HESAPKOD='83005040620'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Hububat Destekleri', HESAPKOD='83005046590' WHERE YIL=2021 AND HESAPKOD='83005040621'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Fark Ödemesi Niteliğindeki Diğer Tarımsal Destekler', HESAPKOD='83005046591' WHERE YIL=2021 AND HESAPKOD='83005040622'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yem Bitkileri Üretimi Desteği', HESAPKOD='83005046103' WHERE YIL=2021 AND HESAPKOD='83005040623'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Alet ve Makine Desteği', HESAPKOD='83005046119' WHERE YIL=2021 AND HESAPKOD='83005040624'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sertifikalı Yem Bitkileri Tohumluğu Üretim Desteği', HESAPKOD='83005046120' WHERE YIL=2021 AND HESAPKOD='83005040625'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Damızlık Büyükbaş Hayvan (Düve) Desteği', HESAPKOD='83005046121' WHERE YIL=2021 AND HESAPKOD='83005040626'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Suni Tohumlama Desteği', HESAPKOD='83005046122' WHERE YIL=2021 AND HESAPKOD='83005040627'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Çiğ Süt Desteği', HESAPKOD='83005046105' WHERE YIL=2021 AND HESAPKOD='83005040628'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Et Desteği', HESAPKOD='83005046123' WHERE YIL=2021 AND HESAPKOD='83005040629'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Buzağı Desteği', HESAPKOD='83005046106' WHERE YIL=2021 AND HESAPKOD='83005040630'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Arıcılık ve Bal Desteği', HESAPKOD='83005046107' WHERE YIL=2021 AND HESAPKOD='83005040631'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Hastalıklardan Ari İşletme Desteği', HESAPKOD='83005046108' WHERE YIL=2021 AND HESAPKOD='83005040632'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Aşı Desteği', HESAPKOD='83005046109' WHERE YIL=2021 AND HESAPKOD='83005040633'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Su Ürünleri Desteği', HESAPKOD='83005046117' WHERE YIL=2021 AND HESAPKOD='83005040634'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sağım Hijyeni ve Süt Kalitesi Desteği', HESAPKOD='83005046124' WHERE YIL=2021 AND HESAPKOD='83005040635'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Islah Amaçlı Küçükbaş Hayvan Yetiştirici Birlikleri Desteği', HESAPKOD='83005046110' WHERE YIL=2021 AND HESAPKOD='83005040636'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Hayvan Gen Kaynakları Desteği', HESAPKOD='83005046118' WHERE YIL=2021 AND HESAPKOD='83005040637'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Tiftik Üretimi Desteği', HESAPKOD='83005046111' WHERE YIL=2021 AND HESAPKOD='83005040638'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='İpek Böceği Desteği', HESAPKOD='83005046112' WHERE YIL=2021 AND HESAPKOD='83005040639'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Gıda Güvenliği Desteği', HESAPKOD='83005046125' WHERE YIL=2021 AND HESAPKOD='83005040640'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Hayvan Kimlik Sistemi Desteği', HESAPKOD='83005046126' WHERE YIL=2021 AND HESAPKOD='83005040641'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kanatlı Hayvan Hastalıkları Tazminatı Desteği', HESAPKOD='83005046127' WHERE YIL=2021 AND HESAPKOD='83005040642'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Büyükbaş Hayvan Hastalıkları Tazminatı Desteği', HESAPKOD='83005046101' WHERE YIL=2021 AND HESAPKOD='83005040643'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Büyükbaş Hayvan Desteği', HESAPKOD='83005046128' WHERE YIL=2021 AND HESAPKOD='83005040644'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Çevre Amaçlı Tarımsal Alanların Korunması (ÇATAK) Desteği', HESAPKOD='83005046902' WHERE YIL=2021 AND HESAPKOD='83005040645'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Alternatif Ürün Desteği', HESAPKOD='83005046012' WHERE YIL=2021 AND HESAPKOD='83005040646'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sınıflandırmaya Girmeyen Diğer Tarımsal Destekler', HESAPKOD='83005046990' WHERE YIL=2021 AND HESAPKOD='83005040647'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sınıflandırmaya Girmeyen Diğer Tarımsal Destekler', HESAPKOD='83005046990' WHERE YIL=2021 AND HESAPKOD='83005040648'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kırsal Kalkınma Destekleri', HESAPKOD='83005046401' WHERE YIL=2021 AND HESAPKOD='83005040649'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Bitkisel Ürün Sigortası Desteği', HESAPKOD='83005046201' WHERE YIL=2021 AND HESAPKOD='83005040650'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Süt Sığırı Sigortası Desteği', HESAPKOD='83005046202' WHERE YIL=2021 AND HESAPKOD='83005040651'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sera Sigortası Desteği', HESAPKOD='83005046203' WHERE YIL=2021 AND HESAPKOD='83005040652'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kümes Hayvanları Sigortası Desteği', HESAPKOD='83005046204' WHERE YIL=2021 AND HESAPKOD='83005040653'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Hasar Fazlası Desteği', HESAPKOD='83005046205' WHERE YIL=2021 AND HESAPKOD='83005040654'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Tarım Sigortası Destekleri', HESAPKOD='83005046290' WHERE YIL=2021 AND HESAPKOD='83005040655'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Patates Siğili Desteği', HESAPKOD='83005046301' WHERE YIL=2021 AND HESAPKOD='83005040656'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Çay Budama Tazminatı Desteği', HESAPKOD='83005046303' WHERE YIL=2021 AND HESAPKOD='83005040657'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Çay Budama Masraflarını Karşılama Desteği', HESAPKOD='83005046304' WHERE YIL=2021 AND HESAPKOD='83005040658'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sınıflandırmaya Girmeyen Diğer Tarımsal Destekler', HESAPKOD='83005046990' WHERE YIL=2021 AND HESAPKOD='83005040659'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Tarımsal Ar-Ge Destekleri', HESAPKOD='83005046910' WHERE YIL=2021 AND HESAPKOD='83005040660'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sertifikalı Tohum Kullanım Desteği', HESAPKOD='83005046903' WHERE YIL=2021 AND HESAPKOD='83005040661'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sertifikalı Fidan Kullanım Desteği', HESAPKOD='83005046904' WHERE YIL=2021 AND HESAPKOD='83005040662'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sınıflandırmaya Girmeyen Diğer Tarımsal Destekler', HESAPKOD='83005046990' WHERE YIL=2021 AND HESAPKOD='83005040663'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sınıflandırmaya Girmeyen Diğer Tarımsal Destekler', HESAPKOD='83005046990' WHERE YIL=2021 AND HESAPKOD='83005040664'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Çiftlik Muhasebe Veri Ağı (ÇMVA) Katılım Desteği', HESAPKOD='83005046907' WHERE YIL=2021 AND HESAPKOD='83005040665'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kuraklık Desteği', HESAPKOD='83005046013' WHERE YIL=2021 AND HESAPKOD='83005040666'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Toprak Analizi Desteği', HESAPKOD='83005046007' WHERE YIL=2021 AND HESAPKOD='83005040667'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Organik Tarım Desteği', HESAPKOD='83005046005' WHERE YIL=2021 AND HESAPKOD='83005040668'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='İyi Tarım Uygulamaları Desteği', HESAPKOD='83005046006' WHERE YIL=2021 AND HESAPKOD='83005040669'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Bakliyat Desteği', HESAPKOD='83005046511' WHERE YIL=2021 AND HESAPKOD='83005040670'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Büyükbaş Hayvan Desteği', HESAPKOD='83005046128' WHERE YIL=2021 AND HESAPKOD='83005040671'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sınıflandırmaya Girmeyen Diğer Tarımsal Destekler', HESAPKOD='83005046990' WHERE YIL=2021 AND HESAPKOD='83005040672'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Su Ürünleri Sigortası Desteği', HESAPKOD='83005046206' WHERE YIL=2021 AND HESAPKOD='83005040673'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sertifikalı Tohum Üretim Desteği', HESAPKOD='83005046905' WHERE YIL=2021 AND HESAPKOD='83005040674'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Tarım Danışmanlığı Desteği', HESAPKOD='83005046901' WHERE YIL=2021 AND HESAPKOD='83005040675'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Bombus Arısı Desteği', HESAPKOD='83005046104' WHERE YIL=2021 AND HESAPKOD='83005040676'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Fındık Üreticileri Alan Bazlı Gelir Desteği', HESAPKOD='83005046004' WHERE YIL=2021 AND HESAPKOD='83005040677'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Fındık Alternatif Ürün Desteği', HESAPKOD='83005046010' WHERE YIL=2021 AND HESAPKOD='83005040678'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Fındık Söküm Tazminatı', HESAPKOD='83005046011' WHERE YIL=2021 AND HESAPKOD='83005040679'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sınıflandırmaya Girmeyen Diğer Tarımsal Destekler', HESAPKOD='83005046990' WHERE YIL=2021 AND HESAPKOD='83005040680'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Büyükbaş Hayvan Besi Desteği', HESAPKOD='83005046113' WHERE YIL=2021 AND HESAPKOD='83005040681'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Küçükbaş Hayvan Sigortası Desteği', HESAPKOD='83005046207' WHERE YIL=2021 AND HESAPKOD='83005040682'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Süt Regülasyonu Desteği', HESAPKOD='83005046114' WHERE YIL=2021 AND HESAPKOD='83005040683'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Biyolojik Mücadele Desteği', HESAPKOD='83005046102' WHERE YIL=2021 AND HESAPKOD='83005040684'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='IPARD Ulusal Eş Finansman Katkı Payı Ödemeleri', HESAPKOD='83005046402' WHERE YIL=2021 AND HESAPKOD='83005040685'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sürü Yöneticisi İstihdamı Desteği', HESAPKOD='83005046115' WHERE YIL=2021 AND HESAPKOD='83005040686'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Lisanslı Depolarda Ürün Depolama Kira Desteği', HESAPKOD='83005046908' WHERE YIL=2021 AND HESAPKOD='83005040687'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Beş Dekar Altındaki Küçük İşletme Desteği', HESAPKOD='83005046008' WHERE YIL=2021 AND HESAPKOD='83005040688'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Genç Çiftçilere Proje Desteği', HESAPKOD='83005046403' WHERE YIL=2021 AND HESAPKOD='83005040689'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sınıflandırmaya Girmeyen Diğer Tarımsal Destekler', HESAPKOD='83005046990' WHERE YIL=2021 AND HESAPKOD='83005040690'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Büyükbaş Damızlık Hayvan Alım Desteği', HESAPKOD='83005046116' WHERE YIL=2021 AND HESAPKOD='83005040693'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sertifikalı Fidan Üretim Desteği', HESAPKOD='83005046906' WHERE YIL=2021 AND HESAPKOD='83005040694'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Geleneksel Zeytin Bahçelerinin Rehabilitasyonu Desteği', HESAPKOD='83005046009' WHERE YIL=2021 AND HESAPKOD='83005040695'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Tarımsal Sulama Elektrik Desteği', HESAPKOD='83005046909' WHERE YIL=2021 AND HESAPKOD='83005040696'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Bitki Karantinası Tazminatı Desteği', HESAPKOD='83005046302' WHERE YIL=2021 AND HESAPKOD='83005040698'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sosyal Amaçlı Transferler', HESAPKOD='830050470' WHERE YIL=2021 AND HESAPKOD='830050407'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Muhtaç ve Engellilere Yardım', HESAPKOD='83005047001' WHERE YIL=2021 AND HESAPKOD='83005040701'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Vakıf İlgililerine Yapılan Ödemeler', HESAPKOD='83005047002' WHERE YIL=2021 AND HESAPKOD='83005040702'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Vatani Hizmet Aylıkları', HESAPKOD='83005047003' WHERE YIL=2021 AND HESAPKOD='83005040703'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sosyal Amaçlı Diğer Aylık ve Tazminatlar', HESAPKOD='83005047004' WHERE YIL=2021 AND HESAPKOD='83005040704'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Şehit Dul ve Yetimlere İkramiye ve Ek Ödeme', HESAPKOD='83005047005' WHERE YIL=2021 AND HESAPKOD='83005040705'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Sosyal Amaçlı Transferler', HESAPKOD='83005047090' WHERE YIL=2021 AND HESAPKOD='83005040790'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Ekonomik/Mali Amaçlı Transferler', HESAPKOD='830050480' WHERE YIL=2021 AND HESAPKOD='830050408'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Ekonomik/Mali Amaçlı Diğer Transferler', HESAPKOD='83005048090' WHERE YIL=2021 AND HESAPKOD='83005040801'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='KREDİ FAİZ DESTEKLERİ', HESAPKOD='8300504809001' WHERE YIL=2021 AND HESAPKOD='8300504080101'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='GENEL DESTEKLER', HESAPKOD='8300504809002' WHERE YIL=2021 AND HESAPKOD='8300504080102'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='AR-GE, İNOVASYON VE ENDÜSTRİYEL UYGULAMA DESTEKLERİ', HESAPKOD='8300504809003' WHERE YIL=2021 AND HESAPKOD='8300504080103'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='GİRİŞİMCİLİK DESTEKLERİ', HESAPKOD='8300504809004' WHERE YIL=2021 AND HESAPKOD='8300504080104'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='İŞBİRLİĞİ-GÜÇBİRLİĞİ DESTEKLERİ', HESAPKOD='8300504809005' WHERE YIL=2021 AND HESAPKOD='8300504080105'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='GELİŞEN İŞLETMELER PİYASASI KOBİ DESTEKLERİ', HESAPKOD='8300504809006' WHERE YIL=2021 AND HESAPKOD='8300504080106'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='TEKNOLOJİK ÜRÜN TANITIM VE PAZARLAMA (TEKNOPAZAR) DESTEKLERİ', HESAPKOD='8300504809007' WHERE YIL=2021 AND HESAPKOD='8300504080107'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='ULUSLARARASI KULUÇKA MERKEZİ VE HIZLANDIRICI DESTEKLERİ', HESAPKOD='8300504809008' WHERE YIL=2021 AND HESAPKOD='8300504080108'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='TEMİNAT GİDERLERİ DESTEKLERİ', HESAPKOD='8300504809009' WHERE YIL=2021 AND HESAPKOD='8300504080109'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='KOBİ GELİŞİM (KOBİGEL) DESTEKLERİ', HESAPKOD='8300504809010' WHERE YIL=2021 AND HESAPKOD='8300504080110'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='STRATEJİK ÜRÜN DESTEKLERİ', HESAPKOD='8300504809011' WHERE YIL=2021 AND HESAPKOD='8300504080111'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='KOBİ TEKNOLOJİK ÜRÜN YATIRIM (KOBİ TEKNOYATIRIM) DESTEKLERİ', HESAPKOD='8300504809012' WHERE YIL=2021 AND HESAPKOD='8300504080112'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='İŞLETME GELİŞTİRME DESTEKLERİ', HESAPKOD='8300504809013' WHERE YIL=2021 AND HESAPKOD='8300504080113'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yurt Dışı Pazar Destek Programı', HESAPKOD='8300504809014' WHERE YIL=2021 AND HESAPKOD='8300504080114'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='GİRİŞİMCİLİĞİ GELİŞTİRME DESTEKLERİ', HESAPKOD='8300504809015' WHERE YIL=2021 AND HESAPKOD='8300504080115'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='İŞBİRLİĞİ DESTEKLERİ', HESAPKOD='8300504809016' WHERE YIL=2021 AND HESAPKOD='8300504080116'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='GELENEKSEL GİRİŞİMCİ DESTEKLERİ', HESAPKOD='8300504809017' WHERE YIL=2021 AND HESAPKOD='8300504080117'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='İLERİ GİRİŞİMCİ DESTEKLERİ', HESAPKOD='8300504809018' WHERE YIL=2021 AND HESAPKOD='8300504080118'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='İŞGEM/TEKMER DESTEKLERİ', HESAPKOD='8300504809019' WHERE YIL=2021 AND HESAPKOD='8300504080119'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='İŞ PLANI ÖDÜLÜ DESTEKLERİ', HESAPKOD='8300504809020' WHERE YIL=2021 AND HESAPKOD='8300504080120'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='TEMATİK PROJE DESTEKLERİ', HESAPKOD='8300504809021' WHERE YIL=2021 AND HESAPKOD='8300504080121'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='KOBİ PROJE DESTEKLERİ', HESAPKOD='8300504809022' WHERE YIL=2021 AND HESAPKOD='8300504080122'
DELETE FROM  MUHHESAPPLANI WHERE YIL=2021 AND HESAPKOD='83005040810'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Hane Halkına ve İşletmelere Yapılan Diğer Transferler', HESAPKOD='830050490' WHERE YIL=2021 AND HESAPKOD='830050409'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sınıflandırmaya Girmeyen Hane Halkı ve İşletmelere Yapılan Diğer Transferler', HESAPKOD='83005049090' WHERE YIL=2021 AND HESAPKOD='83005040901'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Türk Kültür Varlığının Korunması ve Tanıtımı Giderleri', HESAPKOD='83005049001' WHERE YIL=2021 AND HESAPKOD='83005040908'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Siyasi Tanıtma Giderleri', HESAPKOD='83005049002' WHERE YIL=2021 AND HESAPKOD='83005040909'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sağlık Personelinin Mali Sorumluluk Sigortası Giderleri ', HESAPKOD='83005049003' WHERE YIL=2021 AND HESAPKOD='83005040910'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yurtdışına Yapılan Transferler', HESAPKOD='8300506' WHERE YIL=2021 AND HESAPKOD='8300506'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Dış Ülkelere Yapılan Yardımlar', HESAPKOD='830050610' WHERE YIL=2021 AND HESAPKOD='830050601'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kuzey Kıbrıs Türk Cumhuriyeti ne', HESAPKOD='83005061001' WHERE YIL=2021 AND HESAPKOD='83005060101'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Ülkelere', HESAPKOD='83005061090' WHERE YIL=2021 AND HESAPKOD='83005060190'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Uluslararası Kuruluşlara Yapılan Ödemeler', HESAPKOD='830050620' WHERE YIL=2021 AND HESAPKOD='830050602'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Uluslararası Kuruluşlara Üyelik Aidatı Ödemeleri', HESAPKOD='83005062001' WHERE YIL=2021 AND HESAPKOD='83005060201'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Uluslararası Kuruluşlara Katkı Ödemeleri', HESAPKOD='83005062002' WHERE YIL=2021 AND HESAPKOD='83005060202'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Uluslararası Kuruluşlara Yardım Ödemeleri', HESAPKOD='83005062003' WHERE YIL=2021 AND HESAPKOD='83005060203'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Uluslararası Kuruluşlara Diğer Ödemeler', HESAPKOD='83005062090' WHERE YIL=2021 AND HESAPKOD='83005060290'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Uluslarüstü Kuruluşlara', HESAPKOD='830050630' WHERE YIL=2021 AND HESAPKOD='830050603'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Uluslarüstü Kuruluşlara', HESAPKOD='83005063001' WHERE YIL=2021 AND HESAPKOD='83005060301'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Uluslarüstü Kuruluşlardan Merkezlerine ', HESAPKOD='830050640' WHERE YIL=2021 AND HESAPKOD='830050604'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Uluslarüstü Kuruluşlardan Merkezlerine', HESAPKOD='83005064001' WHERE YIL=2021 AND HESAPKOD='83005060401'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Eğitim Kurumlarına', HESAPKOD='830050650' WHERE YIL=2021 AND HESAPKOD='830050605'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Hoca Ahmet Yesevi Uluslararası Türk Kazak Üniversitesine', HESAPKOD='83005065001' WHERE YIL=2021 AND HESAPKOD='83005060501'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kırgızistan-Türkiye Manas Üniversitesine', HESAPKOD='83005065002' WHERE YIL=2021 AND HESAPKOD='83005060502'
DELETE FROM  MUHHESAPPLANI WHERE YIL=2021 AND HESAPKOD='83005060503'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yurtdışına Yapılan Diğer Transferler', HESAPKOD='830050690' WHERE YIL=2021 AND HESAPKOD='830050609'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sınıflandırmaya Girmeyen Diğer Yurtdışı Transferler', HESAPKOD='83005069090' WHERE YIL=2021 AND HESAPKOD='83005060901'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Türk Kültür Varlığının Korunması ve Tanıtımı Giderleri', HESAPKOD='83005069001' WHERE YIL=2021 AND HESAPKOD='83005060908'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Siyasi Tanıtma Giderleri', HESAPKOD='83005069002' WHERE YIL=2021 AND HESAPKOD='83005060909'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Gelirlerden Ayrılan Paylar', HESAPKOD='8300508' WHERE YIL=2021 AND HESAPKOD='8300508'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Genel Bütçeye Verilen Paylar', HESAPKOD='830050810' WHERE YIL=2021 AND HESAPKOD='830050801'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Genel Bütçeye Verilen Paylar', HESAPKOD='83005081001' WHERE YIL=2021 AND HESAPKOD='83005080101'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Evrensel Hizmet Katkı Payı', HESAPKOD='83005081002' WHERE YIL=2021 AND HESAPKOD='83005080103'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Elektronik Haberleşme Sektörüne İlişkin Ar-Ge Payı', HESAPKOD='83005081003' WHERE YIL=2021 AND HESAPKOD='83005080104'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Özel Bütçeli İdarelere Verilen Paylar', HESAPKOD='830050820' WHERE YIL=2021 AND HESAPKOD='830050802'
DELETE FROM  MUHHESAPPLANI WHERE YIL=2021 AND HESAPKOD='83005080201'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yükseköğrenim Kredi Yurtlar Kurumuna Verilen Paylar', HESAPKOD='83005082001' WHERE YIL=2021 AND HESAPKOD='83005080202'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Türkiye Akreditasyon Kurumuna Verilen Paylar', HESAPKOD='83005082002' WHERE YIL=2021 AND HESAPKOD='83005080203'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Türkiye Uzay Ajansına Verilen Paylar', HESAPKOD='83005082003' WHERE YIL=2021 AND HESAPKOD='83005080204'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Düzenleyici ve Denetleyici Kurumlara Verilen Paylar', HESAPKOD='830050830' WHERE YIL=2021 AND HESAPKOD='830050803'
DELETE FROM  MUHHESAPPLANI WHERE YIL=2021 AND HESAPKOD='83005080301'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Enerji Piyasası Düzenleme Kurumuna Verilen Paylar ', HESAPKOD='83005083001' WHERE YIL=2021 AND HESAPKOD='83005080302'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sosyal Güvenlik Kurumlarına Verilen Paylar', HESAPKOD='830050840' WHERE YIL=2021 AND HESAPKOD='830050804'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sosyal Güvenlik Kurumlarına Verilen Paylar', HESAPKOD='83005084001' WHERE YIL=2021 AND HESAPKOD='83005080401'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Mahalli İdarelere Verilen Paylar', HESAPKOD='830050850' WHERE YIL=2021 AND HESAPKOD='830050805'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Genel Bütçe Gelirlerinden İl Özel İdarelerine Ayrılan Paylar', HESAPKOD='83005085001' WHERE YIL=2021 AND HESAPKOD='83005080501'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Genel Bütçe Gelirlerinden Büyükşehir Belediyelere Ayrılan Paylar', HESAPKOD='83005085002' WHERE YIL=2021 AND HESAPKOD='83005080502'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Genel Bütçe Gelirlerinden Diğer Belediyelere Ayrılan Paylar', HESAPKOD='83005085003' WHERE YIL=2021 AND HESAPKOD='83005080503'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Genel Bütçe Gelirlerinden Büyükşehir İlçe Belediyelerine Ayrılan Paylar', HESAPKOD='83005085004' WHERE YIL=2021 AND HESAPKOD='83005080504'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Fonlara Verilen Paylar', HESAPKOD='830050860' WHERE YIL=2021 AND HESAPKOD='830050806'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Savunma Sanayi Destekleme Fonuna Verilen Paylar', HESAPKOD='83005086001' WHERE YIL=2021 AND HESAPKOD='83005080601'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sosyal Yardımlaşma ve Dayanışmayı Teşvik Fonuna Verilen Paylar', HESAPKOD='83005086002' WHERE YIL=2021 AND HESAPKOD='83005080602'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Tanıtma Fonuna Verilen Paylar', HESAPKOD='83005086004' WHERE YIL=2021 AND HESAPKOD='83005080603'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sivil Savunma Fonuna Verilen Paylar ', HESAPKOD='83005086003' WHERE YIL=2021 AND HESAPKOD='83005080604'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Döner Sermayelere Verilen Paylar', HESAPKOD='830050870' WHERE YIL=2021 AND HESAPKOD='830050807'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Döner Sermayelere Verilen Paylar', HESAPKOD='83005087001' WHERE YIL=2021 AND HESAPKOD='83005080701'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Döner Sermayelere Verilen Paylar', HESAPKOD='83005087001' WHERE YIL=2021 AND HESAPKOD='83005080702'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğerlerine Verilen Paylar', HESAPKOD='830050890' WHERE YIL=2021 AND HESAPKOD='830050809'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Türkiye Barolar Birliğine Verilen Paylar', HESAPKOD='83005089001' WHERE YIL=2021 AND HESAPKOD='83005080901'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Türkiye Odalar ve Borsalar Birliğine Verilen Paylar', HESAPKOD='83005089002' WHERE YIL=2021 AND HESAPKOD='83005080902'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Türkiye Esnaf ve Sanatkarlar Konfederasyonuna Bağlı Odalara Verilen Paylar', HESAPKOD='83005089003' WHERE YIL=2021 AND HESAPKOD='83005080903'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Olimpiyat Oyunları Hazırlık ve Düzenleme Kuruluna Verilen Paylar', HESAPKOD='83005089004' WHERE YIL=2021 AND HESAPKOD='83005080904'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Toplu Konut İdaresi Başkanlığına Verilen Paylar', HESAPKOD='83005089006' WHERE YIL=2021 AND HESAPKOD='83005080905'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Şeker Kurumuna Verilen Paylar', HESAPKOD='83005089005' WHERE YIL=2021 AND HESAPKOD='83005080906'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sermaye Giderleri', HESAPKOD='83006' WHERE YIL=2021 AND HESAPKOD='83006'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Mamul Mal Alımları', HESAPKOD='8300601' WHERE YIL=2021 AND HESAPKOD='8300601'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Büro ve İşyeri Mefruşatı Alımları', HESAPKOD='830060110' WHERE YIL=2021 AND HESAPKOD='830060101'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Büro Mefruşatı Alımları', HESAPKOD='83006011001' WHERE YIL=2021 AND HESAPKOD='83006010101'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='İşyeri Mefruşatı Alımları ', HESAPKOD='83006011002' WHERE YIL=2021 AND HESAPKOD='83006010102'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Okul Mefruşatı Alımları', HESAPKOD='83006011003' WHERE YIL=2021 AND HESAPKOD='83006010103'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Hastane Mefruşatı Alımları', HESAPKOD='83006011004' WHERE YIL=2021 AND HESAPKOD='83006010104'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sosyal Tesis Mefruşatı Alımları', HESAPKOD='83006011005' WHERE YIL=2021 AND HESAPKOD='83006010105'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Mefruşat Alımları', HESAPKOD='83006011090' WHERE YIL=2021 AND HESAPKOD='83006010190'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Büro ve İşyeri Makine Teçhizat Alımları', HESAPKOD='830060120' WHERE YIL=2021 AND HESAPKOD='830060102'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Büro Makinaları Alımları ', HESAPKOD='83006012001' WHERE YIL=2021 AND HESAPKOD='83006010201'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Bilgisayar Alımları', HESAPKOD='83006012002' WHERE YIL=2021 AND HESAPKOD='83006010202'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Tıbbi Cihaz Alımları', HESAPKOD='83006012003' WHERE YIL=2021 AND HESAPKOD='83006010203'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Laboratuar Cihazı Alımları', HESAPKOD='83006012004' WHERE YIL=2021 AND HESAPKOD='83006010204'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='İşyeri Makine Teçhizat Alımları', HESAPKOD='83006012005' WHERE YIL=2021 AND HESAPKOD='83006010205'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Makine Teçhizat Alımları', HESAPKOD='83006012090' WHERE YIL=2021 AND HESAPKOD='83006010290'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Avadanlık Alımları', HESAPKOD='830060130' WHERE YIL=2021 AND HESAPKOD='830060103'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Tamir Bakım Aleti Alımları', HESAPKOD='83006013001' WHERE YIL=2021 AND HESAPKOD='83006010301'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Atölye Gereçleri Alımları', HESAPKOD='83006013002' WHERE YIL=2021 AND HESAPKOD='83006010302'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Tıbbi Gereçler Alımları', HESAPKOD='83006013003' WHERE YIL=2021 AND HESAPKOD='83006010303'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Laboratuvar Gereçleri Alımları', HESAPKOD='83006013004' WHERE YIL=2021 AND HESAPKOD='83006010304'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Zirai Gereç Alımları', HESAPKOD='83006013005' WHERE YIL=2021 AND HESAPKOD='83006010305'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Avadanlık Alımları', HESAPKOD='83006013090' WHERE YIL=2021 AND HESAPKOD='83006010390'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Taşıt Alımları', HESAPKOD='830060140' WHERE YIL=2021 AND HESAPKOD='830060104'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kara Taşıtı  Alımları   ', HESAPKOD='83006014001' WHERE YIL=2021 AND HESAPKOD='83006010401'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Demiryolu Taşıtı  Alımları', HESAPKOD='83006014002' WHERE YIL=2021 AND HESAPKOD='83006010430'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Deniz ve Suyolu Taşıtı  Alımları', HESAPKOD='83006014003' WHERE YIL=2021 AND HESAPKOD='83006010440'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='İmal Ettirilecek Deniz ve Suyolu Taşıtları', HESAPKOD='83006014004' WHERE YIL=2021 AND HESAPKOD='83006010441'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Denizaltı Taşıtı  Alımları', HESAPKOD='83006014005' WHERE YIL=2021 AND HESAPKOD='83006010450'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Havayolu Taşıtı  Alımları', HESAPKOD='83006014006' WHERE YIL=2021 AND HESAPKOD='83006010460'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Uzay Taşıtı  Alımları', HESAPKOD='83006014007' WHERE YIL=2021 AND HESAPKOD='83006010470'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Taşıt Alımları', HESAPKOD='83006014090' WHERE YIL=2021 AND HESAPKOD='83006010490'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='İş Makinası ve Taşınır Tesis Alımları', HESAPKOD='830060150' WHERE YIL=2021 AND HESAPKOD='830060105'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sabit İş Makinası Alımları', HESAPKOD='83006015001' WHERE YIL=2021 AND HESAPKOD='83006010501'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Hareketli İş Makinası Alımları', HESAPKOD='83006015002' WHERE YIL=2021 AND HESAPKOD='83006010530'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yayın Alımları ve Yapımları', HESAPKOD='830060160' WHERE YIL=2021 AND HESAPKOD='830060106'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Basılı Yayın Alımları ve Yapımları', HESAPKOD='83006016001' WHERE YIL=2021 AND HESAPKOD='83006010601'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='El Yazması Alımları ve Yapımları', HESAPKOD='83006016002' WHERE YIL=2021 AND HESAPKOD='83006010602'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Elektronik Ortamda Yayın Alımları ve Yapımları', HESAPKOD='83006016003' WHERE YIL=2021 AND HESAPKOD='83006010603'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Görüntülü Yayın Alımları ve Yapımları', HESAPKOD='83006016004' WHERE YIL=2021 AND HESAPKOD='83006010604'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Yayın Alımları ve Yapımları', HESAPKOD='83006016090' WHERE YIL=2021 AND HESAPKOD='83006010690'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kültür Varlığı Yapımları, Alımları ve Korunması Giderleri', HESAPKOD='830060170' WHERE YIL=2021 AND HESAPKOD='830060107'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Arkeolojik Kazı Giderleri', HESAPKOD='83006017001' WHERE YIL=2021 AND HESAPKOD='83006010701'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Tablo-Heykel Yapım, Alım ve Onarımları', HESAPKOD='83006017002' WHERE YIL=2021 AND HESAPKOD='83006010702'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Eski Eser Alım ve Onarımları', HESAPKOD='83006017003' WHERE YIL=2021 AND HESAPKOD='83006010703'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Kültür Varlığı Yapım, Alım ve Korunması Giderleri', HESAPKOD='83006017090' WHERE YIL=2021 AND HESAPKOD='83006010790'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Menkul Sermaye Üretim Giderleri', HESAPKOD='8300602' WHERE YIL=2021 AND HESAPKOD='8300602'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Müşavir Firma ve Kişilere Ödemeler', HESAPKOD='830060210' WHERE YIL=2021 AND HESAPKOD='830060201'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Proje Giderleri', HESAPKOD='83006021001' WHERE YIL=2021 AND HESAPKOD='83006020101'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Müşavirlik Giderleri', HESAPKOD='83006021002' WHERE YIL=2021 AND HESAPKOD='83006020102'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kontrol Giderleri', HESAPKOD='83006021003' WHERE YIL=2021 AND HESAPKOD='83006020103'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Giderler', HESAPKOD='83006021090' WHERE YIL=2021 AND HESAPKOD='83006020190'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Hammadde Alımları', HESAPKOD='830060220' WHERE YIL=2021 AND HESAPKOD='830060202'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Hammadde Alımları', HESAPKOD='83006022001' WHERE YIL=2021 AND HESAPKOD='83006020201'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Gıda Ürünleri, İçecekler ve Tütün  Alımları', HESAPKOD='830060230' WHERE YIL=2021 AND HESAPKOD='830060203'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Gıda Ürünleri, İçecekler ve Tütün  Alımları', HESAPKOD='83006023001' WHERE YIL=2021 AND HESAPKOD='83006020301'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Tekstil ve Tekstil Ürünleri, Deri ve Deri Ürünleri  Alımları', HESAPKOD='830060240' WHERE YIL=2021 AND HESAPKOD='830060204'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Tekstil ve Tekstil Ürünleri, Deri ve Deri Ürünleri  Alımları', HESAPKOD='83006024001' WHERE YIL=2021 AND HESAPKOD='83006020401'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kereste ve Kereste Ürünleri  Alımları', HESAPKOD='830060250' WHERE YIL=2021 AND HESAPKOD='830060205'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kereste ve Kereste Ürünleri  Alımları', HESAPKOD='83006025001' WHERE YIL=2021 AND HESAPKOD='83006020501'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kağıt ve Kağıt Ürünleri  Alımları', HESAPKOD='830060260' WHERE YIL=2021 AND HESAPKOD='830060206'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kağıt ve Kağıt Ürünleri  Alımları', HESAPKOD='83006026001' WHERE YIL=2021 AND HESAPKOD='83006020601'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kimyevi Madde İle Kauçuk ve Plastik Ürün  Alımları', HESAPKOD='830060270' WHERE YIL=2021 AND HESAPKOD='830060207'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kimyevi Madde İle Kauçuk ve Plastik Ürün  Alımları', HESAPKOD='83006027001' WHERE YIL=2021 AND HESAPKOD='83006020701'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Metal Ürün  Alımları', HESAPKOD='830060280' WHERE YIL=2021 AND HESAPKOD='830060208'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Metal Ürün  Alımları', HESAPKOD='83006028001' WHERE YIL=2021 AND HESAPKOD='83006020801'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Menkul Sermaye Üretim Giderleri', HESAPKOD='830060290' WHERE YIL=2021 AND HESAPKOD='830060209'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Menkul Sermaye Üretim Giderleri', HESAPKOD='83006029001' WHERE YIL=2021 AND HESAPKOD='83006020901'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Gayri Maddi Hak Alımları', HESAPKOD='8300603' WHERE YIL=2021 AND HESAPKOD='8300603'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Bilgisayar Yazılımı Alımları', HESAPKOD='830060310' WHERE YIL=2021 AND HESAPKOD='830060301'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Bilgisayar Yazılımı Alımları', HESAPKOD='83006031001' WHERE YIL=2021 AND HESAPKOD='83006030101'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Harita Plan Proje Alımları', HESAPKOD='830060320' WHERE YIL=2021 AND HESAPKOD='830060302'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Harita Alımları', HESAPKOD='83006032001' WHERE YIL=2021 AND HESAPKOD='83006030201'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Plan Proje Alımları', HESAPKOD='83006032002' WHERE YIL=2021 AND HESAPKOD='83006030202'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Lisans Alımları', HESAPKOD='830060330' WHERE YIL=2021 AND HESAPKOD='830060303'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Lisans Alımları', HESAPKOD='83006033001' WHERE YIL=2021 AND HESAPKOD='83006030301'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Patent Alımları', HESAPKOD='830060340' WHERE YIL=2021 AND HESAPKOD='830060304'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Patent Alımları', HESAPKOD='83006034001' WHERE YIL=2021 AND HESAPKOD='83006030401'
DELETE FROM  MUHHESAPPLANI WHERE YIL=2021 AND HESAPKOD='830060308'
DELETE FROM  MUHHESAPPLANI WHERE YIL=2021 AND HESAPKOD='83006030801'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Fikri Hak Alımları', HESAPKOD='830060390' WHERE YIL=2021 AND HESAPKOD='830060309'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Fikri Hak Alımları', HESAPKOD='83006039001' WHERE YIL=2021 AND HESAPKOD='83006030901'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Gayrimenkul Alımları ve Kamulaştırması', HESAPKOD='8300604' WHERE YIL=2021 AND HESAPKOD='8300604'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Arazi Alım ve Kamulaştırma Giderleri', HESAPKOD='830060410' WHERE YIL=2021 AND HESAPKOD='830060401'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Zirai Arazi Alım ve Kamulaştırma Giderleri', HESAPKOD='83006041001' WHERE YIL=2021 AND HESAPKOD='83006040101'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sınai Arazi Alım ve Kamulaştırma Giderleri', HESAPKOD='83006041002' WHERE YIL=2021 AND HESAPKOD='83006040102'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='İskan Arazisi Alım ve Kamulaştırma Giderleri', HESAPKOD='83006041003' WHERE YIL=2021 AND HESAPKOD='83006040103'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Orman Arazisi Alım ve Kamulaştırma Giderleri', HESAPKOD='83006041004' WHERE YIL=2021 AND HESAPKOD='83006040104'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Turizm Arazisi Alım ve Kamulaştırma Giderleri', HESAPKOD='83006041005' WHERE YIL=2021 AND HESAPKOD='83006040105'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Göl ve Baraj için Arazi Alım ve Kamulaştırma Giderleri', HESAPKOD='83006041006' WHERE YIL=2021 AND HESAPKOD='83006040106'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yol için Arazi Alım ve Kamulaştırma Giderleri', HESAPKOD='83006041007' WHERE YIL=2021 AND HESAPKOD='83006040107'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Güvenlik ve Savunma İçin Arazi Alım ve Kamulaştırma Giderleri', HESAPKOD='83006041008' WHERE YIL=2021 AND HESAPKOD='83006040108'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='İçme Suyu Tesisi Arazi Alım ve Kamulaştırma Giderleri', HESAPKOD='83006041009' WHERE YIL=2021 AND HESAPKOD='83006040109'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kanalizasyon Yapımı İçin Arazi Alım ve Kamulaştırma Giderleri', HESAPKOD='83006041010' WHERE YIL=2021 AND HESAPKOD='83006040110'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Gayrimenkul Alım ve Kamulaştırma Giderleri', HESAPKOD='83006041090' WHERE YIL=2021 AND HESAPKOD='83006040190'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Arsa Alım ve Kamulaştırması Giderleri', HESAPKOD='830060420' WHERE YIL=2021 AND HESAPKOD='830060402'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Hizmet Binası İçin Arsa Alım ve Kamulaştırma Giderleri', HESAPKOD='83006042001' WHERE YIL=2021 AND HESAPKOD='83006040201'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Lojman İçin Arsa Alım ve Kamulaştırma Giderleri', HESAPKOD='83006042002' WHERE YIL=2021 AND HESAPKOD='83006040202'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sosyal Tesis İçin Arsa Alım ve Kamulaştırma Giderleri', HESAPKOD='83006042003' WHERE YIL=2021 AND HESAPKOD='83006040203'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Güvenlik ve Savunma İçin Arsa Alım ve Kamulaştırma Giderleri', HESAPKOD='83006042004' WHERE YIL=2021 AND HESAPKOD='83006040204'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yol Yapımı İçin Arsa Alım ve Kamulaştırma Giderleri', HESAPKOD='83006042005' WHERE YIL=2021 AND HESAPKOD='83006040205'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='İçme Suyu Tesisi Arsa Alım ve Kamulaştırma Giderleri', HESAPKOD='83006042006' WHERE YIL=2021 AND HESAPKOD='83006040206'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kanalizasyon Yapımı İçin Arsa Alım ve Kamulaştırma Giderleri', HESAPKOD='83006042007' WHERE YIL=2021 AND HESAPKOD='83006040207'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Arsa Alım ve Kamulaştırma Giderleri', HESAPKOD='83006042090' WHERE YIL=2021 AND HESAPKOD='83006040290'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Bina Alım ve Kamulaştırma Giderleri', HESAPKOD='830060430' WHERE YIL=2021 AND HESAPKOD='830060403'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Hizmet Binası İçin Alım ve Kamulaştırma Giderleri', HESAPKOD='83006043001' WHERE YIL=2021 AND HESAPKOD='83006040301'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Lojman İçin Bina Alım ve Kamulaştırma Giderleri', HESAPKOD='83006043002' WHERE YIL=2021 AND HESAPKOD='83006040302'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sosyal Tesis İçin Bina Alım ve Kamulaştırma Giderleri', HESAPKOD='83006043003' WHERE YIL=2021 AND HESAPKOD='83006040303'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Bina Alım ve Kamulaştırma Giderleri', HESAPKOD='83006043090' WHERE YIL=2021 AND HESAPKOD='83006040390'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Gemi Alımları', HESAPKOD='830060440' WHERE YIL=2021 AND HESAPKOD='830060404'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yolcu Gemisi Alımları', HESAPKOD='83006044001' WHERE YIL=2021 AND HESAPKOD='83006040401'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yük Gemisi Alımları', HESAPKOD='83006044002' WHERE YIL=2021 AND HESAPKOD='83006040402'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Araştırma Gemisi Alımları', HESAPKOD='83006044003' WHERE YIL=2021 AND HESAPKOD='83006040403'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Feribot Alımları', HESAPKOD='83006044004' WHERE YIL=2021 AND HESAPKOD='83006040404'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Tanker Alımları', HESAPKOD='83006044005' WHERE YIL=2021 AND HESAPKOD='83006040405'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Tersane Alımları', HESAPKOD='830060450' WHERE YIL=2021 AND HESAPKOD='830060405'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Tersane Alımları', HESAPKOD='83006045001' WHERE YIL=2021 AND HESAPKOD='83006040501'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yüzer Tersane Alımları', HESAPKOD='83006045002' WHERE YIL=2021 AND HESAPKOD='83006040502'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Gayrimenkul Sermaye Üretim Giderleri', HESAPKOD='8300605' WHERE YIL=2021 AND HESAPKOD='8300605'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Müşavir Firma ve Kişilere Ödemeler', HESAPKOD='830060510' WHERE YIL=2021 AND HESAPKOD='830060501'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Proje Giderleri', HESAPKOD='83006051001' WHERE YIL=2021 AND HESAPKOD='83006050101'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Müşavirlik Giderleri', HESAPKOD='83006051002' WHERE YIL=2021 AND HESAPKOD='83006050102'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kontrol Giderleri', HESAPKOD='83006051003' WHERE YIL=2021 AND HESAPKOD='83006050103'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Müşavir Firma ve Kişilere Diğer Ödemeler', HESAPKOD='83006051090' WHERE YIL=2021 AND HESAPKOD='83006050190'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Malzeme Giderleri', HESAPKOD='830060520' WHERE YIL=2021 AND HESAPKOD='830060502'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='İnşaat Malzemesi Giderleri', HESAPKOD='83006052001' WHERE YIL=2021 AND HESAPKOD='83006050201'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Elektrik Tesisatı Giderleri', HESAPKOD='83006052002' WHERE YIL=2021 AND HESAPKOD='83006050202'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sıhhi Tesisat Giderleri', HESAPKOD='83006052003' WHERE YIL=2021 AND HESAPKOD='83006050203'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Özel Tesisat Giderleri', HESAPKOD='83006052004' WHERE YIL=2021 AND HESAPKOD='83006050204'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Giderler', HESAPKOD='83006052090' WHERE YIL=2021 AND HESAPKOD='83006050290'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Taşıma Giderleri', HESAPKOD='830060530' WHERE YIL=2021 AND HESAPKOD='830060503'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Taşımaya ilişkin Beslenme, Barındırma Giderleri', HESAPKOD='83006053001' WHERE YIL=2021 AND HESAPKOD='83006050301'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yolcu Taşıma Giderleri', HESAPKOD='83006053002' WHERE YIL=2021 AND HESAPKOD='83006050302'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yük Taşıma Giderleri', HESAPKOD='83006053003' WHERE YIL=2021 AND HESAPKOD='83006050303'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Taşıma Giderleri', HESAPKOD='83006053090' WHERE YIL=2021 AND HESAPKOD='83006050390'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Enerji Giderleri', HESAPKOD='830060540' WHERE YIL=2021 AND HESAPKOD='830060504'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yakacak  Alımları', HESAPKOD='83006054001' WHERE YIL=2021 AND HESAPKOD='83006050401'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Akaryakıt ve Yağ  Alımları', HESAPKOD='83006054002' WHERE YIL=2021 AND HESAPKOD='83006050402'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Elektrik Alımları', HESAPKOD='83006054003' WHERE YIL=2021 AND HESAPKOD='83006050403'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Enerji Alımları', HESAPKOD='83006054090' WHERE YIL=2021 AND HESAPKOD='83006050490'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Haberleşme Giderleri', HESAPKOD='830060550' WHERE YIL=2021 AND HESAPKOD='830060505'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Posta ve Telgraf Giderleri', HESAPKOD='83006055001' WHERE YIL=2021 AND HESAPKOD='83006050501'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Telefon Abonelik ve Kullanım Ücretleri', HESAPKOD='83006055002' WHERE YIL=2021 AND HESAPKOD='83006050502'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Bilgiye Abonelik ve İnternet Erişimi Giderleri', HESAPKOD='83006055003' WHERE YIL=2021 AND HESAPKOD='83006050503'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Haberleşme Cihazları Ruhsat ve Kullanım Giderleri', HESAPKOD='83006055004' WHERE YIL=2021 AND HESAPKOD='83006050504'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Uydu Haberleşme Giderleri', HESAPKOD='83006055005' WHERE YIL=2021 AND HESAPKOD='83006050505'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Hat Kira Giderleri', HESAPKOD='83006055006' WHERE YIL=2021 AND HESAPKOD='83006050506'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Haberleşme Giderleri', HESAPKOD='83006055090' WHERE YIL=2021 AND HESAPKOD='83006050590'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kira Giderleri', HESAPKOD='830060560' WHERE YIL=2021 AND HESAPKOD='830060506'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Dayanıklı Mal ve Malzeme Kiralaması Giderleri', HESAPKOD='83006056001' WHERE YIL=2021 AND HESAPKOD='83006050601'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Taşıt Kiralaması Giderleri', HESAPKOD='83006056002' WHERE YIL=2021 AND HESAPKOD='83006050602'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='İş Makinası Kiralaması Giderleri', HESAPKOD='83006056003' WHERE YIL=2021 AND HESAPKOD='83006050603'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Canlı Hayvan Kiralaması Giderleri', HESAPKOD='83006056004' WHERE YIL=2021 AND HESAPKOD='83006050604'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Bina Kiralaması Giderleri', HESAPKOD='83006056005' WHERE YIL=2021 AND HESAPKOD='83006050605'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Arazi Kiralaması Giderleri', HESAPKOD='83006056006' WHERE YIL=2021 AND HESAPKOD='83006050606'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yüzer Taşıt Kiralaması Giderleri', HESAPKOD='83006056007' WHERE YIL=2021 AND HESAPKOD='83006050607'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Uçak Kiralaması Giderleri', HESAPKOD='83006056008' WHERE YIL=2021 AND HESAPKOD='83006050608'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Bilgisayar ve Bilgisayar Sistemleri Kiralaması Giderleri', HESAPKOD='83006056009' WHERE YIL=2021 AND HESAPKOD='83006050609'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yüzer Tersane Kiralaması Giderleri', HESAPKOD='83006056010' WHERE YIL=2021 AND HESAPKOD='83006050610'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Kiralamalar', HESAPKOD='83006056090' WHERE YIL=2021 AND HESAPKOD='83006050690'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Müteahhitlik Giderleri', HESAPKOD='830060570' WHERE YIL=2021 AND HESAPKOD='830060507'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Hizmet Binası Yapım Giderleri', HESAPKOD='83006057001' WHERE YIL=2021 AND HESAPKOD='83006050701'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Hizmet Tesisi Yapım Giderleri', HESAPKOD='83006057002' WHERE YIL=2021 AND HESAPKOD='83006050702'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Lojman Yapım Giderleri', HESAPKOD='83006057003' WHERE YIL=2021 AND HESAPKOD='83006050703'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sosyal Tesis Yapım Giderleri', HESAPKOD='83006057004' WHERE YIL=2021 AND HESAPKOD='83006050704'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Gemi Yapım Giderleri', HESAPKOD='83006057005' WHERE YIL=2021 AND HESAPKOD='83006050705'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yüzer Tersane Yapım Giderleri', HESAPKOD='83006057006' WHERE YIL=2021 AND HESAPKOD='83006050706'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Karayolu Yapım Giderleri', HESAPKOD='83006057007' WHERE YIL=2021 AND HESAPKOD='83006050707'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='İçme Suyu Tesisi Yapım Giderleri', HESAPKOD='83006057008' WHERE YIL=2021 AND HESAPKOD='83006050708'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kanalizasyon Tesisi Yapım Giderleri', HESAPKOD='83006057009' WHERE YIL=2021 AND HESAPKOD='83006050709'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Müteahhitlik Giderleri', HESAPKOD='83006057090' WHERE YIL=2021 AND HESAPKOD='83006050790'
DELETE FROM  MUHHESAPPLANI WHERE YIL=2021 AND HESAPKOD='830060508'
DELETE FROM  MUHHESAPPLANI WHERE YIL=2021 AND HESAPKOD='83006050801'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Gayrimenkul Sermaye Üretim Giderleri', HESAPKOD='830060590' WHERE YIL=2021 AND HESAPKOD='830060509'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Gayrimenkul Sermaye Üretim Giderleri', HESAPKOD='83006059001' WHERE YIL=2021 AND HESAPKOD='83006050901'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Menkul Malların Büyük Onarım Giderleri', HESAPKOD='8300606' WHERE YIL=2021 AND HESAPKOD='8300606'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Müşavir Firma ve Kişilere Ödemeler', HESAPKOD='830060610' WHERE YIL=2021 AND HESAPKOD='830060601'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Proje Giderleri', HESAPKOD='83006061001' WHERE YIL=2021 AND HESAPKOD='83006060101'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Müşavirlik Giderleri', HESAPKOD='83006061002' WHERE YIL=2021 AND HESAPKOD='83006060102'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kontrol Giderleri', HESAPKOD='83006061003' WHERE YIL=2021 AND HESAPKOD='83006060103'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Müşavir Firma ve Kişilere Diğer Ödemeler', HESAPKOD='83006061090' WHERE YIL=2021 AND HESAPKOD='83006060190'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Malzeme Giderleri', HESAPKOD='830060620' WHERE YIL=2021 AND HESAPKOD='830060602'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Malzeme Giderleri', HESAPKOD='83006062001' WHERE YIL=2021 AND HESAPKOD='83006060201'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Taşıma Giderleri', HESAPKOD='830060630' WHERE YIL=2021 AND HESAPKOD='830060603'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Taşımaya ilişkin Beslenme, Barındırma Giderleri', HESAPKOD='83006063001' WHERE YIL=2021 AND HESAPKOD='83006060301'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yolcu Taşıma Giderleri', HESAPKOD='83006063002' WHERE YIL=2021 AND HESAPKOD='83006060302'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yük Taşıma Giderleri', HESAPKOD='83006063003' WHERE YIL=2021 AND HESAPKOD='83006060303'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Taşıma Giderleri', HESAPKOD='83006063090' WHERE YIL=2021 AND HESAPKOD='83006060390'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Enerji Giderleri', HESAPKOD='830060640' WHERE YIL=2021 AND HESAPKOD='830060604'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yakacak  Alımları', HESAPKOD='83006064001' WHERE YIL=2021 AND HESAPKOD='83006060401'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Akaryakıt ve Yağ  Alımları', HESAPKOD='83006064002' WHERE YIL=2021 AND HESAPKOD='83006060402'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Elektrik Alımları', HESAPKOD='83006064003' WHERE YIL=2021 AND HESAPKOD='83006060403'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Enerji Alımları', HESAPKOD='83006064090' WHERE YIL=2021 AND HESAPKOD='83006060490'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Haberleşme Giderleri', HESAPKOD='830060650' WHERE YIL=2021 AND HESAPKOD='830060605'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Posta ve Telgraf Giderleri', HESAPKOD='83006065001' WHERE YIL=2021 AND HESAPKOD='83006060501'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Telefon Abonelik ve Kullanım Ücretleri', HESAPKOD='83006065002' WHERE YIL=2021 AND HESAPKOD='83006060502'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Bilgiye Abonelik ve İnternet Erişimi Giderleri', HESAPKOD='83006065003' WHERE YIL=2021 AND HESAPKOD='83006060503'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Haberleşme Cihazları Ruhsat ve Kullanım Giderleri', HESAPKOD='83006065004' WHERE YIL=2021 AND HESAPKOD='83006060504'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Uydu Haberleşme Giderleri', HESAPKOD='83006065005' WHERE YIL=2021 AND HESAPKOD='83006060505'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Hat Kira Giderleri', HESAPKOD='83006065006' WHERE YIL=2021 AND HESAPKOD='83006060506'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Haberleşme Giderleri', HESAPKOD='83006065090' WHERE YIL=2021 AND HESAPKOD='83006060590'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kira Giderleri', HESAPKOD='830060660' WHERE YIL=2021 AND HESAPKOD='830060606'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Dayanıklı Mal ve Malzeme Kiralaması Giderleri', HESAPKOD='83006066001' WHERE YIL=2021 AND HESAPKOD='83006060601'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Taşıt Kiralaması Giderleri', HESAPKOD='83006066002' WHERE YIL=2021 AND HESAPKOD='83006060602'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='İş Makinası Kiralaması Giderleri', HESAPKOD='83006066003' WHERE YIL=2021 AND HESAPKOD='83006060603'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Canlı Hayvan Kiralaması Giderleri', HESAPKOD='83006066004' WHERE YIL=2021 AND HESAPKOD='83006060604'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Bina Kiralaması Giderleri', HESAPKOD='83006066005' WHERE YIL=2021 AND HESAPKOD='83006060605'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Arazi Kiralaması Giderleri', HESAPKOD='83006066006' WHERE YIL=2021 AND HESAPKOD='83006060606'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yüzer Taşıt Kiralaması Giderleri', HESAPKOD='83006066007' WHERE YIL=2021 AND HESAPKOD='83006060607'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Uçak Kiralaması Giderleri', HESAPKOD='83006066008' WHERE YIL=2021 AND HESAPKOD='83006060608'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Bilgisayar ve Bilgisayar Sistemleri Kiralaması Giderleri', HESAPKOD='83006066009' WHERE YIL=2021 AND HESAPKOD='83006060609'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Kiralamalar', HESAPKOD='83006066090' WHERE YIL=2021 AND HESAPKOD='83006060690'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Müteahhitlik Giderleri', HESAPKOD='830060670' WHERE YIL=2021 AND HESAPKOD='830060607'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Müteahhitlik Giderleri', HESAPKOD='83006067001' WHERE YIL=2021 AND HESAPKOD='83006060701'
DELETE FROM  MUHHESAPPLANI WHERE YIL=2021 AND HESAPKOD='830060608'
DELETE FROM  MUHHESAPPLANI WHERE YIL=2021 AND HESAPKOD='83006060801'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Menkul Malların Diğer Büyük Onarım Giderleri', HESAPKOD='830060690' WHERE YIL=2021 AND HESAPKOD='830060609'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Menkul Malların Diğer Büyük Onarım Giderleri', HESAPKOD='83006069001' WHERE YIL=2021 AND HESAPKOD='83006060901'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Gayrimenkul Büyük Onarım Giderleri', HESAPKOD='8300607' WHERE YIL=2021 AND HESAPKOD='8300607'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Müşavir Firma ve Kişilere Ödemeler', HESAPKOD='830060710' WHERE YIL=2021 AND HESAPKOD='830060701'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Proje Giderleri', HESAPKOD='83006071001' WHERE YIL=2021 AND HESAPKOD='83006070101'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Müşavirlik Giderleri', HESAPKOD='83006071002' WHERE YIL=2021 AND HESAPKOD='83006070102'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kontrol Giderleri', HESAPKOD='83006071003' WHERE YIL=2021 AND HESAPKOD='83006070103'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Müşavir Firma ve Kişilere Diğer Ödemeler', HESAPKOD='83006071090' WHERE YIL=2021 AND HESAPKOD='83006070190'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Malzeme Giderleri', HESAPKOD='830060720' WHERE YIL=2021 AND HESAPKOD='830060702'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='İnşaat Malzemesi Giderleri', HESAPKOD='83006072001' WHERE YIL=2021 AND HESAPKOD='83006070201'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Elektrik Tesisatı Giderleri', HESAPKOD='83006072002' WHERE YIL=2021 AND HESAPKOD='83006070202'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sıhhi Tesisat Giderleri', HESAPKOD='83006072003' WHERE YIL=2021 AND HESAPKOD='83006070203'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Özel Tesisat Giderleri', HESAPKOD='83006072004' WHERE YIL=2021 AND HESAPKOD='83006070204'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Malzeme Giderleri', HESAPKOD='83006072090' WHERE YIL=2021 AND HESAPKOD='83006070290'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Taşıma Giderleri', HESAPKOD='830060730' WHERE YIL=2021 AND HESAPKOD='830060703'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Taşımaya ilişkin Beslenme, Barındırma Giderleri', HESAPKOD='83006073001' WHERE YIL=2021 AND HESAPKOD='83006070301'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yolcu Taşıma Giderleri', HESAPKOD='83006073002' WHERE YIL=2021 AND HESAPKOD='83006070302'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yük Taşıma Giderleri', HESAPKOD='83006073003' WHERE YIL=2021 AND HESAPKOD='83006070303'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Taşıma Giderleri', HESAPKOD='83006073090' WHERE YIL=2021 AND HESAPKOD='83006070390'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Enerji Giderleri', HESAPKOD='830060740' WHERE YIL=2021 AND HESAPKOD='830060704'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yakacak  Alımları', HESAPKOD='83006074001' WHERE YIL=2021 AND HESAPKOD='83006070401'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Akaryakıt ve Yağ  Alımları', HESAPKOD='83006074002' WHERE YIL=2021 AND HESAPKOD='83006070402'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Elektrik Alımları', HESAPKOD='83006074003' WHERE YIL=2021 AND HESAPKOD='83006070403'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Enerji Alımları', HESAPKOD='83006074090' WHERE YIL=2021 AND HESAPKOD='83006070490'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Haberleşme Giderleri', HESAPKOD='830060750' WHERE YIL=2021 AND HESAPKOD='830060705'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Posta ve Telgraf Giderleri', HESAPKOD='83006075001' WHERE YIL=2021 AND HESAPKOD='83006070501'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Telefon Abonelik ve Kullanım Ücretleri', HESAPKOD='83006075002' WHERE YIL=2021 AND HESAPKOD='83006070502'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Bilgiye Abonelik ve İnternet Erişimi Giderleri', HESAPKOD='83006075003' WHERE YIL=2021 AND HESAPKOD='83006070503'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Haberleşme Cihazları Ruhsat ve Kullanım Giderleri', HESAPKOD='83006075004' WHERE YIL=2021 AND HESAPKOD='83006070504'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Uydu Haberleşme Giderleri', HESAPKOD='83006075005' WHERE YIL=2021 AND HESAPKOD='83006070505'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Hat Kira Giderleri', HESAPKOD='83006075006' WHERE YIL=2021 AND HESAPKOD='83006070506'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Haberleşme Giderleri', HESAPKOD='83006075090' WHERE YIL=2021 AND HESAPKOD='83006070590'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kira Giderleri', HESAPKOD='830060760' WHERE YIL=2021 AND HESAPKOD='830060706'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Dayanıklı Mal ve Malzeme Kiralaması Giderleri', HESAPKOD='83006076001' WHERE YIL=2021 AND HESAPKOD='83006070601'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Taşıt Kiralaması Giderleri', HESAPKOD='83006076002' WHERE YIL=2021 AND HESAPKOD='83006070602'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='İş Makinası Kiralaması Giderleri', HESAPKOD='83006076003' WHERE YIL=2021 AND HESAPKOD='83006070603'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Canlı Hayvan Kiralaması Giderleri', HESAPKOD='83006076004' WHERE YIL=2021 AND HESAPKOD='83006070604'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Bina Kiralaması Giderleri', HESAPKOD='83006076005' WHERE YIL=2021 AND HESAPKOD='83006070605'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Arazi Kiralaması Giderleri', HESAPKOD='83006076006' WHERE YIL=2021 AND HESAPKOD='83006070606'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yüzer Taşıt Kiralaması Giderleri', HESAPKOD='83006076007' WHERE YIL=2021 AND HESAPKOD='83006070607'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Hava Taşıtı Kiralaması Giderleri', HESAPKOD='83006076008' WHERE YIL=2021 AND HESAPKOD='83006070608'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Bilgisayar Sistemleri ve Yazılımları Kiralaması Giderleri', HESAPKOD='83006076009' WHERE YIL=2021 AND HESAPKOD='83006070609'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Tersane Kiralaması Giderleri', HESAPKOD='83006076010' WHERE YIL=2021 AND HESAPKOD='83006070610'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Kiralamalar', HESAPKOD='83006076090' WHERE YIL=2021 AND HESAPKOD='83006070690'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Müteahhitlik Giderleri', HESAPKOD='830060770' WHERE YIL=2021 AND HESAPKOD='830060707'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Hizmet Binası Büyük Onarım Giderleri', HESAPKOD='83006077001' WHERE YIL=2021 AND HESAPKOD='83006070701'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Hizmet Tesisi Büyük Onarım Giderleri', HESAPKOD='83006077002' WHERE YIL=2021 AND HESAPKOD='83006070702'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Lojman Büyük Onarım Giderleri', HESAPKOD='83006077003' WHERE YIL=2021 AND HESAPKOD='83006070703'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sosyal Tesis Büyük Onarım Giderleri', HESAPKOD='83006077004' WHERE YIL=2021 AND HESAPKOD='83006070704'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Gemi Büyük Onarım Giderleri', HESAPKOD='83006077005' WHERE YIL=2021 AND HESAPKOD='83006070705'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yüzer Tersane Büyük Onarım Giderleri', HESAPKOD='83006077006' WHERE YIL=2021 AND HESAPKOD='83006070706'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Müteahhitlik Giderleri', HESAPKOD='83006077090' WHERE YIL=2021 AND HESAPKOD='83006070790'
DELETE FROM  MUHHESAPPLANI WHERE YIL=2021 AND HESAPKOD='830060708'
DELETE FROM  MUHHESAPPLANI WHERE YIL=2021 AND HESAPKOD='83006070801'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Gayrimenkul Büyük Onarım Giderleri', HESAPKOD='830060790' WHERE YIL=2021 AND HESAPKOD='830060709'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Gayrimenkul Büyük Onarım Giderleri', HESAPKOD='83006079001' WHERE YIL=2021 AND HESAPKOD='83006070901'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Stok Alımları', HESAPKOD='8300608' WHERE YIL=2021 AND HESAPKOD='8300608'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Stratejik ve Acil Durum Malzemesi', HESAPKOD='830060810' WHERE YIL=2021 AND HESAPKOD='830060801'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Stratejik ve Acil Durum Malzemesi', HESAPKOD='83006081001' WHERE YIL=2021 AND HESAPKOD='83006080101'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Tarımsal Ürünler', HESAPKOD='830060820' WHERE YIL=2021 AND HESAPKOD='830060802'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Buğday Alımları', HESAPKOD='83006082001' WHERE YIL=2021 AND HESAPKOD='83006080201'
DELETE FROM  MUHHESAPPLANI WHERE YIL=2021 AND HESAPKOD='83006080203'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer', HESAPKOD='83006082090' WHERE YIL=2021 AND HESAPKOD='83006080290'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sanayi Ürünleri Alımları', HESAPKOD='830060830' WHERE YIL=2021 AND HESAPKOD='830060803'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Tekstil Ürünleri Alımları', HESAPKOD='83006083001' WHERE YIL=2021 AND HESAPKOD='83006080301'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Ürün Alımları', HESAPKOD='83006083090' WHERE YIL=2021 AND HESAPKOD='83006080390'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Hammadde Alımları', HESAPKOD='830060840' WHERE YIL=2021 AND HESAPKOD='830060804'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Hammadde Alımları', HESAPKOD='83006084001' WHERE YIL=2021 AND HESAPKOD='83006080401'
DELETE FROM  MUHHESAPPLANI WHERE YIL=2021 AND HESAPKOD='830060808'
DELETE FROM  MUHHESAPPLANI WHERE YIL=2021 AND HESAPKOD='83006080801'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Stok Alımları', HESAPKOD='830060890' WHERE YIL=2021 AND HESAPKOD='830060809'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Stok Alımları', HESAPKOD='83006089001' WHERE YIL=2021 AND HESAPKOD='83006080901'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Sermaye Giderleri', HESAPKOD='8300609' WHERE YIL=2021 AND HESAPKOD='8300609'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Gizli Hizmet Giderleri', HESAPKOD='830060910' WHERE YIL=2021 AND HESAPKOD='830060901'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Gizli Hizmet Giderleri', HESAPKOD='83006091001' WHERE YIL=2021 AND HESAPKOD='83006090101'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yolluk Giderleri', HESAPKOD='830060920' WHERE YIL=2021 AND HESAPKOD='830060902'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yurtiçi Geçici Görev Yollukları', HESAPKOD='83006092001' WHERE YIL=2021 AND HESAPKOD='83006090201'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yurtiçi Geçici Görev Yollukları', HESAPKOD='83006092001' WHERE YIL=2021 AND HESAPKOD='83006090202'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yurtdışı Geçici Görev Yollukları', HESAPKOD='83006092003' WHERE YIL=2021 AND HESAPKOD='83006090203'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yolluk Tazminatları', HESAPKOD='83006092004' WHERE YIL=2021 AND HESAPKOD='83006090205'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Arazi Tazminatları', HESAPKOD='83006092005' WHERE YIL=2021 AND HESAPKOD='83006090206'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Sermaye Giderleri', HESAPKOD='830060990' WHERE YIL=2021 AND HESAPKOD='830060909'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Sermaye Giderleri', HESAPKOD='83006099001' WHERE YIL=2021 AND HESAPKOD='83006090901'
DELETE FROM  MUHHESAPPLANI WHERE YIL=2021 AND HESAPKOD='83006090990'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sermaye Transferleri', HESAPKOD='83007' WHERE YIL=2021 AND HESAPKOD='83007'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kurum, İşletme ve Hanehalkına Yapılan Sermaye Transferleri', HESAPKOD='8300701' WHERE YIL=2021 AND HESAPKOD='8300701'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Genel Bütçeye Hazine Yardımları', HESAPKOD='830070310' WHERE YIL=2021 AND HESAPKOD='830070101'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Genel Bütçeye Hazine Yardımları', HESAPKOD='83007031001' WHERE YIL=2021 AND HESAPKOD='83007010101'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Özel Bütçeli İdarelere Hazine Yardımları', HESAPKOD='830070320' WHERE YIL=2021 AND HESAPKOD='830070102'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Ölçme, Seçme ve Yerleştirme Merkezi Başkanlığı na Hazine Yardımı', HESAPKOD='83007032001' WHERE YIL=2021 AND HESAPKOD='83007010201'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Atatürk Kültür, Dil ve Tarih Yüksek Kurumu na Hazine Yardımı', HESAPKOD='83007032002' WHERE YIL=2021 AND HESAPKOD='83007010202'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Atatürk Araştırma Merkezi ne Hazine Yardımı', HESAPKOD='83007032003' WHERE YIL=2021 AND HESAPKOD='83007010203'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Atatürk Kültür Merkezi ne Hazine Yardımı', HESAPKOD='83007032004' WHERE YIL=2021 AND HESAPKOD='83007010204'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Türk Dil Kurumu na Hazine Yardımı', HESAPKOD='83007032005' WHERE YIL=2021 AND HESAPKOD='83007010205'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Türk Tarih Kurumu na Hazine Yardımı', HESAPKOD='83007032006' WHERE YIL=2021 AND HESAPKOD='83007010206'
DELETE FROM  MUHHESAPPLANI WHERE YIL=2021 AND HESAPKOD='83007010207'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Türkiye Bilimsel ve Teknolojik Araştırma Kurumu Başkanlığı na Hazine Yardımı', HESAPKOD='83007032007' WHERE YIL=2021 AND HESAPKOD='83007010208'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Türkiye Bilimler Akademisi Başkanlığı na Hazine Yardımı', HESAPKOD='83007032008' WHERE YIL=2021 AND HESAPKOD='83007010209'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Türkiye Adalet Akademisi’ne Hazine Yardımı', HESAPKOD='83007032009' WHERE YIL=2021 AND HESAPKOD='83007010210'
DELETE FROM  MUHHESAPPLANI WHERE YIL=2021 AND HESAPKOD='83007010213'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sınıflandırmaya Girmeyen Diğer Hazine Yardımları', HESAPKOD='83007039090' WHERE YIL=2021 AND HESAPKOD='83007010214'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Devlet Tiyatroları Genel Müdürlüğü ne Hazine Yardımı', HESAPKOD='83007032010' WHERE YIL=2021 AND HESAPKOD='83007010215'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Devlet Opera ve Balesi Genel Müdürlüğü ne Hazine Yardımı', HESAPKOD='83007032011' WHERE YIL=2021 AND HESAPKOD='83007010216'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Orman Genel Müdürlüğü ne Hazine Yardımı', HESAPKOD='83007032012' WHERE YIL=2021 AND HESAPKOD='83007010217'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Vakıflar Genel Müdürlüğü ne Hazine Yardımı', HESAPKOD='83007032013' WHERE YIL=2021 AND HESAPKOD='83007010218'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Türkiye Hudut ve Sahiller Sağlık Genel Müdürlüğü ne Hazine Yardımı', HESAPKOD='83007032014' WHERE YIL=2021 AND HESAPKOD='83007010219'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Türk Akreditasyon Kurumu na Hazine Yardımı', HESAPKOD='83007032015' WHERE YIL=2021 AND HESAPKOD='83007010221'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Türk Standartları Enstitüsü ne Hazine Yardımı', HESAPKOD='83007032016' WHERE YIL=2021 AND HESAPKOD='83007010222'
DELETE FROM  MUHHESAPPLANI WHERE YIL=2021 AND HESAPKOD='83007010223'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Türk Patent Enstitüsü ne Hazine Yardımı', HESAPKOD='83007032017' WHERE YIL=2021 AND HESAPKOD='83007010224'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Ulusal Bor Araştırma Enstitüsü ne Hazine Yardımı', HESAPKOD='83007032018' WHERE YIL=2021 AND HESAPKOD='83007010226'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Türkiye Atom Enerjisi Kurumu na Hazine Yardımı', HESAPKOD='83007032019' WHERE YIL=2021 AND HESAPKOD='83007010227'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Kamu Kurum ve Kuruluşlarına', HESAPKOD='83007019003' WHERE YIL=2021 AND HESAPKOD='83007010228'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Küçük ve Orta Ölçekli İşletmeleri Geliştirme ve Destekleme İdaresi Başkanlığı na Hazine Yardımı', HESAPKOD='83007032021' WHERE YIL=2021 AND HESAPKOD='83007010230'
DELETE FROM  MUHHESAPPLANI WHERE YIL=2021 AND HESAPKOD='83007010231'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Türk İşbirliği ve Koordinasyon Ajansı Başkanlığı na Hazine Yardımı', HESAPKOD='83007032022' WHERE YIL=2021 AND HESAPKOD='83007010232'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='GAP Bölge Kalkınma İdaresi Başkanlığı na Hazine Yardımı', HESAPKOD='83007032023' WHERE YIL=2021 AND HESAPKOD='83007010234'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Özelleştirme İdaresi Başkanlığı na Hazine Yardımı', HESAPKOD='83007032024' WHERE YIL=2021 AND HESAPKOD='83007010235'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Maden Tetkik ve Arama Genel Müdürlüğü ne Hazine Yardımı', HESAPKOD='83007032025' WHERE YIL=2021 AND HESAPKOD='83007010240'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Ceza İnfaz Kurumları ile Tutukevleri İş Yurtları Kurumu na  Hazine Yardımı', HESAPKOD='83007032026' WHERE YIL=2021 AND HESAPKOD='83007010241'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sivil Havacılık Genel Müdürlüğü ne Hazine Yardımı', HESAPKOD='83007032027' WHERE YIL=2021 AND HESAPKOD='83007010249'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Mesleki Yeterlilik Kurumu na Hazine Yardımı', HESAPKOD='83007032028' WHERE YIL=2021 AND HESAPKOD='83007010250'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yurtdışı Türkler ve Akraba Topluluklar Başkanlığına Hazine Yardımı', HESAPKOD='83007032029' WHERE YIL=2021 AND HESAPKOD='83007010251'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Karayolları Genel Müdürlüğü ne Hazine Yardımı', HESAPKOD='83007032030' WHERE YIL=2021 AND HESAPKOD='83007010252'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Türkiye Yazma Eserler Kurumu Başkanlığı na Hazine Yardımı', HESAPKOD='83007032031' WHERE YIL=2021 AND HESAPKOD='83007010253'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Doğu Anadolu Projesi Bölge Kalkınma İdaresi Başkanlığı na Hazine Yardımı', HESAPKOD='83007032032' WHERE YIL=2021 AND HESAPKOD='83007010254'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Konya Ovası Projesi Bölge Kalkınma İdaresi Başkanlığı na Hazine Yardımı', HESAPKOD='83007032033' WHERE YIL=2021 AND HESAPKOD='83007010255'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Doğu Karadeniz Projesi Bölge Kalkınma İdaresi Başkanlığı na Hazine Yardımı', HESAPKOD='83007032034' WHERE YIL=2021 AND HESAPKOD='83007010256'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Devlet Su İşleri Genel Müdürlüğü ne Hazine Yardımı', HESAPKOD='83007032035' WHERE YIL=2021 AND HESAPKOD='83007010257'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Türkiye Su Enstitüsü ne Hazine Yardımı', HESAPKOD='83007032036' WHERE YIL=2021 AND HESAPKOD='83007010258'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Türkiye İlaç ve Tıbbi Cihaz Kurumu na Hazine Yardımı', HESAPKOD='83007032037' WHERE YIL=2021 AND HESAPKOD='83007010259'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kamu Denetçiliği Kurumu na Hazine Yardımı', HESAPKOD='83007032038' WHERE YIL=2021 AND HESAPKOD='83007010260'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Türkiye İnsan Hakları ve Eşitlik Kurumu na Hazine Yardımı', HESAPKOD='83007032039' WHERE YIL=2021 AND HESAPKOD='83007010261'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Türkiye Sağlık Enstitüleri Başkanlığı na Hazine Yardımı', HESAPKOD='83007032040' WHERE YIL=2021 AND HESAPKOD='83007010262'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Helal Akreditasyon Kurumu na Hazine Yardımı', HESAPKOD='83007032041' WHERE YIL=2021 AND HESAPKOD='83007010263'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Nadir Toprak Elementleri Araştırma Enstitüsü ne Hazine Yardımı', HESAPKOD='83007032042' WHERE YIL=2021 AND HESAPKOD='83007010264'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Maden ve Petrol İşleri Genel Müdürlüğü ne Hazine Yardımı', HESAPKOD='83007032043' WHERE YIL=2021 AND HESAPKOD='83007010265'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Türkiye Uzay Ajansı na Hazine Yardımı', HESAPKOD='83007032044' WHERE YIL=2021 AND HESAPKOD='83007010266'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Düzenleyici ve Denetleyici Kurumlara Hazine Yardımları', HESAPKOD='830070330' WHERE YIL=2021 AND HESAPKOD='830070103'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Radyo ve Televizyon Üst Kurulu na Hazine Yardımı', HESAPKOD='83007033001' WHERE YIL=2021 AND HESAPKOD='83007010301'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Bilgi Teknolojileri ve İletişim Kurumu na Hazine Yardımı', HESAPKOD='83007033002' WHERE YIL=2021 AND HESAPKOD='83007010302'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sermaye Piyasası Kurulu na Hazine Yardımı', HESAPKOD='83007033003' WHERE YIL=2021 AND HESAPKOD='83007010303'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Bankacılık Düzenleme ve Denetleme Kurumu na Hazine Yardımı', HESAPKOD='83007033004' WHERE YIL=2021 AND HESAPKOD='83007010304'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Enerji Piyasası Düzenleme Kurumu na Hazine Yardımı', HESAPKOD='83007033005' WHERE YIL=2021 AND HESAPKOD='83007010305'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kamu İhale Kurumu na Hazine Yardımı', HESAPKOD='83007033006' WHERE YIL=2021 AND HESAPKOD='83007010306'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Rekabet Kurumu na Hazine Yardımı', HESAPKOD='83007033007' WHERE YIL=2021 AND HESAPKOD='83007010307'
DELETE FROM  MUHHESAPPLANI WHERE YIL=2021 AND HESAPKOD='83007010309'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kamu Gözetimi, Muhasebe ve Denetim Standartları Kurumu na Hazine Yardımı', HESAPKOD='83007033008' WHERE YIL=2021 AND HESAPKOD='83007010310'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kişisel Verileri Koruma Kurumu na Hazine Yardımı', HESAPKOD='83007033009' WHERE YIL=2021 AND HESAPKOD='83007010311'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Nükleer Düzenleme Kurumu na Hazine Yardımı', HESAPKOD='83007033010' WHERE YIL=2021 AND HESAPKOD='83007010312'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sosyal Güvenlik Kurumlarına Hazine Yardımları', HESAPKOD='830070340' WHERE YIL=2021 AND HESAPKOD='830070104'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Türkiye İş Kurumu Genel Müdürlüğü ne', HESAPKOD='83007034001' WHERE YIL=2021 AND HESAPKOD='83007010404'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sosyal Güvenlik Kurumu na', HESAPKOD='83007034002' WHERE YIL=2021 AND HESAPKOD='83007010406'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Mahalli İdarelere Hazine Yardımları', HESAPKOD='830070350' WHERE YIL=2021 AND HESAPKOD='830070105'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='İl Özel İdarelerine', HESAPKOD='83007035001' WHERE YIL=2021 AND HESAPKOD='83007010501'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Belediyelere', HESAPKOD='83007035002' WHERE YIL=2021 AND HESAPKOD='83007010502'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Köylere', HESAPKOD='83007035003' WHERE YIL=2021 AND HESAPKOD='83007010503'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Mahalli İdare Birliklerine', HESAPKOD='83007035004' WHERE YIL=2021 AND HESAPKOD='83007010504'
DELETE FROM  MUHHESAPPLANI WHERE YIL=2021 AND HESAPKOD='83007010512'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kamu Teşebbüslerine Sermaye Transferleri', HESAPKOD='830070110' WHERE YIL=2021 AND HESAPKOD='830070106'
DELETE FROM  MUHHESAPPLANI WHERE YIL=2021 AND HESAPKOD='83007010601'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='TC Devlet Demiryolları na', HESAPKOD='83007011001' WHERE YIL=2021 AND HESAPKOD='83007010602'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Tarım İşletmelerine', HESAPKOD='83007011002' WHERE YIL=2021 AND HESAPKOD='83007010603'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Toprak Mahsulleri Ofisi ne', HESAPKOD='83007011003' WHERE YIL=2021 AND HESAPKOD='83007010604'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Teşebbüslere', HESAPKOD='83007011090' WHERE YIL=2021 AND HESAPKOD='83007010605'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Çay İşletmeleri Genel Müdürlüğü ne', HESAPKOD='83007011004' WHERE YIL=2021 AND HESAPKOD='83007010606'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Türkiye Elektrik Dağıtım AŞ ye', HESAPKOD='83007011005' WHERE YIL=2021 AND HESAPKOD='83007010607'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Elektrik Üretim AŞ ye', HESAPKOD='83007011006' WHERE YIL=2021 AND HESAPKOD='83007010608'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Elektrik İletim AŞ ye', HESAPKOD='83007011007' WHERE YIL=2021 AND HESAPKOD='83007010609'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Türkiye Elektrik ve Taahhüt AŞ ye', HESAPKOD='83007011008' WHERE YIL=2021 AND HESAPKOD='83007010610'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Türkiye Taşkömürü Kurumu na', HESAPKOD='83007011009' WHERE YIL=2021 AND HESAPKOD='83007010611'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Üretici Birliklerine', HESAPKOD='83007011010' WHERE YIL=2021 AND HESAPKOD='83007010612'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Tarım Kredi Kooperatiflerine', HESAPKOD='83007011011' WHERE YIL=2021 AND HESAPKOD='83007010613'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Türkiye Şeker Fabrikaları AŞ ye', HESAPKOD='83007011012' WHERE YIL=2021 AND HESAPKOD='83007010614'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sümer Holding AŞ ye', HESAPKOD='83007011013' WHERE YIL=2021 AND HESAPKOD='83007010615'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Makina ve Kimya Endüstrisi Kurumu na', HESAPKOD='83007011014' WHERE YIL=2021 AND HESAPKOD='83007010616'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Teşebbüslere', HESAPKOD='83007011090' WHERE YIL=2021 AND HESAPKOD='83007010617'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Et ve Süt Kurumu Genel Müdürlüğü ne', HESAPKOD='83007011015' WHERE YIL=2021 AND HESAPKOD='83007010619'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Türkiye Petrolleri Anonim Ortaklığı’na', HESAPKOD='83007011016' WHERE YIL=2021 AND HESAPKOD='83007010623'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Teşebbüslere', HESAPKOD='83007011090' WHERE YIL=2021 AND HESAPKOD='83007010649'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Darphane ve Damga Matbaası Genel Müdürlüğü ne', HESAPKOD='83007012001' WHERE YIL=2021 AND HESAPKOD='83007010650'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Döner Sermaye İşletmelerine', HESAPKOD='83007012090' WHERE YIL=2021 AND HESAPKOD='83007010659'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Ziraat Bankasına', HESAPKOD='83007013001' WHERE YIL=2021 AND HESAPKOD='83007010670'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Türkiye Kalkınma ve Yatırım Bankasına', HESAPKOD='83007013002' WHERE YIL=2021 AND HESAPKOD='83007010671'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Türk Eximbanka', HESAPKOD='83007013003' WHERE YIL=2021 AND HESAPKOD='83007010672'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Mali Kurumlara', HESAPKOD='83007013090' WHERE YIL=2021 AND HESAPKOD='83007010679'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Ulusal Fona', HESAPKOD='83007014080' WHERE YIL=2021 AND HESAPKOD='83007010680'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Fonlara', HESAPKOD='83007014090' WHERE YIL=2021 AND HESAPKOD='83007010689'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yükseköğretim Kurumlarına Hazine Yardımları', HESAPKOD='830070370' WHERE YIL=2021 AND HESAPKOD='830070107'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yükseköğretim Kurulu na Hazine Yardımı', HESAPKOD='83007037001' WHERE YIL=2021 AND HESAPKOD='83007010701'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Ankara Üniversitesi ne Hazine Yardımı', HESAPKOD='83007037002' WHERE YIL=2021 AND HESAPKOD='83007010702'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Ortadoğu Teknik Üniversitesi ne Hazine Yardımı', HESAPKOD='83007037003' WHERE YIL=2021 AND HESAPKOD='83007010703'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Hacettepe Üniversitesi ne Hazine Yardımı', HESAPKOD='83007037004' WHERE YIL=2021 AND HESAPKOD='83007010704'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Gazi Üniversitesi ne Hazine Yardımı', HESAPKOD='83007037005' WHERE YIL=2021 AND HESAPKOD='83007010705'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='İstanbul Üniversitesi ne Hazine Yardımı', HESAPKOD='83007037006' WHERE YIL=2021 AND HESAPKOD='83007010706'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='İstanbul Teknik Üniversitesi ne Hazine Yardımı', HESAPKOD='83007037007' WHERE YIL=2021 AND HESAPKOD='83007010707'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Boğaziçi Üniversitesi ne Hazine Yardımı', HESAPKOD='83007037008' WHERE YIL=2021 AND HESAPKOD='83007010708'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Marmara Üniversitesi ne Hazine Yardımı', HESAPKOD='83007037009' WHERE YIL=2021 AND HESAPKOD='83007010709'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yıldız Teknik Üniversitesi ne Hazine Yardımı', HESAPKOD='83007037010' WHERE YIL=2021 AND HESAPKOD='83007010710'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Mimar Sinan Güzel Sanatlar Üniversitesi ne Hazine Yardımı', HESAPKOD='83007037011' WHERE YIL=2021 AND HESAPKOD='83007010711'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Ege Üniversitesi ne Hazine Yardımı', HESAPKOD='83007037012' WHERE YIL=2021 AND HESAPKOD='83007010712'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Dokuz Eylül Üniversitesi ne Hazine Yardımı', HESAPKOD='83007037013' WHERE YIL=2021 AND HESAPKOD='83007010713'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Trakya Üniversitesi ne Hazine Yardımı', HESAPKOD='83007037014' WHERE YIL=2021 AND HESAPKOD='83007010714'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Bursa Uludağ Üniversitesi ne Hazine Yardımı', HESAPKOD='83007037015' WHERE YIL=2021 AND HESAPKOD='83007010715'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Anadolu Üniversitesi ne Hazine Yardımı', HESAPKOD='83007037016' WHERE YIL=2021 AND HESAPKOD='83007010716'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Selçuk Üniversitesi ne Hazine Yardımı', HESAPKOD='83007037017' WHERE YIL=2021 AND HESAPKOD='83007010717'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Akdeniz Üniversitesi ne Hazine Yardımı', HESAPKOD='83007037018' WHERE YIL=2021 AND HESAPKOD='83007010718'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Erciyes Üniversitesi ne Hazine Yardımı', HESAPKOD='83007037019' WHERE YIL=2021 AND HESAPKOD='83007010719'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sivas Cumhuriyet Üniversitesi ne Hazine Yardımı', HESAPKOD='83007037020' WHERE YIL=2021 AND HESAPKOD='83007010720'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Çukurova Üniversitesi ne Hazine Yardımı', HESAPKOD='83007037021' WHERE YIL=2021 AND HESAPKOD='83007010721'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Ondokuz Mayıs Üniversitesi ne Hazine Yardımı', HESAPKOD='83007037022' WHERE YIL=2021 AND HESAPKOD='83007010722'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Karadeniz Teknik Üniversitesi ne Hazine Yardımı', HESAPKOD='83007037023' WHERE YIL=2021 AND HESAPKOD='83007010723'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Atatürk Üniversitesi ne Hazine Yardımı', HESAPKOD='83007037024' WHERE YIL=2021 AND HESAPKOD='83007010724'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='İnönü Üniversitesi ne Hazine Yardımı', HESAPKOD='83007037025' WHERE YIL=2021 AND HESAPKOD='83007010725'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Fırat Üniversitesi ne Hazine Yardımı', HESAPKOD='83007037026' WHERE YIL=2021 AND HESAPKOD='83007010726'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Dicle Üniversitesi ne Hazine Yardımı', HESAPKOD='83007037027' WHERE YIL=2021 AND HESAPKOD='83007010727'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yüzüncüyıl Üniversitesi ne Hazine Yardımı', HESAPKOD='83007037028' WHERE YIL=2021 AND HESAPKOD='83007010728'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Gaziantep Üniversitesi ne Hazine Yardımı', HESAPKOD='83007037029' WHERE YIL=2021 AND HESAPKOD='83007010729'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='İzmir Yüksek Teknoloji Enstitüsü ne Hazine Yardımı', HESAPKOD='83007037030' WHERE YIL=2021 AND HESAPKOD='83007010730'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Gebze Teknik Üniversitesi ne Hazine Yardımı', HESAPKOD='83007037031' WHERE YIL=2021 AND HESAPKOD='83007010731'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Harran Üniversitesi ne Hazine Yardımı', HESAPKOD='83007037032' WHERE YIL=2021 AND HESAPKOD='83007010732'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Süleyman Demirel Üniversitesi ne Hazine Yardımı', HESAPKOD='83007037033' WHERE YIL=2021 AND HESAPKOD='83007010733'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Aydın Adnan Menderes Üniversitesi ne Hazine Yardımı', HESAPKOD='83007037034' WHERE YIL=2021 AND HESAPKOD='83007010734'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Zonguldak Bülent Ecevit Üniversitesi ne Hazine Yardımı', HESAPKOD='83007037035' WHERE YIL=2021 AND HESAPKOD='83007010735'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Mersin Üniversitesi ne Hazine Yardımı', HESAPKOD='83007037036' WHERE YIL=2021 AND HESAPKOD='83007010736'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Pamukkale Üniversitesi ne Hazine Yardımı', HESAPKOD='83007037037' WHERE YIL=2021 AND HESAPKOD='83007010737'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Balıkesir Üniversitesi ne Hazine Yardımı', HESAPKOD='83007037038' WHERE YIL=2021 AND HESAPKOD='83007010738'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kocaeli Üniversitesi ne Hazine Yardımı', HESAPKOD='83007037039' WHERE YIL=2021 AND HESAPKOD='83007010739'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sakarya Üniversitesi ne Hazine Yardımı', HESAPKOD='83007037040' WHERE YIL=2021 AND HESAPKOD='83007010740'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Manisa Celal Bayar Üniversitesi’ne Hazine Yardımı', HESAPKOD='83007037041' WHERE YIL=2021 AND HESAPKOD='83007010741'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Bolu Abant İzzet Baysal Üniversitesi ne Hazine Yardımı', HESAPKOD='83007037042' WHERE YIL=2021 AND HESAPKOD='83007010742'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Hatay Mustafa Kemal Üniversitesi ne Hazine Yardımı', HESAPKOD='83007037043' WHERE YIL=2021 AND HESAPKOD='83007010743'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Afyon Kocatepe Üniversitesi ne Hazine Yardımı', HESAPKOD='83007037044' WHERE YIL=2021 AND HESAPKOD='83007010744'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kafkas Üniversitesi ne Hazine Yardımı', HESAPKOD='83007037045' WHERE YIL=2021 AND HESAPKOD='83007010745'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Çanakkale 18 Mart Üniversitesi ne Hazine Yardımı', HESAPKOD='83007037046' WHERE YIL=2021 AND HESAPKOD='83007010746'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Niğde Ömer Halisdemir Üniversitesi’ne Hazine Yardımı', HESAPKOD='83007037047' WHERE YIL=2021 AND HESAPKOD='83007010747'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kütahya Dumlupınar Üniversitesi ne Hazine Yardımı', HESAPKOD='83007037048' WHERE YIL=2021 AND HESAPKOD='83007010748'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Tokat Gaziosmanpaşa Üniversitesi ne Hazine Yardımı', HESAPKOD='83007037049' WHERE YIL=2021 AND HESAPKOD='83007010749'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Muğla Sıtkı Koçman Üniversitesi ne Hazine Yardımı', HESAPKOD='83007037050' WHERE YIL=2021 AND HESAPKOD='83007010750'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kahramanmaraş Sütçü İmam Üniversitesi ne Hazine Yardımı', HESAPKOD='83007037051' WHERE YIL=2021 AND HESAPKOD='83007010751'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kırıkkale Üniversitesi ne Hazine Yardımı', HESAPKOD='83007037052' WHERE YIL=2021 AND HESAPKOD='83007010752'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Eskişehir Osmangazi Üniversitesi ne Hazine Yardımı', HESAPKOD='83007037053' WHERE YIL=2021 AND HESAPKOD='83007010753'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Galatasaray Üniversitesi ne Hazine Yardımı', HESAPKOD='83007037054' WHERE YIL=2021 AND HESAPKOD='83007010754'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kırşehir Ahi Evran Üniversitesi ne Hazine Yardımı', HESAPKOD='83007037055' WHERE YIL=2021 AND HESAPKOD='83007010755'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kastamonu Üniversitesi ne Hazine Yardımı', HESAPKOD='83007037056' WHERE YIL=2021 AND HESAPKOD='83007010756'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Düzce Üniversitesi ne Hazine Yardımı', HESAPKOD='83007037057' WHERE YIL=2021 AND HESAPKOD='83007010757'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Burdur Mehmet Akif Ersoy Üniversitesi ne Hazine Yardımı', HESAPKOD='83007037058' WHERE YIL=2021 AND HESAPKOD='83007010758'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Uşak Üniversitesi ne Hazine Yardımı', HESAPKOD='83007037059' WHERE YIL=2021 AND HESAPKOD='83007010759'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Recep Tayyip Erdoğan Üniversitesi ne Hazine Yardımı', HESAPKOD='83007037060' WHERE YIL=2021 AND HESAPKOD='83007010760'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Tekirdağ Namık Kemal Üniversitesi ne Hazine Yardımı', HESAPKOD='83007037061' WHERE YIL=2021 AND HESAPKOD='83007010761'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Erzincan Binali Yıldırım Üniversitesi ne Hazine Yardımı', HESAPKOD='83007037062' WHERE YIL=2021 AND HESAPKOD='83007010762'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Aksaray Üniversitesi ne Hazine Yardımı', HESAPKOD='83007037063' WHERE YIL=2021 AND HESAPKOD='83007010763'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Giresun Üniversitesi ne Hazine Yardımı', HESAPKOD='83007037064' WHERE YIL=2021 AND HESAPKOD='83007010764'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Hitit Üniversitesi ne Hazine Yardımı', HESAPKOD='83007037065' WHERE YIL=2021 AND HESAPKOD='83007010765'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yozgat Bozok Üniversitesi ne Hazine Yardımı', HESAPKOD='83007037066' WHERE YIL=2021 AND HESAPKOD='83007010766'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Adıyaman Üniversitesi ne Hazine Yardımı', HESAPKOD='83007037067' WHERE YIL=2021 AND HESAPKOD='83007010767'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Ordu Üniversitesi ne Hazine Yardımı', HESAPKOD='83007037068' WHERE YIL=2021 AND HESAPKOD='83007010768'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Amasya Üniversitesi ne Hazine Yardımı', HESAPKOD='83007037069' WHERE YIL=2021 AND HESAPKOD='83007010769'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Karamanoğlu Mehmetbey Üniversitesi ne Hazine Yardımı', HESAPKOD='83007037070' WHERE YIL=2021 AND HESAPKOD='83007010770'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Ağrı İbrahim Çeçen Üniversitesi ne Hazine Yardımı', HESAPKOD='83007037071' WHERE YIL=2021 AND HESAPKOD='83007010771'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sinop Üniversitesi ne Hazine Yardımı', HESAPKOD='83007037072' WHERE YIL=2021 AND HESAPKOD='83007010772'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Siirt Üniversitesi ne Hazine Yardımı', HESAPKOD='83007037073' WHERE YIL=2021 AND HESAPKOD='83007010773'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Nevşehir Hacı Bektaş Veli Üniversitesi ne Hazine Yardımı', HESAPKOD='83007037074' WHERE YIL=2021 AND HESAPKOD='83007010774'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Karabük Üniversitesi ne Hazine Yardımı', HESAPKOD='83007037075' WHERE YIL=2021 AND HESAPKOD='83007010775'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kilis 7 Aralık Üniversitesi ne Hazine Yardımı', HESAPKOD='83007037076' WHERE YIL=2021 AND HESAPKOD='83007010776'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Çankırı Karatekin Üniversitesi ne Hazine Yardımı', HESAPKOD='83007037077' WHERE YIL=2021 AND HESAPKOD='83007010777'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Artvin Çoruh Üniversitesi ne Hazine Yardımı', HESAPKOD='83007037078' WHERE YIL=2021 AND HESAPKOD='83007010778'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Bilecik Şeyh Edebali Üniversitesi ne Hazine Yardımı', HESAPKOD='83007037079' WHERE YIL=2021 AND HESAPKOD='83007010779'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Bitlis Eren Üniversitesi ne Hazine Yardımı', HESAPKOD='83007037080' WHERE YIL=2021 AND HESAPKOD='83007010780'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kırklareli Üniversitesi ne Hazine Yardımı', HESAPKOD='83007037081' WHERE YIL=2021 AND HESAPKOD='83007010781'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Osmaniye Korkut Ata Üniversitesi ne Hazine Yardımı', HESAPKOD='83007037082' WHERE YIL=2021 AND HESAPKOD='83007010782'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Bingöl Üniversitesi ne Hazine Yardımı', HESAPKOD='83007037083' WHERE YIL=2021 AND HESAPKOD='83007010783'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Muş Alparslan Üniversitesi ne Hazine Yardımı', HESAPKOD='83007037084' WHERE YIL=2021 AND HESAPKOD='83007010784'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Mardin Artuklu Üniversitesi ne Hazine Yardımı', HESAPKOD='83007037085' WHERE YIL=2021 AND HESAPKOD='83007010785'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Batman Üniversitesi ne Hazine Yardımı', HESAPKOD='83007037086' WHERE YIL=2021 AND HESAPKOD='83007010786'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Ardahan Üniversitesi ne Hazine Yardımı', HESAPKOD='83007037087' WHERE YIL=2021 AND HESAPKOD='83007010787'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Bartın Üniversitesi ne Hazine Yardımı', HESAPKOD='83007037088' WHERE YIL=2021 AND HESAPKOD='83007010788'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Bayburt Üniversitesi ne Hazine Yardımı', HESAPKOD='83007037089' WHERE YIL=2021 AND HESAPKOD='83007010789'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Gümüşhane Üniversitesi ne Hazine Yardımı', HESAPKOD='83007037090' WHERE YIL=2021 AND HESAPKOD='83007010790'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Hakkari Üniversitesi ne Hazine Yardımı', HESAPKOD='83007037091' WHERE YIL=2021 AND HESAPKOD='83007010791'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Iğdır Üniversitesi ne Hazine Yardımı', HESAPKOD='83007037092' WHERE YIL=2021 AND HESAPKOD='83007010792'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Şırnak Üniversitesi ne Hazine Yardımı', HESAPKOD='83007037093' WHERE YIL=2021 AND HESAPKOD='83007010793'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Munzur Üniversitesi ne Hazine Yardımı', HESAPKOD='83007037094' WHERE YIL=2021 AND HESAPKOD='83007010794'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yalova Üniversitesi ne Hazine Yardımı', HESAPKOD='83007037095' WHERE YIL=2021 AND HESAPKOD='83007010795'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Türk-Alman Üniversitesi ne Hazine Yardımı', HESAPKOD='83007037096' WHERE YIL=2021 AND HESAPKOD='83007010796'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Ankara Yıldırım Beyazıt Üniversitesi ne Hazine Yardımı', HESAPKOD='83007037097' WHERE YIL=2021 AND HESAPKOD='83007010797'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Bursa Teknik Üniversitesi ne Hazine Yardımı', HESAPKOD='83007037098' WHERE YIL=2021 AND HESAPKOD='83007010798'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='İstanbul Medeniyet Üniversitesi ne Hazine Yardımı', HESAPKOD='83007037099' WHERE YIL=2021 AND HESAPKOD='83007010799'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yükseköğretim Kurumlarına Hazine Yardımları', HESAPKOD='830070380' WHERE YIL=2021 AND HESAPKOD='830070108'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='İzmir Katip Çelebi Üniversitesi ne Hazine Yardımı', HESAPKOD='83007038001' WHERE YIL=2021 AND HESAPKOD='83007010801'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Necmettin Erbakan Üniversitesi ne Hazine Yardımı', HESAPKOD='83007038002' WHERE YIL=2021 AND HESAPKOD='83007010802'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Abdullah Gül Üniversitesi ne Hazine Yardımı', HESAPKOD='83007038003' WHERE YIL=2021 AND HESAPKOD='83007010803'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Erzurum Teknik Üniversitesi ne Hazine Yardımı', HESAPKOD='83007038004' WHERE YIL=2021 AND HESAPKOD='83007010804'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Adana Alparslan Türkeş Bilim ve Teknoloji Üniversitesi ne Hazine Yardımı', HESAPKOD='83007038005' WHERE YIL=2021 AND HESAPKOD='83007010805'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Ankara Sosyal Bilimler Üniversitesi ne Hazine Yardımı', HESAPKOD='83007038006' WHERE YIL=2021 AND HESAPKOD='83007010806'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sağlık Bilimleri Üniversitesi ne Hazine Yardımı', HESAPKOD='83007038007' WHERE YIL=2021 AND HESAPKOD='83007010807'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Bandırma Onyedi Eylül Üniversitesi ne Hazine Yardımı', HESAPKOD='83007038008' WHERE YIL=2021 AND HESAPKOD='83007010808'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='İskenderun Teknik Üniversitesi ne Hazine Yardımı', HESAPKOD='83007038009' WHERE YIL=2021 AND HESAPKOD='83007010809'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Alanya Alaaddin Keykubat Üniversitesi ne Hazine Yardımı', HESAPKOD='83007038010' WHERE YIL=2021 AND HESAPKOD='83007010810'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Türkiye Uluslararası İslam, Bilim ve Teknoloji Üniversitesi ne Hazine Yardımı', HESAPKOD='83007038011' WHERE YIL=2021 AND HESAPKOD='83007010811'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='İzmir Bakırçay Üniversitesi ne Hazine Yardımı', HESAPKOD='83007038012' WHERE YIL=2021 AND HESAPKOD='83007010812'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='İzmir Demokrasi Üniversitesi ne Hazine Yardımı', HESAPKOD='83007038013' WHERE YIL=2021 AND HESAPKOD='83007010813'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yükseköğretim Kalite Kurulu na Hazine Yardımı', HESAPKOD='83007038014' WHERE YIL=2021 AND HESAPKOD='83007010814'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Ankara Müzik ve Güzel Sanatlar Üniversitesi ne Hazine Yardımı', HESAPKOD='83007038015' WHERE YIL=2021 AND HESAPKOD='83007010815'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Gaziantep İslam Bilim ve Teknoloji Üniversitesi ne Hazine Yardımı', HESAPKOD='83007038016' WHERE YIL=2021 AND HESAPKOD='83007010816'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Konya Teknik Üniversitesi ne Hazine Yardımı', HESAPKOD='83007038017' WHERE YIL=2021 AND HESAPKOD='83007010817'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kütahya Sağlık Bilimleri Üniversitesi ne Hazine Yardımı', HESAPKOD='83007038018' WHERE YIL=2021 AND HESAPKOD='83007010818'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Malatya Turgut Özal Üniversitesi ne Hazine Yardımı', HESAPKOD='83007038019' WHERE YIL=2021 AND HESAPKOD='83007010819'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='İstanbul Üniversitesi-Cerrahpaşa ya Hazine Yardımı', HESAPKOD='83007038020' WHERE YIL=2021 AND HESAPKOD='83007010820'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Ankara Hacı Bayram Veli Üniversitesi ne Hazine Yardımı', HESAPKOD='83007038021' WHERE YIL=2021 AND HESAPKOD='83007010821'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sakarya Uygulamalı Bilimler Üniversitesi ne Hazine Yardımı', HESAPKOD='83007038022' WHERE YIL=2021 AND HESAPKOD='83007010822'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Samsun Üniversitesi ne Hazine Yardımı', HESAPKOD='83007038023' WHERE YIL=2021 AND HESAPKOD='83007010823'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sivas Bilim ve Teknoloji Üniversitesi ne Hazine Yardımı', HESAPKOD='83007038024' WHERE YIL=2021 AND HESAPKOD='83007010824'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Tarsus Üniversitesi ne Hazine Yardımı', HESAPKOD='83007038025' WHERE YIL=2021 AND HESAPKOD='83007010825'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Trabzon Üniversitesi ne Hazine Yardımı', HESAPKOD='83007038026' WHERE YIL=2021 AND HESAPKOD='83007010826'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kayseri Üniversitesi ne Hazine Yardımı', HESAPKOD='83007038027' WHERE YIL=2021 AND HESAPKOD='83007010827'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kahramanmaraş İstiklal Üniversitesi ne Hazine Yardımı', HESAPKOD='83007038028' WHERE YIL=2021 AND HESAPKOD='83007010828'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Eskişehir Teknik Üniversitesi ne Hazine Yardımı', HESAPKOD='83007038029' WHERE YIL=2021 AND HESAPKOD='83007010829'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Isparta Uygulamalı Bilimler Üniversitesi ne Hazine Yardımı', HESAPKOD='83007038030' WHERE YIL=2021 AND HESAPKOD='83007010830'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Afyonkarahisar Sağlık Bilimleri Üniversitesi ne Hazine Yardımı', HESAPKOD='83007038031' WHERE YIL=2021 AND HESAPKOD='83007010831'
DELETE FROM  MUHHESAPPLANI WHERE YIL=2021 AND HESAPKOD='83007010890'
DELETE FROM  MUHHESAPPLANI WHERE YIL=2021 AND HESAPKOD='83007010899'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Kurum, İşletme ve Hane Halkına Yapılan Sermaye Transferleri', HESAPKOD='830070190' WHERE YIL=2021 AND HESAPKOD='830070109'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Dernek, Birlik, Kurum, Kuruluş, Sandık vb Kuruluşlara', HESAPKOD='83007019001' WHERE YIL=2021 AND HESAPKOD='83007010901'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Vakıf Üniversitelerine', HESAPKOD='83007019002' WHERE YIL=2021 AND HESAPKOD='83007010902'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Kamu Kurum ve Kuruluşlarına', HESAPKOD='83007019003' WHERE YIL=2021 AND HESAPKOD='83007010903'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kamu Ortaklıklarına', HESAPKOD='83007019004' WHERE YIL=2021 AND HESAPKOD='83007010904'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Özel Teşebbüslere', HESAPKOD='83007019005' WHERE YIL=2021 AND HESAPKOD='83007010905'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Hane Halklarına', HESAPKOD='83007019006' WHERE YIL=2021 AND HESAPKOD='83007010906'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Türk Kültür Varlığının Korunması ve Tanıtımı Giderleri', HESAPKOD='83007019007' WHERE YIL=2021 AND HESAPKOD='83007010908'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Siyasi Tanıtma Giderleri', HESAPKOD='83007019008' WHERE YIL=2021 AND HESAPKOD='83007010909'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Gençlik ve Spor İl Müdürlüklerine Hazine Yardımı', HESAPKOD='83007039001' WHERE YIL=2021 AND HESAPKOD='83007010911'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kalkınma Ajanslarına', HESAPKOD='83007019009' WHERE YIL=2021 AND HESAPKOD='83007010912'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Bölgelerarası İşbirliği Proje Giderleri', HESAPKOD='83007019010' WHERE YIL=2021 AND HESAPKOD='83007010913'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sosyal ve İktisadi Araştırma Proje Giderleri', HESAPKOD='83007019011' WHERE YIL=2021 AND HESAPKOD='83007010914'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Tarım ve Kırsal Kalkınmayı Destekleme Kurumuna', HESAPKOD='83007019012' WHERE YIL=2021 AND HESAPKOD='83007010915'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Akademik Ar-Ge Projelerine İlişkin Transferler', HESAPKOD='83007019013' WHERE YIL=2021 AND HESAPKOD='83007010920'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Akademik Ar-Ge Projelerine İlişkin Kurum Hissesi', HESAPKOD='83007019014' WHERE YIL=2021 AND HESAPKOD='83007010921'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kamu Ar-Ge Projelerine İlişkin Transferler', HESAPKOD='83007019015' WHERE YIL=2021 AND HESAPKOD='83007010922'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kamu Ar-Ge Projelerine İlişkin Kurum Hissesi', HESAPKOD='83007019016' WHERE YIL=2021 AND HESAPKOD='83007010923'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Savunma ve Uzay Sanayii Ar-Ge Projelerine İlişkin Transferler', HESAPKOD='83007019017' WHERE YIL=2021 AND HESAPKOD='83007010924'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Savunma ve Uzay Sanayii Ar-Ge Projelerine İlişkin Kurum Hissesi', HESAPKOD='83007019018' WHERE YIL=2021 AND HESAPKOD='83007010925'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Bilim ve Toplum Projelerine İlişkin Transferler', HESAPKOD='83007019019' WHERE YIL=2021 AND HESAPKOD='83007010926'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Bilim ve Toplum Projelerine İlişkin Kurum Hissesi', HESAPKOD='83007019020' WHERE YIL=2021 AND HESAPKOD='83007010927'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Devlet Yükseköğretim Kurumlarına Bölgesel Kalkınma Destekleri', HESAPKOD='83007019021' WHERE YIL=2021 AND HESAPKOD='83007010928'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sınıflandırmaya Girmeyen Diğer Kurum, İşletme ve Hane Halkına  Yapılan Sermaye Transferleri', HESAPKOD='83007019090' WHERE YIL=2021 AND HESAPKOD='83007010990'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Bilimsel Araştırma Projelerinin Desteklenmesine İlişkin Giderler', HESAPKOD='83007019022' WHERE YIL=2021 AND HESAPKOD='83007010999'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yurtdışı Sermaye Transferleri', HESAPKOD='8300702' WHERE YIL=2021 AND HESAPKOD='8300702'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Dış Ülkelere Yapılan Yardımlar', HESAPKOD='830070210' WHERE YIL=2021 AND HESAPKOD='830070201'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kuzey Kıbrıs Türk Cumhuriyeti ne', HESAPKOD='83007021001' WHERE YIL=2021 AND HESAPKOD='83007020101'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Ülkelere', HESAPKOD='83007021090' WHERE YIL=2021 AND HESAPKOD='83007020190'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Uluslararası Kuruluşlara Yapılan Ödemeler', HESAPKOD='830070220' WHERE YIL=2021 AND HESAPKOD='830070202'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Uluslararası Kuruluşlara Katkı Ödemeleri', HESAPKOD='83007022001' WHERE YIL=2021 AND HESAPKOD='83007020201'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Uluslararası Kuruluşlara Yardım Ödemeleri', HESAPKOD='83007022002' WHERE YIL=2021 AND HESAPKOD='83007020202'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Uluslararası Kuruluşlara Diğer Ödemeler', HESAPKOD='83007022090' WHERE YIL=2021 AND HESAPKOD='83007020290'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Uluslarüstü Kuruluşlara', HESAPKOD='830070230' WHERE YIL=2021 AND HESAPKOD='830070203'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Uluslarüstü Kuruluşlara', HESAPKOD='83007023001' WHERE YIL=2021 AND HESAPKOD='83007020301'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Uluslarüstü Kuruluşlardan Merkezlerine ', HESAPKOD='830070240' WHERE YIL=2021 AND HESAPKOD='830070204'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Uluslarüstü Kuruluşlardan Merkezlerine', HESAPKOD='83007024001' WHERE YIL=2021 AND HESAPKOD='83007020401'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Eğitim Kurumlarına', HESAPKOD='830070250' WHERE YIL=2021 AND HESAPKOD='830070205'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Hoca Ahmet Yesevi Uluslararası Türk Kazak Üniversitesine', HESAPKOD='83007025001' WHERE YIL=2021 AND HESAPKOD='83007020501'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kırgızistan-Türkiye Manas Üniversitesine', HESAPKOD='83007025002' WHERE YIL=2021 AND HESAPKOD='83007020502'
DELETE FROM  MUHHESAPPLANI WHERE YIL=2021 AND HESAPKOD='83007020503'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Yurtdışı Sermaye Transferleri', HESAPKOD='830070290' WHERE YIL=2021 AND HESAPKOD='830070209'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sınıflandırmaya Girmeyen Diğer Yurtdışı Sermaye Transferleri', HESAPKOD='83007029090' WHERE YIL=2021 AND HESAPKOD='83007020901'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Türk Kültür Varlığının Korunması ve Tanıtımı Giderleri', HESAPKOD='83007029001' WHERE YIL=2021 AND HESAPKOD='83007020908'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Siyasi Tanıtma Giderleri', HESAPKOD='83007029002' WHERE YIL=2021 AND HESAPKOD='83007020909'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Borç Verme ', HESAPKOD='83008' WHERE YIL=2021 AND HESAPKOD='83008'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yurtiçi Borç Verme ', HESAPKOD='8300801' WHERE YIL=2021 AND HESAPKOD='8300801'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Özel Bütçeli İdarelere Borç Verme', HESAPKOD='830080120' WHERE YIL=2021 AND HESAPKOD='830080102'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Ölçme, Seçme ve Yerleştirme Merkezi Başkanlığı na', HESAPKOD='83008012001' WHERE YIL=2021 AND HESAPKOD='83008010201'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Atatürk Kültür, Dil ve Tarih Yüksek Kurumu na', HESAPKOD='83008012002' WHERE YIL=2021 AND HESAPKOD='83008010202'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Atatürk Araştırma Merkezi ne', HESAPKOD='83008012003' WHERE YIL=2021 AND HESAPKOD='83008010203'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Atatürk Kültür Merkezi ne', HESAPKOD='83008012004' WHERE YIL=2021 AND HESAPKOD='83008010204'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Türk Dil Kurumu na', HESAPKOD='83008012005' WHERE YIL=2021 AND HESAPKOD='83008010205'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Türk Tarih Kurumu na', HESAPKOD='83008012006' WHERE YIL=2021 AND HESAPKOD='83008010206'
DELETE FROM  MUHHESAPPLANI WHERE YIL=2021 AND HESAPKOD='83008010207'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Türkiye Bilimsel ve Teknolojik Araştırma Kurumu Başkanlığı na', HESAPKOD='83008012007' WHERE YIL=2021 AND HESAPKOD='83008010208'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Türkiye Bilimler Akademisi Başkanlığı na', HESAPKOD='83008012008' WHERE YIL=2021 AND HESAPKOD='83008010209'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Türkiye Adalet Akademisi’ne ', HESAPKOD='83008012009' WHERE YIL=2021 AND HESAPKOD='83008010210'
DELETE FROM  MUHHESAPPLANI WHERE YIL=2021 AND HESAPKOD='83008010213'
DELETE FROM  MUHHESAPPLANI WHERE YIL=2021 AND HESAPKOD='83008010214'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Devlet Tiyatroları Genel Müdürlüğü ne', HESAPKOD='83008012010' WHERE YIL=2021 AND HESAPKOD='83008010215'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Devlet Opera ve Balesi Genel Müdürlüğü ne', HESAPKOD='83008012011' WHERE YIL=2021 AND HESAPKOD='83008010216'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Vakıflar Genel Müdürlüğü ne', HESAPKOD='83008012013' WHERE YIL=2021 AND HESAPKOD='83008010218'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Türkiye Hudut ve Sahiller Sağlık Genel Müdürlüğü ne', HESAPKOD='83008012014' WHERE YIL=2021 AND HESAPKOD='83008010219'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Türk Akreditasyon Kurumu na', HESAPKOD='83008012015' WHERE YIL=2021 AND HESAPKOD='83008010221'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Türk Standartları Enstitüsü ne', HESAPKOD='83008012016' WHERE YIL=2021 AND HESAPKOD='83008010222'
DELETE FROM  MUHHESAPPLANI WHERE YIL=2021 AND HESAPKOD='83008010223'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Türk Patent Enstitüsü ne', HESAPKOD='83008012017' WHERE YIL=2021 AND HESAPKOD='83008010224'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Ulusal Bor Araştırma Enstitüsü ne', HESAPKOD='83008012018' WHERE YIL=2021 AND HESAPKOD='83008010226'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Türkiye Atom Enerjisi Kurumu na', HESAPKOD='83008012019' WHERE YIL=2021 AND HESAPKOD='83008010227'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Savunma Sanayi Başkanlığı na', HESAPKOD='83008012020' WHERE YIL=2021 AND HESAPKOD='83008010228'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Küçük ve Orta Ölçekli İşletmeleri Geliştirme ve Destekleme İdaresi Başkanlığı na', HESAPKOD='83008012021' WHERE YIL=2021 AND HESAPKOD='83008010230'
DELETE FROM  MUHHESAPPLANI WHERE YIL=2021 AND HESAPKOD='83008010231'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Türk İşbirliği ve Koordinasyon Ajansı Başkanlığı na', HESAPKOD='83008012022' WHERE YIL=2021 AND HESAPKOD='83008010232'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='GAP Bölge Kalkınma İdaresi Başkanlığı na', HESAPKOD='83008012023' WHERE YIL=2021 AND HESAPKOD='83008010234'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Özelleştirme İdaresi Başkanlığı na', HESAPKOD='83008012024' WHERE YIL=2021 AND HESAPKOD='83008010235'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Maden Tetkik ve Arama Genel Müdürlüğü ne', HESAPKOD='83008012025' WHERE YIL=2021 AND HESAPKOD='83008010240'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Ceza İnfaz Kurumları ile Tutukevleri İş Yurtları Kurumu na', HESAPKOD='83008012026' WHERE YIL=2021 AND HESAPKOD='83008010241'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sivil Havacılık Genel Müdürlüğü ne', HESAPKOD='83008012027' WHERE YIL=2021 AND HESAPKOD='83008010249'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yurtdışı Türkler ve Akraba Topluluklar Başkanlığı na', HESAPKOD='83008012029' WHERE YIL=2021 AND HESAPKOD='83008010251'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Karayolları Genel Müdürlüğü ne', HESAPKOD='83008012030' WHERE YIL=2021 AND HESAPKOD='83008010252'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Türkiye Yazma Eserler Kurumu Başkanlığı na', HESAPKOD='83008012031' WHERE YIL=2021 AND HESAPKOD='83008010253'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Doğu Anadolu Projesi Bölge Kalkınma İdaresi Başkanlığı na', HESAPKOD='83008012032' WHERE YIL=2021 AND HESAPKOD='83008010254'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Konya Ovası Projesi Bölge Kalkınma İdaresi Başkanlığı na', HESAPKOD='83008012033' WHERE YIL=2021 AND HESAPKOD='83008010255'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Doğu Karadeniz Projesi Bölge Kalkınma İdaresi Başkanlığı na', HESAPKOD='83008012034' WHERE YIL=2021 AND HESAPKOD='83008010256'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Devlet Su İşleri Genel Müdürlüğü ne', HESAPKOD='83008012035' WHERE YIL=2021 AND HESAPKOD='83008010257'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Türkiye Su Enstitüsü ne', HESAPKOD='83008012036' WHERE YIL=2021 AND HESAPKOD='83008010258'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Türkiye İlaç ve Tıbbi Cihaz Kurumu na', HESAPKOD='83008012037' WHERE YIL=2021 AND HESAPKOD='83008010259'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kamu Denetçiliği Kurumu na', HESAPKOD='83008012038' WHERE YIL=2021 AND HESAPKOD='83008010260'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Türkiye İnsan Hakları ve Eşitlik Kurumu na', HESAPKOD='83008012039' WHERE YIL=2021 AND HESAPKOD='83008010261'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Düzenleyici ve Denetleyici Kurumlara Borç Verme', HESAPKOD='830080130' WHERE YIL=2021 AND HESAPKOD='830080103'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Radyo ve Televizyon Üst Kurulu na', HESAPKOD='83008013001' WHERE YIL=2021 AND HESAPKOD='83008010301'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Bilgi Teknolojileri ve İletişim Kurumu na', HESAPKOD='83008013002' WHERE YIL=2021 AND HESAPKOD='83008010302'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sermaye Piyasası Kurulu na', HESAPKOD='83008013003' WHERE YIL=2021 AND HESAPKOD='83008010303'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Bankacılık Düzenleme ve Denetleme Kurumu na', HESAPKOD='83008013004' WHERE YIL=2021 AND HESAPKOD='83008010304'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Enerji Piyasası Düzenleme Kurumu na', HESAPKOD='83008013005' WHERE YIL=2021 AND HESAPKOD='83008010305'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kamu İhale Kurumu na', HESAPKOD='83008013006' WHERE YIL=2021 AND HESAPKOD='83008010306'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Rekabet Kurumu na', HESAPKOD='83008013007' WHERE YIL=2021 AND HESAPKOD='83008010307'
DELETE FROM  MUHHESAPPLANI WHERE YIL=2021 AND HESAPKOD='83008010309'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kamu Gözetimi, Muhasebe ve Denetim Standartları Kurumu na', HESAPKOD='83008013008' WHERE YIL=2021 AND HESAPKOD='83008010310'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sosyal Güvenlik Kurumlarına Borç Verme', HESAPKOD='830080140' WHERE YIL=2021 AND HESAPKOD='830080104'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Türkiye İş Kurumu Genel Müdürlüğü ne', HESAPKOD='83008014001' WHERE YIL=2021 AND HESAPKOD='83008010404'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sosyal Güvenlik Kurumu na', HESAPKOD='83008014002' WHERE YIL=2021 AND HESAPKOD='83008010406'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Sosyal Güvenlik Kurumlarına', HESAPKOD='83008014090' WHERE YIL=2021 AND HESAPKOD='83008010490'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Mahalli İdarelere Borç Verme', HESAPKOD='830080150' WHERE YIL=2021 AND HESAPKOD='830080105'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='İl Özel İdarelerine', HESAPKOD='83008015001' WHERE YIL=2021 AND HESAPKOD='83008010501'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Belediyelere', HESAPKOD='83008015002' WHERE YIL=2021 AND HESAPKOD='83008010502'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Köylere', HESAPKOD='83008015003' WHERE YIL=2021 AND HESAPKOD='83008010503'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Mahalli İdare Birliklerine', HESAPKOD='83008015004' WHERE YIL=2021 AND HESAPKOD='83008010504'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Bağlı İdarelere', HESAPKOD='83008015005' WHERE YIL=2021 AND HESAPKOD='83008010505'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kamu Teşebbüslerine, Döner Sermayelere, Fonlara ve Mali Kurumlara Borç Verme', HESAPKOD='830080160' WHERE YIL=2021 AND HESAPKOD='830080106'
DELETE FROM  MUHHESAPPLANI WHERE YIL=2021 AND HESAPKOD='83008010601'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='TC Devlet Demiryolları na', HESAPKOD='83008016001' WHERE YIL=2021 AND HESAPKOD='83008010602'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Tarım İşletmeleri Genel Müdürlüğü ne', HESAPKOD='83008016002' WHERE YIL=2021 AND HESAPKOD='83008010603'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Toprak Mahsulleri Ofisi ne', HESAPKOD='83008016003' WHERE YIL=2021 AND HESAPKOD='83008010604'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Teşebbüslere', HESAPKOD='83008016090' WHERE YIL=2021 AND HESAPKOD='83008010605'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Çay İşletmeleri Genel Müdürlüğü ne', HESAPKOD='83008016004' WHERE YIL=2021 AND HESAPKOD='83008010606'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Türkiye Elektrik Dağıtım AŞ ye', HESAPKOD='83008016005' WHERE YIL=2021 AND HESAPKOD='83008010607'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Elektrik Üretim AŞ ye', HESAPKOD='83008016006' WHERE YIL=2021 AND HESAPKOD='83008010608'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Elektrik İletim AŞ ye', HESAPKOD='83008016007' WHERE YIL=2021 AND HESAPKOD='83008010609'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Türkiye Elektrik ve Taahhüt AŞ ye', HESAPKOD='83008016008' WHERE YIL=2021 AND HESAPKOD='83008010610'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Türkiye Taşkömürü Kurumu na', HESAPKOD='83008016009' WHERE YIL=2021 AND HESAPKOD='83008010611'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Üretici Birliklerine', HESAPKOD='83008016010' WHERE YIL=2021 AND HESAPKOD='83008010612'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Tarım Kredi Kooperatiflerine', HESAPKOD='83008016011' WHERE YIL=2021 AND HESAPKOD='83008010613'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Türkiye Şeker Fabrikaları AŞ ye', HESAPKOD='83008016012' WHERE YIL=2021 AND HESAPKOD='83008010614'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sümer Holding AŞ ye', HESAPKOD='83008016013' WHERE YIL=2021 AND HESAPKOD='83008010615'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Makina ve Kimya Endüstrisi Kurumu na', HESAPKOD='83008016014' WHERE YIL=2021 AND HESAPKOD='83008010616'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Teşebbüslere', HESAPKOD='83008016090' WHERE YIL=2021 AND HESAPKOD='83008010617'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Türkiye Kömür İşletmeleri Kurumu na', HESAPKOD='83008016015' WHERE YIL=2021 AND HESAPKOD='83008010618'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Et ve Süt Kurumu Genel Müdürlüğü ne', HESAPKOD='83008016016' WHERE YIL=2021 AND HESAPKOD='83008010619'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Türkiye Elektromekanik Sanayii Genel Müdürlüğü ne', HESAPKOD='83008016017' WHERE YIL=2021 AND HESAPKOD='83008010620'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Posta ve Telgraf Teşkilatı Genel Müdürlüğü ne', HESAPKOD='83008016018' WHERE YIL=2021 AND HESAPKOD='83008010622'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Türkiye Petrolleri Anonim Ortaklığı’na', HESAPKOD='83008016019' WHERE YIL=2021 AND HESAPKOD='83008010623'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Uluslararası Sağlık Hizmetleri AŞ’ye', HESAPKOD='83008016020' WHERE YIL=2021 AND HESAPKOD='83008010624'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='NÜTED Nükleer Teknik Destek AŞ’ye', HESAPKOD='83008016021' WHERE YIL=2021 AND HESAPKOD='83008010625'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Türk Reasürans AŞ’ye', HESAPKOD='83008016022' WHERE YIL=2021 AND HESAPKOD='83008010626'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Birleşik İpotek Finansmanı Anonim Şirketi’ne', HESAPKOD='83008016024' WHERE YIL=2021 AND HESAPKOD='83008010627'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Türkiye Raylı Sistem Araçları Sanayii AŞ’ye', HESAPKOD='83008016025' WHERE YIL=2021 AND HESAPKOD='83008010628'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Teşebbüslere', HESAPKOD='83008016090' WHERE YIL=2021 AND HESAPKOD='83008010649'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Darphane ve Damga Matbaası Genel Müdürlüğü ne', HESAPKOD='83008016101' WHERE YIL=2021 AND HESAPKOD='83008010650'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sınıflandırmaya Girmeyen Diğer Yurtiçi Borç Verme', HESAPKOD='83008016190' WHERE YIL=2021 AND HESAPKOD='83008010659'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Ziraat Bankasına', HESAPKOD='83008016201' WHERE YIL=2021 AND HESAPKOD='83008010670'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Türkiye Kalkınma ve Yatırım Bankasına', HESAPKOD='83008016202' WHERE YIL=2021 AND HESAPKOD='83008010671'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Türk Eximbanka', HESAPKOD='83008016203' WHERE YIL=2021 AND HESAPKOD='83008010672'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Mali Kurumlara', HESAPKOD='83008016290' WHERE YIL=2021 AND HESAPKOD='83008010679'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kredi Garanti Fonuna', HESAPKOD='83008016301' WHERE YIL=2021 AND HESAPKOD='83008010680'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Fonlara', HESAPKOD='83008016390' WHERE YIL=2021 AND HESAPKOD='83008010689'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yükseköğretim Kurumlarına Borç Verme', HESAPKOD='830080170' WHERE YIL=2021 AND HESAPKOD='830080107'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yükseköğretim Kurulu na', HESAPKOD='83008017001' WHERE YIL=2021 AND HESAPKOD='83008010701'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Ankara Üniversitesi ne', HESAPKOD='83008017002' WHERE YIL=2021 AND HESAPKOD='83008010702'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Ortadoğu Teknik Üniversitesi ne', HESAPKOD='83008017003' WHERE YIL=2021 AND HESAPKOD='83008010703'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Hacettepe Üniversitesi ne', HESAPKOD='83008017004' WHERE YIL=2021 AND HESAPKOD='83008010704'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Gazi Üniversitesi ne', HESAPKOD='83008017005' WHERE YIL=2021 AND HESAPKOD='83008010705'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='İstanbul Üniversitesi ne', HESAPKOD='83008017006' WHERE YIL=2021 AND HESAPKOD='83008010706'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='İstanbul Teknik Üniversitesi ne', HESAPKOD='83008017007' WHERE YIL=2021 AND HESAPKOD='83008010707'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Boğaziçi Üniversitesi ne', HESAPKOD='83008017008' WHERE YIL=2021 AND HESAPKOD='83008010708'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Marmara Üniversitesi ne', HESAPKOD='83008017009' WHERE YIL=2021 AND HESAPKOD='83008010709'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yıldız Teknik Üniversitesi ne', HESAPKOD='83008017010' WHERE YIL=2021 AND HESAPKOD='83008010710'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Mimar Sinan Güzel Sanatlar Üniversitesi ne', HESAPKOD='83008017011' WHERE YIL=2021 AND HESAPKOD='83008010711'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Ege Üniversitesi ne', HESAPKOD='83008017012' WHERE YIL=2021 AND HESAPKOD='83008010712'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Dokuz Eylül Üniversitesi ne', HESAPKOD='83008017013' WHERE YIL=2021 AND HESAPKOD='83008010713'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Trakya Üniversitesi ne', HESAPKOD='83008017014' WHERE YIL=2021 AND HESAPKOD='83008010714'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Bursa Uludağ Üniversitesi ne', HESAPKOD='83008017015' WHERE YIL=2021 AND HESAPKOD='83008010715'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Anadolu Üniversitesi ne', HESAPKOD='83008017016' WHERE YIL=2021 AND HESAPKOD='83008010716'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Selçuk Üniversitesi ne', HESAPKOD='83008017017' WHERE YIL=2021 AND HESAPKOD='83008010717'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Akdeniz Üniversitesi ne', HESAPKOD='83008017018' WHERE YIL=2021 AND HESAPKOD='83008010718'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Erciyes Üniversitesi ne', HESAPKOD='83008017019' WHERE YIL=2021 AND HESAPKOD='83008010719'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sivas Cumhuriyet Üniversitesi ne', HESAPKOD='83008017020' WHERE YIL=2021 AND HESAPKOD='83008010720'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Çukurova Üniversitesi ne', HESAPKOD='83008017021' WHERE YIL=2021 AND HESAPKOD='83008010721'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Ondokuz Mayıs Üniversitesi ne', HESAPKOD='83008017022' WHERE YIL=2021 AND HESAPKOD='83008010722'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Karadeniz Teknik Üniversitesi ne', HESAPKOD='83008017023' WHERE YIL=2021 AND HESAPKOD='83008010723'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Atatürk Üniversitesi ne', HESAPKOD='83008017024' WHERE YIL=2021 AND HESAPKOD='83008010724'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='İnönü Üniversitesi ne', HESAPKOD='83008017025' WHERE YIL=2021 AND HESAPKOD='83008010725'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Fırat Üniversitesi ne', HESAPKOD='83008017026' WHERE YIL=2021 AND HESAPKOD='83008010726'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Dicle Üniversitesi ne', HESAPKOD='83008017027' WHERE YIL=2021 AND HESAPKOD='83008010727'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yüzüncüyıl Üniversitesi ne', HESAPKOD='83008017028' WHERE YIL=2021 AND HESAPKOD='83008010728'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Gaziantep Üniversitesi ne', HESAPKOD='83008017029' WHERE YIL=2021 AND HESAPKOD='83008010729'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='İzmir Yüksek Teknoloji Enstitüsü ne', HESAPKOD='83008017030' WHERE YIL=2021 AND HESAPKOD='83008010730'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Gebze Teknik Üniversitesi ne', HESAPKOD='83008017031' WHERE YIL=2021 AND HESAPKOD='83008010731'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Harran Üniversitesi ne', HESAPKOD='83008017032' WHERE YIL=2021 AND HESAPKOD='83008010732'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Süleyman Demirel Üniversitesi ne', HESAPKOD='83008017033' WHERE YIL=2021 AND HESAPKOD='83008010733'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Aydın Adnan Menderes Üniversitesi ne', HESAPKOD='83008017034' WHERE YIL=2021 AND HESAPKOD='83008010734'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Zonguldak Bülent Ecevit Üniversitesi ne', HESAPKOD='83008017035' WHERE YIL=2021 AND HESAPKOD='83008010735'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Mersin Üniversitesi ne', HESAPKOD='83008017036' WHERE YIL=2021 AND HESAPKOD='83008010736'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Pamukkale Üniversitesi ne', HESAPKOD='83008017037' WHERE YIL=2021 AND HESAPKOD='83008010737'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Balıkesir Üniversitesi ne', HESAPKOD='83008017038' WHERE YIL=2021 AND HESAPKOD='83008010738'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kocaeli Üniversitesi ne', HESAPKOD='83008017039' WHERE YIL=2021 AND HESAPKOD='83008010739'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sakarya Üniversitesi ne', HESAPKOD='83008017040' WHERE YIL=2021 AND HESAPKOD='83008010740'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Manisa Celal Bayar Üniversitesi’ne', HESAPKOD='83008017041' WHERE YIL=2021 AND HESAPKOD='83008010741'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Bolu Abant İzzet Baysal Üniversitesi ne', HESAPKOD='83008017042' WHERE YIL=2021 AND HESAPKOD='83008010742'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Hatay Mustafa Kemal Üniversitesi ne', HESAPKOD='83008017043' WHERE YIL=2021 AND HESAPKOD='83008010743'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Afyon Kocatepe Üniversitesi ne', HESAPKOD='83008017044' WHERE YIL=2021 AND HESAPKOD='83008010744'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kafkas Üniversitesi ne', HESAPKOD='83008017045' WHERE YIL=2021 AND HESAPKOD='83008010745'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Çanakkale 18 Mart Üniversitesi ne', HESAPKOD='83008017046' WHERE YIL=2021 AND HESAPKOD='83008010746'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Niğde Ömer Halisdemir Üniversitesi’ne', HESAPKOD='83008017047' WHERE YIL=2021 AND HESAPKOD='83008010747'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kütahya Dumlupınar Üniversitesi ne', HESAPKOD='83008017048' WHERE YIL=2021 AND HESAPKOD='83008010748'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Tokat Gaziosmanpaşa Üniversitesi ne', HESAPKOD='83008017049' WHERE YIL=2021 AND HESAPKOD='83008010749'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Muğla Sıtkı Koçman Üniversitesi ne', HESAPKOD='83008017050' WHERE YIL=2021 AND HESAPKOD='83008010750'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kahramanmaraş Sütçü İmam Üniversitesi ne', HESAPKOD='83008017051' WHERE YIL=2021 AND HESAPKOD='83008010751'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kırıkkale Üniversitesi ne', HESAPKOD='83008017052' WHERE YIL=2021 AND HESAPKOD='83008010752'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Eskişehir Osmangazi Üniversitesi ne', HESAPKOD='83008017053' WHERE YIL=2021 AND HESAPKOD='83008010753'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Galatasaray Üniversitesi ne', HESAPKOD='83008017054' WHERE YIL=2021 AND HESAPKOD='83008010754'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kırşehir Ahi Evran Üniversitesi ne', HESAPKOD='83008017055' WHERE YIL=2021 AND HESAPKOD='83008010755'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kastamonu Üniversitesi ne', HESAPKOD='83008017056' WHERE YIL=2021 AND HESAPKOD='83008010756'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Düzce Üniversitesi ne', HESAPKOD='83008017057' WHERE YIL=2021 AND HESAPKOD='83008010757'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Burdur Mehmet Akif Ersoy Üniversitesi ne', HESAPKOD='83008017058' WHERE YIL=2021 AND HESAPKOD='83008010758'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Uşak Üniversitesi ne', HESAPKOD='83008017059' WHERE YIL=2021 AND HESAPKOD='83008010759'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Recep Tayyip Erdoğan Üniversitesi ne', HESAPKOD='83008017060' WHERE YIL=2021 AND HESAPKOD='83008010760'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Tekirdağ Namık Kemal Üniversitesi ne', HESAPKOD='83008017061' WHERE YIL=2021 AND HESAPKOD='83008010761'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Erzincan Binali Yıldırım Üniversitesi ne', HESAPKOD='83008017062' WHERE YIL=2021 AND HESAPKOD='83008010762'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Aksaray Üniversitesi ne', HESAPKOD='83008017063' WHERE YIL=2021 AND HESAPKOD='83008010763'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Giresun Üniversitesi ne', HESAPKOD='83008017064' WHERE YIL=2021 AND HESAPKOD='83008010764'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Hitit Üniversitesi ne', HESAPKOD='83008017065' WHERE YIL=2021 AND HESAPKOD='83008010765'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yozgat Bozok Üniversitesi ne', HESAPKOD='83008017066' WHERE YIL=2021 AND HESAPKOD='83008010766'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Adıyaman Üniversitesi ne', HESAPKOD='83008017067' WHERE YIL=2021 AND HESAPKOD='83008010767'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Ordu Üniversitesi ne', HESAPKOD='83008017068' WHERE YIL=2021 AND HESAPKOD='83008010768'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Amasya Üniversitesi ne', HESAPKOD='83008017069' WHERE YIL=2021 AND HESAPKOD='83008010769'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Karamanoğlu Mehmetbey Üniversitesi ne', HESAPKOD='83008017070' WHERE YIL=2021 AND HESAPKOD='83008010770'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Ağrı İbrahim Çeçen Üniversitesi ne', HESAPKOD='83008017071' WHERE YIL=2021 AND HESAPKOD='83008010771'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sinop Üniversitesi ne', HESAPKOD='83008017072' WHERE YIL=2021 AND HESAPKOD='83008010772'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Siirt Üniversitesi ne', HESAPKOD='83008017073' WHERE YIL=2021 AND HESAPKOD='83008010773'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Nevşehir Hacı Bektaş Veli Üniversitesi ne', HESAPKOD='83008017074' WHERE YIL=2021 AND HESAPKOD='83008010774'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Karabük Üniversitesi ne', HESAPKOD='83008017075' WHERE YIL=2021 AND HESAPKOD='83008010775'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kilis 7 Aralık Üniversitesi ne', HESAPKOD='83008017076' WHERE YIL=2021 AND HESAPKOD='83008010776'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Çankırı Karatekin Üniversitesi ne', HESAPKOD='83008017077' WHERE YIL=2021 AND HESAPKOD='83008010777'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Artvin Çoruh Üniversitesi ne', HESAPKOD='83008017078' WHERE YIL=2021 AND HESAPKOD='83008010778'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Bilecik Şeyh Edebali Üniversitesi ne', HESAPKOD='83008017079' WHERE YIL=2021 AND HESAPKOD='83008010779'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Bitlis Eren Üniversitesi ne', HESAPKOD='83008017080' WHERE YIL=2021 AND HESAPKOD='83008010780'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kırklareli Üniversitesi ne', HESAPKOD='83008017081' WHERE YIL=2021 AND HESAPKOD='83008010781'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Osmaniye Korkut Ata Üniversitesi ne', HESAPKOD='83008017082' WHERE YIL=2021 AND HESAPKOD='83008010782'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Bingöl Üniversitesi ne', HESAPKOD='83008017083' WHERE YIL=2021 AND HESAPKOD='83008010783'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Muş Alparslan Üniversitesi ne', HESAPKOD='83008017084' WHERE YIL=2021 AND HESAPKOD='83008010784'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Mardin Artuklu Üniversitesi ne', HESAPKOD='83008017085' WHERE YIL=2021 AND HESAPKOD='83008010785'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Batman Üniversitesi ne', HESAPKOD='83008017086' WHERE YIL=2021 AND HESAPKOD='83008010786'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Ardahan Üniversitesi ne', HESAPKOD='83008017087' WHERE YIL=2021 AND HESAPKOD='83008010787'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Bartın Üniversitesi ne', HESAPKOD='83008017088' WHERE YIL=2021 AND HESAPKOD='83008010788'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Bayburt Üniversitesi ne', HESAPKOD='83008017089' WHERE YIL=2021 AND HESAPKOD='83008010789'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Gümüşhane Üniversitesi ne', HESAPKOD='83008017090' WHERE YIL=2021 AND HESAPKOD='83008010790'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Hakkari Üniversitesi ne', HESAPKOD='83008017091' WHERE YIL=2021 AND HESAPKOD='83008010791'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Iğdır Üniversitesi ne', HESAPKOD='83008017092' WHERE YIL=2021 AND HESAPKOD='83008010792'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Şırnak Üniversitesi ne', HESAPKOD='83008017093' WHERE YIL=2021 AND HESAPKOD='83008010793'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Munzur Üniversitesi ne', HESAPKOD='83008017094' WHERE YIL=2021 AND HESAPKOD='83008010794'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yalova Üniversitesi ne', HESAPKOD='83008017095' WHERE YIL=2021 AND HESAPKOD='83008010795'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Türk-Alman Üniversitesi ne', HESAPKOD='83008017096' WHERE YIL=2021 AND HESAPKOD='83008010796'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Ankara Yıldırım Beyazıt Üniversitesi ne', HESAPKOD='83008017097' WHERE YIL=2021 AND HESAPKOD='83008010797'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Bursa Teknik Üniversitesi ne', HESAPKOD='83008017098' WHERE YIL=2021 AND HESAPKOD='83008010798'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='İstanbul Medeniyet Üniversitesi ne', HESAPKOD='83008017099' WHERE YIL=2021 AND HESAPKOD='83008010799'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yükseköğretim Kurumlarına Borç Verme', HESAPKOD='830080180' WHERE YIL=2021 AND HESAPKOD='830080108'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='İzmir Katip Çelebi Üniversitesi ne', HESAPKOD='83008018001' WHERE YIL=2021 AND HESAPKOD='83008010801'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Necmettin Erbakan Üniversitesi ne', HESAPKOD='83008018002' WHERE YIL=2021 AND HESAPKOD='83008010802'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Abdullah Gül Üniversitesi ne', HESAPKOD='83008018003' WHERE YIL=2021 AND HESAPKOD='83008010803'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Erzurum Teknik Üniversitesi ne', HESAPKOD='83008018004' WHERE YIL=2021 AND HESAPKOD='83008010804'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Ankara Sosyal Bilimler Üniversitesi ne', HESAPKOD='83008018006' WHERE YIL=2021 AND HESAPKOD='83008010806'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Yurtiçi Borç Verme', HESAPKOD='830080190' WHERE YIL=2021 AND HESAPKOD='830080109'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Dernek, Birlik, Kurum, Kuruluş, Sandık vb Kuruluşlara', HESAPKOD='83008019001' WHERE YIL=2021 AND HESAPKOD='83008010901'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Vakıf Üniversitelerine', HESAPKOD='83008019002' WHERE YIL=2021 AND HESAPKOD='83008010902'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Kamu Kurum ve Kuruluşlarına', HESAPKOD='83008019003' WHERE YIL=2021 AND HESAPKOD='83008010903'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kamu Ortaklıklarına', HESAPKOD='83008019004' WHERE YIL=2021 AND HESAPKOD='83008010904'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Özel Teşebbüslere', HESAPKOD='83008019005' WHERE YIL=2021 AND HESAPKOD='83008010905'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='AR-GE, İNOVASYON VE ENDÜSTRİYEL UYGULAMA DESTEKLERİ', HESAPKOD='8300801900501' WHERE YIL=2021 AND HESAPKOD='8300801090501'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='GİRİŞİMCİLİK DESTEKLERİ', HESAPKOD='8300801900502' WHERE YIL=2021 AND HESAPKOD='8300801090502'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='İŞBİRLİĞİ-GÜÇBİRLİĞİ DESTEKLERİ', HESAPKOD='8300801900503' WHERE YIL=2021 AND HESAPKOD='8300801090503'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='KOBİ GELİŞİM (KOBİGEL) DESTEKLERİ', HESAPKOD='8300801900504' WHERE YIL=2021 AND HESAPKOD='8300801090504'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='STRATEJİK ÜRÜN DESTEKLERİ', HESAPKOD='8300801900505' WHERE YIL=2021 AND HESAPKOD='8300801090505'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='KOBİ TEKNOLOJİK ÜRÜN YATIRIM (KOBİ TEKNOYATIRIM) DESTEKLERİ', HESAPKOD='8300801900506' WHERE YIL=2021 AND HESAPKOD='8300801090506'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='İŞBİRLİĞİ DESTEKLERİ', HESAPKOD='8300801900507' WHERE YIL=2021 AND HESAPKOD='8300801090507'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='İŞGEM/TEKMER DESTEKLERİ', HESAPKOD='8300801900508' WHERE YIL=2021 AND HESAPKOD='8300801090508'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='YURTDIŞI PAZAR DESTEK PROGRAMI', HESAPKOD='8300801900509' WHERE YIL=2021 AND HESAPKOD='8300801090509'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Hane Halklarına', HESAPKOD='83008019006' WHERE YIL=2021 AND HESAPKOD='83008010906'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sınıflandırmaya Girmeyen Diğer Yurtiçi Borç Verme', HESAPKOD='83008019090' WHERE YIL=2021 AND HESAPKOD='83008010990'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='YURTDIŞI BORÇ VERME', HESAPKOD='8300802' WHERE YIL=2021 AND HESAPKOD='8300802'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Dış Ülkelere Yapılan Yardımlar', HESAPKOD='830080210' WHERE YIL=2021 AND HESAPKOD='830080201'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kuzey Kıbrıs Türk Cumhuriyeti ne', HESAPKOD='83008021001' WHERE YIL=2021 AND HESAPKOD='83008020101'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Ülkelere', HESAPKOD='83008021090' WHERE YIL=2021 AND HESAPKOD='83008020190'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Uluslararası Kuruluşlara Yapılan Ödemeler', HESAPKOD='830080220' WHERE YIL=2021 AND HESAPKOD='830080202'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Uluslararası Kuruluşlara Yapılan Ödemeler', HESAPKOD='83008022001' WHERE YIL=2021 AND HESAPKOD='83008020201'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Uluslarüstü Kuruluşlara', HESAPKOD='830080230' WHERE YIL=2021 AND HESAPKOD='830080203'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Uluslarüstü Kuruluşlara', HESAPKOD='83008023002' WHERE YIL=2021 AND HESAPKOD='83008020301'
DELETE FROM  MUHHESAPPLANI WHERE YIL=2021 AND HESAPKOD='83008020795'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Yurtdışı Borç Verme', HESAPKOD='830080290' WHERE YIL=2021 AND HESAPKOD='830080209'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Yurtdışı Borç Verme', HESAPKOD='83008029090' WHERE YIL=2021 AND HESAPKOD='83008020901'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='GÖNDERİLECEK BÜTÇE ÖDENEKLERİ HESABI', HESAPKOD='900' WHERE YIL=2021 AND HESAPKOD='900'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Personel Giderleri', HESAPKOD='90001' WHERE YIL=2021 AND HESAPKOD='90001'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Memurlar', HESAPKOD='9000101' WHERE YIL=2021 AND HESAPKOD='9000101'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sözleşmeli Personel ', HESAPKOD='9000102' WHERE YIL=2021 AND HESAPKOD='9000102'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='İşçiler', HESAPKOD='9000103' WHERE YIL=2021 AND HESAPKOD='9000103'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Geçici Süreli Çalışanlar', HESAPKOD='9000104' WHERE YIL=2021 AND HESAPKOD='9000104'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Personel', HESAPKOD='9000105' WHERE YIL=2021 AND HESAPKOD='9000105'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Milletvekilleri', HESAPKOD='9000107' WHERE YIL=2021 AND HESAPKOD='9000107'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Cumhurbaşkanlığı Ödeneği', HESAPKOD='9000108' WHERE YIL=2021 AND HESAPKOD='9000108'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='İstihbarat Personeli', HESAPKOD='9000109' WHERE YIL=2021 AND HESAPKOD='9000109'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sosyal Güvenlik Kurumlarına Devlet Primi Giderleri', HESAPKOD='90002' WHERE YIL=2021 AND HESAPKOD='90002'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Memurlar', HESAPKOD='9000201' WHERE YIL=2021 AND HESAPKOD='9000201'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sözleşmeli Personel ', HESAPKOD='9000202' WHERE YIL=2021 AND HESAPKOD='9000202'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='İşçiler', HESAPKOD='9000203' WHERE YIL=2021 AND HESAPKOD='9000203'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Geçici Süreli Çalışanlar', HESAPKOD='9000204' WHERE YIL=2021 AND HESAPKOD='9000204'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Personel', HESAPKOD='9000205' WHERE YIL=2021 AND HESAPKOD='9000205'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Milletvekilleri', HESAPKOD='9000207' WHERE YIL=2021 AND HESAPKOD='9000207'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='İstihbarat Personeli', HESAPKOD='9000209' WHERE YIL=2021 AND HESAPKOD='9000209'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Mal ve Hizmet Alım Giderleri', HESAPKOD='90003' WHERE YIL=2021 AND HESAPKOD='90003'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Üretime Yönelik Mal ve Malzeme Alımları', HESAPKOD='9000301' WHERE YIL=2021 AND HESAPKOD='9000301'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Tüketime Yönelik Mal ve Malzeme Alımları', HESAPKOD='9000302' WHERE YIL=2021 AND HESAPKOD='9000302'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yolluklar', HESAPKOD='9000303' WHERE YIL=2021 AND HESAPKOD='9000303'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Görev Giderleri', HESAPKOD='9000304' WHERE YIL=2021 AND HESAPKOD='9000304'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Hizmet Alımları', HESAPKOD='9000305' WHERE YIL=2021 AND HESAPKOD='9000305'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Temsil ve Tanıtma Giderleri', HESAPKOD='9000306' WHERE YIL=2021 AND HESAPKOD='9000306'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Menkul Mal, Gayrimaddi Hak Alım, Bakım ve Onarım Giderleri', HESAPKOD='9000307' WHERE YIL=2021 AND HESAPKOD='9000307'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Gayrimenkul Mal Bakım ve Onarım Giderleri', HESAPKOD='9000308' WHERE YIL=2021 AND HESAPKOD='9000308'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Tedavi ve Cenaze Giderleri', HESAPKOD='9000309' WHERE YIL=2021 AND HESAPKOD='9000309'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Faiz Giderleri', HESAPKOD='90004' WHERE YIL=2021 AND HESAPKOD='90004'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kamu Kurumlarına Ödenen İç Borç Faiz Giderleri', HESAPKOD='9000401' WHERE YIL=2021 AND HESAPKOD='9000401'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer İç Borç Faiz Giderleri', HESAPKOD='9000402' WHERE YIL=2021 AND HESAPKOD='9000402'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Dış Borç Faiz Giderleri', HESAPKOD='9000403' WHERE YIL=2021 AND HESAPKOD='9000403'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='İskonto Giderleri', HESAPKOD='9000404' WHERE YIL=2021 AND HESAPKOD='9000404'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Para Piyasası Nakit İşlemleri Faiz Giderleri', HESAPKOD='9000405' WHERE YIL=2021 AND HESAPKOD='9000405'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Türev Ürün Giderleri', HESAPKOD='9000406' WHERE YIL=2021 AND HESAPKOD='9000406'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kira Sertifikası Giderleri', HESAPKOD='9000407' WHERE YIL=2021 AND HESAPKOD='9000407'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Cari Transferler', HESAPKOD='90005' WHERE YIL=2021 AND HESAPKOD='90005'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Görevlendirme Giderleri', HESAPKOD='9000501' WHERE YIL=2021 AND HESAPKOD='9000501'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Hazine Yardımları', HESAPKOD='9000502' WHERE YIL=2021 AND HESAPKOD='9000502'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kar Amacı Gütmeyen Kuruluşlara Yapılan Transferler', HESAPKOD='9000503' WHERE YIL=2021 AND HESAPKOD='9000503'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Hane Halkı ve İşletmelere Yapılan Transferler', HESAPKOD='9000504' WHERE YIL=2021 AND HESAPKOD='9000504'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Devlet Sosyal Güvenlik Kurumlarından Hane Halkına Yapılan Fayda Ödemeleri', HESAPKOD='9000505' WHERE YIL=2021 AND HESAPKOD='9000505'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yurt Dışına Yapılan Transferler ', HESAPKOD='9000506' WHERE YIL=2021 AND HESAPKOD='9000506'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Gelirlerden Ayrılan Paylar', HESAPKOD='9000508' WHERE YIL=2021 AND HESAPKOD='9000508'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sermaye Giderleri', HESAPKOD='90006' WHERE YIL=2021 AND HESAPKOD='90006'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Mamul Mal Alımları', HESAPKOD='9000601' WHERE YIL=2021 AND HESAPKOD='9000601'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Menkul Sermaye Üretim Giderleri', HESAPKOD='9000602' WHERE YIL=2021 AND HESAPKOD='9000602'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Gayri Maddi Hak Alımları', HESAPKOD='9000603' WHERE YIL=2021 AND HESAPKOD='9000603'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Gayrimenkul Alımları ve Kamulaştırması', HESAPKOD='9000604' WHERE YIL=2021 AND HESAPKOD='9000604'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Gayrimenkul Sermaye Üretim Giderleri', HESAPKOD='9000605' WHERE YIL=2021 AND HESAPKOD='9000605'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Menkul Malların Büyük Onarım Giderleri', HESAPKOD='9000606' WHERE YIL=2021 AND HESAPKOD='9000606'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Gayrimenkul Büyük Onarım Giderleri', HESAPKOD='9000607' WHERE YIL=2021 AND HESAPKOD='9000607'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Stok Alımları', HESAPKOD='9000608' WHERE YIL=2021 AND HESAPKOD='9000608'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Sermaye Giderleri', HESAPKOD='9000609' WHERE YIL=2021 AND HESAPKOD='9000609'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sermaye Transferleri', HESAPKOD='90007' WHERE YIL=2021 AND HESAPKOD='90007'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kurum, İşletme ve Hanehalkına Yapılan Sermaye Transferleri', HESAPKOD='9000701' WHERE YIL=2021 AND HESAPKOD='9000701'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yurtdışı Sermaye Transferleri', HESAPKOD='9000702' WHERE YIL=2021 AND HESAPKOD='9000702'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Borç Verme ', HESAPKOD='90008' WHERE YIL=2021 AND HESAPKOD='90008'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yurtiçi Borç Verme ', HESAPKOD='9000801' WHERE YIL=2021 AND HESAPKOD='9000801'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yurtdışı Borç Verme ', HESAPKOD='9000802' WHERE YIL=2021 AND HESAPKOD='9000802'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yedek Ödenekler', HESAPKOD='90009' WHERE YIL=2021 AND HESAPKOD='90009'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Personel Giderlerini Karşılama Ödeneği', HESAPKOD='9000901' WHERE YIL=2021 AND HESAPKOD='9000901'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kur Farklarını Karşılama Ödeneği', HESAPKOD='9000902' WHERE YIL=2021 AND HESAPKOD='9000902'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yatırımları Hızlandırma Ödeneği', HESAPKOD='9000903' WHERE YIL=2021 AND HESAPKOD='9000903'
DELETE FROM  MUHHESAPPLANI WHERE YIL=2021 AND HESAPKOD='9000904'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Doğal Afet Giderlerini Karşılama Ödeneği', HESAPKOD='9000905' WHERE YIL=2021 AND HESAPKOD='9000905'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yedek Ödenek', HESAPKOD='9000906' WHERE YIL=2021 AND HESAPKOD='9000906'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yeni Kurulacak Daire ve İdarelerin İhtiyaçlarını Karşılama Ödeneği', HESAPKOD='9000907' WHERE YIL=2021 AND HESAPKOD='9000907'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Mülteci ve Göçmen Giderleri Ödeneği', HESAPKOD='9000908' WHERE YIL=2021 AND HESAPKOD='9000908'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Yedek Ödenekler', HESAPKOD='9000909' WHERE YIL=2021 AND HESAPKOD='9000909'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='BÜTÇE ÖDENEKLERİ HESABI', HESAPKOD='901' WHERE YIL=2021 AND HESAPKOD='901'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Bütçe Başlangıç Ödeneği', HESAPKOD='90101' WHERE YIL=2021 AND HESAPKOD='90101'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Personel Giderleri', HESAPKOD='9010101' WHERE YIL=2021 AND HESAPKOD='9010101'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Memurlar', HESAPKOD='901010101' WHERE YIL=2021 AND HESAPKOD='901010101'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sözleşmeli Personel ', HESAPKOD='901010102' WHERE YIL=2021 AND HESAPKOD='901010102'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='İşçiler', HESAPKOD='901010103' WHERE YIL=2021 AND HESAPKOD='901010103'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Geçici Süreli Çalışanlar', HESAPKOD='901010104' WHERE YIL=2021 AND HESAPKOD='901010104'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Personel', HESAPKOD='901010105' WHERE YIL=2021 AND HESAPKOD='901010105'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Milletvekilleri', HESAPKOD='901010107' WHERE YIL=2021 AND HESAPKOD='901010107'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Cumhurbaşkanlığı Ödeneği', HESAPKOD='901010108' WHERE YIL=2021 AND HESAPKOD='901010108'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='İstihbarat Personeli', HESAPKOD='901010109' WHERE YIL=2021 AND HESAPKOD='901010109'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sosyal Güvenlik Kurumlarına Devlet Primi Giderleri', HESAPKOD='9010102' WHERE YIL=2021 AND HESAPKOD='9010102'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Memurlar', HESAPKOD='901010201' WHERE YIL=2021 AND HESAPKOD='901010201'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sözleşmeli Personel ', HESAPKOD='901010202' WHERE YIL=2021 AND HESAPKOD='901010202'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='İşçiler', HESAPKOD='901010203' WHERE YIL=2021 AND HESAPKOD='901010203'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Geçici Süreli Çalışanlar', HESAPKOD='901010204' WHERE YIL=2021 AND HESAPKOD='901010204'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Personel', HESAPKOD='901010205' WHERE YIL=2021 AND HESAPKOD='901010205'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Milletvekilleri', HESAPKOD='901010207' WHERE YIL=2021 AND HESAPKOD='901010207'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='İstihbarat Personeli', HESAPKOD='901010209' WHERE YIL=2021 AND HESAPKOD='901010209'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Mal ve Hizmet Alım Giderleri', HESAPKOD='9010103' WHERE YIL=2021 AND HESAPKOD='9010103'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Üretime Yönelik Mal ve Malzeme Alımları', HESAPKOD='901010301' WHERE YIL=2021 AND HESAPKOD='901010301'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Tüketime Yönelik Mal ve Malzeme Alımları', HESAPKOD='901010302' WHERE YIL=2021 AND HESAPKOD='901010302'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yolluklar', HESAPKOD='901010303' WHERE YIL=2021 AND HESAPKOD='901010303'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Görev Giderleri', HESAPKOD='901010304' WHERE YIL=2021 AND HESAPKOD='901010304'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Hizmet Alımları', HESAPKOD='901010305' WHERE YIL=2021 AND HESAPKOD='901010305'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Temsil ve Tanıtma Giderleri', HESAPKOD='901010306' WHERE YIL=2021 AND HESAPKOD='901010306'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Menkul Mal, Gayrimaddi Hak Alım, Bakım ve Onarım Giderleri', HESAPKOD='901010307' WHERE YIL=2021 AND HESAPKOD='901010307'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Gayrimenkul Mal Bakım ve Onarım Giderleri', HESAPKOD='901010308' WHERE YIL=2021 AND HESAPKOD='901010308'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Tedavi ve Cenaze Giderleri', HESAPKOD='901010309' WHERE YIL=2021 AND HESAPKOD='901010309'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Faiz Giderleri', HESAPKOD='9010104' WHERE YIL=2021 AND HESAPKOD='9010104'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kamu Kurumlarına Ödenen İç Borç Faiz Giderleri', HESAPKOD='901010401' WHERE YIL=2021 AND HESAPKOD='901010401'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer İç Borç Faiz Giderleri', HESAPKOD='901010402' WHERE YIL=2021 AND HESAPKOD='901010402'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Dış Borç Faiz Giderleri', HESAPKOD='901010403' WHERE YIL=2021 AND HESAPKOD='901010403'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='İskonto Giderleri', HESAPKOD='901010404' WHERE YIL=2021 AND HESAPKOD='901010404'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Para Piyasası Nakit İşlemleri Faiz Giderleri', HESAPKOD='901010405' WHERE YIL=2021 AND HESAPKOD='901010405'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Türev Ürün Giderleri', HESAPKOD='901010406' WHERE YIL=2021 AND HESAPKOD='901010406'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kira Sertifikası Giderleri', HESAPKOD='901010407' WHERE YIL=2021 AND HESAPKOD='901010407'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Cari Transferler', HESAPKOD='9010105' WHERE YIL=2021 AND HESAPKOD='9010105'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Görevlendirme Giderleri', HESAPKOD='901010501' WHERE YIL=2021 AND HESAPKOD='901010501'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Hazine Yardımları', HESAPKOD='901010502' WHERE YIL=2021 AND HESAPKOD='901010502'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kar Amacı Gütmeyen Kuruluşlara Yapılan Transferler', HESAPKOD='901010503' WHERE YIL=2021 AND HESAPKOD='901010503'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Hane Halkı ve İşletmelere Yapılan Transferler', HESAPKOD='901010504' WHERE YIL=2021 AND HESAPKOD='901010504'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Devlet Sosyal Güvenlik Kurumlarından Hane Halkına Yapılan Fayda Ödemeleri', HESAPKOD='901010505' WHERE YIL=2021 AND HESAPKOD='901010505'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yurt Dışına Yapılan Transferler ', HESAPKOD='901010506' WHERE YIL=2021 AND HESAPKOD='901010506'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Gelirlerden Ayrılan Paylar', HESAPKOD='901010508' WHERE YIL=2021 AND HESAPKOD='901010508'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sermaye Giderleri', HESAPKOD='9010106' WHERE YIL=2021 AND HESAPKOD='9010106'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Mamul Mal Alımları', HESAPKOD='901010601' WHERE YIL=2021 AND HESAPKOD='901010601'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Menkul Sermaye Üretim Giderleri', HESAPKOD='901010602' WHERE YIL=2021 AND HESAPKOD='901010602'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Gayri Maddi Hak Alımları', HESAPKOD='901010603' WHERE YIL=2021 AND HESAPKOD='901010603'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Gayrimenkul Alımları ve Kamulaştırması', HESAPKOD='901010604' WHERE YIL=2021 AND HESAPKOD='901010604'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Gayrimenkul Sermaye Üretim Giderleri', HESAPKOD='901010605' WHERE YIL=2021 AND HESAPKOD='901010605'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Menkul Malların Büyük Onarım Giderleri', HESAPKOD='901010606' WHERE YIL=2021 AND HESAPKOD='901010606'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Gayrimenkul Büyük Onarım Giderleri', HESAPKOD='901010607' WHERE YIL=2021 AND HESAPKOD='901010607'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Stok Alımları', HESAPKOD='901010608' WHERE YIL=2021 AND HESAPKOD='901010608'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Sermaye Giderleri', HESAPKOD='901010609' WHERE YIL=2021 AND HESAPKOD='901010609'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sermaye Transferleri', HESAPKOD='9010107' WHERE YIL=2021 AND HESAPKOD='9010107'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kurum, İşletme ve Hanehalkına Yapılan Sermaye Transferleri', HESAPKOD='901010701' WHERE YIL=2021 AND HESAPKOD='901010701'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yurtdışı Sermaye Transferleri', HESAPKOD='901010702' WHERE YIL=2021 AND HESAPKOD='901010702'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Borç Verme ', HESAPKOD='9010108' WHERE YIL=2021 AND HESAPKOD='9010108'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yurtiçi Borç Verme ', HESAPKOD='901010801' WHERE YIL=2021 AND HESAPKOD='901010801'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yurtdışı Borç Verme ', HESAPKOD='901010802' WHERE YIL=2021 AND HESAPKOD='901010802'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yedek Ödenekler', HESAPKOD='9010109' WHERE YIL=2021 AND HESAPKOD='9010109'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Personel Giderlerini Karşılama Ödeneği', HESAPKOD='901010901' WHERE YIL=2021 AND HESAPKOD='901010901'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kur Farklarını Karşılama Ödeneği', HESAPKOD='901010902' WHERE YIL=2021 AND HESAPKOD='901010902'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yatırımları Hızlandırma Ödeneği', HESAPKOD='901010903' WHERE YIL=2021 AND HESAPKOD='901010903'
DELETE FROM  MUHHESAPPLANI WHERE YIL=2021 AND HESAPKOD='901010904'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Doğal Afet Giderlerini Karşılama Ödeneği', HESAPKOD='901010905' WHERE YIL=2021 AND HESAPKOD='901010905'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yedek Ödenek', HESAPKOD='901010906' WHERE YIL=2021 AND HESAPKOD='901010906'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yeni Kurulacak Daire ve İdarelerin İhtiyaçlarını Karşılama Ödeneği', HESAPKOD='901010907' WHERE YIL=2021 AND HESAPKOD='901010907'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Mülteci ve Göçmen Giderleri Ödeneği', HESAPKOD='901010908' WHERE YIL=2021 AND HESAPKOD='901010908'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Yedek Ödenekler', HESAPKOD='901010909' WHERE YIL=2021 AND HESAPKOD='901010909'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Bütçe İle Yapılan Kesintiler', HESAPKOD='90102' WHERE YIL=2021 AND HESAPKOD='90102'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Personel Giderleri', HESAPKOD='9010201' WHERE YIL=2021 AND HESAPKOD='9010201'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Memurlar', HESAPKOD='901020101' WHERE YIL=2021 AND HESAPKOD='901020101'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sözleşmeli Personel ', HESAPKOD='901020102' WHERE YIL=2021 AND HESAPKOD='901020102'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='İşçiler', HESAPKOD='901020103' WHERE YIL=2021 AND HESAPKOD='901020103'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Geçici Süreli Çalışanlar', HESAPKOD='901020104' WHERE YIL=2021 AND HESAPKOD='901020104'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Personel', HESAPKOD='901020105' WHERE YIL=2021 AND HESAPKOD='901020105'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Milletvekilleri', HESAPKOD='901020107' WHERE YIL=2021 AND HESAPKOD='901020107'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Cumhurbaşkanlığı Ödeneği', HESAPKOD='901020108' WHERE YIL=2021 AND HESAPKOD='901020108'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='İstihbarat Personeli', HESAPKOD='901020109' WHERE YIL=2021 AND HESAPKOD='901020109'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sosyal Güvenlik Kurumlarına Devlet Primi Giderleri', HESAPKOD='9010202' WHERE YIL=2021 AND HESAPKOD='9010202'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Memurlar', HESAPKOD='901020201' WHERE YIL=2021 AND HESAPKOD='901020201'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sözleşmeli Personel ', HESAPKOD='901020202' WHERE YIL=2021 AND HESAPKOD='901020202'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='İşçiler', HESAPKOD='901020203' WHERE YIL=2021 AND HESAPKOD='901020203'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Geçici Süreli Çalışanlar', HESAPKOD='901020204' WHERE YIL=2021 AND HESAPKOD='901020204'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Personel', HESAPKOD='901020205' WHERE YIL=2021 AND HESAPKOD='901020205'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Milletvekilleri', HESAPKOD='901020207' WHERE YIL=2021 AND HESAPKOD='901020207'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='İstihbarat Personeli', HESAPKOD='901020209' WHERE YIL=2021 AND HESAPKOD='901020209'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Mal ve Hizmet Alım Giderleri', HESAPKOD='9010203' WHERE YIL=2021 AND HESAPKOD='9010203'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Üretime Yönelik Mal ve Malzeme Alımları', HESAPKOD='901020301' WHERE YIL=2021 AND HESAPKOD='901020301'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Tüketime Yönelik Mal ve Malzeme Alımları', HESAPKOD='901020302' WHERE YIL=2021 AND HESAPKOD='901020302'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yolluklar', HESAPKOD='901020303' WHERE YIL=2021 AND HESAPKOD='901020303'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Görev Giderleri', HESAPKOD='901020304' WHERE YIL=2021 AND HESAPKOD='901020304'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Hizmet Alımları', HESAPKOD='901020305' WHERE YIL=2021 AND HESAPKOD='901020305'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Temsil ve Tanıtma Giderleri', HESAPKOD='901020306' WHERE YIL=2021 AND HESAPKOD='901020306'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Menkul Mal, Gayrimaddi Hak Alım, Bakım ve Onarım Giderleri', HESAPKOD='901020307' WHERE YIL=2021 AND HESAPKOD='901020307'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Gayrimenkul Mal Bakım ve Onarım Giderleri', HESAPKOD='901020308' WHERE YIL=2021 AND HESAPKOD='901020308'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Tedavi ve Cenaze Giderleri', HESAPKOD='901020309' WHERE YIL=2021 AND HESAPKOD='901020309'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Faiz Giderleri', HESAPKOD='9010204' WHERE YIL=2021 AND HESAPKOD='9010204'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kamu Kurumlarına Ödenen İç Borç Faiz Giderleri', HESAPKOD='901020401' WHERE YIL=2021 AND HESAPKOD='901020401'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer İç Borç Faiz Giderleri', HESAPKOD='901020402' WHERE YIL=2021 AND HESAPKOD='901020402'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Dış Borç Faiz Giderleri', HESAPKOD='901020403' WHERE YIL=2021 AND HESAPKOD='901020403'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='İskonto Giderleri', HESAPKOD='901020404' WHERE YIL=2021 AND HESAPKOD='901020404'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Para Piyasası Nakit İşlemleri Faiz Giderleri', HESAPKOD='901020405' WHERE YIL=2021 AND HESAPKOD='901020405'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Türev Ürün Giderleri', HESAPKOD='901020406' WHERE YIL=2021 AND HESAPKOD='901020406'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kira Sertifikası Giderleri', HESAPKOD='901020407' WHERE YIL=2021 AND HESAPKOD='901020407'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Cari Transferler', HESAPKOD='9010205' WHERE YIL=2021 AND HESAPKOD='9010205'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Görevlendirme Giderleri', HESAPKOD='901020501' WHERE YIL=2021 AND HESAPKOD='901020501'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Hazine Yardımları', HESAPKOD='901020502' WHERE YIL=2021 AND HESAPKOD='901020502'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kar Amacı Gütmeyen Kuruluşlara Yapılan Transferler', HESAPKOD='901020503' WHERE YIL=2021 AND HESAPKOD='901020503'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Hane Halkı ve İşletmelere Yapılan Transferler', HESAPKOD='901020504' WHERE YIL=2021 AND HESAPKOD='901020504'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Devlet Sosyal Güvenlik Kurumlarından Hane Halkına Yapılan Fayda Ödemeleri', HESAPKOD='901020505' WHERE YIL=2021 AND HESAPKOD='901020505'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yurt Dışına Yapılan Transferler ', HESAPKOD='901020506' WHERE YIL=2021 AND HESAPKOD='901020506'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Gelirlerden Ayrılan Paylar', HESAPKOD='901020508' WHERE YIL=2021 AND HESAPKOD='901020508'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sermaye Giderleri', HESAPKOD='9010206' WHERE YIL=2021 AND HESAPKOD='9010206'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Mamul Mal Alımları', HESAPKOD='901020601' WHERE YIL=2021 AND HESAPKOD='901020601'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Menkul Sermaye Üretim Giderleri', HESAPKOD='901020602' WHERE YIL=2021 AND HESAPKOD='901020602'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Gayri Maddi Hak Alımları', HESAPKOD='901020603' WHERE YIL=2021 AND HESAPKOD='901020603'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Gayrimenkul Alımları ve Kamulaştırması', HESAPKOD='901020604' WHERE YIL=2021 AND HESAPKOD='901020604'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Gayrimenkul Sermaye Üretim Giderleri', HESAPKOD='901020605' WHERE YIL=2021 AND HESAPKOD='901020605'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Menkul Malların Büyük Onarım Giderleri', HESAPKOD='901020606' WHERE YIL=2021 AND HESAPKOD='901020606'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Gayrimenkul Büyük Onarım Giderleri', HESAPKOD='901020607' WHERE YIL=2021 AND HESAPKOD='901020607'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Stok Alımları', HESAPKOD='901020608' WHERE YIL=2021 AND HESAPKOD='901020608'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Sermaye Giderleri', HESAPKOD='901020609' WHERE YIL=2021 AND HESAPKOD='901020609'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sermaye Transferleri', HESAPKOD='9010207' WHERE YIL=2021 AND HESAPKOD='9010207'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kurum, İşletme ve Hanehalkına Yapılan Sermaye Transferleri', HESAPKOD='901020701' WHERE YIL=2021 AND HESAPKOD='901020701'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yurtdışı Sermaye Transferleri', HESAPKOD='901020702' WHERE YIL=2021 AND HESAPKOD='901020702'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Borç Verme ', HESAPKOD='9010208' WHERE YIL=2021 AND HESAPKOD='9010208'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yurtiçi Borç Verme ', HESAPKOD='901020801' WHERE YIL=2021 AND HESAPKOD='901020801'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yurtdışı Borç Verme ', HESAPKOD='901020802' WHERE YIL=2021 AND HESAPKOD='901020802'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yedek Ödenekler', HESAPKOD='9010209' WHERE YIL=2021 AND HESAPKOD='9010209'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Personel Giderlerini Karşılama Ödeneği', HESAPKOD='901020901' WHERE YIL=2021 AND HESAPKOD='901020901'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kur Farklarını Karşılama Ödeneği', HESAPKOD='901020902' WHERE YIL=2021 AND HESAPKOD='901020902'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yatırımları Hızlandırma Ödeneği', HESAPKOD='901020903' WHERE YIL=2021 AND HESAPKOD='901020903'
DELETE FROM  MUHHESAPPLANI WHERE YIL=2021 AND HESAPKOD='901020904'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Doğal Afet Giderlerini Karşılama Ödeneği', HESAPKOD='901020905' WHERE YIL=2021 AND HESAPKOD='901020905'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yedek Ödenek', HESAPKOD='901020906' WHERE YIL=2021 AND HESAPKOD='901020906'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yeni Kurulacak Daire ve İdarelerin İhtiyaçlarını Karşılama Ödeneği', HESAPKOD='901020907' WHERE YIL=2021 AND HESAPKOD='901020907'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Mülteci ve Göçmen Giderleri Ödeneği', HESAPKOD='901020908' WHERE YIL=2021 AND HESAPKOD='901020908'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Yedek Ödenekler', HESAPKOD='901020909' WHERE YIL=2021 AND HESAPKOD='901020909'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Kanunlar Gereğince Geçen Yıldan Devreden Ödenekler', HESAPKOD='90103' WHERE YIL=2021 AND HESAPKOD='90103'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Personel Giderleri', HESAPKOD='9010301' WHERE YIL=2021 AND HESAPKOD='9010301'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Memurlar', HESAPKOD='901030101' WHERE YIL=2021 AND HESAPKOD='901030101'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sosyal Güvenlik Kurumlarına Devlet Primi Giderleri', HESAPKOD='9010302' WHERE YIL=2021 AND HESAPKOD='9010302'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Geçici Süreli Çalışanlar', HESAPKOD='901030204' WHERE YIL=2021 AND HESAPKOD='901030204'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Mal ve Hizmet Alım Giderleri', HESAPKOD='9010303' WHERE YIL=2021 AND HESAPKOD='9010303'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Üretime Yönelik Mal ve Malzeme Alımları', HESAPKOD='901030301' WHERE YIL=2021 AND HESAPKOD='901030301'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Tüketime Yönelik Mal ve Malzeme Alımları', HESAPKOD='901030302' WHERE YIL=2021 AND HESAPKOD='901030302'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yolluklar', HESAPKOD='901030303' WHERE YIL=2021 AND HESAPKOD='901030303'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Görev Giderleri', HESAPKOD='901030304' WHERE YIL=2021 AND HESAPKOD='901030304'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Hizmet Alımları', HESAPKOD='901030305' WHERE YIL=2021 AND HESAPKOD='901030305'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Temsil ve Tanıtma Giderleri', HESAPKOD='901030306' WHERE YIL=2021 AND HESAPKOD='901030306'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Menkul Mal, Gayrimaddi Hak Alım, Bakım ve Onarım Giderleri', HESAPKOD='901030307' WHERE YIL=2021 AND HESAPKOD='901030307'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Gayrimenkul Mal Bakım ve Onarım Giderleri', HESAPKOD='901030308' WHERE YIL=2021 AND HESAPKOD='901030308'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Tedavi ve Cenaze Giderleri', HESAPKOD='901030309' WHERE YIL=2021 AND HESAPKOD='901030309'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Cari Transferler', HESAPKOD='9010305' WHERE YIL=2021 AND HESAPKOD='9010305'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kar Amacı Gütmeyen Kuruluşlara Yapılan Transferler', HESAPKOD='901030503' WHERE YIL=2021 AND HESAPKOD='901030503'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Hane Halkı ve İşletmelere Yapılan Transferler', HESAPKOD='901030504' WHERE YIL=2021 AND HESAPKOD='901030504'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yurt Dışına Yapılan Transferler ', HESAPKOD='901030506' WHERE YIL=2021 AND HESAPKOD='901030506'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sermaye Giderleri', HESAPKOD='9010306' WHERE YIL=2021 AND HESAPKOD='9010306'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Mamul Mal Alımları', HESAPKOD='901030601' WHERE YIL=2021 AND HESAPKOD='901030601'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Menkul Sermaye Üretim Giderleri', HESAPKOD='901030602' WHERE YIL=2021 AND HESAPKOD='901030602'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Gayri Maddi Hak Alımları', HESAPKOD='901030603' WHERE YIL=2021 AND HESAPKOD='901030603'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Gayrimenkul Alımları ve Kamulaştırması', HESAPKOD='901030604' WHERE YIL=2021 AND HESAPKOD='901030604'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Gayrimenkul Sermaye Üretim Giderleri', HESAPKOD='901030605' WHERE YIL=2021 AND HESAPKOD='901030605'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Menkul Malların Büyük Onarım Giderleri', HESAPKOD='901030606' WHERE YIL=2021 AND HESAPKOD='901030606'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Gayrimenkul Büyük Onarım Giderleri', HESAPKOD='901030607' WHERE YIL=2021 AND HESAPKOD='901030607'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Stok Alımları', HESAPKOD='901030608' WHERE YIL=2021 AND HESAPKOD='901030608'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Sermaye Giderleri', HESAPKOD='901030609' WHERE YIL=2021 AND HESAPKOD='901030609'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sermaye Transferleri', HESAPKOD='9010307' WHERE YIL=2021 AND HESAPKOD='9010307'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kurum, İşletme ve Hanehalkına Yapılan Sermaye Transferleri', HESAPKOD='901030701' WHERE YIL=2021 AND HESAPKOD='901030701'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yurtdışı Sermaye Transferleri', HESAPKOD='901030702' WHERE YIL=2021 AND HESAPKOD='901030702'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Borç Verme ', HESAPKOD='9010308' WHERE YIL=2021 AND HESAPKOD='9010308'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yurtiçi Borç Verme ', HESAPKOD='901030801' WHERE YIL=2021 AND HESAPKOD='901030801'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yurtdışı Borç Verme ', HESAPKOD='901030802' WHERE YIL=2021 AND HESAPKOD='901030802'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='5018 Sayılı Kanunun 35 inci Maddesi Gereğince Geçen Yıldan Devreden Ödenek/Akreditif Artığı', HESAPKOD='90104' WHERE YIL=2021 AND HESAPKOD='90104'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Personel Giderleri', HESAPKOD='9010401' WHERE YIL=2021 AND HESAPKOD='9010401'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Memurlar', HESAPKOD='901040101' WHERE YIL=2021 AND HESAPKOD='901040101'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sözleşmeli Personel ', HESAPKOD='901040102' WHERE YIL=2021 AND HESAPKOD='901040102'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='İşçiler', HESAPKOD='901040103' WHERE YIL=2021 AND HESAPKOD='901040103'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Geçici Süreli Çalışanlar', HESAPKOD='901040104' WHERE YIL=2021 AND HESAPKOD='901040104'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Personel', HESAPKOD='901040105' WHERE YIL=2021 AND HESAPKOD='901040105'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sosyal Güvenlik Kurumlarına Devlet Primi Giderleri', HESAPKOD='9010402' WHERE YIL=2021 AND HESAPKOD='9010402'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sözleşmeli Personel ', HESAPKOD='901040202' WHERE YIL=2021 AND HESAPKOD='901040202'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='İşçiler', HESAPKOD='901040203' WHERE YIL=2021 AND HESAPKOD='901040203'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Geçici Süreli Çalışanlar', HESAPKOD='901040204' WHERE YIL=2021 AND HESAPKOD='901040204'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Personel', HESAPKOD='901040205' WHERE YIL=2021 AND HESAPKOD='901040205'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Mal ve Hizmet Alım Giderleri', HESAPKOD='9010403' WHERE YIL=2021 AND HESAPKOD='9010403'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Üretime Yönelik Mal ve Malzeme Alımları', HESAPKOD='901040301' WHERE YIL=2021 AND HESAPKOD='901040301'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Tüketime Yönelik Mal ve Malzeme Alımları', HESAPKOD='901040302' WHERE YIL=2021 AND HESAPKOD='901040302'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yolluklar', HESAPKOD='901040303' WHERE YIL=2021 AND HESAPKOD='901040303'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Görev Giderleri', HESAPKOD='901040304' WHERE YIL=2021 AND HESAPKOD='901040304'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Hizmet Alımları', HESAPKOD='901040305' WHERE YIL=2021 AND HESAPKOD='901040305'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Temsil ve Tanıtma Giderleri', HESAPKOD='901040306' WHERE YIL=2021 AND HESAPKOD='901040306'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Menkul Mal, Gayrimaddi Hak Alım, Bakım ve Onarım Giderleri', HESAPKOD='901040307' WHERE YIL=2021 AND HESAPKOD='901040307'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Gayrimenkul Mal Bakım ve Onarım Giderleri', HESAPKOD='901040308' WHERE YIL=2021 AND HESAPKOD='901040308'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Tedavi ve Cenaze Giderleri', HESAPKOD='901040309' WHERE YIL=2021 AND HESAPKOD='901040309'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sermaye Giderleri', HESAPKOD='9010406' WHERE YIL=2021 AND HESAPKOD='9010406'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Mamul Mal Alımları', HESAPKOD='901040601' WHERE YIL=2021 AND HESAPKOD='901040601'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Menkul Sermaye Üretim Giderleri', HESAPKOD='901040602' WHERE YIL=2021 AND HESAPKOD='901040602'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Gayri Maddi Hak Alımları', HESAPKOD='901040603' WHERE YIL=2021 AND HESAPKOD='901040603'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Gayrimenkul Alımları ve Kamulaştırması', HESAPKOD='901040604' WHERE YIL=2021 AND HESAPKOD='901040604'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Gayrimenkul Sermaye Üretim Giderleri', HESAPKOD='901040605' WHERE YIL=2021 AND HESAPKOD='901040605'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Menkul Malların Büyük Onarım Giderleri', HESAPKOD='901040606' WHERE YIL=2021 AND HESAPKOD='901040606'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Gayrimenkul Büyük Onarım Giderleri', HESAPKOD='901040607' WHERE YIL=2021 AND HESAPKOD='901040607'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Stok Alımları', HESAPKOD='901040608' WHERE YIL=2021 AND HESAPKOD='901040608'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Sermaye Giderleri', HESAPKOD='901040609' WHERE YIL=2021 AND HESAPKOD='901040609'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='5018 Sayılı Kanunun 35 inci Maddesi Gereğince Geçen Yıldan Devreden Ödenek/Taahhüt Artığı', HESAPKOD='90105' WHERE YIL=2021 AND HESAPKOD='90105'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Mal ve Hizmet Alım Giderleri', HESAPKOD='9010503' WHERE YIL=2021 AND HESAPKOD='9010503'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Üretime Yönelik Mal ve Malzeme Alımları', HESAPKOD='901050301' WHERE YIL=2021 AND HESAPKOD='901050301'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Tüketime Yönelik Mal ve Malzeme Alımları', HESAPKOD='901050302' WHERE YIL=2021 AND HESAPKOD='901050302'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yolluklar', HESAPKOD='901050303' WHERE YIL=2021 AND HESAPKOD='901050303'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Görev Giderleri', HESAPKOD='901050304' WHERE YIL=2021 AND HESAPKOD='901050304'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Hizmet Alımları', HESAPKOD='901050305' WHERE YIL=2021 AND HESAPKOD='901050305'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Temsil ve Tanıtma Giderleri', HESAPKOD='901050306' WHERE YIL=2021 AND HESAPKOD='901050306'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Menkul Mal, Gayrimaddi Hak Alım, Bakım ve Onarım Giderleri', HESAPKOD='901050307' WHERE YIL=2021 AND HESAPKOD='901050307'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Gayrimenkul Mal Bakım ve Onarım Giderleri', HESAPKOD='901050308' WHERE YIL=2021 AND HESAPKOD='901050308'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Tedavi ve Cenaze Giderleri', HESAPKOD='901050309' WHERE YIL=2021 AND HESAPKOD='901050309'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sermaye Giderleri', HESAPKOD='9010506' WHERE YIL=2021 AND HESAPKOD='9010506'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Mamul Mal Alımları', HESAPKOD='901050601' WHERE YIL=2021 AND HESAPKOD='901050601'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Menkul Sermaye Üretim Giderleri', HESAPKOD='901050602' WHERE YIL=2021 AND HESAPKOD='901050602'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Gayri Maddi Hak Alımları', HESAPKOD='901050603' WHERE YIL=2021 AND HESAPKOD='901050603'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Gayrimenkul Alımları ve Kamulaştırması', HESAPKOD='901050604' WHERE YIL=2021 AND HESAPKOD='901050604'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Gayrimenkul Sermaye Üretim Giderleri', HESAPKOD='901050605' WHERE YIL=2021 AND HESAPKOD='901050605'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Menkul Malların Büyük Onarım Giderleri', HESAPKOD='901050606' WHERE YIL=2021 AND HESAPKOD='901050606'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Gayrimenkul Büyük Onarım Giderleri', HESAPKOD='901050607' WHERE YIL=2021 AND HESAPKOD='901050607'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Stok Alımları', HESAPKOD='901050608' WHERE YIL=2021 AND HESAPKOD='901050608'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Sermaye Giderleri', HESAPKOD='901050609' WHERE YIL=2021 AND HESAPKOD='901050609'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='4749 Sayılı Kanuna Göre Geçen Yıldan Devreden Dış Proje Kredisi Akreditif ve Avans Artığı ', HESAPKOD='90106' WHERE YIL=2021 AND HESAPKOD='90106'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Mal ve Hizmet Alım Giderleri', HESAPKOD='9010603' WHERE YIL=2021 AND HESAPKOD='9010603'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Üretime Yönelik Mal ve Malzeme Alımları', HESAPKOD='901060301' WHERE YIL=2021 AND HESAPKOD='901060301'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Tüketime Yönelik Mal ve Malzeme Alımları', HESAPKOD='901060302' WHERE YIL=2021 AND HESAPKOD='901060302'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yolluklar', HESAPKOD='901060303' WHERE YIL=2021 AND HESAPKOD='901060303'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Görev Giderleri', HESAPKOD='901060304' WHERE YIL=2021 AND HESAPKOD='901060304'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Hizmet Alımları', HESAPKOD='901060305' WHERE YIL=2021 AND HESAPKOD='901060305'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Temsil ve Tanıtma Giderleri', HESAPKOD='901060306' WHERE YIL=2021 AND HESAPKOD='901060306'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Menkul Mal, Gayrimaddi Hak Alım, Bakım ve Onarım Giderleri', HESAPKOD='901060307' WHERE YIL=2021 AND HESAPKOD='901060307'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Gayrimenkul Mal Bakım ve Onarım Giderleri', HESAPKOD='901060308' WHERE YIL=2021 AND HESAPKOD='901060308'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Tedavi ve Cenaze Giderleri', HESAPKOD='901060309' WHERE YIL=2021 AND HESAPKOD='901060309'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sermaye Giderleri', HESAPKOD='9010606' WHERE YIL=2021 AND HESAPKOD='9010606'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Mamul Mal Alımları', HESAPKOD='901060601' WHERE YIL=2021 AND HESAPKOD='901060601'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Menkul Sermaye Üretim Giderleri', HESAPKOD='901060602' WHERE YIL=2021 AND HESAPKOD='901060602'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Gayri Maddi Hak Alımları', HESAPKOD='901060603' WHERE YIL=2021 AND HESAPKOD='901060603'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Gayrimenkul Alımları ve Kamulaştırması', HESAPKOD='901060604' WHERE YIL=2021 AND HESAPKOD='901060604'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Gayrimenkul Sermaye Üretim Giderleri', HESAPKOD='901060605' WHERE YIL=2021 AND HESAPKOD='901060605'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Menkul Malların Büyük Onarım Giderleri', HESAPKOD='901060606' WHERE YIL=2021 AND HESAPKOD='901060606'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Gayrimenkul Büyük Onarım Giderleri', HESAPKOD='901060607' WHERE YIL=2021 AND HESAPKOD='901060607'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Stok Alımları', HESAPKOD='901060608' WHERE YIL=2021 AND HESAPKOD='901060608'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Sermaye Giderleri', HESAPKOD='901060609' WHERE YIL=2021 AND HESAPKOD='901060609'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='5018 Sayılı Kanunun 39 uncu Maddesine Gereğince Geçen Yıldan Devreden Ödenek', HESAPKOD='90107' WHERE YIL=2021 AND HESAPKOD='90107'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Personel Giderleri', HESAPKOD='9010701' WHERE YIL=2021 AND HESAPKOD='9010701'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Memurlar', HESAPKOD='901070101' WHERE YIL=2021 AND HESAPKOD='901070101'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sözleşmeli Personel ', HESAPKOD='901070102' WHERE YIL=2021 AND HESAPKOD='901070102'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Geçici Süreli Çalışanlar', HESAPKOD='901070104' WHERE YIL=2021 AND HESAPKOD='901070104'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Mal ve Hizmet Alım Giderleri', HESAPKOD='9010703' WHERE YIL=2021 AND HESAPKOD='9010703'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Üretime Yönelik Mal ve Malzeme Alımları', HESAPKOD='901070301' WHERE YIL=2021 AND HESAPKOD='901070301'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Tüketime Yönelik Mal ve Malzeme Alımları', HESAPKOD='901070302' WHERE YIL=2021 AND HESAPKOD='901070302'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yolluklar', HESAPKOD='901070303' WHERE YIL=2021 AND HESAPKOD='901070303'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Görev Giderleri', HESAPKOD='901070304' WHERE YIL=2021 AND HESAPKOD='901070304'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Hizmet Alımları', HESAPKOD='901070305' WHERE YIL=2021 AND HESAPKOD='901070305'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Temsil ve Tanıtma Giderleri', HESAPKOD='901070306' WHERE YIL=2021 AND HESAPKOD='901070306'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Menkul Mal, Gayrimaddi Hak Alım, Bakım ve Onarım Giderleri', HESAPKOD='901070307' WHERE YIL=2021 AND HESAPKOD='901070307'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Gayrimenkul Mal Bakım ve Onarım Giderleri', HESAPKOD='901070308' WHERE YIL=2021 AND HESAPKOD='901070308'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Tedavi ve Cenaze Giderleri', HESAPKOD='901070309' WHERE YIL=2021 AND HESAPKOD='901070309'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Cari Transferler', HESAPKOD='9010705' WHERE YIL=2021 AND HESAPKOD='9010705'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kar Amacı Gütmeyen Kuruluşlara Yapılan Transferler', HESAPKOD='901070503' WHERE YIL=2021 AND HESAPKOD='901070503'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Hane Halkı ve İşletmelere Yapılan Transferler', HESAPKOD='901070504' WHERE YIL=2021 AND HESAPKOD='901070504'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yurt Dışına Yapılan Transferler ', HESAPKOD='901070506' WHERE YIL=2021 AND HESAPKOD='901070506'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sermaye Giderleri', HESAPKOD='9010706' WHERE YIL=2021 AND HESAPKOD='9010706'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Mamul Mal Alımları', HESAPKOD='901070601' WHERE YIL=2021 AND HESAPKOD='901070601'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Menkul Sermaye Üretim Giderleri', HESAPKOD='901070602' WHERE YIL=2021 AND HESAPKOD='901070602'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Gayri Maddi Hak Alımları', HESAPKOD='901070603' WHERE YIL=2021 AND HESAPKOD='901070603'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Gayrimenkul Alımları ve Kamulaştırması', HESAPKOD='901070604' WHERE YIL=2021 AND HESAPKOD='901070604'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Gayrimenkul Sermaye Üretim Giderleri', HESAPKOD='901070605' WHERE YIL=2021 AND HESAPKOD='901070605'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Menkul Malların Büyük Onarım Giderleri', HESAPKOD='901070606' WHERE YIL=2021 AND HESAPKOD='901070606'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Gayrimenkul Büyük Onarım Giderleri', HESAPKOD='901070607' WHERE YIL=2021 AND HESAPKOD='901070607'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Stok Alımları', HESAPKOD='901070608' WHERE YIL=2021 AND HESAPKOD='901070608'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Sermaye Giderleri', HESAPKOD='901070609' WHERE YIL=2021 AND HESAPKOD='901070609'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sermaye Transferleri', HESAPKOD='9010707' WHERE YIL=2021 AND HESAPKOD='9010707'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kurum, İşletme ve Hanehalkına Yapılan Sermaye Transferleri', HESAPKOD='901070701' WHERE YIL=2021 AND HESAPKOD='901070701'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Borç Verme ', HESAPKOD='9010708' WHERE YIL=2021 AND HESAPKOD='9010708'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yurtiçi Borç Verme ', HESAPKOD='901070801' WHERE YIL=2021 AND HESAPKOD='901070801'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='5018 Sayılı Kanunun 40 ıncı Maddesi Gereğince Geçen Yıldan Devreden Ödenek', HESAPKOD='90108' WHERE YIL=2021 AND HESAPKOD='90108'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Personel Giderleri', HESAPKOD='9010801' WHERE YIL=2021 AND HESAPKOD='9010801'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Memurlar', HESAPKOD='901080101' WHERE YIL=2021 AND HESAPKOD='901080101'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='İşçiler', HESAPKOD='901080103' WHERE YIL=2021 AND HESAPKOD='901080103'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Geçici Süreli Çalışanlar', HESAPKOD='901080104' WHERE YIL=2021 AND HESAPKOD='901080104'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sosyal Güvenlik Kurumlarına Devlet Primi Giderleri', HESAPKOD='9010802' WHERE YIL=2021 AND HESAPKOD='9010802'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='İşçiler', HESAPKOD='901080203' WHERE YIL=2021 AND HESAPKOD='901080203'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Geçici Süreli Çalışanlar', HESAPKOD='901080204' WHERE YIL=2021 AND HESAPKOD='901080204'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Mal ve Hizmet Alım Giderleri', HESAPKOD='9010803' WHERE YIL=2021 AND HESAPKOD='9010803'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Üretime Yönelik Mal ve Malzeme Alımları', HESAPKOD='901080301' WHERE YIL=2021 AND HESAPKOD='901080301'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Tüketime Yönelik Mal ve Malzeme Alımları', HESAPKOD='901080302' WHERE YIL=2021 AND HESAPKOD='901080302'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yolluklar', HESAPKOD='901080303' WHERE YIL=2021 AND HESAPKOD='901080303'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Görev Giderleri', HESAPKOD='901080304' WHERE YIL=2021 AND HESAPKOD='901080304'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Hizmet Alımları', HESAPKOD='901080305' WHERE YIL=2021 AND HESAPKOD='901080305'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Temsil ve Tanıtma Giderleri', HESAPKOD='901080306' WHERE YIL=2021 AND HESAPKOD='901080306'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Menkul Mal, Gayrimaddi Hak Alım, Bakım ve Onarım Giderleri', HESAPKOD='901080307' WHERE YIL=2021 AND HESAPKOD='901080307'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Gayrimenkul Mal Bakım ve Onarım Giderleri', HESAPKOD='901080308' WHERE YIL=2021 AND HESAPKOD='901080308'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Tedavi ve Cenaze Giderleri', HESAPKOD='901080309' WHERE YIL=2021 AND HESAPKOD='901080309'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Cari Transferler', HESAPKOD='9010805' WHERE YIL=2021 AND HESAPKOD='9010805'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kar Amacı Gütmeyen Kuruluşlara Yapılan Transferler', HESAPKOD='901080503' WHERE YIL=2021 AND HESAPKOD='901080503'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Hane Halkı ve İşletmelere Yapılan Transferler', HESAPKOD='901080504' WHERE YIL=2021 AND HESAPKOD='901080504'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yurt Dışına Yapılan Transferler ', HESAPKOD='901080506' WHERE YIL=2021 AND HESAPKOD='901080506'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sermaye Giderleri', HESAPKOD='9010806' WHERE YIL=2021 AND HESAPKOD='9010806'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Mamul Mal Alımları', HESAPKOD='901080601' WHERE YIL=2021 AND HESAPKOD='901080601'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Menkul Sermaye Üretim Giderleri', HESAPKOD='901080602' WHERE YIL=2021 AND HESAPKOD='901080602'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Gayri Maddi Hak Alımları', HESAPKOD='901080603' WHERE YIL=2021 AND HESAPKOD='901080603'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Gayrimenkul Alımları ve Kamulaştırması', HESAPKOD='901080604' WHERE YIL=2021 AND HESAPKOD='901080604'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Gayrimenkul Sermaye Üretim Giderleri', HESAPKOD='901080605' WHERE YIL=2021 AND HESAPKOD='901080605'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Menkul Malların Büyük Onarım Giderleri', HESAPKOD='901080606' WHERE YIL=2021 AND HESAPKOD='901080606'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Gayrimenkul Büyük Onarım Giderleri', HESAPKOD='901080607' WHERE YIL=2021 AND HESAPKOD='901080607'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Stok Alımları', HESAPKOD='901080608' WHERE YIL=2021 AND HESAPKOD='901080608'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Sermaye Giderleri', HESAPKOD='901080609' WHERE YIL=2021 AND HESAPKOD='901080609'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sermaye Transferleri', HESAPKOD='9010807' WHERE YIL=2021 AND HESAPKOD='9010807'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kurum, İşletme ve Hanehalkına Yapılan Sermaye Transferleri', HESAPKOD='901080701' WHERE YIL=2021 AND HESAPKOD='901080701'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Borç Verme ', HESAPKOD='9010808' WHERE YIL=2021 AND HESAPKOD='9010808'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yurtiçi Borç Verme ', HESAPKOD='901080801' WHERE YIL=2021 AND HESAPKOD='901080801'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kurum Dışı Aktarmalarla Eklenen Ödenek', HESAPKOD='90109' WHERE YIL=2021 AND HESAPKOD='90109'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Personel Giderleri', HESAPKOD='9010901' WHERE YIL=2021 AND HESAPKOD='9010901'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Memurlar', HESAPKOD='901090101' WHERE YIL=2021 AND HESAPKOD='901090101'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sözleşmeli Personel ', HESAPKOD='901090102' WHERE YIL=2021 AND HESAPKOD='901090102'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='İşçiler', HESAPKOD='901090103' WHERE YIL=2021 AND HESAPKOD='901090103'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Geçici Süreli Çalışanlar', HESAPKOD='901090104' WHERE YIL=2021 AND HESAPKOD='901090104'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Personel', HESAPKOD='901090105' WHERE YIL=2021 AND HESAPKOD='901090105'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Milletvekilleri', HESAPKOD='901090107' WHERE YIL=2021 AND HESAPKOD='901090107'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Cumhurbaşkanlığı Ödeneği', HESAPKOD='901090108' WHERE YIL=2021 AND HESAPKOD='901090108'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='İstihbarat Personeli', HESAPKOD='901090109' WHERE YIL=2021 AND HESAPKOD='901090109'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sosyal Güvenlik Kurumlarına Devlet Primi Giderleri', HESAPKOD='9010902' WHERE YIL=2021 AND HESAPKOD='9010902'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Memurlar', HESAPKOD='901090201' WHERE YIL=2021 AND HESAPKOD='901090201'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sözleşmeli Personel ', HESAPKOD='901090202' WHERE YIL=2021 AND HESAPKOD='901090202'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='İşçiler', HESAPKOD='901090203' WHERE YIL=2021 AND HESAPKOD='901090203'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Geçici Süreli Çalışanlar', HESAPKOD='901090204' WHERE YIL=2021 AND HESAPKOD='901090204'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Personel', HESAPKOD='901090205' WHERE YIL=2021 AND HESAPKOD='901090205'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Milletvekilleri', HESAPKOD='901090207' WHERE YIL=2021 AND HESAPKOD='901090207'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='İstihbarat Personeli', HESAPKOD='901090209' WHERE YIL=2021 AND HESAPKOD='901090209'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Mal ve Hizmet Alım Giderleri', HESAPKOD='9010903' WHERE YIL=2021 AND HESAPKOD='9010903'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Üretime Yönelik Mal ve Malzeme Alımları', HESAPKOD='901090301' WHERE YIL=2021 AND HESAPKOD='901090301'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Tüketime Yönelik Mal ve Malzeme Alımları', HESAPKOD='901090302' WHERE YIL=2021 AND HESAPKOD='901090302'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yolluklar', HESAPKOD='901090303' WHERE YIL=2021 AND HESAPKOD='901090303'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Görev Giderleri', HESAPKOD='901090304' WHERE YIL=2021 AND HESAPKOD='901090304'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Hizmet Alımları', HESAPKOD='901090305' WHERE YIL=2021 AND HESAPKOD='901090305'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Temsil ve Tanıtma Giderleri', HESAPKOD='901090306' WHERE YIL=2021 AND HESAPKOD='901090306'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Menkul Mal, Gayrimaddi Hak Alım, Bakım ve Onarım Giderleri', HESAPKOD='901090307' WHERE YIL=2021 AND HESAPKOD='901090307'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Gayrimenkul Mal Bakım ve Onarım Giderleri', HESAPKOD='901090308' WHERE YIL=2021 AND HESAPKOD='901090308'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Tedavi ve Cenaze Giderleri', HESAPKOD='901090309' WHERE YIL=2021 AND HESAPKOD='901090309'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Faiz Giderleri', HESAPKOD='9010904' WHERE YIL=2021 AND HESAPKOD='9010904'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kamu Kurumlarına Ödenen İç Borç Faiz Giderleri', HESAPKOD='901090401' WHERE YIL=2021 AND HESAPKOD='901090401'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer İç Borç Faiz Giderleri', HESAPKOD='901090402' WHERE YIL=2021 AND HESAPKOD='901090402'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Dış Borç Faiz Giderleri', HESAPKOD='901090403' WHERE YIL=2021 AND HESAPKOD='901090403'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='İskonto Giderleri', HESAPKOD='901090404' WHERE YIL=2021 AND HESAPKOD='901090404'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Para Piyasası Nakit İşlemleri Faiz Giderleri', HESAPKOD='901090405' WHERE YIL=2021 AND HESAPKOD='901090405'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Türev Ürün Giderleri', HESAPKOD='901090406' WHERE YIL=2021 AND HESAPKOD='901090406'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kira Sertifikası Giderleri', HESAPKOD='901090407' WHERE YIL=2021 AND HESAPKOD='901090407'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Cari Transferler', HESAPKOD='9010905' WHERE YIL=2021 AND HESAPKOD='9010905'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Görevlendirme Giderleri', HESAPKOD='901090501' WHERE YIL=2021 AND HESAPKOD='901090501'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Hazine Yardımları', HESAPKOD='901090502' WHERE YIL=2021 AND HESAPKOD='901090502'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kar Amacı Gütmeyen Kuruluşlara Yapılan Transferler', HESAPKOD='901090503' WHERE YIL=2021 AND HESAPKOD='901090503'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Hane Halkı ve İşletmelere Yapılan Transferler', HESAPKOD='901090504' WHERE YIL=2021 AND HESAPKOD='901090504'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Devlet Sosyal Güvenlik Kurumlarından Hane Halkına Yapılan Fayda Ödemeleri', HESAPKOD='901090505' WHERE YIL=2021 AND HESAPKOD='901090505'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yurt Dışına Yapılan Transferler ', HESAPKOD='901090506' WHERE YIL=2021 AND HESAPKOD='901090506'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Gelirlerden Ayrılan Paylar', HESAPKOD='901090508' WHERE YIL=2021 AND HESAPKOD='901090508'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sermaye Giderleri', HESAPKOD='9010906' WHERE YIL=2021 AND HESAPKOD='9010906'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Mamul Mal Alımları', HESAPKOD='901090601' WHERE YIL=2021 AND HESAPKOD='901090601'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Menkul Sermaye Üretim Giderleri', HESAPKOD='901090602' WHERE YIL=2021 AND HESAPKOD='901090602'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Gayri Maddi Hak Alımları', HESAPKOD='901090603' WHERE YIL=2021 AND HESAPKOD='901090603'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Gayrimenkul Alımları ve Kamulaştırması', HESAPKOD='901090604' WHERE YIL=2021 AND HESAPKOD='901090604'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Gayrimenkul Sermaye Üretim Giderleri', HESAPKOD='901090605' WHERE YIL=2021 AND HESAPKOD='901090605'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Menkul Malların Büyük Onarım Giderleri', HESAPKOD='901090606' WHERE YIL=2021 AND HESAPKOD='901090606'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Gayrimenkul Büyük Onarım Giderleri', HESAPKOD='901090607' WHERE YIL=2021 AND HESAPKOD='901090607'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Stok Alımları', HESAPKOD='901090608' WHERE YIL=2021 AND HESAPKOD='901090608'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Sermaye Giderleri', HESAPKOD='901090609' WHERE YIL=2021 AND HESAPKOD='901090609'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sermaye Transferleri', HESAPKOD='9010907' WHERE YIL=2021 AND HESAPKOD='9010907'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kurum, İşletme ve Hanehalkına Yapılan Sermaye Transferleri', HESAPKOD='901090701' WHERE YIL=2021 AND HESAPKOD='901090701'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yurtdışı Sermaye Transferleri', HESAPKOD='901090702' WHERE YIL=2021 AND HESAPKOD='901090702'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Borç Verme ', HESAPKOD='9010908' WHERE YIL=2021 AND HESAPKOD='9010908'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yurtiçi Borç Verme ', HESAPKOD='901090801' WHERE YIL=2021 AND HESAPKOD='901090801'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yurtdışı Borç Verme ', HESAPKOD='901090802' WHERE YIL=2021 AND HESAPKOD='901090802'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yedek Ödenekler', HESAPKOD='9010909' WHERE YIL=2021 AND HESAPKOD='9010909'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Personel Giderlerini Karşılama Ödeneği', HESAPKOD='901090901' WHERE YIL=2021 AND HESAPKOD='901090901'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kur Farklarını Karşılama Ödeneği', HESAPKOD='901090902' WHERE YIL=2021 AND HESAPKOD='901090902'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yatırımları Hızlandırma Ödeneği', HESAPKOD='901090903' WHERE YIL=2021 AND HESAPKOD='901090903'
DELETE FROM  MUHHESAPPLANI WHERE YIL=2021 AND HESAPKOD='901090904'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Doğal Afet Giderlerini Karşılama Ödeneği', HESAPKOD='901090905' WHERE YIL=2021 AND HESAPKOD='901090905'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yedek Ödenek', HESAPKOD='901090906' WHERE YIL=2021 AND HESAPKOD='901090906'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yeni Kurulacak Daire ve İdarelerin İhtiyaçlarını Karşılama Ödeneği', HESAPKOD='901090907' WHERE YIL=2021 AND HESAPKOD='901090907'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Mülteci ve Göçmen Giderleri Ödeneği', HESAPKOD='901090908' WHERE YIL=2021 AND HESAPKOD='901090908'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Yedek Ödenekler', HESAPKOD='901090909' WHERE YIL=2021 AND HESAPKOD='901090909'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kurum Dışı Aktarmalarla Düşülen Ödenek', HESAPKOD='90110' WHERE YIL=2021 AND HESAPKOD='90110'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Personel Giderleri', HESAPKOD='9011001' WHERE YIL=2021 AND HESAPKOD='9011001'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Memurlar', HESAPKOD='901100101' WHERE YIL=2021 AND HESAPKOD='901100101'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sözleşmeli Personel ', HESAPKOD='901100102' WHERE YIL=2021 AND HESAPKOD='901100102'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='İşçiler', HESAPKOD='901100103' WHERE YIL=2021 AND HESAPKOD='901100103'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Geçici Süreli Çalışanlar', HESAPKOD='901100104' WHERE YIL=2021 AND HESAPKOD='901100104'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Personel', HESAPKOD='901100105' WHERE YIL=2021 AND HESAPKOD='901100105'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Milletvekilleri', HESAPKOD='901100107' WHERE YIL=2021 AND HESAPKOD='901100107'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Cumhurbaşkanlığı Ödeneği', HESAPKOD='901100108' WHERE YIL=2021 AND HESAPKOD='901100108'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='İstihbarat Personeli', HESAPKOD='901100109' WHERE YIL=2021 AND HESAPKOD='901100109'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sosyal Güvenlik Kurumlarına Devlet Primi Giderleri', HESAPKOD='9011002' WHERE YIL=2021 AND HESAPKOD='9011002'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Memurlar', HESAPKOD='901100201' WHERE YIL=2021 AND HESAPKOD='901100201'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sözleşmeli Personel ', HESAPKOD='901100202' WHERE YIL=2021 AND HESAPKOD='901100202'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='İşçiler', HESAPKOD='901100203' WHERE YIL=2021 AND HESAPKOD='901100203'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Geçici Süreli Çalışanlar', HESAPKOD='901100204' WHERE YIL=2021 AND HESAPKOD='901100204'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Personel', HESAPKOD='901100205' WHERE YIL=2021 AND HESAPKOD='901100205'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Milletvekilleri', HESAPKOD='901100207' WHERE YIL=2021 AND HESAPKOD='901100207'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='İstihbarat Personeli', HESAPKOD='901100209' WHERE YIL=2021 AND HESAPKOD='901100209'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Mal ve Hizmet Alım Giderleri', HESAPKOD='9011003' WHERE YIL=2021 AND HESAPKOD='9011003'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Üretime Yönelik Mal ve Malzeme Alımları', HESAPKOD='901100301' WHERE YIL=2021 AND HESAPKOD='901100301'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Tüketime Yönelik Mal ve Malzeme Alımları', HESAPKOD='901100302' WHERE YIL=2021 AND HESAPKOD='901100302'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yolluklar', HESAPKOD='901100303' WHERE YIL=2021 AND HESAPKOD='901100303'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Görev Giderleri', HESAPKOD='901100304' WHERE YIL=2021 AND HESAPKOD='901100304'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Hizmet Alımları', HESAPKOD='901100305' WHERE YIL=2021 AND HESAPKOD='901100305'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Temsil ve Tanıtma Giderleri', HESAPKOD='901100306' WHERE YIL=2021 AND HESAPKOD='901100306'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Menkul Mal, Gayrimaddi Hak Alım, Bakım ve Onarım Giderleri', HESAPKOD='901100307' WHERE YIL=2021 AND HESAPKOD='901100307'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Gayrimenkul Mal Bakım ve Onarım Giderleri', HESAPKOD='901100308' WHERE YIL=2021 AND HESAPKOD='901100308'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Tedavi ve Cenaze Giderleri', HESAPKOD='901100309' WHERE YIL=2021 AND HESAPKOD='901100309'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Faiz Giderleri', HESAPKOD='9011004' WHERE YIL=2021 AND HESAPKOD='9011004'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kamu Kurumlarına Ödenen İç Borç Faiz Giderleri', HESAPKOD='901100401' WHERE YIL=2021 AND HESAPKOD='901100401'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer İç Borç Faiz Giderleri', HESAPKOD='901100402' WHERE YIL=2021 AND HESAPKOD='901100402'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Dış Borç Faiz Giderleri', HESAPKOD='901100403' WHERE YIL=2021 AND HESAPKOD='901100403'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='İskonto Giderleri', HESAPKOD='901100404' WHERE YIL=2021 AND HESAPKOD='901100404'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Para Piyasası Nakit İşlemleri Faiz Giderleri', HESAPKOD='901100405' WHERE YIL=2021 AND HESAPKOD='901100405'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Türev Ürün Giderleri', HESAPKOD='901100406' WHERE YIL=2021 AND HESAPKOD='901100406'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kira Sertifikası Giderleri', HESAPKOD='901100407' WHERE YIL=2021 AND HESAPKOD='901100407'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Cari Transferler', HESAPKOD='9011005' WHERE YIL=2021 AND HESAPKOD='9011005'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Görevlendirme Giderleri', HESAPKOD='901100501' WHERE YIL=2021 AND HESAPKOD='901100501'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Hazine Yardımları', HESAPKOD='901100502' WHERE YIL=2021 AND HESAPKOD='901100502'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kar Amacı Gütmeyen Kuruluşlara Yapılan Transferler', HESAPKOD='901100503' WHERE YIL=2021 AND HESAPKOD='901100503'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Hane Halkı ve İşletmelere Yapılan Transferler', HESAPKOD='901100504' WHERE YIL=2021 AND HESAPKOD='901100504'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Devlet Sosyal Güvenlik Kurumlarından Hane Halkına Yapılan Fayda Ödemeleri', HESAPKOD='901100505' WHERE YIL=2021 AND HESAPKOD='901100505'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yurt Dışına Yapılan Transferler ', HESAPKOD='901100506' WHERE YIL=2021 AND HESAPKOD='901100506'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Gelirlerden Ayrılan Paylar', HESAPKOD='901100508' WHERE YIL=2021 AND HESAPKOD='901100508'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sermaye Giderleri', HESAPKOD='9011006' WHERE YIL=2021 AND HESAPKOD='9011006'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Mamul Mal Alımları', HESAPKOD='901100601' WHERE YIL=2021 AND HESAPKOD='901100601'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Menkul Sermaye Üretim Giderleri', HESAPKOD='901100602' WHERE YIL=2021 AND HESAPKOD='901100602'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Gayri Maddi Hak Alımları', HESAPKOD='901100603' WHERE YIL=2021 AND HESAPKOD='901100603'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Gayrimenkul Alımları ve Kamulaştırması', HESAPKOD='901100604' WHERE YIL=2021 AND HESAPKOD='901100604'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Gayrimenkul Sermaye Üretim Giderleri', HESAPKOD='901100605' WHERE YIL=2021 AND HESAPKOD='901100605'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Menkul Malların Büyük Onarım Giderleri', HESAPKOD='901100606' WHERE YIL=2021 AND HESAPKOD='901100606'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Gayrimenkul Büyük Onarım Giderleri', HESAPKOD='901100607' WHERE YIL=2021 AND HESAPKOD='901100607'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Stok Alımları', HESAPKOD='901100608' WHERE YIL=2021 AND HESAPKOD='901100608'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Sermaye Giderleri', HESAPKOD='901100609' WHERE YIL=2021 AND HESAPKOD='901100609'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sermaye Transferleri', HESAPKOD='9011007' WHERE YIL=2021 AND HESAPKOD='9011007'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kurum, İşletme ve Hanehalkına Yapılan Sermaye Transferleri', HESAPKOD='901100701' WHERE YIL=2021 AND HESAPKOD='901100701'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yurtdışı Sermaye Transferleri', HESAPKOD='901100702' WHERE YIL=2021 AND HESAPKOD='901100702'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Borç Verme ', HESAPKOD='9011008' WHERE YIL=2021 AND HESAPKOD='9011008'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yurtiçi Borç Verme ', HESAPKOD='901100801' WHERE YIL=2021 AND HESAPKOD='901100801'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yurtdışı Borç Verme ', HESAPKOD='901100802' WHERE YIL=2021 AND HESAPKOD='901100802'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yedek Ödenekler', HESAPKOD='9011009' WHERE YIL=2021 AND HESAPKOD='9011009'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Personel Giderlerini Karşılama Ödeneği', HESAPKOD='901100901' WHERE YIL=2021 AND HESAPKOD='901100901'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kur Farklarını Karşılama Ödeneği', HESAPKOD='901100902' WHERE YIL=2021 AND HESAPKOD='901100902'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yatırımları Hızlandırma Ödeneği', HESAPKOD='901100903' WHERE YIL=2021 AND HESAPKOD='901100903'
DELETE FROM  MUHHESAPPLANI WHERE YIL=2021 AND HESAPKOD='901100904'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Doğal Afet Giderlerini Karşılama Ödeneği', HESAPKOD='901100905' WHERE YIL=2021 AND HESAPKOD='901100905'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yedek Ödenek', HESAPKOD='901100906' WHERE YIL=2021 AND HESAPKOD='901100906'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yeni Kurulacak Daire ve İdarelerin İhtiyaçlarını Karşılama Ödeneği', HESAPKOD='901100907' WHERE YIL=2021 AND HESAPKOD='901100907'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Mülteci ve Göçmen Giderleri Ödeneği', HESAPKOD='901100908' WHERE YIL=2021 AND HESAPKOD='901100908'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Yedek Ödenekler', HESAPKOD='901100909' WHERE YIL=2021 AND HESAPKOD='901100909'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kurum İçi Aktarmalarla Eklenen Ödenek', HESAPKOD='90111' WHERE YIL=2021 AND HESAPKOD='90111'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Personel Giderleri', HESAPKOD='9011101' WHERE YIL=2021 AND HESAPKOD='9011101'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Memurlar', HESAPKOD='901110101' WHERE YIL=2021 AND HESAPKOD='901110101'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sözleşmeli Personel ', HESAPKOD='901110102' WHERE YIL=2021 AND HESAPKOD='901110102'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='İşçiler', HESAPKOD='901110103' WHERE YIL=2021 AND HESAPKOD='901110103'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Geçici Süreli Çalışanlar', HESAPKOD='901110104' WHERE YIL=2021 AND HESAPKOD='901110104'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Personel', HESAPKOD='901110105' WHERE YIL=2021 AND HESAPKOD='901110105'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Milletvekilleri', HESAPKOD='901110107' WHERE YIL=2021 AND HESAPKOD='901110107'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Cumhurbaşkanlığı Ödeneği', HESAPKOD='901110108' WHERE YIL=2021 AND HESAPKOD='901110108'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='İstihbarat Personeli', HESAPKOD='901110109' WHERE YIL=2021 AND HESAPKOD='901110109'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sosyal Güvenlik Kurumlarına Devlet Primi Giderleri', HESAPKOD='9011102' WHERE YIL=2021 AND HESAPKOD='9011102'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Memurlar', HESAPKOD='901110201' WHERE YIL=2021 AND HESAPKOD='901110201'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sözleşmeli Personel ', HESAPKOD='901110202' WHERE YIL=2021 AND HESAPKOD='901110202'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='İşçiler', HESAPKOD='901110203' WHERE YIL=2021 AND HESAPKOD='901110203'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Geçici Süreli Çalışanlar', HESAPKOD='901110204' WHERE YIL=2021 AND HESAPKOD='901110204'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Personel', HESAPKOD='901110205' WHERE YIL=2021 AND HESAPKOD='901110205'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Milletvekilleri', HESAPKOD='901110207' WHERE YIL=2021 AND HESAPKOD='901110207'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='İstihbarat Personeli', HESAPKOD='901110209' WHERE YIL=2021 AND HESAPKOD='901110209'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Mal ve Hizmet Alım Giderleri', HESAPKOD='9011103' WHERE YIL=2021 AND HESAPKOD='9011103'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Üretime Yönelik Mal ve Malzeme Alımları', HESAPKOD='901110301' WHERE YIL=2021 AND HESAPKOD='901110301'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Tüketime Yönelik Mal ve Malzeme Alımları', HESAPKOD='901110302' WHERE YIL=2021 AND HESAPKOD='901110302'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yolluklar', HESAPKOD='901110303' WHERE YIL=2021 AND HESAPKOD='901110303'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Görev Giderleri', HESAPKOD='901110304' WHERE YIL=2021 AND HESAPKOD='901110304'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Hizmet Alımları', HESAPKOD='901110305' WHERE YIL=2021 AND HESAPKOD='901110305'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Temsil ve Tanıtma Giderleri', HESAPKOD='901110306' WHERE YIL=2021 AND HESAPKOD='901110306'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Menkul Mal, Gayrimaddi Hak Alım, Bakım ve Onarım Giderleri', HESAPKOD='901110307' WHERE YIL=2021 AND HESAPKOD='901110307'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Gayrimenkul Mal Bakım ve Onarım Giderleri', HESAPKOD='901110308' WHERE YIL=2021 AND HESAPKOD='901110308'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Tedavi ve Cenaze Giderleri', HESAPKOD='901110309' WHERE YIL=2021 AND HESAPKOD='901110309'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Faiz Giderleri', HESAPKOD='9011104' WHERE YIL=2021 AND HESAPKOD='9011104'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kamu Kurumlarına Ödenen İç Borç Faiz Giderleri', HESAPKOD='901110401' WHERE YIL=2021 AND HESAPKOD='901110401'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer İç Borç Faiz Giderleri', HESAPKOD='901110402' WHERE YIL=2021 AND HESAPKOD='901110402'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Dış Borç Faiz Giderleri', HESAPKOD='901110403' WHERE YIL=2021 AND HESAPKOD='901110403'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='İskonto Giderleri', HESAPKOD='901110404' WHERE YIL=2021 AND HESAPKOD='901110404'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Para Piyasası Nakit İşlemleri Faiz Giderleri', HESAPKOD='901110405' WHERE YIL=2021 AND HESAPKOD='901110405'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Türev Ürün Giderleri', HESAPKOD='901110406' WHERE YIL=2021 AND HESAPKOD='901110406'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kira Sertifikası Giderleri', HESAPKOD='901110407' WHERE YIL=2021 AND HESAPKOD='901110407'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Cari Transferler', HESAPKOD='9011105' WHERE YIL=2021 AND HESAPKOD='9011105'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Görevlendirme Giderleri', HESAPKOD='901110501' WHERE YIL=2021 AND HESAPKOD='901110501'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Hazine Yardımları', HESAPKOD='901110502' WHERE YIL=2021 AND HESAPKOD='901110502'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kar Amacı Gütmeyen Kuruluşlara Yapılan Transferler', HESAPKOD='901110503' WHERE YIL=2021 AND HESAPKOD='901110503'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Hane Halkı ve İşletmelere Yapılan Transferler', HESAPKOD='901110504' WHERE YIL=2021 AND HESAPKOD='901110504'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Devlet Sosyal Güvenlik Kurumlarından Hane Halkına Yapılan Fayda Ödemeleri', HESAPKOD='901110505' WHERE YIL=2021 AND HESAPKOD='901110505'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yurt Dışına Yapılan Transferler ', HESAPKOD='901110506' WHERE YIL=2021 AND HESAPKOD='901110506'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Gelirlerden Ayrılan Paylar', HESAPKOD='901110508' WHERE YIL=2021 AND HESAPKOD='901110508'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sermaye Giderleri', HESAPKOD='9011106' WHERE YIL=2021 AND HESAPKOD='9011106'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Mamul Mal Alımları', HESAPKOD='901110601' WHERE YIL=2021 AND HESAPKOD='901110601'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Menkul Sermaye Üretim Giderleri', HESAPKOD='901110602' WHERE YIL=2021 AND HESAPKOD='901110602'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Gayri Maddi Hak Alımları', HESAPKOD='901110603' WHERE YIL=2021 AND HESAPKOD='901110603'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Gayrimenkul Alımları ve Kamulaştırması', HESAPKOD='901110604' WHERE YIL=2021 AND HESAPKOD='901110604'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Gayrimenkul Sermaye Üretim Giderleri', HESAPKOD='901110605' WHERE YIL=2021 AND HESAPKOD='901110605'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Menkul Malların Büyük Onarım Giderleri', HESAPKOD='901110606' WHERE YIL=2021 AND HESAPKOD='901110606'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Gayrimenkul Büyük Onarım Giderleri', HESAPKOD='901110607' WHERE YIL=2021 AND HESAPKOD='901110607'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Stok Alımları', HESAPKOD='901110608' WHERE YIL=2021 AND HESAPKOD='901110608'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Sermaye Giderleri', HESAPKOD='901110609' WHERE YIL=2021 AND HESAPKOD='901110609'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sermaye Transferleri', HESAPKOD='9011107' WHERE YIL=2021 AND HESAPKOD='9011107'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kurum, İşletme ve Hanehalkına Yapılan Sermaye Transferleri', HESAPKOD='901110701' WHERE YIL=2021 AND HESAPKOD='901110701'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yurtdışı Sermaye Transferleri', HESAPKOD='901110702' WHERE YIL=2021 AND HESAPKOD='901110702'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Borç Verme ', HESAPKOD='9011108' WHERE YIL=2021 AND HESAPKOD='9011108'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yurtiçi Borç Verme ', HESAPKOD='901110801' WHERE YIL=2021 AND HESAPKOD='901110801'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yurtdışı Borç Verme ', HESAPKOD='901110802' WHERE YIL=2021 AND HESAPKOD='901110802'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yedek Ödenekler', HESAPKOD='9011109' WHERE YIL=2021 AND HESAPKOD='9011109'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Personel Giderlerini Karşılama Ödeneği', HESAPKOD='901110901' WHERE YIL=2021 AND HESAPKOD='901110901'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kur Farklarını Karşılama Ödeneği', HESAPKOD='901110902' WHERE YIL=2021 AND HESAPKOD='901110902'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yatırımları Hızlandırma Ödeneği', HESAPKOD='901110903' WHERE YIL=2021 AND HESAPKOD='901110903'
DELETE FROM  MUHHESAPPLANI WHERE YIL=2021 AND HESAPKOD='901110904'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Doğal Afet Giderlerini Karşılama Ödeneği', HESAPKOD='901110905' WHERE YIL=2021 AND HESAPKOD='901110905'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yedek Ödenek', HESAPKOD='901110906' WHERE YIL=2021 AND HESAPKOD='901110906'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yeni Kurulacak Daire ve İdarelerin İhtiyaçlarını Karşılama Ödeneği', HESAPKOD='901110907' WHERE YIL=2021 AND HESAPKOD='901110907'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Mülteci ve Göçmen Giderleri Ödeneği', HESAPKOD='901110908' WHERE YIL=2021 AND HESAPKOD='901110908'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Yedek Ödenekler', HESAPKOD='901110909' WHERE YIL=2021 AND HESAPKOD='901110909'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kurum İçi Aktarmalarla Düşülen Ödenek', HESAPKOD='90112' WHERE YIL=2021 AND HESAPKOD='90112'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Personel Giderleri', HESAPKOD='9011201' WHERE YIL=2021 AND HESAPKOD='9011201'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Memurlar', HESAPKOD='901120101' WHERE YIL=2021 AND HESAPKOD='901120101'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sözleşmeli Personel ', HESAPKOD='901120102' WHERE YIL=2021 AND HESAPKOD='901120102'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='İşçiler', HESAPKOD='901120103' WHERE YIL=2021 AND HESAPKOD='901120103'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Geçici Süreli Çalışanlar', HESAPKOD='901120104' WHERE YIL=2021 AND HESAPKOD='901120104'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Personel', HESAPKOD='901120105' WHERE YIL=2021 AND HESAPKOD='901120105'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Milletvekilleri', HESAPKOD='901120107' WHERE YIL=2021 AND HESAPKOD='901120107'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Cumhurbaşkanlığı Ödeneği', HESAPKOD='901120108' WHERE YIL=2021 AND HESAPKOD='901120108'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='İstihbarat Personeli', HESAPKOD='901120109' WHERE YIL=2021 AND HESAPKOD='901120109'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sosyal Güvenlik Kurumlarına Devlet Primi Giderleri', HESAPKOD='9011202' WHERE YIL=2021 AND HESAPKOD='9011202'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Memurlar', HESAPKOD='901120201' WHERE YIL=2021 AND HESAPKOD='901120201'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sözleşmeli Personel ', HESAPKOD='901120202' WHERE YIL=2021 AND HESAPKOD='901120202'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='İşçiler', HESAPKOD='901120203' WHERE YIL=2021 AND HESAPKOD='901120203'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Geçici Süreli Çalışanlar', HESAPKOD='901120204' WHERE YIL=2021 AND HESAPKOD='901120204'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Personel', HESAPKOD='901120205' WHERE YIL=2021 AND HESAPKOD='901120205'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Milletvekilleri', HESAPKOD='901120207' WHERE YIL=2021 AND HESAPKOD='901120207'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='İstihbarat Personeli', HESAPKOD='901120209' WHERE YIL=2021 AND HESAPKOD='901120209'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Mal ve Hizmet Alım Giderleri', HESAPKOD='9011203' WHERE YIL=2021 AND HESAPKOD='9011203'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Üretime Yönelik Mal ve Malzeme Alımları', HESAPKOD='901120301' WHERE YIL=2021 AND HESAPKOD='901120301'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Tüketime Yönelik Mal ve Malzeme Alımları', HESAPKOD='901120302' WHERE YIL=2021 AND HESAPKOD='901120302'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yolluklar', HESAPKOD='901120303' WHERE YIL=2021 AND HESAPKOD='901120303'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Görev Giderleri', HESAPKOD='901120304' WHERE YIL=2021 AND HESAPKOD='901120304'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Hizmet Alımları', HESAPKOD='901120305' WHERE YIL=2021 AND HESAPKOD='901120305'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Temsil ve Tanıtma Giderleri', HESAPKOD='901120306' WHERE YIL=2021 AND HESAPKOD='901120306'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Menkul Mal, Gayrimaddi Hak Alım, Bakım ve Onarım Giderleri', HESAPKOD='901120307' WHERE YIL=2021 AND HESAPKOD='901120307'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Gayrimenkul Mal Bakım ve Onarım Giderleri', HESAPKOD='901120308' WHERE YIL=2021 AND HESAPKOD='901120308'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Tedavi ve Cenaze Giderleri', HESAPKOD='901120309' WHERE YIL=2021 AND HESAPKOD='901120309'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Faiz Giderleri', HESAPKOD='9011204' WHERE YIL=2021 AND HESAPKOD='9011204'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kamu Kurumlarına Ödenen İç Borç Faiz Giderleri', HESAPKOD='901120401' WHERE YIL=2021 AND HESAPKOD='901120401'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer İç Borç Faiz Giderleri', HESAPKOD='901120402' WHERE YIL=2021 AND HESAPKOD='901120402'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Dış Borç Faiz Giderleri', HESAPKOD='901120403' WHERE YIL=2021 AND HESAPKOD='901120403'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='İskonto Giderleri', HESAPKOD='901120404' WHERE YIL=2021 AND HESAPKOD='901120404'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Para Piyasası Nakit İşlemleri Faiz Giderleri', HESAPKOD='901120405' WHERE YIL=2021 AND HESAPKOD='901120405'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Türev Ürün Giderleri', HESAPKOD='901120406' WHERE YIL=2021 AND HESAPKOD='901120406'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kira Sertifikası Giderleri', HESAPKOD='901120407' WHERE YIL=2021 AND HESAPKOD='901120407'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Cari Transferler', HESAPKOD='9011205' WHERE YIL=2021 AND HESAPKOD='9011205'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Görevlendirme Giderleri', HESAPKOD='901120501' WHERE YIL=2021 AND HESAPKOD='901120501'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Hazine Yardımları', HESAPKOD='901120502' WHERE YIL=2021 AND HESAPKOD='901120502'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kar Amacı Gütmeyen Kuruluşlara Yapılan Transferler', HESAPKOD='901120503' WHERE YIL=2021 AND HESAPKOD='901120503'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Hane Halkı ve İşletmelere Yapılan Transferler', HESAPKOD='901120504' WHERE YIL=2021 AND HESAPKOD='901120504'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Devlet Sosyal Güvenlik Kurumlarından Hane Halkına Yapılan Fayda Ödemeleri', HESAPKOD='901120505' WHERE YIL=2021 AND HESAPKOD='901120505'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yurt Dışına Yapılan Transferler ', HESAPKOD='901120506' WHERE YIL=2021 AND HESAPKOD='901120506'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Gelirlerden Ayrılan Paylar', HESAPKOD='901120508' WHERE YIL=2021 AND HESAPKOD='901120508'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sermaye Giderleri', HESAPKOD='9011206' WHERE YIL=2021 AND HESAPKOD='9011206'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Mamul Mal Alımları', HESAPKOD='901120601' WHERE YIL=2021 AND HESAPKOD='901120601'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Menkul Sermaye Üretim Giderleri', HESAPKOD='901120602' WHERE YIL=2021 AND HESAPKOD='901120602'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Gayri Maddi Hak Alımları', HESAPKOD='901120603' WHERE YIL=2021 AND HESAPKOD='901120603'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Gayrimenkul Alımları ve Kamulaştırması', HESAPKOD='901120604' WHERE YIL=2021 AND HESAPKOD='901120604'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Gayrimenkul Sermaye Üretim Giderleri', HESAPKOD='901120605' WHERE YIL=2021 AND HESAPKOD='901120605'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Menkul Malların Büyük Onarım Giderleri', HESAPKOD='901120606' WHERE YIL=2021 AND HESAPKOD='901120606'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Gayrimenkul Büyük Onarım Giderleri', HESAPKOD='901120607' WHERE YIL=2021 AND HESAPKOD='901120607'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Stok Alımları', HESAPKOD='901120608' WHERE YIL=2021 AND HESAPKOD='901120608'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Sermaye Giderleri', HESAPKOD='901120609' WHERE YIL=2021 AND HESAPKOD='901120609'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sermaye Transferleri', HESAPKOD='9011207' WHERE YIL=2021 AND HESAPKOD='9011207'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kurum, İşletme ve Hanehalkına Yapılan Sermaye Transferleri', HESAPKOD='901120701' WHERE YIL=2021 AND HESAPKOD='901120701'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yurtdışı Sermaye Transferleri', HESAPKOD='901120702' WHERE YIL=2021 AND HESAPKOD='901120702'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Borç Verme ', HESAPKOD='9011208' WHERE YIL=2021 AND HESAPKOD='9011208'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yurtiçi Borç Verme ', HESAPKOD='901120801' WHERE YIL=2021 AND HESAPKOD='901120801'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yurtdışı Borç Verme ', HESAPKOD='901120802' WHERE YIL=2021 AND HESAPKOD='901120802'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yedek Ödenekler', HESAPKOD='9011209' WHERE YIL=2021 AND HESAPKOD='9011209'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Personel Giderlerini Karşılama Ödeneği', HESAPKOD='901120901' WHERE YIL=2021 AND HESAPKOD='901120901'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kur Farklarını Karşılama Ödeneği', HESAPKOD='901120902' WHERE YIL=2021 AND HESAPKOD='901120902'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yatırımları Hızlandırma Ödeneği', HESAPKOD='901120903' WHERE YIL=2021 AND HESAPKOD='901120903'
DELETE FROM  MUHHESAPPLANI WHERE YIL=2021 AND HESAPKOD='901120904'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Doğal Afet Giderlerini Karşılama Ödeneği', HESAPKOD='901120905' WHERE YIL=2021 AND HESAPKOD='901120905'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yedek Ödenek', HESAPKOD='901120906' WHERE YIL=2021 AND HESAPKOD='901120906'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yeni Kurulacak Daire ve İdarelerin İhtiyaçlarını Karşılama Ödeneği', HESAPKOD='901120907' WHERE YIL=2021 AND HESAPKOD='901120907'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Mülteci ve Göçmen Giderleri Ödeneği', HESAPKOD='901120908' WHERE YIL=2021 AND HESAPKOD='901120908'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Yedek Ödenekler', HESAPKOD='901120909' WHERE YIL=2021 AND HESAPKOD='901120909'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yedek Ödenekten Aktarmalarla Eklenen Ödenek', HESAPKOD='90113' WHERE YIL=2021 AND HESAPKOD='90113'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Personel Giderleri', HESAPKOD='9011301' WHERE YIL=2021 AND HESAPKOD='9011301'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Memurlar', HESAPKOD='901130101' WHERE YIL=2021 AND HESAPKOD='901130101'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sözleşmeli Personel ', HESAPKOD='901130102' WHERE YIL=2021 AND HESAPKOD='901130102'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='İşçiler', HESAPKOD='901130103' WHERE YIL=2021 AND HESAPKOD='901130103'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Geçici Süreli Çalışanlar', HESAPKOD='901130104' WHERE YIL=2021 AND HESAPKOD='901130104'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Personel', HESAPKOD='901130105' WHERE YIL=2021 AND HESAPKOD='901130105'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Milletvekilleri', HESAPKOD='901130107' WHERE YIL=2021 AND HESAPKOD='901130107'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Cumhurbaşkanlığı Ödeneği', HESAPKOD='901130108' WHERE YIL=2021 AND HESAPKOD='901130108'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='İstihbarat Personeli', HESAPKOD='901130109' WHERE YIL=2021 AND HESAPKOD='901130109'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sosyal Güvenlik Kurumlarına Devlet Primi Giderleri', HESAPKOD='9011302' WHERE YIL=2021 AND HESAPKOD='9011302'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Memurlar', HESAPKOD='901130201' WHERE YIL=2021 AND HESAPKOD='901130201'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sözleşmeli Personel ', HESAPKOD='901130202' WHERE YIL=2021 AND HESAPKOD='901130202'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='İşçiler', HESAPKOD='901130203' WHERE YIL=2021 AND HESAPKOD='901130203'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Geçici Süreli Çalışanlar', HESAPKOD='901130204' WHERE YIL=2021 AND HESAPKOD='901130204'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Personel', HESAPKOD='901130205' WHERE YIL=2021 AND HESAPKOD='901130205'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Milletvekilleri', HESAPKOD='901130207' WHERE YIL=2021 AND HESAPKOD='901130207'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='İstihbarat Personeli', HESAPKOD='901130209' WHERE YIL=2021 AND HESAPKOD='901130209'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Mal ve Hizmet Alım Giderleri', HESAPKOD='9011303' WHERE YIL=2021 AND HESAPKOD='9011303'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Üretime Yönelik Mal ve Malzeme Alımları', HESAPKOD='901130301' WHERE YIL=2021 AND HESAPKOD='901130301'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Tüketime Yönelik Mal ve Malzeme Alımları', HESAPKOD='901130302' WHERE YIL=2021 AND HESAPKOD='901130302'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yolluklar', HESAPKOD='901130303' WHERE YIL=2021 AND HESAPKOD='901130303'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Görev Giderleri', HESAPKOD='901130304' WHERE YIL=2021 AND HESAPKOD='901130304'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Hizmet Alımları', HESAPKOD='901130305' WHERE YIL=2021 AND HESAPKOD='901130305'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Temsil ve Tanıtma Giderleri', HESAPKOD='901130306' WHERE YIL=2021 AND HESAPKOD='901130306'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Menkul Mal, Gayrimaddi Hak Alım, Bakım ve Onarım Giderleri', HESAPKOD='901130307' WHERE YIL=2021 AND HESAPKOD='901130307'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Gayrimenkul Mal Bakım ve Onarım Giderleri', HESAPKOD='901130308' WHERE YIL=2021 AND HESAPKOD='901130308'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Tedavi ve Cenaze Giderleri', HESAPKOD='901130309' WHERE YIL=2021 AND HESAPKOD='901130309'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Faiz Giderleri', HESAPKOD='9011304' WHERE YIL=2021 AND HESAPKOD='9011304'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kamu Kurumlarına Ödenen İç Borç Faiz Giderleri', HESAPKOD='901130401' WHERE YIL=2021 AND HESAPKOD='901130401'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer İç Borç Faiz Giderleri', HESAPKOD='901130402' WHERE YIL=2021 AND HESAPKOD='901130402'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Dış Borç Faiz Giderleri', HESAPKOD='901130403' WHERE YIL=2021 AND HESAPKOD='901130403'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='İskonto Giderleri', HESAPKOD='901130404' WHERE YIL=2021 AND HESAPKOD='901130404'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Para Piyasası Nakit İşlemleri Faiz Giderleri', HESAPKOD='901130405' WHERE YIL=2021 AND HESAPKOD='901130405'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Türev Ürün Giderleri', HESAPKOD='901130406' WHERE YIL=2021 AND HESAPKOD='901130406'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kira Sertifikası Giderleri', HESAPKOD='901130407' WHERE YIL=2021 AND HESAPKOD='901130407'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Cari Transferler', HESAPKOD='9011305' WHERE YIL=2021 AND HESAPKOD='9011305'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Görevlendirme Giderleri', HESAPKOD='901130501' WHERE YIL=2021 AND HESAPKOD='901130501'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Hazine Yardımları', HESAPKOD='901130502' WHERE YIL=2021 AND HESAPKOD='901130502'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kar Amacı Gütmeyen Kuruluşlara Yapılan Transferler', HESAPKOD='901130503' WHERE YIL=2021 AND HESAPKOD='901130503'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Hane Halkı ve İşletmelere Yapılan Transferler', HESAPKOD='901130504' WHERE YIL=2021 AND HESAPKOD='901130504'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Devlet Sosyal Güvenlik Kurumlarından Hane Halkına Yapılan Fayda Ödemeleri', HESAPKOD='901130505' WHERE YIL=2021 AND HESAPKOD='901130505'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yurt Dışına Yapılan Transferler ', HESAPKOD='901130506' WHERE YIL=2021 AND HESAPKOD='901130506'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Gelirlerden Ayrılan Paylar', HESAPKOD='901130508' WHERE YIL=2021 AND HESAPKOD='901130508'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sermaye Giderleri', HESAPKOD='9011306' WHERE YIL=2021 AND HESAPKOD='9011306'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Mamul Mal Alımları', HESAPKOD='901130601' WHERE YIL=2021 AND HESAPKOD='901130601'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Menkul Sermaye Üretim Giderleri', HESAPKOD='901130602' WHERE YIL=2021 AND HESAPKOD='901130602'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Gayri Maddi Hak Alımları', HESAPKOD='901130603' WHERE YIL=2021 AND HESAPKOD='901130603'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Gayrimenkul Alımları ve Kamulaştırması', HESAPKOD='901130604' WHERE YIL=2021 AND HESAPKOD='901130604'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Gayrimenkul Sermaye Üretim Giderleri', HESAPKOD='901130605' WHERE YIL=2021 AND HESAPKOD='901130605'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Menkul Malların Büyük Onarım Giderleri', HESAPKOD='901130606' WHERE YIL=2021 AND HESAPKOD='901130606'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Gayrimenkul Büyük Onarım Giderleri', HESAPKOD='901130607' WHERE YIL=2021 AND HESAPKOD='901130607'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Stok Alımları', HESAPKOD='901130608' WHERE YIL=2021 AND HESAPKOD='901130608'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Sermaye Giderleri', HESAPKOD='901130609' WHERE YIL=2021 AND HESAPKOD='901130609'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sermaye Transferleri', HESAPKOD='9011307' WHERE YIL=2021 AND HESAPKOD='9011307'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kurum, İşletme ve Hanehalkına Yapılan Sermaye Transferleri', HESAPKOD='901130701' WHERE YIL=2021 AND HESAPKOD='901130701'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yurtdışı Sermaye Transferleri', HESAPKOD='901130702' WHERE YIL=2021 AND HESAPKOD='901130702'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Borç Verme ', HESAPKOD='9011308' WHERE YIL=2021 AND HESAPKOD='9011308'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yurtiçi Borç Verme ', HESAPKOD='901130801' WHERE YIL=2021 AND HESAPKOD='901130801'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yurtdışı Borç Verme ', HESAPKOD='901130802' WHERE YIL=2021 AND HESAPKOD='901130802'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yedek Ödenekler', HESAPKOD='9011309' WHERE YIL=2021 AND HESAPKOD='9011309'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yedek Ödenek', HESAPKOD='901130906' WHERE YIL=2021 AND HESAPKOD='901130906'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Yedek Ödenekler', HESAPKOD='901130909' WHERE YIL=2021 AND HESAPKOD='901130909'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yedek Ödenekten Aktarmalarla Düşülen Ödenek', HESAPKOD='90114' WHERE YIL=2021 AND HESAPKOD='90114'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Cari Transferler', HESAPKOD='9011405' WHERE YIL=2021 AND HESAPKOD='9011405'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Hane Halkı ve İşletmelere Yapılan Transferler', HESAPKOD='901140504' WHERE YIL=2021 AND HESAPKOD='901140504'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Gelirlerden Ayrılan Paylar', HESAPKOD='901140508' WHERE YIL=2021 AND HESAPKOD='901140508'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sermaye Transferleri', HESAPKOD='9011407' WHERE YIL=2021 AND HESAPKOD='9011407'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kurum, İşletme ve Hanehalkına Yapılan Sermaye Transferleri', HESAPKOD='901140701' WHERE YIL=2021 AND HESAPKOD='901140701'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yedek Ödenekler', HESAPKOD='9011409' WHERE YIL=2021 AND HESAPKOD='9011409'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Personel Giderlerini Karşılama Ödeneği', HESAPKOD='901140901' WHERE YIL=2021 AND HESAPKOD='901140901'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kur Farklarını Karşılama Ödeneği', HESAPKOD='901140902' WHERE YIL=2021 AND HESAPKOD='901140902'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yatırımları Hızlandırma Ödeneği', HESAPKOD='901140903' WHERE YIL=2021 AND HESAPKOD='901140903'
DELETE FROM  MUHHESAPPLANI WHERE YIL=2021 AND HESAPKOD='901140904'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Doğal Afet Giderlerini Karşılama Ödeneği', HESAPKOD='901140905' WHERE YIL=2021 AND HESAPKOD='901140905'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yedek Ödenek', HESAPKOD='901140906' WHERE YIL=2021 AND HESAPKOD='901140906'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yeni Kurulacak Daire ve İdarelerin İhtiyaçlarını Karşılama Ödeneği', HESAPKOD='901140907' WHERE YIL=2021 AND HESAPKOD='901140907'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Mülteci ve Göçmen Giderleri Ödeneği', HESAPKOD='901140908' WHERE YIL=2021 AND HESAPKOD='901140908'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Yedek Ödenekler', HESAPKOD='901140909' WHERE YIL=2021 AND HESAPKOD='901140909'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yılı İçinde Eklenen Ödenek', HESAPKOD='90115' WHERE YIL=2021 AND HESAPKOD='90115'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Personel Giderleri', HESAPKOD='9011501' WHERE YIL=2021 AND HESAPKOD='9011501'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Memurlar', HESAPKOD='901150101' WHERE YIL=2021 AND HESAPKOD='901150101'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sözleşmeli Personel ', HESAPKOD='901150102' WHERE YIL=2021 AND HESAPKOD='901150102'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='İşçiler', HESAPKOD='901150103' WHERE YIL=2021 AND HESAPKOD='901150103'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Geçici Süreli Çalışanlar', HESAPKOD='901150104' WHERE YIL=2021 AND HESAPKOD='901150104'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Personel', HESAPKOD='901150105' WHERE YIL=2021 AND HESAPKOD='901150105'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Milletvekilleri', HESAPKOD='901150107' WHERE YIL=2021 AND HESAPKOD='901150107'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Cumhurbaşkanlığı Ödeneği', HESAPKOD='901150108' WHERE YIL=2021 AND HESAPKOD='901150108'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='İstihbarat Personeli', HESAPKOD='901150109' WHERE YIL=2021 AND HESAPKOD='901150109'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sosyal Güvenlik Kurumlarına Devlet Primi Giderleri', HESAPKOD='9011502' WHERE YIL=2021 AND HESAPKOD='9011502'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Memurlar', HESAPKOD='901150201' WHERE YIL=2021 AND HESAPKOD='901150201'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sözleşmeli Personel ', HESAPKOD='901150202' WHERE YIL=2021 AND HESAPKOD='901150202'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='İşçiler', HESAPKOD='901150203' WHERE YIL=2021 AND HESAPKOD='901150203'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Geçici Süreli Çalışanlar', HESAPKOD='901150204' WHERE YIL=2021 AND HESAPKOD='901150204'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Personel', HESAPKOD='901150205' WHERE YIL=2021 AND HESAPKOD='901150205'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Milletvekilleri', HESAPKOD='901150207' WHERE YIL=2021 AND HESAPKOD='901150207'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='İstihbarat Personeli', HESAPKOD='901150209' WHERE YIL=2021 AND HESAPKOD='901150209'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Mal ve Hizmet Alım Giderleri', HESAPKOD='9011503' WHERE YIL=2021 AND HESAPKOD='9011503'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Üretime Yönelik Mal ve Malzeme Alımları', HESAPKOD='901150301' WHERE YIL=2021 AND HESAPKOD='901150301'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Tüketime Yönelik Mal ve Malzeme Alımları', HESAPKOD='901150302' WHERE YIL=2021 AND HESAPKOD='901150302'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yolluklar', HESAPKOD='901150303' WHERE YIL=2021 AND HESAPKOD='901150303'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Görev Giderleri', HESAPKOD='901150304' WHERE YIL=2021 AND HESAPKOD='901150304'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Hizmet Alımları', HESAPKOD='901150305' WHERE YIL=2021 AND HESAPKOD='901150305'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Temsil ve Tanıtma Giderleri', HESAPKOD='901150306' WHERE YIL=2021 AND HESAPKOD='901150306'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Menkul Mal, Gayrimaddi Hak Alım, Bakım ve Onarım Giderleri', HESAPKOD='901150307' WHERE YIL=2021 AND HESAPKOD='901150307'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Gayrimenkul Mal Bakım ve Onarım Giderleri', HESAPKOD='901150308' WHERE YIL=2021 AND HESAPKOD='901150308'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Tedavi ve Cenaze Giderleri', HESAPKOD='901150309' WHERE YIL=2021 AND HESAPKOD='901150309'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Faiz Giderleri', HESAPKOD='9011504' WHERE YIL=2021 AND HESAPKOD='9011504'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kamu Kurumlarına Ödenen İç Borç Faiz Giderleri', HESAPKOD='901150401' WHERE YIL=2021 AND HESAPKOD='901150401'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer İç Borç Faiz Giderleri', HESAPKOD='901150402' WHERE YIL=2021 AND HESAPKOD='901150402'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Dış Borç Faiz Giderleri', HESAPKOD='901150403' WHERE YIL=2021 AND HESAPKOD='901150403'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='İskonto Giderleri', HESAPKOD='901150404' WHERE YIL=2021 AND HESAPKOD='901150404'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Para Piyasası Nakit İşlemleri Faiz Giderleri', HESAPKOD='901150405' WHERE YIL=2021 AND HESAPKOD='901150405'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Türev Ürün Giderleri', HESAPKOD='901150406' WHERE YIL=2021 AND HESAPKOD='901150406'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kira Sertifikası Giderleri', HESAPKOD='901150407' WHERE YIL=2021 AND HESAPKOD='901150407'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Cari Transferler', HESAPKOD='9011505' WHERE YIL=2021 AND HESAPKOD='9011505'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Görevlendirme Giderleri', HESAPKOD='901150501' WHERE YIL=2021 AND HESAPKOD='901150501'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Hazine Yardımları', HESAPKOD='901150502' WHERE YIL=2021 AND HESAPKOD='901150502'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kar Amacı Gütmeyen Kuruluşlara Yapılan Transferler', HESAPKOD='901150503' WHERE YIL=2021 AND HESAPKOD='901150503'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Hane Halkı ve İşletmelere Yapılan Transferler', HESAPKOD='901150504' WHERE YIL=2021 AND HESAPKOD='901150504'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Devlet Sosyal Güvenlik Kurumlarından Hane Halkına Yapılan Fayda Ödemeleri', HESAPKOD='901150505' WHERE YIL=2021 AND HESAPKOD='901150505'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yurt Dışına Yapılan Transferler ', HESAPKOD='901150506' WHERE YIL=2021 AND HESAPKOD='901150506'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Gelirlerden Ayrılan Paylar', HESAPKOD='901150508' WHERE YIL=2021 AND HESAPKOD='901150508'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sermaye Giderleri', HESAPKOD='9011506' WHERE YIL=2021 AND HESAPKOD='9011506'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Mamul Mal Alımları', HESAPKOD='901150601' WHERE YIL=2021 AND HESAPKOD='901150601'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Menkul Sermaye Üretim Giderleri', HESAPKOD='901150602' WHERE YIL=2021 AND HESAPKOD='901150602'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Gayri Maddi Hak Alımları', HESAPKOD='901150603' WHERE YIL=2021 AND HESAPKOD='901150603'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Gayrimenkul Alımları ve Kamulaştırması', HESAPKOD='901150604' WHERE YIL=2021 AND HESAPKOD='901150604'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Gayrimenkul Sermaye Üretim Giderleri', HESAPKOD='901150605' WHERE YIL=2021 AND HESAPKOD='901150605'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Menkul Malların Büyük Onarım Giderleri', HESAPKOD='901150606' WHERE YIL=2021 AND HESAPKOD='901150606'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Gayrimenkul Büyük Onarım Giderleri', HESAPKOD='901150607' WHERE YIL=2021 AND HESAPKOD='901150607'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Stok Alımları', HESAPKOD='901150608' WHERE YIL=2021 AND HESAPKOD='901150608'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Sermaye Giderleri', HESAPKOD='901150609' WHERE YIL=2021 AND HESAPKOD='901150609'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sermaye Transferleri', HESAPKOD='9011507' WHERE YIL=2021 AND HESAPKOD='9011507'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kurum, İşletme ve Hanehalkına Yapılan Sermaye Transferleri', HESAPKOD='901150701' WHERE YIL=2021 AND HESAPKOD='901150701'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yurtdışı Sermaye Transferleri', HESAPKOD='901150702' WHERE YIL=2021 AND HESAPKOD='901150702'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Borç Verme ', HESAPKOD='9011508' WHERE YIL=2021 AND HESAPKOD='9011508'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yurtiçi Borç Verme ', HESAPKOD='901150801' WHERE YIL=2021 AND HESAPKOD='901150801'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yurtdışı Borç Verme ', HESAPKOD='901150802' WHERE YIL=2021 AND HESAPKOD='901150802'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yedek Ödenekler', HESAPKOD='9011509' WHERE YIL=2021 AND HESAPKOD='9011509'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Personel Giderlerini Karşılama Ödeneği', HESAPKOD='901150901' WHERE YIL=2021 AND HESAPKOD='901150901'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kur Farklarını Karşılama Ödeneği', HESAPKOD='901150902' WHERE YIL=2021 AND HESAPKOD='901150902'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yatırımları Hızlandırma Ödeneği', HESAPKOD='901150903' WHERE YIL=2021 AND HESAPKOD='901150903'
DELETE FROM  MUHHESAPPLANI WHERE YIL=2021 AND HESAPKOD='901150904'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Doğal Afet Giderlerini Karşılama Ödeneği', HESAPKOD='901150905' WHERE YIL=2021 AND HESAPKOD='901150905'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yedek Ödenek', HESAPKOD='901150906' WHERE YIL=2021 AND HESAPKOD='901150906'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yeni Kurulacak Daire ve İdarelerin İhtiyaçlarını Karşılama Ödeneği', HESAPKOD='901150907' WHERE YIL=2021 AND HESAPKOD='901150907'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Mülteci ve Göçmen Giderleri Ödeneği', HESAPKOD='901150908' WHERE YIL=2021 AND HESAPKOD='901150908'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Yedek Ödenekler', HESAPKOD='901150909' WHERE YIL=2021 AND HESAPKOD='901150909'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yılı İçinde Eklenen Özel Ödenek/Özel Gelir', HESAPKOD='90116' WHERE YIL=2021 AND HESAPKOD='90116'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Personel Giderleri', HESAPKOD='9011601' WHERE YIL=2021 AND HESAPKOD='9011601'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Memurlar', HESAPKOD='901160101' WHERE YIL=2021 AND HESAPKOD='901160101'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sözleşmeli Personel ', HESAPKOD='901160102' WHERE YIL=2021 AND HESAPKOD='901160102'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='İşçiler', HESAPKOD='901160103' WHERE YIL=2021 AND HESAPKOD='901160103'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Geçici Süreli Çalışanlar', HESAPKOD='901160104' WHERE YIL=2021 AND HESAPKOD='901160104'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sosyal Güvenlik Kurumlarına Devlet Primi Giderleri', HESAPKOD='9011602' WHERE YIL=2021 AND HESAPKOD='9011602'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Memurlar', HESAPKOD='901160201' WHERE YIL=2021 AND HESAPKOD='901160201'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sözleşmeli Personel ', HESAPKOD='901160202' WHERE YIL=2021 AND HESAPKOD='901160202'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='İşçiler', HESAPKOD='901160203' WHERE YIL=2021 AND HESAPKOD='901160203'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Geçici Süreli Çalışanlar', HESAPKOD='901160204' WHERE YIL=2021 AND HESAPKOD='901160204'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Mal ve Hizmet Alım Giderleri', HESAPKOD='9011603' WHERE YIL=2021 AND HESAPKOD='9011603'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Üretime Yönelik Mal ve Malzeme Alımları', HESAPKOD='901160301' WHERE YIL=2021 AND HESAPKOD='901160301'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Tüketime Yönelik Mal ve Malzeme Alımları', HESAPKOD='901160302' WHERE YIL=2021 AND HESAPKOD='901160302'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yolluklar', HESAPKOD='901160303' WHERE YIL=2021 AND HESAPKOD='901160303'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Görev Giderleri', HESAPKOD='901160304' WHERE YIL=2021 AND HESAPKOD='901160304'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Hizmet Alımları', HESAPKOD='901160305' WHERE YIL=2021 AND HESAPKOD='901160305'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Temsil ve Tanıtma Giderleri', HESAPKOD='901160306' WHERE YIL=2021 AND HESAPKOD='901160306'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Menkul Mal, Gayrimaddi Hak Alım, Bakım ve Onarım Giderleri', HESAPKOD='901160307' WHERE YIL=2021 AND HESAPKOD='901160307'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Gayrimenkul Mal Bakım ve Onarım Giderleri', HESAPKOD='901160308' WHERE YIL=2021 AND HESAPKOD='901160308'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Tedavi ve Cenaze Giderleri', HESAPKOD='901160309' WHERE YIL=2021 AND HESAPKOD='901160309'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Cari Transferler', HESAPKOD='9011605' WHERE YIL=2021 AND HESAPKOD='9011605'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Görevlendirme Giderleri', HESAPKOD='901160501' WHERE YIL=2021 AND HESAPKOD='901160501'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Hazine Yardımları', HESAPKOD='901160502' WHERE YIL=2021 AND HESAPKOD='901160502'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kar Amacı Gütmeyen Kuruluşlara Yapılan Transferler', HESAPKOD='901160503' WHERE YIL=2021 AND HESAPKOD='901160503'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Hane Halkı ve İşletmelere Yapılan Transferler', HESAPKOD='901160504' WHERE YIL=2021 AND HESAPKOD='901160504'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yurt Dışına Yapılan Transferler ', HESAPKOD='901160506' WHERE YIL=2021 AND HESAPKOD='901160506'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sermaye Giderleri', HESAPKOD='9011606' WHERE YIL=2021 AND HESAPKOD='9011606'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Mamul Mal Alımları', HESAPKOD='901160601' WHERE YIL=2021 AND HESAPKOD='901160601'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Menkul Sermaye Üretim Giderleri', HESAPKOD='901160602' WHERE YIL=2021 AND HESAPKOD='901160602'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Gayri Maddi Hak Alımları', HESAPKOD='901160603' WHERE YIL=2021 AND HESAPKOD='901160603'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Gayrimenkul Alımları ve Kamulaştırması', HESAPKOD='901160604' WHERE YIL=2021 AND HESAPKOD='901160604'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Gayrimenkul Sermaye Üretim Giderleri', HESAPKOD='901160605' WHERE YIL=2021 AND HESAPKOD='901160605'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Menkul Malların Büyük Onarım Giderleri', HESAPKOD='901160606' WHERE YIL=2021 AND HESAPKOD='901160606'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Gayrimenkul Büyük Onarım Giderleri', HESAPKOD='901160607' WHERE YIL=2021 AND HESAPKOD='901160607'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Stok Alımları', HESAPKOD='901160608' WHERE YIL=2021 AND HESAPKOD='901160608'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Sermaye Giderleri', HESAPKOD='901160609' WHERE YIL=2021 AND HESAPKOD='901160609'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sermaye Transferleri', HESAPKOD='9011607' WHERE YIL=2021 AND HESAPKOD='9011607'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kurum, İşletme ve Hanehalkına Yapılan Sermaye Transferleri', HESAPKOD='901160701' WHERE YIL=2021 AND HESAPKOD='901160701'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Borç Verme ', HESAPKOD='9011608' WHERE YIL=2021 AND HESAPKOD='9011608'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yurtiçi Borç Verme ', HESAPKOD='901160801' WHERE YIL=2021 AND HESAPKOD='901160801'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yılı İçinde Eklenen Özel Ödenek/Şartlı Bağış ve Yardım ', HESAPKOD='90117' WHERE YIL=2021 AND HESAPKOD='90117'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Personel Giderleri', HESAPKOD='9011701' WHERE YIL=2021 AND HESAPKOD='9011701'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Memurlar', HESAPKOD='901170101' WHERE YIL=2021 AND HESAPKOD='901170101'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='İşçiler', HESAPKOD='901170103' WHERE YIL=2021 AND HESAPKOD='901170103'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Geçici Süreli Çalışanlar', HESAPKOD='901170104' WHERE YIL=2021 AND HESAPKOD='901170104'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Personel', HESAPKOD='901170105' WHERE YIL=2021 AND HESAPKOD='901170105'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sosyal Güvenlik Kurumlarına Devlet Primi Giderleri', HESAPKOD='9011702' WHERE YIL=2021 AND HESAPKOD='9011702'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='İşçiler', HESAPKOD='901170203' WHERE YIL=2021 AND HESAPKOD='901170203'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Geçici Süreli Çalışanlar', HESAPKOD='901170204' WHERE YIL=2021 AND HESAPKOD='901170204'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Personel', HESAPKOD='901170205' WHERE YIL=2021 AND HESAPKOD='901170205'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Mal ve Hizmet Alım Giderleri', HESAPKOD='9011703' WHERE YIL=2021 AND HESAPKOD='9011703'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Üretime Yönelik Mal ve Malzeme Alımları', HESAPKOD='901170301' WHERE YIL=2021 AND HESAPKOD='901170301'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Tüketime Yönelik Mal ve Malzeme Alımları', HESAPKOD='901170302' WHERE YIL=2021 AND HESAPKOD='901170302'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yolluklar', HESAPKOD='901170303' WHERE YIL=2021 AND HESAPKOD='901170303'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Görev Giderleri', HESAPKOD='901170304' WHERE YIL=2021 AND HESAPKOD='901170304'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Hizmet Alımları', HESAPKOD='901170305' WHERE YIL=2021 AND HESAPKOD='901170305'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Temsil ve Tanıtma Giderleri', HESAPKOD='901170306' WHERE YIL=2021 AND HESAPKOD='901170306'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Menkul Mal, Gayrimaddi Hak Alım, Bakım ve Onarım Giderleri', HESAPKOD='901170307' WHERE YIL=2021 AND HESAPKOD='901170307'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Gayrimenkul Mal Bakım ve Onarım Giderleri', HESAPKOD='901170308' WHERE YIL=2021 AND HESAPKOD='901170308'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Tedavi ve Cenaze Giderleri', HESAPKOD='901170309' WHERE YIL=2021 AND HESAPKOD='901170309'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Cari Transferler', HESAPKOD='9011705' WHERE YIL=2021 AND HESAPKOD='9011705'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Görevlendirme Giderleri', HESAPKOD='901170501' WHERE YIL=2021 AND HESAPKOD='901170501'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Hazine Yardımları', HESAPKOD='901170502' WHERE YIL=2021 AND HESAPKOD='901170502'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kar Amacı Gütmeyen Kuruluşlara Yapılan Transferler', HESAPKOD='901170503' WHERE YIL=2021 AND HESAPKOD='901170503'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Hane Halkı ve İşletmelere Yapılan Transferler', HESAPKOD='901170504' WHERE YIL=2021 AND HESAPKOD='901170504'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yurt Dışına Yapılan Transferler ', HESAPKOD='901170506' WHERE YIL=2021 AND HESAPKOD='901170506'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sermaye Giderleri', HESAPKOD='9011706' WHERE YIL=2021 AND HESAPKOD='9011706'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Mamul Mal Alımları', HESAPKOD='901170601' WHERE YIL=2021 AND HESAPKOD='901170601'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Menkul Sermaye Üretim Giderleri', HESAPKOD='901170602' WHERE YIL=2021 AND HESAPKOD='901170602'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Gayri Maddi Hak Alımları', HESAPKOD='901170603' WHERE YIL=2021 AND HESAPKOD='901170603'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Gayrimenkul Alımları ve Kamulaştırması', HESAPKOD='901170604' WHERE YIL=2021 AND HESAPKOD='901170604'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Gayrimenkul Sermaye Üretim Giderleri', HESAPKOD='901170605' WHERE YIL=2021 AND HESAPKOD='901170605'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Menkul Malların Büyük Onarım Giderleri', HESAPKOD='901170606' WHERE YIL=2021 AND HESAPKOD='901170606'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Gayrimenkul Büyük Onarım Giderleri', HESAPKOD='901170607' WHERE YIL=2021 AND HESAPKOD='901170607'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Stok Alımları', HESAPKOD='901170608' WHERE YIL=2021 AND HESAPKOD='901170608'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Sermaye Giderleri', HESAPKOD='901170609' WHERE YIL=2021 AND HESAPKOD='901170609'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sermaye Transferleri', HESAPKOD='9011707' WHERE YIL=2021 AND HESAPKOD='9011707'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kurum, İşletme ve Hanehalkına Yapılan Sermaye Transferleri', HESAPKOD='901170701' WHERE YIL=2021 AND HESAPKOD='901170701'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Borç Verme ', HESAPKOD='9011708' WHERE YIL=2021 AND HESAPKOD='9011708'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yurtiçi Borç Verme ', HESAPKOD='901170801' WHERE YIL=2021 AND HESAPKOD='901170801'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yılı İçinde İptal Edilen Ödenek', HESAPKOD='90118' WHERE YIL=2021 AND HESAPKOD='90118'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Personel Giderleri', HESAPKOD='9011801' WHERE YIL=2021 AND HESAPKOD='9011801'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Memurlar', HESAPKOD='901180101' WHERE YIL=2021 AND HESAPKOD='901180101'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sözleşmeli Personel ', HESAPKOD='901180102' WHERE YIL=2021 AND HESAPKOD='901180102'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='İşçiler', HESAPKOD='901180103' WHERE YIL=2021 AND HESAPKOD='901180103'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Geçici Süreli Çalışanlar', HESAPKOD='901180104' WHERE YIL=2021 AND HESAPKOD='901180104'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Personel', HESAPKOD='901180105' WHERE YIL=2021 AND HESAPKOD='901180105'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Milletvekilleri', HESAPKOD='901180107' WHERE YIL=2021 AND HESAPKOD='901180107'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Cumhurbaşkanlığı Ödeneği', HESAPKOD='901180108' WHERE YIL=2021 AND HESAPKOD='901180108'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='İstihbarat Personeli', HESAPKOD='901180109' WHERE YIL=2021 AND HESAPKOD='901180109'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sosyal Güvenlik Kurumlarına Devlet Primi Giderleri', HESAPKOD='9011802' WHERE YIL=2021 AND HESAPKOD='9011802'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Memurlar', HESAPKOD='901180201' WHERE YIL=2021 AND HESAPKOD='901180201'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sözleşmeli Personel ', HESAPKOD='901180202' WHERE YIL=2021 AND HESAPKOD='901180202'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='İşçiler', HESAPKOD='901180203' WHERE YIL=2021 AND HESAPKOD='901180203'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Geçici Süreli Çalışanlar', HESAPKOD='901180204' WHERE YIL=2021 AND HESAPKOD='901180204'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Personel', HESAPKOD='901180205' WHERE YIL=2021 AND HESAPKOD='901180205'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Milletvekilleri', HESAPKOD='901180207' WHERE YIL=2021 AND HESAPKOD='901180207'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='İstihbarat Personeli', HESAPKOD='901180209' WHERE YIL=2021 AND HESAPKOD='901180209'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Mal ve Hizmet Alım Giderleri', HESAPKOD='9011803' WHERE YIL=2021 AND HESAPKOD='9011803'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Üretime Yönelik Mal ve Malzeme Alımları', HESAPKOD='901180301' WHERE YIL=2021 AND HESAPKOD='901180301'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Tüketime Yönelik Mal ve Malzeme Alımları', HESAPKOD='901180302' WHERE YIL=2021 AND HESAPKOD='901180302'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yolluklar', HESAPKOD='901180303' WHERE YIL=2021 AND HESAPKOD='901180303'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Görev Giderleri', HESAPKOD='901180304' WHERE YIL=2021 AND HESAPKOD='901180304'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Hizmet Alımları', HESAPKOD='901180305' WHERE YIL=2021 AND HESAPKOD='901180305'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Temsil ve Tanıtma Giderleri', HESAPKOD='901180306' WHERE YIL=2021 AND HESAPKOD='901180306'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Menkul Mal, Gayrimaddi Hak Alım, Bakım ve Onarım Giderleri', HESAPKOD='901180307' WHERE YIL=2021 AND HESAPKOD='901180307'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Gayrimenkul Mal Bakım ve Onarım Giderleri', HESAPKOD='901180308' WHERE YIL=2021 AND HESAPKOD='901180308'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Tedavi ve Cenaze Giderleri', HESAPKOD='901180309' WHERE YIL=2021 AND HESAPKOD='901180309'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Faiz Giderleri', HESAPKOD='9011804' WHERE YIL=2021 AND HESAPKOD='9011804'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kamu Kurumlarına Ödenen İç Borç Faiz Giderleri', HESAPKOD='901180401' WHERE YIL=2021 AND HESAPKOD='901180401'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer İç Borç Faiz Giderleri', HESAPKOD='901180402' WHERE YIL=2021 AND HESAPKOD='901180402'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Dış Borç Faiz Giderleri', HESAPKOD='901180403' WHERE YIL=2021 AND HESAPKOD='901180403'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='İskonto Giderleri', HESAPKOD='901180404' WHERE YIL=2021 AND HESAPKOD='901180404'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Para Piyasası Nakit İşlemleri Faiz Giderleri', HESAPKOD='901180405' WHERE YIL=2021 AND HESAPKOD='901180405'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Türev Ürün Giderleri', HESAPKOD='901180406' WHERE YIL=2021 AND HESAPKOD='901180406'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kira Sertifikası Giderleri', HESAPKOD='901180407' WHERE YIL=2021 AND HESAPKOD='901180407'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Cari Transferler', HESAPKOD='9011805' WHERE YIL=2021 AND HESAPKOD='9011805'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Görevlendirme Giderleri', HESAPKOD='901180501' WHERE YIL=2021 AND HESAPKOD='901180501'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Hazine Yardımları', HESAPKOD='901180502' WHERE YIL=2021 AND HESAPKOD='901180502'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kar Amacı Gütmeyen Kuruluşlara Yapılan Transferler', HESAPKOD='901180503' WHERE YIL=2021 AND HESAPKOD='901180503'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Hane Halkı ve İşletmelere Yapılan Transferler', HESAPKOD='901180504' WHERE YIL=2021 AND HESAPKOD='901180504'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Devlet Sosyal Güvenlik Kurumlarından Hane Halkına Yapılan Fayda Ödemeleri', HESAPKOD='901180505' WHERE YIL=2021 AND HESAPKOD='901180505'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yurt Dışına Yapılan Transferler ', HESAPKOD='901180506' WHERE YIL=2021 AND HESAPKOD='901180506'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Gelirlerden Ayrılan Paylar', HESAPKOD='901180508' WHERE YIL=2021 AND HESAPKOD='901180508'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sermaye Giderleri', HESAPKOD='9011806' WHERE YIL=2021 AND HESAPKOD='9011806'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Mamul Mal Alımları', HESAPKOD='901180601' WHERE YIL=2021 AND HESAPKOD='901180601'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Menkul Sermaye Üretim Giderleri', HESAPKOD='901180602' WHERE YIL=2021 AND HESAPKOD='901180602'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Gayri Maddi Hak Alımları', HESAPKOD='901180603' WHERE YIL=2021 AND HESAPKOD='901180603'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Gayrimenkul Alımları ve Kamulaştırması', HESAPKOD='901180604' WHERE YIL=2021 AND HESAPKOD='901180604'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Gayrimenkul Sermaye Üretim Giderleri', HESAPKOD='901180605' WHERE YIL=2021 AND HESAPKOD='901180605'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Menkul Malların Büyük Onarım Giderleri', HESAPKOD='901180606' WHERE YIL=2021 AND HESAPKOD='901180606'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Gayrimenkul Büyük Onarım Giderleri', HESAPKOD='901180607' WHERE YIL=2021 AND HESAPKOD='901180607'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Stok Alımları', HESAPKOD='901180608' WHERE YIL=2021 AND HESAPKOD='901180608'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Sermaye Giderleri', HESAPKOD='901180609' WHERE YIL=2021 AND HESAPKOD='901180609'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sermaye Transferleri', HESAPKOD='9011807' WHERE YIL=2021 AND HESAPKOD='9011807'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kurum, İşletme ve Hanehalkına Yapılan Sermaye Transferleri', HESAPKOD='901180701' WHERE YIL=2021 AND HESAPKOD='901180701'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yurtdışı Sermaye Transferleri', HESAPKOD='901180702' WHERE YIL=2021 AND HESAPKOD='901180702'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Borç Verme ', HESAPKOD='9011808' WHERE YIL=2021 AND HESAPKOD='9011808'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yurtiçi Borç Verme ', HESAPKOD='901180801' WHERE YIL=2021 AND HESAPKOD='901180801'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yurtdışı Borç Verme ', HESAPKOD='901180802' WHERE YIL=2021 AND HESAPKOD='901180802'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yedek Ödenekler', HESAPKOD='9011809' WHERE YIL=2021 AND HESAPKOD='9011809'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Personel Giderlerini Karşılama Ödeneği', HESAPKOD='901180901' WHERE YIL=2021 AND HESAPKOD='901180901'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kur Farklarını Karşılama Ödeneği', HESAPKOD='901180902' WHERE YIL=2021 AND HESAPKOD='901180902'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yatırımları Hızlandırma Ödeneği', HESAPKOD='901180903' WHERE YIL=2021 AND HESAPKOD='901180903'
DELETE FROM  MUHHESAPPLANI WHERE YIL=2021 AND HESAPKOD='901180904'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Doğal Afet Giderlerini Karşılama Ödeneği', HESAPKOD='901180905' WHERE YIL=2021 AND HESAPKOD='901180905'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yedek Ödenek', HESAPKOD='901180906' WHERE YIL=2021 AND HESAPKOD='901180906'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yeni Kurulacak Daire ve İdarelerin İhtiyaçlarını Karşılama Ödeneği', HESAPKOD='901180907' WHERE YIL=2021 AND HESAPKOD='901180907'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Mülteci ve Göçmen Giderleri Ödeneği', HESAPKOD='901180908' WHERE YIL=2021 AND HESAPKOD='901180908'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Yedek Ödenekler', HESAPKOD='901180909' WHERE YIL=2021 AND HESAPKOD='901180909'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Kanunlar Gereğince Ertesi Yıla Devredilen Ödenek', HESAPKOD='90119' WHERE YIL=2021 AND HESAPKOD='90119'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Personel Giderleri', HESAPKOD='9011901' WHERE YIL=2021 AND HESAPKOD='9011901'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Memurlar', HESAPKOD='901190101' WHERE YIL=2021 AND HESAPKOD='901190101'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sözleşmeli Personel ', HESAPKOD='901190102' WHERE YIL=2021 AND HESAPKOD='901190102'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='İşçiler', HESAPKOD='901190103' WHERE YIL=2021 AND HESAPKOD='901190103'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Geçici Süreli Çalışanlar', HESAPKOD='901190104' WHERE YIL=2021 AND HESAPKOD='901190104'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Personel', HESAPKOD='901190105' WHERE YIL=2021 AND HESAPKOD='901190105'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sosyal Güvenlik Kurumlarına Devlet Primi Giderleri', HESAPKOD='9011902' WHERE YIL=2021 AND HESAPKOD='9011902'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sözleşmeli Personel ', HESAPKOD='901190202' WHERE YIL=2021 AND HESAPKOD='901190202'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='İşçiler', HESAPKOD='901190203' WHERE YIL=2021 AND HESAPKOD='901190203'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Geçici Süreli Çalışanlar', HESAPKOD='901190204' WHERE YIL=2021 AND HESAPKOD='901190204'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Personel', HESAPKOD='901190205' WHERE YIL=2021 AND HESAPKOD='901190205'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Mal ve Hizmet Alım Giderleri', HESAPKOD='9011903' WHERE YIL=2021 AND HESAPKOD='9011903'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Üretime Yönelik Mal ve Malzeme Alımları', HESAPKOD='901190301' WHERE YIL=2021 AND HESAPKOD='901190301'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Tüketime Yönelik Mal ve Malzeme Alımları', HESAPKOD='901190302' WHERE YIL=2021 AND HESAPKOD='901190302'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yolluklar', HESAPKOD='901190303' WHERE YIL=2021 AND HESAPKOD='901190303'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Görev Giderleri', HESAPKOD='901190304' WHERE YIL=2021 AND HESAPKOD='901190304'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Hizmet Alımları', HESAPKOD='901190305' WHERE YIL=2021 AND HESAPKOD='901190305'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Temsil ve Tanıtma Giderleri', HESAPKOD='901190306' WHERE YIL=2021 AND HESAPKOD='901190306'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Menkul Mal, Gayrimaddi Hak Alım, Bakım ve Onarım Giderleri', HESAPKOD='901190307' WHERE YIL=2021 AND HESAPKOD='901190307'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Gayrimenkul Mal Bakım ve Onarım Giderleri', HESAPKOD='901190308' WHERE YIL=2021 AND HESAPKOD='901190308'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Tedavi ve Cenaze Giderleri', HESAPKOD='901190309' WHERE YIL=2021 AND HESAPKOD='901190309'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Cari Transferler', HESAPKOD='9011905' WHERE YIL=2021 AND HESAPKOD='9011905'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Hane Halkı ve İşletmelere Yapılan Transferler', HESAPKOD='901190504' WHERE YIL=2021 AND HESAPKOD='901190504'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yurt Dışına Yapılan Transferler ', HESAPKOD='901190506' WHERE YIL=2021 AND HESAPKOD='901190506'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sermaye Giderleri', HESAPKOD='9011906' WHERE YIL=2021 AND HESAPKOD='9011906'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Mamul Mal Alımları', HESAPKOD='901190601' WHERE YIL=2021 AND HESAPKOD='901190601'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Menkul Sermaye Üretim Giderleri', HESAPKOD='901190602' WHERE YIL=2021 AND HESAPKOD='901190602'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Gayri Maddi Hak Alımları', HESAPKOD='901190603' WHERE YIL=2021 AND HESAPKOD='901190603'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Gayrimenkul Alımları ve Kamulaştırması', HESAPKOD='901190604' WHERE YIL=2021 AND HESAPKOD='901190604'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Gayrimenkul Sermaye Üretim Giderleri', HESAPKOD='901190605' WHERE YIL=2021 AND HESAPKOD='901190605'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Menkul Malların Büyük Onarım Giderleri', HESAPKOD='901190606' WHERE YIL=2021 AND HESAPKOD='901190606'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Gayrimenkul Büyük Onarım Giderleri', HESAPKOD='901190607' WHERE YIL=2021 AND HESAPKOD='901190607'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Stok Alımları', HESAPKOD='901190608' WHERE YIL=2021 AND HESAPKOD='901190608'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Sermaye Giderleri', HESAPKOD='901190609' WHERE YIL=2021 AND HESAPKOD='901190609'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sermaye Transferleri', HESAPKOD='9011907' WHERE YIL=2021 AND HESAPKOD='9011907'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kurum, İşletme ve Hanehalkına Yapılan Sermaye Transferleri', HESAPKOD='901190701' WHERE YIL=2021 AND HESAPKOD='901190701'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yurtdışı Sermaye Transferleri', HESAPKOD='901190702' WHERE YIL=2021 AND HESAPKOD='901190702'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Borç Verme ', HESAPKOD='9011908' WHERE YIL=2021 AND HESAPKOD='9011908'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yurtiçi Borç Verme ', HESAPKOD='901190801' WHERE YIL=2021 AND HESAPKOD='901190801'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yurtdışı Borç Verme ', HESAPKOD='901190802' WHERE YIL=2021 AND HESAPKOD='901190802'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='5018 Sayılı Kanunun 35 inci Maddesi Gereğince Ertesi Yıla Devredilen Ödenek/Akreditif Artığı', HESAPKOD='90120' WHERE YIL=2021 AND HESAPKOD='90120'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Mal ve Hizmet Alım Giderleri', HESAPKOD='9012003' WHERE YIL=2021 AND HESAPKOD='9012003'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Üretime Yönelik Mal ve Malzeme Alımları', HESAPKOD='901200301' WHERE YIL=2021 AND HESAPKOD='901200301'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Tüketime Yönelik Mal ve Malzeme Alımları', HESAPKOD='901200302' WHERE YIL=2021 AND HESAPKOD='901200302'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yolluklar', HESAPKOD='901200303' WHERE YIL=2021 AND HESAPKOD='901200303'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Görev Giderleri', HESAPKOD='901200304' WHERE YIL=2021 AND HESAPKOD='901200304'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Hizmet Alımları', HESAPKOD='901200305' WHERE YIL=2021 AND HESAPKOD='901200305'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Temsil ve Tanıtma Giderleri', HESAPKOD='901200306' WHERE YIL=2021 AND HESAPKOD='901200306'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Menkul Mal, Gayrimaddi Hak Alım, Bakım ve Onarım Giderleri', HESAPKOD='901200307' WHERE YIL=2021 AND HESAPKOD='901200307'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Gayrimenkul Mal Bakım ve Onarım Giderleri', HESAPKOD='901200308' WHERE YIL=2021 AND HESAPKOD='901200308'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Tedavi ve Cenaze Giderleri', HESAPKOD='901200309' WHERE YIL=2021 AND HESAPKOD='901200309'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sermaye Giderleri', HESAPKOD='9012006' WHERE YIL=2021 AND HESAPKOD='9012006'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Mamul Mal Alımları', HESAPKOD='901200601' WHERE YIL=2021 AND HESAPKOD='901200601'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Menkul Sermaye Üretim Giderleri', HESAPKOD='901200602' WHERE YIL=2021 AND HESAPKOD='901200602'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Gayri Maddi Hak Alımları', HESAPKOD='901200603' WHERE YIL=2021 AND HESAPKOD='901200603'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Gayrimenkul Alımları ve Kamulaştırması', HESAPKOD='901200604' WHERE YIL=2021 AND HESAPKOD='901200604'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Gayrimenkul Sermaye Üretim Giderleri', HESAPKOD='901200605' WHERE YIL=2021 AND HESAPKOD='901200605'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Menkul Malların Büyük Onarım Giderleri', HESAPKOD='901200606' WHERE YIL=2021 AND HESAPKOD='901200606'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Gayrimenkul Büyük Onarım Giderleri', HESAPKOD='901200607' WHERE YIL=2021 AND HESAPKOD='901200607'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Stok Alımları', HESAPKOD='901200608' WHERE YIL=2021 AND HESAPKOD='901200608'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Sermaye Giderleri', HESAPKOD='901200609' WHERE YIL=2021 AND HESAPKOD='901200609'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='5018 Sayılı Kanunun 35 inci Maddesi Gereğince Ertesi Yıla Devredilen Ödenek/Taahhüt Artığı', HESAPKOD='90121' WHERE YIL=2021 AND HESAPKOD='90121'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Mal ve Hizmet Alım Giderleri', HESAPKOD='9012103' WHERE YIL=2021 AND HESAPKOD='9012103'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Üretime Yönelik Mal ve Malzeme Alımları', HESAPKOD='901210301' WHERE YIL=2021 AND HESAPKOD='901210301'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Tüketime Yönelik Mal ve Malzeme Alımları', HESAPKOD='901210302' WHERE YIL=2021 AND HESAPKOD='901210302'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yolluklar', HESAPKOD='901210303' WHERE YIL=2021 AND HESAPKOD='901210303'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Görev Giderleri', HESAPKOD='901210304' WHERE YIL=2021 AND HESAPKOD='901210304'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Hizmet Alımları', HESAPKOD='901210305' WHERE YIL=2021 AND HESAPKOD='901210305'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Temsil ve Tanıtma Giderleri', HESAPKOD='901210306' WHERE YIL=2021 AND HESAPKOD='901210306'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Menkul Mal, Gayrimaddi Hak Alım, Bakım ve Onarım Giderleri', HESAPKOD='901210307' WHERE YIL=2021 AND HESAPKOD='901210307'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Gayrimenkul Mal Bakım ve Onarım Giderleri', HESAPKOD='901210308' WHERE YIL=2021 AND HESAPKOD='901210308'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Tedavi ve Cenaze Giderleri', HESAPKOD='901210309' WHERE YIL=2021 AND HESAPKOD='901210309'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sermaye Giderleri', HESAPKOD='9012106' WHERE YIL=2021 AND HESAPKOD='9012106'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Mamul Mal Alımları', HESAPKOD='901210601' WHERE YIL=2021 AND HESAPKOD='901210601'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Menkul Sermaye Üretim Giderleri', HESAPKOD='901210602' WHERE YIL=2021 AND HESAPKOD='901210602'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Gayri Maddi Hak Alımları', HESAPKOD='901210603' WHERE YIL=2021 AND HESAPKOD='901210603'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Gayrimenkul Alımları ve Kamulaştırması', HESAPKOD='901210604' WHERE YIL=2021 AND HESAPKOD='901210604'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Gayrimenkul Sermaye Üretim Giderleri', HESAPKOD='901210605' WHERE YIL=2021 AND HESAPKOD='901210605'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Menkul Malların Büyük Onarım Giderleri', HESAPKOD='901210606' WHERE YIL=2021 AND HESAPKOD='901210606'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Gayrimenkul Büyük Onarım Giderleri', HESAPKOD='901210607' WHERE YIL=2021 AND HESAPKOD='901210607'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Stok Alımları', HESAPKOD='901210608' WHERE YIL=2021 AND HESAPKOD='901210608'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Sermaye Giderleri', HESAPKOD='901210609' WHERE YIL=2021 AND HESAPKOD='901210609'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='4749 SKanuna Göre Ertesi Yıla Devredilen Dış Proje Kredisi Akreditif ve Avans Artığı', HESAPKOD='90122' WHERE YIL=2021 AND HESAPKOD='90122'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Mal ve Hizmet Alım Giderleri', HESAPKOD='9012203' WHERE YIL=2021 AND HESAPKOD='9012203'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Üretime Yönelik Mal ve Malzeme Alımları', HESAPKOD='901220301' WHERE YIL=2021 AND HESAPKOD='901220301'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Tüketime Yönelik Mal ve Malzeme Alımları', HESAPKOD='901220302' WHERE YIL=2021 AND HESAPKOD='901220302'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yolluklar', HESAPKOD='901220303' WHERE YIL=2021 AND HESAPKOD='901220303'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Görev Giderleri', HESAPKOD='901220304' WHERE YIL=2021 AND HESAPKOD='901220304'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Hizmet Alımları', HESAPKOD='901220305' WHERE YIL=2021 AND HESAPKOD='901220305'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Temsil ve Tanıtma Giderleri', HESAPKOD='901220306' WHERE YIL=2021 AND HESAPKOD='901220306'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Menkul Mal, Gayrimaddi Hak Alım, Bakım ve Onarım Giderleri', HESAPKOD='901220307' WHERE YIL=2021 AND HESAPKOD='901220307'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Gayrimenkul Mal Bakım ve Onarım Giderleri', HESAPKOD='901220308' WHERE YIL=2021 AND HESAPKOD='901220308'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Tedavi ve Cenaze Giderleri', HESAPKOD='901220309' WHERE YIL=2021 AND HESAPKOD='901220309'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sermaye Giderleri', HESAPKOD='9012206' WHERE YIL=2021 AND HESAPKOD='9012206'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Mamul Mal Alımları', HESAPKOD='901220601' WHERE YIL=2021 AND HESAPKOD='901220601'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Menkul Sermaye Üretim Giderleri', HESAPKOD='901220602' WHERE YIL=2021 AND HESAPKOD='901220602'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Gayri Maddi Hak Alımları', HESAPKOD='901220603' WHERE YIL=2021 AND HESAPKOD='901220603'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Gayrimenkul Alımları ve Kamulaştırması', HESAPKOD='901220604' WHERE YIL=2021 AND HESAPKOD='901220604'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Gayrimenkul Sermaye Üretim Giderleri', HESAPKOD='901220605' WHERE YIL=2021 AND HESAPKOD='901220605'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Menkul Malların Büyük Onarım Giderleri', HESAPKOD='901220606' WHERE YIL=2021 AND HESAPKOD='901220606'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Gayrimenkul Büyük Onarım Giderleri', HESAPKOD='901220607' WHERE YIL=2021 AND HESAPKOD='901220607'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Stok Alımları', HESAPKOD='901220608' WHERE YIL=2021 AND HESAPKOD='901220608'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Sermaye Giderleri', HESAPKOD='901220609' WHERE YIL=2021 AND HESAPKOD='901220609'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='5018 Sayılı Kanunun 39 uncu Maddesi Gereğince Ertesi Yıla Devredilen Ödenek', HESAPKOD='90123' WHERE YIL=2021 AND HESAPKOD='90123'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Personel Giderleri', HESAPKOD='9012301' WHERE YIL=2021 AND HESAPKOD='9012301'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Memurlar', HESAPKOD='901230101' WHERE YIL=2021 AND HESAPKOD='901230101'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sözleşmeli Personel ', HESAPKOD='901230102' WHERE YIL=2021 AND HESAPKOD='901230102'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Geçici Süreli Çalışanlar', HESAPKOD='901230104' WHERE YIL=2021 AND HESAPKOD='901230104'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Mal ve Hizmet Alım Giderleri', HESAPKOD='9012303' WHERE YIL=2021 AND HESAPKOD='9012303'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Üretime Yönelik Mal ve Malzeme Alımları', HESAPKOD='901230301' WHERE YIL=2021 AND HESAPKOD='901230301'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Tüketime Yönelik Mal ve Malzeme Alımları', HESAPKOD='901230302' WHERE YIL=2021 AND HESAPKOD='901230302'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yolluklar', HESAPKOD='901230303' WHERE YIL=2021 AND HESAPKOD='901230303'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Görev Giderleri', HESAPKOD='901230304' WHERE YIL=2021 AND HESAPKOD='901230304'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Hizmet Alımları', HESAPKOD='901230305' WHERE YIL=2021 AND HESAPKOD='901230305'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Temsil ve Tanıtma Giderleri', HESAPKOD='901230306' WHERE YIL=2021 AND HESAPKOD='901230306'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Menkul Mal, Gayrimaddi Hak Alım, Bakım ve Onarım Giderleri', HESAPKOD='901230307' WHERE YIL=2021 AND HESAPKOD='901230307'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Gayrimenkul Mal Bakım ve Onarım Giderleri', HESAPKOD='901230308' WHERE YIL=2021 AND HESAPKOD='901230308'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Tedavi ve Cenaze Giderleri', HESAPKOD='901230309' WHERE YIL=2021 AND HESAPKOD='901230309'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Cari Transferler', HESAPKOD='9012305' WHERE YIL=2021 AND HESAPKOD='9012305'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kar Amacı Gütmeyen Kuruluşlara Yapılan Transferler', HESAPKOD='901230503' WHERE YIL=2021 AND HESAPKOD='901230503'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Hane Halkı ve İşletmelere Yapılan Transferler', HESAPKOD='901230504' WHERE YIL=2021 AND HESAPKOD='901230504'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yurt Dışına Yapılan Transferler ', HESAPKOD='901230506' WHERE YIL=2021 AND HESAPKOD='901230506'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sermaye Giderleri', HESAPKOD='9012306' WHERE YIL=2021 AND HESAPKOD='9012306'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Mamul Mal Alımları', HESAPKOD='901230601' WHERE YIL=2021 AND HESAPKOD='901230601'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Menkul Sermaye Üretim Giderleri', HESAPKOD='901230602' WHERE YIL=2021 AND HESAPKOD='901230602'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Gayri Maddi Hak Alımları', HESAPKOD='901230603' WHERE YIL=2021 AND HESAPKOD='901230603'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Gayrimenkul Alımları ve Kamulaştırması', HESAPKOD='901230604' WHERE YIL=2021 AND HESAPKOD='901230604'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Gayrimenkul Sermaye Üretim Giderleri', HESAPKOD='901230605' WHERE YIL=2021 AND HESAPKOD='901230605'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Menkul Malların Büyük Onarım Giderleri', HESAPKOD='901230606' WHERE YIL=2021 AND HESAPKOD='901230606'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Gayrimenkul Büyük Onarım Giderleri', HESAPKOD='901230607' WHERE YIL=2021 AND HESAPKOD='901230607'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Stok Alımları', HESAPKOD='901230608' WHERE YIL=2021 AND HESAPKOD='901230608'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Sermaye Giderleri', HESAPKOD='901230609' WHERE YIL=2021 AND HESAPKOD='901230609'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sermaye Transferleri', HESAPKOD='9012307' WHERE YIL=2021 AND HESAPKOD='9012307'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kurum, İşletme ve Hanehalkına Yapılan Sermaye Transferleri', HESAPKOD='901230701' WHERE YIL=2021 AND HESAPKOD='901230701'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Borç Verme ', HESAPKOD='9012308' WHERE YIL=2021 AND HESAPKOD='9012308'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yurtiçi Borç Verme ', HESAPKOD='901230801' WHERE YIL=2021 AND HESAPKOD='901230801'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='5018 Sayılı Kanunun 40 ıncı Maddesi Gereğince Ertesi Yıla Devredilen Ödenek', HESAPKOD='90124' WHERE YIL=2021 AND HESAPKOD='90124'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Personel Giderleri', HESAPKOD='9012401' WHERE YIL=2021 AND HESAPKOD='9012401'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Memurlar', HESAPKOD='901240101' WHERE YIL=2021 AND HESAPKOD='901240101'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='İşçiler', HESAPKOD='901240103' WHERE YIL=2021 AND HESAPKOD='901240103'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Geçici Süreli Çalışanlar', HESAPKOD='901240104' WHERE YIL=2021 AND HESAPKOD='901240104'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sosyal Güvenlik Kurumlarına Devlet Primi Giderleri', HESAPKOD='9012402' WHERE YIL=2021 AND HESAPKOD='9012402'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Geçici Süreli Çalışanlar', HESAPKOD='901240204' WHERE YIL=2021 AND HESAPKOD='901240204'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Mal ve Hizmet Alım Giderleri', HESAPKOD='9012403' WHERE YIL=2021 AND HESAPKOD='9012403'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Üretime Yönelik Mal ve Malzeme Alımları', HESAPKOD='901240301' WHERE YIL=2021 AND HESAPKOD='901240301'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Tüketime Yönelik Mal ve Malzeme Alımları', HESAPKOD='901240302' WHERE YIL=2021 AND HESAPKOD='901240302'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yolluklar', HESAPKOD='901240303' WHERE YIL=2021 AND HESAPKOD='901240303'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Görev Giderleri', HESAPKOD='901240304' WHERE YIL=2021 AND HESAPKOD='901240304'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Hizmet Alımları', HESAPKOD='901240305' WHERE YIL=2021 AND HESAPKOD='901240305'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Temsil ve Tanıtma Giderleri', HESAPKOD='901240306' WHERE YIL=2021 AND HESAPKOD='901240306'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Menkul Mal, Gayrimaddi Hak Alım, Bakım ve Onarım Giderleri', HESAPKOD='901240307' WHERE YIL=2021 AND HESAPKOD='901240307'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Gayrimenkul Mal Bakım ve Onarım Giderleri', HESAPKOD='901240308' WHERE YIL=2021 AND HESAPKOD='901240308'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Tedavi ve Cenaze Giderleri', HESAPKOD='901240309' WHERE YIL=2021 AND HESAPKOD='901240309'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Cari Transferler', HESAPKOD='9012405' WHERE YIL=2021 AND HESAPKOD='9012405'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kar Amacı Gütmeyen Kuruluşlara Yapılan Transferler', HESAPKOD='901240503' WHERE YIL=2021 AND HESAPKOD='901240503'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Hane Halkı ve İşletmelere Yapılan Transferler', HESAPKOD='901240504' WHERE YIL=2021 AND HESAPKOD='901240504'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yurt Dışına Yapılan Transferler ', HESAPKOD='901240506' WHERE YIL=2021 AND HESAPKOD='901240506'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sermaye Giderleri', HESAPKOD='9012406' WHERE YIL=2021 AND HESAPKOD='9012406'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Mamul Mal Alımları', HESAPKOD='901240601' WHERE YIL=2021 AND HESAPKOD='901240601'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Menkul Sermaye Üretim Giderleri', HESAPKOD='901240602' WHERE YIL=2021 AND HESAPKOD='901240602'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Gayri Maddi Hak Alımları', HESAPKOD='901240603' WHERE YIL=2021 AND HESAPKOD='901240603'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Gayrimenkul Alımları ve Kamulaştırması', HESAPKOD='901240604' WHERE YIL=2021 AND HESAPKOD='901240604'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Gayrimenkul Sermaye Üretim Giderleri', HESAPKOD='901240605' WHERE YIL=2021 AND HESAPKOD='901240605'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Menkul Malların Büyük Onarım Giderleri', HESAPKOD='901240606' WHERE YIL=2021 AND HESAPKOD='901240606'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Gayrimenkul Büyük Onarım Giderleri', HESAPKOD='901240607' WHERE YIL=2021 AND HESAPKOD='901240607'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Stok Alımları', HESAPKOD='901240608' WHERE YIL=2021 AND HESAPKOD='901240608'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Sermaye Giderleri', HESAPKOD='901240609' WHERE YIL=2021 AND HESAPKOD='901240609'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sermaye Transferleri', HESAPKOD='9012407' WHERE YIL=2021 AND HESAPKOD='9012407'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kurum, İşletme ve Hanehalkına Yapılan Sermaye Transferleri', HESAPKOD='901240701' WHERE YIL=2021 AND HESAPKOD='901240701'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Borç Verme ', HESAPKOD='9012408' WHERE YIL=2021 AND HESAPKOD='9012408'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yurtiçi Borç Verme ', HESAPKOD='901240801' WHERE YIL=2021 AND HESAPKOD='901240801'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yıl Sonunda İptal Edilen Ödenek', HESAPKOD='90125' WHERE YIL=2021 AND HESAPKOD='90125'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Personel Giderleri', HESAPKOD='9012501' WHERE YIL=2021 AND HESAPKOD='9012501'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Memurlar', HESAPKOD='901250101' WHERE YIL=2021 AND HESAPKOD='901250101'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sözleşmeli Personel ', HESAPKOD='901250102' WHERE YIL=2021 AND HESAPKOD='901250102'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='İşçiler', HESAPKOD='901250103' WHERE YIL=2021 AND HESAPKOD='901250103'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Geçici Süreli Çalışanlar', HESAPKOD='901250104' WHERE YIL=2021 AND HESAPKOD='901250104'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Personel', HESAPKOD='901250105' WHERE YIL=2021 AND HESAPKOD='901250105'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Milletvekilleri', HESAPKOD='901250107' WHERE YIL=2021 AND HESAPKOD='901250107'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Cumhurbaşkanlığı Ödeneği', HESAPKOD='901250108' WHERE YIL=2021 AND HESAPKOD='901250108'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='İstihbarat Personeli', HESAPKOD='901250109' WHERE YIL=2021 AND HESAPKOD='901250109'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sosyal Güvenlik Kurumlarına Devlet Primi Giderleri', HESAPKOD='9012502' WHERE YIL=2021 AND HESAPKOD='9012502'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Memurlar', HESAPKOD='901250201' WHERE YIL=2021 AND HESAPKOD='901250201'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sözleşmeli Personel ', HESAPKOD='901250202' WHERE YIL=2021 AND HESAPKOD='901250202'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='İşçiler', HESAPKOD='901250203' WHERE YIL=2021 AND HESAPKOD='901250203'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Geçici Süreli Çalışanlar', HESAPKOD='901250204' WHERE YIL=2021 AND HESAPKOD='901250204'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Personel', HESAPKOD='901250205' WHERE YIL=2021 AND HESAPKOD='901250205'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Milletvekilleri', HESAPKOD='901250207' WHERE YIL=2021 AND HESAPKOD='901250207'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='İstihbarat Personeli', HESAPKOD='901250209' WHERE YIL=2021 AND HESAPKOD='901250209'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Mal ve Hizmet Alım Giderleri', HESAPKOD='9012503' WHERE YIL=2021 AND HESAPKOD='9012503'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Üretime Yönelik Mal ve Malzeme Alımları', HESAPKOD='901250301' WHERE YIL=2021 AND HESAPKOD='901250301'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Tüketime Yönelik Mal ve Malzeme Alımları', HESAPKOD='901250302' WHERE YIL=2021 AND HESAPKOD='901250302'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yolluklar', HESAPKOD='901250303' WHERE YIL=2021 AND HESAPKOD='901250303'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Görev Giderleri', HESAPKOD='901250304' WHERE YIL=2021 AND HESAPKOD='901250304'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Hizmet Alımları', HESAPKOD='901250305' WHERE YIL=2021 AND HESAPKOD='901250305'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Temsil ve Tanıtma Giderleri', HESAPKOD='901250306' WHERE YIL=2021 AND HESAPKOD='901250306'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Menkul Mal, Gayrimaddi Hak Alım, Bakım ve Onarım Giderleri', HESAPKOD='901250307' WHERE YIL=2021 AND HESAPKOD='901250307'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Gayrimenkul Mal Bakım ve Onarım Giderleri', HESAPKOD='901250308' WHERE YIL=2021 AND HESAPKOD='901250308'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Tedavi ve Cenaze Giderleri', HESAPKOD='901250309' WHERE YIL=2021 AND HESAPKOD='901250309'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Faiz Giderleri', HESAPKOD='9012504' WHERE YIL=2021 AND HESAPKOD='9012504'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kamu Kurumlarına Ödenen İç Borç Faiz Giderleri', HESAPKOD='901250401' WHERE YIL=2021 AND HESAPKOD='901250401'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer İç Borç Faiz Giderleri', HESAPKOD='901250402' WHERE YIL=2021 AND HESAPKOD='901250402'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Dış Borç Faiz Giderleri', HESAPKOD='901250403' WHERE YIL=2021 AND HESAPKOD='901250403'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='İskonto Giderleri', HESAPKOD='901250404' WHERE YIL=2021 AND HESAPKOD='901250404'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Para Piyasası Nakit İşlemleri Faiz Giderleri', HESAPKOD='901250405' WHERE YIL=2021 AND HESAPKOD='901250405'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Türev Ürün Giderleri', HESAPKOD='901250406' WHERE YIL=2021 AND HESAPKOD='901250406'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kira Sertifikası Giderleri', HESAPKOD='901250407' WHERE YIL=2021 AND HESAPKOD='901250407'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Cari Transferler', HESAPKOD='9012505' WHERE YIL=2021 AND HESAPKOD='9012505'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Görevlendirme Giderleri', HESAPKOD='901250501' WHERE YIL=2021 AND HESAPKOD='901250501'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Hazine Yardımları', HESAPKOD='901250502' WHERE YIL=2021 AND HESAPKOD='901250502'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kar Amacı Gütmeyen Kuruluşlara Yapılan Transferler', HESAPKOD='901250503' WHERE YIL=2021 AND HESAPKOD='901250503'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Hane Halkı ve İşletmelere Yapılan Transferler', HESAPKOD='901250504' WHERE YIL=2021 AND HESAPKOD='901250504'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Devlet Sosyal Güvenlik Kurumlarından Hane Halkına Yapılan Fayda Ödemeleri', HESAPKOD='901250505' WHERE YIL=2021 AND HESAPKOD='901250505'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yurt Dışına Yapılan Transferler ', HESAPKOD='901250506' WHERE YIL=2021 AND HESAPKOD='901250506'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Gelirlerden Ayrılan Paylar', HESAPKOD='901250508' WHERE YIL=2021 AND HESAPKOD='901250508'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sermaye Giderleri', HESAPKOD='9012506' WHERE YIL=2021 AND HESAPKOD='9012506'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Mamul Mal Alımları', HESAPKOD='901250601' WHERE YIL=2021 AND HESAPKOD='901250601'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Menkul Sermaye Üretim Giderleri', HESAPKOD='901250602' WHERE YIL=2021 AND HESAPKOD='901250602'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Gayri Maddi Hak Alımları', HESAPKOD='901250603' WHERE YIL=2021 AND HESAPKOD='901250603'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Gayrimenkul Alımları ve Kamulaştırması', HESAPKOD='901250604' WHERE YIL=2021 AND HESAPKOD='901250604'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Gayrimenkul Sermaye Üretim Giderleri', HESAPKOD='901250605' WHERE YIL=2021 AND HESAPKOD='901250605'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Menkul Malların Büyük Onarım Giderleri', HESAPKOD='901250606' WHERE YIL=2021 AND HESAPKOD='901250606'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Gayrimenkul Büyük Onarım Giderleri', HESAPKOD='901250607' WHERE YIL=2021 AND HESAPKOD='901250607'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Stok Alımları', HESAPKOD='901250608' WHERE YIL=2021 AND HESAPKOD='901250608'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Sermaye Giderleri', HESAPKOD='901250609' WHERE YIL=2021 AND HESAPKOD='901250609'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sermaye Transferleri', HESAPKOD='9012507' WHERE YIL=2021 AND HESAPKOD='9012507'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kurum, İşletme ve Hanehalkına Yapılan Sermaye Transferleri', HESAPKOD='901250701' WHERE YIL=2021 AND HESAPKOD='901250701'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yurtdışı Sermaye Transferleri', HESAPKOD='901250702' WHERE YIL=2021 AND HESAPKOD='901250702'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Borç Verme ', HESAPKOD='9012508' WHERE YIL=2021 AND HESAPKOD='9012508'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yurtiçi Borç Verme ', HESAPKOD='901250801' WHERE YIL=2021 AND HESAPKOD='901250801'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yurtdışı Borç Verme ', HESAPKOD='901250802' WHERE YIL=2021 AND HESAPKOD='901250802'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yedek Ödenekler', HESAPKOD='9012509' WHERE YIL=2021 AND HESAPKOD='9012509'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Personel Giderlerini Karşılama Ödeneği', HESAPKOD='901250901' WHERE YIL=2021 AND HESAPKOD='901250901'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kur Farklarını Karşılama Ödeneği', HESAPKOD='901250902' WHERE YIL=2021 AND HESAPKOD='901250902'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yatırımları Hızlandırma Ödeneği', HESAPKOD='901250903' WHERE YIL=2021 AND HESAPKOD='901250903'
DELETE FROM  MUHHESAPPLANI WHERE YIL=2021 AND HESAPKOD='901250904'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Doğal Afet Giderlerini Karşılama Ödeneği', HESAPKOD='901250905' WHERE YIL=2021 AND HESAPKOD='901250905'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yedek Ödenek', HESAPKOD='901250906' WHERE YIL=2021 AND HESAPKOD='901250906'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yeni Kurulacak Daire ve İdarelerin İhtiyaçlarını Karşılama Ödeneği', HESAPKOD='901250907' WHERE YIL=2021 AND HESAPKOD='901250907'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Mülteci ve Göçmen Giderleri Ödeneği', HESAPKOD='901250908' WHERE YIL=2021 AND HESAPKOD='901250908'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Yedek Ödenekler', HESAPKOD='901250909' WHERE YIL=2021 AND HESAPKOD='901250909'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='BÜTÇE ÖDENEK HAREKETLERİ HESABI', HESAPKOD='902' WHERE YIL=2021 AND HESAPKOD='902'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Personel Giderleri', HESAPKOD='90201' WHERE YIL=2021 AND HESAPKOD='90201'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Memurlar', HESAPKOD='9020101' WHERE YIL=2021 AND HESAPKOD='9020101'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sözleşmeli Personel ', HESAPKOD='9020102' WHERE YIL=2021 AND HESAPKOD='9020102'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='İşçiler', HESAPKOD='9020103' WHERE YIL=2021 AND HESAPKOD='9020103'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Geçici Süreli Çalışanlar', HESAPKOD='9020104' WHERE YIL=2021 AND HESAPKOD='9020104'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Personel', HESAPKOD='9020105' WHERE YIL=2021 AND HESAPKOD='9020105'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Milletvekilleri', HESAPKOD='9020107' WHERE YIL=2021 AND HESAPKOD='9020107'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Cumhurbaşkanlığı Ödeneği', HESAPKOD='9020108' WHERE YIL=2021 AND HESAPKOD='9020108'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='İstihbarat Personeli', HESAPKOD='9020109' WHERE YIL=2021 AND HESAPKOD='9020109'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sosyal Güvenlik Kurumlarına Devlet Primi Giderleri', HESAPKOD='90202' WHERE YIL=2021 AND HESAPKOD='90202'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Memurlar', HESAPKOD='9020201' WHERE YIL=2021 AND HESAPKOD='9020201'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sözleşmeli Personel ', HESAPKOD='9020202' WHERE YIL=2021 AND HESAPKOD='9020202'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='İşçiler', HESAPKOD='9020203' WHERE YIL=2021 AND HESAPKOD='9020203'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Geçici Süreli Çalışanlar', HESAPKOD='9020204' WHERE YIL=2021 AND HESAPKOD='9020204'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Personel', HESAPKOD='9020205' WHERE YIL=2021 AND HESAPKOD='9020205'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Milletvekilleri', HESAPKOD='9020207' WHERE YIL=2021 AND HESAPKOD='9020207'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='İstihbarat Personeli', HESAPKOD='9020209' WHERE YIL=2021 AND HESAPKOD='9020209'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Mal ve Hizmet Alım Giderleri', HESAPKOD='90203' WHERE YIL=2021 AND HESAPKOD='90203'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Üretime Yönelik Mal ve Malzeme Alımları', HESAPKOD='9020301' WHERE YIL=2021 AND HESAPKOD='9020301'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Tüketime Yönelik Mal ve Malzeme Alımları', HESAPKOD='9020302' WHERE YIL=2021 AND HESAPKOD='9020302'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yolluklar', HESAPKOD='9020303' WHERE YIL=2021 AND HESAPKOD='9020303'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Görev Giderleri', HESAPKOD='9020304' WHERE YIL=2021 AND HESAPKOD='9020304'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Hizmet Alımları', HESAPKOD='9020305' WHERE YIL=2021 AND HESAPKOD='9020305'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Temsil ve Tanıtma Giderleri', HESAPKOD='9020306' WHERE YIL=2021 AND HESAPKOD='9020306'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Menkul Mal, Gayrimaddi Hak Alım, Bakım ve Onarım Giderleri', HESAPKOD='9020307' WHERE YIL=2021 AND HESAPKOD='9020307'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Gayrimenkul Mal Bakım ve Onarım Giderleri', HESAPKOD='9020308' WHERE YIL=2021 AND HESAPKOD='9020308'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Tedavi ve Cenaze Giderleri', HESAPKOD='9020309' WHERE YIL=2021 AND HESAPKOD='9020309'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Faiz Giderleri', HESAPKOD='90204' WHERE YIL=2021 AND HESAPKOD='90204'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kamu Kurumlarına Ödenen İç Borç Faiz Giderleri', HESAPKOD='9020401' WHERE YIL=2021 AND HESAPKOD='9020401'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer İç Borç Faiz Giderleri', HESAPKOD='9020402' WHERE YIL=2021 AND HESAPKOD='9020402'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Dış Borç Faiz Giderleri', HESAPKOD='9020403' WHERE YIL=2021 AND HESAPKOD='9020403'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='İskonto Giderleri', HESAPKOD='9020404' WHERE YIL=2021 AND HESAPKOD='9020404'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Para Piyasası Nakit İşlemleri Faiz Giderleri', HESAPKOD='9020405' WHERE YIL=2021 AND HESAPKOD='9020405'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Türev Ürün Giderleri', HESAPKOD='9020406' WHERE YIL=2021 AND HESAPKOD='9020406'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kira Sertifikası Giderleri', HESAPKOD='9020407' WHERE YIL=2021 AND HESAPKOD='9020407'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Cari Transferler', HESAPKOD='90205' WHERE YIL=2021 AND HESAPKOD='90205'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Görevlendirme Giderleri', HESAPKOD='9020501' WHERE YIL=2021 AND HESAPKOD='9020501'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Hazine Yardımları', HESAPKOD='9020502' WHERE YIL=2021 AND HESAPKOD='9020502'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kar Amacı Gütmeyen Kuruluşlara Yapılan Transferler', HESAPKOD='9020503' WHERE YIL=2021 AND HESAPKOD='9020503'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Hane Halkı ve İşletmelere Yapılan Transferler', HESAPKOD='9020504' WHERE YIL=2021 AND HESAPKOD='9020504'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Devlet Sosyal Güvenlik Kurumlarından Hane Halkına Yapılan Fayda Ödemeleri', HESAPKOD='9020505' WHERE YIL=2021 AND HESAPKOD='9020505'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yurt Dışına Yapılan Transferler ', HESAPKOD='9020506' WHERE YIL=2021 AND HESAPKOD='9020506'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Gelirlerden Ayrılan Paylar', HESAPKOD='9020508' WHERE YIL=2021 AND HESAPKOD='9020508'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sermaye Giderleri', HESAPKOD='90206' WHERE YIL=2021 AND HESAPKOD='90206'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Mamul Mal Alımları', HESAPKOD='9020601' WHERE YIL=2021 AND HESAPKOD='9020601'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Menkul Sermaye Üretim Giderleri', HESAPKOD='9020602' WHERE YIL=2021 AND HESAPKOD='9020602'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Gayri Maddi Hak Alımları', HESAPKOD='9020603' WHERE YIL=2021 AND HESAPKOD='9020603'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Gayrimenkul Alımları ve Kamulaştırması', HESAPKOD='9020604' WHERE YIL=2021 AND HESAPKOD='9020604'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Gayrimenkul Sermaye Üretim Giderleri', HESAPKOD='9020605' WHERE YIL=2021 AND HESAPKOD='9020605'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Menkul Malların Büyük Onarım Giderleri', HESAPKOD='9020606' WHERE YIL=2021 AND HESAPKOD='9020606'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Gayrimenkul Büyük Onarım Giderleri', HESAPKOD='9020607' WHERE YIL=2021 AND HESAPKOD='9020607'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Stok Alımları', HESAPKOD='9020608' WHERE YIL=2021 AND HESAPKOD='9020608'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Sermaye Giderleri', HESAPKOD='9020609' WHERE YIL=2021 AND HESAPKOD='9020609'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sermaye Transferleri', HESAPKOD='90207' WHERE YIL=2021 AND HESAPKOD='90207'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kurum, İşletme ve Hanehalkına Yapılan Sermaye Transferleri', HESAPKOD='9020701' WHERE YIL=2021 AND HESAPKOD='9020701'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yurtdışı Sermaye Transferleri', HESAPKOD='9020702' WHERE YIL=2021 AND HESAPKOD='9020702'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Borç Verme ', HESAPKOD='90208' WHERE YIL=2021 AND HESAPKOD='90208'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yurtiçi Borç Verme ', HESAPKOD='9020801' WHERE YIL=2021 AND HESAPKOD='9020801'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yurtdışı Borç Verme ', HESAPKOD='9020802' WHERE YIL=2021 AND HESAPKOD='9020802'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yedek Ödenekler', HESAPKOD='90209' WHERE YIL=2021 AND HESAPKOD='90209'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Personel Giderlerini Karşılama Ödeneği', HESAPKOD='9020901' WHERE YIL=2021 AND HESAPKOD='9020901'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kur Farklarını Karşılama Ödeneği', HESAPKOD='9020902' WHERE YIL=2021 AND HESAPKOD='9020902'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yatırımları Hızlandırma Ödeneği', HESAPKOD='9020903' WHERE YIL=2021 AND HESAPKOD='9020903'
DELETE FROM  MUHHESAPPLANI WHERE YIL=2021 AND HESAPKOD='9020904'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Doğal Afet Giderlerini Karşılama Ödeneği', HESAPKOD='9020905' WHERE YIL=2021 AND HESAPKOD='9020905'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yedek Ödenek', HESAPKOD='9020906' WHERE YIL=2021 AND HESAPKOD='9020906'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yeni Kurulacak Daire ve İdarelerin İhtiyaçlarını Karşılama Ödeneği', HESAPKOD='9020907' WHERE YIL=2021 AND HESAPKOD='9020907'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Mülteci ve Göçmen Giderleri Ödeneği', HESAPKOD='9020908' WHERE YIL=2021 AND HESAPKOD='9020908'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Yedek Ödenekler', HESAPKOD='9020909' WHERE YIL=2021 AND HESAPKOD='9020909'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='KULLANILACAK ÖDENEKLER HESABI', HESAPKOD='903' WHERE YIL=2021 AND HESAPKOD='903'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Personel Giderleri', HESAPKOD='90301' WHERE YIL=2021 AND HESAPKOD='90301'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Memurlar', HESAPKOD='9030101' WHERE YIL=2021 AND HESAPKOD='9030101'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sözleşmeli Personel ', HESAPKOD='9030102' WHERE YIL=2021 AND HESAPKOD='9030102'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='İşçiler', HESAPKOD='9030103' WHERE YIL=2021 AND HESAPKOD='9030103'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Geçici Süreli Çalışanlar', HESAPKOD='9030104' WHERE YIL=2021 AND HESAPKOD='9030104'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Personel', HESAPKOD='9030105' WHERE YIL=2021 AND HESAPKOD='9030105'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Milletvekilleri', HESAPKOD='9030107' WHERE YIL=2021 AND HESAPKOD='9030107'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Cumhurbaşkanlığı Ödeneği', HESAPKOD='9030108' WHERE YIL=2021 AND HESAPKOD='9030108'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='İstihbarat Personeli', HESAPKOD='9030109' WHERE YIL=2021 AND HESAPKOD='9030109'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sosyal Güvenlik Kurumlarına Devlet Primi Giderleri', HESAPKOD='90302' WHERE YIL=2021 AND HESAPKOD='90302'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Memurlar', HESAPKOD='9030201' WHERE YIL=2021 AND HESAPKOD='9030201'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sözleşmeli Personel ', HESAPKOD='9030202' WHERE YIL=2021 AND HESAPKOD='9030202'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='İşçiler', HESAPKOD='9030203' WHERE YIL=2021 AND HESAPKOD='9030203'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Geçici Süreli Çalışanlar', HESAPKOD='9030204' WHERE YIL=2021 AND HESAPKOD='9030204'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Personel', HESAPKOD='9030205' WHERE YIL=2021 AND HESAPKOD='9030205'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Milletvekilleri', HESAPKOD='9030207' WHERE YIL=2021 AND HESAPKOD='9030207'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='İstihbarat Personeli', HESAPKOD='9030209' WHERE YIL=2021 AND HESAPKOD='9030209'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Mal ve Hizmet Alım Giderleri', HESAPKOD='90303' WHERE YIL=2021 AND HESAPKOD='90303'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Üretime Yönelik Mal ve Malzeme Alımları', HESAPKOD='9030301' WHERE YIL=2021 AND HESAPKOD='9030301'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Tüketime Yönelik Mal ve Malzeme Alımları', HESAPKOD='9030302' WHERE YIL=2021 AND HESAPKOD='9030302'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yolluklar', HESAPKOD='9030303' WHERE YIL=2021 AND HESAPKOD='9030303'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Görev Giderleri', HESAPKOD='9030304' WHERE YIL=2021 AND HESAPKOD='9030304'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Hizmet Alımları', HESAPKOD='9030305' WHERE YIL=2021 AND HESAPKOD='9030305'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Temsil ve Tanıtma Giderleri', HESAPKOD='9030306' WHERE YIL=2021 AND HESAPKOD='9030306'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Menkul Mal, Gayrimaddi Hak Alım, Bakım ve Onarım Giderleri', HESAPKOD='9030307' WHERE YIL=2021 AND HESAPKOD='9030307'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Gayrimenkul Mal Bakım ve Onarım Giderleri', HESAPKOD='9030308' WHERE YIL=2021 AND HESAPKOD='9030308'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Tedavi ve Cenaze Giderleri', HESAPKOD='9030309' WHERE YIL=2021 AND HESAPKOD='9030309'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Faiz Giderleri', HESAPKOD='90304' WHERE YIL=2021 AND HESAPKOD='90304'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kamu Kurumlarına Ödenen İç Borç Faiz Giderleri', HESAPKOD='9030401' WHERE YIL=2021 AND HESAPKOD='9030401'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer İç Borç Faiz Giderleri', HESAPKOD='9030402' WHERE YIL=2021 AND HESAPKOD='9030402'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Dış Borç Faiz Giderleri', HESAPKOD='9030403' WHERE YIL=2021 AND HESAPKOD='9030403'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='İskonto Giderleri', HESAPKOD='9030404' WHERE YIL=2021 AND HESAPKOD='9030404'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Para Piyasası Nakit İşlemleri Faiz Giderleri', HESAPKOD='9030405' WHERE YIL=2021 AND HESAPKOD='9030405'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Türev Ürün Giderleri', HESAPKOD='9030406' WHERE YIL=2021 AND HESAPKOD='9030406'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kira Sertifikası Giderleri', HESAPKOD='9030407' WHERE YIL=2021 AND HESAPKOD='9030407'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Cari Transferler', HESAPKOD='90305' WHERE YIL=2021 AND HESAPKOD='90305'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Görevlendirme Giderleri', HESAPKOD='9030501' WHERE YIL=2021 AND HESAPKOD='9030501'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Hazine Yardımları', HESAPKOD='9030502' WHERE YIL=2021 AND HESAPKOD='9030502'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kar Amacı Gütmeyen Kuruluşlara Yapılan Transferler', HESAPKOD='9030503' WHERE YIL=2021 AND HESAPKOD='9030503'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Hane Halkı ve İşletmelere Yapılan Transferler', HESAPKOD='9030504' WHERE YIL=2021 AND HESAPKOD='9030504'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Devlet Sosyal Güvenlik Kurumlarından Hane Halkına Yapılan Fayda Ödemeleri', HESAPKOD='9030505' WHERE YIL=2021 AND HESAPKOD='9030505'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yurt Dışına Yapılan Transferler ', HESAPKOD='9030506' WHERE YIL=2021 AND HESAPKOD='9030506'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Gelirlerden Ayrılan Paylar', HESAPKOD='9030508' WHERE YIL=2021 AND HESAPKOD='9030508'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sermaye Giderleri', HESAPKOD='90306' WHERE YIL=2021 AND HESAPKOD='90306'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Mamul Mal Alımları', HESAPKOD='9030601' WHERE YIL=2021 AND HESAPKOD='9030601'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Menkul Sermaye Üretim Giderleri', HESAPKOD='9030602' WHERE YIL=2021 AND HESAPKOD='9030602'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Gayri Maddi Hak Alımları', HESAPKOD='9030603' WHERE YIL=2021 AND HESAPKOD='9030603'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Gayrimenkul Alımları ve Kamulaştırması', HESAPKOD='9030604' WHERE YIL=2021 AND HESAPKOD='9030604'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Gayrimenkul Sermaye Üretim Giderleri', HESAPKOD='9030605' WHERE YIL=2021 AND HESAPKOD='9030605'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Menkul Malların Büyük Onarım Giderleri', HESAPKOD='9030606' WHERE YIL=2021 AND HESAPKOD='9030606'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Gayrimenkul Büyük Onarım Giderleri', HESAPKOD='9030607' WHERE YIL=2021 AND HESAPKOD='9030607'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Stok Alımları', HESAPKOD='9030608' WHERE YIL=2021 AND HESAPKOD='9030608'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Sermaye Giderleri', HESAPKOD='9030609' WHERE YIL=2021 AND HESAPKOD='9030609'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sermaye Transferleri', HESAPKOD='90307' WHERE YIL=2021 AND HESAPKOD='90307'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kurum, İşletme ve Hanehalkına Yapılan Sermaye Transferleri', HESAPKOD='9030701' WHERE YIL=2021 AND HESAPKOD='9030701'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yurtdışı Sermaye Transferleri', HESAPKOD='9030702' WHERE YIL=2021 AND HESAPKOD='9030702'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Borç Verme ', HESAPKOD='90308' WHERE YIL=2021 AND HESAPKOD='90308'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yurtiçi Borç Verme ', HESAPKOD='9030801' WHERE YIL=2021 AND HESAPKOD='9030801'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yurtdışı Borç Verme ', HESAPKOD='9030802' WHERE YIL=2021 AND HESAPKOD='9030802'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yedek Ödenekler', HESAPKOD='90309' WHERE YIL=2021 AND HESAPKOD='90309'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Personel Giderlerini Karşılama Ödeneği', HESAPKOD='9030901' WHERE YIL=2021 AND HESAPKOD='9030901'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kur Farklarını Karşılama Ödeneği', HESAPKOD='9030902' WHERE YIL=2021 AND HESAPKOD='9030902'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yatırımları Hızlandırma Ödeneği', HESAPKOD='9030903' WHERE YIL=2021 AND HESAPKOD='9030903'
DELETE FROM  MUHHESAPPLANI WHERE YIL=2021 AND HESAPKOD='9030904'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Doğal Afet Giderlerini Karşılama Ödeneği', HESAPKOD='9030905' WHERE YIL=2021 AND HESAPKOD='9030905'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yedek Ödenek', HESAPKOD='9030906' WHERE YIL=2021 AND HESAPKOD='9030906'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yeni Kurulacak Daire ve İdarelerin İhtiyaçlarını Karşılama Ödeneği', HESAPKOD='9030907' WHERE YIL=2021 AND HESAPKOD='9030907'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Mülteci ve Göçmen Giderleri Ödeneği', HESAPKOD='9030908' WHERE YIL=2021 AND HESAPKOD='9030908'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Yedek Ödenekler', HESAPKOD='9030909' WHERE YIL=2021 AND HESAPKOD='9030909'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='ÖDENEKLER HESABI', HESAPKOD='904' WHERE YIL=2021 AND HESAPKOD='904'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Personel Giderleri', HESAPKOD='90401' WHERE YIL=2021 AND HESAPKOD='90401'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Memurlar', HESAPKOD='9040101' WHERE YIL=2021 AND HESAPKOD='9040101'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sözleşmeli Personel ', HESAPKOD='9040102' WHERE YIL=2021 AND HESAPKOD='9040102'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='İşçiler', HESAPKOD='9040103' WHERE YIL=2021 AND HESAPKOD='9040103'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Geçici Süreli Çalışanlar', HESAPKOD='9040104' WHERE YIL=2021 AND HESAPKOD='9040104'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Personel', HESAPKOD='9040105' WHERE YIL=2021 AND HESAPKOD='9040105'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Milletvekilleri', HESAPKOD='9040107' WHERE YIL=2021 AND HESAPKOD='9040107'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Cumhurbaşkanlığı Ödeneği', HESAPKOD='9040108' WHERE YIL=2021 AND HESAPKOD='9040108'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='İstihbarat Personeli', HESAPKOD='9040109' WHERE YIL=2021 AND HESAPKOD='9040109'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sosyal Güvenlik Kurumlarına Devlet Primi Giderleri', HESAPKOD='90402' WHERE YIL=2021 AND HESAPKOD='90402'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Memurlar', HESAPKOD='9040201' WHERE YIL=2021 AND HESAPKOD='9040201'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sözleşmeli Personel ', HESAPKOD='9040202' WHERE YIL=2021 AND HESAPKOD='9040202'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='İşçiler', HESAPKOD='9040203' WHERE YIL=2021 AND HESAPKOD='9040203'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Geçici Süreli Çalışanlar', HESAPKOD='9040204' WHERE YIL=2021 AND HESAPKOD='9040204'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Personel', HESAPKOD='9040205' WHERE YIL=2021 AND HESAPKOD='9040205'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Milletvekilleri', HESAPKOD='9040207' WHERE YIL=2021 AND HESAPKOD='9040207'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='İstihbarat Personeli', HESAPKOD='9040209' WHERE YIL=2021 AND HESAPKOD='9040209'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Mal ve Hizmet Alım Giderleri', HESAPKOD='90403' WHERE YIL=2021 AND HESAPKOD='90403'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Üretime Yönelik Mal ve Malzeme Alımları', HESAPKOD='9040301' WHERE YIL=2021 AND HESAPKOD='9040301'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Tüketime Yönelik Mal ve Malzeme Alımları', HESAPKOD='9040302' WHERE YIL=2021 AND HESAPKOD='9040302'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yolluklar', HESAPKOD='9040303' WHERE YIL=2021 AND HESAPKOD='9040303'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Görev Giderleri', HESAPKOD='9040304' WHERE YIL=2021 AND HESAPKOD='9040304'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Hizmet Alımları', HESAPKOD='9040305' WHERE YIL=2021 AND HESAPKOD='9040305'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Temsil ve Tanıtma Giderleri', HESAPKOD='9040306' WHERE YIL=2021 AND HESAPKOD='9040306'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Menkul Mal, Gayrimaddi Hak Alım, Bakım ve Onarım Giderleri', HESAPKOD='9040307' WHERE YIL=2021 AND HESAPKOD='9040307'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Gayrimenkul Mal Bakım ve Onarım Giderleri', HESAPKOD='9040308' WHERE YIL=2021 AND HESAPKOD='9040308'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Tedavi ve Cenaze Giderleri', HESAPKOD='9040309' WHERE YIL=2021 AND HESAPKOD='9040309'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Faiz Giderleri', HESAPKOD='90404' WHERE YIL=2021 AND HESAPKOD='90404'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kamu Kurumlarına Ödenen İç Borç Faiz Giderleri', HESAPKOD='9040401' WHERE YIL=2021 AND HESAPKOD='9040401'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer İç Borç Faiz Giderleri', HESAPKOD='9040402' WHERE YIL=2021 AND HESAPKOD='9040402'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Dış Borç Faiz Giderleri', HESAPKOD='9040403' WHERE YIL=2021 AND HESAPKOD='9040403'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='İskonto Giderleri', HESAPKOD='9040404' WHERE YIL=2021 AND HESAPKOD='9040404'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Para Piyasası Nakit İşlemleri Faiz Giderleri', HESAPKOD='9040405' WHERE YIL=2021 AND HESAPKOD='9040405'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Türev Ürün Giderleri', HESAPKOD='9040406' WHERE YIL=2021 AND HESAPKOD='9040406'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kira Sertifikası Giderleri', HESAPKOD='9040407' WHERE YIL=2021 AND HESAPKOD='9040407'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Cari Transferler', HESAPKOD='90405' WHERE YIL=2021 AND HESAPKOD='90405'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Görevlendirme Giderleri', HESAPKOD='9040501' WHERE YIL=2021 AND HESAPKOD='9040501'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Hazine Yardımları', HESAPKOD='9040502' WHERE YIL=2021 AND HESAPKOD='9040502'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kar Amacı Gütmeyen Kuruluşlara Yapılan Transferler', HESAPKOD='9040503' WHERE YIL=2021 AND HESAPKOD='9040503'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Hane Halkı ve İşletmelere Yapılan Transferler', HESAPKOD='9040504' WHERE YIL=2021 AND HESAPKOD='9040504'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Devlet Sosyal Güvenlik Kurumlarından Hane Halkına Yapılan Fayda Ödemeleri', HESAPKOD='9040505' WHERE YIL=2021 AND HESAPKOD='9040505'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yurt Dışına Yapılan Transferler ', HESAPKOD='9040506' WHERE YIL=2021 AND HESAPKOD='9040506'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Gelirlerden Ayrılan Paylar', HESAPKOD='9040508' WHERE YIL=2021 AND HESAPKOD='9040508'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sermaye Giderleri', HESAPKOD='90406' WHERE YIL=2021 AND HESAPKOD='90406'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Mamul Mal Alımları', HESAPKOD='9040601' WHERE YIL=2021 AND HESAPKOD='9040601'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Menkul Sermaye Üretim Giderleri', HESAPKOD='9040602' WHERE YIL=2021 AND HESAPKOD='9040602'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Gayri Maddi Hak Alımları', HESAPKOD='9040603' WHERE YIL=2021 AND HESAPKOD='9040603'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Gayrimenkul Alımları ve Kamulaştırması', HESAPKOD='9040604' WHERE YIL=2021 AND HESAPKOD='9040604'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Gayrimenkul Sermaye Üretim Giderleri', HESAPKOD='9040605' WHERE YIL=2021 AND HESAPKOD='9040605'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Menkul Malların Büyük Onarım Giderleri', HESAPKOD='9040606' WHERE YIL=2021 AND HESAPKOD='9040606'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Gayrimenkul Büyük Onarım Giderleri', HESAPKOD='9040607' WHERE YIL=2021 AND HESAPKOD='9040607'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Stok Alımları', HESAPKOD='9040608' WHERE YIL=2021 AND HESAPKOD='9040608'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Sermaye Giderleri', HESAPKOD='9040609' WHERE YIL=2021 AND HESAPKOD='9040609'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sermaye Transferleri', HESAPKOD='90407' WHERE YIL=2021 AND HESAPKOD='90407'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kurum, İşletme ve Hanehalkına Yapılan Sermaye Transferleri', HESAPKOD='9040701' WHERE YIL=2021 AND HESAPKOD='9040701'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yurtdışı Sermaye Transferleri', HESAPKOD='9040702' WHERE YIL=2021 AND HESAPKOD='9040702'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Borç Verme ', HESAPKOD='90408' WHERE YIL=2021 AND HESAPKOD='90408'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yurtiçi Borç Verme ', HESAPKOD='9040801' WHERE YIL=2021 AND HESAPKOD='9040801'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yurtdışı Borç Verme ', HESAPKOD='9040802' WHERE YIL=2021 AND HESAPKOD='9040802'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yedek Ödenekler', HESAPKOD='90409' WHERE YIL=2021 AND HESAPKOD='90409'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Personel Giderlerini Karşılama Ödeneği', HESAPKOD='9040901' WHERE YIL=2021 AND HESAPKOD='9040901'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kur Farklarını Karşılama Ödeneği', HESAPKOD='9040902' WHERE YIL=2021 AND HESAPKOD='9040902'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yatırımları Hızlandırma Ödeneği', HESAPKOD='9040903' WHERE YIL=2021 AND HESAPKOD='9040903'
DELETE FROM  MUHHESAPPLANI WHERE YIL=2021 AND HESAPKOD='9040904'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Doğal Afet Giderlerini Karşılama Ödeneği', HESAPKOD='9040905' WHERE YIL=2021 AND HESAPKOD='9040905'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yedek Ödenek', HESAPKOD='9040906' WHERE YIL=2021 AND HESAPKOD='9040906'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yeni Kurulacak Daire ve İdarelerin İhtiyaçlarını Karşılama Ödeneği', HESAPKOD='9040907' WHERE YIL=2021 AND HESAPKOD='9040907'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Mülteci ve Göçmen Giderleri Ödeneği', HESAPKOD='9040908' WHERE YIL=2021 AND HESAPKOD='9040908'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Yedek Ödenekler', HESAPKOD='9040909' WHERE YIL=2021 AND HESAPKOD='9040909'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='ÖDENEKLİ GİDERLER HESABI', HESAPKOD='905' WHERE YIL=2021 AND HESAPKOD='905'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Personel Giderleri', HESAPKOD='90501' WHERE YIL=2021 AND HESAPKOD='90501'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Memurlar', HESAPKOD='9050101' WHERE YIL=2021 AND HESAPKOD='9050101'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sözleşmeli Personel ', HESAPKOD='9050102' WHERE YIL=2021 AND HESAPKOD='9050102'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='İşçiler', HESAPKOD='9050103' WHERE YIL=2021 AND HESAPKOD='9050103'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Geçici Süreli Çalışanlar', HESAPKOD='9050104' WHERE YIL=2021 AND HESAPKOD='9050104'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Personel', HESAPKOD='9050105' WHERE YIL=2021 AND HESAPKOD='9050105'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Milletvekilleri', HESAPKOD='9050107' WHERE YIL=2021 AND HESAPKOD='9050107'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Cumhurbaşkanlığı Ödeneği', HESAPKOD='9050108' WHERE YIL=2021 AND HESAPKOD='9050108'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='İstihbarat Personeli', HESAPKOD='9050109' WHERE YIL=2021 AND HESAPKOD='9050109'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sosyal Güvenlik Kurumlarına Devlet Primi Giderleri', HESAPKOD='90502' WHERE YIL=2021 AND HESAPKOD='90502'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Memurlar', HESAPKOD='9050201' WHERE YIL=2021 AND HESAPKOD='9050201'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sözleşmeli Personel ', HESAPKOD='9050202' WHERE YIL=2021 AND HESAPKOD='9050202'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='İşçiler', HESAPKOD='9050203' WHERE YIL=2021 AND HESAPKOD='9050203'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Geçici Süreli Çalışanlar', HESAPKOD='9050204' WHERE YIL=2021 AND HESAPKOD='9050204'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Personel', HESAPKOD='9050205' WHERE YIL=2021 AND HESAPKOD='9050205'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Milletvekilleri', HESAPKOD='9050207' WHERE YIL=2021 AND HESAPKOD='9050207'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='İstihbarat Personeli', HESAPKOD='9050209' WHERE YIL=2021 AND HESAPKOD='9050209'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Mal ve Hizmet Alım Giderleri', HESAPKOD='90503' WHERE YIL=2021 AND HESAPKOD='90503'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Üretime Yönelik Mal ve Malzeme Alımları', HESAPKOD='9050301' WHERE YIL=2021 AND HESAPKOD='9050301'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Tüketime Yönelik Mal ve Malzeme Alımları', HESAPKOD='9050302' WHERE YIL=2021 AND HESAPKOD='9050302'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yolluklar', HESAPKOD='9050303' WHERE YIL=2021 AND HESAPKOD='9050303'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Görev Giderleri', HESAPKOD='9050304' WHERE YIL=2021 AND HESAPKOD='9050304'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Hizmet Alımları', HESAPKOD='9050305' WHERE YIL=2021 AND HESAPKOD='9050305'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Temsil ve Tanıtma Giderleri', HESAPKOD='9050306' WHERE YIL=2021 AND HESAPKOD='9050306'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Menkul Mal, Gayrimaddi Hak Alım, Bakım ve Onarım Giderleri', HESAPKOD='9050307' WHERE YIL=2021 AND HESAPKOD='9050307'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Gayrimenkul Mal Bakım ve Onarım Giderleri', HESAPKOD='9050308' WHERE YIL=2021 AND HESAPKOD='9050308'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Tedavi ve Cenaze Giderleri', HESAPKOD='9050309' WHERE YIL=2021 AND HESAPKOD='9050309'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Faiz Giderleri', HESAPKOD='90504' WHERE YIL=2021 AND HESAPKOD='90504'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kamu Kurumlarına Ödenen İç Borç Faiz Giderleri', HESAPKOD='9050401' WHERE YIL=2021 AND HESAPKOD='9050401'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer İç Borç Faiz Giderleri', HESAPKOD='9050402' WHERE YIL=2021 AND HESAPKOD='9050402'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Dış Borç Faiz Giderleri', HESAPKOD='9050403' WHERE YIL=2021 AND HESAPKOD='9050403'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='İskonto Giderleri', HESAPKOD='9050404' WHERE YIL=2021 AND HESAPKOD='9050404'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Para Piyasası Nakit İşlemleri Faiz Giderleri', HESAPKOD='9050405' WHERE YIL=2021 AND HESAPKOD='9050405'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Türev Ürün Giderleri', HESAPKOD='9050406' WHERE YIL=2021 AND HESAPKOD='9050406'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kira Sertifikası Giderleri', HESAPKOD='9050407' WHERE YIL=2021 AND HESAPKOD='9050407'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Cari Transferler', HESAPKOD='90505' WHERE YIL=2021 AND HESAPKOD='90505'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Görevlendirme Giderleri', HESAPKOD='9050501' WHERE YIL=2021 AND HESAPKOD='9050501'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Hazine Yardımları', HESAPKOD='9050502' WHERE YIL=2021 AND HESAPKOD='9050502'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kar Amacı Gütmeyen Kuruluşlara Yapılan Transferler', HESAPKOD='9050503' WHERE YIL=2021 AND HESAPKOD='9050503'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Hane Halkı ve İşletmelere Yapılan Transferler', HESAPKOD='9050504' WHERE YIL=2021 AND HESAPKOD='9050504'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Devlet Sosyal Güvenlik Kurumlarından Hane Halkına Yapılan Fayda Ödemeleri', HESAPKOD='9050505' WHERE YIL=2021 AND HESAPKOD='9050505'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yurt Dışına Yapılan Transferler ', HESAPKOD='9050506' WHERE YIL=2021 AND HESAPKOD='9050506'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Gelirlerden Ayrılan Paylar', HESAPKOD='9050508' WHERE YIL=2021 AND HESAPKOD='9050508'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sermaye Giderleri', HESAPKOD='90506' WHERE YIL=2021 AND HESAPKOD='90506'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Mamul Mal Alımları', HESAPKOD='9050601' WHERE YIL=2021 AND HESAPKOD='9050601'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Menkul Sermaye Üretim Giderleri', HESAPKOD='9050602' WHERE YIL=2021 AND HESAPKOD='9050602'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Gayri Maddi Hak Alımları', HESAPKOD='9050603' WHERE YIL=2021 AND HESAPKOD='9050603'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Gayrimenkul Alımları ve Kamulaştırması', HESAPKOD='9050604' WHERE YIL=2021 AND HESAPKOD='9050604'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Gayrimenkul Sermaye Üretim Giderleri', HESAPKOD='9050605' WHERE YIL=2021 AND HESAPKOD='9050605'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Menkul Malların Büyük Onarım Giderleri', HESAPKOD='9050606' WHERE YIL=2021 AND HESAPKOD='9050606'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Gayrimenkul Büyük Onarım Giderleri', HESAPKOD='9050607' WHERE YIL=2021 AND HESAPKOD='9050607'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Stok Alımları', HESAPKOD='9050608' WHERE YIL=2021 AND HESAPKOD='9050608'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Sermaye Giderleri', HESAPKOD='9050609' WHERE YIL=2021 AND HESAPKOD='9050609'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sermaye Transferleri', HESAPKOD='90507' WHERE YIL=2021 AND HESAPKOD='90507'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kurum, İşletme ve Hanehalkına Yapılan Sermaye Transferleri', HESAPKOD='9050701' WHERE YIL=2021 AND HESAPKOD='9050701'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yurtdışı Sermaye Transferleri', HESAPKOD='9050702' WHERE YIL=2021 AND HESAPKOD='9050702'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Borç Verme ', HESAPKOD='90508' WHERE YIL=2021 AND HESAPKOD='90508'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yurtiçi Borç Verme ', HESAPKOD='9050801' WHERE YIL=2021 AND HESAPKOD='9050801'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yurtdışı Borç Verme ', HESAPKOD='9050802' WHERE YIL=2021 AND HESAPKOD='9050802'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yedek Ödenekler', HESAPKOD='90509' WHERE YIL=2021 AND HESAPKOD='90509'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Personel Giderlerini Karşılama Ödeneği', HESAPKOD='9050901' WHERE YIL=2021 AND HESAPKOD='9050901'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kur Farklarını Karşılama Ödeneği', HESAPKOD='9050902' WHERE YIL=2021 AND HESAPKOD='9050902'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yatırımları Hızlandırma Ödeneği', HESAPKOD='9050903' WHERE YIL=2021 AND HESAPKOD='9050903'
DELETE FROM  MUHHESAPPLANI WHERE YIL=2021 AND HESAPKOD='9050904'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Doğal Afet Giderlerini Karşılama Ödeneği', HESAPKOD='9050905' WHERE YIL=2021 AND HESAPKOD='9050905'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yedek Ödenek', HESAPKOD='9050906' WHERE YIL=2021 AND HESAPKOD='9050906'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yeni Kurulacak Daire ve İdarelerin İhtiyaçlarını Karşılama Ödeneği', HESAPKOD='9050907' WHERE YIL=2021 AND HESAPKOD='9050907'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Mülteci ve Göçmen Giderleri Ödeneği', HESAPKOD='9050908' WHERE YIL=2021 AND HESAPKOD='9050908'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Yedek Ödenekler', HESAPKOD='9050909' WHERE YIL=2021 AND HESAPKOD='9050909'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='GİDER TAAHHÜTLERİ HESABI', HESAPKOD='920' WHERE YIL=2021 AND HESAPKOD='920'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yılı İçin Geçerli Sözleşmelere İlişkin Taahhütler', HESAPKOD='92001' WHERE YIL=2021 AND HESAPKOD='92001'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Cari Taahhütler', HESAPKOD='9200103' WHERE YIL=2021 AND HESAPKOD='9200103'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Üretime Yönelik Mal ve Malzeme Alımları', HESAPKOD='920010301' WHERE YIL=2021 AND HESAPKOD='920010301'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Hammadde Alımları', HESAPKOD='92001030110' WHERE YIL=2021 AND HESAPKOD='92001030101'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Gıda Ürünleri İçecekler ve Tütün Alımları', HESAPKOD='92001030120' WHERE YIL=2021 AND HESAPKOD='92001030102'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Tekstil ve Tekstil Ürünleri Deri ve Deri Ürünleri Alımları', HESAPKOD='92001030130' WHERE YIL=2021 AND HESAPKOD='92001030103'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kereste ve Kereste Ürünleri Alımları', HESAPKOD='92001030140' WHERE YIL=2021 AND HESAPKOD='92001030104'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kağıt ve Kağıt Ürünleri Alımları', HESAPKOD='92001030150' WHERE YIL=2021 AND HESAPKOD='92001030105'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kimyevi Ürün Alımları', HESAPKOD='92001030160' WHERE YIL=2021 AND HESAPKOD='92001030106'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kauçuk ve Plastik Ürün Alımları', HESAPKOD='92001030170' WHERE YIL=2021 AND HESAPKOD='92001030107'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Metal Ürünü Alımları', HESAPKOD='92001030180' WHERE YIL=2021 AND HESAPKOD='92001030108'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Üretim Mal ve Malzemesi Alımları', HESAPKOD='92001030190' WHERE YIL=2021 AND HESAPKOD='92001030109'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Tüketime Yönelik Mal ve Malzeme Alımları', HESAPKOD='920010302' WHERE YIL=2021 AND HESAPKOD='920010302'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kırtasiye, Yayın, Baskı ve Büro Malzemesi Alımları', HESAPKOD='92001030210' WHERE YIL=2021 AND HESAPKOD='92001030201'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Su ve Temizlik Malzemesi Alımları', HESAPKOD='92001030220' WHERE YIL=2021 AND HESAPKOD='92001030202'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Enerji Alımları', HESAPKOD='92001030230' WHERE YIL=2021 AND HESAPKOD='92001030203'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yiyecek, İçecek ve Yem Alımları', HESAPKOD='92001030240' WHERE YIL=2021 AND HESAPKOD='92001030204'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Giyim ve Kuşam Alımları', HESAPKOD='92001030250' WHERE YIL=2021 AND HESAPKOD='92001030205'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Özel Malzeme Alımları', HESAPKOD='92001030260' WHERE YIL=2021 AND HESAPKOD='92001030206'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Güvenlik ve Savunmaya Yönelik Mal, Malzeme ve Hizmet Alımları, Yapımları ve Giderleri', HESAPKOD='92001030270' WHERE YIL=2021 AND HESAPKOD='92001030207'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='NATO Giderleri İle Gayrimenkul Alım ve Kamulaştırma Giderleri', HESAPKOD='92001030280' WHERE YIL=2021 AND HESAPKOD='92001030208'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Tüketim Mal ve Malzemesi Alımları', HESAPKOD='92001030290' WHERE YIL=2021 AND HESAPKOD='92001030209'
DELETE FROM  MUHHESAPPLANI WHERE YIL=2021 AND HESAPKOD='920010303'
DELETE FROM  MUHHESAPPLANI WHERE YIL=2021 AND HESAPKOD='92001030302'
DELETE FROM  MUHHESAPPLANI WHERE YIL=2021 AND HESAPKOD='92001030304'
DELETE FROM  MUHHESAPPLANI WHERE YIL=2021 AND HESAPKOD='92001030305'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Görev Giderleri', HESAPKOD='920010304' WHERE YIL=2021 AND HESAPKOD='920010304'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Tahliye Giderleri', HESAPKOD='92001030410' WHERE YIL=2021 AND HESAPKOD='92001030401'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yasal Giderler', HESAPKOD='92001030420' WHERE YIL=2021 AND HESAPKOD='92001030402'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Ödenecek Vergi, Resim,  Harçlar ve Benzeri Giderler', HESAPKOD='92001030430' WHERE YIL=2021 AND HESAPKOD='92001030403'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Gizli Hizmet Giderleri', HESAPKOD='92001030450' WHERE YIL=2021 AND HESAPKOD='92001030405'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Görev Giderleri', HESAPKOD='92001030490' WHERE YIL=2021 AND HESAPKOD='92001030409'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Hizmet Alımları', HESAPKOD='920010305' WHERE YIL=2021 AND HESAPKOD='920010305'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Genel Hizmet Alımları ', HESAPKOD='92001030510' WHERE YIL=2021 AND HESAPKOD='92001030501'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Haberleşme Giderleri', HESAPKOD='92001030520' WHERE YIL=2021 AND HESAPKOD='92001030502'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Taşıma Giderleri', HESAPKOD='92001030530' WHERE YIL=2021 AND HESAPKOD='92001030503'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Tarifeye Bağlı Ödemeler', HESAPKOD='92001030540' WHERE YIL=2021 AND HESAPKOD='92001030504'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kiralar', HESAPKOD='92001030550' WHERE YIL=2021 AND HESAPKOD='92001030505'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Devlet Borçları Genel Giderleri', HESAPKOD='92001030560' WHERE YIL=2021 AND HESAPKOD='92001030506'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Hizmet Alımları', HESAPKOD='92001030590' WHERE YIL=2021 AND HESAPKOD='92001030509'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Temsil ve Tanıtma Giderleri', HESAPKOD='920010306' WHERE YIL=2021 AND HESAPKOD='920010306'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Temsil, Tanıtma ve Ağırlama Giderleri', HESAPKOD='92001030610' WHERE YIL=2021 AND HESAPKOD='92001030601'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Toplantı ve Organizasyon Giderleri', HESAPKOD='92001030620' WHERE YIL=2021 AND HESAPKOD='92001030602'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Menkul Mal, Gayrimaddi Hak Alım, Bakım ve Onarım Giderleri', HESAPKOD='920010307' WHERE YIL=2021 AND HESAPKOD='920010307'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Menkul Mal  Alım Giderleri', HESAPKOD='92001030710' WHERE YIL=2021 AND HESAPKOD='92001030701'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Gayri Maddi Hak Alımları', HESAPKOD='92001030720' WHERE YIL=2021 AND HESAPKOD='92001030702'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Bakım ve Onarım Giderleri', HESAPKOD='92001030730' WHERE YIL=2021 AND HESAPKOD='92001030703'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Gayrimenkul Mal Bakım ve Onarım Giderleri', HESAPKOD='920010308' WHERE YIL=2021 AND HESAPKOD='920010308'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Hizmet Binası  Bakım ve Onarım Giderleri', HESAPKOD='92001030810' WHERE YIL=2021 AND HESAPKOD='92001030801'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Lojman  Bakım ve Onarımı Giderleri', HESAPKOD='92001030820' WHERE YIL=2021 AND HESAPKOD='92001030802'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sosyal Tesis  Bakım ve Onarımı Giderleri', HESAPKOD='92001030830' WHERE YIL=2021 AND HESAPKOD='92001030803'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Gemi  Bakım ve Onarımı Giderleri', HESAPKOD='92001030840' WHERE YIL=2021 AND HESAPKOD='92001030804'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Tersane Bakım ve Onarımı Giderleri', HESAPKOD='92001030850' WHERE YIL=2021 AND HESAPKOD='92001030805'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yol  Bakım ve Onarımı Giderleri', HESAPKOD='92001030860' WHERE YIL=2021 AND HESAPKOD='92001030806'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Taşınmaz Yapım, Bakım ve Onarım Giderleri', HESAPKOD='92001030890' WHERE YIL=2021 AND HESAPKOD='92001030809'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Tedavi ve Cenaze Giderleri', HESAPKOD='920010309' WHERE YIL=2021 AND HESAPKOD='920010309'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Cenaze Giderleri', HESAPKOD='92001030930' WHERE YIL=2021 AND HESAPKOD='92001030903'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Tedavi ve Sağlık Malzemesi Giderleri', HESAPKOD='92001030980' WHERE YIL=2021 AND HESAPKOD='92001030908'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Cari Transferlerden Taahhütler', HESAPKOD='9200105' WHERE YIL=2021 AND HESAPKOD='9200105'
DELETE FROM  MUHHESAPPLANI WHERE YIL=2021 AND HESAPKOD='920010503'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Hane Halkı ve İşletmelere Yapılan Transferler', HESAPKOD='920010504' WHERE YIL=2021 AND HESAPKOD='920010504'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Eğitim Amaçlı Diğer Transferler', HESAPKOD='92001050420' WHERE YIL=2021 AND HESAPKOD='92001050402'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yiyecek Amaçlı Transferler', HESAPKOD='92001050440' WHERE YIL=2021 AND HESAPKOD='92001050404'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sosyal Amaçlı Transferler', HESAPKOD='92001050470' WHERE YIL=2021 AND HESAPKOD='92001050407'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sermaye Taahhütleri', HESAPKOD='9200106' WHERE YIL=2021 AND HESAPKOD='9200106'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Mamul Mal Alımları', HESAPKOD='920010601' WHERE YIL=2021 AND HESAPKOD='920010601'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Büro ve İşyeri Mefruşatı Alımları', HESAPKOD='92001060110' WHERE YIL=2021 AND HESAPKOD='92001060101'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Büro ve İşyeri Makine Teçhizat Alımları', HESAPKOD='92001060120' WHERE YIL=2021 AND HESAPKOD='92001060102'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Avadanlık Alımları', HESAPKOD='92001060130' WHERE YIL=2021 AND HESAPKOD='92001060103'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Taşıt Alımları', HESAPKOD='92001060140' WHERE YIL=2021 AND HESAPKOD='92001060104'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='İş Makinası ve Taşınır Tesis Alımları', HESAPKOD='92001060150' WHERE YIL=2021 AND HESAPKOD='92001060105'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yayın Alımları ve Yapımları', HESAPKOD='92001060160' WHERE YIL=2021 AND HESAPKOD='92001060106'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kültür Varlığı Yapımları, Alımları ve Korunması Giderleri', HESAPKOD='92001060170' WHERE YIL=2021 AND HESAPKOD='92001060107'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Menkul Sermaye Üretim Giderleri', HESAPKOD='920010602' WHERE YIL=2021 AND HESAPKOD='920010602'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Müşavir Firma ve Kişilere Ödemeler', HESAPKOD='92001060210' WHERE YIL=2021 AND HESAPKOD='92001060201'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Hammadde Alımları', HESAPKOD='92001060220' WHERE YIL=2021 AND HESAPKOD='92001060202'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Gıda Ürünleri, İçecekler ve Tütün  Alımları', HESAPKOD='92001060230' WHERE YIL=2021 AND HESAPKOD='92001060203'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Tekstil ve Tekstil Ürünleri, Deri ve Deri Ürünleri  Alımları', HESAPKOD='92001060240' WHERE YIL=2021 AND HESAPKOD='92001060204'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kereste ve Kereste Ürünleri  Alımları', HESAPKOD='92001060250' WHERE YIL=2021 AND HESAPKOD='92001060205'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kağıt ve Kağıt Ürünleri  Alımları', HESAPKOD='92001060260' WHERE YIL=2021 AND HESAPKOD='92001060206'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kimyevi Madde İle Kauçuk ve Plastik Ürün  Alımları', HESAPKOD='92001060270' WHERE YIL=2021 AND HESAPKOD='92001060207'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Metal Ürün  Alımları', HESAPKOD='92001060280' WHERE YIL=2021 AND HESAPKOD='92001060208'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Menkul Sermaye Üretim Giderleri', HESAPKOD='92001060290' WHERE YIL=2021 AND HESAPKOD='92001060209'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Gayri Maddi Hak Alımları', HESAPKOD='920010603' WHERE YIL=2021 AND HESAPKOD='920010603'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Bilgisayar Yazılımı Alımları', HESAPKOD='92001060310' WHERE YIL=2021 AND HESAPKOD='92001060301'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Harita Plan Proje Alımları', HESAPKOD='92001060320' WHERE YIL=2021 AND HESAPKOD='92001060302'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Lisans Alımları', HESAPKOD='92001060330' WHERE YIL=2021 AND HESAPKOD='92001060303'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Patent Alımları', HESAPKOD='92001060340' WHERE YIL=2021 AND HESAPKOD='92001060304'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Fikri Hak Alımları', HESAPKOD='92001060390' WHERE YIL=2021 AND HESAPKOD='92001060309'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Gayrimenkul Alımları ve Kamulaştırması', HESAPKOD='920010604' WHERE YIL=2021 AND HESAPKOD='920010604'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Arazi Alım ve Kamulaştırma Giderleri', HESAPKOD='92001060410' WHERE YIL=2021 AND HESAPKOD='92001060401'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Arsa Alım ve Kamulaştırması Giderleri', HESAPKOD='92001060420' WHERE YIL=2021 AND HESAPKOD='92001060402'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Bina Alım ve Kamulaştırma Giderleri', HESAPKOD='92001060430' WHERE YIL=2021 AND HESAPKOD='92001060403'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Gemi Alımları', HESAPKOD='92001060440' WHERE YIL=2021 AND HESAPKOD='92001060404'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Tersane Alımları', HESAPKOD='92001060450' WHERE YIL=2021 AND HESAPKOD='92001060405'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Gayrimenkul Sermaye Üretim Giderleri', HESAPKOD='920010605' WHERE YIL=2021 AND HESAPKOD='920010605'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Müşavir Firma ve Kişilere Ödemeler', HESAPKOD='92001060510' WHERE YIL=2021 AND HESAPKOD='92001060501'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Malzeme Giderleri', HESAPKOD='92001060520' WHERE YIL=2021 AND HESAPKOD='92001060502'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Taşıma Giderleri', HESAPKOD='92001060530' WHERE YIL=2021 AND HESAPKOD='92001060503'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Enerji Giderleri', HESAPKOD='92001060540' WHERE YIL=2021 AND HESAPKOD='92001060504'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Haberleşme Giderleri', HESAPKOD='92001060550' WHERE YIL=2021 AND HESAPKOD='92001060505'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kira Giderleri', HESAPKOD='92001060560' WHERE YIL=2021 AND HESAPKOD='92001060506'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Müteahhitlik Giderleri', HESAPKOD='92001060570' WHERE YIL=2021 AND HESAPKOD='92001060507'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Gayrimenkul Sermaye Üretim Giderleri', HESAPKOD='92001060590' WHERE YIL=2021 AND HESAPKOD='92001060509'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Menkul Malların Büyük Onarım Giderleri', HESAPKOD='920010606' WHERE YIL=2021 AND HESAPKOD='920010606'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Müşavir Firma ve Kişilere Ödemeler', HESAPKOD='92001060610' WHERE YIL=2021 AND HESAPKOD='92001060601'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Malzeme Giderleri', HESAPKOD='92001060620' WHERE YIL=2021 AND HESAPKOD='92001060602'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Taşıma Giderleri', HESAPKOD='92001060630' WHERE YIL=2021 AND HESAPKOD='92001060603'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Enerji Giderleri', HESAPKOD='92001060640' WHERE YIL=2021 AND HESAPKOD='92001060604'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Haberleşme Giderleri', HESAPKOD='92001060650' WHERE YIL=2021 AND HESAPKOD='92001060605'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kira Giderleri', HESAPKOD='92001060660' WHERE YIL=2021 AND HESAPKOD='92001060606'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Müteahhitlik Giderleri', HESAPKOD='92001060670' WHERE YIL=2021 AND HESAPKOD='92001060607'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Menkul Malların Diğer Büyük Onarım Giderleri', HESAPKOD='92001060690' WHERE YIL=2021 AND HESAPKOD='92001060609'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Gayrimenkul Büyük Onarım Giderleri', HESAPKOD='920010607' WHERE YIL=2021 AND HESAPKOD='920010607'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Müşavir Firma ve Kişilere Ödemeler', HESAPKOD='92001060710' WHERE YIL=2021 AND HESAPKOD='92001060701'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Malzeme Giderleri', HESAPKOD='92001060720' WHERE YIL=2021 AND HESAPKOD='92001060702'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Taşıma Giderleri', HESAPKOD='92001060730' WHERE YIL=2021 AND HESAPKOD='92001060703'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Enerji Giderleri', HESAPKOD='92001060740' WHERE YIL=2021 AND HESAPKOD='92001060704'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Haberleşme Giderleri', HESAPKOD='92001060750' WHERE YIL=2021 AND HESAPKOD='92001060705'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kira Giderleri', HESAPKOD='92001060760' WHERE YIL=2021 AND HESAPKOD='92001060706'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Müteahhitlik Giderleri', HESAPKOD='92001060770' WHERE YIL=2021 AND HESAPKOD='92001060707'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Gayrimenkul Büyük Onarım Giderleri', HESAPKOD='92001060790' WHERE YIL=2021 AND HESAPKOD='92001060709'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Stok Alımları', HESAPKOD='920010608' WHERE YIL=2021 AND HESAPKOD='920010608'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Stratejik ve Acil Durum Malzemesi', HESAPKOD='92001060810' WHERE YIL=2021 AND HESAPKOD='92001060801'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Tarımsal Ürünler', HESAPKOD='92001060820' WHERE YIL=2021 AND HESAPKOD='92001060802'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sanayi Ürünleri Alımları', HESAPKOD='92001060830' WHERE YIL=2021 AND HESAPKOD='92001060803'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Hammadde Alımları', HESAPKOD='92001060840' WHERE YIL=2021 AND HESAPKOD='92001060804'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Stok Alımları', HESAPKOD='92001060890' WHERE YIL=2021 AND HESAPKOD='92001060809'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Sermaye Giderleri', HESAPKOD='920010609' WHERE YIL=2021 AND HESAPKOD='920010609'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Gizli Hizmet Giderleri', HESAPKOD='92001060910' WHERE YIL=2021 AND HESAPKOD='92001060901'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Sermaye Giderleri', HESAPKOD='92001060990' WHERE YIL=2021 AND HESAPKOD='92001060909'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sermaye Transferlerinden Taahhütler', HESAPKOD='9200107' WHERE YIL=2021 AND HESAPKOD='9200107'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kurum, İşletme ve Hanehalkına Yapılan Sermaye Transferleri', HESAPKOD='920010701' WHERE YIL=2021 AND HESAPKOD='920010701'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Kurum, İşletme ve Hane Halkına Yapılan Sermaye Transferleri', HESAPKOD='92001070190' WHERE YIL=2021 AND HESAPKOD='92001070109'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Ertesi Mali Yıl veya Yıllara Geçerli Sözleşmelere İlişkin Taahhütler', HESAPKOD='92002' WHERE YIL=2021 AND HESAPKOD='92002'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Cari Taahhütler', HESAPKOD='9200203' WHERE YIL=2021 AND HESAPKOD='9200203'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Üretime Yönelik Mal ve Malzeme Alımları', HESAPKOD='920020301' WHERE YIL=2021 AND HESAPKOD='920020301'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Hammadde Alımları', HESAPKOD='92002030110' WHERE YIL=2021 AND HESAPKOD='92002030101'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Gıda Ürünleri İçecekler ve Tütün Alımları', HESAPKOD='92002030120' WHERE YIL=2021 AND HESAPKOD='92002030102'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Tekstil ve Tekstil Ürünleri Deri ve Deri Ürünleri Alımları', HESAPKOD='92002030130' WHERE YIL=2021 AND HESAPKOD='92002030103'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kereste ve Kereste Ürünleri Alımları', HESAPKOD='92002030140' WHERE YIL=2021 AND HESAPKOD='92002030104'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kağıt ve Kağıt Ürünleri Alımları', HESAPKOD='92002030150' WHERE YIL=2021 AND HESAPKOD='92002030105'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kimyevi Ürün Alımları', HESAPKOD='92002030160' WHERE YIL=2021 AND HESAPKOD='92002030106'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kauçuk ve Plastik Ürün Alımları', HESAPKOD='92002030170' WHERE YIL=2021 AND HESAPKOD='92002030107'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Metal Ürünü Alımları', HESAPKOD='92002030180' WHERE YIL=2021 AND HESAPKOD='92002030108'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Üretim Mal ve Malzemesi Alımları', HESAPKOD='92002030190' WHERE YIL=2021 AND HESAPKOD='92002030109'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Tüketime Yönelik Mal ve Malzeme Alımları', HESAPKOD='920020302' WHERE YIL=2021 AND HESAPKOD='920020302'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kırtasiye, Yayın, Baskı ve Büro Malzemesi Alımları', HESAPKOD='92002030210' WHERE YIL=2021 AND HESAPKOD='92002030201'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Su ve Temizlik Malzemesi Alımları', HESAPKOD='92002030220' WHERE YIL=2021 AND HESAPKOD='92002030202'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Enerji Alımları', HESAPKOD='92002030230' WHERE YIL=2021 AND HESAPKOD='92002030203'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yiyecek, İçecek ve Yem Alımları', HESAPKOD='92002030240' WHERE YIL=2021 AND HESAPKOD='92002030204'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Giyim ve Kuşam Alımları', HESAPKOD='92002030250' WHERE YIL=2021 AND HESAPKOD='92002030205'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Özel Malzeme Alımları', HESAPKOD='92002030260' WHERE YIL=2021 AND HESAPKOD='92002030206'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Güvenlik ve Savunmaya Yönelik Mal, Malzeme ve Hizmet Alımları, Yapımları ve Giderleri', HESAPKOD='92002030270' WHERE YIL=2021 AND HESAPKOD='92002030207'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='NATO Giderleri İle Gayrimenkul Alım ve Kamulaştırma Giderleri', HESAPKOD='92002030280' WHERE YIL=2021 AND HESAPKOD='92002030208'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Tüketim Mal ve Malzemesi Alımları', HESAPKOD='92002030290' WHERE YIL=2021 AND HESAPKOD='92002030209'
DELETE FROM  MUHHESAPPLANI WHERE YIL=2021 AND HESAPKOD='920020303'
DELETE FROM  MUHHESAPPLANI WHERE YIL=2021 AND HESAPKOD='92002030302'
DELETE FROM  MUHHESAPPLANI WHERE YIL=2021 AND HESAPKOD='92002030304'
DELETE FROM  MUHHESAPPLANI WHERE YIL=2021 AND HESAPKOD='92002030305'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Görev Giderleri', HESAPKOD='920020304' WHERE YIL=2021 AND HESAPKOD='920020304'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Tahliye Giderleri', HESAPKOD='92002030410' WHERE YIL=2021 AND HESAPKOD='92002030401'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yasal Giderler', HESAPKOD='92002030420' WHERE YIL=2021 AND HESAPKOD='92002030402'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Ödenecek Vergi, Resim,  Harçlar ve Benzeri Giderler', HESAPKOD='92002030430' WHERE YIL=2021 AND HESAPKOD='92002030403'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Gizli Hizmet Giderleri', HESAPKOD='92002030450' WHERE YIL=2021 AND HESAPKOD='92002030405'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Görev Giderleri', HESAPKOD='92002030490' WHERE YIL=2021 AND HESAPKOD='92002030409'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Hizmet Alımları', HESAPKOD='920020305' WHERE YIL=2021 AND HESAPKOD='920020305'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Genel Hizmet Alımları ', HESAPKOD='92002030510' WHERE YIL=2021 AND HESAPKOD='92002030501'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Haberleşme Giderleri', HESAPKOD='92002030520' WHERE YIL=2021 AND HESAPKOD='92002030502'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Taşıma Giderleri', HESAPKOD='92002030530' WHERE YIL=2021 AND HESAPKOD='92002030503'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Tarifeye Bağlı Ödemeler', HESAPKOD='92002030540' WHERE YIL=2021 AND HESAPKOD='92002030504'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kiralar', HESAPKOD='92002030550' WHERE YIL=2021 AND HESAPKOD='92002030505'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Devlet Borçları Genel Giderleri', HESAPKOD='92002030560' WHERE YIL=2021 AND HESAPKOD='92002030506'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Hizmet Alımları', HESAPKOD='92002030590' WHERE YIL=2021 AND HESAPKOD='92002030509'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Temsil ve Tanıtma Giderleri', HESAPKOD='920020306' WHERE YIL=2021 AND HESAPKOD='920020306'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Temsil, Tanıtma ve Ağırlama Giderleri', HESAPKOD='92002030610' WHERE YIL=2021 AND HESAPKOD='92002030601'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Toplantı ve Organizasyon Giderleri', HESAPKOD='92002030620' WHERE YIL=2021 AND HESAPKOD='92002030602'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Menkul Mal, Gayrimaddi Hak Alım, Bakım ve Onarım Giderleri', HESAPKOD='920020307' WHERE YIL=2021 AND HESAPKOD='920020307'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Menkul Mal  Alım Giderleri', HESAPKOD='92002030710' WHERE YIL=2021 AND HESAPKOD='92002030701'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Gayri Maddi Hak Alımları', HESAPKOD='92002030720' WHERE YIL=2021 AND HESAPKOD='92002030702'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Bakım ve Onarım Giderleri', HESAPKOD='92002030730' WHERE YIL=2021 AND HESAPKOD='92002030703'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Gayrimenkul Mal Bakım ve Onarım Giderleri', HESAPKOD='920020308' WHERE YIL=2021 AND HESAPKOD='920020308'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Hizmet Binası  Bakım ve Onarım Giderleri', HESAPKOD='92002030810' WHERE YIL=2021 AND HESAPKOD='92002030801'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Lojman  Bakım ve Onarımı Giderleri', HESAPKOD='92002030820' WHERE YIL=2021 AND HESAPKOD='92002030802'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sosyal Tesis  Bakım ve Onarımı Giderleri', HESAPKOD='92002030830' WHERE YIL=2021 AND HESAPKOD='92002030803'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Gemi  Bakım ve Onarımı Giderleri', HESAPKOD='92002030840' WHERE YIL=2021 AND HESAPKOD='92002030804'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Tersane Bakım ve Onarımı Giderleri', HESAPKOD='92002030850' WHERE YIL=2021 AND HESAPKOD='92002030805'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yol  Bakım ve Onarımı Giderleri', HESAPKOD='92002030860' WHERE YIL=2021 AND HESAPKOD='92002030806'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Taşınmaz Yapım, Bakım ve Onarım Giderleri', HESAPKOD='92002030890' WHERE YIL=2021 AND HESAPKOD='92002030809'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Tedavi ve Cenaze Giderleri', HESAPKOD='920020309' WHERE YIL=2021 AND HESAPKOD='920020309'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Cenaze Giderleri', HESAPKOD='92002030930' WHERE YIL=2021 AND HESAPKOD='92002030903'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Tedavi ve Sağlık Malzemesi Giderleri', HESAPKOD='92002030980' WHERE YIL=2021 AND HESAPKOD='92002030908'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sermaye Taahhütleri ', HESAPKOD='9200206' WHERE YIL=2021 AND HESAPKOD='9200206'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Mamul Mal Alımları', HESAPKOD='920020601' WHERE YIL=2021 AND HESAPKOD='920020601'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Büro ve İşyeri Mefruşatı Alımları', HESAPKOD='92002060110' WHERE YIL=2021 AND HESAPKOD='92002060101'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Büro ve İşyeri Makine Teçhizat Alımları', HESAPKOD='92002060120' WHERE YIL=2021 AND HESAPKOD='92002060102'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Avadanlık Alımları', HESAPKOD='92002060130' WHERE YIL=2021 AND HESAPKOD='92002060103'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Taşıt Alımları', HESAPKOD='92002060140' WHERE YIL=2021 AND HESAPKOD='92002060104'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='İş Makinası ve Taşınır Tesis Alımları', HESAPKOD='92002060150' WHERE YIL=2021 AND HESAPKOD='92002060105'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Yayın Alımları ve Yapımları', HESAPKOD='92002060160' WHERE YIL=2021 AND HESAPKOD='92002060106'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kültür Varlığı Yapımları, Alımları ve Korunması Giderleri', HESAPKOD='92002060170' WHERE YIL=2021 AND HESAPKOD='92002060107'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Menkul Sermaye Üretim Giderleri', HESAPKOD='920020602' WHERE YIL=2021 AND HESAPKOD='920020602'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Müşavir Firma ve Kişilere Ödemeler', HESAPKOD='92002060210' WHERE YIL=2021 AND HESAPKOD='92002060201'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Hammadde Alımları', HESAPKOD='92002060220' WHERE YIL=2021 AND HESAPKOD='92002060202'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Gıda Ürünleri, İçecekler ve Tütün  Alımları', HESAPKOD='92002060230' WHERE YIL=2021 AND HESAPKOD='92002060203'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Tekstil ve Tekstil Ürünleri, Deri ve Deri Ürünleri  Alımları', HESAPKOD='92002060240' WHERE YIL=2021 AND HESAPKOD='92002060204'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kereste ve Kereste Ürünleri  Alımları', HESAPKOD='92002060250' WHERE YIL=2021 AND HESAPKOD='92002060205'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kağıt ve Kağıt Ürünleri  Alımları', HESAPKOD='92002060260' WHERE YIL=2021 AND HESAPKOD='92002060206'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kimyevi Madde İle Kauçuk ve Plastik Ürün  Alımları', HESAPKOD='92002060270' WHERE YIL=2021 AND HESAPKOD='92002060207'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Metal Ürün  Alımları', HESAPKOD='92002060280' WHERE YIL=2021 AND HESAPKOD='92002060208'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Menkul Sermaye Üretim Giderleri', HESAPKOD='92002060290' WHERE YIL=2021 AND HESAPKOD='92002060209'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Gayri Maddi Hak Alımları', HESAPKOD='920020603' WHERE YIL=2021 AND HESAPKOD='920020603'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Bilgisayar Yazılımı Alımları', HESAPKOD='92002060310' WHERE YIL=2021 AND HESAPKOD='92002060301'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Harita Plan Proje Alımları', HESAPKOD='92002060320' WHERE YIL=2021 AND HESAPKOD='92002060302'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Lisans Alımları', HESAPKOD='92002060330' WHERE YIL=2021 AND HESAPKOD='92002060303'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Patent Alımları', HESAPKOD='92002060340' WHERE YIL=2021 AND HESAPKOD='92002060304'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Fikri Hak Alımları', HESAPKOD='92002060390' WHERE YIL=2021 AND HESAPKOD='92002060309'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Gayrimenkul Alımları ve Kamulaştırması', HESAPKOD='920020604' WHERE YIL=2021 AND HESAPKOD='920020604'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Arazi Alım ve Kamulaştırma Giderleri', HESAPKOD='92002060410' WHERE YIL=2021 AND HESAPKOD='92002060401'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Arsa Alım ve Kamulaştırması Giderleri', HESAPKOD='92002060420' WHERE YIL=2021 AND HESAPKOD='92002060402'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Bina Alım ve Kamulaştırma Giderleri', HESAPKOD='92002060430' WHERE YIL=2021 AND HESAPKOD='92002060403'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Gemi Alımları', HESAPKOD='92002060440' WHERE YIL=2021 AND HESAPKOD='92002060404'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Tersane Alımları', HESAPKOD='92002060450' WHERE YIL=2021 AND HESAPKOD='92002060405'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Gayrimenkul Sermaye Üretim Giderleri', HESAPKOD='920020605' WHERE YIL=2021 AND HESAPKOD='920020605'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Müşavir Firma ve Kişilere Ödemeler', HESAPKOD='92002060510' WHERE YIL=2021 AND HESAPKOD='92002060501'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Malzeme Giderleri', HESAPKOD='92002060520' WHERE YIL=2021 AND HESAPKOD='92002060502'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Taşıma Giderleri', HESAPKOD='92002060530' WHERE YIL=2021 AND HESAPKOD='92002060503'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Enerji Giderleri', HESAPKOD='92002060540' WHERE YIL=2021 AND HESAPKOD='92002060504'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Haberleşme Giderleri', HESAPKOD='92002060550' WHERE YIL=2021 AND HESAPKOD='92002060505'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kira Giderleri', HESAPKOD='92002060560' WHERE YIL=2021 AND HESAPKOD='92002060506'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Müteahhitlik Giderleri', HESAPKOD='92002060570' WHERE YIL=2021 AND HESAPKOD='92002060507'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Gayrimenkul Sermaye Üretim Giderleri', HESAPKOD='92002060590' WHERE YIL=2021 AND HESAPKOD='92002060509'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Menkul Malların Büyük Onarım Giderleri', HESAPKOD='920020606' WHERE YIL=2021 AND HESAPKOD='920020606'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Müşavir Firma ve Kişilere Ödemeler', HESAPKOD='92002060610' WHERE YIL=2021 AND HESAPKOD='92002060601'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Malzeme Giderleri', HESAPKOD='92002060620' WHERE YIL=2021 AND HESAPKOD='92002060602'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Taşıma Giderleri', HESAPKOD='92002060630' WHERE YIL=2021 AND HESAPKOD='92002060603'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Enerji Giderleri', HESAPKOD='92002060640' WHERE YIL=2021 AND HESAPKOD='92002060604'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Haberleşme Giderleri', HESAPKOD='92002060650' WHERE YIL=2021 AND HESAPKOD='92002060605'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kira Giderleri', HESAPKOD='92002060660' WHERE YIL=2021 AND HESAPKOD='92002060606'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Müteahhitlik Giderleri', HESAPKOD='92002060670' WHERE YIL=2021 AND HESAPKOD='92002060607'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Menkul Malların Diğer Büyük Onarım Giderleri', HESAPKOD='92002060690' WHERE YIL=2021 AND HESAPKOD='92002060609'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Gayrimenkul Büyük Onarım Giderleri', HESAPKOD='920020607' WHERE YIL=2021 AND HESAPKOD='920020607'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Müşavir Firma ve Kişilere Ödemeler', HESAPKOD='92002060710' WHERE YIL=2021 AND HESAPKOD='92002060701'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Malzeme Giderleri', HESAPKOD='92002060720' WHERE YIL=2021 AND HESAPKOD='92002060702'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Taşıma Giderleri', HESAPKOD='92002060730' WHERE YIL=2021 AND HESAPKOD='92002060703'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Enerji Giderleri', HESAPKOD='92002060740' WHERE YIL=2021 AND HESAPKOD='92002060704'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Haberleşme Giderleri', HESAPKOD='92002060750' WHERE YIL=2021 AND HESAPKOD='92002060705'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kira Giderleri', HESAPKOD='92002060760' WHERE YIL=2021 AND HESAPKOD='92002060706'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Müteahhitlik Giderleri', HESAPKOD='92002060770' WHERE YIL=2021 AND HESAPKOD='92002060707'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Gayrimenkul Büyük Onarım Giderleri', HESAPKOD='92002060790' WHERE YIL=2021 AND HESAPKOD='92002060709'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Stok Alımları', HESAPKOD='920020608' WHERE YIL=2021 AND HESAPKOD='920020608'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Stratejik ve Acil Durum Malzemesi', HESAPKOD='92002060810' WHERE YIL=2021 AND HESAPKOD='92002060801'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Tarımsal Ürünler', HESAPKOD='92002060820' WHERE YIL=2021 AND HESAPKOD='92002060802'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sanayi Ürünleri Alımları', HESAPKOD='92002060830' WHERE YIL=2021 AND HESAPKOD='92002060803'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Hammadde Alımları', HESAPKOD='92002060840' WHERE YIL=2021 AND HESAPKOD='92002060804'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Stok Alımları', HESAPKOD='92002060890' WHERE YIL=2021 AND HESAPKOD='92002060809'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Sermaye Giderleri', HESAPKOD='920020609' WHERE YIL=2021 AND HESAPKOD='920020609'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Gizli Hizmet Giderleri', HESAPKOD='92002060910' WHERE YIL=2021 AND HESAPKOD='92002060901'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Sermaye Giderleri', HESAPKOD='92002060990' WHERE YIL=2021 AND HESAPKOD='92002060909'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sermaye Transferlerinden Taahhütler', HESAPKOD='9200207' WHERE YIL=2021 AND HESAPKOD='9200207'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kurum, İşletme ve Hanehalkına Yapılan Sermaye Transferleri', HESAPKOD='920020701' WHERE YIL=2021 AND HESAPKOD='920020701'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Kurum, İşletme ve Hane Halkına Yapılan Sermaye Transferleri', HESAPKOD='92002070190' WHERE YIL=2021 AND HESAPKOD='92002070109'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Fiyat Farkları', HESAPKOD='92003' WHERE YIL=2021 AND HESAPKOD='92003'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Cari Taahhütler', HESAPKOD='9200303' WHERE YIL=2021 AND HESAPKOD='9200303'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Üretime Yönelik Mal ve Malzeme Alımları', HESAPKOD='920030301' WHERE YIL=2021 AND HESAPKOD='920030301'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Tüketime Yönelik Mal ve Malzeme Alımları', HESAPKOD='920030302' WHERE YIL=2021 AND HESAPKOD='920030302'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Görev Giderleri', HESAPKOD='920030304' WHERE YIL=2021 AND HESAPKOD='920030304'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Hizmet Alımları', HESAPKOD='920030305' WHERE YIL=2021 AND HESAPKOD='920030305'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Temsil ve Tanıtma Giderleri', HESAPKOD='920030306' WHERE YIL=2021 AND HESAPKOD='920030306'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Menkul Mal, Gayrimaddi Hak Alım, Bakım ve Onarım Giderleri', HESAPKOD='920030307' WHERE YIL=2021 AND HESAPKOD='920030307'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Gayrimenkul Mal Bakım ve Onarım Giderleri', HESAPKOD='920030308' WHERE YIL=2021 AND HESAPKOD='920030308'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sermaye Taahhütleri', HESAPKOD='9200306' WHERE YIL=2021 AND HESAPKOD='9200306'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Mamul Mal Alımları', HESAPKOD='920030601' WHERE YIL=2021 AND HESAPKOD='920030601'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Menkul Sermaye Üretim Giderleri', HESAPKOD='920030602' WHERE YIL=2021 AND HESAPKOD='920030602'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Gayri Maddi Hak Alımları', HESAPKOD='920030603' WHERE YIL=2021 AND HESAPKOD='920030603'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Gayrimenkul Alımları ve Kamulaştırması', HESAPKOD='920030604' WHERE YIL=2021 AND HESAPKOD='920030604'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Gayrimenkul Sermaye Üretim Giderleri', HESAPKOD='920030605' WHERE YIL=2021 AND HESAPKOD='920030605'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Menkul Malların Büyük Onarım Giderleri', HESAPKOD='920030606' WHERE YIL=2021 AND HESAPKOD='920030606'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Gayrimenkul Büyük Onarım Giderleri', HESAPKOD='920030607' WHERE YIL=2021 AND HESAPKOD='920030607'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Stok Alımları', HESAPKOD='920030608' WHERE YIL=2021 AND HESAPKOD='920030608'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Diğer Sermaye Giderleri', HESAPKOD='920030609' WHERE YIL=2021 AND HESAPKOD='920030609'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Sermaye Transferlerinden Taahhütler', HESAPKOD='9200307' WHERE YIL=2021 AND HESAPKOD='9200307'
UPDATE MUHHESAPPLANI SET GRUP='1', ACIKLAMA='Kurum, İşletme ve Hanehalkına Yapılan Sermaye Transferleri', HESAPKOD='920030701' WHERE YIL=2021 AND HESAPKOD='920030701'
";
            string[] sql2 = sql.Split('\n');

            foreach (string ss in sql2)
            {
                if (string.IsNullOrWhiteSpace(ss)) continue;

                try
                {
                    veriErisim.hata = "";
                    KomutCalistir(ss, 0);
                }
                catch
                {


                }


            }

            return new Sonuc();
        }

        public Sonuc YansitmaGuncelle2021()
        {
            string sql = "";
            sql = @"

UPDATE MUHYANSITMA SET HKOD=SUBSTRING(HKOD,1,7)+SUBSTRING(HKOD,9,1)+'0'+SUBSTRING(HKOD,10,20) where YIL=2021 AND LEN(HKOD)>10 AND HKOD LIKE '630%'
UPDATE MUHYANSITMA SET YKOD=SUBSTRING(YKOD,1,7)+SUBSTRING(YKOD,9,1)+'0'+SUBSTRING(YKOD,10,20) where YIL=2021 AND LEN(YKOD)>10 AND YKOD LIKE '830%'
DELETE FROM MUHYANSITMA where YIL=2021 AND HKOD LIKE '834%'
DELETE FROM MUHYANSITMA where YIL=2021 AND YKOD LIKE '834%'
";

            return KomutCalistir(sql, 0);
        }

    }
}
