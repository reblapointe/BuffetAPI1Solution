﻿namespace BuffetAPI.Models
{
    public class Plat
    {
        public int Id { get; set; }
        public string? Nom { get; set; }
        public double? Prix { get; set; }
        public required int TypePlatId { get; set; }
        public TypePlat? TypePlat { get; set; }
    }
}
