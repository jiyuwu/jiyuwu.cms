using JIYUWU.App.ViewModel;

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

        }

        private void OnSwitchToEnglishClicked(object sender, EventArgs e)
        {

        }
    }

}
