using Dental.Application.Services;
using Dental.Core.Enums;
using Dental.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Media.Imaging;

namespace Dental.UI.Views
{
    public partial class VisitDetailsWindow : Window
    {
        private ObservableCollection<ImageItem> _clinicalImages = new();
        private ObservableCollection<ImageItem> _xrayImages = new();
        private List<ImageItem> _allImages = new();

        private int _recordId;
        private ToothRecordService? _toothRecordService;
        private string _oldNotes = string.Empty;
        private readonly AppDbContext _context;

        private class ImageItem
        {
            public int Id { get; set; }
            public string FilePath { get; set; } = "";
            public BitmapImage Source { get; set; } = null!;
        }

        private static AppDbContext CreateDefaultDbContext()
        {
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"]?.ConnectionString
                ?? "Server=.\\SQLEXPRESS;Database=DentalPatientRecordDb;Trusted_Connection=True;TrustServerCertificate=True;";

            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseSqlServer(connectionString)
                .Options;

            return new AppDbContext(options);
        }

        public VisitDetailsWindow()
        {
            InitializeComponent();

            _context = CreateDefaultDbContext();
            _toothRecordService = new ToothRecordService(_context);

            ClinicalImagesItemsControl.ItemsSource = _clinicalImages;
            XrayImagesItemsControl.ItemsSource = _xrayImages;
        }

        public VisitDetailsWindow(int recordId) : this()
        {
            _recordId = recordId;
            Loaded += VisitDetailsWindow_Loaded;
        }

        private async void VisitDetailsWindow_Loaded(object sender, RoutedEventArgs e)
        {
            await LoadVisitDetailsAsync();
        }

        private async Task LoadVisitDetailsAsync()
        {
            if (_toothRecordService == null)
            {
                MessageBox.Show("Service is not initialized.");
                return;
            }

            var record = await _toothRecordService.GetRecordByIdAsync(_recordId);

            if (record == null)
            {
                MessageBox.Show("Visit not found.");
                Close();
                return;
            }

            VisitTitleTextBlock.Text = $"Visit Details";
            VisitDateTextBlock.Text = $"📅 {record.VisitDate:dd MMM yyyy}";

            NotesTextBlock.Text = string.IsNullOrWhiteSpace(record.Notes)
                ? "No notes"
                : record.Notes;

            _clinicalImages.Clear();
            _xrayImages.Clear();

            foreach (var image in record.Images.Where(i => i.ImageType == ImageType.Real))
                _clinicalImages.Add(new ImageItem
                {
                    Id = image.Id,
                    FilePath = image.FilePath,
                    Source = LoadImageWithoutLock(image.FilePath)
                });

            foreach (var image in record.Images.Where(i => i.ImageType == ImageType.XRay))
                _xrayImages.Add(new ImageItem
                {
                    Id = image.Id,
                    FilePath = image.FilePath,
                    Source = LoadImageWithoutLock(image.FilePath)
                });

            _allImages = _clinicalImages.Concat(_xrayImages).ToList();
        }

        private BitmapImage LoadImageWithoutLock(string filePath)
        {
            var bitmap = new BitmapImage();

            bitmap.BeginInit();
            bitmap.CacheOption = BitmapCacheOption.OnLoad; 
            bitmap.UriSource = new Uri(filePath, UriKind.Absolute);
            bitmap.EndInit();

            bitmap.Freeze(); 

            return bitmap;
        }

        private void Image_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount != 2)
                return;

            if (sender is not System.Windows.Controls.Image image)
                return;

            if (image.DataContext is not ImageItem imageItem)
                return;

            var startIndex = _allImages.IndexOf(imageItem);

            if (startIndex < 0)
                startIndex = 0;

            var viewer = new ImageViewerWindow(
                _allImages.Select(i => i.FilePath).ToList(),
                startIndex);

            viewer.Owner = this;
            viewer.Show();
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private async void UploadClinical_Click(object sender, RoutedEventArgs e)
        {
            if (_toothRecordService == null)
                return;

            var files = PickImages();

            foreach (var file in files)
            {
                await _toothRecordService!.AddImageAsync(
                    _recordId,
                    file,
                    ImageType.Real);
            }

            await LoadVisitDetailsAsync();
        }

        private async void UploadXray_Click(object sender, RoutedEventArgs e)
        {
            if (_toothRecordService == null)
                return;

            var files = PickImages();

            foreach (var file in files)
            {
                await _toothRecordService!.AddImageAsync(
                    _recordId,
                    file,
                    ImageType.XRay);
            }

            await LoadVisitDetailsAsync();
        }

        private string[] PickImages()
        {
            var dialog = new OpenFileDialog
            {
                Title = "Select Images",
                Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp;*.webp",
                Multiselect = true
            };

            if (dialog.ShowDialog() == true)
                return dialog.FileNames;

            return Array.Empty<string>();
        }

        private void EditNotesButton_Click(object sender, RoutedEventArgs e)
        {
            _oldNotes = NotesTextBlock.Text;

            NotesTextBox.Text = NotesTextBlock.Text;

            NotesTextBlock.Visibility = Visibility.Collapsed;
            NotesTextBox.Visibility = Visibility.Visible;

            EditNotesButton.Visibility = Visibility.Collapsed;
            EditNotesActionsPanel.Visibility = Visibility.Visible;

            NotesTextBox.Focus();
            NotesTextBox.CaretIndex = NotesTextBox.Text.Length;
        }

        private void CancelNotesButton_Click(object sender, RoutedEventArgs e)
        {
            NotesTextBox.Text = _oldNotes;

            NotesTextBlock.Visibility = Visibility.Visible;
            NotesTextBox.Visibility = Visibility.Collapsed;

            EditNotesButton.Visibility = Visibility.Visible;
            EditNotesActionsPanel.Visibility = Visibility.Collapsed;
        }

        private async void SaveNotesButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_toothRecordService == null)
                {
                    MessageBox.Show("Service is not initialized.");
                    return;
                }

                string newNotes = NotesTextBox.Text.Trim();

                await _toothRecordService.UpdateRecordNotesAsync(_recordId, newNotes);

                NotesTextBlock.Text = newNotes;

                NotesTextBlock.Visibility = Visibility.Visible;
                NotesTextBox.Visibility = Visibility.Collapsed;

                EditNotesButton.Visibility = Visibility.Visible;
                EditNotesActionsPanel.Visibility = Visibility.Collapsed;

                //MessageBox.Show("Notes updated successfully.");

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void MoreButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button)
            {
                button.ContextMenu.PlacementTarget = button;
                button.ContextMenu.IsOpen = true;
            }
        }

        private void DeleteVisitMenuItem_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show(
                "Are you sure you want to delete this visit?",
                "Delete Visit",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result != MessageBoxResult.Yes)
                return;

            var record = _context.ToothRecords
                .Include(r => r.Images)
                .FirstOrDefault(r => r.Id == _recordId);

            if (record == null)
            {
                MessageBox.Show("Visit not found.");
                Close();
                return;
            }

            _context.ToothRecords.Remove(record);
            _context.SaveChanges();

            //MessageBox.Show("Visit deleted successfully.");

            DialogResult = true;
            Close();
        }



        private void DeleteImageMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not MenuItem menuItem)
                return;

            if (menuItem.CommandParameter is not ImageItem imageItem)
                return;

            var result = MessageBox.Show(
                "Are you sure you want to delete this image?",
                "Delete Image",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result != MessageBoxResult.Yes)
                return;

            var image = _context.ToothImages
                .FirstOrDefault(i => i.Id == imageItem.Id);

            if (image == null)
                return;

            _context.ToothImages.Remove(image);
            _context.SaveChanges();

            if (File.Exists(imageItem.FilePath))
                File.Delete(imageItem.FilePath);

            _clinicalImages.Remove(imageItem);
            _xrayImages.Remove(imageItem);

            _allImages = _clinicalImages.Concat(_xrayImages).ToList();
        }
    }
}