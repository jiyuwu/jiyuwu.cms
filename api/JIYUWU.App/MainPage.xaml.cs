using JIYUWU.App.ViewModel;
using JIYUWU.App.Common;
using System.Globalization;

namespace JIYUWU.App
{
    public partial class MainPage : ContentPage
    {        public MainPage(MainPageViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }

        private void OnSwitchToChineseClicked(object sender, EventArgs e)
        {
            Translator.Instance.Cultureinfo = new CultureInfo("zh-CN");
            Translator.Instance.OnPropertyChanged();
        }

        private void OnSwitchToEnglishClicked(object sender, EventArgs e)
        {
            Translator.Instance.Cultureinfo = new CultureInfo("");
            Translator.Instance.OnPropertyChanged();
        }
    }

}
