using System;

namespace Zork.Common
{
    public delegate void InputReceivedHandler(object sender, string inputString);
    public interface IInputService
    {
        event EventHandler<string> InputReceived;
    }
}