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
        public LayerMask hittableLayers;

        private ProjectileDispatcher projectileDispatcher;

        private void Start()
        {
            projectileDispatcher = new ProjectileDispatcher();
        }

        public GameObject Shoot(Quaternion playerRotation)
        {
            Vector3 middleOfScreenInWorldSpace = playerCamera.ViewportToWorldPoint(viewportCenter);
            bool hitAnything = projectileDispatcher.RaycastBullet(playerRotation, middleOfScreenInWorldSpace,
                hittableLayers, maxShootingDistance, out RaycastHit info);

            if (hitAnything)
                return info.collider.gameObject;
            return null;
        }
    }
}
