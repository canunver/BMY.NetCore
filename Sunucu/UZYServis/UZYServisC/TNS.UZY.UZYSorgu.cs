using System;
using TNS.Veri;
using System.Xml;

namespace TNS.UZY
{
    public class Uzaylar
    {
        public string ad = "";
        TNSCollection uzaylar = new TNSCollection();
        public bool ilkGiris = true;

        public static string NodeAttr(XmlNode u, string attrAd)
        {
            XmlAttribute attr = u.Attributes[attrAd];
            if (attr != null)
            {
                return attr.Value;
            }
            else
                return null;
        }

        public void UzaylariOku()
        {
            uzaylar.Clear();
            string dosyaAd = System.Configuration.ConfigurationManager.AppSettings.Get("UzayDosyasi");
            try
            {
                System.Xml.XmlDocument x = new System.Xml.XmlDocument();
                x.Load(dosyaAd);
                for (int i = 0; i < x.ChildNodes.Count; i++)
                {
                    XmlNode ana = x.ChildNodes[i];
                    if (ana.Name == "UZAYLAR")
                    {
                        for (int j = 0; j < ana.ChildNodes.Count; j++)
                        {
                            XmlNode u = ana.ChildNodes[j];
                            if (u.Name == "UZAY")
                            {
                                Uzay uz = null;
                                string dll = NodeAttr(u, "DLL");
                                if (!string.IsNullOrEmpty(dll))
                                    uz = new Uzay(NodeAttr(u, "AD"), NodeAttr(u, "TIP"), NodeAttr(u, "XLLAD"), NodeAttr(u, "XLLKATEGORI"), NodeAttr(u, "XLLYARDIM"), dll, NodeAttr(u, "NESNE"), NodeAttr(u, "EKPARAM"), NodeAttr(u, "VERITABANIPARAMAD"), "");
                                else
                                    uz = new Uzay(NodeAttr(u, "AD"), NodeAttr(u, "FROM"), NodeAttr(u, "WHERE"), NodeAttr(u, "SECALAN"), NodeAttr(u, "TIP"), NodeAttr(u, "VERITABANIPARAMAD"), NodeAttr(u, "XLLAD"), NodeAttr(u, "XLLKATEGORI"), NodeAttr(u, "XLLYARDIM"));
                                for (int k = 0; k < u.ChildNodes.Count; k++)
                                {
                                    XmlNode a = u.ChildNodes[k];
                                    if (a.Name == "ALAN")
                                    {
                                        uz.Add(new UzayAlan(NodeAttr(a, "AD"), NodeAttr(a, "MASKE"), NodeAttr(a, "ZORUNLU"), NodeAttr(a, "KURUMLIMIT"), NodeAttr(a, "BOL2005"), NodeAttr(a, "XLLAD"), NodeAttr(a, "XLLYARDIM"), NodeAttr(a, "MUHASEBATAY")));
                                    }
                                }
                                Ekle(uz);
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                Console.WriteLine("Hata.....");
            }
        }

        public void Ekle(Uzay u)
        {
            uzaylar.Add(u);
        }

        public Uzay UzayBul(string uzayAd)
        {
            if (uzaylar.Count == 0)
                UZY.UZYServis.UzaylariYenile(null);

            for (int i = 0; i < uzaylar.Count; i++)
            {
                if (((Uzay)uzaylar[i]).ad == uzayAd)
                    return (Uzay)uzaylar[i];
            }
            return null;
        }

        public Uzay UzayBul(int uzayNo)
        {
            return (Uzay)uzaylar[uzayNo];
        }

        public int UzaySayisi()
        {
            return uzaylar.Count;
        }

        public TNSCollection UzaylariDondur()
        {
            return uzaylar;
        }

        public string SQLKosulOl(Uzay u, string baglantiAd, string uzayKosul, bool esitle, ref bool bolumYap, out string eksikZorunluAlanlar)
        {
            eksikZorunluAlanlar = "";
            UzayAlan ua;
            string sql = "", kosul;
            bolumYap = false;
            if (u != null)
            {
                string[] d = uzayKosul.Split('-');
                int say = d.Length;
                for (int i = 0; i < u.AlanSay(); i++)
                {
                    ua = u.AlanBul(i);
                    if (ua != null)
                    {
                        if (i < say)
                            kosul = d[i];
                        else
                            kosul = "";
                        if (ua.zorunlu && kosul == "")
                        {
                            eksikZorunluAlanlar += ua.ad + ",";
                        }
                        if (ua.bol2005)
                        {
                            if (kosul.CompareTo("2005") < 0)
                                bolumYap = true;
                        }

                        if (kosul != "")
                        {
                            string maske = ua.MaskeBul(d[0], baglantiAd);

                            kosul = UzayKosul.KosulOlustur("", ua.ad, maske, kosul, esitle, ua.kurumLimit, ua.muhasebatAy);
                            if (kosul != "")
                            {
                                if (sql != "") sql += " AND ";
                                sql += "(" + kosul + ")";
                            }
                        }
                    }
                    else
                        eksikZorunluAlanlar += i.ToString() + ",";
                }

            }
            return sql;
        }

        public string UzayDegeriSQL(string uzayAd, string baglantiAd, string alanAd, string uzayKosul, bool esitle, ref bool bolumYap, bool zorunluKontrol)
        {
            Uzay u = UzayBul(uzayAd);
            if (u == null) return "";
            string eksikAlanlar;
            string sqlKosul = SQLKosulOl(u, baglantiAd, uzayKosul, esitle, ref bolumYap, out eksikAlanlar);

            if (zorunluKontrol && !string.IsNullOrEmpty(eksikAlanlar))
                return "";
            if (!string.IsNullOrEmpty(sqlKosul))
            {
                if (u.where != "")
                    sqlKosul = u.where + " AND " + sqlKosul;
            }
            else
                sqlKosul = u.where;

            if (sqlKosul != "") sqlKosul = " WHERE " + sqlKosul;
            
            if (alanAd == null || alanAd == "")
                alanAd = u.secAlan;
            return "SELECT " + alanAd + " FROM " + u.from + " " + sqlKosul;
        }

    }
}
