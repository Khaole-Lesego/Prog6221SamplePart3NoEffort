using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Media;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace CybersecurityChatbot;

// I built this window to be the main UI for the chatbot. Here's what I do:
// - Load resources (logo, ASCII art, greeting audio) when the app starts
// - Display the conversation as an observable collection so the UI updates automatically
// - Handle user input (keyboard and button clicks)
// - Show a typing indicator while the bot is "thinking" to make the chat feel more natural
// - Manage async message processing so the UI doesn't freeze
// - Keep the memory panel updated so users can see what I remember about them
public partial class MainWindow : Window
{
    private readonly Chatbot _chatbot = new();

    // I use ObservableCollection instead of a regular List because it automatically 
    // notifies the ItemsControl when items are added, so I don't have to manually refresh the UI.
    private readonly ObservableCollection<ChatMessage> _messages = new();

    // I use this flag to prevent the user from sending multiple messages while the bot 
    // is still processing one — this avoids weird timing issues where responses could arrive out of order.
    private bool _isBotResponding = false;

    public MainWindow()
    {
        InitializeComponent();

        // I bind the observable collection to the ItemsControl in XAML so they stay in sync.
        ChatItemsControl.ItemsSource = _messages;

        Loaded += MainWindow_Loaded;
    }

    // ─────────────────────────────────────────────────────────────────────────
    //  Startup — loading resources and greeting the user
    // ─────────────────────────────────────────────────────────────────────────

    private void MainWindow_Loaded(object sender, RoutedEventArgs e)
    {
        try
        {
            LoadBannerImage();
            LoadAsciiArt();
            PlayGreeting();

            // I get the opening welcome message from the chatbot (which asks for the user's name)
            // and display it so the conversation starts immediately.
            AddBotMessage(_chatbot.GetWelcomeMessage());
        }
        catch (Exception ex)
        {
            // If anything goes wrong during startup, I show it in the chat instead of crashing.
            // This way startup errors don't kill the whole app — users can still chat.
            AddBotMessage($"⚠️ Startup warning: {ex.Message}");
        }
    }

    private void LoadBannerImage()
    {
        // I build the path relative to the app's folder so it works whether I'm in Visual Studio or deployed.
        string bannerPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images", "lupuschatlogo.jpg");
        if (!File.Exists(bannerPath))
        {
            // The banner is nice to have but not essential — if it's missing, I just skip it.
            return;
        }
        BannerImage.Source = new BitmapImage(new Uri(bannerPath));
    }

    private void LoadAsciiArt()
    {
        // I load the ASCII art if it exists. If it doesn't, I show a placeholder message.
        // I use a ternary so the code is concise without sacrificing readability.
        string asciiPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Media", "ascii.txt");
        AsciiTextBox.Text = File.Exists(asciiPath)
            ? File.ReadAllText(asciiPath)
            : "[ ascii.txt not found — place it in the Media folder ]";
    }

    private void PlayGreeting()
    {
        try
        {
            string audioPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Media", "greeting.wav");
            if (!File.Exists(audioPath))
                return; // Audio is optional — missing it isn't an error.

            // SoundPlayer.Play() is asynchronous, so the UI stays responsive while audio plays.
            // I use 'using' to make sure it gets cleaned up properly after playback.
            using SoundPlayer player = new(audioPath);
            player.Play();
        }
        catch (Exception ex)
        {
            // Audio is a nice touch but never critical. I log it softly so users know but aren't alarmed.
            AddBotMessage($"🔇 Could not play greeting audio: {ex.Message}");
        }
    }

    // ─────────────────────────────────────────────────────────────────────────
    //  Input handling — button and keyboard events
    // ─────────────────────────────────────────────────────────────────────────

    private void SendButton_Click(object sender, RoutedEventArgs e) =>
        _ = ProcessMessageAsync();

    private void UserInputTextBox_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter)
        {
            // I set Handled to true so the TextBox doesn't treat Enter as a newline character.
            // This gives the user a natural chat-app experience where Enter always sends.
            e.Handled = true;
            _ = ProcessMessageAsync();
        }
    }

    // I handle the quick-topic pill buttons. Each button's Tag contains a message text
    // (like "Tell me about passwords"), so clicking a pill is the same as typing that message.
    private void QuickTopic_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button btn && btn.Tag is string message)
        {
            UserInputTextBox.Text = message;
            _ = ProcessMessageAsync();
        }
    }

    // ─────────────────────────────────────────────────────────────────────────
    //  Core message pipeline — async processing with UX feedback
    // ─────────────────────────────────────────────────────────────────────────

    // This is my main async pipeline. Here's the flow:
    // 1. Validate input (guard against spam/empty messages)
    // 2. Display what the user typed
    // 3. Show a typing indicator
    // 4. Wait a bit so the UI renders the indicator (350-700ms feels natural, not robotic)
    // 5. Get the response from the Chatbot
    // 6. Hide the indicator and show the response
    // 7. If the bot queued a follow-up, show it after a short pause
    // 8. Update the memory panel so users see what I remember
    private async Task ProcessMessageAsync()
    {
        // I block re-entry here so if the user mashes the button, only one message processes.
        // Without this guard, responses could arrive out of order and confuse the chat history.
        if (_isBotResponding)
            return;

        string input = UserInputTextBox.Text.Trim();
        if (string.IsNullOrWhiteSpace(input))
            return;

        _isBotResponding = true;
        UserInputTextBox.Clear();
        UserInputTextBox.IsEnabled = false;
        SendButton.IsEnabled = false;

        try
        {
            // Step 1: Show the user's message immediately so they know I got it.
            AddUserMessage(input);

            // Step 2: Show a typing indicator so the user knows I'm "thinking".
            ShowTypingIndicator();

            // Step 3: Async delay. I use Random(350, 700) because a fixed delay feels mechanical.
            // This allows the UI to render the indicator before the response pops up.
            await Task.Delay(350 + new Random().Next(350));

            // Step 4: Get the main response from the chatbot.
            string response = _chatbot.GenerateResponse(input);

            // Step 5: Hide indicator and show the bot's response.
            HideTypingIndicator();
            AddBotMessage(response);

            // Step 6: If the chatbot queued a follow-up (e.g., after detecting the user is worried),
            // show it with another typing pause so it doesn't feel like they both arrived at once.
            if (_chatbot.HasPendingFollowUp)
            {
                ShowTypingIndicator();
                await Task.Delay(800);
                HideTypingIndicator();
                AddBotMessage(_chatbot.ConsumePendingFollowUp());
            }

            // Step 7: Refresh the memory display so users can see what I learned about them.
            RefreshMemoryPanel();
        }
        catch (Exception ex)
        {
            HideTypingIndicator();
            AddBotMessage($"⚠️ Unexpected error: {ex.Message}");
        }
        finally
        {
            // Always clean up: unlock input and restore focus, even if an error occurred.
            _isBotResponding = false;
            UserInputTextBox.IsEnabled = true;
            SendButton.IsEnabled = true;
            UserInputTextBox.Focus();
        }
    }

    // ─────────────────────────────────────────────────────────────────────────
    //  UI helpers — messages, typing indicator, scroll, memory panel
    // ─────────────────────────────────────────────────────────────────────────

    private void AddUserMessage(string text)
    {
        // I create a ChatMessage object and add it to the observable collection.
        // The ItemsControl automatically renders it using the UserMessageTemplate from XAML.
        _messages.Add(new ChatMessage { Sender = "You", Text = text, IsUser = true });
        ScrollToBottom();
    }

    private void AddBotMessage(string text)
    {
        // Same as above, but with IsUser = false so the XAML template renders it as a bot bubble.
        _messages.Add(new ChatMessage { Sender = "Lupus", Text = text, IsUser = false });
        ScrollToBottom();
    }

    private void ShowTypingIndicator()
    {
        TypingBorder.Visibility = Visibility.Visible;
        ScrollToBottom();
    }

    private void HideTypingIndicator() =>
        TypingBorder.Visibility = Visibility.Collapsed;

    private void ScrollToBottom()
    {
        // I defer the scroll until after the layout pass so the newly added item is actually rendered.
        // If I scroll immediately, the item might not be in the right place yet.
        Dispatcher.InvokeAsync(() =>
        {
            ChatScrollViewer.ScrollToBottom();
        }, System.Windows.Threading.DispatcherPriority.Loaded);
    }

    private void RefreshMemoryPanel()
    {
        // I call a method on the chatbot to get a summary of what I remember about the user.
        // This keeps the memory panel in sync without needing to expose the internal _memories dictionary.
        string summary = _chatbot.GetMemorySummary();
        MemoryDisplay.Text = string.IsNullOrWhiteSpace(summary)
            ? "No user info stored yet."
            : summary;
    }
}
