using System;
using System.Data;
using TNS.PER;
using TNS.PER.IsKurallari.Veri;
using TNS.KYM;
using TNS.Veri;
using TNS.UZY;

namespace TNS.PER.IsKurallari
{
    class PERGorevYerTarih
    {
        public ObjectArray Listele(GorevYerTarih p)
        {
            ObjectArray liste = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriPER", TNS.KYM.Arac.KimlikBul(null));

            TABGorevYerTarih tab = new TABGorevYerTarih(ve);

            IDataReader r = null;

            try
            {
                r = tab.Listele(p);
            }
            catch (Exception e)
            {
                ve.VeriErisimKapat();
                liste.sonuc.islemSonuc = false;
                liste.sonuc.hataStr = e.Message;

                return liste;
            }

            while (r.Read())
            {
                GorevYerTarih yerTarih = new GorevYerTarih();

                yerTarih.gorevYil = VeriErisim.DegerAl(r, (int)TABGorevYerTarih.ENUMAlanlar.GOREVYIL, 0);
                yerTarih.gorevKod = VeriErisim.DegerAl(r, (int)TABGorevYerTarih.ENUMAlanlar.GOREVKOD, 0);
                yerTarih.siraNo = VeriErisim.DegerAl(r, (int)TABGorevYerTarih.ENUMAlanlar.SIRANO, 0);
                yerTarih.bulunduguIlKod = VeriErisim.DegerAl(r, (int)TABGorevYerTarih.ENUMAlanlar.BULUNDUGUILKOD, string.Empty);
                yerTarih.bulunduguIlceKod = VeriErisim.DegerAl(r, (int)TABGorevYerTarih.ENUMAlanlar.BULUNDUGUILCEKOD, string.Empty);
                yerTarih.gidecegiIlKod = VeriErisim.DegerAl(r, (int)TABGorevYerTarih.ENUMAlanlar.GIDECEGIILKOD, string.Empty);
                yerTarih.gidecegiIlceKod = VeriErisim.DegerAl(r, (int)TABGorevYerTarih.ENUMAlanlar.GIDECEGIILCEKOD, string.Empty);
                yerTarih.baslangicTarihi = VeriErisim.DegerAl(r, (int)TABGorevYerTarih.ENUMAlanlar.BASLANGICTARIH);
                yerTarih.bitisTarihi = VeriErisim.DegerAl(r, (int)TABGorevYerTarih.ENUMAlanlar.BITISTARIH);

                liste.objeler.Add(yerTarih);
            }
            r.Close();

            liste.sonuc.islemSonuc = true;

            if (liste.objeler.Count == 0)
                liste.sonuc.bilgiStr = "Verilen kriterlere göre listelenecek bilgi <b>bulunamadı</b>.";

            ve.VeriErisimKapat();

            return liste;
        }

        public Sonuc Kaydet(Kullanici kullanan, ObjectArray liste, int gorevYil, int gorevKod)
        {
            Sonuc sonuc = new Sonuc();

            string hata = string.Empty;

            for (int i = 0; i < liste.ObjeSayisi; i++)
            {
                hata = KontrolEt((GorevYerTarih)liste.objeler[i], gorevKod);
                if (!string.IsNullOrEmpty(hata))
                {
                    sonuc.islemSonuc = false;
                    sonuc.hataStr = hata;
                    return sonuc;
                }
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriPER", TNS.KYM.Arac.KimlikBul(kullanan));
            ve.BeginTransaction();
            TABGorevYerTarih tab = new TABGorevYerTarih(ve);

            sonuc = tab.Sil(gorevYil, gorevKod);
            if (sonuc.islemSonuc)
                sonuc = tab.Kaydet(liste, gorevYil, gorevKod);

            if (sonuc.islemSonuc)
            {
                ve.Commit();
                sonuc.bilgiStr = PersonelResI.PER007;
            }
            else
                ve.Rollback();

            ve.VeriErisimKapat();

            return sonuc;
        }

        public Sonuc Sil(Kullanici kullanan, int gorevYil, int gorevKod)
        {
            Sonuc sonuc = new Sonuc();

            if (gorevKod == 0)
            {
                sonuc.hataStr = "Silmek istenilen <b>Bilginin kodu</b> belirtilmemiş.";
                sonuc.islemSonuc = false;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriPER", TNS.KYM.Arac.KimlikBul(kullanan));

            ve.BeginTransaction();

            TABGorevYerTarih tab = new TABGorevYerTarih(ve);

            sonuc = tab.Sil(gorevYil, gorevKod);

            if (sonuc.islemSonuc)
            {
                ve.Commit();
                sonuc.bilgiStr = "Silme işlemi başarıyla gerçekleştirildi.";
            }
            else
                ve.Rollback();

            ve.VeriErisimKapat();

            return sonuc;
        }

        private string KontrolEt(GorevYerTarih yerTarih, int gorevKod)
        {
            string hata = string.Empty;
            if (gorevKod == 0)
                hata += "Görev No bilgisi boş bırakılamaz.<br>";
            if (yerTarih.bulunduguIlKod == string.Empty || yerTarih.bulunduguIlceKod == string.Empty)
                hata += "Bulunduğu Yer alanı boş bırakılamaz.<br>";
            if (yerTarih.gidecegiIlKod == string.Empty || yerTarih.gidecegiIlceKod == string.Empty)
                hata += "Gideceği Yer alanı boş bırakılamaz.<br>";
            if (yerTarih.baslangicTarihi.isNull)
                hata += "Başlangıç Tarihi boş bırakılamaz.<br>";
            if (yerTarih.bitisTarihi.isNull)
                hata += "Bitiş Tarihi boş bırakılamaz.<br>";
            if (!yerTarih.baslangicTarihi.isNull && !yerTarih.bitisTarihi.isNull)
            {
                if (yerTarih.baslangicTarihi.Yil > yerTarih.bitisTarihi.Yil)
                {
                    hata += "Bitiş Tarihi, Başlangıç Tarihinden önce olamaz.<br>";
                }
                else if (yerTarih.baslangicTarihi.Yil == yerTarih.bitisTarihi.Yil)
                {
                    if (yerTarih.baslangicTarihi.YilinGunu > yerTarih.bitisTarihi.YilinGunu)
                        hata += "Bitiş Tarihi, Başlangıç Tarihinden önce olamaz.<br>";
                }
            }
            return hata;
        }
    }
}
