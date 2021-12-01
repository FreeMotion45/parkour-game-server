using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Messages.ServerOrigin
{
    [Serializable]
    class LinkPlayerNameToIDMessage
    {
        public int id;
        public string name;

        public LinkPlayerNameToIDMessage(int id, string name)
        {
            this.name = name;
            this.id = id;
        }
    }
}
