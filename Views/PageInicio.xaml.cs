using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Maui.Controls.Maps;
using Microsoft.Maui.Devices.Sensors;
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

    public async void Button_Clicked(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(Longitud.Text))
        {
            await DisplayAlert("Error", "Por favor, ingrese longitud.", "OK");
            return;
        }


        if (string.IsNullOrWhiteSpace(Latitud.Text))
        {
            await DisplayAlert("Error", "Por favor, ingrese Latitud.", "OK");
            return;
        }

        if (string.IsNullOrWhiteSpace(Descripcion.Text))
        {
            await DisplayAlert("Error", "Por favor, ingrese Descripcion.", "OK");
            return;
        }
        var Registro = new Models.Registro
        {
            Longitud = Longitud.Text,
            Latitud = Latitud.Text,
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


    private async void Map_OnMapClicked(object sender, MapClickedEventArgs e)
    {
        // Obtenemos la información de geocodificación inversa para las coordenadas del punto tocado en el mapa
        string locationInfo = await GetGeocodeReverseData(e.Location.Latitude, e.Location.Longitude);

        // Mostramos la información al usuario
        await DisplayAlert("Detalles de la Ubicación", locationInfo, "OK");
    }

    private async Task<string> GetGeocodeReverseData(double latitude, double longitude)
    {
        // Obtenemos los detalles de la ubicación usando la geocodificación inversa
        IEnumerable<Placemark> placemarks = await Geocoding.GetPlacemarksAsync(latitude, longitude);

        // Tomamos el primer resultado (si hay alguno)
        Placemark placemark = placemarks?.FirstOrDefault();

        // Creamos una cadena con la información obtenida
        if (placemark != null)
        {
            return
                $"País: {placemark.CountryName}\n" +
                $"Ciudad: {placemark.Locality}\n" +
                $"Código Postal: {placemark.PostalCode}\n" +
                $"Dirección: {placemark.Thoroughfare} {placemark.SubThoroughfare}\n";
        }
        else
        {
            return "No se pudo encontrar información para esta ubicación.";
        }
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        var geolocationRequest = new GeolocationRequest(GeolocationAccuracy.High, TimeSpan.FromSeconds(10));
        var location = await Geolocation.GetLocationAsync(geolocationRequest);

        map.MoveToRegion(MapSpan.FromCenterAndRadius(location, Distance.FromMiles(10)));

        // Obtenemos los detalles de la ubicación usando la geocodificación inversa
        IEnumerable<Placemark> placemarks = await Geocoding.GetPlacemarksAsync(location.Latitude, location.Longitude);

        // Tomamos el primer resultado (si hay alguno)
        Placemark placemark = placemarks?.FirstOrDefault();
        var addressUser = "";
        var label = "";
        if (placemark != null)
        {
            //{placemark.Locality},
            addressUser = $"{placemark.Thoroughfare} {placemark.SubThoroughfare}";
            label = $"{placemark.SubLocality}";
        }
        else
        {
            await DisplayAlert("¡Dirección no encontrada!", "OK");
        }

        var pin = new Pin
        {
            Address = addressUser,
            Location = new Location(location.Latitude, location.Longitude),
            Type = PinType.Place,
            Label = label
        };

        Latitud.Text = location.Latitude.ToString();
        Longitud.Text = location.Longitude.ToString();

        pin.MarkerClicked += Pin_MarkerClicked;
        map.Pins.Add(pin);

    }

    private async Task DisplayAlert(string v1, string v2)
    {
        throw new NotImplementedException();
    }

    private async void Pin_MarkerClicked(object sender, PinClickedEventArgs e)
    {
        var pinInfo = (Pin)sender;

        await DisplayAlert("¡Hola!,", pinInfo.Address, "OK");
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
