using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Class representing a sword action, derived from BaseAction
public class SpecialAction : BaseAction
{
    [SerializeField] private int damage = 30;
    [SerializeField] private float hitTime = 0.5f;
    public static event EventHandler OnAnySpecialHit;

    public event EventHandler OnSpecialActionStarted;
    public event EventHandler OnSpecialActionCompleted;

    private enum State
    {
        DoingSpecialBeforeHit,
        DoingSpecialAfterHit,
    }

    [SerializeField]
    private AudioClip specialSound;

    private int maxSpecialDistance = 1;
    private State state;
    private float stateTimer;
    private Unit targetUnit;

    private void Update()
    {
        if (!isActive)
        {
            return;
        }

        stateTimer -= Time.deltaTime;

        switch (state)
        {
            case State.DoingSpecialBeforeHit:
                Vector3 aimDir = (targetUnit.GetWorldPosition() - unit.GetWorldPosition()).normalized;

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

                // Play sword sound, damage the target unit, and trigger events
                unit.SetAudioClip(specialSound);
                unit.PlayAudioClip();
                targetUnit.Damage(damage);
                OnAnySpecialHit?.Invoke(this, EventArgs.Empty);
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
            actionValue = 200,
        };
    }

    public override List<GridPosition> GetValidActionGridPositionList()
    {
        List<GridPosition> validGridPositionList = new List<GridPosition>();

        GridPosition unitGridPosition = unit.GetGridPosition();

        // Iterate through neighboring grid positions
        for (int x = -maxSpecialDistance; x <= maxSpecialDistance; x++)
        {
            for (int z = -maxSpecialDistance; z <= maxSpecialDistance; z++)
            {
                GridPosition offsetGridPosition = new GridPosition(x, z);
                GridPosition testGridPosition = unitGridPosition + offsetGridPosition;

                if (!LevelGrid.Instance.IsValidGridPosition(testGridPosition))
                {
                    continue;
                }

                if (!LevelGrid.Instance.HasAnyUnitOnGridPosition(testGridPosition))
                {
                    // Grid Position is empty, no Unit
                    continue;
                }

                Unit targetUnit = LevelGrid.Instance.GetUnitAtGridPosition(testGridPosition);

                if (targetUnit.IsEnemy() == unit.IsEnemy())
                {
                    // Both Units on same 'team'
                    continue;
                }

                validGridPositionList.Add(testGridPosition);
            }
        }

        return validGridPositionList;
    }

    public override void TakeAction(GridPosition gridPosition, Action onActionComplete)
    {
        targetUnit = LevelGrid.Instance.GetUnitAtGridPosition(gridPosition);

        state = State.DoingSpecialBeforeHit;
        float beforeHitStateTime = hitTime;
        stateTimer = beforeHitStateTime;

        OnSpecialActionStarted?.Invoke(this, EventArgs.Empty);
        ActionStart(onActionComplete);
    }

    public int GetMaxSpecialDistance()
    {
        return maxSpecialDistance;
    }

}
