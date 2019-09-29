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
        public string DisplayText => "Logger";

        public IGameEventHook GameEventHook { get; set; } = new LoggerWithUIComponentViewModel();

        private bool _isActive;
        public bool IsActiv => _isActive;
        public bool IsAutoRun => false;

        private LoggerWithUIWindow _window;

        public void Start()
        {
            RunOnUIThread(() =>
            {
                _window = new LoggerWithUIWindow
                {
                    DataContext = GameEventHook
                };
                _window.Show();
                _isActive = true;
            });
            
        }

        public void Stop()
        {
            RunOnUIThread(() =>
            {
                _window?.Close();
                _window = null;
                _isActive = false;
            });
        }
    }
}
