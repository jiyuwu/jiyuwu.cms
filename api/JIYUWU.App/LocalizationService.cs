using System.Text.Json;

namespace JIYUWU.App
{

    public class LocalizationService
    {
        private Dictionary<string, string> _localizedStrings;
        private string _currentLanguage;

        public event Action LanguageChanged;

        public LocalizationService()
        {
            _localizedStrings = new Dictionary<string, string>();
            _currentLanguage = "en"; // 默认语言
            LoadLanguage(_currentLanguage);
        }

        public string this[string key] => _localizedStrings.ContainsKey(key) ? _localizedStrings[key] : key;

        public void LoadLanguage(string languageCode)
        {
            _currentLanguage = languageCode;
            var fileName = $"Resources/Languages/{languageCode}.json";
            var fileContent = File.ReadAllText(fileName);
            _localizedStrings = JsonSerializer.Deserialize<Dictionary<string, string>>(fileContent);

            LanguageChanged?.Invoke();
        }

        public string GetString(string key)
        {
            return _localizedStrings.ContainsKey(key) ? _localizedStrings[key] : key;
        }
    }

}
