using JIYUWU.App.Resources.Strings;
using System;
using System.Collections.Generic;
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
    public class Translator
    {
        public  string this [string key]
        {
            get=>AppResources.ResourceManager.GetString(key, Cultureinfo);
        }
        public CultureInfo Cultureinfo { get; set; }
        public static Translator Instance { get; } = new Translator();
    }
}
