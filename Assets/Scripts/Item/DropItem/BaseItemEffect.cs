using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseItemEffect
{
    [SerializeField] private int effectAmount;

    public void UseAction()
    {
        Effect();
        SE();
    }

    protected abstract void SE();
    protected abstract void Effect();

    protected int GetEffectAmount()
    {
        return effectAmount;
    }
}
