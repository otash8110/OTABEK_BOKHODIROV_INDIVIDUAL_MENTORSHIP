using ConsoleApp.Commands;

namespace ConsoleApp
{
    internal class Invoker
    {
        private ICommand command;

        public void SetCommand(ICommand command)
        {
            this.command = command;
        }

        public async Task ExecuteCommand()
        {
            await command.Execute();
        }
    }
}
