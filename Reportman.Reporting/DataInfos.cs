using System;

namespace Reportman.Reporting
{
    /// <summary>
    /// Collection of DataInfo items
    /// </summary>
    public class DataInfos : System.Collections.Generic.List<DataInfo>, ICloneable
    {
        /// <summary>
        /// Get item by name
        /// </summary>
        /// <param name="dname">Dataset name or alias</param>
        /// <returns></returns>
        public DataInfo this[string dname]
        {
            get
            {
                int index = IndexOf(dname);
                if (index >= 0)
                    return this[index];
                else
                    return null;
            }
        }
        /// <summary>
        /// Returns index by name (alias)
        /// </summary>
        /// <param name="avalue">Alias to search for</param>
        /// <returns>Index or -1 when not found</returns>
        public int IndexOf(string avalue)
        {
            int aresult = -1;
            for (int i = 0; i < Count; i++)
            {
                if (this[i].Alias == avalue)
                {
                    aresult = i;
                    break;
                }
            }
            return aresult;
        }
        /// <summary>
        /// Clone the DataInfo collection
        /// </summary>
        /// <returns>A new DataInfo collection</returns>
        public object Clone()
        {
            DataInfos ninfo = new DataInfos();
            foreach (DataInfo ainfo in this)
            {
                ninfo.Add((DataInfo)ainfo.Clone());
            }
            return ninfo;
        }
        /// <summary>
        /// Clone the DataInfo collection
        /// </summary>
        /// <param name="areport">New owner</param>
        /// <returns>A new DataInfo collection</returns>
        public DataInfos Clone(Report areport)
        {
            DataInfos ninfo = new DataInfos();
            foreach (DataInfo ainfo in this)
            {
                ninfo.Add(ainfo.Clone(areport));
            }
            return ninfo;
        }
    }

}
