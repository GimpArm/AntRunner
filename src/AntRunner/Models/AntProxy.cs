using System;
using System.IO;
using System.Linq;
using System.Reflection;
using AntRunner.Interface;

namespace AntRunner.Models
{
    public class AntProxy : MarshalByRefObject, IDisposable
    {
        private Ant _ant;
        private Func<Ant, AntAction> _getAction;

        public string Name => _ant.Name;
        public Stream Flag => _ant.Flag;

        public AntAction Action
        {
            get => _getAction?.Invoke(_ant) ?? _ant.Action;
            set => _ant.Action = value;
        }

        public override object InitializeLifetimeService()
        {
            return null;
        }

        public void Initialize(int mapWidth, int mapHeight, ItemColor antColor, int startX, int startY)
        {
            _ant.Initialize(mapWidth, mapHeight, antColor, startX, startY);
        }

        public void Tick(GameState state)
        {
            _ant.Tick(state);
        }

        public void LoadAssembly(AssemblyLoaderData data)
        {
            try
            {
                var assembly = Assembly.Load(data.AssemblyName);
                var antType = string.IsNullOrEmpty(data.TypeString) ?
                    assembly.GetExportedTypes().FirstOrDefault(t => t.IsClass && !t.IsAbstract && t.IsSubclassOf(typeof(Ant))) :
                    assembly.GetType(data.TypeString);

                if (antType == null)
                {
                    throw new Exception("Could not find class derived from AntRunner.Interface.Ant");
                }

                AppDomain.CurrentDomain.AssemblyResolve += CreateHandler(new FileInfo(assembly.Location).DirectoryName);

                if (!(Activator.CreateInstance(antType, data.ConstructorParameters) is Ant ant))
                {
                    throw new Exception($"Could not initialize Ant class {antType.Name}");
                }

                _ant = ant;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(ex.InnerException?.Message ?? ex.Message);
            }
        }

        public void SetGetAction(Func<Ant, AntAction> getAction)
        {
            _getAction = getAction;
        }

        private static ResolveEventHandler CreateHandler(string directoryName)
        {
            var dllLookup = Directory.GetFiles(directoryName, "*.dll").ToDictionary(Path.GetFileNameWithoutExtension, y => y);
            return (sender, args) =>
            {
                var name = args.Name.Split(',')[0];
                if (dllLookup.ContainsKey(name))
                {
                    return Assembly.Load(AssemblyName.GetAssemblyName(dllLookup[name]));
                }
                return Assembly.Load(args.Name);
            };
        }

        public void Dispose()
        {
            if (_ant is IDisposable disposable)
            {
                disposable.Dispose();
            }
        }
    }
}
