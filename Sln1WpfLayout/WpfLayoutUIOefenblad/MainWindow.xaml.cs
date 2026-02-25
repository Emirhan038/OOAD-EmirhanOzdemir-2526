using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using WpfLayoutUIOefenblad.Assignments;
using WpfLayoutUIOefenblad.Helpers;

namespace WpfLayoutUIOefenblad;

public partial class MainWindow : Window
{
    private List<NavPageInfo> navItems = new List<NavPageInfo>();

    public MainWindow()
    {
        InitializeComponent();
        this.Loaded += this.MainWindow_Loaded;
        fraMain.Navigated += FraMain_Navigated;
    }

    private void FraMain_Navigated(object sender, System.Windows.Navigation.NavigationEventArgs e)
    {
        if (e.Content is not Page page)
        {
            return;
        }

        // Wacht tot de Page volledig is geladen voordat we de visual tree doorzoeken
        page.Loaded += (s, args) =>
        {
            // Zoek het fraAssignment Frame in de geladen pagina
            Frame? assignmentFrame = FindChild<Frame>(page, "fraAssignment");
            if (assignmentFrame != null)
            {
                // Bepaal de bestandsnaam op basis van de pagina class naam
                string fileName = page.GetType().Name;
                assignmentFrame.Navigate(new AssignmentViewer(fileName));
            }
        };
    }

    private static T? FindChild<T>(DependencyObject parent, string childName) where T : DependencyObject
    {
        if (parent == null)
        {
            return null;
        }

        int childrenCount = VisualTreeHelper.GetChildrenCount(parent);
        for (int i = 0; i < childrenCount; i++)
        {
            DependencyObject child = VisualTreeHelper.GetChild(parent, i);

            if (child is T typedChild && child is FrameworkElement element && element.Name == childName)
            {
                return typedChild;
            }

            T? foundChild = FindChild<T>(child, childName);
            if (foundChild != null)
            {
                return foundChild;
            }
        }

        return null;
    }

    private void MainWindow_Loaded(object? sender, RoutedEventArgs e)
    {
        this.navItems = DiscoverNavPages();
        lstNav.ItemsSource = this.navItems;

        int lastOrder = Properties.Settings.Default.LastSelectedExercise;

        int index = this.navItems.FindIndex(x => x.Order == lastOrder);
        if (index < 0)
        {
            index = 0;
        }

        lstNav.SelectedIndex = index;
    }

    private static List<NavPageInfo> DiscoverNavPages()
    {
        Assembly assembly = Assembly.GetExecutingAssembly();

        List<NavPageInfo> items =
            assembly
                .GetTypes()
                .Where(t => t.IsClass && !t.IsAbstract)
                .Where(t => typeof(Page).IsAssignableFrom(t))
                .Select(t => new
                {
                    Type = t,
                    Attr = t.GetCustomAttribute<NavPageAttribute>()
                })
                .Where(x => x.Attr != null)
                .Select(x => new NavPageInfo
                {
                    Title = x.Attr!.Title,
                    Description = x.Attr!.Description,
                    Order = x.Attr!.Order,
                    IsVisible = x.Attr!.IsVisible,
                    PageType = x.Type
                })
                .Where(x => x.IsVisible)
                .OrderBy(x => x.Order)
                .ThenBy(x => x.Title)
                .ToList();

        for (int i = 1; i < items.Count; i++)
        {
            items[i] = new NavPageInfo
            {
                Title = $"{i}. {items[i].Title}",
                Description = items[i].Description,
                Order = items[i].Order,
                IsVisible = items[i].IsVisible,
                PageType = items[i].PageType
            };
        }

        return items;
    }

    private void UpdateNavButtons()
    {
        btnPrev.IsEnabled = lstNav.SelectedIndex > 0;
        btnNext.IsEnabled = lstNav.SelectedIndex < lstNav.Items.Count - 1;
    }

    private void lstNav_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (lstNav.SelectedItem is not NavPageInfo navPageInfo)
        {
            return;
        }

        Page? page = (Page?)Activator.CreateInstance(navPageInfo.PageType);
        if (page != null)
        {
            fraMain.Navigate(page);
        }

        Properties.Settings.Default.LastSelectedExercise = navPageInfo.Order;
        Properties.Settings.Default.Save();

        UpdateNavButtons();
    }

    private void btnPrev_Click(object sender, RoutedEventArgs e)
    {
        if (lstNav.SelectedIndex > 0)
        {
            lstNav.SelectedIndex--;
        }
    }

    private void btnNext_Click(object sender, RoutedEventArgs e)
    {
        if (lstNav.SelectedIndex < lstNav.Items.Count - 1)
        {
            lstNav.SelectedIndex++;
        }
    }

}
