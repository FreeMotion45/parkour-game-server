using Assets.Scripts.Game.Weapons;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class Shotgun : BaseGun
{
    [Header("Shotgun fields")]
    public float spreadAngle = 10f;
    public int projectiles = 6;

    public override IEnumerable<Vector3> GetProjectilesDirections()
    {
        Vector3 forward = RayThroughCameraCenter.direction;
        Quaternion rotation = Quaternion.LookRotation(forward);
        Vector3 right = rotation * Vector3.up;
        Vector3 up = rotation * Vector3.right;

        Vector3[] shootDirections = new Vector3[projectiles];
        for (int i = 0; i < projectiles; i++)
        {
            Quaternion randomHorizontal = Quaternion.AngleAxis(Random.Range(-spreadAngle, spreadAngle), up);
            Quaternion randomVertical = Quaternion.AngleAxis(Random.Range(-spreadAngle, spreadAngle), right);
            Vector3 direction = randomVertical * (randomHorizontal * forward);            
            shootDirections[i] = GetDirectionFromMuzzle(direction);
        }
        return shootDirections;
    }
}
