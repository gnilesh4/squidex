﻿// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschränkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using Microsoft.OData;
using Microsoft.OData.UriParser;
using MongoDB.Driver;
using Squidex.Domain.Apps.Core.Schemas;
using Squidex.Infrastructure;

namespace Squidex.Domain.Apps.Entities.MongoDb.Contents.Visitors
{
    public static class FilterBuilder
    {
        private static readonly FilterDefinitionBuilder<MongoContentEntity> Filter = Builders<MongoContentEntity>.Filter;

        public static (FilterDefinition<MongoContentEntity> Filter, bool Last) Build(ODataUriParser query, Schema schema)
        {
            SearchClause search;
            try
            {
                search = query.ParseSearch();
            }
            catch (ODataException ex)
            {
                throw new ValidationException("Query $search clause not valid.", new ValidationError(ex.Message));
            }

            if (search != null)
            {
                return (Filter.Text(SearchTermVisitor.Visit(search.Expression).ToString()), false);
            }

            FilterClause filter;
            try
            {
                filter = query.ParseFilter();
            }
            catch (ODataException ex)
            {
                throw new ValidationException("Query $filter clause not valid.", new ValidationError(ex.Message));
            }

            if (filter != null)
            {
                return (FilterVisitor.Visit(filter.Expression, schema), true);
            }

            return (null, false);
        }
    }
}