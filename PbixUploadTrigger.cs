using System;
using System.IO;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace AzFuncPBIDeploy
{
    public class PbixUploadTrigger
    {
        private readonly ILogger _logger;

        public PbixUploadTrigger(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<PbixUploadTrigger>();
        }

        [Function("PbixUploadTrigger")]
        public async Task Run([BlobTrigger("samples-workitems/{filename}", 
                        Connection = "AzureWebJobsStorage")]
                        byte[] pbixFileContent, 
                        string filename)
        {
            _logger.LogInformation($"Blob trigger function Processed blob\n Name: {filename}");
             var workspaceId = Environment.GetEnvironmentVariable("WorkspaceId");

             var workspaces = await PowerBiManager.GetWorkspaces();
            _logger.LogInformation($"Workspace Name: {workspaces}");

            await PowerBiManager.ProcessFileUpload(workspaceId, pbixFileContent, filename);            
        }
    }
}
