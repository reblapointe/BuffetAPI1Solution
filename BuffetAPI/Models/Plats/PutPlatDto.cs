namespace BuffetAPI.Models.Plats
{
    public class PutPlatDto
    {
        public int Id { get; set; }
        public string? Nom { get; set; }
        public double? Prix { get; set; }
        public required int TypePlatId { get; set; }
    }
}
