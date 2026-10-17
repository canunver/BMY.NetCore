using System;
using System.Collections;
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
    public class MUHMakbuzServis
    {
        /// <summary>
        /// Makbuz kaydet.
        /// </summary>
        /// <param name="ve">The veri erisim.</param>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="m">The m.</param>
        /// <returns></returns>
        public static Sonuc Kaydet(VeriErisim ve, Kullanici kullanan, Makbuz m)
        {
            Sonuc sonuc = new Sonuc(false, ButceMuhasebeResI.MUH001, "");

            if (!(Yetki.SistemYoneticisimi(kullanan) ||
                Yetki.MuhasebeYoneticisimi(kullanan) ||
                Yetki.MakbuzKullanicisimi(kullanan) ||
                kullanan.KullaniciTipiIceriyorMu(18)))//Ödeme Emri Yetkisi
            {
                sonuc.hataStr = ButceMuhasebeResI.MUH080;
                sonuc.islemSonuc = false;
                return sonuc;
            }
            else
            {
                if (!TNS.MUH.Yetki.MuhasebeBirimYetkiVarMi(kullanan, m.muhasebe.ToString(), m.birim))
                {
                    sonuc.hataStr = ButceMuhasebeResI.MUH081;
                    sonuc.islemSonuc = false;
                    return sonuc;
                }
            }

            ve = VeriErisim.VeriErisimBul(ve, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(kullanan));

            TABMakbuz tMakbuz = new TABMakbuz(ve);
            TABMakbuzHesap tMakbuzHesap = new TABMakbuzHesap(ve);
            TABHesapPlani th = new TABHesapPlani(ve);

            string hata = string.Empty;
            if (m.yil <= 0)
                hata = ButceMuhasebeResI.MUH082 + "<br>";
            if (m.muhasebe <= 0)
                hata += ButceMuhasebeResI.MUH083 + "<br>";
            if (string.IsNullOrEmpty(m.birim))
                hata += ButceMuhasebeResI.MUH084 + "<br>";
            if (m.makbuzTipi.kod <= 0)
                hata += ButceMuhasebeResI.MUH085 + "<br>";
            if ((string.IsNullOrEmpty(m.tcKimlikNoVergiNo) || OrtakFonksiyonlar.ConvertToDbl(m.tcKimlikNoVergiNo) == 0) && m.kisiAd == "")
                hata += "Ödeyenin kimlik numarasını giriniz!" + "<br>";
            if (m.makbuzTipiTutar < 0)
                hata += "Makbuz tipi tutarını giriniz!" + "<br>";
            if (string.IsNullOrEmpty(m.kisiAd))
                hata += ButceMuhasebeResI.MUH086 + "<br>";
            if (string.IsNullOrEmpty(m.duzenleyen))
                hata += ButceMuhasebeResI.MUH087 + "<br>";

            if (hata == "")
            {
                string kriter = m.tcKimlikNoVergiNo.Trim();
                if (kriter != "")//Cumhurbaşkanlığı elle giriş yapıyor. Kimlik No boş bırakılıyor
                {
                    ObjectArray bilgi = new MUHOdemeEmriMIFServis().AltHesapListeHazirla(kullanan, m.muhasebe, kriter);

                    if (bilgi.objeler.Count == 0)
                        hata += "<br>" + kriter + " vergi/kimlik No Firma tablosunda kayıtlı olduğuna emin olun. Firma listesinde yoksa kayıt edin";
                }
            }

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

            int satir = 0;
            foreach (MakbuzHesap mh in m.hesaplar)
            {
                satir++;
                HesapPlaniSatir hKod = new HesapPlaniSatir();
                if (mh.tutar < 0)
                    hata += "<br>" + string.Format("{0} kodlu hesap için girilen rakam sıfırdan küçük olamaz!", mh.hesapKod);
                if (!string.IsNullOrWhiteSpace(mh.hesapKod))
                {
                    hKod.yil = m.yil;
                    hKod.hesapKod = mh.hesapKod;
                    th.HesapKodOzellikAl(hKod);
                    if (hKod.hesapKod == "" || hKod.detay == false)
                        hata += "<br>" + string.Format(ButceMuhasebeResI.MUH090, mh.hesapKod);

                    if (hKod.tertip > 0 && string.IsNullOrEmpty(mh.tertip))
                        hata += "<br>" + string.Format(ButceMuhasebeResI.MUH155, satir, mh.hesapKod);

                    if (hKod.altHesapKod > 0 && string.IsNullOrEmpty(mh.altHesap))
                    {
                        mh.altHesap = m.tcKimlikNoVergiNo;
                        mh.altHesapAd = m.kisiAd;
                    }

                    if (hKod.altHesapKod > 0 && string.IsNullOrEmpty(mh.altHesap))
                        hata += "<br>" + string.Format(mh.hesapKod + " hesabı için alt hesap alanı boş bırakılamaz.");

                    if (mh.altHesap != "" && mh.altHesapAd == "")
                        hata += "<br>" + mh.altHesap + " alt hesabı hatalı. Firma tablosunda kayıtlı olduğuna emin olun. Firma listesinde yoksa kayıt edin";
                }
            }

            bool iptalBelge = false;
            if (!string.IsNullOrEmpty(m.yevmiyeNo))
            {
                m.durum = -1;
                Makbuz okunan = MakbuzOku(ve, kullanan, m.yil, m.muhasebe, m.yevmiyeNo);
                if (okunan.durum == 3)
                {
                    if (!Yetki.MuhasebeYoneticisimi(kullanan))
                        hata += "<br>Bu makbuz ödendi olarak işaretlenmiş, değişiklik yapılamaz.";

                    if (m.krediKartiIleOdenen < 0)
                        hata += "<br>Bu makbuz ödendi olarak işaretlenmiş, kredi kartı ile ödenen " + m.krediKartiIleOdenen + " olamaz.";
                }

                if (okunan.durum == 9) iptalBelge = true;
            }


            if (!string.IsNullOrEmpty(hata))
                sonuc.DegerAta(hata);
            else
            {
                ve.BeginTransaction();
                if (m.yevmiyeNo.Trim() == "")
                {
                    //Makbuza numara alınıyor
                    //**********************************************************
                    TABGenel tg = new TABGenel(ve);
                    string sayacKod = "MAKBUZ" + m.yil.ToString() + m.muhasebe.ToString();
                    m.yevmiyeNo = tg.BelgeNoAl(sayacKod, "000000");
                    //**********************************************************
                }

                if (m.durum > -1)
                {
                    //m.islemTarih.Yaz(DateTime.Today.Date);
                    m.durum = (int)ENUMBelgeDurumu.YENI;
                    if (m.krediKartiIleOdenen == -2)
                        m.krediKartiIleOdenen = 0;
                    else
                        m.krediKartiIleOdenen = -1;
                }

                if (iptalBelge)
                    m.durum = (int)ENUMBelgeDurumu.YENI;

                m.kullaniciKodu = kullanan.kullaniciKodu;
                sonuc = tMakbuz.Kaydet(m);
                if (sonuc.islemSonuc)
                {
                    Makbuz okunan = MakbuzOku(ve, kullanan, m.yil, m.muhasebe, m.yevmiyeNo);
                    m.makbuzTipi = okunan.makbuzTipi;
                    sonuc = HesaplananAlanlariYaz(ve, kullanan, m, Math.Round(m.krediKartiIleOdenen, 2));
                }
            }

            if (sonuc.islemSonuc)
            {
                sonuc.anahtar = m.yevmiyeNo;
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
        /// <param name="veriErisim">The veri erisim.</param>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="makbuzlar">makbuzlar</param>
        /// <returns></returns>
        public static Sonuc DurumGuncelle(VeriErisim veriErisim, Kullanici kullanan, TNSCollection makbuzlar)
        {
            Sonuc sonuc = new Sonuc(false, ButceMuhasebeResI.MUH001, "");

            if (!(Yetki.SistemYoneticisimi(kullanan) ||
                Yetki.MuhasebeYoneticisimi(kullanan)))
            {
                sonuc.hataStr = "Ödemesi yapılmış olan bir makbuzu İptal etme yetkiniz yoktur";
                sonuc.islemSonuc = false;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(kullanan));

            ve.BeginTransaction();
            TABMakbuz tMakbuz = new TABMakbuz(ve);

            foreach (Makbuz m in makbuzlar)
            {
                string hata = string.Empty;
                if (m.yil <= 0)
                    hata = ButceMuhasebeResI.MUH092 + "<br>";
                if (m.muhasebe <= 0)
                    hata += ButceMuhasebeResI.MUH093 + "<br>";
                if (string.IsNullOrEmpty(m.yevmiyeNo))
                    hata += ButceMuhasebeResI.MUH094 + "<br>";
                if (m.durum <= 0)
                    hata += ButceMuhasebeResI.MUH095 + "<br>";
                if (!string.IsNullOrEmpty(hata))
                {
                    sonuc.islemSonuc = false;
                    sonuc.hataStr = hata;
                    break;
                }

                int mevcutDurum = tMakbuz.MevcutDurumGetir(m);
                if (mevcutDurum != (int)ENUMBelgeDurumu.YENI)
                {
                    if (!(Yetki.SistemYoneticisimi(kullanan) || Yetki.MuhasebeYoneticisimi(kullanan)))
                    {
                        sonuc.islemSonuc = false;
                        sonuc.hataStr = string.Format(ButceMuhasebeResI.MUH096, m.yevmiyeNo, m.odemeEmriNo, mevcutDurum);
                        break;
                    }
                }

                sonuc = tMakbuz.DurumGuncelle(m);

                if (!sonuc.islemSonuc)
                    break;
            }

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

        internal static Sonuc MakbuzOdendi(Kullanici kullanan, Makbuz makbuz)
        {
            Sonuc sonuc = new Sonuc();
            string hata = string.Empty;
            if (makbuz.yil <= 0)
                hata = ButceMuhasebeResI.MUH092 + "<br>";
            if (makbuz.muhasebe <= 0)
                hata += ButceMuhasebeResI.MUH093 + "<br>";
            if (string.IsNullOrEmpty(makbuz.yevmiyeNo))
                hata += ButceMuhasebeResI.MUH094 + "<br>";
            makbuz.durum = (int)ENUMBelgeDurumu.MUHASEBATAGONDERILDI;
            if (makbuz.krediKartiIleOdenen < 0)
                hata += "Lütfen kredi kartı ile ödenen miktarı giriniz!" + "<br>";

            if (!string.IsNullOrEmpty(hata))
            {
                sonuc.DegerAta(hata);
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(kullanan));
            Makbuz okunan = MakbuzOku(ve, kullanan, makbuz.yil, makbuz.muhasebe, makbuz.yevmiyeNo);


            if (okunan == null)
                sonuc.DegerAta("Eski makbuz bulunamadı");
            else
            {
                if (okunan.durum == (int)ENUMBelgeDurumu.ONAYLI || okunan.durum == (int)ENUMBelgeDurumu.IPTAL)
                    sonuc.DegerAta("Onaylı veya iptal belgeler ödendi olarak işaretlenemez!");
                if (okunan.durum == 3)
                    sonuc.DegerAta("Bu makbuz ödendi olarak işaretlenmiş, tekrar ödendi işlemi yapılamaz.");
                if (Math.Round(makbuz.krediKartiIleOdenen, 2) > Math.Round(okunan.genelToplam, 2))
                    sonuc.DegerAta(string.Format("Lütfen kredi kartı ile ödenen miktarı, alınacak tutar olan {0}den küçük veya eşit giriniz!" + "<br>", okunan.genelToplam.ToString("#,##0.00")));
            }

            if (!sonuc.islemSonuc)
            {
                ve.VeriErisimKapat();
                return sonuc;
            }

            TABMakbuz tMakbuz = new TABMakbuz(ve);
            ve.BeginTransaction();

            if (sonuc.islemSonuc)
                sonuc = HesaplananAlanlariYaz(ve, kullanan, okunan, Math.Round(makbuz.krediKartiIleOdenen, 2));

            if (sonuc.islemSonuc)
                sonuc = tMakbuz.MakbuzOdendiIsaretle(makbuz);

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

        private static Sonuc HesaplananAlanlariYaz(VeriErisim ve, Kullanici kullanan, Makbuz makbuz, double krediKartiOdenen)
        {
            TABHesapPlani th = new TABHesapPlani(ve);
            double kurDegeri = 1;
            if (!string.IsNullOrWhiteSpace(makbuz.paraBirimiKod) && makbuz.paraBirimiKod != "TRY")
            {
                TNS.HRC.IHRCServis servisHRC = TNS.HRC.Arac.Tanimla();
                TNS.HRC.MerkezBankasiKurSinif kriter = new TNS.HRC.MerkezBankasiKurSinif();
                kriter.kurTarih = makbuz.islemTarih;
                TNS.HRC.MerkezBankasiKurSinif kurListe = servisHRC.MerkezBankKurOku(kullanan, kriter);
                foreach (TNS.HRC.MerkezBankasiKurDetaySinif kur in kurListe.satirlar)
                {
                    if (kur.paraBirim.kisaltma == makbuz.paraBirimiKod)
                    {
                        kurDegeri = OrtakFonksiyonlar.ConvertToDbl(kur.DegerBul("ALIS", 1));
                        break;
                    }
                }
            }

            Sonuc sonuc = new Sonuc();
            makbuz.krediKartiIleOdenen = krediKartiOdenen;

            foreach (MakbuzHesap mh in makbuz.hesaplar)
            {
                if (mh.hesaplananAlan == 1)
                {
                    mh.hesapKod = "";
                    mh.tutar = 0;
                }
            }

            double tutarKDVli = makbuz.makbuzTipiTutar * kurDegeri;
            //double kdvTutar = Math.Round(tutarKDVli - (tutarKDVli / (1 + (m.makbuzTipi.kdv * .01))), 2);
            double kdvTutar = Math.Round(tutarKDVli * makbuz.makbuzTipi.kdv / (100 + makbuz.makbuzTipi.kdv), 2);
            double tutarKDVsiz = tutarKDVli - kdvTutar;

            string hesapKod = makbuz.makbuzTipi.hesapKod;
            double hesapKodTutar = tutarKDVsiz;
            if (hesapKod != "" && hesapKodTutar > 0)
            {
                MakbuzHesap mht = new MakbuzHesap();
                mht.hesapKod = hesapKod;
                mht.tutar = hesapKodTutar;
                mht.hesaplananAlan = 1;
                makbuz.hesaplar.Add(mht);
            }

            string kdvHesapKod = makbuz.makbuzTipi.kdvHesapKod;
            double kdvHesapKodTutar = kdvTutar;
            if (kdvHesapKod != "" && kdvHesapKodTutar > 0)
            {
                MakbuzHesap mht = new MakbuzHesap();
                mht.hesapKod = kdvHesapKod;
                mht.tutar = kdvHesapKodTutar;
                mht.hesaplananAlan = 1;
                makbuz.hesaplar.Add(mht);
            }

            string krediHesapKod = makbuz.makbuzTipi.krediHesapKod;
            double krediHesapKodTutar = Math.Round(krediKartiOdenen * kurDegeri, 2);
            if (krediHesapKod != "" && krediHesapKodTutar > 0)
            {
                MakbuzHesap mht = new MakbuzHesap();
                mht.hesapKod = krediHesapKod;
                mht.tutar = krediHesapKodTutar;
                mht.hesaplananAlan = 1;
                makbuz.hesaplar.Add(mht);
            }

            string kasaHesapKod = makbuz.makbuzTipi.kasaHesapKod;
            double kasaHesapKodTutar = Math.Round(makbuz.genelToplam * kurDegeri, 2) - krediHesapKodTutar;
            if (kasaHesapKod != "" && kasaHesapKodTutar > 0)
            {
                MakbuzHesap mht = new MakbuzHesap();
                mht.hesapKod = kasaHesapKod;
                mht.tutar = kasaHesapKodTutar;
                mht.hesaplananAlan = 1;
                makbuz.hesaplar.Add(mht);
            }

            TABMakbuzHesap tMakbuzHesap = new TABMakbuzHesap(ve);

            sonuc = tMakbuzHesap.Sil(makbuz.yil, makbuz.muhasebe, makbuz.yevmiyeNo);
            int satir = 1;
            foreach (MakbuzHesap mh in makbuz.hesaplar)
            {
                if (mh.hesapKod.Trim() == "") continue;
                if (!sonuc.islemSonuc)
                    break;
                if (!string.IsNullOrWhiteSpace(mh.hesapKod) && mh.tutar > 0)
                    sonuc = tMakbuzHesap.Kaydet(makbuz, satir++, mh);
            }

            return sonuc;
        }

        private static Makbuz MakbuzOku(VeriErisim ve, Kullanici kullanan, int yil, int muhasebe, string makbuzNo)
        {
            Makbuz makbuz = new Makbuz(yil, muhasebe, makbuzNo);
            ObjectArray oa = Listele(ve, kullanan, makbuz);
            if (oa.sonuc.islemSonuc && oa.ObjeSayisi == 1)
            {
                return (Makbuz)oa.objeler[0];
            }
            return null;
        }

        /// <summary>
        /// Makbuz odeme emri guncelle.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="makbuzlar">makbuzlar</param>
        /// <returns></returns>
        public static Sonuc OdemeEmriGuncelle(Kullanici kullanan, ObjectArray makbuzlar)
        {
            Sonuc sonuc = new Sonuc(false, ButceMuhasebeResI.MUH001, "");

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(kullanan));

            ve.BeginTransaction();
            TABMakbuz tMakbuz = new TABMakbuz(ve);

            foreach (Makbuz m in makbuzlar.objeler)
            {
                string hata = string.Empty;
                if (m.yil <= 0)
                    hata = ButceMuhasebeResI.MUH092 + "<br>";
                if (m.muhasebe <= 0)
                    hata += ButceMuhasebeResI.MUH093 + "<br>";
                if (string.IsNullOrEmpty(m.yevmiyeNo))
                    hata += ButceMuhasebeResI.MUH094 + "<br>";
                if (m.durum <= 0)
                    hata += ButceMuhasebeResI.MUH095 + "<br>";
                if (!string.IsNullOrEmpty(hata))
                {
                    sonuc.islemSonuc = false;
                    sonuc.hataStr = hata;
                    break;
                }

                sonuc = tMakbuz.OdemeEmriGuncelle(m);

                if (!sonuc.islemSonuc)
                    break;
            }

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

        /// <summary>
        /// Makbuz listele.
        /// </summary>
        /// <param name="veriErisim">The veri erisim.</param>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="makbuz">makbuz</param>
        /// <returns></returns>
        public static ObjectArray Listele(VeriErisim veriErisim, Kullanici kullanan, Makbuz makbuz)
        {
            ObjectArray mler = new ObjectArray();
            
            string kodYapisiHesap = TNS.MUH.Arac.DegiskenDegerBul(0, "/KODYAPI/KOD8/KODYAPI");

            if (!(Yetki.SistemYoneticisimi(kullanan) ||
                Yetki.MuhasebeYoneticisimi(kullanan) ||
                Yetki.MakbuzKullanicisimi(kullanan) ||
                kullanan.KullaniciTipiIceriyorMu(18)))//Ödeme Emri Yetkisi
            {
                mler.sonuc.hataStr = ButceMuhasebeResI.MUH080;
                mler.sonuc.islemSonuc = false;
                return mler;
            }
            else
            {
                //if (!TNS.MUH.Yetki.MuhasebeBirimYetkiVarMi(kullanan, makbuz.muhasebe.ToString(), makbuz.birim))//birim gelmediği için EGO listeyelemiyordu. 23032020 Melih
                //{
                //    mler.sonuc.hataStr = ButceMuhasebeResI.MUH098;
                //    mler.sonuc.islemSonuc = false;
                //    return mler;
                //}
            }

            Hashtable birimAd = new Hashtable();
            Hashtable muhasebeAd = new Hashtable();
            string muhasebe = "";
            string birim = "";

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(kullanan));
            VeriErisim ve2 = VeriErisim.VeriErisimBul(null, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABMakbuz tMakbuz = new TABMakbuz(ve);

            IDataReader dr = tMakbuz.Listele(makbuz);
            if (string.IsNullOrEmpty(tMakbuz.Hata()))
            {
                Tertip t = new Tertip(makbuz.yil, ENUMTabloKod.TABOdemeEmri);
                Makbuz m = null;

                while (dr.Read())
                {
                    if (m == null
                     || m.yil != VeriErisim.DegerAl(dr, 0, 0)
                     || m.muhasebe != VeriErisim.DegerAl(dr, 1, 0)
                     || m.yevmiyeNo != VeriErisim.DegerAl(dr, 2, string.Empty))
                    {
                        m = new Makbuz();
                        m.yil = VeriErisim.DegerAl(dr, 0, 0);
                        m.muhasebe = VeriErisim.DegerAl(dr, 1, 0);
                        m.yevmiyeNo = VeriErisim.DegerAl(dr, 2, string.Empty);
                        m.birim = VeriErisim.DegerAl(dr, 3, string.Empty);
                        m.kisiAd = VeriErisim.DegerAl(dr, 5, string.Empty);
                        m.makbuzTipiTutar = VeriErisim.DegerAl(dr, 6, (double)0);
                        m.aciklama = VeriErisim.DegerAl(dr, 7, string.Empty);
                        m.duzenleyen = VeriErisim.DegerAl(dr, 8, string.Empty);
                        m.unvan = VeriErisim.DegerAl(dr, 9, string.Empty);
                        m.islemTarih = VeriErisim.DegerAl(dr, 10);
                        m.durum = VeriErisim.DegerAl(dr, 11, 0);
                        m.odemeEmriNo = VeriErisim.DegerAl(dr, 12, string.Empty);
                        m.kullaniciKodu = VeriErisim.DegerAl(dr, 13, string.Empty);
                        m.tcKimlikNoVergiNo = VeriErisim.DegerAl(dr, 14, string.Empty);
                        m.krediKartiIleOdenen = VeriErisim.DegerAl(dr, 15, (double)0);
                        m.paraBirimiKod = VeriErisim.DegerAl(dr, 16, string.Empty);

                        m.makbuzTipi.kod = VeriErisim.DegerAl(dr, 4, 0);
                        m.makbuzTipi.aciklama = VeriErisim.DegerAl(dr, 17, string.Empty);
                        m.makbuzTipi.hesapKod = VeriErisim.DegerAl(dr, 18, string.Empty);
                        m.makbuzTipi.kdv = VeriErisim.DegerAl(dr, 19, 0);
                        m.makbuzTipi.kdvHesapKod = VeriErisim.DegerAl(dr, 20, string.Empty);
                        m.makbuzTipi.kasaHesapKod = VeriErisim.DegerAl(dr, 21, string.Empty);
                        m.makbuzTipi.yansitmaHesapKod = VeriErisim.DegerAl(dr, 22, string.Empty);
                        m.makbuzTipi.krediHesapKod = VeriErisim.DegerAl(dr, 23, string.Empty);

                        m.makbuzTipi.hesapKod = OrtakFonksiyonlar.KodNoktaKoy(kodYapisiHesap, m.makbuzTipi.hesapKod);
                        m.makbuzTipi.kdvHesapKod = OrtakFonksiyonlar.KodNoktaKoy(kodYapisiHesap, m.makbuzTipi.kdvHesapKod);
                        m.makbuzTipi.kasaHesapKod = OrtakFonksiyonlar.KodNoktaKoy(kodYapisiHesap, m.makbuzTipi.kasaHesapKod);
                        m.makbuzTipi.yansitmaHesapKod = OrtakFonksiyonlar.KodNoktaKoy(kodYapisiHesap, m.makbuzTipi.yansitmaHesapKod);
                        m.makbuzTipi.krediHesapKod = OrtakFonksiyonlar.KodNoktaKoy(kodYapisiHesap, m.makbuzTipi.krediHesapKod);

                        if (muhasebeAd[m.muhasebe.ToString()] == null)
                        {
                            muhasebe = MUHServis.UzayYaziDegerGetir(ve2, kullanan, "MUHMUHASEBE", m.muhasebe.ToString(), true, "MUHASEBEAD");
                            muhasebeAd[m.muhasebe.ToString()] = muhasebe;
                        }
                        else
                            muhasebe = (string)muhasebeAd[m.muhasebe.ToString()];

                        if (!string.IsNullOrEmpty(m.birim))
                        {
                            if (birimAd[m.birim + "-" + m.muhasebe.ToString().Trim()] == null)
                            {
                                birim = MUHServis.UzayYaziDegerGetir(ve2, kullanan, "MUHMUHASEBEBIRIM", m.birim + "-" + m.muhasebe.ToString(), true, "BIRIMAD");
                                birimAd[m.birim + "-" + m.muhasebe.ToString().Trim()] = birim;
                            }
                            else
                                birim = (string)birimAd[m.birim + "-" + m.muhasebe.ToString().Trim()];
                        }

                        m.muhasebeAd = muhasebe;
                        m.birimAd = birim;

                        mler.objeler.Add(m);
                    }

                    MakbuzHesap mh = new MakbuzHesap();
                    mh.hesapKod = VeriErisim.DegerAl(dr, 24, string.Empty);
                    mh.hesapKod = OrtakFonksiyonlar.KodNoktaKoy(kodYapisiHesap, mh.hesapKod);
                    mh.tutar = VeriErisim.DegerAl(dr, 25, (double)0);
                    mh.hesaplananAlan = VeriErisim.DegerAl(dr, 26, 0);
                    mh.altHesap = VeriErisim.DegerAl(dr, 27, "");
                    mh.hesapAd = VeriErisim.DegerAl(dr, 28, "");

                    mh.altHesapAd = VeriErisim.DegerAl(dr, 29, "");
                    if (!string.IsNullOrEmpty(mh.altHesap) && string.IsNullOrEmpty(mh.altHesapAd))
                        mh.altHesapAd = VeriErisim.DegerAl(dr, 30, "");

                    mh.tertip = VeriErisim.DegerAl(dr, 31, "");

                    m.hesaplar.Add(mh);
                }
                dr.Close();
            }
            else
            {
                mler.sonuc.hataStr = tMakbuz.Hata();
                mler.sonuc.islemSonuc = false;
            }

            ve2.VeriErisimKapat();
            ve.VeriErisimKapat();
            return mler;
        }

        /// <summary>
        /// Listeles the mizan.
        /// </summary>
        /// <param name="veriErisim">The veri erisim.</param>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="makbuz">The makbuz.</param>
        /// <returns></returns>
        public static ObjectArray ListeleMizan(VeriErisim veriErisim, Kullanici kullanan, Makbuz makbuz)
        {
            string kodYapisiHesap = TNS.MUH.Arac.DegiskenDegerBul(0, "/KODYAPI/KOD8/KODYAPI");

            ObjectArray mler = new ObjectArray();

            if (!(Yetki.SistemYoneticisimi(kullanan) ||
                Yetki.MuhasebeYoneticisimi(kullanan) ||
                Yetki.MakbuzKullanicisimi(kullanan) ||
                kullanan.KullaniciTipiIceriyorMu(18)))//Ödeme Emri Yetkisi
            {
                mler.sonuc.hataStr = ButceMuhasebeResI.MUH080;
                mler.sonuc.islemSonuc = false;
                return mler;
            }
            else
            {
                if (!TNS.MUH.Yetki.MuhasebeBirimYetkiVarMi(kullanan, makbuz.muhasebe.ToString(), makbuz.birim))
                {
                    mler.sonuc.hataStr = ButceMuhasebeResI.MUH098;
                    mler.sonuc.islemSonuc = false;
                    return mler;
                }
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABMakbuz tMakbuz = new TABMakbuz(ve);

            IDataReader dr = tMakbuz.ListeleMizan(makbuz);
            if (string.IsNullOrEmpty(tMakbuz.Hata()))
            {
                Makbuz m = new Makbuz();

                while (dr.Read())
                {
                    MakbuzHesap mh = new MakbuzHesap();
                    mh.hesapKod = VeriErisim.DegerAl(dr, 0, string.Empty);
                    mh.hesapKod = OrtakFonksiyonlar.KodNoktaKoy(kodYapisiHesap, mh.hesapKod);
                    mh.hesapAd = VeriErisim.DegerAl(dr, 1, "");

                    mh.altHesap = VeriErisim.DegerAl(dr, 2, string.Empty);

                    mh.tutar = VeriErisim.DegerAl(dr, 3, (double)0);

                    mh.altHesapAd = VeriErisim.DegerAl(dr, 4, string.Empty);
                    if (string.IsNullOrEmpty(mh.altHesapAd))
                        mh.altHesapAd = VeriErisim.DegerAl(dr, 5, string.Empty);

                    m.hesaplar.Add(mh);
                }
                mler.objeler.Add(m);
                dr.Close();
            }
            else
            {
                mler.sonuc.hataStr = tMakbuz.Hata();
                mler.sonuc.islemSonuc = false;
            }

            ve.VeriErisimKapat();
            return mler;
        }

        /// <summary>
        /// Listeles the birim.
        /// </summary>
        /// <param name="veriErisim">The veri erisim.</param>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="makbuz">The makbuz.</param>
        /// <returns></returns>
        public static ObjectArray ListeleBirim(VeriErisim veriErisim, Kullanici kullanan, Makbuz makbuz)
        {
            ObjectArray mler = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABMakbuz tMakbuz = new TABMakbuz(ve);

            IDataReader dr = tMakbuz.ListeleBirim(makbuz);
            if (string.IsNullOrEmpty(tMakbuz.Hata()))
            {
                Makbuz m = null;

                while (dr.Read())
                {
                    m = new Makbuz();
                    m.birim = VeriErisim.DegerAl(dr, 0, string.Empty);
                    mler.objeler.Add(m);
                }
                dr.Close();
            }
            else
            {
                mler.sonuc.hataStr = tMakbuz.Hata();
                mler.sonuc.islemSonuc = false;
            }

            ve.VeriErisimKapat();
            return mler;
        }

        public static ObjectArray ListeleKisi(VeriErisim veriErisim, Kullanici kullanan, Makbuz makbuz)
        {
            ObjectArray mler = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABMakbuz tMakbuz = new TABMakbuz(ve);

            IDataReader dr = tMakbuz.ListeleKisi(makbuz);
            if (string.IsNullOrEmpty(tMakbuz.Hata()))
            {
                Makbuz m = null;

                while (dr.Read())
                {
                    m = new Makbuz();
                    m.tcKimlikNoVergiNo = VeriErisim.DegerAl(dr, 0, string.Empty);
                    m.kisiAd = VeriErisim.DegerAl(dr, 1, string.Empty);
                    mler.objeler.Add(m);
                }
                dr.Close();
            }
            else
            {
                mler.sonuc.hataStr = tMakbuz.Hata();
                mler.sonuc.islemSonuc = false;
            }

            ve.VeriErisimKapat();
            return mler;
        }

        public static Sonuc MakbuzAciklamaKaydet(VeriErisim ve, Kullanici kullanan, MakbuzAciklama bilgi)
        {
            Sonuc sonuc = new Sonuc();

            sonuc = MakbuzAciklamaKontrol("KAYDET", kullanan, bilgi);
            if (!sonuc.islemSonuc)
                return sonuc;

            ve = VeriErisim.VeriErisimBul(ve, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(kullanan));
            ve.BeginTransaction();

            TABMakbuz to = new TABMakbuz(ve);

            if (bilgi.kod > 0)
                sonuc = to.MakbuzAciklamaGuncelle(bilgi);
            else
            {
                if (bilgi.kod == 0)
                    bilgi.kod = to.EnBuyukAciklamaKod();

                sonuc = to.MakbuzAciklamaKaydet(bilgi);
            }

            if (sonuc.islemSonuc)
                ve.Commit();
            else
                ve.Rollback();
            return sonuc;
        }

        public static ObjectArray MakbuzAciklamaListele(VeriErisim ve, MakbuzAciklama bilgi)
        {
            ObjectArray liste = new ObjectArray();

            ve = VeriErisim.VeriErisimBul(ve, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(null));
            TABMakbuz to = new TABMakbuz(ve);
            IDataReader dr = null;

            try
            {
                dr = to.MakbuzAciklamaListele(bilgi);
            }
            catch (Exception e)
            {
                liste.sonuc.islemSonuc = false;
                liste.sonuc.hataStr = e.Message;
                ve.VeriErisimKapat();
                return liste;
            }

            if (dr == null || !string.IsNullOrEmpty(to.Hata()))
            {
                liste.sonuc.islemSonuc = false;
                liste.sonuc.hataStr = to.Hata();
                ve.VeriErisimKapat();
                return liste;
            }

            while (dr.Read())
            {
                MakbuzAciklama t = new MakbuzAciklama();
                t.kod = VeriErisim.DegerAl(dr, (int)TABMakbuz.ENUMAlanlarAciklama.KOD, 0);
                t.aciklama = VeriErisim.DegerAl(dr, (int)TABMakbuz.ENUMAlanlarAciklama.ACIKLAMA, "");
                t.makbuzTipiKod = VeriErisim.DegerAl(dr, (int)TABMakbuz.ENUMAlanlarAciklama.MAKBUZTIPIKOD, 0);
                liste.Ekle(t);
            }
            dr.Close();

            ve.VeriErisimKapat();
            return liste;
        }

        public static Sonuc MakbuzAciklamaSil(VeriErisim ve, Kullanici kullanan, MakbuzAciklama bilgi)
        {
            Sonuc sonuc = new Sonuc(false, "", "");

            sonuc = MakbuzAciklamaKontrol("SIL", kullanan, bilgi);
            if (!sonuc.islemSonuc)
                return sonuc;

            ve = VeriErisim.VeriErisimBul(ve, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(kullanan));
            ve.BeginTransaction();

            TABMakbuz to = new TABMakbuz(ve);

            sonuc = to.MakbuzAciklamaSil(bilgi);

            if (sonuc.islemSonuc)
                ve.Commit();
            else
                ve.Rollback();

            ve.VeriErisimKapat();
            return sonuc;
        }

        private static Sonuc MakbuzAciklamaKontrol(string tip, Kullanici kullanan, MakbuzAciklama bilgi)
        {
            Sonuc sonuc = new Sonuc();

            if (tip == "SIL")
            {
                if (bilgi.kod == 0)
                    sonuc.hataStr = "Silinecek kaydı seçiniz.";
            }

            if (tip == "KAYDET" || tip == "GUNCELLE")
            {
                if (string.IsNullOrEmpty(bilgi.aciklama))
                    sonuc.hataStr = "Makbuz Açıklama Boş bırakılamaz." + "<br<";

            }

            if (!string.IsNullOrEmpty(sonuc.hataStr))
                sonuc.islemSonuc = false;
            else
                sonuc.islemSonuc = true;

            return sonuc;
        }
    }
}