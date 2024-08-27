using Hjmos.Lcdp.VisualEditor.Core.ItemExtensions;
using System;
using System.Collections.Generic;

namespace Hjmos.Lcdp.VisualEditor.Core.Services
{
    /// <summary>
    /// <see cref="ServiceContainer"/>是一个管理服务列表的内置服务。  
    /// 您只能向它添加服务，不支持删除或替换服务，因为许多设计器依赖于保持其服务可用。  
    /// </summary>
    public sealed class ServiceContainer : IServiceProvider
    {
        private readonly Dictionary<Type, object> _services = new();
        private readonly Dictionary<Type, Delegate> _waitingSubscribers = new();

        /// <summary>
        /// 获取所有注册的服务
        /// </summary>
        public IEnumerable<object> AllServices => _services.Values;

        /// <summary>
        /// 向容器添加新服务。
        /// </summary>
        /// <param name="serviceInterface">服务接口的类型，用作服务的key</param>
        /// <param name="serviceInstance">实现该接口的服务实例</param>
        public void AddService(Type serviceInterface, object serviceInstance)
        {
            if (serviceInterface == null)
                throw new ArgumentNullException("serviceInterface");
            if (serviceInstance == null)
                throw new ArgumentNullException("serviceInstance");

            _services.Add(serviceInterface, serviceInstance);

            if (_waitingSubscribers.TryGetValue(serviceInterface, out Delegate subscriber))
            {
                _waitingSubscribers.Remove(serviceInterface);
                subscriber.DynamicInvoke(serviceInstance);
            }
        }

        /// <summary>
        /// Adds a new service to the container or Replaces a existing one.
        /// </summary>
        /// <param name="serviceInterface">
        /// The type of the service interface to use as a key for the service.
        /// </param>
        /// <param name="serviceInstance">
        /// The service instance implementing that interface.
        /// </param>
        public void AddOrReplaceService(Type serviceInterface, object serviceInstance)
        {
            if (serviceInterface == null)
                throw new ArgumentNullException("serviceInterface");
            if (serviceInstance == null)
                throw new ArgumentNullException("serviceInstance");

            if (_services.ContainsKey(serviceInterface))
                _services.Remove(serviceInterface);

            _services.Add(serviceInterface, serviceInstance);

            if (_waitingSubscribers.TryGetValue(serviceInterface, out Delegate subscriber))
            {
                _waitingSubscribers.Remove(serviceInterface);
                subscriber.DynamicInvoke(serviceInstance);
            }
        }

        /// <summary>
        /// 获取指定类型的服务对象。
        /// 当服务不可用时返回null。
        /// </summary>
        public object GetService(Type serviceType)
        {
            _services.TryGetValue(serviceType, out object instance);
            return instance;
        }

        /// <summary>
        /// Gets the service object of the type T.
        /// Returns null when the service is not available.
        /// </summary>
        public T GetService<T>() where T : class => (T)GetService(typeof(T));

        /// <summary>
        /// 订阅T类型的服务。
        /// serviceAvailableAction将在服务可用后被调用。 如果服务已经可用，则会立即调用操作。 
        /// </summary>
        public void RunWhenAvailable<T>(Action<T> serviceAvailableAction) where T : class
        {
            T service = GetService<T>();
            if (service != null)
            {
                serviceAvailableAction(service);
            }
            else
            {
                Type serviceInterface = typeof(T);
                if (_waitingSubscribers.TryGetValue(serviceInterface, out Delegate existingSubscriber))
                {
                    _waitingSubscribers[serviceInterface] = Delegate.Combine(existingSubscriber, serviceAvailableAction);
                }
                else
                {
                    _waitingSubscribers[serviceInterface] = serviceAvailableAction;
                }
            }
        }

        /// <summary>
        /// 获取所需的服务。
        /// 不会返回null;找不到服务时抛出ServiceRequiredException。
        /// </summary>
        public T GetRequiredService<T>() where T : class
        {
            T service = (T)GetService(typeof(T));
            if (service == null)
            {
                throw new ServiceRequiredException(typeof(T));
            }
            return service;
        }

        /// <summary>
        /// 获取<see cref="ISelectionService"/>。
        /// 如果未找到服务，则抛出异常。  
        /// </summary>
        public ISelectionService Selection => GetRequiredService<ISelectionService>();

        /// <summary>
        /// 获取<see cref="IToolService"/>
        /// 如果未找到服务，则抛出异常。
        /// </summary>
        public IToolService Tool => GetRequiredService<IToolService>();

        /// <summary>
        /// 获取 <see cref="IComponentService"/>.
        /// 如果未找到服务，则抛出异常。
        /// </summary>
        public IComponentService Component => GetRequiredService<IComponentService>();

        /// <summary>
        /// Gets the <see cref="ViewService"/>.
        /// Throws an exception if the service is not found.
        /// </summary>
        public ViewService View => GetRequiredService<ViewService>();

        /// <summary>
        /// Gets the <see cref="ExtensionManager"/>.
        /// Throws an exception if the service is not found.
        /// </summary>
        public ExtensionManager ExtensionManager => GetRequiredService<ExtensionManager>();

        /// <summary>
        /// Gets the <see cref="IDesignPanel"/>.
        /// Throws an exception if the service is not found.
        /// </summary>
        public IDesignPanel DesignPanel => GetRequiredService<IDesignPanel>();
    }
}
