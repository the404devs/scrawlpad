using Microsoft.Win32;
using System;
using System.Deployment.Application;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Drawing;
using Associations;

namespace TxtEdit
{
    public partial class Editor2 : Form
    {
        /*[DllImport("shell32.dll")]

        public static void SHChangeNotify(int wEventId, int uFlags, int dwItem1, int dwItem2)
        {
        }*/
        bool quitButton = false;
        string[] config = new string[10];
        public static string CurrentFile = "Untitled";
        public static int FontSize;
        public static string FontStyle = "";
        public static int Theme;
        string WindowCurrentFile = "Untitled";
        string ProgramName = " - ScrawlPad V4.5";
        Boolean unsaved = false;
        //Assembly assembly = Assembly.GetExecutingAssembly();
        Version version = Assembly.GetEntryAssembly().GetName().Version;

        //Version version = ApplicationDeployment.CurrentDeployment.CurrentVersion;
        int cursorLine = 0;//this is the current cursor position
        DialogResult dr;

        public Editor2()
        {
            InitializeComponent();
            this.AllowDrop = true;
            this.DragEnter += new DragEventHandler(Editor_DragEnter);
            this.DragDrop += new DragEventHandler(Editor_DragDrop);
            this.rtbText.AllowDrop = true;
            this.rtbText.DragDrop += new System.Windows.Forms.DragEventHandler(this.Editor_DragDrop);
            this.rtbText.DragEnter += new System.Windows.Forms.DragEventHandler(this.Editor_DragEnter);
        }

        void Editor_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop)) e.Effect = DragDropEffects.Copy;
        }

        void Editor_DragDrop(object sender, DragEventArgs e)
        {
            if (unsaved == true)
            {
                if (CheckSave() == false)
                {
                    string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                    foreach (string file in files) WindowCurrentFile = file;
                    loadFile();
                }
            }
            else
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                foreach (string file in files) WindowCurrentFile = file;
                loadFile(); 
            }
        }

        void setTitle()
        {
            if (unsaved == true)
            {
                this.Text = WindowCurrentFile + ProgramName + "**";
            }
            else
            {
                this.Text = WindowCurrentFile + ProgramName;
            }
        }

        Boolean CheckSave()
        {
            Boolean NoClose = false;
            if (unsaved == true)
            {
                dr = MessageBox.Show("Would you like to save your work?\n" + WindowCurrentFile + " has unsaved changes.", "Quit", MessageBoxButtons.YesNoCancel);//Show a mesage to warn the user of the severity of their actions.
                if (dr == DialogResult.Yes)
                {
                    if (WindowCurrentFile == "Untitled")
                    {
                        savePrompt();
                    }
                    else
                    {
                        saveFile();
                    }
                    this.Close();
                }
                else if (dr == DialogResult.No)
                {
                    
                }
                else
                {
                    NoClose = true;
                }                
            }
            else
            {
                if (quitButton == true)
                {

                    Application.Exit();
                }
            }
            return NoClose;
        }

        void saveFile()
        {
            StreamWriter file = new StreamWriter(WindowCurrentFile);
            for (int x = 0; x < rtbText.Lines.Length; x++)
            {
                file.WriteLine(rtbText.Lines[x]);
            }
            file.Close();
            unsaved = false;
            setTitle();
        }

        void loadFile()
        {
            string content = "";
            string line;
            StreamReader file = new StreamReader(WindowCurrentFile);
            while ((line = file.ReadLine()) != null)
            {
                content += line + "\n";

            }
            //content = file.ReadToEnd();
            file.Close();
            rtbText.Text = content;
            unsaved = false;
            setTitle();
        }

        void configExport()
        {
            config[0] = Theme + "";
                StreamWriter file = new StreamWriter("config.inf");
                for (int x = 0; x < config.Length; x++)
                {
                    file.WriteLine(config[x]);
                }
                file.Close();
            
        }

        void configImport()
        {
            int x = 0;
            string line;
            if (File.Exists("config.inf") == true)
            {
                StreamReader file = new StreamReader("config.inf");
                while ((line = file.ReadLine()) != null)
                {
                    config[x] += line;
                    x++;

                }
                //content = file.ReadToEnd();
                file.Close();
                Theme = Convert.ToInt32(config[0]);
                setTheme();
            }
        }

        void setTheme()
        {
            if (Theme == 0)//standard
            {
                menuStrip1.BackColor = Color.Black;
                menuStrip1.ForeColor = Color.White;
                tbLineJump.BackColor = Color.Turquoise;
                tbLineJump.ForeColor = Color.Black;
                label1.BackColor = Color.Black;
                label1.ForeColor = Color.White;
                rtbText.BackColor = Color.Silver;
                rtbText.ForeColor = Color.MediumBlue;
            }
            else if (Theme == 1)//development
            {
                menuStrip1.BackColor = Color.DarkBlue;
                menuStrip1.ForeColor = Color.Silver;
                tbLineJump.BackColor = Color.Black;
                tbLineJump.ForeColor = Color.White;
                label1.BackColor = Color.DarkBlue;
                label1.ForeColor = Color.White;
                rtbText.BackColor = Color.LightGreen;
                rtbText.ForeColor = Color.Red;
            }
        }

        void savePrompt()
        {
            Stream myStream;
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();

            saveFileDialog1.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
            saveFileDialog1.FilterIndex = 2;
            saveFileDialog1.RestoreDirectory = true;

            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                if ((myStream = saveFileDialog1.OpenFile()) != null)
                {
                    // Code to write the stream goes here.
                    WindowCurrentFile = saveFileDialog1.FileName;
                    myStream.Close();
                    saveFile();
                }
            }
        }

        void filePrompt()
        {
            Stream myStream = null;
            OpenFileDialog openFileDialog1 = new OpenFileDialog();

            //openFileDialog1.InitialDirectory = "c:\\";
            openFileDialog1.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
            openFileDialog1.FilterIndex = 2;
            openFileDialog1.RestoreDirectory = true;

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    if ((myStream = openFileDialog1.OpenFile()) != null)
                    {
                        using (myStream)
                        {
                            // Insert code to read the stream here.
                            WindowCurrentFile = openFileDialog1.FileName;
                            myStream.Close();
                            loadFile();
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: Could not read file from disk. Original error: " + ex.Message);
                }
            }
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (WindowCurrentFile == "Untitled")
            {
                savePrompt();
            }
            else
            {
                saveFile();
            }
        }

        private void openFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            filePrompt();
        }

        private void MenuSaveAs_Click(object sender, EventArgs e)
        {
            savePrompt();
        }

        private void rtbText_KeyDown(object sender, KeyEventArgs e)
        {
            unsaved = true;
            setTitle();
        }

        private void newFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            /*Editor2 frm = new Editor2();
            frm.Show();*/
            Process.Start(Application.ExecutablePath);
        }

        private void quitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            quitButton = true;
            CheckSave();
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string vx = version.ToString();
            MessageBox.Show("ScrawlPad Version " + vx + "\n12/11/17\nA text editor by Owen Goodwin");
        }

        private void tbLineJump_KeyDown(object sender, KeyEventArgs e)
        {
            int index = 0;
            if (e.KeyCode == Keys.Enter)
            {
                rtbText.Focus();
                //check if valid number here
                cursorLine = (Convert.ToInt32(tbLineJump.Text)) - 1;
                //
                if (cursorLine >= rtbText.Lines.Length)
                {

                }
                else
                {
                    index = rtbText.GetFirstCharIndexFromLine(cursorLine);
                }
                rtbText.Select(index, 0);
            }           

        }

        private void rtbText_CursorChanged(object sender, EventArgs e)
        {

        }

        private void insertTimestampToolStripMenuItem_Click(object sender, EventArgs e)
        {
            rtbText.Text = rtbText.Text.Insert(rtbText.SelectionStart, DateTime.Now.ToString());
        }

        private void Editor2_Load(object sender, EventArgs e)
        {
            /*My.Computer.Registry.ClassesRoot.CreateSubKey(".txt").SetValue("", "Plain Text Document", Microsoft.Win32.RegistryValueKind.String);
            My.Computer.Registry.ClassesRoot.CreateSubKey("Galaxy Database\\shell\\open\\command").SetValue("", Application.ExecutablePath + " \"%l\" ", Microsoft.Win32.RegistryValueKind.String);
            const dynamic SHCNE_ASSOCCHANGED = 0x8000000;
            const dynamic SHCNF_IDLIST = 0;
            SHChangeNotify(SHCNE_ASSOCCHANGED, SHCNF_IDLIST, 0, 0);*/
            /*AF_FileAssociator assoc = new AF_FileAssociator(".txt");
            string icon = assoc.DefaultIcon.IconPath;
            MessageBox.Show(Directory.GetCurrentDirectory()); 
            ProgramIcon newDefIcon = new ProgramIcon(Directory.GetCurrentDirectory() + "txt.ico");
            assoc.DefaultIcon = newDefIcon;*/
            //string[] args =  Environment.GetCommandLineArgs();
            /*for (int x = 0; x<= args.Length; x++)
            {
                MessageBox.Show(args[x]);
            }*/

            //Path.GetDirectoryName(args[0]);
            //MessageBox.Show("hi"+args[0]);
            configImport();
            WindowCurrentFile = CurrentFile;
            if (WindowCurrentFile != "Untitled")
            {
                loadFile();
            }
        }

        private void Editor2_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = CheckSave();
            configExport();
            






        }

        private void tbLineJump_KeyPress(object sender, KeyPressEventArgs e)
        {
            //!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar)
            //this code prevents all characters except for numbers to be inputed
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }

        }

        private void timestampEndOfDoc(object sender, EventArgs e)
        {
            rtbText.Text = rtbText.Text.Insert(rtbText.TextLength, "\n" + DateTime.Now.ToString());
        }

        private void fontToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FontDialog fontDialog1 = new FontDialog();//FIGURE OUT FONT DIALOG ISSUE
            fontDialog1.AllowVectorFonts = true;//SERIOUSLY DO IT
            fontDialog1.ShowDialog();//FIX IT
            rtbText.Font = fontDialog1.Font;//DON'T JUST SIT THERE MAKING MORE THEMES DO IT  
        }

        private void basicToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Theme = 0;
            setTheme();
        }

        private void theme1ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Theme = 1;
            setTheme();
        }

        private void cutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(rtbText.SelectedText);
            rtbText.SelectedText = "";
        }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(rtbText.SelectedText);
        }

        private void pasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            rtbText.Text = rtbText.Text.Insert(rtbText.SelectionStart, Clipboard.GetText());
        }

        private void selectAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            rtbText.Select(0, rtbText.TextLength);
        }
    }

   /* public class FileAssociation
    {
        public string Extension { get; set; }
        public string ProgId { get; set; }
        public string FileTypeDescription { get; set; }
        public string ExecutableFilePath { get; set; }
    }

    public class FileAssociations
    {
        // needed so that Explorer windows get refreshed after the registry is updated
        [System.Runtime.InteropServices.DllImport("Shell32.dll")]
        private static extern int SHChangeNotify(int eventId, int flags, IntPtr item1, IntPtr item2);

        private const int SHCNE_ASSOCCHANGED = 0x8000000;
        private const int SHCNF_FLUSH = 0x1000;

        public static void EnsureAssociationsSet()
        {
            var filePath = Process.GetCurrentProcess().MainModule.FileName;
            EnsureAssociationsSet(
                new FileAssociation
                {
                    
                    Extension = ".ucs",
                    ProgId = "UCS_Editor_File",
                    FileTypeDescription = "UCS File",
                    ExecutableFilePath = filePath
                });
        }

        public static void EnsureAssociationsSet(params FileAssociation[] associations)
        {
            bool madeChanges = false;
            foreach (var association in associations)
            {
                madeChanges |= SetAssociation(
                    association.Extension,
                    association.ProgId,
                    association.FileTypeDescription,
                    association.ExecutableFilePath);
            }

            if (madeChanges)
            {
                SHChangeNotify(SHCNE_ASSOCCHANGED, SHCNF_FLUSH, IntPtr.Zero, IntPtr.Zero);
            }
        }

        public static bool SetAssociation(string extension, string progId, string fileTypeDescription, string applicationFilePath)
        {
            bool madeChanges = false;
            madeChanges |= SetKeyDefaultValue(@"Software\Classes\" + extension, progId);
            madeChanges |= SetKeyDefaultValue(@"Software\Classes\" + progId, fileTypeDescription);
            madeChanges |= SetKeyDefaultValue($@"Software\Classes\{progId}\shell\open\command", "\"" + applicationFilePath + "\" \"%1\"");
            return madeChanges;
        }

        private static bool SetKeyDefaultValue(string keyPath, string value)
        {
            using (var key = Registry.CurrentUser.CreateSubKey(keyPath))
            {
                if (key.GetValue(null) as string != value)
                {
                    key.SetValue(null, value);
                    return true;
                }
            }

            return false;
        }
    }*/
}
