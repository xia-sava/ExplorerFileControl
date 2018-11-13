using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace ExplorerFileControl.FileOperationDriver
{
    internal enum Func : uint
    {
        Move = 0x0001,
        Copy = 0x0002,
        Delete = 0x0003,
        // Rename = 0x0004,
    }

    [Flags]
    internal enum Flags : UInt16
    {
        MultiDestFiles = 0x0001,
        ConfirmMouse = 0x0002,
        Silent = 0x0004,
        RenameOnCollision = 0x0008,
        NoConfirmation = 0x0010,
        WantMappingHandle = 0x0020,
        AllowUndo = 0x0040,
        FilesOnly = 0x0080,
        SimpleProgress = 0x0100,
        NoConfirmMkdir = 0x0200,
        NoErrorUi = 0x0400,
        NoCopySecurityAttribs = 0x0800,
        NoRecursion = 0x1000,
        NoConnectedElements = 0x2000,
        WantNukeWarning = 0x4000,
        NoRecurseReparse = 0x8000,
    }


    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    internal struct SHFILEOPSTRUCT
    {
        public IntPtr hwnd;
        public Int32 wFunc;
        [MarshalAs(UnmanagedType.LPWStr)] public String pFrom;
        [MarshalAs(UnmanagedType.LPWStr)] public String pTo;
        public Int16 fFlags;
        public Int32 fAnyOperationAborted;
        public IntPtr hNameMapping;
        [MarshalAs(UnmanagedType.LPWStr)] public String lpszProgressTitle;
    }

    internal class ShFileOp
    {
        public Window ParentWindow { get; set; }
        public Func Operation { get; set; }
        public Flags Flags { get; set; }
        public IEnumerable<string> From { get; set; }
        public IEnumerable<string> To { get; set; }
        public string ProgressTitle { get; set; } = "ExplorerFileControl";

        public ShFileOp()
        {
            Flags = 0;
            From = new string[]{};
            To = new string[]{};
        }

        public SHFILEOPSTRUCT ToStruct()
        {
            var fileOp = new SHFILEOPSTRUCT
            {
                wFunc = Convert.ToInt16(Operation),
                fFlags = Convert.ToInt16(Flags),
                pFrom = string.Join("\0", From) + "\0\0",
                pTo = string.Join("\0", To) + "\0\0",
                fAnyOperationAborted = 0,
                hNameMapping = IntPtr.Zero,
                lpszProgressTitle = ProgressTitle
            };
            if (ParentWindow != null)
            {
                fileOp.hwnd = new WindowInteropHelper(ParentWindow).Handle;
            }
            return fileOp;
        }
    }

    
    public class ShFileOperationDriver : IFileOperationDriver
    {
        public void Copy(IEnumerable<string> src, string dst)
        {
            var lpFileOp = new ShFileOp
            {
                Operation = Func.Copy,
                Flags = Flags.AllowUndo | Flags.SimpleProgress,
                From = src,
                To = new []{dst},
            };
            ExecShFileOperation(lpFileOp);
        }

        public void Move(IEnumerable<string> src, string dst)
        {
            var lpFileOp = new ShFileOp
            {
                Operation = Func.Move,
                Flags = Flags.AllowUndo | Flags.SimpleProgress,
                From = src,
                To = new []{dst},
            };
            ExecShFileOperation(lpFileOp);
        }

        public void Delete(IEnumerable<string> files)
        {
            var lpFileOp = new ShFileOp
            {
                Operation = Func.Copy,
                Flags = Flags.AllowUndo | Flags.SimpleProgress,
                From = files,
            };
            ExecShFileOperation(lpFileOp);
        }

        private static void ExecShFileOperation(ShFileOp lpFileOp)
        {
            var lpFileOpStruct = lpFileOp.ToStruct();
            SHFileOperation(ref lpFileOpStruct);
        }

        [DllImport("Shell32.dll", CharSet = CharSet.Unicode)]
        private static extern Int32 SHFileOperation(ref SHFILEOPSTRUCT lpFileOp);

    }
}