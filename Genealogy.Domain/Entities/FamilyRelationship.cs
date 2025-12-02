using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genealogy.Domain.Entities
{
    /// <summary>
    /// TODO: This entity can be expanded to include more details about the relationship
    /// Represent a relationship between two family members
    /// </summary>
    public class FamilyRelationship
    {        
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid FromMemberId { get; set; }
        public Guid ToMemberId { get; set; }
        public RelationshipType Type { get; set; }
    }

    public enum RelationshipType
    {
        Parent,
        Child,
        Spouse,
        Sibling,
        Other   
    }
}
