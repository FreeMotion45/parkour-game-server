using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Messages.ClientOrigin
{
    [Serializable]
    class RequestJoinMessage
    {
        public string name;

        public RequestJoinMessage(string name)
        {
            this.name = name;
        }
    }
}
