using Microsoft.Maui;
using Microsoft.Maui.Hosting;
using System;

namespace Tarea_1._3_Aplicacion_de_Autores
{
    internal class Program : MauiApplication
    {
        protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();

        static void Main(string[] args)
        {
            var app = new Program();
            app.Run(args);
        }
    }
}
