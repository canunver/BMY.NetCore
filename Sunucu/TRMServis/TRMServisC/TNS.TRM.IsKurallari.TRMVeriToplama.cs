using System.Data;
using TNS.KYM;
using TNS.TRM.IsKurallari.Veri;
using TNS.Veri;

namespace TNS.TRM.IsKurallari
{
    public class TRMVeriToplama
    {
        public Sonuc Kaydet(Kullanici kullanan, ObjectArray bilgi)
        {
            Sonuc sonuc = new Sonuc(false, "", "");

            string hata = "";

            VeriErisim ve = VeriErisim.VeriErisimBul(null, TABGenel.BaglantiSatiri);
            TABVeriToplama to = new TABVeriToplama(ve);
            ve.BeginTransaction();

            foreach (VeriToplama item in bilgi.objeler)
            {
                if (item.ilKod == 0)
                    hata += "İl bilgisi boş bırakılamaz" + "<br>";

                if (hata != "")
                {
                    sonuc.islemSonuc = false;
                    sonuc.hataStr = hata;
                    return sonuc;
                }

                if (item.kod == 0)
                    item.kod = to.EnBuyukKod();

                sonuc = to.Kaydet(item);
            }

            if (sonuc.islemSonuc)
            {
                ve.Commit();
                sonuc.bilgiStr = "Kaydetme işlemi başarıyla tamamlandı.";
            }
            else
            {
                sonuc.hataStr = "Kayıt sırasında bir hata oluştu" + "<br>" + sonuc.hataStr;
                ve.Rollback();
            }

            ve.VeriErisimKapat();

            return sonuc;
        }

        public Sonuc Sil(Kullanici kullanan, VeriToplama bilgi)
        {
            Sonuc sonuc = new Sonuc();

            //if (!TRMYetkiKontrol.YoneticiMi(kullanan))
            //{
            //    sonuc.hataStr = TNS.TRM.HarcamaSurecleriResI.TRM00013;
            //    sonuc.islemSonuc = false;
            //    return sonuc;
            //}

            if (bilgi.kod == 0)
                sonuc.hataStr = "Kod bilgisi boş bırakılamaz" + "<br>";

            if (sonuc.hataStr != "")
            {
                sonuc.islemSonuc = false;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, TABGenel.BaglantiSatiri);
            TABVeriToplama to = new TABVeriToplama(ve);

            ve.BeginTransaction();

            sonuc = to.Sil(bilgi);

            if (sonuc.islemSonuc)
                ve.Commit();
            else
            {
                ve.Rollback();
                sonuc.hataStr = "Bilgi silinirken bir hata oluştu" + "<br>" + sonuc.hataStr;
            }

            ve.VeriErisimKapat();

            return sonuc;
        }

        public ObjectArray Listele(Kullanici kullanan, VeriToplama kriter)
        {
            ObjectArray bilgi = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, TABGenel.BaglantiSatiri);

            TABVeriToplama to = new TABVeriToplama(ve);
            IDataReader r = to.Listele(kriter);

            if (to.Hata() == "")
            {
                while (r.Read())
                {
                    VeriToplama f = new VeriToplama();

                    f.kod = VeriErisim.DegerAl(r, 0, 0);
                    f.aciklama = VeriErisim.DegerAl(r, 1, "");
                    f.ilKod = VeriErisim.DegerAl(r, 2, 0);
                    f.yeniYatirim = VeriErisim.DegerAl(r, 3, 0);
                    f.modTekYen = VeriErisim.DegerAl(r, 4, 0);
                    f.tamamlama = VeriErisim.DegerAl(r, 5, 0);
                    f.faal = VeriErisim.DegerAl(r, 6, 0);
                    f.faalDegil = VeriErisim.DegerAl(r, 7, 0);
                    f.gercekKisiE = VeriErisim.DegerAl(r, 8, 0);
                    f.gercekKisiK = VeriErisim.DegerAl(r, 9, 0);
                    f.sirket = VeriErisim.DegerAl(r, 10, 0);
                    f.kooperatif = VeriErisim.DegerAl(r, 11, 0);
                    f.toplamYatirimTutari = VeriErisim.DegerAl(r, 12, 0.0);
                    f.toplamHibeTutari = VeriErisim.DegerAl(r, 13, 0.0);
                    f.islenenHammaddeMiktari = VeriErisim.DegerAl(r, 14, 0.0);
                    f.buyukBas = VeriErisim.DegerAl(r, 15, 0);
                    f.kucukBas = VeriErisim.DegerAl(r, 16, 0);
                    f.saglananIstihdam = VeriErisim.DegerAl(r, 17, 0);
                    f.ihracatYapanTesisSayisi = VeriErisim.DegerAl(r, 18, 0);
                    f.sonYilCiro = VeriErisim.DegerAl(r, 19, 0.0);

                    bilgi.objeler.Add(f);
                }
            }

            r.Close();

            ve.VeriErisimKapat();

            return bilgi;
        }

    }
}
