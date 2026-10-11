using System;
using System.Data;
using TNS.Veri;

namespace TNS.TMM.IsKurallari.Veri
{
    /// <summary>
    /// Taşınır İşlem Form bilgilerini tutan AMORTISMANFORM veri tabanı tablosu ile ilgili  silme, listeleme ve yazma işlemleri tutan sınıf
    /// </summary>
    public class TABAmortismanIslemForm : TABOrtak
    {
        /// <summary>
        /// Sınıfın oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABAmortismanIslemForm(VeriErisim veriErisim)
            : base(veriErisim)
        {
        }

        /// <summary>
        /// Tablo adını tutan sabit değişken
        /// </summary>
        static string TABLOAD = "TASAMORTISMANFORM";

        public enum ENUMAlanlar : int
        {
            KOD = 0,
            YIL = 1,
            DONEM = 2,
            MUHASEBEKOD = 3,
            HARCAMABIRIMKOD = 4,
            FISNO = 5,
            FISTARIH = 6,
            AMBARKOD = 7,
            DURUM = 8,
            ISLEMTARIH = 9,
            ISLEMYAPAN = 10,
            ONAYDURUM = 11,
            FISACIKLAMA = 12,
        }

        /// <summary>
        /// Tablo alanlarının isimlerini tutan sabit dizi
        /// </summary>
        static string[] ALANLAR = { "KOD", "YIL", "DONEM", "MUHASEBEKOD", "HARCAMABIRIMKOD", "FISNO", "FISTARIH", "AMBARKOD", "DURUM", "ISLEMTARIH", "ISLEMYAPAN", "ONAYDURUM", "FISACIKLAMA" };


        public Sonuc Kaydet(AmortismanIslemForm form)
        {
            string sql = "SELECT YIL, DONEM, MUHASEBEKOD, HARCAMABIRIMKOD, AMBARKOD FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", form.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.DONEM], "=", form.donem));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", form.muhasebeKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.HARCAMABIRIMKOD], "=", form.harcamaKod.Replace(".", "")));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.AMBARKOD], "=", form.ambarKod));
            //sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.FISNO], "=", form.fisNo));

            IDataReader dr = veriErisim.DataReaderDondur(sql);

            bool sonuc = dr.Read();
            dr.Close();

            if (sonuc) //Kayıt var, güncelle
            {
                sql = "UPDATE " + TABLOAD + " SET ";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.DURUM], form.durum) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ISLEMYAPAN], form.islemYapan) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ISLEMTARIH], form.islemTarih) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ONAYDURUM], form.onayDurum);

                sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", form.yil));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.DONEM], "=", form.donem));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", form.muhasebeKod));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.HARCAMABIRIMKOD], "=", form.harcamaKod.Replace(".", "")));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.AMBARKOD], "=", form.ambarKod));
                //sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.FISNO], "=", form.fisNo));

                KaydetTarihce(form.yil, form.muhasebeKod, form.harcamaKod, form.fisNo, form.islemYapan, TasinirIslemForm.FormDurumAciklama((int)ENUMBelgeDurumu.DEGISTIRILDI), form.kod, "");
            }
            else //Kayıt yok, ekle
            {
                sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
                sql += VeriErisim.DegerYap(form.kod) + ",";
                sql += VeriErisim.DegerYap(form.yil) + ",";
                sql += VeriErisim.DegerYap(form.donem) + ",";
                sql += VeriErisim.DegerYap(form.muhasebeKod) + ",";
                sql += VeriErisim.DegerYap(form.harcamaKod.Replace(".", "")) + ",";
                sql += VeriErisim.DegerYap(form.fisNo) + ",";
                sql += VeriErisim.DegerYap(form.fisTarih) + ",";
                sql += VeriErisim.DegerYap(form.ambarKod) + ",";
                sql += VeriErisim.DegerYap(form.durum) + ",";
                sql += VeriErisim.DegerYap(form.islemTarih) + ",";
                sql += VeriErisim.DegerYap(form.islemYapan) + ",";
                sql += VeriErisim.DegerYap(form.onayDurum) + ",";
                sql += VeriErisim.DegerYap(form.fisAciklama) + ")";

                KaydetTarihce(form.yil, form.muhasebeKod, form.harcamaKod, form.fisNo, form.islemYapan, TasinirIslemForm.FormDurumAciklama((int)ENUMBelgeDurumu.YENI), form.kod, "");
            }

            return KomutCalistir(sql, 1);
        }

        public IDataReader Oku(AmortismanIslemForm form, bool tekil)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR) + ", TASMUHASEBE.AD, TASHARCAMABIRIM.AD, TASAMBAR.AD FROM " + TABLOAD;

            sql += " INNER JOIN TASMUHASEBE ON TASAMORTISMANFORM.MUHASEBEKOD = TASMUHASEBE.KOD";
            sql += " INNER JOIN TASHARCAMABIRIM ON TASAMORTISMANFORM.HARCAMABIRIMKOD = TASHARCAMABIRIM.KOD AND TASAMORTISMANFORM.MUHASEBEKOD = TASHARCAMABIRIM.MUHASEBEKOD";
            sql += " INNER JOIN TASAMBAR ON TASAMORTISMANFORM.AMBARKOD = TASAMBAR.KOD AND TASAMORTISMANFORM.HARCAMABIRIMKOD=TASAMBAR.HARCAMABIRIMKOD AND TASAMORTISMANFORM.MUHASEBEKOD = TASAMBAR.MUHASEBEKOD AND TASAMORTISMANFORM.MUHASEBEKOD = TASHARCAMABIRIM.MUHASEBEKOD";

            if (form.kod != "")
                sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KOD], "=", form.kod));
            else
            {
                sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", form.muhasebeKod.Trim()));

                if (form.yil > 0)
                    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.YIL], "=", form.yil));
                if (form.donem > 0)
                    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.DONEM], "=", form.donem));

                if (!tekil)
                {
                    if (form.tip == "ENFLASYONDUZELTMESI")
                    {
                        if (form.fisNo != "")
                            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.FISNO], "=", form.fisNo));
                        else
                            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.FISNO], "LIKE", "E%"));
                    }
                    else
                        sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.FISNO], "LIKE", "A%"));
                }


                if (form.harcamaKod != "")
                    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.HARCAMABIRIMKOD], "=", form.harcamaKod.Trim().Replace(".", "")));
                if (form.ambarKod != "")
                    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.AMBARKOD], "=", form.ambarKod));

                if (form.durum > 0)
                {
                    sql = VeriErisim.KosulEkle(sql, "AND (", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.DURUM], "=", form.durum));
                    if (form.durum == 1) //Yeni ve değişen belgeleri getir.
                        sql = VeriErisim.KosulEkle(sql, "OR", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.DURUM], "=", (int)ENUMBelgeDurumu.DEGISTIRILDI));
                    sql += ")";
                }
                else if (form.durum == 0)
                {
                    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.DURUM], "<>", (int)ENUMBelgeDurumu.IPTAL));
                }
            }

            return veriErisim.DataReaderDondur(sql);
        }

        public IDataReader Listele(AmortismanIslemForm form)
        {
            string secAlanlar = VeriErisim.TumAlanlar(TABLOAD, ALANLAR) + ",TASMUHASEBE.AD, TASHARCAMABIRIM.AD, TASAMBAR.AD, TASISLEMTIP.AD";
            string sql = "SELECT " + secAlanlar + " FROM ";
            sql += " TASHARCAMABIRIM INNER JOIN ";
            sql += " TASMUHASEBE ON TASHARCAMABIRIM.MUHASEBEKOD = TASMUHASEBE.KOD INNER JOIN ";
            sql += " TASAMBAR ON TASHARCAMABIRIM.MUHASEBEKOD = TASAMBAR.MUHASEBEKOD AND ";
            sql += " TASHARCAMABIRIM.KOD = TASAMBAR.HARCAMABIRIMKOD INNER JOIN ";
            sql += " AMORTISMANFORM ON TASAMBAR.MUHASEBEKOD = AMORTISMANFORM.MUHASEBEKOD AND ";
            sql += " TASAMBAR.HARCAMABIRIMKOD = AMORTISMANFORM.HARCAMABIRIMKOD AND TASAMBAR.KOD = AMORTISMANFORM.AMBARKOD ";

            if (form.hesapKodu != string.Empty)
                sql += " INNER JOIN TASAMORTISMAN ON AMORTISMANFORM.KOD = TASAMORTISMAN.AMORTISMANFORMKOD ";

            //B-A Onayı yetki kontnrolü için eklendi. HÖ - 27.20.2017
            if (form.onayDurum == -100)
            {
                sql += " INNER JOIN ";
                sql += " TASISLEMONAYYETKI ON TASHARCAMABIRIM.MUHASEBEKOD = TASISLEMONAYYETKI.MUHASEBEKOD AND ";
                sql += " TASHARCAMABIRIM.HARCAMABIRIMKOD = TASISLEMONAYYETKI.HARCAMABIRIMKOD AND ";
                sql += " TASHARCAMABIRIM.ONAYDURUM = TASISLEMONAYYETKI.ONAYTUR ";

                sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("TASISLEMONAYYETKI", "MERNIS", "=", form.mernis));
            }
            else
            {
                sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.YIL], "=", form.yil));

                if (form.mernis != string.Empty)
                    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("TASISLEMONAYYETKI", "MERNIS", "=", form.mernis));
            }

            if (form.kod != "")
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KOD], "=", form.kod));

            if (form.muhasebeKod != "")
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", form.muhasebeKod));

            if (form.durum > 0)
            {
                sql = VeriErisim.KosulEkle(sql, "AND (", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.DURUM], "=", form.durum));
                if (form.durum == 1) //Yeni ve değişen belgeleri getir.
                    sql = VeriErisim.KosulEkle(sql, "OR", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.DURUM], "=", (int)ENUMBelgeDurumu.DEGISTIRILDI));
                sql += ")";
            }
            else if (form.durum == 0)
            {
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.DURUM], "<>", (int)ENUMBelgeDurumu.IPTAL));
            }
            if (form.harcamaKod != string.Empty)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.HARCAMABIRIMKOD], "=", form.harcamaKod.Replace(".", "")));
            if (form.ambarKod != string.Empty)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.AMBARKOD], "=", form.ambarKod));

            if (form.islemYapan != string.Empty)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ISLEMYAPAN], "=", form.islemYapan));

            if (form.onayDurum > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ONAYDURUM], "=", form.onayDurum));

            sql += " GROUP BY " + secAlanlar;

            sql += " ORDER BY 1,2,3,4";

            return veriErisim.DataReaderDondur(sql);

        }

        public Sonuc DurumDegistir(AmortismanIslemForm form, ENUMAmortismanFormDurumu yeniDurum)
        {
            string sql = "UPDATE " + TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.DURUM], (int)yeniDurum) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ISLEMTARIH], new TNSDateTime(DateTime.Now)) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ISLEMYAPAN], form.islemYapan);
            //sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", form.kod));

            if (form.kod != "")
                sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KOD], "=", form.kod));
            else
            {
                sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.YIL], "=", form.yil));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.DONEM], "=", form.donem));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.HARCAMABIRIMKOD], "=", form.harcamaKod.Trim().Replace(".", "")));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", form.muhasebeKod.Trim()));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.AMBARKOD], "=", form.ambarKod));
            }

            return KomutCalistir(sql, 1);
        }
        public void KaydetTarihce(int yil, string muhasebeKod, string harcamaBirimKod, string fisNo, string islemYapan, string islem, string tasinirIslemFormKod, string ipAdresi)
        {
            string sql = "";

            sql = "INSERT INTO TASISLEMFORMTARIHCE (YIL,MUHASEBEKOD,HARCAMABIRIMKOD,FISNO,ISLEMYAPAN,ISLEM,ISLEMTARIH,TASISLEMFORMKOD,IPADRESI) VALUES (";
            sql = sql + VeriErisim.DegerYap(yil) + ", ";
            sql = sql + VeriErisim.DegerYap(muhasebeKod) + ", ";
            sql = sql + VeriErisim.DegerYap(harcamaBirimKod.Trim().Replace(".", "")) + ", ";
            sql = sql + VeriErisim.DegerYap(fisNo.Trim()) + ", ";
            sql = sql + VeriErisim.DegerYap(islemYapan) + ", ";
            sql = sql + VeriErisim.DegerYap(islem) + ", ";
            sql = sql + VeriErisim.DegerYap(new TNSDateTime(DateTime.Now)) + ", ";
            sql = sql + VeriErisim.DegerYap(tasinirIslemFormKod) + ",";
            sql = sql + VeriErisim.DegerYap(ipAdresi) + ")";

            KomutCalistir(sql, 1);
        }

        public int DurumGetirTekKolon(AmortismanIslemForm form)
        {
            string sql = "SELECT DURUM FROM " + TABLOAD;

            if (form.kod != "")
                sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KOD], "=", form.kod));
            else
            {
                sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.YIL], "=", form.yil));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.DONEM], "=", form.donem));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.HARCAMABIRIMKOD], "=", form.harcamaKod.Trim().Replace(".", "")));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", form.muhasebeKod.Trim()));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.AMBARKOD], "=", form.ambarKod.Trim()));
            }

            return veriErisim.TekKolonSQLInt(sql);
        }

        public int OnayDurumGetirTekKolon(AmortismanIslemForm form)
        {
            string sql = "SELECT ONAYDURUM FROM " + TABLOAD;

            if (form.kod != "")
                sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KOD], "=", form.kod));
            else
            {
                sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.YIL], "=", form.yil));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.DONEM], "=", form.donem));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.HARCAMABIRIMKOD], "=", form.harcamaKod.Trim().Replace(".", "")));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", form.muhasebeKod.Trim()));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.AMBARKOD], "=", form.ambarKod));
            }
            return veriErisim.TekKolonSQLInt(sql);
        }

        public Sonuc OnayDurumDegistir(AmortismanIslemForm form, ENUMTasinirIslemFormOnayDurumu yeniDurum, string fisAciklama)
        {
            string sql = "UPDATE " + TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ONAYDURUM], (int)yeniDurum);

            if (!string.IsNullOrWhiteSpace(fisAciklama))
                sql += "," + VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.FISACIKLAMA], fisAciklama);

            if (form.kod != "")
                sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KOD], "=", form.kod));
            else
            {
                sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.YIL], "=", form.yil));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.DONEM], "=", form.donem));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.HARCAMABIRIMKOD], "=", form.harcamaKod.Trim().Replace(".", "")));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", form.muhasebeKod.Trim()));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.AMBARKOD], "=", form.ambarKod));
            }

            return KomutCalistir(sql, 1);
        }

        public bool AmortismanYapilmisMi(AmortismanKriter ak)
        {
            string sql = "SELECT COUNT(*) FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", ak.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.DONEM], "=", ak.donem));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", ak.muhasebeKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.HARCAMABIRIMKOD], "=", ak.harcamaKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.AMBARKOD], "=", ak.ambarKod));

            string ekKosul = "";
            ekKosul = VeriErisim.KosulEkle(ekKosul, "OR", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.DURUM], "=", 1)); //Onaysız
            ekKosul = VeriErisim.KosulEkle(ekKosul, "OR", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.DURUM], "=", 5)); //Onaylı
            sql = VeriErisim.KosulEkle(sql, "AND", "(" + ekKosul + ")");

            return veriErisim.TekKolonSQLInt(sql) > 0;
        }

        public IDataReader SonAmortisman(AmortismanIslemForm form)
        {
            string sql = "SELECT TOP(1) " + VeriErisim.TumAlanlar("", ALANLAR) + " FROM " + TABLOAD;

            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", form.muhasebeKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.HARCAMABIRIMKOD], "=", form.harcamaKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.AMBARKOD], "=", form.ambarKod));

            if (form.yil > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", form.yil));

            if (form.donem > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.DONEM], "=", form.donem));
            else
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.DONEM], "<=", 12)); //Amortisman dönemi 12 den büyük olamaz. Bedel düzeltme için 12 den büyük değerler kullanılıyor.

            string ekKosul = "";
            ekKosul = VeriErisim.KosulEkle(ekKosul, "OR", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.DURUM], "=", 1)); //Onaysız
            ekKosul = VeriErisim.KosulEkle(ekKosul, "OR", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.DURUM], "=", 5)); //Onaylı
            sql = VeriErisim.KosulEkle(sql, "AND", "(" + ekKosul + ")");

            sql += " ORDER BY YIL*100 + DONEM DESC";

            return veriErisim.DataReaderDondur(sql);
        }
    }
}
