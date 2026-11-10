using Dental.Application.Services;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;

namespace Dental.UI.Views
{
    public partial class AddNoteWindow : Window
    {
        private readonly int _patientId;
        private readonly PatientService _patientService;

        public AddNoteWindow(int patientId)
        {
            InitializeComponent();

            _patientId = patientId;
            _patientService = App.Services.GetRequiredService<PatientService>();

            Loaded += AddNoteWindow_Loaded;
        }

        private void AddNoteWindow_Loaded(object sender, RoutedEventArgs e)
        {
            NoteTextBox.Focus();
        }

        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            string note = NoteTextBox.Text.Trim();

            if (string.IsNullOrWhiteSpace(note))
            {
                MessageBox.Show("Please write a note first.");
                return;
            }

            await _patientService.AddPatientNoteAsync(_patientId, note);

            DialogResult = true;
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}