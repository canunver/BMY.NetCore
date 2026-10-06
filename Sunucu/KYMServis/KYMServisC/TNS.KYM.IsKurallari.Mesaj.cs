using System;
using System.Data;
using TNS;
using TNS.Veri;
using TNS.KYM.IsKurallari.Veri;
using System.Collections;
using System.Text;

namespace TNS.KYM.IsKurallari
{
    public class MesajServis
    {
        public Sonuc Kaydet(Mesaj mesaj)
        {
            Sonuc sonuc = new Sonuc(false, "", "");
            if (mesaj == null) { return sonuc; }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriKYM", null);
            TABMesaj to = new TABMesaj(ve);

            ve.BeginTransaction();

            sonuc = to.Kaydet(mesaj);
            if (sonuc.islemSonuc) { ve.Commit(); } else { ve.Rollback(); }

            ve.VeriErisimKapat();

            return sonuc;
        }

        public Mesaj[] Listele(Mesaj mesaj)
        {
            ObjectArray liste = new ObjectArray();
            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriKYM", null);
            TABMesaj to = new TABMesaj(ve);
            IDataReader dr = to.Listele(mesaj);

            if (to.Hata() == "" && dr != null)
            {
                while (dr.Read())
                {
                    Mesaj m = new Mesaj();
                    m.tarih = VeriErisim.DegerAl(dr, (int)TABMesaj.ENUMAlanlar.TARIH);
                    m.kullanici = VeriErisim.DegerAl(dr, (int)TABMesaj.ENUMAlanlar.KULLANICI, "");
                    m.alici = VeriErisim.DegerAl(dr, (int)TABMesaj.ENUMAlanlar.ALICI, "");
                    m.mesaj = VeriErisim.DegerAl(dr, (int)TABMesaj.ENUMAlanlar.MESAJ, "");
                    m.okundu = VeriErisim.DegerAl(dr, (int)TABMesaj.ENUMAlanlar.OKUNDU, 0);

                    liste.objeler.Add(m);
                }
            }
            if (dr != null) { dr.Close(); }
            ve.VeriErisimKapat();

            return (Mesaj[])liste.objeler.ToArray(typeof(Mesaj));
        }

        internal static string BuyukMetinOku(Kullanici kullanan, string modulKod, string anahtar, string dilKod)
        {
            ObjectArray oa = BuyukMetinListele(kullanan, modulKod, anahtar, dilKod, false);
            if (oa.sonuc.islemSonuc && oa.ObjeSayisi == 1)
            {
                StrKodStrAd s = (StrKodStrAd)oa.objeler[0];
                return s.ad;
            }
            return "";
            //VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriKYM", kullanan.KimlikBul());
            //TABMesaj to = new TABMesaj(ve);


            //IDataReader r = to.BuyukMetinOku(modulKod, anahtar, dilKod);
            //StringBuilder metin = new StringBuilder("");
            //if (r != null)
            //{
            //    while (r.Read())
            //    {
            //        metin.Append(r.GetString(1));
            //    }
            //    r.Close();
            //}
            //else
            //{
            //    metin.Append(to.Hata());
            //}
            //ve.VeriErisimKapat();
            //return metin.ToString();
        }

        internal static ObjectArray BuyukMetinListele(Kullanici kullanan, string modulKod, string anahtar, string dilKod, bool like)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriKYM", kullanan.KimlikBul());
            TABMesaj to = new TABMesaj(ve);
            ObjectArray oa = new ObjectArray();

            IDataReader r = to.BuyukMetinOku(modulKod, anahtar, dilKod, like);
            StringBuilder metin = new StringBuilder("");
            StrKodStrAd skSa = null;
            if (r != null)
            {
                while (r.Read())
                {
                    string anah = r.GetString(0);
                    if (skSa == null || skSa.kod != anah)
                    {
                        if(skSa != null)
                            skSa.ad = metin.ToString();

                        skSa = new StrKodStrAd(anah, "");
                        metin.Clear();
                        oa.Ekle(skSa);
                    }
                    metin.Append(r.GetString(2));
                }
                if (skSa != null)
                    skSa.ad = metin.ToString();
                r.Close();
            }
            else
            {
                oa.sonuc.DegerAta(to.Hata());
            }
            ve.VeriErisimKapat();
            return oa;
        }

        internal static Sonuc BuyukMetinSakla(VeriErisim veriErisim, Kullanici kullanan, string modulKod, string anahtar, string dilKod, string metin)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriKYM", kullanan.KimlikBul());
            TABMesaj to = new TABMesaj(ve);

            ve.BeginTransaction();
            Sonuc sonuc = to.BuyukMetinSil(modulKod, anahtar, dilKod);
            if (sonuc.islemSonuc)
            {
                int yazilan = 0;
                string parca = "";
                int satir = 0;
                int kalan = metin.Length;
                int parcaBoy;
                while (kalan > 0)
                {
                    if (kalan > 250)
                        parcaBoy = 250;
                    else
                        parcaBoy = kalan;

                    parca = metin.Substring(yazilan, parcaBoy);
                    yazilan = yazilan + parcaBoy;
                    kalan = kalan - parcaBoy;

                    sonuc = to.BuyukMetinSatirSakla(modulKod, anahtar, dilKod, ++satir, parca);
                    if (!sonuc.islemSonuc)
                        break;
                }
            }
            if (sonuc.islemSonuc)
                ve.Commit();
            else
                ve.Rollback();
            ve.VeriErisimKapat();
            return sonuc;
        }

        internal static Sonuc BuyukMetinListeSakla(Kullanici kullanan, string modulKod, string dilKod, TNSCollection liste)
        {
            Sonuc s = new Sonuc();
            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriKYM", kullanan.KimlikBul());
            TABMesaj to = new TABMesaj(ve);

            ve.BeginTransaction();
            foreach (StrKodStrAd item in liste)
            {
                s = BuyukMetinSakla(ve, kullanan, modulKod, item.kod, dilKod, item.ad);
                if (!s.islemSonuc) break;
            }
            if (s.islemSonuc)
                ve.Commit();
            else
                ve.Rollback();
            ve.VeriErisimKapat();
            return s;
        }

    }
}
