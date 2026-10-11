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
    public class MUHTalepTanimForm
    {
        /// <summary>
        /// Talep tanim kaydet.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="makbuzTipleri">makbuz tipleri</param>
        /// <returns></returns>
        public static Sonuc Kaydet(Kullanici kullanan, TalepTanimForm bilgi)
        {
            Sonuc sonuc = new Sonuc(false, ButceMuhasebeResI.MUH001, "");

            sonuc = Kontrol("KAYDET", kullanan, bilgi);
            if (!sonuc.islemSonuc)
                return sonuc;

            //if (!Yetki.SistemYoneticisimi(kullanan))
            //{
            //	sonuc.hataStr = ButceMuhasebeResI.MUH080;
            //	sonuc.islemSonuc = false;
            //	return sonuc;
            //}

            if (bilgi.kod == "")
            {
                ObjectArray liste = Listele(kullanan, bilgi);

                if (liste.objeler.Count > 0)
                {
                    sonuc.hataStr = "Kayıt edilmek istenilen tanımla ilgili tanım daha önce yapılmıştır, Kayıtlı tanımı bulun ve onun üzerinde değişiklik yapın. Verilen yıl, ay ve Talep türüne ait yeni tanım yapamazsınız";
                    sonuc.islemSonuc = false;
                    return sonuc;
                }
            }


            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(kullanan));

            ve.BeginTransaction();
            TABTalepTanimForm to = new TABTalepTanimForm(ve);

            bilgi.kayitEden = kullanan.KullaniciKodu;
            bilgi.kayitTarihi = new TNSDateTime(DateTime.Now);

            if (bilgi.kod == "")
            {
                bilgi.kod = Guid.NewGuid().ToString("N");
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

        /// <summary>
        /// Talep tanim sil.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="bilgi">TalepTanimForm nesnesi</param>
        /// <returns></returns>
        public static Sonuc Sil(Kullanici kullanan, TalepTanimForm bilgi)
        {
            Sonuc sonuc = new Sonuc(false, ButceMuhasebeResI.MUH068, "");

            sonuc = Kontrol("SIL", kullanan, bilgi);
            if (!sonuc.islemSonuc)
                return sonuc;

            TalepTanimForm kriter = new TalepTanimForm();
            kriter.kod = bilgi.kod;
            ObjectArray bilgiler = Listele(kullanan, kriter);

            if (bilgiler.objeler.Count > 0)
                kriter = (TalepTanimForm)bilgiler.objeler[0];

            TkdkTalepForm kriterForm = new TkdkTalepForm();
            kriterForm.yil = kriter.yil;
            kriterForm.ay = kriter.ay;
            kriterForm.tur = kriter.talepTur;

            ObjectArray liste = MUHTKDKTalep.Listele(kullanan, kriterForm);

            if (liste.objeler.Count > 0)
            {
                sonuc.hataStr = "Silinmesi istenilen tanımı kullanan Talep Kaydı bulunmaktadır. Bu nedenle silme işlemi yapılamaz. İlk önce ilgili talep kaydını siliniz.";
                sonuc.islemSonuc = false;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(kullanan));

            ve.BeginTransaction();
            TABTalepTanimForm to = new TABTalepTanimForm(ve);

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
        public static ObjectArray Listele(Kullanici kullanan, TalepTanimForm bilgi)
        {
            ObjectArray liste = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABTalepTanimForm to = new TABTalepTanimForm(ve);

            IDataReader dr = to.Listele(bilgi);
            if (string.IsNullOrEmpty(to.Hata()))
            {
                while (dr.Read())
                {
                    TalepTanimForm t = new TalepTanimForm();
                    t.kod = VeriErisim.DegerAl(dr, 0, string.Empty);
                    t.yil = VeriErisim.DegerAl(dr, 1, 0);
                    t.ay = VeriErisim.DegerAl(dr, 2, 0);
                    t.talepTur = VeriErisim.DegerAl(dr, 3, 0);
                    t.kayitEden = VeriErisim.DegerAl(dr, 4, "");
                    t.kayitTarihi = VeriErisim.DegerAl(dr, 5);

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
                foreach (TalepTanimForm b in liste.objeler)
                {
                    TalepTanimFormDetay d = new TalepTanimFormDetay();
                    d.talepKod = b.kod;
                    b.satirlar = ListeleDetay(ve, kullanan, d);
                }
            }

            ve.VeriErisimKapat();
            return liste;
        }

        private static Sonuc Kontrol(string tip, Kullanici kullanan, TalepTanimForm bilgi)
        {
            Sonuc sonuc = new Sonuc();

            if (tip == "SIL")
            {
                //if (string.IsNullOrWhiteSpace(bilgi.belgeNo))
                //    sonuc.hataStr = "Silinecek kaydı seçiniz.";
            }

            if (tip == "KAYDET" || tip == "GUNCELLE")
            {
                if (DateTime.Now.Month == 12//YILIN SON AYI ISE KAYITEDILMEK ISTENEN YIL SONRAKI YIL ISE OCAK AYINA KAYIT YAPABILSIN
                    && bilgi.yil > DateTime.Now.Year
                    && bilgi.ay > 1)
                {
                    sonuc.hataStr = "Ay bilgisi en fazla Ocak Ayı olarak kayıt edilebilir.";
                }


                if (bilgi.yil == 0)
                    sonuc.hataStr += "Yıl bilgisi boş bırakılamaz." + "<br>";
                if (bilgi.ay == 0)
                    sonuc.hataStr += "Ay bilgisi boş bırakılamaz." + "<br>";
                if (bilgi.talepTur == 0)
                    sonuc.hataStr += "Talep Tür bilgisi boş bırakılamaz." + "<br>";
                if (bilgi.satirlar.objeler.Count == 0)
                    sonuc.hataStr += "Detay bilgisi boş bırakılamaz." + "<br>";
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
        /// <param name="makbuzTipleri">makbuz tipleri</param>
        /// <returns></returns>
        public static Sonuc KaydetDetay(VeriErisim ve, Kullanici kullanan, TalepTanimForm bilgi)
        {
            Sonuc sonuc = new Sonuc(false, ButceMuhasebeResI.MUH001, "");

            ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(kullanan));

            ve.BeginTransaction();
            TABTalepTanimForm to = new TABTalepTanimForm(ve);

            sonuc = to.SilDetay(bilgi);

            foreach (TalepTanimFormDetay item in bilgi.satirlar.objeler)
            {
                if (item.kod.IndexOf("-") > -1) item.kod = "";

                if (item.kod == "")
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
        /// <param name="bilgi">TalepTanimForm nesnesi</param>
        /// <returns></returns>
        public static Sonuc SilDetay(Kullanici kullanan, TalepTanimForm bilgi)
        {
            Sonuc sonuc = new Sonuc(false, ButceMuhasebeResI.MUH068, "");

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(kullanan));

            ve.BeginTransaction();
            TABTalepTanimForm to = new TABTalepTanimForm(ve);

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
        /// Talep tanim detay listele.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="bilgi">Talep tanim</param>
        /// <returns></returns>
        public static ObjectArray ListeleDetay(VeriErisim ve, Kullanici kullanan, TalepTanimFormDetay bilgi)
        {
            ObjectArray liste = new ObjectArray();

            ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABTalepTanimForm to = new TABTalepTanimForm(ve);

            IDataReader dr = to.ListeleDetay(bilgi);
            if (string.IsNullOrEmpty(to.Hata()))
            {
                while (dr.Read())
                {
                    TalepTanimFormDetay t = new TalepTanimFormDetay();
                    t.kod = VeriErisim.DegerAl(dr, 0, string.Empty);
                    t.talepKod = VeriErisim.DegerAl(dr, 1, string.Empty);
                    t.ekoKod = VeriErisim.DegerAl(dr, 2, string.Empty);
                    t.ekoAdi = VeriErisim.DegerAl(dr, 3, string.Empty);

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