// ArchNet · Prefab Library
// Stores prefabs under a key (an enum value such as Enemy.Goblin, or an int) and spawns
// them by key, so spawners and level data never hold prefab references directly.
// Create one with  Assets > Create > ArchNet > Prefab Library.
// Docs: https://archnet-plateform.fr/#PrefabLibrary

using System;
using UnityEngine;

namespace ArchNet.Library
{
    [CreateAssetMenu(fileName = "NewPrefabLibrary", menuName = "ArchNet/Prefab Library")]
    public class PrefabLibrary : KeyedLibrary<GameObject>
    {
        public GameObject GetPrefab(int key) => Get(key);
        public GameObject GetPrefab(Enum key) => Get(key);

        /// <summary>Creates a copy of the prefab stored for this key, or returns null when none is set.</summary>
        public GameObject Spawn(int key, Transform parent = null) =>
            TryGet(key, out GameObject prefab) && prefab != null ? Instantiate(prefab, parent) : null;

        /// <summary>Creates a copy of the prefab stored for this key at a position and rotation.</summary>
        public GameObject Spawn(int key, Vector3 position, Quaternion rotation, Transform parent = null) =>
            TryGet(key, out GameObject prefab) && prefab != null ? Instantiate(prefab, position, rotation, parent) : null;

        public GameObject Spawn(Enum key, Transform parent = null) => Spawn(Convert.ToInt32(key), parent);
    }
}
