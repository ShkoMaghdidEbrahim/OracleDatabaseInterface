using Oracle.ManagedDataAccess.Client;
using System;
using System.Drawing;
using System.Windows;
using System.Windows.Media;


namespace ModernStyledApplication.MVVM.View;

public partial class MainWindow {

    public MainWindow(double left, double top) {
        this.Left = left;
        this.Top = top;

        string username = Application.Current.Resources["Username"].ToString();

        InitializeComponent();

        if (username == "sys as sysdba" || username == "system")
        {
            AdminRadioButton.Foreground = System.Windows.Media.Brushes.LightGray;
            AdminRadioButton.IsEnabled = true;
        }
        else
        {
            AdminRadioButton.Foreground = System.Windows.Media.Brushes.Transparent;
            AdminRadioButton.IsEnabled = false;
        }

    }

    private void LogOutButton_Click(object sender, RoutedEventArgs e)
    {
        LoginView login = new LoginView();
        Application.Current.MainWindow = login;
        login.Show();
        this.Close();
    }
}