using UnityEngine;
using System.Collections.Generic;

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

public class CombatManager : MonoBehaviour
{
    public static CombatManager Instance { get; private set; }

    [Header("Entities")]
    public PlayerEntity player;
    public MonsterEntity monster;

    [Header("Settings")]
    public float gameSpeedMultiplier = 1f;

    public GameState CurrentState { get; private set; } = GameState.Preparation;

    private List<CombatItemTracker> activeItems = new List<CombatItemTracker>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void StartCombat()
    {
        if (CurrentState != GameState.Preparation) return;

        CurrentState = GameState.Combat;

        if (Inventory.Instance != null)
        {
            Inventory.Instance.IsLocked = true;
            InitializeCombatItems(Inventory.Instance.grid.stored);
        }

        Debug.Log("Combat Started! Inventory Locked.");
    }

    private void InitializeCombatItems(List<StoredObject> inventoryItems)
    {
        activeItems.Clear();
        foreach (var item in inventoryItems)
        {
            if (item.storable != null)
            {
                Item itemComponent = item.storable.GetComponentInParent<Item>();
                if (itemComponent != null && itemComponent.Data != null)
                {
                    activeItems.Add(new CombatItemTracker(item, itemComponent.Data));
                }
            }
        }
        
        if (activeItems.Count == 0)
        {
            Debug.LogWarning("[CombatManager] Không có item nào trên bàn cờ hoặc các item không có component 'Item'! Trận đấu sẽ không có tác dụng gì.");
        }
        else
        {
            Debug.Log($"[CombatManager] Đã nhận {activeItems.Count} items trên bàn cờ. Bắt đầu vòng lặp combat!");
        }
    }

    private void Update()
    {
        if (CurrentState != GameState.Combat) return;

        // Check Win/Loss conditions
        if (monster != null && monster.IsDead)
        {
            EndCombat(GameState.Reward);
            return;
        }

        if (player != null && player.IsDead)
        {
            EndCombat(GameState.GameOver);
            return;
        }

        // Execution loop for items
        float deltaTime = Time.deltaTime * gameSpeedMultiplier;

        foreach (var tracker in activeItems)
        {
            // Default trigger speed to 1f if it's not set properly
            float speed = tracker.itemData.triggerSpeed > 0f ? tracker.itemData.triggerSpeed : 1f;

            tracker.currentTimer += deltaTime;

            if (tracker.currentTimer >= speed)
            {
                tracker.currentTimer -= speed;
                ProcessItemEffects(tracker.itemData, tracker.storedObject.storable.gameObject.name);
            }
        }
    }

    private void ProcessItemEffects(ItemData data, string itemName)
    {
        int finalDamage = data.damage;

        // Nếu người dùng quên chưa chỉnh chỉ số trong Scriptable Object, ta sẽ gán tạm 5 sát thương để dễ test
        if (finalDamage == 0 && data.heal == 0 && data.shield == 0)
        {
            Debug.LogWarning($"[CombatManager] Vật phẩm '{itemName}' đang có chỉ số là 0! Tạm gán 5 Damage để test.");
            finalDamage = 5;
        }

        Debug.Log($"[CombatManager] >>> Vật phẩm '{itemName}' vừa kích hoạt! (Dame: {finalDamage}, Heal: {data.heal}, Shield: {data.shield})");

        if (monster != null && finalDamage > 0)
        {
            monster.TakeDamage(finalDamage);
        }

        if (player != null)
        {
            if (data.heal > 0)
                player.Heal(data.heal);
            
            if (data.shield > 0)
                player.AddShield(data.shield);
        }
    }

    public void SetGameSpeed(float multiplier)
    {
        gameSpeedMultiplier = multiplier;
        Debug.Log($"[CombatManager] Game speed set to {multiplier}x");
    }

    private void EndCombat(GameState resultState)
    {
        CurrentState = resultState;
        
        if (Inventory.Instance != null)
        {
            Inventory.Instance.IsLocked = false;
        }

        if (resultState == GameState.Reward)
        {
            Debug.Log("[CombatManager] Monster Defeated! Transitioning to Reward Phase...");
            // TODO: Trigger stage rewards phase
        }
        else if (resultState == GameState.GameOver)
        {
            Debug.Log("[CombatManager] Player Died! Permadeath triggered. Returning to Main Menu...");
            // TODO: Wipe run data, process metagame currency, return to menu
        }
    }
}
