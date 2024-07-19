using System;
using UnityEngine;

// Class responsible for managing turns in the game
public class TurnSystem : MonoBehaviour
{
    [SerializeField] private Unit[] unitOrder;

    public static TurnSystem Instance { get; private set; }

    public event EventHandler OnTurnChanged;


    private int turnNumber = 0;
    private bool isPlayerTurn = true;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("More than one Turn System" + transform + " - " + Instance);
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        UnitActionSystem.Instance.SetSelectedUnit(unitOrder[turnNumber]);
    }

    public Unit[] GetUnitOrder()
    {
        return unitOrder;
    }

    public void NextTurn()
    {
        turnNumber++;

        if (turnNumber >= unitOrder.Length)
        {
            turnNumber = 0;
        }

        var unit = unitOrder[turnNumber];
        isPlayerTurn = !unit.IsEnemy();

        if (unit.GetComponent<HealthSystem>().IsDead && isPlayerTurn)
        {
            NextTurn();
            return;
        }

        UnitActionSystem.Instance.SetSelectedUnit(unit);

        OnTurnChanged?.Invoke(this, EventArgs.Empty);
    }

    public int GetTurnNumber()
    {
        return turnNumber;
    }

    public bool IsPlayerTurn()
    {
        return isPlayerTurn;
    }

}
