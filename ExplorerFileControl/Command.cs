using System.Collections.Generic;
using System.Linq;
using System.Windows;
using ExplorerFileControl.FileOperationDriver;

namespace ExplorerFileControl
{
    public class Command
    {
        private enum Operation
        {
            Invalid,
            Copy,
            Move,
            Delete,
            Echo,
        }

        private enum Rc
        {
            Ok,
            Error,
        }

        private static readonly Dictionary<Operation, List<string>> CommandAliases =
            new Dictionary<Operation, List<string>>
            {
                {Operation.Copy, new List<string> {"copy", "cp"}},
                {Operation.Move, new List<string> {"move", "mv"}},
                {Operation.Delete, new List<string> {"remove", "rm"}},
                {Operation.Echo, new List<string> {"echo"}},
            };

        private static readonly Dictionary<Rc, int> Rcs =
            new Dictionary<Rc, int>
            {
                {Rc.Ok, 0},
                {Rc.Error, -1},
            };

        private readonly Operation _operation = Operation.Invalid;
        private readonly List<string> _arguments;

        private readonly IFileOperationDriver _driver;


        public Command(IEnumerable<string> args)
        {
            _driver = new ShFileOperationDriver();

            var argList = new List<string>(args);
            if (argList.Count == 0)
            {
                return;
            }

            var opStr = argList[0];
            _operation = CommandAliases
                .Where(e => e.Value.Contains(opStr))
                .Select(e => e.Key)
                .DefaultIfEmpty(Operation.Invalid)
                .FirstOrDefault();
            _arguments = new List<string>(argList.Skip(1));
        }

        public int Run()
        {
            var rc = Rc.Error;

            switch (_operation)
            {
                case Operation.Invalid:
                    rc = Usage();
                    break;
                case Operation.Copy:
                    rc = Copy();
                    break;
                case Operation.Move:
                    break;
                case Operation.Delete:
                    break;
                case Operation.Echo:
                    rc = Dialog(string.Join(" ", _arguments));
                    break;
            }
            return Rcs[rc];
        }
        
        private static Rc Usage()
        {
            return Dialog(
                "Usage: ExplorerFileControl <sub command> args...\n" +
                "  sub command:\n" +
                "    cp <files...> <dst>\n" +
                "    mv <files...> <dst>\n" +
                "    rm <files...>\n" +
                ""
            );
        }

        private static Rc Dialog(string message)
        {
            MessageBox.Show(message, "ExplorerFileControl");
            return Rc.Error;
        }

        
        private Rc Copy()
        {
            if (_arguments.Count < 2)
            {
                return Usage();
            }

            var src = _arguments.Take(_arguments.Count - 1);
            var dst = _arguments.Last();
            _driver.Copy(src, dst);
            return Rc.Ok;
        }
    }
}