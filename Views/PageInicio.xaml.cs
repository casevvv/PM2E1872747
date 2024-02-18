using Microsoft.Maui.Controls.Maps;
using Microsoft.Maui.Maps;


namespace Tarea_1._3_Aplicacion_de_Autores.Views;

public partial class PageInicio : ContentPage
{

    Controllers.RegistroController registro;
    FileResult photo;

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
        String address = Descripcion.Text;

        if(string.IsNullOrEmpty(address)) {
            await DisplayAlert("Error", "Ingresa una dirección valida", "OK");
            return;
        }

        IEnumerable<Location> locations = await Geocoding.Default.GetLocationsAsync(address);

        var newLocation = locations?.FirstOrDefault();
        var dataAddress = "";
        var city = "";

        if (newLocation != null)
        {

            Latitud.Text = newLocation.Latitude.ToString();
            Longitud.Text = newLocation.Longitude.ToString();

            double latitud;
            double longitud;

            if (double.TryParse(Latitud.Text, out latitud) && double.TryParse(Longitud.Text, out longitud))
            {

                var Registro = new Models.Registro
                {
                    Longitud = longitud,
                    Latitud = latitud,
                    Descripcion = Descripcion.Text

                };

                if (await registro.AgregarRegistro(Registro) > 0)
                {
                    await DisplayAlert("Éxito", "Registro guardado exitosamente.", "OK");
                    Longitud.Text = string.Empty;
                    Latitud.Text = string.Empty;
                    Descripcion.Text = string.Empty;
                }
            }
            else
            {
                await DisplayAlert("Error", "Ingresa una dirección valida", "OK");
                return;
            }

        }
        else
        {
            await DisplayAlert("¡Dirección no encontrada!", $"La dirección '{address}' no pudo ser encontrada.", "OK");
        }
    }

    public async void btnverlista_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new Buscar());
    }



    private async void btnFoto_Clicked(object sender, EventArgs e)

    {

        photo = await MediaPicker.CapturePhotoAsync();
        if (photo != null)
        {
            string path = Path.Combine(FileSystem.CacheDirectory, photo.FileName);
            using Stream sourcephoto = await photo.OpenReadAsync();
            using FileStream StreamLocal = File.OpenWrite(path);

            foto.Source = ImageSource.FromStream(() => photo.OpenReadAsync().Result);
            await sourcephoto.CopyToAsync(StreamLocal);
        }
    }

  }
