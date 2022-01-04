using Assets.Scripts.Game.Weapons;
using Assets.Scripts.Messages.ClientOrigin;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityMultiplayer.Shared.Networking.Datagrams;

public class BaseGun : InventoryItem
{
    [Space]
    public Camera playerCamera;
    public Transform muzzle;
    public GameObject projectilePrefab;
    public LayerMask hittableLayers;

    [Space]
    public bool enableSfx;

    [Space]
    public float shootForce = 300f;
    public float maxHitDistance = 20f;
    public float minShootInterval = 0.1f;

    [Space]
    public int magazinesAvailable = 2;
    public int magazineSize = 8;
    public int currentAmmoInMagazine = 8;

    protected ParticleSystem muzzleFlashParticles;

    private float nextShootTime;

    #region Gun Logic
    public bool CanShoot()
    {
        return Time.time > nextShootTime && currentAmmoInMagazine > 0;
    }

    public bool TryShoot()
    {
        if (CanShoot())
        {
            Shoot();
            return true;
        }
        return false;
    }

    public virtual void Shoot()
    {
        if (CanShoot())
        {
            currentAmmoInMagazine--;
            nextShootTime = Time.time + minShootInterval;

            if (enableSfx)
            {
                PlayVisualEffects();
            }
        }
    }

    public bool CanReload()
    {
        return magazinesAvailable > 0;
    }

    public bool TryReload()
    {
        if (CanReload())
        {
            Reload();
            return true;
        }
        return false;
    }

    public virtual void Reload()
    {
        if (CanReload())
        {
            magazinesAvailable--;
            currentAmmoInMagazine = magazineSize;
        }
    }

    public virtual void AddAvailableMagazines(int magazines)
    {
        magazinesAvailable += magazines;
    }
    #endregion

    public virtual IEnumerable<Vector3> GetProjectilesDirections()
    {
        return new[] { GetDirectionFromMuzzle(playerCamera.transform.forward) };
    }

    public virtual void LaunchProjectiles()
    {
        foreach (Vector3 dir in GetProjectilesDirections())
        {
            InstantiateProjectile(dir);
        }
    }

    public virtual void PlayVisualEffects()
    {
        PlayParticles();
        LaunchProjectiles();
    }

    protected virtual void PlayParticles()
    {
        if (muzzleFlashParticles != null)
            muzzleFlashParticles.Play();
        else
            Debug.LogWarning(nameof(muzzleFlashParticles) + " is null, no muzzle particles played.");
    }

    protected virtual void InstantiateProjectile(Vector3 direction)
    {
        GameObject projectile = Instantiate(projectilePrefab, muzzle.position, Quaternion.identity);
        Rigidbody rb = projectile.GetComponent<Rigidbody>();
        rb.AddForce(direction.normalized * shootForce, ForceMode.VelocityChange);
    }

    public Vector3 GetDirectionFromMuzzle(Vector3 shootDirectionFromCameraCenter)
    {
        bool hit = Physics.Raycast(ScreenCenterInWorldSpace,
            shootDirectionFromCameraCenter,
            out RaycastHit info,
            maxHitDistance,
            hittableLayers
            );
        if (hit)
        {
            return info.point - muzzle.position;
        }
        return (ScreenCenterInWorldSpace + shootDirectionFromCameraCenter * maxHitDistance) - muzzle.position;
    }

    public Ray RayThroughCameraCenter => new Ray(ScreenCenterInWorldSpace, playerCamera.transform.forward);
    public Vector3 ScreenCenterInWorldSpace => playerCamera.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0));

    private void OnDrawGizmos()
    {
        Vector3 middleOfScreenInWorldSpace = playerCamera.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0));
        Gizmos.DrawLine(middleOfScreenInWorldSpace, maxHitDistance * playerCamera.transform.forward + middleOfScreenInWorldSpace);
    }
}
