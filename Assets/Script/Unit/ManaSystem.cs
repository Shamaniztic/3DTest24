using UnityEngine;

public class ManaSystem : MonoBehaviour
{
    // VARIABLES
    [SerializeField] private int mana;

    private int currentMana;

    public float CurrentManaPercentage => (float)currentMana / (float)mana;

    // EXECUTION FUNCTIONS
    private void Awake()
    {
        currentMana = mana;
    }

    // METHODS
    public void LoseMana(int amount)
    {
        currentMana -= amount;
        currentMana = Mathf.Clamp(currentMana, 0, mana);
    }

    public bool HasMana(int amount) => currentMana >= amount;
}
