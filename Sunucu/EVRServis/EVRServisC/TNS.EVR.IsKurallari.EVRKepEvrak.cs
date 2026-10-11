using System.Data;
using TNS.DEG;
using TNS.EVR.IsKurallari.Veri;
using TNS.KYM;
using TNS.Veri;
using EVRServisI;
using System.Collections;
using System;

namespace TNS.EVR.IsKurallari
{
    public class EVRKepEvrak
    {
        public static Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, KepEvrak form)
        {
            Sonuc sonuc = new Sonuc(false, EvrakI.EVRI110, "");

            string hata = string.Empty;
            if (form.evrEvrakNo == "")
                hata += "Evrak bilgisi boş." + "<br>";
            if (string.IsNullOrEmpty(form.gonderen))
                hata += "Kep gönderen bilgisi boş." + "<br>";
            if (string.IsNullOrEmpty(form.alici))
                hata += "Kep alıcı bilgisi boş." + "<br>";
            if (form.islem == ENUMKEPEvrakIslem.TANIMSIZ)
                hata += "Kep kayıt işlem durumu tanımsız." + "<br>";

            if (!string.IsNullOrEmpty(hata))
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = hata;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);
            TABKepEvrak tab = new TABKepEvrak(ve);

            ve.BeginTransaction();
            try
            {
                sonuc = (form.kod == 0) ? tab.Kaydet(form) : tab.Guncelle(form);


                if (sonuc.islemSonuc && form.islem == ENUMKEPEvrakIslem.GONDER)
                {
                    ObjectArray evraklar = EVREvrak.Listele(ve, kullanan, form.evrak, 0, 1);
                    if (evraklar.sonuc.islemSonuc && evraklar.objeler.Count > 0)
                    {
                        EvrakSinif evrak = (EvrakSinif)evraklar[0];

                        //-------------------Kep Hareketi---------------------------
                        EvrakHareket hareket = new EvrakHareket();
                        hareket.prEvrakNo = evrak.prEvrakNo;
                        hareket.islem = (int)ENUMEvrakHareketDurum.KEPILEGONDERILDI;
                        hareket.birimKod1 = evrak.hareket.birimKod1;
                        hareket.birimKod2 = evrak.hareket.birimKod2;
                        hareket.kisi1 = evrak.hareket.kisi1;
                        hareket.kisi2 = evrak.hareket.kisi2;
                        hareket.havaleNotu = evrak.hareket.havaleNotu;
                        hareket.vekil = evrak.hareket.vekil;
                        hareket.vekilAd = evrak.hareket.vekilAd;

                        sonuc = EVREvrakHareket.Kaydet(ve, kullanan, hareket);
                        if (!sonuc.islemSonuc)
                            sonuc.DegerAta("Evrak hareket bilgisi güncelleme sırasında hata oluştu.<br>prEvrakNo: " + evrak.prEvrakNo + " - Message: " + sonuc.hataStr);


                        if (sonuc.islemSonuc)
                        {
                            EvrakSinif formKepEvrak = new EvrakSinif();
                            formKepEvrak.evrEvrakNo = evrak.evrEvrakNo;
                            formKepEvrak.kepGonderimTarih = new TNSDateTime(DateTime.Now.ToString());

                            TABEvrak tabEvrak = new TABEvrak(ve);
                            sonuc = tabEvrak.Guncelle(formKepEvrak);
                            if (!sonuc.islemSonuc)
                                sonuc.DegerAta("Evrak bilgisi güncelleme sırasında hata oluştu.<br>prEvrakNo: " + evrak.prEvrakNo + " - Message: " + sonuc.hataStr);
                        }
                    }
                    else
                        sonuc.DegerAta("Evrak bilgisine ulaşılamadı.<br>Detay: " + evraklar.sonuc.hataStr);
                }


                if (!sonuc.islemSonuc)
                {
                    sonuc.hataStr = EvrakI.EVRI095 + "<br>" + sonuc.hataStr;
                    ve.Rollback();
                }
                else
                {
                    sonuc.anahtar = form.kod.ToString();
                    sonuc.bilgiStr = EvrakI.EVRI094;
                    ve.Commit();
                }
            }
            catch (Exception ex)
            {
                ve.Rollback();
                OrtakFonksiyonlar.HataStrYaz("{F3F0E323-75A1-4617-9B7A-0E454F7B542E} BeginTransaction - Message: " + ex.Message + " # StackTrace: " + ex.StackTrace);
            }
            ve.VeriErisimKapat();
            return sonuc;
        }

        public static Sonuc Sil(VeriErisim veriErisim, Kullanici kullanan, KepEvrak form)
        {
            Sonuc sonuc = new Sonuc(false, "", "");

            string hata = string.Empty;
            if (form.kod <= 0 || form.evrEvrakNo == "")
                hata = "Silinecek veri bulunamadı." + "<br>";

            if (!string.IsNullOrEmpty(hata))
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = hata;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);
            TABKepEvrak tab = new TABKepEvrak(ve);

            ve.BeginTransaction();
            try
            {
                sonuc = tab.Sil(form);
                if (sonuc.islemSonuc)
                {
                    ve.Commit();
                    sonuc.bilgiStr = EvrakI.EVRI097;
                }
                else
                {
                    ve.Rollback();
                    sonuc.hataStr = EvrakI.EVRI098 + "<br>" + sonuc.hataStr;
                }
            }
            catch (Exception ex)
            {
                ve.Rollback();
                OrtakFonksiyonlar.HataStrYaz("{CDB6C542-AD35-46FB-98D6-D26DCBFF0A5A} BeginTransaction - Message: " + ex.Message + " # StackTrace: " + ex.StackTrace);
            }

            ve.VeriErisimKapat();
            return sonuc;
        }

        public static KepEvrak[] Listele(VeriErisim veriErisim, Kullanici kullanan, KepEvrak form)
        {
            ObjectArray liste = new ObjectArray();
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);
            TABKepEvrak tab = new TABKepEvrak(ve);

            IDataReader dr = tab.Listele(form);
            if (dr != null)
            {
                int index = (int)TABKepEvrak.ENUMAlanlar.KEPMESAJID;
                while (dr.Read())
                {
                    form = new KepEvrak();
                    form.kod = VeriErisim.DegerAl(dr, (int)TABKepEvrak.ENUMAlanlar.KOD, 0);
                    form.evrEvrakNo = VeriErisim.DegerAl(dr, (int)TABKepEvrak.ENUMAlanlar.EVREVRAKNO, "");
                    form.gonderen = VeriErisim.DegerAl(dr, (int)TABKepEvrak.ENUMAlanlar.GONDEREN, "");
                    form.alici = VeriErisim.DegerAl(dr, (int)TABKepEvrak.ENUMAlanlar.ALICI, "");
                    form.islem = (ENUMKEPEvrakIslem)VeriErisim.DegerAl(dr, (int)TABKepEvrak.ENUMAlanlar.ISLEM, 0);
                    form.islemTarihi = VeriErisim.DegerAl(dr, (int)TABKepEvrak.ENUMAlanlar.ISLEMTARIH);
                    form.islemYapan = VeriErisim.DegerAl(dr, (int)TABKepEvrak.ENUMAlanlar.ISLEMYAPAN, "");
                    form.islemYapanAd = VeriErisim.DegerAl(dr, (int)TABKepEvrak.ENUMAlanlar.ISLEMYAPANAD, "");
                    form.kepId = VeriErisim.DegerAl(dr, (int)TABKepEvrak.ENUMAlanlar.KEPID, "");
                    form.kepMesajId = VeriErisim.DegerAl(dr, (int)TABKepEvrak.ENUMAlanlar.KEPMESAJID, "");

                    form.evrak.prEvrakNo = VeriErisim.DegerAl(dr, index + 1, "");
                    form.evrak.yil = VeriErisim.DegerAl(dr, index + 2, 0);
                    form.evrak.evrakNo = VeriErisim.DegerAl(dr, index + 3, 0);
                    form.evrak.evrakTarih = VeriErisim.DegerAl(dr, index + 4);
                    form.evrak.geldigiYer = VeriErisim.DegerAl(dr, index + 5, "");
                    form.evrak.konu = VeriErisim.DegerAl(dr, index + 6, "");
                    form.evrak.dosyasiVar = VeriErisim.DegerAl(dr, index + 7, 0);
                    form.evrak.dagitimTarih = VeriErisim.DegerAl(dr, index + 8);
                    form.evrak.eImzaTarihi = VeriErisim.DegerAl(dr, index + 9);
                    form.evrak.imzaBitti = VeriErisim.DegerAl(dr, index + 10, 0);

                    liste.objeler.Add(form);
                }
                dr.Close();
            }

            ve.VeriErisimKapat();

            KepEvrak[] arr = null;
            if (liste.objeler.Count > 0)
                arr = (KepEvrak[])liste.objeler.ToArray(typeof(KepEvrak));

            return arr;
        }

        public static KepEvrak Oku(VeriErisim veriErisim, Kullanici kullanan, KepEvrak form)
        {
            if (form.kod == 0) return null;

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);

            KepEvrak[] list = Listele(ve, kullanan, form);

            if (list != null && list.Length == 1)
                form = list[0];

            ve.VeriErisimKapat();

            return form;
        }

    }
}
