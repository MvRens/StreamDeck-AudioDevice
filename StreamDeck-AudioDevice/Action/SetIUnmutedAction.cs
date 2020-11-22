using System.Threading.Tasks;
using StreamDeck.NET;
using StreamDeck.NET.Attribute;
using StreamDeck.NET.Message.Received;

namespace AudioDevice.Action
{
    [StreamDeckAction(ActionUUID.SetUnmuted)]
    public class SetUnmutedAction : BaseAudioDeviceAction
    {
        public SetUnmutedAction(IStreamDeckClient client) : base(client) { }

        // TODO support two states

        
        public override async Task KeyDown(StreamDeckKeyDownEventMessage message)
        {
            if (Device == null)
            {
                await Client.ShowAlert();
                return;
            }

            if (Device.IsMuted)
                await Device.SetMuteAsync(false);
        }
    }

}
