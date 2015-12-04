﻿using System;

namespace Unearth
{
    public interface IWorkflowFactory
    {
        IWorkflow CreateAuthWorkflow(string username, string password, bool useCached);

        IWorkflow CreateUpdateGameWorkflow();

        IWorkflow CreateLaunchGameWorkflow();

        IWorkflow CreatePrereqWorkflow();
    }
}

