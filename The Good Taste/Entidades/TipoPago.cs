using System;

namespace The_Good_Taste.Entidades
{
    public class TipoPago
    {
        public int IdTipoPago { get; set; }
        public string NombreTipoPago { get; set; }

        public TipoPago() { }

        public TipoPago(int idTipoPago, string nombreTipoPago)
        {
            IdTipoPago = idTipoPago;
            NombreTipoPago = nombreTipoPago;
        }

        //Sobrescribimos ToString para que al enlazarlo a un ComboBox o ListBox se muestre directamente el nombre en la interfaz.
        public override string ToString()
        {
            return NombreTipoPago;
        }
    }
}