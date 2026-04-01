using AnhPV.SaveSystem;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;


[CreateAssetMenu(fileName ="Game Data SO", menuName = "Game/Game Data")]
public class GameDataSO : SaveDataSO<GameData> { }

#if UNITY_EDITOR
[CustomEditor(typeof(GameDataSO))]
public class GameDataEditor : BaseSaveDataEditor<GameData> { }
#endif
