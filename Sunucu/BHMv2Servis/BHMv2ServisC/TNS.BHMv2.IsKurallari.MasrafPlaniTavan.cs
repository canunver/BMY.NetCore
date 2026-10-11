using System;
using System.Data;
using TNS;
using TNS.Veri;
using TNS.BHMv2;
using TNS.BHMv2.IsKurallari.Veri;
using TNS.KYM;
using System.Collections;
using TNS.BKD;
using TNS.UZY;
using TNS.YAT;

namespace TNS.BHMv2.IsKurallari
{
    public class MasrafPlaniTavan
    {
        public static string VERIERISIMAD = "BaglantiSatiriBHMv2";

        public static UcgenKodBilgileri UcgenKodBilgileriBul(Kullanici kullanan, Tertip kistasTertip)
        {
            bool eko23;
            bool eko2 = TNS.MUH.Arac.OdenekGondermeEko2liMi(kistasTertip.yil);
            if (eko2)
                eko23 = TNS.BKD.Arac.Eko2_3Bul(kullanan);
            else
                eko23 = false;

            UcgenKodBilgileri kodBilgileri = new UcgenKodBilgileri();
            for (int i = 0; i < kistasTertip.KodNoSay(); i++)
            {
                int tut = kistasTertip.KodNoKirilimSay(i);
                bool eko23ic;
                if (i == 3 && eko2)
                {
                    tut = 2;
                    eko23ic = eko23;
                }
                else
                    eko23ic = false;
                kodBilgileri.Add(new UcgenKodBilgi(kistasTertip.KodNolar(i), tut, eko23ic));
            }
            return kodBilgileri;
        }

        internal static OdenekBilgi[] TavanGenelListele(VeriErisim veriErisim, Kullanici kullanan, string senaryoKod, Tertip kistasTertip)
        {
            if (!YetkiServis.OGYYetkisiVarMi(kullanan, null, kistasTertip, ENUMOGY.Gorme))
            {
                throw new Exception("Görme yetkiniz yok!");
            }

            UcgenKodBilgileri kodBilgileri = UcgenKodBilgileriBul(kullanan, kistasTertip);

            EkDegerParam[] ekBilgiler = new EkDegerParam[7];
            ekBilgiler[0] = new EkDegerParam(ENUMTabloKod.TABPEBOdenek, 0, 0, senaryoKod);
            ekBilgiler[1] = new EkDegerParam(ENUMTabloKod.TABMPTavan, 0, 0, "0");
            ekBilgiler[2] = new EkDegerParam(ENUMTabloKod.TABMPTavan, 0, 0, "1");
            ekBilgiler[3] = new EkDegerParam(ENUMTabloKod.TABTasraOdenek, 0, 0, "00.00");
            ekBilgiler[4] = new EkDegerParam(ENUMTabloKod.TABOdenekGonderme, 0, 0, "");
            ekBilgiler[5] = new EkDegerParam(ENUMTabloKod.TABOdenekGonderme, -1, 0, "12");
            ekBilgiler[6] = new EkDegerParam(ENUMTabloKod.TABPEBHarcama, -1, 0, "");

            OdenekBilgi[] obler = BHMv2.IsKurallari.UcgenServisi.ListeTertipOl(null, kullanan, kistasTertip, kodBilgileri, ekBilgiler, false, true, true, -1);
            return obler;
        }

        internal static MPTavanTertip[] TavanListele(VeriErisim veriErisim, Kullanici kullanan, string senaryoKod, Tertip kistasTertip, string kyKod, bool detayliMi, string saymanlikBirim)
        {
            if (detayliMi)
            {
                if (!YetkisiVarMiKY(kullanan, saymanlikBirim, kistasTertip, ENUMOGY.Gorme))
                    throw new Exception("Görme yetkiniz yok!");
            }
            else if (!YetkiServis.OGYYetkisiVarMi(kullanan, null, kistasTertip, ENUMOGY.Gorme))
            {
                throw new Exception("Görme yetkiniz yok!");
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, kullanan.KimlikBul());
            VeriErisim veHarcama = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriBUM", kullanan.KimlikBul());

            UcgenKodBilgileri kodBilgileri = UcgenKodBilgileriBul(kullanan, kistasTertip);

            string[] tertipler = TertipTanim.Listele(veriErisim, kullanan, kistasTertip, kodBilgileri);
            if (tertipler == null)
            {
                ve.VeriErisimKapat();
                throw new Exception("Verilen kriterlere uygun tertip bulunamadı.");
            }
            MPTavanTertip[] tertipOdenekler = new MPTavanTertip[tertipler.Length];
            IUZYServis uzyServis = UZY.Arac.Tanimla();
            for (int i = 0; i < tertipler.Length; i++)
            {
                tertipOdenekler[i] = new MPTavanTertip();
                tertipOdenekler[i].tertipOdenek = new TertipOdenek(3);
                tertipOdenekler[i].tertipOdenek.tertip = new Tertip(kistasTertip.yil, kistasTertip.tabloKod);
                tertipOdenekler[i].tertipOdenek.tertip.TertipAta(tertipler[i]);
                tertipOdenekler[i].tertipOdenek.odenek[0] = BHMv2Genel.UzayDoubleDegerGetir(kullanan, ve, uzyServis, "ABSODENEK", TNS.UZY.Arac.UzayKosulOl(tertipOdenekler[i].tertipOdenek.tertip.yil.ToString(), senaryoKod, "0", tertipOdenekler[i].tertipOdenek.tertip.TertipStrOl()), false, null); // Arac.OdenekBul();
                tertipOdenekler[i].tertipOdenek.odenek[1] = -1; // BHMv2Genel.UzayDoubleDegerGetir(kullanan, ve, uzyServis, "ABSMPTAVAN", TNS.UZY.Arac.UzayKosulOl(tertipOdenekler[i].tertip.yil.ToString(), tertipOdenekler[i].tertip.TertipStrOl(), "", "0"), false, null);
                tertipOdenekler[i].tertipOdenek.odenek[2] = -1; // BHMv2Genel.UzayDoubleDegerGetir(kullanan, ve, uzyServis, "ABSODENEK", TNS.UZY.Arac.UzayKosulOl(tertipOdenekler[i].tertip.yil.ToString(), tertipOdenekler[i].tertip.TertipStrOl(), "", "1"), false, null);
                tertipOdenekler[i].birimTavanlari = BirimTavanBul(kullanan, tertipOdenekler[i], detayliMi, kyKod);
            }
            veHarcama.VeriErisimKapat();
            ve.VeriErisimKapat();
            return tertipOdenekler;
        }

        private static MPTavanDetay[] BirimTavanBul(Kullanici kullanan, MPTavanTertip mpTavanTertip, bool detayliMi, string kyKod)
        {
            ArrayList al = new ArrayList();
            AltFaaliyet pebForm = new AltFaaliyet();
            pebForm.yil = mpTavanTertip.tertipOdenek.tertip.yil;
            pebForm.pebKod = mpTavanTertip.tertipOdenek.tertip.kodlar[4];

            pebForm = BHMv2AltFaaliyet.Oku(null, kullanan, pebForm);

            string[] sqller = new string[5 + (detayliMi ? 0 : 1)];
            string[] baglantilar = new string[5 + (detayliMi ? 0 : 1)];
            sqller[0] = TABBHMKURUMSALYAPI.ListeOlSql(detayliMi ? 11 : 3, kyKod);
            baglantilar[0] = "BaglantiSatiriBUM";
            sqller[1] = TABBHMODENEKGIDER.HarcamaBul(mpTavanTertip.tertipOdenek.tertip, -1, detayliMi ? 11 : 3, kyKod);
            baglantilar[1] = "BaglantiSatiriBUM";
            sqller[2] = TABBKDBIRIMBUTCEHAZIRLIK.TalepBul(mpTavanTertip.tertipOdenek.tertip, "00.00", detayliMi ? 11 : 3, kyKod);
            baglantilar[2] = "BaglantiSatiriBHMv2";
            sqller[3] = TABBHMODENEKGIDER.OdenekGondermeBul(mpTavanTertip.tertipOdenek.tertip, 0, detayliMi ? 11 : 3, kyKod);
            baglantilar[3] = "BaglantiSatiriBUM";
            sqller[4] = TABMPGTAVAN.TavanBul(mpTavanTertip.tertipOdenek.tertip, detayliMi, detayliMi ? 11 : 3, kyKod);
            baglantilar[4] = "BaglantiSatiriBHMv2";
            if (!detayliMi)
            {
                sqller[5] = TABMPGTAVAN.TavanBul(mpTavanTertip.tertipOdenek.tertip, true, 3, kyKod);
                baglantilar[5] = "BaglantiSatiriBHMv2";
            }
            TNS.UZY.IUZYServis uzyServis = TNS.UZY.Arac.Tanimla();

            SortedList sl = uzyServis.SorguBirlestir(sqller, baglantilar, new int[] { 0 });

            foreach (DictionaryEntry de in sl)
            {
                TNS.UZY.HTArray alAnahtar = (TNS.UZY.HTArray)de.Key;
                TNS.UZY.HTArray alVeri = (TNS.UZY.HTArray)de.Value;

                KurumsalYapi ky = new KurumsalYapi();

                ky.kod = OrtakFonksiyonlar.KodNoktaKoy("9.99.999.99999", alAnahtar.StrAl(0));
                ky.ad = alVeri.StrAl(0);
                bool ekle = true;
                if (detayliMi && ky.kod.Length > 8)
                {
                    ky.ad = "(" + KurumsalYapiAd(uzyServis, ky.kod.Substring(0, 8)) + ") " + ky.ad;
                }

                if (!TNS.BKD.Arac.KodKiyasla(mpTavanTertip.tertipOdenek.tertip.kodlar[0], ky.kurKod))
                {
                    ekle = false;
                }
                if (ekle && !pebForm.KurumsalYapiUyumluMu(ky, detayliMi ? 0 : 2))
                {
                    ekle = false;
                }
                if (ekle)
                {
                    MPTavanDetay mpTavanDetay = new MPTavanDetay();
                    mpTavanDetay.kurumsalYapi = ky;
                    mpTavanDetay.odenek = new double[5];

                    mpTavanDetay.odenek[0] = alVeri.DblAl(1);
                    mpTavanDetay.odenek[1] = alVeri.DblAl(2);
                    mpTavanDetay.odenek[2] = alVeri.DblAl(3);
                    if (detayliMi)
                    {
                        mpTavanDetay.odenek[3] = alVeri.DblAl(4);
                        mpTavanDetay.odenek[4] = 0;
                    }
                    else
                    {
                        mpTavanDetay.odenek[3] = alVeri.DblAl(4);
                        mpTavanDetay.odenek[4] = alVeri.DblAl(5);
                    }
                    al.Add(mpTavanDetay);
                }
            }
            return (MPTavanDetay[])al.ToArray(typeof(MPTavanDetay));
        }

        private static string KurumsalYapiAd(IUZYServis servis, string kurumsalKod)
        {
            return servis.UzayDegeriStr(null, "BHMKURUMSALYAPI", kurumsalKod, true);
        }

        internal static Sonuc MPTavanKaydet(VeriErisim veriErisim, Kullanici kullanan, string odenekKontrolSenaryo, TertipOdenek[] tertipOdenek, bool detayliMi, string silKYKod, bool toplamYaz, string saymanlikBirim)
        {
            TNS.YAT.IYATServis servisYat = TNS.YAT.Arac.Tanimla();
            if (tertipOdenek == null || tertipOdenek.Length == 0)
                throw new Exception("Gönderilen ödenek dizisi boş!");
            int yil = tertipOdenek[0].tertip.yil;
            Tertip toPeb = new Tertip(yil, ENUMTabloKod.TABPEBOdenek);

            ArrayList islenmisTertipler = new ArrayList();
            ArrayList islenmisKyler = new ArrayList();
            Hashtable toplamHash = new Hashtable();
            Hashtable afHash = new Hashtable();

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, kullanan.KimlikBul());
            VeriErisim veBUM = VeriErisim.VeriErisimBul(null, "BaglantiSatiriBUM", kullanan.KimlikBul());
            TNS.MUH.IMUHServis muhServis = TNS.MUH.Arac.Tanimla();
            try
            {
                Tertip t = new Tertip(yil, ENUMTabloKod.TABPEBOdenek);
                int eko2Uz = t.KodUzunlukBul(3, 2, true);
                int eko3Uz = t.KodUzunlukBul(3, 3, true);
                foreach (TertipOdenek to in tertipOdenek)
                {
                    string aktifPebKod = OrtakFonksiyonlar.DiziElemanAl(to.tertip.kodlar, 4, "");
                    if (yil != to.tertip.yil)
                        throw new Exception("Tertip yılları birbirinden farklı!");

                    if (to.tertip.tabloKod != ENUMTabloKod.TABTasraOdenek)
                        throw new Exception("Tertip yapısı Taşra ödeneği değil!");

                    toPeb.TertipAta(OrtakFonksiyonlar.DiziElemanAl(to.tertip.kodlar, 0, ""), OrtakFonksiyonlar.DiziElemanAl(to.tertip.kodlar, 1, ""), OrtakFonksiyonlar.DiziElemanAl(to.tertip.kodlar, 2, ""), OrtakFonksiyonlar.DiziElemanAl(to.tertip.kodlar, 3, ""), OrtakFonksiyonlar.DiziElemanAl(to.tertip.kodlar, 4, ""));

                    string toplamAnahtar;
                    if (detayliMi)
                        toplamAnahtar = toPeb.TertipStrOl() + "-" + ParcaAl(OrtakFonksiyonlar.DiziElemanAl(to.tertip.kodlar, 5, ""), 0, 3);
                    else
                        toplamAnahtar = toPeb.TertipStrOl();
                    double eskiToplam = 0;

                    if (toplamHash.ContainsKey(toplamAnahtar))
                    {
                        try
                        {
                            eskiToplam = (double)toplamHash[toplamAnahtar];
                        }
                        catch { eskiToplam = 0; }
                    }
                    else
                        eskiToplam = 0;

                    toplamHash[toplamAnahtar] = eskiToplam + to.odenek[0];

                    int yer = islenmisTertipler.IndexOf(toPeb.TertipStrOl());
                    if (yer < 0)
                    {
                        islenmisTertipler.Add(toPeb.TertipStrOl());
                        if (!toPeb.TamMi(3, eko2Uz, TNS.BKD.Arac.Eko2_3Bul(kullanan), eko3Uz))
                        {
                            throw new Exception(toPeb.TertipStrOl() + " tertibi,  tam değil!");
                        }
                        if (!detayliMi)
                        {
                            if (!YetkiServis.OGYYetkisiVarMi(kullanan, null, toPeb, ENUMOGY.Yazma))
                            {
                                throw new Exception("Yazma yetkiniz yok!");
                            }
                            int onayKod = OnayKodBul(ve, toPeb, 0);
                            if (onayKod == 2)
                                throw new Exception("Onaylı olduğu için kayıt yapamazsınız!!");
                            if (onayKod == 1 && !kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.SISTEMYONETICISI))
                                throw new Exception("Onaylı olduğu için kayıt yapamazsınız!!");
                        }

                        YAT.AltFaalBilgi afb;

                        afb = AltFaaliyetBilgiAl(afHash, aktifPebKod, true, servisYat, kullanan, yil, odenekKontrolSenaryo);
                    }
                    if (to.odenek[0] == 0) continue;
                    string kyKod = to.tertip.kodlar[5];
                    yer = islenmisKyler.IndexOf(kyKod);
                    if (yer < 0)
                    {
                        islenmisKyler.Add(kyKod);
                        if (!kyKod.Replace(".", "").StartsWith(silKYKod.Replace(".", "")))
                            throw new Exception(string.Format("Listelenmiş verilerle (Birim:{0}), kaydedilen veriler (Birim:{1}) arasında tutarsızlık var. !", silKYKod, kyKod));

                        if (detayliMi)
                        {
                            KurumsalYapi kY = new KurumsalYapi();
                            kY.kod = kyKod;
                            KurumsalYapi[] okunanlar = BHMv2KurumsalYapi.Listele(veBUM, kullanan, kY, false, true, false);
                            if (okunanlar == null || okunanlar.Length != 1)
                                throw new Exception("Verilen kurumsal yapı kodu " + kyKod + " bulunamadı!");
                            kY = okunanlar[0];

                            if (muhServis.BirimKapaliMi(kullanan, kY.saymanlikKod, kY.birimKod, kY.kurKod))
                                throw new Exception("Verilen kurumsal yapı kodu " + kyKod + " kapalı olduğundan masraf planı yapılamaz!");

                            if (!to.tertip.TamMi(3, eko2Uz, TNS.BKD.Arac.Eko2_3Bul(kullanan), eko3Uz))
                            {
                                throw new Exception(to.tertip.TertipStrOl() + " tertibi, tam değil!");
                            }
                            else if (!YetkisiVarMiKY(kullanan, saymanlikBirim, to.tertip, ENUMOGY.Yazma))
                            {
                                throw new Exception("Yazma yetkiniz yok!");
                            }
                            int onayKod = OnayKodBul(ve, toPeb, 1);
                            if (onayKod == 2)
                                throw new Exception("Onaylı olduğu için kayıt yapamazsınız!!");
                            if (onayKod == 1 && !kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.SISTEMYONETICISI))
                                throw new Exception("Onaylı olduğu için kayıt yapamazsınız!!");
                        }
                        else
                        {
                            if (kyKod.Length != 4)
                                throw new Exception("Kurumsal kod il değil!");
                        }

                        if (!BirimKurkodIliskisiVarMi(veBUM, kyKod, to.tertip.kodlar[0]))
                        {
                            throw new Exception(kyKod + " Kurumsal kodu ile " + to.tertip.kodlar[0] + " kodu ilişkili olmadığından kayıt yapamazsınız!");
                        }
                    }

                    if (!IlGecerliMi(kyKod, aktifPebKod, afHash))
                    {
                        throw new Exception($"Girdiğiniz birim kodunun ({kyKod}) ait olduğu il, ilgili alt faaliyet kodunun ({aktifPebKod}) yatırım projelerinde girilmiş olan illerden biri olmadığından, ödenek ayrılamaz!");
                    }
                }
            }
            catch (Exception ex)
            {
                veBUM.VeriErisimKapat();
                ve.VeriErisimKapat();
                throw ex;
            }

            IUZYServis uzyServis = UZY.Arac.Tanimla();
            foreach (DictionaryEntry de in toplamHash)
            {
                string tertipStr = (string)de.Key;
                double toplam = (double)de.Value;
                if (detayliMi)
                {
                    if (toplamYaz)
                    {
                    }
                    else
                    {
                        double tavan = uzyServis.UzayDegeriDbl(kullanan, "MPTAVAN", TNS.UZY.Arac.UzayKosulOl(yil.ToString(), "0", tertipStr.Replace(".", "")), false);

                        if (toplam > tavan)
                        {
                            ve.VeriErisimKapat();
                            throw new Exception(string.Format("Harcama birimleri için girmiş olduğunuz toplam ({0}), ilinize ayrılan tavandan ({1}) büyük!", toplam, tavan));
                        }
                    }
                }
                else
                {
                    double tavan = BHMv2Genel.UzayDoubleDegerGetir(kullanan, ve, uzyServis, "ABSODENEK", TNS.UZY.Arac.UzayKosulOl(yil.ToString(), odenekKontrolSenaryo, "0", tertipStr.Replace(".", "")), false, null);
                    if (toplam > tavan)
                    {
                        ve.VeriErisimKapat();
                        throw new Exception(string.Format("İller için girdiğiniz tavanların toplamı ({0:#,##0}), tertip ({2}) ödeneğinden ({1:#,##0}) büyük!", toplam, tavan, tertipStr));
                    }
                }
            }

            TABMPGTAVAN tabMpTavan = new TABMPGTAVAN(ve);
            Sonuc s = new Sonuc(true, "", "");
            ve.BeginTransaction();

            if (toplamYaz && detayliMi)
            {
                foreach (DictionaryEntry de in toplamHash)
                {
                    string tertipStr = (string)de.Key;
                    double toplam = (double)de.Value;
                    TertipOdenek to = new TertipOdenek(1);
                    to.tertip = new Tertip(yil, ENUMTabloKod.TABTasraOdenek);
                    to.tertip.TertipAta(tertipStr);
                    to.odenek[0] = toplam;
                    s = tabMpTavan.Sil(yil, 0, to.tertip, to.tertip.kodlar[5]);
                    if (s.islemSonuc)
                        s = tabMpTavan.Ekle(yil, 0, to);
                    if (!s.islemSonuc)
                        break;
                }
            }

            if (s.islemSonuc)
            {
                foreach (string tertipStr in islenmisTertipler)
                {
                    toPeb.TertipAta(tertipStr);
                    s = tabMpTavan.Sil(yil, detayliMi ? 1 : 0, toPeb, silKYKod);
                    if (!s.islemSonuc)
                        break;
                }
            }

            if (s.islemSonuc)
            {
                foreach (TertipOdenek to in tertipOdenek)
                {
                    if (to.odenek[0] != 0)
                    {
                        s = tabMpTavan.Ekle(yil, detayliMi ? 1 : 0, to);
                        if (!s.islemSonuc)
                            break;
                    }
                }
            }

            if (s.islemSonuc)
                ve.Commit();
            else
                ve.Rollback();
            ve.VeriErisimKapat();
            return s;
        }

        public static bool IlGecerliMi(string kyKod, string pebKod, Hashtable afHash)
        {
            if (string.IsNullOrWhiteSpace(kyKod)) return true;
            if (kyKod.Length < 3) return true;
            int ilKod = OrtakFonksiyonlar.ConvertToInt(kyKod.Replace(".", "").Substring(1, 2), 0);
            if (ilKod == 0) return true;
            AltFaalBilgi afb = AltFaaliyetBilgiAl(afHash, pebKod, false, null, null, -1, null);
            if (afb == null || afb.iller == null || afb.iller.Count == 0) return true;
            foreach (ProjeIlIlce item in afb.iller)
            {
                if (item.ilKod == ilKod || item.ilKod == 999) return true;
            }
            return false;
        }

        public static AltFaalBilgi AltFaaliyetBilgiAl(Hashtable afHash, string pebKod, bool yoksaEkle, IYATServis servisYat, Kullanici kullanan, int yil, string odenekKontrolSenaryo)
        {
            AltFaalBilgi afb;
            try
            {
                afb = (AltFaalBilgi)afHash[pebKod];
            }
            catch
            {
                afb = null;
            }

            if (afb == null && yoksaEkle)
            {
                afb = servisYat.AltFaaliyetBilgiBul(kullanan, yil, pebKod, "", odenekKontrolSenaryo, true);
                if (afb == null)
                    afb = new YAT.AltFaalBilgi();
                afHash[pebKod] = afb;
            }
            return afb;
        }

        private static string ParcaAl(string str, int basla, int bitis)
        {
            if (string.IsNullOrWhiteSpace(str)) return "";
            str = str.Replace(".", "");
            if (str.Length < basla) return "";
            if (str.Length < basla + bitis) return str.Substring(basla);
            return str.Substring(basla, bitis);
        }

        private static bool BirimKurkodIliskisiVarMi(VeriErisim ve, string kyKod, string kurKod)
        {
            TABBHMKURUMSALYAPI tabKurYapi = new TABBHMKURUMSALYAPI(ve);
            int say = tabKurYapi.KurumsalKodSay(kyKod, kurKod);
            if (say == 0) return false;
            return true;
        }

        internal static Sonuc MPIlIzniDegistir(VeriErisim veriErisim, Kullanici kullanan, Tertip tertip)
        {
            if (!YetkiServis.OGYYetkisiVarMi(kullanan, null, tertip, ENUMOGY.Yazma))
                throw new Exception("Yazma hakkınız yok!");
            if (!tertip.TamMi(3, TNS.BHMv2.Arac.Eko2UzBul(tertip.yil)))
                throw new Exception("Tertip tam değil!");

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, kullanan.KimlikBul());
            TABMPGTAVAN tabMpTavan = new TABMPGTAVAN(ve);
            Sonuc s;
            bool izinVar;
            try
            {
                izinVar = MPTavanIlIzniBul(ve, kullanan, tertip);
            }
            catch (Exception ex)
            {
                ve.VeriErisimKapat();
                throw ex;
            }
            ve.BeginTransaction();
            if (izinVar)
                s = tabMpTavan.TertipIzniSil(tertip);
            else
                s = tabMpTavan.TertipIzniEkle(tertip);
            if (s.islemSonuc)
            {
                ve.Commit();
                s.anahtar = izinVar ? "0" : "1";
            }
            else
                ve.Rollback();
            ve.VeriErisimKapat();
            return s;
        }

        internal static bool MPTavanIlIzniBul(VeriErisim veriErisim, Kullanici kullanan, Tertip tertip)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, kullanan.KimlikBul());
            TABMPGTAVAN tabMpTavan = new TABMPGTAVAN(ve);
            int say = tabMpTavan.TertipIzniSay(tertip);
            string tut = ve.hata;
            ve.VeriErisimKapat();
            if (!string.IsNullOrEmpty(tut))
                throw new Exception(tut);
            if (say == 0)
                return false;
            else
                return true;
        }

        private static bool YetkisiVarMiKY(Kullanici kullanan, string saymanlikBirim, Tertip tertip, ENUMOGY OGY)
        {
            if (kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.SISTEMYONETICISI))
                return true;
            if (OGY == ENUMOGY.OnayKaldir) return false;

            if (string.IsNullOrWhiteSpace(saymanlikBirim))
            {
                bool yetkiVar = YetkiServis.OGYYetkisiVarMi(kullanan, null, tertip, OGY);
                if (yetkiVar) return true;
            }
            if (OGY == ENUMOGY.Onaylama) return false;
            if (string.IsNullOrEmpty(saymanlikBirim))
                return false;

            string[] saymanlikBirimler = saymanlikBirim.Split('-');
            MUH.ENUMOGY muhYetki = MUH.ENUMOGY.Gorme;

            if (OGY == ENUMOGY.Gorme)
                muhYetki = MUH.ENUMOGY.Gorme;
            else if (OGY == ENUMOGY.Yazma)
                muhYetki = MUH.ENUMOGY.Yazma;
            else
                return false;
            return servisMuh.MuhasebeBirimYetkiVarMi(kullanan, TNS.MUH.Arac.DiziElemanAl(saymanlikBirimler, 0, ""), TNS.MUH.Arac.DiziElemanAl(saymanlikBirimler, 1, ""), muhYetki);
        }

        private static int OnayKodBul(VeriErisim veriErisim, Tertip tertip, int detayMi)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, null);
            TABMPGTAVAN tab = new TABMPGTAVAN(ve);

            int birimKod = tab.OnayKoduBul(tertip.yil, detayMi, tertip.kodlar[4]);

            ve.VeriErisimKapat();
            return birimKod;
        }

        internal static TertipOdenek[] TavanIlListele(VeriErisim veriErisim, Kullanici kullanan, Tertip kistasTertip, string saymanlikBirim)
        {
            string kyKod = kistasTertip.kodlar[5];
            if (!YetkisiVarMiKY(kullanan, saymanlikBirim, kistasTertip, ENUMOGY.Gorme))
                throw new Exception("Görme yetkiniz yok!");

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, kullanan.KimlikBul());
            ArrayList al = new ArrayList();
            IUZYServis uzyServis = UZY.Arac.Tanimla();

            string[] sorgular = new string[3];
            string[] baglantilar = new string[3];

            sorgular[0] = TABMPGTAVAN.ListeleTavan(kistasTertip, 0);
            baglantilar[0] = VERIERISIMAD;

            sorgular[1] = TABMPGTAVAN.ListeleTavan(kistasTertip, 1);
            baglantilar[1] = VERIERISIMAD;

            sorgular[2] = TABMPGTAVAN.ListeleIzinSQL(kistasTertip);
            baglantilar[2] = VERIERISIMAD;

            SortedList sl = uzyServis.SorguBirlestir(sorgular, baglantilar, new int[] { 0, 1, 2, 3, 4 });
            Hashtable htEko = new Hashtable();
            Hashtable htProg = new Hashtable();
            foreach (DictionaryEntry de in sl)
            {
                TNS.UZY.HTArray alAnahtar = (TNS.UZY.HTArray)de.Key;
                TNS.UZY.HTArray alVeri = (TNS.UZY.HTArray)de.Value;

                TertipOdenek to = new TertipOdenek(3);
                to.tertip = new Tertip(kistasTertip.yil, kistasTertip.tabloKod);
                to.tertip.TertipAta(alAnahtar.StrAl(0), alAnahtar.StrAl(1), alAnahtar.StrAl(2), alAnahtar.StrAl(3), alAnahtar.StrAl(4));
                to.tertip.Noktala();
                to.odenek[0] = alVeri.DblAl(0);
                to.odenek[1] = alVeri.DblAl(1);
                to.odenek[2] = alVeri.IntAl(2);
                to.tertip.adlar[3] = BKDBirimButceHazirlik.EkoAdBul(kullanan, uzyServis, htEko, kistasTertip.yil, to.tertip.kodlar[3]);
                to.tertip.adlar[4] = BKDBirimButceHazirlik.PebAdBul(kullanan, uzyServis, htProg, kistasTertip.yil, to.tertip.kodlar[4]);

                if (to.odenek[0] != 0 || to.odenek[1] != 0)
                    al.Add(to);
            }
            return (TertipOdenek[])al.ToArray(typeof(TertipOdenek));
        }

        public static string AlanIsimleriBul(Kullanici kullanan, Tertip kistasTertip, UcgenKodBilgileri ukb, out int[] tertipKodYer, out int odenekYer, bool uzun)
        {
            TNS.BKD.IBKDServis bkd = TNS.BKD.Arac.Tanimla();
            string alanlarListe = "";
            odenekYer = 0;
            tertipKodYer = new int[ukb.BoyutSayisi];
            for (int i = 0; i < ukb.BoyutSayisi; i++)
            {
                UcgenKodBilgi uk = (UcgenKodBilgi)ukb[i];
                if (uk.kirilimSay > 0)
                {
                    ButceKodYapi bky = bkd.ButceKodBilgiBul(kistasTertip.yil, uk.kodNo, "");
                    string icAlanAd;
                    string tabloAlanAd = bky.vtAlanKod;
                    if (bky.kodNo == 17)
                    {
                        if (uzun)
                        {
                            tabloAlanAd = "KURUMSALYAPIKOD";
                        }
                        else
                        {
                            tabloAlanAd = "KYKOD";
                        }
                    }
                    if (uk.kirilimSay >= bky.bolumSay)
                        icAlanAd = tabloAlanAd;
                    else
                    {
                        if (bky.kodNo == 4 && TNS.BKD.Arac.Eko2_3Bul(kullanan))
                        {
                            icAlanAd = TNS.BKD.Arac.Eko23AlanOl(kistasTertip.yil, bky, "", tabloAlanAd);
                        }
                        else
                            icAlanAd = "SubString(" + tabloAlanAd + ", 1, " + bky.KirilimToplamUzunluk(uk.kirilimSay) + ")";
                    }

                    if (!string.IsNullOrEmpty(alanlarListe)) alanlarListe += ", ";
                    alanlarListe += icAlanAd;
                    tertipKodYer[odenekYer] = TertipYerBul(kistasTertip, uk.kodNo);
                    odenekYer++;
                }
            }
            return alanlarListe;
        }

        public static void TertipAdiBul(IUZYServis uzyServis, Tertip tertip, Hashtable[] ht)
        {
            tertip.Noktala();
            tertip.AdlariBul(uzyServis, ht);

            if (!string.IsNullOrEmpty(tertip.adlar[3]))
            {
                if (!string.IsNullOrEmpty(tertip.adlar[4])) tertip.tertipAdi = tertip.adlar[3] + "(" + tertip.adlar[4] + ")";
                else if (!string.IsNullOrEmpty(tertip.adlar[0])) tertip.tertipAdi = tertip.adlar[3] + "(" + tertip.adlar[0] + ")";
            }
            else if (!string.IsNullOrEmpty(tertip.adlar[4]))
                if (!string.IsNullOrEmpty(tertip.adlar[0])) tertip.tertipAdi = tertip.adlar[4] + "(" + tertip.adlar[0] + ")";

            if (string.IsNullOrEmpty(tertip.tertipAdi))
            {
                for (int i = 0; i < tertip.adlar.Length - 1; i++)
                {
                    if (!string.IsNullOrEmpty(tertip.adlar[i]))
                    {
                        if (!string.IsNullOrEmpty(tertip.tertipAdi)) tertip.tertipAdi += "-";
                        tertip.tertipAdi += tertip.adlar[i];
                    }
                }
            }
        }

        internal static TertipOdenek[] MPListeleRapor(VeriErisim veriErisim, Kullanici kullanan, Tertip kistasTertip, UcgenKodBilgileri ukb, bool ekBilgili, string saymanlikBirim)
        {
            TNS.UZY.IUZYServis uzyServis = TNS.UZY.Arac.Tanimla();
            if (!YetkisiVarMiKY(kullanan, saymanlikBirim, kistasTertip, ENUMOGY.Gorme))
                throw new Exception("Görme yetkiniz yok!");

            int[] tertipKodYer;
            int odenekYer;
            string alanAd = AlanIsimleriBul(kullanan, kistasTertip, ukb, out tertipKodYer, out odenekYer, false);
            if (odenekYer == 0)
                throw new Exception("En az bir alan belirtmelisiniz!");

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, kullanan.KimlikBul());
            TABMPGTAVAN tabMpTavan = new TABMPGTAVAN(ve);
            IDataReader dr = tabMpTavan.ListeleRapor(alanAd, kistasTertip);
            if (dr == null)
            {
                ve.VeriErisimKapat();
                throw new Exception(ve.hata);
            }
            ArrayList al = new ArrayList();
            int alanSay = 1;
            EkDegerParam[] ek = new EkDegerParam[3];
            if (ekBilgili)
            {
                alanSay = 4;
                ek[0] = new EkDegerParam(ENUMTabloKod.TABMasrafPlani, -1, 0, "");
                ek[1] = new EkDegerParam(ENUMTabloKod.TABOdenekGonderme, -1, 0, "");
                ek[2] = new EkDegerParam(ENUMTabloKod.TABOdemeEmri, -1, 0, "");
            }
            Hashtable[] ht = HashtableDiziOl(kistasTertip.KodNoSay());
            while (dr.Read())
            {
                TertipOdenek to = new TertipOdenek(alanSay);
                to.tertip = new Tertip(kistasTertip.yil, kistasTertip.tabloKod);
                for (int i = 0; i < odenekYer; i++)
                {
                    to.tertip.TertibeKodEkle(tertipKodYer[i], VeriErisim.DegerAl(dr, i, ""), "");
                }
                TertipAdiBul(uzyServis, to.tertip, ht);
                to.odenek[0] = VeriErisim.DegerAl(dr, odenekYer, 0.0);
                if (ekBilgili)
                {
                    to.odenek[1] = ek[0].EkDegerHesaplaDbl(uzyServis, kullanan, kistasTertip.yil, to.tertip.TertipStrOl());
                    to.odenek[2] = ek[1].EkDegerHesaplaDbl(uzyServis, kullanan, kistasTertip.yil, to.tertip.TertipStrOl());
                    to.odenek[3] = ek[2].EkDegerHesaplaDbl(uzyServis, kullanan, kistasTertip.yil, to.tertip.TertipStrOl());
                }
                to.aciklama = null;
                al.Add(to);
            }
            dr.Close();
            ve.VeriErisimKapat();
            return (TertipOdenek[])al.ToArray(typeof(TertipOdenek));
        }

        public static Hashtable[] HashtableDiziOl(int diziBoy)
        {
            Hashtable[] ht = new Hashtable[diziBoy];
            for (int i = 0; i < diziBoy; i++)
            {
                ht[i] = new Hashtable();
            }
            return ht;
        }

        private static int TertipYerBul(Tertip kistasTertip, int kodNo)
        {
            for (int i = 0; i < kistasTertip.kodlar.Length; i++)
            {
                if (kistasTertip.KodNolar(i) == kodNo)
                    return i;
            }
            return -1;
        }

        public static ObjectArray MPFazlaGonderilen(VeriErisim veriErisim, Kullanici kullanan, int detayMi, Tertip kistasTertip)
        {
            ObjectArray oa = new ObjectArray();
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, kullanan.KimlikBul());
            TABMPGTAVAN tabMpTavan = new TABMPGTAVAN(ve);

            IDataReader dr = tabMpTavan.HataliTertipleriBul(kistasTertip, detayMi);
            if (dr == null)
            {
                oa.sonuc.DegerAta(ve.hata);
                ve.VeriErisimKapat();
                return oa;
            }
            while (dr.Read())
            {
                TertipOdenekBilgi to = new TertipOdenekBilgi(String.Format("{0}-{1}-{2}-{3}-{4}-{5}", VeriErisim.DegerAl(dr, 0, ""), VeriErisim.DegerAl(dr, 1, ""), VeriErisim.DegerAl(dr, 2, ""), VeriErisim.DegerAl(dr, 3, ""), VeriErisim.DegerAl(dr, 4, ""), VeriErisim.DegerAl(dr, 5, "")), VeriErisim.DegerAl(dr, 6, 0.0), VeriErisim.DegerAl(dr, 7, 0.0));
                oa.Ekle(to);
            }
            dr.Close();
            ve.VeriErisimKapat();
            return oa;
        }

        internal static Sonuc Onayla(VeriErisim veriErisim, Kullanici kullanan, int yil, int detayMi, string pebKod, int onayKod)
        {
            if ((onayKod == 2 || onayKod == 0) && !kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.SISTEMYONETICISI))
                return new Sonuc(false, "Onaylama/Onay kaldırma yetkiniz yok!", "");
            ENUMOGY onayIslem;
            if (onayKod == 0)
                onayIslem = ENUMOGY.OnayKaldir;
            else
                onayIslem = ENUMOGY.Onaylama;
            Tertip t = new Tertip(yil, ENUMTabloKod.TABPEBOdenek, "*", "", "", "", pebKod);
            if (!YetkiServis.OGYYetkisiVarMi(kullanan, null, t, onayIslem))
                return new Sonuc(false, "Onaylama yetkiniz yok!", "");

            if (pebKod.Length < 2)
            {
                return new Sonuc(false, string.Format("Girdiğiniz alt faaliyet kodu({0}), 2 hane veya detay olmalı!", pebKod), "");
            }

            IBKDServis bkdServis = TNS.BKD.Arac.Tanimla();

            ABSKodlarVAN kv = bkdServis.KodListeleSeviye(yil, "14", pebKod, 4, true);
            if (!kv.sonuc.islemSonuc)
                return kv.sonuc;

            Sonuc s = kv.sonuc;
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, kullanan.KimlikBul());
            TABMPGTAVAN tabMpTavan = new TABMPGTAVAN(ve);

            ve.BeginTransaction();
            for (int i = 0; i < kv.absKodlar.Count; i++)
            {
                pebKod = ((ABSKod)kv.absKodlar[i]).kod;
                s = tabMpTavan.Onayla(yil, detayMi, pebKod, onayKod);
                if (!s.islemSonuc) break;
            }

            if (s.islemSonuc)
                ve.Commit();
            else
                ve.Rollback();
            ve.VeriErisimKapat();
            return s;
        }

        private static TNS.MUH.IMUHServis _servisMuh = null;

        public static TNS.MUH.IMUHServis servisMuh
        {
            get
            {
                if (_servisMuh == null) _servisMuh = TNS.MUH.Arac.Tanimla();
                return _servisMuh;
            }
        }
    }
}
