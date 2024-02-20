using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;

namespace Tarea_1._3_Aplicacion_de_Autores.Models
{

    [Table("Registro")]
    public class Registro
    {

        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [MaxLength(100)]
        public double Latitud { get; set; }

        [MaxLength(100)]
        public double Longitud { get; set; }

        [MaxLength(100)]
        public string Descripcion { get; set; }

        public string Imagen { get; set; }
    }
}