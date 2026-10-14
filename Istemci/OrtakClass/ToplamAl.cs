namespace OrtakClass
{
    /// <summary>Eski OrtakClass/GenelIslemler.cs'teki ToplamAl sınıfı (raporlarda ara toplamları tutar); olduğu gibi alındı.</summary>
    public class ToplamAl
    {
        int sat;
        int sut;
        public double[,] toplam;

        public string ToplamStr(int sat, int sut, int noktaSay)
        {
            string ek;
            if (noktaSay > 0)
                ek = "." + "00";
            else
                ek = "";
            return toplam[sat, sut].ToString("#,##0" + ek);
        }

        public ToplamAl(int sat, int sut)
        {
            this.sat = sat;
            this.sut = sut;
            toplam = new double[sat, sut];
            HepsiniSifirla();
        }

        public void HepsiniSifirla()
        {
            for (int i = 0; i < sat; i++)
            {
                SatirSifirla(i);
            }
        }

        public void SatirSifirla(int sifirSatir)
        {
            for (int i = 0; i < sut; i++)
            {
                toplam[sifirSatir, i] = 0;
            }
        }

        public void ToplamEkle(int hedefSat, int kaynakSat)
        {
            for (int i = 0; i < sut; i++)
            {
                toplam[hedefSat, i] += toplam[kaynakSat, i];
            }
        }

        public void DegerAta(int sat, int sut, double deger)
        {
            toplam[sat, sut] = deger;
        }

        public void DegerArttir(int sat, int sut, double deger)
        {
            toplam[sat, sut] += deger;
        }

        public int SutunSay()
        {
            return sut;
        }

        public double SatToplamAl(int sat, int basSut, int bitSut)
        {
            double tut = 0;
            for (int i = basSut; i <= bitSut; i++)
            {
                tut += toplam[sat, i];
            }
            return tut;
        }
    }
}
