using AntRunner.Game.Interface.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntRunner.ExternalComponent.LoggerWithUI.ViewModels
{
    public class LogMessageItemViewModel : NotifyBaseModel
    {
        public DateTime Time { get; set; } = DateTime.UtcNow;
        public string MessageText { get; set; }
    }
}
