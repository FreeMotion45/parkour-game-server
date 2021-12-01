using Assets.Scripts.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.ServerLogic
{
    class NetTransform : MonoBehaviour
    {
        public readonly static Dictionary<int, NetTransform> networkObjects = new Dictionary<int,  NetTransform>();

        private static int currentHash = 0;

        public static int NextHash()
        {
            return ++currentHash;
        }

        public static int RegisterNewNetObject(GameObject gameObject)
        {
            NetTransform nt = gameObject.GetComponent<NetTransform>();
            int hash = NextHash();
            networkObjects[hash] = nt;
            nt.transformHash = hash;
            return hash;
        }

        [HideInInspector] public int transformHash;

        private void Start()
        {
            //PeriodicalPlayerInformationSender.Instance.Add(this);
        }
    }
}
