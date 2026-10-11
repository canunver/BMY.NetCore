using System;
using System.Data;
using TNS.KYM;
using TNS.Veri;
using TNS.PEB.IsKurallari.Veri;

namespace TNS.PEB.IsKurallari
{
    /// <summary>
    /// Sabit maliyetlerin faaliyet/projelerle bağını kuran ağırlık bilgileriyle ilgili,
    /// iş kuralları kontrollerini yapıp, veri tabanı bağlantısını alıp, veri tabanı fonksiyonlarını
    /// çağırarak okuma, yazma işlemlerini gerçekleştiren fonksiyonları tutan sınıf
    /// </summary>
    public class PEBSabitMaliyetAgirlik
    {
        /// <summary>
        /// Parametre olarak verilen sabit maliyet ağırlık bilgilerinin doğruluğunu kontrol eden yordam
        /// </summary>
        /// <param name="agirlik">Sabit maliyet ağırlık bilgileri listesini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        private static Sonuc AgirlikKontrol(TNSCollection agirlik)
        {
            Sonuc sonuc = new Sonuc();

            double toplam = 0;
            foreach (SabitMaliyetAgirlik sms in agirlik)
            {
                if (sms.agirlik < 0)
                {
                    sonuc.islemSonuc = false;
                    sonuc.hataStr = PerformansButceResI.PEB042;
                    return sonuc;
                }
                toplam += sms.agirlik;
            }

            if (Math.Round(toplam, 4) != 100)
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = string.Format(PerformansButceResI.PEB043, toplam.ToString());
            }
            return sonuc;
        }

        /// <summary>
        /// Sabit maliyet ağırlık bilgilerini alarak sabit maliyetin ödeneğini bu ağırlıkları
        /// kullanarak ilgili faaliyet/projelere yansıyan ödenekleri hesaplayıp kaydeden yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="smAgirlik">Sabit maliyet ağırlık bilgileri listesini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public static Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, TNSCollection smAgirlik)
        {
            Sonuc sonuc = new Sonuc();

            SabitMaliyetAgirlik sma = null;

            if (smAgirlik.Count > 0)
            {
                sma = (SabitMaliyetAgirlik)smAgirlik[0];
            }
            else
            {
                sonuc.DegerAta(PerformansButceResI.PEB106);
                return sonuc;
            }
            string hata = string.Empty;
            if (string.IsNullOrEmpty(sma.smKurKod))
                hata += PerformansButceResI.PEB005 + "<br>";
            else if (!PEBGenel.KurKodVarMi(sma.yil, sma.smKurKod))
                hata += PerformansButceResI.PEB006 + "<br>";
            if (sma.smKod <= 0)
                hata += PerformansButceResI.PEB105 + "<br>";

            if (!string.IsNullOrEmpty(hata))
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = hata;
                return sonuc;
            }

            sonuc = AgirlikKontrol(smAgirlik);
            if (!sonuc.islemSonuc)
                return sonuc;

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriPEB", TNS.KYM.Arac.KimlikBul(kullanan));

            sonuc = Yetki.YazmaHakkiVarMi(kullanan, sma.smKurKod);
            if (sonuc.islemSonuc)
            {
                if (!PEBGenel.SabitMaliyetVarMi(ve, sma.yil, sma.smKod))
                {
                    sonuc.islemSonuc = false;
                    sonuc.hataStr = PerformansButceResI.PEB104 + "<br>";
                }
            }

            TABSabitMaliyetAgirlik tAgirlik = new TABSabitMaliyetAgirlik(ve);

            foreach (SabitMaliyetAgirlik sms in smAgirlik)
            {
                if (!PEBGenel.FaaliyetProjeVarMi(ve, sms.yil, sms.fpKod))
                {
                    sonuc.islemSonuc = false;
                    sonuc.hataStr = string.Format(PerformansButceResI.PEB107, sms.fpKod.ToString());
                    break;
                }
            }
            
            if (!sonuc.islemSonuc)
            {
                ve.VeriErisimKapat();
                return sonuc;
            }
            TABOdenek tOdenek = new TABOdenek(ve);
            double smToplam = tOdenek.SabitMaliyetToplami(sma.yil, sma.smKurKod, sma.senaryoKod, sma.smKod);

            ve.BeginTransaction();

            if (sonuc.islemSonuc)
                sonuc = tAgirlik.Sil(sma);

            if (sonuc.islemSonuc)
            {
                int sayac = 0;
                double toplam = 0;
                foreach (SabitMaliyetAgirlik sms in smAgirlik)
                {
                    sayac++;
                    if (sms.agirlik > 0)
                    {
                        if (sayac == smAgirlik.Count)
                            sms.odenek = smToplam - toplam;
                        else
                            sms.odenek = Math.Round(smToplam * sms.agirlik / 100);

                        toplam += sms.odenek;

                        sonuc = tAgirlik.Kaydet(sms);
                        if (!sonuc.islemSonuc)
                            break;
                    }
                }
            }

            if (sonuc.islemSonuc)
            {
                sonuc.bilgiStr = PerformansButceResI.PEB015;
                ve.Commit();
            }
            else
            {
                sonuc.hataStr = PerformansButceResI.PEB016 + "<br>" + sonuc.hataStr;
                ve.Rollback();
            }

            ve.VeriErisimKapat();
            return sonuc;
        }

        /// <summary>
        /// Verilen kriterlere uygun olan sabit maliyet ağırlık ve yansıyan ödenek bilgilerini döndüren yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="smAgirlik">Sabit maliyet ağırlık bilgilerini tutan nesne</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında sabit maliyet ağırlık ve yansıyan ödenek listesi döndürülür.</returns>
        public static ObjectArray Listele(VeriErisim veriErisim, Kullanici kullanan, SabitMaliyetAgirlik smAgirlik)
        {
            ObjectArray smAgirlikVAN = new ObjectArray();
            smAgirlikVAN.sonuc = Yetki.OkumaHakkiVarMi(kullanan, smAgirlik.smKurKod);
            if (!smAgirlikVAN.sonuc.islemSonuc)
                return smAgirlikVAN;

            //string hata = string.Empty;
            //if (string.IsNullOrEmpty(smAgirlik.smKurKod))
            //    hata += "Birim boş bırakılamaz.<br>";
            //if (smAgirlik.smKod <= 0)
            //    hata += "Sabit Maliyet boş bırakılamaz.<br>";
            //if (!string.IsNullOrEmpty(hata))
            //{
            //    smAgirlikVAN.sonuc.islemSonuc = false;
            //    smAgirlikVAN.sonuc.hataStr = hata;
            //    return smAgirlikVAN;
            //}

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriPEB", TNS.KYM.Arac.KimlikBul(kullanan));
            TABSabitMaliyetAgirlik tAgirlik = new TABSabitMaliyetAgirlik(ve);

            IDataReader dr = tAgirlik.Listele(smAgirlik);
            while (dr.Read())
            {
                SabitMaliyetAgirlik gelen = new SabitMaliyetAgirlik();
                gelen.yil = VeriErisim.DegerAl(dr, 0, 0);
                gelen.smKurKod = VeriErisim.DegerAl(dr, 1, string.Empty);
                gelen.smKurKod = PEBGenel.KurKodNoktala(gelen.smKurKod);
                gelen.smKod = VeriErisim.DegerAl(dr, 2, 0);
                gelen.fpKurKod = VeriErisim.DegerAl(dr, 3, string.Empty);
                gelen.fpKurKod = PEBGenel.KurKodNoktala(gelen.fpKurKod);
                gelen.fpKurAd = PEBServis.UzayYaziDegerGetir("ABSKUR", smAgirlik.yil.ToString() + "-" + gelen.fpKurKod, true, "");
                gelen.fpKod = VeriErisim.DegerAl(dr, 4, 0);
                gelen.fpAd = PEBServis.UzayYaziDegerGetir("PEBFAALIYETPROJE", smAgirlik.yil.ToString() + "-" + gelen.fpKurKod + "-" + gelen.fpKod.ToString(), true, "");
                gelen.agirlik = VeriErisim.DegerAl(dr, 5, (double)0);
                gelen.odenek = VeriErisim.DegerAl(dr, 6, (double)0);
                gelen.senaryoKod = VeriErisim.DegerAl(dr, 7, string.Empty);
                gelen.smAd = VeriErisim.DegerAl(dr, 8, string.Empty);
                gelen.harcama = VeriErisim.DegerAl(dr, 9, (double)0);

                smAgirlikVAN.objeler.Add(gelen);
            }
            dr.Close();

            if (smAgirlikVAN.objeler.Count == 0)
                smAgirlikVAN.sonuc.bilgiStr = PerformansButceResI.PEB017;

            ve.VeriErisimKapat();
            return smAgirlikVAN;
        }

        /// <summary>
        /// Verilen kriterlere uygun olan faaliyet/projeleri bulan ve ödeneklerinden ağırlıklarını hesaplıp döndüren yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="smAgirlik">Sabit maliyet ağırlık bilgilerini tutan nesne</param>
        /// <param name="tur">ENUMSabitMaliyetTur listesindeki türlerden biri olmalıdır.</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında sabit maliyet ağırlık listesi döndürülür.</returns>
        public static ObjectArray AgirlikHesapla(VeriErisim veriErisim, Kullanici kullanan, SabitMaliyetAgirlik smAgirlik, int tur)
        {
            ObjectArray smAgirlikVAN = new ObjectArray();
            smAgirlikVAN.sonuc = Yetki.OkumaHakkiVarMi(kullanan, smAgirlik.smKurKod);
            if (!smAgirlikVAN.sonuc.islemSonuc)
                return smAgirlikVAN;

            string hata = string.Empty;
            if (string.IsNullOrEmpty(smAgirlik.smKurKod))
                hata += PerformansButceResI.PEB005 + "<br>";
            if (tur <= 0)
                hata += PerformansButceResI.PEB102 + "<br>";
            if (!string.IsNullOrEmpty(hata))
            {
                smAgirlikVAN.sonuc.islemSonuc = false;
                smAgirlikVAN.sonuc.hataStr = hata;
                return smAgirlikVAN;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriPEB", TNS.KYM.Arac.KimlikBul(kullanan));
            TABOdenek tOdenek = new TABOdenek(ve);

            double faaliyetlerToplami = tOdenek.YansitilacakFaaliyetlerToplami(smAgirlik.yil, smAgirlik.smKurKod, smAgirlik.senaryoKod, tur);

            IDataReader dr = tOdenek.YansitilacakFaaliyetler(smAgirlik.yil, smAgirlik.smKurKod, smAgirlik.senaryoKod, tur);
            double yuzdeTopla = 0;
            SabitMaliyetAgirlik enBuyuk = null;

            while (dr.Read())
            {
                SabitMaliyetAgirlik gelen = new SabitMaliyetAgirlik();
                gelen.yil = VeriErisim.DegerAl(dr, 0, 0);
                gelen.fpKurKod = VeriErisim.DegerAl(dr, 1, string.Empty);
                gelen.fpKurKod = PEBGenel.KurKodNoktala(gelen.fpKurKod);
                gelen.fpKurAd = PEBServis.UzayYaziDegerGetir("ABSKUR", gelen.yil.ToString() + "-" + gelen.fpKurKod, true, "");
                gelen.fpKod = VeriErisim.DegerAl(dr, 2, 0);
                gelen.fpAd = VeriErisim.DegerAl(dr, 3, string.Empty);
                gelen.agirlik = Math.Round(VeriErisim.DegerAl(dr, 4, (double)0) / faaliyetlerToplami, 6) * 100;
                if (enBuyuk == null) enBuyuk = gelen;
                if (enBuyuk.agirlik < gelen.agirlik) enBuyuk = gelen;
                yuzdeTopla += gelen.agirlik;
                smAgirlikVAN.objeler.Add(gelen);
            }
            dr.Close();

            if (enBuyuk != null)
                enBuyuk.agirlik += (100 - yuzdeTopla);
            if (smAgirlikVAN.objeler.Count == 0)
                smAgirlikVAN.sonuc.bilgiStr = PerformansButceResI.PEB017;

            ve.VeriErisimKapat();
            return smAgirlikVAN;
        }
    }
}