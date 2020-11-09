using System.Linq;
using System.Threading.Tasks;
using AudioSwitcher.AudioApi.CoreAudio;
using Newtonsoft.Json.Linq;
using StreamDeck.NET;
using StreamDeck.NET.Message.Received;

namespace AudioDevice.Action
{
    public class BaseAudioDeviceAction : StreamDeckBaseAction
    {
        protected static readonly CoreAudioController Controller = new CoreAudioController();
        protected CoreAudioDevice Device { get; private set; }


        //Controller.AudioDeviceChanged.Subscribe(); -> recheck Device


        public override async Task WillAppear(StreamDeckWillAppearEventEventMessage message)
        {
            await UpdateDevice(message.Payload.Settings);
        }


        public override async Task DidReceiveSettings(StreamDeckDidReceiveSettingsEventMessage message)
        {
            await UpdateDevice(message.Payload.Settings);
        }


        protected async Task UpdateDevice(JObject settings)
        {
            var devices = await Controller.GetPlaybackDevicesAsync();

            // TODO check settings for device ID
            Device = devices.FirstOrDefault(d => d.FullName == "Speakers (Realtek High Definition Audio)");
        }
    }
}
