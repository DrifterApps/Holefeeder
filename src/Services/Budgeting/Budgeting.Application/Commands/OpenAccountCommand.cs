using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

using DrifterApps.Holefeeder.Budgeting.Domain.Enumerations;
using DrifterApps.Holefeeder.Framework.SeedWork.Application;

using MediatR;

namespace DrifterApps.Holefeeder.Budgeting.Application.Commands
{
    [DataContract]
    public class OpenAccountCommand : IRequest<CommandResult<Guid>>
    {
        [DataMember, Required] 
        public AccountType Type { get; init; }
        
        [DataMember, Required]
        public string Name { get; init; }
        
        [DataMember, Required]
        public DateTime OpenDate { get; init; }
        
        [DataMember]
        public decimal OpenBalance { get; init; }
        
        [DataMember]
        public string Description { get; init; }
    }
}
