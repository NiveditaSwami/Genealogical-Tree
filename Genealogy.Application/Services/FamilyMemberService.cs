
using Genealogy.Application.Config;
using Genealogy.Application.Models;
using Genealogy.Domain.Repositories;
using Genealogy.Domain.Entities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Genealogy.Application.Services
{
    /// <summary>
    /// Used to manage family member : Adding relationships and Retrieval of family tree
    /// </summary>
    public class FamilyMemberService : IFamilyMemberService
    {
        private readonly IFamilyMemberRepository _repository;
        private readonly ILogger<FamilyMemberService> _logger;
        private readonly PersistenceOptions _options;

        private readonly List<FamilyMember> _people = new();

        public FamilyMemberService(IFamilyMemberRepository repository,
            ILogger<FamilyMemberService> logger,
            IOptions<PersistenceOptions> options)
        {
            _repository = repository;
            _logger = logger;
            _options = options.Value;
        }

        /// <summary>
        /// Used to add a relationship between two members
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public FamilyMemberDTO AddFamilyMember(AddMemberDTO dto)
        {
            if (dto is null) throw new ArgumentNullException(nameof(dto));
            if (string.IsNullOrWhiteSpace(dto.Member1) || string.IsNullOrWhiteSpace(dto.Member2))
                throw new ArgumentException("Both member names are required.");

            // Relationship parsing
            var relationshipCodes = dto.Relationship.ToUpperInvariant().ToCharArray();
            var finalRelation = relationshipCodes.Last();
            var relationDepth = relationshipCodes.Length;

            // Fetch or create both members
            var member1 = GetOrCreateMember(dto.Member1);
            var member2 = GetOrCreateMember(dto.Member2);

            // Navigate intermediate steps (e.g., CCP means child of child’s parent)
            var targetMember = NavigateRelation(member1, relationshipCodes, relationDepth - 1);

            // Apply final relationship
            ApplyRelationship(targetMember, member2, finalRelation);

            // Update repository and sibling structure
            _repository.Update(targetMember);
            _repository.Update(member2);
            BuildSiblings();

            return new FamilyMemberDTO { Id = targetMember.Id, Name = targetMember.Name };
        }

        /// <summary>
        /// Get the family tree of a member
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public FamilyTreeDto GetFamilyTree(string name)
        {
            var member = _repository.GetByName(name);

            if (member == null)
            {
                throw new ArgumentException("Member not found");
                //log error
            }
            return BuildTree(member);
        }

        /// <summary>
        /// Fetch an existing member or create a new one if not found
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private FamilyMember GetOrCreateMember(string name)
        {
            var member = _repository.GetAll().FirstOrDefault(m => m.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
            if (member == null)
            {
                member = new FamilyMember { Name = name };
                _repository.Add(member);
                _logger.LogInformation("Added new member: {Name}", name);
            }
            return member;
        }

        /// <summary>
        /// Navigate through relationships based on the provided relationship codes
        /// </summary>
        /// <param name="start"></param>
        /// <param name="relCodes"></param>
        /// <param name="steps"></param>
        /// <returns></returns>
        private FamilyMember NavigateRelation(FamilyMember start, char[] relCodes, int steps)
        {
            var current = start;

            for (int i = 0; i < steps; i++)
            {
                if (relCodes.Length <= i) break;

                var code = relCodes[i];
                var nextId = code switch
                {
                    'C' => current.ChildrenIds.FirstOrDefault(),
                    'S' => current.SiblingIds.FirstOrDefault(),
                    'P' => current.ParentIds.FirstOrDefault(),
                    _ => Guid.Empty
                };
                
                if (nextId == Guid.Empty)
                {
                    var newMember = GetOrCreateMember(current.Name + "'s Parent");
                    nextId = newMember.Id;
                    ApplyRelationship(start, newMember, code);
                    current = newMember;
                    _logger.LogWarning("Cannot navigate: created anonymous member {Code}", code);
                    break;
                }

                current = _repository.GetById(nextId) ?? current;
            }

            return current;
        }

        /// <summary>
        /// Apply the final relationship between two members
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="relation"></param>
        private void ApplyRelationship(FamilyMember from, FamilyMember to, char relation)
        {
            switch (relation)
            {
                case 'C': // from → parent, to → child
                    AddUnique(from.ChildrenIds, to.Id);
                    AddUnique(to.ParentIds, from.Id);
                    break;

                case 'P': // from → child, to → parent
                    AddUnique(from.ParentIds, to.Id);
                    AddUnique(to.ChildrenIds, from.Id);
                    break;

                case 'S': // sibling
                    AddUnique(from.SiblingIds, to.Id);
                    AddUnique(to.SiblingIds, from.Id);
                    break;

                default:
                    _logger.LogWarning("Unknown relationship code: {Code}", relation);
                    break;
            }
        }

        private static void AddUnique(List<Guid> list, Guid id)
        {
            if (!list.Contains(id))
                list.Add(id);
        }

        /// <summary>
        /// Recursively build the family tree DTO
        /// </summary>
        /// <param name="member"></param>
        /// <returns></returns>
        private FamilyTreeDto BuildTree(FamilyMember member)
         {
            var memberDto = new FamilyTreeDto
            {
                Id = member.Id,
                Name = member.Name
            };
            // Parents
            foreach (var pid in member.ParentIds)
            {
                var p = _repository.GetById(pid);
                if (p != null) memberDto.Parents.Add(p.Name);
            }
            // Siblings
            foreach (var sibId in member.SiblingIds)
            {
                var s = _repository.GetById(sibId);
                if (s != null) memberDto.Siblings.Add(s.Name);
            }

            // Children and grandchildren
            foreach (var childId in member.ChildrenIds)
            {
                var child = _repository.GetById(childId);
                if (child != null)
                {
                    var childDto = BuildTree(child);
                    memberDto.Children.Add(childDto);

                    foreach (var gchild in child.ChildrenIds)
                    {
                        var gc = _repository.GetById(gchild);
                        if (gc != null)
                            memberDto.GrandChildren.Add(new FamilyTreeDto { Id = gc.Id, Name = gc.Name });
                    }
                }
            }

            return memberDto;
        }

        /// <summary>
        /// Build sibling relationships based on shared parents
        /// </summary>
        private void BuildSiblings()
        {
            var all = _repository.GetAll().ToList();
            foreach (var member in all)
            {
                foreach (var parentId in member.ParentIds)
                {
                    var parent = _repository.GetById(parentId);
                    if (parent == null) continue;

                    var siblings = parent.ChildrenIds.Where(id => id != member.Id);
                    foreach (var s in siblings)
                        if (!member.SiblingIds.Contains(s))
                            member.SiblingIds.Add(s);
                }
                _repository.Update(member);
            }
        }
    }
}
