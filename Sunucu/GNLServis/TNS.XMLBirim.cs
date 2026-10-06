using System;
using System.Collections;

namespace TNS
{
    [Serializable]
    public class XMLBirim
    {
        private string ad;
        private Hashtable ozellikler;
        private ArrayList cocuklar;

        public XMLBirim(string p_ad)
        {
            ad = p_ad;
            ozellikler = null;
            cocuklar = null;
        }

        public void OzellikEkle(string p_ozellikAd, string p_ozellikDeger)
        {
            if (ozellikler == null) ozellikler = new Hashtable();
            ozellikler[p_ozellikAd] = p_ozellikDeger;
        }

        public string OzellikAl(string ozellikAd)
        {
            if (ozellikler == null) return null;
            if (ozellikAd == null) return null;
            return (string)ozellikler[ozellikAd];
        }


        public void OzellikleriBirak()
        {
            if (ozellikler != null)
            {
                ozellikler.Clear();
                ozellikler = null;
            }
        }

        public void CocukEkle(XMLBirim cocuk)
        {
            if (cocuklar == null) cocuklar = new ArrayList();
            cocuklar.Add(cocuk);
        }

        public XMLBirim CocukAl(int i)
        {
            if (cocuklar == null) return null;
            if (i < 0) return null;
            if (i >= cocuklar.Count) return null;
            return (XMLBirim)cocuklar[i];
        }

        public void CocuklariBirak()
        {
            if (cocuklar != null)
            {
                cocuklar.Clear();
                cocuklar = null;
            }
        }

        public int CocukSayisi()
        {
            if (cocuklar == null) return 0;
            return cocuklar.Count;
        }

        public void Clear()
        {
            OzellikleriBirak();
            for (int i = 0; i < CocukSayisi(); i++)
            {
                ((XMLBirim)cocuklar[i]).Clear();
            }
            CocuklariBirak();
        }
        private static void OzellikleriOku(System.Xml.XmlTextReader x, XMLBirim xb)
        {
            for (int i = 0; i < x.AttributeCount; i++)
            {
                x.MoveToAttribute(i);
                xb.OzellikEkle(x.Name, x.Value);
            }
            xb.OzellikEkle("Derinlik", x.Depth.ToString());

        }

        private static void CocuklariOku(System.Xml.XmlTextReader x, XMLBirim baba)
        {
            while (x.Read())
            {
                if (x.NodeType == System.Xml.XmlNodeType.EndElement)
                {
                    return;
                }
                else if (x.NodeType == System.Xml.XmlNodeType.Element)
                {

                    XMLBirim xb = new XMLBirim(x.Name);
                    bool bos = x.IsEmptyElement;
                    OzellikleriOku(x, xb);
                    if (!bos)
                    {
                        CocuklariOku(x, xb);
                    }
                    baba.CocukEkle(xb);
                }

            }
        }

        private static XMLBirim XMLOku(string dosyaAd, string xmlBirimAd, ArrayList kosullar)
        {
            XMLBirim xb = null;
            if (dosyaAd == null || dosyaAd == "")
                return null;
            System.Xml.XmlTextReader x = null;
            try
            {
                x = new System.Xml.XmlTextReader(dosyaAd);
            }
            catch (Exception)
            {
                throw new Exception(dosyaAd + " dosyası bulunamadı!");
            }

            try
            {
                while (x.Read())
                {
                    if (x.NodeType == System.Xml.XmlNodeType.Element)
                    {
                        xb = new XMLBirim(x.Name);
                        OzellikleriOku(x, xb);
                        CocuklariOku(x, xb);
                    }
                }
            }
            catch (Exception e)
            {
                x.Close();
                if (xb != null)
                {
                    xb.Clear();
                    xb = null;
                }
                throw new Exception(dosyaAd + " dosyası okunurken hata ile karşılaşıldı! Hata: (" + e.ToString() + ")");
            }

            x.Close();
            return xb;
        }

        public static XMLBirim XMLOku(string dosyaAd)
        {
            return XMLOku(dosyaAd, null, null);
        }

    }

    public class XMLServis
    {

    }
}
