using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Game.Weapons
{
    public class GunLogic
    {
        private float nextShootTime;

        public GunLogic(float minShootInterval, int magazinesAvailable, int magazineSize, int currentAmmoInMagazine)
        {
            MinShootInterval = minShootInterval;
            MagazinesAvailable = magazinesAvailable;
            MagazineSize = magazineSize;
            CurrentAmmoInMagazine = currentAmmoInMagazine;
        }

        public float MinShootInterval { get; set; }

        public int MagazinesAvailable { get; set; }
        public int MagazineSize { get; private set; }
        public int CurrentAmmoInMagazine { get; private set; }

        public bool CanShoot()
        {
            return Time.time > nextShootTime && CurrentAmmoInMagazine > 0;
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

        public void Shoot()
        {
            if (CanShoot())
            {
                CurrentAmmoInMagazine--;
                nextShootTime = Time.time + MinShootInterval;
            }
        }

        public bool CanReload()
        {
            return MagazinesAvailable > 0;
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

        public void Reload()
        {
            if (CanReload())
            {
                MagazinesAvailable--;
                CurrentAmmoInMagazine = MagazineSize;
            }
        }
    }
}
