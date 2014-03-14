using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Reflection;

namespace CustomizationPlugInLoader
{
    public static class Program
    {
        [FISCA.MainMethod("舊版高中系統外掛載入器")]
        public static void Main()
        {
            AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(CurrentDomain_AssemblyResolve);

            #region 讀取Customize資料夾內的PlugIn
            var  dic = new System.IO.DirectoryInfo(Application.StartupPath + "\\Customize\\");
            if ( dic.Exists )
            {
                #region 掃描每一個有MainMethod屬性進入點的模組
                foreach ( FileInfo file in dic.GetFiles("*.dll") )
                {
                    Assembly assm;
                    try
                    {
                        assm = Assembly.LoadFile(file.FullName);
                    }
                    catch ( Exception e )
                    {
                        continue;
                    }
                    try
                    {
                        foreach ( Type type in assm.GetTypes() )
                        {
                            foreach ( MethodInfo method in type.GetMethods() )
                            {
                                if ( method.IsStatic )
                                {
                                    foreach ( Attribute att in Attribute.GetCustomAttributes(method, true) )
                                    {
                                        if ( att is SmartSchool.Customization.PlugIn.MainMethodAttribute )
                                        {
                                            try
                                            {
                                                method.Invoke(null, null);
                                            }
                                            catch ( Exception e )
                                            {
                                                //SmartSchool.ExceptionHandler.BugReporter.ReportException(e, false);
                                                continue;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    catch ( ReflectionTypeLoadException le )
                    {
                        //SmartSchool.ExceptionHandler.BugReporter.ReportException(le, false);
                    }
                    catch { }
                }
                #endregion

            }
            #endregion
        }

        static System.Reflection.Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            System.Reflection.AssemblyName name = new System.Reflection.AssemblyName(args.Name);
            if ( name.Name == "DevComponents.DotNetBar2" )
            {
                foreach ( var item in AppDomain.CurrentDomain.GetAssemblies() )
                {
                    if ( item.GetName().Name == name.Name && item.GetName().Version >= name.Version )
                        return item;
                }
            }
            return null;
        }
    }
}
