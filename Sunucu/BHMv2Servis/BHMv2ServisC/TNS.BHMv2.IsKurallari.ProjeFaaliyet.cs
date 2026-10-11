using System;
using System.Data;
using TNS.KYM;
using TNS.BHMv2.IsKurallari.Veri;
using TNS.Veri;

namespace TNS.BHMv2.IsKurallari
{
    /// <summary>
    /// 
    /// </summary>
    public class BHMv2ProjeFaaliyet
    {
        /// <summary>
        /// Veri tabanı bağlantısı, web.config dosyasında bu bağlantı ile ile ilgili parametreler bulunmalıdır.
        /// </summary>
        public static string VERIERISIMAD = "BaglantiSatiriBHMv2";

        public static Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, ProjeFaaliyet form)
        {
            Sonuc sonuc = Kontrol("KAYDET", null, kullanan, form);
            if (!sonuc.islemSonuc)
                return sonuc;

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));
            ve.BeginTransaction();

            TABProjeFaaliyet to = new TABProjeFaaliyet(ve);

            sonuc = to.Kaydet(form);

            if (sonuc.islemSonuc)
            {
                sonuc.anahtar = form.kod;
                ve.Commit();
            }
            else
                ve.Rollback();

            ve.VeriErisimKapat();
            return sonuc;
        }

        public static Sonuc Sil(VeriErisim veriErisim, Kullanici kullanan, ProjeFaaliyet form)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));

            Sonuc sonuc = Kontrol("SIL", ve, kullanan, form);
            if (sonuc.islemSonuc)
            {
                TABProjeFaaliyet to = new TABProjeFaaliyet(ve);

                ve.BeginTransaction();
                sonuc = to.Sil(form);

                if (sonuc.islemSonuc)
                    ve.Commit();
                else
                    ve.Rollback();
            }

            ve.VeriErisimKapat();
            return sonuc;
        }

        public static ObjectArray Listele(VeriErisim veriErisim, Kullanici kullanan, ProjeFaaliyet form)
        {
            ObjectArray liste = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));
            TABProjeFaaliyet to = new TABProjeFaaliyet(ve);
            IDataReader dr = to.Listele(form, kullanan.kullaniciKodu);
            if (dr != null)
            {
                while (dr.Read())
                {
                    int sonSira = (int)TABProjeFaaliyet.ENUMAlanlar.TAHMINIMALIYET + 1;

                    ProjeFaaliyet b = new ProjeFaaliyet();
                    b.projeKod = VeriErisim.DegerAl(dr, (int)TABProjeFaaliyet.ENUMAlanlar.PROJEKOD, "");
                    b.kod = VeriErisim.DegerAl(dr, (int)TABProjeFaaliyet.ENUMAlanlar.KOD, "");
                    b.ad = VeriErisim.DegerAl(dr, (int)TABProjeFaaliyet.ENUMAlanlar.AD, "");
                    b.yil = VeriErisim.DegerAl(dr, (int)TABProjeFaaliyet.ENUMAlanlar.YIL, 0);
                    b.basTarih = VeriErisim.DegerAl(dr, (int)TABProjeFaaliyet.ENUMAlanlar.BASTARIH);
                    b.bitTarih = VeriErisim.DegerAl(dr, (int)TABProjeFaaliyet.ENUMAlanlar.BITTARIH);
                    b.anaFaaliyet = VeriErisim.DegerAl(dr, (int)TABProjeFaaliyet.ENUMAlanlar.ANAFAALIYET, "");
                    b.sorumluBirim.birimKod = VeriErisim.DegerAl(dr, (int)TABProjeFaaliyet.ENUMAlanlar.SORUMLUBIRIMKOD, "");
                    b.sorumluBirim.muhasebeKod = VeriErisim.DegerAl(dr, (int)TABProjeFaaliyet.ENUMAlanlar.SORUMLUMUHASEBEKOD, 0);
                    b.sorumluBirim.kurKod = VeriErisim.DegerAl(dr, (int)TABProjeFaaliyet.ENUMAlanlar.SORUMLUKURKOD, "");
                    b.izleyenKod = VeriErisim.DegerAl(dr, (int)TABProjeFaaliyet.ENUMAlanlar.IZLEYENKOD, "");
                    b.izleyenAd = VeriErisim.DegerAl(dr, (int)TABProjeFaaliyet.ENUMAlanlar.IZLEYENAD, "");
                    b.tahminiMaliyet = VeriErisim.DegerAl(dr, (int)TABProjeFaaliyet.ENUMAlanlar.TAHMINIMALIYET, 0.0);

                    b.anaFaaliyetAd = VeriErisim.DegerAl(dr, sonSira++, "");
                    b.sorumluBirim.birimAd = VeriErisim.DegerAl(dr, sonSira++, "");

                    b.projeAd = VeriErisim.DegerAl(dr, sonSira++, "");

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

        private static Sonuc Kontrol(string tip, VeriErisim ve, Kullanici kullanan, ProjeFaaliyet form)
        {
            Sonuc sonuc = new Sonuc();

            if (tip == "SIL")
            {
                if (string.IsNullOrWhiteSpace(form.projeKod))
                    sonuc.hataStr = "Silinecek kaydı seçiniz.";
            }
            else if (tip == "KAYDET" || tip == "GUNCELLE")
            {
                if (string.IsNullOrEmpty(form.projeKod))
                    sonuc.hataStr += "Öncelikle proje bilgisini kaydeniz!" + "<br>";
                if (string.IsNullOrEmpty(form.ad))
                    sonuc.hataStr += "Faaliyet adı boş bırakılamaz!" + "<br>";
                if (TNSDateTime.IsNull(form.basTarih))
                    sonuc.hataStr += "Başlangıç tarihi boş bırakılamaz!" + "<br>";
                else if (!TNSDateTime.IsNull(form.bitTarih) && form.bitTarih.Oku() <= form.basTarih.Oku())
                    sonuc.hataStr += "Bitiş tarihi, başlangıç tarihinden küçük veya eşit olamaz!" + "<br>";
            }

            if (!string.IsNullOrEmpty(sonuc.hataStr))
                sonuc.islemSonuc = false;
            else
                sonuc.islemSonuc = true;

            return sonuc;
        }

        internal static ObjectVAN ProjeFaaliyetGerceklesmeListele(VeriErisim veriErisim, Kullanici kullanan, int yil, int ay, string faaliyetId, bool birimOku)
        {
            ObjectVAN projeGerc = new ObjectVAN();

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));
            TABProjeFaaliyet to = new TABProjeFaaliyet(ve);
            IDataReader dr = to.GerceklesmeListele(yil, ay, faaliyetId);

            ProjeFaaliyetGerceklesme o = new ProjeFaaliyetGerceklesme();
            projeGerc.o = o;
            if (dr != null)
            {
                int sonAlan = (int)TABProjeFaaliyet.ENUMAlanlarGerc.KAYITTARIHI + 1;
                if (dr.Read())
                {
                    o.kod = VeriErisim.DegerAl(dr, (int)TABProjeFaaliyet.ENUMAlanlarGerc.FAALKOD, "");
                    o.yil = VeriErisim.DegerAl(dr, (int)TABProjeFaaliyet.ENUMAlanlarGerc.YIL, 0);
                    o.ay = VeriErisim.DegerAl(dr, (int)TABProjeFaaliyet.ENUMAlanlarGerc.AY, 0);
                    o.gerceklesenDeger = VeriErisim.DegerAl(dr, (int)TABProjeFaaliyet.ENUMAlanlarGerc.NAKDIGERCEKLESME, 0.0);
                    o.aciklama = VeriErisim.DegerAl(dr, (int)TABProjeFaaliyet.ENUMAlanlarGerc.ACIKLAMA, "");
                    o.kullaniciKod = VeriErisim.DegerAl(dr, (int)TABProjeFaaliyet.ENUMAlanlarGerc.KAYDEDENKULKOD, "");
                    o.kayitTarihi = VeriErisim.DegerAl(dr, (int)TABProjeFaaliyet.ENUMAlanlarGerc.KAYITTARIHI);

                    if (birimOku)
                    {
                        o.faaliyet = new ProjeFaaliyet();
                        o.faaliyet.izleyenKod = VeriErisim.DegerAl(dr, sonAlan++, "");
                        o.faaliyet.sorumluBirim.birimKod = VeriErisim.DegerAl(dr, sonAlan++, "");
                        o.faaliyet.sorumluBirim.muhasebeKod = VeriErisim.DegerAl(dr, sonAlan++, 0);
                        o.faaliyet.sorumluBirim.kurKod = VeriErisim.DegerAl(dr, sonAlan++, "");
                        o.faaliyet.tahminiMaliyet = VeriErisim.DegerAl(dr, sonAlan++, 0.0);
                    }
                }
                dr.Close();
            }
            else
            {
                projeGerc.sonuc.DegerAta("İşlem sırasında beklenmedik bir hata oluştu:" + ve.hata);
            }

            if (projeGerc.sonuc.islemSonuc)
            {
                TABProjeFaaliyetGosterge tg = new TABProjeFaaliyetGosterge(ve);
                dr = tg.GerceklesmeListele(yil, ay, faaliyetId);
                if (dr != null)
                {
                    int ekle = (int)TABProjeFaaliyetGosterge.ENUMAlanlar.PLANLANANDEGER + 1;
                    while (dr.Read())
                    {
                        ProjeFaaliyetGosterge g = new ProjeFaaliyetGosterge();

                        g.kod = VeriErisim.DegerAl(dr, (int)TABProjeFaaliyetGosterge.ENUMAlanlar.KOD, "");
                        g.faaliyetKod = VeriErisim.DegerAl(dr, (int)TABProjeFaaliyetGosterge.ENUMAlanlar.FAALIYETKOD, "");
                        g.yil = VeriErisim.DegerAl(dr, (int)TABProjeFaaliyetGosterge.ENUMAlanlar.YIL, 0);
                        g.gosterge = VeriErisim.DegerAl(dr, (int)TABProjeFaaliyetGosterge.ENUMAlanlar.GOSTERGE, "");
                        g.birim = VeriErisim.DegerAl(dr, (int)TABProjeFaaliyetGosterge.ENUMAlanlar.BIRIM, "");
                        g.planlananDeger = VeriErisim.DegerAl(dr, (int)TABProjeFaaliyetGosterge.ENUMAlanlar.PLANLANANDEGER, 0.0);

                        g.ay = VeriErisim.DegerAl(dr, ekle + (int)TABProjeFaaliyetGosterge.ENUMAlanlarGerc.AY, 0);
                        g.gerceklesenDeger = VeriErisim.DegerAl(dr, ekle + (int)TABProjeFaaliyetGosterge.ENUMAlanlarGerc.GERCEKLESME, 0.0);
                        o.gostergeler.Add(g);
                    }
                    dr.Close();
                }
                else
                {
                    projeGerc.sonuc.DegerAta("İşlem sırasında beklenmedik bir hata oluştu:" + ve.hata);
                }
            }
            ve.VeriErisimKapat();

            return projeGerc;
        }

        internal static Sonuc ProjeFaaliyetGerceklesmeKaydet(VeriErisim veriErisim, Kullanici kullanan, ProjeFaaliyetGerceklesme gerc)
        {
            ObjectVAN o = new ObjectVAN();
            if (gerc.yil < 2016)
                o.sonuc.DegerAta("Lütfen geçerli bir yıl giriniz!");

            if (gerc.ay < 1 || gerc.ay > 12)
                o.sonuc.DegerAta("Lütfen geçerli bir ay giriniz!");

            if (string.IsNullOrWhiteSpace(gerc.kod))
                o.sonuc.DegerAta("Lütfen geçerli bir faaliyet seçiniz!");

            if (string.IsNullOrWhiteSpace(gerc.aciklama) || gerc.aciklama.Length < 50 || gerc.aciklama.Length > 2000)
                o.sonuc.DegerAta("Lütfen 50 ile 2000 karakter arası açıklama giriniz!!");

            if (!o.sonuc.islemSonuc)
                return o.sonuc;

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));
            o = ProjeFaaliyetGerceklesmeListele(ve, kullanan, gerc.yil, gerc.ay, gerc.kod, true);
            ProjeFaaliyetGerceklesme gercOkunan = null;

            if (o.sonuc.islemSonuc)
            {
                gercOkunan = (ProjeFaaliyetGerceklesme)o.o;
                if (kullanan.kullaniciKodu != "stratek" && gercOkunan.faaliyet.izleyenKod != kullanan.kullaniciKodu)
                {
                    o.sonuc.DegerAta("Gerçekleşme bilgilerini sadece izleyiciler girebilir!");
                }
            }

            if (o.sonuc.islemSonuc)
            {
                ve.BeginTransaction();
                TABProjeFaaliyet to = new TABProjeFaaliyet(ve);

                TABProjeFaaliyetGosterge tf = new TABProjeFaaliyetGosterge(ve);

                if (TNSDateTime.IsNull(gercOkunan.kayitTarihi))
                    o.sonuc = to.GerceklesmeEkle(gerc, kullanan.kullaniciKodu);
                else
                    o.sonuc = to.GerceklesmeGuncelle(gerc, kullanan.kullaniciKodu);

                for (int i = 0; o.sonuc.islemSonuc && i < gerc.gostergeler.Count; i++)
                {
                    ProjeFaaliyetGosterge gelenGost = (ProjeFaaliyetGosterge)gerc.gostergeler[i];
                    gelenGost.yil = gerc.yil;
                    gelenGost.ay = gerc.ay;
                    ProjeFaaliyetGosterge okunanGost = gercOkunan.GostergeBul(gelenGost);
                    if (okunanGost == null || okunanGost.ay == 0)
                        o.sonuc = tf.GerceklesmeEkle(gelenGost);
                    else
                        o.sonuc = tf.GerceklesmeGuncelle(gelenGost);
                }

                if (o.sonuc.islemSonuc)
                    ve.Commit();
                else
                    ve.Rollback();
            }

            ve.VeriErisimKapat();
            return o.sonuc;
        }
    }
}
