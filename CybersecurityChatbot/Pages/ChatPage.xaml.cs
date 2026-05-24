using CybersecurityChatbot.Services;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace CybersecurityChatbot.Pages;

public partial class ChatPage : Page
{
    private string? _pendingTarget;
    public event Action<string>? NavigationRequested;
    private readonly AppState _state;

    public ChatPage(AppState state)
    {
        InitializeComponent();
        _state = state;
        Messages.Items.Add("🐺 Lupus: Hi! Ask me anything, including 'take me to the quiz'.");
    }

    private void Send_Click(object sender, RoutedEventArgs e) => Process();
    private void Input_KeyDown(object sender, KeyEventArgs e) { if (e.Key == Key.Enter) Process(); }

    private void Process()
    {
        var text = Input.Text.Trim(); if (string.IsNullOrWhiteSpace(text)) return;
        Messages.Items.Add("You: " + text);
        _state.Add("chat_message", text);
        Input.Clear();

        if (_pendingTarget != null)
        {
            var low = text.ToLowerInvariant();
            if (low is "yes" or "y" or "confirm" or "sure")
            {
                Messages.Items.Add($"🐺 Lupus: Perfect, opening {_pendingTarget} now.");
                _state.Add("navigation_confirmed", _pendingTarget);
                NavigationRequested?.Invoke(_pendingTarget);
                _pendingTarget = null;
                return;
            }
            if (low is "no" or "n" or "cancel")
            {
                Messages.Items.Add("🐺 Lupus: No problem, I'll keep you here.");
                _state.Add("navigation_cancelled", "user cancelled");
                _pendingTarget = null;
                return;
            }
        }

        var target = NavigationIntentService.DetectTarget(text);
        if (target != null)
        {
            _pendingTarget = target;
            Messages.Items.Add($"🐺 Lupus: Are you sure you want to open the {target} page? Reply yes or no.");
            return;
        }

        Messages.Items.Add("🐺 Lupus: Got it. I can navigate, track activity, and help with quiz/game/scheduler flows.");
    }
}
