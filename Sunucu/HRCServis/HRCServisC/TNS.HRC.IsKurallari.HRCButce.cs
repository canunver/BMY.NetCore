using System;
using System.Data;
using TNS.HRC;
using TNS.HRC.IsKurallari.Veri;
using TNS.KYM;
using TNS.BKD;
using TNS.Veri;

namespace TNS.HRC.IsKurallari
{
    /// <summary>
    /// Bütçe tertip bilgisi işlemlerini yürüten fonksiyonları yöneten sınıf
    /// </summary>
    public class HRCButce
    {
        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, muhasebe ve birim bilgisini veritabanından okur 
        /// ve MuhasebeBirim nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="muhasebe">İşlem yapılacak muhasebe kod bilgisi</param>
        /// <param name="birim">İşlem yapılacak birim kod bilgisi</param>
        /// <param name="kurKod">İşlem yapılacak kurumsal kod bilgisi</param>
        /// <returns>İstenilen kriterdeki, muhasebe ve birim bilgisi listesini MuhasebeBirim nesne koleksiyonu olarak tutan nesne</returns>
        public ObjectArray MuhasebeBirimListele(int muhasebe, string birim, string kurKod)
        {
            ObjectArray donusDeger = new ObjectArray();
            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", null);

            TABButce to = new TABButce(ve);
            IDataReader r = to.MuhasebeBirimGrupListele(muhasebe, birim, kurKod);

            if (to.Hata() == "")
            {
                while (r.Read())
                {
                    MuhasebeBirim mb = new MuhasebeBirim();

                    mb.muhasebeKod = VeriErisim.DegerAl(r, 0, 0);
                    mb.birimKod = VeriErisim.DegerAl(r, 1, "");
                    mb.muhasebeAd = VeriErisim.DegerAl(r, 2, "");

                    donusDeger.objeler.Add(mb);
                }
            }

            ve.VeriErisimKapat();

            return donusDeger;
        }

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, kurumsal kod bilgisini veritabanından okur 
        /// ve string olarak döner
        /// </summary>
        /// <param name="muhasebe">İşlem yapılacak muhasebe kod bilgisi</param>
        /// <param name="birim">İşlem yapılacak birim kod bilgisi</param>
        /// <returns></returns>
        public string MuhasebeBirimKurumsalKodAl(int muhasebe, string birim)
        {
            string donusDeger = "";
            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", null);

            TABButce to = new TABButce(ve);
            IDataReader r = to.MuhasebeBirimListele(muhasebe, birim);

            if (to.Hata() == "")
            {
                while (r.Read())
                {
                    if (donusDeger != "") donusDeger += ";";
                    donusDeger += VeriErisim.DegerAl(r, 4, "");
                }
            }

            ve.VeriErisimKapat();

            return donusDeger;

        }

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, kullanılabilir ödenek bilgisini veritabanından okur 
        /// ve ButceGider nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="yil">İşlem yapılacak yıl bilgisi</param>
        /// <param name="kontrolTur">İşlem yapılacak kontrol tür bilgisi (Ödenek Gönderme, Ödenek)</param>
        /// <param name="muhasebe">İşlem yapılacak muhasebe kod bilgisi</param>
        /// <param name="birim">İşlem yapılacak birim kod bilgisi</param>
        /// <param name="kur">İşlem yapılacak kurumsal kod bilgisi</param>
        /// <param name="fon">İşlem yapılacak fonksiyonel kod bilgisi</param>
        /// <param name="fin">İşlem yapılacak finansman kod bilgisi</param>
        /// <param name="eko">İşlem yapılacak ekonomik kod bilgisi</param>
        /// <returns>İstenilen kriterdeki, kullanılabilir ödenek listesini ButceGider nesne koleksiyonu olarak tutan nesne</returns>
        public ObjectArray KullanilabilirOdenek(int yil, int kontrolTur, int muhasebe, string birim, string kur, string fon, string fin, string eko)
        {
            int ay = DateTime.Now.Month;
            ObjectArray tertipListe = new ObjectArray();

            if (kontrolTur == (int)ENUMOdenekKontrolTur.ODENEKGONDERME)
                tertipListe = OdenekGondermeListele(yil, muhasebe, birim, kur, fon, fin, eko, (int)TNS.MUH.ENUMBelgeDurumu.TAHSISVEONAYLI);
            else
            {
                if (kur.Trim() == "")
                    kur = MuhasebeBirimKurumsalKodAl(muhasebe, birim);

                tertipListe = TertipListele(yil, kur, fon, fin, eko);
            }

            ObjectArray harcamaListe = HarcamaListele(yil, ay, muhasebe, birim, kur, fon, fin, eko);
            if (ay > 1 && harcamaListe.objeler.Count == 0)
            {
                ay--;
                harcamaListe = HarcamaListele(yil, ay, muhasebe, birim, kur, fon, fin, eko);
            }

            foreach (ButceGider t in tertipListe.objeler)
            {
                foreach (ButceHarcama h in harcamaListe.objeler)
                {
                    if (h.kur.Replace(".", "") == t.kur.Replace(".", "") &&
                        h.fon.Replace(".", "") == t.fon.Replace(".", "") &&
                        h.fin == t.fin &&
                        h.eko.Replace(".", "").StartsWith(t.eko.Replace(".", "")))
                    {
                        if (t.serbest > 0 && t.odenekGonderme == 0)
                            t.odenekGonderme = t.serbest;

                        t.serbest = t.serbest - h.harcama;
                        //break;
                    }
                }
            }

            //Avansların düşülmesi
            foreach (ButceGider t in tertipListe.objeler)
            {
                string[] bler = birim.Split(',');
                for (int i = 0; i < bler.Length; i++)
                {
                    if (bler[i].Trim() != "")
                    {
                        string avansKosul = yil.ToString() + "-" + muhasebe + "-" + bler[i] + "-" + t.kur + "-" + t.fon + "-" + t.fin + "-" + t.eko + "-";
                        string blokeKosul = yil.ToString() + "-" + muhasebe + "-" + bler[i] + "-" + t.kur + "-" + t.fon + "-" + t.fin + "-" + t.eko;
                        t.avans += HRCServis.servisUZY.UzayDegeriDbl(new Kullanici(), "MUHAVANS", avansKosul, false, "");
                        t.bloke += HRCServis.servisUZY.UzayDegeriDbl(new Kullanici(), "MUHBLOKEODENEK", blokeKosul, false, "");
                    }
                }

                t.serbest = t.serbest - t.avans - t.bloke;
            }

            return tertipListe;
        }

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, tertip bilgisini veritabanından okur 
        /// ve ButceGider nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="yil">İşlem yapılacak yıl bilgisi</param>
        /// <param name="kur">İşlem yapılacak kurumsal kod bilgisi</param>
        /// <param name="fon">İşlem yapılacak fonksiyonel kod bilgisi</param>
        /// <param name="fin">İşlem yapılacak finansman kod bilgisi</param>
        /// <param name="eko">İşlem yapılacak ekonomik kod bilgisi</param>
        /// <returns>İstenilen kriterdeki, tertip bilgi listesini ButceGider nesne koleksiyonu olarak tutan nesne</returns>
        public ObjectArray TertipListele(int yil, string kur, string fon, string fin, string eko)
        {
            ObjectArray bilgi = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", null);

            TABButce to = new TABButce(ve);
            IDataReader r = to.TertipListele(yil, kur, fon, fin, eko);

            if (to.Hata() == "")
            {
                while (r.Read())
                {
                    TNS.BKD.Tertip t = new TNS.BKD.Tertip(yil, ENUMTabloKod.TABButce);
                    ButceGider f = new ButceGider();

                    t.kodlar[0] = VeriErisim.DegerAl(r, 1, "");
                    t.kodlar[1] = VeriErisim.DegerAl(r, 2, "");
                    t.kodlar[2] = VeriErisim.DegerAl(r, 3, "");
                    t.kodlar[3] = VeriErisim.DegerAl(r, 4, "");
                    t.Noktala();

                    f.yil = VeriErisim.DegerAl(r, 0, 0);
                    f.kur = t.kodlar[0];
                    f.fon = t.kodlar[1];
                    f.fin = t.kodlar[2];
                    f.eko = t.kodlar[3];
                    f.kanun = VeriErisim.DegerAl(r, 5, 0.0);
                    f.kbo = VeriErisim.DegerAl(r, 6, 0.0);
                    f.eklenen = VeriErisim.DegerAl(r, 7, 0.0);
                    f.dusulen = VeriErisim.DegerAl(r, 8, 0.0);
                    f.serbest = VeriErisim.DegerAl(r, 9, 0.0);
                    f.yuzde10 = 0;

                    bilgi.objeler.Add(f);
                }
            }

            r.Close();

            ve.VeriErisimKapat();

            return bilgi;
        }

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, harcama bilgisini veritabanından okur 
        /// ve ButceHarcama nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="yil">İşlem yapılacak yıl bilgisi</param>
        /// <param name="ay">İşlem yapılacak ay bilgisi</param>
        /// <param name="muhasebe">İşlem yapılacak muhasebe kod bilgisi</param>
        /// <param name="birim">İşlem yapılacak birim kod bilgisi</param>
        /// <param name="kur">İşlem yapılacak kurumsal kod bilgisi</param>
        /// <param name="fon">İşlem yapılacak fonksiyonel kod bilgisi</param>
        /// <param name="fin">İşlem yapılacak finansman kod bilgisi</param>
        /// <param name="eko">İşlem yapılacak ekonomik kod bilgisi</param>
        /// <returns>İstenilen kriterdeki, harcama bilgi listesini ButceHarcama nesne koleksiyonu olarak tutan nesne</returns>
        public ObjectArray HarcamaListele(int yil, int ay, int muhasebe, string birim, string kur, string fon, string fin, string eko)
        {
            ObjectArray bilgi = new ObjectArray();

            if (ay == 0)
                ay = DateTime.Now.Month;

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", null);

            TABButce to = new TABButce(ve);

            int ekoSeviye = 3;//Bütçedeki ekonomik kod

            string[] ekolar = eko.Split(';');
            for (int i = 0; i < ekolar.Length; i++)
            {
                if (ekolar[i] != "" && ekolar[i].Replace(".", "").Length > 3)
                    ekoSeviye = 6;//Ekonomik kodun son seviyesine ulaşmak için
            }

            IDataReader r = r = to.HarcamaListele(yil, ay, muhasebe, birim, kur, fon, fin, eko, ekoSeviye);

            if (to.Hata() == "")
            {
                while (r.Read())
                {
                    TNS.BKD.Tertip t = new TNS.BKD.Tertip(yil, ENUMTabloKod.TABButce);
                    ButceHarcama f = new ButceHarcama();

                    t.kodlar[0] = VeriErisim.DegerAl(r, 3, "");
                    t.kodlar[1] = VeriErisim.DegerAl(r, 4, "");
                    t.kodlar[2] = VeriErisim.DegerAl(r, 5, "");
                    t.kodlar[3] = VeriErisim.DegerAl(r, 6, "");
                    t.Noktala();

                    f.yil = VeriErisim.DegerAl(r, 0, 0);
                    //f.ay = VeriErisim.DegerAl(r, 1, 0);
                    f.muhasebeKod = VeriErisim.DegerAl(r, 1, 0);
                    f.birimKod = VeriErisim.DegerAl(r, 2, "");
                    f.kur = t.kodlar[0];
                    f.fon = t.kodlar[1];
                    f.fin = t.kodlar[2];
                    f.eko = t.kodlar[3];
                    f.harcama = VeriErisim.DegerAl(r, 7, 0.0);

                    bilgi.objeler.Add(f);
                }
                r.Close();
            }
            else
            {
                ve.VeriErisimKapat();
                throw new Exception(to.Hata());
            }
            ve.VeriErisimKapat();

            return bilgi;
        }

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, ödenek gönderme belge bilgisini veritabanından okur 
        /// ve ButceGider nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="yil">İşlem yapılacak yıl bilgisi</param>
        /// <param name="muhasebe">İşlem yapılacak muhasebe kod bilgisi</param>
        /// <param name="birim">İşlem yapılacak birim kod bilgisi</param>
        /// <param name="kur">İşlem yapılacak kurumsal kod bilgisi</param>
        /// <param name="fon">İşlem yapılacak fonksiyonel kod bilgisi</param>
        /// <param name="fin">İşlem yapılacak finansman kod bilgisi</param>
        /// <param name="eko">İşlem yapılacak ekonomik kod bilgisi</param>
        /// <param name="durum">İşlem yapılacak ödenek gönderme belgesinin durum bilgisi</param>
        /// <returns>İstenilen kriterdeki, ödenek gönderme belge bilgi listesini ButceGider nesne koleksiyonu olarak tutan nesne</returns>
        public ObjectArray OdenekGondermeListele(int yil, int muhasebe, string birim, string kur, string fon, string fin, string eko, int durum)
        {
            ObjectArray bilgi = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", null);

            TABButce to = new TABButce(ve);
            IDataReader r = to.OdenekGondermeListele(yil, muhasebe, birim, kur, fon, fin, eko, durum);
            TNS.MUH.ENUMOdenekGondermeTur ogTur = TNS.MUH.Arac.OdenekGondermeTertipYapi(yil);

            if (to.Hata() == "")
            {
                while (r.Read())
                {
                    TNS.BKD.Tertip t = null;
                    if (ogTur != MUH.ENUMOdenekGondermeTur.eko2)
                        t = new TNS.BKD.Tertip(yil, ENUMTabloKod.TABPEBOdenek);
                    else
                        t = new TNS.BKD.Tertip(yil, ENUMTabloKod.TABButce);

                    ButceGider f = new ButceGider();

                    f.yil = VeriErisim.DegerAl(r, 0, 0);
                    f.muhasebe = VeriErisim.DegerAl(r, 1, 0);
                    f.birim = VeriErisim.DegerAl(r, 2, "");
                    f.odenekGonderme = VeriErisim.DegerAl(r, 7, 0.0);
                    f.serbest = f.odenekGonderme;

                    t.kodlar[0] = VeriErisim.DegerAl(r, 3, "");
                    t.kodlar[1] = VeriErisim.DegerAl(r, 4, "");
                    t.kodlar[2] = VeriErisim.DegerAl(r, 5, "");
                    t.kodlar[3] = VeriErisim.DegerAl(r, 6, "");
                    //if (OrtakServis.VarsayilanKurumBul() == "12")
                    //    t.kodlar[4] = VeriErisim.DegerAl(r, 8, "");
                    t.Noktala();

                    f.kur = t.kodlar[0];
                    f.fon = t.kodlar[1];
                    f.fin = t.kodlar[2];
                    f.eko = t.kodlar[3];
                    //if (OrtakServis.VarsayilanKurumBul() == "12")
                    //    f.peb = t.kodlar[4];

                    bilgi.objeler.Add(f);
                }
            }

            r.Close();

            ve.VeriErisimKapat();

            return bilgi;
        }

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, muhasebe ve birime ait tertip bilgisini veritabanından okur 
        /// ve MuhasebeBirimTertip nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="muhasebe">İşlem yapılacak muhasebe kod bilgisi</param>
        /// <param name="birim">İşlem yapılacak birim kod bilgisi</param>
        /// <param name="eko">İşlem yapılacak ekonomik kod bilgisi</param>
        /// <returns>İstenilen kriterdeki, muhasebe ve birime ait tertip bilgisi listesini MuhasebeBirimTertip nesne koleksiyonu olarak tutan nesne</returns>
        public ObjectArray MuhasebeBirimTertipListele(int muhasebe, string birim, string eko)
        {
            ObjectArray donusDeger = new ObjectArray();
            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", null);

            TABButce to = new TABButce(ve);
            IDataReader r = to.MuhasebeBirimTertipListele(muhasebe, birim, eko);

            if (to.Hata() == "")
            {
                while (r.Read())
                {
                    MuhasebeBirimTertip mb = new MuhasebeBirimTertip();

                    mb.muhasebeKod = VeriErisim.DegerAl(r, 0, 0);
                    mb.birimKod = VeriErisim.DegerAl(r, 1, "");
                    mb.kurKod = VeriErisim.DegerAl(r, 2, "");
                    mb.fonKod = VeriErisim.DegerAl(r, 3, "");
                    mb.finKod = VeriErisim.DegerAl(r, 4, "");
                    mb.ekoKod = VeriErisim.DegerAl(r, 5, "");
                    mb.pebEkoKod = VeriErisim.DegerAl(r, 6, "");

                    donusDeger.objeler.Add(mb);
                }
            }

            ve.VeriErisimKapat();

            return donusDeger;
        }

        /// <summary>
        /// Arguman olarak verilen, muhasebe ve birime ait tertip bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="bilgi">Kaydedilecek bilgiyi tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc MuhasebeBirimTertipKaydet(Kullanici kullanan, MuhasebeBirimTertip bilgi)
        {
            Sonuc sonuc = new Sonuc();

            //Kullanıcı sistem yöneticisimi kontrolü
            if (!kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.SISTEMYONETICISI))
            {
                sonuc.hataStr = TNS.HRC.HarcamaSurecleriResI.HRC00013;
                return sonuc;
            }

            string hata = "";
            if (bilgi.muhasebeKod <= 0)
                hata = TNS.HRC.HarcamaSurecleriResI.HRC00004 + "<br>";

            if (bilgi.birimKod == string.Empty)
                hata += TNS.HRC.HarcamaSurecleriResI.HRC00005 + "<br>";

            if (bilgi.kurKod == string.Empty)
                hata += TNS.HRC.HarcamaSurecleriResI.HRC00061 + "<br>";

            if (bilgi.fonKod == string.Empty)
                hata += TNS.HRC.HarcamaSurecleriResI.HRC00062 + "<br>";

            if (bilgi.finKod == string.Empty)
                hata += TNS.HRC.HarcamaSurecleriResI.HRC00063 + "<br>";

            if (bilgi.ekoKod == string.Empty)
                hata += TNS.HRC.HarcamaSurecleriResI.HRC00064 + "<br>";

            if (hata != "")
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = hata;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));
            TABButce to = new TABButce(ve);
            ve.BeginTransaction();

            sonuc = to.MuhasebeBirimTertipKaydet(bilgi);

            if (sonuc.islemSonuc)
            {
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

        /// <summary>
        /// Arguman olarak verilen, muhasebe ve birime ait tertip bilgisini veritabanından siler 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="bilgi">Kaydedilecek bilgiyi tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc MuhasebeBirimTertipSil(Kullanici kullanan, MuhasebeBirimTertip bilgi)
        {
            Sonuc sonuc = new Sonuc();

            //Kullanıcı sistem yöneticisimi kontrolü
            if (!kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.SISTEMYONETICISI))
            {
                sonuc.hataStr = TNS.HRC.HarcamaSurecleriResI.HRC00013;
                return sonuc;
            }

            if (bilgi.muhasebeKod <= 0)
                sonuc.hataStr = TNS.HRC.HarcamaSurecleriResI.HRC00004;
            if (bilgi.birimKod == string.Empty)
                sonuc.hataStr += TNS.HRC.HarcamaSurecleriResI.HRC00005;
            if (bilgi.kurKod == string.Empty)
                sonuc.hataStr += TNS.HRC.HarcamaSurecleriResI.HRC00061 + "<br>";
            if (bilgi.fonKod == string.Empty)
                sonuc.hataStr += TNS.HRC.HarcamaSurecleriResI.HRC00062 + "<br>";
            if (bilgi.finKod == string.Empty)
                sonuc.hataStr += TNS.HRC.HarcamaSurecleriResI.HRC00063 + "<br>";
            if (bilgi.ekoKod == string.Empty)
                sonuc.hataStr += TNS.HRC.HarcamaSurecleriResI.HRC00064 + "<br>";

            if (sonuc.hataStr != "")
            {
                sonuc.islemSonuc = false;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));
            TABButce to = new TABButce(ve);

            ve.BeginTransaction();

            sonuc = to.MuhasebeBirimTertipSil(bilgi);

            if (sonuc.islemSonuc)
            {
                ve.Commit();
                sonuc.bilgiStr = string.Format(TNS.HRC.HarcamaSurecleriResI.HRC00065, bilgi.muhasebeKod, bilgi.birimKod) + "<br>";
            }
            else
            {
                ve.Rollback();
                sonuc.hataStr = TNS.HRC.HarcamaSurecleriResI.HRC00019 + "<br>" + sonuc.hataStr;
            }

            ve.VeriErisimKapat();

            return sonuc;
        }

        ///// <summary>
        ///// Arguman olarak verilen kriterlere bağlı olarak, tertibe ait bütçe gider bilgisini veritabanından okur 
        ///// ve ButceGider nesne koleksiyonu olarak döner
        ///// </summary>
        ///// <param name="yil">İşlem yapılacak yıl bilgisi</param>
        ///// <param name="tertip">Listelemede kriter olarak kullanılacak değerleri içeren nesne koleksiyonu</param>
        ///// <returns>İstenilen kriterdeki, tertibe ait bütçe gider bilgisi listesini ButceGider nesne koleksiyonu olarak tutan nesne</returns>
        //public ObjectArray YuzdeOnRakamiVer(int yil, ObjectArray tertip)
        //{
        //    ObjectArray donenDeger = new ObjectArray();
        //    ObjectArray tertipListe = new ObjectArray();

        //    VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));
        //    TABButce to = new TABButce(ve);

        //    foreach (string t in tertip.objeler)
        //    {
        //        string[] bilgi = t.Replace(".", "").Split('-');
        //        tertipListe = TertipListele(yil, bilgi[0], bilgi[1], bilgi[2], bilgi[3]);

        //        foreach (ButceGider tt in tertipListe.objeler)
        //        {
        //            ButceGider donenGider = new ButceGider();
        //            donenGider.kur = bilgi[0];
        //            donenGider.fon = bilgi[1];
        //            donenGider.fin = bilgi[2];
        //            donenGider.eko = bilgi[3];
        //            donenGider.kbo = tt.kbo;

        //            IDataReader r = to.DogrudanTeminMiktariBul(yil, bilgi[0], bilgi[1], bilgi[2], bilgi[3]);
        //            if (to.Hata() == "")
        //            {
        //                while (r.Read())
        //                    donenGider.avans = VeriErisim.DegerAl(r, 0, 0.0);
        //            }
        //            r.Close();

        //            donenDeger.objeler.Add(donenGider);
        //        }

        //    }

        //    ve.VeriErisimKapat();

        //    return donenDeger;
        //}
    }
}
