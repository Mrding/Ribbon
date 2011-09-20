using System.Linq;
using ConfOrm.Mappers;
using NHibernate.Cfg.MappingSchema;

namespace ConfOrm.NH
{
	public class GeneratorMapper : IGeneratorMapper
	{
		private readonly HbmGenerator generator;

		public GeneratorMapper(HbmGenerator generator)
		{
			this.generator = generator;
		}

		#region Implementation of IGeneratorMapper

		public void Params(object generatorParameters)
		{
			if (generatorParameters == null)
			{
				return;
			}
			generator.param = (from pi in generatorParameters.GetType().GetProperties()
			                   let pname = pi.Name
			                   let pvalue = pi.GetValue(generatorParameters, null)
			                   select
			                   	new HbmParam
			                   		{name = pname, Text = new[] {ReferenceEquals(pvalue, null) ? "null" : pvalue.ToString()}}).
				ToArray();
		}

		#endregion
	}
}