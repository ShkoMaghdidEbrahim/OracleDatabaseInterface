using System.Diagnostics;
using System.Linq;
using System.Windows;
using ModernStyledApplication.MVVM.View;
using ModernVPN.Core;

namespace ModernStyledApplication.MVVM.ViewModel;

internal class MainViewModel : ObservableObject {
    private object _currentView = null!;

    public MainViewModel() {
        HomeVm = new HomeView();
        UserVm = new UserViewModel();
        CurrentView = HomeVm;

        Debug.Assert(Application.Current.MainWindow != null, "Application.Current.MainWindow != null");
        Application.Current.MainWindow.MaxHeight = SystemParameters.MaximizedPrimaryScreenHeight;

        MoveWindowCommand = new RelayCommand(_ => {
            var activeWindow = Application.Current.Windows.OfType<Window>().FirstOrDefault(x => x.IsActive);

            if (activeWindow != null)
            {
                activeWindow.DragMove();
            }
        });
        ShutdownWindowCommand = new RelayCommand(_ => { Application.Current.Shutdown(); });
        MaximizeWindowCommand = new RelayCommand(_ => { Application.Current.MainWindow.WindowState = Application.Current.MainWindow.WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized; });
        MinimizeWindowCommand = new RelayCommand(_ => { Application.Current.MainWindow.WindowState = WindowState.Minimized;});

        ShowHomeView = new RelayCommand(_ => { CurrentView = HomeVm; });
        ShowUserView = new RelayCommand(_ => { CurrentView = UserVm; });
    }

    /* Commands */
    public RelayCommand MoveWindowCommand { get; set; }
    public RelayCommand ShutdownWindowCommand { get; set; }
    public RelayCommand MaximizeWindowCommand { get; set; }
    public RelayCommand MinimizeWindowCommand { get; set; }
    public RelayCommand ShowHomeView { get; set; }
    public RelayCommand ShowSettingsView { get; set; }
    public RelayCommand ShowUserView { get; set; }

    public object CurrentView {
        get => _currentView;
        private set {
            _currentView = value;
            OnPropertyChanged();
        }
    }

    private HomeView HomeVm { get; set; }
    private UserViewModel UserVm { get; set; }
}