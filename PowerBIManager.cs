using Microsoft.Rest;
using Microsoft.PowerBI.Api;
using Microsoft.PowerBI.Api.Models;
using Microsoft.PowerBI.Api.Models.Credentials;

namespace AzFuncPBIDeploy
{
  class PowerBiManager {
    static string accessToken = TokenManager.GetAccessTokenWithLocalCredentials();
    //static string accessToken = TokenManager.GetAccessTokenWithManagedIdentity();
    private const string urlPowerBiServiceApiRoot = "https://api.powerbi.com/";
    static TokenCredentials tokenCredentials = new TokenCredentials(accessToken, "Bearer");
    static PowerBIClient pbiClient = new PowerBIClient(new Uri(urlPowerBiServiceApiRoot), tokenCredentials);

    public static async Task<IList<Group>> GetWorkspaces() 
    {
      // call user API - get all workspaces in which service principal is a member
      var workspaces = (await pbiClient.Groups.GetGroupsAsync()).Value;

      return workspaces;
    }

    public static async Task<string> GetWorkspaceAsAdmin(string workspaceId) 
    {
      var workspaceName = (await pbiClient.Groups.GetGroupAsAdminAsync(new Guid(workspaceId))).Name;

      return workspaceName;
    }

    private static Dataset GetDataset(Guid WorkspaceId, string DatasetName) {
      var datasets = pbiClient.Datasets.GetDatasetsInGroup(WorkspaceId).Value;
      foreach (var dataset in datasets) {
        if (dataset.Name.Equals(DatasetName)) {
          return dataset;
        }
      }
      return null;
    }

    public static async Task ProcessFileUpload(string wrkspaceId, byte[] pbixBytes, string fileName) 
    {
      Guid workspaceId = new Guid(wrkspaceId);
      string datasetDisplayName = fileName.Substring(0, fileName.Length - ".pbix".Length);
      Stream pbixStream = new MemoryStream(pbixBytes);
      await pbiClient.Imports.PostImportWithFileAsyncInGroup(
              workspaceId, 
              pbixStream, 
              datasetDisplayName, 
              ImportConflictHandlerMode.CreateOrOverwrite);
    }    
  }
}