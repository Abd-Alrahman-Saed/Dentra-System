using Dental.Application.Services;
using Dental.Core.Entities;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Input;

namespace Dental.UI.Views
{
    public partial class PatientProfileWindow : Window
    {
        private readonly PatientService _patientService;
        private readonly int _patientId;

        private Patient? _patient;

        public PatientProfileWindow(int patientId)
        {
            InitializeComponent();

            _patientId = patientId;
            _patientService = App.Services.GetRequiredService<PatientService>();

            Loaded += PatientProfileWindow_Loaded;
        }

        private async void PatientProfileWindow_Loaded(object sender, RoutedEventArgs e)
        {
            _patient = await _patientService.GetPatientByIdAsync(_patientId);

            if (_patient == null)
            {
                MessageBox.Show("Patient not found.");
                Close();
                return;
            }

            FullNameTextBlock.Text = _patient.FullName;
            InitialsTextBlock.Text = GetInitials(_patient.FullName);

            int age = CalculateAge(_patient.DateOfBirth);

            PatientDetailsTextBlock.Text =
                $"{age} yrs     ({_patient.DateOfBirth:dd / MM / yyyy})";

            ContactTextBlock.Text =
                $"{_patient.Phone}     |     {_patient.Address}";

            var medicalItems = GetMedicalHistoryItems(_patient);
            var dentalItems = GetDentalHistoryItems(_patient);

            MedicalHistoryItemsControl.ItemsSource = medicalItems;
            MedicalTabItemsControl.ItemsSource = medicalItems;
            DentalHistoryItemsControl.ItemsSource = dentalItems;

            PatientNotesTextBlock.Text =
                string.IsNullOrWhiteSpace(_patient.DentalHistory?.Notes)
                    ? "No notes yet."
                    : _patient.DentalHistory.Notes;

            ShowTab("Overview");

            ColorTeethByStatus();
        }

        private void ShowTab(string tabName)
        {
            OverviewPanel.Visibility = Visibility.Collapsed;
            MedicalHistoryPanel.Visibility = Visibility.Collapsed;
            DentalHistoryPanel.Visibility = Visibility.Collapsed;
            NotesPanel.Visibility = Visibility.Collapsed;

            OverviewTabButton.Foreground = new SolidColorBrush(Color.FromRgb(51, 65, 85));
            MedicalTabButton.Foreground = new SolidColorBrush(Color.FromRgb(51, 65, 85));
            DentalTabButton.Foreground = new SolidColorBrush(Color.FromRgb(51, 65, 85));
            NotesTabButton.Foreground = new SolidColorBrush(Color.FromRgb(51, 65, 85));

            OverviewTabButton.FontWeight = FontWeights.SemiBold;
            MedicalTabButton.FontWeight = FontWeights.SemiBold;
            DentalTabButton.FontWeight = FontWeights.SemiBold;
            NotesTabButton.FontWeight = FontWeights.SemiBold;

            if (tabName == "Overview")
            {
                OverviewPanel.Visibility = Visibility.Visible;
                SetActiveTab(OverviewTabButton);
            }
            else if (tabName == "Medical")
            {
                MedicalHistoryPanel.Visibility = Visibility.Visible;
                SetActiveTab(MedicalTabButton);
            }
            else if (tabName == "Dental")
            {
                DentalHistoryPanel.Visibility = Visibility.Visible;
                SetActiveTab(DentalTabButton);
            }
            else if (tabName == "Notes")
            {
                NotesPanel.Visibility = Visibility.Visible;
                SetActiveTab(NotesTabButton);
            }
        }

        private void SetActiveTab(Button button)
        {
            button.Foreground = new SolidColorBrush(Color.FromRgb(11, 115, 232));
            button.FontWeight = FontWeights.Bold;
        }

        private void OverviewTabButton_Click(object sender, RoutedEventArgs e)
        {
            ShowTab("Overview");
        }

        private void MedicalTabButton_Click(object sender, RoutedEventArgs e)
        {
            ShowTab("Medical");
        }

        private void DentalTabButton_Click(object sender, RoutedEventArgs e)
        {
            ShowTab("Dental");
        }

        private void NotesTabButton_Click(object sender, RoutedEventArgs e)
        {
            ShowTab("Notes");
        }

        private List<string> GetMedicalHistoryItems(Patient patient)
        {
            var items = new List<string>();

            if (patient.MedicalHistory == null)
            {
                items.Add("No medical history added.");
                return items;
            }

            if (patient.MedicalHistory.HasDiabetes)
                items.Add("• Diabetes");

            if (patient.MedicalHistory.HasHypertension)
                items.Add("• Hypertension");

            if (patient.MedicalHistory.HasAllergy)
                items.Add("• Allergy");

            if (!string.IsNullOrWhiteSpace(patient.MedicalHistory.Notes))
                items.Add("• " + patient.MedicalHistory.Notes);

            if (items.Count == 0)
                items.Add("No medical problems recorded.");

            return items;
        }

        private List<string> GetDentalHistoryItems(Patient patient)
        {
            var items = new List<string>();

            if (patient.DentalHistory == null)
            {
                items.Add("No dental history added.");
                return items;
            }

            if (patient.DentalHistory.HasHotColdSensitivity)
                items.Add("• Tooth Sensitivity");

            if (patient.DentalHistory.HasGumDisease)
                items.Add("• Gum Disease");

            if (!string.IsNullOrWhiteSpace(patient.DentalHistory.Notes))
                items.Add("• Notes: " + patient.DentalHistory.Notes);

            if (items.Count == 0)
                items.Add("No dental history recorded.");

            return items;
        }

        private async void ToothButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not Button button || _patient == null)
                return;

            if (button.Tag == null)
            {
                MessageBox.Show("Tooth number is missing.");
                return;
            }

            int toothNumber = int.Parse(button.Tag.ToString()!);

            var tooth = _patient.Teeth.FirstOrDefault(t => t.ToothNumber == toothNumber);

            if (tooth == null)
            {
                MessageBox.Show($"Tooth {toothNumber} not found.");
                return;
            }

            var window = new ToothDetailsWindow(tooth.Id);
            window.ShowDialog();

            if (window.HasChanges)
            {
                await ReloadPatientAsync();
                ColorTeethByStatus();
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
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
        private async void AddNote_Click(object sender, MouseButtonEventArgs e)
        {
            if (_patient == null)
                return;

            var window = new AddNoteWindow(_patient.Id);

            bool? result = window.ShowDialog();

            if (result == true)
            {

                await ReloadPatientAsync();

                ShowTab("Notes");
            }

        }

        private async Task ReloadPatientAsync()
        {
            _patient = await _patientService.GetPatientByIdAsync(_patientId);

            if (_patient == null)
                return;

            PatientNotesTextBlock.Text =
                string.IsNullOrWhiteSpace(_patient.DentalHistory?.Notes)
                    ? "No notes yet."
                    : _patient.DentalHistory.Notes;

            ColorTeethByStatus();
        }

        private void ColorTeethByStatus()
        {
            if (_patient == null)
                return;

            var buttons = FindVisualChildren<Button>(this)
                .Where(b => b.Tag != null);

            foreach (var button in buttons)
            {
                if (!int.TryParse(button.Tag.ToString(), out int toothNumber))
                    continue;

                var tooth = _patient.Teeth.FirstOrDefault(t => t.ToothNumber == toothNumber);

                if (tooth == null)
                    continue;

                button.Background = tooth.Status switch
                {
                    Dental.Core.Enums.ToothStatus.Healthy => Brushes.White,
                    Dental.Core.Enums.ToothStatus.WithNotes => new SolidColorBrush(Color.FromRgb(245, 158, 11)),
                    Dental.Core.Enums.ToothStatus.Treated => new SolidColorBrush(Color.FromRgb(34, 197, 94)),
                    Dental.Core.Enums.ToothStatus.Problem => new SolidColorBrush(Color.FromRgb(239, 68, 68)),
                    _ => Brushes.White
                };

                button.Foreground = tooth.Status == Dental.Core.Enums.ToothStatus.Healthy
                    ? Brushes.Black
                    : Brushes.White;
            }
        }

        private static IEnumerable<T> FindVisualChildren<T>(DependencyObject parent) where T : DependencyObject
        {
            if (parent == null)
                yield break;

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);

                if (child is T typedChild)
                    yield return typedChild;

                foreach (var descendant in FindVisualChildren<T>(child))
                    yield return descendant;
            }
        }
    }
}