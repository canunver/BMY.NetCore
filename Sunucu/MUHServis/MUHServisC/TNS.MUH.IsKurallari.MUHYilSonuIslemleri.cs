using System;
using System.Data;
using TNS.BKD;
using TNS.KYM;
using TNS.MUH.IsKurallari.Veri;
using TNS.Veri;

namespace TNS.MUH.IsKurallari
{
    /// <summary>
    /// Makbuz işlemlerinin yapıldığı sınıf
    /// </summary>
    public class MUHYilSonuIslemleri
    {
        /// <summary>
        /// Makbuz kaydet.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="yilSonuIslemler">makbuzlar</param>
        /// <returns></returns>
        public Sonuc Kaydet(Kullanici kullanan, ObjectArray yilSonuIslemler)
        {
            Ortak000Bul();
            Sonuc sonuc = new Sonuc(false, ButceMuhasebeResI.MUH001, "");
            if (!(Yetki.SistemYoneticisimi(kullanan) ||
                Yetki.MuhasebeYoneticisimi(kullanan) ||
                kullanan.KullaniciTipiIceriyorMu(18)))//Ödeme Emri Yetkisi
            {
                sonuc.hataStr = ButceMuhasebeResI.MUH080;
                sonuc.islemSonuc = false;
                return sonuc;
            }
            else
            {
                foreach (YilSonuIslem m in yilSonuIslemler.objeler)
                {
                    OrtakKontrol(m);
                    if (!TNS.MUH.Yetki.MuhasebeBirimYetkiVarMi(kullanan, m.muhasebe.ToString(), m.birim))
                    {
                        sonuc.hataStr = ButceMuhasebeResI.MUH081;
                        sonuc.islemSonuc = false;
                        return sonuc;
                    }
                }
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(kullanan));

            ve.BeginTransaction();
            TABYilSonuIslemleri to = new TABYilSonuIslemleri(ve);

            foreach (YilSonuIslem m in yilSonuIslemler.objeler)
            {
                string hata = string.Empty;
                if (m.yil <= 0)
                    hata = ButceMuhasebeResI.MUH082 + "<br>";
                if (m.muhasebe <= 0)
                    hata += ButceMuhasebeResI.MUH083 + "<br>";
                if (string.IsNullOrEmpty(m.birim))
                    hata += ButceMuhasebeResI.MUH084 + "<br>";
                if (m.muhasebe > 0 && !string.IsNullOrEmpty(m.birim))
                {
                    if (!(MUHYetkiServis.MakbuzMuhasebeBirimYetkiVarMi(kullanan, m.muhasebe, m.birim) ||
                          TNS.MUH.Yetki.MuhasebeBirimYetkiVarMi(kullanan, m.muhasebe.ToString(), m.birim)))
                        hata += ButceMuhasebeResI.MUH088 + "<br>";
                }
                if (m.birim.Trim() != "000")
                {
                    string kkbirim = MUHServis.UzayYaziDegerGetir(ve, kullanan, "MUHMUHASEBEBIRIM", m.birim + "-" + m.muhasebe.ToString(), false, "BIRIMKOD");
                    if (kkbirim != m.birim)
                        hata += "<br>" + string.Format(ButceMuhasebeResI.MUH089, m.birim);
                }

                if (!string.IsNullOrEmpty(hata))
                {
                    sonuc.islemSonuc = false;
                    sonuc.hataStr = hata;
                    break;
                }

                sonuc = to.Kaydet(m);

                if (!sonuc.islemSonuc)
                    break;
            }

            if (sonuc.islemSonuc)
            {
                ve.Commit();
                sonuc.bilgiStr = ButceMuhasebeResI.MUH091;
            }
            else
                ve.Rollback();

            ve.VeriErisimKapat();
            return sonuc;
        }

        /// <summary>
        /// Makbuz durum guncelle.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="makbuzlar">makbuzlar</param>
        /// <returns></returns>
        public Sonuc GuncelleBelgeNo(Kullanici kullanan, int tur, int yil, string belgeNo, int muhasebe)
        {
            Sonuc sonuc = new Sonuc(false, ButceMuhasebeResI.MUH001, "");

            if (!(Yetki.SistemYoneticisimi(kullanan) ||
                Yetki.MuhasebeYoneticisimi(kullanan)))
            {
                sonuc.hataStr = ButceMuhasebeResI.MUH080;
                sonuc.islemSonuc = false;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(kullanan));

            ve.BeginTransaction();
            TABYilSonuIslemleri to = new TABYilSonuIslemleri(ve);
            sonuc = to.GuncelleBelgeNo(tur, yil, belgeNo, muhasebe);

            if (sonuc.islemSonuc)
            {
                ve.Commit();
                sonuc.bilgiStr = ButceMuhasebeResI.MUH097;
            }
            else
                ve.Rollback();

            ve.VeriErisimKapat();
            return sonuc;
        }

        ///// <summary>
        ///// Makbuz odeme emri guncelle.
        ///// </summary>
        ///// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        ///// <param name="makbuzlar">makbuzlar</param>
        ///// <returns></returns>
        //public Sonuc OdemeEmriGuncelle(Kullanici kullanan, ObjectArray makbuzlar)
        //{
        //    Sonuc sonuc = new Sonuc(false, ButceMuhasebeResI.MUH001, "");

        //    if (!(Yetki.SistemYoneticisimi(kullanan) ||
        //        Yetki.MuhasebeYoneticisimi(kullanan)))
        //    {
        //        sonuc.hataStr = ButceMuhasebeResI.MUH080;
        //        sonuc.islemSonuc = false;
        //        return sonuc;
        //    }

        //    VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(kullanan));

        //    ve.BeginTransaction();
        //    TABMakbuz tMakbuz = new TABMakbuz(ve);

        //    foreach (Makbuz m in makbuzlar.objeler)
        //    {
        //        string hata = string.Empty;
        //        if (m.yil <= 0)
        //            hata = ButceMuhasebeResI.MUH092 + "<br>";
        //        if (m.muhasebe <= 0)
        //            hata += ButceMuhasebeResI.MUH093 + "<br>";
        //        if (string.IsNullOrEmpty(m.yevmiyeNo))
        //            hata += ButceMuhasebeResI.MUH094 + "<br>";
        //        if (m.durum <= 0)
        //            hata += ButceMuhasebeResI.MUH095 + "<br>";
        //        if (!string.IsNullOrEmpty(hata))
        //        {
        //            sonuc.islemSonuc = false;
        //            sonuc.hataStr = hata;
        //            break;
        //        }

        //        sonuc = tMakbuz.OdemeEmriGuncelle(m);

        //        if (!sonuc.islemSonuc)
        //            break;
        //    }

        //    if (sonuc.islemSonuc)
        //    {
        //        ve.Commit();
        //        sonuc.bilgiStr = ButceMuhasebeResI.MUH097;
        //    }
        //    else
        //        ve.Rollback();

        //    ve.VeriErisimKapat();
        //    return sonuc;
        //}

        /// <summary>
        /// Makbuz listele.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="makbuz">makbuz</param>
        /// <returns></returns>
        public ObjectArray Listele(Kullanici kullanan, YilSonuIslem yilSonuIslem, bool yilSonuIslemKaydiOlustur)
        {
            ObjectArray yler = new ObjectArray();
            Ortak000Bul();
            OrtakKontrol(yilSonuIslem);
            if (!(Yetki.SistemYoneticisimi(kullanan) ||
                Yetki.MuhasebeYoneticisimi(kullanan) ||
                kullanan.KullaniciTipiIceriyorMu(18)))//Ödeme Emri Yetkisi
            {
                yler.sonuc.hataStr = ButceMuhasebeResI.MUH080;
                yler.sonuc.islemSonuc = false;
                return yler;
            }
            else
            {
                if (!TNS.MUH.Yetki.MuhasebeBirimYetkiVarMi(kullanan, yilSonuIslem.muhasebe.ToString(), yilSonuIslem.birim))
                {
                    yler.sonuc.hataStr = ButceMuhasebeResI.MUH098;
                    yler.sonuc.islemSonuc = false;
                    return yler;
                }
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABYilSonuIslemleri to = new TABYilSonuIslemleri(ve);

            //Listeye yeni kapatma satırları eklendi (tur:20-21-22) eğer liste kayıt edilmiş ise gösterilmiyor. 
            //Bu nedenle listede eğer bir belge oluşturulmamış ise YilSonuIslemKaydiOlustur tekrar oluşturulması gerekli
            bool listeyiYenidenOlustur = true;

            IDataReader dr = to.Listele(yilSonuIslem);
            if (string.IsNullOrEmpty(to.Hata()))
            {
                while (dr.Read())
                {
                    YilSonuIslem y = new YilSonuIslem();

                    y.yil = VeriErisim.DegerAl(dr, 0, 0);
                    y.muhasebe = VeriErisim.DegerAl(dr, 1, 0);
                    y.belgeNo = VeriErisim.DegerAl(dr, 2, string.Empty);
                    y.tur = VeriErisim.DegerAl(dr, 3, 0);
                    y.ad = VeriErisim.DegerAl(dr, 4, string.Empty);
                    y.aciklama = VeriErisim.DegerAl(dr, 5, string.Empty);

                    if (y.belgeNo != "") listeyiYenidenOlustur = false;

                    yler.objeler.Add(y);
                }
            }
            else
            {
                yler.sonuc.hataStr = to.Hata();
                yler.sonuc.islemSonuc = false;
            }
            dr.Close();

            ve.VeriErisimKapat();


            if (yler.objeler.Count == 0 || listeyiYenidenOlustur)
            {
                if (yilSonuIslemKaydiOlustur)
                    yler = YilSonuIslemKaydiOlustur(kullanan, yilSonuIslem);
            }
            return yler;
        }

        private static string ortak000;
        private void Ortak000Bul()
        {
            ortak000 = TNS.MUH.Arac.DegiskenDegerBul(0, "/Ortak000");
        }

        private void OrtakKontrol(YilSonuIslem yilSonuIslem)
        {
            if (ortak000 == "1")
                yilSonuIslem.birim = "000";
        }

        private ObjectArray YilSonuIslemKaydiOlustur(Kullanici kullanan, YilSonuIslem yilSonuIslem)
        {
            ObjectArray list = new ObjectArray();
            list.objeler.Add(new YilSonuIslem { yil = yilSonuIslem.yil, muhasebe = yilSonuIslem.muhasebe, birim = yilSonuIslem.birim, tur = 1, ad = "Tüm ödenekleri gönder - 1" });
            list.objeler.Add(new YilSonuIslem { yil = yilSonuIslem.yil, muhasebe = yilSonuIslem.muhasebe, birim = yilSonuIslem.birim, tur = 2, ad = "Tüm ödenekleri gönder - 2" });
            list.objeler.Add(new YilSonuIslem { yil = yilSonuIslem.yil, muhasebe = yilSonuIslem.muhasebe, birim = yilSonuIslem.birim, tur = 3, ad = "Tüm tenkisleri yap - 1" });
            list.objeler.Add(new YilSonuIslem { yil = yilSonuIslem.yil, muhasebe = yilSonuIslem.muhasebe, birim = yilSonuIslem.birim, tur = 4, ad = "Tüm tenkisleri yap - 2" });
            list.objeler.Add(new YilSonuIslem { yil = yilSonuIslem.yil, muhasebe = yilSonuIslem.muhasebe, birim = yilSonuIslem.birim, tur = 5, ad = "Ödenek İptal" });
            list.objeler.Add(new YilSonuIslem { yil = yilSonuIslem.yil, muhasebe = yilSonuIslem.muhasebe, birim = yilSonuIslem.birim, tur = 6, ad = "Ödenekli Giderleri Kapat" });
            list.objeler.Add(new YilSonuIslem { yil = yilSonuIslem.yil, muhasebe = yilSonuIslem.muhasebe, birim = yilSonuIslem.birim, tur = 7, ad = "Bütçe Ödeneklerini Kapat" });
            list.objeler.Add(new YilSonuIslem { yil = yilSonuIslem.yil, muhasebe = yilSonuIslem.muhasebe, birim = yilSonuIslem.birim, tur = 8, ad = "Amortisman Hesapları" });
            list.objeler.Add(new YilSonuIslem { yil = yilSonuIslem.yil, muhasebe = yilSonuIslem.muhasebe, birim = yilSonuIslem.birim, tur = 9, ad = "Bütçe Giderleri Kapat" });
            list.objeler.Add(new YilSonuIslem { yil = yilSonuIslem.yil, muhasebe = yilSonuIslem.muhasebe, birim = yilSonuIslem.birim, tur = 10, ad = "Red ve İadeleri Kapat" });
            list.objeler.Add(new YilSonuIslem { yil = yilSonuIslem.yil, muhasebe = yilSonuIslem.muhasebe, birim = yilSonuIslem.birim, tur = 11, ad = "Gider Yansıtmayı Kapat" });
            list.objeler.Add(new YilSonuIslem { yil = yilSonuIslem.yil, muhasebe = yilSonuIslem.muhasebe, birim = yilSonuIslem.birim, tur = 12, ad = "Bütçe Gelirleri Kapat" });
            list.objeler.Add(new YilSonuIslem { yil = yilSonuIslem.yil, muhasebe = yilSonuIslem.muhasebe, birim = yilSonuIslem.birim, tur = 13, ad = "Gelir Yansıtmayı Kapat" });
            list.objeler.Add(new YilSonuIslem { yil = yilSonuIslem.yil, muhasebe = yilSonuIslem.muhasebe, birim = yilSonuIslem.birim, tur = 14, ad = "Faaliyet Giderlerini Kapat" });
            list.objeler.Add(new YilSonuIslem { yil = yilSonuIslem.yil, muhasebe = yilSonuIslem.muhasebe, birim = yilSonuIslem.birim, tur = 15, ad = "Faaliyet Gelirlerini Kapat" });
            list.objeler.Add(new YilSonuIslem { yil = yilSonuIslem.yil, muhasebe = yilSonuIslem.muhasebe, birim = yilSonuIslem.birim, tur = 16, ad = "Faaliyet Hesaplarını Kapat" });
            list.objeler.Add(new YilSonuIslem { yil = yilSonuIslem.yil, muhasebe = yilSonuIslem.muhasebe, birim = yilSonuIslem.birim, tur = 17, ad = "..." });

            list.objeler.Add(new YilSonuIslem { yil = yilSonuIslem.yil, muhasebe = yilSonuIslem.muhasebe, birim = yilSonuIslem.birim, tur = 20, ad = "Nakit Hareketleri ve Muh.Birimleri Arası Kapat (510-511-519)" });
            list.objeler.Add(new YilSonuIslem { yil = yilSonuIslem.yil, muhasebe = yilSonuIslem.muhasebe, birim = yilSonuIslem.birim, tur = 21, ad = "Değer Hareketleri Sonuç Kapat (519)" });
            list.objeler.Add(new YilSonuIslem { yil = yilSonuIslem.yil, muhasebe = yilSonuIslem.muhasebe, birim = yilSonuIslem.birim, tur = 22, ad = "Faliyet Sonuçları Kapat (580-500)" });

            list.objeler.Add(new YilSonuIslem { yil = yilSonuIslem.yil, muhasebe = yilSonuIslem.muhasebe, birim = yilSonuIslem.birim, tur = 40, ad = "..." });

            list.objeler.Add(new YilSonuIslem { yil = yilSonuIslem.yil, muhasebe = yilSonuIslem.muhasebe, birim = yilSonuIslem.birim, tur = 18, ad = "Son Kayıt" });
            list.objeler.Add(new YilSonuIslem { yil = yilSonuIslem.yil, muhasebe = yilSonuIslem.muhasebe, birim = yilSonuIslem.birim, tur = 19, ad = "Açılış Kaydı" });

            list.objeler.Add(new YilSonuIslem { yil = yilSonuIslem.yil, muhasebe = yilSonuIslem.muhasebe, birim = yilSonuIslem.birim, tur = 23, ad = "120 hesabını Kapat" });
            list.objeler.Add(new YilSonuIslem { yil = yilSonuIslem.yil, muhasebe = yilSonuIslem.muhasebe, birim = yilSonuIslem.birim, tur = 24, ad = "320 hesabını Kapat" });

            Sonuc sonuc = Kaydet(kullanan, list);

            if (sonuc.islemSonuc)
            {
                return Listele(kullanan, yilSonuIslem, false);
            }

            return null;
        }
    }
}