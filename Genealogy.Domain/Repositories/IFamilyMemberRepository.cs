using Genealogy.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genealogy.Domain.Repositories
{
    /// <summary>
    /// Used to manage family member data
    /// </summary>
    public interface IFamilyMemberRepository
    {
        FamilyMember Add(FamilyMember member);
        FamilyMember? GetById(Guid id);
        FamilyMember? GetByName(string name);
        IEnumerable<FamilyMember> GetAll();
        void Update(FamilyMember member);
    }
}
