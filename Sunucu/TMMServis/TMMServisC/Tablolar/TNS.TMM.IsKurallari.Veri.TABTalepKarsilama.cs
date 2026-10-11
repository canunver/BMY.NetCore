using System.Data;
using TNS.Veri;
using System;

namespace TNS.TMM.IsKurallari.Veri
{
    public class TABTalepKarsilama : TABOrtak
    {
        public TABTalepKarsilama(VeriErisim veriErisim)
            : base(veriErisim)
        {
        }

        static string TABLOAD = "TASTALEPKARSILAMA";
        public enum ENUMAlanlar : int
        {
            YIL = 0,
            MUHASEBEKOD = 1,
            HARCAMABIRIMKOD = 2,
            BELGENO = 3,
            BELGETARIHI = 4,
            TALEPEDENTCNO = 5,
            DURUM = 6,
            TALEPEDENADISOYADI = 7,
            TALEPEDENUNVAN = 8,
            GEREKCE = 9,
            KARSILAYANADISOYADI = 10,
            KARSILAYANUNVAN = 11,
            REFID = 12,
            TUR = 13,
            AMAC = 14,
        }
        static string[] ALANLAR ={ "YIL", "MUHASEBEKOD", "HARCAMABIRIMKOD",
                                   "BELGENO", "BELGETARIHI", "TALEPEDENTCNO", "DURUM", "TALEPEDENADISOYADI", "TALEPEDENUNVAN", "GEREKCE", "KARSILAYANADISOYADI", "KARSILAYANUNVAN","REFID","TUR","AMAC"};

        static string TABLOADCEVAP = "TASTALEPKARSILAMACEVAP";
        public enum ENUMAlanlarCevap : int
        {
            REFID = 0,
            REFIDANA = 1,
            TARIH = 2,
            ACIKLAMA = 3,
            DURUM = 4,
            KARSILAYANADISOYADI = 5,
            KARSILAYANUNVAN = 6

        }
        static string[] ALANLARCEVAP = { "REFID", "REFIDANA", "TARIH", "ACIKLAMA", "DURUM", "KARSILAYANADISOYADI", "KARSILAYANUNVAN" };

        static string TABLOADCEVAPDETAY = "TASTALEPKARSILAMACEVAPDETAY";
        public enum ENUMAlanlarCevapDetay : int
        {
            REFID = 0,
            REFIDANA = 1,
            REFIDTALEP = 2,
            TUR = 3,
            HESAPPLANKOD = 4,
            HESAPPLANAD = 5,
            OLCUBIRIMI = 6,
            SATINALMADURUM = 7,
            MIKTAR = 8,
        }
        static string[] ALANLARCEVAPDETAY = { "REFID", "REFIDANA", "REFIDTALEP", "TUR", "HESAPPLANKOD", "HESAPPLANAD", "OLCUBIRIMI", "SATINALMADURUM", "MIKTAR" };

        public Sonuc Kaydet(TalepKarsilama tk)
        {
            if (tk.belgeNo == "")
            {
                string[] intAlan = { ALANLAR[(int)ENUMAlanlar.YIL] };
                int[] intDeger = { tk.yil };
                string[] strAlan = { ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], ALANLAR[(int)ENUMAlanlar.HARCAMABIRIMKOD] };
                string[] strDeger = { tk.muhasebe.kod, tk.harcamaBirimi.kod.Replace(".", "") };
                tk.belgeNo = TABGenel.SayacDegerAl(veriErisim, TABLOAD, ALANLAR[(int)ENUMAlanlar.BELGENO], strAlan, strDeger, intAlan, intDeger).ToString();
            }

            string sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
            sql += VeriErisim.DegerYap(tk.yil) + ",";
            sql += VeriErisim.DegerYap(tk.muhasebe.kod) + ",";
            sql += VeriErisim.DegerYap(tk.harcamaBirimi.kod.Replace(".", "")) + ",";
            sql += VeriErisim.DegerYap(tk.belgeNo) + ",";
            sql += VeriErisim.DegerYap(tk.belgeTarihi) + ",";
            sql += VeriErisim.DegerYap(tk.talepEdenTCNo) + ",";
            sql += VeriErisim.DegerYap(tk.durum) + ",";
            sql += VeriErisim.DegerYap(tk.talepedenAdSoyad) + ",";
            sql += VeriErisim.DegerYap(tk.talepedenUnvan) + ",";
            sql += VeriErisim.DegerYap(tk.gerekce) + ",";
            sql += VeriErisim.DegerYap("") + ",";
            sql += VeriErisim.DegerYap("") + ",";
            sql += VeriErisim.DegerYap(tk.refId) + ",";
            sql += VeriErisim.DegerYap(tk.tur) + ",";
            sql += VeriErisim.DegerYap(tk.amac) + ")";

            Sonuc sonuc = KomutCalistir(sql, 1);
            sonuc.anahtar = tk.belgeNo;
            return sonuc;
        }

        public Sonuc Guncelle(TalepKarsilama tk)
        {
            string sql = "UPDATE " + TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.BELGETARIHI], tk.belgeTarihi) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.TALEPEDENTCNO], tk.talepEdenTCNo) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.DURUM], tk.durum) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.TALEPEDENADISOYADI], tk.talepedenAdSoyad) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.TALEPEDENUNVAN], tk.talepedenUnvan) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.GEREKCE], tk.gerekce) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.KARSILAYANADISOYADI], "") + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.KARSILAYANUNVAN], "") + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.AMAC], tk.amac) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.TUR], tk.tur);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", tk.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", tk.muhasebe.kod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.HARCAMABIRIMKOD], "=", tk.harcamaBirimi.kod.Replace(".", "")));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.BELGENO], "=", tk.belgeNo));

            return KomutCalistir(sql, 1);
        }

        public Sonuc DurumGuncelle(string refId, int durumKod)
        {
            string sql = "UPDATE " + TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.DURUM], durumKod);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.REFID], "=", refId));

            return KomutCalistir(sql, 1);
        }

        public Sonuc DurumGuncelle(TalepKarsilama tk)
        {
            string sql = "UPDATE " + TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.DURUM], tk.durum);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", tk.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", tk.muhasebe.kod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.HARCAMABIRIMKOD], "=", tk.harcamaBirimi.kod.Replace(".", "")));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.BELGENO], "=", tk.belgeNo));

            return KomutCalistir(sql, 1);
        }

        public IDataReader Listele(TalepKarsilama tk)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar("A", ALANLAR);
            sql += " FROM " + TABLOAD + " A";

            string kosul = string.Empty;
            if (tk.yil > 0)
                kosul = VeriErisim.KosulOl("A", ALANLAR[(int)ENUMAlanlar.YIL], "=", tk.yil);

            if (!string.IsNullOrEmpty(tk.muhasebe.kod))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("A", ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", tk.muhasebe.kod));

            if (!string.IsNullOrEmpty(tk.harcamaBirimi.kod))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("A", ALANLAR[(int)ENUMAlanlar.HARCAMABIRIMKOD], "LIKE", tk.harcamaBirimi.kod.Replace(".", "") + "%"));

            if (!string.IsNullOrEmpty(tk.belgeNo))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("A", ALANLAR[(int)ENUMAlanlar.BELGENO], "=", tk.belgeNo));

            if (!string.IsNullOrEmpty(tk.talepEdenTCNo))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("A", ALANLAR[(int)ENUMAlanlar.TALEPEDENTCNO], "=", tk.talepEdenTCNo));

            if (tk.tur > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("A", ALANLAR[(int)ENUMAlanlar.TUR], "=", tk.tur));

            if (tk.amac > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("A", ALANLAR[(int)ENUMAlanlar.AMAC], "=", tk.amac));

            if (tk.durum > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("A", ALANLAR[(int)ENUMAlanlar.DURUM], "=", tk.durum));
            else
            {
                if (tk.refId == "" && tk.belgeNo == "")
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("A", ALANLAR[(int)ENUMAlanlar.DURUM], "<", (int)ENUMTalepKarsilamaDepoDurum.TAMAMLANDI));
                else if (tk.belgeNo == "")
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("A", ALANLAR[(int)ENUMAlanlar.DURUM], "<", (int)ENUMTalepKarsilamaDurum.IPTAL));
            }

            if (tk.refId != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("A", ALANLAR[(int)ENUMAlanlar.REFID], "=", tk.refId));

            if (tk.ilkTarih != null && !tk.ilkTarih.isNull)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("A", ALANLAR[(int)ENUMAlanlar.BELGETARIHI], ">=", tk.ilkTarih));

            if (tk.sonTarih != null && !tk.sonTarih.isNull)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("A", ALANLAR[(int)ENUMAlanlar.BELGETARIHI], "<=", tk.sonTarih));

            if (!string.IsNullOrEmpty(kosul))
                sql += " WHERE " + kosul;

            return veriErisim.DataReaderDondur(sql);
        }

        public Sonuc TalepKarsilananMiktarGuncelle(TalepKarsilamaDetay tk)
        {
            string sql = "UPDATE " + TABTalepKarsilamaDetay.TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl("", TABTalepKarsilamaDetay.ALANLAR[(int)TABTalepKarsilamaDetay.ENUMAlanlar.KARSILANANMIKTAR], tk.karsilananMiktar) + ",";
            sql += VeriErisim.AtamaOl("", TABTalepKarsilamaDetay.ALANLAR[(int)TABTalepKarsilamaDetay.ENUMAlanlar.SATINALINANMIKTAR], tk.satinAlinanMiktar);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", TABTalepKarsilamaDetay.ALANLAR[(int)TABTalepKarsilamaDetay.ENUMAlanlar.REFID], "=", tk.refId));

            return KomutCalistir(sql, 1);
        }

        public Sonuc KaydetCevap(TalepKarsilamaCevap tk)
        {
            string sql = "INSERT INTO " + TABLOADCEVAP + " (" + VeriErisim.TumAlanlar("", ALANLARCEVAP) + ") VALUES (";
            sql += VeriErisim.DegerYap(tk.refID) + ",";
            sql += VeriErisim.DegerYap(tk.refIDAna) + ",";
            sql += VeriErisim.DegerYap(tk.tarih) + ",";
            sql += VeriErisim.DegerYap(tk.aciklama) + ",";
            sql += VeriErisim.DegerYap(tk.durum) + ",";
            sql += VeriErisim.DegerYap(tk.karsilayanAdSoyad) + ",";
            sql += VeriErisim.DegerYap(tk.karsilayanUnvan) + ")";

            return KomutCalistir(sql, 1);
        }

        public Sonuc GuncelleCevap(TalepKarsilamaCevap tk)
        {
            string sql = "UPDATE " + TABLOADCEVAP + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLARCEVAP[(int)ENUMAlanlarCevap.TARIH], tk.tarih) + ",";
            sql += VeriErisim.AtamaOl("", ALANLARCEVAP[(int)ENUMAlanlarCevap.KARSILAYANADISOYADI], tk.karsilayanAdSoyad) + ",";
            sql += VeriErisim.AtamaOl("", ALANLARCEVAP[(int)ENUMAlanlarCevap.KARSILAYANUNVAN], tk.karsilayanUnvan) + ",";
            sql += VeriErisim.AtamaOl("", ALANLARCEVAP[(int)ENUMAlanlarCevap.ACIKLAMA], tk.aciklama) + ",";
            sql += VeriErisim.AtamaOl("", ALANLARCEVAP[(int)ENUMAlanlarCevap.DURUM], tk.durum);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARCEVAP[(int)ENUMAlanlarCevap.REFID], "=", tk.refID));

            return KomutCalistir(sql, 1);
        }

        public Sonuc TamamlaCevap(TalepKarsilamaCevap tk)
        {
            string sql = "UPDATE " + TABLOADCEVAP + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLARCEVAP[(int)ENUMAlanlarCevap.DURUM], 2);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARCEVAP[(int)ENUMAlanlarCevap.REFID], "=", tk.refID));

            return KomutCalistir(sql, 1);
        }

        public Sonuc SilCevap(TalepKarsilamaCevap tk)
        {
            string sql = "DELETE FROM " + TABLOADCEVAP;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARCEVAP[(int)ENUMAlanlarCevapDetay.REFID], "=", tk.refID));

            return KomutCalistir(sql, 0);
        }

        public Sonuc SilCevapDetay(TalepKarsilamaCevap tk)
        {
            string sql = "DELETE FROM " + TABLOADCEVAPDETAY;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARCEVAPDETAY[(int)ENUMAlanlarCevapDetay.REFIDANA], "=", tk.refID));

            return KomutCalistir(sql, 0);
        }

        public Sonuc KaydetCevapDetay(TalepKarsilamaCevapDetay tk)
        {
            string sql = "INSERT INTO " + TABLOADCEVAPDETAY + " (" + VeriErisim.TumAlanlar("", ALANLARCEVAPDETAY) + ") VALUES (";
            sql += VeriErisim.DegerYap(tk.refID) + ",";
            sql += VeriErisim.DegerYap(tk.refIDAna) + ",";
            sql += VeriErisim.DegerYap(tk.refIDTalep) + ",";
            sql += VeriErisim.DegerYap(tk.tur) + ",";
            sql += VeriErisim.DegerYap(tk.hesapPlani.hesapKod.Replace(".", "")) + ",";
            sql += VeriErisim.DegerYap(tk.hesapPlani.aciklama) + ",";
            sql += VeriErisim.DegerYap(tk.hesapPlani.olcuBirimAd) + ",";
            sql += VeriErisim.DegerYap(1) + ",";
            sql += VeriErisim.DegerYap(tk.miktar) + ")";

            return KomutCalistir(sql, 1);
        }

        public IDataReader ListeleCevap(TalepKarsilamaCevap tk)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar("A", ALANLARCEVAP);
            sql += " FROM " + TABLOADCEVAP + " A";

            string kosul = "";
            if (!string.IsNullOrEmpty(tk.refIDAna))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("A", ALANLARCEVAP[(int)ENUMAlanlarCevap.REFIDANA], "=", tk.refIDAna));

            if (!string.IsNullOrEmpty(tk.refID))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("A", ALANLARCEVAP[(int)ENUMAlanlarCevap.REFID], "=", tk.refID));

            if (!string.IsNullOrEmpty(kosul))
                sql += " WHERE " + kosul;

            return veriErisim.DataReaderDondur(sql);
        }

        public IDataReader ListeleCevapDetay(TalepKarsilamaCevapDetay tk)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOADCEVAPDETAY, ALANLARCEVAPDETAY);
            sql += " FROM " + TABLOADCEVAPDETAY;
            //sql += " INNER JOIN TASHESAPPLAN ON A.HESAPPLANKOD = TASHESAPPLAN.KOD";
            //sql += " INNER JOIN TASOLCUBIRIM ON TASHESAPPLAN.OLCUBIRIMKOD = TASOLCUBIRIM.KOD";

            //sql += " FROM TASOLCUBIRIM RIGHT OUTER JOIN";
            //sql += " TASHESAPPLAN ON TASOLCUBIRIM.KOD = TASHESAPPLAN.OLCUBIRIMKOD RIGHT OUTER JOIN";
            //sql += " TASTALEPKARSILAMACEVAPDETAY ON TASHESAPPLAN.KOD = TASTALEPKARSILAMACEVAPDETAY.HESAPPLANKOD LEFT OUTER JOIN";
            //sql += " ButceIslemleri.dbo.MUHHESAPPLANI ON ButceIslemleri.dbo.MUHHESAPPLANI.YIL=" + DateTime.Now.Year + " AND ";
            //sql += " TASTALEPKARSILAMACEVAPDETAY.HESAPPLANKOD = ButceIslemleri.dbo.MUHHESAPPLANI.HESAPKOD";

            string kosul = "";
            if (!string.IsNullOrEmpty(tk.refIDAna))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADCEVAPDETAY, ALANLARCEVAPDETAY[(int)ENUMAlanlarCevapDetay.REFIDANA], "=", tk.refIDAna));

            if (!string.IsNullOrEmpty(tk.refID))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADCEVAPDETAY, ALANLARCEVAPDETAY[(int)ENUMAlanlarCevapDetay.REFID], "=", tk.refID));

            if (!string.IsNullOrEmpty(tk.tur))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADCEVAPDETAY, ALANLARCEVAPDETAY[(int)ENUMAlanlarCevapDetay.TUR], "=", tk.tur));

            if (!string.IsNullOrEmpty(kosul))
                sql += " WHERE " + kosul;

            return veriErisim.DataReaderDondur(sql);
        }

        public IDataReader ListeleCevapDetaySatinAlma(TalepKarsilamaCevap tk)
        {
            string sql = "SELECT ";
            sql += " TASTALEPKARSILAMACEVAPDETAY.REFID, TASTALEPKARSILAMACEVAPDETAY.REFIDANA, TASTALEPKARSILAMACEVAPDETAY.REFIDTALEP, TASTALEPKARSILAMACEVAPDETAY.TUR, TASTALEPKARSILAMACEVAPDETAY.HESAPPLANKOD, TASTALEPKARSILAMACEVAPDETAY.HESAPPLANAD, TASTALEPKARSILAMACEVAPDETAY.OLCUBIRIMI, TASTALEPKARSILAMACEVAPDETAY.SATINALMADURUM, TASTALEPKARSILAMACEVAPDETAY.MIKTAR, ";
            sql += " TASTALEPKARSILAMACEVAP.REFID, TASTALEPKARSILAMACEVAP.REFIDANA, TASTALEPKARSILAMACEVAP.TARIH, TASTALEPKARSILAMACEVAP.ACIKLAMA, TASTALEPKARSILAMACEVAP.DURUM, TASTALEPKARSILAMACEVAP.KARSILAYANADISOYADI, TASTALEPKARSILAMACEVAP.KARSILAYANUNVAN, ";
            sql += " TASTALEPKARSILAMADETAY.YIL, TASTALEPKARSILAMADETAY.MUHASEBEKOD, TASTALEPKARSILAMADETAY.HARCAMABIRIMKOD, TASHARCAMABIRIM.AD, TASTALEPKARSILAMADETAY.BELGENO, TASTALEPKARSILAMADETAY.ACIKLAMA ";
            sql += " FROM TASTALEPKARSILAMACEVAP INNER JOIN ";
            sql += " TASTALEPKARSILAMACEVAPDETAY ON TASTALEPKARSILAMACEVAP.REFID = TASTALEPKARSILAMACEVAPDETAY.REFIDANA INNER JOIN ";
            sql += " TASTALEPKARSILAMADETAY ON TASTALEPKARSILAMACEVAPDETAY.REFIDTALEP = TASTALEPKARSILAMADETAY.REFID INNER JOIN ";
            sql += " TASTALEPKARSILAMA ON TASTALEPKARSILAMACEVAP.REFIDANA = TASTALEPKARSILAMA.REFID INNER JOIN ";
            sql += " TASHARCAMABIRIM ON TASTALEPKARSILAMADETAY.MUHASEBEKOD = TASHARCAMABIRIM.MUHASEBEKOD AND TASTALEPKARSILAMADETAY.HARCAMABIRIMKOD = TASHARCAMABIRIM.KOD ";
            sql += " WHERE  (TASTALEPKARSILAMACEVAPDETAY.TUR = 'satinAl') AND (TASTALEPKARSILAMACEVAP.DURUM > 1) AND (TASTALEPKARSILAMA.DURUM<90)";

            if (tk.muhasebe.kod != "")
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("TASTALEPKARSILAMADETAY", "MUHASEBEKOD", "=", tk.muhasebe.kod));

            if (tk.harcamaBirimi.kod != "")
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("TASTALEPKARSILAMADETAY", "HARCAMABIRIMKOD", "=", tk.harcamaBirimi.kod.Replace(".", "")));

            if (tk.talepBelgeNo != "")
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("TASTALEPKARSILAMADETAY", "BELGENO", "=", tk.talepBelgeNo));

            if (!tk.tarih.isNull)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("TASTALEPKARSILAMACEVAP", "TARIH", ">=", tk.tarih));

            if (!tk.tarihSorgu.isNull)
            {
                tk.tarihSorgu.AddDays(1);
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("TASTALEPKARSILAMACEVAP", "TARIH", "<=", tk.tarihSorgu));
            }

            return veriErisim.DataReaderDondur(sql);
        }

        public bool TalepKarsilamaVarMi(TalepKarsilama tk)
        {
            string[] intAlan = { ALANLAR[(int)ENUMAlanlar.YIL] };
            int[] intDeger = { tk.yil };
            string[] strAlan = { ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], ALANLAR[(int)ENUMAlanlar.HARCAMABIRIMKOD], ALANLAR[(int)ENUMAlanlar.BELGENO] };
            string[] strDeger = { tk.muhasebe.kod, tk.harcamaBirimi.kod.Replace(".", ""), tk.belgeNo };

            return TABGenel.KayitVarmi(veriErisim, TABLOAD, strAlan, strDeger, intAlan, intDeger);
        }

        public int DurumBul(TalepKarsilama tk)
        {
            string sql = "SELECT " + ALANLAR[(int)ENUMAlanlar.DURUM] + " FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(string.Empty, ALANLAR[(int)ENUMAlanlar.YIL], "=", tk.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(string.Empty, ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", tk.muhasebe.kod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(string.Empty, ALANLAR[(int)ENUMAlanlar.HARCAMABIRIMKOD], "=", tk.harcamaBirimi.kod.Replace(".", "")));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(string.Empty, ALANLAR[(int)ENUMAlanlar.BELGENO], "=", tk.belgeNo));

            return veriErisim.TekKolonSQLInt(sql);
        }

        public string OncedenTalepVarmi(string muhasebeKod, string harcamaKod, string belgeNo, TalepKarsilamaDetay tkd)
        {
            string sql = "SELECT convert(varchar,TASTALEPKARSILAMA.YIL)+'-'+TASTALEPKARSILAMA.BELGENO FROM TASTALEPKARSILAMADETAY INNER JOIN TASTALEPKARSILAMA ON TASTALEPKARSILAMADETAY.YIL = TASTALEPKARSILAMA.YIL AND TASTALEPKARSILAMADETAY.MUHASEBEKOD = TASTALEPKARSILAMA.MUHASEBEKOD AND TASTALEPKARSILAMADETAY.HARCAMABIRIMKOD = TASTALEPKARSILAMA.HARCAMABIRIMKOD AND TASTALEPKARSILAMADETAY.BELGENO = TASTALEPKARSILAMA.BELGENO ";
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", muhasebeKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.HARCAMABIRIMKOD], "=", harcamaKod.Replace(".", "")));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.DURUM], "<", (int)ENUMTalepKarsilamaDepoDurum.TAMAMLANDI));

            if (belgeNo != "")
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.BELGENO], "<>", OrtakFonksiyonlar.ConvertToInt(belgeNo, 0).ToString()));

            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("TASTALEPKARSILAMADETAY", "HESAPPLANKOD", "=", tkd.hesapPlani.hesapKod.Replace(".", "")));
            sql += " AND TASTALEPKARSILAMADETAY.ISTENILENMIKTAR - TASTALEPKARSILAMADETAY.KARSILANANMIKTAR > 0 AND TASTALEPKARSILAMADETAY.IPTAL=0";

            return veriErisim.TekKolonSQLStr(sql);
        }
    }
}