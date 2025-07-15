using PaymentsCLI.ConsoleUI;

namespace PaymentsCLI;

class App
{
    private readonly IConsoleUI _consoleUI;

    public App(IConsoleUI consoleUI)
    {
        _consoleUI = consoleUI;
    }

    public async Task RunAsync()
    {
        await _consoleUI.Start();
    }
}
