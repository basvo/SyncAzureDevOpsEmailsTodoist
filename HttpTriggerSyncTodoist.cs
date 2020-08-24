using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Collections.Generic;
using System.Text;

namespace Basvo.Function
{
    public static class HttpTriggerSyncTodoist
    {
        [FunctionName("HttpTriggerSyncTodoist")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("HttpTriggerSyncTodoist processed a request.");

            string responseMessage = "";

            try
            {
                string name = req.Query["name"];
                string body = req.Query["body"];

                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                dynamic data = JsonConvert.DeserializeObject(requestBody);
                name = name ?? data?.name;
                body = body ?? data?.body;

                log.LogInformation($"Name: {name}");
                log.LogInformation($"Body: {body}");

                var url = GetEnvironmentVariable("TodoistAPI");
                var token = GetEnvironmentVariable("TodoistToken");

                log.LogInformation($"Using Todoist API {url}");
                log.LogInformation($"Using token {token}");

                // Retrieve active tasks from Todoist
                var client = new HttpClient();
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);

                var response = await client.GetAsync(url);
                var content = await response.Content.ReadAsStringAsync();

                var tasks = JsonConvert.DeserializeObject<List<TodoistTask>>(content as string);

                if (body.Contains("state was changed to Closed"))
                {
                    // The body contains information that the task has been completed
                    bool taskFound = false;

                    foreach (var task in tasks)
                    {
                        if (task.Content == name)
                        {
                            taskFound = true;
                            if (task.Completed)
                            {
                                responseMessage = "Task already completed in Todoist";
                            }
                            else
                            {
                                // Complete the existing task
                                var taskId = task.Id;
                                var closeTaskUrl = $"{url}/{task.Id}/close";

                                var contentCloseTask = await client.PostAsync(closeTaskUrl, null);
                                contentCloseTask.EnsureSuccessStatusCode();
                                responseMessage = "Task completed in Todoist";
                            }
                        }
                    }

                    if (!taskFound)
                        responseMessage = "No task found to complete in Todoist";
                }
                else
                {   
                    // Check if the task already exists before creating a new one
                    bool createTask = true;

                    foreach (var task in tasks)
                    {
                        if (task.Content == name)
                        {
                            // Active task found, do not create
                            createTask = false;

                            if (task.Completed == false)
                            {
                                // Active task is still open. Do not touch. 
                                responseMessage = "Task already exists in Todoist";
                            }
                            else
                            {
                                // Task was completed before. It should now be reopened.
                                var taskId = task.Id;
                                var reopenTaskUrl = $"{url}/{task.Id}/reopen";

                                var contentReopenTask = await client.PostAsync(reopenTaskUrl, null);
                                contentReopenTask.EnsureSuccessStatusCode();
                                responseMessage = "Task reopened in Todoist";
                            }
                        }
                    }

                    if (createTask)
                    {
                        // Create a new Task
                        var createTaskContent = new StringContent("{\"content\": \"" + name + "\"}", Encoding.UTF8, "application/json");
                        var createTaskResult = await client.PostAsync(url, createTaskContent);
                        createTaskResult.EnsureSuccessStatusCode();
                        responseMessage = "Task created in Todoist";
                    }
                }
            }
            catch (Exception ex)
            {
                responseMessage = ex.ToString();
            }

            log.LogInformation(responseMessage);
            return new OkObjectResult(responseMessage);
        }

        public static string GetEnvironmentVariable(string name)
        {
            return System.Environment.GetEnvironmentVariable(name, EnvironmentVariableTarget.Process);
        }
    }
}
