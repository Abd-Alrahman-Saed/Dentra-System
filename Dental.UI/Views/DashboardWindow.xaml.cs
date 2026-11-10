using Dental.Application.Services;
using Dental.Core.Entities;
using Dental.UI.Views;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using System.Windows.Controls;

namespace Dental.UI.Views
{
    public partial class DashboardWindow : Window
    {
        private readonly PatientService _patientService;
        private List<PatientListItem> _patients = new();

        public DashboardWindow()
        {
            InitializeComponent();

            _patientService = App.Services.GetRequiredService<PatientService>();

            Loaded += DashboardWindow_Loaded;
        }

        private async void DashboardWindow_Loaded(object sender, RoutedEventArgs e)
        {
            await LoadPatientsAsync();
        }

        private async Task LoadPatientsAsync()
        {
            var patients = await _patientService.GetAllPatientsAsync();

            _patients = patients.Select(p => new PatientListItem
            {
                Id = p.Id,
                FullName = p.FullName,
                Phone = p.Phone,
                Age = CalculateAge(p.DateOfBirth),
                Initials = GetInitials(p.FullName),
                CreatedAtText = p.CreatedAt.ToString("dd MMM yyyy")
            }).ToList();

            PatientsItemsControl.ItemsSource = _patients;
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string searchText = SearchTextBox.Text.ToLower();

            PatientsItemsControl.ItemsSource = _patients
                .Where(p =>
                    p.FullName.ToLower().Contains(searchText) ||
                    p.Phone.Contains(searchText))
                .ToList();
        }

        private async void AddPatientButton_Click(object sender, RoutedEventArgs e)
        {
            var window = new AddPatientWindow();

            var result = window.ShowDialog();

            if (result == true)
            {
                await LoadPatientsAsync();
            }
        }

        private static int CalculateAge(DateTime dateOfBirth)
        {
            int age = DateTime.Today.Year - dateOfBirth.Year;

            if (dateOfBirth.Date > DateTime.Today.AddYears(-age))
                age--;

            return age;
        }

        private static string GetInitials(string fullName)
        {
            var parts = fullName.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length == 0)
                return "";

            if (parts.Length == 1)
                return parts[0][0].ToString().ToUpper();

            return $"{parts[0][0]}{parts[1][0]}".ToUpper();
        }
        private void OpenPatientButton_Click(object sender, RoutedEventArgs e)
        {
            e.Handled = true;

            if (sender is Button button && button.Tag is int patientId)
            {
                var window = new PatientProfileWindow(patientId);
                window.ShowDialog();

                _ = LoadPatientsAsync();
            }
        }

        private void DashboardWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            foreach (Window window in System.Windows.Application.Current.Windows)
            {
                if (window != this)
                    window.Close();
            }

            System.Windows.Application.Current.Shutdown();
        }

        private void PatientCard_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (sender is FrameworkElement element && element.Tag is int patientId)
            {
                var window = new PatientProfileWindow(patientId);
                window.ShowDialog();

                _ = LoadPatientsAsync();
            }
        }
    }
}

    public class PatientListItem
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public int Age { get; set; }
        public string Initials { get; set; } = string.Empty;
        public string CreatedAtText { get; set; } = string.Empty;


    }

