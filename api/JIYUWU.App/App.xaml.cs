using System.Globalization;

namespace JIYUWU.App
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            // 设置全局的默认语言
            var culture = new CultureInfo("zh-CN");
            CultureInfo.DefaultThreadCurrentCulture = culture;
            CultureInfo.DefaultThreadCurrentUICulture = culture;
            MainPage = new AppShell();
        }
    }
}
