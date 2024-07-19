using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class UnitQueueDisplay : MonoBehaviour
{
    [System.Serializable]
    public class UnitTypePortraitPair
    {
        public string unitTypeName;
        public GameObject portraitObject;
    }

    [SerializeField] private List<RectTransform> queueSlots;
    [SerializeField] private List<UnitTypePortraitPair> unitTypePortraitPairs;
    [SerializeField] private GameObject defaultPortraitObject;

    private Dictionary<string, GameObject> portraitObjectPool = new Dictionary<string, GameObject>();

    private void Start()
    {
        TurnSystem.Instance.OnTurnChanged += UpdateUnitQueue;
        InitializePortraitPool();
        UpdateUnitQueue(null, System.EventArgs.Empty);
    }

    private void OnDestroy()
    {
        if (TurnSystem.Instance != null)
        {
            TurnSystem.Instance.OnTurnChanged -= UpdateUnitQueue;
        }
    }

    private void InitializePortraitPool()
    {
        foreach (var pair in unitTypePortraitPairs)
        {
            portraitObjectPool[pair.unitTypeName] = Instantiate(pair.portraitObject, transform);
            portraitObjectPool[pair.unitTypeName].SetActive(false);
        }
        portraitObjectPool["default"] = Instantiate(defaultPortraitObject, transform);
        portraitObjectPool["default"].SetActive(false);
    }

    private void UpdateUnitQueue(object sender, System.EventArgs e)
    {
        Unit[] allUnits = TurnSystem.Instance.GetUnitOrder();
        int currentTurnNumber = TurnSystem.Instance.GetTurnNumber();

        List<Unit> activeUnits = new List<Unit>();

        // Include all units, including multiple enemies
        for (int i = 0; i < allUnits.Length; i++)
        {
            int index = (currentTurnNumber + i) % allUnits.Length;
            if (!allUnits[index].GetComponent<HealthSystem>().IsDead)
            {
                activeUnits.Add(allUnits[index]);
                if (activeUnits.Count >= queueSlots.Count)
                    break;
            }
        }

        // Clear existing portraits
        foreach (var slot in queueSlots)
        {
            foreach (Transform child in slot)
            {
                Destroy(child.gameObject);
            }
        }

        // Update portraits
        for (int i = 0; i < queueSlots.Count; i++)
        {
            if (i < activeUnits.Count)
            {
                Unit unit = activeUnits[i];
                GameObject portraitObject = GetPortraitObjectForUnit(unit);
                if (portraitObject != null)
                {
                    GameObject portraitInstance = Instantiate(portraitObject, queueSlots[i]);
                    portraitInstance.SetActive(true);
                    RectTransform portraitRect = portraitInstance.GetComponent<RectTransform>();
                    portraitRect.anchoredPosition = Vector2.zero;
                }
            }
        }
    }

    private GameObject GetPortraitObjectForUnit(Unit unit)
    {
        string unitTypeName = DetermineUnitTypeName(unit);
        if (portraitObjectPool.TryGetValue(unitTypeName, out GameObject portraitObject))
        {
            return portraitObject;
        }
        return portraitObjectPool["default"];
    }

    private string DetermineUnitTypeName(Unit unit)
    {
        if (unit.IsEnemy())
        {
            return "Enemy";
        }
        else
        {
            // For player units, you might need to use other existing properties
            // This is just an example and may need adjustment based on your Unit class
            string unitName = unit.gameObject.name.ToLower();
            if (unitName.Contains("ninja")) return "Ninja";
            if (unitName.Contains("mage")) return "Mage";
            if (unitName.Contains("warrior")) return "Warrior";
            if (unitName.Contains("monk")) return "Monk";
            // Add more conditions as needed
        }
        return "default";
    }
}