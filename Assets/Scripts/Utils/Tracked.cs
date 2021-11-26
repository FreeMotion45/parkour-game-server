using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Utils
{
    public struct TrackedValue<T>
    {
        public T latest;
        public T previous;

        public TrackedValue(T startingValue)
        {
            previous = latest = startingValue;
        }

        public void Track(T vec)
        {
            previous = latest;
            latest = vec;
        }

        public bool Changed()
        {
            return !latest.Equals(previous);
        }
    }
}
