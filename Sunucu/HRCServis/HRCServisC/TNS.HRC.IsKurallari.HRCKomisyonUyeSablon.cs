using System;
using System.Data;
using TNS.HRC;
using TNS.HRC.IsKurallari.Veri;
using TNS.KYM;
using TNS.Veri;

namespace TNS.HRC.IsKurallari
{
    public class HRCKomisyonUyeSablon
    {
        public static Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, KomisyonUyeSablon sablon)
        {
            Sonuc sonuc = new Sonuc();

            if (string.IsNullOrWhiteSpace(sablon.adi))
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = "Şablon ad bilgisi boş bırakılamaz";
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));
            TABKomisyonUyeSablon tab = new TABKomisyonUyeSablon(ve);

            ve.BeginTransaction();

            sablon.sahibi = kullanan.kullaniciKodu;

            if (sablon.kod == "")
            {
                sablon.kod = Guid.NewGuid().ToString().ToLower();
                sonuc = tab.Kaydet(sablon);
            }
            else
                sonuc = tab.Guncelle(sablon);

            if (sonuc.islemSonuc)
            {
                sonuc = tab.SilDetay(sablon);
                foreach (KomisyonUyeSablonDetay ud in sablon.uyeler.objeler)
                {
                    ud.sablonKod = sablon.kod;
                    ud.kod = Guid.NewGuid().ToString().ToLower();
                    sonuc = tab.KaydetDetay(ud);
                }

                sonuc.bilgiStr = TNS.HRC.HarcamaSurecleriResI.HRC00010;
                ve.Commit();
            }
            else
            {
                sonuc.hataStr = TNS.HRC.HarcamaSurecleriResI.HRC00011 + "<br>" + sonuc.hataStr;
                ve.Rollback();
            }

            ve.VeriErisimKapat();

            return sonuc;
        }

        public static Sonuc Sil(VeriErisim veriErisim, Kullanici kullanan, KomisyonUyeSablon sablon)
        {
            Sonuc sonuc = new Sonuc();

            if (string.IsNullOrWhiteSpace(sablon.kod))
                sonuc.hataStr += "Kod bilgisi boş olamaz" + "<br>";

            if (sonuc.hataStr != string.Empty)
            {
                sonuc.islemSonuc = false;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));
            TABKomisyonUyeSablon tab = new TABKomisyonUyeSablon(ve);
            ve.BeginTransaction();

            sablon.sahibi = kullanan.kullaniciKodu;
            sonuc = tab.SilDetay(sablon);
            sonuc = tab.Sil(sablon);

            if (sonuc.islemSonuc)
            {
                ve.Commit();
            }
            else
            {
                ve.Rollback();
                sonuc.hataStr = TNS.HRC.HarcamaSurecleriResI.HRC00019 + "<br>" + sonuc.hataStr;
            }

            ve.VeriErisimKapat();

            return sonuc;
        }


        public ObjectArray Listele(Kullanici kullanan, KomisyonUyeSablon sablon)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));
            TABKomisyonUyeSablon tab = new TABKomisyonUyeSablon(ve);

            ObjectArray bilgi = new ObjectArray();
            IDataReader dr = tab.Listele(sablon);

            while (dr.Read())
            {
                KomisyonUyeSablon sbGelen = new KomisyonUyeSablon();

                sbGelen.kod = VeriErisim.DegerAl(dr, 0, "");
                sbGelen.sahibi = VeriErisim.DegerAl(dr, 1, "");
                sbGelen.adi = VeriErisim.DegerAl(dr, 2, "");
                sbGelen.durum = VeriErisim.DegerAl(dr, 3, 0);

                bilgi.Ekle(sbGelen);
            }
            dr.Close();

            if (bilgi.objeler.Count == 1)
            {

                foreach (KomisyonUyeSablon sbl in bilgi.objeler)
                {
                    dr = tab.ListeleDetay(sbl.kod);

                    while (dr.Read())
                    {
                        KomisyonUyeSablonDetay sbGelen = new KomisyonUyeSablonDetay();
                        sbGelen.kod = VeriErisim.DegerAl(dr, 0, "");
                        sbGelen.sablonKod = VeriErisim.DegerAl(dr, 1, "");
                        sbGelen.siraNo = VeriErisim.DegerAl(dr, 2, 0);
                        sbGelen.komisyonGorev = VeriErisim.DegerAl(dr, 3, 0);
                        sbGelen.adi = VeriErisim.DegerAl(dr, 4, "");
                        sbGelen.gorev = VeriErisim.DegerAl(dr, 5, "");
                        sbGelen.unvan = VeriErisim.DegerAl(dr, 6, "");
                        sbGelen.sicilNo = VeriErisim.DegerAl(dr, 7, "");
                        sbGelen.telefon = VeriErisim.DegerAl(dr, 8, "");
                        sbGelen.cepTelefon = VeriErisim.DegerAl(dr, 9, "");

                        sbl.uyeler.Ekle(sbGelen);
                    }
                    dr.Close();
                    break;
                }
            }

            ve.VeriErisimKapat();

            return bilgi;
        }
    }
}
