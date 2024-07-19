using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    // VARIABLES
    [SerializeField] private GameObject winMenu;
    [SerializeField] private GameObject loseMenu;

    private List<Unit> allPlayerUnits = new();
    private List<Unit> allEnemyUnits = new();

    // EXECUTION FUNCTIONS
    void Start()
    {
        var allUnits = FindObjectsOfType<Unit>();
        allPlayerUnits = allUnits.Where(unit => !unit.IsEnemy()).ToList();
        allEnemyUnits = allUnits.Where(unit => unit.IsEnemy()).ToList();

        foreach (var unit in allUnits)
        {
            unit.GetComponent<HealthSystem>().OnDead += HealthSystem_OnDead;
        }
    }

    private void Update()
    {
        if (Keyboard.current.bKey.wasPressedThisFrame)
        {
            Queue<Unit> queue = new(allPlayerUnits);
            while (queue.Count > 0)
            {
                var unit = queue.Dequeue();
                unit.GetComponent<HealthSystem>().Damage(100000);
            }
        }

        if (Keyboard.current.nKey.wasPressedThisFrame)
        {
            Queue<Unit> queue = new(allEnemyUnits);
            while (queue.Count > 0)
            {
                var unit = queue.Dequeue();
                unit.GetComponent<HealthSystem>().Damage(100000);
            }
        }
    }

    // CALLBACKS
    private void HealthSystem_OnDead(object sender, System.EventArgs e)
    {
        Unit unit = ((HealthSystem)sender).GetComponent<Unit>();

        if (unit.IsEnemy())
        {
            allEnemyUnits.Remove(unit);
        }
        else
        {
            allPlayerUnits.Remove(unit);
        }

        if (allEnemyUnits.Count == 0)
        {
            winMenu.SetActive(true);
        }
        else if (allPlayerUnits.Count == 0)
        {
            loseMenu.SetActive(true);
        }
    }
}
