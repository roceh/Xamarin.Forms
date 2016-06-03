using System;

namespace Xamarin.Forms.Platform.Skia
{
	public class KeyboardCharacterEventArgs : EventArgs
	{
		public char Character { get; private set; }
		
		public KeyboardCharacterEventArgs (char character)
		{
			Character = character;
		}
	}
}

