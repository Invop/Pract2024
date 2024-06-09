using Microsoft.EntityFrameworkCore;

namespace ManekiApp.Server.Data
{
    /// <summary>
    /// Class BlankTriggerAddingConvention.
    /// Implements the <see cref="Microsoft.EntityFrameworkCore.Metadata.Conventions.IModelFinalizingConvention" />
    /// </summary>
    /// <seealso cref="Microsoft.EntityFrameworkCore.Metadata.Conventions.IModelFinalizingConvention" />
    public class BlankTriggerAddingConvention : Microsoft.EntityFrameworkCore.Metadata.Conventions.IModelFinalizingConvention
    {
        /// <summary>
        /// Called when a model is being finalized.
        /// </summary>
        /// <param name="modelBuilder">The builder for the model.</param>
        /// <param name="context">Additional information associated with convention execution.</param>
        public virtual void ProcessModelFinalizing(
            Microsoft.EntityFrameworkCore.Metadata.Builders.IConventionModelBuilder modelBuilder,
            Microsoft.EntityFrameworkCore.Metadata.Conventions.IConventionContext<Microsoft.EntityFrameworkCore.Metadata.Builders.IConventionModelBuilder> context)
        {
            foreach (var entityType in modelBuilder.Metadata.GetEntityTypes())
            {
                var table = Microsoft.EntityFrameworkCore.Metadata.StoreObjectIdentifier.Create(entityType, Microsoft.EntityFrameworkCore.Metadata.StoreObjectType.Table);
                if (table != null
                    && entityType.GetDeclaredTriggers().All(t => t.GetDatabaseName(table.Value) == null))
                {
                    entityType.Builder.HasTrigger(table.Value.Name + "_Trigger");
                }

                foreach (var fragment in entityType.GetMappingFragments(Microsoft.EntityFrameworkCore.Metadata.StoreObjectType.Table))
                {
                    if (entityType.GetDeclaredTriggers().All(t => t.GetDatabaseName(fragment.StoreObject) == null))
                    {
                        entityType.Builder.HasTrigger(fragment.StoreObject.Name + "_Trigger");
                    }
                }
            }
        }
    }
}
