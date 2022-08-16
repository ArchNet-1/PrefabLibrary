using UnityEditor;
using UnityEngine;


/// <summary>
/// Description : Prefab Library Editor
/// @author : Louis PAKEL
/// </summary>
namespace ArchNet.Library.Prefab
{
     [CustomEditor(typeof(PrefabLibrary))]
    public class PrefabLibraryEditor : Editor
    {
        int _listCount;
        int _lastDefaultValue;
        int _indexToDelete = -1;
        string[] _enumValues;
        bool _forceUpdate;

        SerializedProperty _prefabList;
        SerializedProperty _enumPath;
        SerializedProperty _keyType;
        SerializedProperty _expandedSettings;
        SerializedProperty _forceDefaultValue;
        SerializedProperty _defaultValue;

        PrefabLibrary manager;

        GUIStyle _warningInfos;

        private void OnEnable()
        {
            _warningInfos = new GUIStyle();
            _warningInfos.normal.textColor = Color.red;
            _warningInfos.fontStyle = FontStyle.Bold;

            manager = target as PrefabLibrary;
        }

        private void OnDisable()
        {
            manager = null;
            _warningInfos = null;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            _prefabList = serializedObject.FindProperty("_prefabList");
            _enumPath = serializedObject.FindProperty("_enumPath");
            _keyType = serializedObject.FindProperty("_keyType");
            _expandedSettings = serializedObject.FindProperty("_expandedSettings");
            _forceDefaultValue = serializedObject.FindProperty("_forceDefaultValue");
            _defaultValue = serializedObject.FindProperty("_defaultValue");

            // Key Type
            EditorGUILayout.LabelField("Key Type", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(_keyType, new GUIContent(""));

            EditorGUILayout.Space(10);

            if (manager.GetKeyType() > 0 && false == manager.isKeyTypeUpToDate(_keyType.intValue))
            {
                if (EditorUtility.DisplayDialog("Warning", "This will clear set properties?\nDo you want to continue?", "Yes", "No"))
                {
                    ResetEnumPath();
                    ResetPrefabList();
                }
                else
                {
                    _keyType.intValue = manager.GetKeyType();
                }
            }
            _keyType.serializedObject.ApplyModifiedProperties();

            // Enum key type
            if (_keyType.intValue == 1)
            {
                EditorGUILayout.LabelField("Full Namespace Enum Path", EditorStyles.boldLabel);

                if (_enumPath.serializedObject.hasModifiedProperties)
                {
                    _enumPath.serializedObject.ApplyModifiedProperties();
                }
                EditorGUILayout.PropertyField(_enumPath, new GUIContent(""));

                EditorGUILayout.Space(10);
            }

            // Settings
            _expandedSettings.isExpanded = EditorGUILayout.Foldout(_expandedSettings.isExpanded, "Settings");
            if (_expandedSettings.isExpanded)
            {
                // Force Default Value
                _forceDefaultValue.boolValue = EditorGUILayout.ToggleLeft(new GUIContent("Force Default Value"), _forceDefaultValue.boolValue);

                _lastDefaultValue = _defaultValue.intValue;
                if (_forceDefaultValue.boolValue)
                {
                    if (_keyType.intValue == 1)
                    {
                        if (_enumValues != null && _enumValues.Length > 0)
                        {
                            _defaultValue.intValue = EditorGUILayout.Popup(_defaultValue.intValue, _enumValues);
                            if (_defaultValue.serializedObject.hasModifiedProperties)
                            {
                                SetPrefabListDefaultValue();
                                _forceUpdate = true;
                            }
                        }
                        else
                        {
                            EditorGUILayout.LabelField("No enum values found", _warningInfos);
                        }
                    }
                    if (_keyType.intValue == 2)
                    {
                        EditorGUILayout.PropertyField(_defaultValue, new GUIContent(""));
                        if (_defaultValue.serializedObject.hasModifiedProperties)
                        {
                            SetPrefabListDefaultValue();
                            _forceUpdate = true;
                        }
                    }
                }
                else if (_forceDefaultValue.serializedObject.hasModifiedProperties)
                {
                    _defaultValue.intValue = 0;
                    SetPrefabListDefaultValue();
                    _forceUpdate = true;
                }
            }

            EditorGUILayout.Space(10);

            // Add / Remove Buttons
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Add a Prefab"))
            {
                if (IsConditionsOK())
                {
                    manager.AddPrefab();
                }
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space(10);
            
            EditorGUILayout.LabelField("Prefab Library", EditorStyles.boldLabel);

            if (IsConditionsOK())
            {
                HandleDatasDisplay();
                SavePrefabListIfNecessary();
            }
            else
            {
                EditorGUILayout.LabelField("Missing parameters!", _warningInfos);
            }

            serializedObject.ApplyModifiedProperties();
        }

        private void SetPrefabListDefaultValue()
        {
            SerializedProperty prefabData;
            SerializedProperty Key;

            _listCount = _prefabList.arraySize;
            for (int i = 0; i < _listCount; i++)
            {
                prefabData = _prefabList.GetArrayElementAtIndex(i);
                Key = prefabData.FindPropertyRelative("PrefabKey");

                if (Key.intValue == _lastDefaultValue)
                {
                    Key.intValue = _defaultValue.intValue;
                }
            }
        }

        private void HandleDatasDisplay()
        {
            SerializedProperty prefabData;
            SerializedProperty prefabValue;
            SerializedProperty prefabKey;

            _listCount = _prefabList.arraySize;
            for (int i = 0; i < _listCount; i++)
            {
                prefabData = _prefabList.GetArrayElementAtIndex(i);

                prefabValue = prefabData.FindPropertyRelative("PrefabValue");
                prefabKey = prefabData.FindPropertyRelative("PrefabKey");

                prefabData.isExpanded = EditorGUILayout.Foldout(prefabData.isExpanded, new GUIContent("Prefab " + i));
                if (prefabData.isExpanded)
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.PropertyField(prefabValue, new GUIContent(""));
                    switch (_keyType.intValue)
                    {
                        // NONE
                        case 0:
                            break;

                        // ENUM
                        case 1:
                            prefabKey.intValue = EditorGUILayout.Popup(prefabKey.intValue, _enumValues, GUILayout.MaxWidth(200));
                            break;

                        // INT
                        case 2:
                            EditorGUILayout.LabelField("=>", GUILayout.MaxWidth(20));
                            EditorGUILayout.PropertyField(prefabKey, new GUIContent(""), GUILayout.MaxWidth(200));
                            break;
                    }
                    if (GUILayout.Button("Delete"))
                    {
                        if (EditorUtility.DisplayDialog("Warning", "Are you sure to delete this Prefab from the library?", "Yes", "No"))
                        {
                            prefabData.isExpanded = false;
                            _prefabList.DeleteArrayElementAtIndex(i);
                            break;
                        }
                    }
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.Space(5);
                }
            }
        }

        private void ResetEnumPath()
        {
            _enumPath.stringValue = "";
            _enumPath.serializedObject.ApplyModifiedProperties();
        }

        private void ResetPrefabList()
        {
            SerializedProperty prefabData;
            _listCount = _prefabList.arraySize;
            for (int i = 0; i < _listCount; i++)
            {
                prefabData = _prefabList.GetArrayElementAtIndex(i);
                prefabData.isExpanded = false;
            }
            _prefabList.ClearArray();
            _enumPath.serializedObject.ApplyModifiedProperties();
        }

        private bool IsConditionsOK()
        {
            if (_keyType.intValue == 0)
            {
                return false;
            }

            if (_keyType.intValue == 1)
            {
                if (string.IsNullOrEmpty(_enumPath.stringValue))
                {
                    return false;
                }

                _enumValues = manager.GetEnumValues(_enumPath.stringValue);
                if (_enumValues == null)
                {
                    return false;
                }
            }

            return true;
        }

        private void SavePrefabListIfNecessary()
        {
            if ((_prefabList.serializedObject.hasModifiedProperties || _forceUpdate) && _prefabList.arraySize > 0)
            {
                manager.SaveDictionnary();
                _forceUpdate = false;
            }
            _prefabList.serializedObject.ApplyModifiedProperties();
        }
    }
}
