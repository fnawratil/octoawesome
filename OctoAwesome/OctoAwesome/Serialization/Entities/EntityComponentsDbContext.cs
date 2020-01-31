﻿using OctoAwesome.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OctoAwesome.Serialization.Entities
{
    public sealed class EntityComponentsDbContext
    {
        private readonly IDatabaseProvider databaseProvider;
        private readonly Guid universeGuid;

        public EntityComponentsDbContext(IDatabaseProvider databaseProvider, Guid universe)
        {
            this.databaseProvider = databaseProvider;
            universeGuid = universe;
        }

        public void AddOrUpdate<T>(T value, Entity entity) where T : EntityComponent
        {
            var database = databaseProvider.GetDatabase<IdTag<T>>(universeGuid);
            var tag = new IdTag<T>(entity.Id);
            database.AddOrUpdate(tag, new Value(Serializer.Serialize(value)));
        }

        public EntityComponent Get<T>(Entity entity) where T : EntityComponent, new()
        {
            var database = databaseProvider.GetDatabase<IdTag<T>>(universeGuid);
            var tag = new IdTag<T>(entity.Id);
            return Serializer.Deserialize<T>(database.GetValue(tag).Content);
        }

        public IEnumerable<IdTag<T>> GetAllKeys<T>() where T : EntityComponent
            => databaseProvider.GetDatabase<IdTag<T>>(universeGuid).Keys;

        public void Remove<T>(Entity entity) where T : EntityComponent
        {
            var database = databaseProvider.GetDatabase<IdTag<T>>(universeGuid);
            var tag = new IdTag<T>(entity.Id);
            database.Remove(tag);
        }

    }
}