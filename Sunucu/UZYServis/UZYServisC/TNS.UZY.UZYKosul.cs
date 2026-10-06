using System;
using TNS.Veri;

namespace TNS.UZY
{
    public class UzayKosul
    {
        public static string muhasebatTur = System.Configuration.ConfigurationManager.AppSettings.Get("MuhasebatTur") + "";

        private static string KosulOlusturBaglac(string tabloOnEk, string maske, string vtAlanKod, string kodSorgu, string baglac, bool muhasebatAy)
        {
            if (maske != "N")
            {
                kodSorgu = VeriErisim.DegerYap(kodSorgu);
            }
            else
            {
                kodSorgu = VeriErisim.DegerYap(OrtakFonksiyonlar.ConvertToDbl(kodSorgu));
            }
            if (maske.StartsWith("9"))
                kodSorgu = kodSorgu.Replace(".", "");

            if (muhasebatAy && baglac == "=" && !string.IsNullOrEmpty(muhasebatTur)) baglac = "<=";
            return tabloOnEk + vtAlanKod + " " + baglac + " " + kodSorgu;
        }

        private static string KosulEkle(string onKosul, string baglac, string ekKosul)
        {
            if (onKosul != "" && baglac != "")
                return onKosul + " " + baglac + " " + ekKosul;
            else
                return ekKosul;
        }

        private static string SubStrKosulOl(string tabloOnEk, string vtAlanKod, int kodPos, int uzunluk, string deger, string baglac)
        {
            if ((kodPos == 0) && (baglac.Trim() == "="))
                return VeriErisim.KosulOl(tabloOnEk, vtAlanKod, "LIKE", deger + "%");
            else
                return VeriErisim.IslecBul("SUBSTRING") + "(" + tabloOnEk + vtAlanKod + "," + (kodPos + 1).ToString() + "," + uzunluk.ToString() + ") " + baglac + VeriErisim.DegerYap(deger);
        }

        private static string KosulOlusturLike(string tabloOnEk, string vtAlanKod, string deger, string maske)
        {
            if (maske == "E")
                if (deger.Contains("%"))
                    return VeriErisim.KosulOl(tabloOnEk, vtAlanKod, "LIKE", deger);
                else
                    return VeriErisim.KosulOl(tabloOnEk, vtAlanKod, "=", deger);
            return VeriErisim.KosulOl(tabloOnEk, vtAlanKod, "LIKE", deger + "%");
        }

        private static string KosulOlusturAlan(string tabloOnEk, string vtAlanKod, string maske, string[] kodBolumler, string kodSorgu, bool kurumLimitiEkle, string araKosul, bool muhasebatAy)
        {
            string kosul = "", icKosul = "";
            if (maske == "S" || maske == "E")
            {
                if (araKosul == "=")
                    kosul = KosulOlusturLike(tabloOnEk, vtAlanKod, kodSorgu, maske);
                else
                    kosul = KosulOlusturBaglac(tabloOnEk, maske, vtAlanKod, kodSorgu, araKosul, muhasebatAy);
            }
            else if (maske == "N")
                kosul = KosulOlusturBaglac(tabloOnEk, maske, vtAlanKod, kodSorgu, araKosul, muhasebatAy);
            else
            {
                string[] bolumler = kodSorgu.Split('.');
                int bolumSay = kodBolumler.Length;
                int kodSorguSay = bolumler.Length;
                int kodPos = 0;
                int enKucukBolum = 0;
                if (kodSorguSay < bolumSay) enKucukBolum = kodSorguSay;
                else enKucukBolum = bolumSay;

                for (int i = 0; i < enKucukBolum; i++)
                {
                    icKosul = "";
                    if (bolumler[i].Length == kodBolumler[i].Length)
                    {
                        string icKod = bolumler[i];
                        int ison = i + 1;
                        while (ison < enKucukBolum && (bolumler[ison].Length == kodBolumler[ison].Length))
                        {
                            icKod += bolumler[ison];
                            ison++;
                        }
                        ison--;
                        if (i == 0)
                        {
                            if (ison == bolumSay - 1)
                            {
                                icKosul = KosulOlusturBaglac(tabloOnEk, maske, vtAlanKod, icKod, araKosul, muhasebatAy);
                            }
                            else
                            {
                                if (i == 0 && ison == 0 && (kurumLimitiEkle) && icKod.CompareTo(OrtakFonksiyonlar.GenelButceSonKod()) < 0)
                                {
                                    icKosul = "(" + KosulOlusturBaglac(tabloOnEk, maske, vtAlanKod, icKod, ">=", muhasebatAy) + " AND " + KosulOlusturBaglac(tabloOnEk, maske, vtAlanKod, icKod + OrtakFonksiyonlar.BagimsizKurumBaslangic(), "<", muhasebatAy) + ")";
                                }
                                else
                                {
                                    if (araKosul == "=")
                                        kosul = KosulOlusturLike(tabloOnEk, vtAlanKod, icKod, maske);
                                    else
                                        kosul = KosulOlusturBaglac(tabloOnEk, maske, vtAlanKod, icKod, araKosul, muhasebatAy);
                                }
                            }
                        }
                        else
                            icKosul = SubStrKosulOl(tabloOnEk, vtAlanKod, kodPos, icKod.Length, icKod, araKosul);
                        kodPos += icKod.Length;
                        i = ison;

                    }
                    else if ((bolumler[i]).Length > 0)
                    {
                        icKosul = SubStrKosulOl(tabloOnEk, vtAlanKod, kodPos, bolumler[i].Length, bolumler[i], araKosul);
                        kodPos += kodBolumler[i].Length;
                    }
                    else
                        kodPos += kodBolumler[i].Length;

                    if (icKosul != "")
                        kosul = KosulEkle(kosul, "AND", icKosul);
                }
            }
            return kosul;
        }

        private static string KosulOlusturAralik(string tabloOnEk, string vtAlanKod, string maske, string kodSorgu, bool kurumLimitiEkle, bool muhasebatAy)
        {
            string kosul = "", kucukBolum = "", buyukBolum = "";
            if (kodSorgu != "")
            {
                if (maske != "S" && maske != "E")
                {
                    if (kodSorgu.IndexOf("/") >= 0)
                    {
                        OrtakFonksiyonlar.StringiIkiyeBol(kodSorgu, "/", ref kucukBolum, ref buyukBolum);
                    }
                }


                string[] d = maske.Split('.');

                if (kucukBolum != "" || buyukBolum != "")
                {
                    kosul = "(" + (kucukBolum != "" ? KosulOlusturAlan(tabloOnEk, vtAlanKod, maske, d, kucukBolum, false, ">=", false) : "") + ((kucukBolum != "" && buyukBolum != "") ? " AND " : "") + (buyukBolum != "" ? KosulOlusturAlan(tabloOnEk, vtAlanKod, maske, d, buyukBolum, false, "<", false) : "") + ")";
                }
                else
                {
                    kosul = KosulOlusturAlan(tabloOnEk, vtAlanKod, maske, d, kodSorgu, kurumLimitiEkle, "=", muhasebatAy);
                }
            }
            return kosul;
        }

        public static string KosulOlustur(string tabloOnEk, string vtAlanKod, string maske, string kodSorgu, bool esitle, bool kurumLimitiEkle, bool muhasebatAy)
        {
            string kosul = "";
            if (tabloOnEk != "") tabloOnEk += ".";
            if (kodSorgu != "")
            {
                if (esitle)
                {
                    if (maske.StartsWith("9"))
                        kosul = kodSorgu.Replace(".", "");
                    else
                        kosul = kodSorgu;
                    kosul = KosulOlusturBaglac(tabloOnEk, maske, vtAlanKod, kosul, "=", muhasebatAy);
                }
                else
                {
                    string[] kosullar;
                    int say;
                    if (maske != "X")//Muhasebe birimkod , ile ayıraçlı olarak gönderiliyor
                    {
                        kosullar = kodSorgu.Split(',');
                        say = kosullar.Length;
                    }
                    else
                    {
                        kosullar = new string[1];
                        kosullar[0] = kodSorgu;
                        say = 1;
                    }
                    for (int i = 0; i < say; i++)
                    {
                        if (kosul != "") kosul += " OR ";
                        kosul += KosulOlusturAralik(tabloOnEk, vtAlanKod, maske, kosullar[i], kurumLimitiEkle, muhasebatAy);
                    }
                    if (say > 1)
                        kosul = "(" + kosul + ")";
                }
            }
            return kosul;
        }
    }
}
