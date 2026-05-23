using System.Windows;
using System.Windows.Controls;

namespace CybersecurityChatbot;

// I created this class to choose the right visual template for each message.
// WPF has a DataTemplateSelector that lets me pick different templates based on the data.
// Instead of hardcoding the template selection in XAML, I keep it as a C# class so it's:
// - Easy to unit-test (I can instantiate it and call SelectTemplate directly)
// - Decoupled from XAML (if I change the logic, I only edit one place)
// - Reusable in code-behind if needed in the future
public class MessageTemplateSelector : DataTemplateSelector
{
    // The template to use for messages the user sent (shows on the right side).
    public DataTemplate? UserTemplate { get; set; }

    // The template to use for bot messages (shows on the left side).
    public DataTemplate? BotTemplate { get; set; }

    // WPF calls this method for each item in the ItemsControl to ask: "Which template should I use?"
    // I check the IsUser flag on the ChatMessage and return the appropriate template.
    public override DataTemplate? SelectTemplate(object item, DependencyObject container)
    {
        if (item is ChatMessage msg)
            return msg.IsUser ? UserTemplate : BotTemplate;

        return base.SelectTemplate(item, container);
    }
}
