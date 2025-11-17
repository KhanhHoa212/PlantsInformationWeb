using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using PlantsInformationWeb.DTOs;
using PlantsInformationWeb.Models;
using PlantsInformationWeb.ViewModels;

namespace PlantsInformationWeb.Utils
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {

            // Map from dto - model
            CreateMap<AddPlantRequestDto, Plant>()
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.Now))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(_ => "Active"));

            CreateMap<EditPlantRequestDto, Plant>()
                .ForMember(dest => dest.Diseases, opt => opt.Ignore())
                .ForMember(dest => dest.Regions, opt => opt.Ignore())
                .ForMember(dest => dest.Soils, opt => opt.Ignore());
            CreateMap<FavoritePlantDto, Favoriteplant>()
                .ForMember(dest => dest.FavoritedAt, opt => opt.MapFrom(src => src.FavoritedAt == default ? DateTime.Now : src.FavoritedAt));
            CreateMap<ChatMessageDto, Chatmessage>();
            CreateMap<ChatSessionDto, Chatsession>();
            CreateMap<PlantCommentDto, Plantcomment>();
            CreateMap<NotificationDto, Notification>();


            //map from model - dto
            CreateMap<Disease, DiseaseDto>();
            CreateMap<Unrecognizedplant, UnrecognizedplantDto>();
            CreateMap<Chatmessage, ChatMessageDto>();
            CreateMap<Notification, NotificationDto>();
            CreateMap<Chatsession, ChatSessionDto>();
            CreateMap<Usage, UsageDto>();
            CreateMap<Soiltype, SoilTypeDto>();
            CreateMap<Plantcomment, PlantCommentDto>()
                .ForMember(dest => dest.UserName,opt => opt.MapFrom(src => src.User != null ? src.User.Username : $"User #{src.UserId}"));
            CreateMap<Climate, ClimateDto>();
            CreateMap<Plantimage, PlantImageDto>();
            CreateMap<Region, RegionDto>();
            CreateMap<Plantcategory, CategoryWithCountDto>();
            CreateMap<Favoriteplant, FavoritePlantDto>();
            CreateMap<Plant, PlantSummaryDto>()
                .ForMember(dest => dest.SoilTypeIds, opt => opt.MapFrom(src => src.Soils.Select(s => s.SoilId)))
                .ForMember(dest => dest.RegionIds, opt => opt.MapFrom(src => src.Regions.Select(r => r.RegionId)))
                .ForMember(dest => dest.DiseaseIds, opt => opt.MapFrom(src => src.Diseases.Select(d => d.DiseaseId)))
                .ForMember(dest => dest.ThumbnailUrl, opt => opt.MapFrom(src => src.Plantimages.FirstOrDefault() != null ? src.Plantimages.FirstOrDefault().ImageUrl : null))
                .ForMember(dest => dest.GrowthCycle, opt => opt.MapFrom(src => src.GrowthCycle))
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.CategoryName))
                .ForMember(dest => dest.ImageUrls, opt => opt.MapFrom(src => src.Plantimages.Select(img => img.ImageUrl).ToList()))
                .ForMember(dest => dest.CategoryId, opt => opt.MapFrom(src => src.CategoryId));

            //map from model - viewmodel
            CreateMap<Plant, PlantViewModel>()
                .ForMember(dest => dest.SoilType, opt => opt.MapFrom(src => src.Soils))
                .ForMember(dest => dest.Regions, opt => opt.MapFrom(src => src.Regions))
                .ForMember(dest => dest.Disease, opt => opt.MapFrom(src => src.Diseases))
                .ForMember(dest => dest.ThumbnailUrl, opt => opt.MapFrom(src => src.Plantimages.FirstOrDefault() != null ? src.Plantimages.FirstOrDefault().ImageUrl : null))
                .ForMember(dest => dest.GrowthCycle, opt => opt.MapFrom(src => src.GrowthCycle))
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.CategoryName))
                 .ForMember(dest => dest.Climate, opt => opt.MapFrom(src => src.Climate))
                 .ForMember(dest => dest.Usages, opt => opt.MapFrom(src => src.Usages))
                 .ForMember(dest => dest.PlantImages, opt => opt.MapFrom(src => src.Plantimages))
                .ForMember(dest => dest.CategoryId, opt => opt.MapFrom(src => src.CategoryId));
            CreateMap<Plantcategory, CategoryViewModel>();
            CreateMap<Climate, ClimateViewModel>();
            CreateMap<Soiltype, SoilViewModel>();


        }

    }
}