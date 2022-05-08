using AutoMapper;
using EventBusRabbitMQ.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ReportMicroService.Entities;
using ReportMicroService.Extensions;

namespace ReportMicroService.Mapping
{
    public class ReportMapping : Profile
    {
        public ReportMapping()
        {
            CreateMap<ReportCreateEvent, Report>()
                .IgnoreAllSourcePropertiesWithAnInaccessibleSetter()
            ;
            CreateMap<Report, ReportCreateEvent>()
                .IgnoreAllSourcePropertiesWithAnInaccessibleSetter()
                .Ignore(m => m.CrateDate)
                .Ignore(m => m.Data)
                .Ignore(m => m.RequestId)
                .Ignore(m => m.Status)
            ;
        }
    }
}
