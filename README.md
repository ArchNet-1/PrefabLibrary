<p align="center">
  <a href="https://archnet-plateform.fr"><img src="https://archnet-plateform.fr/ArchNetLogoWithBck.png" alt="ArchNet" width="240"></a>
</p>

# ArchNet · Prefab Library

Prefabs stored by enum or int key and spawned by key, so spawners and level data never hold prefab references directly.

- Unity **6000.0** or newer · Needs `com.archnet.library.core` (the ArchNet installer adds them for you).
- Part of [ArchNet](https://archnet-plateform.fr), free and open-source Unity packages

## Install

*Window > Package Manager > + > Install package from git URL…*

```
https://github.com/ArchNet-1/PrefabLibrary.git
```

## Use

*Assets > Create > ArchNet > Prefab Library*, then:

```csharp
using ArchNet.Library;

GameObject goblin = _enemies.Spawn(Enemy.Goblin, spawnPoint.position, Quaternion.identity);
```

## License

MIT, see [LICENSE.md](LICENSE.md).
