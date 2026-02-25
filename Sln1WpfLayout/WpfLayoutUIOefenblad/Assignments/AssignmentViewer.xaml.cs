using System.Diagnostics;
using System.Windows.Controls;

namespace WpfLayoutUIOefenblad.Assignments
{
    /// <summary>
    /// Interaction logic for AssignmentViewer.xaml
    /// </summary>
    public partial class AssignmentViewer : Page
    {
        private Uri? _initialUri;
        private bool _initialLoadCompleted;

        public AssignmentViewer(string fileName)
        {
            InitializeComponent();
            string basePath = AppDomain.CurrentDomain.BaseDirectory;
            string fullPath = System.IO.Path.Combine(basePath, "Assignments", $"{fileName}.html");
            _initialUri = new Uri(fullPath);

            brwAssignment.CoreWebView2InitializationCompleted += BrwAssignment_CoreWebView2InitializationCompleted;
            brwAssignment.NavigationStarting += BrwAssignment_NavigationStarting;
            brwAssignment.NavigationCompleted += BrwAssignment_NavigationCompleted;
            brwAssignment.Source = _initialUri;
        }

        private void BrwAssignment_CoreWebView2InitializationCompleted(object? sender, Microsoft.Web.WebView2.Core.CoreWebView2InitializationCompletedEventArgs e)
        {
            if (e.IsSuccess)
            {
                brwAssignment.CoreWebView2.NewWindowRequested += CoreWebView2_NewWindowRequested;
            }
        }

        private void CoreWebView2_NewWindowRequested(object? sender, Microsoft.Web.WebView2.Core.CoreWebView2NewWindowRequestedEventArgs e)
        {
            // Prevent WebView2 from opening a new window
            e.Handled = true;

            // Open in the system's default browser (Edge)
            Process.Start(new ProcessStartInfo
            {
                FileName = e.Uri,
                UseShellExecute = true
            });
        }

        private void BrwAssignment_NavigationCompleted(object? sender, Microsoft.Web.WebView2.Core.CoreWebView2NavigationCompletedEventArgs e)
        {
            if (e.IsSuccess)
            {
                _initialLoadCompleted = true;
            }
        }

        private void BrwAssignment_NavigationStarting(object? sender, Microsoft.Web.WebView2.Core.CoreWebView2NavigationStartingEventArgs e)
        {
            // Allow the initial page to load in the WebView; only after that, open links externally
            if (!_initialLoadCompleted)
            {
                return;
            }

            // Cancel navigation in WebView2 and open in default browser
            e.Cancel = true;
            Process.Start(new ProcessStartInfo
            {
                FileName = e.Uri,
                UseShellExecute = true
            });
        }
    }
}
