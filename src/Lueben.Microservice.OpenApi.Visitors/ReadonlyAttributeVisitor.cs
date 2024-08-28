﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Abstractions;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Visitors;
using Newtonsoft.Json.Serialization;

namespace Lueben.Microservice.OpenApi.Visitors
{
    public class ReadonlyAttributeVisitor : TypeVisitor
    {
        public ReadonlyAttributeVisitor(VisitorCollection visitorCollection) : base(visitorCollection)
        {
        }

        public override bool IsVisitable(Type type)
        {
            return true;
        }

        public override void Visit(IAcceptor acceptor, KeyValuePair<string, Type> type, NamingStrategy namingStrategy, params Attribute[] attributes)
        {
            base.Visit(acceptor, type, namingStrategy, attributes);

            if (!(acceptor is OpenApiSchemaAcceptor instance))
            {
                return;
            }

            if (!instance.Schemas.TryGetValue(type.Key, out var schema))
            {
                return;
            }

            if (schema.ReadOnly)
            {
                return;
            }

            if (!instance.Properties.TryGetValue(type.Key, out var propertyInfo))
            {
                return;
            }

            var readonlyAttr = propertyInfo.GetCustomAttribute<ReadOnlyAttribute>(false);
            schema.ReadOnly = readonlyAttr?.IsReadOnly ?? false;
        }
    }
}