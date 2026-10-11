using System;
using System.Data;
using TNS.KYM;
using TNS.TRM.IsKurallari.Veri;
using TNS.Veri;

namespace TNS.TRM.IsKurallari
{
    public class TRMHibeSulamaBankaKredi
    {
        public static Sonuc Kaydet(VeriErisim ve, Kullanici kullanan, HibeSulamaBankaKredi bilgi)
        {
            Sonuc sonuc = new Sonuc(false, "", "");

            string hata = "";
            if (bilgi.etapNo == 0)
                hata += "<li>Etap No bilgisi boş bırakılamaz</li>";
            if (string.IsNullOrWhiteSpace(bilgi.ilKod))
                hata += "<li>İl bilgisi boş bırakılamaz</li>";
            if (bilgi.tarih.isNull)
                hata += "<li>Tarih bilgisi boş bırakılamaz</li>";
            if (string.IsNullOrWhiteSpace(bilgi.alimyeri))
                hata += "<li>Kredinin alındığı yer bilgisi boş bırakılamaz</li>";

            if (string.IsNullOrWhiteSpace(bilgi.kimlikNo))
                hata += "<li>Kimlik bilgisi boş bırakılamaz</li>";
            else if (!TNS.OrtakFonksiyonlar.TCKNDogrula(bilgi.kimlikNo))
                hata += "<li>Kimlik bilgisi hatalı girilmiş</li>";

            if (string.IsNullOrWhiteSpace(bilgi.adi))
                hata += "<li>Yatırımcı Adı bilgisi boş bırakılamaz</li>";
            if (string.IsNullOrWhiteSpace(bilgi.soyadi))
                hata += "<li>Yatırımcı Soyadı bilgisi boş bırakılamaz</li>";
            if (string.IsNullOrWhiteSpace(bilgi.parselNo))
                hata += "<li>Parsel bilgisi boş bırakılamaz</li>";



            if (!string.IsNullOrEmpty(hata))
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = hata;
                return sonuc;
            }


            ve = VeriErisim.VeriErisimBul(null, TABGenel.BaglantiSatiri);
            TABHibeSulamaBankaKredi to = new TABHibeSulamaBankaKredi(ve);
            ve.BeginTransaction();

            bilgi.kayitEden = kullanan.kullaniciKodu;

            if (string.IsNullOrWhiteSpace(bilgi.kod))
            {
                bilgi.kod = Guid.NewGuid().ToString().Replace("-", "").ToLower();
                sonuc = to.Kaydet(bilgi);
            }
            else
            {
                sonuc = to.Guncelle(bilgi);
            }

            if (sonuc.islemSonuc)
                ve.Commit();
            else
            {
                sonuc.hataStr = "Kayıt sırasında bir hata oluştu" + "<br>" + sonuc.hataStr;
                ve.Rollback();
            }

            ve.VeriErisimKapat();

            return sonuc;
        }

        public static ObjectArray Listele(Kullanici kullanan, HibeSulamaBankaKredi kriter)
        {
            ObjectArray bilgi = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, TABGenel.BaglantiSatiri);
            TABHibeSulamaBankaKredi to = new TABHibeSulamaBankaKredi(ve);

            IDataReader r = to.Listele(kriter);

            if (to.Hata() == "")
            {
                while (r.Read())
                {
                    HibeSulamaBankaKredi f = new HibeSulamaBankaKredi();

                    f.kod = VeriErisim.DegerAl(r, 0, "");
                    f.etapNo = VeriErisim.DegerAl(r, 1, 0);
                    f.ilKod = VeriErisim.DegerAl(r, 2, "");
                    f.tarih = VeriErisim.DegerAl(r, 3);
                    f.alimyeri = VeriErisim.DegerAl(r, 4, "");
                    f.kimlikNo = VeriErisim.DegerAl(r, 5, "");
                    f.adi = VeriErisim.DegerAl(r, 6, "");
                    f.soyadi = VeriErisim.DegerAl(r, 7, "");
                    f.parselNo = VeriErisim.DegerAl(r, 8, "");
                    f.kayitEden = VeriErisim.DegerAl(r, 9, "");

                    f.ilAdi = VeriErisim.DegerAl(r, 10, "");

                    bilgi.objeler.Add(f);
                }
            }

            r.Close();

            ve.VeriErisimKapat();

            return bilgi;
        }

        public static Sonuc Sil(VeriErisim ve, Kullanici kullanan, HibeSulamaBankaKredi bilgi)
        {
            Sonuc sonuc = new Sonuc(false, "", "");

            string hata = "";
            if (string.IsNullOrWhiteSpace(bilgi.kod))
                hata += "Kod bilgisi boş bırakılamaz";

            if (!string.IsNullOrEmpty(hata))
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = hata;
                return sonuc;
            }

            ve = VeriErisim.VeriErisimBul(null, TABGenel.BaglantiSatiri);
            TABHibeSulamaBankaKredi to = new TABHibeSulamaBankaKredi(ve);
            ve.BeginTransaction();

            sonuc = to.Sil(bilgi.kod);

            if (sonuc.islemSonuc)
                ve.Commit();
            else
            {
                sonuc.hataStr = "Silme sırasında bir hata oluştu" + "<br>" + sonuc.hataStr;
                ve.Rollback();
            }

            ve.VeriErisimKapat();

            return sonuc;
        }
    }
}
