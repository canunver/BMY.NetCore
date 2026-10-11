using System;
using System.Data;
using TNS;
using TNS.Veri;
using TNS.BKD;
using TNS.BKD.IsKurallari.Veri;
using TNS.KYM;
using System.Collections;
using TNS.UZY;

namespace TNS.BKD.IsKurallari
{
    /// <summary>
    /// Bütçe tertiplerinde, faaliyet ve yatırım projeleriyle ilişkiyi kuran
    /// Alt Faaliyet boyutu ile ilgili iş kuralları yordamlarını tutan sınıf.
    /// </summary>
    public class BHMv2APrgKodYapi
    {
        /// <summary>
        /// PRGKODYAPI nesnesini veri tabanına yazan fonksiyon
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler</param>
        /// <param name="bilgi">PRGKODYAPI tanım bilgilerini tutan parametre</param>
        /// <returns>Kaydetme işleminin sonucu, Sonuc yapısında döndürülür.</returns>
        public static Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, PrgKodYapi bilgi)
        {
            Sonuc sonuc = Kontrol("KAYDET", kullanan, bilgi);
            if (!sonuc.islemSonuc)
                return sonuc;

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriBHM", kullanan.KimlikBul());
            TABPRGKODYAPI tab = new TABPRGKODYAPI(ve);
            ObjectVAN o = Oku(ve, kullanan, new PrgKodYapi() { yil = bilgi.yil, kodTur = bilgi.kodTur, kod = bilgi.kod });
            if (!o.sonuc.islemSonuc)
            {
                ve.VeriErisimKapat();
                return o.sonuc;
            }

            PrgKodYapi eski = (PrgKodYapi)o.o;
            ve.BeginTransaction();

            if (eski == null)
                sonuc = tab.Ekle(bilgi);
            else
            {
                if (bilgi.fonKod == "") bilgi.fonKod = eski.fonKod;
                sonuc = tab.Guncelle(bilgi);
            }
            if (sonuc.islemSonuc)
                ve.Commit();
            else
                ve.Rollback();

            ve.VeriErisimKapat();
            return sonuc;
        }

        private static ObjectVAN Oku(VeriErisim ve, Kullanici kullanan, PrgKodYapi prgKodYapi)
        {
            ObjectVAN o = new ObjectVAN();
            ObjectArray oa = Listele(ve, kullanan, prgKodYapi);
            o.sonuc = oa.sonuc;
            if (oa.ObjeSayisi == 1)
                o.o = oa.objeler[0];
            else
            {
                o.o = null;
                if (oa.ObjeSayisi > 1)
                    o.sonuc.DegerAta("Birden fazla kayıt geldi!");
            }
            return o;
        }

        public static Sonuc Aktar(VeriErisim veriErisim, Kullanici kullanan, PrgKodYapi bilgi)
        {
            Sonuc sonuc = new Sonuc(true, string.Empty, string.Empty);

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriBHM", kullanan.KimlikBul());

            ObjectArray bilgiler = Listele(ve, kullanan, bilgi);

            TABPRGKODYAPI tab = new TABPRGKODYAPI(ve);

            ve.BeginTransaction();

            foreach (PrgKodYapi item in bilgiler.objeler)
            {
                item.yil++;
                sonuc = tab.Ekle(item);

                if (!sonuc.islemSonuc)
                    break;
            }

            if (sonuc.islemSonuc)
            {
                ve.Commit();
                sonuc.bilgiStr = "Başarıyla aktarıldı";
            }
            else
                ve.Rollback();

            ve.VeriErisimKapat();
            return sonuc;
        }

        public static ObjectArray Listele(VeriErisim veriErisim, Kullanici kullanan, PrgKodYapi bilgi)
        {
            ObjectArray liste = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriBHM", kullanan.KimlikBul());
            TABPRGKODYAPI tab = new TABPRGKODYAPI(ve);

            IDataReader dr = tab.Listele(bilgi);
            if (dr == null)
            {
                ve.VeriErisimKapat();
                throw new Exception(ve.hata);
            }
            while (dr.Read())
            {
                PrgKodYapi t = new PrgKodYapi();
                t.yil = VeriErisim.DegerAl(dr, (int)TABPRGKODYAPI.ENUMAlanlar.YIL, 0);
                t.kodTur = VeriErisim.DegerAl(dr, (int)TABPRGKODYAPI.ENUMAlanlar.KODTUR, 0);
                t.kod = VeriErisim.DegerAl(dr, (int)TABPRGKODYAPI.ENUMAlanlar.KOD, 0);
                t.ad = VeriErisim.DegerAl(dr, (int)TABPRGKODYAPI.ENUMAlanlar.AD, string.Empty);
                t.tur = VeriErisim.DegerAl(dr, (int)TABPRGKODYAPI.ENUMAlanlar.TUR, 0);
                t.fonKod = VeriErisim.DegerAl(dr, (int)TABPRGKODYAPI.ENUMAlanlar.FONKOD, "");

                liste.Ekle(t);
            }
            dr.Close();

            ve.VeriErisimKapat();
            return liste;
        }

        public static Sonuc Sil(VeriErisim veriErisim, Kullanici kullanan, PrgKodYapi bilgi)
        {
            Sonuc sonuc = new Sonuc(true, string.Empty, string.Empty);

            sonuc = Kontrol("SIL", kullanan, bilgi);
            if (!sonuc.islemSonuc)
                return sonuc;

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriBHM", kullanan.KimlikBul());
            TABPRGKODYAPI tab = new TABPRGKODYAPI(ve);

            ve.BeginTransaction();

            if (sonuc.islemSonuc)
                sonuc = tab.Sil(bilgi);

            if (sonuc.islemSonuc)
            {
                ve.Commit();
                sonuc.bilgiStr = "Başarıyla silindi";
            }
            else
                ve.Rollback();

            ve.VeriErisimKapat();
            return sonuc;
        }

        private static Sonuc Kontrol(string tip, Kullanici kullanan, PrgKodYapi bilgi)
        {
            Sonuc sonuc = new Sonuc();

            if (!kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.SISTEMYONETICISI))
                sonuc.hataStr += "<li>İşlemi yapmaya yetkiniz bulunmamaktadır.</li>";

            if (bilgi.kod == 0)
                sonuc.hataStr += "<li>Kod bilgisi boş bırakılamaz.</li>";
            if (bilgi.kodTur == 0)
                sonuc.hataStr += "<li>Program Tür bilgisi boş bırakılamaz.</li>";
            if (bilgi.yil == 0)
                sonuc.hataStr += "<li>Yıl bilgisi boş bırakılamaz.</li>";

            if (tip == "SIL")
            {
            }
            else if (tip == "KAYDET" || tip == "GUNCELLE")
            {
                if (bilgi.fonKod != "XX")
                {
                    if (bilgi.fonKod == null) bilgi.fonKod = "";
                    else bilgi.fonKod = bilgi.fonKod.Trim().Replace(".", "");

                    if (bilgi.kodTur == 3)
                    {
                        if (bilgi.fonKod.Length != 4)
                            sonuc.hataStr += "<li>Fonksiyonel kod noktalar hariç 4 basamak olmalıdır!</li>";
                    }
                    else
                    {
                        if (bilgi.fonKod.Length != 0)
                            sonuc.hataStr += "<li>Fonksiyonel kod dolu olmamalıdır!</li>";
                    }
                }
                else
                    bilgi.fonKod = "";
            }

            sonuc.islemSonuc = string.IsNullOrEmpty(sonuc.hataStr);
            return sonuc;
        }

        private static Sonuc KontrolIliski(string tip, Kullanici kullanan, PrgPebIliski bilgi)
        {
            Sonuc sonuc = new Sonuc();

            if (!kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.SISTEMYONETICISI))
                sonuc.DegerAta("<li>İşlemi yapmaya yetkiniz bulunmamaktadır.</li>");

            if (tip == "KOPYALA")
            {
            }
            else
            {
                if (string.IsNullOrEmpty(bilgi.pebKod))
                    sonuc.DegerAta("<li>PEB kodu alanı boş bırakılamaz.</li>");
                if (bilgi.yil <= 0)
                    sonuc.DegerAta("<li>Yıl bilgisi boş bırakılamaz.</li>");

                if (tip == "SIL")
                {
                }
                else if (tip == "KAYDET" || tip == "GUNCELLE")
                {
                    if (bilgi.prgKod <= 0)
                        sonuc.DegerAta("<li>Program değeri boş bırakılamaz.</li>");
                    if (bilgi.altPrgKod <= 0)
                        sonuc.DegerAta("<li>Alt Program değeri boş bırakılamaz.</li>");
                    if (bilgi.faalKod <= 0)
                        sonuc.DegerAta("<li>Faaliyet değeri boş bırakılamaz.</li>");
                    if (bilgi.altFaalKod <= 0)
                        sonuc.DegerAta("<li>Alt Faaliyet değeri boş bırakılamaz.</li>");
                    if (string.IsNullOrEmpty(bilgi.fonKod))
                        sonuc.DegerAta("<li>Fonksiyonel kod alanı boş bırakılamaz.</li>");
                    if (sonuc.islemSonuc)
                    {
                        string faalFinKod = bilgi.pebKod.Replace(".", "").Substring(2, 2);

                        if (faalFinKod == "11" || faalFinKod == "13" || faalFinKod == "21" || faalFinKod == "23")
                        {
                            if (!bilgi.fonKod.Replace(".", "").StartsWith("042134"))
                            {
                                sonuc.DegerAta($"Yikob faaliyetlerinin fonksiyonel kodu({"04.2.1.34"}) olmalıdır!");
                            }
                        }
                        else
                        {
                            ObjectVAN ov = Oku(null, kullanan, new PrgKodYapi() { yil = bilgi.yil, kodTur = 3, kod = bilgi.faalKod });
                            if (!ov.sonuc.islemSonuc)
                                sonuc.DegerAta(ov.sonuc.hataStr);
                            else if (ov.o == null)
                                sonuc.DegerAta($"{bilgi.faalKod} kodlu faaliyet okunamadı!");
                            else
                            {
                                PrgKodYapi pky = (PrgKodYapi)ov.o;
                                if (!string.IsNullOrWhiteSpace(pky.fonKod) && !bilgi.fonKod.Replace(".", "").StartsWith(pky.fonKod))
                                {
                                    sonuc.DegerAta($"Girilen fonksiyonel kod ({bilgi.fonKod}) ile program faaliyetinin fonksiyonel kodu ({pky.fonKod}) uyumlu değil!");
                                }
                            }
                        }
                    }
                    if (sonuc.islemSonuc && !string.IsNullOrWhiteSpace(bilgi.fonKod))
                    {
                        string fonAd = TNS.UZY.Arac.Tanimla().UzayDegeriStr(kullanan, "ABSFON", bilgi.yil + "-" + bilgi.fonKod, true);
                        if (string.IsNullOrWhiteSpace(fonAd))
                            sonuc.DegerAta($"{bilgi.fonKod} fonksiyonel kodunun tanımı bulunamadı!");
                    }
                }
            }
            sonuc.islemSonuc = string.IsNullOrEmpty(sonuc.hataStr);
            return sonuc;
        }

        public static Sonuc KaydetPEBIliski(VeriErisim veriErisim, Kullanici kullanan, PrgPebIliski bilgi)
        {
            Sonuc sonuc = KontrolIliski("KAYDET", kullanan, bilgi);
            if (!sonuc.islemSonuc)
                return sonuc;

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriBHM", kullanan.KimlikBul());
            TABPRGKODYAPI tab = new TABPRGKODYAPI(ve);

            ObjectVAN o = OkuPEBIliski(veriErisim, kullanan, new PrgPebIliski() { yil = bilgi.yil, pebKod = bilgi.pebKod });

            if (!o.sonuc.islemSonuc)
            {
                ve.VeriErisimKapat();
                return o.sonuc;
            }

            PrgPebIliski eski = (PrgPebIliski)o.o;

            ve.BeginTransaction();

            if (eski == null || eski.altFaalKod == 0)
                sonuc = tab.EklePEBIliski(bilgi);
            else
                sonuc = tab.GuncellePEBIliski(bilgi);

            if (sonuc.islemSonuc)
                ve.Commit();
            else
                ve.Rollback();

            ve.VeriErisimKapat();
            return sonuc;
        }

        public static ObjectVAN OkuPEBIliski(VeriErisim ve, Kullanici kullanan, PrgPebIliski prgPebIliski)
        {
            ObjectVAN o = new ObjectVAN();
            ObjectArray oa = ListelePEBIliski(ve, kullanan, prgPebIliski);
            o.sonuc = oa.sonuc;
            if (oa.ObjeSayisi == 1)
                o.o = oa.objeler[0];
            else
            {
                o.o = null;
                if (oa.ObjeSayisi > 1)
                    o.sonuc.DegerAta("Birden fazla kayıt geldi!");
            }
            return o;
        }

        public static ObjectArray ListelePEBIliski(VeriErisim veriErisim, Kullanici kullanan, PrgPebIliski bilgi)
        {
            ObjectArray liste = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriBHM", kullanan.KimlikBul());
            TABPRGKODYAPI tab = new TABPRGKODYAPI(ve);

            IDataReader dr = tab.ListelePEBIliski(bilgi);
            if (dr == null)
            {
                ve.VeriErisimKapat();
                throw new Exception(ve.hata);
            }
            Tertip tUz = new Tertip(bilgi.yil, ENUMTabloKod.TABPEBOdenek);
            string pebYapi = tUz.Kirilimlar(4);
            string fonYapi = tUz.Kirilimlar(1);
            while (dr.Read())
            {
                PrgPebIliski t = new PrgPebIliski();
                t.yil = bilgi.yil;
                int kol = 0;

                //Cumhur güncellemeden sonra faaliyet kodlarını getirmedi. Burası kapatıldı Melih 07.04.2022
                t.pebKod = OrtakFonksiyonlar.KodNoktaKoy(pebYapi, VeriErisim.DegerAl(dr, kol++, ""));
                //t.pebKod = VeriErisim.DegerAl(dr, kol++, "");
                t.pebAd = VeriErisim.DegerAl(dr, kol++, "");

                t.altFaalKod = VeriErisim.DegerAl(dr, kol++, 0);
                t.altFaalAdi = VeriErisim.DegerAl(dr, kol++, "");

                t.prgKod = VeriErisim.DegerAl(dr, kol++, 0);
                t.prgAdi = VeriErisim.DegerAl(dr, kol++, "");

                t.altPrgKod = VeriErisim.DegerAl(dr, kol++, 0);
                t.altPrgAdi = VeriErisim.DegerAl(dr, kol++, "");

                t.faalKod = VeriErisim.DegerAl(dr, kol++, 0);
                t.faalAdi = VeriErisim.DegerAl(dr, kol++, "");

                t.fonKod = OrtakFonksiyonlar.KodNoktaKoy(fonYapi, VeriErisim.DegerAl(dr, kol++, ""));
                t.faalFonKod = OrtakFonksiyonlar.KodNoktaKoy(fonYapi, VeriErisim.DegerAl(dr, kol++, ""));
                bool ekle;
                if (bilgi.fonUyumsuz && !string.IsNullOrEmpty(t.faalFonKod))
                {
                    ekle = (string.IsNullOrEmpty(t.fonKod) || !t.fonKod.StartsWith(t.faalFonKod));
                }
                else
                    ekle = true;
                if (ekle)
                    liste.Ekle(t);
            }
            dr.Close();

            if (string.IsNullOrEmpty(bilgi.pebKod))
            {
                dr = tab.ListeleIliskisiOlmayanAltFaal(bilgi);
                if (dr == null)
                {
                    ve.VeriErisimKapat();
                    throw new Exception(ve.hata);
                }

                while (dr.Read())
                {
                    PrgPebIliski t = new PrgPebIliski();
                    t.yil = bilgi.yil;
                    int kol = 0;
                    t.altFaalKod = VeriErisim.DegerAl(dr, kol++, 0);
                    t.altFaalAdi = VeriErisim.DegerAl(dr, kol++, "");

                    liste.Ekle(t);
                }
                dr.Close();
            }
            ve.VeriErisimKapat();
            return liste;
        }

        public static Sonuc SilPEBIliski(VeriErisim veriErisim, Kullanici kullanan, PrgPebIliski bilgi)
        {
            Sonuc sonuc = KontrolIliski("SİL", kullanan, bilgi);
            if (!sonuc.islemSonuc)
                return sonuc;

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriBHM", kullanan.KimlikBul());
            TABPRGKODYAPI tab = new TABPRGKODYAPI(ve);

            ObjectVAN o = OkuPEBIliski(veriErisim, kullanan, new PrgPebIliski() { yil = bilgi.yil, pebKod = bilgi.pebKod });

            if (o.o == null)
            {
                sonuc.DegerAta("Silinecek kayıt bulunamadı!");
            }

            if (!o.sonuc.islemSonuc)
            {
                ve.VeriErisimKapat();
                return o.sonuc;
            }

            ve.BeginTransaction();

            if (sonuc.islemSonuc)
                sonuc = tab.SilPEBIliski(bilgi);

            if (sonuc.islemSonuc)
            {
                ve.Commit();
                sonuc.bilgiStr = "Başarıyla silindi";
            }
            else
                ve.Rollback();

            ve.VeriErisimKapat();
            return sonuc;
        }

        internal static Sonuc IliskiKopyala(VeriErisim veriErisim, Kullanici kullanan, int yil)
        {
            Sonuc sonuc = KontrolIliski("KOPYALA", kullanan, null);
            if (!sonuc.islemSonuc)
                return sonuc;

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriBHM", kullanan.KimlikBul());
            TABPRGKODYAPI tab = new TABPRGKODYAPI(ve);

            ve.BeginTransaction();

            if (sonuc.islemSonuc)
                sonuc = tab.KopyalaProg(yil);

            if (sonuc.islemSonuc)
                sonuc = tab.KopyalaPEB(yil);

            if (sonuc.islemSonuc)
                sonuc = tab.KopyalaIliski(yil);

            if (sonuc.islemSonuc)
            {
                ve.Commit();
                sonuc.bilgiStr = "Başarıyla kopyalandı";
            }
            else
                ve.Rollback();

            ve.VeriErisimKapat();
            return sonuc;
        }
    }
}