using AntRunner.ExternalComponent.LoggerWithUI.ViewModels;
using AntRunner.Game.Interface;
using AntRunner.Game.Interface.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace AntRunner.ExternalComponent.LoggerWithUI
{
    public class LoggerWithUIComponent : NotifyBaseModel, IExternalComponent
    {
        public IGameEventHook GameEventHook { get; set; } = new LoggerWithUIComponentViewModel();

        private LoggerWithUIWindow _window;

        public void Init()
        {
            RunOnUIThread(() =>
            {
                _window = new LoggerWithUIWindow();
                _window.DataContext = GameEventHook;
                _window.Show();
            });
        }

        public void Stop()
        {
            RunOnUIThread(() =>
            {
                _window?.Close();
                _window = null;
            });
        }
    }
}
