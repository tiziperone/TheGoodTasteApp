using System;
using System.Data.SqlClient;

namespace The_Good_Taste.Datos
{
    public class BackupDatos
    {
        // Conexión a la base master en LocalDB
        private readonly string _cadenaMaster = @"Server=(localdb)\MSSQLLocalDB;Database=master;Integrated Security=True;";

        public bool GenerarBackup(string rutaDestino)
        {
            string query = @"BACKUP DATABASE [TheGoodTasteDB] 
                             TO DISK = @Ruta 
                             WITH FORMAT, INIT, 
                                  NAME = N'Full Backup TheGoodTasteDB';";

            using (SqlConnection con = new SqlConnection(_cadenaMaster))
            {
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@Ruta", rutaDestino);
                con.Open();
                cmd.ExecuteNonQuery();
                return true;
            }
        }

        public bool RestaurarBackup(string rutaOrigen)
        {
            string query = @"ALTER DATABASE [TheGoodTasteDB] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
                             RESTORE DATABASE [TheGoodTasteDB] 
                             FROM DISK = @Ruta 
                             WITH REPLACE, RECOVERY;
                             ALTER DATABASE [TheGoodTasteDB] SET MULTI_USER;";

            using (SqlConnection con = new SqlConnection(_cadenaMaster))
            {
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@Ruta", rutaOrigen);
                con.Open();
                cmd.ExecuteNonQuery();
                return true;
            }
        }
    }
}