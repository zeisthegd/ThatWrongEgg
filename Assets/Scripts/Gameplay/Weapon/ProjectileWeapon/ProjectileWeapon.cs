using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


using Photon.Pun;
using Penwyn.Tools;

namespace Penwyn.Game
{
    public class ProjectileWeapon : Weapon
    {
        public AimType AimType = AimType.Forward;
        protected ObjectPooler _projectilePooler;
        protected Vector3 _target;

        protected override void UseWeapon()
        {
            if (Owner.photonView.IsMine)
            {
                base.UseWeapon();
                StartCoroutine(IterationCoroutine());
            }
        }

        protected virtual IEnumerator IterationCoroutine()
        {
            if (_weaponAim)
                _weaponAim.enabled = false;
            for (int i = 0; i < CurrentData.Iteration; i++)
            {
                StartCoroutine(UseWeaponCoroutine());
                yield return new WaitForSeconds(CurrentData.DelayBetweenIterations);
            }
            if (_weaponAim)
                _weaponAim.enabled = true;
            if (_weaponAutoAim != null && _weaponAutoAim.enabled == false)
                _weaponAutoAim.enabled = true;
            InvokeWeaponUsedEvent();
            StartCooldown();
        }

        protected virtual IEnumerator UseWeaponCoroutine()
        {
            float projectileStep = GetProjectileStep();
            gameObject.RotateZ(CurrentData.Angle / 2F);
            for (int i = 0; i < CurrentData.BulletPerShot; i++)
            {
                _target = GetTarget();
                //SpawnProjectile(_target);
                photonView.RPC(nameof(RPC_SpawnProjectile), Photon.Pun.RpcTarget.AllViaServer, new object[] { transform.position, _target });
                if (CurrentData.BulletPerShot > 1)
                {
                    if (CurrentData.DelayBetweenBullets > 0)
                        yield return new WaitForSeconds(CurrentData.DelayBetweenBullets);
                    gameObject.RotateZ(-projectileStep);
                }
            }
        }

        /// <summary>
        /// Create a projectile, direction is based on the weapon's rotation.
        /// </summary>
        public virtual void SpawnProjectile(Vector3 target)
        {
            Projectile projectile = _projectilePooler.PullOneObject().GetComponent<Projectile>();
            projectile.transform.position = this.transform.position;
            projectile.transform.rotation = this.transform.rotation;
            projectile.gameObject.SetActive(true);
            projectile.FlyTowards(Vector3.ProjectOnPlane(target - this.transform.position, Vector3.up));// Don't fly on y.
            projectile.SetOwner(this.Owner);
        }

        [PunRPC]
        public virtual void RPC_SpawnProjectile(Vector3 startPos, Vector3 target)
        {
            Projectile projectile = _projectilePooler.PullOneObject().GetComponent<Projectile>();
            projectile.transform.position = startPos;
            projectile.transform.rotation = this.transform.rotation;
            projectile.gameObject.SetActive(true);
            projectile.FlyTowards(Vector3.ProjectOnPlane(target - startPos, Vector3.up));// Don't fly on y.
            projectile.SetOwner(this.Owner);
            Debug.DrawRay(target, Vector3.up * 100, Color.black);
        }

        /// <summary>
        /// Angle distance of each projectile.
        /// </summary>
        protected virtual float GetProjectileStep()
        {
            if (CurrentData.BulletPerShot != 0)
                return 1F * CurrentData.Angle / CurrentData.BulletPerShot;
            return 0;
        }

        public override void LoadWeapon(WeaponData data)
        {
            base.LoadWeapon(data);
            if (CurrentData.Projectile.DamageOnTouch)
                CurrentData.Projectile.DamageOnTouch.DamageDeal = CurrentData.Damage;
            CreateNewPool();
        }

        public virtual Vector3 GetTarget()
        {
            if (_weaponAutoAim != null)
            {
                return _weaponAutoAim.Target.position;
            }

            if (AimType == AimType.Raycast)
            {
                return RaycastTarget();
            }

            if (AimType == AimType.Forward)
            {
                return (Owner.transform.position + Owner.transform.forward * 100);
            }

            return Vector3.zero;
        }

        /// <summary>
        /// Cast a ray from the camera to the ground.
        /// </summary>
        /// <returns>Position on ground at mouse position.</returns>
        public virtual Vector3 RaycastTarget()
        {
            Vector3 target = Vector3.zero;
            target = Vector3.ProjectOnPlane(CursorManager.Instance.GetRayHitUnderMouse().point, Vector3.up);
            return target;
        }

        public virtual void CreateNewPool()
        {
            if (_projectilePooler.NoPoolFound())
            {
                _projectilePooler.ObjectToPool = CurrentData.Projectile.gameObject;
                _projectilePooler.ClearPool();
                _projectilePooler.Init();
            }
        }

        public override void GetComponents()
        {
            base.GetComponents();
            _projectilePooler = GetComponent<ObjectPooler>();
        }
    }

    public enum AimType
    {
        Raycast,
        Forward
    }
}
