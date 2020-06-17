// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Linq;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;

// ReSharper disable once CheckNamespace
namespace Microsoft.EntityFrameworkCore
{
    /// <summary>
    ///     Extension methods for <see cref="INavigationBase" />.
    /// </summary>
    public static class NavigationBaseExtensions
    {
        /// <summary>
        ///     <para>
        ///         Calls <see cref="ILazyLoader.SetLoaded"/> for a <see cref="INavigationBase"/> to mark it as loaded
        ///         when a no-tracking query has eagerly loaded this relationship.
        ///     </para>
        ///     <para>
        ///         This method is not exposed as a normal extension method since it is typically used by database providers.
        ///         It is generally not used in application code.
        ///     </para>
        /// </summary>
        /// <param name="navigation"> The navigation loaded. </param>
        /// <param name="entity"> The entity for which the navigation has been loaded. </param>
        public static void SetIsLoadedWhenNoTracking([NotNull] INavigationBase navigation, [NotNull] object entity)
        {
            var serviceProperties = navigation
                .DeclaringEntityType
                .GetDerivedTypesInclusive()
                .Where(t => t.ClrType.IsInstanceOfType(entity))
                .SelectMany(e => e.GetServiceProperties())
                .Where(p => p.ClrType == typeof(ILazyLoader));

            foreach (var serviceProperty in serviceProperties)
            {
                ((ILazyLoader)serviceProperty.GetGetter().GetClrValue(entity))?.SetLoaded(entity, navigation.Name);
            }
        }
    }
}
