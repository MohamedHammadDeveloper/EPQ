using Microsoft.EntityFrameworkCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EPQ.EF.Repositories
{
    public static class EfLoadHelpers
    {
        public static async Task LoadNavigationPropertiesRecursiveAsync(DbContext context, object entity, HashSet<object>? visited = null)
        {
            if (entity == null) return;
            visited ??= new HashSet<object>();
            if (visited.Contains(entity)) return;
            visited.Add(entity);

            var entityType = context.Model.FindEntityType(entity.GetType());
            if (entityType == null) return;

            var entry = context.Entry(entity);

            foreach (var navigation in entityType.GetNavigations())
            {
                var navName = navigation.Name;

                if (navigation.IsCollection)
                {
                    var coll = entry.Collection(navName);
                    if (!coll.IsLoaded)
                        await coll.LoadAsync();

                    if (coll.CurrentValue is IEnumerable enumerable)
                    {
                        foreach (var child in enumerable)
                        {
                            if (child != null)
                                await LoadNavigationPropertiesRecursiveAsync(context, child, visited);
                        }
                    }
                }
                else
                {
                    var reference = entry.Reference(navName);
                    if (!reference.IsLoaded)
                        await reference.LoadAsync();

                    var child = reference.CurrentValue;
                    if (child != null)
                        await LoadNavigationPropertiesRecursiveAsync(context, child, visited);
                }
            }
        }
    }
}
