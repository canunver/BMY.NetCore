using System.Data;
using TNS.KYM;
using TNS.BHMv2.IsKurallari.Veri;
using TNS.Veri;
using System;

namespace TNS.BHMv2.IsKurallari
{
    /// <summary>
    /// 
    /// </summary>
    public class BHMv2Proje
    {
        /// <summary>
        /// Veri tabanı bağlantısı, web.config dosyasında bu bağlantı ile ile ilgili parametreler bulunmalıdır.
        /// </summary>
        public static string VERIERISIMAD = "BaglantiSatiriBHMv2";

        public static Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, ProjeTeklif form)
        {
            ProjeTeklif okunan;
            Sonuc sonuc = Kontrol("KAYDET", null, kullanan, form, out okunan);
            if (!sonuc.islemSonuc)
                return sonuc;

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));
            ve.BeginTransaction();

            TABProje to = new TABProje(ve);

            if (form.durum == ENUMProjeDurum.TANIMSIZ)
                form.durum = ENUMProjeDurum.DEVAMEDIYOR;

            sonuc = to.Kaydet(form);

            if (sonuc.islemSonuc)
            {
                sonuc.anahtar = form.kod.ToString();
                ve.Commit();

                Sonuc sonucAciklama = BHMv2ProjeAciklama.Kaydet(null, kullanan, form.aciklamalar);
            }
            else
                ve.Rollback();

            ve.VeriErisimKapat();
            return sonuc;
        }

        public static Sonuc Sil(VeriErisim veriErisim, Kullanici kullanan, ProjeTeklif form)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));
            ProjeTeklif okunan;
            Sonuc sonuc = Kontrol("SIL", ve, kullanan, form, out okunan);
            if (sonuc.islemSonuc)
            {
                TABProje to = new TABProje(ve);

                ve.BeginTransaction();
                sonuc = to.Sil(form.kod);

                if (sonuc.islemSonuc)
                    ve.Commit();
                else
                    ve.Rollback();
            }

            ve.VeriErisimKapat();
            return sonuc;
        }

        public static ObjectArray Listele(VeriErisim veriErisim, Kullanici kullanan, ProjeTeklif form)
        {
            ObjectArray liste = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));
            TABProje to = new TABProje(ve);
            IDataReader dr = to.Listele(form, OrtakFonksiyonlar.YetkiTipiDegeriBul(kullanan.yetkiTipiDegerleri, "Proje Görebileceği Birimler"));
            if (dr != null)
            {
                while (dr.Read())
                {
                    int sonSira = (int)TABProje.ENUMAlanlar.SPHEDEFKOD + 1;

                    ProjeTeklif b = new ProjeTeklif();
                    b.kod = VeriErisim.DegerAl(dr, (int)TABProje.ENUMAlanlar.KOD, "");
                    b.sorumluBirim.birimKod = VeriErisim.DegerAl(dr, (int)TABProje.ENUMAlanlar.SORUMLUBIRIMKOD, "");
                    b.sorumluBirim.muhasebeKod = VeriErisim.DegerAl(dr, (int)TABProje.ENUMAlanlar.SORUMLUMUHASEBEKOD, 0);
                    b.sorumluBirim.kurKod = VeriErisim.DegerAl(dr, (int)TABProje.ENUMAlanlar.SORUMLUKURKOD, "");
                    b.ad = VeriErisim.DegerAl(dr, (int)TABProje.ENUMAlanlar.AD, "");
                    b.sektorKod = VeriErisim.DegerAl(dr, (int)TABProje.ENUMAlanlar.SEKTORKOD, "");
                    b.baslamaYil = VeriErisim.DegerAl(dr, (int)TABProje.ENUMAlanlar.BASLAMAYIL, 0);
                    b.bitisYil = VeriErisim.DegerAl(dr, (int)TABProje.ENUMAlanlar.BITISYIL, 0);
                    b.durum = (ENUMProjeDurum)VeriErisim.DegerAl(dr, (int)TABProje.ENUMAlanlar.DURUM, 0);
                    b.yatProjeKod = VeriErisim.DegerAl(dr, (int)TABProje.ENUMAlanlar.YATPROJEKOD, 0);
                    b.yatAltProjeKod = VeriErisim.DegerAl(dr, (int)TABProje.ENUMAlanlar.YATALTPROJEKOD, 0);
                    b.yatProjeYil = VeriErisim.DegerAl(dr, (int)TABProje.ENUMAlanlar.YATPROJEYIL, 0);
                    b.spHedefKod = VeriErisim.DegerAl(dr, (int)TABProje.ENUMAlanlar.SPHEDEFKOD, "");
                    b.yatProjeli = !(string.IsNullOrWhiteSpace(b.projeNo));
                    b.sorumluBirim.muhAd = VeriErisim.DegerAl(dr, sonSira++, "");
                    b.sorumluBirim.birimAd = VeriErisim.DegerAl(dr, sonSira++, "");
                    b.sektorAd = VeriErisim.DegerAl(dr, sonSira++, "");
                    b.sorumluBirim.il = VeriErisim.DegerAl(dr, sonSira++, "");
                    if (string.IsNullOrWhiteSpace(b.sorumluBirim.il) && b.sorumluBirim.muhasebeKod < 1000) b.sorumluBirim.il = "MERKEZ";

                    b.dptProjeNo = VeriErisim.DegerAl(dr, sonSira++, "");
                    b.dptProjeAd = VeriErisim.DegerAl(dr, sonSira++, "");
                    b.dptAltProjeAd = VeriErisim.DegerAl(dr, sonSira++, "");

                    if (!string.IsNullOrWhiteSpace(form.kod))
                    {
                        ObjectArray aciklamalar = BHMv2ProjeAciklama.Listele(veriErisim, kullanan, new ProjeAciklama() { projeKod = b.kod });

                        b.aciklamalar = (ProjeAciklama[])aciklamalar.objeler.ToArray(typeof(ProjeAciklama));
                    }

                    liste.Ekle(b);
                }
                dr.Close();
            }
            else
            {
                liste.sonuc.hataStr = "İşlem sırasında beklenmedik bir hata oluştu:" + ve.hata;
            }
            ve.VeriErisimKapat();

            return liste;
        }

        private static Sonuc Kontrol(string tip, VeriErisim ve, Kullanici kullanan, ProjeTeklif form, out ProjeTeklif okunan)
        {
            Sonuc sonuc = new Sonuc();
            if (!string.IsNullOrEmpty(form.kod))
            {
                okunan = ProjeOku(kullanan, form.kod);
                if (okunan == null)
                    sonuc.DegerAta("Değiştirilecek form bulunamadı!");
            }
            else
                okunan = null;

            if (okunan != null)
            {
                YetkisiVarMi(kullanan, okunan, ENUMOGY.Yazma, sonuc);
                if (!sonuc.islemSonuc) return sonuc;
            }
            if (tip == "SIL")
            {
                if (okunan == null)
                    sonuc.DegerAta("Silinecek kaydı seçiniz.");
            }
            else if (tip == "KAYDET" || tip == "GUNCELLE")
            {
                if (form.sorumluBirim.muhasebeKod == 0)
                    sonuc.DegerAta("Sorumlu muhasebe kodu boş bırakılamaz!");
                if (string.IsNullOrEmpty(form.sorumluBirim.birimKod))
                    sonuc.DegerAta("Sorumlu birim bilgisi boş bırakılamaz!");
                if (string.IsNullOrEmpty(form.ad))
                    sonuc.DegerAta("Proje adı boş bırakılamaz!");
                if (string.IsNullOrEmpty(form.sektorKod))
                    sonuc.DegerAta("Sektör kodu boş bırakılamaz!");
                if (form.baslamaYil == 0)
                    sonuc.DegerAta("Başlangıç yılı boş bırakılamaz!");
                if (form.bitisYil == 0)
                    sonuc.DegerAta("Bitiş yılı boş bırakılamaz!");
                if (form.bitisYil < form.baslamaYil)
                    sonuc.DegerAta("Bitiş yılı, başlama yılından küçük olamaz!");
                if (string.IsNullOrEmpty(form.spHedefKod))
                    sonuc.DegerAta("Stratejik hedef seçilmelidir!");

                if (sonuc.islemSonuc)
                    YetkisiVarMi(kullanan, form, ENUMOGY.Yazma, sonuc);

                if (sonuc.islemSonuc)
                {
                    if (okunan != null && okunan.yatProjeYil > 0)
                    {
                        if ((form.yatProjeYil > 0) && (okunan.yatProjeYil != form.yatProjeYil || okunan.yatProjeKod != form.yatProjeKod || okunan.yatAltProjeKod != form.yatAltProjeKod))
                            sonuc.DegerAta("Proje çakışmalarını engellemek amacıyla, kaydedilen yatırım projesinin değiştirilmesine izin verilmemektedir!");
                    }
                    else if (form.yatProjeYil > 0)
                    {
                        int sayi = YatProjeSayiBul(kullanan, form);
                        if (sayi > 0)
                        {
                            sonuc.DegerAta($"Kaydedilmeye çalışılan proje zaten kayıtlı olduğundan; kayıt işlemi iptal edildi!");
                        }
                    }
                }
            }
            return sonuc;
        }

        private static int YatProjeSayiBul(Kullanici kullanan, ProjeTeklif form)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(null, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));
            TABProje to = new TABProje(ve);

            int say = to.YatProjeSayiBul(form);
            if (!string.IsNullOrWhiteSpace(ve.hata))
                say = -1;
            ve.VeriErisimKapat();
            return say;
        }

        private static void YetkisiVarMi(Kullanici kullanan, ProjeTeklif okunan, ENUMOGY ogy, Sonuc sonuc)
        {
            string yetkiKod;
            if (ogy == ENUMOGY.Onaylama)
                yetkiKod = "Proje Onaylayabileceği Birimler";
            else if (ogy == ENUMOGY.Yazma)
                yetkiKod = "Proje Kaydedebileceği Birimler";
            else if (ogy == ENUMOGY.Gorme)
                yetkiKod = "Proje Kaydedebileceği Birimler";
            else
                yetkiKod = "";

            bool tut;
            if (yetkiKod != "")
                tut = TNS.MUH.Yetki.SaymanliBirimYetkiVarMi(kullanan, okunan.sorumluBirim.muhasebeKod, okunan.sorumluBirim.birimKod, ogy == ENUMOGY.Gorme, yetkiKod);
            else
                tut = false;
            if (!tut)
                sonuc.DegerAta($"Bu birim ({okunan.sorumluBirim.muhasebeKod}-{okunan.sorumluBirim.birimKod}) üzerinde işlem yapma yetkiniz bulunmamaktadır!");
        }

        private static ProjeTeklif ProjeOku(Kullanici kullanan, string kod)
        {
            ObjectArray oa = Listele(null, kullanan, new ProjeTeklif() { kod = kod });
            if (!oa.sonuc.islemSonuc) return null;
            if (oa.ObjeSayisi != 1) return null;
            return (ProjeTeklif)oa.objeler[0];
        }

        public static bool KayitVarMi(VeriErisim veriErisim, Kullanici kullanan, string projeKod)
        {
            bool kayitVar = false;
            if (!string.IsNullOrWhiteSpace(projeKod))
            {
                VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));
                TABProje to = new TABProje(ve);
                kayitVar = to.KayitVarMi(projeKod);
                ve.VeriErisimKapat();
            }
            return kayitVar;
        }
    }
}
