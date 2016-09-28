using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Console = Colorful.Console;

namespace FileWatcherConsole
{
    internal class Program
    {
        [STAThread]
        private static void Main(string[] args)
        {
            AppDomain.CurrentDomain.ProcessExit += CurrentDomain_ProcessExit;

            FolderBrowserDialog folderDialog = new FolderBrowserDialog();

            if (folderDialog.ShowDialog() == DialogResult.OK)
            {
                Console.WriteLine(folderDialog.SelectedPath.ToString());
                string path = @folderDialog.SelectedPath.ToString();
                Program fileWatchClass = new Program();
                fileWatchClass.startWatcher(path);
            }
        }

        public void startWatcher(string path)
        {
            FileSystemWatcher watcher = new FileSystemWatcher(path, "*.*");

            Console.WriteLine("File watcher in {0} started" + Environment.NewLine, path);

            watcher.NotifyFilter = NotifyFilters.Attributes |
      NotifyFilters.CreationTime |
      NotifyFilters.FileName |
      NotifyFilters.LastAccess |
      NotifyFilters.LastWrite |
      NotifyFilters.Size |
      NotifyFilters.Security;

            watcher.Changed += new FileSystemEventHandler(OnChanged);
            watcher.Created += new FileSystemEventHandler(OnChanged);
            watcher.Deleted += new FileSystemEventHandler(OnChanged);
            watcher.Renamed += new RenamedEventHandler(OnRenamed);
            watcher.EnableRaisingEvents = true;
            Console.WriteLine("Press \'q\' to quit" + Environment.NewLine);
            while (Console.Read() != 'q') ;
        }

        private static void CurrentDomain_ProcessExit(object sender, EventArgs e)
        {
        }

        private void OnRenamed(object sender, RenamedEventArgs e)
        {
            string log = "{0} | Renamed from {1}";
            Colorful.Formatter[] files = new Colorful.Formatter[]
            {
                new Colorful.Formatter(e.FullPath,Color.Navy),
                new Colorful.Formatter(e.OldName,Color.Fuchsia)
            };
            Console.WriteLine(DateTime.Now.ToString("h:mm:ss tt") + Environment.NewLine, Color.Cyan);
            Console.WriteLineFormatted(log, Color.Gray, files);
            Console.WriteLine(Environment.NewLine);
        }

        private void OnChanged(object sender, FileSystemEventArgs e)
        {
            Console.WriteLine(DateTime.Now.ToString("h:mm:ss tt") + Environment.NewLine, Color.Cyan);

            string log = "File {0} | {1}";

            Colorful.Formatter[] files = new Colorful.Formatter[]
            {
                new Colorful.Formatter(e.FullPath,Color.Green),
                new Colorful.Formatter(e.ChangeType,getChangedTypeColor(e.ChangeType))
            };
            Console.WriteLineFormatted(log, Color.Gray, files);
            Console.WriteLine(Environment.NewLine);
        }

        private Color getChangedTypeColor(WatcherChangeTypes type)
        {
            switch (type)
            {
                case WatcherChangeTypes.Changed:
                    return Color.Yellow;

                case WatcherChangeTypes.Created:
                    return Color.Green;

                case WatcherChangeTypes.Deleted:
                    return Color.Red;

                case WatcherChangeTypes.Renamed:
                    return Color.LightCoral;

                default:
                    return Color.Aquamarine;
            }
        }
    }
}