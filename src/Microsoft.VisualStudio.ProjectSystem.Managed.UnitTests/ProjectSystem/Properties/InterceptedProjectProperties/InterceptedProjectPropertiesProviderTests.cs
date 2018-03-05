﻿using System.Collections.Generic;
using System.Threading.Tasks;

using Xunit;

namespace Microsoft.VisualStudio.ProjectSystem.Properties
{
    [Trait("UnitTest", "ProjectSystem")]
    public class InterceptedProjectPropertiesProviderTests
    {
        private const string MockPropertyName = "MockProperty";

        [Fact]
        public async Task VerifyInterceptedPropertiesProviderAsync()
        {
            var delegatePropertiesMock = IProjectPropertiesFactory
                .MockWithPropertiesAndValues(new Dictionary<string, string>() {
                    { MockPropertyName, "DummyValue" }
                });

            var delegateProperties = delegatePropertiesMock.Object;
            var delegateProvider = IProjectPropertiesProviderFactory.Create(delegateProperties);

            bool getEvaluatedInvoked = false;
            bool getUnevaluatedInvoked = false;
            bool setValueInvoked = false;

            var mockPropertyProvider = IInterceptingPropertyValueProviderFactory.Create(MockPropertyName,
                onGetEvaluatedPropertyValue: (v, p) => { getEvaluatedInvoked = true; return v; },
                onGetUnevaluatedPropertyValue: (v, p) => { getUnevaluatedInvoked = true; return v; },
                onSetPropertyValue: (v, p, d) => { setValueInvoked = true; return v; });
            var project = UnconfiguredProjectFactory.Create();
            var instanceProvider = IProjectInstancePropertiesProviderFactory.Create();

            var interceptedProvider = new ProjectFileInterceptedProjectPropertiesProvider(delegateProvider, instanceProvider, project, new[] { mockPropertyProvider });
            var properties = interceptedProvider.GetProperties("path/to/project.testproj", null, null);

            // Verify interception for GetEvaluatedPropertyValueAsync.
            var propertyValue = await properties.GetEvaluatedPropertyValueAsync(MockPropertyName);
            Assert.True(getEvaluatedInvoked);

            // Verify interception for GetUnevaluatedPropertyValueAsync.
            propertyValue = await properties.GetUnevaluatedPropertyValueAsync(MockPropertyName);
            Assert.True(getUnevaluatedInvoked);

            // Verify interception for SetPropertyValueAsync.
            await properties.SetPropertyValueAsync(MockPropertyName, "NewValue", null);
            Assert.True(setValueInvoked);
        }
    }
}