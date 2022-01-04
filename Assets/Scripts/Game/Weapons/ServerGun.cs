using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileHitInfo
{    
    public List<RaycastHit> hits = new List<RaycastHit>();
    public int damageDealt;
}

public class ServerGun : MonoBehaviour
{
    [Space]
    public int bulletDamage = 10;

    public Vector3 debug_muzzle;
    public Vector3 debug_dir;

    private BaseGun gun;

    void Start()
    {
        gun = GetComponent<BaseGun>();
    }

    void Update()
    {
        Debug.DrawLine(debug_muzzle, debug_muzzle + debug_dir * gun.maxHitDistance);
    }

    public Dictionary<GameObject, ProjectileHitInfo> CheckPlayersHit()
    {
        Dictionary<GameObject, ProjectileHitInfo> damageDealt = new Dictionary<GameObject, ProjectileHitInfo>();
        if (gun.TryShoot())
        {
            foreach (Vector3 dir in gun.GetProjectilesDirections())
            {
                bool hit = Physics.Raycast(gun.muzzle.position, dir, out RaycastHit info, 1000, gun.hittableLayers);
                debug_muzzle = gun.muzzle.position;
                debug_dir = dir;
                if (hit)
                {
                    if (!damageDealt.ContainsKey(info.collider.gameObject))
                    {
                        damageDealt[info.collider.gameObject] = new ProjectileHitInfo();
                    }
                    damageDealt[info.collider.gameObject].damageDealt += bulletDamage;
                    damageDealt[info.collider.gameObject].hits.Add(info);
                }
            }
        }
        return damageDealt;
    }
}
