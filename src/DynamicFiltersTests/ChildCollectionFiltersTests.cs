﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using EntityFramework.DynamicFilters;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DynamicFiltersTests
{
#if USE_CSPACE

    [TestClass]
    public class ChildCollectionFiltersTests
    {
        [TestMethod]
        public void ChildCollection_ChildCollectionAnyWithPredicate()
        {
            using (var context = new TestContext())
            {
                var list = context.EntityASet.ToList();

                Assert.IsTrue((list.Count == 3) && (list.All(e => e.ID >= 1 && e.ID <= 3)));
            }
        }

        [TestMethod]
        public void ChildCollection_ChildCollectionAnyNoPredicate()
        {
            using (var context = new TestContext())
            {
                var list = context.EntityBSet.ToList();

                Assert.IsTrue((list.Count == 2) && (list.All(e => e.ID >= 1 && e.ID <= 2)));
            }
        }

        [TestMethod]
        public void ChildCollection_ChildCollectionAnyOnFilteredChild()
        {
            using (var context = new TestContext())
            {
                //  Tests where the child entity has a filter and the main entity has an Any filter on that collection.
                var list = context.EntityCSet.Include(c => c.Children).ToList();

                Assert.IsTrue((list.Count == 2) && (list.All(e => e.ID >= 1 && e.ID <= 2)));
            }
        }

        [TestMethod]
        public void ChildCollection_ChildCollectionAnyPredicateOnFilteredChild()
        {
            using (var context = new TestContext())
            {
                //  Tests where the child entity has a filter and the main entity has an Any filter on that collection.
                var list = context.EntityDSet.Include(d => d.Children).ToList();

                Assert.IsTrue((list.Count == 1) && (list.First().Children.All(e => e.ID >= 1 && e.ID <= 3)));
            }
        }

        [TestMethod]
        public void ChildCollection_ChildCollectionAll()
        {
            using (var context = new TestContext())
            {
                var list = context.EntityESet.ToList();

                Assert.IsTrue((list.Count == 2) && (list.All(e => e.ID >= 1 && e.ID <= 2)));
            }
        }

        [TestMethod]
        public void ChildCollection_ChildCollectionAllOnFilteredChild()
        {
            using (var context = new TestContext())
            {
                //  Tests where the child entity has a filter and the main entity has an All filter on that collection.
                var list = context.EntityFSet.Include(d => d.Children).ToList();

                Assert.IsTrue((list.Count == 1) && (list.First().Children.All(e => e.ID >= 1 && e.ID <= 2)));
            }
        }

        #region Models

        public class EntityBase
        {
            [Key]
            [Required]
            [DatabaseGenerated(DatabaseGeneratedOption.None)]
            public int ID { get; set; }
        }

        public class EntityA : EntityBase
        {
            public ICollection<EntityAChild> Children { get; set; }
        }

        public class EntityAChild : EntityBase
        {
            public int ParentID { get; set; }
            public EntityA Parent { get; set; }

            public int ChildValue { get; set; }
        }

        public class EntityB : EntityBase
        {
            public ICollection<EntityBChild> Children { get; set; }
        }

        public class EntityBChild : EntityBase
        {
            public int ParentID { get; set; }
            public EntityB Parent { get; set; }

            public int ChildValue { get; set; }
        }

        public class EntityC : EntityBase
        {
            public ICollection<EntityCChild> Children { get; set; }
        }

        public class EntityCChild : EntityBase
        {
            public int ParentID { get; set; }
            public EntityC Parent { get; set; }

            public int ChildValue { get; set; }
        }

        public class EntityD : EntityBase
        {
            public ICollection<EntityDChild> Children { get; set; }
        }

        public class EntityDChild : EntityBase
        {
            public int ParentID { get; set; }
            public EntityD Parent { get; set; }

            public int ChildValue { get; set; }
        }

        public class EntityE : EntityBase
        {
            public ICollection<EntityEChild> Children { get; set; }
        }

        public class EntityEChild : EntityBase
        {
            public int ParentID { get; set; }
            public EntityE Parent { get; set; }

            public int ChildValue { get; set; }
        }

        public class EntityF : EntityBase
        {
            public ICollection<EntityFChild> Children { get; set; }
        }

        public class EntityFChild : EntityBase
        {
            public int ParentID { get; set; }
            public EntityF Parent { get; set; }

            public int ChildValue { get; set; }
        }

        #endregion

        #region TestContext

        public class TestContext : TestContextBase<TestContext>, ITestContext
        {
            public DbSet<EntityA> EntityASet { get; set; }
            public DbSet<EntityAChild> EntityAChildSet { get; set; }
            public DbSet<EntityB> EntityBSet { get; set; }
            public DbSet<EntityBChild> EntityBChildSet { get; set; }
            public DbSet<EntityC> EntityCSet { get; set; }
            public DbSet<EntityCChild> EntityCChildSet { get; set; }
            public DbSet<EntityD> EntityDSet { get; set; }
            public DbSet<EntityDChild> EntityDChildSet { get; set; }
            public DbSet<EntityE> EntityESet { get; set; }
            public DbSet<EntityEChild> EntityEChildSet { get; set; }
            public DbSet<EntityF> EntityFSet { get; set; }
            public DbSet<EntityFChild> EntityFChildSet { get; set; }

            protected override void OnModelCreating(DbModelBuilder modelBuilder)
            {
                base.OnModelCreating(modelBuilder);

                modelBuilder.Filter("EntityAFilter", (EntityA a, int id) => a.Children.Any(c => c.ID <= id), () => 3);
                modelBuilder.Filter("EntityBFilter", (EntityB b) => b.Children.Any());

                //  Test where the child entities have a filter and the main entity uses Any() on that collection
                modelBuilder.Filter("EntityCFilter", (EntityC c) => c.Children.Any());
                modelBuilder.Filter("EntityCChildFilter", (EntityCChild c, int id) => c.ID <= id, () => 2);

                //  Test where the child entities have a filter and the main entity uses Any([predicate]) on that collection
                modelBuilder.Filter("EntityDFilter", (EntityD d, int value) => d.Children.Any(c => c.ChildValue == value), () => 2);
                modelBuilder.Filter("EntityDChildFilter", (EntityDChild d, int value) => d.ChildValue <= value, () => 2);

                modelBuilder.Filter("EntityEFilter", (EntityE d, int id) => d.Children.All(e => e.ID <= id), () => 2);

                //  Test where the child entities have a filter and the main entity uses All([predicate]) on that collection
                modelBuilder.Filter("EntityFFilter", (EntityF f, int value) => f.Children.All(c => c.ChildValue == value), () => 1);
                modelBuilder.Filter("EntityFChildFilter", (EntityFChild f, int value) => f.ChildValue <= value, () => 2);

                //  TODO: Count()
                //  TODO: Count([predicate])
                //  TODO: Where([predicate])
                //  TODO: Where([predicate]).Any()
                //modelBuilder.Filter("EntityBFilter", (EntityB b, int count) => b.Children.Count() > count, () => 1);
            }

            public override void Seed()
            {
                for (int i = 1; i <= 5; i++)
                {
                    EntityASet.Add(new EntityA
                    {
                        ID = i,
                        Children = new List<EntityAChild>
                        {
                            new EntityAChild { ID = i }
                        }
                    });

                    var entityB = new EntityB { ID = i };
                    if (i <= 2)
                        entityB.Children = new List<EntityBChild>() { new EntityBChild { ID = i * 10 + 1 } };
                    EntityBSet.Add(entityB);

                    EntityCSet.Add(new EntityC { ID = i, Children = new List<EntityCChild>() { new EntityCChild { ID = i } } });

                    EntityESet.Add(new EntityE { ID = i, Children = new List<EntityEChild>() { new EntityEChild { ID = i } } });
                }

                EntityDSet.Add(new EntityD
                {
                    ID = 1,
                    Children = new List<EntityDChild>
                    {
                        new EntityDChild { ID = 1, ChildValue = 1 },
                        new EntityDChild { ID = 2, ChildValue = 2 },
                        new EntityDChild { ID = 3, ChildValue = 1 },
                        new EntityDChild { ID = 4, ChildValue = 3 },
                        new EntityDChild { ID = 5, ChildValue = 4 },
                    }
                });
                EntityDSet.Add(new EntityD
                {
                    ID = 2,
                    Children = new List<EntityDChild>
                    {
                        new EntityDChild { ID = 6, ChildValue = 1 },
                        new EntityDChild { ID = 7, ChildValue = 3 },
                        new EntityDChild { ID = 8, ChildValue = 1 },
                    }
                });

                EntityFSet.Add(new EntityF
                {
                    ID = 1,
                    Children = new List<EntityFChild>
                    {
                        new EntityFChild { ID = 1, ChildValue = 1 },
                        new EntityFChild { ID = 2, ChildValue = 1 },
                        new EntityFChild { ID = 3, ChildValue = 3 },
                        new EntityFChild { ID = 4, ChildValue = 4 },
                    }
                });
                EntityFSet.Add(new EntityF
                {
                    ID = 2,
                    Children = new List<EntityFChild>
                    {
                        new EntityFChild { ID = 5, ChildValue = 1 },
                        new EntityFChild { ID = 6, ChildValue = 2 },
                        new EntityFChild { ID = 7, ChildValue = 3 },
                    }
                });

                SaveChanges();
            }
        }

        #endregion
    }
#endif
}
