using Unity.VisualScripting;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization;

public class Localize
{
    public static string GetStr(string key)
    {
        return LocalizationSettings.StringDatabase.GetLocalizedString("String Table", key, LocalizationSettings.SelectedLocale);
    }

    static public void LocaleChange(int index)
    {
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[index];
    }
}
