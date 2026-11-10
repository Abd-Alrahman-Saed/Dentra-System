using Dental.Application.Services;
using Dental.Core.Entities;
using Dental.Core.Enums;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Dental.UI.Views
{
    public partial class ToothDetailsWindow : Window
    {
        private readonly ToothService _toothService;
        private readonly ToothRecordService _toothRecordService;
        private readonly int _toothId;

        public bool HasChanges { get; private set; } = false;
        public int RecordId { get; set; }

        private Tooth? _tooth;

        private ObservableCollection<VisitViewModel> _visits = new();

        public ToothDetailsWindow(int toothId)
        {
            InitializeComponent();

            _toothId = toothId;

            _toothService = App.Services.GetRequiredService<ToothService>();
            _toothRecordService = App.Services.GetRequiredService<ToothRecordService>();

            TimelineItemsControl.ItemsSource = _visits;
            AllVisitsItemsControl.ItemsSource = _visits;

            Loaded += ToothDetailsWindow_Loaded;
        }

        private async void ToothDetailsWindow_Loaded(object sender, RoutedEventArgs e)
        {
            await LoadToothAsync();
            ShowPanel("Timeline");
        }

        private async Task LoadToothAsync()
        {
            _tooth = await _toothService.GetToothDetailsAsync(_toothId);

            if (_tooth == null)
            {
                MessageBox.Show("Tooth not found.");
                Close();
                return;
            }

            var TNum = _tooth.ToothNumber % 8;

            if (TNum == 0)
                TNum = 8;

            ToothNumberTextBlock.Text = TNum.ToString();
            ToothArchTextBlock.Text = _tooth.Arch.ToString();

            SetStatusStyle(_tooth.Status);

            var records = await _toothRecordService.GetRecordsByToothIdAsync(_toothId);

            var latestVisit = records.FirstOrDefault();

            if (latestVisit != null)
            {
                LastVisitTextBlock.Text = latestVisit.VisitDate.ToString("dd MMM yyyy");
                NotesTextBlock.Text = string.IsNullOrWhiteSpace(latestVisit.Notes)
                    ? "No notes"
                    : latestVisit.Notes;
            }
            else
            {
                LastVisitTextBlock.Text = "No visits yet";
                NotesTextBlock.Text = "No notes";
            }

            _visits.Clear();

            var visitItems = records.Select((record, index) => new VisitViewModel
            {
                RecordId = record.Id,

                VisitTitle = $"Visit {records.Count - index}",
                VisitDate = record.VisitDate,
                Notes = string.IsNullOrWhiteSpace(record.Notes)
                    ? "No notes"
                    : record.Notes,

                ImagesCountText = $"🖼 {record.Images.Count}",
                VisitColor = index == 0 ? "#22C55E" : "#0B73E8",

                ClinicalImages = record.Images
                    .Where(i => i.ImageType == ImageType.Real)
                    .Select(i => i.FilePath)
                    .ToArray(),

                XrayImages = record.Images
                    .Where(i => i.ImageType == ImageType.XRay)
                    .Select(i => i.FilePath)
                    .ToArray()
            });

            foreach (var item in visitItems)
                _visits.Add(item);
        }

        private void SetStatusStyle(ToothStatus status)
        {
            StatusTextBlock.Text = GetStatusDisplayName(status);

            string color = status switch
            {
                ToothStatus.Healthy => "#111827",
                ToothStatus.WithNotes => "#F59E0B",
                ToothStatus.Treated => "#22C55E",
                ToothStatus.Problem => "#EF4444",
                _ => "#111827"
            };

            var brush = new SolidColorBrush(
                (Color)ColorConverter.ConvertFromString(color)
            );

            StatusTextBlock.Foreground = brush;
            StatusDot.Fill = brush;
        }

        private string GetStatusDisplayName(ToothStatus status)
        {
            return status switch
            {
                ToothStatus.Healthy => "Healthy",
                ToothStatus.WithNotes => "With Notes",
                ToothStatus.Treated => "Treated",
                ToothStatus.Problem => "Problem",
                _ => status.ToString()
            };
        }

        private void ShowPanel(string panelName)
        {
            TimelinePanel.Visibility = Visibility.Collapsed;
            AllVisitsPanel.Visibility = Visibility.Collapsed;
            ToothInfoPanel.Visibility = Visibility.Collapsed;

            ResetSideButtons();

            if (panelName == "Timeline")
            {
                TimelinePanel.Visibility = Visibility.Visible;
                SetActiveSideButton(TimelineButton);
            }
            else if (panelName == "AllVisits")
            {
                AllVisitsPanel.Visibility = Visibility.Visible;
                SetActiveSideButton(AllVisitsButton);
            }
            else if (panelName == "ToothInfo")
            {
                ToothInfoPanel.Visibility = Visibility.Visible;
                SetActiveSideButton(ToothInfoButton);
            }
        }

        private void ResetSideButtons()
        {
            TimelineButton.Background = Brushes.Transparent;
            AllVisitsButton.Background = Brushes.Transparent;
            ToothInfoButton.Background = Brushes.Transparent;

            TimelineButton.Foreground = new SolidColorBrush(Color.FromRgb(100, 116, 139));
            AllVisitsButton.Foreground = new SolidColorBrush(Color.FromRgb(100, 116, 139));
            ToothInfoButton.Foreground = new SolidColorBrush(Color.FromRgb(100, 116, 139));
        }

        private void SetActiveSideButton(Button button)
        {
            button.Background = new SolidColorBrush(Color.FromRgb(239, 246, 255));
            button.Foreground = new SolidColorBrush(Color.FromRgb(11, 115, 232));
        }

        private void TimelineButton_Click(object sender, RoutedEventArgs e)
        {
            ShowPanel("Timeline");
        }

        private void AllVisitsButton_Click(object sender, RoutedEventArgs e)
        {
            ShowPanel("AllVisits");
        }

        private void ToothInfoButton_Click(object sender, RoutedEventArgs e)
        {
            ShowPanel("ToothInfo");
        }

        private async void AddNewVisitButton_Click(object sender, RoutedEventArgs e)
        {
            var window = new AddNewVisitWindow(_toothId);
            window.Owner = this;

            if (window.ShowDialog() == true)
            {
                await _toothRecordService.AddRecordAsync(
                    toothId: _toothId,
                    notes: window.Description,
                    window.SelectedStatus,
                    realImagePaths: window.ClinicalPhotoPaths,
                    xrayImagePaths: window.XrayImagePaths
                );

                HasChanges = true;

                await LoadToothAsync();

                //MessageBox.Show("Visit saved successfully.");
            }
        }

        private async void VisitCard_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (sender is not Border border)
                return;

            if (border.DataContext is not VisitViewModel visit)
                return;

            var window = new VisitDetailsWindow(visit.RecordId);

            bool? result = window.ShowDialog();

            if (result == true)
            {
                await LoadToothAsync();
            }

          
        }
    }

    public class VisitViewModel
    {
        public string VisitTitle { get; set; } = "";
        public DateTime VisitDate { get; set; }

        public string VisitDateText => VisitDate.ToString("dd MMM yyyy");

        public string Notes { get; set; } = "";
        public string ImagesCountText { get; set; } = "";
        public string VisitColor { get; set; } = "#2563EB";

        public string[] ClinicalImages { get; set; } = Array.Empty<string>();
        public string[] XrayImages { get; set; } = Array.Empty<string>();
        public int RecordId { get; internal set; }
    }
}