using System;
using System.Collections.Generic;
using System.Text;
public enum GameState { Preparation, Combat, Reward, GameOver }

public class CombatItemTracker
{
    public StoredObject storedObject;
    public ItemData itemData;
    public float currentTimer;

    public CombatItemTracker(StoredObject obj, ItemData data)
    {
        storedObject = obj;
        itemData = data;
        currentTimer = 0f;
    }
}