namespace Xamarin.Forms.Platform.Skia
{
    internal class ResourcesProvider : ISystemResourcesProvider
    {
        ResourceDictionary _dictionary;

        public ResourcesProvider()
        {
        }

        public IResourceDictionary GetSystemResources()
        {
            _dictionary = new ResourceDictionary();
            UpdateStyles();

            return _dictionary;
        }

        Style GenerateStyle(double pointSize, string fontName)
        {
            var result = new Style(typeof(Label));

            result.Setters.Add(new Setter { Property = Label.FontSizeProperty, Value = (double)pointSize });
            result.Setters.Add(new Setter { Property = Label.FontFamilyProperty, Value = fontName });

            return result;
        }

        void UpdateStyles()
        {
            _dictionary[Device.Styles.TitleStyleKey] = GenerateStyle(17, "Arial");
            _dictionary[Device.Styles.SubtitleStyleKey] = GenerateStyle(15, "Arial");
            _dictionary[Device.Styles.BodyStyleKey] = GenerateStyle(17, "Arial");
            _dictionary[Device.Styles.CaptionStyleKey] = GenerateStyle(12, "Arial");

            _dictionary[Device.Styles.ListItemTextStyleKey] = GenerateStyle(12, "Arial");
            _dictionary[Device.Styles.ListItemDetailTextStyleKey] = GenerateStyle(12, "Arial");
        }
    }
}