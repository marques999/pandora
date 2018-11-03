using System.Drawing;

namespace SexyTerminal
{
    partial class ColorPickerForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ColorPickerForm));
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.lblOld = new System.Windows.Forms.Label();
            this.lblNew = new System.Windows.Forms.Label();
            this.txtHex = new System.Windows.Forms.TextBox();
            this.lblHex = new System.Windows.Forms.Label();
            this.nudKey = new System.Windows.Forms.NumericUpDown();
            this.nudYellow = new System.Windows.Forms.NumericUpDown();
            this.nudMagenta = new System.Windows.Forms.NumericUpDown();
            this.nudCyan = new System.Windows.Forms.NumericUpDown();
            this.lblKey = new System.Windows.Forms.Label();
            this.lblYellow = new System.Windows.Forms.Label();
            this.lblMagenta = new System.Windows.Forms.Label();
            this.lblCyan = new System.Windows.Forms.Label();
            this.lblHue = new System.Windows.Forms.Label();
            this.lblBrightnessPerc = new System.Windows.Forms.Label();
            this.lblSaturationPerc = new System.Windows.Forms.Label();
            this.nudBlue = new System.Windows.Forms.NumericUpDown();
            this.nudGreen = new System.Windows.Forms.NumericUpDown();
            this.nudRed = new System.Windows.Forms.NumericUpDown();
            this.nudBrightness = new System.Windows.Forms.NumericUpDown();
            this.nudSaturation = new System.Windows.Forms.NumericUpDown();
            this.nudHue = new System.Windows.Forms.NumericUpDown();
            this.rbBlue = new System.Windows.Forms.RadioButton();
            this.rbGreen = new System.Windows.Forms.RadioButton();
            this.rbRed = new System.Windows.Forms.RadioButton();
            this.rbBrightness = new System.Windows.Forms.RadioButton();
            this.rbSaturation = new System.Windows.Forms.RadioButton();
            this.rbHue = new System.Windows.Forms.RadioButton();
            this.lblDecimal = new System.Windows.Forms.Label();
            this.txtDecimal = new System.Windows.Forms.TextBox();
            this.lblCyanPerc = new System.Windows.Forms.Label();
            this.lblMagentaPerc = new System.Windows.Forms.Label();
            this.lblYellowPerc = new System.Windows.Forms.Label();
            this.lblKeyPerc = new System.Windows.Forms.Label();
            this.nudAlpha = new System.Windows.Forms.NumericUpDown();
            this.lblAlpha = new System.Windows.Forms.Label();
            this.ttMain = new System.Windows.Forms.ToolTip(this.components);
            this.pCursorPosition = new System.Windows.Forms.Panel();
            this.txtY = new System.Windows.Forms.TextBox();
            this.txtX = new System.Windows.Forms.TextBox();
            this.lblY = new System.Windows.Forms.Label();
            this.lblX = new System.Windows.Forms.Label();
            this.lblCursorPosition = new System.Windows.Forms.Label();
            this.btnClose = new System.Windows.Forms.Button();
            this.pRecentColors = new System.Windows.Forms.Panel();
            this.lblRecentColors = new System.Windows.Forms.Label();
            this.flpRecentColors = new System.Windows.Forms.FlowLayoutPanel();
            this.btnScreenColorPicker = new System.Windows.Forms.Button();
            this.cbTransparent = new ColorButton();
            this.pbColorPreview = new MyPictureBox();
            this.colorPicker = new ColorPicker();
            ((System.ComponentModel.ISupportInitialize)(this.nudKey)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudYellow)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudMagenta)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudCyan)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudBlue)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudGreen)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudRed)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudBrightness)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudSaturation)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudHue)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudAlpha)).BeginInit();
            this.pCursorPosition.SuspendLayout();
            this.pRecentColors.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnOK
            // 
            this.btnOK.Name = "btnOK";
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // lblOld
            // 
            this.lblOld.Name = "lblOld";
            this.lblOld.Text = "Previous";
            // 
            // lblNew
            // 
            this.lblNew.Text = "Current";
            this.lblNew.Name = "lblNew";
            // 
            // txtHex
            // 
            this.txtHex.Text = "Hexadecimal";
            this.txtHex.Name = "txtHex";
            this.txtHex.TextChanged += new System.EventHandler(this.txtHex_TextChanged);
            // 
            // lblHex
            // 
            this.lblHex.Text = "Hexadecimal";
            this.lblHex.Name = "lblHex";
            // 
            // nudKey
            // 
            this.nudKey.DecimalPlaces = 1;
            this.nudKey.Name = "nudKey";
            int[] initialValue = new int[] {
            100,
            0,
            0,
            0};
            this.nudKey.Value = new decimal(initialValue);
            this.nudKey.ValueChanged += new System.EventHandler(this.CMYK_ValueChanged);
            // 
            // nudYellow
            // 
            this.nudYellow.DecimalPlaces = 1;
            this.nudYellow.Name = "nudYellow";
            this.nudYellow.Value = new decimal(initialValue);
            this.nudYellow.ValueChanged += new System.EventHandler(this.CMYK_ValueChanged);
            // 
            // nudMagenta
            // 
            this.nudMagenta.DecimalPlaces = 1;
            this.nudMagenta.Name = "nudMagenta";
            this.nudMagenta.Value = new decimal(initialValue);
            this.nudMagenta.ValueChanged += new System.EventHandler(this.CMYK_ValueChanged);
            // 
            // nudCyan
            // 
            this.nudCyan.DecimalPlaces = 1;
            this.nudCyan.Name = "nudCyan";
            this.nudCyan.Value = new decimal(initialValue);
            this.nudCyan.ValueChanged += new System.EventHandler(this.CMYK_ValueChanged);
            // 
            // lblKey
            // 
            this.lblKey.Text = "Key";
            this.lblKey.Name = "lblKey";
            // 
            // lblYellow
            // 
            this.lblYellow.Text = "Yellow";
            this.lblYellow.Name = "lblYellow";
            // 
            // lblMagenta
            // 
            this.lblMagenta.Text = "Magenta";
            this.lblMagenta.Name = "lblMagenta";
            // 
            // lblCyan
            // 
            this.lblCyan.Text = "Cyan";
            this.lblCyan.Name = "lblCyan";
            // 
            // lblHue
            // 
            this.lblHue.Text = "Hue";
            this.lblHue.Name = "lblHue";
            // 
            // lblBrightnessPerc
            // 
            this.lblBrightnessPerc.Text = "Brightness (%)";
            this.lblBrightnessPerc.Name = "lblBrightnessPerc";
            // 
            // lblSaturationPerc
            // 
            this.lblSaturationPerc.Text = "Saturation (%)";
            this.lblSaturationPerc.Name = "lblSaturationPerc";
            // 
            // nudBlue
            //
            int[] defaultBIts = new int[] {
            255,
            0,
            0,
            0};
            this.nudBlue.Maximum = new decimal(defaultBIts);
            this.nudBlue.Name = "nudBlue";
            this.nudBlue.Value = new decimal(defaultBIts);
            this.nudBlue.ValueChanged += new System.EventHandler(this.RGB_ValueChanged);
            // 
            // nudGreen
            // 
            this.nudGreen.Maximum = new decimal(defaultBIts);
            this.nudGreen.Name = "nudGreen";
            this.nudGreen.Value = new decimal(defaultBIts);
            this.nudGreen.ValueChanged += new System.EventHandler(this.RGB_ValueChanged);
            // 
            // nudRed
            // 
            this.nudRed.Maximum = new decimal(defaultBIts);
            this.nudRed.Name = "nudRed";
            this.nudRed.Value = new decimal(defaultBIts);
            this.nudRed.ValueChanged += new System.EventHandler(this.RGB_ValueChanged);
            // 
            // nudBrightness
            // 
            this.nudBrightness.Name = "nudBrightness";
            this.nudBrightness.Value = new decimal(initialValue);
            this.nudBrightness.ValueChanged += new System.EventHandler(this.HSB_ValueChanged);
            // 
            // nudSaturation
            // 
            this.nudSaturation.Name = "nudSaturation";
            this.nudSaturation.Value = new decimal(initialValue);
            this.nudSaturation.ValueChanged += new System.EventHandler(this.HSB_ValueChanged);
            // 
            // nudHue
            // 
            this.nudHue.Maximum = new decimal(new int[] {
            360,
            0,
            0,
            0});
            this.nudHue.Name = "nudHue";
            this.nudHue.Value = new decimal(new int[] {
            360,
            0,
            0,
            0});
            this.nudHue.ValueChanged += new System.EventHandler(this.HSB_ValueChanged);
            // 
            // rbBlue
            // 
            this.rbBlue.Text = "Blue";
            this.rbBlue.Name = "rbBlue";
            this.rbBlue.UseVisualStyleBackColor = true;
            this.rbBlue.CheckedChanged += new System.EventHandler(this.rbBlue_CheckedChanged);
            // 
            // rbGreen
            // 
            this.rbGreen.Text = "Green";
            this.rbGreen.Name = "rbGreen";
            this.rbGreen.UseVisualStyleBackColor = true;
            this.rbGreen.CheckedChanged += new System.EventHandler(this.rbGreen_CheckedChanged);
            // 
            // rbRed
            // 
            this.rbRed.Text = "Red";
            this.rbRed.Name = "rbRed";
            this.rbRed.UseVisualStyleBackColor = true;
            this.rbRed.CheckedChanged += new System.EventHandler(this.rbRed_CheckedChanged);
            // 
            // rbBrightness
            // 
            this.rbBrightness.Text = "Brightness";
            this.rbBrightness.Name = "rbBrightness";
            this.rbBrightness.UseVisualStyleBackColor = true;
            this.rbBrightness.CheckedChanged += new System.EventHandler(this.rbBrightness_CheckedChanged);
            // 
            // rbSaturation
            // 
            this.rbSaturation.Text = "Saturation";
            this.rbSaturation.Name = "rbSaturation";
            this.rbSaturation.UseVisualStyleBackColor = true;
            this.rbSaturation.CheckedChanged += new System.EventHandler(this.rbSaturation_CheckedChanged);
            // 
            // rbHue
            // 
            this.rbHue.Text = "Hue";
            this.rbHue.Checked = true;
            this.rbHue.Name = "rbHue";
            this.rbHue.TabStop = true;
            this.rbHue.UseVisualStyleBackColor = true;
            this.rbHue.CheckedChanged += new System.EventHandler(this.rbHue_CheckedChanged);
            // 
            // lblDecimal
            // 
            this.lblDecimal.Text = "Decimal";
            this.lblDecimal.Name = "lblDecimal";
            // 
            // txtDecimal
            // 
            this.txtDecimal.Name = "txtDecimal";
            this.txtDecimal.TextChanged += new System.EventHandler(this.txtDecimal_TextChanged);
            // 
            // lblCyanPerc
            // 
            this.lblCyanPerc.Text = "Cyan (%)";
            this.lblCyanPerc.Name = "lblCyanPerc";
            // 
            // lblMagentaPerc
            // 
            this.lblMagentaPerc.Text = "Magenta (%)";
            this.lblMagentaPerc.Name = "lblMagentaPerc";
            // 
            // lblYellowPerc
            // 
            this.lblYellowPerc.Text = "Yellow (%)";
            this.lblYellowPerc.Name = "lblYellowPerc";
            // 
            // lblKeyPerc
            // 
            this.lblKeyPerc.Text = "Key (%)";
            this.lblKeyPerc.Name = "lblKeyPerc";
            // 
            // nudAlpha
            // 
            this.nudAlpha.Maximum = new decimal(defaultBIts);
            this.nudAlpha.Name = "nudAlpha";
            this.nudAlpha.Value = new decimal(defaultBIts);
            this.nudAlpha.ValueChanged += new System.EventHandler(this.RGB_ValueChanged);
            // 
            // lblAlpha
            // 
            this.lblAlpha.Text = "Alpha";
            this.lblAlpha.Name = "lblAlpha";
            // 
            // ttMain
            // 
            this.ttMain.AutoPopDelay = 5000;
            this.ttMain.InitialDelay = 100;
            this.ttMain.ReshowDelay = 100;

            // 
            // pCursorPosition
            // 
            this.pCursorPosition.Controls.Add(this.txtY);
            this.pCursorPosition.Controls.Add(this.txtX);
            this.pCursorPosition.Controls.Add(this.lblY);
            this.pCursorPosition.Controls.Add(this.lblX);
            this.pCursorPosition.Controls.Add(this.lblCursorPosition);
            this.pCursorPosition.Name = "pCursorPosition";
            // 
            // txtY
            // 
 
            this.txtY.Name = "txtY";
            this.txtY.ReadOnly = true;
            // 
            // txtX
            // 

            this.txtX.Name = "txtX";
            this.txtX.ReadOnly = true;
            // 
            // lblY
            // 
            this.lblY.Text = "Y";
            this.lblY.Name = "lblY";
            // 
            // lblX
            // 
            this.lblX.Text = "X";
            this.lblX.Name = "lblX";
            // 
            // lblCursorPosition
            // 
            this.lblCursorPosition.Text = "Cursor";
            this.lblCursorPosition.Name = "lblCursorPosition";
            // 
            // btnClose
            // 
            this.btnClose.Text = "Close";
            this.btnClose.Name = "btnClose";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // pRecentColors
            // 
            this.pRecentColors.Controls.Add(this.lblRecentColors);
            this.pRecentColors.Controls.Add(this.flpRecentColors);
            this.pRecentColors.Name = "pRecentColors";
            // 
            // lblRecentColors
            // 
            this.lblRecentColors.Text = "Recent Colors";
            this.lblRecentColors.Name = "lblRecentColors";
            // 
            // flpRecentColors
            // 
            this.flpRecentColors.Name = "flpRecentColors";
            // 
            // btnScreenColorPicker
            // 
            this.btnScreenColorPicker.Text = "Screen";
            this.btnScreenColorPicker.Name = "btnScreenColorPicker";
            this.ttMain.SetToolTip(this.btnScreenColorPicker, "Screen");
            this.btnScreenColorPicker.UseVisualStyleBackColor = true;
            this.btnScreenColorPicker.Click += new System.EventHandler(this.btnScreenColorPicker_Click);

            // 
            // cbTransparent
            // 
            this.cbTransparent.Color = System.Drawing.Color.Transparent;
            this.cbTransparent.ManualButtonClick = true;
            this.cbTransparent.Name = "cbTransparent";
            this.ttMain.SetToolTip(this.cbTransparent, "Transparent Color");
            this.cbTransparent.UseVisualStyleBackColor = true;
            this.cbTransparent.Click += new System.EventHandler(this.cbTransparent_Click);
            // 
            // pbColorPreview
            // 
            this.pbColorPreview.BackColor = System.Drawing.SystemColors.Window;
            this.pbColorPreview.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pbColorPreview.Name = "pbColorPreview";
            this.pbColorPreview.MouseClick += new System.Windows.Forms.MouseEventHandler(this.pbColorPreview_MouseClick);
            // 
            // colorPicker
            // 
            this.colorPicker.DrawStyle = DrawStyle.Hue;
            this.colorPicker.Name = "colorPicker";
            this.colorPicker.ColorChanged += new ColorEventHandler(this.colorPicker_ColorChanged);
            // 
            // ColorPickerForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.CancelButton = this.btnCancel;
            this.Controls.Add(this.btnScreenColorPicker);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.pCursorPosition);
            this.Controls.Add(this.cbTransparent);
            this.Controls.Add(this.nudBlue);
            this.Controls.Add(this.nudGreen);
            this.Controls.Add(this.nudRed);
            this.Controls.Add(this.rbBlue);
            this.Controls.Add(this.rbGreen);
            this.Controls.Add(this.rbRed);
            this.Controls.Add(this.pbColorPreview);
            this.Controls.Add(this.lblAlpha);
            this.Controls.Add(this.nudAlpha);
            this.Controls.Add(this.lblKeyPerc);
            this.Controls.Add(this.lblYellowPerc);
            this.Controls.Add(this.lblMagentaPerc);
            this.Controls.Add(this.lblCyanPerc);
            this.Controls.Add(this.txtDecimal);
            this.Controls.Add(this.lblDecimal);
            this.Controls.Add(this.colorPicker);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.lblOld);
            this.Controls.Add(this.lblNew);
            this.Controls.Add(this.txtHex);
            this.Controls.Add(this.lblHex);
            this.Controls.Add(this.nudKey);
            this.Controls.Add(this.nudYellow);
            this.Controls.Add(this.nudMagenta);
            this.Controls.Add(this.nudCyan);
            this.Controls.Add(this.lblKey);
            this.Controls.Add(this.lblYellow);
            this.Controls.Add(this.lblMagenta);
            this.Controls.Add(this.lblCyan);
            this.Controls.Add(this.lblHue);
            this.Controls.Add(this.lblBrightnessPerc);
            this.Controls.Add(this.lblSaturationPerc);
            this.Controls.Add(this.nudBrightness);
            this.Controls.Add(this.nudSaturation);
            this.Controls.Add(this.nudHue);
            this.Controls.Add(this.rbBrightness);
            this.Controls.Add(this.rbSaturation);
            this.Controls.Add(this.rbHue);
            this.Controls.Add(this.pRecentColors);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "ColorPickerForm";
            this.Size = new Size(640, 480);
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Shown += new System.EventHandler(this.ColorPickerForm_Shown);
            ((System.ComponentModel.ISupportInitialize)(this.nudKey)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudYellow)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudMagenta)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudCyan)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudBlue)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudGreen)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudRed)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudBrightness)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudSaturation)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudHue)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudAlpha)).EndInit();
            this.pCursorPosition.ResumeLayout(false);
            this.pCursorPosition.PerformLayout();
            this.pRecentColors.ResumeLayout(false);
            this.pRecentColors.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion Windows Form Designer generated code

        private System.Windows.Forms.Label lblOld;
        private System.Windows.Forms.Label lblNew;
        private System.Windows.Forms.Label lblHex;
        private System.Windows.Forms.NumericUpDown nudKey;
        private System.Windows.Forms.NumericUpDown nudYellow;
        private System.Windows.Forms.NumericUpDown nudMagenta;
        private System.Windows.Forms.NumericUpDown nudCyan;
        private System.Windows.Forms.Label lblKey;
        private System.Windows.Forms.Label lblYellow;
        private System.Windows.Forms.Label lblMagenta;
        private System.Windows.Forms.Label lblCyan;
        private System.Windows.Forms.Label lblHue;
        private System.Windows.Forms.Label lblBrightnessPerc;
        private System.Windows.Forms.Label lblSaturationPerc;
        private System.Windows.Forms.NumericUpDown nudBlue;
        private System.Windows.Forms.NumericUpDown nudGreen;
        private System.Windows.Forms.NumericUpDown nudRed;
        private System.Windows.Forms.NumericUpDown nudBrightness;
        private System.Windows.Forms.NumericUpDown nudSaturation;
        private System.Windows.Forms.NumericUpDown nudHue;
        private System.Windows.Forms.RadioButton rbBlue;
        private System.Windows.Forms.RadioButton rbGreen;
        private System.Windows.Forms.RadioButton rbRed;
        private System.Windows.Forms.RadioButton rbBrightness;
        private System.Windows.Forms.RadioButton rbSaturation;
        private System.Windows.Forms.RadioButton rbHue;
        private System.Windows.Forms.Label lblDecimal;
        private System.Windows.Forms.TextBox txtDecimal;
        private System.Windows.Forms.Label lblCyanPerc;
        private System.Windows.Forms.Label lblMagentaPerc;
        private System.Windows.Forms.Label lblYellowPerc;
        private System.Windows.Forms.Label lblKeyPerc;
        private System.Windows.Forms.NumericUpDown nudAlpha;
        private System.Windows.Forms.Label lblAlpha;
        private MyPictureBox pbColorPreview;
        protected ColorPicker colorPicker;
        protected System.Windows.Forms.TextBox txtHex;
        protected System.Windows.Forms.Button btnCancel;
        protected System.Windows.Forms.Button btnOK;
        private ColorButton cbTransparent;
        private System.Windows.Forms.ToolTip ttMain;
        private System.Windows.Forms.Panel pCursorPosition;
        private System.Windows.Forms.Label lblCursorPosition;
        private System.Windows.Forms.Label lblX;
        private System.Windows.Forms.Label lblY;
        private System.Windows.Forms.TextBox txtX;
        private System.Windows.Forms.TextBox txtY;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Panel pRecentColors;
        private System.Windows.Forms.FlowLayoutPanel flpRecentColors;
        private System.Windows.Forms.Label lblRecentColors;
        private System.Windows.Forms.Button btnScreenColorPicker;
    }
}