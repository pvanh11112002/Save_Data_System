using UnityEngine;

public class DataManager : MonoBehaviour
{
    public static DataManager Instance;

    [Header("References")]
    [SerializeField] private PlayerDataSO _playerSO;

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
    }

    public void SaveAll()
    {
        _playerSO.SaveData();
    }

    // Nút bấm Debug nhanh trên DataManager
    [ContextMenu("Save All Force")]
    public void ForceSave() => SaveAll();   
}