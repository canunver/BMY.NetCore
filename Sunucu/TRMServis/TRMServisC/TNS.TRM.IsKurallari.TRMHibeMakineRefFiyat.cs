using System.Data;
using TNS.KYM;
using TNS.TRM.IsKurallari.Veri;
using TNS.Veri;

namespace TNS.TRM.IsKurallari
{
    public class TRMHibeMakineRefFiyat
    {
        /// <summary>
        /// Arguman olarak verilen, HibeMakineRefFiyat bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="bilgi">Kaydedilecek bilgiyi tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc Kaydet(Kullanici kullanan, ObjectArray liste)
        {
            Sonuc sonuc = new Sonuc(false, "", "");

            VeriErisim ve = VeriErisim.VeriErisimBul(null, TABGenel.BaglantiSatiri);
            TABHibeMakineReferansFiyat to = new TABHibeMakineReferansFiyat(ve);
            ve.BeginTransaction();

            //xHibeMakineReferansFiyatBasvuran.aspx göre 
            //foreach (HibeMakine m in liste.objeler)
            //{
            //    HibeMakine hm = new HibeMakine();
            //    string hata = new TRMHibeMakine().IslemKontrol(kullanan, m, ref hm, "ReferansFiyat");
            //    if (hata != "")
            //    {
            //        sonuc.islemSonuc = false;
            //        sonuc.hataStr = hata;
            //        break;
            //    }
            //    if (hm.durum >= (int)ENUMHibeMakineBasvuruDurum.ODEMEYEGONDER && hm.durum != (int)ENUMHibeMakineBasvuruDurum.BASVURUREDDEDILDI)
            //    {
            //        sonuc.islemSonuc = false;
            //        sonuc.hataStr = hm.basvuruNo + " nolu başvurunun ödeme işlemleri başlatıldığı için referans fiyatı değiştirilemez.";
            //        break;
            //    }

            //    m.detay[0].adet = hm.detay[0].adet;
            //    m.detay[0].hibeKod = m.kod;
            //    TRMHibeMakine.HibeTutarHesapla(m.hukukiStatu, m.detay[0].referansTutar, m.detay[0].proformaTutar, m.detay[0].malzemeTutar, ref m.detay[0].hibeEsasTutar, ref m.detay[0].toplamTutar, ref m.detay[0].ayniKatki, ref m.detay[0].hibeTutar, m.detay[0].hibeOran, m.detay[0].adet);
            //    sonuc = to.BasvuruFiyatlariGuncelle(m.detay[0]);
            //    if (!sonuc.islemSonuc)
            //        break;
            //}

            //HibeMakineReferansFiyat.aspx göre
            bool merkezRefKayitYapabilir = false;
            if (kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.SISTEMYONETICISI) ||
                kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.DESTEKBASVURUYONETICI) ||
                kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.DESTEKBASVURUUZMAN))
                merkezRefKayitYapabilir = true;

            bool basvurulariGuncelle = false;
            int satirNo = 0;
            foreach (HibeMakineRefFiyat bilgi in liste.objeler)
            {
                satirNo++;

                if (!merkezRefKayitYapabilir && bilgi.ilKod == "0000000")
                {
                    sonuc.islemSonuc = false;
                    sonuc.hataStr = "<br>Merkez referans fiyatları üzerinde değişiklik yapma yetkiniz yok.";
                    break;
                }

                if (bilgi.fiyat > 0)
                {
                    decimal merkezReferansTutar = TRMHibeMakinaTanim.ReferansFiyatListele(bilgi.etapNo, bilgi.malzemeKod, bilgi.ilKod);

                    if (merkezReferansTutar>0 && merkezReferansTutar < OrtakFonksiyonlar.ConvertToDecimal(bilgi.fiyat))
                    {
                        sonuc.islemSonuc = false;
                        sonuc.hataStr = satirNo + ". satırdaki malzeme için Merkez referans fiyatı " + merkezReferansTutar.ToString("#,###.0000") + "TL dir. Bunun üzerinde referans fiyat veremezsiniz.";
                        break;
                    }
                }

                bilgi.islemYapan = kullanan.kullaniciKodu;
                sonuc = to.Kaydet(bilgi);
                if (sonuc.islemSonuc)
                    sonuc = to.KaydetTarihce(bilgi);
                if (sonuc.islemSonuc && bilgi.ilKod != "0000000")
                {
                    sonuc = to.BasvuruRefFiyatlariGuncelle(bilgi);
                    basvurulariGuncelle = true;
                }
                if (!sonuc.islemSonuc)
                    break;
            }

            if (sonuc.islemSonuc)
            {
                ve.Commit();

                if (basvurulariGuncelle)
                    HibeMakineFiyatlariGuncelle(ve, kullanan, liste);//HibeMakineReferansFiyat.aspx göre
            }
            else
            {
                sonuc.hataStr = "Kayıt sırasında bir hata oluştu" + "<br>" + sonuc.hataStr;
                ve.Rollback();
            }

            ve.VeriErisimKapat();

            return sonuc;
        }

        void HibeMakineFiyatlariGuncelle(VeriErisim ve, Kullanici kullanan, ObjectArray liste)
        {
            TABHibeMakineReferansFiyat to = new TABHibeMakineReferansFiyat(ve);

            foreach (HibeMakineRefFiyat bilgi in liste.objeler)
            {
                HibeMakine h = new HibeMakine();
                HibeMakineDetay[] hd = new HibeMakineDetay[1];
                hd[0] = new HibeMakineDetay();

                h.etapNo = bilgi.etapNo;
                h.ilKod = bilgi.ilKod;
                hd[0].malzemeKod = bilgi.malzemeKod;
                h.detay = hd;

                ObjectArray makListe = TRMHibeMakine.Sorgula(kullanan, h, 0, 0, "kod", true);
                foreach (HibeMakine m in makListe.objeler)
                {
                    if (m.durum == (int)ENUMHibeMakineBasvuruDurum.BASVURUREDDEDILDI ||
                        m.durum > (int)ENUMHibeMakineBasvuruDurum.PROJENUMARASIVERILDI ||
                        m.detay[0].referansTutar == 0)
                        continue;

                    TRMHibeMakine.HibeTutarHesapla(m.hukukiStatu, m.detay[0].referansTutar, m.detay[0].proformaTutar, m.detay[0].malzemeTutar, ref m.detay[0].hibeEsasTutar, ref m.detay[0].toplamTutar, ref m.detay[0].ayniKatki, ref m.detay[0].hibeTutar, m.detay[0].hibeOran, m.detay[0].adet);
                    to.BasvuruFiyatlariGuncelle(m.detay[0]);
                }
            }
            ve.VeriErisimKapat();
        }

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, BankaSube bilgisini veritabanından okur 
        /// ve HibeMakineRefFiyat nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, BankaSube listesini BankaSube nesne koleksiyonu olarak tutan nesne</returns>
        public ObjectArray Listele(Kullanici kullanan, HibeMakineRefFiyat kriter)
        {
            ObjectArray bilgi = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, TABGenel.BaglantiSatiri);

            TABHibeMakineReferansFiyat to = new TABHibeMakineReferansFiyat(ve);

            bool sadeceAktifKayitlar = false;
            int aktifEtapNo = TRMHibeMakineYonetim.HibeMakineAktifEtapNo(ve);
            if (kriter.etapNo == aktifEtapNo) sadeceAktifKayitlar = true;
            IDataReader r = to.Listele(kriter, sadeceAktifKayitlar);

            if (to.Hata() == "")
            {
                while (r.Read())
                {
                    HibeMakineRefFiyat f = new HibeMakineRefFiyat();

                    f.malzemeGrupKod = VeriErisim.DegerAl(r, 0, 0);
                    f.malzemeGrupAd = VeriErisim.DegerAl(r, 1, "");
                    f.malzemeKod = VeriErisim.DegerAl(r, 2, 0);
                    f.malzemeAd = VeriErisim.DegerAl(r, 3, "");
                    f.fiyat = VeriErisim.DegerAl(r, 4, 0.0);

                    bilgi.objeler.Add(f);
                }
            }

            r.Close();

            ve.VeriErisimKapat();

            return bilgi;
        }

        public ObjectArray ListeleTarihce(Kullanici kullanan, HibeMakineRefFiyat kriter)
        {
            ObjectArray bilgi = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, TABGenel.BaglantiSatiri);

            TABHibeMakineReferansFiyat to = new TABHibeMakineReferansFiyat(ve);
            IDataReader r = to.ListeleTarihce(kriter);

            if (to.Hata() == "")
            {
                while (r.Read())
                {
                    HibeMakineRefFiyat f = new HibeMakineRefFiyat();
                    f.fiyat = VeriErisim.DegerAl(r, 3, 0.0);
                    f.islemYapan = VeriErisim.DegerAl(r, 4, "");
                    f.islemTarihi = VeriErisim.DegerAl(r, 5);

                    bilgi.objeler.Add(f);
                }
            }

            r.Close();

            ve.VeriErisimKapat();

            return bilgi;
        }

        public Sonuc SonrakiYilaAktar(Kullanici kullanan, HibeMakineRefFiyat bilgi, double ufeOran)
        {
            Sonuc sonuc = new Sonuc(false, "", "");
            VeriErisim ve = VeriErisim.VeriErisimBul(null, TABGenel.BaglantiSatiri);

            TABHibeMakineReferansFiyat to = new TABHibeMakineReferansFiyat(ve);
            ve.BeginTransaction();

            sonuc = to.SilTarihce(bilgi.etapNo + 1, bilgi.ilKod);

            if (sonuc.islemSonuc)
                sonuc = to.Sil(bilgi.etapNo + 1, bilgi.ilKod);

            if (sonuc.islemSonuc)
                sonuc = to.SonrakiYilaAktar(bilgi, ufeOran, kullanan.ad + " " + kullanan.soyad);

            if (sonuc.islemSonuc)
                ve.Commit();
            else
            {
                sonuc.hataStr = "Kayıt sırasında bir hata oluştu" + "<br>" + sonuc.hataStr;
                ve.Rollback();
            }

            ve.VeriErisimKapat();

            return sonuc;
        }
    }
}
