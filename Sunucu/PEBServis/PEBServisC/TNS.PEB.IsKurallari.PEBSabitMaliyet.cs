using System;
using System.Data;
using TNS.KYM;
using TNS.Veri;
using TNS.PEB.IsKurallari.Veri;

namespace TNS.PEB.IsKurallari
{
    /// <summary>
    /// Sabit maliyet bilgileriyle ilgili, iş kuralları kontrollerini yapıp,
    /// veri tabanı bağlantısını alıp, veri tabanı fonksiyonlarını çağırarak
    /// okuma, yazma işlemlerini gerçekleştiren fonksiyonları tutan sınıf
    /// </summary>
    public class PEBSabitMaliyet
    {
        /// <summary>
        /// Sabit maliyet varsa güncelleyen, yoksa kaydeden yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen SabitMaliyet nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// Sabit maliyetin bütçe hazırlık alt faaliyetlerle bağının da ilgili yordamı çağırarak kaydedilmesini sağlar.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="sm">Sabit maliyet bilgisini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public static Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, SabitMaliyet sm)
        {
            Sonuc sonuc = Yetki.YazmaHakkiVarMi(kullanan, sm.kurKod);
            if (!sonuc.islemSonuc)
                return sonuc;

            string hata = string.Empty;
            if (string.IsNullOrEmpty(sm.kurKod))
                hata += PerformansButceResI.PEB005 + "<br>";
            else if (!PEBGenel.KurKodVarMi(sm.yil, sm.kurKod))
                hata += PerformansButceResI.PEB006 + "<br>";
            if (string.IsNullOrEmpty(sm.smAd))
                hata += PerformansButceResI.PEB101 + "<br>";
            if (sm.tur <= 0)
                hata += PerformansButceResI.PEB102 + "<br>";
            //if (sm.smOdenek.Count <= 0)
            //    hata += "Sabit Maliyet Ödenek bilgileri boş bırakılamaz.<br>";
            if (!string.IsNullOrEmpty(hata))
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = hata;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriPEB", TNS.KYM.Arac.KimlikBul(kullanan));
            TABSabitMaliyet tSM = new TABSabitMaliyet(ve);
            ve.BeginTransaction();

            if (sm.perHedef.perHedefKod > 0)
            {
                if (!PEBGenel.PerformansHedefiVarMi(ve, sm.yil, sm.perHedef.kurKod, sm.perHedef.perHedefKod))
                {
                    sonuc.islemSonuc = false;
                    sonuc.hataStr = PerformansButceResI.PEB103 + "<br>";
                    ve.VeriErisimKapat();
                    return sonuc;
                }
            }

            if (tSM.SMVarMi(sm.yil, sm.smKod))
                sonuc = tSM.Guncelle(sm);
            else if (sm.smKod > 0)
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = PerformansButceResI.PEB104 + "<br>";
            }
            //Erdal
            //Daha önceden böyle bir kural vardı, harcamaları takip ederken
            //aynı türden sabit maliyetler farklı fonksiyonel kodlarda olduğu için ayrılmak zorunda kaldı.
            //Bu nedenle bu kontrol (kural) kaldırıldı
            //else if (tSM.SMVarMiFromTur(sm.yil, sm.kurKod, sm.asama, sm.tur))
            //{
            //    sonuc.islemSonuc = false;
            //    sonuc.hataStr = "Sistemde bu türden bir Sabit Maliyet kayıtlı görünüyor. Var olan Sabit Maliyet üzerinde işlem yapınız.<br>";
            //}
            else
            {
                sonuc = tSM.Kaydet(sm);
                sm.smKod = OrtakFonksiyonlar.ConvertToInt(sonuc.anahtar.Trim(), 0);
            }

            //if (sonuc.islemSonuc)
            //{
            //    foreach (SabitMaliyetOdenek smo in sm.smOdenek)
            //    {
            //        smo.yil = sm.yil;
            //        smo.kurKod = sm.kurKod;
            //        smo.asama = sm.asama;
            //        smo.smKod = sm.smKod;
            //        smo.smTur = sm.tur;
            //    }
            //    sonuc = PEBSabitMaliyetOdenek.Kaydet(ve, kullanan, sm.smOdenek);
            //}

            if (sonuc.islemSonuc)
            {
                TABFaaliyetProje tFP = new TABFaaliyetProje(ve);
                sonuc = PEBFaaliyetProje.PEBFonKodlariSilKaydet(tFP, sm.yil, sm.kurKod, 1, sm.smKod, sm.smPEBFonKodlar);
            }

            if (sonuc.islemSonuc)
            {
                sonuc.anahtar = sm.smKod.ToString();
                sonuc.bilgiStr = PerformansButceResI.PEB015;
                ve.Commit();
                TNS.KYM.IKYMServis kymServis = TNS.KYM.Arac.Tanimla();
                kymServis.BuyukMetinSakla(kullanan, "PEB", "sma" + sm.yil + sm.kurKod + sm.smKod, kullanan.aktifKultur, sm.smAciklama);
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
        /// Verilen kriterlere uygun olan sabit maliyet bilgilerini döndüren yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// Sabit maliyetin bütçe ile bağını kuran alt faaliyet bilgilerini de listeye ekler.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="sm">Sabit maliyet bilgisini tutan nesne</param>
        /// <param name="detayOdenek">Sabit maliyetin ödenek bilgilerinin istenip istenmediği bilgisi</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında sabit maliyet listesi döndürülür.</returns>
        public static ObjectArray Listele(VeriErisim veriErisim, Kullanici kullanan, SabitMaliyet sm, bool detayOdenek)
        {
            ObjectArray smVAN = new ObjectArray();
            //smVAN.sonuc = Yetki.OkumaHakkiVarMi(kullanan, sm.kurKod);
            //if (!smVAN.sonuc.islemSonuc)
            //    return smVAN;

            if (detayOdenek)
            {
                string hata = string.Empty;
                if (string.IsNullOrEmpty(sm.kurKod))
                    hata += PerformansButceResI.PEB005 + "<br>";
                if (sm.smKod <= 0)
                    hata += PerformansButceResI.PEB098 + "<br>";
                if (!string.IsNullOrEmpty(hata))
                {
                    smVAN.sonuc.islemSonuc = false;
                    smVAN.sonuc.hataStr = hata;
                    return smVAN;
                }
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriPEB", TNS.KYM.Arac.KimlikBul(kullanan));
            VeriErisim ve2 = VeriErisim.VeriErisimBul(null, "BaglantiSatiriPEB", TNS.KYM.Arac.KimlikBul(kullanan));
            TABSabitMaliyet tSM = new TABSabitMaliyet(ve);

            IDataReader dr = tSM.Listele(sm);
            if (dr != null)
            {
                while (dr.Read())
                {
                    SabitMaliyet gelen = new SabitMaliyet();
                    gelen.yil = VeriErisim.DegerAl(dr, 0, 0);
                    gelen.kurKod = VeriErisim.DegerAl(dr, 1, string.Empty);
                    gelen.kurKod = PEBGenel.KurKodNoktala(gelen.kurKod);
                    gelen.kurAd = PEBServis.UzayYaziDegerGetir("ABSKUR", gelen.yil.ToString() + "-" + gelen.kurKod, true, "");
                    gelen.smKod = VeriErisim.DegerAl(dr, 2, 0);
                    gelen.smAd = VeriErisim.DegerAl(dr, 3, string.Empty);
                    gelen.tur = VeriErisim.DegerAl(dr, 4, 0);
                    gelen.perHedef.kurKod = VeriErisim.DegerAl(dr, 5, string.Empty);
                    gelen.perHedef.kurKod = PEBGenel.KurKodNoktala(gelen.perHedef.kurKod);
                    gelen.perHedef.kurAd = PEBServis.UzayYaziDegerGetir("ABSKUR", gelen.yil.ToString() + "-" + gelen.perHedef.kurKod, true, "");
                    gelen.perHedef.perHedefKod = VeriErisim.DegerAl(dr, 6, 0);
                    gelen.perHedef.perHedef = VeriErisim.DegerAl(dr, 7, string.Empty);
                    smVAN.objeler.Add(gelen);
                }
                dr.Close();

                foreach (SabitMaliyet gelen in smVAN.objeler)
                {
                    gelen.smPEBFonKodlar = PEBFaaliyetProje.PEBFonlariBul(ve, gelen.yil, gelen.kurKod, 1, gelen.smKod);
                }

                if (detayOdenek)
                {
                    foreach (SabitMaliyet gelen in smVAN.objeler)
                    {
                        SabitMaliyetOdenek smo = new SabitMaliyetOdenek();
                        smo.yil = gelen.yil;
                        smo.kurKod = gelen.kurKod;
                        smo.smKod = gelen.smKod;
                        ObjectArray smOdenek = PEBSabitMaliyetOdenek.Listele(ve2, kullanan, smo);
                        if (!smOdenek.sonuc.islemSonuc)
                        {
                            smVAN.sonuc = smOdenek.sonuc;
                            break;
                        }
                        else
                            gelen.smOdenek = smOdenek.objeler;
                    }
                }
            }

            if (smVAN.objeler.Count == 0)
                smVAN.sonuc.bilgiStr = PerformansButceResI.PEB017;
            if (smVAN.objeler.Count == 1)
            {
                SabitMaliyet gelen = (SabitMaliyet) smVAN.objeler[0];
                TNS.KYM.IKYMServis kymServis = TNS.KYM.Arac.Tanimla();
                gelen.smAciklama = kymServis.BuyukMetinOku(kullanan, "PEB", "sma" + gelen.yil + gelen.kurKod + gelen.smKod, kullanan.aktifKultur);
            }

            ve.VeriErisimKapat();
            ve2.VeriErisimKapat();
            return smVAN;
        }

        /// <summary>
        /// Sabit maliyeti ve detay bilgilerini silen yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="smler">SabitMaliyet nesnelerinden oluşan listeyi tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public static Sonuc Sil(VeriErisim veriErisim, Kullanici kullanan, TNSCollection smler)
        {
            Sonuc sonuc = new Sonuc(false, PerformansButceResI.PEB040, "");

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriPEB", TNS.KYM.Arac.KimlikBul(kullanan));
            TABSabitMaliyet tSM = new TABSabitMaliyet(ve);
            TABSabitMaliyetAgirlik tSMAgirlik = new TABSabitMaliyetAgirlik(ve);
            TABOdenek tOdenek = new TABOdenek(ve);
            ve.BeginTransaction();

            foreach (SabitMaliyet sm in smler)
            {
                sonuc = Yetki.YazmaHakkiVarMi(kullanan, sm.kurKod);
                if (!sonuc.islemSonuc)
                    break;

                //Sabit Maliyetin ağırlıkları siliniyor.
                SabitMaliyetAgirlik agirlik = new SabitMaliyetAgirlik();
                agirlik.yil = sm.yil;
                agirlik.smKurKod = sm.kurKod;
                agirlik.smKod = sm.smKod;
                sonuc = tSMAgirlik.Sil(agirlik);
                if (!sonuc.islemSonuc)
                    break;

                //Sabit Maliyetin kendisi siliniyor.
                sonuc = tSM.Sil(sm);
                if (!sonuc.islemSonuc)
                    break;
            }

            if (sonuc.islemSonuc)
            {
                ve.Commit();
                sonuc.bilgiStr = PerformansButceResI.PEB020;
            }
            else
            {
                ve.Rollback();
                sonuc.hataStr = PerformansButceResI.PEB021 + "<br>" + sonuc.hataStr;
            }

            ve.VeriErisimKapat();
            return sonuc;
        }
    }
}