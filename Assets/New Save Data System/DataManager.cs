using UnityEngine;

public class DataManager : MonoBehaviour
{
    public static DataManager Instance;

    [Header("References")]
    [SerializeField] private PlayerDataSO _playerSO;
    [SerializeField] private GameConfigSO _gameConfigSO;

    // Getter tiện lợi để truy cập data nhanh: DataManager.Instance.Player...
    public PlayerData Player => _playerSO.data;
    public GameData GameConfig => _gameConfigSO.data;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadAll();
        }
        else Destroy(gameObject);
    }

    private void OnApplicationQuit() => SaveAll();
    private void OnApplicationPause(bool pause) { if (pause) SaveAll(); }

    public void LoadAll()
    {
        _playerSO.LoadData();
        _gameConfigSO.LoadData();
    }

    public void SaveAll()
    {
        _playerSO.SaveData();
        _gameConfigSO.SaveData();
    }

    // Nút bấm Debug nhanh trên DataManager
    [ContextMenu("Save All Force")]
    public void ForceSave() => SaveAll();

    public void SetPlayerName(string newName)
    {
        Player.playerName = newName;
    }

    public string GetPlayerName()
    {
        return Player.playerName;
    }    
}