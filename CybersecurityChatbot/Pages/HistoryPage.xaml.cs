using System.Windows.Controls;
namespace CybersecurityChatbot.Pages;
public partial class HistoryPage : Page
{
    public HistoryPage()
    {
        InitializeComponent();
        BodyText.Text = "Progress and history are recorded with timestamps in activity-log.json for every message and navigation action.";
    }
}
