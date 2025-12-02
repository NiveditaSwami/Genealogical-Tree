using Genealogy.Application.Models;
using Genealogy.Application.Services;
using Genealogy.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace Genealogy.Application.UseCases
{
    /// <summary>
    /// Add a family member with relationship to another member
    /// </summary>
    public class AddFamilyMember
    {
        private readonly IFamilyMemberService _service;
        private readonly ILogger<AddFamilyMember> _logger;

        public AddFamilyMember(
           IFamilyMemberService service,
           ILogger<AddFamilyMember> logger)
        {
            _service = service;
            _logger = logger;
        }

        public FamilyMemberDTO Execute(AddMemberDTO member)
        {
            // Validate input

            if(member.Member1.Trim().Length <= 0 || member.Member2.Trim().Length <= 0)
            {
                _logger.LogWarning("Member names cannot be empty.");
                throw new ArgumentException("Member names cannot be empty.");
            }

            if(member.Relationship.Trim().Length <= 0)
            {
                _logger.LogWarning("Relationship cannot be empty.");
                throw new ArgumentException("Relationship cannot be empty.");
            }

            return _service.AddFamilyMember(member);
            
        }        
    }
}
