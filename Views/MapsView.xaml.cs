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
            // Realiza la b�squeda de direcci�n en el mapa
            String address = direccion.Trim();

            IEnumerable<Location> locations = await Geocoding.Default.GetLocationsAsync(address);

            var newLocation = locations?.FirstOrDefault();

            if (newLocation != null)
            {
                string addressNew = null, city = null, nameCountry = null, postalCode = null;

                // Obtenemos los detalles de la ubicaci�n usando la geocodificaci�n inversa
                IEnumerable<Placemark> placemarks = await Geocoding.GetPlacemarksAsync(newLocation.Latitude, newLocation.Longitude);

                // Tomamos el primer resultado (si hay alguno)
                Placemark placemark = placemarks?.FirstOrDefault();

                // Creamos una cadena con la informaci�n obtenida
                if (placemark != null)
                {
                    city = placemark.Locality;

                    if (!string.IsNullOrEmpty(placemark.Thoroughfare) || !string.IsNullOrEmpty(placemark.SubThoroughfare))
                    {
                        // Si al menos uno de los valores est� completo, se muestra esa parte de la direcci�n
                        addressNew = $"{placemark.Thoroughfare} {placemark.SubThoroughfare}".Trim();
                    }
                    else
                    {
                        // Si ninguno de los valores est� completo, se muestra un mensaje alternativo
                        addressNew = $"{placemark.Thoroughfare} {placemark.SubThoroughfare}".Trim();
                    }

                    nameCountry = placemark.CountryName;
                    postalCode = placemark.PostalCode;

                }
                else
                {
                    await DisplayAlert("�Direcci�n no encontrada!", $"La direcci�n no pudo ser encontrada.", "OK");
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
                await DisplayAlert("�Direcci�n no encontrada!", $"La direcci�n '{address}' no pudo ser encontrada.", "OK");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al obtener la descripci�n de la direcci�n: {ex.Message}");
        }
    }

    private async void DisplayAlert(string title, string message)
    {
        await Application.Current.MainPage.DisplayAlert(title, message, "OK");
    }


    private async void Pin_MarkerClicked(object sender, PinClickedEventArgs e)
    {
        var pinInfo = (Pin)sender;

        await DisplayAlert("Detalles de la direcci�n",
                            $"Pa�s: {((dynamic)pinInfo.BindingContext).NameCountry}\n" +
                            $"Ciudad: {pinInfo.Label}\n" +
                            $"Direcci�n: {pinInfo.Address}\n" +
                            $"C�digo Postal: {((dynamic)pinInfo.BindingContext).PostalCode}",
                            "OK");
    }

}