using CybersecurityChatbot.Services;
using System.Windows.Controls;

namespace CybersecurityChatbot.Pages;
public partial class HistoryPage : Page
{
    public HistoryPage(AppState state)
    {
        InitializeComponent();
        state.Load();
        LogGrid.ItemsSource = state.Logs.OrderByDescending(x => x.StartTime).ToList();
    }
}
