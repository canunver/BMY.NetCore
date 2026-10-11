using System;
using System.Data;
using TNS.IKY;
using TNS.IKY.IsKurallari.Veri;
using TNS.KYM;
using TNS.Veri;
using TNS.UZY;
using TNS.MUH;

namespace TNS.IKY.IsKurallari
{
    class IKYPersonelSozlesme
    {
        public static string VERIERISIMAD = "BaglantiSatiriIKY";

        public static ObjectArray Listele(VeriErisim veriErisim, Kullanici kullanan, PersonelSozlesme form)
        {
            ObjectArray pVAN = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));

            TABPersonelSozlesme tab = new TABPersonelSozlesme(ve);

            IDataReader r = tab.Listele(form);

            if (r == null)
            {
                ve.VeriErisimKapat();
                pVAN.sonuc.DegerAta(ve.hata);
                return pVAN;
            }

            while (r.Read())
            {
                PersonelSozlesme d = new PersonelSozlesme();
                d.kod = VeriErisim.DegerAl(r, (int)TABPersonelSozlesme.ENUMAlanlar.KOD, 0);
                d.yil = VeriErisim.DegerAl(r, (int)TABPersonelSozlesme.ENUMAlanlar.YIL, 0);
                d.tcKimlikNo = VeriErisim.DegerAl(r, (int)TABPersonelSozlesme.ENUMAlanlar.TCKIMLIKNO, "");
                d.adSoyad = VeriErisim.DegerAl(r, (int)TABPersonelSozlesme.ENUMAlanlar.ADSOYAD, "");
                d.basTarih = VeriErisim.DegerAl(r, (int)TABPersonelSozlesme.ENUMAlanlar.BASLANGICTARIHI);
                d.bitTarih = VeriErisim.DegerAl(r, (int)TABPersonelSozlesme.ENUMAlanlar.BITISTARIHI);
                d.tutar = VeriErisim.DegerAl(r, (int)TABPersonelSozlesme.ENUMAlanlar.TUTAR, 0.0);
                d.sozlesmeKod = VeriErisim.DegerAl(r, (int)TABPersonelSozlesme.ENUMAlanlar.SOZLESMEKOD, 0);

                pVAN.objeler.Add(d);
            }
            r.Close();

            pVAN.sonuc.islemSonuc = true;

            if (pVAN.objeler.Count == 0)
                pVAN.sonuc.bilgiStr = "Verilen kriterlere göre listelenecek bilgi <b>bulunamadı</b>.";

            ve.VeriErisimKapat();

            return pVAN;
        }

        public static Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, PersonelSozlesme[] liste)
        {
            Sonuc sonuc = new Sonuc();

            string hata = string.Empty;

            if (liste == null || liste.Length == 0)
                hata = "Kaydedilecek veri bulunamadı.<br>";
            else
            {
                foreach (var p in liste)
                {
                    if (p.yil == 0)
                        hata += "Yıl bilgisi sıfır olamaz.<br>";
                    if (string.IsNullOrWhiteSpace(p.tcKimlikNo))
                        hata += "Personel TCKimlikNo boş olamaz.<br>";
                    if (p.basTarih.isNull || p.basTarih.Yil == 1)
                        hata += "Sözleşme başlangıç tarihi boş olamaz.<br>";
                    if (p.bitTarih.isNull || p.bitTarih.Yil == 1)
                        hata += "Sözleşme bitiş tarihi boş olamaz.<br>";
                    if (p.tutar <= 0)
                        hata += "Sözleşme tutarı boş olamaz.<br>";

                    if (hata != "")
                        break;
                }
            }


            if (hata != "")
            {
                sonuc.DegerAta(hata);
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));
            TABPersonelSozlesme tab = new TABPersonelSozlesme(ve);
            ve.BeginTransaction();


            VeriErisim ve2 = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));

            foreach (var p in liste)
            {
                if (p.kod == 0)
                {
                    p.kod = TABGenel.SayacDegerAlInt(ve, TABPersonelSozlesme.TABLOAD);
                    sonuc = tab.Kaydet(p);
                }
                else
                    sonuc = tab.Guncelle(p);

                if (sonuc.islemSonuc)
                    sonuc = KaydetSozlesme(ve2, kullanan, p);

                if (sonuc.islemSonuc)
                {
                    p.sozlesmeKod = OrtakFonksiyonlar.ConvertToInt(sonuc.anahtar, 0);
                    sonuc = KaydetSozlesmeKod(ve, kullanan, p);
                }

                if (!sonuc.islemSonuc)
                    break;
            }

            if (sonuc.islemSonuc)
            {
                sonuc.bilgiStr = PersonelResI.PER007;
                ve.Commit();
            }
            else
            {
                sonuc.hataStr = PersonelResI.PER008 + "<br>" + sonuc.hataStr;
                ve.Rollback();
            }

            ve2.VeriErisimKapat();
            ve.VeriErisimKapat();
            return sonuc;
        }

        public static Sonuc Sil(VeriErisim veriErisim, Kullanici kullanan, PersonelSozlesme form)
        {
            Sonuc sonuc = new Sonuc();

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));
            TABPersonelSozlesme tab = new TABPersonelSozlesme(ve);
            ve.BeginTransaction();


            if (form.sozlesmeKod > 0)
            {
                IMUHServis servisMUH = TNS.MUH.Arac.Tanimla();
                sonuc = servisMUH.TaahhutKartiSil(kullanan, form.sozlesmeKod);
            }

            if (sonuc.islemSonuc)
            {
                sonuc = tab.Sil(form);

                if (sonuc.islemSonuc)
                {
                    sonuc.bilgiStr = PersonelResI.PER009;
                    ve.Commit();
                }
                else
                {
                    sonuc.hataStr = PersonelResI.PER010 + "<br>" + sonuc.hataStr;
                    ve.Rollback();
                }
            }

            ve.VeriErisimKapat();
            return sonuc;
        }

        public static Sonuc KaydetSozlesmeKod(VeriErisim veriErisim, Kullanici kullanan, PersonelSozlesme form)
        {
            Sonuc sonuc = new Sonuc();

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));
            TABPersonelSozlesme tab = new TABPersonelSozlesme(ve);
            ve.BeginTransaction();

            sonuc = tab.KaydetSozlesmeKod(form);

            if (sonuc.islemSonuc)
            {
                sonuc.bilgiStr = PersonelResI.PER007;
                ve.Commit();
            }
            else
            {
                sonuc.hataStr = PersonelResI.PER008 + "<br>" + sonuc.hataStr;
                ve.Rollback();
            }

            ve.VeriErisimKapat();
            return sonuc;
        }

        public static Sonuc KaydetSozlesme(VeriErisim veriErisim, Kullanici kullanan, PersonelSozlesme bilgi)
        {
            Sonuc sonuc = new Sonuc();

            string hata = "";

            TNS.IKY.Personel p = new TNS.IKY.Personel();
            p.tcKimlikNo = bilgi.tcKimlikNo;
            ObjectArray listePer = IKYPersonel.Listele(veriErisim, kullanan, p, false, true);
            if (listePer.sonuc.islemSonuc && listePer.objeler.Count == 1)
            {
                p = (TNS.IKY.Personel)listePer[0];

                TaahhutKartiBilgi form = new TaahhutKartiBilgi();
                form.kod = bilgi.sozlesmeKod;
                form.yil = bilgi.yil;
                form.muhasebeKod = p.kadro.muhasebeKodu;
                form.birimKod = p.kadro.muhasebeBirimi;
                form.projeSiraNo = ""; //txtSiraNoKayit.Text;
                form.DptProjeSiraNo = ""; //txtDptSiraNoKayit.Text;
                form.muteahhitKimlikNo = bilgi.tcKimlikNo;
                form.muteahhitUnvan = p.adSoyad;
                form.muteahhitAdres = "";//lblAdres.Text;
                form.konu = "Sözleşmeli Personel";//txtKonuKayit.Text;
                form.kesifBeldeli = bilgi.tutar;
                form.indirimOrani = 0;//ButceMuhasebeGenel.NumberAlandanDegerAl(txtIndirimOraniKayit);
                form.ihaleBedeli = bilgi.tutar;
                form.gerceklesenTaahhutTutar = 0;//ButceMuhasebeGenel.NumberAlandanDegerAl(txtGerceklesenTaahhutKayit);
                //if (txtDVTarihKayit.SelectedValue != null)
                //    form.damgaVergisiTarihi = new TNSDateTime(txtDVTarihKayit.Text);
                //form.damgaVergisiNo = txtDVNoKayit.Text;
                //form.damgaVergisiTutar = OrtakFonksiyonlar.ConvertToDbl(txtDVTutarKayit.Text);
                //if (txtSozlesmeDVTarihKayit.SelectedValue != null)
                //    form.sozlesmeDVTarihi = new TNSDateTime(txtSozlesmeDVTarihKayit.Text);
                //form.sozlesmeDVNo = txtSozlesmeDVNoKayit.Text;
                //form.sozlesmeDVTutar = OrtakFonksiyonlar.ConvertToDbl(txtSozlesmeDVTutarKayit.Text);
                //if (txtSozlesmeTarihiKayit.SelectedValue != null)
                form.sozlesmeTarihi = bilgi.basTarih; //new TNSDateTime(txtSozlesmeTarihiKayit.Text);
                //if (txtIseBaslamaTarihiKayit.SelectedValue != null)
                form.iseBaslamaTarihi = bilgi.basTarih; // new TNSDateTime(txtIseBaslamaTarihiKayit.Text);
                //if (txtGeciciKabulTarihiKayit.SelectedValue != null)
                //form.geciciKabulTarihi = new TNSDateTime(txtGeciciKabulTarihiKayit.Text);
                //if (txtIsiBitirmeTarihiKayit.SelectedValue != null)
                form.isBitirmeTarihi = bilgi.bitTarih; //new TNSDateTime(txtIsiBitirmeTarihiKayit.Text);
                //form.sureUzatimi1 = txtSure1Kayit.Text;
                //form.sureUzatimi2 = txtSure2Kayit.Text;
                //form.sureUzatimi3 = txtSure3Kayit.Text;
                //form.isKod = OrtakFonksiyonlar.ConvertToInt(ddlIs.SelectedItem.Value, 0);
                //if (txtGarantiBaslamaTarihi.SelectedValue != null)
                //    form.garantiBaslamaTarihi = new TNSDateTime(txtGarantiBaslamaTarihi.Text);
                //if (txtGarantiBitisTarihi.SelectedValue != null)
                //    form.garantiBitisTarihi = new TNSDateTime(txtGarantiBitisTarihi.Text);

                form.odenekTertip = OrtakFonksiyonlar.WebConfigOku("PersonelSozlesmeTertip", "");
                form.tertipHesapKod = ""; //???
                form.duranTertip = form.odenekTertip;
                form.aciklama = ""; //txtAciklamaKayit.Text;
                form.islemYapanAd = kullanan.ADSOYAD;
                form.islemYapanKod = kullanan.TCKIMLIKNO;
                form.islemTarihi = new TNSDateTime(DateTime.Now);

                //if (TNS.MUH.Arac.MuhasebeDovizVarMi())
                //    form.parabirimiKod = ddlParabirimi.SelectedItem.Value.ToString();
                //else
                form.parabirimiKod = "";

                IMUHServis servisMUH = TNS.MUH.Arac.Tanimla();

                sonuc = servisMUH.TaahhutKartiKaydet(kullanan, form, null);
                if (sonuc.islemSonuc)
                    hata += sonuc.hataStr;
            }
            else
            {
                hata += bilgi.tcKimlikNo + " TCKimlikNo'lu personel bulunamadı.";
            }

            return sonuc;
        }
    }
}
