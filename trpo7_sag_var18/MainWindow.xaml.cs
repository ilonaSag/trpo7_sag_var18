using System.Collections.ObjectModel;
using System.IO;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace trpo7_sag_var18
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        HttpClient client = new HttpClient();
        PackageApi api;
        public ObservableCollection<Package> Packages { get; set; } = new();
        public Package SelectedPackage { get; set;  }
        public int id { get; set; } = 0;
        private Package _originalPackage;
        private Package _currentPackage = new();
        public Package CurrentPackage
        {
            get => _currentPackage;
            set
            {
                _currentPackage = value;
                FormPanel.DataContext = _currentPackage;
            }
        }
        public MainWindow()
        {
            InitializeComponent();
            client.BaseAddress = new Uri("https://localhost:7218/");
            api = new PackageApi(client);

            DataContext = this;
            FormPanel.DataContext = CurrentPackage;
        }
        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                HttpResponseMessage response = await client.GetAsync("/Package");
                if (response.IsSuccessStatusCode)
                {
                    var list = await api.GetPackages();
                    Packages.Clear();
                    foreach (var item in list)
                        Packages.Add(item);
                }
                else
                {
                    MessageBox.Show("Ошибка: " + response.StatusCode);
                }
            }
            catch (HttpRequestException ex)
            {
                MessageBox.Show(ex.Message, "Ошибка");
            }
        }

        private async void GetOne_Click(object sender, RoutedEventArgs e)
        {

            if (string.IsNullOrWhiteSpace(IdTextBox.Text))
            {
                MessageBox.Show("Поле Id не должно быть пустым");
                return;
            }
            try
            {
                var response = await client.GetFromJsonAsync<Package>($"/Package/{IdTextBox.Text}");
                if (response != null)
                {
                    Packages.Clear();
                    Packages.Add(response);
                }

            }
            catch (HttpRequestException ex)
            {
                MessageBox.Show("Сервер недоступен: " + ex.Message);
            }
        }

        private async void Add_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var result = await api.CreatePackage(new Package
                {
                    TrackingNumber = CurrentPackage.TrackingNumber,
                    Recipient = CurrentPackage.Recipient,
                    Weight = CurrentPackage.Weight,
                    Status = CurrentPackage.Status,
                    IsDelivered = CurrentPackage.IsDelivered,
                });
                if (result == null)
                {
                    MessageBox.Show("Успешно!");
                    Button_Click(null, null);
                }
                else
                    MessageBox.Show(result, "Ошибка");
            }
            catch (HttpRequestException ex)
            {
                MessageBox.Show(ex.Message, "Ошибка");
            }
        }
        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selected = (sender as ListView)?.SelectedItem as Package;
            if (selected != null)
            {
                CurrentPackage = selected;
                FormPanel.DataContext = CurrentPackage; 
            }
        }

        private async void Delete_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentPackage.Id == 0)
            {
                MessageBox.Show("Выберите запись из списка");
                return;
            }

            var confirm = MessageBox.Show($"Вы уверены, что хотите удалить {CurrentPackage.TrackingNumber}?", "Подтверждение", MessageBoxButton.YesNo);
            if (confirm == MessageBoxResult.No)
                return;
            try
            {
                var result = await api.DeletePackage(CurrentPackage.Id);
                if (result == null)
                {
                    MessageBox.Show("Успешно!");
                    CurrentPackage = new();
                    Button_Click(null, null);
                }
                else
                    MessageBox.Show(result, "Ошибка");
            }
            catch (HttpRequestException ex)
            {
                MessageBox.Show(ex.Message, "Ошибка");
            }
        }

        private async void Put_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentPackage == null || CurrentPackage.Id == 0)
            {
                MessageBox.Show("Выберите запись из списка");
                return;
            }

            try
            {
                var result = await api.PutPackage(CurrentPackage.Id, CurrentPackage);
                if (result == null)
                {
                    MessageBox.Show("Успешно изменено!");
                    Button_Click(null, null);
                }
                else
                    MessageBox.Show(result, "Ошибка");
            }
            catch (HttpRequestException ex)
            {
                MessageBox.Show(ex.Message, "Ошибка");
            }
        }

        private async void Patch_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentPackage == null || CurrentPackage.Id == 0 || _originalPackage == null)
            {
                MessageBox.Show("Выберите запись из списка");
                return;
            }

            try
            {
                var changes = new Dictionary<string, object>();

                if (CurrentPackage.TrackingNumber != _originalPackage.TrackingNumber)
                    changes["trackingNumber"] = CurrentPackage.TrackingNumber;

                if (CurrentPackage.Recipient != _originalPackage.Recipient)
                    changes["recipient"] = CurrentPackage.Recipient;

                if (CurrentPackage.Weight != _originalPackage.Weight)
                    changes["weight"] = CurrentPackage.Weight;

                if (CurrentPackage.Status != _originalPackage.Status)
                    changes["status"] = CurrentPackage.Status;

                if (CurrentPackage.IsDelivered != _originalPackage.IsDelivered)
                    changes["isDelivered"] = CurrentPackage.IsDelivered;

                if (changes.Count == 0)
                {
                    MessageBox.Show("Вы ничего не изменили");
                    return;
                }

                var result = await api.PatchPackage(CurrentPackage.Id, changes);
                if (result == null)
                {
                    MessageBox.Show("Успешно сохранено!");
                    Button_Click(null, null); 
                }
                else
                    MessageBox.Show(result, "Ошибка");
            }
            catch (HttpRequestException ex)
            {
                MessageBox.Show(ex.Message, "Ошибка");
            }
        }
    }

}