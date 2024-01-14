using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Path = System.IO.Path;

namespace NaviEaze
{
    /// <summary>
    /// Interaction logic for MainPage.xaml
    /// </summary>
    public partial class MainPage : Page
    {
        int lineNumber = 0;
        bool outpuViewVisible;
        RowDefinition rowDef;

        // Get the TransformGroup from the Canvas
        TransformGroup transformGroup;
        // Get the ScaleTransform from the TransformGroup
        ScaleTransform scaleTransform;

        public static string? CurrentPath;
        string?[] folderNames;
        string?[] fileNames;
        double filesFontSize = 30;
        double folderFontSize = 32;

        public MainPage()
        {
            InitializeComponent();
            outpuViewVisible = true;
            rowDef = lowerGrid.RowDefinitions[1];

            transformGroup = (TransformGroup)myCanvas.RenderTransform;
            scaleTransform = (ScaleTransform)transformGroup.Children[0];

            CurrentPath = @"C:\test";
            //CurrentPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

            commandsInput.Text = "";

            currentPathLabel.Content = CurrentPath + "> ";
            folderNames = GetFolderNames(CurrentPath);
            fileNames = GetFileNames(CurrentPath);

            AppendExplorerButtons();

            //SendButton.AddHandler(MouseLeftButtonUpEvent, new RoutedEventHandler(CommandButton_Click), true);
        }
        public MainPage(int startButtonNr)
        {
            InitializeComponent();
            outpuViewVisible = true;
            rowDef = lowerGrid.RowDefinitions[1];

            transformGroup = (TransformGroup)myCanvas.RenderTransform;
            scaleTransform = (ScaleTransform)transformGroup.Children[0];

            if (startButtonNr == 0)
                //CurrentPath = @"C:\test";
                CurrentPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            else if (startButtonNr == 1)
                CurrentPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + @"\Desktop";
            else if (startButtonNr == 2)
                CurrentPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + @"\Documents";
            else
                CurrentPath = @"C:\test";

            commandsInput.Text = "";

            currentPathLabel.Content = CurrentPath + "> ";
            folderNames = GetFolderNames(CurrentPath);
            fileNames = GetFileNames(CurrentPath);

            AppendExplorerButtons();

            //SendButton.AddHandler(MouseLeftButtonUpEvent, new RoutedEventHandler(CommandButton_Click), true);
        }

        public static void SetPath(int startButtonNr)
        {
            Debug.WriteLine("BAM jag finns " + startButtonNr);
            if (startButtonNr == 0)
            {
                CurrentPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            }
        }

        private void PageSizeChanged(object sender, SizeChangedEventArgs e)
        {
            double width = e.NewSize.Width;
            double height = e.NewSize.Height;
            double area = width * height;
            double newFontSize;
            //Debug.WriteLine($"Width: {width} Height:  {height}");
            // Hann tyvärr inte få till detta så dem dynamiskt genererade knapparna updateras direkt
            // Men dem får iaf ny textstorlek då ny läses in vid katalogbyte
            Debug.WriteLine($"Area {area}");
            if (area < 500000)
            {
                newFontSize = 20;
                folderFontSize = 18;
                filesFontSize = 16;
            }
            else if (area < 750000)
            {
                newFontSize = 30;
                folderFontSize = 28;
                filesFontSize = 24;
            }
            else if (area < 800000)
            {
                newFontSize = 40;
                folderFontSize = 32;
                filesFontSize = 30;
            }
            else if (area < 1000000)
            {
                newFontSize = 46;
                folderFontSize = 40;
                filesFontSize = 38;
            }
            else
            {
                newFontSize = 50;
                folderFontSize = 44;
                filesFontSize = 40;
            }
            GotoText.FontSize = newFontSize;
            NewText.FontSize = newFontSize;
            ParentText.FontSize = newFontSize;
            RenameText.FontSize = newFontSize;
            ListText.FontSize = newFontSize;
            CutText.FontSize = newFontSize;
            CopyText.FontSize = newFontSize;

            PasteText.FontSize = newFontSize * 0.8;
            MoveText.FontSize = newFontSize * 0.8;

            HelpText.FontSize = newFontSize;
            fullHelpText.FontSize = newFontSize;
        }

        private void AppendExplorerButtons()
        {
            AppendControl();
            try
            {
                AppendFolderButtons(folderNames);
                AppendFileButtons(fileNames);
            }
            catch (Exception)
            {
                throw;
            }
        }

        private string?[] GetFolderNames(string @path)
        {
            folderNames = Directory.GetDirectories(path).Select(Path.GetFileName).ToArray();
            return folderNames;
            //fileNames = Directory.GetFiles(path).Select(Path.GetFileName).ToArray();
        }
        private string?[] GetFileNames(string @path)
        {
            //folderNames = Directory.GetDirectories(path).Select(Path.GetFileName).ToArray();
            fileNames = Directory.GetFiles(path).Select(Path.GetFileName).ToArray();
            return fileNames;
        }

        private void CommandButton_Click(object sender, RoutedEventArgs e)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            Process oProcess = new Process();
            lineNumber = 0;
            oProcess.StartInfo.FileName = "cmd.exe";
            oProcess.StartInfo.WorkingDirectory = CurrentPath;
            oProcess.StartInfo.StandardOutputEncoding = Encoding.GetEncoding(850);
            // oProcess.StartInfo.StandardOutputEncoding = Encoding.UTF8;
            oProcess.StartInfo.RedirectStandardInput = true;
            oProcess.StartInfo.RedirectStandardOutput = true;
            oProcess.StartInfo.RedirectStandardError = true;
            oProcess.StartInfo.CreateNoWindow = true;
            oProcess.StartInfo.UseShellExecute = false;
            string command = commandsInput.Text;
            string? line;

            oProcess.Start();
            // oProcess.StandardInput.WriteLine("chcp 1252");
            oProcess.StandardInput.WriteLine($"{command}");
            oProcess.StandardInput.Flush();
            oProcess.StandardInput.Close();
            oProcess.WaitForExit();

            CurrentPath = oProcess.StartInfo.WorkingDirectory;

            //output.Text += oProcess.StandardOutput.ReadToEnd() + "\n";
            lineNumber = 0;
            //output.Text = ""; //empty string
            string? prePath = CurrentPath;
            string? afterpath = null;
            while ((line = oProcess.StandardError.ReadLine()) != null)
            {
                //output.Text = $"E{lineNumber}: {line}\n";
                output.Text = $"Oups, something went wrong\nPlease make shure the command is correct\nTip: send help for a list of available commands.\n";
                lineNumber++;
            }
            if (lineNumber == 0) //there is no Error
            {
                lineNumber = 0;
                while ((line = oProcess.StandardOutput.ReadLine()) != null)
                {
                    if (oProcess.StandardOutput.EndOfStream)
                    {
                        afterpath = @line.Substring(0, line.Length - 1);
                        Debug.WriteLine(afterpath);
                    }
                    if (lineNumber > 2)
                    {
                        output.Text += $"{line}\n";
                    }
                    lineNumber++;
                }
                if (afterpath != null)
                {
                    CurrentPath = afterpath;
                }

            }

            cmdScrollViewer.ScrollToEnd();

            FilesAndFoldersGrid.Children.Clear();
            if (CurrentPath != null)
            {
                string?[] stashedFolderNames = folderNames;
                string?[] stashedFileNames = fileNames;
                try
                {
                    folderNames = GetFolderNames(CurrentPath);
                    fileNames = GetFileNames(CurrentPath);

                }
                catch (UnauthorizedAccessException)
                {
                    output.Text += "We unfortunately do not have permission for the selected directory.\nTry running the program as an administrator.\n";
                    cmdScrollViewer.ScrollToEnd();

                    folderNames = stashedFolderNames;
                    fileNames = stashedFileNames;
                    CurrentPath = prePath;
                }
                catch (Exception)
                {

                    throw;
                }
            }
            else
            {
                output.Text = "PATH IS NULL.";
            }
            currentPathLabel.Content = CurrentPath;
            AppendExplorerButtons();
            commandsInput.Text = "";
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog oOpenFileDialog = new OpenFileDialog();
            oOpenFileDialog.ShowDialog();
        }


        private void ShowHideOutputView(object sender, MouseButtonEventArgs e)
        {

            if (outpuViewVisible)
            {
                outputView.Visibility = Visibility.Collapsed;
                outpuViewVisible = false;
                scaleTransform.ScaleY = -1;
                foldableRow.Height = GridLength.Auto;
            }
            else
            {
                outputView.Visibility = Visibility.Visible;
                outpuViewVisible = true;
                scaleTransform.ScaleY = 1;
                foldableRow.Height = new GridLength(1, GridUnitType.Star);
            }
        }

        private void AppendControl()
        {
            // Create a new Grid
            Grid grid = new Grid();
            grid.Margin = new Thickness(10);

            // Create a new Rectangle
            Rectangle rect = new Rectangle();
            rect.VerticalAlignment = VerticalAlignment.Stretch;
            rect.HorizontalAlignment = HorizontalAlignment.Stretch;
            rect.RadiusX = 7;
            rect.RadiusY = 7;
            rect.Fill = new SolidColorBrush(Color.FromRgb(197, 197, 232));
            rect.MouseUp += new MouseButtonEventHandler(Folder_Controll_Clicked);

            // Create a new Button
            Button btn = new Button();
            btn.Click += new RoutedEventHandler(Folder_Controll_Clicked);
            btn.MouseDoubleClick += new MouseButtonEventHandler(Controll_Double_Clicked);


            // Bind the height of the button to its actual width
            Binding heightBinding = new Binding("ActualWidth");
            heightBinding.Source = btn;
            heightBinding.Mode = BindingMode.OneWay;
            btn.SetBinding(HeightProperty, heightBinding);

            // Create a new TextBlock
            TextBlock txt = new TextBlock();
            txt.Foreground = Brushes.Black;
            txt.TextAlignment = TextAlignment.Center;
            txt.VerticalAlignment = VerticalAlignment.Stretch;
            txt.FontFamily = new FontFamily("Agency FB");
            txt.FontWeight = FontWeights.Bold;
            txt.FontSize = 16;
            txt.TextWrapping = TextWrapping.Wrap;

            // Create a new Run
            Run run = new Run(".. /");
            run.FontSize = 32;

            // Add the Run to the TextBlock
            txt.Inlines.Add(run);

            // Add the TextBlock to the Button
            btn.Content = txt;

            // Add the Rectangle and Button to the Grid
            grid.Children.Add(rect);
            grid.Children.Add(btn);

            // Add the Grid to the UniformGrid
            FilesAndFoldersGrid.Children.Add(grid);
        }

        private void AppendFolderButtons(string?[] fNames)
        {
            foreach (string? fName in fNames)
            {
                // Create a new Grid
                Grid grid = new Grid();
                grid.Margin = new Thickness(10);

                // Create a new image Rectangle
                Rectangle rect = new Rectangle();
                rect.VerticalAlignment = VerticalAlignment.Stretch;
                rect.HorizontalAlignment = HorizontalAlignment.Stretch;
                rect.RadiusX = 7;
                rect.RadiusY = 7;

                // Set the background image of the rectangle
                ImageBrush brush = new ImageBrush();
                brush.ImageSource = new BitmapImage(new Uri("Assets/foldersquare.png", UriKind.Relative));
                rect.Fill = brush;

                rect.MouseUp += new MouseButtonEventHandler(File_Clicked);

                // Create a new Button
                Button btn = new Button();
                btn.Click += new RoutedEventHandler(Folder_Clicked);
                btn.MouseDoubleClick += new MouseButtonEventHandler(Folder_Double_Clicked);

                // Bind the height of the button to its actual width
                Binding heightBinding = new Binding("ActualWidth");
                heightBinding.Source = btn;
                heightBinding.Mode = BindingMode.OneWay;
                btn.SetBinding(HeightProperty, heightBinding);

                // Create a new TextBlock
                TextBlock txt = new TextBlock();
                txt.Foreground = Brushes.Black;
                txt.TextAlignment = TextAlignment.Center;
                txt.VerticalAlignment = VerticalAlignment.Stretch;
                txt.FontFamily = new FontFamily("Agency FB");
                txt.FontWeight = FontWeights.Bold;
                txt.FontSize = folderFontSize;
                txt.Text = fName;
                txt.TextWrapping = TextWrapping.Wrap;


                // Add the TextBlock to the Button
                btn.Content = txt;
                // Add the Rectangle and Button to the Grid
                grid.Children.Add(rect);
                grid.Children.Add(btn);
                // Add the Grid to the UniformGrid
                FilesAndFoldersGrid.Children.Add(grid);
            }
        }

        private void AppendFileButtons(string?[] fNames)
        {
            foreach (string? fName in fNames)
            {
                // Create a new Grid
                Grid grid = new Grid();
                grid.Margin = new Thickness(10);

                // Create a new image Rectangle
                Rectangle rect = new Rectangle();
                rect.VerticalAlignment = VerticalAlignment.Stretch;
                rect.HorizontalAlignment = HorizontalAlignment.Stretch;
                rect.RadiusX = 7;
                rect.RadiusY = 7;

                // Set the background image of the rectangle
                ImageBrush brush = new ImageBrush();
                brush.ImageSource = new BitmapImage(new Uri("Assets/filebg.png", UriKind.Relative));
                rect.Fill = brush;

                rect.MouseUp += new MouseButtonEventHandler(File_Clicked);

                // Create a new Button
                Button btn = new Button();
                btn.Click += new RoutedEventHandler(File_Clicked);
                btn.MouseDoubleClick += new MouseButtonEventHandler(File_Double_Clicked);


                // Bind the height of the button to its actual width
                Binding heightBinding = new Binding("ActualWidth");
                heightBinding.Source = btn;
                heightBinding.Mode = BindingMode.OneWay;
                btn.SetBinding(Button.HeightProperty, heightBinding);

                // Create a new TextBlock
                TextBlock txt = new TextBlock();
                txt.Foreground = Brushes.Black;
                txt.TextAlignment = TextAlignment.Center;
                txt.VerticalAlignment = VerticalAlignment.Stretch;
                txt.FontFamily = new FontFamily("Agency FB");
                txt.FontWeight = FontWeights.Bold;
                txt.FontSize = filesFontSize;
                txt.Text = fName;
                txt.TextWrapping = TextWrapping.Wrap;

                // Add the TextBlock to the Button
                btn.Content = txt;

                // Add the Rectangle and Button to the Grid
                grid.Children.Add(rect);
                grid.Children.Add(btn);

                // Add the Grid to the UniformGrid
                FilesAndFoldersGrid.Children.Add(grid);
            }
        }

        private void Button_Click_Goto(object sender, RoutedEventArgs e)
        {
            commandsInput.Text += "cd ";
        }

        private void Button_Click_New(object sender, RoutedEventArgs e)
        {
            commandsInput.Text += "md ";
        }

        private void Button_Click_Parent(object sender, RoutedEventArgs e)
        {
            commandsInput.Text += "cd.. ";
        }

        private void Button_Click_Rename(object sender, RoutedEventArgs e)
        {
            commandsInput.Text += "ren ";
        }

        private void Button_Click_Remove(object sender, RoutedEventArgs e)
        {
            commandsInput.Text += "ren ";
        }

        private void Button_Click_List(object sender, RoutedEventArgs e)
        {
            commandsInput.Text += "dir ";
        }

        private void Button_Click_Cut(object sender, RoutedEventArgs e)
        {
            commandsInput.Text += "move ";
        }

        private void Button_Click_Copy(object sender, RoutedEventArgs e)
        {
            commandsInput.Text += "copy ";
        }

        private void Folder_Controll_Clicked(object sender, RoutedEventArgs e)
        {
            commandsInput.Text += ".. ";
        }
        private void Controll_Double_Clicked(object sender, RoutedEventArgs e)
        {
            commandsInput.Text = "cd ..";
            CommandButton_Click(sender, e);
        }

        private void Button_Click_Help(object sender, RoutedEventArgs e)
        {
            // For some reason the plain help command hangs. not time to debug so i solved it with a copypaste.
            // Lucky for me the help [commad work] (atleast with dir wich is what i tried)
            output.Text += "For more information on a specific command, type HELP command-name\r\n" +
                "CD             Displays the name of or changes the current directory.\r\n" +
                "CHCP           Displays or sets the active code page number.\r\n" +
                "CHDIR          Displays the name of or changes the current directory.\r\n" +
                "CHKDSK         Checks a disk and displays a status report.\r\n" +
                "CLS            Clears the screen.\r\n" +
                "CMD            Starts a new instance of the Windows command interpreter.\r\n" +
                "COPY           Copies one or more files to another location.\r\n" +
                "DATE           Displays or sets the date.\r\n" +
                "DEL            Deletes one or more files.\r\n" +
                "DIR            Displays a list of files and subdirectories in a directory.\r\n" +
                "ERASE          Deletes one or more files.\r\n" +
                "HELP           Provides Help information for Windows commands.\r\n" +
                "MD             Creates a directory.\r\n" +
                "MKDIR          Creates a directory.\r\n" +
                "MOVE           Moves one or more files from one directory to another directory.\r\n" +
                "PRINT          Prints a text file.\r\n" +
                "RD             Removes a directory.\r\n" +
                "REN            Renames a file or files.\r\n" +
                "RENAME         Renames a file or files.\r\n" +
                "REPLACE        Replaces files.\r\n" +
                "RMDIR          Removes a directory.\r\n" +
                "SHUTDOWN       Allows proper local or remote shutdown of machine.\r\n" +
                "START          Starts a separate window to run a specified program or command.\r\n" +
                "SYSTEMINFO     Displays machine specific properties and configuration.\r\n" +
                "TIME           Displays or sets the system time.\r\n" +
                "TREE           Graphically displays the directory structure of a drive or path.\r\n" +
                "TYPE           Displays the contents of a text file.\r\n" +
                "VER            Displays the Windows version.\r\n" +
                "VOL            Displays a disk volume label and serial number.\r\n" +
                "XCOPY          Copies files and directory trees.\r\n" +
                "For more information on tools see the command-line reference in the online help.";
            cmdScrollViewer.ScrollToEnd();
        }
        private void Button_Click_Help_Verbose(object sender, RoutedEventArgs e)
        {
            output.Text += "For more information on a specific command, type HELP command-name" +
                "\r\nASSOC          Displays or modifies file extension associations.\r\n" +
                "ATTRIB         Displays or changes file attributes.\r\n" +
                "BREAK          Sets or clears extended CTRL+C checking.\r\n" +
                "BCDEDIT        Sets properties in boot database to control boot loading.\r\n" +
                "CACLS          Displays or modifies access control lists (ACLs) of files.\r\n" +
                "CALL           Calls one batch program from another.\r\n" +
                "CD             Displays the name of or changes the current directory.\r\n" +
                "CHCP           Displays or sets the active code page number.\r\n" +
                "CHDIR          Displays the name of or changes the current directory.\r\n" +
                "CHKDSK         Checks a disk and displays a status report.\r\n" +
                "CHKNTFS        Displays or modifies the checking of disk at boot time.\r\n" +
                "CLS            Clears the screen.\r\n" +
                "CMD            Starts a new instance of the Windows command interpreter.\r\n" +
                "COLOR          Sets the default console foreground and background colors.\r\n" +
                "COMP           Compares the contents of two files or sets of files.\r\n" +
                "COMPACT        Displays or alters the compression of files on NTFS partitions.\r\n" +
                "CONVERT        Converts FAT volumes to NTFS.  You cannot convert the current drive.\r\n" +
                "COPY           Copies one or more files to another location.\r\n" +
                "DATE           Displays or sets the date.\r\n" +
                "DEL            Deletes one or more files.\r\n" +
                "DIR            Displays a list of files and subdirectories in a directory.\r\n" +
                "DISKPART       Displays or configures Disk Partition properties.\r\n" +
                "DOSKEY         Edits command lines, recalls Windows commands, and\r\n" +
                "               creates macros.\r\n" +
                "DRIVERQUERY    Displays current device driver status and properties.\r\n" +
                "ECHO           Displays messages, or turns command echoing on or off.\r\n" +
                "ENDLOCAL       Ends localization of environment changes in a batch file.\r\n" +
                "ERASE          Deletes one or more files.\r\n" +
                "EXIT           Quits the CMD.EXE program (command interpreter).\r\n" +
                "FC             Compares two files or sets of files, and displays the\r\n" +
                "               differences between them.\r\n" +
                "FIND           Searches for a text string in a file or files.\r\n" +
                "FINDSTR        Searches for strings in files.\r\n" +
                "FOR            Runs a specified command for each file in a set of files.\r\n" +
                "FORMAT         Formats a disk for use with Windows.\r\n" +
                "FSUTIL         Displays or configures the file system properties.\r\n" +
                "FTYPE          Displays or modifies file types used in file extension\r\n" +
                "               associations.\r\n" +
                "GOTO           Directs the Windows command interpreter to a labeled line in\r\n" +
                "               a batch program.\r\n" +
                "GPRESULT       Displays Group Policy information for machine or user.\r\n" +
                "GRAFTABL       Enables Windows to display an extended character set in\r\n" +
                "               graphics mode.\r\n" +
                "HELP           Provides Help information for Windows commands.\r\n" +
                "ICACLS         Display, modify, backup, or restore ACLs for files and\r\n" +
                "               directories.\r\n" +
                "IF             Performs conditional processing in batch programs.\r\n" +
                "LABEL          Creates, changes, or deletes the volume label of a disk.\r\n" +
                "MD             Creates a directory.\r\n" +
                "MKDIR          Creates a directory.\r\n" +
                "MKLINK         Creates Symbolic Links and Hard Links\r\n" +
                "MODE           Configures a system device.\r\n" +
                "MORE           Displays output one screen at a time.\r\n" +
                "MOVE           Moves one or more files from one directory to another directory.\r\n" +
                "OPENFILES      Displays files opened by remote users for a file share.\r\n" +
                "PATH           Displays or sets a search path for executable files.\r\n" +
                "PAUSE          Suspends processing of a batch file and displays a message.\r\n" +
                "POPD           Restores the previous value of the current directory saved by PUSHD.\r\n" +
                "PRINT          Prints a text file.\r\n" +
                "PROMPT         Changes the Windows command prompt.\r\n" +
                "PUSHD          Saves the current directory then changes it.\r\n" +
                "RD             Removes a directory.\r\n" +
                "RECOVER        Recovers readable information from a bad or defective disk.\r\n" +
                "REM            Records comments (remarks) in batch files or CONFIG.SYS.\r\n" +
                "REN            Renames a file or files.\r\n" +
                "RENAME         Renames a file or files.\r\n" +
                "REPLACE        Replaces files.\r\n" +
                "RMDIR          Removes a directory.\r\n" +
                "ROBOCOPY       Advanced utility to copy files and directory trees\r\n" +
                "SET            Displays, sets, or removes Windows environment variables.\r\n" +
                "SETLOCAL       Begins localization of environment changes in a batch file.\r\n" +
                "SC             Displays or configures services (background processes).\r\n" +
                "SCHTASKS       Schedules commands and programs to run on a computer.\r\n" +
                "SHIFT          Shifts the position of replaceable parameters in batch files.\r\n" +
                "SHUTDOWN       Allows proper local or remote shutdown of machine.\r\n" +
                "SORT           Sorts input.\r\n" +
                "START          Starts a separate window to run a specified program or command.\r\n" +
                "SUBST          Associates a path with a drive letter.\r\n" +
                "SYSTEMINFO     Displays machine specific properties and configuration.\r\n" +
                "TASKLIST       Displays all currently running tasks including services.\r\n" +
                "TASKKILL       Kill or stop a running process or application.\r\n" +
                "TIME           Displays or sets the system time.\r\n" +
                "TITLE          Sets the window title for a CMD.EXE session.\r\n" +
                "TREE           Graphically displays the directory structure of a drive or path.\r\n" +
                "TYPE           Displays the contents of a text file.\r\n" +
                "VER            Displays the Windows version.\r\n" +
                "VERIFY         Tells Windows whether to verify that your files are written correctly to a disk.\r\n" +
                "VOL            Displays a disk volume label and serial number.\r\n" +
                "XCOPY          Copies files and directory trees.\r\n" +
                "WMIC           Displays WMI information inside interactive command shell.\r\n\r\n" +
                "For more information on tools see the command-line reference in the online help.";
            cmdScrollViewer.ScrollToEnd();
        }


        private void Folder_Clicked(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            TextBlock txt = btn.Content as TextBlock;
            Debug.WriteLine("SINGLE CLICK");
            Debug.WriteLine(txt.Text);
            string folderName = txt.Text;
            commandsInput.Text += folderName + " ";
        }

        // On Double Clicked Folder Enter Instantly
        private void Folder_Double_Clicked(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            TextBlock txt = btn.Content as TextBlock;
            Debug.WriteLine("DOUBLE CLICK");
            Debug.WriteLine(txt.Text);
            string folderName = txt.Text;
            commandsInput.Text = "cd " + folderName + " ";
            CommandButton_Click(sender, e);
        }

        private void File_Clicked(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            TextBlock txt = btn.Content as TextBlock;
            Debug.WriteLine(txt.Text);
            string fileName = txt.Text;
            commandsInput.Text += fileName + " ";

        }
        private void File_Double_Clicked(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            TextBlock txt = btn.Content as TextBlock;
            Debug.WriteLine(txt.Text);
            string fileName = txt.Text;
            // commandsInput.Text += fileName + " ";
            commandsInput.Text = fileName;
            CommandButton_Click(sender, e);

        }

        private void Button_Click_Paste(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("We regret to tell you we failed to implement this.\nDue to lack of time\nWill be implemented in the next update.", "Alert", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void MouseOverSendButton(object sender, MouseEventArgs e)
        {
            SendButton.Background = Brushes.Fuchsia;
        }

        private void MouseLeftSendButton(object sender, MouseEventArgs e)
        {
            SendButton.Background = Brushes.Transparent;
        }

        private void InputTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                CommandButton_Click(sender, e);
            }
        }

        private void textInputBoxClicked(object sender, MouseButtonEventArgs e)
        {
            commandsInput.Focus();
            commandsInput.CaretIndex = commandsInput.Text.Length;
        }

    }
}
