using System;
using System.Data;
using TNS.PER;
using TNS.PER.IsKurallari.Veri;
using TNS.KYM;
using TNS.Veri;
using TNS.UZY;

namespace TNS.PER.IsKurallari
{
    class PERGorev
    {
        private enum ENUMKontrolDurum
        {
            KAYDET = 1,
            SIL = 2,
            DURUMDEGISTIR = 3,

        }

        public ObjectArray Sorgula(Kullanici kullanan, Gorev p)
        {
            ObjectArray liste = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriPER", TNS.KYM.Arac.KimlikBul(kullanan));

            TABGorev tab = new TABGorev(ve);
            KullanicininYetkiliOlduguMuhasebeBirim(kullanan, ref p.muhasebeKod, ref p.birimKod);
            IDataReader r = null;

            try
            {
                r = tab.Sorgula(p);
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
                Gorev gorev = new Gorev();

                gorev.yil = VeriErisim.DegerAl(r, (int)TABGorev.ENUMAlanlar.YIL, 0);
                gorev.kod = VeriErisim.DegerAl(r, (int)TABGorev.ENUMAlanlar.KOD, 0);
                gorev.muhasebeKod = VeriErisim.DegerAl(r, (int)TABGorev.ENUMAlanlar.MUHASEBEKOD, 0);
                gorev.birimKod = VeriErisim.DegerAl(r, (int)TABGorev.ENUMAlanlar.BIRIMKOD, string.Empty);
                gorev.amac = VeriErisim.DegerAl(r, (int)TABGorev.ENUMAlanlar.AMAC, string.Empty);
                gorev.tur = VeriErisim.DegerAl(r, (int)TABGorev.ENUMAlanlar.TUR, 0);
                gorev.durum = VeriErisim.DegerAl(r, (int)TABGorev.ENUMAlanlar.DURUM, 0);
                gorev.islemKod = VeriErisim.DegerAl(r, (int)TABGorev.ENUMAlanlar.ISLEMKOD, 0);
                gorev.evrEvrakNo = VeriErisim.DegerAl(r, (int)TABGorev.ENUMAlanlar.EVREVRAKNO, string.Empty);
                gorev.prEvrakNo = VeriErisim.DegerAl(r, (int)TABGorev.ENUMAlanlar.PREVRAKNO, string.Empty);
                gorev.orgKod = VeriErisim.DegerAl(r, (int)TABGorev.ENUMAlanlar.ORGKOD, 0);
                gorev.kurum = VeriErisim.DegerAl(r, (int)TABGorev.ENUMAlanlar.KURUM, string.Empty);
                gorev.vasita = VeriErisim.DegerAl(r, (int)TABGorev.ENUMAlanlar.VASITA, string.Empty);
                gorev.avans = VeriErisim.DegerAl(r, (int)TABGorev.ENUMAlanlar.AVANS, string.Empty);
                gorev.alan1 = VeriErisim.DegerAl(r, (int)TABGorev.ENUMAlanlar.ALAN1, string.Empty);
                gorev.imza1 = VeriErisim.DegerAl(r, (int)TABGorev.ENUMAlanlar.IMZA1, string.Empty);
                gorev.imza2 = VeriErisim.DegerAl(r, (int)TABGorev.ENUMAlanlar.IMZA2, string.Empty);
                gorev.imza3 = VeriErisim.DegerAl(r, (int)TABGorev.ENUMAlanlar.IMZA3, string.Empty);

                liste.objeler.Add(gorev);
            }
            r.Close();

            liste.sonuc.islemSonuc = true;

            if (liste.ObjeSayisi == 0)
                liste.sonuc.bilgiStr = "Verilen kriterlere göre listelenecek bilgi <b>bulunamadı</b>.";

            ve.VeriErisimKapat();

            return liste;
        }

        public ObjectArray Listele(Kullanici kullanan, Gorev p)
        {
            ObjectArray liste = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriPER", TNS.KYM.Arac.KimlikBul(kullanan));

            TABGorev tab = new TABGorev(ve);
            KullanicininYetkiliOlduguMuhasebeBirim(kullanan, ref p.muhasebeKod, ref p.birimKod);
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
                Gorev gorev = new Gorev();

                gorev.yil = VeriErisim.DegerAl(r, (int)TABGorev.ENUMAlanlar.YIL, 0);
                gorev.kod = VeriErisim.DegerAl(r, (int)TABGorev.ENUMAlanlar.KOD, 0);
                gorev.muhasebeKod = VeriErisim.DegerAl(r, (int)TABGorev.ENUMAlanlar.MUHASEBEKOD, 0);
                gorev.birimKod = VeriErisim.DegerAl(r, (int)TABGorev.ENUMAlanlar.BIRIMKOD, string.Empty);
                gorev.amac = VeriErisim.DegerAl(r, (int)TABGorev.ENUMAlanlar.AMAC, string.Empty);
                gorev.tur = VeriErisim.DegerAl(r, (int)TABGorev.ENUMAlanlar.TUR, 0);
                gorev.durum = VeriErisim.DegerAl(r, (int)TABGorev.ENUMAlanlar.DURUM, 0);
                gorev.islemKod = VeriErisim.DegerAl(r, (int)TABGorev.ENUMAlanlar.ISLEMKOD, 0);
                gorev.evrEvrakNo = VeriErisim.DegerAl(r, (int)TABGorev.ENUMAlanlar.EVREVRAKNO, string.Empty);
                gorev.prEvrakNo = VeriErisim.DegerAl(r, (int)TABGorev.ENUMAlanlar.PREVRAKNO, string.Empty);
                gorev.orgKod = VeriErisim.DegerAl(r, (int)TABGorev.ENUMAlanlar.ORGKOD, 0);
                gorev.kurum = VeriErisim.DegerAl(r, (int)TABGorev.ENUMAlanlar.KURUM, string.Empty);
                gorev.vasita = VeriErisim.DegerAl(r, (int)TABGorev.ENUMAlanlar.VASITA, string.Empty);
                gorev.avans = VeriErisim.DegerAl(r, (int)TABGorev.ENUMAlanlar.AVANS, string.Empty);
                gorev.alan1 = VeriErisim.DegerAl(r, (int)TABGorev.ENUMAlanlar.ALAN1, string.Empty);
                gorev.imza1 = VeriErisim.DegerAl(r, (int)TABGorev.ENUMAlanlar.IMZA1, string.Empty);
                gorev.imza2 = VeriErisim.DegerAl(r, (int)TABGorev.ENUMAlanlar.IMZA2, string.Empty);
                gorev.imza3 = VeriErisim.DegerAl(r, (int)TABGorev.ENUMAlanlar.IMZA3, string.Empty);

                gorev.gorevYerleri = ListeleGorevYerleri(gorev);
                gorev.gorevliPersoneller = ListeleGorevliPersoneller(gorev);

                liste.objeler.Add(gorev);
            }
            r.Close();

            liste.sonuc.islemSonuc = true;

            if (liste.ObjeSayisi == 0)
                liste.sonuc.bilgiStr = "Verilen kriterlere göre listelenecek bilgi <b>bulunamadı</b>.";

            ve.VeriErisimKapat();

            return liste;
        }

        private ObjectArray ListeleGorevYerleri(Gorev gorev)
        {
            GorevYerTarih yerTarih = new GorevYerTarih();
            yerTarih.gorevYil = gorev.yil;
            yerTarih.gorevKod = gorev.kod;
            return new PERGorevYerTarih().Listele(yerTarih);
        }

        private ObjectArray ListeleGorevliPersoneller(Gorev gorev)
        {
            GorevPersonel gorevPersonel = new GorevPersonel();
            gorevPersonel.gorevYil = gorev.yil;
            gorevPersonel.gorevKod = gorev.kod;
            return new PERGorevPersonel().Listele(gorevPersonel);
        }

        public Sonuc Kaydet(Kullanici kullanan, Gorev p)
        {
            Sonuc sonuc = new Sonuc();
            string hata = string.Empty;

            hata = KontrolEt(p, ENUMKontrolDurum.KAYDET);

            if (!string.IsNullOrEmpty(hata))
            {
                sonuc.hataStr = hata;
                sonuc.islemSonuc = false;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriPER", TNS.KYM.Arac.KimlikBul(kullanan));

            //Belgeye numara alınıyor
            if (p.kod == 0)
            {
                p.kod = TABGenel.SayacDegerAl(ve, "PERGOREV", "KOD", "");
            }

            ve.BeginTransaction();

            TABGorev tab = new TABGorev(ve);
            sonuc = tab.Kaydet(p);

            if (sonuc.islemSonuc)
            {
                sonuc = new PERGorevYerTarih().Kaydet(kullanan, p.gorevYerleri, p.yil, p.kod);
                if (sonuc.islemSonuc)
                {
                    sonuc = new PERGorevPersonel().Kaydet(kullanan, p.gorevliPersoneller, p.yil, p.kod);
                    if (sonuc.islemSonuc)
                    {
                        ve.Commit();
                        sonuc.anahtar = p.kod.ToString();
                        sonuc.bilgiStr = PersonelResI.PER007;
                    }
                    else
                        ve.Rollback();
                }
                else
                    ve.Rollback();
            }
            else
                ve.Rollback();

            ve.VeriErisimKapat();

            return sonuc;
        }

        public Sonuc KaydetEvrakBilgi(Kullanici kullanan, int yil, int kod, string evrEvrakNo, string prEvrakNo)
        {
            Sonuc sonuc = new Sonuc();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriPER", TNS.KYM.Arac.KimlikBul(kullanan));

            ve.BeginTransaction();

            TABGorev tab = new TABGorev(ve);
            sonuc = tab.KaydetEvrakBilgi(yil, kod, evrEvrakNo, prEvrakNo);

            if (sonuc.islemSonuc)
            {
                sonuc = DurumDegistir(ve, kullanan, new Gorev { yil = yil, kod = kod, durum = (int)ENUMGOREVDURUM.EVRAKNOALDI });

                if (sonuc.islemSonuc)
                {
                    ve.Commit();
                    sonuc.bilgiStr = "Evrak Bilgileri başarıyla kaydedildi.";
                }
                else
                    ve.Rollback();
            }
            else
                ve.Rollback();

            ve.VeriErisimKapat();

            return sonuc;
        }

        public Sonuc KaydetAlanBilgi(Kullanici kullanan, int yil, int kod, string alan1, string imza1, string imza2, string imza3)
        {
            Sonuc sonuc = new Sonuc();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriPER", TNS.KYM.Arac.KimlikBul(kullanan));

            ve.BeginTransaction();

            TABGorev tab = new TABGorev(ve);
            sonuc = tab.KaydetAlanBilgi(yil, kod, alan1, imza1, imza2, imza3);


            if (sonuc.islemSonuc)
            {
                ve.Commit();
                sonuc.bilgiStr = "Evrak Alan Bilgileri başarıyla kaydedildi.";
            }
            else
                ve.Rollback();

            ve.VeriErisimKapat();

            return sonuc;
        }

        public Sonuc DurumDegistir(VeriErisim veriErisim, Kullanici kullanan, Gorev p)
        {
            Sonuc sonuc = new Sonuc();
            string hata = string.Empty;

            hata = KontrolEt(p, ENUMKontrolDurum.DURUMDEGISTIR);

            if (!string.IsNullOrEmpty(hata))
            {
                sonuc.hataStr = hata;
                sonuc.islemSonuc = false;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriPER", TNS.KYM.Arac.KimlikBul(kullanan));

            ve.BeginTransaction();

            TABGorev tab = new TABGorev(ve);

            sonuc = tab.DurumDegistir(p.yil, p.kod, p.durum);

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

        public Sonuc DurumDegistir(VeriErisim veriErisim, Kullanici kullanan, string prEvrakNo, string evrEvrakNo, int durum = (int)ENUMGOREVDURUM.TANIMSIZ)
        {
            Sonuc sonuc = new Sonuc();
            string hata = string.Empty;

            TNS.PER.Gorev p = new TNS.PER.Gorev();
            ObjectArray liste = Listele(kullanan, new TNS.PER.Gorev { prEvrakNo = prEvrakNo, evrEvrakNo = evrEvrakNo });
            if (liste.ObjeSayisi > 0)
                p = (TNS.PER.Gorev)liste.objeler[0];
            else
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = "Görevlendirme sistemde bulunamadı.";
                return sonuc;
            }

            hata = KontrolEt(p, ENUMKontrolDurum.DURUMDEGISTIR);

            if (!string.IsNullOrEmpty(hata))
            {
                sonuc.hataStr = hata;
                sonuc.islemSonuc = false;
                return sonuc;
            }

            if (durum != (int)ENUMGOREVDURUM.TANIMSIZ)
                p.durum = durum;

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriPER", TNS.KYM.Arac.KimlikBul(kullanan));

            ve.BeginTransaction();

            TABGorev tab = new TABGorev(ve);

            sonuc = tab.DurumDegistir(p.yil, p.kod, p.durum);

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

        public Sonuc Sil(Kullanici kullanan, Gorev p)
        {
            Sonuc sonuc = new Sonuc();

            if (p.kod == 0)
            {
                sonuc.hataStr = "Silmek istenilen <b>Bilginin kodu</b> belirtilmemiş.";
                sonuc.islemSonuc = false;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriPER", TNS.KYM.Arac.KimlikBul(kullanan));

            ve.BeginTransaction();

            TABGorev tab = new TABGorev(ve);

            sonuc = tab.Sil(p);

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

        private string KontrolEt(Gorev p, ENUMKontrolDurum durum)
        {
            string hata = string.Empty;
            if (durum == ENUMKontrolDurum.KAYDET)
            {
                if (p.orgKod == 0)
                {
                    if (p.muhasebeKod == 0)
                        hata += "Görev Muhasebe kodu boş bırakılamaz.<br>";
                    if (p.birimKod == "")
                        hata += "Görev Birim Kodu boş bırakılamaz.<br>";
                }
                if (p.tur == 0)
                    hata += "Görev Türü boş bırakılamaz.<br>";
                if (p.gorevYerleri == null || p.gorevYerleri.ObjeSayisi == 0)
                    hata += "Görev Yer ve Tarih Bilgisi boş bırakılamaz.<br>";
                if (p.gorevliPersoneller == null || p.gorevliPersoneller.ObjeSayisi == 0)
                    hata += "Görevli Personel Bilgisi boş bırakılamaz.<br>";
            }
            else if (durum == ENUMKontrolDurum.SIL)
            {
                if (p.kod == 0)
                    hata += "Silmek istenilen <b>Bilginin kodu</b> belirtilmemiş.<br>";
            }
            else if (durum == ENUMKontrolDurum.DURUMDEGISTIR)
            {
                if (p.kod == 0)
                    hata += "Görev kodu boş bırakılamaz.<br>";
            }
            return hata;
        }

        private static void KullanicininYetkiliOlduguMuhasebeBirim(Kullanici kullanan, ref int muhasebe, ref string gelenBirim)
        {
            if (kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.SISTEMYONETICISI))
                return;

            int calistigiMuhasebe = OrtakFonksiyonlar.ConvertToInt(kullanan.calistigiKurum, 0);

            //Kullanıcı yöneticisi ise sadece muhasebe set edilicek

            string birimListe = "";
            string muhasebeListe = "";

            string yetkiTipiDegeri = OrtakFonksiyonlar.YetkiTipiDegeriBul(kullanan.yetkiTipiDegerleri, "Personel Saymanlık-Birim Yetkisi");
            if (yetkiTipiDegeri != "")
            {
                ObjectArray d = new ObjectArray();
                int parca = OrtakFonksiyonlar.StringiParcala(yetkiTipiDegeri, ";", d);
                foreach (string yetkiTipiDegeriParca in d.objeler)
                {
                    string[] muhasebeBirim = yetkiTipiDegeriParca.Split('-');
                    if (muhasebeBirim.Length == 2)
                    {
                        if (muhasebeBirim[0].Trim() != "*")
                        {
                            if (muhasebeListe != "")
                                muhasebeListe += ";";
                            muhasebeListe += OrtakFonksiyonlar.ConvertToInt(muhasebeBirim[0], 0).ToString();
                        }

                        if (muhasebeBirim[1].Trim() != "*")
                        {
                            if (gelenBirim.Trim() == "")
                            {
                                //kullanicinin birden fazla birime yetkisi varsa ve bu birimlerden sadece biri için listeleme yapmak istiyorsa birim değerini direk alıyoruz.
                                //eğer kullanicinin yazdığı birim için yetkisi yok ise listele bölümündeki HRCYetkiKontrol.MuhasebeBirimYetkiVarMi kodu ile izni olmayan birimlerin süreçlerini göremeyecek.
                                if (birimListe != "") birimListe += ";";
                                birimListe += muhasebeBirim[1];
                            }
                        }
                        else
                        {
                            if (gelenBirim.Trim() != "")
                            {
                                if (birimListe != "") birimListe += ";";
                                birimListe += gelenBirim;
                            }
                        }
                    }
                }
            }
            else if (!kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.KULLANICIYONETICISI))
            {
                yetkiTipiDegeri = OrtakFonksiyonlar.YetkiTipiDegeriBul(kullanan.yetkiTipiDegerleri, "Taşınır Muhasebe Harcama Birim Yetkisi");
                if (yetkiTipiDegeri != "")
                {
                    ObjectArray d = new ObjectArray();
                    int parca = OrtakFonksiyonlar.StringiParcala(yetkiTipiDegeri, ";", d);
                    foreach (string yetkiTipiDegeriParca in d.objeler)
                    {
                        string[] muhasebeBirim = yetkiTipiDegeriParca.Split('-');
                        if (muhasebeBirim.Length == 3)
                        {
                            if (muhasebeBirim[0].Trim() != "*")
                            {
                                if (muhasebeListe != "")
                                    muhasebeListe += ";";
                                muhasebeListe += OrtakFonksiyonlar.ConvertToInt(muhasebeBirim[0], 0).ToString();
                            }

                            if (muhasebeBirim[1].Trim() != "*")
                            {
                                if (gelenBirim.Trim() == "")
                                {
                                    //kullanicinin birden fazla birime yetkisi varsa ve bu birimlerden sadece biri için listeleme yapmak istiyorsa birim değerini direk alıyoruz.
                                    //eğer kullanicinin yazdığı birim için yetkisi yok ise listele bölümündeki HRCYetkiKontrol.MuhasebeBirimYetkiVarMi kodu ile izni olmayan birimlerin süreçlerini göremeyecek.
                                    if (birimListe != "") birimListe += ";";
                                    if (muhasebeBirim[1].Length > 10)
                                        birimListe += muhasebeBirim[1].Replace(".", "").Substring(8, 3);//Son 3 hanesi
                                    else
                                        birimListe += "999";
                                }
                            }
                            else
                            {
                                if (gelenBirim.Trim() != "")
                                {
                                    if (birimListe != "") birimListe += ";";
                                    birimListe += gelenBirim;
                                }
                            }
                        }
                    }
                }
            }
            if (birimListe != "")
                gelenBirim = birimListe;
            if (birimListe == "")
                muhasebe = calistigiMuhasebe;
        }
    }
}
