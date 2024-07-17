using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

// Class representing a grenade action, derived from BaseAction
public class GrenadeAction : BaseAction
{
    [SerializeField] private float grenadeDelay = 0.5f;
    [SerializeField]
    private Transform grenadeProjectilePrefab;
    [SerializeField]
    private AudioClip grenadeSound;

    private int maxThrowDistance = 7;

    public event EventHandler OnSpecialActionStarted;
    public event EventHandler OnSpecialActionCompleted;

    private GridPosition targetPosition;

    private enum State
    {
        DoingSpecialBeforeHit,
        DoingSpecialAfterHit,
    }

    private State state;
    private float stateTimer;

    private void Update()
    {
        if(!isActive)
        {
            return;
        }

        stateTimer -= Time.deltaTime;

        switch (state)
        {
            case State.DoingSpecialBeforeHit:
                Vector3 pos = new(targetPosition.x, 0f, targetPosition.z);
                Vector3 aimDir = (pos - unit.GetWorldPosition()).normalized;

                float rotateSpeed = 10f;
                transform.forward = Vector3.Lerp(transform.forward, aimDir, Time.deltaTime * rotateSpeed);
                break;
            case State.DoingSpecialAfterHit:
                break;
        }

        if (stateTimer <= 0f)
        {
            NextState();
        }
    }

    private void NextState()
    {
        switch (state)
        {
            case State.DoingSpecialBeforeHit:
                state = State.DoingSpecialAfterHit;
                float afterHitStateTime = 0.5f;
                stateTimer = afterHitStateTime;

                // Instantiate the grenade projectile and set up its behavior
                Transform grenadeProjectileTransform = Instantiate(grenadeProjectilePrefab, unit.GetWorldPosition(), Quaternion.identity);
                GrenadeProjectile grenadeProjectile = grenadeProjectileTransform.GetComponent<GrenadeProjectile>();
                grenadeProjectile.Setup(targetPosition, OnGrenadeBehaviourComplete);

                break;
            case State.DoingSpecialAfterHit:
                OnSpecialActionCompleted?.Invoke(this, EventArgs.Empty);
                ActionComplete();
                break;
        }
    }

    public override EnemyAIAction GetEnemyAIAction(GridPosition gridPosition)
    {
        return new EnemyAIAction
        {
            gridPosition = gridPosition,
            actionValue = 0,
        };
    }
    // Method to get a list of valid target grid positions for the action
    public override List<GridPosition> GetValidActionGridPositionList()
    {
        List<GridPosition> validGridPositionList = new List<GridPosition>();
        GridPosition unitGridPosition = unit.GetGridPosition();

        for (int x = -maxThrowDistance; x <= maxThrowDistance; x++)
        {
            for (int z = -maxThrowDistance; z <= maxThrowDistance; z++)
            {
                GridPosition offsetGridPosition = new GridPosition(x, z);
                GridPosition testGridPosition = unitGridPosition + offsetGridPosition;

                if (!LevelGrid.Instance.IsValidGridPosition(testGridPosition))
                {
                    continue;
                }

                int testDistance = Mathf.Abs(x) + Mathf.Abs(z);
                if (testDistance > maxThrowDistance)
                {
                    continue;
                }

                validGridPositionList.Add(testGridPosition);
            }
        }


        return validGridPositionList;
    }

    public override void TakeAction(GridPosition gridPosition, Action onActionComplete)
    {
        targetPosition = gridPosition;

        state = State.DoingSpecialBeforeHit;
        float beforeHitStateTime = grenadeDelay;
        stateTimer = beforeHitStateTime;

        OnSpecialActionStarted?.Invoke(this, EventArgs.Empty);
        
        ActionStart(onActionComplete);
    }

    // Method called when the grenade projectile behavior is complete
    private void OnGrenadeBehaviourComplete()
    {
        ActionComplete();
        unit.SetAudioClip(grenadeSound);
        unit.PlayAudioClip();
    }
}
