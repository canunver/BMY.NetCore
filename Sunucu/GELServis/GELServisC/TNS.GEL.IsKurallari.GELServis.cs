using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TNS.UZY;
using TNS.KYM;


namespace TNS.GEL.IsKurallari
{
    [Serializable()]
    public class GELServis : MarshalByRefObject, IGELServis
    {
        public static IUZYServis servisUZY = null;

        public GELServis()
        {
        }

        public static string UzayYaziDegerGetir(string uzayAd, string kosul, bool esitle, string uzayAlan)
        {
            if (servisUZY == null)
                servisUZY = TNS.UZY.Arac.Tanimla();

            string deger = "";
            if (uzayAlan == "")
                deger = servisUZY.UzayDegeriStr(new TNS.KYM.Kullanici(), uzayAd, kosul, esitle);
            else
                deger = servisUZY.UzayDegeriStr(new TNS.KYM.Kullanici(), uzayAd, kosul, esitle, uzayAlan);

            return deger;
        }

        public Sonuc TasinmazKaydet(KYM.Kullanici kullanan, Tasinmaz bilgi)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = GELTasinmazTanim.Kaydet(null, kullanan, bilgi);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        public Sonuc TasinmazSil(KYM.Kullanici kullanan, Tasinmaz bilgi)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = GELTasinmazTanim.Sil(null, kullanan, bilgi);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        public ObjectArray TasinmazListele(KYM.Kullanici kullanan, Tasinmaz bilgi)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = GELTasinmazTanim.Listele(null, kullanan, bilgi);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }
            return donusDeger;
        }

        public ObjectArray TasinmazTalepListele(KYM.Kullanici kullanan, TasinmazTalep bilgi)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = GELTasinmazTalep.Listele(null, kullanan, bilgi);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }
            return donusDeger;
        }

        public Sonuc TasinmazTalepKaydet(KYM.Kullanici kullanan, TasinmazTalep bilgi)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = GELTasinmazTalep.Kaydet(null, kullanan, bilgi);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;

        }

        public Sonuc TasinmazTalepSil(KYM.Kullanici kullanan, TasinmazTalep bilgi, TasinmazTalepEki[] talepEkleri)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = GELTasinmazTalep.Sil(null, kullanan, bilgi, talepEkleri);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        public ObjectArray TasinmazTalepEkListele(KYM.Kullanici kullanan, TasinmazTalepEki bilgi)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = GELTasinmazTalepEkDosya.Listele(null, kullanan, bilgi);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }
            return donusDeger;
        }

        public Sonuc TasinmazTalepEkiKaydet(KYM.Kullanici kullanan, TasinmazTalepEki bilgi)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = GELTasinmazTalepEkDosya.Kaydet(null, kullanan, bilgi);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        public Sonuc TasinmazTalepEkiSil(KYM.Kullanici kullanan, TasinmazTalepEki bilgi, bool ekNoKontrol = true)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = GELTasinmazTalepEkDosya.Sil(null, kullanan, bilgi, ekNoKontrol);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;

        }

        public ObjectArray IhaleTurListele(KYM.Kullanici kullanan, IhaleTur kriter)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = GELIhaleTur.Listele(kullanan, kriter);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }
            return donusDeger;
        }
        public Sonuc IhaleTurKaydet(KYM.Kullanici kullanan, IhaleTur bilgi)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = GELIhaleTur.Kaydet(null, kullanan, bilgi);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }
        public Sonuc IhaleTurSil(KYM.Kullanici kullanan, IhaleTur bilgi)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = GELIhaleTur.Sil(null, kullanan, bilgi);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        public ObjectArray IhaleTurMaddeListele(KYM.Kullanici kullanan, int ihaleKod, int tur)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = GELIhaleTurMadde.Listele(null, kullanan, ihaleKod, tur);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }
            return donusDeger;
        }
        public Sonuc IhaleTurMaddeKaydet(KYM.Kullanici kullanan, IhaleTurMadde bilgi)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = GELIhaleTurMadde.Kaydet(null, kullanan, bilgi);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }
        public Sonuc IhaleTurMaddeSil(KYM.Kullanici kullanan, IhaleTurMadde bilgi)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = GELIhaleTurMadde.Sil(null, kullanan, bilgi);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        public ObjectArray IhaleIlanSekliListele(KYM.Kullanici kullanan, int ihaleKod, int ihaleMaddeSira, int tur)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = GELIhaleIlanTanim.Listele(null, kullanan, ihaleKod, ihaleMaddeSira, tur);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }
            return donusDeger;
        }
        public Sonuc IhaleIlanSekliKaydet(KYM.Kullanici kullanan, IhaleIlanTanim bilgi)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = GELIhaleIlanTanim.Kaydet(null, kullanan, bilgi);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }
        public Sonuc IhaleIlanSekliSil(KYM.Kullanici kullanan, IhaleIlanTanim bilgi)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = GELIhaleIlanTanim.Sil(null, kullanan, bilgi);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        public Sonuc SurecKaydet(KYM.Kullanici kullanan, Surec bilgi)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = GELSurec.Kaydet(null, kullanan, bilgi);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }
        public Sonuc SurecGuncelle(KYM.Kullanici kullanan, Surec bilgi)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = GELSurec.Guncelle(null, kullanan, bilgi);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        public Sonuc SurecSifreKaydet(KYM.Kullanici kullanan, SurecSifre bilgi)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = GELSurec.SifreKaydet(null, kullanan, bilgi);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        public Sonuc SurecSifreKaldir(Kullanici kullanan, SurecSifre bilgi)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = GELSurec.SifreKaldir(null, kullanan, bilgi);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        public ObjectArray SurecSifreListele(KYM.Kullanici kullanan, SurecSifre bilgi)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = GELSurec.SifreListele(null, kullanan, bilgi);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }
            return donusDeger;
        }

        public Sonuc SurecDurumGuncelle(KYM.Kullanici kullanan, Surec bilgi)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = GELSurec.DurumGuncelle(null, kullanan, bilgi);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        public ObjectArray SurecParcaDurumListele(KYM.Kullanici kullanan, int yil, string surecno)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = GELSurec.ParcaDurumListele(null, kullanan, yil, surecno, 0, 0);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }
            return donusDeger;
        }

        public ObjectArray SurecListele(KYM.Kullanici kullanan, Surec bilgi)
        {

            ObjectArray donusDeger = new ObjectArray();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = GELSurec.Listele(null, kullanan, bilgi);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }
            return donusDeger;
        }

        public Sonuc SurecParcaDurumGuncelle(KYM.Kullanici kullanan, SurecParcaDurum bilgi)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = GELSurec.ParcaDurumGuncelle(null, kullanan, bilgi);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        public ObjectArray SurecParcaListele(KYM.Kullanici kullanan, int yil, string surecno)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = GELSurec.ParcaListele(null, kullanan, yil, surecno);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }
            return donusDeger;
        }

        public Sonuc SurecTeklifTipiGetir(Kullanici kullanan, Surec bilgi)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = GELSurec.TeklifTipiGetir(null, kullanan, bilgi);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        public ObjectArray ParaBirimiListele(KYM.Kullanici kullanan, TNS.BKD.ParaBirimi paraBirimi)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = GELOrtakIslemler.ParabirimiListele(null, kullanan, paraBirimi);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }
            return donusDeger;
        }

        public ObjectArray OnayBilgisiListele(KYM.Kullanici kullanan, OnayForm bilgi)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = GELOnayForm.Listele(null, kullanan, bilgi);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }
            return donusDeger;
        }

        public Sonuc OnayBilgisiKaydet(KYM.Kullanici kullanan, OnayForm bilgi, int islemkod, int parcakod)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = GELOnayForm.Kaydet(null, kullanan, bilgi, islemkod, parcakod);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }


        public ObjectArray MuhammenBedelListele(KYM.Kullanici kullanan, MuhammenBedel bilgi)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = GELMuhammenBedel.Listele(null, kullanan, bilgi);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }
            return donusDeger;
        }
        public Sonuc MuhammenBedelKaydet(KYM.Kullanici kullanan, MuhammenBedel bilgi, int islemkod, int parcakod)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = GELMuhammenBedel.Kaydet(null, kullanan, bilgi, islemkod, parcakod);

            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }
        public Sonuc MuhammenBedelSil(KYM.Kullanici kullanan, MuhammenBedel bilgi, int islemkod, int parcakod)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = GELMuhammenBedel.Sil(null, kullanan, bilgi, islemkod, parcakod);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        public Sonuc KomisyonUyesiKaydet(KYM.Kullanici kullanan, KomisyonUye bilgi, int islemkod, int parcakod)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = GELKomisyonUye.Kaydet(null, kullanan, bilgi, islemkod, parcakod);

            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }
        public Sonuc KomisyonUyesiSil(KYM.Kullanici kullanan, KomisyonUye bilgi, int islemkod, int parcakod)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = GELKomisyonUye.Sil(null, kullanan, bilgi, islemkod, parcakod);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;

        }
        public ObjectArray KomisyonUyesiListele(KYM.Kullanici kullanan, KomisyonUye bilgi)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = GELKomisyonUye.Listele(null, kullanan, bilgi);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }
            return donusDeger;
        }



        public Sonuc TeklifFirmaKaydet(KYM.Kullanici kullanan, TeklifFirma[] bilgi, int islemkod, int parcakod)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = GELIhaleTeklif.TeklifFirmaKaydet(null, kullanan, bilgi, islemkod, parcakod);

            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        public Sonuc TeklifFirmaSil(KYM.Kullanici kullanan, TeklifFirma bilgi, int islemkod, int parcakod)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = GELIhaleTeklif.TeklifFirmaSil(null, kullanan, bilgi, islemkod, parcakod);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        public ObjectArray TeklifFirmaListele(KYM.Kullanici kullanan, TeklifFirma bilgi)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = GELIhaleTeklif.TeklifFirmaListele(null, kullanan, bilgi);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }
            return donusDeger;
        }


        public Sonuc TeklifTasinmazKaydet(KYM.Kullanici kullanan, TeklifTasinmaz[] bilgi, int islemkod, int parcakod)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = GELIhaleTeklif.TeklifTasinmazKaydet(null, kullanan, bilgi, islemkod, parcakod);

            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        public Sonuc TeklifTasinmazSil(KYM.Kullanici kullanan, TeklifTasinmaz bilgi, int islemkod, int parcakod)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = GELIhaleTeklif.TeklifTasinmazSil(null, kullanan, bilgi, islemkod, parcakod);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        public ObjectArray TeklifTasinmazListele(KYM.Kullanici kullanan, TeklifTasinmaz bilgi)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = GELIhaleTeklif.TeklifTasinmazListele(null, kullanan, bilgi);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }
            return donusDeger;
        }

        public ObjectArray TeklifListele(KYM.Kullanici kullanan, Teklif bilgi)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = GELIhaleTeklif.TeklifListele(null, kullanan, bilgi);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }
            return donusDeger;
        }

        public Sonuc TeklifSil(KYM.Kullanici kullanan, Teklif[] bilgi, int islemkod, int parcakod)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = GELIhaleTeklif.TeklifSil(null, kullanan, bilgi, islemkod, parcakod);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        public Sonuc TeklifKaydet(KYM.Kullanici kullanan, Teklif[] bilgi, int islemkod, int parcakod)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = GELIhaleTeklif.TeklifKaydet(null, kullanan, bilgi, islemkod, parcakod);

            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }


        public Sonuc TeklifKaydiVarmi(KYM.Kullanici kullanan, Teklif bilgi)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = GELIhaleTeklif.TeklifKaydiVarmi(null, kullanan, bilgi);

            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        public ObjectArray KomisyonKarariListele(KYM.Kullanici kullanan, KomisyonKarar bilgi)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = GELIhaleKomisyonKarar.Listele(null, kullanan, bilgi);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }
            return donusDeger;
        }
        public Sonuc KomisyonKarariSil(KYM.Kullanici kullanan, KomisyonKarar[] bilgi, int islemkod, int parcakod)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = GELIhaleKomisyonKarar.Sil(null, kullanan, bilgi, islemkod, parcakod);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }
        public Sonuc KomisyonKarariKaydet(KYM.Kullanici kullanan, KomisyonKarar[] bilgi, int islemkod, int parcakod)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = GELIhaleKomisyonKarar.Kaydet(null, kullanan, bilgi, islemkod, parcakod);

            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        public Sonuc KomisyonKarariVarmi(KYM.Kullanici kullanan, KomisyonKarar bilgi)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = GELIhaleKomisyonKarar.KararVarmi(null, kullanan, bilgi);

            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;

        }

        public ObjectArray SonTeklifGetir(KYM.Kullanici kullanan, Teklif bilgi)
        {
            ObjectArray donusdeger = new ObjectArray();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusdeger = GELIhaleTeklif.SonTeklif(null, kullanan, bilgi);
            }
            catch (Exception e)
            {
                donusdeger.sonuc.islemSonuc = false;
                donusdeger.sonuc.hataStr = e.ToString();
            }
            return donusdeger;
        }

        public Sonuc HakedisKaydet(KYM.Kullanici kullanan, Tahsilat bilgi, int islemkod, int parcakod)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = GELTahsilat.TahsilatBilgiKaydet(null, kullanan, bilgi, islemkod, parcakod);

            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        public ObjectArray HakedisListele(KYM.Kullanici kullanan, Tahsilat bilgi)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = GELTahsilat.TahsilatBilgiListele(null, kullanan, bilgi);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }
            return donusDeger;
        }

        public Sonuc HakedisTaksitSil(KYM.Kullanici kullanan, TasinmazOdemePlan[] bilgi, int islemkod, int parcakod)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = GELTahsilat.TaksitSil(null, kullanan, bilgi, islemkod, parcakod);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }


        public Sonuc MakbuzFormKaydet(KYM.Kullanici kullanan, Makbuz bilgi, int islemkod, int parcakod)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = GELMakbuzForm.Kaydet(null, kullanan, bilgi, islemkod, parcakod);

            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }
        public ObjectArray MakbuzFormListele(KYM.Kullanici kullanan, Makbuz bilgi)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = GELMakbuzForm.Listele(null, kullanan, bilgi);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }
            return donusDeger;
        }
        public Sonuc MakbuzFormSil(KYM.Kullanici kullanan, Makbuz bilgi, int islemkod, int parcakod)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = GELMakbuzForm.MakbuzSil(null, kullanan, bilgi, islemkod, parcakod);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        public Sonuc MakbuzFormDetaySil(KYM.Kullanici kullanan, MakbuzDetay bilgi, int islemkod, int parcakod)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = GELMakbuzForm.MakbuzDetaySil(null, kullanan, bilgi, islemkod, parcakod);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        public Sonuc DosyaKaydet(KYM.Kullanici kullanan, Dosya bilgi)
        {

            Sonuc donusDeger = new Sonuc();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = DosyaIslem.Kaydet(null, kullanan, bilgi);

            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        public Sonuc DosyaSil(KYM.Kullanici kullanan, Dosya bilgi)
        {

            Sonuc donusDeger = new Sonuc();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = DosyaIslem.Sil(null, kullanan, bilgi);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        public ObjectArray DosyaListele(KYM.Kullanici kullanan, Dosya bilgi)
        {

            ObjectArray donusDeger = new ObjectArray();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = DosyaIslem.Listele(null, kullanan, bilgi);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }
            return donusDeger;
        }


        public Sonuc MuhasebeIslemFormKaydet(KYM.Kullanici kullanan, MuhasebeIslemForm bilgi, int islemkod, int parcakod)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = GELMuhasebeIslemForm.Kaydet(null, kullanan, bilgi, islemkod, parcakod);

            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        public Sonuc MuhasebeIslemFormSil(KYM.Kullanici kullanan, MuhasebeIslemForm bilgi, int islemkod, int parcakod)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = GELMuhasebeIslemForm.Sil(null, kullanan, bilgi, islemkod, parcakod);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        public ObjectArray MuhasebeIslemFormListele(KYM.Kullanici kullanan, MuhasebeIslemForm bilgi)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = GELMuhasebeIslemForm.Listele(null, kullanan, bilgi);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }
            return donusDeger;
        }

        public Sonuc IsKaydet(KYM.Kullanici kullanan, Is bilgi)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = GELIs.Kaydet(null, kullanan, bilgi);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        public Sonuc IsSil(KYM.Kullanici kullanan, Is bilgi)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = GELIs.Sil(null, kullanan, bilgi);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        public ObjectArray IsListele(KYM.Kullanici kullanan, Is bilgi)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = GELIs.Listele(null, kullanan, bilgi);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }
            return donusDeger;
        }

        public Sonuc ParcaKaydet(KYM.Kullanici kullanan, TNS.GEL.Parca bilgi)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = GELParca.Kaydet(null, kullanan, bilgi);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        public Sonuc ParcaSil(KYM.Kullanici kullanan, TNS.GEL.Parca bilgi)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = GELParca.Sil(null, kullanan, bilgi);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        public ObjectArray ParcaListele(KYM.Kullanici kullanan, TNS.GEL.Parca bilgi)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = GELParca.Listele(null, kullanan, bilgi);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }
            return donusDeger;
        }

        public Sonuc IsParcaKaydet(KYM.Kullanici kullanan, IsParca bilgi)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = GELIsParca.Kaydet(null, kullanan, bilgi);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        public ObjectArray IsParcaListele(KYM.Kullanici kullanan, int isKod)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = GELIsParca.Listele(null, kullanan, isKod);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }
            return donusDeger;
        }

        public Sonuc OrnekPersonelKaydet(Kullanici kullanan, OrnekPersonel bilgi)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                donusDeger = GELOrnekPersonel.Kaydet(null, kullanan, bilgi);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }


        public Sonuc TasinmazCinsKaydet(Kullanici kullanan, TasinmazCins cins)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return GELTasinmazCins.Kaydet(null, kullanan, cins);
            }
            catch (Exception ex)
            {
                Sonuc sonuc = new Sonuc(false, ex.ToString(), "");
                return sonuc;
            }
        }

        public Sonuc TasinmazCinsKaydet(Kullanici kullanan, TasinmazCins[] cinsler)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return GELTasinmazCins.Kaydet(null, kullanan, cinsler);
            }
            catch (Exception ex)
            {
                Sonuc sonuc = new Sonuc(false, ex.ToString(), "");
                return sonuc;
            }
        }

        public Sonuc TasinmazCinsSil(Kullanici kullanan, string kod)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return GELTasinmazCins.Sil(null, kullanan, kod);
            }
            catch (Exception ex)
            {
                Sonuc sonuc = new Sonuc(false, ex.ToString(), "");
                return sonuc;
            }
        }

        public ObjectArray TasinmazCinsListele(Kullanici kullanan, TasinmazCins cins)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return GELTasinmazCins.Listele(null, kullanan, cins);
            }
            catch (Exception ex)
            {
                ObjectArray sonuclar = new ObjectArray();
                sonuclar.sonuc.DegerAta(ex.ToString());
                return sonuclar;
            }
        }


        public Sonuc OdemePlaniKaydet(Kullanici kullanan, TasinmazOdemePlan[] bilgiler)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return GELTasinmazOdemePlani.Kaydet(null, kullanan, bilgiler);
            }
            catch (Exception ex)
            {
                Sonuc sonuc = new Sonuc(false, ex.ToString(), "");
                return sonuc;
            }
        }

        public Sonuc OdemePalniSil(Kullanici kullanan, TasinmazOdemePlan bilgi)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return GELTasinmazOdemePlani.Sil(null, kullanan, bilgi);
            }
            catch (Exception ex)
            {
                Sonuc sonuc = new Sonuc(false, ex.ToString(), "");
                return sonuc;
            }
        }

        public ObjectArray OdemePalniListele(Kullanici kullanan, TasinmazOdemePlan bilgi)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return GELTasinmazOdemePlani.Listele(null, kullanan, bilgi);
            }
            catch (Exception ex)
            {
                ObjectArray sonuclar = new ObjectArray();
                sonuclar.sonuc.DegerAta(ex.ToString());
                return sonuclar;
            }


        }

        public ObjectArray OdemePlaniFirmaKisiListele(Kullanici kullanan, string tasinmazSistemKod, int tasinmazYili)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return GELTasinmazOdemePlani.TasinmazaAitFirmaKisiListele(null, kullanan, tasinmazSistemKod, tasinmazYili);
            }
            catch (Exception ex)
            {
                ObjectArray sonuclar = new ObjectArray();
                sonuclar.sonuc.DegerAta(ex.ToString());
                return sonuclar;
            }
        }

        public ObjectArray TasinmazIcmalListele(Kullanici kullanan, TasinmazIcmal icmal)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return GELTasinmazTanim.IcmalListele(null, kullanan, icmal);
            }
            catch (Exception ex)
            {
                ObjectArray sonuclar = new ObjectArray();
                sonuclar.sonuc.DegerAta(ex.ToString());
                return sonuclar;
            }
        }

        public Sonuc TasinmazBolgeKaydet(Kullanici kullanan, Bolge bilgi)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return GELTasinmazBolge.Kaydet(null, kullanan, bilgi);
            }
            catch (Exception ex)
            {
                Sonuc sonuc = new Sonuc(false, ex.ToString(), "");
                return sonuc;
            }
        }

        public ObjectArray TasinmazBolgeListele(Kullanici kullanan, Bolge bilgi)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return GELTasinmazBolge.Listele(null, kullanan, bilgi);
            }
            catch (Exception ex)
            {
                ObjectArray sonuclar = new ObjectArray();
                sonuclar.sonuc.DegerAta(ex.ToString());
                return sonuclar;
            }
        }

        public Sonuc TasinmazBolgeSil(Kullanici kullanan, TNSCollection bilgiler)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return GELTasinmazBolge.Sil(null, kullanan, bilgiler);
            }
            catch (Exception ex)
            {
                Sonuc sonuc = new Sonuc(false, ex.ToString(), "");
                return sonuc;
            }
        }

        public Sonuc TasinmazBolgeIlKaydet(Kullanici kullanan, Bolge bolge)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return GELTasinmazBolgeIl.Kaydet(null, kullanan, bolge);
            }
            catch (Exception ex)
            {
                Sonuc sonuc = new Sonuc(false, ex.ToString(), "");
                return sonuc;
            }
        }

        public ObjectArray TasinmazBolgeIlListele(Kullanici kullanan, Bolge bolge)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return GELTasinmazBolgeIl.Listele(null, kullanan, bolge);
            }
            catch (Exception ex)
            {
                ObjectArray sonuclar = new ObjectArray();
                sonuclar.sonuc.DegerAta(ex.ToString());
                return sonuclar;
            }
        }

        public Sonuc TasinmazBolgeIlSil(Kullanici kullanan, Bolge bolge)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return GELTasinmazBolgeIl.Sil(null, kullanan, bolge);
            }
            catch (Exception ex)
            {
                Sonuc sonuc = new Sonuc(false, ex.ToString(), "");
                return sonuc;
            }
        }

        public Sonuc TasinmazEkDosyaKaydet(Kullanici kullanan, TasinmazEkDosya dosya)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return GELTasinmazEkDosya.Kaydet(null, kullanan, dosya);
            }
            catch (Exception ex)
            {
                Sonuc sonuc = new Sonuc(false, ex.ToString(), "");
                return sonuc;
            }
        }

        public Sonuc TasinmazEkDosyaSil(Kullanici kullanan, TasinmazEkDosya dosya)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return GELTasinmazEkDosya.Sil(null, kullanan, dosya);
            }
            catch (Exception ex)
            {
                Sonuc sonuc = new Sonuc(false, ex.ToString(), "");
                return sonuc;
            }
        }

        public ObjectArray TasinmazEkDosyaListele(Kullanici kullanan, TasinmazEkDosya dosya)
        {
            try
            {
                TNS.KYM.Arac.KulturDegistir(kullanan);
                return GELTasinmazEkDosya.Listele(null, kullanan, dosya);
            }
            catch (Exception ex)
            {
                ObjectArray sonuclar = new ObjectArray();
                sonuclar.sonuc.DegerAta(ex.ToString());
                return sonuclar;
            }
        }
    }
}
