namespace Tarea_1._3_Aplicacion_de_Autores
{
    public partial class App : Application
    {

        static Controllers.RegistroController database;

        public static Controllers.RegistroController Database
        { 
            get 
            {
                if (database == null)
                {
                    database = new Controllers.RegistroController();
                }
                return database; 
            }
        }


        public App()
        {
            InitializeComponent();

            MainPage = new AppShell();
        }
    }
}
