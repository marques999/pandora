using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace RomLauncher
{
    /// <inheritdoc />
    /// <summary>
    /// </summary>
    public partial class MainForm : Form
    {
        /// <inheritdoc />
        /// <summary>
        /// </summary>
        public MainForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// </summary>
        public string[] Roms { get; set; }

        /// <summary>
        /// </summary>
        public string WorkingPath { get; set; }

        /// <summary>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArgs"></param>
        private void Form1_Load(object sender, EventArgs eventArgs)
        {
            Directory.CreateDirectory(@"C:\working\");

            if (File.Exists("zzz.path"))
            {
                folderBrowserDialog1.SelectedPath = File.ReadAllText("zzz.path");
            }
            else
            {
                folderBrowserDialog1.ShowDialog();
            }

            if (folderBrowserDialog1.SelectedPath == string.Empty)
            {
                switch (MessageBox.Show("Please select a directory before launching a game.", "Empty directory", MessageBoxButtons.RetryCancel, MessageBoxIcon.Exclamation))
                {
                case DialogResult.Retry:
                    Application.Restart();
                    break;
                case DialogResult.Cancel:
                    Application.Exit();
                    break;
                }
            }
            else
            {
                Roms = Directory.GetFiles(folderBrowserDialog1.SelectedPath, "*.7z", SearchOption.TopDirectoryOnly);
                WorkingPath = folderBrowserDialog1.SelectedPath;

                try
                {
                    foreach (var filename in Roms)
                    {
                        var row1 = new[]
                        {
                            filename,
                            new FileInfo(filename).Length / 1024 + " kB", "s3"
                        };

                        var path = filename.Remove(0, WorkingPath.Length + 1);

                        listView1.Items.Add(path.Remove(path.Length - 3, 3)).SubItems.AddRange(row1);
                        listView1.Columns[0].Text = "Games (" + Roms.Length + ")";
                    }
                }
                catch
                {
                    MessageBox.Show("Couldn't find any Nintendo 64 ROMs in the specified directory.");
                }
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArgs"></param>
        private void ListView1_ItemActivate(object sender, EventArgs eventArgs)
        {
            var path = listView1.SelectedItems[0].SubItems[1].Text;
            var path5 = listView1.SelectedItems[0].Text + ".n64";

            const string path2 = @"c:\working\Temp.7z";
            const string path6 = @"c:\working\Temp.n64";
            const string path3 = @"c:\working\7z.exe";
            const string path4 = "7z.exe";
            const string workingDirectory = @"C:\working\";

            try
            {
                File.Delete(path2);
                File.Delete(path3);

                using (File.OpenRead(path))
                {
                    File.Copy(path, path2);
                }

                using (File.OpenRead(path4))
                {
                    File.Copy(path4, path3);
                }
            }
            catch
            {
                Console.WriteLine("Double copy is not allowed, which was not expected.");
            }

            var decompress = new Process
            {
                StartInfo =
                {
                    FileName = path3,
                    WorkingDirectory = workingDirectory,
                    Arguments = "x Temp.7z",
                    CreateNoWindow = true
                }
            };

            decompress.Start();
            decompress.WaitForExit();

            using (File.OpenRead(workingDirectory + path5))
            {
                File.Delete(path6);
                File.Move(workingDirectory + path5, path6);
            }

            var emulator = new Process
            {
                StartInfo =
                {
                    Arguments = "Temp.n64",
                    WorkingDirectory = workingDirectory,
                    FileName = @"E:\Arcade\Emulators\Project64\Project64.exe"
                }
            };

            emulator.Start();
            emulator.WaitForExit();
            Show();
        }

        /// <summary>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArgs"></param>
        private void Form1_FormClosing(object sender, FormClosingEventArgs eventArgs)
        {
            if (Directory.Exists(@"C:\working\"))
            {
                Directory.Delete(@"C:\working\", true);
            }

            if (File.Exists("zzz.path") == false)
            {
                File.WriteAllText("zzz.path", folderBrowserDialog1.SelectedPath);
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArgs"></param>
        private void ListViewColumnClick(object sender, ColumnClickEventArgs eventArgs)
        {
            if (eventArgs.Column == 0)
            {
                if (listView1.Sorting == SortOrder.Descending)
                {
                    listView1.Sorting = SortOrder.Ascending;
                }
                else
                {
                    listView1.Sorting = SortOrder.Descending;
                }
            }

            listView1.Sort();
        }

        /// <summary>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArgs"></param>
        private void Button1_Click(object sender, EventArgs eventArgs)
        {
            new NameForm().ShowDialog();
        }

        /// <summary>
        /// </summary>
        [STAThread]
        private static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }
}