using System;
using System.IO;
using System.Reflection;
using AntRunner.Interface;

namespace AntRunner.Models
{
    public class AntProxy : MarshalByRefObject, IDisposable
    {
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
        
        public void LoadAssembly(IWrapperLoader loader, string filePath)
        {
            try
            {
                _ant = loader.LoadAnt(filePath);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(ex.Message);
            }
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
