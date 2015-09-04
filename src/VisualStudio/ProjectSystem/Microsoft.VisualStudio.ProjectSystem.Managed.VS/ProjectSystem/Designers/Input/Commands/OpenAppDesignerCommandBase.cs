﻿// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.ProjectSystem.Utilities;
using Microsoft.VisualStudio.ProjectSystem.Utilities.Designers;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Threading;

namespace Microsoft.VisualStudio.ProjectSystem.Designers.Input.Commands
{
    internal abstract class OpenAppDesignerCommandBase : SingleNodeProjectCommandBase
    {
        private readonly IUnconfiguredProjectVsServices _projectServices;

        public OpenAppDesignerCommandBase(IUnconfiguredProjectVsServices projectServices)
        {
            Requires.NotNull(projectServices, nameof(projectServices));

            _projectServices = projectServices;
        }

        protected override Task<CommandStatusResult> GetCommandStatusAsync(IProjectTree node, bool focused, string commandText, CommandStatus progressiveStatus)
        {
            // We assume that if the AppDesignerTreeModifier marked an AppDesignerFolder, that we support the Project Designer
            if (node.Capabilities.Contains(ProjectTreeCapabilities.AppDesignerFolder))
            {
                return GetCommandStatusResult.Handled(commandText, CommandStatus.Enabled);
            }

            return GetCommandStatusResult.Unhandled;
        }

        protected override Task<bool> TryHandleCommandAsync(IProjectTree node, bool focused, long commandExecuteOptions, IntPtr variantArgIn, IntPtr variantArgOut)
        {
            if (node.Capabilities.Contains(ProjectTreeCapabilities.AppDesignerFolder))
            {
                Guid projectDesignerGuid = _projectServices.Hierarchy.GetGuidProperty(VsHierarchyPropID.ProjectDesignerEditor);

                IVsWindowFrame windowFrame;
                HResult hr = _projectServices.Project.OpenItemWithSpecific(VSConstants.VSITEMID_ROOT, 0, ref projectDesignerGuid, "", VSConstants.LOGVIEWID_Primary, (IntPtr)(-1), out windowFrame);
                if (hr.Failed)
                    throw hr.Exception;

                hr = windowFrame.Show();
                if (hr.Failed)
                    throw hr.Exception;

                return TaskResult.True;
            }

            return TaskResult.False;
        }
    }
}
