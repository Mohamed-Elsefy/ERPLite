using ERPLite.Data.Constants;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using System.Linq.Expressions;

namespace ERPLite.Data.Extensions
{
    public static class ModelBuilderExtensions
    {
        public static void ApplySoftDeleteQueryFilters(this ModelBuilder modelBuilder)
        {
            foreach (IMutableEntityType entityType in modelBuilder.Model.GetEntityTypes())
            {
                if (typeof(ISoftDeletable)
                    .IsAssignableFrom(entityType.ClrType))
                {
                    var parameter =
                        Expression.Parameter(entityType.ClrType, "e");

                    var property =
                        Expression.Property(
                            parameter,
                            nameof(ISoftDeletable.IsActive));

                    var condition =
                        Expression.Equal(
                            property,
                            Expression.Constant(true));

                    var lambda =
                        Expression.Lambda(
                            condition,
                            parameter);

                    modelBuilder.Entity(entityType.ClrType)
                        .HasQueryFilter(lambda);
                }
            }
        }
    }
}
