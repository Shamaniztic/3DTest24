using System;
using UnityEngine;

public class HealthSystem : MonoBehaviour
{
    public event EventHandler OnDead;
    public event EventHandler OnDamaged;

    [SerializeField]
    private int health = 100;

    private int healthMax;

    public float HealthPercentage => health / healthMax;
    public bool IsDead => health <= 0f;

    private void Awake()
    {
        healthMax = health;
    }

    public void Damage(int damageAmount)
    {
        if (IsDead)
        {
            return;
        }

        health -= damageAmount;

        if(health < 0)
        {
            health = 0;
        }
        OnDamaged?.Invoke(this, EventArgs.Empty);

        if(health ==  0) 
        {
            Die();
        }
        else
        {
            GetComponentInChildren<Animator>().CrossFadeInFixedTime("Hit", 0.1f);
        }

        //Debug.Log(health);

    }

    private void Die()
    {
        GetComponentInChildren<Animator>().CrossFadeInFixedTime("Death", 0.1f);
        OnDead?.Invoke(this, EventArgs.Empty);
    }

    // Method to get the normalized health value (between 0 and 1)
    public float GetHealthNormalized()
    {
        return (float)health / healthMax;
    }

}
