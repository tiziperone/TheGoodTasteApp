using System;
using The_Good_Taste.Datos;

namespace TheGoodTaste.Negocio
{
    public class BackupNegocio
    {
        private readonly BackupDatos _datos = new BackupDatos();

        public bool RealizarBackup(string rutaDestino)
        {
            if (string.IsNullOrWhiteSpace(rutaDestino))
                throw new ArgumentException("Debe especificar una ruta válida para el backup.");

            return _datos.GenerarBackup(rutaDestino);
        }

        public bool RestaurarBackup(string rutaOrigen)
        {
            if (string.IsNullOrWhiteSpace(rutaOrigen))
                throw new ArgumentException("Debe seleccionar un archivo de backup válido.");

            return _datos.RestaurarBackup(rutaOrigen);
        }
    }
}