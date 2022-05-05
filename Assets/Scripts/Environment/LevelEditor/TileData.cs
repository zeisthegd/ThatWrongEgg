using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Penwyn.LevelEditor
{
    [System.Serializable]
    public class TileData
    {
        public string BlockID;
        public Vector3 Position;
        public float RotationAngleY;

        public TileData(string blockID, Vector3 position, float rotationAngleY)
        {
            BlockID = blockID;
            Position = position;
            RotationAngleY = rotationAngleY;
        }
    }
}

