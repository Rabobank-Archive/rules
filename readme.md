[![build](https://github.com/azure-devops-compliance/rules/workflows/nuget/badge.svg)](https://github.com/azure-devops-compliance/rules/actions)
[![codecov](https://codecov.io/gh/azure-devops-compliance/rules/branch/master/graph/badge.svg)](https://codecov.io/gh/azure-devops-compliance/rules)

# Azure DevOps Compliance - Rules

This repo is the heart of the azure devops compliance solution containing
the default rules that are used to inspect projects in an organization.

Example rules are:

 * NobodyCanDeleteTheTeamProject
 * NobodyCanDeleteReleases
 * NobodyCanDeleteTheRepository
 * ReleaseBranchesAreProtectedByPolicies
 * etc.
 
## Evaluate 
 
These rules are primarily evaluated in an [azure function](https://github.com/azure-devops-compliance/azure-functions)
and the reports are uploaded into Azure DevOps and accessible via this [extension](https://github.com/azure-devops-compliance/extension).

## Reconcile

Most rules also implement functionality to reconcile [[ rek-*uhn*-sahyl ]](https://www.dictionary.com/browse/reconcile?s=t)
meaning it will bring your project or item into the desired state.

For example, reconciling the `ReleaseBranchesAreProtectedByPolicies` does:

 * Require a minimum number of reviewers policy is created or updated.
 * Minimum number of reviewers is set to at least 2
 * Reset code reviewer votes when there are new changes is enabled.
 * Policy is blocking the PR.