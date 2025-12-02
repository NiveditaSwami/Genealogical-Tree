using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genealogy.Application.Models
{
    /// <summary>
    /// DTO for returning family member information
    /// </summary>
    public class FamilyMemberDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }

    /// <summary>
    /// DTO for adding a new family member with relationship
    /// </summary>
    public class AddMemberDTO
    {
        public required string Member1 { get; set; }
        public required string Member2 { get; set; }
        public required string Relationship { get; set; }
    }

}
