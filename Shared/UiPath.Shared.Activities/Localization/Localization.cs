using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using Smarthis.UiPathLib.Activities.Properties;

namespace UiPath.Shared.Activities.Localization;

[AttributeUsage(AttributeTargets.Property)]
public class LocalizedCategoryAttribute : CategoryAttribute
{
   public LocalizedCategoryAttribute(string category) : base(category)
   {
   }

   protected override string GetLocalizedString(string value) => SharedResources.ResourceManager.GetString(value) ?? base.GetLocalizedString(value);
}

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Class)]
public class LocalizedDisplayNameAttribute : DisplayNameAttribute
{
   public LocalizedDisplayNameAttribute(string displayName) : base(displayName)
   {
   }

   public override string DisplayName => SharedResources.ResourceManager.GetString(DisplayNameValue) ?? base.DisplayName;
}

public class LocalizedDescriptionAttribute : DescriptionAttribute
{
   public LocalizedDescriptionAttribute(string displayName) : base(displayName)
   {
   }

   public override string Description => SharedResources.ResourceManager.GetString(DescriptionValue) ?? base.Description;
}
public class LocalizedEnum
{
    public string Name { get; private set; }
    public Enum Value { get; private set; }

    protected internal LocalizedEnum(string name, Enum value)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentNullException(nameof(name));
        }
        if (value == null)
        {
            throw new ArgumentNullException(nameof(value));
        }

        Name = name;
        Value = value;
    }
}

public class LocalizedEnum<T> : LocalizedEnum
{
    protected LocalizedEnum(string name, Enum value) : base(name, value)
    {
    }

    public static List<LocalizedEnum> GetLocalizedValues()
    {
        List<LocalizedEnum> localizedValues = new List<LocalizedEnum>();

        Type enumType = typeof(T);
        Array enumValues = Enum.GetValues(enumType);

        foreach (Enum value in enumValues)
        {
            string name = enumType.GetEnumName(value);
            FieldInfo field = enumType.GetField(name);
            DescriptionAttribute descriptionAttribute = field?.GetCustomAttribute<DescriptionAttribute>();

            localizedValues.Add(new LocalizedEnum(descriptionAttribute?.Description ?? name, value));
        }

        return localizedValues;
    }
}