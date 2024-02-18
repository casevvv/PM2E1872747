﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using SQLite;
using Tarea_1._3_Aplicacion_de_Autores.Models;

namespace Tarea_1._3_Aplicacion_de_Autores.Controllers
{
    public class RegistroController
    {
        readonly SQLiteAsyncConnection _connection;

        // Constructor VACIO
        public RegistroController() 
        {
            SQLite.SQLiteOpenFlags extensiones = SQLite.SQLiteOpenFlags.ReadWrite |
                                                SQLite.SQLiteOpenFlags.Create |
                                                SQLite.SQLiteOpenFlags.SharedCache;

            _connection = new SQLiteAsyncConnection(Path.Combine(FileSystem.AppDataDirectory, "E1PM2.db3"), extensiones);

            _connection.CreateTableAsync<Registro>();
        }

        // Inicialización


        // CREATE
        public async Task<int> AgregarRegistro(Registro registro)
        {

            if (registro.Id == 0)
            {
                return await _connection.InsertAsync(registro);
            }
            else
            {
                return await _connection.UpdateAsync(registro);
            }
        }

        // READ
        public async Task<List<Registro>> ObtenerListaMapas()
        {
           
            return await _connection.Table<Registro>().ToListAsync();
        }

        // READ elemento
        public async Task<Registro> ObtenerMapa(int id)
        {
            
            return await _connection.Table<Registro>().Where(i => i.Id == id).FirstOrDefaultAsync();
        }

        // DELETE
        public async Task<int> EliminarRegistro(Registro mapa)
        {
            
            return await _connection.DeleteAsync(mapa);
        }

        // Método para buscar autores por nombre
        public async Task<List<Registro>> BuscarMapa(string textoBusqueda)
        {
            // Realizar la búsqueda utilizando LINQ
            return await _connection.Table<Registro>()
                                    .Where(a => a.Descripcion.Contains(textoBusqueda))
                                    .ToListAsync();
        }
    }
}

