using AntRunner.Interface;
using System;

namespace AntRunner.Game.Interface
{
    public interface IGameEventHook
    {
        void CreateGame(Guid gameID);
        void StartGame(Guid gameID);
        void StopGame(Guid gameID);
        void SetMap(byte[] mapArray);

        void SetPlayerAction(AntState antState);
    }
}
