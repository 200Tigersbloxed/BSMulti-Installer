using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSMulti_Installer2.XML
{
    public static class Extensions
    {
        public static IEnumerable<MultiplayerComponent> GetComponents(this MultiplayerMod mod, MultiplayerInstallerConfiguration installer)
        {
            Dictionary<string, MultiplayerComponent> components = new Dictionary<string, MultiplayerComponent>();
            var compAry = mod.Dependencies;
            if (compAry == null || compAry.Length == 0)
                return Array.Empty<MultiplayerComponent>();
            for(int i = 0; i < compAry.Length; i++)
            {
                ComponentReference c = compAry[i];
                string cId = MultiplayerInstallerConfiguration.GetComponentString(c);
                if (!components.ContainsKey(cId))
                {
                    if (installer.TryGetComponent(c, out MultiplayerComponent mpComp))
                    {
                        components.Add(cId, mpComp);
                        mpComp.GetComponents(installer, components);
                    }
                    else
                        throw new InvalidOperationException($"Multiplayer mod {mod.Name} ({mod.Version}) references a component that doesn't exist: {c.Name} ({c.Version})");
                }
            }
            return components.Values.ToArray();
        }
        public static IEnumerable<MultiplayerComponent> GetOptionalComponents(this MultiplayerMod mod, MultiplayerInstallerConfiguration installer)
        {
            Dictionary<string, MultiplayerComponent> components = new Dictionary<string, MultiplayerComponent>();
            var compAry = mod.OptionalComponents;
            if (compAry == null || compAry.Length == 0)
                return Array.Empty<MultiplayerComponent>();
            for (int i = 0; i < compAry.Length; i++)
            {
                ComponentReference c = compAry[i];
                string cId = MultiplayerInstallerConfiguration.GetComponentString(c);
                if (!components.ContainsKey(cId))
                {
                    if (installer.TryGetComponent(c, out MultiplayerComponent mpComp))
                    {
                        components.Add(cId, mpComp);
                        mpComp.GetComponents(installer, components);
                    }
                    else
                        throw new InvalidOperationException($"Multiplayer mod {mod.Name} ({mod.Version}) references a component that doesn't exist: {c.Name} ({c.Version})");
                }
            }
            return components.Values.ToArray();
        }

        public static IEnumerable<MultiplayerComponent> GetComponents(this MultiplayerComponent comp, MultiplayerInstallerConfiguration installer)
        {
            Dictionary<string, MultiplayerComponent> components = new Dictionary<string, MultiplayerComponent>();
            ComponentReference[] componentReferences = comp?.Requires;
            if (componentReferences == null || componentReferences.Length == 0)
                return Array.Empty<MultiplayerComponent>();
            for (int i = 0; i < componentReferences.Length; i++)
            {
                ComponentReference c = componentReferences[i];
                string cId = MultiplayerInstallerConfiguration.GetComponentString(c);
                if (!components.ContainsKey(cId))
                {
                    if (installer.TryGetComponent(c, out MultiplayerComponent mpComp))
                    {
                        components.Add(cId, mpComp);
                        mpComp.GetComponents(installer, components);
                    }
                    else
                        throw new InvalidOperationException($"Component {comp.Name} ({comp.Version}) references a component that doesn't exist: {c.Name} ({c.Version})");
                }
            }
            return components.Values.ToArray();
        }
        private static void GetComponents(this MultiplayerComponent comp, MultiplayerInstallerConfiguration installer, Dictionary<string, MultiplayerComponent> existing)
        {
            ComponentReference[] componentReferences = comp?.Requires;
            if (componentReferences == null || componentReferences.Length == 0) 
                return;
            for(int i = 0; i < componentReferences.Length; i++)
            {
                ComponentReference c = componentReferences[i];
                string cId = MultiplayerInstallerConfiguration.GetComponentString(c);
                if (!existing.ContainsKey(cId))
                {
                    if (installer.TryGetComponent(c, out MultiplayerComponent mpComp))
                    {
                        existing.Add(cId, mpComp);
                        mpComp.GetComponents(installer, existing);
                    }
                    else
                        throw new InvalidOperationException($"Component {comp.Name} ({comp.Version}) references a component that doesn't exist: {c.Name} ({c.Version})");
                }
            }
        }

        public static IEnumerable<T> TSort<T>(this IEnumerable<T> source, Func<T, IEnumerable<T>> dependencies, bool throwOnCycle = false)
        {
            var sorted = new List<T>();
            var visited = new HashSet<T>();

            foreach (var item in source)
                Visit(item, visited, sorted, dependencies, throwOnCycle);

            return sorted;
        }

        private static void Visit<T>(T item, HashSet<T> visited, List<T> sorted, Func<T, IEnumerable<T>> dependencies, bool throwOnCycle)
        {
            if (!visited.Contains(item))
            {
                visited.Add(item);

                foreach (var dep in dependencies(item))
                    Visit(dep, visited, sorted, dependencies, throwOnCycle);

                sorted.Add(item);
            }
            else
            {
                if (throwOnCycle && !sorted.Contains(item))
                    throw new Exception("Cyclic dependency found");
            }
        }
    }
}
