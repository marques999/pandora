using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace MemcardRex.GUI
{
    /// <inheritdoc />
    /// <summary>
    /// </summary>
    public sealed class PluginsWindow : Form
    {
        /// <summary>
        /// </summary>
        private int _pluginIndex;

        /// <inheritdoc />
        /// <summary>
        /// </summary>
        public PluginsWindow(MainWindow mainWindow, IReadOnlyList<PluginMetadata> metadata)
        {
            var okButton = new Button
            {
                FlatStyle = FlatStyle.System,
                Location = new Point(376, 256),
                Name = "okButton",
                Size = new Size(76, 24),
                TabIndex = 0,
                Text = "OK",
                UseMnemonic = false,
                UseVisualStyleBackColor = true
            };

            var aboutButton = new Button
            {
                Location = new Point(104, 256),
                Name = "aboutButton",
                Size = new Size(64, 24),
                TabIndex = 3,
                Text = "About",
                UseVisualStyleBackColor = true
            };

            var configurationButton = new Button
            {
                Location = new Point(4, 256),
                Name = "configurationButton",
                Size = new Size(96, 24),
                TabIndex = 2,
                Text = "Configuration",
                UseVisualStyleBackColor = true
            };

            var pluginListView = new ListView
            {
                FullRowSelect = true,
                HeaderStyle = ColumnHeaderStyle.Nonclickable,
                HideSelection = false,
                Location = new Point(4, 4),
                MultiSelect = false,
                Name = "pluginListView",
                Size = new Size(448, 248),
                TabIndex = 1,
                UseCompatibleStateImageBehavior = false,
                View = View.Details
            };

            pluginListView.Columns.AddRange(new[]
            {
                new ColumnHeader
                {
                    Text = "Name",
                    Width = 131
                },
                new ColumnHeader
                {
                    Text = "Author",
                    Width = 92
                },
                new ColumnHeader
                {
                    Text = "Supported game(s)",
                    Width = 219
                }
            });

            pluginListView.SelectedIndexChanged += (sender, arguments) =>
            {
                if (pluginListView.SelectedIndices.Count != 0)
                {
                    _pluginIndex = pluginListView.SelectedIndices[0];
                }
            };

            aboutButton.Click += (sender, arguments) =>
            {
                if (metadata.Count > 0)
                {
                    mainWindow.PluginSystem.ShowAboutDialog(_pluginIndex);
                }
            };

            configurationButton.Click += (sender, arguments) =>
            {
                if (metadata.Count > 0)
                {
                    mainWindow.PluginSystem.ShowConfigDialog(_pluginIndex);
                }
            };

            SuspendLayout();
            okButton.Click += (sender, arguments) => Close();
            AutoScaleDimensions = new SizeF(6F, 13F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(456, 283);
            Controls.Add(pluginListView);
            Controls.Add(configurationButton);
            Controls.Add(aboutButton);
            Controls.Add(okButton);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "PluginsWindow";
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterParent;
            Text = "Plugins";
            ResumeLayout(false);

            for (var metadataIndex = 0; metadataIndex < metadata.Count; metadataIndex++)
            {
                pluginListView.Items.Add(metadata[metadataIndex].Name);
                pluginListView.Items[metadataIndex].SubItems.Add(metadata[metadataIndex].Author);
                pluginListView.Items[metadataIndex].SubItems.Add(metadata[metadataIndex].Games);
                pluginListView.Items[0].Selected = true;
            }
        }
    }
}