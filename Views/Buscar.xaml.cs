namespace Tarea_1._3_Aplicacion_de_Autores.Views;

using Microsoft.Maui.Platform;
using System.Threading.Tasks;
using Tarea_1._3_Aplicacion_de_Autores.Controllers;
using Tarea_1._3_Aplicacion_de_Autores.Models;

public partial class Buscar : ContentPage
{

    private RegistroController registro;

    public Buscar()
    {
        InitializeComponent();
        registro = new RegistroController();
    }

    private void Button_Clicked(object sender, EventArgs e)
    {

    }

    private async void ToolbarItem_Clicked(object sender, EventArgs e)
    {
        // Obtener el texto de b�squeda del Entry
        string textoBusqueda = SearchText.Text;

        // Realizar la b�squeda de autores utilizando el m�todo del controlador
        List<Registro> DireccionEncontrada = await registro.BuscarMapa(textoBusqueda);

        // Actualizar la lista de autores en la CollectionView
        ListaMapas.ItemsSource = DireccionEncontrada;
    }

    private async void ListaMapas_SelectionChangedAsync(object sender, SelectionChangedEventArgs e)
    {
        // Verificar si se seleccion� un elemento
        if (e.CurrentSelection.Count > 0)
        {
            // Obtener el elemento seleccionado de la lista
            Registro registroSeleccionado = (Registro)e.CurrentSelection[0];

            // Mostrar un cuadro de di�logo para preguntar al usuario si quiere ir a otra vista
            bool irOtraVista = await DisplayAlert("Confirmaci�n", "�Quieres ir a �sta ubicaci�n?", "S�", "No");

            // Si el usuario elige ir a otra vista
            if (irOtraVista)
            {
                // Crear una instancia de la p�gina de destino y pasar el elemento seleccionado al constructor
                MapsView nextPage = new(registroSeleccionado);

                // Navegar a la p�gina de destino
                await Navigation.PushAsync(nextPage);
            }
        }
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        ListaMapas.ItemsSource = await App.Database.ObtenerListaMapas();
    }


}