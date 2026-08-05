using System.Globalization;

namespace AgronicaNetCore.Base.Services.Culture
{
    public class CultureService : ICultureService
    {
        private enum LanguageCod
        {
            it = 1,
            en = 2,
            fr = 3,
            ch = 4,
            pt = 5,
        }

        public CultureService(IServiceProvider provider) { }

        public void SetUICulture(int linguaCod)
        {
            CultureInfo.CurrentUICulture = new CultureInfo(GetCultureCodeFromLinguaCod(linguaCod));
            //Thread.CurrentThread.CurrentUICulture = new CultureInfo(GetCultureCodeFromLinguaCod(linguaCod));
        }

        private string GetCultureCodeFromLinguaCod(int linguaCod)
        {
            return Enum.GetName(typeof(LanguageCod), linguaCod)!;
        }
    }
}
