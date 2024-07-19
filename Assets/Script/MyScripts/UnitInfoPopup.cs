using UnityEngine;
using TMPro;

public class UnitInfoPopup : MonoBehaviour
{
    [SerializeField] private GameObject popupPanel;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private TextMeshProUGUI jobText;

    private TurnSystem turnSystem;

    private void Start()
    {
        popupPanel.SetActive(false);
        turnSystem = TurnSystem.Instance;
        if (turnSystem == null)
        {
            Debug.LogError("TurnSystem instance not found!");
            return;
        }
        turnSystem.OnTurnChanged += UpdateUnitInfo;
        UpdateUnitInfo(null, System.EventArgs.Empty);
    }

    private void OnDestroy()
    {
        if (turnSystem != null)
        {
            turnSystem.OnTurnChanged -= UpdateUnitInfo;
        }
    }

    private void UpdateUnitInfo(object sender, System.EventArgs e)
    {
        if (turnSystem == null)
        {
            Debug.LogError("TurnSystem is null in UpdateUnitInfo!");
            return;
        }

        Unit[] allUnits = turnSystem.GetUnitOrder();
        if (allUnits == null || allUnits.Length == 0)
        {
            Debug.LogError("No units found in TurnSystem!");
            return;
        }

        int currentTurnNumber = turnSystem.GetTurnNumber();
        if (currentTurnNumber < 0 || currentTurnNumber >= allUnits.Length)
        {
            Debug.LogError($"Invalid turn number: {currentTurnNumber}. Total units: {allUnits.Length}");
            return;
        }

        Unit activeUnit = allUnits[currentTurnNumber];
        if (activeUnit == null)
        {
            Debug.LogError("Active unit is null!");
            return;
        }

        HealthSystem healthSystem = activeUnit.GetComponent<HealthSystem>();
        if (healthSystem == null)
        {
            Debug.LogError("HealthSystem not found on active unit!");
            return;
        }

        if (!healthSystem.IsDead)
        {
            ShowInfo(activeUnit);
        }
        else
        {
            HideInfo();
        }
    }

    private void ShowInfo(Unit unit)
    {
        UnitData unitData = unit.GetComponent<UnitData>();
        if (unitData != null)
        {
            Debug.Log($"Displaying info for unit: {unitData.unitName}, Level: {unitData.unitLevel}, Job: {unitData.unitJob}");
            nameText.text = unitData.unitName;
            levelText.text = unitData.unitLevel.ToString();
            jobText.text = unitData.unitJob;
            popupPanel.SetActive(true);
        }
        else
        {
            Debug.LogWarning("UnitData component not found on the active unit: " + unit.name);
            HideInfo();
        }
    }

    private void HideInfo()
    {
        popupPanel.SetActive(false);
    }
}