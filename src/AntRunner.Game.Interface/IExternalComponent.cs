using System;
using System.Collections.Generic;
using System.Text;

namespace AntRunner.Game.Interface
{
    public interface IExternalComponent
    {
        string DisplayText { get; }
        IGameEventHook GameEventHook { get; }
        bool IsActiv { get; }
        bool IsAutoRun { get; }
        void Start();
        void Stop();
    }
}
