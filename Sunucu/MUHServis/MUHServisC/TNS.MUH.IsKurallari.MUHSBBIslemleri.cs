using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TNS.BKD;
using TNS.KYM;
using TNS.MUH.IsKurallari.Veri;
using TNS.Veri;
using System.Collections;
using MUHServisC.tr.gov.sbb.programbutce;
using TNS.BHMv2;
using TNS.BUM;

namespace TNS.MUH.IsKurallari
{
    public class MUHSBBIslemleri
    {
        public class GonderimSayac
        {
            int bekleyen = 0;
            //private string log = "";

            public GonderimSayac()
            {
                bekleyen = 0;
            }

            internal void IslemBasladi()
            {
                Bekle(5);
                bekleyen++;
            }

            internal void IslemBitti()
            {
                bekleyen--;
            }

            internal void Bekle(int say)
            {
                //while (bekleyen > say)
                //    Thread.Sleep(100);
            }
        }

        public class GondermeIslemleri
        {
            private Kullanici kullanan;
            private string kurumKod;
            private GondermeTenkisFormW os;
            private int mpDurum;
            private int i;
            GonderimSayac gSayac;
            //private bool bitti = false;
            string islemId;
            private string log = "";
            private Tertip ty;

            public GondermeIslemleri(Kullanici kullanan, string kurumKod, GondermeTenkisFormW os, int mpDurum, int i, GonderimSayac gSayac, string islemId, Tertip ty)
            {
                this.ty = ty;
                this.kullanan = kullanan;
                this.kurumKod = kurumKod;
                this.os = os;
                this.mpDurum = mpDurum;
                this.i = i;
                this.gSayac = gSayac;
                this.islemId = islemId;
            }

            private void Logla(string log)
            {
                TNS.MUH.IsKurallari.MUHServis.AktarimLogYaz(log);
                //if (this.log != "") this.log += "\r\n";
                //this.log += log;
            }

            internal void Cagir()
            {
                TNS.MUH.IsKurallari.MUHProgramButceIslemleri.EButceParolaBilgileri();
                //Logla("Sayaç başla: " + i + "-" + os.yil + "/" + os.belgeTur.ToString() + "/" + os.belgeNo + "/" + os.muhasebe + "/" + os.vkn + " Gönderiliyor");
                gSayac.IslemBasladi();
                try
                {
                    //servisMUH.OdenekGondermeKaydetCompleted += new MUHServisC.ebutce.OdenekTenkis.OdenekGondermeKaydetCompletedEventHandler(SonucMetot);
                    //new EbutceCevap(kullanan, os, kurumKod, mpDurum);
                    //servisMUH.OdenekGondermeKaydetAsync(ebutceUser, ebutcePassword, os);
                    os.kurum = Cevir(os.yil, 1, os.kurum, ty);
                    MUHServisC.tr.gov.sbb.programbutce.Sonuc sIc = servisSBB.OdenekGondermeKaydet(MUHProgramButceIslemleri.ebutceUser, MUHProgramButceIslemleri.ebutcePassword, os);
                    SonucIsle(sIc);
                }
                catch (Exception ex)
                {
                    Logla("Exception: " + i + "-" + os.yil + "/" + os.belgeTur.ToString() + "/" + os.belgeNo + "/" + os.muhasebe + "/" + os.vkn + " Ex:" + ex.Message + ex.StackTrace + StrYap(ex.InnerException));
                }
            }

            private string StrYap(Exception innerException)
            {
                if (innerException == null)
                    return "innerException null";
                else
                    return "innerException:" + innerException.Message + innerException.StackTrace;
            }

            private void SonucIsle(MUHServisC.tr.gov.sbb.programbutce.Sonuc sonucKayit)
            {
                if (!sonucKayit.islemSonuc)
                {
                    if (sonucKayit.hataStr.Contains("Bu belge daha önce Muhasebat tarafından onaylandığı"))
                        BelgeDurumDegistir(kullanan, os.yil, os.belgeTur, kurumKod, BelgeNoYap(os.yil, os.belgeNo, kurumKod), (int)ENUMBelgeDurumu.ONAYLI, mpDurum);
                    else if (sonucKayit.hataStr.Contains("tertibin kullanabilirliğinden"))
                        BelgeDurumDegistir(kullanan, os.yil, os.belgeTur, kurumKod, BelgeNoYap(os.yil, os.belgeNo, kurumKod), (int)ENUMBelgeDurumu.IPTAL, mpDurum, sonucKayit.hataStr);
                    else if (sonucKayit.hataStr.Contains("tertibin serbestliğinden"))
                        BelgeDurumDegistir(kullanan, os.yil, os.belgeTur, kurumKod, BelgeNoYap(os.yil, os.belgeNo, kurumKod), (int)ENUMBelgeDurumu.IPTAL, mpDurum, sonucKayit.hataStr);
                    else if (sonucKayit.hataStr.Contains("gönderildiği için belge üzerinde"))
                        BelgeDurumDegistir(kullanan, os.yil, os.belgeTur, kurumKod, BelgeNoYap(os.yil, os.belgeNo, kurumKod), (int)ENUMBelgeDurumu.MUHASEBATALDI, mpDurum);
                    else if (sonucKayit.hataStr.Contains("Muhasebat tarafından iade edildiği için belge "))
                        BelgeDurumDegistir(kullanan, os.yil, os.belgeTur, kurumKod, BelgeNoYap(os.yil, os.belgeNo, kurumKod), (int)ENUMBelgeDurumu.MUHASEBATIADE, mpDurum);
                    else
                    {
                        BelgeDurumDegistir(kullanan, os.yil, os.belgeTur, kurumKod, BelgeNoYap(os.yil, os.belgeNo, kurumKod), (int)ENUMBelgeDurumu.YENI, mpDurum, sonucKayit.hataStr);
                        Logla("Hata:" + os.yil + "/" + os.belgeTur.ToString() + "/" + os.belgeNo + "/" + os.muhasebe + "/" + os.vkn + "/" + islemId + " Hata:" + sonucKayit.hataStr + " Bilgi:" + sonucKayit.bilgiStr);
                        for (int i = 0; i < os.detay.Length; i++)
                        {
                            TNS.MUH.IsKurallari.MUHServis.AktarimLogYaz($"      {os.detay[i].tertip} : {os.detay[i].miktar}");
                        }
                    }
                }
                else
                {
                    BelgeDurumDegistir(kullanan, os.yil, os.belgeTur, kurumKod, BelgeNoYap(os.yil, os.belgeNo, kurumKod), (int)ENUMBelgeDurumu.MUHASEBATALDI, mpDurum);
                }
                //Logla("Sayaç bitir: " + i + "-" + os.yil + "/" + os.belgeTur.ToString() + "/" + os.belgeNo + "/" + os.muhasebe + "/" + os.vkn + "/" + islemId + " Hata");
                gSayac.IslemBitti();
                //TNS.MUH.IsKurallari.MUHServis.AktarimLogYaz(log);
            }
        }

        static MUHServisC.tr.gov.sbb.programbutce.OdenekGondermeTenkisServis servisSBB = new MUHServisC.tr.gov.sbb.programbutce.OdenekGondermeTenkisServis();

        static MUHSBBIslemleri()
        {
            // .NET 8: ServicePointManager.CertificatePolicy yok (eski kod her sertifikayi kabul ediyordu); sertifikalar normal dogrulanir.
            servisSBB.Proxy = OrtakFonksiyonlar.ProxyAyarla();
            servisSBB.Timeout = 20 * 60 * 1000;
        }

        static private void BelgeDurumDegistir(Kullanici kullanan, int yil, MUHServisC.tr.gov.sbb.programbutce.EnumodenekIslemTuruW belgeTur, string kurum, string belgeNo, int formDurum, int mpDurum, string hataStr = null)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABGondermeTenkis to = new TABGondermeTenkis(ve);
            GondermeTenkisForm guncelDurum = new GondermeTenkisForm();
            guncelDurum.yil = yil;
            guncelDurum.belgeTur = (ENUMBelgeTuru)((int)belgeTur);// + 1;
            if (!(guncelDurum.belgeTur == ENUMBelgeTuru.ODENEKGONDERME || guncelDurum.belgeTur == ENUMBelgeTuru.TENKIS))
            {
                TNS.MUH.IsKurallari.MUHServis.AktarimLogYaz($"Belge tür bulunamadı: belgeTur {belgeTur}, int belgetur:{(int)belgeTur}, dönüşen:{guncelDurum.belgeTur}");
                return;
            }
            guncelDurum.belgeNo = belgeNo;
            guncelDurum.kurum = kurum;
            guncelDurum.formDurum = formDurum;
            guncelDurum.onayNo = "";
            if (hataStr != null)
            {
                if (hataStr.Length > 250) hataStr = hataStr.Substring(0, 250);
                guncelDurum.iptalNeden = hataStr;
            }
            guncelDurum.onayTarih = new TNSDateTime(DateTime.Now);
            guncelDurum.islemYapan = "uygulama";
            ve.hata = "";
            ve.BeginTransaction();
            Sonuc s = to.BelgeDurumDegistir(guncelDurum);
            TNSCollection tc = new TNSCollection();
            if (s.islemSonuc)
            {
                ve.Commit();

                if (mpDurum == 5 && (formDurum >= 5))
                {
                    BKD.MPRevizeForm mpRevizeForm = new BKD.MPRevizeForm();
                    mpRevizeForm.formDurum = formDurum == 9 ? BKD.ENUMRevizeFormDurum.Iptal : BKD.ENUMRevizeFormDurum.Onayli;
                    mpRevizeForm.odGonNo = guncelDurum.belgeNo;
                    mpRevizeForm.odGonTur = (int)guncelDurum.belgeTur;
                    mpRevizeForm.odGonYil = guncelDurum.yil;
                    tc.Add(mpRevizeForm);
                }
            }
            else
            {
                ve.Rollback();
            }
            ve.VeriErisimKapat();
            TNS.BHMv2.IBHMv2Servis bhmServis = TNS.BHMv2.Arac.Tanimla();
            bhmServis.MPRevizeFormDurumAta(kullanan, tc);
        }

        public static void ProgramButceyeGonder(TNSCollection gonderilecekler, Kullanici kullanan, string kurumKod, int mpDurum, string islemId, Tertip ty)
        {
            GonderimSayac gSayac = new GonderimSayac();
            if (gonderilecekler != null)
            {
                for (int i = 0; i < gonderilecekler.Count; i++)
                {
                    GondermeTenkisFormW os = (GondermeTenkisFormW)gonderilecekler[i];
                    if (os != null && os.yil != 0)
                    {
                        new GondermeIslemleri(kullanan, kurumKod, os, mpDurum, i, gSayac, islemId, ty).Cagir();
                    }
                }
            }
            gSayac.Bekle(0);
        }

        public void programButceOdenekGondermeGonder(Kullanici kullanan, int yil)
        {
            programButceOdenekGondermeBulGonder(kullanan, yil, ENUMBelgeTuru.TANIMSIZ, null);
        }

        public void programButceOdenekGondermeBulGonder(Kullanici kullanan, int yil, ENUMBelgeTuru belgeTur, string belgeNoListe)
        {
            TNS.BHMv2.IBHMv2Servis bhmServis = TNS.BHMv2.Arac.Tanimla();
            int mpDurum = bhmServis.MasrafPlaniGenelOnayDurumBul(kullanan, yil, "", BHMv2.ENUMMasrafPlanAsama.AHP);
            string kurumKod = TNS.MUH.Yetki.VarsayilanKurumBul();

            string islemId = MUHServis.ZamanBul();
            TNSCollection gonderilenBelgeler = new TNSCollection();
            TNSCollection gonderilecekler = GonderilecekleriBul(kullanan, yil, belgeTur, belgeNoListe, islemId, gonderilenBelgeler);

            ProgramButceyeGonder(gonderilecekler, kullanan, kurumKod, mpDurum, islemId, new Tertip(yil, ENUMTabloKod.TABPEBOdenek));
            if (gonderilenBelgeler.Count > 0)
            {
                Tertip tUzBul = new Tertip(yil, ENUMTabloKod.TABPEBOdenek);
                ListeyiKontrolEt(kullanan, yil, (int)ENUMBelgeDurumu.MUHASEBATALDI, gonderilenBelgeler, tUzBul);
            }
        }

        public TNSCollection GonderilecekleriBul(Kullanici kullanan, int yil, ENUMBelgeTuru belgeTur, string belgeNolar, string islemId, TNSCollection gonderilenBelgeler)
        {
            TNS.MUH.IsKurallari.MUHServis.AktarimLogYaz($"{yil} için liste çekiliyor... işlem id:{islemId}, belge kodları:{belgeNolar}");
            TNSCollection okunanBelgeler = new TNSCollection();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(null));
            try
            {
                TABGondermeTenkis to = new TABGondermeTenkis(ve);

                GondermeTenkisForm sorguForm = new GondermeTenkisForm();
                sorguForm.yil = yil;
                sorguForm.kurum = TNS.MUH.Yetki.VarsayilanKurumBul();
                if (string.IsNullOrWhiteSpace(belgeNolar))
                {
                    sorguForm.formDurum = (int)ENUMBelgeDurumu.MUHASEBATAGONDERILDI;
                    sorguForm.belgeTur = ENUMBelgeTuru.TANIMSIZ;
                }
                else
                {
                    sorguForm.belgeTur = belgeTur;
                    sorguForm.belgeNo = belgeNolar;
                }
                int pebVar;
                if (yil >= 2021)
                    pebVar = 2;
                else
                    pebVar = 1;

                Tertip tUzBul = new Tertip(sorguForm.yil, ENUMTabloKod.TABPEBOdenek);

                IDataReader r = to.FormListe(kullanan, sorguForm, pebVar, 0, tUzBul.KodUzunlukBul(3, 2, true), true, false);
                if (to.Hata() == "")
                {
                    int detaySayac = 0;
                    GondermeTenkisFormW os = null;
                    while (r.Read())
                    {
                        GondermeTenkisDetay d = new GondermeTenkisDetay();
                        GondermeTenkisForm osos = MUHGondermeTenkisServis.ReaderdanAl(r, pebVar, tUzBul, d);

                        if (os == null ||
                            os.belgeNo != BelgeNoYap(osos.yil, osos.belgeNo) ||
                            os.yil != osos.yil ||
                            os.belgeTur != (MUHServisC.tr.gov.sbb.programbutce.EnumodenekIslemTuruW)((int)osos.belgeTur)
                           )
                        {
                            os = new GondermeTenkisFormW();
                            KopyalaForm(os, osos);
                            os.detay = FormDetayDiziOl(null);
                            detaySayac = 0;
                            if (os.formDurum == (int)ENUMBelgeDurumu.MUHASEBATALDI)
                                GonderileceklereEkle(os, gonderilenBelgeler);
                            else
                                GonderileceklereEkle(os, okunanBelgeler);
                        }
                        GondermeTenkisDetayW detay = new GondermeTenkisDetayW();
                        KopyalaDetay(pebVar, tUzBul, os, detay, d);

                        if (detaySayac >= os.detay.Length)
                            os.detay = FormDetayDiziOl(os.detay);
                        os.detay[detaySayac] = detay;
                        detaySayac++;
                    }
                    r.Close();
                }
                else
                    TNS.MUH.IsKurallari.MUHServis.AktarimLogYaz($"Gönderilecek belgeler okunamadı, hata:{to.Hata()}");
                TNS.MUH.IsKurallari.MUHServis.AktarimLogYaz($"{yil} için {okunanBelgeler.Count} kayıt bulundu... işlem id:{islemId}");
            }
            catch (Exception ex)
            {
                TNS.MUH.IsKurallari.MUHServis.AktarimLogYaz($"Gönderilecek belgeler okunurken hata oluştu:{ex.Message}");
            }
            ve.VeriErisimKapat();
            return okunanBelgeler;
        }

        private void KopyalaForm(GondermeTenkisFormW os, GondermeTenkisForm osos)
        {
            os.yil = osos.yil;


            os.belgeTur = (MUHServisC.tr.gov.sbb.programbutce.EnumodenekIslemTuruW)((int)osos.belgeTur);
            os.belgeNo = BelgeNoYap(osos.yil, osos.belgeNo);
            os.kurum = osos.kurum;
            os.muhasebe = 0;

            os.aciklama = osos.aciklama;
            if (os.aciklama.Length > 250)
                os.aciklama = os.aciklama.Remove(250);
            os.islemTarih = osos.islemTarih.ToString();
            os.formDurumTarih = osos.formDurumTarih.ToString();
            os.formDurum = osos.formDurum;
            os.iptalNeden = osos.iptalNeden;
            os.icmalNo = osos.icmalNo;
            os.icmalTarih = osos.icmalTarih.ToString();
            os.onayNo = osos.onayNo;
            os.onayTarih = osos.onayTarih.ToString();
            os.yaziTarih = osos.yaziTarih.ToString();
            os.yaziNo = osos.yaziNo;
            os.istekYaziTarih = osos.istekYaziTarih.ToString();
            os.istekYaziNo = osos.istekYaziNo;
            os.islemYapan = osos.islemYapan;
            if (os.yil >= 2018)
            {
                os.vkn = osos.vergiNo;
            }
        }

        private void KopyalaDetay(int pebVar, Tertip ty, GondermeTenkisFormW os, GondermeTenkisDetayW detay, GondermeTenkisDetay d)
        {
            if (pebVar == 1)
            {
                detay.tertip = d.tertip.kodlar[0].Replace(".", "") + "-" + d.tertip.kodlar[1].Replace(".", "") + "-" + d.tertip.kodlar[2].Replace(".", "") + "-" + d.tertip.kodlar[3].Replace(".", "");
            }
            else
            {
                string kurKod = d.tertip.kodlar[0].Replace(".", "");
                string fonKod = d.tertip.kodlar[1].Replace(".", "");
                string finKod = d.tertip.kodlar[2].Replace(".", "");
                string ekoKod = d.tertip.kodlar[3].Replace(".", "");
                string pebKod = d.tertip.kodlar[4].Replace(".", "");

                string prgKur = Cevir(os.yil, 1, kurKod, ty);
                string prgFin = Cevir(os.yil, 3, finKod, ty);
                string prgEko = Cevir(os.yil, 4, ekoKod, ty);
                if (ekoKod == "0303" || ekoKod == "0304" || ekoKod == "0103")
                {
                    prgEko += "." + fonKod.Substring(fonKod.Length - 1) + "0";
                }
                string prgPEB = Cevir(os.yil, 140, pebKod, ty);

                //TNS.MUH.IsKurallari.MUHServis.AktarimLogYaz($"Tertip: {kurKod}:{prgKur} - {fonKod}:{"0" + fonKod.Substring(fonKod.Length - 1)} - {finKod}:{prgFin} - {ekoKod}:{prgEko} - {pebKod}:{prgPEB} - {d.miktar}");

                if (string.IsNullOrWhiteSpace(prgPEB) || string.IsNullOrWhiteSpace(prgKur) || string.IsNullOrWhiteSpace(prgFin) || string.IsNullOrWhiteSpace(prgEko))
                {
                    TNS.MUH.IsKurallari.MUHServis.AktarimLogYaz($"Hatalı Tertip: Belge:{((int)os.belgeTur)}/{os.belgeNo} tertip:{pebKod + "-" + kurKod + "-" + finKod + "-" + ekoKod} : {prgPEB + "-" + prgKur + "-" + prgFin + "-" + prgEko}");
                }
                detay.tertip = prgPEB + "-" + prgKur + "-" + prgFin + "-" + prgEko;
            }
            detay.miktar = d.miktar;
        }

        private static string Cevir(int yil, int kodNo, string kod, Tertip ty)
        {
            if (yil < 2021) return kod;
            if (kodNo < 10)
            {
                return OrtakFonksiyonlar.KodNoktaKoy(KodYapiBul(ty, kodNo), kod);
            }
            else
            {
                try
                {
                    return TNS.BHMv2.Arac.ProgramButceKodaCevir(yil, kodNo, kod.Replace(".", ""));
                }
                catch
                {
                    return null;
                }
            }
        }

        private static string KodYapiBul(Tertip ty, int kodNo)
        {
            return ty.Kirilimlar(kodNo - 1);
        }

        GondermeTenkisDetayW[] FormDetayDiziOl(GondermeTenkisDetayW[] kaynak)
        {
            if (kaynak == null)
                return new GondermeTenkisDetayW[1];
            else
            {
                GondermeTenkisDetayW[] tut = new GondermeTenkisDetayW[kaynak.Length * 2];
                kaynak.CopyTo(tut, 0);
                return tut;
            }
        }

        private void GonderileceklereEkle(GondermeTenkisFormW os, TNSCollection okunanBelgeler)
        {
            okunanBelgeler.Add(os);
        }

        /// <summary>
        /// e-bütçe sisteminden Ödenek gönderme belgelerinin durumlarını kontrol eder
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="yil">Yıl</param>
        /// <param name="tur">tur</param>
        public void ebutceOdenekGondermeKontrol(Kullanici kullanan, int yil, int tur)
        {
            //if (yil > 2020) return;
            try
            {
                TNS.MUH.IsKurallari.MUHServis.AktarimLogYaz(yil + " / " + tur + " kontrol ediliyor");
                ebutceOdenekGondermeDurumKontrol(kullanan, tur, yil);
            }
            catch (Exception ex) { OrtakFonksiyonlar.HataStrYaz("hata oluştu. -->" + ex.Message); }
        }

        /// <summary>
        /// e-bütçe sisteminden Ödenek gönderme belgelerinin durumlarını kontrol eder
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kontrolDurum">tur</param>
        /// <param name="yil">Yıl</param>
        public void ebutceOdenekGondermeDurumKontrol(Kullanici kullanan, int kontrolDurum, int yil)
        {
            //if (yil > 2020) return;

            MUHProgramButceIslemleri.EButceParolaBilgileri();


            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABGondermeTenkis to = new TABGondermeTenkis(ve);
            GondermeTenkisForm sorguForm = new GondermeTenkisForm();

            sorguForm = new GondermeTenkisForm();
            sorguForm.yil = yil;
            sorguForm.belgeTur = ENUMBelgeTuru.TANIMSIZ;
            sorguForm.kurum = TNS.MUH.Yetki.VarsayilanKurumBul();
            sorguForm.formDurum = kontrolDurum;
            ObjectArray oslar = new ObjectArray();
            TNS.MUH.IsKurallari.MUHServis.AktarimLogYaz($"Durum sorgula:{yil}-{sorguForm.kurum}-{sorguForm.formDurum} sorgulanıyor....");
            Tertip tUzBul = new Tertip(yil, ENUMTabloKod.TABPEBOdenek);

            IDataReader r = to.FormListe(kullanan, sorguForm, 0, 0, tUzBul.KodUzunlukBul(3, 2, true), true, false);
            if (r != null)
            {
                while (r.Read())
                {
                    GondermeTenkisFormW os = new GondermeTenkisFormW();

                    os.yil = VeriErisim.DegerAl(r, 0, 0);
                    os.belgeTur = (EnumodenekIslemTuruW)VeriErisim.DegerAl(r, 1, 0);
                    os.belgeNo = BelgeNoYap(os.yil, VeriErisim.DegerAl(r, 2, ""));
                    os.kurum = VeriErisim.DegerAl(r, 3, "");
                    os.formDurum = VeriErisim.DegerAl(r, 9, 0);

                    oslar.Ekle(os);
                }
                r.Close();
            }
            ve.VeriErisimKapat();

            ListeyiKontrolEt(kullanan, yil, kontrolDurum, oslar.objeler, tUzBul);
        }

        private static void ListeyiKontrolEt(Kullanici kullanan, int yil, int kontrolDurum, TNSCollection oslar, Tertip tUzBul)
        {
            TNS.BHMv2.IBHMv2Servis bhmServis = TNS.BHMv2.Arac.Tanimla();
            int mpDurum = bhmServis.MasrafPlaniGenelOnayDurumBul(kullanan, yil, "", BHMv2.ENUMMasrafPlanAsama.AHP);

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABGondermeTenkis to = new TABGondermeTenkis(ve);

            TNSCollection tcMPRevizeForm = new TNSCollection();
            foreach (GondermeTenkisFormW os in oslar)
            {
                string saklaKurum = os.kurum;
                try
                {
                    os.kurum = Cevir(os.yil, 1, os.kurum, tUzBul);

                    //TNS.MUH.IsKurallari.MUHServis.AktarimLogYaz($"Durum sorgula:{os.yil}-{os.belgeTur}-{saklaKurum}:{os.kurum}-{os.belgeNo}-{os.formDurum} sorgulanıyor....");
                    GondermeTenkisFormW sonDurum = servisSBB.OdenekGondermeBelgeDurum(MUHProgramButceIslemleri.ebutceUser, MUHProgramButceIslemleri.ebutcePassword, os);

                    os.kurum = saklaKurum;

                    if (sonDurum != null && sonDurum.formDurum > kontrolDurum || (kontrolDurum == (int)ENUMBelgeDurumu.MUHASEBATIADE && sonDurum.formDurum == (int)ENUMBelgeDurumu.ONAYLI))
                    {
                        if ((sonDurum.formDurum == (int)ENUMBelgeDurumu.ONAYLI) || (sonDurum.formDurum == (int)ENUMBelgeDurumu.MUHASEBATIADE) || (sonDurum.formDurum == (int)ENUMBelgeDurumu.IPTAL))
                            BelgeDurumuDegistir(mpDurum, ve, to, os.kurum, tcMPRevizeForm, os, sonDurum);
                    }
                    else
                    {
                        //if (sonDurum == null)
                        //    TNS.MUH.IsKurallari.MUHServis.AktarimLogYaz($"{guncelDurum.belgeNo} için belge gelmedi ....");
                        //else if(sonDurum.formDurum != guncelDurum.formDurum)
                        //    TNS.MUH.IsKurallari.MUHServis.AktarimLogYaz($"{sonDurum.belgeNo} için, gelen durum:{sonDurum.formDurum}, belgeTur: {sonDurum.belgeTur}; Biz belgeNo:{os.belgeNo}, formDurum:{os.formDurum}, belgeTur:{os.belgeTur}");
                    }
                }
                catch (Exception ex)
                {
                    if (ex.Message.IndexOf("EnumodenekIslemTuruW") > 0)
                    {
                        TNS.MUH.IsKurallari.MUHServis.AktarimLogYaz("Exception: " + os.yil + "/" + os.belgeNo + "/" + os.belgeTur.ToString() + " İptal ediliyor");
                        try
                        {
                            BelgeDurumuDegistir(mpDurum, ve, to, os.kurum, tcMPRevizeForm, os, null);
                        }
                        catch (Exception exIc)
                        {
                            TNS.MUH.IsKurallari.MUHServis.AktarimLogYaz("Exception: " + os.yil + "/" + os.belgeNo + "/" + os.belgeTur.ToString() + " İptal edilemedi:" + exIc.Message);
                        }
                    }
                    else
                        TNS.MUH.IsKurallari.MUHServis.AktarimLogYaz("Exception: " + os.yil + "/" + os.belgeNo + "/" + os.belgeTur.ToString() + " " + ex.Message);
                }
            }
            ve.VeriErisimKapat();
            bhmServis.MPRevizeFormDurumAta(kullanan, tcMPRevizeForm);
        }

        private static void BelgeDurumuDegistir(int mpDurum, VeriErisim ve, TABGondermeTenkis to, string kurumKod, TNSCollection tcMPRevizeForm, GondermeTenkisFormW os, GondermeTenkisFormW sonDurum)
        {
            GondermeTenkisForm guncelDurum = new GondermeTenkisForm();

            guncelDurum.yil = os.yil;
            guncelDurum.belgeTur = (ENUMBelgeTuru)((int)os.belgeTur);
            guncelDurum.kurum = os.kurum;
            guncelDurum.belgeNo = BelgeNoYap(guncelDurum.yil, os.belgeNo, kurumKod);

            if (sonDurum != null)
            {
                guncelDurum.formDurum = sonDurum.formDurum;
                guncelDurum.onayNo = sonDurum.onayNo;
                guncelDurum.onayTarih = new TNSDateTime(sonDurum.onayTarih.ToString());
            }
            else
                guncelDurum.formDurum = (int)ENUMBelgeDurumu.IPTAL;
            guncelDurum.islemYapan = "uygulama";
            ve.hata = "";
            ve.BeginTransaction();
            Sonuc s = to.BelgeDurumDegistir(guncelDurum);
            if (s.islemSonuc)
            {
                ve.Commit();

                if (mpDurum == 5 && (guncelDurum.formDurum == (int)ENUMBelgeDurumu.MUHASEBATIADE || guncelDurum.formDurum == (int)ENUMBelgeDurumu.IPTAL || guncelDurum.formDurum == (int)ENUMBelgeDurumu.ONAYLI))
                {
                    TNS.BKD.MPRevizeForm mpRevizeForm = new BKD.MPRevizeForm();
                    mpRevizeForm.formDurum = (guncelDurum.formDurum == (int)ENUMBelgeDurumu.MUHASEBATIADE || guncelDurum.formDurum == (int)ENUMBelgeDurumu.IPTAL) ? BKD.ENUMRevizeFormDurum.Iptal : BKD.ENUMRevizeFormDurum.Onayli;
                    mpRevizeForm.odGonNo = guncelDurum.belgeNo;
                    mpRevizeForm.odGonTur = (int)guncelDurum.belgeTur;
                    mpRevizeForm.odGonYil = guncelDurum.yil;
                    tcMPRevizeForm.Add(mpRevizeForm);
                }
            }
            else
                ve.Rollback();

            if (guncelDurum.formDurum == (int)ENUMBelgeDurumu.ONAYLI && guncelDurum.belgeTur == ENUMBelgeTuru.TENKIS)
            {
                //TNS.MUH.IsKurallari.MUHServis.AktarimLogYaz(guncelDurum.yil + "/" + guncelDurum.belgeNo + "/" + guncelDurum.belgeTur.ToString() + " " + "");
            }
        }

        private static long BelgeNoYap(int yil, string belgeNo)
        {
            int basla;
            if (yil >= 2021)
                basla = 4;
            else
                basla = 0;
            return OrtakFonksiyonlar.ConvertToLong(belgeNo.Substring(basla));
        }

        private static string BelgeNoYap(int yil, long belgeNo, string kurumKod)
        {
            string tut = belgeNo.ToString();
            if (yil >= 2021)
            {
                tut = kurumKod.Replace(".", "").PadRight(4, '0') + tut.PadLeft(5, '0');
            }
            return tut;
        }
    }
}
