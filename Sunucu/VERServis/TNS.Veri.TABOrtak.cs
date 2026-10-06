using System;
using TNS;
using TNS.Veri;
using System.Data;

namespace TNS.Veri
{
    public class TABOrtak
    {
        public static string IcSorguOl(string icSql)
        {
            return IcSorguOl(icSql, "\r\n");
        }

        public static string IcSorguOl(string icSql, string onStr)
        {
            return onStr + "(" + icSql + "  FOR XML RAW, ELEMENTS xsinil, TYPE)";
        }

        private static bool? _tekDB = null;
        public static bool TekDB
        {
            get
            {
                if (_tekDB == null)
                {
                    string okunan = System.Configuration.ConfigurationManager.AppSettings.Get("TEKDB");
                    if (okunan == "1") _tekDB = true;
                    else _tekDB = false;
                }
                return _tekDB.Value;
            }

        }

        public static string DatabaseAdBul(string onAd)
        {
            if (TekDB) return "";
            return onAd;
        }

        protected VeriErisim veriErisim;

        public TABOrtak(VeriErisim pVeriErisim)
        {
            veriErisim = pVeriErisim;
        }

        public string Hata()
        {
            return veriErisim.hata;
        }

        protected Sonuc KomutCalistir(string sql, int beklenenKayitSay)
        {
            return KomutCalistir(sql, beklenenKayitSay, veriErisim.sureAsimi);
        }

        protected Sonuc KomutCalistir(string sql, int beklenenKayitSay, int timeOut)
        {
            Sonuc sonuc = new Sonuc(false, "", "");
            int donusDeger;

            if (veriErisim.hata == "")
            {
                donusDeger = veriErisim.ExecuteNonquery(sql, timeOut);
                sonuc.kayitSay = donusDeger;

                if (veriErisim.hata != "")
                    sonuc.hataStr = veriErisim.hata;
                else
                {
                    if (beklenenKayitSay != 0)
                    {
                        if (donusDeger != beklenenKayitSay)
                        {
                            string h = "";
                            if (donusDeger == 0)
                                h = "Verilen koşullara uygun kayıt bulunamadı veya kayıt eklenemedi";
                            else
                                h = "Verilen koşullara uygun beklenen sayıdan daha fazla kayıt güncellendi.";

                            if (h != "")
                            {
                                sonuc.hataStr = h;
                                veriErisim.Logla(sql, 3, h);
                            }
                        }
                        else
                            sonuc.islemSonuc = true;
                    }
                    else
                        sonuc.islemSonuc = true;
                }
            }
            else
                sonuc.hataStr = veriErisim.hata;

            return sonuc;
        }
    }
}

