using Microsoft.Maui.Controls.Maps;
using Microsoft.Maui.Maps;
using Tarea_1._3_Aplicacion_de_Autores.Models;

namespace Tarea_1._3_Aplicacion_de_Autores.Views;

public partial class MapsView : ContentPage
{
    private Registro registroSeleccionado;

    public MapsView(Registro registroSeleccionado)
    {
        InitializeComponent();
        this.registroSeleccionado = registroSeleccionado;

        GetValueDescription(this.registroSeleccionado.Descripcion);
    }

    public List<Pin> Pins { get; set; } = new List<Pin>();


    private async void GetValueDescription(string direccion)
    {
        try
        {
            // Realiza la búsqueda de dirección en el mapa
            String address = direccion.Trim();

            IEnumerable<Location> locations = await Geocoding.Default.GetLocationsAsync(address);

            var newLocation = locations?.FirstOrDefault();

            if (newLocation != null)
            {
                string addressNew = null, city = null, nameCountry = null, postalCode = null;

                // Obtenemos los detalles de la ubicación usando la geocodificación inversa
                IEnumerable<Placemark> placemarks = await Geocoding.GetPlacemarksAsync(newLocation.Latitude, newLocation.Longitude);

                // Tomamos el primer resultado (si hay alguno)
                Placemark placemark = placemarks?.FirstOrDefault();

                // Creamos una cadena con la información obtenida
                if (placemark != null)
                {
                    city = placemark.Locality;

                    if (!string.IsNullOrEmpty(placemark.Thoroughfare) || !string.IsNullOrEmpty(placemark.SubThoroughfare))
                    {
                        // Si al menos uno de los valores está completo, se muestra esa parte de la dirección
                        addressNew = $"{placemark.Thoroughfare} {placemark.SubThoroughfare}".Trim();
                    }
                    else
                    {
                        // Si ninguno de los valores está completo, se muestra un mensaje alternativo
                        addressNew = $"{placemark.Thoroughfare} {placemark.SubThoroughfare}".Trim();
                    }

                    nameCountry = placemark.CountryName;
                    postalCode = placemark.PostalCode;

                }
                else
                {
                    await DisplayAlert("¡Dirección no encontrada!", $"La dirección no pudo ser encontrada.", "OK");
                }


                var pin = new Pin
                {
                    Address = addressNew,
                    Location = new Location(newLocation.Latitude, newLocation.Longitude),
                    Type = PinType.Place,
                    Label = city,
                    BindingContext = new
                    {
                        PostalCode = postalCode,
                        NameCountry = nameCountry
                    }

                };

                pin.MarkerClicked += Pin_MarkerClicked;
                map.Pins.Add(pin);
                map.MoveToRegion(MapSpan.FromCenterAndRadius(newLocation, Distance.FromMiles(10)));
            }
            else
            {
                await DisplayAlert("¡Dirección no encontrada!", $"La dirección '{address}' no pudo ser encontrada.", "OK");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al obtener la descripción de la dirección: {ex.Message}");
        }
    }

    private async void DisplayAlert(string title, string message)
    {
        await Application.Current.MainPage.DisplayAlert(title, message, "OK");
    }


    private async void Pin_MarkerClicked(object sender, PinClickedEventArgs e)
    {
        var pinInfo = (Pin)sender;

        await DisplayAlert("Detalles de la dirección",
                            $"País: {((dynamic)pinInfo.BindingContext).NameCountry}\n" +
                            $"Ciudad: {pinInfo.Label}\n" +
                            $"Dirección: {pinInfo.Address}\n" +
                            $"Código Postal: {((dynamic)pinInfo.BindingContext).PostalCode}",
                            "OK");
    }

}