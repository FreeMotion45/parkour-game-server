using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Server.Inputs
{
    public interface INetInputReceiver
    {
        void Tick(INetInputSource source);
    }
}
