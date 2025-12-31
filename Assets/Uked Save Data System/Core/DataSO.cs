using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace AnhPV.SaveSystem
{
    // ==========================================
    // PHẦN 1: RUNTIME CODE (Giữ nguyên khi Build)
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
        }

        public void SaveData()
        {
            SaveSystem.Save(fileName, data);
        }

        public void DeleteData()
        {
            SaveSystem.Delete(fileName);
            data = new T();
        }
    }


    //[CreateAssetMenu(fileName = "Player Data SO", menuName = "Save System/Player Data SO")]
    //public class PlayerDataSO : SaveDataSO<PlayerData> { }

    //[CreateAssetMenu(fileName = "Game Config SO", menuName = "Save System/Game Config SO")]
    //public class GameConfigSO : SaveDataSO<GameData> { }


#if UNITY_EDITOR
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
                EditorUtility.SetDirty(target);
            }
            GUILayout.EndHorizontal();

            GUI.backgroundColor = new Color(1f, 0.4f, 0.4f);
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
            GUI.backgroundColor = Color.white;
        }
    }
    

#endif
}
