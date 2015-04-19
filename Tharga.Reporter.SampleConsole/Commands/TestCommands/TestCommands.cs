using Tharga.Toolkit.Console.Command.Base;

namespace Tharga.Reporter.ConsoleSample.Commands.TestCommands
{
    public class TestCommands : ContainerCommandBase
    {
        public TestCommands()
            : base("test")
        {
            RegisterCommand(new TestFileCommand());
        }
    }
}