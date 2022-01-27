using System.Collections.Generic;
using System.IO;

namespace Reportman.Reporting
{
    public class ExternalReport
    {
        public static void ExportSubReport(SubReport subreport,System.IO.Stream destination)
        {
            using (MemoryStream mstream = new MemoryStream())
            {
                subreport.Report.SaveToStream(mstream);
                mstream.Seek(0, SeekOrigin.Begin);
                Report newreport = new Report();
                newreport.LoadFromStream(mstream);
                // Drop other subreports
                List<SubReport> lsubreports = new List<SubReport>();
                List<SubReport> lchildsubreports = new List<SubReport>();
                foreach (SubReport subrep in newreport.SubReports)
                {
                    if (subrep.Name != subreport.Name)                    
                        lsubreports.Add(subrep);
                    else
                    {
                        foreach (Section nsection in subrep.Sections)
                        {
                            if (nsection.ChildSubReport != null)
                            {
                                lchildsubreports.Add(nsection.ChildSubReport);
                            }
                        }
                    }

                }
                foreach (SubReport subrep in lsubreports)
                {
                    if (lchildsubreports.IndexOf(subrep)<0)
                        DeleteSubReport(newreport, subrep);
                }
                newreport.SaveToStream(destination);
            }
        }
        public static void DeleteSubReport(Report newreport,SubReport subrep)
        {
            // Remove related dataset
            if (subrep.Alias.Length > 0)
            {
                // Remove parameters not assigned to the subreport
                List<Param> lparams = new List<Param>();
                foreach (Param rparam in newreport.Params)
                {
                    if (rparam.Datasets.Count > 0)
                    {
                        if (rparam.Datasets.IndexOf(subrep.Alias) < 0)
                        {
                            /*newreport.Params.Remove(rparam);
                            int index2 = newreport.Components.IndexOfValue(rparam);
                            if (index2 >= 0)
                                newreport.Components.RemoveAt(index2);*/
                        }
                        else
                        {
                            List<string> toremove = new List<string>();
                            foreach (string ndataset in rparam.Datasets)
                            {
                                if (ndataset != subrep.Alias)
                                {
                                    toremove.Add(ndataset);
                                }
                            }
                            foreach (string nstring in toremove)
                            {
                                rparam.Datasets.Remove(nstring);
                                if (rparam.Datasets.Count == 0)
                                {
                                    newreport.Params.Remove(rparam);
                                    int index2 = newreport.Components.IndexOfValue(rparam);
                                    if (index2 >= 0)
                                        newreport.Components.RemoveAt(index2);
                                }
                            }
                        }
                    }
                }
                int indexdata = newreport.DataInfo.IndexOf(subrep.Alias);
                if (indexdata >= 0)
                {
                    DataInfo dinfo = newreport.DataInfo[subrep.Alias];
                    newreport.DataInfo.Remove(dinfo);
                    int index = newreport.Components.IndexOfValue(dinfo);
                    if (index >= 0)
                        newreport.Components.RemoveAt(index);
                    // Remove related union datasets
                    foreach (string related in dinfo.DataUnions)
                    {
                        if (newreport.DataInfo.IndexOf(related)>=0)
                        {
                            newreport.DataInfo.Remove(newreport.DataInfo[related]);
                        }
                    }
                }
            }
            newreport.DeleteSubReport(subrep);
        }
        public static void ImportReport(Report destination,Report source)
        {
            foreach (DataInfo dinfo in source.DataInfo)
            {
                if (destination.DataInfo.IndexOf(dinfo.Alias) >= 0)
                {
                    //throw new Exception("Ya existe un dataset llamado " + dinfo.Alias);
                }
            }
            foreach (Param nparam in source.Params)
            {
                if (destination.Params.IndexOf(nparam.Alias)<=0)
                {
                    if (destination.Components.IndexOfKey(nparam.Name)>=0)
                    {
                        destination.GenerateNewName(nparam);
                    }
                    destination.Params.Add(nparam);
                    destination.Components.Add(nparam.Name, nparam);
                }
                else
                {
                    Param destparam = destination.Params[nparam.Alias];
                    foreach (string datasetname in destparam.Datasets)
                    {
                        if (destparam.Datasets.IndexOf(datasetname) < 0)
                            destparam.Datasets.Add(datasetname);
                    }
                }
            }

            foreach (DatabaseInfo dbinfo in source.DatabaseInfo)
            {
                if (destination.DatabaseInfo.IndexOf(dbinfo.Alias) < 0)
                {
                    if (destination.Components.IndexOfKey(dbinfo.Name) >= 0)
                    {
                        destination.GenerateNewName(dbinfo);
                    }
                    destination.Components.Add(dbinfo.Name, dbinfo);
                    destination.DatabaseInfo.Add(dbinfo);
                }
            }
            foreach (DataInfo dinfo in source.DataInfo)
            {
                if (destination.Components.IndexOfKey(dinfo.Name)>=0)
                {
                    destination.GenerateNewName(dinfo);
                }
                destination.Components.Add(dinfo.Name, dinfo);
                destination.DataInfo.Add(dinfo);
            }
            foreach (SubReport nsubreport in source.SubReports)
            {
                if (destination.Components.IndexOfKey(nsubreport.Name)>=0)
                {
                    destination.GenerateNewName(nsubreport);
                }
                destination.Components.Add(nsubreport.Name,nsubreport);
                destination.SubReports.Add(nsubreport);
            }
        }
    }
}
