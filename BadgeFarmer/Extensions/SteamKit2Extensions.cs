using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using SteamKit2;

namespace BadgeFarmer.Extensions
{
    public static class SteamKit2Extensions
    {
        public static IList<T> AsMany<T>(this IEnumerable<KeyValue> keyValues)
        {
            return keyValues.Select(x => x.Children.Any() ? x.Children.As<T>() : x.As<T>()).ToList();
        }

        public static T As<T>(this IEnumerable<KeyValue> keyValues)
        {
            var type = typeof(T);

            if (type.GetInterface("IEnumerable`1") != null ||
                type.IsConstructedGenericType &&
                typeof(IEnumerable<>).MakeGenericType(type.GenericTypeArguments[0]) == type)
            {
                var method = typeof(SteamKit2Extensions).GetMethod("AsMany")
                    .MakeGenericMethod(type.GenericTypeArguments[0]);
                return (T) method.Invoke(null, new[] {(object) keyValues});
            }

            var typeProps = type
                .GetProperties()
                .ToDictionary(x => x.Name,
                    x => x.GetCustomAttributesData()
                        .FirstOrDefault(x => x.AttributeType == typeof(JsonPropertyNameAttribute))
                        .ConstructorArguments
                        .First().Value as string
                );

            var typeCtorParams = type
                .GetConstructors()
                .First()
                .GetParameters()
                .ToDictionary(x => x.Position, x => new {name = x.Name, type = x.ParameterType});


            var methodSingle = typeof(SteamKit2Extensions).GetMethod("As", new[] {typeof(KeyValue)});
            var methodCollection = typeof(SteamKit2Extensions).GetMethod("As", new[] {typeof(IEnumerable<KeyValue>)});

            var parameters = new List<object>();

            for (int i = 0; i < typeCtorParams.Count; i++)
            {
                var par = typeCtorParams[i];
                var val = keyValues.FirstOrDefault(x => x.Name == typeProps[par.name]);
                object param = null;
                if (val != null)
                {
                    param = val.Children.Count > 0
                        ? methodCollection.MakeGenericMethod(par.type).Invoke(null, new[] {(object) val.Children})
                        : methodSingle.MakeGenericMethod(par.type).Invoke(null, new[] {(object) val});
                }

                parameters.Add(param);
            }

            var result = type.GetConstructors()[0].Invoke(parameters.ToArray());
            return (T) result;
        }

        public static T As<T>(this KeyValue keyValue)
        {
            var type = typeof(T);
            Type a;
            while ((a = Nullable.GetUnderlyingType(type)) != null)
                type = a;
            if (type == typeof(bool))
                return (dynamic) keyValue.AsBoolean();
            if (type == typeof(float))
                return (dynamic) keyValue.AsFloat();
            if (type == typeof(int))
                return (dynamic) keyValue.AsInteger();
            if (type == typeof(long))
                return (dynamic) keyValue.AsLong();
            if (type == typeof(string))
                return (dynamic) keyValue.AsString();
            if (type == typeof(byte))
                return (dynamic) keyValue.AsUnsignedByte();
            if (type == typeof(uint))
                return (dynamic) keyValue.AsUnsignedInteger();
            if (type == typeof(ulong))
                return (dynamic) keyValue.AsUnsignedLong();
            if (type == typeof(ushort))
                return (dynamic) keyValue.AsUnsignedShort();

            var methodCollection = typeof(SteamKit2Extensions).GetMethod("As", new[] {typeof(IEnumerable<KeyValue>)});

            if (keyValue.Children.Any())
            {
                var typeProps = type
                    .GetProperties()
                    .ToDictionary(x => x.Name,
                        x => x.GetCustomAttributesData()
                            .FirstOrDefault(x => x.AttributeType == typeof(JsonPropertyNameAttribute))
                            .ConstructorArguments
                            .First().Value as string
                    );

                var typeCtorParams = type
                    .GetConstructors()
                    .First()
                    .GetParameters()
                    .ToDictionary(x => x.Position, x => new {name = x.Name, type = x.ParameterType});

                var innerResult = methodCollection.MakeGenericMethod(typeCtorParams[0].type)
                    .Invoke(null, new[] {(object) keyValue.Children});


                var result = Activator.CreateInstance(type, innerResult);

                return (T) result;
            }

            throw new ArgumentException();
        }
    }
}