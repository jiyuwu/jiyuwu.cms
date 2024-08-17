using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace JIYUWU.Core.ObjectActionValidator
{
    public class NullObjectModelValidator : IObjectModelValidator
    {

        public void Validate(
           ActionContext actionContext,
           ValidationStateDictionary validationState,
           string prefix,
           object model)
        {
            if (string.IsNullOrEmpty(prefix))
            {
                actionContext.ModelValidator(prefix, model);
                return;
            }
        }
    }
}
