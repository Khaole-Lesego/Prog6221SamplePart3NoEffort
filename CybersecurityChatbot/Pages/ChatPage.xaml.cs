using CybersecurityChatbot.Services;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace CybersecurityChatbot.Pages;

public partial class ChatPage : Page
{
    private readonly Chatbot _chatbot = new();
    private readonly ObservableCollection<ChatMessage> _messages = new();
    private readonly AppState _state;
    private string? _pendingTarget;
    private bool _busy;

    public event Action<string>? NavigationRequested;

    public ChatPage(AppState state)
    {
        InitializeComponent();
        _state = state;
        Messages.ItemsSource = _messages;
        AddBot(_chatbot.GetWelcomeMessage());
    }

    private void Send_Click(object sender, RoutedEventArgs e) => _ = ProcessAsync();
    private void Input_KeyDown(object sender, KeyEventArgs e) { if (e.Key == Key.Enter) { e.Handled = true; _ = ProcessAsync(); } }

    // I keep one message pipeline so responses stay in order and the chat feels intentional.
    private async Task ProcessAsync()
    {
        if (_busy) return;
        var text = Input.Text.Trim();
        if (string.IsNullOrWhiteSpace(text)) return;

        _busy = true;
        AddUser(text);
        _state.Add("chat_message", text);
        Input.Clear();

        TypingText.Visibility = Visibility.Visible;
        await Task.Delay(250);

        if (_pendingTarget != null)
        {
            var low = text.ToLowerInvariant();
            if (low is "yes" or "y" or "confirm" or "sure")
            {
                AddBot($"Awesome — navigating to {_pendingTarget} now.");
                _state.Add("navigation_confirmed", _pendingTarget);
                NavigationRequested?.Invoke(_pendingTarget);
                _pendingTarget = null;
                TypingText.Visibility = Visibility.Collapsed;
                _busy = false;
                return;
            }

            if (low is "no" or "n" or "cancel")
            {
                AddBot("No stress — we can stay right here.");
                _state.Add("navigation_cancelled", "user cancelled");
                _pendingTarget = null;
                TypingText.Visibility = Visibility.Collapsed;
                _busy = false;
                return;
            }
        }

        var target = NavigationIntentService.DetectTarget(text);
        if (target != null)
        {
            _pendingTarget = target;
            AddBot($"Are you sure you want me to open the {target} page? Reply yes or no.");
            _state.Add("navigation_offer", target);
        }
        else
        {
            AddBot(_chatbot.GenerateResponse(text));
            if (_chatbot.HasPendingFollowUp)
            {
                await Task.Delay(450);
                AddBot(_chatbot.ConsumePendingFollowUp());
            }
        }

        TypingText.Visibility = Visibility.Collapsed;
        _busy = false;
    }

    private void AddUser(string text) { _messages.Add(new ChatMessage { IsUser = true, Sender = "You", Text = text }); Scroll(); }
    private void AddBot(string text) { _messages.Add(new ChatMessage { IsUser = false, Sender = "Lupus", Text = text }); Scroll(); }
    private void Scroll() => Dispatcher.InvokeAsync(() => ChatScrollViewer.ScrollToBottom());
}
