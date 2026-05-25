namespace CybersecurityChatbot.Services;

public static class NavigationIntentService
{
    public static string? DetectTarget(string text)
    {
        string input = text.ToLowerInvariant();
        if (ContainsAny(input, "home", "main menu", "start page")) return "home";
        if (ContainsAny(input, "chat", "chatbot", "assistant", "talk")) return "chat";
        if (ContainsAny(input, "quiz", "test me", "its time to quiz", "questions")) return "quiz";
        if (ContainsAny(input, "memory game", "play game", "play a game", "game time")) return "game";
        if (ContainsAny(input, "schedule", "scheduler", "task", "reminder")) return "scheduler";
        if (ContainsAny(input, "history", "progress", "show my progress", "activity")) return "history";
        if (ContainsAny(input, "settings", "preferences")) return "settings";
        return null;
    }

    private static bool ContainsAny(string text, params string[] phrases) => phrases.Any(text.Contains);
}
