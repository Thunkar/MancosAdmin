using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MancosAdmin.Model
{
    class Player
    {
        public string Name { get; set; }

        public Player(string name)
        {
            this.Name = name;
        }


        public override bool Equals(object anotherPlayer)
        {
            if (anotherPlayer is Player) return this.Name.Equals(((Player)anotherPlayer).Name);
            else return base.Equals(anotherPlayer);
        }

        public override int GetHashCode()
        {
            byte[] stringBytes = System.Text.Encoding.ASCII.GetBytes(this.Name);
            int sum = 0;
            for (int i = 0; i < stringBytes.Length; i++)
            {
                sum += stringBytes[i];
            }
            return this.Name.Length ^ sum;
        }

    }
}
