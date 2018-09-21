using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Conduit.Api.OperationFilters
{
    public class OptionOperationFilter : IOperationFilter
    {
        public void Apply(Operation operation, OperationFilterContext context)
        {
            if (operation.Parameters == null)
                return;

            foreach (var parameter in operation.Parameters)
            {
                if (parameter.Name.EndsWith(".HasValue") && parameter is NonBodyParameter nonBodyParameter)
                {
                    nonBodyParameter.Name = parameter.Name.Substring(0, parameter.Name.Length - ".HasValue".Length);
                    nonBodyParameter.Type = "string";
                }
            }
        }
    }
}
