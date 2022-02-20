using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace LAB.DataScanner.Components.Services.Validators
{
    public static class SettingsValidator
    {
        public static void Validate<T>(T argument)
        {
            CheckForNull(argument, nameof(argument));

            var properties = argument.GetType().GetProperties()
                .Where(prop => Attribute.IsDefined(prop, typeof(RequiredAttribute))).ToArray();

            Array.ForEach(properties, propertyInfo =>
            {
                CheckForNull(propertyInfo.GetValue(argument), propertyInfo.Name);

                if (propertyInfo.PropertyType == typeof(string)) 
                {
                    CheckForStringEmpty(propertyInfo.GetValue(argument) as string, propertyInfo.Name);
                }
            });
        }


        private static void CheckForStringEmpty(string v, string name)
        {
            if (v.Equals(string.Empty)) 
            {
                throw new ArgumentException($"Value \"{name}\" was empty");
            }
        }

        private static void CheckForNull(object v, string name)
        {
            if (v is null || v.Equals(0) || v.Equals(false)) 
            {
                 throw new ArgumentNullException($"Value \"{name}\" was null");
            }
        }
    }
}
