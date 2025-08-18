using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore;

namespace EPQ.EF.Repositories
{
    public static class EfIncludeHelper
    {
        public static IQueryable<T> IncludeAll<T>(this IQueryable<T> query, DbContext context) where T : class
        {
            var entityType = context.Model.FindEntityType(typeof(T));
            var navigationPaths = GetNavigationPaths(entityType, new HashSet<IEntityType>());

            foreach (var path in navigationPaths)
                query = query.Include(path);

            return query;
        }

        private static List<string> GetNavigationPaths(IEntityType entityType, HashSet<IEntityType> visited)
        {
            var paths = new List<string>();
            if (entityType == null || visited.Contains(entityType)) return paths;

            visited.Add(entityType);

            foreach (var navigation in entityType.GetNavigations())
            {
                var target = navigation.TargetEntityType;
                var subPaths = GetNavigationPaths(target, visited);
                if (subPaths.Any())
                {
                    foreach (var sp in subPaths)
                        paths.Add($"{navigation.Name}.{sp}");
                }
                else
                {
                    paths.Add(navigation.Name);
                }
            }

            visited.Remove(entityType);
            return paths;
        }
    }
}
