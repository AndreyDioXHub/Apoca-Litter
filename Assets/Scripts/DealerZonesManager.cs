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

        // ������� ��������� ������ ��� ������
        List<DealerZone> availableZones = new List<DealerZone>(_zones);
        List<DealerZone> usedAsSiblings = new List<DealerZone>();

        // ������� ���������� ��� ������� ��������
        foreach (DealerZone zone in _zones)
        {
            if (zone != null)
            {
                zone.SetNewSibling(null);
            }
        }

        // �������� �������� ���������� ���������
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

            // ������� ������ ��������� ��������� ��� ������� ����
            List<DealerZone> possibleSiblings = new List<DealerZone>();

            foreach (DealerZone candidate in availableZones)
            {
                if (candidate == null) 
                { 
                    continue;
                } 

                if (candidate == currentZone)
                {
                    continue;// �� ��������� ������ ����
                } 

                if (usedAsSiblings.Contains(candidate))
                {
                    continue; // �� ���������� ��� �������
                } 

                possibleSiblings.Add(candidate);
            }

            // ���� ��� ��������� ��������� - ����������
            if (possibleSiblings.Count == 0)
            {
                continue;
            }

            // �������� ���������� ��������
            int randomIndex = Random.Range(0, possibleSiblings.Count);
            DealerZone selectedSibling = possibleSiblings[randomIndex];

            // ��������� ��������
            currentZone.SetNewSibling(selectedSibling);
            selectedSibling.SetNewSibling(currentZone);

            // �������� ���������� �������� ��� ���������������
            usedAsSiblings.Add(selectedSibling);

            // ������� ������� ���� �� ���������, ����� ��� ����� �������� �������� �����
            availableZones.Remove(currentZone);
        }
    }
}
