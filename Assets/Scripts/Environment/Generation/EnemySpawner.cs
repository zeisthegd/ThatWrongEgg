using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using NaughtyAttributes;

using Penwyn.Tools;

namespace Penwyn.Game
{
    public class EnemySpawner : MonoBehaviour
    {
        [Header("Map Data")]
        public MapData MapData;

        [Header("Pooling Settings")]
        public ObjectPooler EnemyPoolPrefab;
        [ReadOnly] public List<ObjectPooler> ObjectPoolers;

        [HorizontalLine]
        [Header("Spawn Settings")]
        public float MinDistanceToPlayer;
        public float MaxDistanceToPlayer;
        public float TimeTillSpawnNewEnemies = 2;


        protected float _waitToSpawnTime = 0;
        protected bool _isSpawning = false;
    }
}

