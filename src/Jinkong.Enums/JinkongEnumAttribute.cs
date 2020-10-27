using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.DependencyInjection;

namespace Jinkong.Enums
{
    /// <summary>
    /// 枚举值验证
    /// </summary>
    public class JinkongEnumAttribute : ValidationAttribute
    {
        public JinkongEnumAttribute(string name)
        {
            Name = name;
        }

        /// <summary>
        /// 枚举名称
        /// </summary>
        public string Name { get; }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null)
                return ValidationResult.Success;

            if (value.GetType() != typeof(int))
                return ValidationResult.Success;

            var enumService = validationContext.GetRequiredService<IEnumService>();
            if (enumService.IsValid(Name, (int) value))
                return ValidationResult.Success;

            return new ValidationResult(ErrorMessage ??
                                        $"{validationContext.DisplayName ?? validationContext.MemberName} invalid enum value.");
        }
    }
}