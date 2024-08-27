using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Hjmos.Lcdp.VisualEditor.Core.Services;

namespace Hjmos.Lcdp.VisualEditor.Core.ItemExtensions
{
    /// <summary>
    /// 管理设计上下文的扩展创建。
    /// </summary>
    public sealed class ExtensionManager
    {
        private readonly DesignContext _context;

        internal ExtensionManager(DesignContext context)
        {
            Debug.Assert(context != null);
            _context = context;

            context.Services.RunWhenAvailable<IComponentService>(componentService => componentService.ComponentRegistered += OnComponentRegistered);
        }

        private void OnComponentRegistered(object sender, DesignItemEventArgs e) => e.Item.SetExtensionServers(this, GetExtensionServersForItem(e.Item));

        /// <summary>
        /// 将ExtensionServer中的扩展重新应用到指定的设计项。
        /// </summary>
        private void ReapplyExtensions(IEnumerable<DesignItem> items, ExtensionServer server)
        {
            foreach (DesignItem item in items)
            {
                if (item != null)
                {
                    item.ReapplyExtensionServer(this, server);
                }
            }
        }

        #region Manage ExtensionEntries
        private sealed class ExtensionEntry
        {
            internal readonly Type ExtensionType;
            internal readonly ExtensionServer Server;
            internal readonly List<Type> OverriddenExtensionTypes = new List<Type>();
            internal readonly int Order;

            public ExtensionEntry(Type extensionType, ExtensionServer server, Type overriddenExtensionType, int order)
            {
                this.ExtensionType = extensionType;
                this.Server = server;
                this.OverriddenExtensionTypes.Add(overriddenExtensionType);
                this.Order = order;
            }

            public ExtensionEntry(Type extensionType, ExtensionServer server, List<Type> overriddenExtensionTypes, int order)
            {
                this.ExtensionType = extensionType;
                this.Server = server;
                this.OverriddenExtensionTypes = overriddenExtensionTypes;
                this.Order = order;
            }
        }

        private readonly Dictionary<Type, List<ExtensionEntry>> _extensions = new();

        private void AddExtensionEntry(Type extendedItemType, ExtensionEntry entry)
        {
            if (!_extensions.TryGetValue(extendedItemType, out List<ExtensionEntry> list))
            {
                list = _extensions[extendedItemType] = new List<ExtensionEntry>();
            }
            list.Add(entry);
        }

        /// <summary>
        /// 删除一个扩展窗体类型，所以它不会被使用!  
        /// </summary>
        /// <param name="extendedItemType"></param>
        /// <param name="extensionType"></param>
        public void RemoveExtension(Type extendedItemType, Type extensionType)
        {
            if (!_extensions.TryGetValue(extendedItemType, out List<ExtensionEntry> list))
            {
                list = _extensions[extendedItemType] = new List<ExtensionEntry>();
            }
            list.RemoveAll(x => x.ExtensionType == extensionType);
        }

        private List<ExtensionEntry> GetExtensionEntries(Type extendedItemType)
        {
            List<ExtensionEntry> result = new();
            List<Type> overriddenExtensions = new();
            IEnumerable<ExtensionEntry> ie = _extensions.Where(x => x.Key.IsAssignableFrom(extendedItemType)).SelectMany(x => x.Value);
            foreach (ExtensionEntry entry in ie)
            {
                if (!overriddenExtensions.Contains(entry.ExtensionType))
                {
                    overriddenExtensions.AddRange(entry.OverriddenExtensionTypes);

                    result.RemoveAll(x => overriddenExtensions.Contains(x.ExtensionType));
                    result.Add(entry);
                }
            }
            return result.OrderBy(x => x.Order).ToList();
        }

        /// <summary>
        /// 获取应用于指定项类型的所有扩展的所有类型。
        /// </summary>
        public IEnumerable<Type> GetExtensionTypes(Type extendedItemType)
        {
            if (extendedItemType == null)
                throw new ArgumentNullException("extendedItemType");

            foreach (ExtensionEntry entry in GetExtensionEntries(extendedItemType))
            {
                yield return entry.ExtensionType;
            }
        }
        #endregion

        #region Create Extensions
        private static readonly ExtensionEntry[] emptyExtensionEntryArray = new ExtensionEntry[0];

        private IEnumerable<ExtensionEntry> GetExtensionEntries(DesignItem extendedItem) => extendedItem.Component == null ? emptyExtensionEntryArray : GetExtensionEntries(extendedItem.ComponentType);

        /// <summary>
        /// 获取设计项的扩展服务
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        private ExtensionServer[] GetExtensionServersForItem(DesignItem item)
        {
            Debug.Assert(item != null);

            HashSet<ExtensionServer> servers = new();
            foreach (ExtensionEntry entry in GetExtensionEntries(item))
            {
                servers.Add(entry.Server);
            }
            return servers.ToArray();
        }

        internal IEnumerable<Extension> CreateExtensions(ExtensionServer server, DesignItem item, Type extensionType = null)
        {
            Debug.Assert(server != null);
            Debug.Assert(item != null);

            foreach (ExtensionEntry entry in GetExtensionEntries(item))
            {
                if (entry.Server == server && (extensionType == null || entry.ExtensionType == extensionType))
                {
                    string disabledExtensions = Extension.GetDisabledExtensions(item.View);
                    if (string.IsNullOrEmpty(disabledExtensions) || !disabledExtensions.Split(';').Contains(entry.ExtensionType.Name))
                        yield return server.CreateExtension(entry.ExtensionType, item);
                }
            }
        }

        #endregion

        #region RegisterAssembly
        private readonly HashSet<Assembly> _registeredAssemblies = new();

        /// <summary>
        /// 从指定程序集注册扩展。
        /// </summary>
        public void RegisterAssembly(Assembly assembly)
        {
            if (assembly == null)
                throw new ArgumentNullException("assembly");

            //			object[] assemblyAttributes = assembly.GetCustomAttributes(typeof(IsWpfDesignerAssemblyAttribute), false);
            //			if (assemblyAttributes.Length == 0)
            //				return;

            if (!_registeredAssemblies.Add(assembly))
            {
                // the assembly already is registered, don't try to register it again.
                return;
            }

            //			IsWpfDesignerAssemblyAttribute isWpfDesignerAssembly = (IsWpfDesignerAssemblyAttribute)assemblyAttributes[0];
            //			foreach (Type type in isWpfDesignerAssembly.UsePrivateReflection ? assembly.GetTypes() : assembly.GetExportedTypes()) {
            foreach (Type type in assembly.GetTypes())
            {
                object[] extensionForAttributes = type.GetCustomAttributes(typeof(ExtensionForAttribute), false);
                if (extensionForAttributes.Length == 0)
                    continue;

                foreach (ExtensionForAttribute designerFor in extensionForAttributes)
                {
                    ExtensionServer server = GetServerForExtension(type);
                    ExtensionAttribute extensionAttribute = type.GetCustomAttributes(typeof(ExtensionAttribute), false).FirstOrDefault() as ExtensionAttribute;
                    AddExtensionEntry(designerFor.DesignedItemType, new ExtensionEntry(type, server, designerFor.OverrideExtensions.ToList(), extensionAttribute != null ? extensionAttribute.Order : 0));
                }
            }
        }
        #endregion

        #region Extension Server Creation
        // extension server type => extension server instance
        private readonly Dictionary<Type, ExtensionServer> _extensionServers = new();

        private ExtensionServer GetServerForExtension(Type extensionType)
        {
            Debug.Assert(extensionType != null);

            object[] extensionServerAttributes = extensionType.GetCustomAttributes(typeof(ExtensionServerAttribute), true);
            if (extensionServerAttributes.Length != 1)
                throw new DesignerException("Extension types must have exactly one [ExtensionServer] attribute.");

            return GetExtensionServer((ExtensionServerAttribute)extensionServerAttributes[0]);
        }

        /// <summary>
        /// 获取指定扩展服务特性对应的扩展服务
        /// </summary>
        public ExtensionServer GetExtensionServer(ExtensionServerAttribute attribute)
        {
            if (attribute == null)
                throw new ArgumentNullException("attribute");

            Type extensionServerType = attribute.ExtensionServerType;

            if (_extensionServers.TryGetValue(extensionServerType, out ExtensionServer server))
                return server;

            server = (ExtensionServer)Activator.CreateInstance(extensionServerType);
            server.InitializeExtensionServer(_context);
            _extensionServers[extensionServerType] = server;
            server.ShouldApplyExtensionsInvalidated += delegate (object sender, DesignItemCollectionEventArgs e)
            {
                // 重新应用扩展
                ReapplyExtensions(e.Items, (ExtensionServer)sender);
            };
            return server;
        }
        #endregion

        #region Special extensions (CustomInstanceFactory and DefaultInitializer)
        private static readonly object[] emptyObjectArray = new object[0];

        /// <summary>
        /// 使用指定的参数创建指定类型的实例。
        /// 使用为该类型注册的CustomInstanceFactory创建实例，如果没有找到实例工厂，则使用反射创建实例。
        /// </summary>
        public object CreateInstanceWithCustomInstanceFactory(Type instanceType, object[] arguments)
        {
            if (instanceType == null)
                throw new ArgumentNullException("instanceType");
            if (arguments == null)
                arguments = emptyObjectArray;

            foreach (Type extensionType in GetExtensionTypes(instanceType))
            {
                if (typeof(CustomInstanceFactory).IsAssignableFrom(extensionType))
                {
                    CustomInstanceFactory factory = (CustomInstanceFactory)Activator.CreateInstance(extensionType);
                    return factory.CreateInstance(instanceType, arguments);
                }
            }
            return CustomInstanceFactory.DefaultInstanceFactory.CreateInstance(instanceType, arguments);
        }

        /// <summary>
        /// 对设计项应用所有DefaultInitializer扩展。
        /// </summary>
        public void ApplyDefaultInitializers(DesignItem item)
        {
            if (item == null)
                throw new ArgumentNullException("item");

            foreach (ExtensionEntry entry in GetExtensionEntries(item))
            {
                if (typeof(DefaultInitializer).IsAssignableFrom(entry.ExtensionType))
                {
                    DefaultInitializer initializer = (DefaultInitializer)Activator.CreateInstance(entry.ExtensionType);
                    initializer.InitializeDefaults(item);
                }
            }
        }

        /// <summary>
        /// 对设计项应用所有DefaultInitializer扩展。
        /// </summary>
        public void ApplyDesignItemInitializers(DesignItem item)
        {
            if (item == null)
                throw new ArgumentNullException("item");

            foreach (ExtensionEntry entry in GetExtensionEntries(item))
            {
                if (typeof(DesignItemInitializer).IsAssignableFrom(entry.ExtensionType))
                {
                    DesignItemInitializer initializer = (DesignItemInitializer)Activator.CreateInstance(entry.ExtensionType);
                    initializer.InitializeDesignItem(item);
                }
            }
        }
        #endregion
    }
}
