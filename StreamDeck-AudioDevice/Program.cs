using System.Diagnostics;
using StreamDeck.NET;

namespace AudioDevice
{
    public class Program
    {
        public static void Main(string[] args)
        {
            #if DEBUG
            // To debug, Start without debugging and select the Visual Studio instance when the debug popup shows up.
            //Debugger.Launch();
            #endif

            StreamDeckApplication.Instance
                .RegisterAll()
                .Run(args);
        }
    }
}
