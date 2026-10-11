using System.Data;
using TNS.KYM;
using TNS.TRM.IsKurallari.Veri;
using TNS.Veri;
using System;
using System.Text.RegularExpressions;
using System.Reflection;
using System.Collections.Generic;

namespace TNS.TRM.IsKurallari
{
    public class TRMHibeDegerlendirmeKomisyonu
    {
        /// <summary>
        /// Arguman olarak verilen, başvuru yönetim kurulu bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="bilgi">Kaydedilecek bilgiyi tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc IlDegerlendirmeKomisyonuKaydet(Kullanici kullanan, IlDegerlendirmeKomisyonu[] komisyon)
        {
            Sonuc sonuc = new Sonuc();

            if (komisyon.Length == 0)
            {
                sonuc.DegerAta("Kaydedilecek yeni kayıt bulunamadı.");
                return sonuc;
            }

            if (!(kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.DESTEKBASVURUTASRA) || kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.SISTEMYONETICISI) ||
                kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.DESTEKBASVURUYONETICI)))
            {
                sonuc.DegerAta("Kaydetme yetkiniz yoktur.");
                return sonuc;
            }

            foreach (IlDegerlendirmeKomisyonu k in komisyon)
            {
                if (k == null)
                    sonuc.DegerAta("TC Kimlik numarası hatalı girilmiş");
                else if (string.IsNullOrWhiteSpace(k.tcNo) || string.IsNullOrWhiteSpace(k.eMail) || string.IsNullOrWhiteSpace(k.adSoyad))
                    sonuc.DegerAta("TC kimlik numarası, ad-soyad, email bilgileri zorunludur.");
                else if (!OrtakFonksiyonlar.MailAdresiDogrumu(k.eMail))
                    sonuc.DegerAta("e-posta adresi hatalı girilmiş");
                else if (!string.IsNullOrWhiteSpace(TNS.KPM.Arac.TCNoDogruMu(k.tcNo)))
                    sonuc.DegerAta("TC Kimlik numarası hatalı girilmiş");
            }
            if (!sonuc.islemSonuc) return sonuc;

            IlDegerlendirmeKomisyonu idk = ListeleIlDegerlendirmeKomisyonuAna(kullanan, komisyon[0]);
            bool komisyonNoAl = false;
            if (idk.il != "")
            {
                komisyon[0].kod = idk.kod;
                if (idk.kilit > 0)
                {
                    sonuc.islemSonuc = false;
                    sonuc.hataStr = "Liste merkez tarafından kilitlendiği için işlem yapılamaz.";
                    return sonuc;
                }
                sonuc.islemSonuc = true;
            }
            else
            {
                komisyonNoAl = true;
            }

            if (sonuc.islemSonuc && komisyon != null)
            {
                VeriErisim ve = VeriErisim.VeriErisimBul(null, TABGenel.BaglantiSatiri, TNS.KYM.Arac.KimlikBul(kullanan));
                TABHibeDegerlendirmeKomisyonu to = new TABHibeDegerlendirmeKomisyonu(ve);
                ve.BeginTransaction();
                if (komisyonNoAl)
                {
                    komisyon[0].kod = new TABGenel(ve).BelgeNoAlInt("TRMHIBEDEGERLENDIRMEKOMISYONU", "000000000");
                    sonuc = to.KaydetIlDegerlendirmeKomisyonuAna(komisyon[0]);
                }
                foreach (IlDegerlendirmeKomisyonu k in komisyon)
                {
                    if (!sonuc.islemSonuc) break;
                    k.kod = komisyon[0].kod;
                    sonuc = to.KaydetIlDegerlendirmeKomisyonu(k);
                }
                if (sonuc.islemSonuc)
                {
                    sonuc.anahtar = komisyon[0].kod.ToString();
                    ve.Commit();
                }
                else
                {
                    sonuc.hataStr = "Kayıt sırasında bir hata oluştu" + "<br>" + sonuc.hataStr;
                    ve.Rollback();
                }
                ve.VeriErisimKapat();
            }

            return sonuc;
        }

        public Sonuc SilIlDegerlendirmeKomisyonu(Kullanici kullanan, IlDegerlendirmeKomisyonu[] komisyon)
        {
            Sonuc sonuc = new Sonuc();

            if (komisyon == null || komisyon.Length == 0)
            {
                sonuc.DegerAta("Silinecek kayıtlı bilgi bulunamadı.");
                return sonuc;
            }

            if (!(kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.DESTEKBASVURUTASRA) || kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.SISTEMYONETICISI) ||
                kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.DESTEKBASVURUYONETICI)))
            {
                sonuc.DegerAta("Silme yetkiniz yoktur.");
                return sonuc;
            }

            IlDegerlendirmeKomisyonu idk = ListeleIlDegerlendirmeKomisyonuAna(kullanan, komisyon[0]);

            if (idk.il != "")
            {
                //if (komisyon[0].kod == 0)
                //{
                //    sonuc.islemSonuc = false;
                //    sonuc.hataStr = "Sistemde kaydı mevcut.";
                //    ve.Rollback();
                //    ve.VeriErisimKapat();
                //    return sonuc;
                //}
                komisyon[0].kod = idk.kod;
                if (idk.kilit > 0)
                {
                    sonuc.DegerAta("Liste merkez tarafından kilitlendiği için işlem yapılamaz.");
                    return sonuc;
                }
            }
            else
            {
                sonuc.DegerAta("Komisyon bulunamadı!");
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, TABGenel.BaglantiSatiri, TNS.KYM.Arac.KimlikBul(kullanan));
            TABHibeDegerlendirmeKomisyonu to = new TABHibeDegerlendirmeKomisyonu(ve);
            ve.BeginTransaction();

            foreach (IlDegerlendirmeKomisyonu k in komisyon)
                if (k != null && k.kod > 0 && k.tcNo.Length == 11)
                {
                    sonuc = to.SilIlDegerlendirmeKomisyonu(k);
                    if (!sonuc.islemSonuc)
                        break;
                }

            if (sonuc.islemSonuc)
            {
                sonuc.bilgiStr = "Silme işlemi başarıyla tamamladı";
                ve.Commit();
            }
            else
            {
                sonuc.hataStr = "Silme sırasında bir hata oluştu" + "<br>" + sonuc.hataStr;
                ve.Rollback();
            }

            ve.VeriErisimKapat();
            return sonuc;
        }

        public Sonuc KilitleIlDegerlendirmeKomisyonu(Kullanici kullanan, int kod, int durum)
        {
            Sonuc sonuc = new Sonuc(false, "", "");
            if (!(kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.SISTEMYONETICISI) || kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.DESTEKBASVURUYONETICI)))
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = "Kilitleme yetkiniz yoktur.";
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, TABGenel.BaglantiSatiri);
            TABHibeDegerlendirmeKomisyonu to = new TABHibeDegerlendirmeKomisyonu(ve);
            ve.BeginTransaction();

            sonuc = to.KilitleIlDegerlendirmeKomisyonu(kod, durum);

            if (sonuc.islemSonuc)
                ve.Commit();
            else
            {
                sonuc.hataStr = "Kilitleme sırasında bir hata oluştu" + "<br>" + sonuc.hataStr;
                ve.Rollback();
            }

            ve.VeriErisimKapat();

            return sonuc;
        }

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, yönetim kurulunu veritabanından okur 
        /// ve YonetimKurulu nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, yönetim kurulu listesini YonetimKurulu nesne koleksiyonu olarak tutan nesne</returns>
        public ObjectArray ListeleIlDegerlendirmeKomisyonu(Kullanici kullanan, IlDegerlendirmeKomisyonu komisyon)
        {
            ObjectArray bilgi = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, TABGenel.BaglantiSatiri);

            TABHibeDegerlendirmeKomisyonu to = new TABHibeDegerlendirmeKomisyonu(ve);
            IDataReader r = to.ListeleIlDegerlendirmeKomisyonu(komisyon);

            if (r != null)
            {
                while (r.Read())
                {
                    IlDegerlendirmeKomisyonu y = new IlDegerlendirmeKomisyonu();

                    y.kod = VeriErisim.DegerAl(r, (int)TABHibeDegerlendirmeKomisyonu.ENUMAlanlar.KOD, 0);
                    y.etapNo = VeriErisim.DegerAl(r, (int)TABHibeDegerlendirmeKomisyonu.ENUMAlanlar.ETAPNO, 0);
                    y.hibeTuru = VeriErisim.DegerAl(r, (int)TABHibeDegerlendirmeKomisyonu.ENUMAlanlar.HIBETUR, 0);
                    y.il = VeriErisim.DegerAl(r, (int)TABHibeDegerlendirmeKomisyonu.ENUMAlanlar.IL, string.Empty);
                    y.kilit = VeriErisim.DegerAl(r, (int)TABHibeDegerlendirmeKomisyonu.ENUMAlanlar.KILIT, 0);
                    y.konuTur = VeriErisim.DegerAl(r, (int)TABHibeDegerlendirmeKomisyonu.ENUMAlanlar.KONUTUR, 0);
                    y.altYapiGrup = VeriErisim.DegerAl(r, (int)TABHibeDegerlendirmeKomisyonu.ENUMAlanlar.ALTYAPIGRUP, "");

                    int sonKolon = (int)TABHibeDegerlendirmeKomisyonu.ENUMAlanlar.ALTYAPIGRUP + 1;

                    y.tcNo = VeriErisim.DegerAl(r, (int)TABHibeDegerlendirmeKomisyonu.ENUMAlanlarDetay.TCKIMLIKNO + sonKolon, string.Empty);
                    y.adSoyad = VeriErisim.DegerAl(r, (int)TABHibeDegerlendirmeKomisyonu.ENUMAlanlarDetay.ADSOYAD + sonKolon, string.Empty);
                    y.eMail = VeriErisim.DegerAl(r, (int)TABHibeDegerlendirmeKomisyonu.ENUMAlanlarDetay.EMAIL + sonKolon, string.Empty);
                    y.telefon = VeriErisim.DegerAl(r, (int)TABHibeDegerlendirmeKomisyonu.ENUMAlanlarDetay.TELEFON + sonKolon, string.Empty);
                    y.baskan = VeriErisim.DegerAl(r, (int)TABHibeDegerlendirmeKomisyonu.ENUMAlanlarDetay.BASKAN + sonKolon, 0);
                    y.degerlendirmeci = VeriErisim.DegerAl(r, (int)TABHibeDegerlendirmeKomisyonu.ENUMAlanlarDetay.DEGERLENDIRMECI + sonKolon, 0);
                    y.dogumTarih.Yaz(VeriErisim.DegerAl(r, (int)TABHibeDegerlendirmeKomisyonu.ENUMAlanlarDetay.DOGUMTARIH + sonKolon));

                    sonKolon = (int)TABHibeDegerlendirmeKomisyonu.ENUMAlanlarDetay.DOGUMTARIH + sonKolon + 1;
                    y.ilAd = VeriErisim.DegerAl(r, sonKolon, string.Empty);

                    if (komisyon.maskele)
                    {
                        y.tcNo = Arac.MaskeleTC(y.tcNo);
                        y.telefon = Arac.MaskeleTel(y.telefon);
                    }

                    bilgi.objeler.Add(y);
                }
                r.Close();
            }
            else
                bilgi.sonuc.DegerAta(ve.hata);

            ve.VeriErisimKapat();
            return bilgi;
        }

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, yönetim kurulunu veritabanından okur 
        /// ve YonetimKurulu nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, yönetim kurulu listesini YonetimKurulu nesne koleksiyonu olarak tutan nesne</returns>
        public IlDegerlendirmeKomisyonu ListeleIlDegerlendirmeKomisyonuAna(Kullanici kullanan, IlDegerlendirmeKomisyonu komisyon)
        {
            IlDegerlendirmeKomisyonu y = new IlDegerlendirmeKomisyonu();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, TABGenel.BaglantiSatiri, TNS.KYM.Arac.KimlikBul(kullanan));

            TABHibeDegerlendirmeKomisyonu to = new TABHibeDegerlendirmeKomisyonu(ve);
            IDataReader r = to.ListeleIlDegerlendirmeKomisyonuAna(komisyon);

            if (to.Hata() == "")
            {
                if (r.Read())
                {
                    y.kod = VeriErisim.DegerAl(r, (int)TABHibeDegerlendirmeKomisyonu.ENUMAlanlar.KOD, 0);
                    y.etapNo = VeriErisim.DegerAl(r, (int)TABHibeDegerlendirmeKomisyonu.ENUMAlanlar.ETAPNO, 0);
                    y.hibeTuru = VeriErisim.DegerAl(r, (int)TABHibeDegerlendirmeKomisyonu.ENUMAlanlar.HIBETUR, 0);
                    y.il = VeriErisim.DegerAl(r, (int)TABHibeDegerlendirmeKomisyonu.ENUMAlanlar.IL, string.Empty);
                    y.kilit = VeriErisim.DegerAl(r, (int)TABHibeDegerlendirmeKomisyonu.ENUMAlanlar.KILIT, 0);
                    y.konuTur = VeriErisim.DegerAl(r, (int)TABHibeDegerlendirmeKomisyonu.ENUMAlanlar.KONUTUR, 0);
                    y.altYapiGrup = VeriErisim.DegerAl(r, (int)TABHibeDegerlendirmeKomisyonu.ENUMAlanlar.ALTYAPIGRUP, "");
                }
            }

            r.Close();

            ve.VeriErisimKapat();

            return y;
        }
    }
}
