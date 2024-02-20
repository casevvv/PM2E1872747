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

    private async void ShareLocation_Clicked(object sender, EventArgs e)
    {
        try
        {
            var location = await Geolocation.GetLastKnownLocationAsync();
            if (location != null)
            {
                // Construye el enlace a partir de la latitud y longitud
                string mapUrl = $"https://www.google.com/maps?q={location.Latitude},{location.Longitude}";

                // Comparte el enlace
                await Share.RequestAsync(new ShareTextRequest
                {
                    Text = mapUrl,
                    Title = "Compartir ubicaci�n"
                });
            }
            else
            {
                await DisplayAlert("Error", "No se pudo obtener la ubicaci�n actual.", "OK");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al compartir la ubicaci�n: {ex.Message}");
            await DisplayAlert("Error", "Error al compartir la ubicaci�n.", "OK");
        }
    }


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
                string addressNew = null, city = null, nameCountry = null, postalCode = null, street= null;

                // Obtenemos los detalles de la ubicaci�n usando la geocodificaci�n inversa
                IEnumerable<Placemark> placemarks = await Geocoding.GetPlacemarksAsync(newLocation.Latitude, newLocation.Longitude);

                // Tomamos el primer resultado (si hay alguno)
                Placemark placemark = placemarks?.FirstOrDefault();

                // Creamos una cadena con la informaci�n obtenida
                if (placemark != null)
                {
                    city = placemark.Locality;


                    if (!string.IsNullOrEmpty(placemark.SubAdminArea))
                    {
                        // Si al menos uno de los valores est� completo, se muestra esa parte de la direcci�n
                        addressNew += $"{placemark.SubAdminArea}".Trim();
                    }else if (!string.IsNullOrEmpty(placemark.SubLocality))
                    {
                        addressNew += $"{placemark.SubLocality}".Trim();
                    }
                    if (!string.IsNullOrEmpty(placemark.Thoroughfare))
                    {
                        // Si ninguno de los valores est� completo, se muestra un mensaje alternativo
                        street += $"{placemark.Thoroughfare}".Trim();
                    }else if (!string.IsNullOrEmpty(placemark.SubThoroughfare))
                    {
                        street += $"{placemark.SubThoroughfare}".Trim();
                    }
                         
                    nameCountry = placemark.CountryName;

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
                        NameCountry = nameCountry,
                        Street = street
                    }

                };

                pin.MarkerClicked += Pin_MarkerClicked;
                map.Pins.Add(pin);
                map.MoveToRegion(MapSpan.FromCenterAndRadius(newLocation, Distance.FromMeters(1000)));
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

    private async void Pin_MarkerClicked(object sender, PinClickedEventArgs e)
    {
        var pinInfo = (Pin)sender;

        await DisplayAlert("Detalles de la direcci�n",
                            $"Pa�s: {((dynamic)pinInfo.BindingContext).NameCountry}\n" +
                            $"Ciudad: {pinInfo.Label}\n" +
                            $"Direcci�n: {pinInfo.Address}\n" +
                            $"Calle principal y sub-calle: {((dynamic)pinInfo.BindingContext).Street}\n",
                            "OK");
    }

}