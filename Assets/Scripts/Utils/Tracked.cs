using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Utils
{
    public struct TrackedVector3
    {
        public Vector3 latest;
        public Vector3 previous;

        public TrackedVector3(Vector3 startingValue)
        {
            previous = latest = startingValue;
        }

        public void Track(Vector3 vec)
        {
            previous = latest;
            latest = vec;
        }

        public bool Changed()
        {
            return latest != previous;
        }
    }
}
