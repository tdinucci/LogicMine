using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using LogicMine.DataObject;
using LogicMine.Routing.Json;
using Microsoft.Extensions.DependencyInjection;

namespace LogicMine.Web
{
    /// <summary>
    /// A <see cref="JsonRequestRouter"/> that scans a given assembly for types involved in request routing
    /// and works out for itself how to route requests.
    /// </summary>
    public class IntelligentJsonRequestRouter : JsonRequestRouter
    {
        private readonly Assembly _serviceAssembly;
        private readonly IServiceCollection _serviceCollection;

        public IntelligentJsonRequestRouter(Assembly serviceAssembly, IServiceCollection serviceCollection,
            ITraceExporter traceExporter = null) : base(new Mine(traceExporter), traceExporter)
        {
            _serviceAssembly = serviceAssembly ?? throw new ArgumentNullException(nameof(serviceAssembly));
            _serviceCollection = serviceCollection ?? throw new ArgumentNullException(nameof(serviceCollection));
        }

        /// <summary>
        /// Dynamically finds all IDataObjectDescriptor's within the provided assembly and constructs them.
        /// It is expected that the implementations all have a default constructor
        /// </summary>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        protected override IEnumerable<IDataObjectDescriptor> GetDataObjectDescriptors()
        {
            var descriptorTypes = _serviceAssembly.GetTypes()
                .Where(t => typeof(IDataObjectDescriptor).IsAssignableFrom(t) && t.IsClass && !t.IsAbstract);

            var descriptors = new List<IDataObjectDescriptor>();
            foreach (var descriptorType in descriptorTypes)
            {
                try
                {
                    var descriptor = (IDataObjectDescriptor) Activator.CreateInstance(descriptorType);
                    descriptors.Add(descriptor);
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException(
                        $"Failed to construct instance of '{descriptorType}': {ex.Message}");
                }
            }

            return descriptors;
        }

        /// <summary>
        /// Dynamically discovers the IShaftRegistrar implementations and instantiates them.
        /// The IServiceCollection which was passed to the constructor is for injecting dependencies into the
        /// constructed registrars
        /// </summary>
        /// <returns></returns>
        protected override IEnumerable<IShaftRegistrar> GetShaftRegistrars()
        {
            var registrarTypes = _serviceAssembly.GetTypes()
                .Where(t => typeof(IShaftRegistrar).IsAssignableFrom(t) && t.IsClass && !t.IsAbstract &&
                            t.GetConstructors().Length > 0)
                .ToArray();

            // register types with the DI container that will be required to construct the registrars
            _serviceCollection.AddSingleton(DataObjectDescriptorRegistry);
            foreach (var registrarType in registrarTypes)
                _serviceCollection.AddSingleton(registrarType);

            var provider = _serviceCollection.BuildServiceProvider();

            return registrarTypes.Select(registrarType => (IShaftRegistrar) provider.GetService(registrarType));
        }

        /// <summary>
        /// Discovers the custom request types which have been defined in the provided assembly 
        /// </summary>
        /// <returns></returns>
        protected override IEnumerable<Type> GetCustomRequestTypes()
        {
            return _serviceAssembly.GetTypes()
                .Where(t => typeof(IRequest).IsAssignableFrom(t) && t.IsClass && !t.IsAbstract)
                .ToArray();
        }
    }
}