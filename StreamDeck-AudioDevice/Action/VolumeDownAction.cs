using System;
using System.Threading.Tasks;
using StreamDeck.NET.Attribute;
using StreamDeck.NET.Message.Received;

namespace AudioDevice.Action
{
    [StreamDeckAction(ActionUUID.VolumeDown)]
    public class VolumeDownAction : BaseAudioDeviceAction
    {
        public override async Task KeyDown(StreamDeckKeyDownEventMessage message)
        {
            if (Device == null)
                return;

            if (Device.Volume <= 0)
                return;

            // TODO configurable steps?
            await Device.SetVolumeAsync(Math.Max(Device.Volume - 5, 0));
        }
    }

}
