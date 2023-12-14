using System;
using System.Buffers;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using Oracle.ManagedDataAccess.Client;
using OracleDatabase.MVVM.View;

namespace ModernStyledApplication.MVVM.View;

public partial class HomeView {
    public static string connectionString = Application.Current.Resources["ConnectionString"].ToString();
    public static DataTable dataTable = new();
    private readonly OracleConnection conn = new(connectionString);

    public string CellColumnName;
    public string CellColumnValue;
    public DataSet dataSet = new();
    public string TableName;
    string? ColumnName = "";
    string query = "";

    public HomeView() {
        conn.Open();
        InitializeComponent();
        Dispatcher.BeginInvoke(new Action(() => TableNameTextBox.Focus()), System.Windows.Threading.DispatcherPriority.Render);
        TableNameTextBox.Focus();
    }

    public ObservableCollection<string?> TableNames { get; set; }

    private void LoadTable() {
        try
        {

            bool isStartCommand = false;
            if (AddCheckbox.IsChecked ?? false)
            {
                ColumnName += ColumnNameTextBox.Text != "" ? ", " + ColumnNameTextBox.Text : "";
            }
            else
            {
                if (ColumnNameTextBox.Text != "")
                {
                    ColumnName = ColumnNameTextBox.Text;
                }
                else
                {
                    ColumnName = "*";
                    isStartCommand = true;
                }
            }

            var columnNames = ColumnNamesComboBox.SelectedItem;
            
            if (ColumnNamesComboBox.SelectedItem == null || columnNames == "")
            {
                FilterValueTextBox.Text = "";
                query = $"SELECT {ColumnName} FROM {TableName}";

            }
            else if (checkDataType(columnNames.ToString()) == "System.DateTime")
            {
                var date = Convert.ToDateTime(FilterValueTextBox.Text).ToString("dd-MMM-yyyy");
                if(AndCheckbox.IsChecked ?? false)
                {
                    query += $" AND {ColumnNamesComboBox.SelectedItem} = '{date}'";
                }
                else if (OrCheckbox.IsChecked ?? false)
                {
                    query += $" OR {ColumnNamesComboBox.SelectedItem} = '{date}'";
                }
                else
                {
                    query = $"SELECT {ColumnName} FROM {TableName} WHERE {ColumnNamesComboBox.SelectedItem} = '{date}'";
                }
                
            }
            else if (checkDataType(columnNames.ToString()) == "System.String")
            {
                if (AndCheckbox.IsChecked ?? false)
                {
                    query += $" AND UPPER({ColumnNamesComboBox.SelectedItem}) LIKE UPPER('{FilterValueTextBox.Text}')";
                }
                else if (OrCheckbox.IsChecked ?? false)
                {
                    query += $" OR UPPER({ColumnNamesComboBox.SelectedItem}) LIKE UPPER('{FilterValueTextBox.Text}')";
                }
                else
                {
                    query = $"SELECT {ColumnName} FROM {TableName} WHERE UPPER({ColumnNamesComboBox.SelectedItem}) LIKE UPPER('{FilterValueTextBox.Text}')";
                }            }
            else
            {
                string filterValue = FilterValueTextBox.Text.Replace(" ", "");
                if (AndCheckbox.IsChecked ?? false)
                {
                    if (!new Regex(@"^\d+").IsMatch(filterValue))
                    {
                        query += $" AND {ColumnNamesComboBox.SelectedItem} {FilterValueTextBox.Text}";
                    }
                    else
                    {
                        query += $" AND {ColumnNamesComboBox.SelectedItem} = {FilterValueTextBox.Text}";
                    }
                }
                else if (OrCheckbox.IsChecked ?? false)
                {
                    if (!new Regex(@"^\d+").IsMatch(filterValue))
                    {
                        query += $" OR {ColumnNamesComboBox.SelectedItem} {FilterValueTextBox.Text}";
                    }
                    else
                    {
                        query += $" OR {ColumnNamesComboBox.SelectedItem} = {FilterValueTextBox.Text}";
                    }
                   
                }
                else
                {
                    if (!new Regex(@"^\d+").IsMatch(filterValue))
                    {
                        query = $"SELECT {ColumnName} FROM {TableName} WHERE {ColumnNamesComboBox.SelectedItem} {FilterValueTextBox.Text}";
                    }
                    else
                    {
                        query = $"SELECT {ColumnName} FROM {TableName} WHERE {ColumnNamesComboBox.SelectedItem} = {FilterValueTextBox.Text}";

                    }
                }
            }
            using (var cmd = new OracleCommand(query, conn))
                {
                    using (var dataAdapter = new OracleDataAdapter())
                    {
                        dataSet = new DataSet();
                        dataAdapter.SelectCommand = cmd;
                        dataAdapter.Fill(dataSet);
                        TableData.ItemsSource = dataTable.DefaultView;
                        dataTable = dataSet.Tables[0];
                        TableData.ItemsSource = dataTable.DefaultView;
                    if(isStartCommand)
                    {
FillComboBoxWithColumnNames();
                    }
                        
                        UpdateStatus("Table Loaded Succesfully!", Colors.Green);
                    }
                    
                }
        }
        catch (OracleException ex)
        {
            TableNameTextBox.Text = "";
            ColumnNameTextBox.Text = "";
            ColumnNamesComboBox.SelectedItem = null;
            AndCheckbox.IsChecked = false;
            OrCheckbox.IsChecked = false;
            AddCheckbox.IsChecked = false;
            FilterValueTextBox.Text = "";
            MessageBox.Show("Error: " + ex.Message);
            UpdateStatus("Failed To Load Table", Colors.Red);
        }
    }

    private void RefreshTable()
    {
        if (query != "" && query != null)
        {
            try
            {
                using (var cmd = new OracleCommand(query, conn))
                {
                    using (var dataAdapter = new OracleDataAdapter())
                    {
                        dataSet = new DataSet();
                        dataAdapter.SelectCommand = cmd;

                        dataAdapter.Fill(dataSet);
                        TableData.ItemsSource = dataTable.DefaultView;
                        dataTable = dataSet.Tables[0];
                        TableData.ItemsSource = dataTable.DefaultView;
                    }
                }
            }

            catch (OracleException ex)
            {
                TableNameTextBox.Text = "";
                MessageBox.Show("Error: " + ex.Message);
            }
        }
        else
        {
            MessageBox.Show("Select A Table First\nIf You Have Then Check If It Exists");
        }
    }
    private void Button_Click(object sender, RoutedEventArgs e) {
        TableName = TableNameTextBox.Text;
        LoadTable();
    }

    private void MyDataGrid_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e) {
        e.Column.Width = new DataGridLength(1, DataGridLengthUnitType.SizeToHeader);
        e.Column.Width = new DataGridLength(e.Column.ActualWidth + 150, DataGridLengthUnitType.Pixel);
    }

    public void TableData_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e) {
        var grid = (DataGrid)sender;

        var rowView = e.Row.Item as DataRowView;

        var ID = rowView.Row.ItemArray[0].ToString();

        Dispatcher.BeginInvoke(new Action(() => {
            if (e.EditAction == DataGridEditAction.Commit) {
                rowView?.EndEdit();

                var updateQuery = "";


                switch (rowView.Row.RowState) {
                    case DataRowState.Added:
                        updateQuery = $"INSERT INTO {TableName} (";

                        for (var i = 0; i < rowView.Row.ItemArray.Length; i++)
                            if (i == rowView.Row.ItemArray.Length - 1)
                                updateQuery += $"{grid.Columns[i].Header} ";
                            else
                                updateQuery += $"{grid.Columns[i].Header}, ";
                        updateQuery += ") VALUES (";
                        for (var i = 0; i < rowView.Row.ItemArray.Length; i++) {
                            var isDate = checkDataType(grid.Columns[i].Header.ToString()) == "System.DateTime";
                            if (isDate) {
                                if (rowView.Row.ItemArray[i].ToString() == "") {
                                    MessageBox.Show("Date Can't Be Null");
                                    return;
                                }

                                var date = Convert.ToDateTime(rowView.Row.ItemArray[i]).ToString("dd-MMM-yyyy");

                                if (i == rowView.Row.ItemArray.Length - 1)
                                    updateQuery += $"'{date}'";
                                else
                                    updateQuery += $"'{date}', ";
                            }
                            else if (i == rowView.Row.ItemArray.Length - 1) {
                                updateQuery += $"'{rowView.Row.ItemArray[i]}'";
                            }
                            else {
                                updateQuery += $"'{rowView.Row.ItemArray[i]}', ";
                            }
                        }

                        updateQuery += ")";
                        ExecuteCommand(updateQuery);
                        break;

                    case DataRowState.Modified:
                        if (checkDataType(ColumnName) == "System.DateTime")
                            CellColumnValue = Convert.ToDateTime(CellColumnValue).ToString("dd-MMM-yyyy");
                        updateQuery =
                            $"UPDATE {TableName} SET {ColumnName} = '{CellColumnValue}' WHERE {grid.Columns[0].Header} = '{ID}'";
                        try
                        {
                           ExecuteCommand(updateQuery);
                           UpdateStatus("Row Updated Succesfully!", Colors.Green);
                           RefreshTable();
                        }
                        catch (OracleException ex)
                        {
                            MessageBox.Show("Error: " + ex.Message);
                            UpdateStatus("Failed To Update Row", Colors.Red);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Error: " + ex.Message);
                            UpdateStatus("Failed To Update Row", Colors.Red);
                        }
                        break;

                    case DataRowState.Unchanged:
                        UpdateStatus("Row Unchanged!", Colors.White);
                        LoadTable();
                        break;
                }
            }
        }), DispatcherPriority.Background);
    }

    private void TableData_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e) {
        var grid = (DataGrid)sender;

        ColumnName = e.Column.Header.ToString();

        var editingElement = e.EditingElement;
        var editedValue = "";

        if (editingElement is TextBox) CellColumnValue = ((TextBox)editingElement).Text;
    }

    private void PreviewKeyDownHandler(object sender, KeyEventArgs e) {
        if (e.Key == Key.Delete) {
            var grid = (DataGrid)sender;
            var selectedItem = grid.SelectedItem;

            if (selectedItem != null) {
                var row = (DataRowView)grid.SelectedItem;
                var firstColumnValue = row[0];
                var firstColumnName = grid.Columns[0].Header.ToString();

                var deleteQuery = $"DELETE FROM {TableName} WHERE {firstColumnName} = {firstColumnValue}";

                try {
                    var cmd = new OracleCommand(deleteQuery, conn);
                    try {
                        cmd.ExecuteNonQuery();
                        UpdateStatus("Row Deleted Succesfully!", Colors.Green);
                    }
                    catch (OracleException ex) {
                        MessageBox.Show("Error: " + ex.Message);
                        UpdateStatus("Failed To Delete Row", Colors.Red);
                    }

                    RefreshTable();
                }
                catch (OracleException ex) {
                    MessageBox.Show("Error: " + ex.Message);
                    UpdateStatus("Failed To Delete Row", Colors.Red);
                }
                catch (Exception ex) {
                    MessageBox.Show("Error: " + ex.Message);
                    UpdateStatus("Failed To Delete Row", Colors.Red);
                }
            }

            e.Handled = true;
        }
    }

    public void ExecuteCommand(string query) {
        if (conn.State != ConnectionState.Open) conn.Open();
        var cmd = new OracleCommand(query, conn);
            cmd.ExecuteNonQuery();
            UpdateStatus("Command Executed Succesfully!", Colors.Green);
            LoadTable();
        
    }

    public string checkDataType(string fieldName) {
        using (var command = new OracleCommand($"select {fieldName} from {TableName}", conn)) {
            using (var reader = command.ExecuteReader()) {
                var schemaTable = reader.GetSchemaTable();

                foreach (DataRow row in schemaTable.Rows) return row["DataType"].ToString();
            }
        }

        return "";
    }

    private void Button_Click1(object sender, RoutedEventArgs e) {
        RefreshTable();
    }

    private void SearchTextBox_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter)
        {
            TableName = TableNameTextBox.Text;
            LoadTable();
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

    private void FillComboBoxWithColumnNames()
    {
        
        var columnNames = TableData.Columns.Select(col => col.Header.ToString()).ToList();
        columnNames.Insert(0, "");
        ColumnNamesComboBox.ItemsSource = columnNames;
    }

    private void FilterButton_Click(object sender, RoutedEventArgs e)
    {
        LoadTable();
    }

    private void CheckBox_Checked(object sender, RoutedEventArgs e)
    {
        var checkBox = sender as CheckBox;

        if (checkBox == AndCheckbox)
        {
            OrCheckbox.IsChecked = false;
        }
        else if (checkBox == OrCheckbox)
        {
            AndCheckbox.IsChecked = false;
        }
    }

    private void ClearButton_Click(object sender, RoutedEventArgs e)
    {
        TableNameTextBox.Text = "";
        ColumnNameTextBox.Text = "";
        ColumnNamesComboBox.SelectedItem = null;
        FilterValueTextBox.Text = "";
        AndCheckbox.IsChecked = false;
        OrCheckbox.IsChecked = false;
        AddCheckbox.IsChecked = false;
        UpdateStatus("All Cleared!", Colors.White);
    }

    private void CreateReportButton_Click(object sender, RoutedEventArgs e)
    {

        var printView = new PrintView(dataSet);

        printView.Owner = Application.Current.MainWindow;

        printView.Loaded += (s, args) =>
        {
            CenterWindowOnScreen(printView);
        };

        printView.Show();
    }

    private void CenterWindowOnScreen(Window window)
    {
        double parentLeft = window.Owner.Left;
        double parentTop = window.Owner.Top;
        double parentWidth = window.Owner.ActualWidth;
        double parentHeight = window.Owner.ActualHeight;

        double windowWidth = window.ActualWidth;
        double windowHeight = window.ActualHeight;

        window.Left = parentLeft + (parentWidth - windowWidth) / 2;
        window.Top = parentTop + (parentHeight - windowHeight) / 2;
    }
}