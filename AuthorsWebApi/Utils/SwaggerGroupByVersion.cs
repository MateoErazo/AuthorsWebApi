using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace AuthorsWebApi.Utils
{
    public class SwaggerGroupByVersion : IControllerModelConvention
    {
        public void Apply(ControllerModel controller)
        {
            string namespaceController = controller.ControllerType.Namespace; //Controllers.V1
            string apiVersion = namespaceController.Split('.').Last().ToLower(); //v1
            controller.ApiExplorer.GroupName = apiVersion;
        }
    }
}
