using Genealogy.Application.Models;
using Genealogy.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genealogy.Application.Services
{
    /// <summary>
    /// Manage family member : Adding relationships and Retrieval of family tree
    /// </summary>
    public interface IFamilyMemberService
    {
        FamilyMemberDTO AddFamilyMember(AddMemberDTO dto);
        FamilyTreeDto GetFamilyTree(string name);
    }
}
