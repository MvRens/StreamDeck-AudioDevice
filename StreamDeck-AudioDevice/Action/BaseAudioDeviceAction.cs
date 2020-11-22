using System;
using System.Linq;
using System.Threading.Tasks;
using AudioSwitcher.AudioApi;
using AudioSwitcher.AudioApi.CoreAudio;
using Newtonsoft.Json.Linq;
using StreamDeck.NET;
using StreamDeck.NET.Message.Received;

namespace AudioDevice.Action
{
    public class BaseAudioDeviceAction : StreamDeckBaseAction, IObserver<DeviceChangedArgs>
    {
        protected static readonly CoreAudioController Controller = new CoreAudioController();

        protected Guid? DeviceID { get; private set; }
        protected CoreAudioDevice Device { get; private set; }

        protected readonly IStreamDeckClient Client;
        private readonly IDisposable controllerSubscriber;

        private volatile bool deviceInvalidated = true;


        public BaseAudioDeviceAction(IStreamDeckClient client)
        {
            Client = client;
            controllerSubscriber = Controller.AudioDeviceChanged.Subscribe(this);
        }


        public override void Dispose()
        {
            controllerSubscriber?.Dispose();

            base.Dispose();
        }


        public override async Task WillAppear(StreamDeckWillAppearEventEventMessage message)
        {
            await UpdateDevice(message.Payload.Settings);
        }


        public override async Task DidReceiveSettings(StreamDeckDidReceiveSettingsEventMessage message)
        {
            await UpdateDevice(message.Payload.Settings);
        }


        public override async Task SendToPlugin(StreamDeckSendToPluginEventMessage message)
        {
            var messageEvent = message.Payload["event"];
            if (messageEvent?.Value<string>() != "getDevices")
                return;

            var devices = await Controller.GetDevicesAsync(DeviceType.Playback);
            var responseDevices = new JArray(devices.OrderBy(d => d.FullName).Select(d => new JObject
            {
                { "id", d.Id.ToString() },
                { "displayName", d.FullName },
                { "active", (d.State & DeviceState.Active) > 0 }
            }));

            var response = new JObject
            {
                { "event", "getDevices" },
                { "devices", responseDevices }
            };

            await Client.SendToPropertyInspector(message.Action, response);
        }


        protected async Task UpdateDevice(JObject settings)
        {
            var newDeviceId = settings["deviceId"]?.Value<string>();
            Guid? newDeviceGuid = null;

            if (!string.IsNullOrEmpty(newDeviceId) && Guid.TryParse(newDeviceId, out var parsedGuid))
                newDeviceGuid = parsedGuid;

            if (newDeviceGuid != DeviceID)
            {
                DeviceID = newDeviceGuid;
                deviceInvalidated = true;
            }

            if (deviceInvalidated)
                await UpdateDevice();
        }


        protected async Task UpdateDevice()
        {
            deviceInvalidated = false;

            if (DeviceID.HasValue)
            {
                var devices = await Controller.GetPlaybackDevicesAsync();
                Device = devices.FirstOrDefault(d => d.Id == DeviceID.Value);

                // Even if a specific device is selected and not available, do not fall back to the default device
                return;
            }

            Device = await Controller.GetDefaultDeviceAsync(DeviceType.Playback, Role.Multimedia);
        }


        protected void InvalidateDevice()
        {
            deviceInvalidated = true;
        }


        public void OnNext(DeviceChangedArgs value)
        {
            if (value.ChangedType == DeviceChangedType.DefaultChanged ||
                value.ChangedType == DeviceChangedType.DeviceAdded || 
                value.ChangedType == DeviceChangedType.DeviceRemoved)
            {
                InvalidateDevice();
            }
        }

        public void OnError(Exception error)
        {
        }

        public void OnCompleted()
        {
        }
    }
}
