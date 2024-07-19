using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class UnitAnimator : MonoBehaviour
{
    // Animation parameter names
    private const string IS_WALKING = "IsWalking";
    private const string SHOOTING = "Shoot";
    private const string SWORD_SLASH = "SwordSlash";
    private const string SPECIAL = "SpecialTrigger";

    [SerializeField] private Animator animator;
    [SerializeField] private Transform bulletProjectilePrefab;
    [SerializeField] private Transform shootPointTransform;
    [SerializeField] private Transform rifleTransform;
    [SerializeField] private Transform swordTransform;

    private void Awake()
    {
        // Subscribe to events of different action types
        if (TryGetComponent<MoveActions>(out MoveActions moveAction))
        {
            moveAction.OnStartMoving += MoveAction_OnStartMoving;
            moveAction.OnStopMoving += MoveAction_OnStopMoving;
        }
        if(TryGetComponent<ShootAction>(out ShootAction shootAction))
        {
            shootAction.OnShoot += ShootAction_OnShoot;
        }
        if (TryGetComponent<SwordAction>(out SwordAction swordAction))
        {
            swordAction.OnSwordActionStarted += SwordAction_OnSwordActionStarted;
            swordAction.OnSwordActionCompleted += SwordAction_OnSwordActionCompleted;
        }
        if (TryGetComponent(out SpecialAction specialAction))
        {
            specialAction.OnSpecialActionStarted += SpecialAction_OnSpecialActionStarted;
            specialAction.OnSpecialActionCompleted += SpecialAction_OnSpecialActionCompleted;
        }
        if (TryGetComponent(out GrenadeAction grenadeAction))
        {
            grenadeAction.OnSpecialActionStarted += SpecialAction_OnSpecialActionStarted;
            grenadeAction.OnSpecialActionCompleted += SpecialAction_OnSpecialActionCompleted;
        }
    }

    private void SpecialAction_OnSpecialActionCompleted(object sender, EventArgs e)
    {
    }

    private void SpecialAction_OnSpecialActionStarted(object sender, EventArgs e)
    {
        animator.SetTrigger(SPECIAL);
    }

    private void Start()
    {
        EquipRifle();
    }

    private void SwordAction_OnSwordActionCompleted(object sender, EventArgs e)
    {
        EquipRifle();
    }

    private void SwordAction_OnSwordActionStarted(object sender, EventArgs e)
    {
        EquipSword();
        animator.SetTrigger(SWORD_SLASH);
    }

    private void MoveAction_OnStartMoving(object sender, EventArgs e)
    {
        animator.SetBool(IS_WALKING, true);
    } 
    
    private void MoveAction_OnStopMoving(object sender, EventArgs e)
    {
        animator.SetBool(IS_WALKING, false);
    }

    private void ShootAction_OnShoot (object sender, ShootAction.OnShootEventArgs e)
    {
        animator.SetTrigger(SHOOTING);

        Transform bulletProjectileTransform = Instantiate(bulletProjectilePrefab, shootPointTransform.position, Quaternion.identity);
        BulletProjectile bulletProjectile = bulletProjectileTransform.GetComponent<BulletProjectile>();

        Vector3 targetUnitShootAtPosition = e.targetUnit.GetWorldPosition();
        targetUnitShootAtPosition.y = shootPointTransform.position.y;

        bulletProjectile.SetUp(targetUnitShootAtPosition);
    }

    private void EquipSword()
    {
        if (swordTransform != null)
        {
            swordTransform.gameObject.SetActive(true);
        }
    }
    private void EquipRifle()
    {
    }


}
