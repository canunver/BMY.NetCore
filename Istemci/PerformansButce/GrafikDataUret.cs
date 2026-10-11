using System;
using System.Collections.Generic;
using System.Data;
using Istemci.OrtakSayfa;
using TNS.KYM;
using TNS.PEB;
using TNS.UZY;

// Eski Istemci\PerformanButceBirim\GrafikDataUret.cs içinden: stratejik plan grafiklerinin (sp1, sp2) verisini hazırlayan iki yordam
// (SPBasariSeyriDataTableOl, SPBasariDuzeyiDataTableOl) aynen alındı. Eski sınıf veriyi Ext JS grafik tanımına çeviriyordu
// (TNSChart); burada aynı veri tarayıcıda çizilen sade grafik verisine (GrafikVeri) çevrilir. Sınıftaki öteki grafikler
// (okul / sınav grafikleri) ait oldukları sayfalarla birlikte taşınacak.
namespace PerformansButce
{
    public class GrafikDataUret
    {
        // Başarı düzeylerinin renkleri (1 en iyi ... 4 en düşük): satır renklerinin koyuları
        private static readonly string[] renkler = { "#4c9a4c", "#3d8fc4", "#d9a300", "#cf4b47" };
        private static readonly string[] duzeyAdlari = { "Başarı (%90-100)", "Yeterli (%75-89)", "Kısmen Yeterli (%50-74)", "Yetersiz (%0-49)" };

        /// <summary>param: "1-yıl-kurum-t-s-d-grafikTür" (eski GrafikGoster.aspx?param=...). Taşınan türler: sp1, sp2.</summary>
        public GrafikVeri Uret(Kullanici kullanan, string[] paramParca)
        {
            if (paramParca.Length < 7) return null;
            int yil = TNS.OrtakFonksiyonlar.ConvertToInt(paramParca[1], 0);
            // Kurum kodu içinde "-" olmaz; tür her zaman son parçadır.
            string kurOrOrg = paramParca[2];
            string grafikTur = paramParca[6];
            if (grafikTur == "sp1") return GrafikSP1(kullanan, yil, kurOrOrg);
            if (grafikTur == "sp2") return GrafikSP2(kullanan, yil, kurOrOrg);
            return null;
        }

        private GrafikVeri GrafikSP1(Kullanici kullanan, int yil, string kurOrOrg)
        {
            DataTable dt = SPBasariDuzeyiDataTableOl(TNS.UZY.Arac.Tanimla(), kullanan, kurOrOrg, yil);

            GrafikVeri g = new GrafikVeri { baslik = "Başarı Düzeyine Göre Hedef Dağılımı", tur = "pasta" };
            GrafikSeri seri = new GrafikSeri { ad = "Hedef Sayısı" };
            foreach (DataRow dr in dt.Rows)
            {
                string ad = (string)dr[0];
                g.etiketler.Add(ad);
                seri.degerler.Add((double)dr[1]);
                g.renkler.Add(renkler[Math.Max(0, Array.IndexOf(duzeyAdlari, ad))]);
            }
            g.seriler.Add(seri);
            return g;
        }

        private GrafikVeri GrafikSP2(Kullanici kullanan, int yil, string kurKod)
        {
            string[] kodlar = { "90 ve üzeri", "75-89", "50-74", "0-49" };
            DataTable dt = SPBasariSeyriDataTableOl(TNS.UZY.Arac.Tanimla(), kullanan, kurKod, yil, kodlar);

            GrafikVeri g = new GrafikVeri { baslik = "Stratejik Planın Başarı Seyri", tur = "cizgi" };
            foreach (DataRow dr in dt.Rows) g.etiketler.Add(Convert.ToString(dr[0]));
            for (int i = 0; i < kodlar.Length; i++)
            {
                GrafikSeri seri = new GrafikSeri { ad = kodlar[i], renk = renkler[i] };
                foreach (DataRow dr in dt.Rows) seri.degerler.Add((double)dr[i + 1]);
                g.seriler.Add(seri);
            }
            return g;
        }

        public static DataTable SPBasariSeyriDataTableOl(IUZYServis servisUZY, Kullanici kullanan, string kurumKod, int yil, string[] kodlar)
        {
            Misyon ms = null;
            IPEBServis servisPEB = TNS.PEB.Arac.Tanimla();
            int[,] basariSay = null;

            List<PerformansButce.SPIzlemeKurum.IzlemeKurum> liste = PerformansButce.SPIzlemeKurum.IzlemeGenelOlustur(kullanan, servisPEB, yil, 4, kurumKod, -1, -1, out ms, out basariSay);

            try
            {
                System.Data.DataTable anaDT = new System.Data.DataTable();
                anaDT.Columns.Add("yil", typeof(int));
                anaDT.Columns.Add(kodlar[0], typeof(double));
                anaDT.Columns.Add(kodlar[1], typeof(double));
                anaDT.Columns.Add(kodlar[2], typeof(double));
                anaDT.Columns.Add(kodlar[3], typeof(double));

                for (int icYil = ms.aralikBasiYil; icYil <= Math.Min(yil, ms.aralikSonuYil); icYil++)
                {
                    int yer = icYil - ms.aralikBasiYil;
                    DataRow dr = anaDT.NewRow();
                    dr[0] = icYil;
                    dr[1] = basariSay[yer, 0];
                    dr[2] = basariSay[yer, 1];
                    dr[3] = basariSay[yer, 2];
                    dr[4] = basariSay[yer, 3];
                    anaDT.Rows.Add(dr);
                }
                return anaDT;
            }
            catch (Exception ex)
            {
                throw new Exception("Grafik verileri okunamadı!" + ex.Message);
            }
        }

        public static DataTable SPBasariDuzeyiDataTableOl(IUZYServis servisUZY, Kullanici kullanan, string kurumKod, int yil)
        {
            Misyon ms = null;
            IPEBServis servisPEB = TNS.PEB.Arac.Tanimla();
            int[,] basariSay = null;
            List<PerformansButce.SPIzlemeKurum.IzlemeKurum> liste = PerformansButce.SPIzlemeKurum.IzlemeGenelOlustur(kullanan, servisPEB, yil, 4, kurumKod, -1, -1, out ms, out basariSay);

            try
            {
                System.Data.DataTable anaDT = new System.Data.DataTable();
                anaDT.Columns.Add("ad", typeof(string));
                anaDT.Columns.Add("deger", typeof(double));
                anaDT.Columns.Add("aciklama", typeof(string));
                int yer = yil - ms.aralikBasiYil;
                DataRow dr = null;
                //DataTable dt = servisUZY.UzayDataTableDondur(kullanan, "STRPERFORMANSHAMVERI", TNS.UZY.Arac.UzayKosulOl(baslaYil.ToString() + "/" + (bitYil + 1).ToString(), "2", turBirimIlIlceUlke.ToString(), orgStr, "", bilgiKod), false, sorguAlan, alanlar);
                if (basariSay[yer, 0] > 0)
                {
                    dr = anaDT.NewRow();
                    dr[0] = "Başarı (%90-100)";
                    dr[1] = basariSay[yer, 0];
                    dr[2] = dr[0] + ": " + dr[1];
                    anaDT.Rows.Add(dr);
                }

                if (basariSay[yer, 1] > 0)
                {
                    dr = anaDT.NewRow();
                    dr[0] = "Yeterli (%75-89)";
                    dr[1] = basariSay[yer, 1];
                    dr[2] = dr[0] + ": " + dr[1];
                    anaDT.Rows.Add(dr);
                }

                if (basariSay[yer, 2] > 0)
                {
                    dr = anaDT.NewRow();
                    dr[0] = "Kısmen Yeterli (%50-74)";
                    dr[1] = basariSay[yer, 2];
                    dr[2] = dr[0] + ": " + dr[1];
                    anaDT.Rows.Add(dr);
                }

                if (basariSay[yer, 3] > 0)
                {
                    dr = anaDT.NewRow();
                    dr[0] = "Yetersiz (%0-49)";
                    dr[1] = basariSay[yer, 3];
                    dr[2] = dr[0] + ": " + dr[1];
                    anaDT.Rows.Add(dr);
                }
                return anaDT;
            }
            catch (Exception ex)
            {
                throw new Exception("Grafik verileri okunamadı!" + ex.Message);
            }
        }
    }
}
