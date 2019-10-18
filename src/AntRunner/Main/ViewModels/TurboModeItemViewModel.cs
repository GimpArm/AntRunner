using AntRunner.Game.Interface.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntRunner.Main.ViewModels
{
    public class TurboModeItemViewModel : NotifyBaseModel
    {
        public int Value { get; set; }
        public string Text => $"{Value}ms";
    }
}
