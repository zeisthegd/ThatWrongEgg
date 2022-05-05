using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Penwyn.LevelEditor
{
    [CreateAssetMenu(menuName = "Level Editor/Level Data")]
    public class LevelData : ScriptableObject
    {
        public string LevelName;
        public List<TileData> BlockMap;
    }
}

