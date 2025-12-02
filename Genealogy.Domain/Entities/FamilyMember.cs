using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genealogy.Domain.Entities
{
    /// <summary>
    /// Represent a family member and their relationships
    /// </summary>
    public class FamilyMember
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public required string Name { get; set; }

        // Relationships (store only IDs)
        public List<Guid> ParentIds { get; set; } = new();
        public List<Guid> ChildrenIds { get; set; } = new();
        public List<Guid> SiblingIds { get; set; } = new();
    }
}
