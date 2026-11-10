using Dental.Application.Services;
using Dental.Core.Entities;
using Dental.Core.Enums;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using System.Windows.Controls;

namespace Dental.UI.Views
{
    public partial class AddPatientWindow : Window
    {
        private readonly PatientService _patientService;

        public AddPatientWindow()
        {
            InitializeComponent();

            _patientService = App.Services.GetRequiredService<PatientService>();
        }

        private async void SavePatientButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(FullNameTextBox.Text))
                {
                    MessageBox.Show("Please enter patient full name.");
                    return;
                }

                if (DateOfBirthPicker.SelectedDate == null)
                {
                    MessageBox.Show("Please select date of birth.");
                    return;
                }

                var selectedGender = ((ComboBoxItem)GenderComboBox.SelectedItem).Content.ToString();

                var patient = new Patient
                {
                    FullName = FullNameTextBox.Text,
                    DateOfBirth = DateOfBirthPicker.SelectedDate.Value,
                    Phone = PhoneTextBox.Text,
                    Address = AddressTextBox.Text,
                    Gender = selectedGender == "Female" ? Gender.Female : Gender.Male
                };

                var medicalHistory = new MedicalHistory
                {
                    HasDiabetes = DiabetesCheckBox.IsChecked == true,
                    HasHypertension = HypertensionCheckBox.IsChecked == true,
                    HasAllergy = AllergyCheckBox.IsChecked == true,
                    Notes = OtherConditionsTextBox.Text
                };

                var dentalHistory = new DentalHistory
                {
                    HasHotColdSensitivity = ToothSensitivityCheckBox.IsChecked == true,
                    HasGumDisease = GumDiseaseCheckBox.IsChecked == true,
                    Notes = DentalNotesTextBox.Text
                };

                await _patientService.AddPatientAsync(patient, medicalHistory, dentalHistory);

                DialogResult = true;

                //MessageBox.Show("Patient saved successfully.");

                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            
        }
        private async void GoBackButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}