using AntRunner.Game.Interface.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Waf.Applications;
using System.Windows.Input;

namespace AntRunner.Main.ViewModels
{
    public class MenuItemViewModel : NotifyBaseModel
    {
        #region Member - DisplayText
        private string _displayText;
        public string DisplayText
        {
            get => _displayText;
            set => SetValue(ref _displayText, value);
        }
        #endregion

        public Action ExecuteAction { get; set; }

        public ICommand MenuCommand => new DelegateCommand(() => {
            ExecuteAction?.Invoke();
        });

        public ObservableCollection<MenuItemViewModel> ChildItems { get; set; } = new ObservableCollection<MenuItemViewModel>();
    }
}
