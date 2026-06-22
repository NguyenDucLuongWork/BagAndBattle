using UnityEngine;
using UnityEngine.InputSystem; 

public class CombatTester : MonoBehaviour
{
    [Header("Entities")]
    public PlayerEntity player;
    public MonsterEntity monster;

    [Header("Auto Setup For Combat")]
    [Tooltip("Kéo Inventory vào đây (nếu không kéo, sẽ tự lấy Inventory.Instance)")]
    public Inventory inventory;
    
    [Tooltip("Kéo các Prefab Item vào đây để tự động spawn")]
    public Item[] testItemPrefabs;

    void Update()
    {
        // 0. Nhấn nút T: Tự động spawn Item vào Grid để test
        if (Keyboard.current.tKey.wasPressedThisFrame)
        {
            SpawnTestItems();
        }
   

        // 1. Nhấn nút S: Player đánh Monster
        if (Keyboard.current.sKey.wasPressedThisFrame)
        {
            if (monster != null) monster.TakeDamage(15);
        }

        // 2. Nhấn nút D: Monster đánh Player
        if (Keyboard.current.dKey.wasPressedThisFrame)
        {
            if (player != null) player.TakeDamage(10);
        }

        // 3. Nhấn nút H: Hồi máu cho Player
        if (Keyboard.current.hKey.wasPressedThisFrame)
        {
            if (player != null) player.Heal(20);
        }

        // --- TEST AUTO BATTLE ---
        // Nhấn Enter để bắt đầu Auto Battle
        if (Keyboard.current.enterKey.wasPressedThisFrame)
        {
            Debug.Log("[CombatTester] Phím Enter được nhấn! Đang gọi StartCombat...");
            if (CombatManager.Instance != null)
            {
                CombatManager.Instance.StartCombat();
            }
            else
            {
                Debug.LogError("[CombatTester] Không tìm thấy CombatManager.Instance! Hãy chắc chắn bạn đã tạo GameObject có gắn CombatManager.");
            }
        }

        // Nhấn 1, 2, 3 để đổi tốc độ
        if (Keyboard.current.digit1Key.wasPressedThisFrame) 
        {
            Debug.Log("[CombatTester] Đổi tốc độ x1");
            if (CombatManager.Instance != null) CombatManager.Instance.SetGameSpeed(1f);
        }
        if (Keyboard.current.digit2Key.wasPressedThisFrame) 
        {
            Debug.Log("[CombatTester] Đổi tốc độ x2");
            if (CombatManager.Instance != null) CombatManager.Instance.SetGameSpeed(2f);
        }
        if (Keyboard.current.digit3Key.wasPressedThisFrame) 
        {
            Debug.Log("[CombatTester] Đổi tốc độ x3");
            if (CombatManager.Instance != null) CombatManager.Instance.SetGameSpeed(3f);
        }
    }

    private void SpawnTestItems()
    {
        Inventory inv = inventory != null ? inventory : Object.FindAnyObjectByType<Inventory>();
        if (inv == null)
        {
            Debug.LogError("[CombatTester] Không tìm thấy Inventory nào trong Scene! Để sử dụng phím T (Spawn vào grid), bạn phải tạo một Grid Inventory trước. Hoặc nhấn luôn Enter để chạy combat giả lập không cần Grid.");
            return;
        }

        if (testItemPrefabs == null || testItemPrefabs.Length == 0)
        {
            Debug.LogWarning("[CombatTester] Bạn chưa kéo Prefab Item nào vào ô Test Item Prefabs!");
            return;
        }

        Debug.Log("[CombatTester] Bắt đầu tự động gắn Item vào Inventory...");
        
        // Spawn từng item vào các vị trí trống trên lưới
        int currentX = 0;
        int currentY = 0;

        foreach (var prefab in testItemPrefabs)
        {
            if (prefab == null) continue;

            Item newItem = Instantiate(prefab);
            // Ép item khởi tạo (để load ItemData)
            newItem.Start(); 
            
            Storable storable = newItem.GetComponentInChildren<Storable>();
            if (storable == null)
            {
                Debug.LogWarning($"[CombatTester] Prefab '{prefab.name}' không có Storable!");
                continue;
            }

            // Thử place vào lưới
            bool placed = false;
            for (int y = currentY; y < inv.grid.height; y++)
            {
                for (int x = currentX; x < inv.grid.width; x++)
                {
                    if (inv.TryPlace(storable, new Vector2Int(x, y), 0, out StoredObject obj))
                    {
                        placed = true;
                        currentX = x + 1; // Để lần sau tìm từ ô tiếp theo
                        if (currentX >= inv.grid.width)
                        {
                            currentX = 0;
                            currentY = y + 1;
                        }
                        Debug.Log($"[CombatTester] Đã đặt thành công '{prefab.name}' vào ({x}, {y})!");
                        break;
                    }
                }
                if (placed) break;
            }

            if (!placed)
            {
                Debug.LogWarning($"[CombatTester] Không còn chỗ trống để đặt '{prefab.name}' trên bàn cờ, hoặc footprint không vừa!");
                Destroy(newItem.gameObject);
            }
        }
    }
}