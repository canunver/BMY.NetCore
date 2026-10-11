using System;
using System.Data;
//using MUHServisC.ebutce.ButceUygulama;
//using MUHServisC.ebutce.Listeler;
//using MUHServisC.ebutce.OdenekTenkis;
using TNS.KYM;
using TNS.MUH.IsKurallari.Veri;
using TNS.Veri;
using System.Collections;
using System.Threading;
using System.Threading.Tasks;

namespace TNS.MUH.IsKurallari
{
    /// <summary>
    /// Program Bütçe işlemlerinin yapıldığı sınıf
    /// </summary>
    public class MUHProgramButceIslemleri
    {
        //static MUHServisC.ebutce.OdenekTenkis.OdenekGondermeTenkisServis servisMUH = new OdenekGondermeTenkisServis();
        //static MUHServisC.ebutce.Listeler.Listeler servis2MUH = new Listeler();
        static MUHServisC.HarcamaMuhasebatServisi.TertipHarcamasiSorgulaPortService muhServis = new MUHServisC.HarcamaMuhasebatServisi.TertipHarcamasiSorgulaPortService();
        //static MUHServisC.ebutce.ButceUygulama.ButceUygulama servis3MUH = new ButceUygulama();
        public static string ebutceUser = "";
        public static string ebutcePassword = "";

        static MUHProgramButceIslemleri()
        {
            // .NET 8: ServicePointManager.CertificatePolicy yok (eski kod her sertifikayi kabul ediyordu); SoapIstemci HttpClient kullanir, sertifikalar normal dogrulanir.
            //servisMUH.Proxy = OrtakFonksiyonlar.ProxyAyarla();
            //servis2MUH.Proxy = OrtakFonksiyonlar.ProxyAyarla();
            //servis3MUH.Proxy = OrtakFonksiyonlar.ProxyAyarla();

            //servisMUH.Timeout = 20 * 60 * 1000;
            //servis2MUH.Timeout = 20 * 60 * 1000;
            //servis3MUH.Timeout = 20 * 60 * 1000;

            EButceParolaBilgileri();
        }

        public static Sonuc EbutceAktarimBilgiOku()
        {
            EButceParolaBilgileri();
            Sonuc s = new Sonuc();
            s.anahtar = ebutceUser;
            s.bilgiStr = ebutcePassword;
            return s;
        }

        /// <summary>
        /// Ebütçe kullanıcısının parola bilgilerini okur
        /// </summary>
        public static void EButceParolaBilgileri()
        {
            if (string.IsNullOrEmpty(ebutceUser))
                EbutceParolaOku(ref ebutceUser, ref ebutcePassword);

            if (string.IsNullOrEmpty(ebutceUser))
            {
                ebutceUser = System.Configuration.ConfigurationManager.AppSettings.Get("ebutceUser") + "";
                ebutcePassword = OrtakFonksiyonlar.SifreliCozOrtakAnahtar(System.Configuration.ConfigurationManager.AppSettings.Get("ebutcePassword") + "");
            }
        }

        /// <summary>
        /// Muhasebata ait harcama rakamlarını aktarma işlemi
        /// </summary>
        /// <param name="yil">Yıl</param>
        /// <param name="ay">ay</param>
        /// <param name="kurum">Kurum kodu</param>
        /// <returns></returns>
        //public Sonuc ebutceMuhasebatKontrol(Kullanici kullanan, string yil, string ay, string kurum)
        //{
        //    Sonuc sonuc = new Sonuc(false, "", "");
        //    int iYil = OrtakFonksiyonlar.ConvertToInt(yil, 0);
        //    if (iYil >= 2019)
        //    {
        //        sonuc.DegerAta("2019 yılından itibaren ebütçe harcama servisi kapatılmıştır!");
        //        return sonuc;
        //    }
        //    EButceParolaBilgileri();

        //    if (kurum == "")
        //        kurum = TNS.MUH.Yetki.VarsayilanKurumBul();

        //    VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(null));
        //    TABMuhasebat to = new TABMuhasebat(ve);

        //    ve.BeginTransaction();

        //    sonuc.islemSonuc = true;
        //    Hashtable birimHash = new Hashtable();
        //    TNSDateTime dt = new TNSDateTime(DateTime.Now);
        //    for (int i = 1; i < 10; i++)
        //    {
        //        MUHServisC.ebutce.Listeler.MuhasebatBilgi[] mler = new MuhasebatBilgi[0];
        //        try
        //        {
        //            mler = servis2MUH.MuhasebatListele(ebutceUser, ebutcePassword, yil, ay, kurum, i.ToString());

        //            if (mler.Length > 0 && i == 1)
        //                sonuc = to.Sil(iYil, OrtakFonksiyonlar.ConvertToInt(ay, 0), "", 0, "");
        //        }
        //        catch (Exception e) { sonuc.hataStr = e.Message; ve.Rollback(); ve.VeriErisimKapat(); return sonuc; }
        //        foreach (MuhasebatBilgi m in mler)
        //        {
        //            if (m.borc > 0 || m.alacak > 0)
        //            {
        //                if (m.kur.Length < 8)
        //                {
        //                    OrtakFonksiyonlar.HataDosyaYaz("MuhasebatAktar_KurKod" + dt.ToString() + ".txt", m.yil + "," + m.ay + "," + m.muhasebe + "," + m.birim + "," + m.hesapKod + "," + m.kur + "." + m.fon + "." + m.fin + "." + m.eko + ": " + m.borc + " - " + m.alacak, false);
        //                    continue;
        //                }
        //                if (m.eko.Length > 6)
        //                {
        //                    OrtakFonksiyonlar.HataDosyaYaz("MuhasebatAktar_EkoKod" + dt.ToString() + ".txt", m.yil + "," + m.ay + "," + m.muhasebe + "," + m.birim + "," + m.hesapKod + "," + m.kur + "." + m.fon + "." + m.fin + "." + m.eko + ": " + m.borc + " - " + m.alacak, false);
        //                    continue;
        //                }

        //                if (iYil >= 2018)
        //                {
        //                    if (m.birim.Length > 3)
        //                    {
        //                        string birKod;
        //                        int sayKod;
        //                        VergiNodanBirime(kullanan, birimHash, m.birim, m.kur, out sayKod, out birKod);

        //                        if (sayKod <= 0 || birKod.Length != 3)
        //                        {
        //                            OrtakFonksiyonlar.HataDosyaYaz("MuhasebatAktar" + dt.ToString() + ".txt", "muhasebe - birim kod geçerli değil: vergi No:" + m.birim, false);
        //                            continue;
        //                        }

        //                        m.birim = birKod;
        //                        m.muhasebe = sayKod;
        //                    }
        //                }

        //                sonuc = to.Kaydet(m.yil, m.ay, m.muhasebe, m.birim, m.hesapKod.Replace(".", ""), m.kur, m.fon, m.fin, m.eko, "", m.borc, m.alacak);
        //                if (!sonuc.islemSonuc)
        //                    OrtakFonksiyonlar.HataStrYaz("MuhasebatAktar-->" + sonuc.hataStr);
        //            }
        //        }
        //    }
        //    ve.Commit();

        //    ve.VeriErisimKapat();

        //    return sonuc;
        //}

        /// <summary>
        /// e-bütçe sisteminden Muhasebata ait harcama rakamlarını aktarma işlemi
        /// </summary>
        /// <param name="yil">Yıl</param>
        /// <param name="ay">ay</param>
        /// <param name="kurum">Kurum kodu</param>
        /// <param name="hesapKod">Hesap Kodu</param>
        /// <returns></returns>

        public static Sonuc MuhasebatHarcamaKaydet(Kullanici kullanan, int yil, int ay, int muhasebeKod, string birimKod, string vergiNo, HesapBilgi[] mler, bool prg)
        {
            return MuhasebatHarcamaKaydet("HarcamaAktar" + yil.ToString() + "_" + ay.ToString() + ".txt", kullanan, yil, ay, muhasebeKod, birimKod, vergiNo, mler, prg);
        }

        public static Sonuc MuhasebatHarcamaKaydet(string logDosyaAd, Kullanici kullanan, int yil, int ay, int muhasebeKod, string birimKod, string vergiNo, HesapBilgi[] mler, bool prg)
        {
            TNS.BKD.Tertip tUz = new TNS.BKD.Tertip(yil, TNS.BKD.ENUMTabloKod.TABPEBOdenek);
            int eko4Uz = tUz.KodUzunlukBul(3, 4, true);

            Sonuc sonuc = new Sonuc();
            if (!kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.SISTEMYONETICISI))
            {
                sonuc.DegerAta("Bu işlemi yapmaya yetkiniz yok!");
                return sonuc;
            }

            if (!string.IsNullOrWhiteSpace(vergiNo))
            {
                VergiNodanBirime(kullanan, null, vergiNo, "", out muhasebeKod, out birimKod);
            }
            if (muhasebeKod <= 0 || birimKod.Length != 3)
            {
                sonuc.DegerAta("muhasebe - birim kod geçerli değil: vergi No:" + vergiNo);
                return sonuc;
            }
            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(null));

            if (prg)
            {
                ProgramdanPEBeCevir(logDosyaAd, yil, mler);
            }

            TABMuhasebat to = new TABMuhasebat(ve);
            ve.BeginTransaction();
            if (mler.Length > 0)
            {
                sonuc = to.Sil(OrtakFonksiyonlar.ConvertToInt(yil, 0), OrtakFonksiyonlar.ConvertToInt(ay, 0), "", muhasebeKod, birimKod);
                if (sonuc.islemSonuc)
                {
                    foreach (HesapBilgi m in mler)
                    {
                        if (m.borc > 0)
                        {
                            m.NoktalariAt();
                            if (m.TamMi)
                            {
                                if (string.IsNullOrWhiteSpace(m.hesapKod))
                                    m.hesapKod = TNS.MUH.Arac.EkonomikKoddanHesapkodBul(m.eko, "830", eko4Uz).Replace(".", "");
                                else
                                    m.hesapKod = m.hesapKod.Replace(".", "");
                                sonuc = to.Kaydet(yil, ay, muhasebeKod, birimKod, m.hesapKod, m.kur, m.fon, m.fin, m.eko, m.peb, m.borc, 0);
                            }
                            else
                                Logla(logDosyaAd, "Tertip hatalı" + m.TertipStr);
                            if (!sonuc.islemSonuc)
                                break;
                        }
                    }
                }
            }

            if (sonuc.islemSonuc)
                ve.Commit();
            else
                ve.Rollback();
            ve.VeriErisimKapat();
            return sonuc;
        }

        private static void ProgramdanPEBeCevir(string logDosyaAd, int yil, HesapBilgi[] mler)
        {
            foreach (var item in mler)
            {
                string[] prgParcalar = item.fon.Split('.');
                if (prgParcalar.Length != 4)
                {
                    item.borc = 0;
                    Logla(logDosyaAd, "Hatalı program yapısı: " + item.fon);
                    continue;
                }
                //int prg = OrtakFonksiyonlar.ConvertToInt(prgParcalar[0], 0);
                //int aprg = OrtakFonksiyonlar.ConvertToInt(prgParcalar[1], 0);
                //int faal = OrtakFonksiyonlar.ConvertToInt(prgParcalar[2], 0);

                int afaal = OrtakFonksiyonlar.ConvertToInt(prgParcalar[3], 0);
                if (afaal <= 0)
                {
                    item.borc = 0;
                    Logla(logDosyaAd, "Hatalı program yapısı: " + item.fon);
                    continue;
                }
                string afaal_fon = TNS.BHMv2.Arac.ProgramButceKodaCevir(yil, 1200, prgParcalar[3]);
                if (string.IsNullOrWhiteSpace(afaal_fon))
                {
                    item.borc = 0;
                    Logla(logDosyaAd, "Alt faaliyet çözümlenemedi:" + prgParcalar[3]);
                    continue;
                }
                string[] a_f = afaal_fon.Split('.');
                if (a_f.Length != 2)
                {
                    item.borc = 0;
                    Logla(logDosyaAd, "Alt faaliyet çözümü yanlış: " + prgParcalar[3] + ", " + afaal_fon);
                    continue;
                }

                item.peb = a_f[0];
                item.fon = a_f[1];
                //string gelenEko = item.eko.Replace(".", "");

                //if (gelenEko.StartsWith("0303") || gelenEko.StartsWith("0304") || gelenEko.StartsWith("0103"))
                //{
                //    item.fon = item.fon.Replace(".", "").Substring(0, 4) + "6" + gelenEko.Substring(4, 1);
                //}
            }
        }

        ///// <summary>
        ///// e-bütçe sisteminden Muhasebata ait harcama rakamlarını aktarma işlemi
        ///// </summary>
        ///// <param name="yil">Yıl</param>
        ///// <param name="ay">ay</param>
        ///// <param name="kurum">Kurum kodu</param>
        ///// <param name="hesapKod">Hesap Kodu</param>
        ///// <returns></returns>
        //public Sonuc ebutceHarcamaKontrolx(Kullanici kullanan, string yil, string ay, string kurum, string hesapKod)
        //{
        //    int iYil = OrtakFonksiyonlar.ConvertToInt(yil, 0);
        //    Sonuc sonuc = new Sonuc();
        //    //if (iYil >= 2019)
        //    //{
        //    //    sonuc.DegerAta("2018 yılında itibaren ebütçe servisi çalışmamaktadır!");
        //    //    return sonuc;
        //    //}
        //    EButceParolaBilgileri();

        //    if (kurum == "")
        //        kurum = TNS.MUH.Yetki.VarsayilanKurumBul();

        //    MUHServisC.ebutce.Listeler.MuhasebatBilgi[] mler = new MuhasebatBilgi[0];

        //    try
        //    {
        //        mler = servis2MUH.MuhasebatListele(ebutceUser, ebutcePassword, yil, ay, kurum, hesapKod);
        //    }
        //    catch (Exception e)
        //    {
        //        OrtakFonksiyonlar.HataDosyaYaz("HarcamaAktar" + yil.ToString() + "_" + ay.ToString() + ".txt", "HarcamaAktar-->" + e.Message, false);
        //        sonuc.DegerAta(e.Message);
        //        return sonuc;
        //    }
        //    TNSDateTime dt = new TNSDateTime(DateTime.Now);
        //    OrtakFonksiyonlar.HataDosyaYaz("HarcamaAktar" + yil.ToString() + "_" + ay.ToString() + ".txt", dt.ToString() + "/" + kurum + "/" + hesapKod + " için gelen kayıt sayısı: " + mler.Length, false);

        //    VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(null));
        //    TABMuhasebat to = new TABMuhasebat(ve);
        //    ve.BeginTransaction();

        //    if (mler.Length > 0)
        //    {
        //        sonuc = to.Sil(iYil, OrtakFonksiyonlar.ConvertToInt(ay, 0), hesapKod, 0, "");
        //        if (sonuc.islemSonuc)
        //        {
        //            //int borcYokSayisi = 0;
        //            Hashtable birimHash = new Hashtable();
        //            foreach (MuhasebatBilgi m in mler)
        //            {
        //                //OrtakFonksiyonlar.HataDosyaYaz("MuhasebatAktar_Hepsi_" + dt.ToString() + ".txt", "HesapKod: " + m.hesapKod + " Borç: " + m.borc + " Alacak: " + m.alacak, false);

        //                if (m.borc > 0 || m.alacak > 0)
        //                {
        //                    if (m.kur.Length < 8)
        //                    {
        //                        OrtakFonksiyonlar.HataDosyaYaz("MuhasebatAktar_KurKod" + dt.ToString() + ".txt", m.yil + "," + m.ay + "," + m.muhasebe + "," + m.birim + "," + m.hesapKod + "," + m.kur + "." + m.fon + "." + m.fin + "." + m.eko + "," + m.borc + "," + m.alacak, false);
        //                        continue;
        //                    }
        //                    if (m.eko.Length > 6)
        //                    {
        //                        OrtakFonksiyonlar.HataDosyaYaz("MuhasebatAktar_EkoKod" + dt.ToString() + ".txt", m.yil + "," + m.ay + "," + m.muhasebe + "," + m.birim + "," + m.hesapKod + "," + m.kur + "." + m.fon + "." + m.fin + "." + m.eko + "," + m.borc + "," + m.alacak, false);
        //                        continue;
        //                    }
        //                    if (iYil >= 2018)
        //                    {
        //                        if (m.birim.Length > 3)
        //                        {
        //                            string birKod;
        //                            int sayKod;
        //                            VergiNodanBirime(kullanan, birimHash, m.birim, m.kur, out sayKod, out birKod);

        //                            if (sayKod <= 0 || birKod.Length != 3)
        //                            {
        //                                string tertip = m.kur + "-" + m.fon + "-" + m.fin + "-" + m.eko.Substring(0, 3);
        //                                OrtakFonksiyonlar.HataDosyaYaz("MuhasebatAktar" + dt.ToString() + ".txt", "muhasebe - birim kod geçerli değil: vergi No:" + m.birim + "**Tertip:" + tertip + "**Harcama:" + (m.borc - m.alacak), false);
        //                                //OrtakFonksiyonlar.HataDosyaYaz("MuhasebatAktar" + dt.ToString() + ".txt", "muhasebe - birim kod geçerli değil: vergi No:" + m.birim, false);

        //                                continue;
        //                            }

        //                            m.birim = birKod;
        //                            m.muhasebe = sayKod;
        //                        }
        //                    }
        //                    sonuc = to.Kaydet(m.yil, m.ay, m.muhasebe, m.birim, m.hesapKod.Replace(".", ""), m.kur, m.fon, m.fin, m.eko, m.borc, m.alacak);
        //                    if (!sonuc.islemSonuc)
        //                        break;
        //                }
        //                /*else
        //                {
        //                    OrtakFonksiyonlar.HataDosyaYaz("MuhasebatAktar_BorcAlacakYok_" + dt.ToString() + ".txt", "borc alacak olmayan hesapkodları: "+ m.hesapKod.Substring(0,3), false);
        //                    borcYokSayisi++;
        //                }*/
        //            }

        //            //OrtakFonksiyonlar.HataDosyaYaz("MuhasebatAktar_BorcAlacakYok_" + dt.ToString() + ".txt", borcYokSayisi.ToString(), false);
        //        }
        //    }

        //    if (sonuc.islemSonuc)
        //        ve.Commit();
        //    else
        //    {
        //        OrtakFonksiyonlar.HataDosyaYaz("HarcamaAktar" + yil.ToString() + "_" + ay.ToString() + ".txt", "hata:" + sonuc.hataStr, false);
        //        ve.Rollback();
        //    }


        //    ve.VeriErisimKapat();
        //    return sonuc;
        //}

        //public Sonuc ebutceHarcamaKontrolYeni2(Kullanici kullanan, string yil, string ay, string kurum, string hesapKod)
        //{
        //    int iYil = OrtakFonksiyonlar.ConvertToInt(yil, 0);
        //    Sonuc sonuc = new Sonuc();
        //    //if (iYil >= 2019)
        //    //{
        //    //    sonuc.DegerAta("2018 yılında itibaren ebütçe servisi çalışmamaktadır!");
        //    //    return sonuc;
        //    //}
        //    EButceParolaBilgileri();

        //    if (kurum == "")
        //        kurum = TNS.MUH.Yetki.VarsayilanKurumBul();

        //    MUHServisC.ebutce.Listeler.MuhasebatBilgi[] mler = new MuhasebatBilgi[0];

        //    try
        //    {
        //        mler = servis2MUH.HarcamaAvansListele(ebutceUser, ebutcePassword, yil, ay, kurum, "", "", "", "", "");
        //        //mler = servis2MUH.HarcamaAvansListele("Mercan", "Sgbbütçe401", yil, ay, "07.82", "", "", "", "", "");
        //        //servis2MUH.MuhasebatListele(ebutceUser, ebutcePassword, yil, ay, kurum, hesapKod);
        //    }
        //    catch (Exception e)
        //    {
        //        OrtakFonksiyonlar.HataDosyaYaz("HarcamaAktar" + yil.ToString() + "_" + ay.ToString() + ".txt", "HarcamaAktar-->" + e.Message, false);
        //        sonuc.DegerAta(e.Message);
        //        return sonuc;
        //    }
        //    TNSDateTime dt = new TNSDateTime(DateTime.Now);
        //    OrtakFonksiyonlar.HataDosyaYaz("HarcamaAktar" + yil.ToString() + "_" + ay.ToString() + ".txt", dt.ToString() + "/" + kurum + "/" + hesapKod + " için gelen kayıt sayısı: " + mler.Length, false);

        //    VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(null));
        //    TABMuhasebat to = new TABMuhasebat(ve);
        //    ve.BeginTransaction();

        //    if (mler.Length > 0)
        //    {
        //        sonuc = to.Sil(iYil, OrtakFonksiyonlar.ConvertToInt(ay, 0), hesapKod, 0, "");
        //        if (sonuc.islemSonuc)
        //        {
        //            //int borcYokSayisi = 0;
        //            Hashtable birimHash = new Hashtable();
        //            foreach (MuhasebatBilgi m in mler)
        //            {
        //                //OrtakFonksiyonlar.HataDosyaYaz("MuhasebatAktar_Hepsi_" + dt.ToString() + ".txt", "HesapKod: " + m.hesapKod + " Borç: " + m.borc + " Alacak: " + m.alacak, false);

        //                //if (m.kur == "12010062" && m.fon == "099900" && m.eko.Substring(0, 3) == "033" && m.VKN=="6110369462")
        //                //{
        //                if (m.borc != 0)// || m.alacak > 0)
        //                {
        //                    if (m.kur.Length < 8)
        //                    {
        //                        OrtakFonksiyonlar.HataDosyaYaz("MuhasebatAktar_KurKod" + dt.ToString() + ".txt", m.yil + "," + m.ay + "," + m.muhasebe + "," + m.birim + "," + m.hesapKod + "," + m.kur + "." + m.fon + "." + m.fin + "." + m.eko + "," + m.borc + "," + m.alacak, false);
        //                        continue;
        //                    }
        //                    if (m.eko.Length > 6)
        //                    {
        //                        OrtakFonksiyonlar.HataDosyaYaz("MuhasebatAktar_EkoKod" + dt.ToString() + ".txt", m.yil + "," + m.ay + "," + m.muhasebe + "," + m.birim + "," + m.hesapKod + "," + m.kur + "." + m.fon + "." + m.fin + "." + m.eko + "," + m.borc + "," + m.alacak, false);
        //                        continue;
        //                    }
        //                    if (iYil >= 2018)
        //                    {
        //                        string birKod;
        //                        int sayKod;
        //                        VergiNodanBirime(kullanan, birimHash, m.VKN, m.kur, out sayKod, out birKod);
        //                        //VergiNodanBirimeYeni(kullanan, birimHash, m.VKN, m.kur, out sayKod, out birKod);

        //                        if (sayKod <= 0 || birKod.Length != 3)
        //                        {
        //                            string tertip = m.kur + "-" + m.fon + "-" + m.fin + "-" + m.eko.Substring(0, 3);
        //                            OrtakFonksiyonlar.HataDosyaYaz("MuhasebatAktar" + dt.ToString() + ".txt", "muhasebe - birim kod geçerli değil: vergi No:" + m.VKN + "**Tertip:" + tertip + "**Harcama:" + (m.borc - m.alacak), false);

        //                            continue;
        //                        }

        //                        m.birim = birKod;
        //                        m.muhasebe = sayKod;
        //                    }
        //                    sonuc = to.Kaydet(m.yil, m.ay, m.muhasebe, m.birim, m.hesapKod.Replace(".", ""), m.kur, m.fon, m.fin, m.eko, "", m.borc, m.alacak);
        //                    if (!sonuc.islemSonuc)
        //                        break;
        //                }
        //            }
        //        }
        //    }

        //    if (sonuc.islemSonuc)
        //        ve.Commit();
        //    else
        //    {
        //        OrtakFonksiyonlar.HataDosyaYaz("HarcamaAktar" + yil.ToString() + "_" + ay.ToString() + ".txt", "hata:" + sonuc.hataStr, false);
        //        ve.Rollback();
        //    }


        //    ve.VeriErisimKapat();
        //    return sonuc;
        //}

        public Sonuc ebutceHarcamaKontrolPrg(Kullanici kullanan, int yil, int ay, string kurum, string hesapKod)
        {
            Sonuc sonuc = new Sonuc();
            EButceParolaBilgileri();

            if (kurum == "")
                kurum = TNS.MUH.Yetki.VarsayilanKurumBul();
            TNS.BHMv2.Arac.ProgramButceKodCevirSifirla(yil);
            TNSDateTime dt = new TNSDateTime(DateTime.Now);

            string logDosyaAd = "HarcamaAktar" + yil.ToString() + "_" + ay.ToString() + ".txt";

            OrtakFonksiyonlar.HataDosyaYaz(logDosyaAd, dt.ToString() + "/" + kurum + " Harcama servisi başladı: ", true);

            try
            {
                MUHServisC.tr.gov.sbb.programbutce.listeler.Listeler servisSBB = new MUHServisC.tr.gov.sbb.programbutce.listeler.Listeler();
                MUHServisC.tr.gov.sbb.programbutce.listeler.OdenekHarcamaTahminWI[] harcamalar = servisSBB.HarcamaListesi(MUHProgramButceIslemleri.ebutceUser, MUHProgramButceIslemleri.ebutcePassword, yil, ay, kurum);
                OrtakFonksiyonlar.HataDosyaYaz(logDosyaAd, "Servisten gelen kayıt sayısı: " + harcamalar.Length, false);

                System.Collections.Generic.List<HesapBilgi> hbler = new System.Collections.Generic.List<HesapBilgi>();
                string oncekiVKN = "";
                foreach (MUHServisC.tr.gov.sbb.programbutce.listeler.OdenekHarcamaTahminWI sbbHarcama in harcamalar)
                {
                    if (sbbHarcama.vkn != oncekiVKN)
                    {
                        if (hbler.Count > 0)
                            Logla(logDosyaAd, MuhasebatHarcamaKaydet(logDosyaAd, kullanan, yil, ay, 0, "", oncekiVKN, hbler.ToArray(), true));
                        oncekiVKN = sbbHarcama.vkn;
                        hbler.Clear();
                    }

                    HesapBilgi hb = new HesapBilgi();
                    hb.kur = sbbHarcama.KurKod;
                    hb.fin = sbbHarcama.FinKod;
                    hb.eko = sbbHarcama.EkoKod;
                    hb.fon = sbbHarcama.ProgramId + "." + sbbHarcama.AltProgramId + "." + sbbHarcama.FaaliyetId + "." + sbbHarcama.AltFaaliyetId;
                    hb.hesapKod = sbbHarcama.hesapKod;
                    hb.borc = sbbHarcama.harcama;
                    hb.alacak = 0;
                    hbler.Add(hb);
                }

                if (hbler.Count > 0)
                    Logla(logDosyaAd, MuhasebatHarcamaKaydet(logDosyaAd, kullanan, yil, ay, 0, "", oncekiVKN, hbler.ToArray(), true));


                OrtakFonksiyonlar.HataDosyaYaz(logDosyaAd, "Aktarım sonuçlandı.", false);
            }
            catch (Exception e)
            {
                OrtakFonksiyonlar.HataDosyaYaz(logDosyaAd, "HarcamaAktar-->" + e.Message, false);
                sonuc.DegerAta(e.Message);
                return sonuc;
            }

            return sonuc;
        }

        private static void Logla(string logDosyaAd, Sonuc sonuc)
        {
            if (!sonuc.islemSonuc)
                Logla(logDosyaAd, sonuc.hataStr);
        }

        private static void Logla(string logDosyaAd, string mesaj)
        {
            OrtakFonksiyonlar.HataDosyaYaz(logDosyaAd, "HarcamaAktar-->" + mesaj, false);
        }

        /// <summary>
        /// e-bütçe sisteminden Muhasebata ait harcama rakamlarını aktarma işlemi
        /// </summary>
        /// <param name="yil">Yıl</param>
        /// <param name="ay">ay</param>
        /// <param name="kurum">Kurum kodu</param>
        /// <param name="hesapKod">Hesap Kodu</param>
        /// <returns></returns>
        //public Sonuc ebutceHarcamaKontrolYeni(Kullanici kullanan, string yil, string ay, string kurum)
        //{
        //    int iYil = OrtakFonksiyonlar.ConvertToInt(yil, 0);
        //    Sonuc sonuc = new Sonuc();
        //    //if (iYil >= 2019)
        //    //{
        //    //    sonuc.DegerAta("2018 yılında itibaren ebütçe servisi çalışmamaktadır!");
        //    //    return sonuc;
        //    //}
        //    //EButceParolaBilgileri();

        //    if (kurum == "")
        //        kurum = TNS.MUH.Yetki.VarsayilanKurumBul();

        //    //MUHServisC.ebutce.Listeler.MuhasebatBilgi[] mler = new MuhasebatBilgi[0];

        //    MUHServisC.HarcamaMuhasebatServisi.SonIslemZamanAraliginaGoreTertipHarcamasiSorgulaRequest r2 = new MUHServisC.HarcamaMuhasebatServisi.SonIslemZamanAraliginaGoreTertipHarcamasiSorgulaRequest();
        //    //r2.harcamaBirimiVergiNo = "1790356007"; 
        //    r2.baslangicZamani = "25-06-2018 00:00:00";
        //    r2.bitisZamani = "03-07-2018 00:00:00";
        //    r2.kamuIdaresiNo = 71;

        //    MUHServisC.HarcamaMuhasebatServisi.SonIslemZamaninaGoreTertipHarcamasiSorgulaRequest r1 = new MUHServisC.HarcamaMuhasebatServisi.SonIslemZamaninaGoreTertipHarcamasiSorgulaRequest();
        //    r1.sonIslemZamani = "01-06-2018 00:00:00";
        //    r1.kamuIdaresiNo = 71;

        //    VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(null));
        //    TABMuhasebat to = new TABMuhasebat(ve);
        //    ve.BeginTransaction();

        //    try
        //    {
        //        MUHServisC.HarcamaMuhasebatServisi.odenek[] odenekler = muhServis.SonIslemZamaninaGoreTertipHarcamasiSorgula(r1);
        //        //MUHServisC.HarcamaMuhasebatServisi.odenek[] odenekler = muhServis.SonIslemZamanAraliginaGoreTertipHarcamasiSorgula(r2);

        //        ButceMuhasebe[] mmler = new ButceMuhasebe[odenekler.Length];
        //        int mCount = 0;
        //        TNSDateTime dt = new TNSDateTime(DateTime.Now);

        //        OrtakFonksiyonlar.HataDosyaYaz("HarcamaAktar" + yil.ToString() + "_" + ay.ToString() + ".txt", dt.ToString() + "/" + kurum + " için gelen kayıt sayısı: " + odenekler.Length, false);

        //        if (odenekler.Length > 0)
        //        {
        //            sonuc = to.Sil(iYil, OrtakFonksiyonlar.ConvertToInt(ay, 0), "", 0, "");
        //            if (sonuc.islemSonuc)
        //            {
        //                Hashtable birimHash = new Hashtable();
        //                int cntBorcsuz = 0, cntVknoEslesmeyen = 0;
        //                foreach (MUHServisC.HarcamaMuhasebatServisi.odenek o in odenekler)
        //                {
        //                    mmler[mCount] = new ButceMuhasebe();
        //                    string[] kodlar = o.tertip.Split('.');

        //                    if (!(kodlar.Length == 11 || kodlar.Length == 13))
        //                    {
        //                        OrtakFonksiyonlar.HataDosyaYaz("MuhasebatAktar_KodHatalari" + dt.ToString() + ".txt", o.sonIslemZamani.ToString() + "\t" + o.tertip + "\t" + o.harcamaBirimiVergiNo + "\t" + o.harcamaBirimiAdi + "\t" + o.harcanan.kurus, false);
        //                        continue;
        //                    }

        //                    if (o.harcanan.kurus > 0)
        //                    {
        //                        string hesapKod = "";
        //                        TNSDateTime tarih = new TNSDateTime(o.sonIslemZamani);
        //                        if (tarih.Yil != dt.Yil)
        //                            mmler[mCount].ay = 12;
        //                        else
        //                            mmler[mCount].ay = tarih.Ay;

        //                        mmler[mCount].yil = tarih.Yil;
        //                        mmler[mCount].borc = o.harcanan.kurus / 100;
        //                        mmler[mCount].birim = o.harcamaBirimiVergiNo;

        //                        mmler[mCount].kurKod = o.tertip.Substring(0, 11).Replace(".", "");
        //                        mmler[mCount].fonKod = o.tertip.Substring(12, 9).Replace(".", "");
        //                        mmler[mCount].finKod = o.tertip.Substring(22, 1).Replace(".", "");
        //                        //mmler[mCount].ekoKod = o.tertip.Substring(23, 10).Replace(".", "");//4

        //                        mmler[mCount].ekoKod = o.tertip.Substring(23, o.tertip.Length - 23).Replace(".", "");

        //                        if (mmler[mCount].ekoKod.Length == 3)
        //                            mmler[mCount].ekoKod += "000";
        //                        else if (mmler[mCount].ekoKod.Length > 6)
        //                        {
        //                            OrtakFonksiyonlar.HataDosyaYaz("MuhasebatAktar_KodHatalari" + dt.ToString() + ".txt", o.sonIslemZamani.ToString() + "\t" + o.tertip + "\t" + o.harcamaBirimiVergiNo + "\t" + o.harcamaBirimiAdi + "\t" + o.harcanan.kurus, false);
        //                            continue;
        //                        }

        //                        if (mmler[mCount].ekoKod.Substring(4, 2) == "00")
        //                            hesapKod = "160";
        //                        else
        //                            hesapKod = "830";

        //                        hesapKod += mmler[mCount].ekoKod.Substring(0, 2) + "0" + mmler[mCount].ekoKod.Substring(2, 1) + "0" + mmler[mCount].ekoKod.Substring(3, 3);

        //                        mmler[mCount].hesapKod = hesapKod;

        //                        if (iYil >= 2018)
        //                        {
        //                            if (mmler[mCount].birim.Length > 3)
        //                            {
        //                                string birKod;
        //                                int sayKod;
        //                                VergiNodanBirime(kullanan, birimHash, mmler[mCount].birim, mmler[mCount].kurKod, out sayKod, out birKod);

        //                                if (sayKod <= 0 || birKod.Length != 3)
        //                                {
        //                                    string tertip = mmler[mCount].kurKod + "-" + mmler[mCount].fonKod + "-" + mmler[mCount].finKod + "-" + mmler[mCount].ekoKod.Substring(0, 3);
        //                                    OrtakFonksiyonlar.HataDosyaYaz("MuhasebatAktar" + dt.ToString() + ".txt", "muhasebe - birim kod geçerli değil: vergi No:" + mmler[mCount].birim + "**Tertip:" + tertip + "**Harcama:" + (mmler[mCount].borc), false);
        //                                    //OrtakFonksiyonlar.HataDosyaYaz("MuhasebatAktar" + dt.ToString() + ".txt", "muhasebe - birim kod geçerli değil: vergi No:" + m.birim, false);
        //                                    cntVknoEslesmeyen++;
        //                                    continue;
        //                                }

        //                                mmler[mCount].birim = birKod;
        //                                mmler[mCount].muhasebe = sayKod;
        //                            }
        //                        }

        //                        //OrtakFonksiyonlar.HataDosyaYaz("MuhasebatAktar_Hepsi" + System.DateTime.Now.ToString() + ".txt", o.sonIslemZamani.ToString() + "\t" + o.tertip + "\t" + o.harcamaBirimiVergiNo + "\t" + o.harcamaBirimiAdi + "\t" + o.harcanan.kurus, false);

        //                        //OrtakFonksiyonlar.HataDosyaYaz("MuhasebatAktar_Hepsi_" + dt.ToString() + ".txt", "Yil: " + mmler[mCount].yil + " ** Ay: " + mmler[mCount].ay + " ** Muhasebe: " + mmler[mCount].muhasebe + " ** Birim: " + mmler[mCount].birim + " ** HesapKod: " + mmler[mCount].hesapKod + " ** KurKod: " + mmler[mCount].kurKod + " ** FonKod: " + mmler[mCount].fonKod + " ** FinKod: " + mmler[mCount].finKod + " ** EkoKod: " + mmler[mCount].ekoKod + " Harcanan: " + mmler[mCount].borc, false);
        //                        sonuc = to.Kaydet(mmler[mCount].yil, mmler[mCount].ay, mmler[mCount].muhasebe, mmler[mCount].birim, mmler[mCount].hesapKod.Replace(".", ""), mmler[mCount].kurKod, mmler[mCount].fonKod, mmler[mCount].finKod, mmler[mCount].ekoKod, mmler[mCount].borc, 0);
        //                        if (!sonuc.islemSonuc)
        //                            break;
        //                        mCount++;
        //                    }
        //                    else
        //                        cntBorcsuz++;
        //                    //OrtakFonksiyonlar.HataDosyaYaz("MuhasebatAktar_BorcYok_" + dt.ToString() + ".txt", "Yil: " + mmler[mCount].yil + " ** Ay: " + mmler[mCount].ay + " ** Muhasebe: " + mmler[mCount].muhasebe + " ** Birim: " + mmler[mCount].birim + " ** HesapKod: " + mmler[mCount].hesapKod + " ** KurKod: " + mmler[mCount].kurKod + " ** FonKod: " + mmler[mCount].fonKod + " ** FinKod: " + mmler[mCount].finKod + " ** EkoKod: " + mmler[mCount].ekoKod + " Harcanan: " + mmler[mCount].borc, false);
        //                    //System.Console.WriteLine(o.sonIslemZamani.ToString() + "\t" + o.tertip + "\t" + o.harcamaBirimiVergiNo + "\t" + o.harcamaBirimiAdi + "\t" + o.harcanan.kurus);
        //                }
        //                OrtakFonksiyonlar.HataDosyaYaz("HarcamaAktar" + yil.ToString() + "_" + ay.ToString() + ".txt", dt.ToString() + "/" + kurum + " için borcsuz kayıt sayısı: " + cntBorcsuz, false);
        //                OrtakFonksiyonlar.HataDosyaYaz("HarcamaAktar" + yil.ToString() + "_" + ay.ToString() + ".txt", dt.ToString() + "/" + kurum + " için VKNO eşleşmeyen kayıt sayısı: " + cntVknoEslesmeyen, false);
        //            }
        //        }

        //        if (sonuc.islemSonuc)
        //            ve.Commit();
        //        else
        //        {
        //            OrtakFonksiyonlar.HataDosyaYaz("HarcamaAktar" + yil.ToString() + "_" + ay.ToString() + ".txt", "hata:" + sonuc.hataStr, false);
        //            ve.Rollback();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        OrtakFonksiyonlar.HataDosyaYaz("HarcamaAktar" + yil.ToString() + "_" + ay.ToString() + ".txt", "HarcamaAktar-->" + ex.Message, false);
        //        sonuc.DegerAta(ex.Message);

        //        return sonuc;
        //    }

        //    ve.VeriErisimKapat();
        //    return sonuc;
        //}

        class BirimBilgi
        {
            public string birimKod;
            public int muhasebeKod;
        }
        public static void VergiNodanBirime(Kullanici kullanan, Hashtable birimHash, string vergiNo, string kur, out int muhasebeKod, out string birimKod)
        {
            BirimBilgi birimBilgi = null;
            if (birimHash != null)
            {
                try
                {
                    birimBilgi = (BirimBilgi)birimHash[vergiNo];
                }
                catch
                {
                    birimBilgi = null;
                }
            }

            if (birimBilgi == null)
            {
                HRC.IdareBilgi kriter = new HRC.IdareBilgi();
                kriter.vNo = vergiNo.PadLeft(10, '0');
                kriter = TNS.HRC.Arac.Tanimla().IdareBilgiListele(kullanan, kriter);
                birimBilgi = new BirimBilgi();
                birimBilgi.muhasebeKod = kriter.muhasebe;
                birimBilgi.birimKod = kriter.birim;
                if (birimHash != null)
                {
                    birimHash[vergiNo] = birimBilgi;
                }
            }
            muhasebeKod = birimBilgi.muhasebeKod;
            birimKod = birimBilgi.birimKod;
        }

        private static void VergiNodanBirimeYeni(Kullanici kullanan, Hashtable birimHash, string vergiNo, string kur, out int muhasebeKod, out string birimKod)
        {
            BirimBilgi birimBilgi = null;
            if (birimHash != null)
            {
                try
                {
                    birimBilgi = (BirimBilgi)birimHash[vergiNo];
                }
                catch
                {
                    birimBilgi = null;
                }
            }

            if (birimBilgi == null)
            {
                //HRC.IdareBilgi kriter = new HRC.IdareBilgi();
                //kriter.vNo = vergiNo.PadLeft(10, '0');
                //kriter = TNS.HRC.Arac.Tanimla().IdareBilgiListele(kullanan, kriter);

                MuhasebeBirim kriter = new MuhasebeBirim();
                kriter.vno = vergiNo.PadLeft(10, '0');
                kriter.kurum = kur;
                kriter = TNS.MUH.IsKurallari.MUHServis.VNOdanMuhasebeBirimListele(kriter);

                birimBilgi = new BirimBilgi();
                birimBilgi.muhasebeKod = kriter.muhasebe;
                birimBilgi.birimKod = kriter.birimKod;
                if (birimHash != null)
                {
                    birimHash[vergiNo] = birimBilgi;
                }
            }
            muhasebeKod = birimBilgi.muhasebeKod;
            birimKod = birimBilgi.birimKod;
        }

        /// <summary>
        /// Ödenek gönderme belgelerini e-bütçe sistemine kayıt eder
        /// </summary>
        /// <param name="yil">Yıl</param>

        //private static int THREAD_SAY = 1;

        /// <summary>
        /// Ödenek gönderme belgelerini e-bütçe sistemine kayıt eder
        /// </summary>
        /// <param name="yil">Yıl</param>

        //private string NoktaliVirgulle(TNSCollection belgeNolar)
        //{
        //    string tut = "";
        //    foreach (string item in belgeNolar)
        //    {
        //        if (tut != "") tut += ";";
        //        tut += item;
        //    }
        //    return tut;
        //}


        /// <summary>
        /// e-bütçe sisteminde bulunan ödenek gönderme belgelerini listeler
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="yil">Yıl</param>
        /// <param name="ay">ay</param>
        /// <param name="tur">tur</param>
        //public string ebutceOdenekGondermeListele(TNS.KYM.Kullanici kullanan, int yil, int ay, int tur)
        //{
        //    GondermeTenkisFormW sorguForm = new GondermeTenkisFormW();
        //    sorguForm.yil = yil;
        //    sorguForm.kurum = TNS.MUH.Yetki.VarsayilanKurumBul();
        //    sorguForm.ay = ay;

        //    if (!TNS.MUH.Arac.OdenekGOndermeSadeceEko2Mi(yil))
        //        return "Kurumunuzda ödenek gönderme ekonomik 2 seviyede olmadığından işlem gerçekleştirilemiyor.";

        //    EButceParolaBilgileri();

        //    MUHGondermeTenkisServis d = new MUHGondermeTenkisServis();
        //    Sonuc sonuc = new Sonuc();

        //    if (tur == (int)ENUMBelgeTuru.ODENEKGONDERME)
        //        sorguForm.belgeTur = MUHServisC.ebutce.OdenekTenkis.ENUMBelgeTuru.ODENEKGONDERME;
        //    else
        //        sorguForm.belgeTur = MUHServisC.ebutce.OdenekTenkis.ENUMBelgeTuru.TENKIS;

        //    MUHServisC.ebutce.OdenekTenkis.Sonuc sonucKayit = new MUHServisC.ebutce.OdenekTenkis.Sonuc();

        //    GondermeTenkisFormW[] belgeler = null;
        //    try
        //    {
        //        sorguForm.detay = new MUHServisC.ebutce.OdenekTenkis.GondermeTenkisDetayW[0];
        //        belgeler = servisMUH.OdenekGondermeBelgeListele(ebutceUser, ebutcePassword, sorguForm);
        //    }
        //    catch (Exception e) { return e.Message; }


        //    if (belgeler != null && belgeler.Length > 0)
        //    {
        //        for (int i = 0; i < belgeler.Length; i++)
        //        {
        //            GondermeTenkisFormW tBelge = new GondermeTenkisFormW();
        //            GondermeTenkisForm belge = new GondermeTenkisForm();

        //            tBelge = belgeler[i];

        //            belge.aciklama = tBelge.aciklama;
        //            belge.belgeNo = tBelge.belgeNo;
        //            if (tBelge.belgeTur == MUHServisC.ebutce.OdenekTenkis.ENUMBelgeTuru.ODENEKGONDERME)
        //                belge.belgeTur = ENUMBelgeTuru.ODENEKGONDERME;
        //            else
        //                belge.belgeTur = ENUMBelgeTuru.TENKIS;

        //            belge.birim = tBelge.birim;
        //            belge.formDurum = tBelge.formDurum;
        //            belge.formDurumTarih = new TNSDateTime(tBelge.formDurumTarih + "");
        //            belge.icmalNo = tBelge.icmalNo;
        //            belge.icmalTarih = new TNSDateTime(tBelge.icmalTarih + "");
        //            belge.iptalNeden = tBelge.iptalNeden;
        //            belge.islemTarih = new TNSDateTime(tBelge.islemTarih + "");
        //            belge.islemYapan = tBelge.islemYapan;
        //            belge.istekYaziNo = tBelge.istekYaziNo;
        //            belge.istekYaziTarih = new TNSDateTime(tBelge.istekYaziTarih + "");
        //            belge.kurum = tBelge.kurum;
        //            belge.muhasebe = tBelge.muhasebe;
        //            belge.onayNo = tBelge.onayNo;
        //            belge.onayTarih = new TNSDateTime(tBelge.onayTarih + "");
        //            belge.yaziNo = tBelge.yaziNo;
        //            belge.yaziTarih = new TNSDateTime(tBelge.yaziTarih + "");
        //            belge.yil = tBelge.yil;

        //            belge.islemTarih = belge.icmalTarih;

        //            for (int j = 0; j < tBelge.detay.Length; j++)
        //            {
        //                GondermeTenkisDetayW tDetay = new GondermeTenkisDetayW();
        //                tDetay = tBelge.detay[j];
        //                GondermeTenkisDetay detay = new GondermeTenkisDetay();
        //                detay.miktar = tDetay.miktar;
        //                tDetay.tertip = tDetay.tertip.Replace(".", "");
        //                detay.tertip.TertipAta(tDetay.tertip);

        //                belge.detay.objeler.Add(detay);
        //            }

        //            sonuc = d.Kaydet(kullanan, belge, false, false, false, true);
        //        }
        //    }

        //    return "";
        //}


        /// <summary>
        /// e-bütçe sisteminden Ödenek rakamlarını aktarır.
        /// </summary>
        /// <param name="yil">Yıl</param>
        /// <param name="asama">asama</param>
        /// <param name="kurum">Kurum kodu</param>
        //public Sonuc ebutceOdenekKontrol(string yil, string asama, string kurum)
        //{
        //    EButceParolaBilgileri();
        //    Sonuc sonuc = new Sonuc(false, "", "");

        //    if (kurum == "")
        //        kurum = TNS.MUH.Yetki.VarsayilanKurumBul();

        //    MUHServisC.ebutce.Listeler.OdenekBilgi mler = new OdenekBilgi();
        //    try
        //    {
        //        //***Şifre
        //        mler = servis2MUH.OdenekListele(ebutceUser, ebutcePassword, yil, asama, kurum, "", "", "");
        //    }
        //    catch (Exception e) { sonuc.hataStr = e.Message; return sonuc; }

        //    VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriBHM", TNS.KYM.Arac.KimlikBul(null));
        //    TABOdenek to = new TABOdenek(ve);
        //    ve.BeginTransaction();

        //    sonuc = to.ButceHazirlikOdenekSil(OrtakFonksiyonlar.ConvertToInt(yil, 0), OrtakFonksiyonlar.ConvertToInt(asama, 0));

        //    if (sonuc.islemSonuc)
        //    {
        //        foreach (OdenekSatir m in mler.odenekler)
        //        {
        //            if (m.odenek1 > 0)
        //                sonuc = to.ButceHazirlikOdenekKaydet(mler.yil, mler.asama, m.kur, m.fon, m.fin, m.eko, m.odenek1);
        //            if (m.odenek2 > 0 && sonuc.islemSonuc)
        //                sonuc = to.ButceHazirlikOdenekKaydet(mler.yil, mler.asama + 10, m.kur, m.fon, m.fin, m.eko, m.odenek3);
        //            if (m.odenek3 > 0 && sonuc.islemSonuc)
        //                sonuc = to.ButceHazirlikOdenekKaydet(mler.yil, mler.asama + 20, m.kur, m.fon, m.fin, m.eko, m.odenek3);

        //            if (!sonuc.islemSonuc)
        //                break;
        //        }
        //        if (sonuc.islemSonuc)
        //            ve.Commit();
        //        else
        //            ve.Rollback();
        //    }
        //    else
        //        ve.Rollback();

        //    ve.VeriErisimKapat();

        //    return sonuc;
        //}

        /// <summary>
        /// e-bütçe sisteminden bütçe kodlarını aktarır
        /// </summary>
        /// <param name="yil">Yıl</param>
        /// <param name="kodTuru">kod tur</param>
        /// <param name="ustKod">ust kod</param>
        /// <param name="seviye">seviye</param>
        /// <param name="sil">seviye</param>
        /// <returns></returns>
        //public Sonuc ebutceKodKontrol(string yil, string kodTuru, string ustKod, int seviye, bool sil)
        //{
        //    Sonuc sonuc = new Sonuc();

        //    MUHServisC.ebutce.Listeler.ABSKodlarW mler = new ABSKodlarW();
        //    try
        //    {
        //        //***Şifre
        //        mler = servis2MUH.KodListele(OrtakFonksiyonlar.ConvertToInt(yil, 0), kodTuru, ustKod, seviye);
        //    }
        //    catch (Exception e) { sonuc.hataStr = e.Message; sonuc.islemSonuc = false; return sonuc; }

        //    VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriBHM", TNS.KYM.Arac.KimlikBul(null));
        //    TABOdenek to = new TABOdenek(ve);
        //    ve.BeginTransaction();

        //    if (sil)
        //        sonuc = to.ButceHazirlikOdenekSilKod(OrtakFonksiyonlar.ConvertToInt(yil, 0), kodTuru, ustKod);

        //    foreach (ABSKodW m in mler.absKodlar)
        //    {
        //        ve.hata = "";//Eğer silmeden dolayı hata oldu ise kayıt edilemez.
        //        if (kodTuru == "1")//Kurumsal kod
        //        {
        //            if (m.kod.Replace(".", "").StartsWith(TNS.MUH.Yetki.VarsayilanKurumBul().Replace(".", "")))
        //                sonuc = to.ButceHazirlikOdenekKaydetKod(yil, kodTuru, m.kod.Replace(".", ""), m.ad);
        //        }
        //        else
        //            sonuc = to.ButceHazirlikOdenekKaydetKod(yil, kodTuru, m.kod.Replace(".", ""), m.ad);
        //    }

        //    if (sonuc.islemSonuc)
        //        ve.Commit();
        //    else
        //        ve.Rollback();

        //    ve.VeriErisimKapat();

        //    return sonuc;
        //}

        /// <summary>
        /// e-bütçe sisteminden bütçe rakamlarını aktarır
        /// </summary>
        /// <param name="yil">Yıl</param>
        /// <param name="kurum">Kurum kodu</param>
        //public Sonuc ebutceButceKontrol(string yil, string kurum)
        //{
        //    Sonuc sonuc = new Sonuc();
        //    EButceParolaBilgileri();

        //    if (kurum == "")
        //        kurum = TNS.MUH.Yetki.VarsayilanKurumBul();

        //    MUHServisC.ebutce.Listeler.ButceBilgileriW mler = new ButceBilgileriW();

        //    try
        //    {
        //        //****şifre
        //        mler = servis2MUH.ButceSonDurumListele(ebutceUser, ebutcePassword, OrtakFonksiyonlar.ConvertToInt(yil, 0), kurum);
        //    }
        //    catch (Exception e) { sonuc.hataStr = e.Message; sonuc.islemSonuc = false; return sonuc; }

        //    VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(null));
        //    TABOdenek to = new TABOdenek(ve);

        //    ve.BeginTransaction();

        //    sonuc.islemSonuc = true;
        //    if (mler.butceBilgisi.Length > 0)
        //        sonuc = to.ButceSil(OrtakFonksiyonlar.ConvertToInt(yil, 0));

        //    if (sonuc.islemSonuc)
        //    {
        //        foreach (ButceBilgisiW m in mler.butceBilgisi)
        //        {
        //            sonuc = to.ButceKaydet(OrtakFonksiyonlar.ConvertToInt(yil, 0), m.kur, m.fon, m.fin, m.eko, m.kanun, m.kbo, m.eklenen, m.dusulen, m.serbest);

        //            if (!sonuc.islemSonuc)
        //                break;
        //        }
        //        if (sonuc.islemSonuc)
        //            ve.Commit();
        //        else
        //            ve.Rollback();
        //    }
        //    else
        //        ve.Rollback();

        //    ve.VeriErisimKapat();

        //    return sonuc;
        //}

        /// <summary>
        /// e-bütçe sisteminden bütçe rakamlarını aktarır
        /// </summary>
        /// <param name="yil">Yıl</param>
        /// <param name="kurum">Kurum kodu</param>
        //public Sonuc ebutceButceIslemKontrol(int yil, string kurum)
        //{
        //    Sonuc sonuc = new Sonuc();
        //    EButceParolaBilgileri();

        //    if (kurum == "")
        //        kurum = TNS.MUH.Yetki.VarsayilanKurumBul();

        //    kurum = kurum.Replace(".", "");
        //    servis2MUH.Timeout = 20 * 60 * 1000;

        //    VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(null));
        //    TABOdenek to = new TABOdenek(ve);
        //    ve.BeginTransaction();

        //    sonuc = to.ButceIslemSil(yil);

        //    MUHServisC.ebutce.Listeler.ButceIslemBilgi[] mler;

        //    string[] islemler = { "02", "03" };

        //    for (int islem = 0; islem < islemler.Length; islem++)
        //    {
        //        try
        //        {
        //            //****şifre
        //            mler = servis2MUH.ButceIslemListeleToplam(ebutceUser, ebutcePassword, yil, kurum, islemler[islem]);
        //            for (int i = 0; i < mler.Length; i++)
        //            {
        //                sonuc = ebutceButceIslemKaydet(mler[i], kurum, to);
        //                if (!sonuc.islemSonuc)
        //                    break;
        //            }
        //        }
        //        catch (Exception e) { sonuc.hataStr = e.Message; sonuc.islemSonuc = false; break; }

        //    }

        //    if (sonuc.islemSonuc)
        //        ve.Commit();
        //    else
        //        ve.Rollback();

        //    ve.VeriErisimKapat();

        //    return sonuc;
        //}

        //Sonuc ebutceButceIslemKaydet(ButceIslemBilgi m, string kurum, TABOdenek to)
        //{
        //    Sonuc sonuc = new Sonuc();
        //    ButceIslemSatir[] sGider;
        //    //ButceIslemSatir[] sGelir;
        //    //ButceIslemSatir[] sFinansman;

        //    if (m.giderler != null)
        //    {
        //        sGider = m.giderler;
        //        //if (mler[i].gelirler != null)
        //        //    sGelir = mler[i].gelirler;
        //        //if (mler[i].finanslar != null)
        //        //    sFinansman = mler[i].finanslar;

        //        for (int j = 0; j < sGider.Length; j++)
        //        {
        //            double odenek = sGider[j].eklenen_serbest;
        //            if (sGider[j].dusulen > 0)
        //                odenek = sGider[j].dusulen * -1;

        //            string[] t = sGider[j].tertip.Split('-');
        //            int yildizYer = sGider[j].yildizYer;

        //            if (t.Length == 4)
        //            {
        //                TNSDateTime tarih = new TNSDateTime(m.onayTarih);

        //                if (t[0].StartsWith(kurum))
        //                    sonuc = to.ButceIslemKaydet(m.yil, m.kurum, m.islemKod, tarih.Oku().Month, m.siraNo, t[0], t[1], t[2], t[3], "0000", odenek, m.formDurum, yildizYer);
        //            }
        //        }
        //    }
        //    return sonuc;

        //}


        /// <summary>
        /// e-bütçe sisteminden ahp rakamlarını aktarır
        /// </summary>
        /// <param name="yil">Yıl</param>
        /// <param name="kurum">Kurum kodu</param>
        //public Sonuc ebutceAHPKontrol(int yil, string kurum)
        //{
        //    if (kurum == "")
        //        kurum = TNS.MUH.Yetki.VarsayilanKurumBul();
        //    Sonuc sonuc = new Sonuc();
        //    EButceParolaBilgileri();

        //    if (!TNS.MUH.Arac.OdenekGOndermeSadeceEko2Mi(yil))
        //    {
        //        sonuc.hataStr = "Kurumunuzda AHP ekonomik 2 seviyede yapılmadığından işlem gerçekleştirilemiyor.";
        //        sonuc.islemSonuc = false;
        //        return sonuc;
        //    }

        //    kurum = kurum.Replace(".", "");
        //    AHPSonuc mler = new AHPSonuc();

        //    try
        //    {
        //        //****şifre
        //        mler = servis3MUH.AHPListele(ebutceUser, ebutcePassword, yil, kurum, "");
        //    }
        //    catch (Exception e) { sonuc.hataStr = e.Message; sonuc.islemSonuc = false; return sonuc; }

        //    VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(null));
        //    TABOdenek to = new TABOdenek(ve);

        //    ve.BeginTransaction();

        //    int siraNo = 10;
        //    if (mler.AHPSatirlar.Length > 0)
        //    {
        //        sonuc = to.AHPSil(yil, kurum, siraNo);
        //        if (sonuc.islemSonuc)
        //            sonuc = to.AHPOnaylariKaldir(yil, kurum);
        //    }

        //    if (sonuc.islemSonuc)
        //    {
        //        foreach (AHPSatir m in mler.AHPSatirlar)
        //        {
        //            for (int i = 0; i < m.aylar.Length; i++)
        //            {
        //                string[] t = m.tertip.Split('-');
        //                if (t.Length == 4)
        //                {
        //                    sonuc = to.AHPKaydet(yil, kurum, "0101", siraNo, t[0], t[1], t[2], t[3], "0000", m.aylar[i], 5, i + 1);

        //                    if (!sonuc.islemSonuc)
        //                        break;
        //                }
        //            }
        //            if (!sonuc.islemSonuc)
        //                break;
        //        }
        //        if (sonuc.islemSonuc)
        //            ve.Commit();
        //        else
        //            ve.Rollback();
        //    }
        //    else
        //        ve.Rollback();

        //    ve.VeriErisimKapat();

        //    return sonuc;
        //}

        /// <summary>
        /// Ebutce kullanıcı bilgilerini ebütçe sistemine bağlantı kurarak test eder.
        /// </summary>
        /// <param name="kullanici">kullanici.</param>
        /// <param name="parola">parola.</param>
        /// <returns></returns>
        public Sonuc EbutceParolaKontrol(string kullanici, string parola)
        {
            Sonuc sonuc = new Sonuc(); // false, "e-bütçe kullanıcı veya parolası hatalı girilmiş.", "");
            //MUHServisC.ebutce.Listeler.ButceBilgileriW mler = new ButceBilgileriW();

            //try
            //{
            //    mler = servis2MUH.ButceSonDurumListele(kullanici.Trim(), parola.Trim(), DateTime.Now.Year, TNS.DEG.Arac.VarsayilanKurumBul());

            //    //OrtakFonksiyonlar.HataStrYaz("parola kontrol-->" + mler.butceBilgisi.Length);
            //    if (mler.butceBilgisi.Length > 0)
            //    {
            //        sonuc.bilgiStr = "e-bütçe sistemine başarıyla bağlandı.";
            //        sonuc.islemSonuc = true;
            //    }
            //}
            //catch { return sonuc; }

            return sonuc;
        }

        static void EbutceParolaOku(ref string kullanici, ref string parola)
        {
            IKYMServis servisKYM = TNS.KYM.Arac.Tanimla();

            TNSCollection kDeg = servisKYM.DegiskenleriListele("stratek", "EBUTCEKULLANICI");
            if (kDeg != null && kDeg.Count > 0) kullanici = kDeg[0].ToString();

            kDeg = servisKYM.DegiskenleriListele("stratek", "EBUTCEPAROLA");
            if (kDeg != null && kDeg.Count > 0) parola = kDeg[0].ToString();
        }

    }
}
