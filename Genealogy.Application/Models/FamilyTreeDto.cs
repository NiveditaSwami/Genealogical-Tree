using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genealogy.Application.Models
{
    public class FamilyTreeDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public List<string> Parents { get; set; } = new();
        public List<string> Siblings { get; set; } = new();
        public List<FamilyTreeDto> Children { get; set; } = new();
        public List<FamilyTreeDto> GrandChildren { get; set; } = new();
    }
}
