using System;
namespace Xamarin.Forms.Platform.Skia
{
    public interface IKeyboardHandler
    {
        event EventHandler<KeyboardKeyEventArgs> KeyboardKey;
        event EventHandler<KeyboardCharacterEventArgs> KeyboardCharacter;

        void ProcessKeyboardBuffer();
    }
}

