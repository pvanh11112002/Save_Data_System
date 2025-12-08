using System;
using System.Collections.Generic;

[Serializable]
public class PlayerData
{
    public string playerName = "Newbie";
    public int coins = 0;
    public int level = 1;
    // Thêm các biến khác ở đây
}

[Serializable]
public class GameData
{
    public bool isMusicOn = true;
    public float masterVolume = 1.0f;
    // Dữ liệu cài đặt game, tiến độ ải, v.v.
}
