using System.Data;
using TNS.KYM;
using TNS.TRM.IsKurallari.Veri;
using TNS.Veri;
using System;

namespace TNS.TRM.IsKurallari
{
    public class TRMHibeYatirimBasvuruTablolari
    {
        /// <summary>
        /// Arguman olarak verilen, başvuru yönetim kurulu bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="bilgi">Kaydedilecek bilgiyi tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc YonetimKuruluKaydet(Kullanici kullanan, YonetimKurulu[] yonetimKurulu)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(null, TABGenel.BaglantiSatiri);
            ObjectArray list = new TRMHibeYatirim().Listele(ve, kullanan, new HibeYatirim() { kod = yonetimKurulu[0].hibeKod });
            string basvuruNo = "";
            if (list.ObjeSayisi > 0)
                basvuruNo = ((HibeYatirim)list.objeler[0]).basvuruNo;

            string hata = IsKurallari.TRMHibeYatirimYonetim.HibeYatirimOnlineKapandiMi(ve, kullanan, basvuruNo);

            Sonuc sonuc = new Sonuc(false, "", "");
            if (!string.IsNullOrEmpty(hata))
            {
                sonuc.hataStr = "Yazma yetkiniz bulunmamaktadır.";
                sonuc.islemSonuc = false;
                ve.VeriErisimKapat();
                return sonuc;
            }

            TABHibeYatirimBasvuruTablolari to = new TABHibeYatirimBasvuruTablolari(ve);
            ve.BeginTransaction();

            sonuc = to.SilYonetimKurulu(yonetimKurulu[0].hibeKod);
            if (sonuc.islemSonuc)
            {
                if (yonetimKurulu != null)
                {
                    foreach (YonetimKurulu yk in yonetimKurulu)
                        if (yk != null)
                        {
                            if (sonuc.islemSonuc)
                                sonuc = to.KaydetYonetimKurulu(yk);
                        }
                }
            }


            if (sonuc.islemSonuc)
            {
                TABHibeYatirim tot = new TABHibeYatirim(ve);

                HibeYatirimTarihce tarihce = new HibeYatirimTarihce();
                tarihce.kullaniciKod = kullanan.mernis == "" ? kullanan.kullaniciKodu : kullanan.mernis;
                tarihce.kullaniciAdi = kullanan.ADSOYAD;
                tarihce.hibeKod = yonetimKurulu[0].hibeKod;
                tarihce.tarih = new TNSDateTime(DateTime.Now);
                tarihce.durum = (int)ENUMHibeYatirimBasvuruDurum.ONLINE;
                tarihce.ipAdresi = kullanan.telefon;
                tarihce.aciklama = "Yonetim Kurulu kaydetme. ";
                sonuc = tot.KaydetTarihce(tarihce);

                ve.Commit();
            }
            else
            {
                sonuc.hataStr = "Kayıt sırasında bir hata oluştu" + "<br>" + sonuc.hataStr;
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
        public ObjectArray YonetimKuruluListele(Kullanici kullanan, int hibeKod)
        {
            ObjectArray bilgi = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, TABGenel.BaglantiSatiri);

            TABHibeYatirimBasvuruTablolari to = new TABHibeYatirimBasvuruTablolari(ve);
            IDataReader r = to.YonetimKuruluListele(hibeKod);

            if (to.Hata() == "")
            {
                while (r.Read())
                {
                    YonetimKurulu y = new YonetimKurulu();

                    y.hibeKod = VeriErisim.DegerAl(r, (int)TABHibeYatirimBasvuruTablolari.ENUMAlanlarYonetimKurulu.HIBEKOD, 0);
                    y.satirNo = VeriErisim.DegerAl(r, (int)TABHibeYatirimBasvuruTablolari.ENUMAlanlarYonetimKurulu.SATIRNO, 0);
                    y.adSoyad = VeriErisim.DegerAl(r, (int)TABHibeYatirimBasvuruTablolari.ENUMAlanlarYonetimKurulu.ADSOYAD, string.Empty);
                    y.meslek = VeriErisim.DegerAl(r, (int)TABHibeYatirimBasvuruTablolari.ENUMAlanlarYonetimKurulu.MESLEK, string.Empty);
                    y.cinsiyet = VeriErisim.DegerAl(r, (int)TABHibeYatirimBasvuruTablolari.ENUMAlanlarYonetimKurulu.CINSIYET, 0);
                    y.pozisyon = VeriErisim.DegerAl(r, (int)TABHibeYatirimBasvuruTablolari.ENUMAlanlarYonetimKurulu.POZISYON, string.Empty);
                    y.yil = VeriErisim.DegerAl(r, (int)TABHibeYatirimBasvuruTablolari.ENUMAlanlarYonetimKurulu.YIL, 0);

                    bilgi.objeler.Add(y);
                }
            }

            r.Close();

            ve.VeriErisimKapat();

            return bilgi;
        }

        public Sonuc OrtaklarKaydet(Kullanici kullanan, Ortaklar[] ortaklar)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(null, TABGenel.BaglantiSatiri);
            ObjectArray list = new TRMHibeYatirim().Listele(ve, kullanan, new HibeYatirim() { kod = ortaklar[0].hibeKod });
            string basvuruNo = "";
            if (list.ObjeSayisi > 0)
                basvuruNo = ((HibeYatirim)list.objeler[0]).basvuruNo;

            string hata = IsKurallari.TRMHibeYatirimYonetim.HibeYatirimOnlineKapandiMi(ve, kullanan, basvuruNo);

            Sonuc sonuc = new Sonuc(false, "", "");
            if (!string.IsNullOrEmpty(hata))
            {
                sonuc.hataStr = "Yazma yetkiniz bulunmamaktadır.";
                sonuc.islemSonuc = false;
                ve.VeriErisimKapat();
                return sonuc;
            }

            TABHibeYatirimBasvuruTablolari to = new TABHibeYatirimBasvuruTablolari(ve);
            ve.BeginTransaction();

            sonuc = to.SilOrtaklar(ortaklar[0].hibeKod);
            if (sonuc.islemSonuc)
            {
                if (ortaklar != null)
                {
                    foreach (Ortaklar yk in ortaklar)
                        if (yk != null)
                        {
                            if (string.IsNullOrWhiteSpace(yk.adSoyad)) continue;

                            if (sonuc.islemSonuc)
                                sonuc = to.KaydetOrtaklar(yk);
                        }
                }
            }

            if (sonuc.islemSonuc)
            {
                TABHibeYatirim tot = new TABHibeYatirim(ve);

                HibeYatirimTarihce tarihce = new HibeYatirimTarihce();
                tarihce.kullaniciKod = kullanan.mernis == "" ? kullanan.kullaniciKodu : kullanan.mernis;
                tarihce.kullaniciAdi = kullanan.ADSOYAD;
                tarihce.hibeKod = ortaklar[0].hibeKod;
                tarihce.tarih = new TNSDateTime(DateTime.Now);
                tarihce.durum = (int)ENUMHibeYatirimBasvuruDurum.ONLINE;
                tarihce.ipAdresi = kullanan.telefon;
                tarihce.aciklama = "Şirket Ortakları kaydetme. ";
                sonuc = tot.KaydetTarihce(tarihce);

                ve.Commit();
            }
            else
            {
                sonuc.hataStr = "Kayıt sırasında bir hata oluştu" + "<br>" + sonuc.hataStr;
                ve.Rollback();
            }

            ve.VeriErisimKapat();

            return sonuc;
        }
        public ObjectArray OrtaklarListele(Kullanici kullanan, int hibeKod)
        {
            ObjectArray bilgi = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, TABGenel.BaglantiSatiri);

            TABHibeYatirimBasvuruTablolari to = new TABHibeYatirimBasvuruTablolari(ve);
            IDataReader r = to.OrtaklarListele(hibeKod);

            if (to.Hata() == "")
            {
                while (r.Read())
                {
                    Ortaklar y = new Ortaklar();

                    y.hibeKod = VeriErisim.DegerAl(r, (int)TABHibeYatirimBasvuruTablolari.ENUMAlanlarOrtaklar.HIBEKOD, 0);
                    y.satirNo = VeriErisim.DegerAl(r, (int)TABHibeYatirimBasvuruTablolari.ENUMAlanlarOrtaklar.SATIRNO, 0);
                    y.adSoyad = VeriErisim.DegerAl(r, (int)TABHibeYatirimBasvuruTablolari.ENUMAlanlarOrtaklar.ADSOYAD, string.Empty);
                    y.kimlikNo = VeriErisim.DegerAl(r, (int)TABHibeYatirimBasvuruTablolari.ENUMAlanlarOrtaklar.KIMLIKNO, string.Empty);
                    y.yuzde = VeriErisim.DegerAl(r, (int)TABHibeYatirimBasvuruTablolari.ENUMAlanlarOrtaklar.YUZDE, 0.0);

                    bilgi.objeler.Add(y);
                }
            }

            r.Close();

            ve.VeriErisimKapat();

            return bilgi;
        }

        public ObjectArray OrtaklarListeleKontrol(Kullanici kullanan, Ortaklar ortaklar)
        {
            ObjectArray bilgi = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, TABGenel.BaglantiSatiri);

            TABHibeYatirimBasvuruTablolari to = new TABHibeYatirimBasvuruTablolari(ve);
            IDataReader r = to.OrtaklarListele(ortaklar);

            if (to.Hata() == "")
            {
                while (r.Read())
                {
                    Ortaklar y = new Ortaklar();

                    y.hibeKod = VeriErisim.DegerAl(r, (int)TABHibeYatirimBasvuruTablolari.ENUMAlanlarOrtaklar.HIBEKOD, 0);
                    y.satirNo = VeriErisim.DegerAl(r, (int)TABHibeYatirimBasvuruTablolari.ENUMAlanlarOrtaklar.SATIRNO, 0);
                    y.adSoyad = VeriErisim.DegerAl(r, (int)TABHibeYatirimBasvuruTablolari.ENUMAlanlarOrtaklar.ADSOYAD, string.Empty);
                    y.kimlikNo = VeriErisim.DegerAl(r, (int)TABHibeYatirimBasvuruTablolari.ENUMAlanlarOrtaklar.KIMLIKNO, string.Empty);
                    y.yuzde = VeriErisim.DegerAl(r, (int)TABHibeYatirimBasvuruTablolari.ENUMAlanlarOrtaklar.YUZDE, 0.0);

                    bilgi.objeler.Add(y);
                }
            }

            r.Close();

            ve.VeriErisimKapat();

            return bilgi;
        }

        public ObjectArray OrtaklarListeleHibeKontrol(Kullanici kullanan, HibeYatirim hibe)
        {
            ObjectArray bilgi = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, TABGenel.BaglantiSatiri);

            TABHibeYatirimBasvuruTablolari to = new TABHibeYatirimBasvuruTablolari(ve);
            IDataReader r = to.OrtaklarHibeListele(hibe);

            if (to.Hata() == "")
            {
                while (r.Read())
                {
                    HibeYatirim y = new HibeYatirim();

                    y.kod = VeriErisim.DegerAl(r, 0, 0);

                    bilgi.objeler.Add(y);
                }
            }

            r.Close();

            ve.VeriErisimKapat();

            return bilgi;
        }

        /// <summary>
        /// Arguman olarak verilen, başvuru yönetim kurulu bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="bilgi">Kaydedilecek bilgiyi tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc FonKaynakKaydet(Kullanici kullanan, YonetimFonKaynak[] yonetimFonKaynak)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(null, TABGenel.BaglantiSatiri);
            ObjectArray list = new TRMHibeYatirim().Listele(ve, kullanan, new HibeYatirim() { kod = yonetimFonKaynak[0].hibeKod });
            string basvuruNo = "";
            if (list.ObjeSayisi > 0)
                basvuruNo = ((HibeYatirim)list.objeler[0]).basvuruNo;

            string hata = IsKurallari.TRMHibeYatirimYonetim.HibeYatirimOnlineKapandiMi(ve, kullanan, basvuruNo);

            Sonuc sonuc = new Sonuc(false, "", "");
            if (!string.IsNullOrEmpty(hata))
            {
                sonuc.hataStr = "Yazma yetkiniz bulunmamaktadır.";
                sonuc.islemSonuc = false;
                ve.VeriErisimKapat();
                return sonuc;
            }

            TABHibeYatirimBasvuruTablolari to = new TABHibeYatirimBasvuruTablolari(ve);
            ve.BeginTransaction();

            sonuc = to.SilFonKaynak(yonetimFonKaynak[0].hibeKod, yonetimFonKaynak[0].fonTip);
            if (sonuc.islemSonuc)
            {
                if (yonetimFonKaynak != null)
                {
                    foreach (YonetimFonKaynak yk in yonetimFonKaynak)
                        if (yk != null)
                        {
                            if (sonuc.islemSonuc)
                                sonuc = to.KaydetFonKaynak(yk);
                        }
                }
            }


            if (sonuc.islemSonuc)
            {
                TABHibeYatirim tot = new TABHibeYatirim(ve);
                HibeYatirimTarihce tarihce = new HibeYatirimTarihce();
                tarihce.kullaniciKod = kullanan.mernis == "" ? kullanan.kullaniciKodu : kullanan.mernis;
                tarihce.kullaniciAdi = kullanan.ADSOYAD;
                tarihce.hibeKod = yonetimFonKaynak[0].hibeKod;
                tarihce.tarih = new TNSDateTime(DateTime.Now);
                tarihce.durum = (int)ENUMHibeYatirimBasvuruDurum.ONLINE;
                tarihce.ipAdresi = kullanan.telefon;
                tarihce.aciklama = "Fon kaynağı kaydetme. ";
                sonuc = tot.KaydetTarihce(tarihce);

                ve.Commit();
            }
            else
            {
                sonuc.hataStr = "Kayıt sırasında bir hata oluştu" + "<br>" + sonuc.hataStr;
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
        public ObjectArray FonKaynakListele(Kullanici kullanan, int hibeKod, int fonTip)
        {
            ObjectArray bilgi = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, TABGenel.BaglantiSatiri);

            TABHibeYatirimBasvuruTablolari to = new TABHibeYatirimBasvuruTablolari(ve);
            IDataReader r = to.FonKaynakListele(hibeKod, fonTip);

            if (to.Hata() == "")
            {
                while (r.Read())
                {
                    YonetimFonKaynak y = new YonetimFonKaynak();

                    y.hibeKod = VeriErisim.DegerAl(r, (int)TABHibeYatirimBasvuruTablolari.ENUMAlanlarFonKaynak.HIBEKOD, 0);
                    y.satirNo = VeriErisim.DegerAl(r, (int)TABHibeYatirimBasvuruTablolari.ENUMAlanlarFonKaynak.SATIRNO, 0);
                    y.fonTip = VeriErisim.DegerAl(r, (int)TABHibeYatirimBasvuruTablolari.ENUMAlanlarFonKaynak.FONTIP, 0);
                    y.yil = VeriErisim.DegerAl(r, (int)TABHibeYatirimBasvuruTablolari.ENUMAlanlarFonKaynak.YIL, 0);
                    y.projeAdiReferansNo = VeriErisim.DegerAl(r, (int)TABHibeYatirimBasvuruTablolari.ENUMAlanlarFonKaynak.PROJEADIREFNO, string.Empty);
                    y.kurulusProgramAdi = VeriErisim.DegerAl(r, (int)TABHibeYatirimBasvuruTablolari.ENUMAlanlarFonKaynak.KURULUSPROGRAMADI, string.Empty);
                    y.tutar = VeriErisim.DegerAl(r, (int)TABHibeYatirimBasvuruTablolari.ENUMAlanlarFonKaynak.TUTAR, 0.0);
                    y.hibeKredi = VeriErisim.DegerAl(r, (int)TABHibeYatirimBasvuruTablolari.ENUMAlanlarFonKaynak.HIBEKREDI, 0.0);
                    y.projeAsamasi = VeriErisim.DegerAl(r, (int)TABHibeYatirimBasvuruTablolari.ENUMAlanlarFonKaynak.PROJEASAMASI, "");

                    bilgi.objeler.Add(y);
                }
            }

            r.Close();

            ve.VeriErisimKapat();

            return bilgi;
        }

        /// <summary>
        /// Arguman olarak verilen, başvuru Yatirim Giderler Tablosu bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="bilgi">Kaydedilecek bilgiyi tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc GiderleriKaydet(Kullanici kullanan, YatirimGiderlerTablosu[] giderler)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(null, TABGenel.BaglantiSatiri);
            ObjectArray list = new TRMHibeYatirim().Listele(ve, kullanan, new HibeYatirim() { kod = giderler[0].hibeKod });
            string basvuruNo = "";
            if (list.ObjeSayisi > 0)
                basvuruNo = ((HibeYatirim)list.objeler[0]).basvuruNo;

            string hata = IsKurallari.TRMHibeYatirimYonetim.HibeYatirimOnlineKapandiMi(ve, kullanan, basvuruNo);

            Sonuc sonuc = new Sonuc(false, "", "");
            if (!string.IsNullOrEmpty(hata))
            {
                sonuc.hataStr = "Yazma yetkiniz bulunmamaktadır.";
                sonuc.islemSonuc = false;
                ve.VeriErisimKapat();
                return sonuc;
            }
            TABHibeYatirimBasvuruTablolari to = new TABHibeYatirimBasvuruTablolari(ve);
            ve.BeginTransaction();

            sonuc.islemSonuc = true;
            foreach (YatirimGiderlerTablosu yk in giderler)
            {
                if (yk == null) continue;

                if (yk.detaylar != null)
                {
                    foreach (YatirimGiderlerTablosuDetay d in yk.detaylar)
                    {
                        sonuc = to.SilGiderlerDetay(d);
                        if (!sonuc.islemSonuc)
                            break;
                    }
                }
                if (sonuc.islemSonuc)
                    sonuc = to.SilGiderler(yk);
                if (!sonuc.islemSonuc)
                    break;
            }

            if (sonuc.islemSonuc)
            {
                if (giderler != null)
                {
                    foreach (YatirimGiderlerTablosu yk in giderler)
                    {
                        if (yk == null || yk.detaylar == null) continue;

                        if (string.IsNullOrEmpty(yk.giderKalemi))
                        {
                            sonuc.islemSonuc = false;
                            sonuc.hataStr = "Gider kalemi alanı boş olamaz";
                            break;
                        }
                        if (yk.detaylar != null)
                        {
                            foreach (YatirimGiderlerTablosuDetay d in yk.detaylar)
                            {
                                sonuc = to.KaydetGiderlerDetay(d);
                                if (!sonuc.islemSonuc)
                                    break;
                            }
                            if (sonuc.islemSonuc)
                                sonuc = to.KaydetGiderler(yk);
                            if (!sonuc.islemSonuc)
                                break;
                        }
                    }
                }
            }

            if (sonuc.islemSonuc)
            {
                TABHibeYatirim tot = new TABHibeYatirim(ve);
                HibeYatirimTarihce tarihce = new HibeYatirimTarihce();
                tarihce.kullaniciKod = kullanan.mernis == "" ? kullanan.kullaniciKodu : kullanan.mernis;
                tarihce.kullaniciAdi = kullanan.ADSOYAD;
                tarihce.hibeKod = giderler[0].hibeKod;
                tarihce.tarih = new TNSDateTime(DateTime.Now);
                tarihce.durum = (int)ENUMHibeYatirimBasvuruDurum.ONLINE;
                tarihce.ipAdresi = kullanan.telefon;
                tarihce.aciklama = "Giderleri kaydetme. ";
                sonuc = tot.KaydetTarihce(tarihce);

                ve.Commit();
            }
            else
            {
                sonuc.hataStr = "Kayıt sırasında bir hata oluştu" + "<br>" + sonuc.hataStr;
                ve.Rollback();
            }

            ve.VeriErisimKapat();

            return sonuc;
        }
        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, yönetim kurulunu veritabanından okur 
        /// ve YatirimGiderlerTablosu nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, Yatirim Giderler Tablosu listesini YatirimGiderlerTablosu nesne koleksiyonu olarak tutan nesne</returns>
        public ObjectArray GiderleriListele(Kullanici kullanan, YatirimGiderlerTablosu gider)
        {
            ObjectArray bilgi = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, TABGenel.BaglantiSatiri);

            TABHibeYatirimBasvuruTablolari to = new TABHibeYatirimBasvuruTablolari(ve);
            IDataReader r = to.GiderleriListele(gider);

            if (to.Hata() == "")
            {
                int siraNo = 1;
                while (r.Read())
                {
                    YatirimGiderlerTablosu y = new YatirimGiderlerTablosu();

                    y.hibeKod = VeriErisim.DegerAl(r, (int)TABHibeYatirimBasvuruTablolari.ENUMAlanlarGiderler.HIBEKOD, 0);
                    y.siraNo = VeriErisim.DegerAl(r, (int)TABHibeYatirimBasvuruTablolari.ENUMAlanlarGiderler.SIRANO, 0);
                    y.tip = VeriErisim.DegerAl(r, (int)TABHibeYatirimBasvuruTablolari.ENUMAlanlarGiderler.TIP, 0);
                    y.ayni = VeriErisim.DegerAl(r, (int)TABHibeYatirimBasvuruTablolari.ENUMAlanlarGiderler.AYNI, 0);
                    y.giderKalemi = VeriErisim.DegerAl(r, (int)TABHibeYatirimBasvuruTablolari.ENUMAlanlarGiderler.GIDERKALEMLERI, string.Empty);
                    y.baslangicTarihi = VeriErisim.DegerAl(r, (int)TABHibeYatirimBasvuruTablolari.ENUMAlanlarGiderler.BASLANGICTARIHI);
                    y.bitisTarihi = VeriErisim.DegerAl(r, (int)TABHibeYatirimBasvuruTablolari.ENUMAlanlarGiderler.BITISTARIHI);
                    y.toplamGider = VeriErisim.DegerAl(r, (int)TABHibeYatirimBasvuruTablolari.ENUMAlanlarGiderler.TOPLAMGIDER, 0.0);
                    gider.siraNo = siraNo;
                    y.detaylar = GiderleriListeleDetay(kullanan, gider);
                    bilgi.objeler.Add(y);
                    siraNo++;
                }
            }

            r.Close();

            ve.VeriErisimKapat();

            return bilgi;
        }
        public YatirimGiderlerTablosuDetay[] GiderleriListeleDetay(Kullanici kullanan, YatirimGiderlerTablosu gider)
        {
            ObjectArray bilgi = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, TABGenel.BaglantiSatiri);

            TABHibeYatirimBasvuruTablolari to = new TABHibeYatirimBasvuruTablolari(ve);
            IDataReader r = to.GiderleriListeleDetay(gider);
            TNSCollection detaylar = new TNSCollection();

            if (to.Hata() == "")
            {
                while (r.Read())
                {
                    YatirimGiderlerTablosuDetay y = new YatirimGiderlerTablosuDetay();

                    y.hibeKod = VeriErisim.DegerAl(r, (int)TABHibeYatirimBasvuruTablolari.ENUMAlanlarGiderlerDetay.HIBEKOD, 0);
                    y.siraNo = VeriErisim.DegerAl(r, (int)TABHibeYatirimBasvuruTablolari.ENUMAlanlarGiderlerDetay.SIRANO, 0);
                    y.tip = VeriErisim.DegerAl(r, (int)TABHibeYatirimBasvuruTablolari.ENUMAlanlarGiderlerDetay.TIP, 0);
                    y.ayni = VeriErisim.DegerAl(r, (int)TABHibeYatirimBasvuruTablolari.ENUMAlanlarGiderlerDetay.AYNI, 0);
                    y.ay = VeriErisim.DegerAl(r, (int)TABHibeYatirimBasvuruTablolari.ENUMAlanlarGiderlerDetay.AY, 0);
                    y.gider = VeriErisim.DegerAl(r, (int)TABHibeYatirimBasvuruTablolari.ENUMAlanlarGiderlerDetay.GIDER, 0.0);

                    detaylar.Add(y);
                }
            }

            r.Close();

            ve.VeriErisimKapat();

            return (YatirimGiderlerTablosuDetay[])detaylar.ToArray(typeof(YatirimGiderlerTablosuDetay));
        }

        /// <summary>
        /// Arguman olarak verilen, başvuru Yatirim Süre Eylem Planı bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="bilgi">Kaydedilecek bilgiyi tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc SureEylemPlaniKaydet(Kullanici kullanan, YatirimSureEylemPlani[] plan)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(null, TABGenel.BaglantiSatiri);
            ObjectArray list = new TRMHibeYatirim().Listele(ve, kullanan, new HibeYatirim() { kod = plan[0].hibeKod });
            string basvuruNo = "";
            if (list.ObjeSayisi > 0)
                basvuruNo = ((HibeYatirim)list.objeler[0]).basvuruNo;

            string hata = IsKurallari.TRMHibeYatirimYonetim.HibeYatirimOnlineKapandiMi(ve, kullanan, basvuruNo);

            Sonuc sonuc = new Sonuc(false, "", "");
            if (!string.IsNullOrEmpty(hata))
            {
                sonuc.hataStr = "Yazma yetkiniz bulunmamaktadır.";
                sonuc.islemSonuc = false;
                ve.VeriErisimKapat();
                return sonuc;
            }
            TABHibeYatirimBasvuruTablolari to = new TABHibeYatirimBasvuruTablolari(ve);
            ve.BeginTransaction();

            sonuc = to.SilSureEylemPlaniDetay(plan[0]);
            if (sonuc.islemSonuc)
            {
                sonuc = to.SilSureEylemPlani(plan[0]);
                if (sonuc.islemSonuc)
                {
                    if (plan != null)
                    {
                        foreach (YatirimSureEylemPlani yk in plan)
                        {
                            if (yk.detaylar != null)
                            {
                                foreach (YatirimSureEylemPlaniDetay d in yk.detaylar)
                                {
                                    sonuc = to.KaydetSureEylemPlaniDetay(d);
                                    if (!sonuc.islemSonuc)
                                        break;
                                }
                                if (sonuc.islemSonuc)
                                    sonuc = to.KaydetSureEylemPlani(yk);
                                if (!sonuc.islemSonuc)
                                    break;
                            }
                        }
                    }
                }
            }

            if (sonuc.islemSonuc)
            {
                TABHibeYatirim tot = new TABHibeYatirim(ve);
                HibeYatirimTarihce tarihce = new HibeYatirimTarihce();
                tarihce.kullaniciKod = kullanan.mernis == "" ? kullanan.kullaniciKodu : kullanan.mernis;
                tarihce.kullaniciAdi = kullanan.ADSOYAD;
                tarihce.hibeKod = plan[0].hibeKod;
                tarihce.tarih = new TNSDateTime(DateTime.Now);
                tarihce.durum = (int)ENUMHibeYatirimBasvuruDurum.ONLINE;
                tarihce.ipAdresi = kullanan.telefon;
                tarihce.aciklama = "Süre eylem Planı kaydetme. ";
                sonuc = tot.KaydetTarihce(tarihce);

                ve.Commit();
            }
            else
            {
                sonuc.hataStr = "Kayıt sırasında bir hata oluştu" + "<br>" + sonuc.hataStr;
                ve.Rollback();
            }

            ve.VeriErisimKapat();

            return sonuc;
        }
        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, yönetim kurulunu veritabanından okur 
        /// ve SureEylemPlani nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, Yatirim Sure Eylem Plani listesini SureEylemPlani nesne koleksiyonu olarak tutan nesne</returns>
        public ObjectArray SureEylemPlaniListele(Kullanici kullanan, YatirimSureEylemPlani plan)
        {
            ObjectArray bilgi = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, TABGenel.BaglantiSatiri);

            TABHibeYatirimBasvuruTablolari to = new TABHibeYatirimBasvuruTablolari(ve);
            IDataReader r = to.SureEylemPlaniListele(plan);

            if (to.Hata() == "")
            {
                int satir = 1;
                while (r.Read())
                {
                    YatirimSureEylemPlani y = new YatirimSureEylemPlani();

                    y.hibeKod = VeriErisim.DegerAl(r, (int)TABHibeYatirimBasvuruTablolari.ENUMAlanlarSureEylemPlani.HIBEKOD, 0);
                    y.satirNo = VeriErisim.DegerAl(r, (int)TABHibeYatirimBasvuruTablolari.ENUMAlanlarSureEylemPlani.SATIRNO, 0);
                    y.yil = VeriErisim.DegerAl(r, (int)TABHibeYatirimBasvuruTablolari.ENUMAlanlarSureEylemPlani.YIL, 0);
                    y.faaliyetNo = VeriErisim.DegerAl(r, (int)TABHibeYatirimBasvuruTablolari.ENUMAlanlarSureEylemPlani.FAALIYETNO, string.Empty);
                    y.asama = VeriErisim.DegerAl(r, (int)TABHibeYatirimBasvuruTablolari.ENUMAlanlarSureEylemPlani.ASAMA, 0);
                    y.aciklama = VeriErisim.DegerAl(r, (int)TABHibeYatirimBasvuruTablolari.ENUMAlanlarSureEylemPlani.ACIKLAMA, string.Empty);
                    y.kurum = VeriErisim.DegerAl(r, (int)TABHibeYatirimBasvuruTablolari.ENUMAlanlarSureEylemPlani.KURUM, string.Empty);
                    plan.satirNo = satir;
                    y.detaylar = SureEylemPlaniDetayListele(kullanan, plan);
                    bilgi.objeler.Add(y);
                    satir++;
                }
            }

            r.Close();

            ve.VeriErisimKapat();

            return bilgi;
        }
        public YatirimSureEylemPlaniDetay[] SureEylemPlaniDetayListele(Kullanici kullanan, YatirimSureEylemPlani plan)
        {
            ObjectArray bilgi = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, TABGenel.BaglantiSatiri);

            TABHibeYatirimBasvuruTablolari to = new TABHibeYatirimBasvuruTablolari(ve);
            IDataReader r = to.SureEylemPlaniDetayListele(plan);
            TNSCollection detaylar = new TNSCollection();

            if (to.Hata() == "")
            {
                while (r.Read())
                {
                    YatirimSureEylemPlaniDetay y = new YatirimSureEylemPlaniDetay();

                    y.hibeKod = VeriErisim.DegerAl(r, (int)TABHibeYatirimBasvuruTablolari.ENUMAlanlarSureEylemPlaniDetay.HIBEKOD, 0);
                    y.satirNo = VeriErisim.DegerAl(r, (int)TABHibeYatirimBasvuruTablolari.ENUMAlanlarSureEylemPlaniDetay.SATIRNO, 0);
                    y.ay = VeriErisim.DegerAl(r, (int)TABHibeYatirimBasvuruTablolari.ENUMAlanlarSureEylemPlaniDetay.AY, 0);
                    y.calisma = VeriErisim.DegerAl(r, (int)TABHibeYatirimBasvuruTablolari.ENUMAlanlarSureEylemPlaniDetay.CALISMA, 0);

                    detaylar.Add(y);
                }
            }

            r.Close();

            ve.VeriErisimKapat();

            return (YatirimSureEylemPlaniDetay[])detaylar.ToArray(typeof(YatirimSureEylemPlaniDetay));
        }

        /// <summary>
        /// Arguman olarak verilen, başvuru Yatirim Iş Takvimi bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="bilgi">Kaydedilecek bilgiyi tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc IsTakvimiKaydet(Kullanici kullanan, YatirimIsTakvimi[] plan)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(null, TABGenel.BaglantiSatiri);
            ObjectArray list = new TRMHibeYatirim().Listele(ve, kullanan, new HibeYatirim() { kod = plan[0].hibeKod });
            string basvuruNo = "";
            if (list.ObjeSayisi > 0)
                basvuruNo = ((HibeYatirim)list.objeler[0]).basvuruNo;

            string hata = IsKurallari.TRMHibeYatirimYonetim.HibeYatirimOnlineKapandiMi(ve, kullanan, basvuruNo);

            Sonuc sonuc = new Sonuc(false, "", "");
            if (!string.IsNullOrEmpty(hata))
            {
                sonuc.hataStr = "Yazma yetkiniz bulunmamaktadır.";
                sonuc.islemSonuc = false;
                ve.VeriErisimKapat();
                return sonuc;
            }
            TABHibeYatirimBasvuruTablolari to = new TABHibeYatirimBasvuruTablolari(ve);
            ve.BeginTransaction();

            sonuc = to.SilIsTakvimiDetay(plan[0]);
            if (sonuc.islemSonuc)
            {
                sonuc = to.SilIsTakvimi(plan[0]);
                if (sonuc.islemSonuc)
                {
                    if (plan != null)
                    {
                        foreach (YatirimIsTakvimi yk in plan)
                        {
                            if (yk.detaylar != null)
                            {
                                foreach (YatirimIsTakvimiDetay d in yk.detaylar)
                                {
                                    sonuc = to.KaydetIsTakvimiDetay(d);
                                    if (!sonuc.islemSonuc)
                                        break;
                                }
                                if (sonuc.islemSonuc)
                                    sonuc = to.KaydetIsTakvimi(yk);
                                if (!sonuc.islemSonuc)
                                    break;
                            }
                        }
                    }
                }
            }

            if (sonuc.islemSonuc)
            {
                TABHibeYatirim tot = new TABHibeYatirim(ve);
                HibeYatirimTarihce tarihce = new HibeYatirimTarihce();
                tarihce.kullaniciKod = kullanan.mernis == "" ? kullanan.kullaniciKodu : kullanan.mernis;
                tarihce.kullaniciAdi = kullanan.ADSOYAD;
                tarihce.hibeKod = plan[0].hibeKod;
                tarihce.tarih = new TNSDateTime(DateTime.Now);
                tarihce.durum = (int)ENUMHibeYatirimBasvuruDurum.ONLINE;
                tarihce.ipAdresi = kullanan.telefon;
                tarihce.aciklama = "İş takvimi kaydetme. ";
                sonuc = tot.KaydetTarihce(tarihce);

                ve.Commit();
            }
            else
            {
                sonuc.hataStr = "Kayıt sırasında bir hata oluştu" + "<br>" + sonuc.hataStr;
                ve.Rollback();
            }

            ve.VeriErisimKapat();

            return sonuc;
        }
        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, yönetim kurulunu veritabanından okur 
        /// ve IsTakvimi nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, Yatirim Iş Takvimi listesini IsTakvimi nesne koleksiyonu olarak tutan nesne</returns>
        public ObjectArray IsTakvimiListele(Kullanici kullanan, YatirimIsTakvimi plan)
        {
            ObjectArray bilgi = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, TABGenel.BaglantiSatiri);

            TABHibeYatirimBasvuruTablolari to = new TABHibeYatirimBasvuruTablolari(ve);
            IDataReader r = to.IsTakvimiListele(plan);

            if (to.Hata() == "")
            {
                while (r.Read())
                {
                    YatirimIsTakvimi y = new YatirimIsTakvimi();

                    y.hibeKod = VeriErisim.DegerAl(r, (int)TABHibeYatirimBasvuruTablolari.ENUMAlanlarIsTakvimi.HIBEKOD, 0);
                    y.tip = VeriErisim.DegerAl(r, (int)TABHibeYatirimBasvuruTablolari.ENUMAlanlarIsTakvimi.TIP, 0);
                    y.tur = VeriErisim.DegerAl(r, (int)TABHibeYatirimBasvuruTablolari.ENUMAlanlarIsTakvimi.TUR, 0);
                    y.konu = VeriErisim.DegerAl(r, (int)TABHibeYatirimBasvuruTablolari.ENUMAlanlarIsTakvimi.KONU, string.Empty);
                    plan.tip = y.tip;
                    plan.tur = y.tur;
                    y.detaylar = IsTakvimiDetayListele(kullanan, plan);
                    bilgi.objeler.Add(y);
                }
            }

            r.Close();

            ve.VeriErisimKapat();

            return bilgi;
        }
        public YatirimIsTakvimiDetay[] IsTakvimiDetayListele(Kullanici kullanan, YatirimIsTakvimi plan)
        {
            ObjectArray bilgi = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, TABGenel.BaglantiSatiri);

            TABHibeYatirimBasvuruTablolari to = new TABHibeYatirimBasvuruTablolari(ve);
            IDataReader r = to.IsTakvimiDetayListele(plan);
            TNSCollection detaylar = new TNSCollection();

            if (to.Hata() == "")
            {
                while (r.Read())
                {
                    YatirimIsTakvimiDetay y = new YatirimIsTakvimiDetay();

                    y.hibeKod = VeriErisim.DegerAl(r, (int)TABHibeYatirimBasvuruTablolari.ENUMAlanlarIsTakvimiDetay.HIBEKOD, 0);
                    y.tip = VeriErisim.DegerAl(r, (int)TABHibeYatirimBasvuruTablolari.ENUMAlanlarIsTakvimiDetay.TIP, 0);
                    y.tur = VeriErisim.DegerAl(r, (int)TABHibeYatirimBasvuruTablolari.ENUMAlanlarIsTakvimiDetay.TUR, 0);
                    y.ay = VeriErisim.DegerAl(r, (int)TABHibeYatirimBasvuruTablolari.ENUMAlanlarIsTakvimiDetay.AY, 0);
                    y.aciklama = VeriErisim.DegerAl(r, (int)TABHibeYatirimBasvuruTablolari.ENUMAlanlarIsTakvimiDetay.ACIKLAMA, string.Empty);

                    detaylar.Add(y);
                }
            }

            r.Close();

            ve.VeriErisimKapat();

            return (YatirimIsTakvimiDetay[])detaylar.ToArray(typeof(YatirimIsTakvimiDetay));
        }

        /// <summary>
        /// Arguman olarak verilen, başvuru Yatirim Proje Bütcesi bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="bilgi">Kaydedilecek bilgiyi tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc ProjeButcesiKaydet(Kullanici kullanan, YatirimProjeButcesi[] butce)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(null, TABGenel.BaglantiSatiri);
            ObjectArray list = new TRMHibeYatirim().Listele(ve, kullanan, new HibeYatirim() { kod = butce[0].hibeKod });
            string basvuruNo = "";
            if (list.ObjeSayisi > 0)
                basvuruNo = ((HibeYatirim)list.objeler[0]).basvuruNo;

            string hata = IsKurallari.TRMHibeYatirimYonetim.HibeYatirimOnlineKapandiMi(ve, kullanan, basvuruNo);

            Sonuc sonuc = new Sonuc(false, "", "");
            if (!string.IsNullOrEmpty(hata))
            {
                sonuc.hataStr = "Yazma yetkiniz bulunmamaktadır.";
                sonuc.islemSonuc = false;
                ve.VeriErisimKapat();
                return sonuc;
            }
            TABHibeYatirimBasvuruTablolari to = new TABHibeYatirimBasvuruTablolari(ve);
            ve.BeginTransaction();

            sonuc = to.SilProjeButcesiDetay(butce[0]);
            if (sonuc.islemSonuc)
            {
                sonuc = to.SilProjeButcesi(butce[0]);
                if (sonuc.islemSonuc)
                {
                    if (butce != null)
                    {
                        foreach (YatirimProjeButcesi yk in butce)
                        {
                            if (yk.detaylar != null)
                            {
                                foreach (YatirimProjeButcesiDetay d in yk.detaylar)
                                {
                                    sonuc = to.KaydetProjeButcesiDetay(d);
                                    if (!sonuc.islemSonuc)
                                        break;
                                }
                                if (sonuc.islemSonuc)
                                    sonuc = to.KaydetProjeButcesi(yk);
                                if (!sonuc.islemSonuc)
                                    break;
                            }
                        }
                    }
                }
            }

            if (sonuc.islemSonuc)
            {
                TABHibeYatirim tot = new TABHibeYatirim(ve);
                HibeYatirimTarihce tarihce = new HibeYatirimTarihce();
                tarihce.kullaniciKod = kullanan.mernis == "" ? kullanan.kullaniciKodu : kullanan.mernis;
                tarihce.kullaniciAdi = kullanan.ADSOYAD;
                tarihce.hibeKod = butce[0].hibeKod;
                tarihce.tarih = new TNSDateTime(DateTime.Now);
                tarihce.durum = (int)ENUMHibeYatirimBasvuruDurum.ONLINE;
                tarihce.ipAdresi = kullanan.telefon;
                tarihce.aciklama = "Proje Bütçesi kaydetme. ";
                sonuc = tot.KaydetTarihce(tarihce);

                ve.Commit();
            }
            else
            {
                sonuc.hataStr = "Kayıt sırasında bir hata oluştu" + "<br>" + sonuc.hataStr;
                ve.Rollback();
            }

            ve.VeriErisimKapat();

            return sonuc;
        }
        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, yönetim kurulunu veritabanından okur 
        /// ve ProjeButcesi nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, Yatirim Proje Bütcesi listesini ProjeButcesi nesne koleksiyonu olarak tutan nesne</returns>
        public ObjectArray ProjeButcesiListele(Kullanici kullanan, YatirimProjeButcesi butce)
        {
            ObjectArray bilgi = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, TABGenel.BaglantiSatiri);

            TABHibeYatirimBasvuruTablolari to = new TABHibeYatirimBasvuruTablolari(ve);
            IDataReader r = to.ProjeButcesiListele(butce);

            if (to.Hata() == "")
            {
                while (r.Read())
                {
                    YatirimProjeButcesi y = new YatirimProjeButcesi();

                    y.hibeKod = VeriErisim.DegerAl(r, (int)TABHibeYatirimBasvuruTablolari.ENUMAlanlarProjeButcesi.HIBEKOD, 0);
                    y.giderKalemTuru = VeriErisim.DegerAl(r, (int)TABHibeYatirimBasvuruTablolari.ENUMAlanlarProjeButcesi.GIDERKALEMTURU, 0);
                    y.siraNo = VeriErisim.DegerAl(r, (int)TABHibeYatirimBasvuruTablolari.ENUMAlanlarProjeButcesi.SIRANO, string.Empty);
                    y.giderKalemi = VeriErisim.DegerAl(r, (int)TABHibeYatirimBasvuruTablolari.ENUMAlanlarProjeButcesi.GIDERKALEMI, string.Empty);
                    y.toplamTutar = VeriErisim.DegerAl(r, (int)TABHibeYatirimBasvuruTablolari.ENUMAlanlarProjeButcesi.TOPLAMTUTAR, 0.0);
                    butce.giderKalemTuru = y.giderKalemTuru;
                    y.detaylar = ProjeButcesiDetayListele(kullanan, butce);
                    bilgi.objeler.Add(y);
                }
            }

            r.Close();

            ve.VeriErisimKapat();

            return bilgi;
        }
        public YatirimProjeButcesiDetay[] ProjeButcesiDetayListele(Kullanici kullanan, YatirimProjeButcesi butce)
        {
            ObjectArray bilgi = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, TABGenel.BaglantiSatiri);

            TABHibeYatirimBasvuruTablolari to = new TABHibeYatirimBasvuruTablolari(ve);
            IDataReader r = to.ProjeButcesiDetayListele(butce);
            TNSCollection detaylar = new TNSCollection();

            if (to.Hata() == "")
            {
                while (r.Read())
                {
                    YatirimProjeButcesiDetay y = new YatirimProjeButcesiDetay();

                    y.hibeKod = VeriErisim.DegerAl(r, (int)TABHibeYatirimBasvuruTablolari.ENUMAlanlarProjeButcesiDetay.HIBEKOD, 0);
                    y.giderKalemTuru = VeriErisim.DegerAl(r, (int)TABHibeYatirimBasvuruTablolari.ENUMAlanlarProjeButcesiDetay.GIDERKALEMTURU, 0);
                    y.ay = VeriErisim.DegerAl(r, (int)TABHibeYatirimBasvuruTablolari.ENUMAlanlarProjeButcesiDetay.AY, 0);
                    y.tutar = VeriErisim.DegerAl(r, (int)TABHibeYatirimBasvuruTablolari.ENUMAlanlarProjeButcesiDetay.TUTAR, 0.0);

                    detaylar.Add(y);
                }
            }

            r.Close();

            ve.VeriErisimKapat();

            return (YatirimProjeButcesiDetay[])detaylar.ToArray(typeof(YatirimProjeButcesiDetay));
        }

        /// <summary>
        /// Arguman olarak verilen, başvuru Yatirim Finansman Kaynagi bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="bilgi">Kaydedilecek bilgiyi tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc FinansmanKaynagiKaydet(Kullanici kullanan, YatirimFinansmanKaynagi[] kaynak)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(null, TABGenel.BaglantiSatiri);
            ObjectArray list = new TRMHibeYatirim().Listele(ve, kullanan, new HibeYatirim() { kod = kaynak[0].hibeKod });
            string basvuruNo = "";
            if (list.ObjeSayisi > 0)
                basvuruNo = ((HibeYatirim)list.objeler[0]).basvuruNo;

            string hata = IsKurallari.TRMHibeYatirimYonetim.HibeYatirimOnlineKapandiMi(ve, kullanan, basvuruNo);

            Sonuc sonuc = new Sonuc(false, "", "");
            if (!string.IsNullOrEmpty(hata))
            {
                sonuc.hataStr = "Yazma yetkiniz bulunmamaktadır.";
                sonuc.islemSonuc = false;
                ve.VeriErisimKapat();
                return sonuc;
            }
            TABHibeYatirimBasvuruTablolari to = new TABHibeYatirimBasvuruTablolari(ve);
            ve.BeginTransaction();

            sonuc = to.SilFinansmanKaynagiDetay(kaynak[0]);
            if (sonuc.islemSonuc)
            {
                sonuc = to.SilFinansmanKaynagi(kaynak[0]);
                if (sonuc.islemSonuc)
                {
                    if (kaynak != null)
                    {
                        foreach (YatirimFinansmanKaynagi yk in kaynak)
                        {
                            if (yk.detaylar != null)
                            {
                                foreach (YatirimFinansmanKaynagiDetay d in yk.detaylar)
                                {
                                    sonuc = to.KaydetFinansmanKaynagiDetay(d);
                                    if (!sonuc.islemSonuc)
                                        break;
                                }
                                if (sonuc.islemSonuc)
                                    sonuc = to.KaydetFinansmanKaynagi(yk);
                                if (!sonuc.islemSonuc)
                                    break;
                            }
                        }
                    }
                }
            }

            if (sonuc.islemSonuc)
            {
                TABHibeYatirim tot = new TABHibeYatirim(ve);
                HibeYatirimTarihce tarihce = new HibeYatirimTarihce();
                tarihce.kullaniciKod = kullanan.mernis == "" ? kullanan.kullaniciKodu : kullanan.mernis;
                tarihce.kullaniciAdi = kullanan.ADSOYAD;
                tarihce.hibeKod = kaynak[0].hibeKod;
                tarihce.tarih = new TNSDateTime(DateTime.Now);
                tarihce.durum = (int)ENUMHibeYatirimBasvuruDurum.ONLINE;
                tarihce.ipAdresi = kullanan.telefon;
                tarihce.aciklama = "Finansman Kaynağı kaydetme. ";
                sonuc = tot.KaydetTarihce(tarihce);

                ve.Commit();
            }
            else
            {
                sonuc.hataStr = "Kayıt sırasında bir hata oluştu" + "<br>" + sonuc.hataStr;
                ve.Rollback();
            }

            ve.VeriErisimKapat();

            return sonuc;
        }
        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, yönetim kurulunu veritabanından okur 
        /// ve FinansmanKaynagi nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, Yatirim Finansman Kaynagi listesini FinansmanKaynagi nesne koleksiyonu olarak tutan nesne</returns>
        public ObjectArray FinansmanKaynagiListele(Kullanici kullanan, YatirimFinansmanKaynagi kaynak)
        {
            ObjectArray bilgi = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, TABGenel.BaglantiSatiri);

            TABHibeYatirimBasvuruTablolari to = new TABHibeYatirimBasvuruTablolari(ve);
            IDataReader r = to.FinansmanKaynagiListele(kaynak);

            if (to.Hata() == "")
            {
                while (r.Read())
                {
                    YatirimFinansmanKaynagi y = new YatirimFinansmanKaynagi();

                    y.hibeKod = VeriErisim.DegerAl(r, (int)TABHibeYatirimBasvuruTablolari.ENUMAlanlarFinansmanKaynagi.HIBEKOD, 0);
                    y.giderKalemTuru = VeriErisim.DegerAl(r, (int)TABHibeYatirimBasvuruTablolari.ENUMAlanlarFinansmanKaynagi.GIDERKALEMTURU, 0);
                    y.siraNo = VeriErisim.DegerAl(r, (int)TABHibeYatirimBasvuruTablolari.ENUMAlanlarFinansmanKaynagi.SIRANO, string.Empty);
                    y.giderKalemi = VeriErisim.DegerAl(r, (int)TABHibeYatirimBasvuruTablolari.ENUMAlanlarFinansmanKaynagi.GIDERKALEMI, string.Empty);
                    y.toplamTutar = VeriErisim.DegerAl(r, (int)TABHibeYatirimBasvuruTablolari.ENUMAlanlarFinansmanKaynagi.TOPLAMTUTAR, 0.0);
                    kaynak.giderKalemTuru = y.giderKalemTuru;
                    y.detaylar = FinansmanKaynagiDetayListele(kullanan, kaynak);
                    bilgi.objeler.Add(y);
                }
            }

            r.Close();

            ve.VeriErisimKapat();

            return bilgi;
        }
        public YatirimFinansmanKaynagiDetay[] FinansmanKaynagiDetayListele(Kullanici kullanan, YatirimFinansmanKaynagi kaynak)
        {
            ObjectArray bilgi = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, TABGenel.BaglantiSatiri);

            TABHibeYatirimBasvuruTablolari to = new TABHibeYatirimBasvuruTablolari(ve);
            IDataReader r = to.FinansmanKaynagiDetayListele(kaynak);
            TNSCollection detaylar = new TNSCollection();

            if (to.Hata() == "")
            {
                while (r.Read())
                {
                    YatirimFinansmanKaynagiDetay y = new YatirimFinansmanKaynagiDetay();

                    y.hibeKod = VeriErisim.DegerAl(r, (int)TABHibeYatirimBasvuruTablolari.ENUMAlanlarFinansmanKaynagiDetay.HIBEKOD, 0);
                    y.giderKalemTuru = VeriErisim.DegerAl(r, (int)TABHibeYatirimBasvuruTablolari.ENUMAlanlarFinansmanKaynagiDetay.GIDERKALEMTURU, 0);
                    y.kaynakTipi = VeriErisim.DegerAl(r, (int)TABHibeYatirimBasvuruTablolari.ENUMAlanlarFinansmanKaynagiDetay.KAYNAKTIP, 0);
                    y.tutar = VeriErisim.DegerAl(r, (int)TABHibeYatirimBasvuruTablolari.ENUMAlanlarFinansmanKaynagiDetay.TUTAR, 0.0);
                    y.oran = VeriErisim.DegerAl(r, (int)TABHibeYatirimBasvuruTablolari.ENUMAlanlarFinansmanKaynagiDetay.ORAN, 0.0);

                    detaylar.Add(y);
                }
            }

            r.Close();

            ve.VeriErisimKapat();

            return (YatirimFinansmanKaynagiDetay[])detaylar.ToArray(typeof(YatirimFinansmanKaynagiDetay));
        }
    }
}
