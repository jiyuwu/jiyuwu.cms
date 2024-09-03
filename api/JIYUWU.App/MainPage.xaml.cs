using JIYUWU.App.ViewModel;

namespace JIYUWU.App
{
    public partial class MainPage : ContentPage
    {
        private readonly LocalizationService _localizationService;
        public MainPage(MainPageViewModel viewModel, LocalizationService localizationService)
        {
            InitializeComponent();
            BindingContext = viewModel;
            _localizationService = localizationService;
        }

        private void OnSwitchToEnglishClicked(object sender, EventArgs e)
        {
            _localizationService.LoadLanguage("en");
        }

        private void OnSwitchToChineseClicked(object sender, EventArgs e)
        {
            _localizationService.LoadLanguage("zh");
        }
    }

}
