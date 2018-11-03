using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ScintillaNET;
using SexyTerminal.Formats;

namespace SexyTerminal
{
    public partial class Form1 : Form
    {
        private static Color dummy = ColorHelpers.HexToColor("#336699");
        private static Color dummy2 = ColorHelpers.HexToColor("#996633");

        private static readonly Scheme SchemeDefault = new Scheme("brewer.light")
        {
            Background = ColorHelpers.HexToColor("1d1f21"),
            Foreground = ColorHelpers.HexToColor("c5c8c6"),
            Colors = new[]
            {
                ColorHelpers.HexToColor("282a2e"),
                ColorHelpers.HexToColor("a54242"),
                ColorHelpers.HexToColor("8c9440"),
                ColorHelpers.HexToColor("de935f"),
                ColorHelpers.HexToColor("5f819d"),
                ColorHelpers.HexToColor("85678f"),
                ColorHelpers.HexToColor("5d8d87"),
                ColorHelpers.HexToColor("707880"),
                ColorHelpers.HexToColor("373b41"),
                ColorHelpers.HexToColor("cc6666"),
                ColorHelpers.HexToColor("b5bd68"),
                ColorHelpers.HexToColor("f0c674"),
                ColorHelpers.HexToColor("81a2be"),
                ColorHelpers.HexToColor("b294bb"),
                ColorHelpers.HexToColor("8abeb7"),
                ColorHelpers.HexToColor("c5c8c6")
            }
        };

        private List<Scheme> palettes = new List<Scheme>
        {
           SchemeDefault,
            new Scheme("shit.dark")
            {
                Background = ColorHelpers.HexToColor("#fcfdfe"),
                Foreground = ColorHelpers.HexToColor("#515253"),
                Colors = new[]
                {
                    dummy2,
                    dummy,
                    dummy2,
                    dummy,
                    dummy2,
                    dummy,
                    dummy2,
                    dummy,
                    dummy2,
                    dummy,
                    dummy2,
                    dummy,
                    dummy2,
                    dummy,
                    dummy2,
                    dummy
                }
            }
        };

        private readonly ColorButton2[] buttons;

        public Form1()
        {
            InitializeComponent();
            buttons = new[]
            {
                colorButton1,
                colorButton10,
                colorButton2,
                colorButton3,
                colorButton4,
                colorButton5,
                colorButton6,
                colorButton7,
                colorButton8,
                colorButton9,
                colorButton11,
                colorButton12,
                colorButton13,
                colorButton14,
                colorButton15,
                colorButton16,
                colorButton17,
                colorButton18
            };

            listBox1.Items.AddRange(palettes.Select(scheme => scheme.Name).ToArray());
        }

        private void splitContainer1_Panel1_Paint(object sender, PaintEventArgs e)
        {
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (sender is ListBox listBox)
            {
                var itemIndex = listBox.SelectedIndex;

                if (itemIndex >= 0 && itemIndex < buttons.Length)
                {
                    InitializePalette(itemIndex);
                }
            }
        }

        private Scheme _current = SchemeDefault;
        private void InitializePalette(int itemIndex)
        {
            InitializePalette(palettes[itemIndex]);
        }

        private void InitializePalette(Scheme scheme)
        {
            var schemeColors = scheme.Colors;

            buttons[0].UpdateColor(scheme.Background);
            buttons[1].UpdateColor(scheme.Foreground);

            for (var index = 0; index < schemeColors.Length; index++)
            {
                buttons[index + 2].UpdateColor(schemeColors[index]);
            }

            scintilla1.StyleResetDefault();
            scintilla1.Styles[Style.Default].Font = "Consolas";
            scintilla1.Styles[Style.Default].Size = 11;
            scintilla1.Styles[Style.Default].BackColor = scheme.Background;
            scintilla1.Styles[Style.Default].ForeColor = scheme.Foreground;
            scintilla1.StyleClearAll();
            scintilla1.ResetText();
            scintilla1.AppendText(Export(scheme, _exporters[comboBox1.SelectedIndex]));

            Io.WritePreset(scheme.Name + ".bin", scheme);
        }
        /// <summary>
        /// </summary>
        /// <param name="scheme"></param>
        /// <returns></returns>
        internal string Export(Scheme scheme, IExporter exporter)
        {
            var builder = new StringBuilder();

            exporter.WriteHeader(builder, scheme);

            if (exporter.Group)
            {
                var maximum = scheme.Colors.Length / 2;

                for (var index = 0; index < maximum; index++)
                {
                    exporter.WriteColor(builder, scheme, index, index + maximum);
                }
            }
            else
            {
                for (var index = 0; index < scheme.Colors.Length; index++)
                {
                    exporter.WriteColor(builder, scheme, index, -1);
                }
            }

            exporter.WriteEpilogue(builder, scheme);

            return builder.ToString();
        }


        private void Form1_Load(object sender, EventArgs e)
        {
            InitializePalette(Io.ReadPreset("brewer.light.bin"));
        }

        private void colorButton2_Load(object sender, EventArgs e)
        {

        }

        private void PickColor(ColorButton2 colorButton)
        {
            colorDialog.Color = colorButton.BackColor;
            colorDialog.FullOpen = true;
            colorDialog.ShowDialog();
            colorButton.UpdateColor(colorDialog.Color);
        }

        private void ColorButtonClick(object sender, EventArgs e)
        {
            if (sender is ColorButton2 colorButton)
            {
                PickColor(colorButton);
            }
        }

        private static IExporter[] _exporters =
        {
            new Json(),
            new Konsole(),
            new XfceTerminal(),
            new Xresources(),
            new Xshell(),
            new Yaml()
        };

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (sender is ComboBox comboBox)
            {
                var itemIndex = comboBox.SelectedIndex;

                if (itemIndex >= 0 && itemIndex < _exporters.Length)
                {
                    scintilla1.ResetText();
                    scintilla1.AppendText(Export(_current, _exporters[itemIndex]));
                }
            }
        }
    }
}