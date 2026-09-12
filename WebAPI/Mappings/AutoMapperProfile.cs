using AutoMapper;
using WebAPI.DTOs;
using WebAPI.Models;
using System.Linq;

namespace WebAPI.Mappings
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            // DB -> DTO
            CreateMap<Vrsta, VrstaDto>().ReverseMap();
            CreateMap<Pristupacnost, PristupacnostDto>().ReverseMap();

            CreateMap<Aktivnost, AktivnostDTO>()
                .ForMember(d => d.VrstaNaziv, cfg => cfg.MapFrom(s => s.Vrsta != null ? s.Vrsta.Naziv : ""))
                .ForMember(d => d.Pristupacnosti, cfg => cfg.MapFrom(s => s.AktivnostPristupacnosti.Select(ap => ap.Pristupacnost!.Naziv).ToList()))
                .ForMember(d => d.ProsjecnaOcjena, cfg => cfg.MapFrom(s => s.Recenzije.Any()? s.Recenzije.Average(r=> (double)r.Ocjena) : (double?)null));

            // DTO -> DB (kod create/update)
            CreateMap<AktivnostAddDto, Aktivnost>()
                .ForMember(d => d.Id, cfg => cfg.Ignore()); // id dolazi iz rute kod PUT
        }
    }
}
