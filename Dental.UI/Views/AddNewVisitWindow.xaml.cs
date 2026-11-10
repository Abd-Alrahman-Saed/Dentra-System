using Dental.Application.Services;
using Dental.Core.Enums;
using Dental.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Dental.UI.Views
{
    public partial class AddNewVisitWindow : Window
    {
        public DateTime VisitDate { get; private set; }
        public string Description { get; private set; } = string.Empty;

        public List<string> ClinicalPhotoPaths { get; private set; } = new();
        public List<string> XrayImagePaths { get; private set; } = new();

        private readonly int _toothId;
        private readonly AppDbContext _context;
        public ToothStatus SelectedStatus { get; private set; }

        public AddNewVisitWindow(int toothId)
        {
            InitializeComponent();
            _toothId = toothId;
            VisitDatePicker.SelectedDate = DateTime.Today;


            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"]?.ConnectionString
                ?? "Server=.\\SQLEXPRESS;Database=DentalPatientRecordDb;Trusted_Connection=True;TrustServerCertificate=True;";

            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseSqlServer(connectionString)
                .Options;

            _context = new AppDbContext(options);

            LoadCurrentToothStatus();
        }

        private void LoadCurrentToothStatus()
        {
            var tooth = _context.Teeth.FirstOrDefault(t => t.Id == _toothId);

            if (tooth == null)
                return;

            foreach (ComboBoxItem item in ToothStatusComboBox.Items)
            {
                if (item.Tag?.ToString() == tooth.Status.ToString())
                {
                    ToothStatusComboBox.SelectedItem = item;
                    break;
                }
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void SaveVisitButton_Click(object sender, RoutedEventArgs e)
        {
            if (VisitDatePicker.SelectedDate == null)
            {
                MessageBox.Show("Please select visit date.", "Validation",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var selectedStatusItem = ToothStatusComboBox.SelectedItem as ComboBoxItem;

            if (selectedStatusItem == null)
            {
                MessageBox.Show("Please select tooth status.");
                return;
            }

            SelectedStatus = Enum.Parse<ToothStatus>(
                    selectedStatusItem.Tag!.ToString()!
                );

            var tooth = _context.Teeth.FirstOrDefault(t => t.Id == _toothId);

            if (tooth == null)
            {
                MessageBox.Show("Tooth not found.");
                return;
            }

            tooth.Status = SelectedStatus;
            _context.SaveChanges();

            VisitDate = VisitDatePicker.SelectedDate.Value;
            Description = DescriptionTextBox.Text.Trim();

            DialogResult = true;
            Close();
        }

        private void UploadClinicalPhotos_Click(object sender, RoutedEventArgs e)
        {
            UploadClinicalPhotos();
        }

        private void UploadXrayImages_Click(object sender, RoutedEventArgs e)
        {
            UploadXrayImages();
        }

        private void ClinicalDropArea_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            UploadClinicalPhotos();
        }

        private void XrayDropArea_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            UploadXrayImages();
        }

        private void ClinicalDropArea_Drop(object sender, DragEventArgs e)
        {
            var files = GetDroppedImageFiles(e);

            if (files.Count == 0)
                return;

            ClinicalPhotoPaths.AddRange(files);
            UpdateClinicalPhotosText();
        }

        private void XrayDropArea_Drop(object sender, DragEventArgs e)
        {
            var files = GetDroppedImageFiles(e);

            if (files.Count == 0)
                return;

            XrayImagePaths.AddRange(files);
            UpdateXrayImagesText();
        }

        private void UploadClinicalPhotos()
        {
            var files = OpenImagePicker();

            if (files.Count == 0)
                return;

            ClinicalPhotoPaths.AddRange(files);
            UpdateClinicalPhotosText();
        }

        private void UploadXrayImages()
        {
            var files = OpenImagePicker();

            if (files.Count == 0)
                return;

            XrayImagePaths.AddRange(files);
            UpdateXrayImagesText();
        }

        private List<string> OpenImagePicker()
        {
            var dialog = new OpenFileDialog
            {
                Title = "Select Images",
                Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp;*.webp",
                Multiselect = true
            };

            if (dialog.ShowDialog() == true)
                return dialog.FileNames.ToList();

            return new List<string>();
        }

        private List<string> GetDroppedImageFiles(DragEventArgs e)
        {
            if (!e.Data.GetDataPresent(DataFormats.FileDrop))
                return new List<string>();

            var files = (string[])e.Data.GetData(DataFormats.FileDrop);

            return files
                .Where(file =>
                    file.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase) ||
                    file.EndsWith(".jpeg", StringComparison.OrdinalIgnoreCase) ||
                    file.EndsWith(".png", StringComparison.OrdinalIgnoreCase) ||
                    file.EndsWith(".bmp", StringComparison.OrdinalIgnoreCase) ||
                    file.EndsWith(".webp", StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        private void UpdateClinicalPhotosText()
        {
            ClinicalPhotosText.Text =
                ClinicalPhotoPaths.Count == 0
                    ? "Drag & drop or click to upload"
                    : $"{ClinicalPhotoPaths.Count} clinical photo(s) selected";
        }

        private void UpdateXrayImagesText()
        {
            XrayImagesText.Text =
                XrayImagePaths.Count == 0
                    ? "Drag & drop or click to upload"
                    : $"{XrayImagePaths.Count} x-ray image(s) selected";
        }
    }
}