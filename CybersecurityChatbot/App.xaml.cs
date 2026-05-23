using System.Windows;

namespace CybersecurityChatbot;

// This is the entry point for the whole app. I don't really do much in the code here
// because most of the setup happens automatically. The MainWindow.xaml file tells WPF
// to open MainWindow when the app starts, and then all the loading of images, audio,
// and chat setup happens in MainWindow's Loaded event. This class is mostly just here
// because WPF expects it to exist.
public partial class App : Application
{
}
