namespace SadaqaAccounting.Shared.Models
{
    public class SelectModel
    {
        public dynamic Id { get; set; }
        public string EncryptedId { get; set; }
        public string Name { get; set; }
        public string Group { get; set; }
        public bool IsDefault { get; set; }
        public dynamic ValueOne { get; set; }
        public dynamic ValueTwo { get; set; }
        public dynamic ValueThree { get; set; }
        public dynamic ValueFour { get; set; }
        public int DisplayOrder { get; set; }
    }
}