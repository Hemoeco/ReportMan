using System;
using System.Collections;

namespace Reportman.Reporting
{
    public class Params : IEnumerable, ICloneable
    {
        Param[] FItems;
        const int FIRST_ALLOCATION_OBJECTS = 10;
        int FCount;
        public Params()
        {
            FCount = 0;
            FItems = new Param[FIRST_ALLOCATION_OBJECTS];
        }
        public void Clear()
        {
            for (int i = 0; i < FCount; i++)
                FItems[i] = null;
            FCount = 0;
        }
        private void CheckRange(int index)
        {
            if ((index < 0) || (index >= FCount))
                throw new Exception("Index out of range on Params collection");
        }
        public int IndexOf(string avalue)
        {
            int aresult = -1;
            for (int i = 0; i < Count; i++)
            {
                if (FItems[i].Alias == avalue)
                {
                    aresult = i;
                    break;
                }
            }
            return aresult;
        }
        public int IndexOf(Param avalue)
        {
            int aresult = -1;
            for (int i = 0; i < Count; i++)
            {
                if (FItems[i] == avalue)
                {
                    aresult = i;
                    break;
                }
            }
            return aresult;
        }
        public Param this[int index]
        {
            get { CheckRange(index); return FItems[index]; }
            set { CheckRange(index); FItems[index] = value; }
        }
        public void Remove(Param nparam)
        {
            int index = IndexOf(nparam);
            if (index < 0)
                throw new Exception("Parameter does not exists:" + nparam.Alias);
            for (int i = index; i < FCount - 1; i++)
            {
                FItems[i] = FItems[i + 1];
            }
            FCount--;
        }
        public void RemoveAt(int index)
        {
            if ((index >= FCount) || (index < 0))
                throw new Exception("Parameter index out of range: " + index.ToString());
            for (int i = index; i < FCount - 1; i++)
            {
                FItems[i] = FItems[i + 1];
            }
            FCount--;
        }
        public Param this[string paramname]
        {
            get
            {
                int index = IndexOf(paramname);
                if (index >= 0)
                    return FItems[index];
                else
                    return null;
            }
        }
        public int Count { get { return FCount; } }
        public void Add(Param obj)
        {
            if (FCount > (FItems.Length - 2))
            {
                Param[] nobjects = new Param[FCount];
                System.Array.Copy(FItems, 0, nobjects, 0, FCount);
                FItems = new Param[FItems.Length * 2];
                System.Array.Copy(nobjects, 0, FItems, 0, FCount);
            }
            FItems[FCount] = obj;
            FCount++;
        }
        // IEnumerable Interface Implementation:
        //   Declaration of the GetEnumerator() method 
        //   required by IEnumerable
        public IEnumerator GetEnumerator()
        {
            return new ParamEnumerator(this);
        }
        // Inner class implements IEnumerator interface:
        public class ParamEnumerator : IEnumerator
        {
            private int position = -1;
            private Params t;

            public ParamEnumerator(Params t)
            {
                this.t = t;
            }

            // Declare the MoveNext method required by IEnumerator:
            public bool MoveNext()
            {
                if (position < t.Count - 1)
                {
                    position++;
                    return true;
                }
                else
                {
                    return false;
                }
            }

            // Declare the Reset method required by IEnumerator:
            public void Reset()
            {
                position = -1;
            }

            // Declare the Current property required by IEnumerator:
            public object Current
            {
                get
                {
                    return t[position];
                }
            }
        }
        public Params Clone(Report rp)
        {
            Params aparams = (Params)Clone();
            foreach (Param p in aparams)
            {
                p.Report = rp;
            }
            return aparams;
        }
        public object Clone()
        {
            Params aparams = new Params();
            foreach (Param p in this)
            {
                aparams.Add((Param)p.Clone());
            }
            return aparams;
        }
        public void Switch(int index1, int index2)
        {
            if ((index1 < 0) || (index2 < 0))
                throw new Exception("Index out of bounds in Params.Switch");
            if ((index1 >= FCount) || (index2 >= FCount))
                throw new Exception("Index out of bounds in Params.Switch");
            Param buf = FItems[index1];
            FItems[index1] = FItems[index2];
            FItems[index2] = buf;
        }
    }
}
