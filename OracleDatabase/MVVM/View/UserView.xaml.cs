using Oracle.ManagedDataAccess.Client;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace ModernStyledApplication.MVVM.View;

/// <summary>
///     Interaction logic for UserView.xaml
/// </summary>
public partial class UserView {
    public UserView() {
        InitializeComponent();
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
            TableTextBox.Focus();
        }
    }

    private void TableTextBox_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter)
        {
            CreateUserButton.Focus();
        }
    }

    private void CreateUserButton_Click(object sender, System.Windows.RoutedEventArgs e)
    {
        string username = UsernameTextBox.Text;
        string password = PasswordTextBox.Text;
        string tables = TableTextBox.Text;
        string query = $"BEGIN\nexecute immediate 'CREATE USER {username} IDENTIFIED BY \"{password}\"\nDEFAULT TABLESPACE users\nTEMPORARY TABLESPACE temp';\nEND;";
        OracleConnection connection = new OracleConnection(Application.Current.Resources["ConnectionString"].ToString());
        connection.Open();
        ExecuteSqlStatement(connection, query);
        if (CreateSessionCheckBox.IsChecked ?? false)
        {
            query = $"GRANT CREATE SESSION TO {username}";
            if (CreateSessionWithGrantOptionCheckbox.IsChecked ?? false)
            {
                query += " WITH ADMIN OPTION";
            }
            if(query != null)
            {
                ExecuteSqlStatement(connection, query);
            }
        }
        query = "";
        if (CreateTableCheckBox.IsChecked ?? false)
        {
            query = $"GRANT CREATE TABLE TO {username}";
            if (CreateTableWithGrantOptionCheckbox.IsChecked ?? false)
            {
                query += " WITH ADMIN OPTION";
            }
            if (query != null)
            {
                ExecuteSqlStatement(connection, query);
            }
        }
        query = "";
        if (CreateProcedureCheckBox.IsChecked ?? false)
        {
            query = $"GRANT CREATE PROCEDURE TO {username}";
            if (CreateProcedureWithGrantOptionCheckbox.IsChecked ?? false)
            {
                query += " WITH ADMIN OPTION";
            }
            if (query != null)
            {
                ExecuteSqlStatement(connection, query);
            }
        }
        query = "";
        if (CreateViewCheckBox.IsChecked ?? false)
        {
            query += $"GRANT CREATE VIEW TO {username}";
            if (CreateViewWithGrantOptionCheckbox.IsChecked ?? false)
            {
                query += " WITH ADMIN OPTION";
            }
            if (query != null)
            {
                ExecuteSqlStatement(connection, query);
            }
        }
        query = "";
        if (CreateTriggerCheckBox.IsChecked ?? false)
        {
            query = $"GRANT CREATE TRIGGER TO {username}";
            if (CreateTriggerWithGrantOptionCheckbox.IsChecked ?? false)
            {
                query += " WITH ADMIN OPTION";
            }
            if (query != null)
            {
                ExecuteSqlStatement(connection, query);
            }
        }
        query = "";
        if (CreateSequenceCheckBox.IsChecked ?? false)
        {
            query = $"GRANT CREATE SEQUENCE TO {username}";
            if (CreateSequenceWithGrantOptionCheckbox.IsChecked ?? false)
            {
                query += " WITH ADMIN OPTION";
            }
            if (query != null)
            {
                ExecuteSqlStatement(connection, query);
            }
        }
        query = "";
        if (SelectCheckBox.IsChecked ?? false)
        {
            query = $"GRANT SELECT ON {tables} TO {username}";
            if (SelectWithGrantOptionCheckbox.IsChecked ?? false)
            {
                query += " WITH GRANT OPTION";
            }
            if (query != null)
            {
                ExecuteSqlStatement(connection, query);
            }
        }
        query = "";
        if (InsertCheckBox.IsChecked ?? false)
        {
            query = $"GRANT INSERT ON {tables} TO {username}";
            if (InsertWithGrantOptionCheckbox.IsChecked ?? false)
            {
                query += " WITH GRANT OPTION";
            }
            if (query != null)
            {
                ExecuteSqlStatement(connection, query);
            }
        }
        query = "";
        if (UpdateCheckBox.IsChecked ?? false)
        {
            query = $"GRANT UPDATE ON {tables} TO {username}";
            if (UpdateWithGrantOptionCheckbox.IsChecked ?? false)
            {
                query += " WITH GRANT OPTION";
            }
            if (query != null)
            {
                ExecuteSqlStatement(connection, query);
            }
        }
        query = "";
        if (DeleteCheckBox.IsChecked ?? false)
        {
            query = $"GRANT DELETE ON {tables} TO {username}";
            if (DeleteWithGrantOptionCheckbox.IsChecked ?? false)
            {
                query += " WITH GRANT OPTION";
            }
            if (query != null)
            {
                ExecuteSqlStatement(connection, query);
            }
        }
        query = "";
        if (ExecuteCheckBox.IsChecked ?? false)
        {
            query = $"GRANT EXECUTE ON {tables} TO {username}";
            if (DeleteWithGrantOptionCheckbox.IsChecked ?? false)
            {
                query += " WITH GRANT OPTION";
            }
            if (query != null)
            {
                ExecuteSqlStatement(connection, query);
            }
        }
        query = "";
        if (GrantRoleCheckBox.IsChecked ?? false)
        {
            query = $"GRANT GRANT ROLE TO {username}";
            if (query != null)
            {
                ExecuteSqlStatement(connection, query);
            }
        }
        query = "";
        if (SetRoleCheckBox.IsChecked ?? false)
        {
            query = $"GRANT SET ROLE TO {username}";
            if (query != null)
            {
                ExecuteSqlStatement(connection, query);
            }
        }

        connection.Close();
    }

    static void ExecuteSqlStatement(OracleConnection connection, string sql)
    {
        if(sql != null)
        {
            MessageBox.Show(sql);
using (OracleCommand command = new OracleCommand(sql, connection))
        {
            try
            {
                command.ExecuteNonQuery();
                MessageBox.Show("Success");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        }
        
    }
    private void CreateSessionCheckBox_Checked(object sender, RoutedEventArgs e)
    {
        HandleCheckBoxChecked(CreateSessionCheckBox, CreateSessionWithGrantOptionCheckbox);
    }
    private void CreateTableCheckBox_Checked(object sender, RoutedEventArgs e)
    {
        HandleCheckBoxChecked(CreateTableCheckBox, CreateTableWithGrantOptionCheckbox);
    }
    private void CreateProcedureCheckBox_Checked(object sender, RoutedEventArgs e)
    {
        HandleCheckBoxChecked(CreateProcedureCheckBox, CreateProcedureWithGrantOptionCheckbox);
    }
    private void CreateViewCheckBox_Checked(object sender, RoutedEventArgs e)
    {
        HandleCheckBoxChecked(CreateViewCheckBox, CreateViewWithGrantOptionCheckbox);
    }
    private void CreateTriggerCheckBox_Checked(object sender, RoutedEventArgs e)
    {
        HandleCheckBoxChecked(CreateTriggerCheckBox, CreateTriggerWithGrantOptionCheckbox);
    }
    private void CreateSequenceCheckBox_Checked(object sender, RoutedEventArgs e)
    {
        HandleCheckBoxChecked(CreateSequenceCheckBox, CreateSequenceWithGrantOptionCheckbox);
    }
    private void SelectCheckBox_Checked(object sender, RoutedEventArgs e)
    {
        HandleCheckBoxChecked(SelectCheckBox, SelectWithGrantOptionCheckbox);
    }
    private void InsertCheckBox_Checked(object sender, RoutedEventArgs e)
    {
        HandleCheckBoxChecked(InsertCheckBox, InsertWithGrantOptionCheckbox);
    }
    private void UpdateCheckBox_Checked(object sender, RoutedEventArgs e)
    {
        HandleCheckBoxChecked(UpdateCheckBox, UpdateWithGrantOptionCheckbox);
    }
    private void DeleteCheckBox_Checked(object sender, RoutedEventArgs e)
    {
        HandleCheckBoxChecked(DeleteCheckBox, DeleteWithGrantOptionCheckbox);
    }
    private void ExecuteCheckBox_Checked(object sender, RoutedEventArgs e)
    {
        HandleCheckBoxChecked(ExecuteCheckBox, ExecuteWithGrantOptionCheckbox);
    }
    private void HandleCheckBoxChecked(CheckBox sourceCheckBox, CheckBox grantOptionCheckBox)
    {
        if (sourceCheckBox.IsChecked ?? false)
        {
            grantOptionCheckBox.IsEnabled = true;
            grantOptionCheckBox.Visibility = Visibility.Visible;

        }
        else
        {
            grantOptionCheckBox.IsChecked = false;
            grantOptionCheckBox.IsEnabled = false;
            grantOptionCheckBox.Visibility = Visibility.Collapsed;
        }
    }
}