using ModernVPN.Core;
using System;
using System.Linq;
using System.Printing;
using System.Windows;
using System.Windows.Controls;

public class PrintViewModel
{
    public PrintViewModel(DataGrid dataGrid)
    {
        Application.Current.MainWindow.MaxHeight = SystemParameters.MaximizedPrimaryScreenHeight;

        MoveWindowCommand = new RelayCommand(_ => {
            var activeWindow = Application.Current.Windows.OfType<Window>().FirstOrDefault(x => x.IsActive);

            if (activeWindow != null)
            {
                activeWindow.DragMove();
            }
        });
        ShutdownWindowCommand = new RelayCommand(_ => {
            var activeWindow = Application.Current.Windows.OfType<Window>().FirstOrDefault(x => x.IsActive);
            if (activeWindow != null)
            {
                activeWindow.Close();
            }
        });
    }

    /* Commands */
    public RelayCommand MoveWindowCommand { get; set; }
    public RelayCommand ShutdownWindowCommand { get; set; }

}
