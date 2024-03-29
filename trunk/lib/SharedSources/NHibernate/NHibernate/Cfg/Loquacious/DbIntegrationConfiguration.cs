using System.Data;
using System.Data.Common;
using NHibernate.Connection;
using NHibernate.Driver;
using NHibernate.AdoNet;
using NHibernate.Exceptions;
using NHibernate.Transaction;

namespace NHibernate.Cfg.Loquacious
{
	internal class DbIntegrationConfiguration : IDbIntegrationConfiguration
	{
		private readonly Configuration configuration;

		public DbIntegrationConfiguration(Configuration configuration)
		{
			this.configuration = configuration;
			Connected = new ConnectionConfiguration(this);
			BatchingQueries = new BatcherConfiguration(this);
			Transactions = new TransactionConfiguration(this);
			CreateCommands = new CommandsConfiguration(this);
			Schema = new DbSchemaIntegrationConfiguration(this);
		}

		public Configuration Configuration
		{
			get { return configuration; }
		}

		#region Implementation of IDbIntegrationConfiguration

		public IDbIntegrationConfiguration Using<TDialect>() where TDialect : Dialect.Dialect
		{
			configuration.SetProperty(Environment.Dialect, typeof(TDialect).AssemblyQualifiedName);
			return this;
		}

		public IDbIntegrationConfiguration DisableKeywordsAutoImport()
		{
			configuration.SetProperty(Environment.Hbm2ddlKeyWords, "none");
			return this;
		}

		public IDbIntegrationConfiguration AutoQuoteKeywords()
		{
			configuration.SetProperty(Environment.Hbm2ddlKeyWords, "auto-quote");
			return this;
		}

		public IDbIntegrationConfiguration LogSqlInConsole()
		{
			configuration.SetProperty(Environment.ShowSql, "true");
			return this;
		}

		public IDbIntegrationConfiguration DisableLogFormatedSql()
		{
			configuration.SetProperty(Environment.FormatSql, "false");
			return this;
		}

		public IConnectionConfiguration Connected { get; private set; }
		public IBatcherConfiguration BatchingQueries { get; private set; }
		public ITransactionConfiguration Transactions { get; private set; }

		public ICommandsConfiguration CreateCommands { get; private set; }

		public IDbSchemaIntegrationConfiguration Schema { get; private set; }

		#endregion
	}

	internal class DbSchemaIntegrationConfiguration : IDbSchemaIntegrationConfiguration
	{
		private readonly DbIntegrationConfiguration dbc;

		public DbSchemaIntegrationConfiguration(DbIntegrationConfiguration dbc)
		{
			this.dbc = dbc;
		}

		#region Implementation of IDbSchemaIntegrationConfiguration

		public IDbIntegrationConfiguration Recreating()
		{
			dbc.Configuration.SetProperty(Environment.Hbm2ddlAuto, SchemaAutoAction.Recreate.ToString());
			return dbc;
		}

		public IDbIntegrationConfiguration Creating()
		{
			dbc.Configuration.SetProperty(Environment.Hbm2ddlAuto, SchemaAutoAction.Create.ToString());
			return dbc;
		}

		public IDbIntegrationConfiguration Updating()
		{
			dbc.Configuration.SetProperty(Environment.Hbm2ddlAuto, SchemaAutoAction.Update.ToString());
			return dbc;
		}

		public IDbIntegrationConfiguration Validating()
		{
			dbc.Configuration.SetProperty(Environment.Hbm2ddlAuto, SchemaAutoAction.Validate.ToString());
			return dbc;
		}

		#endregion
	}

	internal class CommandsConfiguration : ICommandsConfiguration
	{
		private readonly DbIntegrationConfiguration dbc;

		public CommandsConfiguration(DbIntegrationConfiguration dbc)
		{
			this.dbc = dbc;
		}

		#region Implementation of ICommandsConfiguration

		public ICommandsConfiguration Preparing()
		{
			dbc.Configuration.SetProperty(Environment.PrepareSql, "true");
			return this;
		}

		public ICommandsConfiguration WithTimeout(byte seconds)
		{
			dbc.Configuration.SetProperty(Environment.CommandTimeout, seconds.ToString());
			return this;
		}

		public ICommandsConfiguration ConvertingExceptionsThrough<TExceptionConverter>()
			where TExceptionConverter : ISQLExceptionConverter
		{
			dbc.Configuration.SetProperty(Environment.SqlExceptionConverter, typeof(TExceptionConverter).AssemblyQualifiedName);
			return this;
		}

		public ICommandsConfiguration AutoCommentingSql()
		{
			dbc.Configuration.SetProperty(Environment.UseSqlComments, "true");
			return this;
		}

		public IDbIntegrationConfiguration WithHqlToSqlSubstitutions(string csvQuerySubstitutions)
		{
			dbc.Configuration.SetProperty(Environment.QuerySubstitutions, csvQuerySubstitutions);
			return dbc;
		}

		public IDbIntegrationConfiguration WithDefaultHqlToSqlSubstitutions()
		{
			return dbc;
		}

		public ICommandsConfiguration WithMaximumDepthOfOuterJoinFetching(byte maxFetchDepth)
		{
			dbc.Configuration.SetProperty(Environment.MaxFetchDepth, maxFetchDepth.ToString());
			return this;
		}

		#endregion
	}

	internal class TransactionConfiguration : ITransactionConfiguration
	{
		private readonly DbIntegrationConfiguration dbc;

		public TransactionConfiguration(DbIntegrationConfiguration dbc)
		{
			this.dbc = dbc;
		}

		#region Implementation of ITransactionConfiguration

		public IDbIntegrationConfiguration Through<TFactory>() where TFactory : ITransactionFactory
		{
			dbc.Configuration.SetProperty(Environment.TransactionStrategy, typeof(TFactory).AssemblyQualifiedName);
			return dbc;
		}

		#endregion
	}

	internal class BatcherConfiguration : IBatcherConfiguration
	{
		private readonly DbIntegrationConfiguration dbc;

		public BatcherConfiguration(DbIntegrationConfiguration dbc)
		{
			this.dbc = dbc;
		}

		#region Implementation of IBatcherConfiguration

		public IBatcherConfiguration Through<TBatcher>() where TBatcher : IBatcherFactory
		{
			dbc.Configuration.SetProperty(Environment.BatchStrategy, typeof(TBatcher).AssemblyQualifiedName);
			return this;
		}

		public IDbIntegrationConfiguration Each(short batchSize)
		{
			dbc.Configuration.SetProperty(Environment.BatchSize, batchSize.ToString());
			return dbc;
		}

		#endregion
	}

	internal class ConnectionConfiguration : IConnectionConfiguration
	{
		private readonly DbIntegrationConfiguration dbc;

		public ConnectionConfiguration(DbIntegrationConfiguration dbc)
		{
			this.dbc = dbc;
		}

		#region Implementation of IConnectionConfiguration

		public IConnectionConfiguration Through<TProvider>() where TProvider : IConnectionProvider
		{
			dbc.Configuration.SetProperty(Environment.ConnectionProvider, typeof(TProvider).AssemblyQualifiedName);
			return this;
		}

		public IConnectionConfiguration By<TDriver>() where TDriver : IDriver
		{
			dbc.Configuration.SetProperty(Environment.ConnectionDriver, typeof(TDriver).AssemblyQualifiedName);
			return this;
		}

		public IConnectionConfiguration With(IsolationLevel level)
		{
			dbc.Configuration.SetProperty(Environment.Isolation, level.ToString());
			return this;
		}

		public IConnectionConfiguration Releasing(ConnectionReleaseMode releaseMode)
		{
			dbc.Configuration.SetProperty(Environment.ReleaseConnections, ConnectionReleaseModeParser.ToString(releaseMode));
			return this;
		}

		public IDbIntegrationConfiguration Using(string connectionString)
		{
			dbc.Configuration.SetProperty(Environment.ConnectionString, connectionString);
			return dbc;
		}

		public IDbIntegrationConfiguration Using(DbConnectionStringBuilder connectionStringBuilder)
		{
			dbc.Configuration.SetProperty(Environment.ConnectionString, connectionStringBuilder.ConnectionString);
			return dbc;
		}

		public IDbIntegrationConfiguration ByAppConfing(string connectionStringName)
		{
			dbc.Configuration.SetProperty(Environment.ConnectionStringName, connectionStringName);
			return dbc;
		}

		#endregion
	}

	internal class DbIntegrationConfigurationProperties: IDbIntegrationConfigurationProperties
	{
		private readonly Configuration configuration;

		public DbIntegrationConfigurationProperties(Configuration configuration)
		{
			this.configuration = configuration;
		}

		#region Implementation of IDbIntegrationConfigurationProperties

		public void Dialect<TDialect>() where TDialect : Dialect.Dialect
		{
			configuration.SetProperty(Environment.Dialect, typeof(TDialect).AssemblyQualifiedName);
		}

		public Hbm2DDLKeyWords KeywordsAutoImport
		{
			set { configuration.SetProperty(Environment.Hbm2ddlKeyWords, value.ToString()); }
		}

		public bool LogSqlInConsole
		{
			set { configuration.SetProperty(Environment.ShowSql, value.ToString().ToLowerInvariant()); }
		}

		public bool LogFormatedSql
		{
			set { configuration.SetProperty(Environment.FormatSql, value.ToString().ToLowerInvariant()); }
		}

		public void ConnectionProvider<TProvider>() where TProvider : IConnectionProvider
		{
			configuration.SetProperty(Environment.ConnectionProvider, typeof(TProvider).AssemblyQualifiedName);
		}

		public void Driver<TDriver>() where TDriver : IDriver
		{
			configuration.SetProperty(Environment.ConnectionDriver, typeof(TDriver).AssemblyQualifiedName);
		}

		public IsolationLevel IsolationLevel
		{
			set { configuration.SetProperty(Environment.Isolation, value.ToString()); }
		}

		public ConnectionReleaseMode ConnectionReleaseMode
		{
			set { configuration.SetProperty(Environment.ReleaseConnections, ConnectionReleaseModeParser.ToString(value)); }
		}

		public string ConnectionString
		{
			set { configuration.SetProperty(Environment.ConnectionString, value); }
		}

		public string ConnectionStringName
		{
			set { configuration.SetProperty(Environment.ConnectionStringName, value); }
		}

		public void Batcher<TBatcher>() where TBatcher : IBatcherFactory
		{
			configuration.SetProperty(Environment.BatchStrategy, typeof(TBatcher).AssemblyQualifiedName);
		}

		public short BatchSize
		{
			set { configuration.SetProperty(Environment.BatchSize, value.ToString()); }
		}

		public void TransactionFactory<TFactory>() where TFactory : ITransactionFactory
		{
			configuration.SetProperty(Environment.TransactionStrategy, typeof(TFactory).AssemblyQualifiedName);
		}

		public bool PrepareCommands
		{
			set { configuration.SetProperty(Environment.PrepareSql, value.ToString().ToLowerInvariant()); }
		}

		public byte Timeout
		{
			set { configuration.SetProperty(Environment.CommandTimeout, value.ToString()); }
		}

		public void ExceptionConverter<TExceptionConverter>() where TExceptionConverter : ISQLExceptionConverter
		{
			configuration.SetProperty(Environment.SqlExceptionConverter, typeof(TExceptionConverter).AssemblyQualifiedName);
		}

		public bool AutoCommentSql
		{
			set { configuration.SetProperty(Environment.UseSqlComments, value.ToString().ToLowerInvariant()); }
		}

		public string HqlToSqlSubstitutions
		{
			set { configuration.SetProperty(Environment.QuerySubstitutions, value); }
		}

		public byte MaximumDepthOfOuterJoinFetching
		{
			set { configuration.SetProperty(Environment.MaxFetchDepth, value.ToString()); }
		}

		public SchemaAutoAction SchemaAction
		{
			set { configuration.SetProperty(Environment.Hbm2ddlAuto, value.ToString()); }
		}

		#endregion
	}
}