
namespace JetSquid;
using System;

public static class Program
{
    private const int WIDTH = 1920;
    private const int HEIGHT = 1080;

    [STAThread]
    static void Main()
    {
        using (var game = new JetSquid(WIDTH, HEIGHT, new JetSquidStartState(), false))
        game.Run();
    }
}