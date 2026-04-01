using AnhPV.SaveSystem;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;


[CreateAssetMenu(fileName ="Player Data SO", menuName = "Game/Game Data")]
public class PlayerDataSO : SaveDataSO<PlayerData> { }

#if UNITY_EDITOR
[CustomEditor(typeof(PlayerDataSO))]
public class PlayerDataEditor : BaseSaveDataEditor<PlayerData> { }
#endif
