using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;


public static class EnumExtensions
{
    public static string GetDisplayFormat(this Enum value)
    {
        FieldInfo fi = value.GetType().GetField(value.ToString());

        if (fi != null)
        {
            var att = (DisplayFormatAttribute)fi.GetCustomAttributes(typeof(DisplayFormatAttribute), true).FirstOrDefault();

            if (att != null)
            {
                return att.DataFormatString;
            }
        }

        return null;
    }

    public static string GetName(this Enum value)
    {
        FieldInfo fi = value.GetType().GetField(value.ToString());

        if (fi != null)
        {
            DisplayAttribute[] att = (DisplayAttribute[])fi.GetCustomAttributes(typeof(DisplayAttribute), false);

            if (att != null && att.Length > 0)
            {
                return att[0].Name;
            }
            else
            {
                return value.ToString();
            }
        }
        else
        {
            return value.ToString();
        }
    }


    public static string GetDescription(this Enum value)
    {
        FieldInfo fi = value.GetType().GetField(value.ToString());

        if (fi != null)
        {
            DescriptionAttribute[] att = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);

            if (att != null && att.Length > 0)
            {
                return att[0].Description;
            }
            else
            {
                return value.ToString();
            }
        }
        else
        {
            return value.ToString();
        }
    }

    public static T GetAttribute<T>(this Enum en)
        where T : class, new()
    {
        Type type = en.GetType();
        MemberInfo[] memInfo = type.GetMember(en.ToString());

        if (memInfo.Length > 0)
        {
            object[] attrs = memInfo[0].GetCustomAttributes(typeof(T), false);
            if (attrs.Length > 0)
                return ((T)attrs[0]);
        }
        return new T();
    }
}
