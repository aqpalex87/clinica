namespace MC.Business.Entity.Layer
{
    public class Cliente
    {
        public decimal CodCliente { get; set; }
        public string TipoDoc { get; set; }
        public string NumDoc { get; set; }
        public string Nombre { get; set; }
        public string Calif_Banco { get; set; }
        public string Calif_SBS { get; set; }
        public int CodDeudor { get; set; }
    }
}
