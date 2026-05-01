namespace SadaqaAccounting.Model.Models.MasterSettings.Enum
{
    [Table("EnumTypes", Schema = "MasterSettings")]
    public class EnumType
    {
        public EnumType()
        {
            EnumTypeCollections = new HashSet<EnumTypeCollection>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        [Column(TypeName = "nvarchar(50)")]
        [Required(ErrorMessage = "Please, provide name.")]
        [StringLength(maximumLength: 50, MinimumLength = 2)]
        public string Name { get; set; }

        [Required(ErrorMessage = "Please, provide deleted or not.")]
        public bool IsDeleted { get; set; }

        public ICollection<EnumTypeCollection> EnumTypeCollections { get; set; }
    }
}