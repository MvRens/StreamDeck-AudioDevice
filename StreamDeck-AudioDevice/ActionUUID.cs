namespace AudioDevice
{
    public static class ActionUUID
    {
        private const string BaseUUID = "com.x2software.streamdeck.audiodevice.";

        public const string VolumeUp = BaseUUID + "volumeup";
        public const string VolumeDown = BaseUUID + "volumedown";
        public const string ToggleMuted = BaseUUID + "togglemuted";
        public const string SetMuted = BaseUUID + "setmuted";
        public const string SetUnmuted = BaseUUID + "setunmuted";
        public const string SetDefault = BaseUUID + "setdefault";
    }
}
