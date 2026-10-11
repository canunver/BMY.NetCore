using System;
using System.Collections.Generic;
using System.Data;
using TNS.HRC;
using TNS.KYM;
using TNS.TMM.IsKurallari.Veri;
using TNS.Veri;

namespace TNS.TMM.IsKurallari
{
    public class TMMTCCBEntegrasyon
    {
        public static Sonuc Kaydet(Kullanici kullanan, TasinirIslemForm form, string tip)
        {
            Sonuc sonuc = new Sonuc();
            IHRCServis servisHRC = TNS.HRC.Arac.Tanimla();


            //Entegrasyonlarda(kky ve sosyalsgbnet) sadece muhasebe kodu 00200, 00201 ve 00202 olan taşınır işlem fişlerinin gönderilmesi gerekmektedir. 2024.07.08 HÖ
            string sosyalSgbNetMuhasebeKod = OrtakFonksiyonlar.WebConfigOku("TasinirTCCBEntegrasyonSosyalSgbNetMuhasebeKod", "");
            if (Arac.CumhurbaskanligiSosyalSgbNetKullaniyor() && !string.IsNullOrWhiteSpace(sosyalSgbNetMuhasebeKod) && !sosyalSgbNetMuhasebeKod.Contains(form.muhasebeKod))
            {
                sonuc.islemSonuc = true;
                return sonuc;
            }

            //if (string.IsNullOrWhiteSpace(form.nereyeGitti))
            //{
            //    sonuc.islemSonuc = true;
            //    sonuc.hataStr = "NereyeGitti alanı boş olduğu için Entegrasyon işlemi yapılmayacak";

            //    OrtakFonksiyonlar.HataDosyaYaz("TCCBEntegrasyonHata.txt", form.yil + ">" + form.muhasebeKod + ">" + form.harcamaKod + ">" + form.fisNo + ">>>" + sonuc.hataStr, false);
            //    return sonuc;
            //}

            ItemTransaction it = new ItemTransaction();

            //HRC.FirmaBilgisi firma = new HRC.FirmaBilgisi();
            //firma.ad = form.nereyeGitti;
            //firma.vergiDairesi = "SANALDEPO";
            //firma.muhasebe = OrtakFonksiyonlar.ConvertToInt(form.muhasebeKod, 0);
            //ObjectArray firmaListe = servisHRC.FirmaBilgisiListele(kullanan, firma);
            //if (firmaListe.ObjeSayisi == 0)
            //{
            //    sonuc.islemSonuc = true;
            //    sonuc.hataStr = form.nereyeGitti + " adındaki sanal ambar bulunamadı. Entegrasyon işlemi yapılmayacak";

            //    OrtakFonksiyonlar.HataDosyaYaz("TCCBEntegrasyonHata.txt", form.yil + ">" + form.muhasebeKod + ">" + form.harcamaKod + ">" + form.fisNo + ">>>" + sonuc.hataStr, false);
            //    return sonuc;
            //}
            //else
            //{
            //    foreach (HRC.FirmaBilgisi ff in firmaListe.objeler)
            //    {
            //        if (ff.ad != form.nereyeGitti) continue;
            //        it.WarehouseCode = ff.vergiNo;
            //        it.WarehouseDescription = ff.ad;
            //    }
            //}

            it.WarehouseCode = form.ambarKod;
            it.WarehouseDescription = form.ambarAd;

            //if (string.IsNullOrWhiteSpace(it.WarehouseCode))
            //{
            //    sonuc.islemSonuc = true;
            //    sonuc.hataStr = form.nereyeGitti + " adındaki sanal ambar kayıtlı firmalar içinde yok. Entegrasyon işlemi yapılmayacak";

            //    OrtakFonksiyonlar.HataDosyaYaz("TCCBEntegrasyonHata.txt", form.yil + ">" + form.muhasebeKod + ">" + form.harcamaKod + ">" + form.fisNo + ">>>" + sonuc.hataStr, false);
            //    return sonuc;
            //}

            it.ReferanceNumber = "";

            if (tip == "Onay")
                it.TransactionType = "10";//Belge onaylanmış, Bizden çıkış, karşı tarafa giriş
            else
            {
                //Onay kaldırılmış o zaman önceden gönderilen referans verilerek çıkış olarak gönderilecek
                TCCBEntegrasyon oncekiGonderilen = ReferansNoGetir(form);
                it.TransactionType = "11";//Belgenin onayı kaldırılmış, karşı taraftan çıkış olacak

                //it.ReferanceNumber = oncekiGonderilen.referansNo; 06.06.2022 Referans no gönderildiğinde önceki kayıt update edildiği için kaldırıldı TCCB Toplantı Umut Yurdakul-Melih

                if (string.IsNullOrWhiteSpace(oncekiGonderilen.referansNo))
                {
                    sonuc.islemSonuc = true;
                    sonuc.hataStr = "Önceden gönderilen kayıt bulunamadı. Bu nedenle gönderme işlemi yapılmayacak";

                    OrtakFonksiyonlar.HataDosyaYaz("TCCBEntegrasyonHata.txt", tip + ">" + DateTime.Now.ToString("yyyyMMdd HH:mm:ss.fff") + ">" + form.yil + ">" + form.muhasebeKod + ">" + form.harcamaKod + ">" + form.fisNo + ">>>" + sonuc.hataStr, false);
                    return sonuc;
                }
            }

            it.TransactionDateTime = DateTime.Now.ToString("dd.MM.yyyy HH:mm");
            it.SpeacialCode1 = form.yil + " - " + form.muhasebeKod + " - " + form.harcamaKod + " - " + form.fisNo;
            it.SpeacialCode2 = form.kimeGitti;
            it.TransactionUserCode = form.islemYapan;

            List<TransactionItem> tiler = new List<TransactionItem>();
            foreach (TasinirIslemDetay detay in form.detay.objeler)
            {
                TransactionItem ti = new TransactionItem();
                ti.CurrencyUnit = "TL";
                ti.ItemCode = detay.hesapPlanKod;
                ti.Quantity = OrtakFonksiyonlar.ConvertToDbl(detay.miktar);
                ti.UnitPrice = OrtakFonksiyonlar.ConvertToDbl(detay.birimFiyatKDVLi);
                ti.ItemDescription = detay.hesapPlanAd;
                ti.Unit = detay.olcuBirimAd;
                tiler.Add(ti);
            }

            it.Items = tiler.ToArray();

            BW2WS servis = new BW2WS();
            servis.Url = "https://vys.tccb.gov.tr/BeamItemService/BW2WS.asmx";
            DoItemTransactionResult di = servis.DoItemTransaction("TCCB", 100, "webservis", "tccb2023**", it);

            if (di.Success)
            {
                TCCBEntegrasyon yeni = new TCCBEntegrasyon();
                yeni.yil = form.yil;
                yeni.muhasebeKod = form.muhasebeKod;
                yeni.harcamaKod = form.harcamaKod;
                yeni.fisNo = form.fisNo;
                yeni.referansNo = di.ReferanceNumber;
                if (string.IsNullOrWhiteSpace(yeni.referansNo) && tip != "Onay")
                    yeni.referansNo = "GERICEKILDI";

                Sonuc refSonuc = ReferansNoKaydet(yeni);
                if (refSonuc.islemSonuc)
                    OrtakFonksiyonlar.HataDosyaYaz("TCCBEntegrasyonTamam.txt", tip + ">" + DateTime.Now.ToString("yyyyMMdd HH:mm:ss.fff") + ">" + form.yil + ">" + form.muhasebeKod + ">" + form.harcamaKod + ">" + form.fisNo + ">>>" + yeni.referansNo + " refSonuc:" + refSonuc.anahtar, false);
                else
                    OrtakFonksiyonlar.HataDosyaYaz("TCCBEntegrasyonHata.txt", tip + ">" + DateTime.Now.ToString("yyyyMMdd HH:mm:ss.fff") + ">" + form.yil + ">" + form.muhasebeKod + ">" + form.harcamaKod + ">" + form.fisNo + ">>>" + yeni.referansNo + " refSonuc:" + refSonuc.hataStr, false);
            }
            else
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = di.ErrorMessage;
                OrtakFonksiyonlar.HataDosyaYaz("TCCBEntegrasyonHata.txt", tip + ">" + DateTime.Now.ToString("yyyyMMdd HH:mm:ss.fff") + ">" + form.yil + ">" + form.muhasebeKod + ">" + form.harcamaKod + ">" + form.fisNo + ">>>" + " di.ErrorMessage:" + di.ErrorMessage, false);
            }

            return sonuc;
        }

        public static Sonuc Kaydet255(Kullanici kullanan, ObjectArray sicilListe, string tip)
        {
            Sonuc sonuc = new Sonuc();

            //Entegrasyonlarda(kky ve sosyalsgbnet) sadece muhasebe kodu 00200, 00201 ve 00202 olan taşınır işlem fişlerinin gönderilmesi gerekmektedir. 2024.07.08 HÖ
            string sosyalSgbNetMuhasebeKod = OrtakFonksiyonlar.WebConfigOku("TasinirTCCBEntegrasyonSosyalSgbNetMuhasebeKod", "");
            if (sicilListe.objeler.Count > 0 && Arac.CumhurbaskanligiSosyalSgbNetKullaniyor() && !string.IsNullOrWhiteSpace(sosyalSgbNetMuhasebeKod) && !sosyalSgbNetMuhasebeKod.Contains(((SicilNoHareket)sicilListe.objeler[0]).muhasebeKod))
            {
                sonuc.islemSonuc = true;
                return sonuc;
            }

            BW2WS servis = new BW2WS();
            servis.Url = "https://vys.tccb.gov.tr/BeamItemService/BW2WS.asmx";

            TCCBEntegrasyon tifBilgi = new TCCBEntegrasyon();

            foreach (SicilNoHareket item in sicilListe.objeler)
            {
                Equipment eq = new Equipment();
                eq.Code = item.sicilNo;
                eq.Description = item.hesapPlanAd;
                if (tip == "numaraVer" || tip == "ambar")
                    eq.OwnerEquipmentCode = item.muhasebeKod + "|" + item.harcamaBirimKod + "|" + item.ambarKod;
                else if (tip == "çıkış")
                    eq.OwnerEquipmentCode = "ÇIKIŞ";
                else if (tip == "iptal")
                    eq.OwnerEquipmentCode = "İPTAL";
                else if (tip == "zimmet")
                {
                    if (item.islemTurKod == (int)ENUMZimmetVermeDusme.ZIMMETVERME || item.islemTurKod == (int)ENUMIslemTipi.DTLVERME || item.islemTurKod == (int)ENUMIslemTipi.ZFVERME)
                        eq.OwnerEquipmentCode = item.kimeGitti + "|" + item.nereyeGitti;
                    else
                        eq.OwnerEquipmentCode = item.muhasebeKod + "|" + item.harcamaBirimKod + "|" + item.ambarKod;
                }

                AddEquipmentResult ae = servis.MoveEquipment("TCCB", 100, "webservis", "tccb2023**", eq);
                if (ae.Success)
                {
                    OrtakFonksiyonlar.HataDosyaYaz("TCCBEntegrasyonTamam.txt", tip + ">" + DateTime.Now.ToString("yyyyMMdd HH:mm:ss.fff") + ">" + item.yil + ">" + item.muhasebeKod + ">" + item.harcamaBirimKod + ">" + item.fisNo + ">" + item.sicilNo + ">>>" + eq.OwnerEquipmentCode + ">>>" + ae.ReferanceNumber, false);
                }
                else
                {
                    OrtakFonksiyonlar.HataDosyaYaz("TCCBEntegrasyonHata.txt", tip + ">" + DateTime.Now.ToString("yyyyMMdd HH:mm:ss.fff") + ">" + item.yil + ">" + item.muhasebeKod + ">" + item.harcamaBirimKod + ">" + item.fisNo + ">" + item.sicilNo + ">>>" + eq.OwnerEquipmentCode + ">>>" + ae.ReferanceNumber + " refSonuc:" + ae.ErrorMessage, false);
                }
            }

            sonuc.islemSonuc = true;
            return sonuc;
        }

        public static Sonuc ReferansNoKaydet(TCCBEntegrasyon bilgi)
        {
            Sonuc sonuc = new Sonuc();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriTMM", "");
            TABTCCBEntegrasyon to = new TABTCCBEntegrasyon(ve);

            ve.BeginTransaction();

            bilgi.kod = Guid.NewGuid().ToString().Replace("-", "");

            sonuc = to.Sil(bilgi);//Önceden kayıtlı olanlar varsa silindi olarak işaretlensin
            sonuc = to.Kaydet(bilgi);

            if (sonuc.islemSonuc)
            {
                ve.Commit();
                sonuc.anahtar = bilgi.kod;
                sonuc.bilgiStr = "Referans No kayıt edildi";
            }
            else
                ve.Rollback();

            ve.VeriErisimKapat();
            return sonuc;
        }

        public static TCCBEntegrasyon ReferansNoGetir(TasinirIslemForm bilgi)
        {
            TCCBEntegrasyon donenDeger = new TCCBEntegrasyon();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriTMM", "");

            TABTCCBEntegrasyon to = new TABTCCBEntegrasyon(ve);
            IDataReader r = to.Getir(bilgi);

            if (to.Hata() == "")
            {
                while (r.Read())
                {
                    donenDeger.kod = VeriErisim.DegerAl(r, 0, "");
                    donenDeger.yil = VeriErisim.DegerAl(r, 1, 0);
                    donenDeger.muhasebeKod = VeriErisim.DegerAl(r, 2, "");
                    donenDeger.harcamaKod = VeriErisim.DegerAl(r, 3, "");
                    donenDeger.fisNo = VeriErisim.DegerAl(r, 4, "");
                    donenDeger.referansNo = VeriErisim.DegerAl(r, 5, "");
                    donenDeger.silindi = VeriErisim.DegerAl(r, 6, 0);
                }
            }

            r.Close();

            ve.VeriErisimKapat();

            return donenDeger;
        }

    }
}
