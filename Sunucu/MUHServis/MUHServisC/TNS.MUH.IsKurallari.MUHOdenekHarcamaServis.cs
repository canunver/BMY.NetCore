using System;
using System.Collections;
using System.Data;
using TNS.BKD;
//using TNS.BUM;
using TNS.KYM;
using TNS.MUH.IsKurallari.Veri;
using TNS.Veri;

namespace TNS.MUH.IsKurallari
{
    /// <summary>
    /// ödenek/Harcama işlemlerinin yapıldığı sınıf
    /// </summary>
    public class MUHOdenekHarcamaServis
    {
        //static IBUMServis TanimlaBUM()
        //{
        //    IBUMServis servis = TNS.BUM.Arac.Tanimla();

        //    return servis;
        //}

        /// <summary>
        /// Harcama listele.
        /// </summary>
        /// <param name="kriter">kriter</param>
        /// <returns></returns>
        public ObjectArray OdenekDurum(OdenekDurumKriter kriter)
        {
            ObjectArray oVAN = new ObjectArray();

            if (kriter.ay == 0)
            {
                if (kriter.yil == DateTime.Now.Year)
                    kriter.ay = DateTime.Now.Month;
                else
                    kriter.ay = 12;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(null));
            IDataReader hr = null;

            OdenekDurum o = new OdenekDurum();
            o.yil = kriter.yil;

            if (kriter.odenekGoster)
            {
                TABOdenekHarcamaSorgu odenek = new TABOdenekHarcamaSorgu(ve);
                hr = odenek.OdenekListe(kriter);

                if (odenek.Hata() == "")
                {
                    while (hr.Read())
                    {
                        o.kanun = VeriErisim.DegerAl(hr, 0, 0.0);
                        o.kbo = VeriErisim.DegerAl(hr, 1, 0.0);
                        o.eklenen = VeriErisim.DegerAl(hr, 2, 0.0);
                        o.dusulen = VeriErisim.DegerAl(hr, 3, 0.0);
                        o.serbest = VeriErisim.DegerAl(hr, 4, 0.0);

                        o.toplamOdenek = o.kbo + o.eklenen - o.dusulen;
                        o.bloke = o.toplamOdenek - o.serbest;
                    }
                    hr.Close();
                }
                else
                {
                    oVAN.sonuc.hataStr = odenek.Hata();
                    return oVAN;
                }
            }

            //Harcama Toplamı Göster
            double harcamaToplam = 0;
            double avansToplam = 0;
            double blokeEdilenToplam = 0;
            if (kriter.harcamaGoster)
            {
                kriter.detay = ENUMOdenekDurumDetay.KONSOLIDE;
                TABOdenekHarcamaSorgu harcama = new TABOdenekHarcamaSorgu(ve);
                hr = harcama.HarcamaListe(kriter, false);
                if (harcama.Hata() == "")
                {
                    while (hr.Read())
                        harcamaToplam = VeriErisim.DegerAl(hr, 0, 0.0);
                    hr.Close();
                }
                else
                {
                    oVAN.sonuc.hataStr = harcama.Hata();
                    return oVAN;
                }

                //string avansKosul = kriter.yil.ToString() + "-" + kriter.ay + "-" + "" + "-" + "" + "-" + kriter.tertip.TertipStrOl();
                //if (kriter.muhasebe > 0)
                //    avansKosul = kriter.yil.ToString() + "-" + kriter.ay + "-" + kriter.muhasebe + "-" + kriter.birim + "-" + kriter.tertip.TertipStrOl();
                //avansToplam = MUHServis.UzaySayiDegerGetir("ISLAVANS", avansKosul, false, "");
                string avansKosul = kriter.yil.ToString() + "-" + "" + "-" + "" + "-" + kriter.tertip.kodlar[0] + "-" + kriter.tertip.kodlar[1] + "-" + kriter.tertip.kodlar[2] + "-" + kriter.tertip.kodlar[3] + "-1/8-";
                string blokeKosul = kriter.yil.ToString() + "-" + "" + "-" + "" + "-" + kriter.tertip.kodlar[0] + "-" + kriter.tertip.kodlar[1] + "-" + kriter.tertip.kodlar[2] + "-" + kriter.tertip.kodlar[3];
                if (kriter.muhasebe > 0)
                {
                    avansKosul = kriter.yil.ToString() + "-" + kriter.muhasebe + "-" + kriter.birim + "-" + kriter.tertip.kodlar[0] + "-" + kriter.tertip.kodlar[1] + "-" + kriter.tertip.kodlar[2] + "-" + kriter.tertip.kodlar[3] + "-1/8-";
                    blokeKosul = kriter.yil.ToString() + "-" + kriter.muhasebe + "-" + kriter.birim + "-" + kriter.tertip.kodlar[0] + "-" + kriter.tertip.kodlar[1] + "-" + kriter.tertip.kodlar[2] + "-" + kriter.tertip.kodlar[3];
                }
                avansToplam = MUHServis.UzaySayiDegerGetir(ve, null, "MUHAVANS", avansKosul, false, "");
                blokeEdilenToplam = MUHServis.UzaySayiDegerGetir(ve, null, "MUHBLOKEODENEK", blokeKosul, false, "");
            }

            //Ödenek Gönderme Toplamı Göster
            double onayliGon = 0;
            double onaysizGon = 0;
            double onayliTen = 0;
            double onaysizTen = 0;
            if (kriter.odenekGondermeGoster)
            {
                TABGondermeTenkis gonTen = new TABGondermeTenkis(ve);
                hr = gonTen.GondermeTenkisToplam(kriter);
                if (gonTen.Hata() == "")
                {
                    while (hr.Read())
                    {
                        int belgeTur = VeriErisim.DegerAl(hr, 0, 0);
                        int formDurum = VeriErisim.DegerAl(hr, 1, 0);
                        double miktar = Math.Abs(VeriErisim.DegerAl(hr, 2, 0.0));

                        if ((ENUMBelgeTuru)belgeTur == ENUMBelgeTuru.ODENEKGONDERME)
                        {
                            if ((ENUMBelgeDurumu)formDurum == ENUMBelgeDurumu.ONAYLI)
                                onayliGon += miktar;
                            else
                                onaysizGon += miktar;
                        }
                        if ((ENUMBelgeTuru)belgeTur == ENUMBelgeTuru.TENKIS)
                        {
                            if ((ENUMBelgeDurumu)formDurum == ENUMBelgeDurumu.ONAYLI)
                                onayliTen += miktar;
                            else
                                onaysizTen += miktar;
                        }
                    }
                    hr.Close();
                }
                else
                {
                    oVAN.sonuc.hataStr = gonTen.Hata();
                    return oVAN;
                }
            }

            //Ödeme Emri Toplamı Göster
            double onayliOde = 0;
            double onaysizOde = 0;
            if (kriter.odemeEmriGoster)
            {
                TABOdemeEmriMIF odeMIF = new TABOdemeEmriMIF(ve);
                hr = odeMIF.OdemeEmriMIFToplam(kriter, false);
                if (odeMIF.Hata() == "")
                {
                    while (hr.Read())
                    {
                        //int belgeTur=VeriErisim.DegerAl(hr,0,0);
                        int formDurum = VeriErisim.DegerAl(hr, 0, 0);
                        double miktar = Math.Abs(VeriErisim.DegerAl(hr, 1, 0.0));

                        //if ((ENUMBelgeTuru)belgeTur==ENUMBelgeTuru.ODEMEEMRI)
                        //{
                        if ((ENUMBelgeDurumu)formDurum == ENUMBelgeDurumu.ONAYLI || (ENUMBelgeDurumu)formDurum == ENUMBelgeDurumu.YEVMIYE_OESGBYEGONDER)
                            onayliOde += miktar;
                        else
                            onaysizOde += miktar;
                        //}
                    }
                    hr.Close();
                }
                else
                {
                    oVAN.sonuc.hataStr = odeMIF.Hata();
                    return oVAN;
                }
            }

            o.harcama = harcamaToplam;
            o.avans = avansToplam;
            o.blokeEdilen = blokeEdilenToplam;
            o.onayliTenkis = onayliTen;
            o.onaysizTenkis = onaysizTen;
            o.onayliOdenekGonderme = onayliGon;
            o.onaysizOdenekGonderme = onaysizGon;
            o.onayliOdemeEmri = onayliOde;
            o.onaysizOdemeEmri = onaysizOde;

            oVAN.objeler.Add(o);

            oVAN.sonuc.islemSonuc = true;

            ve.VeriErisimKapat();

            return oVAN;
        }

        /// <summary>
        /// Ödenek durum bilgisi listesi.
        /// </summary>
        /// <param name="kriter">İşlem yapılacak bilgilerin taşındığı nesne</param>
        /// <returns></returns>
        public ObjectArray OdenekDurumListesi(OdenekDurumKriter kriter)
        {
            if (kriter.yeniMetot)
                return OdenekDurumListesiYeni(null, kriter);
            ObjectArray oVAN = new ObjectArray();
            ObjectArray olar = new ObjectArray();
            ObjectArray hler = new ObjectArray();
            ObjectArray gler = new ObjectArray();
            ObjectArray mler = new ObjectArray();

            Tertip tb = new Tertip(DateTime.Now.Year, ENUMTabloKod.TABButce);
            Tertip to = new Tertip(DateTime.Now.Year, ENUMTabloKod.TABOdenek);

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(null));
            TABOdenekHarcamaSorgu odenek = new TABOdenekHarcamaSorgu(ve);

            int tut = kriter.ekoKodKirilim;
            string vKurum = TNS.DEG.Arac.VarsayilanKurumBul();
            if (TNS.HRC.Arac.CumhurbaskanligiMi())//Cumhur eko kod 2 kırılım bazında çalışıyor
                kriter.ekoKodKirilim = 2;

            IDataReader or = odenek.OdenekListe(kriter);

            kriter.ekoKodKirilim = tut;

            if (odenek.Hata() == "")
            {
                while (or.Read())
                {
                    ButceDurum o = new ButceDurum();

                    o.yil = VeriErisim.DegerAl(or, 0, 0);
                    o.tertip.kodlar[0] = OrtakFonksiyonlar.KodNoktaKoy((string)tb.Kirilimlar(0), VeriErisim.DegerAl(or, 1, ""));
                    o.tertip.kodlar[1] = OrtakFonksiyonlar.KodNoktaKoy((string)tb.Kirilimlar(1), VeriErisim.DegerAl(or, 2, ""));
                    o.tertip.kodlar[2] = OrtakFonksiyonlar.KodNoktaKoy((string)tb.Kirilimlar(2), VeriErisim.DegerAl(or, 3, ""));
                    o.tertip.kodlar[3] = OrtakFonksiyonlar.KodNoktaKoy((string)tb.Kirilimlar(3), VeriErisim.DegerAl(or, 4, ""));
                    o.kanun = VeriErisim.DegerAl(or, 5, 0.0);
                    o.kbo = VeriErisim.DegerAl(or, 6, 0.0);
                    o.eklenen = VeriErisim.DegerAl(or, 7, 0.0);
                    o.dusulen = VeriErisim.DegerAl(or, 8, 0.0);
                    o.serbest = VeriErisim.DegerAl(or, 9, 0.0);
                    o.toplamOdenek = o.kbo + o.eklenen - o.dusulen;
                    o.bloke = o.toplamOdenek - o.serbest;

                    olar.objeler.Add(o);
                }
            }

            or.Close();

            //Ödenek Gönderme belgeleri için
            if (kriter.odenekGondermeGoster)
            {
                TABGondermeTenkis gonTen = new TABGondermeTenkis(ve);
                kriter.detay = ENUMOdenekDurumDetay.TERTIP;

                or = gonTen.GondermeTenkisToplam(kriter);

                if (gonTen.Hata() == "")
                {
                    while (or.Read())
                    {
                        OdenekGondermeDurum o = new OdenekGondermeDurum();
                        int i = -1;

                        if (kriter.detay == ENUMOdenekDurumDetay.MUHASEBE || kriter.detay == ENUMOdenekDurumDetay.BIRIM)
                            o.muhasebe = VeriErisim.DegerAl(or, ++i, 0);
                        if (kriter.detay == ENUMOdenekDurumDetay.BIRIM)
                            o.birim = VeriErisim.DegerAl(or, ++i, "");

                        o.tertip.kodlar[0] = OrtakFonksiyonlar.KodNoktaKoy((string)tb.Kirilimlar(0), VeriErisim.DegerAl(or, ++i, ""));
                        o.tertip.kodlar[1] = OrtakFonksiyonlar.KodNoktaKoy((string)tb.Kirilimlar(1), VeriErisim.DegerAl(or, ++i, ""));
                        o.tertip.kodlar[2] = OrtakFonksiyonlar.KodNoktaKoy((string)tb.Kirilimlar(2), VeriErisim.DegerAl(or, ++i, ""));
                        o.tertip.kodlar[3] = OrtakFonksiyonlar.KodNoktaKoy((string)tb.Kirilimlar(3), VeriErisim.DegerAl(or, ++i, ""));
                        if (kriter.faaliyetGoster)
                            o.tertip.kodlar[3] = OrtakFonksiyonlar.KodNoktaKoy((string)tb.Kirilimlar(4), VeriErisim.DegerAl(or, ++i, ""));

                        o.belgeTur = VeriErisim.DegerAl(or, ++i, 0);
                        o.formDurum = VeriErisim.DegerAl(or, ++i, 0);

                        o.miktar = VeriErisim.DegerAl(or, ++i, 0.0);

                        gler.objeler.Add(o);
                    }
                }
                or.Close();
            }

            TABOdenekHarcamaSorgu harcama = new TABOdenekHarcamaSorgu(ve);
            kriter.detay = ENUMOdenekDurumDetay.TERTIP;

            or = harcama.HarcamaListe(kriter, false);

            if (harcama.Hata() == "")
            {
                while (or.Read())
                {
                    HarcamaDurum o = new HarcamaDurum();
                    int i = -1;
                    if (kriter.detay == ENUMOdenekDurumDetay.MUHASEBE || kriter.detay == ENUMOdenekDurumDetay.BIRIM)
                        o.muhasebe = VeriErisim.DegerAl(or, ++i, 0);
                    if (kriter.detay == ENUMOdenekDurumDetay.BIRIM)
                        o.birim = VeriErisim.DegerAl(or, ++i, "");

                    o.tertip.kodlar[0] = OrtakFonksiyonlar.KodNoktaKoy((string)to.Kirilimlar(0), VeriErisim.DegerAl(or, ++i, ""));
                    o.tertip.kodlar[1] = OrtakFonksiyonlar.KodNoktaKoy((string)to.Kirilimlar(1), VeriErisim.DegerAl(or, ++i, ""));
                    o.tertip.kodlar[2] = OrtakFonksiyonlar.KodNoktaKoy((string)to.Kirilimlar(2), VeriErisim.DegerAl(or, ++i, ""));
                    o.tertip.kodlar[3] = OrtakFonksiyonlar.KodNoktaKoy((string)to.Kirilimlar(3), VeriErisim.DegerAl(or, ++i, ""));
                    o.harcama = VeriErisim.DegerAl(or, ++i, 0.0);

                    hler.objeler.Add(o);
                }
            }
            or.Close();


            TABOdemeEmriMIF odeMIF = new TABOdemeEmriMIF(ve);
            kriter.detay = ENUMOdenekDurumDetay.TERTIP;

            or = odeMIF.OdemeEmriMIFToplam(kriter, false);

            if (odeMIF.Hata() == "")
            {
                while (or.Read())
                {
                    OdenekGondermeDurum o = new OdenekGondermeDurum();
                    int i = -1;

                    if (kriter.detay == ENUMOdenekDurumDetay.MUHASEBE || kriter.detay == ENUMOdenekDurumDetay.BIRIM)
                        o.muhasebe = VeriErisim.DegerAl(or, ++i, 0);
                    if (kriter.detay == ENUMOdenekDurumDetay.BIRIM)
                        o.birim = VeriErisim.DegerAl(or, ++i, "");

                    o.formDurum = VeriErisim.DegerAl(or, ++i, 0);

                    o.tertip.kodlar[0] = OrtakFonksiyonlar.KodNoktaKoy((string)to.Kirilimlar(0), VeriErisim.DegerAl(or, ++i, ""));
                    o.tertip.kodlar[1] = OrtakFonksiyonlar.KodNoktaKoy((string)to.Kirilimlar(1), VeriErisim.DegerAl(or, ++i, ""));
                    o.tertip.kodlar[2] = OrtakFonksiyonlar.KodNoktaKoy((string)to.Kirilimlar(2), VeriErisim.DegerAl(or, ++i, ""));
                    o.tertip.kodlar[3] = OrtakFonksiyonlar.KodNoktaKoy((string)to.Kirilimlar(3), VeriErisim.DegerAl(or, ++i, ""));

                    //o.belgeTur = VeriErisim.DegerAl(or, ++i, 0);

                    o.miktar = VeriErisim.DegerAl(or, ++i, 0.0);

                    mler.objeler.Add(o);
                }
            }
            or.Close();


            int sOdenek = 0;
            int sHarcama = 0;
            bool cik = false;
            while (!cik)
            {
                OdenekDurum oYeni = new OdenekDurum();
                ButceDurum o = null;
                HarcamaDurum h = null;
                string oTertip = "";
                string hTertip = "";

                if (sOdenek >= olar.objeler.Count && sHarcama >= hler.objeler.Count)
                    break;

                if (sOdenek < olar.objeler.Count)
                {
                    o = (ButceDurum)olar.objeler[sOdenek];
                    oTertip = o.tertip.TertipStrOl();
                }
                else
                    oTertip = "999999999999999999";

                if (sHarcama < hler.objeler.Count)
                {
                    h = (HarcamaDurum)hler.objeler[sHarcama];
                    hTertip = SubStringBul(h.tertip.TertipStrOl(), oTertip.Length);
                }
                else
                    hTertip = "999999999999999999";

                if (String.Compare(oTertip, hTertip) == 0)
                {
                    oYeni.yil = o.yil;
                    oYeni.tertipButce = o.tertip;
                    oYeni.tertipOdemeEmri = h.tertip;
                    oYeni.kanun = o.kanun;
                    oYeni.kbo = o.kbo;
                    oYeni.serbest = o.serbest;
                    oYeni.eklenen = o.eklenen;
                    oYeni.dusulen = o.dusulen;
                    oYeni.toplamOdenek = o.toplamOdenek;
                    oYeni.bloke = o.bloke;
                    oYeni.harcama = h.harcama;
                    oYeni.muhasebe = h.muhasebe;
                    oYeni.birim = h.birim;

                    oVAN.objeler.Add(oYeni);

                    sHarcama++;
                    //Bir sonraki harcama şimdiki ödenek ile aynı tertiptemi (4düzey eko kodlu olunca olabilir)
                    bool ayni = false;
                    if (sHarcama < hler.objeler.Count)
                    {
                        h = (HarcamaDurum)hler.objeler[sHarcama];
                        hTertip = SubStringBul(h.tertip.TertipStrOl(), oTertip.Length);
                        if (String.Compare(oTertip, hTertip) == 0)
                            ayni = true;
                    }

                    if (!ayni)
                        sOdenek++;
                }
                else if (String.Compare(oTertip, hTertip) > 0)//Harcama tertibi büyük
                {
                    string[] dizi = h.tertip.TertipStrOl().Split('-');
                    string e = (string)dizi[3];
                    e = SubStringBul(e, ((string)tb.Kirilimlar(3)).Length);
                    oYeni.tertipButce.kodlar[0] = (string)dizi[0];
                    oYeni.tertipButce.kodlar[1] = (string)dizi[1];
                    oYeni.tertipButce.kodlar[2] = (string)dizi[2];
                    oYeni.tertipButce.kodlar[3] = e;

                    oYeni.yil = h.yil;
                    oYeni.tertipOdemeEmri = h.tertip;
                    oYeni.harcama = h.harcama;
                    oYeni.muhasebe = h.muhasebe;
                    oYeni.birim = h.birim;

                    oVAN.objeler.Add(oYeni);
                    sHarcama++;
                }
                else
                {
                    oYeni.yil = o.yil;
                    oYeni.tertipButce = o.tertip;
                    oYeni.kanun = o.kanun;
                    oYeni.kbo = o.kbo;
                    oYeni.serbest = o.serbest;
                    oYeni.eklenen = o.eklenen;
                    oYeni.dusulen = o.dusulen;
                    oYeni.toplamOdenek = o.toplamOdenek;
                    oYeni.bloke = o.bloke;

                    oVAN.objeler.Add(oYeni);
                    sOdenek++;
                }
            }

            for (int i = 0; i < gler.objeler.Count; i++)
            {
                OdenekGondermeDurum og = new OdenekGondermeDurum();
                og = (OdenekGondermeDurum)gler.objeler[i];
                og.tertip.kodlar[3] = SubStringBul(og.tertip.kodlar[3], 4);
                string ogt = og.tertip.TertipStrOl();


                for (int j = 0; j < oVAN.objeler.Count; j++)
                {
                    OdenekDurum o = new OdenekDurum();
                    o = (OdenekDurum)oVAN.objeler[j];
                    string t = o.tertipButce.TertipStrOl();
                    if (t == ogt)
                    {
                        if (og.belgeTur == (int)ENUMBelgeTuru.ODENEKGONDERME)
                        {
                            if (og.formDurum == (int)ENUMBelgeDurumu.ONAYLI)
                                o.onayliOdenekGonderme += og.miktar;
                            else
                                o.onaysizOdenekGonderme += og.miktar;
                        }
                        if (og.belgeTur == (int)ENUMBelgeTuru.TENKIS)
                        {
                            if (og.formDurum == (int)ENUMBelgeDurumu.ONAYLI)
                                o.onayliTenkis += og.miktar;
                            else
                                o.onaysizTenkis += og.miktar;
                        }
                        //gler.OdenekGondermeDurum[i]=o;
                        break;
                    }
                }
            }


            ve.VeriErisimKapat();

            //Ödeme emrini ekle
            if (kriter.odemeEmriGoster)
            {

                foreach (OdenekGondermeDurum m in mler.objeler)
                {
                    string hTertip = m.tertip.TertipStrOl();
                    bool bulundu = false;
                    foreach (TNS.MUH.OdenekDurum o in oVAN.objeler)
                    {
                        string oTertip = o.tertipOdemeEmri.TertipStrOl();
                        if (oTertip == hTertip)
                        {
                            o.onayliOdemeEmri += m.miktar;
                            bulundu = true;
                            break;
                        }
                    }

                    if (!bulundu)
                    {
                        OdenekDurum oYeni = new OdenekDurum();
                        for (int jj = 0; jj < oVAN.objeler.Count; jj++)
                        {
                            TNS.MUH.OdenekDurum o = new OdenekDurum();
                            o = (OdenekDurum)oVAN.objeler[jj];

                            string oTertip = "";

                            oTertip = o.tertipOdemeEmri.TertipStrOl();
                            if (oTertip == "")
                                oTertip = o.tertipButce.TertipStrOl();
                            if (String.Compare(oTertip, hTertip) > 0)//Harcama tertibi büyük
                            {
                                string[] dizi = m.tertip.TertipStrOl().Split('-');
                                string e = (string)dizi[3];
                                e = SubStringBul(e, ((string)tb.Kirilimlar(3)).Length);
                                oYeni.tertipButce.kodlar[0] = (string)dizi[0];
                                oYeni.tertipButce.kodlar[1] = (string)dizi[1];
                                oYeni.tertipButce.kodlar[2] = (string)dizi[2];
                                oYeni.tertipButce.kodlar[3] = e;

                                oYeni.yil = m.yil;
                                oYeni.tertipOdemeEmri = m.tertip;
                                oYeni.onayliOdemeEmri = m.miktar;
                                oYeni.muhasebe = m.muhasebe;
                                oYeni.birim = m.birim;

                                oYeni.avans = o.avans;
                                oYeni.bloke = o.bloke;
                                oYeni.blokeEdilen = o.blokeEdilen;
                                oYeni.dusulen = o.dusulen;
                                oYeni.eklenen = o.eklenen;
                                oYeni.kanun = o.kanun;
                                oYeni.kbo = o.kbo;
                                oYeni.serbest = o.serbest;
                                oYeni.toplamOdenek = o.toplamOdenek;

                                oVAN.objeler.Insert(jj, oYeni);
                                break;
                            }
                        }
                    }
                }
            }
            return oVAN;
        }

        private string SubStringBul(string str, int uzunluk)
        {
            if (str.Length > uzunluk)
                return str.Substring(0, uzunluk);
            return str;
        }

        public ObjectArray OdenekDurumListesiYeni(Kullanici kullanan, OdenekDurumKriter kriter)
        {
            if (kullanan != null)
            {
                bool yetkiVar = TNS.MUH.Yetki.MuhasebeBirimYetkiVarMi(kullanan, kriter.muhasebe.ToString(), kriter.birim, true);
                if (!yetkiVar)
                    throw new Exception("Verdiğiniz birimler ilgili verileri görme yetkiniz bulunmamaktadır.");
            }

            Tertip to = new Tertip(kriter.yil, ENUMTabloKod.TABPEBOdenek);
            TNS.MUH.OdenekDurum.eko2Uz = to.KodUzunlukBul(3, 2, false);
            TNS.MUH.OdenekDurum.eko3Uz = to.KodUzunlukBul(3, 3, false);
            string vKurum = TNS.DEG.Arac.VarsayilanKurumBul();

            System.Collections.Generic.Dictionary<string, OdenekDurum> hs = new System.Collections.Generic.Dictionary<string, OdenekDurum>();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABOdenekHarcamaSorgu odenek = new TABOdenekHarcamaSorgu(ve);
            IDataReader or;

            int tut = kriter.ekoKodKirilim;
            bool tut23 = kriter.eko23;
            if (TNS.MUH.Arac.OdenekGondermeEko2liMi(kriter.yil) && kriter.ekoKodKirilim > 2)
            {
                kriter.ekoKodKirilim = 2;
                kriter.eko23 = TNS.BKD.Arac.Eko2_3Bul(kullanan);
            }

            #region odenekGoster
            if (TNS.HRC.Arac.CumhurbaskanligiMi())
                kriter.ekoKodKirilim = 2;

            if (kriter.odenekGoster)
            {
                or = odenek.OdenekListe(kriter);

                if (or != null)
                {
                    while (or.Read())
                    {
                        OdenekDurum o = new OdenekDurum();
                        int i = -1;

                        if (!kriter.konsolide)
                        {
                            o.yil = VeriErisim.DegerAl(or, ++i, 0);
                            o.tertipButce.kodlar[0] = OrtakFonksiyonlar.KodNoktaKoy((string)to.Kirilimlar(0), VeriErisim.DegerAl(or, ++i, ""));
                            o.tertipButce.kodlar[1] = OrtakFonksiyonlar.KodNoktaKoy((string)to.Kirilimlar(1), VeriErisim.DegerAl(or, ++i, ""));
                            o.tertipButce.kodlar[2] = OrtakFonksiyonlar.KodNoktaKoy((string)to.Kirilimlar(2), VeriErisim.DegerAl(or, ++i, ""));
                            o.tertipButce.kodlar[3] = OrtakFonksiyonlar.KodNoktaKoy((string)to.Kirilimlar(3), VeriErisim.DegerAl(or, ++i, ""));
                            if (kriter.faaliyetGoster)
                                o.tertipButce.kodlar[4] = OrtakFonksiyonlar.KodNoktaKoy((string)to.Kirilimlar(4), VeriErisim.DegerAl(or, ++i, ""));
                        }
                        o.kanun = VeriErisim.DegerAl(or, ++i, 0.0);
                        o.kbo = VeriErisim.DegerAl(or, ++i, 0.0);
                        o.eklenen = VeriErisim.DegerAl(or, ++i, 0.0);
                        o.dusulen = VeriErisim.DegerAl(or, ++i, 0.0);
                        o.serbest = VeriErisim.DegerAl(or, ++i, 0.0);
                        o.toplamOdenek = o.kbo + o.eklenen - o.dusulen;
                        o.bloke = o.toplamOdenek - o.serbest;
                        hs[o.HashKod] = o;
                    }
                    or.Close();
                }
                if (TNS.MUH.OdenekDurum.SerbestMPden)
                {
                    or = odenek.SerbestListe(kriter);
                    if (or != null)
                    {
                        while (or.Read())
                        {
                            OdenekDurum o = new OdenekDurum();
                            int i = -1;
                            if (!kriter.konsolide)
                            {
                                o.yil = VeriErisim.DegerAl(or, ++i, 0);
                                o.tertipButce.kodlar[0] = OrtakFonksiyonlar.KodNoktaKoy((string)to.Kirilimlar(0), VeriErisim.DegerAl(or, ++i, ""));
                                o.tertipButce.kodlar[1] = OrtakFonksiyonlar.KodNoktaKoy((string)to.Kirilimlar(1), VeriErisim.DegerAl(or, ++i, ""));
                                o.tertipButce.kodlar[2] = OrtakFonksiyonlar.KodNoktaKoy((string)to.Kirilimlar(2), VeriErisim.DegerAl(or, ++i, ""));
                                o.tertipButce.kodlar[3] = OrtakFonksiyonlar.KodNoktaKoy((string)to.Kirilimlar(3), VeriErisim.DegerAl(or, ++i, ""));
                                if (kriter.faaliyetGoster)
                                    o.tertipButce.kodlar[4] = OrtakFonksiyonlar.KodNoktaKoy((string)to.Kirilimlar(4), VeriErisim.DegerAl(or, ++i, ""));
                            }
                            o.ahpSerbest = VeriErisim.DegerAl(or, ++i, 0.0);
                            OdenekDurum HashOD = YerBul(hs, o.HashKod);
                            if (HashOD == null)
                            {
                                hs[o.HashKod] = o;
                                HashOD = o;
                            }
                            else
                                HashOD.ahpSerbest += o.ahpSerbest;
                            HashOD.ahpBloke = HashOD.toplamOdenek - HashOD.ahpSerbest;
                        }
                        or.Close();
                    }
                }
                kriter.detay = ENUMOdenekDurumDetay.TERTIP;
            }
            #endregion

            #region odenekGondermeGoster
            if (kriter.odenekGondermeGoster)
            {
                TABGondermeTenkis gonTen = new TABGondermeTenkis(ve);

                or = gonTen.GondermeTenkisToplam(kriter);

                if (or != null)
                {
                    while (or.Read())
                    {
                        OdenekDurum o = new OdenekDurum();
                        int i = -1;

                        if (kriter.detay == ENUMOdenekDurumDetay.MUHASEBE || kriter.detay == ENUMOdenekDurumDetay.BIRIM)
                            o.muhasebe = VeriErisim.DegerAl(or, ++i, 0);
                        if (kriter.detay == ENUMOdenekDurumDetay.BIRIM)
                            o.birim = VeriErisim.DegerAl(or, ++i, "");
                        if (kriter.detay == ENUMOdenekDurumDetay.EKO1BASAMAK)
                        {
                            o.muhasebe = VeriErisim.DegerAl(or, ++i, 0);
                            o.birim = VeriErisim.DegerAl(or, ++i, "");
                            o.tertipButce.kodlar[3] = OrtakFonksiyonlar.KodNoktaKoy((string)to.Kirilimlar(3), VeriErisim.DegerAl(or, ++i, ""));
                        }
                        else
                        {
                            if (!kriter.konsolide)
                            {
                                o.tertipButce.kodlar[0] = OrtakFonksiyonlar.KodNoktaKoy((string)to.Kirilimlar(0), VeriErisim.DegerAl(or, ++i, ""));
                                o.tertipButce.kodlar[1] = OrtakFonksiyonlar.KodNoktaKoy((string)to.Kirilimlar(1), VeriErisim.DegerAl(or, ++i, ""));
                                o.tertipButce.kodlar[2] = OrtakFonksiyonlar.KodNoktaKoy((string)to.Kirilimlar(2), VeriErisim.DegerAl(or, ++i, ""));
                                o.tertipButce.kodlar[3] = OrtakFonksiyonlar.KodNoktaKoy((string)to.Kirilimlar(3), VeriErisim.DegerAl(or, ++i, ""));
                                if (kriter.faaliyetGoster)
                                    o.tertipButce.kodlar[4] = OrtakFonksiyonlar.KodNoktaKoy((string)to.Kirilimlar(4), VeriErisim.DegerAl(or, ++i, ""));
                            }
                        }
                        OdenekDurum HashOD = YerBul(hs, o.HashKod);
                        if (HashOD == null)
                        {
                            hs[o.HashKod] = o;
                            HashOD = o;
                        }

                        int belgeTur = VeriErisim.DegerAl(or, ++i, 0);
                        int formDurum = VeriErisim.DegerAl(or, ++i, 0);
                        double miktar = VeriErisim.DegerAl(or, ++i, 0.0);
                        if (belgeTur == (int)ENUMBelgeTuru.TENKIS)
                            if (formDurum == (int)ENUMBelgeDurumu.ONAYLI)
                                HashOD.onayliTenkis += miktar;
                            else
                                HashOD.onaysizTenkis += miktar;
                        else
                            if (formDurum == (int)ENUMBelgeDurumu.ONAYLI)
                            HashOD.onayliOdenekGonderme += miktar;
                        else
                            HashOD.onaysizOdenekGonderme += miktar;
                    }
                    or.Close();
                }
            }
            #endregion

            kriter.ekoKodKirilim = tut;
            kriter.eko23 = tut23;
            #region harcamaGoster
            if (kriter.harcamaGoster)
            {
                TABOdenekHarcamaSorgu harcama = new TABOdenekHarcamaSorgu(ve);
                or = harcama.HarcamaListe(kriter, false);
                if (or != null)
                {
                    while (or.Read())
                    {
                        OdenekDurum o = new OdenekDurum();
                        int i = -1;
                        if (kriter.detay == ENUMOdenekDurumDetay.MUHASEBE || kriter.detay == ENUMOdenekDurumDetay.BIRIM)
                            o.muhasebe = VeriErisim.DegerAl(or, ++i, 0);
                        if (kriter.detay == ENUMOdenekDurumDetay.BIRIM)
                            o.birim = VeriErisim.DegerAl(or, ++i, "");
                        if (kriter.detay == ENUMOdenekDurumDetay.EKO1BASAMAK)
                        {
                            o.muhasebe = VeriErisim.DegerAl(or, ++i, 0);
                            o.birim = VeriErisim.DegerAl(or, ++i, "");
                            o.tertipButce.kodlar[3] = OrtakFonksiyonlar.KodNoktaKoy((string)to.Kirilimlar(3), VeriErisim.DegerAl(or, ++i, ""));
                        }
                        else
                        {
                            if (!kriter.konsolide)
                            {
                                o.tertipButce.kodlar[0] = OrtakFonksiyonlar.KodNoktaKoy((string)to.Kirilimlar(0), VeriErisim.DegerAl(or, ++i, ""));
                                o.tertipButce.kodlar[1] = OrtakFonksiyonlar.KodNoktaKoy((string)to.Kirilimlar(1), VeriErisim.DegerAl(or, ++i, ""));
                                o.tertipButce.kodlar[2] = OrtakFonksiyonlar.KodNoktaKoy((string)to.Kirilimlar(2), VeriErisim.DegerAl(or, ++i, ""));
                                o.tertipButce.kodlar[3] = OrtakFonksiyonlar.KodNoktaKoy((string)to.Kirilimlar(3), VeriErisim.DegerAl(or, ++i, ""));
                                if (kriter.harcamaPebli)
                                    o.tertipButce.kodlar[4] = OrtakFonksiyonlar.KodNoktaKoy((string)to.Kirilimlar(4), VeriErisim.DegerAl(or, ++i, ""));
                            }
                        }

                        OdenekDurum HashOD = YerBul(hs, o.HashKod);
                        if (HashOD == null)
                        {
                            hs[o.HashKod] = o;
                            HashOD = o;
                        }
                        HashOD.harcama += VeriErisim.DegerAl(or, ++i, 0.0);
                    }
                    or.Close();
                }
            }
            #endregion

            #region mifGoster
            TABOdemeEmriMIF odeMIF = new TABOdemeEmriMIF(ve);
            if (kriter.mifGoster || kriter.odemeEmriGoster)
            {
                or = odeMIF.OdemeEmriMIFToplam(kriter, false);
                if (or != null)
                {
                    while (or.Read())
                    {
                        OdenekDurum o = new OdenekDurum();
                        int i = -1;

                        int formDurum = VeriErisim.DegerAl(or, ++i, 0);
                        if (kriter.detay == ENUMOdenekDurumDetay.MUHASEBE || kriter.detay == ENUMOdenekDurumDetay.BIRIM)
                            o.muhasebe = VeriErisim.DegerAl(or, ++i, 0);
                        if (kriter.detay == ENUMOdenekDurumDetay.BIRIM)
                            o.birim = VeriErisim.DegerAl(or, ++i, "");
                        if (kriter.detay == ENUMOdenekDurumDetay.EKO1BASAMAK)
                        {
                            o.muhasebe = VeriErisim.DegerAl(or, ++i, 0);
                            o.birim = VeriErisim.DegerAl(or, ++i, "");
                            o.tertipButce.kodlar[3] = OrtakFonksiyonlar.KodNoktaKoy((string)to.Kirilimlar(3), VeriErisim.DegerAl(or, ++i, ""));
                        }
                        else
                        {
                            if (!kriter.konsolide)
                            {
                                o.tertipButce.kodlar[0] = OrtakFonksiyonlar.KodNoktaKoy((string)to.Kirilimlar(0), VeriErisim.DegerAl(or, ++i, ""));
                                o.tertipButce.kodlar[1] = OrtakFonksiyonlar.KodNoktaKoy((string)to.Kirilimlar(1), VeriErisim.DegerAl(or, ++i, ""));
                                o.tertipButce.kodlar[2] = OrtakFonksiyonlar.KodNoktaKoy((string)to.Kirilimlar(2), VeriErisim.DegerAl(or, ++i, ""));
                                o.tertipButce.kodlar[3] = OrtakFonksiyonlar.KodNoktaKoy((string)to.Kirilimlar(3), VeriErisim.DegerAl(or, ++i, ""));
                                if (kriter.faaliyetGoster)
                                    o.tertipButce.kodlar[4] = OrtakFonksiyonlar.KodNoktaKoy((string)to.Kirilimlar(4), VeriErisim.DegerAl(or, ++i, ""));
                            }
                        }

                        double miktar = VeriErisim.DegerAl(or, ++i, 0.0);

                        OdenekDurum HashOD = YerBul(hs, o.HashKod);
                        if (HashOD == null)
                        {
                            hs[o.HashKod] = o;
                            HashOD = o;
                        }

                        if (formDurum == (int)ENUMBelgeDurumu.ONAYLI || formDurum == (int)ENUMBelgeDurumu.YEVMIYE_OESGBYEGONDER)
                        {
                            HashOD.onayliOdemeEmri += miktar;
                        }
                        else
                            HashOD.onaysizOdemeEmri += miktar;
                    }
                    or.Close();
                }
            }
            #endregion

            if (kriter.avansGoster)
            {
                // Avansları al
                or = odeMIF.OdemeEmriMIFToplam(kriter, true);
                if (or != null)
                {
                    while (or.Read())
                    {
                        OdenekDurum o = new OdenekDurum();
                        int i = -1;

                        int formDurum = VeriErisim.DegerAl(or, ++i, 0);
                        if (kriter.detay == ENUMOdenekDurumDetay.MUHASEBE || kriter.detay == ENUMOdenekDurumDetay.BIRIM)
                            o.muhasebe = VeriErisim.DegerAl(or, ++i, 0);
                        if (kriter.detay == ENUMOdenekDurumDetay.BIRIM)
                            o.birim = VeriErisim.DegerAl(or, ++i, "");
                        if (kriter.detay == ENUMOdenekDurumDetay.EKO1BASAMAK)
                        {
                            o.muhasebe = VeriErisim.DegerAl(or, ++i, 0);
                            o.birim = VeriErisim.DegerAl(or, ++i, "");
                            o.tertipButce.kodlar[3] = OrtakFonksiyonlar.KodNoktaKoy((string)to.Kirilimlar(3), VeriErisim.DegerAl(or, ++i, ""));
                        }
                        else
                        {
                            if (!kriter.konsolide)
                            {
                                o.tertipButce.kodlar[0] = OrtakFonksiyonlar.KodNoktaKoy((string)to.Kirilimlar(0), VeriErisim.DegerAl(or, ++i, ""));
                                o.tertipButce.kodlar[1] = OrtakFonksiyonlar.KodNoktaKoy((string)to.Kirilimlar(1), VeriErisim.DegerAl(or, ++i, ""));
                                o.tertipButce.kodlar[2] = OrtakFonksiyonlar.KodNoktaKoy((string)to.Kirilimlar(2), VeriErisim.DegerAl(or, ++i, ""));
                                o.tertipButce.kodlar[3] = OrtakFonksiyonlar.KodNoktaKoy((string)to.Kirilimlar(3), VeriErisim.DegerAl(or, ++i, ""));
                                if (kriter.faaliyetGoster)
                                    o.tertipButce.kodlar[4] = OrtakFonksiyonlar.KodNoktaKoy((string)to.Kirilimlar(4), VeriErisim.DegerAl(or, ++i, ""));
                            }
                        }

                        double miktar = VeriErisim.DegerAl(or, ++i, 0.0);

                        OdenekDurum HashOD = YerBul(hs, o.HashKod);
                        if (HashOD == null)
                        {
                            hs[o.HashKod] = o;
                            HashOD = o;
                        }
                        HashOD.avans += miktar;
                    }
                    or.Close();
                }
            }

            #region harcamaBlokeGoster
            if (kriter.harcamaBlokeGoster)
            {
                // Süreçlerde bloke konulmuş tertipleri al
                or = odeMIF.SurecBlokeToplam(kriter);

                if (or != null)
                {
                    while (or.Read())
                    {
                        OdenekDurum o = new OdenekDurum();
                        int i = -1;

                        if (kriter.detay == ENUMOdenekDurumDetay.MUHASEBE || kriter.detay == ENUMOdenekDurumDetay.BIRIM)
                            o.muhasebe = VeriErisim.DegerAl(or, ++i, 0);
                        if (kriter.detay == ENUMOdenekDurumDetay.BIRIM)
                            o.birim = VeriErisim.DegerAl(or, ++i, "");

                        o.tertipButce.kodlar[0] = OrtakFonksiyonlar.KodNoktaKoy((string)to.Kirilimlar(0), VeriErisim.DegerAl(or, ++i, ""));
                        o.tertipButce.kodlar[1] = OrtakFonksiyonlar.KodNoktaKoy((string)to.Kirilimlar(1), VeriErisim.DegerAl(or, ++i, ""));
                        o.tertipButce.kodlar[2] = OrtakFonksiyonlar.KodNoktaKoy((string)to.Kirilimlar(2), VeriErisim.DegerAl(or, ++i, ""));
                        o.tertipButce.kodlar[3] = OrtakFonksiyonlar.KodNoktaKoy((string)to.Kirilimlar(3), VeriErisim.DegerAl(or, ++i, ""));
                        if (kriter.faaliyetGoster)
                            o.tertipButce.kodlar[4] = OrtakFonksiyonlar.KodNoktaKoy((string)to.Kirilimlar(4), VeriErisim.DegerAl(or, ++i, ""));

                        double miktar = VeriErisim.DegerAl(or, ++i, 0.0);

                        OdenekDurum HashOD = YerBul(hs, o.HashKod);
                        if (HashOD == null)
                        {
                            hs[o.HashKod] = o;
                            HashOD = o;
                        }

                        HashOD.blokeEdilen += miktar;
                    }
                    or.Close();
                }
            }
            #endregion

            ObjectArray oa = new ObjectArray();
            Hashtable ht = new Hashtable();
            foreach (System.Collections.Generic.KeyValuePair<string, OdenekDurum> item in hs)
            {
                oa.Ekle(item.Value);
                if (kriter.ekoAdBul)
                {
                    item.Value.tertipButce.adlar[3] = EkoAdBul(ve, ht, kriter.yil, item.Value.tertipButce.kodlar[3]);
                }
            }
            ve.VeriErisimKapat();
            oa.objeler.Sort();
            return oa;
        }

        private string EkoAdBul(VeriErisim ve, Hashtable ht, int yil, string ekoKod)
        {
            string st;
            try
            {
                st = (string)ht[ekoKod];
            }
            catch
            {
                st = null;
            }
            if (st == null)
            {
                st = MUHServis.UzayYaziDegerGetir(ve, null, "ABSEKO", yil + "-" + ekoKod, true, "");
                ht[ekoKod] = st;
            }
            return st;
        }

        private OdenekDurum YerBul(System.Collections.Generic.Dictionary<string, OdenekDurum> hs, string p)
        {
            try
            {
                return hs[p];
            }
            catch { }
            return null;
        }

        /// <summary>
        /// Harcama listele.
        /// </summary>
        /// <param name="kriter">kriter</param>
        /// <returns></returns>
        public ObjectArray HarcamaListele(OdenekDurumKriter kriter)
        {
            ObjectArray hListe = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(null));
            TABOdenekHarcamaSorgu harcama = new TABOdenekHarcamaSorgu(ve);
            IDataReader or = harcama.HarcamaListe(kriter, false);

            if (!string.IsNullOrEmpty(harcama.Hata()))
            {
                hListe.sonuc.islemSonuc = false;
                hListe.sonuc.hataStr = harcama.Hata();
                return hListe;
            }

            Tertip to = new Tertip(DateTime.Now.Year, ENUMTabloKod.TABOdenek);
            while (or.Read())
            {
                HarcamaDurum o = new HarcamaDurum();
                int i = -1;
                if (kriter.detay == ENUMOdenekDurumDetay.MUHASEBE || kriter.detay == ENUMOdenekDurumDetay.BIRIM)
                    o.muhasebe = VeriErisim.DegerAl(or, ++i, 0);
                if (kriter.detay == ENUMOdenekDurumDetay.BIRIM)
                    o.birim = VeriErisim.DegerAl(or, ++i, "");

                o.tertip.kodlar[0] = OrtakFonksiyonlar.KodNoktaKoy((string)to.Kirilimlar(0), VeriErisim.DegerAl(or, ++i, ""));
                o.tertip.kodlar[1] = OrtakFonksiyonlar.KodNoktaKoy((string)to.Kirilimlar(1), VeriErisim.DegerAl(or, ++i, ""));
                o.tertip.kodlar[2] = OrtakFonksiyonlar.KodNoktaKoy((string)to.Kirilimlar(2), VeriErisim.DegerAl(or, ++i, ""));
                o.tertip.kodlar[3] = OrtakFonksiyonlar.KodNoktaKoy((string)to.Kirilimlar(3), VeriErisim.DegerAl(or, ++i, ""));
                o.harcama = VeriErisim.DegerAl(or, ++i, 0.0);

                hListe.objeler.Add(o);
            }
            or.Close();

            if (hListe.objeler.Count == 0)
                hListe.sonuc.bilgiStr = ButceMuhasebeResI.MUH051;

            ve.VeriErisimKapat();
            return hListe;
        }

        /// <summary>
        /// Harcama durum bilgisi listele.
        /// </summary>
        /// <param name="kriter">İşlem yapılacak bilgilerin taşındığı nesne</param>
        /// <returns></returns>
        public ObjectArray HarcamaDurumListesi(Kullanici kullanan, OdenekDurumKriter kriter)
        {
            if (!MUH.IsKurallari.MUHYetkiServis.GormeYetkisiVarMiKurum(kullanan, (int)ENUMBelgeTuru.ODENEKGONDERME, kriter.tertip.kodlar[0]))
            {
                ObjectArray hata = new ObjectArray();
                hata.sonuc.DegerAta(ButceMuhasebeResI.MUH048);
                return hata;
            }
            if (!MUH.IsKurallari.MUHYetkiServis.GormeYetkisiVarMiKurum(kullanan, (int)ENUMBelgeTuru.ODENEKGONDERME, kriter.tertip.kodlar[0]))
            {
                ObjectArray hata = new ObjectArray();
                hata.sonuc.DegerAta(ButceMuhasebeResI.MUH048);
                return hata;
            }
            if (!TNS.MUH.Yetki.MuhasebeBirimYetkiVarMi(kullanan, kriter.muhasebe.ToString(), kriter.birim))
            {
                ObjectArray hata = new ObjectArray();
                hata.sonuc.hataStr = ButceMuhasebeResI.MUH048;
                hata.sonuc.islemSonuc = false;
                return hata;
            }

            if (kriter.yeniMetot)
            {
                return OdenekDurumListesiYeni(kullanan, kriter);
            }
            if (kullanan != null)
            {
                bool yetkiVar = TNS.MUH.Yetki.MuhasebeBirimYetkiVarMi(kullanan, kriter.muhasebe.ToString(), kriter.birim, true);
                if (!yetkiVar)
                    throw new Exception("Verdiğiniz birimler ilgili verileri görme yetkiniz bulunmamaktadır.");
            }

            ObjectArray oVAN = new ObjectArray();
            ObjectArray hler = new ObjectArray();
            ObjectArray gler = new ObjectArray();

            Tertip to = new Tertip(DateTime.Now.Year, ENUMTabloKod.TABOdenek);
            Tertip tb = new Tertip(DateTime.Now.Year, ENUMTabloKod.TABPEBOdenek);

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(kullanan));
            IDataReader or = null;

            TABGondermeTenkis gonTen = new TABGondermeTenkis(ve);
            or = gonTen.GondermeTenkisToplam(kriter);

            if (gonTen.Hata() == "")
            {
                double miktar = 0;
                while (or.Read())
                {
                    OdenekDurum o = new OdenekDurum();

                    int i = -1;

                    if (kriter.detay == ENUMOdenekDurumDetay.MUHASEBE || kriter.detay == ENUMOdenekDurumDetay.BIRIM)
                        o.muhasebe = VeriErisim.DegerAl(or, ++i, 0);
                    if (kriter.detay == ENUMOdenekDurumDetay.BIRIM)
                        o.birim = VeriErisim.DegerAl(or, ++i, "");

                    o.tertipButce.kodlar[0] = OrtakFonksiyonlar.KodNoktaKoy((string)tb.Kirilimlar(0), VeriErisim.DegerAl(or, ++i, ""));
                    o.tertipButce.kodlar[1] = OrtakFonksiyonlar.KodNoktaKoy((string)tb.Kirilimlar(1), VeriErisim.DegerAl(or, ++i, ""));
                    o.tertipButce.kodlar[2] = OrtakFonksiyonlar.KodNoktaKoy((string)tb.Kirilimlar(2), VeriErisim.DegerAl(or, ++i, ""));
                    o.tertipButce.kodlar[3] = OrtakFonksiyonlar.KodNoktaKoy((string)tb.Kirilimlar(3), VeriErisim.DegerAl(or, ++i, ""));
                    if (kriter.faaliyetGoster)
                        o.tertipButce.kodlar[4] = OrtakFonksiyonlar.KodNoktaKoy((string)tb.Kirilimlar(4), VeriErisim.DegerAl(or, ++i, ""));

                    int belgeTur = VeriErisim.DegerAl(or, ++i, 0);
                    int formDurum = VeriErisim.DegerAl(or, ++i, 0);

                    miktar = VeriErisim.DegerAl(or, ++i, 0.0);

                    bool bulundu = false;
                    for (int kk = 0; kk < gler.objeler.Count; kk++)
                    {
                        OdenekDurum temp = (OdenekDurum)gler.objeler[kk];
                        if (temp.muhasebe == o.muhasebe && temp.birim == o.birim && temp.tertipButce.TertipStrOl() == o.tertipButce.TertipStrOl())
                        {
                            if (belgeTur == (int)ENUMBelgeTuru.ODENEKGONDERME)
                            {
                                if (formDurum == (int)ENUMBelgeDurumu.ONAYLI)
                                    temp.onayliOdenekGonderme += miktar;
                                else
                                    temp.onaysizOdenekGonderme += miktar;
                            }

                            if (belgeTur == (int)ENUMBelgeTuru.TENKIS)
                            {
                                if (formDurum == (int)ENUMBelgeDurumu.ONAYLI)
                                    temp.onayliTenkis += miktar;
                                else
                                    temp.onaysizTenkis += miktar;
                            }
                            bulundu = true;
                            break;
                        }
                    }

                    if (!bulundu)
                    {
                        if (belgeTur == (int)ENUMBelgeTuru.ODENEKGONDERME)
                        {
                            if (formDurum == (int)ENUMBelgeDurumu.ONAYLI)
                                o.onayliOdenekGonderme += miktar;
                            else
                                o.onaysizOdenekGonderme += miktar;
                        }

                        if (belgeTur == (int)ENUMBelgeTuru.TENKIS)
                        {
                            if (formDurum == (int)ENUMBelgeDurumu.ONAYLI)
                                o.onayliTenkis += miktar;
                            else
                                o.onaysizTenkis += miktar;
                        }

                        gler.objeler.Add(o);
                    }
                }

                or.Close();
            }

            //Harcama
            if (kriter.harcamaGoster)
            {
                TABOdenekHarcamaSorgu harcama = new TABOdenekHarcamaSorgu(ve);
                or = harcama.HarcamaListe(kriter, false);

                if (harcama.Hata() == "")
                {
                    while (or.Read())
                    {
                        HarcamaDurum o = new HarcamaDurum();
                        int i = -1;
                        if (kriter.detay == ENUMOdenekDurumDetay.MUHASEBE || kriter.detay == ENUMOdenekDurumDetay.BIRIM)
                            o.muhasebe = VeriErisim.DegerAl(or, ++i, 0);
                        if (kriter.detay == ENUMOdenekDurumDetay.BIRIM)
                            o.birim = VeriErisim.DegerAl(or, ++i, "");

                        o.tertip.kodlar[0] = OrtakFonksiyonlar.KodNoktaKoy((string)to.Kirilimlar(0), VeriErisim.DegerAl(or, ++i, ""));
                        o.tertip.kodlar[1] = OrtakFonksiyonlar.KodNoktaKoy((string)to.Kirilimlar(1), VeriErisim.DegerAl(or, ++i, ""));
                        o.tertip.kodlar[2] = OrtakFonksiyonlar.KodNoktaKoy((string)to.Kirilimlar(2), VeriErisim.DegerAl(or, ++i, ""));
                        o.tertip.kodlar[3] = OrtakFonksiyonlar.KodNoktaKoy((string)to.Kirilimlar(3), VeriErisim.DegerAl(or, ++i, ""));
                        o.harcama = VeriErisim.DegerAl(or, ++i, 0.0);

                        hler.objeler.Add(o);
                    }
                    or.Close();
                }
            }

            int sOdenek = 0;
            int sHarcama = 0;
            int eskiMuh = 0;
            string eskiBirim = "";
            bool cik = false;
            int uzunlukButceTertip = ((string)tb.Kirilimlar(0)).Length + ((string)tb.Kirilimlar(1)).Length + ((string)tb.Kirilimlar(2)).Length + ((string)tb.Kirilimlar(3)).Length;
            uzunlukButceTertip += 3;
            while (!cik)
            {
                OdenekDurum oYeni = new OdenekDurum();
                OdenekDurum o = null;
                HarcamaDurum h = null;
                string oTertip = "";
                string hTertip = "";
                int oMuh = 0;
                int hMuh = 0;
                string oBir = "";
                string hBir = "";

                if (sOdenek >= gler.objeler.Count && sHarcama >= hler.objeler.Count)
                    break;

                if (sOdenek < gler.objeler.Count)
                {
                    o = (OdenekDurum)gler.objeler[sOdenek];
                    oTertip = o.tertipButce.TertipStrOl();
                    oMuh = o.muhasebe;
                    oBir = o.birim;
                }
                else
                {
                    oTertip = "999999999999999999";
                    oMuh = 9999999;
                    oBir = "999";
                }

                if (sHarcama < hler.objeler.Count)
                {
                    h = (HarcamaDurum)hler.objeler[sHarcama];
                    hTertip = h.tertip.TertipStrOl();
                    if (hTertip.Length > uzunlukButceTertip)
                        hTertip = SubStringBul(hTertip, uzunlukButceTertip);
                    hMuh = h.muhasebe;
                    hBir = h.birim;
                }
                else
                {
                    hTertip = "999999999999999999";
                    hMuh = 9999999;
                    hBir = "999";
                }

                string karsilastirO = oMuh.ToString().PadLeft(5, '0') + "-" + oBir.ToString().PadLeft(5, '0') + "-" + oTertip;
                string karsilastirH = hMuh.ToString().PadLeft(5, '0') + "-" + hBir.ToString().PadLeft(5, '0') + "-" + hTertip;

                if (String.Compare(karsilastirO, karsilastirH) == 0)
                {
                    oYeni.yil = o.yil;
                    oYeni.tertipButce = o.tertipButce;
                    oYeni.tertipOdemeEmri = h.tertip;
                    oYeni.harcama = h.harcama;
                    oYeni.muhasebe = h.muhasebe;
                    oYeni.birim = h.birim;
                    oYeni.onayliOdenekGonderme = o.onayliOdenekGonderme;
                    oYeni.onayliTenkis = o.onayliTenkis;
                    oYeni.onaysizOdenekGonderme = o.onaysizOdenekGonderme;
                    oYeni.onaysizTenkis = o.onaysizTenkis;

                    if (eskiMuh != h.muhasebe)
                    {
                        oYeni.muhasebeAd = MUHServis.UzayYaziDegerGetir(ve, kullanan, "MUHMUHASEBE", h.muhasebe.ToString(), true, "MUHASEBEAD");
                        eskiMuh = h.muhasebe;
                        eskiBirim = "";
                    }
                    if (eskiBirim != h.birim && h.birim != "")
                    {
                        oYeni.birimAd = MUHServis.UzayYaziDegerGetir(ve, kullanan, "MUHMUHASEBEBIRIM", h.birim + "-" + h.muhasebe.ToString() + "-" + (string)h.tertip.kodlar[0], true, "BIRIMAD");
                        eskiBirim = h.birim;
                    }

                    oVAN.objeler.Add(oYeni);

                    sHarcama++;
                    //Bir sonraki harcama şimdiki ödenek ile aynı tertiptemi (4düzey eko kodlu olunca olabilir)
                    bool ayni = false;
                    if (sHarcama < hler.objeler.Count)
                    {
                        h = (HarcamaDurum)hler.objeler[sHarcama];
                        hTertip = h.tertip.TertipStrOl();
                        if (hTertip.Length > uzunlukButceTertip)
                            hTertip = SubStringBul(hTertip, uzunlukButceTertip);

                        hMuh = h.muhasebe;
                        hBir = h.birim;
                        karsilastirH = hMuh.ToString().PadLeft(5, '0') + "-" + hBir.ToString().PadLeft(5, '0') + "-" + hTertip;
                        if (String.Compare(karsilastirO, karsilastirH) == 0)
                            ayni = true;
                    }

                    if (!ayni)
                        sOdenek++;
                }
                else if (String.Compare(karsilastirO, karsilastirH) > 0)//Harcama tertibi büyük
                {
                    string[] dizi = h.tertip.TertipStrOl().Split('-');
                    string e = (string)dizi[3];
                    try
                    {
                        e = SubStringBul(e, ((string)tb.Kirilimlar(3)).Length);
                    }
                    catch
                    {
                        throw new Exception("Elma");
                    }
                    oYeni.tertipButce.kodlar[0] = (string)dizi[0];
                    oYeni.tertipButce.kodlar[1] = (string)dizi[1];
                    oYeni.tertipButce.kodlar[2] = (string)dizi[2];
                    oYeni.tertipButce.kodlar[3] = e;

                    oYeni.yil = h.yil;
                    oYeni.tertipOdemeEmri = h.tertip;
                    oYeni.harcama = h.harcama;
                    oYeni.muhasebe = h.muhasebe;
                    oYeni.birim = h.birim;

                    if (eskiMuh != h.muhasebe)
                    {
                        oYeni.muhasebeAd = MUHServis.UzayYaziDegerGetir(ve, kullanan, "MUHMUHASEBE", h.muhasebe.ToString(), true, "MUHASEBEAD");
                        eskiMuh = h.muhasebe;
                        eskiBirim = "";
                    }
                    if (eskiBirim != h.birim && h.birim != "")
                    {
                        oYeni.birimAd = MUHServis.UzayYaziDegerGetir(ve, kullanan, "MUHMUHASEBEBIRIM", h.birim + "-" + h.muhasebe.ToString() + "-" + (string)h.tertip.kodlar[0], true, "BIRIMAD");
                        eskiBirim = h.birim;
                    }

                    oVAN.objeler.Add(oYeni);
                    sHarcama++;
                }
                else
                {
                    oYeni.yil = o.yil;
                    oYeni.tertipButce = o.tertipButce;
                    oYeni.muhasebe = o.muhasebe;
                    oYeni.birim = o.birim;
                    oYeni.onayliOdenekGonderme = o.onayliOdenekGonderme;
                    oYeni.onayliTenkis = o.onayliTenkis;
                    oYeni.onaysizOdenekGonderme = o.onaysizOdenekGonderme;
                    oYeni.onaysizTenkis = o.onaysizTenkis;

                    if (eskiMuh != o.muhasebe)
                    {
                        oYeni.muhasebeAd = MUHServis.UzayYaziDegerGetir(ve, kullanan, "MUHMUHASEBE", o.muhasebe.ToString(), true, "MUHASEBEAD");
                        eskiMuh = o.muhasebe;
                        eskiBirim = "";
                    }
                    if (eskiBirim != o.birim && o.birim != "")
                    {
                        oYeni.birimAd = MUHServis.UzayYaziDegerGetir(ve, kullanan, "MUHMUHASEBEBIRIM", o.birim + "-" + o.muhasebe.ToString() + "-" + (string)o.tertipButce.kodlar[0], true, "BIRIMAD");
                        eskiBirim = o.birim;
                    }

                    oVAN.objeler.Add(oYeni);
                    sOdenek++;
                }
            }

            if (kriter.odenekGoster)
            {
                foreach (OdenekDurum o in oVAN.objeler)
                    o.toplamOdenek = MUHServis.UzaySayiDegerGetir(ve, kullanan, "ABSBUTCEGIDER", kriter.yil + "-" + o.tertipButce.TertipStrOl(), false, "SUM(KBO+EKLENEN-DUSULEN)");
            }

            //Ödeme Emri
            if (kriter.odemeEmriGoster)
            {
                TABOdemeEmriMIF odeMIF = new TABOdemeEmriMIF(ve);

                or = odeMIF.OdemeEmriMIFToplam(kriter, false);

                if (odeMIF.Hata() == "")
                {
                    while (or.Read())
                    {
                        int muhasebe = 0;
                        string birim = "";
                        string kur;
                        string fon;
                        string fin;
                        string eko;
                        string peb;
                        double miktar = 0;
                        int i = 0;
                        if (kriter.detay == ENUMOdenekDurumDetay.MUHASEBE || kriter.detay == ENUMOdenekDurumDetay.BIRIM)
                            muhasebe = VeriErisim.DegerAl(or, ++i, 0);
                        if (kriter.detay == ENUMOdenekDurumDetay.BIRIM)
                            birim = VeriErisim.DegerAl(or, ++i, "");

                        kur = OrtakFonksiyonlar.KodNoktaKoy((string)to.Kirilimlar(0), VeriErisim.DegerAl(or, ++i, ""));
                        fon = OrtakFonksiyonlar.KodNoktaKoy((string)to.Kirilimlar(1), VeriErisim.DegerAl(or, ++i, ""));
                        fin = OrtakFonksiyonlar.KodNoktaKoy((string)to.Kirilimlar(2), VeriErisim.DegerAl(or, ++i, ""));
                        eko = OrtakFonksiyonlar.KodNoktaKoy((string)to.Kirilimlar(3), VeriErisim.DegerAl(or, ++i, ""));
                        string tertip = kur + "-" + fon + "-" + fin + "-" + eko;
                        if (kriter.faaliyetGoster)
                        {
                            peb = OrtakFonksiyonlar.KodNoktaKoy((string)tb.Kirilimlar(4), VeriErisim.DegerAl(or, ++i, ""));
                            tertip += "-" + peb;
                        }
                        else peb = "";
                        miktar = VeriErisim.DegerAl(or, ++i, 0.0);

                        foreach (OdenekDurum o in oVAN.objeler)
                        {
                            if (muhasebe > 0 && muhasebe == o.muhasebe)
                            {
                                if (birim == o.birim)
                                {
                                    if (tertip.Contains(o.tertipOdemeEmri.TertipStrOl()) && o.tertipOdemeEmri.TertipStrOl() != "")
                                        o.onaysizOdemeEmri += miktar;
                                    else if (tertip.Contains(o.tertipButce.TertipStrOl()) && o.tertipButce.TertipStrOl() != "")
                                    {
                                        if (o.tertipButce.kodlar.Length >= 3) // && eko.Replace(".", "") == o.tertipButce.kodlar[3].Replace(".", ""))
                                            o.onaysizOdemeEmri += miktar;
                                    }
                                }
                            }
                            else
                            {
                                if (muhasebe == 0 && tertip.Contains(o.tertipOdemeEmri.TertipStrOl()) && o.tertipOdemeEmri.TertipStrOl() != "")
                                    o.onaysizOdemeEmri += miktar;
                            }
                        }
                    }
                    or.Close();
                }
            }

            ve.VeriErisimKapat();
            return oVAN;
        }

        /// <summary>
        /// Bütce durumu bilgilendirme.
        /// </summary>
        /// <param name="degerler">ButceDurumuVAN tipindeki collection</param>
        /// <returns>İşlem başarılı olmuşsa ButceDurumuVAN collectionunda degerler döner.Hata oluşmuşsa Sonuc yapısı döndürülür.</returns>
        public ObjectArray ButceDurumuBilgilendirme(ObjectArray degerler)
        {
            ObjectArray donusDegerler = new ObjectArray();
            //IBUMServis servisBUM = TanimlaBUM();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(null));

            foreach (ButceDurumu bd in degerler.objeler)
            {
                ButceDurumu donusDeger = new ButceDurumu();

                OdenekDurumKriter kr = new OdenekDurumKriter();
                kr.yil = bd.yil;
                kr.tertip.kodlar[0] = bd.kurKod;
                kr.tertip.kodlar[1] = bd.fonKod;
                kr.tertip.kodlar[2] = bd.finKod;
                kr.tertip.kodlar[3] = bd.ekoKod;
                kr.detay = ENUMOdenekDurumDetay.KONSOLIDE;
                kr.konsolide = true;
                if (bd.ay > 0)
                    kr.ay = bd.ay;

                donusDeger.yil = bd.yil;
                donusDeger.kurKod = bd.kurKod;
                donusDeger.kurAd = bd.kurAd;
                donusDeger.fonKod = bd.fonKod;
                donusDeger.fonAd = bd.fonAd;
                donusDeger.finKod = bd.finKod;
                donusDeger.finAd = bd.finAd;
                donusDeger.ekoKod = bd.ekoKod;
                donusDeger.ekoAd = bd.ekoAd;

                IDataReader hr = null;
                if (bd.butceGetir)
                {
                    //Ödenek Toplamı Göster
                    TABOdenekHarcamaSorgu odenek = new TABOdenekHarcamaSorgu(ve);
                    hr = odenek.OdenekListe(kr);
                    if (odenek.Hata() == "")
                    {
                        if (hr.Read())
                        {
                            donusDeger.kbo = VeriErisim.DegerAl(hr, 1, 0.0);
                            donusDeger.eklenen = VeriErisim.DegerAl(hr, 2, 0.0);
                            donusDeger.dusulen = VeriErisim.DegerAl(hr, 3, 0.0);
                            donusDeger.serbest = VeriErisim.DegerAl(hr, 4, 0.0);
                        }
                    }
                    hr.Close();
                }

                if (bd.harcamaGetir)
                {
                    //Harcama Toplamı Göster
                    TABOdenekHarcamaSorgu harcama = new TABOdenekHarcamaSorgu(ve);
                    hr = harcama.HarcamaListe(kr, false);

                    if (harcama.Hata() == "")
                        if (hr.Read())
                            donusDeger.harcama = VeriErisim.DegerAl(hr, 0, 0.0);
                    hr.Close();
                }

                if (bd.odenekGodermeGetir)
                {
                    //Ödenek Gönderme Toplamı Göster
                    TABGondermeTenkis gonTen = new TABGondermeTenkis(ve);
                    hr = gonTen.GondermeTenkisToplam(kr);
                    if (gonTen.Hata() == "")
                    {
                        while (hr.Read())
                        {
                            int belgeTur = VeriErisim.DegerAl(hr, 0, 0);
                            int formDurum = VeriErisim.DegerAl(hr, 1, 0);
                            double miktar = Math.Abs(VeriErisim.DegerAl(hr, 2, 0.0));

                            if ((ENUMBelgeTuru)belgeTur == ENUMBelgeTuru.ODENEKGONDERME)
                            {
                                if ((ENUMBelgeDurumu)formDurum == ENUMBelgeDurumu.ONAYLI)
                                    donusDeger.onayliOdenekGonderme += miktar;
                                else
                                    donusDeger.onaysizOdenekGonderme += miktar;
                            }
                            if ((ENUMBelgeTuru)belgeTur == ENUMBelgeTuru.TENKIS)
                            {
                                if ((ENUMBelgeDurumu)formDurum == ENUMBelgeDurumu.ONAYLI)
                                    donusDeger.onayliTenkis += miktar;
                                else
                                    donusDeger.onaysizTenkis += miktar;
                            }
                        }
                    }
                    hr.Close();
                }

                if (bd.muhasebeGetir)
                {
                    //ButçeMuhasebe Göster
                    foreach (ButceMuhasebe bm in bd.butceMuhasebe.objeler)
                    {
                        ButceMuhasebe bmd = new ButceMuhasebe();

                        TABOdenekHarcamaSorgu btMuhasebe = new TABOdenekHarcamaSorgu(ve);

                        bmd.yil = bm.yil;
                        bmd.hesapKod = bm.hesapKod;
                        bmd.kurumKod = bm.kurumKod;
                        bmd.kurKod = bd.kurKod;
                        bmd.fonKod = bd.fonKod;
                        bmd.finKod = bd.finKod;
                        bmd.ekoKod = bd.ekoKod;

                        hr = btMuhasebe.MuhasebeListe(bmd);

                        if (btMuhasebe.Hata() == "")
                        {
                            if (hr.Read())
                            {
                                bmd.borc = VeriErisim.DegerAl(hr, 0, 0.0);
                                bmd.alacak = VeriErisim.DegerAl(hr, 1, 0.0);
                            }
                        }
                        hr.Close();

                        donusDeger.butceMuhasebe.objeler.Add(bmd);
                    }
                }

                if (bd.butceIslemGetir)
                {
                    foreach (ButceIslem bi in bd.butceIslem.objeler)
                    {
                        ButceIslem bmi = new ButceIslem();
                        bmi.yil = bi.yil;
                        bmi.islemKod = bi.islemKod;
                        bmi.kurKod = bd.kurKod;
                        bmi.fonKod = bd.fonKod;
                        bmi.finKod = bd.finKod;

                        bmi.ekoKod = bd.ekoKod;
                        ObjectArray kodlar = new ObjectArray();
                        kodlar.objeler.Add(bmi.kurKod);
                        kodlar.objeler.Add(bmi.fonKod);
                        kodlar.objeler.Add(bmi.finKod);
                        kodlar.objeler.Add(bmi.ekoKod);


                        //BITertipBilgi bti = servisBUM.EklenenDusulenBulOnayli(bmi.yil, bmi.islemKod, kodlar);

                        //bmi.eklenen = bti.eklenen;
                        //bmi.dusulen = bti.dusulen;

                        donusDeger.butceIslem.objeler.Add(bmi);
                    }
                }

                donusDegerler.objeler.Add(donusDeger);
            }

            ve.VeriErisimKapat();

            return donusDegerler;
        }

        /// <summary>
        /// Ödenek üstü harcama listesi.
        /// </summary>
        /// <param name="kriter">İşlem yapılacak bilgilerin taşındığı nesne</param>
        /// <returns></returns>
        public ObjectArray OdenekUstuHarcamaListesi(OdenekDurumKriter kriter)
        {
            ObjectArray oVAN = new ObjectArray();

            Tertip tb = new Tertip(DateTime.Now.Year, ENUMTabloKod.TABButce);

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(null));

            TABOdenekHarcamaSorgu odenekHarcama = new TABOdenekHarcamaSorgu(ve);
            IDataReader or = odenekHarcama.OdenekUstuHarcama(kriter);

            if (odenekHarcama.Hata() == "")
            {
                while (or.Read())
                {
                    OdenekDurum o = new OdenekDurum();

                    o.tertipButce.kodlar[0] = OrtakFonksiyonlar.KodNoktaKoy((string)tb.Kirilimlar(0), VeriErisim.DegerAl(or, 4, ""));
                    o.tertipButce.kodlar[1] = OrtakFonksiyonlar.KodNoktaKoy((string)tb.Kirilimlar(1), VeriErisim.DegerAl(or, 5, ""));
                    o.tertipButce.kodlar[2] = OrtakFonksiyonlar.KodNoktaKoy((string)tb.Kirilimlar(2), VeriErisim.DegerAl(or, 6, ""));
                    o.tertipButce.kodlar[3] = OrtakFonksiyonlar.KodNoktaKoy((string)tb.Kirilimlar(3), VeriErisim.DegerAl(or, 7, ""));

                    o.muhasebe = VeriErisim.DegerAl(or, 2, 0);
                    o.birim = VeriErisim.DegerAl(or, 3, "");
                    o.onayliOdenekGonderme = VeriErisim.DegerAl(or, 8, 0.0);
                    o.harcama = VeriErisim.DegerAl(or, 9, 0.0);

                    oVAN.objeler.Add(o);
                }

                or.Close();
            }

            ve.VeriErisimKapat();
            return oVAN;
        }

        /// <summary>
        /// Gelir durum listesi.
        /// </summary>
        /// <param name="kriter">İşlem yapılacak bilgilerin taşındığı nesne</param>
        /// <returns></returns>
        public ObjectArray GelirDurumListesi(OdenekDurumKriter kriter)
        {
            ObjectArray oVAN = new ObjectArray();
            ObjectArray hVAN = new ObjectArray();
            ObjectArray gVAN = new ObjectArray();

            Tertip tb = new Tertip(DateTime.Now.Year, ENUMTabloKod.TABGelir);

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriBHM", TNS.KYM.Arac.KimlikBul(null));
            IDataReader or = null;

            //Gerçekleşme
            TABOdenekHarcamaSorgu harcama = new TABOdenekHarcamaSorgu(ve);
            or = harcama.GelirGerceklesenListe(kriter);

            if (harcama.Hata() == "")
            {
                while (or.Read())
                {
                    GelirDurumu o = new GelirDurumu();
                    int i = -1;
                    if (kriter.detay == ENUMOdenekDurumDetay.MUHASEBE || kriter.detay == ENUMOdenekDurumDetay.BIRIM)
                        o.muhasebe = VeriErisim.DegerAl(or, ++i, 0);
                    if (kriter.detay == ENUMOdenekDurumDetay.BIRIM)
                        o.birim = VeriErisim.DegerAl(or, ++i, "");

                    o.kurKod = OrtakFonksiyonlar.KodNoktaKoy((string)tb.Kirilimlar(0), VeriErisim.DegerAl(or, ++i, ""));
                    o.ekoKod = OrtakFonksiyonlar.KodNoktaKoy((string)tb.Kirilimlar(1), VeriErisim.DegerAl(or, ++i, ""));
                    o.gerceklesen = VeriErisim.DegerAl(or, ++i, 0.0);

                    hVAN.objeler.Add(o);
                }
            }
            or.Close();

            //Planlanan
            or = harcama.GelirListe(kriter);

            if (harcama.Hata() == "")
            {
                while (or.Read())
                {
                    GelirDurumu o = new GelirDurumu();
                    int i = -1;
                    o.kurKod = OrtakFonksiyonlar.KodNoktaKoy((string)tb.Kirilimlar(0), VeriErisim.DegerAl(or, ++i, ""));
                    o.ekoKod = OrtakFonksiyonlar.KodNoktaKoy((string)tb.Kirilimlar(1), VeriErisim.DegerAl(or, ++i, ""));
                    o.planlanan = VeriErisim.DegerAl(or, ++i, 0.0);

                    gVAN.objeler.Add(o);
                }
            }
            or.Close();

            int hSayac = 0;
            int gSayac = 0;
            bool cik = false;
            while (!cik)
            {
                GelirDurumu h = new GelirDurumu();
                GelirDurumu g = new GelirDurumu();
                GelirDurumu o = new GelirDurumu();

                if (hSayac < hVAN.objeler.Count)
                    h = (GelirDurumu)hVAN.objeler[hSayac];

                if (gSayac < gVAN.objeler.Count)
                    g = (GelirDurumu)gVAN.objeler[gSayac];

                if (g.ekoKod == "" && h.ekoKod == "")
                    break;

                if (g.ekoKod == "")
                    g.ekoKod = "9999999999";
                if (h.ekoKod == "")
                    h.ekoKod = "9999999999";

                if (hSayac < hVAN.objeler.Count || gSayac < gVAN.objeler.Count)
                {
                    if (h.ekoKod == g.ekoKod)
                    {
                        o.kurKod = h.kurKod;
                        o.ekoKod = h.ekoKod;
                        o.planlanan = g.planlanan;
                        o.gerceklesen = h.gerceklesen;
                        oVAN.objeler.Add(o);
                        gSayac++;
                        hSayac++;
                    }
                    else if (h.ekoKod.CompareTo(g.ekoKod) < 0 || (g.ekoKod == "" && h.ekoKod != ""))
                    {
                        o.kurKod = h.kurKod;
                        o.ekoKod = h.ekoKod;
                        o.gerceklesen = h.gerceklesen;
                        oVAN.objeler.Add(o);
                        hSayac++;
                    }
                    else if (h.ekoKod.CompareTo(g.ekoKod) > 0 || (h.ekoKod == "" && g.ekoKod != ""))
                    {
                        o.kurKod = g.kurKod;
                        o.ekoKod = g.ekoKod;
                        o.planlanan = g.planlanan;
                        oVAN.objeler.Add(o);
                        gSayac++;
                    }
                }
                else
                    break;
            }

            ve.VeriErisimKapat();
            return oVAN;
        }

        /// <summary>
        /// Muhasebattan gelen bilgilerden bütçe gideri fon eko liste.
        /// </summary>
        /// <param name="kriter">İşlem yapılacak bilgilerin taşındığı nesne</param>
        /// <returns></returns>
        public ObjectArray MuhasebatButceGideriFonEkoListe(OdenekDurumKriter kriter)
        {
            ObjectArray liste = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(null));

            TABOdenekHarcamaSorgu odenekHarcama = new TABOdenekHarcamaSorgu(ve);
            IDataReader or;

            ObjectArray yler = new MUHOdemeEmriMIFServis().KBSAktarimListele(null, kriter.yil);
            foreach (KBSAktarim kr in yler.objeler)
            {
                if (kr.goster > 0) continue;

                or = odenekHarcama.KBSAktarimDegerListele(kr, kriter.yil, kriter.ay, kriter.muhasebe, kriter.birim, kr.hesapKod, "", "", "", "", kriter.sorguTarih1, kriter.sorguTarih2, kriter.durum);
                while (or.Read())
                {
                    KBSAktarimDeger kbs = new KBSAktarimDeger();

                    kbs.hesapKod = kr.hesapKod;
                    int alanYer = 0;
                    kbs.hesap = VeriErisim.DegerAl(or, 0, "");
                    if (kr.fonUzunluk > 0) { alanYer++; kbs.fon = VeriErisim.DegerAl(or, alanYer, ""); }
                    if (kr.finUzunluk > 0) { alanYer++; kbs.fin = VeriErisim.DegerAl(or, alanYer, ""); }
                    alanYer++;
                    kbs.borc = VeriErisim.DegerAl(or, alanYer, 0.0);
                    alanYer++;
                    kbs.alacak = VeriErisim.DegerAl(or, alanYer, 0.0);

                    liste.objeler.Add(kbs);
                }
                or.Close();
            }

            ObjectArray dler = new MUHOdemeEmriMIFServis().KBSAktarimDetayListele(null, kriter.yil);
            foreach (KBSAktarim kr in dler.objeler)
            {
                foreach (KBSAktarimDeger deger in liste.objeler)
                {
                    string hesapKod1 = deger.hesap.Replace(".", "");
                    string hesapKod2 = kr.hesapKod.Replace(".", "");
                    string fonfin = deger.fon.Replace(".", "") + "*" + deger.fin;

                    if (hesapKod1.StartsWith(hesapKod2) && hesapKod1 != hesapKod2)
                    {
                        bool bulunamadi = true;
                        foreach (KBSAktarimDeger deger2 in liste.objeler)
                        {
                            string hesapKod3 = deger2.hesap.Replace(".", "");
                            string fonfin2 = deger2.fon.Replace(".", "") + "*" + deger2.fin;

                            if (hesapKod3 == hesapKod2 && fonfin == fonfin2)
                            {
                                deger2.borc += deger.borc;
                                deger2.alacak += deger.alacak;

                                deger.borc = 0;
                                deger.alacak = 0;
                                bulunamadi = false;
                                break;
                            }
                        }

                        if (bulunamadi)
                        {
                            deger.hesap = hesapKod2;
                        }
                    }
                }
            }

            ve.VeriErisimKapat();
            return liste;
        }

        /// <summary>
        /// Gelir Bütçesi rakam olustur.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="yil">Yıl</param>
        /// <param name="serbestMi">Eğer <c>true</c> ise serbest ödenek tutarını ödenek kadar oluştur</param>
        /// <returns></returns>
        public Sonuc ButceGelirRakamOlustur(Kullanici kullanan, int yil, bool serbestMi)
        {
            Sonuc sonuc = new Sonuc();

            if (!kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.SISTEMYONETICISI))
            {
                sonuc.hataStr = ButceMuhasebeResI.MUH177;
                sonuc.islemSonuc = false;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(kullanan));
            VeriErisim ve2 = VeriErisim.VeriErisimBul(null, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(kullanan));
            VeriErisim ve3 = VeriErisim.VeriErisimBul(null, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(kullanan));
            ve3.BeginTransaction();

            TABOdenekHarcamaSorgu odenekHarcama = new TABOdenekHarcamaSorgu(ve);
            TABOdenekHarcamaSorgu odenekHarcama2 = new TABOdenekHarcamaSorgu(ve2);
            TABOdenekHarcamaSorgu odenekHarcama3 = new TABOdenekHarcamaSorgu(ve2);

            string hata = "";

            //ABSBUTCEGELIR tablosunda eklenen düşülen alanları dolumu
            double deger = odenekHarcama.ButceGelirEklenenDusulenVarmi(yil);
            if (deger > 0)
                hata = string.Format(ButceMuhasebeResI.MUH178, yil) + "<br>";

            ////BUTCEHAZIRLIK yılın 5 ıncı aşamalarında rakam varmı
            //if (hata == "")
            //{
            //    deger = odenekHarcama.ButceGelirHazirlikAsamaVarmi(yil, 5);
            //    if (deger == 0)
            //        hata += string.Format(ButceMuhasebeResI.MUH179, yil) + "<br>";
            //}

            //BUTCEHAZIRLIK yılın 6 ıncı aşamalarında rakam varmı
            if (hata == "")
            {
                deger = odenekHarcama.ButceGelirHazirlikAsamaVarmi(yil, 6);
                if (deger == 0)
                    hata += string.Format(ButceMuhasebeResI.MUH180, yil) + "<br>";
            }

            //sil
            if (hata == "")
            {
                sonuc = odenekHarcama.ButceGelirBilgisiSil(yil);
                if (!sonuc.islemSonuc)
                    hata += sonuc.hataStr;
            }

            //kopyala ekokod2 düzeyinde
            if (hata == "")
            {
                IDataReader r = odenekHarcama.ButceGelirBilgisiOlustur(yil);

                if (odenekHarcama.Hata() == "")
                {
                    while (r.Read())
                    {
                        int aYil = VeriErisim.DegerAl(r, 0, 0);
                        string aKur = VeriErisim.DegerAl(r, 1, "");
                        string aEko = VeriErisim.DegerAl(r, 2, "");
                        double aKBO = VeriErisim.DegerAl(r, 3, 0.0);
                        double aKanun = aKBO;
                        double aSerbest = 0;

                        if (serbestMi)
                            aSerbest = aKBO;

                        sonuc = odenekHarcama3.ButceGelirBilgisiKaydet(yil, aKur, aEko, aKanun, aKBO, aSerbest);

                        if (!sonuc.islemSonuc)
                        {
                            hata += sonuc.hataStr;
                            break;
                        }
                    }
                    r.Close();
                }
                else
                    hata += odenekHarcama.Hata();
            }

            if (hata != "")
            {
                ve3.Rollback();
                sonuc.hataStr = hata;
                sonuc.islemSonuc = false;
            }
            else
                ve3.Commit();

            ve.VeriErisimKapat();
            ve2.VeriErisimKapat();
            ve3.VeriErisimKapat();

            return sonuc;
        }


        /// <summary>
        /// Bütçe gider rakam olustur.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="yil">Yıl</param>
        /// <param name="serbestMi">Eğer <c>true</c> ise serbest ödenek tutarını ödenek kadar oluştur</param>
        /// <returns></returns>
        public Sonuc ButceGiderRakamOlustur(Kullanici kullanan, int yil, bool serbestMi)
        {
            Sonuc sonuc = new Sonuc();

            if (!kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.SISTEMYONETICISI))
            {
                sonuc.hataStr = ButceMuhasebeResI.MUH177;
                sonuc.islemSonuc = false;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(kullanan));
            VeriErisim ve2 = VeriErisim.VeriErisimBul(null, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(kullanan));
            VeriErisim ve3 = VeriErisim.VeriErisimBul(null, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(kullanan));
            ve3.BeginTransaction();

            TABOdenekHarcamaSorgu odenekHarcama = new TABOdenekHarcamaSorgu(ve);
            TABOdenekHarcamaSorgu odenekHarcama2 = new TABOdenekHarcamaSorgu(ve2);
            TABOdenekHarcamaSorgu odenekHarcama3 = new TABOdenekHarcamaSorgu(ve2);

            string hata = "";

            //ABSBUTCEGIDER tablosunda eklenen düşülen alanları dolumu
            double deger = odenekHarcama.ButceGiderEklenenDusulenVarmi(yil);
            if (deger > 0)
                hata = string.Format(ButceMuhasebeResI.MUH181, yil) + "<br>";

            ////BUTCEHAZIRLIK yılın 5 ıncı aşamalarında rakam varmı
            //if (hata == "")
            //{
            //    deger = odenekHarcama.ButceGiderHazirlikAsamaVarmi(yil, 5);
            //    if (deger == 0)
            //        hata += string.Format(ButceMuhasebeResI.MUH182, yil) + "<br>";
            //}

            //BUTCEHAZIRLIK yılın 6 ıncı aşamalarında rakam varmı
            if (hata == "")
            {
                deger = odenekHarcama.ButceGiderHazirlikAsamaVarmi(yil, "0600");
                if (deger == 0)
                    hata += string.Format(ButceMuhasebeResI.MUH183, yil) + "<br>";
            }

            //sil
            if (hata == "")
            {
                sonuc = odenekHarcama.ButceGiderBilgisiSil(yil);
                if (!sonuc.islemSonuc)
                    hata += sonuc.hataStr;
            }

            //kopyala ekokod2 düzeyinde
            if (hata == "")
            {
                IDataReader r = odenekHarcama.ButceGiderBilgisiOlustur(yil);

                if (odenekHarcama.Hata() == "")
                {
                    while (r.Read())
                    {
                        int aYil = VeriErisim.DegerAl(r, 0, 0);
                        string aKur = VeriErisim.DegerAl(r, 1, "");
                        string aFon = VeriErisim.DegerAl(r, 2, "");
                        string aFin = VeriErisim.DegerAl(r, 3, "");
                        string aEko = VeriErisim.DegerAl(r, 4, "");
                        double aKBO = VeriErisim.DegerAl(r, 5, 0.0);
                        double aKanun = aKBO;// odenekHarcama2.ButceGiderHazirlikOdenekRakami(yil, 5, aKur, aFon, aFin, aEko);
                        double aSerbest = 0;

                        if (serbestMi)
                            aSerbest = aKBO;

                        sonuc = odenekHarcama3.ButceGiderBilgisiKaydet(yil, aKur, aFon, aFin, aEko, aKanun, aKBO, aSerbest);

                        if (!sonuc.islemSonuc)
                        {
                            hata += sonuc.hataStr;
                            break;
                        }
                    }
                    r.Close();
                }
                else
                    hata += odenekHarcama.Hata();
            }

            if (hata != "")
            {
                ve3.Rollback();
                sonuc.hataStr = hata;
                sonuc.islemSonuc = false;
            }
            else
                ve3.Commit();

            ve.VeriErisimKapat();
            ve2.VeriErisimKapat();
            ve3.VeriErisimKapat();

            return sonuc;
        }

        /// <summary>
        /// Harcamaların Aylık dağılımları.
        /// </summary>
        /// <param name="kriter">kriter</param>
        /// <returns></returns>
        public ObjectArray AylikDagilimHarcama(AylikDagilim kriter)
        {
            ObjectArray hListe = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(null));
            TABOdenekHarcamaSorgu harcama = new TABOdenekHarcamaSorgu(ve);
            IDataReader or = harcama.AylikDagilimHarcama(kriter);

            if (!string.IsNullOrEmpty(harcama.Hata()))
            {
                hListe.sonuc.islemSonuc = false;
                hListe.sonuc.hataStr = harcama.Hata();
                return hListe;
            }

            Tertip to = new Tertip(DateTime.Now.Year, ENUMTabloKod.TABOdenek);
            while (or.Read())
            {
                AylikDagilim o = new AylikDagilim();

                o.ay = VeriErisim.DegerAl(or, 0, 0);
                o.muhasebe = VeriErisim.DegerAl(or, 1, 0);
                o.birim = VeriErisim.DegerAl(or, 2, "");

                o.tertip.kodlar[0] = OrtakFonksiyonlar.KodNoktaKoy((string)to.Kirilimlar(0), VeriErisim.DegerAl(or, 3, ""));
                o.tertip.kodlar[1] = OrtakFonksiyonlar.KodNoktaKoy((string)to.Kirilimlar(1), VeriErisim.DegerAl(or, 4, ""));
                o.tertip.kodlar[2] = OrtakFonksiyonlar.KodNoktaKoy((string)to.Kirilimlar(2), VeriErisim.DegerAl(or, 5, ""));
                o.tertip.kodlar[3] = OrtakFonksiyonlar.KodNoktaKoy((string)to.Kirilimlar(3), VeriErisim.DegerAl(or, 6, ""));

                o.tutar = VeriErisim.DegerAl(or, 7, 0.0);

                hListe.objeler.Add(o);
            }
            or.Close();

            if (hListe.objeler.Count == 0)
                hListe.sonuc.bilgiStr = ButceMuhasebeResI.MUH051;

            ve.VeriErisimKapat();
            return hListe;
        }

        /// <summary>
        /// Ödenek gönderme  Aylık dağılımları.
        /// </summary>
        /// <param name="kriter">kriter</param>
        /// <returns></returns>
        public ObjectArray AylikDagilimOdenekGonderme(AylikDagilim kriter)
        {
            ObjectArray hListe = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(null));
            TABOdenekHarcamaSorgu harcama = new TABOdenekHarcamaSorgu(ve);
            IDataReader or = harcama.AylikDagilimOdenekGonderme(kriter);

            if (!string.IsNullOrEmpty(harcama.Hata()))
            {
                hListe.sonuc.islemSonuc = false;
                hListe.sonuc.hataStr = harcama.Hata();
                return hListe;
            }

            while (or.Read())
            {
                AylikDagilim o = new AylikDagilim();

                o.ay = VeriErisim.DegerAl(or, 0, 0);
                o.muhasebe = VeriErisim.DegerAl(or, 1, 0);
                o.birim = VeriErisim.DegerAl(or, 2, "");

                o.tertip = new Tertip(kriter.yil, ENUMTabloKod.TABButce);
                o.tertip.kodlar[0] = OrtakFonksiyonlar.KodNoktaKoy((string)o.tertip.Kirilimlar(0), VeriErisim.DegerAl(or, 3, ""));
                o.tertip.kodlar[1] = OrtakFonksiyonlar.KodNoktaKoy((string)o.tertip.Kirilimlar(1), VeriErisim.DegerAl(or, 4, ""));
                o.tertip.kodlar[2] = OrtakFonksiyonlar.KodNoktaKoy((string)o.tertip.Kirilimlar(2), VeriErisim.DegerAl(or, 5, ""));
                o.tertip.kodlar[3] = OrtakFonksiyonlar.KodNoktaKoy((string)o.tertip.Kirilimlar(3), VeriErisim.DegerAl(or, 6, ""));

                o.tutar = VeriErisim.DegerAl(or, 7, 0.0);

                hListe.objeler.Add(o);
            }
            or.Close();

            if (hListe.objeler.Count == 0)
                hListe.sonuc.bilgiStr = ButceMuhasebeResI.MUH051;

            ve.VeriErisimKapat();
            return hListe;
        }
    }
}