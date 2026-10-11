using System;
using System.Data;
using TNS;
using TNS.Veri;
using TNS.HRC;
using TNS.HRC.IsKurallari.Veri;
using TNS.KYM;
using System.Collections;

namespace TNS.HRC.IsKurallari
{
    /// <summary>
    /// HRCFaturaAna formu ile ilgili, iş kuralları kontrollerini yapıp,
    /// veri tabanı bağlantısını alıp, veri tabanı fonksiyonlarını çağırarak
    /// güncelleme işlemlerini gerçekleştiren fonksiyonları tutan sınıf.
    /// </summary>
    public class HRCFaturaTum
    {
        /// <summary>
        /// Veri tabanı bağlantısı, web.config dosyasında bu bağlantı ile ile ilgili parametreler bulunmalıdır.
        /// </summary>
        public static string VERIERISIMAD = "BaglantiSatiriHRC";

        /// <summary>
        /// Kullanıcının verilen anahtar alanlara sahip olan nesneyi görme yetkisinin var olup olmadığına bakan fonksiyon
        /// <param name="kullanan">Yetkisi kontrol edilecek kullanıcı bilgilerini tutan nesne</param>
        /// <param name="form">Kullanıcının görmeye çalıştığı anahtar değerleri tutan nesne</param>
        /// </summary>
        /// <returns>Görme yetkisi varsa true, yoksa false döndürür.</returns>
        public static bool GormeYetkisiVarMi(Kullanici kullanan, FaturaTum form)
        {
            return true;
        }

        /// <summary>
        /// FaturaAna listesini veri tabanından okuyan ve dizi olarak döndüren fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">FaturaAna yapısında FaturaAna:  bilgilerini tutan değişken</param>
        public static FaturaTum[] Listele(HRCServis hrcServis, VeriErisim veriErisim, Kullanici kullanan, FaturaTum form)
        {
            if (!GormeYetkisiVarMi(kullanan, form))
            {
                throw new Exception("Görme Yetkiniz Yok");
            }

            ArrayList alAna = new ArrayList();

            AboneFatura kriter1 = new AboneFatura();
            kriter1.faturaYil = form.yil;
            if (!string.IsNullOrEmpty(form.firmaKodu))
                kriter1.aboneTip = OrtakFonksiyonlar.ConvertToInt(form.firmaKodu, -1);

            if (form.muhasebe > 0)
                kriter1.muhasebe = form.muhasebe;

            if (!string.IsNullOrEmpty(form.birim))
                kriter1.birim = form.birim;

            if (!string.IsNullOrEmpty(form.faturaNo))
                kriter1.faturaNo = form.faturaNo;

            if (form.tip == -1)
            {
                if (string.IsNullOrWhiteSpace(form.firmaAdi))
                {
                    TNSDateTime kontrolTarih = new TNSDateTime();
                    kriter1.mifDurum = -100;//süreç iptaller gelmesin
                    ObjectArray oa = new HRCAboneFatura().Listele(kullanan, kriter1, kontrolTarih, false);

                    foreach (AboneFatura item in oa.objeler)
                    {
                        FaturaTum detay = new FaturaTum();
                        detay.muhasebe = item.muhasebe;
                        detay.birim = item.birim;
                        detay.firmaKodu = item.aboneTip.ToString();
                        detay.firmaAdi = TNS.HRC.Arac.AboneAdBul(item.aboneTip);
                        detay.faturaNo = item.faturaNo;
                        detay.faturaTarihi = new TNSDateTime("01." + item.faturaAy + "." + kriter1.faturaYil);
                        detay.aciklama = "Abone No:" + item.aboneNo;
                        detay.faturaTutari = item.faturaTutar;
                        detay.faturaTutariKDVLI = item.faturaTutar;
                        detay.surecYil = item.yil;
                        detay.surecNo = item.surecNo;

                        detay.mIFYil = item.odemeEmriYil;
                        if (detay.mIFYil == 0) detay.mIFYil = detay.surecYil;
                        detay.mIFNo = item.odemeEmriNo;

                        detay.yevmiyeTarih = item.yevmiyeTarih;
                        detay.yevmiyeNo = item.yevmiyeNo;
                        detay.mifDurum = item.mifDurum;
                        alAna.Add(detay);
                    }
                }
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));
            IDataReader dr;
            if (form.tip == -1)
            {
                TABFaturaForm tabF = new TABFaturaForm(ve);
                form.mifDurum = -100;
                dr = tabF.FaturaListele(form);
                if (dr != null)
                {
                    while (dr.Read())
                    {
                        FaturaTum detay = new FaturaTum();
                        detay.surecYil = VeriErisim.DegerAl(dr, 0, 0);
                        detay.muhasebe = VeriErisim.DegerAl(dr, 1, 0);
                        detay.birim = VeriErisim.DegerAl(dr, 2, "");
                        detay.surecNo = VeriErisim.DegerAl(dr, 3, "");

                        detay.faturaNo = VeriErisim.DegerAl(dr, 4, "");
                        detay.faturaTarihi = VeriErisim.DegerAl(dr, 5);
                        detay.aciklama = VeriErisim.DegerAl(dr, 6, "");

                        detay.firmaAdi = VeriErisim.DegerAl(dr, 7, "");
                        detay.firmaKodu = VeriErisim.DegerAl(dr, 8, "");
                        detay.mIFYil = VeriErisim.DegerAl(dr, 9, 0);
                        detay.mIFNo = VeriErisim.DegerAl(dr, 10, "");

                        detay.yevmiyeTarih = VeriErisim.DegerAl(dr, 11);
                        detay.yevmiyeNo = VeriErisim.DegerAl(dr, 12, 0);
                        detay.mifDurum = VeriErisim.DegerAl(dr, 13, 0);

                        detay.faturaTutari = VeriErisim.DegerAl(dr, 14, (double)0);
                        detay.faturaTutariKDVLI = VeriErisim.DegerAl(dr, 15, (double)0);

                        if (detay.mIFYil == 0) detay.mIFYil = detay.surecYil;

                        alAna.Add(detay);
                    }
                    dr.Close();
                }
                ve.hata = "";
            }

            TABHRCPSLGECICIGOREVUCAKFATURA tabU = new TABHRCPSLGECICIGOREVUCAKFATURA(ve);
            dr = tabU.Listele(form);
            if (dr != null)
            {
                while (dr.Read())
                {
                    FaturaTum detay = new FaturaTum();

                    detay.surecYil = VeriErisim.DegerAl(dr, 0, 0);
                    detay.muhasebe = VeriErisim.DegerAl(dr, 1, 0);
                    detay.birim = VeriErisim.DegerAl(dr, 2, "");
                    detay.surecNo = VeriErisim.DegerAl(dr, 3, "");

                    detay.faturaNo = VeriErisim.DegerAl(dr, 4, "");
                    detay.faturaTarihi = VeriErisim.DegerAl(dr, 5);
                    detay.aciklama = VeriErisim.DegerAl(dr, 6, "");

                    detay.firmaAdi = VeriErisim.DegerAl(dr, 7, "");
                    detay.firmaKodu = VeriErisim.DegerAl(dr, 8, "");
                    detay.mIFYil = VeriErisim.DegerAl(dr, 9, 0);
                    detay.mIFNo = VeriErisim.DegerAl(dr, 10, "");

                    detay.yevmiyeTarih = VeriErisim.DegerAl(dr, 11);
                    detay.yevmiyeNo = VeriErisim.DegerAl(dr, 12, 0);
                    detay.mifDurum = VeriErisim.DegerAl(dr, 13, 0);


                    detay.faturaTutari = VeriErisim.DegerAl(dr, 14, (double)0);
                    detay.faturaTutariKDVLI = detay.faturaTutari;

                    if (detay.mIFYil == 0) detay.mIFYil = detay.surecYil;

                    alAna.Add(detay);
                }
                dr.Close();
            }
            ve.VeriErisimKapat();
            return (FaturaTum[])alAna.ToArray(typeof(FaturaTum));
        }
    }
}
