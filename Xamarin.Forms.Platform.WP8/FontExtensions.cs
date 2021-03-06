﻿using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace Xamarin.Forms.Platform.WinPhone
{
	public static class FontExtensions
	{
		public static void ApplyFont(this Control self, Font font)
		{
			if (font.UseNamedSize)
			{
				switch (font.NamedSize)
				{
					case NamedSize.Micro:
						self.FontSize = (double)System.Windows.Application.Current.Resources["PhoneFontSizeSmall"] - 3;
						break;
					case NamedSize.Small:
						self.FontSize = (double)System.Windows.Application.Current.Resources["PhoneFontSizeSmall"];
						break;
					case NamedSize.Medium:
						self.FontSize = (double)System.Windows.Application.Current.Resources["PhoneFontSizeMedium"];
						// use medium instead of normal as this is the default for non-labels
						break;
					case NamedSize.Large:
						self.FontSize = (double)System.Windows.Application.Current.Resources["PhoneFontSizeLarge"];
						break;
					default:
						throw new ArgumentOutOfRangeException();
				}
			}
			else
				self.FontSize = font.FontSize;

			if (!string.IsNullOrEmpty(font.FontFamily))
				self.FontFamily = new FontFamily(font.FontFamily);
			else
				self.FontFamily = (FontFamily)System.Windows.Application.Current.Resources["PhoneFontFamilySemiBold"];

			if (font.FontAttributes.HasFlag(FontAttributes.Italic))
				self.FontStyle = FontStyles.Italic;
			else
				self.FontStyle = FontStyles.Normal;

			if (font.FontAttributes.HasFlag(FontAttributes.Bold))
				self.FontWeight = FontWeights.Bold;
			else
				self.FontWeight = FontWeights.Normal;
		}

		public static void ApplyFont(this TextBlock self, Font font)
		{
			if (font.UseNamedSize)
			{
				switch (font.NamedSize)
				{
					case NamedSize.Micro:
						self.FontSize = (double)System.Windows.Application.Current.Resources["PhoneFontSizeSmall"] - 3;
						break;
					case NamedSize.Small:
						self.FontSize = (double)System.Windows.Application.Current.Resources["PhoneFontSizeSmall"];
						break;
					case NamedSize.Medium:
						self.FontSize = (double)System.Windows.Application.Current.Resources["PhoneFontSizeNormal"];
						// use normal instead of  medium as this is the default
						break;
					case NamedSize.Large:
						self.FontSize = (double)System.Windows.Application.Current.Resources["PhoneFontSizeLarge"];
						break;
					default:
						throw new ArgumentOutOfRangeException();
				}
			}
			else
				self.FontSize = font.FontSize;

			if (!string.IsNullOrEmpty(font.FontFamily))
				self.FontFamily = new FontFamily(font.FontFamily);
			else
				self.FontFamily = (FontFamily)System.Windows.Application.Current.Resources["PhoneFontFamilyNormal"];

			if (font.FontAttributes.HasFlag(FontAttributes.Italic))
				self.FontStyle = FontStyles.Italic;
			else
				self.FontStyle = FontStyles.Normal;

			if (font.FontAttributes.HasFlag(FontAttributes.Bold))
				self.FontWeight = FontWeights.Bold;
			else
				self.FontWeight = FontWeights.Normal;
		}

		public static void ApplyFont(this TextElement self, Font font)
		{
			if (font.UseNamedSize)
			{
				switch (font.NamedSize)
				{
					case NamedSize.Micro:
						self.FontSize = (double)System.Windows.Application.Current.Resources["PhoneFontSizeSmall"] - 3;
						break;
					case NamedSize.Small:
						self.FontSize = (double)System.Windows.Application.Current.Resources["PhoneFontSizeSmall"];
						break;
					case NamedSize.Medium:
						self.FontSize = (double)System.Windows.Application.Current.Resources["PhoneFontSizeNormal"];
						// use normal instead of  medium as this is the default
						break;
					case NamedSize.Large:
						self.FontSize = (double)System.Windows.Application.Current.Resources["PhoneFontSizeLarge"];
						break;
					default:
						throw new ArgumentOutOfRangeException();
				}
			}
			else
				self.FontSize = font.FontSize;

			if (!string.IsNullOrEmpty(font.FontFamily))
				self.FontFamily = new FontFamily(font.FontFamily);
			else
				self.FontFamily = (FontFamily)System.Windows.Application.Current.Resources["PhoneFontFamilyNormal"];

			if (font.FontAttributes.HasFlag(FontAttributes.Italic))
				self.FontStyle = FontStyles.Italic;
			else
				self.FontStyle = FontStyles.Normal;

			if (font.FontAttributes.HasFlag(FontAttributes.Bold))
				self.FontWeight = FontWeights.Bold;
			else
				self.FontWeight = FontWeights.Normal;
		}

		internal static void ApplyFont(this Control self, IFontElement element)
		{
			self.FontSize = element.FontSize;

			if (!string.IsNullOrEmpty(element.FontFamily))
				self.FontFamily = new FontFamily(element.FontFamily);
			else
				self.FontFamily = (FontFamily)System.Windows.Application.Current.Resources["PhoneFontFamilySemiBold"];

			if (element.FontAttributes.HasFlag(FontAttributes.Italic))
				self.FontStyle = FontStyles.Italic;
			else
				self.FontStyle = FontStyles.Normal;

			if (element.FontAttributes.HasFlag(FontAttributes.Bold))
				self.FontWeight = FontWeights.Bold;
			else
				self.FontWeight = FontWeights.Normal;
		}

		internal static bool IsDefault(this IFontElement self)
		{
			return self.FontFamily == null && self.FontSize == Device.GetNamedSize(NamedSize.Default, typeof(Label), true) && self.FontAttributes == FontAttributes.None;
		}
	}
}