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
    {// Verificar si se seleccion� un elemento
        if (e.CurrentSelection.Count > 0)
        {
            // Obtener el elemento seleccionado de la lista
            Registro registroSeleccionado = (Registro)e.CurrentSelection[0];
            // Display options to the user
            string action = await DisplayActionSheet("Options", "�Ir a �sta ubicaci�n?", "Actualizar", "Eliminar");

            // Mostrar un cuadro de di�logo para preguntar al usuario
            switch (action)
            {
                case "�Ir a �sta ubicaci�n?":
                    //Crear una instancia de la p�gina de destino y pasar el elemento seleccionado al constructor
                    MapsView nextPage = new(registroSeleccionado);
                    //Navegar a la p�gina de destino
                    await Navigation.PushAsync(nextPage);
                    break;

                case "Actualizar":
                    PageActualizar actualizar = new PageActualizar(registroSeleccionado);
                    await Navigation.PushAsync(actualizar);
                    break;

                case "Eliminar":
                    await DeleteRegistro(registroSeleccionado);

                    break;
            }
        }
    }

    private async Task DeleteRegistro(Registro registro)
    {


        try
        {
            int result = await this.registro.EliminarRegistro(registro);

            if (result > 0)
            {
                await DisplayAlert("Success", "Registro eliminado correctamente.", "OK");
                // Refresh the list after deletion
                ListaMapas.ItemsSource = await this.registro.ObtenerListaMapas();
            }
            else
            {
                await DisplayAlert("Error", "No se pudo eliminar el registro.", "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Error al eliminar el registro: {ex.Message}", "OK");
        }
    }


    protected override async void OnAppearing()
    {
        base.OnAppearing();

        ListaMapas.ItemsSource = await App.Database.ObtenerListaMapas();
    }


}