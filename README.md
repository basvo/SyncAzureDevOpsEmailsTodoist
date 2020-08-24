# SyncAzureDevOpsEmailsTodoist

A small Azure Function I wrote to sync an Azure Devops on-prem instance with Todoist. This way I can update tasks in Azure DevOps on-prem which are reflected pretty much instantly in Todoist.

The intention is to enable Azure Devops notification emails which can be picked up by a Power Automate workflow. This flow should then call this function with a HTTP Post, forwarding the `subject` and `bodyPreview` variables of the notification email in JSON format.

Example of a HTTP Post the function accepts:

```json
{
  "name": "Task 1337 - Azure DevOps Task",
  "body": "Azure DevOps Task\r\n        Azure DevOps Server\r\nTask 1337 state was changed to Closed\r\n\r\nAzure DevOps Task\r\n\r\nView work item\r\nChanged By      John Doe\r\nState   Closed     Active\r\nReason  Completed     Reactivated\r\nWe sent you this no"
}
```

Based on the body of the email and the state of Todoist the function will create, complete or reopen a task.

The following application settings are needed by the function:

| | | |
|-|-|-|
| TodoistAPI | `https://api.todoist.com/rest/v1/tasks` |
| TodoistToken | `<your Todoist API token>` |