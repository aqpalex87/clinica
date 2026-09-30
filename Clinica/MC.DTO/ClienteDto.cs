namespace MC.DTO
{
    public class ClienteDto
    {
        public decimal? CodCliente { get; set; }
        public string TipoDoc { get; set; }
        public string NumDoc { get; set; }
        public string Nombre { get; set; }
        public string Calif_Banco { get; set; }
        public string Calif_SBS { get; set; }
        public decimal CodDeudor { get; set; }
    }
}
