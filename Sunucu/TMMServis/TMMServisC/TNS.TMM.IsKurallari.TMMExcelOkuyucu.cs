using System;

namespace TNS.TMM.IsKurallari
{
    /// <summary>
    /// Eski TMMAktarim, aktarım dosyasını (Excel) istemci kitaplığındaki OrtakClass.Tablo sınıfıyla okuyordu
    /// (GenelIslemler.NewTablo). O kitaplık FarPoint/Ext/Aspose'a bağlı olduğu için sunucuya taşınmadı.
    /// Bu sınıf yalnızca derlemeyi sağlayan yer tutucudur: bir Excel okuyucu seçilip DosyaAc ve HucreDegerAl
    /// yazılana kadar taşınır aktarım dosyası işlenemez.
    /// </summary>
    internal class TMMExcelOkuyucu
    {
        private static Exception Tasinmadi()
        {
            return new NotSupportedException("Taşınır aktarım dosyasının (Excel) okunması yeni sisteme henüz taşınmadı.");
        }

        public void DosyaAc(string dosyaYol)
        {
            throw Tasinmadi();
        }

        public string HucreDegerAl(int satir, int sutun)
        {
            throw Tasinmadi();
        }

        public double HucreDegerAlDbl(int satir, int sutun)
        {
            throw Tasinmadi();
        }
    }
}
