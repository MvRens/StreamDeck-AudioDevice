using System;
using System.Threading.Tasks;
using StreamDeck.NET.Attribute;
using StreamDeck.NET.Message.Received;

namespace AudioDevice.Action
{
    [StreamDeckAction(ActionUUID.VolumeUp)]
    public class VolumeUpAction : BaseAudioDeviceAction
    {
        public override async Task KeyDown(StreamDeckKeyDownEventMessage message)
        {
            if (Device == null)
                return;

            if (Device.Volume >= 100)
                return;

            // TODO configurable steps?
            await Device.SetVolumeAsync(Math.Min(Device.Volume + 5, 100));
        }
    }

}
