using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BDO.Domain;
using BDO.Import;
using BDO.Persistence.Config;
using BDO.Persistence.Repo;
using NHibernate.Tool.hbm2ddl;
using NUnit.Framework;

namespace BDO.Tests
{
    public class TestDatabase
    {
        [Test]
        [Explicit]
        public void CreateDatabase()
        {
            //Assert.IsTrue(false, TestContext.CurrentContext.TestDirectory);
            throw new InvalidOperationException();

            Configuration.GetConfiguration().ExposeConfiguration(p =>
            {
                var export = new SchemaExport(p);
                export.Create(true, true);
                export.Execute(false, true, false);
            });
        }

        [Test]
        public void CreateSession()
        {
            var session = Configuration.GetSession();
            session.Dispose();
        }

        [Test]
        //[Explicit]
        public void UpdateDatabase()
        {
            var sesion = Configuration.GetSession();
            sesion.Dispose();
            //throw new InvalidOperationException();
            Configuration.GetConfiguration().ExposeConfiguration(Expose);

            sesion = Configuration.GetSession();
            sesion.Dispose();
        }

        void Expose(NHibernate.Cfg.Configuration cfg)
        {
            var logPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "log.txt");
            var logFile = new FileInfo(logPath);
            if (!logFile.Exists)
                logFile.Create().Dispose();

            using (var sw = new StreamWriter(logPath))
            {
                if (cfg == null)
                    Debug.Write("Null");
                else
                    Debug.Write(cfg);
                var update = new SchemaUpdate(cfg);
                update.Execute((str) => sw.WriteLine(str), true);
            }
        }

        [Test]
        public void CreateItem()
        {
            UpdateDatabase();

            var item = new Item();
            item.Name = "Item";
            item.VendorCost = 1;

            using (var repo = new ItemRepository())
            {
                repo.Save(item);
            }

            IEnumerable<Item> allItems;

            using (var repo = new ItemRepository())
            {
                allItems = repo.Get();
            }

            Assert.IsTrue(allItems.Count() == 1);
            Assert.IsTrue(allItems.FirstOrDefault().Name == "Item");
        }

        [Test]
        public void ImportItems()
        {
            //Program.ImportItems();
        }

        [Test]
        public void ImportMarket()
        {
            //Program.ImportMarket();
        }

        [Test]
        public void ImportRecipes()
        {
            //Program.ImportRecipes();
        }
    }
}
