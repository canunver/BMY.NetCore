using System;
using System.Data;
using TNS;
using TNS.Veri;
using TNS.BHMv2;
using TNS.BHMv2.IsKurallari.Veri;
using TNS.KYM;
using System.Collections;

namespace TNS.BHMv2.IsKurallari
{
    /// <summary>
    /// Bütçe tasnif tanım bilgileriyle ilgili, iş kuralları kontrollerini yapıp,
    /// veri tabanı bağlantısını alıp, veri tabanı fonksiyonlarını çağırarak
    /// güncelleme işlemlerini gerçekleştiren fonksiyonları tutan sınıf.
    /// </summary>
    public class ButceTasnifIslemler
    {
        /// <summary>
        /// Kullanıcının, Tasnif bilgileri üzerinde değişiklik yapma hakkını bulup döndüren yordam.
        /// Kullanıcı sistem yöndetici ise değişiklik yapma hakkı vardır.
        /// </summary>
        /// <param name="kullanan">Yetki kontrolü yapılacak kullanıcıya ait bilgiler</param>
        /// <returns>Değişiklik yapma hakkı varsa doğru, yoksa yanlış döndürülür.</returns>
        public static bool YazmaYetkisiVarMi(Kullanici kullanan)
        {
            if (kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.SISTEMYONETICISI))
                return true;
            return false;
        }

        /// <summary>
        /// Verilen tasnif formundaki alanların tutarlılığını kontrol eden fonksiyon
        /// <param name="kullanan">Alan tutarlılığı kontrol edilirken, kullanıcılara göre farklılık olacaksa, kullanıcı bilgisini tutan nesne</param>
        /// <param name="form">Alan tutarlılığı kontrol edilecek tasnif formu nesnesi</param>
        /// </summary>
        /// <returns>Alanlar tutarlı ise, Sonuc yapısında true, değilse false ve açıklaması döndürülür.</returns>
        public static Sonuc AlanlarGecerliMi(Kullanici kullanan, ButceTasnif form)
        {
            return new Sonuc(true, string.Empty, string.Empty);
        }

        /// <summary>
        /// Bütçe tasnifi tanım bilgisini veri tabanına yazan fonksiyon.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler</param>
        /// <param name="butceTasnif">Bütçe tasnif tanım bilgilerini tutan ButceTasnif yapısında parametre</param>
        /// <returns>Kaydetme işleminin sonucu, Sonuc yapısında döndürülür.</returns>
        public static Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, ButceTasnif butceTasnif)
        {
            Sonuc s = new Sonuc(true, string.Empty, string.Empty);
            if (!YazmaYetkisiVarMi(kullanan))
            {
                s.DegerAta(ButceHazirlikResI.BHMBT001);
                return s;
            }

            s = AlanlarGecerliMi(kullanan, butceTasnif);
            if (!s.islemSonuc)
            {
                return s;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, BHMv2Genel.VeriErisimAdBulSozluk(), kullanan.KimlikBul());
            TABBHMBUTCETASNIF tabButceTasnif = new TABBHMBUTCETASNIF(ve);

            ve.BeginTransaction();
            if (s.islemSonuc)
            {
                if (butceTasnif.siraNo == 0)
                {
                    butceTasnif.siraNo = tabButceTasnif.MaksSiraNoBul() + 1;
                    if (ve.hata == string.Empty)
                        s = tabButceTasnif.Ekle(butceTasnif);
                    else
                        s.DegerAta(ve.hata);
                }
                else
                {
                    //s = tabButceTasnif.TasnifIliskileriSil(butceTasnif);
                    //if (s.islemSonuc)
                    s = tabButceTasnif.Guncelle(butceTasnif);
                }
                //for (int i = 0; i < butceTasnif.senaryolar.Length && s.islemSonuc; i++)
                //{
                //    s = tabButceTasnif.TasnifIliskisiEkle(butceTasnif, i);
                //}
            }

            if (s.islemSonuc)
                ve.Commit();
            else
                ve.Rollback();

            ve.VeriErisimKapat();
            s.anahtar = butceTasnif.siraNo.ToString();
            return s;
        }

        /// <summary>
        /// Bütçe tasnif tanım bilgisini veri tabanından okuyan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler</param>
        /// <param name="tasnifSiraNo">Bilgileri okunacak tasnifin sıra numarası</param>
        /// <returns>Okunan tasnif tanım bilgisi, ButceTanif yapısında döndürülür.</returns>
        public static ButceTasnif Oku(VeriErisim veriErisim, Kullanici kullanan, int tasnifSiraNo)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, BHMv2Genel.VeriErisimAdBulSozluk(), kullanan.KimlikBul());
            TABBHMBUTCETASNIF tabBHMBUTCETASNIF = new TABBHMBUTCETASNIF(ve);

            ButceTasnif formIc = new ButceTasnif();
            IDataReader dr;
            dr = tabBHMBUTCETASNIF.Oku(tasnifSiraNo);
            if (dr == null)
            {
                ve.VeriErisimKapat();
                throw new Exception(ve.hata);
            }
            if (dr.Read())
            {
                formIc.siraNo = VeriErisim.DegerAl(dr, (int)TABBHMBUTCETASNIF.ENUMAlanlar.TASNIFNO, 0);
                formIc.ad = VeriErisim.DegerAl(dr, (int)TABBHMBUTCETASNIF.ENUMAlanlar.TASNIFAD, string.Empty);
                formIc.kur = VeriErisim.DegerAl(dr, (int)TABBHMBUTCETASNIF.ENUMAlanlar.KURKOD, string.Empty);
                formIc.tur = VeriErisim.DegerAl(dr, (int)TABBHMBUTCETASNIF.ENUMAlanlar.TUR, 0);
            }
            else
            {
                dr.Close();
                ve.VeriErisimKapat();
                throw new Exception(ButceHazirlikResI.BHMBT002);
            }
            dr.Close();
            formIc.senaryolar = TNS.BHMv2.IsKurallari.BHMv2ButceSenaryo.SenaryolariBul(ve, formIc.siraNo);
            ve.VeriErisimKapat();
            return formIc;
        }

        /// <summary>
        /// Veri tabanında kayıtlı tüm tasniflerin listesini döndüren fonksiyon.
        /// Hata olursa Exception oluşturur.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="kurKod">Hangi birime ait tasniflerin listeleneceği bilgisi.</param>
        /// <param name="tur">Hangi türdeki tasnifler listelenecek.</param>
        /// <returns>Okunan tasnif tanım bilgileri, ButceTanif yapıları içeren dizide döndürülür.</returns>
        public static ButceTasnif[] Listele(VeriErisim veriErisim, Kullanici kullanan, string kurKod, int tur)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, BHMv2Genel.VeriErisimAdBulSozluk(), kullanan.KimlikBul());
            TABBHMBUTCETASNIF tabTasnif = new TABBHMBUTCETASNIF(ve);

            IDataReader dr;
            ArrayList al;
            dr = tabTasnif.Listele(kurKod, tur);
            if (dr == null)
            {
                ve.VeriErisimKapat();
                throw new Exception(ve.hata);
            }
            al = new ArrayList();
            while (dr.Read())
            {
                ButceTasnif detay = new ButceTasnif();
                detay.siraNo = VeriErisim.DegerAl(dr, (int)TABBHMBUTCETASNIF.ENUMAlanlar.TASNIFNO, 0);
                detay.ad = VeriErisim.DegerAl(dr, (int)TABBHMBUTCETASNIF.ENUMAlanlar.TASNIFAD, string.Empty);
                detay.kur = VeriErisim.DegerAl(dr, (int)TABBHMBUTCETASNIF.ENUMAlanlar.KURKOD, "");
                detay.tur = VeriErisim.DegerAl(dr, (int)TABBHMBUTCETASNIF.ENUMAlanlar.TUR, 0);
                al.Add(detay);
            }
            dr.Close();
            foreach (ButceTasnif bt in al)
            {
                bt.senaryolar = TNS.BHMv2.IsKurallari.BHMv2ButceSenaryo.SenaryolariBul(ve, bt.siraNo);
            }
            ve.VeriErisimKapat();
            return (ButceTasnif[])al.ToArray(typeof(ButceTasnif));
        }

        internal static Sonuc Sil(VeriErisim veriErisim, Kullanici kullanan, int tasnifSiraNo)
        {
            Sonuc s = new Sonuc(true, string.Empty, string.Empty);
            if (!YazmaYetkisiVarMi(kullanan))
            {
                s.DegerAta(ButceHazirlikResI.BHMBT001);
                return s;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, BHMv2Genel.VeriErisimAdBulSozluk(), kullanan.KimlikBul());

            TABBHMBUTCETASNIF tabBT = new TABBHMBUTCETASNIF(ve);
            //tabBHMBUTCETASNIFSENARYO
            TABBHMBUTCETASNIFHIYERARSI tabBTH = new TABBHMBUTCETASNIFHIYERARSI(ve);
            TABBHMBUTCETASNIFODENEK tabBTO = new TABBHMBUTCETASNIFODENEK(ve);
            TABBHMBUTCETASNIFTERTIPSABLON tabBTT = new TABBHMBUTCETASNIFTERTIPSABLON(ve);

            ve.BeginTransaction();

            s = tabBTT.Sil(tasnifSiraNo);
            if (s.islemSonuc)
                s = tabBTO.Sil(tasnifSiraNo);
            if (s.islemSonuc)
                s = tabBTH.Sil(tasnifSiraNo);
            if (s.islemSonuc)
                s = tabBT.SilSenaryo(tasnifSiraNo);
            if (s.islemSonuc)
                s = tabBT.Sil(tasnifSiraNo);

            if (s.islemSonuc)
                ve.Commit();
            else
                ve.Rollback();
            return s;
        }

        internal static ButceTasnif SenaryoEkle(VeriErisim veriErisim, Kullanici kullanan, ButceTasnif butceTasnif, ButceSenaryo butceSenaryo)
        {
            if (!YazmaYetkisiVarMi(kullanan))
            {
                throw new Exception(ButceHazirlikResI.BHMBT001);
            }

            Sonuc s;
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, BHMv2Genel.VeriErisimAdBulSozluk(), kullanan.KimlikBul());
            ve.BeginTransaction();
            if (string.IsNullOrEmpty(butceSenaryo.kod))
            {
                s = BHMv2ButceSenaryo.Kaydet(veriErisim, kullanan, butceSenaryo);
                if (!s.islemSonuc)
                {
                    ve.Rollback();
                    ve.VeriErisimKapat();
                    throw new Exception(s.hataStr);
                }
                butceSenaryo.kod = s.anahtar;
            }

            TABBHMBUTCETASNIF tabButceTasnif = new TABBHMBUTCETASNIF(ve);

            s = tabButceTasnif.TasnifIliskisiSil(butceTasnif.siraNo, butceSenaryo.kod);

            if (s.islemSonuc)
            {
                int siraNo = tabButceTasnif.MaksSiraNoBul(butceTasnif.siraNo);
                s = tabButceTasnif.TasnifIliskisiEkle(butceTasnif.siraNo, siraNo + 1, butceSenaryo.kod);
            }

            if (s.islemSonuc)
                ve.Commit();
            else
            {
                ve.Rollback();
                ve.VeriErisimKapat();
                throw new Exception(s.hataStr);
            }
            ButceTasnif yeni = Oku(veriErisim, kullanan, butceTasnif.siraNo);
            ve.VeriErisimKapat();
            return yeni;
        }

        internal static Sonuc SenaryolarAta(VeriErisim veriErisim, Kullanici kullanan, ButceTasnif butceTasnif, ButceSenaryo[] bsler)
        {
            Sonuc sonuc = new Sonuc(true, "", "");
            if (!YazmaYetkisiVarMi(kullanan))
            {
                sonuc.DegerAta(ButceHazirlikResI.BHMBT001);
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, BHMv2Genel.VeriErisimAdBulSozluk(), kullanan.KimlikBul());
            ve.BeginTransaction();
            TABBHMBUTCETASNIF tabButceTasnif = new TABBHMBUTCETASNIF(ve);

            sonuc = tabButceTasnif.SilSenaryo(butceTasnif.siraNo);

            for (int i = 0; i < bsler.Length && sonuc.islemSonuc; i++)
            {
                sonuc = tabButceTasnif.TasnifIliskisiEkle(butceTasnif.siraNo, i + 1, bsler[i].kod);
            }

            if (sonuc.islemSonuc)
                ve.Commit();
            else
            {
                ve.Rollback();
            }
            ve.VeriErisimKapat();
            return sonuc;
        }

        internal static ButceTasnif SenaryoSil(VeriErisim veriErisim, Kullanici kullanan, ButceTasnif butceTasnif, ButceSenaryo butceSenaryo)
        {
            if (!YazmaYetkisiVarMi(kullanan))
            {
                throw new Exception(ButceHazirlikResI.BHMBT001);
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, BHMv2Genel.VeriErisimAdBulSozluk(), kullanan.KimlikBul());
            TABBHMBUTCETASNIF tabButceTasnif = new TABBHMBUTCETASNIF(ve);

            ve.BeginTransaction();
            Sonuc s = tabButceTasnif.TasnifIliskisiSil(butceTasnif.siraNo, butceSenaryo.kod);

            if (s.islemSonuc)
                ve.Commit();
            else
            {
                ve.Rollback();
                throw new Exception(s.hataStr);
            }
            return Oku(veriErisim, kullanan, butceTasnif.siraNo);
        }

        internal static int OnayBul(VeriErisim veriErisim, int tasnifSiraNo, int yil, string kurKod, string asamaKod)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, BHMv2Genel.VeriErisimAdBulSozluk(), null);
            TABBHMBUTCETASNIF tabButceTasnif = new TABBHMBUTCETASNIF(ve);

            int s = tabButceTasnif.OnayKod(tasnifSiraNo, yil, kurKod, asamaKod);

            ve.VeriErisimKapat();

            return s;
        }

        internal static Sonuc Onayla(VeriErisim veriErisim, Kullanici kullanan, int tasnifSiraNo, int yil, string kurKod, string asamaKod, int kaldir)
        {
            if (!YazmaYetkisiVarMi(kullanan))
            {
                throw new Exception(ButceHazirlikResI.BHMBT001);
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, BHMv2Genel.VeriErisimAdBulSozluk(), kullanan.KimlikBul());
            TABBHMBUTCETASNIF tabButceTasnif = new TABBHMBUTCETASNIF(ve);

            ve.BeginTransaction();
            Sonuc s = tabButceTasnif.OnayKodSil(tasnifSiraNo, yil, kurKod, asamaKod);
            if (s.islemSonuc && kaldir!=1)
                s = tabButceTasnif.OnayKodYaz(tasnifSiraNo, yil, kurKod, asamaKod, 1);

            if (s.islemSonuc)
                ve.Commit();
            else
                ve.Rollback();
            ve.VeriErisimKapat();
            return s;
        }
    }
}

/*

        //public static Sonuc Sil(VeriErisim veriErisim, Kullanici kullanan, TNSCollection kodlar)
        //{
        //    Sonuc s = new Sonuc(true, string.Empty, string.Empty);
        //    if (kodlar.Count <= 0)
        //    {
        //        s.islemSonuc = false;
        //        s.hataStr = "Silmek istediğiniz Tertip Gruplarını seçiniz.";
        //        return s;
        //    }

        //    VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, BHMv2Genel.VeriErisimAdBulSozluk(), kullanan.KimlikBul());
        //    TABBHMBUTCETASNIF tabTasnif = new TABBHMBUTCETASNIF(ve);

        //    ve.BeginTransaction();

        //    for (int i = 0; i < kodlar.Count; i++)
        //    {
        //        ButceTasnif form = new ButceTasnif();
        //        form.siraNo = OrtakFonksiyonlar.ConvertToInt(kodlar[i], 0);

        //        if (form.siraNo <= 0)
        //            s.hataStr = "Bütçe Tasnif no  boş bırakılamaz!<br>";
        //        if (!string.IsNullOrEmpty(s.hataStr))
        //        {
        //            s.islemSonuc = false;
        //            break;
        //        }

        //        if (s.islemSonuc)
        //            s = tabTasnif.Sil(form);

        //        if (!s.islemSonuc)
        //            break;
        //    }

        //    if (s.islemSonuc)
        //    {
        //        ve.Commit();
        //        s.bilgiStr = "Silme işlemi başarıyla sonuçlanmıştır.";
        //    }
        //    else
        //    {
        //        ve.Rollback();
        //    }

        //    ve.VeriErisimKapat();
        //    return s;
        //}

*/