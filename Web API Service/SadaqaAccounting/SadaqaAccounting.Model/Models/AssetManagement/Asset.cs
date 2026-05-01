namespace SadaqaAccounting.Model.Models.AssetManagement
{
    [Table("Assets", Schema = "AssetManagement")]
    public class Asset: IAuditableEntity, IDelatableEntity
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Please, select a unit.")]
        public int AccountUnitId { get; set; }
        [Column(TypeName = "nvarchar(100)")]
        [Required(ErrorMessage = "Please, provide name.")]
        [StringLength(maximumLength: 50, MinimumLength = 2)]
        public string Name { get; set; }
        [Required(ErrorMessage = "Please, provide asset number.")]
        public string AssetNo { get; set; }
        public DateTime PurchaseDate { get; set; }
        public decimal PurchaseValue { get; set; }
        public string? Description { get; set; }
        public int StatusId { get; set; }

        public string CreatedById { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public string? UpdatedById { get; set; }
        public DateTime? UpdatedDateTime { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedDateTime { get; set; }
        public AccountUnit AccountUnit { get; set; }
        public EnumTypeCollection Status { get; set; }
    }
}
