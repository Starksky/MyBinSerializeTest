using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Serialization.Extensions;

namespace Serialization
{
    public class BinarySerialization
    {
        private static byte[] GetBytes(object obj)
        {
            if (obj is string s)
            {
                List<byte> data = new List<byte>();
                var result = s.GetBytes();
                data.AddRange(result.Length.GetBytes());
                data.AddRange(result);
                return data.ToArray();
            }
        
            if(obj is int i) 
                return i.GetBytes();
        
            if(obj is float f) 
                return f.GetBytes();
        
            if (obj is IDictionary dictionary)
            {
                List<byte> data = new List<byte>();
            
                data.AddRange(GetBytes(dictionary.Count));
            
                foreach (var item in dictionary)
                {
                    var property = item.GetType().GetProperty("Key");
                    var value = property.GetValue(item);
                    data.AddRange(GetBytes(value));
                
                    property = item.GetType().GetProperty("Value");
                    value = property.GetValue(item);
                    data.AddRange(GetBytes(value));
                }
            
                return data.ToArray(); 
            }
        
            if (obj is IList list)
            {
                List<byte> data = new List<byte>();
            
                data.AddRange(GetBytes(list.Count));
                foreach (var item in list)
                    data.AddRange(GetBytes(item));

                return data.ToArray(); 
            }

            if (obj.GetType().IsClass)
                return Serialization(obj);
        
            return default;
        }

        private static object GetValue(Type type, byte[] data, ref int offset)
        {
            if (type == typeof(string))
            {
                int count = IntExtension.GetValue(data, ref offset);
                return StringExtension.GetValue(data, ref offset, count);
            }
        
            if (type == typeof(int)) 
                return IntExtension.GetValue(data, ref offset);
        
            if (type == typeof(float)) 
                return FloatExtension.GetValue(data, ref offset);
        
            if(type.GetInterfaces().Any(i => i == typeof(IDictionary)))
            {
                Type[] arguments = type.GetGenericArguments();
                Type keyType = arguments[0];
                Type valueType = arguments[1];
            
                var result = type.Assembly.CreateInstance(type.FullName) as IDictionary;
                int count = (int)GetValue(typeof(int), data, ref offset);
            
                for (int i = 0; i < count; i++)
                {
                    var key = GetValue(keyType, data, ref offset);
                    var value = GetValue(valueType, data, ref offset);
                    result.Add(key, value);
                }
            
                return result;
            }

            if(type.GetInterfaces().Any(i => i == typeof(IList)))
            {
                Type[] arguments = type.GetGenericArguments();
                Type valueType = arguments[0];

                var result = type.Assembly.CreateInstance(type.FullName) as IList;
                int count = (int)GetValue(typeof(int), data, ref offset);
            
                for (int i = 0; i < count; i++)
                {
                    var value = GetValue(valueType, data, ref offset);
                    result.Add(value);
                }
            
                return result;
            }
        
            if (type.IsClass)
                return Deserialization(type, data, ref offset);

            return default;
        }

        public static byte[] Serialization(object obj)
        {
            Type type = obj.GetType();
            if (type.GetCustomAttribute(typeof(SerializableAttribute), true) == null) return default;

            if (obj is IDictionary || obj is IList)
            {
                var value = GetBytes(obj);
                if (value != null) return value;
            }
        

            List<byte> result = new List<byte>();
        
            foreach (var field in type.GetFields())
            {
                if ( field.FieldType.GetCustomAttribute(typeof(SerializableAttribute), true) == null ||
                     field.FieldType.GetCustomAttribute(typeof(NonSerializedAttribute), true) != null)
                    continue;

                var data = GetBytes(field.GetValue(obj));
                result.AddRange(data);
            }
        
            foreach (var property in type.GetProperties())
            {
                if ( property.PropertyType.GetCustomAttribute(typeof(SerializableAttribute), true) == null || 
                     property.PropertyType.GetCustomAttribute(typeof(NonSerializedAttribute), true) != null)
                    continue;
            
                var data = GetBytes(property.GetValue(obj));
                result.AddRange(data);
            }
        
            return result.ToArray();
        }
    
        private static object Deserialization(Type type, byte[] data, ref int offset)
        {
            if (type.GetCustomAttribute(typeof(SerializableAttribute), true) == null) return default;

            if (type.GetInterfaces().Any(i => i == typeof(IList) || i == typeof(IDictionary)))
                return GetValue(type, data, ref offset);

            object result = type.Assembly.CreateInstance(type.FullName);

            foreach (var field in type.GetFields())
            {
                if ( field.FieldType.GetCustomAttribute(typeof(SerializableAttribute), true) == null ||
                     field.FieldType.GetCustomAttribute(typeof(NonSerializedAttribute), true) != null)
                    continue;

                field.SetValue(result, GetValue(field.FieldType, data, ref offset));
            }
        
            foreach (var property in type.GetProperties())
            {
                if ( property.PropertyType.GetCustomAttribute(typeof(SerializableAttribute), true) == null || 
                     property.PropertyType.GetCustomAttribute(typeof(NonSerializedAttribute), true) != null)
                    continue;

                property.SetValue(result, GetValue(property.PropertyType, data, ref offset));
            }
        
            return result;
        }
    
        public static T Deserialization<T>(byte[] data) where T: class, new()
        {
            Type type = typeof(T);
            int offset = 0;
            return Deserialization(type, data, ref offset) as T;
        }
    }
}