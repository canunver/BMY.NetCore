using System.Data;
using TNS.KYM;
using TNS.TMM.IsKurallari.Veri;
using TNS.Veri;

namespace TNS.TMM.IsKurallari
{
    public class TMMTasinirIslemOnayYetki
    {
        public static ObjectArray Listele(Kullanici kullanan, TasinirIslemOnayYetki form)
        {
            ObjectArray iVAN = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));

            TABTasinirIslemOnayYetki to = new TABTasinirIslemOnayYetki(ve);
            IDataReader r = to.Listele(form);

            if (to.Hata() == "")
            {
                int sonKolon = (int)TABTasinirIslemOnayYetki.ENUMAlanlar.ONAYTUR;
                while (r.Read())
                {
                    TasinirIslemOnayYetki yetki = new TasinirIslemOnayYetki();

                    yetki.muhasebeKod = VeriErisim.DegerAl(r, (int)TABTasinirIslemOnayYetki.ENUMAlanlar.MUHASEBEKOD, "");
                    yetki.harcamaBirimKod = VeriErisim.DegerAl(r, (int)TABTasinirIslemOnayYetki.ENUMAlanlar.HARCAMABIRIMKOD, "");
                    yetki.mernis = VeriErisim.DegerAl(r, (int)TABTasinirIslemOnayYetki.ENUMAlanlar.MERNIS, "");
                    yetki.adSoyad = VeriErisim.DegerAl(r, (int)TABTasinirIslemOnayYetki.ENUMAlanlar.ADSOYAD, "");
                    yetki.onayTur = VeriErisim.DegerAl(r, (int)TABTasinirIslemOnayYetki.ENUMAlanlar.ONAYTUR, 0);
                    yetki.eposta = VeriErisim.DegerAl(r, sonKolon + 1, "");

                    iVAN.objeler.Add(yetki);
                }
            }

            if (r != null)
                r.Close();

            ve.VeriErisimKapat();

            return iVAN;
        }

        public static Sonuc Kaydet(Kullanici kullanan, string muhasebeKod, string harcamaBirimKod, TasinirIslemOnayYetki[] liste)
        {
            Sonuc sonuc = new Sonuc(false, "Kayıt işlemi sırasında bilinmeyen bir hata oluştu.", "");

            string hata = "";
            if (string.IsNullOrWhiteSpace(muhasebeKod))
                hata += "Muhasebe bilgisi boş bırakılamaz<br />";
            if (string.IsNullOrWhiteSpace(harcamaBirimKod))
                hata += "Harcama Birim bilgisi boş bırakılamaz<br />";

            if (hata != "")
            {
                sonuc.DegerAta(hata);
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABTasinirIslemOnayYetki to = new TABTasinirIslemOnayYetki(ve);

            ve.BeginTransaction();

            sonuc = to.Sil(new TasinirIslemOnayYetki() { muhasebeKod = muhasebeKod, harcamaBirimKod = harcamaBirimKod });
            if (sonuc.islemSonuc)
            {
                if (liste != null)
                    foreach (TasinirIslemOnayYetki yetki in liste)
                    {
                        if (yetki.mernis != "" && yetki.onayTur > 0)
                        {
                            sonuc = to.Kaydet(yetki);
                            if (!sonuc.islemSonuc)
                                break;
                        }
                    }
            }

            if (!sonuc.islemSonuc)
                ve.Rollback();
            else
            {
                sonuc.bilgiStr = "İşlem başarıyla tamamlandı.";
                ve.Commit();
            }

            ve.VeriErisimKapat();
            return sonuc;
        }
    }
}
