using Multiplayer_Games_Programming_Packet_Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Multiplayer_Games_Programming_Server
{
    internal class GameObject
    {
        private ObjData data;
        public GameObject()
        {

        }

        public void UpdateData(ObjData data)
        {
            lock (this)
            {
                this.data = data;
            }
        }

        public ObjData GetData() { return data; }

    }
}