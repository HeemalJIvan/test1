using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.EnterpriseServices.Internal;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text;

namespace ICompare.Setup.PostInstallerLibrary
{
    public class HealthCheck
    {
        private Dictionary<string, int> _versionDictionary = new Dictionary<string, int>();
        private List<Tuple<string, string>> _allFluidICompareFiles = new List<Tuple<string, string>>();
        //private string[] _allAssemblies = null;
        public string[] _allAssemblies = null;

        public List<string> ErrorList { get; set; }

        public HealthCheck()
        {
            try
            {
                ErrorList = new List<string>();

                string temp = Directory.GetCurrentDirectory();

                //string temp = @"C:\\Doorway\\ICompare 4\\lib";

                //var allDlls = Directory.GetFiles(Directory.GetCurrentDirectory(), "*.dll", SearchOption.AllDirectories);
                var _allDlls = Directory.GetFiles(temp, "*.dll", SearchOption.AllDirectories);
                var _exes = Directory.GetFiles(temp, "*.exe", SearchOption.AllDirectories);

                List<string> allAsms = new List<string>();
                allAsms.AddRange(_allDlls);
                allAsms.AddRange(_exes);
                _allAssemblies = allAsms.ToArray();

                foreach (var v in _allAssemblies.Where(x => x.Contains("Fluid.") || x.Contains("ICompare.")))
                {
                    var fileVersion = FileVersionInfo.GetVersionInfo(v);
                    _allFluidICompareFiles.Add(new Tuple<string, string>(v, fileVersion.FileVersion));

                    if (_versionDictionary.ContainsKey(fileVersion.FileVersion))
                        _versionDictionary[fileVersion.FileVersion]++;
                    else
                        _versionDictionary.Add(fileVersion.FileVersion, 1);
                }
            }
            catch (Exception ex)
            {

                throw new Exception("Error occurred in health check. {" + ex.Message+"}");
            }
        }

        public bool CheckDllVersions()
        {
            try
            {
                int modalVersionCount = _versionDictionary.Values.Max();
                string modalVersion = _versionDictionary.First(x => x.Value == modalVersionCount).Key;

                var outOfSyncFiles = _allFluidICompareFiles.Where(x => x.Item2 != modalVersion);

                foreach (var v in outOfSyncFiles)
                {
                    FileInfo fi = new FileInfo($"{v.Item1} - {v.Item2}");

                    ErrorList.Add(fi.Name);
                }

                return outOfSyncFiles.Count() == 0;
            }
            catch (Exception ex)
            {

                throw new Exception("Error occurred on check dll versions. {" + ex.Message + "}");
            }

        }

        public bool CheckDllReferences(bool ReadGAC = false)
        {
            try
            {
                #region Lists
                List<string> missingAssemblies = new List<string>();
                List<string> AllExlcudedDlls = new List<string>();
                List<string> ExcludedDlls = new List<string>();
                List<string> VerifiedMissingAssemblies = new List<string>();
                List<string> LibList = new List<string>();
                List<string> VerifiedLibList = new List<string>();
                List<string> SetExcludedList = new List<string>
                {
                "ICompareCore.Test.UnitTests",
                "Microsoft.Dynamic",
                "Microsoft.Exchange.WebServices",
                "MsgKit",
                "Renci.SshNet",
                "Solar.Common.Transformers.ActivityTransformers",
                "ICSharpCode.SharpZipLib",
                "System.Buffers",
                "System.Runtime.InteropServices.RuntimeInformation",
                "Mono.Posix"
                };
                #endregion
                if (!ReadGAC)
                {
                    string _OSDirectory = Path.GetPathRoot(Environment.SystemDirectory);
                    string _AssemblyDirectory = _OSDirectory + "Windows\\assembly";
                    var _ExcludedFiles = Directory.GetFiles(_AssemblyDirectory, "*.dll", SearchOption.AllDirectories)
                                         .Select(Path.GetFileNameWithoutExtension)
                                         .ToList();
                    ExcludedDlls = _ExcludedFiles.Distinct().ToList();
                    ExcludedDlls = ExcludedDlls.ConvertAll(s => s.Replace(".ni", ""));
                }
                LibList.AddRange(_allAssemblies.Select(Path.GetFileNameWithoutExtension));
                LibList = LibList.ConvertAll(s => s.Replace(".ni", ""));
                LibList.ToList().Sort();
                foreach (var v in _allAssemblies)
                {
                    Assembly assembly = Assembly.LoadFrom(v);

                    var assemblies = assembly.GetReferencedAssemblies().ToList();
                    List<string> assembliescheck = new List<string>();

                    foreach (var asm in assemblies)
                    {
                        assembliescheck.Add(asm.Name);                          
                    }
                    assembliescheck.Sort();
                    assemblies.Distinct();
                    missingAssemblies.AddRange(assembliescheck.Except(LibList).ToList());
                }
                missingAssemblies.Sort();
                VerifiedLibList = missingAssemblies.Count() == 0 ? new List<string>() : missingAssemblies.Except(LibList).ToList();
                List<string> VerifiedSetExcludedList = VerifiedLibList.Except(SetExcludedList).ToList();
                VerifiedSetExcludedList.RemoveAll(rs => rs.StartsWith("System"));
                VerifiedSetExcludedList.RemoveAll(rm => rm.StartsWith("Microsoft"));
                // Add to config for testing and set to true to view all missing assemblies <add key="ReadGAC" value="false"/>
                if (!ReadGAC)
                {
                    ExcludedDlls.Sort();
                    VerifiedMissingAssemblies = VerifiedSetExcludedList.Count() == 0 ? new List<string>() : VerifiedSetExcludedList.Except(ExcludedDlls).ToList();
                }
                else
                {
                    VerifiedMissingAssemblies = VerifiedLibList;
                }

                foreach (string s in VerifiedMissingAssemblies)
                    ErrorList.Add($"Assembly not found - {s}");


                return VerifiedMissingAssemblies.Count() == 0;
            }
            catch (Exception ex)
            {

                throw new Exception("Error occurred in check for DLL references. {"+ex.Message+"}");
            }
        }


        private void PerformReferenceAnalysis(System.Reflection.Assembly assembly)
        {
            StringBuilder builder = new StringBuilder();
            HashSet<string> loadedAssemblies = new HashSet<string>();
            PerformReferenceAnalysis(assembly, builder, string.Empty, loadedAssemblies);
            string temp = builder.ToString();
        }

        private void PerformReferenceAnalysis(System.Reflection.Assembly assembly, StringBuilder builder, string leadingWhitespace, HashSet<string> loadedAssemblies)
        {
            if (builder.Length > 0)
            {
                builder.AppendLine();
            }
            builder.Append(leadingWhitespace + assembly.FullName);
            System.Reflection.AssemblyName[] referencedAssemblies = assembly.GetReferencedAssemblies();
            foreach (System.Reflection.AssemblyName assemblyName in referencedAssemblies)
            {
                if (loadedAssemblies.Contains(assemblyName.Name))
                {
                    continue;
                }
                loadedAssemblies.Add(assemblyName.Name);
                System.Reflection.Assembly nextAssembly;
                try
                {
                    nextAssembly = System.Reflection.Assembly.ReflectionOnlyLoad(assemblyName.FullName);
                }
                catch (Exception)
                {
                    try
                    {
                        nextAssembly = System.Reflection.Assembly.ReflectionOnlyLoad(assemblyName.Name);
                    }
                    catch (Exception)
                    {
                        nextAssembly = null;
                    }
                }
                if (nextAssembly != null)
                {
                    PerformReferenceAnalysis(nextAssembly, builder, leadingWhitespace + "| ", loadedAssemblies);
                }
            }
        }
    }
}