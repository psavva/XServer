using Renci.SshNet;
using System;
using System.Diagnostics;
using System.Text;
using System.Threading;

namespace x42.Feature.WordPressPreview
{
    public class InteractiveShellStream : IDisposable
    {
        private const int TIMEOUT = 3000;
        private bool disposed = false;
        private readonly Stopwatch wait;
        private readonly ShellStream shell;

        public InteractiveShellStream(ShellStream shellStream)
        {
            wait = new Stopwatch();
            shell = shellStream;

            wait.Start();
            while (!shell.DataAvailable && wait.ElapsedMilliseconds < TIMEOUT)
                Thread.Sleep(200);
            wait.Stop();
        }

        public void Write(string command)
        {
            shell.Write(command);
        }

        public void WriteLine(string command)
        {
            shell.WriteLine(command);
        }

        public string Read()
        {
            return shell.Read();
        }

        public string ReadLine()
        {
            return shell.ReadLine();
        }

        public string ReadWithTimeout(int timeOut)
        {
            StringBuilder buffer = new StringBuilder();

            wait.Restart();
            while (!shell.DataAvailable)
                Thread.Sleep(100);
            wait.Stop();

            while (shell.DataAvailable)
            {
                buffer.Append(shell.Read());
                Console.WriteLine(buffer.ToString());
                buffer.Clear();

                wait.Restart();
                while (!shell.DataAvailable && wait.ElapsedMilliseconds < timeOut)
                    Thread.Sleep(200);
                wait.Stop();
            }

            return buffer.ToString();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (disposing)
            {
                shell.Dispose();
            }

            disposed = true;
        }

        ~InteractiveShellStream()
        {
            Dispose(false);
        }
    }
}
