using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Description : Prefab Library is a link between prefab and enum or integer
/// @author : Louis PAKEL
/// </summary>
namespace ArchNet.Library.Prefab
{
    [Serializable]
    public class PrefabData
    {
        public int PrefabKey;
        public GameObject PrefabValue;
        
        public PrefabData(int prefabKey, GameObject prefabValue)
        {
            PrefabKey = prefabKey;
            PrefabValue = prefabValue;
        }
    }

    [CreateAssetMenu(fileName = "PrefabLibrary", menuName = "ArchNet/PrefabLibrary")]
    public class PrefabLibrary : ScriptableObject
    {
          public enum KeyType
        {
            NONE,
            ENUM,
            INT
        }

        #region SerializeField

        [SerializeField] private string _enumPath;
        [SerializeField] private List<PrefabData> _prefabList;
        [SerializeField] bool _expandedSettings = true;
        [SerializeField] bool _forceDefaultValue;
        [SerializeField] private int _defaultValue;
        [SerializeField] KeyType _keyType = KeyType.NONE;

        #endregion

        #region Private Properties
        
        private Dictionary<int, GameObject> _prefabDict;
        private string[] _enumValues;
        private Type _enumType;

        #endregion

        #region Public Methods

        public GameObject GetPrefab(int pKeyValue)
        {
            if (CheckExistingMaterial(pKeyValue))
            {
                return _prefabDict[pKeyValue];
            }

            //TODO: RETURN ERROR
            return null;
        }

        public GameObject GetPrefab(Enum pEnumValue)
        {
            Type actualEnumType = GetEnumType(_enumPath);

            if (_keyType != KeyType.ENUM)
            {
                Debug.LogWarning("[" + name + "] It seems that you're trying to get a Prefab with an enum within a library not set for enum key");
            }
            else if (pEnumValue.GetType() != actualEnumType)
            {
                Debug.LogWarning("[" + name + "] It seems that you're trying to get a Prefab with a different enum that the one defined to be the key in the library");
            }

            int value = Convert.ToInt32(pEnumValue);
            if (CheckExistingMaterial(value))
            {
                return _prefabDict[value];
            }

            // TODO: RETURN ERROR
            return null;
        }

        #endregion

        #region Private Methods

        private bool CheckExistingMaterial(int enumValue)
        {
            if (_prefabDict == null)
            {
                if (_prefabList != null)
                {
                    SaveDictionnary();
                }

                if (_prefabList.Count == 0)
                {
                    Debug.LogWarning("Prefab Library \'" + name + "\' doesn't contain anything");
                    return false;
                }
            }
            if (false == _prefabDict.ContainsKey(enumValue))
            {
                Debug.LogWarning("Library do not contain a Prefab for value: " + enumValue);
                return false;
            }

            if (_prefabDict.ContainsKey(enumValue))
            {
                return true;
            }

            return false;
        }

        #endregion

        #region Editor Methods

        /// <summary>
        /// FOR CUSTOM EDITOR PURPOSE ONLY! DO NOT USE
        /// </summary>
        public string[] GetEnumValues(string enumName)
        {
            Type type = GetEnumType(enumName);
            if (type != null)
            {
                _enumType = type;
                _enumPath = enumName;
                _enumValues = Enum.GetNames(type);
                return _enumValues;
            }

            return null;
        }

        /// <summary>
        /// FOR CUSTOM EDITOR PURPOSE ONLY! DO NOT USE
        /// </summary>
        public Type GetEnumType(string enumName)
        {
            if (false == string.IsNullOrEmpty(enumName))
            {
                foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
                {
                    var type = assembly.GetType(enumName);
                    if (type == null)
                        continue;
                    if (type.IsEnum)
                        return type;
                }
            }
            return null;
        }

        /// <summary>
        /// FOR CUSTOM EDITOR PURPOSE ONLY! DO NOT USE
        /// </summary>
        public int GetKeyType()
        {
            return (int)_keyType;
        }

        /// <summary>
        /// FOR CUSTOM EDITOR PURPOSE ONLY! DO NOT USE
        /// </summary>
        public bool isKeyTypeUpToDate(int pKeyValue)
        {
            if ((int)_keyType == pKeyValue)
            { return true; }

            return false;
        }

        /// <summary>
        /// FOR CUSTOM EDITOR PURPOSE ONLY! DO NOT USE
        /// </summary>
        public void SaveDictionnary()
        {
            if (_prefabList == null)
            { return; }

            if (_prefabList.Count == 0)
            {
                Debug.LogWarning("The Prefab List is empty");
            }
            _prefabDict = new Dictionary<int, GameObject>();
            for (int i = 0; i < _prefabList.Count; i++)
            {
                if (_prefabList[i].PrefabKey != _defaultValue)
                {
                    if (_prefabDict.ContainsKey(_prefabList[i].PrefabKey))
                    {
                        if (_prefabDict[_prefabList[i].PrefabKey] != _prefabList[i].PrefabValue)
                        {
                            if (_keyType == KeyType.ENUM)
                            {
                                Debug.LogWarning("Prefab \'" + _prefabDict[_prefabList[i].PrefabKey] + "\' is already defined for \'" + _enumValues[_prefabList[i].PrefabKey] + "\'\nCannot define \'" + _prefabList[i].PrefabValue + "\' as well.");
                            }
                            else
                            {
                                Debug.LogWarning("Prefab \'" + _prefabDict[_prefabList[i].PrefabKey] + "\' is already defined for \'" + _prefabList[i].PrefabKey + "\'\nCannot define \'" + _prefabList[i].PrefabValue + "\' as well.");
                            }
                        }
                    }
                    else
                    {
                        _prefabDict.Add(_prefabList[i].PrefabKey, _prefabList[i].PrefabValue);
                    }
                }
            }
        }
        
        public void AddPrefab()
        {
            _prefabList.Add(new PrefabData(_defaultValue, null));
        }
        
        #endregion
    }
}

