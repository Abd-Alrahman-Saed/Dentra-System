using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace Dental.UI.Views
{
    public partial class ImageViewerWindow : Window
    {
        private readonly List<string> _imagePaths;
        private int _currentIndex;
        private double _zoom = 1.0;

        private bool _isDragging = false;
        private Point _startPoint;
        private double _startX;
        private double _startY;

        public ImageViewerWindow(List<string> imagePaths, int startIndex)
        {
            InitializeComponent();

            _imagePaths = imagePaths;
            _currentIndex = startIndex;

            ShowCurrentImage();

            Loaded += (s, e) =>
            {
                Activate();
                Focus();
                Keyboard.Focus(this);
            };
        }

        private void ShowCurrentImage()
        {
            if (_imagePaths.Count == 0)
                return;

            var imagePath = _imagePaths[_currentIndex];

            PreviewImage.Source = new BitmapImage(new Uri(imagePath));
            CounterTextBlock.Text = $"{_currentIndex + 1} / {_imagePaths.Count}";

            _zoom = 1.0;

            ImageScaleTransform.ScaleX = 1;
            ImageScaleTransform.ScaleY = 1;

            ImageTranslateTransform.X = 0;
            ImageTranslateTransform.Y = 0;
        }
        private void ImageContainer_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (e.Delta > 0)
                _zoom += 0.1;
            else
                _zoom -= 0.1;

            if (_zoom < 1)
                _zoom = 1;

            if (_zoom > 5)
                _zoom = 5;

            ImageScaleTransform.ScaleX = _zoom;
            ImageScaleTransform.ScaleY = _zoom;

            ImageContainer.Cursor = _zoom > 1
                ? Cursors.Hand
                : Cursors.Arrow;
        }

        private void Previous_Click(object sender, RoutedEventArgs e)
        {
            GoPrevious();
        }

        private void Next_Click(object sender, RoutedEventArgs e)
        {
            GoNext();
        }

        private void GoPrevious()
        {
            if (_imagePaths.Count == 0)
                return;

            _currentIndex--;

            if (_currentIndex < 0)
                _currentIndex = _imagePaths.Count - 1;

            ShowCurrentImage();
        }

        private void GoNext()
        {
            if (_imagePaths.Count == 0)
                return;

            _currentIndex++;

            if (_currentIndex >= _imagePaths.Count)
                _currentIndex = 0;

            ShowCurrentImage();
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
                Close();

            if (e.Key == Key.Left)
                GoPrevious();

            if (e.Key == Key.Right)
                GoNext();
        }

        private void Image_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (_zoom <= 1)
                return;

            _isDragging = true;
            _startPoint = e.GetPosition(this);

            _startX = ImageTranslateTransform.X;
            _startY = ImageTranslateTransform.Y;

            ImageContainer.Cursor = Cursors.SizeAll;

            Mouse.Capture((UIElement)sender);
        }

        private void Image_MouseMove(object sender, MouseEventArgs e)
        {
            if (!_isDragging) return;

            var currentPoint = e.GetPosition(this);

            var deltaX = currentPoint.X - _startPoint.X;
            var deltaY = currentPoint.Y - _startPoint.Y;

            ImageTranslateTransform.X = _startX + deltaX;
            ImageTranslateTransform.Y = _startY + deltaY;
        }

        private void Image_MouseUp(object sender, MouseButtonEventArgs e)
        {
            _isDragging = false;

            ImageContainer.Cursor = _zoom > 1
                ? Cursors.Hand
                : Cursors.Arrow;

            Mouse.Capture(null);
        }
    }
}