using System;
using System.Threading.Tasks;
using StreamDeck.NET;
using StreamDeck.NET.Attribute;
using StreamDeck.NET.Message.Received;

namespace AudioDevice.Action
{
    [StreamDeckAction(ActionUUID.SetDefault)]
    public class SetDefaultAction : BaseAudioDeviceAction
    {
        public SetDefaultAction(IStreamDeckClient client) : base(client) { }

        // TODO support two states
        
        public override async Task KeyDown(StreamDeckKeyDownEventMessage message)
        {
            if (Device == null)
            {
                await Client.ShowAlert();
                return;
            }

            await Device.SetAsDefaultAsync();
        }
    }

}
