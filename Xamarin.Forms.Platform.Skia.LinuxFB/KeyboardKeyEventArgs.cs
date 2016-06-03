using System;

namespace Xamarin.Forms.Platform.Skia
{
	public class KeyboardKeyEventArgs : EventArgs
	{
		public Key Key { get; private set; }

		public bool Down { get; private set; }

		public KeyboardKeyEventArgs (Key key, bool down)
		{
			Key = key;
			Down = down;
		}
	}
}

