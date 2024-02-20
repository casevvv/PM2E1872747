using System.Net;
using Tarea_1._3_Aplicacion_de_Autores.Controllers;
using Tarea_1._3_Aplicacion_de_Autores.Models;


namespace Tarea_1._3_Aplicacion_de_Autores.Views;

public partial class PageActualizar : ContentPage
{
    string fotoPath;

    // Objeto seleccionado para actualizar
    private Registro _registroSeleccionado;

    // Controlador de registro
    private RegistroController registroController;

    public PageActualizar(Registro registroSeleccionado)
	{
		InitializeComponent();

        // Guardar el objeto seleccionado
        _registroSeleccionado = registroSeleccionado;

        // Inicializar el controlador de registro
        registroController = new RegistroController();

        // Mostrar los datos del registro seleccionado
        
        foto.Source = _registroSeleccionado.Imagen;
        LatitudAct.Text = _registroSeleccionado.Latitud.ToString();
        LongitudAct.Text = _registroSeleccionado.Longitud.ToString();
        DescripcionAct.Text = _registroSeleccionado.Descripcion.Trim();

    }

    private async void btnFoto_ClickedAct(object sender, EventArgs e)
    {

        var photo = await MediaPicker.CapturePhotoAsync();
        if (photo != null)
        {
            string fileName = photo.FileName;
            string path = Path.Combine(FileSystem.CacheDirectory, fileName);

            using (Stream sourcephoto = await photo.OpenReadAsync())
            using (FileStream StreamLocal = File.OpenWrite(path))
            {
                await sourcephoto.CopyToAsync(StreamLocal);
            }

            foto.Source = ImageSource.FromFile(path);

            // Guardar la ruta de la imagen en la variable fotoPath
            fotoPath = path;

        }

    }

    private async void ActualizarRegistro(Object sender, EventArgs e)
    {
        try
        {

            if (string.IsNullOrEmpty(DescripcionAct.Text))
            {
                await DisplayAlert("Error", "Ingresa una dirección válida", "OK");
                return;
            }else
            {

                IEnumerable<Location> locations = await Geocoding.Default.GetLocationsAsync(DescripcionAct.Text);
                Location newLocation = locations?.FirstOrDefault();

                if (newLocation == null)
                {
                    await DisplayAlert("Error", $"La dirección '{DescripcionAct.Text}' no pudo ser encontrada.", "OK");
                    return;
                }
                else
                {
                    double latitud = newLocation.Latitude;
                    double longitud = newLocation.Longitude;

                    // Actualizar el registro con los nuevos datos
                    if(fotoPath != null)
                    {
                        fotoPath = _registroSeleccionado.Imagen;
                    }
                    
                    _registroSeleccionado.Latitud = latitud;
                    _registroSeleccionado.Longitud = longitud;
                    _registroSeleccionado.Descripcion = DescripcionAct.Text;


                    // Actualizar el registro en la base de datos
                    int result = await registroController.ActualizarRegistro(_registroSeleccionado);

                    if (result > 0)
                    {
                        await DisplayAlert("Éxito", "Registro actualizado exitosamente.", "OK");
                        // Limpiar los campos después de guardar
                    }
                    else
                    {
                        await DisplayAlert("Error", "Error al actualizar el registro.", "OK");
                    }
                } 
            }

        }
        catch (Exception ex)
        {
            Console.WriteLine($"ERROR AL ACTUALIZAR: {ex.Message}\n\n");
            await DisplayAlert("Error", "Error al actualizar el registro.", "OK");
        }
    }

}