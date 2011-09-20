using System;
using System.Collections.Generic;
using System.Linq;
using Luna.Common;
using Luna.Globalization;
using Luna.Shifts.Domain;
using System.Collections;
using Luna.Core.Extensions;

namespace Luna.Infrastructure.Domain.Impl
{

    public class EmployeePropertyFilter : ICustomFilter
    {
        public EmployeePropertyFilter(object model)
        {
            Model = model;
        }

        public string ResourceKey { get; set; }

        public object Model { get; set; }

        public MulticastDelegate WhereClause { get; set; }

        public virtual void BeforeQuery() { }

        public virtual void Dispose()
        {
            Model = null;
            WhereClause = null;
        }
    }

    public class TimeBoxFilter : EmployeePropertyFilter
    {
        public TimeBoxFilter(object model, IList<string> types)
            : base(model)
        {
            ResourceKey = "ShiftNameSearching";

            Types = types;

            WhereClause = new Func<IAgent, DateTime, bool>((agent, date) =>
            {
                if (SelectedTypes==null || SelectedTypes.Count == 0)
                    return false;

                var found = agent.Schedule.TermSet.Where(o => o.Start.Date == date)
                    .Any(o =>
                            {
                                var matched = false;
                                foreach (string t in SelectedTypes)
                                {
                                    if (t == o.Text)
                                    {
                                        matched = true;
                                        break;
                                    }
                                }
                                return matched;
                            });
                return found;
            });
        }

        public override void BeforeQuery()
        {
            //_selectedTypes = Types.Cast<ISelectable>().Where(o => o.IsSelected == true).Cast<AssignmentType>().ToArray();
        }

        //private AssignmentType[] _selectedTypes;

        public IList<string> Types { get; set; }

        public IList SelectedTypes { get; set; }
    }

    public class AgentOnlyFilter : EmployeePropertyFilter
    {
        public AgentOnlyFilter(object model)
            : base(model)
        {
            ResourceKey = "AgentOnly";

            WhereClause = new Func<Employee, bool>(o =>
                                                   {
                                                       return o.IsAgent;
                                                   });
        }
    }

    public class EqEmployeeTypeFilter : EmployeePropertyFilter
    {
        public EqEmployeeTypeFilter(string propertyName, object model)
            : base(model)
        {
            var pInfo = typeof(Employee).GetProperty(propertyName);

            ResourceKey = string.Format("Eq{0}", propertyName);

            WhereClause = new Func<Employee, bool>(o =>
                                                       {
                                                           var value = pInfo.GetValue(o, null);
                                                           return value.ToString() == SelectedType.ToString();
                                                       });
        }

        public object SelectedType { get; set; }
    }

    public class SkillMatchingFilter : EmployeePropertyFilter
    {
        public SkillMatchingFilter(IEnumerable<Skill> skills, object model)
            : base(model)
        {
            ResourceKey = "SkillMatching";

            WhereClause = new Func<Employee, bool>(o =>
            {
                if (SelectedSkill == null) return true;
                var contains = o.Skills.Keys.Any(k => k.Id == SelectedSkill.Id);
                return contains;
            });
            Items = skills;
            SelectedSkill = Items.FirstOrDefault();
        }

        public Skill SelectedSkill { get; set; }

        public IEnumerable<Skill> Items { get; set; }
    }

    public class OrganizationMatchingFilter : EmployeePropertyFilter
    {
        public OrganizationMatchingFilter(IEnumerable<Organization> organizations, object model)
            : base(model)
        {
            ResourceKey = "OrganizationMatching";

            WhereClause = new Func<Employee, bool>(o =>
            {
                return o.Organization.Equals(SelectedOrganization);
            });
            Items = organizations;
            SelectedOrganization = Items.FirstOrDefault();
        }

        public Organization SelectedOrganization { get; set; }

        public IEnumerable<Organization> Items { get; set; }
    }

    public class InRankFilter : EmployeePropertyFilter
    {
        public InRankFilter(object model)
            : base(model)
        {
            ResourceKey = "InRank";

            WhereClause = new Func<Employee, bool>(o =>
            {
                return o.Rank >= Min && o.Rank <= Max;
            });
            Min = 1;
            Max = 10;
        }

        public int Min { get; set; }

        public int Max { get; set; }
    }

    public class InEnrollDateFilter : EmployeePropertyFilter
    {
        public InEnrollDateFilter(object model)
            : base(model)
        {
            ResourceKey = "InEnrollDate";

            WhereClause = new Func<Employee, bool>(o =>
            {
                return o.EnrollmentDate >= Min && o.EnrollmentDate <= Max;
            });
            Min = DateTime.Today.AddYears(-1);
            Max = DateTime.Today;
        }

        public DateTime Min { get; set; }

        public DateTime Max { get; set; }
    }

    public class EmployeeProperty : ISelectable
    {
        private static IDictionary<string, Func<Employee, string, bool>> _propertyClause;

        static EmployeeProperty()
        {
            _propertyClause = new Dictionary<string, Func<Employee, string, bool>> {
                {"Infrastructure_AgentFinder_Restriction_Name", (o, text)=> { return !string.IsNullOrEmpty(o.Name) && o.Name.Contains(text); } },
                {"Infrastructure_AgentFinder_Restriction_AgentId", (o, text)=> { return !string.IsNullOrEmpty(o.AgentId) && o.AgentId.Contains(text); } },
                {"Infrastructure_AgentFinder_Restriction_AgentAcdid", (o, text)=> { 

                    return o.AgentAcdids.Any(i=> i.Contains(text));
                } },
                {"Infrastructure_AgentFinder_Restriction_BornAddress", (o, text)=>
                                                                           {
                                                                               return !string.IsNullOrEmpty(o.Address2) && o.Address2.Contains(text);
                                                                           }
                    } , 
                {"Infrastructure_AgentFinder_Restriction_HomeAddress", (o, text)=> !string.IsNullOrEmpty(o.Address1) && o.Address1.Contains(text) } , 
                {"Infrastructure_AgentFinder_Restriction_Language", (o, text)=> !string.IsNullOrEmpty(o.Language) && o.Language.Contains(text) }};
        }

        public EmployeeProperty(string name, bool isSelected)
        {
            Name = LanguageReader.GetValue(name);
            IsSelected = isSelected;

            WhereClause = _propertyClause.ContainsKey(name) ? _propertyClause[name] : (o, text) => true;
        }

        public string Name { get; set; }

        public bool? IsSelected { get; set; }

        public Func<Employee, string, bool> WhereClause { get; private set; }

        public override string ToString()
        {
            return Name;
        }
    }



    public class FieldRestrictionFilter : EmployeePropertyFilter
    {
        public FieldRestrictionFilter(object model)
            : base(model)
        {
            ResourceKey = "FieldRestriction";

            var fields = new[] { "Infrastructure_AgentFinder_Restriction_Name", 
                "Infrastructure_AgentFinder_Restriction_AgentAcdid", 
                "Infrastructure_AgentFinder_Restriction_AgentId",
            "Infrastructure_AgentFinder_Restriction_BornAddress",
            "Infrastructure_AgentFinder_Restriction_HomeAddress", 
            "Infrastructure_AgentFinder_Restriction_Language"};

            InputValue = string.Empty;
            Fields = fields.Select(o => new EmployeeProperty(o, false)).ToArray();

            WhereClause = new Func<Employee, bool>(o =>
            {
                var matched = _selectedFields.Any(p => p.WhereClause(o, InputValue));

                return matched;
            });


        }

        public override void BeforeQuery()
        {
            _selectedFields = Fields.Where(f => f.IsSelected == true).ToArray();
        }

        private EmployeeProperty[] _selectedFields;

        public IEnumerable<EmployeeProperty> Fields { get; set; }

        public string InputValue { get; set; }
    }
}
