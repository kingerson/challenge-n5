namespace MsClean.Application;
using System;
using System.Collections.Generic;
using System.ComponentModel;

public static class Helper
{
    public static Dictionary<string, object> ConvertToDictionary(object obj)
    {
        var dict = new Dictionary<string, object>();

        foreach (var property in obj.GetType().GetProperties())
        {
            if (property.CanRead)
                dict.Add(property.Name, property.GetValue(obj));
        }
        return dict;
    }

    public static string GetDescription(this Enum value)
    {
        var fieldInfo = value.GetType().GetField(value.ToString());

        if (fieldInfo == null)
            return value.ToString();

        var attributes = (DescriptionAttribute[])fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false);

        return attributes.Length > 0 ? attributes[0].Description : value.ToString();
    }
}
