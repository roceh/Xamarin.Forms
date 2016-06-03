using System;
using System.Collections.Generic;

namespace Xamarin.Forms.SkiaLinuxFBDemo
{
    public class Frogs : List<Frog>
    {
        public Frogs()
        {
        }
    }

    public class Frog
    {
        public string Name { get; set; }
        public string Image { get; set; }
        public string Description { get; set; }
    }
}

