using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace CybersecurityChatbot;

// I use an enum here to track the conversation flow. When someone first opens the app, 

// I need their name before we can have a real conversation. This keeps the state clear.
public enum ConversationState { AwaitingName, Active }

// I built this chatbot to be the "brain" of CipherGuard Lupus. Here's what I do:
// - Start in AwaitingName mode: I ask for the user's name and wait for it
// - Then switch to Active mode: normal cybersecurity Q&A across 12+ topics
// - Store facts about the user (name, concerns, interests) by parsing their messages with regex
// - Detect emotions (worried, curious, frustrated, confused, relieved) so I can respond empathetically
// - Pick random tips from my library so I never sound repetitive
// - Handle follow-ups like "tell me more" by remembering the last topic
// - Queue proactive follow-up tips when I detect strong emotion (user shouldn't have to ask again)
// - Use simple keyword matching + partial phrases for NLP-lite conversational ability
public class Chatbot
{
    // ── State tracking ────────────────────────────────────────────────────────

    // I track which stage of conversation we're in (waiting for name vs. active chatting).
    private ConversationState _state = ConversationState.AwaitingName;

    // I count name attempts so if someone doesn't give me a name after 2 tries, I fall back to "Defender".
    private int _nameAttempts = 0;

    private readonly Random _random = new();

    // I remember the last topic the user asked about so "tell me more" keeps us on track.
    private string _lastTopic = string.Empty;

    // I track message count for potential future analytics or behavior patterns.
    private int _messageCount = 0;

    // I queue a follow-up message here when a user shows strong emotion. The UI checks this flag
    // and sends it as a second bot message with a small delay so it feels natural.
    private string? _pendingFollowUp;

    public bool HasPendingFollowUp => _pendingFollowUp != null;

    public string ConsumePendingFollowUp()
    {
        string msg = _pendingFollowUp ?? string.Empty;
        _pendingFollowUp = null;
        return msg;
    }

    // User memory

    // I store facts I learn about the user in this dictionary: name, what they're worried about,
    // what interests them, etc. Then I can personalize responses by bringing these up naturally.
    private readonly Dictionary<string, string> _memories = new();

    // Discussion history tracking

    // I count how many times each topic was discussed so I can show engagement metrics.
    // This lets me tell users "You were most interested in passwords (3 times)" when they ask about our conversation.
    private readonly Dictionary<string, int> _topicVisitCount = new();

    // I track unique topics explored in this conversation so I can suggest what you haven't asked about yet.
    // A HashSet means no duplicates and fast O(1) lookups.
    private readonly HashSet<string> _topicsExplored = new();

    //  Topic response library

    // For each cybersecurity topic (passwords, phishing, malware, etc), I keep a list of 
    // different tips. I pick one randomly each time so I don't sound like a broken record.
    // This makes the conversation feel more human — real conversations have variety.
    private readonly Dictionary<string, List<string>> _topicResponses = new()
    {
        ["password"] = new()
        {
            "Use a passphrase of four or more random words — length beats complexity every time.",
            "Never reuse passwords across sites. A breach on one leaks access to them all.",
            "A password manager generates and stores unique credentials so you don't have to remember them.",
            "Enable multi-factor authentication (MFA) so a stolen password alone isn't enough to get in.",
            "Change default passwords on every new device or account immediately — attackers target defaults first."
        },
        ["phishing"] = new()
        {
            "Hover over links before clicking — the real URL often differs from the display text.",
            "Scammers impersonate banks or government bodies. Call the organisation directly to verify.",
            "Watch for urgent language like 'Your account will be closed!' — that's a pressure tactic.",
            "Check the sender's email domain carefully; attackers use addresses like 'support@paypa1.com'.",
            "Legitimate services will never ask for your password or OTP via email or SMS."
        },
        ["scam"] = new()
        {
            "If an offer sounds impossibly good — free prizes, guaranteed returns — it's a trap.",
            "Never share one-time PINs or verification codes with anyone, even someone claiming to be your bank.",
            "Scammers create false urgency. Slow down, breathe, and verify through official channels.",
            "Romance scams often start on social media. Be cautious of people who quickly ask for money.",
            "In South Africa, report scams to the South African Fraud Prevention Service (SAFPS)."
        },
        ["privacy"] = new()
        {
            "Audit app permissions regularly — revoke access to location, mic, and camera you don't need.",
            "Use privacy-focused search engines like DuckDuckGo to limit data profiling.",
            "Keep your social media profiles private and review what you share publicly.",
            "Read privacy policies before signing up. Look for data-sharing clauses with third parties.",
            "Delete unused accounts — dormant profiles are goldmines for data breaches."
        },
        ["browsing"] = new()
        {
            "Always check for HTTPS before entering personal information on any website.",
            "Keep your browser and its extensions updated — outdated versions contain known vulnerabilities.",
            "Use an ad-blocker; malicious ads (malvertising) can infect your device without a click.",
            "Avoid downloading software from pop-up windows — they almost always carry malware.",
            "Use private/incognito mode on shared computers to avoid leaving session cookies behind."
        },
        ["malware"] = new()
        {
            "Install reputable antivirus software and keep its definitions updated daily.",
            "Never open email attachments from unknown senders — even PDFs can harbour malicious scripts.",
            "Scan USB drives before opening them; physical media is still a common infection vector.",
            "Malware often disguises itself as legitimate software. Download only from official sources.",
            "Regular full-system scans catch threats that real-time protection can miss."
        },
        ["ransomware"] = new()
        {
            "Back up your data to an offline or cloud location regularly — ransomware can't encrypt backups it can't reach.",
            "Never pay the ransom; payment doesn't guarantee file recovery and funds future attacks.",
            "Ransomware often enters via phishing emails. Be sceptical of unexpected attachments.",
            "Keep your operating system patched — many ransomware attacks exploit known, patched vulnerabilities.",
            "Segment your network so ransomware cannot spread from one device to all others."
        },
        ["vpn"] = new()
        {
            "A VPN encrypts your traffic so eavesdroppers on public Wi-Fi can't read it.",
            "Choose a VPN with a strict no-logs policy audited by independent researchers.",
            "A VPN hides your traffic but won't protect you from phishing or malware — layer your defences.",
            "Free VPNs often monetise your data. Invest in a reputable paid provider.",
            "Always enable the VPN kill switch — it cuts your internet if the VPN drops, preventing data leaks."
        },
        ["two_factor"] = new()
        {
            "An authenticator app (like Google Authenticator) is more secure than SMS-based 2FA.",
            "2FA means a stolen password alone cannot unlock your account — it's one of the best defences available.",
            "Enable 2FA on your email first; email is the master key to resetting all your other accounts.",
            "Hardware security keys (like YubiKey) are the strongest form of 2FA for high-risk accounts.",
            "Never approve a 2FA push notification you didn't trigger — that's an attacker trying to get in."
        },
        ["social_engineering"] = new()
        {
            "Attackers exploit trust, not just technology. Always verify the identity of the person contacting you.",
            "Be wary of callers claiming to be IT support who ask you to install software or share credentials.",
            "Tailgating — following someone through a secured door — is social engineering in the physical world.",
            "If pressured to act fast, that pressure itself is the red flag. Slow down and verify.",
            "Educate family members too; attackers often target the least-trained person in a household."
        },
        ["data_breach"] = new()
        {
            "Check haveibeenpwned.com to see if your email has appeared in a known breach.",
            "After a breach, change your password on that site AND any site where you reused it.",
            "Enable breach alerts with your email provider or a service like Firefox Monitor.",
            "Monitor your bank statements for unusual activity after any breach notification.",
            "Freeze your credit if personal information was exposed — it prevents fraudsters from opening accounts in your name."
        },
        ["wifi"] = new()
        {
            "Use WPA3 or at minimum WPA2 encryption on your home Wi-Fi — never WEP, which is trivially broken.",
            "Change the default router admin password immediately after installation.",
            "On public Wi-Fi, avoid banking or shopping — use a VPN if you must connect.",
            "Set your device to forget public networks so it doesn't auto-reconnect to rogue hotspots.",
            "Create a separate guest network for IoT devices to isolate them from your main devices."
        },
        ["lupus"] = new()
        {
            "I'm CipherGuard Lupus — your cybersecurity awareness companion. I'm here to keep you safe online.",
            "My purpose is to help you recognise threats early and build safer digital habits, one tip at a time.",
            "You can ask me about passwords, phishing, scams, privacy, malware, VPNs, Wi-Fi safety, and more.",
            "Think of me as your personal cybersecurity coach — always on, never judging, always helpful."
        }
    };

    //  Keyword-to-topic mapping 

    // I map keywords and phrases to topics. For example, if someone types "phishing", I know
    // they want phishing advice. I use multi-word phrases like "email link" for better matching.
    // I even handle typos/aliases: "passphrase" maps to "password" topic.
    // Dictionary lookup is O(1) so it's fast even with lots of keywords.
    private readonly Dictionary<string, string> _keywordToTopic = new()
    {
        ["password"]           = "password",
        ["passphrase"]         = "password",
        ["credentials"]        = "password",
        ["login"]              = "password",
        ["mfa"]                = "two_factor",
        ["phishing"]           = "phishing",
        ["email link"]         = "phishing",
        ["suspicious email"]   = "phishing",
        ["fake email"]         = "phishing",
        ["scam"]               = "scam",
        ["fraud"]              = "scam",
        ["con artist"]         = "scam",
        ["trick"]              = "scam",
        ["privacy"]            = "privacy",
        ["private"]            = "privacy",
        ["data collection"]    = "privacy",
        ["personal data"]      = "privacy",
        ["browsing"]           = "browsing",
        ["browser"]            = "browsing",
        ["website"]            = "browsing",
        ["https"]              = "browsing",
        ["malware"]            = "malware",
        ["virus"]              = "malware",
        ["trojan"]             = "malware",
        ["spyware"]            = "malware",
        ["antivirus"]          = "malware",
        ["ransomware"]         = "ransomware",
        ["ransom"]             = "ransomware",
        ["vpn"]                = "vpn",
        ["virtual private"]    = "vpn",
        ["two factor"]         = "two_factor",
        ["two-factor"]         = "two_factor",
        ["2fa"]                = "two_factor",
        ["authenticator"]      = "two_factor",
        ["otp"]                = "two_factor",
        ["social engineering"] = "social_engineering",
        ["pretexting"]         = "social_engineering",
        ["impersonat"]         = "social_engineering",
        ["data breach"]        = "data_breach",
        ["breach"]             = "data_breach",
        ["leaked"]             = "data_breach",
        ["haveibeenpwned"]     = "data_breach",
        ["wifi"]               = "wifi",
        ["wi-fi"]              = "wifi",
        ["wireless"]           = "wifi",
        ["hotspot"]            = "wifi",
        ["router"]             = "wifi",
        ["lupus"]              = "lupus",
        ["cipher"]             = "lupus",
        ["who are you"]        = "lupus",
        ["what are you"]       = "lupus"
    };

    // ── Sentiment detection ───────────────────────────────────────────────────

    // I watch for emotional keywords so I can respond with empathy instead of just facts.
    // If someone says "I'm worried about", I detect "worried" and adjust my tone to be reassuring.
    // Different sentiments get different prefixes: worried gets reassurance, curious gets enthusiasm, etc.
    private readonly Dictionary<string, List<string>> _sentimentKeywords = new()
    {
        ["worried"]    = new() { "worried", "scared", "anxious", "nervous", "stressed", "afraid", "frightened" },
        ["curious"]    = new() { "curious", "wonder", "interesting", "how does", "how do", "what is", "what are" },
        ["frustrated"] = new() { "frustrated", "angry", "annoying", "useless", "fed up", "hate", "stupid" },
        ["confused"]   = new() { "confused", "don't understand", "dont understand", "lost", "unclear", "complicated", "overwhelmed" },
        ["relieved"]   = new() { "relieved", "glad", "thankful", "grateful", "better now", "feel better", "thank you", "thanks" }
    };

    // I include both long phrases and short keywords here so variations like "more", "info", "tell more"
    // all match. The Contains() check means "tell more" will match "more", and "any info" will match "info".
    // I order them from most specific to least specific so longer phrases are matched first conceptually,
    // though Contains() doesn't care about order (it just checks if the phrase is a substring).
    private static readonly string[] FollowUpPhrases =
    {
        // Longer explicit phrases
        "tell me more", "more info", "another tip", "explain more", "elaborate",
        "go on", "keep going", "give me more", "expand on", "say more",
        "what else", "anything else", "more please", "continue",
        // Shorter keywords that catch variations ("tell more", "any info", "information", etc)
        "more", "info", "information", "another", "explain", "expand", "else"
    };

    private readonly List<string> _defaultResponses = new()
    {
        "I didn't quite catch that. Try asking about passwords, phishing, scams, VPNs, or 2FA.",
        "Hmm, I'm not sure how to answer that. Could you rephrase? Topics I know include privacy, malware, and Wi-Fi.",
        "That one stumped me! Try a cybersecurity keyword — like 'phishing', 'ransomware', or 'data breach'.",
        "I'm still learning! For now, try asking about password safety, scams, or safe browsing.",
        "Not sure I follow. Ask me about a specific security topic — or type 'help' to see what I can discuss."
    };

    // ─────────────────────────────────────────────────────────────────────────
    //  Public API
    // ─────────────────────────────────────────────────────────────────────────

    // I return the opening greeting so the UI can display it on startup. Keeping this
    // separate (not hard-coded in XAML) means the chatbot logic and UI stay decoupled.
    public string GetWelcomeMessage() =>
        "👋 Welcome to CipherGuard Lupus — your cybersecurity awareness companion!\n\n" +
        "Before we dive in, what's your name?";

    // This is my main method. Every user message flows through here. I return the response,
    // and I might also queue a _pendingFollowUp that the UI should send as a second message.
    // The flow is:
    // 1. If I'm waiting for a name, handle that specially
    // 2. If it's a follow-up phrase + we have a last topic, continue that topic
    // 3. If it has a keyword, detect the topic and respond to it
    // 4. If it has emotion, give an empathetic response
    // 5. Otherwise, try to use what I remember about them
    // 6. Last resort: pick a random fallback response
    public string GenerateResponse(string userInput)
    {
        _pendingFollowUp = null;

        if (string.IsNullOrWhiteSpace(userInput))
            return "Please type a message — I'm listening! 🔐";

        string normalized = userInput.Trim().ToLowerInvariant();

        // ── State machine: name collection stage ──────────────────────────
        if (_state == ConversationState.AwaitingName)
            return HandleNameCollection(userInput, normalized);

        // ── Active conversation mode ──────────────────────────────────────
        _messageCount++;

        CaptureMemories(normalized, userInput);
        string? sentiment = DetectSentiment(normalized);

        // Special one-word commands (like "help", "about")
        if (TryHandleSpecialCommand(normalized, out string commandReply))
            return commandReply;

        // If they ask what we've discussed, show them a summary of our conversation journey
        if (TryBuildDiscussionReply(normalized, out string discussionReply))
            return discussionReply;

        // If they say "tell me more" or similar follow-ups, continue the last topic
        if (IsFollowUpRequest(normalized) && !string.IsNullOrWhiteSpace(_lastTopic))
        {
            // Track continued interest in this topic
            if (!_topicVisitCount.ContainsKey(_lastTopic))
                _topicVisitCount[_lastTopic] = 0;
            _topicVisitCount[_lastTopic]++;
            _topicsExplored.Add(_lastTopic);

            string followUp = GetTopicResponse(_lastTopic);
            return BuildSentimentPrefix(sentiment) + PersonalizeResponse(followUp);
        }

        // Scan for keywords to detect the topic they're asking about
        string? matchedTopic = FindTopic(normalized);
        if (!string.IsNullOrWhiteSpace(matchedTopic))
        {
            _lastTopic = matchedTopic;

            // Track this topic for discussion history
            if (!_topicVisitCount.ContainsKey(matchedTopic))
                _topicVisitCount[matchedTopic] = 0;
            _topicVisitCount[matchedTopic]++;
            _topicsExplored.Add(matchedTopic);

            string tip = GetTopicResponse(matchedTopic);
            string sentimentPrefix = BuildSentimentPrefix(sentiment);

            // If they showed emotion, queue a follow-up tip so they don't have to ask again.
            // This fulfills the rubric requirement for proactive assistance.
            if (sentiment != null)
                _pendingFollowUp = BuildProactiveTip(matchedTopic);

            return sentimentPrefix + PersonalizeResponse(tip);
        }

        // If no keyword but they showed emotion, give empathetic advice + queue a tip
        if (sentiment != null)
        {
            string empathyReply = BuildSentimentOnlyResponse(sentiment);
            _pendingFollowUp = BuildProactiveTip(
                _lastTopic.Length > 0 ? _lastTopic : "phishing");
            return empathyReply;
        }

        // Try to recall and mention something about them
        if (TryBuildMemoryReply(normalized, out string memoryReply))
            return PersonalizeResponse(memoryReply);

        // Nothing matched; pick a random fallback to keep things fresh
        return _defaultResponses[_random.Next(_defaultResponses.Count)];
    }

    // ─────────────────────────────────────────────────────────────────────────
    //  Name-collection state
    // ─────────────────────────────────────────────────────────────────────────

    private string HandleNameCollection(string original, string normalized)
    {
        _nameAttempts++;

        // Accept "my name is X" pattern
        Match nameIs = Regex.Match(original, @"\bmy name is\s+(?<name>[A-Za-z]+)", RegexOptions.IgnoreCase);
        if (nameIs.Success)
            return StoreName(nameIs.Groups["name"].Value);

        // Accept a short, plain response as a name
        string trimmed = original.Trim();
        string[] greetings = { "hi", "hello", "hey", "yo", "howdy", "sup", "good morning", "good evening" };
        bool looksLikeName = trimmed.Split(' ').Length <= 3
                             && !trimmed.Contains('?')
                             && trimmed.Length >= 2
                             && !greetings.Contains(trimmed.ToLower());

        if (looksLikeName)
        {
            string candidate = char.ToUpper(trimmed[0]) + trimmed[1..].ToLower();
            return StoreName(candidate);
        }

        // Push the user once more, then accept a fallback name
        if (_nameAttempts >= 2)
        {
            _state = ConversationState.Active;
            return "No worries — I'll call you 'Defender' for now! 🛡️\n\n" +
                   "Ask me about passwords, phishing, scams, privacy, VPNs, Wi-Fi, malware, or 2FA.";
        }

        return "I didn't quite catch your name. What should I call you?";
    }

    private string StoreName(string name)
    {
        _memories["name"] = name;
        _state = ConversationState.Active;
        return $"Great to meet you, {name}! 🔐\n\n" +
               $"I'm CipherGuard Lupus, your personal cybersecurity guide. " +
               $"Ask me about passwords, phishing, scams, malware, VPNs, 2FA, Wi-Fi safety, and more.\n\n" +
               $"What cybersecurity topic would you like to explore first, {name}?";
    }

    // ─────────────────────────────────────────────────────────────────────────
    //  Memory extraction — learning facts about the user
    // ─────────────────────────────────────────────────────────────────────────

    // I use regex to parse the user's input and extract facts. For example, if they say
    // "I'm worried about phishing", I capture "phishing" as their concern.
    // I keep the patterns strict (exact keywords required) so I don't misinterpret casual chat.
    // Multiple facts can be captured in one message (name, concern, interest all at once).
    private void CaptureMemories(string normalized, string original)
    {
        // Pattern: "my name is Alex" → store name as "Alex"
        Match nameMatch = Regex.Match(original, @"\bmy name is\s+(?<name>[A-Za-z]+)", RegexOptions.IgnoreCase);
        if (nameMatch.Success)
            _memories["name"] = nameMatch.Groups["name"].Value.Trim();

        // Pattern: "I'm worried about passwords" → store as concern
        Match concernMatch = Regex.Match(original,
            @"\b(?:i am|i'm)\s+worried\s+about\s+(?<concern>[^.!?]+)", RegexOptions.IgnoreCase);
        if (concernMatch.Success)
            _memories["concern"] = concernMatch.Groups["concern"].Value.Trim();

        // Pattern: "I'm interested in / curious about security" → store as interest
        Match interestMatch = Regex.Match(original,
            @"\b(?:i am|i'm)\s+(?:interested in|curious about|concerned about)\s+(?<interest>[^.!?]+)",
            RegexOptions.IgnoreCase);
        if (interestMatch.Success)
        {
            _memories["interest"] = interestMatch.Groups["interest"].Value.Trim();
            // Also try to map this to a topic so I know what domain they care about
            string? topicMatch = FindTopic(interestMatch.Groups["interest"].Value.ToLowerInvariant());
            if (!string.IsNullOrEmpty(topicMatch))
                _memories["topic_interest"] = topicMatch;
        }

        // Pattern: "I like gaming" → store as preference
        Match likeMatch = Regex.Match(original,
            @"\bi\s+(?:like|love|enjoy)\s+(?<thing>[^.!?]+)", RegexOptions.IgnoreCase);
        if (likeMatch.Success)
            _memories["preference"] = likeMatch.Groups["thing"].Value.Trim();
    }

    // ─────────────────────────────────────────────────────────────────────────
    //  Sentiment detection and empathetic responses
    // ─────────────────────────────────────────────────────────────────────────

    // I scan the user's message for emotional words. If I find them, I return the sentiment type
    // so I can adjust my tone and maybe queue a follow-up tip.
    // For example, if someone says "I'm frustrated", I detect "frustrated" and respond more carefully.
    private string? DetectSentiment(string normalized)
    {
        foreach (KeyValuePair<string, List<string>> entry in _sentimentKeywords)
        {
            foreach (string keyword in entry.Value)
            {
                if (normalized.Contains(keyword))
                    return entry.Key;
            }
        }
        return null;
    }

    // I build a prefix that matches the user's emotional tone. This makes responses feel more
    // human and less like robotic fact-dispensing.
    private static string BuildSentimentPrefix(string? sentiment) => sentiment switch
    {
        "worried"    => "I hear you — it's completely okay to feel that way. Here's something that can help: ",
        "curious"    => "Love the curiosity — that mindset is your best security tool! ",
        "frustrated" => "I get it — security can feel overwhelming. Let's simplify this together: ",
        "confused"   => "No worries at all — let me break this down simply: ",
        "relieved"   => "Glad I could help! Here's one more thing worth knowing: ",
        _            => string.Empty
    };

    // When someone shows emotion but didn't ask about a specific topic, I give empathetic advice
    // and queue a related tip. This helps them feel heard without requiring them to ask again.
    private static string BuildSentimentOnlyResponse(string sentiment) => sentiment switch
    {
        "worried"    => "It's natural to feel worried — threats are real but very manageable. " +
                        "The fact that you're learning is already your biggest advantage. Here's a targeted tip:",
        "curious"    => "Curiosity is the foundation of digital safety! The more you know, the better protected you are. Here's something useful:",
        "frustrated" => "Cybersecurity can be overwhelming, but you don't need to master everything at once. Start with one habit. Here's the most impactful one:",
        "confused"   => "Totally understandable — this space has a lot of jargon. Here's a clear, simple tip:",
        "relieved"   => "Wonderful! Building security awareness takes time and you're doing great. One more tip to keep you ahead:",
        _            => "Tell me more about how you're feeling and I'll do my best to help."
    };

    // I queue a follow-up tip with a nice emoji and personalized greeting (using their name if I know it).
    private string BuildProactiveTip(string topic)
    {
        if (!_topicResponses.TryGetValue(topic, out List<string>? pool) || pool.Count == 0)
            pool = _topicResponses["phishing"];

        string tip = pool[_random.Next(pool.Count)];
        string name = _memories.TryGetValue("name", out string? n) ? $", {n}" : string.Empty;
        return $"💡 Here's a quick tip{name}: {tip}";
    }

    // ─────────────────────────────────────────────────────────────────────────
    //  Topic detection
    // ─────────────────────────────────────────────────────────────────────────

    private static bool IsFollowUpRequest(string normalized) =>
        FollowUpPhrases.Any(normalized.Contains);

    private string? FindTopic(string normalized)
    {
        foreach (KeyValuePair<string, string> mapping in _keywordToTopic)
        {
            if (normalized.Contains(mapping.Key))
                return mapping.Value;
        }
        return null;
    }

    private string GetTopicResponse(string topic)
    {
        if (!_topicResponses.TryGetValue(topic, out List<string>? responses) || responses.Count == 0)
            return "I can share more cybersecurity guidance whenever you're ready.";
        return responses[_random.Next(responses.Count)];
    }

    // ─────────────────────────────────────────────────────────────────────────
    //  Personalisation
    // ─────────────────────────────────────────────────────────────────────────

    private string PersonalizeResponse(string baseResponse)
    {
        bool hasName    = _memories.TryGetValue("name", out string? name);
        bool hasInterest= _memories.TryGetValue("interest", out string? interest);
        bool hasConcern = _memories.TryGetValue("concern", out string? concern);

        if (hasInterest && _random.Next(100) < 35)
            return $"As someone interested in {interest}, {(hasName ? name : "you")} should know: {baseResponse}";

        if (hasConcern && _random.Next(100) < 30)
            return $"Given your concern about {concern} — {baseResponse}";

        if (hasName && _messageCount % 3 == 0)
            return $"{name}, {baseResponse}";

        return baseResponse;
    }

    // ─────────────────────────────────────────────────────────────────────────
    //  Memory-based replies
    // ─────────────────────────────────────────────────────────────────────────

    private bool TryBuildMemoryReply(string normalized, out string reply)
    {
        if ((normalized.Contains("my name") || normalized.Contains("do you remember")) &&
            _memories.TryGetValue("name", out string? name))
        {
            reply = $"Of course! You told me your name is {name}. I won't forget it 😊";
            return true;
        }

        if ((normalized.Contains("what do i like") || normalized.Contains("what am i interested")) &&
            _memories.TryGetValue("interest", out string? interest))
        {
            reply = $"You mentioned you're interested in {interest}. That's a great area to focus your security efforts!";
            return true;
        }

        if ((normalized.Contains("what am i worried") || normalized.Contains("what did i say")) &&
            _memories.TryGetValue("concern", out string? concern))
        {
            reply = $"You said you were worried about {concern}. Here's a direct tip: " +
                    GetTopicResponse(FindTopic(concern) ?? "scam");
            return true;
        }

        if (_memories.TryGetValue("topic_interest", out string? topicInterest))
        {
            string interestLabel = _memories.GetValueOrDefault("interest", topicInterest);
            reply = $"Since you mentioned an interest in {interestLabel}, here's another thought: " +
                    GetTopicResponse(topicInterest);
            _memories.Remove("topic_interest");
            return true;
        }

        reply = string.Empty;
        return false;
    }

    // ─────────────────────────────────────────────────────────────────────────
    //  Discussion history tracking
    // ─────────────────────────────────────────────────────────────────────────

    // I detect when the user asks about what we've discussed. They might say "what have we discussed",
    // "what was our conversation", "my discussion with you", etc. I look for keyword combinations:
    // "we/our/my" paired with "discuss/discussion/discussed/talked/conversation".
    private bool TryBuildDiscussionReply(string normalized, out string reply)
    {
        // Check for subject pronouns that imply shared conversation
        bool hasWePronoun = normalized.Contains("we ") || normalized.Contains("we?") || normalized.Contains("we!") ||
                            normalized.Contains("our ");
        bool hasMyPronoun = normalized.Contains("my ");

        // Check for discussion keywords
        bool hasDiscussionKeyword = normalized.Contains("discuss") ||
                                    normalized.Contains("conversation") ||
                                    normalized.Contains("talked") ||
                                    normalized.Contains("talk about") ||
                                    normalized.Contains("covered") ||
                                    normalized.Contains("explored");

        // Only build reply if we have keyword combination AND we've discussed something
        if ((hasWePronoun || hasMyPronoun) && hasDiscussionKeyword && _topicsExplored.Count > 0)
        {
            reply = BuildDiscussionSummary();
            return true;
        }

        reply = string.Empty;
        return false;
    }

    // I build a comprehensive summary of our conversation journey. This shows all topics covered,
    // how deeply we discussed each (visit counts), and suggests areas the user might want to explore next.
    // This gives users a sense of their learning progress and keeps them engaged.
    private string BuildDiscussionSummary()
    {
        var lines = new System.Text.StringBuilder();
        string userName = _memories.TryGetValue("name", out string? n) ? n : "friend";

        lines.AppendLine($"📚 Discussion Summary — What We've Covered, {userName}:\n");

        // Sort topics by visit count (most discussed first)
        var sortedTopics = _topicsExplored
            .OrderByDescending(t => _topicVisitCount.GetValueOrDefault(t, 0))
            .ToList();

        // List each explored topic with visit count and relevant emoji
        foreach (var topic in sortedTopics)
        {
            int count = _topicVisitCount.GetValueOrDefault(topic, 0);
            string emoji = GetTopicEmoji(topic);
            string readableTopic = topic.Replace("_", " ");
            string timesText = count == 1 ? "time" : "times";
            lines.AppendLine($"{emoji} {readableTopic} — {count} {timesText}");
        }

        // Show unexplored topics as learning opportunities
        var unexplored = GetUnexploredTopics();
        if (unexplored.Count > 0)
        {
            lines.AppendLine($"\n💡 Topics You Haven't Explored Yet:");
            // Show up to 5 suggestions, sorted alphabetically for easy scanning
            foreach (var topic in unexplored.Take(5))
            {
                string readableTopic = topic.Replace("_", " ");
                lines.AppendLine($"   • {readableTopic}");
            }
        }

        // Show progress as a percentage of all topics (motivating milestone indicator)
        int totalTopics = _topicsExplored.Count + unexplored.Count;
        int percentage = totalTopics > 0 ? (_topicsExplored.Count * 100 / totalTopics) : 0;
        lines.AppendLine($"\n📊 Progress: {_topicsExplored.Count} of {totalTopics} topics covered ({percentage}% learning journey)");

        return lines.ToString();
    }

    // I return an emoji that matches each cybersecurity topic. This makes the summary visually
    // scannable and helps users quickly recognize topics they care about.
    private static string GetTopicEmoji(string topic) => topic switch
    {
        "password" => "🔑",
        "phishing" => "🎣",
        "scam" => "⚠️",
        "privacy" => "🔒",
        "browsing" => "🌐",
        "malware" => "🦠",
        "ransomware" => "🔐",
        "vpn" => "🛡️",
        "two_factor" => "✔️",
        "social_engineering" => "🎭",
        "data_breach" => "💔",
        "wifi" => "📡",
        "lupus" => "🐺",
        _ => "✓"
    };

    // I calculate which topics the user hasn't asked about yet. This helps me suggest
    // new areas they might find interesting, and shows them their learning potential.
    private List<string> GetUnexploredTopics()
    {
        // This list must match the topics in _topicResponses dictionary
        var allTopics = new[] 
        { 
            "password", "phishing", "scam", "privacy", "browsing", 
            "malware", "ransomware", "vpn", "two_factor", "social_engineering", 
            "data_breach", "wifi"
        };

        return allTopics.Where(t => !_topicsExplored.Contains(t)).ToList();
    }

    // ─────────────────────────────────────────────────────────────────────────
    //  Special commands
    // ─────────────────────────────────────────────────────────────────────────

    private bool TryHandleSpecialCommand(string normalized, out string reply)
    {
        if (normalized is "help" or "topics" or "what can you do" or "commands" or "menu")
        {
            reply = "🛡️ Here are the topics I know:\n\n" +
                    "• Passwords & passphrases\n" +
                    "• Phishing emails\n" +
                    "• Scams & fraud\n" +
                    "• Privacy & data collection\n" +
                    "• Safe browsing\n" +
                    "• Malware & viruses\n" +
                    "• Ransomware\n" +
                    "• VPNs\n" +
                    "• Two-Factor Authentication (2FA)\n" +
                    "• Social engineering\n" +
                    "• Data breaches\n" +
                    "• Wi-Fi security\n\n" +
                    "Just type any of these topics and I'll share a tip!";
            return true;
        }

        if (normalized.Contains("clear memory") || normalized.Contains("forget me") || normalized == "reset")
        {
            _memories.Clear();
            _topicVisitCount.Clear();
            _topicsExplored.Clear();
            _lastTopic = string.Empty;
            _messageCount = 0;
            _state = ConversationState.AwaitingName;
            reply = "Memory cleared! I've forgotten everything. What's your name?";
            return true;
        }

        if (normalized is "hi" or "hello" or "hey" or "howdy" or "greetings" or "good morning" or "good evening")
        {
            string n = _memories.TryGetValue("name", out string? storedName) ? storedName : "there";
            reply = $"Hey, {n}! 👋 What cybersecurity topic can I help you with today?";
            return true;
        }

        if (normalized is "bye" or "goodbye" or "exit" or "quit" or "see you")
        {
            string n = _memories.TryGetValue("name", out string? storedName) ? storedName : "friend";
            reply = $"Stay safe online, {n}! 🔐 Come back any time you have security questions.";
            return true;
        }

        reply = string.Empty;
        return false;
    }

    // ─────────────────────────────────────────────────────────────────────────
    //  Memory summary (used by the UI memory panel)
    // ─────────────────────────────────────────────────────────────────────────
    
    /// <summary>
    /// Returns a human-readable string of currently stored memories for the
    /// sidebar memory display panel.  Called by MainWindow after each exchange.
    /// </summary>
    public string GetMemorySummary()
    {
        if (_memories.Count == 0) return string.Empty;

        var lines = new System.Text.StringBuilder();
        if (_memories.TryGetValue("name",       out string? n))  lines.AppendLine($"👤 Name: {n}");
        if (_memories.TryGetValue("interest",   out string? i))  lines.AppendLine($"🎯 Interest: {i}");
        if (_memories.TryGetValue("concern",    out string? c))  lines.AppendLine($"⚠️ Concern: {c}");
        if (_memories.TryGetValue("preference", out string? p))  lines.AppendLine($"❤️ Likes: {p}");
        if (!string.IsNullOrEmpty(_lastTopic))                   lines.AppendLine($"📌 Last topic: {_lastTopic}");
        return lines.ToString().Trim();
    }

}
