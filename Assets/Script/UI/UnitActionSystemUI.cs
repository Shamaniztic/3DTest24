using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Class responsible for managing the UI related to unit actions
public class UnitActionSystemUI : MonoBehaviour
{
    [SerializeField]
    private Transform actionButtonPrefab;
    [SerializeField]
    private Transform actionButtonContainerTransform;
    [SerializeField]
    private Transform attackContainerTransform;
    [SerializeField]
    private Transform skillContainerTransform;
    [SerializeField]
    private TextMeshProUGUI actionPointsText;

    private List<ActionButtonUI> actionButtonUIList = new List<ActionButtonUI>();

    private void Awake()
    {
        actionButtonUIList = new List<ActionButtonUI>();
    }

    private void Start()
    {
        UnitActionSystem.Instance.OnSelectedUnitChanged += UnitActionSystem_OnSelectedUnitChanged;
        UnitActionSystem.Instance.OnSelectedActionChanged += UnitActionSystem_OnSelectedActionChanged;
        UnitActionSystem.Instance.OnActionStarted += UnitActionSystem_OnActionStarted;

        TurnSystem.Instance.OnTurnChanged += TurnSystem_OnTurnChanged;

        Unit.OnAnyActionPointsChanged += Unit_OnAnyActionPointsChanged;


        UpdateActionPoints();
        CreateUnitActionButtons();
        UpdateSelectedVisual();
    }

    // Method to create action buttons for the selected unit
    private void CreateUnitActionButtons()
    {
        //Destroy all previous button game objects
        foreach (Transform buttonTransform in actionButtonContainerTransform)
        {
            if (buttonTransform.gameObject.name.Contains("Attack") || buttonTransform.gameObject.name.Contains("Skill"))
            {
                continue;
            }

            Destroy(buttonTransform.gameObject);
        }

        foreach (Transform buttonTransform in attackContainerTransform)
        {
            Destroy(buttonTransform.gameObject);
        }

        foreach (Transform buttonTransform in skillContainerTransform)
        {
            Destroy(buttonTransform.gameObject);
        }

        actionButtonUIList.Clear();

        Unit selectedUnit = UnitActionSystem.Instance.GetSelectedUnit();

        if (selectedUnit.IsEnemy())
        {
            return;
        }

        // Create action buttons for each base action of the selected unit
        foreach (BaseAction baseAction in selectedUnit.GetBaseActionArray())
        {
            Transform targetContainer = actionButtonContainerTransform;

            if (baseAction.IsAttack)
            {
                targetContainer = attackContainerTransform;
            }
            else if (baseAction.IsSkill)
            {
                targetContainer = skillContainerTransform;
            }

            Transform actionButtonTransform = Instantiate(actionButtonPrefab, targetContainer);
            ActionButtonUI actionButtonUI = actionButtonTransform.GetComponent<ActionButtonUI>();
            actionButtonUI.SetBaseAction(baseAction);

            if (!baseAction.IsAttack && !baseAction.IsSkill)
            {
                actionButtonUI.GetComponent<Button>().onClick.AddListener(() => 
                {
                    attackContainerTransform.gameObject.SetActive(false);
                    skillContainerTransform.gameObject.SetActive(false);
                });
            }

            actionButtonUIList.Add(actionButtonUI);                             
        }
    }

    private void UnitActionSystem_OnSelectedUnitChanged(object sender, EventArgs e)
    {
        CreateUnitActionButtons();
        UpdateSelectedVisual();
        UpdateActionPoints();
    }

    private void UnitActionSystem_OnSelectedActionChanged(object sender, EventArgs e)
    {
        UpdateSelectedVisual();
    }

    private void UnitActionSystem_OnActionStarted(object sender, EventArgs e)
    {
        UpdateActionPoints();
    }

    // Method to update the visual state of the action buttons based on the selected action
    private void UpdateSelectedVisual()
    {
        foreach(ActionButtonUI actionButtonUI in actionButtonUIList)
        {
            actionButtonUI.UpdateSelectedVisual();
        }
    }

    private void UpdateActionPoints()
    {
        // Get the selected unit and update the action points text
        Unit selectedUnit = UnitActionSystem.Instance.GetSelectedUnit();
        actionPointsText.text = "Action Points: " + selectedUnit.GetActionPoints();
    }

    private void TurnSystem_OnTurnChanged(object sender, EventArgs e)
    {
        UpdateActionPoints();
    }

    private void Unit_OnAnyActionPointsChanged(object sender, EventArgs e)
    {
        UpdateActionPoints();
    }

}
