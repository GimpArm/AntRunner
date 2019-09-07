using System;
using System.Collections.Generic;
using System.Text;

namespace AntRunner.Game.Interface
{
    public interface IExternalComponent
    {
        IGameEventHook GameEventHook { get; }
        void Init();
        void Stop();
    }
}
