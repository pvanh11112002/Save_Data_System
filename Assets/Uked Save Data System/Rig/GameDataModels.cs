using System;

[Serializable]
public class PlayerData
{
    public string playerName = "Newbie";
    public int coins = 0;
    public int level = 1;
    // Thêm các biến/ phương thức khác ở đây
}

[Serializable]
public class GameData
{
    public bool isMusicOn = true;
    public float masterVolume = 1.0f;
    // Thêm các biến/ phương thức khác ở đây
}