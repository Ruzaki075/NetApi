using System.ComponentModel.DataAnnotations;

namespace Api.Validators
{
    /// <summary>
    /// Атрибут валидации для проверки, что дата окончания позже даты начала
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class DateRangeAttribute : ValidationAttribute
    {
        private readonly string _startDatePropertyName;
        private readonly string _endDatePropertyName;

        /// <summary>
        /// Конструктор атрибута валидации диапазона дат
        /// </summary>
        /// <param name="startDatePropertyName">Имя свойства с датой начала</param>
        /// <param name="endDatePropertyName">Имя свойства с датой окончания</param>
        public DateRangeAttribute(string startDatePropertyName, string endDatePropertyName)
        {
            _startDatePropertyName = startDatePropertyName;
            _endDatePropertyName = endDatePropertyName;
        }

        /// <summary>
        /// Выполняет валидацию объекта
        /// </summary>
        /// <param name="value">Объект для валидации</param>
        /// <param name="validationContext">Контекст валидации</param>
        /// <returns>Результат валидации</returns>
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value == null)
            {
                return ValidationResult.Success;
            }

            var startDateProperty = validationContext.ObjectType.GetProperty(_startDatePropertyName);
            var endDateProperty = validationContext.ObjectType.GetProperty(_endDatePropertyName);

            if (startDateProperty == null || endDateProperty == null)
            {
                return new ValidationResult($"Свойства {_startDatePropertyName} или {_endDatePropertyName} не найдены.");
            }

            var startDateValue = startDateProperty.GetValue(value);
            var endDateValue = endDateProperty.GetValue(value);

            if (startDateValue is DateTime startDate && endDateValue is DateTime endDate)
            {
                if (endDate <= startDate)
                {
                    return new ValidationResult("Дата окончания должна быть позже даты начала.");
                }

                if (startDate < DateTime.Today)
                {
                    return new ValidationResult("Дата начала не может быть в прошлом.");
                }
            }

            return ValidationResult.Success;
        }
    }
}

