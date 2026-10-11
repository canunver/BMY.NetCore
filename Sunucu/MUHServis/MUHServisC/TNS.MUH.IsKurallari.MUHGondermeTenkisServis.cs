using System;
using System.Collections;
using System.Data;
using TNS.BKD;
using TNS.KYM;
using TNS.MUH.IsKurallari.Veri;
using TNS.Veri;
using System.Collections.Generic;
using System.Text;
using TNS.UZY;
using TNS.BHMv2;

namespace TNS.MUH.IsKurallari
{
    /// <summary>
    /// ödenek Gönderme / Tenkis işlemlerinin yapıldığı sınıf
    /// </summary>
    public class MUHGondermeTenkisServis
    {
        TNS.BHMv2.IBHMv2Servis bhmServis;

        public bool SurecliTalep { get { return false; } }

        /// <summary>
        /// Ödenek Gönderme/tenkis kontrolü
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">İşlem yapılacak bilgilerin taşındığı nesne</param>
        /// <param name="sadeceKontrolEt">Sadece gönderilen bilgilerin kayıt edilmesinde bir aykırılık var mı kontrol et. Kayıt yapma.</param>
        /// <param name="MPRevizeDeYap">Eğer değeri <c>true</c> Masraf Planı revize de yapılsın.</param>
        /// <returns></returns>
        public Sonuc Kaydet(Kullanici kullanan, GondermeTenkisForm form, bool sadeceKontrolEt, bool MPRevizeDeYap, bool donemOdenegi, bool sgbKbsKontrolYapma, string talepYapan = null, string talepNo = null, List<KullaniciMesaj> mesajListe = null)
        {
            SaymanlikBirimBul(kullanan, form);
            List<KullaniciMesaj> al = new List<KullaniciMesaj>();
            TNS.BHMv2.IBHMv2Servis servisBhmv2 = TNS.BHMv2.Arac.Tanimla();
            Sonuc sonuc = DonemKapaliMi(servisBhmv2, kullanan, form);
            if (!sonuc.islemSonuc) return sonuc;
            TNS.BKD.MPRevizeForm mpRevizeForm = new BKD.MPRevizeForm();
            bhmServis = TNS.BHMv2.Arac.Tanimla();

            int mpDurum = bhmServis.MasrafPlaniGenelOnayDurumBul(kullanan, form.yil, "", BHMv2.ENUMMasrafPlanAsama.AHP);

            if (mpDurum == 5 && !string.IsNullOrWhiteSpace(form.belgeNo) && form.talep == "")
            {
                TNS.BKD.MPRevizeForm mpRevizeFormOnayKaldir = new BKD.MPRevizeForm();

                mpRevizeFormOnayKaldir.yil = 0;
                mpRevizeForm.formDurum = BKD.ENUMRevizeFormDurum.Iptal;

                mpRevizeForm.odGonNo = form.belgeNo;
                mpRevizeForm.odGonTur = (int)form.belgeTur;
                mpRevizeForm.odGonYil = form.yil;
                TNSCollection tc = new TNSCollection();
                tc.Add(mpRevizeForm);
                bhmServis.MPRevizeFormDurumAta(kullanan, tc);
            }
            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(kullanan));
            int girisFormDurum = form.formDurum;
            sonuc = KontrolEt(ve, kullanan, servisBhmv2, form, mpRevizeForm, mpDurum, donemOdenegi, ref talepNo, ref talepYapan, sgbKbsKontrolYapma, true, new Hashtable());

            if (sadeceKontrolEt)
            {
                if (sonuc.islemSonuc)
                    sonuc.anahtar = "TAMAM";
                else
                    sonuc.anahtar = "HATA";
                ve.VeriErisimKapat();
                return sonuc;
            }

            if (sonuc.islemSonuc)
            {
                //Sayac kod bilgisi
                string sayacKod = form.yil + form.kurum.Replace(".", "");
                if (form.belgeTur == ENUMBelgeTuru.ODENEKGONDERME)
                    sayacKod = "GON" + sayacKod;
                else if (form.belgeTur == ENUMBelgeTuru.TENKIS)
                    sayacKod = "TEN" + sayacKod;
                //*************************************************************
                bool ekle = false;
                if (form.belgeNo.Trim() == "")
                {
                    ekle = true;
                    form.formDurum = (int)ENUMBelgeDurumu.YENI;
                    form.icmalNo = 0;

                    //Belgeye numara alınıyor
                    //**********************************************************
                    TABGenel tg = new TABGenel(ve);

                    form.belgeNo = tg.BelgeNoAl(sayacKod, "00000");
                    //**********************************************************
                    //Eğer belgeno alındı ise aşağıda başına kurum kodu konuluyor
                    //sonuc.Çıkışta Sonuç Anahtar alanına konuluyor ki client tarafı yeni kayıt edilen belge numarasını alabilsin
                    //**********************************************************
                    if (OrtakFonksiyonlar.ConvertToInt(form.belgeNo, 0) == 0)
                    {
                        ve.VeriErisimKapat();
                        sonuc.hataStr = ButceMuhasebeResI.MUH007;
                        sonuc.islemSonuc = false;
                        return sonuc;
                    }
                    else
                        form.belgeNo = form.kurum.Replace(".", "").PadRight(4, '0') + form.belgeNo;
                    //**********************************************************
                }
                else
                {
                    if ((form.formDurum > 3 || form.formDurum < 2))
                        form.formDurum = (int)ENUMBelgeDurumu.DEGISTIRILDI;
                }

                if (form.talep != "")
                {
                    form.formDurum = SurecliTalep ? (int)ENUMBelgeDurumu.YENITALEP : (int)ENUMBelgeDurumu.TALEP;
                }
                if (OrtakFonksiyonlar.ConvertToDouble(form.belgeNo, 0) > 0)
                {
                    ve.BeginTransaction();
                    TABGondermeTenkis to = new TABGondermeTenkis(ve);

                    if (!ekle)
                        sonuc = to.SilDetay(form.yil, form.belgeNo, (int)form.belgeTur);

                    if (sonuc.islemSonuc && !string.IsNullOrEmpty(talepNo))
                    {
                        GondermeTenkisForm formEskTalep = new GondermeTenkisForm();
                        formEskTalep.formDurum = (int)ENUMBelgeDurumu.TALEPKARSILA;
                        formEskTalep.onayNo = form.onayNo;
                        formEskTalep.onayTarih.Yaz(form.onayTarih);
                        formEskTalep.islemYapan = form.islemYapan;
                        formEskTalep.islemTarih.Yaz(DateTime.Now.Date);
                        formEskTalep.yil = form.yil;
                        formEskTalep.belgeNo = talepNo;
                        formEskTalep.belgeTur = form.belgeTur;

                        sonuc = to.BelgeDurumDegistir(formEskTalep, form.belgeNo, true);
                        al.Add(new KullaniciMesaj(talepYapan, string.Format("{0} nolu talebiniz, {1} nolu ödenek gönderme belgesi haline getirildi.", talepNo, form.belgeNo)));
                    }

                    if (sonuc.islemSonuc)
                        sonuc = to.Kaydet(form, talepYapan, talepNo, ekle);

                    if (sonuc.islemSonuc)
                    {
                        ve.Commit();
                        if (girisFormDurum == (int)ENUMBelgeDurumu.ONAYLI && form.DortDuzeyDegisim == 1)
                        {
                            form.formDurum = (int)ENUMBelgeDurumu.ONAYLI;
                            to.BelgeDurumDegistir(form);
                            ve.hata = "";
                        }

                        if (form.formDurum == (int)ENUMBelgeDurumu.YENI)
                        {
                            sonuc.bilgiStr = string.Format(ButceMuhasebeResI.MUH008, form.belgeNo);
                            sonuc.anahtar = form.belgeNo;
                        }
                        else
                        {
                            sonuc.bilgiStr = string.Format(ButceMuhasebeResI.MUH009, form.belgeNo);
                            sonuc.anahtar = form.belgeNo;
                        }

                        Sonuc sonucMP = new Sonuc();
                        if (mpDurum == 5 && sonuc.islemSonuc && MPRevizeDeYap && string.IsNullOrWhiteSpace(form.talep))
                        {
                            if (form.belgeTur == ENUMBelgeTuru.TENKIS)
                            {
                                sonucMP = TenkistenMPRUret(kullanan, ve, form, bhmServis, true, 0, 0);
                            }
                            else if (mpRevizeForm.satirlar != null && mpRevizeForm.satirlar.Length > 0)
                            {
                                mpRevizeForm.aciklama = form.belgeNo + " numaralı ödenek gönderme kaydedilirken üretildi.";
                                mpRevizeForm.odGonNo = form.belgeNo;
                                mpRevizeForm.odGonTur = (int)form.belgeTur;
                                mpRevizeForm.odGonYil = form.yil;
                                sonucMP = bhmServis.MPRevizeFormKaydet(kullanan, mpRevizeForm, false);
                            }
                        }

                        if (!sonucMP.islemSonuc)
                            sonuc.bilgiStr += " İlgili masraf planı revizesi oluşturulamadı! Lütfen elle oluşturunuz!" + sonucMP.hataStr;
                    }
                    else
                        ve.Rollback();
                }
            }
            ve.VeriErisimKapat();

            TNS.KYM.IKYMServis servis = TNS.KYM.Arac.Tanimla();
            servis.KaydetMesajPanosu(kullanan, al.ToArray());
            return sonuc;
        }

        private void SaymanlikBirimBul(Kullanici kullanan, GondermeTenkisForm form)
        {
            if (form.birim.Length > 3)
            {
                string birKod;
                int sayKod;
                MUHProgramButceIslemleri.VergiNodanBirime(kullanan, null, form.birim, form.kurum, out sayKod, out birKod);
                if (sayKod <= 0 || birKod.Length != 3)
                {
                }
                else
                {
                    form.birim = birKod;
                    form.muhasebe = sayKod;
                }
            }
        }

        private Sonuc DonemKapaliMi(TNS.BHMv2.IBHMv2Servis servisBhmv2, Kullanici kullanan, GondermeTenkisForm form)
        {
            Sonuc s = new Sonuc();
            if (kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.SISTEMYONETICISI)) return s;

            int onayKod = servisBhmv2.OnayKodOkuTasra(kullanan, form.yil, 0, "000", form.OnayKodBul(), TNS.BHMv2.ENUMBirimOnayTur.ODENEKGONDERME);
            //Sonuc s = servisBhmv2.OnayKodAtaTasra(kullanan, form.yil, 0, "000", "0000", TNS.BHMv2.ENUMBirimOnayTur.PROJEGEREKCE, onayIslem);
            if (onayKod == 5)
                s.DegerAta("Ödenek gönderme işlemi birimlere kapatılmıştır. Lütfen SGB ile iletişime geçiniz!");
            return s;
        }

        private ObjectArray TalepDetayAl(ObjectArray formDetay, int yil, string aciklama)
        {
            ObjectArray liste = new ObjectArray();

            for (int i = 0; i < formDetay.objeler.Count; i++)
            {
                GondermeTenkisDetay dty = (GondermeTenkisDetay)formDetay[i];
                string tertip = dty.tertip.TertipStrOl().Replace(".", "");
                decimal tutar = OrtakFonksiyonlar.ConvertToDecimal(dty.miktar);
                if (!string.IsNullOrEmpty(tertip) && tutar > 0)
                {
                    TalepDetay detay = new TalepDetay();
                    detay.yil = yil;
                    //detay.talepNo = OrtakFonksiyonlar.ConvertToInt(txtTalepNo.Text, 0);
                    detay.siraNo = i;
                    detay.talepTutar = OrtakFonksiyonlar.ConvertToDecimal(dty.miktar);
                    detay.talepAciklama = aciklama;

                    string[] tertipler = tertip.Split('-');
                    detay.kurKod = tertipler[0];
                    detay.fonKod = tertipler[1];
                    detay.finKod = tertipler[2];
                    detay.ekoKod = tertipler[3];
                    if (tertipler.Length > 4)
                        detay.pebKod = tertipler[5];

                    liste.objeler.Add(detay);
                }
            }

            return liste;
        }

        Sonuc KontrolEt(VeriErisim ve, Kullanici kullanan, TNS.BHMv2.IBHMv2Servis servisBhmv2, GondermeTenkisForm form, TNS.BKD.MPRevizeForm mpRevizeForm, int mpDurum, bool donemOdenegi, ref string eskiTalepNo, ref string talepYapan, bool sgbKbsKontrolYapma, bool kaydetdenCagrildi, Hashtable gtiFormHT)
        {
            bool debug = false;
            StringBuilder sb;
            if (debug) sb = new StringBuilder();
            else sb = null;
            Tertip tUzBul = new Tertip(form.yil, ENUMTabloKod.TABPEBOdenek);
            int eko2Uz = tUzBul.KodUzunlukBul(3, 2, true);
            int eko3Uz = tUzBul.KodUzunlukBul(3, 3, true);
            TNS.MUH.IMUHServis muhServis = TNS.MUH.Arac.Tanimla();
            string ortak000 = TNS.MUH.Arac.DegiskenDegerBul(form.yil, "/Ortak000");

            //Para Formatla
            System.Globalization.NumberFormatInfo n = (System.Globalization.NumberFormatInfo)System.Globalization.CultureInfo.CurrentCulture.NumberFormat.Clone();
            n.NumberDecimalDigits = 2;
            string hata = "";
            Sonuc sonuc = new Sonuc(true, "", "");
            if (form.belgeTur == TNS.MUH.ENUMBelgeTuru.ODENEKGONDERME)
            {
                if (MUHMuhasebeBirimServis.BirimKapaliMi(ve, kullanan, form.muhasebe, form.birim, form.kurum))
                {
                    sonuc.DegerAta("Birim kapalı olduğundan ödenek gönderilemez!");
                    return sonuc;
                }
            }
            ENUMOdenekKontrolTur okTur = TNS.MUH.Arac.OdenekKontrolTurBul(form.yil);
            bool eko2mi = TNS.MUH.Arac.OdenekGOndermeSadeceEko2Mi(form.yil);
            if (form.belgeTur != ENUMBelgeTuru.TENKIS)
            {
                string sgbKbsKontrol = TNS.MUH.Arac.DegiskenDegerBul(0, "SGBKBSKontrol");
                if (sgbKbsKontrol == "1" && !sgbKbsKontrolYapma)
                {
                    bool odenekKontrolluEkoVar = false;
                    for (int s = 0; s < form.detay.objeler.Count; s++)
                    {
                        GondermeTenkisDetay detay = (GondermeTenkisDetay)form.detay.objeler[s];
                        if (!OdemeEmriMIFForm.OdenekKontroluYapilmayanEkoMu4(detay.tertip.kodlar[3]))
                        {
                            odenekKontrolluEkoVar = true;
                            break;
                        }
                    }
                    if (odenekKontrolluEkoVar)
                    {
                        string kontrolHata;
                        TNS.BHMv2.Arac.SGBKBSFarkVarMi(muhServis, form.yil, kullanan, form.muhasebe, form.birim, out kontrolHata);
                        if (!string.IsNullOrWhiteSpace(kontrolHata))
                        {
                            sonuc.DegerAta(kontrolHata);
                            return sonuc;
                        }
                    }
                }
            }
            //Yetki Kontrolü
            string kbirimKur = form.kurum;
            int kontrolBelgeTur;

            if (form.talep != "")
                kontrolBelgeTur = (int)ENUMBelgeTuru.ODEMEEMRI;
            else
                kontrolBelgeTur = (int)form.belgeTur;

            if (!MUH.IsKurallari.MUHYetkiServis.YazmaYetkisiVarMiKurum(kullanan, kontrolBelgeTur, kbirimKur))
            {
                bool yetkiVar = false;
                if (form.DortDuzeyDegisim == 1)
                    if (TNS.MUH.Yetki.MuhasebeBirimYetkiVarMi(kullanan, form.muhasebe.ToString(), form.birim))
                        yetkiVar = true;
                if (!yetkiVar)
                {
                    if (form.surecNo != "" && form.surecYil != 0)
                    {
                        TNS.HRC.IHRCServis hrcServis = TNS.HRC.Arac.Tanimla();
                        Sonuc s = hrcServis.SurecYetkisiVarMi(kullanan, form.surecYil, form.surecNo, HRC.ENUMKullaniciYetkiTipi.ODEMEEMRIBELGESI);
                        if (s.islemSonuc)
                            yetkiVar = true;
                    }
                }

                if (!yetkiVar)
                {
                    sonuc.DegerAta(ButceMuhasebeResI.MUH010);
                    return sonuc;
                }
            }

            string kurumlar = MUHYetkiServis.GondermeBelgesiKurumlariVer(kullanan);
            bool yetkisiVar = false;

            if (!kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.SISTEMYONETICISI))
            {
                if (form.talep != "" || form.talep == "harcama")
                {
                    if (kurumlar == "")
                    {
                        string muhasebeBirimler = TNS.MUH.Yetki.OdemeEmriMIFMuhasebeBirimVer(kullanan);
                        string[] birimler = muhasebeBirimler.Split(';');
                        for (int kk = 0; kk < birimler.Length; kk++)
                        {
                            string[] mb = birimler[kk].Split('-');

                            if (form.muhasebe == OrtakFonksiyonlar.ConvertToInt(mb[0], 0))
                            {
                                yetkisiVar = true;
                                break;
                            }
                        }
                    }
                }

                if (!(kurumlar != "" || yetkisiVar))
                    hata += string.Format(ButceMuhasebeResI.MUH011, form.muhasebe);
            }

            if (form.yil > DateTime.Now.Year)
                hata += "<br>" + "Lütfen geçerli bir yıl giriniz!";

            if (form.yil <= 2000)
                hata += "<br>" + string.Format(ButceMuhasebeResI.MUH012, DateTime.Now.Year.ToString());

            if (string.IsNullOrWhiteSpace(form.aciklama))
                hata += "<br>" + "Açıklama alanı boş bırakılamaz!";
            else
            {
                form.aciklama= form.aciklama.Trim();
                if (form.aciklama.Length < 10)
                    hata += "<br>" + "Açıklama alanı en az 10 karakter olmalıdır!";
            }

            if (form.islemTarih == null || form.islemTarih.isNull)
                hata += "<br>" + ButceMuhasebeResI.MUH013;

            if (form.kurum.Replace(".", "").Trim() == "")
                hata += "<br>" + ButceMuhasebeResI.MUH014;
            else
            {
                string kKurum = MUHServis.UzayYaziDegerGetir(ve, kullanan, "ABSKUR", form.yil.ToString() + "-" + form.kurum, true, "KURKOD");
                if (kKurum.Replace(".", "") != form.kurum.Replace(".", ""))
                    hata += "<br><b>" + form.kurum + "</b> " + ButceMuhasebeResI.MUH015;
            }

            if (form.muhasebe == 0)
                hata += "<br>" + ButceMuhasebeResI.MUH016;
            else
            {
                double kMuhasebe = MUHServis.UzaySayiDegerGetir(ve, kullanan, "MUHMUHASEBE", form.muhasebe.ToString(), true, "MUHASEBEKOD");
                if (Convert.ToInt32(kMuhasebe) != form.muhasebe)
                    hata += "<br><b>" + form.muhasebe + "</b> " + ButceMuhasebeResI.MUH017;
            }

            if (form.belgeNo != "")
            {
                //Eğer belge değiştirilmek üzere gönderilmiş ise 
                //Kayıtlı mı
                //Kayıtlı ise veri tabanındaki belge 
                //belgeyi gönderen kuruma mı ait,
                //durumu değişikliğe uygun mu kontrolü
                if (form.belgeNo.Length < 9)
                    hata += "<br>" + string.Format(ButceMuhasebeResI.MUH018, form.belgeNo);
                if (!form.belgeNo.StartsWith(form.kurum.Replace(".", "")))
                    hata += "<br>" + string.Format(ButceMuhasebeResI.MUH019, form.belgeNo);

                GondermeTenkisForm oS = TekBelgeGetir(ve, kullanan, form, false, false, true);
                if (kaydetdenCagrildi && oS.icmalNo > 0)
                {
                    GondermeTenkisIcmal gtiFormOku = MUHOdenekGondermeIcmal_Oku(ve, kullanan, gtiFormHT, oS.yil, oS.belgeTur, oS.icmalNo);
                    if (gtiFormOku != null && gtiFormOku.formDurum == 5) //icmal onaylı, belge değiştirilemez.
                    {
                        hata += "<br>" + oS.icmalNo + " numaralı icmal onaylandığından, bu icmale ait belgelerde değişiklik yapılamaz!";
                    }
                }
                if (oS.kurum != "")
                {
                    if (oS.kurum.Replace(".", "") != form.kurum.Replace(".", ""))
                    {
                        sonuc.DegerAta(string.Format(ButceMuhasebeResI.MUH020, form.belgeNo.ToString()));
                        return sonuc;
                    }

                    if (oS.formDurum != (int)ENUMBelgeDurumu.YENI && oS.formDurum != (int)ENUMBelgeDurumu.DEGISTIRILDI && oS.formDurum != (int)ENUMBelgeDurumu.TAHSIS && oS.formDurum != (int)ENUMBelgeDurumu.TALEP && oS.formDurum != (int)ENUMBelgeDurumu.YENITALEP)
                    {
                        if (!(oS.formDurum == (int)ENUMBelgeDurumu.IPTAL && form.formDurum == (int)ENUMBelgeDurumu.MUHASEBATALDI))
                        {
                            if (oS.formDurum == (int)ENUMBelgeDurumu.YEVMIYE_OESGBYEGONDER)
                            {
                                if (!kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.SISTEMYONETICISI))
                                {
                                    hata = "<br>" + "SGB'ye gönderilen formlar değiştirilemez!";
                                }
                            }
                            else if (oS.formDurum == (int)ENUMBelgeDurumu.MUHASEBATAGONDERILDI)
                                hata = "<br>" + ButceMuhasebeResI.MUH021;
                            else if (oS.formDurum == (int)ENUMBelgeDurumu.MUHASEBATALDI)
                                hata = "<br>" + ButceMuhasebeResI.MUH022;
                            else if (oS.formDurum == (int)ENUMBelgeDurumu.ONAYLI)
                                hata = "<br>" + ButceMuhasebeResI.MUH023;
                            else if (oS.formDurum == (int)ENUMBelgeDurumu.MUHASEBATIADE)
                                hata = "<br>" + ButceMuhasebeResI.MUH024;
                            else if (oS.formDurum == (int)ENUMBelgeDurumu.IPTAL)
                                hata = "<br>" + ButceMuhasebeResI.MUH025;
                            else if (oS.formDurum == (int)ENUMBelgeDurumu.TALEPKARSILA)
                                hata = "<br>" + "Karşılanmış talepler değiştirilemez.";
                            else
                                hata += "<br>" + ButceMuhasebeResI.MUH026;

                            if (hata != "")
                            {
                                sonuc.DegerAta(hata);
                                return sonuc;
                            }
                        }
                    }
                    if (oS.formDurum == (int)ENUMBelgeDurumu.TALEP && string.IsNullOrEmpty(form.talep)) // eski talep yeni belgeye çevriliyor
                    {
                        talepYapan = oS.islemYapan;
                        eskiTalepNo = form.belgeNo;
                        form.belgeNo = "";
                        form.islemTarih.Yaz(DateTime.Now.Date);
                        form.SatirlariTopla(form.yil, okTur, TNS.BKD.Arac.Eko2_3Bul(kullanan));
                    }
                }
                else
                    hata += "<br>" + ButceMuhasebeResI.MUH027;
            }

            int sayDiger = 0;
            int say9 = 0;
            int say16 = 0;
            for (int s = 0; s < form.detay.objeler.Count; s++)
            {
                GondermeTenkisDetay detay = (GondermeTenkisDetay)form.detay.objeler[s];
                int iFin = OrtakFonksiyonlar.ConvertToInt(detay.tertip.kodlar[2], 0);
                if (iFin == 9) say9++;
                else if (iFin == 16) say16++;
                else sayDiger++;

                string satirTertip = detay.tertip.TertipStrOl();
                for (int i = s + 1; i < form.detay.objeler.Count; i++)
                {
                    GondermeTenkisDetay detayKont = (GondermeTenkisDetay)form.detay.objeler[i];
                    if (satirTertip == detayKont.tertip.TertipStrOl())
                    {
                        hata += "<br>" + string.Format(ButceMuhasebeResI.MUH029, (s + 1).ToString(), (i + 1).ToString(), satirTertip);
                        break;
                    }
                }
            }

            if (say9 > 0 || say16 > 0)
            {
                if (sayDiger > 0)
                    hata += "<br>" + "YİKOB ve İÖİ finans kodları ile diğer finans kodları aynı belgede olamaz!";
                else if (say9 > 0 && say16 > 0)
                    hata += "<br>" + "YİKOB ve İÖİ finans kodları ile diğer finans kodları aynı belgede olamaz!";
                else if (form.isKodu == 0)
                    hata += "<br>" + "YİKOB ve İÖİ finans kodları girildiğinde bir iş seçmelisiniz!";
                else if (kaydetdenCagrildi && !donemOdenegi && form.belgeTur == ENUMBelgeTuru.ODENEKGONDERME)
                    hata += "<br>" + "YİKOB ve İÖİ finans kodları girildiğinde dönem ödeneği seçmelisiniz!";
                else
                {
                    ObjectArray oa = servisBhmv2.ListeleIOIProjeGiris(kullanan, new IOIProje() { siraNo = form.isKodu }, false, -1);
                    if (oa.ObjeSayisi != 1)
                        hata += "<br>" + $"{form.isKodu} kodlu is bulunamadı!";
                    else
                    {
                        IOIProje okunanIs = (IOIProje)oa.objeler[0];
                        if (form.yil < okunanIs.baslaYil)
                        {
                            hata += "<br>" + $"{okunanIs.siraNo}-{okunanIs.ioiProjeAdi} işi {form.yil} yılında başlamadığından ödenek gönderilemez!";
                        }
                        else if (okunanIs.bitisYil > 0 && form.yil > okunanIs.bitisYil)
                        {
                            hata += "<br>" + $"{okunanIs.siraNo}-{okunanIs.ioiProjeAdi} işi {okunanIs.bitisYil} yılında bittiğinden ödenek gönderilemez!";
                        }
                        else if (okunanIs.muhasebeKod != form.muhasebe || okunanIs.birimKod != form.birim)
                        {
                            hata += "<br>" + $"{okunanIs.siraNo}-{okunanIs.ioiProjeAdi} işi {okunanIs.muhasebeKod}-{okunanIs.birimKod} biriminde tanımlıdır. O yüzden {form.muhasebe}-{form.birim} muhasebe birimine bu işten ödenek gönderemezsiniz!";
                        }
                    }

                }
            }
            else if (form.isKodu != 0)
            {
                hata += "<br>" + "YİKOB ve İÖİ finans kodları girilmediği için form bir iş ile ilişkilendirilemez!";
            }
            //**********************************************************

            //**********************************************************
            int satir = 0;

            if (form.detay.objeler.Count == 0)
                hata += ButceMuhasebeResI.MUH030;

            string kurumsalYapiKod = "";
            ArrayList revizeIslemSatirlar = null;
            if (mpRevizeForm != null && string.IsNullOrEmpty(form.talep))
                revizeIslemSatirlar = new ArrayList();
            System.Collections.Hashtable OdenekToplam = new System.Collections.Hashtable();
            Hashtable ht = new Hashtable();


            //VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(kullanan));
            bool eko23 = TNS.BKD.Arac.Eko2_3Bul(kullanan);
            foreach (GondermeTenkisDetay detay in form.detay.objeler)
            {
                satir++;

                detay.miktar = OrtakFonksiyonlar.KurusYap(detay.miktar);
                if (detay.miktar == 0)
                    hata += "<br>" + string.Format(ButceMuhasebeResI.MUH031, satir.ToString());

                if (detay.miktar < 0 && form.DortDuzeyDegisim != 1)
                    hata += "<br>" + string.Format(ButceMuhasebeResI.MUH032, satir.ToString());

                if (!TNS.MUH.Arac.TertipTamMi(detay.tertip, okTur, form.belgeTur, form.talep, eko23))
                    hata += "<br>" + string.Format(ButceMuhasebeResI.MUH033, satir.ToString()) + "(" + detay.tertip.TertipStrOl() + ")";
                else
                {
                    string kur = detay.tertip.kodlar[0];
                    string eko = detay.tertip.kodlar[3];
                    string peb = TNS.MUH.Arac.DiziElemanAl(detay.tertip.kodlar, 4, "");
                    //Yetki kontrolü
                    if (form.talep != "")
                        kontrolBelgeTur = (int)ENUMBelgeTuru.ODEMEEMRI;
                    else
                        kontrolBelgeTur = (int)form.belgeTur;

                    if (!MUH.IsKurallari.MUHYetkiServis.YazmaYetkisiVarMi(kullanan, kontrolBelgeTur, kur))
                        hata += "<br>" + string.Format(ButceMuhasebeResI.MUH034, satir.ToString(), kur);

                    if (!MUH.IsKurallari.MUHYetkiServis.EkonomikYetkiVarMi(kullanan, kontrolBelgeTur, eko))
                        hata += "<br>" + string.Format(ButceMuhasebeResI.MUH035, satir.ToString(), eko);

                    if (!MUH.IsKurallari.MUHYetkiServis.AltFaaliyetYetkiVarMi(kullanan, kontrolBelgeTur, peb))
                        hata += "<br>" + string.Format("Belgede {0}. satırda kullandığınız Faaliyet ({1}) ile ilgili işlem yapma yetkiniz yok", satir.ToString(), peb);

                    if (form.birim != "" && OrtakFonksiyonlar.ConvertToInt(form.birim, 0) > 0)
                    {
                        string kbirim = MUHServis.UzayYaziDegerGetir(ve, kullanan, "MUHMUHASEBEBIRIM", form.birim + "-" + form.muhasebe.ToString() + "-" + kur.Replace(".", ""), true, "BIRIMKOD");
                        if (kbirim != form.birim)
                            hata += string.Format(ButceMuhasebeResI.MUH036, form.birim, kur);
                    }
                    else if (form.birim.Trim() == "")
                        hata += ButceMuhasebeResI.MUH037;

                    if (form.DortDuzeyDegisim != 1)
                    {
                        if (string.IsNullOrWhiteSpace(form.talep))
                        {
                            if (!eko2mi)
                            {
                                if (mpDurum == 5)
                                {
                                    if (string.IsNullOrEmpty(kurumsalYapiKod))
                                    {
                                        string[] kodDizi = bhmServis.KurumsalYapiListele(form.muhasebe.ToString(), form.birim, kur);
                                        if (kodDizi != null && kodDizi.Length == 1)
                                        {
                                            kurumsalYapiKod = kodDizi[0];
                                        }
                                        else
                                        {
                                            hata += "<br>" + string.Format(ButceMuhasebeResI.MUH038, form.muhasebe, form.birim, kur);
                                            kurumsalYapiKod = "H";
                                            break;
                                        }
                                    }
                                }
                                hata += OdenekTenkisSatirKontrolPeb(ve, kullanan, form, detay, satir, n, kurumsalYapiKod, ht, revizeIslemSatirlar, mpDurum, donemOdenegi, eko2Uz, eko3Uz);
                            }
                            else
                                hata += OdenekTenkisSatirKontrolEko2(ve, kullanan, form, detay, satir, n);
                        }
                        else
                        {
                            if (form.belgeTur == ENUMBelgeTuru.TENKIS)
                            {
                                double belgeToplam = 0;
                                if (!string.IsNullOrWhiteSpace(form.belgeNo))
                                {
                                    string sorguTENBirim = form.yil + "-" + (int)ENUMBelgeTuru.TENKIS + "-" + form.belgeNo + "-" + "-" + "" + "-" + "" + "-" + "-" + "" + "-" + detay.tertip.TertipStrOl();
                                    belgeToplam = Math.Abs(MUHServis.UzaySayiDegerGetir(ve, kullanan, "MUHGONDERMEBELGESITENKISKOSULSUZ", sorguTENBirim, false, "SUM(MIKTAR)"));
                                }

                                SatirDurumBul(ve, kullanan, bhmServis, detay, form.yil, form.muhasebe.ToString(), form.birim, form.belgeTur, mpDurum, false, belgeToplam, true, ortak000 == "1");
                                if (Math.Round(detay.kullanilabilir - detay.miktar, 3) < 0)
                                {
                                    hata += "<br>" + string.Format("{0} tertibi için en fazla {1} TL Tenkis edebilirsiniz!", detay.tertip.TertipStrOl(), detay.kullanilabilir.ToString("#,##0.00"));
                                }
                            }
                        }
                    }
                    else if (form.DortDuzeyDegisim == 1)
                    {
                        hata += OdenekTenkisSatirKontrolDegisim(ve, kullanan, form, detay, satir, n, OdenekToplam, eko2Uz, eko3Uz);
                    }
                }
            }

            if (form.talep == "harcama")
                form.talep = "";

            hata += OdenekTenkisSatirKontrolPebHT(ve, kullanan, form, n, ht);

            if (form.DortDuzeyDegisim == 1)
            {
                foreach (System.Collections.DictionaryEntry de in OdenekToplam)
                {
                    if ((double)de.Value != 0)
                    {
                        hata += "<br>" + string.Format(ButceMuhasebeResI.MUH039, (string)de.Key + "(" + ((double)de.Value).ToString() + ")");
                    }
                }
            }

            if (string.IsNullOrEmpty(hata) && mpDurum == 5 && mpRevizeForm != null && revizeIslemSatirlar != null)
            {
                mpRevizeForm.yil = form.yil;
                mpRevizeForm.revizeFormTip = BKD.ENUMRevizeFormTip.MasrafPlaniRevize;
                mpRevizeForm.onaylayanKullaniciKodu = kullanan.kullaniciKodu;
                mpRevizeForm.onaylamaTarihi = new TNSDateTime(DateTime.Now);
                mpRevizeForm.kayitTarihi = new TNSDateTime(DateTime.Now);
                mpRevizeForm.kaydedenKullaniciKodu = kullanan.kullaniciKodu;
                mpRevizeForm.formDurum = BKD.ENUMRevizeFormDurum.Taslak;
                mpRevizeForm.evrakTarih = new TNSDateTime(DateTime.Now);

                if (form.DortDuzeyDegisim == 1)
                    hata = DegisimdenMPRUret(kullanan, ve, form, mpRevizeForm, OdenekToplam, mpDurum);
                else if (form.belgeTur == ENUMBelgeTuru.ODENEKGONDERME)
                    hata = GondermedenMPRUret(kullanan, ve, form, revizeIslemSatirlar, mpRevizeForm, mpDurum, donemOdenegi, debug, sb);
            }

            if (hata != "")
                sonuc.DegerAta(hata);

            return sonuc;
        }

        private bool IlUyumluMu(int ilKod, int muhasebe)
        {
            return (muhasebe / 1000 == ilKod);
        }

        private static string KurYapiBul(string muhasebe, string birim, string kurKod, out string hataStr, BHMv2.IBHMv2Servis bhmServis)
        {
            hataStr = "";
            string[] kyKodlar = bhmServis.KurumsalYapiListele(muhasebe, birim, kurKod);
            if (kyKodlar.Length == 1)
            {
                return kyKodlar[0];
            }
            else
            {
                hataStr = "<br>" + string.Format(ButceMuhasebeResI.MUH198, muhasebe, birim, kurKod);
                return "";
            }
        }

        private void MPRevizeEklenenTertiptenDus(BKD.MPRevizeSatir rs, BKD.MPRevizeSatir rsD, double eklenenMiktar, int baslaAy)
        {
            if (baslaAy > 12) return;
            double[] tertipRakamlar = bhmServis.MasrafPlaniOdenekBilgiBul(rs.tertip);

            for (int ay = baslaAy - 1; eklenenMiktar != 0 && ay < 12; ay++)
            {
                double ayRakam = tertipRakamlar[6 + ay];
                if (ayRakam > 0)
                {
                    if (eklenenMiktar > ayRakam)
                    {
                        rsD.odenek[ay] = ayRakam;
                        eklenenMiktar = eklenenMiktar - ayRakam;
                    }
                    else
                    {
                        rsD.odenek[ay] = eklenenMiktar;
                        eklenenMiktar = 0;
                    }
                }
            }
        }

        public static double[] MasrafPlaniOdenekBilgiBul(VeriErisim ve, Tertip kistasTertip, bool onaysiz, int haricRevizeNo, bool harcamaVar = true, bool blokeVar = true)
        {
            double[] donusDeger = new double[18];
            BHMv2.IsKurallari.Veri.TABMPGMASRAFPLANI tabMP = new BHMv2.IsKurallari.Veri.TABMPGMASRAFPLANI(ve);

            int kistasAy = TNS.BHMv2.Arac.SerbestAyBul(kistasTertip.yil);

            IDataReader dr = tabMP.ListeleRapor("AY", kistasTertip, BHMv2.ENUMMasrafPlanAsama.SONDURUMONAYLI, -1, haricRevizeNo);
            donusDeger[0] = 0;
            donusDeger[1] = 0;
            if (dr != null)
            {
                while (dr.Read())
                {
                    int ay = VeriErisim.DegerAl(dr, 0, (int)0);
                    double deger = VeriErisim.DegerAl(dr, 1, (double)0);
                    if (ay >= 1 && ay <= 12)
                    {
                        donusDeger[5 + ay] = deger;
                        donusDeger[0] += deger;
                        if (ay <= kistasAy)
                            donusDeger[1] += deger;
                    }
                }
                dr.Close();
            }
            else
            {
                throw new Exception(ve.hata);
            }

            IUZYServis uzyServis = TNS.UZY.Arac.Tanimla();
            EkDegerParam ep;
            if (harcamaVar)
            {
                ep = new EkDegerParam(ENUMTabloKod.TABPEBHarcama, 0, 0, "");
                donusDeger[2] = ep.EkDegerHesaplaDbl(uzyServis, null, kistasTertip.yil, kistasTertip.TertipStrOl());
            }

            if (blokeVar)
            {
                ep = new EkDegerParam(ENUMTabloKod.TABHYBloke, 0, 0, "");
                donusDeger[3] = ep.EkDegerHesaplaDbl(uzyServis, null, kistasTertip.yil, kistasTertip.TertipStrOl());
            }
            donusDeger[4] = donusDeger[0] - donusDeger[2] - donusDeger[3];
            donusDeger[5] = donusDeger[1] - donusDeger[2];

            return donusDeger;
        }

        private static string MasrafPlaninaDagit(VeriErisim ve, BKD.MPRevizeSatir rs, double dusulen, out double artan, int sonAy, bool onaysiz, bool debug, StringBuilder sb)
        {
            artan = OrtakFonksiyonlar.KurusYap(Math.Abs(dusulen));
            double[] tutuluRakamlar = MasrafPlaniOdenekBilgiBul(ve, rs.tertip, onaysiz, 0, false, false);
            int ay = 0;
            for (; artan != 0 && ay < sonAy; ay++)
            {
                double ayRakam = tutuluRakamlar[6 + ay];
                if (ayRakam < 0)
                    return string.Format("{0} tertibinde {1}. ayın rakamı hatalı olduğundan masraf planı dağıtımı yapılamadı!", rs.tertip, ay + 1);
                if (artan <= ayRakam)
                {
                    rs.odenek[ay] = 0 - artan;
                    artan = 0;
                }
                else if (ayRakam > 0)
                {
                    rs.odenek[ay] = 0 - ayRakam;
                    artan = OrtakFonksiyonlar.KurusYap(artan - ayRakam);
                }
            }
            for (; ay < 12; ay++)
            {
                double ayRakam = tutuluRakamlar[6 + ay];
                if (ayRakam < 0)
                    return string.Format("{0} tertibinde {1}. ayın rakamı hatalı olduğundan masraf planı dağıtımı yapılamadı!", rs.tertip, ay + 1);
            }
            return "";
        }

        private string DegisimdenMPRUret(Kullanici kullanan, VeriErisim ve, GondermeTenkisForm form, BKD.MPRevizeForm mpRevizeForm, Hashtable OdenekToplam, int mpDurum)
        {
            if (mpDurum != 5) return "";
            mpRevizeForm.revizeFormTip = BKD.ENUMRevizeFormTip.MasrafPlaniAktarma;
            string hataStr;
            double kalan = 0;
            ArrayList al = new ArrayList();
            foreach (System.Collections.DictionaryEntry de in OdenekToplam)
            {
                double[] toplamDusen = new double[12];
                Tertip ikiDuzeyTertip = new Tertip(mpRevizeForm.yil, ENUMTabloKod.TABMasrafPlani);
                ikiDuzeyTertip.TertipAta((string)de.Key);
                string kyKod = KurYapiBul(form.muhasebe.ToString(), form.birim.ToString(), ikiDuzeyTertip.kodlar[0], out hataStr, bhmServis);
                if (!string.IsNullOrEmpty(hataStr)) return hataStr;
                foreach (GondermeTenkisDetay detay in form.detay.objeler)
                {
                    if (detay.miktar < 0)
                    {
                        BKD.MPRevizeSatir rsD = new BKD.MPRevizeSatir();
                        rsD.tertip = new Tertip(detay.tertip.yil, ENUMTabloKod.TABMasrafPlani, detay.tertip.kodlar[0], detay.tertip.kodlar[1], detay.tertip.kodlar[2], detay.tertip.kodlar[3], detay.tertip.kodlar[4], kyKod);
                        if (rsD.tertip.Kiyasla(ikiDuzeyTertip))
                        {
                            string hata = MasrafPlaninaDagit(ve, rsD, detay.miktar, out kalan, 12, false, false, null);
                            if (!string.IsNullOrWhiteSpace(hata)) return hata;
                            MPDagilimiToplamaEkle(rsD, toplamDusen);
                            al.Add(rsD);
                        }
                    }
                }
                foreach (GondermeTenkisDetay detay in form.detay.objeler)
                {
                    if (detay.miktar > 0)
                    {
                        BKD.MPRevizeSatir rsD = new BKD.MPRevizeSatir();
                        rsD.tertip = new Tertip(detay.tertip.yil, ENUMTabloKod.TABMasrafPlani, detay.tertip.kodlar[0], detay.tertip.kodlar[1], detay.tertip.kodlar[2], detay.tertip.kodlar[3], detay.tertip.kodlar[4], kyKod);
                        if (rsD.tertip.Kiyasla(ikiDuzeyTertip))
                        {
                            ToplamDizidenMPSatiraAktar(rsD, detay.miktar, toplamDusen);
                            al.Add(rsD);
                        }
                    }
                }
            }
            mpRevizeForm.satirlar = (BKD.MPRevizeSatir[])al.ToArray(typeof(BKD.MPRevizeSatir));
            return "";
        }

        private void ToplamDizidenMPSatiraAktar(BKD.MPRevizeSatir rsD, double eklenen, double[] toplamDusen)
        {
            for (int i = 0; i < rsD.odenek.Length && eklenen > 0; i++)
            {
                if (eklenen + toplamDusen[i] <= 0)
                {
                    rsD.odenek[i] = eklenen;
                    toplamDusen[i] = toplamDusen[i] + eklenen;
                    eklenen = 0;
                }
                else
                {
                    rsD.odenek[i] = 0 - toplamDusen[i];
                    toplamDusen[i] = 0;
                    eklenen = eklenen - rsD.odenek[i];
                }
            }
        }

        private void MPDagilimiToplamaEkle(BKD.MPRevizeSatir rsD, double[] toplamDusen)
        {
            for (int i = 0; i < toplamDusen.Length; i++)
            {
                toplamDusen[i] += rsD.odenek[i];
            }
        }

        private string GondermedenMPRUret(Kullanici kullanan, VeriErisim ve, GondermeTenkisForm form, ArrayList revizeIslemSatirlar, BKD.MPRevizeForm mpRevizeForm, int mpDurum, bool donemOdenegi, bool debug, StringBuilder sb)
        {
            if (mpDurum != 5)
                return "";
            string hata = "";
            int sonAy = TNS.BHMv2.Arac.SerbestAyBul(form.yil);

            int revizeTertipSay = revizeIslemSatirlar.Count;
            for (int i = 0; i < revizeTertipSay; i++)
            {
                TNS.BKD.MPRevizeSatir rs = (TNS.BKD.MPRevizeSatir)revizeIslemSatirlar[i];
                double eklenecek = rs.odenek[0];
                if (Math.Round(eklenecek, 4) > 0 && donemOdenegi)
                    hata = String.Format("Dönem ödeneğini geçecek şekilde ödenek gönderiyorsunuz! (tertip:{0}, aşan miktar:{1})", rs.tertip.TertipStrOl(), eklenecek.ToString("#,##0.00"));
                else
                {
                    TNS.BKD.MPRevizeSatir rsD = new BKD.MPRevizeSatir();
                    rsD.tertip = new Tertip(rs.tertip.yil, rs.tertip.tabloKod, rs.tertip.kodlar[0], rs.tertip.kodlar[1], rs.tertip.kodlar[2], rs.tertip.kodlar[3], rs.tertip.kodlar[4], "9");
                    double artan;
                    hata += MasrafPlaninaDagit(ve, rsD, eklenecek, out artan, sonAy, true, debug, sb);
                    if (OrtakFonksiyonlar.KurusYap(artan) != 0)
                        hata += "<br>" + string.Format(ButceMuhasebeResI.MUH041, rs.tertip.TertipStrOl(), artan.ToString("#,##0"));

                    if (donemOdenegi)
                        MPRevizeEklenenTertiptenDus(rs, rsD, eklenecek, sonAy + 1);
                    for (int ay = 0; ay < 12; ay++)
                    {
                        rs.odenek[ay] = 0 - rsD.odenek[ay];
                    }
                    revizeIslemSatirlar.Add(rsD);
                }
            }
            mpRevizeForm.satirlar = (BKD.MPRevizeSatir[])revizeIslemSatirlar.ToArray(typeof(BKD.MPRevizeSatir));
            return hata;
        }

        private string OdenekTenkisSatirKontrolPebHT(VeriErisim ve, Kullanici kullanan, GondermeTenkisForm form, System.Globalization.NumberFormatInfo n, Hashtable ht)
        {
            string hata = "";
            foreach (DictionaryEntry de in ht)
            {
                string tertip = (string)de.Key;
                double detayMiktar = (double)de.Value;

                if (form.belgeTur == ENUMBelgeTuru.ODENEKGONDERME)
                {
                    if (form.yil <= 2020)
                    {
                        double serbest = MUHServis.UzaySayiDegerGetir(ve, kullanan, "ABSBUTCEGIDER", form.yil + "-" + tertip, false, "SUM(SERBEST)");
                        string sorguGON = form.yil + "-" + (int)ENUMBelgeTuru.ODENEKGONDERME + "-" + "-" + "-" + "-" + "-" + "-" + "-" + tertip;
                        string sorguTEN = form.yil + "-" + (int)ENUMBelgeTuru.TENKIS + "-" + "-" + "-" + "-" + "-" + "-" + (int)ENUMBelgeDurumu.ONAYLI + "-" + tertip;
                        double toplamGON = MUHServis.UzaySayiDegerGetir(ve, kullanan, "MUHGONDERMEBELGESITENKIS", sorguGON, false, "SUM(MIKTAR)");
                        double toplamTEN = Math.Abs(MUHServis.UzaySayiDegerGetir(ve, kullanan, "MUHGONDERMEBELGESITENKIS", sorguTEN, false, "SUM(MIKTAR)"));
                        double belgeGON = 0;
                        if (form.belgeNo.Trim() != "")
                        {
                            sorguGON = form.yil + "-" + (int)ENUMBelgeTuru.ODENEKGONDERME + "-" + form.belgeNo + "-" + "-" + "-" + "-" + "-" + "-" + tertip;
                            belgeGON = MUHServis.UzaySayiDegerGetir(ve, kullanan, "MUHGONDERMEBELGESITENKIS", sorguGON, false, "SUM(MIKTAR)");
                        }

                        toplamGON -= belgeGON;
                        double kullanabilir = serbest - (toplamGON - toplamTEN);

                        if (serbest < detayMiktar - 0.01)
                            hata += "<br>" + string.Format(ButceMuhasebeResI.MUH195, tertip, detayMiktar.ToString("N", n), serbest.ToString("N", n));

                        if (kullanabilir < detayMiktar - 0.01)
                            hata += "<br>" + string.Format(ButceMuhasebeResI.MUH196, tertip, detayMiktar.ToString("N", n), kullanabilir.ToString("N", n));
                    }
                }
                else
                {
                    /**************************************/
                    //double odenek = 0;
                    //double serbest = 0;
                    double toplamGON = 0;
                    double toplamTEN = 0;
                    //double belgeGON = 0;
                    double belgeTEN = 0;
                    string sorguGON = "";
                    string sorguTEN = "";
                    double harcama = 0;
                    double avans = 0;

                    harcama = MUHServis.UzaySayiDegerGetir(ve, kullanan, "ISLHARCAMAMUHASEBE", form.yil.ToString() + "-" + DateTime.Now.Month.ToString() + "-" + form.muhasebe + "-" + form.birim + "-" + tertip, false, "");
                    avans = MUHServis.UzaySayiDegerGetir(ve, kullanan, "MUHAVANS", form.yil.ToString() + "-" + form.muhasebe + "-" + form.birim + "-" + tertip + "-" + "5,6", false, "");
                    if (avans < 0) avans = 0;
                    //avans += MUHServis.UzaySayiDegerGetir(kullanan, "MUHAVANS", form.yil.ToString() + "-" + form.muhasebe + "-" + form.birim + "-" + tertip + "-" + "6", true, "");
                    harcama += avans;

                    //odenek = MUHServis.UzaySayiDegerGetir(kullanan, "ABSBUTCEGIDER", form.yil + "-" + kur + "-" + fon + "-" + fin + "-" + eko, true, "KBO+EKLENEN-DUSULEN");

                    //YIL+"-"+BELGETUR+"-"+BELGENO+"-"+KURUM+"-"+MUHASEBEKOD+bırımkod"-"+AY+"-"+DURUM+"-"+KURKOD+"-"+FONKOD+"-"+FINKOD+"-"+EKOKOD
                    sorguGON = form.yil + "-" + (int)ENUMBelgeTuru.ODENEKGONDERME + "-" + "-" + "-" + form.muhasebe + "-" + form.birim + "-" + "-" + (int)ENUMBelgeDurumu.ONAYLI + "-" + tertip;
                    sorguTEN = form.yil + "-" + (int)ENUMBelgeTuru.TENKIS + "-" + "-" + "-" + form.muhasebe + "-" + form.birim + "-" + "-" + "-" + tertip;
                    toplamGON = MUHServis.UzaySayiDegerGetir(ve, kullanan, "MUHGONDERMEBELGESITENKIS", sorguGON, false, "SUM(MIKTAR)");
                    toplamTEN = Math.Abs(MUHServis.UzaySayiDegerGetir(ve, kullanan, "MUHGONDERMEBELGESITENKIS", sorguTEN, false, "SUM(MIKTAR)"));

                    if (form.belgeNo.Trim() != "")
                    {
                        sorguTEN = form.yil + "-" + (int)ENUMBelgeTuru.TENKIS + "-" + form.belgeNo + "-" + "-" + "-" + "-" + "-" + "-" + tertip;
                        belgeTEN = Math.Abs(MUHServis.UzaySayiDegerGetir(ve, kullanan, "MUHGONDERMEBELGESITENKIS", sorguTEN, false, "SUM(MIKTAR)"));
                        toplamTEN -= belgeTEN;
                    }

                    double kullanabilir = toplamGON - toplamTEN - harcama;

                    if (kullanabilir < detayMiktar - 0.01)
                    {
                        hata += "<br>" + string.Format("{0} tertibi için, verilen birimdeki ({1}-{2}), tenkis edilebilir miktar {3} iken, daha fazla ({4})tenkis edilmeye çalışılıyor!", tertip, form.muhasebe, form.birim, kullanabilir.ToString("N", n), detayMiktar.ToString("N", n) + "; toplamGON:" + toplamGON.ToString("#,##0") + "; toplamTEN:" + toplamTEN.ToString("#,##0") + ";  harcama:" + harcama.ToString("#,##0"));
                    }
                    /****************************************/
                }
            }
            return hata;
        }

        private string OdenekTenkisSatirKontrolPeb(VeriErisim ve, Kullanici kullanan, GondermeTenkisForm form, GondermeTenkisDetay detay, int satir, System.Globalization.NumberFormatInfo n, string kurumsalYapiKod, Hashtable ht, ArrayList revizeIslemSatirlar, int mpDurum, bool donemOdenegi, int eko2Uz, int eko3Uz)
        {
            string hata = "";

            string kur = detay.tertip.kodlar[0];
            string fon = detay.tertip.kodlar[1];
            string fin = detay.tertip.kodlar[2];
            string eko = detay.tertip.kodlar[3];
            string peb = detay.tertip.kodlar[4];

            string noktasizEko = eko.Replace(".", "");
            int ekoParacAl;
            if (TNS.BKD.Arac.Eko23Mu(form.yil, noktasizEko))
                ekoParacAl = eko3Uz;
            else
                ekoParacAl = eko2Uz;
            string eko2_3 = eko.Replace(".", "").Substring(0, ekoParacAl);

            string htStr = kur + "-" + fon + "-" + fin + "-" + eko2_3;
            double onc2odenek;
            try
            {
                onc2odenek = (double)ht[htStr];
            }
            catch
            {
                onc2odenek = 0;
            }

            ht[htStr] = onc2odenek + detay.miktar;

            if (form.belgeTur == ENUMBelgeTuru.ODENEKGONDERME)
            {
                if (string.IsNullOrEmpty(form.talep))
                {
                    if (mpDurum == 5)
                    {
                        if (revizeIslemSatirlar != null)
                        {
                            double belgeGON = 0;
                            string sorguGON = "";

                            if (form.belgeNo.Trim() != "")
                            {
                                sorguGON = form.yil + "-" + (int)ENUMBelgeTuru.ODENEKGONDERME + "-" + form.belgeNo + "-" + "-" + "-" + "-" + "-" + "-" + kur + "-" + fon + "-" + fin + "-" + eko + "-" + peb;
                                belgeGON = MUHServis.UzaySayiDegerGetir(ve, kullanan, "MUHGONDERMEBELGESITENKIS", sorguGON, false, "SUM(MIKTAR)");
                            }
                            else
                                belgeGON = 0;

                            string sorguGONBirim = form.yil + "-" + (int)ENUMBelgeTuru.ODENEKGONDERME + "-" + "-" + "-" + form.muhasebe.ToString() + "-" + form.birim.ToString() + "-" + "-" + "-" + kur + "-" + fon + "-" + fin + "-" + eko + "-" + peb;
                            string sorguTENBirim = form.yil + "-" + (int)ENUMBelgeTuru.TENKIS + "-" + "-" + "-" + form.muhasebe.ToString() + "-" + form.birim.ToString() + "-" + "-" + (int)ENUMBelgeDurumu.ONAYLI + "-" + kur + "-" + fon + "-" + fin + "-" + eko + "-" + peb;
                            double toplamGONBirim = MUHServis.UzaySayiDegerGetir(ve, kullanan, "MUHGONDERMEBELGESITENKIS", sorguGONBirim, false, "SUM(MIKTAR)");
                            double toplamTENBirim = Math.Abs(MUHServis.UzaySayiDegerGetir(ve, kullanan, "MUHGONDERMEBELGESITENKIS", sorguTENBirim, false, "SUM(MIKTAR)"));
                            toplamGONBirim = toplamGONBirim - toplamTENBirim - belgeGON; double ekle = 0;
                            if (donemOdenegi)
                            {
                                int sonAy = TNS.BHMv2.Arac.SerbestAyBul(form.yil);
                                string ayKosul = "0/" + (sonAy + 1).ToString();
                                string uzayKosul = form.yil + "-" + "5" + "---" + kur + "-" + fon + "-" + fin + "-" + eko + "-" + peb + "-" + kurumsalYapiKod + "-" + ayKosul + "-" + "";
                                double donemMP = MUHServis.UzaySayiDegerGetir(ve, kullanan, "MPGMASRAFPLANIBIRIM", uzayKosul, false, "");
                                if (detay.miktar + toplamGONBirim > donemMP)
                                {
                                    ekle = (detay.miktar + toplamGONBirim) - donemMP;
                                }
                            }
                            else
                                ekle = detay.miktar;
                            if (ekle > 0)
                            {
                                BKD.MPRevizeSatir rs = new BKD.MPRevizeSatir();
                                rs.tertip = new Tertip(detay.tertip.yil, ENUMTabloKod.TABMasrafPlani, kur, fon, fin, eko, peb, kurumsalYapiKod);
                                rs.odenek[0] = ekle;
                                revizeIslemSatirlar.Add(rs);
                            }
                        }
                    }
                    else
                    {
                        double serbest = MUHServis.UzaySayiDegerGetir(ve, kullanan, "ABSBUTCEGIDER", form.yil + "-" + kur + "-" + fon + "-" + fin + "-" + eko + "-" + peb, false, "SUM(SERBEST)");
                        string sorguGON = form.yil + "-" + (int)ENUMBelgeTuru.ODENEKGONDERME + "-" + "-" + "-" + "-" + "-" + "-" + "-" + kur + "-" + fon + "-" + fin + "-" + eko + "-" + peb;
                        string sorguTEN = form.yil + "-" + (int)ENUMBelgeTuru.TENKIS + "-" + "-" + "-" + "-" + "-" + "-" + (int)ENUMBelgeDurumu.ONAYLI + "-" + kur + "-" + fon + "-" + fin + "-" + eko + "-" + peb;
                        double toplamGON = MUHServis.UzaySayiDegerGetir(ve, kullanan, "MUHGONDERMEBELGESITENKIS", sorguGON, true, "SUM(MIKTAR)");
                        double toplamTEN = Math.Abs(MUHServis.UzaySayiDegerGetir(ve, kullanan, "MUHGONDERMEBELGESITENKIS", sorguTEN, true, "SUM(MIKTAR)"));
                        if (form.belgeNo.Trim() != "")
                        {
                            sorguGON = form.yil + "-" + (int)ENUMBelgeTuru.ODENEKGONDERME + "-" + form.belgeNo + "-" + "-" + "-" + "-" + "-" + "-" + kur + "-" + fon + "-" + fin + "-" + eko + "-" + peb;
                            sorguTEN = form.yil + "-" + (int)ENUMBelgeTuru.TENKIS + "-" + form.belgeNo + "-" + "-" + "-" + "-" + "-" + "-" + kur + "-" + fon + "-" + fin + "-" + eko + "-" + peb;
                            double belgeGON = MUHServis.UzaySayiDegerGetir(ve, kullanan, "MUHGONDERMEBELGESITENKIS", sorguGON, true, "SUM(MIKTAR)");
                            toplamGON -= belgeGON;
                        }

                        double kullanabilir = serbest - (toplamGON - toplamTEN);
                        if (kullanabilir < detay.miktar - 0.01)
                            hata += "<br>" + string.Format(ButceMuhasebeResI.MUH045, satir.ToString(), detay.miktar.ToString("N", n), kur, fon, fin, eko, kullanabilir.ToString("N", n));
                    }
                }
            }
            else
            {
                #region Tenkis kontrol

                /**************************************/
                double toplamGON = 0;
                double toplamTEN = 0;
                double belgeTEN = 0;
                string sorguGON = "";
                string sorguTEN = "";
                double harcama = 0;
                string tertip = kur + "-" + fon + "-" + fin + "-" + eko;

                harcama = MUHServis.UzaySayiDegerGetir(ve, kullanan, "MUHODEMEEMRIMIF", form.yil.ToString() + "-" + form.muhasebe + "-" + form.birim + "-830,160,161,164,165,166,167-" + tertip + "--" + peb, false, "");

                sorguGON = form.yil + "-" + (int)ENUMBelgeTuru.ODENEKGONDERME + "-" + "-" + "-" + form.muhasebe + "-" + form.birim + "-" + "-" + (int)ENUMBelgeDurumu.ONAYLI + "-" + tertip + "-" + peb;
                sorguTEN = form.yil + "-" + (int)ENUMBelgeTuru.TENKIS + "-" + "-" + "-" + form.muhasebe + "-" + form.birim + "-" + "-" + "-" + tertip + "-" + peb;

                toplamGON = MUHServis.UzaySayiDegerGetir(ve, kullanan, "MUHGONDERMEBELGESITENKIS", sorguGON, false, "SUM(MIKTAR)");
                toplamTEN = Math.Abs(MUHServis.UzaySayiDegerGetir(ve, kullanan, "MUHGONDERMEBELGESITENKIS", sorguTEN, false, "SUM(MIKTAR)"));

                if (form.belgeNo.Trim() != "" && string.IsNullOrEmpty(form.talep))
                {
                    sorguTEN = form.yil + "-" + (int)ENUMBelgeTuru.TENKIS + "-" + form.belgeNo + "-" + "-" + "-" + "-" + "-" + "-" + tertip + "-" + peb;
                    belgeTEN = Math.Abs(MUHServis.UzaySayiDegerGetir(ve, kullanan, "MUHGONDERMEBELGESITENKIS", sorguTEN, false, "SUM(MIKTAR)"));
                    toplamTEN -= belgeTEN;
                }

                double kullanabilir = toplamGON - toplamTEN - harcama;

                if (kullanabilir < detay.miktar - 0.01)
                {
                    hata += "<br>" + string.Format("{0} tertibi için, verilen birimdeki ({1}-{2}), tenkis edilebilir miktar {3} iken, daha fazla ({4})tenkis edilmeye çalışılıyor!", tertip + "-" + peb, form.muhasebe, form.birim, kullanabilir.ToString("N", n), detay.miktar.ToString("N", n) + "; toplamGON:" + toplamGON.ToString("#,##0") + "; toplamTEN:" + toplamTEN.ToString("#,##0") + ";  harcama+avans:" + harcama.ToString("#,##0"));
                }

                #endregion
            }
            return hata;
        }

        private string OdenekTenkisSatirKontrolDegisim(VeriErisim ve, Kullanici kullanan, GondermeTenkisForm form, GondermeTenkisDetay detay, int satir, System.Globalization.NumberFormatInfo n, System.Collections.Hashtable OdenekToplam, int eko2uz, int eko3uz)
        {
            string kur = detay.tertip.kodlar[0];
            string fon = detay.tertip.kodlar[1];
            string fin = detay.tertip.kodlar[2];
            string eko = detay.tertip.kodlar[3];
            string peb = TNS.MUH.Arac.DiziElemanAl(detay.tertip.kodlar, 4, "");

            double toplamGON = 0;
            double toplamTEN = 0;
            double belgeGON = 0;
            string sorguGON = "";
            string sorguTEN = "";
            double harcama = 0;
            string hata = "";

            string noktasizEko = eko.Replace(".", "");

            string ikiDuzeyTertip = kur + "-" + fon + "-" + fin + "-";
            if (TNS.BKD.Arac.Eko23Mu(form.yil, noktasizEko))
                ikiDuzeyTertip += noktasizEko.Substring(0, eko3uz);
            else
                ikiDuzeyTertip += noktasizEko.Substring(0, eko2uz);
            double tut = 0;
            if (OdenekToplam.ContainsKey(ikiDuzeyTertip))
            {
                tut = (double)OdenekToplam[ikiDuzeyTertip];
            }
            OdenekToplam[ikiDuzeyTertip] = Math.Round(tut + detay.miktar, 2);

            if (detay.miktar < 0)
            {
                harcama = MUHServis.UzaySayiDegerGetir(ve, kullanan, "MUHODEMEEMRIMIF", form.yil + "-" + form.muhasebe + "-" + form.birim + "-" + "830" + "-" + kur + "-" + fon + "-" + fin + "-" + eko + "-" + "0/6" + "-" + peb, false, "");
                sorguGON = form.yil + "-" + (int)ENUMBelgeTuru.ODENEKGONDERME + "-" + "-" + "-" + "-" + "-" + "-" + "-" + kur + "-" + fon + "-" + fin + "-" + eko + "-" + peb;
                sorguTEN = form.yil + "-" + (int)ENUMBelgeTuru.TENKIS + "-" + "-" + "-" + "-" + "-" + "-" + (int)ENUMBelgeDurumu.ONAYLI + "-" + kur + "-" + fon + "-" + fin + "-" + eko + "-" + peb;
                toplamGON = MUHServis.UzaySayiDegerGetir(ve, kullanan, "MUHGONDERMEBELGESITENKIS", sorguGON, false, "SUM(MIKTAR)");
                toplamTEN = Math.Abs(MUHServis.UzaySayiDegerGetir(ve, kullanan, "MUHGONDERMEBELGESITENKIS", sorguTEN, false, "SUM(MIKTAR)"));

                if (form.belgeNo.Trim() != "")
                {
                    sorguGON = form.yil + "-" + (int)ENUMBelgeTuru.ODENEKGONDERME + "-" + form.belgeNo + "-" + "-" + "-" + "-" + "-" + "-" + kur + "-" + fon + "-" + fin + "-" + eko + "-" + peb;
                    //sorguTEN = form.yil + "-" + (int)ENUMBelgeTuru.TENKIS + "-" + form.belgeNo + "-" + "-" + "-" + "-" + "-" + "-" + kur + "-" + fon + "-" + fin + "-" + eko + "-" + peb;
                    belgeGON = MUHServis.UzaySayiDegerGetir(ve, kullanan, "MUHGONDERMEBELGESITENKIS", sorguGON, false, "SUM(MIKTAR)");
                    toplamGON -= belgeGON;
                }

                double kullanabilir = toplamGON - toplamTEN - harcama;
                if (kullanabilir + detay.miktar < 0.00)
                    hata += "<br>" + string.Format(ButceMuhasebeResI.MUH045, satir.ToString(), detay.miktar.ToString("N", n), kur, fon, fin, eko, kullanabilir.ToString("N", n));
            }
            return hata;
        }

        private string OdenekTenkisSatirKontrolEko2(VeriErisim ve, Kullanici kullanan, GondermeTenkisForm form, GondermeTenkisDetay detay, int satir, System.Globalization.NumberFormatInfo n)
        {
            if (form.yil >= 2021)
                return "";

            string kur = detay.tertip.kodlar[0];
            string fon = detay.tertip.kodlar[1];
            string fin = detay.tertip.kodlar[2];
            string eko = detay.tertip.kodlar[3];

            //double odenek = 0;
            double serbest = 0;
            double toplamGON = 0;
            double toplamTEN = 0;
            double belgeGON = 0;
            double belgeTEN = 0;
            string sorguGON = "";
            string sorguTEN = "";
            double harcama = 0;
            double avans = 0;
            string hata = "";

            serbest = MUHServis.UzaySayiDegerGetir(ve, kullanan, "ABSBUTCEGIDER", form.yil + "-" + kur + "-" + fon + "-" + fin + "-" + eko, false, "SUM(SERBEST)");
            harcama = MUHServis.UzaySayiDegerGetir(ve, kullanan, "ISLHARCAMAMUHASEBE", form.yil.ToString() + "-" + DateTime.Now.Month.ToString() + "-" + form.muhasebe + "-" + form.birim + "-" + kur + "-" + fon + "-" + fin + "-" + eko, true, "");
            avans = MUHServis.UzaySayiDegerGetir(ve, kullanan, "MUHAVANS", form.yil.ToString() + "-" + form.muhasebe + "-" + form.birim + "-" + kur + "-" + fon + "-" + fin + "-" + eko + "-" + "5", true, "");
            avans += MUHServis.UzaySayiDegerGetir(ve, kullanan, "MUHAVANS", form.yil.ToString() + "-" + form.muhasebe + "-" + form.birim + "-" + kur + "-" + fon + "-" + fin + "-" + eko + "-" + "6", true, "");
            harcama += avans;

            //if (form.belgeTur == ENUMBelgeTuru.ODENEKGONDERME)
            //    odenek = MUHServis.UzaySayiDegerGetir(ve, kullanan, "ABSBUTCEGIDER", form.yil + "-" + kur + "-" + fon + "-" + fin + "-" + eko, false, "SUM(KBO+EKLENEN-DUSULEN-DUSULEN_TASLAK)");
            //else
            //    odenek = MUHServis.UzaySayiDegerGetir(ve, kullanan, "ABSBUTCEGIDER", form.yil + "-" + kur + "-" + fon + "-" + fin + "-" + eko, false, "SUM(KBO+EKLENEN-DUSULEN)");

            //YIL+"-"+BELGETUR+"-"+BELGENO+"-"+KURUM+"-"+MUHASEBEKOD+bırımkod"-"+AY+"-"+DURUM+"-"+KURKOD+"-"+FONKOD+"-"+FINKOD+"-"+EKOKOD
            if (form.belgeTur == ENUMBelgeTuru.ODENEKGONDERME)
            {
                sorguGON = form.yil + "-" + (int)ENUMBelgeTuru.ODENEKGONDERME + "-" + "-" + "-" + "-" + "-" + "-" + "-" + kur + "-" + fon + "-" + fin + "-" + eko;
                sorguTEN = form.yil + "-" + (int)ENUMBelgeTuru.TENKIS + "-" + "-" + "-" + "-" + "-" + "-" + (int)ENUMBelgeDurumu.ONAYLI + "-" + kur + "-" + fon + "-" + fin + "-" + eko;
            }
            else
            {
                sorguGON = form.yil + "-" + (int)ENUMBelgeTuru.ODENEKGONDERME + "-" + "-" + "-" + form.muhasebe + "-" + form.birim + "-" + "-" + (int)ENUMBelgeDurumu.ONAYLI + "-" + kur + "-" + fon + "-" + fin + "-" + eko;
                sorguTEN = form.yil + "-" + (int)ENUMBelgeTuru.TENKIS + "-" + "-" + "-" + form.muhasebe + "-" + form.birim + "-" + "-" + "-" + kur + "-" + fon + "-" + fin + "-" + eko;
            }

            toplamGON = MUHServis.UzaySayiDegerGetir(ve, kullanan, "MUHGONDERMEBELGESITENKIS", sorguGON, true, "SUM(MIKTAR)");
            toplamTEN = Math.Abs(MUHServis.UzaySayiDegerGetir(ve, kullanan, "MUHGONDERMEBELGESITENKIS", sorguTEN, true, "SUM(MIKTAR)"));

            if (form.belgeNo.Trim() != "")
            {
                sorguGON = form.yil + "-" + (int)ENUMBelgeTuru.ODENEKGONDERME + "-" + form.belgeNo + "-" + "-" + "-" + "-" + "-" + "-" + kur + "-" + fon + "-" + fin + "-" + eko;
                sorguTEN = form.yil + "-" + (int)ENUMBelgeTuru.TENKIS + "-" + form.belgeNo + "-" + "-" + "-" + "-" + "-" + "-" + kur + "-" + fon + "-" + fin + "-" + eko;

                if (form.belgeTur == ENUMBelgeTuru.ODENEKGONDERME)
                {
                    belgeGON = MUHServis.UzaySayiDegerGetir(ve, kullanan, "MUHGONDERMEBELGESITENKIS", sorguGON, true, "SUM(MIKTAR)");
                    toplamGON -= belgeGON;
                }
                else if (form.belgeTur == ENUMBelgeTuru.TENKIS)
                {
                    belgeTEN = Math.Abs(MUHServis.UzaySayiDegerGetir(ve, kullanan, "MUHGONDERMEBELGESITENKIS", sorguTEN, true, "SUM(MIKTAR)"));
                    toplamTEN -= belgeTEN;
                }
            }

            double kullanabilir = 0;
            if (form.belgeTur == ENUMBelgeTuru.ODENEKGONDERME)
                kullanabilir = serbest - (toplamGON - toplamTEN);

            if (form.belgeTur == ENUMBelgeTuru.TENKIS)
                kullanabilir = toplamGON - toplamTEN - harcama;

            if (form.belgeTur == ENUMBelgeTuru.TENKIS)
            {
                toplamGON -= harcama;

                if (toplamGON == 0)
                    hata += "<br>" + string.Format(ButceMuhasebeResI.MUH042, satir.ToString(), kur, fon, fin, eko, form.muhasebe);
                else if (OrtakFonksiyonlar.ConvertToDbl((toplamGON - toplamTEN).ToString("N", n)) < OrtakFonksiyonlar.ConvertToDbl(detay.miktar.ToString("N", n)))
                    hata += "<br>" + string.Format(ButceMuhasebeResI.MUH043, satir.ToString(), detay.miktar.ToString("N", n), form.muhasebe, (toplamGON - toplamTEN).ToString("N", n));

                if (toplamGON < toplamTEN)
                    hata += "<br>" + string.Format(ButceMuhasebeResI.MUH046, satir.ToString(), kur, fon, fin, eko, form.muhasebe, toplamGON.ToString("N", n));
            }
            else if (serbest < detay.miktar - 0.01)
                hata += "<br>" + string.Format(ButceMuhasebeResI.MUH040, satir.ToString(), detay.miktar.ToString("N", n), kur, fon, fin, eko, serbest.ToString("N", n));

            if (kullanabilir < detay.miktar - 0.01)
                hata += "<br>" + string.Format(ButceMuhasebeResI.MUH045, satir.ToString(), detay.miktar.ToString("N", n), kur, fon, fin, eko, kullanabilir.ToString("N", n));
            return hata;
        }

        /// <summary>
        /// Kurumsa PEB kullanılıyor mu kontrolü
        /// </summary>
        /// <param name="yil">Yıl bilgisi</param>
        /// <returns></returns>
        public static bool PEBTertipMi(int yil)
        {
            return !TNS.MUH.Arac.OdenekGOndermeSadeceEko2Mi(yil);
        }

        /// <summary>
        /// Ödenek Gönderme/tenkis tek belge listele
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">İşlem yapılacak bilgilerin taşındığı nesne</param>
        /// <param name="tekBelge">Tek belge mi listelencek?</param>
        /// <param name="detay">Detay satırlar listelensin mi?</param>
        /// <returns></returns>
        public ObjectVAN KimlikOku(Kullanici kullanan, GondermeTenkisForm form)
        {
            Tertip tUz = new Tertip(form.yil, ENUMTabloKod.TABPEBOdenek);

            ObjectVAN ov = new ObjectVAN();

            string kurum = form.kurum;

            int kontrolBelgeTur = 0;
            if (form.talep != "")
                kontrolBelgeTur = (int)ENUMBelgeTuru.ODEMEEMRI;
            else
                kontrolBelgeTur = (int)form.belgeTur;

            if (!MUH.IsKurallari.MUHYetkiServis.GormeYetkisiVarMiKurum(kullanan, kontrolBelgeTur, kurum))
            {
                ov.sonuc.DegerAta(ButceMuhasebeResI.MUH048);
                return ov;
            }

            ov.o = TekBelgeGetir(null, kullanan, form, false, false, true);

            return ov;
        }

        /// <summary>
        /// Ödenek Gönderme/tenkis tek belge listele
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">İşlem yapılacak bilgilerin taşındığı nesne</param>
        /// <param name="tekBelge">Tek belge mi listelencek?</param>
        /// <param name="detay">Detay satırlar listelensin mi?</param>
        /// <returns></returns>
        public ObjectArray Listele(Kullanici kullanan, GondermeTenkisForm form, bool tekBelge, int detay)
        {
            Tertip tUz = new Tertip(form.yil, ENUMTabloKod.TABPEBOdenek);

            Sonuc sonuc = new Sonuc(false, ButceMuhasebeResI.MUH047, "");

            ObjectArray fV = new ObjectArray();

            string kurum = "";
            if (form.kurum == "")
                kurum = OrtakFonksiyonlar.KurumsaldanKuruma(form.belgeNo.Substring(0, 4), tUz.Kirilimlar(0));
            else
                kurum = form.kurum;

            int kontrolBelgeTur = 0;
            if (form.talep != "")
                kontrolBelgeTur = (int)ENUMBelgeTuru.ODEMEEMRI;
            else
                kontrolBelgeTur = (int)form.belgeTur;

            if (!MUH.IsKurallari.MUHYetkiServis.GormeYetkisiVarMiKurum(kullanan, kontrolBelgeTur, kurum))
            {
                fV.sonuc.hataStr = ButceMuhasebeResI.MUH048;
                fV.sonuc.islemSonuc = false;
                return fV;
            }
            if (!tekBelge)
            {
                string pebKod;
                if (form.detay.ObjeSayisi > 0)
                    pebKod = ((GondermeTenkisDetay)form.detay.objeler[0]).tertip.kodlar[4];
                else
                    pebKod = "";
                if (!MUH.IsKurallari.MUHYetkiServis.AltFaaliyetYetkiVarMi(kullanan, kontrolBelgeTur, pebKod))
                {
                    fV.sonuc.hataStr = "Kriter alanında lütfen görebileceğiniz faaliyetleri seçiniz!";
                    fV.sonuc.islemSonuc = false;
                    return fV;
                }
            }

            if (tekBelge)
            {
                GondermeTenkisForm f = new GondermeTenkisForm();
                f = TekBelgeGetir(null, kullanan, form, true, true, true);

                if (f != null)
                {
                    //Tek Belge Açılıyorsa
                    string kurumlar = MUHYetkiServis.GondermeBelgesiKurumlariVer(kullanan);
                    bool yetkisiVar = false;
                    if (f.formDurum == (int)ENUMBelgeDurumu.TALEPKARSILA || f.formDurum == (int)ENUMBelgeDurumu.YENITALEP || f.formDurum == (int)ENUMBelgeDurumu.TALEP || form.tasra > 0) //ht:(form.tasra > 0)
                    {
                        if (kurumlar == "")
                        {
                            string muhasebeBirimler = TNS.MUH.Yetki.OdemeEmriMIFMuhasebeBirimVer(kullanan);
                            string[] birimler = muhasebeBirimler.Split(';');
                            for (int kk = 0; kk < birimler.Length; kk++)
                            {
                                string[] mb = birimler[kk].Split('-');

                                if (f.muhasebe == OrtakFonksiyonlar.ConvertToInt(mb[0], 0))
                                {
                                    yetkisiVar = true;
                                    break;
                                }
                            }
                        }
                    }

                    if (kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.SISTEMYONETICISI))
                        yetkisiVar = true;

                    if (kurumlar != "" || yetkisiVar)
                    {
                        fV.objeler.Add(f);
                        fV.sonuc.islemSonuc = true;
                    }
                    else
                    {
                        fV.sonuc.islemSonuc = false;
                        fV.sonuc.hataStr = string.Format(ButceMuhasebeResI.MUH049, form.belgeNo);
                    }
                }
                else
                {
                    fV.sonuc.islemSonuc = false;
                    fV.sonuc.hataStr = string.Format(ButceMuhasebeResI.MUH050, form.belgeNo);
                }
            }
            else
            {
                if (!TNS.MUH.Yetki.MuhasebeBirimYetkiVarMi(kullanan, form.muhasebe.ToString(), form.birim))
                {
                    fV.sonuc.hataStr = ButceMuhasebeResI.MUH048;
                    fV.sonuc.islemSonuc = false;
                    return fV;
                }

                fV = CokBelgeGetir(kullanan, form, detay);

                if (fV.objeler.Count == 0)
                {
                    fV.sonuc.islemSonuc = false;
                    fV.sonuc.hataStr = ButceMuhasebeResI.MUH051;
                }
            }

            return fV;
        }

        /// <summary>
        /// Teks the belge getir.
        /// </summary>
        /// <param name="veGelen">The ve gelen.</param>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="form">The form.</param>
        /// <param name="detayVer">if set to <c>true</c> [detay ver].</param>
        /// <returns></returns>
        public GondermeTenkisForm TekBelgeGetir(VeriErisim veGelen, Kullanici kullanan, GondermeTenkisForm form, bool detayVer, bool satirDurumBul, bool ekliDosyalar)
        {
            bhmServis = TNS.BHMv2.Arac.Tanimla();
            int mpDurum = bhmServis.MasrafPlaniGenelOnayDurumBul(kullanan, form.yil, "", BHMv2.ENUMMasrafPlanAsama.AHP);

            bool bulundu = false;

            VeriErisim ve = VeriErisim.VeriErisimBul(veGelen, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(kullanan));

            TABGondermeTenkis to = new TABGondermeTenkis(ve);
            GondermeTenkisForm sorguForm = new GondermeTenkisForm();
            sorguForm.yil = form.yil;
            sorguForm.belgeTur = form.belgeTur;
            sorguForm.belgeNo = form.belgeNo;
            sorguForm.kurum = form.kurum;
            Tertip tt = new Tertip(form.yil, ENUMTabloKod.TABPEBOdenek);

            IDataReader r = to.FormListe(kullanan, sorguForm, 0, 0, tt.KodUzunlukBul(3, 2, true), true, ekliDosyalar);

            GondermeTenkisForm os = null;
            if (to.Hata() == "")
            {
                if (r.Read())
                {
                    bulundu = true;
                    os = ReaderdanAl(r, 0, tt, null);
                }
            }
            if (r != null)
                r.Close();
            if (os == null)
            {
                os = new GondermeTenkisForm();
                os.belgeTur = form.belgeTur;
            }

            if (bulundu && detayVer)
            {
                VeriErisim ve3 = VeriErisim.VeriErisimBul(null, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(kullanan));
                string ortak000 = TNS.MUH.Arac.DegiskenDegerBul(os.yil, "/Ortak000");

                r = to.TekBelgeOkuDetay(os.yil, os.belgeNo, (int)os.belgeTur);

                if (to.Hata() == "")
                {
                    while (r.Read())
                    {
                        GondermeTenkisDetay sd;
                        sd = new GondermeTenkisDetay(os.yil);
                        string kur = VeriErisim.DegerAl(r, 3, "");
                        string fon = VeriErisim.DegerAl(r, 4, "");
                        string fin = VeriErisim.DegerAl(r, 5, "");
                        string eko = VeriErisim.DegerAl(r, 6, "");
                        string peb = VeriErisim.DegerAl(r, 7, "");
                        string yeniPeb = peb.Replace("0", "");
                        if (string.IsNullOrEmpty(yeniPeb)) peb = "";

                        sd.tertip.kodlar[0] = OrtakFonksiyonlar.KodNoktaKoy((string)tt.Kirilimlar(0), kur);
                        sd.tertip.kodlar[1] = OrtakFonksiyonlar.KodNoktaKoy((string)tt.Kirilimlar(1), fon);
                        sd.tertip.kodlar[2] = OrtakFonksiyonlar.KodNoktaKoy((string)tt.Kirilimlar(2), fin);
                        sd.tertip.kodlar[3] = OrtakFonksiyonlar.KodNoktaKoy((string)tt.Kirilimlar(3), eko);
                        sd.tertip.kodlar[4] = OrtakFonksiyonlar.KodNoktaKoy((string)tt.Kirilimlar(4), peb);

                        sd.miktar = VeriErisim.DegerAl(r, 8, 0.0);
                        if (os.belgeTur == ENUMBelgeTuru.TENKIS && sd.miktar < 0 && os.DortDuzeyDegisim != 1)
                            sd.miktar = 0 - sd.miktar;
                        if (satirDurumBul)
                            SatirDurumBul(ve3, kullanan, bhmServis, sd, form.yil, os.muhasebe.ToString(), os.birim, os.belgeTur, mpDurum, false, sd.miktar, os.formDurum == 99, ortak000 == "1");

                        os.detay.objeler.Add(sd);
                    }
                    r.Close();
                }
                ve3.VeriErisimKapat();
            }

            ve.VeriErisimKapat();
            return os;
        }

        public GondermeTenkisDetay SatirDurumBul(Kullanici kullanan, int form_yil, string muhasebe, string birim, ENUMBelgeTuru belgeTur, GondermeTenkisDetay sd, bool planlananOdenek, bool talep)
        {
            TNS.BHMv2.IBHMv2Servis bhmServis = TNS.BHMv2.Arac.Tanimla();
            int mpDurum = bhmServis.MasrafPlaniGenelOnayDurumBul(kullanan, form_yil, "", BHMv2.ENUMMasrafPlanAsama.AHP);
            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(kullanan));
            string ortak000 = TNS.MUH.Arac.DegiskenDegerBul(form_yil, "/Ortak000");

            SatirDurumBul(ve, kullanan, bhmServis, sd, form_yil, muhasebe, birim, belgeTur, mpDurum, planlananOdenek, 0, talep, ortak000 == "1");
            ve.VeriErisimKapat();
            return sd;
        }

        private static void SatirDurumBul(VeriErisim ve, Kullanici kullanan, TNS.BHMv2.IBHMv2Servis bhmServis, GondermeTenkisDetay sd, int form_yil, string muhasebe, string birim, ENUMBelgeTuru belgeTur, int mpDurum, bool planlananOdenek, double belgeMiktar, bool talep, bool ortak000)
        {
            if (ortak000)
                birim = "";

            string uzayTertip5 = sd.tertip.kodlar[0] + "-" + sd.tertip.kodlar[1] + "-" + sd.tertip.kodlar[2] + "-" + sd.tertip.kodlar[3] + "-" + sd.tertip.kodlar[4];

            double serbest = 0;
            string sorguGONBirim = form_yil + "-" + (int)ENUMBelgeTuru.ODENEKGONDERME + "-" + "-" + "-" + muhasebe + "-" + birim + "-" + "-" + (int)ENUMBelgeDurumu.ONAYLI + "-" + uzayTertip5;
            string sorguTENBirim = form_yil + "-" + (int)ENUMBelgeTuru.TENKIS + "-" + "-" + "-" + muhasebe + "-" + birim + "-" + "-" + (talep ? "0/8,99" : "0/8") + "-" + uzayTertip5;
            string sorguHRCBirim = form_yil + "-" + muhasebe + "-" + birim + "-" + "830" + "-" + uzayTertip5 + "-" + "0/6" + "-" + sd.tertip.kodlar[4];

            double toplamGONBirim = MUHServis.UzaySayiDegerGetir(ve, kullanan, "MUHGONDERMEBELGESITENKISKOSULSUZ", sorguGONBirim, false, "SUM(MIKTAR)");
            double toplamTENBirim = Math.Abs(MUHServis.UzaySayiDegerGetir(ve, kullanan, "MUHGONDERMEBELGESITENKISKOSULSUZ", sorguTENBirim, false, "SUM(MIKTAR)"));

            sd.gonderilen = toplamGONBirim - toplamTENBirim;
            sd.harcanan = MUHServis.UzaySayiDegerGetir(ve, kullanan, "MUHODEMEEMRIMIF", sorguHRCBirim, false, "");

            int sonAy = ((DateTime.Now.Month - 1) / 3 + 1) * 3 + 1;
            string ayKosul = "0/" + sonAy.ToString();
            string[] kodDizi = null;
            if (mpDurum == 5)
            {
                kodDizi = bhmServis.KurumsalYapiListele(muhasebe.ToString(), birim, sd.tertip.kodlar[0]);
                if (kodDizi != null && kodDizi.Length == 1)
                {
                    sd.donemMP = MUHServis.UzaySayiDegerGetir(ve, kullanan, "MPGMASRAFPLANITERTIP", form_yil + "-" + "5" + "-" + uzayTertip5 + "-" + kodDizi[0] + "-" + ayKosul + "-" + "0/6", false, "");
                    sd.toplamMP = MUHServis.UzaySayiDegerGetir(ve, kullanan, "MPGMASRAFPLANITERTIP", form_yil + "-" + "5" + "-" + uzayTertip5 + "-" + kodDizi[0] + "-" + "" + "-" + "0/6", false, "");
                }
            }

            if (belgeTur == ENUMBelgeTuru.ODENEKGONDERME)
            {
                serbest = 0;
                double toplamGONT = 0;
                double toplamTENT = 0;
                if (mpDurum != 5)
                {
                    serbest = MUHServis.UzaySayiDegerGetir(ve, kullanan, "ABSODENEK", form_yil + "-0600-0-" + uzayTertip5, false, "");
                    string sorguGONToplam = form_yil + "-" + (int)ENUMBelgeTuru.ODENEKGONDERME + "------" + "" + "-" + uzayTertip5;
                    string sorguTENToplam = form_yil + "-" + (int)ENUMBelgeTuru.TENKIS + "------" + (int)ENUMBelgeDurumu.ONAYLI + "-" + uzayTertip5;
                    toplamGONT = MUHServis.UzaySayiDegerGetir(ve, kullanan, "MUHGONDERMEBELGESITENKIS", sorguGONToplam, false, "SUM(MIKTAR)");
                    toplamTENT = Math.Abs(MUHServis.UzaySayiDegerGetir(ve, kullanan, "MUHGONDERMEBELGESITENKIS", sorguTENToplam, false, "SUM(MIKTAR)"));
                }
                else if (planlananOdenek)
                {
                    serbest = sd.donemMP;
                    toplamGONT = toplamGONBirim;
                    toplamTENT = toplamTENBirim;
                }
                else
                {
                    serbest = MUHServis.UzaySayiDegerGetir(ve, kullanan, "MPGMASRAFPLANITERTIP", form_yil + "-" + "5" + "-" + uzayTertip5 + "-" + "9" + "-" + ayKosul + "-" + "0/6", false, "");
                }
                sd.kullanilabilir = serbest - (toplamGONT - toplamTENT);
            }
            else
                sd.kullanilabilir = sd.gonderilen - sd.harcanan + belgeMiktar;
        }

        /// <summary>
        /// Ödenek Gönderme/tenkis tek belge tek detay listele
        /// </summary>
        /// <param name="yil">Yıl</param>
        /// <param name="belgeNo">Belge numarası</param>
        /// <param name="belgeTur">Belge turü</param>
        /// <param name="tertip">Tertip bilgisi</param>
        /// <returns></returns>
        public GondermeTenkisDetay TekBelgeTekDetayGetir(int yil, string belgeNo, int belgeTur, string tertip)
        {
            GondermeTenkisDetay sd = null;

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(null));
            TABGondermeTenkis to = new TABGondermeTenkis(ve);

            IDataReader r = to.TekBelgeOkuDetay(yil, belgeNo, belgeTur, tertip);

            if (to.Hata() == "")
            {
                Tertip tt = new Tertip(yil, ENUMTabloKod.TABPEBOdenek);

                while (r.Read())
                {
                    sd = new GondermeTenkisDetay(yil);
                    string kur = VeriErisim.DegerAl(r, 3, "");
                    string fon = VeriErisim.DegerAl(r, 4, "");
                    string fin = VeriErisim.DegerAl(r, 5, "");
                    string eko = VeriErisim.DegerAl(r, 6, "");
                    string peb = VeriErisim.DegerAl(r, 7, "");

                    sd.tertip.kodlar[0] = OrtakFonksiyonlar.KodNoktaKoy((string)tt.Kirilimlar(0), kur);
                    sd.tertip.kodlar[1] = OrtakFonksiyonlar.KodNoktaKoy((string)tt.Kirilimlar(1), fon);
                    sd.tertip.kodlar[2] = OrtakFonksiyonlar.KodNoktaKoy((string)tt.Kirilimlar(2), fin);
                    sd.tertip.kodlar[3] = OrtakFonksiyonlar.KodNoktaKoy((string)tt.Kirilimlar(3), eko);
                    sd.tertip.kodlar[4] = OrtakFonksiyonlar.KodNoktaKoy((string)tt.Kirilimlar(4), peb);
                    sd.miktar = VeriErisim.DegerAl(r, 8, 0.0);
                }
                r.Close();
            }

            return sd;
        }

        ObjectArray CokBelgeGetir(Kullanici kullanan, GondermeTenkisForm form, int detay)
        {
            Tertip tUzBul = new Tertip(form.yil, ENUMTabloKod.TABPEBOdenek);
            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(kullanan));

            TABGondermeTenkis to = new TABGondermeTenkis(ve);
            //Listeleme sırasında kurum kullanıcıları kendilerine ait evrakları görsün
            if (!(kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.SISTEMYONETICISI) ||
                kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.KULLANICIYONETICISI)))
            {
                for (int i = 0; i < form.detay.objeler.Count; i++)
                {
                    GondermeTenkisDetay d = (GondermeTenkisDetay)form.detay[i];

                    string kur = Arac.DiziElemanAl(d.tertip.kodlar, 0, "").Replace(".", "");
                    string fon = Arac.DiziElemanAl(d.tertip.kodlar, 1, "").Replace(".", "");
                    string fin = Arac.DiziElemanAl(d.tertip.kodlar, 2, "").Replace(".", "");
                    string eko = Arac.DiziElemanAl(d.tertip.kodlar, 3, "").Replace(".", "");
                    string peb = "";
                    if (d.tertip.kodlar.Length > 4)
                        peb = Arac.DiziElemanAl(d.tertip.kodlar, 4, "").Replace(".", "");

                    string kurumlar = MUHYetkiServis.GondermeBelgesiKurumlariVer(kullanan);
                    if (form.formDurum == (int)ENUMBelgeDurumu.TALEP || form.formDurum == (int)ENUMBelgeDurumu.YENITALEP)
                    {
                        //**hem taşra talep hemde taşra bütün belgelerde kullanıcını muhasebe birimi dikkate alınacağı için
                        //fonksiyon haline getirildi(Birimine ait belgeleri görsün)
                        KullaniciMuhasebeListesiGetir(kullanan, kurumlar, ref form);
                    }

                    if (kur != "")
                    {
                        if (kurumlar != "")
                        {
                            string[] kurlar = kurumlar.Split(';');
                            for (int j = 0; j < kurlar.Length; j++)
                            {
                                string tmp = kurlar[j].Replace(".", "");
                                if (tmp != "" && kur.Length >= tmp.Length)
                                {
                                    if (kur.Substring(0, tmp.Length).Replace(".", "") == tmp)
                                    {
                                        kurumlar = kur;
                                        break;
                                    }
                                }
                            }
                            if (kurumlar.IndexOf(kur + ";") > -1)
                                kurumlar = kur;
                        }
                        else
                            kurumlar = kur;
                    }

                    GondermeTenkisDetay dt = new GondermeTenkisDetay();

                    dt.tertip.kodlar[0] = kurumlar;
                    dt.tertip.kodlar[1] = fon;
                    dt.tertip.kodlar[2] = fin;
                    dt.tertip.kodlar[3] = eko;
                    dt.tertip.kodlar[4] = peb;
                    dt.miktar = ((GondermeTenkisDetay)form.detay[i]).miktar;

                    form.detay.objeler.Clear();
                    form.detay.objeler.Add(dt);
                }

            }
            //**ht kullanıcı taşra kullanıcısı ise ve (talep!="") ve listelemek istediği veri(22=Bütün Belgeler(İptaller Hariç) ise)
            //kullanıcı birimine ait "bütün belgeler(iptaller hariç)" görsün
            if ((!kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.SISTEMYONETICISI) || kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.KULLANICIYONETICISI)) &&
                (form.tasra > 0 && (form.formDurum == 22 || form.formDurum == (int)ENUMBelgeDurumu.ONAYLI)))
            {
                string kurumlar = MUHYetkiServis.GondermeBelgesiKurumlariVer(kullanan);
                KullaniciMuhasebeListesiGetir(kullanan, kurumlar, ref form);
            }

            ObjectArray oslar = new ObjectArray();
            IDataReader r = to.FormListe(kullanan, form, detay, 0, tUzBul.KodUzunlukBul(3, 2, true), false, false);
            if (r != null)
            {
                GondermeTenkisForm os = null;

                while (r.Read())
                {
                    GondermeTenkisDetay d = new GondermeTenkisDetay();
                    GondermeTenkisForm osos = ReaderdanAl(r, detay, tUzBul, d);

                    if (os == null || (os.belgeNo != osos.belgeNo || os.yil != osos.yil || os.belgeTur != osos.belgeTur))
                    {
                        os = osos;
                        oslar.objeler.Add(os);
                    }

                    if (detay > 0)
                    {
                        os.detay.objeler.Add(d);
                        os.belgeToplami += d.miktar;
                    }
                }
                r.Close();
            }
            else
                oslar.sonuc.DegerAta(ve.hata);

            ve.VeriErisimKapat();

            return oslar;
        }

        public static GondermeTenkisForm ReaderdanAl(IDataReader r, int detay, Tertip tt, GondermeTenkisDetay formDetay)
        {
            GondermeTenkisForm os = new GondermeTenkisForm();
            int kolNo = 0;
            os.yil = VeriErisim.DegerAl(r, kolNo++, 0);
            os.belgeTur = (ENUMBelgeTuru)VeriErisim.DegerAl(r, kolNo++, 0);
            os.belgeNo = VeriErisim.DegerAl(r, kolNo++, "");
            os.kurum = VeriErisim.DegerAl(r, kolNo++, "");
            if (tt != null)
                os.kurum = OrtakFonksiyonlar.KodNoktaKoy((string)tt.Kirilimlar(0), os.kurum);
            os.muhasebe = VeriErisim.DegerAl(r, kolNo++, 0);
            os.birim = VeriErisim.DegerAl(r, kolNo++, "");
            os.aciklama = VeriErisim.DegerAl(r, kolNo++, "");
            os.islemTarih = VeriErisim.DegerAl(r, kolNo++);
            os.formDurumTarih = VeriErisim.DegerAl(r, kolNo++);
            os.formDurum = VeriErisim.DegerAl(r, kolNo++, 0);
            os.iptalNeden = VeriErisim.DegerAl(r, kolNo++, "");

            os.icmalNo = VeriErisim.DegerAl(r, kolNo++, 0);
            os.icmalTarih = VeriErisim.DegerAl(r, kolNo++);
            os.onayNo = VeriErisim.DegerAl(r, kolNo++, "");
            os.onayTarih = VeriErisim.DegerAl(r, kolNo++);
            os.yaziTarih = VeriErisim.DegerAl(r, kolNo++);
            os.yaziNo = VeriErisim.DegerAl(r, kolNo++, "");
            os.istekYaziTarih = VeriErisim.DegerAl(r, kolNo++);
            os.istekYaziNo = VeriErisim.DegerAl(r, kolNo++, "");

            os.islemYapan = VeriErisim.DegerAl(r, kolNo++, "");
            os.DortDuzeyDegisim = VeriErisim.DegerAl(r, kolNo++, 0);
            os.surecYil = VeriErisim.DegerAl(r, kolNo++, 0);
            os.surecNo = VeriErisim.DegerAl(r, kolNo++, "");
            os.isKodu = VeriErisim.DegerAl(r, kolNo++, 0);

            os.kurumAd = VeriErisim.DegerAl(r, kolNo++, "");
            os.muhasebeAd = VeriErisim.DegerAl(r, kolNo++, "");
            os.birimAd = VeriErisim.DegerAl(r, kolNo++, "");
            os.vergiNo = VeriErisim.DegerAl(r, kolNo++, "");
            os.isAdi = VeriErisim.DegerAl(r, kolNo++, "");
            if (os.isKodu > 0)
                os.isAdi = os.isKodu + "- " + os.isAdi;

            os.ekDosyaAd1 = VeriErisim.DegerAl(r, kolNo++, "");
            os.ekDosyaAd2 = VeriErisim.DegerAl(r, kolNo++, "");

            if (detay == 0)
                os.belgeToplami = System.Math.Abs(VeriErisim.DegerAl(r, kolNo++, 0.0));
            else if (formDetay != null)
            {
                formDetay.tertip.kodlar[0] = OrtakFonksiyonlar.KodNoktaKoy((string)tt.Kirilimlar(0), VeriErisim.DegerAl(r, kolNo++, ""));
                formDetay.tertip.kodlar[1] = OrtakFonksiyonlar.KodNoktaKoy((string)tt.Kirilimlar(1), VeriErisim.DegerAl(r, kolNo++, ""));
                formDetay.tertip.kodlar[2] = OrtakFonksiyonlar.KodNoktaKoy((string)tt.Kirilimlar(2), VeriErisim.DegerAl(r, kolNo++, ""));
                formDetay.tertip.kodlar[3] = OrtakFonksiyonlar.KodNoktaKoy((string)tt.Kirilimlar(3), VeriErisim.DegerAl(r, kolNo++, ""));
                if (detay == 2)
                {
                    formDetay.tertip.kodlar[4] = OrtakFonksiyonlar.KodNoktaKoy((string)tt.Kirilimlar(4), VeriErisim.DegerAl(r, kolNo++, ""));
                }
                formDetay.miktar = Math.Abs(VeriErisim.DegerAl(r, kolNo++, 0.0));
            }

            return os;
        }

        private void KullaniciMuhasebeListesiGetir(Kullanici kullanan, string kurumlar, ref GondermeTenkisForm form)
        {
            if (kurumlar == "")
            {
                kurumlar = TNS.MUH.Yetki.OdemeEmriMIFKurumlariVer(kullanan);

                string muhasebeBirimler = TNS.MUH.Yetki.OdemeEmriMIFMuhasebeBirimVer(kullanan);
                string[] birimler = muhasebeBirimler.Split(';');
                for (int kk = 0; kk < birimler.Length; kk++)
                {
                    string[] mb = birimler[kk].Split('-');
                    if (form.sorguMuhasebeKod != "") form.sorguMuhasebeKod += ";";
                    form.sorguMuhasebeKod += mb[0];
                    if (mb.Length > 1 && OrtakFonksiyonlar.ConvertToInt(mb[1], -1) >= 0)
                    {
                        if (form.birim != "") form.birim += ";";
                        form.birim += mb[1];
                    }
                }
            }
        }

        /// <summary>
        /// Belge durum değiştir
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="formlar">İşlem yapılacak bilgileri içeren nesme</param>
        /// <returns></returns>
        public Sonuc BelgeDurumDegistir(Kullanici kullanan, TNSCollection formlar)
        {
            Hashtable gtiFormOkuHt = new Hashtable();
            Hashtable mesajlar = new Hashtable();
            bhmServis = TNS.BHMv2.Arac.Tanimla();
            Sonuc sonuc = new Sonuc(false, ButceMuhasebeResI.MUH052, "");

            if (formlar.Count == 0) return new Sonuc(false, "Belge durumu değiştirilecek kayıt seçiniz!", "");

            GondermeTenkisForm ilkForm = (GondermeTenkisForm)formlar[0];

            int mpDurum = bhmServis.MasrafPlaniGenelOnayDurumBul(kullanan, ilkForm.yil, "", BHMv2.ENUMMasrafPlanAsama.AHP);

            string hata = "";
            string yetkiHata = "";
            bool gelismisIcmalKontrol = TNS.MUH.Arac.GelismisIcmalKontrol(ilkForm.yil);

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(kullanan));
            List<string> kurList = new List<string>();
            List<string> ekoList = new List<string>();
            List<string> pebList = new List<string>();
            List<int> icmalNolar = new List<int>();

            try
            {
                foreach (GondermeTenkisForm form in formlar)
                {
                    GondermeTenkisForm oS = TekBelgeGetir(ve, kullanan, form, true, false, form.formDurum == (int)ENUMBelgeDurumu.TALEP);
                    if (ilkForm.formDurum == (int)ENUMBelgeDurumu.ICMALYEVMIYENOVER || ilkForm.formDurum == (int)ENUMBelgeDurumu.MUHASEBATAGONDERILDI)
                    {
                        if (oS.icmalNo == 0)
                        {
                            if (ilkForm.formDurum == (int)ENUMBelgeDurumu.MUHASEBATAGONDERILDI)
                            {
                                hata += "<br>" + oS.belgeNo + " numaralı belge, icmali olmadığından muhasebata gönderilemez!";
                            }
                        }
                        else if (icmalNolar.IndexOf(oS.icmalNo) < 0)
                        {
                            icmalNolar.Add(oS.icmalNo);

                            GondermeTenkisIcmal gtiFormOku = MUHOdenekGondermeIcmal_Oku(ve, kullanan, gtiFormOkuHt, oS.yil, oS.belgeTur, oS.icmalNo);

                            if (ilkForm.formDurum == (int)ENUMBelgeDurumu.ICMALYEVMIYENOVER)
                            {
                                if (gtiFormOku != null && gtiFormOku.formDurum == 5) //icmal onaylı, icmal no verilemez.durumu değiştirilemez
                                {
                                    hata += "<br>" + oS.icmalNo + " numaralı icmal onaylandığından, bu icmale ait belgelerde değişiklik yapılamaz!";
                                }
                            }
                            else
                            {
                                if (!gtiFormOku.KullaniciVarMi(kullanan.kullaniciKodu))
                                {
                                    hata += string.Format("{0} numaralı icmali muhasebata ilgili icmale yetkili kullanıcılar gönderebilir!", oS.icmalNo);
                                }
                            }
                        }
                    }
                    int kontrolBelgeTur = 0;
                    if (form.talep != "")
                        kontrolBelgeTur = (int)ENUMBelgeTuru.ODEMEEMRI;
                    else
                        kontrolBelgeTur = (int)form.belgeTur;

                    bool yetki = false;

                    if (form.formDurum == (int)ENUMBelgeDurumu.MUHASEBATAGONDERILDI || form.formDurum == (int)ENUMBelgeDurumu.MUHASEBATALDI || form.formDurum == (int)ENUMBelgeDurumu.ONAYLI)
                        yetki = MUH.IsKurallari.MUHYetkiServis.MuhasebatOnaylamaYetkisiVarMi(kullanan, kontrolBelgeTur, form.kurum);
                    else
                        yetki = MUH.IsKurallari.MUHYetkiServis.YazmaYetkisiVarMiKurum(kullanan, kontrolBelgeTur, form.kurum);

                    int satir = 0;

                    foreach (GondermeTenkisDetay detay in oS.detay.objeler)
                    {
                        satir++;
                        string kur = detay.tertip.kodlar[0];
                        string eko = detay.tertip.kodlar[3];
                        string peb = TNS.MUH.Arac.DiziElemanAl(detay.tertip.kodlar, 4, "");

                        if (!kurList.Contains(kur))
                        {
                            kurList.Add(kur);
                            if (!MUH.IsKurallari.MUHYetkiServis.YazmaYetkisiVarMi(kullanan, kontrolBelgeTur, kur))
                            {
                                yetkiHata += "<br>" + string.Format(ButceMuhasebeResI.MUH034, satir.ToString(), kur);
                                yetki = false;
                            }
                        }

                        if (!ekoList.Contains(eko))
                        {
                            ekoList.Add(eko);

                            if (!MUH.IsKurallari.MUHYetkiServis.EkonomikYetkiVarMi(kullanan, kontrolBelgeTur, eko))
                            {
                                yetkiHata += "<br>" + string.Format(ButceMuhasebeResI.MUH035, satir.ToString(), eko);
                                yetki = false;
                            }
                        }

                        if (!pebList.Contains(peb))
                        {
                            pebList.Add(peb);

                            if (!MUH.IsKurallari.MUHYetkiServis.AltFaaliyetYetkiVarMi(kullanan, kontrolBelgeTur, peb))
                            {
                                yetkiHata += "<br>" + string.Format("Belgede {0}. satırda kullandığınız Faaliyet ({1}) ile ilgili işlem yapma yetkiniz yok", satir.ToString(), peb);
                                yetki = false;
                            }
                        }
                        if (!yetki) break;
                    }

                    if (!yetki && (form.DortDuzeyDegisim == 1 || form.formDurum == (int)ENUMBelgeDurumu.TALEPIPTAL || form.formDurum == (int)ENUMBelgeDurumu.TALEP))
                    {
                        yetki = TNS.MUH.Yetki.MuhasebeBirimYetkiVarMi(kullanan, oS.muhasebe.ToString(), oS.birim);
                        yetkiHata = "";
                    }

                    if (!yetki)
                    {
                        if (form.formDurum == (int)ENUMBelgeDurumu.MUHASEBATAGONDERILDI || form.formDurum == (int)ENUMBelgeDurumu.ONAYLI)
                            yetkiHata = "<br>" + string.Format(ButceMuhasebeResI.MUH053, form.belgeNo, form.kurum) + "<br>" + yetkiHata;
                        else
                            yetkiHata = "<br>" + string.Format(ButceMuhasebeResI.MUH054, form.belgeNo) + "<br>" + yetkiHata;
                        break;
                    }

                    if (oS.kurum != "")
                    {
                        if (oS.kurum.Replace(".", "") != form.kurum.Replace(".", ""))
                            hata += "<br>" + ButceMuhasebeResI.MUH055;

                        if (form.formDurum == (int)ENUMBelgeDurumu.TALEP)
                        {
                            if (SurecliTalep)
                            {
                                if (string.IsNullOrEmpty(oS.ekDosyaAd1) || string.IsNullOrEmpty(oS.ekDosyaAd2))
                                    hata += "<br>" + "Talep merkeze gönderilirken, Lütfen izmalı belge ve destekleyici belge yükleyiniz!";
                            }
                        }

                        if (form.formDurum == (int)ENUMBelgeDurumu.DEGISIMONAYLA && oS.DortDuzeyDegisim == 0)
                            hata += "<br>" + ButceMuhasebeResI.MUH056;

                        if ((form.formDurum == (int)ENUMBelgeDurumu.MUHASEBATAGONDERILDI || form.formDurum == (int)ENUMBelgeDurumu.MUHASEBATALDI) && oS.icmalNo == 0)
                            hata += "<br>" + "İcmal no verilmemiş belgeler, muhasebata gönderilemez!";

                        if (form.formDurum == (int)ENUMBelgeDurumu.MUHASEBATAGONDERILDI && oS.DortDuzeyDegisim == 1)
                            hata += "<br>" + ButceMuhasebeResI.MUH057;

                        if (oS.formDurum == (int)ENUMBelgeDurumu.TALEPKARSILA)
                            hata += "<br>" + "Karşılanan taleplerin durumu değiştirilemez!";

                        if (form.formDurum == (int)ENUMBelgeDurumu.BELGEDURUMUYENIYAP)
                        {
                            if (!kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.SISTEMYONETICISI))
                                hata += "<br>" + ButceMuhasebeResI.MUH058;
                            else if (oS.formDurum == (int)ENUMBelgeDurumu.MUHASEBATAGONDERILDI || oS.formDurum == (int)ENUMBelgeDurumu.TAHSIS)
                                form.formDurum = (int)ENUMBelgeDurumu.DEGISTIRILDI;
                            else
                                hata += ButceMuhasebeResI.MUH059;
                        }
                        else if (oS.formDurum != (int)ENUMBelgeDurumu.YENI && oS.formDurum != (int)ENUMBelgeDurumu.DEGISTIRILDI && oS.formDurum != (int)ENUMBelgeDurumu.TAHSIS && oS.formDurum != (int)ENUMBelgeDurumu.YENITALEP && oS.formDurum != (int)ENUMBelgeDurumu.TALEP && oS.formDurum != (int)ENUMBelgeDurumu.YEVMIYE_OESGBYEGONDER)
                        {
                            if (!(oS.formDurum == (int)ENUMBelgeDurumu.IPTAL && form.formDurum == (int)ENUMBelgeDurumu.MUHASEBATALDI))
                            {
                                if (oS.formDurum == (int)ENUMBelgeDurumu.MUHASEBATAGONDERILDI)
                                    hata = "<br>" + ButceMuhasebeResI.MUH021;
                                else if (oS.formDurum == (int)ENUMBelgeDurumu.MUHASEBATALDI)
                                    hata = "<br>" + ButceMuhasebeResI.MUH022;
                                else if (oS.formDurum == (int)ENUMBelgeDurumu.ONAYLI)
                                    hata = "<br>" + ButceMuhasebeResI.MUH023;
                                else if (oS.formDurum == (int)ENUMBelgeDurumu.MUHASEBATIADE)
                                    hata = "<br>" + ButceMuhasebeResI.MUH024;
                                else if (oS.formDurum == (int)ENUMBelgeDurumu.IPTAL && form.formDurum != (int)ENUMBelgeDurumu.MUHASEBATALDI)
                                    hata = "<br>" + ButceMuhasebeResI.MUH025;
                                else
                                    hata += "<br>" + ButceMuhasebeResI.MUH026;
                            }
                        }

                        if (oS.icmalNo <= 0 && form.formDurum == (int)ENUMBelgeDurumu.MUHASEBATAGONDERILDI)
                            hata += "<br>" + string.Format(ButceMuhasebeResI.MUH060, form.belgeNo);

                        if (oS.icmalNo > 0 && form.formDurum == (int)ENUMBelgeDurumu.ICMALYEVMIYENOVER)
                        {
                            if ((form.icmalNo != 0))
                                hata += "<br>" + string.Format(ButceMuhasebeResI.MUH061, form.belgeNo);
                            else if (gelismisIcmalKontrol)
                            {
                                GondermeTenkisIcmal gti = MUHOdenekGondermeIcmal_Oku(ve, kullanan, gtiFormOkuHt, oS.yil, oS.belgeTur, oS.icmalNo);
                                if (gti != null && gti.formDurum == 5)
                                {
                                    hata += "<br>" + string.Format("{0} nolu ödeme emrinin dahil olduğu icmal {1} onaylı olduğundan, icmalden ödeme emrini çıkartamazsınız!", oS.belgeNo, oS.icmalNo);
                                }
                            }
                        }

                        if (oS.formDurum == (int)ENUMBelgeDurumu.TALEP && (form.formDurum != (int)ENUMBelgeDurumu.TALEPIPTAL && form.formDurum != (int)ENUMBelgeDurumu.IPTAL))
                        {
                            hata += "<br>" + string.Format("{0} nolu talebi sadece iptal edebilirsiniz, lütfen kaydetme veya belge değiştirme işlemlerini kullanınız!", form.belgeNo);
                        }

                        if (oS.formDurum == (int)ENUMBelgeDurumu.YENITALEP && (form.formDurum != (int)ENUMBelgeDurumu.TALEPIPTAL && form.formDurum != (int)ENUMBelgeDurumu.TALEP))
                        {
                            hata += "<br>" + string.Format("{0} nolu talebi sadece merkeze gönderelebilir veya iptal edebilirsiniz, lütfen kaydetme veya belge değiştirme işlemlerini kullanınız!", form.belgeNo);
                        }

                        if (form.formDurum == (int)ENUMBelgeDurumu.TALEPIPTAL)
                        {
                            if (oS.formDurum != (int)ENUMBelgeDurumu.TALEP || oS.formDurum != (int)ENUMBelgeDurumu.YENITALEP)
                            {
                                hata += "<br>" + string.Format(ButceMuhasebeResI.MUH061, form.belgeNo);
                            }
                            else
                            {
                                form.formDurum = (int)ENUMBelgeDurumu.IPTAL;
                            }
                        }

                        if (form.formDurum != (int)ENUMBelgeDurumu.IPTAL && form.formDurum != (int)ENUMBelgeDurumu.MUHASEBATIADE && string.IsNullOrEmpty(hata))
                        {
                            if (!(form.formDurum == (int)ENUMBelgeDurumu.DEGISTIRILDI && oS.formDurum == (int)ENUMBelgeDurumu.MUHASEBATAGONDERILDI))
                            {
                                if (!(oS.formDurum == (int)ENUMBelgeDurumu.IPTAL && form.formDurum == (int)ENUMBelgeDurumu.MUHASEBATALDI))
                                {
                                    string eskiTalepNo = null;
                                    string talepYapan = null;
                                    Sonuc ss = new Sonuc();
                                    oS.talep = form.talep;
                                    ss = KontrolEt(ve, kullanan, TNS.BHMv2.Arac.Tanimla(), oS, null, mpDurum, false, ref eskiTalepNo, ref talepYapan, true, false, gtiFormOkuHt);
                                    if (ss.hataStr != "")
                                        hata += string.Format(ButceMuhasebeResI.MUH062, oS.belgeNo) + "<br>" + ss.hataStr;
                                }
                            }
                        }

                        if (form.formDurum == (int)ENUMBelgeDurumu.IPTAL && (oS.formDurum == (int)ENUMBelgeDurumu.TALEP || oS.formDurum == (int)ENUMBelgeDurumu.YENITALEP))
                            mesajlar[form.belgeNo] = new KullaniciMesaj(oS.islemYapan, string.Format("{0} nolu talebiniz iptal edildi ({1}).", form.belgeNo, form.iptalNeden));
                    }
                    else
                        hata += "<br>" + string.Format(ButceMuhasebeResI.MUH063, form.belgeNo);
                }
            }
            catch (Exception ex)
            {
                hata += "Kontrol sırasında beklenmedik hata:" + ex.Message;
            }

            if (kurList.Count == 0)
            {
                hata += "Kontrol edilecek kurumsal kod bulunamadı!";
            }

            if (yetkiHata != "")
                hata += "<br>" + yetkiHata;

            if (hata != "")
            {
                sonuc.DegerAta(hata);
                ve.VeriErisimKapat();
                return sonuc;
            }

            TABGondermeTenkis to = new TABGondermeTenkis(ve);
            int icmalNo = 0;
            bool icmalFormKontrolYapma = false;
            if (hata == "")
            {
                if (ilkForm.formDurum == (int)ENUMBelgeDurumu.ICMALYEVMIYENOVER)
                {
                    lock (this)
                    {
                        icmalNo = to.EnBuyukIcmalNo(ilkForm.yil, (int)ilkForm.belgeTur, ilkForm.kurum);
                        if (ilkForm.icmalNo > -1)
                        {
                            if (ilkForm.icmalNo > icmalNo)
                            {
                                hata += string.Format(ButceMuhasebeResI.MUH064, ilkForm.icmalNo, icmalNo);
                            }
                            else
                                icmalNo = ilkForm.icmalNo;
                        }
                        else
                            icmalFormKontrolYapma = true;
                    }

                    if (gelismisIcmalKontrol && icmalNo > 0)
                    {
                        bool tasraVar = false;
                        foreach (string kurKod in kurList)
                        {
                            if (kurKod.EndsWith("62"))
                            {
                                tasraVar = true;
                                break;
                            }
                        }

                        if (tasraVar && !kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.SISTEMYONETICISI))
                        {
                            hata += "Taşra kurumsalı için İcmal verme yetkiniz bulunmamaktadır. Lütfen seçili Ödenek Gönderme Belgelerini SGBye gönderiniz!";
                        }

                        GondermeTenkisIcmal gtiFormOku = MUHOdenekGondermeIcmal_Oku(ve, kullanan, gtiFormOkuHt, ilkForm.yil, ilkForm.belgeTur, icmalNo);
                        if (gtiFormOku == null || icmalFormKontrolYapma)
                        {
                            gtiFormOku = new GondermeTenkisIcmal();
                            gtiFormOku.yil = ilkForm.yil;
                            gtiFormOku.icmalNo = icmalNo;
                            gtiFormOku.belgeTur = ilkForm.belgeTur;
                            gtiFormOku.formDurum = 0;
                            gtiFormOku.kayitTarih.Yaz(DateTime.Now);
                            gtiFormOku.kurKod = kurList[0];
                            gtiFormOku.kullanicilar = new TNSCollection();
                            gtiFormOku.kullanicilar.Add(new GondermeTenkisIcmalKullanici(kullanan.kullaniciKodu));
                            Sonuc s = MUHOdenekGondermeIcmal.Kaydet(ve, kullanan, gtiFormOku, icmalFormKontrolYapma);
                            if (!s.islemSonuc)
                            {
                                ve.VeriErisimKapat();
                                return s;
                            }
                        }

                        if (gtiFormOku != null)
                        {
                            if (gtiFormOku.formDurum == 5)
                            {
                                hata += string.Format("{0} numaralı icmal onaylı olduğundan değiştirilemez!", icmalNo);
                            }

                            if (!gtiFormOku.KullaniciVarMi(kullanan.kullaniciKodu))
                            {
                                hata += string.Format("{0} numaralı icmale yazma hakkınız yok, eğer bu icmalde değişiklik yapmak istiyorsanız kullanıcı kodunuzu icmal formuna ekleyiniz!", icmalNo);
                            }

                            foreach (string gelenKurKod in kurList)
                            {
                                if (!gelenKurKod.Replace(".", "").StartsWith(gtiFormOku.kurKod.Replace(".", "")))
                                {
                                    hata += string.Format("İcmal {0} kurumsal kodu için hazırlanmış fakat içine farklı bir kurumsal kod {1} eklemeye çalışıyorsunuz!", gtiFormOku.kurKod, gelenKurKod);
                                }
                            }
                        }
                    }
                }
            }

            if (hata != "")
            {
                sonuc.DegerAta(hata);
                ve.VeriErisimKapat();
                return sonuc;
            }

            try
            {
                TNSCollection islenenFormlar = new TNSCollection();
                foreach (GondermeTenkisForm form in formlar)
                {
                    ve.BeginTransaction();
                    Sonuc s = new Sonuc();
                    if (form.formDurum == (int)ENUMBelgeDurumu.ICMALYEVMIYENOVER)
                    {
                        form.icmalNo = icmalNo;
                        s = to.IcmalNoVer(form);
                    }
                    else
                    {
                        if (form.formDurum == (int)ENUMBelgeDurumu.DEGISIMONAYLA) form.formDurum = (int)ENUMBelgeDurumu.ONAYLI;
                        if (kullanan != null && !string.IsNullOrEmpty(kullanan.kullaniciKodu))
                        {
                            form.islemYapan = kullanan.kullaniciKodu;
                        }

                        s = to.BelgeDurumDegistir(form);
                    }

                    if (!s.islemSonuc)
                    {
                        ve.Rollback();
                        hata = s.hataStr;
                        break;
                    }
                    else
                    {
                        ve.Commit();
                        islenenFormlar.Add(form);
                    }
                }

                List<KullaniciMesaj> al = new List<KullaniciMesaj>();
                TNSCollection tc = new TNSCollection();
                foreach (GondermeTenkisForm form in islenenFormlar)
                {
                    try
                    {
                        KullaniciMesaj km = (KullaniciMesaj)mesajlar[form.belgeNo];
                        al.Add(km);
                    }
                    catch { }
                    if (mpDurum == 5)
                    {
                        if (form.formDurum == (int)ENUMBelgeDurumu.IPTAL || form.formDurum == (int)ENUMBelgeDurumu.MUHASEBATIADE || form.formDurum == (int)ENUMBelgeDurumu.ONAYLI)
                        {
                            TNS.BKD.MPRevizeForm mpRevizeForm = new BKD.MPRevizeForm();
                            mpRevizeForm.formDurum = (form.formDurum == (int)ENUMBelgeDurumu.ONAYLI) ? BKD.ENUMRevizeFormDurum.Onayli : BKD.ENUMRevizeFormDurum.Iptal;
                            mpRevizeForm.odGonNo = form.belgeNo;
                            mpRevizeForm.odGonTur = (int)form.belgeTur;
                            mpRevizeForm.odGonYil = form.yil;
                            tc.Add(mpRevizeForm);
                        }
                    }
                }
                bhmServis.MPRevizeFormDurumAta(kullanan, tc);

                TNS.KYM.IKYMServis servis = TNS.KYM.Arac.Tanimla();
                servis.KaydetMesajPanosu(kullanan, al.ToArray());

                if (hata != "")
                {
                    sonuc.hataStr = hata;
                    sonuc.islemSonuc = false;
                }
                else
                {
                    sonuc.hataStr = string.Empty;
                    sonuc.bilgiStr = "<br>" + ButceMuhasebeResI.MUH065;
                    sonuc.anahtar = icmalNo.ToString();
                    sonuc.islemSonuc = true;
                }
            }
            catch (Exception ex)
            {
                sonuc.DegerAta("Kayıt sırasında hata oluştu:" + ex.Message);
                ve.Rollback();
            }

            ve.VeriErisimKapat();
            return sonuc;
        }

        private GondermeTenkisIcmal MUHOdenekGondermeIcmal_Oku(VeriErisim ve, Kullanici kullanan, Hashtable gtiFormOkuHt, int yil, ENUMBelgeTuru belgeTur, int icmalNo)
        {
            string htAnahtar = yil.ToString() + "_" + ((int)belgeTur).ToString() + "_" + icmalNo.ToString();
            if (gtiFormOkuHt.ContainsKey(htAnahtar))
                return (GondermeTenkisIcmal)gtiFormOkuHt[htAnahtar];
            GondermeTenkisIcmal tut =  MUHOdenekGondermeIcmal.Oku(ve, kullanan, yil, belgeTur, icmalNo);
            gtiFormOkuHt[htAnahtar] = tut;
            return tut;
        }

        internal Sonuc TalepBelgeYapGonTen(Kullanici kullanan, TNSCollection formlar, bool sgbKbsKontrolYapma)
        {
            TNS.BKD.MPRevizeForm mpRevizeForm = new BKD.MPRevizeForm();
            Sonuc sonuc = new Sonuc(false, ButceMuhasebeResI.MUH052, "");
            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(kullanan));
            string hata = "";
            string islenen = "";
            foreach (GondermeTenkisForm form in formlar)
            {
                GondermeTenkisForm oS = TekBelgeGetir(ve, kullanan, form, true, false, true);
                if (oS.detay.ObjeSayisi > 0)
                {
                    oS.islemTarih.Yaz(DateTime.Now.Date);
                    oS.formDurum = (int)ENUMBelgeDurumu.YENI;
                    string talepYapan = oS.islemYapan;
                    oS.islemYapan = kullanan.kullaniciKodu;
                    Sonuc s = Kaydet(kullanan, oS, false, true, false, sgbKbsKontrolYapma, talepYapan, oS.belgeNo);
                    if (s.islemSonuc)
                    {
                        if (islenen != "") islenen += ",";
                        islenen += oS.belgeNo;
                    }
                    else
                    {
                        hata += form.belgeNo + " kaydedilirken hata oluştu:!" + s.hataStr;
                    }
                }
                else
                {
                    hata += form.belgeNo + " kodlu belgenin satırları okunamadı!";
                }
            }
            return new Sonuc(true, "", islenen + "<br>" + hata);
        }

        /// <summary>
        /// Tenkistens the MPR uret.
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="form">The form.</param>
        /// <returns></returns>
        public static Sonuc TenkistenMPRUret(Kullanici kullanan, VeriErisim ve, GondermeTenkisForm form, TNS.BHMv2.IBHMv2Servis bhmServis, bool tenkis, int revizeYil, int revizeNo)
        {
            if (form == null)
                return new Sonuc("form okunamadı");
            if (form.detay == null || form.detay.objeler.Count == 0)
                return new Sonuc("form detayı okunamadı");

            string belgeAd = tenkis ? "tenkis" : "ödenek gönderme";
            int revizeTertipSay = form.detay.objeler.Count;
            ArrayList revizeIslemSatirlar = new ArrayList();
            string oncKurKod = "";
            string kyKod = "";

            for (int i = 0; i < revizeTertipSay; i++)
            {
                GondermeTenkisDetay rs = (GondermeTenkisDetay)form.detay[i];

                if (oncKurKod != rs.tertip.kodlar[0])
                {
                    oncKurKod = rs.tertip.kodlar[0];
                    string kyHata;
                    kyKod = KurYapiBul(form.muhasebe.ToString(), form.birim.ToString(), oncKurKod, out kyHata, bhmServis);
                    if (!string.IsNullOrEmpty(kyHata))
                    {
                        return new Sonuc(string.Format("{0} numaralı belgeden, revize üretilemedi. Belgede belirtilen; saymanlık({1}), harcama birimi({2}) ve kurumsal kod({3}) için kurumsal yapı kodu bulunamadı.", form.belgeNo, form.muhasebe.ToString(), form.birim.ToString(), oncKurKod));
                    }
                }
                BKD.MPRevizeSatir rsD = new BKD.MPRevizeSatir();
                BKD.MPRevizeSatir rsE = new BKD.MPRevizeSatir();

                if (tenkis)
                {
                    rsD.tertip = new Tertip(rs.tertip.yil, ENUMTabloKod.TABMasrafPlani, rs.tertip.kodlar[0], rs.tertip.kodlar[1], rs.tertip.kodlar[2], rs.tertip.kodlar[3], rs.tertip.kodlar[4], kyKod);
                    rsE.tertip = new Tertip(rs.tertip.yil, ENUMTabloKod.TABMasrafPlani, rs.tertip.kodlar[0], rs.tertip.kodlar[1], rs.tertip.kodlar[2], rs.tertip.kodlar[3], rs.tertip.kodlar[4], "9");
                }
                else
                {
                    rsD.tertip = new Tertip(rs.tertip.yil, ENUMTabloKod.TABMasrafPlani, rs.tertip.kodlar[0], rs.tertip.kodlar[1], rs.tertip.kodlar[2], rs.tertip.kodlar[3], rs.tertip.kodlar[4], "9");
                    rsE.tertip = new Tertip(rs.tertip.yil, ENUMTabloKod.TABMasrafPlani, rs.tertip.kodlar[0], rs.tertip.kodlar[1], rs.tertip.kodlar[2], rs.tertip.kodlar[3], rs.tertip.kodlar[4], kyKod);
                }

                double kalan;
                string hata = MasrafPlaninaDagit(ve, rsD, Math.Abs(rs.miktar), out kalan, 12, false, false, null);
                if (!string.IsNullOrWhiteSpace(hata))
                    return new Sonuc(hata);
                if (OrtakFonksiyonlar.KurusYap(kalan) != 0)
                    return new Sonuc(string.Format("{0} tertibinde, {1} TL masraf planına dağıtılamadı!", rsD.tertip.TertipStrOl(), OrtakFonksiyonlar.KurusYap(kalan).ToString("#,##0")));
                for (int j = 0; j < rsD.odenek.Length; j++)
                {
                    rsE.odenek[j] = 0 - rsD.odenek[j];
                }

                revizeIslemSatirlar.Add(rsD);
                revizeIslemSatirlar.Add(rsE);
            }
            TNS.BKD.MPRevizeForm mpRevizeForm = new BKD.MPRevizeForm();

            mpRevizeForm.yil = revizeYil > 0 ? revizeYil : form.yil;
            mpRevizeForm.revizeNo = revizeNo;
            mpRevizeForm.revizeFormTip = BKD.ENUMRevizeFormTip.MasrafPlaniRevize;
            mpRevizeForm.onaylayanKullaniciKodu = kullanan.kullaniciKodu;
            mpRevizeForm.onaylamaTarihi = new TNSDateTime(DateTime.Now);
            mpRevizeForm.kayitTarihi = new TNSDateTime(DateTime.Now);
            mpRevizeForm.kaydedenKullaniciKodu = kullanan.kullaniciKodu;

            if (revizeNo > 0)
                mpRevizeForm.formDurum = BKD.ENUMRevizeFormDurum.Onayli;
            else
                mpRevizeForm.formDurum = BKD.ENUMRevizeFormDurum.Taslak;

            mpRevizeForm.evrakTarih = new TNSDateTime(DateTime.Now);
            mpRevizeForm.aciklama = form.belgeNo + " numaralı " + belgeAd + " onaylanırken üretildi.";
            mpRevizeForm.satirlar = (TNS.BKD.MPRevizeSatir[])revizeIslemSatirlar.ToArray(typeof(TNS.BKD.MPRevizeSatir));

            mpRevizeForm.odGonNo = form.belgeNo;
            mpRevizeForm.odGonTur = (int)form.belgeTur;
            mpRevizeForm.odGonYil = form.yil;

            return bhmServis.MPRevizeFormKaydet(kullanan, mpRevizeForm, false);
        }

        /// <summary>
        /// Ödenek Gönderme/tenkis kontrol listesi
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">İşlem yapılacak bilgilerin taşındığı nesne</param>
        /// <returns></returns>
        //public ObjectArray KontrolListesi(Kullanici kullanan, GondermeTenkisForm form)
        //{
        //    VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(kullanan));

        //    TABGondermeTenkis to = new TABGondermeTenkis(ve);

        //    //Listeleme sırasında kurum kullanıcıları kendilerine ait evrakları görsün
        //    if (!(kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.SISTEMYONETICISI) ||
        //        kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.KULLANICIYONETICISI)))
        //    {
        //        for (int i = 0; i < form.detay.objeler.Count; i++)
        //        {
        //            GondermeTenkisDetay d = (GondermeTenkisDetay)form.detay[i];

        //            string kur = ((string)d.tertip.kodlar[0]).Replace(".", "");
        //            string fon = ((string)d.tertip.kodlar[1]).Replace(".", "");
        //            string fin = ((string)d.tertip.kodlar[2]).Replace(".", "");
        //            string eko = ((string)d.tertip.kodlar[3]).Replace(".", "");

        //            string kurumlar = TNS.MUH.Yetki.OdemeEmriMIFKurumlariVer(kullanan);
        //            if (kur != "")
        //            {
        //                if (kurumlar != "")
        //                    kurumlar += ";" + kur;
        //                else
        //                    kurumlar = kur;
        //            }

        //            GondermeTenkisDetay dt = new GondermeTenkisDetay();
        //            dt.tertip.kodlar[0] = kurumlar;
        //            dt.tertip.kodlar[1] = fon;
        //            dt.tertip.kodlar[2] = fin;
        //            dt.tertip.kodlar[3] = eko;

        //            form.detay.objeler.Clear();
        //            form.detay.objeler.Add(dt);
        //        }
        //    }
        //    Tertip tUzBul = new Tertip(form.yil, ENUMTabloKod.TABPEBOdenek);

        //    IDataReader r = to.FormListe(kullanan, form, 1, form.kontrolTur, tUzBul.KodUzunlukBul(3, 2, true));

        //    ObjectArray gVAN = new ObjectArray();

        //    if (to.Hata() == "")
        //    {
        //        Tertip tt = new Tertip(DateTime.Now.Year, ENUMTabloKod.TABOdenek);
        //        while (r.Read())
        //        {
        //            GondermeTenkisKontrolListesi os = new GondermeTenkisKontrolListesi();

        //            os.yil = VeriErisim.DegerAl(r, 0, 0);
        //            os.belgeTur = (ENUMBelgeTuru)VeriErisim.DegerAl(r, 1, 0);
        //            os.belgeNo = VeriErisim.DegerAl(r, 2, "");
        //            os.kurum = VeriErisim.DegerAl(r, 3, "");
        //            os.muhasebe = VeriErisim.DegerAl(r, 4, 0);
        //            os.birim = VeriErisim.DegerAl(r, 5, "");
        //            os.islemTarih = VeriErisim.DegerAl(r, 7);
        //            os.formDurumTarih = VeriErisim.DegerAl(r, 8);
        //            os.formDurum = VeriErisim.DegerAl(r, 9, 0);
        //            os.icmalNo = VeriErisim.DegerAl(r, 11, 0);
        //            os.icmalTarih = VeriErisim.DegerAl(r, 12);
        //            os.onayNo = VeriErisim.DegerAl(r, 13, "");
        //            os.onayTarih = VeriErisim.DegerAl(r, 14);
        //            os.islemYapan = VeriErisim.DegerAl(r, 19, "");

        //            os.kurumAd = VeriErisim.DegerAl(r, 24, "");
        //            os.muhasebeAd = VeriErisim.DegerAl(r, 25, "");
        //            os.birimAd = VeriErisim.DegerAl(r, 26, "");
        //            os.vergiNo = VeriErisim.DegerAl(r, 27, "");
        //            os.tertip.kodlar[0] = OrtakFonksiyonlar.KodNoktaKoy((string)tt.Kirilimlar(0), VeriErisim.DegerAl(r, 29, ""));
        //            os.tertip.kodlar[1] = OrtakFonksiyonlar.KodNoktaKoy((string)tt.Kirilimlar(1), VeriErisim.DegerAl(r, 30, ""));
        //            os.tertip.kodlar[2] = OrtakFonksiyonlar.KodNoktaKoy((string)tt.Kirilimlar(2), VeriErisim.DegerAl(r, 31, ""));
        //            os.tertip.kodlar[3] = OrtakFonksiyonlar.KodNoktaKoy((string)tt.Kirilimlar(3), VeriErisim.DegerAl(r, 32, ""));
        //            os.miktar = Math.Abs(VeriErisim.DegerAl(r, 33, 0.0));
        //            os.kurum = OrtakFonksiyonlar.KodNoktaKoy((string)tt.Kirilimlar(0), os.kurum);
        //            gVAN.objeler.Add(os);
        //        }
        //        r.Close();
        //    }
        //    ve.VeriErisimKapat();

        //    return gVAN;
        //}

        internal static Sonuc BIBagKopart(Kullanici kullanan, int revizeYil, int revizeNo)
        {
            if (revizeYil <= 0 || revizeNo <= 0)
            {
                return new Sonuc("Revize numarası verilmelidir.!");
            }
            if (!kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.SISTEMYONETICISI))
            {
                return new Sonuc("Bu işlemi yapmak için sistem yöneticisi olmanız gerekir!");
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(kullanan));

            TABGondermeTenkis tab = new TABGondermeTenkis(ve);

            Sonuc s = tab.BIBagKopart(revizeYil, revizeNo);

            ve.VeriErisimKapat();
            return s;
        }

        internal static Sonuc BIDuzelt(Kullanici kullanan, MPRevizeForm f)
        {
            if (string.IsNullOrWhiteSpace(f.odGonNo) || f.odGonTur <= 0 || f.odGonYil <= 0)
            {
                return new Sonuc("Gönderme/Tenkis numarası verilmelidir.!");
            }
            if (!kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.SISTEMYONETICISI))
            {
                return new Sonuc("Bu işlemi yapmak için sistem yöneticisi olmanız gerekir!");
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(kullanan));

            GondermeTenkisForm gf = new GondermeTenkisForm();
            gf.yil = f.odGonYil;
            gf.belgeTur = (ENUMBelgeTuru)f.odGonTur;
            gf.belgeNo = f.odGonNo;

            MUHGondermeTenkisServis servis = new MUHGondermeTenkisServis();
            Sonuc s = new Sonuc();
            GondermeTenkisForm gfOkunan = servis.TekBelgeGetir(ve, kullanan, gf, true, false, false);

            if (gfOkunan.formDurum == (int)ENUMBelgeDurumu.IPTAL)
            {
                if (f.yil <= 0 || f.revizeNo <= 0)
                {
                    s.DegerAta("Revize numarası verilmelidir.!");
                }
            }
            else if (gfOkunan.formDurum != (int)ENUMBelgeDurumu.ONAYLI)
            {
                s.DegerAta("İşlem yapılabilmesi için belgenin onaylı veya iptal olması gerekir.");
            }

            if (s.islemSonuc)
            {
                TABGondermeTenkis tab = new TABGondermeTenkis(ve);

                ve.BeginTransaction();
                if (f.revizeNo > 0)
                {
                    s = tab.BIIptalEt(f.yil, f.revizeNo);
                    if (s.islemSonuc)
                        ve.Commit();
                    else
                        ve.Rollback();
                }

                if (s.islemSonuc && gfOkunan.formDurum == (int)ENUMBelgeDurumu.ONAYLI)
                {
                    s = MUHGondermeTenkisServis.TenkistenMPRUret(kullanan, ve, gfOkunan, TNS.BHMv2.Arac.Tanimla(), gfOkunan.belgeTur == ENUMBelgeTuru.TENKIS, f.yil, f.revizeNo);
                }
            }
            ve.VeriErisimKapat();
            return s;
        }

        //public GondermeTenkisSayBirimVAN SaymanlıkDurum(Kullanici kullanan, GondermeTenkisSayBirim form)
        //{
        //    VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(kullanan));
        //    IDataReader hr = null;
        //    TABGondermeTenkis to = new TABGondermeTenkis(ve);

        //    //Listeleme sırasında kurum kullanıcıları kendilerine ait evrakları görsün
        //    if (!(kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.SISTEMYONETICISI) ||
        //        kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.KULLANICIYONETICISI)))
        //    {
        //        for (int i = 0; i < form.detay.Count; i++)
        //        {
        //            GondermeTenkisDetay d = (GondermeTenkisDetay)form.detay[i];

        //            string kur = ((string)d.tertip.kodlar[0]).Replace(".", "");
        //            string fon = ((string)d.tertip.kodlar[1]).Replace(".", "");
        //            string fin = ((string)d.tertip.kodlar[2]).Replace(".", "");
        //            string eko = ((string)d.tertip.kodlar[3]).Replace(".", "");

        //            string kurumlar = MUHYetkiServis.OdemeEmriMIFKurumlariVer(kullanan);
        //            if (kur != "")
        //            {
        //                if (kurumlar != "")
        //                    kurumlar += ";" + kur;
        //                else
        //                    kurumlar = kur;
        //            }

        //            GondermeTenkisDetay dt = new GondermeTenkisDetay();
        //            dt.tertip.kodlar[0] = kurumlar;
        //            dt.tertip.kodlar[1] = fon;
        //            dt.tertip.kodlar[2] = fin;
        //            dt.tertip.kodlar[3] = eko;

        //            form.detay.Clear();
        //            form.detay.Add(dt);
        //        }
        //    }
        //    string saymanlik = "";
        //    string birim = "";
        //    foreach (string saybirim in form.saymanlikBirim)
        //    {

        //        if (saybirim != "")
        //        {
        //            string[] saybirimler = saybirim.Split('-');

        //            saymanlik = saybirimler[0].Trim();
        //            birim = saybirimler[1].Trim();
        //        }

        //    }
        //    IDataReader r = to.SaymanlikBirimBelgeListe(form);

        //    GondermeTenkisSayBirimVAN gVAN = new GondermeTenkisSayBirimVAN();

        //    if (to.Hata() == "")
        //    {
        //        Tertip tt = new Tertip(DateTime.Now.Year, ENUMTabloKod.TABOdenek);
        //        Tertip t = new Tertip(DateTime.Now.Year, ENUMTabloKod.TABOdenek);

        //        while (r.Read())
        //        {
        //            GondermeTenkisSayBirim os = new GondermeTenkisSayBirim();

        //            os.yil = VeriErisim.DegerAl(r, 0, 0);
        //            os.tertip.kodlar[0] = OrtakFonksiyonlar.KodNoktaKoy((string)tt.Kirilimlar(0), VeriErisim.DegerAl(r, 1, ""));
        //            os.tertip.kodlar[1] = OrtakFonksiyonlar.KodNoktaKoy((string)tt.Kirilimlar(1), VeriErisim.DegerAl(r, 2, ""));
        //            os.tertip.kodlar[2] = OrtakFonksiyonlar.KodNoktaKoy((string)tt.Kirilimlar(2), VeriErisim.DegerAl(r, 3, ""));
        //            os.tertip.kodlar[3] = OrtakFonksiyonlar.KodNoktaKoy((string)tt.Kirilimlar(3), VeriErisim.DegerAl(r, 4, ""));
        //            os.odenekToplami = Math.Abs(VeriErisim.DegerAl(r, 5, 0.0));
        //            /*
        //            if (os.birim != "")
        //                os.birimAd = MUHServis.UzayYaziDegerGetir(null, "MUHMUHASEBEBIRIM", os.birim + "-" + os.muhasebe.ToString(), false, "BIRIMAD");

        //            os.kurum = OrtakFonksiyonlar.KodNoktaKoy((string)t.Kirilimlar(0), os.kurum);
        //            */

        //            OdenekDurumKriter kr = new OdenekDurumKriter();
        //            TABGondermeTenkis gonTen = new TABGondermeTenkis(ve);
        //            kr.yil = form.yil;
        //            kr.tertip.TertipAta(os.tertip);
        //            kr.odenekGondermeGoster = true;
        //            kr.birim = birim;
        //            kr.muhasebe = Convert.ToInt32(saymanlik);
        //            VeriErisim ve2 = VeriErisim.VeriErisimBul(null, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(kullanan));
        //            hr = gonTen.GondermeTenkisToplam(kr, (int)ENUMBelgeTuru.TENKIS);
        //            if (gonTen.Hata() == "")
        //            {
        //                while (hr.Read())
        //                {
        //                    double miktar = Math.Abs(VeriErisim.DegerAl(hr, 0, 0.0));
        //                    os.tenkisToplami = miktar;
        //                }
        //            }

        //            hr.Close();


        //            double harcamaToplam = 0;

        //            TABOdenekHarcamaSorgu harcama = new TABOdenekHarcamaSorgu(ve2);
        //            hr = harcama.AvansListe(kr);
        //            if (harcama.Hata() == "")
        //            {
        //                while (hr.Read())
        //                    harcamaToplam = VeriErisim.DegerAl(hr, 0, 0.0);
        //                hr.Close();
        //            }
        //            else
        //            {
        //                gVAN.sonuc.hataStr = harcama.Hata();
        //                return gVAN;
        //            }

        //            os.avansToplami = harcamaToplam;
        //            gVAN.liste.Add(os);
        //            ve2.VeriErisimKapat();
        //        }
        //    }

        //    r.Close();

        //    ve.VeriErisimKapat();

        //    return gVAN;
        //}

        //private static void HataKaydet(ObjectArray donusDeger, int yil, int belgeTur, string belgeno, int bumkoDurum, int muhDurum, TNSDateTime durumTarihi, string onayNo, int hataKod, string hata)
        //{
        //    donusDeger.Add(yil);
        //    donusDeger.Add(belgeTur);
        //    donusDeger.Add(belgeno);
        //    donusDeger.Add(bumkoDurum);
        //    donusDeger.Add(muhDurum);
        //    donusDeger.Add(durumTarihi);
        //    donusDeger.Add(onayNo);
        //    donusDeger.Add(hataKod);
        //    donusDeger.Add(hata);
        //}

        //static ObjectArray HataliKayitlarBul(Kullanici kullanan, VeriErisim veriErisim, int yil, string kurum)
        //{
        //    ObjectArray donusDeger = new ObjectArray();
        //    VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(kullanan));
        //    VeriErisim ve2 = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriBUM");
        //    TABGondermeTenkis tg = new TABGondermeTenkis(ve2);
        //    TABMuhasebatOdenekGonderme t = new TABMuhasebatOdenekGonderme(ve);
        //    IDataReader r = tg.GondermeTenkisTumFormlar(yil, kurum);
        //    int belgeTur;
        //    string belgeno;

        //    int muhDurum, bumkoDurum;
        //    TNSDateTime durumTarihi;
        //    string onayNo;

        //    if (r != null)
        //    {
        //        while (r.Read())
        //        {
        //            belgeTur = VeriErisim.DegerAl(r, 1, 0);
        //            belgeno = VeriErisim.DegerAl(r, 2, "");
        //            bumkoDurum = VeriErisim.DegerAl(r, 9, 0);
        //            t.BelgeGetir(yil, belgeTur, BastakiSifirlariAt(belgeno), out muhDurum, out durumTarihi, out onayNo);
        //            if (muhDurum == 7)
        //                muhDurum = 5;
        //            else if (muhDurum == 6)
        //                muhDurum = 8;
        //            if (!(muhDurum == 4 || muhDurum == 5 || muhDurum == 8 || muhDurum == -2))
        //            {
        //                HataKaydet(donusDeger, yil, belgeTur, belgeno, bumkoDurum, muhDurum, durumTarihi, onayNo, 9, "Hatalı muhasebat durumu!");
        //            }
        //            else if (!(bumkoDurum == 1 || bumkoDurum == 2 || bumkoDurum == 3 || bumkoDurum == 4 || bumkoDurum == 5 || bumkoDurum == 8 || bumkoDurum == 9))
        //            {
        //                HataKaydet(donusDeger, yil, belgeTur, belgeno, bumkoDurum, muhDurum, durumTarihi, onayNo, 9, "Hatalı bümko durumu!");
        //            }
        //            else if (((bumkoDurum == 1 || bumkoDurum == 2 || bumkoDurum == 9) && (muhDurum != -2)))
        //            {
        //                HataKaydet(donusDeger, yil, belgeTur, belgeno, bumkoDurum, muhDurum, durumTarihi, onayNo, 9, "Muhasebatta olmaması gereken kayıt!");
        //            }
        //            else if ((bumkoDurum == 3) && (muhDurum != -2))
        //            {
        //                HataKaydet(donusDeger, yil, belgeTur, belgeno, bumkoDurum, muhDurum, durumTarihi, onayNo, 1, "");
        //            }
        //            else if ((bumkoDurum == 4) && (muhDurum == -2))
        //            {
        //                HataKaydet(donusDeger, yil, belgeTur, belgeno, bumkoDurum, muhDurum, durumTarihi, onayNo, 2, "zışın durumu");
        //            }
        //            else if ((bumkoDurum == 4) && (muhDurum != 4))
        //            {
        //                HataKaydet(donusDeger, yil, belgeTur, belgeno, bumkoDurum, muhDurum, durumTarihi, onayNo, 1, "");
        //            }
        //            else if ((bumkoDurum == 5) && (muhDurum != 5))
        //            {
        //                if (muhDurum == -2)
        //                    HataKaydet(donusDeger, yil, belgeTur, belgeno, bumkoDurum, 9, durumTarihi, onayNo, 2, "zışın durumu");
        //                else
        //                    HataKaydet(donusDeger, yil, belgeTur, belgeno, bumkoDurum, muhDurum, durumTarihi, onayNo, 1, "Bizde muhasebat onaylı, muhasebatta farklı");
        //            }
        //            else if ((bumkoDurum == 8) && (muhDurum != 8))
        //            {
        //                if (muhDurum == -2)
        //                    HataKaydet(donusDeger, yil, belgeTur, belgeno, bumkoDurum, 9, durumTarihi, onayNo, 2, "zışın durumu");
        //                else
        //                    HataKaydet(donusDeger, yil, belgeTur, belgeno, bumkoDurum, muhDurum, durumTarihi, onayNo, 1, "Bizde muhasebat iade, muhasebatta farklı");
        //            }

        //        }
        //        r.Close();
        //    }
        //    //Console.WriteLine(durum);

        //    ve.VeriErisimKapat();
        //    ve2.VeriErisimKapat();
        //    return donusDeger;
        //}


        //static string BastakiSifirlariAt(string s)
        //{
        //    while (s.StartsWith("0"))
        //        s = s.Remove(0, 1);
        //    return s;

        //}

        //static void HataliKayitlariDuzelt(ObjectArray hatalar, string dosyaAd)
        //{
        //    System.IO.StreamWriter sw = new System.IO.StreamWriter(dosyaAd);
        //    int yil;
        //    int belgeTur;
        //    string belgeno;
        //    int bumkoDurum;
        //    int muhDurum;
        //    TNSDateTime durumTarihi;
        //    string onayNo;
        //    int hataKod;//, yeniDurum;
        //    string hata, mesaj;
        //    VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(kullanan));
        //    VeriErisim ve2 = VeriErisim.VeriErisimBul(null, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(kullanan));
        //    TABGondermeTenkis tab = new TABGondermeTenkis(ve);
        //    GondermeTenkisForm f = new GondermeTenkisForm();
        //    for (int i = 0; i < hatalar.Count; )
        //    {

        //        yil = (int)hatalar[i++];
        //        belgeTur = (int)hatalar[i++];
        //        belgeno = (string)hatalar[i++];
        //        bumkoDurum = (int)hatalar[i++];
        //        muhDurum = (int)hatalar[i++];
        //        durumTarihi = (TNSDateTime)hatalar[i++];
        //        onayNo = (string)hatalar[i++];
        //        hataKod = (int)hatalar[i++];
        //        hata = (string)hatalar[i++];
        //        mesaj = "";
        //        /*
        //        if (hataKod == 1)
        //        {
        //            if (hataKod == 2) yeniDurum = TarihceDurumBul(ve2,yil, belgeTur, belgeno);
        //            else
        //                yeniDurum = muhDurum;
        //            if (yeniDurum > 0)
        //            {
        //                f.yil = yil;
        //                f.belgeTur = (ENUMBelgeTuru) belgeTur;
        //                f.belgeNo = belgeno;
        //                f.formDurum = yeniDurum;
        //                f.islemYapan = "Sistem";
        //                ve.BeginTransaction();
        //                Sonuc s;
        //                s=tab.BelgeDurumDegistir(f);
        //                if (s.islemSonuc)
        //                {
        //                    mesaj = yeniDurum.ToString() + " olarak değiştirildi";
        //                    ve.Commit();
        //                    ve.hata = "";
        //                }
        //                else
        //                {
        //                    mesaj = s.hataStr.ToString();
        //                    ve.Rollback();
        //                }
        //            }
        //            else
        //                mesaj = "Tarihçesi Bulunamadı...";
        //        }
        //        else
        //            mesaj = "";
        //         * */
        //        sw.Write(yil);
        //        sw.Write("\t");
        //        sw.Write(belgeTur);
        //        sw.Write("\t");
        //        sw.Write(belgeno);
        //        sw.Write("\t");
        //        sw.Write(bumkoDurum);
        //        sw.Write("\t");
        //        sw.Write(muhDurum);
        //        sw.Write("\t");
        //        if (durumTarihi != null)
        //            sw.Write(durumTarihi.ToString());
        //        sw.Write("\t");
        //        sw.Write(onayNo);
        //        sw.Write("\t");
        //        sw.Write(hataKod);
        //        sw.Write("\t");
        //        sw.Write(hata);
        //        sw.Write("\t");
        //        sw.WriteLine(mesaj);

        //    }
        //    sw.Close();
        //}

        //private static int TarihceDurumBul(VeriErisim ve, int yil, int belgeTur, string belgeno)
        //{
        //    TABTarihce tab = new TABTarihce(ve);
        //    string islemyapan = "";
        //    string islem = "";
        //    IDataReader r = tab.Listele(yil, (ENUMBelgeTuru)belgeTur, belgeno);
        //    if (r != null)
        //    {
        //        while (r.Read())
        //        {
        //            islemyapan = VeriErisim.DegerAl(r, 3, "");
        //            if (!islemyapan.StartsWith("Saymanlık"))
        //                islem = VeriErisim.DegerAl(r, 5, "");
        //        }

        //        r.Close();
        //    }
        //    if (islem == "Yeni Giriş")
        //        return 1;
        //    else if (islem == "Muhasebata Gönderildi")
        //        return 3;
        //    else if (islem == "Değiştirildi")
        //        return 2;
        //    else if (islem == "İptal Edildi")
        //        return 9;
        //    else return -1;

        //}
    }
}
