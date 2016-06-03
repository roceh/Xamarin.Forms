using System;
using System.Collections.Concurrent;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;

namespace Xamarin.Forms.Platform.Skia
{
    public class DefaultKeyboardHandler : IKeyboardHandler
    {
        const int EV_KEY = 0x01;
        const int EV_MSC = 0x04;
        const int EV_SYN = 0x00;
        const int KDGETLED = 0x4B31;
        const int MSC_SCAN = 0x04;

        bool _capsLock;
        string _device;
        ConcurrentQueue<input_event> _events = new ConcurrentQueue<input_event>();
        bool _hasShift;
        Thread _keyboardThread;

        public DefaultKeyboardHandler(string device)
        {
            _device = device;

            _keyboardThread = new Thread(new ThreadStart(KeyboardRun));
            _keyboardThread.Start();
        }

        public event EventHandler<KeyboardCharacterEventArgs> KeyboardCharacter;

        public event EventHandler<KeyboardKeyEventArgs> KeyboardKey;

        public void KeyboardRun()
        {
            FileStream fs = new FileStream(_device, System.IO.FileMode.Open, System.IO.FileAccess.Read);

            int structSize = Marshal.SizeOf(typeof(input_event));

            byte[] data = new byte[structSize];

            int bytes = 0;

            while (true)
            {
                bytes += fs.Read(data, bytes, structSize - bytes);

                if (bytes == structSize)
                {
                    lock (this)
                    {
                        _events.Enqueue(Utils.BytesToStruct<input_event>(data, 0));
                    }

                    bytes = 0;
                }
            }
        }

        public void ProcessKeyboardBuffer()
        {
            input_event inputEvent;

            while (_events.TryDequeue(out inputEvent))
            {
                if (inputEvent.type == EV_KEY)
                {
                    char character = (inputEvent.code >= 0 && inputEvent.code < KeyMap.Characters.Length) ? KeyMap.Characters[inputEvent.code] : (char)0;

                    if (character != 0 && (inputEvent.value == 1 || inputEvent.value == 2))
                    {
                        KeyboardCharacter?.Invoke(this, new KeyboardCharacterEventArgs((_capsLock ^ _hasShift) ? Char.ToUpper(character) : Char.ToLower(character)));
                    }

                    if ((Key)inputEvent.code == Key.CAPSLOCK && (inputEvent.value == 1 || inputEvent.value == 2))
                    {
                        _capsLock = !_capsLock;
                    }

                    if (((Key)inputEvent.code == Key.LEFTSHIFT || (Key)inputEvent.code == Key.RIGHTSHIFT))
                    {
                        _hasShift = inputEvent.value == 1 || inputEvent.value == 2;
                    }

                    KeyboardKey?.Invoke(this, new KeyboardKeyEventArgs((Key)inputEvent.code, inputEvent.value == 1 || inputEvent.value == 2));
                }
            }
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        struct input_event
        {
            public timeval time;
            public UInt16 type;
            public UInt16 code;
            public Int32 value;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        struct timeval
        {
            public Int32 tv_sec;
            public Int32 tv_usec;
        }
    }
}