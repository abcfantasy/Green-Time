using System;

namespace GreenTime
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            //try
            //{
                using (Game game = new Game())
                    game.Run();
            //}

            //catch (Exception e)
            //{
            //    System.Windows.Forms.MessageBox.Show(e.ToString());
            //}
        }
    }
#endif
}

