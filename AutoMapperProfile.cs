using AutoMapper;
using Prober.Dto;
using Prober.Entities;

namespace Prober;

public class AutoMapperProfile : Profile {
  public AutoMapperProfile() {
    CreateMap<V1Alpha1ProbeEntity, ProbeDto>()
      .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Metadata.Name))
      .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Spec.Type))
      .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.Status))
      .ForMember(dest => dest.NodeStatus, opt => opt.MapFrom(src => src.Status.NodeStatus.Select(ns =>
        new NodeStatusDto() {
          Name = ns.Name,
          Status = ns.Status,
          Timestamp = ns.Timestamp
        })));
  }
}