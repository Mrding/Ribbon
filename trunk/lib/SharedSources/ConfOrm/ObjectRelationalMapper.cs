using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using ConfOrm.Patterns;

namespace ConfOrm
{
	public class ObjectRelationalMapper : IObjectRelationalMapper, IDomainInspector
	{
		private readonly IExplicitDeclarationsHolder explicitDeclarations;

		// polymorphismSolutions is the cached result to avoid the usage of reflection all the time.
		private readonly Dictionary<Type, IEnumerable<Type>> polymorphismSolutions = new Dictionary<Type, IEnumerable<Type>>();

		public ObjectRelationalMapper()
		{
			explicitDeclarations = new ExplicitDeclarationsHolder();
			Patterns = new DefaultNHibernatePatternsHolder(this, explicitDeclarations);
		}

		public ObjectRelationalMapper(IPatternsHolder patterns): this(patterns, new ExplicitDeclarationsHolder())
		{
		}

		public ObjectRelationalMapper(IPatternsHolder patterns, IExplicitDeclarationsHolder explicitDeclarations)
		{
			if (patterns == null)
			{
				throw new ArgumentNullException("patterns");
			}
			if (explicitDeclarations == null)
			{
				throw new ArgumentNullException("explicitDeclarations");
			}
			Patterns = patterns;
			this.explicitDeclarations = explicitDeclarations;
		}

		public IPatternsHolder Patterns { get; protected set; }

		#region Implementation of IObjectRelationalMapper

		public virtual void TablePerClass(IEnumerable<Type> baseEntities)
		{
			foreach (var baseEntity in baseEntities)
			{
				RegisterTablePerClass(baseEntity);
			}
		}

		public virtual void TablePerClassHierarchy(IEnumerable<Type> baseEntities)
		{
			foreach (var baseEntity in baseEntities)
			{
				RegisterTablePerClassHierarchy(baseEntity);
			}
		}

		public virtual void TablePerConcreteClass(IEnumerable<Type> baseEntities)
		{
			foreach (var baseEntity in baseEntities)
			{
				RegisterTablePerConcreteClass(baseEntity);
			}
		}

		public virtual void TablePerClassHierarchy<TBaseEntity>() where TBaseEntity : class
		{
			var type = typeof(TBaseEntity);
			RegisterTablePerClassHierarchy(type);
		}

		protected void RegisterTablePerClassHierarchy(Type type)
		{
			AddToDomain(type);
			PreventComponentAsEntity(type);
			explicitDeclarations.RootEntities.Add(type);
			explicitDeclarations.TablePerClassHierarchyEntities.Add(type);
		}

		private void PreventComponentAsEntity(Type type)
		{
			if (explicitDeclarations.Components.Contains(type))
			{
				throw new MappingException("Ambiguous type registered as component and as entity; type:" + type);
			}
		}

		public virtual void TablePerClass<TBaseEntity>() where TBaseEntity : class
		{
			var type = typeof (TBaseEntity);
			RegisterTablePerClass(type);
		}

		protected void RegisterTablePerClass(Type type)
		{
			AddToDomain(type);
			PreventComponentAsEntity(type);
			explicitDeclarations.RootEntities.Add(type);
			explicitDeclarations.TablePerClassEntities.Add(type);
		}

		public virtual void TablePerConcreteClass<TBaseEntity>() where TBaseEntity : class
		{
			var type = typeof(TBaseEntity);
			RegisterTablePerConcreteClass(type);
		}

		protected void RegisterTablePerConcreteClass(Type type)
		{
			AddToDomain(type);
			PreventComponentAsEntity(type);
			explicitDeclarations.RootEntities.Add(type);
			explicitDeclarations.TablePerConcreteClassEntities.Add(type);
		}

		public virtual void Component<TComponent>()
		{
			var type = typeof(TComponent);
			if(IsEntity(type))
			{
				throw new MappingException("Ambiguous type registered as component and as entity; type:" + type);
			}
			explicitDeclarations.Components.Add(type);
		}

		public virtual void Complex<TComplex>()
		{
			var type = typeof(TComplex);
			Complex(type);
		}

		public void Complex(Type type)
		{
			if (type == null)
			{
				throw new ArgumentNullException("type");
			}
			explicitDeclarations.ComplexTypes.Add(type);
		}

		public void Exclude<TClass>()
		{
			Exclude(typeof (TClass));
		}

		public void Exclude(Type type)
		{
			if (type == null)
			{
				throw new ArgumentNullException("type");
			}
			if(explicitDeclarations.ClassExclusions.Add(type))
			{
				InvalidatePolymorphismSolutions();
			}
		}

		public virtual void Poid<TEntity>(Expression<Func<TEntity, object>> propertyGetter)
		{
			var member = TypeExtensions.DecodeMemberAccessExpression(propertyGetter);
			explicitDeclarations.Poids.Add(member);
		}

		public virtual void NaturalId<TBaseEntity>(params Expression<Func<TBaseEntity, object>>[] propertiesGetters) where TBaseEntity : class
		{
			if (!IsRootEntity(typeof(TBaseEntity)))
			{
				throw new ArgumentOutOfRangeException("TBaseEntity", "The entity class should be a root-entity; register the table-strategy first.");
			}
			foreach (var propertyGetter in propertiesGetters.Where(pg=> pg!=null))
			{
				var member = TypeExtensions.DecodeMemberAccessExpression(propertyGetter);
				explicitDeclarations.NaturalIds.Add(member);
			}
		}

		public virtual void ManyToMany<TLeftEntity, TRigthEntity>()
		{
			explicitDeclarations.ManyToManyRelations.Add(new Relation(typeof(TLeftEntity), typeof(TRigthEntity)));
			explicitDeclarations.ManyToManyRelations.Add(new Relation(typeof(TRigthEntity), typeof(TLeftEntity), Declared.Implicit));
		}

		public virtual void ManyToOne<TLeftEntity, TRigthEntity>()
		{
			explicitDeclarations.ManyToOneRelations.Add(new Relation(typeof(TLeftEntity), typeof(TRigthEntity)));
			explicitDeclarations.OneToManyRelations.Add(new Relation(typeof(TRigthEntity), typeof(TLeftEntity)));
		}

		public virtual void OneToOne<TLeftEntity, TRigthEntity>()
		{
			explicitDeclarations.OneToOneRelations.Add(new Relation(typeof(TLeftEntity), typeof(TRigthEntity)));
			explicitDeclarations.OneToOneRelations.Add(new Relation(typeof(TRigthEntity), typeof(TLeftEntity), Declared.Implicit));
		}

		public virtual void Set<TEntity>(Expression<Func<TEntity, object>> propertyGetter)
		{
			var member = TypeExtensions.DecodeMemberAccessExpression(propertyGetter);
			explicitDeclarations.Sets.Add(member);
			var memberOf = TypeExtensions.DecodeMemberAccessExpressionOf(propertyGetter);
			explicitDeclarations.Sets.Add(memberOf);
		}

		public virtual void Bag<TEntity>(Expression<Func<TEntity, object>> propertyGetter)
		{
			var member = TypeExtensions.DecodeMemberAccessExpression(propertyGetter);
			explicitDeclarations.Bags.Add(member);
			var memberOf = TypeExtensions.DecodeMemberAccessExpressionOf(propertyGetter);
			explicitDeclarations.Bags.Add(memberOf);
		}

		public virtual void List<TEntity>(Expression<Func<TEntity, object>> propertyGetter)
		{
			var member = TypeExtensions.DecodeMemberAccessExpression(propertyGetter);
			explicitDeclarations.Lists.Add(member);
			var memberOf = TypeExtensions.DecodeMemberAccessExpressionOf(propertyGetter);
			explicitDeclarations.Lists.Add(memberOf);
		}

		public virtual void Array<TEntity>(Expression<Func<TEntity, object>> propertyGetter)
		{
			var member = TypeExtensions.DecodeMemberAccessExpression(propertyGetter);
			explicitDeclarations.Arrays.Add(member);
			var memberOf = TypeExtensions.DecodeMemberAccessExpressionOf(propertyGetter);
			explicitDeclarations.Arrays.Add(memberOf);
		}

		public virtual void Dictionary<TEntity>(Expression<Func<TEntity, object>> propertyGetter)
		{
			var member = TypeExtensions.DecodeMemberAccessExpression(propertyGetter);
			explicitDeclarations.Dictionaries.Add(member);
			var memberOf = TypeExtensions.DecodeMemberAccessExpressionOf(propertyGetter);
			explicitDeclarations.Dictionaries.Add(memberOf);
		}

		public virtual void Complex<TEntity>(Expression<Func<TEntity, object>> propertyGetter)
		{
			var member = TypeExtensions.DecodeMemberAccessExpression(propertyGetter);
			explicitDeclarations.ComplexTypeMembers.Add(member);
			var memberOf = TypeExtensions.DecodeMemberAccessExpressionOf(propertyGetter);
			explicitDeclarations.ComplexTypeMembers.Add(memberOf);
		}

		public virtual void HeterogeneousAssociation<TEntity>(Expression<Func<TEntity, object>> propertyGetter)
		{
			var member = TypeExtensions.DecodeMemberAccessExpression(propertyGetter);
			explicitDeclarations.HeterogeneousAssociations.Add(member);
			var memberOf = TypeExtensions.DecodeMemberAccessExpressionOf(propertyGetter);
			explicitDeclarations.HeterogeneousAssociations.Add(memberOf);
		}

		public void Bidirectional<TEntity1, TEntity2>(Expression<Func<TEntity1, TEntity2>> propertyGetter1, Expression<Func<TEntity2, TEntity1>> propertyGetter2)
		{
			var member1 = TypeExtensions.DecodeMemberAccessExpression(propertyGetter1);
			var member2 = TypeExtensions.DecodeMemberAccessExpression(propertyGetter2);
			RegisterBidirectionalRelation<TEntity1, TEntity2>(member1, member2);
		}

		private void RegisterBidirectionalRelation<TEntity1, TEntity2>(MemberInfo member1, MemberInfo member2)
		{
			var relation1 = new RelationOn(typeof (TEntity1), member1, typeof (TEntity2));
			var relation2 = new RelationOn(typeof(TEntity2), member2, typeof(TEntity1));
			ThrowsWhereAmbiguous(relation1, member2);
			ThrowsWhereAmbiguous(relation2, member1);
			explicitDeclarations.BidirectionalMembers[relation1] = member2;
			explicitDeclarations.BidirectionalMembers[relation2] = member1;
		}

		private void ThrowsWhereAmbiguous(RelationOn relation, MemberInfo actualMember)
		{
			MemberInfo alreadyRegisteredMember;
			if(explicitDeclarations.BidirectionalMembers.TryGetValue(relation, out alreadyRegisteredMember))
			{
				if(!alreadyRegisteredMember.Equals(actualMember))
				{
					var message = new StringBuilder(500);
					message.AppendLine("Ambiguous registration of bidirectional relation on specific property:");
					message.AppendLine(string.Format("Was defined for '{0}.{1}' to '{2}.{3}' ", relation.From.FullName, relation.On.Name, relation.To.FullName, alreadyRegisteredMember.Name));
					message.AppendLine(string.Format("Then defined for '{0}.{1}' to '{2}.{3}' ", relation.From.FullName, relation.On.Name, relation.To.FullName, actualMember.Name));
					throw new MappingException(message.ToString());
				}
			}
		}

		public void Bidirectional<TEntity1, TEntity2>(Expression<Func<TEntity1, IEnumerable<TEntity2>>> propertyGetter1, Expression<Func<TEntity2, TEntity1>> propertyGetter2)
		{
			var member1 = TypeExtensions.DecodeMemberAccessExpression(propertyGetter1);
			var member2 = TypeExtensions.DecodeMemberAccessExpression(propertyGetter2);
			RegisterBidirectionalRelation<TEntity1, TEntity2>(member1, member2);
		}

		public void Bidirectional<TEntity1, TEntity2>(Expression<Func<TEntity1, TEntity2>> propertyGetter1, Expression<Func<TEntity2, IEnumerable<TEntity1>>> propertyGetter2)
		{
			var member1 = TypeExtensions.DecodeMemberAccessExpression(propertyGetter1);
			var member2 = TypeExtensions.DecodeMemberAccessExpression(propertyGetter2);
			RegisterBidirectionalRelation<TEntity1, TEntity2>(member1, member2);
		}

		public void Bidirectional<TEntity1, TEntity2>(Expression<Func<TEntity1, IEnumerable<TEntity2>>> propertyGetter1, Expression<Func<TEntity2, IEnumerable<TEntity1>>> propertyGetter2)
		{
			var member1 = TypeExtensions.DecodeMemberAccessExpression(propertyGetter1);
			var member2 = TypeExtensions.DecodeMemberAccessExpression(propertyGetter2);
			RegisterBidirectionalRelation<TEntity1, TEntity2>(member1, member2);
		}

		public virtual void Cascade<TFromEntity, TToEntity>(Cascade cascadeOptions)
		{
			explicitDeclarations.Cascades.Add(new Relation(typeof(TFromEntity), typeof(TToEntity)), cascadeOptions);
		}

		public virtual void PersistentProperty<TEntity>(Expression<Func<TEntity, object>> propertyGetter)
		{
			var member = TypeExtensions.DecodeMemberAccessExpressionOf(propertyGetter);
			var memberOf = TypeExtensions.DecodeMemberAccessExpressionOf(propertyGetter);
			explicitDeclarations.PersistentProperties.Add(member);
			explicitDeclarations.PersistentProperties.Add(memberOf);
		}

		public void ExcludeProperty<TEntity>(Expression<Func<TEntity, object>> propertyGetter)
		{
			var member = TypeExtensions.DecodeMemberAccessExpressionOf(propertyGetter);
			var memberOf = TypeExtensions.DecodeMemberAccessExpressionOf(propertyGetter);
			explicitDeclarations.ExclusionProperties.Add(member);
			explicitDeclarations.ExclusionProperties.Add(memberOf);
		}

		public virtual void VersionProperty<TEntity>(Expression<Func<TEntity, object>> propertyGetter)
		{
			var member = TypeExtensions.DecodeMemberAccessExpression(propertyGetter);
			explicitDeclarations.VersionProperties.Add(member);
			var memberOf = TypeExtensions.DecodeMemberAccessExpression(propertyGetter);
			explicitDeclarations.VersionProperties.Add(memberOf);
		}

		public void ExcludeProperty(Predicate<MemberInfo> matchForExclusion)
		{
			Patterns.PersistentPropertiesExclusions.Add(new DelegatedPattern<MemberInfo>(matchForExclusion));
		}

		#endregion

		#region Implementation of IDomainInspector

		public virtual bool IsRootEntity(Type type)
		{
			return explicitDeclarations.RootEntities.Contains(type) && !IsExplicitlyExcluded(type);
		}

		public virtual bool IsComponent(Type type)
		{
			return (explicitDeclarations.Components.Contains(type) || (Patterns.Components.Match(type) && !IsEntity(type)))
			       && !explicitDeclarations.ComplexTypes.Contains(type);
		}

		public virtual bool IsComplex(MemberInfo member)
		{
			return explicitDeclarations.ComplexTypes.Contains(member.GetPropertyOrFieldType()) || explicitDeclarations.ComplexTypeMembers.ContainsMember(member);
		}

		public bool IsVersion(MemberInfo member)
		{
			return explicitDeclarations.VersionProperties.ContainsMember(member) || Patterns.Versions.Match(member);
		}

		public virtual IEnumerable<Type> GetBaseImplementors(Type ancestor)
		{
			// Thoughts : this operation is reflection-expensive because GetFirstImplementorOf and because it iterate the domain-classes
			// The real usage, so far, need :
			// - just the 'single' implementor
			// - just know if there are more then one entity (to use any)
			// TODO: find a way to make it most efficient
			if (ancestor == null)
			{
				return Enumerable.Empty<Type>();
			}
			IEnumerable<Type> result;
			if (!polymorphismSolutions.TryGetValue(ancestor, out result))
			{
				polymorphismSolutions[ancestor] = result = GetBaseImplementorsPerHierarchy(ancestor);
			}
			return result;
		}

		private IEnumerable<Type> GetBaseImplementorsPerHierarchy(Type ancestor)
		{
			IEnumerable<Type> implementorsPerEachDomainClass =
				explicitDeclarations.DomainClasses.Select(type => type.GetFirstImplementorOf(ancestor)).Where(implementor => implementor != null && !IsExplicitlyExcluded(implementor));

			var implementorsPerEachDomainClassSet = new HashSet<Type>(implementorsPerEachDomainClass);

			var inherited =
				implementorsPerEachDomainClassSet.SelectMany(implementor => implementorsPerEachDomainClassSet.Where(t => !t.Equals(implementor) && implementor.IsAssignableFrom(t))).ToArray();

			implementorsPerEachDomainClassSet.ExceptWith(inherited);
			return implementorsPerEachDomainClassSet;
		}

		public virtual void AddToDomain(Type domainClass)
		{
			if (domainClass == null)
			{
				return;
			}

			if (explicitDeclarations.DomainClasses.Add(domainClass))
			{
				InvalidatePolymorphismSolutions();
			}
		}

		public virtual void AddToDomain(IEnumerable<Type> domainClasses)
		{
			foreach (var domainClass in domainClasses)
			{
				AddToDomain(domainClass);
			}
		}

		protected void InvalidatePolymorphismSolutions()
		{
			polymorphismSolutions.Clear();
		}

		public virtual bool IsEntity(Type type)
		{
			return !IsExplicitlyExcluded(type) && (explicitDeclarations.RootEntities.Contains(type)
			       || type.GetBaseTypes().Any(t => explicitDeclarations.RootEntities.Contains(t)));
		}

		private bool IsExplicitlyExcluded(Type type)
		{
			return explicitDeclarations.ClassExclusions.Contains(type) || (type.IsGenericType && explicitDeclarations.ClassExclusions.Contains(type.GetGenericTypeDefinition()));
		}

		public virtual bool IsTablePerClass(Type type)
		{
			return IsMappedFor(explicitDeclarations.TablePerClassEntities, type) && !IsExplicitlyExcluded(type);
		}

		public virtual bool IsTablePerClassHierarchy(Type type)
		{
			return IsMappedFor(explicitDeclarations.TablePerClassHierarchyEntities, type) && !IsExplicitlyExcluded(type);
		}

		protected bool IsMappedFor(ICollection<Type> explicitMappedEntities, Type type)
		{
			bool isExplicitMapped = explicitMappedEntities.Contains(type);
			bool isDerived = false;

			if (!isExplicitMapped)
			{
				isDerived = type.GetBaseTypes().Any(explicitMappedEntities.Contains);
				if (isDerived)
				{
					explicitMappedEntities.Add(type);
				}
			}
			return isExplicitMapped || isDerived;
		}

		public virtual bool IsTablePerConcreteClass(Type type)
		{
			return IsMappedFor(explicitDeclarations.TablePerConcreteClassEntities, type) && !IsExplicitlyExcluded(type);
		}

		public virtual bool IsOneToOne(Type from, Type to)
		{
			var relation = new Relation(from, to);
			return IsEntity(from) && IsEntity(to) && explicitDeclarations.OneToOneRelations.Contains(relation) && !Patterns.ManyToOneRelations.Match(relation);
		}

		public bool IsMasterOneToOne(Type from, Type to)
		{
			Relation relation = explicitDeclarations.OneToOneRelations.SingleOrDefault(oto => oto.Equals(new Relation(from, to)));
			return relation != null && relation.DeclaredAs == Declared.Explicit;
		}

		public virtual bool IsManyToOne(Type from, Type to)
		{
			var relation = new Relation(from, to);
			if(explicitDeclarations.ManyToOneRelations.Contains(relation))
			{
				return true;
			}
			bool areEntities = IsEntity(from) && IsEntity(to);
			bool isFromComponentToEntity = IsComponent(from) && IsEntity(to);
			return isFromComponentToEntity || Patterns.ManyToOneRelations.Match(relation) || (areEntities && !IsOneToOne(from, to));
			//&& !explicitDeclarations.ManyToManyRelations.Contains(relation) cause of CfgORM-5
		}

		public virtual bool IsManyToMany(Type role1, Type role2)
		{
			return explicitDeclarations.ManyToManyRelations.Contains(new Relation(role1, role2));
		}

		public virtual bool IsMasterManyToMany(Type from, Type to)
		{
			return IsManyToMany(from, to)
			       &&
			       explicitDeclarations.ManyToManyRelations.Single(mtm => mtm.Equals(new Relation(from, to))).DeclaredAs
			       == Declared.Explicit;
		}

		public virtual bool IsOneToMany(Type from, Type to)
		{
			var relation = new Relation(from, to);
			if (explicitDeclarations.OneToManyRelations.Contains(relation))
			{
				return true;
			}
			bool areEntities = IsEntity(from) && IsEntity(to);
			bool isFromComponentToEntity = IsComponent(from) && IsEntity(to);
			return !explicitDeclarations.ManyToManyRelations.Contains(relation) &&
						 !IsOneToOne(from, to) && (areEntities || isFromComponentToEntity || Patterns.OneToManyRelations.Match(relation));
		}

		public virtual bool IsHeterogeneousAssociation(MemberInfo member)
		{
			return explicitDeclarations.HeterogeneousAssociations.ContainsMember(member) || Patterns.HeterogeneousAssociations.Match(member);
		}

		public virtual Cascade? ApplyCascade(Type from, MemberInfo on, Type to)
		{
			var relationOn = new RelationOn(from, on, to);

			Cascade resultByClasses;
			var relation = new Relation(from, to);
			if (explicitDeclarations.Cascades.TryGetValue(relation, out resultByClasses))
			{
				// Has Explicit Cascade By Classes
				return resultByClasses;
			}

			return Patterns.Cascades.GetValueOfFirstMatch(relationOn);
		}

		public MemberInfo GetBidirectionalMember(Type from, MemberInfo on, Type to)
		{
			MemberInfo relatedProperty;
			if (explicitDeclarations.BidirectionalMembers.TryGetValue(new RelationOn(from, on, to), out relatedProperty))
			{
				return relatedProperty;
			}
			return null;
		}

		public virtual bool IsPersistentId(MemberInfo member)
		{
			return explicitDeclarations.Poids.ContainsMember(member) || Patterns.Poids.Match(member);
		}

		public virtual IPersistentIdStrategy GetPersistentIdStrategy(MemberInfo member)
		{
			return Patterns.PoidStrategies.GetValueOfFirstMatch(member);
		}

		public virtual bool IsMemberOfNaturalId(MemberInfo member)
		{
			return explicitDeclarations.NaturalIds.ContainsMember(member);
		}

		public virtual bool IsPersistentProperty(MemberInfo role)
		{
			return (!explicitDeclarations.ExclusionProperties.ContainsMember(role) && !Patterns.PersistentPropertiesExclusions.Match(role))
						 || explicitDeclarations.PersistentProperties.ContainsMember(role);
		}

		public virtual bool IsSet(MemberInfo role)
		{
			return (explicitDeclarations.Sets.ContainsMember(role) || Patterns.Sets.Match(role)) &&
				(!explicitDeclarations.Bags.ContainsMember(role) && !explicitDeclarations.Lists.ContainsMember(role)
							&& !explicitDeclarations.Arrays.ContainsMember(role));
		}

		public virtual bool IsBag(MemberInfo role)
		{
			return explicitDeclarations.Bags.ContainsMember(role)
			       ||
						 (!explicitDeclarations.Sets.ContainsMember(role) && !explicitDeclarations.Lists.ContainsMember(role)
							&& !explicitDeclarations.Arrays.ContainsMember(role) && Patterns.Bags.Match(role));
		}

		public virtual bool IsList(MemberInfo role)
		{
			return explicitDeclarations.Lists.ContainsMember(role)
			       ||
						 (!explicitDeclarations.Arrays.ContainsMember(role) && !explicitDeclarations.Bags.ContainsMember(role)
			        && Patterns.Lists.Match(role));
		}

		public virtual bool IsArray(MemberInfo role)
		{
			return explicitDeclarations.Arrays.ContainsMember(role)
			       ||
						 (!explicitDeclarations.Lists.ContainsMember(role) && !explicitDeclarations.Bags.ContainsMember(role)
			        && Patterns.Arrays.Match(role));
		}

		public virtual bool IsDictionary(MemberInfo role)
		{
			return explicitDeclarations.Dictionaries.ContainsMember(role) || Patterns.Dictionaries.Match(role);
		}

		#endregion
	}
}