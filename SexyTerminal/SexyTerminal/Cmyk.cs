using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;

namespace SexyTerminal
{
    public struct Cmyk
    {
        private double _cyan;
        private double _magenta;
        private double _yellow;
        private double _key;
        private int _alpha;

        public double Cyan
        {
            get => _cyan;
            set => _cyan = ColorHelpers.ValidColor(value);
        }

        public double Cyan100
        {
            get => _cyan * 100;
            set => _cyan = ColorHelpers.ValidColor(value / 100);
        }

        public double Magenta
        {
            get => _magenta;
            set => _magenta = ColorHelpers.ValidColor(value);
        }

        public double Magenta100
        {
            get => _magenta * 100;
            set => _magenta = ColorHelpers.ValidColor(value / 100);
        }

        public double Yellow
        {
            get => _yellow;
            set => _yellow = ColorHelpers.ValidColor(value);
        }

        public double Yellow100
        {
            get => _yellow * 100;
            set => _yellow = ColorHelpers.ValidColor(value / 100);
        }

        public double Key
        {
            get => _key;
            set => _key = ColorHelpers.ValidColor(value);
        }

        public double Key100
        {
            get => _key * 100;
            set => _key = ColorHelpers.ValidColor(value / 100);
        }

        public int Alpha
        {
            get => _alpha;
            set => _alpha = ColorHelpers.ValidColor(value);
        }

        public Cmyk(double cyan, double magenta, double yellow, double key, int alpha = 255) : this()
        {
            Key = key;
            Cyan = cyan;
            Alpha = alpha;
            Yellow = yellow;
            Magenta = magenta;
        }

        public Cmyk(int cyan, int magenta, int yellow, int key, int alpha = 255) : this()
        {
            Key100 = key;
            Alpha = alpha;
            Cyan100 = cyan;
            Magenta100 = magenta;
            Yellow100 = yellow;
        }

        public Cmyk(Color color)
        {
            this = ColorHelpers.ColorToCmyk(color);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator Cmyk(Color color)
        {
            return ColorHelpers.ColorToCmyk(color);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator Color(Cmyk color)
        {
            return color.ToColor();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator Rgba(Cmyk color)
        {
            return color.ToColor();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator Hsb(Cmyk color)
        {
            return color.ToColor();
        }

        public static bool operator ==(Cmyk left, Cmyk right)
        {
            return (left.Cyan == right.Cyan) && (left.Magenta == right.Magenta) && (left.Yellow == right.Yellow) && (left.Key == right.Key);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(Cmyk left, Cmyk right)
        {
            return !(left == right);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Color ToColor()
        {
            return ColorHelpers.CmykToColor(this);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }
    }
}