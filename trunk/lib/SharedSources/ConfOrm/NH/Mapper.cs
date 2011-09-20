using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ConfOrm.Mappers;
using ConfOrm.NH.CustomizersImpl;
using ConfOrm.Patterns;
using NHibernate.Cfg.MappingSchema;
using NHibernate.UserTypes;

namespace ConfOrm.NH
{
	public class Mapper
	{
		private readonly IDomainInspector domainInspector;
		private readonly ICustomizersHolder customizerHolder;
		private readonly IPatternsAppliersHolder patternsAppliers;
		private readonly ICandidatePersistentMembersProvider membersProvider;

		public Mapper(IDomainInspector domainInspector)
			: this(
				domainInspector, new CustomizersHolder(), new DefaultPatternsAppliersHolder(domainInspector),
				new DefaultCandidatePersistentMembersProvider()) {}

		public Mapper(IDomainInspector domainInspector, ICustomizersHolder customizerHolder)
			: this(
				domainInspector, customizerHolder, new DefaultPatternsAppliersHolder(domainInspector),
				new DefaultCandidatePersistentMembersProvider()) {}

		public Mapper(IDomainInspector domainInspector, IPatternsAppliersHolder patternsAppliers)
			: this(domainInspector, new CustomizersHolder(), patternsAppliers, new DefaultCandidatePersistentMembersProvider()) {}

		public Mapper(IDomainInspector domainInspector, ICustomizersHolder customizerHolder,
		              IPatternsAppliersHolder patternsAppliers, ICandidatePersistentMembersProvider membersProvider)
		{
			if (domainInspector == null)
			{
				throw new ArgumentNullException("domainInspector");
			}
			if (customizerHolder == null)
			{
				throw new ArgumentNullException("customizerHolder");
			}
			if (patternsAppliers == null)
			{
				throw new ArgumentNullException("patternsAppliers");
			}
			if (membersProvider == null)
			{
				throw new ArgumentNullException("membersProvider");
			}
			this.domainInspector = domainInspector;
			this.customizerHolder = customizerHolder;
			this.patternsAppliers = patternsAppliers;
			this.membersProvider = membersProvider;
		}

		public IDomainInspector DomainInspector
		{
			get { return domainInspector; }
		}

		public void Class<TRootEntity>(Action<IClassMapper<TRootEntity>> customizeAction) where TRootEntity : class
		{
			var customizer = new ClassCustomizer<TRootEntity>(customizerHolder);
			customizeAction(customizer);
		}

		public void Subclass<TEntity>(Action<ISubclassMapper<TEntity>> customizeAction) where TEntity : class
		{
			var customizer = new SubclassCustomizer<TEntity>(customizerHolder);
			customizeAction(customizer);
		}

		public void JoinedSubclass<TEntity>(Action<IJoinedSubclassMapper<TEntity>> customizeAction) where TEntity : class
		{
			var customizer = new JoinedSubclassCustomizer<TEntity>(customizerHolder);
			customizeAction(customizer);
		}

		public void UnionSubclass<TEntity>(Action<IUnionSubclassMapper<TEntity>> customizeAction) where TEntity : class
		{
			var customizer = new UnionSubclassCustomizer<TEntity>(customizerHolder);
			customizeAction(customizer);
		}

		public void Component<TComponent>(Action<IComponentMapper<TComponent>> customizeAction) where TComponent : class
		{
			var customizer = new ComponentCustomizer<TComponent>(customizerHolder);
			customizeAction(customizer);
		}

		public void Customize<TPersistent>(Action<IPersistentClassCustomizer<TPersistent>> customizeAction ) where TPersistent: class
		{
			var customizer = new PersistentClassCustomizer<TPersistent>(customizerHolder);
			customizeAction(customizer);
		}

		public IPatternsAppliersHolder PatternsAppliers
		{
			get { return patternsAppliers; }
		}

		public void AddPoidPattern(Predicate<MemberInfo> matcher, Action<MemberInfo, IIdMapper> applier)
		{
			PatternsAppliers.Poid.Add(new DelegatedAdvancedApplier<MemberInfo, IIdMapper>(matcher, applier));
		}

		public void AddPropertyPattern(Predicate<MemberInfo> matcher, Action<IPropertyMapper> applier)
		{
			PatternsAppliers.Property.Add(new DelegatedApplier<MemberInfo, IPropertyMapper>(matcher, applier));
		}

		public void AddPropertyPattern(Predicate<MemberInfo> matcher, Action<MemberInfo, IPropertyMapper> applier)
		{
			PatternsAppliers.Property.Add(new DelegatedAdvancedApplier<MemberInfo, IPropertyMapper>(matcher, applier));
		}

		public void AddCollectionPattern(Predicate<MemberInfo> matcher, Action<ICollectionPropertiesMapper> applier)
		{
			PatternsAppliers.Collection.Add(new DelegatedApplier<MemberInfo, ICollectionPropertiesMapper>(matcher, applier));
		}

		public void AddCollectionPattern(Predicate<MemberInfo> matcher, Action<MemberInfo, ICollectionPropertiesMapper> applier)
		{
			PatternsAppliers.Collection.Add(new DelegatedAdvancedApplier<MemberInfo, ICollectionPropertiesMapper>(matcher, applier));
		}

		public void AddManyToOnePattern(Predicate<MemberInfo> matcher, Action<IManyToOneMapper> applier)
		{
			PatternsAppliers.ManyToOne.Add(new DelegatedApplier<MemberInfo, IManyToOneMapper>(matcher, applier));
		}

		public void AddManyToOnePattern(Predicate<MemberInfo> matcher, Action<MemberInfo, IManyToOneMapper> applier)
		{
			PatternsAppliers.ManyToOne.Add(new DelegatedAdvancedApplier<MemberInfo, IManyToOneMapper>(matcher, applier));
		}

		public void AddRootClassPattern(Predicate<Type> matcher, Action<Type, IClassAttributesMapper> applier)
		{
			PatternsAppliers.RootClass.Add(new DelegatedAdvancedApplier<Type, IClassAttributesMapper>(matcher, applier));
		}

		public void AddSubclassPattern(Predicate<Type> matcher, Action<Type, ISubclassAttributesMapper> applier)
		{
			PatternsAppliers.Subclass.Add(new DelegatedAdvancedApplier<Type, ISubclassAttributesMapper>(matcher, applier));
		}

		public void AddJoinedSubclassPattern(Predicate<Type> matcher, Action<Type, IJoinedSubclassAttributesMapper> applier)
		{
			PatternsAppliers.JoinedSubclass.Add(new DelegatedAdvancedApplier<Type, IJoinedSubclassAttributesMapper>(matcher, applier));
		}

		public void AddUnionSubclassPattern(Predicate<Type> matcher, Action<Type, IUnionSubclassAttributesMapper> applier)
		{
			PatternsAppliers.UnionSubclass.Add(new DelegatedAdvancedApplier<Type, IUnionSubclassAttributesMapper>(matcher, applier));
		}

		public void AddRootClassPattern(Predicate<Type> matcher, Action<IClassAttributesMapper> applier)
		{
			PatternsAppliers.RootClass.Add(new DelegatedApplier<Type, IClassAttributesMapper>(matcher, applier));
		}

		public void AddSubclassPattern(Predicate<Type> matcher, Action<ISubclassAttributesMapper> applier)
		{
			PatternsAppliers.Subclass.Add(new DelegatedApplier<Type, ISubclassAttributesMapper>(matcher, applier));
		}

		public void AddJoinedSubclassPattern(Predicate<Type> matcher, Action<IJoinedSubclassAttributesMapper> applier)
		{
			PatternsAppliers.JoinedSubclass.Add(new DelegatedApplier<Type, IJoinedSubclassAttributesMapper>(matcher, applier));
		}

		public void AddUnionSubclassPattern(Predicate<Type> matcher, Action<IUnionSubclassAttributesMapper> applier)
		{
			PatternsAppliers.UnionSubclass.Add(new DelegatedApplier<Type, IUnionSubclassAttributesMapper>(matcher, applier));
		}

		public HbmMapping CompileMappingFor(IEnumerable<Type> types)
		{
			if (types == null)
			{
				throw new ArgumentNullException("types");
			}
			var typeToMap = new HashSet<Type>(types);
			DomainInspector.AddToDomain(typeToMap);

			string defaultAssemblyName = null;
			string defaultNamespace = null;
			var firstType = typeToMap.FirstOrDefault();
			if (firstType != null && typeToMap.All(t => t.Assembly.Equals(firstType.Assembly)))
			{
				defaultAssemblyName = firstType.Assembly.GetName().Name;
			}
			if (firstType != null && typeToMap.All(t => t.Namespace.Equals(firstType.Namespace)))
			{
				defaultNamespace = firstType.Namespace;
			}
			var mapping = new HbmMapping {assembly = defaultAssemblyName, @namespace = defaultNamespace};
			foreach (var type in RootClasses(typeToMap))
			{
				AddRootClassMapping(type, mapping);
			}
			foreach (var type in Subclasses(typeToMap))
			{
				AddSubclassMapping(mapping, type);
			}
			return mapping;
		}

		private IEnumerable<Type> Subclasses(IEnumerable<Type> types)
		{
			return types.Where(type => domainInspector.IsEntity(type) && !domainInspector.IsRootEntity(type));
		}

		private IEnumerable<Type> RootClasses(IEnumerable<Type> types)
		{
			return types.Where(type => domainInspector.IsEntity(type) && domainInspector.IsRootEntity(type));
		}

		private void AddSubclassMapping(HbmMapping mapping, Type type)
		{
			if (domainInspector.IsTablePerClassHierarchy(type))
			{
				MapSubclass(type, mapping);
			}
			else if (domainInspector.IsTablePerClass(type))
			{
				MapJoinedSubclass(type, mapping);
			}
			else if (domainInspector.IsTablePerConcreteClass(type))
			{
				MapUnionSubclass(type, mapping);
			}
		}

		private void MapUnionSubclass(Type type, HbmMapping mapping)
		{
			var classMapper = new UnionSubclassMapper(type, mapping);

			IEnumerable<MemberInfo> candidateProperties = null;
			if (!domainInspector.IsEntity(type.BaseType))
			{
				var baseType = GetEntityBaseType(type);
				if (baseType != null)
				{
					classMapper.Extends(baseType);
					candidateProperties = membersProvider.GetSubEntityMembers(type, baseType);
				}
			}
			candidateProperties = candidateProperties ?? membersProvider.GetSubEntityMembers(type, type.BaseType);
			var propertiesToMap =
				candidateProperties.Where(p => domainInspector.IsPersistentProperty(p) && !domainInspector.IsPersistentId(p));

			PatternsAppliers.UnionSubclass.ApplyAllMatchs(type, classMapper);
			customizerHolder.InvokeCustomizers(type, classMapper);
			MapProperties(type, propertiesToMap, classMapper);
		}

		private void MapSubclass(Type type, HbmMapping mapping)
		{
			var classMapper = new SubclassMapper(type, mapping);
			IEnumerable<MemberInfo> candidateProperties = null;
			if (!domainInspector.IsEntity(type.BaseType))
			{
				var baseType = GetEntityBaseType(type);
				if (baseType != null)
				{
					classMapper.Extends(baseType);
					candidateProperties = membersProvider.GetSubEntityMembers(type, baseType);
				}
			}
			candidateProperties = candidateProperties ?? membersProvider.GetSubEntityMembers(type, type.BaseType);
			var propertiesToMap =
				candidateProperties.Where(p => domainInspector.IsPersistentProperty(p) && !domainInspector.IsPersistentId(p));

			PatternsAppliers.Subclass.ApplyAllMatchs(type, classMapper);
			customizerHolder.InvokeCustomizers(type, classMapper);
			MapProperties(type, propertiesToMap, classMapper);
		}

		private void MapJoinedSubclass(Type type, HbmMapping mapping)
		{
			var classMapper = new JoinedSubclassMapper(type, mapping);
			IEnumerable<MemberInfo> candidateProperties = null;
			if (!domainInspector.IsEntity(type.BaseType))
			{
				var baseType = GetEntityBaseType(type);
				if(baseType != null)
				{
					classMapper.Extends(baseType);
					classMapper.Key(km => km.Column(baseType.Name.ToLowerInvariant() + "_key"));
					candidateProperties = membersProvider.GetSubEntityMembers(type, baseType);
				}
			}
			candidateProperties = candidateProperties ?? membersProvider.GetSubEntityMembers(type, type.BaseType);
			var propertiesToMap =
				candidateProperties.Where(p => domainInspector.IsPersistentProperty(p) && !domainInspector.IsPersistentId(p));

			PatternsAppliers.JoinedSubclass.ApplyAllMatchs(type, classMapper);
			customizerHolder.InvokeCustomizers(type, classMapper);
			MapProperties(type, propertiesToMap, classMapper);
		}

		private Type GetEntityBaseType(Type type)
		{
			Type analizingType = type;
			while (analizingType != null && analizingType != typeof (object))
			{
				analizingType = analizingType.BaseType;
				if (domainInspector.IsEntity(analizingType))
				{
					return analizingType;
				}
			}
			return type.GetInterfaces().FirstOrDefault(i => domainInspector.IsEntity(i));
		}

		private void AddRootClassMapping(Type type, HbmMapping mapping)
		{
			var poidPropertyOrField = membersProvider.GetEntityMembersForPoid(type).FirstOrDefault(mi => domainInspector.IsPersistentId(mi));
			var classMapper = new ClassMapper(type, mapping, poidPropertyOrField);
			classMapper.Id(idMapper =>
				{
					var persistentIdStrategy = domainInspector.GetPersistentIdStrategy(poidPropertyOrField);
					if (persistentIdStrategy != null && persistentIdStrategy.Strategy != PoIdStrategy.Assigned)
					{
						idMapper.Generator(GetGenerator(persistentIdStrategy.Strategy), gm =>
						{
							if (persistentIdStrategy.Params != null)
							{
								gm.Params(persistentIdStrategy.Params);
							}
						});
					}
					PatternsAppliers.Poid.ApplyAllMatchs(poidPropertyOrField, idMapper);
				});
			if (domainInspector.IsTablePerClassHierarchy(type))
			{
				classMapper.Discriminator(x => { });
			}
			var persistentProperties =
				membersProvider.GetRootEntityMembers(type).Where(
					p => domainInspector.IsPersistentProperty(p) && !domainInspector.IsPersistentId(p)).ToArray();
			var versionMember = persistentProperties.SingleOrDefault(mi => domainInspector.IsVersion(mi));
			if (versionMember!= null)
			{
				classMapper.Version(versionMember, versionMapper =>
					{
						PatternsAppliers.Version.ApplyAllMatchs(versionMember, versionMapper);
					});
			}
			PatternsAppliers.RootClass.ApplyAllMatchs(type, classMapper);
			InvokeClassCustomizers(type, classMapper);

			var naturalIdPropeties = persistentProperties.Where(mi => domainInspector.IsMemberOfNaturalId(mi)).ToArray();
			if (naturalIdPropeties.Length > 0)
			{
				classMapper.NaturalId(naturalIdMapper =>
					{
						foreach (PropertyInfo property in naturalIdPropeties)
						{
							MapNaturalIdProperties(type, naturalIdMapper, property);
						}
					});
			}

			MapProperties(type, persistentProperties.Where(mi => !domainInspector.IsVersion(mi)).Except(naturalIdPropeties), classMapper);
		}

		private void InvokeClassCustomizers(Type type, IClassAttributesMapper classMapper)
		{
			InvokeAncestorsCustomizers(type.GetInterfaces(), classMapper);
			InvokeAncestorsCustomizers(type.GetHierarchyFromBase(), classMapper);
			customizerHolder.InvokeCustomizers(type, classMapper);
		}

		private void InvokeAncestorsCustomizers(IEnumerable<Type> typeAncestors, IClassAttributesMapper classMapper)
		{
			// only apply the polymorphic mapping for no entities:
			// this is to avoid a possible caos in entity-subclassing:
			// an example of caos is when a base class has a specific TableName and a subclass does not have a specific name (use the default class name).
			// I can remove this "limitation", where required, and delegate to the user the responsibility of his caos.
			foreach (var entityType in typeAncestors.Where(t => !domainInspector.IsEntity(t)))
			{
				customizerHolder.InvokeCustomizers(entityType, classMapper);
			}
		}

		private void MapNaturalIdProperties(Type rootEntityType, INaturalIdMapper naturalIdMapper, PropertyInfo property)
		{
			MemberInfo member = property;
			Type propertyType = property.GetPropertyOrFieldType();
			var memberPath = new PropertyPath(null, member);
			if (domainInspector.IsComplex(member))
			{
				MapProperty(member, memberPath, naturalIdMapper);
			}
			else if (domainInspector.IsHeterogeneousAssociation(member))
			{
				MapAny(member, memberPath, naturalIdMapper);
			}
			else if (domainInspector.IsManyToOne(rootEntityType, propertyType))
			{
				MapManyToOne(member, memberPath, propertyType, naturalIdMapper, rootEntityType);
			}
			else if (domainInspector.IsComponent(propertyType))
			{
				MapComponent(member, memberPath, propertyType, naturalIdMapper, rootEntityType);
			}
			else if (domainInspector.IsOneToOne(rootEntityType, propertyType) || domainInspector.IsSet(property)
			         || domainInspector.IsDictionary(property) || domainInspector.IsArray(property)
			         || domainInspector.IsList(property) || domainInspector.IsBag(property))
			{
				throw new ArgumentOutOfRangeException("property",
				                                      string.Format("The property {0} of {1} can't be part of natural-id.",
				                                                    property.Name, property.DeclaringType));
			}
			else
			{
				MapProperty(member, memberPath, naturalIdMapper);
			}
		}

		private IGeneratorDef GetGenerator(PoIdStrategy strategy)
		{
			switch (strategy)
			{
				case PoIdStrategy.HighLow:
					return Generators.HighLow;
				case PoIdStrategy.Sequence:
					return Generators.Sequence;
				case PoIdStrategy.Guid:
					return Generators.Guid;
				case PoIdStrategy.GuidOptimized:
					return Generators.GuidComb;
				case PoIdStrategy.Identity:
					return Generators.Identity;
				case PoIdStrategy.Native:
					return Generators.Native;
				default:
					throw new ArgumentOutOfRangeException("strategy");
			}
		}

		private void MapProperties(Type propertiesContainerType, IEnumerable<MemberInfo> propertiesToMap,
															 IPropertyContainerMapper propertiesContainer)
		{
			MapProperties(propertiesContainerType, propertiesToMap, propertiesContainer, null);			
		}

		private void MapProperties(Type propertiesContainerType, IEnumerable<MemberInfo> propertiesToMap,
		                           IPropertyContainerMapper propertiesContainer, PropertyPath path)
		{
			foreach (PropertyInfo property in propertiesToMap)
			{
				MemberInfo member = property;
				Type propertyType = property.GetPropertyOrFieldType();
				var memberPath = new PropertyPath(path, member);
				if(domainInspector.IsComplex(member))
				{
					MapProperty(member, memberPath, propertiesContainer);					
				}
				else if (domainInspector.IsHeterogeneousAssociation(member))
				{
					MapAny(member, memberPath, propertiesContainer);
				}
				else if (domainInspector.IsManyToOne(propertiesContainerType, propertyType))
				{
					MapManyToOne(member, memberPath, propertyType, propertiesContainer, propertiesContainerType);
				}
				else if (domainInspector.IsOneToOne(propertiesContainerType, propertyType))
				{
					MapOneToOne(member, memberPath, propertyType, propertiesContainer, propertiesContainerType);
				}
				else if (domainInspector.IsSet(property))
				{
					MapSet(member, memberPath, propertyType, propertiesContainer, propertiesContainerType);
				}
				else if (domainInspector.IsDictionary(property))
				{
					MapDictionary(member, memberPath, propertyType, propertiesContainer, propertiesContainerType);
				}
				else if (domainInspector.IsArray(property))
				{
					throw new NotSupportedException();
				}
				else if (domainInspector.IsList(property))
				{
					MapList(member, memberPath, propertyType, propertiesContainer, propertiesContainerType);
				}
				else if (domainInspector.IsBag(property))
				{
					MapBag(member, memberPath, propertyType, propertiesContainer, propertiesContainerType);
				}
				else if (domainInspector.IsComponent(propertyType))
				{
					MapComponent(member, memberPath, propertyType, propertiesContainer, propertiesContainerType);
				}
				else
				{
					MapProperty(member, memberPath, propertiesContainer);
				}
			}
		}

		private void MapAny(MemberInfo member, PropertyPath memberPath, IBasePlainPropertyContainerMapper propertiesContainer)
		{
			propertiesContainer.Any(member, typeof(int), anyMapper =>
			{
				var poidPropertyOrField =
					membersProvider.GetEntityMembersForPoid(memberPath.GetRootMember().DeclaringType).FirstOrDefault(
						mi => domainInspector.IsPersistentId(mi));

				if (poidPropertyOrField != null)
				{
					anyMapper.IdType(poidPropertyOrField.GetPropertyOrFieldType());
				}
				PatternsAppliers.Any.ApplyAllMatchs(member, anyMapper);
				PatternsAppliers.AnyPath.ApplyAllMatchs(memberPath, anyMapper);
				ForEachMemberPath(member, memberPath, pp => customizerHolder.InvokeCustomizers(pp, anyMapper));
			});
		}

		private void MapProperty(MemberInfo member, PropertyPath propertyPath, IBasePlainPropertyContainerMapper propertiesContainer)
		{
			propertiesContainer.Property(member, propertyMapper =>
				{
					PatternsAppliers.Property.ApplyAllMatchs(member, propertyMapper);
					PatternsAppliers.PropertyPath.ApplyAllMatchs(propertyPath, propertyMapper);
					ForEachMemberPath(member, propertyPath, pp => customizerHolder.InvokeCustomizers(pp, propertyMapper));
				});
		}

		protected void ForEachMemberPath(MemberInfo member, PropertyPath progressivePath, Action<PropertyPath> invoke)
		{
			// To ensure that a customizer is called just once I can't use a set because all customizers
			// needs to be called in a certain sequence starting from the most general (interfaces) to the
			// most specific (on progressivePath).

			// There are three levels of direct-customization of a property of the class under-mapping:
			// 1) at interface level
			// 2) at class implementation level
			// 3) at inherited class implementation level
			// After these three levels we have a special behavior for componets mapping.
			// As example :
			// We three components classes named C1, C2, C3. The C1 has a property of C2 and C2 has a property of C3.
			// Given a class X with a collection or a property of type C1 we can customize C3 starting from:
			// a) C3 itself (the cases 1-2-3 above)
			// b) from C2
			// c) from C1 then from C2
			// d) from the collection in X then C1 then from C2
			// The full-progressive-path of the property is : X.Collection->C1.C2.C3.MyProp
			// I have to execute the customization at each possible level (a,b,c,d).

			var invokedPaths = new HashSet<PropertyPath>();

			// paths on interfaces (note: when a property is the implementation of more then one interface a specific order can't be applied...AFAIK)
			var propertiesOnInterfaces = member.GetPropertyFromInterfaces();
			foreach (var propertyOnInterface in propertiesOnInterfaces)
			{
				var propertyPathInterfaceLevel = new PropertyPath(null, propertyOnInterface);
				invoke(propertyPathInterfaceLevel);
			}

			// path on declaring type
			var propertyPathLevel0 = new PropertyPath(null, member.GetMemberFromDeclaringType());
			// path on reflected type
			var propertyPathLevel1 = new PropertyPath(null, member);

			invoke(propertyPathLevel0); invokedPaths.Add(propertyPathLevel0);

			if(!propertyPathLevel0.Equals(propertyPathLevel1))
			{
				invoke(propertyPathLevel1); invokedPaths.Add(propertyPathLevel1);
			}

			foreach (var propertyPath in progressivePath.InverseProgressivePath())
			{
				if (!invokedPaths.Contains(propertyPath))
				{
					invoke(propertyPath);
					invokedPaths.Add(propertyPath);
				}
			}
		}

		private void MapComponent(MemberInfo member, PropertyPath memberPath, Type propertyType, IBasePlainPropertyContainerMapper propertiesContainer,
		                          Type propertiesContainerType)
		{
			propertiesContainer.Component(member, componentMapper =>
				{
					Type componentType = propertyType;
					IEnumerable<MemberInfo> persistentProperties =
						membersProvider.GetComponentMembers(componentType).Where(p => domainInspector.IsPersistentProperty(p));

					MemberInfo parentReferenceProperty = domainInspector.GetBidirectionalMember(propertiesContainerType, member, componentType) ??
					                                     persistentProperties.FirstOrDefault(pp => pp.GetPropertyOrFieldType() == propertiesContainerType);
					if (parentReferenceProperty != null)
					{
						componentMapper.Parent(parentReferenceProperty,
						                       componentParentMapper =>
						                       patternsAppliers.ComponentParent.ApplyAllMatchs(parentReferenceProperty,
						                                                                       componentParentMapper));
					}
					PatternsAppliers.Component.ApplyAllMatchs(componentType, componentMapper);
					PatternsAppliers.ComponentProperty.ApplyAllMatchs(member, componentMapper);
					PatternsAppliers.ComponentPropertyPath.ApplyAllMatchs(memberPath, componentMapper);
					customizerHolder.InvokeCustomizers(componentType, componentMapper);
					ForEachMemberPath(member, memberPath, pp=> customizerHolder.InvokeCustomizers(pp, componentMapper));

					MapProperties(propertyType, persistentProperties.Where(pi => pi != parentReferenceProperty), componentMapper, memberPath);
				});
		}

		private void MapBag(MemberInfo member, PropertyPath propertyPath, Type propertyType, IPropertyContainerMapper propertiesContainer,
		                    Type propertiesContainerType)
		{
			Type collectionElementType = GetCollectionElementTypeOrThrow(propertiesContainerType, member, propertyType);
			ICollectionElementRelationMapper cert = DetermineCollectionElementRelationType(member, propertyPath, collectionElementType);
			propertiesContainer.Bag(member, collectionPropertiesMapper =>
				{
					cert.MapCollectionProperties(collectionPropertiesMapper);
					PatternsAppliers.Collection.ApplyAllMatchs(member, collectionPropertiesMapper);
					PatternsAppliers.CollectionPath.ApplyAllMatchs(propertyPath, collectionPropertiesMapper);
					PatternsAppliers.Bag.ApplyAllMatchs(member, collectionPropertiesMapper);
					PatternsAppliers.BagPath.ApplyAllMatchs(propertyPath, collectionPropertiesMapper);
					ForEachMemberPath(member, propertyPath, pp => customizerHolder.InvokeCustomizers(pp, collectionPropertiesMapper));
				}, cert.Map);
		}

		private void MapList(MemberInfo member, PropertyPath propertyPath, Type propertyType, IPropertyContainerMapper propertiesContainer,
		                     Type propertiesContainerType)
		{
			Type collectionElementType = GetCollectionElementTypeOrThrow(propertiesContainerType, member, propertyType);
			ICollectionElementRelationMapper cert = DetermineCollectionElementRelationType(member, propertyPath, collectionElementType);
			propertiesContainer.List(member, collectionPropertiesMapper =>
				{
					cert.MapCollectionProperties(collectionPropertiesMapper);
					PatternsAppliers.Collection.ApplyAllMatchs(member, collectionPropertiesMapper);
					PatternsAppliers.CollectionPath.ApplyAllMatchs(propertyPath, collectionPropertiesMapper);
					PatternsAppliers.List.ApplyAllMatchs(member, collectionPropertiesMapper);
					PatternsAppliers.ListPath.ApplyAllMatchs(propertyPath, collectionPropertiesMapper);
					ForEachMemberPath(member, propertyPath, pp => customizerHolder.InvokeCustomizers(pp, collectionPropertiesMapper));
				}, cert.Map);
		}

		private void MapDictionary(MemberInfo member, PropertyPath propertyPath, Type propertyType, IPropertyContainerMapper propertiesContainer,
		                           Type propertiesContainerType)
		{
			Type dictionaryKeyType = propertyType.DetermineDictionaryKeyType();
			if (dictionaryKeyType == null)
			{
				throw new NotSupportedException(string.Format("Can't determine collection element relation (property {0} in {1})",
				                                              member.Name, propertiesContainerType));
			}
			IMapKeyRelationMapper mkrm = DetermineMapKeyRelationType(member, propertyPath, dictionaryKeyType);

			Type dictionaryValueType = propertyType.DetermineDictionaryValueType();
			ICollectionElementRelationMapper cert = DetermineCollectionElementRelationType(member, propertyPath, dictionaryValueType);
			
			propertiesContainer.Map(member, collectionPropertiesMapper =>
				{
					cert.MapCollectionProperties(collectionPropertiesMapper);
					PatternsAppliers.Collection.ApplyAllMatchs(member, collectionPropertiesMapper);
					PatternsAppliers.CollectionPath.ApplyAllMatchs(propertyPath, collectionPropertiesMapper);
					PatternsAppliers.Map.ApplyAllMatchs(member, collectionPropertiesMapper);
					PatternsAppliers.MapPath.ApplyAllMatchs(propertyPath, collectionPropertiesMapper);
					ForEachMemberPath(member, propertyPath, pp => customizerHolder.InvokeCustomizers(pp, collectionPropertiesMapper));
				}, mkrm.Map, cert.Map);
		}

		private void MapSet(MemberInfo member, PropertyPath propertyPath, Type propertyType, IPropertyContainerMapper propertiesContainer,
		                    Type propertiesContainerType)
		{
			Type collectionElementType = GetCollectionElementTypeOrThrow(propertiesContainerType, member, propertyType);
			ICollectionElementRelationMapper cert = DetermineCollectionElementRelationType(member, propertyPath, collectionElementType);
			propertiesContainer.Set(member, collectionPropertiesMapper =>
				{
					cert.MapCollectionProperties(collectionPropertiesMapper);
					PatternsAppliers.Collection.ApplyAllMatchs(member, collectionPropertiesMapper);
					PatternsAppliers.CollectionPath.ApplyAllMatchs(propertyPath, collectionPropertiesMapper);
					PatternsAppliers.Set.ApplyAllMatchs(member, collectionPropertiesMapper);
					PatternsAppliers.SetPath.ApplyAllMatchs(propertyPath, collectionPropertiesMapper);
					ForEachMemberPath(member, propertyPath, pp => customizerHolder.InvokeCustomizers(pp, collectionPropertiesMapper));
				}, cert.Map);
		}

		private void MapOneToOne(MemberInfo member, PropertyPath propertyPath, Type propertyType, IPropertyContainerMapper propertiesContainer,
		                         Type propertiesContainerType)
		{
			propertiesContainer.OneToOne(member, oneToOneMapper =>
				{
					Cascade? cascade = domainInspector.ApplyCascade(propertiesContainerType, member, propertyType);
					oneToOneMapper.Cascade(cascade.GetValueOrDefault(Cascade.None));
					PatternsAppliers.OneToOne.ApplyAllMatchs(member, oneToOneMapper);
					PatternsAppliers.OneToOnePath.ApplyAllMatchs(propertyPath, oneToOneMapper);
					ForEachMemberPath(member, propertyPath, pp => customizerHolder.InvokeCustomizers(pp, oneToOneMapper));
				});
		}

		private void MapManyToOne(MemberInfo member, PropertyPath propertyPath, Type propertyType, IBasePlainPropertyContainerMapper propertiesContainer,
		                          Type propertiesContainerType)
		{
			propertiesContainer.ManyToOne(member, manyToOneMapper =>
				{
					Cascade? cascade = domainInspector.ApplyCascade(propertiesContainerType, member, propertyType);
					manyToOneMapper.Cascade(cascade.GetValueOrDefault(Cascade.None));
					PatternsAppliers.ManyToOne.ApplyAllMatchs(member, manyToOneMapper);
					PatternsAppliers.ManyToOnePath.ApplyAllMatchs(propertyPath, manyToOneMapper);
					ForEachMemberPath(member, propertyPath, pp => customizerHolder.InvokeCustomizers(pp, manyToOneMapper));
				});
		}

		private Type GetCollectionElementTypeOrThrow(Type type, MemberInfo property, Type propertyType)
		{
			Type collectionElementType = propertyType.DetermineCollectionElementType();
			if (collectionElementType == null)
			{
				throw new NotSupportedException(string.Format("Can't determine collection element relation (property {0} in {1})",
				                                              property.Name, type));
			}
			return collectionElementType;
		}

		protected interface IMapKeyRelationMapper
		{
			void Map(IMapKeyRelation relation);
		}

		protected interface ICollectionElementRelationMapper
		{
			void Map(ICollectionElementRelation relation);
			void MapCollectionProperties(ICollectionPropertiesMapper mapped);
		}

		private class ElementRelationMapper : ICollectionElementRelationMapper
		{
			private readonly MemberInfo member;
			private readonly PropertyPath propertyPath;
			private readonly IPatternsAppliersHolder appliers;
			private readonly ICustomizersHolder customizersHolder;

			public ElementRelationMapper(MemberInfo member, PropertyPath propertyPath, IPatternsAppliersHolder appliers, ICustomizersHolder customizersHolder)
			{
				this.member = member;
				this.propertyPath = propertyPath;
				this.appliers = appliers;
				this.customizersHolder = customizersHolder;
			}

			#region Implementation of ICollectionElementRelationMapper

			public void Map(ICollectionElementRelation relation)
			{
				relation.Element(x =>
					{
						appliers.Element.ApplyAllMatchs(member, x);
						appliers.ElementPath.ApplyAllMatchs(propertyPath, x);
						customizersHolder.InvokeCustomizers(propertyPath, x);
					});
			}

			public void MapCollectionProperties(ICollectionPropertiesMapper mapped)
			{
			}

			#endregion
		}

		private class OneToManyRelationMapper : ICollectionElementRelationMapper
		{
			private const BindingFlags FlattenHierarchyBindingFlags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy;
			private readonly MemberInfo member;
			private readonly PropertyPath propertyPath;
			private readonly Type ownerType;
			private readonly Type collectionElementType;
			private readonly IDomainInspector domainInspector;
			private readonly IPatternsAppliersHolder appliers;
			private readonly ICustomizersHolder customizersHolder;

			public OneToManyRelationMapper(MemberInfo member, PropertyPath propertyPath, Type ownerType, Type collectionElementType, IDomainInspector domainInspector, IPatternsAppliersHolder appliers, ICustomizersHolder customizersHolder)
			{
				this.member = member;
				this.propertyPath = propertyPath;
				this.ownerType = ownerType;
				this.collectionElementType = collectionElementType;
				this.domainInspector = domainInspector;
				this.appliers = appliers;
				this.customizersHolder = customizersHolder;
			}

			#region Implementation of ICollectionElementRelationMapper

			public void Map(ICollectionElementRelation relation)
			{
				relation.OneToMany(x =>
					{
						appliers.OneToMany.ApplyAllMatchs(member, x);
						appliers.OneToManyPath.ApplyAllMatchs(propertyPath, x);
						customizersHolder.InvokeCustomizers(propertyPath, x);
					});
			}

			public void MapCollectionProperties(ICollectionPropertiesMapper mapped)
			{
				var parentColumnNameInChild = GetParentColumnNameInChild();
				if (parentColumnNameInChild != null)
				{
					mapped.Key(k => k.Column(parentColumnNameInChild));
				}
				var cascadeToApply = domainInspector.ApplyCascade(ownerType, member, collectionElementType);
				mapped.Cascade(cascadeToApply.GetValueOrDefault(Cascade.None));
			}

			private string GetParentColumnNameInChild()
			{
				MemberInfo propertyInfo = domainInspector.GetBidirectionalMember(ownerType, member, collectionElementType) ??
				                          collectionElementType.GetProperties(FlattenHierarchyBindingFlags).FirstOrDefault(p => p.PropertyType.IsAssignableFrom(ownerType) && domainInspector.IsPersistentProperty(p));
				if (propertyInfo != null)
				{
					return propertyInfo.Name;
				}
				return null;
			}

			#endregion
		}

		private class ManyToManyRelationMapper : ICollectionElementRelationMapper
		{
			private readonly MemberInfo member;
			private readonly PropertyPath propertyPath;
			private readonly Type ownerType;
			private readonly Type collectionElementType;
			private readonly IDomainInspector domainInspector;
			private readonly IPatternsAppliersHolder appliers;
			private readonly ICustomizersHolder customizersHolder;

			public ManyToManyRelationMapper(MemberInfo member, PropertyPath propertyPath, Type ownerType, Type collectionElementType, IDomainInspector domainInspector, IPatternsAppliersHolder appliers, ICustomizersHolder customizersHolder)
			{
				this.member = member;
				this.propertyPath = propertyPath;
				this.ownerType = ownerType;
				this.collectionElementType = collectionElementType;
				this.domainInspector = domainInspector;
				this.appliers = appliers;
				this.customizersHolder = customizersHolder;
			}

			#region Implementation of ICollectionElementRelationMapper

			public void Map(ICollectionElementRelation relation)
			{
				relation.ManyToMany(x =>
					{
						appliers.ManyToMany.ApplyAllMatchs(member, x);
						appliers.ManyToManyPath.ApplyAllMatchs(propertyPath, x);
						customizersHolder.InvokeCustomizers(propertyPath, x);
					});
			}

			public void MapCollectionProperties(ICollectionPropertiesMapper mapped)
			{
				var cascadeToApply = domainInspector.ApplyCascade(ownerType, member, collectionElementType);
				mapped.Cascade(cascadeToApply.GetValueOrDefault(Cascade.None));
			}

			#endregion
		}

		private class ComponentRelationMapper : ICollectionElementRelationMapper
		{
			private readonly MemberInfo collectionMember;
			private readonly Type ownerType;
			private readonly Type componentType;
			private readonly ICandidatePersistentMembersProvider membersProvider;
			private readonly IDomainInspector domainInspector;
			private readonly IPatternsAppliersHolder patternsAppliersHolder;
			private readonly ICustomizersHolder customizersHolder;
			private readonly Mapper mapper;

			public ComponentRelationMapper(MemberInfo collectionMember, Type ownerType, Type componentType, ICandidatePersistentMembersProvider membersProvider, IDomainInspector domainInspector, IPatternsAppliersHolder patternsAppliersHolder, ICustomizersHolder customizersHolder, Mapper mapper)
			{
				this.collectionMember = collectionMember;
				this.ownerType = ownerType;
				this.componentType = componentType;
				this.membersProvider = membersProvider;
				this.domainInspector = domainInspector;
				this.patternsAppliersHolder = patternsAppliersHolder;
				this.customizersHolder = customizersHolder;
				this.mapper = mapper;
			}

			#region Implementation of ICollectionElementRelationMapper

			public void Map(ICollectionElementRelation relation)
			{
				relation.Component(x =>
					{
						var persistentProperties = GetPersistentProperties(componentType);
						MemberInfo parentReferenceProperty = domainInspector.GetBidirectionalMember(ownerType, collectionMember, componentType) ??
						                                     persistentProperties.FirstOrDefault(pp => pp.GetPropertyOrFieldType() == ownerType);
						if (parentReferenceProperty != null)
						{
							x.Parent(parentReferenceProperty,
							         componentParentMapper =>
							         patternsAppliersHolder.ComponentParent.ApplyAllMatchs(parentReferenceProperty,
							                                                               componentParentMapper));
						}
						patternsAppliersHolder.Component.ApplyAllMatchs(componentType, x);
						customizersHolder.InvokeCustomizers(componentType, x);

						PropertyPath propertyPath = new PropertyPath(null, collectionMember);
						MapProperties(componentType, propertyPath, x, persistentProperties.Where(pi => pi != parentReferenceProperty));
					});
			}

			public void MapCollectionProperties(ICollectionPropertiesMapper mapped)
			{
			}

			#endregion

			private IEnumerable<MemberInfo> GetPersistentProperties(Type type)
			{
				var properties = membersProvider.GetComponentMembers(type);
				return properties.Where(p => domainInspector.IsPersistentProperty(p));
			}

			private void MapProperties(Type type, PropertyPath memberPath, IComponentElementMapper propertiesContainer, IEnumerable<MemberInfo> persistentProperties)
			{
				// TODO check PropertyPath behaviour when the component is in a collection
				foreach (var property in persistentProperties)
				{
					var member = property;
					var propertyType = property.GetPropertyOrFieldType();
					var propertyPath = new PropertyPath(memberPath, member);

					if (domainInspector.IsManyToOne(type, propertyType))
					{
						propertiesContainer.ManyToOne(member, manyToOneMapper =>
							{
								patternsAppliersHolder.ManyToOne.ApplyAllMatchs(member, manyToOneMapper);
								patternsAppliersHolder.ManyToOnePath.ApplyAllMatchs(propertyPath, manyToOneMapper);
								mapper.ForEachMemberPath(member, propertyPath, pp => customizersHolder.InvokeCustomizers(pp, manyToOneMapper));
							});
					}
					else if (domainInspector.IsComponent(propertyType))
					{
						propertiesContainer.Component(member, x =>
							{
								// Note: for nested-components the Parent discovering is mandatory (recursive nested-component); 
								// for the same reason you can't have more than one property of the type of the Parent component
								var componentOwnerType = type;
								var componentPropertyType = propertyType;

								var componentProperties = GetPersistentProperties(componentPropertyType);
								var parentReferenceProperty = componentProperties.FirstOrDefault(pp => pp.GetPropertyOrFieldType() == componentOwnerType);
								if (parentReferenceProperty != null)
								{
									x.Parent(parentReferenceProperty,
									         componentParentMapper =>
									         patternsAppliersHolder.ComponentParent.ApplyAllMatchs(parentReferenceProperty,
									                                                               componentParentMapper));
								}
								patternsAppliersHolder.Component.ApplyAllMatchs(componentPropertyType, x);
								patternsAppliersHolder.ComponentProperty.ApplyAllMatchs(member, x);
								patternsAppliersHolder.ComponentPropertyPath.ApplyAllMatchs(propertyPath, x);
								customizersHolder.InvokeCustomizers(componentPropertyType, x);
								mapper.ForEachMemberPath(member, propertyPath, pp => customizersHolder.InvokeCustomizers(pp, x));

								MapProperties(componentPropertyType, propertyPath, x, componentProperties.Where(pi => pi != parentReferenceProperty));
							});
					}
					else
					{
						propertiesContainer.Property(member, propertyMapper =>
							{
								patternsAppliersHolder.Property.ApplyAllMatchs(member, propertyMapper);
								patternsAppliersHolder.PropertyPath.ApplyAllMatchs(propertyPath, propertyMapper);
								mapper.ForEachMemberPath(member, propertyPath, pp => customizersHolder.InvokeCustomizers(pp, propertyMapper));
							});
					}
				}
			}
		}

		protected virtual ICollectionElementRelationMapper DetermineCollectionElementRelationType(MemberInfo property, PropertyPath propertyPath, Type collectionElementType)
		{
			var ownerType = property.ReflectedType;
			if (domainInspector.IsOneToMany(ownerType, collectionElementType))
			{
				return new OneToManyRelationMapper(property, propertyPath, ownerType, collectionElementType, domainInspector, PatternsAppliers, customizerHolder);
			}
			if (domainInspector.IsManyToMany(ownerType, collectionElementType))
			{
				return new ManyToManyRelationMapper(property, propertyPath, ownerType, collectionElementType, domainInspector, PatternsAppliers, customizerHolder);
			}
			if (domainInspector.IsComponent(collectionElementType))
			{
				return new ComponentRelationMapper(property, ownerType, collectionElementType, membersProvider, domainInspector, PatternsAppliers, customizerHolder, this);
			}
			return new ElementRelationMapper(property, propertyPath, PatternsAppliers, customizerHolder);
		}

		private IMapKeyRelationMapper DetermineMapKeyRelationType(MemberInfo member, PropertyPath propertyPath, Type dictionaryKeyType)
		{
			var ownerType = member.ReflectedType;
			if (domainInspector.IsManyToMany(ownerType, dictionaryKeyType) || domainInspector.IsOneToMany(ownerType, dictionaryKeyType))
			{
				// OneToMany is not possible as map-key so we map it as many-to-many instead ignore the case
				return new KeyManyToManyRelationMapper(member, propertyPath, PatternsAppliers, customizerHolder);
			}
			if (domainInspector.IsComponent(dictionaryKeyType))
			{
				return new KeyComponentRelationMapper(dictionaryKeyType, propertyPath, membersProvider, domainInspector, PatternsAppliers, customizerHolder, this);
			}
			return new KeyElementRelationMapper(member, propertyPath, PatternsAppliers, customizerHolder);
		}

		private class KeyElementRelationMapper : IMapKeyRelationMapper
		{
			private readonly MemberInfo member;
			private readonly PropertyPath propertyPath;
			private readonly IPatternsAppliersHolder patternsAppliersHolder;
			private readonly ICustomizersHolder customizersHolder;

			public KeyElementRelationMapper(MemberInfo member, PropertyPath propertyPath, IPatternsAppliersHolder patternsAppliersHolder, ICustomizersHolder customizersHolder)
			{
				this.member = member;
				this.propertyPath = propertyPath;
				this.patternsAppliersHolder = patternsAppliersHolder;
				this.customizersHolder = customizersHolder;
			}

			public void Map(IMapKeyRelation relation)
			{
				relation.Element(x=>
				                 	{
														patternsAppliersHolder.MapKey.ApplyAllMatchs(member, x);
														patternsAppliersHolder.MapKeyPath.ApplyAllMatchs(propertyPath, x);
														customizersHolder.InvokeCustomizers(propertyPath, x);
				                 	});
			}
		}

		private class KeyComponentRelationMapper : IMapKeyRelationMapper
		{
			private readonly Type dictionaryKeyType;
			private readonly PropertyPath propertyPath;
			private readonly ICandidatePersistentMembersProvider membersProvider;
			private readonly IDomainInspector domainInspector;
			private readonly IPatternsAppliersHolder patternsAppliersHolder;
			private readonly ICustomizersHolder customizersHolder;
			private readonly Mapper mapper;

			public KeyComponentRelationMapper(Type dictionaryKeyType, PropertyPath propertyPath, ICandidatePersistentMembersProvider membersProvider, IDomainInspector domainInspector, IPatternsAppliersHolder patternsAppliersHolder, ICustomizersHolder customizersHolder, Mapper mapper)
			{
				this.dictionaryKeyType = dictionaryKeyType;
				this.propertyPath = propertyPath;
				this.membersProvider = membersProvider;
				this.domainInspector = domainInspector;
				this.patternsAppliersHolder = patternsAppliersHolder;
				this.customizersHolder = customizersHolder;
				this.mapper = mapper;
			}

			public void Map(IMapKeyRelation relation)
			{
				relation.Component(x =>
				{
					var persistentProperties = GetPersistentProperties(dictionaryKeyType);

					MapProperties(dictionaryKeyType, x, persistentProperties);
				});
			}

			private IEnumerable<MemberInfo> GetPersistentProperties(Type type)
			{
				var properties = membersProvider.GetComponentMembers(type);
				return properties.Where(p => domainInspector.IsPersistentProperty(p));
			}

			private void MapProperties(Type type, IComponentMapKeyMapper propertiesContainer, IEnumerable<MemberInfo> persistentProperties)
			{
				foreach (var property in persistentProperties)
				{
					var member = property;
					var propertyType = property.GetPropertyOrFieldType();
					if (domainInspector.IsManyToOne(type, propertyType))
					{
						propertiesContainer.ManyToOne(member, manyToOneMapper =>
						                                      	{
																											patternsAppliersHolder.ManyToOne.ApplyAllMatchs(member, manyToOneMapper);
																											var progressivePath = new PropertyPath(propertyPath, member);
																											mapper.ForEachMemberPath(member, progressivePath, pp => customizersHolder.InvokeCustomizers(pp, manyToOneMapper));
						                                      	});
					}
					else
					{
						propertiesContainer.Property(member, propertyMapper =>
						                                     	{
																										patternsAppliersHolder.Property.ApplyAllMatchs(member, propertyMapper);
																										var progressivePath = new PropertyPath(propertyPath, member);
																										mapper.ForEachMemberPath(member, progressivePath, pp => customizersHolder.InvokeCustomizers(pp, propertyMapper));
																									});
					}
				}
			}
		}

		private class KeyManyToManyRelationMapper : IMapKeyRelationMapper
		{
			private readonly MemberInfo member;
			private readonly PropertyPath propertyPath;
			private readonly IPatternsAppliersHolder patternsAppliers;
			private readonly ICustomizersHolder customizersHolder;

			public KeyManyToManyRelationMapper(MemberInfo member, PropertyPath propertyPath, IPatternsAppliersHolder patternsAppliers, ICustomizersHolder customizersHolder)
			{
				this.member = member;
				this.propertyPath = propertyPath;
				this.patternsAppliers = patternsAppliers;
				this.customizersHolder = customizersHolder;
			}

			public void Map(IMapKeyRelation relation)
			{
				relation.ManyToMany(x=>
				                    	{
																patternsAppliers.MapKeyManyToMany.ApplyAllMatchs(member, x);
																patternsAppliers.MapKeyManyToManyPath.ApplyAllMatchs(propertyPath, x);
																customizersHolder.InvokeCustomizers(propertyPath, x);
				                    	});
			}
		}

		public IEnumerable<HbmMapping> CompileMappingForEach(IEnumerable<Type> types)
		{
			if (types == null)
			{
				throw new ArgumentNullException("types");
			}
			var typeToMap = new HashSet<Type>(types);
			DomainInspector.AddToDomain(typeToMap);

			foreach (var type in RootClasses(typeToMap))
			{
				var mapping = new HbmMapping { assembly = type.Assembly.GetName().Name, @namespace = type.Namespace };
				AddRootClassMapping(type, mapping);
				yield return mapping;
			}
			foreach (var type in Subclasses(typeToMap))
			{
				var mapping = new HbmMapping { assembly = type.Assembly.GetName().Name, @namespace = type.Namespace };
				AddSubclassMapping(mapping, type);
				yield return mapping;
			}
		}

		public void TypeDef<TComplex, TUserType>() where TUserType: IUserType
		{
			TypeDef(typeof(TComplex), typeof(TUserType));
		}

		public void TypeDef(Type typeOfComplex, Type typeOfCustomPersistentType)
		{
			AddPropertyPattern(mi => mi.GetPropertyOrFieldType() == typeOfComplex, pm => pm.Type(typeOfCustomPersistentType, null));
			PatternsAppliers.Element.Add(new CustomUserTypeInCollectionElementApplier(typeOfComplex, typeOfCustomPersistentType));
			PatternsAppliers.MapKey.Add(new CustomUserTypeInDictionaryKeyApplier(typeOfComplex, typeOfCustomPersistentType));
		}
	}
}