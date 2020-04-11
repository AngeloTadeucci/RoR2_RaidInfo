using RoR2;

namespace RaidInfo {
    public static class Message { // @JackPendarvesRead awesome code!
        public static void Send(string message) {
            Chat.SendBroadcastChat((Chat.ChatMessageBase)new Chat.SimpleChatMessage() {
                baseToken = "{0}",
                paramTokens = new string[] { message }
            });
        }

        public static void SendColoured(string message, string colourHex) {
            Chat.SendBroadcastChat((Chat.ChatMessageBase)new Chat.SimpleChatMessage() {
                baseToken = $"<color={colourHex}>{{0}}</color>",
                paramTokens = new string[] { message }
            });
        }
    }

    public static class Colours {
        public static string LightBlue => "#03ffff";
        public static string Red => "#f01d1d";
        public static string Orange => "#ff7912";
        public static string Yellow => "#ffff26";
        public static string Green => "#0afa2a";
    }
}
