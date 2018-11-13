using System;

namespace ExplorerFileControl
{
    public static class App
    {
        [STAThread]
        public static int Main(string[] args)
        {
            var rc = new Command(args).Run();
            return rc;
        }
    }
}