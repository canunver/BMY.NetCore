using System.Data;
using TNS.KYM;
using TNS.MUH.IsKurallari.Veri;
using TNS.Veri;

namespace TNS.MUH.IsKurallari
{
    /// <summary>
    /// Nakit talebi işlemlerinin yapıldığı sınıf
    /// </summary>
    public class MUHNakitTalebi
    {
        /// <summary>
        /// Nakit talebi kaydet
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="bilgi">İşlem yapılacak bilgileri içeren nesne</param>
        /// <param name="kontrolSekli">Kontrol şekli</param>
        /// <returns></returns>
        public Sonuc Kaydet(Kullanici kullanan, ObjectArray bilgi, string kontrolSekli)
        {
            Sonuc sonuc = new Sonuc(false, ButceMuhasebeResI.MUH001, "");

            sonuc = KontrolEt(kullanan, (NakitTalebi)bilgi.objeler[0], kontrolSekli);

            if (!sonuc.islemSonuc)
                return sonuc;

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(kullanan));

            ve.BeginTransaction();
            TABNakitTalebi to = new TABNakitTalebi(ve);

            bool ilk = true;
            sonuc.islemSonuc = true;//bütün satırlarda tutar 0 ise işlemi başarıyla tamamlamak için
            foreach (NakitTalebi talep in bilgi.objeler)
            {
                if (ilk)
                {
                    sonuc = to.Sil(talep);
                    ilk = false;
                }

                if (talep.tutar > 0)
                    sonuc = to.Kaydet(kullanan.kullaniciKodu, talep);

                if (!sonuc.islemSonuc)
                    break;
            }

            if (sonuc.islemSonuc)
            {
                ve.Commit();
                sonuc.bilgiStr = ButceMuhasebeResI.MUH135;
            }
            else
                ve.Rollback();

            ve.VeriErisimKapat();

            return sonuc;
        }

        Sonuc KontrolEt(Kullanici kullanan, NakitTalebi form, string kontrolSekli)
        {
            string hata = "";
            Sonuc sonuc = new Sonuc(false, ButceMuhasebeResI.MUH071, "");

            //Yetki Kontrolü

            if (!kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.SISTEMYONETICISI))
            {
                int kontrolBelgeTur = (int)ENUMBelgeTuru.ODENEKGONDERME;
                bool kontrol = false;

                if (kontrolSekli == "MerkezTasra")
                    kontrol = MUH.IsKurallari.MUHYetkiServis.YazmaYetkisiVarMi(kullanan, kontrolBelgeTur, form.birim);
                else
                    kontrol = TNS.MUH.Yetki.MuhasebeBirimYetkiVarMi(kullanan, form.teskilat.ToString(), form.birim);


                if (!kontrol)
                    hata += form.birim + " " + ButceMuhasebeResI.MUH136;
            }

            if (form.yil == 0)
                hata += ButceMuhasebeResI.MUH072 + "<br>";
            if (form.ay == 0)
                hata += ButceMuhasebeResI.MUH137 + "<br>";
            if (form.teskilat == 0)
                hata += ButceMuhasebeResI.MUH138 + "<br>";
            if (form.birim == "")
                hata += ButceMuhasebeResI.MUH139 + "<br>";

            if (hata != "")
            {
                sonuc.hataStr = hata;
                sonuc.islemSonuc = false;
                return sonuc;
            }
            else
            {
                sonuc.islemSonuc = true;
                return sonuc;
            }
        }
        /// <summary>
        /// Nakit talebi listele
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="kriter">İşlem yapılacak bilgileri içeren nesne</param>
        /// <param name="kontrolSekli">Kontrol şekli</param>
        /// <returns></returns>
        public ObjectArray Listele(Kullanici kullanan, NakitTalebi kriter, string kontrolSekli)
        {
            ObjectArray bilgi = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABNakitTalebi to = new TABNakitTalebi(ve);

            IDataReader r = to.Listele(kriter);

            if (to.Hata() == "")
            {
                while (r.Read())
                {
                    NakitTalebi talep = new NakitTalebi();

                    talep.yil = VeriErisim.DegerAl(r, 0, 0);
                    talep.ay = VeriErisim.DegerAl(r, 1, 0);
                    //talep.muhasebe = VeriErisim.DegerAl(r, 2, 0);
                    //talep.birim = VeriErisim.DegerAl(r, 3, "");
                    talep.asama = VeriErisim.DegerAl(r, 2, 0);
                    talep.hafta = VeriErisim.DegerAl(r, 3, 0);
                    talep.ekoKod = VeriErisim.DegerAl(r, 4, "");
                    talep.tutar = VeriErisim.DegerAl(r, 5, 0.0);
                    //talep.kullaniciKodu = VeriErisim.DegerAl(r, 6, "");

                    bilgi.objeler.Add(talep);
                }
                r.Close();
            }
            ve.VeriErisimKapat();
            return bilgi;
        }
    }
}
