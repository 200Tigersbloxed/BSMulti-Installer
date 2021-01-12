using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSMulti_Installer2.XML
{
    public static class Validator
    {
        public static void ValidateXML(MultiplayerInstallerConfiguration installer)
        {
            foreach (MultiplayerComponent component in installer.ComponentDefinitions)
            {
                ValidateDependencies(installer, component);
            }
            foreach (MultiplayerMod mod in installer.ModGroup)
            {
                ValidateMod(installer, mod);
            }
        }

        public static void ValidateMod(MultiplayerInstallerConfiguration installer, MultiplayerMod mod)
        {
            ComponentReference[] deps = mod.Dependencies;
            if (deps == null) return;
            foreach (var dep in deps)
            {
                if (installer.TryGetComponent(dep, out MultiplayerComponent component))
                {
                    ValidateDependencies(installer, component);
                }
                else
                    throw new ValidationException($"Component {dep.Name} ({dep.Version}) was not listed in ComponentDefinitions.");
            }
        }

        internal static void ValidateDependencies(MultiplayerInstallerConfiguration installer, MultiplayerComponent component, Dictionary<string, MultiplayerComponent> existing)
        {
            if (component == null) throw new ArgumentNullException("Component cannot be null.");
            existing.Add(MultiplayerInstallerConfiguration.GetComponentString(component), component);
            var componentReferences = component.Requires;
            if (componentReferences == null || componentReferences.Length == 0)
                return;
            foreach (var dependency in componentReferences)
            {
                if (existing.TryGetValue(MultiplayerInstallerConfiguration.GetComponentString(dependency), out _))
                {
                    throw new CircularDependencyException($"Circular dependency detected: {string.Join(" => ", existing.Values.Select(c => c.ToString()).Concat(new string[] { dependency.ToString() }))}");
                }
                if (installer.TryGetComponent(dependency.Name, dependency.Version, out MultiplayerComponent dep))
                {
                    ValidateDependencies(installer, dep, existing);
                }
                else
                    throw new ValidationException($"Component {dependency.Name} ({dependency.Version}) was not listed in ComponentDefinitions.");
            }
        }

        public static void ValidateDependencies(MultiplayerInstallerConfiguration installer, MultiplayerComponent component)
        {
            if (component == null) throw new ArgumentNullException("Component cannot be null.");
            var componentReferences = component.Requires;
            if (componentReferences == null || componentReferences.Length == 0)
                return;
            foreach (var dep in componentReferences)
            {
                if (installer.TryGetComponent(dep.Name, dep.Version, out MultiplayerComponent dependency))
                {
                    Dictionary<string, MultiplayerComponent> existing = new Dictionary<string, MultiplayerComponent>(StringComparer.OrdinalIgnoreCase);
                    ValidateDependencies(installer, dependency, existing);
                }
                else
                    throw new ValidationException($"Component {dependency.Name} ({dependency.Version}) was not listed in ComponentDefinitions.");
            }
        }
    }

    public class ValidationException : Exception
    {
        public ValidationException(string message) : base(message)
        {
        }

        public ValidationException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public ValidationException()
        {
        }
    }
    public class CircularDependencyException : ValidationException
    {
        public CircularDependencyException(string message) : base(message)
        {
        }

        public CircularDependencyException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public CircularDependencyException()
        {
        }
    }
}
