﻿// Licensed to the .NET Foundation under one or more agreements. The .NET Foundation licenses this file to you under the MIT license. See the LICENSE.md file in the project root for more information.

#nullable disable

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows;
using Microsoft.VisualStudio.Shell.TableControl;
using Microsoft.VisualStudio.Utilities;

namespace Microsoft.VisualStudio.ProjectSystem.Tools.TableControl
{
    [Export(typeof(ITableColumnDefinition))]
    [Name(TableColumnNames.Dimensions)]
    internal sealed class DimensionsColumnDefinition : TableColumnDefinitionBase
    {
        public override string Name => TableColumnNames.Dimensions;

        public override string DisplayName => TableControlResources.DimensionsHeaderLabel;

        public override StringComparer Comparer => StringComparer.Ordinal;

        public override double MinWidth => 100.0;

        public override TextWrapping TextWrapping => TextWrapping.NoWrap;

        public override bool TryCreateStringContent(ITableEntryHandle entry, bool truncatedText, bool singleColumnView, out string content)
        {
            if (entry.TryGetValue(TableKeyNames.Dimensions, out var value) && value != null && 
                value is IEnumerable<string> dimensions &&
                dimensions.Any())
            {
                content = dimensions.Aggregate((current, next) => $"{current}|{next}");
                return true;
            }

            content = null;
            return false;
        }
    }
}
