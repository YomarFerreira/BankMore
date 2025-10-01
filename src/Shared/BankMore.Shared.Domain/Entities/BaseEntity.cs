using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace BankMore.Shared.Domain.Entities
{
    public abstract class BaseEntity
    {
        [Key]
        public string Id { get; protected set; } = Guid.NewGuid().ToString();
        public DateTime DataCriacao { get; protected set; } = DateTime.Now;
    }
}