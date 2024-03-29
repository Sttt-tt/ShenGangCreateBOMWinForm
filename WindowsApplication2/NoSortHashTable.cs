using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace WindowsApplication2
{

    /**/
    /// <summary>
    /// Summary description for NoSortedHashtable.
    /// </summary>
    public class NoSortHashtable : Hashtable
    {
        private ArrayList keys = new ArrayList();

        public NoSortHashtable()
        {
        }


        public override void Add(object key, object value)
        {
            base.Add(key, value);
            keys.Add(key);
        }

        public override ICollection Keys
        {
            get
            {
                return keys;
            }
        }

        public override void Clear()
        {
            base.Clear();
            keys.Clear();
        }

        public override void Remove(object key)
        {
            base.Remove(key);
            keys.Remove(key);
        }
        public override IDictionaryEnumerator GetEnumerator()
        {
            return base.GetEnumerator();
        }

    }
}

