using System.Diagnostics;
using System.Windows;
using ModernVPN.Core;

namespace ModernStyledApplication.MVVM.ViewModel;

internal class LoginViewModel : ObservableObject {
    public LoginViewModel() {
        Application.Current.MainWindow.MaxHeight = SystemParameters.MaximizedPrimaryScreenHeight;

        MoveWindowCommand = new RelayCommand(_ => { Application.Current.MainWindow.DragMove(); });
        ShutdownWindowCommand = new RelayCommand(_ => { Application.Current.Shutdown(); });
        MaximizeWindowCommand = new RelayCommand(_ => { Application.Current.MainWindow.WindowState = Application.Current.MainWindow.WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized; });
        MinimizeWindowCommand = new RelayCommand(_ => {
            Application.Current.MainWindow.WindowState = WindowState.Minimized;
        });
    }

    /* Commands */
    public RelayCommand MoveWindowCommand { get; set; }
    public RelayCommand ShutdownWindowCommand { get; set; }
    public RelayCommand MaximizeWindowCommand { get; set; }
    public RelayCommand MinimizeWindowCommand { get; set; }
}