using CybersecurityChatbot.Pages;
using CybersecurityChatbot.Services;
using System.IO;
using System.Windows;

namespace CybersecurityChatbot;

public partial class MainWindow : Window
{
    private readonly AppState _state = new();
    public string AsciiArt { get; } = "";
    private readonly ChatPage _chatPage;

    public MainWindow()
    {
        InitializeComponent();
        DataContext = this;
        string asciiPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Media", "ascii.txt");
        AsciiArt = File.Exists(asciiPath) ? File.ReadAllText(asciiPath) : "ASCII missing";
        _state.Load();
        _chatPage = new ChatPage(_state);
        _chatPage.NavigationRequested += NavigateTo;
        NavigateTo("home");
    }

    private void NavigateTo(string target)
    {
        _state.Add("navigation", target);
        MainFrame.Navigate(target switch
        {
            "home" => new HomePage(),
            "chat" => _chatPage,
            "quiz" => new QuizPage(),
            "game" => new GamePage(),
            "scheduler" => new SchedulerPage(),
            "history" => new HistoryPage(_state),
            "settings" => new SettingsPage(),
            _ => new HomePage()
        });
    }

    private void HomeNav_Click(object sender, RoutedEventArgs e) => NavigateTo("home");
    private void ChatNav_Click(object sender, RoutedEventArgs e) => NavigateTo("chat");
    private void QuizNav_Click(object sender, RoutedEventArgs e) => NavigateTo("quiz");
    private void GameNav_Click(object sender, RoutedEventArgs e) => NavigateTo("game");
    private void SchedulerNav_Click(object sender, RoutedEventArgs e) => NavigateTo("scheduler");
    private void HistoryNav_Click(object sender, RoutedEventArgs e) => NavigateTo("history");
    private void SettingsNav_Click(object sender, RoutedEventArgs e) => NavigateTo("settings");
}
