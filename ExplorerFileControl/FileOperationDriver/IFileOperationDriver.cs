using System.Collections.Generic;

namespace ExplorerFileControl.FileOperationDriver
{
    public interface IFileOperationDriver
    {
        void Copy(IEnumerable<string> src, string dst);
        void Move(IEnumerable<string> src, string dst);
        void Delete(IEnumerable<string> files);
    }
}