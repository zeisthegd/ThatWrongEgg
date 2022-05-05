using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Penwyn.Game
{
    [RequireComponent(typeof(ObjectPooler))]
    public class Grenade : Projectile
    {
        public ObjectPooler ExplosionPooler;

        protected override void Awake()
        {
            base.Awake();
            if (ExplosionPooler == null)
                ExplosionPooler = GetComponent<ObjectPooler>();
            ExplosionPooler.Init();
            _health.OnDeath += SpawnExplosion;
        }

        public virtual void SpawnExplosion(Character grenade)
        {
            var explosion = ExplosionPooler.PullOneObject();
            explosion.transform.position = this.transform.position;
            explosion.SetActive(true);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
        }
    }

}