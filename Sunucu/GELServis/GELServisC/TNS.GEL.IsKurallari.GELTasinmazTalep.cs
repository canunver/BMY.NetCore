using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TNS.Veri;
using TNS.KYM;
using TNS.GEL.IsKurallari.Veri;
using System.Data;
namespace TNS.GEL.IsKurallari
{
    public class GELTasinmazTalep
    {
        public static ObjectArray Listele(VeriErisim veriErisim, Kullanici kullanan, TasinmazTalep tasinmazTalep)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriGEL");
            TABTasinmazTalep tabTasinmazTalep = new TABTasinmazTalep(ve);
            ObjectArray tasinmazTalepListesi = new ObjectArray();
            ve.BeginTransaction();
            IDataReader dr = tabTasinmazTalep.Listele(tasinmazTalep);
            if (dr != null)
            {
                while (dr.Read())
                {
                    TasinmazTalep ttalep = new TasinmazTalep();
                    ttalep.tasinmazYil = VeriErisim.DegerAl(dr, 0, 0);
                    ttalep.tasinmazKod = VeriErisim.DegerAl(dr, 1, "");
                    ttalep.firmaKodu = VeriErisim.DegerAl(dr, 2, 0);
                    ttalep.siraNo = VeriErisim.DegerAl(dr, 3, 0);
                    ttalep.yaziNo = VeriErisim.DegerAl(dr, 4, 0);
                    ttalep.yaziTarihi = VeriErisim.DegerAl(dr, 5);
                    ttalep.aciklama = VeriErisim.DegerAl(dr, 6, "");
                    ttalep.sonucKodu = VeriErisim.DegerAl(dr, 7, 0);
                    switch (ttalep.sonucKodu)
                    {
                        case (int)ENUMTalepSonuc.Tanimsiz:
                            ttalep.sonucAciklama = "Tanımsız";
                            break;
                        case (int)ENUMTalepSonuc.OnayAsamasinda:
                            ttalep.sonucAciklama = "Onay Aşamasında";
                            break;
                        case (int)ENUMTalepSonuc.Onaylandi:
                            ttalep.sonucAciklama = "Onaylandı";
                            break;
                        case (int)ENUMTalepSonuc.RedEdildi:
                            ttalep.sonucAciklama = "Red Edildi";
                            break;
                        default:
                            break;
                    }
                    ttalep.sonIslemYapanKodu = VeriErisim.DegerAl(dr, 8, "");
                    ttalep.firmaAd = VeriErisim.DegerAl(dr, 9, "");
                    ttalep.tasinmazAdi = VeriErisim.DegerAl(dr, 10, "");
                    tasinmazTalepListesi.objeler.Add(ttalep);
                }
                dr.Close();

            }
            ve.VeriErisimKapat();
            return tasinmazTalepListesi;
        }

        public static Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, TasinmazTalep bilgi)
        {
            Sonuc sonuc = new Sonuc(false, TNS.GEL.GelirSurecleriResI.GEL00001, "");
            if (!Yetki.YazmaYetkisiVarMi(kullanan))
            {
                sonuc.hataStr = TNS.GEL.GelirSurecleriResI.GEL00003;
                sonuc.islemSonuc = false;
                return sonuc;
            }
            if (bilgi.tasinmazYil <= 0)
            {
                sonuc.hataStr = "Taşınmaz yılı sıfıdan büyük olmalıdır";
                sonuc.islemSonuc = false;
                return sonuc;
            }

            if (string.IsNullOrEmpty(bilgi.tasinmazKod))
            {
                sonuc.hataStr = TNS.GEL.GelirSurecleriResI.GEL00009;
                sonuc.islemSonuc = false;
                return sonuc;
            }
            if (bilgi.firmaKodu <= 0)
            {
                sonuc.hataStr = TNS.GEL.GelirSurecleriResI.GEL00010;
                sonuc.islemSonuc = false;
                return sonuc;
            }
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriGEL");
            TABTasinmazTalep tabTalep = new TABTasinmazTalep(ve);
            ve.BeginTransaction();
            bool kayitvar = tabTalep.KayitVarmi(bilgi);
            if (kayitvar) //güncelle
            {
                sonuc = tabTalep.Guncelle(bilgi);
            }
            else//kaydet
            {
                if (bilgi.siraNo <= 0)
                    bilgi.siraNo = tabTalep.TalepTasinmazSiraNoAl(bilgi);
                sonuc = tabTalep.Kaydet(bilgi);
            }
            if (sonuc.islemSonuc)
            {
                sonuc.anahtar = bilgi.siraNo.ToString();
                ve.Commit();
                sonuc.bilgiStr = TNS.GEL.GelirSurecleriResI.GEL00012;
            }
            else
            {
                ve.Rollback();
            }
            ve.VeriErisimKapat();
            return sonuc;

        }

        public static Sonuc Sil(VeriErisim veriErisim, Kullanici kullanan, TasinmazTalep bilgi, TasinmazTalepEki[] talepEkleri)
        {
            Sonuc sonuc = new Sonuc(true,  "", "");
            if (!Yetki.YazmaYetkisiVarMi(kullanan))
            {
                sonuc.hataStr = TNS.GEL.GelirSurecleriResI.GEL00003;
                sonuc.islemSonuc = false;
                return sonuc;
            }
            if (string.IsNullOrEmpty(bilgi.tasinmazKod))
            {
                sonuc.hataStr = TNS.GEL.GelirSurecleriResI.GEL00009;
                sonuc.islemSonuc = false;
                return sonuc;
            }
            if (bilgi.tasinmazYil <= 0)
            {
                sonuc.hataStr = "Taşınmaz yılı sıfırdan büyük olmalıdır";
                sonuc.islemSonuc = false;
                return sonuc;
            }

            if (bilgi.firmaKodu <= 0)
            {
                sonuc.hataStr = TNS.GEL.GelirSurecleriResI.GEL00010;
                sonuc.islemSonuc = false;
                return sonuc;
            }
            if (bilgi.siraNo <= 0)
            {
                sonuc.hataStr = TNS.GEL.GelirSurecleriResI.GEL00011;
                sonuc.islemSonuc = false;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriGEL");
            TABTasinmazTalep tabTalep = new TABTasinmazTalep(ve);
            ve.BeginTransaction();
            //********************tasinmazı silerken önce tasınmaza ait ekleri siliyoruz
            TABTasinmazTalepEkDosya tabTalepEk = new TABTasinmazTalepEkDosya(ve);
            if (talepEkleri.Length > 0)
            {
                for (int i = 0; i < talepEkleri.Length; i++)
                {
                    tabTalepEk.Sil(talepEkleri[i], true);
                    //**********ek silinirken en az birinde sorun oluşursa silme işlemi iptal ediliyor
                    if (ve.hata.Trim() != "")
                    {
                        sonuc.DegerAta(ve.hata);
                        ve.Rollback();
                        break;
                    }
                }
            }

            //ekler başarılı şekilde silindi ise talebi siliyoruz
            if (sonuc.islemSonuc)
            {
                tabTalep.Sil(bilgi);
                //***talep başarılı şekild silinirse transaction commit ediliyor
                if (ve.hata.Trim() == "")
                {
                    sonuc.islemSonuc = true;
                    sonuc.hataStr = "";
                    sonuc.bilgiStr = TNS.GEL.GelirSurecleriResI.GEL00013;
                    ve.Commit();
                }
                else
                {
                    ve.Rollback();
                    sonuc.islemSonuc = false;
                    sonuc.hataStr = ve.hata;
                }
            }
            ve.VeriErisimKapat();
            return sonuc;
        }

    }
}
