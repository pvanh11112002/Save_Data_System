using AnhPV.SaveSystem;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/Player Data")]
public class PlayerDataSO : SaveDataSO<PlayerData> { }

#if UNITY_EDITOR
[CustomEditor(typeof(PlayerDataSO))]
public class PlayerDataEditor : BaseSaveDataEditor<PlayerData> { }
#endif
