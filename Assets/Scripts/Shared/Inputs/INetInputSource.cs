using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Shared.Inputs
{
    public interface INetInputSource
    {
        bool GetButton(string btn);
        bool GetButtonDown(string btn);
        bool GetButtonUp(string btn);
        int GetAxisRaw(string axis);
    }
}
