using System;
using System.IO;
using System.Linq;
using System.Reflection;
using AntRunner.Interface;

namespace AntRunner.Models
{
    public class AntProxy : MarshalByRefObject
    {
        private Assembly _assembly;
        private Ant _ant;

        public string Name => _ant.Name;
        public Stream Flag => _ant.Flag;
        
        public AntAction Action
        {
            get => _ant.Action;
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

        public void LoadAssembly(string assemblyPath)
        {
            try
            {
                _assembly = Assembly.Load(AssemblyName.GetAssemblyName(assemblyPath));

                var antType = _assembly.GetTypes().FirstOrDefault(t => t.IsClass && !t.IsAbstract && t.IsSubclassOf(typeof(Ant)));
                if (antType == null)
                {
                    throw new Exception("Could not find class derived from AntRunner.Interface.Ant");
                }

                if (!(Activator.CreateInstance(antType) is Ant ant))
                {
                    throw new Exception($"Could not initialize Ant class {antType.Name}");
                }

                _ant = ant;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(ex.Message);
            }
        }
    }
}
