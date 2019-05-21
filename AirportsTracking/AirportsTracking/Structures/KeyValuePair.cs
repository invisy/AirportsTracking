using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Structures
{
    class KeyValuePair<Tkey, Tval> : IComparable<KeyValuePair<Tkey, Tval>> where Tkey : IComparable
    {
        public Tkey key;
        public Tval value;

        public KeyValuePair(Tkey key, Tval value)
        {
            this.key = key;
            this.value = value;
        }

        public int CompareTo(KeyValuePair<Tkey, Tval> kvp)
        {
            return this.key.CompareTo(kvp.key);
        }
    }
}
