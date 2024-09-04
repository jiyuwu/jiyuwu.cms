using JIYUWU.App.Resources.Strings;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JIYUWU.App.Common
{
    public class TranslateHelper : IMarkupExtension
    {
        public string Key { get; set; }
        public object ProvideValue(IServiceProvider serviceProvider)
        {
            var binding=new Binding
            {
                Mode=BindingMode.OneWay,
                Path = $"[{Key}]",
                Source =Translator.Instance,
            };
            return binding;
        }
    }
    public class Translator : INotifyPropertyChanged
    {
        private CultureInfo _cultureInfo;

        public string this[string key]
        {
            get => AppResources.ResourceManager.GetString(key, _cultureInfo);
        }

        public CultureInfo Cultureinfo
        {
            get => _cultureInfo;
            set
            {
                if (_cultureInfo != value)
                {
                    _cultureInfo = value;
                    OnPropertyChanged();
                }
            }
        }

        public static Translator Instance { get; } = new Translator();

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged()
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(null));
        }

        public Translator()
        {
            _cultureInfo = new CultureInfo(""); // 默认语言
        }
    }
}
