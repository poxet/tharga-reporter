using System.Drawing;
using PdfSharp.Drawing;

namespace Tharga.Reporter.Engine.Entity.Element
{
    internal static class FontExtensions
    {
        internal static XFontStyle GetStyle(this Font font, Section section)
        {
            bool bold;
            bool italic;
            bool underline;
            bool strikeout;

            ////TODO: Use the font class (Somehow)

            if (font != null)
            {
                bold = font.Bold;
                italic = font.Italic;
                underline = font.Underline;
                strikeout = font.Strikeout;
            }
            else
            {
                bold = section.DefaultFont.Bold;
                italic = section.DefaultFont.Italic;
                underline = section.DefaultFont.Underline;
                strikeout = section.DefaultFont.Strikeout;
            }

            if (bold && italic)
                return XFontStyle.BoldItalic;
            if (bold)
                return XFontStyle.Bold;
            if (italic)
                return XFontStyle.Italic;
            if (underline)
                return XFontStyle.Underline;
            if (strikeout)
                return XFontStyle.Strikeout;

            return XFontStyle.Regular;
        }

        internal static string GetName(this Font font, Section section)
        {
            if (font != null)
                return font.FontName;

            ////TODO: Use the font class.
            
            return section.DefaultFont.FontName;
        }

        internal static double GetSize(this Font font, Section section)
        {
            if (font != null)
                return font.Size;

            ////TODO: Use the font class
            
            return section.DefaultFont.Size;
        }

        internal static Color GetColor(this Font font, Section section)
        {
            if (font != null)
                return font.Color;

            ////TODO: Use the font class

            return section.DefaultFont.Color;
        }
    }
}