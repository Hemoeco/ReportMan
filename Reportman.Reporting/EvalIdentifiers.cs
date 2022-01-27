using Reportman.Drawing;
using System.Collections;

namespace Reportman.Reporting
{
    /// <summary>
    /// Collections of Evaluator identifiers
    /// </summary>
    public class EvalIdentifiers : System.Collections.Generic.SortedList<string, EvalIdentifier>
    {
        /// <summary>
        /// Returns the eval identifier by index
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public EvalIdentifier this[int index]
        {
            get
            {
                CheckRange(index);
                return this[Keys[index]];
            }
            set
            {
                CheckRange(index);
                this[Keys[index]] = value;
            }
        }

        private void CheckRange(int index)
        {
            if ((index < 0) || (index >= Count))
                throw new UnNamedException("Index out of range on EvalIdentifiers collection");
        }
        /// <summary>
        /// IEnumerable Interface Implementation:
        ///   Declaration of the GetEnumerator() method 
        ///   required by IEnumerable
        /// </summary>
        /// <returns></returns>
        public new IEnumerator GetEnumerator()
        {
            return new EvalIdentifierEnumerator(this);
        }
        /// <summary>
        /// Inner class implements IEnumerator interface:
        /// </summary>
        public class EvalIdentifierEnumerator : IEnumerator
        {
            private int position = -1;
            private EvalIdentifiers t;
            /// <summary>
            /// Constructor for a enumerator of report items
            /// </summary>
            /// <param name="t"></param>
            public EvalIdentifierEnumerator(EvalIdentifiers t)
            {
                this.t = t;
            }

            /// <summary>
            /// Go to next element in the lis
            /// </summary>
            /// <returns></returns>
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

            /// <summary>
            /// Declare the Reset method required by IEnumerator
            /// </summary>
            public void Reset()
            {
                position = -1;
            }
            /// <summary>
            /// Declare the Current property required by IEnumerator
            /// </summary>
            public object Current
            {
                get
                {
                    return t[position];
                }
            }
        }
    }
}
