using System;
using System.Windows.Forms;

namespace PirateRobotArmy
{
#if WINDOWS || LINUX
    /// <summary>
    /// The main class.
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            while (true)
            {
                var mainForm = new AiSelectionForm();
                Application.Run(mainForm);
                if (mainForm.ShouldRunGame)
                {
                    using (var game = new Game1(mainForm.Game))
                        game.Run();
                }
                else
                {
                    break;
                }
            }
        }
    }
#endif
}
