using Cysharp.Threading.Tasks;
using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DealerZonesManager : MonoBehaviour
{
    
    public static DealerZonesManager Instance;

    [SerializeField]
    private List<DealerZone> _zones = new List<DealerZone>();

    private bool _needInitZones = true;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        InitZones();
    }

    void Update()
    {
        
    }
    
    public void HideDefaultMarkers()
    {
        foreach (DealerZone zone in _zones)
        {
            zone.HideDefaultMarker();
        }
    }

    public void ShowDefaultMarkers()
    {
        foreach (DealerZone zone in _zones)
        {
            zone.ShowDefaultMarker();
        }
    }

    public void RegisterZone(DealerZone zone)
    {
        _zones.Add(zone);
    }    
    
    public async void InitZones()
    {
        await UniTask.WaitForSeconds(0.1f);

        AssignRandomSiblings();
        /*
        for (int i = 0; i < _zones.Count; i++)
        {

        }*/
    }

    [ContextMenu("Assign Random Siblings")]
    public void AssignRandomSiblings()
    {
        if (_zones == null || _zones.Count < 2)
        {
            return;
        }

        // Создаем временные списки для работы
        List<DealerZone> availableZones = new List<DealerZone>(_zones);
        List<DealerZone> usedAsSiblings = new List<DealerZone>();

        // Сначала сбрасываем все текущие сиблинги
        foreach (DealerZone zone in _zones)
        {
            if (zone != null)
            {
                zone.SetNewSibling(null);
            }
        }

        // Основной алгоритм назначения сиблингов
        foreach (DealerZone currentZone in _zones)
        {
            if (currentZone == null)
            { 
                continue; 
            }

            if (usedAsSiblings.Contains(currentZone))
            {
                continue;
            }

            // Создаем список возможных сиблингов для текущей зоны
            List<DealerZone> possibleSiblings = new List<DealerZone>();

            foreach (DealerZone candidate in availableZones)
            {
                if (candidate == null) 
                { 
                    continue;
                } 

                if (candidate == currentZone)
                {
                    continue;// Не назначаем самого себя
                } 

                if (usedAsSiblings.Contains(candidate))
                {
                    continue; // Не используем уже занятые
                } 

                possibleSiblings.Add(candidate);
            }

            // Если нет возможных вариантов - пропускаем
            if (possibleSiblings.Count == 0)
            {
                continue;
            }

            // Выбираем случайного сиблинга
            int randomIndex = Random.Range(0, possibleSiblings.Count);
            DealerZone selectedSibling = possibleSiblings[randomIndex];

            // Назначаем сиблинга
            currentZone.SetNewSibling(selectedSibling);
            selectedSibling.SetNewSibling(currentZone);

            // Помечаем выбранного сиблинга как использованного
            usedAsSiblings.Add(selectedSibling);

            // Удаляем текущую зону из доступных, чтобы она могла получить сиблинга позже
            availableZones.Remove(currentZone);
        }
    }
}
