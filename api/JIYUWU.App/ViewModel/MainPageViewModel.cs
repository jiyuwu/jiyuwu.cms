using System.ComponentModel;
using System.Runtime.CompilerServices;
namespace JIYUWU.App.ViewModel
{
    public class MainPageViewModel : INotifyPropertyChanged
    {
        private readonly LocalizationService _localizationService;

        public MainPageViewModel(LocalizationService localizationService)
        {
            _localizationService = localizationService;
            _localizationService.LanguageChanged += OnLanguageChanged;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public string GreetingText => _localizationService.GetString("Greeting");
        public string FarewellText => _localizationService.GetString("Farewell");

        private void OnLanguageChanged()
        {
            OnPropertyChanged(nameof(GreetingText));
            OnPropertyChanged(nameof(FarewellText));
        }

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

}
