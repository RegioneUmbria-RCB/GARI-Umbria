using AgronicaCoreDTOStd.InData.Zoo;
using AutoMapper;
using InData;
using InData.Agenda;
using InData.Zoo;
using System.Reflection;

namespace AgronicaNetCoreApi.Extensions
{
    public class DataRowWrapperToDtoConverter<TDto> : ITypeConverter<DataRowWrapper, TDto>
    where TDto : new()
    {
        public TDto Convert(DataRowWrapper source, TDto destination, ResolutionContext context)
        {
            var dto = new TDto();
            var dtoProps = typeof(TDto).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (var prop in dtoProps)
            {
                if (!prop.CanWrite) continue;

                var rawValue = source.GetValue(prop.Name);
                if (rawValue == null) continue;

                try
                {
                    var targetType = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;
                    var convertedValue = System.Convert.ChangeType(rawValue, targetType);
                    prop.SetValue(dto, convertedValue);
                }
                catch
                {
                    // optionally log skipped fields
                    continue;
                }
            }

            return dto;
        }
    }

    public class MappingProfile : Profile
    {
        public MappingProfile() 
        {
            #region Agenda

            CreateMap(typeof(DataRowWrapper), typeof(WriteRicette)).ConvertUsing(typeof(DataRowWrapperToDtoConverter<>).MakeGenericType(typeof(WriteRicette)));
            CreateMap(typeof(DataRowWrapper), typeof(WriteRicettexAgenda)).ConvertUsing(typeof(DataRowWrapperToDtoConverter<>).MakeGenericType(typeof(WriteRicettexAgenda)));
            CreateMap(typeof(DataRowWrapper), typeof(WriteAgenda)).ConvertUsing(typeof(DataRowWrapperToDtoConverter<>).MakeGenericType(typeof(WriteAgenda)));
            CreateMap(typeof(DataRowWrapper), typeof(WriteLogAgenda)).ConvertUsing(typeof(DataRowWrapperToDtoConverter<>).MakeGenericType(typeof(WriteLogAgenda)));
            CreateMap(typeof(DataRowWrapper), typeof(WriteMovimenti)).ConvertUsing(typeof(DataRowWrapperToDtoConverter<>).MakeGenericType(typeof(WriteMovimenti)));
            CreateMap(typeof(DataRowWrapper), typeof(WriteMovDettagli)).ConvertUsing(typeof(DataRowWrapperToDtoConverter<>).MakeGenericType(typeof(WriteMovDettagli)));
            CreateMap(typeof(DataRowWrapper), typeof(WriteMovDestinazioni)).ConvertUsing(typeof(DataRowWrapperToDtoConverter<>).MakeGenericType(typeof(WriteMovDestinazioni)));
            CreateMap(typeof(DataRowWrapper), typeof(WriteMovDettTecnico)).ConvertUsing(typeof(DataRowWrapperToDtoConverter<>).MakeGenericType(typeof(WriteMovDettTecnico)));
            CreateMap(typeof(DataRowWrapper), typeof(WriteMovDettTecnicoExtra)).ConvertUsing(typeof(DataRowWrapperToDtoConverter<>).MakeGenericType(typeof(WriteMovDettTecnicoExtra)));

            #endregion

            #region Zoo

            //CreateMap(typeof(DataRowWrapper), typeof(WriteZooAnimali)).ConvertUsing(typeof(DataRowWrapperToDtoConverter<>).MakeGenericType(typeof(WriteZooAnimali)));
            //CreateMap(typeof(DataRowWrapper), typeof(WriteZooDistinte)).ConvertUsing(typeof(DataRowWrapperToDtoConverter<>).MakeGenericType(typeof(WriteZooDistinte)));
            //CreateMap(typeof(DataRowWrapper), typeof(WriteZooStatiAccrescimento)).ConvertUsing(typeof(DataRowWrapperToDtoConverter<>).MakeGenericType(typeof(WriteZooStatiAccrescimento)));

            CreateMap(typeof(DataRowWrapper), typeof(WriteMovimentiZoo)).ConvertUsing(typeof(DataRowWrapperToDtoConverter<>).MakeGenericType(typeof(WriteMovimentiZoo)));

            CreateMap(typeof(DataRowWrapper), typeof(WriteRicetteZoo)).ConvertUsing(typeof(DataRowWrapperToDtoConverter<>).MakeGenericType(typeof(WriteRicetteZoo)));
            CreateMap(typeof(DataRowWrapper), typeof(WriteRicetteZooAgenda)).ConvertUsing(typeof(DataRowWrapperToDtoConverter<>).MakeGenericType(typeof(WriteRicetteZooAgenda)));
            CreateMap(typeof(DataRowWrapper), typeof(WriteRicetteZooxAgenda)).ConvertUsing(typeof(DataRowWrapperToDtoConverter<>).MakeGenericType(typeof(WriteRicetteZooxAgenda)));
            CreateMap(typeof(DataRowWrapper), typeof(WriteRicetteZooMovimenti)).ConvertUsing(typeof(DataRowWrapperToDtoConverter<>).MakeGenericType(typeof(WriteRicetteZooMovimenti)));
            CreateMap(typeof(DataRowWrapper), typeof(WriteRicetteZooDettagli)).ConvertUsing(typeof(DataRowWrapperToDtoConverter<>).MakeGenericType(typeof(WriteRicetteZooDettagli)));
            CreateMap(typeof(DataRowWrapper), typeof(WriteRicetteZooDestinazioni)).ConvertUsing(typeof(DataRowWrapperToDtoConverter<>).MakeGenericType(typeof(WriteRicetteZooDestinazioni)));
            CreateMap(typeof(DataRowWrapper), typeof(WriteRicetteZooDettaglioTecnico)).ConvertUsing(typeof(DataRowWrapperToDtoConverter<>).MakeGenericType(typeof(WriteRicetteZooDettaglioTecnico)));
            CreateMap(typeof(DataRowWrapper), typeof(WriteTerapia)).ConvertUsing(typeof(DataRowWrapperToDtoConverter<>).MakeGenericType(typeof(WriteTerapia)));
            CreateMap(typeof(DataRowWrapper), typeof(WriteIntervento)).ConvertUsing(typeof(DataRowWrapperToDtoConverter<>).MakeGenericType(typeof(WriteIntervento)));
            CreateMap(typeof(DataRowWrapper), typeof(WriteInterventoxProtocollo)).ConvertUsing(typeof(DataRowWrapperToDtoConverter<>).MakeGenericType(typeof(WriteInterventoxProtocollo)));

            // for dto cloning
            CreateMap<WriteRicetteZoo, WriteRicetteZoo>();
            CreateMap<WriteRicetteZooAgenda, WriteRicetteZooAgenda>();
            CreateMap<WriteRicetteZooxAgenda, WriteRicetteZooxAgenda>();
            CreateMap<WriteRicetteZooMovimenti, WriteRicetteZooMovimenti>();
            CreateMap<WriteRicetteZooDettagli, WriteRicetteZooDettagli>();
            CreateMap<WriteRicetteZooDestinazioni, WriteRicetteZooDestinazioni>();
            CreateMap<WriteRicetteZooDettaglioTecnico, WriteRicetteZooDettaglioTecnico>();
            CreateMap<WriteTerapia, WriteTerapia>();
            CreateMap<WriteIntervento, WriteIntervento>();
            CreateMap<WriteInterventoxProtocollo, WriteInterventoxProtocollo>();
            #endregion
        }
    }

    public static class ApplicationMappingExtensions
    {
        public static IServiceCollection AddApplicationMappers(this IServiceCollection services, IConfiguration configuration, ILoggingBuilder logging, IWebHostEnvironment env)
        {
            services.AddAutoMapper(typeof(MappingProfile).Assembly);

            return services;
        }
    }
}
