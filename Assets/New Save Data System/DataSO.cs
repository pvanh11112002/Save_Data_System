using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

// ==========================================
// PHẦN 1: RUNTIME CODE (Giữ nguyên khi Build)
// Tuyệt đối KHÔNG được bọc phần này trong #if UNITY_EDITOR
// ==========================================

// --- BASE CLASS ---
public abstract class SaveDataSO<T> : ScriptableObject where T : new()
{
    [Header("File Settings")]
    [SerializeField] protected string fileName;

    [Header("Runtime Data")]
    public T data;

    public void LoadData()
    {
        data = SaveSystem.Load<T>(fileName);
        Debug.Log($"[{name}] Loaded data from {fileName}");
    }

    public void SaveData()
    {
        SaveSystem.Save(fileName, data);
        Debug.Log($"[{name}] Saved data to {fileName}");
    }

    public void DeleteData()
    {
        SaveSystem.Delete(fileName);
        data = new T();
        Debug.Log($"[{name}] Deleted data file {fileName}");
    }
}

// --- CONCRETE CLASSES ---
// Bạn phải khai báo các class con ở đây để tạo file SO

[CreateAssetMenu(fileName = "Player Data SO", menuName = "Save System/Player Data SO")]
public class PlayerDataSO : SaveDataSO<PlayerData> { }

[CreateAssetMenu(fileName = "Game Config SO", menuName = "Save System/Game Config SO")]
public class GameConfigSO : SaveDataSO<GameData> { }


// ==========================================
// PHẦN 2: EDITOR CODE (Chỉ chạy trong Unity Editor)
// Phần này SẼ bị cắt bỏ khi Build game để tối ưu
// ==========================================
#if UNITY_EDITOR

// Tạo một class Editor base để không phải viết lại code vẽ nút bấm nhiều lần
public class BaseSaveDataEditor<T> : Editor where T : new()
{
    public override void OnInspectorGUI()
    {
        // 1. Vẽ giao diện mặc định (Các biến public)
        base.OnInspectorGUI();

        // 2. Lấy đối tượng đang chọn
        SaveDataSO<T> myTarget = (SaveDataSO<T>)target;

        // 3. Vẽ các nút bấm chức năng
        GUILayout.Space(10);
        GUILayout.Label("Developer Controls", EditorStyles.boldLabel);

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Load"))
        {
            myTarget.LoadData();
        }
        if (GUILayout.Button("Save"))
        {
            myTarget.SaveData();
            EditorUtility.SetDirty(target); // Đánh dấu để Unity lưu thay đổi trên file SO
        }
        GUILayout.EndHorizontal();

        GUI.backgroundColor = new Color(1f, 0.4f, 0.4f); // Màu đỏ nhạt
        if (GUILayout.Button("Delete Save File"))
        {
            if (EditorUtility.DisplayDialog("Delete Data",
                $"Bạn có chắc muốn xóa file '{myTarget.name}' không?\nHành động này không thể hoàn tác.",
                "Xóa Ngay", "Hủy"))
            {
                myTarget.DeleteData();
                EditorUtility.SetDirty(target);
            }
        }
        GUI.backgroundColor = Color.white; // Reset màu
    }
}

// --- Đăng ký Editor cho từng loại SO ---
// Vì class Editor Generic không được Unity hỗ trợ trực tiếp attribute [CustomEditor],
// nên ta phải kế thừa thủ công như sau:

[CustomEditor(typeof(PlayerDataSO))]
public class PlayerDataSOEditor : BaseSaveDataEditor<PlayerData> { }

[CustomEditor(typeof(GameConfigSO))]
public class GameConfigSOEditor : BaseSaveDataEditor<GameData> { }

#endif