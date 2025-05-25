using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using Pawesome.Models.Dtos.Advert;
using Pawesome.Models.ViewModels.AnimalType;

namespace Pawesome.Models.ViewModels.Advert
{
    public class AdvertViewModel
    {
        public List<PetSittingAdvertDto> Adverts { get; set; } = new();
        public bool? IsPetSitterOffer { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public decimal MinPriceBeforeReload { get; set; } = 0;
        public decimal MaxPriceBeforeReload { get; set; } = 1000;
        
        public DateTime? StartDateFrom { get; set; }
        public DateTime? EndDateTo { get; set; }
        public List<int>? AnimalTypeIds { get; set; } = new();
        public string? City { get; set; }
        public int? CityId { get; set; }
        public int? CountryId { get; set; }
        public string? PostalCode { get; set; }
        public DateTime? CreatedAtFrom { get; set; }
        public DateTime? CreatedAtTo { get; set; }
        
        public List<AnimalTypeViewModel>? AnimalTypes { get; set; } = new();
        public List<string>? SelectedAnimalTypes { get; set; } = new();
        
        public List<SelectListItem> AnimalTypeOptions { get; set; } = new();
        public List<SelectListItem> CityOptions { get; set; } = new();
        public List<SelectListItem> CountryOptions { get; set; } = new();
        
        public string SortOption { get; set; } = "newest";
        public List<SelectListItem> SortOptions { get; set; } = new List<SelectListItem>
        {
            new SelectListItem { Value = "newest", Text = "Plus récent", Selected = true },
            new SelectListItem { Value = "oldest", Text = "Plus ancien" },
            new SelectListItem { Value = "price_asc", Text = "Prix croissant" },
            new SelectListItem { Value = "price_desc", Text = "Prix décroissant" },
            new SelectListItem { Value = "date_start_asc", Text = "Date de début (croissant)" },
            new SelectListItem { Value = "date_end_asc", Text = "Date de fin (croissant)" }
        };
    }
}
