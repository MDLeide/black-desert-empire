using System.IO;
using System.Xml.Schema;
using BDO.Domain;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using NHibernate;
using NHibernate.Tool.hbm2ddl;
using Environment = System.Environment;

namespace BDO.Persistence.Config
{
    public static class Configuration
    {
        private static ISessionFactory _sessionFactory;
        private static FluentConfiguration _config;

        private static ISessionFactory SessionFactory => _sessionFactory ?? (_sessionFactory = CreateSessionFactory());
        private static FluentConfiguration Config => _config ?? (_config = CreateConfiguration());


        public static ISession GetSession()
        {
            return SessionFactory.OpenSession();
        }

        public static FluentConfiguration GetConfiguration()
        {
            return Config;
        }


        private static ISessionFactory CreateSessionFactory()
        {
            return Config.BuildSessionFactory();
        }

        private static FluentConfiguration CreateConfiguration()
        {
            return Fluently.Configure()
                .Database(
                    SQLiteConfiguration.Standard.UsingFile(
                        Path.Combine(System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "bdo.db")))
                        .ExposeConfiguration(cfg => new SchemaUpdate(cfg).Execute(true, true))
                .Mappings(p => p.FluentMappings.AddFromAssemblyOf<Item>());
        }
    }
}
