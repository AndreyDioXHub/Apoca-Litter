using System;
using System.Collections.Generic;

public static class LocalizationStrings
{
    public static event Action OnLanguageChanged = delegate { };
    //public event Action<string> OnDataPacked = delegate { };

    public static bool first_start = true;

    public static Dictionary<langkey, string> Strings = new Dictionary<langkey, string>()
    {
        { langkey.pistol1, "Пистолет 1"},
        { langkey.pistol2, "Пистолет 2"},
        { langkey.pistol3, "Пистолет 3"},
        { langkey.pistol4, "Пистолет 4"},
        { langkey.smg1, "Пистолет пулемет 1"},
        { langkey.smg2, "Пистолет пулемет 2"},
        { langkey.smg3, "Пистолет пулемет 3"},
        { langkey.smg4, "Пистолет пулемет 4"},
        { langkey.smg5, "Пистолет пулемет 5"},
        { langkey.ar1, "Автомат 1"},
        { langkey.ar2, "Автомат 2"},
        { langkey.ar3, "Автомат 3"},
        { langkey.sniper1, "Снайперка 1"},
        { langkey.sniper2, "Снайперка 2"},
        { langkey.gl1, "Гранатомет"},
        { langkey.rl1, "Ракетница"},
        { langkey.grip, "Рукоять"},
        { langkey.laser, "Лазер"},
        { langkey.muzzle, "Глушитель"},
        { langkey.scope, "Прицел"},
        { langkey.look, "Осмотреть"},
        { langkey.play, "Играть"},
        { langkey.expensive, "Дорого"},
        { langkey.back, "Назад"},
        { langkey.die, "Помер"},
        { langkey.press_eny_key, "Нажмите любую кнопку для выхода в меню"},
        { langkey.reward, "Награда"},
        { langkey.successful_extraction, "Успешное извлечение"},
        { langkey.loading, "Загрузка"},
        { langkey.sensitivity, "Чувствительность мыши"},
        { langkey.lang, "Русский"},
        { langkey.audiolvl, "Громкость"},
        { langkey.exit, "Выход"},
        { langkey.grenade, "Граната"}
    };

    public static Dictionary<langkey, string> StringsRu = new Dictionary<langkey, string>()
    {
        { langkey.pistol1, "Пистолет 1"},
        { langkey.pistol2, "Пистолет 2"},
        { langkey.pistol3, "Пистолет 3"},
        { langkey.pistol4, "Пистолет 4"},
        { langkey.smg1, "Пистолет пулемет 1"},
        { langkey.smg2, "Пистолет пулемет 2"},
        { langkey.smg3, "Пистолет пулемет 3"},
        { langkey.smg4, "Пистолет пулемет 4"},
        { langkey.smg5, "Пистолет пулемет 5"},
        { langkey.ar1, "Автомат 1"},
        { langkey.ar2, "Автомат 2"},
        { langkey.ar3, "Автомат 3"},
        { langkey.sniper1, "Снайперка 1"},
        { langkey.sniper2, "Снайперка 2"},
        { langkey.gl1, "Гранатомет"},
        { langkey.rl1, "Ракетница"},
        { langkey.grip, "Рукоять"},
        { langkey.laser, "Лазер"},
        { langkey.muzzle, "Глушитель"},
        { langkey.scope, "Прицел"},
        { langkey.look, "Осмотреть"},
        { langkey.play, "Играть"},
        { langkey.expensive, "Дорого"},
        { langkey.back, "Назад"},
        { langkey.die, "Помер"},
        { langkey.press_eny_key, "Нажмите любую кнопку для выхода в меню"},
        { langkey.reward, "Награда"},
        { langkey.successful_extraction, "Успешное извлечение"},
        { langkey.loading, "Загрузка"},
        { langkey.sensitivity, "Чувствительность мыши"},
        { langkey.lang, "Русский"},
        { langkey.audiolvl, "Громкость"},
        { langkey.exit, "Выход"},
        { langkey.grenade, "Граната"}
    };

    public static Dictionary<langkey, string> StringsEn = new Dictionary<langkey, string>()
    {
        { langkey.pistol1, "Handgun 1"},
        { langkey.pistol2, "Handgun 2"},
        { langkey.pistol3, "Handgun 3"},
        { langkey.pistol4, "Handgun 4"},
        { langkey.smg1, "SMG 1"},
        { langkey.smg2, "SMG 2"},
        { langkey.smg3, "SMG 3"},
        { langkey.smg4, "SMG 4"},
        { langkey.smg5, "SMG 5"},
        { langkey.ar1, "AR 1"},
        { langkey.ar2, "AR 2"},
        { langkey.ar3, "AR 3"},
        { langkey.sniper1, "SR 1"},
        { langkey.sniper2, "SR 2"},
        { langkey.gl1, "Grenade Launcher"},
        { langkey.rl1, "Rocket Launcher"},
        { langkey.grip, "Grip"},
        { langkey.laser, "Laser"},
        { langkey.muzzle, "Muzzle"},
        { langkey.scope, "Scope"},
        { langkey.look, "Look"},
        { langkey.play, "Play"},
        { langkey.expensive, "Expensive"},
        { langkey.back, "Back"},
        { langkey.die, "Died"},
        { langkey.press_eny_key, "Press any button to exit the menu"},
        { langkey.reward, "Reward"},
        { langkey.successful_extraction, "Successful extraction"},
        { langkey.loading, "Loading"},
        { langkey.sensitivity, "Mouse sensitivity"},
        { langkey.lang, "English"},
        { langkey.audiolvl, "Audio lvl"},
        { langkey.exit, "Exit"},
        { langkey.grenade, "Grenade"}
    };

    public static void SetLanguage(string lang)
    {
        switch (lang)
        {
            case "ru":
                Strings = StringsRu;
                break;
            case "en":
                Strings = StringsEn;
                break;
            default:
                break;
        }
        OnLanguageChanged?.Invoke();
    } 

}

public enum langkey
{
    pistol1,
    pistol2,
    pistol3,
    pistol4,
    smg1,
    smg2,
    smg3,
    smg4,
    smg5,
    ar1,
    ar2,
    ar3,
    sniper1,
    sniper2,
    gl1,
    rl1,
    grip,
    laser,
    muzzle,
    scope,
    look,
    play,
    expensive,
    back,
    die,
    press_eny_key,
    reward,
    successful_extraction,
    loading,
    sensitivity,
    lang,
    audiolvl,
    exit,
    grenade

}
