using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cc3k.Menus
{
    public abstract class GameMenu
    {
        public bool Active { get; protected set; }

        protected string? Notification { get; set; }

        public GameMenu()
        {
            Active = true;
        }

        private void Render()
        {
            Console.Clear();

            DisplayMenu();

            if (Notification != null)
            {
                var temp = Console.ForegroundColor;

                if (Notification.StartsWith("Error:"))
                    Console.ForegroundColor = ConsoleColor.Red; // red = bad/break
                else
                    Console.ForegroundColor = ConsoleColor.Gray; //notifications arent green so.. they stand out
                Console.WriteLine(Notification);
                Notification = null;
                Console.ForegroundColor = temp;
            }

            try
            {
                HandleInput();
            }
            catch (MenuException ex)
            {
                Notification = $"Error: {ex.Message}";
            }
        }

        public void Display(bool blocking = true)
        {
            if (blocking)
            {
                while (Active)
                {
                    Render();
                }
            }
            else
            {
                Render();
            }
        }

        protected abstract void DisplayMenu();
        protected abstract void HandleInput();
    }
}
