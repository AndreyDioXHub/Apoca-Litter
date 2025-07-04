using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorTree : MonoBehaviour
{
    void Start()
    {
        
    }

    void Update()
    {
        
    }
    [ContextMenu("Flor")]
    public void Flor()
    {
        Vector3Int floredPosition = Vector3Int.FloorToInt(transform.position);
        transform.position = floredPosition;
        RandomRotate90();
    }

    public void RandomRotate90()
    {
        // ���������� ��������� ����� 0, 1, 2 ��� 3
        int randomSteps = Random.Range(0, 4);

        // ��������� ���� �������� (0�, 90�, 180� ��� 270�)
        float angle = randomSteps * 90f;

        // ��������� ������� ������������ ������� ����������
        transform.Rotate(Vector3.up, angle, Space.Self);
    }
}
