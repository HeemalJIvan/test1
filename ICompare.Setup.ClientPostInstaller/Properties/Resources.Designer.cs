//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ICompare.Setup.ClientPostInstaller.Properties {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "16.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Resources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("ICompare.Setup.ClientPostInstaller.Properties.Resources", typeof(Resources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &lt;?xml version=&quot;1.0&quot; encoding=&quot;utf-8&quot;?&gt;
        ///&lt;configuration&gt;
        ///  &lt;configSections&gt;
        ///    &lt;section name=&quot;log4net&quot; type=&quot;log4net.Config.Log4NetConfigurationSectionHandler,log4net&quot; /&gt;
        ///  &lt;/configSections&gt;
        ///
        ///  &lt;runtime&gt;
        ///    &lt;assemblyBinding xmlns=&quot;urn:schemas-microsoft-com:asm.v1&quot;&gt;
        ///      &lt;probing privatePath=&quot;[PRIVATEPATH]&quot; /&gt;
        ///    &lt;/assemblyBinding&gt;
        ///  &lt;/runtime&gt;
        ///
        ///  &lt;appSettings&gt;
        ///    &lt;add key=&quot;ScriptPath&quot; value=&quot;[SCRIPTPATH]&quot;/&gt;
        ///  &lt;/appSettings&gt;
        ///  
        ///  &lt;!-- This section contains the log4net configuration setting [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string Fluid_UI_Shell_exe {
            get {
                return ResourceManager.GetString("Fluid_UI_Shell_exe", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &lt;?xml version=&quot;1.0&quot; encoding=&quot;utf-8&quot; ?&gt;
        ///&lt;configuration&gt;
        ///  &lt;connectionStrings&gt;
        ///    &lt;add name=&quot;OperationsDb&quot; connectionString=&quot;Data Source=MSISIGNATURE142\MSISIGNATURE2017;Initial Catalog=iCompare_VatGlobal_Operations;User Id=Jaco;Password=Signature@1234&quot; providerName=&quot;System.Data.SqlClient&quot;/&gt;
        ///    &lt;add name=&quot;MasterDb&quot; connectionString=&quot;Data Source=VITUKSQLTEST01;Initial Catalog=iCompare_Master_Vitality_Invest_Prod;User Id=icompare;Password=icompare2012&quot; providerName=&quot;System.Data.SqlClient&quot;/&gt;
        ///  &lt;/connecti [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string ICompareCore_Test_DBToolConsole_exe {
            get {
                return ResourceManager.GetString("ICompareCore_Test_DBToolConsole_exe", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &lt;?xml version=&quot;1.0&quot; encoding=&quot;utf-8&quot; ?&gt;
        ///&lt;configuration&gt;
        ///  &lt;connectionStrings&gt;
        ///    &lt;add name=&quot;VikingsX&quot; connectionString=&quot;Data Source=WYBO;Integrated Security=True&quot; /&gt;
        ///  &lt;/connectionStrings&gt;
        ///
        ///  &lt;runtime&gt;
        ///    &lt;assemblyBinding xmlns=&quot;urn:schemas-microsoft-com:asm.v1&quot;&gt;
        ///      &lt;probing privatePath=&quot;[PRIVATEPATH]&quot; /&gt;
        ///    &lt;/assemblyBinding&gt;
        ///  &lt;/runtime&gt;
        ///
        ///  &lt;appSettings&gt;
        ///    &lt;add key=&quot;DbName&quot; value=&quot;VikingsX3&quot; /&gt;
        ///    &lt;add key=&quot;DbCreateMethod&quot; value=&quot;Full&quot; /&gt;
        ///    &lt;add key=&quot;CommandTimeout&quot; value=&quot;200&quot; / [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string ICompareCore_Test_TestBench_exe {
            get {
                return ResourceManager.GetString("ICompareCore_Test_TestBench_exe", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to --IF (NOT EXISTS(SELECT * FROM MenubarItem WHERE ActionName = &apos;V3.ProcessFlowManager&apos;))
        ///--BEGIN
        ///--INSERT INTO MenubarItem(Id, MenubarId, ParentId, Name, Header, ImageSource, ActionName, ToolTip, Seq, AccessLevel, StartUp, GroupId)
        ///--VALUES ((SELECT MAX(Id) + 1 FROM MenubarItem), 1, (SELECT Id FROM MenubarItem WHERE Name = &apos;Setup&apos;), &apos;ProcessFlowManager&apos;, &apos;Process Flow&apos;, NULL, &apos;V3.ProcessFlowManager&apos;, NULL, (SELECT MAX(Seq) + 1 FROM MenubarItem WHERE ActionName LIKE &apos;Setup%&apos;), 100, NULL, NULL)
        ///--END
        ///
        ///
        /// [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string Post_Install {
            get {
                return ResourceManager.GetString("Post_Install", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized resource of type System.Byte[].
        /// </summary>
        internal static byte[] settings {
            get {
                object obj = ResourceManager.GetObject("settings", resourceCulture);
                return ((byte[])(obj));
            }
        }
    }
}