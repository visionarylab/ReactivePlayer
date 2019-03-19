using System;

namespace ReactivePlayer.Exps.Domain.Models
{
    public class Employee : Person
    {
        public Employee(
            Guid id,
            string firstName,
            string lastName,
            DateTime birthDateTime,
            string companyName)
            : base(id, firstName, lastName, birthDateTime)
        {
            this.CompanyName = companyName.TrimmedOrNull() ?? throw new ArgumentNullException(nameof(companyName));
        }

        public string CompanyName { get; }
    }
}
