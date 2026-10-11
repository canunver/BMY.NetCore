using System.Data;
using TNS.KYM;
using TNS.TMM.IsKurallari.Veri;
using TNS.Veri;

namespace TNS.TMM.IsKurallari
{
    public class TMMHarmoni
    {
        public static Sonuc Kaydet(Kullanici kullanan, Harmoni form)
        {
            Sonuc sonuc = new Sonuc(false, TasinirMalResI.TMM069, "");

            string hata = "";
            if (string.IsNullOrWhiteSpace(form.sicilNo))
                hata = "Sicil No bilgisi boş bırakılamaz." + "<br>";
            if (form.tip <= 0)
                hata += "Tip bilgisi boş bırakılamaz." + "<br>";
            if (string.IsNullOrWhiteSpace(form.aciklama))
                hata = "Açıklama bilgisi boş bırakılamaz." + "<br>";


            if (hata == "")
            {
                //SicilNo kontrol eklenebilir
            }


            if (hata != "")
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = hata;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABHarmoni to = new TABHarmoni(ve);
            ve.BeginTransaction();

            sonuc = to.Kaydet(form);

            if (sonuc.islemSonuc)
            {
                sonuc.bilgiStr = TasinirMalResI.TMM118;
                ve.Commit();
            }
            else
            {
                sonuc.hataStr = TasinirMalResI.TMM119 + "<br>" + sonuc.hataStr;
                ve.Rollback();
            }

            ve.VeriErisimKapat();

            return sonuc;
        }

        public static ObjectArray Listele(Kullanici kullanan, Harmoni form)
        {
            ObjectArray liste = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));

            TABHarmoni to = new TABHarmoni(ve);
            IDataReader dr = to.Listele(form);

            if (dr != null)
            {
                while (dr.Read())
                {
                    form = new Harmoni();
                    form.sicilNo = VeriErisim.DegerAl(dr, (int)TABHarmoni.ENUMAlanlar.SICILNO, "");
                    form.tip = (ENUMHarmoniTip)VeriErisim.DegerAl(dr, (int)TABHarmoni.ENUMAlanlar.TIP, 0);
                    form.aciklama = VeriErisim.DegerAl(dr, (int)TABHarmoni.ENUMAlanlar.ACIKLAMA, "");
                    form.islemTarih = VeriErisim.DegerAl(dr, (int)TABHarmoni.ENUMAlanlar.ISLEMTARIH);

                    liste.objeler.Add(form);
                }
                dr.Close();
            }
            else
                liste.sonuc.DegerAta("Hata Oluştu. Hata Detayı : " + to.Hata());


            ve.VeriErisimKapat();

            return liste;
        }

        public static Harmoni Oku(Kullanici kullanan, Harmoni form)
        {
            ObjectArray liste = Listele(kullanan, form);
            if (liste.objeler.Count > 0)
                return (Harmoni)liste.objeler[0];
            return new Harmoni();
        }
    }
}
