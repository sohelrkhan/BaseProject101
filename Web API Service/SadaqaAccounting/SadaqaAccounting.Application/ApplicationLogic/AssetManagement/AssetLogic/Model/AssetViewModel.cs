using SadaqaAccounting.Model.Models.AssetManagement;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Dynamic;
using AutoMapper;

namespace SadaqaAccounting.Application.ApplicationLogic.AssetManagement.AssetLogic.Model
{
    public class AssetViewModel
    {
        public AssetCreateModel CreateModel { get; set; }
        public AssetUpdateModel UpdateModel { get; set; }
        public AssetGridModel GridModel { get; set; }
        public dynamic OptionsDataSources { get; set; } = new ExpandoObject();
    }

    public class AssetCreateModel : IMapFrom<Asset>
    {
        [Required(ErrorMessage = "Please, select a unit.")]
        public int AccountUnitId { get; set; }

        [Column(TypeName = "nvarchar(100)")]
        [Required(ErrorMessage = "Please, provide name.")]
        [StringLength(maximumLength: 50, MinimumLength = 2)]
        public string Name { get; set; }

        [Required(ErrorMessage = "Please, provide asset number.")]
        public string AssetNo { get; set; }

        [Required(ErrorMessage = "Please, provide purchase date.")]
        public DateTime PurchaseDate { get; set; }

        [Required(ErrorMessage = "Please, provide purchase value.")]
        public decimal PurchaseValue { get; set; }

        public string? Description { get; set; }

        public int StatusId { get; set; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<Asset, AssetCreateModel>();
            profile.CreateMap<AssetCreateModel, Asset>();
        }
    }

    public class AssetUpdateModel : IMapFrom<Asset>
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

        [Required(ErrorMessage = "Please, provide purchase date.")]
        public DateTime PurchaseDate { get; set; }

        [Required(ErrorMessage = "Please, provide purchase value.")]
        public decimal PurchaseValue { get; set; }

        public string? Description { get; set; }

        public int StatusId { get; set; }
        public string StatusName { get; set; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<Asset, AssetUpdateModel>()
                .ForMember(dest => dest.StatusName, opt => opt.MapFrom(src => src.Status.Name));
            profile.CreateMap<AssetUpdateModel, Asset>();
        }
    }

    public class AssetGridModel : IMapFrom<Asset>
    {
        public int Id { get; set; }
        public string EncryptedId { get; set; }
        public int AccountUnitId { get; set; }
        public string AccountUnitName { get; set; }
        public string Name { get; set; }
        public string AssetNo { get; set; }
        public DateTime PurchaseDate { get; set; }
        public decimal PurchaseValue { get; set; }
        public string? Description { get; set; }
        public int StatusId { get; set; }
        public string StatusName { get; set; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<Asset, AssetGridModel>()
                .ForMember(dest => dest.EncryptedId, s => s.MapFrom(m => EncryptionService.Encrypt(m.Id.ToString())))
                .ForMember(dest => dest.AccountUnitName, opt => opt.MapFrom(src => src.AccountUnit.Name))
                .ForMember(dest => dest.StatusName, opt => opt.MapFrom(src => src.Status.Name));
            profile.CreateMap<AssetGridModel, Asset>();
        }
    }
}
