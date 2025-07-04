using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CharacterValues
{
    public static event Action<CharacterValueKey, object> OnLanguageChanged = delegate { };

    private static Dictionary<CharacterValueKey, object> _values = new Dictionary<CharacterValueKey, object>();

    public static void UpdateValue(CharacterValueKey key, object incomeValue)
    {
        if(_values.TryGetValue(key, out object value))
        {
            _values[key] = incomeValue;
        }
        else
        {
            _values.Add(key, incomeValue);
        }

        OnLanguageChanged?.Invoke(key, _values[key]);
    }
}

public enum CharacterValueKey
{
    AmmoCountTotal,
    AmmoCountCurent,
    LifeLVL,
    GrenadeCountCurent
}

/*
            {AmmoType.grenade, 5},
            {AmmoType.grenadeLaunched, 5},
            {AmmoType.rocket, 3},
            {AmmoType.rifle, 90},
            {AmmoType.smg, 150},
            {AmmoType.shotgun, 32},
            {AmmoType.sniperRifle, 40}
 */
