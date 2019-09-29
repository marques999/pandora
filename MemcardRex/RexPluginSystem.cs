using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace MemcardRex
{
    /// <summary>
    /// </summary>
    public class RexPluginSystem
    {
        /// <summary>
        /// </summary>
        private List<Assembly> _loadedAssemblies = new List<Assembly>();

        /// <summary>
        /// </summary>
        public List<PluginMetadata> AssembliesMetadata = new List<PluginMetadata>();

        /// <summary>
        /// </summary>
        /// <param name="pluginDirectory"></param>
        public void FetchPlugins(string pluginDirectory)
        {
            if (Directory.Exists(pluginDirectory) == false)
            {
                return;
            }

            _loadedAssemblies = new List<Assembly>();
            AssembliesMetadata = new List<PluginMetadata>();

            foreach (var dirFile in Directory.GetFiles(pluginDirectory, "*.dll"))
            {
                var assemblyTypes = new List<string>();
                var currentMetadata = new PluginMetadata();
                var currentAssembly = Assembly.LoadFile(dirFile);

                assemblyTypes.AddRange(currentAssembly.GetTypes().Select(loadedTypes => loadedTypes.ToString()));

                if (assemblyTypes.Contains("rexPluginSystem.rexPluginInterfaceV2") == false || assemblyTypes.Contains("rexPluginSystem.rexPlugin") == false)
                {
                    continue;
                }

                _loadedAssemblies.Add(currentAssembly);
                currentMetadata.Name = GetPluginName(_loadedAssemblies.Count - 1);
                currentMetadata.Author = GetPluginAuthor(_loadedAssemblies.Count - 1);
                currentMetadata.Games = GetPluginSupportedGames(_loadedAssemblies.Count - 1);
                AssembliesMetadata.Add(currentMetadata);
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="productCode"></param>
        /// <returns></returns>
        public int[] GetSupportedPlugins(string productCode)
        {
            if (_loadedAssemblies.Count <= 0)
            {
                return null;
            }

            var assembliesIndex = new List<int>();

            for (var assemblyIndex = 0; assemblyIndex < _loadedAssemblies.Count; assemblyIndex++)
            {
                var assemblyProductCode = new List<string>(GetSupportedProductCodes(assemblyIndex));

                if (assemblyProductCode.Contains(productCode) || assemblyProductCode.Contains("*.*"))
                {
                    assembliesIndex.Add(assemblyIndex);
                }
            }

            if (assembliesIndex.Count > 0)
            {
                return assembliesIndex.ToArray();
            }

            return null;
        }

        /// <summary>
        /// </summary>
        /// <param name="assemblyIndex"></param>
        /// <param name="methodName"></param>
        /// <returns></returns>
        private string ExecuteMethodString(int assemblyIndex, string methodName)
        {
            if (_loadedAssemblies.Count <= 0)
            {
                return null;
            }

            var assemblyType = _loadedAssemblies[assemblyIndex].GetType("rexPluginSystem.rexPlugin");

            return (string)assemblyType.InvokeMember(
                methodName,
                BindingFlags.Default | BindingFlags.InvokeMethod,
                null,
                Activator.CreateInstance(assemblyType),
                null
            );
        }

        /// <summary>
        /// </summary>
        /// <param name="assemblyIndex"></param>
        /// <param name="methodName"></param>
        /// <returns></returns>
        private IEnumerable<string> ExecuteMethodArray(int assemblyIndex, string methodName)
        {
            if (_loadedAssemblies.Count <= 0)
            {
                return null;
            }

            var assemblyType = _loadedAssemblies[assemblyIndex].GetType("rexPluginSystem.rexPlugin");

            return (string[])assemblyType.InvokeMember(
                methodName,
                BindingFlags.Default | BindingFlags.InvokeMethod,
                null,
                Activator.CreateInstance(assemblyType),
                null
            );
        }

        /// <summary>
        /// </summary>
        /// <param name="assemblyIndex"></param>
        /// <param name="methodName"></param>
        /// <param name="saveData"></param>
        /// <param name="saveProductCode"></param>
        /// <returns></returns>
        private byte[] ExecuteMethodByte(int assemblyIndex, string methodName, IEnumerable<byte> saveData, string saveProductCode)
        {
            if (_loadedAssemblies.Count <= 0)
            {
                return null;
            }

            var assemblyType = _loadedAssemblies[assemblyIndex].GetType("rexPluginSystem.rexPlugin");

            return (byte[])assemblyType.InvokeMember(methodName, BindingFlags.Default | BindingFlags.InvokeMethod, null, Activator.CreateInstance(assemblyType), new object[]
            {
                saveData, saveProductCode
            });
        }

        /// <summary>
        /// </summary>
        /// <param name="assemblyIndex"></param>
        /// <param name="methodName"></param>
        private void ExecuteMethodVoid(int assemblyIndex, string methodName)
        {
            if (_loadedAssemblies.Count <= 0)
            {
                return;
            }

            var assemblyType = _loadedAssemblies[assemblyIndex].GetType("rexPluginSystem.rexPlugin");

            assemblyType.InvokeMember(
                methodName,
                BindingFlags.Default | BindingFlags.InvokeMethod,
                null,
                Activator.CreateInstance(assemblyType),
                null
            );
        }

        /// <summary>
        /// </summary>
        /// <param name="assemblyIndex"></param>
        /// <returns></returns>
        private string GetPluginName(int assemblyIndex)
        {
            return ExecuteMethodString(assemblyIndex, "getPluginName");
        }

        /// <summary>
        /// </summary>
        /// <param name="assemblyIndex"></param>
        /// <returns></returns>
        private string GetPluginAuthor(int assemblyIndex)
        {
            return ExecuteMethodString(assemblyIndex, "getPluginAuthor");
        }

        /// <summary>
        /// </summary>
        /// <param name="assemblyIndex"></param>
        /// <returns></returns>
        private string GetPluginSupportedGames(int assemblyIndex)
        {
            return ExecuteMethodString(assemblyIndex, "getPluginSupportedGames");
        }

        /// <summary>
        /// </summary>
        /// <param name="assemblyIndex"></param>
        /// <returns></returns>
        private IEnumerable<string> GetSupportedProductCodes(int assemblyIndex)
        {
            return ExecuteMethodArray(assemblyIndex, "getSupportedProductCodes");
        }

        /// <summary>
        /// </summary>
        /// <param name="assemblyIndex"></param>
        /// <param name="saveData"></param>
        /// <param name="saveProductCode"></param>
        /// <returns></returns>
        public byte[] EditSaveData(int assemblyIndex, byte[] saveData, string saveProductCode)
        {
            return ExecuteMethodByte(assemblyIndex, "editSaveData", saveData, saveProductCode);
        }

        /// <summary>
        /// </summary>
        /// <param name="assemblyIndex"></param>
        public void ShowAboutDialog(int assemblyIndex)
        {
            ExecuteMethodVoid(assemblyIndex, "showAboutDialog");
        }

        /// <summary>
        /// </summary>
        /// <param name="assemblyIndex"></param>
        public void ShowConfigDialog(int assemblyIndex)
        {
            ExecuteMethodVoid(assemblyIndex, "showConfigDialog");
        }
    }
}