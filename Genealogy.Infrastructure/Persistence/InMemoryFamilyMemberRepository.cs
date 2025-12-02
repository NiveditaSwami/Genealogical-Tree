using Genealogy.Application.Config;
using Genealogy.Domain.Repositories;
using Genealogy.Domain.Entities;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genealogy.Infrastructure.Persistence
{
    /// <summary>
    /// Inmemory Repository for Family Members
    /// </summary>
    public class InMemoryFamilyMemberRepository : IFamilyMemberRepository
    {
        private readonly PersistenceOptions _options;
        private readonly List<FamilyMember> _members = new();

        public InMemoryFamilyMemberRepository(IOptions<PersistenceOptions> options)
        {
            _options = options.Value; 
        }

        public FamilyMember Add(FamilyMember member)
        {
            member.Id = Guid.NewGuid();
            _members.Add(member);
            return member;
        }

        public IEnumerable<FamilyMember> GetAll() => _members;

        public FamilyMember? GetById(Guid id) => _members.FirstOrDefault(m => m.Id == id);

        public FamilyMember? GetByName(string name) => _members.FirstOrDefault(m => m.Name == name);

        public void Update(FamilyMember member)
        {
            var index = _members.FindIndex(m => m.Id == member.Id);
            // if  
            if (index >= 0)
                _members[index] = member;
        }
    }
}
