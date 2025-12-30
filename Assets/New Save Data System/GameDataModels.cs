using System;
using System.Collections.Generic;
using System.Numerics;

[Serializable]
public class PlayerData
{
    public string playerName = "Newbie";
    public int coins = 0;
    public int level = 1;
    public List<InventoryItem> inventory = new List<InventoryItem>();
    public PlayerPos lastPosition = new PlayerPos();
    // Thêm các biến khác ở đây
}

[Serializable]
public class GameData
{
    public bool isMusicOn = true;
    public float masterVolume = 1.0f;
    // Dữ liệu cài đặt game, tiến độ ải, v.v.
}

[Serializable] 
public class InventoryItem
{
    public string itemId;
    public int quantity;
}

[Serializable]
public class PlayerPos
{
    public Vector3 position;
}