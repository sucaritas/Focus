namespace Focus
{
    using Newtonsoft.Json.Linq;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;

    public class Monitored : IDisposable
    {
        private string filePath { get; set; }
        private Container container { get; set; }
        private FileSystemWatcher fileSystemWatcher { get; set; }
        private object fileLock = new object();

        public Monitored(string filePath, IList<string> constants = null)
        {
            if (string.IsNullOrWhiteSpace(filePath) || !File.Exists(filePath))
            {
                throw new FileNotFoundException($"Cannot find file '{filePath}'.");
            }

            this.filePath = filePath;
            var json = File.ReadAllText(filePath);
            this.container = new Focus.Container(json);
            if (constants != null && constants.Count > 0)
            {
                foreach (var c in constants)
                {
                    if(string.IsNullOrWhiteSpace(c))
                    {
                        continue;
                    }

                    this.container.Constants.Add(c);
                }
            }

            this.fileSystemWatcher = new FileSystemWatcher(Path.GetDirectoryName(filePath));
            this.fileSystemWatcher.Filter = Path.GetFileName(filePath);
            this.fileSystemWatcher.Changed += OnChanged;
            this.fileSystemWatcher.EnableRaisingEvents = true;
        }

        private void OnChanged(object source, FileSystemEventArgs e)
        {
            lock (fileLock)
            {
                this.fileSystemWatcher.EnableRaisingEvents = false;

                // Because the file was changed it mean that the file might not have closed yet.
                // Sleeping here to give a chance ofr it to close.
                System.Threading.Thread.Sleep(100);
                var json = File.ReadAllText(this.filePath);
                this.container = new Focus.Container(json);
                this.fileSystemWatcher.EnableRaisingEvents = true;
            }
        }

        #region Focus access
        public JToken Focus()
        {
            return container.Focus();
        }

        public JToken Focus(IDictionary<string, string> lenses)
        {
            return container.Focus(lenses);
        }

        public T Focus<T>()
        {
            return container.Focus<T>();
        }

        public T Focus<T>(IDictionary<string, string> lenses)
        {
            return container.Focus<T>(lenses);
        }
        #endregion

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                    if (fileSystemWatcher != null)
                    {
                        fileSystemWatcher.Dispose();
                    }
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~Monitored() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}
