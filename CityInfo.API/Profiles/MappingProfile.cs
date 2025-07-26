using System.Reflection;

using AutoMapper;

namespace CityInfo.API.Profiles
{
    public interface IMapFrom<TSrouce> where TSrouce : class
    {
        void Mapping(Profile profile) => profile.CreateMap(typeof(TSrouce), GetType()).ReverseMap();
    }

    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            Assembly[] assmeblies = [Assembly.GetExecutingAssembly()];

            foreach (var assembly in assmeblies)
            {
                ApplyMappingFromAssebmly(assembly);
            }
        }

        private void ApplyMappingFromAssebmly(Assembly assembly)
        {
            List<Type> types = assembly.GetExportedTypes()
                .Where(t => t.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IMapFrom<>)))
                .ToList();

            foreach (Type type in types)
            {
                // invoke the IMapFrom<>.Mapping method
                object? instance = Activator.CreateInstance(type);

                MethodInfo? mappingMethod = type.GetInterfaces()
                    .FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IMapFrom<>))
                    ?.GetMethod(nameof(IMapFrom<object>.Mapping));

                mappingMethod?.Invoke(instance, [this]);
            }
        }
    }
}
