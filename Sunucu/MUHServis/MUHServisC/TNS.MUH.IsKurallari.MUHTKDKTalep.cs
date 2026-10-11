using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using TNS.HRC;
using TNS.KYM;
using TNS.MUH.IsKurallari.Veri;
using TNS.Veri;
using System.Linq;

namespace TNS.MUH.IsKurallari
{
    /// <summary>
    /// Talep Tanım Form işlemlerinin yapıldığı sınıf
    /// </summary>
    public class MUHTKDKTalep
    {
        /// <summary>
        /// Talep tanim kaydet.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <returns></returns>
        public static Sonuc Kaydet(Kullanici kullanan, TkdkTalepForm bilgi)
        {
            Sonuc sonuc = new Sonuc(false, ButceMuhasebeResI.MUH001, "");

            if (!TNS.MUH.Yetki.MuhasebeBirimYetkiVarMi(kullanan, bilgi.muhasebeKod.ToString(), bilgi.birimKod))
            {
                sonuc.hataStr = string.Format(ButceMuhasebeResI.MUH011, bilgi.muhasebeKod + "-" + bilgi.birimKod);
                sonuc.islemSonuc = false;
                return sonuc;
            }

            sonuc = Kontrol("KAYDET", kullanan, bilgi);
            if (!sonuc.islemSonuc)
                return sonuc;

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(kullanan));

            ve.BeginTransaction();
            TABTKDKTalep to = new TABTKDKTalep(ve);
            TABGenel tg = new TABGenel(ve);

            bilgi.kayitEden = kullanan.KullaniciKodu;
            bilgi.kayitEdenAdi = kullanan.ADSOYAD;
            bilgi.kayitTarihi = new TNSDateTime(DateTime.Now);

            if (bilgi.kod == "")
            {
                bilgi.kod = Guid.NewGuid().ToString("N");

                string sayacKod = bilgi.yil + kullanan.calistigiKurum.Replace(".", "");
                sayacKod = "TKDKTLP" + sayacKod;
                bilgi.belgeNo = tg.BelgeNoAl(sayacKod, "000000");

                sonuc = to.Kaydet(bilgi);
            }
            else
                sonuc = to.Guncelle(bilgi);

            if (sonuc.islemSonuc)
            {
                sonuc = KaydetDetay(ve, kullanan, bilgi);
            }

            if (sonuc.islemSonuc)
            {
                ve.Commit();
                sonuc.anahtar = bilgi.kod.ToString();
                sonuc.bilgiStr = ButceMuhasebeResI.MUH132;
            }
            else
                ve.Rollback();

            ve.VeriErisimKapat();
            return sonuc;
        }

        public static Sonuc TertipKaydet(Kullanici kullanan, ObjectArray liste)
        {
            Sonuc sonuc = new Sonuc(false, ButceMuhasebeResI.MUH001, "");

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(kullanan));

            ve.BeginTransaction();
            TABTKDKTalep to = new TABTKDKTalep(ve);

            foreach (TkdkTalepDetay item in liste.objeler)
            {
                sonuc = to.TertipKaydet(item);
            }

            if (sonuc.islemSonuc)
            {
                ve.Commit();
                sonuc.bilgiStr = ButceMuhasebeResI.MUH132;
            }
            else
                ve.Rollback();

            ve.VeriErisimKapat();
            return sonuc;
        }

        public static Sonuc GuncelleDurum(Kullanici kullanan, ObjectArray bilgi)
        {
            Sonuc sonuc = new Sonuc(false, ButceMuhasebeResI.MUH052, "");

            if (bilgi.ObjeSayisi == 0)
            {
                sonuc.hataStr = "Durum değişikliği için listeden seçim yapın";
                sonuc.islemSonuc = false;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(kullanan));

            ve.BeginTransaction();
            TABTKDKTalep to = new TABTKDKTalep(ve);

            foreach (TkdkTalepForm item in bilgi.objeler)
            {
                sonuc = to.GuncelleDurum(item);

                if (!sonuc.islemSonuc)
                    break;
            }

            if (sonuc.islemSonuc)
            {
                ve.Commit();
                sonuc.bilgiStr = "Değişiklik yapılmak istenilen belgeler başarıyla güncellendi.";//string.Format(ButceMuhasebeResI.MUH188, ");
            }
            else
                ve.Rollback();

            ve.VeriErisimKapat();
            return sonuc;
        }

        /// <summary>
        /// Talep tanim sil.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="bilgi">TkdkTalepForm nesnesi</param>
        /// <returns></returns>
        public static Sonuc Sil(Kullanici kullanan, TkdkTalepForm bilgi)
        {
            Sonuc sonuc = new Sonuc(false, ButceMuhasebeResI.MUH068, "");

            if (!TNS.MUH.Yetki.MuhasebeBirimYetkiVarMi(kullanan, bilgi.muhasebeKod.ToString(), bilgi.birimKod))
            {
                sonuc.hataStr = string.Format(ButceMuhasebeResI.MUH011, bilgi.muhasebeKod);
                sonuc.islemSonuc = false;
                return sonuc;
            }

            sonuc = Kontrol("SIL", kullanan, bilgi);
            if (!sonuc.islemSonuc)
                return sonuc;
            //if (!Yetki.SistemYoneticisimi(kullanan))
            //{
            //	sonuc.hataStr = ButceMuhasebeResI.MUH080;
            //	sonuc.islemSonuc = false;
            //	return sonuc;
            //}

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(kullanan));

            ve.BeginTransaction();
            TABTKDKTalep to = new TABTKDKTalep(ve);

            sonuc = to.SilDetay(bilgi);

            sonuc = to.Sil(bilgi);

            if (sonuc.islemSonuc)
            {
                ve.Commit();
                sonuc.bilgiStr = ButceMuhasebeResI.MUH207;
            }
            else
                ve.Rollback();

            ve.VeriErisimKapat();
            return sonuc;
        }

        /// <summary>
        /// Talep tanim listele.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="bilgi">Talep tanim</param>
        /// <returns></returns>
        public static ObjectArray Listele(Kullanici kullanan, TkdkTalepForm bilgi)
        {
            ObjectArray liste = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABTKDKTalep to = new TABTKDKTalep(ve);

            IDataReader dr = to.Listele(bilgi);
            if (string.IsNullOrEmpty(to.Hata()))
            {
                while (dr.Read())
                {
                    TkdkTalepForm t = new TkdkTalepForm();
                    t.kod = VeriErisim.DegerAl(dr, 0, string.Empty);
                    t.belgeNo = VeriErisim.DegerAl(dr, 1, string.Empty);
                    t.yil = VeriErisim.DegerAl(dr, 2, 0);
                    t.ay = VeriErisim.DegerAl(dr, 3, 0);
                    t.tur = VeriErisim.DegerAl(dr, 4, 0);
                    t.muhasebeKod = VeriErisim.DegerAl(dr, 5, 0);
                    t.birimKod = VeriErisim.DegerAl(dr, 6, "");
                    t.durum = VeriErisim.DegerAl(dr, 7, 0);
                    t.kayitEden = VeriErisim.DegerAl(dr, 8, "");
                    t.kayitEdenAdi = VeriErisim.DegerAl(dr, 9, "");
                    t.kayitTarihi = VeriErisim.DegerAl(dr, 10);
                    t.gerekce = VeriErisim.DegerAl(dr, 11, "");
                    t.odemeEmriNo1 = VeriErisim.DegerAl(dr, 12, "");
                    t.odemeEmriNo2 = VeriErisim.DegerAl(dr, 13, "");
                    t.odemeEmriNo3 = VeriErisim.DegerAl(dr, 14, "");
                    t.odemeEmriNo4 = VeriErisim.DegerAl(dr, 15, "");
                    t.mevcutBakiye = VeriErisim.DegerAl(dr, 16, 0.0);
                    t.birimAdi = VeriErisim.DegerAl(dr, 17, "");
                    t.birimIban = VeriErisim.DegerAl(dr, 18, "");
                    t.birimVergiNo = VeriErisim.DegerAl(dr, 19, "");
                    t.birimVergiAdi = VeriErisim.DegerAl(dr, 20, "");
                    t.gelenNakit = VeriErisim.DegerAl(dr, 21, 0);
                    t.birimKurumsalkod = VeriErisim.DegerAl(dr, 22, "");

                    bool ekle = true;
                    if (!TNS.MUH.Yetki.MuhasebeBirimYetkiVarMi(kullanan, t.muhasebeKod.ToString(), t.birimKod))
                    {
                        ekle = false;
                    }

                    if (ekle)
                        liste.objeler.Add(t);
                }
            }
            else
            {
                liste.sonuc.hataStr = to.Hata();
                liste.sonuc.islemSonuc = false;
            }
            dr.Close();

            if (bilgi.detayGoster)
            {
                foreach (TkdkTalepForm b in liste.objeler)
                {
                    TkdkTalepDetay d = new TkdkTalepDetay();
                    d.talepKod = b.kod;
                    b.satirlar = ListeleDetay(ve, kullanan, d);
                }
            }

            ve.VeriErisimKapat();
            return liste;
        }

        private static Sonuc Kontrol(string tip, Kullanici kullanan, TkdkTalepForm bilgi)
        {
            Sonuc sonuc = new Sonuc();

            if (tip == "SIL")
            {
                if (bilgi.kod != "")
                {
                    ObjectArray bilgiler = Listele(kullanan, bilgi);

                    if (bilgiler.ObjeSayisi > 0)
                        bilgi = (TkdkTalepForm)bilgiler.objeler[0];
                    else
                        sonuc.hataStr = "Silinecek kaydı seçiniz.<br>";

                    if (bilgi.durum == (int)ENUMTKDKTalepDurum.TAMAMLANDI)
                    {
                        sonuc.hataStr = "Silmek istediğiniz belgenin durumu uygun değil<br>";
                    }
                }
                else
                    sonuc.hataStr = "Silinecek kaydı seçiniz.";
            }

            if (tip == "KAYDET" || tip == "GUNCELLE")
            {
                if (bilgi.yil == 0)
                    sonuc.hataStr += "Yıl bilgisi boş bırakılamaz." + "<br>";
                if (bilgi.ay == 0)
                    sonuc.hataStr += "Ay bilgisi boş bırakılamaz." + "<br>";
                if (bilgi.muhasebeKod == 0)
                    sonuc.hataStr += "Muhasebe bilgisi boş bırakılamaz." + "<br>";
                if (bilgi.birimKod.Trim() == "")
                    sonuc.hataStr += "Birim bilgisi boş bırakılamaz." + "<br>";
                if (bilgi.tur == 0)
                    sonuc.hataStr += "Tür bilgisi boş bırakılamaz." + "<br>";
                if (bilgi.tur == 1 && bilgi.mevcutBakiye < 0)//Nakit talep ise
                    sonuc.hataStr += "Mevcut Bakiye bilgisi boş bırakılamaz." + "<br>";
                if (bilgi.satirlar.objeler.Count == 0 && bilgi.tur != 3)
                    sonuc.hataStr += "Detay bilgisi boş bırakılamaz." + "<br>";

                if (bilgi.kod != "")
                {
                    TkdkTalepForm tmp = new TkdkTalepForm();
                    tmp.kod = bilgi.kod;
                    tmp.detayGoster = false;
                    ObjectArray bilgiler = Listele(kullanan, tmp);

                    if (bilgiler.ObjeSayisi > 0)
                    {
                        bilgi = (TkdkTalepForm)bilgiler.objeler[0];

                        if (bilgi.durum == (int)ENUMTKDKTalepDurum.TAMAMLANDI)
                        {
                            sonuc.hataStr = "Güncellemek istediğiniz belge TAMAMLANDI durumunda olduğu için değiştirilemez<br>";
                        }
                    }
                }
                else
                {
                    TNSDateTime tarihKontrol = new TNSDateTime("01." + bilgi.ay.ToString("00") + "." + bilgi.yil);
                    TimeSpan fark = DateTime.Now - tarihKontrol.Oku();
                    if (Math.Abs(fark.Days) > 32)
                        sonuc.hataStr += "Yıl ve ay bilgisini kontrol edin. En fazla 1 ay sonrası için talep yapabilirsiniz." + "<br>";

                    TkdkTalepForm tmp = new TkdkTalepForm();
                    tmp.yil = bilgi.yil;
                    tmp.ay = bilgi.ay;
                    tmp.muhasebeKod = bilgi.muhasebeKod;
                    tmp.birimKod = bilgi.birimKod;
                    tmp.tur = bilgi.tur;
                    ObjectArray bilgiler = Listele(kullanan, tmp);
                    if (bilgiler.ObjeSayisi > 0)
                    {
                        sonuc.hataStr = bilgi.muhasebeKod + "-" + bilgi.birimKod + " birimi ve " + bilgi.yil + " " + OrtakFonksiyonlar.AyAdiGetir(bilgi.ay) + " ayına ait talep kaydı bulunmaktadır. Bu nedenle yeni talep girilemez<br>";
                    }
                }

            }

            if (!string.IsNullOrEmpty(sonuc.hataStr))
                sonuc.islemSonuc = false;
            else
                sonuc.islemSonuc = true;

            return sonuc;
        }

        /// <summary>
        /// Talep tanim detay kaydet.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <returns></returns>
        public static Sonuc KaydetDetay(VeriErisim ve, Kullanici kullanan, TkdkTalepForm bilgi)
        {
            Sonuc sonuc = new Sonuc(false, ButceMuhasebeResI.MUH001, "");

            ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(kullanan));

            ve.BeginTransaction();
            TABTKDKTalep to = new TABTKDKTalep(ve);

            sonuc = to.SilDetay(bilgi);

            foreach (TkdkTalepDetay item in bilgi.satirlar.objeler)
            {
                item.kod = Guid.NewGuid().ToString("N");
                item.talepKod = bilgi.kod;

                sonuc = to.KaydetDetay(item);

                if (!sonuc.islemSonuc)
                {
                    sonuc.islemSonuc = false;
                    ve.Rollback();
                    ve.VeriErisimKapat();
                    return sonuc;
                }
            }

            if (sonuc.islemSonuc)
            {
                ve.Commit();
                sonuc.anahtar = bilgi.kod.ToString();
                sonuc.bilgiStr = ButceMuhasebeResI.MUH132;
            }
            else
                ve.Rollback();

            ve.VeriErisimKapat();
            return sonuc;
        }

        /// <summary>
        /// Talep tanim detay sil.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="bilgi">TkdkTalepForm nesnesi</param>
        /// <returns></returns>
        public static Sonuc SilDetay(Kullanici kullanan, TkdkTalepForm bilgi)
        {
            Sonuc sonuc = new Sonuc(false, ButceMuhasebeResI.MUH068, "");

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(kullanan));

            ve.BeginTransaction();
            TABTKDKTalep to = new TABTKDKTalep(ve);

            sonuc = to.Sil(bilgi);

            if (sonuc.islemSonuc)
            {
                ve.Commit();
                sonuc.bilgiStr = ButceMuhasebeResI.MUH207;
            }
            else
                ve.Rollback();

            ve.VeriErisimKapat();
            return sonuc;
        }

        public static Sonuc OdemeEmriNoGuncelle(Kullanici kullanan, TkdkTalepForm bilgi)
        {
            Sonuc sonuc = new Sonuc(false, ButceMuhasebeResI.MUH068, "");

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(kullanan));

            ve.BeginTransaction();
            TABTKDKTalep to = new TABTKDKTalep(ve);

            sonuc = to.OdemeEmriNoGuncelle(bilgi);

            if (sonuc.islemSonuc)
            {
                ve.Commit();
            }
            else
                ve.Rollback();

            ve.VeriErisimKapat();
            return sonuc;
        }

        /// <summary>
        /// Talep tanim detay listele.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="bilgi">Talep tanim</param>
        /// <returns></returns>
        public static ObjectArray ListeleDetay(VeriErisim ve, Kullanici kullanan, TkdkTalepDetay bilgi)
        {
            ObjectArray liste = new ObjectArray();

            ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABTKDKTalep to = new TABTKDKTalep(ve);

            IDataReader dr = to.ListeleDetay(bilgi);
            if (string.IsNullOrEmpty(to.Hata()))
            {
                while (dr.Read())
                {
                    TkdkTalepDetay t = new TkdkTalepDetay();
                    t.kod = VeriErisim.DegerAl(dr, 0, string.Empty);
                    t.talepKod = VeriErisim.DegerAl(dr, 1, string.Empty);
                    t.siraNo = VeriErisim.DegerAl(dr, 2, 0);
                    t.ekoKod = VeriErisim.DegerAl(dr, 3, string.Empty);
                    t.tutar = VeriErisim.DegerAl(dr, 4, 0.0);
                    t.tutar1 = VeriErisim.DegerAl(dr, 5, 0.0);
                    t.tutar2 = VeriErisim.DegerAl(dr, 6, 0.0);
                    t.tutar3 = VeriErisim.DegerAl(dr, 7, 0.0);
                    t.gerekce = VeriErisim.DegerAl(dr, 8, string.Empty);
                    t.ekoAdi = VeriErisim.DegerAl(dr, 9, string.Empty);
                    t.tertip = VeriErisim.DegerAl(dr, 10, string.Empty);

                    liste.objeler.Add(t);
                }
            }
            else
            {
                liste.sonuc.hataStr = to.Hata();
                liste.sonuc.islemSonuc = false;
            }
            dr.Close();

            ve.VeriErisimKapat();
            return liste;
        }
    }
}