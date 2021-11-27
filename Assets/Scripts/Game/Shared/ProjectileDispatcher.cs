using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Game.Shared
{
    class ProjectileDispatcher
    {        
        public bool RaycastBullet(Quaternion playerRotation, Vector3 rayOrigin, LayerMask hittableLayers,
            float maxDistance, out RaycastHit info)
        {                        
            Vector3 furthestDestination = rayOrigin + playerRotation * Vector3.forward * maxDistance;

            bool hitAnything = Physics.Raycast(rayOrigin, furthestDestination,
                out info, maxDistance, hittableLayers);
            return hitAnything;
        }
    }
}
