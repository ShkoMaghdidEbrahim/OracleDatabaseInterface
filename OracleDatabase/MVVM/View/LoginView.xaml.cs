using System;
using System.Data;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Oracle.ManagedDataAccess.Client;

namespace ModernStyledApplication.MVVM.View;

public partial class LoginView {

    public LoginView() {
        InitializeComponent();
        UsernameTextBox.Focus();
        CenterWindowOnScreen(this);
    }

    private async void LoginButton_Click(object sender, RoutedEventArgs e) {
        string username = UsernameTextBox.Text;
        string password = PasswordTextBox.Text;

        var connectionString = $"User Id={username};Password={password};" + "Data Source=(DESCRIPTION=" +
                               "(ADDRESS=(PROTOCOL=TCP)(HOST=localhost)(PORT=1521))" +
                               "(CONNECT_DATA=" +
                               "(SERVER=DEDICATED)" +
                               "(SERVICE_NAME=orclpdb)));" +
                               "Connection Timeout = 120";
        
        if (username == "sys as sysdba")
        {
            connectionString += ";DBA Privilege=SYSDBA";
        }
        else
        {
              connectionString += ";";
        }
        
        Application.Current.Resources["ConnectionString"] = connectionString;
        Application.Current.Resources["Username"] = username;
        Application.Current.Resources["Password"] = password;
        try {
            using (OracleConnection connection = new OracleConnection(connectionString))
            {
                UpdateStatus("Connecting...", Colors.White);
                await Task.Run(() =>
                {
                    connection.Open();
                });

                if (connection.State == ConnectionState.Open)
                {
                    UpdateStatus("Connected successfully.", Colors.Green);
                    await Task.Delay(50);

                    double left = this.Left;
                    double top = this.Top;

                    var mainWindow = new MainWindow(left, top);
                    Application.Current.MainWindow = mainWindow;
                    mainWindow.Show();
                    Close();
                }
                else
                {
                    UsernameTextBox.Text = "";
                    PasswordTextBox.Text = "";
                    UpdateStatus("Failed to connect.", Colors.Red);
                }
            }
        }
        catch (OracleException ex) {
            UsernameTextBox.Text = "";
            PasswordTextBox.Text = "";
            UpdateStatus($"Error: {ex.Message}", Colors.Red);
            MessageBox.Show($"Error: {ex.Message}");
        }
        catch (Exception ex) {
            UsernameTextBox.Text = "";
            PasswordTextBox.Text = "";
            UpdateStatus($"Error: {ex.Message}", Colors.Red);
            MessageBox.Show($"Error: {ex.Message}");
        }
    }

    private void UsernameTextBox_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter)
        {
            PasswordTextBox.Focus();
        }
    }

    private void PasswordTextBox_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter)
        {
            LoginButton.Focus();
        }
    }

    private void UpdateStatus(string message, Color color)
    {
        StatusTextBlock.Text = message;
        StatusTextBlock.Foreground = new SolidColorBrush(color);

        var clearTextTask = Task.Delay(TimeSpan.FromSeconds(2));
        clearTextTask.ContinueWith(_ =>
        {
            StatusTextBlock.Text = string.Empty;
        }, TaskScheduler.FromCurrentSynchronizationContext());
    }

    private void CenterWindowOnScreen(Window window)
    {
        double screenWidth = SystemParameters.PrimaryScreenWidth;
        double screenHeight = SystemParameters.PrimaryScreenHeight;
        double windowWidth = window.Width;
        double windowHeight = window.Height;

        window.Left = (screenWidth - windowWidth) / 2;
        window.Top = (screenHeight - windowHeight) / 2;
    }
}