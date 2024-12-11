namespace BuffetAPI.Models.Plats
{
    public class PostPlatDto
    {
        public string? Nom { get; set; }
        public double? Prix { get; set; }
        public required int TypePlatId { get; set; }
    }
}
