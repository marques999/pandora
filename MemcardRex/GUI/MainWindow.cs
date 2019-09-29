using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MemcardRex.Properties;

namespace MemcardRex.GUI
{
    public partial class MainWindow : Form
    {
        private const string ApplicationName = "MemcardRex";
        private const string ApplicationVersion = "1.9 (Debug)";
        private const string SelectedSaveNotDeleted = "The selected save is not deleted.";
        private const string SelectedSaveAlreadyDeleted = "The selected save is already deleted.";
        private const string SelectedSlotLinked = "The selected slot is linked. Select the initial save slot to proceed.";
        private const string SelectedSlotNotEmpty = "The selected slot is not empty.";
        private const string TemporaryBufferIsEmpty = "Temporary buffer is empty.";
        private const string FileAlreadyOpened = "<{0}> is already opened.";
        private const string SaveFilter = "ePSXe/PSEmu Pro Memory Card (*.mcr)|*.mcr|DexDrive Memory Card (*.gme)|*.gme|pSX/AdriPSX Memory Card (*.bin)|*.bin|Bleem! Memory Card (*.mcd)|*.mcd|VGS Memory Card (*.mem, *.vgs)|*.mem; *.vgs|PSXGame Edit Memory Card (*.mc)|*.mc|DataDeck Memory Card (*.ddf)|*.ddf|WinPSM Memory Card (*.ps)|*.ps|Smart Link Memory Card (*.psm)|*.psm|MCExplorer (*.mci)|*.mci|PS3 virtual Memory Card (*.VM1)|*.VM1";
        private const string FreeSlotsRequired = "To complete this operation, {0} free slots are required.";

        private void ShowAndDispose(Form dialog)
        {
            dialog.ShowDialog(this);
            dialog.Dispose();
        }
        //Location of the application
        private readonly string _appPath = Application.StartupPath;


        //Plugin system (public because plugin dialog has to access it)
        public RexPluginSystem PluginSystem = new RexPluginSystem();

        //Supported plugins for the currently selected save
        private int[] _supportedPlugins;

        //Currently clicked plugin (0 - clicked flag, 1 - plugin index)
        private readonly int[] _clickedPlugin = { 0, 0 };

        //Struct holding all program related settings (public because settings dialog has to access it)
        public struct ProgramSettings
        {
            public int TitleEncoding;
            public int ShowListGrid;
            public int IconInterpolationMode;
            public int IconPropertiesSize;
            public int IconBackgroundColor;
            public int BackupMemcards;
            public int WarningMessage;
            public int RestoreWindowPosition;
            public int FormatType;
            public string ListFont;
            public string CommunicationPort;
        }

        //All program settings
        private ProgramSettings _settings;

        private readonly List<MemoryCard> _pScard = new List<MemoryCard>();
        private readonly List<ListView> _cardList = new List<ListView>();
        private readonly List<ImageList> _iconList = new List<ImageList>();

        private byte[] _tempBuffer;
        private string _tempBufferName;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void ApplySettings()
        {
            for (var i = 0; i < _cardList.Count; i++)
                RefreshListView(i, _cardList[i].SelectedIndices[0]);

            mainStatusStrip.Visible = true;
        }

        public void ApplyProgramSettings(ProgramSettings progSettings)
        {
            _settings = progSettings;
            ApplySettings();
        }

        private void LoadProgramSettings()
        {
            if (!File.Exists(_appPath + "/Settings.xml"))
            {
                return;
            }

            using (var xmlReader = new XmlReader(_appPath + "/Settings.xml"))
            {
                _settings.ListFont = xmlReader.Read("ListFont");
                _settings.CommunicationPort = xmlReader.Read("ComPort");
                _settings.TitleEncoding = xmlReader.ReadInteger("TitleEncoding", 0, 1);
                _settings.ShowListGrid = xmlReader.ReadInteger("ShowGrid", 0, 1);
                _settings.IconInterpolationMode = xmlReader.ReadInteger("IconInterpolationMode", 0, 1);
                _settings.IconPropertiesSize = xmlReader.ReadInteger("IconSize", 0, 1);
                _settings.IconBackgroundColor = xmlReader.ReadInteger("IconBackgroundColor", 0, 4);
                _settings.BackupMemcards = xmlReader.ReadInteger("BackupMemoryCards", 0, 1);
                _settings.WarningMessage = xmlReader.ReadInteger("WarningMessage", 0, 1);
                _settings.RestoreWindowPosition = xmlReader.ReadInteger("RestoreWindowPosition", 0, 1);
                _settings.FormatType = xmlReader.ReadInteger("HardwareFormatType", 0, 1);

                var windowLocation = new Point(0, 0);

                if (_settings.RestoreWindowPosition == 1)
                {
                    windowLocation.X = xmlReader.ReadInteger("WindowX", -65535, 65535);
                    windowLocation.Y = xmlReader.ReadInteger("WindowY", -65535, 65535);
                    Location = windowLocation;
                }
            }

            ApplySettings();
        }

        private void SaveProgramSettings()
        {
            using (var xmlWriter = new XmlWriter(_appPath + "/Settings.xml"))
            {
                xmlWriter.Write("ListFont", _settings.ListFont);
                xmlWriter.Write("ComPort", _settings.CommunicationPort);
                xmlWriter.Write("TitleEncoding", _settings.TitleEncoding.ToString());
                xmlWriter.Write("ShowGrid", _settings.ShowListGrid.ToString());
                xmlWriter.Write("IconInterpolationMode", _settings.IconInterpolationMode.ToString());
                xmlWriter.Write("IconSize", _settings.IconPropertiesSize.ToString());
                xmlWriter.Write("IconBackgroundColor", _settings.IconBackgroundColor.ToString());
                xmlWriter.Write("BackupMemoryCards", _settings.BackupMemcards.ToString());
                xmlWriter.Write("WarningMessage", _settings.WarningMessage.ToString());
                xmlWriter.Write("RestoreWindowPosition", _settings.RestoreWindowPosition.ToString());
                xmlWriter.Write("HardwareFormatType", _settings.FormatType.ToString());
                xmlWriter.Write("WindowX", Location.X.ToString());
                xmlWriter.Write("WindowY", Location.Y.ToString());
            }
        }

        private void BackupMemcard(string fileName)
        {
            if (_settings.BackupMemcards != 1 || fileName == null)
            {
                return;
            }

            var fileInfo = new FileInfo(fileName);

            if (fileInfo.Length >= 524288)
            {
                return;
            }

            if (!Directory.Exists(_appPath + "/Backup")) Directory.CreateDirectory(_appPath + "/Backup");

            File.Copy(fileName, _appPath + "/Backup/" + fileInfo.Name);
        }

        private void FilterNullCard()
        {
            if (_pScard.Count > 0 && _pScard.Count == 2 && _pScard[0].CardLocation == null && _pScard[0].WasChanged == false)
            {
                CloseCard(0);
            }
        }

        private void OpenCardDialog()
        {
            var openFileDialog = new OpenFileDialog
            {
                Title = "Open Memory Card",
                Filter = "All supported|*.mcr;*.gme;*.bin;*.mcd;*.mem;*.vgs;*.mc;*.ddf;*.ps;*.psm;*.mci;*.VMP;*.VM1|ePSXe/PSEmu Pro Memory Card (*.mcr)|*.mcr|DexDrive Memory Card (*.gme)|*.gme|pSX/AdriPSX Memory Card (*.bin)|*.bin|Bleem! Memory Card (*.mcd)|*.mcd|VGS Memory Card (*.mem, *.vgs)|*.mem; *.vgs|PSXGame Edit Memory Card (*.mc)|*.mc|DataDeck Memory Card (*.ddf)|*.ddf|WinPSM Memory Card (*.ps)|*.ps|Smart Link Memory Card (*.psm)|*.psm|MCExplorer (*.mci)|*.mci|PSP virtual Memory Card (*.VMP)|*.VMP|PS3 virtual Memory Card (*.VM1)|*.VM1|All files (*.*)|*.*",
                Multiselect = true
            };

            if (openFileDialog.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            foreach (var fileName in openFileDialog.FileNames)
            {
                OpenCard(fileName);
            }
        }

        private void OpenCard(string fileName)
        {
            foreach (var checkCard in _pScard)
            {
                if (checkCard.CardLocation != fileName || fileName == null)
                {
                    continue;
                }

                Messages.Warning(ApplicationName, string.Format(FileAlreadyOpened, Path.GetFileName(fileName)));

                return;
            }

            _pScard.Add(new MemoryCard());

            var message = _pScard[_pScard.Count - 1].OpenMemoryCard(fileName);

            if (message == null)
            {
                BackupMemcard(fileName);
                CreateTabPage();
            }
            else
            {
                _pScard.RemoveAt(_pScard.Count - 1);
                Messages.Warning(ApplicationName, message);
            }
        }

        private void CreateTabPage()
        {
            var tabPage = new TabPage
            {
                BackColor = SystemColors.Window
            };

            mainTabControl.TabPages.Add(tabPage);
            MakeListView();
            tabPage.Controls.Add(_cardList[_cardList.Count - 1]);

            if (_pScard[_pScard.Count - 1].CardLocation != null)
            {
                FilterNullCard();
            }

            mainTabControl.SelectedIndex = _pScard.Count - 1;
            RefreshStatusStrip();
            closeToolStripMenuItem.Enabled = true;
            closeAllToolStripMenuItem.Enabled = true;
            saveToolStripMenuItem.Enabled = true;
            saveButton.Enabled = true;
            saveAsToolStripMenuItem.Enabled = true;
        }

        /// <summary>
        /// </summary>
        /// <param name="listIndex"></param>
        private void SaveCardDialog(int listIndex)
        {
            if (_pScard.Count <= 0)
            {
                return;
            }

            var saveFileDialog = new SaveFileDialog
            {
                Title = "Save Memory Card",
                Filter = SaveFilter
            };

            if (saveFileDialog.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            switch (saveFileDialog.FilterIndex)
            {
            case 2:
                SaveMemoryCard(listIndex, saveFileDialog.FileName, 2);
                break;
            case 5:
                SaveMemoryCard(listIndex, saveFileDialog.FileName, 3);
                break;
            default:
                SaveMemoryCard(listIndex, saveFileDialog.FileName, 1);
                break;
            }
        }

        private void SaveMemoryCard(int listIndex, string fileName, byte memoryCardType)
        {
            if (_pScard[listIndex].SaveMemoryCard(fileName, memoryCardType))
            {
                RefreshListView(listIndex, _cardList[listIndex].SelectedIndices[0]);
                RefreshStatusStrip();
            }
            else
            {
                Messages.Warning(ApplicationName, "Memory card could not be saved.");
            }
        }

        private void SaveCardFunction(int listIndex)
        {
            if (_pScard.Count <= 0)
            {
                return;
            }

            if (_pScard[listIndex].CardLocation == null || _pScard[listIndex].CardType == 4)
            {
                SaveCardDialog(listIndex);
            }
            else
            {
                SaveMemoryCard(listIndex, _pScard[listIndex].CardLocation, _pScard[listIndex].CardType);
            }
        }

        private void CloseCard(int listIndex, bool switchToFirst = true)
        {
            if (_pScard.Count > 0)
            {
                SavePrompt(listIndex);
                _pScard.RemoveAt(listIndex);
                _cardList.RemoveAt(listIndex);
                _iconList.RemoveAt(listIndex);
                mainTabControl.TabPages.RemoveAt(listIndex);

                if (_pScard.Count > 0 && switchToFirst)
                {
                    mainTabControl.SelectedIndex = 0;
                }

                RefreshPluginBindings();
                EnableSelectiveEditItems();
            }

            if (_pScard.Count > 0)
            {
                return;
            }

            saveButton.Enabled = false;
            saveToolStripMenuItem.Enabled = false;
            closeToolStripMenuItem.Enabled = false;
            saveAsToolStripMenuItem.Enabled = false;
            closeAllToolStripMenuItem.Enabled = false;
        }

        private void CloseAllCards()
        {
            while (_pScard.Count > 0)
            {
                mainTabControl.SelectedIndex = 0;
                CloseCard(0);
            }
        }

        private void EditSaveComments()
        {
            if (_pScard.Count <= 0)
            {
                return;
            }

            var listIndex = mainTabControl.SelectedIndex;
            var cardListView = _cardList[listIndex];

            if (cardListView.SelectedIndices.Count <= 0)
            {
                return;
            }

            var memoryCard = _pScard[listIndex];
            var slotNumber = cardListView.SelectedIndices[0];
            var saveTitle = memoryCard.SaveName[slotNumber, _settings.TitleEncoding];
            var saveComment = memoryCard.SaveComments[slotNumber];

            if (memoryCard.SaveType[slotNumber] != 1 && memoryCard.SaveType[slotNumber] != 4)
            {
                return;
            }

            var commentsWindow = new CommentsWindow(saveTitle, saveComment);

            if (commentsWindow.ShowDialog(this) == DialogResult.OK)
            {
                memoryCard.SaveComments[slotNumber] = commentsWindow.Comment;
            }

            commentsWindow.Dispose();
        }

        /// <summary>
        /// </summary>
        private void ShowInformation()
        {
            if (_pScard.Count <= 0)
            {
                return;
            }

            var listIndex = mainTabControl.SelectedIndex;

            if (_cardList[listIndex].SelectedIndices.Count <= 0)
            {
                return;
            }

            var slotNumber = _cardList[listIndex].SelectedIndices[0];
            var saveRegion = _pScard[listIndex].SaveRegion[slotNumber];
            var saveSize = _pScard[listIndex].SaveSize[slotNumber];
            var iconFrames = _pScard[listIndex].IconFrames[slotNumber];
            var saveProdCode = _pScard[listIndex].SaveProdCode[slotNumber];
            var saveIdentifier = _pScard[listIndex].SaveIdentifier[slotNumber];
            var saveTitle = _pScard[listIndex].SaveName[slotNumber, _settings.TitleEncoding];
            var saveIcons = new Bitmap[3];

            for (var i = 0; i < 3; i++)
            {
                saveIcons[i] = _pScard[listIndex].IconData[slotNumber, i];
            }

            if (_pScard[listIndex].SaveType[slotNumber] == 1 || _pScard[listIndex].SaveType[slotNumber] == 4)
                ShowAndDispose(new InformationWindow(
                    saveTitle,
                    saveProdCode,
                    saveIdentifier,
                    saveRegion,
                    saveSize,
                    iconFrames,
                    _settings.IconInterpolationMode,
                    _settings.IconPropertiesSize,
                    saveIcons, _pScard[listIndex].FindSaveLinks(slotNumber), _settings.IconBackgroundColor
                ));
        }

        /// <summary>
        /// </summary>
        private void RestoreSave()
        {
            if (_pScard.Count <= 0)
            {
                return;
            }

            var listIndex = mainTabControl.SelectedIndex;
            var cardListView = _cardList[listIndex];

            if (cardListView.SelectedIndices.Count <= 0)
            {
                return;
            }

            var memoryCard = _pScard[listIndex];
            var slotIndex = cardListView.SelectedIndices[0];
            var saveType = memoryCard.SaveType[slotIndex];

            if (saveType == 4)
            {
                memoryCard.ToggleDeleteSave(slotIndex);
                RefreshListView(listIndex, slotIndex);
            }
            else if (saveType == 1)
            {
                Messages.Warning(ApplicationName, SelectedSaveNotDeleted);
            }
            else
            {
                Messages.Warning(ApplicationName, SelectedSlotLinked);
            }
        }

        /// <summary>
        /// </summary>
        private void DeleteSave()
        {
            if (_pScard.Count <= 0)
            {
                return;
            }

            var listIndex = mainTabControl.SelectedIndex;
            var cardListView = _cardList[listIndex];

            if (cardListView.SelectedIndices.Count <= 0)
            {
                return;
            }

            var memoryCard = _pScard[listIndex];
            var slotIndex = cardListView.SelectedIndices[0];
            var saveType = memoryCard.SaveType[slotIndex];

            if (saveType == 1)
            {
                memoryCard.ToggleDeleteSave(slotIndex);
                RefreshListView(listIndex, slotIndex);
            }
            else if (saveType == 4)
            {
                Messages.Warning(ApplicationName, SelectedSaveAlreadyDeleted);
            }
            else
            {
                Messages.Warning(ApplicationName, SelectedSlotLinked);
            }
        }

        /// <summary>
        /// </summary>
        private void FormatSave()
        {
            if (_pScard.Count <= 0)
            {
                return;
            }

            var listIndex = mainTabControl.SelectedIndex;
            var cardListView = _cardList[listIndex];

            if (cardListView.SelectedIndices.Count <= 0)
            {
                return;
            }

            var slotIndex = cardListView.SelectedIndices[0];
            var saveType = _pScard[listIndex].SaveType[slotIndex];

            if (saveType == 2 || saveType == 3 || saveType == 5 || saveType == 6)
            {
                Messages.Warning(ApplicationName, SelectedSlotLinked);
            }
            else if (Messages.Prompt(ApplicationName, "Formatted slots cannot be restored.\nDo you want to proceed with this operation?") == DialogResult.Yes)
            {
                _pScard[listIndex].FormatSave(slotIndex);
                RefreshListView(listIndex, slotIndex);
            }
        }

        /// <summary>
        /// </summary>
        private void CopySave()
        {
            if (_pScard.Count <= 0)
            {
                return;
            }

            var listIndex = mainTabControl.SelectedIndex;
            var cardListView = _cardList[listIndex];

            if (cardListView.SelectedIndices.Count <= 0)
            {
                return;
            }

            var memoryCard = _pScard[listIndex];
            var slotIndex = cardListView.SelectedIndices[0];
            var saveType = memoryCard.SaveType[slotIndex];

            if (saveType == 1 || saveType == 4)
            {
                _tempBuffer = memoryCard.GetSaveBytes(slotIndex);
                _tempBufferName = memoryCard.SaveName[slotIndex, 0];
                tBufToolButton.Enabled = true;
                tBufToolButton.Image = memoryCard.IconData[slotIndex, 0];
                tBufToolButton.Text = memoryCard.SaveName[slotIndex, 0];
                RefreshListView(listIndex, slotIndex);
            }
            else
            {
                Messages.Warning(ApplicationName, SelectedSlotLinked);
            }
        }

        /// <summary>
        /// </summary>
        private void PasteSave()
        {
            if (_pScard.Count <= 0)
            {
                return;
            }

            var listIndex = mainTabControl.SelectedIndex;

            if (_cardList[listIndex].SelectedIndices.Count <= 0)
            {
                return;
            }

            var slotNumber = _cardList[listIndex].SelectedIndices[0];

            if (_tempBuffer == null)
            {
                Messages.Warning(ApplicationName, TemporaryBufferIsEmpty);
            }
            else if (_pScard[listIndex].SaveType[slotNumber] == 0)
            {
                if (_pScard[listIndex].SetSaveBytes(slotNumber, _tempBuffer, out var slotsRequired))
                {
                    RefreshListView(listIndex, slotNumber);
                }
                else
                {
                    Messages.Warning(ApplicationName, string.Format(FreeSlotsRequired, slotsRequired));
                }
            }
            else
            {
                Messages.Warning(ApplicationName, SelectedSlotNotEmpty);
            }
        }

        /// <summary>
        /// </summary>
        private void ExportSaveDialog()
        {
            if (_pScard.Count <= 0)
            {
                return;
            }

            var listIndex = mainTabControl.SelectedIndex;
            var cardListView = _cardList[listIndex];

            if (cardListView.SelectedIndices.Count <= 0)
            {
                return;
            }

            var memoryCard = _pScard[listIndex];
            var slotNumber = cardListView.SelectedIndices[0];
            var saveType = memoryCard.SaveType[slotNumber];

            if (saveType == 1)
            {
                var outputFilename = GetRegionString(memoryCard.SaveRegion[slotNumber]) +
                                     memoryCard.SaveProdCode[slotNumber] +
                                     memoryCard.SaveIdentifier[slotNumber];

                outputFilename = Path.GetInvalidPathChars().Aggregate(outputFilename,
                    (current, illegalChar) => current.Replace(illegalChar.ToString(), ""));

                var saveFileDialog = new SaveFileDialog
                {
                    Title = "Export save",
                    FileName = outputFilename,
                    Filter =
                        "PSXGameEdit single save (*.mcs)|*.mcs|XP, AR, GS, Caetla single save (*.psx)|*.psx|Memory Juggler (*.ps1)|*.ps1|Smart Link (*.mcb)|*.mcb|Datel (*.mcx;*.pda)|*.mcx;*.pda|RAW single save|B???????????*"
                };

                if (saveFileDialog.ShowDialog() != DialogResult.OK)
                {
                    return;
                }

                if (saveFileDialog.FilterIndex == 1 || saveFileDialog.FilterIndex == 3)
                {
                    memoryCard.SaveSingleSave(saveFileDialog.FileName, slotNumber, 2);
                }
                else if (saveFileDialog.FilterIndex == 6)
                {

                    saveFileDialog.FileName = saveFileDialog.FileName.Split('.')[0];
                    memoryCard.SaveSingleSave(saveFileDialog.FileName, slotNumber, 3);
                }
                else
                {
                    memoryCard.SaveSingleSave(saveFileDialog.FileName, slotNumber, 1);
                }

                saveFileDialog.Dispose();
            }
            else if (saveType == 4)
            {
                Messages.Warning(ApplicationName, "Deleted saves cannot be exported. Restore a save to proceed.");
            }
            else
            {
                Messages.Warning(ApplicationName, SelectedSlotLinked);
            }
        }

        private void ImportSaveDialog()
        {
            if (_pScard.Count > 0)
            {
                var listIndex = mainTabControl.SelectedIndex;

                if (_cardList[listIndex].SelectedIndices.Count == 0) return;

                var slotNumber = _cardList[listIndex].SelectedIndices[0];

                if (_pScard[listIndex].SaveType[slotNumber] == 0)
                {
                    var openFileDialog = new OpenFileDialog
                    {
                        Title = "Import",
                        Filter =
                            "All supported|*.mcs;*.psx;*.ps1;*.mcb;*.mcx;*.pda;B???????????*;*.psv|PSXGameEdit single save (*.mcs)|*.mcs|XP, AR, GS, Caetla single save (*.psx)|*.psx|Memory Juggler (*.ps1)|*.ps1|Smart Link (*.mcb)|*.mcb|Datel (*.mcx;*.pda)|*.mcx;*.pda|RAW single save|B???????????*|PS3 virtual save (*.psv)|*.psv"
                    };

                    if (openFileDialog.ShowDialog() != DialogResult.OK)
                    {
                        return;
                    }

                    if (_pScard[listIndex].OpenSingleSave(openFileDialog.FileName, slotNumber, out var requiredSlots))
                    {
                        RefreshListView(listIndex, slotNumber);
                    }
                    else if (requiredSlots > 0)
                    {
                        Messages.Warning(ApplicationName, $"To complete this operation {requiredSlots} free slots are required.");
                    }
                    else
                    {
                        Messages.Warning(ApplicationName, "File could not be opened.");
                    }
                }
                else
                {
                    Messages.Warning(ApplicationName, SelectedSlotNotEmpty);
                }
            }
        }

        private static string GetRegionString(ushort regionUshort)
        {
            return Encoding.Default.GetString(new byte[]
            {
                (byte)(regionUshort & 0xFF),
                (byte)(regionUshort >> 8),
                0x00
            });
        }

        private void SavePrompt(int listIndex)
        {
            if (_pScard[listIndex].WasChanged && Messages.Prompt(ApplicationName, "Do you want to save changes to <" + _pScard[listIndex].CardName + ">?") == DialogResult.Yes)
            {
                SaveCardFunction(listIndex);
            }
        }

        private void EditIcon()
        {
            if (_pScard.Count <= 0)
            {
                return;
            }

            var listIndex = mainTabControl.SelectedIndex;
            var cardListView = _cardList[listIndex];

            if (cardListView.SelectedIndices.Count <= 0)
            {
                return;
            }

            var memoryCard = _pScard[listIndex];
            var slotIndex = cardListView.SelectedIndices[0];
            var saveType = memoryCard.SaveType[slotIndex];

            if (saveType == 1 || saveType == 4)
            {
                var iconWindow = new IconWindow(
                    memoryCard.SaveName[slotIndex, _settings.TitleEncoding],
                    memoryCard.IconFrames[slotIndex],
                    memoryCard.GetIconBytes(slotIndex)
                );

                iconWindow.ShowDialog(this);

                if (iconWindow.OkPressed)
                {
                    memoryCard.SetIconBytes(slotIndex, iconWindow.IconData);
                    RefreshListView(listIndex, slotIndex);
                }

                iconWindow.Dispose();
            }
            else
            {
                Messages.Warning(ApplicationName, SelectedSlotLinked);
            }
        }

        private void ShowPluginsWindow()
        {
            ShowAndDispose(new PluginsWindow(this, PluginSystem.AssembliesMetadata));
        }

        private void MakeListView()
        {
            _iconList.Add(new ImageList());

            var iconListView = _iconList[_iconList.Count - 1];

            iconListView.ImageSize = new Size(48, 16);
            iconListView.ColorDepth = ColorDepth.Depth32Bit;
            iconListView.TransparentColor = Color.Magenta;
            _cardList.Add(new ListView());

            var cardListView = _cardList[_cardList.Count - 1];

            cardListView.Location = new Point(0, 3);
            cardListView.Size = new Size(492, 286);
            cardListView.SmallImageList = iconListView;
            cardListView.ContextMenuStrip = mainContextMenu;
            cardListView.FullRowSelect = true;
            cardListView.MultiSelect = false;
            cardListView.HeaderStyle = ColumnHeaderStyle.Nonclickable;
            cardListView.HideSelection = false;
            cardListView.Columns.Add("Icon, region and title");
            cardListView.Columns.Add("Product code");
            cardListView.Columns.Add("Identifier");
            cardListView.Columns[0].Width = 315;
            cardListView.Columns[1].Width = 87;
            cardListView.Columns[2].Width = 84;
            cardListView.View = View.Details;
            cardListView.DoubleClick += cardList_DoubleClick;
            cardListView.SelectedIndexChanged += cardList_IndexChanged;
            RefreshListView(_cardList.Count - 1, 0);
        }

        private void RefreshListView(int listIndex, int slotNumber)
        {
            var iconListView = _iconList[listIndex];
            var cardListView = _cardList[listIndex];

            mainTabControl.TabPages[listIndex].Text = _pScard[listIndex].CardName;
            iconListView.Images.Clear();
            cardListView.Items.Clear();
            iconListView.Images.Add(Resources.linked);
            iconListView.Images.Add(Resources.linked_disabled);

            for (var i = 0; i < 15; i++)
            {
                iconListView.Images.Add(PrepareIcons(listIndex, i));

                switch (_pScard[listIndex].SaveType[i])
                {
                default:
                    cardListView.Items.Add("Corrupted slot");
                    break;

                case 0:
                    cardListView.Items.Add("Free slot");
                    break;

                case 1:
                case 4:
                    cardListView.Items.Add(_pScard[listIndex].SaveName[i, _settings.TitleEncoding]);
                    cardListView.Items[i].SubItems.Add(_pScard[listIndex].SaveProdCode[i]);
                    cardListView.Items[i].SubItems.Add(_pScard[listIndex].SaveIdentifier[i]);
                    cardListView.Items[i].ImageIndex = i + 2;      //Skip two linked slot icons
                    break;

                case 2:         //Middle link
                    cardListView.Items.Add("Linked slot (middle link)");
                    cardListView.Items[i].ImageIndex = 0;
                    break;

                case 5:         //Middle link deleted
                    cardListView.Items.Add("Linked slot (middle link)");
                    cardListView.Items[i].ImageIndex = 1;
                    break;

                case 3:         //End link
                    cardListView.Items.Add("Linked slot (end link)");
                    cardListView.Items[i].ImageIndex = 0;
                    break;

                case 6:         //End link deleted
                    cardListView.Items.Add("Linked slot (end link)");
                    cardListView.Items[i].ImageIndex = 1;
                    break;

                }
            }

            cardListView.Items[slotNumber].Selected = true;

            if (_settings.ListFont != null)
            {
                var tempFontFamily = new FontFamily(_settings.ListFont);

                if (tempFontFamily.IsStyleAvailable(FontStyle.Regular))
                {
                    cardListView.Font = new Font(_settings.ListFont, 8.25f);
                }
                else
                {
                    _settings.ListFont = FontFamily.GenericSansSerif.Name;
                    cardListView.Font = new Font(_settings.ListFont, 8.25f);
                }
            }

            cardListView.GridLines = _settings.ShowListGrid != 0;
            RefreshPluginBindings();
            EnableSelectiveEditItems();
        }

        private Bitmap PrepareIcons(int listIndex, int slotNumber)
        {
            var iconBitmap = new Bitmap(48, 16);
            var iconGraphics = Graphics.FromImage(iconBitmap);

            switch (_settings.IconBackgroundColor)
            {
            case 1:
                iconGraphics.FillRegion(new SolidBrush(Color.Black), new Region(new Rectangle(0, 0, 16, 16)));
                break;
            case 2:
                iconGraphics.FillRegion(new SolidBrush(Color.FromArgb(0xFF, 0x30, 0x30, 0x30)), new Region(new Rectangle(0, 0, 16, 16)));
                break;
            case 3:
                iconGraphics.FillRegion(new SolidBrush(Color.FromArgb(0xFF, 0x44, 0x44, 0x98)), new Region(new Rectangle(0, 0, 16, 16)));
                break;
            }

            var memoryCard = _pScard[listIndex];

            iconGraphics.DrawImage(memoryCard.IconData[slotNumber, 0], 0, 0, 16, 16);

            switch (memoryCard.SaveRegion[slotNumber])
            {
            default:
                iconGraphics.DrawImage(Resources.naflag, 17, 0, 30, 16);
                break;

            case 0x4142:
                iconGraphics.DrawImage(Resources.amflag, 17, 0, 30, 16);
                break;

            case 0x4542:
                iconGraphics.DrawImage(Resources.euflag, 17, 0, 30, 16);
                break;

            case 0x4942:
                iconGraphics.DrawImage(Resources.jpflag, 17, 0, 30, 16);
                break;
            }

            if (memoryCard.SaveType[slotNumber] == 4)
            {
                iconGraphics.FillRegion(new SolidBrush(Color.FromArgb(0xA0, 0xFF, 0xFF, 0xFF)), new Region(new Rectangle(0, 0, 16, 16)));
            }

            iconGraphics.Dispose();

            return iconBitmap;
        }

        private void RefreshStatusStrip()
        {
            if (_pScard.Count > 0)
            {
                toolString.Text = _pScard[mainTabControl.SelectedIndex].CardLocation;
            }
            else
            {
                toolString.Text = null;
            }
        }

        private void ExitApplication()
        {
            CloseAllCards();
            SaveProgramSettings();
        }

        private void EditSaveHeader()
        {
            if (_pScard.Count <= 0)
            {
                return;
            }

            var listIndex = mainTabControl.SelectedIndex;
            var cardListView = _cardList[listIndex];

            if (cardListView.SelectedIndices.Count == 0)
            {
                return;
            }

            var memoryCard = _pScard[listIndex];
            var slotNumber = cardListView.SelectedIndices[0];

            if (memoryCard.SaveType[slotNumber] != 1 && memoryCard.SaveType[slotNumber] != 4)
            {
                return;
            }

            var saveRegion = memoryCard.SaveRegion[slotNumber];
            var saveProdCode = memoryCard.SaveProdCode[slotNumber];
            var saveIdentifier = memoryCard.SaveIdentifier[slotNumber];
            var saveTitle = memoryCard.SaveName[slotNumber, _settings.TitleEncoding];
            var headerWindow = new HeaderWindow(ApplicationName, saveTitle, saveProdCode, saveIdentifier, saveRegion);

            headerWindow.ShowDialog(this);

            if (headerWindow.OkPressed)
            {
                memoryCard.SetHeaderData(slotNumber, headerWindow.ProductCode, headerWindow.SaveIdentifier, headerWindow.SaveRegion);
                RefreshListView(listIndex, slotNumber);
            }

            headerWindow.Dispose();
        }

        private bool LoadCommandLine()
        {
            if (Environment.GetCommandLineArgs().Length > 1)
            {
                OpenCard(Environment.GetCommandLineArgs()[1]);
            }
            else
            {
                return false;
            }

            return true;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Text = ApplicationName + " " + ApplicationVersion;
            _settings.TitleEncoding = 0;
            _settings.ListFont = FontFamily.GenericSansSerif.Name;
            _settings.ShowListGrid = 0;
            _settings.IconInterpolationMode = 0;
            _settings.IconPropertiesSize = 1;
            _settings.BackupMemcards = 0;
            _settings.WarningMessage = 1;
            _settings.RestoreWindowPosition = 0;
            _settings.CommunicationPort = "COM1";
            _settings.FormatType = 0;
            LoadProgramSettings();
            PluginSystem.FetchPlugins(_appPath + "/Plugins");

            if (LoadCommandLine() == false)
            {
                OpenCard(null);
            }
        }

        private void ManagePluginsToolStripMenuItem_Click(object sender, EventArgs arguments)
        {
            ShowPluginsWindow();
        }

        private void RefreshPluginBindings()
        {
            editWithPluginToolStripMenuItem.DropDownItems.Clear();
            editWithPluginToolStripMenuItem.Enabled = false;
            editWithPluginToolStripMenuItem1.DropDownItems.Clear();
            editWithPluginToolStripMenuItem1.Enabled = false;

            if (_cardList.Count <= 0)
            {
                return;
            }

            var listIndex = mainTabControl.SelectedIndex;
            var cardListView = _cardList[listIndex];

            if (cardListView.SelectedItems.Count <= 0)
            {
                return;
            }

            var slotNumber = cardListView.SelectedIndices[0];
            var memoryCard = _pScard[listIndex];

            if (memoryCard.SaveType[slotNumber] != 1 && memoryCard.SaveType[slotNumber] != 4)
            {
                return;
            }

            _supportedPlugins = PluginSystem.GetSupportedPlugins(memoryCard.SaveProdCode[slotNumber]);

            if (_supportedPlugins != null)
            {
                editWithPluginToolStripMenuItem.Enabled = true;
                editWithPluginToolStripMenuItem1.Enabled = true;

                foreach (var currentAssembly in _supportedPlugins)
                {
                    editWithPluginToolStripMenuItem.DropDownItems.Add(PluginSystem
                        .AssembliesMetadata[currentAssembly].Name);
                    editWithPluginToolStripMenuItem1.DropDownItems.Add(PluginSystem
                        .AssembliesMetadata[currentAssembly].Name);
                }
            }
        }

        private void EditWithPlugin(int pluginIndex)
        {
            if (_pScard.Count <= 0)
            {
                return;
            }

            if (_settings.WarningMessage == 1 && Messages.Prompt(ApplicationName, "Save editing may potentialy corrupt the save.\nDo you want to proceed with this operation?") == DialogResult.No)
            {
                return;
            }

            var listIndex = mainTabControl.SelectedIndex;
            var slotNumber = _cardList[listIndex].SelectedIndices[0];
            var editedSaveBytes = PluginSystem.EditSaveData(_supportedPlugins[pluginIndex], _pScard[listIndex].GetSaveBytes(slotNumber), _pScard[listIndex].SaveProdCode[slotNumber]);

            if (editedSaveBytes == null)
            {
                return;
            }

            var memoryCard = _pScard[listIndex];

            memoryCard.FormatSave(slotNumber);
            memoryCard.SetSaveBytes(slotNumber, editedSaveBytes, out _);
            RefreshListView(listIndex, slotNumber);
            memoryCard.WasChanged = true;
        }

        private void DisableEditItems()
        {
            editSaveHeaderToolStripMenuItem.Enabled = false;
            editSaveCommentToolStripMenuItem.Enabled = false;
            compareWithTempBufferToolStripMenuItem.Enabled = false;
            editIconToolStripMenuItem.Enabled = false;
            deleteSaveToolStripMenuItem.Enabled = false;
            restoreSaveToolStripMenuItem.Enabled = false;
            removeSaveformatSlotsToolStripMenuItem.Enabled = false;
            copySaveToTempraryBufferToolStripMenuItem.Enabled = false;
            pasteSaveFromTemporaryBufferToolStripMenuItem.Enabled = false;
            importSaveToolStripMenuItem.Enabled = false;
            exportSaveToolStripMenuItem.Enabled = false;

            //Edit toolbar
            editHeaderButton.Enabled = false;
            commentsButton.Enabled = false;
            editIconButton.Enabled = false;
            importButton.Enabled = false;
            exportButton.Enabled = false;

            //Edit popup
            editSaveHeaderToolStripMenuItem1.Enabled = false;
            editSaveCommentsToolStripMenuItem.Enabled = false;
            compareWithTempBufferToolStripMenuItem1.Enabled = false;
            editIconToolStripMenuItem1.Enabled = false;
            deleteSaveToolStripMenuItem1.Enabled = false;
            restoreSaveToolStripMenuItem1.Enabled = false;
            removeSaveformatSlotsToolStripMenuItem1.Enabled = false;
            copySaveToTempBufferToolStripMenuItem.Enabled = false;
            paseToolStripMenuItem.Enabled = false;
            importSaveToolStripMenuItem1.Enabled = false;
            exportSaveToolStripMenuItem1.Enabled = false;
            saveInformationToolStripMenuItem.Enabled = false;
        }

        //Enable all items related to save editing
        private void EnableEditItems()
        {
            //Edit menu
            editSaveHeaderToolStripMenuItem.Enabled = true;
            editSaveCommentToolStripMenuItem.Enabled = true;
            editIconToolStripMenuItem.Enabled = true;
            deleteSaveToolStripMenuItem.Enabled = true;
            restoreSaveToolStripMenuItem.Enabled = true;
            removeSaveformatSlotsToolStripMenuItem.Enabled = true;
            copySaveToTempraryBufferToolStripMenuItem.Enabled = true;
            pasteSaveFromTemporaryBufferToolStripMenuItem.Enabled = true;
            importSaveToolStripMenuItem.Enabled = true;
            exportSaveToolStripMenuItem.Enabled = true;
            editHeaderButton.Enabled = true;
            commentsButton.Enabled = true;
            editIconButton.Enabled = true;
            importButton.Enabled = true;
            exportButton.Enabled = true;
            editSaveHeaderToolStripMenuItem1.Enabled = true;
            editSaveCommentsToolStripMenuItem.Enabled = true;
            editIconToolStripMenuItem1.Enabled = true;
            deleteSaveToolStripMenuItem1.Enabled = true;
            restoreSaveToolStripMenuItem1.Enabled = true;
            removeSaveformatSlotsToolStripMenuItem1.Enabled = true;
            copySaveToTempBufferToolStripMenuItem.Enabled = true;
            paseToolStripMenuItem.Enabled = true;
            importSaveToolStripMenuItem1.Enabled = true;
            exportSaveToolStripMenuItem1.Enabled = true;
            saveInformationToolStripMenuItem.Enabled = true;

            if (_tempBuffer != null)
            {
                compareWithTempBufferToolStripMenuItem.Enabled = true;
                compareWithTempBufferToolStripMenuItem1.Enabled = true;
            }
            else
            {
                compareWithTempBufferToolStripMenuItem.Enabled = false;
                compareWithTempBufferToolStripMenuItem1.Enabled = false;
            }
        }

        private void EnableSelectiveEditItems()
        {
            if (_cardList.Count <= 0)
            {
                DisableEditItems();
            }
            else
            {
                var listIndex = mainTabControl.SelectedIndex;

                if (_cardList[listIndex].SelectedItems.Count <= 0)
                {
                    DisableEditItems();
                }
                else
                {
                    var slotNumber = _cardList[listIndex].SelectedIndices[0];

                    switch (_pScard[listIndex].SaveType[slotNumber])
                    {
                    case 0: //Formatted
                        DisableEditItems();
                        pasteSaveFromTemporaryBufferToolStripMenuItem.Enabled = true;
                        paseToolStripMenuItem.Enabled = true;
                        importSaveToolStripMenuItem.Enabled = true;
                        importSaveToolStripMenuItem1.Enabled = true;
                        importButton.Enabled = true;
                        break;

                    case 1: //Initial
                        EnableEditItems();
                        restoreSaveToolStripMenuItem.Enabled = false;
                        restoreSaveToolStripMenuItem1.Enabled = false;
                        pasteSaveFromTemporaryBufferToolStripMenuItem.Enabled = false;
                        paseToolStripMenuItem.Enabled = false;
                        importSaveToolStripMenuItem.Enabled = false;
                        importSaveToolStripMenuItem1.Enabled = false;
                        importButton.Enabled = false;
                        break;

                    case 2: //Middle link
                    case 3: //End link
                        DisableEditItems();
                        break;

                    case 4: //Deleted initial
                        EnableEditItems();
                        deleteSaveToolStripMenuItem.Enabled = false;
                        deleteSaveToolStripMenuItem1.Enabled = false;
                        pasteSaveFromTemporaryBufferToolStripMenuItem.Enabled = false;
                        paseToolStripMenuItem.Enabled = false;
                        importSaveToolStripMenuItem.Enabled = false;
                        importSaveToolStripMenuItem1.Enabled = false;
                        importButton.Enabled = false;
                        exportSaveToolStripMenuItem.Enabled = false;
                        exportSaveToolStripMenuItem1.Enabled = false;
                        exportButton.Enabled = false;
                        break;

                    case 5: //Deleted middle link
                    case 6: //Deleted end link
                        DisableEditItems();
                        break;

                    case 7: //Corrupted
                        DisableEditItems();
                        removeSaveformatSlotsToolStripMenuItem.Enabled = true;
                        removeSaveformatSlotsToolStripMenuItem1.Enabled = true;
                        break;
                    }
                }
            }
        }

        private void CompareSaveWithTemp()
        {
            if (_tempBuffer == null)
            {
                Messages.Warning(ApplicationName, TemporaryBufferIsEmpty);
                return;
            }

            if (_pScard.Count <= 0)
            {
                return;
            }

            var listIndex = mainTabControl.SelectedIndex;

            if (_cardList[listIndex].SelectedIndices.Count == 0) return;

            var slotNumber = _cardList[listIndex].SelectedIndices[0];

            //Check the save type
            if (_pScard[listIndex].SaveType[slotNumber] == 1 || _pScard[listIndex].SaveType[slotNumber] == 4)
            {
                var fetchedData = _pScard[listIndex].GetSaveBytes(slotNumber);
                var fetchedDataTitle = _pScard[listIndex].SaveName[slotNumber, _settings.TitleEncoding];

                if (fetchedData.Length != _tempBuffer.Length)
                {
                    Messages.Warning(ApplicationName, "Save file size mismatch. Saves can't be compared.");
                    return;
                }

                ShowAndDispose(new CompareWindow(this, ApplicationName, fetchedData, fetchedDataTitle, _tempBuffer, _tempBufferName + " (temp buffer)"));
            }
            else
            {
                Messages.Warning(ApplicationName, SelectedSlotLinked);
            }
        }

        private void CardReaderRead(byte[] readData)
        {
            if (readData == null)
            {
                return;
            }

            _pScard.Add(new MemoryCard());

            var memoryCard = _pScard[_pScard.Count - 1];

            memoryCard.OpenMemoryCardStream(readData);
            memoryCard.CardLocation = "\0";
            CreateTabPage();
            memoryCard.CardLocation = null;
        }

        private void mainTabControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            RefreshStatusStrip();
            RefreshPluginBindings();
            EnableSelectiveEditItems();
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenCardDialog();
        }

        private void ExitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenCard(null);
        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CloseCard(mainTabControl.SelectedIndex);
        }

        private void closeAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CloseAllCards();
        }

        private void editSaveHeaderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            EditSaveHeader();
        }

        private void editSaveHeaderToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            EditSaveHeader();
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveCardDialog(mainTabControl.SelectedIndex);
        }

        private void newButton_Click(object sender, EventArgs e)
        {
            OpenCard(null);
        }

        private void openButton_Click(object sender, EventArgs e)
        {
            OpenCardDialog();
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            SaveCardFunction(mainTabControl.SelectedIndex);
        }

        private void editSaveCommentToolStripMenuItem_Click(object sender, EventArgs e)
        {
            EditSaveComments();
        }

        private void editSaveCommentsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            EditSaveComments();
        }

        private void commentsButton_Click(object sender, EventArgs e)
        {
            EditSaveComments();
        }

        private void cardList_DoubleClick(object sender, EventArgs e)
        {
            ShowInformation();
        }

        private void cardList_IndexChanged(object sender, EventArgs e)
        {
            RefreshPluginBindings();
            EnableSelectiveEditItems();
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveCardFunction(mainTabControl.SelectedIndex);
        }

        private void preferencesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowAndDispose(new PreferencesWindow(this, _settings));
        }

        private void mainWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            ExitApplication();
        }

        private void deleteSaveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DeleteSave();
        }

        private void deleteSaveToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            DeleteSave();
        }

        private void restoreSaveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RestoreSave();
        }

        private void restoreSaveToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            RestoreSave();
        }

        private void editHeaderButton_Click(object sender, EventArgs e)
        {
            EditSaveHeader();
        }

        private void editIconButton_Click(object sender, EventArgs e)
        {
            EditIcon();
        }

        private void removeSaveformatSlotsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormatSave();
        }

        private void removeSaveformatSlotsToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            FormatSave();
        }

        private void editIconToolStripMenuItem_Click(object sender, EventArgs e)
        {
            EditIcon();
        }

        private void editIconToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            EditIcon();
        }

        private void copySaveToTempraryBufferToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CopySave();
        }

        private void copySaveToTempBufferToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CopySave();
        }

        private void exportSaveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ExportSaveDialog();
        }

        private void exportSaveToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            ExportSaveDialog();
        }

        private void exportButton_Click(object sender, EventArgs e)
        {
            ExportSaveDialog();
        }

        private void importSaveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ImportSaveDialog();
        }

        private void importSaveToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            ImportSaveDialog();
        }

        private void importButton_Click(object sender, EventArgs e)
        {
            ImportSaveDialog();
        }

        private void pasteSaveFromTemporaryBufferToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PasteSave();
        }

        private void paseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PasteSave();
        }

        private void tBufToolButton_Click(object sender, EventArgs e)
        {
            PasteSave();
        }

        private void saveInformationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowInformation();
        }

        private void mainTabControl_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.All;
        }

        private void editWithPluginToolStripMenuItem_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            _clickedPlugin[1] = e.ClickedItem.Owner.Items.IndexOf(e.ClickedItem);
            _clickedPlugin[0] = 1;
        }

        private void editWithPluginToolStripMenuItem_DropDownClosed(object sender, EventArgs e)
        {
            if (_clickedPlugin[0] == 1) EditWithPlugin(_clickedPlugin[1]);

            _clickedPlugin[0] = 0;
        }

        private void mainTabControl_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Middle)
            {
                return;
            }

            for (var cardIndex = 0; cardIndex < mainTabControl.TabCount; cardIndex++)
            {
                if (mainTabControl.GetTabRect(cardIndex).Contains(e.X, e.Y))
                {
                    CloseCard(cardIndex, false);
                }
            }
        }

        private void mainTabControl_DragDrop(object sender, DragEventArgs e)
        {
            foreach (var fileName in (string[])e.Data.GetData(DataFormats.FileDrop))
            {
                OpenCard(fileName);
            }
        }

        private void dexDriveMenuRead_Click(object sender, EventArgs e)
        {
            CardReaderRead(new CardReaderWindow().ReadMemoryCardDexDrive(this, ApplicationName, _settings.CommunicationPort));
        }

        private void dexDriveMenuWrite_Click(object sender, EventArgs e)
        {
            if (_pScard.Count > 0)
            {
                new CardReaderWindow().WriteMemoryCardDexDrive(this, ApplicationName, _settings.CommunicationPort, _pScard[mainTabControl.SelectedIndex].SaveMemoryCardStream(), 1024);
            }
        }

        private void compareWithTempBufferToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CompareSaveWithTemp();
        }

        private void compareWithTempBufferToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            CompareSaveWithTemp();
        }

        private void memCARDuinoMenuRead_Click(object sender, EventArgs e)
        {
            CardReaderRead(new CardReaderWindow().ReadMemoryCardCarDuino(this, ApplicationName, _settings.CommunicationPort));
        }

        private void memCARDuinoMenuWrite_Click(object sender, EventArgs e)
        {
            var listIndex = mainTabControl.SelectedIndex;

            if (_pScard.Count > 0)
            {
                new CardReaderWindow().WriteMemoryCardCarDuino(this, ApplicationName, _settings.CommunicationPort, _pScard[listIndex].SaveMemoryCardStream(), 1024);
            }
        }

        private void pS1CardLinkMenuRead_Click(object sender, EventArgs e)
        {
            CardReaderRead(new CardReaderWindow().ReadMemoryCardPs1CLnk(this, ApplicationName, _settings.CommunicationPort));
        }

        private void pS1CardLinkMenuWrite_Click(object sender, EventArgs e)
        {
            if (_pScard.Count > 0)
            {
                new CardReaderWindow().WriteMemoryCardPs1CLnk(this, ApplicationName, _settings.CommunicationPort, _pScard[mainTabControl.SelectedIndex].SaveMemoryCardStream(), 1024);
            }
        }

        private void dexDriveMenuFormat_Click(object sender, EventArgs e)
        {
            FormatHardwareCard(0);
        }

        private void memCARDuinoMenuFormat_Click(object sender, EventArgs arguments)
        {
            FormatHardwareCard(1);
        }

        private void pS1CardLinkMenuFormat_Click(object sender, EventArgs arguments)
        {
            FormatHardwareCard(2);
        }

        private void FormatHardwareCard(int hardDevice)
        {
            var dialogResult = Messages.Prompt(ApplicationName, "Formatting will delete all saves on the Memory Card.\nDo you want to proceed with this operation?");

            if (dialogResult == DialogResult.No)
            {
                return;
            }

            var frameNumber = 1024;
            var blankCard = new MemoryCard();

            if (_settings.FormatType == 0)
            {
                frameNumber = 64;
            }

            blankCard.OpenMemoryCard(null);

            switch (hardDevice)
            {
            case 0:
                new CardReaderWindow().WriteMemoryCardDexDrive(this, ApplicationName, _settings.CommunicationPort, blankCard.SaveMemoryCardStream(), frameNumber);
                break;
            case 1:
                new CardReaderWindow().WriteMemoryCardCarDuino(this, ApplicationName, _settings.CommunicationPort, blankCard.SaveMemoryCardStream(), frameNumber);
                break;
            case 2:
                new CardReaderWindow().WriteMemoryCardPs1CLnk(this, ApplicationName, _settings.CommunicationPort, blankCard.SaveMemoryCardStream(), frameNumber);
                break;
            }
        }
    }
}