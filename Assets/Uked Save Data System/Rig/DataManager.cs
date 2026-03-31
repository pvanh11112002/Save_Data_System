using UnityEngine;

public class DataManager : MonoBehaviour
{
    public static DataManager Instance;

    [Header("References")]
    [SerializeField] private PlayerDataSO _playerSO;

    private bool _isDirty = false;
    private readonly object _managerLock = new object();

    public void SetDirty() { _isDirty = true; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            
            // [THREAD SAFETY FIX] Cache path on main thread for background saving
            AnhPV.SaveSystem.SaveSystem.Initialize(Application.persistentDataPath);

            LoadAll();
        }
        else Destroy(gameObject);
    }

    private void OnApplicationQuit() => SaveAll();
    
    private void OnApplicationPause(bool pause)
    {
        if (!pause || !_isDirty) return;

#if UNITY_IOS
        // [SAFETY FIX] iOS: Phải lưu đồng bộ để đảm bảo dữ liệu ghi xong trước khi App bị OS suspended.
        try 
        { 
            SaveAll(); 
        }
        catch (System.Exception e) 
        { 
            Debug.LogError($"[DataManager] iOS Save failed: {e.Message}"); 
        }
#else
        // [PERF] Android/Editor: Chạy save trên background thread — không block main thread.
        System.Threading.ThreadPool.QueueUserWorkItem(_ =>
        {
            try { SaveAll(); }
            catch (System.Exception e) { Debug.LogError($"[DataManager] Background save failed: {e.Message}"); }
        });
#endif
    }

    public void LoadAll()
    {
        _playerSO.LoadData();
    }

    public void SaveAll()
    {
        lock (_managerLock)
        {
            if (!_isDirty) return; // [SAVE OPTIMIZATION & THREAD SAFETY]
            
            try
            {
                _playerSO.SaveData();
                _isDirty = false;
            }
            catch (System.Exception e)
            {
                Debug.LogError($"[DataManager] Execute SaveAll Error: {e.Message}");
            }
        }
    }

    // Nút bấm Debug nhanh trên DataManager
    [ContextMenu("Save All Force")]
    public void ForceSave() 
    {
        _isDirty = true;
        SaveAll();   
    }
}