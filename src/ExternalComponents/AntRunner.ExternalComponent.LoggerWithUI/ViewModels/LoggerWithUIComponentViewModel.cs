using AntRunner.Game.Interface;
using AntRunner.Game.Interface.Models;
using AntRunner.Interface;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntRunner.ExternalComponent.LoggerWithUI.ViewModels
{
    public class LoggerWithUIComponentViewModel : NotifyBaseModel, IGameEventHook
    {
        public ObservableCollection<LogMessageItemViewModel> LogMessages { get; set; } = new ObservableCollection<LogMessageItemViewModel>();

        private JsonSerializerSettings _jsonSettings = new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            Converters = new List<JsonConverter> { new StringEnumConverter(new CamelCaseNamingStrategy(), true) },
            NullValueHandling = NullValueHandling.Ignore
        };

        public void CreateGame(Guid gameID)
        {
            AddMessage($"Create Game with ID: {gameID}");
        }

        public void SetMap(byte[] mapArray)
        {
            AddMessage($"Set new Map");
        }

        public void StartGame(Guid gameID)
        {
            AddMessage($"Start Game with ID: {gameID}");
        }

        public void StopGame(Guid gameID)
        {
            AddMessage($"Stop Game with ID: {gameID}");
        }

        public void SetPlayerAction(AntState antState)
        {
            string message = JsonConvert.SerializeObject(antState, _jsonSettings);

            AddMessage($"New AntState: {message}");
        }

        private void AddMessage(string message)
        {
            RunOnUIThread(() =>
            {
                LogMessages.Insert(0, new LogMessageItemViewModel
                {
                    MessageText = message
                });

                RemoveLastActions();
            });   
        }

        private void RemoveLastActions()
        {
            if (LogMessages.Count > 50)
            {
                LogMessages.RemoveAt(50);
            }
        }
    }
}
