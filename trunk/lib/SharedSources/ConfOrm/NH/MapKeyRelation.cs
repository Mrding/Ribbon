using System;
using ConfOrm.Mappers;
using NHibernate.Cfg.MappingSchema;

namespace ConfOrm.NH
{
	public class MapKeyRelation: IMapKeyRelation
	{
		private readonly Type dictionaryKeyType;
		private readonly HbmMap mapMapping;
		private readonly HbmMapping mapDoc;
		private MapKeyMapper mapKeyMapper;
		private MapKeyManyToManyMapper mapKeyManyToManyMapper;
		private ComponentMapKeyMapper componentMapKeyMapper;

		public MapKeyRelation(Type dictionaryKeyType, HbmMap mapMapping, HbmMapping mapDoc)
		{
			if (dictionaryKeyType == null)
			{
				throw new ArgumentNullException("dictionaryKeyType");
			}
			if (mapMapping == null)
			{
				throw new ArgumentNullException("mapMapping");
			}
			if (mapDoc == null)
			{
				throw new ArgumentNullException("mapDoc");
			}
			this.dictionaryKeyType = dictionaryKeyType;
			this.mapMapping = mapMapping;
			this.mapDoc = mapDoc;
		}

		public void Element(Action<IMapKeyMapper> mapping)
		{
			if (mapKeyMapper == null)
			{
				var hbm = new HbmMapKey { type = dictionaryKeyType.GetNhTypeName() };
				mapKeyMapper = new MapKeyMapper(hbm);
			}
			mapping(mapKeyMapper);
			mapMapping.Item = mapKeyMapper.MapKeyMapping;
		}

		public void ManyToMany(Action<IMapKeyManyToManyMapper> mapping)
		{
			if (mapKeyManyToManyMapper == null)
			{
				var hbm = new HbmMapKeyManyToMany { @class = dictionaryKeyType.GetShortClassName(mapDoc) };
				mapKeyManyToManyMapper = new MapKeyManyToManyMapper(hbm);
			}
			mapping(mapKeyManyToManyMapper);
			mapMapping.Item = mapKeyManyToManyMapper.MapKeyManyToManyMapping;
		}

		public void Component(Action<IComponentMapKeyMapper> mapping)
		{
			if (componentMapKeyMapper == null)
			{
				var hbm = new HbmCompositeMapKey();
				componentMapKeyMapper = new ComponentMapKeyMapper(dictionaryKeyType, hbm, mapDoc);
			}
			mapping(componentMapKeyMapper);
			mapMapping.Item = componentMapKeyMapper.CompositeMapKeyMapping;
		}
	}
}