using Assets.Scripts.Game.Shared;
using Assets.Scripts.Game.Weapons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.ServerLogic.Player
{
    class Gun : MonoBehaviour
    {
        public static readonly Vector3 viewportCenter = new Vector3(0.5f, 0.5f, 0);

        public Camera playerCamera;
        public float maxShootingDistance;                  

        private ProjectileDispatcher projectileDispatcher;
        private Quaternion q;

        public GunLogic gunLogic { get; set; }

        private void Start()
        {
            projectileDispatcher = new ProjectileDispatcher();

            // This is hardcoded in Gun.cs in the client-side.
            gunLogic = new GunLogic(0.2f, 2, 8, 8);
        }

        public bool Shoot(Quaternion playerRotation, LayerMask mask, out RaycastHit info)
        {
            info = new RaycastHit();

            if (gunLogic.TryShoot())
            {                
                q = playerRotation;
                Vector3 middleOfScreenInWorldSpace = playerCamera.ViewportToWorldPoint(viewportCenter);
                projectileDispatcher.RaycastBullet(playerRotation, middleOfScreenInWorldSpace,
                    mask, maxShootingDistance, out info);
                return true;
            }
            return false;
        }

        public bool Reload()
        {
            return gunLogic.TryReload();
        }

        private void OnDrawGizmos()
        {
            Vector3 middleOfScreenInWorldSpace = playerCamera.ViewportToWorldPoint(viewportCenter);
            Gizmos.DrawLine(middleOfScreenInWorldSpace, maxShootingDistance * (q * Vector3.forward) + middleOfScreenInWorldSpace);
        }
    }
}
