using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.ServerLogic.PickUp
{
    class MagazineAmmoSpawner : MonoBehaviour
    {        
        public IncrementingPositionByTransformContainer positionGenerator;

        public GameObject ammoPickUpPrefab;

        public float spawnIntervalMin = 10f;
        public float spawnIntervalMax = 20f;

        private float nextSpawnTime;

        private void OnEnable()
        {
            nextSpawnTime = UnityEngine.Random.Range(spawnIntervalMin, spawnIntervalMax);
        }

        private void FixedUpdate()
        {
            if (Time.time > nextSpawnTime)
            {
                nextSpawnTime = Time.time + UnityEngine.Random.Range(spawnIntervalMin, spawnIntervalMax);
                SpawnItem(positionGenerator.GetNextPosition());                
            }
        }

        private void SpawnItem(Vector3 spawnPosition)
        {
            Instantiate(ammoPickUpPrefab, spawnPosition, Quaternion.identity);
            Debug.Log("A new pick up has been spawned!");
        }
    }
}
