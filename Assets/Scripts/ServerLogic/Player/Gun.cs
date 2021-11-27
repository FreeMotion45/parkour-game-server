using Assets.Scripts.Game.Shared;
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

        private void Start()
        {
            projectileDispatcher = new ProjectileDispatcher();
        }

        public GameObject Shoot(Quaternion playerRotation, LayerMask mask)
        {
            q = playerRotation;
            Vector3 middleOfScreenInWorldSpace = playerCamera.ViewportToWorldPoint(viewportCenter);
            bool hitAnything = projectileDispatcher.RaycastBullet(playerRotation, middleOfScreenInWorldSpace,
                mask, maxShootingDistance, out RaycastHit info);

            if (hitAnything)
                return info.collider.gameObject;
            return null;
        }

        private void OnDrawGizmos()
        {
            Vector3 middleOfScreenInWorldSpace = playerCamera.ViewportToWorldPoint(viewportCenter);
            Gizmos.DrawLine(middleOfScreenInWorldSpace, maxShootingDistance * (q * Vector3.forward) + middleOfScreenInWorldSpace);
        }
    }
}
