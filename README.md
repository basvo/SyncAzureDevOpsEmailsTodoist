# SyncAzureDevOpsEmailsTodoist

A small Azure Function I wrote to sync an Azure Devops on-prem instance with Todoist. This way I can update tasks in Azure DevOps on-prem which are reflected pretty much instantly in Todoist.

The intention is to enable Azure Devops notification emails which can be picked up by a Power Automate workflow. This flow should then call this function with a HTTP Post, forwarding the `subject` and `body` variables of the notification email in JSON format.

Example of a HTTP Post the function accepts:

```json
{
  "name": "Task 1337 - Azure DevOps Task",
  "body": "<html><head>\r\n<meta http-equiv=\"Content-Type\" content=\"text/html; charset=utf-8\"><meta content=\"text/html; charset=utf-8\"><meta content=\"IE=edge\"><!--snip--></html>"
}
```

Based on the body of the email and the state of Todoist the function will create, complete or reopen a task.

The following application settings are needed by the function:

| | |
|-|-|
| TodoistAPI | `https://api.todoist.com/rest/v1/tasks` |
| TodoistToken | `<your Todoist API token>` |