using Prism.Mvvm;

namespace Hjmos.Lcdp.MessageCenter.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        private string _title = "MessageCenter";
        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }

        public MainWindowViewModel()
        {

        }
    }
}
