using System;
using System.Data;
using TNS.KYM;
using TNS.Veri;
using TNS.BKD;
using TNS.PEB.IsKurallari.Veri;
using TNS.MUH;

namespace TNS.PEB.IsKurallari
{
    public class PEBHarcama
    {
        public static Sonuc HarcamaHesaplaKaydet(VeriErisim veriErisim, Kullanici kullanan, Harcama hk)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriPEB", TNS.KYM.Arac.KimlikBul(kullanan));

            ObjectArray agirliklar = AgirlikHesapla(ve, kullanan, hk);
            if (!agirliklar.sonuc.islemSonuc || agirliklar.objeler.Count <= 0)
            {
                ve.VeriErisimKapat();
                return agirliklar.sonuc;
            }

            agirliklar.sonuc = Kaydet(ve, kullanan, hk, agirliklar.objeler);
            if (!agirliklar.sonuc.islemSonuc)
            {
                ve.VeriErisimKapat();
                return agirliklar.sonuc;
            }

            agirliklar.sonuc = PEBFaaliyetProjeHarcamaYansiyan.Kaydet(ve, kullanan, hk.yil, hk.ay);
            if (agirliklar.sonuc.islemSonuc)
                agirliklar.sonuc.bilgiStr = "İşlem başarıyla tamamlandı.";

            ve.VeriErisimKapat();
            return agirliklar.sonuc;
        }

        public static ObjectArray AgirlikHesapla(VeriErisim veriErisim, Kullanici kullanan, Harcama hk)
        {
            ObjectArray agirliklar = new ObjectArray();
            if (!Yetki.YoneticiMi(kullanan))
            {
                agirliklar.sonuc.islemSonuc = false;
                agirliklar.sonuc.hataStr = "Bu işlemi yapmaya yetkiniz yok!";
                return agirliklar;
            }

            string hata = string.Empty;
            if (hk.yil <= 0)
                hata = "Yıl boş bırakılamaz.<br>";
            if (hk.ay <= 0)
                hata += "Ay boş bırakılamaz.<br>";
            if (!string.IsNullOrEmpty(hata))
            {
                agirliklar.sonuc.islemSonuc = false;
                agirliklar.sonuc.hataStr = hata;
                return agirliklar;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriPEB", TNS.KYM.Arac.KimlikBul(kullanan));
            TABOdenek tOdenek = new TABOdenek(ve);

            TNSCollection tertipOdenekToplam = TertipOdenekToplamVer(tOdenek, hk.yil, "0600", hk.tertip);
            TNSCollection tertipOdenekDetay = TertipOdenekDetayVer(tOdenek, hk.yil, "0600", hk.tertip);

            //Bir tertip kaç defa kullanılmış onu hesaplayıp
            //Hashtable'a atıyor
            System.Collections.Hashtable sayac = new System.Collections.Hashtable();
            foreach (Odenek o in tertipOdenekDetay)
            {
                if (sayac.ContainsKey(o.tertip.TertipStrOl()))
                    sayac[o.tertip.TertipStrOl()] = (int)sayac[o.tertip.TertipStrOl()] + 1;
                else
                    sayac.Add(o.tertip.TertipStrOl(), 1);
            }

            foreach (Odenek o in tertipOdenekDetay)
            {
                foreach (TertipOdenek to in tertipOdenekToplam)
                {
                    if (o.tertip.TertipStrOl() == to.tertip.TertipStrOl())
                    {
                        Harcama h = new Harcama();
                        h.yil = o.yil;
                        h.ay = hk.ay;
                        h.maliyetKurKod = o.maliyetKurKod;
                        h.maliyetKod = o.maliyetKod;
                        h.sabitMi = o.sabitMi;
                        h.tertip = o.tertip;
                        h.maliyetFonKod = o.maliyetFonKod;

                        //İlgili tertip için eklenen sayacı ve ağırlığı buluyor
                        //Bu sayede bir tertip için ağırlık toplamı 100'ü geçmeyecek
                        int sayacEklenen = 0;
                        double agirlikEklenen = 0;
                        foreach (Harcama ha in agirliklar.objeler)
                        {
                            if (h.tertip.TertipStrOl() == ha.tertip.TertipStrOl())
                            {
                                sayacEklenen++;
                                agirlikEklenen += ha.agirlik;
                            }
                        }

                        if ((int)sayac[h.tertip.TertipStrOl()] == sayacEklenen + 1)
                            h.agirlik = 100 - agirlikEklenen;
                        else
                            h.agirlik = (o.odenek / to.odenek) * 100;

                        agirliklar.objeler.Add(h);
                        break;
                    }
                }
            }

            if (agirliklar.objeler.Count == 0)
                agirliklar.sonuc.bilgiStr = "Verilen kriterlere uygun kayıt bulunamadı.";

            ve.VeriErisimKapat();
            return agirliklar;
        }

        private static TNSCollection TertipOdenekToplamVer(TABOdenek tOdenek, int yil, string senaryoKod, Tertip tertip)
        {
            TNSCollection tertipOdenekToplam = new TNSCollection();

            IDataReader dr = tOdenek.TertipToplamlari(yil, senaryoKod, tertip);

            while (dr.Read())
            {
                TertipOdenek gelen = new TertipOdenek(yil);
                gelen.tertip.kodlar[0] = VeriErisim.DegerAl(dr, 0, string.Empty);
                gelen.tertip.kodlar[1] = VeriErisim.DegerAl(dr, 1, string.Empty);
                gelen.tertip.kodlar[2] = VeriErisim.DegerAl(dr, 2, string.Empty);
                gelen.tertip.kodlar[3] = VeriErisim.DegerAl(dr, 3, string.Empty);
                gelen.odenek = VeriErisim.DegerAl(dr, 4, (double)0);

                gelen.tertip.Noktala();

                tertipOdenekToplam.Add(gelen);
            }
            dr.Close();

            return tertipOdenekToplam;
        }

        private static TNSCollection TertipOdenekDetayVer(TABOdenek tOdenek, int yil, string senaryoKod, Tertip tertip)
        {
            TNSCollection tertipOdenekDetay = new TNSCollection();

            IDataReader dr = tOdenek.Listele(yil, senaryoKod, tertip);

            while (dr.Read())
            {
                Odenek gelen = new Odenek();
                gelen.yil = VeriErisim.DegerAl(dr, 0, 0);
                gelen.maliyetKurKod = VeriErisim.DegerAl(dr, 1, string.Empty);
                gelen.maliyetKurKod = PEBGenel.KurKodNoktala(gelen.maliyetKurKod);
                gelen.senaryoKod = VeriErisim.DegerAl(dr, 2, string.Empty);
                gelen.maliyetKod = VeriErisim.DegerAl(dr, 3, 0);
                gelen.sabitMi = VeriErisim.DegerAl(dr, 4, -1);
                gelen.tertip.kodlar[0] = VeriErisim.DegerAl(dr, 5, string.Empty);
                gelen.tertip.kodlar[1] = VeriErisim.DegerAl(dr, 6, string.Empty);
                gelen.tertip.kodlar[2] = VeriErisim.DegerAl(dr, 7, string.Empty);
                gelen.tertip.kodlar[3] = VeriErisim.DegerAl(dr, 8, string.Empty);
                gelen.odenek = VeriErisim.DegerAl(dr, 9, (double)0);
                gelen.maliyetFonKod = VeriErisim.DegerAl(dr, 10, string.Empty);
                gelen.maliyetFonKod = PEBGenel.FonKodNoktala(gelen.maliyetFonKod);

                gelen.tertip.Noktala();

                tertipOdenekDetay.Add(gelen);
            }
            dr.Close();

            return tertipOdenekDetay;
        }

        //private static Sonuc AgirlikKontrol(TNSCollection agirlik)
        //{
        //    Sonuc sonuc = new Sonuc();

        //    double toplam = 0;
        //    foreach (SabitMaliyetAgirlik sms in agirlik)
        //    {
        //        if (sms.agirlik < 0)
        //        {
        //            sonuc.islemSonuc = false;
        //            sonuc.hataStr = "Girilen Ağırlıklarda sıfırdan küçük değer var. Lütfen kontrol ediniz.";
        //            return sonuc;
        //        }
        //        toplam += sms.agirlik;
        //    }

        //    if (Math.Round(toplam, 4) != 100)
        //    {
        //        sonuc.islemSonuc = false;
        //        sonuc.hataStr = "Girilen Ağırlıkların toplamı 100 olmalı. Lütfen kontrol ediniz. Şu anki Toplam = " + toplam.ToString();
        //    }
        //    return sonuc;
        //}

        public static Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, Harcama hk, TNSCollection agirliklar)
        {
            Sonuc sonuc = new Sonuc();
            if (!Yetki.YoneticiMi(kullanan))
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = "Bu işlemi yapmaya yetkiniz yok!";
                return sonuc;
            }

            string hata = string.Empty;
            if (hk.yil <= 0)
                hata = "Yıl boş bırakılamaz.<br>";
            if (hk.ay <= 0)
                hata += "Ay boş bırakılamaz.<br>";
            if (agirliklar.Count <= 0)
                hata += "Ağırlıklar boş bırakılamaz.<br>";
            if (!string.IsNullOrEmpty(hata))
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = hata;
                return sonuc;
            }

            //sonuc = AgirlikKontrol(smAgirlik);
            //if (!sonuc.islemSonuc)
            //    return sonuc;

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriPEB", TNS.KYM.Arac.KimlikBul(kullanan));
            TABHarcama tHarcama = new TABHarcama(ve);
            ve.BeginTransaction();

            //Bütçe Muhasebeden harcamalar alınıyor
            OdenekDurumKriter kriter = new OdenekDurumKriter();
            kriter.yil = hk.yil;
            kriter.ay = hk.ay;
            kriter.tertip = hk.tertip;
            kriter.ekoKodKirilim = 4;
            ObjectArray harcamalar = TNS.MUH.Arac.Tanimla().HarcamaListele(kriter);

            if (!harcamalar.sonuc.islemSonuc || harcamalar.objeler.Count <= 0)
            {
                sonuc = harcamalar.sonuc;
                ve.VeriErisimKapat();
                return sonuc;
            }

            //Bir tertip kaç defa kullanılmış onu hesaplayıp
            //Hashtable'a atıyor
            System.Collections.Hashtable sayac = new System.Collections.Hashtable();
            foreach (Harcama h in agirliklar)
            {
                if (sayac.ContainsKey(h.tertip.TertipStrOl()))
                    sayac[h.tertip.TertipStrOl()] = (int)sayac[h.tertip.TertipStrOl()] + 1;
                else
                    sayac.Add(h.tertip.TertipStrOl(), 1);
            }

            //Eski kayıtlar siliniyor
            tHarcama.Sil(hk);

            System.Collections.Hashtable harcamaKayitli = new System.Collections.Hashtable();
            System.Collections.Hashtable sayacKayitli = new System.Collections.Hashtable();
            foreach (Harcama h in agirliklar)
            {
                if (h.agirlik > 0)
                {
                    foreach (HarcamaDurum hd in harcamalar.objeler)
                    {
                        if (h.tertip.TertipStrOl() == hd.tertip.TertipStrOl())
                        {
                            int sayacKayitEdilenen = 0;
                            if (sayacKayitli.ContainsKey(h.tertip.TertipStrOl()))
                                sayacKayitEdilenen = (int)sayacKayitli[h.tertip.TertipStrOl()];

                            double harcamaKayitEdilenen = 0;
                            if (harcamaKayitli.ContainsKey(h.tertip.TertipStrOl()))
                                harcamaKayitEdilenen = (double)harcamaKayitli[h.tertip.TertipStrOl()];

                            if ((int)sayac[h.tertip.TertipStrOl()] == sayacKayitEdilenen + 1)
                                h.harcama = hd.harcama - harcamaKayitEdilenen;
                            else
                                h.harcama = Math.Round(hd.harcama * h.agirlik / 100);

                            sonuc = tHarcama.Kaydet(h);
                            if (!sonuc.islemSonuc)
                                break;

                            if (sayacKayitli.ContainsKey(h.tertip.TertipStrOl()))
                                sayacKayitli[h.tertip.TertipStrOl()] = (int)sayacKayitli[h.tertip.TertipStrOl()] + 1;
                            else
                                sayacKayitli.Add(h.tertip.TertipStrOl(), 1);

                            if (harcamaKayitli.ContainsKey(h.tertip.TertipStrOl()))
                                harcamaKayitli[h.tertip.TertipStrOl()] = (double)harcamaKayitli[h.tertip.TertipStrOl()] + h.harcama;
                            else
                                harcamaKayitli.Add(h.tertip.TertipStrOl(), h.harcama);

                            break;
                        }
                    }

                    if (!sonuc.islemSonuc)
                        break;
                }
            }

            if (sonuc.islemSonuc)
            {
                sonuc.bilgiStr = "Kayıt işlemi başarıyla gerçekleştirildi.";
                ve.Commit();
            }
            else
            {
                sonuc.hataStr = "Kayıt işlemi gerçekleştirilemedi.<br>" + sonuc.hataStr;
                ve.Rollback();
            }

            ve.VeriErisimKapat();
            return sonuc;
        }

        public static ObjectArray Listele(VeriErisim veriErisim, Kullanici kullanan, Harcama hk)
        {
            ObjectArray harcamalar = new ObjectArray();
            if (!Yetki.YoneticiMi(kullanan))
            {
                harcamalar.sonuc.islemSonuc = false;
                harcamalar.sonuc.hataStr = "Bu işlemi yapmaya yetkiniz yok!";
                return harcamalar;
            }

            string hata = string.Empty;
            if (hk.yil <= 0)
                hata = "Yıl boş bırakılamaz.<br>";
            if (hk.ay <= 0)
                hata += "Ay boş bırakılamaz.<br>";
            if (!string.IsNullOrEmpty(hata))
            {
                harcamalar.sonuc.islemSonuc = false;
                harcamalar.sonuc.hataStr = hata;
                return harcamalar;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriPEB", TNS.KYM.Arac.KimlikBul(kullanan));
            TABHarcama tHarcama = new TABHarcama(ve);

            IDataReader dr = tHarcama.Listele(hk);
            while (dr.Read())
            {
                Harcama gelen = new Harcama();
                gelen.yil = VeriErisim.DegerAl(dr, 0, 0);
                gelen.ay = VeriErisim.DegerAl(dr, 1, 0);
                gelen.maliyetKurKod = VeriErisim.DegerAl(dr, 2, string.Empty);
                gelen.maliyetKurKod = PEBGenel.KurKodNoktala(gelen.maliyetKurKod);
                gelen.maliyetKod = VeriErisim.DegerAl(dr, 3, 0);
                gelen.sabitMi = VeriErisim.DegerAl(dr, 4, -1);

                gelen.tertip.kodlar[0] = VeriErisim.DegerAl(dr, 5, string.Empty);
                gelen.tertip.kodlar[1] = VeriErisim.DegerAl(dr, 6, string.Empty);
                gelen.tertip.kodlar[2] = VeriErisim.DegerAl(dr, 7, string.Empty);
                gelen.tertip.kodlar[3] = VeriErisim.DegerAl(dr, 8, string.Empty);
                gelen.tertip.Noktala();

                gelen.maliyetFonKod = VeriErisim.DegerAl(dr, 9, string.Empty);
                gelen.maliyetFonKod = PEBGenel.FonKodNoktala(gelen.maliyetFonKod);

                gelen.agirlik = VeriErisim.DegerAl(dr, 10, (double)0);
                gelen.harcama = VeriErisim.DegerAl(dr, 11, (double)0);

                harcamalar.objeler.Add(gelen);
            }
            dr.Close();

            if (harcamalar.objeler.Count == 0)
                harcamalar.sonuc.bilgiStr = "Verilen kriterlere uygun kayıt bulunamadı.";

            ve.VeriErisimKapat();
            return harcamalar;
        }

        public static ObjectArray SabitMaliyetHarcamaToplamlari(VeriErisim veriErisim, Kullanici kullanan, int yil, int ay)
        {
            Harcama h = new Harcama();
            h.yil = yil;
            h.ay = ay;
            h.sabitMi = (int)ENUMOdenekTur.SabitMaliyet;
            return HarcamaToplamlari(veriErisim, kullanan, h);
        }

        public static ObjectArray FaaliyetProjeHarcamaToplamlari(VeriErisim veriErisim, Kullanici kullanan, int yil, int ay)
        {
            Harcama h = new Harcama();
            h.yil = yil;
            h.ay = ay;
            h.sabitMi = (int)ENUMOdenekTur.FaaliyetProje;
            return HarcamaToplamlari(veriErisim, kullanan, h);
        }

        public static ObjectArray HarcamaToplamlari(VeriErisim veriErisim, Kullanici kullanan, Harcama hk)
        {
            ObjectArray harcamalar = new ObjectArray();
            if (!Yetki.YoneticiMi(kullanan))
            {
                harcamalar.sonuc.islemSonuc = false;
                harcamalar.sonuc.hataStr = "Bu işlemi yapmaya yetkiniz yok!";
                return harcamalar;
            }

            string hata = string.Empty;
            if (hk.yil <= 0)
                hata = "Yıl boş bırakılamaz.<br>";
            if (hk.ay <= 0)
                hata += "Ay boş bırakılamaz.<br>";
            if (!string.IsNullOrEmpty(hata))
            {
                harcamalar.sonuc.islemSonuc = false;
                harcamalar.sonuc.hataStr = hata;
                return harcamalar;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriPEB", TNS.KYM.Arac.KimlikBul(kullanan));
            TABHarcama tHarcama = new TABHarcama(ve);

            IDataReader dr = tHarcama.HarcamaToplamlari(hk);
            while (dr.Read())
            {
                Harcama gelen = new Harcama();
                gelen.maliyetKurKod = VeriErisim.DegerAl(dr, 0, string.Empty);
                gelen.maliyetKurKod = PEBGenel.KurKodNoktala(gelen.maliyetKurKod);
                gelen.maliyetKod = VeriErisim.DegerAl(dr, 1, 0);
                gelen.sabitMi = VeriErisim.DegerAl(dr, 2, -1);
                gelen.harcama = VeriErisim.DegerAl(dr, 3, (double)0);

                harcamalar.objeler.Add(gelen);
            }
            dr.Close();

            if (harcamalar.objeler.Count == 0)
                harcamalar.sonuc.bilgiStr = "Verilen kriterlere uygun kayıt bulunamadı.";

            ve.VeriErisimKapat();
            return harcamalar;
        }
    }
}