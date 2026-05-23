# Cybersecurity Awareness Chatbot – CipherGuard Lupus

Welcome! This is my **CipherGuard Lupus** cybersecurity awareness chatbot, built as part of my Programming 2 POE.  
What started as a console application in Part 1 has now grown into a fully-fledged **Graphical User Interface (GUI) application** in Part 2, built using **Windows Presentation Foundation (WPF)**.  
The chatbot greets you with a voice, displays an ASCII logo, asks your name, and chats with you about online safety — covering topics like passwords, phishing, scams, privacy, malware, VPNs, and more.  

Below you'll find everything you need to run both parts, a behind‑the‑scenes look at how I built them, the problems I ran into along the way, and how I eventually solved them.

---

## Table of Contents
- [How to Run — Part 1 (Console)](#how-to-run--part-1-console)
- [Part 1 — Project Overview](#-part-1--project-overview)
- [Part 1 — My Development Journey](#️-part-1--my-development-journey)
  - [Errors I Encountered & How I Fixed Them](#errors-i-encountered--how-i-fixed-them)
  - [Multimedia Files](#multimedia-files)
  - [GitHub & CI Setup](#github--ci-setup)
- [How to Run — Part 2 (GUI)](#how-to-run--part-2-gui)
- [Part 2 — Project Overview](#-part-2--project-overview)
- [Part 2 — From Console to GUI: What Changed and Why](#️-part-2--from-console-to-gui-what-changed-and-why)
  - [Project Structure Changes](#project-structure-changes)
  - [The chatbot.cs Overhaul](#the-chatbotcs-overhaul)
  - [The GUI Layer: MainWindow.xaml & MainWindow.xaml.cs](#the-gui-layer-mainwindowxaml--mainwindowxamlcs)
  - [Emojis in the Project](#emojis-in-the-project)
  - [Errors I Encountered in Part 2 & How I Fixed Them](#errors-i-encountered-in-part-2--how-i-fixed-them)
  - [GitHub & Releases](#github--releases)
- [Resources Used](#-resources-used)
- [Harvard Reference List](#-harvard-reference-list)

---

## How to Run — Part 1 (Console)

1. **Clone the repository**  
   Open a terminal and run:  
   `git clone https://github.com/IIEMSA/prog6221-poe-Khaole-Lesego.git`

2. **Open the solution**  
   Double‑click `CybersecurityChatbot.slnx` in the cloned folder. This will launch Visual Studio.

3. **Build and run**  
   Press **F5** (or click the green **Start** button).  
   The program will:
   - Play a voice greeting (you'll hear "Lupus online…").
   - Display the ASCII art logo.
   - Ask for your name.
   - Start a conversation.

4. **What to type**  
   - Try: `how are you`, `what is phishing`, `password tips`, `safe browsing`, `who are you`, `what is cipher`.  
   - To quit: type `exit` or `quit`.

> **Note:** The Media folder (`Media\greeting.wav` and `Media\ascii.txt`) must be present in the build output – they are automatically copied because I set **Copy to Output Directory = Copy if newer**.

---

## Part 1 — Project Overview

This is a **command‑line chatbot** that meets all Part 1 requirements:

- Voice greeting (`.wav` file)  
- ASCII art logo (loaded from `ascii.txt`)  
-  Personalised greeting (asks for name, uses it)  
- Basic responses for:
  - `how are you`  
  - `purpose` / `what can you do`  
  - `phishing`  
  - `password`  
  - `safe browsing`  
  - `lupus` / `who are you`  
  - `cipher`
-  Random responses (each topic has multiple replies, chosen randomly)  
-  Input validation (empty input triggers a helpful message)  
-  Enhanced console UI (coloured text, headers, separators, typing effect)  
-  Clean code structure (`UIHelper.cs`, `Chatbot.cs`, `Program.cs`)  
-  Git with 6+ meaningful commits  
-  CI workflow (GitHub Actions) that builds and tests the project automatically

  <img width="1919" height="1079" alt="image" src="https://github.com/user-attachments/assets/19baaae5-6fa2-46d1-bce8-290857a72a0d" />
  Workflow Screenshot


---

##  Part 1 — My Development Journey

I started with absolutely no coding experience. I followed step‑by‑step instructions, asked a lot of questions, and learned by doing. Here's a look at how it all came together.

### Errors I Encountered & How I Fixed Them

| Error | What caused it | How I fixed it |
|-------|----------------|----------------|
| **`SoundPlayer` not found** | I was using modern .NET (10.0) which doesn't include `System.Media` by default. | I installed the NuGet package `System.Windows.Extensions` (right‑click project → Manage NuGet Packages → browse → install). |
| **Nullable warnings (CS8618)** | The `userName` field wasn't initialised when the class was created. | I declared it as `private string? userName;` (the `?` tells the compiler it may be null). Then in `GreetUser`, I made sure it gets a value before use. |
| **Console window properties only on Windows** | `Console.WindowWidth`, `BufferWidth`, and `WindowHeight` are not supported on Linux/macOS. | I wrapped them in a `#if WINDOWS` preprocessor directive. Now they only run when the target is Windows (the default for my project). |
| **CI workflow failed with .NET version mismatch** | My project targeted .NET 10.0, but the workflow specified `dotnet-version: '9.0.x'`. | I changed the workflow file to `dotnet-version: '10.0.x'`. |
| **ASCII art didn't display (file not found)** | I hadn't set **Copy to Output Directory = Copy if newer** for the media files. | I right‑clicked each file in Solution Explorer → Properties → set **Copy to Output Directory** to **Copy if newer**. |
| **Folder structure was messy** | I accidentally placed the solution inside a subfolder, and the `.github` folder was inside the project folder. | I moved everything to the root (where `.git` lives). The solution now sits directly in the root, and `.github` is also at the root. |

### Multimedia Files

- **`greeting.wav`** – I recorded my voice using the Windows Voice Recorder app, then used an online converter to turn the `.m4a` into a WAV file. The file is saved in the `Media` folder.  
- **`ascii.txt`** – I designed a custom logo using [FontB's ASCII generator](https://www.fontb.com/ascii-generator) and [ManyTools' image‑to‑ASCII converter](https://manytools.org/hacker-tools/convert-images-to-ascii-art/). I then pasted the result into a text file and added decorative borders.  

### GitHub & CI Setup

I made sure to:

- Commit **at least 6 times** with clear messages (e.g., "Add UIHelper class", "Add Chatbot class", "Add media files", etc.).  
- Set up a **GitHub Actions workflow** (`.github/workflows/build.yml`) that:
  - Restores NuGet packages.
  - Builds the project.
  - Runs tests (there are no tests yet, but the workflow is ready for them).
- Verified the CI passes with a **green check** every time I pushed.


### Here is my Cipher Guard Lupus WalkThrough Youtube Video Link:

- **`CipherGuard Lupus CyberSecurity ChatBot`** – [CipherGuard Lupus CyberSecurity ChatBot](https://youtu.be/L_M070q_SH4) 

---

##  How to Run — Part 2 (GUI)

> **Important:** Part 2 is a **WPF (Windows Presentation Foundation) application**. It will only run on **Windows**. If you are on macOS or Linux, you will need a Windows machine or a Windows virtual machine to run it.

1. **Clone the repository** (if you haven't already)  
   `git clone https://github.com/IIEMSA/prog6221-poe-Khaole-Lesego.git`

2. **Open the solution**  
   Double‑click `CybersecurityChatbot.slnx` in the cloned folder. Visual Studio will open it automatically.

3. **Restore NuGet packages** (usually automatic)  
   If Visual Studio prompts you, click **Restore**. Alternatively, right‑click the solution in Solution Explorer → **Restore NuGet Packages**.

4. **Build and run**  
   Press **F5** (or click the green **Start** button).  
   The GUI window will open and:
   - The voice greeting plays automatically in the background.
   - The banner logo loads at the top.
   - The ASCII art loads in the left panel.
   - The bot greets you in the chat window and asks for your name.
   - You type in the input box at the bottom and press **Enter** or click **Send**.

5. **Things to try**  
   - Tell it your name: `my name is [your name]`  
   - Ask about cybersecurity topics: `password tips`, `what is phishing`, `tell me about scams`, `privacy advice`, `what is a VPN`  
   - Ask how the bot is doing: `how are you`  
   - Express how you feel: `I'm worried about online scams`, `I'm curious about malware`  
   - Ask for more: `tell me more`, `another tip`, `explain more`  
   - Ask what you've discussed: `what have we discussed`, `show activity log`  

> **Note:** The `Media` folder (`greeting.wav`, `ascii.txt`) and the `Images` folder (`lupuschatlogo.jpg`) must be present in the build output directory. All three files are set to **Copy to Output Directory = Preserve Newest** in the `.csproj` file, so they copy automatically when you build.

---

##  Part 2 — Project Overview

Part 2 is a **full WPF GUI application** that carries forward everything from Part 1 and adds a significant layer of intelligence, interactivity, and visual polish. Here is a summary of what it delivers:

- ✅ **GUI built with WPF/XAML** — full graphical interface with no console window  
- ✅ **Voice greeting** carried over from Part 1, now playing on window load  
- ✅ **ASCII art** carried over from Part 1, displayed in a dedicated left panel  
- ✅ **Banner logo image** (`lupuschatlogo.jpg`) displayed at the top of the window  
- ✅ **Proactive name prompt** — bot asks for your name on startup instead of waiting for you to volunteer it  
- ✅ **Keyword recognition** — 12+ cybersecurity topics with synonym mapping (e.g., `fraud` → scam, `passphrase` → password)  
- ✅ **Random responses** — each topic has multiple responses selected randomly via `Dictionary<string, List<string>>`  
- ✅ **Conversation flow** — follow-up phrases like `tell me more`, `another tip`, and `explain more` continue the last topic seamlessly  
- ✅ **Memory & recall** — the bot remembers your name, what you're worried about, and what you're interested in, and references them later  
- ✅ **Sentiment detection** — detects worried, curious, and frustrated tones and adjusts its response accordingly  
- ✅ **Automatic tip delivery after sentiment** — if you express a feeling with no specific topic keyword, the bot delivers a relevant tip automatically rather than waiting  
- ✅ **Activity summary** — typing `what have we discussed` or `show activity log` gives you a summary of the topics covered  
- ✅ **Input validation and error handling** — empty input is handled gracefully, and all file loading is wrapped in try-catch so the app never crashes on missing files  
- ✅ **Clean OOP code structure** — `Chatbot.cs` handles all conversation logic; `MainWindow.xaml.cs` handles only UI events; clear separation of concerns throughout  
- ✅ **6+ meaningful GitHub commits** with descriptive messages  
- ✅ **GitHub releases with tags** (minimum of 2)  
- ✅ **CI workflow** (GitHub Actions) confirmed green on every push  

---

## 🛠️ Part 2 — From Console to GUI: What Changed and Why

This is the section I'm most proud of, because moving from a console app to a WPF GUI was genuinely challenging. It wasn't just a visual makeover — almost everything under the hood had to change. Here's the full story.

### Project Structure Changes

The first thing that had to change was the project file itself (`CybersecurityChatbot.csproj`). The Part 1 project was configured as a standard console executable. For Part 2 I had to make three key changes:

| Setting | Part 1 | Part 2 | Why |
|---------|--------|--------|-----|
| `OutputType` | `Exe` | `WinExe` | `WinExe` suppresses the console window so only the GUI appears |
| `TargetFramework` | `net10.0` | `net10.0-windows` | WPF is a Windows-only framework and requires the `-windows` suffix |
| `UseWPF` | *(absent)* | `true` | Enables the WPF toolset, XAML compilation, and WPF-specific controls |

A new `Images` folder was also added to the project to hold `lupuschatlogo.jpg`, with its own `CopyToOutputDirectory` entry so it copies to the build folder automatically just like the media files.

The files that were **removed** in Part 2 are `Program.cs` and `UIHelper.cs`. In a WPF application there is no console window, so there is no need for `Console.ForegroundColor`, `Console.Write`, or `Thread.Sleep` typing effects. The `Main` method that previously lived in `Program.cs` is now replaced entirely by `App.xaml` and `App.xaml.cs`, which are the standard WPF application entry points. The work that `UIHelper.cs` used to do — formatting output, setting colours, printing headers — is now handled through XAML styles, `TextBlock` elements, colour values in the markup, and the structured layout of the window itself.

The files that were **added** in Part 2 are:

- `App.xaml` and `App.xaml.cs` — the WPF application entry point  
- `MainWindow.xaml` — the XAML layout describing the full GUI  
- `MainWindow.xaml.cs` — the code-behind that handles UI events and connects to the chatbot  
- `Images/lupuschatlogo.jpg` — the banner image displayed at the top of the window  
- `Images/.gitkeep` — an empty placeholder file so the Images folder is tracked by Git even when the image is not committed  

### The chatbot.cs Overhaul

The Part 1 `chatbot.cs` was a solid foundation, but it was built specifically around the console experience. The `PlayGreeting`, `DisplayAsciiArt`, `GreetUser`, and `StartConversation` methods all used `Console.ReadLine`, `Console.WriteLine`, and `UIHelper` — none of which exist in a WPF application. For Part 2, the entire `chatbot.cs` was rebuilt from scratch to be a **pure conversation engine** with no UI dependencies at all. It receives a string, processes it, and returns a string. The GUI layer handles everything else.

Here is what changed specifically inside `chatbot.cs`:

**Response storage moved from `string[]` to `List<string>`**  
In Part 1 I used `Dictionary<string, string[]>` to store responses. In Part 2 I switched to `Dictionary<string, List<string>>`. A `List` is more flexible than an array — I can add or remove responses at runtime without knowing the size upfront, which makes the chatbot much easier to expand for Part 3.

**Keyword detection moved from if-else chains to a keyword-to-topic Dictionary**  
Part 1 used a sequence of `if (input.Contains("phishing"))` checks inside a single `GetResponse` method. Part 2 introduces a dedicated `_keywordToTopic` dictionary that maps individual keywords and multi-word phrases to topic names. This makes the logic data-driven rather than hard-coded: to add a new keyword I just add a line to the dictionary instead of touching control flow. Multi-word phrases (like `"safe browsing"` or `"two factor"`) are placed at the top of the dictionary so they are matched before shorter single-word entries, which prevents partial mis-matches.

**The topic library expanded from 7 to 12+ topics**  
Part 1 covered `how are you`, `purpose`, `phishing`, `password`, `browsing`, `lupus`, and `cipher`. Part 2 adds `scam`, `privacy`, `malware`, `VPN`, `2FA`, `social engineering`, `ransomware`, `data breach`, and `identity theft` — each with multiple varied responses. Conversational responses for `thanks`, `farewell`, and `what can you do` were also added so the bot handles polite social exchanges naturally.

**Memory and recall added via a `_memories` Dictionary**  
The bot now uses `Regex` patterns to extract facts from what the user says and stores them in a `Dictionary<string, string>`. It captures the user's name from phrases like `"my name is Alex"`, their concern from phrases like `"I'm worried about online scams"`, and their interest from phrases like `"I love learning about privacy"`. These facts are referenced later in responses to make the conversation feel genuinely personalised rather than generic.

**Sentiment detection added via a `_sentimentKeywords` Dictionary**  
The bot detects three emotional states — worried, curious, and frustrated — by scanning the user's input for a list of associated keywords. The key design choice here is that these keyword lists live in a dictionary rather than being hard-coded into if-statements. This makes it trivial to add new sentiments or new keywords for existing ones. When a sentiment is detected, a tailored prefix is prepended to the response (e.g., `"I completely understand — and you're right to take this seriously."` for a worried user).

**Automatic tip delivery when sentiment has no matching topic**  
This was a specific Part 2 requirement that I had to think carefully about. In the original version, if someone typed `"I'm frustrated"` with no topic keyword, the sentiment was detected but then the bot fell through to the default fallback message, which felt cold and unhelpful. Part 2 adds a dedicated `_sentimentAutoTips` dictionary that maps each sentiment to a standalone tip. If sentiment is detected but no topic keyword is found, this tip is returned automatically — no second input required from the user.

**Follow-up handling improved**  
Part 1 had no follow-up detection at all. Part 2 stores the last matched topic in `_lastTopic` and checks for follow-up phrases (`"tell me more"`, `"another tip"`, `"explain more"`, `"continue"`, `"go on"`, and several others) before attempting any keyword match. If a follow-up is detected and a last topic exists, the bot returns a new random response from that topic's response list — so the conversation can stay on one subject naturally for as long as the user wants.

**Topic visit tracking and activity summary added**  
Every time a cybersecurity topic is discussed, a counter is incremented in `_topicVisitCount` and the topic is added to `_topicsExplored` (if not already present). When the user asks `"what have we discussed"` or `"show activity log"`, the bot returns a summary of every topic explored, notes which one was visited most often, and offers an encouraging comment based on how many topics have been covered so far.

**Proactive related-topic suggestions added**  
After responding to a topic, the bot has a 45% chance of suggesting a related topic as a separate follow-up message. This is handled by a `_relatedTopics` dictionary that maps each topic to two or three closely connected ones. The UI layer calls `GetProactiveSuggestion()` after `GenerateResponse()` and, if a suggestion is returned, displays it as a second bot message. This keeps the conversation moving forward organically.

### The GUI Layer: MainWindow.xaml & MainWindow.xaml.cs

The GUI was designed with a dark, professional aesthetic to match the security-focused nature of the chatbot. The colour palette is deep navy (`#111827` for the background, `#0F172A` for the chat panel, `#020617` for the inner surfaces) with soft purple and blue accents. The layout is divided into three rows:

- **Top row (120px fixed height):** the banner image panel — a dark bordered container with rounded corners holding `lupuschatlogo.jpg`  
- **Middle row (fills remaining space):** a two-column layout — the left column (320px wide) holds the ASCII art in its own scrollable panel with a `"Lupus Signature"` label in purple; the right column (fills the rest) holds the chat `ListBox` with a `"CipherGuard Lupus Chat"` heading  
- **Bottom row (auto-height):** the input area — a `TextBox` stretching to fill most of the width, followed by a fixed-width `Send` button in indigo (`#4F46E5`)

The `MainWindow.xaml.cs` code-behind is intentionally kept lean. Its only jobs are to load resources on startup (banner image, ASCII art, voice greeting), add messages to the `ChatListBox`, and pass user input to the chatbot. All conversation intelligence lives in `Chatbot.cs`.

The startup sequence in `MainWindow_Loaded` is:
1. Load and display the banner image from the `Images` folder  
2. Load and display the ASCII art from the `Media` folder into `AsciiTextBox`  
3. Play `greeting.wav` using `SoundPlayer.Play()` (non-blocking, runs in the background)  
4. Display the bot's opening message in the chat  
5. Ask the user for their name as the very first chat message — this was a specific requirement from Part 2 that was missing from the earlier version

The `ProcessMessage` method reads from `UserInputTextBox`, passes the input to `_chatbot.GenerateResponse()`, displays both the user message and the bot response, then clears the input box and returns focus to it so the user can immediately type again. Auto-scroll is handled by `ScrollChatToBottom()`, which calls `ChatListBox.ScrollIntoView(ChatListBox.Items[^1])` after every message.

### Emojis in the Project

One small detail that made a big difference to the feel of the UI: emojis. Rather than using icon libraries or external image assets, I sourced emojis directly from **WhatsApp** — simply copying them from a chat and pasting them straight into the XAML and response strings in Visual Studio. This turned out to be surprisingly effective. The 💡 emoji used in the proactive topic suggestion messages, for example, draws the eye naturally and signals "tip" without any additional styling work. Pasting from WhatsApp meant the emojis render as proper Unicode characters, display correctly in WPF text elements, and require zero extra dependencies. It's a small trick but it genuinely enhances the visual detail and personality of the interface.

### Errors I Encountered in Part 2 & How I Fixed Them

Moving from a console app to a WPF GUI introduced a completely new category of errors. Here is every significant problem I hit and exactly what I did to resolve it.

| Error | What caused it | How I fixed it |
|-------|----------------|----------------|
| **`OutputType` still set to `Exe`** | When I first added the WPF files, the `.csproj` still had `<OutputType>Exe</OutputType>`. This caused a console window to flash open alongside the GUI every time I ran the app. | I changed `OutputType` to `WinExe`. This tells the build system that the entry point is a WPF window, not a console, so no console window appears. |
| **`TargetFramework` missing `-windows` suffix** | I initially tried to run the WPF app with `<TargetFramework>net10.0</TargetFramework>`. The build failed immediately with errors saying WPF types like `Window` and `Application` could not be found. | I changed the framework to `<TargetFramework>net10.0-windows</TargetFramework>` and added `<UseWPF>true</UseWPF>`. WPF is a Windows-only framework and requires both of these settings. |
| **`Program.cs` conflicting with `App.xaml.cs`** | When I added `App.xaml` to the project, I forgot to remove `Program.cs`. This created two entry points and the compiler threw an error saying there was more than one `Main` method. | I deleted `Program.cs` entirely. In a WPF application, `App.xaml` takes over as the entry point and the separate `Main` method is no longer needed or allowed. |
| **`UIHelper.cs` referencing `Console` methods** | I initially tried to keep `UIHelper.cs` in the Part 2 project, thinking I might still use some of its methods. But `Console.ForegroundColor`, `Console.Write`, and `Thread.Sleep` all produced warnings or simply had no effect in a WPF context because there is no console window to write to. | I removed `UIHelper.cs` from the project entirely. All visual formatting in Part 2 is handled through XAML — colours, fonts, spacing, and layout are all declared in `MainWindow.xaml` rather than being applied programmatically at runtime. |
| **`chatbot.cs` missing the `namespace` declaration** | When I rebuilt `chatbot.cs` for Part 2, I initially forgot to add `namespace CybersecurityChatbot;` at the top. This caused `MainWindow.xaml.cs` to throw a `CS0246` error saying the type `Chatbot` could not be found, even though the file was in the same project. | I added the `namespace CybersecurityChatbot;` file-scoped namespace declaration to `chatbot.cs`. All classes in a WPF project need to be in a declared namespace so the XAML compiler and the code-behind can find them. |
| **Banner image not loading at runtime** | The `lupuschatlogo.jpg` file existed in the project folder but the app threw a "file not found" error at runtime. I had added the file to the `Images` folder but had not told the build system to copy it to the output directory. | I added an `<ItemGroup>` entry to the `.csproj` file for the image file: `<None Update="Images\lupuschatlogo.jpg"><CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory></None>`. After this, the file was copied to the `bin\Release\net10.0-windows\` folder automatically on every build. |
| **ASCII art panel reformatting the art** | The ASCII art displayed correctly in the console in Part 1, but when I loaded the same `ascii.txt` into a WPF `TextBox`, the text was wrapping at the panel boundary and completely mangling the logo. | I set `TextWrapping="NoWrap"` and enabled both `HorizontalScrollBarVisibility="Auto"` and `VerticalScrollBarVisibility="Auto"` on the `TextBox`. I also set `FontFamily="Consolas"` so the monospace characters align correctly — ASCII art relies on every character being exactly the same width, which proportional fonts like Segoe UI will break. |
| **`SoundPlayer.PlaySync()` freezing the GUI on startup** | I initially used `PlaySync()` (carried over from Part 1) to play the greeting audio. In a console app this is fine because there is nothing else to display. In WPF it completely froze the GUI window until the audio finished, which looked like the app had crashed. | I changed `PlaySync()` to `Play()`. The `Play()` method runs the audio on a background thread so the GUI remains fully responsive. The window loads and all elements appear instantly while the audio plays behind the scenes. |
| **`ChatListBox` item text appearing cut off** | Early in development I noticed that long bot responses were being cut off at the edge of the `ListBox`. This was because `ListBox` items have no text wrapping by default — they simply overflow and get clipped. | I added an `ItemContainerStyle` with a `ListBoxItem` style that sets `HorizontalContentAlignment="Stretch"` and added a `DataTemplate` using a `TextBlock` with `TextWrapping="Wrap"`. This makes every chat message wrap naturally within the available panel width. |
| **`ChatListBox.ScrollIntoView` throwing a null reference on first message** | When the very first bot message was added on startup, `ScrollChatToBottom()` was called before the `ListBox` had fully rendered its item container, occasionally throwing a null reference exception. | I added a `if (ChatListBox.Items.Count > 0)` guard check before calling `ScrollIntoView`. This ensures the method only runs when there is at least one item present, preventing the null reference entirely. |

### GitHub & Releases

Part 2 introduced a new requirement: **GitHub releases with tags**. In addition to the 6+ commits with meaningful messages carried over from Part 1, Part 2 requires a minimum of **two tagged releases** in the repository.

I created the releases as follows:

- **v1.0.0** — tagged at the final Part 1 console application commit. Release notes summarise the Part 1 features: voice greeting, ASCII art, console UI, basic keyword responses.
- **v2.0.0** — tagged at the first stable Part 2 GUI commit. Release notes summarise the Part 2 features: WPF GUI, expanded chatbot engine, keyword recognition, memory and recall, sentiment detection, and the new banner image.

The CI workflow itself (`build.yml`) did not need to change between Part 1 and Part 2. It was already targeting `windows-latest` as the runner (which is required for WPF), using `dotnet-version: '10.0.x'`, and running `dotnet restore`, `dotnet build`, and `dotnet test` in sequence. Since the project now builds as a WPF app, the same workflow file produces a WPF executable on every push, and the CI green check confirms the build succeeds.

### Here is my Part 2 CipherGuard Lupus GUI WalkThrough YouTube Video Link:

- **`CipherGuard Lupus GUI CyberSecurity ChatBot — Part 2`** – *(https://youtu.be/bMgHA3RMfsU)*

---

## Resources Used

Throughout this project I relied on several resources to understand concepts, fix errors, and create the artwork. This list covers both Part 1 and Part 2.

- **YouTube** – [The video that helped me understand the console typing effect](https://youtu.be/fB8OraROTwk?si=RBJ03JHxw6Gtob_F)  
- **YouTube** – [WPF tutorial that helped me understand XAML layout and data binding concepts](https://www.youtube.com/results?search_query=wpf+tutorial+csharp+beginner)  
- **ChatGPT** – I used it to understand error messages, explain C# and XAML syntax, and get suggestions for code structure. And improve explainations, and structure, and design the content around my README.md  
- **WhatsApp** – Used as an emoji source. I copied emojis directly from WhatsApp and pasted them into XAML and response strings in Visual Studio to enhance the visual detail of the interface.  
- **Textbooks**  
  - Troelsen, A. & Japikse, P., 2022. *Pro C# 10 with .NET 6: Foundational Principles and Practices in Programming*. 11th ed. Berkeley, CA: Apress.  
  - Stellman, A. & Greene, J., 2022. *Head First C#: A Learner's Guide to Real‑World Programming with C# and .NET*. 5th ed. Sebastopol, CA: O'Reilly Media.  
- **Websites**  
  - FontB ASCII Generator – https://www.fontb.com/ascii-generator  
  - ManyTools Image to ASCII Art – https://manytools.org/hacker-tools/convert-images-to-ascii-art/  
  - FontB Story Generator – https://www.fontb.com/story-generator (used for brainstorming the chatbot's voice)  
  - Microsoft Learn — WPF documentation – https://learn.microsoft.com/en-us/dotnet/desktop/wpf/  
  - HaveIBeenPwned – https://haveibeenpwned.com (referenced in chatbot responses)  

---

## Harvard Reference List — Part 2

### 1. Primary Textbook
*Used throughout Part 2 for C# language fundamentals, object‑oriented design, WPF controls, generic collections, delegates, and string manipulation*

Troelsen, A. and Japikse, P. (2022) *Pro C# 10 with .NET 6: Foundational Principles and Practices in Programming*. 11th edn. New York: Apress.

> **Chapters consulted:**  
> – Ch. 12 (Delegates, Events & Lambda Expressions) – delegate requirement  
> – Ch. 10 (Collections & Generics) – `List<T>` and `Dictionary<TKey,TValue>` for keyword responses, random tips, and memory  
> – Chs. 25–28 (Windows Presentation Foundation) – GUI design, XAML, and event handling  
> – Ch. 4 (Core C# Programming) – string methods for keyword recognition and sentiment detection  

---

### 2. Graphical User Interface – WPF / Windows Forms
*Task: Translate Part 1 console features into a GUI application (Question 1)*

Microsoft (2023) ‘WPF overview (.NET 6)’, *Microsoft Learn*. Available at: https://learn.microsoft.com/en-us/dotnet/desktop/wpf/overview/ (Accessed: 5 March 2026).

Microsoft (2024) ‘Windows Forms overview for .NET’, *Microsoft Learn*. Available at: https://learn.microsoft.com/en-us/dotnet/desktop/winforms/overview/ (Accessed: 5 March 2026).

Microsoft (2023) ‘XAML in WPF’, *Microsoft Learn*. Available at: https://learn.microsoft.com/en-us/dotnet/desktop/wpf/xaml/ (Accessed: 6 March 2026).

Microsoft (2024) ‘Layout in WPF’, *Microsoft Learn*. Available at: https://learn.microsoft.com/en-us/dotnet/desktop/wpf/layout/ (Accessed: 6 March 2026).

Microsoft (2024) ‘Styles and templates (WPF .NET)’, *Microsoft Learn*. Available at: https://learn.microsoft.com/en-us/dotnet/desktop/wpf/styles-templates-overview (Accessed: 7 March 2026).

## Tutorial Videos Watched for Learning WPF GUI Design and Coding

1. **WPF Tutorial for C# Beginners (2026)**  
   *[YouTube video]*  
   Available at: [https://www.youtube.com/watch?v=qCoB8mqNyzk&list=PLe_Naf3_gfQl8ZDxkIQpB3-aZJ7aEPFUC](https://www.youtube.com/watch?v=qCoB8mqNyzk&list=PLe_Naf3_gfQl8ZDxkIQpB3-aZJ7aEPFUC)  
   *(Accessed: 28 April 2026)*

2. **C# WPF UI Design Tutorial (2026)**  
   *[YouTube video]*  
   Available at: [https://youtu.be/tUSCm_t6Ypw?si=qWv_azWAIcXHngCv](https://youtu.be/tUSCm_t6Ypw?si=qWv_azWAIcXHngCv)  
   *(Accessed: 28 April 2026)*

---

### 3. Delegates
*Learning outcome: Use delegates to solve a programming problem – event handling in WPF/WinForms*

Microsoft (2024) ‘Delegates (C# programming guide)’, *Microsoft Learn*. Available at: https://learn.microsoft.com/en-us/dotnet/csharp/programming-guide/delegates/ (Accessed: 23 April 2026).

Microsoft (2024) ‘Routed events overview (WPF .NET)’, *Microsoft Learn*. Available at: https://learn.microsoft.com/en-us/dotnet/desktop/wpf/events/routed-events-overview (Accessed: 26 April 2026).

---

### 4. Generic Collections
*Learning outcome: Use a generic collection to solve a programming problem – `Dictionary` for keyword mapping and memory, `List` for random response pools*

Microsoft (2024) ‘Dictionary\<TKey,TValue\> class’, *Microsoft Learn*. Available at: https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.dictionary-2 (Accessed: 20 April2026).

Microsoft (2024) ‘List\<T\> class’, *Microsoft Learn*. Available at: https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.list-1 (Accessed: 20 April 2026).

Microsoft (2024) ‘Collections (C# and Visual Basic)’, *Microsoft Learn*. Available at: https://learn.microsoft.com/en-us/dotnet/csharp/programming-guide/concepts/collections (Accessed: 18 April 2026).

---

### 5. Keyword Recognition (Question 2)
*String methods used to detect cybersecurity‑related keywords in user input*

Microsoft (2024) ‘String.Contains method’, *Microsoft Learn*. Available at: https://learn.microsoft.com/en-us/dotnet/api/system.string.contains (Accessed: 10 April 2026).

Microsoft (2024) ‘String.ToLower method’, *Microsoft Learn*. Available at: https://learn.microsoft.com/en-us/dotnet/api/system.string.tolower (Accessed: 14 pril 2026).

Microsoft (2024) ‘Best practices for comparing strings in .NET’, *Microsoft Learn*. Available at: https://learn.microsoft.com/en-us/dotnet/standard/base-types/best-practices-strings (Accessed: 11 April 2026).

---

### 6. Random Responses (Question 3)
*Used to vary chatbot responses for the same cybersecurity topic*

Microsoft (2024) ‘Random class’, *Microsoft Learn*. Available at: https://learn.microsoft.com/en-us/dotnet/api/system.random (Accessed: 12 March 2026).

Microsoft (2024) ‘Random.Next method’, *Microsoft Learn*. Available at: https://learn.microsoft.com/en-us/dotnet/api/system.random.next (Accessed: 12 March 2026).

---

### 7. Conversation Flow (Question 4)
*State tracking to maintain conversational context across turns*

Microsoft (2024) ‘How to: Create a simple chatbot’, *Microsoft Learn*. Available at: https://learn.microsoft.com/en-us/azure/bot-service/bot-builder-howto-send-messages (Accessed: 24 April 2026).

Microsoft (2024) ‘String class’, *Microsoft Learn*. Available at: https://learn.microsoft.com/en-us/dotnet/api/system.string (Accessed: 27 April 2026).

---

### 8. Memory and Recall (Question 5)
*`Dictionary` used to store user‑provided information (name, favourite topic) for personalised responses*

Microsoft (2024) ‘Dictionary\<TKey,TValue\>.ContainsKey method’, *Microsoft Learn*. Available at: https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.dictionary-2.containskey (Accessed: 20 April 2026).

Microsoft (2024) ‘Dictionary\<TKey,TValue\>.TryGetValue method’, *Microsoft Learn*. Available at: https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.dictionary-2.trygetvalue (Accessed: 12 April 2026).

Microsoft (2024) ‘String interpolation (C# reference)’, *Microsoft Learn*. Available at: https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/tokens/interpolated (Accessed: 12 April 2026).

---

### 9. Sentiment Detection (Question 6)
*Keyword‑based detection of user emotions to adapt chatbot tone*

Microsoft (2024) ‘String.Contains method’, *Microsoft Learn*. Available at: https://learn.microsoft.com/en-us/dotnet/api/system.string.contains (Accessed: 27 April 2026).

Kumar, D. (2020) ‘Simple sentiment analysis in C# without external libraries’, *C# Corner*. Available at: https://www.c-sharpcorner.com/article/simple-sentiment-analysis-in-c-sharp/ (Accessed: 15 April 2026).

Minni, N. (2023) ‘Sentiment analysis: A concise overview’, *Towards Data Science*. Available at: https://towardsdatascience.com/sentiment-analysis-a-concise-overview-8db20c41f5a3 (Accessed: 17 April 2026).

---

### 10. Voice Greeting Ported to GUI (Question 1 – Part 1 Carry‑over)
*`System.Media.SoundPlayer` integrated into WPF/WinForms window loaded event*

Microsoft (2024) ‘SoundPlayer class’, *Microsoft Learn*. Available at: https://learn.microsoft.com/en-us/dotnet/api/system.media.soundplayer (Accessed: 17 March 2026).

Microsoft (2024) ‘MediaPlayer class (WPF)’, *Microsoft Learn*. Available at: https://learn.microsoft.com/en-us/dotnet/api/system.windows.media.mediaplayer (Accessed: 17 March 2026).

---

### 11. Error Handling and Input Validation (Question 7)
*Exception handling and null‑checking to prevent crashes on invalid user input*

Microsoft (2024) ‘Exception handling (C# programming guide)’, *Microsoft Learn*. Available at: https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/exceptions/ (Accessed: 19 April 2026).

Microsoft (2024) ‘String.IsNullOrWhiteSpace method’, *Microsoft Learn*. Available at: https://learn.microsoft.com/en-us/dotnet/api/system.string.isnullorwhitespace (Accessed: 22 April 2026).

---

### 12. Code Structure, OOP, and Optimisation (Question 8)
*Separation of concerns across classes and methods; use of properties*

Microsoft (2024) ‘Object‑oriented programming (C#)’, *Microsoft Learn*. Available at: https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/object-oriented/ (Accessed: 19 March 2026).

Microsoft (2024) ‘Auto‑implemented properties (C# programming guide)’, *Microsoft Learn*. Available at: https://learn.microsoft.com/en-us/dotnet/csharp/programming-guide/classes-and-structs/auto-implemented-properties (Accessed: 19 March 2026).

---

### 13. GitHub Version Control, Tags, Releases and CI
*Repository management, semantic versioning tags, and GitHub Actions workflow*

GitHub (2024) ‘Understanding GitHub Actions’, *GitHub Docs*. Available at: https://docs.github.com/en/actions/learn-github-actions/understanding-github-actions (Accessed: 20 March 2026).

GitHub (2024) ‘Managing releases in a repository’, *GitHub Docs*. Available at: https://docs.github.com/en/repositories/releasing-projects-on-github/managing-releases-in-a-repository (Accessed:  1 May 2026).

GitHub (2024) ‘About tags’, *GitHub Docs*. Available at: https://docs.github.com/en/repositories/releasing-projects-on-github/viewing-your-repositorys-releases-and-tags (Accessed: 2 May 2026).

Microsoft (2024) ‘dotnet build command’, *Microsoft Learn*. Available at: https://learn.microsoft.com/en-us/dotnet/core/tools/dotnet-build (Accessed: 20 April 2026).

---

### 14. ASCII Art and Multimedia Assets
*Creating the logo and banners used in both Part 1 and Part 2*

FontB (2026) ‘ASCII Art Generator’, *FontB.com*. Available at: https://www.fontb.com/ascii-generator (Accessed: 25 April 2026).

ManyTools (2026) ‘Convert Images to ASCII Art’, *ManyTools.org*. Available at: https://manytools.org/hacker-tools/convert-images-to-ascii-art/ (Accessed: 23 April 2026).

FontB (2026) ‘Story Generator’, *FontB.com*. Available at: https://www.fontb.com/story-generator (Accessed: 24 April 2026).  
*(Used for brainstorming the chatbot’s voice and personality, which carries into the conversational flow of Part 2)*

---

### 15. Cybersecurity Domain Knowledge
*Background information that informed chatbot response content on phishing, passwords, privacy, and scams*

Pieterse, H. (2021) ‘The cyber threat landscape in South Africa: A 10‑year review’, *The African Journal of Information and Communication*, 28(28). doi: https://doi.org/10.23962/10539/32213. Available at: https://www.scielo.org.za/scielo.php?pid=S2077-72132021000200003&script=sci_arttext (Accessed: 11 April 2026).

National Cyber Security Centre (NCSC) (2023) ‘Password security’, *NCSC.gov.uk*. Available at: https://www.ncsc.gov.uk/collection/passwords (Accessed: 23 April 2026).

South African Banking Risk Information Centre (SABRIC) (2023) ‘Phishing’, *SABRIC.co.za*. Available at: https://www.sabric.co.za/stay-safe/phishing/ (Accessed: 27 April 2026).

HaveIBeenPwned (2026) *Have I Been Pwned: Check if your email has been compromised in a data breach*. Available at: https://haveibeenpwned.com (Accessed: 28 April 2026).

Information Regulator (South Africa) (2024) ‘Protection of Personal Information Act (POPIA)’, *Inforeg.org.za*. Available at: https://inforeg.org.za/act/ (Accessed: 27 April 2026).

---

### 16. Emoji Source
*Emojis used in the GUI and chatbot responses were copied directly from WhatsApp.*

WhatsApp (2024) *WhatsApp application*, [Mobile app]. Used for emoji characters pasted into XAML and response strings.

---

### 17. General Development Assistance
*Used for debugging, explaining C# and XAML syntax, and suggesting code structure improvements.*

ChatGPT (2026) *OpenAI ChatGPT*. Available at: https://chat.openai.com 

---

## Final Notes

Part 2 was a genuinely big transition from Part 1. The move from console to GUI meant rebuilding the project scaffolding, firmilarising myself with  XAML (Last did it when creating Mobile application projects with Kotlin) layout, rethinking how the chatbot delivers and receives messages, and solving a completely new set of errors. That said, every problem I hit taught me something concrete — about WPF's threading model, how namespaces work across files, why `CopyToOutputDirectory` matters, and how to structure a class so it can be tested and expanded without breaking everything else.

The chatbot is now far more intelligent and far more engaging than it was in Part 1. It remembers who you are, responds to how you feel, suggests related topics, and maintains a conversational thread across multiple turns. Part 3 will build on this further — adding a task assistant with a database, a cybersecurity quiz mini-game, NLP simulation, and a full activity log — all within the same GUI.

Well that is all that can be said for now.

**Until Next Time**  

**Author:** Lesego Khaole  
**Degree:** Bachelor of Computer and Information Sciences in Application Development (BCAD0701)  
**Student Number:** ST10455441  
**Year & Group:** Second Year, Group 2  
**Contact:** [lesegokhaolemsa@outlook.com](mailto:lesegokhaolemsa@outlook.com) | [lesegokhaole@icloud.com](mailto:lesegokhaole@icloud.com) 



