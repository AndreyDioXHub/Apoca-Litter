using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MainMenuMouseReleaser : MonoBehaviour
{
    private int MIN_NUMBER = 1;
    private int MAX_NUMBER = 45;
    private int NUMBERS_TO_SELECT = 6;
    [SerializeField]
    private List<int> _archiveNumbers = new List<int>();

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnEnable()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    [ContextMenu("RandomNumbers")]
    public void RandomNumbers()
    {
        List<int> numbers = new List<int>();

        for (int j = 0; j < 6; j++)
        {
            int random = -1;

            random = UnityEngine.Random.Range(1, 45);

            bool notMayOut = true;

            while (notMayOut)
            {
                random = UnityEngine.Random.Range(1, 45);
                notMayOut = _archiveNumbers.Contains(random);
            }

            _archiveNumbers.Add(random);
            numbers.Add(random);
        }

        string result = "";

        foreach (int number in numbers)
        {
            result = $"{result} {number}";
        }

        Debug.Log(result);
    }
}
