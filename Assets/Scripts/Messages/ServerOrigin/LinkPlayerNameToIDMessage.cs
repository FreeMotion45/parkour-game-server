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
        public string name;
        public int id;

        public LinkPlayerNameToIDMessage(string name, int id)
        {
            this.name = name;
            this.id = id;
        }
    }
}
