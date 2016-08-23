using Sc.Scheduler.Contracts;

    using System;
    using System.Collections.Concurrent;
    using System.Linq;


namespace Sc.Scheduler
{

    public class DefaultServiceProvider : ISchedulerContainer
    {
        private readonly object _syncLock = new object();
        private readonly ConcurrentDictionary<Type, object> _factories = new ConcurrentDictionary<Type, object>();
        private readonly ConcurrentDictionary<Type, Type> _registrations = new ConcurrentDictionary<Type, Type>();
        private readonly ConcurrentDictionary<Type, object> _instances = new ConcurrentDictionary<Type, object>();

        private bool ServiceIsRegistered(Type serviceType)
        {
            return _factories.ContainsKey(serviceType) || _registrations.ContainsKey(serviceType);
        }

        public virtual ISchedulerServiceRegister Register<TService>(Func<ISchedulerServiceProvider, TService> serviceCreator) where TService : class
        {
            if (serviceCreator==null) throw new ArgumentNullException("serviceCreator");

            lock (_syncLock)
            {
                var serviceType = typeof (TService);
                if (ServiceIsRegistered(serviceType))
                    return this;
                _factories.TryAdd(serviceType, serviceCreator);
                return this;
            }
        }

        public virtual TService Resolve<TService>() where TService : class
        {
            var serviceType = typeof (TService);
            object service;
            if (_instances.TryGetValue(serviceType, out service))
                return (TService) service;
            lock (_syncLock)
            {
                if (_instances.TryGetValue(serviceType, out service))
                    return (TService) service;

                if (_registrations.ContainsKey(serviceType))
                {
                    var implementationType = _registrations[serviceType];
                    service = CreateServiceInstance(implementationType);
                    _instances.TryAdd(serviceType, service);
                }
                else if (_factories.ContainsKey(serviceType))
                {
                    service = ((Func<ISchedulerServiceProvider, TService>) _factories[serviceType])(this);
                    _instances.TryAdd(serviceType, service);
                }
                else
                {
                    throw new InvalidOperationException(string.Format("No service of type {0} has been registered", serviceType.Name));
                }
                return (TService) service;
            }
        }

        public object Resolve(Type serviceType)
        {
            if (serviceType==null) throw new ArgumentNullException("serviceType");

            return typeof (DefaultServiceProvider)
                .GetMethod("Resolve", new Type[0])
                .MakeGenericMethod(serviceType)
                .Invoke(this, new object[0]);
        }

        private object CreateServiceInstance(Type implementationType)
        {
            var constructors = implementationType.GetConstructors();

            var parameters = constructors[0]
                .GetParameters()
                .Select(parameterInfo => Resolve(parameterInfo.ParameterType))
                .ToArray();

            return constructors[0].Invoke(parameters);
        }


        public ISchedulerServiceRegister Register<TService, TImplementation>()
            where TService : class
            where TImplementation : class, TService
        {
            lock (_syncLock)
            {
                var serviceType = typeof (TService);

                var implementationType = typeof (TImplementation);

                if (ServiceIsRegistered(serviceType))
                    return this;

                if (!serviceType.IsAssignableFrom(implementationType))
                {
                    throw new InvalidOperationException(string.Format("Component {0} does not implement service interface {1}", implementationType.Name, serviceType.Name));
                }

                var constructors = implementationType.GetConstructors();
                if (constructors.Length != 1)
                {
                    throw new InvalidOperationException(string.Format("Service must have one and one only constructor. Service {0} has {1}.", implementationType.Name, constructors.Length));
                }

                _registrations.TryAdd(serviceType, implementationType);

                return this;
            }
        }
    }
}

