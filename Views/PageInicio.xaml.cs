using Microsoft.Maui.Controls.Maps;
using Microsoft.Maui.Maps;


namespace Tarea_1._3_Aplicacion_de_Autores.Views;

public partial class PageInicio : ContentPage
{

    Controllers.RegistroController registro;
    string fotoPath;

    public List<Pin> Pins { get; set; } = new List<Pin>();

    public PageInicio()
	{
        registro = new Controllers.RegistroController();
		InitializeComponent();
	}

    public PageInicio(Controllers.RegistroController dbpath)
    {      
        InitializeComponent();
        registro = dbpath;
    }

    private async void GetValueDescription(object sender, EventArgs e)
    {
        // Realiza la búsqueda de dirección en el mapa
        string address = Descripcion.Text;

        if (string.IsNullOrEmpty(address))
        {
            await DisplayAlert("Error", "Ingresa una dirección", "OK");
            return;
        }

        IEnumerable<Location> locations = await Geocoding.Default.GetLocationsAsync(address);

        Location newLocation = locations?.FirstOrDefault();
        if (newLocation == null)
        {
            await DisplayAlert("Error", $"La dirección '{address}' no pudo ser encontrada.", "OK");
            return;
        }

        double latitud = newLocation.Latitude;
        double longitud = newLocation.Longitude;

        if (IsValidLatitude(latitud) && IsValidLongitude(longitud))
        {
            try
            {
                Models.Registro registro = new Models.Registro
                {
                    Imagen = fotoPath,
                    Latitud = latitud,
                    Longitud = longitud,
                    Descripcion = address
                };


                if (await this.registro.AgregarRegistro(registro) > 0)
                {
                    await DisplayAlert("Éxito", "Registro guardado exitosamente.", "OK");
                    // Limpiar los campos después de guardar
                    LimpiarCampos();
                }
                else
                {
                    await DisplayAlert("Error", "Error al guardar el registro.", "OK");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al guardar el registro: {ex.Message}");
                await DisplayAlert("Error", "Error al guardar el registro.", "OK");
            }
        }
        else
        {
            await DisplayAlert("Error", "Latitud y longitud no válidas.", "OK");
        }
    }

    private void LimpiarCampos()
    {
        Longitud.Text = string.Empty;
        Latitud.Text = string.Empty;
        Descripcion.Text = string.Empty;
        foto.Source = null;
    }

    private bool IsValidLatitude(double latitude)
    {
        // Validar latitud según su rango
        return latitude >= -90 && latitude <= 90;
    }

    private bool IsValidLongitude(double longitude)
    {
        // Validar longitud según su rango
        return longitude >= -180 && longitude <= 180;
    }


    public async void btnverlista_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new Buscar());
    }



    private async void btnFoto_Clicked(object sender, EventArgs e)
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

  }
