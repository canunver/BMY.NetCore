using System.Collections.Concurrent;

namespace TNS
{
    /// <summary>
    /// Eski kodda System.Web.HttpRuntime.Cache ile yapılan süresiz, uygulama ömürlü önbelleğin karşılığı.
    /// </summary>
    public static class Onbellek
    {
        private static readonly ConcurrentDictionary<string, object> veri = new ConcurrentDictionary<string, object>();

        public static object Get(string anahtar)
        {
            object deger;
            return veri.TryGetValue(anahtar, out deger) ? deger : null;
        }

        public static void Insert(string anahtar, object deger)
        {
            if (deger == null)
            {
                Remove(anahtar);
                return;
            }
            veri[anahtar] = deger;
        }

        public static object Remove(string anahtar)
        {
            object deger;
            veri.TryRemove(anahtar, out deger);
            return deger;
        }
    }
}
