using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace SexyTerminal
{
    public partial class MyPictureBox : UserControl
    {
        public Image Image
        {
            get
            {
                return pbMain.Image;
            }
            private set
            {
                pbMain.Image = value;
            }
        }

        private string text;

        [EditorBrowsable(EditorBrowsableState.Always), Browsable(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible), Bindable(true)]
        public override string Text
        {
            get
            {
                return text;
            }
            set
            {
                text = value;

                if (string.IsNullOrEmpty(value))
                {
                    lblStatus.Visible = false;
                }
                else
                {
                    lblStatus.Text = value;
                    lblStatus.Visible = true;
                }
            }
        }

        private bool drawCheckeredBackground;

        [DefaultValue(false)]
        public bool DrawCheckeredBackground
        {
            get
            {
                return drawCheckeredBackground;
            }
            set
            {
                drawCheckeredBackground = value;
                UpdateCheckers();
            }
        }

        [DefaultValue(false)]
        public bool FullscreenOnClick { get; set; }

        [DefaultValue(false)]
        public bool EnableRightClickMenu { get; set; }

        [DefaultValue(false)]
        public bool ShowImageSizeLabel { get; set; }

        public new event MouseEventHandler MouseDown
        {
            add
            {
                pbMain.MouseDown += value;
                lblStatus.MouseDown += value;
            }
            remove
            {
                pbMain.MouseDown -= value;
                lblStatus.MouseDown -= value;
            }
        }

        public new event MouseEventHandler MouseUp
        {
            add
            {
                pbMain.MouseUp += value;
                lblStatus.MouseUp += value;
            }
            remove
            {
                pbMain.MouseUp -= value;
                lblStatus.MouseUp -= value;
            }
        }

        public new event MouseEventHandler MouseClick
        {
            add
            {
                pbMain.MouseClick += value;
                lblStatus.MouseClick += value;
            }
            remove
            {
                pbMain.MouseClick -= value;
                lblStatus.MouseClick -= value;
            }
        }

        public bool IsValidImage
        {
            get
            {
                return !isImageLoading; //&& pbMain.IsValidImage();
            }
        }

        private readonly object imageLoadLock = new object();
        private BackgroundWorker backgroundWorker1;
        private bool isImageLoading;

        public MyPictureBox()
        {
            InitializeComponent();
            Text = "";
            pbMain.BackColor = SystemColors.Control;
            pbMain.LoadCompleted += pbMain_LoadCompleted;
            pbMain.Resize += pbMain_Resize;
            pbMain.MouseUp += MyPictureBox_MouseUp;
            pbMain.MouseEnter += PbMain_MouseEnter;
            pbMain.MouseLeave += PbMain_MouseLeave;
            MouseDown += MyPictureBox_MouseDown;
        }

        private void PbMain_MouseEnter(object sender, EventArgs e)
        {
            if (ShowImageSizeLabel && IsValidImage)
            {
                lblImageSize.Visible = true;
            }
        }

        private void PbMain_MouseLeave(object sender, EventArgs e)
        {
            lblImageSize.Visible = false;
        }

        private void pbMain_Resize(object sender, EventArgs e)
        {
            UpdateCheckers();
            AutoSetSizeMode();
        }

        private void UpdateCheckers()
        {
            if (DrawCheckeredBackground)
            {
                if (pbMain.BackgroundImage == null || pbMain.BackgroundImage.Size != pbMain.ClientSize)
                {
                    pbMain.BackgroundImage?.Dispose();
                   // pbMain.BackgroundImage = ImageHelpers.CreateCheckerPattern();
                }
            }
            else
            {
                pbMain.BackgroundImage?.Dispose();
                pbMain.BackgroundImage = null;
            }
        }

        public void LoadImage(Image img)
        {
            lock (imageLoadLock)
            {
                if (!isImageLoading)
                {
                    Reset();
                    isImageLoading = true;
                    Image = (Image)img.Clone();
                    isImageLoading = false;
                    AutoSetSizeMode();
                }
            }
        }

        public void LoadImageFromFile(string filePath)
        {
            lock (imageLoadLock)
            {
                if (!isImageLoading)
                {
                    Reset();
                    isImageLoading = true;
                   // Image = ImageHelpers.LoadImage(filePath);
                    isImageLoading = false;
                    AutoSetSizeMode();
                }
            }
        }

        public void LoadImageFromFileAsync(string filePath)
        {
            if (!string.IsNullOrEmpty(filePath) && File.Exists(filePath))
            {
                LoadImageAsync(filePath);
            }
        }

        public void LoadImageFromURLAsync(string url)
        {
            if (!string.IsNullOrEmpty(url) && !url.StartsWith("ftp://") && !url.StartsWith("ftps://"))
            {
                LoadImageAsync(url);
            }
        }

        private void LoadImageAsync(string path)
        {
            lock (imageLoadLock)
            {
                if (!isImageLoading)
                {
                    Reset();
                    isImageLoading = true;
                    lblStatus.Visible = true;
                    pbMain.LoadAsync(path);
                }
            }
        }

        public void Reset()
        {
            if (!isImageLoading && Image != null)
            {
                Image temp = null;

                try
                {
                    temp = Image;
                    Image = null;
                }
                finally
                {
                    // If error happened in previous image load then PictureBox set image as error image and if we dispose it then error happens
                    if (temp != null && temp != pbMain.ErrorImage && temp != pbMain.InitialImage)
                    {
                        temp.Dispose();
                    }
                }
            }

            if (FullscreenOnClick && Cursor != Cursors.Default)
            {
                Cursor = Cursors.Default;
            }
        }

        private void pbMain_LoadCompleted(object sender, AsyncCompletedEventArgs e)
        {
            lblStatus.Visible = false;
            isImageLoading = false;
            if (e.Error == null) AutoSetSizeMode();
        }

        private void AutoSetSizeMode()
        {
            if (IsValidImage)
            {
                lblImageSize.Text = $"{Image.Width} x {Image.Height}";

                if (Image.Width > pbMain.ClientSize.Width || Image.Height > pbMain.ClientSize.Height)
                {
                    pbMain.SizeMode = PictureBoxSizeMode.Zoom;
                }
                else
                {
                    pbMain.SizeMode = PictureBoxSizeMode.CenterImage;
                }

                if (FullscreenOnClick)
                {
                    Cursor = Cursors.Hand;
                }
            }
        }

        private void MyPictureBox_MouseDown(object sender, MouseEventArgs e)
        {
            //if (FullscreenOnClick && e.Button == MouseButtons.Left && IsValidImage)
           // {
           //     pbMain.Enabled = false;
           //     ImageViewer.ShowImage(Image);
           //     pbMain.Enabled = true;
           // }
        }

        private void MyPictureBox_MouseUp(object sender, MouseEventArgs e)
        {
            if (EnableRightClickMenu && e.Button == MouseButtons.Right && IsValidImage)
            {
                cmsMenu.Show(pbMain, e.X + 1, e.Y + 1);
            }
        }

        private void MyPictureBox_Resize(object sender, EventArgs e)
        {
            lblImageSize.Location = new Point((Width - lblImageSize.Width) / 2, Height - lblImageSize.Height);
        }

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

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MyPictureBox));
            this.lblStatus = new System.Windows.Forms.Label();
            this.pbMain = new System.Windows.Forms.PictureBox();
            this.cmsMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tsmiCopyImage = new System.Windows.Forms.ToolStripMenuItem();
            this.lblImageSize = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pbMain)).BeginInit();
            this.cmsMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblStatus
            // 
            this.lblStatus.BackColor = System.Drawing.Color.DimGray;
            resources.ApplyResources(this.lblStatus, "lblStatus");
            this.lblStatus.ForeColor = System.Drawing.Color.White;
            this.lblStatus.Name = "lblStatus";
            // 
            // pbMain
            // 
            resources.ApplyResources(this.pbMain, "pbMain");
            this.pbMain.Name = "pbMain";
            this.pbMain.TabStop = false;
            // 
            // cmsMenu
            // 
            this.cmsMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiCopyImage});
            this.cmsMenu.Name = "cmsMenu";
            this.cmsMenu.ShowImageMargin = false;
            resources.ApplyResources(this.cmsMenu, "cmsMenu");
            // 
            // tsmiCopyImage
            // 
            this.tsmiCopyImage.Name = "tsmiCopyImage";
            resources.ApplyResources(this.tsmiCopyImage, "tsmiCopyImage");
            // 
            // lblImageSize
            // 
            resources.ApplyResources(this.lblImageSize, "lblImageSize");
            this.lblImageSize.BackColor = System.Drawing.SystemColors.Window;
            this.lblImageSize.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblImageSize.Name = "lblImageSize";
            // 
            // MyPictureBox
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.Controls.Add(this.lblImageSize);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.pbMain);
            this.Name = "MyPictureBox";
            this.Resize += new System.EventHandler(this.MyPictureBox_Resize);
            ((System.ComponentModel.ISupportInitialize)(this.pbMain)).EndInit();
            this.cmsMenu.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion Component Designer generated code

        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.PictureBox pbMain;
        private System.Windows.Forms.ContextMenuStrip cmsMenu;
        private System.Windows.Forms.ToolStripMenuItem tsmiCopyImage;
        private System.Windows.Forms.Label lblImageSize;
    }
}