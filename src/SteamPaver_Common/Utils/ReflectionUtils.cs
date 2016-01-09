using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.Serialization;
using System.Threading;


namespace SteamPaver.Common
{
    public static class ReflexionUtils
    {


        public static T GetAtribute<T>(this ICustomAttributeProvider prov)
        {
            return GetAtributes<T>(prov).FirstOrDefault();
        }

        public static IEnumerable<T> GetAtributes<T>(this ICustomAttributeProvider prov)
        {
            if (prov == null)
                return new T[0];

            var atr = prov.GetCustomAttributes(typeof(T), true).Cast<T>();
            return atr;
        }


        public static T GetAtribute<T>(this Enum enumValue)
        {
            return GetAtributes<T>(enumValue).FirstOrDefault();
        }

        public static IEnumerable<T> GetAtributes<T>(this Enum enumValue)
        {
            if (enumValue == null)
                return new T[0];

            var type = enumValue.GetType();
            var memInfo = type.GetMember(enumValue.ToString());

            if (memInfo.Length == 0)
                return new T[0];

            return memInfo[0].GetCustomAttributes(typeof(T), false).Cast<T>();

        }
        

        public static string GetDescription<TClass, TPropertie>(Expression<Func<TClass, TPropertie>> exp)
        {
            var prop = PropertyOf(exp);

            return prop.ToDescriptionString();
        }

        public static string GetDescription<TPropertie>(Expression<Func<TPropertie>> exp)
        {
            var prop = PropertyOf(exp);

            return prop.ToDescriptionString();
        }


        public static string GetName<TClass, TPropertie>(Expression<Func<TClass, TPropertie>> exp)
        {
            var prop = PropertyOf(exp);

            return prop.ToDisplayString();
        }


        public static string GetShortName<TClass, TPropertie>(Expression<Func<TClass, TPropertie>> exp)
        {
            var prop = PropertyOf(exp);

            return prop.ToShortNameString();
        }


        public static string GetName<TPropertie>(Expression<Func<TPropertie>> exp)
        {
            var prop = PropertyOf(exp);

            return prop.ToDisplayString();
        }

        public static string GetShortName<TPropertie>(Expression<Func<TPropertie>> exp)
        {
            var prop = PropertyOf(exp);

            return prop.ToShortNameString();
        }

        public static string ToDescriptionString(this Enum cat)
        {
            var atrDisp = ReflexionUtils.GetAtribute<DisplayAttribute>(cat);
            if (atrDisp != null)
            {
                return atrDisp.GetDescription();
            }

            var atrDesc = GetAtribute<DescriptionAttribute>(cat);
            if (atrDesc != null)
                return atrDesc.Description;

            return "-";
        }
        public static string ToDescriptionString(this ICustomAttributeProvider cat)
        {
            var atrDisp = ReflexionUtils.GetAtribute<DisplayAttribute>(cat);
            if (atrDisp != null)
            {
                return atrDisp.GetDescription();
            }

            var atrDesc = GetAtribute<DescriptionAttribute>(cat);
            if (atrDesc != null)
                return atrDesc.Description;

            return "-";
        }

        public static string ToDisplayString(this ICustomAttributeProvider cat)
        {
            var atrDisp = ReflexionUtils.GetAtribute<DisplayAttribute>(cat);
            if (atrDisp != null)
            {
                return atrDisp.GetName();
            }

            var atrDispName = GetAtribute<DisplayNameAttribute>(cat);
            if (atrDispName != null)
                return atrDispName.DisplayName;

            if (cat is PropertyInfo)
                return (cat as PropertyInfo).Name;

            if (cat is MemberInfo)
                return (cat as MemberInfo).Name;

            if (cat is FieldInfo)
                return (cat as FieldInfo).Name;

            if (cat is Type)
                return (cat as Type).Name;

            return cat.ToString();
        }

        public static string ToDisplayString(this Enum cat)
        {
            var atrDisp = ReflexionUtils.GetAtribute<DisplayAttribute>(cat);
            if (atrDisp != null)
            {
                return atrDisp.GetName();
            }

            var atrDispName = GetAtribute<DisplayNameAttribute>(cat);
            if (atrDispName != null)
                return atrDispName.DisplayName;

            return cat.ToString();
        }

        public static string ToShortNameString(this ICustomAttributeProvider cat)
        {
            var atrDisp = ReflexionUtils.GetAtribute<DisplayAttribute>(cat);
            if (atrDisp != null)
            {
                return atrDisp.GetShortName();
            }

            if (cat is PropertyInfo)
                return (cat as PropertyInfo).Name;

            if (cat is MemberInfo)
                return (cat as MemberInfo).Name;

            if (cat is FieldInfo)
                return (cat as FieldInfo).Name;

            if (cat is Type)
                return (cat as Type).Name;

            return cat.ToString();
        }

        public static string ToShortNameString(this Enum cat)
        {
            var atrDisp = ReflexionUtils.GetAtribute<DisplayAttribute>(cat);
            if (atrDisp != null)
            {
                return atrDisp.GetShortName();
            }

            return cat.ToString();
        }

        public static PropertyInfo PropertyOf<TInstance, TProperty>(Expression<Func<TInstance, TProperty>> propertyGetExpression)
        {
            if (propertyGetExpression == null)
                return null;
            return ((MemberExpression)propertyGetExpression.Body).Member as PropertyInfo;
        }


        public static PropertyInfo PropertyOf<TProperty>(Expression<Func<TProperty>> propertyGetExpression)
        {
            if (propertyGetExpression == null)
                return null;
            var res = ((MemberExpression)propertyGetExpression.Body).Member as PropertyInfo;
            return res;
        }
    }
}