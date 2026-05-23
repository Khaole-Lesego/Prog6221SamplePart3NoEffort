using System;

namespace CybersecurityChatbot;

// I created this class to represent a single chat message. I use it as a data container
// that the XAML DataTemplate can bind to directly, so I don't need any custom converters
// and the XAML stays clean.
// I made the properties immutable (init-only) so messages can't be accidentally modified after creation.
public class ChatMessage
{
    // Who sent this — "You" for user, "Lupus" for bot. I use a string instead of an enum
    // because it's simpler to bind in XAML and display directly.
    public string Sender { get; init; } = string.Empty;

    // The actual message text that gets displayed.
    public string Text { get; init; } = string.Empty;

    // True if the human sent it, false if the bot did. I use this flag so the XAML template
    // selector can pick the right visual style (user bubbles align right, bot bubbles align left).
    public bool IsUser { get; init; }

    // A timestamp captured when the message object is created. I format it as HH:mm
    // so it displays like "14:23" without date information (keeps the UI clean).
    public string Time { get; init; } = DateTime.Now.ToString("HH:mm");
}
