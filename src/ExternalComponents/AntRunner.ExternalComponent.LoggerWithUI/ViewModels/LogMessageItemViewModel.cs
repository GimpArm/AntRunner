using AntRunner.Game.Interface.Models;
using System;

namespace AntRunner.ExternalComponent.LoggerWithUI.ViewModels
{
    public class LogMessageItemViewModel : NotifyBaseModel
    {
        public DateTime Time { get; set; } = DateTime.UtcNow;
        public string MessageText { get; set; }
    }
}
